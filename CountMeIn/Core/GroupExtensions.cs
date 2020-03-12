using CountMeIn.ViewModels;
using DML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CountMeIn.Core
{
    public static class GroupExtensions
    {
        public static Group ToGroup(this GroupItem inputGroupItem)
        {
            return new Group 
            {
                Name = inputGroupItem.Name,
                OwnerID = inputGroupItem.OwnerID
            };
        }

        public static GroupItem ToGroupItem(this Group inputGroup)
        {
            GroupItem groupItem = new GroupItem();
            groupItem.ID = inputGroup.ID;
            groupItem.Name = inputGroup.Name;
            groupItem.OwnerID = inputGroup.OwnerID;
            groupItem.OwnerFullName = string.Format("{0} {1}", inputGroup.User.FirstName, inputGroup.User.LastName);
            groupItem.userItems = new List<UserItem>();
            groupItem.userGroupInviteItems = new List<GroupInvite>();

            foreach (UsersToGroup utu in inputGroup.UsersToGroups)
            {
                groupItem.userItems.Add(utu.User.ToUserItem());
            }

            foreach (GroupInvite gi in inputGroup.GroupInvites)
            {
                groupItem.userGroupInviteItems.Add(gi);
            }

            return groupItem;
        }
    }
}