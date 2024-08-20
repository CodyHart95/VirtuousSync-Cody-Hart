using System.Threading.Tasks;

namespace Sync.Database
{
    internal interface IQuery
    {
        Task Execute();
    }
}
