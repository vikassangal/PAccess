using System;
using System.Windows.Forms;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Domain.Parties.Exceptions;
using PatientAccess.Rules;
using PatientAccess.UI.HelperClasses;
using PatientAccess.UI.PhysicianSearchViews;
using PatientAccess.UI.QuickAccountCreation.ViewImpl;
using PatientAccess.UI.QuickAccountCreation.Views;

namespace PatientAccess.UI.QuickAccountCreation.Presenters
{
    internal class QuickPhysicianSelectionPresenter : IQuickPhysicianSelectionPresenter
    {
        #region Constructors

        internal QuickPhysicianSelectionPresenter( IQuickPhysicianSelectionView view, IMessageBoxAdapter messageBoxAdapter, Account modelAccount, IRuleEngine ruleEngine )
        {
            View = view;
            MessageBoxAdapter = messageBoxAdapter;
            Account = modelAccount;
            RuleEngine = ruleEngine;
        }

        #endregion Constructors

        #region Properties

        private IQuickPhysicianSelectionView View { get; set; }

        private IMessageBoxAdapter MessageBoxAdapter { get; set; }

        private Account Account { get; set; }

        private IRuleEngine RuleEngine { get; set; }

        private string AdmPhysicianNumber { get; set; }
        private string RefPhysicianNumber { get; set; }

        #endregion Properties

        #region Public Methods

        private void UpdatePhysicianModel( Account account )
        {
            Account = account;
            PopulateReferalPhysician();
            PopulateAdmittingPhysician();
        }

        public void ShowDetails( long physicianNumber, string physicianRelationship )
        {

            if ( physicianNumber == NONSTAFFPHYSICIAN_NBR )
            {
                DisplayNonstaffPhysicianDetails( physicianRelationship );
            }

            else
            {
                var physicianDetailView = new PhysicianDetailView { SelectPhysicians = physicianNumber };

                try
                {
                    physicianDetailView.ShowDialog( (QuickPhysicianSelectionView)View );
                }
                finally
                {
                    physicianDetailView.Dispose();
                }
            }
        }

        private void DisplayNonstaffPhysicianDetails( string physicianRelationship )
        {

            PhysicianSearchFormView physicianSearchForm = CreatePhysicianSearchForm();
            physicianSearchForm.PhysicianRelationshipToView = physicianRelationship;
            physicianSearchForm.physicianSearchTabView1.tcPhysicianSearch.SelectedIndex = (int)SelectedPhysicianTab.RecNonstaffPhysician;
            physicianSearchForm.CallingObject = VIEWDETAIL;
            physicianSearchForm.UpdateView();

            try
            {
                if ( physicianSearchForm.ShowDialog( (QuickPhysicianSelectionView)View ) == DialogResult.OK )
                {
                    Account = physicianSearchForm.Model;
                    UpdateViewDetail();
                }
            }

            finally
            {
                physicianSearchForm.Dispose();
            }
        }

        private PhysicianSearchFormView CreatePhysicianSearchForm()
        {
            var physicianSearchForm = new PhysicianSearchFormView
                                          {
                                              Model = Account,
                                              OperatingPhysicianSelectionVisibility = false,
                                              AttendingPhysicianSelectionVisibility = false,
                                              PrimaryCarePhysicianSelectionVisibility = false
                                          };


            physicianSearchForm.UpdateView();

            return physicianSearchForm;
        }

        public void RemovePhysicianRelationship( PhysicianRelationship physicianRelationship )
        {
            Account.RemovePhysicianRelationship( physicianRelationship );
        }

        public void Find()
        {
            View.ClearSpecifyPhysicianPanel();

            if ( PhysicianSelectionPreRequisitesAreMet() )
            {
                PhysicianSearchFormView physicianSearchForm = CreatePhysicianSearchForm();

                physicianSearchForm.physicianSearchTabView1.tcPhysicianSearch.SelectedIndex = (int)SelectedPhysicianTab.SearchByName;

                try
                {
                    if ( physicianSearchForm.ShowDialog( (QuickPhysicianSelectionView)View ) == DialogResult.OK )
                    {
                        UpdatePhysicianModel( physicianSearchForm.Model );
                    }
                }

                finally
                {
                    physicianSearchForm.Dispose();
                }
            }

            else
            {
                MessageBoxAdapter.ShowMessageBox( UIErrorMessages.QUICK_PHYSICIAN_SELECTION_PREREQS,
                                                 UIErrorMessages.ERROR,
                                                 MessageBoxAdapterButtons.OK, MessageBoxAdapterIcon.Exclamation,
                                                 MessageBoxAdapterDefaultButton.Button1 );

            }
        }

        public void RecordNonStaffPhysician()
        {
            View.ClearSpecifyPhysicianPanel();

            if ( PhysicianSelectionPreRequisitesAreMet() )
            {
                PhysicianSearchFormView physicianSearchForm = CreatePhysicianSearchForm();
                physicianSearchForm.CallingObject = NONSTAFF_CALLINGOBJECT;
                physicianSearchForm.physicianSearchTabView1.tcPhysicianSearch.SelectedIndex =
                    (int)SelectedPhysicianTab.RecNonstaffPhysician;
                try
                {
                    if ( physicianSearchForm.ShowDialog( (QuickPhysicianSelectionView)View ) == DialogResult.OK )
                    {
                        UpdatePhysicianModel( physicianSearchForm.Model );
                    }
                }

                finally
                {
                    physicianSearchForm.Dispose();

                }
            }

            else
            {
                MessageBoxAdapter.ShowMessageBox( UIErrorMessages.QUICK_PHYSICIAN_SELECTION_PREREQS,
                                                 UIErrorMessages.ERROR,
                                                 MessageBoxAdapterButtons.OK, MessageBoxAdapterIcon.Exclamation,
                                                 MessageBoxAdapterDefaultButton.Button1 );
            }
        }

        private bool IsPhysicianNumberValid( string physicianNumber )
        {
            if ( ( physicianNumber == FOURDIGIT_RECORDNONSTAFFPHYSICIAN_NUMBER || physicianNumber == FIVEDIGIT_RECORDNONSTAFFPHYSICIAN_NUMBER ) )
            {
                invalidPhysicianEntry = true;

                MessageBoxAdapter.ShowMessageBox( UIErrorMessages.PHYSICIAN_NUMBER_8888_ERRMSG2,
                                                 UIErrorMessages.ERROR,
                                                 MessageBoxAdapterButtons.OK, MessageBoxAdapterIcon.Exclamation,
                                                 MessageBoxAdapterDefaultButton.Button1 );
                return false;
            }
            invalidPhysicianEntry = false;

            return true;
        }

        public void ValidatePhysicianNumber()
        {
            if ( !IsPhysicianNumberValid( RefPhysicianNumber ) )
            {

                View.SetErrorReferringPhysicianTextBox();
            }
            else
            {
                View.SetNormalReferringPhysicianTextBox();
            }
            if ( !IsPhysicianNumberValid( AdmPhysicianNumber ) )
            {
                View.SetErrorAdmissionPhysicianTextBox();
            }
            else
            {
                View.SetNormalAdmissionPhysicianTextBox();
            }

        }

        public void ValidatePhysicians()
        {
            if ( PhysicianSelectionPreRequisitesAreMet() )
            {

                if ( invalidPhysicianEntry )
                {
                    invalidPhysicianEntry = false;
                    return;
                }

                if ( !IsPhysicianNumberValid( View.RefPhysicianNumber ) )
                    return;

                if ( !IsPhysicianNumberValid( View.AdmPhysicianNumber ) )
                    return;

                VerifyPhysicians();
            }
            else
            {
                MessageBoxAdapter.ShowMessageBox( UIErrorMessages.QUICK_PHYSICIAN_SELECTION_PREREQS,
                                                 UIErrorMessages.ERROR,
                                                 MessageBoxAdapterButtons.OK, MessageBoxAdapterIcon.Exclamation,
                                                 MessageBoxAdapterDefaultButton.Button1 );
            }

        }

        private void VerifyPhysicians()
        {

            if ( View.RefPhysicianNumber != string.Empty )
            {
                try
                {
                    PhysicianService.SearchAndAssignPhysician( Convert.ToInt64( View.RefPhysicianNumber ),
                        PhysicianRole.Referring(),
                        Account );
                    View.SetNormalReferringPhysicianTextBox();
                    View.SetBackColorReferringPhysicianTextBox();
                    View.RefPhysicianNumber = string.Empty;

                    UpdateViewDetail();
                }
                catch ( PhysicianNotFoundException )
                {
                    View.SetErrorReferringPhysicianTextBox();
                }
                catch ( InvalidPhysicianAssignmentException )
                {
                    View.SetErrorReferringPhysicianTextBox();
                }
            }

            if ( View.AdmPhysicianNumber != string.Empty )
            {
                try
                {
                    PhysicianService.SearchAndAssignPhysician( Convert.ToInt64( View.AdmPhysicianNumber ),
                        PhysicianRole.Admitting(),
                        Account );

                    View.SetNormalAdmissionPhysicianTextBox();
                    View.AdmPhysicianNumber = string.Empty;
                    UpdateViewDetail();
                }
                catch ( PhysicianNotFoundException )
                {
                    View.SetErrorAdmissionPhysicianTextBox();
                }
                catch ( InvalidPhysicianAssignmentException )
                {
                    View.SetErrorAdmissionPhysicianTextBox();
                }
            }

            PhysicianService.VerifyPhysicians( Account, View.RefPhysicianNumber, View.AdmPhysicianNumber,
                string.Empty, string.Empty, string.Empty );
        }
        private bool PhysicianSelectionPreRequisitesAreMet()
        {
            return RuleEngine.EvaluateRule( typeof( PhysicianSelectionPreRequisites ), Account );
        }


        private void PopulateReferalPhysician()
        {

            if ( PhysicianIsValid( Account.ReferringPhysician ) )
            {
                View.DisplayReferringPhysician( String.Format( PHYSICIAN_DISPAY_FORMAT, Account.ReferringPhysician.PhysicianNumber,
                                  Account.ReferringPhysician.FormattedName ) );
            }

            View.RunRules();
        }

        private void PopulateAdmittingPhysician()
        {
            if ( PhysicianIsValid( Account.AdmittingPhysician ) )
            {
                View.DisplayAdmittingPhysician(
                    String.Format( PHYSICIAN_DISPAY_FORMAT, Account.AdmittingPhysician.PhysicianNumber,
                                  Account.AdmittingPhysician.FormattedName ) );
            }
            View.RunRules();
        }

        public bool PhysicianIsValid( Physician aPhysician )
        {
            return aPhysician != null && aPhysician.PhysicianNumber != 0;
        }

        public void UpdateViewDetail()
        {
            PopulateReferalPhysician();
            PopulateAdmittingPhysician();
        }

        #endregion Public Methods

        #region Private Methods


        #endregion Private Methods

        #region Data Elements

        #endregion Data Elements

        private enum SelectedPhysicianTab
        {
            SearchByName = 0,
            RecNonstaffPhysician = 2,
        }

        #region Constants
        private const long NONSTAFFPHYSICIAN_NBR = 8888L;
        private const string NONSTAFF_CALLINGOBJECT = "NONSTAFF";
        private const string PHYSICIAN_DISPAY_FORMAT = "{0:00000} {1}";
        private const string VIEWDETAIL = "VIEWDETAIL";
        private const string FOURDIGIT_RECORDNONSTAFFPHYSICIAN_NUMBER = "8888";
        private const string FIVEDIGIT_RECORDNONSTAFFPHYSICIAN_NUMBER = "08888";
        private bool invalidPhysicianEntry;

        #endregion Constants
    }
}
