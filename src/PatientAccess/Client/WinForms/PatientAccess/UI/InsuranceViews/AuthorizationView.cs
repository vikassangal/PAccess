using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Rules;
using PatientAccess.UI.Factories;
using PatientAccess.UI.HelperClasses;
using PatientAccess.UI.InsuranceViews.InsuranceVerificationViews;

namespace PatientAccess.UI.InsuranceViews
{
    public partial class AuthorizationView : ControlView
    {
        #region Event Handlers

        private void AuthorizationView_Leave( object sender, EventArgs e )
        {
            if ( i_AuthorizationViewService.IsCoverageValidForAuthorization() )
            {
                if ( InsuranceDateVerify.IsValidDateTime( mtbEffectiveDate ) )
                {
                    CoverageAuthorization.EffectiveDate = Convert.ToDateTime(mtbEffectiveDate.Text.Trim());
                    isValidEffDate = true;
                }
                else
                {
                    isValidEffDate = false;
                    CoverageAuthorization.EffectiveDate = DateTime.MinValue;
                }
                if ( InsuranceDateVerify.IsValidDateTime( mtbExpirationDate ) )
                {
                    isValidExpDate = true;
                    CoverageAuthorization.ExpirationDate = Convert.ToDateTime(mtbExpirationDate.Text.Trim());
                }
                else
                {
                    isValidExpDate = false;
                    CoverageAuthorization.ExpirationDate = DateTime.MinValue;
                }

                DialogResult result;
                if (CoverageAuthorization.EffectiveDate != DateTime.MinValue)
                {
                    if (CoverageAuthorization.ExpirationDate.Equals(DateTime.MinValue))
                    {
                        result = MessageBox.Show( UIErrorMessages.AUTHORIZATION_EXPIRATION_DATE_REQUIRED, "Warning",
                              MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1 );
                        if ( result == DialogResult.No )
                        {
                            mtbExpirationDate.Focus();
                        }

                    }

                }
                else if (CoverageAuthorization.ExpirationDate != DateTime.MinValue)
                {
                    if (CoverageAuthorization.EffectiveDate.Equals(DateTime.MinValue))
                    {
                        result = MessageBox.Show( UIErrorMessages.AUTHORIZATION_EEFECTIVE_DATE_REQUIRED, "Warning",
                             MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1 );
                        if ( result == DialogResult.No )
                        {
                            mtbEffectiveDate.Focus();
                        }

                    }
                }
            }
            if ( isValidEffDate && isValidExpDate )
            {
                UpdateModel();

            }


        }
      
        private void tbTrackingNumber_Validating( object sender, CancelEventArgs e )
        {
            if ( Model.GetType().Equals( typeof( CoverageForCommercialOther ) ) )
            {
                ( (CoverageForCommercialOther)Model ).TrackingNumber = tbTrackingNumber.Text;
            }

            if (Model.GetType().Equals(typeof(GovernmentMedicaidCoverage)))
            {
                ((GovernmentMedicaidCoverage) Model).TrackingNumber = tbTrackingNumber.Text;
            }

            if (Model.GetType().Equals(typeof(GovernmentMedicareCoverage)))
            {
                var medicareCoverage = (GovernmentMedicareCoverage) Model;
                if (medicareCoverage != null && medicareCoverage.IsMedicareCoverageValidForAuthorization )
                {
                    ((GovernmentMedicareCoverage) Model).TrackingNumber = tbTrackingNumber.Text;
                }
            }
        }

        private void dtpEffectiveDate_CloseUp( object sender, EventArgs e )
        {
            UIColors.SetNormalBgColor( dtpEffectiveDate );
            Refresh();

            DateTime dt = dtpEffectiveDate.Value;
            mtbEffectiveDate.UnMaskedText = String.Format( "{0:D2}{1:D2}{2:D4}", dt.Month, dt.Day, dt.Year );

            mtbEffectiveDate.Focus();
        }

        private void dtpExpirationDate_CloseUp( object sender, EventArgs e )
        {
            UIColors.SetNormalBgColor( dtpExpirationDate );
            Refresh();

            DateTime dt = dtpExpirationDate.Value;
            mtbExpirationDate.UnMaskedText = String.Format( "{0:D2}{1:D2}{2:D4}", dt.Month, dt.Day, dt.Year );

            mtbExpirationDate.Focus();
        }

        private void mtbEffectiveDate_Validating( object sender, CancelEventArgs e )
        {
            UIColors.SetNormalBgColor( mtbEffectiveDate );

            MaskedEditTextBox mtb = sender as MaskedEditTextBox;

            bool errorFlag = false;
            if ( dtpEffectiveDate.Focused )
            {
                return;
            }
            if ( mtb.UnMaskedText.Trim() != String.Empty )
            {
                month = 0;
                day = 0;
                year = 0;
                if ( InsuranceDateVerify.VerifyInsuranceDate( ref mtb, ref year, ref month, ref day ) )
                {
                    UIColors.SetNormalBgColor( mtb );
                    Refresh();

                    DateTime effectiveDate = new DateTime( year, month, day ).Date;
                    CoverageAuthorization.EffectiveDate = effectiveDate;


                    DialogResult result;
                    if ( mtbExpirationDate.UnMaskedText.Trim() != String.Empty )
                    {
                        DateTime expirationDate = Convert.ToDateTime( mtbExpirationDate.Text.Trim() ).Date;

                        if ( effectiveDateWarning && effectiveDate > expirationDate )
                        {
                            mtb.Focus();
                            UIColors.SetErrorBgColor( mtb );
                            MessageBox.Show( UIErrorMessages.AUTHORIZATION_EFFECTIVE_DATE_PRECEDES_EXPIRATION_DATE, "Error",
                                 MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1 );
                            effectiveDateWarning = false;
                            errorFlag = true;
                        }
                        else if ( effectiveAdmitDateWarning && ( Account.AdmitDate.Date < effectiveDate ) )
                        {
                            result = MessageBox.Show( UIErrorMessages.AUTHORIZATION_ADMIT_DATE_OUT_OF_RANGE, "Warning",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1 );
                            effectiveAdmitDateWarning = false;
                            errorFlag = true;
                            if ( result == DialogResult.No )
                            {
                                mtb.Focus();
                            }
                        }
                    }
                    if ( !errorFlag && effectiveGreaterThanAdmitDateWarning && Account.AdmitDate.Date < effectiveDate && mtbExpirationDate.UnMaskedText.Trim() == String.Empty )
                    {
                        result = MessageBox.Show( UIErrorMessages.AUTHORIZATION_EFFECTIVE_DATE_GREATER_THAN_ADMIT_DATE, "Warning",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1 );
                        effectiveGreaterThanAdmitDateWarning = false;
                        if ( result == DialogResult.No )
                        {
                            mtb.Focus();
                        }
                    }

                }

            }
            else
            {
                CoverageAuthorization.EffectiveDate = DateTime.MinValue;
            }


        }

        private void mtbExpirationDate_Validating( object sender, CancelEventArgs e )
        {

            UIColors.SetNormalBgColor( mtbExpirationDate );
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            bool errorFlag = false;
            if ( dtpExpirationDate.Focused )
            {
                return;
            }
            if ( mtb.UnMaskedText.Trim() != String.Empty )
            {
                month = 0;
                day = 0;
                year = 0;
                if ( InsuranceDateVerify.VerifyInsuranceDate( ref mtb, ref year, ref month, ref day ) )
                {
                    UIColors.SetNormalBgColor( mtb );
                    Refresh();

                    DateTime expirationDate = new DateTime( year, month, day ).Date;
                    CoverageAuthorization.ExpirationDate = expirationDate;

                    DialogResult result;
                    if ( mtbEffectiveDate.UnMaskedText.Trim() != String.Empty )
                    {
                        DateTime effectiveDate = Convert.ToDateTime( mtbEffectiveDate.Text.Trim() ).Date;

                        if ( expirationDateWarning && effectiveDate > expirationDate )
                        {
                            UIColors.SetErrorBgColor( mtb );
                            mtb.Focus();
                            MessageBox.Show( UIErrorMessages.AUTHORIZATION_EFFECTIVE_DATE_PRECEDES_EXPIRATION_DATE, "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1 );
                            expirationDateWarning = false;
                            errorFlag = true;
                        }
                        else if ( expirationAdmitDateWarning && ( Account.AdmitDate.Date < effectiveDate ) )
                        {
                            result = MessageBox.Show( UIErrorMessages.AUTHORIZATION_ADMIT_DATE_OUT_OF_RANGE, "Warning",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1 );
                            expirationAdmitDateWarning = false;
                            errorFlag = true;

                            if ( result == DialogResult.No )
                            {
                                mtb.Focus();
                            }
                        }
                    }
                    if ( !errorFlag && expirationLesserThanAdmitDateWarning && ( Account.AdmitDate.Date > expirationDate && mtbEffectiveDate.UnMaskedText.Trim() == String.Empty ) )
                    {
                        result = MessageBox.Show( UIErrorMessages.AUTHORIZATION_EXPIRATION_DATE_LESSER_THAN_ADMIT_DATE, "Warning",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1 );
                        expirationLesserThanAdmitDateWarning = false;
                        if ( result == DialogResult.No )
                        {
                            mtb.Focus();
                        }
                    }
                }

            }
            else
            {
                 CoverageAuthorization.ExpirationDate = DateTime.MinValue;
            }


        }

        private void CompanyRepFirstNamePreferredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetPreferredBgColor( tbCompanyRepFirstName );
        }

        private void CompanyRepLastNamePreferredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetPreferredBgColor( tbCompanyRepLastName );
        }

        private void ServicesAuthorizedPreferredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetPreferredBgColor( tbServicesAuthorized );
        }

        private void AuthorizationNumberPreferredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetPreferredBgColor( mtbAuthorizationNumber );
        }


        private void mtbAuthorizationNumber_Validating( object sender, CancelEventArgs e )
        {
             CoverageAuthorization.AuthorizationNumber = mtbAuthorizationNumber.UnMaskedText.Trim();
            CheckForRequiredFields();
        }

        private void tbCompanyRepFirstName_TextChanged( object sender, EventArgs e )
        {
            TextBox tb = sender as TextBox;

            if ( tb.Text.Trim().Length == 0 )
            {
                CheckForRequiredFields();
            }
            else
            {
                UIColors.SetNormalBgColor( tb );
            }

        }

        private void tbCompanyRepLastName_TextChanged( object sender, EventArgs e )
        {
            TextBox tb = sender as TextBox;

            if ( tb.Text.Trim().Length == 0 )
            {
                CheckForRequiredFields();
            }
            else
            {
                UIColors.SetNormalBgColor( tb );
            }
        }

        private void tbServicesAuthorized_TextChanged( object sender, EventArgs e )
        {
            TextBox tb = sender as TextBox;

            if ( tb.Text.Trim().Length == 0 )
            {
                CheckForRequiredFields();
            }
            else
            {
                UIColors.SetNormalBgColor( tb );
            }
        }

        private void mtbAuthorizationNumber_TextChanged( object sender, EventArgs e )
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;

            if ( mtb.Text.Trim().Length == 0 )
            {
                CheckForRequiredFields();
            }
            else
            {
                UIColors.SetNormalBgColor( mtb );
            }
        }

        private void tbCompanyRepFirstName_Validating( object sender, CancelEventArgs e )
        {
            CoverageAuthorization.NameOfCompanyRepresentative =
                new Name( tbCompanyRepFirstName.Text.Trim(), tbCompanyRepLastName.Text.Trim(), string.Empty );
            CheckForRequiredFields();
        }

        private void tbCompanyRepLastName_Validating( object sender, CancelEventArgs e )
        {
             CoverageAuthorization.NameOfCompanyRepresentative =
                new Name( tbCompanyRepFirstName.Text.Trim(), tbCompanyRepLastName.Text.Trim(), string.Empty );
            CheckForRequiredFields();
        }

        private void tbServicesAuthorized_Validating( object sender, CancelEventArgs e )
        {
             CoverageAuthorization.ServicesAuthorized =
                tbServicesAuthorized.Text.Trim();
            CheckForRequiredFields();
        }

        #endregion

        #region Methods
        public override void UpdateView()
        {
            if ( Model != null )
            {
                InitializeAuthorizationStatus();
                i_AuthorizationViewService.Coverage = Model;
                if ( !i_AuthorizationViewService.IsCoverageValidForAuthorization() )
                {
                    panelAuthorization.Visible = false;
                    insuranceVerificationSummary.Visible = false;
                    lblAuthorizationUnAvailable.Visible = true;
                    lblAuthorizationUnAvailable.Enabled = true;
                    return;
                }
                if ( i_AuthorizationViewService.EnableTrackingNumber() )
                {
                    tbTrackingNumber.Enabled = true;
                    tbTrackingNumber.Text = TrackingNumberText;
                }
                else
                {
                    tbTrackingNumber.Enabled = false;
                    mtbAuthorizationNumber.Focus();
                }
                insuranceVerificationSummary.Model = Model;
                insuranceVerificationSummary.Account = Account;
                insuranceVerificationSummary.UpdateView();

                Authorization authorization = i_AuthorizationViewService.GetAuthorization();
                if ( authorization != null )
                {
                    mtbAuthorizationNumber.Text = authorization.AuthorizationNumber;
                    if ( authorization.NameOfCompanyRepresentative != null )
                    {
                        tbCompanyRepFirstName.Text = authorization.NameOfCompanyRepresentative.FirstName;
                        tbCompanyRepLastName.Text = authorization.NameOfCompanyRepresentative.LastName;
                    }
                    if ( authorization.NumberOfDaysAuthorized != 0 )
                    {
                        tbDaysAuthorized.Text = authorization.NumberOfDaysAuthorized.ToString();
                    }

                    tbServicesAuthorized.Text = authorization.ServicesAuthorized;
                    if ( authorization.EffectiveDate != DateTime.MinValue )
                    {
                        mtbEffectiveDate.UnMaskedText = String.Format( "{0:D2}{1:D2}{2:D4}",
                                    authorization.EffectiveDate.Month,
                                    authorization.EffectiveDate.Day,
                                    authorization.EffectiveDate.Year );
                    }
                    else
                    {
                        mtbEffectiveDate.UnMaskedText = string.Empty;
                    }

                    if ( authorization.ExpirationDate != DateTime.MinValue )
                    {
                        mtbExpirationDate.UnMaskedText = String.Format( "{0:D2}{1:D2}{2:D4}",
                                    authorization.ExpirationDate.Month,
                                    authorization.ExpirationDate.Day,
                                    authorization.ExpirationDate.Year );
                    }
                    else
                    {
                        mtbExpirationDate.UnMaskedText = string.Empty;
                    }

                    mtbRemarks.Text = authorization.Remarks;
                    if ( authorization.AuthorizationStatus != null && authorization.AuthorizationStatus.Oid > 0 )
                    {
                        cmbStatus.SelectedItem = authorization.AuthorizationStatus;

                    }

                }

                RegisterEvents();
                CheckForRequiredFields();
            }
        }


        private string TrackingNumberText
        {
            get
            {
                if (Model is CoverageForCommercialOther)
                {
                    return ((CoverageForCommercialOther) Model).TrackingNumber;
                }
                else if (Model is GovernmentMedicaidCoverage)
                {
                    return ((GovernmentMedicaidCoverage) Model).TrackingNumber;
                }
                else if (Model is GovernmentMedicareCoverage)
                {
                    var medicareCoverage = (GovernmentMedicareCoverage) Model;
                    if (medicareCoverage != null && medicareCoverage.IsMedicareCoverageValidForAuthorization )
                    {
                        return ((GovernmentMedicareCoverage) Model).TrackingNumber;
                    }
                }

                return string.Empty;
            }
            set
            {
                if (Model is CoverageForCommercialOther)
                {
                    ((CoverageForCommercialOther) Model).TrackingNumber = tbTrackingNumber.Text;
                }
                else if (Model is GovernmentMedicaidCoverage)
                {
                    ((GovernmentMedicaidCoverage) Model).TrackingNumber = tbTrackingNumber.Text;
                }
                else if (Model is GovernmentMedicareCoverage)
                {
                    var medicareCoverage = (GovernmentMedicareCoverage) Model;
                    if (medicareCoverage != null && medicareCoverage.IsMedicareCoverageValidForAuthorization )
                    {
                        ((GovernmentMedicareCoverage) Model).TrackingNumber = tbTrackingNumber.Text;
                    }
                }
            }
        }


        public override void UpdateModel()
        {
            if ( Model == null )
            {
                return;
            }

            i_AuthorizationViewService.Coverage = Model;
            if ( i_AuthorizationViewService.IsCoverageValidForAuthorization() )
            {
                if (i_AuthorizationViewService.EnableTrackingNumber())
                {
                    TrackingNumberText = tbTrackingNumber.Text;
                }

                Authorization authorization = i_AuthorizationViewService.GetAuthorization();
                authorization.AuthorizationNumber = mtbAuthorizationNumber.Text.Trim();
                authorization.NameOfCompanyRepresentative = new Name( tbCompanyRepFirstName.Text.Trim(), tbCompanyRepLastName.Text.Trim(), string.Empty );
                if ( tbDaysAuthorized.Text.Trim().Length > 0 )
                {
                    authorization.NumberOfDaysAuthorized = Convert.ToInt32( tbDaysAuthorized.Text.Trim() );
                }
                else
                {
                    authorization.NumberOfDaysAuthorized = 0;
                }
                authorization.ServicesAuthorized = tbServicesAuthorized.Text.Trim();

                if ( mtbEffectiveDate.UnMaskedText.Trim() != String.Empty )
                {
                    authorization.EffectiveDate = Convert.ToDateTime( mtbEffectiveDate.Text.Trim() );
                }
                else
                {
                    authorization.EffectiveDate = DateTime.MinValue;
                }
                if ( mtbExpirationDate.UnMaskedText.Trim() != String.Empty )
                {
                    authorization.ExpirationDate = Convert.ToDateTime( mtbExpirationDate.Text.Trim() );
                }
                else
                {
                    authorization.ExpirationDate = DateTime.MinValue;
                }

                if ( cmbStatus.SelectedItem != null && cmbStatus.Enabled )
                {
                    authorization.AuthorizationStatus = cmbStatus.SelectedItem as AuthorizationStatus;
                }

                authorization.Remarks = mtbRemarks.Text.Trim();

                CoverageAuthorization = authorization;
            }
        }

        #endregion

        #region Properties
        public new Coverage Model
        {
            set
            {
                base.Model = value;
            }
            private get
            {
                return base.Model as Coverage;
            }
        }

        public Account Account
        {
            private get
            {
                return i_Account;
            }
            set
            {
                i_Account = value;
            }
        }


        [Browsable( false )]
        private RuleEngine RuleEngine
        {
            get
            {
                if ( i_RuleEngine == null )
                {
                    i_RuleEngine = RuleEngine.GetInstance();
                }
                return i_RuleEngine;
            }
        }

        private Authorization CoverageAuthorization
        {
            get
            {

                if (Model.GetType() == typeof(GovernmentMedicareCoverage))
                {
                    var cov = (GovernmentMedicareCoverage) Model;
                    if (cov != null && cov.IsMedicareCoverageValidForAuthorization )
                    {
                        return cov.Authorization;
                    }
                }

                if (Model.GetType().IsSubclassOf(typeof(CoverageGroup)))
                {
                    var cov = (CoverageGroup) Model;
                    return cov.Authorization;
                }

                if (Model.GetType() == typeof(GovernmentMedicaidCoverage))
                {
                    var cov = (GovernmentMedicaidCoverage) Model;
                    return cov.Authorization;
                }

                return new Authorization();
            }
            set
            {
                if (Model.GetType() == typeof(GovernmentMedicareCoverage))
                {
                    var cov = (GovernmentMedicareCoverage) Model;
                    if (cov != null && cov.IsMedicareCoverageValidForAuthorization )
                    {
                        cov.Authorization = value;
                        Model = cov;
                    }
                }

                if (Model.GetType().IsSubclassOf(typeof(CoverageGroup)))
                {
                    var cov = (CoverageGroup) Model;
                    cov.Authorization = value;
                    Model = cov;
                }

                if (Model.GetType() == typeof(GovernmentMedicaidCoverage))
                {
                    var cov = (GovernmentMedicaidCoverage) Model;
                    cov.Authorization = value;
                    Model = cov;
                }
            }
        }

        #endregion

        #region Private Methods

        private void CheckForRequiredFields()
        {
            UIColors.SetNormalBgColor( mtbAuthorizationNumber );
            UIColors.SetNormalBgColor( tbCompanyRepFirstName );
            UIColors.SetNormalBgColor( tbCompanyRepLastName );
            UIColors.SetNormalBgColor( tbServicesAuthorized );
            RuleEngine.GetInstance().EvaluateRule( typeof( WorkersCompAuthCodePreferred ), Model );
            RuleEngine.GetInstance().EvaluateRule( typeof( CompanyRepFirstNamePreferred ), Model );
            RuleEngine.GetInstance().EvaluateRule( typeof( CompanyRepLastNamePreferred ), Model );

            if ( Model != null &&
                 Model.Account != null &&
                 Model.Account.Activity != null &&
                 Model.Account.Activity.GetType() != typeof( PostMSERegistrationActivity ) )
            {
                RuleEngine.GetInstance().EvaluateRule( typeof( ServicesAuthorizedPreferred ), Model );
            }

        }

        private void UnRegisterEvents()
        {
            RuleEngine.GetInstance().UnregisterEvent( typeof( WorkersCompAuthCodePreferred ), Model, AuthorizationNumberPreferredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( CompanyRepFirstNamePreferred ), Model, CompanyRepFirstNamePreferredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( CompanyRepLastNamePreferred ), Model, CompanyRepLastNamePreferredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( ServicesAuthorizedPreferred ), Model, ServicesAuthorizedPreferredEventHandler );

        }

        private void RegisterEvents()
        {
            RuleEngine.GetInstance().RegisterEvent( typeof( WorkersCompAuthCodePreferred ), Model, new EventHandler( AuthorizationNumberPreferredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( CompanyRepFirstNamePreferred ), Model, new EventHandler( CompanyRepFirstNamePreferredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( CompanyRepLastNamePreferred ), Model, new EventHandler( CompanyRepLastNamePreferredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( ServicesAuthorizedPreferred ), Model, new EventHandler( ServicesAuthorizedPreferredEventHandler ) );

        }

        private void InitializeAuthorizationStatus()
        {
            ICollection authorizationStatuses = i_AuthorizationViewService.GetAllAuthorizationStatuses();

            cmbStatus.Items.Clear();
            if ( authorizationStatuses != null )
            {
                foreach ( AuthorizationStatus authorizationStatus in authorizationStatuses )
                {
                    cmbStatus.Items.Add( authorizationStatus );
                }
            }
        }

        private void ConfigureControls()
        {
            MaskedEditTextBoxBuilder.ConfigureAuthorizationRemarks( mtbRemarks );
        }

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public AuthorizationView()
        {
            InitializeComponent();

            ConfigureControls();
        }

        #endregion

        #region Data Elements

        private int month = 0;
        private int day = 0;
        private int year = 0;
        private Account i_Account = null;
        private RuleEngine i_RuleEngine;
        private bool effectiveDateWarning = true;
        private bool expirationDateWarning = true;
        private bool effectiveGreaterThanAdmitDateWarning = true;
        private bool expirationLesserThanAdmitDateWarning = true;
        private bool effectiveAdmitDateWarning = true;
        private bool expirationAdmitDateWarning = true;
        bool isValidEffDate = true;
        bool isValidExpDate = true;
        private readonly AuthorizationViewService i_AuthorizationViewService = new AuthorizationViewService();

        #endregion

        #region Constants
        #endregion


    }
}

