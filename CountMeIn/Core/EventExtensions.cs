using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DML;
using CountMeIn.ViewModels;

namespace CountMeIn.Core
{
    public static class EventExtensions
    {
        public static EventItem ToEventItem(this Event singleEvent)
        {
            EventItem singleEventItem = new EventItem();
            singleEventItem.ID = singleEvent.ID;
            singleEventItem.Name = singleEvent.Name;
            singleEventItem.Location = singleEvent.Location;
            
            singleEventItem.StartDate = singleEvent.StartTime.Date.ToString("dd.MM.yyyy"); //singleEvent.StartTime;  
            singleEventItem.StartHour = singleEvent.StartTime.ToString("HH:mm"); //singleEvent.StartTime;

            singleEventItem.EndDate = singleEvent.EndTime.Date.ToString("dd.MM.yyyy"); //singleEvent.EndTime.Date;
            singleEventItem.EndHour = singleEvent.EndTime.ToString("HH:mm"); //singleEvent.EndTime;
            
            singleEventItem.HoursOffset = singleEvent.HoursOffset;
            singleEventItem.MaxUsers = singleEvent.MaxUsers;
            //singleEventItem.Users = singleEvent.UsersToEvents.ToList();
            //singleEventItem.UsersCount = singleEvent.UsersToEvents.Count;

            singleEventItem.OwnerID = singleEvent.OwnerID;
            singleEventItem.GroupID = singleEvent.GroupID;

            singleEventItem.IsPublic = singleEvent.IsPublic;

            singleEventItem.Users = new List<UserItem>();

            foreach (UsersToEvent utu in singleEvent.UsersToEvents)
            {
                singleEventItem.Users.Add(utu.User.ToUserItem());
            }

            return singleEventItem;
        }

        public static Event ToEvent(this EventItem inputEvent)
        {
            return new Event()
            {
                ID = inputEvent.ID,
                Name = inputEvent.Name,
                Location = inputEvent.Location,
                //StartTime = inputEvent.StartDate + inputEvent.StartHour.TimeOfDay,
                StartTime = DateTime.Parse(inputEvent.StartDate + " " + inputEvent.StartHour),
                //EndTime = inputEvent.EndDate + inputEvent.EndHour.TimeOfDay,
                EndTime = DateTime.Parse(inputEvent.EndDate + " " + inputEvent.EndHour),
                HoursOffset = inputEvent.HoursOffset,
                MaxUsers = inputEvent.MaxUsers,
                UnitState = UnitState.Unchanged,
                GroupID = inputEvent.GroupID,
                OwnerID = inputEvent.OwnerID,
                IsPublic = inputEvent.IsPublic
            };
        }
    }
}