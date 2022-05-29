using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.UI.InsuranceViews.InsuranceVerificationViews
{
    /// <summary>
    /// Summary description for InsuranceVerificationView.
    /// </summary>
    [Serializable]
    public class InsuranceVerificationView : ControlView
    {
        #region Event Handlers

        private void splitter1_LocationChanged(object sender, EventArgs e)
        {
            //return;

            Size newSize1   = new Size(0,0);
            Size newSize2   = new Size(0,0);

            int height = this.splitter1.Location.Y - 85;

            if( height <= 0 )
            {
                newSize1.Height = 85 - height;
            }
            else
            {
                newSize1.Height  = height;
            }
            
            newSize1.Width   = this.verificationCoverage.Width;

            this.verificationResponse.richTextBox.Size = newSize1;         
        }

        private void verificationCoverage_ShowValidationResponseEvent(object sender, EventArgs e)
        {
            string response = string.Empty;

            if( e != null )
            {
                response = ( (LooseArgs)e ).Context as string;
            }
            
            this.verificationResponse.richTextBox.Text = response;
            if( response != BenefitsValidationResponse.RESPONSE_TEXT_ON_MISMATCH )
            {
                // SR39495 - populate the verification entry screen with results from the call to
                // Data Validation.  Note, the domain was updated in VerificationCoverage view.

                this.verificationEntry.UpdateView();
                if( this.Model_Coverage.GetType() == typeof( CommercialCoverage ) )
                {
                    this.verificationEntry.UpdateBenefitsCategoriesView();
                }
            }
        } 

        private void InsuranceVerificationView_Load(object sender, EventArgs e)
        {
            this.splitter1_LocationChanged(this, null);
        }
        #endregion

        #region Methods

        public void UpdateView(bool runVerificationRules)
        {
            if( Model_Coverage != null && Model_Account != null)
            {
                verificationEntry.Model_Coverage = Model_Coverage;
                verificationEntry.Account        = Model_Account;
                verificationEntry.UpdateView();
                
                verificationCoverage.Model_Coverage = Model_Coverage;
                verificationCoverage.Account        = Model_Account;

                if( runVerificationRules )
                {
                    verificationCoverage.UpdateView();
                }
                else
                {
                    verificationCoverage.PopulateView();
                }
                
            }
            // Set the Initiate button to have the focus
            verificationCoverage.InitiateButton.Focus();
            Refresh();
        }

        public override void UpdateModel()
        {           
            verificationCoverage.UpdateModel();
            verificationEntry.UpdateModel();
        }
        #endregion

        #region Properties
        public Coverage Model_Coverage
        {
            set
            {
                this.Model = value;
            }
            private get
            {
                return (Coverage)this.Model;
            }
        }

        public Account Model_Account
        {
            private get
            {
                return i_Account;
            }
            set
            {
                i_Account = value;
            }
        }

        public Insured Model_Insured
        {
            get
            {
                return i_Insured;
            }
            set
            {
                i_Insured = value;
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
			this.panel1 = new System.Windows.Forms.Panel();
			this.verificationCoverage = new PatientAccess.UI.InsuranceViews.InsuranceVerificationViews.VerificationCoverage();
			this.panel2 = new System.Windows.Forms.Panel();
			this.verificationEntry = new PatientAccess.UI.InsuranceViews.InsuranceVerificationViews.VerificationEntry();
			this.verificationResponse = new PatientAccess.UI.InsuranceViews.InsuranceVerificationViews.VerificationResponse();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.verificationCoverage);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(885, 85);
			this.panel1.TabIndex = 0;
			// 
			// verificationCoverage
			// 
			this.verificationCoverage.Account = null;
			this.verificationCoverage.BackColor = System.Drawing.Color.White;
			this.verificationCoverage.Dock = System.Windows.Forms.DockStyle.Fill;
			this.verificationCoverage.Location = new System.Drawing.Point(0, 0);
			this.verificationCoverage.Model = null;
			this.verificationCoverage.Model_Coverage = null;
			this.verificationCoverage.Name = "verificationCoverage";
			this.verificationCoverage.Size = new System.Drawing.Size(885, 90);
			this.verificationCoverage.TabIndex = 0;
			this.verificationCoverage.ShowValidationResponseEvent += new System.EventHandler(this.verificationCoverage_ShowValidationResponseEvent);
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.verificationEntry);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel2.Location = new System.Drawing.Point(0, 230);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(885, 225);
			this.panel2.TabIndex = 1;
			// 
			// verificationEntry
			// 
			this.verificationEntry.Account = null;
			this.verificationEntry.BackColor = System.Drawing.Color.White;
			this.verificationEntry.Dock = System.Windows.Forms.DockStyle.Fill;
			this.verificationEntry.Location = new System.Drawing.Point(0, 0);
			this.verificationEntry.Model = null;
			this.verificationEntry.Model_Coverage = null;
			this.verificationEntry.Name = "verificationEntry";
			this.verificationEntry.Size = new System.Drawing.Size(885, 225);
			this.verificationEntry.TabIndex = 8;
			// 
			// verificationResponse
			// 
			this.verificationResponse.BackColor = System.Drawing.Color.White;
			this.verificationResponse.Dock = System.Windows.Forms.DockStyle.Fill;
			this.verificationResponse.Location = new System.Drawing.Point(0, 90);
			this.verificationResponse.Model = null;
			this.verificationResponse.Name = "verificationResponse";
			this.verificationResponse.Size = new System.Drawing.Size(885, 140);
			this.verificationResponse.TabIndex = 0;
			this.verificationResponse.TabStop = false;
			// 
			// splitter1
			// 
			this.splitter1.BackColor = System.Drawing.Color.DarkBlue;
			this.splitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.splitter1.Location = new System.Drawing.Point(0, 225);
			this.splitter1.MinSize = 50;
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(885, 5);
			this.splitter1.TabIndex = 0;
			this.splitter1.TabStop = false;
			this.splitter1.LocationChanged += new System.EventHandler(this.splitter1_LocationChanged);
			// 
			// InsuranceVerificationView
			// 
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.splitter1);
			this.Controls.Add(this.verificationResponse);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.panel1);
			this.Name = "InsuranceVerificationView";
			this.Size = new System.Drawing.Size(885, 455);
			this.Load += new System.EventHandler(this.InsuranceVerificationView_Load);
    
            this.Leave +=new EventHandler(InsuranceVerificationView_Leave);

			this.panel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.ResumeLayout(false);

		}
        #endregion

        #endregion

        #region Construction and Finalization
        public InsuranceVerificationView()
        {
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
        private IContainer                components = null;
        
        private Account                                         i_Account;
        private Insured                                         i_Insured;

        private Panel panel1;
        private Panel panel2;
        private Splitter splitter1;
        
        public VerificationEntry verificationEntry;
        public VerificationCoverage verificationCoverage;
        private VerificationResponse verificationResponse = new VerificationResponse();

        #endregion

        #region Constants

        #endregion

        private void InsuranceVerificationView_Leave(object sender, EventArgs e)
        {
            this.verificationCoverage.UpdateModel();
        }
    }
}
