using System.Collections;
using Extensions.UI.Builder;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    [TestFixture()]
    public class RuleBrokerTests : AbstractBrokerTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown RuleBrokerTests
        [SetUp()]
        public void SetUpRuleBrokerTests( )
        {
            i_RuleBroker = BrokerFactory.BrokerOfType<IRuleBroker>(); 
        }

        [TearDown()]
        public void TearDownRuleBrokerTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestRuleWorklists()
        {
            Hashtable t = i_RuleBroker.RuleWorklistMapping();

            Assert.IsTrue( t.Count > 0, "No Rule to Worklists Found" );
        }

        [Test()]
        public void TestRuleActions()
        {
            Hashtable t = i_RuleBroker.RuleActionMapping();

            Assert.IsTrue( t.Count > 0, "No Rule to Actions Found" );
        }

        [Test()]
        public void TestLoadRules()
        {
            bool found = false;
            Activity activity = new PreMSERegisterActivity();
            ArrayList rules = i_RuleBroker.LoadRules( activity.GetType().ToString() );
            Assert.IsNotNull( rules, "Rules were not loaded for PreMSE Activity" );
            Assert.IsTrue( rules.Count > 0, "No rules were loaded for PreMSE Activity" );

            foreach( LeafRule r in rules )
            {
                if( r.Oid == 82 )
                {
                    found  = true;
                    Assert.AreEqual( r.Description, "Admitting physician", "Expected description is 'Admitting physician'");
                    Assert.AreEqual( r.DisplayString, "PatientAccess.Rules.AdmittingPhysicianRequired", "Expected DisplayString is 'PatientAccess.Rules.AdmittingPhysicianRequired'" );
                }             
            }
            Assert.IsTrue( found, "failed to find AdmittingPhysicianRequired Rule for PreMSE Activity" );        
        }

        [Test()]
        public void TestActionWorklistMapping()
        {
            bool found = false;
            Hashtable mappings = new Hashtable();
            string kindOfVisitCode = VisitType.PREREG_PATIENT;
            string financialClassCode = "02";

            mappings = i_RuleBroker.ActionWorklistMapping( kindOfVisitCode, financialClassCode );
            Assert.IsNotNull( mappings, "We got null collection from ActionWorklistMapping()" );
            Assert.IsTrue( mappings.Count > 0, "We got zero elements collection from ActionWorklistMapping()" );

            ArrayList a;
            string workListName = string.Empty;
            for( int i = 1; i <= mappings.Count; i++ )
            {
                a = (ArrayList)mappings[(long)i];
                if( a != null )
                {
                    workListName = (string)a[1];
                    if( workListName.Equals( "Pre-MSE" ) )
                    {
                        found = true;
                    }
                }
            }
            Assert.IsTrue( found, "Haven't found workList with the name 'Pre-MSE'" );
        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements

        private IRuleBroker i_RuleBroker;
        //private IDbConnection dbConnection;

        #endregion
    }
}