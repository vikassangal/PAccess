using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Extensions.UI.Winforms;

namespace PatientAccess.UI.CensusInquiries
{
    /// <summary>
    /// Summary description for PatientCensusResultsView.
    /// </summary>
    [Serializable]
    public class CensusbyPatientView : ControlView
    {
        #region Events
        #endregion

        #region Event Handlers

        private void AccountsFound( object sender, EventArgs e )
        {
            patientCensusResultsView.Model = null;
            patientCensusResultsView.Model = 
                ( ArrayList ) ( (LooseArgs)e ).Context;
            patientCensusResultsView.Show();
            patientCensusResultsView.UpdateView();
        }

        private void ResetView( object sender, EventArgs e )
        {
            this.patientCensusResultsView.ResetView();
        }

        private void NoAccountsFound( object sender, EventArgs e )
        {            
            this.patientCensusResultsView.DisplayNoAccountsFound();
        }

        #endregion

        #region Methods
        public void SetRowSelectionActiveAppearance()
        {
            if( this.patientCensusResultsView.ContainsFocus  )
            {
                this.patientCensusResultsView.SetRowSelectionActiveAppearance();
            }
        }
        public void SetRowSelectionInActiveAppearance()
        {
            this.patientCensusResultsView.SetRowSelectionInActiveAppearance();
            
        }
        #endregion
        
        #region Windows Form Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.patientCensusSearchViewPanel = new System.Windows.Forms.Panel();
            this.patientCensusSearchView = new PatientAccess.UI.CensusInquiries.PatientCensusSearchView();
            this.patientCensusResultsViewPanel = new System.Windows.Forms.Panel();
            this.patientCensusResultsView = new PatientAccess.UI.CensusInquiries.PatientCensusResultsView();
            this.patientCensusSearchViewPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // patientCensusSearchViewPanel
            // 
            this.patientCensusSearchViewPanel.Controls.Add(this.patientCensusSearchView);
            this.patientCensusSearchViewPanel.Location = new System.Drawing.Point(0, 0);
            this.patientCensusSearchViewPanel.Name = "patientCensusSearchViewPanel";
            this.patientCensusSearchViewPanel.Size = new System.Drawing.Size(926, 72);
            this.patientCensusSearchViewPanel.TabIndex = 0;
            // 
            // patientCensusSearchView
            // 
            this.patientCensusSearchView.BackColor = System.Drawing.Color.White;
            this.patientCensusSearchView.Location = new System.Drawing.Point(10, 0);
            this.patientCensusSearchView.Model = null;
            this.patientCensusSearchView.Name = "patientCensusSearchView";
            this.patientCensusSearchView.Size = new System.Drawing.Size(926, 72);
            this.patientCensusSearchView.TabIndex = 0;
            this.patientCensusSearchView.AccountsFound += new System.EventHandler(this.AccountsFound);

            this.patientCensusSearchView.BeforeWorkEvent +=
                new EventHandler(patientCensusResultsView.BeforeWork);
            this.patientCensusSearchView.AfterWorkEvent +=
                new EventHandler(patientCensusResultsView.AfterWork);
            // 
            // patientCensusResultsViewPanel
            // 
            this.patientCensusResultsViewPanel.Location = new System.Drawing.Point(0, 84);
            this.patientCensusResultsViewPanel.Name = "patientCensusResultsViewPanel";
            this.patientCensusResultsViewPanel.Size = new System.Drawing.Size(926, 374);
            this.patientCensusResultsViewPanel.TabIndex = 1;
            this.patientCensusResultsViewPanel.TabStop = true;
            // 
            // CensusbyPatientView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.patientCensusResultsViewPanel);
            this.Controls.Add(this.patientCensusSearchViewPanel);
            this.Name = "CensusbyPatientView";
            this.Size = new System.Drawing.Size(926, 470);
            this.patientCensusSearchViewPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods

        private void WireupPatientCensusSearchView()
        {
            this.patientCensusSearchView.AccountsFound      += 
                new EventHandler( this.AccountsFound );
            this.patientCensusSearchView.ResetView          += 
                new EventHandler( this.ResetView );
            this.patientCensusSearchView.NoAccountsFound    += 
                new EventHandler( this.NoAccountsFound );
            
            this.patientCensusResultsView.BackColor = 
                Color.White;
            this.patientCensusResultsView.Location = 
                new Point( 10, 0 );
            this.patientCensusResultsView.Model = null;
            this.patientCensusResultsView.Name = "patientCensusResultsView";
            this.patientCensusResultsView.Size = 
                new Size( 899, 374 );
            this.patientCensusResultsView.TabIndex = 0;
            this.patientCensusResultsView.TabStop = true;
            this.patientCensusResultsView.AcceptButton = base.AcceptButton;
            this.patientCensusResultsViewPanel.Controls.Add( 
                this.patientCensusResultsView );           
        }

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public CensusbyPatientView()
        {
            InitializeComponent();
            WireupPatientCensusSearchView();
            base.EnableThemesOn( this );
        }

        /// <summary>
        /// Dispose method.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if ( components != null ) 
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }

        #endregion
        
        #region Data Elements

        private IContainer                            components = null;

        private Panel                                  patientCensusSearchViewPanel;
        private Panel                                  patientCensusResultsViewPanel;

        private PatientCensusSearchView    patientCensusSearchView;
        private PatientCensusResultsView   patientCensusResultsView;

        #endregion

        #region Constants
        #endregion

    }
}
