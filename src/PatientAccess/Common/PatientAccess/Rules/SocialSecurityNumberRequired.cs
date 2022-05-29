using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for SocialSecurityNumberRequired.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class SocialSecurityNumberRequired : LeafRule
    {
        #region Event Handlers
        public event EventHandler SocialSecurityNumberRequiredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            SocialSecurityNumberRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            SocialSecurityNumberRequiredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.SocialSecurityNumberRequiredEvent = null;   
        }

        public override bool ShouldStopProcessing()
        {
            return true;
        }

        public override void ApplyTo( object context )
        {
        }

        public override bool CanBeAppliedTo( object context )
        {
            if (context == null || context.GetType() != typeof(Person)
                && context.GetType().BaseType != typeof(Person)
                && context.GetType().BaseType.BaseType != typeof(Person))
            {
                return true;
            }

            Person aPerson = context as Person;
            if (aPerson != null
                && aPerson.SocialSecurityNumber != null
                && aPerson.SocialSecurityNumber.UnformattedSocialSecurityNumber.Trim() != string.Empty)
            {
                return true;
            }

            if( this.FireEvents && SocialSecurityNumberRequiredEvent != null )
            {
                SocialSecurityNumberRequiredEvent( this, null );
            }

            return false;
         
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public SocialSecurityNumberRequired()
        {
        }
        #endregion

        #region Data Elements
        
        #endregion

        #region Constants
        #endregion
    }
}
