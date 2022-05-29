using System;
using System.Collections;
using System.Configuration;
using Extensions.SecurityService.Domain;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.PAIWalkinOutpatientCreation;
using PatientAccess.Domain.QuickAccountCreation;
using PatientAccess.Domain.ShortRegistration;
using PatientAccess.Domain.UCCRegistration;

namespace PatientAccess.Persistence.Security
{
	/// <summary>
	/// Summary description for PatientAccessApplication.
	/// </summary>
	//TODO: Create XML summary comment for PatientAccessApplication
    [Serializable]
    public class PatientAccessApplication : Application
    {
        #region Event Handlers
        #endregion

        #region Methods
        #endregion

        #region Properties
        
        public string LegacyApplicationName
        {
            get
            {
                return i_LegacyApplicationName;
            }
            private set
            {
                i_LegacyApplicationName = value;
            }
        }
        #endregion

        #region Private Methods

        private void BuildSystemAdminPrivileges( Role systemAdmin )
        {
            systemAdmin.AddPrivilege( new Privilege( Privilege.Actions.View, typeof( Announcement ) ) );
            systemAdmin.AddPrivilege( new Privilege( Privilege.Actions.Add, typeof( Announcement ) ) );
            systemAdmin.AddPrivilege( new Privilege( Privilege.Actions.Edit, typeof( Announcement ) ) );

            this.BuildFileMenuViewPrivileges( systemAdmin );
            this.BuildEditMenuViewPrivileges( systemAdmin );
            this.BuildHelpMenuViewPrivileges( systemAdmin );
            
        }

        private void BuildRegistrationAdministratorRivileges( Role registrationAdministrator )
        {
            registrationAdministrator.AddPrivilege( new Privilege( Privilege.Actions.View, typeof( Announcement ) ) );
            registrationAdministrator.AddPrivilege( new Privilege( Privilege.Actions.Add, typeof( Announcement ) ) );
            registrationAdministrator.AddPrivilege( new Privilege( Privilege.Actions.Edit, typeof( Announcement ) ) );

            registrationAdministrator.AddPrivilege( new Privilege( Privilege.Actions.Edit, typeof( MonthlyPaymentOverride ) ) );
            registrationAdministrator.AddPrivilege( new Privilege( Privilege.Actions.Edit, typeof( Payment ) ) );
            registrationAdministrator.AddPrivilege( new Privilege( Privilege.Actions.Edit, typeof( Liability ) ) );

            this.BuildFileMenuViewPrivileges( registrationAdministrator );
            this.BuildEditMenuViewPrivileges( registrationAdministrator );
            this.BuildRegisterMenuViewPrivileges( registrationAdministrator );
            this.BuildDischargeMenuViewPrivileges( registrationAdministrator );
            this.BuildTransferMenuViewPrivileges( registrationAdministrator );
            this.BuildWorklistMenuViewPrivileges( registrationAdministrator );
            this.BuildCensusMenuViewPrivileges( registrationAdministrator );
            this.BuildReportsMenuViewPrivileges( registrationAdministrator );
            this.BuildHelpMenuViewPrivileges( registrationAdministrator );

            this.BuilEditPatientAccoutPrivileges( registrationAdministrator );
            this.BuildActivatePreregisteredAccountPrivileges( registrationAdministrator );
            this.BuildAdminMenuViewPrivileges(registrationAdministrator  );
        }

        private void BuildRegistrationUserPrivileges( Role registrationUser )
        {
            registrationUser.AddPrivilege( new Privilege( Privilege.Actions.View, typeof( Announcement ) ) );
            registrationUser.AddPrivilege( new Privilege( Privilege.Actions.Edit, typeof( Payment ) ) );
            registrationUser.AddPrivilege( new Privilege( Privilege.Actions.Edit, typeof( Liability ) ) );

            this.BuildFileMenuViewPrivileges( registrationUser );
            this.BuildEditMenuViewPrivileges( registrationUser );
            this.BuildRegisterMenuViewPrivileges( registrationUser );
            this.BuildDischargeMenuViewPrivileges( registrationUser );

            //Build TransferMenu ViewPrivileges 
            //(SR - 44648 - gives previlege for Registration user to 
            // perform TransferInToOut):
            registrationUser.AddPrivilege( new Privilege( Privilege.Actions.View, new TransferActivity().Description ) );
            registrationUser.AddPrivilege( new Privilege( Privilege.Actions.View, new TransferOutToInActivity().Description ) );
            registrationUser.AddPrivilege( new Privilege( Privilege.Actions.View, new TransferBedSwapActivity().Description ) );
            registrationUser.AddPrivilege( new Privilege( Privilege.Actions.View, new TransferInToOutActivity().Description ) );
            registrationUser.AddPrivilege( new Privilege( Privilege.Actions.View, new TransferERToOutpatientActivity().Description ) );
            registrationUser.AddPrivilege( new Privilege( Privilege.Actions.View, new TransferOutpatientToERActivity().Description ) );


            this.BuildWorklistMenuViewPrivileges( registrationUser );
            this.BuildCensusMenuViewPrivileges( registrationUser );
            this.BuildReportsMenuViewPrivileges( registrationUser );
            this.BuildHelpMenuViewPrivileges( registrationUser );

            this.BuilEditPatientAccoutPrivileges( registrationUser );
            this.BuildActivatePreregisteredAccountPrivileges( registrationUser );
        }

        private void BuildFinancialUserPrivileges( Role financialUser )
        {
            financialUser.AddPrivilege( new Privilege( Privilege.Actions.View, typeof( Announcement ) ) );
            financialUser.AddPrivilege( new Privilege( Privilege.Actions.Edit, typeof( Payment ) ) );
            financialUser.AddPrivilege( new Privilege( Privilege.Actions.Edit, typeof( Liability ) ) );
            
            this.BuildFileMenuViewPrivileges( financialUser );
            this.BuildEditMenuViewPrivileges( financialUser );
            this.BuildRegisterMenuViewPrivileges( financialUser );
            this.BuildDischargeMenuViewPrivileges( financialUser );
            this.BuildTransferMenuViewPrivileges( financialUser );
            this.BuildWorklistMenuViewPrivileges( financialUser );
            this.BuildCensusMenuViewPrivileges( financialUser );
            this.BuildReportsMenuViewPrivileges( financialUser );
            this.BuildHelpMenuViewPrivileges( financialUser );

            this.BuilEditPatientAccoutPrivileges( financialUser );
            this.BuildActivatePreregisteredAccountPrivileges( financialUser );
        }

        private void BuildSchedulerPrivileges( Role scheduler )
        {
            scheduler.AddPrivilege( new Privilege( Privilege.Actions.View, typeof( Announcement ) ) );

            this.BuildFileMenuViewPrivileges( scheduler );
            this.BuildEditMenuViewPrivileges( scheduler );
            scheduler.AddPrivilege( new Privilege( Privilege.Actions.View, new OnlinePreRegistrationActivity().Description ) );
            scheduler.AddPrivilege( new Privilege( Privilege.Actions.View, new PreRegistrationActivity().Description ) );
            scheduler.AddPrivilege( new Privilege( Privilege.Actions.View, new EditAccountActivity().Description ) );
            scheduler.AddPrivilege( new Privilege( Privilege.Actions.View, new PrintFaceSheetActivity().Description ) );
            scheduler.AddPrivilege( new Privilege( Privilege.Actions.View, new ViewAccountActivity().Description ) );
            scheduler.AddPrivilege( new Privilege( Privilege.Actions.View, new CancelPreRegActivity().Description ) );
            scheduler.AddPrivilege( new Privilege( Privilege.Actions.View, new PreRegistrationWithOfflineActivity().Description ) );
            scheduler.AddPrivilege( new Privilege( Privilege.Actions.View, new PreRegistrationWorklistActivity().Description ) );
            scheduler.AddPrivilege( new Privilege( Privilege.Actions.View, new NoShowWorklistActivity().Description ) );
            scheduler.AddPrivilege( new Privilege( Privilege.Actions.View, new QuickAccountCreationActivity().Description ) );
            scheduler.AddPrivilege( new Privilege( Privilege.Actions.View, new PreAdmitNewbornActivity().Description ) );
            scheduler.AddPrivilege( new Privilege( Privilege.Actions.View, new PreAdmitNewbornWithOfflineActivity().Description ) );
            scheduler.AddPrivilege( new Privilege( Privilege.Actions.View, new ShortPreRegistrationWithOfflineActivity().Description));
            scheduler.AddPrivilege(new Privilege(Privilege.Actions.View, new ShortPreRegistrationActivity().Description));
            scheduler.AddPrivilege( new Privilege( Privilege.Actions.View, new UCCPreMSERegistrationActivity().Description ));
            scheduler.AddPrivilege( new Privilege( Privilege.Actions.View, new UCCPostMseRegistrationActivity().Description ));
            this.BuildHelpMenuViewPrivileges( scheduler );

            scheduler.AddPrivilege( new Privilege( Privilege.Actions.Edit, VisitType.PREREG_PATIENT ) );
        }

        private void BuildFileMenuViewPrivileges( Role aRole )
        {
            aRole.AddPrivilege( new Privilege( Privilege.Actions.View, "&Home" ) );
            aRole.AddPrivilege( new Privilege( Privilege.Actions.View, "&Log Off" ) );
            aRole.AddPrivilege( new Privilege( Privilege.Actions.View, "E&xit  && Log off" ) );
        }

        private void BuildEditMenuViewPrivileges( Role aRole )
        {
            aRole.AddPrivilege( new Privilege( Privilege.Actions.View, "&Undo" ) );
            aRole.AddPrivilege( new Privilege( Privilege.Actions.View, "Cu&t" ) );
            aRole.AddPrivilege( new Privilege( Privilege.Actions.View, "&Copy" ) );
            aRole.AddPrivilege( new Privilege( Privilege.Actions.View, "&Paste" ) );
            aRole.AddPrivilege( new Privilege( Privilege.Actions.View, "Select &All" ) );
        }

        private void BuildRegisterMenuViewPrivileges( Role aRole )
        {
            aRole.AddPrivilege( new Privilege( Privilege.Actions.View, new PreRegistrationActivity().Description ) );
            aRole.AddPrivilege( new Privilege( Privilege.Actions.View, new OnlinePreRegistrationActivity().Description ) );
            aRole.AddPrivilege( new Privilege( Privilege.Actions.View, new RegistrationActivity().Description ) );
            aRole.AddPrivilege( new Privilege( Privilege.Actions.View, new ShortRegistrationActivity().Description ) );
            aRole.AddPrivilege( new Privilege( Privilege.Actions.View, new ShortPreRegistrationActivity().Description ) );
            aRole.AddPrivilege( new Privilege( Privilege.Actions.View, new PreMSERegisterActivity().Description ) );
            aRole.AddPrivilege( new Privilege( Privilege.Actions.View, new PostMSERegistrationActivity().Description ) );
            aRole.AddPrivilege( new Privilege( Privilege.Actions.View, new AdmitNewbornActivity().Description ) );
            aRole.AddPrivilege( new Privilege( Privilege.Actions.View, new PreAdmitNewbornActivity().Description ) );
            aRole.AddPrivilege( new Privilege( Privilege.Actions.View, new EditAccountActivity().Description ) );
            aRole.AddPrivilege( new Privilege( Privilege.Actions.View, new PrintFaceSheetActivity().Description ) );
            aRole.AddPrivilege( new Privilege( Privilege.Actions.View, new ViewAccountActivity().Description ) );
            aRole.AddPrivilege( new Privilege( Privilege.Actions.View, new CancelPreRegActivity().Description ) );
            aRole.AddPrivilege( new Privilege( Privilege.Actions.View, new CancelInpatientStatusActivity().Description ) );
            aRole.AddPrivilege( new Privilege( Privilege.Actions.View, new QuickAccountCreationActivity().Description ) );
            aRole.AddPrivilege(new Privilege(Privilege.Actions.View, new PAIWalkinOutpatientCreationActivity().Description));
            aRole.AddPrivilege( new Privilege( Privilege.Actions.View, new UCCPreMSERegistrationActivity().Description ));
            aRole.AddPrivilege( new Privilege( Privilege.Actions.View, new UCCPostMseRegistrationActivity().Description ));
            
            

            aRole.AddPrivilege( new Privilege( Privilege.Actions.View, new PreRegistrationWithOfflineActivity().Description ) );
            aRole.AddPrivilege( new Privilege( Privilege.Actions.View, new RegistrationWithOfflineActivity().Description ) );
            aRole.AddPrivilege( new Privilege( Privilege.Actions.View, new PreMSERegistrationWithOfflineActivity().Description ) );
            aRole.AddPrivilege( new Privilege( Privilege.Actions.View, new AdmitNewbornWithOfflineActivity().Description ) );
            aRole.AddPrivilege( new Privilege( Privilege.Actions.View, new PreAdmitNewbornWithOfflineActivity().Description ) );
            aRole.AddPrivilege( new Privilege( Privilege.Actions.View, new ShortPreRegistrationWithOfflineActivity().Description));
            aRole.AddPrivilege( new Privilege( Privilege.Actions.View, new ShortRegistrationWithOfflineActivity().Description));
        }

        private void BuildDischargeMenuViewPrivileges( Role aRole )
        {
            aRole.AddPrivilege( new Privilege( Privilege.Actions.View, new PreDischargeActivity().Description ) );
            aRole.AddPrivilege( new Privilege( Privilege.Actions.View, new DischargeActivity().Description ) );
            aRole.AddPrivilege( new Privilege( Privilege.Actions.View, new EditDischargeDataActivity().Description ) );
            aRole.AddPrivilege( new Privilege( Privilege.Actions.View, new EditRecurringDischargeActivity().Description ) );
            aRole.AddPrivilege( new Privilege( Privilege.Actions.View, new CancelInpatientDischargeActivity().Description ) );
            aRole.AddPrivilege( new Privilege( Privilege.Actions.View, new CancelOutpatientDischargeActivity().Description ) );
        }

        private void BuildTransferMenuViewPrivileges( Role aRole )
        {
            aRole.AddPrivilege( new Privilege( Privilege.Actions.View, new TransferActivity().Description ) );
            aRole.AddPrivilege( new Privilege( Privilege.Actions.View, new TransferOutToInActivity().Description ) );
            aRole.AddPrivilege( new Privilege( Privilege.Actions.View, new TransferInToOutActivity().Description ) );
            aRole.AddPrivilege( new Privilege( Privilege.Actions.View, new TransferBedSwapActivity().Description ) );
            aRole.AddPrivilege(new Privilege(Privilege.Actions.View, new TransferOutpatientToERActivity().Description));
            aRole.AddPrivilege(new Privilege(Privilege.Actions.View, new TransferERToOutpatientActivity().Description));
     
        }

        private void BuildWorklistMenuViewPrivileges( Role aRole )
        {
            aRole.AddPrivilege( new Privilege( Privilege.Actions.View, new PreRegistrationWorklistActivity().Description ) );
            aRole.AddPrivilege( new Privilege( Privilege.Actions.View, new PostRegistrationWorklistActivity().Description ) );
            aRole.AddPrivilege( new Privilege( Privilege.Actions.View, new InsuranceVerificationWorklistActivity().Description ) );
            aRole.AddPrivilege( new Privilege( Privilege.Actions.View, new PatientLiabilityWorklistActivity().Description ) );
            aRole.AddPrivilege( new Privilege( Privilege.Actions.View, new PreMSEWorklistActivity().Description ) );
            aRole.AddPrivilege( new Privilege( Privilege.Actions.View, new NoShowWorklistActivity().Description ) );
        }

        private void BuildCensusMenuViewPrivileges( Role aRole )
        {
            aRole.AddPrivilege( new Privilege( Privilege.Actions.View, new CensusByPatientActivity().Description ) );
            aRole.AddPrivilege( new Privilege( Privilege.Actions.View, new CensusByNursingStationActivity().Description ) );
            aRole.AddPrivilege( new Privilege( Privilege.Actions.View, new CensusByADTActivity().Description ) );
            aRole.AddPrivilege( new Privilege( Privilege.Actions.View, new CensusByPhysicianActivity().Description ) );
            aRole.AddPrivilege( new Privilege( Privilege.Actions.View, new CensusByBloodlessActivity().Description ) );
            aRole.AddPrivilege( new Privilege( Privilege.Actions.View, new CensusByReligionActivity().Description ) );
            aRole.AddPrivilege( new Privilege( Privilege.Actions.View, new CensusByPayorActivity().Description ) );
        }

        private void BuildReportsMenuViewPrivileges( Role aRole )
        {
            aRole.AddPrivilege( new Privilege( Privilege.Actions.View, new PhysiciansReportActivity().Description ) );
        }

        private void BuildHelpMenuViewPrivileges( Role aRole )
        {
            aRole.AddPrivilege( new Privilege( Privilege.Actions.View, "Patient Access &Help" ) );
            aRole.AddPrivilege( new Privilege( Privilege.Actions.View, "&About Patient Access" ) );
        }
        
        private void BuilEditPatientAccoutPrivileges( Role aRole )
        {
            aRole.AddPrivilege( new Privilege( Privilege.Actions.Edit, VisitType.PREREG_PATIENT ) );
            aRole.AddPrivilege( new Privilege( Privilege.Actions.Edit, VisitType.INPATIENT ) );
            aRole.AddPrivilege( new Privilege( Privilege.Actions.Edit, VisitType.OUTPATIENT ) );
            aRole.AddPrivilege( new Privilege( Privilege.Actions.Edit, VisitType.EMERGENCY_PATIENT ) );
            aRole.AddPrivilege( new Privilege( Privilege.Actions.Edit, VisitType.RECURRING_PATIENT ) );
            aRole.AddPrivilege( new Privilege( Privilege.Actions.Edit, VisitType.NON_PATIENT ) );
        }

        private void BuildActivatePreregisteredAccountPrivileges ( Role aRole)
        {
            aRole.AddPrivilege( new Privilege( Privilege.Actions.Activate, typeof( Account ) ) );
        }

        private void BuildAdminMenuViewPrivileges(Role aRole)
        {
            aRole.AddPrivilege( new Privilege( Privilege.Actions.View, "Manage &New Employers" ) );
        }

        private void BuildDischargeandTransferUserPrivileges(Role dischargeandTransferUser)
        {
            this.BuildFileMenuViewPrivileges(dischargeandTransferUser);
            this.BuildHelpMenuViewPrivileges(dischargeandTransferUser);
            this.BuildCensusMenuViewPrivileges(dischargeandTransferUser);

            dischargeandTransferUser.AddPrivilege(new Privilege(Privilege.Actions.View, new PreDischargeActivity().Description));
            dischargeandTransferUser.AddPrivilege(new Privilege(Privilege.Actions.View, new DischargeActivity().Description));
            dischargeandTransferUser.AddPrivilege(new Privilege(Privilege.Actions.View, new EditDischargeDataActivity().Description));
            dischargeandTransferUser.AddPrivilege(new Privilege(Privilege.Actions.View, new CancelInpatientDischargeActivity().Description));
            dischargeandTransferUser.AddPrivilege(new Privilege(Privilege.Actions.View, new TransferActivity().Description));
            dischargeandTransferUser.AddPrivilege(new Privilege(Privilege.Actions.View, new TransferBedSwapActivity().Description));
            dischargeandTransferUser.AddPrivilege(new Privilege(Privilege.Actions.View, new ViewAccountActivity().Description));
            dischargeandTransferUser.AddPrivilege(new Privilege(Privilege.Actions.View, new PrintFaceSheetActivity().Description));

        }

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public PatientAccessApplication()
            : base(ConfigurationManager.AppSettings[SETTING_ACCESS_KEY],
            ConfigurationManager.AppSettings[SETTING_APPLICATION_GUID],
            ConfigurationManager.AppSettings[SETTING_APPLICATION_NAME] )
        {
            this.LegacyApplicationName = ConfigurationManager.AppSettings[SETTING_LEGACY_APPLICATION_NAME];

            IRoleBroker rb = BrokerFactory.BrokerOfType< IRoleBroker >() ;
            Hashtable allRoles = rb.AllRoles();

            foreach( Role role in allRoles.Values )
            {
                this.AddRole( role );
            }
           Role systemAdmin = this.RoleWith( SYSTEM_ADMIN ),
                registrationAdministrator = this.RoleWith( REGISTRATION_ADMINISTRATOR ),
                registrationUser = this.RoleWith( REGISTRATION_USER ),
                financialUser = this.RoleWith( FINANCIAL_USER ),
                creditReportViewer = this.RoleWith( CREDIT_REPORT_VIEWER ),
                DischargeTransferUser = this.RoleWith(DISCHARGE_TRANSFER_USER),
                scheduler = this.RoleWith( SCHEDULER );

            this.BuildSystemAdminPrivileges( systemAdmin );
            this.BuildRegistrationAdministratorRivileges( registrationAdministrator );
            this.BuildRegistrationUserPrivileges( registrationUser );
            this.BuildFinancialUserPrivileges( financialUser );
            this.BuildSchedulerPrivileges( scheduler );
            this.BuildDischargeandTransferUserPrivileges(DischargeTransferUser);
        }

        #endregion

        #region Data Elements

        private string i_LegacyApplicationName;

        #endregion

        #region Constants

	    private const string
            SYSTEM_ADMIN                = "SystemAdmin",
            REGISTRATION_ADMINISTRATOR  = "RegistrationAdministrator",
            REGISTRATION_USER           = "RegistrationUser",
            FINANCIAL_USER              = "FinancialUser",
            CREDIT_REPORT_VIEWER        = "CreditReportViewer",
            SCHEDULER                   = "Scheduler",
            DISCHARGE_TRANSFER_USER     = "DischargeTransferUser";

        private const string 
            SETTING_APPLICATION_NAME        = "ADAMApplicationName",
            SETTING_APPLICATION_GUID        = "ADAMApplicationGUID",
            SETTING_ACCESS_KEY              = "ADAMAccessKey",
            SETTING_LEGACY_APPLICATION_NAME = "ADAMLegacyApplicationName";

        #endregion
    }
}
