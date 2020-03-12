using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DAL;
using DML;

namespace BLL
{
    public static class GroupInviteManager
    {
        public static List<GroupInvite> GetGroupInviteByUserName(string userName, GroupInviteRelatedData relatedData = GroupInviteRelatedData.None)
        {
            using (IUnitOfWork unitOfWork = new cmiUnitOfWork())
            {
                Repository<GroupInvite> repository = new Repository<GroupInvite>(unitOfWork);
                List<GroupInvite> singleGroupInvite = repository.Get(gi => gi.UserMail == userName, GetIncludeProperties(relatedData)).ToList();
                return singleGroupInvite;
            }
        }

        public static bool Add(GroupInvite groupInvite)
        {
            bool successful = false;
            if (groupInvite == null) return successful;

            using (IUnitOfWork unitOfWork = new cmiUnitOfWork())
            {
                Repository<GroupInvite> repository = new Repository<GroupInvite>(unitOfWork);
                if (!repository.Get(u => u.UserMail == groupInvite.UserMail && u.GroupID == groupInvite.GroupID).Any())
                {
                    groupInvite.UnitState = UnitState.Added;
                    groupInvite = repository.Create(groupInvite);
                    unitOfWork.Commit();
                    successful = true;
                }
            }

            return successful;
        }

        public static bool Remove(int id)
        {

            using (IUnitOfWork unitOfWork = new cmiUnitOfWork())
            {
                Repository<GroupInvite> repository = new Repository<GroupInvite>(unitOfWork);
                //usersToGroup.UnitState = UnitState.Deleted;
                //repository.Delete(GetGroupID(usersToGroup.UserID, usersToGroup.GroupID));
                repository.Delete(id);
                unitOfWork.Commit();
                return true;
            }
        }

        public static bool Remove(GroupInvite groupInvite)
        {
            bool successful = false;
            if (groupInvite == null) return successful;
            
            using (IUnitOfWork unitOfWork = new cmiUnitOfWork())
            {
                Repository<GroupInvite> repository = new Repository<GroupInvite>(unitOfWork);
                groupInvite.UnitState = UnitState.Deleted;
                repository.Delete(groupInvite.ID);
                unitOfWork.Commit();
                successful = true;
            }
            return successful;
        }

        private static string GetIncludeProperties(GroupInviteRelatedData includes)
        {
            string includeProperties = "";
            switch (includes)
            {
                case GroupInviteRelatedData.None:
                    break;
                case GroupInviteRelatedData.Group:
                    includeProperties = @"Group";
                    break;
                default:
                    break;
            }

            return includeProperties;
        }

        public enum GroupInviteRelatedData
        {
            None,
            Group
        }

    }
}
