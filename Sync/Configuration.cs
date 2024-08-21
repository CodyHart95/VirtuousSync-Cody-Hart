using System.Collections.Generic;

namespace Sync
{
    public interface IConfiguration
    {
        string GetValue(string key);
    }

    internal class Configuration : IConfiguration
    {
        public Configuration(string apiKey, string dbConnectionString) 
        {
            Values = new Dictionary<string, string>()
            {
                { "VirtuousApiBaseUrl", "https://api.virtuoussoftware.com" },
                { "VirtuousApiKey", apiKey },
                { "DBConnectionString",  dbConnectionString}
            };
        }

        private Dictionary<string, string> Values { get; set; }

        public string GetValue(string key)
        {
            return Values[key];
        }
    }
}
