
using Microsoft.Extensions.Hosting;

using Deloitte.Ink.Transition.Importer.Reports.StartUp;

var host = new HostBuilder().BuildAppHost().Build();

using (host)
{
    await host.RunAsync();
}