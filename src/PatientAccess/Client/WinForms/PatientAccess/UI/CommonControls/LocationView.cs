using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.PersistenceCommon;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI.FindBedViews;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.CommonControls
{
    public class LocationView : ControlView
    {
        #region Events
        public event EventHandler BedSelected;
        #endregion

        #region EventHandlers

        private void Button_Verify_Click(object sender, EventArgs e)
        {
            if (this.textBox_NursingStation.UnMaskedText.Length > 0 &&
                this.textBox_RoomNumber.UnMaskedText.Length > 0 &&
                this.textBox_Bed.UnMaskedText.Length > 0)
            {
                Cursor cursorSave = this.Cursor;
                this.Cursor = Cursors.WaitCursor;

                if (!this.CheckForRequiredFields())
                {
                    this.Cursor = Cursors.Default;
                    return;
                }

                if (this.CheckIfDuplicatesFound())
                {
                    this.Cursor = Cursors.Default;
                    return;
                }

                try
                {
                    VerifyAndReserveBed();
                }
                finally
                {
                    this.Cursor = cursorSave;
                }
            }
            else
            {

                MessageBox.Show(UIErrorMessages.LOCATION_MISSING_ELEMENTS_ERRMSG, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1);

                this.textBox_Bed.Focus();
            }
        }

        private void Button_FindBed_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            if (!this.CheckForRequiredFields())
            {
                this.Cursor = Cursors.Default;
                return;
            }

            if (this.CheckIfDuplicatesFound())
            {
                this.Cursor = Cursors.Default;
                return;
            }

            FindBedForm findBedForm = new FindBedForm();
            ActivityEventAggregator.GetInstance().ActivityCancelled += CloseForm( findBedForm );
            try
            {
                if (findBedForm.ShowDialog(Model) == DialogResult.OK)
                {
                    if (findBedForm.NewLocation != null)
                    {
                        this.ResetAll();
                        Model.Location = findBedForm.NewLocation;
                        OnBedSelected();
                    }
                }
            }

            finally
            {
                this.Cursor = Cursors.Default;
                findBedForm.Dispose();
            }

            this.RunRules();
        }

        private static EventHandler CloseForm( Form form )
        {
            return delegate
                   {
                       if( form.IsHandleCreated && !form.IsDisposed )
                       {
                           form.Close();
                       }
                   };
        }

        private void TextBox_NursingStation_TextChanged(object sender, EventArgs e)
        {
            if (textBox_NursingStation.TextLength == 2)
            {
                textBox_RoomNumber.Focus();
            }
        }

        private void TextBox_RoomNumber_TextChanged(object sender, EventArgs e)
        {
            if (textBox_RoomNumber.TextLength == 4)
            {
                textBox_Bed.Focus();
            }
        }

        private void TextBox_Bed_TextChanged(object sender, EventArgs e)
        {
            if (textBox_Bed.TextLength == 1)
            {
                button_Verify.Focus();
            }
        }

        private void TextBox_RoomNumber_Validating(object sender, CancelEventArgs e)
        {
            if (textBox_RoomNumber.UnMaskedText.Length >= 1 &&
                textBox_RoomNumber.UnMaskedText.Length <= 3)
            {
                textBox_RoomNumber.UnMaskedText = textBox_RoomNumber.UnMaskedText.PadLeft(4, '0');
            }
        }
        
        private void BedFieldEnter(object sender, EventArgs e)
        {
            this.AcceptButton = button_Verify;
        }

        private void BedFieldLeave(object sender, EventArgs e)
        {
            this.AcceptButton = button_FindBed;
        }

        private void LocationRequiredEventHandler(object sender, EventArgs e)
        {
            UIColors.SetRequiredBgColor(this.field_AssignedBed);
        }

        private void LocationPreferredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetPreferredBgColor( this.field_AssignedBed );
        }

        private void LocationView_Leave(object sender, EventArgs e)
        {
            this.AcceptButton = null;
            this.Refresh();
        }

        #endregion

        #region Methods

        /// <summary>
        /// ReleaseReservedBed - the user has canceled the action or closed the window... release the reserved
        /// bed if it was reserved this session
        /// </summary>

        public void ReleaseReservedBed()
        {
            if (this.i_NewLocationReserved)
            {
                Location location = this.Model.Location;
                if (location != null && location.NursingStation != null && location.Room != null && location.Bed != null)
                {
                    LocationBrokerProxy locationPBARBroker = new LocationBrokerProxy();
                    locationPBARBroker.ReleaseReservedBed(this.Model.Location, User.GetCurrent().Facility);
                    this.Model.Location = new Location();
                }
            }
        }

        public override void UpdateView()
        {
            this.RegisterRulesEvents();

            if ((Model != null) && (Model.Location != null))
            {
                if (Model.Location.PrintString == string.Empty)
                {
                    field_AssignedBed.Text = "          ";
                }
                else
                {
                    field_AssignedBed.Text = Model.Location.PrintString;
                }

                textBox_NursingStation.Text = String.Empty;         
                textBox_RoomNumber.Text = String.Empty;             
                textBox_Bed.Text = String.Empty;                    

                button_Verify.Enabled = true;
                button_FindBed.Enabled = true;
            }

            this.RunRules();
        }

        public void Reset()
        {
            //  TODO - Location Refactor- clear model data
            this.ReleaseReservedBed();
            this.ResetAll();
            this.button_FindBed.Enabled = false;
            this.button_Verify.Enabled = false;
            this.i_NewLocationReserved = false;
        }

        public void DisableLocationControls()
        {
            UIColors.SetNormalBgColor(this.field_AssignedBed);
            button_Verify.Enabled = false;
            button_FindBed.Enabled = false;
            textBox_NursingStation.Enabled = false;
            textBox_RoomNumber.Enabled = false;
            textBox_Bed.Enabled = false;
        }

        public void EnableLocationControls()
        {
            button_Verify.Enabled = true;
            button_FindBed.Enabled = true;
            textBox_NursingStation.Enabled = true;
            textBox_RoomNumber.Enabled = true;
            textBox_Bed.Enabled = true;
        }


        public void SedBedBackgroundError()
        {
            UIColors.SetErrorBgColor(this.textBox_Bed);
        }

        public void SetBedBackgroundNormal()
        {
            UIColors.SetNormalBgColor(this.textBox_Bed);
        }
        #endregion

        #region Properties

        public new Account Model
        {
            get
            {
                return i_Account;
            }
            set
            {
                i_Account = value;
                //                UpdateView();
            }
        }

        public string EditVerifyButtonText
        {   // Allows the setting of custom text
            get
            {
                return this.button_Verify.Text;
            }
            set
            {
                this.button_Verify.Text = value;
            }
        }

        public string EditFindButtonText
        {   // Allows the setting of custom text
            get
            {
                return this.button_FindBed.Text;
            }
            set
            {
                this.button_FindBed.Text = value;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Determines if there are duplicate bed assignments
        /// </summary>
        /// <returns>true if dupes were found, else false</returns>
        private bool CheckIfDuplicatesFound()
        {
            if (Model == null
                || Model.Patient == null)
            {
                return true;
            }
           
            long parmAccount = 0;

            if (!(Model.Activity is RegistrationActivity ||
                Model.Activity is ActivatePreRegistrationActivity ||
                Model.Activity is TransferOutToInActivity ||
                Model.Activity is TransferActivity ||
                Model.Activity is TransferERToOutpatientActivity))
            {
                return false;
            }

            // if we are transferring, then the current account gets excluded from the check

            if (Model.Activity is TransferOutToInActivity ||
                Model.Activity is TransferActivity ||
                Model.Activity is TransferERToOutpatientActivity )
            {
                parmAccount = this.Model.AccountNumber;
            }

            ILocationBroker locationBroker = BrokerFactory.BrokerOfType<ILocationBroker>();

            string zip = string.Empty;
            ContactPoint contactPoint = this.Model.Patient.ContactPointWith(TypeOfContactPoint.NewMailingContactPointType());
            if (contactPoint != null)
            {
                zip = contactPoint.Address.ZipCode.ZipCodePrimary;
            }

            DuplicateLocationResult duplicateLocationResult = locationBroker.CheckForDuplicateBedAssignments(this.Model.Facility,
                                                this.Model.Patient.LastName,
                                                this.Model.Patient.FirstName,
                                                parmAccount,
                                                this.Model.Patient.MedicalRecordNumber,
                                                this.Model.Patient.SocialSecurityNumber,
                                                this.Model.Patient.DateOfBirth,
                                                zip);

            if (duplicateLocationResult.dupeStatus != DuplicateBeds.NoDupes &&
                duplicateLocationResult.accounts.Count > 0)
            {
                //display a message box with dupes (matches or potential)
                if ( Model.Facility.IsDuplicateBedsAllowed &&
                    duplicateLocationResult.dupeStatus == DuplicateBeds.MatchedDupes )
                {
                    duplicateLocationResult.dupeStatus = DuplicateBeds.AllowDupes;
                }

                if (duplicateLocationResult.dupeStatus == DuplicateBeds.MatchedDupes)
                {
                    using (DupeBedAssignmentWarning warningBox = new DupeBedAssignmentWarning(duplicateLocationResult))
                    {
                        warningBox.ShowDialog();
                    }
                    return true;
                }
                else if (duplicateLocationResult.dupeStatus == DuplicateBeds.AllowDupes)
                {
                    DialogResult dialogResult;
                    using (DupeBedAssignmentWarning warningBox = new DupeBedAssignmentWarning(duplicateLocationResult))
                    {
                        dialogResult = warningBox.ShowDialog();
                    }
                    if (dialogResult != DialogResult.Yes)
                    {
                        return true;
                    }
                }
                else
                {
                    DialogResult dialogResult;
                    using (DupeBedAssignmentWarning warningBox = new DupeBedAssignmentWarning(duplicateLocationResult))
                    {
                        dialogResult = warningBox.ShowDialog();
                    }
                    if (dialogResult != DialogResult.Yes)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// CheckForRequiredFields - display required fields warning if missing fields to determine dupe bed assignment
        /// </summary>
        /// <returns>false if missing fields, else true</returns>
        private bool CheckForRequiredFields()
        {
            RuleEngine.GetInstance().ClearActions();

            if (this.Model.Activity is RegistrationActivity)
            {

                string requiredFields = RuleEngine.GetInstance().GetCompositeSummary(this.Model, null, LOCATION_FIELDS);

                if (requiredFields != String.Empty)
                {
                    AccountView.GetInstance().DisplayLocationRequiredFieldSummary();
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }

        private void VerifyAndReserveBed()
        {
            LocationBrokerProxy locationPBARBroker = new LocationBrokerProxy();

            Location alocation = CreateLocationFromFields();

            string locationStatus = locationPBARBroker.ValidateLocation(alocation, User.GetCurrent().Facility);
            string message = String.Empty;
            switch (locationStatus)
            {

                case "Invalid Nursingstation":
                    {
                        message = MSG_NS_DOESNT_EXIST;
                        break;
                    }
                case "Invalid Room":
                    {
                        message = MSG_ROOM_DOESNT_EXIST;
                        break;
                    }
                case "Invalid Bed":
                    {
                        message = MSG_BED_DOESNT_EXIST;
                        break;
                    }
            }
            if (message != String.Empty)
            {

                MessageBox.Show(
                    message,
                    "Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }


            ValidateRequestedBedFields();

            if (this.IsValid)
            {
                if (Model.KindOfVisit.Code == VisitType.INPATIENT
                    && (alocation.Bed.Accomodation == null
                            || alocation.Bed.Accomodation.Code.Trim() == string.Empty))
                {
                    ArrayList accCodes = (ArrayList)locationPBARBroker.AccomodationCodesFor(this.textBox_NursingStation.Text.Trim(), this.Model.Facility);

                    if (accCodes.Count <= 1)
                    {
                        MessageBox.Show(
                            UIErrorMessages.REQUESTED_BED_NO_ACCOMODATION,
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);

                        return;
                    }
                }

                ReservationCriteria reservationCriteria = new ReservationCriteria
                    (PersistentModel.NEW_OID
                    , PersistentModel.NEW_VERSION
                    , Model.Location
                    , alocation
                    , User.GetCurrent().Facility
                    , Model.KindOfVisit
                    );
                
                try
                {
                    ReservationResults reservationResults = locationPBARBroker.Reserve(reservationCriteria);

                    message = reservationResults.Message;

                    MessageBox.Show(
                        message,
                        "Information",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    
                    if (message.Trim().Equals(ReservationResults.MSG_RESERVED.Trim()))
                    {
                        this.textBox_Bed.Focus();
                    }

                    if (reservationResults.ReservationSucceeded)
                    {
                        //Raise bed lock event to patientAccess view
                        ActivityEventAggregator.GetInstance().RaiseBedLockEvent(this,
                            new LooseArgs(reservationResults.Location));

                        Model.Location = reservationResults.Location;
                        //all location entry fields must be cleared after successful reservation
                        this.ResetAll();
                        OnBedSelected();
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show(LOCATION_RESERVATION_FAILED, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            if ((Model != null) && (Model.Location != null))
            {
                if (Model.Location.PrintString == string.Empty)
                {
                    field_AssignedBed.Text = "          ";
                }
                else
                {
                    field_AssignedBed.Text = Model.Location.PrintString;
                }
            }

            this.RunRules();
        }

        private Location CreateLocationFromFields()
        {
            NursingStation ns = new NursingStation(PersistentModel.NEW_OID,
                PersistentModel.NEW_VERSION, string.Empty, this.textBox_NursingStation.Text);
            Room room = new Room(PersistentModel.NEW_OID,
                PersistentModel.NEW_VERSION, string.Empty, this.textBox_RoomNumber.Text);
            Bed bed = new Bed(PersistentModel.NEW_OID,
                PersistentModel.NEW_VERSION, string.Empty, this.textBox_Bed.Text);
            Location alocation = new Location(PersistentModel.NEW_OID,
                PersistentModel.NEW_VERSION, string.Empty, string.Empty, ns, room, bed);
            return alocation;
        }

        private void OnBedSelected()
        {
            field_AssignedBed.Text = Model.Location.ToString();
            if (BedSelected != null)
            {
                BedSelected(this, new LooseArgs(this.Model.Location));
                this.i_NewLocationReserved = true;
            }
        }

        private void ResetAll()
        {
            textBox_NursingStation.Clear();
            textBox_RoomNumber.Clear();
            textBox_Bed.Clear();
            field_AssignedBed.Text = String.Empty;

            UIColors.SetNormalBgColor(this.textBox_NursingStation);
            UIColors.SetNormalBgColor(this.textBox_RoomNumber);
            UIColors.SetNormalBgColor(this.textBox_Bed);
            UIColors.SetNormalBgColor(this.field_AssignedBed);
        }

        /// <summary>
        /// Validate Requested Bed Fields
        /// </summary>
        private void ValidateRequestedBedFields()
        {
            UIColors.SetNormalBgColor(this.textBox_Bed);
            UIColors.SetNormalBgColor(this.textBox_NursingStation);
            UIColors.SetNormalBgColor(this.textBox_RoomNumber);

            UIColors.SetNormalBgColor(field_AssignedBed);

            if ((this.textBox_NursingStation.Text != String.Empty &&
                this.textBox_RoomNumber.Text != String.Empty &&
                this.textBox_Bed.Text != String.Empty))
            {
                // valid
                IsValid = true;
            }
            else if (this.textBox_NursingStation.Text.Equals(String.Empty) &&
                this.textBox_RoomNumber.Text.Equals(String.Empty) &&
                this.textBox_Bed.Text.Equals(String.Empty))
            {
                if (Model.Location != null && Model.Location.PrintString == string.Empty)
                {
                    field_AssignedBed.Text = "          ";
                }
                this.SetRequiredBgColor(this.field_AssignedBed);
                this.field_AssignedBed.Visible = true;

                this.textBox_NursingStation.Focus();

                IsValid = false;
            }
            else
            {
                IsValid = false;
                MessageBox.Show(LOCATION_INCOMPLETE_ERRMSG, "Error",MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Sets the background color to the required field color for the control passed.
        /// </summary>
        /// <param name="field"></param>
        private void SetRequiredBgColor(Control field)
        {
            UIColors.SetNormalBgColor(field);
            Refresh();
        }

        private void RegisterRulesEvents()
        {
            RuleEngine.GetInstance().RegisterEvent(typeof(LocationRequired), this.Model, new EventHandler(LocationRequiredEventHandler));
            RuleEngine.GetInstance().RegisterEvent( typeof( LocationPreferred ), this.Model, new EventHandler( LocationPreferredEventHandler ) );
        }

        private void UnRegisterRulesEvents()
        {
            RuleEngine.GetInstance().UnregisterEvent(typeof(LocationRequired), this.Model, this.LocationRequiredEventHandler);
            RuleEngine.GetInstance().UnregisterEvent( typeof( LocationPreferred ), this.Model, new EventHandler( LocationPreferredEventHandler ) );
        }

        public void RunRules()
        {
            // reset all fields that might have error, preferred, or required backgrounds

            UIColors.SetNormalBgColor(this.field_AssignedBed);
            this.Refresh();

            RuleEngine.GetInstance().EvaluateRule(typeof(LocationRequired), this.Model);
            RuleEngine.GetInstance().EvaluateRule( typeof( LocationPreferred ), this.Model );
        }

        protected override void Dispose(bool disposing)
        {
            this.UnRegisterRulesEvents();

            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.field_AssignedBed = new System.Windows.Forms.Label();
            this.textBox_NursingStation = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.textBox_RoomNumber = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.textBox_Bed = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.label_NS = new System.Windows.Forms.Label();
            this.label_Room = new System.Windows.Forms.Label();
            this.label_Bed = new System.Windows.Forms.Label();
            this.label_RequestedBed = new System.Windows.Forms.Label();
            this.button_Verify = new LoggingButton();
            this.label_FindBed = new System.Windows.Forms.Label();
            this.button_FindBed = new LoggingButton();
            this.groupBox_Location = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblDash1 = new System.Windows.Forms.Label();
            this.label_AssignedBed = new System.Windows.Forms.Label();
            this.groupBox_Location.SuspendLayout();
            this.SuspendLayout();
            // 
            // field_AssignedBed
            // 
            this.field_AssignedBed.BackColor = System.Drawing.Color.Transparent;
            this.field_AssignedBed.Location = new System.Drawing.Point(100, 26);
            this.field_AssignedBed.Name = "field_AssignedBed";
            this.field_AssignedBed.Size = new System.Drawing.Size(65, 15);
            this.field_AssignedBed.TabIndex = 0;
            this.field_AssignedBed.Text = "XX-XXXX-X";
            this.field_AssignedBed.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBox_NursingStation
            // 
            this.textBox_NursingStation.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBox_NursingStation.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.textBox_NursingStation.Location = new System.Drawing.Point(100, 49);
            this.textBox_NursingStation.Mask = "";
            this.textBox_NursingStation.MaxLength = 2;
            this.textBox_NursingStation.Name = "textBox_NursingStation";
            this.textBox_NursingStation.Size = new System.Drawing.Size(28, 20);
            this.textBox_NursingStation.TabIndex = 1;
            this.textBox_NursingStation.Leave += new System.EventHandler(this.BedFieldLeave);
            this.textBox_NursingStation.TextChanged += new System.EventHandler(this.TextBox_NursingStation_TextChanged);
            this.textBox_NursingStation.Enter += new System.EventHandler(this.BedFieldEnter);
            // 
            // textBox_RoomNumber
            // 
            this.textBox_RoomNumber.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBox_RoomNumber.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.textBox_RoomNumber.KeyPressExpression = "^\\d*$";
            this.textBox_RoomNumber.Location = new System.Drawing.Point(142, 49);
            this.textBox_RoomNumber.Mask = "";
            this.textBox_RoomNumber.MaxLength = 4;
            this.textBox_RoomNumber.Name = "textBox_RoomNumber";
            this.textBox_RoomNumber.Size = new System.Drawing.Size(43, 20);
            this.textBox_RoomNumber.TabIndex = 2;
            this.textBox_RoomNumber.ValidationExpression = "^\\d*$";
            this.textBox_RoomNumber.Leave += new System.EventHandler(this.BedFieldLeave);
            this.textBox_RoomNumber.Validating += new System.ComponentModel.CancelEventHandler(this.TextBox_RoomNumber_Validating);
            this.textBox_RoomNumber.TextChanged += new System.EventHandler(this.TextBox_RoomNumber_TextChanged);
            this.textBox_RoomNumber.Enter += new System.EventHandler(this.BedFieldEnter);
            // 
            // textBox_Bed
            // 
            this.textBox_Bed.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBox_Bed.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.textBox_Bed.Location = new System.Drawing.Point(201, 49);
            this.textBox_Bed.Mask = "";
            this.textBox_Bed.MaxLength = 1;
            this.textBox_Bed.Name = "textBox_Bed";
            this.textBox_Bed.Size = new System.Drawing.Size(28, 20);
            this.textBox_Bed.TabIndex = 3;
            this.textBox_Bed.Leave += new System.EventHandler(this.BedFieldLeave);
            this.textBox_Bed.TextChanged += new System.EventHandler(this.TextBox_Bed_TextChanged);
            this.textBox_Bed.Enter += new System.EventHandler(this.BedFieldEnter);
            // 
            // label_NS
            // 
            this.label_NS.AutoSize = true;
            this.label_NS.Location = new System.Drawing.Point(102, 70);
            this.label_NS.Name = "label_NS";
            this.label_NS.Size = new System.Drawing.Size(20, 16);
            this.label_NS.TabIndex = 0;
            this.label_NS.Text = "NS";
            // 
            // label_Room
            // 
            this.label_Room.AutoSize = true;
            this.label_Room.Location = new System.Drawing.Point(144, 70);
            this.label_Room.Name = "label_Room";
            this.label_Room.Size = new System.Drawing.Size(34, 16);
            this.label_Room.TabIndex = 0;
            this.label_Room.Text = "Room";
            // 
            // label_Bed
            // 
            this.label_Bed.AutoSize = true;
            this.label_Bed.Location = new System.Drawing.Point(203, 70);
            this.label_Bed.Name = "label_Bed";
            this.label_Bed.Size = new System.Drawing.Size(24, 16);
            this.label_Bed.TabIndex = 0;
            this.label_Bed.Text = "Bed";
            // 
            // label_RequestedBed
            // 
            this.label_RequestedBed.AutoSize = true;
            this.label_RequestedBed.Location = new System.Drawing.Point(10, 52);
            this.label_RequestedBed.Name = "label_RequestedBed";
            this.label_RequestedBed.Size = new System.Drawing.Size(84, 16);
            this.label_RequestedBed.TabIndex = 0;
            this.label_RequestedBed.Text = "Requested bed:";
            // 
            // button_Verify
            // 
            this.button_Verify.Location = new System.Drawing.Point(244, 47);
            this.button_Verify.Name = "button_Verify";
            this.button_Verify.TabIndex = 4;
            this.button_Verify.Text = "Verify";
            this.button_Verify.Click += new System.EventHandler(this.Button_Verify_Click);
            // 
            // label_FindBed
            // 
            this.label_FindBed.AutoSize = true;
            this.label_FindBed.Location = new System.Drawing.Point(10, 94);
            this.label_FindBed.Name = "label_FindBed";
            this.label_FindBed.Size = new System.Drawing.Size(49, 16);
            this.label_FindBed.TabIndex = 0;
            this.label_FindBed.Text = "Find Bed";
            // 
            // button_FindBed
            // 
            this.button_FindBed.Location = new System.Drawing.Point(98, 89);
            this.button_FindBed.Name = "button_FindBed";
            this.button_FindBed.TabIndex = 5;
            this.button_FindBed.Text = "Find...";
            this.button_FindBed.Click += new System.EventHandler(this.Button_FindBed_Click);
            // 
            // groupBox_Location
            // 
            this.groupBox_Location.Controls.Add(this.label1);
            this.groupBox_Location.Controls.Add(this.lblDash1);
            this.groupBox_Location.Controls.Add(this.textBox_RoomNumber);
            this.groupBox_Location.Controls.Add(this.label_FindBed);
            this.groupBox_Location.Controls.Add(this.label_Bed);
            this.groupBox_Location.Controls.Add(this.button_FindBed);
            this.groupBox_Location.Controls.Add(this.button_Verify);
            this.groupBox_Location.Controls.Add(this.label_Room);
            this.groupBox_Location.Controls.Add(this.textBox_Bed);
            this.groupBox_Location.Controls.Add(this.label_RequestedBed);
            this.groupBox_Location.Controls.Add(this.textBox_NursingStation);
            this.groupBox_Location.Controls.Add(this.field_AssignedBed);
            this.groupBox_Location.Controls.Add(this.label_AssignedBed);
            this.groupBox_Location.Controls.Add(this.label_NS);
            this.groupBox_Location.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox_Location.Location = new System.Drawing.Point(0, 0);
            this.groupBox_Location.Name = "groupBox_Location";
            this.groupBox_Location.Size = new System.Drawing.Size(354, 133);
            this.groupBox_Location.TabIndex = 0;
            this.groupBox_Location.TabStop = false;
            this.groupBox_Location.Text = "Location";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(189, 51);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(8, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "-";
            // 
            // lblDash1
            // 
            this.lblDash1.Location = new System.Drawing.Point(131, 51);
            this.lblDash1.Name = "lblDash1";
            this.lblDash1.Size = new System.Drawing.Size(8, 16);
            this.lblDash1.TabIndex = 0;
            this.lblDash1.Text = "-";
            // 
            // label_AssignedBed
            // 
            this.label_AssignedBed.AutoSize = true;
            this.label_AssignedBed.Location = new System.Drawing.Point(10, 26);
            this.label_AssignedBed.Name = "label_AssignedBed";
            this.label_AssignedBed.Size = new System.Drawing.Size(76, 16);
            this.label_AssignedBed.TabIndex = 0;
            this.label_AssignedBed.Text = "Assigned bed:";
            // 
            // LocationView
            // 
            this.Controls.Add(this.groupBox_Location);
            this.Name = "LocationView";
            this.Size = new System.Drawing.Size(354, 133);
            this.Leave += new System.EventHandler(this.LocationView_Leave);
            this.groupBox_Location.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        #endregion

        #region Private Properties

        private bool IsValid
        {
            get
            {
                return isValid;
            }
            set
            {
                isValid = value;
            }
        }
        
        #endregion

        #region Construction and Finalization
        public LocationView()
        {
            InitializeComponent();

            Reset();

        }

        #endregion

        #region Data Elements
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private Container components = null;

        private MaskedEditTextBox textBox_NursingStation;
        private MaskedEditTextBox textBox_RoomNumber;
        private MaskedEditTextBox textBox_Bed;

        private LoggingButton button_Verify;
        private LoggingButton button_FindBed;

        private GroupBox groupBox_Location;

        public Label field_AssignedBed;
        private Label label_NS;
        private Label label_Room;
        private Label label_Bed;
        private Label label_RequestedBed;
        private Label label_FindBed;
        private Label lblDash1;
        private Label label1;
        private Label label_AssignedBed;


        private Account i_Account = null;

        private bool isValid;
        private bool i_NewLocationReserved = false;

        #endregion

        #region Constants

        private const string LOCATION_INCOMPLETE_ERRMSG = "The request for a bed assignment cannot proceed until all three location elements are specified.";
        private const string LOCATION_RESERVATION_FAILED = "The system was unable to reserve the requested bed.  \nTry your request again, or contact the Tenet Help Desk \nat 1-800-639-7575 for assistance.";
        private const string MSG_NS_DOESNT_EXIST = " The requested nursing station does not exist in the system.";
        private const string MSG_ROOM_DOESNT_EXIST = " The requested room is invalid for the specified nursing station.";
        private const string MSG_BED_DOESNT_EXIST = " The requested bed is invalid for the specified room.";

        private const string LOCATION_FIELDS = "LocationFields";

        #endregion
    }
}
