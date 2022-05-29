using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain;

namespace PatientAccess.UI.InsuranceViews.MedicareSecondaryPayorViews
{
    /// <summary>
    /// Summary description for WelcomeScreenView.
    /// </summary>
    public class WelcomeScreenView : ControlView
    {
        [DllImport("User32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        #region Methods
        public override void UpdateView()
        {
            response = MSPEventCode.YesStimulus();
            SendMessage( parentForm.Handle, NEXT_BUTTON_ENABLED, IntPtr.Zero, IntPtr.Zero );
            SendMessage( parentForm.Handle, NEXT_BUTTON_FOCUS, IntPtr.Zero, IntPtr.Zero );
        }
        #endregion

        #region Properties
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

        #region Construction
        public WelcomeScreenView( MSPDialog form )
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
            EnableThemesOn( this );
            parentForm = form;
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.label1.Location = new System.Drawing.Point(16, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(560, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "Welcome to the Medicare Secondary Payor Form Analysis Wizard!";
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.label2.Location = new System.Drawing.Point(16, 64);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(144, 23);
            this.label2.TabIndex = 0;
            this.label2.Text = "How does it work?";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(16, 80);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(610, 27);
            this.label3.TabIndex = 0;
            this.label3.Text = "The Medicare Secondary Payor (MSP) Form Analysis wizard is designed to present on" +
                "ly those questions that are applicable to the patient\'s reason for today\'s visit" +
                ".  For patients receiving Black Lung benefits, obtain the date black lung";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(16, 136);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(610, 32);
            this.label4.TabIndex = 0;
            this.label4.Text = "When the primary payor can be predicted, the wizard will end and you will be pres" +
                "ented with a summary of reasons for the predicted primary payor, based on your r" +
                "esponse to the questions asked.";
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.label5.Location = new System.Drawing.Point(16, 176);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(360, 23);
            this.label5.TabIndex = 0;
            this.label5.Text = "What if I want to go back and change my responses?";
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(16, 200);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(352, 23);
            this.label6.TabIndex = 0;
            this.label6.Text = "This wizard allows you two methods to return to a previous screen:";
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(16, 232);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(368, 23);
            this.label7.TabIndex = 0;
            this.label7.Text = "1. The Back button allows you to return to the last screen you visited.";
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(16, 264);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(616, 23);
            this.label8.TabIndex = 0;
            this.label8.Text = "2. In the Progress Navigator area, located at the top of each set of questions, y" +
                "ou may select a previous screen in forward";
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(28, 291);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(603, 23);
            this.label10.TabIndex = 0;
            this.label10.Text = "been previously presented or questions that are not applicable (based on your res" +
                "ponse to the questions asked).";
            // 
            // label11
            // 
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.label11.Location = new System.Drawing.Point(16, 328);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(320, 23);
            this.label11.TabIndex = 0;
            this.label11.Text = "What if I\'ve chosen Medicare as a payor by mistake?";
            // 
            // label12
            // 
            this.label12.Location = new System.Drawing.Point(16, 344);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(610, 43);
            this.label12.TabIndex = 0;
            this.label12.Text = @"If while using this wizard you determine that any information presented is not correct, please cancel the wizard, make adjustments  in the Patient Access application and then restart the wizard.  Canceling the MSP Form Analysis wizard will not save your responses to the questions.  There are two methods to cancel the wizard:";
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(25, 277);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(592, 23);
            this.label9.TabIndex = 0;
            this.label9.Text = " or backward sequence order.  The Progress Navigator does not allow you to move f" +
                "orward to questions that have not";
            // 
            // label13
            // 
            this.label13.Location = new System.Drawing.Point(16, 400);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(496, 23);
            this.label13.TabIndex = 0;
            this.label13.Text = "1.  Each set of questions displays a Cancel button at the bottom of the screen.";
            // 
            // label14
            // 
            this.label14.Location = new System.Drawing.Point(16, 432);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(488, 23);
            this.label14.TabIndex = 0;
            this.label14.Text = "2.  Edit && Cancel buttons may be displayed based on certain responses.";
            // 
            // label15
            // 
            this.label15.Location = new System.Drawing.Point(16, 106);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(610, 28);
            this.label15.TabIndex = 0;
            this.label15.Text = "benefits began, even if the reason for today\'s visit is not due to Black Lung.  A" +
                "s you respond to each question the wizard will adjust and present the next appli" +
                "cable question.";
            // 
            // WelcomeScreenView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "WelcomeScreenView";
            this.Size = new System.Drawing.Size(680, 520);
            this.ResumeLayout(false);

        }
        #endregion

        #region Data Elements
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private Container     components = null;
        private Label          label1;
        private Label          label2;
        private Label          label3;
        private Label          label4;
        private Label          label5;
        private Label          label6;
        private Label          label7;
        private Label          label8;
        private Label          label9;
        private Label          label10;
        private Label          label11;
        private Label          label12;
        private Label          label13;
        private Label          label14;
        private Label          label15;

        private MSPDialog                           parentForm;
        private bool                                formWasChanged;
        private int                                 response;
        #endregion

        #region Constants
        const Int32                                 WM_USER                  = 0x400;
        const Int32                                 NEXT_BUTTON_ENABLED      = WM_USER + 5;
        const Int32                                 NEXT_BUTTON_FOCUS        = WM_USER + 6;
        #endregion

        private void label3_Click(object sender, EventArgs e)
        {
        
        }
    }
}
