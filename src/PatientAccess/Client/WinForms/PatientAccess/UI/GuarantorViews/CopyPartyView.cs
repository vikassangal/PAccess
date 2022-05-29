using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.UI.GuarantorViews
{
    /// <summary>
    /// Summary description for CopyPartyView.
    /// </summary>
    [Serializable]
    public class CopyPartyView : ControlView
    {
        #region Event Handlers
        public event EventHandler PartySelected;

        private void cmbCopyTo_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetSelectedParty();
        }
        #endregion

        #region Methods
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

        public override void UpdateView()
        {
            this.InitializeCopyTo();
        }

        /// <summary>
        /// UpdateModel method.
        /// </summary>
        public override void UpdateModel()
        {   

        }

        public void ResetView()
        {
            cmbCopyTo.SelectedIndex = -1;
        }
        #endregion

        #region Properties

        private Account Model_Account
        {
            get
            {
                return (Account)this.Model;
            }
        }

        public Type KindOfTargetParty
        {
            private get
            {
                return i_KindOfTargetParty;
            }
            set
            {
                i_KindOfTargetParty = value;
            }
        }
        public CoverageOrder CoverageOrder
        {
            get
            {
                return i_CoverageOrder;
            }
            set
            {
                i_CoverageOrder = value;
            }
        }

        public ComboBox ComboBox
        {
            get
            {
                return cmbCopyTo;
            }
        }
        #endregion

        #region Private Methods
        private void InitializeCopyTo()
        {
            if( base.IsInRuntimeMode )
            {
                cmbCopyTo.Items.Clear();
                
                cmbCopyTo.ValueMember   = "Value";
                cmbCopyTo.DisplayMember = "Key";

                cmbCopyTo.Items.Add("");
                cmbCopyTo.SelectedItem  = "";

                if( this.Model_Account != null )
                {
                    IDictionary entries  = this.Model_Account.PartiesForCopyingTo( this.KindOfTargetParty ,this.CoverageOrder );

                     foreach( DictionaryEntry entry in entries )
                    {
                         if( entry.Key.ToString() == "Patient" )
                         {
                             cmbCopyTo.Items.Add( entry );
                             break;
                         }
                        
                    }
                    foreach( DictionaryEntry entry in entries )
                    {
                        if( entry.Key.ToString() == "Patient's Employer" )
                        {
                            cmbCopyTo.Items.Add( entry );
                            break;
                        }
                        
                    }
                    foreach( DictionaryEntry entry in entries )
                    {
                        if( entry.Key.ToString() == "Guarantor" )
                        {
                            cmbCopyTo.Items.Add( entry );
                            break;
                        }
                        
                    }
                    foreach( DictionaryEntry entry in entries )
                    {
                        if( entry.Key.ToString() == "Insured - Primary" )
                        {
                            cmbCopyTo.Items.Add( entry );
                            break;
                        }
                       
                    }
                    foreach( DictionaryEntry entry in entries )
                    {
                        if( entry.Key.ToString() ==  "Insured - Secondary" )
                        {
                            cmbCopyTo.Items.Add( entry );
                            break;
                        }
                       
                    }


                }
            }
            cmbCopyTo.Refresh();
        }

        private void GetSelectedParty()
        {
            if( cmbCopyTo.Items.Count > 1 && cmbCopyTo.SelectedIndex > 0 )
            {
                Party selectedParty = ((DictionaryEntry)cmbCopyTo.SelectedItem).Value as Party;
                LooseArgs e = new LooseArgs( selectedParty );

                if( PartySelected != null )
                {
                    PartySelected( this, e );
                }
            }
        }   

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cmbCopyTo = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // cmbCopyTo
            // 
            this.cmbCopyTo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
                | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbCopyTo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCopyTo.Location = new System.Drawing.Point(1, 1);
            this.cmbCopyTo.Name = "cmbCopyTo";
            this.cmbCopyTo.Size = new System.Drawing.Size(174, 21);
            this.cmbCopyTo.TabIndex = 1;
            this.cmbCopyTo.SelectedIndexChanged += new System.EventHandler(this.cmbCopyTo_SelectedIndexChanged);
            // 
            // CopyPartyView
            // 
            this.Controls.Add(this.cmbCopyTo);
            this.Name = "CopyPartyView";
            this.Size = new System.Drawing.Size(178, 24);
            this.ResumeLayout(false);

        }
        #endregion

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public CopyPartyView()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
            //base.EnableThemesOn( this );
        }
        #endregion

        #region Data Elements
        private Container components = null;
        private ComboBox   cmbCopyTo;
        private Type                            i_KindOfTargetParty;
        private CoverageOrder i_CoverageOrder = new CoverageOrder();
        #endregion

        #region Constants
        #endregion
    }
}
