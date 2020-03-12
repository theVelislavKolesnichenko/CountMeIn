using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CountMeIn.ViewModels
{
    public class UserItem 
    {
        public int ID { get; set; }

        [Required(ErrorMessageResourceType = typeof(App_GlobalResources.Common), ErrorMessageResourceName = "Required")]
        [RegularExpression(@"^([0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$", ErrorMessageResourceType = typeof(App_GlobalResources.Common), ErrorMessageResourceName = "UserNameError")]
        public string UserName { get; set; }

        [Required(ErrorMessageResourceType = typeof(App_GlobalResources.Common), ErrorMessageResourceName = "Required")]
        [MinLength(6, ErrorMessageResourceType = typeof(App_GlobalResources.Common), ErrorMessageResourceName = "PasswordLengthError")]
        public string Password { get; set; }

        [Required(ErrorMessageResourceType = typeof(App_GlobalResources.Common), ErrorMessageResourceName = "Required")]
        [MinLength(6, ErrorMessageResourceType = typeof(App_GlobalResources.Common), ErrorMessageResourceName = "PasswordLengthError")]
        public string NewPassword { get; set; }

        [Required(ErrorMessageResourceType = typeof(App_GlobalResources.Common), ErrorMessageResourceName = "Required")]
        [MinLength(6, ErrorMessageResourceType = typeof(App_GlobalResources.Common), ErrorMessageResourceName = "PasswordLengthError")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessageResourceType = typeof(App_GlobalResources.Common), ErrorMessageResourceName = "Required")]
        public string FirstName { get; set; }

        [Required(ErrorMessageResourceType = typeof(App_GlobalResources.Common), ErrorMessageResourceName = "Required")]
        public string LastName { get; set; }

        public int UserTypeID { get; set; }

        public int GroupRoleID { get; set; }

        public string ProfileImageName { get; set; }

        public List<DML.Group> GroupItems { get; set; }

        public List<DML.GroupInvite> GroupInviteItems { get; set; }

        public List<DML.Event> JointEvents { get; set; }
    }
}
