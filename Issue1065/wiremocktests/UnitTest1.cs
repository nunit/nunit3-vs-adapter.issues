
using NFluent;
using NUnit.Framework;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace wiremocktests;

public class Tests
{
    private WireMockServer _server;
    
    [SetUp]
public void StartMockServer()
{
    _server = WireMockServer.Start();
}

[Test]
public async Task Should_respond_to_request()
{
  // Arrange (start WireMock.Net server)
  _server
    .Given(Request.Create().WithPath("/foo").UsingGet())
    .RespondWith(
      Response.Create()
        .WithStatusCode(200)
        .WithBody(@"{ ""msg"": ""Hello world!"" }")
    );

  // Act (use a HttpClient which connects to the URL where WireMock.Net is running)
  var response = await new HttpClient().GetAsync($"{_server.Urls[0]}/foo");

        // Assert
        Check.That(response).IsNotNull();
        Assert.That(response,Is.Not.Null);
}

[TearDown]
public void ShutdownServer()
{
    _server.Stop();
}
}