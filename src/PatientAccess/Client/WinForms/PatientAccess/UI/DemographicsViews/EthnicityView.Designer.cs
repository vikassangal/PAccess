using System;
using System.ComponentModel;
using System.Windows.Forms;
using Infragistics.Win.UltraWinToolbars;
using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI.DemographicsViews
{
    partial class EthnicityView
    {
        /// <summary>
        /// Required designer variable.
        ///  </summary>
        private System.ComponentModel.IContainer components = null;
        private PatientAccessTreeDropdownBox ethnicityControl;
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
            this.ethnicityControl = new PatientAccessTreeDropdownBox();
            this.SuspendLayout();
            // 
            // ethnicityControl
            // 
            this.ethnicityControl.Location = new System.Drawing.Point(10, 50);
            this.ethnicityControl.Name = "ethnicityControl";
            this.ethnicityControl.Model = null;
            this.ethnicityControl.Size = new System.Drawing.Size(135, 21);
            this.ethnicityControl.TabStop = false;
            this.ethnicityControl.UltraToolbarsManager.ToolClick += new ToolClickEventHandler(ethnicityControl_TreeDropdownBoxClickEvent);
            this.ethnicityControl.UltraDropDownButton.Validating += new CancelEventHandler(ethnicityControl_UltraDropDownButton_Validating);
            // 
            // EthnicityView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ethnicityControl);
            this.Name = "EthnicityView";
            this.Leave += new EventHandler(EthnicityView_Leave);
            this.Size = new System.Drawing.Size(135, 25);
            this.ResumeLayout(false);

        }
        
       

        #endregion
    }
}