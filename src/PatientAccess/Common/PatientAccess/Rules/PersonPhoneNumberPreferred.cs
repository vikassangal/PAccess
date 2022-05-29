using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for PersonPhoneNumberPreferred.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class PersonPhoneNumberPreferred : LeafRule
    {
        #region Event Handlers
        public event EventHandler PersonPhoneNumberPreferredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            PersonPhoneNumberPreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            PersonPhoneNumberPreferredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.PersonPhoneNumberPreferredEvent = null;   
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

            if( this.FireEvents && PersonPhoneNumberPreferredEvent != null )
            {
                PersonPhoneNumberPreferredEvent( this, new PropertyChangedArgs( this.AssociatedControl ) );
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
        public PersonPhoneNumberPreferred()
        {
        }
        #endregion

        #region Data Elements
        
        #endregion

        #region Constants
        #endregion
    }
}
