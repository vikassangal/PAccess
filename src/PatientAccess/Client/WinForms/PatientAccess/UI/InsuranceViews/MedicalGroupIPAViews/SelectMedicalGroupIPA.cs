using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI.InsuranceViews.MedicalGroupIPAViews
{
    /// <summary>
    /// Summary description for SelectMedicalGroupIPA
    /// </summary>
    [Serializable]
    public class SelectMedicalGroupIPA : TimeOutFormView
    {
        #region Event Handlers
        private void lvGroups_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.btnOK.Enabled = false;
            this.lvClinics.Items.Clear();

            this.GetSelectedMedicalGroupIPA();
        }

        private void lvClinics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if( this.lvClinics.SelectedItems.Count > 0 )
            {
                Clinic clinic = (Clinic)this.lvClinics.SelectedItems[0].Tag;
                if( clinic != null )
                {
                    i_Clinic = clinic;
                    this.btnOK.Enabled = true;
                }
                else
                {
                    this.btnOK.Enabled = false;
                }
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            FindMatchingMedicalGroups( this.mtbSearchCriteria.UnMaskedText );
        }

        private void mtbSearchCriteria_GotFocus(object sender, EventArgs e)
        {
            this.AcceptButton = this.btnSearch;
        }

        private void mtbSearchCriteria_LostFocus(object sender, EventArgs e)
        {
            this.AcceptButton = this.btnOK;
        }

        private void SelectMedicalGroupIPA_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar.Equals((char)27))
            {
                this.Close();
            }
        }

        #endregion

        #region Methods
        public override void UpdateView()
        {
        }

        public override void UpdateModel()
        {
        }
        #endregion

        #region Properties
        public MedicalGroupIPA Model_MedicalGroupIPA
        {
            set
            {
                this.Model = value;
            }
            get
            {
                return (MedicalGroupIPA)this.Model;
            }
        }
        #endregion

        #region Private Methods
        private void GetSelectedMedicalGroupIPA()
        {
            if( this.lvGroups.SelectedItems.Count > 0 )
            {
                MedicalGroupIPA medicalGroupIPA = (MedicalGroupIPA)this.lvGroups.SelectedItems[0].Tag;
                if( medicalGroupIPA != null )
                {
                    i_MedicalGroupIPA = medicalGroupIPA;
                    this.PopulateClinicsForSelectedIPA( medicalGroupIPA );
                }
            }
        }

        private void FindMatchingMedicalGroups( string medicalGroupName)
        {
            this.lvGroups.Items.Clear();
            this.lvClinics.Items.Clear();

            IInsuranceBroker broker = BrokerFactory.BrokerOfType<IInsuranceBroker>();
            ICollection ipas = (ICollection)broker.IPAsFor( User.GetCurrent().Facility.Oid, medicalGroupName );

            if( ipas.Count > 0 )
            {
                foreach( MedicalGroupIPA ipa in ipas )
                {
                    ListViewItem item = new ListViewItem();

                    item.Tag = ipa;

                    item.Text = ipa.Name;
                    //item.SubItems.Add( ipa.Name.ToString() );
                    item.SubItems.Add( ipa.Code );
                    lvGroups.Items.Add( item );
                }

                this.labelGroup.Visible = false;
                this.lvGroups.Focus();
                this.lvGroups.Items[0].Selected = true;
            }
            else
            {
                this.labelGroup.Visible = true;
            }
        }

        private void PopulateClinicsForSelectedIPA( MedicalGroupIPA medicalGroupIPA )
        {
            ICollection clinics = medicalGroupIPA.Clinics;
            if( clinics.Count > 0 )
            {
                foreach( Clinic clinic in clinics )
                {
                    if( clinic.Name == "NO CLINIC" )
                    {
                        this.labelClinics.Visible = true;
                        return;
                    }
                    
                    ListViewItem item = new ListViewItem();

                    item.Tag = clinic;

                    item.Text = clinic.Name;
                    item.SubItems.Add( clinic.Code );
                    lvClinics.Items.Add( item );
                }

                this.labelClinics.Visible = false;
                this.lvClinics.Focus();
            }
            else
            {
                this.labelClinics.Visible = true;
            }
        }

        #region Construction and Finalization
        private void InitializeComponent()
        {
            this.btnCancel = new LoggingButton();
            this.btnOK = new LoggingButton();
            this.lblMedicalGroup = new Label();
            this.mtbSearchCriteria = new MaskedEditTextBox();
            this.btnSearch = new LoggingButton();
            this.label = new Label();
            this.lblClinics = new Label();
            this.labelGroup = new Label();
            this.labelClinics = new Label();
            this.lvGroups = new ListView();
            this.chName = new ColumnHeader();
            this.chCode = new ColumnHeader();
            this.lvClinics = new ListView();
            this.chClinicName = new ColumnHeader();
            this.chClinicCode = new ColumnHeader();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = DialogResult.Cancel;
            this.btnCancel.Location = new Point(353, 358);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new Size(75, 22);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = DialogResult.OK;
            this.btnOK.Enabled = false;
            this.btnOK.Location = new Point(273, 358);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new Size(75, 22);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "OK";
            // 
            // lblMedicalGroup
            // 
            this.lblMedicalGroup.Location = new Point(21, 19);
            this.lblMedicalGroup.Name = "lblMedicalGroup";
            this.lblMedicalGroup.Size = new Size(196, 13);
            this.lblMedicalGroup.TabIndex = 0;
            this.lblMedicalGroup.Text = "IPA/Primary Medical Group";
            // 
            // mtbSearchCriteria
            // 
            this.mtbSearchCriteria.CharacterCasing = CharacterCasing.Upper;
            this.mtbSearchCriteria.EntrySelectionStyle = EntrySelectionStyle.SelectionAtEnd;
            this.mtbSearchCriteria.KeyPressExpression = "^[a-zA-Z0-9].*";
            this.mtbSearchCriteria.Location = new Point(159, 43);
            this.mtbSearchCriteria.Mask = "";
            this.mtbSearchCriteria.MaxLength = 15;
            this.mtbSearchCriteria.Name = "mtbSearchCriteria";
            this.mtbSearchCriteria.Size = new Size(153, 20);
            this.mtbSearchCriteria.TabIndex = 1;
            this.mtbSearchCriteria.ValidationExpression = "^[a-zA-Z0-9].*";
            this.mtbSearchCriteria.LostFocus += new EventHandler(this.mtbSearchCriteria_LostFocus);
            this.mtbSearchCriteria.GotFocus += new EventHandler(this.mtbSearchCriteria_GotFocus);
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new Point(325, 41);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new Size(75, 21);
            this.btnSearch.TabIndex = 2;
            this.btnSearch.Text = "Search";
            this.btnSearch.Click += new EventHandler(this.btnSearch_Click);
            // 
            // label
            // 
            this.label.Location = new Point(21, 45);
            this.label.Name = "label";
            this.label.Size = new Size(144, 13);
            this.label.TabIndex = 0;
            this.label.Text = "Find groups beginning with:";
            // 
            // lblClinics
            // 
            this.lblClinics.Location = new Point(21, 214);
            this.lblClinics.Name = "lblClinics";
            this.lblClinics.Size = new Size(139, 14);
            this.lblClinics.TabIndex = 0;
            this.lblClinics.Text = "Clinics from selected group";
            // 
            // labelGroup
            // 
            this.labelGroup.BorderStyle = BorderStyle.Fixed3D;
            this.labelGroup.Location = new Point(62, 113);
            this.labelGroup.Name = "labelGroup";
            this.labelGroup.Size = new Size(323, 33);
            this.labelGroup.TabIndex = 0;
            this.labelGroup.Text = "No items found.";
            this.labelGroup.TextAlign = ContentAlignment.MiddleCenter;
            this.labelGroup.Visible = false;
            // 
            // labelClinics
            // 
            this.labelClinics.BorderStyle = BorderStyle.Fixed3D;
            this.labelClinics.Location = new Point(62, 282);
            this.labelClinics.Name = "labelClinics";
            this.labelClinics.Size = new Size(323, 33);
            this.labelClinics.TabIndex = 0;
            this.labelClinics.Text = "No items found.";
            this.labelClinics.TextAlign = ContentAlignment.MiddleCenter;
            this.labelClinics.Visible = false;
            // 
            // lvGroups
            // 
            this.lvGroups.Columns.AddRange(new ColumnHeader[] {
                                                                                       this.chName,
                                                                                       this.chCode});
            this.lvGroups.FullRowSelect = true;
            this.lvGroups.GridLines = true;
            this.lvGroups.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            this.lvGroups.Location = new Point(21, 76);
            this.lvGroups.MultiSelect = false;
            this.lvGroups.Name = "lvGroups";
            this.lvGroups.Size = new Size(407, 115);
            this.lvGroups.TabIndex = 3;
            this.lvGroups.View = View.Details;
            this.lvGroups.SelectedIndexChanged += new EventHandler(this.lvGroups_SelectedIndexChanged);
            // 
            // chName
            // 
            this.chName.Text = "Name";
            this.chName.Width = 300;
            // 
            // chCode
            // 
            this.chCode.Text = "Code";
            this.chCode.Width = 103;
            // 
            // lvClinics
            // 
            this.lvClinics.Columns.AddRange(new ColumnHeader[] {
                                                                                        this.chClinicName,
                                                                                        this.chClinicCode});
            this.lvClinics.FullRowSelect = true;
            this.lvClinics.GridLines = true;
            this.lvClinics.Location = new Point(21, 235);
            this.lvClinics.MultiSelect = false;
            this.lvClinics.Name = "lvClinics";
            this.lvClinics.Size = new Size(407, 115);
            this.lvClinics.TabIndex = 4;
            this.lvClinics.View = View.Details;
            this.lvClinics.SelectedIndexChanged += new EventHandler(this.lvClinics_SelectedIndexChanged);
            // 
            // chClinicName
            // 
            this.chClinicName.Text = "Name";
            this.chClinicName.Width = 300;
            // 
            // chClinicCode
            // 
            this.chClinicCode.Text = "Code";
            this.chClinicCode.Width = 103;
            // 
            // SelectMedicalGroupIPA
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleBaseSize = new Size(5, 13);
            this.BackColor = Color.FromArgb(((Byte)(209)), ((Byte)(228)), ((Byte)(243)));
            this.ClientSize = new Size(448, 387);
            this.Controls.Add(this.lblClinics);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.mtbSearchCriteria);
            this.Controls.Add(this.lblMedicalGroup);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.label);
            this.Controls.Add(this.labelGroup);
            this.Controls.Add(this.labelClinics);
            this.Controls.Add(this.lvClinics);
            this.Controls.Add(this.lvGroups);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectMedicalGroupIPA";
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Select IPA/Primary Medical Group";
            this.KeyPress += new KeyPressEventHandler(this.SelectMedicalGroupIPA_KeyPress);
            this.ResumeLayout(false);

        }
        #endregion
        #endregion

        #region Construction and Finalization
        public SelectMedicalGroupIPA()
        {
            // Required for Windows Form Designer support
            InitializeComponent();
            
            i_MedicalGroupIPA = new MedicalGroupIPA();
            i_Clinic = new Clinic();

            base.EnableThemesOn( this );
        }
        
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
        private MaskedEditTextBox       mtbSearchCriteria;

        private Container     components = null;
        private LoggingButton         btnCancel;
        private LoggingButton         btnOK;       
        private LoggingButton         btnSearch;

        private Label          label;
        private Label          lblClinics;
        private Label          labelGroup;
        private Label          labelClinics;
        private Label          lblMedicalGroup;
        private ListView       lvGroups;
        
        private ColumnHeader   chName;
        private ColumnHeader   chCode;
        private ColumnHeader   chClinicName;
        private ColumnHeader   chClinicCode;
        
        private ListView       lvClinics;
        
        public  Clinic                              i_Clinic;
        public  MedicalGroupIPA                     i_MedicalGroupIPA;
        #endregion

        #region Constants
        #endregion
    }
}
