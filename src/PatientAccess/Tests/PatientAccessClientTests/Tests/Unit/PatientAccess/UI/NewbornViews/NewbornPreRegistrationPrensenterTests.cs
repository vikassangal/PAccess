using System;
using System.Threading;
using System.Windows.Forms;
using NUnit.Framework;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.UI.Registration;

namespace Tests.Unit.PatientAccess.UI.NewbornViews
{
    [TestFixture]
    [Category( "Fast" )]
    public class NewbornPreRegistrationPrensenterTests
    {
        [Ignore]
        [Test]
        public void CreatePreNewBornAccountEventHandler_WithSelectedAccount_ModelAccountIsNew()
        {
            //Got error when run this test with others in solution: The agent process was stopped while the test was running.
            //create view
            var newbornPreRegView = new FakeNewbornPreRegView();
            //set selectedAccount for the view
            newbornPreRegView.SelectedAccount =
                new Account
                {
                    Patient = new Patient { FirstName = "Test", LastName = "Mom" },
                    Activity= new PreAdmitNewbornActivity()
                };
            //create presenter
            var newbornPreRegPresenter = new NewbornPreRegistrationPresenter(newbornPreRegView);
            newbornPreRegPresenter.CreatePreNewBornAccountEventHandler(null,null);
            //wait for the thread finish work for presenter.... Any better way to do this?
            Thread.Sleep(5000);
            //verify view.ModelAccount
            Assert.IsTrue(newbornPreRegView.ModelAccount.Patient.IsNew, "Newborn Patient should be New");
            Assert.IsTrue(newbornPreRegView.ModelAccount.IsNew, "newborn Account should be New");
            Assert.IsTrue( newbornPreRegView.ModelAccount.KindOfVisit == VisitType.PreRegistration, "visit type should be PreRegistration" ); 
        }
    }

    internal class FakeNewbornPreRegView : INewbornPreRegistrationView
    {
        public FakeNewbornPreRegView()
        {
            //presenter = new NewbornPreRegistrationPresenter( this );
        }

        public Account ModelAccount
        {
            get;set;
        }

        public Activity CurrentActivity
        {
            get 
            {
                return new PreAdmitNewbornActivity();				
			}
        }

        public IAccount SelectedAccount
        {
            get;set;
        }

        public IAccountView accountView
        {
            get;set;
        }

        public EventHandler ReturnToMainScreen
        {
            get;set;
        }

        public bool IsInDesignMode
        {
            get { return false; }
        }

        public bool IsDisposedOrDisposing
        {
            get { return false; }
        }

        public void SetMasterPatientIndexViewActivity(Activity activity)
        {
            return;
        }

        public void RegisterOnReturnToMainScreen(EventHandler returnToMainScreen)
        {
            return;
        }

        public void EnableAccountView()
        {
            return;
        }

        public void SetCursor(Cursor cursor)
        {
            return;
        }

        public void DisplayMasterPatientIndexView()
        {
            return;
        }
    }
}
