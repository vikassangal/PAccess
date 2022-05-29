using System;
using System.ComponentModel;
using System.Windows.Forms;
using Infragistics.Win.UltraWinToolbars;
using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI.DemographicsViews
{
    partial class RaceView
    {
        /// <summary>
        /// Required designer variable.
        ///  </summary>
        private System.ComponentModel.IContainer components = null;
        private PatientAccessTreeDropdownBox raceControl;
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.raceControl = new PatientAccessTreeDropdownBox();
            this.SuspendLayout();
            // 
            // raceControl
            // 
            this.raceControl.Location = new System.Drawing.Point(10, 50);
            this.raceControl.Name = "raceControl";
            this.raceControl.Model = null;
            this.raceControl.Size = new System.Drawing.Size(135, 21);
            this.raceControl.TabStop = false;
            this.raceControl.UltraToolbarsManager.ToolClick += new ToolClickEventHandler(raceControl_TreeDropdownBoxClickEvent);
            this.raceControl.UltraDropDownButton.Validating += new CancelEventHandler(raceControl_UltraDropDownButton_Validating);
            // 
            // RaceView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.raceControl);
            this.Name = "RaceView";
            this.Leave += new EventHandler(RaceView_Leave);
            this.Size = new System.Drawing.Size(135, 25);
            this.ResumeLayout(false);

        }
        
       

        #endregion
    }
}