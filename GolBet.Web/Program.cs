using System.Globalization;
using GolBet.Entities;
using GolBet.Repositories.Data;
using GolBet.Repositories.Implementations;
using GolBet.Repositories.Interfaces;
using GolBet.Services.Implementations;
using GolBet.Services.Interfaces;
using GolBet.Services.Mapping;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var culture = new CultureInfo("es-CO");

CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

var builder = WebApplication.CreateBuilder(args);

// Servicios MVC
builder.Services.AddControllersWithViews(options =>
{
    var messages = options.ModelBindingMessageProvider;

    messages.SetValueMustBeANumberAccessor(
        fieldName =>
            $"El campo {fieldName} debe ser un número.");

    messages.SetAttemptedValueIsInvalidAccessor(
        (value, fieldName) =>
            $"El valor '{value}' no es válido para {fieldName}.");

    messages.SetMissingBindRequiredValueAccessor(
        fieldName =>
            $"El campo {fieldName} es obligatorio.");
});

// Entity Framework Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString(
            "DefaultConnection")));

// Identity
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 6;

    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireDigit = true;

    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<AppDbContext>();

// Configuración de cookies de Identity
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

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

// Seeder de la base de datos, roles y administrador
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context =
        services.GetRequiredService<AppDbContext>();

    var roleManager =
        services.GetRequiredService<RoleManager<IdentityRole>>();

    var userManager =
        services.GetRequiredService<UserManager<AppUser>>();

    await DbSeeder.SeedAsync(
        context,
        roleManager,
        userManager);
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

// Debe estar antes de UseAuthorization
app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
