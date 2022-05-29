using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms; 
using PatientAccess.Domain;
 
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.DemographicsViews.Presenters;
using PatientAccess.UI.DemographicsViews.Views;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.DemographicsViews.ViewImpl
{
    /// <summary>
    /// Summary description for GenderView.
    /// </summary>
    public class GenderView : ControlView, IGenderView
    {
         
        #region Admin Gender Event Handlers

        private void genderControl_GenderControlValidating(object sender, CancelEventArgs e)
        {
            GenderViewPresenter.ValidateGender();
        }

        private void GenderSelectedEvent(object sender, EventArgs e)
        {
            var args = (LooseArgs) e;
            if (args != null)
            {
                GenderViewPresenter.UpdateGenderSelected(args.Context as Gender);
            }
        }

        public void MakeGenderControlError()
        {
            UIColors.SetDeactivatedBgColor(genderControl.ComboBox);
        }

        public void ProcessInvalidCode()
        {
            ProcessInvalidCodeEvent(genderControl.ComboBox);
        }

        private static void ProcessInvalidCodeEvent(Control comboBox)
        {
            UIColors.SetDeactivatedBgColor(comboBox);

            MessageBox.Show(UIErrorMessages.INVALID_VALUE_ERRMSG, UIErrorMessages.ERROR,
                MessageBoxButtons.OK, MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button1);

            if (!comboBox.Focused)
            {
                comboBox.Focus();
            }
        }

   
        #endregion Admin Gender Event Handlers

        #region Methods
         

        public void UpdateGenderOnView(DictionaryEntry gender)
        {
            genderControl.ComboBox.SelectedItem = gender;
        }
         
        #endregion

        #region Properties
         
        public void SetNormal()
        {
            UIColors.SetNormalBgColor(GenderControl.ComboBox);
            Refresh();
        }

        public void MakeGenderRequired()
        {
            UIColors.SetRequiredBgColor(genderControl.ComboBox);
            Refresh();
        }
         
        public GenderControl GenderControl
        {
            get { return genderControl; } 
        }

        public GenderViewPresenter GenderViewPresenter { get; set; }

        #endregion

        #region Private Methods

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

        #region Construction and Finalization
        public GenderView()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
        }

        #endregion

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.genderControl = new PatientAccess.UI.CommonControls.GenderControl();
            this.SuspendLayout();
            // 
            // genderControl
            // 
            this.genderControl.Location = new System.Drawing.Point(2, 2);
            this.genderControl.Model = null;
            this.genderControl.Name = "genderControl";
            this.genderControl.Size = new System.Drawing.Size(85, 21);
            this.genderControl.TabIndex = 1;
            this.genderControl.GenderSelectedEvent += new System.EventHandler(this.GenderSelectedEvent);
            this.genderControl.GenderControlValidating += new System.ComponentModel.CancelEventHandler(this.genderControl_GenderControlValidating);
          
            // 
            // GenderView
            // 
            this.Controls.Add(this.genderControl);
            this.Name = "GenderView";
            this.Size = new System.Drawing.Size(93, 24);
            this.ResumeLayout(false);

        }

        #endregion

        #region Data Elements

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null; 
        private GenderControl genderControl; 

        #endregion
    }
     
}
