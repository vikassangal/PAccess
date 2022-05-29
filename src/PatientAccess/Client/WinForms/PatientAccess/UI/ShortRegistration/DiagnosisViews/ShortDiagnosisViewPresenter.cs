using System;
using PatientAccess.Domain;
using PatientAccess.Domain.ShortRegistration;
using PatientAccess.Rules;

namespace PatientAccess.UI.ShortRegistration.DiagnosisViews
{
    public interface IShortDiagnosisViewPresenter
    {
        #region Operations

        void EnableOrDisableProcedureField( string visitTypeCode );

        void HandleProcedureField( VisitType visitType, bool isViewBeingUpdated );

        void UpdateProcedureField( string procedureText );

        #endregion Operations
    }


    /// <summary>
    /// 
    /// </summary>
    public class ShortDiagnosisViewPresenter : IShortDiagnosisViewPresenter
    {

        #region Fields

        private readonly IRuleEngine _ruleEngine = Rules.RuleEngine.GetInstance();

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ShortDiagnosisViewPresenter"/> class.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="activity">The activity.</param>
        public ShortDiagnosisViewPresenter( IShortDiagnosisView view, Activity activity )
        {
            ShortDiagnosisView = view;
            Activity = activity;
            VisitType = new VisitType();
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the activity.
        /// </summary>
        /// <value>The activity.</value>
        private Activity Activity { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [clear procedure field].
        /// </summary>
        /// <value><c>true</c> if [clear procedure field]; otherwise, <c>false</c>.</value>
        private bool ClearProcedureField { get; set; }

        /// <summary>
        /// Gets or sets the diagnosis view.
        /// </summary>
        /// <value>The diagnosis view.</value>
        private IShortDiagnosisView ShortDiagnosisView { get; set; }

        /// <summary>
        /// Gets the rule engine.
        /// </summary>
        /// <value>The rule engine.</value>
        private IRuleEngine RuleEngine
        {
            get { return _ruleEngine; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show enabled procedure field].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [show enabled procedure field]; otherwise, <c>false</c>.
        /// </value>
        private bool ShowEnabledProcedureField { get; set; }

        private VisitType VisitType { get; set; }

        #endregion Properties

        #region Public Methods

        /// <summary>
        /// Clears the procedure from model.
        /// </summary>
        private void ClearProcedureFromModel()
        {
            ShortDiagnosisView.Model.Diagnosis.Procedure = String.Empty;
        }

        /// <summary>
        /// Enables the or disable procedure field.
        /// </summary>
        /// <param name="visitTypeCode">The visit type code.</param>
        public void EnableOrDisableProcedureField( string visitTypeCode )
        {
            if ( IsValidVisitType( visitTypeCode ) )
            {
                ShortDiagnosisView.ShowProcedureEnabled();
                ShortDiagnosisView.PopulateProcedure();
            }
            else
            {
                ShortDiagnosisView.ShowProcedureDisabled();
            }
            RuleEngine.EvaluateRule( typeof( ProcedureRequired ), ShortDiagnosisView.Model );
        }

        /// <summary>
        /// Handles the procedure field.
        /// </summary>
        /// <param name="visitType">Type of the visit.</param>
        /// <param name="isViewBeingUpdated">if set to <c>true</c> [is view being updated].</param>
        public void HandleProcedureField( VisitType visitType, bool isViewBeingUpdated )
        {
            VisitType = visitType;
            ShortDiagnosisView.PopulateProcedure();
            ShowEnabledProcedureField = ShouldWeShowEnabledProcedureField( visitType );
            ClearProcedureField = ShouldWeClearProcedureField( isViewBeingUpdated );

            if ( visitType.IsEmergencyPatient )
            {
                ShortDiagnosisView.ClearProcedureField();
                ClearProcedureFromModel();
            }

            if ( ShouldWeShowEnabledProcedureField( visitType ) )
            {
                ShortDiagnosisView.ShowProcedureEnabled();
            }
            else
            {
                ShortDiagnosisView.ShowProcedureDisabled();
            }

        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Updates the procedure field.
        /// </summary>
        /// <param name="procedureText">The procedure text.</param>
        void IShortDiagnosisViewPresenter.UpdateProcedureField( string procedureText )
        {
            ShortDiagnosisView.Model.Diagnosis.Procedure = procedureText;
            RuleEngine.EvaluateRule( typeof( ProcedureRequired ), ShortDiagnosisView.Model );
        }

        /// <summary>
        /// Determines whether [is maintenance activity].
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if [is maintenance activity]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsMaintenanceActivity()
        {
            return Activity.GetType().Equals( typeof( MaintenanceActivity ) );
        }

        /// <summary>
        /// Determines whether [is pre registration activity].
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if [is pre registration activity]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsPreRegistrationActivity()
        {
            return Activity.GetType().Equals( typeof( PreRegistrationActivity ) );
        }

        /// <summary>
        /// Determines whether [is registration activity].
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if [is registration activity]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsRegistrationActivity()
        {
            return Activity.GetType().Equals( typeof( RegistrationActivity ) );
        }

        /// <summary>
        /// Determines whether [is maintenance activity].
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if [is maintenance activity]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsShortMaintenanceActivity()
        {
            return Activity.GetType().Equals( typeof( ShortMaintenanceActivity ) );
        }

        /// <summary>
        /// Determines whether [is pre registration activity].
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if [is pre registration activity]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsShortPreRegistrationActivity()
        {
            return Activity.GetType().Equals( typeof( ShortPreRegistrationActivity ) );
        }

        /// <summary>
        /// Determines whether [is registration activity].
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if [is registration activity]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsShortRegistrationActivity()
        {
            return Activity.GetType().Equals( typeof( ShortRegistrationActivity ) );
        }

        private bool IsValidActivityForProcedure()
        {
            return IsPreRegistrationActivity() ||
                   IsRegistrationActivity() ||
                   IsMaintenanceActivity() ||
                   IsShortPreRegistrationActivity() ||
                   IsShortRegistrationActivity() ||
                   IsShortMaintenanceActivity();
        }

        /// <summary>
        /// Determines whether [is valid visit type] [the specified visit type code].
        /// </summary>
        /// <param name="visitTypeCode">The visit type code.</param>
        /// <returns>
        /// 	<c>true</c> if [is valid visit type] [the specified visit type code]; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsValidVisitType( string visitTypeCode )
        {
            return visitTypeCode != VisitType.EMERGENCY_PATIENT;
        }

        /// <summary>
        /// Shoulds the we clear procedure field.
        /// </summary>
        /// <param name="isViewBeingUpdated">if set to <c>true</c> [is view being updated].</param>
        /// <returns></returns>
        private static bool ShouldWeClearProcedureField( bool isViewBeingUpdated )
        {
            if (
                !isViewBeingUpdated
               )
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Shoulds the we show enabled procedure field.
        /// </summary>
        /// <param name="visitType">Type of the visit.</param>
        /// <returns></returns>
        private bool ShouldWeShowEnabledProcedureField( VisitType visitType )
        {
            return IsValidActivityForProcedure() && IsValidVisitType( visitType.Code );
        }

        #endregion Private Methods
    }
}