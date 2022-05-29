using Infragistics.Win.UltraWinGrid;
using PatientAccess.Annotations;

namespace PatientAccess.UI.CensusInquiries
{
    [UsedImplicitly]
	public class ADTCensusAllADTResults : ADTCensusResults
	{
        #region Methods
        
        public override void CustomizeGridLayout()
        {
            base.CustomizeGridLayout();
            UltraGridBand ADTBand  = ADTGrid.DisplayLayout.Bands[ADT_BAND];

            ADTBand.Columns[COL_CONFIDENTIAL].Hidden         = false;
            ADTBand.Columns[COL_ADT_TYPE].Hidden             = false;
            ADTBand.Columns[COL_TRANSACTION_TIME].Hidden     = false;
            ADTBand.Columns[COL_PATIENT_NAME_ACCOUNT].Hidden = false;
            ADTBand.Columns[COL_PATIENT_TYPE].Hidden         = false;
            ADTBand.Columns[COL_LOCATION_FROM].Hidden        = false;
            ADTBand.Columns[COL_LOCATION_TO].Hidden          = false;
            ADTBand.Columns[COL_PHYSICIANS].Hidden           = false;
        }

        #endregion

        #region Construction and Finalization

        public ADTCensusAllADTResults()
        {
        }

        #endregion
	}
}
