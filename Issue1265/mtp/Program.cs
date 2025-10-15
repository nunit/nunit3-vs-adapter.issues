using System.Reflection;

using Microsoft.Testing.Extensions;
using Microsoft.Testing.Platform.Builder;

using NUnit.VisualStudio.TestAdapter.TestingPlatformAdapter;

var testApplicationBuilder = await TestApplication.CreateBuilderAsync(args);

testApplicationBuilder.AddNUnit(() => [Assembly.GetEntryAssembly()!]);
testApplicationBuilder.AddTrxReportProvider();
testApplicationBuilder.AddAppInsightsTelemetryProvider();
using var testApplication = await testApplicationBuilder.BuildAsync();
return await testApplication.RunAsync();


