using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DML
{
    public enum UnitState
    {
        Added,
        Unchanged,
        Modified,
        Deleted    
    }
    
    public interface IUnitState
    {
        UnitState UnitState { get; set; }
    }
}
