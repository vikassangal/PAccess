using System;
using System.Diagnostics;
using System.Reflection;

namespace PatientAccess.Domain
{
    [Serializable]
    public class MedicaidConstraints : CoverageConstraints
    {
        
        #region Event Handlers
        
        #endregion

        #region Methods

        public override void ResetFieldsToDefault( bool resetTracking )
        {
            this.i_EligibilityDate = DateTime.MinValue;
            this.i_MedicaidCopay = -1;
            this.i_EVCNumber = String.Empty;

            if( resetTracking )
                this.ResetChangeTracking();
        }

        #endregion

        #region Properties

        public DateTime EligibilityDate
        {
            get
            {
                return i_EligibilityDate;
            }
            set
            {
                this.SetAndTrack<DateTime>( ref this.i_EligibilityDate, value, MethodBase.GetCurrentMethod() );
            }
        }

        public float MedicaidCopay
        {
            get
            {
                return i_MedicaidCopay;
            }
            set
            {
                this.SetAndTrack<float>( ref this.i_MedicaidCopay, value, MethodBase.GetCurrentMethod() );
            }
        }

        public string EVCNumber
        {
            get
            {
                return i_EVCNumber;
            }
            set
            {
                Debug.Assert( value != null );
                this.SetAndTrack<string>( ref this.i_EVCNumber, value, MethodBase.GetCurrentMethod() );
            }
        }


        #endregion

        #region Private Methods

        #endregion

        #region Private Properties

        #endregion

        #region Construction and Finalization

        public MedicaidConstraints( )
        {
            this.ResetFieldsToDefault( true );
        }

        #endregion

        #region Field Elements

        private DateTime    i_EligibilityDate;
        private float       i_MedicaidCopay;
        private string      i_EVCNumber;

        #endregion

        #region Constants

        #endregion
    }
}
