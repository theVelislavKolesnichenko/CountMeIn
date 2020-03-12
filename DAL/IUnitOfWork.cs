using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;

namespace DAL
{
    /// <summary>
    /// IUnitOfWork interface.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Get or set current context
        /// </summary> 
        DbContext Context { get; set; }

        /// <summary>
        /// Enable/Disable entity option for lazy loading 
        /// </summary>
        bool LazyLoadingEnabled { get; set; }

        /// <summary>
        /// Check all entities for IUnitState interface implementation.
        /// </summary>
        bool HasUnitStateImplemented { get; }

        /// <summary>
        /// Saves all pending context changes.
        /// </summary>   
        int Commit();

        /// <summary>
        /// Apply changes for all modified or added entities.
        /// </summary>
        void ApplyChanges();
    }
}
