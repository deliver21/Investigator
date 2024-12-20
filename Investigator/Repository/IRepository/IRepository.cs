﻿using System.Linq.Expressions;
namespace Investigator.Repository.IRepository
{
    public interface IRepository<T> where T: class
    {
        void Add(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
        T Get(Expression<Func<T, bool>> filter, string ? includeProperties = null, bool track = false);
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null , string? includeProperties = null, bool track = false);
    }
}