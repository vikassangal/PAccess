using System;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.UI.TransferViews;
using Rhino.Mocks;

namespace Tests.Unit.PatientAccess.UI.TransferViews
{
    [TestFixture]
    [Category( "Fast" )]
    public class TransferOutPatToInPatViewPresenterTests
    {
        [Test,Sequential] 
        public void TestSetAdmittingCategory( 
            [Values( "2", "3", "3" )] String expectedAdmittingType,
            [Values( "3", "2", "9" )] string patientType )
        {
            var account = new Account();
            var transferStep1View = GetStubTransferOutPatToInPatStep1View( account );
            var transferStep1Presenter = new TransferOutPatToInPatStep1Presenter(transferStep1View);
            account.KindOfVisit = new VisitType { Code = patientType };

            transferStep1Presenter.SetAdmittingCategory();

            Assert.IsTrue( account.AdmittingCategory.Equals( expectedAdmittingType ),
                string.Format("Admitting Type should be set to {0} when transfer {1} to {2}",
                    expectedAdmittingType, patientType,
                    VisitType.INPATIENT_DESC));
        }

        private static ITransferOutPatToInPatStep1View GetStubTransferOutPatToInPatStep1View( Account account )
        {
            var transferStep1View = MockRepository.GenerateStub<ITransferOutPatToInPatStep1View>();
            transferStep1View.Model = account;
            return transferStep1View;
        }
    }
}
