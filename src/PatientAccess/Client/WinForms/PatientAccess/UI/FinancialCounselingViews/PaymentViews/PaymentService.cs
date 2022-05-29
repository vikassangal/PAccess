using System;
using Extensions.SecurityService.Domain;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using User = PatientAccess.Domain.User;

namespace PatientAccess.UI.FinancialCounselingViews.PaymentViews
{
	/// <summary>
	/// Summary description for PaymentService.
	/// </summary>
	//TODO: Create XML summary comment for PaymentService
    [Serializable]
    public class PaymentService 
    {
        #region Event Handlers
        #endregion

        #region Methods

        public bool HasPermissionToOverrideMonthlyPayment( string userName, string password, Facility facility )
        {
            bool hasPermission = false;
            IUserBroker userBroker = BrokerFactory.BrokerOfType<IUserBroker>();
            hasPermission = userBroker.HasPermissionToOverrideMonthlyPayment( userName, password, facility );
            return hasPermission;
        }

        public bool CanOverrideMonthlyPayment()
        {
            bool canOverrideMonthlyPayment = false;
            User patientAccessUser = User.GetCurrent();
            Facility facility = patientAccessUser.Facility;
            Peradigm.Framework.Domain.Parties.Facility securityFrameworkFacility = new Peradigm.Framework.Domain.Parties.Facility( facility.Code, facility.Description );
            canOverrideMonthlyPayment = patientAccessUser.SecurityUser.HasPermissionTo( Privilege.Actions.Edit, typeof( MonthlyPaymentOverride ), securityFrameworkFacility );
            return canOverrideMonthlyPayment;        
        }

        public int NumberOfMonthsToDisplay ( decimal balanceDue, bool canOverrideMonthlyPayment )
        {
            int numberOfMonths = 0;
            if( balanceDue >= 0 )
            {
                if( balanceDue <= 0m )
                {
                    numberOfMonths = 0;
                }
                else if( balanceDue > 0m && balanceDue <= 250m )
                {   //max of 4 monthly payments for regular user; 24 - for Registration-Administrator
                    if( canOverrideMonthlyPayment )
                        numberOfMonths = 24;
                    else
                        numberOfMonths = 4;
                }
                else if( balanceDue >= 250.01m && balanceDue <= 500m )
                {   //max of 6 monthly payments for regular user; 24 - for Registration-Administrator
                    if( canOverrideMonthlyPayment )
                        numberOfMonths = 24;
                    else
                        numberOfMonths = 6;
                }
                else if( balanceDue >= 500.01m && balanceDue <= 1000m )
                {   //max of 10 monthly payments for regular user; 24 - for Registration-Administrator
                    if( canOverrideMonthlyPayment )
                        numberOfMonths = 24;
                    else
                        numberOfMonths = 10;
                }
                else if( balanceDue >= 1000.01m && balanceDue <= 2500m )
                {   //max of 12 monthly payments for regular user; 24 - for Registration-Administrator
                    if( canOverrideMonthlyPayment )
                        numberOfMonths = 24;
                    else
                        numberOfMonths = 12;
                }
                else if( balanceDue > 2500 )
                {   //max of 24 monthly payments for regular user; 48 - for Registration-Administrator
                    if( canOverrideMonthlyPayment )
                        numberOfMonths = 48;
                    else
                        numberOfMonths = 24;
                }
            }
            return numberOfMonths;
        }


        #endregion

        #region Properties

        public FormViewRecordPayment FormViewRecordPayment
        {
            get
            {
                return i_FormViewRecordPayment;
            }
            set
            {
                i_FormViewRecordPayment = value;
            }
        }

        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public PaymentService()
        {
        }
        #endregion

        #region Data Elements

        private FormViewRecordPayment i_FormViewRecordPayment;

        #endregion

        #region Constants
        #endregion
    }
}
