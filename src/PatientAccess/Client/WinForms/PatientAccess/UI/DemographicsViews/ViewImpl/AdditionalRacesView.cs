using System;
using System.Collections;
using System.Windows.Forms;
using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI.DemographicsViews.Views;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.DemographicsViews.Presenters;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.DemographicsViews.ViewImpl
{
    public partial class AdditionalRacesView : TimeOutFormView, IAdditionalRacesView
    {
        #region Properties
        public IAdditionalRacesViewPresenter AdditionalRacesViewPresenter { get; set; }
        public Account ModelAccount { get; set; }
 
        #endregion

        #region Construction and Finalization

        public AdditionalRacesView()
        {
            InitializeComponent();
            EnableThemesOn(this);
            CenterToScreen();
        }

        #endregion
      
      
        #region Private Methods and Events

        private void cmbRace3_SelectedIndexChanged(object sender, System.EventArgs e)
        {

            if (AdditionalRacesViewPresenter.IsValidRace3)
            {
                cmbRace4.Enabled = true;
            }
            else
            {
                cmbRace4.Enabled = false;
                cmbRace4.SelectedIndex = -1;
                cmbRace5.Enabled = false;
                cmbRace5.SelectedIndex = -1;
            }
        }

        private void cmbRace4_SelectedIndexChanged(object sender, System.EventArgs e)
        {

            if (AdditionalRacesViewPresenter.IsValidRace4)
            {
                cmbRace5.Enabled = true;
            }
            else
            {
                cmbRace5.Enabled = false;
                cmbRace5.SelectedIndex = -1;
            }
        }
         
        private void SaveAdditionalRaces()
        {
            AdditionalRacesViewPresenter.UpdateRace3ToModel();
            AdditionalRacesViewPresenter.UpdateRace4ToModel();
            AdditionalRacesViewPresenter.UpdateRace5ToModel();

        }

        private void btnSaveResponse_Click(object sender, EventArgs e)
        {
            SaveAdditionalRaces();
            CloseView();
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            CloseView();
        }
        private void CloseView()
        {
            Close();
        }
        #endregion

        #region public method
        public override void UpdateView()
        {
            AdditionalRacesViewPresenter = new AdditionalRacesViewPresenter(this, ModelAccount,
                new AdditionalRacesFeatureManager());
            AdditionalRacesViewPresenter.UpdateView();

        }

        public void SetRace3DeactivatedBgColor()
        {
            UIColors.SetDeactivatedBgColor(cmbRace3);
        }
        public void ProcessRace3InvalidCodeEvent()
        {
            ProcessInvalidCodeEvent(cmbRace3);
        }
        public void SetRace4DeactivatedBgColor()
        {
            UIColors.SetDeactivatedBgColor(cmbRace4);
        }
        public void ProcessRace4InvalidCodeEvent()
        {
            ProcessInvalidCodeEvent(cmbRace4);
        }
        public void SetRace5DeactivatedBgColor()
        {
            UIColors.SetDeactivatedBgColor(cmbRace5);
        }
        public void ProcessRace5InvalidCodeEvent()
        {
            ProcessInvalidCodeEvent(cmbRace5);
        }
        private static void ProcessInvalidCodeEvent(Control comboBox)
        {
            UIColors.SetDeactivatedBgColor(comboBox);

            MessageBox.Show(UIErrorMessages.INVALID_VALUE_ERRMSG, "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button1);

            if (!comboBox.Focused)
            {
                comboBox.Focus();
            }
        }
        public void PopulateRace(ArrayList raceArrayList)
        {
            cmbRace3.Items.Clear();
            cmbRace4.Items.Clear();
            cmbRace5.Items.Clear();
            foreach (var race in raceArrayList)
            {
                if (!cmbRace3.Items.Contains(race))
                {
                    cmbRace3.Items.Add(race);
                }
                if (!cmbRace4.Items.Contains(race))
                {
                    cmbRace4.Items.Add(race);
                }
                if (!cmbRace5.Items.Contains(race))
                {
                    cmbRace5.Items.Add(race);
                }
            }
        }

        public Race Race3
        {
            get { return cmbRace3.SelectedItem as Race;}
            set { cmbRace3.SelectedItem = value; }
        }
        public Race Race4 
        { 
            get{return cmbRace4.SelectedItem as Race;}
            set { cmbRace4.SelectedItem = value; } 
        }
        public Race Race5
        {
            get { return cmbRace5.SelectedItem as Race; }
            set { cmbRace5.SelectedItem = value; }
        }
        public void EnableRace4ComboBox()
        {
            cmbRace4.Enabled = true;
        }

        public void DisableRace4ComboBox()
        {
            cmbRace4.Enabled = false;
        }

        public void EnableRace5ComboBox()
        {
            cmbRace5.Enabled = true;
        }
        public void DisableRace5ComboBox()
        {
            cmbRace5.Enabled = false;
        }
        public void ShowAdditionalRacesView()
        {
            CenterToScreen();
            ShowDialog();
        }

        #endregion

    }
}
