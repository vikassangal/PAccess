 using PatientAccess.Domain;

namespace PatientAccess.UI.RegulatoryViews.Views
{
    public interface IAuthorizePortalUserDetailView
    {
        string FirstName { get; set; }
        string LastName { get; set; }
        //DateTime DOB { get; set; }
        string DOB { get; set; }

        //EmailAddress EmailAddress { get; set; }
        string EmailAddress { get; set; }
        YesNoFlag RemoveUserFlag { get; set; }
        bool HasNoFirstName { get; }
        bool HasNoLastName { get; }
        bool HasNoEmail { get; }
        bool HasNoDob { get; }
        void SetNormalColorFirstName();
        void SetRequiredColorFirstName();
        void SetNormalColorLastName();
        void SetRequiredColorLastName();
        void SetNormalColorEmail();
        void SetRequiredColorEmail();
        void SetNormalColorDob();
        void SetRequiredColorDob();
        void SetErrorColorDob();
        void SetErrorColorEmail();
    }
}
