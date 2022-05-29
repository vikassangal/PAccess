using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.Remoting;
using System.Windows.Forms;
using Extensions.UI.Winforms;  
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.HelperClasses;
using PatientAccess.Utilities;

namespace PatientAccess.UI.Reports
{
    [Serializable]
    public class PhysicianSearchBySpecialtyView : ControlView
    {
        #region Events

        public event EventHandler PhysiciansFound;
        public event EventHandler ResetView;
        public event EventHandler NoPhysiciansFound;
        public event EventHandler DisableDetailsButton;

        #endregion

        #region Events Handler

        private void showButton_Click(object sender, EventArgs e)
        {
            this.ResetView(this, null);
            ViewPhysicianInquiry();
        }

        private void specialityComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            EnableorDisableShowButton();  
        }

        private void EnableorDisableShowButton()
        {

            try
            {
                if (specialityComboBox.SelectedItem != null &&
                    specialityComboBox.SelectedItem.ToString().Trim() == "")
                {
                    showButton.Enabled = false;
                }
                else if (String.IsNullOrEmpty(SpecialityText.Trim()))
                {
                    showButton.Enabled = false;
                }
                else if (SpecialityFoundInTheList)
                {
                    showButton.Enabled = true;
                }
                else
                {
                    showButton.Enabled = false;
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                showButton.Enabled = false;
            }
        }

        private bool SpecialityFoundInTheList
        {
            get
            {
                var specialityText = SpecialityText;
                return specialityComboBox.Items.Cast<string>().Any(speciality => speciality.Trim() == specialityText.Trim());
            }
        }

        private void SpecialityButton_Click(object sender, EventArgs e)
        {
            if (this.ResetView != null)
            {
                this.ResetView(this, null);
            }
            if (this.DisableDetailsButton != null)
            {
                DisableDetailsButton(this, null);
            }
            FillComboBox();
            this.specialityComboBox.SelectedIndex = -1;
            SpecialityText = String.Empty;
            this.printButton.Enabled = false;
            this.showButton.Enabled = false;
            this.Invalidate();
            this.Update(); 
            
        }

        private void printButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Functionality is not supported.",
                "Physician Speciality Search",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        #endregion

        #region Construction And Finalization

        /// <summary>
        /// Constructor
        /// </summary>
        public PhysicianSearchBySpecialtyView()
        {
            InitializeComponent();
            base.EnableThemesOn(this);
            CreateSearchSequence();
        }

        private void CreateSearchSequence()
        {
            SearchList();

            Observable.FromEventPattern(h => specialityComboBox.TextChanged += h,
                h => specialityComboBox.TextChanged -= h)
                .Do(a => SearchList())
                .Subscribe();
        }

        private void SearchList()
        {
            showButton.Enabled = false;
            ConfigureSpecialitySearchText();
            SpecialityText = SpecialityText.ToUpper();
            if (SpecialityText.Length > 1 && specialityComboBox.SelectedIndex < 0 )
            {
                SpecialityText = SpecialitySearchText;
            }

            SpecialitySearchText = SpecialityText;
            string query = SpecialityText;
          
            if (specialityComboBox.SelectedIndex < 0)
            {
                if (query.Length > 0)
                {
                    specialityComboBox.Items.Clear(); 

                    LoadAllSpecialities();

                    if (AllPhysicianSpecialities != null)
                    {
                        foreach (
                           Speciality item in
                                AllPhysicianSpecialities.Values.Where(
                                    s => s.Description.StartsWith(query) || s.Code.StartsWith(query)))
                        {
                            AddPhysicianSpeciality(item);
                        }
                    }


                    if (specialityComboBox.Items.Count > 0)
                    {
                        specialityComboBox.DroppedDown = true;
                        Cursor.Current = Cursors.Default;
                        SpecialityText = query;
                    }
                    else
                    {
                        specialityComboBox.DroppedDown = false;
                    }
                    
                }
                else
                {
                    FillComboBox();
                    showButton.Enabled = false;
                }
            }
        }

        /// <summary>
        /// Clean up any resources being used
        /// </summary>
        /// <param name="disposing"></param>
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
            this.specialityComboBox = new System.Windows.Forms.ComboBox();
            this.specialtyLabel = new System.Windows.Forms.Label();
            this.showButton = new LoggingButton();
            this.specialityResetButton = new LoggingButton();
            this.printButton = new LoggingButton();
            this.SuspendLayout();
            // 
            // specialityComboBox
            // 
            this.specialityComboBox.Size = new System.Drawing.Size(360, 21);
            this.specialityComboBox.Location = new System.Drawing.Point(71, 17);
            this.specialityComboBox.Name = "specialityComboBox";
            this.specialityComboBox.TabIndex = 0;
            this.specialityComboBox.MaxLength = 55;
            this.specialityComboBox.SelectedIndexChanged +=
                new System.EventHandler(this.specialityComboBox_SelectedIndexChanged);
            this.specialityComboBox.KeyPress +=
                new System.Windows.Forms.KeyPressEventHandler(this.specialityComboBox_KeyPress);

            this.specialityComboBox.Validating +=
                new System.ComponentModel.CancelEventHandler(this.specialityComboBox_Validating);
            this.specialityComboBox.DropDownClosed += new System.EventHandler(this.specialityComboBox_DropdownClosed);
            // 
            // specialtyLabel
            // 
            this.specialtyLabel.AutoSize = true;
            this.specialtyLabel.Location = new System.Drawing.Point(10, 21);
            this.specialtyLabel.Name = "specialtyLabel";
            this.specialtyLabel.Size = new System.Drawing.Size(53, 16);
            this.specialtyLabel.TabIndex = 4;
            this.specialtyLabel.Text = "Specialty:";
            // 
            // showButton
            // 
            this.showButton.Enabled = false;
            this.showButton.Location = new System.Drawing.Point(610, 16);
            this.showButton.Name = "showButton";
            this.showButton.TabIndex = 1;
            this.showButton.Text = "Sh&ow";
            this.showButton.Click += new System.EventHandler(this.showButton_Click);
            // 
            // specialityResetButton
            // 
            this.specialityResetButton.Location = new System.Drawing.Point(698, 16);
            this.specialityResetButton.Name = "specialityResetButton";
            this.specialityResetButton.TabIndex = 2;
            this.specialityResetButton.Text = "Rese&t";
            this.specialityResetButton.Click += new System.EventHandler(this.SpecialityButton_Click);
            // 
            // printButton
            // 
            this.printButton.Enabled = false;
            this.printButton.Location = new System.Drawing.Point(824, 17);
            this.printButton.Name = "printButton";
            this.printButton.TabIndex = 3;
            this.printButton.Text = "Pri&nt Report";
            this.printButton.Click += new System.EventHandler(this.printButton_Click);
            // 
            // PhysicianSearchBySpecialtyView
            // 
            this.AcceptButton = this.showButton;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.printButton);
            this.Controls.Add(this.specialityResetButton);
            this.Controls.Add(this.showButton);
            this.Controls.Add(this.specialtyLabel);
            this.Controls.Add(this.specialityComboBox);
            this.Name = "PhysicianSearchBySpecialtyView";
            this.Size = new System.Drawing.Size(911, 56);
            this.ResumeLayout(false);
        }

        private void specialityComboBox_Dropdown(object sender, EventArgs e)
        {
            if (specialityComboBox.Items.Count < 1)
            {
                specialityComboBox.DroppedDown = false;
            }

        }

        private void specialityComboBox_DropdownClosed(object sender, EventArgs e)
        {
            EnableorDisableShowButton(); 
        }

        private void specialityComboBox_Validating(object sender, CancelEventArgs e)
        {

            if (specialityComboBox.SelectedItem != null &&
                specialityComboBox.SelectedItem.ToString().Trim() != "")
            {
                SpecialityText = specialityComboBox.SelectedItem.ToString().Trim();
            }
            EnableorDisableShowButton();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Fills the <see cref="ComboBox"/> with list of specialties from database
        /// </summary>
        public void FillComboBox()
        {

            specialityComboBox.Items.Clear();

            LoadAllSpecialities();
           
            foreach (Speciality speciality in AllPhysicianSpecialities.Values)
            {
                AddPhysicianSpeciality(speciality); 
            }
        }

        #endregion

        public void LoadAllSpecialities()
        {
            if (AllSpecialities.Count == 0)
            {
                IPhysicianBroker physicianBroker = BrokerFactory.BrokerOfType<IPhysicianBroker>();

                physiciansSpeciality = (ArrayList) physicianBroker.SpecialtiesFor(User.GetCurrent().Facility.Oid);

                foreach (Speciality speciality in physiciansSpeciality)
                {
                    AllSpecialities.Add(speciality.Code, speciality);
                }
            }
        }

        #region public Properties

        private Dictionary<string, Speciality> AllPhysicianSpecialities
        {
            get { return AllSpecialities; }
        }

        private string SpecialityText
        {
            get
            {
                try
                {
                    return specialityComboBox.Text;
                }
                catch (ArgumentOutOfRangeException)
                {
                    return String.Empty;
                }
            }

            set
            {
                try
                {
                    specialityComboBox.Text = value;
                }
                catch (ArgumentOutOfRangeException)
                {
                }
            }
        }

        #endregion

        #region Private Methods

        private void ConfigureSpecialitySearchText()
        {

            SpecialityText =
                StringFilter.RemoveFirstCharNonLetterAndRestNonLetterAmpersandHyphenSlashBlankUnderscoreComma(
                    SpecialityText);
        }

        private void AddPhysicianSpeciality(Speciality speciality)
        {
            specialityComboBox.Items.Add(speciality.AsFormattedSpeciality);
        }

        /// <summary>
        /// Validates wheather the selected speciality is valid
        /// </summary>
        /// <returns></returns>
        private new ValidationResult Validate()
        {
            if (SpecialityText != String.Empty)
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
            ICollection specialityList = null;
            try
            {
                this.Cursor = Cursors.WaitCursor;
                //Check to see if search data entered is valid.
                ValidationResult result = Validate();
                if (result.IsValid)
                {
                    try
                    {
                        //Create PatientSearchCriteria and pass in all args.

                        IPhysicianBroker physicianBroker = BrokerFactory.BrokerOfType<IPhysicianBroker>();
                        Speciality speciality = new Speciality();
                        var selectedCode = SelectedCode;
                        if (AllSpecialities.ContainsKey(selectedCode))
                        {
                            speciality = AllSpecialities[selectedCode];
                        }
                        specialityList = physicianBroker.PhysiciansSpecialtyMatching(User.GetCurrent().Facility.Oid, speciality);

                        if (specialityList != null)
                        {
                            if (specialityList.Count != 0 && PhysiciansFound != null)
                            {
                                PhysiciansFound(this, new LooseArgs(specialityList));
                                this.printButton.Enabled = true;
                            }
                            else
                            {
                                if (NoPhysiciansFound != null)
                                {
                                    NoPhysiciansFound(this, new LooseArgs(this));
                                    this.printButton.Enabled = false;
                                }
                            }
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
                else
                {
                    if (this.ResetView != null)
                    {
                        this.ResetView(this, null);
                    }
                }
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private string SelectedCode
        {
            get
            {
                var text = SpecialityText;
                var indexOfSpecialityCode = text.IndexOf(BLANK);
                var selectedCode = String.Empty;
                
                if (indexOfSpecialityCode > 0)
                {
                    selectedCode = text.Substring(0, indexOfSpecialityCode);
                }
                return selectedCode;
            }
        }

        #endregion

        #region Data Elements

        private ComboBox specialityComboBox;
        private LoggingButton showButton;
        private LoggingButton printButton;
        private LoggingButton specialityResetButton;
        private Container components = null;
        private ArrayList physiciansSpeciality = new ArrayList();
        private Label specialtyLabel;
        private readonly Dictionary<string, Speciality> AllSpecialities = new Dictionary<string, Speciality>();
        private String SpecialitySearchText = String.Empty;
         
        #endregion

        private void specialityComboBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar >= 'a' && e.KeyChar <= 'z')
            {
                e.KeyChar = Convert.ToChar(e.KeyChar.ToString().ToUpper());
                SpecialitySearchText = e.KeyChar.ToString();
            }
            
            if (e.KeyChar == '\b')
            {
                SpecialityText = String.Empty; 
                specialityComboBox.SelectedIndex = -1;
            }
        }

        #region Constants

        public static readonly string BLANK = " ";

        #endregion

    }
}
