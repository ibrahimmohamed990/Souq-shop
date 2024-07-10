using Microsoft.EntityFrameworkCore;
using Souq.DataAccessLayer.Context;
using Souq.DataAccessLayer.Implementation;
using Souq.Entities.Repositories;
using Souq.Web.Services.Product;
using Microsoft.AspNetCore.Identity;
using Souq.Entities.Models;
using Souq.Utilities;
using Microsoft.AspNetCore.Identity.UI.Services;
using Souq.DataAccessLayer.Implementation.Identity;
using Souq.Entities.Repositories.Identity;
using Stripe;
using Souq.Web.Middlewares;
using Souq.DataAccessLayer.DbInitializer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddDbContext<IdentityUserDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityUserDbContextConnection"));
});


//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
//    .AddEntityFrameworkStores<IdentityUserDbContext>();

builder.Services.Configure<StripeKeys>(builder.Configuration.GetSection("Stripe"));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromHours(4);
    options.User.RequireUniqueEmail = true;
})
       .AddEntityFrameworkStores<IdentityUserDbContext>()
       .AddDefaultTokenProviders().AddDefaultUI();


var keyDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "keys");
if (!Directory.Exists(keyDirectory))
{
    Directory.CreateDirectory(keyDirectory);
}
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(keyDirectory))
    .SetApplicationName("souq-shop");


builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    //options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(2);
    options.SlidingExpiration = true;
    options.Cookie.IsEssential = true;
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        //options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromDays(2); // Set the cookie expiration to 2 days
        options.SlidingExpiration = true; // Optional: enable sliding expiration
        options.Cookie.IsEssential = true;
        options.LoginPath = "/Identity/Account/Login"; 
    });

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUnitOfWorkForIdentity, UnitOfWorkForIdentity>();
builder.Services.AddScoped<IDbInitializer, DbInitializer>();

builder.Services.AddAutoMapper(typeof(ProductProfile));
builder.Services.AddSingleton<IEmailSender, EmailSender>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
await SeedDatabase();  // to seed the database with admin user and roles 

StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();


//app.UseRoleBasedRedirect();

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Admin}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "Customer",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");


app.Run();


async Task SeedDatabase()
{
    using (var serviceScope = app.Services.CreateScope())
    {
        var context = serviceScope.ServiceProvider.GetRequiredService<IDbInitializer>();
        await context.Initialize();
    }
}

//"ConnectionStrings": {
//  "DefaultConnection": "server=(localdb)\\MSSQLLocalDB; database=SouqAppDB; Trusted_Connection=True;MultipleActiveResultSets=true;",
//  "IdentityUserDbContextConnection": "Server=(localdb)\\mssqllocaldb;Database=IdentityUserDB;Trusted_Connection=True;MultipleActiveResultSets=true;"
//}

//"ConnectionStrings": {
//    "DefaultConnection": "server=(localdb)\\MSSQLLocalDB; database=SouqAppDB2; Trusted_Connection=True;MultipleActiveResultSets=true;",
//    "IdentityUserDbContextConnection": "server=(localdb)\\MSSQLLocalDB; database=SouqAppDB3; Trusted_Connection=True;MultipleActiveResultSets=true;"
//  }