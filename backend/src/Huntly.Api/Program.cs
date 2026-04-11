using System.Text.Json.Serialization;
using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using Huntly.Api.Middleware;
using Huntly.Application;
using Huntly.Infra;
using Huntly.Infra.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddAuthenticationJwtBearer(s => s.SigningKey = builder.Configuration["Jwt:Secret"]!)
    .AddAuthorization()
    .AddFastEndpoints()
    .SwaggerDocument(o => o.DocumentSettings = s =>
    {
        s.Title = "Huntly API";
        s.Version = "v1";
        s.AddAuth("Bearer", new NSwag.OpenApiSecurityScheme
        {
            Type = NSwag.OpenApiSecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "Enter your JWT token. Get one from POST /api/auth/login."
        });
    })
    .AddCors(options =>
        options.AddDefaultPolicy(p =>
            p.WithOrigins(builder.Configuration["Cors:Origin"]!)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()));

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseAuthentication();
app.UseMiddleware<UserContextMiddleware>();
app.UseAuthorization();
app.UseCors();
app.UseFastEndpoints(c =>
{
    c.Endpoints.RoutePrefix = "api";
    c.Serializer.Options.Converters.Add(new JsonStringEnumConverter());
});
app.UseSwaggerGen();
app.MapScalarApiReference(options =>
{
    options.WithOpenApiRoutePattern("swagger/v1/swagger.json")
        .WithTitle("Huntly API Reference")
        .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient)
        .AddPreferredSecuritySchemes("Bearer");
});

await ApplyMigrations(app);

app.Run();

return;

async Task ApplyMigrations(WebApplication webApplication)
{
    using var scope = webApplication.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}