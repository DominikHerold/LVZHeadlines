﻿using System.Security.Cryptography;
using CodeHollow.FeedReader;
using Flurl.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualBasic;

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
    using CancellationTokenSource tokenSource = new();

    Console.CancelKeyPress += delegate { tokenSource.Cancel(); };

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

        var result = await item.Link.GetStringAsync();
        var endIndex = result.IndexOf("</h2>", StringComparison.Ordinal);
        var startIndex = result.Substring(0, endIndex).LastIndexOf('>');
        var title = result.Substring(startIndex + 1, endIndex - startIndex - 1);

        Directory.CreateDirectory("./MD5");
        Directory.CreateDirectory("./Title");

        var md5FullPath = Path.Combine("./MD5", CreateMD5(item.Id));
        await File.WriteAllTextAsync(
            md5FullPath,
            md5,
            tokenSource.Token);

        var titleFullPath = Path.Combine("./Title", CreateMD5(item.Id));
        await File.WriteAllTextAsync(
            titleFullPath,
            title,
            tokenSource.Token);
    }

    Environment.Exit(0);
}

await StartAnalysisAsync(host);
await host.RunAsync();
