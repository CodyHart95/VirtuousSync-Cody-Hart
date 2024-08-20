using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;

namespace Sync.Database
{
    internal class InsertContactsQueryEntityFramework : IQuery
    {
        private readonly IEnumerable<AbbreviatedContact> contacts;
        private readonly IEntityFrameworkContext efContext;

        public InsertContactsQueryEntityFramework(IEnumerable<AbbreviatedContact> contacts, IEntityFrameworkContext efContext)
        {
            this.contacts = contacts;
            this.efContext = efContext;
        }

        public async Task Execute()
        {
            efContext.Contacts.AddOrUpdate(c => c.Id, contacts.ToArray());
            await efContext.SaveChangesAsync();
        }
    }
}
