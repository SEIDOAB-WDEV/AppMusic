//Show all files in Solution explorer.
//./obj/Debug/net7.0/AppMvc.GlobalUsings.g.cs show all implicit "using"

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;
using Models;
using DbContext;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();

builder.Services.AddIdentityDbContext();

builder.Services.AddDefaultIdentity<csUser>(options => {
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.SignIn.RequireConfirmedAccount = true;
})
    .AddEntityFrameworkStores<DbContext.csMainDbContext>();

builder.Services.AddTransient<Microsoft.AspNetCore.Identity.UI.Services.IEmailSender, csEmailService>();


#region Injecting a dependency service to read MusicWebApi
if (Configuration.csAppConfig.DataSource == "WebApi")
{
    builder.Services.AddHttpClient(name: "MusicWebApi", configureClient: options =>
    {
        options.BaseAddress = Configuration.csAppConfig.WebApiBaseUri;
        options.DefaultRequestHeaders.Accept.Add(
            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(
                mediaType: "application/json",
                quality: 1.0));
    });
    builder.Services.AddScoped<IMusicService, csMusicServiceWaco>();
}
else
{
    builder.Services.AddScoped<IMusicService, csMusicService>();

}
#endregion

var app = builder.Build();

//using Hsts and https to secure the site
if (!app.Environment.IsDevelopment())
{
    //https://en.wikipedia.org/wiki/HTTP_Strict_Transport_Security
    //https://learn.microsoft.com/en-us/aspnet/core/security/enforcing-ssl
    app.UseHsts();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();


app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

//Map Razorpages into Pages folder
app.MapRazorPages();

//Default HTTPGet response
app.MapGet("/hello", () =>
{
    //read the environment variable ASPNETCORE_ENVIRONMENT
    //Change in launchSettings.json, (not VS2022 Debug/Release)
    var _env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
    var _envMyOwn = Environment.GetEnvironmentVariable("MyOwn");

    return $"Hello World!\nASPNETCORE_ENVIRONMENT: {_env}\nMyOwn: {_envMyOwn}";
});

app.Run();

#region L1.1 Start App in Kestrel without VS2022 environment:
//Here I add to be shown in console as final after application stopped
//open terminal in AppMvc and type
//dotnet run --launch-profile https

//stopp server in Kesterl by ctrl-C
Console.WriteLine("The AppMvc webserver has stopped");
#endregion
