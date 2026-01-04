using Microsoft.EntityFrameworkCore;
using TicTacToe.Data;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// -----------------------------------------------------------------------------
// 1. DATABASE CONFIGURATION
// -----------------------------------------------------------------------------
var connectionString = builder.Configuration.GetConnectionString("TicTacToeContextConnection")
    ?? builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string not found.");

// FIX: Switch to TicTacToeContext (The one that has both Users AND Games)
builder.Services.AddDbContext<TicTacToeContext>(options =>
    options.UseSqlServer(connectionString));

// -----------------------------------------------------------------------------
// 2. IDENTITY CONFIGURATION
// -----------------------------------------------------------------------------
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
        options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<TicTacToeContext>(); // FIX: Connect Identity to TicTacToeContext

// -----------------------------------------------------------------------------
// 3. Add services to the container
// -----------------------------------------------------------------------------
builder.Services.AddControllersWithViews();

var app = builder.Build();

// -----------------------------------------------------------------------------
// 4. Configure the HTTP request pipeline
// -----------------------------------------------------------------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// -----------------------------------------------------------------------------
// 5. ENABLE SECURITY
// -----------------------------------------------------------------------------
app.UseAuthentication();
app.UseAuthorization();

// -----------------------------------------------------------------------------
// 6. ROUTES
// -----------------------------------------------------------------------------
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Game}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();