using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Issue_972.common
{
    public abstract class AbstractRepository<T> : IRepository<T> where T : class
    {
        protected readonly AdministrationContext _administrationContext;
        //protected readonly ILogger<AbstractRepository<T>> _logger;
        protected readonly DbSet<T> _data;
        protected AbstractRepository(AdministrationContext administrationContext, DbSet<T> data)
        {
            _administrationContext = administrationContext;
            _data = data;
        }

        public IQueryable<T> GetAll()
        {
            return _data;
        }

        public IQueryable<string> GetAllString()
        {
            return _data.Select(x => x.ToString());
        }

        public async Task Add(T t)
        {
            _data.Add(t);
            int i = await _administrationContext.SaveChangesAsync();
            if (i == 0) return;//do some logging stuff
        }

        public async Task<T> AddReturn(T t)
        {
            await Add(t);
            return t;
        }

        public async Task AddList(IEnumerable<T> list)
        {
            foreach(T t in list)
                _data.Add(t);
            
            int i = await _administrationContext.SaveChangesAsync();
            if (i < list.Count()) return; //do some logging stuff
        }

        public async Task Delete(T t)
        {
            _data.Remove(t);
            int i = await _administrationContext.SaveChangesAsync();
            if (i == 0) return;//do some logging stuff
        }

        public async Task DeleteList(IQueryable<T> list)
        {
            foreach(var t in list)
                _data.Remove(t);

            int i = await _administrationContext.SaveChangesAsync();
            if (i < list.Count()) return; //do some logging stuff
        }

        public async Task<string> ListToString(List<T> t = default)
        {
            string answer = "";

            if (t == default) t = await GetAll().ToListAsync();

            for (int i = 0; i < t.Count; i++)
            {
                T temp = t[i];

                answer += i + " - " + temp.ToString();

                if (i < t.Count - 1) answer += "\n";
            }

            return answer;
        }

        public abstract Task<T> GetById(int id);
    }
}