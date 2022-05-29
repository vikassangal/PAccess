using System;
using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class TransferInToOutActivityTests
    {

        #region Properties

        /// <summary>
        /// Gets or sets the object unber test.
        /// </summary>
        /// <value>The object unber test.</value>
        protected virtual TransferInToOutActivity ObjectUnberTest
        { 
            get
            {

                return this.i_ObjectUnderTest;   

            }
            set
            {

                this.i_ObjectUnderTest = value;

            }

        }

        #endregion Properties

        #region Fields

        TransferInToOutActivity i_ObjectUnderTest;

        #endregion

        #region Constants
        private const string TEST_REMARKS = "Test Remarks for TransferInToOutActivity";
        #endregion

        #region SetUp and TearDown TransferInToOutActivityTests
        [SetUp]
        public void SetUpTransferInToOutActivityTests()
        {
            TransferInToOutActivity activity = new TransferInToOutActivity();
        }

        [TearDown]
        public void TearDownTransferInToOutActivityTests()
        {
        }
        #endregion

        #region Test Methods
        
        [Test]
        public void TestTransferInToOutRemarks_HappyPath()
        {

            TransferInToOutActivity activity = new TransferInToOutActivity();
            activity.Remarks = TEST_REMARKS;

            Assert.AreEqual( TEST_REMARKS, 
                             activity.Remarks, 
                             "TransferInToOutActivity Remarks do not match." );

        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestTransferInToOutRemarks_ThrowNullOnSet()
        {
            
            TransferInToOutActivity activity = new TransferInToOutActivity();
            activity.Remarks = null;

        }

        #endregion

        #region Support Methods
        #endregion

    }
}


//namespace
