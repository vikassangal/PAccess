using System;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;

namespace PatientAccess.UI.FinancialCounselingViews
{
	/// <summary>
	/// Summary description for FinancialCouncelingService.
	/// </summary>
	//TODO: Create XML summary comment for FinancialCouncelingService
    [Serializable]
    public class FinancialCouncelingService : object
    {
        #region Event Handlers
        #endregion

        #region Methods

        public bool IsUninsuredFCWithoutInsurance( Account account )
        {
            if( account.FinancialClass != null )
            {
                if( !IsUninsured(account.Facility.Oid,account.FinancialClass ) && account.Insurance.Coverages.Count == 0 )
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        public static bool IsPatientInsured( FinancialClass aFinancialClass )
        {
            FinancialClassesBrokerProxy broker = new FinancialClassesBrokerProxy();
            return broker.IsPatientInsured( aFinancialClass );
        }

        public static bool IsUninsured( long facilityID,FinancialClass aFinancialClass )
        {
            FinancialClassesBrokerProxy broker = new FinancialClassesBrokerProxy();
            return broker.IsUninsured( facilityID,aFinancialClass );
        }

        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public FinancialCouncelingService()
        {
        }
        #endregion

        #region Data Elements

        public static bool PriorAccountsRetrieved     = false;

        #endregion

        #region Constants
        #endregion
    }
}
