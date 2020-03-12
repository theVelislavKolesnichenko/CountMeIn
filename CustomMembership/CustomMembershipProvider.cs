using System;
using System.Web.Security;
using BLL;

namespace CustomMembership
{
    /// <summary>
    /// Custom membership provider 
    /// </summary>
    class CustomMembershipProvider : MembershipProvider
    {
        #region Properties
        //
        // System.Web.Security.MembershipProvider properties.
        //

        private string applicationName = string.Empty;
        private string passwordStrengthRegularExpression = string.Empty;

        private bool enablePasswordReset = false;
        private bool enablePasswordRetrieval = false;
        private bool requiresQuestionAndAnswer = false;
        private bool requiresUniqueEmail = true;

        private int maxInvalidPasswordAttempts = 1;
        private int minRequiredNonAlphanumericCharacters = 0;
        private int minRequiredPasswordLength = 6;
        private int passwordAttemptWindow = 0;
        private MembershipPasswordFormat passwordFormat = MembershipPasswordFormat.Hashed;

        /// <summary>
        /// Not implemented
        /// </summary>
        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        ///  Enable/Disable Password Reset Option
        /// </summary>
        public override bool EnablePasswordReset
        {
            get { return enablePasswordReset; }
        }

        /// <summary>
        ///  Enable/Disable Password Retrieval Option
        /// </summary>
        public override bool EnablePasswordRetrieval
        {
            get { return enablePasswordRetrieval; }
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        public override bool RequiresQuestionAndAnswer
        {
            get { return requiresQuestionAndAnswer; }
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        public override bool RequiresUniqueEmail
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        public override int MaxInvalidPasswordAttempts
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        public override int MinRequiredNonAlphanumericCharacters
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        public override int MinRequiredPasswordLength
        {
            get { return minRequiredPasswordLength; }
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        public override int PasswordAttemptWindow
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        public override MembershipPasswordFormat PasswordFormat
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        public override string PasswordStrengthRegularExpression
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///  System.Configuration.Provider.ProviderBase.Initialize Method
        ///  Initialize values from web.config.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="config"> values from web.config </param>
        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            if (config["applicationName"] != null)
            {
                applicationName = config["applicationName"];
            }
            if (config["enablePasswordReset"] != null)
            {
                enablePasswordReset = bool.Parse(config["enablePasswordReset"]);
            }
            if (config["enablePasswordRetrieval"] != null)
            {
                enablePasswordRetrieval = bool.Parse(config["enablePasswordRetrieval"]);
            }
            if (config["maxInvalidPasswordAttempts"] != null)
            {
                maxInvalidPasswordAttempts = int.Parse(config["maxInvalidPasswordAttempts"]);
            }
            if (config["minRequiredNonAlphanumericCharacters"] != null)
            {
                minRequiredNonAlphanumericCharacters = int.Parse(config["minRequiredNonAlphanumericCharacters"]);
            }
            if (config["minRequiredNonAlphanumericCharacters"] != null)
            {
                minRequiredNonAlphanumericCharacters = int.Parse(config["minRequiredNonAlphanumericCharacters"]);
            }
            if (config["minRequiredPasswordLength"] != null)
            {
                minRequiredPasswordLength = int.Parse(config["minRequiredPasswordLength"]);
            }
            if (config["passwordAttemptWindow"] != null)
            {
                passwordAttemptWindow = int.Parse(config["passwordAttemptWindow"]);
            }
            if (config["passwordFormat"] != null)
            {
                passwordFormat = (MembershipPasswordFormat)Enum.Parse(typeof(MembershipPasswordFormat), config["passwordFormat"]);
            }
            if (config["passwordStrengthRegularExpression"] != null)
            {
                passwordStrengthRegularExpression = config["passwordStrengthRegularExpression"];
            }
            if (config["requiresQuestionAndAnswer"] != null)
            {
                requiresQuestionAndAnswer = bool.Parse(config["requiresQuestionAndAnswer"]);
            }
            if (config["requiresUniqueEmail"] != null)
            {
                requiresUniqueEmail = bool.Parse(config["requiresUniqueEmail"]);
            }

            //other formats are not supported
            passwordFormat = MembershipPasswordFormat.Hashed;

            base.Initialize(name, config);
        }

        /// <summary>
        /// Processes a request to update the password for a membership user.
        /// </summary>
        /// <param name="username">The user to update the password for.</param>
        /// <param name="oldPassword">The current password for the specified user.</param>
        /// <param name="newPassword">The new password for the specified user.</param>
        /// <returns>true if the password was updated successfully; otherwise, false.</returns>
        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
            // if used find logged user id by User.Identity.Name
            //return UserManager.ChangePassword(username, oldPassword, newPassword);
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="newPasswordQuestion"></param>
        /// <param name="newPasswordAnswer"></param>
        /// <returns></returns>
        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="email"></param>
        /// <param name="passwordQuestion"></param>
        /// <param name="passwordAnswer"></param>
        /// <param name="isApproved"></param>
        /// <param name="providerUserKey"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="username"></param>
        /// <param name="deleteAllRelatedData"></param>
        /// <returns></returns>
        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="emailToMatch"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="usernameToMatch"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <returns></returns>
        public override int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="username"></param>
        /// <param name="answer"></param>
        /// <returns></returns>
        public override string GetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="username"></param>
        /// <param name="userIsOnline"></param>
        /// <returns></returns>
        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="providerUserKey"></param>
        /// <param name="userIsOnline"></param>
        /// <returns></returns>
        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Gets the user name associated with the specified e-mail address.
        /// </summary>
        /// <param name="email">The e-mail address to search for.</param>
        /// <returns>The user name associated with the specified e-mail address. If no match is found, return null.</returns>
        public override string GetUserNameByEmail(string email)
        {
            return null;// UserManager.GetUserNameByEmail(email);
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="username"></param>
        /// <param name="answer"></param>
        /// <returns></returns>
        public override string ResetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public override bool UnlockUser(string userName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="user"></param>
        public override void UpdateUser(MembershipUser user)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Verifies that the specified user name and password exist in the data source.
        /// </summary>
        /// <param name="username">The name of the user to validate.</param>
        /// <param name="password">The password for the specified user.</param>
        /// <returns>true if the specified username and password are valid; otherwise, false.</returns>
        public override bool ValidateUser(string username, string password)
        {
            throw new NotImplementedException();
            //return UserManager.ValidateUser(username, password);
        }

        #endregion
    }
}