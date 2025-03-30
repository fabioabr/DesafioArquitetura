using FinancialServices.Infrastructure.Data.Contract;
using FinancialServices.Infrastructure.Data.Contract.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialServices.Infrastructure.Data.Concrete.Adapters
{
    public class InMemorySqliteDatabaseAdapter : IDatabaseAdapter
    {
        public Task AddAsync<T>(T entity) where T : IEntity
        {
            throw new NotImplementedException();
        }

        public Task AddRangeAsync<T>(IEnumerable<T> entities) where T : IEntity
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> Query<T>() where T : IEntity
        {
            throw new NotImplementedException();
        }

        public Task RemoveAsync<T>(T entity) where T : IEntity
        {
            throw new NotImplementedException();
        }

        public Task RemoveRangeAsync<T>(IEnumerable<T> entities) where T : IEntity
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync<T>(T entity, object updateFields) where T : IEntity
        {
            throw new NotImplementedException();
        }
    }
}
