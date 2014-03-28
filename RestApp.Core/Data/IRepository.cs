using System.Linq;
using System.Linq.Expressions;
using System;


namespace RestApp.Core.Data
{
    /// <summary>
    /// Repository
    /// </summary>
    public partial interface IRepository<T> where T : BaseEntity
    {
        void Insert(T entity);
        void Update(T entity);
        void Delete(T entity);

        T GetById(object id);
        T Get(Expression<Func<T, bool>> predicate);

        IQueryable<T> Table { get; }

        int Count(Expression<Func<T, bool>> predicate);
        IQueryable<T> Fetch(Expression<Func<T, bool>> predicate);
        IQueryable<T> Fetch(Expression<Func<T, bool>> predicate, Action<Orderable<T>> order);
        IQueryable<T> Fetch(Expression<Func<T, bool>> predicate, Action<Orderable<T>> order, int skip, int count);
    }
}
