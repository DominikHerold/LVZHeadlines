using Microsoft.Extensions.Hosting;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) => { })
    .Build();

static async Task StartAnalysisAsync(IHost host)
{
    await Task.CompletedTask;

    Console.WriteLine("LVZ Headlines");

    Environment.Exit(0);
}

await StartAnalysisAsync(host);
await host.RunAsync();
