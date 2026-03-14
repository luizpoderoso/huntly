using FastEndpoints;
using FastEndpoints.Security;
using Huntly.Api.Middleware;
using Huntly.Application;
using Huntly.Infra;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddAuthenticationJwtBearer(s => s.SigningKey = builder.Configuration["Jwt:Secret"]!)
    .AddAuthorization()
    .AddFastEndpoints()
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
app.UseFastEndpoints();

app.Run();