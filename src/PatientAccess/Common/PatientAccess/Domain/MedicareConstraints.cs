using System;
using System.Diagnostics;
using System.Reflection;

namespace PatientAccess.Domain
{
    [Serializable]
    public class MedicareConstraints : CoverageConstraints
	{
		
        #region Event Handlers

        #endregion
      
        #region Methods

        public override void ResetFieldsToDefault( bool resetTracking )
        {

            this.i_PartACoverage = new YesNoFlag( );
            this.i_PartACoverageEffectiveDate = DateTime.MinValue;
            this.i_PartBCoverage = new YesNoFlag( );
            this.i_PartBCoverageEffectiveDate = DateTime.MinValue;
            this.i_PatientHasMedicareHMOCoverage = new YesNoFlag( );
            this.i_RemainingCoInsurance = -1;
            this.i_RemainingBenefitPeriod = -1;
            this.i_RemainingLifeTimeReserve = -1;
            this.i_RemainingSNF = -1;
            this.i_PatientIsPartOfHospiceProgram = new YesNoFlag( );
            this.i_RemainingSNFCoInsurance = -1;
            this.i_DateOfLastBillingActivity = DateTime.MinValue;
            this.i_RemainingPartADeductible = -1;
            this.i_RemainingPartBDeductible = -1;

            if( resetTracking )
                this.ResetChangeTracking();

        }

        #endregion
      
        #region Properties

        public YesNoFlag PartACoverage
        {
            get
            {
                return i_PartACoverage;
            }
            set
            {
                Debug.Assert( value != null );
                this.SetAndTrack<YesNoFlag>( ref this.i_PartACoverage, value, MethodBase.GetCurrentMethod() );
            }
        }

        public DateTime PartACoverageEffectiveDate
        {
            get
            {
                return i_PartACoverageEffectiveDate;
            }
            set
            {
                this.SetAndTrack<DateTime>( ref this.i_PartACoverageEffectiveDate, value, MethodBase.GetCurrentMethod() );
            }
        }

        public YesNoFlag PartBCoverage
        {
            get
            {
                return i_PartBCoverage;
            }
            set
            {
                Debug.Assert( value != null );
                this.SetAndTrack<YesNoFlag>( ref this.i_PartBCoverage, value, MethodBase.GetCurrentMethod() );
            }
        }

        public DateTime PartBCoverageEffectiveDate
        {
            get
            {
                return i_PartBCoverageEffectiveDate;
            }
            set
            {
                this.SetAndTrack<DateTime>( ref this.i_PartBCoverageEffectiveDate, value, MethodBase.GetCurrentMethod() );
            }
        }

        public YesNoFlag PatientHasMedicareHMOCoverage
        {
            get
            {
                return i_PatientHasMedicareHMOCoverage;
            }
            set
            {
                Debug.Assert( value != null );
                this.SetAndTrack<YesNoFlag>( ref this.i_PatientHasMedicareHMOCoverage, value, MethodBase.GetCurrentMethod() );
            }
        }

        public int RemainingCoInsurance
        {
            get
            {
                return i_RemainingCoInsurance;
            }
            set
            {
                this.SetAndTrack<int>( ref this.i_RemainingCoInsurance, value, MethodBase.GetCurrentMethod() );
            }
        }

        public int RemainingBenefitPeriod
        {
            get
            {
                return i_RemainingBenefitPeriod;
            }
            set
            {
                this.SetAndTrack<int>( ref this.i_RemainingBenefitPeriod, value, MethodBase.GetCurrentMethod() );
            }
        }

        public int RemainingLifeTimeReserve
        {
            get
            {
                return i_RemainingLifeTimeReserve;
            }
            set
            {
                this.SetAndTrack<int>( ref this.i_RemainingLifeTimeReserve, value, MethodBase.GetCurrentMethod() );
            }
        }

        public int RemainingSNF
        {
            get
            {
                return i_RemainingSNF;
            }
            set
            {
                this.SetAndTrack<int>( ref this.i_RemainingSNF, value, MethodBase.GetCurrentMethod() );
            }
        }

        public YesNoFlag PatientIsPartOfHospiceProgram
        {
            get
            {
                return i_PatientIsPartOfHospiceProgram;
            }
            set
            {
                Debug.Assert( value != null );
                this.SetAndTrack<YesNoFlag>( ref this.i_PatientIsPartOfHospiceProgram, value, MethodBase.GetCurrentMethod() );
            }
        }

        public int RemainingSNFCoInsurance
        {
            get
            {
                return this.i_RemainingSNFCoInsurance;
            }
            set
            {
                this.SetAndTrack<int>( ref this.i_RemainingSNFCoInsurance, value, MethodBase.GetCurrentMethod() );
            }
        }

        public DateTime DateOfLastBillingActivity
        {
            get
            {
                return i_DateOfLastBillingActivity;
            }
            set
            {
                this.SetAndTrack<DateTime>( ref this.i_DateOfLastBillingActivity, value, MethodBase.GetCurrentMethod() );
            }
        }

        public float RemainingPartADeductible
        {
            get
            {
                return i_RemainingPartADeductible;
            }
            set
            {
                this.SetAndTrack<float>( ref this.i_RemainingPartADeductible, value, MethodBase.GetCurrentMethod() );
            }
        }

        public float RemainingPartBDeductible
        {
            get
            {
                return i_RemainingPartBDeductible;
            }
            set
            {
                this.SetAndTrack<float>( ref this.i_RemainingPartBDeductible, value, MethodBase.GetCurrentMethod() );
            }
        }

        #endregion
      
        #region Private Methods
      
        #endregion
      
        #region Private Properties
      
        #endregion
      
        #region Construction and Finalization

        public MedicareConstraints( )
        {
            this.ResetFieldsToDefault( true );
        }

        #endregion
      
        #region Field Elements

        private YesNoFlag   i_PartACoverage = new YesNoFlag( );
        private DateTime    i_PartACoverageEffectiveDate;
        private YesNoFlag   i_PartBCoverage = new YesNoFlag( );
        private DateTime    i_PartBCoverageEffectiveDate;
        private YesNoFlag   i_PatientHasMedicareHMOCoverage = new YesNoFlag( );
        private int         i_RemainingCoInsurance = -1;
        private int         i_RemainingBenefitPeriod = -1;
        private int         i_RemainingLifeTimeReserve = -1;
        private int         i_RemainingSNF = -1;
        private YesNoFlag   i_PatientIsPartOfHospiceProgram = new YesNoFlag( );
        private int         i_RemainingSNFCoInsurance = -1;
        private DateTime    i_DateOfLastBillingActivity;
        private float       i_RemainingPartADeductible;
        private float       i_RemainingPartBDeductible;
        #endregion
        
        #region Constants
      
        #endregion
	}    
}
