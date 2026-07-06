using LeafUpload.Core.Abstractions;
using LeafUpload.Infrastructure.Persistence;
using LeafUpload.Infrastructure.Rules;
using LeafUpload.Infrastructure.ML;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Register services (Dependency Injection)
builder.Services.AddSingleton<ILeafRepository, InMemoryLeafRepository>();
builder.Services.AddSingleton<ILeafDiseaseModel, LeafDiseaseModel>();
builder.Services.AddSingleton<ITreatmentAdvisor, SimpleTreatmentAdvisor>();

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

// Return a predictable JSON error body for unhandled exceptions instead of
// an empty response or an HTML error page, so app clients can always parse
// error bodies the same way as success bodies.
app.UseExceptionHandler(errorApp => errorApp.Run(async context =>
{
    context.Response.ContentType = "application/json";
    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
    await context.Response.WriteAsJsonAsync(new { error = "An unexpected error occurred." });
}));

app.UseCors("AppClients");

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
