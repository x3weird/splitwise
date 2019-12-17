using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Splitwise.Repository.DataRepository
{
    public interface IDataRepository
    {
        Task AddAsync<T>(T obj) where T : class;
        IQueryable<T> Where<T>(Expression<Func<T, bool>> expression) where T : class;
        void Remove<T>(T obj) where T : class;
        Task<List<T>> Get<T>() where T : class;
    }
}
