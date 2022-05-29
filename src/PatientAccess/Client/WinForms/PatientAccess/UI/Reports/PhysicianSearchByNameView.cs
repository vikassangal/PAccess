using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.Remoting;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.Reports
{
    /// <summary>
    /// Summary description for PhysicianSearchByNameView.
    /// </summary>
    public class PhysicianSearchByNameView : ControlView
    {
        #region Events

        public event EventHandler PhysiciansFound;

        public event EventHandler ResetView;

        public event EventHandler NoPhysiciansFound;

        public event EventHandler DisableDetailsButton;

        public event EventHandler ShowNoResultsLabel;

        #endregion

        #region Event Handlers
        private void View_Load(object sender, EventArgs e)
        {
            this.lastNameTextBox.Focus();
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            this.ResetView(this, null);
            ResetBackGroundColor();
            ViewPhysicianInquiry();
        }


        private void lastNameTextBox_Enter(object sender, EventArgs e)
        {
            this.lastNameTextBox.SelectionStart = this.lastNameTextBox.TextLength;
        }


        private void lastNameTextBox_Leave(object sender, EventArgs e)
        {
            this.lastNameTextBox.Text = this.lastNameTextBox.Text.TrimEnd(null);
        }


        private void lastNameTextBox_TextChanged(object sender, EventArgs e)
        {
            CheckForDataEntered();
        }


        private void firstNameTextBox_Enter(object sender, EventArgs e)
        {
            this.firstNameTextBox.SelectionStart = this.firstNameTextBox.TextLength;
        }


        private void firstNameTextBox_Leave(object sender, EventArgs e)
        {
            this.firstNameTextBox.Text = this.firstNameTextBox.Text.TrimEnd(null);
        }


        private void firstNameTextBox_TextChanged(object sender, EventArgs e)
        {
            CheckForDataEntered();
        }

        private void numberTextBox_Leave(object sender, EventArgs e)
        {
            this.numberTextBox.Text = this.numberTextBox.Text.TrimEnd(null);
        }

        private void numberTextBox_TextChanged(object sender, EventArgs e)
        {
            UIColors.SetNormalBgColor(numberTextBox);
            CheckForDataEntered();
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            if (DisableDetailsButton != null)
            {
                DisableDetailsButton(this, e);
            }
            ResetBackGroundColor();
            this.numberTextBox.ResetText();
            this.firstNameTextBox.ResetText();
            this.lastNameTextBox.ResetText();
            this.searchButton.Enabled = false;
            if (ShowNoResultsLabel != null)
            {
                ShowNoResultsLabel(this, e);
            }
            this.ResetView(this, null);
            this.Invalidate();
            this.Update();
        }


        #endregion

        #region Construction and Finalization

        /// <summary>
        /// Constructor
        /// </summary>
        public PhysicianSearchByNameView()
        {
            InitializeComponent();
            base.EnableThemesOn(this);
        }


        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #endregion

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lastNameLabel = new System.Windows.Forms.Label();
            this.lastNameTextBox = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.firstNameLabel = new System.Windows.Forms.Label();
            this.firstNameTextBox = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.numberLabel = new System.Windows.Forms.Label();
            this.numberTextBox = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.searchButton = new LoggingButton();
            this.resetButton = new LoggingButton();
            this.searchCriteriaLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lastNameLabel
            // 
            this.lastNameLabel.AutoSize = true;
            this.lastNameLabel.Location = new System.Drawing.Point(10, 37);
            this.lastNameLabel.Name = "lastNameLabel";
            this.lastNameLabel.Size = new System.Drawing.Size(60, 16);
            this.lastNameLabel.TabIndex = 0;
            this.lastNameLabel.Text = "Last name:";
            this.lastNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lastNameTextBox
            // 
            this.lastNameTextBox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.lastNameTextBox.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.lastNameTextBox.KeyPressExpression = "^[a-zA-Z][ a-zA-Z0-9,\\- ]*$";
            this.lastNameTextBox.Location = new System.Drawing.Point(72, 34);
            this.lastNameTextBox.Mask = "";
            this.lastNameTextBox.MaxLength = 25;
            this.lastNameTextBox.Name = "lastNameTextBox";
            this.lastNameTextBox.Size = new System.Drawing.Size(157, 20);
            this.lastNameTextBox.TabIndex = 1;
            this.lastNameTextBox.ValidationExpression = "^[a-zA-Z][ a-zA-Z0-9,\\- ]*$";
            this.lastNameTextBox.Leave += new System.EventHandler(this.lastNameTextBox_Leave);
            this.lastNameTextBox.TextChanged += new System.EventHandler(this.lastNameTextBox_TextChanged);
            this.lastNameTextBox.Enter += new System.EventHandler(this.lastNameTextBox_Enter);
            // 
            // firstNameLabel
            // 
            this.firstNameLabel.AutoSize = true;
            this.firstNameLabel.Location = new System.Drawing.Point(243, 37);
            this.firstNameLabel.Name = "firstNameLabel";
            this.firstNameLabel.Size = new System.Drawing.Size(61, 16);
            this.firstNameLabel.TabIndex = 0;
            this.firstNameLabel.Text = "First name:";
            this.firstNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // firstNameTextBox
            // 
            this.firstNameTextBox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.firstNameTextBox.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.firstNameTextBox.KeyPressExpression = "^[a-zA-Z][ a-zA-Z0-9,\\- ]*$";
            this.firstNameTextBox.Location = new System.Drawing.Point(304, 34);
            this.firstNameTextBox.Mask = "";
            this.firstNameTextBox.MaxLength = 15;
            this.firstNameTextBox.Name = "firstNameTextBox";
            this.firstNameTextBox.Size = new System.Drawing.Size(134, 20);
            this.firstNameTextBox.TabIndex = 2;
            this.firstNameTextBox.ValidationExpression = "^[a-zA-Z][ a-zA-Z0-9,\\- ]*$";
            this.firstNameTextBox.Leave += new System.EventHandler(this.firstNameTextBox_Leave);
            this.firstNameTextBox.TextChanged += new System.EventHandler(this.firstNameTextBox_TextChanged);
            this.firstNameTextBox.Enter += new System.EventHandler(this.firstNameTextBox_Enter);
            // 
            // numberLabel
            // 
            this.numberLabel.AutoSize = true;
            this.numberLabel.Location = new System.Drawing.Point(453, 37);
            this.numberLabel.Name = "numberLabel";
            this.numberLabel.Size = new System.Drawing.Size(48, 16);
            this.numberLabel.TabIndex = 0;
            this.numberLabel.Text = "Number:";
            this.numberLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // numberTextBox
            // 
            this.numberTextBox.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.numberTextBox.KeyPressExpression = "^[0-9]*$";
            this.numberTextBox.Location = new System.Drawing.Point(502, 34);
            this.numberTextBox.Mask = "";
            this.numberTextBox.MaxLength = 5;
            this.numberTextBox.Name = "numberTextBox";
            this.numberTextBox.Size = new System.Drawing.Size(77, 20);
            this.numberTextBox.TabIndex = 3;
            this.numberTextBox.ValidationExpression = "^[0-9]*$";
            this.numberTextBox.Leave += new System.EventHandler(this.numberTextBox_Leave);
            this.numberTextBox.TextChanged += new System.EventHandler(this.numberTextBox_TextChanged);
            // 
            // searchButton
            // 
            this.searchButton.Enabled = false;
            this.searchButton.Location = new System.Drawing.Point(720, 33);
            this.searchButton.Name = "searchButton";
            this.searchButton.TabIndex = 4;
            this.searchButton.Text = "Sear&ch";
            this.searchButton.Click += new System.EventHandler(this.searchButton_Click);
            // 
            // resetButton
            // 
            this.resetButton.Location = new System.Drawing.Point(808, 33);
            this.resetButton.Name = "resetButton";
            this.resetButton.TabIndex = 5;
            this.resetButton.Text = "Rese&t";
            this.resetButton.Click += new System.EventHandler(this.resetButton_Click);
            // 
            // searchCriteriaLabel
            // 
            this.searchCriteriaLabel.AutoSize = true;
            this.searchCriteriaLabel.Location = new System.Drawing.Point(10, 7);
            this.searchCriteriaLabel.Name = "searchCriteriaLabel";
            this.searchCriteriaLabel.Size = new System.Drawing.Size(824, 16);
            this.searchCriteriaLabel.TabIndex = 0;
            this.searchCriteriaLabel.Text = "Search by Name, Number, or both. When searching by First Name, Last Name is requi" +
                "red. Partial search is supported for Last and First Names, but not for Number.";
            // 
            // PhysicianSearchByNameView
            // 
            this.AcceptButton = this.searchButton;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.searchCriteriaLabel);
            this.Controls.Add(this.resetButton);
            this.Controls.Add(this.searchButton);
            this.Controls.Add(this.numberTextBox);
            this.Controls.Add(this.numberLabel);
            this.Controls.Add(this.firstNameTextBox);
            this.Controls.Add(this.firstNameLabel);
            this.Controls.Add(this.lastNameTextBox);
            this.Controls.Add(this.lastNameLabel);
            this.Name = "PhysicianSearchByNameView";
            this.Size = new System.Drawing.Size(892, 69);
            this.Load += new System.EventHandler(this.View_Load);
            this.ResumeLayout(false);

        }
        #endregion

        #region Methods
        #endregion

        #region public Properties
        #endregion

        #region Private Methods

        /// <summary>
        /// Resets the background color of all the controls to white.
        /// </summary>
        private void ResetBackGroundColor()
        {
            UIColors.SetNormalBgColor(firstNameTextBox);
            UIColors.SetNormalBgColor(lastNameTextBox);
            UIColors.SetNormalBgColor(numberTextBox);
        }

        /// <summary>
        /// Highlights the appropriate text box with red color if appropriate value is not entered
        /// </summary>
        /// <param name="aspectInError"></param>
        private void SearchError(string aspectInError)
        {
            switch (aspectInError)
            {
                case "FirstName":
                    this.firstNameTextBox.BackColor =
                        Color.Red;
                    this.firstNameTextBox.Select();
                    break;
                case "Number":
                    this.numberTextBox.BackColor =
                        Color.Red;
                    this.numberTextBox.Select();
                    break;
                case "LastName":
                    this.lastNameTextBox.BackColor =
                        Color.Red;
                    this.lastNameTextBox.Select();
                    break;
                case "PhysicianNumber":
                    this.numberTextBox.Clear();
                    this.numberTextBox.BackColor =
                        Color.Red;
                    this.numberTextBox.Select();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Checks wheather data has been entered or no in the textboxes
        /// </summary>
        private void CheckForDataEntered()
        {
            this.searchButton.Enabled = !(this.numberTextBox.Text == String.Empty &&
               this.lastNameTextBox.Text == String.Empty &&
               this.firstNameTextBox.Text == String.Empty);
        }

        /// <summary>
        /// Checks wheather the data entered is valid
        /// </summary>
        /// <returns></returns>
        private new ValidationResult Validate()
        {
            if (this.lastNameTextBox.Text != String.Empty)
            {
                return new ValidationResult(true);
            }
            if (this.firstNameTextBox.Text != String.Empty &&
                this.lastNameTextBox.Text == String.Empty)
            {
                return new ValidationResult(false,
                    UIErrorMessages.FIRSTNAME_WITHOUT_LASTNAME_ERRMSG, LAST_NAME);
            }
            if (RESERVED_PHYSICIAN_NUMBER_8888_ERR_MSG == Convert.ToInt64(numberTextBox.Text))
            {
                return new ValidationResult(
                    false,
                    UIErrorMessages.RESERVED_PHYSICIAN_NUMBER_8888_ERR_MSG,
                    PHYSICIAN_NUMBER);
            }
            if (this.numberTextBox.Text != String.Empty)
            {
                return new ValidationResult(true);
            }

            return new ValidationResult(false);
        }

        /// <summary>
        /// Gets the data from database based on search criteria.
        /// </summary>
        private void ViewPhysicianInquiry()
        {
            ICollection physicianList = null;
            try
            {
                this.Cursor = Cursors.WaitCursor;

                //Check to see if search data entered is valid.
                ValidationResult result = Validate();

                if (result.IsValid)
                {
                    try
                    {
                        long number;
                        if (numberTextBox.Text == string.Empty)
                        {
                            number = 0;
                        }
                        else
                        {
                            number = Convert.ToInt64(numberTextBox.Text);
                        }
                        //Create PatientSearchCriteria and pass in all args.
                        PhysicianSearchCriteria physicianSearchCriteria =
                            new PhysicianSearchCriteria(User.GetCurrent().Facility,
                            this.firstNameTextBox.Text,
                            this.lastNameTextBox.Text,
                            number);
                        
                        IPhysicianBroker broker = BrokerFactory.BrokerOfType<IPhysicianBroker>();
                        physicianList = broker.PhysiciansMatching(physicianSearchCriteria);
                        
                        if (physicianList != null)
                        {
                            if (physicianList.Count != 0 && PhysiciansFound != null)
                            {
                                PhysiciansFound(this, new LooseArgs(physicianList));
                            }
                            else
                            {
                                if (NoPhysiciansFound != null)
                                {
                                    NoPhysiciansFound(this, new LooseArgs(this));
                                }
                            }
                        }

                    }
                    finally
                    {
                        this.Cursor = Cursors.Default;
                    }
                }
                else
                {
                    this.ResetView(this, null);

                    string errorMsg = result.Message;
                    if (errorMsg != String.Empty)
                    {
                        SearchError(result.AspectInError);
                        MessageBox.Show(errorMsg, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                    }
                    ResetBackGroundColor();
                }
            }
            catch (RemotingTimeoutException)
            {
                MessageBox.Show(UIErrorMessages.PHYSICIAN_SEARCH_TIMEOUT_MSG);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        #endregion

        #region Data Elements

        private Label lastNameLabel;
        private MaskedEditTextBox lastNameTextBox;
        private Label firstNameLabel;
        private MaskedEditTextBox firstNameTextBox;
        private Label numberLabel;
        private MaskedEditTextBox numberTextBox;
        private LoggingButton searchButton;
        private LoggingButton resetButton;
        private Label searchCriteriaLabel;
        private Container components = null;

        #endregion

        #region Constants
        private const string LAST_NAME = "LastName",
                                PHYSICIAN_NUMBER = "PhysicianNumber";

        private const long RESERVED_PHYSICIAN_NUMBER_8888_ERR_MSG = 8888;
        #endregion

    }
}
