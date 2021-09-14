using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using HttpTriggerCSharpSample.Validators;
using Configurations;
using Microsoft.Extensions.Configuration;
using Services;

[assembly: FunctionsStartup(typeof(Dependencies.Startup))]

namespace Dependencies
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddOptions<AzAdOptions>()
                                        .Configure<IConfiguration>((settings, configuration) =>
                                        {
                                            configuration.GetSection("AzAdOptions").Bind(settings);
                                        });

            builder.Services.AddHttpClient();

            builder.Services.AddSingleton<IMyService>((s) => {
                return new MyService();
            });

            builder.Services.AddTransient<IAzManagementApiService,AzManagementApiService>().AddHttpClient();

            builder.Services.AddTransient<IAzDevOpsServices,AzDevOpsServices>().AddHttpClient();

            builder.Services.AddScoped<RequestValidator>();

        }
    }
}
