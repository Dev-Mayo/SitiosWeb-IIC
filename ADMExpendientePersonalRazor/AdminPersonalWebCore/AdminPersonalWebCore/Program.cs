using AdminPersonalWebCore.Repository;
using AdminPersonalWebCore.Repository.ModuloOferenteRepository;
using AdminPersonalWebCore.Repository;
using AdminPersonalWebCore.Services;
using AdminPersonalWebCore.Services.Abstract.ModuloOferenteAbstractServices;
using AdminPersonalWebCore.Services.ModuloOferenteServices;
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

builder.Services.AddScoped<IDbConnectionFactory, DbConnectionFactory>(); // Inyectar la fábrica de conexiones para que los repositorios puedan usarla

// Repository (DAL)
builder.Services.AddSingleton<UsuarioRepository>(_ => new UsuarioRepository(segConn));
builder.Services.AddSingleton<MenuRepository>(_ => new MenuRepository(segConn));
builder.Services.AddSingleton<BitacoraRepository>(_ => new BitacoraRepository(bitConn));
builder.Services.AddSingleton<AdminUsuarioRepository>(_ => new AdminUsuarioRepository(segConn));
builder.Services.AddSingleton<InstEducativaRepository>(_ => new InstEducativaRepository(genConn));
builder.Services.AddSingleton<RequisitoPuestoRepository>(_ => new RequisitoPuestoRepository(empConn)); //emp3
builder.Services.AddSingleton<AreaRepository>(_ => new AreaRepository(empConn)); // EMP4
builder.Services.AddSingleton<AccionPersonalRepository>(_ => new AccionPersonalRepository(empConn)); // EMP5

builder.Services.AddScoped<OferenteRepository>();
builder.Services.AddScoped<AdminRolRepository>();
builder.Services.AddScoped<PrepAcademicaRepository>();
builder.Services.AddScoped<EntrevistaRepository>();
builder.Services.AddScoped<ExpLaboralRepository>();

builder.Services.AddSingleton<ModuloRepository>(_ => new ModuloRepository(segConn)); // SEG5//SEG5
builder.Services.AddSingleton(new ContratacionRepository(empConn));// EMP1
builder.Services.AddSingleton<CompaniaRepository>(_ => new CompaniaRepository(genConn));
builder.Services.AddSingleton<ConcursoRepository>(_ => new ConcursoRepository(ofeConn));
builder.Services.AddSingleton<PuestoRepository>(_ => new PuestoRepository(empConn));
builder.Services.AddSingleton<ParametroRepository>(_ => new ParametroRepository(genConn));
builder.Services.AddSingleton<UbicacionRepository>(_ => new UbicacionRepository(genConn));




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

builder.Services.AddScoped<IOferenteService, OferenteService>();
builder.Services.AddScoped<IAdminRolService, AdminRolService>();
builder.Services.AddScoped<IPrepAcademicaService, PrepAcademicaService>();
builder.Services.AddScoped<IEntrevistaService, EntrevistaService>();
builder.Services.AddScoped<IExpLaboralService, ExpLaboralService>();


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