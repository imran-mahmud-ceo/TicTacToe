using Microsoft.EntityFrameworkCore;
using TicTacToe.Data;

var builder = WebApplication.CreateBuilder(args);

// -----------------------------------------------------------------------------
// 1. DATABASE CONFIGURATION (Added this part)
// -----------------------------------------------------------------------------
// This reads the connection string from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// This configures the app to use SQL Server
builder.Services.AddDbContext<GameContext>(options =>
    options.UseSqlServer(connectionString));

// -----------------------------------------------------------------------------
// 2. Add services to the container
// -----------------------------------------------------------------------------
builder.Services.AddControllersWithViews();

var app = builder.Build();

// -----------------------------------------------------------------------------
// 3. Configure the HTTP request pipeline
// -----------------------------------------------------------------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Changed the default controller from 'Home' to 'Game' so it opens your game list first
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Game}/{action=Index}/{id?}");

app.Run();