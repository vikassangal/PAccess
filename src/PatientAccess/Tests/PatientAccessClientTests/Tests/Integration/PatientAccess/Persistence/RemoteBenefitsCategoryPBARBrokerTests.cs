using System.Collections;
using NUnit.Framework;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;

namespace Tests.Integration.PatientAccess.Persistence
{
    [TestFixture()]
    public class RemoteBenefitsCategoryPBARBrokerTests : RemoteAbstractBrokerTests
    {

        [Test()]
        public void TestBenefitsCategoryBrokerRemote()
        {
            IBenefitsCategoryBroker bcb = BrokerFactory.BrokerOfType<IBenefitsCategoryBroker>();
            Assert.IsNotNull( bcb, "Did not create remote broker" );
            ICollection benefits = bcb.AllBenefitsCategories( ACO_FACILITYID );

            this.ValidateResults( benefits );
        }

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

    }
}