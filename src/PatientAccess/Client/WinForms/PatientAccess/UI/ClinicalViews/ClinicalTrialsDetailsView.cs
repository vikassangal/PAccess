using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.ClinicalViews
{
    public partial class ClinicalTrialsDetailsView : TimeOutFormView, IClinicalTrialsDetailsView
    {
        public ClinicalTrialsDetailsPresenter Presenter { get; set; }

        public ClinicalTrialsDetailsView()
        {
            InitializeComponent();
            SetBackgroundColor();
            CreateAllResearchStudiesDataTableColumns();
        }

        private void SetBackgroundColor()
        {
            BackColor = UIColors.FormBackColor;
        }

        public void ShowMe()
        {
            ShowDialog();
        }

        private void UpdateStudySelectionList( IEnumerable<ResearchStudy> allStudies )
        {
            dataGridView1.AutoGenerateColumns = false;
            GenerateAllResearchStudiesDataTable( allStudies );
            dataGridView1.DataSource = allStudiesDataTable;
        }

        private void CreateAllResearchStudiesDataTableColumns()
        {
            allStudiesDataTable.Columns.Add( sponsorDataColumn );
            allStudiesDataTable.Columns.Add( descriptionDataColumn );
            allStudiesDataTable.Columns.Add( researchCodeDataColumn );
            allStudiesDataTable.Columns.Add( registryNumberDataColumn );
            allStudiesDataTable.Columns.Add( isExpiredDataColumn );
            allStudiesDataTable.Columns.Add(researchStudyColumn);
        }

        private void GenerateAllResearchStudiesDataTable( IEnumerable<ResearchStudy> allStudies )
        {
            allStudiesDataTable.Rows.Clear();

            foreach ( ResearchStudy researchStudy in allStudies )
            {
                DataRow row = allStudiesDataTable.NewRow();

                row[RESEARCH_SPONSOR] = researchStudy.ResearchSponsor;
                row[RESEARCH_DESCRIPTION] = researchStudy.Description;
                row[RESEARCH_STUDY_CODE] = researchStudy.Code;
                row[REGISTRY_NUMBER] = researchStudy.RegistryNumber;
                row[IS_EXPIRED] = Presenter.IsResearchStudyExpired(researchStudy);
                row[RESEARCH_STUDY] = researchStudy;

                allStudiesDataTable.Rows.Add( row );
            }
        }

        private void UpdatePatientStudyList( IEnumerable<ConsentedResearchStudy> patientStudies )
        {
            dataGridView2.AutoGenerateColumns = false;
            dataGridView2.DataSource = new List<ConsentedResearchStudy>( patientStudies );
        }

        public void Update( IEnumerable<ConsentedResearchStudy> patientStudies, IEnumerable<ResearchStudy> studySelectionList )
        {
            //this is a bit of a hack to solve the issue when extra spacing appears in the 
            //grid when no items are available, this happens because the grid does not show
            //the sorting glyph, I could not find any properties for the grid what would 
            //automatically take care of this sizing issue
            if ( studySelectionList.Count() < 1 )
            {
                ResearchStudyCode.Width = 160;
            }

            UpdatePatientStudyList( patientStudies );
            UpdateStudySelectionList( studySelectionList );
        }

        public void CloseMe()
        {
            Close();
        }

        public bool SaveCommandEnabled
        {
            get { return btnOK.Enabled; }
            set { btnOK.Enabled = value; }
        }

        public bool EnrollCommandsEnabled
        {
            get { return btnEnrollWithConsent.Enabled && btnEnrollWithoutConsent.Enabled; }
            set
            {
                btnEnrollWithConsent.Enabled = value;
                btnEnrollWithoutConsent.Enabled = value;
            }
        }

        public bool RemoveCommandEnabled
        {
            get { return btnRemoveSelectedStudy.Enabled; }
            set { btnRemoveSelectedStudy.Enabled = value; }
        }

        public bool ShowExpiredStudies { get; set; }
        

        private void ClinicalTrialsDetailsView_FormClosing( object sender, FormClosingEventArgs e )
        {
            if ( Presenter.ShowWarningMessage() )
            {
                var result = MessageBox.Show( UIErrorMessages.WILL_LOSE_CLINICALTRIALS_DATA_ON_DETAILS_SCREEN, "Warning!",
                                              MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2 );
                if ( result == DialogResult.Yes )
                {
                    Presenter.DiscardChanges();
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        private void btnEnrollWithoutConsent_Click( object sender, EventArgs e )
        {
            Presenter.EnrollWithoutConsent();
        }

        private void btnEnrollWithConsent_Click( object sender, EventArgs e )
        {
            Presenter.EnrollWithConsent();
        }

        private void btnRemoveSelectedStudy_Click( object sender, EventArgs e )
        {
            Presenter.RemoveSelectedPatientStudy();
        }

        private void dataGridView1_SelectionChanged( object sender, EventArgs e )
        {
            if ( dataGridView1.SelectedRows.Count > 0 )
            {
                var selectedDataRowView = ( DataRowView )dataGridView1.SelectedRows[0].DataBoundItem;

                var selectedStudy = (ResearchStudy)selectedDataRowView.Row[RESEARCH_STUDY];

                Presenter.UpdateSelectedStudyInSelectionList( selectedStudy );
            }
        }

        private void btnOK_Click( object sender, EventArgs e )
        {
            Presenter.SaveChangesAndExit();
        }

        private void dataGridView2_SelectionChanged( object sender, EventArgs e )
        {
            if ( dataGridView2.SelectedRows.Count > 0 )
            {
                var selectedStudy = ( ConsentedResearchStudy )dataGridView2.SelectedRows[0].DataBoundItem;
                Presenter.UpdateSelectedPatientStudy( selectedStudy );
            }
        }

        private void expandCollapseStudiesToSelectButton_Click( object sender, EventArgs e )
        {
            selectedStudiesPanel.Visible = !selectedStudiesPanel.Visible;

            switch ( expandCollapseStudiesToSelectButton.ImageIndex )
            {
                case 0:
                    expandCollapseStudiesToSelectButton.ImageIndex = 1;
                    break;
                
                case 1:
                    expandCollapseStudiesToSelectButton.ImageIndex = 0;
                    break;
            } 
        }

        private void showExpiredStudiesCheckBox_CheckedChanged( object sender, EventArgs e )
        {
            ShowExpiredStudies = showExpiredStudiesCheckBox.Checked;
            Presenter.UpdateView();
        }

        private void dataGridView1_CellFormatting( object sender, DataGridViewCellFormattingEventArgs e )
        {
            if ( e.Value != null )
            {
                if ( (bool)( (DataRowView)dataGridView1.Rows[e.RowIndex].DataBoundItem ).Row[IS_EXPIRED] )
                {
                    dataGridView1.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.DarkGray;
                    dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightGray;
                }
            }
        }

        #region Constants

        private const string RESEARCH_STUDY_CODE = "ResearchStudyCode";
        private const string RESEARCH_DESCRIPTION = "Description";
        private const string RESEARCH_SPONSOR = "ResearchSponsor";
        private const string REGISTRY_NUMBER = "RegistryNumber";
        private const string IS_EXPIRED = "IsExpired";
        private const string RESEARCH_STUDY = "ResearchStudy";

        #endregion

        #region Data Elements

        DataTable allStudiesDataTable = new DataTable();
        DataColumn sponsorDataColumn = new DataColumn( RESEARCH_SPONSOR, typeof( string ) );
        DataColumn descriptionDataColumn = new DataColumn( RESEARCH_DESCRIPTION, typeof( string ) );
        DataColumn researchCodeDataColumn = new DataColumn( RESEARCH_STUDY_CODE, typeof( string ) );
        DataColumn registryNumberDataColumn = new DataColumn( REGISTRY_NUMBER, typeof( string ) );
        DataColumn isExpiredDataColumn = new DataColumn( IS_EXPIRED, typeof( bool ) );
        DataColumn researchStudyColumn = new DataColumn( RESEARCH_STUDY, typeof( ResearchStudy ) );

        #endregion
    }
}
