using System;
using PatientAccess.Domain;
using PatientAccess.Rules;

namespace PatientAccess.UI.DiagnosisViews
{

    /// <summary>
    /// 
    /// </summary>
    public interface IDiagnosisViewPresenter
    {
		#region Operations 

        void EnableOrDisableProcedureField( string visitTypeCode);

        void HandleProcedureField(VisitType visitType, bool isViewBeingUpdated);

        void UpdateProcedureField(string procedureText);

        #endregion Operations 
    }


    /// <summary>
    /// 
    /// </summary>
    public class DiagnosisViewPresenter : IDiagnosisViewPresenter
    {

		#region Fields 

        private readonly IRuleEngine _ruleEngine = Rules.RuleEngine.GetInstance();

		#endregion Fields 

		#region Constructors 

        /// <summary>
        /// Initializes a new instance of the <see cref="DiagnosisViewPresenter"/> class.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="activity">The activity.</param>
        public DiagnosisViewPresenter(IDiagnosisView view, Activity activity)
        {
            DiagnosisView = view;
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
        private IDiagnosisView DiagnosisView { get; set; }

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
            this.DiagnosisView.Model.Diagnosis.Procedure = String.Empty;
        }

        /// <summary>
        /// Enables the or disable procedure field.
        /// </summary>
        /// <param name="visitTypeCode">The visit type code.</param>
        public void EnableOrDisableProcedureField(string visitTypeCode)
        {
            if (IsValidVisitType(visitTypeCode))
            {
                this.DiagnosisView.ShowProcedureEnabled();
                this.DiagnosisView.PopulateProcedure();
            }
            else
            {
                this.DiagnosisView.ShowProcedureDisabled();
            }
            this.RuleEngine.EvaluateRule(typeof(ProcedureRequired), this.DiagnosisView.Model);
        }

        /// <summary>
        /// Handles the procedure field.
        /// </summary>
        /// <param name="visitType">Type of the visit.</param>
        /// <param name="isViewBeingUpdated">if set to <c>true</c> [is view being updated].</param>
        public void HandleProcedureField(VisitType visitType, bool isViewBeingUpdated)
        {
            this.VisitType = visitType;
            this.DiagnosisView.PopulateProcedure();
            this.ShowEnabledProcedureField = this.ShouldWeShowEnabledProcedureField(visitType);
            this.ClearProcedureField = ShouldWeClearProcedureField(isViewBeingUpdated);

            if( visitType.IsEmergencyPatient )
            {
                this.DiagnosisView.ClearProcedureField();
                this.ClearProcedureFromModel();
            }

            if( ShouldWeShowEnabledProcedureField(visitType) )
            {
                this.DiagnosisView.ShowProcedureEnabled();
            }
            else
            {
                this.DiagnosisView.ShowProcedureDisabled();
            }

        }

		#endregion Public Methods 

		#region Private Methods 

        /// <summary>
        /// Updates the procedure field.
        /// </summary>
        /// <param name="procedureText">The procedure text.</param>
        void IDiagnosisViewPresenter.UpdateProcedureField(string procedureText)
        {
            this.DiagnosisView.Model.Diagnosis.Procedure = procedureText;
            RuleEngine.EvaluateRule(typeof(ProcedureRequired), this.DiagnosisView.Model);
        }

        /// <summary>
        /// Determines whether [is maintenance activity].
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if [is maintenance activity]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsMaintenanceActivity()
        {
            return Activity.GetType().Equals(typeof(MaintenanceActivity));
        }

        /// <summary>
        /// Determines whether [is pre registration activity].
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if [is pre registration activity]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsPreRegistrationActivity()
        {
            return Activity.GetType().Equals(typeof(PreRegistrationActivity));
        }

        /// <summary>
        /// Determines whether [is registration activity].
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if [is registration activity]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsRegistrationActivity()
        {
            return this.Activity.GetType().Equals(typeof(RegistrationActivity));
        }

        private bool IsValidActivityForProcedure()
        {
            return this.IsPreRegistrationActivity() ||
                   this.IsRegistrationActivity() ||
                   (this.IsMaintenanceActivity() 
                        && (Activity.AssociatedActivityType==null || Activity.AssociatedActivityType!=typeof(PreAdmitNewbornActivity)));
        }

        /// <summary>
        /// Determines whether [is valid visit type] [the specified visit type code].
        /// </summary>
        /// <param name="visitTypeCode">The visit type code.</param>
        /// <returns>
        /// 	<c>true</c> if [is valid visit type] [the specified visit type code]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsValidVisitType(string visitTypeCode)
        {
            return visitTypeCode != VisitType.EMERGENCY_PATIENT;
        }

        /// <summary>
        /// Shoulds the we clear procedure field.
        /// </summary>
        /// <param name="isViewBeingUpdated">if set to <c>true</c> [is view being updated].</param>
        /// <returns></returns>
        private static bool ShouldWeClearProcedureField(bool isViewBeingUpdated)
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
        private bool ShouldWeShowEnabledProcedureField(VisitType visitType)
        {
            return IsValidActivityForProcedure() && IsValidVisitType(visitType.Code);
        }

		#endregion Private Methods 
    
        #region IDiagnosisViewPresenter Members


        public static string GetAccidentHourString( TimeAndLocationBoundCondition condition )
        {
            var returnValue = condition.OccurredAtHour;

            //we get time from PBAR in military format (e.g. '700' and '1100')
            //we get '9900' or '2400' for 'Unknown' 
            if ( condition.OccurredAtHour == "9900" || condition.OccurredAtHour == "2400" )
            {
                returnValue = "Unknown";
            }

            else switch ( returnValue.Length )
            {
                case 3:
                    returnValue = "0" + returnValue.Substring( 0, 1 );
                    break;
                
                case 4:
                    returnValue = returnValue.Substring( 0, 2 );
                    break;
                
                case 1:
                    returnValue = "0" + returnValue;
                    break;
            }

            return returnValue;
        }

        #endregion
    }
}