using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DML;
using System.Data.Entity.Infrastructure;
using System.Data;

namespace DAL
{
    public class cmiUnitOfWork : UnitOfWork
    {
        #region Fields & Properties

        /// <summary>
        /// Check all entities for IUnitState interface implementation.
        /// </summary>
        public override bool HasUnitStateImplemented
        {
            get
            {
                return CheckForEntitiesWithoutUnitStateInterface();
            }
        }

        #endregion

         #region Constructor

        /// <summary>
        ///  FROB UnitOfWork implementation constructor.
        /// </summary>
        public cmiUnitOfWork()
        {
            Context = new CountMeInEntities();
            LazyLoadingEnabled = false;
            Context.Configuration.AutoDetectChangesEnabled = false;

            InitEntitiesWithtUnitStateInterface();
            
        }

        #endregion

        /// <summary>
        /// Init all entities implements IUnitState interface.
        /// </summary>
        private void InitEntitiesWithtUnitStateInterface()
        {
            if (HasUnitStateImplemented)
            {
                ((IObjectContextAdapter)Context).ObjectContext
                .ObjectMaterialized += (sender, args) =>
                {
                    var entity = args.Entity as IUnitState;
                    if (entity != null)
                    {
                        entity.UnitState = UnitState.Unchanged;
                    }
                };
            }
        }

        /// <summary>
        /// Check all entities for unit state implementation.
        /// </summary>
        private bool CheckForEntitiesWithoutUnitStateInterface()
        {
            var entitiesWithoutState =
              from e in Context.ChangeTracker.Entries()
              where !(e.Entity is IUnitState)
              select e;

            return !entitiesWithoutState.Any();
        }

        /// <summary>
        /// Convert unit state to entity state.
        /// </summary>
        public static EntityState ConvertToEntityState(UnitState unitState)
        {
            switch (unitState)
            {
                case UnitState.Added:
                    return EntityState.Added;

                case UnitState.Modified:
                    return EntityState.Modified;

                case UnitState.Deleted:
                    return EntityState.Deleted;

                default:
                    return EntityState.Unchanged;
            }
        }

        /// <summary>
        /// Apply changes for all modified or added entities.
        /// </summary>
        public override void ApplyChanges()
        {
            Context.ChangeTracker.DetectChanges();
            foreach (var entry in Context.ChangeTracker.Entries<IUnitState>())
            {
                IUnitState stateInfo = entry.Entity;
                entry.State = ConvertToEntityState(stateInfo.UnitState);

                if (stateInfo.UnitState == UnitState.Unchanged)
                {
                    var databaseValues = entry.GetDatabaseValues();
                    entry.OriginalValues.SetValues(databaseValues);
                }
            }
        }

    }
}
