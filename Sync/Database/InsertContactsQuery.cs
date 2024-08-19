using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sync.Database
{
    public class InsertContactsQuery : IQuery
    {
        private readonly IEnumerable<AbbreviatedContact> contacts;

        public InsertContactsQuery(IEnumerable<AbbreviatedContact> contacts) 
        {
            this.contacts = contacts;
        }

        // TODO: We can do better than this
        public string ToDbQuery()
        {
            var queryBuilder = new StringBuilder();
            queryBuilder.Append("MERGE INTO dbo.Contacts AS target USING (VALUES ");

            ParseContactRowsToQuery(contacts, queryBuilder);

            queryBuilder.Append(@"
) AS source (Id, Name, ContactType, ContactName, Address, Email, Phone)
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
    VALUES (source.Id, source.Name, source.ContactType, source.ContactName, source.Address, source.Email, source.Phone);
            ");

            return queryBuilder.ToString();
        }

        // TODO: Think about if this is the best approach some more
        private void ParseContactRowsToQuery(IEnumerable<AbbreviatedContact> contacts, StringBuilder queryBuilder)
        {

            var insertValues = contacts.Select(contact => $"({contact.Id}, '{EscapeCharacters(contact.Name)}', '{EscapeCharacters(contact.ContactType)}', '{EscapeCharacters(contact.ContactName)}', '{EscapeCharacters(contact.Address)}', '{EscapeCharacters(contact.Email)}', '{EscapeCharacters(contact.Phone)}')");
            var joinedInsertValues = string.Join(",\n", insertValues);

            queryBuilder.Append(joinedInsertValues);
        }

        // THIS IS BAD, DEF NEED A BETTER WAY TO DO THIS. QUERY PARAMS MAYBE???
        private string EscapeCharacters(string value)
        {
            return value.Replace("'", "''");
        }
    }
}
