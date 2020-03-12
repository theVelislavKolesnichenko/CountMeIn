using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Text;

namespace DAL
{

    /// <summary>
    /// No instance creation , used only for inheritance
    /// </summary>
    public abstract class UnitOfWork : IUnitOfWork
    {
        #region Constructor

        /// <summary>
        /// Public empty constructor
        /// </summary>
        public UnitOfWork()
        {
        }

        #endregion

        #region IUnitOfWork Members

        /// <summary>
        /// A DbContext instance represents a combination of the Unit Of Work and Repository patterns such that it can be used to query from a database and group together
        /// changes that will then be written back to the store as a unit.  DbContext is conceptually similar to ObjectContext.
        /// </summary>
        public DbContext Context { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether lazy loading of relationships exposed 
        /// as navigation properties is enabled. Lazy loading is enabled by default.
        /// </summary>
        public bool LazyLoadingEnabled
        {
            get { return Context.Configuration.LazyLoadingEnabled; }
            set { Context.Configuration.LazyLoadingEnabled = value; }
        }

        /// <summary>
        /// Gets the value indicating whether has IUnitState interface implemented.
        /// Check all entities for unit state implementation.
        /// </summary>
        public virtual bool HasUnitStateImplemented
        {
            get { return false; }
        }

        /// <summary>
        /// Saves all changes made in this context to the underlying database.
        /// </summary>
        /// <returns>The number of objects written to the underlying database.</returns>
        public int Commit()
        {
            try
            {
                if (HasUnitStateImplemented)
                {
                    ApplyChanges();
                }

                return Context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                StringBuilder exMessage = new StringBuilder();
                foreach (var entityValidationErrors in dbEx.EntityValidationErrors)
                {
                    string entity = entityValidationErrors.Entry.Entity.ToString();
                    foreach (var validationError in entityValidationErrors.ValidationErrors)
                    {
                        exMessage.AppendLine(String.Format("\tProperty '{0}.{1}' ->  {2}", entity, validationError.PropertyName, validationError.ErrorMessage));
                    }
                }

                throw new DbContextCommitValidationException(exMessage.ToString(), dbEx.InnerException);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //return 0; //unreachable code
        }

        /// <summary>
        /// Virtual 
        /// </summary>
        public virtual void ApplyChanges(){}
      
        /// <summary>
        /// Calls the protected DBContext Dispose method.
        /// </summary>
        public void Dispose()
        {
            if (Context != null)
            {
                Context.Dispose();
            }
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
