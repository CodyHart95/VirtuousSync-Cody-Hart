using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;

namespace Sync.Database
{
    internal class DatabaseHelper
    {
        //TODO: This isn't Connecting for some reason Its using my windows account not the username password...
        private const string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Virtuous;Integrated Security=True;Connect Timeout=30;Encrypt=False;User Id=VirtuousDbAdmin; Password=Password1!;";

        public async Task ExecuteQuery(IQuery query)
        {
            var queryString = query.ToDbQuery();

            using(StreamWriter sw = new StreamWriter("Query.sql"))
            {
                await sw.WriteAsync(queryString);
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(queryString, conn);
                conn.Open();

                try
                {
                    await cmd.ExecuteNonQueryAsync();
                }
                finally
                {
                    conn.Close();
                }
            }
        }
    }
}
