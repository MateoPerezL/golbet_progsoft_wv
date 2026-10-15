using System.Globalization;
using GolBet.Repositories.Data;
using GolBet.Repositories.Implementations;
using GolBet.Repositories.Interfaces;
using GolBet.Services.Implementations;
using GolBet.Services.Interfaces;
using GolBet.Services.Mapping;
using Microsoft.EntityFrameworkCore;

var culture = new CultureInfo("es-CO");

CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

var builder = WebApplication.CreateBuilder(args);

// Servicios MVC
builder.Services.AddControllersWithViews();

// Entity Framework Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositorio genérico
builder.Services.AddScoped(
    typeof(IGenericRepository<>),
    typeof(GenericRepository<>));

// Repositorio específico de partidos
builder.Services.AddScoped<IMatchRepository, MatchRepository>();

// AutoMapper
builder.Services.AddAutoMapper(
    configuration => { },
    typeof(MappingProfile));

// Servicios de la aplicación
builder.Services.AddScoped<IMatchService, MatchService>();
builder.Services.AddScoped<ITeamService, TeamService>();


var app = builder.Build();

// Seeder de la base de datos
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();

    await DbSeeder.SeedAsync(context);
}

// Configuración del pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
