using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync.Database
{
    internal class InsertContactsQuery : IQuery
    {
        private const string queryMergeStatement = "MERGE INTO dbo.Contacts AS target USING (VALUES ";
        private const string queryUpdateStatment = @") AS source (Id, Name, ContactType, ContactName, Address, Email, Phone)
ON target.Id = source.Id
WHEN MATCHED THEN 
    UPDATE SET 
        Name = source.Name,
        ContactType = source.ContactType,
        ContactName = source.ContactName,
        Address = source.Address,
        Email = source.Email,
        Phone = source.Phone
WHEN NOT MATCHED THEN
    INSERT (Id, Name, ContactType, ContactName, Address, Email, Phone)
    VALUES (source.Id, source.Name, source.ContactType, source.ContactName, source.Address, source.Email, source.Phone);";
      
        private readonly IEnumerable<AbbreviatedContact> contacts;
        private readonly string connectionString;

        public InsertContactsQuery(IEnumerable<AbbreviatedContact> contacts, IConfiguration config) 
        {
            this.contacts = contacts;
            this.connectionString = config.GetValue("DBConnectionString");
        }

        public async Task Execute()
        {
            var queryCommands = CreateSqlCommands();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    foreach (var command in queryCommands)
                    {
                        command.Connection = conn;
                        await command.ExecuteNonQueryAsync();
                    }
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        private List<SqlCommand> CreateSqlCommands()
        {
            var commands = new List<SqlCommand>();
            var commandGroups = BatchContacts(contacts);

            foreach(var group in commandGroups)
            {
                var sqlCommand = new SqlCommand();
                var queryBuilder = new StringBuilder();

                queryBuilder.Append(queryMergeStatement);

                AppendContactRowsToQuery(group, queryBuilder, sqlCommand);

                queryBuilder.Append(queryUpdateStatment);

                sqlCommand.CommandText = queryBuilder.ToString();
                commands.Add(sqlCommand);
            }


            return commands;
        }

        private void AppendContactRowsToQuery(IEnumerable<AbbreviatedContact> contacts, StringBuilder queryBuilder, SqlCommand sqlCommand)
        {
            foreach(var contact in contacts)
            {
                queryBuilder.AppendLine($"(@Id_{contact.Id}, @Name_{contact.Id}, @ContactType_{contact.Id}, @ContactName_{contact.Id}, @Address_{contact.Id}, @Email_{contact.Id}, @Phone_{contact.Id}),");

                sqlCommand.Parameters.AddWithValue($"@Id_{contact.Id}", contact.Id);
                sqlCommand.Parameters.AddWithValue($"@Name_{contact.Id}", contact.Name);
                sqlCommand.Parameters.AddWithValue($"@ContactType_{contact.Id}", contact.ContactType);
                sqlCommand.Parameters.AddWithValue($"@ContactName_{contact.Id}", contact.ContactName);
                sqlCommand.Parameters.AddWithValue($"@Address_{contact.Id}", contact.Address);
                sqlCommand.Parameters.AddWithValue($"@Email_{contact.Id}", contact.Email);
                sqlCommand.Parameters.AddWithValue($"@Phone_{contact.Id}", contact.Phone);
            }

            queryBuilder.Remove(queryBuilder.Length - 3, 1);
        }

        private List<IEnumerable<AbbreviatedContact>> BatchContacts(IEnumerable<AbbreviatedContact> contacts)
        {
            var groups = new List<IEnumerable<AbbreviatedContact>>();
            for(var i = 0; i < contacts.Count(); i += 200 )
            {
                var group = contacts.Skip(i).Take(200);
                groups.Add(group);
            }

            return groups;
        }
    }
}
