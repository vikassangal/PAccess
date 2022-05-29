using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for InsuredEmployerRequired.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class InsuredEmployerRequired : LeafRule
    {
        #region Event Handlers
        public event EventHandler InsuredEmployerRequiredEvent;
        #endregion
        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            InsuredEmployerRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            InsuredEmployerRequiredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.InsuredEmployerRequiredEvent = null;   
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
           
            if( aPerson != null 
                && aPerson.Employment != null
                && aPerson.Employment.Status != null
                && aPerson.Employment.Status.Description.Trim().Length > 0 )
            {
                if( aPerson.Employment.Employer != null 
                    && aPerson.Employment.Employer.Name.Trim().Length > 0)
                {
                    return true;
                }
                
                if( this.FireEvents && InsuredEmployerRequiredEvent != null )
                {
                    InsuredEmployerRequiredEvent( this, new PropertyChangedArgs( this.AssociatedControl ) );
                }
                return false;
                
            }
            
            return true;
            
        }

        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public InsuredEmployerRequired()
        {
        }
        #endregion

        #region Data Elements
        
        #endregion

        #region Constants
        #endregion
    }
}
