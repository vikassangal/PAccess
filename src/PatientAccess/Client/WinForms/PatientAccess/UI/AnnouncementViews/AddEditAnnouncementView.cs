using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.SecurityService.Domain;
using Extensions.UI.Winforms;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.AnnouncementViews
{
    public class AddEditAnnouncementView : ControlView
    {
        #region Event Handlers
        private void groupsAllCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            this.groupsListBox.ClearSelected();
            CheckForDataEntered();
            if( this.groupsAllCheckBox.Checked )
            {
                this.groupsListBox.Enabled = false;
            }
            else
            {
                this.groupsListBox.Enabled = true;
            }
        }
        private void facilitiesAllCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            this.facilitiesListBox.ClearSelected();
            CheckForDataEntered();
            if( this.facilitiesAllCheckBox.Checked )
            {
                this.facilitiesListBox.Enabled = false;
            }
            else
            {
                this.facilitiesListBox.Enabled = true;
            }      
        }
        private void dateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            if( this.dateTimePicker.Checked )
            {
                DateTime dt = dateTimePicker.Value;
                stopDateMaskedEditTextBox.Text = String.Format( "{0:D2}{1:D2}{2:D4}", dt.Month, dt.Day, dt.Year );
            }
            else
            {
                stopDateMaskedEditTextBox.Text = String.Empty;
            }
        }

        private void stopDateMaskedEditTextBox_Enter(object sender, EventArgs e)
        {
            //SetDateWithNormalBgColor();
        }

        private void dateTimePicker_CloseUp(object sender, EventArgs e)
        {
            //VerifyStopDate();
            CheckForDataEntered();
        }
        private void addNewButton_Click(object sender, EventArgs e)
        {
            Announcement anAnnouncement = new Announcement();
            this.Model = anAnnouncement;
            this.AnnouncementService.ResetControls();
        }
        private void clearAllButton_Click(object sender, EventArgs e)
        {
            this.ResetControls();
            this.ClearModel();
        }


        private void okButton_Click(object sender, EventArgs e)
        {
            if( this.Model == null )
            {
                Announcement anAnnouncement = new Announcement();
                this.Model = anAnnouncement;
            }
            this.UpdateModel();
            Announcement announcementToSave = this.Model;
            this.AnnouncementService.Save( announcementToSave );
            this.ClearModel();
            this.ResetControls();
        }

        private void announcementTextBox_TextChanged(object sender, EventArgs e)
        {
            CheckForDataEntered();
        }

        private void groupsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckForDataEntered();
        }
        private void facilitiesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckForDataEntered();
        }

		private void stopDateMaskedEditTextBox_Validating(object sender, CancelEventArgs e)
		{
			if( dateTimePicker.Focused )
			{
				return;
			}
			VerifyStopDate();
			CheckForDataEntered();		
		}
        #endregion

        #region Methods
        public override void UpdateView()
        {
            if( this.Model != null )
            {
                this.authorNameLabel.Text = this.Model.Author;
                this.stopDateMaskedEditTextBox.Text = this.Model.StopDate.ToString( "MM/dd/yyyy" );
                this.postDateValueLabel.Text = this.Model.SaveDate.ToString( "MM/dd/yyyy" );
                this.announcementTextBox.Text = this.Model.Description;
                int index;
                this.facilitiesListBox.ClearSelected();
                this.groupsListBox.ClearSelected();
                if( this.Model.Roles != null )
                {
                    foreach( Role role in this.Model.Roles )
                    {
                        index = this.groupsListBox.FindStringExact( role.Name );
                        if( index > -1 )
                        {
                            this.groupsListBox.SetSelected( index, true );
                        }
                    }
                }
                if( this.Model.Facilities != null )
                {
                    foreach( Facility facility in this.Model.Facilities )
                    {
                        index = this.facilitiesListBox.FindStringExact( facility.Code + " " + facility.Description );
                        if( index > -1 )
                        {
                            this.facilitiesListBox.SetSelected( index, true );
                        }
                    }
                }
            }

        }

        public override void UpdateModel()
        {
            Facility facility = null;
            ICollection selectedRoles = null;
            this.Model.Author = this.authorNameLabel.Text;
            this.Model.Description = this.announcementTextBox.Text;
            this.Model.StopDate = DateTime.Parse( this.stopDateMaskedEditTextBox.Text );
            this.Model.Roles.Clear();
            if( groupsAllCheckBox.Checked )
            {
                selectedRoles = ( (Hashtable)this.Roles ).Values;
            }
            else
            {
                selectedRoles = this.groupsListBox.SelectedItems;
            }
            foreach ( Role role in selectedRoles ) 
            {
                this.Model.Roles.Add( role );
            }
            this.Model.Facilities.Clear();
            if( facilitiesAllCheckBox.Checked )
            {
                foreach ( Facility aFacility in FacilitiesToManage ) 
                {
                    this.Model.Facilities.Add( aFacility );
                }
            }
            else
            {
                Array facilities = Array.CreateInstance( typeof(Facility) , FacilitiesToManage.Count );
                this.FacilitiesToManage.CopyTo( facilities, 0 );
                foreach( object listItem in facilitiesListBox.SelectedItems )
                {
                    facility = ( Facility )facilities.GetValue( 
                        facilitiesListBox.Items.IndexOf( listItem.ToString() ) );
                    this.Model.Facilities.Add( facility );  
                }
            }
        }


        public void ResetControls()
        {
            this.groupsListBox.Items.Clear();
            this.facilitiesListBox.Items.Clear();
            if( this.Roles != null )
            {
                foreach( Role role in ( (Hashtable)this.Roles ).Values )
                {
                    groupsListBox.Items.Add( role );
                }
            }
            if( this.FacilitiesToManage != null )
            {
                foreach( Facility facility in this.FacilitiesToManage )
                {
                    facilitiesListBox.Items.Add( facility.Code + " " + facility.Description );
                }
            }

            this.authorNameLabel.Text = this.AnnouncementService.GetCurrentUserName();
            DateTime todaysDate = this.AnnouncementService.GetCurrentFacilityTime();
            DateTime futureDateTime = todaysDate.AddDays( NUMBER_OF_DAYS_IN_FUTURE );
            this.stopDateMaskedEditTextBox.Text = futureDateTime.ToString( "MM/dd/yyyy" );
            this.postDateValueLabel.Text = todaysDate.ToString( "MM/dd/yyyy" );
            this.okButton.Enabled = false;
            this.groupsAllCheckBox.Checked = false;
            this.groupsListBox.ClearSelected();
            this.facilitiesAllCheckBox.Checked = false;
            this.facilitiesListBox.ClearSelected();
            this.announcementTextBox.Text = String.Empty;
        }

        private void ClearModel()
        {
            this.Model = null;
        }

        #endregion

        #region Properties
        
        public ICollection FacilitiesToManage
        {
            private get
            {
                return i_FacilitiesToManage;
            }
            set
            {
                i_FacilitiesToManage = value;
            }
        }


        public ICollection Roles
        {
            private get
            {
                return i_Roles;
            }
            set
            {
                i_Roles = value;
            }
        }


        public AnnouncementService AnnouncementService
        {
            private get
            {
                return i_AnnouncementService;
            }
            set
            {
                i_AnnouncementService = value;
            }
        }


        public new Announcement Model
        {
            private get
            {
                return base.Model as Announcement;
            }
            set
            {
                base.Model = value;
            }
        }
        #endregion

        #region Private Methods
        private void SetDateWithErrBgColor()
        {
            UIColors.SetErrorBgColor( this.stopDateMaskedEditTextBox );
            Refresh();
        }

        private void SetDateWithNormalBgColor()
        {
            UIColors.SetNormalBgColor( stopDateMaskedEditTextBox );
            Refresh();
        }

        private void VerifyStopDate()
        {
			if( stopDateMaskedEditTextBox.Text.Length != FULL_DATE_FIELD )
			{
				SetDateWithErrBgColor();
				MessageBox.Show( UIErrorMessages.DATE_INVALID_ERRMSG, "Error",
					MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
					MessageBoxDefaultButton.Button1 );
				stopDateMaskedEditTextBox.Focus();
				return;
			}
			else
			{
				SetDateWithNormalBgColor();
			}
			try
            {   
                //check for valid date.
                DateTime stopDate = DateTime.Parse( stopDateMaskedEditTextBox.Text );

                if( DateValidator.IsValidDate( stopDate ) == false )
                {
                    SetDateWithErrBgColor();
                    MessageBox.Show( UIErrorMessages.DATE_NOT_EXIST_ERRMSG, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1 );
					stopDateMaskedEditTextBox.Focus();
					//SetDateWithNormalBgColor();
                }
                else
                {
                    //check for old date.
                    if( DateTime.Compare( stopDate.Date, DateTime.Now.Date ) < 0 )
                    {
                        SetDateWithErrBgColor();
                        MessageBox.Show( UIErrorMessages.ANNOUNCEMENT_STOP_DATE_EARLIER, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                            MessageBoxDefaultButton.Button1 );
						stopDateMaskedEditTextBox.Focus();
						//SetDateWithNormalBgColor();
                    }
                    else
                    {
                        SetDateWithNormalBgColor();
                    }
                }
            }
            catch
            {   // DateTime ctor throws ArgumentOutOfRange exception when there's
                // an invalid year, month, or day.  Simply set field to error color.
                SetDateWithErrBgColor();
				
                stopDateMaskedEditTextBox.Focus();
                throw;
            }

        }

        private void CheckForDataEntered()
        {
            this.okButton.Enabled = false;
            if( ( ( groupsAllCheckBox.Checked ) || ( groupsListBox.SelectedItems.Count > 0 ) )
                && ( ( facilitiesAllCheckBox.Checked ) || ( facilitiesListBox.SelectedItems.Count > 0 ) )
                && ( announcementTextBox.TextLength > 0 )
                && ( stopDateMaskedEditTextBox.UnMaskedText.Length > 0 && stopDateMaskedEditTextBox.BackColor !=  UIColors.TextFieldBackgroundError )
                )
            {
                this.okButton.Enabled = true;
                if( this.Parent != null )
                {
                    base.AcceptButton = this.okButton;
                }
            }
        }
        #endregion

        #region Private Properties
        #endregion

        #region Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.addEditLabel = new System.Windows.Forms.Label();
			this.addNewButton = new LoggingButton();
			this.authorLabel = new System.Windows.Forms.Label();
			this.authorNameLabel = new System.Windows.Forms.Label();
			this.stopDateLabel = new System.Windows.Forms.Label();
			this.dateTimePicker = new System.Windows.Forms.DateTimePicker();
			this.stopDateMaskedEditTextBox = new Extensions.UI.Winforms.MaskedEditTextBox();
			this.announcementLabel = new System.Windows.Forms.Label();
			this.announcementTextBox = new System.Windows.Forms.TextBox();
			this.postDateLabel = new System.Windows.Forms.Label();
			this.postDateValueLabel = new System.Windows.Forms.Label();
			this.ViewingGroupsGroupBox = new System.Windows.Forms.GroupBox();
			this.groupsListBox = new System.Windows.Forms.ListBox();
			this.groupsAllCheckBox = new System.Windows.Forms.CheckBox();
			this.viewingFacilitiesGroupBox = new System.Windows.Forms.GroupBox();
			this.facilitiesListBox = new System.Windows.Forms.ListBox();
			this.facilitiesAllCheckBox = new System.Windows.Forms.CheckBox();
			this.okButton = new LoggingButton();
            this.clearAllButton = new LoggingButton();
			this.ViewingGroupsGroupBox.SuspendLayout();
			this.viewingFacilitiesGroupBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// addEditLabel
			// 
			this.addEditLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.addEditLabel.Location = new System.Drawing.Point(22, 8);
			this.addEditLabel.Name = "addEditLabel";
			this.addEditLabel.Size = new System.Drawing.Size(160, 23);
			this.addEditLabel.TabIndex = 24;
			this.addEditLabel.Text = "Add/Edit Announcement";
			// 
			// addNewButton
			// 
			this.addNewButton.BackColor = System.Drawing.SystemColors.Control;
			this.addNewButton.Location = new System.Drawing.Point(222, 8);
			this.addNewButton.Name = "addNewButton";
			this.addNewButton.Size = new System.Drawing.Size(136, 23);
			this.addNewButton.TabIndex = 0;
			this.addNewButton.Text = "Add &New Announcement";
			this.addNewButton.Click += new System.EventHandler(this.addNewButton_Click);
			// 
			// authorLabel
			// 
			this.authorLabel.Location = new System.Drawing.Point(22, 44);
			this.authorLabel.Name = "authorLabel";
			this.authorLabel.Size = new System.Drawing.Size(42, 20);
			this.authorLabel.TabIndex = 22;
			this.authorLabel.Text = "Author:";
			this.authorLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// authorNameLabel
			// 
			this.authorNameLabel.Location = new System.Drawing.Point(64, 45);
			this.authorNameLabel.Name = "authorNameLabel";
			this.authorNameLabel.Size = new System.Drawing.Size(232, 20);
			this.authorNameLabel.TabIndex = 21;
			// 
			// stopDateLabel
			// 
			this.stopDateLabel.Location = new System.Drawing.Point(326, 45);
			this.stopDateLabel.Name = "stopDateLabel";
			this.stopDateLabel.Size = new System.Drawing.Size(64, 20);
			this.stopDateLabel.TabIndex = 20;
			this.stopDateLabel.Text = "Stop date:";
			this.stopDateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// dateTimePicker
			// 
			this.dateTimePicker.CalendarTitleForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.dateTimePicker.Checked = false;
			this.dateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
			this.dateTimePicker.Location = new System.Drawing.Point(454, 43);
			this.dateTimePicker.MinDate = new System.DateTime(1800, 1, 1, 0, 0, 0, 0);
			this.dateTimePicker.Name = "dateTimePicker";
			this.dateTimePicker.Size = new System.Drawing.Size(21, 20);
			this.dateTimePicker.TabIndex = 12;
			this.dateTimePicker.TabStop = false;
			this.dateTimePicker.CloseUp += new System.EventHandler(this.dateTimePicker_CloseUp);
			this.dateTimePicker.ValueChanged += new System.EventHandler(this.dateTimePicker_ValueChanged);
			// 
			// stopDateMaskedEditTextBox
			// 
			this.stopDateMaskedEditTextBox.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
			this.stopDateMaskedEditTextBox.KeyPressExpression = HelperClasses.DateValidator.DATEKeyPressExpression;
			this.stopDateMaskedEditTextBox.Location = new System.Drawing.Point(390, 43);
			this.stopDateMaskedEditTextBox.Mask = "  /  /";
			this.stopDateMaskedEditTextBox.MaxLength = 10;
			this.stopDateMaskedEditTextBox.Name = "stopDateMaskedEditTextBox";
			this.stopDateMaskedEditTextBox.Size = new System.Drawing.Size(64, 20);
			this.stopDateMaskedEditTextBox.TabIndex = 1;
			this.stopDateMaskedEditTextBox.ValidationExpression = HelperClasses.DateValidator.DATEValidationExpression;
			this.stopDateMaskedEditTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.stopDateMaskedEditTextBox_Validating);
			this.stopDateMaskedEditTextBox.Enter += new System.EventHandler(this.stopDateMaskedEditTextBox_Enter);
			// 
			// announcementLabel
			// 
			this.announcementLabel.Location = new System.Drawing.Point(22, 72);
			this.announcementLabel.Name = "announcementLabel";
			this.announcementLabel.Size = new System.Drawing.Size(100, 20);
			this.announcementLabel.TabIndex = 9;
			this.announcementLabel.Text = "Announcement:";
			// 
			// announcementTextBox
			// 
			this.announcementTextBox.Location = new System.Drawing.Point(14, 96);
			this.announcementTextBox.MaxLength = 3000;
			this.announcementTextBox.Multiline = true;
			this.announcementTextBox.Name = "announcementTextBox";
			this.announcementTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.announcementTextBox.Size = new System.Drawing.Size(464, 152);
			this.announcementTextBox.TabIndex = 2;
			this.announcementTextBox.Text = "";
			this.announcementTextBox.TextChanged += new System.EventHandler(this.announcementTextBox_TextChanged);
			// 
			// postDateLabel
			// 
			this.postDateLabel.Location = new System.Drawing.Point(14, 264);
			this.postDateLabel.Name = "postDateLabel";
			this.postDateLabel.Size = new System.Drawing.Size(56, 20);
			this.postDateLabel.TabIndex = 3;
			this.postDateLabel.Text = "Post date:";
			// 
			// postDateValueLabel
			// 
			this.postDateValueLabel.Location = new System.Drawing.Point(71, 264);
			this.postDateValueLabel.Name = "postDateValueLabel";
			this.postDateValueLabel.Size = new System.Drawing.Size(98, 20);
			this.postDateValueLabel.TabIndex = 25;
			// 
			// ViewingGroupsGroupBox
			// 
			this.ViewingGroupsGroupBox.Controls.Add(this.groupsListBox);
			this.ViewingGroupsGroupBox.Controls.Add(this.groupsAllCheckBox);
			this.ViewingGroupsGroupBox.Location = new System.Drawing.Point(510, 40);
			this.ViewingGroupsGroupBox.Name = "ViewingGroupsGroupBox";
			this.ViewingGroupsGroupBox.Size = new System.Drawing.Size(200, 208);
			this.ViewingGroupsGroupBox.TabIndex = 4;
			this.ViewingGroupsGroupBox.TabStop = false;
			this.ViewingGroupsGroupBox.Text = "Viewing groups";
			// 
			// groupsListBox
			// 
			this.groupsListBox.DisplayMember = "Name";
			this.groupsListBox.Location = new System.Drawing.Point(8, 48);
			this.groupsListBox.Name = "groupsListBox";
			this.groupsListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
			this.groupsListBox.Size = new System.Drawing.Size(184, 147);
			this.groupsListBox.TabIndex = 1;
			this.groupsListBox.ValueMember = "Oid";
			this.groupsListBox.SelectedIndexChanged += new System.EventHandler(this.groupsListBox_SelectedIndexChanged);
			// 
			// groupsAllCheckBox
			// 
			this.groupsAllCheckBox.Location = new System.Drawing.Point(16, 24);
			this.groupsAllCheckBox.Name = "groupsAllCheckBox";
			this.groupsAllCheckBox.Size = new System.Drawing.Size(104, 20);
			this.groupsAllCheckBox.TabIndex = 0;
			this.groupsAllCheckBox.Text = "All";
			this.groupsAllCheckBox.CheckedChanged += new System.EventHandler(this.groupsAllCheckBox_CheckedChanged);
			// 
			// viewingFacilitiesGroupBox
			// 
			this.viewingFacilitiesGroupBox.Controls.Add(this.facilitiesListBox);
			this.viewingFacilitiesGroupBox.Controls.Add(this.facilitiesAllCheckBox);
			this.viewingFacilitiesGroupBox.Location = new System.Drawing.Point(744, 40);
			this.viewingFacilitiesGroupBox.Name = "viewingFacilitiesGroupBox";
			this.viewingFacilitiesGroupBox.Size = new System.Drawing.Size(200, 208);
			this.viewingFacilitiesGroupBox.TabIndex = 5;
			this.viewingFacilitiesGroupBox.TabStop = false;
			this.viewingFacilitiesGroupBox.Text = "Viewing facilities";
			// 
			// facilitiesListBox
			// 
			this.facilitiesListBox.Location = new System.Drawing.Point(8, 48);
			this.facilitiesListBox.Name = "facilitiesListBox";
			this.facilitiesListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
			this.facilitiesListBox.Size = new System.Drawing.Size(184, 147);
			this.facilitiesListBox.TabIndex = 1;
			this.facilitiesListBox.SelectedIndexChanged += new System.EventHandler(this.facilitiesListBox_SelectedIndexChanged);
			// 
			// facilitiesAllCheckBox
			// 
			this.facilitiesAllCheckBox.Location = new System.Drawing.Point(16, 24);
			this.facilitiesAllCheckBox.Name = "facilitiesAllCheckBox";
			this.facilitiesAllCheckBox.Size = new System.Drawing.Size(104, 20);
			this.facilitiesAllCheckBox.TabIndex = 0;
			this.facilitiesAllCheckBox.Text = "All";
			this.facilitiesAllCheckBox.CheckedChanged += new System.EventHandler(this.facilitiesAllCheckBox_CheckedChanged);
			// 
			// okButton
			// 
			this.okButton.Enabled = false;
			this.okButton.Location = new System.Drawing.Point(774, 280);
			this.okButton.Name = "okButton";
			this.okButton.TabIndex = 6;
			this.okButton.Text = "OK";
			this.okButton.Click += new System.EventHandler(this.okButton_Click);
			// 
			// clearAllButton
			// 
			this.clearAllButton.Location = new System.Drawing.Point(870, 280);
			this.clearAllButton.Name = "clearAllButton";
			this.clearAllButton.TabIndex = 7;
			this.clearAllButton.Text = "&Clear All";
			this.clearAllButton.Click += new System.EventHandler(this.clearAllButton_Click);
			// 
			// AddEditAnnouncementView
			// 
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.clearAllButton);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.viewingFacilitiesGroupBox);
			this.Controls.Add(this.ViewingGroupsGroupBox);
			this.Controls.Add(this.postDateValueLabel);
			this.Controls.Add(this.postDateLabel);
			this.Controls.Add(this.announcementTextBox);
			this.Controls.Add(this.announcementLabel);
			this.Controls.Add(this.dateTimePicker);
			this.Controls.Add(this.stopDateMaskedEditTextBox);
			this.Controls.Add(this.stopDateLabel);
			this.Controls.Add(this.authorNameLabel);
			this.Controls.Add(this.authorLabel);
			this.Controls.Add(this.addNewButton);
			this.Controls.Add(this.addEditLabel);
			this.Name = "AddEditAnnouncementView";
			this.Size = new System.Drawing.Size(955, 312);
			this.ViewingGroupsGroupBox.ResumeLayout(false);
			this.viewingFacilitiesGroupBox.ResumeLayout(false);
			this.ResumeLayout(false);

		}
        #endregion

        #region Construction and Finalization
        public AddEditAnnouncementView()
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();
            base.EnableThemesOn( this );

        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if (components != null) 
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }
        #endregion

        #region Data Elements
        private IContainer components = null;
        private Label addEditLabel;
        private LoggingButton addNewButton;
        private Label authorLabel;
        private Label authorNameLabel;
        private Label stopDateLabel;
        private DateTimePicker dateTimePicker;
        private MaskedEditTextBox stopDateMaskedEditTextBox;
        private Label announcementLabel;
        private TextBox announcementTextBox;
        private GroupBox ViewingGroupsGroupBox;
        private GroupBox viewingFacilitiesGroupBox;
        private CheckBox groupsAllCheckBox;
        private ListBox groupsListBox;
        private ListBox facilitiesListBox;
        private CheckBox facilitiesAllCheckBox;
        private LoggingButton okButton;
        private LoggingButton clearAllButton;
        private Label postDateValueLabel;
        private Label postDateLabel;
        private ICollection i_FacilitiesToManage;
        private ICollection i_Roles;        
        private AnnouncementService i_AnnouncementService;
        #endregion

        #region Constants
        private const int    FULL_DATE_FIELD  = 10;
        private const double NUMBER_OF_DAYS_IN_FUTURE = 30.0;

        #endregion



    }
}
