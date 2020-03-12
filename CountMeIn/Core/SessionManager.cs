using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CountMeIn.Core
{
    public static class SessionManager
    {

        public static int UserID 
        {
            get
            {
                if (!IsAuthenticated || HttpContext.Current.Session["UserID"] == null)
                {
                    return 0;
                }
                
                return (int)HttpContext.Current.Session["UserID"];
            }
            set 
            {
                if (value == 0)
                {
                    HttpContext.Current.Session["UserID"] = null;
                }
                else
                {
                    HttpContext.Current.Session["UserID"] = value;
                }
            } 
        }

        public static string UserFullName
        {
            get
            {
                if (!IsAuthenticated || string.IsNullOrEmpty((string)HttpContext.Current.Session["UserFullName"])  )
                {
                    return null;
                }

                return (string)HttpContext.Current.Session["UserFullName"];
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    HttpContext.Current.Session["UserFullName"] = null;
                }
                else 
                { 
                    HttpContext.Current.Session["UserFullName"] = value;
                }
            }
        }

        public static DML.Enums.UserType UserType
        {
            get
            {
                if (!IsAuthenticated || HttpContext.Current.Session["UserType"] == null )
                {
                    return DML.Enums.UserType.Unknow;
                }

                return (DML.Enums.UserType)HttpContext.Current.Session["UserType"];
                
            }
            set 
            {
                if (value == DML.Enums.UserType.Unknow)
                {
                    HttpContext.Current.Session["UserType"] = null;
                }
                else
                {
                    HttpContext.Current.Session["UserType"] = value;
                }
            }
        }

        public static bool IsAuthenticated
        {
            get
            {
                if (!HttpContext.Current.User.Identity.IsAuthenticated )
                {
                    ClearUserSession();
                }
                else if (HttpContext.Current.Session["UserID"] == null)
                {
                    DML.User user = BLL.UserManager.GetByUsername(HttpContext.Current.User.Identity.Name);
                   
                    if (user != null)
                    {
                        LoadUserSession(user);
                    }
                    else 
                    {
                        ClearUserSession();
                        System.Web.Security.FormsAuthentication.SignOut();
                    } 
                }
                return HttpContext.Current.User.Identity.IsAuthenticated;
            }
        }

        public static int? EventListPageNumber
        {
            get
            {
                if (!IsAuthenticated || HttpContext.Current.Session["EventListPageNumber"] == null)
                {
                    return 0;
                }

                return (int)HttpContext.Current.Session["EventListPageNumber"];
            }
            set
            {
                if (value == 0)
                {
                    HttpContext.Current.Session["EventListPageNumber"] = null;
                }
                else
                {
                    HttpContext.Current.Session["EventListPageNumber"] = value;
                }
            }
        }

        public static string ReturnUrl
        {
            get
            {
                if (!IsAuthenticated || string.IsNullOrEmpty((string)HttpContext.Current.Session["ReturnUrl"]))
                {
                    return null;
                }

                return (string)HttpContext.Current.Session["ReturnUrl"];
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    HttpContext.Current.Session["ReturnUrl"] = null;
                }
                else
                {
                    HttpContext.Current.Session["ReturnUrl"] = value;
                }
            }
        }

        public static int BackToGroupID
        {
            get
            {
                if (!IsAuthenticated || HttpContext.Current.Session["BackToGroupID"] == null)
                {
                    return 0;
                }

                return (int)HttpContext.Current.Session["BackToGroupID"];
            }
            set
            {
                if (value == 0)
                {
                    HttpContext.Current.Session["BackToGroupID"] = null;
                }
                else
                {
                    HttpContext.Current.Session["BackToGroupID"] = value;
                }
            }
        }

        public static void LoadUserSession(DML.User user)
        {
            UserID = user.ID;
            UserFullName = string.Format("{0} {1}", user.FirstName, user.LastName);
            UserType = (DML.Enums.UserType)user.UserTypeID;
            EventListPageNumber = 1;
            BackToGroupID = 0;
        }

        public static void ClearUserSession()
        {
            UserID = 0;
            UserFullName = null;
            UserType = DML.Enums.UserType.Unknow;
            EventListPageNumber = 0;
            BackToGroupID = 0;
        }
    }
}