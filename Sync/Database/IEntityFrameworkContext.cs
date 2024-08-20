using System.Data.Entity;
using System.Threading.Tasks;

namespace Sync.Database
{
    internal interface IEntityFrameworkContext
    {
        DbSet<AbbreviatedContact> Contacts { get; set; }

        Task<int> SaveChangesAsync();
    }
}