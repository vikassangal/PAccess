using System;
using PatientAccess.Domain; 
using PatientAccess.UI.RegulatoryViews.Views;
using PatientAccess.Utilities;


namespace PatientAccess.UI.RegulatoryViews.Presenters
{
    public class AuthorizePortalUserDetailPresenter : IAuthorizePortalUserDetailPresenter
    {

        #region Variables and Properties

        public IAuthorizePortalUserDetailView AuthorizePortalUserDetailView;

        private readonly AuthorizedAdditionalPortalUser AuthorizedAdditionalPortalUser;

        #endregion

        #region Constructors

        public AuthorizePortalUserDetailPresenter(IAuthorizePortalUserDetailView authorizePortalUserDetailView,
            AuthorizedAdditionalPortalUser portalUser)
        {
            Guard.ThrowIfArgumentIsNull(authorizePortalUserDetailView, "authorizePortalUserDetailView");
            Guard.ThrowIfArgumentIsNull(portalUser, "portalUser");

            AuthorizePortalUserDetailView = authorizePortalUserDetailView;
            AuthorizedAdditionalPortalUser = portalUser;
        }

        #endregion

        #region Public Methods

        public void UpdateView()
        {
            SetNormalColor();
            PopulateAuthorizePortalUserDetails();
        }

        public void PopulateAuthorizePortalUserDetails()
        {
            AuthorizePortalUserDetailView.FirstName = AuthorizedAdditionalPortalUser.FirstName;
            AuthorizePortalUserDetailView.LastName = AuthorizedAdditionalPortalUser.LastName;
            if (AuthorizedAdditionalPortalUser.DateOfBirth == DateTime.MinValue)
            {
                AuthorizePortalUserDetailView.DOB = String.Empty;
            }
            else
            {

                AuthorizePortalUserDetailView.DOB = String.Format("{0:D2}{1:D2}{2:D4}",
                    AuthorizedAdditionalPortalUser.DateOfBirth.Month,
                    AuthorizedAdditionalPortalUser.DateOfBirth.Day,
                    AuthorizedAdditionalPortalUser.DateOfBirth.Year);
            }

            AuthorizePortalUserDetailView.EmailAddress = AuthorizedAdditionalPortalUser.EmailAddress.Uri;
            AuthorizePortalUserDetailView.RemoveUserFlag = AuthorizedAdditionalPortalUser.RemoveUserFlag;
        }

        public void CheckAllFieldsValuesAreEntered()
        {
            if (AuthorizePortalUserDetailView.HasNoFirstName)
            {
                AuthorizePortalUserDetailView.SetRequiredColorFirstName();
            }
            else
            {
                AuthorizePortalUserDetailView.SetNormalColorFirstName();
            }

            if (AuthorizePortalUserDetailView.HasNoLastName)
            {
                AuthorizePortalUserDetailView.SetRequiredColorLastName();
            }
            else
            {
                AuthorizePortalUserDetailView.SetNormalColorLastName();
            }

            if (AuthorizePortalUserDetailView.HasNoEmail)
            {
                AuthorizePortalUserDetailView.SetNormalColorEmail();
                AuthorizePortalUserDetailView.SetRequiredColorEmail();
            }
            else
            {
                AuthorizePortalUserDetailView.SetNormalColorEmail();
            }

            if (AuthorizePortalUserDetailView.HasNoDob)
            {
                AuthorizePortalUserDetailView.SetNormalColorDob();
                AuthorizePortalUserDetailView.SetRequiredColorDob();
            }
            else
            {
                AuthorizePortalUserDetailView.SetNormalColorDob();
            }

            if (AuthorizePortalUserDetailView.HasNoFirstName && AuthorizePortalUserDetailView.HasNoLastName &&
                AuthorizePortalUserDetailView.HasNoDob && AuthorizePortalUserDetailView.HasNoEmail)
            {
                SetNormalColor();
            }
        }

        private void SetNormalColor()
        {
            AuthorizePortalUserDetailView.SetNormalColorFirstName();
            AuthorizePortalUserDetailView.SetNormalColorLastName();
            AuthorizePortalUserDetailView.SetNormalColorEmail();
            AuthorizePortalUserDetailView.SetNormalColorDob();
        }

        public bool HasMissingEntries
        {
            get
            {
                return HasMissingEntriesWithFirstName || HasMissingEntriesWithLastName ||
                       HasMissingEntriesWithDOB || HasMissingEntriesWithEmail;
            }
        }

        private bool HasMissingEntriesWithFirstName
        {
            get
            {
                return !AuthorizePortalUserDetailView.HasNoFirstName &&
                       (AuthorizePortalUserDetailView.HasNoLastName || AuthorizePortalUserDetailView.HasNoDob ||
                        AuthorizePortalUserDetailView.HasNoEmail);
            }
        }

        private bool HasMissingEntriesWithLastName
        {
            get
            {
                return !AuthorizePortalUserDetailView.HasNoLastName &&
                       (AuthorizePortalUserDetailView.HasNoFirstName || AuthorizePortalUserDetailView.HasNoDob ||
                        AuthorizePortalUserDetailView.HasNoEmail);
            }
        }

        private bool HasMissingEntriesWithDOB
        {
            get
            {
                return !AuthorizePortalUserDetailView.HasNoDob &&
                       (AuthorizePortalUserDetailView.HasNoFirstName || AuthorizePortalUserDetailView.HasNoLastName ||
                        AuthorizePortalUserDetailView.HasNoEmail);
            }
        }

        private bool HasMissingEntriesWithEmail
        {
            get
            {
                return !AuthorizePortalUserDetailView.HasNoEmail &&
                       (AuthorizePortalUserDetailView.HasNoFirstName || AuthorizePortalUserDetailView.HasNoLastName ||
                        AuthorizePortalUserDetailView.HasNoDob);
            }
        }

        public bool HasAllFieldsEntered()
        {
            bool result = true;

            if (AuthorizePortalUserDetailView.HasNoFirstName)
            {
                result = false;
            }
            else if (AuthorizePortalUserDetailView.HasNoLastName)
            {
                result = false;
            }
            else if (AuthorizePortalUserDetailView.HasNoEmail)
            {
                result = false;
            }
            else if (AuthorizePortalUserDetailView.HasNoDob)
            {
                result = false;
            }

            return result;

        }

        public bool CheckOneAuthorizedUserIsEntered()
        {
            if (AuthorizePortalUserDetailView.HasNoFirstName && AuthorizePortalUserDetailView.HasNoLastName &&
                AuthorizePortalUserDetailView.HasNoEmail && AuthorizePortalUserDetailView.HasNoDob)
            {
                return true;
            }

            return false;
        }

        #endregion
    }
}