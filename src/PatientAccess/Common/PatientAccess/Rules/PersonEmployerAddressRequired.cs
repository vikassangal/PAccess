using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for PersonEmployerAddressRequired.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class PersonEmployerAddressRequired : LeafRule
    {
        #region Event Handlers
        public event EventHandler PersonEmployerAddressRequiredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            PersonEmployerAddressRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            PersonEmployerAddressRequiredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.PersonEmployerAddressRequiredEvent = null;   
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
                && aPerson.Employment.Status.Description.Trim().Length > 0 ) &&
                aPerson.Employment.Status.Code != EmploymentStatus.NOT_EMPLOYED_CODE )
            {
                if( aPerson.Employment.Employer != null )
                {
                    cp = aPerson.Employment.Employer.PartyContactPoint;
                }

                if( cp != null && cp.Address != null  && cp.Address.Address1 != string.Empty)
                {
                    return true;
                }            
                else
                {
                    if( this.FireEvents && PersonEmployerAddressRequiredEvent != null )
                    {
                        PersonEmployerAddressRequiredEvent( this, new PropertyChangedArgs( this.AssociatedControl ) );
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
        public PersonEmployerAddressRequired()
        {
        }
        #endregion

        #region Data Elements
        
        #endregion

        #region Constants
        #endregion
    }
}
