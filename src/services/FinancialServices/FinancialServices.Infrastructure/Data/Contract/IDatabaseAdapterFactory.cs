using FinancialServices.Infrastructure.Enum;

namespace FinancialServices.Infrastructure.Data.Contract
{
    public interface IDatabaseAdapterFactory
    {
        IDatabaseAdapter CreateDatabaseAdapter(DatabaseTypeEnum databaseType,string connectionString, string databaseName);

    }
}
