using FinancialServices.Domain.Core.Contracts;
using FinancialServices.Infrastructure.Data.Contract;

namespace FinancialServices.Infrastructure.Data.Repository
{
    public class Repository<T> : IRepository<T> where T : IEntity
    {
        private readonly IDatabaseAdapter databaseAdapter;
         
        public Repository(IDatabaseAdapter databaseAdapter)
        {
            this.databaseAdapter = databaseAdapter;
        }
        public Task AddAsync(T entity)
        {
            return databaseAdapter.AddAsync(entity);
        }
        public Task AddRangeAsync(IEnumerable<T> entities)
        {
            return databaseAdapter.AddRangeAsync(entities);
        }

        public Task InsertOrUpdate(T entity)
        {
            return databaseAdapter.InsertOrUpdate(entity);
        }

        public IQueryable<T> Query()
        {
            return databaseAdapter.Query<T>();
        }
        public Task RemoveAsync(T entity)
        {
            return databaseAdapter.RemoveAsync(entity);
        }
        public Task RemoveRangeAsync(IEnumerable<T> entities)
        {
            return databaseAdapter.RemoveRangeAsync(entities);
        }
        public Task UpdateAsync(T entity, object updateFields)
        {
            return databaseAdapter.UpdateAsync(entity, updateFields);
        }
    }
}
