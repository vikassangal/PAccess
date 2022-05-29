using PatientAccess.Domain;
using PatientAccess.Domain.Specialized;

namespace PatientAccess.UI.DiagnosisViews.Specialized
{
    /// <summary>
    /// This DiagnosisView subclass adds a "Clinical Trials" field to the
    /// display. Given that this field is only used sparingly by few facilities,
    /// this subclass was created to avoid cluttering up the main view
    /// with "exceptional" code.
    /// </summary>
    public partial class ClinicalTrialsBoardDiagnosisView : DiagnosisView
    {

		#region Constructors 

        /// <summary>
        /// Initializes a new instance of the <see cref="ClinicalTrialsBoardDiagnosisView"/> class.
        /// </summary>
        internal ClinicalTrialsBoardDiagnosisView()
        {
            this.InitializeComponent();
        }

		#endregion Constructors 

		#region Methods 

        /// <summary>
        /// Re-displays the view when the Model is changed.
        /// </summary>
        public override void UpdateView()
        {
            base.UpdateView();

            YesNoFlag isInClinicalTrial = 
                this.GetTextValueForClinicalTrialLabel();

            if( !IsPreRegistrationActivity( this.Model.Activity ) )
            {
                this.ClinicalTrialsBoardFlagValueLabel.Text = isInClinicalTrial.ToString();
                this.ClinicalTrialsBoardPanel.Visible = true;                
            }

        }


        /// <summary>
        /// Determines whether [is pre registration activity] [the specified activity].
        /// </summary>
        /// <param name="activity">The activity.</param>
        /// <returns>
        /// 	<c>true</c> if [is pre registration activity] [the specified activity]; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsPreRegistrationActivity( Activity activity )
        {
            return ( activity is PreRegistrationActivity ) ||
                   ( activity is PreRegistrationWorklistActivity ) ||
                   ( activity is PreRegistrationWithOfflineActivity );
        }


        /// <summary>
        /// Gets the text value for clinical trial label.
        /// </summary>
        /// <returns></returns>
        private YesNoFlag GetTextValueForClinicalTrialLabel()
        {
            YesNoFlag isInClinicalTrial = new YesNoFlag();
            
            if( this.Model.HasExtendedProperty( ClinicalTrialsConstants.KEY_IS_ACCOUNT_ELIGIBLE_FOR_CLINICAL_TRIALS ) &&
                this.Model[ClinicalTrialsConstants.KEY_IS_ACCOUNT_ELIGIBLE_FOR_CLINICAL_TRIALS].ToString().Equals( YesNoFlag.CODE_YES ) )
            {
                isInClinicalTrial.SetYes();
            }
            else
            {
                isInClinicalTrial.SetNo();
            }

            return isInClinicalTrial;
        }

        #endregion Methods 

    }
}