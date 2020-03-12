using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DAL;
using DML;
using DML.Enums;


namespace BLL
{
    public static class UserManager
    {
        public static LoginState Validate(string username, string password, out User user)
        {
            using (IUnitOfWork unitOfWork = new cmiUnitOfWork())
            {
                //string passwordHash = Utils.
                Repository<User> repository = new Repository<User>(unitOfWork);
                user = repository.Get(u => u.UserName == username && u.Password == password).SingleOrDefault(); // && u.IsApproved 

                if (user != null)
                {
                    return LoginState.Success; //user.IsApproved ? LoginState.Success : LoginState.NotApproved;
                }
            }

            return LoginState.Error;
        }

        public static User GetByUsername(string username, UserRelatedData relatedData = UserRelatedData.None)
        {
            using (IUnitOfWork unitOfWork = new cmiUnitOfWork())
            {
                Repository<User> repository = new Repository<User>(unitOfWork);
                return repository.Get(u => u.UserName == username, null, GetIncludeProperties(relatedData)).SingleOrDefault(); // && u.IsApproved 
            }
        }

        public static User GetById(int id, UserRelatedData relatedData = UserRelatedData.None)
        {
            using (IUnitOfWork unitOfWork = new cmiUnitOfWork())
            {
                Repository<User> repository = new Repository<User>(unitOfWork);
                return repository.Get(u => u.ID == id, null, GetIncludeProperties(relatedData)).SingleOrDefault(); // && u.IsApproved 
            }
        }

        public static ProfileItem GetProfileByUserId(int id, int eventNumber, UserRelatedData relatedData = UserRelatedData.None)
        {
            using (IUnitOfWork unitOfWork = new cmiUnitOfWork())
            {
                Repository<User> repositoryUser = new Repository<User>(unitOfWork);
                User outUser = repositoryUser.Get(u => u.ID == id, null, GetIncludeProperties(relatedData)).SingleOrDefault();

                Repository<GroupInvite> repositoryGroupInvite = new Repository<GroupInvite>(unitOfWork);
                List<GroupInvite> outGroupInvite = repositoryGroupInvite.Get(gi => gi.UserMail == outUser.UserName, "Group").ToList();

                Repository<UsersToEvent> repositoryUsersToEvent = new Repository<UsersToEvent>(unitOfWork);
                List<UsersToEvent> outUsersToEvent = repositoryUsersToEvent.Get(ue => ue.UserID == id && ue.Event.StartTime >= DateTime.Today, "Event").Take(eventNumber).ToList();

                return new ProfileItem { user = outUser, groupInvite = outGroupInvite, usersToEvent = outUsersToEvent };
            }
        }

        public static bool Create(User user)
        {
            using (IUnitOfWork unitOfWork = new cmiUnitOfWork())
            {
                Repository<User> repository = new Repository<User>(unitOfWork);
                if (!repository.Get(u => u.UserName == user.UserName).Any())
                {
                    user.UnitState = UnitState.Added;
                    user = repository.Create(user);
                    unitOfWork.Commit();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static bool Update(User user)
        {
            bool successful = false;
            if (user == null) return successful;

            using (IUnitOfWork unitOfWork = new cmiUnitOfWork())
            {
                Repository<User> repository = new Repository<User>(unitOfWork);
                user.UnitState = UnitState.Modified;
                repository.Update(user);
                unitOfWork.Commit();
                successful = true;
            }
            return successful;
        }

        public static bool Delete(User user)
        {
            bool successful = false;
            if (user == null) return successful;

            using (IUnitOfWork unitOfWork = new cmiUnitOfWork())
            {
                Repository<User> repository = new Repository<User>(unitOfWork);
                user.UnitState = UnitState.Deleted;
                //repository.Delete(user.ID);
                repository.Delete(user);
                unitOfWork.Commit();
                successful = true;
            }
            return successful;
        }

        public static bool Delete(int id)
        {
            bool successful = false;
            User user = GetById(id, UserRelatedData.All);
            if (user == null) return successful;


            using (IUnitOfWork unitOfWork = new cmiUnitOfWork())
            {
                Repository<User> repository = new Repository<User>(unitOfWork);
                user.UnitState = UnitState.Deleted;

                foreach (Group item in user.Groups)
                {
                    DAL.DalHelper<GroupInvite>.RemoveCollection(item.GroupInvites);

                    foreach (var eventItem in item.Events)
                    {
                        DAL.DalHelper<UsersToEvent>.RemoveCollection(eventItem.UsersToEvents);
                        eventItem.UnitState = UnitState.Deleted;
                    }

                    DAL.DalHelper<UsersToGroup>.RemoveCollection(item.UsersToGroups);
                }

                foreach (Event item in user.Events)
                {
                    DAL.DalHelper<UsersToEvent>.RemoveCollection(item.UsersToEvents);
                    item.UnitState = UnitState.Deleted;
                }

                DAL.DalHelper<UsersToGroup>.RemoveCollection(user.UsersToGroups);

                DAL.DalHelper<UsersToEvent>.RemoveCollection(user.UsersToEvents);
                user.UnitState = UnitState.Deleted;
                repository.Attach(user);
                unitOfWork.Commit();
                successful = true;
            }
            return successful;
        }

        private static string GetIncludeProperties(UserRelatedData includes)
        {
            string includeProperties = "";
            switch (includes)
            {
                case UserRelatedData.None:
                    break;
                case UserRelatedData.Type:
                    includeProperties = @"UserType";
                    break;
                case UserRelatedData.Events:
                    includeProperties = @"UsersToEvents";
                    break;
                case UserRelatedData.Groups:
                    includeProperties = @"UsersToGroups.Group";
                    break;
                case UserRelatedData.GroupsEvents:
                    includeProperties = @"Groups,Events";
                    break;
                case UserRelatedData.UsersToEventsGroups:
                    includeProperties = @"UsersToEvents.Event,UsersToGroups.Group";
                    break;
                case UserRelatedData.All:
                    //includeProperties = @"UsersToEvents,UsersToGroups,UserType,Events,Events.UsersToEvents,Groups,Groups.GroupInvites,Groups.UsersToGroups";
                    includeProperties = @"Events.UsersToEvents,Groups.GroupInvites,Groups.Events.UsersToEvents,Groups.UsersToGroups,UsersToEvents,UsersToGroups";
                    break;

                    

                default:
                    break;
            }

            return includeProperties;
        }

        public enum UserRelatedData
        {
            None,
            Type,
            Events,
            Groups,
            GroupsEvents,
            UsersToEventsGroups,
            All
        }

    }
}
