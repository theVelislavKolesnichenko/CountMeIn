using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DML;

namespace DAL
{
    public class DalHelper<T> where T : class, IUnitState
    {
        public static void RemoveCollection(IEnumerable<T> collection)
        {
            foreach (T item in collection)
            {
                item.UnitState = UnitState.Deleted;
            }
        }
    }
}
