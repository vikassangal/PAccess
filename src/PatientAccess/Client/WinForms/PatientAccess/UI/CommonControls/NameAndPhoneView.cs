using System;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.UI.CommonControls
{
	/// <summary>
	/// Summary description for NameAndPhoneView.
	/// </summary>
	public class NameAndPhoneView : ControlView
	{
        #region Events

        public event EventHandler PhoneNumberEnteredEvent;

        #endregion

        #region Event Handlers

        void phoneNumberControl_Validating( object sender, CancelEventArgs e )
        {
            if( PhoneNumberEnteredEvent != null )
            {
                PhoneNumberEnteredEvent( this, new LooseArgs(
                    Model_Person.ContactPointWith(
                    TypeOfContactPoint.NewBusinessContactPointType() ).PhoneNumber ) );
            }
        }

        private void mtbName_Validating(object sender, CancelEventArgs e)
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            string namemtb = mtb.Text.TrimEnd();
            Model_Person.Name.FirstName = (String)namemtb.Clone();
          
        }

        #endregion

        #region Methods
        public override void UpdateView()
        {
            if( Model_Person.ContactPointWith( 
                TypeOfContactPoint.NewBusinessContactPointType() ) != null )
            {
                this.phoneNumberControl.Model
                    = Model_Person.ContactPointWith( TypeOfContactPoint.NewBusinessContactPointType() ).PhoneNumber;
            }

            if( Model_Person.Name != null )
            {
                this.mtbName.Text = Model_Person.FirstName;
            }
            this.mtbName_Validating( this.mtbName, null );

            this.phoneNumberControl.RunRules();
        } 
        
        public void ResetView()
        {
            this.phoneNumberControl.PhoneNumber = string.Empty;
            this.phoneNumberControl.AreaCode = string.Empty;

            this.phoneNumberControl.SetNormalColor();
            mtbName.ResetText();

			if( this.Model_Person != null &&
				this.Model_Person.Name != null)
			{
				this.Model_Person.Name.FirstName = String.Empty;

				if( Model_Person.ContactPointWith( 
					TypeOfContactPoint.NewBusinessContactPointType() ) != null )
				{
					ContactPoint cp = Model_Person.ContactPointWith( TypeOfContactPoint.NewBusinessContactPointType() );
					cp.PhoneNumber = new PhoneNumber();
				}
			}
 
            Refresh();
        }
        #endregion

        #region Properties
        [Browsable( false )]
        public Person Model_Person
        {
            set
            {
                this.Model = value;
            }
            get
            {
                return (Person)this.Model;
            }
        }
       
        public string NameLabel
        {
            get
            {
                return labelName.Text;
            }
            set
            {
                labelName.Text = value;
            }
        }
        public string PhoneLabel
        {
            get
            {
                return labelPhone.Text;
            }
            set
            {
                labelPhone.Text = value;
            }
        }
        #endregion

        #region Private Methods
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

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( NameAndPhoneView ) );
            this.labelPhone = new System.Windows.Forms.Label();
            this.mtbName = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.labelName = new System.Windows.Forms.Label();
            this.phoneNumberControl = new PatientAccess.UI.CommonControls.PhoneNumberControl();
            this.SuspendLayout();
            // 
            // labelPhone
            // 
            this.labelPhone.Location = new System.Drawing.Point( 8, 32 );
            this.labelPhone.Name = "labelPhone";
            this.labelPhone.Size = new System.Drawing.Size( 100, 16 );
            this.labelPhone.TabIndex = 0;
            this.labelPhone.Text = "Phone:";
            // 
            // mtbName
            // 
            this.mtbName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbName.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbName.KeyPressExpression = "^[a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.mtbName.Location = new System.Drawing.Point( 200, 8 );
            this.mtbName.Mask = "";
            this.mtbName.MaxLength = 35;
            this.mtbName.Name = "mtbName";
            this.mtbName.Size = new System.Drawing.Size( 256, 20 );
            this.mtbName.TabIndex = 1;
            this.mtbName.ValidationExpression = "^[a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.mtbName.Validating += new System.ComponentModel.CancelEventHandler( this.mtbName_Validating );
            // 
            // labelName
            // 
            this.labelName.Location = new System.Drawing.Point( 8, 8 );
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size( 184, 16 );
            this.labelName.TabIndex = 0;
            this.labelName.Text = "Auto/Home insurance agent name:";
            // 
            // phoneNumberControl1
            // 
            this.phoneNumberControl.AreaCode = "";
            this.phoneNumberControl.Location = new System.Drawing.Point( 197, 29 );
            this.phoneNumberControl.Model = ( (PatientAccess.Domain.Parties.PhoneNumber)( resources.GetObject( "phoneNumberControl1.Model" ) ) );
            this.phoneNumberControl.Name = "phoneNumberControl1";
            this.phoneNumberControl.PhoneNumber = "";
            this.phoneNumberControl.Size = new System.Drawing.Size( 94, 27 );
            this.phoneNumberControl.TabIndex = 2;
            this.phoneNumberControl.Validating += new CancelEventHandler( phoneNumberControl_Validating );
            // 
            // NameAndPhoneView
            // 
            this.Controls.Add( this.phoneNumberControl );
            this.Controls.Add( this.labelName );
            this.Controls.Add( this.mtbName );
            this.Controls.Add( this.labelPhone );
            this.Name = "NameAndPhoneView";
            this.Size = new System.Drawing.Size( 464, 56 );
            this.ResumeLayout( false );
            this.PerformLayout();

        }

        #endregion
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public NameAndPhoneView()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            // TODO: Add any initialization after the InitializeComponent call

        }
        #endregion

        #region Data Elements
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private Container                         components = null;
        private Label                              labelPhone;
        private MaskedEditTextBox                mtbName;
        private PhoneNumberControl                                      phoneNumberControl;
        private Label                              labelName;
        #endregion

        #region Constants
        #endregion 
	}
}
