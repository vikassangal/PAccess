using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for PersonDriversLicenseStateRequired.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class PersonDriversLicenseStateRequired : LeafRule
    {
        #region Event Handlers
        public event EventHandler PersonDriversLicenseStateRequiredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            PersonDriversLicenseStateRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            PersonDriversLicenseStateRequiredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.PersonDriversLicenseStateRequiredEvent = null;   
        }

        public override bool ShouldStopProcessing()
        {
            return true;
        }

        public override void ApplyTo( object context )
        {
        }

        /// <summary>
        /// CanBeAppliedTo - this rule is a member of a composite (to PersonInfo).
        /// Refer to PersonInfo.cs for additional logic applied to this rule.
        /// </summary>
        /// <param DriversLicenseState="context"></param>
        /// <returns></returns>
        public override bool CanBeAppliedTo( object context )
        {
            if( context == null || context.GetType() != typeof( Person )
                && context.GetType().BaseType != typeof( Person ))
            {
                return true;
            }

            Person aPerson = context as Person;
           
            if( aPerson == null 
                || aPerson.DriversLicense == null   
                || aPerson.DriversLicense.Number == null
                || aPerson.DriversLicense.Number.Trim().Length == 0 )
            {
                return true;
            }

            if( aPerson.DriversLicense.State != null
                && aPerson.DriversLicense.State.Description.Trim().Length > 0)
            {
                return true;
            }
            else
            {
                if( this.FireEvents && PersonDriversLicenseStateRequiredEvent != null )
                {
                    PersonDriversLicenseStateRequiredEvent( this, new PropertyChangedArgs( this.AssociatedControl ) );
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
        public PersonDriversLicenseStateRequired()
        {
        }
        #endregion

        #region Data Elements
        
        #endregion

        #region Constants
        #endregion
    }
}
