using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;

namespace Utils
{
    public static class AppUtils
    {
        public static string AppDomainPath
        {
            get
            {
                return (HttpContext.Current != null && HttpContext.Current.Request != null) ?
                string.Format("{0}{1}", HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority), HttpRuntime.AppDomainAppVirtualPath) :
                ConfigurationManager.AppSettings["DomainPath"];// if the library is used from windows service or windows forms read from app.config settings
            }
        }
  
    }
}
