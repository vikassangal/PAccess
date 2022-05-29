using System;
using System.Collections;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    /// <summary>
    /// Summary description for BenefitsCategoryPBARBrokerTests.
    /// </summary>

    //TODO: Create XML summary comment for BenefitsCategoryPBARBrokerTests
    [TestFixture()]
    public class BenefitsCategoryPBARBrokerTests : AbstractBrokerTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown BenefitsCategoryPBARBrokerTests
        [TestFixtureSetUp()]
        public static void SetUpBenefitsCategoryPBARBrokerTests()
        {
            IFacilityBroker fb = BrokerFactory.BrokerOfType<IFacilityBroker>();
            i_ACOFacility = fb.FacilityWith( ACO_FACILITYID );
            i_bcBroker = BrokerFactory.BrokerOfType<IBenefitsCategoryBroker>();
        }

        [TestFixtureTearDown()]
        public static void TearDownBenefitsCategoryPBARBrokerTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestGetAllBenefitsCategory()
        {
            ICollection benefits = i_bcBroker.AllBenefitsCategories( ACO_FACILITYID );
            this.ValidateResults( benefits );
        }       

        [Test()]
        public void TestGetCategoriesForHSV()
        {
            ArrayList list = (ArrayList)i_bcBroker.BenefitsCategoriesFor( i_ACOFacility, "VE" );

            Assert.IsNotNull( list, "No list returned for BenefitsCategoriesFor(VE)" );
            Assert.IsTrue( list.Count == 1, "Wrong Number of Categories found" );
            BenefitsCategory bc = (BenefitsCategory)list[0];
            Assert.AreEqual( "SNF/Subacute", bc.Description, "Wrong description found" );

            list = (ArrayList)i_bcBroker.BenefitsCategoriesFor( i_ACOFacility, "16" );

            Assert.IsNotNull( list, "No list returned for BenefitsCategoriesFor(16)" );
            Assert.IsTrue( list.Count == 2, "Wrong Number of Categories found" );
        }

        [Test()]
        public void TestGetCategoriesWith()
        {
            BenefitsCategory benefitsCategory = new BenefitsCategory();
            benefitsCategory  = i_bcBroker.BenefitsCategoryWith( ACO_FACILITYID, 1 );

            Assert.AreEqual( "Inpatient" , benefitsCategory.Description, "the description should be Inpatient" );
        }

        [Test()]
        public void TestGetCategoriesForBlank()
        {
            string blank = String.Empty;
            ICollection list = i_bcBroker.BenefitsCategoriesFor( i_ACOFacility, blank );

            Assert.AreEqual( list.Count, 0 , "No list found for blank code" );
        }
        #endregion

        #region Support Methods
        private void ValidateResults( ICollection benefits )
        {
            Assert.IsNotNull( benefits, "No list of Categories found" );
            Assert.IsTrue( benefits.Count > 0, "No Categories found" );

            BenefitsCategory bc = null;
            BenefitsCategory badBc = null;
            foreach( BenefitsCategory lbc in benefits )
            {
                if( lbc.Oid == 6 )
                {
                    bc = lbc;
                }
                if( lbc.Oid == 90953 )
                {
                    badBc = lbc;
                }
            }
            Assert.IsNotNull( bc, "Did not find Benefits category with ID = 6" );
            Assert.IsTrue( bc.Description == "Psych IP", "Incorrect Description found" );

            Assert.IsNull( badBc, "Should not have found BenefitsCategory 90953" );
        }
        #endregion

        #region Data Elements
        private static  IBenefitsCategoryBroker i_bcBroker;
        private static  Facility i_ACOFacility;
        #endregion
    }
}