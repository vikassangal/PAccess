using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.Logging;

namespace PatientAccess.UI.InsuranceViews.SelectBillingInfo
{
    public class SelectBillingInfoView : TimeOutFormView
    {
        #region Event Handlers
        public event EventHandler AddressSelectedEvent;

        private void SelectBillingInfoViewLoad( object sender, EventArgs e )
        {
            btnOk.Enabled                 = false;
            selectedBillingItem           = null;
            if( Model_Coverage == null || Model_Coverage.InsurancePlan == null )
            {
                return;
            }
            ICollection billingCollection = Model_Coverage.InsurancePlan.BillingInformations;

            if( billingCollection == null )
            {
                return;
            }

            lblPayorBroker.Text = Model_Coverage.InsurancePlan.PlanName;
            listBox.BeginUpdate();
            listBox.Items.Clear();

            foreach( BillingInformation billing in billingCollection )
            {
                if( billing == null )
                {
                    continue;
                }
                listBox.Items.Add( billing );
            }

            listBox.EndUpdate();
        }

        private void SelectBillingInfoView_Layout( object sender, LayoutEventArgs e )
        {   // This gets called after Load event handler
            if( listBox.Items.Count > 0 )
            {
                btnOk.Enabled = true;
                listBox.SetSelected( 0, true );
                listBox.SelectedIndex = 0;
            }
        }

        private void ListBoxDrawItem( object sender, DrawItemEventArgs e )
        {
            if( e.Index >= 0 )
            {
                RenderInListItem(e, (BillingInformation)listBox.Items[ e.Index ]);
            }
        }

        private void ListBoxMeasureItem( object sender, MeasureItemEventArgs e )
        {
            if( e.Index >= 0 )
            {
                BillingInformation info = (BillingInformation)listBox.Items[ e.Index ];
                int lineCount = addressLinesForAddress( info );
                e.ItemHeight = (listBox.Font.Height * (lineCount + 1 ) );
            }
        }

        private void ListBoxSelectedIndexChanged( object sender, EventArgs e )
        {
            BillingInformation aBI = (BillingInformation)listBox.SelectedItem;

            if( aBI != null )
            {
                selectedBillingItem = new BillingInformation();
                selectedBillingItem = aBI.Clone() as BillingInformation;
                BreadCrumbLogger.GetInstance.Log( " BillingInformation selected " + selectedBillingItem.BillingName );
            }
            
        }

        private void CancelButtonClick( object sender, EventArgs e )
        {
            this.Close();
        }

        private void OkButtonClick( object sender, EventArgs e )
        {
            if( selectedBillingItem == null )
            {
                return;
            }
            LooseArgs args = new LooseArgs( selectedBillingItem );

            if( AddressSelectedEvent != null )
            {
                AddressSelectedEvent( this, args );
            }
            this.Close();
        }
        #endregion

        #region Methods
        private int addressLinesForAddress( BillingInformation info )
        {
            stringCollection.Clear();

            if( info.BillingCOName != String.Empty )
            {
                stringCollection.Add( info.BillingCOName );
            }
            if( info.BillingName != String.Empty )
            {
                stringCollection.Add( info.BillingName );
            }
            if(info.Address != null)
            {
                if( info.Address.Address1 != String.Empty )
                {
                    stringCollection.Add( info.Address.Address1 );
                }
                if( info.Address.City != String.Empty )
                {
                    stringCollection.Add( info.Address.City );
                }
            }
            if( info.PhoneNumber.Number != String.Empty )
            {
                stringCollection.Add( info.PhoneNumber.Number );
            }
            if( info.Area != String.Empty )
            {
                stringCollection.Add( info.Area );
            }

            return stringCollection.Count;
        }

        private void RenderInListItem( DrawItemEventArgs eventArgs,
                                       BillingInformation info )
        {
            Rectangle bounds = eventArgs.Bounds;
            Color textColor  = SystemColors.ControlText;

            if( (eventArgs.State & DrawItemState.Selected) == DrawItemState.Selected )
            {
                if( (eventArgs.State & DrawItemState.Focus) == DrawItemState.Focus )
                {
                    eventArgs.DrawBackground();
                    eventArgs.DrawFocusRectangle();
                    textColor = SystemColors.HighlightText;
                }
                else
                {
                    using ( Brush b = new SolidBrush( SystemColors.InactiveBorder ) )
                    {
                        eventArgs.Graphics.FillRectangle( b, eventArgs.Bounds );
                    }
                }
            }
            else
            {
                eventArgs.DrawBackground();
            }
            int horizontalOffset = listBox.Font.Height / 3;
            int verticalOffset = horizontalOffset;

            using( SolidBrush textBrush = new SolidBrush( textColor ) )
            {
                renderLine( eventArgs, info.BillingCOName, textBrush, ref horizontalOffset, verticalOffset );
                horizontalOffset++;

                renderLine( eventArgs, info.BillingName, textBrush, ref horizontalOffset, verticalOffset );
                horizontalOffset++;
                if(info.Address != null)
                {
                    renderLine( eventArgs, info.Address.Address1, textBrush, ref horizontalOffset, verticalOffset );
                    horizontalOffset++;

                    renderLine( eventArgs, (info.Address.City + ", " + info.Address.State + " " + info.Address.ZipCode.PostalCode), textBrush, ref horizontalOffset, verticalOffset );
                    horizontalOffset++;
                }

                string phoneString = string.Empty;
                
                if( info.PhoneNumber != null )
                {
                    phoneString = info.PhoneNumber.AsFormattedString();
                }

                renderLine( eventArgs, "Phone: " + phoneString, textBrush, ref horizontalOffset, verticalOffset );
                horizontalOffset++;

                renderLine( eventArgs, "Area: " + info.Area, textBrush, ref horizontalOffset, verticalOffset );
            }

            Pen blackPen = new Pen( SystemColors.InactiveBorder );
            int lineCoord = eventArgs.Bounds.Top + eventArgs.Bounds.Height - 1;

            eventArgs.Graphics.DrawLine( blackPen, 0, lineCoord, eventArgs.Bounds.Width, lineCoord );
        }

        private void renderLine( DrawItemEventArgs e,
                                 string text,
                                 SolidBrush TextBrush,
                                 ref int top,
                                 int left)
        {
            if( top + e.Font.Height < e.Bounds.Height )
            {
                e.Graphics.DrawString( text, e.Font, TextBrush, e.Bounds.Left + left, e.Bounds.Top + top );
                top += e.Font.Height;
            }
        }
        #endregion

        #region Properties
        public Coverage Model_Coverage
        {
            private get
            {
                return (Coverage)this.Model;
            }
            set
            {
                this.Model = value;
            }
        }
        #endregion

        #region Private Properties
        #endregion

        #region Private Methods
        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.lblStaticPayorBroker = new System.Windows.Forms.Label();
			this.lblPayorBroker = new System.Windows.Forms.Label();
			this.panelTop = new System.Windows.Forms.Panel();
			this.panelBottom = new System.Windows.Forms.Panel();
			this.btnOk = new LoggingButton();
			this.btnCancel = new LoggingButton();
			this.panelMiddle = new System.Windows.Forms.Panel();
			this.listBox = new System.Windows.Forms.ListBox();
			this.panelHeader = new System.Windows.Forms.Panel();
			this.listViewHeader = new System.Windows.Forms.ListView();
			this.columnHeader = new System.Windows.Forms.ColumnHeader();
			this.panelTop.SuspendLayout();
			this.panelBottom.SuspendLayout();
			this.panelMiddle.SuspendLayout();
			this.panelHeader.SuspendLayout();
			this.SuspendLayout();
			// 
			// lblStaticPayorBroker
			// 
			this.lblStaticPayorBroker.Location = new System.Drawing.Point(7, 7);
			this.lblStaticPayorBroker.Name = "lblStaticPayorBroker";
			this.lblStaticPayorBroker.Size = new System.Drawing.Size(81, 19);
			this.lblStaticPayorBroker.TabIndex = 1;
			this.lblStaticPayorBroker.Text = "Payor/Broker:";
			// 
			// lblPayorBroker
			// 
			this.lblPayorBroker.Location = new System.Drawing.Point(80, 7);
			this.lblPayorBroker.Name = "lblPayorBroker";
			this.lblPayorBroker.Size = new System.Drawing.Size(240, 19);
			this.lblPayorBroker.TabIndex = 2;
			// 
			// panelTop
			// 
			this.panelTop.Controls.Add(this.lblPayorBroker);
			this.panelTop.Controls.Add(this.lblStaticPayorBroker);
			this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
			this.panelTop.Location = new System.Drawing.Point(0, 0);
			this.panelTop.Name = "panelTop";
			this.panelTop.Size = new System.Drawing.Size(450, 30);
			this.panelTop.TabIndex = 3;
			// 
			// panelBottom
			// 
			this.panelBottom.Controls.Add(this.btnOk);
			this.panelBottom.Controls.Add(this.btnCancel);
			this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panelBottom.DockPadding.Right = 30;
			this.panelBottom.Location = new System.Drawing.Point(0, 398);
			this.panelBottom.Name = "panelBottom";
			this.panelBottom.Size = new System.Drawing.Size(450, 36);
			this.panelBottom.TabIndex = 4;
			// 
			// btnOk
			// 
			this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOk.BackColor = System.Drawing.SystemColors.Control;
			this.btnOk.Location = new System.Drawing.Point(280, 7);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(74, 22);
			this.btnOk.TabIndex = 1;
			this.btnOk.Text = "OK";
			this.btnOk.Click += new System.EventHandler(this.OkButtonClick);
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.BackColor = System.Drawing.SystemColors.Control;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(361, 7);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(80, 22);
			this.btnCancel.TabIndex = 2;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.Click += new System.EventHandler(this.CancelButtonClick);
			// 
			// panelMiddle
			// 
			this.panelMiddle.Controls.Add(this.listBox);
			this.panelMiddle.Controls.Add(this.panelHeader);
			this.panelMiddle.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelMiddle.DockPadding.Left = 7;
			this.panelMiddle.DockPadding.Right = 7;
			this.panelMiddle.Location = new System.Drawing.Point(0, 30);
			this.panelMiddle.Name = "panelMiddle";
			this.panelMiddle.Size = new System.Drawing.Size(450, 368);
			this.panelMiddle.TabIndex = 0;
			// 
			// listBox
			// 
			this.listBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.listBox.IntegralHeight = false;
			this.listBox.ItemHeight = 100;
			this.listBox.Location = new System.Drawing.Point(7, 19);
			this.listBox.Name = "listBox";
			this.listBox.ScrollAlwaysVisible = true;
			this.listBox.Size = new System.Drawing.Size(436, 349);
			this.listBox.TabIndex = 0;
			this.listBox.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.ListBoxMeasureItem);
			this.listBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.ListBoxDrawItem);
			this.listBox.SelectedIndexChanged += new System.EventHandler(this.ListBoxSelectedIndexChanged);
			// 
			// panelHeader
			// 
			this.panelHeader.Controls.Add(this.listViewHeader);
			this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
			this.panelHeader.Location = new System.Drawing.Point(7, 0);
			this.panelHeader.Name = "panelHeader";
			this.panelHeader.Size = new System.Drawing.Size(436, 19);
			this.panelHeader.TabIndex = 0;
			// 
			// listViewHeader
			// 
			this.listViewHeader.BackColor = System.Drawing.SystemColors.Control;
			this.listViewHeader.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.listViewHeader.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																							 this.columnHeader});
			this.listViewHeader.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listViewHeader.Location = new System.Drawing.Point(0, 0);
			this.listViewHeader.Name = "listViewHeader";
			this.listViewHeader.Scrollable = false;
			this.listViewHeader.Size = new System.Drawing.Size(436, 19);
			this.listViewHeader.TabIndex = 0;
			this.listViewHeader.TabStop = false;
			this.listViewHeader.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader
			// 
			this.columnHeader.Text = "Billing Information";
			this.columnHeader.Width = 435;
			// 
			// SelectBillingInfoView
			// 
			this.AcceptButton = this.btnOk;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(450, 434);
			this.Controls.Add(this.panelMiddle);
			this.Controls.Add(this.panelBottom);
			this.Controls.Add(this.panelTop);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SelectBillingInfoView";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Select Billing Information";
			this.Load += new System.EventHandler(this.SelectBillingInfoViewLoad);
			this.Layout += new System.Windows.Forms.LayoutEventHandler(this.SelectBillingInfoView_Layout);
			this.panelTop.ResumeLayout(false);
			this.panelBottom.ResumeLayout(false);
			this.panelMiddle.ResumeLayout(false);
			this.panelHeader.ResumeLayout(false);
			this.ResumeLayout(false);

		}
        #endregion
        #endregion

        #region Constructors and Finalization
        public SelectBillingInfoView()
        {
            InitializeComponent();
            stringCollection = new StringCollection();
			base.EnableThemesOn( this );
        }

        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if( components != null )
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }
        #endregion

        #region Data Elements
        private Container     components = null;
        private LoggingButton         btnCancel;
        private LoggingButton         btnOk;

        private ColumnHeader   columnHeader;

        private Label          lblPayorBroker;
        private Label          lblStaticPayorBroker;
        
        private Panel          panelTop;
        private Panel          panelBottom;
        private Panel          panelMiddle;
        private Panel          panelHeader;
        
        private ListView       listViewHeader;
        private ListBox        listBox;
        private StringCollection                    stringCollection;
        private BillingInformation                  selectedBillingItem;
        #endregion

        #region Constants
        #endregion
    }
}
