using Infragistics.Win.UltraWinGrid;
using PatientAccess.Annotations;

namespace PatientAccess.UI.CensusInquiries
{
	[UsedImplicitly]
	public class ADTCensusAdmissionResults : ADTCensusResults
	{
        #region Methods

        public override void CustomizeGridLayout()
        {
            base.CustomizeGridLayout();
            UltraGridBand ADTBand  = ADTGrid.DisplayLayout.Bands[ADT_BAND];

            ADTBand.Columns[COL_CONFIDENTIAL].Hidden         = false;
            ADTBand.Columns[COL_TRANSACTION_TIME].Hidden     = false;
            ADTBand.Columns[COL_PATIENT_NAME_ACCOUNT].Hidden = false;
            ADTBand.Columns[COL_LOCATION].Hidden             = false;
            ADTBand.Columns[COL_PHYSICIANS].Hidden           = false;
            ADTBand.Columns[COL_CHIEF_COMPLAINT].Hidden      = false;
        }

        #endregion

        #region Construction and Finalization

        public ADTCensusAdmissionResults()
        {
        }

        #endregion
	}
}
