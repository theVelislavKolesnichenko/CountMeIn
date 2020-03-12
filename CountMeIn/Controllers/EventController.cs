using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DML;
using BLL;
using CountMeIn.Configuration;
using PagedList;
using CountMeIn.ViewModels;
using CountMeIn.Core;
using System.Text.RegularExpressions;

namespace CountMeIn.Controllers
{
    [Authorize]
    public class EventController : Controller
    {
        //
        // GET: /Event/
        public ActionResult EventSingle(int id)
        {
            try
            {
                //Response.StatusCode = 404;
                Event singleEvent = EventManager.GetEventByID(id, EventManager.EventRelatedData.User);

                bool hasPermission = PermissionManager.HasPermission(Core.SessionManager.UserID, DML.Enums.PermissionsItem.CanEditEvent);

                if (singleEvent == null || (!UsersToGroupManager.IsMember(Core.SessionManager.UserID, singleEvent.GroupID) && !singleEvent.IsPublic && !hasPermission))
                {
                    return RedirectToAction("NotFound", "Error");
                    //Response.Redirect("~/Error/NotFound");
                }

                EventItem singleEventItem = singleEvent.ToEventItem();

                if (Request.UrlReferrer != null)
                {
                    Core.SessionManager.ReturnUrl = Request.UrlReferrer.ToString();
                }

                return View(singleEventItem);
            }
            catch (Exception ex)
            {
                return View("Error");
            }
        }

        //[AllowAnonymous]
        public ActionResult EventsList(int? currentPage)
        {
            currentPage = currentPage ?? 1;
            //IPagedList<Event> events = EventManager.GetPagedEvents(currentPage.Value, ConfigurationSettings.ItemsPerPage, EventManager.EventRelatedData.User);

            IPagedList<Event> events;

            if (BLL.PermissionManager.GetPermissionForUser(User.Identity.Name).Equals(DML.Enums.UserType.Admin.ToString()))
            {
                events = EventManager.GetPagedEvents(currentPage.Value, ConfigurationSettings.ItemsPerPage, EventManager.EventRelatedData.UserAndGroup);
            }
            else 
            {
                events = EventManager.GetPagedEventsByUserID(Core.SessionManager.UserID, currentPage.Value, ConfigurationSettings.ItemsPerPage, EventManager.EventRelatedData.UserAndGroup);
            }
            

            
            //.Where(e => e.StartTime > DateTime.Today).ToPagedList(currentPage.Value, ConfigurationSettings.ItemsPerPage)
            if (events == null)
            {
                return RedirectToAction("Error", "Error");
            }

            //CountMeIn.Core.SessionManager.EventListPageNumber = currentPage;

            return View(events);
            
        }

        public ActionResult JoinAnEvent(int id, int groupID)
        {
            if (!BLL.UsersToGroupManager.IsMember(Core.SessionManager.UserID, groupID) && !BLL.EventManager.GetEventByID(id).IsPublic)
            {
                return RedirectToAction("NotFound", "Error");
            }

            DML.UsersToEvent usersToEvent = new DML.UsersToEvent();
            usersToEvent.EventID = id;
            usersToEvent.UserID = Core.SessionManager.UserID;

            Event events = BLL.EventManager.GetEventByID(id, EventManager.EventRelatedData.User);

            if (events.UsersToEvents.Count < events.MaxUsers)
            {
                 BLL.UsersToEventManager.Add(usersToEvent);
            }
           
            return RedirectToAction("EventSingle", "Event", new { id = id });
        }

        public ActionResult UnsubscribeAnEvent(int userId, int EventId)
        {
            DML.UsersToEvent usersToEvent = new DML.UsersToEvent();
            usersToEvent.EventID = EventId;
            usersToEvent.UserID = userId; //BLL.UserManager.GetByUsername(User.Identity.Name, BLL.UserManager.UserRelatedData.None).ID;
            BLL.UsersToEventManager.Remove(usersToEvent);
            return RedirectToAction("EventSingle", "Event", new { id = EventId });
        }

        public ActionResult EventCreate(int id)
        {
            if (!BLL.PermissionManager.HasPermission(Core.SessionManager.UserID, DML.Enums.PermissionsItem.CanCreateEvent) 
                && !BLL.GroupManager.IsOwner(Core.SessionManager.UserID, id)
                && !BLL.UsersToGroupManager.IsModerator(CountMeIn.Core.SessionManager.UserID, id))
            {
                return RedirectToAction("NotFound", "Error");
            }

            Core.SessionManager.BackToGroupID = id;

            return View();
        }

        [HttpPost]
        public ActionResult EventCreate(EventItem eventItem)
        {
            if (!BLL.PermissionManager.HasPermission(Core.SessionManager.UserID, DML.Enums.PermissionsItem.CanCreateEvent)
               && !BLL.GroupManager.IsOwner(Core.SessionManager.UserID, eventItem.GroupID)
                && !BLL.UsersToGroupManager.IsModerator(CountMeIn.Core.SessionManager.UserID, eventItem.GroupID))
            {
                return RedirectToAction("NotFound", "Error");
            }

            if (eventItem.HoursOffset == 0)
            {
                ModelState["HoursOffset"].Errors.Clear();
            }

            try
            {
                if ((string.IsNullOrEmpty(eventItem.StartDate) && string.IsNullOrEmpty(eventItem.EndDate)) || DateTime.Parse(eventItem.StartDate) > DateTime.Parse(eventItem.EndDate))
                {
                    ModelState.AddModelError("EndDate", "Крайната дата не може да е преди Началната");
                }

                if ((string.IsNullOrEmpty(eventItem.StartHour) && string.IsNullOrEmpty(eventItem.EndHour)) || DateTime.Parse(eventItem.StartHour) > DateTime.Parse(eventItem.EndHour))
                {
                    ModelState.AddModelError("EndHour", "Крайния час не може да е преди Началния");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("StartDate", ex.Source);
            }

            if (ModelState.IsValid)
            {
                Event inputEvent = eventItem.ToEvent();
                inputEvent.OwnerID = SessionManager.UserID;
                inputEvent.GroupID = eventItem.GroupID; //SessionManager.GroupID;
                BLL.EventManager.Add(inputEvent);
                return RedirectToAction("EventsList", "Event");
            }
           
            return View(eventItem);
        }

        public ActionResult EventEdit(int id)
        {

            //if (!BLL.PermissionManager.HasPermission(Core.SessionManager.UserID, DML.Enums.PermissionsItem.CanEditEvent) 
            //    && !BLL.GroupManager.IsOwner(Core.SessionManager.UserID, Core.SessionManager.GroupID))
            //{
            //    return RedirectToAction("NotFound", "Error");
            //}

            try
            {
                //Response.StatusCode = 404;
                Event singleEvent = EventManager.GetEventByID(id, EventManager.EventRelatedData.User);

                if (singleEvent == null || 
                    (!(Core.SessionManager.UserID == singleEvent.OwnerID) 
                    && !BLL.PermissionManager.HasPermission(Core.SessionManager.UserID, DML.Enums.PermissionsItem.CanEditEvent)
                    && !BLL.GroupManager.IsOwner(Core.SessionManager.UserID, singleEvent.GroupID)))
                {
                    return RedirectToAction("NotFound", "Error");
                    //Response.Redirect("~/Error/NotFound");
                }

                EventItem singleEventItem = singleEvent.ToEventItem();

                return View(singleEventItem);
            }
            catch (Exception ex)
            {
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult EventEdit(EventItem eventItem)
        {
            if (!BLL.PermissionManager.HasPermission(Core.SessionManager.UserID, DML.Enums.PermissionsItem.CanEditEvent)
                 && !(Core.SessionManager.UserID == eventItem.OwnerID) && !BLL.GroupManager.IsOwner(Core.SessionManager.UserID, eventItem.GroupID))
            {
                return RedirectToAction("NotFound", "Error");
            }

            if (eventItem.HoursOffset == 0)
            {
                ModelState["HoursOffset"].Errors.Clear();
            }

            if (eventItem.MaxUsers < BLL.EventManager.GetEventByID(eventItem.ID, EventManager.EventRelatedData.User).UsersToEvents.Count)
            {
                ModelState["MaxUsers"].Errors.Add(App_GlobalResources.Common.MaxUsersFull);
            }

            //if ((string.IsNullOrEmpty(eventItem.StartDate) && string.IsNullOrEmpty(eventItem.EndDate)) || DateTime.Parse(eventItem.StartDate) > DateTime.Parse(eventItem.EndDate))
            //{
            //    //ModelState.AddModelError("StartDate", "");
            //    ModelState.AddModelError("EndDate", "Крайната дата не може да е преди Началната");
            //}

            //if ((string.IsNullOrEmpty(eventItem.StartHour) && string.IsNullOrEmpty(eventItem.EndHour)) || DateTime.Parse(eventItem.StartHour) > DateTime.Parse(eventItem.EndHour))
            //{
            //    ModelState.AddModelError("EndHour", "Крайния час не може да е преди Началния");
            //}

            if (ModelState.IsValid)
            {
                Event inputEvent = eventItem.ToEvent();
                
                BLL.EventManager.Update(inputEvent);
                return RedirectToAction("EventSingle", "Event", new { id = eventItem.ID });
            }

            return View(eventItem);
        }

    }
}
