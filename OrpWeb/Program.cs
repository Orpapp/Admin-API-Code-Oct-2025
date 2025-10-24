using Errlake.Crosscutting;
using Hangfire;
using IOC.Extensions;
using OrpWeb.Controllers;
using OrpWeb.Hangfire;
using Shared.Common;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
.AddEnvironmentVariables();

var services = builder.Services;
var configuration = builder.Configuration;

builder.Services.Configure<EmailConfigurationKeys>(configuration.GetSection("EmailConfigurationKeys"));//for Email configuration keys
builder.Services.Configure<SiteKeys>(configuration.GetSection("SiteKeys"));
SiteKeys.SitePhysicalPath = configuration["SiteKeys:SitePhysicalPath"];
SiteKeys.SiteUrl = configuration["SiteKeys:SiteUrl"];
SiteKeys.FCMServerKey = configuration["SiteKeys:FCMServerKey"];
SiteKeys.FCMSenderId = configuration["SiteKeys:FCMSenderId"]; 

FirebaseKeys.FCMServerKeyFilePath = configuration["SiteKeys:FCMServerKeyFilePath"];
FirebaseKeys.FCMProjectId = configuration["SiteKeys:FCMProjectId"];
FirebaseKeys.FirebaseMessagingUrl = configuration["SiteKeys:FirebaseMessagingUrl"];


services.AddRazorPages().AddRazorRuntimeCompilation();

services.AddHangfire(configurationh => configurationh
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseSqlServerStorage(configuration.GetConnectionString("OrpDbConnection")));
services.AddHangfireServer();
services.RegisterWebApi(configuration);

services.AddAuthorization(config =>
{
    config.AddPolicy("AuthorisedUser", policy =>
    {
        policy.RequireClaim("UserId");
    });

    config.AddPolicy("AdminRolePolicy", policy =>
    {
        policy.RequireClaim("Device");
        policy.RequireClaim("DeviceType");
        policy.RequireClaim("Offset");
    });

});



// Inside Configure method


services.AddAuthentication("CookiesAuth").AddCookie("CookiesAuth", config =>
{
    config.Cookie.Name = "Identitye.Cookie";
    config.LoginPath = "/Account/login";
    // config.AccessDeniedPath = "/home/Login";
});

services.AddMvc().AddViewLocalization().AddDataAnnotationsLocalization();



services.AddControllersWithViews();
services.AddError();
services.AddSession();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.ConfigureExceptionMiddlewareHandler();


app.UseHttpsRedirection();
app.UseStatusCodePagesWithRedirects("/Error/Error{0}");
app.UseAuthentication();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseSession();
app.UseCookiePolicy();
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    // Optional: Add authorization, if needed
    Authorization = new[] { new HangfireAuthorization() }
});

HangfireUtility.ScheduleJobs();

app.MapHangfireDashboard();

app.MapControllers();
app.MapRazorPages();
app.MapControllerRoute("areaRoute", "{area:exists}/{controller=Account}/{action=login}/{id?}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=login}/{id?}");

app.Run();

