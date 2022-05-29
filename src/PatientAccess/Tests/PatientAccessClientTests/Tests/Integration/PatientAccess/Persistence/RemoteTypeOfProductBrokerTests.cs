using System.Collections;
using NUnit.Framework;
using PatientAccess.BrokerInterfaces;

namespace Tests.Integration.PatientAccess.Persistence
{
    [TestFixture]
    public class RemoteTypeOfProductBrokerTests
    {
        [Test()]
        public void TestTypeOfProductBrokerRemote()
        {

            ITypeOfProductBroker typeOfProductBroker = BrokerFactory.BrokerOfType<ITypeOfProductBroker>();
            Assert.IsNotNull( typeOfProductBroker, "Could not create remote TypeOfProductBroker" );
            IList allTypeOfProducts = typeOfProductBroker.AllTypeOfProducts();
            Assert.IsNotNull( allTypeOfProducts, "No TypeOfProducts returned" );
            Assert.IsTrue( allTypeOfProducts.Count == 8, "Incorrect number of Type Of Products returned" );

        }

    }
}