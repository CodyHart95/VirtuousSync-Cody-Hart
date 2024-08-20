using System.Collections.Generic;

namespace Sync.Database
{
    internal class QueryFactory
    {
        private readonly IConfiguration config;
        private readonly EntityFrameworkContext context;

        public QueryFactory(IConfiguration config, EntityFrameworkContext context) 
        {
            this.config = config;
            this.context = context;
        }

        public IQuery BuildInsertContactsQuery(IEnumerable<AbbreviatedContact> contacts, QueryExecutionType executionType)
        {
            if(executionType == QueryExecutionType.Raw)
            {
                return new InsertContactsQuery(contacts, config);
            }

            return new InsertContactsQueryEntityFramework(contacts, context);
        }
    }
}
