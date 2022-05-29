using PatientAccess.Domain;

namespace PatientAccess.UI.InsuranceViews.InsuranceVerificationViews
{
    /// <summary>
    /// CommMgdCareVerifyView screen has requirement that the values related to each
    /// BenefitsCategory that may be entered be saved so that if the user selects a
    /// BenefitsCategory again for which data has been entered, the values will re-display
    /// on the screen controls.  This class manages that data.
    /// </summary>
    public class BenefitsCategoryData
    {
        #region Construction and Finalization
        public BenefitsCategoryData()
        {
        }
        #endregion

        #region Data Elements
        public BenefitsCategoryDetails bcd = new BenefitsCategoryDetails();
        public decimal  deductible = 0M;
        public decimal  coPayAmount = 0M;
        #endregion
    }
}
