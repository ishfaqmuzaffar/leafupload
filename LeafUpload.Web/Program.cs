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
