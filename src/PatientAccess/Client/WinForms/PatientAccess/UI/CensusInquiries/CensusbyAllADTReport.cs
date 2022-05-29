using Infragistics.Win.UltraWinGrid;
using PatientAccess.Annotations;

namespace PatientAccess.UI.CensusInquiries
{
    [UsedImplicitly]
	public class CensusbyAllADTReport : CensusbyADTReport
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
            patientBand.Header.Caption = ADT_ALL_CAPTION;

            UltraGridBand summaryHeaderBand  = PrintGrid.DisplayLayout.Bands[SUMMARY_HEADER_BAND];
            summaryHeaderBand.Header.Caption = ADT_ALL_CAPTION;

            patientBand.Columns[COL_CONFIDENTIAL].Hidden              = false;
            patientBand.Columns[COL_ADT_TYPE].Hidden                  = false;
            patientBand.Columns[COL_TRANSACTION_TIME].Hidden          = false;
            patientBand.Columns[COL_PATIENT_NAME_ACCOUNT_TYPE].Hidden = false;
            patientBand.Columns[COL_LOCATION_FROM_TO].Hidden          = false;
            patientBand.Columns[COL_PHYSICIANS].Hidden                = false;
            patientBand.Columns[COL_LOCATION_FROM_TO].Header.Caption  = COL_LOCATION;
            patientBand.Columns[COL_PATIENT_NAME_ACCOUNT_TYPE].Header.Caption =
                COL_PATIENT_NAME_ACCOUNT;
        }

        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public CensusbyAllADTReport()
        {
        }

        #endregion

        #region Windows Form Designer generated code
        #endregion

        #region Data Elements
        #endregion

        #region Constants

        private const string ADT_ALL_CAPTION = " All A-D-T";

        #endregion
	}
}
