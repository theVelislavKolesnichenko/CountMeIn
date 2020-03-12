using System;
using System.Linq;
using System.Linq.Expressions;

namespace DAL
{
    /// <summary>
    /// Repository Interface defines the methods that need to be implemented for the operation with a DbContext instance. 
    /// And to be used to query from a database and group together  changes that will then be written back to the store as a unit.
    /// </summary>
    /// <typeparam name="T">Entity Type</typeparam>
    public interface IRepository<T> where T : class, DML.IUnitState
    {
        /// <summary>
        ///   Get the total entities count.
        /// </summary>
        int Count { get; }

        /// <summary>
        ///   Gets all entities from database
        /// </summary>
        IQueryable<T> All();

        /// <summary>
        /// Gets an entity by primary key.
        /// </summary>
        /// <param name="id"> primary key </param>
        /// <returns> </returns>
        T GetById(object id);

        /// <summary>
        /// Gets entities via optional filter, sort order, and includes
        /// </summary>
        /// <param name="filter"> </param>
        /// <param name="orderBy"> </param>
        /// <param name="includeProperties"> </param>
        /// <returns> </returns>
        IQueryable<T> Get(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "");

        /// <summary>
        /// Gets entities from database by filter.
        /// </summary>
        /// <param name="predicate">Specified a filter </param>
        IQueryable<T> Filter(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Gets entities from database with filting and paging.
        /// </summary>
        /// <param name="filter">Specified a filter </param>
        /// <param name="total">Returns the total records count of the filter. </param>
        /// <param name="index">Specified the page index. </param>
        /// <param name="size">Specified the page size </param>
        IQueryable<T> Filter(Expression<Func<T, bool>> filter, out int total, int index = 0, int size = 50);

        /// <summary>
        /// Gets the entity(s) is exists in database by specified filter.
        /// </summary>
        /// <param name="predicate"> Specified the filter expression </param>
        bool Contains(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Find entity by keys.
        /// </summary>
        /// <param name="keys">Specified the search keys. </param>
        T Find(params object[] keys);

        /// <summary>
        ///   Find entity by specified expression.
        /// </summary>
        /// <param name="predicate"> </param>
        T Find(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Create a new entity to database.
        /// </summary>
        /// <param name="entity">Specified a new entity to create. </param>
        T Create(T entity);

        /// <summary>
        /// Deletes the entity by primary key
        /// </summary>
        /// <param name="id"> </param>
        void Delete(object id);

        /// <summary>
        /// Delete the entity from database.
        /// </summary>
        /// <param name="entity">Specified a existing entity to delete. </param>
        void Delete(T entity);

        /// <summary>
        /// Delete entities from database by specified filter expression.
        /// </summary>
        /// <param name="predicate"> </param>
        void Delete(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Update entity changes and save to database.
        /// </summary>
        /// <param name="entity">Specified the entity to save. </param>
        void Update(T entity);

        /// <summary>
        /// Attaches the given entity to the context and set the properties that are modified and need to be updated in db.
        /// </summary>
        /// <param name="entity">The entity to be attacted to the context</param>
        /// <param name="changedProperties">The properties that are changed, these are the only properties that will be updated in db.</param>
        void Attach(T entity, params string[] changedProperties);
    }
}