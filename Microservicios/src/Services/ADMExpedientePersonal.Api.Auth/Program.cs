using ADMExpedientePersonal.Api.Auth;
using ADMExpedientePersonal.Api.Auth.Services;
using ADMExpedientePersonal.Api.Auth.Repositories;
using ADMExpedientePersonal.Api.Auth.Data;
using ADMExpedientePersonal.Api.Auth.Security;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ADMExpedientePersonal - Auth",
        Version = "v1",
        Description = "Microservicio de autenticación (emisión y validación de JWT)."
    });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Ingrese el token JWT: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(_ => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer", null, null),
            new List<string>()
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactDev", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173",
                "https://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddMemoryCache();

builder.Services.AddSingleton<IDbConnectionFactory, MySqlConnectionFactory>();
builder.Services.AddScoped<AutenticacionRepository>();
builder.Services.AddScoped<AutenticacionService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("ReactDev");
app.UseAuthentication();
app.UseAuthorization();

app.MapAuthEndpoints();
app.Run();
