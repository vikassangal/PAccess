using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for GuarantorValidationFieldsProvided.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class GuarantorValidationFieldsProvided : LeafRule
    {
        #region Event Handlers
        public event EventHandler GuarantorValidationFieldsProvidedEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            GuarantorValidationFieldsProvidedEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            GuarantorValidationFieldsProvidedEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.GuarantorValidationFieldsProvidedEvent = null;   
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
        /// <param FirstName="context"></param>
        /// <returns></returns>
        public override bool CanBeAppliedTo( object context )
        {
            if( context == null || context.GetType() != typeof( Guarantor ) )
            {
                return true;
            }

            Guarantor aGuarantor = context as Guarantor;

            ContactPoint cp = aGuarantor.ContactPointWith( TypeOfContactPoint.NewMailingContactPointType() );
            Address addr = null;
            
            if( cp != null )
            {
                addr = cp.Address;
            }
           
            if( aGuarantor != null 
                && aGuarantor.LastName != null
                && aGuarantor.LastName.Trim().Length > 0
                && aGuarantor.FirstName != null
                && aGuarantor.FirstName.Trim().Length > 0 
                && addr != null
                && addr.Address1 != null
                && addr.Address1.Trim().Length > 0)
            {
                return true;
            }
            else
            {
                if( this.FireEvents && GuarantorValidationFieldsProvidedEvent != null )
                {
                    GuarantorValidationFieldsProvidedEvent( this, new PropertyChangedArgs( this.AssociatedControl ) );
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
        public GuarantorValidationFieldsProvided()
        {
        }
        #endregion

        #region Data Elements
        
        #endregion

        #region Constants
        #endregion
    }
}
