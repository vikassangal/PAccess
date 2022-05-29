using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for PersonPhoneNumberRequired.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class PersonPhoneNumberRequired : LeafRule
    {
        #region Event Handlers
        public event EventHandler PersonPhoneNumberRequiredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            PersonPhoneNumberRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            PersonPhoneNumberRequiredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.PersonPhoneNumberRequiredEvent = null;   
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
        /// <param PhoneNumber="context"></param>
        /// <returns></returns>
        public override bool CanBeAppliedTo( object context )
        {
            if( context == null || context.GetType() != typeof( Person )
                && context.GetType().BaseType != typeof( Person ))
            {
                return true;
            }

            Person aPerson = context as Person;
           
            if( aPerson != null )  
            {
                ContactPoint cp = null;

                if( context.GetType() == typeof( Guarantor ) )                  
                {
                    cp = aPerson.ContactPointWith( TypeOfContactPoint.NewMailingContactPointType() );
                }
                else
                {
                    cp = aPerson.ContactPointWith( TypeOfContactPoint.NewPhysicalContactPointType() );
                }

                if( cp.PhoneNumber != null
                    && cp.PhoneNumber.Number != null 
                    && cp.PhoneNumber.Number.Trim().Length > 0)
                {
                    return true;
                }                
            }

            if( this.FireEvents && PersonPhoneNumberRequiredEvent != null )
            {
                PersonPhoneNumberRequiredEvent( this, new PropertyChangedArgs( this.AssociatedControl ) );
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
        public PersonPhoneNumberRequired()
        {
        }
        #endregion

        #region Data Elements
        
        #endregion

        #region Constants
        #endregion
    }
}
