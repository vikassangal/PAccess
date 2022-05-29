using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Rules;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.InsuranceViews.InsuranceVerificationViews
{
    /// <summary>
    /// Summary description for VerificationEntry.
    /// </summary>
    public class VerificationEntry : ControlView
    {
        #region Event Handlers

        private void VerificationEntry_Disposed( object sender, EventArgs e )
        {
            UnRegisterEvents();
        }

        private void InsuranceBenefitsVerifiedPreferredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetPreferredBgColor( cmbBenefitsVerified );
        }

        private void InsuranceAuthorizationRequiredPreferredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetPreferredBgColor( cmbAuthorizationRequired );
        }


        private void vScrollBar_Scroll( object sender, ScrollEventArgs e )
        {
            vScrollPosn = e.NewValue;
            panelViewport.Invalidate();
        }

        private void panelViewport_Paint( object sender, PaintEventArgs e )
        {
            if( currentView != null )
            {
                currentView.Location = new Point( 0, -vScrollPosn );
            }
        }

        private void VerificationEntry_Validating( object sender, CancelEventArgs e )
        {
        }

        private void cmbBenefitsVerified_SelectedIndexChanged( object sender, EventArgs e )
        {
            UIColors.SetNormalBgColor( this.cmbBenefitsVerified );

            ComboBox cb = sender as ComboBox;

            benefitsVerifiedFlag = cb.SelectedItem as YesNotApplicableFlag;

            if( benefitsVerifiedFlag.Code.ToUpper().Equals( "Y" ) )
            {
                // Enable "Initiated By" field and clear the date.
                mtbInitiatedBy.UnMaskedText = String.Empty;
                benefitsInitiatedBy = String.Empty;
                mtbInitiatedBy.Enabled = true;
                cmbAuthorizationRequired.Enabled = !isSubClassOfCoverage;

                // Enable Date field and set to facility date
                ITimeBroker broker = ProxyFactory.GetTimeBroker();
                DateTime date = broker.TimeAt( Account.Facility.GMTOffset,
                                               Account.Facility.DSTOffset );
                mtbBenefitsDate.Enabled = true;
                dtpBenefitsDate.Enabled = true;
                mtbBenefitsDate.UnMaskedText = String.Format( "{0:D2}{1:D2}{2:D4}", date.Month, date.Day, date.Year );
            }
            else if( benefitsVerifiedFlag.Code.ToUpper().Equals( "N/A" ) )
            {
                if( Model_Coverage.InsurancePlan.GetType() != typeof( SelfPayInsurancePlan ) )
                {
                    ITimeBroker broker = ProxyFactory.GetTimeBroker();
                    DateTime date = broker.TimeAt( Account.Facility.GMTOffset,
                                                   Account.Facility.DSTOffset );
                    mtbBenefitsDate.Enabled = true;
                    dtpBenefitsDate.Enabled = true;
                    mtbBenefitsDate.UnMaskedText = String.Format( "{0:D2}{1:D2}{2:D4}", date.Month, date.Day, date.Year );
                }
                // Enable "Initiated By" field and clear the date.
                mtbInitiatedBy.UnMaskedText = String.Empty;
                benefitsInitiatedBy = String.Empty;
                mtbInitiatedBy.Enabled = true;

                // Set "Authorization Required" ComboBox to "N/A" and disable it.
                cmbAuthorizationRequired.SelectedIndex = 2;
                cmbAuthorizationRequired.Enabled = false;
            }
            else
            {
                // Clear "Initiated By" field, but do not enable it
                mtbInitiatedBy.UnMaskedText = String.Empty;
                mtbInitiatedBy.Enabled = false;
                benefitsInitiatedBy = String.Empty;
                // Clear "Date" field, but do not enable it
                mtbBenefitsDate.UnMaskedText = String.Empty;
                mtbBenefitsDate.Enabled = false;
                UIColors.SetNormalBgColor( mtbBenefitsDate );
                dtpBenefitsDate.Enabled = false;
                benefitsVerifiedDate = DateTime.MinValue;
                cmbAuthorizationRequired.Enabled = !isSubClassOfCoverage;
            }

            Model_Coverage.BenefitsVerified = benefitsVerifiedFlag;
            CheckForRequiredFields();
        }

        private void cmbAuthorizationRequired_SelectedIndexChanged( object sender, EventArgs e )
        {
            UIColors.SetNormalBgColor( this.cmbAuthorizationRequired );
            ComboBox cb = sender as ComboBox;

            authorizationRequiredFlag = cb.SelectedItem as YesNotApplicableFlag;

            if( authorizationRequiredFlag.Code.ToUpper().Equals( "Y" ) )
            {
                mtbAuthorizationCompany.Enabled = true;
                mtbPromptExt.Enabled = true;
                this.phoneNumberControl.ToggleEnabled( true );
            }
            else if( authorizationRequiredFlag.Code.ToUpper().Equals( "N/A" ) )
            {
                mtbAuthorizationCompany.Enabled = false;
                mtbPromptExt.Enabled = false;
                this.phoneNumberControl.ToggleEnabled( false );
            }
            else
            {
                mtbAuthorizationCompany.Enabled = false;
                mtbAuthorizationCompany.UnMaskedText = String.Empty;
                authorizationCompany = String.Empty;

                this.phoneNumberControl.ToggleEnabled( false );
                this.phoneNumberControl.ClearText();
                benefitsPhoneNumber = new PhoneNumber();

                mtbPromptExt.Enabled = false;
                mtbPromptExt.UnMaskedText = String.Empty;
                promptExt = String.Empty;

                mtbInitiatedBy.UnMaskedText = String.Empty;
                benefitsInitiatedBy = String.Empty;
            }
            if( Model_Coverage.GetType().IsSubclassOf( typeof( CoverageGroup ) ) )
            {
                CoverageGroup coverageGroup = Model_Coverage as CoverageGroup;
                coverageGroup.Authorization.AuthorizationRequired = this.authorizationRequiredFlag;

            }

            CheckForRequiredFields();
        }

        private void mtbInitiatedBy_Validating( object sender, CancelEventArgs e )
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            if( mtb.UnMaskedText != String.Empty )
            {
                benefitsInitiatedBy = mtb.UnMaskedText;
            }
        }

        private void dtpBenefitsDate_CloseUp( object sender, EventArgs e )
        {
            DateTimePicker dtp = sender as DateTimePicker;
            UIColors.SetNormalBgColor( mtbBenefitsDate );
            mtbBenefitsDate.Refresh();
            this.Refresh();
            DateTime dt = dtp.Value;
            mtbBenefitsDate.UnMaskedText = String.Format( "{0:D2}{1:D2}{2:D4}", dt.Month, dt.Day, dt.Year );
            this.mtbBenefitsDate.Focus();
        }

        private void mtbBenefitsDate_Enter( object sender, EventArgs e )
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            mtb.Refresh();
            this.Refresh();
        }

        private void mtbBenefitsDate_Validating( object sender, CancelEventArgs e )
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            if( dtpBenefitsDate.Focused )
            {
                return;
            }
            if( mtb.UnMaskedText == String.Empty )
            {
                UIColors.SetNormalBgColor( mtb );
                Refresh();
                return;
            }

            bool correctDate = InsuranceDateVerify.VerifyInsuranceDate( ref mtb, ref insuranceYear, ref insuranceMonth, ref insuranceDay );
            if( correctDate )
            {
                UIColors.SetNormalBgColor( mtb );
                Refresh();
                benefitsVerifiedDate = new DateTime( insuranceYear, insuranceMonth, insuranceDay );
            }
            else
            {
                benefitsVerifiedDate = DateTime.MinValue;
            }

            if( mtbBenefitsDate.UnMaskedText != String.Empty )
            {   // Verify for a valid date
                if( correctDate )
                {
                    // If a valid date, see if the date is in the future
                    ITimeBroker broker = ProxyFactory.GetTimeBroker();
                    DateTime todaysDate = broker.TimeAt( Account.Facility.GMTOffset,
                                                         Account.Facility.DSTOffset );
                    DateTime verifyDate = new DateTime( insuranceYear,
                                                        insuranceMonth,
                                                        insuranceDay );

                    if( verifyDate > todaysDate )
                    {
                        e.Cancel = true;
                        benefitsVerifiedDate = DateTime.MinValue;
                        UIColors.SetErrorBgColor( mtbBenefitsDate );
                        MessageBox.Show( UIErrorMessages.BENEFITS_VERIFIED_FUTURE_DATE, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                            MessageBoxDefaultButton.Button1 );
                        return;
                    }
                }
                else
                {
                    // Mal-formed date.

                    benefitsVerifiedDate = DateTime.MinValue;
                    return;
                }
            }

        }

        private void mtbAuthorizationCompany_Validating( object sender, CancelEventArgs e )
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            if( mtb.UnMaskedText != String.Empty )
            {
                authorizationCompany = mtb.Text;
            }
        }

        private void phoneNumberControl_Validating( object sender, CancelEventArgs e )
        {
            this.benefitsPhoneNumber = this.phoneNumberControl.Model;
        }

        private void mtbPromptExt_Validating( object sender, CancelEventArgs e )
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            if( mtb.UnMaskedText != String.Empty )
            {
                promptExt = mtb.Text;
            }
        }

        private void VerificationEntry_Leave( object sender, EventArgs e )
        {
            this.UpdateModel();
        }

        #endregion

        #region Methods
        public override void UpdateView()
        {
            if( loadingModelData )
            {
                loadingModelData = false;

                PopulateBenefitsVerifiedComboBox();
                PopulateAuthorizationRequiredComboBox();

                if( Model_Coverage.BenefitsVerified != null )
                {
                    cmbBenefitsVerified.SelectedItem = Model_Coverage.BenefitsVerified;
                }
                if( Model_Coverage.GetType().IsSubclassOf( typeof( CoverageGroup ) ) )
                {
                    CoverageGroup coverageGroup = Model_Coverage as CoverageGroup;
                    if( coverageGroup.Authorization != null )
                    {
                        if( coverageGroup.Authorization.AuthorizationRequired != null )
                        {
                            cmbAuthorizationRequired.SelectedItem = coverageGroup.Authorization.AuthorizationRequired;
                        }
                        if( coverageGroup.Authorization.AuthorizationCompany != String.Empty )
                        {
                            mtbAuthorizationCompany.Text = coverageGroup.Authorization.AuthorizationCompany.Trim();
                        }
                        if( coverageGroup.Authorization.AuthorizationPhone != null )
                        {
                            this.phoneNumberControl.Model = coverageGroup.Authorization.AuthorizationPhone;
                        }
                        if( coverageGroup.Authorization.PromptExt != null )
                        {
                            mtbPromptExt.Text = coverageGroup.Authorization.PromptExt.Trim();
                        }
                    }
                }

                this.mtbInitiatedBy.UnMaskedText = Model_Coverage.AuthorizingPerson.Trim();

                if( Model_Coverage.DateTimeOfVerification != DateTime.MinValue )
                {
                    mtbBenefitsDate.UnMaskedText = Model_Coverage.DateTimeOfVerification.ToString( "MMddyyyy" );
                }


                // populate the original value variables:

                if( Model_Coverage.BenefitsVerified != null )
                {
                    originalBenefitsVerifiedFlag = Model_Coverage.BenefitsVerified;
                    this.benefitsVerifiedFlag = originalBenefitsVerifiedFlag;
                }
                else
                {
                    originalBenefitsVerifiedFlag = new YesNotApplicableFlag();
                    originalBenefitsVerifiedFlag.SetBlank();
                }

                originalBenefitsInitiatedBy = Model_Coverage.AuthorizingPerson.Trim();
                this.benefitsInitiatedBy = originalBenefitsInitiatedBy;

                originalBenefitsVerifiedDate = Model_Coverage.DateTimeOfVerification;
                this.benefitsVerifiedDate = originalBenefitsVerifiedDate;
                if( Model_Coverage.GetType().IsSubclassOf( typeof( CoverageGroup ) ) )
                {
                    CoverageGroup coverageGroup = Model_Coverage as CoverageGroup;
                    if( coverageGroup.Authorization.AuthorizationRequired != null )
                    {
                        originalAuthRequiredFlag = coverageGroup.Authorization.AuthorizationRequired;
                        this.authorizationRequiredFlag = originalAuthRequiredFlag;
                    }
                    else
                    {
                        originalAuthRequiredFlag = new YesNotApplicableFlag();
                        originalAuthRequiredFlag.SetBlank();
                    }
                    originalAuthorizationCompany = coverageGroup.Authorization.AuthorizationCompany.Trim();
                    this.authorizationCompany = originalAuthorizationCompany;

                    originalAuthorizationPhone = coverageGroup.Authorization.AuthorizationPhone;

                    if( originalAuthorizationPhone != null )
                    {
                        this.benefitsPhoneNumber = originalAuthorizationPhone;
                    }

                    originalAuthorizationExtension = coverageGroup.Authorization.PromptExt.Trim();
                    this.promptExt = originalAuthorizationExtension;

                }

                //RuleEngine.LoadRules( Account );

                RuleEngine.GetInstance().RegisterEvent( typeof( InsuranceBenefitsVerifiedPreferred ), Model_Coverage, new EventHandler( InsuranceBenefitsVerifiedPreferredEventHandler ) );
                RuleEngine.GetInstance().RegisterEvent( typeof( InsuranceAuthorizationRequiredPreferred ), Model_Coverage, new EventHandler( InsuranceAuthorizationRequiredPreferredEventHandler ) );

            }
            if( Model_Coverage != null && Model_Coverage.InsurancePlan != null )
            {
                DisplayPayorView( Model_Coverage.InsurancePlan.GetType() );
            }
            Refresh();
            //UIColors.SetErrorBgColor( this.mtbPhone );

            if( mtbBenefitsDate.UnMaskedText != String.Empty )
            {
                if( InsuranceDateVerify.VerifyInsuranceDate( ref mtbBenefitsDate, ref insuranceYear, ref insuranceMonth, ref insuranceDay ) )
                {
                    ITimeBroker broker = ProxyFactory.GetTimeBroker();
                    DateTime todaysDate = broker.TimeAt( Account.Facility.GMTOffset,
                                                         Account.Facility.DSTOffset );
                    DateTime verifyDate = new DateTime( insuranceYear, insuranceMonth, insuranceDay );

                    if( verifyDate > todaysDate )
                    {
                        UIColors.SetErrorBgColor( mtbBenefitsDate );
                        MessageBox.Show( "The Benefits verified date cannot be in the future. ", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                            MessageBoxDefaultButton.Button1 );
                    }
                }
                else
                {
                    return;
                }
            }

            this.phoneNumberControl.RunRules();

            CheckForRequiredFields();
        }

        public override void UpdateModel()
        {
            if( Model_Coverage == null )
            {
                return;
            }

            if( this.cmbBenefitsVerified.SelectedItem != null )
            {
                Model_Coverage.BenefitsVerified = this.cmbBenefitsVerified.SelectedItem as YesNotApplicableFlag;
            }
            if( Model_Coverage.GetType().IsSubclassOf( typeof( CoverageGroup ) ) )
            {
                CoverageGroup coverageGroup = Model_Coverage as CoverageGroup;
                if( this.cmbAuthorizationRequired != null )
                {
                    coverageGroup.Authorization.AuthorizationRequired = this.cmbAuthorizationRequired.SelectedItem as YesNotApplicableFlag;
                }
                coverageGroup.Authorization.AuthorizationCompany = this.mtbAuthorizationCompany.UnMaskedText.Trim();

                if( this.phoneNumberControl.PhoneNumber != null )
                {
                    coverageGroup.Authorization.AuthorizationPhone = this.phoneNumberControl.Model;
                }

                coverageGroup.Authorization.PromptExt = this.mtbPromptExt.UnMaskedText.Trim();
            }


            Model_Coverage.AuthorizingPerson = this.mtbInitiatedBy.UnMaskedText.Trim();

            if( this.mtbBenefitsDate.UnMaskedText.Replace( "/", string.Empty ).Trim() != string.Empty )
            {
                try
                {
                    Model_Coverage.DateTimeOfVerification = DateTime.Parse( this.mtbBenefitsDate.Text );
                }
                catch
                {
                    Model_Coverage.DateTimeOfVerification = DateTime.MinValue;
                }
            }

            if( currentView != null )
            {
                currentView.UpdateModel();
            }

            // Determine if RBVCA note should be written            

            if( Model_Coverage.BenefitsVerified != null
                && this.originalBenefitsVerifiedFlag.Code != Model_Coverage.BenefitsVerified.Code )
            {
                if( !Model_Coverage.BenefitsVerified.IsBlank )
                {
                    Model_Coverage.WriteBenefitsVerifiedFUSNote = true;
                }
                else
                {
                    Model_Coverage.WriteBenefitsVerifiedFUSNote = false;
                }
            }
            else
            {
                if( Model_Coverage.BenefitsVerified != null
                    && !Model_Coverage.BenefitsVerified.IsBlank )
                {
                    if( this.originalBenefitsInitiatedBy != Model_Coverage.AuthorizingPerson
                        || this.originalBenefitsVerifiedDate != Model_Coverage.DateTimeOfVerification )
                    {
                        Model_Coverage.WriteBenefitsVerifiedFUSNote = true;
                    }
                }
            }

            // Determine if RARRA note should be written            
            if( Model_Coverage.GetType().IsSubclassOf( typeof( CoverageGroup ) ) )
            {
                CoverageGroup coverageGroup = Model_Coverage as CoverageGroup;

                if( coverageGroup.Authorization.AuthorizationRequired != null
                    && this.originalAuthRequiredFlag.Code != coverageGroup.Authorization.AuthorizationRequired.Code )
                {
                    if( !coverageGroup.Authorization.AuthorizationRequired.IsBlank )
                    {
                        Model_Coverage.WriteAuthRequiredFUSNote = true;
                    }
                    else
                    {
                        Model_Coverage.WriteAuthRequiredFUSNote = false;
                    }
                }
                else
                {
                    if( coverageGroup.Authorization.AuthorizationRequired != null
                        && !coverageGroup.Authorization.AuthorizationRequired.IsBlank )
                    {
                        if( this.originalAuthorizationCompany != coverageGroup.Authorization.AuthorizationCompany
                            || this.originalAuthorizationPhone != coverageGroup.Authorization.AuthorizationPhone
                            || this.originalAuthorizationExtension != coverageGroup.Authorization.PromptExt )
                        {
                            Model_Coverage.WriteAuthRequiredFUSNote = true;
                        }
                    }
                }
            }
            else
            {
                Model_Coverage.WriteAuthRequiredFUSNote = true;
            }

            // Unconditionally format RVINB, RVINS, MCWFI, or IMEVC notes
            // They will only be written if values were provided.

            Model_Coverage.WriteVerificationEntryFUSNote = true;
        }

        public void UpdateBenefitsCategoriesView()
        {
            this.commMgdCareVerifyView.InvokeBenefitsCategoriesSelectionChangedEvent();
        }

        #endregion

        #region Properties
        [Browsable( false )]
        public Coverage Model_Coverage
        {
            set
            {
                this.Model = value;
            }
            private get
            {
                return (Coverage)this.Model;
            }
        }

        [Browsable( false )]
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
                if( i_RuleEngine == null )
                {
                    i_RuleEngine = RuleEngine.GetInstance();
                }
                return i_RuleEngine;
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// CheckForRequiredFields - determine if the user has entered all required fields
        /// Some are conditional based on other fields.  Returns true or false;
        /// </summary>
        /// <returns></returns>
        private void CheckForRequiredFields()
        {
            RuleEngine.GetInstance().EvaluateRule( typeof( InsuranceBenefitsVerifiedPreferred ), Model_Coverage );
            RuleEngine.GetInstance().EvaluateRule( typeof( InsuranceAuthorizationRequiredPreferred ), Model_Coverage );
        }

        private void DisplayPayorView( Type coverageType )
        {
            if( currentView != null )
            {
                currentView.Visible = false;
                currentView.Hide();
            }
            isSubClassOfCoverage = false;

            currentView = (ControlView)viewTable[coverageType];


            if( coverageType == typeof( CommercialInsurancePlan ) )
            {
                commMgdCareVerifyView.Model_Coverage = (CommercialCoverage)Model_Coverage;
                commMgdCareVerifyView.Account = Account;

                vScrollBar.Enabled = true;
                vScrollBar.Maximum = commMgdCareVerifyView.Height - commMgdCareVerifyView.Height / 4 + 10;
                vScrollBar.LargeChange = vScrollBar.Maximum / 10;
                vScrollBar.SmallChange = vScrollBar.Maximum / 20;
            }
            else if( coverageType == typeof( GovernmentMedicaidInsurancePlan ) )
            {
                medicaidVerifyView.Model_Coverage = (GovernmentMedicaidCoverage)Model_Coverage;
                medicaidVerifyView.Account = Account;

                vScrollBar.Enabled = true;
                vScrollBar.Maximum = governmentVerifyView.Height - governmentVerifyView.Height / 2;
                vScrollBar.LargeChange = vScrollBar.Maximum / 10;
                vScrollBar.SmallChange = vScrollBar.Maximum / 20;
            }
            else if( coverageType == typeof( GovernmentMedicareInsurancePlan ) )
            {
                medicareVerifyView.Model_Coverage = (GovernmentMedicareCoverage)Model_Coverage;
                medicareVerifyView.Account = Account;
                isSubClassOfCoverage = true;
                // Defect # 8372 - first evaluate if the value was previously
                // set before defaulting to N/A


                cmbAuthorizationRequired.Enabled = false;
                cmbAuthorizationRequired.SelectedIndex = 2;

                vScrollBar.Enabled = true;
                vScrollBar.Maximum = medicareVerifyView.Height - medicareVerifyView.Height / 2 - 18;
                vScrollBar.LargeChange = vScrollBar.Maximum / 10;
                vScrollBar.SmallChange = vScrollBar.Maximum / 20;

            }
            else if( coverageType == typeof( GovernmentOtherInsurancePlan ) )
            {
                governmentVerifyView.Model_Coverage = (GovernmentOtherCoverage)Model_Coverage;
                governmentVerifyView.Account = Account;

                vScrollBar.Enabled = true;
                vScrollBar.Maximum = governmentVerifyView.Height - governmentVerifyView.Height / 2;
                vScrollBar.LargeChange = vScrollBar.Maximum / 10;
                vScrollBar.SmallChange = vScrollBar.Maximum / 20;
            }
            else if( coverageType == typeof( OtherInsurancePlan ) )
            {
                commMgdCareVerifyView.Model_Coverage = (OtherCoverage)Model_Coverage;
                commMgdCareVerifyView.Account = Account;

                vScrollBar.Enabled = true;
                vScrollBar.Maximum = commMgdCareVerifyView.Height - commMgdCareVerifyView.Height / 4 + 10;
                vScrollBar.LargeChange = vScrollBar.Maximum / 10;
                vScrollBar.SmallChange = vScrollBar.Maximum / 20;
            }
            else if( coverageType == typeof( SelfPayInsurancePlan ) )
            {
                selfPayVerifyView.Model = (SelfPayCoverage)Model_Coverage;
                vScrollBar.Enabled = true;
                vScrollBar.Maximum = medicareVerifyView.Height - medicareVerifyView.Height / 2 - 18;
                vScrollBar.LargeChange = vScrollBar.Maximum / 10;
                vScrollBar.SmallChange = vScrollBar.Maximum / 20;
                isSubClassOfCoverage = true;
                cmbBenefitsVerified.Enabled = true;

                mtbInitiatedBy.Enabled = false;
                mtbBenefitsDate.Enabled = false;

                // Defect # 8372 - first evaluate if the value was previously
                // set before defaulting to N/A


                cmbAuthorizationRequired.Enabled = false;
                cmbAuthorizationRequired.SelectedIndex = 2;

            }
            else if( coverageType == typeof( WorkersCompensationInsurancePlan ) )
            {
                workersCompVerifyView.Model_Coverage = (WorkersCompensationCoverage)Model_Coverage;
                workersCompVerifyView.Account = Account;

                vScrollBar.Enabled = true;
                vScrollBar.Maximum = workersCompVerifyView.Height - workersCompVerifyView.Height / 2;
                vScrollBar.LargeChange = vScrollBar.Maximum / 10;
                vScrollBar.SmallChange = vScrollBar.Maximum / 20;
            }
            if( currentView != null )
            {
                currentView.Visible = true;
                currentView.Show();
                currentView.UpdateView();
            }
        }

        private CommMgdCareVerifyView CreateCommercialManagedCareView()
        {
            commMgdCareVerifyView = new CommMgdCareVerifyView();
            panelViewport.Controls.Add( this.commMgdCareVerifyView );
            commMgdCareVerifyView.AutoScroll = true;
            commMgdCareVerifyView.Location = new Point( 0, 0 );
            commMgdCareVerifyView.Name = "commMgdCareVerifyView";
            commMgdCareVerifyView.Size = new Size( 850, 1200 );
            commMgdCareVerifyView.TabIndex = 0;
            commMgdCareVerifyView.Visible = false;
            return commMgdCareVerifyView;
        }

        private MedicaidVerifyView CreateMedicaidView()
        {
            medicaidVerifyView = new MedicaidVerifyView();
            panelViewport.Controls.Add( medicaidVerifyView );
            medicaidVerifyView.AutoScroll = true;
            medicaidVerifyView.Location = new Point( 0, 0 );
            medicaidVerifyView.Name = "medicaidVerifyView";
            medicaidVerifyView.Size = new Size( 847, 1876 );
            medicaidVerifyView.TabIndex = 0;
            medicaidVerifyView.Visible = false;
            return medicaidVerifyView;
        }

        private MedicareVerifyView CreateMedicareView()
        {
            medicareVerifyView = new MedicareVerifyView();
            panelViewport.Controls.Add( medicareVerifyView );
            medicareVerifyView.AutoScroll = true;
            medicareVerifyView.Location = new Point( 0, 0 );
            medicareVerifyView.Name = "medicareVerifyView";
            medicareVerifyView.Size = new Size( 847, 335 );
            medicareVerifyView.TabIndex = 0;
            medicareVerifyView.Visible = false;
            return medicareVerifyView;
        }

        private GovernmentVerifyView CreateGovernmentView()
        {
            governmentVerifyView = new GovernmentVerifyView();
            panelViewport.Controls.Add( governmentVerifyView );
            governmentVerifyView.AutoScroll = true;
            governmentVerifyView.Location = new Point( 0, 0 );
            governmentVerifyView.Name = "governmentVerifyView";
            governmentVerifyView.Size = new Size( 847, 425 );
            governmentVerifyView.TabIndex = 0;
            governmentVerifyView.Visible = false;
            return governmentVerifyView;
        }

        private SelfPayVerifyView CreateSelfPayView()
        {
            selfPayVerifyView = new SelfPayVerifyView();
            panelViewport.Controls.Add( selfPayVerifyView );
            selfPayVerifyView.AutoScroll = true;
            selfPayVerifyView.Location = new Point( 0, 0 );
            selfPayVerifyView.Name = "selfPayVerifyView";
            selfPayVerifyView.Size = new Size( 847, 180 );
            selfPayVerifyView.TabIndex = 0;
            selfPayVerifyView.Visible = false;
            return selfPayVerifyView;
        }

        private WorkersCompVerifyView CreateWorkersCompView()
        {
            this.workersCompVerifyView = new WorkersCompVerifyView();
            this.panelViewport.Controls.Add( this.workersCompVerifyView );
            this.workersCompVerifyView.AutoScroll = true;
            this.workersCompVerifyView.Location = new Point( 0, 0 );
            this.workersCompVerifyView.Name = "workersCompVerifyView";
            this.workersCompVerifyView.Size = new Size( 850, 550 );
            this.workersCompVerifyView.TabIndex = 0;
            this.workersCompVerifyView.Visible = false;
            return workersCompVerifyView;
        }

        private void PopulateViewTable()
        {
            commMgdCareVerifyView = CreateCommercialManagedCareView();
            medicaidVerifyView = CreateMedicaidView();
            medicareVerifyView = CreateMedicareView();
            governmentVerifyView = CreateGovernmentView();
            selfPayVerifyView = CreateSelfPayView();
            workersCompVerifyView = CreateWorkersCompView();

            viewTable.Add( typeof( CommercialInsurancePlan ), commMgdCareVerifyView );
            viewTable.Add( typeof( GovernmentMedicaidInsurancePlan ), medicaidVerifyView );
            viewTable.Add( typeof( GovernmentMedicareInsurancePlan ), medicareVerifyView );
            viewTable.Add( typeof( GovernmentOtherInsurancePlan ), governmentVerifyView );
            viewTable.Add( typeof( OtherInsurancePlan ), commMgdCareVerifyView );
            viewTable.Add( typeof( SelfPayInsurancePlan ), selfPayVerifyView );
            viewTable.Add( typeof( WorkersCompensationInsurancePlan ), workersCompVerifyView );
        }

        private void PopulateAuthorizationRequiredComboBox()
        {
            cmbAuthorizationRequired.Items.Add( blankYesNotAppFlag );
            cmbAuthorizationRequired.Items.Add( yesYesNotAppFlag );
            cmbAuthorizationRequired.Items.Add( noYesNotAppFlag );
        }

        private void PopulateBenefitsVerifiedComboBox()
        {
            cmbBenefitsVerified.Items.Add( blankYesNotAppFlag );
            cmbBenefitsVerified.Items.Add( yesYesNotAppFlag );
            cmbBenefitsVerified.Items.Add( noYesNotAppFlag );
        }

        private void UnRegisterEvents()
        {
            RuleEngine.GetInstance().UnregisterEvent( typeof( InsuranceBenefitsVerifiedPreferred ), Model_Coverage, new EventHandler( InsuranceBenefitsVerifiedPreferredEventHandler ) );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InsuranceAuthorizationRequiredPreferred ), Model_Coverage, new EventHandler( InsuranceAuthorizationRequiredPreferredEventHandler ) );
        }

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( VerificationEntry ) );
            this.cmbBenefitsVerified = new System.Windows.Forms.ComboBox();
            this.lblBenefits = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.cmbAuthorizationRequired = new System.Windows.Forms.ComboBox();
            this.lblInitiatedBy = new System.Windows.Forms.Label();
            this.mtbInitiatedBy = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtbAuthorizationCompany = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblDate = new System.Windows.Forms.Label();
            this.dtpBenefitsDate = new System.Windows.Forms.DateTimePicker();
            this.mtbBenefitsDate = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblPhone = new System.Windows.Forms.Label();
            this.lblPhoneExt = new System.Windows.Forms.Label();
            this.mtbPromptExt = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.panelViewport = new System.Windows.Forms.Panel();
            this.vScrollBar = new System.Windows.Forms.VScrollBar();
            this.panelControls = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.panelSeparator = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.phoneNumberControl = new PatientAccess.UI.CommonControls.PhoneNumberControl();
            this.panelViewport.SuspendLayout();
            this.panelControls.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmbBenefitsVerified
            // 
            this.cmbBenefitsVerified.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBenefitsVerified.Location = new System.Drawing.Point( 128, 6 );
            this.cmbBenefitsVerified.Name = "cmbBenefitsVerified";
            this.cmbBenefitsVerified.Size = new System.Drawing.Size( 47, 21 );
            this.cmbBenefitsVerified.TabIndex = 2;
            this.cmbBenefitsVerified.SelectedIndexChanged += new System.EventHandler( this.cmbBenefitsVerified_SelectedIndexChanged );
            // 
            // lblBenefits
            // 
            this.lblBenefits.Location = new System.Drawing.Point( 8, 8 );
            this.lblBenefits.Name = "lblBenefits";
            this.lblBenefits.Size = new System.Drawing.Size( 88, 17 );
            this.lblBenefits.TabIndex = 0;
            this.lblBenefits.Text = "Benefits verified:";
            // 
            // lblTitle
            // 
            this.lblTitle.BackColor = System.Drawing.Color.FromArgb( ( (int)( ( (byte)( 204 ) ) ) ), ( (int)( ( (byte)( 204 ) ) ) ), ( (int)( ( (byte)( 204 ) ) ) ) );
            this.lblTitle.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            this.lblTitle.Location = new System.Drawing.Point( 8, 6 );
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size( 240, 16 );
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Insurance Verification Entry Form";
            // 
            // cmbAuthorizationRequired
            // 
            this.cmbAuthorizationRequired.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAuthorizationRequired.Location = new System.Drawing.Point( 128, 30 );
            this.cmbAuthorizationRequired.Name = "cmbAuthorizationRequired";
            this.cmbAuthorizationRequired.Size = new System.Drawing.Size( 48, 21 );
            this.cmbAuthorizationRequired.TabIndex = 5;
            this.cmbAuthorizationRequired.SelectedIndexChanged += new System.EventHandler( this.cmbAuthorizationRequired_SelectedIndexChanged );
            // 
            // lblInitiatedBy
            // 
            this.lblInitiatedBy.Location = new System.Drawing.Point( 192, 8 );
            this.lblInitiatedBy.Name = "lblInitiatedBy";
            this.lblInitiatedBy.Size = new System.Drawing.Size( 64, 17 );
            this.lblInitiatedBy.TabIndex = 0;
            this.lblInitiatedBy.Text = "Initialed by:";
            // 
            // mtbInitiatedBy
            // 
            this.mtbInitiatedBy.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbInitiatedBy.Enabled = false;
            this.mtbInitiatedBy.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbInitiatedBy.KeyPressExpression = "^[a-zA-Z][a-zA-Z0-9\\-]*$";
            this.mtbInitiatedBy.Location = new System.Drawing.Point( 256, 6 );
            this.mtbInitiatedBy.Mask = string.Empty;
            this.mtbInitiatedBy.MaxLength = 4;
            this.mtbInitiatedBy.Name = "mtbInitiatedBy";
            this.mtbInitiatedBy.Size = new System.Drawing.Size( 36, 20 );
            this.mtbInitiatedBy.TabIndex = 3;
            this.mtbInitiatedBy.ValidationExpression = "^[a-zA-Z][a-zA-Z0-9\\-]*$";
            this.mtbInitiatedBy.Validating += new System.ComponentModel.CancelEventHandler( this.mtbInitiatedBy_Validating );
            // 
            // mtbAuthorizationCompany
            // 
            this.mtbAuthorizationCompany.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbAuthorizationCompany.Enabled = false;
            this.mtbAuthorizationCompany.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbAuthorizationCompany.KeyPressExpression = "^[a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.mtbAuthorizationCompany.Location = new System.Drawing.Point( 315, 30 );
            this.mtbAuthorizationCompany.Mask = string.Empty;
            this.mtbAuthorizationCompany.MaxLength = 25;
            this.mtbAuthorizationCompany.Name = "mtbAuthorizationCompany";
            this.mtbAuthorizationCompany.Size = new System.Drawing.Size( 162, 20 );
            this.mtbAuthorizationCompany.TabIndex = 6;
            this.mtbAuthorizationCompany.ValidationExpression = "^[a-zA-Z][a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.mtbAuthorizationCompany.Validating += new System.ComponentModel.CancelEventHandler( this.mtbAuthorizationCompany_Validating );
            // 
            // lblDate
            // 
            this.lblDate.Location = new System.Drawing.Point( 313, 8 );
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size( 33, 17 );
            this.lblDate.TabIndex = 0;
            this.lblDate.Text = "Date:";
            // 
            // dtpBenefitsDate
            // 
            this.dtpBenefitsDate.CalendarTitleForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.dtpBenefitsDate.Checked = false;
            this.dtpBenefitsDate.Enabled = false;
            this.dtpBenefitsDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpBenefitsDate.Location = new System.Drawing.Point( 422, 6 );
            this.dtpBenefitsDate.MinDate = new System.DateTime( 1800, 1, 1, 0, 0, 0, 0 );
            this.dtpBenefitsDate.Name = "dtpBenefitsDate";
            this.dtpBenefitsDate.Size = new System.Drawing.Size( 21, 20 );
            this.dtpBenefitsDate.TabIndex = 0;
            this.dtpBenefitsDate.TabStop = false;
            this.dtpBenefitsDate.CloseUp += new System.EventHandler( this.dtpBenefitsDate_CloseUp );
            // 
            // mtbBenefitsDate
            // 
            this.mtbBenefitsDate.Enabled = false;
            this.mtbBenefitsDate.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbBenefitsDate.KeyPressExpression = HelperClasses.DateValidator.DATEKeyPressExpression;
            this.mtbBenefitsDate.Location = new System.Drawing.Point( 352, 6 );
            this.mtbBenefitsDate.Mask = "  /  /";
            this.mtbBenefitsDate.MaxLength = 10;
            this.mtbBenefitsDate.Name = "mtbBenefitsDate";
            this.mtbBenefitsDate.Size = new System.Drawing.Size( 70, 20 );
            this.mtbBenefitsDate.TabIndex = 4;
            this.mtbBenefitsDate.ValidationExpression = HelperClasses.DateValidator.DATEValidationExpression;
            this.mtbBenefitsDate.Enter += new System.EventHandler( this.mtbBenefitsDate_Enter );
            this.mtbBenefitsDate.Validating += new System.ComponentModel.CancelEventHandler( this.mtbBenefitsDate_Validating );
            // 
            // lblPhone
            // 
            this.lblPhone.Location = new System.Drawing.Point( 528, 32 );
            this.lblPhone.Name = "lblPhone";
            this.lblPhone.Size = new System.Drawing.Size( 41, 16 );
            this.lblPhone.TabIndex = 0;
            this.lblPhone.Text = "Phone:";
            // 
            // lblPhoneExt
            // 
            this.lblPhoneExt.Location = new System.Drawing.Point( 712, 32 );
            this.lblPhoneExt.Name = "lblPhoneExt";
            this.lblPhoneExt.Size = new System.Drawing.Size( 64, 16 );
            this.lblPhoneExt.TabIndex = 0;
            this.lblPhoneExt.Text = "Prompt/ext:";
            // 
            // mtbPromptExt
            // 
            this.mtbPromptExt.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbPromptExt.Enabled = false;
            this.mtbPromptExt.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbPromptExt.KeyPressExpression = "^[a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.mtbPromptExt.Location = new System.Drawing.Point( 777, 28 );
            this.mtbPromptExt.Mask = string.Empty;
            this.mtbPromptExt.MaxLength = 10;
            this.mtbPromptExt.Name = "mtbPromptExt";
            this.mtbPromptExt.Size = new System.Drawing.Size( 83, 20 );
            this.mtbPromptExt.TabIndex = 8;
            this.mtbPromptExt.ValidationExpression = "^[a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.mtbPromptExt.Validating += new System.ComponentModel.CancelEventHandler( this.mtbPromptExt_Validating );
            // 
            // panelViewport
            // 
            this.panelViewport.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
                        | System.Windows.Forms.AnchorStyles.Left )
                        | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.panelViewport.BackColor = System.Drawing.Color.White;
            this.panelViewport.Controls.Add( this.vScrollBar );
            this.panelViewport.Location = new System.Drawing.Point( 0, 27 );
            this.panelViewport.Name = "panelViewport";
            this.panelViewport.Size = new System.Drawing.Size( 885, 221 );
            this.panelViewport.TabIndex = 1;
            this.panelViewport.TabStop = true;
            this.panelViewport.Paint += new System.Windows.Forms.PaintEventHandler( this.panelViewport_Paint );
            // 
            // vScrollBar
            // 
            this.vScrollBar.Dock = System.Windows.Forms.DockStyle.Right;
            this.vScrollBar.Location = new System.Drawing.Point( 868, 0 );
            this.vScrollBar.Name = "vScrollBar";
            this.vScrollBar.Size = new System.Drawing.Size( 17, 221 );
            this.vScrollBar.TabIndex = 0;
            this.vScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler( this.vScrollBar_Scroll );
            // 
            // panelControls
            // 
            this.panelControls.Controls.Add( this.phoneNumberControl );
            this.panelControls.Controls.Add( this.mtbPromptExt );
            this.panelControls.Controls.Add( this.mtbAuthorizationCompany );
            this.panelControls.Controls.Add( this.mtbInitiatedBy );
            this.panelControls.Controls.Add( this.cmbAuthorizationRequired );
            this.panelControls.Controls.Add( this.cmbBenefitsVerified );
            this.panelControls.Controls.Add( this.lblBenefits );
            this.panelControls.Controls.Add( this.lblInitiatedBy );
            this.panelControls.Controls.Add( this.lblDate );
            this.panelControls.Controls.Add( this.mtbBenefitsDate );
            this.panelControls.Controls.Add( this.dtpBenefitsDate );
            this.panelControls.Controls.Add( this.lblPhone );
            this.panelControls.Controls.Add( this.lblPhoneExt );
            this.panelControls.Controls.Add( this.label1 );
            this.panelControls.Controls.Add( this.label3 );
            this.panelControls.Controls.Add( this.panelSeparator );
            this.panelControls.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelControls.Location = new System.Drawing.Point( 0, 248 );
            this.panelControls.Name = "panelControls";
            this.panelControls.Size = new System.Drawing.Size( 885, 56 );
            this.panelControls.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point( 8, 34 );
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size( 120, 13 );
            this.label1.TabIndex = 0;
            this.label1.Text = "Authorization required:";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point( 192, 32 );
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size( 123, 16 );
            this.label3.TabIndex = 0;
            this.label3.Text = "Authorization company:";
            // 
            // panelSeparator
            // 
            this.panelSeparator.BackColor = System.Drawing.Color.Black;
            this.panelSeparator.CausesValidation = false;
            this.panelSeparator.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelSeparator.Location = new System.Drawing.Point( 0, 0 );
            this.panelSeparator.Name = "panelSeparator";
            this.panelSeparator.Size = new System.Drawing.Size( 885, 1 );
            this.panelSeparator.TabIndex = 1;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb( ( (int)( ( (byte)( 204 ) ) ) ), ( (int)( ( (byte)( 204 ) ) ) ), ( (int)( ( (byte)( 204 ) ) ) ) );
            this.panel1.Location = new System.Drawing.Point( 0, 0 );
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size( 885, 30 );
            this.panel1.TabIndex = 3;
            // 
            // phoneNumberControl
            // 
            this.phoneNumberControl.AreaCode = string.Empty;
            this.phoneNumberControl.Location = new System.Drawing.Point( 571, 25 );
            this.phoneNumberControl.Model = ( (PatientAccess.Domain.Parties.PhoneNumber)( resources.GetObject( "phoneNumberControl.Model" ) ) );
            this.phoneNumberControl.Name = "phoneNumberControl";
            this.phoneNumberControl.PhoneNumber = string.Empty;
            this.phoneNumberControl.Size = new System.Drawing.Size( 94, 27 );
            this.phoneNumberControl.TabIndex = 7;
            this.phoneNumberControl.Validating += new CancelEventHandler( phoneNumberControl_Validating );
            // 
            // VerificationEntry
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add( this.panelControls );
            this.Controls.Add( this.lblTitle );
            this.Controls.Add( this.panelViewport );
            this.Controls.Add( this.panel1 );
            this.Name = "VerificationEntry";
            this.Size = new System.Drawing.Size( 885, 304 );
            this.Disposed += new System.EventHandler( this.VerificationEntry_Disposed );
            this.Leave += new System.EventHandler( this.VerificationEntry_Leave );
            this.Validating += new System.ComponentModel.CancelEventHandler( this.VerificationEntry_Validating );
            this.panelViewport.ResumeLayout( false );
            this.panelControls.ResumeLayout( false );
            this.panelControls.PerformLayout();
            this.ResumeLayout( false );

        }

        #endregion

        #endregion

        #region Construction and Finalization
        public VerificationEntry()
        {
            InitializeComponent();
            PopulateViewTable();
            base.EnableThemesOn( this );
            loadingModelData = true;

            blankYesNotAppFlag = new YesNotApplicableFlag();
            blankYesNotAppFlag.SetBlank();
            yesYesNotAppFlag = new YesNotApplicableFlag();
            yesYesNotAppFlag.SetYes();
            noYesNotAppFlag = new YesNotApplicableFlag();
            noYesNotAppFlag.SetNotApplicable();
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if( components != null )
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }
        #endregion

        #region Data Elements
        private IContainer components = null;

        private ComboBox cmbBenefitsVerified;
        private ComboBox cmbAuthorizationRequired;

        private DateTimePicker dtpBenefitsDate;

        private Label lblBenefits;
        private Label lblTitle;
        private Label lblInitiatedBy;
        private Label lblDate;
        private Label lblPhone;
        private Label lblPhoneExt;
        private Label label1;
        private Label label3;

        private Panel panelViewport;
        private Panel panelControls;
        private Panel panel1;
        private Panel panelSeparator;

        private VScrollBar vScrollBar;

        private MaskedEditTextBox mtbInitiatedBy;
        private MaskedEditTextBox mtbAuthorizationCompany;
        private MaskedEditTextBox mtbBenefitsDate;
        private MaskedEditTextBox mtbPromptExt;

        private Account i_Account;
        private DateTime benefitsVerifiedDate;
        private Hashtable viewTable = new Hashtable( NUM_VIEWS );
        private bool loadingModelData;
        private int insuranceMonth;
        private int insuranceDay;
        private int insuranceYear;
        private int vScrollPosn;

        // hold the original entries to determine if the user changed the values; this determines if a 
        // FUS not gets generated or not

        private YesNotApplicableFlag originalBenefitsVerifiedFlag;
        private string originalBenefitsInitiatedBy = string.Empty;
        private DateTime originalBenefitsVerifiedDate;

        private YesNotApplicableFlag originalAuthRequiredFlag;
        private string originalAuthorizationCompany = string.Empty;
        private PhoneNumber originalAuthorizationPhone;
        private string originalAuthorizationExtension = string.Empty;

        private PhoneNumber benefitsPhoneNumber;
        private string benefitsInitiatedBy;
        private string authorizationCompany;
        private string promptExt;
        private YesNotApplicableFlag benefitsVerifiedFlag;
        private YesNotApplicableFlag authorizationRequiredFlag;
        private YesNotApplicableFlag blankYesNotAppFlag;
        private YesNotApplicableFlag yesYesNotAppFlag;
        private YesNotApplicableFlag noYesNotAppFlag;

        private RuleEngine i_RuleEngine;
        private bool isSubClassOfCoverage;

        // original values

        private CommMgdCareVerifyView commMgdCareVerifyView;
        private GovernmentVerifyView governmentVerifyView;
        private MedicaidVerifyView medicaidVerifyView;
        private MedicareVerifyView medicareVerifyView;
        private SelfPayVerifyView selfPayVerifyView;
        private WorkersCompVerifyView workersCompVerifyView;
        private ControlView currentView;
        #endregion
        private PhoneNumberControl phoneNumberControl;

        #region Constants
        private static int NUM_VIEWS = 7;    // Number of buckets in the form view hash table
        #endregion

    }
}
