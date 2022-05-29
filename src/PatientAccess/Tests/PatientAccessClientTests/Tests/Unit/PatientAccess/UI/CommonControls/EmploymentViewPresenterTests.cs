using System;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;
using Rhino.Mocks;

namespace Tests.Unit.PatientAccess.UI.CommonControls
{
    [TestFixture]
    [Category( "Fast" )]
    public class EmploymentViewPresenterTests
    {
        #region Test Methods
        [Test, Sequential]
        public void SetControlStatesOnEmploymentStatus_EmploymentStatusNotNull(
            [Values(EmploymentStatus.NOT_EMPLOYED_DESC, EmploymentStatus.RETIRED_DESC, EmploymentStatus.SELF_EMPLOYED_DESC, "Empty", "Other")] String statusDesc,
            [Values(EmploymentStatus.NOT_EMPLOYED_CODE, EmploymentStatus.RETIRED_CODE, EmploymentStatus.SELF_EMPLOYED_CODE, "", EmploymentStatus.OTHER_CODE)] String statusCode,
            [Values(false, true, true, false, true) ] Boolean clearButtonExpectedStatus,
            [Values(false, true, true, false, true) ] Boolean otherControlsExpectedStatus)
        {
            var mockEmploymentView = prepareStubEmploymentView(statusDesc,statusCode);
            var employmentViewPresenter = new EmploymentViewPresenter(mockEmploymentView);
            employmentViewPresenter.SetControlStatesOnEmploymentStatus();
            Assert.AreEqual(clearButtonEnabled, clearButtonExpectedStatus, 
                    string.Format("clear button should be {0} for {1}",clearButtonExpectedStatus?"enabled":"disabled", statusDesc));
            Assert.AreEqual(otherControlsEnabled, otherControlsExpectedStatus, 
                string.Format("other controls should be {0} for {1}", otherControlsExpectedStatus?"enabled":"disabled",statusDesc));
        }

        [Test]
        public void SetControlStatesOnEmploymentStatus_Null()
           
        {
            var mockEmploymentView = prepareStubEmploymentView("", "");
            mockEmploymentView.Model_Employment.Status = null;
            var employmentViewPresenter = new EmploymentViewPresenter(mockEmploymentView);
            employmentViewPresenter.SetControlStatesOnEmploymentStatus();
            Assert.AreEqual(clearButtonEnabled, true, "clear button should be enabled when employment status is null");

            Assert.AreEqual(otherControlsEnabled, false, "other controls should be disabled when employment status is null");
        }

        [Test]
        public void Test_SetControlStatesOnEmploymentStatus_WhenEmploymentStatusIsEmpty_ClearButtonAndOtherControls_ShouldBeDisabled()
        {
            var mockEmploymentView = prepareStubEmploymentView("", "");
            mockEmploymentView.Model_Employment.Status.Code = string.Empty;
            var employmentViewPresenter = new EmploymentViewPresenter(mockEmploymentView);
            employmentViewPresenter.SetControlStatesOnEmploymentStatus();

            Assert.AreEqual(clearButtonEnabled, false, "clear button should be disabled when employment status is empty");
            Assert.AreEqual(otherControlsEnabled, false, "other controls should be disabled when employment status is empty");
        }
       

        #endregion

        #region Private Methods
        IEmploymentView prepareStubEmploymentView(string employmentDesc, string employmentStatusCode)
        {
            var mockEmploymentView = MockRepository.GenerateStub<IEmploymentView>();
            mockEmploymentView.Model_Employment = new Employment
            {
                Status =
                    new EmploymentStatus(0, DateTime.Now, employmentDesc, employmentStatusCode)
            };
            mockEmploymentView.Stub(x => x.SetControlState(Arg<bool>.Is.Anything)).WhenCalled(x => otherControlsEnabled = (bool)x.Arguments[0]);
            mockEmploymentView.Stub(x => x.EnableClearButton(Arg<bool>.Is.Anything)).WhenCalled(
                x => clearButtonEnabled = (bool)x.Arguments[0]);
            return mockEmploymentView;
        }

        #endregion

        #region Data Elements

        private bool clearButtonEnabled;
        private bool otherControlsEnabled;
        #endregion
    }
}
