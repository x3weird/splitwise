using Microsoft.EntityFrameworkCore;
using Splitwise.DomainModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splitwise.Repository.DataRepository
{
    public class DataRepository<T> : IDataRepository<T> where T: class
    {
        private readonly SplitwiseDbContext _db;
        private readonly DbSet<T> table;

        public DataRepository(SplitwiseDbContext db)
        {
            _db = db;
            table = _db.Set<T>();
        }

        public async Task AddAsync(T obj)
        {
            await table.AddAsync(obj);
        }

        public async Task WhereAsync(T obj)
        {
            //await table.Where(t=>t.)
        }
    }
}
