using System; 
using NUnit.Framework;
using PatientAccess.Domain; 
using PatientAccess.Domain.ShortRegistration;
using PatientAccess.Rules; 
using PatientAccess.UI.CommonControls.Email.Presenters;
using PatientAccess.UI.CommonControls.Email.Views;
using Rhino.Mocks;

namespace Tests.Unit.PatientAccess.UI.AddressViews
{
    [TestFixture]
    [Category( "Fast" )]
    public class EmailAddressPresenterTests
    {
        
        [Test]
        public void TestEmailAddress_VisibleForShortRegistration_AccountCreatedAfterFeatureStart()
        {
            var presenter = GetPresenterWithMockView(new ShortRegistrationActivity(),  
                                                     GetAccountCreatedDateAfterFeatureStart() );
            presenter.HandleEmailAddress();
            presenter.AddressView.AssertWasNotCalled( view => view.DoNotShowEmailAddress() );
        }
        [Test]
        public void TestEmailAddress_NotVisibleForShortRegistration_AccountCreatedBeforeFeatureStart()
        {
            var presenter = GetPresenterWithMockView(new ShortRegistrationActivity(),
                                                     GetAccountCreatedDateBeforeFeatureStart());
            presenter.HandleEmailAddress();
            presenter.AddressView.AssertWasCalled(view => view.DoNotShowEmailAddress());
        }
       
        #region Support Methods
        private static Account GetAccount(Activity activity, DateTime accountCreatedDate )
        {
            return new Account
                       {
                           Activity = activity,
                           AccountCreatedDate = accountCreatedDate,
                       };
        }
        private EmailAddressPresenter GetPresenterWithMockView(Activity activity , DateTime accountCreatedDate)
        {
            var view = MockRepository.GenerateMock<IEmailAddressView>();
            var account = GetAccount(activity, accountCreatedDate );
            account.IsShortRegistered = true;
            return new EmailAddressPresenter(view, account, RuleEngine.GetInstance() );
        }
        #endregion

        #region Constants
      
       private static DateTime GetAccountCreatedDateAfterFeatureStart()
        {
            return new EmailAddressFeatureManager().FeatureStartDate.AddDays(10);
        }
       
        private static DateTime GetAccountCreatedDateBeforeFeatureStart()
        {
            return new EmailAddressFeatureManager().FeatureStartDate.AddDays(-10);
        }
       
        #endregion
    }

}
