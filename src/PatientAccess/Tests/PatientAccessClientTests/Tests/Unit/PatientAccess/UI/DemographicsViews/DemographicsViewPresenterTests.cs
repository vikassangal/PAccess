using System;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.UI.DemographicsViews;
using Rhino.Mocks;

namespace Tests.Unit.PatientAccess.UI.DemographicsViews
{
    /// <summary>
    /// Summary description for DemographicsViewPresenterTests
    /// </summary>
    [TestFixture]
    public class DemographicsViewPresenterTests
    {
        #region Tests

        [Test]
        [Category( "Fast" )]
        public void TestHandlePreOpDateDisplayWithDateChange_WhenAccountCreatedDateLessThanFeatureImplementationDate_ShouldNotCallAnyOtherViewMethods()
        {
            IDemographicsView demographicsView = GetStubDemographicsViewWithAdmitDateAndPreOpDateEqual();
            DemographicsViewPresenter demographicsViewPresenter = new DemographicsViewPresenter( demographicsView );

            demographicsView.ModelAccount.AccountCreatedDate = new DateTime( 2000, 01, 01 );
            DateTime admitDateCleared = DateTime.MinValue;

            demographicsViewPresenter.HandlePreOpDateDisplayWithDateChange( true, false, admitDateCleared, demographicsView.ModelAccount.PreopDate );
            demographicsView.AssertWasNotCalled( view => view.AutoSetPreOpDateWithAdmitDate() );
            demographicsView.AssertWasNotCalled( view => view.ShowPreOpDateAfterAdmitDateErrorMessage() );
            demographicsView.AssertWasNotCalled( view => view.SetBlankPreOpDate() );
            demographicsView.AssertWasNotCalled( view => view.SetPreOpDateFocus() );
        }

        # region Test HandlePreOpDateDisplayWithDateChange when Model.AdmitDate = Model.PreOpDate & Change Admit Date on UI
        [Test]
        [Category( "Fast" )]
        public void TestHandlePreOpDateDisplayWithDateChange_WhenModelAdmitDateAndPreOpDateEqual_AndAdmitDateIsCleared_ShouldAutoSetPreOpDate()
        {
            IDemographicsView demographicsView = GetStubDemographicsViewWithAdmitDateAndPreOpDateEqual();
            DemographicsViewPresenter demographicsViewPresenter = new DemographicsViewPresenter( demographicsView );

            DateTime admitDateCleared = DateTime.MinValue;

            demographicsViewPresenter.HandlePreOpDateDisplayWithDateChange( true, false, admitDateCleared, demographicsView.ModelAccount.PreopDate );
            demographicsView.AssertWasCalled( view => view.AutoSetPreOpDateWithAdmitDate() );
            demographicsView.AssertWasNotCalled( view => view.ShowPreOpDateAfterAdmitDateErrorMessage() );
            demographicsView.AssertWasNotCalled( view => view.SetBlankPreOpDate() );
            demographicsView.AssertWasNotCalled( view => view.SetPreOpDateFocus() );
        }

        [Test]
        [Category( "Fast" )]
        public void TestHandlePreOpDateDisplayWithDateChange_WhenModelAdmitDateAndPreOpDateEqual_AndAdmitDateIsMadeGreater_ShouldAutoSetPreOpDate()
        {
            IDemographicsView demographicsView = GetStubDemographicsViewWithAdmitDateAndPreOpDateEqual();
            DemographicsViewPresenter demographicsViewPresenter = new DemographicsViewPresenter( demographicsView );

            DateTime admitDateGreater = new DateTime( 2009, 12, 13 );

            demographicsViewPresenter.HandlePreOpDateDisplayWithDateChange( true, false, admitDateGreater, demographicsView.ModelAccount.PreopDate );
            demographicsView.AssertWasCalled( view => view.AutoSetPreOpDateWithAdmitDate() );
            demographicsView.AssertWasNotCalled( view => view.ShowPreOpDateAfterAdmitDateErrorMessage() );
            demographicsView.AssertWasNotCalled( view => view.SetBlankPreOpDate() );
            demographicsView.AssertWasNotCalled( view => view.SetPreOpDateFocus() );
        }

        [Test]
        [Category( "Fast" )]
        public void TestHandlePreOpDateDisplayWithDateChange_WhenModelAdmitDateAndPreOpDateEqual_AndAdmitDateIsMadeLesser_ShouldAutoSetPreOpDate()
        {
            IDemographicsView demographicsView = GetStubDemographicsViewWithAdmitDateAndPreOpDateEqual();
            DemographicsViewPresenter demographicsViewPresenter = new DemographicsViewPresenter( demographicsView );

            DateTime admitDateLesser = new DateTime( 2009, 12, 11 );

            demographicsViewPresenter.HandlePreOpDateDisplayWithDateChange( true, false, admitDateLesser, demographicsView.ModelAccount.PreopDate );
            demographicsView.AssertWasCalled( view => view.AutoSetPreOpDateWithAdmitDate() );
            demographicsView.AssertWasNotCalled( view => view.ShowPreOpDateAfterAdmitDateErrorMessage() );
            demographicsView.AssertWasNotCalled( view => view.SetBlankPreOpDate() );
            demographicsView.AssertWasNotCalled( view => view.SetPreOpDateFocus() );
        }
        # endregion Test HandlePreOpDateDisplayWithDateChange when Model.AdmitDate = Model.PreOpDate & Change Admit Date on UI

        # region Test HandlePreOpDateDisplayWithDateChange when Model.AdmitDate <> Model.PreOpDate & Change Admit Date on UI
        [Test]
        [Category( "Fast" )]
        public void TestHandlePreOpDateDisplayWithDateChange_WhenModelAdmitDateAndPreOpDateNotEqual_AndAdmitDateIsCleared_ShouldAutoSetPreOpDate()
        {
            IDemographicsView demographicsView = GetStubDemographicsViewWithAdmitDateAndPreOpDateNotEqual();
            DemographicsViewPresenter demographicsViewPresenter = new DemographicsViewPresenter( demographicsView );

            DateTime admitDateCleared = DateTime.MinValue;

            demographicsViewPresenter.HandlePreOpDateDisplayWithDateChange( true, false, admitDateCleared, demographicsView.ModelAccount.PreopDate );
            demographicsView.AssertWasNotCalled( view => view.AutoSetPreOpDateWithAdmitDate() );
            demographicsView.AssertWasNotCalled( view => view.ShowPreOpDateAfterAdmitDateErrorMessage() );
            demographicsView.AssertWasCalled( view => view.SetBlankPreOpDate() );
            demographicsView.AssertWasCalled( view => view.SetPreOpDateFocus() );
        }

        [Test]
        [Category( "Fast" )]
        public void TestHandlePreOpDateDisplayWithDateChange_WhenModelAdmitDateAndPreOpDateNotEqual_AndAdmitDateIsMadeGreater_ShouldNotCallAnyOtherViewMethods()
        {
            IDemographicsView demographicsView = GetStubDemographicsViewWithAdmitDateAndPreOpDateNotEqual();
            DemographicsViewPresenter demographicsViewPresenter = new DemographicsViewPresenter( demographicsView );

            DateTime admitDateGreater = new DateTime( 2009, 12, 13 );

            demographicsViewPresenter.HandlePreOpDateDisplayWithDateChange( true, false, admitDateGreater, demographicsView.ModelAccount.PreopDate );
            demographicsView.AssertWasNotCalled( view => view.AutoSetPreOpDateWithAdmitDate() );
            demographicsView.AssertWasNotCalled( view => view.ShowPreOpDateAfterAdmitDateErrorMessage() );
            demographicsView.AssertWasNotCalled( view => view.SetBlankPreOpDate() );
            demographicsView.AssertWasNotCalled( view => view.SetPreOpDateFocus() );
        }

        [Test]
        [Category( "Fast" )]
        public void TestHandlePreOpDateDisplayWithDateChange_WhenModelAdmitDateAndPreOpDateNotEqual_AndAdmitDateIsMadeEqual_ShouldNotCallAnyOtherViewMethods()
        {
            IDemographicsView demographicsView = GetStubDemographicsViewWithAdmitDateAndPreOpDateNotEqual();
            DemographicsViewPresenter demographicsViewPresenter = new DemographicsViewPresenter( demographicsView );

            DateTime admitDateEqual = demographicsView.ModelAccount.PreopDate;

            demographicsViewPresenter.HandlePreOpDateDisplayWithDateChange( true, false, admitDateEqual, demographicsView.ModelAccount.PreopDate );
            demographicsView.AssertWasNotCalled( view => view.AutoSetPreOpDateWithAdmitDate() );
            demographicsView.AssertWasNotCalled( view => view.ShowPreOpDateAfterAdmitDateErrorMessage() );
            demographicsView.AssertWasNotCalled( view => view.SetBlankPreOpDate() );
            demographicsView.AssertWasNotCalled( view => view.SetPreOpDateFocus() );
        }

        [Test]
        [Category( "Fast" )]
        public void TestHandlePreOpDateDisplayWithDateChange_WhenModelAdmitDateAndPreOpDateNotEqual_AndAdmitDateIsMadeLesser_ShouldClearPreOpDateAndSetFocus()
        {
            IDemographicsView demographicsView = GetStubDemographicsViewWithAdmitDateAndPreOpDateNotEqual();
            DemographicsViewPresenter demographicsViewPresenter = new DemographicsViewPresenter( demographicsView );

            DateTime admitDateLesser = new DateTime( 2009, 12, 10 );

            demographicsViewPresenter.HandlePreOpDateDisplayWithDateChange( true, false, admitDateLesser, demographicsView.ModelAccount.PreopDate );
            demographicsView.AssertWasNotCalled( view => view.AutoSetPreOpDateWithAdmitDate() );
            demographicsView.AssertWasNotCalled( view => view.ShowPreOpDateAfterAdmitDateErrorMessage() );
            demographicsView.AssertWasCalled( view => view.SetBlankPreOpDate() );
            demographicsView.AssertWasCalled( view => view.SetPreOpDateFocus() );
        }
        # endregion Test HandlePreOpDateDisplayWithDateChange when Model.AdmitDate <> Model.PreOpDate & Change Admit Date on UI


        # region Test HandlePreOpDateDisplayWithDateChange when Model.AdmitDate = Model.PreOpDate & Change PreOpDate on UI
        [Test]
        [Category( "Fast" )]
        public void TestHandlePreOpDateDisplayWithDateChange_WhenModelAdmitDateAndPreOpDateEqual_AndPreOpDateIsCleared_ShouldNotCallAnyOtherViewMethods()
        {
            IDemographicsView demographicsView = GetStubDemographicsViewWithAdmitDateAndPreOpDateEqual();
            DemographicsViewPresenter demographicsViewPresenter = new DemographicsViewPresenter( demographicsView );

            DateTime preOpDateCleared = DateTime.MinValue;

            demographicsViewPresenter.HandlePreOpDateDisplayWithDateChange( false, true, demographicsView.ModelAccount.AdmitDate, preOpDateCleared );
            demographicsView.AssertWasNotCalled( view => view.AutoSetPreOpDateWithAdmitDate() );
            demographicsView.AssertWasNotCalled( view => view.ShowPreOpDateAfterAdmitDateErrorMessage() );
            demographicsView.AssertWasNotCalled( view => view.SetBlankPreOpDate() );
            demographicsView.AssertWasNotCalled( view => view.SetPreOpDateFocus() );
        }

        [Test]
        [Category( "Fast" )]
        public void TestHandlePreOpDateDisplayWithDateChange_WhenModelAdmitDateAndPreOpDateEqual_AndPreOpDateIsMadeGreater_ShouldDisplayErrorMsgAndClearPreOpDate()
        {
            IDemographicsView demographicsView = GetStubDemographicsViewWithAdmitDateAndPreOpDateEqual();
            DemographicsViewPresenter demographicsViewPresenter = new DemographicsViewPresenter( demographicsView );

            DateTime preOpDateGreater = new DateTime( 2009, 12, 13 );

            demographicsViewPresenter.HandlePreOpDateDisplayWithDateChange( false, true, demographicsView.ModelAccount.AdmitDate, preOpDateGreater );
            demographicsView.AssertWasNotCalled( view => view.AutoSetPreOpDateWithAdmitDate() );
            demographicsView.AssertWasCalled( view => view.ShowPreOpDateAfterAdmitDateErrorMessage() );
            demographicsView.AssertWasCalled( view => view.SetBlankPreOpDate() );
            demographicsView.AssertWasCalled( view => view.SetPreOpDateFocus() );
        }

        [Test]
        [Category( "Fast" )]
        public void TestHandlePreOpDateDisplayWithDateChange_WhenModelAdmitDateAndPreOpDateEqual_AndPreOpDateIsMadeLesser_ShouldNotCallAnyOtherViewMethods()
        {
            IDemographicsView demographicsView = GetStubDemographicsViewWithAdmitDateAndPreOpDateEqual();
            DemographicsViewPresenter demographicsViewPresenter = new DemographicsViewPresenter( demographicsView );

            DateTime preOpDateLesser = new DateTime( 2009, 12, 11 );

            demographicsViewPresenter.HandlePreOpDateDisplayWithDateChange( false, true, demographicsView.ModelAccount.AdmitDate, preOpDateLesser );
            demographicsView.AssertWasNotCalled( view => view.AutoSetPreOpDateWithAdmitDate() );
            demographicsView.AssertWasNotCalled( view => view.ShowPreOpDateAfterAdmitDateErrorMessage() );
            demographicsView.AssertWasNotCalled( view => view.SetBlankPreOpDate() );
            demographicsView.AssertWasNotCalled( view => view.SetPreOpDateFocus() );
        }
        # endregion Test HandlePreOpDateDisplayWithDateChange when Model.AdmitDate = Model.PreOpDate & Change PreOpDate on UI

        # region Test HandlePreOpDateDisplayWithDateChange when Model.AdmitDate <> Model.PreOpDate & Change PreOpDate on UI
        [Test]
        [Category( "Fast" )]
        public void TestHandlePreOpDateDisplayWithDateChange_WhenModelAdmitDateAndPreOpDateNotEqual_AndPreOpDateIsCleared_ShouldNotCallAnyOtherViewMethods()
        {
            IDemographicsView demographicsView = GetStubDemographicsViewWithAdmitDateAndPreOpDateNotEqual();
            DemographicsViewPresenter demographicsViewPresenter = new DemographicsViewPresenter( demographicsView );

            DateTime preOpDateCleared = DateTime.MinValue;

            demographicsViewPresenter.HandlePreOpDateDisplayWithDateChange( false, true, demographicsView.ModelAccount.AdmitDate, preOpDateCleared );
            demographicsView.AssertWasNotCalled( view => view.AutoSetPreOpDateWithAdmitDate() );
            demographicsView.AssertWasNotCalled( view => view.ShowPreOpDateAfterAdmitDateErrorMessage() );
            demographicsView.AssertWasNotCalled( view => view.SetBlankPreOpDate() );
            demographicsView.AssertWasNotCalled( view => view.SetPreOpDateFocus() );
        }

        [Test]
        [Category( "Fast" )]
        public void TestHandlePreOpDateDisplayWithDateChange_WhenModelAdmitDateAndPreOpDateNotEqual_AndPreOpDateIsMadeEqual_ShouldNotCallAnyOtherViewMethods()
        {
            IDemographicsView demographicsView = GetStubDemographicsViewWithAdmitDateAndPreOpDateNotEqual();
            DemographicsViewPresenter demographicsViewPresenter = new DemographicsViewPresenter( demographicsView );

            DateTime preOpDateEqual = demographicsView.ModelAccount.AdmitDate;

            demographicsViewPresenter.HandlePreOpDateDisplayWithDateChange( false, true, demographicsView.ModelAccount.AdmitDate, preOpDateEqual );
            demographicsView.AssertWasNotCalled( view => view.AutoSetPreOpDateWithAdmitDate() );
            demographicsView.AssertWasNotCalled( view => view.ShowPreOpDateAfterAdmitDateErrorMessage() );
            demographicsView.AssertWasNotCalled( view => view.SetBlankPreOpDate() );
            demographicsView.AssertWasNotCalled( view => view.SetPreOpDateFocus() );
        }

        [Test]
        [Category( "Fast" )]
        public void TestHandlePreOpDateDisplayWithDateChange_WhenModelAdmitDateAndPreOpDateNotEqual_AndPreOpDateIsMadeGreater_ShouldDisplayErrorMsgAndClearPreOpDate()
        {
            IDemographicsView demographicsView = GetStubDemographicsViewWithAdmitDateAndPreOpDateNotEqual();
            DemographicsViewPresenter demographicsViewPresenter = new DemographicsViewPresenter( demographicsView );

            DateTime preOpDateGreater = new DateTime( 2009, 12, 13 );

            demographicsViewPresenter.HandlePreOpDateDisplayWithDateChange( false, true, demographicsView.ModelAccount.AdmitDate, preOpDateGreater );
            demographicsView.AssertWasNotCalled( view => view.AutoSetPreOpDateWithAdmitDate() );
            demographicsView.AssertWasCalled( view => view.ShowPreOpDateAfterAdmitDateErrorMessage() );
            demographicsView.AssertWasCalled( view => view.SetBlankPreOpDate() );
            demographicsView.AssertWasCalled( view => view.SetPreOpDateFocus() );
        }

        [Test]
        [Category( "Fast" )]
        public void TestHandlePreOpDateDisplayWithDateChange_WhenModelAdmitDateAndPreOpDateNotEqual_AndPreOpDateIsMadeLesser_ShouldNotCallAnyOtherViewMethods()
        {
            IDemographicsView demographicsView = GetStubDemographicsViewWithAdmitDateAndPreOpDateNotEqual();
            DemographicsViewPresenter demographicsViewPresenter = new DemographicsViewPresenter( demographicsView );

            DateTime preOpDateLesser = new DateTime( 2009, 12, 11 );

            demographicsViewPresenter.HandlePreOpDateDisplayWithDateChange( false, true, demographicsView.ModelAccount.AdmitDate, preOpDateLesser );
            demographicsView.AssertWasNotCalled( view => view.AutoSetPreOpDateWithAdmitDate() );
            demographicsView.AssertWasNotCalled( view => view.ShowPreOpDateAfterAdmitDateErrorMessage() );
            demographicsView.AssertWasNotCalled( view => view.SetBlankPreOpDate() );
            demographicsView.AssertWasNotCalled( view => view.SetPreOpDateFocus() );
        }
        # endregion Test HandlePreOpDateDisplayWithDateChange when Model.AdmitDate <> Model.PreOpDate & Change PreOpDate on UI

        # region Test HandlePreOpDateDisplayWithDateChange & HandlePreOpDateDisplayForUpdateView during UpdateView
        [Test]
        [Category( "Fast" )]
        public void TestHandlePreOpDateDisplayWithDateChange_WhenUpdateView_AndModelAdmitDateIsMinValue_ShouldSetPreOpDateEqualToAdmitDate()
        {
            IDemographicsView demographicsView = GetStubDemographicsViewWithBlankAdmitDateAndPreOpDate();
            DemographicsViewPresenter demographicsViewPresenter = new DemographicsViewPresenter( demographicsView );

            demographicsViewPresenter.HandlePreOpDateDisplayWithDateChange( false, false, demographicsView.ModelAccount.AdmitDate, DateTime.MinValue );
            Assert.IsTrue( demographicsView.ModelAccount.PreopDate == demographicsView.ModelAccount.AdmitDate );
            demographicsView.AssertWasCalled( view => view.SetBlankPreOpDate() );
        }
        # endregion Test HandlePreOpDateDisplayWithDateChange during UpdateView


        # region Test HandlePreOpDateDisplayForUpdateView during UpdateView
        [Test]
        [Category( "Fast" )]
        public void TestHandlePreOpDateDisplayForUpdateView_WhenModelAdmitDateNotMinValueAndPreOpDateIsMinValueAndEnabled_ShouldSetPreOpDateEqualToAdmitDate()
        {
            IDemographicsView demographicsView = GetStubDemographicsViewWithAdmitDateAndPreOpDateNotEqual();
            DemographicsViewPresenter demographicsViewPresenter = new DemographicsViewPresenter( demographicsView );

            demographicsView.ModelAccount.PreopDate = DateTime.MinValue;

            demographicsViewPresenter.HandlePreOpDateDisplayForUpdateView();
            Assert.IsTrue( demographicsView.ModelAccount.PreopDate == demographicsView.ModelAccount.AdmitDate );
            demographicsView.AssertWasNotCalled( view => view.SetBlankPreOpDate() );
            demographicsView.AssertWasCalled( view => view.SetPreopDateFromModel() );
        }

        [Test]
        [Category( "Fast" )]
        public void TestHandlePreOpDateDisplayForUpdateView_WhenActivatePreRegistrationActivityAndLoadingModelData_ShouldNotSetPreOpDateEqualToAdmitDate()
        {
            IDemographicsView demographicsView = GetStubDemographicsViewWithAdmitDateAndPreOpDateNotEqual();
            DemographicsViewPresenter demographicsViewPresenter = new DemographicsViewPresenter( demographicsView );

            demographicsView.LoadingModelData = true;
            demographicsView.ModelAccount.PreopDate = DateTime.MinValue;
            demographicsView.ModelAccount.Activity = new RegistrationActivity
                                                     {
                                                         AssociatedActivityType = typeof( ActivatePreRegistrationActivity )
                                                     };

            demographicsViewPresenter.HandlePreOpDateDisplayForUpdateView();
            Assert.IsFalse( demographicsView.ModelAccount.PreopDate == demographicsView.ModelAccount.AdmitDate );
            demographicsView.AssertWasNotCalled( view => view.SetBlankPreOpDate() );
            demographicsView.AssertWasNotCalled( view => view.SetPreopDateFromModel() );
        }
        # endregion Test HandlePreOpDateDisplayWithDateChange during UpdateView

        # region Test HandlePreOpDateDisplayForActivatePreRegistrationInitialViewLoad
        [Test]
        [Category( "Fast" )]
        public void TestHandlePreOpDateDisplayForActivatePreRegistration_WhenPreOpDateIsMinValueAndEnabled_ShouldSetPreOpDateEqualToAdmitDate()
        {
            IDemographicsView demographicsView = GetStubDemographicsViewWithAdmitDateAndPreOpDateNotEqual();
            DemographicsViewPresenter demographicsViewPresenter = new DemographicsViewPresenter( demographicsView );

            demographicsView.ModelAccount.PreopDate = DateTime.MinValue;
            demographicsView.PreRegAdmitDate = new DateTime( 2009, 12, 11 );

            demographicsViewPresenter.HandlePreOpDateDisplayForActivatePreRegistrationInitialViewLoad();
            Assert.IsTrue( demographicsView.ModelAccount.PreopDate == demographicsView.ModelAccount.AdmitDate );
            demographicsView.AssertWasCalled( view => view.SetPreopDateFromModel() );
            demographicsView.AssertWasNotCalled( view => view.SetBlankPreOpDate() );
            demographicsView.AssertWasNotCalled( view => view.AutoSetPreOpDateWithAdmitDate() );
            demographicsView.AssertWasNotCalled( view => view.SetPreOpDateFocus() );
        }

        [Test]
        [Category( "Fast" )]
        public void TestHandlePreOpDateDisplayForActivatePreRegistration_WhenAdmitDateAndPreOpDateIsMinValueAndEnabled_ShouldSetBlankPreOpDateEqualToAdmitDate()
        {
            IDemographicsView demographicsView = GetStubDemographicsViewWithBlankAdmitDateAndPreOpDate();
            DemographicsViewPresenter demographicsViewPresenter = new DemographicsViewPresenter( demographicsView );

            demographicsViewPresenter.HandlePreOpDateDisplayForActivatePreRegistrationInitialViewLoad();
            Assert.IsTrue( demographicsView.ModelAccount.PreopDate == demographicsView.ModelAccount.AdmitDate );
            demographicsView.AssertWasCalled( view => view.SetBlankPreOpDate() );
            demographicsView.AssertWasCalled( view => view.AutoSetPreOpDateWithAdmitDate() );
            demographicsView.AssertWasNotCalled( view => view.SetPreopDateFromModel() );
            demographicsView.AssertWasNotCalled( view => view.SetPreOpDateFocus() );
        }

        [Test]
        [Category( "Fast" )]
        public void TestHandlePreOpDateDisplayForActivatePreRegistration_WhenPreRegAdmitDateAndPreRegPreOpDateAreEqual_ShouldSetPreOpDateEqualToAdmitDate()
        {
            IDemographicsView demographicsView = GetStubDemographicsViewWithAdmitDateAndPreOpDateNotEqual();
            DemographicsViewPresenter demographicsViewPresenter = new DemographicsViewPresenter( demographicsView );

            demographicsView.PreRegAdmitDate = new DateTime( 2009, 12, 11 );

            demographicsViewPresenter.HandlePreOpDateDisplayForActivatePreRegistrationInitialViewLoad();
            demographicsView.AssertWasCalled( view => view.AutoSetPreOpDateWithAdmitDate() );
            demographicsView.AssertWasNotCalled( view => view.SetBlankPreOpDate() );
            demographicsView.AssertWasNotCalled( view => view.SetPreopDateFromModel() );
            demographicsView.AssertWasNotCalled( view => view.SetPreOpDateFocus() );
        }

        [Test]
        [Category( "Fast" )]
        public void TestHandlePreOpDateDisplayForActivatePreRegistration_WhenPreRegAdmitDateAndPreRegPreOpDateAreNotEqual_ShouldClearPreOpDateAndSetFocus()
        {
            IDemographicsView demographicsView = GetStubDemographicsViewWithAdmitDateAndPreOpDateNotEqual();
            DemographicsViewPresenter demographicsViewPresenter = new DemographicsViewPresenter( demographicsView );

            demographicsView.ModelAccount.AdmitDate = new DateTime( 2009, 12, 10 );
            demographicsView.PreRegAdmitDate = new DateTime( 2009, 12, 10 );

            demographicsViewPresenter.HandlePreOpDateDisplayForActivatePreRegistrationInitialViewLoad();
            demographicsView.AssertWasNotCalled( view => view.AutoSetPreOpDateWithAdmitDate() );
            demographicsView.AssertWasCalled( view => view.SetBlankPreOpDate() );
            demographicsView.AssertWasNotCalled( view => view.SetPreopDateFromModel() );
            demographicsView.AssertWasCalled( view => view.SetPreOpDateFocus() );
        }

        [Test]
        [Category( "Fast" )]
        public void TestHandlePreOpDateDisplayForActivatePreRegistration_WhenIsNotActivatePreRegistrationActivity_ShouldRetainOldPreOpDateValue()
        {
            IDemographicsView demographicsView = GetStubDemographicsViewWithAdmitDateAndPreOpDateNotEqual();
            DemographicsViewPresenter demographicsViewPresenter = new DemographicsViewPresenter( demographicsView );

            demographicsView.ModelAccount.Activity.AssociatedActivityType = typeof( MaintenanceActivity );

            demographicsViewPresenter.HandlePreOpDateDisplayForActivatePreRegistrationInitialViewLoad();
            Assert.IsFalse( demographicsView.ModelAccount.PreopDate == demographicsView.ModelAccount.AdmitDate );
            demographicsView.AssertWasNotCalled( view => view.SetBlankPreOpDate() );
            demographicsView.AssertWasNotCalled( view => view.SetPreopDateFromModel() );
            demographicsView.AssertWasNotCalled( view => view.AutoSetPreOpDateWithAdmitDate() );
            demographicsView.AssertWasNotCalled( view => view.SetPreOpDateFocus() );
        }
        # endregion Test HandlePreOpDateDisplayWithDateChange during UpdateView

        # region Test GetPreOpDateFromUI
        [Test]
        [Category( "Fast" )]
        public void TestGetPreOpDateFromUI_ShouldCallGetPreopDateFromUIOfView()
        {
            IDemographicsView demographicsView = GetStubDemographicsViewWithBlankAdmitDateAndPreOpDate();
            DemographicsViewPresenter demographicsViewPresenter = new DemographicsViewPresenter( demographicsView );

            demographicsViewPresenter.GetPreOpDateFromUI();
            demographicsView.AssertWasCalled( view => view.GetPreopDateFromUI() );
        }
        # endregion Test GetPreOpDateFromUI

        # region Test UpdatePreOpDate
        [Test]
        public void TestUpdatePreOpDate_WhenPreOpDateIsEmpty_ShouldSetPreopDateToMinValue()
        {
            IDemographicsView demographicsView = GetStubDemographicsViewWithBlankAdmitDateAndPreOpDate();
            DemographicsViewPresenter demographicsViewPresenter = new DemographicsViewPresenter( demographicsView );

            demographicsViewPresenter.UpdatePreOpDate( String.Empty );
            Assert.IsTrue( demographicsView.ModelAccount.PreopDate == DateTime.MinValue );
        }
        [Test]
        public void TestUpdatePreOpDate_WhenPreOpDateIsValid_ShouldCallGetDateAndTimeFromOfView()
        {
            IDemographicsView demographicsView = GetStubDemographicsViewWithAdmitDateAndPreOpDateNotEqual();
            DemographicsViewPresenter demographicsViewPresenter = new DemographicsViewPresenter( demographicsView );

            demographicsViewPresenter.UpdatePreOpDate( "05051999" );
            Assert.IsTrue( demographicsView.ModelAccount.PreopDate == DateTime.MinValue );
            demographicsView.AssertWasCalled( view => view.GetDateAndTimeFrom( Arg<string>.Is.Anything, Arg<string>.Is.Anything ) );
        }
        # endregion Test UpdatePreOpDate


        # region Test PopulatePreOpDate
        [Test]
        [Category( "Fast" )]
        public void TestPopulatePreOpDate_WhenPreOpDateIsEmpty_ShouldCallEnablePreOpDateAndSetBlankPreOpDate()
        {
            IDemographicsView demographicsView = GetStubDemographicsViewWithBlankAdmitDateAndPreOpDate();
            DemographicsViewPresenter demographicsViewPresenter = new DemographicsViewPresenter( demographicsView );

            demographicsViewPresenter.PopulatePreOpDate( true );
            Assert.IsTrue( demographicsView.ModelAccount.PreopDate == DateTime.MinValue );
            demographicsView.AssertWasCalled( view => view.EnablePreOpDate( Arg<Boolean>.Is.Anything ) );
            demographicsView.AssertWasCalled( view => view.SetBlankPreOpDate() );
        }
        # endregion Test PopulatePreOpDate

        # region Test ValidatePreOpDate
        [Test]
        [Category( "Fast" )]
        public void TestValidatePreOpDate_WhenpreOpDateTimePickerFocusedIsTrue_ShouldReturnFalse()
        {
            IDemographicsView demographicsView = GetStubDemographicsViewWithBlankAdmitDateAndPreOpDate();
            DemographicsViewPresenter demographicsViewPresenter = new DemographicsViewPresenter( demographicsView );

            bool result = demographicsViewPresenter.ValidatePreOpDate( true, false, false, String.Empty );
            Assert.IsFalse( result );
        }

        [Test]
        [Category( "Fast" )]
        public void TestValidatePreOpDate_WhenpreOpDateTimePickerFocusedIsFalseAndIncompletePreOpDate_ShouldReturnFalseAndCallIncompletePreOpDateMethods()
        {
            IDemographicsView demographicsView = GetStubDemographicsViewWithAdmitDateAndPreOpDateNotEqual();
            DemographicsViewPresenter demographicsViewPresenter = new DemographicsViewPresenter( demographicsView );

            bool result = demographicsViewPresenter.ValidatePreOpDate( false, false, false, "050599" );
            Assert.IsFalse( result );
            Assert.IsFalse( demographicsViewPresenter.preOpDateComplete );
            demographicsView.AssertWasNotCalled( view => view.GetPreopDateFromUI() );
            demographicsView.AssertWasCalled( view => view.SetPreOpDateErrBgColor() );
            demographicsView.AssertWasCalled( view => view.ShowPreOpDateIncompleteErrorMessage() );
            demographicsView.AssertWasCalled( view => view.SetPreOpDateFocus() );
        }

        [Test]
        [Category( "Fast" )]
        public void TestValidatePreOpDate_WhenpreOpDateTimePickerFocusedIsFalseAndInvalidPreOpDate_ShouldReturnFalseAndCallInvalidPreOpDateMethods()
        {
            IDemographicsView demographicsView = GetStubDemographicsViewWithAdmitDateAndPreOpDateNotEqual();
            DemographicsViewPresenter demographicsViewPresenter = new DemographicsViewPresenter( demographicsView );

            bool result = demographicsViewPresenter.ValidatePreOpDate( false, false, false, "05051999" );
            Assert.IsFalse( result );
            Assert.IsTrue( demographicsViewPresenter.preOpDateComplete );
            demographicsView.AssertWasCalled( view => view.GetPreopDateFromUI() );
            Assert.IsFalse( demographicsViewPresenter.preOpDateIsValid );
            demographicsView.AssertWasCalled( view => view.SetPreOpDateErrBgColor() );
            demographicsView.AssertWasCalled( view => view.ShowPreOpDateInvalidErrorMessage() );
            demographicsView.AssertWasCalled( view => view.SetPreOpDateFocus() );
        }
        # endregion Test ValidatePreOpDate


        # region Test VerifyPreOpDateIsComplete
        [Test]
        [Category( "Fast" )]
        public void TestVerifyPreOpDateIsComplete_WhenPreOpDateIsMinValue_ShouldReturnTrueAndSetPreOpDateCompleteToFalse()
        {
            IDemographicsView demographicsView = GetStubDemographicsViewWithAdmitDateAndPreOpDateNotEqual();
            DemographicsViewPresenter demographicsViewPresenter = new DemographicsViewPresenter( demographicsView );

            bool result = demographicsViewPresenter.VerifyPreOpDateIsComplete( String.Empty );

            Assert.IsTrue( result );
            Assert.IsFalse( demographicsViewPresenter.preOpDateComplete );
            demographicsView.AssertWasCalled( view => view.SetBlankPreOpDate() );
            demographicsView.AssertWasCalled( view => view.SetPreOpDateNormalBgColor() );
        }

        [Test]
        [Category( "Fast" )]
        public void TestVerifyPreOpDateIsComplete_WhenPreOpDateLengthIsLessThan10_ShouldReturnFalseAndCallPreOpDateIncompleteMethods()
        {
            IDemographicsView demographicsView = GetStubDemographicsViewWithAdmitDateAndPreOpDateNotEqual();
            DemographicsViewPresenter demographicsViewPresenter = new DemographicsViewPresenter( demographicsView );

            bool result = demographicsViewPresenter.VerifyPreOpDateIsComplete( "050599" );

            Assert.IsFalse( result );
            Assert.IsFalse( demographicsViewPresenter.preOpDateComplete );
            demographicsView.AssertWasCalled( view => view.SetPreOpDateErrBgColor() );
            demographicsView.AssertWasCalled( view => view.ShowPreOpDateIncompleteErrorMessage() );
            demographicsView.AssertWasCalled( view => view.SetPreOpDateFocus() );
        }

        [Test]
        [Category( "Fast" )]
        public void TestVerifyPreOpDateIsComplete_WhenPreOpDateLengthIsAValidDate_ShouldReturnTrueAndSetPreOpDateCompleteToTrue()
        {
            IDemographicsView demographicsView = GetStubDemographicsViewWithAdmitDateAndPreOpDateNotEqual();
            DemographicsViewPresenter demographicsViewPresenter = new DemographicsViewPresenter( demographicsView );

            bool result = demographicsViewPresenter.VerifyPreOpDateIsComplete( "05051999" );

            Assert.IsTrue( result );
            Assert.IsTrue( demographicsViewPresenter.preOpDateComplete );
            demographicsView.AssertWasNotCalled( view => view.SetPreOpDateErrBgColor() );
            demographicsView.AssertWasNotCalled( view => view.ShowPreOpDateIncompleteErrorMessage() );
            demographicsView.AssertWasNotCalled( view => view.SetPreOpDateFocus() );
            demographicsView.AssertWasNotCalled( view => view.SetBlankPreOpDate() );
            demographicsView.AssertWasNotCalled( view => view.SetPreOpDateNormalBgColor() );
        }
        # endregion Test VerifyPreOpDateIsComplete


        # region Test VerifyPreOpDateIsValidForDisplay
        [Test]
        [Category( "Fast" )]
        public void TestVerifyPreOpDateIsValidForDisplay_WhenPreOpDateIsMinValue_ShouldReturnFalseAndCallPreOpDateInvalidMethods()
        {
            IDemographicsView demographicsView = GetStubDemographicsViewWithAdmitDateAndPreOpDateNotEqual();
            DemographicsViewPresenter demographicsViewPresenter = new DemographicsViewPresenter( demographicsView );

            bool result = demographicsViewPresenter.VerifyPreOpDateIsValidForDisplay( DateTime.MinValue );

            Assert.IsFalse( result );
            Assert.IsFalse( demographicsViewPresenter.preOpDateIsValid );
            demographicsView.AssertWasCalled( view => view.SetPreOpDateErrBgColor() );
            demographicsView.AssertWasCalled( view => view.ShowPreOpDateInvalidErrorMessage() );
            demographicsView.AssertWasCalled( view => view.SetPreOpDateFocus() );
        }

        [Test]
        [Category( "Fast" )]
        public void TestVerifyPreOpDateIsValidForDisplay_WhenPreOpDateIsInvalid_ShouldReturnFalseAndCallPreOpDateInvalidMethods()
        {
            IDemographicsView demographicsView = GetStubDemographicsViewWithAdmitDateAndPreOpDateNotEqual();
            DemographicsViewPresenter demographicsViewPresenter = new DemographicsViewPresenter( demographicsView );

            DateTime invalidDate = new DateTime( 0001, 01, 01 );
            bool result = demographicsViewPresenter.VerifyPreOpDateIsValidForDisplay( invalidDate );

            Assert.IsFalse( result );
            Assert.IsFalse( demographicsViewPresenter.preOpDateIsValid );
            demographicsView.AssertWasCalled( view => view.SetPreOpDateErrBgColor() );
            demographicsView.AssertWasCalled( view => view.ShowPreOpDateInvalidErrorMessage() );
            demographicsView.AssertWasCalled( view => view.SetPreOpDateFocus() );
        }

        [Test]
        [Category( "Fast" )]
        public void TestVerifyPreOpDateIsValidForDisplay_WhenPreOpDateIsvalid_ShouldReturnTrueAndNotCallPreOpDateInvalidMethods()
        {
            IDemographicsView demographicsView = GetStubDemographicsViewWithAdmitDateAndPreOpDateNotEqual();
            DemographicsViewPresenter demographicsViewPresenter = new DemographicsViewPresenter( demographicsView );

            DateTime validDate = new DateTime( 2000, 01, 01 );
            bool result = demographicsViewPresenter.VerifyPreOpDateIsValidForDisplay( validDate );

            Assert.IsTrue( result );
            Assert.IsTrue( demographicsViewPresenter.preOpDateIsValid );
            demographicsView.AssertWasNotCalled( view => view.SetPreOpDateErrBgColor() );
            demographicsView.AssertWasNotCalled( view => view.ShowPreOpDateInvalidErrorMessage() );
            demographicsView.AssertWasNotCalled( view => view.SetPreOpDateFocus() );
        }
        # endregion Test VerifyPreOpDateIsValidForDisplay

        #endregion Tests

        #region Helper Methods

        private static IDemographicsView GetStubDemographicsViewWithAdmitDateAndPreOpDateEqual()
        {
            var demographicsView = MockRepository.GenerateStub<IDemographicsView>();
            demographicsView.ModelAccount = new Account
            {
                Activity = new RegistrationActivity(),
                AdmitDate = new DateTime( 2009, 12, 12 ),
                PreopDate = new DateTime( 2009, 12, 12 )
            };
            return demographicsView;
        }

        private static IDemographicsView GetStubDemographicsViewWithAdmitDateAndPreOpDateNotEqual()
        {
            var demographicsView = MockRepository.GenerateStub<IDemographicsView>();
            demographicsView.ModelAccount = new Account
            {
                Activity = new RegistrationActivity(),
                AdmitDate = new DateTime( 2009, 12, 12 ),
                PreopDate = new DateTime( 2009, 12, 11 )
            };
            return demographicsView;
        }

        private static IDemographicsView GetStubDemographicsViewWithBlankAdmitDateAndPreOpDate()
        {
            var demographicsView = MockRepository.GenerateStub<IDemographicsView>();
            demographicsView.ModelAccount = new Account
            {
                Activity = new RegistrationActivity(),
                AdmitDate = DateTime.MinValue,
                PreopDate = DateTime.MinValue
            };
            return demographicsView;
        }
        #endregion Helper Methods

    }
}
