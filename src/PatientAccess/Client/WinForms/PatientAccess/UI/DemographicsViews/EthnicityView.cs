
using System;
using System.Collections;
using System.Collections.Generic;
using Extensions.UI.Winforms;
using Infragistics.Win.UltraWinToolbars;
using PatientAccess.Domain;
using System.ComponentModel;
using System.Windows.Forms;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.DemographicsViews
{
    public partial class EthnicityView : ControlView, IEthnicityView
    {
        #region Variable Declaration
        
        private const string blankethnicity = "00";
        private bool blnLeaveRun;

        #endregion

        # region Constructor

        public EthnicityView()
        {
            InitializeComponent();
        }

        #endregion

        # region Properties

        public Account ModelAccount
        {
            get
            {
                return (Account)Model;
            }
            set
            {
                Model = value;
            }
        }
        
        public Ethnicity Ethnicity
        {
            get { return ethnicityControl.ButtonTool.SharedProps.Tag as Ethnicity; }

            set
            {
                ethnicityControl.UltraDropDownButton.Text = value.Description;
            }
        }
      
        #endregion

      
        public void PopulateEthnicity(IDictionary<Ethnicity, ArrayList> ethnicityCollection)
        {
            if (ethnicityCollection != null)
            {
                foreach (var ethnicity in ethnicityCollection)
                {
                    if (ethnicity.Key.Code.Equals(blankethnicity) || ethnicity.Value.Count == 0)
                    {
                        ethnicityControl.ButtonTool = new ButtonTool(ethnicity.Key.Code);
                        ethnicityControl.ButtonTool.SharedProps.Caption = ethnicity.Key.Description;
                        ethnicityControl.ButtonTool.SharedProps.Tag = ethnicity.Key;
                        ethnicityControl.PopupMenuTool.SharedProps.Tag = ethnicity.Key;
                        ethnicityControl.UltraToolbarsManager.Tools.Add(ethnicityControl.ButtonTool);
                        ethnicityControl.PopupMenuTool.Tools.Add(ethnicityControl.ButtonTool);
                    }
                    else
                    {
                        var ethnicityMenuTool = GetEthnicityPopupMenuTool(ethnicity);
                        ethnicityControl.PopupMenuTool.Tools.Add(ethnicityMenuTool);
                        foreach (var subItems in ethnicity.Value)
                        {
                            ethnicityControl.ButtonTool = new ButtonTool(((Ethnicity)(((ValidatingCode)(subItems)))).Code);
                            ethnicityControl.ButtonTool.SharedProps.Caption = ((Ethnicity)(((ValidatingCode)(subItems)))).Description;
                            ethnicityControl.ButtonTool.SharedProps.Tag = ((Ethnicity)(((ValidatingCode)(subItems))));
                            ethnicityControl.UltraToolbarsManager.Tools.Add(ethnicityControl.ButtonTool);
                            ethnicityMenuTool.Tools.Add(ethnicityControl.ButtonTool);
                        }
                    }
                }
            }
          
            this.Controls.Add(this.ethnicityControl.UltraDropDownButton);
            ((ISupportInitialize)(this.ethnicityControl.UltraToolbarsManager)).EndInit();
            this.ethnicityControl.UltraToolbarsManager.DockWithinContainer = this;
            
        }
        
        private PopupMenuTool GetEthnicityPopupMenuTool(KeyValuePair<Ethnicity, ArrayList> ethnicity)
           
        {
            PopupMenuTool popupMenuToolMenuTool =
                new PopupMenuTool(ethnicity.Key.Code);
            popupMenuToolMenuTool.SharedProps.Caption = ethnicity.Key.Description;
            popupMenuToolMenuTool.SharedProps.Tag = ethnicity.Key;
            popupMenuToolMenuTool.DropDownArrowStyle = DropDownArrowStyle.SegmentedStateButton;
            this.ethnicityControl.UltraToolbarsManager.Tools.Add(popupMenuToolMenuTool);
            return popupMenuToolMenuTool;
        }
        #region Event Handlers

        private void ethnicityControl_UltraDropDownButton_Validating(object sender, CancelEventArgs e)
        {
            if (blnLeaveRun)
            {
                SetNormalBgColor();
                Refresh();
                EthnicityViewPresenter.RunInvalidCodeRules();
            }

            EthnicityViewPresenter.RunRules();
        }
     
        private void ethnicityControl_TreeDropdownBoxClickEvent(object sender, ToolClickEventArgs e)
        {
            if (ModelAccount == null || ModelAccount.Patient == null)
            {
                return;
            }
            
            if (ModelAccount.Patient.Ethnicity == null)
            {
                ModelAccount.Patient.Ethnicity = new Ethnicity();
            }

            if (e != null &&
                e.Tool != null)
            {
                SetNormalBgColor();
                var ethnicity = e.Tool.SharedProps.Tag as Ethnicity;
                if (e.Tool.SharedProps.Tag != null)
                {
                    ethnicityControl.UltraDropDownButton.Text = ethnicity.Description;
                    EthnicityViewPresenter.UpdateEthnicityAndDescentModelValue(e.Tool.SharedProps.Tag as Ethnicity);
                }
               
                EthnicityViewPresenter.RunRules();
            }

        }
        public void SetDeactivatedBgColor()
        {
            UIColors.SetDeactivatedBgColor(ethnicityControl.UltraToolbarsManager);
        }

        public void ProcessInvalidCodeEvent()
        {
            ProcessInvalidCodeEvent(ethnicityControl.UltraToolbarsManager);
        }

        public void SetEthnicityAsRequiredColor()
        {
            UIColors.SetRequiredBgColor(ethnicityControl.UltraToolbarsManager);
        }

        public void SetEthnicityAsPreferredColor()
        {
            UIColors.SetPreferredBgColor(ethnicityControl.UltraToolbarsManager);
        }

        public void SetNormalBgColor()
        {
            UIColors.SetNormalBgColor(ethnicityControl.UltraToolbarsManager);
        }

        public IEthnicityViewPresenter EthnicityViewPresenter { get; set; }

        public void SetSizeForEthnicityDropdownButton()
        {
            ethnicityControl.UltraDropDownButton.Size = new System.Drawing.Size(118, 21);
        }
        
        private static void ProcessInvalidCodeEvent(UltraToolbarsManager ultraToolbar)
        {
            UIColors.SetDeactivatedBgColor(ultraToolbar);

            MessageBox.Show(UIErrorMessages.INVALID_VALUE_ERRMSG, "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button1);
        }
     
        #endregion  

        #region Private Method

        private void EthnicityView_Leave(object sender, EventArgs e)
        {
            blnLeaveRun = true;
        }
    }

    #endregion
}
