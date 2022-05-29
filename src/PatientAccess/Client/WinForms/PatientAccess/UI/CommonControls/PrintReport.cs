using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using Infragistics.Win;
using Infragistics.Win.Printing;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;
using PatientAccess.Domain;
using Appearance = Infragistics.Win.Appearance;

namespace PatientAccess.UI.CommonControls
{
    /// <summary>
    /// Summary description for PrintReport.
    /// </summary>
    public abstract class PrintReport : ControlView, IUIElementDrawFilter
    {
        #region Event Handlers
        
        private void PrintDocumentPagePrintingEvent(object sender, PagePrintingEventArgs e)
        {
            e.Document.Header.TextLeft = this.HeaderText;
            e.Document.Footer.TextLeft = this.FooterText;
            e.Document.DocumentName = this.HeaderText + " Report";
        }

        private void PrintDocumentPagePrintedEvent(object sender, PagePrintedEventArgs e)
        {                
            int imageYPosition = e.PageSettings.Margins.Top + 27;
            int imageXPosition = e.PageSettings.Margins.Left + 8;

            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream imageStream = assembly.GetManifestResourceStream(
                "PatientAccess.Images.patient_access_logo.gif");
            Bitmap logo = new Bitmap( imageStream );

            Color backColor = logo.GetPixel( 2, 2 );
            logo.MakeTransparent( backColor );
            e.Graphics.DrawImage( 
                logo, imageXPosition, 
                imageYPosition, logo.Width, logo.Height 
                );
        }
        
        private void PrintGridInitializeLayoutEvent(object sender, InitializeLayoutEventArgs e)
        {
            this.i_PrintGrid.DrawFilter = this;          
            Appearance gridAppearance = new Appearance();
            gridAppearance.BackColor = Color.White;
            gridAppearance.FontData.Name = "Tahoma";
            gridAppearance.FontData.SizeInPoints = 7;//9;
            e.Layout.Appearance = gridAppearance;

            e.Layout.Override.BorderStyleCell = UIElementBorderStyle.None;
            e.Layout.Override.BorderStyleRow = UIElementBorderStyle.Solid;
            e.Layout.Appearance.BorderColor = Border_Color;
            e.Layout.RowConnectorStyle = RowConnectorStyle.None;
            e.Layout.RowConnectorColor = SystemColors.Window;
            e.Layout.Override.RowAppearance.BorderColor = Row_Separator_Color;
            e.Layout.Override.CellAppearance.BorderColor = Color.FromArgb( 140, 140, 140 );

            e.Layout.Override.RowSizing = 
                RowSizing.AutoFixed;
            e.Layout.Override.ExpansionIndicator = ShowExpansionIndicator.Never;

            e.Layout.Override.AllowColSizing = AllowColSizing.Free;
            e.Layout.Override.ColumnAutoSizeMode = ColumnAutoSizeMode.AllRowsInBand;
            e.Layout.AutoFitStyle = AutoFitStyle.None;
            e.Layout.Override.RowSelectors = DefaultableBoolean.False;
                        
            e.Layout.Rows.ExpandAll( true );
        }
        private void printPreviewDialog_PageSetupDialogDisplaying(object sender, PageSetupDialogDisplayingEventArgs e)
        {
            e.Dialog.AllowMargins = false;
            e.Dialog.AllowOrientation = false;            
        }
        #endregion

        #region Methods
        
        public void GeneratePrintPreview()
        {
            // Uncomment these lines only if print preview hangs

//            PageSetupDialog pageSetupDialog = new PageSetupDialog();
//            pageSetupDialog.Document = this.i_GridPrintDocument;
//            pageSetupDialog.ShowDialog( this );
            UltraPrintPreviewDialog printPreviewDialog = new UltraPrintPreviewDialog();
            try
            {
                printPreviewDialog.PageSetupDialogDisplaying += 
                    new PageSetupDialogDisplayingEventHandler(printPreviewDialog_PageSetupDialogDisplaying);
                printPreviewDialog.Document = this.i_GridPrintDocument;
                printPreviewDialog.ShowDialog();
            }
            finally
            {
                printPreviewDialog.Dispose();
            }
        }       
        
        public override void UpdateView()
        {
            PrintGrid.DataSource = this.DataSource;
            PrintGrid.DataBind();
        }

        // Implemenatation of IUIElementDrawFilter Interface Method
        public DrawPhase GetPhasesToFilter( ref UIElementDrawParams drawParams ) 
        { 
            if ( drawParams.Element is RowCellAreaUIElement ) 
                return DrawPhase.BeforeDrawBorders;
            else
                return DrawPhase.None;  
        }
 
        // Implemenatation of IUIElementDrawFilter Interface Method
        public bool DrawElement( DrawPhase drawPhase, ref UIElementDrawParams drawParams ) 
        {  
            if( drawPhase.Equals( DrawPhase.BeforeDrawBorders ) )
            {
                drawParams.DrawBorders( UIElementBorderStyle.Solid, Border3DSide.Top );
            }
            return true;  
        }

        #endregion

        #region Properties

        public UltraDataSource DataSource
        {
            private get
            {
                return i_DataSource;
            }
            set
            {
                i_DataSource = value;
            }
        }

        public Appearance BandHeaderRowAppearance
        {
            get
            {
                return i_BandHeaderRowAppearance;
            }
        }

        public Appearance SummaryHeaderAppearance
        {
            get
            {
                return i_SummaryHeaderAppearance;
            }
        }
        public UltraGridOverride OverrideWithBorder
        {
            get
            {
                return i_OverrideWithBorder;
            }
        }
        public UltraGridOverride OverrideWithoutBorder
        {
            get
            {
                return i_OverrideWithoutBorder;
            }
        }
        public string HeaderText
        {
            private get
            {
                return i_HeaderText;
            }
            set
            {
                i_HeaderText = value;
            }
        }

        public string FooterText
        {
            private get
            {
                return i_FooterText;
            }
            set
            {
                i_FooterText = value;
            }
        }

        public UltraGridPrintDocument GridPrintDocument
        {
            get
            {
                return i_GridPrintDocument;
            }
        }

        public UltraGrid PrintGrid
        {
            get
            {
                return i_PrintGrid;
            }
        }
        
        public object SearchCriteria
        {
            get
            {
                return i_SearchCriteria;
            }
            set
            {
                i_SearchCriteria = value;
            }
        }

        public object SummaryInformation
        {
            get
            {
                return i_SummaryInformation;
            }
            set
            {
                i_SummaryInformation = value;
            }
        }

        #endregion

        #region Construction and Finalization
        public PrintReport()
        {			
            InitializeComponent();
            SetBandHeaderRowAppearance();
            SetSummaryHeaderAppearance();
            SetOverrideWithBorder();
            SetOverrideWithoutBorder();
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if(components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }
        #endregion 

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            i_GridPrintDocument = new Infragistics.Win.UltraWinGrid.UltraGridPrintDocument();
            i_PrintGrid = new Infragistics.Win.UltraWinGrid.UltraGrid();

            
            this.i_GridPrintDocument.Grid = i_PrintGrid;
            this.i_GridPrintDocument.Appearance.ForeColor = Color.Black;
            this.i_GridPrintDocument.Appearance.BackColor = Color.White;
            this.i_GridPrintDocument.PrintColorStyle = ColorRenderMode.Color;

            //
            // i_GridPrintDocument Header Settings
            //
            this.i_GridPrintDocument.Header.Appearance.BackColor = Header_Footer_Color;
            this.i_GridPrintDocument.Header.Appearance.FontData.Name = "Verdana";
            this.i_GridPrintDocument.Header.Appearance.FontData.SizeInPoints = 12;
            this.i_GridPrintDocument.Header.Appearance.ForeColor = Color.White;
            this.i_GridPrintDocument.Header.Appearance.FontData.Bold = DefaultableBoolean.False;
            this.i_GridPrintDocument.Header.Height = 69;
            this.i_GridPrintDocument.Header.TextCenter = this.HeaderText; 
            this.i_GridPrintDocument.Header.TextRight = User.GetCurrent().Facility.Code + 
                "\nDate of Report: " + 
                DateTime.Now.ToString( "MM/dd/yyyy HH:mm" ); 
            this.i_GridPrintDocument.Header.Appearance.TextVAlign = VAlign.Bottom;
            this.i_GridPrintDocument.Header.BorderStyle = UIElementBorderStyle.Solid; 
            this.i_GridPrintDocument.Header.BorderSides = Border3DSide.Top;
            this.i_GridPrintDocument.Header.Appearance.BorderColor = Border_Color;
            this.i_GridPrintDocument.Header.Padding.Right = 5;
            this.i_GridPrintDocument.Header.Padding.Left = 140;
            this.i_GridPrintDocument.Header.Padding.Top = 1;
            this.i_GridPrintDocument.Header.Padding.Bottom = 1;
 
            //
            // i_GridPrintDocument Footer Settings
            //
            this.i_GridPrintDocument.Footer.Appearance.FontData.Name = "Tahoma";
            this.i_GridPrintDocument.Footer.Appearance.FontData.SizeInPoints = 7;
            this.i_GridPrintDocument.Footer.BorderSides = Border3DSide.Bottom;
            this.i_GridPrintDocument.Footer.Appearance.BorderColor = Header_Footer_Color;
            this.i_GridPrintDocument.Footer.Height = 21;
            this.i_GridPrintDocument.Footer.Appearance.TextVAlign = VAlign.Middle;
            this.i_GridPrintDocument.Footer.TextCenter = this.FooterText;
            this.i_GridPrintDocument.Footer.TextRight = "<#>";
            this.i_GridPrintDocument.Footer.BorderStyle = UIElementBorderStyle.Solid;
            this.i_GridPrintDocument.Footer.ReverseTextOnEvenPages = false;
            this.i_GridPrintDocument.Footer.Appearance.TextVAlign = VAlign.Middle;
            this.i_GridPrintDocument.Footer.Padding.Right = 40;
            this.i_GridPrintDocument.Footer.Padding.Left = 50;
            this.i_GridPrintDocument.Footer.Padding.Top = 1;
            this.i_GridPrintDocument.Footer.Padding.Bottom = 1;            

            //
            // i_GridPrintDocument Page Settings
            //            
            this.i_GridPrintDocument.Page.BorderStyle = UIElementBorderStyle.Solid;
            this.i_GridPrintDocument.Page.BorderSides = Border3DSide.All;
            this.i_GridPrintDocument.Page.Appearance.BorderColor = Border_Color;
            this.i_GridPrintDocument.Page.Padding.Left = 2;
            this.i_GridPrintDocument.Page.Padding.Right = 2;
            this.i_GridPrintDocument.Page.Padding.Top = 2;
            this.i_GridPrintDocument.Page.Padding.Bottom = 2;

            this.i_GridPrintDocument.DefaultPageSettings.Margins.Bottom = 37;
            this.i_GridPrintDocument.DefaultPageSettings.Margins.Top = 17;
            this.i_GridPrintDocument.DefaultPageSettings.Margins.Left = 13;
            this.i_GridPrintDocument.DefaultPageSettings.Margins.Right = 40;
            this.i_GridPrintDocument.DefaultPageSettings.Landscape = true;

            //
            //i_GridPrintDocument Events
            //
            this.i_GridPrintDocument.PagePrinted += 
                new PagePrintedEventHandler(PrintDocumentPagePrintedEvent);
            this.i_GridPrintDocument.PagePrinting += 
                new PagePrintingEventHandler(PrintDocumentPagePrintingEvent);

            //
            // i_PrintGrid
            //
            this.i_PrintGrid.Name = "i_PrintGrid";
            this.i_PrintGrid.TabIndex = 0;
            this.i_PrintGrid.InitializeLayout += 
                new InitializeLayoutEventHandler(PrintGridInitializeLayoutEvent);

            //
            // i_GridPrintDocument
            //
            this.Controls.Add( i_PrintGrid );
        }
        #endregion

        #region Private Methods
        private void SetBandHeaderRowAppearance()
        {
            i_BandHeaderRowAppearance = new Appearance();
            i_BandHeaderRowAppearance.TextHAlign = HAlign.Left;
            i_BandHeaderRowAppearance.BackColor = Border_Color;
            i_BandHeaderRowAppearance.ForeColor = Color.White;
            i_BandHeaderRowAppearance.BorderColor = Color.FromArgb( 140, 140, 140 );
        }

        private void SetOverrideWithoutBorder()
        {
            i_OverrideWithoutBorder = new UltraGridOverride();
            i_OverrideWithoutBorder.BorderStyleCell = UIElementBorderStyle.None;
            i_OverrideWithoutBorder.BorderStyleRow = UIElementBorderStyle.None;
            i_OverrideWithoutBorder.RowAppearance.BorderColor = SystemColors.Window;
            i_OverrideWithoutBorder.RowAppearance.BorderAlpha = Alpha.Transparent;
        }

        private void SetOverrideWithBorder()
        {
            i_OverrideWithBorder = new UltraGridOverride();
            i_OverrideWithBorder.BorderStyleCell = UIElementBorderStyle.None;
            i_OverrideWithBorder.BorderStyleRow = UIElementBorderStyle.None;
            i_OverrideWithBorder.RowAppearance.BorderColor = Border_Color;
        }

        private void SetSummaryHeaderAppearance()
        {
            i_SummaryHeaderAppearance = new Appearance();
            i_SummaryHeaderAppearance.BackColor = SystemColors.Window;
            i_SummaryHeaderAppearance.ForeColor = Color.Black;
            i_SummaryHeaderAppearance.BorderColor = SystemColors.Window;
            i_SummaryHeaderAppearance.TextHAlign = HAlign.Left;
        }
        #endregion

        #region Private Properties
        #endregion

        #region Data Elements

        private string i_FooterText;
        private string i_HeaderText;
        private object i_SearchCriteria;
        private object i_SummaryInformation;
        private UltraGridOverride i_OverrideWithoutBorder;
        private UltraGridOverride i_OverrideWithBorder;
        private Appearance i_BandHeaderRowAppearance;
        private Appearance i_SummaryHeaderAppearance;
        private Container components = null;
        private UltraGridPrintDocument i_GridPrintDocument;        
        private UltraGrid i_PrintGrid;
        private UltraDataSource i_DataSource;
        protected Color Border_Color = Color.FromArgb( 159, 159, 159 );
        private Color Header_Footer_Color = Color.FromArgb( 121,127,136 );
        protected Color Row_Separator_Color = Color.FromArgb( 225, 225, 228 );

        #endregion

        #region Constants
        #endregion
        
    }
}

