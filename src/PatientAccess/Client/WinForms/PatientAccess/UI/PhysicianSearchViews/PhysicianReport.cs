using System;
using System.Drawing;
using Infragistics.Win;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;
using PatientAccess.Domain.Parties;
using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI.PhysicianSearchViews
{
	/// <summary>
	/// Summary description for PhysicianReport.
	/// </summary>
	public class PhysicianReport : PrintReport
    {
        #region Events
        #endregion

        #region Event Handler
        #endregion

        #region Construction And Finalization

        public PhysicianReport()
        {
            CreateDataStructure();
        }

        #endregion

        #region Public Methods
        
        public void PrintPreview()
        {
            FillDataSource();
            base.DataSource = dataSource;
            base.UpdateView();
            CustomizeGridLayout();
            base.HeaderText = HEADER_TEXT;
            //           base.CenterHeaderText = HEADER_TEXT;
            base.GeneratePrintPreview();
        }

        #endregion

        #region Private Methods
        // This Report is upposed to be implemented using Card View.But as for the moment 
        // the R&D is going on it, we have used the traditional approach of Rows and Coloumn

        private void CreateDataStructure()
        {
            dataSource = new UltraDataSource();
            this.dataSource.Rows.Band.Key = DATA_BAND;
            this.dataSource.Band.Columns.Add( COLOUMN1 );
            this.dataSource.Band.Columns.Add( COLOUMN2 );
            this.dataSource.Band.Columns.Add( COLOUMN3 );
            this.dataSource.Band.Columns.Add( COLOUMN4 );
        }

        private void CustomizeGridLayout()
        {
            dataBand = PrintGrid.DisplayLayout.Bands[DATA_BAND];
            dataBand.ColHeadersVisible = false;
            dataBand.Indentation = 25;
            dataBand.Columns[DATABAND_COLOUMN1].CellMultiLine = DefaultableBoolean.True;
            dataBand.Columns[DATABAND_COLOUMN2].CellMultiLine = DefaultableBoolean.True;
            dataBand.Columns[DATABAND_COLOUMN3].CellMultiLine = DefaultableBoolean.True;
            dataBand.Columns[DATABAND_COLOUMN4].CellMultiLine = DefaultableBoolean.True;
            dataBand.Columns[DATABAND_COLOUMN1].Width = 80;
            dataBand.Columns[DATABAND_COLOUMN2].Width = 400;
            dataBand.Columns[DATABAND_COLOUMN3].Width = 175;
            dataBand.Columns[DATABAND_COLOUMN4].Width = 225;
            dataBand.Override.BorderStyleCell = UIElementBorderStyle.None;
            dataBand.Override.BorderStyleRow = UIElementBorderStyle.None;
            dataBand.Override.RowAppearance.BorderColor = SystemColors.Window;
            dataBand.Override.RowAppearance.BorderAlpha = Alpha.Transparent;
            PrintGrid.Refresh();
        }
        
        private void FillDataSource()
        {
            dataSource.Rows.Clear();
            physicianDetail = this.Model;
            object[] physicianDetailList = new object[20]; 

            dataSource.Rows.Add( new string[] { "","","","" } );
            dataSource.Rows.Add( new string[] { "","","","" } );             
            dataSource.Rows.Add( new string[] { "Name:   " , 
                                                  physicianDetail.FullName,"","" } );
            dataSource.Rows.Add( new string[] { "Title:   " , physicianDetail.Title,
                                                  "","" } );
            dataSource.Rows.Add( new string[] { "","","","" } );
            dataSource.Rows.Add( new string[] { "Address:   " , physicianDetail.Address
                                                  .Address1 + physicianDetail.Address.Address2,
                                                  "Number:  " , 
                                                  physicianDetail.PhysicianNumber.ToString() } );
            dataSource.Rows.Add( new string[] { "",physicianDetail.Address.City + "," +
                                                  physicianDetail.Address.State
                                                  + " " + physicianDetail.Address.ZipCode.PostalCode,
                                                  "Federal license number:   " , 
                                                  physicianDetail.FederalLicense.Number } );
            //			dataSource.Rows.Add( new string[] { "" , "" , "State license number:   "
            //												  , physicianDetail.StateLicense.Number } );
            dataSource.Rows.Add( new string[] { "" , "" , "State license number:   "
                                                  , physicianDetail.StateLicense } );
            dataSource.Rows.Add( new string[] { "Phone:   ",physicianDetail.PhoneNumber
                                                  .AsFormattedString(),
                                                  "Medical group number:   ",physicianDetail
                                                  .MedicalGroupNumber } );
            dataSource.Rows.Add( new string[] { "Cell:   ",physicianDetail.CellPhoneNumber
                                                  .AsFormattedString(),
                                                  "UPIN:   ",physicianDetail.UPIN } );
            dataSource.Rows.Add( new string[] { "Pager:   ",physicianDetail.PagerNumber
                                                  .AsFormattedString() 
                                                  + " " + "\t\t PIN:" +"\t\t "+ physicianDetail.PIN,
                                                  "NPI:   ",physicianDetail.NPI } );
            dataSource.Rows.Add( new string[] { "", "", "", "" } );

            dataSource.Rows.Add( new string[] { "Specialty:   ",physicianDetail.Specialization.AsFormattedSpeciality,
                                                  "", "" } );
            dataSource.Rows.Add( new string[] { "","","Status:   ",physicianDetail.Status } );

            dataSource.Rows.Add( new string[] { "", "",
                                                  "Active/Inactive:   ",
                                                  physicianDetail.ActiveInactiveFlag } );
            

            if( ! ( physicianDetail.DateActivated.Equals( DateTime.MinValue ) ) )
            {
                dateActivated = ( physicianDetail.DateActivated ).ToString( "MM/dd/yyyy" );
            }
            dataSource.Rows.Add( new string[] { "","","Date activated:   ",this.dateActivated } );
            if( ! ( physicianDetail.DateInactivated.Equals( DateTime.MinValue ) ) )
            {
                dateInActivated = ( physicianDetail.DateInactivated ).ToString( "MM/dd/yyyy" );
            }
            dataSource.Rows.Add( new string[] { "","","Date inactivated:   ",this.dateInActivated } );
            dataSource.Rows.Add( new string[] { "","","Adm Privileges:   "
                                                  ,physicianDetail.AdmittingPrivileges } );
            dataSource.Rows.Add( new string[] { "","","Excluded from Adm and Att:   "
                                                  ,physicianDetail.ExcludedStatus } );
            if( ! ( physicianDetail.DateExcluded.Equals( DateTime.MinValue ) ) )
            {
                dateExcluded = ( physicianDetail.DateExcluded ).ToString( "MM/dd/yyyy" );
            }
            dataSource.Rows.Add( new string[] { "","","Date Excluded:   ",this.dateExcluded } );
        }

        #endregion

        #region Property

        public new Physician Model
        {
            private get
            {
                return base.Model as Physician;
            }
            set
            {
                base.Model = value;
            }
        }

        #endregion

        #region Constants

        private const string HEADER_TEXT = "Physician Detail",
            DATA_BAND = "Data_Band",
            COLOUMN1 = "Coloumn1",
            COLOUMN2 = "Coloumn2",
            COLOUMN3 = "Coloumn3",
            COLOUMN4 = "Coloumn4";
        private const int DATABAND_COLOUMN1 = 0,
            DATABAND_COLOUMN2 = 1,
            DATABAND_COLOUMN3 = 2,
            DATABAND_COLOUMN4 = 3;  
        #endregion

        #region Data Elements
        
        private UltraDataSource dataSource;
        private Physician physicianDetail;
        private UltraGridBand dataBand;
        private string dateActivated;
        private string dateInActivated;
        private string dateExcluded;

        #endregion
    }
}
