using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace TravelAPI.Interfaces 
{
    public interface IBaseRepository<T> where T : class
    {
        public Task<IEnumerable<T>> GetObjects(); 
        public Task<IEnumerable<T>> GetObjects(Expression<Func<T,bool>> match);
        public Task<T> FindObjectAsync(int ID);    
        public Task<bool>   Create(T obj);
        public Task<bool>   DeleteObject(int iD);
        public bool         Update(T Entity);
    }
}
