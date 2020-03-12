using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using PagedList;


namespace CountMeIn.ViewModels
{
    public class GroupItem
    {
        [Required(ErrorMessageResourceType = typeof(App_GlobalResources.Common), ErrorMessageResourceName = "Required")]
        [MaxLength(25, ErrorMessage="Максималма дължина 25" /*ErrorMessageResourceType = typeof(App_GlobalResources.Common), ErrorMessageResourceName = "PasswordLengthError"*/)]
        public string Name { get; set; }

        [Required(ErrorMessageResourceType = typeof(App_GlobalResources.Common), ErrorMessageResourceName = "Required")]
        [RegularExpression(@"^([0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$", ErrorMessageResourceType = typeof(App_GlobalResources.Common), ErrorMessageResourceName = "MailError")]
        public string GroupInviteUserMail { get; set; }

        public int ID { get; set; }

        public int OwnerID { get; set; }

        public string OwnerFullName { get; set; }

        public List<UserItem> userItems { get; set; }

        //public List<UserItem> userItems { get; set; }

        public List<DML.GroupInvite> userGroupInviteItems { get; set; }

        public IPagedList<DML.Event> groupEvents { get; set; }
    }
}