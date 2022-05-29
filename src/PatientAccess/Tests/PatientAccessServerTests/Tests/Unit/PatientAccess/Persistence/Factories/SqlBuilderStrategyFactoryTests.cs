using PatientAccess.Persistence;
using PatientAccess.Persistence.Factories;
using PatientAccess.Persistence.Specialized;
using NUnit.Framework;

namespace Tests.Unit.PatientAccess.Persistence.Factories
{
    /// <summary>
    ///This is a test class for SqlBuilderStrategyFactoryTests and is intended
    ///to contain all SqlBuilderStrategyFactoryTests Unit Tests
    ///</summary>
    [TestFixture]
    [Category( "Fast" )]
    public class SqlBuilderStrategyFactoryTests
    {

        /// <summary>
        ///A test for CreatePatientInsertStrategy
        ///</summary>
        [Test]
        public void TestCreatePatientInsertStrategyShouldReturnSpecialStrategy()
        {
            PatientInsertStrategy actual = SqlBuilderStrategyFactory.CreatePatientInsertStrategy();
            Assert.IsInstanceOf<ClinicalTrialsPatientInsertStrategy>( actual );
        }

    }
}