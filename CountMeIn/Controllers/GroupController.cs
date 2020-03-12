using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DML;
using DML.Enums;
using BLL;
using CountMeIn.Core;
using CountMeIn.ViewModels;
using CountMeIn.Configuration;

namespace CountMeIn.Controllers
{
    [Authorize]
    public class GroupController : Controller
    {
        //
        // GET: /Group/GroupCreate
        public ActionResult GroupCreate()
        {
            return View();
        }

        // POST: /Group/GroupCreate
        [HttpPost]
        public ActionResult GroupCreate(GroupItem model)
        {
            ModelState["GroupInviteUserMail"].Errors.Clear();

            if (ModelState.IsValid)
            {
                model.OwnerID = SessionManager.UserID;
                Group outGroup = GroupManager.Create(model.ToGroup());

                if (outGroup != null)
                {
                    UsersToGroup addUsersToGroup = new UsersToGroup();
                    addUsersToGroup.GroupID = outGroup.ID;
                    addUsersToGroup.UserID = outGroup.OwnerID;
                    addUsersToGroup.GroupRoleID = (int)DML.Enums.GroupRoles.Moderator;
                    UsersToGroupManager.Add(addUsersToGroup);
                    return RedirectToAction("GroupDetails", "Group", new { id = outGroup.ID });
                }
                else
                {
                    ModelState.AddModelError("Name", App_GlobalResources.Common.GroupIsCreate);
                    return View(model);
                }

            }

            return View(model);
        }

        public ActionResult GroupDetails(int id, int? currentPage, DML.Enums.MessageId message = DML.Enums.MessageId.Unknow)
        {
            currentPage = currentPage ?? 1;

            GroupEventItems groupEvents = BLL.GroupManager.GetGroupEventItemsByID(id, currentPage.Value, ConfigurationSettings.ItemsPerPage, GroupManager.GroupRelatedData.UsersAndGroupInvite);

            if (groupEvents == null ||
                (!groupEvents.Group.UsersToGroups.Any(u => u.UserID == Core.SessionManager.UserID) && !BLL.PermissionManager.HasPermission(Core.SessionManager.UserID, PermissionsItem.CanEditGroup)))
            {
                return RedirectToAction("NotFound", "Error");
            }

            GroupItem outGroupItem = groupEvents.Group.ToGroupItem();
            outGroupItem.groupEvents = groupEvents.Events;

            Core.SessionManager.BackToGroupID = id;

            ViewBag.Message = CountMeIn.Configuration.ConfigurationSettings.Message(message);

            return View(outGroupItem);
        }

        public ActionResult JoinAnGroup(int groupID, int id)
        {
            UsersToGroup addUsersToGroup = new UsersToGroup();
            addUsersToGroup.GroupID = groupID;
            addUsersToGroup.UserID = Core.SessionManager.UserID;
            addUsersToGroup.GroupRoleID = (int)DML.Enums.GroupRoles.Player;
            UsersToGroupManager.Add(addUsersToGroup);

            GroupInviteManager.Remove(id);

            return RedirectToAction("Profile", "User", new { id = Core.SessionManager.UserID });
        }

        public ActionResult UnsubscribeAnGroup(int userID, int groupID)
        {
            if (ClearFromGroup(userID, groupID))
            {
                return RedirectToAction("Profile", "User", new { id = userID });
            }
            else
            {
                return RedirectToAction("GroupDetails", "Group", new { id = groupID, message = MessageId.GroupOwner });
            }
        }

        public ActionResult RemovalFromGroup(int userID, int groupID)
        {
            if (ClearFromGroup(userID, groupID))
            {
                return RedirectToAction("GroupDetails", "Group", new { id = groupID });
            }
            else
            {
                return RedirectToAction("GroupDetails", "Group", new { id = groupID, message = MessageId.GroupOwner });
            }
        }

        private bool ClearFromGroup(int userID, int groupID)
        {
            bool successful = false;

            if (BLL.GroupManager.GetGroupByID(groupID).OwnerID == userID)
            {
                return successful;
            }
            else
            {
                DML.Group groupEvents = BLL.GroupManager.GetGroupByID(groupID, GroupManager.GroupRelatedData.Events);
                foreach (var e in groupEvents.Events)
                {
                    if (e.StartTime > DateTime.Today && !e.IsPublic)
                    {
                        BLL.UsersToEventManager.Remove(new UsersToEvent { UserID = userID, EventID = e.ID });
                    }
                }
                BLL.UsersToGroupManager.Remove(new UsersToGroup { UserID = userID, GroupID = groupID });
                successful = true;
            }
            return successful;

        }

        public ActionResult AddGroupInvite(GroupItem groupItem, int userID)
        {
            ModelState["Name"].Errors.Clear();
            Group outGroup = GroupManager.GetGroupByID(groupItem.ID, GroupManager.GroupRelatedData.UsersAndGroupInvite);

            if (ModelState.IsValid && !UsersToGroupManager.IsMember(groupItem.GroupInviteUserMail, groupItem.ID))
            {
                GroupInvite groupInvite = new GroupInvite();
                groupInvite.GroupID = groupItem.ID;
                groupInvite.UserMail = groupItem.GroupInviteUserMail;

                string url = Utils.AppUtils.AppDomainPath + ((UserManager.GetByUsername(groupItem.GroupInviteUserMail) != null) ? "User/Login" : "User/Register");

                GroupInviteManager.Add(groupInvite);
                Utils.NotificationManager.SendEmail(//string.Format("{0} {1}", outGroup.User.FirstName, outGroup.User.LastName),
                                                    Core.SessionManager.UserFullName,
                                                    //outGroup.User.UserName,
                                                    User.Identity.Name,
                                                    groupInvite.UserMail,
                                                    App_GlobalResources.Mail.Subject,
                                                    string.Format(App_GlobalResources.Mail.Body, outGroup.User.FirstName, url));

                return RedirectToAction("GroupDetails", new { id = groupItem.ID });
            }

            GroupItem outGroupitem = outGroup.ToGroupItem();
            outGroupitem.GroupInviteUserMail = groupItem.GroupInviteUserMail;
            ModelState.AddModelError("GroupInviteUserMail", App_GlobalResources.Common.UserIsInviteGroup);
            return View("GroupDetails", outGroupitem);
        }

        public ActionResult CancelGroupInvite(int id)
        {
            GroupInviteManager.Remove(id);
            return RedirectToAction("Profile", "User", new { id = SessionManager.UserID });
        }

        public ActionResult ClearGroupInvite(int id, int groupID)
        {
            GroupInviteManager.Remove(id);
            return RedirectToAction("GroupDetails", "Group", new { id = groupID });
        }

        //[HttpPost]
        public ActionResult OwnerChange(int id, int ownerID)
        {
            if (BLL.GroupManager.IsOwner(Core.SessionManager.UserID, id))
            {
                Group group = BLL.GroupManager.GetGroupByID(id);
                if(group != null)
                {
                    group.OwnerID = ownerID;
                    BLL.GroupManager.Update(group);
                }
            }

            return RedirectToAction("GroupDetails", "Group", new { id = id });
        }

        //[HttpPost]
        public ActionResult UserRoleChange(int id, int userID, int roleID)
        {
            //if (BLL.GroupManager.IsOwner(Core.SessionManager.UserID, id))
            {
                UsersToGroup userToGroup = BLL.UsersToGroupManager.GetByUserIDGroupID(userID, id);
                if (userToGroup != null)
                {
                    userToGroup.GroupRoleID = roleID;
                    BLL.UsersToGroupManager.Update(userToGroup);
                }
            }


            return Content("Променихте ролята на потребител");
            //return Json(new { msg = "Променихте Собственика на групата" });
        }

        public ActionResult RemoveGroup(int id)
        {
            BLL.GroupManager.Remove(id);
            return RedirectToAction("Profile", "User", new { id = Core.SessionManager.UserID });
        }
    }

}
