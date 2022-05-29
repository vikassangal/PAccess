using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using Extensions.UI.Winforms;
using log4net;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.UI.PatientSearch;
using PatientAccess.UI.PreMSEViews;

namespace PatientAccess.UI.Registration
{
    partial class NewbornPreRegistrationView 
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private Container                components = null;
        private MasterPatientIndexView   masterPatientIndexView;

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.masterPatientIndexView = new PatientAccess.UI.PatientSearch.MasterPatientIndexView();
            //this.masterPatientIndexView.ReturnToMainScreen += new EventHandler( OnReturnToMainScreen );
            this.SuspendLayout();
            
            this.masterPatientIndexView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.masterPatientIndexView.Location = new System.Drawing.Point(0, 0);
            this.masterPatientIndexView.Model = null;
            this.masterPatientIndexView.Name = "masterPatientIndexView";
            this.masterPatientIndexView.Size = new System.Drawing.Size(1024, 512);
            this.masterPatientIndexView.TabIndex = 0;
            
            this.Controls.Add(this.masterPatientIndexView);
            this.Name = "newbornPreRegistrationView";
            this.Size = new System.Drawing.Size(1024, 512);
            this.ResumeLayout(false);

        }
        #endregion
    }
}
