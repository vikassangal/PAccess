using System;
using System.Collections;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.EmploymentViews;

namespace PatientAccess.UI.GuarantorViews
{
    /// <summary>
    /// Summary description for EmploymentView.
    /// </summary>
    [Serializable]
    public class EmploymentView : ControlView, IEmploymentView
    {
        #region Events
        public event EventHandler EmploymentViewChangedEvent;
        public event CancelEventHandler EmploymentStatusForGuarantorValidating;
        #endregion

        #region Event Handlers

        private void button_EditEmployer_Click( object sender, EventArgs e )
        {
            OnEditEmployer();

            if( EmploymentViewChangedEvent != null )
            {
                EmploymentViewChangedEvent(this, null);
            }
        }

        private void OnEditEmployer()
        {
			UIColors.SetNormalBgColor(lblAddress);

            FormSelectEmployer formSelectEmployer = new FormSelectEmployer();
            formSelectEmployer.Activity = Activity;
			try
			{
				formSelectEmployer.ShowDialog( this );

				if( formSelectEmployer.DialogResult == DialogResult.OK )
				{
					if( formSelectEmployer.Model_Employer != null )
					{
						Model_Employment.Employer = formSelectEmployer.Model_Employer;
					}
					else
					{
						Model_Employment.Employer = formSelectEmployer.SelectedEmployer;
					}

					if( formSelectEmployer.SelectedContactPoint.Address.Address1.Length > 0 )
					{
						Model_Employment.Employer.PartyContactPoint = formSelectEmployer.SelectedContactPoint;
					}
					else if( formSelectEmployer.SelectedEmployerContactPoint.Address.Address1.Length > 0 )
					{
						Model_Employment.Employer.PartyContactPoint = 
							formSelectEmployer.SelectedEmployerContactPoint;
					}
				}
			}
			finally
			{
                UpdateView();
				formSelectEmployer.Dispose();
			}
        }

        private void comboBox_EmploymentStatus_SelectedIndexChanged( object sender, EventArgs e )
        {
            OnEmploymentStatusSelected();

            UpdateModel();

            if( EmploymentViewChangedEvent != null )
            {
                EmploymentViewChangedEvent(this, null);
            }
        }

        private void comboBox_EmploymentStatus_Validating(object sender, CancelEventArgs e)
        { 
            EmploymentStatus es = comboBox_EmploymentStatus.SelectedItem as EmploymentStatus;

            if( es == null 
                || es.Description == NOT_EMPLOYED
                || es.Description == string.Empty)
            {
                return;
            }

            if( priorEmploymentStatus != null
                && es.Code == priorEmploymentStatus.Code)
            {
                return;
            }

            priorEmploymentStatus = es;
                            
            if( EmploymentStatusForGuarantorValidating != null )
            {
                EmploymentStatusForGuarantorValidating( this, null );        
            }  

            OnEditEmployer();
        }


        private void UpdateModelEvent( object sender, EventArgs e )
        {
            UpdateModel();
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            phoneNumberControl.ClearText();
            
            textBoxOccIndustry.ResetText();

			lblAddress.Text = String.Empty;

			Model_Employment.Employer = new Employer();
            Model_Employment.Status = comboBox_EmploymentStatus.SelectedItem as EmploymentStatus;

			if( EmploymentViewChangedEvent != null )
			{
				EmploymentViewChangedEvent(this, null);
			}
        }

        #endregion

        #region Methods
        public void ResetView()
        {
            Model_Employment.PhoneNumber = new PhoneNumber();
            Model_Employment.Occupation = String.Empty;
            Model_Employment.Status = new EmploymentStatus();
            Model_Employment.Employer = new Employer();
                        
            UpdateView();
        }

        public override void UpdateView()
        {
            if( !UpdatingView )
            {
                try
                {
                    UpdatingView = true;
                    InitializeEmploymentStatus();

                    if( Model_Employment != null )
                    {
						if( Model_Employment.Employer != null &&
							Model_Employment.Employer.PartyContactPoint != null )
						{
                            phoneNumberControl.Model = Model_Employment.Employer.PartyContactPoint.PhoneNumber;
						}

                        textBoxOccIndustry.Text = Model_Employment.Occupation;

						lblAddress.Text = BuildEmploymentAddress();

                        if( Model_Employment.Status != null )
                        {
                            priorEmploymentStatus = Model_Employment.Status;

                            comboBox_EmploymentStatus.SelectedItem = Model_Employment.Status;
                            
                        }
                        employmentViewPresenter.SetControlStatesOnEmploymentStatus();
                    }
                }
                finally
                {
                    UpdatingView = false;
                    
                    if( comboBox_EmploymentStatus.Items.Count > 0 && comboBox_EmploymentStatus.SelectedIndex < 0 )
                    {
                        comboBox_EmploymentStatus.SelectedIndex = 0;
                    }                    
                }
            }
        }

        public override void UpdateModel()
        {
            if( !UpdatingView )
            {
                Model_Employment.Occupation = textBoxOccIndustry.Text;

				if( phoneNumberControl.Model == null )
				{
						Model_Employment.Employer.PartyContactPoint.PhoneNumber = new PhoneNumber();
				}

                Model_Employment.Status = comboBox_EmploymentStatus.SelectedItem as EmploymentStatus;
            }
        }

        public void SetControlState(bool state)
        {
            button_EditEmployer.Enabled = state;
            phoneNumberControl.ToggleEnabled(state);
            textBoxOccIndustry.Enabled = state;
        }

        public void EnableClearButton(bool state)
        {
            buttonClear.Enabled = state;
        }

        #endregion

        #region Properties

        public PhoneNumberControl PhoneNumberControl
        {
            get
            {
                return phoneNumberControl;
            }
        }

        public object EmployerField
        {
            get
            {
				return lblAddress;
            }
        }

        public PatientAccessComboBox ComboBox
        {
            get
            {
                return comboBox_EmploymentStatus;
            }
        }

        public Activity Activity
        {
            private get
            {
                return i_Activity;
            }
            set
            {
                i_Activity = value;
            }
        }

        #endregion

        #region Private Methods

        private void InitializeEmploymentStatus()
        {
            IEmploymentStatusBroker broker = new EmploymentStatusBrokerProxy() ;
            ICollection employmentStatuses = broker.AllTypesOfEmploymentStatuses( User.GetCurrent().Facility.Oid );

            comboBox_EmploymentStatus.Items.Clear();

            foreach( EmploymentStatus employmentStatus in employmentStatuses )
            {
                comboBox_EmploymentStatus.Items.Add( employmentStatus );
            }
        }

        private string BuildEmploymentAddress()
        {
			Address addr = null;
            StringBuilder msg = new StringBuilder();

            if( Model_Employment.Employer == null )
            {
                return string.Empty;
            }

			if( Model_Employment.Employer != null &&
				Model_Employment.Employer.PartyContactPoint != null && 
				Model_Employment.Employer.PartyContactPoint.Address != null )
			{
				addr = Model_Employment.Employer.PartyContactPoint.Address;
			}
            
            if( Model_Employment.Employer != null )
            {
                msg.Append(Model_Employment.Employer.Name);
            }

            if( addr != null )
            {
                if( addr.Address1 != null && addr.Address1.Length > 1 )
                {
                    msg.Append(NEWLINE_TAG);
                    msg.Append( addr.Address1 );
                }

                if( addr.Address2 != null && addr.Address2.Length > 1 )
                {
                    msg.Append( NEWLINE_TAG );
                    msg.Append( addr.Address2 );
                }

                if( (addr.City.Length + addr.State.PrintString.Length + addr.ZipCode.PostalCode.Length) > 0 )
                {
                    if( addr.City.Length > 0 )
                    {
                        msg.Append( NEWLINE_TAG );
                        msg.Append( addr.City );
                    }

                    if( addr.State.PrintString.Length > 0 )
                    {
                        msg.Append( CITYSTATE_DELIMITER );
                        msg.Append( addr.State );
                    }

                    if( addr.ZipCode.PostalCode.Length > 0 )
                    {
                        msg.Append( SPACE_DELIMITER );
                        msg.Append( addr.ZipCode.ZipCodePrimary + addr.ZipCode.ZipCodeExtended );
                    }
                }

                if( addr.Country != null && addr.Country.PrintString.Length > 0 )
                {
                    msg.Append( NEWLINE_TAG );
                    msg.Append( addr.Country.PrintString );
                }
            }

            return msg.ToString();
        }

        private void OnEmploymentStatusSelected()
        {
            if( !UpdatingView )
            {
                object obj = comboBox_EmploymentStatus.SelectedItem;
                if( obj is EmploymentStatus )
                {
                    // TLG 08/02/2006 CR0371 - Employer name defaulted to Employer Status if the status is:
                    //   a. Retired
                    //   b. Not-Employed
                    //   c. Self-Employed

                    EmploymentStatus selectedStatus = (EmploymentStatus)obj;

                    if( // selectedStatus.Code == string.Empty  ||  tlg 09/07/2006 
                        lblAddress.Text.Replace("-"," ").Replace("\r\n",string.Empty) == EmploymentStatus.NewNotEmployed().Description.Replace("-"," ")
                        || lblAddress.Text.Replace("-"," ").Replace("\r\n",string.Empty) == EmploymentStatus.NewRetired().Description.Replace("-"," ")
                        || lblAddress.Text.Replace("-"," ").Replace("\r\n",string.Empty) == EmploymentStatus.NewSelfEmployed().Description.Replace("-"," ") )
                    {
                        Model_Employment.Employer = new Employer();
                        lblAddress.Text = string.Empty;
                    }

                    if( selectedStatus != null )
                    {
                        if( selectedStatus.Code == EmploymentStatus.NOT_EMPLOYED_CODE )
                        {
                            Model_Employment.Employer = new Employer();
                            Model_Employment.Employer.Name = selectedStatus.Description;
                            lblAddress.Text = string.Empty;
                            lblAddress.Text = selectedStatus.Description;
                            phoneNumberControl.ClearText();
                            textBoxOccIndustry.Text = string.Empty;
                        } 
                        else if( selectedStatus.Code == EmploymentStatus.RETIRED_CODE )
                        {
                            Model_Employment.Employer = new Employer();
                            Model_Employment.Employer.Name = selectedStatus.Description;
                            lblAddress.Text = string.Empty;                            
                            lblAddress.Text = selectedStatus.Description;
                            phoneNumberControl.ClearText();
                            textBoxOccIndustry.Text = string.Empty;
                        }
                        else if( selectedStatus.Code == EmploymentStatus.SELF_EMPLOYED_CODE )
                        {
                            Model_Employment.Employer = new Employer();
                            Model_Employment.Employer.Name = selectedStatus.Description;
                            lblAddress.Text = string.Empty;
                            lblAddress.Text = selectedStatus.Description;
                            phoneNumberControl.ClearText();
                            textBoxOccIndustry.Text = string.Empty;
                        }

                        if( Model_Employment != null &&
                            Model_Employment.Status != null )
                        {
                            Model_Employment.Status = selectedStatus;
                        }
                    }
                    else
                    {
                        if (Model_Employment != null)
                        {
                            Model_Employment.Status = null;

                            if (Model_Employment.Employer.PartyContactPoint.Address != null)
                            {
                                Model_Employment.Employer.PartyContactPoint.Address = null;
                            }
                        }
                    }
                    employmentViewPresenter.SetControlStatesOnEmploymentStatus();
                }
            }
        }

        #endregion

        #region Private Properties

        public Employment Model_Employment
        {
            get
            {
                if( Model != null )
                {
                    return ( Employment )Model;
                }
                
                return null;
            }
            set
            {
                Model = value;
            }
        }

        private bool UpdatingView
        {
            get
            {
                return i_UpdatingView;
            }
            set
            {
                i_UpdatingView = value;
            }
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( EmploymentView ) );
            this.gbEmployment = new System.Windows.Forms.GroupBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.lblAddress = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.textBoxOccIndustry = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.buttonClear = new PatientAccess.UI.CommonControls.LoggingButton();
            this.button_EditEmployer = new PatientAccess.UI.CommonControls.LoggingButton();
            this.comboBox_EmploymentStatus = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.phoneNumberControl = new PatientAccess.UI.CommonControls.PhoneNumberControl();
            this.gbEmployment.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbEmployment
            // 
            this.gbEmployment.Controls.Add( this.panel3 );
            this.gbEmployment.Controls.Add( this.panel2 );
            this.gbEmployment.Controls.Add( this.panel1 );
            this.gbEmployment.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbEmployment.Location = new System.Drawing.Point( 0, 0 );
            this.gbEmployment.Name = "gbEmployment";
            this.gbEmployment.Size = new System.Drawing.Size( 359, 320 );
            this.gbEmployment.TabIndex = 0;
            this.gbEmployment.TabStop = false;
            this.gbEmployment.Text = "Employment";
            // 
            // panel3
            // 
            this.panel3.Controls.Add( this.lblAddress );
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point( 3, 75 );
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size( 353, 182 );
            this.panel3.TabIndex = 2;
            // 
            // lblAddress
            // 
            this.lblAddress.BackColor = System.Drawing.Color.White;
            this.lblAddress.Location = new System.Drawing.Point( 7, 0 );
            this.lblAddress.Name = "lblAddress";
            this.lblAddress.Size = new System.Drawing.Size( 335, 182 );
            this.lblAddress.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Controls.Add( this.phoneNumberControl );
            this.panel2.Controls.Add( this.textBoxOccIndustry );
            this.panel2.Controls.Add( this.label3 );
            this.panel2.Controls.Add( this.label2 );
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point( 3, 257 );
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size( 353, 60 );
            this.panel2.TabIndex = 1;
            // 
            // textBoxOccIndustry
            // 
            this.textBoxOccIndustry.BackColor = System.Drawing.Color.White;
            this.textBoxOccIndustry.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBoxOccIndustry.Location = new System.Drawing.Point( 84, 32 );
            this.textBoxOccIndustry.MaxLength = 14;
            this.textBoxOccIndustry.Name = "textBoxOccIndustry";
            this.textBoxOccIndustry.Size = new System.Drawing.Size( 164, 20 );
            this.textBoxOccIndustry.TabIndex = 4;
            this.textBoxOccIndustry.Validated += new System.EventHandler( this.UpdateModelEvent );
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point( 9, 35 );
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size( 75, 15 );
            this.label3.TabIndex = 1;
            this.label3.Text = "Occ/Industry:";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point( 11, 9 );
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size( 75, 15 );
            this.label2.TabIndex = 0;
            this.label2.Text = "Phone:";
            // 
            // panel1
            // 
            this.panel1.Controls.Add( this.buttonClear );
            this.panel1.Controls.Add( this.button_EditEmployer );
            this.panel1.Controls.Add( this.comboBox_EmploymentStatus );
            this.panel1.Controls.Add( this.label1 );
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point( 3, 16 );
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size( 353, 59 );
            this.panel1.TabIndex = 0;
            // 
            // buttonClear
            // 
            this.buttonClear.Location = new System.Drawing.Point( 157, 30 );
            this.buttonClear.Message = null;
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size( 64, 23 );
            this.buttonClear.TabIndex = 3;
            this.buttonClear.Text = "Clear";
            this.buttonClear.Click += new System.EventHandler( this.buttonClear_Click );
            // 
            // button_EditEmployer
            // 
            this.button_EditEmployer.Location = new System.Drawing.Point( 50, 30 );
            this.button_EditEmployer.Message = null;
            this.button_EditEmployer.Name = "button_EditEmployer";
            this.button_EditEmployer.Size = new System.Drawing.Size( 99, 23 );
            this.button_EditEmployer.TabIndex = 2;
            this.button_EditEmployer.Text = "Select E&mployer...";
            this.button_EditEmployer.Click += new System.EventHandler( this.button_EditEmployer_Click );
            // 
            // comboBox_EmploymentStatus
            // 
            this.comboBox_EmploymentStatus.BackColor = System.Drawing.Color.White;
            this.comboBox_EmploymentStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_EmploymentStatus.Location = new System.Drawing.Point( 50, 4 );
            this.comboBox_EmploymentStatus.Name = "comboBox_EmploymentStatus";
            this.comboBox_EmploymentStatus.Size = new System.Drawing.Size( 187, 21 );
            this.comboBox_EmploymentStatus.TabIndex = 1;
            this.comboBox_EmploymentStatus.Validating += new System.ComponentModel.CancelEventHandler( this.comboBox_EmploymentStatus_Validating );
            this.comboBox_EmploymentStatus.SelectedIndexChanged += new System.EventHandler( this.comboBox_EmploymentStatus_SelectedIndexChanged );
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point( 11, 7 );
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size( 52, 15 );
            this.label1.TabIndex = 2;
            this.label1.Text = "Status:";
            // 
            // phoneNumberControl
            // 
            this.phoneNumberControl.AreaCode = "";
            this.phoneNumberControl.Location = new System.Drawing.Point( 81, 4 );
            this.phoneNumberControl.Model = ( (PatientAccess.Domain.Parties.PhoneNumber)( resources.GetObject( "phoneNumberControl.Model" ) ) );
            this.phoneNumberControl.Name = "phoneNumberControl";
            this.phoneNumberControl.PhoneNumber = "";
            this.phoneNumberControl.Size = new System.Drawing.Size( 94, 27 );
            this.phoneNumberControl.TabIndex = 3;
            // 
            // EmploymentView
            // 
            this.Controls.Add( this.gbEmployment );
            this.Name = "EmploymentView";
            this.Size = new System.Drawing.Size( 359, 320 );
            this.gbEmployment.ResumeLayout( false );
            this.panel3.ResumeLayout( false );
            this.panel2.ResumeLayout( false );
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout( false );
            this.ResumeLayout( false );

		}

        #endregion

        #endregion

        #region Construction and Finalization

        public EmploymentView()
        {
            employmentViewPresenter = new EmploymentViewPresenter(this);
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
            
            // Set phone number & industry fields disabled until an employment status is chosen,

            this.phoneNumberControl.ToggleEnabled( false );
            textBoxOccIndustry.Enabled = false;

            UpdatingView = false;
        }
        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
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

        private Container             components = null;

        private bool                                        i_UpdatingView;

        private LoggingButton                               button_EditEmployer;
        private LoggingButton                               buttonClear;

        private GroupBox               gbEmployment;
        
        private Panel                  panel1;
        private Panel                  panel2;
        private Panel                  panel3;
        
        private PatientAccessComboBox                       comboBox_EmploymentStatus;
        
        private Label                  label1;
        private Label                  label2;
        private Label                  label3;
        private TextBox                textBoxOccIndustry;
        private EmploymentStatus                            priorEmploymentStatus;
        private Activity                                    i_Activity;
        private PhoneNumberControl                          phoneNumberControl = null;
        private EmploymentViewPresenter                     employmentViewPresenter;
        #endregion

        #region Constants
        
        private const string NEWLINE_TAG                    = "\r\n";
        private const string CITYSTATE_DELIMITER            = ", ";
        private const string SPACE_DELIMITER                = " ";
		private Label lblAddress;
       
        private const string NOT_EMPLOYED                   = "NOT-EMPLOYED";

		#endregion

    }
    
}
