using CommandLine;

namespace LVZHeadlines
{
    public class ActionInputs
    {
        [Option('t', "token", Required = true)]
        public string Token { get; set; } = string.Empty;
    }
}
