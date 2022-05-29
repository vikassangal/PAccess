using System;
using System.ComponentModel;
using System.Windows.Forms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.UI.Factories;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.DischargeViews
{
	/// <summary>
	/// Summary description for DischargeIntentView.
	/// </summary>
	[Serializable]
	public class DischargeIntentView : DischargeBaseView
	{
		#region Events
		#endregion

		#region Event Handlers
		private new void btnOk_Click(object sender, EventArgs e)
		{
            Cursor = Cursors.WaitCursor;
			SetIntentToDischarge();
            Cursor = Cursors.Default;
		}
		#endregion

		#region Methods
		public override void UpdateView()
		{
			if( Model != null )
			{
				DisplayPatientContext();

				//PopulateControls
				lblPatientName.Text = Model.Patient.FormattedName;
				lblAccount.Text = Model.AccountNumber.ToString();
				lblPatientType.Text = Model.KindOfVisit.DisplayString;

                lblAdmitDate.Text = CommonFormatting.LongDateFormat( Model.AdmitDate );
                lblAdmitTime.Text = CommonFormatting.DisplayedTimeFormat( Model.AdmitDate );

				if( Model.Location != null )
				{
					lblDischargeLocation.Text = Model.Location.DisplayString;
				}

				if( Model.DischargeDate.Year.ToString() != "1" || !ValidPatientType() )
				{
                    mtbDischargeDate.UnMaskedText = CommonFormatting.MaskedDateFormat( Model.DischargeDate );
                    
                    if( Model.DischargeDate.Hour != 0 ||
                        Model.DischargeDate.Minute != 0 )
                    {
                        mtbDischargeTime.UnMaskedText = CommonFormatting.MaskedTimeFormat( Model.DischargeDate );
                    }
                    else
                    {
                        mtbDischargeTime.UnMaskedText = "";
                    }
                    
					lblInstructionalMessage.Text = UIErrorMessages.DISCHARGE_INTENT_MSG;

					DisplayDischargeDispositions();

					if( Model.DischargeDisposition != null )
					{
						int dischargeDispositionSelectedIndex = -1;
						dischargeDispositionSelectedIndex = 
							cmbDischargeDisposition.FindString( Model.DischargeDisposition.ToString() );

						if( dischargeDispositionSelectedIndex != -1 )
						{
							cmbDischargeDisposition.SelectedIndex = dischargeDispositionSelectedIndex;
						}
					}

					btnOk.Enabled = false;
				}
				else
				{
					CheckForValuables();
					CheckForRemainingActionItems();
				}

				btnCancel.Focus();
			}
		}

		#endregion

		#region Properties

        //TODO-AC needs to be removed here and at other places
		public new Account Model
		{
			private get
			{
				return base.Model;
			}
			set
			{
				base.Model = value;
			}
		}

	    private Activity CurrentActivity
        {
            get { return i_CurrentActivity ?? (i_CurrentActivity = new PreDischargeActivity()); }
            set
            {
                i_CurrentActivity = value;
            }
        }

		#endregion

		#region Private Methods

	    private void SetIntentToDischarge()
		{
			Model.DischargeStatus = DischargeStatus.PendingDischarge();
 			Model.Activity = CurrentActivity;
			panelButtons.Hide();

			panelActions.Show();
            gbScanDocuments.Visible = true;

			cmbDischargeDisposition.Enabled = false;
			mtbDischargeDate.Enabled = false;
			dtpDischargeDate.Enabled = false;
			mtbDischargeTime.Enabled = false;

			lblInstructionalMessage.Text = "Record Intent to Discharge Patient submitted for processing.";
			userContextView1.Description = "Record Intent to Discharge - Submitted";

			CheckForValuables();
			CheckForRemainingActionItems();
			SaveAccount();

			lblNextAction.Visible = true;
			lblNextActionLine.Visible = true;

            // clear any FUS notes that were manually entered so that - if the View FUS notes icon/menu option is selected,
            // the newly added notes do not show twice.

            if( ViewFactory.Instance.CreateView<PatientAccessView>().Model != null )
            {
                (( Account )ViewFactory.Instance.CreateView<PatientAccessView>().Model ).ClearFusNotes();
            }

			RemoveMaskedTextBorder();
			mtbDischargeDate.Visible = false;
			mtbDischargeTime.Visible = false;
			lblDischargeDateVal.Visible = false;
			lblDischargeTimeVal.Visible = false;
			
			btnCloseActivity.Focus();

            // OTD# 37031 fix - Disable icon/menu options for FUS notes upon confirmation
            ViewFactory.Instance.CreateView<PatientAccessView>().SetFUSNotesOptions( false );
		}

		private new void SaveAccount()
		{
			Account anAccount = Model;
			Activity currentActivity = anAccount.Activity;
			IAccountBroker broker = BrokerFactory.BrokerOfType<IAccountBroker>();
			if ( broker != null )
			{
                AccountSaveResults results = broker.Save( anAccount, currentActivity );
                results.SetResultsTo(Model);
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
			// 
			// userContextView1
			// 
			this.userContextView1.Description = "Record Intent to Discharge";
			this.userContextView1.Name = "userContextView1";
			// 
			// patientContextView1
			// 
			this.patientContextView1.Name = "patientContextView1";
			// 
			// panel2
			// 
			this.panel2.Name = "panel2";
			// 
			// panelPatientContext
			// 
			this.panelPatientContext.Name = "panelPatientContext";
			// 
			// btnCancel
			// 
			this.btnCancel.Name = "btnCancel";
			// 
			// btnOk
			// 
			this.btnOk.Name = "btnOk";
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			// 
			// panelButtons
			// 
			this.panelButtons.Name = "panelButtons";
			// 
			// lblInstructionalMessage
			// 
			this.lblInstructionalMessage.Name = "lblInstructionalMessage";
			// 
			// panelMessages
			// 
			this.panelMessages.Name = "panelMessages";
			// 
			// lblDischargeLocation
			// 
			this.lblDischargeLocation.Name = "lblDischargeLocation";
			// 
			// lblPatientName
			// 
			this.lblPatientName.Name = "lblPatientName";
			// 
			// lblAccount
			// 
			this.lblAccount.Name = "lblAccount";
			// 
			// lblPatientType
			// 
			this.lblPatientType.Name = "lblPatientType";
			// 
			// lblAdmitDate
			// 
			this.lblAdmitDate.Name = "lblAdmitDate";
			// 
			// lblAdmitTime
			// 
			this.lblAdmitTime.Name = "lblAdmitTime";
			// 
			// dtpDischargeDate
			// 
			this.dtpDischargeDate.Enabled = false;
			this.dtpDischargeDate.Name = "dtpDischargeDate";
			this.dtpDischargeDate.TabStop = false;
			// 
			// mtbDischargeDate
			// 
			this.mtbDischargeDate.Enabled = false;
			this.mtbDischargeDate.MaxLength = 10;
			this.mtbDischargeDate.Name = "mtbDischargeDate";
			// 
			// cmbDischargeDisposition
			// 
			this.cmbDischargeDisposition.Enabled = false;
			this.cmbDischargeDisposition.Name = "cmbDischargeDisposition";
			this.cmbDischargeDisposition.Size = new System.Drawing.Size(208, 21);
			// 
			// label2
			// 
			this.label2.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label2.Name = "label2";
			// 
			// label55
			// 
			this.label55.Name = "label55";
			// 
			// lblOutstandingActionItemsMsg
			// 
			this.lblOutstandingActionItemsMsg.Name = "lblOutstandingActionItemsMsg";
			// 
			// btnCloseActivity
			// 
			this.btnCloseActivity.Name = "btnCloseActivity";
			// 
			// btnRepeatActivity
			// 
			this.btnRepeatActivity.Name = "btnRepeatActivity";
			// 
			// btnEditAccount
			// 
			this.btnEditAccount.Name = "btnEditAccount";
			// 
			// lblMessages
			// 
			this.lblMessages.Name = "lblMessages";
			// 
			// mtbDischargeTime
			// 
			this.mtbDischargeTime.Name = "mtbDischargeTime";
			this.mtbDischargeTime.Enabled = false;
			// 
			// lblCurOccupant
			// 
			this.lblCurOccupant.Name = "lblCurOccupant";
			this.lblCurOccupant.Visible = false;
			// 
			// lblCurrentOccupant
			// 
			this.lblCurrentOccupant.Name = "lblCurrentOccupant";
			this.lblCurrentOccupant.Visible = false;
			// 
			// panelActions
			// 
			this.panelActions.Name = "panelActions";
			// 
			// DischargeIntentView
			// 
			this.AcceptButton = this.btnOk;
			this.Name = "DischargeIntentView";

		}
		#endregion

		#region Private Properties
		#endregion

		#region Construction and Finalization
		public DischargeIntentView()
		{
            CurrentActivity = new PreDischargeActivity();
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			EnableThemesOn( this );
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
		private Container                 components = null;
        private Activity                                        i_CurrentActivity;
		#endregion

		#region Constants
		#endregion
	}
}
