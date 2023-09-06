using MudBlazor.Services;
using RestApiReporting.WebApp.Service;
using RestApiReporting.WebApp.Shared;

namespace RestApiReporting.WebApp;

public class Program
{
    /// <summary>Reporting web app</summary>
    /// <param name="args">supported arguments
    /// --api, example: --api=https://localhost:7082
    /// </param>
    /// <exception cref="ReportException"></exception>
    public static void Main(string[] args)
    {
        const string apiArgName = "--api=";
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorPages();
        builder.Services.AddServerSideBlazor();

        // configuration
        var configBuilder = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .AddUserSecrets<Program>();
        var configuration = configBuilder.Build();
        builder.Services.AddSingleton<IConfiguration>(configuration);

        // api url
        var apiUrlArg = args.FirstOrDefault(x => x.StartsWith(apiArgName, StringComparison.OrdinalIgnoreCase));
        var apiUrl = apiUrlArg?.Substring(apiArgName.Length);
        if (string.IsNullOrWhiteSpace(apiUrl))
        {
            apiUrl = configuration.ApiUrl();
        }
        if (string.IsNullOrWhiteSpace(apiUrl))
        {
            Console.WriteLine("Missing Reporting API URL.");
            Console.WriteLine(" - use the command line argument --api, example: --api=https://localhost:7082");
            Console.WriteLine($" - set the {nameof(ProgramConfiguration)}:{nameof(ProgramConfiguration.ApiUrl)} setting in appsettings.json");
            Environment.ExitCode = -1;
            return;
        }
        builder.Services.AddScoped(_ => new HttpClient
        {
            BaseAddress = new Uri(apiUrl)
        });
        Console.WriteLine($"Reporting on {apiUrl}.");

        // application services
        builder.Services.AddTransient<IQueryService>(x => new QueryService(
            x.GetRequiredService<HttpClient>()));
        builder.Services.AddTransient<IReportingService>(x => new ReportingService(
            x.GetRequiredService<HttpClient>()));

        // mud blazor
        builder.Services.AddMudServices();

        var app = builder.Build();

        // configure the HTTP request pipeline
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.MapBlazorHub();
        app.MapFallbackToPage("/_Host");

        app.Run();
    }
}