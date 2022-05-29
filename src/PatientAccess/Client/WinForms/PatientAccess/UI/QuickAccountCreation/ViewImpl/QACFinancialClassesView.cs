using System;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.PersistenceCommon;
using Extensions.UI.Winforms;
using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI.FinancialCounselingViews;

namespace PatientAccess.UI.QuickAccountCreation.ViewImpl
{
    [Serializable]
    public partial class QACFinancialClassesView : ControlView
    {
        #region Events
        #endregion

        #region Event Handlers
        private void cmboFinancialClass_Validating( object sender, CancelEventArgs e )
        {
            FinancialClassesSelectedIndexChanged( sender, e );
            RuleEngine.GetInstance().EvaluateRule( typeof( FinancialClassRequired ), Model_Account );
            RuleEngine.GetInstance().EvaluateRule( typeof( FinancialClassPreferred ), Model_Account );
        }

        private void QACFinancialClassesView_Load( object sender, EventArgs e )
        {
            cmboFinancialClass.Enabled = false;
        }

        private void FinancialClassesSelectedIndexChanged( object sender, EventArgs e )
        {
            UIColors.SetNormalBgColor( cmboFinancialClass );
            var comboBox = sender as ComboBox;

            if ( comboBox.SelectedIndex > 0 )
            {
                var financialClass = comboBox.SelectedItem as FinancialClass;

                if ( ( Model_Account.FinancialClass == null ||
                    FinancialCouncelingService.IsUninsured( Model_Account.Facility.Oid, Model_Account.FinancialClass ) ) &&
                    !FinancialCouncelingService.IsUninsured( Model_Account.Facility.Oid, financialClass ) )
                {
                    //based on UC071 12a.
                    Model_Account.TotalCurrentAmtDue = 0m; //is estimated account due
                    //Model_Account.TotalPaid has no change
                    Model_Account.NumberOfMonthlyPayments = 0;
                    Model_Account.MonthlyPayment = 0; // auto set by NumberOfMonthlyPayments = 0;
                    //Model_Account.TotalCurrentAmtDue // same.
                    Model_Account.Insurance.HasNoLiability = false;
                    //monthly due date has no change.
                    //community resource list has no change.
                }

                else if ( ( Model_Account.FinancialClass == null ||
                    !FinancialCouncelingService.IsUninsured( Model_Account.Facility.Oid, Model_Account.FinancialClass ) ) &&
                    FinancialCouncelingService.IsUninsured( Model_Account.Facility.Oid, financialClass ) )
                {   //based on UC071 12b.
                    Model_Coverage.Deductible = 0m;
                    Model_Coverage.CoPay = 0m;
                    Model_Account.MonthlyPayment = 0m;
                    Model_Account.MonthlyPayment = 0; // auto set by NumberOfMonthlyPayments = 0;
                    Model_Account.TotalCurrentAmtDue = 0m;
                    Model_Coverage.NoLiability = false;
                    //monthly due date has no change.                    
                }
            }


            Model_Account.FinancialClass = (FinancialClass)cmboFinancialClass.SelectedItem;
            RunRules();

        }

        private void FinancialClassPreferredEventHandler( object sender, EventArgs e )
        {
            if ( cmboFinancialClass.Enabled )
            {
                UIColors.SetPreferredBgColor( cmboFinancialClass );
            }
        }

        private void FinancialClassRequiredEventHandler( object sender, EventArgs e )
        {
            if ( cmboFinancialClass.Enabled )
            {
                UIColors.SetRequiredBgColor( cmboFinancialClass );
            }
        }


        #endregion

        #region Methods

        public void ResetFinancialClass()
        {
            Model_Account.FinancialClass = new FinancialClass( 296L, PersistentModel.NEW_VERSION, "MED (MSE) SCREEN EXM", MED_MSE_SCREEN_EXM );
            cmboFinancialClass.Items.Add( Model_Account.FinancialClass );
            cmboFinancialClass.SelectedItem = Model_Account.FinancialClass;
            cmboFinancialClass.Enabled = false;
        } 

        public void RunRules()
        {
            UIColors.SetNormalBgColor( cmboFinancialClass );

            RuleEngine.GetInstance().EvaluateRule( typeof( FinancialClassRequired ), Model_Account );
            RuleEngine.GetInstance().EvaluateRule( typeof( FinancialClassPreferred ), Model_Account );
        }

        public void SetSelectedFinancialClass( FinancialClass financialClass )
        {
            cmboFinancialClass.SelectedItem = financialClass;
        }

        public override void UpdateView()
        {
            RegisterRulesEvents();

            if ( Model_Account != null )
            {
                // if there is a secondary coverage wipe out any HIC number
               
                ResetMBIForSecondaryIns(Model_Account);
            }


            if ( Model_Coverage == null )
            {
                return;
            }

            // ComboBox should only be populated if the Coverage is primary coverage.
            if ( Model_Coverage.CoverageOrder.Oid == CoverageOrder.PRIMARY_OID )
            {
                PopulateFinancialClass();
            }

            RunRules();
        }

        #endregion

        #region Properties

        public Coverage Model_Coverage
        {
            private get
            {
                return (Coverage)Model;
            }
            set
            {
                Model = value;
            }
        }

        public Account Model_Account
        {
            private get
            {
                return i_account;
            }
            set
            {
                i_account = value;
            }
        }

        #endregion

        #region Private Methods
       
        private static void ResetMBIForSecondaryIns(Account anAccount)
        {
            // if the finClass is blank or anything other than SIGNED_OVER_MEDICARE_FINANCIAL_CLASS_CODE (84 or 87)
            // blank out the HIC on the secondary insurance if present
            if (anAccount.Insurance.SecondaryCoverage != null &&  // there is a seconary coverage
                    (anAccount.FinancialClass != null &&      // there is a finclass other than 84 or 87
                     !string.IsNullOrEmpty(anAccount.FinancialClass.Code) &&
                     !anAccount.FinancialClass.IsSignedOverMedicare())
                    ||
                    (anAccount.FinancialClass == null ||      // there is no finclass
                     anAccount.FinancialClass.Code == null ||
                     (anAccount.FinancialClass.Code != null &&
                      anAccount.FinancialClass.Code.Equals(string.Empty))
                    )

                )
            {
                var secondaryCoverage = anAccount.Insurance.SecondaryCoverage;
                if (secondaryCoverage is CoverageForCommercialOther)
                {
                    var comCoverage = secondaryCoverage as CoverageForCommercialOther;
                    comCoverage.MBINumber = string.Empty;
                }
            }
        }

        private void PopulateFinancialClass()
        { 
            if ( Model_Account.FinancialClass != null )
            {
                cmboFinancialClass.SelectedItem = Model_Account.FinancialClass;
            } 
        }

        private void RegisterRulesEvents()
        {
            if ( !i_Registered )
            {
                RuleEngine.GetInstance().RegisterEvent( typeof( FinancialClassPreferred ), Model_Account, FinancialClassPreferredEventHandler );
                RuleEngine.GetInstance().RegisterEvent( typeof( FinancialClassRequired ), Model_Account, FinancialClassRequiredEventHandler );
            }
        }

        private void UnRegisterRulesEvents()
        {
            i_Registered = false;

            RuleEngine.GetInstance().UnregisterEvent( typeof( FinancialClassPreferred ), Model_Account, FinancialClassPreferredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( FinancialClassRequired ), Model_Account, FinancialClassRequiredEventHandler );
        }

        #endregion

        #region Private Properties

        #endregion

        #region Construction and Finalization

        public QACFinancialClassesView()
        {
            InitializeComponent();
            EnableThemesOn( this );
        }

        #endregion

        #region Data Elements

        private Account i_account;

        private bool i_Registered;

        #endregion

        #region Constants

        private const string MED_MSE_SCREEN_EXM = "37";

        #endregion
    }
}
