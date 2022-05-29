using PatientAccess.Domain;

namespace PatientAccess.UI.InsuranceViews.MSP2
{
// ReSharper disable MemberCanBeInternal
// this needs to be public to generate mocks
    public interface IAgeEntitlementPage1View
// ReSharper restore MemberCanBeInternal
    {
        string SpouseRetirementDateText { get; set; }
        bool SpouseEmployed { get; set; }
        bool SpouseNeverEmployed { get; set; }
        bool SpouseRetired { get; set; }
        bool SpouseOtherEmployed { get; set; }
        
        string PatientRetirementDateText { get; set; }
        bool PatientEmployed { get; set; }
        bool PatientNeverEmployed { get; set; }
        bool PatientRetired { get; set; }
        bool PatientOtherEmployed { get; set; }

        void DisplaySpouseEmployment( Employment spouseEmployment );

        void DisablePage();
        void EnablePage();

        /// <summary>
        /// CheckForSummary - determine if the Summary button can be enabled
        /// </summary>
        /// <returns></returns>
        bool CheckForSummary();

        /// <summary>
        /// ResetPage - set the page back to an un-initialized state
        /// </summary>
        void ResetPage();

        /// <summary>
        /// DisplayPatientEmployment - display the patient's employment
        /// </summary>
        /// <param name="display"></param>
        void DisplayPatientEmployment( bool display );
    }

}
