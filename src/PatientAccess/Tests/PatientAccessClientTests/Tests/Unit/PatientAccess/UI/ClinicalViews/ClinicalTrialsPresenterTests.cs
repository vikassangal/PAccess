using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Test.Text;
using NUnit.Framework;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI.ClinicalViews;
using Rhino.Mocks;

namespace Tests.Unit.PatientAccess.UI.ClinicalViews
{
    /// <summary>
    /// Summary description for ClinicalTrialsPresenterTests
    /// </summary>
    [TestFixture]
    public class ClinicalTrialsPresenterTests
    {
        [Test]
        public void TestUserOpensStudyDetailsAddsAStudyAndSaves_StudyDetailsAreSavedToAccount()
        {
            var mockery = new MockRepository();
            var view = mockery.DynamicMock<IClinicalTrialsView>();
            var studyBroker = mockery.Stub<IResearchStudyBroker>();
            var featureManager = mockery.Stub<IClinicalTrialsFeatureManager>();
            var detailsView = mockery.Stub<IClinicalTrialsDetailsView>();
            var mockStudySelectionList = GetMockStudies( 15 );
            studyBroker.Expect( x => x.AllResearchStudies( Arg<long>.Is.Anything ) ).Return( mockStudySelectionList ).Repeat. Any();
            var account = new Account { Activity = new RegistrationActivity() };
            mockery.ReplayAll();

            var presenter = new ClinicalTrialsPresenter( view, detailsView, featureManager, account, 900, studyBroker );

            presenter.ShowDetails();
            var selectedStudy = mockStudySelectionList[0];
            var detailsPresenter = presenter.DetailsPresenter;
            detailsPresenter.UpdateSelectedStudyInSelectionList( selectedStudy );
            detailsPresenter.EnrollWithConsent();
            detailsPresenter.SaveChangesAndExit();
            var consentedStudyThatWasAdded = new ConsentedResearchStudy( selectedStudy, YesNoFlag.Yes );
            Assert.IsTrue( account.ClinicalResearchStudies.Contains( consentedStudyThatWasAdded ), 
                "The added study should be added to the patient account as the user saved the changes" );
        }


        [Test]
        [Category( "Fast" )]
        public void TestUserOpensStudyDetailsAddsAStudyAndCancelsSave_StudyDetailsAreNotSavedToAccount()
        {
            var mockery = new MockRepository();
            var view = mockery.DynamicMock<IClinicalTrialsView>();
            var studyBroker = mockery.Stub<IResearchStudyBroker>();
            var featureManager = mockery.Stub<IClinicalTrialsFeatureManager>();
            var detailsView = mockery.Stub<IClinicalTrialsDetailsView>();
            var mockStudySelectionList = GetMockStudies( 15 );
            studyBroker.Expect( x => x.AllResearchStudies( Arg<long>.Is.Anything ) ).Return( mockStudySelectionList ).Repeat. Any();
            var account = new Account { Activity = new RegistrationActivity() };
            mockery.ReplayAll();

            var presenter = new ClinicalTrialsPresenter( view, detailsView, featureManager, account, 0, studyBroker );

            presenter.ShowDetails();
            var selectedStudy = mockStudySelectionList[0];
            var detailsPresenter = presenter.DetailsPresenter;
            detailsPresenter.UpdateSelectedStudyInSelectionList( selectedStudy );
            detailsPresenter.EnrollWithConsent();
            detailsPresenter.DiscardChanges();
            var consentedStudyThatWasAdded = new ConsentedResearchStudy( selectedStudy, YesNoFlag.Yes );

            Assert.IsFalse( account.ClinicalResearchStudies.Contains( consentedStudyThatWasAdded ),
                           "The added study should not be added to the patient account as the user discarded the changes" );
        }

        [Test]
        [Category( "Fast" )]
        public void TestUserAddsPatientStudiesAndCancelsSave_TheDiscardedStudiesShouldBeAddedBackToSelectionList()
        {
            var mockery = new MockRepository();
            var view = mockery.DynamicMock<IClinicalTrialsView>();
            var studyBroker = mockery.Stub<IResearchStudyBroker>();
            var featureManager = mockery.Stub<IClinicalTrialsFeatureManager>();
            var detailsView = mockery.Stub<IClinicalTrialsDetailsView>();
            var mockStudySelectionList = GetMockStudies( 15 );
            studyBroker.Expect( x => x.AllResearchStudies( Arg<long>.Is.Anything ) ).Return( mockStudySelectionList ).Repeat. Any();
            var account = new Account { Activity = new RegistrationActivity() };
            mockery.ReplayAll();

            var presenter = new ClinicalTrialsPresenter( view, detailsView, featureManager, account, 0, studyBroker );

            presenter.ShowDetails();
            var selectedStudy = mockStudySelectionList[0];
            var detailsPresenter = presenter.DetailsPresenter;
            detailsPresenter.UpdateSelectedStudyInSelectionList( selectedStudy );
            detailsPresenter.EnrollWithConsent();
            detailsPresenter.DiscardChanges();
            presenter.ShowDetails();

            var consentedStudyThatWasAdded = new ConsentedResearchStudy( selectedStudy, YesNoFlag.Yes );

            Assert.IsTrue( presenter.DetailsPresenter.StudySelectionList.Contains( consentedStudyThatWasAdded.ResearchStudy ),
                           "The study should be added back to the selection list" );
        }

        [Test]
        [Category( "Fast" )]
        public void TestUserSetsIsPatientInStudyToYesForNewPatient_StudyDetailsViewIsShown()
        {
            var mockery = new MockRepository();
            var view = mockery.DynamicMock<IClinicalTrialsView>();
            var studyBroker = mockery.Stub<IResearchStudyBroker>();
            var featureManager = mockery.Stub<IClinicalTrialsFeatureManager>();
            var detailsView = mockery.Stub<IClinicalTrialsDetailsView>();
            var mockStudySelectionList = GetMockStudies( 15 );
            studyBroker.Expect( x => x.AllResearchStudies( Arg<long>.Is.Anything ) ).Return( mockStudySelectionList ).Repeat. Any();
            var account = new Account { Activity = new RegistrationActivity() };

            mockery.ReplayAll();
            var presenter = new ClinicalTrialsPresenter( view, detailsView, featureManager, account, 0, studyBroker );

            presenter.DetailsView = detailsView;

            presenter.UserChangedPatientInClinicalTrialsTo( new YesNoFlag( YesNoFlag.CODE_YES ) );

            Assert.IsNotNull( presenter.DetailsPresenter );
            Assert.IsNotNull( presenter.DetailsView );
            CollectionAssert.AreEquivalent( mockStudySelectionList.ToList(),
                                           presenter.DetailsPresenter.StudySelectionList.ToList() );

            detailsView.AssertWasCalled( x => x.ShowMe() );
            detailsView.AssertWasCalled(
                x =>
                x.Update( Arg<IEnumerable<ConsentedResearchStudy>>.Is.Anything,
                         Arg<IEnumerable<ResearchStudy>>.Is.Anything ) );
        }


        [Test]
        [Category( "Fast" )]
        public void TestUserOpenDetailsViewForPatientWithStudies_DetailsViewSelectionListDoesNotContainPatientsStudies()
        {
            var mockery = new MockRepository();
            var view = mockery.DynamicMock<IClinicalTrialsView>();
            var studyBroker = mockery.Stub<IResearchStudyBroker>();
            var featureManager = mockery.Stub<IClinicalTrialsFeatureManager>();
            var detailsView = mockery.Stub<IClinicalTrialsDetailsView>();
            var mockStudySelectionList = GetMockStudies( 15 );

            studyBroker.Expect( x => x.AllResearchStudies( Arg<long>.Is.Anything ) ).Return( mockStudySelectionList ).Repeat. Any();
            var account = new Account { Activity = new RegistrationActivity() };
            var studyAddedToAccount = new ConsentedResearchStudy( mockStudySelectionList.ToList()[0], YesNoFlag.Yes );
            account.AddConsentedResearchStudy( studyAddedToAccount );
            account.IsPatientInClinicalResearchStudy = YesNoFlag.Yes;

            mockery.ReplayAll();

            var presenter = new ClinicalTrialsPresenter( view, detailsView, featureManager, account, 0, studyBroker );
            presenter.ShowDetails();
            var selectionList = presenter.DetailsPresenter.StudySelectionList;
            Assert.IsFalse( selectionList.Contains( studyAddedToAccount.ResearchStudy ),
                           "If a study is associated with a patient then it should not appear in the selection list" );
        }


        [Test]
        public void TestUserEnrollsPatientInStudyWithConsent_StudyWithConsentIsAssociatedWithPatient()
        {
            var mockStudySelectionList = GetMockStudies( 15 );

            var currentAccount = new Account { Activity = new RegistrationActivity() };
            var detailsView = MockRepository.GenerateStub<IClinicalTrialsDetailsView>();
            var detailsPresenter = new ClinicalTrialsDetailsPresenter( 900, currentAccount, detailsView, mockStudySelectionList );

            detailsPresenter.SetStudySelectionList( mockStudySelectionList );
            var selectedStudy = mockStudySelectionList[0]; 

            detailsPresenter.UpdateSelectedStudyInSelectionList( selectedStudy );

            detailsPresenter.EnrollWithConsent();
            detailsPresenter.SaveChangesAndExit();
            bool accountContainsSelectedStudyWithoutConsent =
                currentAccount.ClinicalResearchStudies.Count(
                    x => x.ResearchStudy == selectedStudy && x.ProofOfConsent == YesNoFlag.Yes ) == 1;
            Assert.IsTrue( accountContainsSelectedStudyWithoutConsent,
                          "Selected study should be added to the account with consent" );
            CollectionAssert.DoesNotContain( detailsPresenter.StudySelectionList.ToList(), selectedStudy,
                                            "When the study is associated with the account it should be removed from the master list" );
        }

        [Test]
        public void TestUserEnrollsPatientInStudyWithoutConsent_StudyWithoutConsentIsAssociatedWithPatient()
        {
            var mockStudySelectionList = GetMockStudies( 15 );

            var currentAccount = new Account { Activity = new RegistrationActivity() };
            var detailsView = MockRepository.GenerateStub<IClinicalTrialsDetailsView>();
            var detailsPresenter = new ClinicalTrialsDetailsPresenter( 900, currentAccount, detailsView, mockStudySelectionList );

            detailsPresenter.SetStudySelectionList( mockStudySelectionList );
            var selectedStudy = mockStudySelectionList[0];

            detailsPresenter.UpdateSelectedStudyInSelectionList( selectedStudy );

            detailsPresenter.EnrollWithoutConsent();
            detailsPresenter.SaveChangesAndExit();
            bool accountContainsSelectedStudyWithoutConsent =
                currentAccount.ClinicalResearchStudies.Count(
                    x => x.ResearchStudy == selectedStudy && x.ProofOfConsent == YesNoFlag.No ) == 1;
            Assert.IsTrue( accountContainsSelectedStudyWithoutConsent,
                          "Selected study should be added to the account without consent" );
            CollectionAssert.DoesNotContain( detailsPresenter.StudySelectionList.ToList(), selectedStudy,
                                            "When the study is associated with the account it should be removed from the master list" );
        }

        [Test]
        public void TestUserRemovesAStudyAndSavesChanges_StudyIsNoLongerAssociatedWithPatient()
        {
            IList<ResearchStudy> mockStudySelectionList = GetMockStudies( 15 );

            var currentAccount = new Account { Activity = new RegistrationActivity() };
            var detailsView = MockRepository.GenerateStub<IClinicalTrialsDetailsView>();
            var detailsPresenter = new ClinicalTrialsDetailsPresenter( 900, currentAccount, detailsView,
                                                                      mockStudySelectionList );

            detailsPresenter.SetStudySelectionList( mockStudySelectionList );
            var studyToRemove = new ConsentedResearchStudy( mockStudySelectionList[0], YesNoFlag.No );
            var studyToRemain = new ConsentedResearchStudy( mockStudySelectionList[1], YesNoFlag.No );

            detailsPresenter.UpdateSelectedStudyInSelectionList( studyToRemain.ResearchStudy );
            detailsPresenter.EnrollWithoutConsent();

            detailsPresenter.UpdateSelectedStudyInSelectionList( studyToRemove.ResearchStudy );
            detailsPresenter.EnrollWithoutConsent();


            detailsPresenter.UpdateSelectedPatientStudy( studyToRemove );
            detailsPresenter.RemoveSelectedPatientStudy();

            detailsPresenter.SaveChangesAndExit();
            bool containsRemovedStudy = currentAccount.ClinicalResearchStudies.Contains( studyToRemove );
            bool containsRemainingStudy = currentAccount.ClinicalResearchStudies.Contains( studyToRemain );

            Assert.IsTrue( containsRemainingStudy,
                          "This study was not removed so it should be associated with the account" );
            Assert.IsFalse( containsRemovedStudy,
                           "This study should not have been added to account as it was removed before save" );

            CollectionAssert.Contains( mockStudySelectionList.ToList(), mockStudySelectionList[0],
                                      "When the study is dis-associated with the account it should be added back to the master list" );
        }

        [Test]
        public void TestUserAddsAClinicalTrialToAccountWithNoExistingConditionCode30_ClinicalTrialsConditionCodeShouldbeAddedToTheAccount()
        {
            var mockStudyListForSelection = GetMockStudies( 15 );

            var account = new Account { Activity = new RegistrationActivity() };
            var detailsView = MockRepository.GenerateStub<IClinicalTrialsDetailsView>();
            var detailsPresenter = new ClinicalTrialsDetailsPresenter( 900, account, detailsView,
                                                                      mockStudyListForSelection );
            detailsPresenter.SetStudySelectionList( mockStudyListForSelection );

            var selctedStudyFromSelectionList = mockStudyListForSelection[1];

            detailsPresenter.UpdateSelectedStudyInSelectionList( selctedStudyFromSelectionList );
            detailsPresenter.EnrollWithoutConsent();
            var addedStudy = detailsPresenter.PatientStudies.First(x => x.Code == selctedStudyFromSelectionList.Code);
            detailsPresenter.UpdateSelectedPatientStudy( addedStudy );

            detailsPresenter.SaveChangesAndExit();
            var conditionCodes = account.ConditionCodes.Cast<ConditionCode>();

            Assert.IsTrue( conditionCodes.Count( x => x.Code == ConditionCode.CONDITIONCODE_QUALIFYING_CLINICAL_TRIALS ) == 1 );
        }

        [Test]
        public void TestUserAddsAClinicalTrialToAccountWithExistingConditionCode30_OnlyOneClinicalTrialsConditionCodeShouldbeAddedToTheAccount()
        {
            var mockStudyListForSelection = GetMockStudies( 15 );

            var account = new Account { Activity = new RegistrationActivity() };

            var clinicalTrialConditionCode = new ConditionCode();
            clinicalTrialConditionCode.Code = ConditionCode.CONDITIONCODE_QUALIFYING_CLINICAL_TRIALS;
            clinicalTrialConditionCode.Description = ConditionCode.CONDITIONCODE_QUALIFYING_CLINICAL_TRIALS;
            account.AddConditionCode( clinicalTrialConditionCode );

            var detailsView = MockRepository.GenerateStub<IClinicalTrialsDetailsView>();
            var detailsPresenter = new ClinicalTrialsDetailsPresenter( 900, account, detailsView,
                                                                      mockStudyListForSelection );
            detailsPresenter.SetStudySelectionList( mockStudyListForSelection );

            var selctedStudyFromSelectionList = mockStudyListForSelection[1];

            detailsPresenter.UpdateSelectedStudyInSelectionList( selctedStudyFromSelectionList );
            detailsPresenter.EnrollWithoutConsent();
            var addedStudy = detailsPresenter.PatientStudies.First( x => x.Code == selctedStudyFromSelectionList.Code );
            detailsPresenter.UpdateSelectedPatientStudy( addedStudy );

            detailsPresenter.SaveChangesAndExit();
            var conditionCodes = account.ConditionCodes.Cast<ConditionCode>();

            Assert.IsTrue( conditionCodes.Count() == 1 );
            Assert.IsTrue( conditionCodes.Count(x => x.Code == ConditionCode.CONDITIONCODE_QUALIFYING_CLINICAL_TRIALS) == 1 );
        }

        [Test]
        public void TestUserSelectsNoForIsPatientInStudyForAPatientWithStudies_TheConditionCodeShouldBeRemovedFromTheAccount()
        {
            var mockery = new MockRepository();
            const int totalStudies = 15;

            var view = mockery.DynamicMock<IClinicalTrialsView>();
            var studyBroker = mockery.Stub<IResearchStudyBroker>();
            var featureManager = mockery.Stub<IClinicalTrialsFeatureManager>();
            var detailsView = mockery.DynamicMock<IClinicalTrialsDetailsView>();
            var mockStudySelectionList = GetMockStudies( totalStudies );
            studyBroker.Expect( x => x.AllResearchStudies( Arg<long>.Is.Anything ) ).Return( mockStudySelectionList ).Repeat.Any();
            view.Expect( x => x.GetConfirmationForDiscardingPatientStudies() ).Return( true );
            var accountWithStudies = new Account
                                         {
                                             Activity = new RegistrationActivity(),
                                             IsPatientInClinicalResearchStudy = YesNoFlag.Yes
                                         };

            accountWithStudies.AddConsentedResearchStudy( new ConsentedResearchStudy( mockStudySelectionList[0],
                                                                                      YesNoFlag.Yes ) );
            mockery.ReplayAll();

            var presenter = new ClinicalTrialsPresenter( view, detailsView, featureManager, accountWithStudies, 900,
                                                         studyBroker );
            presenter.UserChangedPatientInClinicalTrialsTo( YesNoFlag.No );

            presenter.DetailsPresenter.SaveChangesAndExit();
            var conditionCodes = accountWithStudies.ConditionCodes.Cast<ConditionCode>();

            Assert.IsTrue( conditionCodes.All(x => x.Code != ConditionCode.CONDITIONCODE_QUALIFYING_CLINICAL_TRIALS) );

        }

        [Test]
        [Category( "Fast" )]
        public void
            TestUserAddsAClinicalTrialToAnAccountThenRemovesIt_ClinicalTrialsConditionCodeShouldbeRemovedFromTheAccount()
        {
            var mockStudyListForSelection = GetMockStudies( 15 );

            var account = new Account { Activity = new RegistrationActivity() };
            var detailsView = MockRepository.GenerateStub<IClinicalTrialsDetailsView>();
            var detailsPresenter = new ClinicalTrialsDetailsPresenter( 900, account, detailsView,
                                                                      mockStudyListForSelection );
            detailsPresenter.SetStudySelectionList( mockStudyListForSelection );

            var selctedStudyFromSelectionList = mockStudyListForSelection[1];

            detailsPresenter.UpdateSelectedStudyInSelectionList( selctedStudyFromSelectionList );
            detailsPresenter.EnrollWithoutConsent();
            var addedStudy = detailsPresenter.PatientStudies.First( x => x.Code == selctedStudyFromSelectionList.Code );
            detailsPresenter.UpdateSelectedPatientStudy( addedStudy );

            detailsPresenter.RemoveSelectedPatientStudy();
            detailsPresenter.SaveChangesAndExit();
            var conditionCodes = account.ConditionCodes.Cast<ConditionCode>();

            Assert.IsTrue( conditionCodes.All(x => x.Code != ConditionCode.CONDITIONCODE_QUALIFYING_CLINICAL_TRIALS) );
        }

        [Test]
        [Category( "Fast" )]
        public void UserSelectsNoForAccountWithoutStudiies_WarningMessageShouldNotBeShown()
        {
            var mockery = new MockRepository();
            const int totalStudies = 15;

            var view = mockery.DynamicMock<IClinicalTrialsView>();
            var studyBroker = mockery.Stub<IResearchStudyBroker>();
            var featureManager = mockery.Stub<IClinicalTrialsFeatureManager>();
            var detailsView = mockery.Stub<IClinicalTrialsDetailsView>();
            var mockStudySelectionList = GetMockStudies( totalStudies );
            studyBroker.Expect( x => x.AllResearchStudies( Arg<long>.Is.Anything ) ).Return( mockStudySelectionList );
            var accountWithoutStudies = new Account
                                            {
                                                Activity = new RegistrationActivity(),
                                                IsPatientInClinicalResearchStudy = YesNoFlag.Blank
                                            };
            mockery.ReplayAll();

            var presenter = new ClinicalTrialsPresenter( view, detailsView, featureManager, accountWithoutStudies, 900,
                                                        studyBroker );

            presenter.UserChangedPatientInClinicalTrialsTo( YesNoFlag.No );
            view.AssertWasNotCalled( x => x.GetConfirmationForDiscardingPatientStudies() );
        }

        [Test]
        [Category( "Fast" )]
        public void UserSelectsNoForAccountWithStudies_WarningMessageShouldBeShown()
        {
            var mockery = new MockRepository();
            const int totalStudies = 15;

            var view = mockery.DynamicMock<IClinicalTrialsView>();
            var studyBroker = mockery.Stub<IResearchStudyBroker>();
            var featureManager = mockery.Stub<IClinicalTrialsFeatureManager>();
            var detailsView = mockery.Stub<IClinicalTrialsDetailsView>();
            var mockStudySelectionList = GetMockStudies( totalStudies );
            studyBroker.Expect( x => x.AllResearchStudies( Arg<long>.Is.Anything ) ).Return( mockStudySelectionList );

            var accountWithStudies = new Account
                                         {
                                             Activity = new RegistrationActivity(),
                                             IsPatientInClinicalResearchStudy = YesNoFlag.Yes
                                         };
            accountWithStudies.AddConsentedResearchStudy( new ConsentedResearchStudy( mockStudySelectionList[0], YesNoFlag.Yes ) );
            mockery.ReplayAll();

            var presenter = new ClinicalTrialsPresenter( view, detailsView, featureManager, accountWithStudies, 900, studyBroker );

            presenter.UserChangedPatientInClinicalTrialsTo( YesNoFlag.No );
            view.AssertWasCalled( x => x.GetConfirmationForDiscardingPatientStudies() );
        }

        [Test]
        [Category( "Fast" )]
        public void UserSelectsNoForAccountWithStudies_ViewDetailsButtonShouldNotBeVisible()
        {
            var mockery = new MockRepository();
            const int totalStudies = 15;

            var view = mockery.DynamicMock<IClinicalTrialsView>();
            var studyBroker = mockery.Stub<IResearchStudyBroker>();
            var featureManager = mockery.Stub<IClinicalTrialsFeatureManager>();
            var detailsView = mockery.Stub<IClinicalTrialsDetailsView>();
            var mockStudySelectionList = GetMockStudies( totalStudies );
            studyBroker.Expect( x => x.AllResearchStudies( Arg<long>.Is.Anything ) ).Return( mockStudySelectionList );

            var accountWithStudies = new Account
                                         {
                                             Activity = new RegistrationActivity(),
                                             IsPatientInClinicalResearchStudy = YesNoFlag.Yes
                                         };

            accountWithStudies.AddConsentedResearchStudy( new ConsentedResearchStudy( mockStudySelectionList[0], YesNoFlag.Yes ) );
            mockery.ReplayAll();

            var presenter = new ClinicalTrialsPresenter( view, detailsView, featureManager, accountWithStudies, 900, studyBroker );

            presenter.UserChangedPatientInClinicalTrialsTo( YesNoFlag.No );
            Assert.IsFalse( view.ViewDetailsCommandVisible, "The view details command should be disabled when no is selected" );
        }

        [Test]
        [Category( "Fast" )]
        public void UserSelectsYesForAccountWithStudiesAndAdmitDateAfterReleaseDate_ViewDetailsButtonShouldBeVisibleAndEnabled()
        {
            var mockery = new MockRepository();
            const int totalStudies = 15;

            var view = mockery.Stub<IClinicalTrialsView>();
            var studyBroker = mockery.Stub<IResearchStudyBroker>();
            var featureManager = mockery.DynamicMock<IClinicalTrialsFeatureManager>();
            var detailsView = mockery.Stub<IClinicalTrialsDetailsView>();
            var mockStudySelectionList = GetMockStudies( totalStudies );
            studyBroker.Expect( x => x.AllResearchStudies( Arg<long>.Is.Anything ) ).Return( mockStudySelectionList );

            var accountWithStudies = new Account
                                         {
                                             Activity = new RegistrationActivity(),
                                             AdmitDate = new DateTime( 2010, 01, 01 ),
                                             IsPatientInClinicalResearchStudy = YesNoFlag.Yes
                                         };

            featureManager.Expect( x => x.ShouldWeEnableClinicalResearchFields(
                Arg<DateTime>.Is.Equal( accountWithStudies.AdmitDate ), Arg<DateTime>.Is.Anything ) ).Return( true ).Repeat.Any();

            accountWithStudies.AddConsentedResearchStudy( new ConsentedResearchStudy( mockStudySelectionList[0],
                                                                                    YesNoFlag.Yes ) );
            mockery.ReplayAll();

            var presenter = new ClinicalTrialsPresenter( view, detailsView, featureManager, accountWithStudies, 900, studyBroker );

            presenter.UserChangedPatientInClinicalTrialsTo( YesNoFlag.Yes );
            Assert.IsTrue( view.ViewDetailsCommandVisible,
                          "The view details command should be Visible when yes is selected and Admit Date is after release date." );
            Assert.IsTrue( view.ViewDetailsCommandEnabled,
                          "The view details command should be Enabled when yes is selected and Admit Date is after release date." );
        }

        [Test]
        [Category( "Fast" )]
        public void PreExistingAccountWithStudiesAndUserModifiedAdmitDateToBePreReleaseDate_ViewDetailsButtonShouldBeVisibleAndDisabled()
        {
            var mockery = new MockRepository();
            const int totalStudies = 15;

            var view = mockery.Stub<IClinicalTrialsView>();
            var studyBroker = mockery.Stub<IResearchStudyBroker>();
            var featureManager = mockery.DynamicMock<IClinicalTrialsFeatureManager>();
            var detailsView = mockery.Stub<IClinicalTrialsDetailsView>();
            var mockStudySelectionList = GetMockStudies( totalStudies );
            studyBroker.Expect( x => x.AllResearchStudies( Arg<long>.Is.Anything ) ).Return( mockStudySelectionList );

            var accountWithStudies = new Account
                                         {
                                             Activity = new RegistrationActivity(),
                                             AdmitDate = new DateTime( 2009, 11, 01 ),
                                             IsPatientInClinicalResearchStudy = YesNoFlag.Yes
                                         };

            featureManager.Expect( x => x.ShouldWeEnableClinicalResearchFields(
                Arg<DateTime>.Is.Equal( accountWithStudies.AdmitDate ), Arg<DateTime>.Is.Anything ) ).Return( false ).Repeat.Any();

            accountWithStudies.AddConsentedResearchStudy( new ConsentedResearchStudy( mockStudySelectionList[0],
                                                                                    YesNoFlag.Yes ) );
            mockery.ReplayAll();

            var presenter = new ClinicalTrialsPresenter( view, detailsView, featureManager, accountWithStudies, 900,
                                                        studyBroker );

            presenter.UserChangedPatientInClinicalTrialsTo( YesNoFlag.Yes );
            Assert.IsTrue( view.ViewDetailsCommandVisible,
                          "The view details command should be Visible when yes is selected and Admit Date is before release date." );
            Assert.IsFalse( view.ViewDetailsCommandEnabled,
                          "The view details command should not be Enabled when yes is selected and Admit Date is before release date." );
        }

        [Test]
        [Category( "Fast" )]
        public void TestUserSelectsYesForIsPatientInClinicalStudyTwice_TheViewDetailsCommandShouldBeEnabled()
        {
            var mockery = new MockRepository();
            const int totalStudies = 15;

            var view = mockery.Stub<IClinicalTrialsView>();
            var studyBroker = mockery.Stub<IResearchStudyBroker>();
            var featureManager = mockery.Stub<IClinicalTrialsFeatureManager>();
            var detailsView = mockery.DynamicMock<IClinicalTrialsDetailsView>();
            var mockStudySelectionList = GetMockStudies( totalStudies );
            studyBroker.Expect( x => x.AllResearchStudies( Arg<long>.Is.Anything ) ).Return( mockStudySelectionList ).Repeat.
                Any();

            var accountWithStudies = new Account
                                         {
                                             Activity = new RegistrationActivity(),
                                             AdmitDate = new DateTime( 2010, 01, 01 )
                                         };

            featureManager.Expect( x => x.ShouldWeEnableClinicalResearchFields(
                Arg<DateTime>.Is.Equal( accountWithStudies.AdmitDate ), Arg<DateTime>.Is.Anything ) ).Return( true ).Repeat.Any();
            accountWithStudies.IsPatientInClinicalResearchStudy = YesNoFlag.Yes;
            accountWithStudies.AddConsentedResearchStudy( new ConsentedResearchStudy( mockStudySelectionList[0],
                                                                                    YesNoFlag.Yes ) );
            mockery.ReplayAll();

            var presenter = new ClinicalTrialsPresenter( view, detailsView, featureManager, accountWithStudies, 900,
                                                        studyBroker );
            presenter.UserChangedPatientInClinicalTrialsTo( YesNoFlag.Yes );
            presenter.UserChangedPatientInClinicalTrialsTo( YesNoFlag.Yes );
            Assert.IsTrue( view.ViewDetailsCommandVisible, "The view details command should be enabled when yes is selected" );
        }


        [Test]
        [Category( "Fast" )]
        public void TestUserSelectsBlankForIsPatientInStudyForNewPatientWhenBlankIsAlreadySelected_TheBlankOptionShouldBeselected()
        {
            var mockery = new MockRepository();
            const int totalStudies = 15;

            var view = mockery.Stub<IClinicalTrialsView>();
            var studyBroker = mockery.Stub<IResearchStudyBroker>();
            var featureManager = mockery.Stub<IClinicalTrialsFeatureManager>();
            var detailsView = mockery.DynamicMock<IClinicalTrialsDetailsView>();
            var mockStudySelectionList = GetMockStudies( totalStudies );
            studyBroker.Expect( x => x.AllResearchStudies( Arg<long>.Is.Anything ) ).Return( mockStudySelectionList ).Repeat.
                Any();

            var account = new Account
                              {
                                  Activity = new RegistrationActivity(),
                                  IsPatientInClinicalResearchStudy = YesNoFlag.Blank
                              };

            mockery.ReplayAll();

            var presenter = new ClinicalTrialsPresenter( view, detailsView, featureManager, account, 900,
                                                        studyBroker );
            presenter.UserChangedPatientInClinicalTrialsTo( YesNoFlag.Blank );
            Assert.IsTrue( presenter.PatientIsInClinicalStudy == YesNoFlag.Blank, "Is Patient in clinical study should be set to blank" );
        }

        [Test]
        [Category( "Fast" )]
        public void TestUserOpensStudyDetailsAddsAStudyAndCancelsSave_ShowWarningMessageIsTrue()
        {
            var mockery = new MockRepository();
            var view = mockery.DynamicMock<IClinicalTrialsView>();
            var studyBroker = mockery.Stub<IResearchStudyBroker>();
            var featureManager = mockery.Stub<IClinicalTrialsFeatureManager>();
            var detailsView = mockery.Stub<IClinicalTrialsDetailsView>();
            var mockStudySelectionList = GetMockStudies( 15 );
            studyBroker.Expect( x => x.AllResearchStudies( Arg<long>.Is.Anything ) ).Return( mockStudySelectionList ).Repeat. Any();
            var account = new Account { Activity = new RegistrationActivity() };
            mockery.ReplayAll();

            var presenter = new ClinicalTrialsPresenter( view, detailsView, featureManager, account, 0, studyBroker );

            presenter.ShowDetails();
            var selectedStudy = mockStudySelectionList[0];
            var detailsPresenter = presenter.DetailsPresenter;
            detailsPresenter.UpdateSelectedStudyInSelectionList( selectedStudy );
            detailsPresenter.EnrollWithConsent();
            Assert.IsTrue( detailsPresenter.ShowWarningMessage() );
        }

        [Test]
        [Category( "Fast" )]
        public void TestUserOpensStudyDetailsAndDoesNotMakeAnyChanges_ShowWarningMessageIsFalse()
        {
            var mockery = new MockRepository();
            var view = mockery.DynamicMock<IClinicalTrialsView>();
            var studyBroker = mockery.Stub<IResearchStudyBroker>();
            var featureManager = mockery.Stub<IClinicalTrialsFeatureManager>();
            var detailsView = mockery.Stub<IClinicalTrialsDetailsView>();
            var mockStudySelectionList = GetMockStudies( 15 );
            studyBroker.Expect( x => x.AllResearchStudies( Arg<long>.Is.Anything ) ).Return( mockStudySelectionList ).Repeat.
                Any();
            var account = new Account { Activity = new RegistrationActivity() };
            mockery.ReplayAll();

            var presenter = new ClinicalTrialsPresenter( view, detailsView, featureManager, account, 0, studyBroker );

            presenter.ShowDetails();
            var detailsPresenter = presenter.DetailsPresenter;
            Assert.IsFalse( detailsPresenter.ShowWarningMessage() );
        }

        [Test]
        public void TestUserOpensStudyDetailsAddsAStudyAndSavesChanges_ShowWarningMessageIsFalse()
        {
            var mockery = new MockRepository();
            var view = mockery.DynamicMock<IClinicalTrialsView>();
            var studyBroker = mockery.Stub<IResearchStudyBroker>();
            var featureManager = mockery.Stub<IClinicalTrialsFeatureManager>();
            var detailsView = mockery.Stub<IClinicalTrialsDetailsView>();
            var mockStudySelectionList = GetMockStudies( 15 );
            studyBroker.Expect( x => x.AllResearchStudies( Arg<long>.Is.Anything ) ).Return( mockStudySelectionList ).Repeat. Any();
            var account = new Account { Activity = new RegistrationActivity() };
            mockery.ReplayAll();

            var presenter = new ClinicalTrialsPresenter( view, detailsView, featureManager, account, 900, studyBroker );

            presenter.ShowDetails();
            var selectedStudy = mockStudySelectionList[0];
            var detailsPresenter = presenter.DetailsPresenter;
            detailsPresenter.UpdateSelectedStudyInSelectionList( selectedStudy );
            detailsPresenter.EnrollWithConsent();
            detailsPresenter.SaveChangesAndExit();
            Assert.IsFalse( detailsPresenter.ShowWarningMessage() );
        }

        [Test]
        [Category( "Fast" )]
        public void TestUserOpensStudyDetailsAddsAStudyAndDoesNotSaveChanges_ShowWarningMessageIsTrue()
        {
            var mockery = new MockRepository();
            var view = mockery.DynamicMock<IClinicalTrialsView>();
            var studyBroker = mockery.Stub<IResearchStudyBroker>();
            var featureManager = mockery.Stub<IClinicalTrialsFeatureManager>();
            var detailsView = mockery.Stub<IClinicalTrialsDetailsView>();
            var mockStudySelectionList = GetMockStudies( 15 );
            studyBroker.Expect( x => x.AllResearchStudies( Arg<long>.Is.Anything ) ).Return( mockStudySelectionList ).Repeat.
                Any();
            var account = new Account { Activity = new RegistrationActivity() };
            mockery.ReplayAll();

            var presenter = new ClinicalTrialsPresenter( view, detailsView, featureManager, account, 900, studyBroker );

            presenter.ShowDetails();
            var selectedStudy = mockStudySelectionList[0];
            var detailsPresenter = presenter.DetailsPresenter;
            detailsPresenter.UpdateSelectedStudyInSelectionList( selectedStudy );
            detailsPresenter.EnrollWithConsent();
            Assert.IsTrue( detailsPresenter.ShowWarningMessage() );
        }
        private static IList<ResearchStudy> GetMockStudies( int numberOfStudies )
        {
            var unicodeRangeForASCIICapitalLetters = new UnicodeRange( 0x0041, 0x005A );

            var propertiesForStudyCode = new StringProperties();
            propertiesForStudyCode.UnicodeRanges.Add(unicodeRangeForASCIICapitalLetters);
            propertiesForStudyCode.MinNumberOfCodePoints = propertiesForStudyCode.MaxNumberOfCodePoints = 9;

            var propertiesForDescription = new StringProperties();
            propertiesForDescription.UnicodeRanges.Add(unicodeRangeForASCIICapitalLetters);
            propertiesForDescription.MinNumberOfCodePoints = propertiesForDescription.MaxNumberOfCodePoints = 40;

            var propertiesForSponsor = new StringProperties();
            propertiesForSponsor.UnicodeRanges.Add(unicodeRangeForASCIICapitalLetters);
            propertiesForSponsor.MinNumberOfCodePoints = propertiesForSponsor.MaxNumberOfCodePoints = 25;

            var studies = new List<ResearchStudy>();

            for ( int i = 0; i < numberOfStudies; i++ )
            {
                string studyCode = StringFactory.GenerateRandomString( propertiesForStudyCode, i );
                string description = StringFactory.GenerateRandomString( propertiesForDescription, i );
                string researchSponsor = StringFactory.GenerateRandomString( propertiesForSponsor, i );
                var researchStudy = new ResearchStudy( studyCode, description, researchSponsor );

                studies.Add( researchStudy );
            }

            return studies;
        }

        [Test]
        [Category( "Fast" )]
        public void TestEnrollCommandsEnabled_WhenFacilityHasNoResearchStudies_EnrollCommandsEnabledShouldBeFalse()
        {
            var mockery = new MockRepository();
            var view = mockery.DynamicMock<IClinicalTrialsView>();
            var studyBroker = mockery.Stub<IResearchStudyBroker>();
            var featureManager = mockery.Stub<IClinicalTrialsFeatureManager>();
            var detailsView = mockery.Stub<IClinicalTrialsDetailsView>();
            var mockStudySelectionList = new List<ResearchStudy>();
            studyBroker.Expect( x => x.AllResearchStudies( Arg<long>.Is.Anything ) ).Return( mockStudySelectionList ).Repeat.
                Any();
            var account = new Account { Activity = new RegistrationActivity() };
            mockery.ReplayAll();

            var presenter = new ClinicalTrialsPresenter( view, detailsView, featureManager, account, 900, studyBroker );

            presenter.UserChangedPatientInClinicalTrialsTo( YesNoFlag.Yes );
            Assert.IsFalse( detailsView.EnrollCommandsEnabled );
        }

        [Test]
        [Category( "Fast" )]
        public void TestEnrollCommandsEnabled_WhenFacilityHasResearchStudies_EnrollCommandsEnabledShouldBeTrue()
        {
            var mockery = new MockRepository();
            var view = mockery.DynamicMock<IClinicalTrialsView>();
            var studyBroker = mockery.Stub<IResearchStudyBroker>();
            var featureManager = mockery.Stub<IClinicalTrialsFeatureManager>();
            var detailsView = mockery.Stub<IClinicalTrialsDetailsView>();
            var mockStudySelectionList = GetMockStudies( 15 );
            studyBroker.Expect( x => x.AllResearchStudies( Arg<long>.Is.Anything ) ).Return( mockStudySelectionList ).Repeat. Any();
            var account = new Account { Activity = new RegistrationActivity() };
            mockery.ReplayAll();

            var presenter = new ClinicalTrialsPresenter( view, detailsView, featureManager, account, 900, studyBroker );

            presenter.UserChangedPatientInClinicalTrialsTo( YesNoFlag.Yes );
            Assert.IsTrue( detailsView.EnrollCommandsEnabled );
        }
    }
}
