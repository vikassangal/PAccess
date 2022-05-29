using System;
using System.Text;
using PatientAccess.Domain;
using PatientAccess.Domain.QuickAccountCreation;
using PatientAccess.Rules;
using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI.HelperClasses
{
    public class AdmitDateToInsuranceValidation
    {

        #region Events
        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        public static void CheckAdmitDateToInsurance( Account account, string viewName )
        {
            bool primaryPlanHasChanged = false;
            bool secondaryPlanHasChanged = false;

            RuleEngine.GetInstance().RegisterEvent( typeof( AdmitDateToPrimaryPlanDates ), account, viewName, null );
            RuleEngine.GetInstance().RegisterEvent( typeof( AdmitDateToSecondaryPlanDates ), account, viewName, null );

            // SR 45229 - if the plan is invalid, try to retrieve a valid one from the collection of contracts on the plan

            if( account.Insurance != null &&
                account.Insurance.PrimaryCoverage != null )
            {
                InsurancePlan primaryPlan = account.Insurance.PrimaryCoverage.InsurancePlan;
                InsurancePlanContract primaryPlanContract = primaryPlan.GetBestPlanContractFor( account.AdmitDate );

                // if a valid primary PlanContract is found 

                if( CheckIfPlanHasChanged( primaryPlanContract, primaryPlan ) )
                {
                    primaryPlanHasChanged = true;
                    primaryPlan.UpdateFromPlanContract( primaryPlanContract );
                }
            }

            if( account.Insurance != null &&
                account.Insurance.SecondaryCoverage != null )
            {

                InsurancePlan secondaryPlan = account.Insurance.SecondaryCoverage.InsurancePlan;
                InsurancePlanContract secondaryPlanContract = secondaryPlan.GetBestPlanContractFor( account.AdmitDate );

                // if a valid secondary PlanContract is found 

                if( CheckIfPlanHasChanged( secondaryPlanContract, secondaryPlan ) )
                {
                    secondaryPlanHasChanged = true;
                    secondaryPlan.UpdateFromPlanContract( secondaryPlanContract );
                }
            }

            // determine if the current plan(s) is(are) valid for the AdmitDate specified

            bool primaryIsValid = RuleEngine.GetInstance().EvaluateRule( typeof( AdmitDateToPrimaryPlanDates ), account, viewName );
            bool secondaryIsValid = RuleEngine.GetInstance().EvaluateRule( typeof( AdmitDateToSecondaryPlanDates ), account, viewName );

            if( !primaryIsValid || !secondaryIsValid || primaryPlanHasChanged || secondaryPlanHasChanged )
            {
                Activity activity = account.Activity;
                if ( activity is QuickAccountCreationActivity ||
                    activity is QuickAccountMaintenanceActivity
            )
                {
                    ShowInsuranceMessageForQuickAccountCreationActivity( account, primaryIsValid, secondaryIsValid,
                                                                        primaryPlanHasChanged, secondaryPlanHasChanged );
                }
                else
                {
                    ShowInsuranceMessage(account, primaryIsValid, secondaryIsValid, primaryPlanHasChanged,
                                         secondaryPlanHasChanged);
                }
            }

            RuleEngine.GetInstance().UnregisterEvent( typeof( AdmitDateToPrimaryPlanDates ), account, null );
            RuleEngine.GetInstance().UnregisterEvent( typeof( AdmitDateToSecondaryPlanDates ), account, null );

            RuleEngine.GetInstance().ClearActionsForRule( typeof( AdmitDateToPrimaryPlanDates ) );
            RuleEngine.GetInstance().ClearActionsForRule( typeof( AdmitDateToSecondaryPlanDates ) );
        }

        #endregion

        #region Properties
        #endregion

        #region Private Methods

        private static bool CheckIfPlanHasChanged( InsurancePlanContract aPlanContract, InsurancePlan aPlan )
        {
            if( !( aPlanContract.EffectiveOn == DateTime.MinValue &&
                        aPlanContract.TerminatedOn == DateTime.MinValue &&
                        aPlanContract.ApprovedOn == DateTime.MinValue &&
                        aPlanContract.CanceledOn == DateTime.MinValue )

                    && // and it does not match the existing plan values, use it

                    !( aPlanContract.EffectiveOn == aPlan.EffectiveOn &&
                        aPlanContract.TerminatedOn == aPlan.TerminatedOn &&
                        aPlanContract.ApprovedOn == aPlan.ApprovedOn &&
                        aPlanContract.CanceledOn == aPlan.CanceledOn ) )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Format a message indicating the status of the insurance plan relative to the admit date
        /// </summary>
        /// <param name="account"></param>
        /// <param name="primaryIsValid"></param>
        /// <param name="secondaryIsValid"></param>
        /// <param name="primaryChanged"></param>
        /// <param name="secondaryChanged"></param>
        private static void ShowInsuranceMessage( Account account, bool primaryIsValid, bool secondaryIsValid, bool primaryChanged, bool secondaryChanged )
        {
            bool primaryMessage = false;

            StringBuilder sb = new StringBuilder();

            if( !primaryIsValid )
            {
                Coverage primaryCoverage = account.Insurance.CoverageFor( CoverageOrder.PRIMARY_OID );

                if( primaryCoverage != null )
                {
                    primaryMessage = true;
                    string fromDate = NO_DATE;
                    string toDate = NO_DATE;

                    if( primaryCoverage.InsurancePlan.EffectiveOn != DateTime.MinValue )
                    {
                        fromDate = primaryCoverage.InsurancePlan.EffectiveOn.ToString( MM_DD_YYYY );
                    }

                    if( primaryCoverage.InsurancePlan.AdjustedTerminationDate != DateTime.MinValue )
                    {
                        if( primaryCoverage.InsurancePlan.AdjustedCancellationDate != DateTime.MinValue
                            && primaryCoverage.InsurancePlan.AdjustedCancellationDate < primaryCoverage.InsurancePlan.AdjustedTerminationDate )
                        {
                            toDate = primaryCoverage.InsurancePlan.AdjustedCancellationDate.ToString( MM_DD_YYYY );
                        }
                        else
                        {
                            toDate = primaryCoverage.InsurancePlan.AdjustedTerminationDate.ToString( MM_DD_YYYY );
                        }
                    }

                    sb.Append( "The Admit date " );
                    sb.Append( account.AdmitDate.ToString( MM_DD_YYYY ) );
                    sb.Append( " conflicts with the coverage period (" );
                    sb.Append( fromDate );
                    sb.Append( " to " );
                    sb.Append( toDate );
                    sb.Append( ") of \r\nthe primary insurance Plan (Plan ID: " );
                    sb.Append( primaryCoverage.InsurancePlan.PlanID );
                    sb.Append( ") based on the following rules:" );
                    sb.Append( "\r\n\r\n" );
                    sb.Append( "          \"A valid insurance plan must have become effective on or before the admission date AND\r\n" );
                    sb.Append( "            the insurance plan must not have expired on or before the admission date.\"" );
                    sb.Append( "\r\n\r\n" );
                    sb.Append( "Before finishing this activity, either modify the Admit date on the Demographics screen OR\r\n" );
                    sb.Append( "modify the Plan ID on the Insurance screen." );
                }
            }
            else if( primaryChanged )
            {
                primaryMessage = true;
                sb.Append( "The contract for the primary insurance plan has been updated based on the Admit date.\r\n\r\n" );
                sb.Append( "No other information has been updated. " );
                sb.Append( "If you are creating a new account from a previous account,\r\n" );
                sb.Append( "you may want to check to ensure that the insurance plan is still valid." );
            }


            if( !secondaryIsValid )
            {
                string fromDate = NO_DATE;
                string toDate = NO_DATE;

                Coverage secondaryCoverage = account.Insurance.CoverageFor( CoverageOrder.SECONDARY_OID );

                if( secondaryCoverage.InsurancePlan.EffectiveOn != DateTime.MinValue )
                {
                    fromDate = secondaryCoverage.InsurancePlan.EffectiveOn.ToString( MM_DD_YYYY );
                }

                if( secondaryCoverage.InsurancePlan.AdjustedTerminationDate != DateTime.MinValue )
                {
                    if( secondaryCoverage.InsurancePlan.AdjustedCancellationDate != DateTime.MinValue
                        && secondaryCoverage.InsurancePlan.AdjustedCancellationDate < secondaryCoverage.InsurancePlan.AdjustedTerminationDate )
                    {
                        toDate = secondaryCoverage.InsurancePlan.AdjustedCancellationDate.ToString( MM_DD_YYYY );
                    }
                    else
                    {
                        toDate = secondaryCoverage.InsurancePlan.AdjustedTerminationDate.ToString( MM_DD_YYYY );
                    }
                }

                if( primaryMessage )
                {
                    sb.Append( "\r\n\r\n" );
                    sb.Append( "________________________________________________________________" );
                    sb.Append( "________________________________________________________________" );
                    sb.Append( "\r\n\r\n" );
                }

                if( secondaryCoverage != null )
                {
                    sb.Append( "The Admit date " );
                    sb.Append( account.AdmitDate.ToString( MM_DD_YYYY ) );
                    sb.Append( " conflicts with the coverage period (" );
                    sb.Append( fromDate );
                    sb.Append( " to " );
                    sb.Append( toDate );
                    sb.Append( ") of \r\nthe secondary insurance Plan (Plan ID: " );
                    sb.Append( secondaryCoverage.InsurancePlan.PlanID );
                    sb.Append( ") based on the following rules:" );
                    sb.Append( "\r\n\r\n" );
                    sb.Append( "          \"A valid insurance plan must have become effective on or before the admission date AND\r\n" );
                    sb.Append( "            the insurance plan must not have expired on or before the admission date.\"" );
                    sb.Append( "\r\n\r\n" );
                    sb.Append( "Before finishing this activity, either modify the Admit date on the Demographics screen OR\r\n" );
                    sb.Append( "modify the Plan ID on the Insurance screen." );
                }
            }
            else if( secondaryChanged )
            {
                if( primaryMessage )
                {
                    sb.Append( "\r\n\r\n" );
                    sb.Append( "________________________________________________________________" );
                    sb.Append( "________________________________________________________________" );
                    sb.Append( "\r\n\r\n" );
                }

                sb.Append( "The contract for the secondary insurance plan has been updated based on the Admit date.\r\n\r\n" );
                sb.Append( "No other information has been updated. " );
                sb.Append( "If you are creating a new account from a previous account,\r\n" );
                sb.Append( "you may want to check to ensure that the insurance plan is still valid." );

            }

            if( sb.Length > 0 )
            {
                InsuranceErrorsDialog insuranceErrorsSummary = new InsuranceErrorsDialog();
                insuranceErrorsSummary.Text = "Warning";
                insuranceErrorsSummary.HeaderText = "The following error(s) and/or update(s) have occurred. Please read carefully and make corrections as recommended.";
                insuranceErrorsSummary.ErrorText = sb.ToString();
                insuranceErrorsSummary.UpdateView();

                try
                {
                    insuranceErrorsSummary.ShowDialog();
                }
                finally
                {
                    insuranceErrorsSummary.Dispose();
                }
            }
        }

        /// <summary>
        /// Format a message indicating the status of the insurance plan relative to the admit date
        /// </summary>
        /// <param name="account"></param>
        /// <param name="primaryIsValid"></param>
        /// <param name="secondaryIsValid"></param>
        /// <param name="primaryChanged"></param>
        /// <param name="secondaryChanged"></param>
        private static void ShowInsuranceMessageForQuickAccountCreationActivity( Account account, bool primaryIsValid, bool secondaryIsValid, bool primaryChanged, bool secondaryChanged )
        {
            bool primaryMessage = false;

            StringBuilder sb = new StringBuilder();

            if ( !primaryIsValid )
            {
                Coverage primaryCoverage = account.Insurance.CoverageFor( CoverageOrder.PRIMARY_OID );

                if ( primaryCoverage != null )
                {
                    primaryMessage = true;
                    string fromDate = NO_DATE;
                    string toDate = NO_DATE;

                    if ( primaryCoverage.InsurancePlan.EffectiveOn != DateTime.MinValue )
                    {
                        fromDate = primaryCoverage.InsurancePlan.EffectiveOn.ToString( MM_DD_YYYY );
                    }

                    if ( primaryCoverage.InsurancePlan.AdjustedTerminationDate != DateTime.MinValue )
                    {
                        if ( primaryCoverage.InsurancePlan.AdjustedCancellationDate != DateTime.MinValue
                            && primaryCoverage.InsurancePlan.AdjustedCancellationDate < primaryCoverage.InsurancePlan.AdjustedTerminationDate )
                        {
                            toDate = primaryCoverage.InsurancePlan.AdjustedCancellationDate.ToString( MM_DD_YYYY );
                        }
                        else
                        {
                            toDate = primaryCoverage.InsurancePlan.AdjustedTerminationDate.ToString( MM_DD_YYYY );
                        }
                    }

                    sb.Append( "The Admit date " );
                    sb.Append( account.AdmitDate.ToString( MM_DD_YYYY ) );
                    sb.Append( " conflicts with the coverage period (" );
                    sb.Append( fromDate );
                    sb.Append( " to " );
                    sb.Append( toDate );
                    sb.Append( ") of \r\nthe primary insurance Plan (Plan ID: " );
                    sb.Append( primaryCoverage.InsurancePlan.PlanID );
                    sb.Append( ") based on the following rules:" );
                    sb.Append( "\r\n\r\n" );
                    sb.Append( " A valid plan ID must have become effective on or before the admission date. " );
                    sb.Append( "\r\n\r\n" );
                    sb.Append( " A valid plan ID must not have expired on or after the admission date." );
                    sb.Append( "\r\n\r\n" );
                    sb.Append( "Before finishing this activity, either modify the Admit date OR\r\n" );
                    sb.Append( "the Plan ID on the Quick Account Creation screen." );
                }
            }
            else if ( primaryChanged )
            {
                primaryMessage = true;
                sb.Append( "The contract for the primary insurance plan has been updated based on the Admit date.\r\n\r\n" );
                sb.Append( "No other information has been updated. " );
                sb.Append( "If you are creating a new account from a previous account,\r\n" );
                sb.Append( "you may want to check to ensure that the insurance plan is still valid." );
            }


            if ( !secondaryIsValid )
            {
                string fromDate = NO_DATE;
                string toDate = NO_DATE;

                Coverage secondaryCoverage = account.Insurance.CoverageFor( CoverageOrder.SECONDARY_OID );

                if ( secondaryCoverage.InsurancePlan.EffectiveOn != DateTime.MinValue )
                {
                    fromDate = secondaryCoverage.InsurancePlan.EffectiveOn.ToString( MM_DD_YYYY );
                }

                if ( secondaryCoverage.InsurancePlan.AdjustedTerminationDate != DateTime.MinValue )
                {
                    if ( secondaryCoverage.InsurancePlan.AdjustedCancellationDate != DateTime.MinValue
                        && secondaryCoverage.InsurancePlan.AdjustedCancellationDate < secondaryCoverage.InsurancePlan.AdjustedTerminationDate )
                    {
                        toDate = secondaryCoverage.InsurancePlan.AdjustedCancellationDate.ToString( MM_DD_YYYY );
                    }
                    else
                    {
                        toDate = secondaryCoverage.InsurancePlan.AdjustedTerminationDate.ToString( MM_DD_YYYY );
                    }
                }

                if ( primaryMessage )
                {
                    sb.Append( "\r\n\r\n" );
                    sb.Append( "________________________________________________________________" );
                    sb.Append( "________________________________________________________________" );
                    sb.Append( "\r\n\r\n" );
                }

                if ( secondaryCoverage != null )
                {
                    sb.Append( "The Admit date " );
                    sb.Append( account.AdmitDate.ToString( MM_DD_YYYY ) );
                    sb.Append( " conflicts with the coverage period (" );
                    sb.Append( fromDate );
                    sb.Append( " to " );
                    sb.Append( toDate );
                    sb.Append( ") of \r\nthe secondary insurance Plan (Plan ID: " );
                    sb.Append( secondaryCoverage.InsurancePlan.PlanID );
                    sb.Append( ") based on the following rules:" );
                    sb.Append( "\r\n\r\n" );
                    sb.Append( "          \"A valid insurance plan must have become effective on or before the admission date AND\r\n" );
                    sb.Append( "            the insurance plan must not have expired on or before the admission date.\"" );
                    sb.Append( "\r\n\r\n" );
                    sb.Append( "Before finishing this activity, either modify the Admit date on the Demographics screen OR\r\n" );
                    sb.Append( "modify the Plan ID on the Insurance screen." );
                }
            }
            else if ( secondaryChanged )
            {
                if ( primaryMessage )
                {
                    sb.Append( "\r\n\r\n" );
                    sb.Append( "________________________________________________________________" );
                    sb.Append( "________________________________________________________________" );
                    sb.Append( "\r\n\r\n" );
                }

                sb.Append( "The contract for the secondary insurance plan has been updated based on the Admit date.\r\n\r\n" );
                sb.Append( "No other information has been updated. " );
                sb.Append( "If you are creating a new account from a previous account,\r\n" );
                sb.Append( "you may want to check to ensure that the insurance plan is still valid." );

            }

            if ( sb.Length > 0 )
            {
                InsuranceErrorsDialog insuranceErrorsSummary = new InsuranceErrorsDialog();
                insuranceErrorsSummary.Text = "Warning";
                insuranceErrorsSummary.HeaderText = "The following error(s) and/or update(s) have occurred. Please read carefully and make corrections as recommended.";
                insuranceErrorsSummary.ErrorText = sb.ToString();
                insuranceErrorsSummary.UpdateView();

                try
                {
                    insuranceErrorsSummary.ShowDialog();
                }
                finally
                {
                    insuranceErrorsSummary.Dispose();
                }
            }
        }
    

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        #endregion

        #region Component Designer generated code
        #endregion

        #region Data Elements
        #endregion

        #region Constants

        private const string NO_DATE = "--/--/----",
                             MM_DD_YYYY = "MM/dd/yyyy";

        #endregion             
    }
}
