using System;
using System.Collections;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    /// <summary>
    /// SpanCodePBARBrokerTests
    /// </summary>
    [TestFixture()]
    public class SpanCodePBARBrokerTests : AbstractBrokerTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown SpanCodePBARBrokerTests
        [TestFixtureSetUp()]
        public static void SetUpSpanCodePBARBrokerTests()
        {
            spanCodeBroker = BrokerFactory.BrokerOfType<ISpanCodeBroker>();
        }

        [TestFixtureTearDown()]
        public static void TearDownSpanCodePBARBrokerTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestAllSpanCodes()
        {
            ICollection list = spanCodeBroker.AllSpans( ACO_FACILITYID );

            Assert.IsNotNull( list, "No spanCode list returned from broker" );
            SpanCode foundSC = null;
            foreach( SpanCode sc in list )
            {
                if( sc.Code.Equals(SNF_LEVEL_OF_CARE ))
                {
                    foundSC = sc;
                }
            }
            Assert.IsNotNull( foundSC, "Did not find Span Code with code of " + SNF_LEVEL_OF_CARE );
            Assert.AreEqual( "SNF LEVEL OF CARE", foundSC.Description );
        }

        [Test()]
        public void TestOneSpanCode()
        {
            SpanCode foundSC = null;
            foundSC = spanCodeBroker.SpanCodeWith( ACO_FACILITYID, SNF_LEVEL_OF_CARE );
            Assert.IsNotNull( foundSC, "Did not find Span Code with code of " + SNF_LEVEL_OF_CARE );
            Assert.AreEqual( "SNF LEVEL OF CARE", foundSC.Description );
        }

        [Test()]
        public void TestSpanCodeForBlank()
        {
            SpanCode spanCode = spanCodeBroker.SpanCodeWith( ACO_FACILITYID, BLANK_SPAN_CODE );

            Assert.AreEqual
                ( BLANK_SPAN_CODE,
                  spanCode.Code,
                  "Code should be blank"
                );
            Assert.IsTrue(
                spanCode.IsValid
                );
        }

        [Test()]
        public void TestSpanCodeForInvalid()
        {
            SpanCode spanCode = spanCodeBroker.SpanCodeWith( ACO_FACILITYID, INVALID_SPAN_CODE );

            Assert.IsFalse( spanCode.IsValid );
        }

        [Test(), ExpectedException( typeof( ArgumentNullException ) )]
        public void TestSpanCodeForNULL()
        {
            SpanCode spanCode = spanCodeBroker.SpanCodeWith( ACO_FACILITYID, null );
        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements

        private static  ISpanCodeBroker spanCodeBroker;

        #endregion

        #region Data Constants
        private const string SNF_LEVEL_OF_CARE = "75";
        private const string INVALID_SPAN_CODE = "60";
        private static readonly string BLANK_SPAN_CODE = string.Empty;
        #endregion

    }
}