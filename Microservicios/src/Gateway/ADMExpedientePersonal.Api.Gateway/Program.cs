var builder = WebApplication.CreateBuilder(args);

// CORS para que React (http://localhost:5173) pueda consumir el gateway.
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

// YARP: reverse proxy configurado en appsettings.json (sección ReverseProxy).
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.UseCors("ReactDev");

// Reenvía las peticiones a los microservicios según las rutas configuradas.
app.MapReverseProxy();

app.Run();
