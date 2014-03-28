using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using RestApp.Core;
using RestApp.Core.Data;
using System.Linq.Expressions;

namespace RestApp.Services
{
    /// <summary>
    /// Entity Framework repository
    /// </summary>
    public partial class EfRepository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly IDbContext _context;
        private IDbSet<T> _entities;

        private IDbSet<T> Entities
        {
            get
            {
                if (_entities == null)
                    _entities = _context.Set<T>();
                return _entities;
            }
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="context">Object context</param>
        public EfRepository(IDbContext context)
        {
            this._context = context;
        }



        public void Insert(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");

                this.Entities.Add(entity);

                this._context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        msg += string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage) + Environment.NewLine;

                var fail = new Exception(msg, dbEx);
                //Debug.WriteLine(fail.Message, fail);
                throw fail;
            }
        }

        public void Update(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");

                this._context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        msg += Environment.NewLine + string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);

                var fail = new Exception(msg, dbEx);
                //Debug.WriteLine(fail.Message, fail);
                throw fail;
            }
        }

        public void Delete(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");

                this.Entities.Remove(entity);

                this._context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        msg += Environment.NewLine + string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);

                var fail = new Exception(msg, dbEx);
                //Debug.WriteLine(fail.Message, fail);
                throw fail;
            }
        }

        public T GetById(object id)
        {
            return this.Entities.Find(id);
        }

        public T Get(Expression<Func<T, bool>> predicate)
        {
            return this.Entities.FirstOrDefault(predicate);
        }
        public virtual IQueryable<T> Table
        {
            get
            {
                return this.Entities;
            }
        }

        public int Count(Expression<Func<T, bool>> predicate)
        {
            return Fetch(predicate).Count();
        }

        public IQueryable<T> Fetch(Expression<Func<T, bool>> predicate)
        {
            return this.Entities.Where(predicate);
        }

        public IQueryable<T> Fetch(Expression<Func<T, bool>> predicate, Action<Orderable<T>> order)
        {
            //return this.Entities.Where(predicate);

            var orderable = new Orderable<T>(Fetch(predicate));
            order(orderable);

            return orderable.Queryable;
        }

        public IQueryable<T> Fetch(Expression<Func<T, bool>> predicate, Action<Orderable<T>> order, int skip, int count)
        {
            return Fetch(predicate, order).Skip(skip).Take(count);
        }
    }
}