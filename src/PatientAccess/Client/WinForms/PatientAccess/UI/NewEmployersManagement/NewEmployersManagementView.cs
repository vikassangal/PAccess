using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.UI.CommonControls;
using PatientAccess.Rules;
using PatientAccess.UI.AddressViews;
using PatientAccess.UI.HelperClasses;
using PatientAccess.UI.Factories;

namespace PatientAccess.UI.NewEmployersManagement
{

    /// <summary>
    /// This class represents the new employers management screen.
    /// </summary>
    internal sealed partial class NewEmployersManagementView : ControlView, INewEmployersListView, IMasterEmployersListView
    {
        private const string
            NO_NEW_EMPLOYERS_FOUND = "No items found.",
            INCOMPLETE_PHONE_NUMBER_MSG = "The phone number is incomplete. Enter one or more missing digits.",
            INVALID_INPUT_MSG = "We're sorry, but only digits (0-9) are allowed in phone numbers.",
            BLANK_EMPLOYER_NAME_MESSAGE = "The employer field cannot be blank. Enter an employer.",
            RETRIEVING_NEW_EMPLOYERS_MESSAGE = "Retrieving new employers",
            SEARCHING_MESSAGE = "Searching...",
            NO_ITEMS_FOUND_MESSAGE = "No items found",
            MOVE_INFO_MESSAGE_WITH_NO_SELECTION = "Move all information or only address and phone if desired. When " +
                "moving address and phone, first select the desired employer in the Master Employer List.",
            MOVE_INFO_MESSAGE_WITH_SELECTION = "Move All Info will move all the selected employer information above " +
                "to a new entry in the Master Employer List. Move Address and Phone will append only the address and " +
                "phone number above to the employer you have selected in the Master Employer List.",
            ERROR_MESSAGE_CAPTION = "Error";


        private INewEmployersManagementPresenter _presenter;

        public NewEmployersManagementView()
        {
            InitializeComponent();
            ConfigureControls();
            // leave these here, lest the Designer destroy them
            this.stepOneLabel.UseCompatibleTextRendering = false;
            this.stepTwoLabel.UseCompatibleTextRendering = false;
            this.stepThreeLabel.UseCompatibleTextRendering = false;
            this.moveButtonInstructionsLabel.UseCompatibleTextRendering = false;
            this.featureIsLockedMessageLabel.UseCompatibleTextRendering = false;
            this.editAddressButton.UseCompatibleTextRendering = false;
            this.clearButton.UseCompatibleTextRendering = false;

        }

        #region IMasterEmployersListView Members

        string IMasterEmployersListView.SearchString
        {
            get
            {
                return this.masterEmployerSearchTextBox.Text;
            }
            set
            {
                ControlExtensions.UseInvokeIfNeeded(this, delegate { this.masterEmployerSearchTextBox.Text = value; });

            }
        }

        bool IMasterEmployersListView.IsSearchButtonEnabled
        {
            get
            {
                return this.masterEmployerSearchButton.Enabled;
            }
            set
            {
                ControlExtensions.UseInvokeIfNeeded(this, delegate { this.masterEmployerSearchButton.Enabled = value; });
            }
        }


        void IMasterEmployersListView.SelectEmployer(Employer employer)
        {
            ControlExtensions.UseInvokeIfNeeded(this, delegate
            {
                foreach (DataGridViewRow row in this.masterEmployerListDataGridView.Rows)
                {
                    if (row.Tag == employer)
                    {
                        row.Selected = true;
                        return;
                    }
                }
            });
        }


        void IMasterEmployersListView.ShowSearchInProgressMessage()
        {
            ControlExtensions.UseInvokeIfNeeded(this,
                delegate
                {
                    this.masterEmployerListDataGridView.Visible = false;
                    this.MasterListMessageLabel.Visible = true;
                    this.MasterListMessageLabel.Text = SEARCHING_MESSAGE;
                    this.Cursor = Cursors.WaitCursor;
                });
        }


        void IMasterEmployersListView.ClearSearchInProgressMessage()
        {
            ControlExtensions.UseInvokeIfNeeded(this, delegate
            {
                this.masterEmployerListDataGridView.Visible = true;
                this.MasterListMessageLabel.Visible = false;
                this.MasterListMessageLabel.Text = string.Empty;
                this.Cursor = Cursors.Default;
            });
        }


        void IMasterEmployersListView.ClearMasterListViewResults()
        {
            ControlExtensions.UseInvokeIfNeeded(this,
                delegate
                {
                    this.masterEmployerListDataGridView.Rows.Clear();
                    this.masterEmployerListDataGridView.Padding = Padding.Empty;
                    this.masterEmployerListDataGridView.Margin = Padding.Empty;
                    this.masterEmployerListDataGridView.ClearSelection();
                    this.masterEmployerListDataGridView.CurrentCell = null;


                    this.masterEmployerAddressesDataGridView.Rows.Clear();
                    this.masterEmployerAddressesDataGridView.Padding = Padding.Empty;
                    this.masterEmployerAddressesDataGridView.Margin = Padding.Empty;

                });
        }


        void IMasterEmployersListView.SelectFirstAddress()
        {
            if (this.masterEmployerAddressesDataGridView.Rows != null && this.masterEmployerAddressesDataGridView.Rows.Count != 0)
            {
                this.masterEmployerAddressesDataGridView.Rows[0].Selected = true;
            }
        }


        void INewEmployersListView.DisplayMoveAllInfoMessageWithNoEmployerSelection()
        {
            ControlExtensions.UseInvokeIfNeeded(this,
                delegate
                {
                    this.moveButtonInstructionsLabel.Text = MOVE_INFO_MESSAGE_WITH_NO_SELECTION;
                });
        }


        void INewEmployersListView.DisplayMoveAllInfoMessageWithEmployerSelection()
        {
            ControlExtensions.UseInvokeIfNeeded(this,
                delegate
                {
                    this.moveButtonInstructionsLabel.Text = MOVE_INFO_MESSAGE_WITH_SELECTION;
                });
        }


        void IMasterEmployersListView.ShowSelectedEmployerAddresses(IEnumerable<string> addresses)
        {
            ControlExtensions.UseInvokeIfNeeded(this, delegate
            {
                this.masterEmployerAddressesDataGridView.Visible = true;
                this.noAddressesLabel.Visible = false;

                this.masterEmployerAddressesDataGridView.Rows.Clear();
                this.masterEmployerAddressesDataGridView.Padding = Padding.Empty;
                this.masterEmployerAddressesDataGridView.Margin = Padding.Empty;
                foreach (string address in addresses)
                {
                    object[] row = new object[] { address };
                    this.masterEmployerAddressesDataGridView.Rows.Add(row);
                }

                masterEmployerAddressesDataGridView.ClearSelection();
                masterEmployerAddressesDataGridView.CurrentCell = null;
            });
        }


        void IMasterEmployersListView.ShowMessageWhenEmployerDoesNotHaveAnAddress()
        {
            ControlExtensions.UseInvokeIfNeeded(this,
                delegate
                {
                    this.masterEmployerAddressesDataGridView.Visible = false;
                    this.noAddressesLabel.Visible = true;
                    this.noAddressesLabel.Text = NO_ITEMS_FOUND_MESSAGE;
                });
        }


        void IMasterEmployersListView.ShowMessageWhenSearchReturnsNoResults()
        {
            ControlExtensions.UseInvokeIfNeeded(this,
                delegate
                {
                    this.masterEmployerListDataGridView.Visible = false;
                    this.MasterListMessageLabel.Visible = true;
                    this.MasterListMessageLabel.Text = NO_ITEMS_FOUND_MESSAGE;
                });
        }


        void IMasterEmployersListView.ShowEmployersWithoutSelection(IEnumerable<Employer> employers)
        {
            ControlExtensions.UseInvokeIfNeeded(this,
                delegate
                {
                    this.masterEmployerListDataGridView.Visible = true;
                    this.MasterListMessageLabel.Visible = false;

                    this.masterEmployerListDataGridView.Rows.Clear();
                    this.masterEmployerListDataGridView.Padding = Padding.Empty;
                    this.masterEmployerListDataGridView.Margin = Padding.Empty;

                    foreach (Employer employer in employers)
                    {
                        object[] row = new object[] { employer.Name, employer.NationalId };
                        int rowIndex = this.masterEmployerListDataGridView.Rows.Add(row);
                        this.masterEmployerListDataGridView.Rows[rowIndex].Tag = employer;
                    }
                    this.masterEmployerListDataGridView.ClearSelection();
                    this.masterEmployerListDataGridView.CurrentCell = null;

                });
        }

        #endregion

        #region INewEmployersListView Members

        bool INewEmployersListView.ClearEmployerContactInformationEnabled
        {
            get
            {
                return clearButton.Enabled;

            }
            set
            {
                ControlExtensions.UseInvokeIfNeeded(this, delegate { clearButton.Enabled = value; });
            }
        }

        bool INewEmployersListView.DeleteEnabled
        {
            get
            {
                return deleteButton.Enabled;
            }

            set
            {
                ControlExtensions.UseInvokeIfNeeded(this, delegate { deleteButton.Enabled = value; });
            }
        }

        bool INewEmployersListView.EditAddressEnabled
        {
            get
            {
                return this.editAddressButton.Enabled;
            }

            set
            {
                ControlExtensions.UseInvokeIfNeeded(this, delegate { editAddressButton.Enabled = value; });
            }
        }

        bool INewEmployersListView.MoveAddressAndPhoneEnabled
        {
            get
            {
                return moveAddressAndPhoneButton.Enabled;
            }

            set
            {
                ControlExtensions.UseInvokeIfNeeded(this, delegate { moveAddressAndPhoneButton.Enabled = value; });
            }
        }

        bool INewEmployersListView.MoveAllInfoEnabled
        {
            get
            {
                return moveAllInfoButton.Enabled;
            }

            set
            {
                ControlExtensions.UseInvokeIfNeeded(this, delegate { moveAllInfoButton.Enabled = value; });
            }
        }

        bool INewEmployersListView.UndoEnabled
        {
            get
            {
                return undoButton.Enabled;
            }

            set
            {
                ControlExtensions.UseInvokeIfNeeded(this, delegate { undoButton.Enabled = value; });
            }
        }

        bool INewEmployersListView.SelectedEmployerNameFieldEnabled
        {
            get
            {
                return this.selectedEmployerNameTextBox.Enabled;
            }
            set
            {
                ControlExtensions.UseInvokeIfNeeded(this, delegate { this.selectedEmployerNameTextBox.Enabled = value; });

            }
        }

        bool INewEmployersListView.SelectedEmployerNationalIdFieldEnabled
        {
            get
            {
                return this.selectedEmployerNationalIdTextBox.Enabled;
            }
            set
            {
                ControlExtensions.UseInvokeIfNeeded(this, delegate { this.selectedEmployerNationalIdTextBox.Enabled = value; });

            }
        }

        bool INewEmployersListView.SelectedEmployerPhoneFieldEnabled
        {
            get
            {
                return this.selectedEmployerPhoneTextBox.Enabled;
            }

            set
            {
                ControlExtensions.UseInvokeIfNeeded(this, delegate { selectedEmployerPhoneTextBox.Enabled = value; });
            }
        }

        bool INewEmployersListView.FinishEnabled
        {
            get
            {
                return this.finishButton.Enabled;
            }

            set
            {
                ControlExtensions.UseInvokeIfNeeded(this, delegate { finishButton.Enabled = value; });
            }
        }

        bool INewEmployersListView.CancelEnabled
        {
            get
            {
                return this.cancelButton.Enabled;
            }

            set
            {
                ControlExtensions.UseInvokeIfNeeded(this, delegate { cancelButton.Enabled = value; });
            }
        }

        bool INewEmployersListView.SelectedEmployerAddressFieldEnabled
        {
            get
            {
                return this.selectedEmployerAddressTextBox.Enabled;
            }
            set
            {
                ControlExtensions.UseInvokeIfNeeded(this, delegate { selectedEmployerAddressTextBox.Enabled = value; });
            }
        }

        string INewEmployersListView.SelectedEmployerAddress
        {
            get
            {
                return this.selectedEmployerAddressTextBox.Text;
            }
            set
            {
                ControlExtensions.UseInvokeIfNeeded(this, delegate { this.selectedEmployerAddressTextBox.Text = value; });
            }
        }

        string INewEmployersListView.SelectedEmployerName
        {
            get
            {
                return this.selectedEmployerNameTextBox.Text;
            }


            set
            {
                ControlExtensions.UseInvokeIfNeeded(this, delegate { this.selectedEmployerNameTextBox.Text = value; });
            }
        }

        string INewEmployersListView.SelectedEmployerNationalId
        {
            get
            {
                return this.selectedEmployerNationalIdTextBox.Text;
            }

            set
            {
                ControlExtensions.UseInvokeIfNeeded(this, delegate
                                                           { this.selectedEmployerNationalIdTextBox.Text = value; });
            }
        }

        string INewEmployersListView.SelectedEmployerPhoneNumber
        {
            get
            {
                return this.selectedEmployerPhoneTextBox.Text;
            }
            set
            {
                ControlExtensions.UseInvokeIfNeeded(this, delegate { this.selectedEmployerPhoneTextBox.Text = value; });
            }
        }

        public INewEmployersManagementPresenter Presenter
        {
            get
            {
                return this._presenter;
            }
            set
            {
                this._presenter = value;
            }
        }


        void INewEmployersListView.ShowRetrievingNewEmployersMessage()
        {
            ControlExtensions.UseInvokeIfNeeded(this,
                delegate
                {
                    this.newEmployerDataGridView.Rows.Clear();
                    this.newEmployerDataGridView.Visible = false;
                    this.noNewEmployersMessageLabel.Text = RETRIEVING_NEW_EMPLOYERS_MESSAGE;
                    this.noNewEmployersMessageLabel.Visible = true;
                });
        }


        void INewEmployersListView.ClearRetrievingNewEmployersMessage()
        {
            ControlExtensions.UseInvokeIfNeeded(this,
                delegate
                {
                    this.newEmployerDataGridView.Visible = true;
                    this.noNewEmployersMessageLabel.Text = String.Empty;
                    this.noNewEmployersMessageLabel.Visible = false;
                });

        }


        void INewEmployersListView.ClearSavingMessage()
        {
            this.Cursor = Cursors.WaitCursor;
        }


        public void ShowFeatureIsLockedMessage()
        {
            this.featureIsLockedMessageLabel.BringToFront();
            this.featureIsLockedMessageLabel.Visible = true;
        }


        void INewEmployersListView.UpdateEmployer(NewEmployerEntry employerEntry)
        {
            string employerInfo = Presenter.GetFormattedEmployerInfo(employerEntry);

            foreach (DataGridViewRow row in this.newEmployerDataGridView.Rows)
            {
                if (row.Tag == employerEntry)
                {
                    DataGridViewRow myRow = row;
                    myRow.Cells[0].Value = employerInfo;
                    myRow.Cells[1].Value = employerEntry.Employer.NationalId;
                    return;
                }
            }
        }

        void INewEmployersListView.ShowSavingMessage()
        {
            this.Cursor = DefaultCursor;
        }


        void INewEmployersListView.ShowNoNewUsersAvailableMessage()
        {
            ControlExtensions.UseInvokeIfNeeded(this,
                delegate
                {
                    this.newEmployerDataGridView.Visible = false;
                    this.noNewEmployersMessageLabel.Text = NO_NEW_EMPLOYERS_FOUND;
                    this.noNewEmployersMessageLabel.Visible = true;
                });

        }

        void INewEmployersListView.ShowEmployersWithoutSelection(IEnumerable<NewEmployerEntry> employers)
        {
            if (this.newEmployerDataGridView.Columns.Count > 0)
            {
                ControlExtensions.UseInvokeIfNeeded(this,
                    delegate
                    {
                        this.newEmployerDataGridView.Rows.Clear();

                        foreach (NewEmployerEntry employerEntry in employers)
                        {
                            this.AddRowToNewEmployerGird(employerEntry);
                        }

                        this.newEmployerDataGridView.Visible = true;
                        this.newEmployerDataGridView.ClearSelection();
                        this.newEmployerDataGridView.CurrentCell = null;
                        this.noNewEmployersMessageLabel.Visible = false;
                    });
            }
        }


        void INewEmployersListView.SelectEmployer(NewEmployerEntry employerEntry)
        {
            ControlExtensions.UseInvokeIfNeeded(this,
                delegate
                {
                    foreach (DataGridViewRow row in this.newEmployerDataGridView.Rows)
                    {
                        if (row.Tag == employerEntry)
                        {
                            row.Selected = true;
                            this.newEmployerDataGridView.CurrentCell = row.Cells[0];
                            return;
                        }
                    }
                });
        }
        #endregion

        protected override void OnPaint(PaintEventArgs e)
        {

            base.OnPaint(e);

            // Draw the vertical seperator
            Pen customPen =
                new Pen(Color.Black);
            Graphics formGraphics =
                this.CreateGraphics();

            int leftBoundary = this.noNewEmployersMessageLabel.Location.X + this.noNewEmployersMessageLabel.Width;
            int rightBoundary = this.masterEmployerListGroupBox.Location.X;

            // This prevents us from having to recalculate the position as we move controls around (hopefully)
            Point startPoint =
                new Point(leftBoundary + (rightBoundary - leftBoundary) / 2,
                           this.masterEmployerListGroupBox.Location.Y);
            Point endPoint =
                new Point(startPoint.X,
                           this.masterEmployerListGroupBox.Location.Y + this.masterEmployerListGroupBox.Height);

            formGraphics.DrawLine(customPen, startPoint, endPoint);
            customPen.Dispose();
            formGraphics.Dispose();

        }


        private void AddRowToNewEmployerGird(NewEmployerEntry employerEntry)
        {
            string employerInfo = this.Presenter.GetFormattedEmployerInfo(employerEntry);

            object[] row = new object[] { employerInfo, employerEntry.Employer.NationalId };

            int rowIndex = this.newEmployerDataGridView.Rows.Add(row);
            this.newEmployerDataGridView.Rows[rowIndex].Tag = employerEntry;
        }


        private void ClearButtonOnClick(object sender, EventArgs e)
        {
            ControlExtensions.UseInvokeIfNeeded(this, delegate { this.Presenter.ClearAddressAndPhoneNumber(); });
        }


        private void DeleteButtonOnClick(object sender, EventArgs e)
        {
            ControlExtensions.UseInvokeIfNeeded(this, delegate { this.Presenter.DeleteSelectedNewEmployer(); });
        }


        private void EditAddressButtonOnClick(object sender, EventArgs e)
        {
            ControlExtensions.UseInvokeIfNeeded(this,
                delegate
                {
                    var currentUser = User.GetCurrent();
                    var facility = currentUser.Facility;
                    var ruleEngine = RuleEngine.GetInstance();
                    
                    var formAddressVerification = new EmployerFormAddressVerification(facility, ruleEngine);
                    NewEmployerEntry employerEntry;
                    try
                    {
                        employerEntry = this.Presenter.SelectedNewEmployerEntry;
                        ContactPoint cp = EmployerHelper.GetFirstContactPointFor(employerEntry.Employer);

                        formAddressVerification.Model = cp.Address.Clone();
                        formAddressVerification.UpdateView();

                        DialogResult dialogresult = formAddressVerification.ShowDialog(this);

                        if (dialogresult == DialogResult.OK)
                        {
                            formAddressVerification.Model = formAddressVerification.i_AddressSelected;
                            this.Presenter.ChangeSelectedNewEmployerAddress(formAddressVerification.i_AddressSelected);
                        }
                    }
                    finally
                    {
                        formAddressVerification.Dispose();
                    }
                });
        }


        private bool IsItemSelectedOnNewEmployerView()
        {
            return this.newEmployerDataGridView.SelectedRows.Count != 0 &&
                   this.newEmployerDataGridView.SelectedRows[0].Tag != null;
        }


        private void MoveAddressAndPhoneButtonOnClick(object sender, EventArgs e)
        {
            ControlExtensions.UseInvokeIfNeeded(this, delegate { this.Presenter.MoveAddressAndPhoneToMasterList(); });
        }


        private void MoveAllInfoButtonOnClick(object sender, EventArgs e)
        {
            ControlExtensions.UseInvokeIfNeeded(this,
                                                 delegate { this.Presenter.MoveSelectedNewEmployerToMasterList(); });
        }


        private void UndoButtonOnClick(object sender, EventArgs e)
        {
            ControlExtensions.UseInvokeIfNeeded(this, delegate { this.Presenter.Undo(); });
        }


        private void CancelButtonOnClick(object sender, EventArgs e)
        {
            ControlExtensions.UseInvokeIfNeeded(this,
                delegate
                {
                    if (this.featureIsLockedMessageLabel.Visible)
                    {
                        this.CancelActivity();
                    }
                    else
                    {
                        DialogResult result = MessageBox.Show(UIErrorMessages.LEAVING_ACTIVITY_MSG,
                            UIErrorMessages.LEAVING_ACTIVITY_TITLE,
                            MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);

                        if (result == DialogResult.Yes)
                        {
                            this.CancelActivity();
                        }
                    }
                });
        }

        private void CancelActivity()
        {
            ActivityEventAggregator.GetInstance().RaiseActivityCancelEvent(this, EventArgs.Empty);
            ActivityEventAggregator.GetInstance().RaiseReturnToMainScreen(this, EventArgs.Empty);
        }

        private void MasterEmployerSearchButtonOnClick(object sender, EventArgs e)
        {
            this.SearchForEmployerInMasterList();
        }


        private void SearchForEmployerInMasterList()
        {
            ControlExtensions.UseInvokeIfNeeded(this,
                delegate{
                
                    this.Presenter.SearchForEmployerInMasterList(this.masterEmployerSearchTextBox.Text);

                });
        }


        private void SelectedEmployerPhoneTextBoxOnValidating(object sender, CancelEventArgs e)
        {
            ControlExtensions.UseInvokeIfNeeded(this,
                delegate
                {
                    if (!string.IsNullOrEmpty(this.selectedEmployerPhoneTextBox.Text))
                    {
                        if (!this.Presenter.IsPhoneNumberValid(this.selectedEmployerPhoneTextBox.Text))
                        {
                            UIColors.SetErrorBgColor(this.selectedEmployerPhoneTextBox);

                            MessageBox.Show(
                                INCOMPLETE_PHONE_NUMBER_MSG,
                                ERROR_MESSAGE_CAPTION, MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation,
                                MessageBoxDefaultButton.Button1);

                            UIColors.SetNormalBgColor(this.selectedEmployerPhoneTextBox);
                            e.Cancel = true;
                            this.SnapCaretToEndOfTextForUSPhoneNumberMask(this.selectedEmployerPhoneTextBox);
                            this.selectedEmployerPhoneTextBox.Focus();
                        }
                    }
                });
        }


        private void SelectedEmployerPhoneTextBoxOnMaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {
            ControlExtensions.UseInvokeIfNeeded(this,
                delegate
                {
                    if (!selectedEmployerPhoneTextBox.MaskCompleted)
                    {
                        this.phoneNumberErrorToolTip.Show(
                            INVALID_INPUT_MSG,
                            selectedEmployerPhoneTextBox, 2000);
                    }
                });
        }


        private void FinishButtonOnClick(object sender, EventArgs e)
        {
            LooseArgs args = new LooseArgs(null);
            this.Presenter.Save();
            ActivityEventAggregator.GetInstance().RaiseActivityCompleteEvent(this, args);
            ActivityEventAggregator.GetInstance().RaiseReturnToMainScreen(this, args);
        }

        private void MasterEmployerListDataGridViewOnSelectionChanged(object sender, EventArgs e)
        {
            ControlExtensions.UseInvokeIfNeeded(this,
                delegate
                {
                    if (this.IsANewEmployerSelected())
                    {
                        Employer employer = (Employer)this.masterEmployerListDataGridView.SelectedRows[0].Tag;
                        this.Presenter.SelectEmployerOnMasterEmployerView(employer);
                    }
                });
        }


        private bool IsANewEmployerSelected()
        {
            return this.masterEmployerListDataGridView.SelectedRows.Count != 0 && this.masterEmployerListDataGridView.SelectedRows[0].Tag != null;
        }

        private void MasterEmployerSearchTextBoxOnTextChanged(object sender, EventArgs e)
        {
            this.Presenter.SearchStringChanged();
        }

        private void SelectedEmployerNameTextBoxOnValidating(object sender, CancelEventArgs e)
        {
            ControlExtensions.UseInvokeIfNeeded(this,
                delegate
                {
                    if (!this.Presenter.IsEmployerNameValid(this.selectedEmployerNameTextBox.Text))
                    {

                        UIColors.SetErrorBgColor(this.selectedEmployerNameTextBox);
                        MessageBox.Show(
                            BLANK_EMPLOYER_NAME_MESSAGE,
                            ERROR_MESSAGE_CAPTION, MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation,
                            MessageBoxDefaultButton.Button1);

                        UIColors.SetNormalBgColor(this.selectedEmployerNameTextBox);
                        e.Cancel = true;

                        this.selectedEmployerNameTextBox.Focus();
                    }
                });

        }

        private void MasterEmployerAddressesDataGridViewOnEnter(object sender, EventArgs e)
        {
            DataGridViewRowCollection rows = this.masterEmployerAddressesDataGridView.Rows;
            if (rows != null && rows.Count > 0)
            {
                rows[0].Selected = true;
            }
        }

        private void SelectedEmployerNameTextBoxOnValidated(object sender, EventArgs e)
        {
            this.Presenter.SelectedEmployerNameChanged();
        }

        private void SelectedEmployerNationalIdTextBoxOnValidated(object sender, EventArgs e)
        {
            this.Presenter.SelectedEmployerNationalIdChanged();
        }

        private void SelectedEmployerPhoneTextBoxOnValidated(object sender, EventArgs e)
        {
            this.Presenter.SelectedEmployerPhoneChanged();
        }

        private void NewEmployerDataGridViewOnSelectionChanged(object sender, EventArgs e)
        {
            ControlExtensions.UseInvokeIfNeeded(this,
                delegate
                {
                    if (this.IsItemSelectedOnNewEmployerView())
                    {
                        NewEmployerEntry employer = (NewEmployerEntry)this.newEmployerDataGridView.SelectedRows[0].Tag;
                        this.Presenter.SelectNewEmployerAndSearchMasterEmployerList(employer);
                    }
                });
        }

        private void NewEmployerDataGridViewOnRowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            this.DisableFocusCueRectangle(e);
        }




        private void DisableFocusCueRectangle(DataGridViewRowPrePaintEventArgs e)
        {
            e.PaintParts &= ~DataGridViewPaintParts.Focus;
        }

        private void SelectedEmployerNationalIdTextBoxOnClick(object sender, EventArgs e)
        {
            this.SnapCaretToEndOfText((MaskedTextBox)sender);
        }


        private void SnapCaretToEndOfText(TextBoxBase maskedTextBox)
        {
            if (!String.IsNullOrEmpty(maskedTextBox.Text))
            {
                if (maskedTextBox.SelectionStart > maskedTextBox.Text.Length)
                {
                    maskedTextBox.SelectionStart = maskedTextBox.Text.Length;
                    maskedTextBox.SelectionLength = 0;
                }
            }

            else
            {
                maskedTextBox.SelectionStart = 0;
                maskedTextBox.SelectionLength = 0;
            }
        }

        private void SelectedEmployerPhoneTextBoxOnClick(object sender, EventArgs e)
        {
            MaskedTextBox maskedTextBox = (MaskedTextBox)sender;

            this.SnapCaretToEndOfTextForUSPhoneNumberMask(maskedTextBox);
        }


        /// <summary>
        /// Snaps the caret to end of text for US phone number mask.
        /// </summary>
        /// <param name="maskedTextBox">The masked text box.</param>
        /// <exception cref="InvalidOperationException">If the MaskedTexBox mask is set to anything other than '(000)000-0000'</exception>
        private void SnapCaretToEndOfTextForUSPhoneNumberMask(MaskedTextBox maskedTextBox)
        {
            if (maskedTextBox.Mask != "(000) 000-0000")
            {
                throw new InvalidOperationException(@"This method only works when the mask is set to '(000)000-0000'");
            }

            if (maskedTextBox.Text.Length == 0)
            {
                maskedTextBox.SelectionStart = 0;
            }

            if (maskedTextBox.Text.Length == 1 && maskedTextBox.SelectionStart > 2)
            {
                maskedTextBox.SelectionStart = 2;
            }

            if (maskedTextBox.Text.Length == 2 && maskedTextBox.SelectionStart > 3)
            {
                maskedTextBox.SelectionStart = 3;
            }

            if (maskedTextBox.Text.Length == 3 && maskedTextBox.SelectionStart > 5)
            {
                maskedTextBox.SelectionStart = 5;
            }

            if (maskedTextBox.Text.Length == 4 && maskedTextBox.SelectionStart > 7)
            {
                maskedTextBox.SelectionStart = 7;
            }

            if (maskedTextBox.Text.Length == 5 && maskedTextBox.SelectionStart > 8)
            {
                maskedTextBox.SelectionStart = 8;
            }

            if (maskedTextBox.Text.Length == 6 && maskedTextBox.SelectionStart > 10)
            {
                maskedTextBox.SelectionStart = 10;
            }

            if (maskedTextBox.Text.Length == 7 && maskedTextBox.SelectionStart > 11)
            {
                maskedTextBox.SelectionStart = 11;
            }

            if (maskedTextBox.Text.Length == 8 && maskedTextBox.SelectionStart > 12)
            {
                maskedTextBox.SelectionStart = 12;
            }

            if (maskedTextBox.Text.Length == 9 && maskedTextBox.SelectionStart > 13)
            {
                maskedTextBox.SelectionStart = 13;
            }
        }


        private void SelectedEmployerNameTextBoxOnClick(object sender, EventArgs e)
        {
            MaskedEditTextBox maskedTextBox = (MaskedEditTextBox)sender;
            this.SnapCaretToEndOfText(maskedTextBox);
        }


        private void MasterEmployerSearchTextBoxOnClick(object sender, EventArgs e)
        {
            this.SnapCaretToEndOfText((MaskedTextBox)sender);
        }


        private void MasterEmployerSearchTextBoxOnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                if (this.Presenter.IsSearchStringValid(this.masterEmployerSearchTextBox.Text) && this.masterEmployerSearchTextBox.MaskCompleted)
                {
                    this.SearchForEmployerInMasterList();
                }
            }
        }

        private void ConfigureControls()
        {
            MaskedEditTextBoxBuilder.ConfigureFirstNameAndLastName(selectedEmployerNameTextBox);
        }
    }

    public delegate void Action();
}
