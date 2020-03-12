using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL;
using DML;
using DML.Enums;
using System.Web.Security;
using CountMeIn.ViewModels;
using Utils;
using CountMeIn.Core;
using System.Web.Configuration;
using CountMeIn.Configuration;
using PagedList;
using System.Drawing;

namespace CountMeIn.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        //
        // GET: /User/
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (CountMeIn.Core.SessionManager.IsAuthenticated)
            {
                return RedirectToAction("Profile", new { id = Core.SessionManager.UserID });
            }

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(UserItem user, string returnUrl)
        {
            ModelState["NewPassword"].Errors.Clear();
            ModelState["ConfirmPassword"].Errors.Clear();
            ModelState["FirstName"].Errors.Clear();
            ModelState["LastName"].Errors.Clear();

            if (ModelState.IsValid)
                if (user.Password != null)
                {
                    User outUser;
                    LoginState status = BLL.UserManager.Validate(user.UserName, Utils.MD5Encrypt.MD5Hash(user.Password), out outUser);
                    if (status == LoginState.Success)
                    {
                        Core.SessionManager.LoadUserSession(outUser);
                        FormsAuthentication.SetAuthCookie(outUser.UserName, true);
                        if (!string.IsNullOrEmpty(returnUrl))
                        {
                            Response.Redirect(Server.UrlDecode(returnUrl), true);
                        }
                        //string returnUrl = Request.QueryString["ReturnUrl"];

                        return RedirectToAction("Profile", "User", new { id = outUser.ID, message = DML.Enums.MessageId.LoginSuccess });
                    }
                }

            ModelState["UserName"].Errors.Clear();
            ModelState["Password"].Errors.Clear();

            ModelState.AddModelError("", App_GlobalResources.Common.UserNamePassword);
            return View(user);
        }

        public ActionResult Logon(string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl))
            {
                Response.Redirect(Server.UrlDecode(returnUrl), true);
            }
            return View();
        }

        public ActionResult LogOff()
        {
            Core.SessionManager.ClearUserSession();
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Register(UserItem user)
        {
            ModelState["NewPassword"].Errors.Clear();
            ModelState["ConfirmPassword"].Errors.Clear();
            if (string.IsNullOrWhiteSpace(user.Password) || string.IsNullOrWhiteSpace(user.ConfirmPassword) || !user.Password.Equals(user.ConfirmPassword))
            {
                ModelState.AddModelError("ConfirmPassword", App_GlobalResources.Common.PosswordSame);
            }
            else if (ModelState.IsValid)
            {
                User newUser = user.ToUser();
                newUser.Password = Utils.MD5Encrypt.MD5Hash(newUser.Password);
                newUser.UserTypeID = (int)DML.Enums.UserType.Player;

                if (BLL.UserManager.Create(newUser))
                {
                    Core.SessionManager.LoadUserSession(newUser);
                    FormsAuthentication.SetAuthCookie(user.UserName, true);
                    return RedirectToAction("Profile", "User", new { id = newUser.ID, message = MessageId.ResisterSuccess });
                }
                else
                {
                    ModelState.AddModelError("UserName", App_GlobalResources.Common.InvalidData);
                }
            }
            return View(user);
        }

        public ActionResult Profile(int id, MessageId message = DML.Enums.MessageId.Unknow, int? groupPage = 1)
        {
            //User outUser = BLL.UserManager.GetById(id, UserManager.UserRelatedData.UsersToEventsGroups);

            DML.ProfileItem outProfile = BLL.UserManager.GetProfileByUserId(id, ConfigurationSettings.ItemsPerPage, UserManager.UserRelatedData.Groups);

            if (outProfile == null)
            {
                if (id == SessionManager.UserID)
                {
                    SessionManager.ClearUserSession();
                }
                return RedirectToAction("NotFound", "Error");
            }

            UserItem userItem = outProfile.user.ToUserItem();
            userItem.GroupInviteItems = outProfile.groupInvite;
            userItem.JointEvents = (from value in outProfile.usersToEvent
                                    select value.Event).OrderBy(e => e.StartTime).ToList();


            //userItem.GroupInviteItems = BLL.GroupInviteManager.GetGroupInviteByUserName(userItem.UserName, GroupInviteManager.GroupInviteRelatedData.Group);

            //userItem.JointEvents = (from value in BLL.UsersToEventManager.GetPagedUsersToEventsByUserID(id, 1, ConfigurationSettings.ItemsPerPage, UsersToEventManager.UsersToEventRelatedData.Events)
            //                        select value.Event).OrderBy(e => e.StartTime).ToList();

            if (message != MessageId.Unknow && id == Core.SessionManager.UserID)
            {
                Core.SessionManager.LoadUserSession(outProfile.user);
            }

            ViewBag.Message = CountMeIn.Configuration.ConfigurationSettings.Message(message);

            return View(userItem);
        }

        public ActionResult Edit(int id)
        {

            ViewBag.EditUserNamePasswordValidator = true;
            ViewBag.EditChangePasswordValidator = true;
            ViewBag.EditFullNameValidator = true;

            if (!(id == Core.SessionManager.UserID || BLL.PermissionManager.HasPermission(CountMeIn.Core.SessionManager.UserID, DML.Enums.PermissionsItem.CanEditUsers)))
            {
                return RedirectToAction("NotFound", "Error");
            }

            User outUser = BLL.UserManager.GetById(id);

            if (outUser == null)
            {
                return RedirectToAction("NotFound", "Error");
            }

            UserItem userItem = outUser.ToUserItem();
            userItem.Password = userItem.ConfirmPassword = string.Empty;

            return View(userItem);
        }

        [HttpPost]
        public ActionResult Edit(UserItem userItem, int id)
        {
            User outUser = BLL.UserManager.GetById(id);

            ViewBag.EditUserNamePasswordValidator = true;
            ViewBag.EditChangePasswordValidator = true;
            ViewBag.EditFullNameValidator = true;

            if (string.IsNullOrEmpty(userItem.NewPassword) && string.IsNullOrEmpty(userItem.ConfirmPassword))
            {
                ModelState["NewPassword"].Errors.Clear();
                ModelState["ConfirmPassword"].Errors.Clear();
                ViewBag.EditChangePasswordValidator = false;
            }

            if (userItem.UserName.Equals(outUser.UserName))
            {
                ViewBag.EditUserNamePasswordValidator = false;
            }

            if (outUser.FirstName.Equals(userItem.FirstName) && outUser.LastName.Equals(userItem.LastName))
            {
                ViewBag.EditFullNameValidator = false;
            }

            if (!ViewBag.EditUserNamePasswordValidator && !ViewBag.EditChangePasswordValidator)
            {
                ModelState["Password"].Errors.Clear();
                if (!ViewBag.EditFullNameValidator)
                {
                    userItem.ProfileImageName = outUser.ProfileImageName;
                    return View(userItem);
                }
            }

            if (ModelState.IsValid)
            {
                if (ViewBag.EditFullNameValidator)
                {
                    outUser.FirstName = userItem.FirstName;
                    outUser.LastName = userItem.LastName;
                }

                if (ViewBag.EditUserNamePasswordValidator)
                {
                    outUser.UserName = userItem.UserName;
                }

                if (ViewBag.EditChangePasswordValidator)
                {
                    outUser.Password = Utils.MD5Encrypt.MD5Hash(userItem.NewPassword);
                }

                if (!BLL.UserManager.Update(outUser))
                {
                    return RedirectToAction("NotFound", "Error");
                }

                return RedirectToAction("Profile", "User", new { id = id, message = MessageId.ChangesSuccessfully });
            }
            else
            {
                if (ViewBag.EditChangePasswordValidator)
                {
                    ModelState.AddModelError("NewPassword", App_GlobalResources.Common.RequiresPassword);
                    ModelState.AddModelError("ConfirmPassword", App_GlobalResources.Common.RequiresPassword);
                }

                if (ViewBag.EditUserNamePasswordValidator)
                {
                    ModelState.AddModelError("UserName", App_GlobalResources.Common.RequiresPassword);
                }
            }

            userItem.ProfileImageName = outUser.ProfileImageName;
            return View(userItem); //RedirectToAction("Profile", "User", new { id = id, message = MessageId.ChangeFullNameSuccess });
        }

        [HttpPost]
        public ActionResult ChangeFullName(UserItem userItem, int id)
        {
            ViewBag.EditUserNamePasswordValidator = false;
            ViewBag.EditChangePasswordValidator = false;
            ViewBag.EditFullNameValidator = true;

            //if (id != Core.SessionManager.UserID)
            //{
            //    return RedirectToAction("NotFound", "Error");
            //}Core.SessionManager.UserID

            User outUser = BLL.UserManager.GetById(id);

            //if (!ModelState.IsValid)
            //{
            //    userItem = outUser.ToUserItem();
            //    userItem.Password = userItem.ConfirmPassword = string.Empty;
            //    return View("Edit", userItem);
            //}

            //if (userItem.FirstName == null)
            //{
            //     outUser.FirstName = string.Empty;
            //}
            //else
            //{
            //     outUser.FirstName = userItem.FirstName;
            //}
            //if (userItem.LastName == null)
            //{
            //    outUser.LastName = string.Empty;
            //}
            //else
            //{
            //    outUser.LastName = userItem.LastName;
            //}

            if (string.IsNullOrEmpty(userItem.FirstName) || string.IsNullOrEmpty(userItem.LastName))
            {
                return View("Edit", userItem);
            }
            else
            {
                outUser.FirstName = userItem.FirstName;
                outUser.LastName = userItem.LastName;
            }

            if (!BLL.UserManager.Update(outUser))
            {
                return RedirectToAction("NotFound", "Error");
            }

            return RedirectToAction("Profile", "User", new { id = id, message = MessageId.ChangeFullNameSuccess });
        }

        [HttpPost]
        public ActionResult ChangeUserName(UserItem userItem, int id)
        {

            ViewBag.EditUserNamePasswordValidator = true;
            ViewBag.EditChangePasswordValidator = false;
            ViewBag.EditFullNameValidator = false;

            User outUser = BLL.UserManager.GetById(id);

            if (PermissionManager.HasPermission(CountMeIn.Core.SessionManager.UserID, PermissionsItem.CanEditUsers) || outUser.Password == Utils.MD5Encrypt.MD5Hash(userItem.Password))
            {
                outUser.UserName = userItem.UserName;
            }
            else
            {
                userItem = outUser.ToUserItem();
                userItem.Password = userItem.ConfirmPassword = string.Empty;
                ModelState.AddModelError("Password", App_GlobalResources.Common.PasswordInvalid);
                return View("Edit", userItem);
            }

            if (!BLL.UserManager.Update(outUser))
            {
                return RedirectToAction("NotFound", "Error");
            }

            return RedirectToAction("Profile", "User", new { id = id, message = MessageId.ChangeUserNameSuccess });
        }

        //[HttpPost]
        //public ActionResult ChangePassword(EditItem editItem)
        //{
        //    bool passwordError = true;

        //    User outUser = BLL.UserManager.GetById(Core.SessionManager.UserID);

        //    if (outUser.Password != Utils.MD5Encrypt.MD5Hash(editItem.OldPassword))
        //    {
        //        ModelState.AddModelError("OldPassword", "Невалидна парола");
        //        passwordError = false;
        //    }

        //    if (string.IsNullOrWhiteSpace(editItem.NewPassword) || string.IsNullOrWhiteSpace(editItem.ConfirmPassword) || !editItem.NewPassword.Equals(editItem.ConfirmPassword))
        //    {
        //        ModelState.AddModelError("NewPassword", "Некоректна парола!");
        //        ModelState.AddModelError("ConfirmPassword", "Некоректна пароли!");
        //        passwordError = false;
        //    }

        //    if (passwordError)
        //    {
        //        outUser.Password = Utils.MD5Encrypt.MD5Hash(editItem.NewPassword);
        //    }

        //    if (!passwordError || !BLL.UserManager.Update(outUser))
        //    {
        //        UserItem userItem = outUser.ToUserItem();
        //        userItem.Password = userItem.ConfirmPassword = string.Empty;
        //        return View("Edit", userItem);
        //    }

        //    return RedirectToAction("Profile/" + Core.SessionManager.UserID, "User");
        //}

        [HttpPost]
        public ActionResult ChangePassword(UserItem userItem, int id)
        {
            bool passwordError = false;
            ViewBag.EditUserNamePasswordValidator = false;
            ViewBag.EditChangePasswordValidator = true;
            ViewBag.EditFullNameValidator = false;

            User outUser = BLL.UserManager.GetById(id);

            if (!PermissionManager.HasPermission(CountMeIn.Core.SessionManager.UserID, PermissionsItem.CanEditUsers) && (outUser.Password != Utils.MD5Encrypt.MD5Hash(userItem.Password) || userItem.Password.Length < 6))
            {
                ModelState.AddModelError("Password", App_GlobalResources.Common.PasswordInvalid);
                passwordError = true;
            }

            if
                (string.IsNullOrWhiteSpace(userItem.NewPassword) || string.IsNullOrWhiteSpace(userItem.ConfirmPassword) || !userItem.NewPassword.Equals(userItem.ConfirmPassword) || userItem.NewPassword.Length < 6)
            {
                ModelState.AddModelError("NewPassword", App_GlobalResources.Common.PasswordInvalid);
                passwordError = true;
            }

            if (!passwordError)
            {
                outUser.Password = Utils.MD5Encrypt.MD5Hash(userItem.NewPassword);
            }

            if (passwordError || !BLL.UserManager.Update(outUser))
            {
                userItem = outUser.ToUserItem();
                userItem.Password = userItem.ConfirmPassword = string.Empty;
                return View("Edit", userItem);
            }

            return RedirectToAction("Profile", "User", new { id = id, message = MessageId.ChangePasswordSuccess });
        }

        [HttpPost]
        public ActionResult ChangeUserType(UserItem userItem, int id)
        {
            ViewBag.EditUserNamePasswordValidator = false;
            ViewBag.EditChangePasswordValidator = false;
            ViewBag.EditFullNameValidator = false;

            User outUser = BLL.UserManager.GetById(id);

            outUser.UserTypeID = userItem.UserTypeID;

            if (!BLL.UserManager.Update(outUser))
            {
                return RedirectToAction("NotFound", "Error");
            }

            return RedirectToAction("Profile", "User", new { id = id, message = MessageId.ChangeUserTypeSuccess });
        }

        public ActionResult DeleteUser(int id)
        {
            ViewBag.EditUserNamePasswordValidator = false;
            ViewBag.EditChangePasswordValidator = false;
            ViewBag.EditFullNameValidator = false;

            //User outUser = BLL.UserManager.GetById(id, UserManager.UserRelatedData.GroupsEvents);

            //foreach (Event e in outUser.Events)
            //{
            //    BLL.EventManager.Remove(e);
            //}

            //foreach (Group e in outUser.Groups)
            //{
            //    BLL.GroupManager.Remove(e);
            //}

            //outUser = BLL.UserManager.GetById(id);
            if (!BLL.UserManager.Delete(id))
            {
                return RedirectToAction("NotFound", "Error");
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult UploadImage(int id)
        {
            User outUser = BLL.UserManager.GetById(id);
            string newFileName = string.Format("image_{0}_{1}", "CountMeIn", id);
            string returnFileName;

            if (Request.Files.Count != 0)
            {
                HttpPostedFileBase file = Request.Files["file"];

                Bitmap src = Bitmap.FromStream(file.InputStream) as Bitmap;

                if (src.Width < 250 || src.Height < 250)
                {
                    return Json(new { msg = App_GlobalResources.Common.dropZoneInvalidSize, status = "error", code = "403" });
                }

                if (!Utils.ProcessImage.allowedImageFiles.Any(e => e.Key == System.IO.Path.GetExtension(file.FileName)))
                {
                    return Json(new { msg = App_GlobalResources.Common.dropZoneInvalidFileType, status = "error", code = "403" });
                }

                if (file.ContentLength < 2048)
                {
                    return Json(new { msg = "Файлае повече от 2MB", status = "error", code = "403" });
                }

                returnFileName = Utils.ProcessImage.SaveFileOnDisk(WebConfigurationManager.AppSettings["UploadFileLocation"], file, DirectoryToUploads.ProfileImages.ToString(), newFileName);
                outUser.ProfileImageName = returnFileName;
                BLL.UserManager.Update(outUser);

            }
            else
            {
                outUser.ProfileImageName = null;
                BLL.UserManager.Update(outUser);
            }

            return RedirectToAction("Edit", "User", new { id = id });

        }

    }
}
