//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DML
{
    using System;
    using System.Collections.Generic;
    
    public partial class UsersToGroup : IUnitState
    {
        public int ID { get; set; }
        public int GroupID { get; set; }
        public int UserID { get; set; }
        public int GroupRoleID { get; set; }
    
        public virtual GroupRole GroupRole { get; set; }
        public virtual Group Group { get; set; }
        public virtual User User { get; set; }
    
    
    	#region IUnitState Members
    
        public UnitState UnitState	{ get; set; }
    
        #endregion
    }
}
