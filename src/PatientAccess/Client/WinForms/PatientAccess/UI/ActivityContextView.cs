using System;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain;

namespace PatientAccess.UI
{
	/// <summary>
	/// Summary description for ActivityContextView.
	/// </summary>
	[Serializable]
	public class ActivityContextView : ControlView
	{
		#region Events
		public event EventHandler FilterOff;
		public event EventHandler FilterOn;
		#endregion

		#region Event Handlers
		private void rbOn_CheckedChanged(object sender, EventArgs e)
		{
			if( FilterOn != null ) //&& rbOn.Checked )
			{
				FilterOn( sender, e );
				lblDescriptionTxt.Text = filterDescription;
			}
		}

		private void rbOff_CheckedChanged(object sender, EventArgs e)
		{
			if( FilterOff != null ) // && rbOff.Checked )
			{
				lblDescriptionTxt.Text = "No filter";
				FilterOff( sender, e );
			}
		}
		#endregion

		#region Methods

		public override void UpdateView()
		{
			if( Model != null )
			{
				lblActivityTxt.Text = CurrentActivity.ContextDescription;
				SetDefaultFilterAndText();
			}
		}

		#endregion

		#region Properties
		public bool RadioButtonsEnabled
		{
			get
			{
				return rbOn.Enabled;
			}
		}

		public Activity CurrentActivity
		{
		    private get
			{
				return (Activity)Model;
			}
			set
			{
				Model = value;
			}
		}

        public bool OnRadioButtonChecked
        {
            get
            {
                return rbOn.Checked;
            }
        }

        public bool OffRadioButtonChecked
        {
            get
            {
                return rbOff.Checked;
            }
        }

		#endregion

		#region Private Methods
		private void SetFilterRadioButtons( bool filterEnabled)
		{
			rbOff.Enabled = filterEnabled;
			rbOn.Enabled = filterEnabled;

			if( filterEnabled )
			{
                // force check changed to fire if it was initialized to true

                if( rbOn.Checked )
                {
                    rbOn.CheckedChanged -= rbOn_CheckedChanged;
                    rbOn.Checked = false;
                    rbOn.CheckedChanged += rbOn_CheckedChanged;
                }
				rbOn.Checked = true;
			}
			else
			{
                // force check changed to fire if it was initialized to true

                if (rbOff.Checked)
                {
                    rbOff.CheckedChanged -= rbOff_CheckedChanged;
                    rbOff.Checked = false;
                    rbOff.CheckedChanged += rbOff_CheckedChanged;
                }
				rbOff.Checked = true;
			}
		}

		private void SetDefaultFilterAndText()
		{
			filterDescription = "No filter";

			switch( CurrentActivity.GetType().Name )
			{
				case "PreRegistrationActivity":
					SetFilterRadioButtons( false );
					break;
                case "QuickAccountCreationActivity":
                    SetFilterRadioButtons( false );
                    break;
				case "RegistrationActivity":
					SetFilterRadioButtons( false );
					break;
                case "ShortPreRegistrationActivity":
                    SetFilterRadioButtons( false );
                    break;
                case "ShortRegistrationActivity":
                    SetFilterRadioButtons( false );
                    break;
                case "PreMSERegisterActivity":
					SetFilterRadioButtons( false );
					break;
				case "PostMSERegistrationActivity":
					filterDescription = "Accounts with pre-MSE ED registration";
					SetFilterRadioButtons( true );
					break;
                case "PreAdmitNewbornActivity":
                    filterDescription = "Accounts for newborn delivery";
                    SetFilterRadioButtons( true );
                    break;
				case "AdmitNewbornActivity":
					filterDescription = "Accounts for newborn delivery";
					SetFilterRadioButtons( true );
					break;
				case "MaintenanceActivity":
					SetFilterRadioButtons( false );
					break;
				case "CancelPreRegActivity":
					filterDescription = "Preregistered accounts";
					SetFilterRadioButtons( true );
					break;
				case "PreDischargeActivity":
				case "DischargeActivity":
					filterDescription = "Accounts eligible for discharge";
					SetFilterRadioButtons( true );
					break;
				case "EditDischargeDataActivity":
					filterDescription = "Discharged accounts";
					SetFilterRadioButtons( true );
					break;
				case "EditRecurringDischargeActivity":
					filterDescription = "Recurring outpatient accounts";
					SetFilterRadioButtons( true );
					break;
				case "CancelInpatientDischargeActivity":
					filterDescription = "Discharged inpatient accounts";
					SetFilterRadioButtons( true );
					break;
				case "CancelOutpatientDischargeActivity":
					filterDescription = "Discharged outpatient accounts";
					SetFilterRadioButtons( true );
					break;
				case "TransferActivity":
					filterDescription = "Accounts eligible for location transfer";
					SetFilterRadioButtons( true );
					break;
				case "TransferOutToInActivity":
					filterDescription = "Outpatient accounts eligible for transfer to inpatient";
					SetFilterRadioButtons( true );
					break;
				case "TransferInToOutActivity":
					filterDescription = "Inpatient accounts eligible for transfer to outpatient";
					SetFilterRadioButtons( true );
					break;
				case "CancelInpatientStatusActivity":
					filterDescription = "Inpatient accounts eligible for cancelling inpatient status";
					SetFilterRadioButtons( true );
					break;
                case Activity.TransferErPatientToOutPatient:
                    filterDescription = ELIGIBLE_ERPATIENT_ACCOUNTS;
                    SetFilterRadioButtons(true);
                    break;
                case Activity.TransferOutPatientToErPatient:
                    filterDescription = ELIGIBLE_OUTPATIENTS_ACCOUNTS;
                    SetFilterRadioButtons(true);
                    break;
                case "PAIWalkinOutpatientCreationActivity":
                    SetFilterRadioButtons(false);
                    break;
                case "UCCPreMSERegistrationActivity":
                    SetFilterRadioButtons(false);
                    break;
                case "UCCPostMseRegistrationActivity":
                    filterDescription = "Accounts with Urgent Care Pre-MSE";
                    SetFilterRadioButtons(true);
                    break;
				default:
					break;
			}

			lblDescriptionTxt.Text = filterDescription;
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.label1 = new System.Windows.Forms.Label();
            this.lblActivityTxt = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.rbOn = new System.Windows.Forms.RadioButton();
            this.rbOff = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.lblDescriptionTxt = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( ( byte )( 0 ) ) );
            this.label1.Location = new System.Drawing.Point( 3, 6 );
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size( 52, 18 );
            this.label1.TabIndex = 0;
            this.label1.Text = "Activity:";
            // 
            // lblActivityTxt
            // 
            this.lblActivityTxt.Location = new System.Drawing.Point( 54, 6 );
            this.lblActivityTxt.Name = "lblActivityTxt";
            this.lblActivityTxt.Size = new System.Drawing.Size( 234, 15 );
            this.lblActivityTxt.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( ( byte )( 0 ) ) );
            this.label3.Location = new System.Drawing.Point( 312, 6 );
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size( 73, 15 );
            this.label3.TabIndex = 2;
            this.label3.Text = "Filter status:";
            // 
            // rbOn
            // 
            this.rbOn.Location = new System.Drawing.Point( 385, 1 );
            this.rbOn.Name = "rbOn";
            this.rbOn.Size = new System.Drawing.Size( 40, 24 );
            this.rbOn.TabIndex = 3;
            this.rbOn.Text = "On";
            this.rbOn.CheckedChanged += new System.EventHandler( this.rbOn_CheckedChanged );
            // 
            // rbOff
            // 
            this.rbOff.Location = new System.Drawing.Point( 425, 1 );
            this.rbOff.Name = "rbOff";
            this.rbOff.Size = new System.Drawing.Size( 38, 24 );
            this.rbOff.TabIndex = 4;
            this.rbOff.Text = "Off";
            this.rbOff.CheckedChanged += new System.EventHandler( this.rbOff_CheckedChanged );
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( ( byte )( 0 ) ) );
            this.label4.Location = new System.Drawing.Point( 471, 6 );
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size( 100, 14 );
            this.label4.TabIndex = 5;
            this.label4.Text = "Description:";
            // 
            // lblDescriptionTxt
            // 
            this.lblDescriptionTxt.Location = new System.Drawing.Point( 542, 6 );
            this.lblDescriptionTxt.Name = "lblDescriptionTxt";
            this.lblDescriptionTxt.Size = new System.Drawing.Size( 267, 15 );
            this.lblDescriptionTxt.TabIndex = 6;
            this.lblDescriptionTxt.Text = "Outpatient accounts eligible for transfer to inpatient";
            // 
            // ActivityContextView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add( this.lblDescriptionTxt );
            this.Controls.Add( this.label4 );
            this.Controls.Add( this.rbOff );
            this.Controls.Add( this.rbOn );
            this.Controls.Add( this.label3 );
            this.Controls.Add( this.lblActivityTxt );
            this.Controls.Add( this.label1 );
            this.Name = "ActivityContextView";
            this.Size = new System.Drawing.Size( 979, 31 );
            this.ResumeLayout( false );
		}
		#endregion
		#endregion

		#region Private Properties
		#endregion

		#region Construction and Finalization
		public ActivityContextView()
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
		private Container components = null;
		private Label label3;
		private Label label4;
		private Label lblActivityTxt;
		private RadioButton rbOn;
		private RadioButton rbOff;
		private Label lblDescriptionTxt;
		private Label label1;
		private string filterDescription = String.Empty;
		#endregion

		#region Constants

	    private const string ELIGIBLE_ERPATIENT_ACCOUNTS = "ER accounts eligible for transfer to outpatient";
	    private const string ELIGIBLE_OUTPATIENTS_ACCOUNTS = "Outpatient accounts eligible for transfer to ER";

	    #endregion
	}
}
