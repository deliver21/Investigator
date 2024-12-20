using AutoMapper.Execution;
using Investigator.Data;
using Investigator.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Investigator.Repository
{
    public class Repository<T>:IRepository<T> where T : class
    {
        private readonly AppDbContext _context;
        internal DbSet<T> _dbSet;
        public Repository(AppDbContext context)
        {
            _context= context;
            _dbSet = _context.Set<T>();
        }
        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }
        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }
        public void RemoveRange(IEnumerable<T> enitities)
        {
            _dbSet.RemoveRange(enitities);   
        }
        public T Get(Expression<Func<T, bool>>? filter, string ? includeProperties = null, bool track = false)
        {
            IQueryable<T> query;
            query = track ? _dbSet : _dbSet.AsNoTracking();
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach(var includeProperty in includeProperties.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries))
                {
                    query.Include(includeProperty);
                }
            }
            return query.FirstOrDefault();
        }
        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter, string? includeProperties = null, bool track = false)
        {
            IQueryable<T> queries = !track ? _dbSet.AsNoTracking() : _dbSet;
            if (filter != null)
            {
                queries = queries.Where(filter);
            }
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    queries.Include(includeProperty);
                }
            }
            return queries.ToList();
        }
    }
}
