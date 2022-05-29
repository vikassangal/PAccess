using System.Collections;
using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class InformationReceivedSourceTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown InformationReceivedSourceTests
        [TestFixtureSetUp()]
        public static void SetUpInformationReceivedSourceTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownInformationReceivedSourceTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void testInformationReceivedSource()
        {
           
            InformationReceivedSource phone = new InformationReceivedSource(3,ReferenceValue.NEW_VERSION,"phone");
           
            Assert.AreEqual(
                typeof(InformationReceivedSource),
                phone.GetType()
                );
                 
            Assert.AreEqual(
                "phone",
                phone.Description
                );
            Assert.AreEqual(
                3,
                phone.Oid
                );
            InformationReceivedSource systemElectronicVerification = new InformationReceivedSource(3,ReferenceValue.NEW_VERSION,"SystemElectronicVerification");
            InformationReceivedSource otherElectronicVerification = new InformationReceivedSource(3,ReferenceValue.NEW_VERSION,"OtherElectronicVerification");
            ArrayList sources = new ArrayList();
            sources.Add(phone);
            sources.Add(systemElectronicVerification);
            sources.Add(otherElectronicVerification);
            Assert.AreEqual(3,
                            sources.Count                         
                );
            Assert.IsTrue(sources.Contains(systemElectronicVerification) );


  

           
        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        #endregion
    }
}