using ADMExpedientePersonal.Api.DetalleOferente;
using ADMExpedientePersonal.Api.DetalleOferente.Services;
using ADMExpedientePersonal.Api.DetalleOferente.Repositories;
using ADMExpedientePersonal.Api.DetalleOferente.Data;
using ADMExpedientePersonal.Api.DetalleOferente.Security;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ADMExpedientePersonal - DetalleOferente",
        Version = "v1",
        Description = "Microservicio de detalle de oferentes."
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

builder.Services.AddSingleton<IDbConnectionFactory, MySqlConnectionFactory>();
builder.Services.AddScoped<DetalleOferenteRepository>();
builder.Services.AddScoped<BitacoraRepository>();
builder.Services.AddScoped<DetalleOferenteService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("ReactDev");
app.UseAuthentication();
app.UseAuthorization();

app.MapDetalleOferenteEndpoints();
app.Run();
