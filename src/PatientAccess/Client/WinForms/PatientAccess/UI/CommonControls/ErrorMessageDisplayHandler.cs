using System;
using System.Collections.Generic;
using System.Windows.Forms;
using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI.HelperClasses;
using PatientAccess.Utilities;

namespace PatientAccess.UI.CommonControls
{
    public class ErrorMessageDisplayHandler : IErrorMessageDisplayHandler
    {
        internal const string CAPTION_WARNING = "Warning";

        #region Delegates

        private delegate DialogResult DisplayAbstractWarningMessage();
        private delegate DialogResult DisplayAbstractErrorMessage();

        #endregion

        #region Public Methods

        public DialogResult DisplayYesNoErrorMessageFor( Type ruleType )
        {
            DisplayAbstractErrorMessage displayMethodPointer = GetDisplayErrorMethodFor( ruleType );
            return displayMethodPointer();
        }

        public void DisplayOkWarningMessageFor(Type ruleType)
        {
            DisplayAbstractWarningMessage displayMethodPointer = GetDisplayWarningMethodFor( ruleType );
            displayMethodPointer();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Retrieves the correct delegate to use based on the Rule Type.
        /// </summary>
        /// <returns></returns>
        private DisplayAbstractErrorMessage GetDisplayErrorMethodFor( Type ruleType )
        {
            VerifyModelAccount();

            return ruleErrorMessageMap[ ruleType ];
        }

        /// <summary>
        /// Retrieves the correct delegate to use based on the Rule Type.
        /// </summary>
        /// <returns></returns>
        private DisplayAbstractWarningMessage GetDisplayWarningMethodFor( Type ruleType )
        {
            VerifyModelAccount();

            return ruleWarningMap[ ruleType ];
        }

        private DialogResult DisplayNoPrimaryMedicareForAutoAccidentYesNoErrorMessage()
        {
            DialogResult warningResult = DialogResult.Yes;

            EventAggregator.RaiseErrorMessageDisplayedEvent(
                this, new LooseArgs( typeof( NoMedicarePrimaryPayorForAutoAccident ).ToString() ) );

            Accident accidentCondition = ( ModelAccount != null ? ModelAccount.Diagnosis.Condition as Accident : null );
            if( IsAutoAccident( accidentCondition ) )
            {
                warningResult = DisplayAutoAccidentErrorMessage();
            }
            else if( IsEmploymentRelatedAccident( accidentCondition ) )
            {
                warningResult = DisplayEmploymentRelatedAccidentErrorMessage();
            }

            return warningResult;
        }

        private DialogResult DisplayAutoAccidentErrorMessage()
        {
            DialogResult warningResult = MessageBoxAdapter.ShowMessageBox(
                                            UIErrorMessages.NO_MEDICARE_FOR_AUTO_ACCIDENT_QUESTION, 
                                            CAPTION_WARNING,
                                            MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
                                            MessageBoxDefaultButton.Button1 );
            return warningResult;
        }

        private DialogResult DisplayEmploymentRelatedAccidentErrorMessage()
        {
            DialogResult warningResult = MessageBoxAdapter.ShowMessageBox(
                                            UIErrorMessages.NO_MEDICARE_FOR_EMPLOYMENT_RELATED_ACCIDENT_QUESTION, 
                                            CAPTION_WARNING,
                                            MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
                                            MessageBoxDefaultButton.Button1 );
            return warningResult;
        }

        private DialogResult DisplayNoPrimaryMedicareForAutoAccidentWarningMessage()
        {
            DialogResult warningResult = DialogResult.None;
            EventAggregator.RaiseErrorMessageDisplayedEvent(
                this, new LooseArgs( typeof( NoMedicarePrimaryPayorForAutoAccident ).ToString() ) );

            Accident accidentCondition = ( ModelAccount != null ? ModelAccount.Diagnosis.Condition as Accident : null );
            if( IsAutoAccident( accidentCondition ) )
            {
                warningResult = DisplayAutoAccidentWarningMessage();
            }
            else if( IsEmploymentRelatedAccident( accidentCondition ) )
            {
                warningResult = DisplayEmploymentRelatedAccidentWarningMessage();
            }

            return warningResult;
        }

        private DialogResult DisplayAutoAccidentWarningMessage()
        {
            DialogResult warningResult = MessageBoxAdapter.ShowMessageBox( 
                                            UIErrorMessages.NO_MEDICARE_FOR_AUTO_ACCIDENT_MSG, 
                                            CAPTION_WARNING,
                                            MessageBoxButtons.OK, MessageBoxIcon.Warning,
                                            MessageBoxDefaultButton.Button1 );
            return warningResult;
        }

        private DialogResult DisplayEmploymentRelatedAccidentWarningMessage()
        {
            DialogResult warningResult = MessageBoxAdapter.ShowMessageBox(
                                            UIErrorMessages.NO_MEDICARE_FOR_EMPLOYMENT_RELATED_ACCIDENT_MSG, 
                                            CAPTION_WARNING,
                                            MessageBoxButtons.OK, MessageBoxIcon.Warning,
                                            MessageBoxDefaultButton.Button1 );
            return warningResult;
        }

        /// <exception cref="InvalidOperationException">Cannot retrieve the delegate for displaying Warning Message when Account is null</exception>
        private void VerifyModelAccount()
        {
            if( ModelAccount == null )
            {
                throw new InvalidOperationException( "Cannot retrieve the delegate for displaying Warning Message when Account is null");
            }
        }

        private static bool IsEmploymentRelatedAccident( Accident accidentCondition )
        {
            return accidentCondition != null &&
                   accidentCondition.Kind.OccurrenceCode.IsEmploymentRelatedOrTortLiabilityOccurrenceCode();
        }

        private static bool IsAutoAccident( Accident accidentCondition )
        {
            return accidentCondition != null &&
                   accidentCondition.Kind.OccurrenceCode.IsAutoAccidentOccurrenceCode();
        }

        /// <summary>
        /// Populate Rule - Error Message (With Yes-No buttons) Map
        /// </summary>
        private void PopulateErrorMessageMap()
        {
            ruleErrorMessageMap.Add( typeof( NoMedicarePrimaryPayorForAutoAccident ),
                DisplayNoPrimaryMedicareForAutoAccidentYesNoErrorMessage );
        }

        /// <summary>
        /// Populate Rule - Warning Message (With OK button) Map
        /// </summary>
        private void PopulateWarningMessageMap()
        {
            ruleWarningMap.Add( typeof( NoMedicarePrimaryPayorForAutoAccident ),
                DisplayNoPrimaryMedicareForAutoAccidentWarningMessage );

            ruleWarningMap.Add( typeof( MedicareCannotBePrimaryPayorForInPatientWithoutPartACoverage ),
                ShowWarningForMedicareCannotBePrimaryPayorForInPatientWithoutPartACoverage );


        }

        private static DialogResult ShowWarningForMedicareCannotBePrimaryPayorForInPatientWithoutPartACoverage()
        {
            return MessageBox.Show( UIErrorMessages.MEDICARE_CANNOT_BE_PRIMARY_PAYOR_FOR_INPATIENT_WITHOUT_PARTA_COVERAGE,
                                    UIErrorMessages.MEDICARE_PART_B,
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Exclamation );
        }

        #endregion

        #region Properties

        private Account ModelAccount { get; set; }

        internal IMessageBoxAdapter MessageBoxAdapter { get; private set; }

        internal IActivityEventAggregator EventAggregator { get; private set; }

        #endregion

        #region Construction and Finalization

        public ErrorMessageDisplayHandler( Account account ): this(account,new MessageBoxAdapter(),ActivityEventAggregator.GetInstance() )
        {
        }

        internal ErrorMessageDisplayHandler( Account account, IMessageBoxAdapter messageBoxAdapter, IActivityEventAggregator eventAggregator )
        {

            Guard.ThrowIfArgumentIsNull(account,"account");
            Guard.ThrowIfArgumentIsNull(messageBoxAdapter, "messageBoxAdapter");
            Guard.ThrowIfArgumentIsNull(eventAggregator, "eventAggregator");

            ModelAccount = account;
            MessageBoxAdapter = messageBoxAdapter;
            EventAggregator = eventAggregator;

            PopulateErrorMessageMap();
            PopulateWarningMessageMap();
        }

        #endregion

        #region Data Elements

        private readonly Dictionary< Type, DisplayAbstractErrorMessage > ruleErrorMessageMap =
            new Dictionary< Type, DisplayAbstractErrorMessage >();

        private readonly Dictionary< Type, DisplayAbstractWarningMessage > ruleWarningMap =
            new Dictionary< Type, DisplayAbstractWarningMessage >();

        #endregion
    }
}
