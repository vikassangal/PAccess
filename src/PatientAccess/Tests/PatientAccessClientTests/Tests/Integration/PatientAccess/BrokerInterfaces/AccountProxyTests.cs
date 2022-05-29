using System;
using System.Collections;
using Extensions.UI.Builder;
using NUnit.Framework;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace Tests.Integration.PatientAccess.BrokerInterfaces
{
    [TestFixture()]
    public class AccountProxyTests
    {
        #region Constants
        private const long
            ACO_FACILITY_ID				= 900L,
            DR_GERRY_DICKMON_ID			= 3L,
            SAUL_FESTINGER_ACCT_NUM		= 5208513L;

        private const int
            MAX_CONSULT_PHYS		= 5;

        private const string
            PHYSICIAN1_FIRST_NAME			= "GERRY",
            PHYSICIAN1_LAST_NAME			= "DICKMON";
        #endregion

        #region SetUp and TearDown AccountProxyTests
        [TestFixtureSetUp()]
        public static void SetUpAccountProxyTests()
        {
            i_facilityBroker    = BrokerFactory.BrokerOfType<IFacilityBroker>();
            i_worklistBroker    = BrokerFactory.BrokerOfType<IWorklistSettingsBroker>();
            i_PhysicianBroker   = BrokerFactory.BrokerOfType<IPhysicianBroker>();
        }

        [TestFixtureTearDown()]
        public static void TearDownAccountProxyTests()
        {
        }

        [SetUp()]
        public void SetupTests()
        {
            i_AccountProxy  = new AccountProxy();
            i_AccountProxy.AccountNumber    = SAUL_FESTINGER_ACCT_NUM;
            i_AccountProxy.Facility         = i_facilityBroker.FacilityWith( ACO_FACILITY_ID );
        }
        #endregion

        #region Test Methods
        [Test()]
        
        public void AsAccount()
        {
            IPatientBroker patientBroker = BrokerFactory.BrokerOfType<IPatientBroker>();

            PatientSearchCriteria criteria = new PatientSearchCriteria(
                "DEL",
                String.Empty,
                String.Empty,
                String.Empty,
                null,
                PatientSearchCriteria.NO_MONTH,
                PatientSearchCriteria.NO_YEAR,
                String.Empty,
                "5129966"
                );

            PatientSearchResponse patientSearchResponse = patientBroker.GetPatientSearchResponseFor( criteria );

            Assert.IsTrue( 
                patientSearchResponse.PatientSearchResults.Count > 0,
                "There should be one patient with this account number");

            Patient aPatient = patientBroker.PatientFrom( patientSearchResponse.PatientSearchResults[0]);

            AccountProxy accountProxy = aPatient.Accounts[0] as AccountProxy;
            
            Account realAccount = accountProxy.AsAccount();
        

            Assert.AreEqual(
                accountProxy.AccountNumber,
                realAccount.AccountNumber
                );            
        }

        [Test()]
        [Ignore()]
        public void TestRemainingActionsForWorklist()
        {
            AccountProxy ap = new AccountProxy();
            ap.AccountNumber = 30015;
            ap.Facility = i_facilityBroker.FacilityWith(900);
            ap.ActionsLoader = new ActionLoader(ap);
		        
            Worklist wl = i_worklistBroker.WorklistWith(1);
            ActionsList actions = (ActionsList)ap.GetRemainingActionsFor(wl);
            Assert.IsNotNull( actions, "Can not get remaining actions" );
            Assert.AreEqual( 1, actions.Count, "Wrong number of actions" );

            foreach( IAction action in actions )
            {
                if ( action.Context == null )
                    Assert.Fail("Did not find context for actions");
            }
        }

        [Test()]
        [Ignore()]
        public void TestAllRemainingActions()
        {
            AccountProxy ap = new AccountProxy();
            ap.AccountNumber = 30015;
            ap.Facility = i_facilityBroker.FacilityWith(900);
            ap.ActionsLoader = new ActionLoader(ap);

            ICollection actions = ap.GetAllRemainingActions();

            Assert.IsNotNull( actions, "Can not get remaining actions" );
            Assert.AreEqual( 1, actions.Count, "Wrong number of actions" );
        }

        [Test()]
        public void AddAdmittingPhysicianRelationship()
        {
            this.AccountProxy.AddPhysicianRelationship( 
                this.IPhysicianBroker.BuildAdmittingPhysicianRelationship( 
                    ACO_FACILITY_ID,
                    DR_GERRY_DICKMON_ID ) );

            Assert.AreEqual( 1, 
                             this.AccountProxy.AllPhysicianRelationships.Count,
                             "There should be one physician relationship" );

            PhysicianRelationship admRelationship = this.AccountProxy.PhysicianRelationshipWithRole( 
                PhysicianRole.Admitting().Role() );

            Assert.IsNotNull( admRelationship.Physician, "Admitting Physician does not exist" );
            Assert.IsNotNull( admRelationship.PhysicianRole,  
                              "Admitting Physician Role does not exist" );
            Assert.AreEqual( admRelationship.Physician.FirstName, PHYSICIAN1_FIRST_NAME ,
                             "Admitting Physician's first name does not match" );
            Assert.AreEqual( 
                admRelationship.Physician.LastName, PHYSICIAN1_LAST_NAME ,
                "Admitting Physician's last name does not match" );

            Assert.AreEqual( PhysicianRole.Admitting().Role(), 
                             admRelationship.PhysicianRole.Role() ,
                             "Admitting Physician Roles do not match" );
        }

        [Test()]
        public void AddAttendingPhysicianRelationship()
        {
            this.AccountProxy.AddPhysicianRelationship( 
                this.IPhysicianBroker.BuildAttendingPhysicianRelationship( 
                    ACO_FACILITY_ID,
                    DR_GERRY_DICKMON_ID ) );

            Assert.AreEqual( 1, 
                             this.AccountProxy.AllPhysicianRelationships.Count,
                             "There should be one physician relationship" );

            PhysicianRelationship attRelationship = this.AccountProxy.PhysicianRelationshipWithRole( 
                PhysicianRole.Attending().Role() );

            Assert.IsNotNull( attRelationship.Physician, "Attending Physician does not exist" );
            Assert.IsNotNull( attRelationship.PhysicianRole,
                              "Attending Physician Role does not exist" );
            Assert.AreEqual( attRelationship.Physician.FirstName, PHYSICIAN1_FIRST_NAME,
                             "Attending Physician's first name does not match");
            Assert.AreEqual( attRelationship.Physician.LastName, PHYSICIAN1_LAST_NAME,
                             "Attending Physician's last name does not match");

            Assert.AreEqual( PhysicianRole.Attending().Role(), attRelationship.PhysicianRole.Role(),
                             "Attending Physician Roles do not match" );
        }

        [Test()]
        public void AddReferringPhysicianRelationship()
        {
            this.AccountProxy.AddPhysicianRelationship( 
                this.IPhysicianBroker.BuildReferringPhysicianRelationship( 
                    ACO_FACILITY_ID,
                    DR_GERRY_DICKMON_ID ) );

            Assert.AreEqual( 1, 
                             this.AccountProxy.AllPhysicianRelationships.Count,
                             "There should be one physician relationship" );

            PhysicianRelationship refRelationship = this.AccountProxy.PhysicianRelationshipWithRole( 
                PhysicianRole.Referring().Role() );

            Assert.IsNotNull( refRelationship.Physician,
                              "Referring Physician does not exist");
            Assert.IsNotNull( refRelationship.PhysicianRole,
                              "Referring Physician Role does not exist");
            Assert.AreEqual( refRelationship.Physician.FirstName, PHYSICIAN1_FIRST_NAME,
                             "Referring Physician's first name does not match");
            Assert.AreEqual( refRelationship.Physician.LastName, PHYSICIAN1_LAST_NAME,
                             "Referring Physician's last name does not match");

            Assert.AreEqual( PhysicianRole.Referring().Role(), refRelationship.PhysicianRole.Role(),
                             "Referring Physician Roles do not match" );
        }

        [Test()]
        public void AddOperatingPhysicianRelationship()
        {
            this.AccountProxy.AddPhysicianRelationship( 
                this.IPhysicianBroker.BuildOperatingPhysicianRelationship( 
                    ACO_FACILITY_ID,
                    DR_GERRY_DICKMON_ID ) );

            Assert.AreEqual( 1, 
                             this.AccountProxy.AllPhysicianRelationships.Count,
                             "There should be one physician relationship" );

            PhysicianRelationship oprRelationship = this.AccountProxy.PhysicianRelationshipWithRole( 
                PhysicianRole.Operating().Role() );

            Assert.IsNotNull( oprRelationship.Physician,
                              "Operating Physician does not exist" );
            Assert.IsNotNull( oprRelationship.PhysicianRole,
                              "Operating Physician Role does not exist" );
            Assert.AreEqual( oprRelationship.Physician.FirstName, PHYSICIAN1_FIRST_NAME,
                             "Operating Physician's first name does not match");
            Assert.AreEqual( oprRelationship.Physician.LastName, PHYSICIAN1_LAST_NAME,
                             "Operating Physician's last name does not match" );

            Assert.AreEqual( PhysicianRole.Operating().Role(), oprRelationship.PhysicianRole.Role(),
                             "Operating Physician Roles do not match" );
        }

        [Test()]
        public void AddPrimaryCarePhysicianRelationship()
        {
            this.AccountProxy.AddPhysicianRelationship( 
                this.IPhysicianBroker.BuildPrimaryCarePhysicianRelationship( 
                    ACO_FACILITY_ID,
                    DR_GERRY_DICKMON_ID ) );

            Assert.AreEqual( 1, 
                             this.AccountProxy.AllPhysicianRelationships.Count,
                             "There should be one physician relationship" );

            PhysicianRelationship othRelationship = this.AccountProxy.PhysicianRelationshipWithRole( 
                PhysicianRole.PrimaryCare().Role() );

            Assert.IsNotNull( othRelationship.Physician,
                              "PrimaryCare Physician does not exist" );
            Assert.IsNotNull( othRelationship.PhysicianRole,
                              "PrimaryCare Physician Role does not exist" );
            Assert.AreEqual( othRelationship.Physician.FirstName, PHYSICIAN1_FIRST_NAME,
                             "PrimaryCare Physician's first name does not match" );
            Assert.AreEqual( othRelationship.Physician.LastName, PHYSICIAN1_LAST_NAME,
                             "PrimaryCare Physician's last name does not match" );

            Assert.AreEqual( PhysicianRole.PrimaryCare().Role(), othRelationship.PhysicianRole.Role(),
                             "PrimaryCare Physician Roles do not match" );
        }

        [Test()]
        public void AddConsultingPhysicianRelationship()
        {
            this.AccountProxy.AddPhysicianRelationship( 
                this.IPhysicianBroker.BuildConsultingPhysicianRelationship( 
                    ACO_FACILITY_ID,
                    DR_GERRY_DICKMON_ID ) );

            Assert.AreEqual( 1, 
                             this.AccountProxy.AllPhysicianRelationships.Count,
                             "There should be one physician relationship" );

            PhysicianRelationship conRelationship = this.AccountProxy.PhysicianRelationshipWithRole( 
                PhysicianRole.Consulting().Role() );

            Assert.IsNotNull( conRelationship.Physician,
                              "Consulting Physician does not exist" );
            Assert.IsNotNull( conRelationship.PhysicianRole,
                              "Consulting Physician Role does not exist" );
            Assert.AreEqual( conRelationship.Physician.FirstName, PHYSICIAN1_FIRST_NAME,
                             "Consulting Physician's first name does not match" );
            Assert.AreEqual( conRelationship.Physician.LastName, PHYSICIAN1_LAST_NAME,
                             "Consulting Physician's last name does not match" );

            Assert.AreEqual( PhysicianRole.Consulting().Role(), conRelationship.PhysicianRole.Role(),
                             "Consulting Physician Roles do not match" );
        }

        [Test()]
        public void AddMaxPlusOneConsultingPhysicianRelationship()
        {
            for( int i=1; i<=MAX_CONSULT_PHYS; i++ )
            {
                this.AccountProxy.AddPhysicianRelationship( 
                    this.IPhysicianBroker.BuildConsultingPhysicianRelationship( 
                        ACO_FACILITY_ID,
                        DR_GERRY_DICKMON_ID ) );

                Assert.AreEqual( i, 
                                 this.AccountProxy.AllPhysicianRelationships.Count,
                                 "There should be " + i + " physician relationship(s)" );
            }

            this.AccountProxy.AddPhysicianRelationship( 
                this.IPhysicianBroker.BuildConsultingPhysicianRelationship( 
                    ACO_FACILITY_ID,
                    DR_GERRY_DICKMON_ID ) );

            Assert.AreEqual( MAX_CONSULT_PHYS, 
                             this.AccountProxy.AllPhysicianRelationships.Count,
                             "There should be 5 physician relationships" );
        }
		
        [Test()]
        public void PhysicianAndPhysicianRelationshipTests()
        {
            #region AddAdmittingPhysicianRelationship
            this.AccountProxy.AddPhysicianRelationship( 
                this.IPhysicianBroker.BuildAdmittingPhysicianRelationship( 
                    ACO_FACILITY_ID,
                    DR_GERRY_DICKMON_ID ) );

            Assert.AreEqual( 1, 
                             this.AccountProxy.AllPhysicianRelationships.Count,
                             "There should be one physician relationship" );

            PhysicianRelationship admRelationship = this.AccountProxy.PhysicianRelationshipWithRole( 
                PhysicianRole.Admitting().Role() );

            Assert.IsNotNull( admRelationship.Physician,
                              "Admitting Physician does not exist" );
            Assert.IsNotNull( admRelationship.PhysicianRole,
                              "Admitting Physician Role does not exist" );
            Assert.AreEqual( admRelationship.Physician.FirstName, PHYSICIAN1_FIRST_NAME,
                             "Admitting Physician's first name does not match" );
            Assert.AreEqual( admRelationship.Physician.LastName, PHYSICIAN1_LAST_NAME,
                             "Admitting Physician's last name does not match" );

            Assert.AreEqual( PhysicianRole.Admitting().Role(), admRelationship.PhysicianRole.Role(),
                             "Admitting Physician Roles do not match" );

            ArrayList admittingPhysicians = (ArrayList)this.AccountProxy.PhysiciansWith( 
                                                           PhysicianRole.Admitting().Role() );
            Assert.AreEqual( 1, 
                             admittingPhysicians.Count,
                             "There should be one admitting physician" );

            Physician admittingPhysician = this.AccountProxy.AdmittingPhysician;
            Assert.IsNotNull( admittingPhysician, "Admitting Physician does not exist" );
            Assert.AreEqual( admittingPhysician.FirstName, PHYSICIAN1_FIRST_NAME, "Admitting Physician's first name does not match" );
            Assert.AreEqual( admittingPhysician.LastName, PHYSICIAN1_LAST_NAME, "Admitting Physician's last name does not match" );
            #endregion

            #region AddAttendingPhysicianRelationship
            this.AccountProxy.AddPhysicianRelationship( 
                this.IPhysicianBroker.BuildAttendingPhysicianRelationship( 
                    ACO_FACILITY_ID,
                    DR_GERRY_DICKMON_ID ) );

            Assert.AreEqual( 2, 
                             this.AccountProxy.AllPhysicianRelationships.Count,
                             "There should be two physician relationships" );

            PhysicianRelationship attRelationship = this.AccountProxy.PhysicianRelationshipWithRole( 
                PhysicianRole.Attending().Role() );

            Assert.IsNotNull( attRelationship.Physician,
                              "Attending Physician does not exist" );
            Assert.IsNotNull( attRelationship.PhysicianRole,
                              "Attending Physician Role does not exist" );
            Assert.AreEqual( attRelationship.Physician.FirstName, PHYSICIAN1_FIRST_NAME,
                             "Attending Physician's first name does not match" );
            Assert.AreEqual( attRelationship.Physician.LastName, PHYSICIAN1_LAST_NAME,
                             "Attending Physician's last name does not match" );

            Assert.AreEqual( PhysicianRole.Attending().Role(), attRelationship.PhysicianRole.Role(),
                             "Attending Physician Roles do not match" );

            ArrayList attendingPhysicians = (ArrayList)this.AccountProxy.PhysiciansWith( 
                                                           PhysicianRole.Attending().Role() );
            Assert.AreEqual( 1, 
                             attendingPhysicians.Count,
                             "There should be one attending physician" );

            Physician attendingPhysician = this.AccountProxy.AttendingPhysician;
            Assert.IsNotNull( attendingPhysician, "Attending Physician does not exist");
            Assert.AreEqual( attendingPhysician.FirstName, PHYSICIAN1_FIRST_NAME,
                             "Attending Physician's first name does not match" );
            Assert.AreEqual( attendingPhysician.LastName, PHYSICIAN1_LAST_NAME,
                             "Attending Physician's last name does not match" );
            #endregion

            #region AddReferringPhysicianRelationship
            this.AccountProxy.AddPhysicianRelationship( 
                this.IPhysicianBroker.BuildReferringPhysicianRelationship( 
                    ACO_FACILITY_ID,
                    DR_GERRY_DICKMON_ID ) );

            Assert.AreEqual( 3, 
                             this.AccountProxy.AllPhysicianRelationships.Count,
                             "There should be three physician relationships" );

            PhysicianRelationship refRelationship = this.AccountProxy.PhysicianRelationshipWithRole( 
                PhysicianRole.Referring().Role() );

            Assert.IsNotNull( refRelationship.Physician, "Referring Physician does not exist" );
            Assert.IsNotNull( refRelationship.PhysicianRole,
                              "Referring Physician Role does not exist" );
            Assert.AreEqual( refRelationship.Physician.FirstName, PHYSICIAN1_FIRST_NAME,
                             "Referring Physician's first name does not match" );
            Assert.AreEqual( refRelationship.Physician.LastName, PHYSICIAN1_LAST_NAME ,
                             "Referring Physician's last name does not match" );

            Assert.AreEqual( PhysicianRole.Referring().Role(), refRelationship.PhysicianRole.Role(),
                             "Referring Physician Roles do not match" );

            ArrayList referringPhysicians = (ArrayList)this.AccountProxy.PhysiciansWith( 
                                                           PhysicianRole.Referring().Role() );
            Assert.AreEqual( 1, 
                             referringPhysicians.Count,
                             "There should be one referring physician" );

            Physician referringPhysician = this.AccountProxy.ReferringPhysician;
            Assert.IsNotNull( referringPhysician, "Referring Physician does not exist" );
            Assert.AreEqual( referringPhysician.FirstName, PHYSICIAN1_FIRST_NAME, 
                             "Referring Physician's first name does not match" );
            Assert.AreEqual( referringPhysician.LastName, PHYSICIAN1_LAST_NAME,
                             "Referring Physician's last name does not match" );
            #endregion

            #region AddOperatingPhysicianRelationship
            this.AccountProxy.AddPhysicianRelationship( 
                this.IPhysicianBroker.BuildOperatingPhysicianRelationship( 
                    ACO_FACILITY_ID,
                    DR_GERRY_DICKMON_ID ) );

            Assert.AreEqual( 4, 
                             this.AccountProxy.AllPhysicianRelationships.Count,
                             "There should be fours physician relationship" );

            PhysicianRelationship oprRelationship = this.AccountProxy.PhysicianRelationshipWithRole( 
                PhysicianRole.Operating().Role() );

            Assert.IsNotNull( oprRelationship.Physician, "Operating Physician does not exist" );
            Assert.IsNotNull( oprRelationship.PhysicianRole,
                              "Operating Physician Role does not exist" );
            Assert.AreEqual( oprRelationship.Physician.FirstName, PHYSICIAN1_FIRST_NAME,
                             "Operating Physician's first name does not match" );
            Assert.AreEqual( oprRelationship.Physician.LastName, PHYSICIAN1_LAST_NAME,
                             "Operating Physician's last name does not match" );

            Assert.AreEqual( PhysicianRole.Operating().Role(), oprRelationship.PhysicianRole.Role(),
                             "Operating Physician Roles do not match" );

            ArrayList operatingPhysicians = (ArrayList)this.AccountProxy.PhysiciansWith( 
                                                           PhysicianRole.Operating().Role() );
            Assert.AreEqual( 1, 
                             operatingPhysicians.Count,
                             "There should be one operating physician" );

            Physician operatingPhysician = this.AccountProxy.OperatingPhysician;
            Assert.IsNotNull( operatingPhysician,
                              "Operating Physician does not exist" );
            Assert.AreEqual( operatingPhysician.FirstName, PHYSICIAN1_FIRST_NAME,
                             "Operating Physician's first name does not match" );
            Assert.AreEqual( operatingPhysician.LastName, PHYSICIAN1_LAST_NAME,
                             "Operating Physician's last name does not match" );
            #endregion

            #region AddPrimaryCarePhysicianRelationship
            this.AccountProxy.AddPhysicianRelationship( 
                this.IPhysicianBroker.BuildPrimaryCarePhysicianRelationship( 
                    ACO_FACILITY_ID,
                    DR_GERRY_DICKMON_ID ) );

            Assert.AreEqual( MAX_CONSULT_PHYS, 
                             this.AccountProxy.AllPhysicianRelationships.Count,
                             "There should be five physician relationships" );

            PhysicianRelationship othRelationship = this.AccountProxy.PhysicianRelationshipWithRole( 
                PhysicianRole.PrimaryCare().Role() );

            Assert.IsNotNull( othRelationship.Physician,
                              "PrimaryCare Physician does not exist" );
            Assert.IsNotNull( othRelationship.PhysicianRole,
                              "PrimaryCare Physician Role does not exist" );
            Assert.AreEqual( othRelationship.Physician.FirstName, PHYSICIAN1_FIRST_NAME,
                             "PrimaryCare Physician's first name does not match" );
            Assert.AreEqual( othRelationship.Physician.LastName, PHYSICIAN1_LAST_NAME,
                             "PrimaryCare Physician's last name does not match" );

            Assert.AreEqual( PhysicianRole.PrimaryCare().Role(), othRelationship.PhysicianRole.Role(),
                             "PrimaryCare Physician Roles do not match" );

            ArrayList primaryCarePhysicians = (ArrayList)this.AccountProxy.PhysiciansWith( 
                                                       PhysicianRole.PrimaryCare().Role() );
            Assert.AreEqual( 1, 
                             primaryCarePhysicians.Count,
                             "There should be one Primary care physician" );

            Physician primaryCarePhysician = this.AccountProxy.PrimaryCarePhysician;
            Assert.IsNotNull(primaryCarePhysicians,
                              "Primary care Physician does not exist" );
            Assert.AreEqual( primaryCarePhysician.FirstName, PHYSICIAN1_FIRST_NAME,
                             "Primary care Physician's first name does not match");
            Assert.AreEqual( primaryCarePhysician.LastName, PHYSICIAN1_LAST_NAME,
                             "Primary care Physician's last name does not match");
            #endregion

            #region AddConsultingPhysicianRelationships
            for( int i=1; i<=MAX_CONSULT_PHYS; i++ )
            {
                this.AccountProxy.AddPhysicianRelationship( 
                    this.IPhysicianBroker.BuildConsultingPhysicianRelationship( 
                        ACO_FACILITY_ID,
                        DR_GERRY_DICKMON_ID ) );

                Assert.AreEqual( i, 
                                 this.AccountProxy.ConsultingPhysicians.Count,
                                 "There should be " + i + " physician relationship(s)" );
            }

            ArrayList consultingPhysicians = this.AccountProxy.ConsultingPhysicians;
            Assert.AreEqual( MAX_CONSULT_PHYS, 
                             consultingPhysicians.Count,
                             "There should be five consulting physician relationships" );

            foreach( Physician consultingPhysician in consultingPhysicians )
            {
                Assert.IsNotNull( consultingPhysician,"Consulting Physician does not exist"  );
                Assert.AreEqual( consultingPhysician.FirstName, PHYSICIAN1_FIRST_NAME,
                                 "Consulting Physician's first name does not match" );
                Assert.AreEqual( consultingPhysician.LastName, PHYSICIAN1_LAST_NAME,
                                 "Consulting Physician's last name does not match" );
            }

            consultingPhysicians.Clear();
            consultingPhysicians = (ArrayList)this.AccountProxy.PhysiciansWith( 
                                                  PhysicianRole.Consulting().Role() );
            Assert.AreEqual( MAX_CONSULT_PHYS, 
                             consultingPhysicians.Count,
                             "There should be five consulting physicians" );

            foreach( Physician consultingPhysician in consultingPhysicians )
            {
                Assert.IsNotNull( consultingPhysician, "Consulting Physician does not exist" );
                Assert.AreEqual( consultingPhysician.FirstName, PHYSICIAN1_FIRST_NAME,
                                 "Consulting Physician's first name does not match" );
                Assert.AreEqual( consultingPhysician.LastName, PHYSICIAN1_LAST_NAME,
                                 "Consulting Physician's last name does not match" );
            }

            ArrayList consultingPhysRelationships = 
                (ArrayList)this.AccountProxy.PhysicianRelationshipsWith( 
                               PhysicianRole.Consulting().Role() );
            Assert.AreEqual( MAX_CONSULT_PHYS, 
                             consultingPhysRelationships.Count,
                             "There should be five consulting physician relationships" );

            foreach( PhysicianRelationship consultingRelationship in consultingPhysRelationships )
            {
                Assert.IsNotNull( consultingRelationship, "Consulting Physician relationship does not exist" );
                Assert.AreEqual( consultingRelationship.Physician.FirstName, PHYSICIAN1_FIRST_NAME,
                                 "Consulting Physician's first name does not match" );
                Assert.AreEqual( consultingRelationship.Physician.LastName, PHYSICIAN1_LAST_NAME,
                                 "Consulting Physician's last name does not match" );
            }
            #endregion

            Assert.AreEqual( 10, 
                             this.AccountProxy.AllPhysicianRelationships.Count,
                             "There should be ten physician relationships" );
        }

        [Test()]
        public void ReplaceExistingPhysicianRelationship()
        {
            #region InitializePhysicianRelationships
            //Admitting Physician
            Physician admittingPhysician = new Physician();
            admittingPhysician.FirstName = "Admitting";
            admittingPhysician.LastName = "Physician";
            admittingPhysician.PhysicianNumber = 1L;
            PhysicianRelationship admRelationship = new PhysicianRelationship( 
                PhysicianRole.Admitting(), 
                admittingPhysician );

            //Attending Physician
            Physician attendingPhysician = new Physician();
            attendingPhysician.FirstName = "Attending";
            attendingPhysician.LastName = "Physician";
            attendingPhysician.PhysicianNumber = 2L;
            PhysicianRelationship attRelationship = new PhysicianRelationship( 
                PhysicianRole.Attending(), 
                attendingPhysician );


            //Referring Physician
            Physician referringPhysician = new Physician();
            referringPhysician.FirstName = "Referring";
            referringPhysician.LastName = "Physician";
            referringPhysician.PhysicianNumber = 3L;
            PhysicianRelationship refRelationship = new PhysicianRelationship( 
                PhysicianRole.Referring(), 
                referringPhysician );

            //Operating Physician
            Physician operatingPhysician = new Physician();
            operatingPhysician.FirstName = "Operating";
            operatingPhysician.LastName = "Physician";
            operatingPhysician.PhysicianNumber = 4L;
            PhysicianRelationship oprRelationship = new PhysicianRelationship( 
                PhysicianRole.Operating(), 
                operatingPhysician );

            //PrimaryCare Physician
            Physician primaryCarePhysician = new Physician();
            primaryCarePhysician.FirstName = "PrimaryCare";
            primaryCarePhysician.LastName = "Physician";
            primaryCarePhysician.PhysicianNumber = 5L;
            PhysicianRelationship othRelationship = new PhysicianRelationship( 
                PhysicianRole.PrimaryCare(), 
                primaryCarePhysician );

            //Consulting1 Physician
            Physician consulting1Physician = new Physician();
            consulting1Physician.FirstName = "Consulting1";
            consulting1Physician.LastName = "Physician";
            consulting1Physician.PhysicianNumber = 6L;
            PhysicianRelationship con1Relationship = new PhysicianRelationship( 
                PhysicianRole.Consulting(), 
                consulting1Physician );

            //Consulting2 Physician
            Physician consulting2Physician = new Physician();
            consulting2Physician.FirstName = "Consulting2";
            consulting2Physician.LastName = "Physician";
            consulting2Physician.PhysicianNumber = 7L;
            PhysicianRelationship con2Relationship = new PhysicianRelationship( 
                PhysicianRole.Consulting(), 
                consulting2Physician );

            //Consulting3 Physician
            Physician consulting3Physician = new Physician();
            consulting3Physician.FirstName = "Consulting3";
            consulting3Physician.LastName = "Physician";
            consulting3Physician.PhysicianNumber = 8L;
            PhysicianRelationship con3Relationship = new PhysicianRelationship( 
                PhysicianRole.Consulting(), 
                consulting3Physician );

            //Consulting4 Physician
            Physician consulting4Physician = new Physician();
            consulting4Physician.FirstName = "Consulting4";
            consulting4Physician.LastName = "Physician";
            consulting4Physician.PhysicianNumber = 9L;
            PhysicianRelationship con4Relationship = new PhysicianRelationship( 
                PhysicianRole.Consulting(), 
                consulting4Physician );

            //Consulting5 Physician
            Physician consulting5Physician = new Physician();
            consulting5Physician.FirstName = "Consulting5";
            consulting5Physician.LastName = "Physician";
            consulting5Physician.PhysicianNumber = 10L;
            PhysicianRelationship con5Relationship = new PhysicianRelationship( 
                PhysicianRole.Consulting(), 
                consulting5Physician );

            //New Admitting Physician
            Physician admittingPhysicianNew = new Physician();
            admittingPhysicianNew.FirstName = "NewAdmitting";
            admittingPhysicianNew.LastName = "Physician";
            admittingPhysicianNew.PhysicianNumber = 11L;
            PhysicianRelationship newAdmRelationship = new PhysicianRelationship( 
                PhysicianRole.Admitting(), 
                admittingPhysicianNew );

            //New Attending Physician
            Physician attendingPhysicianNew = new Physician();
            attendingPhysicianNew.FirstName = "NewAttending";
            attendingPhysicianNew.LastName = "Physician";
            attendingPhysicianNew.PhysicianNumber = 12L;
            PhysicianRelationship newAttRelationship = new PhysicianRelationship( 
                PhysicianRole.Attending(), 
                attendingPhysicianNew );

            //New Referring Physician
            Physician referringPhysicianNew = new Physician();
            referringPhysicianNew.FirstName = "NewReferring";
            referringPhysicianNew.LastName = "Physician";
            referringPhysicianNew.PhysicianNumber = 13L;
            PhysicianRelationship newRefRelationship = new PhysicianRelationship( 
                PhysicianRole.Referring(), 
                referringPhysicianNew );

            //New Operating Physician
            Physician operatingPhysicianNew = new Physician();
            operatingPhysicianNew.FirstName = "NewOperating";
            operatingPhysicianNew.LastName = "Physician";
            operatingPhysicianNew.PhysicianNumber = 14L;
            PhysicianRelationship newOprRelationship = new PhysicianRelationship( 
                PhysicianRole.Operating(), 
                operatingPhysicianNew );

            //New PrimaryCare Physician
            Physician primaryCarePhysicianNew = new Physician();
            primaryCarePhysicianNew.FirstName = "NewOther";
            primaryCarePhysicianNew.LastName = "Physician";
            primaryCarePhysicianNew.PhysicianNumber = 15L;
            PhysicianRelationship newOthRelationship = new PhysicianRelationship( 
                PhysicianRole.PrimaryCare(), 
                primaryCarePhysicianNew );

            //New Consulting Physician
            Physician consultingPhysicianNew = new Physician();
            consultingPhysicianNew.FirstName = "NewConsulting";
            consultingPhysicianNew.LastName = "Physician";
            consultingPhysicianNew.PhysicianNumber = 16L;
            PhysicianRelationship newConRelationship = new PhysicianRelationship( 
                PhysicianRole.Consulting(), 
                consultingPhysicianNew );
            #endregion

            #region AddIntialRelationships
            this.AccountProxy.AddPhysicianRelationship( admRelationship );
            this.AccountProxy.AddPhysicianRelationship( attRelationship );
            this.AccountProxy.AddPhysicianRelationship( refRelationship );
            this.AccountProxy.AddPhysicianRelationship( oprRelationship );
            this.AccountProxy.AddPhysicianRelationship( othRelationship );
            this.AccountProxy.AddPhysicianRelationship( con1Relationship );
            this.AccountProxy.AddPhysicianRelationship( con2Relationship );
            this.AccountProxy.AddPhysicianRelationship( con3Relationship );
            this.AccountProxy.AddPhysicianRelationship( con4Relationship );
            this.AccountProxy.AddPhysicianRelationship( con5Relationship );

            Assert.AreEqual( 10, 
                             this.AccountProxy.AllPhysicianRelationships.Count,
                             "There should be ten physician relationships" );
            #endregion

            #region Test and Replace Admitting Relationship
            admRelationship = this.AccountProxy.PhysicianRelationshipWithRole( 
                PhysicianRole.Admitting().Role() );

            Assert.IsNotNull( admRelationship.Physician, "Admitting Physician does not exist" );
            Assert.IsNotNull( admRelationship.PhysicianRole,
                              "Admitting Physician Role does not exist" );
            Assert.AreEqual( admRelationship.Physician.FirstName, "Admitting",
                             "Admitting Physician's first name does not match" );
            Assert.AreEqual( admRelationship.Physician.LastName, "Physician", 
                             "Admitting Physician's last name does not match" );
            Assert.AreEqual( admRelationship.Physician.PhysicianNumber, 1L,
                             "Admitting Physician's number does not match" );
            Assert.AreEqual( PhysicianRole.Admitting().Role(), admRelationship.PhysicianRole.Role(),
                             "Admitting Physician Roles do not match" );

            this.AccountProxy.AddPhysicianRelationship( newAdmRelationship );

            newAdmRelationship = this.AccountProxy.PhysicianRelationshipWithRole( 
                PhysicianRole.Admitting().Role() );

            Assert.IsNotNull( newAdmRelationship.Physician,
                              "New Admitting Physician does not exist" );
            Assert.IsNotNull( newAdmRelationship.PhysicianRole,
                              "New Admitting Physician Role does not exist" );
            Assert.AreEqual( newAdmRelationship.Physician.FirstName, "NewAdmitting",
                             "New Admitting Physician's first name does not match" );
            Assert.AreEqual( newAdmRelationship.Physician.LastName, "Physician",
                             "New Admitting Physician's last name does not match" );
            Assert.AreEqual( newAdmRelationship.Physician.PhysicianNumber, 11L,
                             "New Admitting Physician's number does not match" );
            Assert.AreEqual( PhysicianRole.Admitting().Role(), newAdmRelationship.PhysicianRole.Role(),
                             "New Admitting Physician Roles do not match" );

            Assert.AreEqual( 10, 
                             this.AccountProxy.AllPhysicianRelationships.Count,
                             "There should be ten physician relationships" );
            #endregion

            #region Test and Replace Attending Relationship
            attRelationship = this.AccountProxy.PhysicianRelationshipWithRole( 
                PhysicianRole.Attending().Role() );

            Assert.IsNotNull( attRelationship.Physician,
                              "Attending Physician does not exist" );
            Assert.IsNotNull( attRelationship.PhysicianRole,
                              "Attending Physician Role does not exist" );
            Assert.AreEqual( attRelationship.Physician.FirstName, "Attending",
                             "Attending Physician's first name does not match" );
            Assert.AreEqual( attRelationship.Physician.LastName, "Physician",
                             "Attending Physician's last name does not match" );
            Assert.AreEqual( attRelationship.Physician.PhysicianNumber, 2L,
                             "Attending Physician's number does not match" );
            Assert.AreEqual( PhysicianRole.Attending().Role(), attRelationship.PhysicianRole.Role(),
                             "Attending Physician Roles do not match" );

            this.AccountProxy.AddPhysicianRelationship( newAttRelationship );

            newAttRelationship = this.AccountProxy.PhysicianRelationshipWithRole( 
                PhysicianRole.Attending().Role() );

            Assert.IsNotNull( newAttRelationship.Physician,
                              "New Attending Physician does not exist" );
            Assert.IsNotNull( newAttRelationship.PhysicianRole,
                              "New Attending Physician Role does not exist" );
            Assert.AreEqual( newAttRelationship.Physician.FirstName, "NewAttending",
                             "New Attending Physician's first name does not match" );
            Assert.AreEqual( newAttRelationship.Physician.LastName, "Physician",
                             "New Attending Physician's last name does not match" );
            Assert.AreEqual( newAttRelationship.Physician.PhysicianNumber, 12L,
                             "New Attending Physician's number does not match" );
            Assert.AreEqual( PhysicianRole.Attending().Role(), newAttRelationship.PhysicianRole.Role(),
                             "New Attending Physician Roles do not match" );

            Assert.AreEqual( 10, 
                             this.AccountProxy.AllPhysicianRelationships.Count,
                             "There should be ten physician relationships" );
            #endregion

            #region Test and Replace Referring Relationship
            refRelationship = this.AccountProxy.PhysicianRelationshipWithRole( 
                PhysicianRole.Referring().Role() );

            Assert.IsNotNull( refRelationship.Physician, "Referring Physician does not exist" );
            Assert.IsNotNull( refRelationship.PhysicianRole,
                              "Referring Physician Role does not exist" );
            Assert.AreEqual( refRelationship.Physician.FirstName, "Referring",
                             "Referring Physician's first name does not match" );
            Assert.AreEqual( refRelationship.Physician.LastName, "Physician",
                             "Referring Physician's last name does not match" );
            Assert.AreEqual( refRelationship.Physician.PhysicianNumber, 3L,
                             "Referring Physician's number does not match" );
            Assert.AreEqual( PhysicianRole.Referring().Role(), refRelationship.PhysicianRole.Role(),
                             "Referring Physician Roles do not match" );

            this.AccountProxy.AddPhysicianRelationship( newRefRelationship );

            newRefRelationship = this.AccountProxy.PhysicianRelationshipWithRole( 
                PhysicianRole.Referring().Role() );

            Assert.IsNotNull( newRefRelationship.Physician,
                              "New Referring Physician does not exist" );
            Assert.IsNotNull( newRefRelationship.PhysicianRole,
                              "New Referring Physician Role does not exist" );
            Assert.AreEqual( newRefRelationship.Physician.FirstName, "NewReferring",
                             "New Referring Physician's first name does not match" );
            Assert.AreEqual( newRefRelationship.Physician.LastName, "Physician",
                             "New Referring Physician's last name does not match" );
            Assert.AreEqual( newRefRelationship.Physician.PhysicianNumber, 13L,
                             "New Referring Physician's number does not match" );
            Assert.AreEqual( PhysicianRole.Referring().Role(), newRefRelationship.PhysicianRole.Role(),
                             "New Referring Physician Roles do not match" );

            Assert.AreEqual( 10, 
                             this.AccountProxy.AllPhysicianRelationships.Count,
                             "There should be ten physician relationships" );
            #endregion

            #region Test and Replace Operating Relationship
            oprRelationship = this.AccountProxy.PhysicianRelationshipWithRole( 
                PhysicianRole.Operating().Role() );

            Assert.IsNotNull( oprRelationship.Physician,
                              "Operating Physician does not exist" );
            Assert.IsNotNull( oprRelationship.PhysicianRole,
                              "Operating Physician Role does not exist" );
            Assert.AreEqual( oprRelationship.Physician.FirstName, "Operating",
                             "Operating Physician's first name does not match" );
            Assert.AreEqual( oprRelationship.Physician.LastName, "Physician",
                             "Operating Physician's last name does not match" );
            Assert.AreEqual( oprRelationship.Physician.PhysicianNumber, 4L,
                             "Operating Physician's number does not match" );
            Assert.AreEqual( PhysicianRole.Operating().Role(), oprRelationship.PhysicianRole.Role(),
                             "Operating Physician Roles do not match" );

            this.AccountProxy.AddPhysicianRelationship( newOprRelationship );

            newOprRelationship = this.AccountProxy.PhysicianRelationshipWithRole( 
                PhysicianRole.Operating().Role() );

            Assert.IsNotNull( newOprRelationship.Physician,
                              "New Operating Physician does not exist" );
            Assert.IsNotNull( newOprRelationship.PhysicianRole,
                              "New Operating Physician Role does not exist" );
            Assert.AreEqual( newOprRelationship.Physician.FirstName, "NewOperating",
                             "New Operating Physician's first name does not match" );
            Assert.AreEqual( newOprRelationship.Physician.LastName, "Physician",
                             "New Operating Physician's last name does not match" );
            Assert.AreEqual( newOprRelationship.Physician.PhysicianNumber, 14L,
                             "New Operating Physician's number does not match" );
            Assert.AreEqual( PhysicianRole.Operating().Role(), newOprRelationship.PhysicianRole.Role(),
                             "New Operating Physician Roles do not match" );

            Assert.AreEqual( 10, 
                             this.AccountProxy.AllPhysicianRelationships.Count,
                             "There should be ten physician relationships" );
            #endregion

            #region Test and Replace PrimaryCare Relationship
            othRelationship = this.AccountProxy.PhysicianRelationshipWithRole( 
                PhysicianRole.PrimaryCare().Role() );

            Assert.IsNotNull( othRelationship.Physician,
                              "PrimaryCare Physician does not exist" );
            Assert.IsNotNull( othRelationship.PhysicianRole,
                              "PrimaryCare Physician Role does not exist" );
            Assert.AreEqual( othRelationship.Physician.FirstName, "PrimaryCare",
                             "PrimaryCare Physician's first name does not match" );
            Assert.AreEqual( othRelationship.Physician.LastName, "Physician",
                             "PrimaryCare Physician's last name does not match" );
            Assert.AreEqual( othRelationship.Physician.PhysicianNumber, 5L,
                             "PrimaryCare Physician's number does not match" );
            Assert.AreEqual( PhysicianRole.PrimaryCare().Role(), othRelationship.PhysicianRole.Role(),
                             "PrimaryCare Physician Roles do not match" );

            this.AccountProxy.AddPhysicianRelationship( newOthRelationship );

            newOthRelationship = this.AccountProxy.PhysicianRelationshipWithRole( 
                PhysicianRole.PrimaryCare().Role() );

            Assert.IsNotNull( newOthRelationship.Physician,
                              "New PrimaryCare Physician does not exist" );
            Assert.IsNotNull( newOthRelationship.PhysicianRole,
                              "New PrimaryCare Physician Role does not exist" );
            Assert.AreEqual( newOthRelationship.Physician.FirstName, "NewOther",
                             "New PrimaryCare Physician's first name does not match" );
            Assert.AreEqual( newOthRelationship.Physician.LastName, "Physician",
                             "New PrimaryCare Physician's last name does not match" );
            Assert.AreEqual( newOthRelationship.Physician.PhysicianNumber, 15L,
                             "New PrimaryCare Physician's number does not match" );
            Assert.AreEqual( PhysicianRole.PrimaryCare().Role(), newOthRelationship.PhysicianRole.Role(),
                             "New PrimaryCare Physician Roles do not match" );

            Assert.AreEqual( 10, 
                             this.AccountProxy.AllPhysicianRelationships.Count,
                             "There should be ten physician relationships" );
            #endregion

            #region Test and Replace Consulting Relationship
            ArrayList conRelationships = (ArrayList)this.AccountProxy.PhysicianRelationshipsWith( 
                                                        PhysicianRole.Consulting().Role() );

            Assert.AreEqual( MAX_CONSULT_PHYS, 
                             conRelationships.Count,
                             "There should be five consulting physician relationships" );
            long physicianNumber = 1L;

            foreach( PhysicianRelationship conRelationship in conRelationships )
            {
                Assert.IsNotNull( conRelationship, "Consulting Physician relationship does not exist" );
                Assert.AreEqual( conRelationship.Physician.FirstName, string.Concat( "Consulting", physicianNumber.ToString() ),
                                 "Consulting Physician's first name does not match" );
                Assert.AreEqual( conRelationship.Physician.PhysicianNumber, physicianNumber + 5L,
                                 "Consulting Physician's number does not match" );
                physicianNumber++;
            }

            conRelationships.Clear();
            this.AccountProxy.AddPhysicianRelationship( newConRelationship );
            conRelationships = (ArrayList)this.AccountProxy.PhysicianRelationshipsWith( 
                                              PhysicianRole.Consulting().Role() );

            Assert.AreEqual( MAX_CONSULT_PHYS, 
                             conRelationships.Count,
                             "There should be five consulting physician relationships" );

            Physician newConsultingPhysician = null;
            foreach( PhysicianRelationship conRelationship in conRelationships )
            {
                if( conRelationship.Physician.FirstName.Equals( "NewConsulting" ) &&
                    conRelationship.Physician.PhysicianNumber == 16L )
                {
                    newConsultingPhysician = conRelationship.Physician;
                    break;
                }
            }

            Assert.IsNotNull( newConsultingPhysician,
                              "New Consulting Physician does not exist" );
            Assert.AreEqual( newConsultingPhysician.FirstName, "NewConsulting",
                             "New Consulting Physician's first name does not match" );
            Assert.AreEqual( newConsultingPhysician.PhysicianNumber, 16L,
                             "New Consulting Physician's number does not match" );

            Assert.AreEqual( 10, 
                             this.AccountProxy.AllPhysicianRelationships.Count,
                             "There should be ten physician relationships" );
            #endregion
        }

        [Test()]
        public void TestLegends()
        {
            AccountProxy ap = new AccountProxy();
            ap.AccountNumber = 30015;
            ap.Facility = i_facilityBroker.FacilityWith(900);

            ap.OptOutInformation = "YNNNN";
            ap.Confidential = string.Empty;
            string legend = ap.AddOnlyLegends();
            Assert.AreEqual( "[nhlr]", legend, "Wrong legends generated" );

            ap.OptOutInformation = "YYNNN";
            legend = ap.AddOnlyLegends();
            Assert.AreEqual( "[hlr]", legend, "Wrong legends generated" );
            ap.Confidential = "C";
            legend = ap.AddOnlyLegends();
            Assert.AreEqual( "*[hlr]", legend, "Wrong legends generated" );
        }
        #endregion

        #region Support Methods
        private IPhysicianBroker IPhysicianBroker
        {
            get
            {
                return i_PhysicianBroker;
            }
        }

        private AccountProxy AccountProxy
        {
            get
            {
                return i_AccountProxy;
            }
        }
        #endregion

        #region Data Elements
        private static  IFacilityBroker         i_facilityBroker;
        private static  IWorklistSettingsBroker i_worklistBroker;
        private static  AccountProxy            i_AccountProxy;
        private static  IPhysicianBroker        i_PhysicianBroker;
        #endregion
    }
}