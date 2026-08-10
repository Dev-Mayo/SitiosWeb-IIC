var builder = WebApplication.CreateBuilder(args);

// Render asigna el puerto por la variable de entorno PORT.
var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// CORS para que React (local y producción en Render) pueda consumir el gateway.
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactDev", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173",
                "https://localhost:5173",
                builder.Configuration["Cors:AllowedOrigin"] ?? "")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// YARP: reverse proxy configurado en appsettings.json (sección ReverseProxy).
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.UseCors("ReactDev");

// Health check para que Render sepa que el gateway está vivo.
app.MapGet("/", () => Results.Ok("Gateway OK"));

// Reenvía las peticiones a los microservicios según las rutas configuradas.
app.MapReverseProxy();

app.Run();
