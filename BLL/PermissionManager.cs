using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DAL;
using DML;

namespace BLL
{
    public static class PermissionManager
    {
        public static string GetPermissionForUser(string username)
        {
            using (IUnitOfWork unitofWork = new cmiUnitOfWork())
            {
                User user = UserManager.GetByUsername(username, UserManager.UserRelatedData.Type);
                if (user != null)
                {
                    return user.UserType.Name;
                }
                return DML.Enums.UserType.Unknow.ToString();
            } 
        }

        public static string[] GetPermissionsForUser(string username)
        {
            return new string[] { GetPermissionForUser(username) };
        }

        public static bool HasPermission(int id, DML.Enums.PermissionsItem permissionsItem)
        {
            using (IUnitOfWork unitOfWork = new cmiUnitOfWork())
            {
                Repository<UserTypesToPermission> repository = new Repository<UserTypesToPermission>(unitOfWork);
                return repository.Contains(ut2p => ut2p.PermissionID == (int)permissionsItem && ut2p.UserType.Users.Any(u => u.ID == id));
            }
        }

    }
}
