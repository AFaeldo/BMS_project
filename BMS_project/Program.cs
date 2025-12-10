using BMS_project.Data;
using BMS_project.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Globalization; // Added for culture configuration

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer(); // Required for Swagger
builder.Services.AddSwaggerGen(); // Add Swagger Generator
builder.Services.AddSession(); // Add Session
builder.Services.AddHttpContextAccessor(); // Add HttpContextAccessor
builder.Services.AddScoped<ISystemLogService, SystemLogService>();
builder.Services.AddScoped<ITermService, TermService>();
builder.Services.AddScoped<IEmailService, EmailService>();

// Configure Cookie Authentication (only once)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

// Configure DbContext with MySQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 36)),
        mySqlOptions => mySqlOptions.EnableRetryOnFailure()
    ));

// Configure localization for currency formatting
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("en-PH"),
        new CultureInfo("fil-PH")
    };

    options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en-PH");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Use localization middleware
app.UseRequestLocalization(); // Added for culture configuration

// Authentication before Authorization
app.UseAuthentication();
app.UseAuthorization();
app.UseSession(); // Enable Session

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    //pattern: "{controller=SuperAdmin}/{action=ManageUsers}/{id?}")
    //pattern: "{controller=BarangaySk}/{action=Dashboard}/{id?}")
    //pattern: "{controller=FederationPresident}/{action=Dashboard}/{id?}")
    .WithStaticAssets();

app.Run();
