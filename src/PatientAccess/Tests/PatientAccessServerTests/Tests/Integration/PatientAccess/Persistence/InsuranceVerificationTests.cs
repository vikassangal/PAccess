using System;
using System.Collections;
using PatientAccess.Actions;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Rules;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    [TestFixture()]
    public class InsuranceVerificationTests : AbstractBrokerTests
    {
        #region Constants
        private DateTime expected_Dob = new DateTime(1953, 2, 7 );
        #endregion

        #region SetUp and TearDown InsuranceVerificationTests
        [TestFixtureSetUp()]
        public static void SetUpRulesTests()
        {
            CreateUser();

            i_facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            i_Facility = i_facilityBroker.FacilityWith(900);
            i_accountBroker = BrokerFactory.BrokerOfType<IAccountBroker>();
            patient.Oid = 46;
            patient.Facility = i_Facility;
            patient.MedicalRecordNumber = 785213;
            accounts = i_accountBroker.AccountsFor( patient );
           
        }

        [TestFixtureTearDown()]
        public static void TearDownRulesTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]		
        //             [Ignore( "Until ZPA updated " )]
        public void TestBenefitsVerificationIncompleteRule()
        {    
            foreach( AccountProxy ap in accounts )
            {
                Account anAccount = ap.AsAccount();
                BenefitsVerificationRequired benefitsRule = new BenefitsVerificationRequired();
                 
                if( !benefitsRule.CanBeAppliedTo(anAccount))
                {
                    BenefitsVerificationIncomplete  action = new BenefitsVerificationIncomplete();
                    action.Context = ap;
                    action.Execute();
                }               
            }               
        }

        [Test()]
        //        [Ignore( "Breaking the build; DC 20060310" )]
        public void TestIncorrectPlanIDRule()
        {
            
            foreach( AccountProxy ap in accounts )
            {
                Account anAccount = ap.AsAccount();
                PlanIDRequired  incorrectPlanID = new  PlanIDRequired();
                if(incorrectPlanID.CanBeAppliedTo(anAccount))
                {
                    IncorrectPlanID planIDAction = new IncorrectPlanID();
                    planIDAction.Context = ap;
                    planIDAction.Execute();
                }
            }
        }
        [Test()]
        //        [Ignore( "Breaking the build; DC 20060310" )]
        public void TestAuthorizationIncompleteRule()
        {
            foreach( AccountProxy ap in accounts )
            {
                Account anAccount = ap.AsAccount();
                AuthorizationRequired authorizationIncomplete = new AuthorizationRequired();
                if(authorizationIncomplete.CanBeAppliedTo(anAccount))
                {
                    AuthorizationIncomplete authorizationIncompleteAction = new AuthorizationIncomplete();
                    authorizationIncompleteAction.Context = ap;
                    authorizationIncompleteAction.Execute();
                }
            }
        }
        [Test()]
             
        [Description("VeryLongExecution")]
        //             [Ignore( "Until ZPA updated " )]
        public void TestInsuranceVerificationRule()
        {
            foreach( AccountProxy ap in accounts )
            {
                Account anAccount = ap.AsAccount();
                InsuranceVerificationRule rule = new InsuranceVerificationRule();

                rule.CanBeAppliedTo(anAccount);
            }
        }
        
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        private static IAccountBroker i_accountBroker = null;
        private static IFacilityBroker i_facilityBroker = null;
        private static  Facility i_Facility;
        private static Patient patient = new Patient();
        private static ArrayList accounts  = new ArrayList();          
       
        #endregion
    }
}