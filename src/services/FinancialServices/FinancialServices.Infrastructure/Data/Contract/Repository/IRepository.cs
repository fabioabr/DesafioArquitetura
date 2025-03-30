﻿using FinancialServices.Infrastructure.Data.Contract.Entity;

namespace FinancialServices.Infrastructure.Data.Contract.Repository
{
    public interface IRepository<T> where T : IEntity
    {
        IQueryable<T> Query();
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        Task UpdateAsync(T entity, object updateFields);
        Task RemoveAsync(T entity);
        Task RemoveRangeAsync(IEnumerable<T> entities);

    }

}
