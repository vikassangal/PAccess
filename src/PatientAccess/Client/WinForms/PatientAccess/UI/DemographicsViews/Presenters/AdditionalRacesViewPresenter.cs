using System;
using PatientAccess.Domain;
using PatientAccess.Utilities;
using System.Collections;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Rules;
using PatientAccess.UI.DemographicsViews.Views;

namespace PatientAccess.UI.DemographicsViews.Presenters
{
    public class AdditionalRacesViewPresenter : IAdditionalRacesViewPresenter
    {
        #region Variables and Properties
        
        private readonly IAdditionalRacesView AdditionalRacesView;
        private readonly IAdditionalRacesFeatureManager AdditionalRacesFeatureManager;
        public readonly Account ModelAccount;
        public ICollection RaceCollection { get; set; }
        public ArrayList RaceArrayList { get; set; }

        #endregion

        #region Constructors
        public AdditionalRacesViewPresenter(IAdditionalRacesView raceView, Account account, IAdditionalRacesFeatureManager additionalRacesFeatureManager)
        {
            Guard.ThrowIfArgumentIsNull(raceView, "RaceView"); 
            Guard.ThrowIfArgumentIsNull(account, "account");
            Guard.ThrowIfArgumentIsNull(additionalRacesFeatureManager, "AdditionalRacesFeatureManager");

            AdditionalRacesView = raceView;
            ModelAccount = account;
            raceView.AdditionalRacesViewPresenter = this;
            AdditionalRacesFeatureManager = additionalRacesFeatureManager;
        }
        
        #endregion

        #region Public Methods

        public void UpdateView()
        {
            if (AdditionalRacesFeatureManager.IsAdditionalRacesFeatureValidForAccount(ModelAccount))
            {
                PopulateRace();
                UpdateRaceToView();
                EnableRace4ComboBox();
                EnableRace5ComboBox();
                RunInvalidCodeRules();
            }
         }
        public bool ShouldAdditionRaceEditButtonBeVisible(Account account)
        {
            return AdditionalRacesFeatureManager.IsAdditionalRacesFeatureValidForAccount(account);
        }
        public bool ShouldAdditionRaceEditButtonBeEnabled(Account account)
        {
           return (IsValidRace && IsValidRace2);
        }
        public void UpdateRace3ToModel()
        {
            ModelAccount.Patient.Race3 = AdditionalRacesView.Race3;
        }
        public void UpdateRace4ToModel()
        {
            ModelAccount.Patient.Race4 = AdditionalRacesView.Race4;
        }
        public void UpdateRace5ToModel()
        {
            ModelAccount.Patient.Race5 = AdditionalRacesView.Race5;
        }
        public void ShowAdditionalRacesView()
        {
            AdditionalRacesView.ShowAdditionalRacesView();
        }

        public void RunInvalidCodeRules()
        {
            RuleEngine.GetInstance()
                .OneShotRuleEvaluation<InvalidRace3Code>(ModelAccount, InvalidRace3CodeEventHandler);
            RuleEngine.GetInstance()
                .OneShotRuleEvaluation<InvalidRace3CodeChange>(ModelAccount, InvalidRace3CodeChangeEventHandler);

            RuleEngine.GetInstance()
                .OneShotRuleEvaluation<InvalidRace4Code>(ModelAccount, InvalidRace4CodeEventHandler);
            RuleEngine.GetInstance()
                .OneShotRuleEvaluation<InvalidRace4CodeChange>(ModelAccount, InvalidRace4CodeChangeEventHandler);

            RuleEngine.GetInstance()
                .OneShotRuleEvaluation<InvalidRace5Code>(ModelAccount, InvalidRace5CodeEventHandler);

            RuleEngine.GetInstance()
                .OneShotRuleEvaluation<InvalidRace5CodeChange>(ModelAccount, InvalidRace5CodeChangeEventHandler);
        }

        #endregion

        #region Private Methods and Properties

        private void InvalidRace3CodeEventHandler(object sender, EventArgs e)
        {
            AdditionalRacesView.SetRace3DeactivatedBgColor();
        }
        private void InvalidRace3CodeChangeEventHandler(object sender, EventArgs e)
        {
            AdditionalRacesView.ProcessRace3InvalidCodeEvent();
        }
        private void InvalidRace4CodeEventHandler(object sender, EventArgs e)
        {
            AdditionalRacesView.SetRace4DeactivatedBgColor();
        }
      
        private void InvalidRace4CodeChangeEventHandler(object sender, EventArgs e)
        {
            AdditionalRacesView.ProcessRace4InvalidCodeEvent();
        }
        private void InvalidRace5CodeEventHandler(object sender, EventArgs e)
        {
            AdditionalRacesView.SetRace5DeactivatedBgColor();
        }
       
        private void InvalidRace5CodeChangeEventHandler(object sender, EventArgs e)
        {
            AdditionalRacesView.ProcessRace5InvalidCodeEvent();
        }
        
        private void PopulateRace()
        {
            // Call Origin PBARbroker to get all races for the facility 
            var originBroker = BrokerFactory.BrokerOfType<IOriginBroker>();
            RaceCollection = originBroker.LoadRaces(User.GetCurrent().Facility.Oid);
            RaceArrayList = BuildRaceArrayLists();
            AdditionalRacesView.ModelAccount = ModelAccount;
            AdditionalRacesView.PopulateRace(RaceArrayList);
        }
        
        private ArrayList BuildRaceArrayLists()
        {
          RaceArrayList = new ArrayList();
            RaceArrayList.Add(BlankRace);
            foreach (Race race in RaceCollection)
            {
                if (!RaceArrayList.Contains(race))
                {
                    RaceArrayList.Add(race);
                }
              }
            return RaceArrayList;
        }
        private Race AccountRace
        {
            get { return ModelAccount.Patient.Race; }
        }
        private Race AccountRace2
        {
            get { return ModelAccount.Patient.Race2; }
        }
        private Race AccountRace3
        {
            get { return ModelAccount.Patient.Race3; }
        }
        private Race AccountRace4
        {
            get { return ModelAccount.Patient.Race4; }
        }
        private Race AccountRace5
        {
            get { return ModelAccount.Patient.Race5; }
        }
        private void UpdateRaceToView()
        {
            if (IsPatientHasRace3)
            {
                AdditionalRacesView.Race3 = AccountRace3;
            }
            if (IsPatientHasRace4)
            {
                AdditionalRacesView.Race4 = AccountRace4;
            }
            if (IsPatientHasRace5)
            {
                AdditionalRacesView.Race5 = AccountRace5;
            }
        }
        private bool IsPatientHasRace3
        {
            get
            {
                return (AccountRace3 != null && !String.IsNullOrEmpty(AccountRace3.Code));
            }
        }
        private bool IsPatientHasRace4
        {
            get
            {
                return (AccountRace4 != null && !String.IsNullOrEmpty(AccountRace4.Code));
            }
        }
        private bool IsPatientHasRace5
        {
            get
            {
                return (AccountRace5 != null && !String.IsNullOrEmpty(AccountRace5.Code));
            }
        }
        private void EnableRace4ComboBox()
        {
            if (IsValidRace3)
            {
                AdditionalRacesView.EnableRace4ComboBox();
            }
            else
            {
                AdditionalRacesView.DisableRace4ComboBox();
            }
        }
        private void EnableRace5ComboBox()
        {
            if (IsValidRace4)
            {
                AdditionalRacesView.EnableRace5ComboBox();
            }
            else
            {
                AdditionalRacesView.DisableRace5ComboBox();
            }
        }
        private bool IsValidRace
        {
            get
            {
                return AccountRace!=null &&
                       !(AccountRace.Description.Trim() == Race.RACE_DECLINED ||
                             AccountRace.Description.Trim() == Race.RACE_UNKNOWN ||
                             AccountRace.Description.Trim() == Race.RACE_OTHER ||
                             AccountRace.Description.Trim() == String.Empty);
            }
        }
        private bool IsValidRace2
        {
            get
            {
                return AccountRace2 != null &&
                       !(AccountRace2.Description.Trim() == Race.RACE_DECLINED ||
                         AccountRace2.Description.Trim() == Race.RACE_UNKNOWN ||
                         AccountRace2.Description.Trim() == Race.RACE_OTHER ||
                         AccountRace2.Description.Trim() == String.Empty);
            }
        }

        public bool IsValidRace3
        {
            get
            {
                return AdditionalRacesView.Race3 != null &&
                       !(AdditionalRacesView.Race3.Description.Trim() == Race.RACE_DECLINED ||
                         AdditionalRacesView.Race3.Description.Trim() == Race.RACE_UNKNOWN ||
                         AdditionalRacesView.Race3.Description.Trim() == Race.RACE_OTHER ||
                         AdditionalRacesView.Race3.Description.Trim() == String.Empty);
            }
        }

        public bool IsValidRace4
        {
            get
            {
                return AdditionalRacesView.Race4 != null &&
                       !(AdditionalRacesView.Race4.Description.Trim() == Race.RACE_DECLINED ||
                         AdditionalRacesView.Race4.Description.Trim() == Race.RACE_UNKNOWN ||
                         AdditionalRacesView.Race4.Description.Trim() == Race.RACE_OTHER ||
                         AdditionalRacesView.Race4.Description.Trim() == String.Empty);
            }
        }

        #endregion 
       
        #region Contants
        Race BlankRace = new Race(-1, ReferenceValue.NEW_VERSION, " ", Race.ZERO_CODE);
        #endregion
        
    }
}
