using System.Collections;
using System.Windows.Forms;
using Extensions.PersistenceCommon;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI.Factories;
using PatientAccess.UI.PAIWalkinAccountCreation.Views;
using PatientAccess.UI.PhysicianSearchViews;
using PatientAccess.UI.PreRegistrationViews;

namespace PatientAccess.UI.PAIWalkinAccountCreation.Presenters
{
    /// <summary>
    /// 
    /// </summary>
    public class PAIWalkinAccountViewPresenter
    {

        #region Fields

        private readonly IRuleEngine ruleEngine = Rules.RuleEngine.GetInstance();

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PAIWalkinAccountViewPresenter"/> class.
        /// </summary>

        public PAIWalkinAccountViewPresenter( IPAIWalkinAccountView view )
        {
            paiWalkinAccountView = view;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the diagnosis view.
        /// </summary>
        /// <value>The diagnosis view.</value>
        private IPAIWalkinAccountView paiWalkinAccountView { get; set; }

        private Account Account
        {
            get { return paiWalkinAccountView.Model_Account; }
        }
        public bool DoNotProceedWithFinish { get; set; }
        /// <summary>
        /// Gets the rule engine.
        /// </summary>
        /// <value>The rule engine.</value>
        private IRuleEngine RuleEngine
        {
            get { return ruleEngine; }
        }
        private readonly ISpanCodeBroker scBroker = new SpanCodeBrokerProxy();
        private ICollection accountProxiesCollection;
        private ShowDuplicatePreRegAccountsDialog showDuplicatePreRegAccountsDialog;

        #endregion Properties

        #region Public Methods

        public void HandleFinish()
        {
            DoNotProceedWithFinish = false;
            ViewFactory.Instance.CreateView<PatientAccessView>().SetFUSNotesOptions( false );

            if ( DoNotProceedWithFinish )
            {
                return;
            }

            if ( DoNotProceedWithFinish )
            {
                paiWalkinAccountView.ReEnableFinishButtonAndFusIcon();
                return;
            }
            //TODO-AC end todo
            paiWalkinAccountView.SetCursorWait();


            paiWalkinAccountView.SetActivatingTab( string.Empty );
            var isPhysicianValid = PhysiciansValidated();

            if ( isPhysicianValid != true )
            {
                paiWalkinAccountView.ReEnableFinishButtonAndFusIcon();
                return;
            }

            if ( !RuleEngine.EvaluateAllRules( Account ) )
            {
                var inValidCodesSummary = RuleEngine.GetInvalidCodeFieldSummary();
                if ( inValidCodesSummary != string.Empty )
                {
                    paiWalkinAccountView.ShowInvalidFieldsDialog( inValidCodesSummary );
                    paiWalkinAccountView.SetCursorDefault();
                    paiWalkinAccountView.SetActiveButtons( true );
                    return;
                }

                var summary = RuleEngine.GetRemainingErrorsSummary();
                paiWalkinAccountView.SetCursorDefault();


                var requiredFieldsDialogShown = paiWalkinAccountView.ShowRequiredFieldsDialogAsNeeded(summary);
                if ( requiredFieldsDialogShown )
                {
                    paiWalkinAccountView.SetCursorDefault();
                    return;
                }
            }
            // BUG 1512 Fix - Verify if SpanCodes 70 and 71 are still valid for
            // the current Patient Type, in case the Patient Type was changed,
            // and 'Finish' clicked, without going to the Billing View.

            var patient = Account.Patient;
            patient.SelectedAccount = Account;


            patient.ClearPriorSystemGeneratedOccurrenceSpans();

            var spanCode70 = scBroker.SpanCodeWith( User.GetCurrent().Facility.Oid, SpanCode.QUALIFYING_STAY_DATES );
            var spanCode71 = scBroker.SpanCodeWith( User.GetCurrent().Facility.Oid, SpanCode.PRIOR_STAY_DATES );

            patient.AddAutoGeneratedSpanCodes70And71With( spanCode70, spanCode71 );


            // BUG 1512 Fix - End

            //Raise Activitycomplete event
            if ( !CheckForError() )
            {
                if ( !CheckForDuplicatePreRegAccounts() )
                {
                    RuleEngine.ClearActions();
                    paiWalkinAccountView.RunRulesForTab();
                    paiWalkinAccountView.ReEnableFinishButtonAndFusIcon();
                    return;
                }
                paiWalkinAccountView.EnableTODO( false );
                paiWalkinAccountView.EnableRefreshTODOList( false );
                paiWalkinAccountView.EnableCancel( false );

                paiWalkinAccountView.SaveAccount();
            }
            else
            {
                paiWalkinAccountView.ReEnableFinishButtonAndFusIcon();
                return;
            }

            RuleEngine.UnloadHandlers();

            // Close Account Supplemental Information View if open for Online PreRegistration Account creation
            ViewFactory.Instance.CreateView<PatientAccessView>().CloseAccountSupplementalInformationView();

            paiWalkinAccountView.SetCursorDefault();
        }
        public AccountProxy GetLatestAccountProxyForSelectedAccount()
        {
            var accountBroker = BrokerFactory.BrokerOfType<IAccountBroker>();

            long accountNumber = Account.AccountNumber;
            long medRecNo = Account.Patient.MedicalRecordNumber;

            var accountProxy = accountBroker.AccountProxyFor( User.GetCurrent().Facility.Code, medRecNo, accountNumber );

            return accountProxy;
        }

        #endregion Public Methods

        #region Private Methods

        private string GetAdmittingPhysicianId()
        {
            var result = string.Empty;
            if ( Account != null && Account.AdmittingPhysician != null )
            {
                result = Account.AdmittingPhysician.PhysicianNumber.ToString();
            }
            return result;
        }

        private string GetReferringPhysicianId()
        {
            var result = string.Empty;

            if ( Account != null && Account.ReferringPhysician != null )
            {
                result = Account.ReferringPhysician.PhysicianNumber.ToString();
            }
            return result;
        }

        private bool PhysiciansValidated()
        {
            return PhysicianService.VerifyPhysicians( Account,
                                                     GetReferringPhysicianId(), GetAdmittingPhysicianId(),
                                                     PersistentModel.NEW_OID.ToString(), PersistentModel.NEW_OID.ToString(),
                                                     PersistentModel.NEW_OID.ToString() );
        }

        private bool CheckForDuplicatePreRegAccounts()
        {
            var result = true;
            if ( Account.KindOfVisit != null &&
                Account.KindOfVisit.Code == VisitType.PREREG_PATIENT )
            {
                var searchCriteria = new DuplicatePreRegAccountsSearchCriteria(
                    Account.Facility.Oid,
                    Account.AccountNumber,
                    Account.Patient.Name,
                    Account.Patient.SocialSecurityNumber,
                    Account.Patient.DateOfBirth,
                    Account.Patient.MedicalRecordNumber,
                    Account.AdmitDate );

                var accountBroker = BrokerFactory.BrokerOfType<IAccountBroker>();

                accountProxiesCollection = accountBroker.SelectDuplicatePreRegAccounts( searchCriteria );

                if ( accountProxiesCollection != null && accountProxiesCollection.Count > 0 )
                {
                    using ( showDuplicatePreRegAccountsDialog = new ShowDuplicatePreRegAccountsDialog() )
                    {
                        showDuplicatePreRegAccountsDialog.Model = accountProxiesCollection;
                        showDuplicatePreRegAccountsDialog.UpdateView();
                        showDuplicatePreRegAccountsDialog.ShowDialog();
                        if ( showDuplicatePreRegAccountsDialog.DialogResult == DialogResult.No )
                        {
                            result = false;
                        }
                    }
                }
            }

            return result;
        }

        private bool CheckForError()
        {
            var rcErrors = RuleEngine.AccountHasFailedError();
            return rcErrors;
        }

        #endregion Private Methods
    }
}