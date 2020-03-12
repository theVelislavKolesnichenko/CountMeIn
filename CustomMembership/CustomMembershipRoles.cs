using System;
using BLL;

namespace CustomMembership
{
    class CustomMembershipRoles : System.Web.Security.RoleProvider
    {
        #region Properties

        private string applicationName;
        /// <summary>
        /// Not implemented
        /// </summary>
        public override string ApplicationName
        {
            get
            {
                return applicationName;
            }
            set
            {
                applicationName = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///  System.Configuration.Provider.RoleProvider.Initialize Method
        ///  Initialize values from web.config.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="config"> values from web.config </param>
        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            applicationName = config["applicationName"];
            base.Initialize(name, config);
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="usernames"></param>
        /// <param name="roleNames"></param>
        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented 
        /// New role created from RoleManager <see cref="RoleManager"/> 
        /// </summary>
        /// <param name="roleName"></param>
        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="throwOnPopulatedRole"></param>
        /// <returns></returns>
        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets an array of user names in a role where the user name contains the specified user name to match.
        /// </summary>
        /// <param name="roleName">The role to search in.</param>
        /// <param name="usernameToMatch">The user name to search for.</param>
        /// <returns>A string array containing the names of all the users where the user name matches usernameToMatch and the user is a member of the specified role.</returns>
        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
            //#Merge
            //return (RoleManager.FindUsersInRole(roleName, usernameToMatch));
        }

        /// <summary>
        /// Gets a list of all the roles for the application.
        /// </summary>
        /// <returns>A string array containing the names of all the roles stored in the data source for the application.</returns>
        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
            //#Merge
            //return RoleManager.GetAllRoleDefaultNames();
        }

        /// <summary>
        /// Gets a list of the roles that a specified user is in for the application.
        /// </summary>
        /// <param name="username">The user to return a list of roles for.</param>
        /// <returns>A string array containing the names of all the roles that the specified user is in for the application.</returns>
        public override string[] GetRolesForUser(string username)
        {
            return PermissionManager.GetPermissionsForUser(username);
        }
            
        /// <summary>
        /// Gets a list of users in the specified role for the application.
        /// </summary>
        /// <param name="roleName">The name of the role to get the list of users for.</param>
        /// <returns>A string array containing the names of all the users who are members of the specified role for the application.</returns>
        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
            //#Merge
            //return RoleManager.GetUsersInRole(roleName);
        }

        /// <summary>
        /// Gets a value indicating whether the specified user is in the specified role for the application.
        /// </summary>
        /// <param name="username">The user name to search for.</param>
        /// <param name="roleName">The role to search in.</param>
        /// <returns>true if the specified user is in the specified role for the application; otherwise, false.</returns>
        public override bool IsUserInRole(string username, string roleName)
        {
            throw new NotImplementedException();
            //#Merge
            //return RoleManager.IsUserInRole(username, roleName);
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="usernames"></param>
        /// <param name="roleNames"></param>
        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a value indicating whether the specified role name already exists in the role data source for the application.
        /// </summary>
        /// <param name="roleName">The name of the role to search for in the data source.</param>
        /// <returns>true if the role name already exists in the data source for the application; otherwise, false.</returns>
        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
            //#Merge
            //return RoleManager.RoleExists(roleName);
        }

        #endregion
    }
}