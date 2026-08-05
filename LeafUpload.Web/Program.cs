using LeafUpload.Core.Abstractions;
using LeafUpload.Core.Models;
using LeafUpload.Infrastructure.Auth;
using LeafUpload.Infrastructure.Persistence;
using LeafUpload.Infrastructure.Rules;
using LeafUpload.Infrastructure.ML;
using LeafUpload.Infrastructure.Weather;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

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
builder.Services.AddScoped<IFarmerAuthService, FarmerAuthService>();
builder.Services.AddSingleton<IPasswordHasher<Farmer>, PasswordHasher<Farmer>>();

builder.Services.AddHttpClient("OpenMeteoGeocoding", (sp, c) =>
    c.BaseAddress = new Uri(sp.GetRequiredService<IConfiguration>()["OpenMeteo:GeocodingBaseUrl"]!));
builder.Services.AddHttpClient("OpenMeteoForecast", (sp, c) =>
    c.BaseAddress = new Uri(sp.GetRequiredService<IConfiguration>()["OpenMeteo:ForecastBaseUrl"]!));
builder.Services.AddScoped<IWeatherService, OpenMeteoWeatherService>();

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

// 👇 Redirect root URL (/) → Razor UI
app.MapGet("/", () => Results.Redirect("/Home/Index"));

app.Run();
