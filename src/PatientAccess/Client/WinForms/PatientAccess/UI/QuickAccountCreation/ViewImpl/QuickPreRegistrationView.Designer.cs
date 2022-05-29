using System;
using System.Windows.Forms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.UI.PatientSearch;

namespace PatientAccess.UI.QuickAccountCreation.ViewImpl
{
    partial class QuickPreRegistrationView
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
            SearchEventAggregator.GetInstance().AccountSelected -= AccountSelectedEventHandler;
            SearchEventAggregator.GetInstance().ActivatePreregisteredAccount -= ActivatePreregisteredAccountEventHandler;

            if ( disposing )
            {
                if ( components != null )
                {
                    components.Dispose();
                }
                // cancel the background worker here...
                CancelBackgroundWorker();
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
            this.masterPatientIndexView = new PatientAccess.UI.PatientSearch.MasterPatientIndexView();
            this.masterPatientIndexView.ReturnToMainScreen += new EventHandler( OnReturnToMainScreen );

            this.SuspendLayout();
            // 
            // masterPatientIndexView
            // 
            this.masterPatientIndexView.Location = new System.Drawing.Point( 0, 0 );
            this.masterPatientIndexView.Model = null;
            this.masterPatientIndexView.Name = "masterPatientIndexView";
            this.masterPatientIndexView.Size = new System.Drawing.Size( 1024, 512 );
            this.masterPatientIndexView.Dock = DockStyle.Fill;
            this.masterPatientIndexView.TabIndex = 0;
            // 
            // PreRegistrationView
            // 
            this.Controls.Add( this.masterPatientIndexView );
            this.Name = "PreRegistrationView";
            this.Size = new System.Drawing.Size( 1024, 512 );
            this.ResumeLayout( false );

        }

        #endregion
        #region Data Elements
        private IAccountView accountView;
        private MasterPatientIndexView masterPatientIndexView;
     
        #endregion
    }
}
