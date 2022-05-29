using System.Windows.Forms;
using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI.PAIWalkinAccountCreation.ViewImpl
{
    partial class PAIWalkinFinancialClassesView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
        {
            Application.DoEvents();

            this.UnRegisterRulesEvents();

            if ( disposing && ( components != null ) )
            {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cmboFinancialClass = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.lblFinancialClass = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cmboFinancialClass
            // 
            this.cmboFinancialClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmboFinancialClass.Location = new System.Drawing.Point(13, 23);
            this.cmboFinancialClass.Name = "cmboFinancialClass";
            this.cmboFinancialClass.Size = new System.Drawing.Size(260, 28);
            this.cmboFinancialClass.TabIndex = 0;
            this.cmboFinancialClass.SelectedIndexChanged += new System.EventHandler(this.FinancialClassesSelectedIndexChanged);
            this.cmboFinancialClass.Validating += new System.ComponentModel.CancelEventHandler(this.cmboFinancialClass_Validating);
            // 
            // lblFinancialClass
            // 
            this.lblFinancialClass.Location = new System.Drawing.Point(13, 0);
            this.lblFinancialClass.Name = "lblFinancialClass";
            this.lblFinancialClass.Size = new System.Drawing.Size(124, 17);
            this.lblFinancialClass.TabIndex = 0;
            this.lblFinancialClass.Text = "Financial Class:";
            // 
            // PAIWalkinFinancialClassesView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cmboFinancialClass);
            this.Controls.Add(this.lblFinancialClass);
            this.Name = "PAIWalkinFinancialClassesView";
            this.Size = new System.Drawing.Size(242, 54);
            this.Load += new System.EventHandler(this.PAIWalkinFinancialClassesView_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private PatientAccessComboBox cmboFinancialClass;
        private Label lblFinancialClass;
    }
}
