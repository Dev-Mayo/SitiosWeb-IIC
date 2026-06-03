using AdminPersonalWebCore.Repository;
using AdminPersonalWebCore.Services;

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

string empConn = builder.Configuration.GetConnectionString("EMP");//st

// Repository (DAL)
builder.Services.AddSingleton<UsuarioRepository>(_ => new UsuarioRepository(segConn));
builder.Services.AddSingleton<MenuRepository>(_ => new MenuRepository(segConn));
builder.Services.AddSingleton<BitacoraRepository>(_ => new BitacoraRepository(bitConn));
builder.Services.AddSingleton<AdminUsuarioRepository>(_ => new AdminUsuarioRepository(segConn));
builder.Services.AddSingleton<InstEducativaRepository>(_ => new InstEducativaRepository(genConn));
builder.Services.AddSingleton<RequisitoPuestoRepository>(_ => new RequisitoPuestoRepository(empConn)); //emp3

// Services (BLL) 
builder.Services.AddSingleton<BitacoraService>();
builder.Services.AddSingleton<AuthService>();
builder.Services.AddSingleton<MenuService>();
builder.Services.AddSingleton<AdminUsuarioService>();
builder.Services.AddSingleton<InstEducativaService>();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<RequisitoPuestoService>();//emp3

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