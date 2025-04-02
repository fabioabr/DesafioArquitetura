using FinancialServices.Infrastructure.Data.Adapter;
using FinancialServices.Infrastructure.Data.Contract;
using FinancialServices.Infrastructure.Enums;

namespace FinancialServices.Infrastructure.Data.Factory
{
    public class DatabaseAdapterFactory : IDatabaseAdapterFactory
    {

        public IDatabaseAdapter CreateDatabaseAdapter(DatabaseTypeEnum databaseType)
        {




            switch (databaseType)
            {
                case DatabaseTypeEnum.InMemorySQLite:
                    return new InMemorySqliteDatabaseAdapter();
                case DatabaseTypeEnum.MongoDB:
                    var databaseName = Environment.GetEnvironmentVariable("CustomSettings__Database__DatabaseName") ?? throw new System.Exception("CustomSettings__DatabaseName is not set");
                    var connectionStrng = Environment.GetEnvironmentVariable("CustomSettings__Database__ConnectionStrings__Default") ?? throw new System.Exception("CustomSettings__ConnectionStrings__Default is not set");
                    return new MongoDBDatabaseAdapter(connectionStrng, databaseName);
                default:
                    throw new NotImplementedException();
            }
        }


    }
}
