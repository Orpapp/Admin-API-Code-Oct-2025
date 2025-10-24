using Api.Helper;
using Asp.Versioning;
using Errlake.Crosscutting;
using IOC.Extensions;
using Microsoft.Extensions.Options;
using Shared.Common;
using Swashbuckle.AspNetCore.SwaggerGen;

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

SiteKeys.IOSInAppSandboxURL = configuration["SiteKeys:IOSInAppSandboxURL"];
SiteKeys.IOSInAppProductionURL = configuration["SiteKeys:IOSInAppProductionURL"];
SiteKeys.IOSInAppSharedSecret = configuration["SiteKeys:IOSInAppSharedSecret"];

SiteKeys.AndroidInAppAccountEmail = configuration["SiteKeys:AndroidInAppAccountEmail"];
SiteKeys.AndroidInAppPackage = configuration["SiteKeys:AndroidInAppPackage"];
SiteKeys.AndroidInAppCertificatePath = configuration["SiteKeys:AndroidInAppCertificatePath"];
SiteKeys.AndroidInAppGoogleApisURL = configuration["SiteKeys:AndroidInAppGoogleApisURL"];
SiteKeys.AndroidInAppCertificatePassword = configuration["SiteKeys:AndroidInAppCertificatePassword"];
SiteKeys.AndroidInAppApplicationName = configuration["SiteKeys:AndroidInAppApplicationName"];





FirebaseKeys.FCMServerKeyFilePath = configuration["SiteKeys:FCMServerKeyFilePath"];
FirebaseKeys.FCMProjectId = configuration["SiteKeys:FCMProjectId"];
FirebaseKeys.FirebaseMessagingUrl = configuration["SiteKeys:FirebaseMessagingUrl"];


services.AddControllers();

services.AddEndpointsApiExplorer();

services.ConfigureModelSettings(configuration);

services.RegisterWebApi(configuration);

services.ConfigureJWTAuthentication(configuration);
services.ConfigureSwagger();
services.ConfigureAuthorization();
services.AddControllersWithViews();
services.AddRazorPages();
services.AddError();

builder.Services.AddApiVersioning(opt =>
{
    opt.DefaultApiVersion = new ApiVersion(1, 0);
    opt.AssumeDefaultVersionWhenUnspecified = true;
    opt.ReportApiVersions = true;
    opt.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader(),
                                                    new HeaderApiVersionReader("x-api-version"),
                                                    new MediaTypeApiVersionReader("x-api-version"));
}).AddApiExplorer(options =>
{
    // Add the versioned API explorer, which also adds IApiVersionDescriptionProvider service
    // note: the specified format code will format the version as "'v'major[.minor][-status]"
    options.GroupNameFormat = "'v'VVV";

    // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
    // can also be used to control the format of the API version in route templates
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();






var app = builder.Build();

// Configure the HTTP request pipeline.


app.ConfigureApiExceptionMiddlewareHandler();


app.UseSwagger();
app.UseSwaggerUI(options =>
{
    var descriptions = app.DescribeApiVersions();

    // Build a swagger endpoint for each discovered API version
    foreach (var description in descriptions)
    {
        var url = $"/swagger/{description.GroupName}/swagger.json";
        if (!app.Environment.IsDevelopment())
        {
            url = $"/api/swagger/{description.GroupName}/swagger.json";
        }
        var name = description.GroupName.ToUpperInvariant();
        options.SwaggerEndpoint(url, name);
    }
});
app.UseCors(x => x
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true)
                );

app.UseHttpsRedirection();
app.UseStatusCodePagesWithRedirects("/Error/Error{0}");
app.UseAuthentication();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();


app.MapControllers();
app.MapRazorPages();
app.Run();
