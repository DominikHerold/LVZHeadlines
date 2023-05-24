using System.Security.Cryptography;
using CodeHollow.FeedReader;
using CommandLine;
using Flurl.Http;
using LVZHeadlines;
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

static async Task StartAnalysisAsync(IHost host, ActionInputs actionInputs)
{
    using CancellationTokenSource tokenSource = new();

    Console.CancelKeyPress += delegate { tokenSource.Cancel(); };

    Console.WriteLine(actionInputs.Token);
    Console.WriteLine(Environment.GetEnvironmentVariable("MY_SECRET"));

    Console.WriteLine("LVZ Headlines");

    //var feed = await FeedReader.ReadAsync("https://www.lvz.de/arc/outboundfeeds/rss/tags_slug/leipzig/");

    //Console.WriteLine("Feed Title: " + feed.Title);
    //Console.WriteLine("Feed Description: " + feed.Description);
    //// ...
    //foreach (var item in feed.Items)
    //{
    //    Console.WriteLine(item.Id);
    //    Console.WriteLine(item.PublishingDateString);
    //    var md5 = CreateMD5(item.Id + item.PublishingDateString + item.Author + item.Content + item.Description +
    //                        item.Link + item.Title);
    //    Console.WriteLine(md5);

    //    Directory.CreateDirectory("./MD5");
    //    Directory.CreateDirectory("./Title");

    //    var id = item.Id.Substring(0, item.Id.LastIndexOf('-'));
    //    var md5FullPath = Path.Combine("./MD5", CreateMD5(id));
    //    if (!File.Exists(md5FullPath) || File.ReadAllText(md5FullPath) != md5)
    //    {
    //        var result = await item.Link.GetStringAsync();
    //        var endIndex = result.IndexOf("</h2>", StringComparison.Ordinal);
    //        var startIndex = result.Substring(0, endIndex).LastIndexOf('>');
    //        var title = result.Substring(startIndex + 1, endIndex - startIndex - 1);

    //        await File.WriteAllTextAsync(
    //            md5FullPath,
    //            md5,
    //            tokenSource.Token);

    //        var titleFullPath = Path.Combine("./Title", CreateMD5(id));
    //        await File.WriteAllTextAsync(
    //            titleFullPath,
    //            title,
    //            tokenSource.Token);
    //    }
    //}

    Environment.Exit(0);
}

var actionInputs = Parser.Default.ParseArguments<ActionInputs>(args).Value;
await StartAnalysisAsync(host, actionInputs);
await host.RunAsync();
