using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DAL;
using DML;
using PagedList;


namespace BLL
{
    public static class GroupManager
    {
        public static Group GetGroupByID(int id, GroupRelatedData relatedData = GroupRelatedData.None)
        {
            using (IUnitOfWork unitOfWork = new cmiUnitOfWork())
            {   
                Repository<Group> repository = new Repository<Group>(unitOfWork);
                Group singleGroup = repository.Get(g => g.ID == id, GetIncludeProperties(relatedData)).SingleOrDefault();
                return singleGroup;
            }
        }

        public static GroupEventItems GetGroupEventItemsByID(int id, int pageNumber, int pageSize = int.MaxValue, GroupRelatedData relatedData = GroupRelatedData.None)
        {
            using (IUnitOfWork unitOfWork = new cmiUnitOfWork())
            {
                GroupEventItems groupEvents = new GroupEventItems();
                Repository<Group> repositoryGroup = new Repository<Group>(unitOfWork);
                groupEvents.Group = repositoryGroup.Get(g => g.ID == id, GetIncludeProperties(relatedData)).SingleOrDefault();

                Repository<Event> repositoryEvent = new Repository<Event>(unitOfWork);
                groupEvents.Events = repositoryEvent.Get(e => e.GroupID == id, "UsersToEvents").OrderBy(e => e.StartTime).ToPagedList(pageNumber, pageSize);

                return groupEvents;
            }
        }

        public static bool IsOwner(int ownerID, int groupID)
        {
            using (IUnitOfWork unitOfWork = new cmiUnitOfWork())
            {
                Repository<Group> repository = new Repository<Group>(unitOfWork);
                return repository.Contains(g => g.ID == groupID && g.OwnerID == ownerID);
            }
        }

        public static Group Create(DML.Group groupAdd)
        {
            using (IUnitOfWork unitOfWork = new cmiUnitOfWork())
            {
                Repository<Group> repository = new Repository<Group>(unitOfWork);
                if (!repository.Get(g => g.Name == groupAdd.Name).Any())
                {
                    groupAdd.UnitState = UnitState.Added;
                    Group returnGroup = repository.Create(groupAdd);
                    unitOfWork.Commit();
                    return returnGroup;
                }
                else 
                {
                    return null;
                }
            }
        }

        public static bool Update(Group group)
        {
            bool successful = false;
            if (group == null) return successful;

            using (IUnitOfWork unitOfWork = new cmiUnitOfWork())
            {
                Repository<Group> repository = new Repository<Group>(unitOfWork);
                group.UnitState = UnitState.Modified;
                //repository.Update(user);
                repository.Attach(group);
                unitOfWork.Commit();
                successful = true;
            }
            return successful;
        }

        public static bool Remove(Group group)
        {
            bool successful = false;
            if (group == null) return successful;

            using (IUnitOfWork unitOfWork = new cmiUnitOfWork())
            {
                Repository<Group> repository = new Repository<Group>(unitOfWork);
                group.UnitState = UnitState.Deleted;
                //repository.Delete(group.ID);
                unitOfWork.Commit();
                successful = true;
            }
            return successful;
        }

        public static bool Remove(int id)
        {
            bool successful = false;
            Group group = GetGroupByID(id, GroupRelatedData.ALL);
            if (group == null) return successful;

            using (IUnitOfWork unitOfWork = new cmiUnitOfWork())
            {
                PrepareRemoveGroup(unitOfWork, group);
                
                
                successful = true;
            }
            return successful;
        }

        private static void PrepareRemoveGroup(IUnitOfWork unitOfWork, Group group)
        {
            Repository<Group> repository = new Repository<Group>(unitOfWork);

            DAL.DalHelper<GroupInvite>.RemoveCollection(group.GroupInvites);
            
            foreach(var eventItem in group.Events)
            {
                DAL.DalHelper<UsersToEvent>.RemoveCollection(eventItem.UsersToEvents);
                eventItem.UnitState = UnitState.Deleted;
            }

            DAL.DalHelper<UsersToGroup>.RemoveCollection(group.UsersToGroups);
            
            group.UnitState = UnitState.Deleted;
            repository.Attach(group);
            unitOfWork.Commit();
            //repository.Delete(group);
        }

        private static string GetIncludeProperties(GroupRelatedData includes)
        {
            string includeProperties = "";
            switch (includes)
            {
                case GroupRelatedData.None:
                    break;
                case GroupRelatedData.Owner:
                    includeProperties = @"User";
                    break;
                case GroupRelatedData.Users:
                    includeProperties = @"UsersToGroups.User";
                    break;
                case GroupRelatedData.UsersAndGroupInvite:
                    includeProperties = @"UsersToGroups.User,GroupInvites";
                    break;
                case GroupRelatedData.Events:
                    includeProperties = @"Events";
                    break;
                case GroupRelatedData.ALL:
                    includeProperties = @"Events.UsersToEvents,UsersToGroups,GroupInvites";
                    break;
                default:
                    break;
            }
            return includeProperties;
        }

        public enum GroupRelatedData
        {
            None,
            Owner,
            Users,
            Events,
            UsersAndGroupInvite,
            ALL
        }

    }
}
