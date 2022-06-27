using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Issue_972.common
{
    public interface IRepository<T> where T : class
    {
        Task Add(T t);
        Task<T> AddReturn(T t);
        Task AddList(IEnumerable<T> list);
        Task Delete(T t);
        Task DeleteList(IQueryable<T> list);
        IQueryable<T> GetAll();
        IQueryable<string> GetAllString();
        Task<T> GetById(int id);
        Task<string> ListToString(List<T> t = null);
    }
}
