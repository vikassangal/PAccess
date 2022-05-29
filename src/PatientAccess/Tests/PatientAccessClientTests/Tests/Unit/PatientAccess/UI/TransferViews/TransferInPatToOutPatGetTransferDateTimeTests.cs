using System;
using NUnit.Framework;
using PatientAccess.UI.TransferViews;

namespace Tests.Unit.PatientAccess.UI.TransferViews
{
    /// <summary>
    /// Summary description for TransferInPatToOutPatGetTransferDateTimeTests
    /// </summary>
    [TestFixture]
    [Category( "Fast" )]
    public class TransferInPatToOutPatGetTransferDateTimeTests
    {
        private const string TRANSFER_DATE = "07072010";
        private const string TRANSFER_TIME = "0010";

        #region Test Methods

        [Test]
        public void TestEvaluateGetTrasnsferDateTimeWhenTimeIsZeroHoursAndNonZeroMin_ShouldReturnValidNonZeroTime()
        {
            var transferDate = DateTime.MinValue;

            var transferInToOutView = new TransferInPatToOutPatView();

            transferInToOutView.SetTransferDateUnMaskedText( TRANSFER_DATE );
            transferInToOutView.SetTransferTimeUnMaskedText( TRANSFER_TIME );

            transferDate = transferInToOutView.GetTransferDateTime();
            Assert.IsTrue(transferDate.Hour == 0);
            Assert.IsTrue(transferDate.Minute == 10);
        }

        #endregion
    }
}