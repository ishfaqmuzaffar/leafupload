using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using LeafUpload.Core.Abstractions;
using LeafUpload.Core.Models;
using LeafUpload.Infrastructure.Auth;
using LeafUpload.Infrastructure.Persistence;
using LeafUpload.Infrastructure.Rules;
using LeafUpload.Infrastructure.ML;
using LeafUpload.Infrastructure.Advisories;
using LeafUpload.Infrastructure.Notifications;
using LeafUpload.Infrastructure.Weather;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Register services (Dependency Injection)
builder.Services.AddSingleton<ILeafRepository, InMemoryLeafRepository>();
builder.Services.AddSingleton<ILeafDiseaseModel, LeafDiseaseModel>();
builder.Services.AddSingleton<ITreatmentAdvisor, SimpleTreatmentAdvisor>();

// Farmer accounts + farms need to survive a redeploy, unlike the in-memory
// diagnosis repository above, so this uses a real (SQLite) database.
builder.Services.AddDbContext<LeafUploadDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("LeafUploadDb") ?? "Data Source=leafupload.db"));

builder.Services.AddScoped<IFarmerRepository, EfFarmerRepository>();
builder.Services.AddScoped<IAdvisoryRepository, EfAdvisoryRepository>();
builder.Services.AddScoped<IDeviceTokenRepository, EfDeviceTokenRepository>();

// Push notifications for weather alerts - degrades to a no-op if the Firebase
// service account credential isn't present (e.g. a fresh clone of the repo),
// same "fail soft" pattern as the Anthropic-backed advisory generator below.
var firebaseCredentialPath = Path.Combine(builder.Environment.ContentRootPath, "firebase-service-account.json");
if (File.Exists(firebaseCredentialPath) && FirebaseApp.DefaultInstance == null)
{
    FirebaseApp.Create(new AppOptions
    {
        Credential = GoogleCredential.FromFile(firebaseCredentialPath),
    });
    builder.Services.AddSingleton<IPushNotificationService, FcmPushNotificationService>();
}
else
{
    builder.Services.AddSingleton<IPushNotificationService, NoOpPushNotificationService>();
}
builder.Services.AddScoped<IFarmerAuthService, FarmerAuthService>();
builder.Services.AddSingleton<IPasswordHasher<Farmer>, PasswordHasher<Farmer>>();
builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();

builder.Services.AddHttpClient("OpenMeteoGeocoding", (sp, c) =>
    c.BaseAddress = new Uri(sp.GetRequiredService<IConfiguration>()["OpenMeteo:GeocodingBaseUrl"]!));
builder.Services.AddHttpClient("OpenMeteoForecast", (sp, c) =>
    c.BaseAddress = new Uri(sp.GetRequiredService<IConfiguration>()["OpenMeteo:ForecastBaseUrl"]!));
builder.Services.AddHttpClient("NominatimReverseGeocoding", c =>
{
    c.BaseAddress = new Uri("https://nominatim.openstreetmap.org/");
    // Nominatim's usage policy requires a descriptive User-Agent identifying the app.
    c.DefaultRequestHeaders.Add("User-Agent", "KrishiMitra-AI/1.0 (crop advisory farm registration)");
});
builder.Services.AddScoped<IWeatherService, OpenMeteoWeatherService>();

builder.Services.AddHttpClient("AnthropicClient", (sp, c) =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();
    c.BaseAddress = new Uri(cfg["Anthropic:BaseUrl"]!);
    // Missing key is fine - the request will just fail with 401 and
    // ClaudeAdvisoryGenerator degrades to its fallback message rather than crash.
    c.DefaultRequestHeaders.Add("x-api-key", cfg["ANTHROPIC_API_KEY"] ?? string.Empty);
    c.DefaultRequestHeaders.Add("anthropic-version", cfg["Anthropic:ApiVersion"]);
});
builder.Services.AddScoped<ClaudeAdvisoryGenerator>();

// Static, no-API-call advisory engine is the active IAdvisoryGenerator for now (no
// Anthropic credit). ClaudeAdvisoryGenerator stays registered above, dormant - swap
// this line to bring it back once the account has credit again.
builder.Services.AddScoped<IAdvisoryGenerator, StaticRuleAdvisoryGenerator>();
builder.Services.AddScoped<LeafUpload.Web.Services.FarmAdvisoryService>();

// Proactively pushes severe-weather alerts to affected farmers instead of only
// checking a farm's forecast when that farmer happens to open the app.
builder.Services.AddHostedService<LeafUpload.Web.Services.WeatherAlertSweepService>();

// Cookie stays the default scheme for the MVC web app (AccountController). JWT bearer
// is added alongside it purely for api/mobile/* clients - controllers opt in explicitly
// via [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)].
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.ExpireTimeSpan = TimeSpan.FromDays(14);
        options.SlidingExpiration = true;
        options.Cookie.Name = "LeafUpload.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
    })
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        var jwtSecretKey = builder.Configuration["Jwt:SecretKey"];
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey ?? string.Empty)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(2),
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages(); // ✅ Enable Razor Pages

// Mobile app + web app clients call the API directly with no auth for now,
// so allow any origin. Lock this down to specific origins once the web
// app's domain is known.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AppClients", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

// ✅ Add Swagger and configure for file uploads
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Leaf Upload API",
        Version = "v1"
    });

    c.MapType<IFormFile>(() => new OpenApiSchema
    {
        Type = "string",
        Format = "binary"
    });
});

var app = builder.Build();

// Auto-apply EF Core migrations on startup so a fresh Coolify deploy against
// an empty mounted volume self-initializes the schema with no manual step.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LeafUploadDbContext>();
    db.Database.Migrate();
}

// Return a predictable JSON error body for unhandled exceptions instead of
// an empty response or an HTML error page, so app clients can always parse
// error bodies the same way as success bodies.
app.UseExceptionHandler(errorApp => errorApp.Run(async context =>
{
    context.Response.ContentType = "application/json";
    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
    await context.Response.WriteAsJsonAsync(new { error = "An unexpected error occurred." });
}));

// Farmer's chosen UI language (en/hi/ur), set via the nav language switcher below
// and stored in a cookie. CurrentUICulture flows through the async call chain, so
// deeper services (e.g. DiseaseKnowledgeBase, SimpleTreatmentAdvisor) pick it up
// without needing the culture threaded through every method signature.
app.Use(async (context, next) =>
{
    var cultureCode = context.Request.Cookies["app_lang"];
    if (cultureCode is "hi" or "ur")
    {
        var culture = new System.Globalization.CultureInfo(cultureCode);
        System.Globalization.CultureInfo.CurrentCulture = culture;
        System.Globalization.CultureInfo.CurrentUICulture = culture;
    }
    await next();
});

app.MapGet("/SetLanguage", (HttpContext context, string culture, string? returnUrl) =>
{
    if (culture is "en" or "hi" or "ur")
    {
        context.Response.Cookies.Append("app_lang", culture, new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddYears(1),
            IsEssential = true,
            SameSite = SameSiteMode.Lax,
        });
    }
    var target = !string.IsNullOrEmpty(returnUrl) && Uri.IsWellFormedUriString(returnUrl, UriKind.Relative) ? returnUrl : "/";
    return Results.Redirect(target);
});

app.UseRouting();
app.UseCors("AppClients");
app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Leaf Upload API v1");
    c.RoutePrefix = "swagger"; // 👈 Swagger now lives at /swagger
});

// Default routes
app.MapControllers();
app.MapDefaultControllerRoute();
app.MapRazorPages();


app.Run();
