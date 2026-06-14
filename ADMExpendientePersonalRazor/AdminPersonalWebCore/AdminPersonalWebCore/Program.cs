using AdminPersonalWebCore.Repository;
using AdminPersonalWebCore.Services;
using Dapper;
using MySql.Data.MySqlClient;

var builder = WebApplication.CreateBuilder(args);

// Razor Pages
builder.Services.AddRazorPages();

// Memory Cache (needed for login attempts)
builder.Services.AddMemoryCache();

// Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(5);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Connection strings
string segConn = builder.Configuration.GetConnectionString("SEG");
string bitConn = builder.Configuration.GetConnectionString("BIT");
string genConn = builder.Configuration.GetConnectionString("GEN");
string ofeConn = builder.Configuration.GetConnectionString("OFE");
string empConn = builder.Configuration.GetConnectionString("EMP");//st

//builder.Services.AddScoped<IDbConnectionFactory, DbConnectionFactory>(); // Inyectar la fábrica de conexiones para que los repositorios puedan usarla

builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();//Inyeccion a la fabrica pero con el singleton para que se mantenga la misma instancia durante toda la aplicación,
                                                                           //lo que es adecuado para una fábrica de conexiones que no tiene estado y
                                                                           //puede ser compartida de manera segura entre múltiples hilos.
                                                                           // Repository (DAL)
builder.Services.AddSingleton<UsuarioRepository>();
builder.Services.AddSingleton<MenuRepository>();
builder.Services.AddSingleton<BitacoraRepository>();
builder.Services.AddSingleton<AdminUsuarioRepository>();
builder.Services.AddSingleton<InstEducativaRepository>();


builder.Services.AddSingleton<RequisitoPuestoRepository>(); //EMP3 ya cambiado con el interface IDb
builder.Services.AddSingleton<AreaRepository>(); // EMP4 ya cambiado con el interface IDb
builder.Services.AddSingleton<AccionPersonalRepository>(); // EMP5 ya cambiado con el interface IDb

builder.Services.AddSingleton<ModuloRepository>();// SEG5 ya cambiado con el interface IDb
builder.Services.AddSingleton<ContratacionRepository>(); // EMP1 ya cambiado con el interface IDb
builder.Services.AddSingleton<CompaniaRepository>(_ => new CompaniaRepository(genConn));
builder.Services.AddSingleton<ConcursoRepository>(_ => new ConcursoRepository(ofeConn));
builder.Services.AddSingleton<PuestoRepository>(_ => new PuestoRepository(empConn));
builder.Services.AddSingleton<ParametroRepository>(_ => new ParametroRepository(genConn));
builder.Services.AddSingleton<UbicacionRepository>(_ => new UbicacionRepository(genConn));

builder.Services.AddSingleton<OferenteRepository>();
builder.Services.AddSingleton<AdminRolRepository>();
builder.Services.AddSingleton<PrepAcademicaRepository>();
builder.Services.AddSingleton<EntrevistaRepository>();
builder.Services.AddSingleton<ExpLaboralRepository>();




// Services (BLL) 
builder.Services.AddSingleton<BitacoraService>();
builder.Services.AddSingleton<AuthService>();
builder.Services.AddSingleton<MenuService>();
builder.Services.AddSingleton<AdminUsuarioService>();
builder.Services.AddSingleton<InstEducativaService>();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<RequisitoPuestoService>();//emp3
builder.Services.AddSingleton<AreaService>(); // EMP4
builder.Services.AddSingleton<AccionPersonalService>(); // EMP5

builder.Services.AddSingleton<OferenteService>();
builder.Services.AddSingleton<AdminRolService>(); //SEG 4
builder.Services.AddSingleton<PrepAcademicaService>();
builder.Services.AddSingleton<EntrevistaService>();
builder.Services.AddSingleton<ExpLaboralService>();


builder.Services.AddSingleton<ModuloService>(); // SEG5
builder.Services.AddSingleton<ContratacionService>();// EMP1


builder.Services.AddSingleton<CompaniaService>();
builder.Services.AddSingleton<ConcursoService>();
builder.Services.AddSingleton<PuestoService>();
builder.Services.AddSingleton<ParametroService>();
builder.Services.AddSingleton<UbicacionService>();
var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();
app.MapRazorPages();

app.MapGet("/", context =>
{
    context.Response.Redirect("/SEG/Login");
    return Task.CompletedTask;
});

app.Run();