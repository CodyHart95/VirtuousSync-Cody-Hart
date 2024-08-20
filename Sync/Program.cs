using Sync.Database;
using System;
using System.Collections.Generic;
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
            try
            {
                var start = DateTime.Now;
                var apiKey = "";
                var connectionString = "";
                var configuration = new Configuration(apiKey, connectionString);
                var virtuousService = new VirtuousService(configuration);
                var efContext = new EntityFrameworkContext(configuration);
                var queryFactory = new QueryFactory(configuration, efContext);

                var queryType = GetExecutionTypeInput();
                Console.WriteLine();

                var skip = 0;
                var take = 100;
                var maxContacts = 1000;
                var hasMore = true;

                var records = new List<AbbreviatedContact>();

                do
                {
                    Console.WriteLine($"Fetching Contacts {skip} through {skip + take}");
                    var contacts = await virtuousService.GetContactsAsync(skip, take);
                    skip += take;
                    records.AddRange(contacts.List);
                    hasMore = skip < maxContacts && skip < contacts.Total;
                }
                while (hasMore);

                var fetchTime = DateTime.Now - start;
                Console.WriteLine($"Time to fetch records: {fetchTime}");

                var queryTypeText = queryType == QueryExecutionType.EntityFramework ? "EntityFramework" : "Raw Query";
                Console.WriteLine($"Writing {records.Count} records to DB using {queryTypeText}");

                var query = queryFactory.BuildInsertContactsQuery(records, queryType);
                await query.Execute();

                var end = DateTime.Now - start;
                Console.WriteLine($"Time to update DB: {end - fetchTime}");
                Console.WriteLine($"Total Execution Time: {end}");
            }
            catch( Exception ex )
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                Console.ReadKey();
            }
        }

        private static QueryExecutionType GetExecutionTypeInput()
        {
            Console.Write("Use Entity Framework for query (y/n): ");
            var useEntityFramework = Console.ReadKey();
            if (useEntityFramework.KeyChar != 'y' && useEntityFramework.KeyChar != 'n')
            {
                Console.WriteLine();
                return GetExecutionTypeInput();
            }

            return useEntityFramework.KeyChar == 'y' ? QueryExecutionType.EntityFramework : QueryExecutionType.Raw;
        }
    }
}
