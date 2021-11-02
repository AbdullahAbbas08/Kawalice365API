using BalarinaAPI.Core.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TravelAPI.Core;
using TravelAPI.Interfaces;

namespace TravelAPI.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly BalarinaDatabaseContext dbContext;

        public BaseRepository(BalarinaDatabaseContext dbContext)
        {
            this.dbContext = dbContext;
            dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        }

        public async Task<IEnumerable<T>> GetObjects()
        {
            return await dbContext.Set<T>().AsQueryable().ToListAsync();
        }

        public async Task<IEnumerable<T>> GetObjects(int Number)
        {
            return await dbContext.Set<T>().Take(Number).AsQueryable().ToListAsync();
        }

        public async Task<IEnumerable<T>> GetObjects(Expression<Func<T, bool>> match)
        {
            return  await dbContext.Set<T>().AsQueryable().Where(match).ToListAsync(); 
        }
        public async Task<T> FindObjectAsync(int ID)
        {
            return await  dbContext.Set<T>().FindAsync(ID);
        }        
        public async Task<bool> Create(T obj)
        {
            try
            {
                await dbContext.Set<T>().AddAsync(obj);
            }
            catch
            {
                return false;
            }
            return true;
        }
        public async Task<bool> DeleteObject(int ID)
        {
            try
            {
                 T obj = (T) await FindObjectAsync(ID);
                dbContext.Set<T>().Remove(obj);
            }
            catch
            {
                return false;
            }
            return true;
        }
        public bool Update(T Entity)
        {
            try
            {
                //dbContext.Update(Entity);
                dbContext.Entry(Entity).State = EntityState.Modified;
            }
            catch
            {
                return false;
            }
            return true;

        }
    }
}

