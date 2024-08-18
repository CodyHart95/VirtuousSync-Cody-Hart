using CsvHelper;
using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace Sync
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Sync().GetAwaiter().GetResult();
        }

        private static async Task Sync()
        {
            var apiKey = "";
            var configuration = new Configuration(apiKey);
            var virtuousService = new VirtuousService(configuration);

            var skip = 0;
            var take = 100;
            var maxContacts = 1000;
            var hasMore = true;

            using (var writer = new StreamWriter($"Contacts_{DateTime.Now:MM_dd_yyyy}.csv"))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                do
                {
                    Console.WriteLine($"Fetching Contacts {skip} through {skip + take}");
                    var contacts = await virtuousService.GetContactsAsync(skip, take, "AZ");
                    skip += take;
                    csv.WriteRecords(contacts.List);
                    hasMore = skip < maxContacts && skip < contacts.Total;
                }
                while (hasMore);
            }
        }
    }
}
