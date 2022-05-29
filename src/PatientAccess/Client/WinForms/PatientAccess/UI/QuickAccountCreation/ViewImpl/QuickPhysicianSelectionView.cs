using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Rules;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.QuickAccountCreation.Presenters;
using PatientAccess.UI.QuickAccountCreation.Views;

namespace PatientAccess.UI.QuickAccountCreation.ViewImpl
{
    public partial class QuickPhysicianSelectionView : ControlView, IQuickPhysicianSelectionView
    {
       #region Events
        #endregion

        #region Event Handlers

        private void QuickPhysicianSelectionView_Leave( object sender, EventArgs e )
        {
            AcceptButton = null;
            Refresh();
        }

        private void btnFind_Click( object sender, EventArgs e )
        {
            Cursor = Cursors.WaitCursor;
 
            Presenter.Find();

            Cursor = Cursors.Default;
        }

        private void btnRecNonStaff_Click(object sender, EventArgs e)
        {
            
            Cursor = Cursors.WaitCursor;

            Presenter.RecordNonStaffPhysician();

            Cursor = Cursors.Default;
        }
    
 

        private void btnRefViewDetails_Click( object sender, EventArgs e )
        {
            i_PhysicianRelationship = PhysicianRelationship.REFERRING_PHYSICIAN;
            Presenter.ShowDetails(Model.ReferringPhysician.PhysicianNumber, i_PhysicianRelationship);
        }

        private void btnAdmViewDetails_Click(object sender, EventArgs e)
        {
            i_PhysicianRelationship = PhysicianRelationship.ADMITTING_PHYSICIAN;
            Presenter.ShowDetails(Model.AdmittingPhysician.PhysicianNumber,
                                                              i_PhysicianRelationship);
        }

        private void btnRefClear_Click( object sender, EventArgs e )
        {
            lblRefDisplayVal.Text = String.Empty;
            btnRefViewDetails.Enabled = false;
            btnRefClear.Enabled = false;
            var physicianRelationship = new PhysicianRelationship( PhysicianRole.Referring(), Model.ReferringPhysician );
            Presenter.RemovePhysicianRelationship(physicianRelationship);
            RunRules();
        }

        private void btnAdmClear_Click( object sender, EventArgs e )
        {
            lblAdmDisplayVal.Text = String.Empty;
            btnAdmViewDetails.Enabled = false;
            btnAdmClear.Enabled = false;

            var physicianRelationship = new PhysicianRelationship( PhysicianRole.Admitting(), Model.AdmittingPhysician );
            Presenter.RemovePhysicianRelationship( physicianRelationship );
            RunRules();
        }
        
        #endregion

        #region Rule Event Handlers
        private void ReferringPhysicianRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( lblRefDisplayVal );
        }

        private void AdmittingPhysicianRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( lblAdmDisplayVal );
        }
 
        #endregion

        #region Public Methods
    
        public override void UpdateView()
        {
            Presenter = new QuickPhysicianSelectionPresenter(this, new MessageBoxAdapter(), Model, RuleEngine.GetInstance());
            RegisterRulesEvents();
            Presenter.UpdateViewDetail();
        }

        public void RunRules()
        {
            // reset all fields that might have error, preferred, or required backgrounds
            UIColors.SetNormalBgColor(lblRefDisplayVal);
            if (!Presenter.PhysicianIsValid(Model.ReferringPhysician))
            {
                btnRefViewDetails.Enabled = false;
                btnRefClear.Enabled = false;
            }
            else
            {
                btnRefViewDetails.Enabled = true;
                btnRefClear.Enabled = true;
            }

            UIColors.SetNormalBgColor(lblAdmDisplayVal);
            if ( ! Presenter.PhysicianIsValid( Model.AdmittingPhysician ) )
            {
                btnAdmViewDetails.Enabled = false;
                btnAdmClear.Enabled = false;
            }
            else
            {
                btnAdmViewDetails.Enabled = true;
                btnAdmClear.Enabled = true;
            }
            RegisterRulesEvents();
            RuleEngine.GetInstance().EvaluateRule(typeof (ReferringPhysicianRequired), Model);
            RuleEngine.GetInstance().EvaluateRule(typeof (AdmittingPhysicianRequired), Model);
        }

        public void DisplayAdmittingPhysician( string admittingPhysicianText )
        {
            lblAdmDisplayVal.Text = admittingPhysicianText;
        }
        public void DisplayReferringPhysician( string referringPhysicianText )
        {
            lblRefDisplayVal.Text = referringPhysicianText;
        }
      
        #endregion

        #region public Properties
        public new Account Model
        {
            private get
            {
                return ( Account )base.Model;
            }
            set
            {
                base.Model = value;
            }
        }
        #endregion

        #region Private Methods

        private void PhysicianNum_Enter( object sender, EventArgs e )
        {
            if ( AcceptButton != null )
            {
                i_btnOriginalAcceptButton = ( LoggingButton )AcceptButton;
            }
            AcceptButton = btnVerify;
        }
        
        private void PhysicianNum_Validating(object sender, CancelEventArgs e)
        {
            Presenter.ValidatePhysicianNumber();
            if (i_btnOriginalAcceptButton != null)
            {
                AcceptButton = i_btnOriginalAcceptButton;
            }
            Refresh();
        }
        private void btnVerify_Click( object sender, EventArgs e )
        {
            Cursor = Cursors.WaitCursor;

            Presenter.ValidatePhysicians();
            Refresh();
            Cursor = Cursors.Default;
        }
        public void SetNormalReferringPhysicianTextBox()
        {
            UIColors.SetNormalBgColor(mtbRef);
        }
        public void SetBackColorReferringPhysicianTextBox()
        {
            mtbRef.BackColor = Color.White;
        }
        public void SetErrorReferringPhysicianTextBox()
        {
            UIColors.SetErrorBgColor(mtbRef);
        }

        public void SetNormalAdmissionPhysicianTextBox()
        {
            UIColors.SetNormalBgColor(mtbAdm);
        }
        public void SetErrorAdmissionPhysicianTextBox()
        {
            UIColors.SetErrorBgColor(mtbAdm);
        }

        private void RegisterRulesEvents()
        {
            UnRegisterRulesEvents();         

            RuleEngine.GetInstance().RegisterEvent(typeof (ReferringPhysicianRequired), Model, ReferringPhysicianRequiredEventHandler);
            RuleEngine.GetInstance().RegisterEvent(typeof (AdmittingPhysicianRequired), Model, AdmittingPhysicianRequiredEventHandler);
        }

        private void UnRegisterRulesEvents()
        {
            RuleEngine.GetInstance().UnregisterEvent(typeof (ReferringPhysicianRequired), Model, ReferringPhysicianRequiredEventHandler);
            RuleEngine.GetInstance().UnregisterEvent(typeof (AdmittingPhysicianRequired), Model, AdmittingPhysicianRequiredEventHandler);
        }

        public void ClearSpecifyPhysicianPanel()
        {
            if ( mtbRef.UnMaskedText != string.Empty )
            {
                UIColors.SetNormalBgColor( mtbRef );
                mtbRef.BackColor = Color.White;
                mtbRef.UnMaskedText = string.Empty;
            }
            if ( mtbAdm.UnMaskedText != string.Empty )
            {
                UIColors.SetNormalBgColor( mtbAdm );
                mtbAdm.UnMaskedText = string.Empty;
            }
           
        }
        #endregion

        #region private Properties

      
        private IQuickPhysicianSelectionPresenter Presenter { get; set; }

        #endregion

        #region Construction and Finalization
        public QuickPhysicianSelectionView()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
        }
 
     
        #endregion

        #region Data Elements
        public string AdmPhysicianNumber
        {
            get { return mtbAdm.UnMaskedText; }
            set
            {
                mtbAdm.UnMaskedText = value;
            }
        }
        public string RefPhysicianNumber
        {
            get { return mtbRef.UnMaskedText; }
            set
            {
                mtbRef.UnMaskedText = value;
            }
        }
        private string i_PhysicianRelationship;
    
        #endregion

        #region Constants
        #endregion
    }
}
