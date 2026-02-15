using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace App.Test;

public class TestOptionsFactory : WebApplicationFactory<Program>
{
    private readonly string _environmentName;

    public TestOptionsFactory(string environmentName)
    {
        _environmentName = environmentName;
    }

    public new IServiceProvider Services => Server.Services;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment(_environmentName);
        
        builder.ConfigureAppConfiguration(config =>
        {
            var collection = new Dictionary<string, string?>
            {
                ["TestSetting:Key1"] = "test-value-1",
                ["TestSetting:Key2"] = "test-value-2"
            };

            config.AddInMemoryCollection(collection);
        });
        
        base.ConfigureWebHost(builder);
    }
}
