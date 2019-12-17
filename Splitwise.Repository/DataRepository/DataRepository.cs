using Microsoft.EntityFrameworkCore;
using Splitwise.DomainModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Splitwise.Repository.DataRepository
{
    public class DataRepository : IDataRepository
    {
        private readonly SplitwiseDbContext _db;
        //private readonly DbSet<T> table;

        public DataRepository(SplitwiseDbContext db)
        {
            _db = db;
        }

        public DbSet<T> SetDb<T>() where T : class
        {
            DbSet<T> table = _db.Set<T>();
            return table;
        }

        public async Task AddAsync<T>(T obj) where T : class
        {
            DbSet<T> table = SetDb<T>();
            await table.AddAsync(obj);
        }

        public IQueryable<T> Where<T>(Expression<Func<T, bool>> expression) where T : class
        {
            DbSet<T> table = SetDb<T>();
            return table.Where(expression);
        }

        public void Remove<T>(T obj) where T : class
        {
            DbSet<T> table = SetDb<T>();
            table.Remove(obj);
        }

        public async Task<List<T>> Get<T>() where T : class
        {
            DbSet<T> table = SetDb<T>();
            return await  table.ToListAsync();
        }

        //public IQueryable<T> Join<T, U, TKey>(Expression<Func<T, TKey>> tKey, Expression<Func<U, TKey>> uKey) where T : class, new() where U : class
        //{
        //    DbSet<T> tableT = SetDb<T>();
        //    DbSet<U> tableU = SetDb<U>();
        //    var data = tableT.Join(tableU, tKey, uKey, (tblT, tblU) => tblT);
        //    return data;
        //}


    }
}
