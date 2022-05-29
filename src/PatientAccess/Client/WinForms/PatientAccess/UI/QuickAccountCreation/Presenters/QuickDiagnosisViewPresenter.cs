using PatientAccess.Rules;
using PatientAccess.UI.QuickAccountCreation.Views;

namespace PatientAccess.UI.QuickAccountCreation.Presenters
{
    public interface IQuickDiagnosisViewPresenter
    {
        #region Operations 

        void HandleProcedureField();
        void HandleChiefComplaintField(); 
        void UpdateChiefComplaintField( string procedureText );
        void UpdateProcedureField( string procedureText );

        #endregion Operations
    }


    /// <summary>
    /// 
    /// </summary>
    public class QuickDiagnosisViewPresenter : IQuickDiagnosisViewPresenter
    {

        #region Fields

        private readonly IRuleEngine ruleEngine = Rules.RuleEngine.GetInstance();

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QuickDiagnosisViewPresenter"/> class.
        /// </summary>

        public QuickDiagnosisViewPresenter( IQuickAccountCreationView view )
        {
            QuickAccountCreationView = view;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the diagnosis view.
        /// </summary>
        /// <value>The diagnosis view.</value>
        private IQuickAccountCreationView QuickAccountCreationView { get; set; }

        /// <summary>
        /// Gets the rule engine.
        /// </summary>
        /// <value>The rule engine.</value>
        private IRuleEngine RuleEngine
        {
            get { return ruleEngine; }
        }


        #endregion Properties

        #region Public Methods

        public void HandleProcedureField()
        {
            QuickAccountCreationView.PopulateProcedure();
            RuleEngine.EvaluateRule( typeof( ProcedureRequired ), QuickAccountCreationView.ModelAccount );
        }

        /// <summary>
        /// Handles the Chief complaint field.
        /// </summary>
        public void HandleChiefComplaintField()
        {
            QuickAccountCreationView.PopulateChiefComplaint();
            RuleEngine.EvaluateRule( typeof( ChiefComplaintRequired ), QuickAccountCreationView.ModelAccount );
        }


        /// <summary>
        /// Updates the procedure field.
        /// </summary>
        /// <param name="procedureText">The procedure text.</param>
        void IQuickDiagnosisViewPresenter.UpdateProcedureField( string procedureText )
        {
            QuickAccountCreationView.ModelAccount.Diagnosis.Procedure = procedureText;
            RuleEngine.EvaluateRule( typeof( ProcedureRequired ), QuickAccountCreationView.ModelAccount );
        }
        void IQuickDiagnosisViewPresenter.UpdateChiefComplaintField( string complaintText )
        {
            QuickAccountCreationView.ModelAccount.Diagnosis.ChiefComplaint = complaintText;
            RuleEngine.EvaluateRule( typeof( ChiefComplaintRequired ), QuickAccountCreationView.ModelAccount );
        }
        #endregion Public Methods

        #region Private Methods
        #endregion Private Methods
    }
}