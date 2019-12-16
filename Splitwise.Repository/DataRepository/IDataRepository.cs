using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Splitwise.Repository.DataRepository
{
    public interface IDataRepository<T> where T : class
    {
        Task AddAsync(T entity);
        Task WhereAsync(T entity);
    }
}
