using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DML;
using DAL;

namespace BLL
{
    public static class UsersToGroupManager
    {
        public static bool Add(UsersToGroup usersToGroup)
        {
            using (IUnitOfWork unitOfWork = new cmiUnitOfWork())
            {
                Repository<UsersToGroup> repository = new Repository<UsersToGroup>(unitOfWork);
                usersToGroup.UnitState = UnitState.Added;
                usersToGroup = repository.Create(usersToGroup);
                unitOfWork.Commit(); 
                return true;
            }
        }

        public static bool Remove(UsersToGroup usersToGroup)
        {
            bool successful = false;
            if (usersToGroup == null) return successful;

            using (IUnitOfWork unitOfWork = new cmiUnitOfWork())
            {
                Repository<UsersToGroup> repository = new Repository<UsersToGroup>(unitOfWork);
                usersToGroup.UnitState = UnitState.Deleted;
                repository.Delete(GetByUserIDGroupID(usersToGroup.UserID, usersToGroup.GroupID));
                //repository.Delete(usersToGroup.ID);
                unitOfWork.Commit();
                successful = true;
            }

            return successful;
        }

        public static UsersToGroup GetByUserIDGroupID(int userID, int groupID, UsersToGroupRelatedData relatedData = UsersToGroupRelatedData.None)
        {
            using (IUnitOfWork unitOfWork = new cmiUnitOfWork())
            {
                Repository<UsersToGroup> repository = new Repository<UsersToGroup>(unitOfWork);
                UsersToGroup userToGroup = repository.Get(x => x.GroupID == groupID && x.UserID == userID, GetIncludeProperties(relatedData)).SingleOrDefault();
                return userToGroup;
            }
        }

        public static bool IsMember(string userName, int groupID)
        {
            using (IUnitOfWork unitOfWork = new cmiUnitOfWork())
            {
                Repository<UsersToGroup> repository = new Repository<UsersToGroup>(unitOfWork);
                return repository.Contains(ut2g => ut2g.User.UserName == userName && ut2g.GroupID == groupID);
            }
        }

        public static bool IsMember(int userID, int groupID)
        {
            using (IUnitOfWork unitOfWork = new cmiUnitOfWork())
            {
                Repository<UsersToGroup> repository = new Repository<UsersToGroup>(unitOfWork);
                return repository.Contains(ut2g => ut2g.User.ID == userID && ut2g.GroupID == groupID);
            }
        }

        public static bool IsModerator(int userID, int groupID)
        {
            using (IUnitOfWork unitOfWork = new cmiUnitOfWork())
            {
                Repository<UsersToGroup> repository = new Repository<UsersToGroup>(unitOfWork);
                return repository.Contains(ut2g => ut2g.User.ID == userID && ut2g.GroupID == groupID && ut2g.GroupRoleID == (int)DML.Enums.GroupRoles.Moderator);
            }
        }

        private static string GetIncludeProperties(UsersToGroupRelatedData includes)
        {
            string includeProperties = "";
            switch (includes)
            {
                case UsersToGroupRelatedData.None:
                    break;
                case UsersToGroupRelatedData.Events:
                    includeProperties = @"Group.Events";
                    break;
                default:
                    break;
            }

            return includeProperties;
        }

        public enum UsersToGroupRelatedData
        {
            None,
            Events
        }


        public static bool Update(UsersToGroup userToGroup)
        {
            bool successful = false;
            if (userToGroup == null) return successful;

            using (IUnitOfWork unitOfWork = new cmiUnitOfWork())
            {
                Repository<UsersToGroup> repository = new Repository<UsersToGroup>(unitOfWork);
                userToGroup.UnitState = UnitState.Modified;
                //repository.Update(user);
                repository.Attach(userToGroup);
                unitOfWork.Commit();
                successful = true;
            }
            return successful;
        }
    }
}
