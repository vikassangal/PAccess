using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Extensions.UI.Builder;
using PatientAccess.Domain;
using PatientAccess.Domain.PAIWalkinOutpatientCreation;
using PatientAccess.Domain.QuickAccountCreation;
using PatientAccess.Domain.ShortRegistration;
using PatientAccess.UI.PAIWalkinAccountCreation.ViewImpl;
using PatientAccess.UI.QuickAccountCreation.ViewImpl;
using PatientAccess.UI.ShortRegistration;

namespace PatientAccess.UI.CommonControls
{
    /// <summary>
    /// This class was refactored out of a the RequireFieldsSummaryView class. 
    /// This currently does not present the optimal implementation for the MVP pattern. 
    /// </summary>
    public class RequiredFieldsSummaryPresenter : IRequiredFieldsSummaryPresenter
    {
        public event EventHandler TabSelectedEvent;

        #region Methods

        public void ShowViewAsDialog( IWin32Window owner )
        {
            RequiredFieldsSummaryView.ShowAsModalDialog( owner );
        }

        public bool IsTabEventRegistered( EventHandler eventHandler )
        {

            if ( TabSelectedEvent == null )
            {
                return false;
            }

            IEnumerable<Delegate> delegates = TabSelectedEvent.GetInvocationList();

            bool isTabEventRegistered = false;

            foreach ( Delegate d in delegates )
            {
                if ( d.Target.GetType() == eventHandler.Target.GetType() && d.Method.Name == eventHandler.Method.Name )
                {
                    isTabEventRegistered = true;
                }
            }

            return isTabEventRegistered;
        }

        public void SetActionItems( IEnumerable<CompositeAction> actionItems )
        {
            BindActionItems( actionItems );
        }

        private void RaiseNonStaffPhysicianTabSelectedEvent( int nonStaffPhysicianType )
        {
            TabSelectedEvent( this, new LooseArgs( nonStaffPhysicianType ) );
        }

        private void BindActionItems( IEnumerable<CompositeAction> actionItems )
        {
            List<RequiredFieldItem> itemInLists = SetupItemsToBind( actionItems );

            RequiredFieldsSummaryView.Update( itemInLists );
        }

        private static List<RequiredFieldItem> SetupItemsToBind( IEnumerable<CompositeAction> actionItems )
        {
            var itemInLists = new List<RequiredFieldItem>();

            itemInLists.AddRange( from ca in actionItems
                                  from LeafAction la in ca.Constituents
                                  where la.Severity == REQUIRED
                                  select new RequiredFieldItem( ca.Description, la.Description ) );

            itemInLists.Sort();

            if ( itemInLists.Count < 10 )
            {
                while ( itemInLists.Count < 10 )
                {
                    itemInLists.Add( new RequiredFieldItem( String.Empty, String.Empty ) );
                }
            }

            return itemInLists;
        }

        public void RequiredFieldSelected( string tab )
        {
            if ( TabSelectedEvent != null )
            {
                tab = tab.Trim();
                RequiredFieldsSummaryView.Hide();

                if ( Model != null &&
                     Model.Activity != null && 
                     ( Model.Activity.GetType() == typeof( TransferOutToInActivity ) ||
                     Model.Activity.GetType() == typeof( TransferERToOutpatientActivity ) ||
                     Model.Activity.GetType() == typeof( TransferOutpatientToERActivity ) ))
                {
                    switch ( tab )
                    {
                        case MISSING_TRANSFER_TO_FIELDS:
                            TabSelectedEvent( this, new LooseArgs( SCREEN_0 ) );
                            break;
                        case MISSING_PHYSICIANS_FIELDS:
                            TabSelectedEvent( this, new LooseArgs( SCREEN_1 ) );
                            break;
                        case MISSING_INSURANCE_FIELDS:
                            TabSelectedEvent( this, new LooseArgs( SCREEN_2 ) );
                            break;
                        case MISSING_PRIMARY_INSURED_FIELDS:
                            TabSelectedEvent( this, new LooseArgs( SCREEN_3 ) );
                            break;
                        case MISSING_SECONDARY_INSURED_FIELDS:
                            TabSelectedEvent( this, new LooseArgs( SCREEN_4 ) );
                            break;
                        case MISSING_PRIMARY_INS_PAYOR_FIELDS:
                            TabSelectedEvent( this, new LooseArgs( SCREEN_5 ) );
                            break;
                        case MISSING_SECONDARY_INS_PAYOR_FIELDS:
                            TabSelectedEvent( this, new LooseArgs( SCREEN_6 ) );
                            break;

                        // Non-Staff Physician View
                        case MISSING_REFERRING_PHYSICIAN_NPI:
                            TabSelectedEvent( this, new LooseArgs( SCREEN_7 ) );
                            break;
                        case MISSING_ATTENDING_PHYSICIAN_NPI:
                            TabSelectedEvent( this, new LooseArgs( SCREEN_8 ) );
                            break;
                        case MISSING_ADMITTING_PHYSICIAN_NPI:
                            TabSelectedEvent( this, new LooseArgs( SCREEN_9 ) );
                            break;
                        case MISSING_OPERATING_PHYSICIAN_NPI:
                            TabSelectedEvent( this, new LooseArgs( SCREEN_10 ) );
                            break;
                        case MISSING_PCP_PHYSICIAN_NPI:
                            TabSelectedEvent( this, new LooseArgs( SCREEN_11 ) );
                            break;
                    }
                }
                else if ( Model != null && Model.Activity != null &&
                          ( Model.Activity.GetType() == typeof( ShortRegistrationActivity )  ||
                            Model.Activity.GetType() == typeof( ShortPreRegistrationActivity )  ||
                            Model.Activity.GetType() == typeof( ShortMaintenanceActivity ) )
                        )
                {
                    switch ( tab )
                    {
                        case MISSING_PRIMARY_INS_AUTH_FIELDS:
                            TabSelectedEvent( this, new LooseArgs( ShortAccountView.ShortRegistrationScreenIndexes.PRIMARYAUTHORIZATION ) );
                            break;
                        case MISSING_SECONDARY_INS_AUTH_FIELDS:
                            TabSelectedEvent( this, new LooseArgs( ShortAccountView.ShortRegistrationScreenIndexes.SECONDARYAUTHORIZATION ) );
                            break;
                        case MISSING_BILLING_FIELDS:
                            TabSelectedEvent( this, new LooseArgs( ShortAccountView.ShortRegistrationScreenIndexes.SHORTBILLING ) );
                            break;
                        case MISSING_DIAGNOSIS_CLINICAL_FIELDS:
                            TabSelectedEvent( this, new LooseArgs( ShortAccountView.ShortRegistrationScreenIndexes.SHORTDIAGNOSIS ) );
                            break;
                        case MISSING_DEMOGRAPHICS_FIELDS:
                            TabSelectedEvent( this, new LooseArgs( ShortAccountView.ShortRegistrationScreenIndexes.SHORTDEMOGRAPHICS ) );
                            break;
                        case MISSING_GUARANTOR_FIELDS:
                            TabSelectedEvent( this, new LooseArgs( ShortAccountView.ShortRegistrationScreenIndexes.SHORTGUARANTOR ) );
                            break;
                        case MISSING_INSURANCE_FIELDS:
                            TabSelectedEvent( this, new LooseArgs( ShortAccountView.ShortRegistrationScreenIndexes.INSURANCE ) );
                            break;
                        case MISSING_PRIMARY_INSURED_FIELDS:
                            TabSelectedEvent( this, new LooseArgs( ShortAccountView.ShortRegistrationScreenIndexes.INSURED ) );
                            break;
                        case MISSING_SECONDARY_INSURED_FIELDS:
                            TabSelectedEvent( this, new LooseArgs( ShortAccountView.ShortRegistrationScreenIndexes.SECONDARYINSURED ) );
                            break;
                        case MISSING_PRIMARY_INS_PAYOR_FIELDS:
                            TabSelectedEvent( this, new LooseArgs( ShortAccountView.ShortRegistrationScreenIndexes.PAYORDETAILS ) );
                            break;
                        case MISSING_SECONDARY_INS_PAYOR_FIELDS:
                            TabSelectedEvent( this, new LooseArgs( ShortAccountView.ShortRegistrationScreenIndexes.SECONDARYPAYORDETAILS ) );
                            break;
                        case MISSING_PHYSICIANS_FIELDS:
                            TabSelectedEvent( this, new LooseArgs( ShortAccountView.ShortRegistrationScreenIndexes.SHORTDIAGNOSIS ) );
                            break;
                        case MISSING_REGULATORY_DOCUMENTS_FIELDS:
                            TabSelectedEvent( this, new LooseArgs( ShortAccountView.ShortRegistrationScreenIndexes.SHORTREGULATORYANDDOCUMENTS ) );
                            break;
                        case MISSING_PRIMARY_INS_VERIFICATION_FIELDS:
                            TabSelectedEvent( this, new LooseArgs( ShortAccountView.ShortRegistrationScreenIndexes.PRIMARYINSVERIFICATION ) );
                            break;
                        case MISSING_SEC_INS_VERIFICATION_FIELDS:
                            TabSelectedEvent( this, new LooseArgs( ShortAccountView.ShortRegistrationScreenIndexes.SECONDARYINSVERIFICATION ) );
                            break;
                        case MISSING_PAYMENT_FIELDS:
                            TabSelectedEvent(this, new LooseArgs(ShortAccountView.ShortRegistrationScreenIndexes.PAYMENT));
                            break;

                        // Non-Staff Physician View
                        case MISSING_REFERRING_PHYSICIAN_NPI:
                            RaiseNonStaffPhysicianTabSelectedEvent( ( int )ShortAccountView.ShortRegistrationScreenIndexes.REFERRINGNONSTAFFPHYSICIAN );
                            break;
                        case MISSING_ATTENDING_PHYSICIAN_NPI:
                            RaiseNonStaffPhysicianTabSelectedEvent( ( int )ShortAccountView.ShortRegistrationScreenIndexes.ATTENDINGNONSTAFFPHYSICIAN );
                            break;
                        case MISSING_ADMITTING_PHYSICIAN_NPI:
                            RaiseNonStaffPhysicianTabSelectedEvent( ( int )ShortAccountView.ShortRegistrationScreenIndexes.ADMITTINGNONSTAFFPHYSICIAN );
                            break;
                        case MISSING_OPERATING_PHYSICIAN_NPI:
                            RaiseNonStaffPhysicianTabSelectedEvent( ( int )ShortAccountView.ShortRegistrationScreenIndexes.OPERATINGNONSTAFFPHYSICIAN );
                            break;
                        case MISSING_PCP_PHYSICIAN_NPI:
                            RaiseNonStaffPhysicianTabSelectedEvent( ( int )ShortAccountView.ShortRegistrationScreenIndexes.PRIMARYCARENONSTAFFPHYSICIAN );
                            break;
                    
                    }
                }
                else if (Model != null && Model.Activity != null &&
                    (Model.Activity.GetType() == typeof(QuickAccountCreationActivity) ||
                      Model.Activity.GetType() == typeof(QuickAccountMaintenanceActivity) 
                     ))
                {
                    switch (tab)
                    {
                        case MISSING_QUICKACCOUNTCREATION_FIELDS:
                            TabSelectedEvent(this,
                                             new LooseArgs(QuickAccountView.QuickAccountCreationScreenIndexes.QUICKACCOUNTCREATION));
                            break;

                        case MISSING_PRIMARY_INS_AUTH_FIELDS:
                            TabSelectedEvent(this,
                                             new LooseArgs(
                                                 QuickAccountView.QuickAccountCreationScreenIndexes.PRIMARYAUTHORIZATION));
                            break;

                        case MISSING_PRIMARY_INSURED_FIELDS:
                            TabSelectedEvent(this, new LooseArgs(QuickAccountView.QuickAccountCreationScreenIndexes.INSURED));
                            break;

                        case MISSING_PRIMARY_INS_PAYOR_FIELDS:
                            TabSelectedEvent(this,
                                             new LooseArgs(QuickAccountView.QuickAccountCreationScreenIndexes.PAYORDETAILS));
                            break;

                        case MISSING_PRIMARY_INS_VERIFICATION_FIELDS:
                            TabSelectedEvent(this,
                                             new LooseArgs(
                                                 QuickAccountView.QuickAccountCreationScreenIndexes.PRIMARYINSVERIFICATION));
                            break;

                            // Non-Staff Physician View
                        case MISSING_REFERRING_PHYSICIAN_NPI:
                            RaiseNonStaffPhysicianTabSelectedEvent(
                                (int) QuickAccountView.QuickAccountCreationScreenIndexes.REFERRINGNONSTAFFPHYSICIAN);
                            break;

                        case MISSING_ADMITTING_PHYSICIAN_NPI:
                            RaiseNonStaffPhysicianTabSelectedEvent(
                                (int) QuickAccountView.QuickAccountCreationScreenIndexes.ADMITTINGNONSTAFFPHYSICIAN);
                            break;
                    }
                }
                else if (Model != null && Model.Activity != null &&
                         (Model.Activity.GetType() == typeof ( PAIWalkinOutpatientCreationActivity )))
                {
                    switch (tab)
                    {
                        case MISSING_QUICKACCOUNTCREATION_FIELDS:
                            TabSelectedEvent(this,
                                new LooseArgs(PAIWalkinAccountView.PAIWalkinAccountCreationScreenIndexes.PAIWALKINACCOUNTCREATION));
                            break;

                        case MISSING_PRIMARY_INS_AUTH_FIELDS:
                            TabSelectedEvent(this,
                                new LooseArgs(
                                    PAIWalkinAccountView.PAIWalkinAccountCreationScreenIndexes.PRIMARYAUTHORIZATION));
                            break;

                        case MISSING_PRIMARY_INSURED_FIELDS:
                            TabSelectedEvent(this,
                                new LooseArgs(PAIWalkinAccountView.PAIWalkinAccountCreationScreenIndexes.INSURED));
                            break;

                        case MISSING_PRIMARY_INS_PAYOR_FIELDS:
                            TabSelectedEvent(this,
                                new LooseArgs(PAIWalkinAccountView.PAIWalkinAccountCreationScreenIndexes.PAYORDETAILS));
                            break;

                        case MISSING_PRIMARY_INS_VERIFICATION_FIELDS:
                            TabSelectedEvent(this,
                                new LooseArgs(
                                    PAIWalkinAccountView.PAIWalkinAccountCreationScreenIndexes.PRIMARYINSVERIFICATION));
                            break;

                            // Non-Staff Physician View
                        case MISSING_REFERRING_PHYSICIAN_NPI:
                            RaiseNonStaffPhysicianTabSelectedEvent(
                                (int)PAIWalkinAccountView.PAIWalkinAccountCreationScreenIndexes.REFERRINGNONSTAFFPHYSICIAN);
                            break;

                        case MISSING_ADMITTING_PHYSICIAN_NPI:
                            RaiseNonStaffPhysicianTabSelectedEvent(
                                (int)PAIWalkinAccountView.PAIWalkinAccountCreationScreenIndexes.ADMITTINGNONSTAFFPHYSICIAN);
                            break;
                    }
                }
                else
                {
                    switch ( tab )
                    {
                        case MISSING_PRIMARY_INS_AUTH_FIELDS:
                            TabSelectedEvent( this, new LooseArgs( AccountView.ScreenIndexes.PRIMARYAUTHORIZATION ) );
                            break;
                        case MISSING_SECONDARY_INS_AUTH_FIELDS:
                            TabSelectedEvent( this, new LooseArgs( AccountView.ScreenIndexes.SECONDARYAUTHORIZATION ) );
                            break;
                        case MISSING_BILLING_FIELDS:
                            TabSelectedEvent( this, new LooseArgs( AccountView.ScreenIndexes.BILLING ) );
                            break;
                        case MISSING_CLINICAL_FIELDS:
                            TabSelectedEvent( this, new LooseArgs( ( int )AccountView.ScreenIndexes.CLINICAL ) );
                            break;
                        case MISSING_CONTACT_DIAGNOSIS_FIELDS:
                            TabSelectedEvent( this, new LooseArgs( 1 ) );
                            break;
                        case MISSING_CONTACTS_FIELDS:
                            TabSelectedEvent( this, new LooseArgs( ( int )AccountView.ScreenIndexes.CONTACTS ) );
                            break;
                        case MISSING_DEMOGRAPHICS_FIELDS:
                            TabSelectedEvent( this, new LooseArgs( ( int )AccountView.ScreenIndexes.DEMOGRAPHICS ) );
                            break;
                        case MISSING_DIAGNOSIS_FIELDS:
                            TabSelectedEvent( this, new LooseArgs( ( int )AccountView.ScreenIndexes.DIAGNOSIS ) );
                            break;
                        case MISSING_EMPLOYMENT_FIELDS:
                            TabSelectedEvent( this, new LooseArgs( ( int )AccountView.ScreenIndexes.EMPLOYMENT ) );
                            break;
                        case MISSING_GUARANTOR_FIELDS:
                            TabSelectedEvent( this, new LooseArgs( ( int )AccountView.ScreenIndexes.GUARANTOR ) );
                            break;
                        case MISSING_INSURANCE_FIELDS:
                            TabSelectedEvent( this, new LooseArgs( ( int )AccountView.ScreenIndexes.INSURANCE ) );
                            break;
                        case MISSING_PRIMARY_INSURED_FIELDS:
                            TabSelectedEvent( this, new LooseArgs( ( int )AccountView.ScreenIndexes.INSURED ) );
                            break;
                        case MISSING_SECONDARY_INSURED_FIELDS:
                            TabSelectedEvent( this, new LooseArgs( ( int )AccountView.ScreenIndexes.SECONDARYINSURED ) );
                            break;
                        case MISSING_PRIMARY_INS_PAYOR_FIELDS:
                            TabSelectedEvent( this, new LooseArgs( ( int )AccountView.ScreenIndexes.PAYORDETAILS ) );
                            break;
                        case MISSING_SECONDARY_INS_PAYOR_FIELDS:
                            TabSelectedEvent( this, new LooseArgs( ( int )AccountView.ScreenIndexes.SECONDARYPAYORDETAILS ) );
                            break;
                        case MISSING_PHYSICIANS_FIELDS:
                            TabSelectedEvent( this, new LooseArgs( ( int )AccountView.ScreenIndexes.CLINICAL ) );
                            break;
                        case MISSING_REGULATORY_FIELDS:
                            TabSelectedEvent( this, new LooseArgs( ( int )AccountView.ScreenIndexes.REGULATORY ) );
                            break;
                        case MISSING_TRANSFER_TO_FIELDS:
                            TabSelectedEvent( this, new LooseArgs( 0 ) );
                            break;
                        case MISSING_PRIMARY_INS_VERIFICATION_FIELDS:
                            TabSelectedEvent( this, new LooseArgs( ( int )AccountView.ScreenIndexes.PRIMARYINSVERIFICATION ) );
                            break;
                        case MISSING_SEC_INS_VERIFICATION_FIELDS:
                            TabSelectedEvent( this, new LooseArgs( ( int )AccountView.ScreenIndexes.SECONDARYINSVERIFICATION ) );
                            break;
                        case MISSING_PAYMENT_FIELDS:
                            TabSelectedEvent(this, new LooseArgs((int)AccountView.ScreenIndexes.PAYMENT));
                            break;
                        // Non-Staff Physician View
                        case MISSING_REFERRING_PHYSICIAN_NPI:
                            RaiseNonStaffPhysicianTabSelectedEvent( ( int )AccountView.ScreenIndexes.REFERRINGNONSTAFFPHYSICIAN );
                            break;
                        case MISSING_ATTENDING_PHYSICIAN_NPI:
                            RaiseNonStaffPhysicianTabSelectedEvent( ( int )AccountView.ScreenIndexes.ATTENDINGNONSTAFFPHYSICIAN );
                            break;
                        case MISSING_ADMITTING_PHYSICIAN_NPI:
                            RaiseNonStaffPhysicianTabSelectedEvent( ( int )AccountView.ScreenIndexes.ADMITTINGNONSTAFFPHYSICIAN );
                            break;
                        case MISSING_OPERATING_PHYSICIAN_NPI:
                            RaiseNonStaffPhysicianTabSelectedEvent( ( int )AccountView.ScreenIndexes.OPERATINGNONSTAFFPHYSICIAN );
                            break;
                        case MISSING_PCP_PHYSICIAN_NPI:
                            RaiseNonStaffPhysicianTabSelectedEvent( ( int )AccountView.ScreenIndexes.PRIMARYCARENONSTAFFPHYSICIAN );
                            break;
                    }
                }
            }
        }

        #endregion

        #region Properties

        private IRequiredFieldsSummaryView RequiredFieldsSummaryView { get; set; }

        public Account Model { private get; set; }

        public string Header
        {
            set { RequiredFieldsSummaryView.HeaderText = value; }
        }

        #endregion

        #region Construction and Finalization

        public RequiredFieldsSummaryPresenter( IRequiredFieldsSummaryView view, string title )
        {
            RequiredFieldsSummaryView = view;
            view.Presenter = this;
            RequiredFieldsSummaryView.Text = title;
        }

        public RequiredFieldsSummaryPresenter( IRequiredFieldsSummaryView view, IEnumerable<CompositeAction> actionItems )
            : this( view, WARNING_FOR_REQUIRED_FIELDS )
        {
            BindActionItems( actionItems );
        }

        #endregion

        #region Constants

        private const int REQUIRED = 4;
        private const int SCREEN_0 = 0;
        private const int SCREEN_1 = 1;
        private const int SCREEN_2 = 2;
        private const int SCREEN_3 = 3;
        private const int SCREEN_4 = 4;
        private const int SCREEN_5 = 5;
        private const int SCREEN_6 = 6;
        private const int SCREEN_7 = 7;
        private const int SCREEN_8 = 8;
        private const int SCREEN_9 = 9;
        private const int SCREEN_10 = 10;
        private const int SCREEN_11 = 11;

        private const string WARNING_FOR_REQUIRED_FIELDS = "Warning for Required Fields";

        public const string REQUIRED_FIELDS_HEADER =
            "The following required fields must be completed to finish this activity. " +
            "Double-click a row in the table to complete the required field or click OK to dismiss this message.";

        private const string
            MISSING_DEMOGRAPHICS_FIELDS = "Complete missing fields on the Demographics form",
            MISSING_EMPLOYMENT_FIELDS = "Complete missing fields on the Employment form",
            MISSING_DIAGNOSIS_FIELDS = "Complete missing fields on the Diagnosis form",
            MISSING_CLINICAL_FIELDS = "Complete missing fields on the Clinical form",
            MISSING_INSURANCE_FIELDS = "Complete missing fields on the Insurance form",
            MISSING_PRIMARY_INS_PAYOR_FIELDS = "Complete missing fields on the Payor Details form for Primary Insurance",
            MISSING_SECONDARY_INS_PAYOR_FIELDS =
                "Complete missing fields on the Payor Details form for Secondary Insurance",
            MISSING_PRIMARY_INSURED_FIELDS = "Complete missing fields on the Insured form for Primary Insurance",
            MISSING_SECONDARY_INSURED_FIELDS = "Complete missing fields on the Insured form for Secondary Insurance",
            MISSING_PRIMARY_INS_VERIFICATION_FIELDS =
                "Complete missing fields on the Verification form for Primary Insurance",
            MISSING_SEC_INS_VERIFICATION_FIELDS =
                "Complete missing fields on the Verification form for Secondary Insurance",
            MISSING_PRIMARY_INS_AUTH_FIELDS = "Complete missing fields on the Authorization form for Primary Insurance",
            MISSING_SECONDARY_INS_AUTH_FIELDS =
                "Complete missing fields on the Authorization form for Secondary Insurance",
            MISSING_GUARANTOR_FIELDS = "Complete missing fields on the Guarantor form",
            MISSING_BILLING_FIELDS = "Complete missing fields on the Billing form",
            MISSING_CONTACTS_FIELDS = "Complete missing fields on the Contacts form",
            MISSING_REGULATORY_FIELDS = "Complete missing fields on the Regulatory form",
            MISSING_CONTACT_DIAGNOSIS_FIELDS = "Complete missing fields on the Contact & Diagnosis form",
            MISSING_TRANSFER_TO_FIELDS = "Complete missing fields on the Transfer To form",
            MISSING_REFERRING_PHYSICIAN_NPI = "Complete missing NPI details for Referring NonStaff Physician",
            MISSING_ADMITTING_PHYSICIAN_NPI = "Complete missing NPI details for Admitting NonStaff Physician",
            MISSING_ATTENDING_PHYSICIAN_NPI = "Complete missing NPI details for Attending NonStaff Physician",
            MISSING_OPERATING_PHYSICIAN_NPI = "Complete missing NPI details for Operating NonStaff Physician",
            MISSING_PCP_PHYSICIAN_NPI = "Complete missing NPI details for Primary Care Non-staff Physician",
            MISSING_PHYSICIANS_FIELDS = "Complete missing fields on the Physicians form",

            // Short Registration specific tabs
            MISSING_DIAGNOSIS_CLINICAL_FIELDS = "Complete missing fields on the Diagnosis/Clinical form",
            MISSING_REGULATORY_DOCUMENTS_FIELDS = "Complete missing fields on the Regulatory/Documents form",

            //Quick Account Creation Specific
            MISSING_QUICKACCOUNTCREATION_FIELDS = "Complete missing fields on the Quick Account Creation form",
            MISSING_PAYMENT_FIELDS = "Complete missing fields on the Payment form";

        #endregion
    }
}