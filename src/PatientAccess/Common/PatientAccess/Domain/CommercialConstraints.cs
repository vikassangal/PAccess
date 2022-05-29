using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;

namespace PatientAccess.Domain
{
    [Serializable]
    public class CommercialConstraints : CoverageConstraints
	{
        #region Event Handlers

        #endregion
      
        #region Methods

        public override void ResetFieldsToDefault( bool resetTracking )
        {

            this.i_EligibilityPhone = string.Empty;
            this.i_EffectiveDateForInsured = DateTime.MinValue;
            this.i_InsuranceCompanyRepName = string.Empty;
            this.i_TerminationDateForInsured = DateTime.MinValue;
            this.i_BenefitsCategoryDetails = new ArrayList();
            this.i_KindOfConstraint = new KindOfConstraint();
            this.i_TypeOfProduct = new TypeOfProduct();

            if( resetTracking )
                this.ResetChangeTracking();

        }

        #endregion
      
        #region Properties
        
        public string EligibilityPhone
        {
            get 
            { 
                return i_EligibilityPhone; 
            }
            set 
            {
                Debug.Assert( value != null );
                this.SetAndTrack<string>( ref this.i_EligibilityPhone, value, MethodBase.GetCurrentMethod() );
            }
        }        

        public DateTime EffectiveDateForInsured
        {
            get
            {
                return i_EffectiveDateForInsured;
            }
            set
            {
                this.SetAndTrack<DateTime>(ref this.i_EffectiveDateForInsured, value, MethodBase.GetCurrentMethod() );
            }
        }

        public string InsuranceCompanyRepName
        {
            get
            {
                return i_InsuranceCompanyRepName;
            }
            set
            {
                Debug.Assert( value != null );
                this.SetAndTrack<string>(ref this.i_InsuranceCompanyRepName, value, MethodBase.GetCurrentMethod() );
            }
        }

        public DateTime TerminationDateForInsured
        {
            get
            {
                return i_TerminationDateForInsured;
            }
            set
            {
                this.SetAndTrack<DateTime>(ref this.i_TerminationDateForInsured, value, MethodBase.GetCurrentMethod() );
            }
        }

        public ArrayList BenefitsCategoryDetails
        {
            get
            {
                return i_BenefitsCategoryDetails;
            }
            set
            {
                Debug.Assert( value != null );
                this.SetAndTrack<ArrayList>(ref this.i_BenefitsCategoryDetails, value, MethodBase.GetCurrentMethod() );
            }
        }

        public KindOfConstraint KindOfConstraint
        {
            get
            {
                return i_KindOfConstraint;
            }
            set
            {
                Debug.Assert( value != null );
                this.SetAndTrack<KindOfConstraint>(ref this.i_KindOfConstraint, value, MethodBase.GetCurrentMethod() );
            }
        }

        public TypeOfProduct TypeOfProduct
        {
            get
            {
                return i_TypeOfProduct;
            }
            set
            {
                Debug.Assert( value != null );
                this.SetAndTrack<TypeOfProduct>(ref this.i_TypeOfProduct, value, MethodBase.GetCurrentMethod() );
            }
        }


        #endregion
      
        #region Private Methods
      
        #endregion
      
        #region Private Properties
      
        #endregion
      
        #region Construction and Finalization
        
        public CommercialConstraints( )
        {
            this.ResetFieldsToDefault( true );
        }
      
        #endregion

        #region Data Elements

        private string i_EligibilityPhone;  
        private DateTime i_EffectiveDateForInsured;
        private string i_InsuranceCompanyRepName;
        private DateTime i_TerminationDateForInsured;
        private ArrayList i_BenefitsCategoryDetails;
        private KindOfConstraint i_KindOfConstraint;
        private TypeOfProduct i_TypeOfProduct;

        #endregion
        
        #region Constants

        #endregion
    }
}
