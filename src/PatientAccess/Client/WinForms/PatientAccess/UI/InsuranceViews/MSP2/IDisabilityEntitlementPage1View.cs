
namespace PatientAccess.UI.InsuranceViews.MSP2
{
    public interface IDisabilityEntitlementPage1View
    {
        void DisablePage();

        /// <summary>
        /// CheckForSummary - determine if the Summary button can be enabled
        /// </summary>
        /// <returns></returns>
        bool CheckForSummary();

        /// <summary>
        /// ResetPage - set the page back to an un-initialized state
        /// </summary>
        void ResetPage();

    }
}
