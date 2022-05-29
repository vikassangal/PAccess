using System;
using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.DemographicsViews
{

    /// <summary>
    /// 
    /// </summary>
    public interface IDemographicsViewPresenter
    {
        #region Operations

        void UpdatePreOpDate( string preOpDateUnmaskedText );
        bool ValidatePreOpDate( bool preOpDateTimePickerFocused, bool isAdmitDateChange, bool isPreOpDateChange, string preOpDateUnmaskedText );
        void HandlePreOpDateDisplayWithDateChange( bool isAdmitDateChange, bool isPreOpDateChange, DateTime admitDate, DateTime preOpDate );
        void HandlePreOpDateDisplayForActivatePreRegistrationInitialViewLoad();
        void PopulatePreOpDate( bool preOpDateTimePickerFocused );

        #endregion Operations
    }

    public class DemographicsViewPresenter : IDemographicsViewPresenter
    {
        #region Fields

        private readonly IRuleEngine _ruleEngine = Rules.RuleEngine.GetInstance();
        public bool preOpDateComplete = true;
        public bool preOpDateIsValid = true;

        #endregion Fields

        #region Constructors

        public DemographicsViewPresenter( IDemographicsView view )
        {
            DemographicsView = view;
        }

        #endregion Constructors

        #region Properties

        private IDemographicsView DemographicsView { get; set; }

        private IRuleEngine RuleEngine
        {
            get { return _ruleEngine; }
        }

        private Account ModelAccount
        {
            get
            {
                return DemographicsView.ModelAccount;
            }
        }

        private string PreOpDateUnmaskedText
        {
            get
            {
                return DemographicsView.GetPreOpDateUnmaskedText();
            }

        }

        #endregion Properties

        #region Public Methods

        /// <summary>
        /// Handles the Pre-Op date field display.
        /// </summary>
        /// <param name="isAdmitDateChange">if set to <c>true</c> [Admit date was changed].</param>
        /// <param name="isPreOpDateChange">if set to <c>true</c> [PreOp date was changed].</param>
        /// <param name="admitDate">Admit date from UI</param>
        /// <param name="preOpDate">PreOp date from UI</param>
        public void HandlePreOpDateDisplayWithDateChange( bool isAdmitDateChange, bool isPreOpDateChange, DateTime admitDate, DateTime preOpDate )
        {
            if( isAdmitDateChange )
            {
                if ( !ModelAccount.ShouldWeEnablePreopDate() )
                {
                    return;
                }
                if ( ModelAccount.AdmitDate.Date == ModelAccount.PreopDate.Date )
                {
                    if ( ( admitDate < preOpDate ) || ( admitDate > preOpDate ) )
                    {
                        DemographicsView.AutoSetPreOpDateWithAdmitDate();
                    }
                }
                if ( ModelAccount.AdmitDate.Date != ModelAccount.PreopDate.Date )
                {
                    if ( ( preOpDate != DateTime.MinValue ) && ( admitDate < preOpDate ) )
                    {
                        ClearPreOpDateAndSetFocus();
                    }
                    if ( admitDate > preOpDate || admitDate == preOpDate )
                    {
                        return;
                    }
                }
            }
            else if( isPreOpDateChange )
            {
                if ( preOpDate < admitDate || preOpDate == admitDate )
                {
                    return;
                }
                if ( ( preOpDate != DateTime.MinValue ) && ( preOpDate > admitDate ) )
                {
                    HandlePreOpDateAfterAdmitDate();
                }
            }
            else
            {
                HandlePreOpDateDisplayForUpdateView();
            }
        }

        public void HandlePreOpDateDisplayForUpdateView()
        {
            // Do not handle PreOpDate any further for Activate 
            // PreRegistration Activity during initial load of account.
            if ( DemographicsView.LoadingModelData &&
                 ( ModelAccount.Activity != null &&
                   ModelAccount.Activity.GetType() == typeof( RegistrationActivity ) &&
                   ModelAccount.Activity.AssociatedActivityType == typeof( ActivatePreRegistrationActivity ) ) )
            {
                return;
            }

            try
            {
                if( ModelAccount.PreopDate == DateTime.MinValue &&
                    ModelAccount.ShouldWeEnablePreopDate() )
                {
                    ModelAccount.PreopDate = ModelAccount.AdmitDate;
                }

                if( ModelAccount.PreopDate == DateTime.MinValue ||
                    ModelAccount.PreopDate.Date == DateTime.MinValue.Date )
                {
                    DemographicsView.SetBlankPreOpDate();
                }
                else
                {
                    DemographicsView.SetPreopDateFromModel();
                }
            }
            catch ( FormatException )
            {
                DemographicsView.SetBlankPreOpDate();
            }
        }

        public void HandlePreOpDateDisplayForActivatePreRegistrationInitialViewLoad()
        {
            if ( ModelAccount.Activity != null &&
                 ModelAccount.Activity.GetType() == typeof( RegistrationActivity ) &&
                 ModelAccount.Activity.AssociatedActivityType != null &&
                 ModelAccount.Activity.AssociatedActivityType != typeof( ActivatePreRegistrationActivity ) )
            {
                return;
            }

            try
            {
                DateTime preRegPreOpDate = ModelAccount.PreopDate;
                DateTime preRegAdmitDate = DemographicsView.PreRegAdmitDate;

                if ( ModelAccount.PreopDate == DateTime.MinValue &&
                     ModelAccount.ShouldWeEnablePreopDate() )
                {
                    ModelAccount.PreopDate = ModelAccount.AdmitDate;
                }

                if ( ModelAccount.PreopDate == DateTime.MinValue ||
                     ModelAccount.PreopDate.Date == DateTime.MinValue.Date )
                {
                    DemographicsView.SetBlankPreOpDate();
                }

                if ( preRegAdmitDate.Date == preRegPreOpDate.Date )
                {
                    DemographicsView.AutoSetPreOpDateWithAdmitDate();
                }

                if ( preRegAdmitDate.Date != preRegPreOpDate.Date )
                {
                    if ( ( preRegPreOpDate != DateTime.MinValue ) && 
                         ( ModelAccount.AdmitDate.Date < preRegPreOpDate.Date ) )
                    {
                        ClearPreOpDateAndSetFocus();
                    }
                    if ( ModelAccount.AdmitDate.Date > preRegPreOpDate.Date )
                    {
                        DemographicsView.SetPreopDateFromModel();
                    }
                }
            }
            catch ( FormatException )
            {
                DemographicsView.SetBlankPreOpDate();
            }
        }

        private void ClearPreOpDateAndSetFocus()
        {
            DemographicsView.SetBlankPreOpDate();
            DemographicsView.SetPreOpDateFocus();
        }

        void HandleInvalidPreOpDate()
        {
            DemographicsView.SetPreOpDateErrBgColor();
            DemographicsView.ShowPreOpDateInvalidErrorMessage();
            DemographicsView.SetPreOpDateFocus();
        }

        void HandleIncompletePreOpDate()
        {
            DemographicsView.SetPreOpDateErrBgColor();
            DemographicsView.ShowPreOpDateIncompleteErrorMessage();
            DemographicsView.SetPreOpDateFocus();
        }

        void HandlePreOpDateAfterAdmitDate()
        {
            DemographicsView.ShowPreOpDateAfterAdmitDateErrorMessage();
            DemographicsView.SetBlankPreOpDate();
            DemographicsView.SetPreOpDateFocus();
        }

        public void PopulatePreOpDate( bool preOpDateTimePickerFocused )
        {
            DemographicsView.EnablePreOpDate( ModelAccount.ShouldWeEnablePreopDate() );
            HandlePreOpDateDisplayWithDateChange( false, false, ModelAccount.AdmitDate, ModelAccount.PreopDate );
            ValidatePreOpDate( preOpDateTimePickerFocused, false, false, PreOpDateUnmaskedText );
        }

        public void UpdatePreOpDate( string preOpDateUnmaskedText )
        {
            if ( ModelAccount != null )
            {
                if ( preOpDateUnmaskedText.Trim() == String.Empty )
                {
                    ModelAccount.PreopDate = DateTime.MinValue;
                }
                else if ( preOpDateUnmaskedText.Length == 8 )
                {
                    DateTime theDate = DateTime.MinValue;
                    try
                    {
                        theDate = DemographicsView.GetDateAndTimeFrom( preOpDateUnmaskedText, string.Empty );
                    }
                    catch
                    {
                        HandleInvalidPreOpDate();
                    }
                    ModelAccount.PreopDate = theDate;
                }

                DemographicsView.RegisterPreOpDateEvent();
                bool ruleDidNotFire = RuleEngine.EvaluateRule( typeof( PreopDateRequired ), ModelAccount ) && RuleEngine.EvaluateRule( typeof( PreopDatePreferred ), ModelAccount ); 
                if ( ruleDidNotFire )
                {
                    DemographicsView.SetPreOpDateNormalBgColor();
                }
            }
        }

        public bool VerifyPreOpDateIsComplete( string preOpDateUnmaskedText )
        {
            if ( preOpDateUnmaskedText.Trim() == string.Empty
                 || preOpDateUnmaskedText.Trim() == "01010001" )
            {
                preOpDateComplete = false;
                DemographicsView.SetBlankPreOpDate();
                DemographicsView.SetPreOpDateNormalBgColor();
                return true;
            }

            if ( preOpDateUnmaskedText != string.Empty
                 && preOpDateUnmaskedText.Trim().Length != 0
                 && preOpDateUnmaskedText.Length != 8 )
            {
                HandleIncompletePreOpDate();
                preOpDateComplete = false;
            }

            return preOpDateComplete;
        }

        public DateTime GetPreOpDateFromUI()
        {
            DateTime preOpDate = DateTime.MinValue;
            try
            {
                preOpDate = DemographicsView.GetPreopDateFromUI();
            }
            catch
            {
                preOpDateIsValid = false;
                HandleInvalidPreOpDate();
            }

            return preOpDate;
        }

        public bool VerifyPreOpDateIsValidForDisplay( DateTime preOpDate )
        {
            if ( preOpDateIsValid &&
                 ( preOpDate == DateTime.MinValue || DateValidator.IsValidDate( preOpDate ) == false ) )
            {
                HandleInvalidPreOpDate();
                preOpDateIsValid = false;
            }

            return preOpDateIsValid;
        }

        public bool ValidatePreOpDate(bool preOpDateTimePickerFocused, bool isAdmitDateChange, bool isPreOpDateChange, string preOpDateUnmaskedText)
        {
            preOpDateComplete = true;
            preOpDateIsValid = true;

            if ( preOpDateTimePickerFocused )
            {
                return false;
            }

            if ( !VerifyPreOpDateIsComplete( preOpDateUnmaskedText ) )
            {
                return false;
            }

            if ( preOpDateComplete )
            {
                DateTime preOpDate = GetPreOpDateFromUI();

                if ( !VerifyPreOpDateIsValidForDisplay( preOpDate ) )  
                {
                    return false;
                }

                if ( preOpDateIsValid )
                {
                    HandlePreOpDateDisplayWithDateChange( isAdmitDateChange, isPreOpDateChange, DemographicsView.GetAdmitDateFromUI(), preOpDate );
                }
            }

            UpdatePreOpDate( PreOpDateUnmaskedText );

            return true;
        }

        #endregion Public Methods
    }
}