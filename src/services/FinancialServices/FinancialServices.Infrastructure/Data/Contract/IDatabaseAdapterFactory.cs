using FinancialServices.Infrastructure.Enums;

namespace FinancialServices.Infrastructure.Data.Contract
{
    public interface IDatabaseAdapterFactory
    {
        IDatabaseAdapter CreateDatabaseAdapter(DatabaseTypeEnum databaseType);

    }
}
