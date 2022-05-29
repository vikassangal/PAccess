using System;
using System.Collections.ObjectModel;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.UI.PatientSearch;

namespace Tests.Unit.PatientAccess.UI.PatientSearch
{
    /// <summary>
    /// Summary description for PatientsAccountsViewTests
    /// </summary>
    [TestFixture]
    [Category( "Fast" )]
    public class PatientAccountsViewPresenterTests
    {
        private const string _messageTemplateForOneAcccount = "A pre-registered account ({0}) exists for this patient. Do you want to proceed with the current registration or cancel to use the existing account?";
        private const string _messageTemplateForMoreThanOneAcccount = "Pre-registered accounts ({0}) exist for this patient. Do you want to proceed with the current registration or cancel to use the existing account?";

        [Test]
        [ExpectedException( typeof( ArgumentNullException ) )]
        public void ConstructDuplicateAccountWarningMessageTest_WithNullInput_ShouldThrowException()
        {
            PatientAccountsViewPresenter.ConstructDuplicateAccountWarningMessage( null );
        }

        [Test]
        [ExpectedException( typeof( ArgumentException ) )]
        public void ConstructDuplicateAccountWarningMessageTest_WhenThereAreNoAccounts_ShouldThrowException()
        {
            var accounts = new Collection<IAccount>();

            PatientAccountsViewPresenter.ConstructDuplicateAccountWarningMessage( accounts );
        }

        [Test]
        public void ConstructDuplicateAccountWarningMessageTest_WhenThereIOneAccount()
        {
            const int firstAccountNumber = 1;
            var accounts = new Collection<IAccount> 
                { 
                    new Account { AccountNumber = firstAccountNumber },
                };

            string actualMessage = PatientAccountsViewPresenter.ConstructDuplicateAccountWarningMessage( accounts );
            string expectedMessage = string.Format( _messageTemplateForOneAcccount, "#" + firstAccountNumber );
            Assert.AreEqual( expectedMessage, actualMessage );
        }

        [Test]
        public void ConstructDuplicateAccountWarningMessageTest_WhenThereAreTwoAccounts()
        {
            const int firstAccountNumber = 1;
            const int secondAccountNumber = 2;
            var accounts = new Collection<IAccount> 
                { 
                    new Account { AccountNumber = firstAccountNumber },
                    new Account { AccountNumber = secondAccountNumber }
                };

            string actualMessage = PatientAccountsViewPresenter.ConstructDuplicateAccountWarningMessage( accounts );
            string expectedMessage = string.Format( _messageTemplateForMoreThanOneAcccount, "#" + firstAccountNumber + "," + "#" + secondAccountNumber );
            Assert.AreEqual( expectedMessage, actualMessage );
        }

        [Test]
        public void ConstructDuplicateAccountWarningMessageTest_WhenThereAreThreeAccounts()
        {
            const int firstAccountNumber = 1;
            const int secondAccountNumber = 2;
            const int thirdAccountNumber = 3;

            var accounts = new Collection<IAccount> 
                { 
                    new Account { AccountNumber = firstAccountNumber },
                    new Account { AccountNumber = secondAccountNumber },
                    new Account { AccountNumber = thirdAccountNumber }
                };

            string actualMessage = PatientAccountsViewPresenter.ConstructDuplicateAccountWarningMessage( accounts );
            string expectedMessage = string.Format( _messageTemplateForMoreThanOneAcccount, "#" + firstAccountNumber + "," + "#" + secondAccountNumber + "," + "#" + thirdAccountNumber );
            Assert.AreEqual( expectedMessage, actualMessage );
        }

        [Test]
        public void ConstructDuplicateAccountWarningMessageTest_WhenThereAreFourAccounts()
        {
            const int firstAccountNumber = 1;
            const int secondAccountNumber = 2;
            const int thirdAccountNumber = 3;
            const int fourthAccountNumber = 4;

            var accounts = new Collection<IAccount> 
                { 
                    new Account { AccountNumber = firstAccountNumber },
                    new Account { AccountNumber = secondAccountNumber },
                    new Account { AccountNumber = thirdAccountNumber },
                    new Account { AccountNumber = fourthAccountNumber }
                };

            string actualMessage = PatientAccountsViewPresenter.ConstructDuplicateAccountWarningMessage( accounts );
            string expectedMessage = string.Format( _messageTemplateForMoreThanOneAcccount, "#" + firstAccountNumber + "," + "#" + secondAccountNumber + "," + "#" + thirdAccountNumber + "," + "#" + fourthAccountNumber );
            Assert.AreEqual( expectedMessage, actualMessage );
        }

        [Test]
        public void ConstructDuplicateAccountWarningMessageTest_WhenThereAreMoreThanFourAccounts()
        {
            const int firstAccountNumber = 1;
            const int secondAccountNumber = 2;
            const int thirdAccountNumber = 3;
            const int fourthAccountNumber = 4;
            const int fifthAccountNumber = 5;

            var accounts = new Collection<IAccount> 
                { 
                    new Account { AccountNumber = firstAccountNumber },
                    new Account { AccountNumber = secondAccountNumber },
                    new Account { AccountNumber = thirdAccountNumber },
                    new Account { AccountNumber = fourthAccountNumber },
                    new Account { AccountNumber = fifthAccountNumber }
                };

            string actualMessage = PatientAccountsViewPresenter.ConstructDuplicateAccountWarningMessage( accounts );
            string expectedMessage = string.Format( _messageTemplateForMoreThanOneAcccount, "#" + firstAccountNumber + "," + "#" + secondAccountNumber + "," + "#" + thirdAccountNumber + "," + "#" + fourthAccountNumber );
            Assert.AreEqual( expectedMessage, actualMessage );
        }

//tests to check that the message is shown only if there is a Pre-Reg account for the patient on the same day
//        [TestMethod]
//        public void TestHandleCreateNewAccount()
//        {
//            IPatientsAccountsView mockView = MockRepository.GenerateMock<IPatientsAccountsView>();
//            IMessageBoxAdapter mockMessageBoxAdapter = MockRepository.GenerateMock<IMessageBoxAdapter>();;
//            IFacility mockFacility= MockRepository.GenerateMock<IFacility>();
//
//            PatientAccountsViewPresenter presenter = new PatientAccountsViewPresenter( mockView, mockFacility, mockMessageBoxAdapter, new RegistrationActivity() );
//
//            const int firstAccountNumber = 1;
//            const int secondAccountNumber = 2;
//            const int thirdAccountNumber = 3;
//            const int fourthAccountNumber = 4;
//
//            IList<IAccount> accounts = new List<IAccount> 
//            { 
//                new Account { AccountNumber = firstAccountNumber },
//                new Account { AccountNumber = secondAccountNumber },
//                new Account { AccountNumber = thirdAccountNumber },
//                new Account { AccountNumber = fourthAccountNumber }
//            };
//            presenter.AccountsForSelectedPatient = accounts;
//           
//            presenter.View.PatientAccounts = new ListView();
//
//            foreach (var account in accounts)
//            {
//                presenter.View.PatientAccounts.Items.Add(new ListViewItem() {Tag = account});
//            }
//            presenter.SelectAccount(accounts[0]);
//            presenter.HandleCreateNewAccount();
//            mockMessageBoxAdapter.AssertWasNotCalled(x=>x.ShowMessageBox( Arg<string>.Is.Anything,  Arg<string>.Is.Anything, MessageBoxButtons.OKCancel ));
//
//        }

        //TODO-AC add tests as to when a message should and should not appear and tests for when there are non pre-reg accounts with the same admit date
    }
}