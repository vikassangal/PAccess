using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for BenVerCertSSN.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class BenVerCertSSN : LeafRule
    {
        #region Event Handlers
        public event EventHandler BenVerCertSSNEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            BenVerCertSSNEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            BenVerCertSSNEvent -= eventHandler;
            return true;
        }
                                
        public override void UnregisterHandlers()
        {
            this.BenVerCertSSNEvent = null;  
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
            if( context == null )
            {
                return true;
            }
            
            if( this.AssociatedControl == null
                || ((InsurancePlanCategory)AssociatedControl).Oid == InsurancePlanCategory.PLANCATEGORY_GOVERNMENT_MEDICAID
                || ((InsurancePlanCategory)AssociatedControl).Oid == InsurancePlanCategory.PLANCATEGORY_GOVERNMENT_MEDICARE
                || ((InsurancePlanCategory)AssociatedControl).Oid == InsurancePlanCategory.PLANCATEGORY_WORKERS_COMPENSATION
                || ((InsurancePlanCategory)AssociatedControl).Oid == InsurancePlanCategory.PLANCATEGORY_SELF_PAY )
            {
                return true;
            }            

            if( ((InsurancePlanCategory)AssociatedControl).Oid == InsurancePlanCategory.PLANCATEGORY_COMMERCIAL )
            {
                CoverageForCommercialOther aCoverage = context as CoverageForCommercialOther;
                if( aCoverage.CertSSNID == String.Empty )
                {
                    if( this.FireEvents && BenVerCertSSNEvent != null )
                    {
                        BenVerCertSSNEvent( this, new PropertyChangedArgs( this.AssociatedControl ) );
                    }
                    return false;
                }
            }

            if( ((InsurancePlanCategory)AssociatedControl).Oid == InsurancePlanCategory.PLANCATEGORY_GOVERNMENT_OTHER )
            {
                GovernmentOtherCoverage aCoverage = context as GovernmentOtherCoverage;

                if( aCoverage.CertSSNID == String.Empty )
                {
                    if( this.FireEvents && BenVerCertSSNEvent != null )
                    {
                        BenVerCertSSNEvent( this, new PropertyChangedArgs( this.AssociatedControl ) );
                    }
                    return false;
                }
            }

            if( ((InsurancePlanCategory)AssociatedControl).Oid == InsurancePlanCategory.PLANCATEGORY_OTHER )
            {
                OtherCoverage aCoverage = context as OtherCoverage;
                if( aCoverage.CertSSNID == String.Empty )
                {
                    if( this.FireEvents && BenVerCertSSNEvent != null )
                    {
                        BenVerCertSSNEvent( this, new PropertyChangedArgs( this.AssociatedControl ) );
                    }
                    return false;
                }
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
        public BenVerCertSSN()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
