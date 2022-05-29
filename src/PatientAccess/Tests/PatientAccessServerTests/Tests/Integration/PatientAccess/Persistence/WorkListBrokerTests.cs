using System;
using System.Collections;
using System.Reflection;
using Extensions.UI.Builder;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    [TestFixture()]
    public class WorklistBrokerTests : AbstractBrokerTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown WorklistBrokerTests
        [TestFixtureSetUp()]
        public static void SetUpWorklistBrokerTests()
        {
            i_WorklistBroker = BrokerFactory.BrokerOfType<IWorklistSettingsBroker>();
            i_facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
        }

        [TestFixtureTearDown()]
        public static void TearDownWorklistBrokerTests()
        {
        }

        [SetUp()]
        public void SetUpWorklist()
        {
            WorklistSelectionRange range = i_WorklistBroker.WorklistSelectionRangeWith( 9 );
            WorklistSettings ws = new WorklistSettings( "F", "M",
                                                        DateTime.Now, DateTime.Now,
                                                        range, 1, 1);
            i_User = User.GetCurrent();
            i_User.Oid = 1;
            i_User.SecurityUser = new Extensions.SecurityService.Domain.User();
            i_User.SecurityUser.TenetID = 1;

            i_WorklistBroker.SavePreRegWorklistSettings( i_User.SecurityUser.TenetID, ws );
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestSaveEDSettings()
        {
            WorklistSelectionRange range = i_WorklistBroker.WorklistSelectionRangeWith( 9 );
            WorklistSettings ws = new WorklistSettings( "C", "G",
                                                        DateTime.Now, DateTime.Now,
                                                        range, 1, 1 );
            i_WorklistBroker.SaveEmergencyDepartmentWorklistSettings( i_User.SecurityUser.TenetID, ws );
        }

        [Test()]
        public void TestSaveIVSettings()
        {
            WorklistSelectionRange range = i_WorklistBroker.WorklistSelectionRangeWith( 9 );
            WorklistSettings ws = new WorklistSettings( "C", "G",
                                                        DateTime.Now, DateTime.Now,
                                                        range, 1, 1 );
            i_WorklistBroker.SaveInsuranceVerificationWorklistSettings( i_User.SecurityUser.TenetID, ws );
        }

        [Test()]
        public void TestSaveNoShowSettings()
        {
            WorklistSelectionRange range = i_WorklistBroker.WorklistSelectionRangeWith( 9 );
            WorklistSettings ws = new WorklistSettings( "C", "G",
                                                        DateTime.Now, DateTime.Now,
                                                        range, 1, 1 );
            i_WorklistBroker.SaveNoShowWorklistSettings( i_User.SecurityUser.TenetID, ws );
        }

        [Test()]
        public void TestSavePLSettings()
        {
            WorklistSelectionRange range = i_WorklistBroker.WorklistSelectionRangeWith( 1 );
            WorklistSettings ws = new WorklistSettings("C", "G",
                                                       DateTime.Now, DateTime.Now,
                                                       range, 1, 1);

            i_WorklistBroker.SavePatientLiabilityWorklistSettings( i_User.SecurityUser.TenetID, ws );
        }

        [Test()]
        public void TestSavePostRegSettings()
        {
            WorklistSelectionRange range = i_WorklistBroker.WorklistSelectionRangeWith( 1 );
            WorklistSettings ws = new WorklistSettings( "A", "B",
                                                        DateTime.Now, DateTime.Now,
                                                        range, 1, 1);
            i_WorklistBroker.SavePostRegWorklistSettings( i_User.SecurityUser.TenetID, ws );
        }

        [Test()]
        public void TestSavePreRegSettings()
        {
            WorklistSelectionRange range = i_WorklistBroker.WorklistSelectionRangeWith( 1 );
            WorklistSettings ws = new WorklistSettings( "A", "B",
                                                        DateTime.Now, DateTime.Now,
                                                        range, 1, 1);
            i_WorklistBroker.SavePreRegWorklistSettings( i_User.SecurityUser.TenetID, ws );
        }

        [Test()]
        public void TestSaveSettingsNullDates()
        {
            WorklistSelectionRange range = i_WorklistBroker.WorklistSelectionRangeWith( 1 );
            WorklistSettings ws = new WorklistSettings("A","B",
                                                       new DateTime(), new DateTime(),
                                                       range, 1, 1);
            User u = User.GetCurrent();
            u.Oid = 3;
            u.SecurityUser = new Extensions.SecurityService.Domain.User();
            u.SecurityUser.TenetID = 3;
            i_WorklistBroker.SavePreRegWorklistSettings( u.SecurityUser.TenetID, ws );
        }
 
        [Test()]
        public void TestPreRegSelectionRanges()
        {
            ArrayList list = i_WorklistBroker.GetPreRegWorklistRanges();
            Assert.IsNotNull(list, "Did not receive any selection ranges");
            Assert.AreEqual(6, list.Count, "Wrong number of ranges found");

            WorklistSelectionRange range = (WorklistSelectionRange)list[0];
            Assert.AreEqual("All", range.Description, "Invalid range description");
            Assert.AreEqual(8, range.Oid, "Invalid range id");
        }

        [Test()]
        public void TestPostRegSelectionRanges()
        {
            ArrayList list = i_WorklistBroker.GetPostRegWorklistRanges();
            foreach( WorklistSelectionRange r  in list )
            {
                Console.WriteLine(r.Description);
            }
            Assert.IsNotNull(list, "Did not receive any selection ranges");
            Assert.AreEqual(6, list.Count, "Wrong number of ranges found");

            WorklistSelectionRange range = (WorklistSelectionRange)list[0];
            Assert.AreEqual("All", range.Description, "Invalid range description");
            Assert.AreEqual(8, range.Oid, "Invalid range id");
        }

        [Test()]
        public void TestInsuranceSelectionRanges()
        {
            ArrayList list = i_WorklistBroker.GetInsuranceVerificationWorklistRanges();
            foreach( WorklistSelectionRange r  in list )
            {
                Console.WriteLine(r.Description);
            }
            Assert.IsNotNull(list, "Did not receive any selection ranges");
            Assert.AreEqual(9, list.Count, "Wrong number of ranges found");

            WorklistSelectionRange range = (WorklistSelectionRange)list[0];
            Assert.AreEqual("All", range.Description, "Invalid range description");
            Assert.AreEqual(8, range.Oid, "Invalid range id");
        }

        [Test()]
        public void TestEDWorklistSettings()
        {
            WorklistSettings ws = i_WorklistBroker.EmergencyDepartmentWorklistSettings( i_User.SecurityUser.TenetID );
            Assert.IsNotNull(ws, "No WorklistSetting found");
            Assert.AreEqual("C", ws.BeginningWithLetter, "Wrong StartLetters found");
            Assert.AreEqual("G", ws.EndingWithLetter, "Wrong EndingLetters found");
            Assert.AreEqual(9, ws.WorklistSelectionRange.Oid, "Wrong RangeType found");
            Assert.AreEqual(1, ws.SortedColumn);
        }

        [Test()]
        public void TestIVWorklistSettings()
        {
            User u = User.GetCurrent();
            u.Facility = null;
            u.SecurityUser = new Extensions.SecurityService.Domain.User();
            u.SecurityUser.TenetID = 9999999;
            u.Oid = 9999999;

            WorklistSettings ws = i_WorklistBroker.InsuranceVerificationWorklistSettings( u.SecurityUser.TenetID );
            Assert.IsNotNull(ws, "No WorklistSetting found");
            Assert.AreEqual("A", ws.BeginningWithLetter, "Wrong StartLetters found");
            Assert.AreEqual("Z", ws.EndingWithLetter, "Wrong EndingLetters found");
            Assert.AreEqual(8, ws.WorklistSelectionRange.Oid, "Wrong RangeType found");
            Assert.AreEqual(3, ws.SortedColumn);
        }

        [Test()]
        public void TestNoShowWorklistSettings()
        {
            User u = User.GetCurrent();
            u.Facility = null;
            u.SecurityUser = new Extensions.SecurityService.Domain.User();
            u.SecurityUser.TenetID = 9999999;
            u.Oid = 9999999;

            WorklistSettings ws = i_WorklistBroker.NoShowWorklistSettings( u.SecurityUser.TenetID );
            Assert.IsNotNull(ws, "No WorklistSetting found");
            Assert.AreEqual("A", ws.BeginningWithLetter, "Wrong StartLetters found");
            Assert.AreEqual("Z", ws.EndingWithLetter, "Wrong EndingLetters found");
            Assert.AreEqual(5, ws.WorklistSelectionRange.Oid, "Wrong RangeType found");
            Assert.AreEqual(3, ws.SortedColumn);
        }

        [Test()]
        public void TestPLWorklistSettings()
        {
            User u = User.GetCurrent();
            u.Facility = null;
            u.SecurityUser = new Extensions.SecurityService.Domain.User();
            u.SecurityUser.TenetID = 9999999;
            u.Oid = 9999999;

            WorklistSettings ws = i_WorklistBroker.PatientLiabilityWorklistSettings( u.SecurityUser.TenetID );
            Assert.IsNotNull(ws, "No WorklistSetting found");
            Assert.AreEqual("A", ws.BeginningWithLetter, "Wrong StartLetters found");
            Assert.AreEqual("Z", ws.EndingWithLetter, "Wrong EndingLetters found");
            Assert.AreEqual(8, ws.WorklistSelectionRange.Oid, "Wrong RangeType found");
            Assert.AreEqual(3, ws.SortedColumn);
        }

        [Test()]
        public void TestPostRegWorklistSettings()
        {
            User u = User.GetCurrent();
            u.Facility = null;
            u.SecurityUser = new Extensions.SecurityService.Domain.User();
            u.SecurityUser.TenetID = 9999999;
            u.Oid = 9999999;

            WorklistSettings ws = i_WorklistBroker.PatientLiabilityWorklistSettings( u.SecurityUser.TenetID );
            Assert.IsNotNull(ws, "No WorklistSetting found");
            Assert.AreEqual("A", ws.BeginningWithLetter, "Wrong StartLetters found");
            Assert.AreEqual("Z", ws.EndingWithLetter, "Wrong EndingLetters found");
            Assert.AreEqual(8, ws.WorklistSelectionRange.Oid, "Wrong RangeType found");
            Assert.AreEqual(3, ws.SortedColumn);
        }
        
        [Test()]
        public void TestPreRegWorklistSettings()
        {
            User u = User.GetCurrent();
            u.Facility = null;
            u.SecurityUser = new Extensions.SecurityService.Domain.User();
            u.SecurityUser.TenetID = 9999999;
            u.Oid = 9999999;

            WorklistSettings ws = i_WorklistBroker.PreRegWorklistSettings( u.SecurityUser.TenetID );
            Assert.IsNotNull(ws, "No WorklistSetting found");
            Assert.AreEqual("A", ws.BeginningWithLetter, "Wrong StartLetters found");
            Assert.AreEqual("Z", ws.EndingWithLetter, "Wrong EndingLetters found");
            Assert.AreEqual(3, ws.WorklistSelectionRange.Oid, "Wrong RangeType found");
            Assert.AreEqual(3, ws.SortedColumn);
        }

        [Test()]
        public void TestNotPresentWorklistSettings()
        {
            User u = User.GetCurrent();
            u.Facility = null;
            u.Oid = -1;
            u.SecurityUser = new Extensions.SecurityService.Domain.User();
            u.SecurityUser.TenetID = -1;

            WorklistSettings ws = i_WorklistBroker.EmergencyDepartmentWorklistSettings( u.SecurityUser.TenetID );
            Assert.IsNull(ws, "Should not have found a WorklistSetting");
        }        

        [Test()]
        public void TestWorklists()
        {
            Worklist wl = i_WorklistBroker.WorklistWith( 1 );
            Assert.IsNotNull( wl, "Can not find worklist 1");
            Assert.AreEqual( wl.Description, "Preregistration", "Worklist name is not correct");

            wl = i_WorklistBroker.PostRegWorklist();
            Assert.AreEqual(2,wl.Oid);
            Assert.AreEqual("Postregistration", wl.Description);

            wl = i_WorklistBroker.InsuranceVerificationWorklist();
            Assert.AreEqual(3,wl.Oid);
            Assert.AreEqual("Insurance Verification",wl.Description);

            wl = i_WorklistBroker.PatientLiabilityWorklist();
            Assert.AreEqual(4,wl.Oid);
            Assert.AreEqual("Patient Liability",wl.Description);

            wl = i_WorklistBroker.EmergencyDepartmentWorklist();
            Assert.AreEqual(5,wl.Oid);
            Assert.AreEqual( "Pre-MSE",wl.Description );

            wl = i_WorklistBroker.NoShowWorklist();
            Assert.AreEqual( 6,wl.Oid );
            Assert.AreEqual( "No Show",wl.Description );

        }

        [Test()]
        public void TestAllWorklists()
        {
            ArrayList list = i_WorklistBroker.GetAllWorkLists();
            Assert.IsNotNull( list, "No worklists found" );
            Assert.IsTrue( list.Count > 0, "No worklists found" );
            Console.WriteLine(((Worklist)list[0]).Description);
        }

        [Test()]
        public void TestActionCreation()
        {
            IAction action = null;
            try
            {
                Assembly a = Assembly.Load("PatientAccess.Common");

                Type actionType = a.GetType("PatientAccess.Actions.ProvideEthnicity");
                action = Activator.CreateInstance(actionType) as IAction;
            }
            catch(Exception ex)
            {
                string s = ex.Message;
            }

            Assert.IsNotNull( action, "Can not create action" );
        }

        [Test()]
        [Ignore()] //"Re implement once test accounts are established"
        public void TestRemainingActionsForPreRegWorklist()
        {
            AccountProxy ap = new AccountProxy();
            ap.AccountNumber = 30015;
            ap.Facility = i_facilityBroker.FacilityWith(900);
            ap.ActionsLoader = new ActionLoader(ap);

            Worklist wl = i_WorklistBroker.WorklistWith( 1 );
            ActionsList actions = (ActionsList)ap.GetRemainingActionsFor(wl);
            Assert.IsNotNull( actions, "Can not get remaining actions" );
            Assert.AreEqual( 1, actions.Count, "Wrong number of actions" );

            foreach( IAction action in actions )
            {
                if ( action.Context == null )
                    Assert.Fail("Did not find context for actions");
            }
            Console.WriteLine("Count = " + ap.GetCountOfAllRemainingActions());
        }
        
        [Test()]
        [Ignore()] //"IBM ADO.NET provider error"
        public void TestRemainingActionsForPostRegWorklist()
        {
            AccountProxy ap = new AccountProxy();
            ap.AccountNumber = 32110;
            ap.Facility = i_facilityBroker.FacilityWith(900);
            ap.ActionsLoader = new ActionLoader(ap);

            Worklist wl = i_WorklistBroker.WorklistWith( 2 );
            ActionsList actions = (ActionsList)ap.GetRemainingActionsFor(wl);
            Assert.IsNotNull( actions, "Can not get remaining actions" );
            Assert.AreEqual( 1, actions.Count,"Wrong number of actions" );

            foreach( IAction action in actions )
            {
                if ( action.Context == null )
                    Assert.Fail("Did not find context for actions");
            }
        }
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements

        private static IWorklistSettingsBroker         i_WorklistBroker            = null;
        private static IFacilityBroker         i_facilityBroker    = null;
        private static User                    i_User              = null;
        #endregion
    }
}