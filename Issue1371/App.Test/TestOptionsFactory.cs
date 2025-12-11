using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

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
        
        base.ConfigureWebHost(builder);
    }
}
