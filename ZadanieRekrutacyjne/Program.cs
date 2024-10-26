using System.Reflection;
using FluentMigrator.Runner;
using ZadanieRekrutacyjne;

var builder = WebApplication.CreateBuilder(args);

// Call NHibernateHelper.Initialize directly
NHibernateHelper.Initialize(builder.Configuration);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddFluentMigratorCore() 
    .ConfigureRunner(c =>
    {
        c.AddMySql5() // Use MySQL provider (compatible with MariaDB)
            .WithGlobalConnectionString(builder.Configuration.GetConnectionString("Default"))
            .ScanIn(Assembly.GetExecutingAssembly()).For.All();
    })
    .AddLogging(config => config.AddFluentMigratorConsole());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

using var scope = app.Services.CreateScope();
var migrator = scope.ServiceProvider.GetService<IMigrationRunner>();

if (migrator != null)
{
    migrator.ListMigrations();
    migrator.MigrateUp();
}
else
{
    throw new Exception("Migration fault");
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();