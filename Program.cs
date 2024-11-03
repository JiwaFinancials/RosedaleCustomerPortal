using JiwaCustomerPortal.Components;
using System.Net;

namespace JiwaCustomerPortal
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            ConfigurationManager configuration = builder.Configuration;
            Config.JiwaAPIURL = configuration.GetSection("JiwaAPIURL").Value;
            Config.JiwaAPIKey = configuration.GetSection("JiwaAPIKey").Value;

            // We want to read some config settings and we should wait until thats' finished before proceediing
            // if it fails, it will throw an exception and we'll not be able to start - which is what we want because
            // we can't do anything without the API, and if the call to the API to get some config values fails then we cannot go on.
            try
            {
                Task readConfigTask = Task.Run(async () =>
                    {
                        await Config.ReadSettingsFromAPI();
                    });
                readConfigTask.Wait();
            }            
            catch (Exception ex)
            {
                // wrap the exception with something more obvious to the operator what the problem is.
                throw new Exception("Could not read the Config settings from the API - ensure the Jiwa API is running and you have configured in appsettings.json the correct value for JiwaAPIURL and JiwaAPIKey.", ex);
            }

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            // ColourModeServices is used to store the colour mode and it's a singleton so it can be referenced everywhere
            builder.Services.AddSingleton<IColourModeServices, ColourModeServices>();
            // BrowserService is what we use to look at what the preferred browser dark mode is - we'll fall back to that
            // if we didn't find the colourmode in the browser local storage.
            // We also get from BrowserService the users date format and the Bootstrap version we're using (to display on the diagnostics page)
            builder.Services.AddScoped<BrowserService>();

            builder.Services.AddScoped<WebPortalUserSessionStateContainer>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
