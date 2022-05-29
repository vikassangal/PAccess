using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Appearance = Infragistics.Win.Appearance;
using ToolTip = System.Windows.Forms.ToolTip;

namespace PatientAccess.UI.CommonControls
{
	/// <summary>
	/// This control implements all common grid features required in
	/// Census Inquiry reports screens.
	/// </summary>
	public class GridControl : ControlView
	{
        #region Event Handlers
        private void GridControl_Resize( object sender, EventArgs e)
        {
            this.ultraGrid1.Location = new Point(0, 0);
            this.ultraGrid1.Size = this.Size;
        }
        
        private void ultraGrid1_AfterRowActivate(object sender, EventArgs e)
        {
            OnGridControl_Click ();
        }
       
        private void BeforeSortOrderChange( object sender, BeforeSortChangeEventArgs e )
        {
            GridControl_BeforeSortOrderChange( sender, e );
        }
        private void ultraGrid1_BeforeSelectChange( object sender, BeforeSelectChangeEventArgs e )
        {
            e.Cancel  = true;  
        }

        private void ultraGrid1_DoubleClick(object sender, EventArgs e)
        {
            //Determine if a DoubleClick event took place on a row or not
            if( sender != null )
            {

                UltraGrid grid = sender as UltraGrid;
                if( grid != null )
                {
                    //Get the last element that the mouse entered
                    UIElement lastElementEntered = grid.DisplayLayout.UIElement.LastElementEntered;

                    if( lastElementEntered == null )
                    {
                        return;
                    }

                    //See if there's a RowUIElement in the chain.
                    RowUIElement rowElement = null;
                    if( lastElementEntered is RowUIElement )
                    {
                        rowElement = (RowUIElement)lastElementEntered;
                    }
                    else
                    {
                        rowElement = (RowUIElement)lastElementEntered.GetAncestor( typeof( RowUIElement ) );
                    }

                    if( rowElement == null )
                    {
                        return;
                    }

                    //Try to get a row from the element
                    UltraGridRow row = (UltraGridRow)rowElement.GetContext( typeof( UltraGridRow ) );

                    //If no row was returned, then the mouse is not over a row. 
                    if( row == null )
                    {
                        return;
                    }

                    //Handle DoubleClick event if it is on a valid row
                    OnGridControl_DoubleClick();
                }
            }
        }

        private void GridControl_Enter(object sender, EventArgs e)
        {
            SetRowSelectionActiveAppearance();
            
        }

        private void GridControl_Leave(object sender, EventArgs e)
        {
            SetRowSelectionDimAppearance();
        }

        private void ultraGrid1_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Indentation = 0;

            Appearance gridAppearance = new Appearance();
            gridAppearance.BackColor = Color.White;
            e.Layout.Appearance = gridAppearance;

            Appearance headerAppearance = new Appearance();
            headerAppearance.BackColor = Color.LightGray;
            headerAppearance.TextHAlign = HAlign.Left;
            e.Layout.UseFixedHeaders = false;
            e.Layout.Override.HeaderAppearance = headerAppearance;
            e.Layout.Bands[0].ColHeadersVisible = true;
            e.Layout.Override.AllowColSizing = AllowColSizing.None;
            e.Layout.Override.AllowColMoving = AllowColMoving.NotAllowed;
            e.Layout.Override.AllowColSwapping = AllowColSwapping.NotAllowed;
            e.Layout.Override.AllowUpdate = DefaultableBoolean.False;
            e.Layout.Override.TipStyleCell = TipStyle.Hide;

            e.Layout.Override.SelectTypeRow = SelectType.SingleAutoDrag;
            e.Layout.Override.RowSizing = RowSizing.AutoFree;
            e.Layout.Override.RowSelectors = DefaultableBoolean.False;
            e.Layout.RowConnectorStyle = RowConnectorStyle.None;
            e.Layout.Override.RowSizing = RowSizing.AutoFree;
            e.Layout.Override.ExpansionIndicator = ShowExpansionIndicator.Never;
            e.Layout.Override.RowAppearance.BorderColor = SystemColors.WindowText;
            e.Layout.Override.RowAppearance.BorderColor = Color.LightGray;

            e.Layout.Override.CellClickAction = CellClickAction.RowSelect;
            e.Layout.Override.CellAppearance.TextTrimming = TextTrimming.EllipsisCharacter;
            e.Layout.Override.CellAppearance.BorderAlpha = Alpha.Transparent;
            e.Layout.Override.BorderStyleCell = UIElementBorderStyle.None;

            e.Layout.Override.ColumnAutoSizeMode = ColumnAutoSizeMode.None;
            e.Layout.Override.SelectTypeCol = SelectType.None;
            e.Layout.AutoFitStyle = AutoFitStyle.ResizeAllColumns;

            e.Layout.BorderStyle = UIElementBorderStyle.None;
            ultraGrid1.FlatMode = true;
            ultraGrid1.SupportThemes = true;
             

            ultraGrid1.Rows.ExpandAll(true);
            ultraGrid1.Refresh();

        }

        private void OnTimerTick( object sender, EventArgs e )
        {
            tooltip = new ToolTip();
            tooltip.SetToolTip( ultraGrid1, msg );
            timer.Stop();
        }

        private void ultraGrid1_MouseEnterElement(object sender, UIElementEventArgs e)
        {
            // if we are not entering a cell, then don't do anything
            if( ! ( e.Element is CellUIElement ) )
            {
                return;
            }
			
            // find the cell that the cursor is over, if any
            UltraGridCell cell = e.Element.GetContext( typeof( UltraGridCell ) ) as UltraGridCell;
            if( cell != null )
            {
                msg = cell.Text;
                timer.Stop();
                timer.Start();
            }
        }

        private void ultraGrid1_MouseLeaveElement(object sender, UIElementEventArgs e)
        {
            // if we are not leaving a cell, then don't do anything
            if( ! ( e.Element is CellUIElement ) )
            {
                return;
            }
			
            // prevent the timer from ticking again
            timer.Stop();

            // destroy the tooltip
            if( tooltip != null )
            {
                tooltip.SetToolTip( this, String.Empty );
                tooltip.Dispose();
                tooltip = null;
            }
        }

        private void OnGridControl_Click()
        {
            if( GridControl_Click != null )
            {
                GridControl_Click ( this.ultraGrid1.ActiveRow );
            }
        }

        private void OnGridControl_DoubleClick()
        {
            if(this.ultraGrid1 != null)
            {
                if( GridControl_DoubleClick != null && this.ultraGrid1.ActiveRow != null)
                {
                    GridControl_DoubleClick ( this.ultraGrid1.ActiveRow );
                    //check if the row selected is already disabled.
                    if( this.ultraGrid1.ActiveRow.Activation != Activation.Disabled )
                    {
                        this.ultraGrid1.ActiveRow.Activated = false;
                    }
                }
            }
        }

        
      
        #endregion

        #region Methods
        public void PadBlankRows (int numberOfRows)
        {
            //turn off events for now so the grid does not fire any updating
            //deactivating events while we add our row
            this.ultraGrid1.EventManager.SetEnabled(EventGroups.AllEvents,false);
            for( int i = 0; i <= TOTAL_GRID_ROWS; i++)
            {
                //add a blank row
                this.ultraGrid1.DisplayLayout.Bands[0].AddNew();
            }
            this.ultraGrid1.EventManager.SetEnabled(EventGroups.AllEvents,true);
        }

        public void SetRowSelectionDimAppearance()
        {
            Appearance rowAppearance = new Appearance();
            rowAppearance.BackColor = SystemColors.Control;
            rowAppearance.ForeColor = Color.Black;
            UltraGridLayout gridLayout= this.ultraGrid1.DisplayLayout; 
            if( this.ultraGrid1.Rows.Count > 0 && this.ultraGrid1.ActiveRow != null)
            {
                gridLayout.Override.ActiveRowAppearance = rowAppearance;
                gridLayout.Override.SelectedRowAppearance = rowAppearance;
            }          
        }
        public void SetRowSelectionActiveAppearance()
        {
            
            UltraGridLayout gridLayout= this.ultraGrid1.DisplayLayout; 
            Appearance activeRowAppearance = new Appearance();
            activeRowAppearance.BackColor = SystemColors.ActiveCaption;
            activeRowAppearance.ForeColor = SystemColors.ActiveCaptionText; 
          
            if( this.ultraGrid1.Rows.Count > 0   )
            {
                gridLayout.Override.ActiveRowAppearance = activeRowAppearance;
                gridLayout.Override.SelectedRowAppearance = activeRowAppearance;

            }
        }

        #endregion

        #region Properties
        public UltraGrid CensusGrid
        {
            get
            {
                return ultraGrid1;
            }
            set
            {
                ultraGrid1 = value;
            }
        }
        #endregion

        #region Private Methods
        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            this.ultraGrid1 = new Infragistics.Win.UltraWinGrid.UltraGrid();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGrid1)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraGrid1
            // 
            this.ultraGrid1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
                | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            appearance1.BackColor = System.Drawing.Color.White;
            this.ultraGrid1.DisplayLayout.Appearance = appearance1;
            this.ultraGrid1.DisplayLayout.MaxColScrollRegions = 1;
            this.ultraGrid1.DisplayLayout.MaxRowScrollRegions = 1;

            this.ultraGrid1.DisplayLayout.TabNavigation = TabNavigation.NextControl; 
            this.ultraGrid1.DisplayLayout.Override.TipStyleScroll = Infragistics.Win.UltraWinGrid.TipStyle.Hide;
            this.ultraGrid1.Location = new System.Drawing.Point(0, 0);
            this.ultraGrid1.Name = "ultraGrid1";
            this.ultraGrid1.Size = new System.Drawing.Size(384, 224);
            this.ultraGrid1.TabIndex = 0;
            this.ultraGrid1.TabStop = false;
             
                       
            this.ultraGrid1.DoubleClick +=
                new System.EventHandler(this.ultraGrid1_DoubleClick);
            this.ultraGrid1.DisplayLayout.Override.TipStyleScroll=
                Infragistics.Win.UltraWinGrid.TipStyle.Hide;
            this.ultraGrid1.MouseLeaveElement += 
                new Infragistics.Win.UIElementEventHandler(this.ultraGrid1_MouseLeaveElement);
            this.ultraGrid1.InitializeLayout += 
                new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ultraGrid1_InitializeLayout);
            this.ultraGrid1.MouseEnterElement += 
                new Infragistics.Win.UIElementEventHandler(this.ultraGrid1_MouseEnterElement);
            this.ultraGrid1.AfterRowActivate += 
                new System.EventHandler(this.ultraGrid1_AfterRowActivate);
            this.ultraGrid1.DoubleClick += new EventHandler(ultraGrid1_DoubleClick);
            this.ultraGrid1.BeforeSortChange += 
                new Infragistics.Win.UltraWinGrid.BeforeSortChangeEventHandler(this.BeforeSortOrderChange);
            this.ultraGrid1.BeforeSelectChange +=
                new Infragistics.Win.UltraWinGrid.BeforeSelectChangeEventHandler (this.ultraGrid1_BeforeSelectChange);
            // 
            // GridControl
            // 
            this.Controls.Add(this.ultraGrid1);
            this.Name = "GridControl";
            this.Size = new System.Drawing.Size(384, 224);
            this.Resize += new System.EventHandler(this.GridControl_Resize);
            this.Enter += new System.EventHandler(this.GridControl_Enter);
            this.Leave += new System.EventHandler(this.GridControl_Leave);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGrid1)).EndInit();

            

            timer.Interval = DELAY_BEFORE_SHOW_TOOLTIP;
            timer.Tick += new EventHandler( OnTimerTick );

            base.EnableThemesOn( this );

            this.ResumeLayout(false);

        }
        #endregion
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public GridControl()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
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

        #region Data Elements
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private Container components = null;
      
        private UltraGrid ultraGrid1;
        /// <summary>
        /// The tooltip that we will use when the cursor is over a cell of the grid
        /// with a delay before the tooltip message appears
        /// </summary>
        private ToolTip tooltip = null;
        private Timer timer = new Timer();
        private string msg = String.Empty;

        public delegate void UltraGridClickEventHandler
            ( UltraGridRow ultraGridRow );
        public delegate void UltraGridDoubleClickEventHandler
            ( UltraGridRow ultraGridRow );
        public delegate void BeforeSortChangeEventHandler
            ( object sender, BeforeSortChangeEventArgs e );
        public event UltraGridClickEventHandler GridControl_Click;
        public event UltraGridDoubleClickEventHandler GridControl_DoubleClick;
        public event BeforeSortChangeEventHandler GridControl_BeforeSortOrderChange;

        #endregion

        #region Constants
        const int DELAY_BEFORE_SHOW_TOOLTIP = 50;
        const int TOTAL_GRID_ROWS = 10;
        #endregion

       
    }
}
