using DAL;
using DML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PagedList;

namespace BLL
{
    public static class UsersToEventManager
    {
        public static bool Add(UsersToEvent usersToEvent)
        {
            bool successful = false;
            if (usersToEvent == null) return successful;

            using (IUnitOfWork unitOfWork = new cmiUnitOfWork())
            {
                Repository<UsersToEvent> repository = new Repository<UsersToEvent>(unitOfWork);
                if (!repository.Get(ute => ute.UserID == usersToEvent.UserID && ute.EventID == usersToEvent.EventID).Any())
                {
                    usersToEvent.UnitState = UnitState.Added;
                    usersToEvent = repository.Create(usersToEvent);
                    unitOfWork.Commit();
                    successful = true;
                }
                else 
                { 
                    successful = false;
                }
            }

            return successful;
        }

        public static bool Remove(UsersToEvent usersToEvent)
        {
            bool successful = false;
            if (usersToEvent == null) return successful;

            using (IUnitOfWork unitOfWork = new cmiUnitOfWork())
            {
                Repository<UsersToEvent> repository = new Repository<UsersToEvent>(unitOfWork);

                usersToEvent = GetEventID(usersToEvent.UserID, usersToEvent.EventID);
                
                if (usersToEvent != null)
                {
                    usersToEvent.UnitState = UnitState.Deleted;
                    repository.Delete(usersToEvent);
                    unitOfWork.Commit();
                    successful = true;
                }

            }
            return successful;
        }

        public static UsersToEvent GetEventID(int UserID, int EventID)
        {
            using (IUnitOfWork unitOfWork = new cmiUnitOfWork())
            {
                Repository<UsersToEvent> repository = new Repository<UsersToEvent>(unitOfWork);
                UsersToEvent userToEvent = repository.Get(x => x.EventID == EventID && x.UserID == UserID).SingleOrDefault();
                return userToEvent;
            }
        }

        public static List<UsersToEvent> GetPagedUsersToEventsByUserID(int userID, int pageNumber, int pageSize = int.MaxValue, UsersToEventRelatedData relatedData = UsersToEventRelatedData.None)
        {
            using (IUnitOfWork unitOfWork = new cmiUnitOfWork())
            {
                Repository<UsersToEvent> repository = new Repository<UsersToEvent>(unitOfWork);
                List<UsersToEvent> events = repository.Get(e => e.UserID == userID && e.Event.StartTime > DateTime.Today, GetIncludeProperties(relatedData)).OrderBy(e => e.Event.StartTime).ToPagedList(pageNumber, pageSize).ToList();
                return events;
            }
        }

        private static string GetIncludeProperties(UsersToEventRelatedData includes)
        {
            string includeProperties = "";
            switch (includes)
            {
                case UsersToEventRelatedData.None:
                    break;
                case UsersToEventRelatedData.Events:
                    includeProperties = @"Event";
                    break;
                default:
                    break;
            }

            return includeProperties;
        }

        public enum UsersToEventRelatedData
        {
            None,
            Events,
        }

    }
}
