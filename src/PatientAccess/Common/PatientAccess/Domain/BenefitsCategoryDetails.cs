using System;
using System.Diagnostics;
using System.Reflection;
using Extensions.PersistenceCommon;

namespace PatientAccess.Domain

{
    //TODO: Create XML summary comment for BenefitsCategoryDetails
    [Serializable]
    public class BenefitsCategoryDetails : PersistentModel, ICloneable
    {
        private const string PROPERTY_DEDUCTIBLE = "Deductible";
        private const string PROPERTY_COPAY = "CoPay";
        private const string PROPERTY_TIMEPERIOD = "TimePeriod";
        private const string PROPERTY_DEDUCTIBLEMET = "DeductibleMet";
        private const string PROPERTY_DEDUCTIBLEDOLLARSMET = "DeductibleDollarsMet";
        private const string PROPERTY_COINSURANCE = "CoInsurance";
        private const string PROPERTY_OUTOFPOCKET = "OutOfPocket";
        private const string PROPERTY_OUTOFPOCKETMET = "OutOfPocketMet";
        private const string PROPERTY_OUTOFPOCKETDOLLARSMET = "OutOfPocketDollarsMet";
        private const string PROPERTY_AFTEROUTOFPOCKETPERCENT = "AfterOutOfPocketPercent";
        private const string PROPERTY_WAIVECOPAYIFADMITTED = "WaiveCopayIfAdmitted";
        private const string PROPERTY_VISITSPERYEAR = "VisitsPerYear";
        private const string PROPERTY_LIFETIMEMAXBENEFIT = "LifeTimeMaxBenefit";
        private const string PROPERTY_MAXBENEFITPERVISIT = "MaxBenefitPerVisit";
        private const string PROPERTY_REMAININGLIFETIMEVALUE = "RemainingLifetimeValue";
        private const string PROPERTY_REMAININGLIFETIMEVALUEMET = "RemainingLifetimeValueMet";
        private const string PROPERTY_REMAININGBENEFITPERVISITS = "RemainingBenefitPerVisits";
        private const string PROPERTY_REMAININGBENEFITPERVISITSMET = "RemainingBenefitPerVisitsMet";

        #region Event Handlers
        #endregion

        #region Methods

        private void ResetFieldsToDefault( bool resetTracking )
        {

            this.i_Deductible = -1;
            this.i_CoPay = -1;
            this.i_TimePeriod = new TimePeriodFlag();
            this.i_TimePeriod.SetBlank();
            this.i_DeductibleMet = new YesNoFlag();
            this.i_DeductibleDollarsMet = -1;
            this.i_CoInsurance = -1;
            this.i_OutOfPocket = -1;
            this.i_OutOfPocketMet = new YesNoFlag();
            this.i_OutOfPocketDollarsMet = -1;
            this.i_AfterOutOfPocketPercent = -1;
            this.i_WaiveCopayIfAdmitted = new YesNoFlag();
            this.i_VisitsPerYear = -1;
            this.i_LifeTimeMaxBenefit = -1;
            this.i_MaxBenefitPerVisit = -1;
            this.i_RemainingLifetimeValue = -1;
            this.i_RemainingLifetimeValueMet = new YesNoFlag();
            this.i_RemainingBenefitPerVisits = -1;
            this.i_RemainingBenefitPerVisitsMet = new YesNoFlag();
            this.i_BenefitCategory = new BenefitsCategory();

            if( resetTracking )
                this.ResetChangeTracking();

        }

        #endregion

        #region Properties

        public virtual bool HaveVerificationDetailsChanged
        {

            get
            {

                return this.HasChangedFor( PROPERTY_DEDUCTIBLE ) ||
                       this.HasChangedFor( PROPERTY_COPAY ) ||
                       this.HasChangedFor( PROPERTY_TIMEPERIOD ) ||
                       this.HasChangedFor( PROPERTY_DEDUCTIBLEMET ) ||
                       this.HasChangedFor( PROPERTY_DEDUCTIBLEDOLLARSMET ) ||
                       this.HasChangedFor( PROPERTY_COINSURANCE ) ||
                       this.HasChangedFor( PROPERTY_OUTOFPOCKET ) ||
                       this.HasChangedFor( PROPERTY_OUTOFPOCKETMET ) ||
                       this.HasChangedFor( PROPERTY_OUTOFPOCKETDOLLARSMET ) ||
                       this.HasChangedFor( PROPERTY_AFTEROUTOFPOCKETPERCENT ) ||
                       this.HasChangedFor( PROPERTY_WAIVECOPAYIFADMITTED ) ||
                       this.HasChangedFor( PROPERTY_VISITSPERYEAR ) ||
                       this.HasChangedFor( PROPERTY_LIFETIMEMAXBENEFIT ) ||
                       this.HasChangedFor( PROPERTY_MAXBENEFITPERVISIT ) ||
                       this.HasChangedFor( PROPERTY_REMAININGLIFETIMEVALUE ) ||
                       this.HasChangedFor( PROPERTY_REMAININGLIFETIMEVALUEMET ) ||
                       this.HasChangedFor( PROPERTY_REMAININGBENEFITPERVISITS ) ||
                       this.HasChangedFor( PROPERTY_REMAININGBENEFITPERVISITSMET );

            }

        }

        public float Deductible
        {
            get
            {
                return i_Deductible;
            }
            set
            {

                this.SetAndTrack<float>( ref this.i_Deductible, value, MethodBase.GetCurrentMethod() );

            }
        }

        public float CoPay
        {
            get
            {
                return i_CoPay;
            }
            set
            {
                this.SetAndTrack<float>( ref this.i_CoPay, value, MethodBase.GetCurrentMethod() );
            }
        }

        public TimePeriodFlag TimePeriod
        {
            get
            {
                return i_TimePeriod;
            }
            set
            {
                Debug.Assert( value != null );
                this.SetAndTrack<TimePeriodFlag>( ref this.i_TimePeriod, value, MethodBase.GetCurrentMethod() );
            }
        }

        public YesNoFlag DeductibleMet
        {
            get
            {
                return i_DeductibleMet;
            }
            set
            {
                
                Debug.Assert( value != null );
                this.SetAndTrack<YesNoFlag>( ref this.i_DeductibleMet, value, MethodBase.GetCurrentMethod() );

            }
        }
        public float DeductibleDollarsMet
        {
            get
            {
                return i_DeductibleDollarsMet;
            }
            set
            {
                this.SetAndTrack<float>( ref this.i_DeductibleDollarsMet, value, MethodBase.GetCurrentMethod() );
            }
        }

        public int CoInsurance
        {
            get
            {
                return i_CoInsurance;
            }
            set
            {
                this.SetAndTrack<int>( ref this.i_CoInsurance, value, MethodBase.GetCurrentMethod() );
            }
        }

        public float OutOfPocket
        {
            get
            {
                return i_OutOfPocket;
            }
            set
            {
                this.SetAndTrack<float>( ref this.i_OutOfPocket, value, MethodBase.GetCurrentMethod() );
            }
        }

        public YesNoFlag OutOfPocketMet
        {
            get
            {
                return i_OutOfPocketMet;
            }
            set
            {
                Debug.Assert( value != null );
                this.SetAndTrack<YesNoFlag>( ref this.i_OutOfPocketMet, value, MethodBase.GetCurrentMethod() );
            }
        }

        public float OutOfPocketDollarsMet
        {
            get
            {
                return i_OutOfPocketDollarsMet;
            }
            set
            {
                this.SetAndTrack<float>( ref this.i_OutOfPocketDollarsMet, value, MethodBase.GetCurrentMethod() );
            }
        }
        public int AfterOutOfPocketPercent
        {
            get
            {
                return i_AfterOutOfPocketPercent;
            }
            set
            {
                this.SetAndTrack<int>( ref this.i_AfterOutOfPocketPercent, value, MethodBase.GetCurrentMethod() );
            }
        }
        public YesNoFlag WaiveCopayIfAdmitted
        {
            get
            {
                return i_WaiveCopayIfAdmitted;
            }
            set
            {
                Debug.Assert( value != null );
                this.SetAndTrack<YesNoFlag>( ref this.i_WaiveCopayIfAdmitted, value, MethodBase.GetCurrentMethod() );
            }
        }
        public int VisitsPerYear
        {
            get
            {
                return i_VisitsPerYear;
            }
            set
            {
                this.SetAndTrack<int>( ref this.i_VisitsPerYear, value, MethodBase.GetCurrentMethod() );
            }
        }

        public double LifeTimeMaxBenefit
        {
            get
            {
                return i_LifeTimeMaxBenefit;
            }
            set
            {
                this.SetAndTrack<double>( ref this.i_LifeTimeMaxBenefit, value, MethodBase.GetCurrentMethod() );
            }
        }

        public double MaxBenefitPerVisit
        {
            get
            {
                return i_MaxBenefitPerVisit;
            }
            set
            {
                this.SetAndTrack<double>( ref this.i_MaxBenefitPerVisit, value, MethodBase.GetCurrentMethod() );
            }
        }

        public double RemainingLifetimeValue
        {
            get
            {
                return i_RemainingLifetimeValue;
            }
            set
            {
                this.SetAndTrack<double>( ref this.i_RemainingLifetimeValue, value, MethodBase.GetCurrentMethod() );
            }
        }
        public YesNoFlag RemainingLifetimeValueMet
        {
            get
            {
                return i_RemainingLifetimeValueMet;
            }
            set
            {
                Debug.Assert( value != null );
                this.SetAndTrack<YesNoFlag>( ref this.i_RemainingLifetimeValueMet, value, MethodBase.GetCurrentMethod() );
            }
        }
        public double RemainingBenefitPerVisits
        {
            get
            {
                return i_RemainingBenefitPerVisits;
            }
            set
            {
                this.SetAndTrack<double>( ref this.i_RemainingBenefitPerVisits, value, MethodBase.GetCurrentMethod() );
            }
        }

        public YesNoFlag RemainingBenefitPerVisitsMet
        {
            get
            {
                return i_RemainingBenefitPerVisitsMet;
            }
            set
            {
                Debug.Assert( value != null );
                this.SetAndTrack<YesNoFlag>( ref this.i_RemainingBenefitPerVisitsMet, value, MethodBase.GetCurrentMethod() );
            }
        }

        public BenefitsCategory BenefitCategory
        {
            get
            {
                return i_BenefitCategory;
            }
            set
            {
                i_BenefitCategory = value;
            }
        }

        
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public BenefitsCategoryDetails()
        {
            this.ResetFieldsToDefault( true );
        }
        #endregion

        #region Data Elements

        private float i_Deductible;
        private float i_CoPay;
        private TimePeriodFlag i_TimePeriod;
        private YesNoFlag i_DeductibleMet;
        private float i_DeductibleDollarsMet;
        private int i_CoInsurance;
        private float i_OutOfPocket;
        private YesNoFlag i_OutOfPocketMet;
        private float i_OutOfPocketDollarsMet;
        private int i_AfterOutOfPocketPercent;
        private YesNoFlag i_WaiveCopayIfAdmitted;
        private int i_VisitsPerYear;
        private double i_LifeTimeMaxBenefit;
        private double i_MaxBenefitPerVisit;
        private double i_RemainingLifetimeValue;
        private YesNoFlag i_RemainingLifetimeValueMet;
        private double i_RemainingBenefitPerVisits;
        private YesNoFlag i_RemainingBenefitPerVisitsMet;
        private BenefitsCategory i_BenefitCategory;

        #endregion

        #region Constants
        #endregion
    }
}
