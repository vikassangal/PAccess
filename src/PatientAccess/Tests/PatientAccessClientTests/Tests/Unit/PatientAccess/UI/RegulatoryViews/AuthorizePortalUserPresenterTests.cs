using System;
using System.Windows.Forms;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI;
using PatientAccess.UI.HelperClasses;
using PatientAccess.UI.RegulatoryViews.Presenters;
using PatientAccess.UI.RegulatoryViews.ViewImpl;
using PatientAccess.UI.RegulatoryViews.Views;
using Rhino.Mocks;

namespace Tests.Unit.PatientAccess.UI.RegulatoryViews
{
    [TestFixture]
    [Category("Fast")]
    public class AuthorizePortalUserPresenterTests
    {
        #region ValidateAuthorizePortalUser
        [Test]
        public void TestAuthorizePortalUser_ValidateAuthorizePortalUser_WhenAtLeastOneRowIsEntered_returnsTrue()
        {
            AuthPortalUsersPresenter  = GetPresenterWithMockView(new RegistrationActivity(), VisitType.Inpatient, GetAccountCreatedDateAfterFeatureStart(), BLANK_HSV);
            
            AuthPortalUsersPresenter.PopulateAuthorizePortalUserDetails();
            var portalUser = new AuthorizedAdditionalPortalUser()
            {
                FirstName = FIRSTNAME,
                LastName = LASTNAME,
                DateOfBirth = DOB,
                EmailAddress = EMAIL
            };

            SetAuthorizedPortalUserDetails(AuthPortalUsersPresenter.AuthorizePortalUserView.AuthorizePortalUserDetail0, portalUser);
            Assert.IsTrue(AuthPortalUsersPresenter.ValidateAuthorizePortalUser());
            
        }

        [Test]
        public void TestAuthorizePortalUser_ValidateAuthorizePortalUser_WhenAtNoRowIsEntered_returnsFalse()
        {

            var presenter = GetPresenterWithMockView(new RegistrationActivity(), VisitType.Inpatient, GetAccountCreatedDateAfterFeatureStart(), BLANK_HSV);

            messageBoxAdapter.Expect(
                    x => x.Show(Arg<string>.Is.Equal(UIErrorMessages.AUTHORIZE_ADDITIONAL_PORTAL_USER_REQUIRED_MSG),
                        Arg<string>.Is.Anything,
                        Arg<MessageBoxButtons>.Is.Anything,
                        Arg<MessageBoxIcon>.Is.Anything,
                        Arg<MessageBoxDefaultButton>.Is.Anything))
                .Return(DialogResult.OK);

            presenter.PopulateAuthorizePortalUserDetails();
            var portalUser = new AuthorizedAdditionalPortalUser();
            SetAuthorizedPortalUserDetails(AuthPortalUsersPresenter.AuthorizePortalUserView.AuthorizePortalUserDetail0, portalUser);

            Assert.IsFalse(presenter.ValidateAuthorizePortalUser());

        }
        [Test]
        public void TestAuthorizePortalUser_ValidateAuthorizePortalUser_WhenPartialRowIsEntered_returnsFalse()
        {

            var presenter = GetPresenterWithMockView(new RegistrationActivity(), VisitType.Inpatient, GetAccountCreatedDateAfterFeatureStart(), BLANK_HSV);

            messageBoxAdapter.Expect(
                    x => x.Show(Arg<string>.Is.Equal(UIErrorMessages.AUTHORIZE_ADDITIONAL_PORTAL_USER_REQUIRED_MSG),
                        Arg<string>.Is.Anything,
                        Arg<MessageBoxButtons>.Is.Anything,
                        Arg<MessageBoxIcon>.Is.Anything,
                        Arg<MessageBoxDefaultButton>.Is.Anything))
                .Return(DialogResult.OK);

            presenter.PopulateAuthorizePortalUserDetails();
            var portalUser = new AuthorizedAdditionalPortalUser()
            {
                FirstName = String.Empty,
                LastName = LASTNAME,
                DateOfBirth = new DateTime(1955, 3, 5),
                EmailAddress = EMAIL
            };

            SetAuthorizedPortalUserDetails(AuthPortalUsersPresenter.AuthorizePortalUserView.AuthorizePortalUserDetail0, portalUser);

            Assert.IsFalse(presenter.ValidateAuthorizePortalUser());

        }
      
        #endregion
        #region HandleSaveResponse

        [Test]
        public void TestAuthorizePortalUser_HandleSaveResponse_WhenAtLeastOneRowIsEntered_returnsTrue()
        {
            AuthPortalUsersPresenter = GetPresenterWithMockView(new RegistrationActivity(), VisitType.Inpatient,
                GetAccountCreatedDateAfterFeatureStart(), BLANK_HSV);


            AuthPortalUsersPresenter.PopulateAuthorizePortalUserDetails();
            var portalUser = new AuthorizedAdditionalPortalUser()
            {
                FirstName = FIRSTNAME,
                LastName = LASTNAME,
                DateOfBirth = DOB,
                EmailAddress = EMAIL
            };

            SetAuthorizedPortalUserDetails(AuthPortalUsersPresenter.AuthorizePortalUserView.AuthorizePortalUserDetail0, portalUser);
            Assert.IsTrue(AuthPortalUsersPresenter.HandleSaveResponse());
        }
        [Test]
        public void TestAuthorizePortalUser_HandleSaveResponse_WhenPartialRowIsEntered_returnsFalse()
        {
            AuthPortalUsersPresenter = GetPresenterWithMockView(new RegistrationActivity(), VisitType.Inpatient,
                GetAccountCreatedDateAfterFeatureStart(), BLANK_HSV);


            AuthPortalUsersPresenter.PopulateAuthorizePortalUserDetails();
            var portalUser = new AuthorizedAdditionalPortalUser()
            {
                FirstName = FIRSTNAME,
                LastName = String.Empty,
                DateOfBirth = DOB,
                EmailAddress = EMAIL
            };

            SetAuthorizedPortalUserDetails(AuthPortalUsersPresenter.AuthorizePortalUserView.AuthorizePortalUserDetail0, portalUser);
            Assert.IsFalse(AuthPortalUsersPresenter.HandleSaveResponse());
        }
        [Test]
        public void TestAuthorizePortalUser_HandleSaveResponse_WhenNoRowIsEntered_returnsFalse()
        {
            AuthPortalUsersPresenter = GetPresenterWithMockView(new RegistrationActivity(), VisitType.Inpatient,
                GetAccountCreatedDateAfterFeatureStart(), BLANK_HSV);


            AuthPortalUsersPresenter.PopulateAuthorizePortalUserDetails();
            var portalUser = new AuthorizedAdditionalPortalUser(); 

            SetAuthorizedPortalUserDetails(AuthPortalUsersPresenter.AuthorizePortalUserView.AuthorizePortalUserDetail0, portalUser);
            Assert.IsFalse(AuthPortalUsersPresenter.HandleSaveResponse());
        }

        #endregion

        #region Support Methods
        private static Account GetAccount(Activity activity, VisitType visitType, DateTime accountCreatedDate, HospitalService hsv)
        {
            return new Account
            {
                Activity = activity,
                AdmitDate = new DateTime(2019, 01,15 ),
                AccountCreatedDate = accountCreatedDate,
                KindOfVisit = visitType,
                HospitalService = hsv

            };
        }

        private void SetAuthorizedPortalUserDetails(AuthorizePortalUserDetailView detailView,
            AuthorizedAdditionalPortalUser portalUser)
        {
            detailView.FirstName = portalUser.FirstName;
            detailView.LastName = portalUser.LastName;
            AuthPortalUsersPresenter.AuthorizePortalUserView.AuthorizePortalUserDetail0.DOB = String.Format("{0:D2}{1:D2}{2:D4}",
                portalUser.DateOfBirth.Month,
                portalUser.DateOfBirth.Day,
                portalUser.DateOfBirth.Year);
            AuthPortalUsersPresenter.AuthorizePortalUserView.AuthorizePortalUserDetail0.EmailAddress = portalUser.EmailAddress.Uri;
        }

        private AuthorizePortalUserPresenter GetPresenterWithMockView(Activity activity, VisitType patientType, DateTime accountCreatedDate, HospitalService hsv)
        {
            var view = MockRepository.GenerateStub<IAuthorizePortalUserView>();
            view.AuthorizePortalUserDetail0 = new AuthorizePortalUserDetailView(); 
            view.AuthorizePortalUserDetail1 = new AuthorizePortalUserDetailView(); 
            view.AuthorizePortalUserDetail2 = new AuthorizePortalUserDetailView(); 
            view.AuthorizePortalUserDetail3 = new AuthorizePortalUserDetailView(); 

            var account = GetAccount(activity, patientType, accountCreatedDate, hsv);
              messageBoxAdapter = MockRepository.GenerateMock<IMessageBoxAdapter>();
            return new AuthorizePortalUserPresenter(view, messageBoxAdapter, account);
        }
         
        private readonly HospitalService BLANK_HSV = new HospitalService
        {
            Code = HospitalService.BLANK_CODE,
            Description = "HSVBLANK"
        };

        private IMessageBoxAdapter messageBoxAdapter;
        private AuthorizePortalUserPresenter AuthPortalUsersPresenter;
        private readonly DateTime DOB = new DateTime(1955, 03, 05);
        private readonly EmailAddress EMAIL = new EmailAddress("JonDoe@gmail.com");
        private readonly string LASTNAME = "Doe";
        private readonly string FIRSTNAME = "Jon";

        #endregion

        #region Constants
    
        private static DateTime GetAccountCreatedDateAfterFeatureStart()
        {
            return new AuthorizePortalUserFeatureManager().FeatureStartDate.AddDays(10);
        }
        
        #endregion
    }
}
