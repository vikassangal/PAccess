using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;
using PatientAccess.Domain.ShortRegistration;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for ConditionCode1Required.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class ConditionCode1Required : LeafRule
    {
        #region Events

        public event EventHandler ConditionCode1RequiredEvent;

        #endregion
        
        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.ConditionCode1RequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.ConditionCode1RequiredEvent -= eventHandler;
            return true;
        }
                                
        public override void UnregisterHandlers()
        {
            this.ConditionCode1RequiredEvent = null;  
        }

        public override bool CanBeAppliedTo( object context )
        {
            if (context == null || context.GetType() != typeof (Account))
            {
                return true;
            }

            Account anAccount = ((Account) context);
            if ( anAccount.ConditionCodes != null &&
                anAccount.ConditionCodes.Count == 0 &&
                IsMedicareSelectedPayor( anAccount ) )
            {
                if ((anAccount.Activity.GetType().Equals(typeof(ShortRegistrationActivity)) ||
                     (anAccount.IsShortRegistered &&
                      (anAccount.KindOfVisit != null) &&
                      (anAccount.KindOfVisit.Code == VisitType.OUTPATIENT))
                    ))
                {
                    if (anAccount.HospitalService != null &&
                        anAccount.HospitalService.Code != HospitalService.COVID_VACCINE_CLINIC)
                    {
                        if (this.FireEvents && ConditionCode1RequiredEvent != null)
                        {
                            this.ConditionCode1RequiredEvent(this, null);
                        }
                        return false;
                    }
                    
                }

                else
                {
                    if (anAccount.HospitalService != null &&
                        anAccount.HospitalService.Code != HospitalService.SPECIMEN_ONLY  &&
                        anAccount.HospitalService.Code != HospitalService.COVID_VACCINE_CLINIC)
                    {
                        if (this.FireEvents && ConditionCode1RequiredEvent != null)
                        {
                            this.ConditionCode1RequiredEvent(this, null);
                        }

                        return false;
                    }
                }
            }
            return true;
        }

        public override void ApplyTo(object context)
        {
            
        }
     
        public override bool ShouldStopProcessing()
        {
            return false;
        }

        #endregion

        #region Properties
        #endregion

        #region Private Methods
        private bool IsMedicareSelectedPayor( Account anAccount )    
        {
            bool result = false;
            
            if( anAccount.Insurance.CoverageFor(CoverageOrder.PRIMARY_OID) != null &&
                anAccount.Insurance.CoverageFor(CoverageOrder.PRIMARY_OID).InsurancePlan != null )
            {
                string planID = anAccount.Insurance.CoverageFor(CoverageOrder.PRIMARY_OID).InsurancePlan.PlanID.Trim();
    
                if( planID.Length >= 2 )
                {
                    if (planID.Substring(0, 3).ToUpper().Equals(InsurancePlan.MSP_PAYORCODE))
                    {
                        result = true;
                    }
                    else if( planID.Length >= 3 && planID.Substring( 0, 3 ) == "535" )
                    {     
                        result = true;
                    }
                }
            }
        
            return result;       
        }    
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public ConditionCode1Required()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}
