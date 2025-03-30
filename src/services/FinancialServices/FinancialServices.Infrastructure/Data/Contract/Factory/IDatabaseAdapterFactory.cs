using FinancialServices.Infrastructure.Enum;

namespace FinancialServices.Infrastructure.Data.Contract.Factory
{
    public interface IDatabaseAdapterFactory
    {
        IDatabaseAdapter CreateDatabaseAdapter(DatabaseTypeEnum databaseType,string connectionString, string databaseName);

    }
}
