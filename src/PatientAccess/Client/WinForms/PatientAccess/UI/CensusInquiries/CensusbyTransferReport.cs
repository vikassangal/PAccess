using Infragistics.Win.UltraWinGrid;
using PatientAccess.Annotations;

namespace PatientAccess.UI.CensusInquiries
{
    [UsedImplicitly]
	public class CensusbyTransferReport : CensusbyADTReport
	{
        #region Events
        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        public override void CustomizeGridLayout()
        {
            base.CustomizeGridLayout();
            UltraGridBand patientBand  = PrintGrid.DisplayLayout.Bands[PATIENT_BAND];
            patientBand.Header.Caption = ADT_TRANSFER_CAPTION;

            UltraGridBand summaryHeaderBand  = PrintGrid.DisplayLayout.Bands[SUMMARY_HEADER_BAND];
            summaryHeaderBand.Header.Caption = ADT_TRANSFER_CAPTION;

            patientBand.Columns[COL_CONFIDENTIAL].Hidden             = false;
            patientBand.Columns[COL_TRANSACTION_TIME].Hidden         = false;
            patientBand.Columns[COL_PATIENT_NAME_ACCOUNT].Hidden     = false;
            patientBand.Columns[COL_PATIENT_TYPE].Hidden             = false;
            patientBand.Columns[COL_LOCATION_FROM_TO].Hidden         = false;
            patientBand.Columns[COL_PHYSICIANS].Hidden               = false;
            patientBand.Columns[COL_LOCATION_FROM_TO].Header.Caption = COL_LOCATION;
        }

        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public CensusbyTransferReport()
        {
        }

        #endregion

        #region Windows Form Designer generated code
        #endregion

        #region Data Elements
        #endregion

        #region Constants

        private const string ADT_TRANSFER_CAPTION = " Transfers";

        #endregion
	}
}
