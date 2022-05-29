using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using PatientAccess.Annotations;

namespace PatientAccess.Domain
{
    [Serializable]
    [UsedImplicitly]
    public abstract class CoverageForCommercialOther : CoverageGroup
    {

        #region Event Handlers
        #endregion

        #region Methods

        public override void RemoveCoverageVerificationData()
        {

            base.RemoveCoverageVerificationData();
            
            this.RemoveAllBenefitsCategories();
            
            this.EligibilityPhone = string.Empty;
            this.ForceUnChangedStatusFor( PROPERTY_ELIGIBILITY_PHONE );
            this.InsuranceCompanyRepName = string.Empty;
            this.ForceUnChangedStatusFor( PROPERTY_INSURANCE_COMPANY_REPNAME );
            this.EffectiveDateForInsured = DateTime.MinValue;
            this.ForceUnChangedStatusFor( PROPERTY_EFFECTIVE_DATE_FORINSURED );
            this.TerminationDateForInsured = DateTime.MinValue;
            this.ForceUnChangedStatusFor( PROPERTY_TERMINATION_DATE_FOR_INSURED );

        }

        public void RemoveCoveragePayorData( CoverageForCommercialOther coverage )
        {
            this.EffectiveDateForInsured                = DateTime.MinValue;
            this.EligibilityPhone                       = string.Empty;
            this.InsuranceCompanyRepName                = string.Empty;
            this.PreCertNumber                          = 0;
            this.TerminationDateForInsured              = DateTime.MinValue;
            this.TrackingNumber                         = string.Empty;
            this.RemoveAuthorization();
        }

        public override void RemoveAuthorization()
        {
            base.RemoveAuthorization();

            this.PermittedDays                          = 0;
            this.Authorization.AuthorizationNumber      = string.Empty;
            this.Authorization.NumberOfDaysAuthorized   = 0;
            this.TrackingNumber = String.Empty;
            this.ForceUnChangedStatusFor( "TrackingNumber" );

        }
        public void AddBenefitsCategory( BenefitsCategory aBenefitsCategory, BenefitsCategoryDetails aBenefitsCategoryDetails )
        {
            if( (aBenefitsCategory != null) && (aBenefitsCategoryDetails != null) )
            {
                if( i_BenefitsCategories.Contains( aBenefitsCategory ) == false )
                {
                    i_BenefitsCategories.Add( aBenefitsCategory, aBenefitsCategoryDetails );
                }
                else
                {
                    this.i_BenefitsCategories.Remove( aBenefitsCategory );
                    i_BenefitsCategories.Add( aBenefitsCategory, aBenefitsCategoryDetails );
                }
            }
        }

        private void RemoveAllBenefitsCategories()
        {
            this.i_BenefitsCategories.Clear();
        }

        public void RemoveBenefitsCategory(  BenefitsCategory aBenefitsCategory )
        {
            this.i_BenefitsCategories.Remove( aBenefitsCategory );
        }

        public BenefitsCategoryDetails BenefitsCategoryDetailsWith( long BenefitsCategoryOID )
        {
            BenefitsCategory bc = new BenefitsCategory();
            bc.Oid = BenefitsCategoryOID;

            return this.i_BenefitsCategories[bc] as BenefitsCategoryDetails;
        }

        public BenefitsCategoryDetails BenefitsCategoryDetailsWith( string benefitsCategoryDescription )
        {
            BenefitsCategory bc = new BenefitsCategory();
            bc.Description = benefitsCategoryDescription;

            return this.i_BenefitsCategories[bc] as BenefitsCategoryDetails;
        }

        public BenefitsCategoryDetails BenefitsCategoryDetailsFor( BenefitsCategory BenefitsCategory )
        {
            if( i_BenefitsCategories == null )
            {
                this.i_BenefitsCategories = new Hashtable();
            }

            BenefitsCategoryDetails bcd =  this.i_BenefitsCategories[BenefitsCategory] as BenefitsCategoryDetails;

            if( bcd == null )
            {
                bcd = new BenefitsCategoryDetails();
                this.i_BenefitsCategories.Add( BenefitsCategory, bcd );
            }

            return bcd;
        }
        #endregion

        #region Properties

        protected override bool HaveVerificationFieldsChanged
        {

            get
            {

                bool benefitsCategoryDetailsChanged = false;
                if( this.BenefitsCategories.Count > 0 )
                {

                    foreach( BenefitsCategoryDetails benefitsCategoryDetails in this.BenefitsCategories.Values )
                    {

                        if( benefitsCategoryDetails != null )
                        {

                            if( benefitsCategoryDetails.HaveVerificationDetailsChanged )
                            {

                                benefitsCategoryDetailsChanged = true;
                                break;

                            } //if

                        } //if

                    } //foreach

                }//if

                return base.HaveVerificationFieldsChanged ||
                       benefitsCategoryDetailsChanged || 
                       this.HasChangedFor( PROPERTY_ELIGIBILITY_PHONE ) || 
                       this.HasChangedFor( PROPERTY_INSURANCE_COMPANY_REPNAME ) ||
                       this.HasChangedFor( PROPERTY_EFFECTIVE_DATE_FORINSURED ) ||
                       this.HasChangedFor( PROPERTY_TERMINATION_DATE_FOR_INSURED );

            }
        }

        public string TrackingNumber
        {
            get
            {
                return i_TrackingNumber;
            }
            set
            {
                Debug.Assert( value != null );
                if( !string.IsNullOrEmpty( value ) ) value = value.Trim();
                this.SetAndTrack<string>( ref this.i_TrackingNumber, value, MethodBase.GetCurrentMethod() );
            }
        }

        public long PreCertNumber
        {
            get
            {
                return i_PreCertNumber;
            }
            set
            {
                i_PreCertNumber = value;
            }
        }

        public string  CertSSNID
        {
            get
            {
                return i_CertSSNID;
            }
            set
            {
                i_CertSSNID = value;
            }
        }
        
        public string  SignedOverMedicareHICNumber
        {
            get
            {
                return i_SignedOverMedicareHICNumber;
            }
            set
            {
                i_SignedOverMedicareHICNumber = value;
            }
        }
        public string MBINumber
        {
            get
            {
                return i_MBINumber;
            }
            set
            {
                i_MBINumber = value;
            }
        }
        public string  GroupNumber
        {
            get
            {
                return i_GroupNumber;
            }
            set
            {
                i_GroupNumber = value;
            }
        }

        public virtual string  EligibilityPhone
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

        public virtual string  InsuranceCompanyRepName
        {
            get
            {
                return i_InsuranceCompanyRepName;
            }
            set
            {
                Debug.Assert( value != null );
                this.SetAndTrack<string>( ref this.i_InsuranceCompanyRepName, value, MethodBase.GetCurrentMethod() );
            }
        }

        public virtual DateTime  EffectiveDateForInsured
        {
            get
            {
                return i_EffectiveDateForInsured;
            }
            set
            {
                this.SetAndTrack<DateTime>( ref this.i_EffectiveDateForInsured, value, MethodBase.GetCurrentMethod() );
            }
        }

        public virtual DateTime  TerminationDateForInsured
        {
            get
            {
                return i_TerminationDateForInsured;
            }
            set
            {
                this.SetAndTrack<DateTime>( ref this.i_TerminationDateForInsured, value, MethodBase.GetCurrentMethod() );
            }
        }

        public Hashtable BenefitsCategories
        {
            get
            {
                if( this.i_BenefitsCategories != null )
                {
                    return ( Hashtable )this.i_BenefitsCategories.Clone();
                }
                else
                {
                    return new Hashtable();
                }                
            }
            set
            {
                this.i_BenefitsCategories = value;
            }
        }

        public string AidCodeType
        {
            get
            {
                return i_AidCodeType;
            }
            set
            {
                this.SetAndTrack(ref this.i_AidCodeType, value, MethodBase.GetCurrentMethod());
            }
        }
        public string AidCode { get; set; }
        public bool? IsInsurancePlanPartOfIPA{get; set;}
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public CoverageForCommercialOther() : base()
        {
        }
        #endregion

        #region Data Elements
        private string  i_TrackingNumber = String.Empty;
        private long    i_PreCertNumber;
        private string  i_CertSSNID = String.Empty;
        private string  i_GroupNumber = String.Empty;
        private Hashtable i_BenefitsCategories = new Hashtable();
        private string  i_EligibilityPhone = String.Empty;
        private string  i_InsuranceCompanyRepName = String.Empty;
        private DateTime i_EffectiveDateForInsured = new DateTime(); 
        private DateTime i_TerminationDateForInsured = new DateTime();
        private string  i_SignedOverMedicareHICNumber = String.Empty;
        private string i_MBINumber = String.Empty;
        private string i_AidCodeType = String.Empty;
        #endregion

        #region Constants
        private const string PROPERTY_TRACKING_NUMBER = "TrackingNumber" ;
        private const string PROPERTY_ELIGIBILITY_PHONE = "EligibilityPhone";
        private const string PROPERTY_INFORMATIONRECEIVEDSOURCE = "InformationReceivedSource";
        private const string PROPERTY_INSURANCE_COMPANY_REPNAME = "InsuranceCompanyRepName";
        private const string PROPERTY_EFFECTIVE_DATE_FORINSURED = "EffectiveDateForInsured";
        private const string PROPERTY_TERMINATION_DATE_FOR_INSURED = "TerminationDateForInsured";

        #endregion
    }
}

