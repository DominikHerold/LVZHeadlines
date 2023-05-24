using System.Security.Cryptography;
using System.Text;
using CodeHollow.FeedReader;
using Flurl.Http;
using Microsoft.Extensions.Hosting;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) => { })
    .Build();

static string CreateMD5(string input)
{
    using var md5 = MD5.Create();
    byte[] inputBytes = Encoding.ASCII.GetBytes(input);
    byte[] hashBytes = md5.ComputeHash(inputBytes);

    return Convert.ToHexString(hashBytes);
}

static async Task StartAnalysisAsync(IHost host)
{
    using CancellationTokenSource tokenSource = new();

    Console.CancelKeyPress += delegate { tokenSource.Cancel(); };

    Console.WriteLine("LVZ Headlines");

    var feed = await FeedReader.ReadAsync("https://www.lvz.de/arc/outboundfeeds/rss/tags_slug/leipzig/");

    Console.WriteLine("Feed Title: " + feed.Title);
    Console.WriteLine("Feed Description: " + feed.Description);

    var pushOverToken = Environment.GetEnvironmentVariable("PUSH_OVER_TOKEN");
    var pushOverUser = Environment.GetEnvironmentVariable("PUSH_OVER_USER");

    foreach (var item in feed.Items)
    {
        Console.WriteLine(item.Id);
        Console.WriteLine(item.PublishingDateString);
        var md5 = CreateMD5(item.Id + item.PublishingDateString + item.Author + item.Content + item.Description +
                            item.Link + item.Title);
        Console.WriteLine(md5);

        Directory.CreateDirectory("./MD5");
        Directory.CreateDirectory("./Title");

        var id = item.Id.Substring(0, item.Id.LastIndexOf('-'));
        var md5FullPath = Path.Combine("./MD5", CreateMD5(id));
        if (!File.Exists(md5FullPath) || File.ReadAllText(md5FullPath) != md5)
        {
            var result = await item.Link.GetStringAsync();
            var endIndex = result.IndexOf("</h2>", StringComparison.Ordinal);
            var startIndex = result.Substring(0, endIndex).LastIndexOf('>');
            var title = result.Substring(startIndex + 1, endIndex - startIndex - 1);

            await File.WriteAllTextAsync(
                md5FullPath,
                md5,
                tokenSource.Token);

            var titleFullPath = Path.Combine("./Title", CreateMD5(id));
            var oldTitle = string.Empty;
            if (File.Exists(titleFullPath))
            {
                oldTitle = await File.ReadAllTextAsync(titleFullPath, tokenSource.Token);
            }

            await File.WriteAllTextAsync(
                titleFullPath,
                title,
                tokenSource.Token);

            if (!string.IsNullOrEmpty(oldTitle) && oldTitle != title)
            {
                var diffHelper = new HtmlDiff.HtmlDiff(oldTitle, title);
                var output = diffHelper.Build();
                output = output.Replace("</ins>", "</font>").Replace("</del>", "</font>")
                    .Replace("<del class='diffmod'>", "<font color=\"#e70000\">")
                    .Replace("<del class='diffdel'>", "<font color=\"#e70000\">")
                    .Replace("<ins class='diffmod'>", "<font color=\"#00e700\">")
                    .Replace("<ins class='diffins'>", "<font color=\"#00e700\">");
                var client = new HttpClient();
                var toSend = $"token={pushOverToken}&user={pushOverUser}&message={output}&html=1";

                client.PostAsync("https://api.pushover.net/1/messages.json",
                        new StringContent(toSend, Encoding.UTF8, "application/x-www-form-urlencoded")).GetAwaiter()
                    .GetResult();
            }
        }
    }

    Environment.Exit(0);
}

await StartAnalysisAsync(host);
await host.RunAsync();
