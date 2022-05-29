using System; 
using System.Collections.Generic;
using System.Linq;
using PatientAccess.Domain;
using PatientAccess.UI.HelperClasses;
using PatientAccess.UI.RegulatoryViews.ViewImpl;
using PatientAccess.UI.RegulatoryViews.Views;
using PatientAccess.Utilities;

namespace PatientAccess.UI.RegulatoryViews.Presenters
{
    public class AuthorizePortalUserPresenter : IAuthorizePortalUserPresenter
    {

        #region Variables and Properties

        public IAuthorizePortalUserView AuthorizePortalUserView;
        Dictionary<int, AuthorizedAdditionalPortalUser> TempAuthorizedAdditionalPortalUsers;
        private readonly IMessageBoxAdapter MessageBoxAdapter;
        private readonly Account ModelAccount;

        private Dictionary<int, AuthorizedAdditionalPortalUser> AuthorizePortalUsers
        {
            get { return ModelAccount.AuthorizedAdditionalPortalUsers; }
        }

        #endregion

        #region Constructors
        public AuthorizePortalUserPresenter(IAuthorizePortalUserView authorizePortalUserView,
            IMessageBoxAdapter messageBoxAdapter, Account account)
        {
            Guard.ThrowIfArgumentIsNull(authorizePortalUserView, "authorizePortalUserView");
            Guard.ThrowIfArgumentIsNull(account, "account");
            Guard.ThrowIfArgumentIsNull(messageBoxAdapter, "messageBoxAdapter");

            AuthorizePortalUserView = authorizePortalUserView;
            MessageBoxAdapter = messageBoxAdapter;
            ModelAccount = account; 
        }

        #endregion
        
        #region Public Methods
        public void UpdateView()
        {
            PopulateAuthorizePortalUserDetails();
        }

        #endregion

        #region private Methods
        public void PopulateAuthorizePortalUserDetails()
        {
            // TODO : use foreach
            
                AuthorizePortalUserView.AuthorizePortalUserDetail0.PortalUser =
                    AuthorizePortalUsers.ContainsKey(0) && AuthorizePortalUsers[0] != null
                        ? AuthorizePortalUsers[0]
                        : new AuthorizedAdditionalPortalUser();
           

            AuthorizePortalUserView.AuthorizePortalUserDetail1.PortalUser =
                AuthorizePortalUsers.ContainsKey(1) && AuthorizePortalUsers[1] != null
                    ? AuthorizePortalUsers[1]
                    : new AuthorizedAdditionalPortalUser();

            AuthorizePortalUserView.AuthorizePortalUserDetail2.PortalUser =
                AuthorizePortalUsers.ContainsKey(2) && AuthorizePortalUsers[2] != null
                    ? AuthorizePortalUsers[2]
                    : new AuthorizedAdditionalPortalUser();

            AuthorizePortalUserView.AuthorizePortalUserDetail3.PortalUser =
                AuthorizePortalUsers.ContainsKey(3) && AuthorizePortalUsers[3] != null
                    ? AuthorizePortalUsers[3]
                    : new AuthorizedAdditionalPortalUser();
            

            AuthorizePortalUserView.AuthorizePortalUserDetail0.AuthPortalUserSequenceNumber = 0;
            AuthorizePortalUserView.AuthorizePortalUserDetail0.UpdateView();

            AuthorizePortalUserView.AuthorizePortalUserDetail1.AuthPortalUserSequenceNumber = 1;
            AuthorizePortalUserView.AuthorizePortalUserDetail1.UpdateView();

            AuthorizePortalUserView.AuthorizePortalUserDetail2.AuthPortalUserSequenceNumber = 2;
            AuthorizePortalUserView.AuthorizePortalUserDetail2.UpdateView();

            AuthorizePortalUserView.AuthorizePortalUserDetail3.AuthPortalUserSequenceNumber = 3;
            AuthorizePortalUserView.AuthorizePortalUserDetail3.UpdateView();
        }
         
        public bool HandleSaveResponse()
        {
            return SaveAuthorizedPortalUsers();
        }
       
        private bool HasOneUserPopulated
        {
            get
            {
                return (AuthorizePortalUserView.AuthorizePortalUserDetail0.AuthorizePortalUserDetailPresenter
                            .HasAllFieldsEntered() ||
                        AuthorizePortalUserView.AuthorizePortalUserDetail1.AuthorizePortalUserDetailPresenter
                            .HasAllFieldsEntered() ||
                        AuthorizePortalUserView.AuthorizePortalUserDetail2.AuthorizePortalUserDetailPresenter
                            .HasAllFieldsEntered() ||
                        AuthorizePortalUserView.AuthorizePortalUserDetail3.AuthorizePortalUserDetailPresenter
                            .HasAllFieldsEntered());
            }
        }

        public bool ValidateAuthorizePortalUser()
        {
            if (!HasOneUserPopulated)
            {
                MessageBoxAdapter.ShowMessageBox(
                    UIErrorMessages.AUTHORIZE_ADDITIONAL_PORTAL_USER_REQUIRED_MSG, "Warning",
                    MessageBoxAdapterButtons.OK, MessageBoxAdapterIcon.Exclamation,
                    MessageBoxAdapterDefaultButton.Button1);
                return false;
            }

            if (HasMissingEntriesInARow())
            {
                return false;
            }

            return true;
        }

        public bool HasMissingEntriesInARow()
        {
            if (AuthorizePortalUserView.AuthorizePortalUserDetail0.AuthorizePortalUserDetailPresenter
                        .HasMissingEntries ||
                    AuthorizePortalUserView.AuthorizePortalUserDetail1.AuthorizePortalUserDetailPresenter
                        .HasMissingEntries ||
                    AuthorizePortalUserView.AuthorizePortalUserDetail2.AuthorizePortalUserDetailPresenter
                        .HasMissingEntries ||
                    AuthorizePortalUserView.AuthorizePortalUserDetail3.AuthorizePortalUserDetailPresenter
                        .HasMissingEntries)
            {
                MessageBoxAdapter.ShowMessageBox(
                    UIErrorMessages.AUTHORIZE_ADDITIONAL_PORTAL_USER_REQUIRED_MSG, "Warning",
                    MessageBoxAdapterButtons.OK, MessageBoxAdapterIcon.Exclamation,
                    MessageBoxAdapterDefaultButton.Button1);
                return true;
            }
            return false;
        }

        private bool SaveAuthorizedPortalUsers()
        {

            TempAuthorizedAdditionalPortalUsers = new Dictionary<int, AuthorizedAdditionalPortalUser>();
            var tempAuthUsersIndex = 0;
            //--------------------------------authUser0-------------------------------------------------
            var authUser0 = ExtractAuthorizedUser(AuthorizePortalUserView.AuthorizePortalUserDetail0);
            if (authUser0 != null)
            {
                TempAuthorizedAdditionalPortalUsers.Add(tempAuthUsersIndex,
                    authUser0);
                tempAuthUsersIndex++;
            }
            else if (  AuthorizePortalUsers.ContainsKey(0) &&
                     AuthorizePortalUsers[0].SequenceNumber > 0)
            {
                authUser0 = AuthorizePortalUsers[0];
                authUser0.RemoveUserFlag = AuthorizePortalUserView.AuthorizePortalUserDetail0.RemoveUserFlag;
                TempAuthorizedAdditionalPortalUsers.Add(tempAuthUsersIndex,
                    authUser0);
                tempAuthUsersIndex++;
            }
            //--------------------------------authUser0-------------------------------------------------
            //--------------------------------authUser1-------------------------------------------------
            var authUser1 = ExtractAuthorizedUser(AuthorizePortalUserView.AuthorizePortalUserDetail1);
            if (authUser1 != null)
            {
                TempAuthorizedAdditionalPortalUsers.Add(tempAuthUsersIndex,
                    authUser1);
                tempAuthUsersIndex++;
            }
            else if (  AuthorizePortalUsers.ContainsKey(1) &&
                     AuthorizePortalUsers[1].SequenceNumber > 0)
            {
                authUser1 = AuthorizePortalUsers[1];
                authUser1.RemoveUserFlag = AuthorizePortalUserView.AuthorizePortalUserDetail1.RemoveUserFlag;
                TempAuthorizedAdditionalPortalUsers.Add(tempAuthUsersIndex,
                    authUser1);
                tempAuthUsersIndex++;
            }
            //--------------------------------authUser1-------------------------------------------------
            //--------------------------------authUser2-------------------------------------------------
            var authUser2 = ExtractAuthorizedUser(AuthorizePortalUserView.AuthorizePortalUserDetail2);
            if (authUser2 != null)
            {
                TempAuthorizedAdditionalPortalUsers.Add(tempAuthUsersIndex,
                    authUser2);
                tempAuthUsersIndex++;
            }
            else if ( AuthorizePortalUsers.ContainsKey(2) &&
                     AuthorizePortalUsers[2].SequenceNumber > 0)
            {
                authUser2 = AuthorizePortalUsers[2];
                authUser2.RemoveUserFlag = AuthorizePortalUserView.AuthorizePortalUserDetail2.RemoveUserFlag;
                TempAuthorizedAdditionalPortalUsers.Add(tempAuthUsersIndex,
                    authUser2);
                tempAuthUsersIndex++;
            }
            //--------------------------------authUser2-------------------------------------------------
            //--------------------------------authUser3-------------------------------------------------
            var authUser3 = ExtractAuthorizedUser(AuthorizePortalUserView.AuthorizePortalUserDetail3);
            if (authUser3 != null)
            {
                TempAuthorizedAdditionalPortalUsers.Add(tempAuthUsersIndex,
                    authUser3);
            }
            else if ( AuthorizePortalUsers.ContainsKey(3) &&
                     AuthorizePortalUsers[3].SequenceNumber > 0)
            {
                authUser3 = AuthorizePortalUsers[3];
                authUser3.RemoveUserFlag = AuthorizePortalUserView.AuthorizePortalUserDetail3.RemoveUserFlag;
                TempAuthorizedAdditionalPortalUsers.Add(tempAuthUsersIndex,
                    authUser3);
                tempAuthUsersIndex++;
            }
            //--------------------------------authUser3-------------------------------------------------

            if (TempAuthorizedAdditionalPortalUsers.Count <= 0)
            {
                MessageBoxAdapter.ShowMessageBox(
                    UIErrorMessages.AUTHORIZE_ADDITIONAL_PORTAL_USER_REQUIRED_MSG, "Warning",
                    MessageBoxAdapterButtons.OK, MessageBoxAdapterIcon.Exclamation,
                    MessageBoxAdapterDefaultButton.Button1);
                return false;
            }
            else
            {
                foreach (KeyValuePair<int, AuthorizedAdditionalPortalUser> tempAuthUser in TempAuthorizedAdditionalPortalUsers)
                {
                    SetModelAuthorizedUsers(tempAuthUser);
                }

                if (AuthorizePortalUsers.Count > TempAuthorizedAdditionalPortalUsers.Count)
                {
                    foreach (var authUser in AuthorizePortalUsers.Reverse())
                    {
                        if (!TempAuthorizedAdditionalPortalUsers.ContainsKey(authUser.Key))
                        {
                            AuthorizePortalUsers.Remove(authUser.Key);
                        }
                    }
                }
            }

            return true;
        }

        private void SetModelAuthorizedUsers(KeyValuePair<int, AuthorizedAdditionalPortalUser> tempAuthUser)
        {
            if (!String.IsNullOrEmpty(tempAuthUser.Value.FirstName))
            {
                if (ModelAccount.AuthorizedAdditionalPortalUsers.ContainsKey(tempAuthUser.Key))
                {
                    ModelAccount.AuthorizedAdditionalPortalUsers[tempAuthUser.Key].RemoveUserFlag =
                        tempAuthUser.Value.RemoveUserFlag;
                    ModelAccount.AuthorizedAdditionalPortalUsers[tempAuthUser.Key].FirstName =
                        tempAuthUser.Value.FirstName;
                    ModelAccount.AuthorizedAdditionalPortalUsers[tempAuthUser.Key].LastName =
                        tempAuthUser.Value.LastName;
                    ModelAccount.AuthorizedAdditionalPortalUsers[tempAuthUser.Key].DateOfBirth =
                        tempAuthUser.Value.DateOfBirth;
                    ModelAccount.AuthorizedAdditionalPortalUsers[tempAuthUser.Key].EmailAddress =
                        tempAuthUser.Value.EmailAddress;
                }
                else
                {
                    ModelAccount.AuthorizedAdditionalPortalUsers.Add(tempAuthUser.Key, tempAuthUser.Value);
                }
            }
        }

        private AuthorizedAdditionalPortalUser ExtractAuthorizedUser(
            AuthorizePortalUserDetailView authorizePortalUserDetail)
        {
           
            AuthorizedAdditionalPortalUser authUser = null;
            if ( authorizePortalUserDetail.AuthorizePortalUserDetailPresenter
                .HasAllFieldsEntered() )
            {
                var firstName = authorizePortalUserDetail.FirstName;
                var lastName = authorizePortalUserDetail.LastName;
                var dob = authorizePortalUserDetail.DOB;
                var email = authorizePortalUserDetail.EmailAddress;
                var removeFlag = authorizePortalUserDetail.RemoveUserFlag;
                var DOBEntered = new DateTime(Convert.ToInt32(dob.Substring(4, 4)),
                    Convert.ToInt32(dob.Substring(0, 2)),
                    Convert.ToInt32(dob.Substring(2, 2)), 0, 0, 0);
                authUser = new AuthorizedAdditionalPortalUser()
                {
                    Oid = authorizePortalUserDetail.AuthPortalUserSequenceNumber,
                    FirstName = firstName,
                    LastName = lastName,
                    DateOfBirth = DOBEntered,
                    EmailAddress = (email == String.Empty) ? new EmailAddress() : new EmailAddress(email ),
                    RemoveUserFlag = removeFlag
                };
            }

            return authUser;

        }
         
        #endregion

        
    }
}