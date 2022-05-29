using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain.Parties;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.CommonControls
{
    public class ContactView : ControlView
    {
        #region Events
        public event EventHandler PhoneNumberEnteredEvent;
        #endregion

        #region Event Handlers
        private void maskPhoneNumber_Validating(object sender, CancelEventArgs e)
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            UIColors.SetNormalBgColor( mtb );
            Refresh();

            if( mtb.UnMaskedText == String.Empty )
            {
                Model_PhoneNumber = new PhoneNumber();

                if( PhoneNumberEnteredEvent != null )
                {
                    PhoneNumberEnteredEvent( this, new LooseArgs( Model_PhoneNumber ) );
                }
            }
            else if( mtb.UnMaskedText.Length != 10 )
            {
                e.Cancel = true;
                UIColors.SetErrorBgColor( mtb );
                MessageBox.Show( UIErrorMessages.PHONE_NUMBER_INVALID , "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1 );
            }
            else
            {
                Model_PhoneNumber = new PhoneNumber( mtb.UnMaskedText );

                if( PhoneNumberEnteredEvent != null )
                {
                    PhoneNumberEnteredEvent( this, new LooseArgs( Model_PhoneNumber ) );
                }
            }
        }
        #endregion

        #region Methods
        public override void UpdateView()
        {
            if( Model_PhoneNumber != null )
            {
                maskPhoneNumber.Text = Model_PhoneNumber.ToString();
            }
        }

        public void ResetView()
        {
            maskPhoneNumber.ResetText();
            UIColors.SetNormalBgColor( maskPhoneNumber );
            Model_PhoneNumber = null;
            Refresh();
        }
        #endregion

        #region Properties

        private PhoneNumber Model_PhoneNumber
        {
            get
            {
                return phoneNumber;
            }
            set
            {
                phoneNumber = value;
            }
        }

        public string GroupBoxText
        {
            get
            {
                return grpContact.Text;
            }
            set
            {
                grpContact.Text = value;
            }
        }

        public MaskedEditTextBox MaskedEditTextBox
        {   // Individual field validation rules require exposing the text control
            // to be able to check data and set BackColor, if necessary.
            get
            {
                return maskPhoneNumber;
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
            this.grpContact = new GroupBox();
            this.maskPhoneNumber = new MaskedEditTextBox();
            this.lblStaticPhone = new Label();
            this.grpContact.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpContact
            // 
            this.grpContact.Controls.Add(this.maskPhoneNumber);
            this.grpContact.Controls.Add(this.lblStaticPhone);
            this.grpContact.Dock = DockStyle.Fill;
            this.grpContact.Location = new Point(0, 0);
            this.grpContact.Name = "grpContact";
            this.grpContact.Size = new Size(257, 53);
            this.grpContact.TabIndex = 0;
            this.grpContact.TabStop = false;
            this.grpContact.Text = "Contact";
            // 
            // maskPhoneNumber
            // 
            this.maskPhoneNumber.EntrySelectionStyle = EntrySelectionStyle.SelectionAtEnd;
            this.maskPhoneNumber.KeyPressExpression = "^\\d*$";
            this.maskPhoneNumber.Location = new Point(51, 18);
            this.maskPhoneNumber.Mask = "(   )   -";
            this.maskPhoneNumber.MaxLength = 13;
            this.maskPhoneNumber.Name = "maskPhoneNumber";
            this.maskPhoneNumber.TabIndex = 2;
            this.maskPhoneNumber.ValidationExpression = "^\\d*$";
            this.maskPhoneNumber.Validating += new CancelEventHandler(this.maskPhoneNumber_Validating);
            // 
            // lblStaticPhone
            // 
            this.lblStaticPhone.Location = new Point(8, 21);
            this.lblStaticPhone.Name = "lblStaticPhone";
            this.lblStaticPhone.Size = new Size(39, 20);
            this.lblStaticPhone.TabIndex = 1;
            this.lblStaticPhone.Text = "Phone:";
            // 
            // ContactView
            // 
            this.Controls.Add(this.grpContact);
            this.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.Name = "ContactView";
            this.Size = new Size(257, 53);
            this.grpContact.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion
        #endregion

        #region Construction and Finalization
        public ContactView()
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
        private GroupBox               grpContact;
        private Label                  lblStaticPhone;
        private MaskedEditTextBox    maskPhoneNumber;
        private PhoneNumber                                 phoneNumber;
        #endregion
    }
}
