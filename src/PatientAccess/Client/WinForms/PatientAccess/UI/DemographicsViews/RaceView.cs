
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
    public partial class RaceView : ControlView, IRaceView
    {
        #region Variable Declaration
        
        private const string blankRace = "00";
        private bool blnLeaveRun;

        #endregion

        # region Constructor

        public RaceView()
        {
            InitializeComponent();
        }

        #endregion

        # region Properties
        public IRaceViewPresenter RaceViewPresenter { get; set; }

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
        
        public Race Race
        {
            get { return raceControl.ButtonTool.SharedProps.Tag as Race; }

            set
            {
                raceControl.UltraDropDownButton.Text = value.Description;
            }
        }
       
        #endregion

        #region Public Methods

        public void PopulateRace(IDictionary<Race, ArrayList> raceNationalityDictionary)
        {
            if (raceNationalityDictionary != null)
            {
                foreach (var race in raceNationalityDictionary)
                {
                    if (race.Key.Code.Equals(blankRace) || race.Value.Count == 0)
                    {
                        raceControl.ButtonTool = new ButtonTool(race.Key.Code);
                        raceControl.ButtonTool.SharedProps.Caption = race.Key.Description;
                        raceControl.ButtonTool.SharedProps.Tag = race.Key;
                        raceControl.UltraToolbarsManager.Tools.Add(raceControl.ButtonTool);
                        raceControl.PopupMenuTool.Tools.Add(raceControl.ButtonTool);
                    }
                    else
                    {
                        var raceMenuTool = GetRacePopupMenuTool(race);
                        raceControl.PopupMenuTool.Tools.Add(raceMenuTool);
                        foreach (var subItems in race.Value)
                        {
                            raceControl.ButtonTool = new ButtonTool(((Race) (((ValidatingCode) (subItems)))).Code);
                            raceControl.ButtonTool.SharedProps.Caption =
                                ((Race) (((ValidatingCode) (subItems)))).Description;
                            raceControl.ButtonTool.SharedProps.Tag = ((Race) (((ValidatingCode) (subItems))));
                            raceControl.UltraToolbarsManager.Tools.Add(raceControl.ButtonTool);
                            raceMenuTool.Tools.Add(raceControl.ButtonTool);
                        }
                    }
                }
            }

            this.Controls.Add(this.raceControl.UltraDropDownButton);
            ((ISupportInitialize) (this.raceControl.UltraToolbarsManager)).EndInit();
            this.raceControl.UltraToolbarsManager.DockWithinContainer = this;
        }
        public void SetDeactivatedBgColor()
        {
            UIColors.SetDeactivatedBgColor(raceControl.UltraToolbarsManager);
        }

        public void ProcessInvalidCodeEvent()
        {
            ProcessInvalidCodeEvent(raceControl.UltraToolbarsManager);
        }

        public void SetRaceAsRequiredColor()
        {
            UIColors.SetRequiredBgColor(raceControl.UltraToolbarsManager);
        }

        public void SetRaceAsPreferredColor()
        {
            UIColors.SetPreferredBgColor(raceControl.UltraToolbarsManager);
        }

        public void SetNormalBgColor()
        {
            UIColors.SetNormalBgColor(raceControl.UltraToolbarsManager);
        }
        
        public void SetSizeForRaceDropdownButton()
        {
            raceControl.UltraDropDownButton.Size = new System.Drawing.Size(118, 21);
        }
 
        #endregion
        
        #region Event Handlers

        private void raceControl_UltraDropDownButton_Validating(object sender, CancelEventArgs e)
        {
            if (blnLeaveRun)
            {
                SetNormalBgColor();
                Refresh();
                RaceViewPresenter.RunInvalidCodeRules();
            }

            RaceViewPresenter.RunRules();
        }

        private void raceControl_TreeDropdownBoxClickEvent(object sender, ToolClickEventArgs e)
        {
            if (ModelAccount == null || ModelAccount.Patient == null)
            {
                return;
            }
            
            if (e != null &&
                e.Tool != null)
            {
                SetNormalBgColor();
                if (e.Tool.SharedProps.Tag != null)
                {
                    var race = e.Tool.SharedProps.Tag as Race;
                    if (race != null)
                    {
                        raceControl.UltraDropDownButton.Text = race.Description;
                        RaceViewPresenter.UpdateRaceAndNationalityModelValue(race);
                    }
                }
                RaceViewPresenter.RunRules();
            }
        }
        
        #endregion  

        #region Private Method

        private PopupMenuTool GetRacePopupMenuTool(KeyValuePair<Race, ArrayList> race)
        {
            PopupMenuTool popupMenuToolMenuTool =
                new PopupMenuTool(race.Key.Code);
            popupMenuToolMenuTool.SharedProps.Caption = race.Key.Description;
            popupMenuToolMenuTool.SharedProps.Tag = race.Key;
            popupMenuToolMenuTool.DropDownArrowStyle = DropDownArrowStyle.SegmentedStateButton;
            this.raceControl.UltraToolbarsManager.Tools.Add(popupMenuToolMenuTool);
            return popupMenuToolMenuTool;
        }
        private void RaceView_Leave(object sender, EventArgs e)
        {
            blnLeaveRun = true;
        }
        private static void ProcessInvalidCodeEvent(UltraToolbarsManager ultraToolbar)
        {
            UIColors.SetDeactivatedBgColor(ultraToolbar);

            MessageBox.Show(UIErrorMessages.INVALID_VALUE_ERRMSG, "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button1);
        }

        #endregion
    }

   
}
