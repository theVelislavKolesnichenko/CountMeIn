using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DAL;
using DML;
using PagedList;

namespace BLL
{
    public static class EventManager
    {
        public static DML.Event GetEventByID(int id, EventRelatedData relatedData = EventRelatedData.None)
        {
            using (IUnitOfWork unitOfWork = new cmiUnitOfWork())
            {
                Repository<Event> repository = new Repository<Event>(unitOfWork);
                Event singleEvent = repository.Get(e => e.ID == id, GetIncludeProperties(relatedData)).SingleOrDefault(); //"UsersToEvents.User"
                return singleEvent;
            }
        }

        public static IPagedList<DML.Event> GetPagedEvents(int pageNumber, int pageSize = int.MaxValue, EventRelatedData relatedData = EventRelatedData.None)
        {
            using (IUnitOfWork unitOfWork = new cmiUnitOfWork())
            {
                Repository<Event> repository = new Repository<Event>(unitOfWork);                
                IPagedList<Event> events = repository.Get(null, GetIncludeProperties(relatedData)).OrderBy(e => e.StartTime).ToPagedList(pageNumber, pageSize);
                return events;
            }
        }

        //public static IPagedList<DML.Event> PreparePagedEvents(int pageNumber, IUnitOfWork unitOfWork, int pageSize = int.MaxValue, EventRelatedData relatedData = EventRelatedData.None)
        //{
        //    Repository<Event> repository = new Repository<Event>(unitOfWork);
        //    IPagedList<Event> events = repository.Get(e => e.IsPublic == true, GetIncludeProperties(relatedData)).OrderBy(e => e.StartTime).ToPagedList(pageNumber, pageSize);
        //    return events;
        //}

        public static IPagedList<DML.Event> GetPagedEventsByUserID(int userID, int pageNumber, int pageSize = int.MaxValue, EventRelatedData relatedData = EventRelatedData.None)
        {
            using (IUnitOfWork unitOfWork = new cmiUnitOfWork())
            {
                Repository<Event> repository = new Repository<Event>(unitOfWork);
                IPagedList<Event> events = repository.Get(e => e.IsPublic == true || e.Group.UsersToGroups.Any(ut2g => ut2g.UserID == userID), GetIncludeProperties(relatedData)).OrderBy(e => e.StartTime).ToPagedList(pageNumber, pageSize);
                return events;
            }
        }

        public static Event Add(DML.Event eventAdd)
        {

            using(IUnitOfWork unitOfWork = new cmiUnitOfWork())
            {
                Repository<Event> repository = new Repository<Event>(unitOfWork);
                eventAdd.UnitState = UnitState.Added;
                Event returnEvent = repository.Create(eventAdd);
                unitOfWork.Commit();
                return returnEvent;
            }
        }

        public static bool Update(DML.Event eventUpdate)
        {
            bool successful = false;
            if (eventUpdate == null) return successful;

            using (IUnitOfWork unitOfWork = new cmiUnitOfWork())
            {
                Repository<Event> repository = new Repository<Event>(unitOfWork);
                eventUpdate.UnitState = UnitState.Modified;
                repository.Update(eventUpdate);
                unitOfWork.Commit();
                successful = true;
            }

            return successful;
        }

        public static bool Remove(Event eventItem)
        {
            bool successful = false;
            if (eventItem == null) return successful;

            using (IUnitOfWork unitOfWork = new cmiUnitOfWork())
            {
                Repository<Event> repository = new Repository<Event>(unitOfWork);
                eventItem.UnitState = UnitState.Deleted;
                repository.Delete(eventItem.ID);
                unitOfWork.Commit();
                successful = true;
            }
            return successful;
        }

        private static string GetIncludeProperties(EventRelatedData includes)
        {
            string includeProperties = "";
            switch (includes)
            {
                case EventRelatedData.None:
                    break;
                case EventRelatedData.User:
                    includeProperties = @"UsersToEvents.User";
                    break;
                case EventRelatedData.UserAndGroup:
                    includeProperties = @"UsersToEvents.User,Group";
                    break;
                default:
                    break;
            }

            return includeProperties;
        }

        public enum EventRelatedData
        {
            None,
            User,
            UserAndGroup
        }

    }
}
