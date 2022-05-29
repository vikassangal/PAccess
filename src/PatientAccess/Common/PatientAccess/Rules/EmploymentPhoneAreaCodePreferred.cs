using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for EmploymentPhoneAreaCodePreferred.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class EmploymentPhoneAreaCodePreferred : LeafRule
    {
        #region Event Handlers
        public event EventHandler EmploymentPhoneAreaCodePreferredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            EmploymentPhoneAreaCodePreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            EmploymentPhoneAreaCodePreferredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.EmploymentPhoneAreaCodePreferredEvent  = null;   
        }

        public override bool ShouldStopProcessing()
        {
            return true;
        }

        public override void ApplyTo( object context )
        {
        }

        /// <summary>
        /// CanBeAppliedTo - this rule is a member of a composite (to EmploymentInfo).
        /// Refer to EmploymentInfo.cs for additional logic applied to this rule.
        /// </summary>
        /// <param PhoneAreaCode="context"></param>
        /// <returns></returns>
        public override bool CanBeAppliedTo( object context )
        {
            if( context == null || context.GetType() != typeof( Employment )
                && context.GetType().BaseType != typeof( Employment ))
            {
                return true;
            }

            Employment anEmployment = context as Employment;
           
            if(  anEmployment != null
                && anEmployment.Status != null &&
                ( anEmployment.Status.Code == EmploymentStatus.NOT_EMPLOYED_CODE 
                    || anEmployment.Status.Code == EmploymentStatus.RETIRED_CODE
                    || anEmployment.Status.Code == EmploymentStatus.SELF_EMPLOYED_CODE
                    || anEmployment.Status.Code == string.Empty) )
            {
                return true;
            }
            else
            {
                if( anEmployment != null
                    && anEmployment.Employer != null
                    && anEmployment.Employer.PartyContactPoint != null
                    && anEmployment.Employer.PartyContactPoint.PhoneNumber != null
                    && anEmployment.Employer.PartyContactPoint.PhoneNumber.AreaCode != null 
                    && anEmployment.Employer.PartyContactPoint.PhoneNumber.AreaCode.Trim().Length > 0 ) 
                {
                    return true;
                }            
                else
                {
                    if( this.FireEvents && EmploymentPhoneAreaCodePreferredEvent != null )
                    {
                        EmploymentPhoneAreaCodePreferredEvent( this, new PropertyChangedArgs( this.AssociatedControl ) );
                    }

                    return false;
                }
            }
        }

        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public EmploymentPhoneAreaCodePreferred()
        {
        }
        #endregion

        #region Data Elements
        
        #endregion

        #region Constants
        #endregion
    }
}
