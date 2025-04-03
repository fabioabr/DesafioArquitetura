using FinancialServices.Domain.Model;
using FinancialServices.Infrastructure.Data.Adapter;
using FinancialServices.Infrastructure.Data.Contract;
using FinancialServices.Infrastructure.Enums;

namespace FinancialServices.Infrastructure.Data.Factory
{
    public class DatabaseAdapterFactory : IDatabaseAdapterFactory
    {
        private readonly ApplicationSettingsModel settings;
        public DatabaseAdapterFactory(ApplicationSettingsModel settings)
        {
            this.settings = settings;
        }
        public IDatabaseAdapter CreateDatabaseAdapter( )
        { 
            switch (Enum.Parse<DatabaseTypeEnum>(settings.DatabaseToUse))
            {
                case DatabaseTypeEnum.InMemorySQLite:
                    return new InMemorySqliteDatabaseAdapter();
                case DatabaseTypeEnum.MongoDB:
                    var databaseName = settings.DatabasSettings.MongoDbSettings.DatabaseName;
                    var connectionString = settings.DatabasSettings.MongoDbSettings.ConnectionString;
                    return new MongoDBDatabaseAdapter(connectionString, databaseName);
                default:
                    throw new NotImplementedException();
            }
        }


    }
}
