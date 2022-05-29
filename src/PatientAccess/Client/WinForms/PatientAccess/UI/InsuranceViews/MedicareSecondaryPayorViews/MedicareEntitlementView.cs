using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI.InsuranceViews.MedicareSecondaryPayorViews
{
    /// <summary>
    /// Summary description for MedicareEntitlementView.
    /// </summary>
    public class MedicareEntitlementView : ControlView
    {
        [DllImport("User32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        #region Event Handlers
        private void rbESRD_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;

            if( rb.Checked )
            {
                Model_MedicareSecondaryPayor.EntitlementType( typeof( ESRDEntitlement ) );

                FormCanTransition();

                if( (bool) Tag == true )
                {
                    formWasChanged = true;
                    Tag = false;
                    parentForm.ClearLinkLabels();
                }
            }
        }

        private void rbDisability_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;

            if( rb.Checked )
            {
                Model_MedicareSecondaryPayor.EntitlementType( typeof( DisabilityEntitlement ) );

                FormCanTransition();

                if( (bool) Tag == true )
                {
                    formWasChanged = true;
                    Tag = false;
                    parentForm.ClearLinkLabels();
                }
            }
        }

        private void rbAge_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;

            if( rb.Checked )
            {
                Model_MedicareSecondaryPayor.EntitlementType( typeof( AgeEntitlement ) );

                FormCanTransition();

                if( (bool) Tag == true )
                {
                    formWasChanged = true;
                    Tag = false;
                    parentForm.ClearLinkLabels();
                }
            }
        }
        #endregion

        #region Methods
        public override void UpdateView()
        {
            SendMessage( parentForm.Handle, CONTINUE_BUTTON_DISABLED, IntPtr.Zero, IntPtr.Zero );
            SendMessage( parentForm.Handle, NEXT_BUTTON_DISABLED, IntPtr.Zero, IntPtr.Zero );

            if( (bool) Tag == true && FormChanged )
            {   // User went back and made a change
                ResetView();
            }
            else if( formActivating )
            {
                ResetView();

                if( Model_Account.MedicareSecondaryPayor.MedicareEntitlement == null )
                {
                    return;
                }
                else if( Model_Account.MedicareSecondaryPayor.MedicareEntitlement.GetType() ==
                    typeof( ESRDEntitlement ) )
                {
                    rbESRD.Checked = true;
                }
                else if( Model_Account.MedicareSecondaryPayor.MedicareEntitlement.GetType() ==
                    typeof( DisabilityEntitlement ) )
                {
                    rbDisability.Checked = true;
                }
                else if( Model_Account.MedicareSecondaryPayor.MedicareEntitlement.GetType() ==
                    typeof( AgeEntitlement ) )
                {
                    rbAge.Checked = true;
                }
            }
            FormCanTransition();
        }
        #endregion

        #region Properties
        [Browsable(false)]
        private MedicareSecondaryPayor Model_MedicareSecondaryPayor
        {
            get
            {
                return (MedicareSecondaryPayor) this.Model;
            }
        }

        [Browsable(false)]
        public Account Model_Account
        {
            private get
            {
                return (Account) this.i_account;
            }
            set
            {
                i_account = value;
            }
        }

        [Browsable(false)]
        public bool FormChanged
        {
            get
            {
                return formWasChanged;
            }
            set
            {
                formWasChanged = value;
            }
        }

        [Browsable(false)]
        public int Response
        {
            get
            {
                return response;
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Determines if the form responses are complete enough to allow the form to transition.
        /// </summary>
        private void FormCanTransition()
        {
            bool result = false;

            if( rbESRD.Checked )
            {
                response = MSPEventCode.ESRDStimulus();
                result   = true;
            }
            else if( rbDisability.Checked )
            {
                response = MSPEventCode.DisabilityStimulus();
                result   = true;
            }
            else if( rbAge.Checked )
            {
                response = MSPEventCode.AgeStimulus();
                result   = true;
            }
            if( result )
            {
                SendMessage( parentForm.Handle, NEXT_BUTTON_ENABLED, IntPtr.Zero, IntPtr.Zero );
                SendMessage( parentForm.Handle, NEXT_BUTTON_FOCUS, IntPtr.Zero, IntPtr.Zero );
            }
            else
            {
                SendMessage( parentForm.Handle, NEXT_BUTTON_DISABLED, IntPtr.Zero, IntPtr.Zero );
            }
        }

        private void ResetView()
        {
            rbDisability.Checked = false;
            rbAge.Checked = false;
            rbESRD.Checked = false;
            formActivating = false;
        }
        #endregion

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.rbDisability = new System.Windows.Forms.RadioButton();
            this.rbESRD = new System.Windows.Forms.RadioButton();
            this.lblQuestion = new System.Windows.Forms.Label();
            this.rbAge = new System.Windows.Forms.RadioButton();
            this.lblTitle = new PatientAccess.UI.CommonControls.NonSelectableTextBox();
            this.lblInstructions = new System.Windows.Forms.Label();
            this.groupBox = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.groupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rbDisability);
            this.panel1.Controls.Add(this.rbESRD);
            this.panel1.Controls.Add(this.lblQuestion);
            this.panel1.Controls.Add(this.rbAge);
            this.panel1.Location = new System.Drawing.Point(16, 88);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(620, 24);
            this.panel1.TabIndex = 1;
            this.panel1.TabStop = true;
            // 
            // rbDisability
            // 
            this.rbDisability.Location = new System.Drawing.Point(487, 0);
            this.rbDisability.Name = "rbDisability";
            this.rbDisability.Size = new System.Drawing.Size(68, 24);
            this.rbDisability.TabIndex = 2;
            this.rbDisability.TabStop = true;
            this.rbDisability.Text = "Disability";
            this.rbDisability.CheckedChanged += new System.EventHandler(this.rbDisability_CheckedChanged);
            // 
            // rbESRD
            // 
            this.rbESRD.Location = new System.Drawing.Point(408, 0);
            this.rbESRD.Name = "rbESRD";
            this.rbESRD.Size = new System.Drawing.Size(60, 24);
            this.rbESRD.TabIndex = 1;
            this.rbESRD.TabStop = true;
            this.rbESRD.Text = "ESRD";
            this.rbESRD.CheckedChanged += new System.EventHandler(this.rbESRD_CheckedChanged);
            // 
            // lblQuestion
            // 
            this.lblQuestion.Location = new System.Drawing.Point(0, 0);
            this.lblQuestion.Name = "lblQuestion";
            this.lblQuestion.Size = new System.Drawing.Size(232, 23);
            this.lblQuestion.TabIndex = 0;
            this.lblQuestion.Text = "1. Are you entitled to Medicare based on:";
            // 
            // rbAge
            // 
            this.rbAge.Location = new System.Drawing.Point(563, 0);
            this.rbAge.Name = "rbAge";
            this.rbAge.Size = new System.Drawing.Size(50, 24);
            this.rbAge.TabIndex = 3;
            this.rbAge.TabStop = true;
            this.rbAge.Text = "Age";
            this.rbAge.CheckedChanged += new System.EventHandler(this.rbAge_CheckedChanged);
            // 
            // lblTitle
            // 
            this.lblTitle.BackColor = System.Drawing.Color.White;
            this.lblTitle.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(16, 16);
            this.lblTitle.Multiline = true;
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.ReadOnly = true;
            this.lblTitle.Size = new System.Drawing.Size(169, 23);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Medicare Entitlement";
            // 
            // lblInstructions
            // 
            this.lblInstructions.Location = new System.Drawing.Point(16, 60);
            this.lblInstructions.Name = "lblInstructions";
            this.lblInstructions.Size = new System.Drawing.Size(200, 23);
            this.lblInstructions.TabIndex = 0;
            this.lblInstructions.Text = "Choose only one entitlement option.";
            // 
            // groupBox
            // 
            this.groupBox.Controls.Add(this.label5);
            this.groupBox.Controls.Add(this.label4);
            this.groupBox.Controls.Add(this.label3);
            this.groupBox.Controls.Add(this.label2);
            this.groupBox.Controls.Add(this.label1);
            this.groupBox.Location = new System.Drawing.Point(32, 160);
            this.groupBox.Name = "groupBox";
            this.groupBox.Size = new System.Drawing.Size(581, 168);
            this.groupBox.TabIndex = 0;
            this.groupBox.TabStop = false;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(16, 140);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(555, 23);
            this.label5.TabIndex = 0;
            this.label5.Text = "If the patient\'s ESRD entitlement began before age entitlement OR disability enti" +
                "tlement, choose ESRD.";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(16, 100);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(555, 32);
            this.label4.TabIndex = 0;
            this.label4.Text = "If the patient\'s entitlement by disability began before he or she became entitled" +
                " to Medicare by ESRD, choose Disability.";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(16, 72);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(555, 23);
            this.label3.TabIndex = 0;
            this.label3.Text = "If the patient\'s entitlement by age began before he or she became entitled to Med" +
                "icare by ESRD, choose Age.";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(16, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(512, 23);
            this.label2.TabIndex = 0;
            this.label2.Text = "The earliest entitlement date dictates which entitlement you should choose.";
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.label1.Location = new System.Drawing.Point(16, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(352, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "What should I choose if the patient has dual entitlement?";
            // 
            // MedicareEntitlementView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.groupBox);
            this.Controls.Add(this.lblInstructions);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lblTitle);
            this.Name = "MedicareEntitlementView";
            this.Size = new System.Drawing.Size(680, 520);
            this.panel1.ResumeLayout(false);
            this.groupBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        #region Construction
        public MedicareEntitlementView( MSPDialog form )
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
            EnableThemesOn( this );
            parentForm = form;
            formActivating = true;  // Used in setting radio button states
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

        private GroupBox groupBox;

        private RadioButton            rbDisability;
        private RadioButton            rbAge;
        private RadioButton            rbESRD;

        private Label                  lblQuestion;
        private Label                  label1;
        private Label                  label2;
        private Label                  label3;
        private Label                  label4;
        private Label                  label5;
        private Label                  lblInstructions;

        private Panel                  panel1;

        private NonSelectableTextBox lblTitle;

        private Account                                     i_account;
        private MSPDialog                                   parentForm;
        private static bool                                  formWasChanged;
        private bool                                        formActivating;
        private int                                         response;
        #endregion

        #region Constants
        const Int32                                         WM_USER                  = 0x400;
        const Int32                                         CONTINUE_BUTTON_DISABLED = WM_USER + 1;
        const Int32                                         CONTINUE_BUTTON_ENABLED  = WM_USER + 2;
        const Int32                                         CONTINUE_BUTTON_FOCUS    = WM_USER + 3;
        const Int32                                         NEXT_BUTTON_DISABLED     = WM_USER + 4;
        const Int32                                         NEXT_BUTTON_ENABLED      = WM_USER + 5;
        const Int32                                         NEXT_BUTTON_FOCUS        = WM_USER + 6;
        #endregion
    }
}
