using FinancialServices.Infrastructure.Data.Contract.Entity;

namespace FinancialServices.Infrastructure.Data.Contract
{
    public interface IDatabaseAdapter
    {
        IQueryable<T> Query<T>() where T : IEntity; 
        Task AddAsync<T>(T entity) where T : IEntity;
        Task AddRangeAsync<T>(IEnumerable<T> entities) where T : IEntity;
        Task UpdateAsync<T>(T entity, object updateFields) where T : IEntity;
        Task RemoveAsync<T>(T entity) where T : IEntity;
        Task RemoveRangeAsync<T>(IEnumerable<T> entities) where T : IEntity;        
    }
}
