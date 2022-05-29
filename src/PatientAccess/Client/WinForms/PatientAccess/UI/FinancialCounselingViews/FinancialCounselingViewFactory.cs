using System;
using Extensions.SecurityService.Domain;
using Extensions.UI.Winforms;
using PatientAccess.Domain;
using PatientAccess.UI.FinancialCounselingViews.PaymentViews;
using Facility = Peradigm.Framework.Domain.Parties.Facility;
using User = PatientAccess.Domain.User;

namespace PatientAccess.UI.FinancialCounselingViews
{
	/// <summary>
	/// Summary description for FinancialCounselingFactory.
	/// </summary>
	//TODO: Create XML summary comment for FinancialCounselingFactory
    [Serializable]
    public class FinancialCounselingViewFactory : object
    {
        #region Event Handlers
        #endregion

        #region Methods

        public static ControlView LiabilityViewFor( Account anAccount )
        {
            ControlView result = null;

            User patientAccessUser = User.GetCurrent();
            Extensions.SecurityService.Domain.User securityUser = patientAccessUser.SecurityUser;
            Facility securityFrameworkFacility = new Facility( patientAccessUser.Facility.Code, patientAccessUser.Facility.Description );
            bool hasEditPermissionForLiability  = securityUser.HasPermissionTo( Privilege.Actions.Edit, typeof( Payment ), securityFrameworkFacility );

            if( hasEditPermissionForLiability )
            {
                if( anAccount.Insurance.Coverages.Count > 0 &&
                    anAccount.FinancialClass != null && 
                    anAccount.FinancialClass.Code.Trim() != String.Empty && 
                    FinancialCouncelingService.IsPatientInsured( anAccount.FinancialClass ) &&
                    !anAccount.BillHasDropped )
                {
                    if( FinancialCouncelingService.IsUninsured( anAccount.Facility.Oid,anAccount.FinancialClass ) )
                    {
                        result = NewLiabilityInstanceUsing( typeof( UnInsuredFinancialCounselingView ) );
                    }
                    else
                    {
                        result = NewLiabilityInstanceUsing( typeof( InsuredFinancialCounselingView ) );
                    }
                }
                else
                {
                    result = NewLiabilityInstanceUsing( typeof( IncompleteInsuranceFinancialCounselingView )); 
                    ((IncompleteInsuranceFinancialCounselingView)result).SelectedTab = "LIABILITY";
                }
            }
            else
            {
                result = NewLiabilityInstanceUsing( typeof( IncompleteInsuranceFinancialCounselingView )); 
                ((IncompleteInsuranceFinancialCounselingView)result).SelectedTab = "NO_PERMISSION";
            }

            return result;
        }

        public static ControlView PaymentViewFor( Account anAccount )
        {    
            ControlView result = null;

            User patientAccessUser = User.GetCurrent();
            Extensions.SecurityService.Domain.User securityUser = patientAccessUser.SecurityUser;
            Facility securityFrameworkFacility = new Facility( patientAccessUser.Facility.Code, patientAccessUser.Facility.Description );
            bool hasEditPermissionForPayment  = securityUser.HasPermissionTo( Privilege.Actions.Edit, typeof( Payment ), securityFrameworkFacility );

            if( hasEditPermissionForPayment )
            {
                if( IsIncompletePaymentView( anAccount ) )
                {
                    result = NewPaymentInstanceUsing( typeof( IncompleteInsuranceFinancialCounselingView )); 
                    ((IncompleteInsuranceFinancialCounselingView)result).SelectedTab = "PAYMENT";
                }
                else
                {
                    result = NewPaymentInstanceUsing( typeof( PaymentView ));
                }
            }
            else
            {
                result = NewPaymentInstanceUsing( typeof( IncompleteInsuranceFinancialCounselingView ));
                ((IncompleteInsuranceFinancialCounselingView)result).SelectedTab = "NO_PERMISSION";
            }
            return result;
        }

        #endregion

        #region Properties
        #endregion

        #region Private Methods

        private static bool IsIncompletePaymentView( Account anAccount )
        {
            bool result = false;
            if(  anAccount.BillHasDropped ||
                    
                (anAccount.Insurance.FinancialClass != null &&
                 anAccount.Insurance.FinancialClass.Code != String.Empty &&
                 anAccount.Insurance.Coverages.Count == 0 ) ||

                (anAccount.TotalCurrentAmtDue == 0m &&
                 anAccount.TotalPaid  == 0m &&
                 anAccount.NumberOfMonthlyPayments == 0 ))
            {
                result = true;
            }
            return result;
        }

        private static ControlView NewPaymentInstanceUsing( Type instance )
        {
            ControlView result = null;

            lock( c_syncRootPayment )
            {
                if( c_paymentInstance != null && c_paymentInstance.IsDisposed != true && c_paymentInstance.GetType().Equals( instance ) )
                {
                    result = c_paymentInstance;
                }
                else
                {
                    if( c_paymentInstance != null )
                    {
                        c_paymentInstance.Dispose();
                        c_paymentInstance = null;
                    }
                    c_paymentInstance = Activator.CreateInstance( instance ) as ControlView;
                    result = c_paymentInstance;
                }
            }
            return result;
        }

        private static ControlView NewLiabilityInstanceUsing( Type instance )
        {
            ControlView result = null;

            lock( c_syncRootLiability )
            {
                if( c_liabilityInstance != null && c_liabilityInstance.IsDisposed != true && c_liabilityInstance.GetType().Equals( instance ) )
                {
                    result = c_liabilityInstance;
                }
                else
                {
                    if( c_liabilityInstance != null )
                    {
                        c_liabilityInstance.Dispose();
                        c_liabilityInstance = null;
                    }
                    c_liabilityInstance = Activator.CreateInstance( instance ) as ControlView;
                    result = c_liabilityInstance;
                }
            }
            return result;
        }

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public FinancialCounselingViewFactory()
        {
        }
        #endregion

        #region Data Elements

        private static ControlView c_paymentInstance = null;
        private static ControlView c_liabilityInstance = null;

        private static object c_syncRootPayment = new Object();
        private static object c_syncRootLiability = new Object();

        #endregion

        #region Constants
        #endregion
    }
}
