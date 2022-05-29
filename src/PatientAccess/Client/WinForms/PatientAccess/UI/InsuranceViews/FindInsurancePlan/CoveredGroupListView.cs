using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI.InsuranceViews.FindInsurancePlan
{
    /// <summary>
    /// Summary description for CoveredGroupListView.
    /// </summary>
    public class CoveredGroupListView : CoverControlView
    {
        #region Events
        public event EventHandler SelectedCoveredGroupChanged;
        public event EventHandler CloseFormOnEnterKeyEvent;
        #endregion

        #region Event Handlers
        private void CustomKeyEventHandler(object sender, EventArgs e)
        {   // Close dialog if ENTER key pressed if OK button is disabled.
            if( CloseFormOnEnterKeyEvent != null )
            {
                CloseFormOnEnterKeyEvent( this, e );
            }
        }

        /// <summary>
        /// Raise event for dialog's parent to catch new group
        /// </summary>
        protected virtual void OnSelectedCoveredGroupChanged( EventArgs e ) 
        {
            if( SelectedCoveredGroupChanged != null) 
            {
                LooseArgs args = new LooseArgs( SelectedCoveredGroup );
                SelectedCoveredGroupChanged( this, args );
            }
        }

        private void listBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if( e.Index < 0 )
            {
                return;
            }
            CoveredGroup coveredGroup = listBox.Items[ e.Index ] as CoveredGroup;
            Color textColor = SystemColors.ControlText;

            // render background and select text color based on selected and focus states
            if( (e.State & DrawItemState.Selected) == DrawItemState.Selected )
            {
                if( (e.State & DrawItemState.Focus) == DrawItemState.Focus )
                {
                    e.DrawBackground();
                    e.DrawFocusRectangle();
                    textColor = SystemColors.HighlightText;
                }
                else
                {
                    using( Brush b = new SolidBrush( SystemColors.InactiveBorder) )
                    {
                        e.Graphics.FillRectangle( b, e.Bounds );
                    }	
                }
            }
            else
            {
                e.DrawBackground();
            }

            using( SolidBrush textBrush = new SolidBrush( textColor ) )
            {
                int top = 2;
                int tempTop = top;
                renderLine( e, coveredGroup.Name, textBrush, ref top, 0 );

                if( coveredGroup.Employer != null )
                {
                    renderLine(e, coveredGroup.Employer.NationalId, textBrush, ref tempTop, NATIONALID_COLUMN_POSN );
                }

                StringCollection addressLines = addressLinesForCoveredGroup( coveredGroup );

                foreach( string s in addressLines  )
                {
                    renderLine( e, s, textBrush, ref top, 20 );
                }
            }

            Pen blackPen = new Pen( SystemColors.InactiveBorder );
            int y = e.Bounds.Top + e.Bounds.Height - 1;
            // Draw line below item
            e.Graphics.DrawLine( blackPen, 0, y, e.Bounds.Width, y );
        }

        private void listBox_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            CoveredGroup coveredGroup = (CoveredGroup)listBox.Items[ e.Index ];
            int numLines = measureAddressLInes( coveredGroup );
            e.ItemHeight = listBox.Font.Height * (numLines + 1) + ITEMHEIGHT_OVERHEAD; // (Add 1 for group name)
        }

        private void listBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListBox lb = sender as ListBox;
            if( lb.SelectedIndex != -1 )
            {
                SelectedCoveredGroup = listBox.SelectedItem as CoveredGroup;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Populate listBox with covered groups
        /// </summary>
        public void DisplayCoveredGroups( ICollection coveredGroups )
        {
            listBox.BeginUpdate();

            if( coveredGroups != null )
            {
                listBox.Items.Clear();
                foreach( CoveredGroup coveredGroup in coveredGroups )
                {
                    listBox.Items.Add( coveredGroup );
                }
                if( listBox.Items.Count > 0 )
                {
                    listBox.SetSelected( 0, true );
                }
            }

            ShowCover = (coveredGroups != null && listBox.Items.Count == 0);
            if( ShowCover )
            {
                listBox.Items.Clear();
            }
            listBox.EndUpdate();
        }

        public void Activate()
        {
            FindForm().ActiveControl = listBox;
        }
        #endregion

        #region Properties
        [Browsable(false)]
        public ICollection CoveredGroups
        {
            get
            {
                return i_CoveredGroups;
            }
            set
            {
                i_CoveredGroups = value;
            }
        }

        [Browsable(false)]
        public CoveredGroup SelectedCoveredGroup
        {
            get
            {
                return i_SelectedCoveredGroup;
            }
            set
            {
                if( value != i_SelectedCoveredGroup )
                {
                    i_SelectedCoveredGroup = value;
                    OnSelectedCoveredGroupChanged( EventArgs.Empty );
                }
            }
        }
        #endregion

        #region Private Methods
        private StringCollection addressLinesForCoveredGroup( CoveredGroup coveredGroup )
        {
            ICollection contactPoints = coveredGroup.ContactPoints;
            StringCollection result = dummyEmptyList;

            if( contactPoints.Count > 0 )
            {
                foreach( ContactPoint cp in contactPoints )
                {
                    if( cp.TypeOfContactPoint != null
                        && cp.TypeOfContactPoint.Oid == TypeOfContactPoint.NewEmployerContactPointType().Oid )
                    {
                        StringCollection sc = cp.Address.GetMailingLabelStrings();
                        if( sc.ToString() != String.Empty )
                        {
                            result = sc;
                            break;
                        }
                    }                    
                }
            }
            return result;
        }

        /// <summary>
        /// Count the number of lines in address for 
        /// </summary>
        private int measureAddressLInes( CoveredGroup coveredGroup )
        {
            int length = 0;
            ICollection contactPoints = coveredGroup.ContactPoints;

            if( contactPoints.Count > 0 )
            {
                foreach( ContactPoint cp in contactPoints )
                {
                    StringCollection addressLines = cp.Address.GetMailingLabelStrings();
                    foreach( string s in addressLines  )
                    {
                        if( s.Length > 0 )
                        {
                            length++;
                        }
                    }
                }
            }
            return length;
        }

        private void renderLine( DrawItemEventArgs e, string text,
            SolidBrush TextBrush, ref int top, int left )
        {
            if( top + e.Font.Height < e.Bounds.Height  )
            {
                e.Graphics.DrawString( text, e.Font, TextBrush, e.Bounds.Left + left, e.Bounds.Top + top );
                top += e.Font.Height;
            }
        }
        #endregion

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.listBox = new PatientAccess.UI.InsuranceViews.FindInsurancePlan.ListBoxKeyHdlr();
            this.SuspendLayout();
            // 
            // listBox
            // 
            this.listBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.listBox.IntegralHeight = false;
            this.listBox.Location = new System.Drawing.Point(0, 0);
            this.listBox.Name = "listBox";
            this.listBox.Size = new System.Drawing.Size(375, 250);
            this.listBox.TabIndex = 1;
            this.listBox.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.listBox_MeasureItem);
            this.listBox.SelectedIndexChanged += new System.EventHandler(this.listBox_SelectedIndexChanged);
            this.listBox.CustomKeyEvent += new System.EventHandler(this.CustomKeyEventHandler);
            this.listBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listBox_DrawItem);
            // 
            // CoveredGroupListView
            // 
            this.Controls.Add(this.listBox);
            this.CoverMessage = "No Items Found";
            this.CoverPadding = 40;
            this.Name = "CoveredGroupListView";
            this.ShowCover = true;
            this.Size = new System.Drawing.Size(375, 250);
            this.Controls.SetChildIndex(this.listBox, 0);
            this.ResumeLayout(false);

        }
        #endregion

        #region Constructors and Finalization
        public CoveredGroupListView()
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
        private Container     components = null;
        private ListBoxKeyHdlr listBox;

        private CoveredGroup                        i_SelectedCoveredGroup;
        private StringCollection                    dummyEmptyList = new StringCollection();
        private ICollection                         i_CoveredGroups;
        #endregion

        #region Constants
        const int ITEMHEIGHT_OVERHEAD = 4;
        const int NATIONALID_COLUMN_POSN = 418;
        #endregion

    }
}
