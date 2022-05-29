using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using NUnit.Framework;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Locking;
using PatientAccess.Domain.Parties;
using PatientAccess.UI.NewEmployersManagement;
using Rhino.Mocks;

namespace Tests.Unit.PatientAccess.UI.NewEmployersManagement
{
    /// <summary>
    /// Summary description for NewEmployersManagementPresenterTests
    /// </summary>
    [TestFixture]
    [Category( "Fast" )]
    public class NewEmployersManagementPresenterTests
    {
        #region Tests

        [Test]
        public void TestClearAddressAndPhoneNumber_SelectedEmployerHasAnAddress_ShouldClearAddressAndPhone_DisablePhoneAndAddressFieldsAndMoveAddressAndPhoneOperation()
        {
            var newEmployers = CreateStubNewEmployerListWithFullAddresses();
            var presenter = GetInitializedPresenterWithStubDependencies(newEmployers);

            presenter.SelectEmployerOnNewEmployerView(newEmployers.Values[0]);
            var selectedEmployer = newEmployers.Values[1];
            presenter.SelectEmployerOnNewEmployerView(selectedEmployer);

            presenter.ClearAddressAndPhoneNumber();

            Assert.AreEqual(string.Empty, presenter.NewEmployerListView.SelectedEmployerAddress, "The clear operation should clear the address field in the view.");
            Assert.AreEqual(string.Empty, presenter.NewEmployerListView.SelectedEmployerPhoneNumber, "The clear operation should clear the phone field in the view.");
            Assert.IsFalse(presenter.NewEmployerListView.MoveAddressAndPhoneEnabled, "The Move Address And Phone operation should be disabled");
            Assert.IsFalse(presenter.NewEmployerListView.SelectedEmployerPhoneFieldEnabled, "The phone field should be disabled");
            Assert.IsFalse(presenter.NewEmployerListView.SelectedEmployerAddressFieldEnabled, "The address field should be disabled");


            presenter.EmployerBroker.AssertWasNotCalled(x => x.GetAllEmployersWith(Arg<long>.Is.Anything, Arg<String>.Is.Anything));
        }

        [Test]
        public void TestConstructor()
        {
            var mockNewEmployerListView = MockRepository.GenerateStub<INewEmployersListView>();
            var mockMasterEmployerListView = MockRepository.GenerateStub<IMasterEmployersListView>();
            var mockEmployerBroker = MockRepository.GenerateStub<IEmployerBroker>();

            User user = GetStubUser();

            var presenter = new NewEmployersManagementPresenter(user, mockNewEmployerListView, mockMasterEmployerListView, mockEmployerBroker);

            Assert.IsNotNull(presenter.NewEmployerListView);
            Assert.IsNotNull(presenter.NewEmployers);
            Assert.IsNotNull(presenter.MasterEmployerListView);
            Assert.IsNotNull(presenter.MasterListEmployers);
            Assert.IsNotNull(presenter.Facility);
            Assert.IsNotNull(presenter.FollowupUnit);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestConstructor_WithNullBroker_ShouldThrowException()
        {
            var mockNewEmployerListView = MockRepository.GenerateStub<INewEmployersListView>();
            var mockMasterEmployerListView = MockRepository.GenerateStub<IMasterEmployersListView>();
            User user = GetStubUser();
            new NewEmployersManagementPresenter(user, mockNewEmployerListView, mockMasterEmployerListView, null);

        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestConstructor_WithNullNewEmployerListView_ShouldThrowException()
        {
            var mockMasterEmployerListView = MockRepository.GenerateStub<IMasterEmployersListView>();

            User user = GetStubUser();

            new NewEmployersManagementPresenter(user, null, mockMasterEmployerListView, null);

        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestConstructorWithNullNewMasterEmployerListViewShouldThrowException()
        {
            var mockNewEmployerListView = MockRepository.GenerateStub<INewEmployersListView>();

            User user = GetStubUser();

            new NewEmployersManagementPresenter(user, mockNewEmployerListView, null, null);
        }


        [Test]
        public void TestDeleteAllNewEmployers_WhenMoreThanOneEmployers()
        {
            var newEmployers = CreateStubNewEmployerListWithFullAddresses();
            var presenter = GetInitializedPresenterWithStubDependencies(newEmployers);

            foreach (var employer in newEmployers.Values)
            {
                presenter.SelectEmployerOnNewEmployerView(employer);
                presenter.DeleteSelectedNewEmployer();
            }

            Assert.AreEqual(0, presenter.NewEmployers.Count, "All employers should be deleted");
            Assert.AreEqual(String.Empty, presenter.NewEmployerListView.SelectedEmployerName, "Selected new details should be blank after all employers are deleted");
            Assert.AreEqual(String.Empty, presenter.NewEmployerListView.SelectedEmployerNationalId, "Selected new details should be blank after all employers are deleted");
        }

        [Test]
        public void TestDeleteAllNewEmployers_WhenThereIsOnlyOneEmployer()
        {
            var newEmployers = CreateStubNewEmployerListWithFullAddresses();
            RemoveAllButOneFrom(newEmployers);
            var presenter = GetInitializedPresenterWithStubDependencies(newEmployers);
            var selectedEmployer = newEmployers.Values[0];

            presenter.SelectEmployerOnNewEmployerView(selectedEmployer);
            presenter.DeleteSelectedNewEmployer();

            Assert.AreEqual(0, presenter.NewEmployers.Count, "All employers should be deleted");
            Assert.AreEqual(String.Empty, presenter.NewEmployerListView.SelectedEmployerName, "Selected new details should be blank after all employers are deleted");
            Assert.AreEqual(String.Empty, presenter.NewEmployerListView.SelectedEmployerNationalId, "Selected new details should be blank after all employers are deleted");

            presenter.NewEmployerListView.AssertWasCalled(x => x.ShowEmployersWithoutSelection(Arg<IList<NewEmployerEntry>>.Is.Anything), y => y.Repeat.AtLeastOnce());
        }

        [Test]
        public void TestDeleteSelectedNewEmployer()
        {
            var newEmployers = CreateStubNewEmployerListWithFullAddresses();
            var presenter = GetInitializedPresenterWithStubDependencies(newEmployers);

            presenter.SelectEmployerOnNewEmployerView(newEmployers.Values[1]);
            presenter.NewEmployerListView.UndoEnabled = false;
            presenter.DeleteSelectedNewEmployer();

            Assert.IsFalse(presenter.NewEmployers.Contains(newEmployers.Values[1]), "The selected employer should have been deleted");
            Assert.IsNull(presenter.SelectedNewEmployerEntry, "Selected new employer should be set to null after a deletion");
            Assert.AreEqual(string.Empty, presenter.NewEmployerListView.SelectedEmployerName, "Selected employer name should be set to blank");
            Assert.AreEqual(string.Empty, presenter.NewEmployerListView.SelectedEmployerNationalId, "Selected employer national id should be set to blank");
            Assert.AreEqual(typeof(DeleteNewEmployerCommand), presenter.LastCommand.GetType(), "Last command should be set to an instance of the delete command");
            Assert.IsTrue(presenter.Commands.Contains(presenter.LastCommand), "the last command should be added to the commands collection of the presenter");
            Assert.IsTrue(presenter.NewEmployerListView.UndoEnabled, "The undo operation should now be enabled.");
        }

        [Test]
        public void TestFinishForMoveAddressAndPhoneToMasterList()
        {
            var newEmployers = CreateStubNewEmployerListWithFullAddresses();
            var existingEmployersInSearchResult = CreateStubExistingEmployerListWithFullAddresses();
            var presenter = GetInitializedPresenterWithStubDependencies(newEmployers, existingEmployersInSearchResult);

            presenter.NewEmployers = newEmployers.Values;
            presenter.MasterListEmployers = existingEmployersInSearchResult.Values;
            presenter.UpdateNewEmployersViewWithoutAnySelection();
            presenter.UpdateMasterEmployerViewWithFirstItemSelected();
            var selectedEmployerEntry = newEmployers.Values[0];
            presenter.SelectEmployerOnNewEmployerView(selectedEmployerEntry);
            presenter.SelectEmployerOnMasterEmployerView(existingEmployersInSearchResult.Values[0]);

            var selectedNewEmployer = presenter.SelectedNewEmployerEntry;
            var selectedMasterListEmployer = presenter.SelectedMasterListEmployer;
            var contactPointToAdd = EmployerUtilities.GetFirstContactPointFrom(selectedNewEmployer.Employer);

            presenter.MoveAddressAndPhoneToMasterList();
            presenter.SaveSynchronous();
            presenter.EmployerBroker.AssertWasCalled(x => x.AddContactPointForEmployer(selectedMasterListEmployer, contactPointToAdd, presenter.Facility.Code));
            presenter.EmployerBroker.AssertWasCalled(
                x => x.DeleteEmployerForApproval(
                         Arg<Employer>.Is.Equal(selectedEmployerEntry.Employer),
                         Arg<String>.Is.Equal(presenter.Facility.Code)));
        }

        [Test]
        public void TestFinishForMoveNewEmployerToMasterList()
        {
            var newEmployers = CreateStubNewEmployerListWithFullAddresses();
            var presenter = GetInitializedPresenterWithStubDependencies(newEmployers);
            presenter.MasterListEmployers = CreateStubExistingEmployerListWithFullAddresses().Values;
            presenter.UpdateNewEmployersViewWithoutAnySelection();
            presenter.SelectEmployerOnNewEmployerView(newEmployers.Values[0]);
            presenter.UpdateMasterEmployerViewWithFirstItemSelected();
            presenter.SelectEmployerOnNewEmployerView(newEmployers.Values[1]);

            var employerToMove = presenter.SelectedNewEmployerEntry;
            var contactPointToAdd = EmployerUtilities.GetFirstContactPointFrom(employerToMove.Employer);

            presenter.MoveSelectedNewEmployerToMasterList();
            presenter.SaveSynchronous();

            presenter.EmployerBroker.AssertWasCalled(
                x => x.AddNewEmployer(
                         Arg<Employer>.Is.Equal(employerToMove.Employer),
                         Arg<FollowupUnit>.Is.Anything,
                         Arg<string>.Is.Equal(presenter.Facility.Code),
                         Arg<String>.Is.Equal("PACCESS")));


            presenter.EmployerBroker.AssertWasCalled(
                x => x.AddContactPointForEmployer(
                         employerToMove.Employer,
                         contactPointToAdd,
                         presenter.Facility.Code));

            presenter.EmployerBroker.AssertWasCalled(
                x => x.DeleteEmployerForApproval(
                         Arg<Employer>.Is.Equal(employerToMove.Employer),
                         Arg<String>.Is.Equal(presenter.Facility.Code)));
        }

        [Test]
        public void TestFinishWithDeleteNewEmployer()
        {
            var newEmployers = CreateStubNewEmployerListWithFullAddresses();
            RemoveAllButOneFrom(newEmployers);
            var presenter = GetInitializedPresenterWithStubDependencies(newEmployers, CreateStubExistingEmployerListWithFullAddresses());
            var selectedEmployer = newEmployers.Values[0];

            presenter.SelectEmployerOnNewEmployerView(selectedEmployer);
            presenter.DeleteSelectedNewEmployer();
            presenter.NewEmployerListView.FinishEnabled = true;

            presenter.SaveSynchronous();

            presenter.EmployerBroker.AssertWasCalled(x => x.DeleteEmployerForApproval(selectedEmployer.Employer, presenter.Facility.Code));
            presenter.NewEmployerListView.AssertWasCalled(x => x.ShowEmployersWithoutSelection(presenter.NewEmployers), y => y.Repeat.AtLeastOnce());

            Assert.AreEqual(typeof(NullCommand), presenter.LastCommand.GetType(), "The last command should be set to a null command after a finish operation");
            Assert.IsFalse(presenter.NewEmployerListView.UndoEnabled, "The undo operation should no longer be available");
        }


        [Test]
        public void TestFinish_WhenANewEmployersDetailsHaveBeenEdited_ShouldSaveTheNewEmployer()
        {
            var newEmployers = CreateStubNewEmployerListWithFullAddresses();
            var presenter = GetInitializedPresenterWithStubDependencies(newEmployers);
            var employertoChange = newEmployers.Values[0].Employer;
            string newName = Guid.NewGuid().ToString();
            employertoChange.Name = newName;
            employertoChange.NationalId = Guid.NewGuid().ToString();

            presenter.SaveSynchronous();

            presenter.EmployerBroker.AssertWasCalled(x => x.DeleteAllEmployersForApproval(presenter.Facility.Code));
            presenter.EmployerBroker.AssertWasCalled(x => x.SaveEmployersForApproval(newEmployers.Values, presenter.Facility.Code));
        }


        [Test]
        public void TestMoveAddressAndPhoneToMasterList_WhenNewEmployerHasAnAddress()
        {
            var newEmployers = CreateStubNewEmployerListWithFullAddresses();
            var existingEmployersInSearchResult = CreateStubExistingEmployerListWithFullAddresses();
            var presenter = GetInitializedPresenterWithStubDependencies(newEmployers, existingEmployersInSearchResult);

            presenter.NewEmployers = newEmployers.Values;
            presenter.MasterListEmployers = existingEmployersInSearchResult.Values;
            presenter.UpdateNewEmployersViewWithoutAnySelection();
            presenter.UpdateMasterEmployerViewWithFirstItemSelected();
            presenter.SelectEmployerOnNewEmployerView(newEmployers.Values[0]);
            presenter.SelectEmployerOnMasterEmployerView(existingEmployersInSearchResult.Values[0]);

            var selectedNewEmployer = presenter.SelectedNewEmployerEntry;
            var selectedMasterListEmployer = presenter.SelectedMasterListEmployer;
            var expectedAddress = EmployerUtilities.GetAddressOfFirstContactPointFrom(selectedNewEmployer);

            int expectedNewEmployerListCount = presenter.NewEmployers.Count - 1;

            presenter.LastCommand = new NullCommand();
            presenter.NewEmployerListView.UndoEnabled = false;
            presenter.MoveAddressAndPhoneToMasterList();
            var addressesForselectedMasterListEmployer = EmployerUtilities.GetAddressesFrom(selectedMasterListEmployer);

            Assert.AreEqual(expectedNewEmployerListCount, presenter.NewEmployers.Count, "An employer should have been removed from the new employers list");
            Assert.IsFalse(presenter.NewEmployers.Contains(selectedNewEmployer), "The employer should have been moved out of the new employer list");
            Assert.IsTrue(addressesForselectedMasterListEmployer.Contains(expectedAddress), "The address should have been added to the selected employer on the master list");
            var address = EmployerUtilities.GetAddressOfFirstContactPointFrom(selectedMasterListEmployer);
            Assert.AreEqual(expectedAddress, address, "The new address should be the first address on the master list employer");
            presenter.MasterEmployerListView.AssertWasCalled(x => x.SelectFirstAddress());

            Assert.AreEqual(typeof(MoveAddressAndPhoneToMasterListCommand), presenter.LastCommand.GetType(), "The last command should be set to an object of the type move employer command");
            Assert.IsTrue(presenter.Commands.Contains(presenter.LastCommand), "The last command should be added to the command collection");
            Assert.AreEqual(selectedMasterListEmployer, presenter.SelectedMasterListEmployer, "The employer for which the address was added should still be selected after the move operation");

            Assert.IsTrue(presenter.NewEmployerListView.UndoEnabled, "The undo operation should now be enabled.");

            presenter.NewEmployerListView.AssertWasCalled(x => x.ShowEmployersWithoutSelection(presenter.NewEmployers), y => y.Repeat.AtLeastOnce());
            presenter.MasterEmployerListView.AssertWasCalled(x => x.ShowEmployersWithoutSelection(presenter.MasterListEmployers), y => y.Repeat.AtLeastOnce());
            presenter.MasterEmployerListView.AssertWasCalled(x => x.SelectEmployer(selectedMasterListEmployer), y => y.Repeat.AtLeastOnce());
        }

        [Test]
        public void TestMoveAddressAndPhoneToMasterList_WhenNewEmployerPhoneNumberHasBeenChangedOnTheView()
        {
            var newEmployers = CreateStubNewEmployerListWithFullAddresses();
            var existingEmployersInSearchResult = CreateStubExistingEmployerListWithFullAddresses();
            var presenter = GetInitializedPresenterWithStubDependencies(newEmployers, existingEmployersInSearchResult);

            presenter.NewEmployers = newEmployers.Values;
            presenter.MasterListEmployers = existingEmployersInSearchResult.Values;
            presenter.UpdateNewEmployersViewWithoutAnySelection();
            presenter.UpdateMasterEmployerViewWithFirstItemSelected();

            var selectedNewEmployerEntry = newEmployers.Values[0];
            presenter.SelectEmployerOnNewEmployerView(selectedNewEmployerEntry);
            presenter.SelectEmployerOnMasterEmployerView(existingEmployersInSearchResult.Values[0]);

            EmployerUtilities.SetAllPhoneNumbersTo(selectedNewEmployerEntry.Employer, "1111111111");
            var selectedMasterListEmployer = presenter.SelectedMasterListEmployer;

            var expectedPhoneNumber = new PhoneNumber("9999999999");

            presenter.NewEmployerListView.SelectedEmployerPhoneNumber = expectedPhoneNumber.AsUnformattedString();
            presenter.MoveAddressAndPhoneToMasterList();

            var contactpoints = EmployerUtilities.GetContactPointsFrom(selectedMasterListEmployer);
            Assert.IsTrue(contactpoints.Any(x => x.PhoneNumber.AsUnformattedString() == expectedPhoneNumber.AsUnformattedString()));
        }

        [Test]
        public void TestMoveAddressAndPhoneToMasterList_WhenNewEmployerDoesNotHaveAnAddress_MoveAddressOperationShouldBeDisabled()
        {
            var newEmployers = CreateStubNewEmployerListWithFullAddresses();

            Employer employerWithoutAnAddress = new Employer(01, DateTime.Now, "Employer without an address", "SomeID", 02);
            NewEmployerEntry employerEntryWithoutAnAddress = new NewEmployerEntry(employerWithoutAnAddress, "userID");
            newEmployers.Add(employerEntryWithoutAnAddress.Employer.Name, employerEntryWithoutAnAddress);
            var presenter = GetInitializedPresenterWithStubDependencies(newEmployers);
            presenter.NewEmployers = newEmployers.Values;

            presenter.UpdateNewEmployersViewWithoutAnySelection();
            presenter.UpdateMasterEmployerViewWithFirstItemSelected();

            presenter.LastCommand = new NullCommand();
            presenter.NewEmployerListView.MoveAddressAndPhoneEnabled = true;
            presenter.SelectEmployerOnNewEmployerView(employerEntryWithoutAnAddress);

            Assert.IsTrue(presenter.NewEmployers.Contains(employerEntryWithoutAnAddress), "The employer should not have been moved out of the new employer list");
            Assert.AreNotEqual(typeof(MoveAddressAndPhoneToMasterListCommand), presenter.LastCommand.GetType(), "The last command should not be set to an object of the type move employer command");
            Assert.IsFalse(presenter.NewEmployerListView.MoveAddressAndPhoneEnabled, "The move address and phone operation should be disabled as the selected new employer does not have an address");
        }

        [Test]
        public void TestMoveAddressAndPhoneToMasterList_WhenNewEmployerHasAnAddressAndMasterListEmployerSelectedFirst_MoveAddressOperationShouldBeEnabled()
        {
            var newEmployers = CreateStubNewEmployerListWithFullAddresses();
            var searchResults = CreateStubExistingEmployerListWithFullAddresses();
            var presenter = GetInitializedPresenterWithStubDependencies(newEmployers);
            NewEmployerEntry employerEntryWithAnAddress = newEmployers.Values[0];
            presenter.NewEmployers = newEmployers.Values;

            presenter.NewEmployerListView.MoveAddressAndPhoneEnabled = false;

            presenter.SelectEmployerOnMasterEmployerView(searchResults.Values[0]);
            presenter.SelectEmployerOnNewEmployerView(employerEntryWithAnAddress);

            Assert.IsTrue(presenter.NewEmployerListView.MoveAddressAndPhoneEnabled, "The move address and phone operation should be enabled.");
        }

        [Test]
        public void TestMoveAddressAndPhoneToMasterList_WhenNewEmployerIsSelectedFirst_MoveAddressOperationShouldBeEnabled()
        {
            var newEmployers = CreateStubNewEmployerListWithFullAddresses();
            var searchResults = CreateStubExistingEmployerListWithFullAddresses();
            var presenter = GetInitializedPresenterWithStubDependencies(newEmployers);
            NewEmployerEntry employerEntryWithAnAddress = newEmployers.Values[0];
            presenter.NewEmployers = newEmployers.Values;

            presenter.NewEmployerListView.MoveAddressAndPhoneEnabled = false;

            presenter.SelectEmployerOnNewEmployerView(employerEntryWithAnAddress);
            presenter.SelectEmployerOnMasterEmployerView(searchResults.Values[0]);

            Assert.IsTrue(presenter.NewEmployerListView.MoveAddressAndPhoneEnabled, "The move address and phone operation should be enabled.");
        }

        [Test]
        public void TestMoveNewEmployerToMasterList()
        {
            var newEmployers = CreateStubNewEmployerListWithFullAddresses();
            var presenter = GetInitializedPresenterWithStubDependencies(newEmployers);
            presenter.MasterListEmployers = CreateStubExistingEmployerListWithFullAddresses().Values;
            presenter.UpdateNewEmployersViewWithoutAnySelection();
            presenter.SelectEmployerOnNewEmployerView(newEmployers.Values[0]);
            presenter.UpdateMasterEmployerViewWithFirstItemSelected();
            presenter.SelectEmployerOnNewEmployerView(newEmployers.Values[1]);

            var employerToMove = presenter.SelectedNewEmployerEntry;
            int expectedNewEmployerListCount = presenter.NewEmployers.Count - 1;

            presenter.LastCommand = new NullCommand();
            presenter.NewEmployerListView.UndoEnabled = false;
            presenter.MoveSelectedNewEmployerToMasterList();
            presenter.MasterEmployerListView.AssertWasCalled(x => x.ShowEmployersWithoutSelection(Arg<IList<Employer>>.Is.Anything), y => y.Repeat.AtLeastOnce());

            Assert.AreEqual(1, presenter.MasterListEmployers.Count, "The master list should contain only one employer after a move employer operation.");
            Assert.AreEqual(expectedNewEmployerListCount, presenter.NewEmployers.Count, "An employer should have been removed from the new employers list");
            Assert.AreEqual(1, presenter.MasterListEmployers.Count, "There should only be one employer in the master list after a move all info i.e. the moved employer");
            Assert.AreEqual(string.Empty, presenter.MasterEmployerListView.SearchString, "The search string should be cleared after a move all info operation");
            Assert.IsTrue(presenter.MasterListEmployers.Contains(employerToMove.Employer), "The selected new employer should have been added to the master employer list");
            Assert.IsTrue(presenter.NewEmployerListView.UndoEnabled, "The undo operation should now be enabled.");
            Assert.IsFalse(presenter.NewEmployers.Contains(employerToMove), "The employer should have been moved out of the new employer list");

            Assert.AreEqual(typeof(MoveEmployerToMasterListCommand), presenter.LastCommand.GetType(), "The last command should be set to an object of the type move employer command");
            Assert.IsTrue(presenter.Commands.Contains(presenter.LastCommand), "The last command should be added to the command collection");

        }

        [Test]
        public void TestMoveNewEmployerToMasterList_WhenEmployerNameAndNationalIdIsChangedOnTheView()
        {
            var newEmployers = CreateStubNewEmployerListWithFullAddresses();
            var presenter = GetInitializedPresenterWithStubDependencies(newEmployers);
            presenter.MasterListEmployers = CreateStubExistingEmployerListWithFullAddresses().Values;
            presenter.UpdateNewEmployersViewWithoutAnySelection();
            presenter.SelectEmployerOnNewEmployerView(newEmployers.Values[0]);
            presenter.UpdateMasterEmployerViewWithFirstItemSelected();


            string expectedEmployerName = Guid.NewGuid().ToString();
            string expectedNationalId = Guid.NewGuid().ToString();
            presenter.NewEmployerListView.SelectedEmployerName = expectedEmployerName;
            presenter.NewEmployerListView.SelectedEmployerNationalId = expectedNationalId;
            presenter.MoveSelectedNewEmployerToMasterList();

            Assert.AreEqual(expectedEmployerName, presenter.SelectedMasterListEmployer.Name);
            Assert.AreEqual(expectedNationalId, presenter.SelectedMasterListEmployer.NationalId);
        }

        [Test]
        public void TestMoveNewEmployerToMasterList_WhenEmployerPhoneNumberIsChangedOnTheView()
        {
            var newEmployers = CreateStubNewEmployerListWithFullAddresses();
            var presenter = GetInitializedPresenterWithStubDependencies(newEmployers);
            presenter.MasterListEmployers = CreateStubExistingEmployerListWithFullAddresses().Values;
            presenter.UpdateNewEmployersViewWithoutAnySelection();
            var selectedNewEmployerEntry = newEmployers.Values[0];
            presenter.SelectEmployerOnNewEmployerView(selectedNewEmployerEntry);
            presenter.UpdateMasterEmployerViewWithFirstItemSelected();

            EmployerUtilities.SetAllPhoneNumbersTo(selectedNewEmployerEntry.Employer, "1111111111");

            var expectedPhoneNumber = new PhoneNumber("9999999999");

            presenter.NewEmployerListView.SelectedEmployerPhoneNumber = expectedPhoneNumber.AsUnformattedString();
            presenter.MoveSelectedNewEmployerToMasterList();

            var contactpoints = EmployerUtilities.GetContactPointsFrom(presenter.SelectedMasterListEmployer);
            Assert.IsTrue(contactpoints.Any(x => x.PhoneNumber.AsUnformattedString() == expectedPhoneNumber.AsUnformattedString()));
        }

        [Test]
        public void TestSearchForEmployerInMasterListCore_WithNonBlankSearchString_ShouldPerformSearch()
        {
            var newEmployers = CreateStubNewEmployerListWithFullAddresses();
            var presenter = GetInitializedPresenterWithStubDependencies(newEmployers);
            const string searchString = "TA";

            presenter.MasterEmployerListView.SearchString = searchString;

            presenter.SearchMasterEmployerListBefore();
            presenter.SearchMasterEmployerListCore(searchString);
            presenter.SearchMasterEmployerListAfter();

            presenter.EmployerBroker.AssertWasCalled(x => x.GetAllEmployersWith(Arg<long>.Is.Anything, Arg<string>.Is.Equal(searchString)));
            presenter.MasterEmployerListView.AssertWasCalled(x => x.ShowEmployersWithoutSelection(Arg<IList<Employer>>.Is.Anything), y => y.Repeat.AtLeastOnce());
        }

        [Test]
        public void TestSearchForEmployerInMasterList_WithNonBlankSearchString_ShouldPerformSearch()
        {
            var newEmployers = CreateStubNewEmployerListWithFullAddresses();
            var presenter = GetInitializedPresenterWithStubDependencies(newEmployers);
            const string searchString = "TA";

            presenter.MasterEmployerListView.SearchString = searchString;
            presenter.SearchForEmployerInMasterList(searchString);

            //this is a test for the asynchronous wrapper for the search method. 
            //Therefore we are only checking if the results view was cleared 
            //before the search. The search results themselves are tested in other tests.

            presenter.MasterEmployerListView.AssertWasCalled(x => x.ShowSearchInProgressMessage());
            presenter.MasterEmployerListView.AssertWasCalled(x => x.ClearMasterListViewResults());
        }

        [Test]
        public void TestSearchForEmployerInMasterList_WithNoSearchResults_ShouldShowNoResultsMessage()
        {
            var emptyNewEmployerList = new SortedList<string, NewEmployerEntry>();
            var emptySearchResults = new SortedList<string, Employer>();
            var presenter = GetInitializedPresenterWithStubDependencies(emptyNewEmployerList, emptySearchResults);
            const string actualSearchString = "something";
            presenter.MasterEmployerListView.SearchString = actualSearchString;

            presenter.SearchMasterEmployerListBefore();
            presenter.SearchMasterEmployerListCore(actualSearchString);
            presenter.SearchMasterEmployerListAfter();

            presenter.MasterEmployerListView.AssertWasCalled(x => x.ShowSearchInProgressMessage());
            presenter.MasterEmployerListView.AssertWasCalled(x => x.ClearSearchInProgressMessage());
            presenter.MasterEmployerListView.AssertWasCalled(x => x.ShowMessageWhenSearchReturnsNoResults());
        }

        [Test]
        public void TestSearchForEmployerInMasterList_WhenSearchResultNotEmpty_ShouldShowSearchResults()
        {
            var newEmployers = CreateStubNewEmployerListWithFullAddresses();
            var presenter = GetInitializedPresenterWithStubDependencies(newEmployers);
            const string actualSearchString = "something";
            presenter.MasterEmployerListView.SearchString = actualSearchString;

            presenter.SearchMasterEmployerListBefore();
            presenter.SearchMasterEmployerListCore(actualSearchString);
            presenter.SearchMasterEmployerListAfter();

            presenter.EmployerBroker.AssertWasCalled(x => x.GetAllEmployersWith(Arg<long>.Is.Equal(presenter.Facility.Oid), Arg<string>.Is.Equal(actualSearchString)));
            presenter.MasterEmployerListView.AssertWasCalled(x => x.ShowSearchInProgressMessage());
            presenter.MasterEmployerListView.AssertWasCalled(x => x.ClearSearchInProgressMessage());
            presenter.MasterEmployerListView.AssertWasNotCalled(x => x.ShowMessageWhenSearchReturnsNoResults());
        }

        [Test]
        public void TestSelectEmployerOnMasterList_WhenEmployerDoesNotHaveAnAddress_ShouldShowNoAddressesMessage()
        {
            var employerWithoutAnAddress = new Employer(01, DateTime.Now, "SomeParty", "nationalID", 02);
            var searchResults = new SortedList<string, Employer>
                                    {
                                        { employerWithoutAnAddress.Name, employerWithoutAnAddress }
                                    };

            var newEmployers = CreateStubNewEmployerListWithFullAddresses();
            var presenter = GetInitializedPresenterWithStubDependencies(newEmployers, searchResults);

            presenter.SearchMasterEmployerListBefore();
            presenter.SearchMasterEmployerListCore("something");
            presenter.SearchMasterEmployerListAfter();

            var selectedEmployer = searchResults.Values[0];

            presenter.SelectEmployerOnMasterEmployerView(selectedEmployer);

            presenter.MasterEmployerListView.AssertWasNotCalled(x => x.ShowSelectedEmployerAddresses(Arg<IList<String>>.Is.Anything));
            presenter.MasterEmployerListView.AssertWasCalled(x => x.ShowMessageWhenEmployerDoesNotHaveAnAddress(), y => y.Repeat.AtLeastOnce());
            Assert.AreEqual(selectedEmployer, presenter.SelectedMasterListEmployer, "The presenter should set the selected employer");
        }

        [Test]
        public void TestSelectEmployerOnMasterList_WhenEmployerHasAddresses_ShouldShowAddressesOnTheView()
        {
            var newEmployers = CreateStubNewEmployerListWithFullAddresses();
            var searchResults = CreateStubExistingEmployerListWithFullAddresses();
            var presenter = GetInitializedPresenterWithStubDependencies(newEmployers, searchResults);
            const string actualSearchString = "something";
            presenter.MasterEmployerListView.SearchString = actualSearchString;

            presenter.SearchMasterEmployerListBefore();
            presenter.SearchMasterEmployerListCore(actualSearchString);
            presenter.SearchMasterEmployerListAfter();


            var selectedEmployer = searchResults.Values[0];
            var expectedContactInfo = GetFormatedContactInformation(selectedEmployer);

            presenter.SelectEmployerOnMasterEmployerView(selectedEmployer);

            presenter.MasterEmployerListView.AssertWasCalled(x => x.ShowSelectedEmployerAddresses(expectedContactInfo), y => y.Repeat.AtLeastOnce());
            Assert.AreEqual(selectedEmployer, presenter.SelectedMasterListEmployer, "The presenter should set the selected employer");
        }

        [Test]
        public void TestSelectNewEmployer_WhenNewEmployerDoesNotHaveAnAddress_ShouldDisablePhoneAndAddressFieldsButStillShowOtherInfo()
        {
            Employer employerWithoutCityInAddress = EmployerUtilities.GetEmployerWithFullAddress();
            EmployerUtilities.SetEmployerAddressCityTo(employerWithoutCityInAddress, null);

            NewEmployerEntry employerEntryWithoutAnAddress = new NewEmployerEntry(employerWithoutCityInAddress, "userID");
            var newEmployers = CreateStubNewEmployerListWithFullAddresses();
            var presenter = GetInitializedPresenterWithStubDependencies(newEmployers);
            presenter.NewEmployerListView.SelectedEmployerPhoneFieldEnabled = true;
            presenter.NewEmployerListView.SelectedEmployerAddressFieldEnabled = true;
            var employerWithAddress = newEmployers.Values[0];

            presenter.SelectEmployerOnNewEmployerView(employerWithAddress);

            presenter.SelectEmployerOnNewEmployerView(employerEntryWithoutAnAddress);
            var employerWithoutAddress = employerEntryWithoutAnAddress.Employer;

            Assert.IsFalse(presenter.NewEmployerListView.SelectedEmployerPhoneFieldEnabled, "Phone field should be disabled");
            Assert.IsFalse(presenter.NewEmployerListView.SelectedEmployerAddressFieldEnabled, "Address field should be disabled");
            Assert.AreEqual(string.Empty, presenter.NewEmployerListView.SelectedEmployerPhoneNumber, "Phone field should be cleared");
            Assert.AreEqual(string.Empty, presenter.NewEmployerListView.SelectedEmployerAddress, "Address field should be cleared");
            Assert.AreEqual(employerWithoutAddress.Name, presenter.NewEmployerListView.SelectedEmployerName, "The presenter should set the selected employer name.");
            Assert.AreEqual(employerWithoutAddress.NationalId, presenter.NewEmployerListView.SelectedEmployerNationalId, "The presenter should set the selected employer national id");
        }

        [Test]
        public void TestSelectNewEmployer_WhenSelectionIsChanged()
        {
            var newEmployers = CreateStubNewEmployerListWithFullAddresses();
            var presenter = GetInitializedPresenterWithStubDependencies(newEmployers);
            presenter.SelectEmployerOnNewEmployerView(newEmployers.Values[0]);

            var selectedEmployer = newEmployers.Values[1];
            var selectedEmployerAddress = EmployerUtilities.GetAddressOfFirstContactPointFrom(selectedEmployer);
            var selectedEmployerPhoneNumber = selectedEmployer.Employer.ContactPoints.OfType<ContactPoint>().ElementAt(0).PhoneNumber;

            presenter.SelectEmployerOnNewEmployerView(selectedEmployer);

            Assert.AreEqual(selectedEmployer.Employer.Name, presenter.NewEmployerListView.SelectedEmployerName, "The presenter should set the selected employer name.");
            Assert.AreEqual(selectedEmployerAddress.AsMailingLabel(), presenter.NewEmployerListView.SelectedEmployerAddress);
            Assert.AreEqual(selectedEmployer.Employer.NationalId, presenter.NewEmployerListView.SelectedEmployerNationalId, "The presenter should set the selected employer national id");
            Assert.AreEqual(selectedEmployerAddress.AsMailingLabel(), presenter.NewEmployerListView.SelectedEmployerAddress, "The presenter should set the selected employer address");
            Assert.AreEqual(selectedEmployerPhoneNumber.AsFormattedString(), presenter.NewEmployerListView.SelectedEmployerPhoneNumber, "The presenter should set the selected employer phone number");
            Assert.IsTrue(selectedEmployer == presenter.SelectedNewEmployerEntry, "The presenter should set the selected new employer");
        }

        [Test]
        public void TestSelectNewEmployer_WithEmployerNameShorterThanTwoChars_ShouldNotPerformSearch()
        {
            var newEmployers = CreateStubNewEmployerListWithFullAddresses();

            var presenter = GetInitializedPresenterWithStubDependencies(newEmployers);
            var selectedEmployer = newEmployers.Values[0];
            selectedEmployer.Employer.Name = "T";
            var selectedEmployerAddress = EmployerUtilities.GetAddressOfFirstContactPointFrom(selectedEmployer);
            var selectedEmployerPhoneNumber = selectedEmployer.Employer.ContactPoints.OfType<ContactPoint>().ElementAt(0).PhoneNumber;

            presenter.SelectEmployerOnNewEmployerView(selectedEmployer);

            Assert.AreEqual(selectedEmployer.Employer.Name, presenter.NewEmployerListView.SelectedEmployerName, "The presenter should set the selected employer name.");
            Assert.AreEqual(selectedEmployer.Employer.NationalId, presenter.NewEmployerListView.SelectedEmployerNationalId, "The presenter should set the selected employer national id");
            Assert.AreEqual(selectedEmployerAddress.AsMailingLabel(), presenter.NewEmployerListView.SelectedEmployerAddress, "The presenter should set the selected employer address");
            Assert.AreEqual(selectedEmployerPhoneNumber.AsFormattedString(), presenter.NewEmployerListView.SelectedEmployerPhoneNumber, "The presenter should set the selected employer phone number");
            Assert.AreEqual(selectedEmployer, presenter.SelectedNewEmployerEntry, "The presenter should set the selected new employer");
            Assert.IsTrue(String.IsNullOrEmpty(presenter.MasterEmployerListView.SearchString), "The presenter should set the search string to empty string.");

            presenter.EmployerBroker.AssertWasNotCalled(x => x.SelectEmployerByName(Arg<string>.Is.Anything, Arg<string>.Is.Anything));
            presenter.EmployerBroker.AssertWasNotCalled(x => x.GetAllEmployersWith(Arg<long>.Is.Anything, Arg<string>.Is.Anything));
            presenter.MasterEmployerListView.AssertWasNotCalled(x => x.ShowEmployersWithoutSelection(Arg<IList<Employer>>.Is.Anything));
        }


        [Test]
        public void TestSelectNewEmployer_WithSelectionChangedToEmployerWithNameShorterThanTwoChars_ShouldNotPerformSearchAndShouldClearMasterEmployerView()
        {
            var newEmployers = CreateStubNewEmployerListWithFullAddresses();
            var presenter = GetInitializedPresenterWithStubDependencies(newEmployers);
            presenter.SelectEmployerOnNewEmployerView(newEmployers.Values[0]);

            var selectedEmployer = newEmployers.Values[1];
            selectedEmployer.Employer.Name = "T";

            var selectedEmployerAddress = EmployerUtilities.GetAddressOfFirstContactPointFrom(selectedEmployer);
            var selectedEmployerPhoneNumber = selectedEmployer.Employer.ContactPoints.OfType<ContactPoint>().ElementAt(0).PhoneNumber;

            presenter.SelectEmployerOnNewEmployerView(selectedEmployer);

            Assert.AreEqual(selectedEmployer.Employer.Name, presenter.NewEmployerListView.SelectedEmployerName, "The presenter should set the selected employer name.");
            Assert.AreEqual(selectedEmployerAddress.AsMailingLabel(), presenter.NewEmployerListView.SelectedEmployerAddress);
            Assert.AreEqual(selectedEmployer.Employer.NationalId, presenter.NewEmployerListView.SelectedEmployerNationalId, "The presenter should set the selected employer national id");
            Assert.AreEqual(selectedEmployerAddress.AsMailingLabel(), presenter.NewEmployerListView.SelectedEmployerAddress, "The presenter should set the selected employer address");
            Assert.AreEqual(selectedEmployerPhoneNumber.AsFormattedString(), presenter.NewEmployerListView.SelectedEmployerPhoneNumber, "The presenter should set the selected employer phone number");
            Assert.IsTrue(selectedEmployer == presenter.SelectedNewEmployerEntry, "The presenter should set the selected new employer");

            presenter.MasterEmployerListView.AssertWasCalled(x => x.ClearMasterListViewResults(), y => y.Repeat.AtLeastOnce());

        }

        [Test]
        public void TestSelectNewEmployer_AfterMoveAllInfo_ShouldDoAutoSearch()
        {
            var newEmployers = CreateStubNewEmployerListWithFullAddresses();
            var searchResults = CreateStubExistingEmployerListWithFullAddresses();
            var presenter = GetInitializedPresenterWithStubDependencies(newEmployers, searchResults);

            presenter.SelectEmployerOnNewEmployerView(newEmployers.Values[0]);

            var selectedEmployer = newEmployers.Values[1];
            var selectedEmployerAddress = EmployerUtilities.GetAddressOfFirstContactPointFrom(selectedEmployer);
            var selectedEmployerPhoneNumber = selectedEmployer.Employer.ContactPoints.OfType<ContactPoint>().ElementAt(0).PhoneNumber;
            presenter.MoveSelectedNewEmployerToMasterList();

            presenter.SelectEmployerOnNewEmployerView(selectedEmployer);
            //the ui will call the method below as part of a selection changed event
            presenter.SelectNewEmployerAndSearchMasterEmployerList(selectedEmployer);

            Assert.AreEqual(selectedEmployer.Employer.Name, presenter.NewEmployerListView.SelectedEmployerName, "The presenter should set the selected employer name.");
            Assert.AreEqual(selectedEmployer.Employer.NationalId, presenter.NewEmployerListView.SelectedEmployerNationalId, "The presenter should set the selected employer national id");
            Assert.AreEqual(selectedEmployerAddress.AsMailingLabel(), presenter.NewEmployerListView.SelectedEmployerAddress, "The presenter should set the selected employer address");
            Assert.AreEqual(selectedEmployerPhoneNumber.AsFormattedString(), presenter.NewEmployerListView.SelectedEmployerPhoneNumber, "The presenter should set the selected employer phone number");

            string actualSearchString = selectedEmployer.Employer.Name.Substring(0, 2);
            Assert.AreEqual(actualSearchString, presenter.MasterEmployerListView.SearchString, "The presenter should set the search string to the first two characters of the selected employer's name");
            presenter.MasterEmployerListView.AssertWasCalled(x => x.ShowEmployersWithoutSelection(Arg<IList<Employer>>.Is.Anything), y => y.Repeat.AtLeastOnce());
        }

        [Test]
        public void TestSelectNewEmployer_WithEmployerNameLongerThanTwoChars_ShouldPerformSearch()
        {
            var newEmployers = CreateStubNewEmployerListWithFullAddresses();
            var presenter = GetInitializedPresenterWithStubDependencies(newEmployers);

            var selectedEmployer = newEmployers.Values[0];
            var selectedEmployerAddress = EmployerUtilities.GetAddressOfFirstContactPointFrom(selectedEmployer);
            var selectedEmployerPhoneNumber = selectedEmployer.Employer.ContactPoints.OfType<ContactPoint>().ElementAt(0).PhoneNumber;
            string actualSearchString = selectedEmployer.Employer.Name.Substring(0, 2);

            presenter.SelectNewEmployerAndSearchMasterEmployerList(selectedEmployer);

            Assert.AreEqual(selectedEmployer.Employer.Name, presenter.NewEmployerListView.SelectedEmployerName, "The presenter should set the selected employer name.");
            Assert.AreEqual(selectedEmployer.Employer.NationalId, presenter.NewEmployerListView.SelectedEmployerNationalId, "The presenter should set the selected employer national id");
            Assert.AreEqual(selectedEmployerAddress.AsMailingLabel(), presenter.NewEmployerListView.SelectedEmployerAddress, "The presenter should set the selected employer address");
            Assert.AreEqual(selectedEmployerPhoneNumber.AsFormattedString(), presenter.NewEmployerListView.SelectedEmployerPhoneNumber, "The presenter should set the selected employer phone number");

            Assert.AreEqual(actualSearchString, presenter.MasterEmployerListView.SearchString, "The presenter should set the search string to the first two characters of the selected employer's name");
//            presenter.EmployerBroker.AssertWasCalled(x => x.GetAllEmployersWith(Arg<long>.Is.Equal(presenter.Facility.Oid), Arg<string>.Is.Equal(actualSearchString)));
//            presenter.MasterEmployerListView.AssertWasCalled(x => x.ShowEmployersWithoutSelection(Arg<IList<Employer>>.Is.Anything), y => y.Repeat.AtLeastOnce());
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSelectNewEmployer_WithNull_ShouldThrowException()
        {
            var presenter = GetInitializedPresenterWithStubDependencies();
            presenter.SelectEmployerOnNewEmployerView(null);
        }

        [Test]
        public void TestUndoDelete()
        {
            var newEmployers = CreateStubNewEmployerListWithFullAddresses();
            var presenter = GetInitializedPresenterWithStubDependencies(newEmployers);

            var selectedEmployer = newEmployers.Values[1];
            presenter.SelectEmployerOnNewEmployerView(selectedEmployer);
            presenter.DeleteSelectedNewEmployer();
            ICommand lastCommandBeforeUndo = presenter.LastCommand;
            presenter.Undo();

            Assert.AreEqual(typeof(NullCommand), presenter.LastCommand.GetType(), "The last command should be set to the Null command after an undo operation.");
            Assert.IsFalse(presenter.Commands.Contains(lastCommandBeforeUndo), "The delete command should have been removed from the command collection after an undo operation");
            Assert.IsTrue(presenter.NewEmployers.Contains(selectedEmployer), "The selected employer should have been undeleted");
            Assert.AreEqual(selectedEmployer, presenter.SelectedNewEmployerEntry, "The employer that was deleted should be selected after the undoing the delete operation");
            Assert.AreEqual(selectedEmployer.Employer.Name, presenter.NewEmployerListView.SelectedEmployerName, "Selected employer name should be set to the selected employer name");
            Assert.AreEqual(selectedEmployer.Employer.NationalId, presenter.NewEmployerListView.SelectedEmployerNationalId, "Selected employer national id should be set to the selected employer national id");
        }

        [Test]
        public void TestUndoDelete_WhenThereIsOnlyOneNewEmployer()
        {
            var newEmployers = CreateStubNewEmployerListWithFullAddresses();
            RemoveAllButOneFrom(newEmployers);
            var presenter = GetInitializedPresenterWithStubDependencies(newEmployers);
            var selectedEmployer = newEmployers.Values[0];

            presenter.SelectEmployerOnNewEmployerView(selectedEmployer);
            presenter.DeleteSelectedNewEmployer();

            presenter.Undo();

            Assert.IsTrue(presenter.NewEmployers.Contains(newEmployers.Values[0]), "The selected employer should have been undeleted");
            Assert.AreEqual(selectedEmployer, presenter.SelectedNewEmployerEntry, "The employer that was deleted should be selected after the undoing the delete operation");
            Assert.AreEqual(selectedEmployer.Employer.Name, presenter.NewEmployerListView.SelectedEmployerName, "Selected employer name should be set to the selected employer name");
            Assert.AreEqual(selectedEmployer.Employer.NationalId, presenter.NewEmployerListView.SelectedEmployerNationalId, "Selected employer national id should be set to the selected employer national id");
            Assert.IsFalse(presenter.NewEmployerListView.UndoEnabled, "Undo operation should be disabled as we only support one level of undo");
        }

        [Test]
        public void TestUndoForMoveAddressAndPhoneToMasterList()
        {
            var newEmployers = CreateStubNewEmployerListWithFullAddresses();
            var existingEmployersInSearchResult = CreateStubExistingEmployerListWithFullAddresses();
            var presenter = GetInitializedPresenterWithStubDependencies(newEmployers, existingEmployersInSearchResult);

            presenter.NewEmployers = newEmployers.Values;
            presenter.MasterListEmployers = existingEmployersInSearchResult.Values;
            presenter.UpdateNewEmployersViewWithoutAnySelection();
            presenter.UpdateMasterEmployerViewWithFirstItemSelected();
            presenter.SelectEmployerOnNewEmployerView(newEmployers.Values[0]);
            presenter.SelectEmployerOnMasterEmployerView(existingEmployersInSearchResult.Values[0]);

            var selectedNewEmployer = presenter.SelectedNewEmployerEntry;
            var selectedMasterListEmployer = presenter.SelectedMasterListEmployer;
            var addressThatWasMoved = EmployerUtilities.GetAddressOfFirstContactPointFrom(selectedNewEmployer.Employer);
            int newEmployerCountBeforeUndo = presenter.NewEmployers.Count;
            presenter.LastCommand = new NullCommand();
            presenter.MoveAddressAndPhoneToMasterList();
            ICommand lastCommandBeforeUndo = presenter.LastCommand;

            presenter.Undo();

            int newEmployerCountAfterUndo = presenter.NewEmployers.Count;
            var addressesForselectedMasterListEmployer = EmployerUtilities.GetAddressesFrom(selectedMasterListEmployer);

            Assert.AreEqual(newEmployerCountAfterUndo, newEmployerCountBeforeUndo, "Both counts should be same.");
            Assert.IsFalse(addressesForselectedMasterListEmployer.Contains(addressThatWasMoved), "The address should have been removed as a result of the undo operation.");
            Assert.IsTrue(presenter.NewEmployers.Contains(selectedNewEmployer), "The employer should be back in the new employers list after the undo operation.");
            Assert.AreEqual(typeof(NullCommand), presenter.LastCommand.GetType(), "The last command should be set to the Null command after an undo operation.");
            Assert.IsFalse(presenter.Commands.Contains(lastCommandBeforeUndo), "The commands collection should not contain the command executed before the undo operation.");
            Assert.AreEqual(selectedMasterListEmployer, presenter.SelectedMasterListEmployer, "The employer for which the address was added should still be selected after the move operation");
            presenter.NewEmployerListView.AssertWasCalled(x => x.ShowEmployersWithoutSelection(presenter.NewEmployers), y => y.Repeat.AtLeastOnce());
            presenter.MasterEmployerListView.AssertWasCalled(x => x.ShowEmployersWithoutSelection(presenter.MasterListEmployers), y => y.Repeat.AtLeastOnce());
            presenter.MasterEmployerListView.AssertWasCalled(x => x.SelectEmployer(selectedMasterListEmployer), y => y.Repeat.AtLeastOnce());
        }

        [Test]
        public void TestUndoForMoveNewEmployerToMasterList()
        {
            var newEmployers = CreateStubNewEmployerListWithFullAddresses();
            var searResults = CreateStubExistingEmployerListWithFullAddresses();
            var presenter = GetInitializedPresenterWithStubDependencies(newEmployers, searResults);
            presenter.MasterListEmployers = CreateStubExistingEmployerListWithFullAddresses().Values;
            presenter.UpdateMasterEmployerViewWithFirstItemSelected();

            presenter.UpdateNewEmployersViewWithoutAnySelection();
            presenter.SelectEmployerOnNewEmployerView(newEmployers.Values[0]);

            var employerEntryToMove = newEmployers.Values[1];
            presenter.SelectEmployerOnNewEmployerView(employerEntryToMove);
            presenter.MoveSelectedNewEmployerToMasterList();

            var lastCommandBeforeUndo = presenter.LastCommand;

            presenter.Undo();

            presenter.MasterEmployerListView.AssertWasCalled(x => x.ShowEmployersWithoutSelection(Arg<IList<Employer>>.Is.Anything), y => y.Repeat.AtLeastOnce());

            Assert.IsTrue(presenter.NewEmployers.Contains(employerEntryToMove), "The new employer collection should once again contain the employer that was moved before the undo operation");
            Assert.AreEqual(presenter.SelectedNewEmployerEntry, employerEntryToMove, "The employer that was moved back as a result of the undo operation should be selected in the new employers view");

            Assert.AreEqual(typeof(NullCommand), presenter.LastCommand.GetType(), "The last command should be set to the Null command after an undo operation.");
            Assert.IsFalse(presenter.Commands.Contains(lastCommandBeforeUndo), "The commands collection should not contain the command executed before the undo operation");
        }

        [Test]
        public void TestUpdateMasterEmployerView_WithFirstItemSelected()
        {
            var presenter = GetInitializedPresenterWithStubDependencies();
            presenter.MasterListEmployers = CreateStubExistingEmployerListWithFullAddresses().Values;
            presenter.UpdateMasterEmployerViewWithFirstItemSelected();
            Assert.AreEqual(presenter.MasterListEmployers[0], presenter.SelectedMasterListEmployer, "The first employer on the master list should be selected after an update on the master list view");
            presenter.MasterEmployerListView.AssertWasCalled(x => x.ShowEmployersWithoutSelection(presenter.MasterListEmployers));
            presenter.MasterEmployerListView.AssertWasCalled(x => x.SelectEmployer(presenter.MasterListEmployers[0]));
        }

        [Test]
        public void TestUpdateMasterEmployerView_WithEmptyMasterEmployerList()
        {
            var presenter = GetInitializedPresenterWithStubDependencies();
            presenter.MasterListEmployers = new List<Employer>();
            presenter.UpdateMasterEmployerViewWithFirstItemSelected();

            Assert.AreEqual(null, presenter.SelectedMasterListEmployer, "Nothing should be selected if the master list is empty.");
            presenter.MasterEmployerListView.AssertWasNotCalled(x => x.SelectEmployer(Arg<Employer>.Is.Anything));
        }

        [Test]
        public void TestUpdateNewEmployersViewWithoutAnySelection()
        {
            var newEmployers = CreateStubNewEmployerListWithFullAddresses();
            var presenter = GetInitializedPresenterWithStubDependencies(newEmployers);

            presenter.UpdateNewEmployersViewWithoutAnySelection();
            var newEmployerView = presenter.NewEmployerListView;
            Assert.IsNull(presenter.SelectedNewEmployerEntry, "No employer should be selected after the view is updated");
            presenter.NewEmployerListView.AssertWasCalled(x => x.ShowEmployersWithoutSelection(newEmployers.Values), y => y.Repeat.AtLeastOnce());
            Assert.IsFalse(newEmployerView.EditAddressEnabled);
            Assert.IsFalse(newEmployerView.ClearEmployerContactInformationEnabled);
            Assert.IsFalse(newEmployerView.MoveAllInfoEnabled);
            Assert.IsFalse(newEmployerView.MoveAddressAndPhoneEnabled);
            Assert.IsFalse(newEmployerView.DeleteEnabled);
            Assert.IsFalse(newEmployerView.UndoEnabled);
        }

        [Test]
        public void TestUpdateNewEmployersView_WhenNoNewEmployersAreAvailable()
        {
            var emptyNewEmployerList = new SortedList<string, NewEmployerEntry>();
            //using a mock (vs a stub) here as Rhino mocks stub does not record property get and set operations
            var mockNewEmployerListView = MockRepository.GenerateMock<INewEmployersListView>();

            var mockMasterEmployerListView = MockRepository.GenerateStub<IMasterEmployersListView>();
            var mockEmployerBroker = MockRepository.GenerateStub<IEmployerBroker>();

            mockEmployerBroker
                .Stub(x => x.GetAllEmployersForApproval(Arg<string>.Is.Anything))
                .Return(emptyNewEmployerList);

            var user = GetStubUser();

            var presenter = new NewEmployersManagementPresenter(user, mockNewEmployerListView, mockMasterEmployerListView, mockEmployerBroker);

            presenter.InitializeSynchronous();

            presenter.UpdateNewEmployersViewWithoutAnySelection();
            var newEmployerView = presenter.NewEmployerListView;

            newEmployerView.AssertWasCalled(x => x.EditAddressEnabled = Arg<bool>.Is.Equal(false), y => y.Repeat.AtLeastOnce());
            newEmployerView.AssertWasCalled(x => x.ClearEmployerContactInformationEnabled = Arg<bool>.Is.Equal(false), y => y.Repeat.AtLeastOnce());
            newEmployerView.AssertWasCalled(x => x.MoveAllInfoEnabled = Arg<bool>.Is.Equal(false), y => y.Repeat.AtLeastOnce());
            newEmployerView.AssertWasCalled(x => x.MoveAddressAndPhoneEnabled = Arg<bool>.Is.Equal(false), y => y.Repeat.AtLeastOnce());
            newEmployerView.AssertWasCalled(x => x.DeleteEnabled = Arg<bool>.Is.Equal(false), y => y.Repeat.AtLeastOnce());
            newEmployerView.AssertWasCalled(x => x.UndoEnabled = Arg<bool>.Is.Equal(false), y => y.Repeat.AtLeastOnce());
            newEmployerView.AssertWasCalled(x => x.ShowNoNewUsersAvailableMessage(), y => y.Repeat.AtLeastOnce());
        }

        [Test]
        public void TestUpdateNewEmployersView_WithoutAnySelection()
        {
            var presenter = GetInitializedPresenterWithStubDependencies();
            presenter.NewEmployers = CreateStubNewEmployerListWithFullAddresses().Values;


            presenter.NewEmployerListView.SelectedEmployerNameFieldEnabled = true;
            presenter.NewEmployerListView.SelectedEmployerNationalIdFieldEnabled = true;
            presenter.NewEmployerListView.SelectedEmployerPhoneFieldEnabled = true;
            presenter.NewEmployerListView.SelectedEmployerAddressFieldEnabled = true;


            presenter.UpdateNewEmployersViewWithoutAnySelection();
            Assert.IsNull(presenter.SelectedNewEmployerEntry, "Nothing should be selected after an update on the new employer view.");
            Assert.IsFalse(presenter.NewEmployerListView.SelectedEmployerNameFieldEnabled, "The name field should be disabled");
            Assert.IsFalse(presenter.NewEmployerListView.SelectedEmployerNationalIdFieldEnabled, "The national id field should be disabled");
            Assert.IsFalse(presenter.NewEmployerListView.SelectedEmployerPhoneFieldEnabled, "The phone field should be disabled");
            Assert.IsFalse(presenter.NewEmployerListView.SelectedEmployerAddressFieldEnabled, "The address field should be disabled");

        }

        [Test]
        public void TestUpdateNewEmployersViewWithoutAnySelectionAndWithEmptyNewEmployerList()
        {
            var presenter = GetInitializedPresenterWithStubDependencies();
            presenter.NewEmployers = new List<NewEmployerEntry>();
            presenter.UpdateNewEmployersViewWithoutAnySelection();
            Assert.IsNull(presenter.SelectedNewEmployerEntry, "Nothing should be selected after an update on the new employer view.");
        }


        [Test]
        public void TestSearchMasterListOperation_WhenSearchStringIsTwoCharactersLong_OperationShouldBeEnabled()
        {
            var presenter = GetInitializedPresenterWithStubDependencies();
            presenter.MasterEmployerListView.SearchString = "ab";
            presenter.UpdateEnableStatusOfOperationsAndViewFields();
            Assert.IsTrue(presenter.MasterEmployerListView.IsSearchButtonEnabled, "The search operation should be enabled");
        }

        [Test]
        public void TestSearchMasterListOperation_WhenSearchStringIsNull_OperationShouldBeDisabled()
        {
            var presenter = GetInitializedPresenterWithStubDependencies();
            presenter.MasterEmployerListView.SearchString = null;
            presenter.UpdateEnableStatusOfOperationsAndViewFields();
            Assert.IsFalse(presenter.MasterEmployerListView.IsSearchButtonEnabled, "The search operation should be disabled");
        }


        [Test]
        public void TestSearchMasterListOperation_WhenSearchStringIsEmpty_OperationShouldBeDisabled()
        {
            var presenter = GetInitializedPresenterWithStubDependencies();
            presenter.MasterEmployerListView.SearchString = string.Empty;
            presenter.UpdateEnableStatusOfOperationsAndViewFields();
            Assert.IsFalse(presenter.MasterEmployerListView.IsSearchButtonEnabled, "The search operation should be disabled");
        }


        [Test]
        public void TestSearchMasterListOperation_WhenSearchStringIsSmallerThanTwoCharacters_OperationShouldBeDisabled()
        {
            var presenter = GetInitializedPresenterWithStubDependencies();
            presenter.MasterEmployerListView.SearchString = "a";
            presenter.UpdateEnableStatusOfOperationsAndViewFields();
            Assert.IsFalse(presenter.MasterEmployerListView.IsSearchButtonEnabled, "The search operation should be disabled");
        }


        [Test]
        public void TestSearchMasterListOperation_WhenAnSearchStringIsChangedFromInvalidToValid_OperationShouldBeEnabled()
        {
            var presenter = GetInitializedPresenterWithStubDependencies();
            presenter.MasterEmployerListView.SearchString = "a";
            presenter.UpdateEnableStatusOfOperationsAndViewFields();
            presenter.MasterEmployerListView.SearchString = "ab";
            presenter.UpdateEnableStatusOfOperationsAndViewFields();
            Assert.IsTrue(presenter.MasterEmployerListView.IsSearchButtonEnabled, "The search operation should be enabled");
        }

        [Test]
        public void TestSearchMasterListOperation_WhenAnSearchStringIsChangedFromValidToInvalid_OperationShouldBeDisabled()
        {
            var presenter = GetInitializedPresenterWithStubDependencies();
            presenter.MasterEmployerListView.SearchString = "ab";
            presenter.UpdateEnableStatusOfOperationsAndViewFields();
            presenter.MasterEmployerListView.SearchString = string.Empty;
            presenter.UpdateEnableStatusOfOperationsAndViewFields();
            Assert.IsFalse(presenter.MasterEmployerListView.IsSearchButtonEnabled, "The search operation should be disabled");
        }


        [Test]
        public void TestSelectedEmployerNameChanged()
        {
            var presenter = GetInitializedPresenterWithStubDependencies();
            presenter.SelectEmployerOnNewEmployerView(presenter.NewEmployers[0]);
            string expectedName = Guid.NewGuid().ToString();
            presenter.NewEmployerListView.SelectedEmployerName = expectedName;

            presenter.SelectedEmployerNameChanged();

            string actualName = presenter.SelectedNewEmployerEntry.Employer.Name;

            Assert.AreEqual(expectedName, actualName, "The presenter should have updated the selected new employer's name");
        }


        [Test]
        public void TestSelectedEmployerNationalIdChanged()
        {
            var presenter = GetInitializedPresenterWithStubDependencies();
            presenter.SelectEmployerOnNewEmployerView(presenter.NewEmployers[0]);
            string expectedNationalId = Guid.NewGuid().ToString();
            presenter.NewEmployerListView.SelectedEmployerNationalId = expectedNationalId;

            presenter.SelectedEmployerNationalIdChanged();

            string actualNationalId = presenter.SelectedNewEmployerEntry.Employer.NationalId;

            Assert.AreEqual(expectedNationalId, actualNationalId, "The presenter should have updated the selected new employer's national Id");
        }

        [Test]
        public void TestSelectedEmployerPhoneChanged()
        {
            var presenter = GetInitializedPresenterWithStubDependencies();
            var employerEntry = presenter.NewEmployers[0];
            EmployerUtilities.SetAllPhoneNumbersTo(employerEntry.Employer, "1111111111");

            presenter.SelectEmployerOnNewEmployerView(presenter.NewEmployers[0]);

            const string expectedPhoneNumber = "3333333333";
            presenter.NewEmployerListView.SelectedEmployerPhoneNumber = expectedPhoneNumber;

            presenter.SelectedEmployerPhoneChanged();

            string actualPhoneNumber = EmployerUtilities.GetFirstContactPointFrom(presenter.SelectedNewEmployerEntry.Employer).PhoneNumber.AsUnformattedString();

            Assert.AreEqual(expectedPhoneNumber, actualPhoneNumber, "The presenter should have updated the selected new employer's phone number");
        }





        //[TestMethod]
        /// <summary>
        /// A test used to interact with the actual WinForms view implementation during development.
        /// </summary>
        public void TempTest()
        {
            //            var newEmployers = new SortedList<string, NewEmployerEntry>();
            var newEmployers = CreateStubNewEmployerListWithFullAddresses();
            var searchResultsExistingEmployers = CreateStubExistingEmployerListWithFullAddresses();
            //var searchResultsExistingEmployers = new SortedList<string,Employer>();


            var employerWithoutAnAddress = new Employer(01, DateTime.Now, "Employer without an address", "nationalID", 02);

            var employerWithoutAnAddressEntry = new NewEmployerEntry(employerWithoutAnAddress, "user 4");
            newEmployers.Add(employerWithoutAnAddress.Name, employerWithoutAnAddressEntry);

            var mockEmployerBroker = MockRepository.GenerateStub<IEmployerBroker>();

            mockEmployerBroker.Stub(x => x.GetAllEmployersForApproval(Arg<string>.Is.Anything)).Return(newEmployers);

            mockEmployerBroker.Stub(x => x.GetAllEmployersWith(Arg<long>.Is.Anything, Arg<string>.Is.Anything)).Return(searchResultsExistingEmployers);

            var employersManagementView = new NewEmployersManagementView();
            var user = GetStubUser();

            var presenter = new NewEmployersManagementPresenter(user, employersManagementView, employersManagementView, mockEmployerBroker);

            presenter.InitializeSynchronous();
            presenter.UpdateNewEmployersViewWithoutAnySelection();

            var form = new Form
                           {
                               AutoSize = true,
                               AutoSizeMode = AutoSizeMode.GrowAndShrink
                           };

            form.Controls.Add(employersManagementView);
            form.ShowDialog();
        }
        #endregion Tests

        #region Helper Methods

        private static SortedList<string, Employer> CreateStubExistingEmployerListWithFullAddresses()
        {

            var employer1 = new Employer(01, DateTime.Now, "Perot Systems (Existing Employer)", "ABC123XYZ", 1);

            employer1.AddContactPoint(
                new ContactPoint(
                    new Address(
                        "2300 W Plano Parkway",
                        "Suite 1",
                        "Plano",
                        new ZipCode("75075"),
                        new State(),
                        Country.NewUnitedStatesCountry(),
                        new County("1")),
                    new PhoneNumber("123", "1234567"),
                    new EmailAddress("someone@ps.net"),
                    TypeOfContactPoint.NewEmployerContactPointType()));

            employer1.AddContactPoint(
                new ContactPoint(
                    new Address(
                        "Another Address",
                        "Suite 1",
                        "Plano",
                        new ZipCode("75075"),
                        new State(),
                        Country.NewUnitedStatesCountry(),
                        new County("1")),
                    new PhoneNumber("123", "1234567"),
                    new EmailAddress("someone@ps.net"),
                    TypeOfContactPoint.NewEmployerContactPointType()));


            var employer2 = new Employer(02, DateTime.Now, "Microsoft Corporation (Existing Employer)", "XYZ123ABC", 1);

            employer2.AddContactPoint(
                new ContactPoint(
                    new Address(
                        "1 Microsoft Way",
                        "Suite 2",
                        "Redmond",
                        new ZipCode("12345"),
                        new State(),
                        Country.NewUnitedStatesCountry(),
                        new County("2")),
                    new PhoneNumber("456", "7894561"),
                    new EmailAddress("someone@microsoft.com"),
                    TypeOfContactPoint.NewEmployerContactPointType()));


            employer2.AddContactPoint(
                new ContactPoint(
                    new Address(
                        "Another Address",
                        "Suite 2",
                        "Redmond",
                        new ZipCode("12345"),
                        new State(),
                        Country.NewUnitedStatesCountry(),
                        new County("2")),
                    new PhoneNumber("456", "7894561"),
                    new EmailAddress("someone@microsoft.com"),
                    TypeOfContactPoint.NewEmployerContactPointType()));



            var employer3 = new Employer(03, DateTime.Now, "Another Company (Existing Employer)", "123XYZABC", 1);

            employer3.AddContactPoint(
                new ContactPoint(
                    new Address(
                        "1000 W Hill Road",
                        "Suite 3",
                        "Addison",
                        new ZipCode("75001"),
                        new State(),
                        Country.NewUnitedStatesCountry(),
                        new County("1")),
                    new PhoneNumber("123", "1234567"),
                    new EmailAddress("someone@somecompany.net"),
                    TypeOfContactPoint.NewEmployerContactPointType()));


            employer3.AddContactPoint(
                new ContactPoint(
                    new Address(
                        "Another Address",
                        "Suite 3",
                        "Addison",
                        new ZipCode("75001"),
                        new State(),
                        Country.NewUnitedStatesCountry(),
                        new County("1")),
                    new PhoneNumber("123", "1234567"),
                    new EmailAddress("someone@somecompany.net"),
                    TypeOfContactPoint.NewEmployerContactPointType()));

            employer3.AddContactPoint(
                new ContactPoint(
                    new Address(
                        "Yet Another Address",
                        "Suite 3",
                        "Addison",
                        new ZipCode("75001"),
                        new State(),
                        Country.NewUnitedStatesCountry(),
                        new County("1")),
                    new PhoneNumber("123", "1234567"),
                    new EmailAddress("someone@somecompany.net"),
                    TypeOfContactPoint.NewEmployerContactPointType()));

            var employerWithoutAnAddress = new Employer(01, DateTime.Now, "Employer without an address", "nationalID", 02);

            return new SortedList<string, Employer>
                       {
                           { employer1.Name, employer1 }, 
                           { employer2.Name, employer2 }, 
                           { employer3.Name, employer3 },
                           {employerWithoutAnAddress.Name,employerWithoutAnAddress}
                       };
        }

        private static SortedList<string, NewEmployerEntry> CreateStubNewEmployerListWithFullAddresses()
        {
            var employer1 = new Employer(01, DateTime.Now, "Perot Systems", "ABC123XYZ", 1);

            employer1.AddContactPoint(
                new ContactPoint(
                    new Address(
                        "2300 W Plano Parkway (New Address)",
                        "Suite 1",
                        "Plano",
                        new ZipCode("75075"),
                        new State(),
                        Country.NewUnitedStatesCountry(),
                        new County("1")),
                    new PhoneNumber("123", "1234567"),
                    new EmailAddress("someone@ps.net"),
                    TypeOfContactPoint.NewEmployerContactPointType()));

            employer1.AddContactPoint(
                new ContactPoint(
                    new Address(
                        "Another Address (New Address)",
                        "Suite 1",
                        "Plano",
                        new ZipCode("75075"),
                        new State(),
                        Country.NewUnitedStatesCountry(),
                        new County("1")),
                    new PhoneNumber("123", "1234567"),
                    new EmailAddress("someone@ps.net"),
                    TypeOfContactPoint.NewEmployerContactPointType()));

            var employerEntry1 = new NewEmployerEntry(employer1, "user 1");

            var employer2 = new Employer(02, DateTime.Now, "Microsoft Corporation", "XYZ123ABC", 1);

            employer2.AddContactPoint(
                new ContactPoint(
                    new Address(
                        "1 Microsoft Way (New Address)",
                        "Suite 2",
                        "Redmond",
                        new ZipCode("12345"),
                        new State(),
                        Country.NewUnitedStatesCountry(),
                        new County("2")),
                    new PhoneNumber("456", "7894561"),
                    new EmailAddress("someone@microsoft.com"),
                    TypeOfContactPoint.NewEmployerContactPointType()));


            employer2.AddContactPoint(
                new ContactPoint(
                    new Address(
                        "Another Address (New Address)",
                        "Suite 2",
                        "Redmond",
                        new ZipCode("12345"),
                        new State(),
                        Country.NewUnitedStatesCountry(),
                        new County("2")),
                    new PhoneNumber("456", "7894561"),
                    new EmailAddress("someone@microsoft.com"),
                    TypeOfContactPoint.NewEmployerContactPointType()));


            var employerEntry2 = new NewEmployerEntry(employer2, "user 2");

            var employer3 = new Employer(03, DateTime.Now, "Another Company", "123XYZABC", 1);

            employer3.AddContactPoint(
                new ContactPoint(
                    new Address(
                        "1000 W Hill Road (New Address)",
                        "Suite 3",
                        "Addison",
                        new ZipCode("75001"),
                        new State(),
                        Country.NewUnitedStatesCountry(),
                        new County("1")),
                    new PhoneNumber("123", "1234567"),
                    new EmailAddress("someone@somecompany.net"),
                    TypeOfContactPoint.NewEmployerContactPointType()));


            employer3.AddContactPoint(
                new ContactPoint(
                    new Address(
                        "Another Address (New Address)",
                        "Suite 3",
                        "Addison",
                        new ZipCode("75001"),
                        new State(),
                        Country.NewUnitedStatesCountry(),
                        new County("1")),
                    new PhoneNumber("123", "1234567"),
                    new EmailAddress("someone@somecompany.net"),
                    TypeOfContactPoint.NewEmployerContactPointType()));

            employer3.AddContactPoint(
                new ContactPoint(
                    new Address(
                        "Yet Another Address (New Address)",
                        "Suite 3",
                        "Addison",
                        new ZipCode("75001"),
                        new State(),
                        Country.NewUnitedStatesCountry(),
                        new County("1")),
                    new PhoneNumber("123", "1234567"),
                    new EmailAddress("someone@somecompany.net"),
                    TypeOfContactPoint.NewEmployerContactPointType()));

            var employerEntry3 = new NewEmployerEntry(employer3, "user 3");

            return new SortedList<string, NewEmployerEntry> 
                       { 
                           { employer1.Name, employerEntry1 }, 
                           { employer2.Name, employerEntry2 }, 
                           { employer3.Name, employerEntry3 },
                       };
        }

        private static List<string> GetFormatedContactInformation(Party employer)
        {
            List<string> addresses = new List<string>();
            foreach (ContactPoint contactPoint in employer.ContactPoints)
            {
                string fomatedAddressAndPhoneNumber = contactPoint.Address.AsMailingLabel() + Environment.NewLine
                                                      + contactPoint.PhoneNumber.AsFormattedString();
                addresses.Add(fomatedAddressAndPhoneNumber);
            }
            return addresses;
        }

        private static NewEmployersManagementPresenter GetInitializedPresenterWithStubDependencies()
        {
            SortedList<string, NewEmployerEntry> employers = CreateStubNewEmployerListWithFullAddresses();
            var existingEmployers = CreateStubExistingEmployerListWithFullAddresses();
            return GetInitializedPresenterWithStubDependencies(employers, existingEmployers);
        }

        private static NewEmployersManagementPresenter GetInitializedPresenterWithStubDependencies(
            SortedList<string, NewEmployerEntry> employers,
            SortedList<string, Employer> employerSearchResults)
        {
            NewEmployersManagementPresenter presenter = GetPresenterWithStubDependencies(employers, employerSearchResults);
            presenter.InitializeSynchronous();
            return presenter;
        }

        private static NewEmployersManagementPresenter GetInitializedPresenterWithStubDependencies(SortedList<string, NewEmployerEntry> newEmployers)
        {
            return GetInitializedPresenterWithStubDependencies(newEmployers, CreateStubExistingEmployerListWithFullAddresses());
        }

        private static NewEmployersManagementPresenter GetPresenterWithStubDependencies(SortedList<string, NewEmployerEntry> employers, SortedList<string, Employer> employerSearchResults)
        {
            var mockNewEmployerListView = MockRepository.GenerateStub<INewEmployersListView>();
            var mockMasterEmployerListView = MockRepository.GenerateStub<IMasterEmployersListView>();
            var mockEmployerBroker = MockRepository.GenerateStub<IEmployerBroker>();

            mockEmployerBroker
                .Stub(x => x.GetAllEmployersForApproval(Arg<string>.Is.Anything))
                .Return(employers);

            mockEmployerBroker
                .Stub(x => x.GetAllEmployersWith(Arg<long>.Is.Anything, Arg<string>.Is.Anything))
                .Return(employerSearchResults);

            var user = GetStubUser();
            var presenter = new NewEmployersManagementPresenter(user, mockNewEmployerListView, mockMasterEmployerListView, mockEmployerBroker);

            var lockBroker = MockRepository.GenerateMock<IOfflineLockBroker>();
            lockBroker.Stub( x => x.AddLockEntry( Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<ResourceType>.Is.Equal( ResourceType.FacilityForNewEmployerManagementFeature ) ) ).Return( true );

            return presenter;
        }

        private static void RemoveAllButOneFrom(SortedList<string, NewEmployerEntry> newEmployers)
        {
            for (int i = 0; i < newEmployers.Count; i++)
            {
                newEmployers.RemoveAt(i);
            }
        }

        private static User GetStubUser()
        {
            Facility facility = GetStubFacility();
            User user = User.NewInstance();
            user.SecurityUser = new Extensions.SecurityService.Domain.User(1234, "userFirstName", "userLastName");
            user.SecurityUser.UPN = "UserUPN";
            user.UserID = "SomeUserID";
            user.Facility = facility;
            return user;
        }

        private static Facility GetStubFacility()
        {
            return new Facility(01, DateTime.Now, "SomeFacility", "hospital code")
                       {
                           FollowupUnit = new FollowupUnit(02, DateTime.Now, "description")
                       };
        }

        #endregion Helper Methods
    }
}