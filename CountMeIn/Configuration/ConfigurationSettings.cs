using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DML.Enums;
namespace CountMeIn.Configuration
{
    public static class ConfigurationSettings
    {
        public const int ItemsPerPage = 5;
        public const string DateRegex = @"[0-3]?[0-9].[0-1]?[0-9].{2,4}\d";

        static IEnumerable<DML.Enums.UserType> values = Enum.GetValues(typeof(DML.Enums.UserType)).Cast<DML.Enums.UserType>();
        public static IEnumerable<SelectListItem> userTypeItems
        {
            get
            {
                return from value in values
                       where value != DML.Enums.UserType.Unknow
                       select new SelectListItem
                       {
                           Text = GetUserTypeBG((DML.Enums.UserType)value),
                           Value = ((int)(DML.Enums.UserType)value).ToString(),
                           //Selected = (int)(DML.Enums.UserType)value == (int)DML.Enums.UserType.Player
                       };
            }
        }

        static IEnumerable<DML.Enums.GroupRoles> groupRoles = Enum.GetValues(typeof(DML.Enums.GroupRoles)).Cast<DML.Enums.GroupRoles>();
        public static IEnumerable<SelectListItem> groupRoleItems
        {
            get
            {
                return from value in groupRoles
                       where value != DML.Enums.GroupRoles.Unknow
                       select new SelectListItem
                       {
                           Text = GetGroupRoleBG((DML.Enums.GroupRoles)value),
                           Value = ((int)(DML.Enums.GroupRoles)value).ToString()
                           //Selected = (int)(DML.Enums.GroupRole)value == (int)DML.Enums.GroupRole.Player
                       };
            }
        }

        public static string GetGroupRoleBG(DML.Enums.GroupRoles includes)
        {
            string includeProperties = "";
            switch (includes)
            {
                case DML.Enums.GroupRoles.Unknow:
                    break;
                case DML.Enums.GroupRoles.Moderator:
                    includeProperties = @"Модератор";
                    break;
                case DML.Enums.GroupRoles.Player:
                    includeProperties = @"Играч";
                    break;
                default:
                    break;
            }

            return includeProperties;
        }

        private static string GetUserTypeBG(DML.Enums.UserType includes)
        {
            string includeProperties = "";
            switch (includes)
            {
                case DML.Enums.UserType.Unknow:
                    break;
                case DML.Enums.UserType.Admin:
                    includeProperties = @"Администратор";
                    break;
                case DML.Enums.UserType.Player:
                    includeProperties = @"Играч";
                    break;
                default:
                    break;
            }

            return includeProperties;
        }

        public static string Message(MessageId? message)
        {
            return message == MessageId.ChangePasswordSuccess ? App_GlobalResources.Common.ChangePasswordSuccess
                : message == MessageId.ChangeUserNameSuccess ? App_GlobalResources.Common.ChangeUserNameSuccess
                : message == MessageId.ChangeFullNameSuccess ? App_GlobalResources.Common.ChangeFullNameSuccess
                : message == MessageId.ChangeUserTypeSuccess ? App_GlobalResources.Common.ChangeUserTypeSuccess
                : message == MessageId.GroupOwner ? App_GlobalResources.Common.GroupOwner
                : message == MessageId.ResisterSuccess ? App_GlobalResources.Common.SuccessfullyRegistered + "\t" + App_GlobalResources.Common.Welcome
                : message == MessageId.ChangesSuccessfully ? App_GlobalResources.Common.ChangesSuccessfully
                : message == MessageId.LoginSuccess ? App_GlobalResources.Common.Welcome
                : "";
        }

        public static string ImegPath(string imageName, ImageSize imageSize)
        {
            if (imageName != null)
            {
                return Utils.AppUtils.AppDomainPath + "Uploads/ProfileImages/" + imageSize + "/" + imageName;
            }

            return Utils.AppUtils.AppDomainPath + "Uploads/ProfileImages/" + imageSize + "/" + "Empty.png";
        }

    }
}