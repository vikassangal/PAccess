using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for InsuredAddressRequired.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class InsuredAddressRequired : LeafRule
    {
        #region Event Handlers
        public event EventHandler InsuredAddressRequiredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            InsuredAddressRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            InsuredAddressRequiredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.InsuredAddressRequiredEvent = null;
        }   

        public override bool ShouldStopProcessing()
        {
            return true;
        }

        public override void ApplyTo( object context )
        {
        }

        /// <summary>
        /// CanBeAppliedTo - this rule is a member of a composite (to InsuredInfo).
        /// Refer to InsuredInfo.cs for additional logic applied to this rule.
        /// </summary>
        /// <param Relationship="context"></param>
        /// <returns></returns>
        public override bool CanBeAppliedTo( object context )
        {
            if( context == null || context.GetType() != typeof( Insured ) )
            {
                return true;
            }

            Insured anInsured = context as Insured;
            
            ContactPoint cp = anInsured.ContactPointWith( TypeOfContactPoint.NewPhysicalContactPointType() );
           
            if( cp != null && cp.Address != null  
                && cp.Address.Address1 != null
                && cp.Address.Address1.Trim().Length > 0 )
            {
                return true;
            }
            else
            {
                if( this.FireEvents && InsuredAddressRequiredEvent != null )
                {
                    InsuredAddressRequiredEvent( this, new PropertyChangedArgs( this.AssociatedControl ) );
                }
                return false;
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
        public InsuredAddressRequired()
        {
        }
        #endregion

        #region Data Elements
        
        #endregion

        #region Constants
        #endregion
    }
}
