using System;
using Extensions.PersistenceCommon;
using NUnit.Framework;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Rules;

namespace Tests.Integration.PatientAccess.UI
{
    [TestFixture]
    public class PreRegistrationActivityPreferredFieldsTests
    {

        # region Private Properties
        private RuleEngine i_RuleEngine;

        private RuleEngine RuleEngine
        {
            get
            {
                if (i_RuleEngine == null)
                {
                    i_RuleEngine = RuleEngine.GetInstance();
                }
                return i_RuleEngine;
            }
        }

        #endregion

        #region Test Methods

        [Test]
        public void MailingAddressisPreferredForPreRegistrationActivity()
        {
            //This unit test is written to test fix for the defect 23229
            
            //Create Mother's account
            var mothersAccount = GetMothersAccount();

            //Create Newborn account
           mothersAccount.Activity = new PreAdmitNewbornActivity();
            var babysAccount = GetNewBornAccount();

            //Load Rules for NewBorn account
            accountCopyBroker.CreateAccountCopyFor(mothersAccount);
            RuleEngine.RulesToRun.Clear();
            RuleEngine.LoadRules(babysAccount);

           //Edit/Maintain Newborn account
            babysAccount.Activity = new MaintenanceActivity
            {
                AssociatedActivity = new PreRegistrationActivity()
            };

            //Load Rules for NewBorn account
            RuleEngine.LoadRules(babysAccount);

           //Edit/Maintain mothers account
            mothersAccount.Activity = new MaintenanceActivity
            {
                AssociatedActivity = new PreRegistrationActivity()
            };

           //Load Rules for Mother's account
            RuleEngine.LoadRules(mothersAccount);

           //Mother's account should show mailing address preferred
            Assert.IsTrue(RuleEngine.RulesToRun.Contains(typeof(MailingAddressPreferred)), mailingAddressPreferredRule);
            
        }

        #endregion

        #region Support Methods

        private Account GetMothersAccount()
        {
            var patient = new Patient(PATIENT_OID, PersistentModel.NEW_VERSION, PATIENT_NAME,
                PATIENT_MRN, PATIENT_DOB, PATIENT_SSN, PATIENT_SEX, DHF_FACILITY);
            var account = new Account
            {
                Activity = new PreRegistrationActivity(),
                Facility = DHF_FACILITY,
                HospitalService = new HospitalService(0, PersistentModel.NEW_VERSION, "PRE ADMIT", "35"),
                Patient = patient,
                AccountNumber = MOTHER_ACC_NUM,
                KindOfVisit = VisitType.PreRegistration
            };
            return account;
        }

        private Account GetNewBornAccount()
        {
            var account = new Account
            {
                Activity = new PreAdmitNewbornActivity(),
                Facility = DHF_FACILITY,
                IsNewBorn = true,
                KindOfVisit = VisitType.PreRegistration
            };
            return account;
        }

    #endregion

        #region Data Elements

        private const string
            PATIENT_F_NAME = "Sam",
            PATIENT_L_NAME = "Spade",
            PATIENT_MI = "L";

        private readonly Name
           PATIENT_NAME = new Name(PATIENT_F_NAME, PATIENT_L_NAME, PATIENT_MI);

        private const long
            PATIENT_OID = 45L,
            PATIENT_MRN = 123456789,
            MOTHER_ACC_NUM = 98765;

        private readonly SocialSecurityNumber
            PATIENT_SSN = new SocialSecurityNumber("123121234");

        private readonly DateTime
            PATIENT_DOB = new DateTime(1955, 3, 5);

        private readonly Gender PATIENT_SEX = new Gender(0, DateTime.Now, "Female", "F");

        private readonly IAccountCopyBroker accountCopyBroker = BrokerFactory.BrokerOfType<IAccountCopyBroker>();
        private static readonly IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
        private static Facility DHF_FACILITY = facilityBroker.FacilityWith(DHF_FACILITY_CODE);

    #endregion

        #region Constants

        private const string DHF_FACILITY_CODE = "DHF";
        private const int preAdmitRuleCount = 261;
        private const int preRegRulesCount = 294;
        private const string mailingAddressRequiredRule = "PatientAccess.Rules.MailingAddressRequired";
        private const string mailingAddressPreferredRule = "PatientAccess.Rules.MailingAddressPreferred";

        #endregion

    }
}
