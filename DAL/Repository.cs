using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace DAL
{
    /// <summary>
    /// Repository implements methods for the operation with a DbContext instance. 
    /// And to be used to query from a database and group together changes that will then be written back to the store as a unit.
    /// </summary>
    /// <typeparam name="T">Entity Type</typeparam>
    public class Repository<T> : IRepository<T> where T : class, DML.IUnitState
    {
        #region  Properties

        /// <summary>
        /// Holds a reference to the <see cref="IUnitOfWork" />provider.
        /// </summary>
        protected readonly IUnitOfWork UnitOfWork;

        /// <summary>
        /// Current database context.
        /// </summary>
        protected readonly DbContext _dbContext;

        /// <summary>
        /// Current entity set.
        /// </summary>
        protected readonly IDbSet<T> _dbSet;

        #endregion

        #region Constructors

        /// <summary>
        ///   Initializes a new instance of the class with a
        ///   <see cref="IUnitOfWork" /> implementation.
        /// </summary>
        /// <param name="unitOfWork"> The Unit of Work implementation. </param>
        public Repository(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
            _dbContext = unitOfWork.Context;
            _dbSet = unitOfWork.Context.Set<T>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the number of elements in a sequence.
        /// </summary>
        public virtual int Count
        {
            get { return _dbSet.Count(); }
        }

        /// <summary>
        /// Gets all entities from database as IQuerable
        /// </summary>
        public virtual IQueryable<T> All()
        {
            return _dbSet.AsQueryable();
        }

        /// <summary>
        /// Gets an entity with the given primary key values.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual T GetById(object id)
        {
            return _dbSet.Find(id);
        }
        
        /// <summary>
        ///   Gets entities via optional filter, sort order, and includes
        /// </summary>
        /// <param name="filter">A function to test each element for a condition.</param>
        /// <param name="includeProperties">The dot-separated list of related objects to return in the query results and then comma-separated to accept list of properties.</param>
        public virtual IQueryable<T> Get(Expression<Func<T, bool>> filter, string includeProperties)
        {
            return Get(filter, null, includeProperties);
        }

        /// <summary>
        ///   Gets entities via optional filter, sort order, and includes
        /// </summary>
        /// <param name="filter">A function to test each element for a condition.</param>
        /// <param name="orderBy">unused</param>
        /// <param name="includeProperties">The dot-separated list of related objects to return in the query results and then comma-separated to accept list of properties.</param>
        public virtual IQueryable<T> Get(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "")
        {
            IQueryable<T> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (!String.IsNullOrWhiteSpace(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = System.Data.Entity.DbExtensions.Include(query, includeProperty); 
                }
            }

            if (orderBy != null)
            {
                return orderBy(query).AsQueryable();
            }
            else
            {
                return query.AsQueryable();
            }
        }

        /// <summary>
        /// Gets entities from database by filter.
        /// </summary>
        /// <param name="predicate">Specified a filter </param>
        public virtual IQueryable<T> Filter(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate).AsQueryable();
        }

        /// <summary>
        /// Gets entities from database with filting and paging.
        /// </summary>
        /// <param name="filter">Specified a filter </param>
        /// <param name="total">Returns the total records count of the filter. </param>
        /// <param name="index">Specified the page index. </param>
        /// <param name="size">Specified the page size </param>
        public virtual IQueryable<T> Filter(Expression<Func<T, bool>> filter, out int total, int index = 0, int size = 50)
        {
            int skipCount = index * size;
            var resetSet = filter != null ? _dbSet.Where(filter).AsQueryable() : _dbSet.AsQueryable();
            resetSet = skipCount == 0 ? resetSet.Take(size) : resetSet.Skip(skipCount).Take(size);
            total = resetSet.Count();
            return resetSet.AsQueryable();
        }

        /// <summary>
        /// Gets the entity(s) is exists in database by specified filter.
        /// </summary>
        /// <param name="predicate"> Specified the filter expression </param>
        public bool Contains(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Count(predicate) > 0;
        }

        /// <summary>
        /// Find entity by keys.
        /// </summary>
        /// <param name="keys">Specified the search keys. </param>
        public virtual T Find(params object[] keys)
        {
            return _dbSet.Find(keys);
        }

        /// <summary>
        /// Find entity by specified expression.
        /// </summary>
        /// <param name="predicate"> </param>
        public virtual T Find(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.FirstOrDefault(predicate);
        }

        /// <summary>
        /// Create a new object to database.
        /// </summary>
        /// <param name="entity">Specified a new object to create. </param>
        public virtual T Create(T entity)
        {
            var newEntry = _dbSet.Add(entity);
            return newEntry;
        }

        /// <summary>
        /// Deletes the entity by primary key
        /// </summary>
        /// <param name="id"> </param>
        public virtual void Delete(object id)
        {
            var entityToDelete = _dbSet.Find(id);
            if (entityToDelete != null)
            {
                Delete(entityToDelete);
            }
        }

        /// <summary>
        /// Delete the entity from database.
        /// </summary>
        /// <param name="entity">Specified a existing entity to delete. </param>
        public virtual void Delete(T entity)
        {
            if (_dbContext.Entry(entity).State == EntityState.Detached)
            {
                _dbSet.Attach(entity);
            }
            entity.UnitState = DML.UnitState.Deleted;
            _dbSet.Remove(entity);
        }

        /// <summary>
        /// Delete entities from database by specified filter expression.
        /// </summary>
        /// <param name="predicate"> </param>
        public virtual void Delete(Expression<Func<T, bool>> predicate)
        {
            var entitiesToDelete = Filter(predicate);
            foreach (var entity in entitiesToDelete)
            {
                if (_dbContext.Entry(entity).State == EntityState.Detached)
                {
                    _dbSet.Attach(entity);
                }
                entity.UnitState = DML.UnitState.Deleted;
                _dbSet.Remove(entity);
            }
        }

        /// <summary>
        /// Update entity changes and save to database - use only for instances not from current UnitOfWork.
        /// Be careful for foreign keys.
        /// </summary>
        /// <param name="entity">Specified the entity to save. </param>
        public virtual void Update(T entity)
        {
            var entry = _dbContext.Entry(entity);
            _dbSet.Add(entity);
            entry.State = EntityState.Modified;
        }

        /// <summary>
        /// Attaches the given entity to the context and set the properties that are modified and need to be updated in db.
        /// </summary>
        /// <param name="entity">The entity to be attacted to the context</param>
        /// <param name="changedProperties">The properties that are changed, these are the only properties that will be updated in db.</param>
        public virtual void Attach(T entity, params string[] changedProperties) 
        {
            _dbSet.Attach(entity);
            foreach (string property in changedProperties) 
            {
                _dbContext.Entry(entity).Property(changedProperties[0]).IsModified = true;
            }
        }

        #endregion
    }

}