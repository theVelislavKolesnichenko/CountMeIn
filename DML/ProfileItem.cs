using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DML
{
    public class ProfileItem
    {
        public User user { get; set; }
        public List<GroupInvite> groupInvite { get; set; }
        public List<UsersToEvent> usersToEvent { get; set; } 
    }
}
