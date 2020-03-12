using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CountMeIn.ViewModels;
using DML;
using System.Web.Mvc;

namespace CountMeIn.Core
{
    public static class UserExtensions
    {
        public static UserItem ToUserItem(this User inputUser)
        {
            UserItem singleGroupItem = new UserItem();

                singleGroupItem.ID = inputUser.ID;
                singleGroupItem.UserName = inputUser.UserName;
                singleGroupItem.FirstName = inputUser.FirstName;
                singleGroupItem.LastName = inputUser.LastName;
                singleGroupItem.Password = inputUser.Password;
                singleGroupItem.ConfirmPassword = inputUser.Password;
                singleGroupItem.UserTypeID = inputUser.UserTypeID;

                singleGroupItem.ProfileImageName = inputUser.ProfileImageName;

                singleGroupItem.GroupItems = new List<Group>();    

                foreach (UsersToGroup utg in inputUser.UsersToGroups)
                {
                    singleGroupItem.GroupRoleID = utg.GroupRoleID;
                    singleGroupItem.GroupItems.Add(utg.Group);
                }

                //singleGroupItem.JointEvents = new List<Event>();

                //foreach (UsersToEvent ute in inputUser.UsersToEvents)
                //{
                //    singleGroupItem.JointEvents.Add(ute.Event);
                //}


            return singleGroupItem;
        }

        public static User ToUser(this UserItem inputuser)
        {
            return new User
            {
                UserName = inputuser.UserName,
                FirstName = inputuser.FirstName,
                LastName = inputuser.LastName,
                Password = inputuser.Password
            };
        }
    }
}