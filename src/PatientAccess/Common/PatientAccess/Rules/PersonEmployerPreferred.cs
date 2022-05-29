using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for PersonEmployerPreferred.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class PersonEmployerPreferred : LeafRule
    {
        #region Event Handlers
        public event EventHandler PersonEmployerPreferredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            PersonEmployerPreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            PersonEmployerPreferredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.PersonEmployerPreferredEvent = null;   
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
        /// <param Employer="context"></param>
        /// <returns></returns>
        public override bool CanBeAppliedTo( object context )
        {
            if( context == null || context.GetType() != typeof( Person )
                && context.GetType().BaseType != typeof( Person ))
            {
                return true;
            }

            Person aPerson = context as Person;
           
            if( ( aPerson != null 
                  && aPerson.Employment != null
                  && aPerson.Employment.Status != null
                  && aPerson.Employment.Status.Description.Trim().Length > 0 ) &&
				  aPerson.Employment.Status.Code != EmploymentStatus.NOT_EMPLOYED_CODE )
            {
                if( aPerson.Employment.Employer != null 
                    && aPerson.Employment.Employer.Name.Trim().Length > 0)
                {
                    return true;
                }
                else
                {
                    if(this.FireEvents && PersonEmployerPreferredEvent != null )
                    {
                        PersonEmployerPreferredEvent( this, new PropertyChangedArgs( this.AssociatedControl ) );
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
        public PersonEmployerPreferred()
        {
        }
        #endregion

        #region Data Elements
        
        #endregion

        #region Constants
        #endregion
    }
}
