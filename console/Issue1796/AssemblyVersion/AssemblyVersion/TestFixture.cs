using CoreWCF.Configuration;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

public class TestFixture
{
    [Test]
    public void Test()
    {
        MultipartReader xx = null!;
        var services = new ServiceCollection();
        // use Modules window to see that after this line older runtime libraries loaded
        ServiceModelServiceCollectionExtensions.AddServiceModelServices(services);
    }
}
