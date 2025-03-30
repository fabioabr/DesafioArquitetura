using FinancialServices.Infrastructure.Data.Contract;
using FinancialServices.Infrastructure.Data.Contract.Entity;
using FinancialServices.Infrastructure.Data.Contract.Repository;

namespace FinancialServices.Infrastructure.Data.Concrete
{
    public abstract class Repository<T> : IRepository<T> where T : IEntity
    {
        private readonly IDatabaseAdapter databaseAdapter;

        protected Repository()
        {
            
        }

        protected Repository(IDatabaseAdapter databaseAdapter)
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
