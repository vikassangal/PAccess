using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain.Parties;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.CommonControls
{
	/// <summary>
    /// GBLExceptionDialog - displays global exception information.
	/// </summary>
	[Serializable]
    public class GBLExceptionDialogCrash : LoggingFormView
    {
        #region Event Handlers
        private void GBLExceptionDialog_Load(object sender, EventArgs e)
        {
            ToggleErrorDetails( sender, e );
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if( this.mtbPhoneNumber.UnMaskedText.Length > 0 )
            {
                this.PhoneNumber = new PhoneNumber( this.mtbPhoneNumber.UnMaskedText );
            }
            else
            {
                this.PhoneNumber = new PhoneNumber();
            }

            
            //this.Dispose();
        }

        private void btnDetails_Click(object sender, EventArgs e)
        {
            ToggleErrorDetails( sender, e );
        }

        private void lnkErrorMessageDetail_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            lnkErrorMessageDetail.LinkVisited = false;
            Process.Start(e.Link.LinkData.ToString());
        }

        private void mtbPhoneNumber_Validating(object sender, CancelEventArgs e)
        {
            this.IsPhoneNumberValid();
        }
        #endregion

        #region Public Methods
        public override void UpdateView()
        {
            // Set the error message.
            this.lblErrorMessage.Text = this.i_ErrorText;
            
            // Set the error details.
            this.lnkErrorMessageDetail.Text = String.Format( "{0}{1}{2}{3}{4}",
                Environment.NewLine,
                "An error has occurred within the appliation.", 
                Environment.NewLine, 
                Environment.NewLine, 
                "We have created an error report that will be sent to help us improve Patient Access",
                Environment.NewLine, 
                "To see what this error report contains, click here."
                );

            this.lnkErrorMessageDetail.Links.Add( lnkErrorMessageDetail.Text.Length - 12, 12, "home.hdc.net" );

        }
        #endregion

        #region Public Properties
        public string ErrorText
        {
            get
            {
                return i_ErrorText;
            }
            set
            {
                i_ErrorText = value;
            }
        }

        public string ErrorDetails
        {
            get
            {
                return i_ErrorDetails;
            }
            set
            {
                i_ErrorDetails = value;
            }
        }

	    private PhoneNumber PhoneNumber
        {
            get
            {
                return i_PhoneNumber;
            }
            set
            {
                i_PhoneNumber = value;
            }
        }

        public string Comments
        {
            get
            {
                return i_Comments;
            }
            set
            {
                i_Comments = value;
            }
        }
        #endregion

        #region Private Methods

	    private bool IsPhoneNumberValid()
        {
            if( this.mtbPhoneNumber.UnMaskedText.Length > 0 && this.mtbPhoneNumber.UnMaskedText.Length < 10 )
            {
                MessageBox.Show( UIErrorMessages.INVALID_PHONE_NUMBER,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1 );

                if( this.ContainsFocus )
                {
                    this.mtbPhoneNumber.Focus();
                }

                return false;
            }
            return true;
        }

        private void ToggleErrorDetails( object sender, EventArgs e )
        {
            if( DetailsShown )
            {
                btnDetails.Text = "&Details >>";
                txtDetailErrors.Visible = false;
                this.Height = this.Height - 168;
                DetailsShown = false;
            } 
            else
            {
                btnDetails.Text = "&Details <<";
                txtDetailErrors.Visible = true;
                this.Height = this.Height + 168;
                DetailsShown = true;
            }
            Invalidate();
        }
        #endregion

        #region Private Properties
        private bool DetailsShown
        {
            get
            {
                return i_DetailsShown;
            }
            set
            {
                i_DetailsShown = value;
            }
        }
        #endregion

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnDetails = new LoggingButton();
            this.lblErrorMessage = new Label();
            this.txtDetailErrors = new TextBox();
            this.btnSend = new Button();
            this.lblErrorMEssageBackground = new Label();
            this.lblErrorMessageDetailBackground = new Label();
            this.lnkErrorMessageDetail = new LinkLabel();
            this.pnlSplitter = new Panel();
            this.lblComments = new Label();
            this.txtComments = new TextBox();
            this.lblEmailAddress = new Label();
            this.mtbPhoneNumber = new MaskedEditTextBox();
            this.SuspendLayout();
            // 
            // btnDetails
            // 
            this.btnDetails.Anchor = ((AnchorStyles)((AnchorStyles.Top | AnchorStyles.Right)));
            this.btnDetails.CausesValidation = false;
            this.btnDetails.Location = new Point(224, 384);
            this.btnDetails.Message = null;
            this.btnDetails.Name = "btnDetails";
            this.btnDetails.Size = new Size(90, 23);
            this.btnDetails.TabIndex = 0;
            this.btnDetails.TabStop = false;
            this.btnDetails.Text = "&Details >>";
            this.btnDetails.Click += this.btnDetails_Click;
            // 
            // lblErrorMessage
            // 
            this.lblErrorMessage.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left) 
                | AnchorStyles.Right)));
            this.lblErrorMessage.BackColor = Color.White;
            this.lblErrorMessage.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((Byte)(0)));
            this.lblErrorMessage.Location = new Point(20, 0);
            this.lblErrorMessage.Name = "lblErrorMessage";
            this.lblErrorMessage.Size = new Size(433, 92);
            this.lblErrorMessage.TabIndex = 8;
            this.lblErrorMessage.Text = "The system has experienced an unexpected error related to the data in this accoun" +
                "t. The system must cancel this activity. You will lose any information you have " +
                "added or changed, but you can continue working in the system.";
            this.lblErrorMessage.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtDetailErrors
            // 
            this.txtDetailErrors.BackColor = Color.White;
            this.txtDetailErrors.CausesValidation = false;
            this.txtDetailErrors.Location = new Point(8, 384);
            this.txtDetailErrors.MaxLength = 5000;
            this.txtDetailErrors.Multiline = true;
            this.txtDetailErrors.Name = "txtDetailErrors";
            this.txtDetailErrors.ReadOnly = true;
            this.txtDetailErrors.ScrollBars = ScrollBars.Vertical;
            this.txtDetailErrors.Size = new Size(16, 16);
            this.txtDetailErrors.TabIndex = 5;
            this.txtDetailErrors.Text = string.Empty;
            // 
            // btnSend
            // 
            this.btnSend.DialogResult = DialogResult.Cancel;
            this.btnSend.Location = new Point(344, 384);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new Size(112, 23);
            this.btnSend.TabIndex = 9;
            this.btnSend.Text = "&Send Error Report";
            this.btnSend.Click += new EventHandler(this.btnSend_Click);
            // 
            // lblErrorMEssageBackground
            // 
            this.lblErrorMEssageBackground.BackColor = Color.White;
            this.lblErrorMEssageBackground.Location = new Point(0, 0);
            this.lblErrorMEssageBackground.Name = "lblErrorMEssageBackground";
            this.lblErrorMEssageBackground.Size = new Size(473, 92);
            this.lblErrorMEssageBackground.TabIndex = 10;
            // 
            // lblErrorMessageDetailBackground
            // 
            this.lblErrorMessageDetailBackground.BackColor = SystemColors.Control;
            this.lblErrorMessageDetailBackground.Location = new Point(0, 94);
            this.lblErrorMessageDetailBackground.Name = "lblErrorMessageDetailBackground";
            this.lblErrorMessageDetailBackground.Size = new Size(473, 168);
            this.lblErrorMessageDetailBackground.TabIndex = 11;
            // 
            // lnkErrorMessageDetail
            // 
            this.lnkErrorMessageDetail.LinkBehavior = LinkBehavior.AlwaysUnderline;
            this.lnkErrorMessageDetail.Location = new Point(20, 94);
            this.lnkErrorMessageDetail.Name = "lnkErrorMessageDetail";
            this.lnkErrorMessageDetail.Size = new Size(433, 168);
            this.lnkErrorMessageDetail.TabIndex = 12;
            this.lnkErrorMessageDetail.VisitedLinkColor = Color.Blue;
            this.lnkErrorMessageDetail.LinkClicked += new LinkLabelLinkClickedEventHandler(this.lnkErrorMessageDetail_LinkClicked);
            // 
            // pnlSplitter
            // 
            this.pnlSplitter.BackColor = Color.Black;
            this.pnlSplitter.BorderStyle = BorderStyle.FixedSingle;
            this.pnlSplitter.Location = new Point(0, 93);
            this.pnlSplitter.Name = "pnlSplitter";
            this.pnlSplitter.Size = new Size(473, 1);
            this.pnlSplitter.TabIndex = 13;
            // 
            // lblComments
            // 
            this.lblComments.Location = new Point(20, 272);
            this.lblComments.Name = "lblComments";
            this.lblComments.Size = new Size(300, 16);
            this.lblComments.TabIndex = 14;
            this.lblComments.Text = "&What were you doing when the error happened? (optional)";
            // 
            // txtComments
            // 
            this.txtComments.Location = new Point(20, 288);
            this.txtComments.Multiline = true;
            this.txtComments.Name = "txtComments";
            this.txtComments.Size = new Size(433, 50);
            this.txtComments.TabIndex = 15;
            this.txtComments.Text = string.Empty;
            // 
            // lblEmailAddress
            // 
            this.lblEmailAddress.Location = new Point(20, 352);
            this.lblEmailAddress.Name = "lblEmailAddress";
            this.lblEmailAddress.Size = new Size(132, 16);
            this.lblEmailAddress.TabIndex = 16;
            this.lblEmailAddress.Text = "&Email address (optional):";
            // 
            // mtbPhoneNumber
            // 
            this.mtbPhoneNumber.EntrySelectionStyle = EntrySelectionStyle.SelectionAtEnd;
            this.mtbPhoneNumber.KeyPressExpression = "^\\d*$";
            this.mtbPhoneNumber.Location = new Point(152, 352);
            this.mtbPhoneNumber.Mask = "(   )   -";
            this.mtbPhoneNumber.MaxLength = 13;
            this.mtbPhoneNumber.Name = "mtbPhoneNumber";
            this.mtbPhoneNumber.Size = new Size(77, 20);
            this.mtbPhoneNumber.TabIndex = 4;
            this.mtbPhoneNumber.ValidationExpression = "^\\d*$";
            this.mtbPhoneNumber.Validating += new CancelEventHandler(this.mtbPhoneNumber_Validating);
            // 
            // GBLExceptionDialogCrash
            // 
            this.AcceptButton = this.btnSend;
            this.AutoScaleBaseSize = new Size(5, 13);
            this.BackColor = SystemColors.Control;
            this.CancelButton = this.btnSend;
            this.ClientSize = new Size(467, 416);
            this.ControlBox = false;
            this.Controls.Add(this.mtbPhoneNumber);
            this.Controls.Add(this.lblEmailAddress);
            this.Controls.Add(this.txtComments);
            this.Controls.Add(this.lblComments);
            this.Controls.Add(this.pnlSplitter);
            this.Controls.Add(this.lnkErrorMessageDetail);
            this.Controls.Add(this.lblErrorMessageDetailBackground);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.lblErrorMessage);
            this.Controls.Add(this.txtDetailErrors);
            this.Controls.Add(this.btnDetails);
            this.Controls.Add(this.lblErrorMEssageBackground);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GBLExceptionDialogCrash";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = SizeGripStyle.Show;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Patient Access Error";
            this.Load += new EventHandler(this.GBLExceptionDialog_Load);
            this.ResumeLayout(false);

        }
        #endregion

        #region Construction and Finalization
        public GBLExceptionDialogCrash()
        {
            this.InitializeComponent();
            base.EnableThemesOn( this );
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
        private PhoneNumber i_PhoneNumber = new PhoneNumber();
        private string i_Comments = String.Empty;
        private LoggingButton btnDetails;
        string i_ErrorText;
        string i_ErrorDetails;
        private TextBox txtDetailErrors;
        private Button btnSend;
        private Label lblErrorMessage;
        private LinkLabel lnkErrorMessageDetail;
        private Label lblErrorMEssageBackground;
        private Label lblErrorMessageDetailBackground;
        private Label lblComments;
        private Label lblEmailAddress;
        private MaskedEditTextBox mtbPhoneNumber;
        private TextBox txtComments;
        private Panel pnlSplitter;
        private bool i_DetailsShown = true;
        #endregion

        #region Constants
        #endregion
    }
}