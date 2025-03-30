using FinancialServices.Infrastructure.Data.Adapters;
using FinancialServices.Infrastructure.Data.Contract;
using FinancialServices.Infrastructure.Enum;

namespace FinancialServices.Infrastructure.Data.Factory
{
    public class DatabaseAdapterFactory : IDatabaseAdapterFactory
    {



        public IDatabaseAdapter CreateDatabaseAdapter(DatabaseTypeEnum databaseType, string connectionString, string databaseName)
        {


            switch (databaseType)
            {
                case DatabaseTypeEnum.InMemorySQLite:
                    return new InMemorySqliteDatabaseAdapter();

                case DatabaseTypeEnum.MongoDB:
                    return new MongoDBDatabaseAdapter(connectionString, databaseName);
                default:
                    throw new NotImplementedException();
            }
        }

       
    }
}
