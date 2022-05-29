using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for PersonEmployerAddressPreferred.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class PersonEmployerAddressPreferred : LeafRule
    {
        #region Event Handlers
        public event EventHandler PersonEmployerAddressPreferredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            PersonEmployerAddressPreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            PersonEmployerAddressPreferredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.PersonEmployerAddressPreferredEvent = null;   
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
        /// <param Relationship="context"></param>
        /// <returns></returns>
        public override bool CanBeAppliedTo( object context )
        {
            if( context == null || context.GetType() != typeof( Person )
                && context.GetType().BaseType != typeof( Person ))
            {
                return true;
            }

            Person aPerson = context as Person;
            ContactPoint cp = null;
                       
            if( ( aPerson != null 
                && aPerson.Employment != null
                && aPerson.Employment.Status != null
                && aPerson.Employment.Status.Description.Trim().Length > 0 ) 
                && aPerson.Employment.Status.Code != EmploymentStatus.NOT_EMPLOYED_CODE
                && aPerson.Employment.Status.Code != EmploymentStatus.RETIRED_CODE
                && aPerson.Employment.Status.Code != EmploymentStatus.SELF_EMPLOYED_CODE)
            {
                if( aPerson.Employment.Employer != null )
                {
                    cp = aPerson.Employment.Employer.PartyContactPoint;
                }

                if( cp != null && cp.Address != null && 
                    (cp.Address.Address1 != string.Empty ||cp.Address.City != string.Empty)
                    )
                {
                    return true;
                }
                else
                {
                    if( this.FireEvents && PersonEmployerAddressPreferredEvent != null )
                    {
                        PersonEmployerAddressPreferredEvent( this, new PropertyChangedArgs( this.AssociatedControl ) );
                    }
                    return false;
                }
            }
            else
            {
                return true;
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
        public PersonEmployerAddressPreferred()
        {
        }
        #endregion

        #region Data Elements
        
        #endregion

        #region Constants
        #endregion
    }
}
