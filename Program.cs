using System.Security.Cryptography;
using CodeHollow.FeedReader;
using Microsoft.Extensions.Hosting;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) => { })
    .Build();

static string CreateMD5(string input)
{
    using var md5 = MD5.Create();
    byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
    byte[] hashBytes = md5.ComputeHash(inputBytes);

    return Convert.ToHexString(hashBytes);
}

static async Task StartAnalysisAsync(IHost host)
{
    await Task.CompletedTask;

    Console.WriteLine("LVZ Headlines");

    var feed = await FeedReader.ReadAsync("https://www.lvz.de/arc/outboundfeeds/rss/");

    Console.WriteLine("Feed Title: " + feed.Title);
    Console.WriteLine("Feed Description: " + feed.Description);
    // ...
    foreach (var item in feed.Items)
    {
        Console.WriteLine(item.Id);
        Console.WriteLine(item.PublishingDateString);
        var md5 = CreateMD5(item.Id + item.PublishingDateString + item.Author + item.Content + item.Description +
                            item.Link + item.Title);
        Console.WriteLine(md5);
    }

    Environment.Exit(0);
}

await StartAnalysisAsync(host);
await host.RunAsync();
