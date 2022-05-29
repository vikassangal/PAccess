using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Utilities;

namespace PatientAccess.UI.NewEmployersManagement
{
    /// <summary>
    /// Manages the new employer management related views
    /// </summary>
    internal sealed class NewEmployersManagementPresenter : INewEmployersManagementPresenter, IDisposable
    {
        private const string HardcodedUserIdForMasterEmployerEntry = "PACCESS";

        #region Fields

        private IList<ICommand> _commands;
        private IEmployerBroker _employerBroker;
        private Facility _facility;
        private FollowupUnit _followupUnit;
        private ICommand _lastCommand;
        private IList<Employer> _masterEmployerList;
        private IMasterEmployersListView _masterEmployerListView;
        private INewEmployersListView _newEmployerListView;
        private List<NewEmployerEntry> _newEmployers;
        private Employer _selectedMasterListEmployer;
        private NewEmployerEntry _selectedNewEmployer;
        private BackgroundWorker _masterEmployerBackgroundSearcher;
        private bool _isSearching;
        private BackgroundWorker _newEmployerBackgroundRetreiver;
        private bool _isNewEmployerListBeingRetreived;
        private bool _isSaving;
        private BackgroundWorker _saveBackgroundWorker;

        #endregion Fields

        #region Constructors

        internal NewEmployersManagementPresenter(User user,
                                                  INewEmployersListView newEmployerListView,
                                                  IMasterEmployersListView masterEmployerListView,
                                                  IEmployerBroker employerBroker)
        {

            Guard.ThrowIfArgumentIsNull(newEmployerListView, "newEmployerListView");
            Guard.ThrowIfArgumentIsNull(masterEmployerListView, "masterEmployerListView");
            Guard.ThrowIfArgumentIsNull(employerBroker, "employerBroker");
            Guard.ThrowIfArgumentIsNull(user, "user");

            this.Facility = user.Facility;
            this.FollowupUnit = this.Facility.FollowupUnit;
            this.EmployerBroker = employerBroker;
            this.Commands = new List<ICommand>();
            this.LastCommand = new NullCommand();
            this.NewEmployerListView = newEmployerListView;
            this.NewEmployers = new List<NewEmployerEntry>();


            this.NewEmployerListView.Presenter = this;

            this.NewEmployerBackgroundRetreiver = new BackgroundWorker();
            this.NewEmployerBackgroundRetreiver.WorkerSupportsCancellation = true;
            this.NewEmployerBackgroundRetreiver.DoWork += DoRetrevalOfNewEmployers;
            this.NewEmployerBackgroundRetreiver.RunWorkerCompleted += NewEmployerRetreivalCompleted;

            this.MasterListEmployers = new List<Employer>();
            this.MasterEmployerListView = masterEmployerListView;
            this.MasterEmployerBackgroundSearcher = new BackgroundWorker();
            this.MasterEmployerBackgroundSearcher.WorkerSupportsCancellation = true;
            this.MasterEmployerBackgroundSearcher.DoWork += this.DoSearchForMasterEmployerList;
            this.MasterEmployerBackgroundSearcher.RunWorkerCompleted += this.MasterEmployerListSearchCompleted;

            this.SaveBackgroundWorker = new BackgroundWorker();
            this.SaveBackgroundWorker.WorkerSupportsCancellation = true;
            this.SaveBackgroundWorker.DoWork += this.DoSave;
            this.SaveBackgroundWorker.RunWorkerCompleted += SaveCompleted;
        }

        #endregion Constructors

        #region Properties

        internal IList<ICommand> Commands
        {
            get
            {
                return this._commands;
            }
            private set
            {
                this._commands = value;
            }
        }

        internal FollowupUnit FollowupUnit
        {
            get
            {
                return this._followupUnit;
            }
            private set
            {
                this._followupUnit = value;
            }
        }

        internal ICommand LastCommand
        {
            get
            {
                return this._lastCommand;
            }
            set
            {
                this._lastCommand = value;
            }
        }

        internal IList<NewEmployerEntry> NewEmployers
        {
            get
            {
                return this._newEmployers;
            }
            //TODO-AC this was done because removing an element from a list that is part of a sorted list throws a notSupported exception. 
            //Look for a more appropriate solution to this this
            set
            {
                this._newEmployers = new List<NewEmployerEntry>(value);
            }
        }

        internal Employer SelectedMasterListEmployer
        {
            get
            {
                return this._selectedMasterListEmployer;
            }
            private set
            {
                this._selectedMasterListEmployer = value;
            }
        }

        internal IEmployerBroker EmployerBroker
        {
            get
            {
                return this._employerBroker;
            }
            private set
            {
                this._employerBroker = value;
            }
        }

        internal IList<Employer> MasterListEmployers
        {
            get
            {
                return this._masterEmployerList;
            }
            set
            {
                //TODO-AC this was done because removing an element from a list that is part of a sorted list throws a notSupported exception. 
                //Look for a more appropriate solution to this this
                this._masterEmployerList = new List<Employer>(value);
            }
        }

        private BackgroundWorker SaveBackgroundWorker
        {
            get
            {
                return this._saveBackgroundWorker;
            }
            set
            {
                this._saveBackgroundWorker = value;
            }
        }

        private BackgroundWorker NewEmployerBackgroundRetreiver
        {
            get
            {
                return this._newEmployerBackgroundRetreiver;
            }
            set
            {
                this._newEmployerBackgroundRetreiver = value;
            }
        }

        private bool IsSearchingMasterEmployerList
        {
            get
            {
                return this._isSearching;
            }
            set
            {
                this._isSearching = value;
            }
        }

        private bool IsNewEmployerListBeingRetreived
        {
            get
            {
                return this._isNewEmployerListBeingRetreived;
            }
            set
            {
                this._isNewEmployerListBeingRetreived = value;
            }
        }

        public Facility Facility
        {
            get
            {
                return this._facility;
            }
            private set
            {
                this._facility = value;
            }
        }

        private BackgroundWorker MasterEmployerBackgroundSearcher
        {
            get
            {
                return this._masterEmployerBackgroundSearcher;
            }
            set
            {
                this._masterEmployerBackgroundSearcher = value;
            }
        }

        private bool IsSaving
        {
            get
            {
                return this._isSaving;
            }
            set
            {
                this._isSaving = value;
            }
        }

        #endregion Properties

        #region INewEmployersManagementPresenter Members

        public NewEmployerEntry SelectedNewEmployerEntry
        {
            get
            {
                return this._selectedNewEmployer;
            }

            set
            {
                this._selectedNewEmployer = value;
            }
        }

        public INewEmployersListView NewEmployerListView
        {
            get
            {
                return this._newEmployerListView;
            }
            set
            {
                this._newEmployerListView = value;
            }
        }

        public IMasterEmployersListView MasterEmployerListView
        {
            get
            {
                return this._masterEmployerListView;
            }
            set
            {
                this._masterEmployerListView = value;
            }
        }


        public void SelectNewEmployerAndSearchMasterEmployerList(NewEmployerEntry employerEntry)
        {
            Guard.ThrowIfArgumentIsNull(employerEntry, "employerEntry");
            this.MasterEmployerListView.ClearMasterListViewResults();
            this.SelectNewEmployer(employerEntry);


            string employerName = employerEntry.Employer.Name;

            if (IsSearchStringValid(employerName))
            {
                string searchString = employerName.Substring(0, 2);
                this.MasterEmployerListView.SearchString = searchString;
                this.SearchForEmployerInMasterList(searchString);
            }
        }


        public void UpdateNewEmployersViewWithoutAnySelection()
        {
            this.NewEmployerListView.ShowEmployersWithoutSelection(this.NewEmployers);

            this.SelectedNewEmployerEntry = null;
            this.ClearNewEmployerDetailFields();
            this.NewEmployerListView.DisplayMoveAllInfoMessageWithNoEmployerSelection();

            if (this.IsNewEmployerListEmpty())
            {
                this.NewEmployerListView.ShowNoNewUsersAvailableMessage();
            }
            this.UpdateEnableStatusOfOperationsAndViewFields();
        }

        public void SearchForEmployerInMasterList(string searchText)
        {
            Guard.ThrowIfArgumentIsNullOrEmpty(searchText, "searchText");

            if (!this.MasterEmployerBackgroundSearcher.IsBusy)
            {
                this.SearchMasterEmployerListBefore();
                this.MasterEmployerBackgroundSearcher.RunWorkerAsync(searchText);
            }
        }


        public bool IsPhoneNumberValid(string phoneNumber)
        {

            // Phone Number regex
            //
            //\(?\b[0-9]{3}\)?[-. ]?[0-9]{3}[-. ]?[0-9]{4}\b
            //
            //Options: case insensitive; free-spacing
            //
            //Match the character “(” literally «\(?»
            //   Between zero and one times, as many times as possible, giving back as needed (greedy) «?»
            //Assert position at a word boundary «\b»
            //Match a single character in the range between “0” and “9” «[0-9]{3}»
            //   Exactly 3 times «{3}»
            //Match the character “)” literally «\)?»
            //   Between zero and one times, as many times as possible, giving back as needed (greedy) «?»
            //Match a single character present in the list “-. ” «[-. ]?»
            //   Between zero and one times, as many times as possible, giving back as needed (greedy) «?»
            //Match a single character in the range between “0” and “9” «[0-9]{3}»
            //   Exactly 3 times «{3}»
            //Match a single character present in the list “-. ” «[-. ]?»
            //   Between zero and one times, as many times as possible, giving back as needed (greedy) «?»
            //Match a single character in the range between “0” and “9” «[0-9]{4}»
            //   Exactly 4 times «{4}»
            //Assert position at a word boundary «\b»
            //
            //
            //Created with RegexBuddy


            return Regex.IsMatch(phoneNumber, @"\A\(?\b[0-9]{3}\)?[-. ]?[0-9]{3}[-. ]?[0-9]{4}\b\Z",
                                 RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
        }

        public void DeleteSelectedNewEmployer()
        {
            DeleteNewEmployerCommand deleteNewEmployerCommand =
                new DeleteNewEmployerCommand(this, this.SelectedNewEmployerEntry);

            this.Commands.Add(deleteNewEmployerCommand);
            this.LastCommand = deleteNewEmployerCommand;
            deleteNewEmployerCommand.ExecuteUIAction();
        }


        public void AddNewEmployer(NewEmployerEntry employer)
        {
            this.NewEmployers.Add(employer);
            this.UpdateNewEmployersViewWithoutAnySelection();
        }


        public void ClearAddressAndPhoneNumber()
        {
            this.NewEmployerListView.SelectedEmployerAddress = String.Empty;
            this.NewEmployerListView.SelectedEmployerPhoneNumber = String.Empty;
            ClearAddressAndPhoneNumberFor(this.SelectedNewEmployerEntry.Employer);
            this.UpdateSelectedNewEmployerOnTheView();
        }


        public void SelectEmployerOnMasterEmployerView(Employer employer)
        {
            Guard.ThrowIfArgumentIsNull(employer, "employer");

            this.SelectedMasterListEmployer = employer;
            this.MasterEmployerListView.SelectEmployer(employer);

            if (EmployerHelper.EmployerHasOneOrMoreAddresses(employer))
            {
                List<string> addresses = this.GetFormatedContactInformation(employer);

                this.MasterEmployerListView.ShowSelectedEmployerAddresses(addresses);
            }

            else
            {
                this.MasterEmployerListView.ShowMessageWhenEmployerDoesNotHaveAnAddress();
            }

            this.UpdateEnableStatusOfOperationsAndViewFields();
        }


        public void MoveAddressAndPhoneToMasterList()
        {
            MoveAddressAndPhoneToMasterListCommand moveAddressAndPhoneCommand =
                new MoveAddressAndPhoneToMasterListCommand(this, this.SelectedNewEmployerEntry,
                                                            this.SelectedMasterListEmployer);

            this.LastCommand = moveAddressAndPhoneCommand;
            this.Commands.Add(moveAddressAndPhoneCommand);
            moveAddressAndPhoneCommand.ExecuteUIAction();
        }


        public void MoveSelectedNewEmployerToMasterList()
        {
            MoveEmployerToMasterListCommand employerToMasterListCommand = new MoveEmployerToMasterListCommand(this,
                                                                                                               this.SelectedNewEmployerEntry);

            this.LastCommand = employerToMasterListCommand;
            this.Commands.Add(employerToMasterListCommand);
            employerToMasterListCommand.ExecuteUIAction();
        }


        public void ShowFeatureIsLockedMessage()
        {
            this.NewEmployerListView.FinishEnabled = false;
            this.NewEmployerListView.ShowFeatureIsLockedMessage();
        }


        public void SearchStringChanged()
        {
            this.UpdateEnableStatusOfOperationsAndViewFields();
        }


        public bool IsEmployerNameValid(string employerName)
        {
            return Regex.IsMatch(employerName, @"^[A-Z][ A-Z0-9-]{0,24}$",
                                 RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
        }


        public bool IsSearchStringValid(string searchString)
        {
            if (string.IsNullOrEmpty(searchString))
            {
                return false;
            }

            //TODO-AC check if we need to do any additional checking on the input e.g for malicious input 

            searchString = searchString.Trim();


            //search string validation regular expression
            //
            //^[a-zA-Z][\w\s]{1,24}$
            //
            //Options: case insensitive; free-spacing
            //
            //Assert position at the beginning of the string «^»
            //Match a single character present in the list below «[a-zA-Z]»
            //   A character in the range between “a” and “z” «a-z»
            //   A character in the range between “A” and “Z” «A-Z»
            //Match a single character present in the list below «[\w\s]{1,24}»
            //   Between one and 24 times, as many times as possible, giving back as needed (greedy) «{1,24}»
            //   A word character (letters, digits, etc.) «\w»
            //   A whitespace character (spaces, tabs, line breaks, etc.) «\s»
            //Assert position at the end of the string (or before the line break at the end of the string, if any) «$»

            return Regex.IsMatch(searchString, @"^[a-zA-Z][\w\s\&\.\~\!\@\#\$\%\^\&\*\(\)\-\+\:\”\|\}\{\<\>\?]{1,24}$",
                                 RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

        }


        public void SelectedEmployerNameChanged()
        {
            if (this.IsANewEmployerSelected())
            {
                this.SelectedNewEmployerEntry.Employer.Name = this.NewEmployerListView.SelectedEmployerName;
                this.UpdateSelectedNewEmployerOnTheView();
            }
        }


        public void SelectedEmployerNationalIdChanged()
        {
            if (this.IsANewEmployerSelected())
            {
                this.SelectedNewEmployerEntry.Employer.NationalId = this.NewEmployerListView.SelectedEmployerNationalId;
                this.UpdateSelectedNewEmployerOnTheView();
            }
        }


        public void SelectedEmployerPhoneChanged()
        {
            if (this.IsANewEmployerSelected())
            {
                ContactPoint contactPoint = EmployerHelper.GetFirstContactPointFor(this.SelectedNewEmployerEntry.Employer);
                PhoneNumber newPhoneNumber = new PhoneNumber(this.NewEmployerListView.SelectedEmployerPhoneNumber);
                contactPoint.PhoneNumber = newPhoneNumber;
                this.UpdateSelectedNewEmployerOnTheView();
            }
        }


        public void Save()
        {
            this.SaveBefore();
            this.SaveBackgroundWorker.RunWorkerAsync();
        }

        public void Undo()
        {
            this.RemoveLastCommand();
            this.UndoLastUIAction();
            this.ClearLastCommand();
            this.UpdateEnableStatusOfOperationsAndViewFields();
        }


        public void UpdateSelectedNewEmployerOnTheView()
        {
            this.NewEmployerListView.UpdateEmployer(this.SelectedNewEmployerEntry);
            this.UpdateEnableStatusOfOperationsAndViewFields();
        }


        public string GetFormattedEmployerInfo(NewEmployerEntry employer)
        {
            string address = EmployerHelper.GetFirstAddressFor(employer);

            string employerInfo = employer.Employer.Name + Environment.NewLine + address + Environment.NewLine +
                                  employer.UserID;

            return employerInfo;
        }

        public void ChangeSelectedNewEmployerAddress(Address newlySelectedAddress)
        {
            ContactPoint currentlySelectedNewEmployer = EmployerHelper.GetFirstContactPointFor(
                this.SelectedNewEmployerEntry.Employer);
            currentlySelectedNewEmployer.Address = newlySelectedAddress;
            this.SelectNewEmployer(this.SelectedNewEmployerEntry);
            this.UpdateSelectedNewEmployerOnTheView();
        }

        #endregion

        #region Methods

        private List<string> GetFormatedContactInformation(Employer employer)
        {
            List<string> addresses = new List<string>();
            foreach (ContactPoint contactPoint in employer.ContactPoints)
            {
                if (EmployerHelper.AddressHasCityAndStreet(contactPoint.Address))
                {
                    string fomatedAddressAndPhoneNumber = contactPoint.Address.AsMailingLabel() +
                                                          Environment.NewLine +
                                                          contactPoint.PhoneNumber.AsFormattedString();

                    addresses.Add(fomatedAddressAndPhoneNumber);
                }
            }
            return addresses;
        }


        private void SelectNewEmployer(NewEmployerEntry employerEntry)
        {
            this.SelectedNewEmployerEntry = employerEntry;
            this.ShowNewEmployerDetails(employerEntry);
            this.UpdateEnableStatusOfOperationsAndViewFields();
        }


        internal void SelectEmployerOnNewEmployerView(NewEmployerEntry employerEntry)
        {
            Guard.ThrowIfArgumentIsNull(employerEntry, "employerEntry");
            this.MasterEmployerListView.ClearMasterListViewResults();
            this.SelectNewEmployer(employerEntry);

            this.NewEmployerListView.SelectEmployer(employerEntry);
        }

        internal void SaveSynchronous()
        {
            this.SaveBefore();
            this.SaveCore();
            this.SaveAfter();
        }


        private void DoSave(object sender, DoWorkEventArgs e)
        {
            this.SaveCore();
        }


        private void SaveBefore()
        {
            this.IsSaving = true;
            this.NewEmployerListView.ShowSavingMessage();
            this.UpdateEnableStatusOfOperationsAndViewFields();
        }


        private void SaveCore()
        {
            this.ExecuteCommands();
            this.ClearCommands();
            this.ClearLastCommand();
            this.SaveNewEmployers();
        }


        private void SaveNewEmployers()
        {
            foreach (NewEmployerEntry employerEntry in NewEmployers)
            {
                this.SetEmployerPartyContactPoint(employerEntry);
            }

            this.EmployerBroker.DeleteAllEmployersForApproval(this.Facility.Code);
            this.EmployerBroker.SaveEmployersForApproval(this.NewEmployers, this.Facility.Code);
        }


        private void SetEmployerPartyContactPoint(NewEmployerEntry employerEntry)
        {
            ContactPoint contactPoint = EmployerHelper.GetFirstContactPointFor(employerEntry.Employer);
            employerEntry.Employer.RemoveContactPoint(contactPoint);
            employerEntry.Employer.PartyContactPoint.Address = contactPoint.Address;
            employerEntry.Employer.PartyContactPoint.PhoneNumber = contactPoint.PhoneNumber;
        }


        private void SaveAfter()
        {
            this.IsSaving = false;
            this.UpdateEnableStatusOfOperationsAndViewFields();
            this.NewEmployerListView.ClearSavingMessage();
        }


        private void SaveCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            if (e.Error != null)
            {
                throw e.Error;
            }

            this.SaveAfter();
        }


        internal void Initialize()
        {
            if (!this.NewEmployerBackgroundRetreiver.IsBusy)
            {
                this.InitializeBefore();
                this.NewEmployerBackgroundRetreiver.RunWorkerAsync();
            }
        }


        /// <summary>
        /// Initializes the presenter synchronously. This intended to be used for testing purposes only
        /// This is the synchronous version of the <see cref="Initialize"/> method.
        /// </summary>
        internal void InitializeSynchronous()
        {
            this.InitializeBefore();
            this.InitializeCore();
            this.InitializeAfter();
        }

        private void InitializeBefore()
        {
            this.IsNewEmployerListBeingRetreived = true;
            this.NewEmployerListView.ShowRetrievingNewEmployersMessage();
            this.UpdateEnableStatusOfOperationsAndViewFields();
        }

        private void InitializeCore()
        {
            this.GetEmployersForApproval();
        }


        private void InitializeAfter()
        {
            this.IsNewEmployerListBeingRetreived = false;

            this.NewEmployerListView.ClearRetrievingNewEmployersMessage();
            this.UpdateNewEmployersViewWithoutAnySelection();
            this.UpdateEnableStatusOfOperationsAndViewFields();
        }


        internal void SaveEmployerToMasterList(NewEmployerEntry employerEntry)
        {
            int empCode = this.EmployerBroker.AddNewEmployer(employerEntry.Employer, this.FollowupUnit, this.Facility.Code, HardcodedUserIdForMasterEmployerEntry);
            employerEntry.Employer.EmployerCode = empCode;
            ContactPoint contactPoint = EmployerHelper.GetFirstContactPointFor(employerEntry.Employer);
            this.EmployerBroker.AddContactPointForEmployer(employerEntry.Employer, contactPoint, this.Facility.Code);
        }


        internal void SaveContactPointForMasterListEmployer(Employer employer, ContactPoint contactPoint)
        {
            this.EmployerBroker.AddContactPointForEmployer(employer, contactPoint, this.Facility.Code);
        }


        internal void DeleteNewEmployer(Employer employer)
        {
            this.EmployerBroker.DeleteEmployerForApproval(employer, this.Facility.Code);
        }


        private void DoSearchForMasterEmployerList(object sender, DoWorkEventArgs e)
        {
            string searchString = (string)e.Argument;
            this.SearchMasterEmployerListCore(searchString);
        }


        internal void SearchMasterEmployerListBefore()
        {
            this.MasterEmployerListView.ShowSearchInProgressMessage();
            this.MasterEmployerListView.ClearMasterListViewResults();
            this.IsSearchingMasterEmployerList = true;
            this.UpdateEnableStatusOfOperationsAndViewFields();
        }


        internal void SearchMasterEmployerListCore(string searchString)
        {
            Guard.ThrowIfArgumentIsNullOrEmpty(searchString, "searchString");

            //TODO-AC do validation on the search string before using it

            SortedList<string, Employer> employers = this.EmployerBroker.GetAllEmployersWith(this._facility.Oid, searchString);
            this.MasterListEmployers = employers.Values;
        }


        /// <exception cref="Exception"><c>Exception</c>.</exception>
        private void MasterEmployerListSearchCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                throw e.Error;
            }

            this.SearchMasterEmployerListAfter();
        }


        internal void SearchMasterEmployerListAfter()
        {
            this.IsSearchingMasterEmployerList = false;
            this.MasterEmployerListView.ClearSearchInProgressMessage();
            this.UpdateMasterEmployerViewWithFirstItemSelected();
            this.UpdateEnableStatusOfOperationsAndViewFields();
        }


        internal void ClearNewEmployerDetailFields()
        {
            this.NewEmployerListView.SelectedEmployerName = String.Empty;
            this.NewEmployerListView.SelectedEmployerAddress = String.Empty;
            this.NewEmployerListView.SelectedEmployerNationalId = String.Empty;
            this.NewEmployerListView.SelectedEmployerPhoneNumber = String.Empty;
        }


        internal void UpdateMasterEmployerViewWithFirstItemSelected()
        {
            if (this.IsMasterListEmpty())
            {
                this.SelectedMasterListEmployer = null;
                this.MasterEmployerListView.ShowMessageWhenSearchReturnsNoResults();
            }
            else
            {
                UpdateMasterEmployerViewWithoutSelection();
                this.SelectEmployerOnMasterEmployerView(this.MasterListEmployers[0]);
            }
        }


        internal void UpdateMasterEmployerViewWithoutSelection()
        {
            this.MasterEmployerListView.ShowEmployersWithoutSelection(this.MasterListEmployers);
        }


        private void GetEmployersForApproval()
        {
            this.NewEmployers =
                new List<NewEmployerEntry>(this.EmployerBroker.GetAllEmployersForApproval(this._facility.Code).Values);
        }

        private bool IsNewEmployerListEmpty()
        {
            return this.NewEmployers.Count == 0;
        }


        private bool IsMasterListEmpty()
        {
            return this.MasterListEmployers.Count == 0;
        }


        private void ShowNewEmployerDetails(NewEmployerEntry employerEntry)
        {
            this.NewEmployerListView.SelectedEmployerName = employerEntry.Employer.Name;
            this.NewEmployerListView.SelectedEmployerNationalId = employerEntry.Employer.NationalId;
            if (EmployerHelper.EmployerAddressHasStreetAndCity(employerEntry.Employer))
            {
                ContactPoint employerContactPoint = EmployerHelper.GetFirstContactPointFor(employerEntry.Employer);
                this.NewEmployerListView.SelectedEmployerAddress = employerContactPoint.Address.AsMailingLabel();
                this.NewEmployerListView.SelectedEmployerPhoneNumber =
                    employerContactPoint.PhoneNumber.AsFormattedString();

                this.NewEmployerListView.DisplayMoveAllInfoMessageWithEmployerSelection();
            }
            else
            {
                this.NewEmployerListView.SelectedEmployerPhoneNumber = String.Empty;
                this.NewEmployerListView.SelectedEmployerAddress = string.Empty;
                this.NewEmployerListView.DisplayMoveAllInfoMessageWithNoEmployerSelection();
            }
        }


        private static void ClearAddressAndPhoneNumberFor(Employer employer)
        {
            ContactPoint contactPoint = EmployerHelper.GetFirstContactPointFor(employer);
            contactPoint.PhoneNumber = new PhoneNumber();
            contactPoint.Address = new Address();
        }


        internal void UpdateEnableStatusOfOperationsAndViewFields()
        {
            this.NewEmployerListView.MoveAddressAndPhoneEnabled =

                this.SelectedNewEmployerEntry != null &&
                this.SelectedMasterListEmployer != null &&
                EmployerHelper.EmployerAddressHasStreetAndCity(this.SelectedNewEmployerEntry.Employer) &&
                !this.IsSearchingMasterEmployerList &&
                !this.IsNewEmployerListBeingRetreived &&
                !this.IsSaving;

            this.NewEmployerListView.MoveAllInfoEnabled =
                this.SelectedNewEmployerEntry != null &&
                !this.IsSearchingMasterEmployerList &&
                !this.IsNewEmployerListBeingRetreived &&
                !this.IsSaving;

            this.NewEmployerListView.ClearEmployerContactInformationEnabled =
                this.SelectedNewEmployerEntry != null &&
                !this.IsSearchingMasterEmployerList &&
                !this.IsNewEmployerListBeingRetreived &&
                !this.IsSaving;

            this.NewEmployerListView.DeleteEnabled =
                this.SelectedNewEmployerEntry != null &&
                !this.IsSearchingMasterEmployerList &&
                !this.IsNewEmployerListBeingRetreived &&
                !this.IsSaving;

            this.NewEmployerListView.UndoEnabled =
                this.LastCommand.GetType() != typeof(NullCommand) &&
                !this.IsSearchingMasterEmployerList &&
                !this.IsNewEmployerListBeingRetreived &&
                !this.IsSaving;

            this.NewEmployerListView.EditAddressEnabled =
                this.SelectedNewEmployerEntry != null &&
                !this.IsSearchingMasterEmployerList &&
                !this.IsNewEmployerListBeingRetreived &&
                !this.IsSaving;

            this.NewEmployerListView.FinishEnabled =

                !this.IsSearchingMasterEmployerList &&
                !this.IsNewEmployerListBeingRetreived &&
                !this.IsSaving;

            this.NewEmployerListView.CancelEnabled = !this.IsSaving;

            this.NewEmployerListView.SelectedEmployerNameFieldEnabled =
                this.SelectedNewEmployerEntry != null &&
                !this.IsSearchingMasterEmployerList &&
                !this.IsNewEmployerListBeingRetreived &&
                !this.IsSaving;

            this.NewEmployerListView.SelectedEmployerNationalIdFieldEnabled =
                this.SelectedNewEmployerEntry != null &&
                !this.IsSearchingMasterEmployerList &&
                !this.IsNewEmployerListBeingRetreived &&
                !this.IsSaving;

            this.NewEmployerListView.SelectedEmployerPhoneFieldEnabled =
                this.SelectedNewEmployerEntry != null &&
                !this.IsSearchingMasterEmployerList &&
                !this.IsNewEmployerListBeingRetreived &&
                !this.IsSaving &&
                EmployerHelper.EmployerAddressHasStreetAndCity(this.SelectedNewEmployerEntry.Employer);

            this.NewEmployerListView.SelectedEmployerAddressFieldEnabled =
                this.SelectedNewEmployerEntry != null &&
                !this.IsSearchingMasterEmployerList &&
                !this.IsNewEmployerListBeingRetreived &&
                !this.IsSaving &&
                EmployerHelper.EmployerAddressHasStreetAndCity(this.SelectedNewEmployerEntry.Employer);



            this.MasterEmployerListView.IsSearchButtonEnabled = !this.IsSearchingMasterEmployerList
                && IsSearchStringValid(this.MasterEmployerListView.SearchString);

        }

        private void ClearLastCommand()
        {
            this.LastCommand = new NullCommand();
        }


        private void ClearCommands()
        {
            this.Commands.Clear();
        }

        private void ExecuteCommands()
        {
            foreach (ICommand command in this._commands)
            {
                command.CommitChanges();
            }
        }

        private void UndoLastUIAction()
        {
            this.LastCommand.UndoUIAction();
        }


        private void NewEmployerRetreivalCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                throw e.Error;
            }

            this.InitializeAfter();
        }


        private void DoRetrevalOfNewEmployers(object sender, DoWorkEventArgs e)
        {
            this.InitializeCore();
        }


        private void RemoveLastCommand()
        {
            this.Commands.Remove(this.LastCommand);
        }

        private bool IsANewEmployerSelected()
        {
            return this.SelectedNewEmployerEntry != null;
        }

        #endregion Methods

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.DisposeBackgroundWorkers();
            }

        }

        private void DisposeBackgroundWorkers()
        {
            this.NewEmployerBackgroundRetreiver.Dispose();
            this.MasterEmployerBackgroundSearcher.Dispose();
            this.SaveBackgroundWorker.Dispose();
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
