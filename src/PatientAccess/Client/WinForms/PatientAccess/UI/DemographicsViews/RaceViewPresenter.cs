using System;
using PatientAccess.Domain;
using PatientAccess.Utilities;
using System.Collections;
using System.Collections.Generic;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Rules;


namespace PatientAccess.UI.DemographicsViews
{
    public class RaceViewPresenter : IRaceViewPresenter
    {
        #region Variables and Properties
        
        private readonly IRaceView RaceView;
        public readonly Account ModelAccount;
        private readonly string RACENATIONALITY_CONTROL;
        public ICollection NationalityCollection { get; set; }
        public ICollection RaceCollection { get; set; }
        public IDictionary<Race, ArrayList> RaceNationalityDictionary { get; set; }

        #endregion

        #region Constructors
        public RaceViewPresenter(IRaceView raceView, Account account, string raceNationalityControl)
        {
            Guard.ThrowIfArgumentIsNull(raceView, "RaceView");
            Guard.ThrowIfArgumentIsNull(account, "account");
           
            RaceView = raceView;
            ModelAccount = account;
            RaceView.RaceViewPresenter = this;
            RACENATIONALITY_CONTROL = raceNationalityControl;
        }
        #endregion

        #region Public Methods

        public void UpdateView()
        {
            RaceView.ModelAccount = ModelAccount;
            PopulateRace();
            UpdateRaceToView();
            RunInvalidCodeRules();
            RunRules();
        }

        public void RunInvalidCodeRules()
        {
            if (RACENATIONALITY_CONTROL.Equals(Race.RACENATIONALITY_CONTROL))
            {
                RuleEngine.GetInstance()
                    .OneShotRuleEvaluation<InvalidRaceCode>(ModelAccount, InvalidRaceCodeEventHandler);
                RuleEngine.GetInstance()
                    .OneShotRuleEvaluation<InvalidRaceCodeChange>(ModelAccount, InvalidRaceCodeChangeEventHandler);
            }
            else
            {
                RuleEngine.GetInstance()
                    .OneShotRuleEvaluation<InvalidRace2Code>(ModelAccount, InvalidRaceCodeEventHandler);
                RuleEngine.GetInstance()
                    .OneShotRuleEvaluation<InvalidRace2CodeChange>(ModelAccount, InvalidRaceCodeChangeEventHandler);
            }
        }

        public void RunRules()
        {
            RaceView.SetNormalBgColor();
            if (RACENATIONALITY_CONTROL.Equals(Race.RACENATIONALITY_CONTROL))
            {
                RuleEngine.GetInstance().OneShotRuleEvaluation<RacePreferred>(ModelAccount, RacePreferredEventHandler);
                RuleEngine.GetInstance().OneShotRuleEvaluation<RaceRequired>(ModelAccount, RaceRequiredEventHandler);
            }
        }

        public void UpdateRaceAndNationalityModelValue(Race race)
        {
            if (RACENATIONALITY_CONTROL.Equals(Race.RACENATIONALITY_CONTROL))
            {
                if (!string.IsNullOrEmpty(race.ParentRaceCode))
                {
                    ModelAccount.Patient.Nationality = race;
                    Race parentRace = GetKeyRaceFromDictionary(race.ParentRaceCode);
                    if (parentRace != null)
                        ModelAccount.Patient.Race = parentRace;
                }
                else
                {
                    ModelAccount.Patient.Race = race;
                    ModelAccount.Patient.Nationality = new Race();
                }
            }
            if (RACENATIONALITY_CONTROL.Equals(Race.RACENATIONALITY2_CONTROL))
            {
                if (!string.IsNullOrEmpty(race.ParentRaceCode))
                {
                    ModelAccount.Patient.Nationality2 = race;
                    Race parentRace = GetKeyRaceFromDictionary(race.ParentRaceCode);
                    if (parentRace != null)
                        ModelAccount.Patient.Race2 = parentRace;
                }
                else
                {
                    ModelAccount.Patient.Race2 = race;
                    ModelAccount.Patient.Nationality2 = new Race();
                }
            }
        }

        #endregion

        #region Private Methods and Properties

        private void InvalidRaceCodeChangeEventHandler(object sender, EventArgs e)
        {
           RaceView.ProcessInvalidCodeEvent();
        }

        private void InvalidRaceCodeEventHandler(object sender, EventArgs e)
        {
            RaceView.SetDeactivatedBgColor();
        }
        
        private void RaceRequiredEventHandler(object sender, EventArgs e)
        {
           RaceView.SetRaceAsRequiredColor();
        }

        private void RacePreferredEventHandler(object sender, EventArgs e)
        {
            RaceView.SetRaceAsPreferredColor();
        }
        
        private void PopulateRace()
        {
            // Call Origin PBARbroker to get all races for the facility 
            var originBroker = BrokerFactory.BrokerOfType<IOriginBroker>();
            RaceCollection = originBroker.LoadRaces(User.GetCurrent().Facility.Oid);
            NationalityCollection = originBroker.LoadNationalities(User.GetCurrent().Facility.Oid);
            RaceNationalityDictionary = BuildRaceNationalityDictionary();
            BuildNationality();
            RaceView.PopulateRace(RaceNationalityDictionary);
        }
        
        public IDictionary<Race, ArrayList> BuildRaceNationalityDictionary()
        {
            RaceNationalityDictionary = new Dictionary<Race, ArrayList>();
            RaceNationalityDictionary.Add(BlankRace, new ArrayList());
            foreach (Race race in RaceCollection)
            {
                if (!RaceNationalityDictionary.ContainsKey(race))
                {
                    var raceList = new ArrayList();
                    RaceNationalityDictionary.Add(race, raceList);
                }
              }
            return RaceNationalityDictionary;
        }
       
        public void BuildNationality()
        {
            foreach (Race nationality in NationalityCollection)
            {
                var parentRaceCode = nationality.ParentRaceCode;
                var parentRace = GetKeyRaceFromDictionary(parentRaceCode);
                if (parentRace != null)
                {
                    if (RaceNationalityDictionary.ContainsKey(parentRace))
                    {
                        if (!RaceNationalityDictionary[parentRace].Contains(nationality))
                        {
                            RaceNationalityDictionary[parentRace].Add(nationality);
                        }
                    }
                    else
                    {
                        ArrayList raceList = null;
                        raceList = new ArrayList();
                        RaceNationalityDictionary.Add(parentRace, raceList);
                        if (!RaceNationalityDictionary[parentRace].Contains(nationality))
                        {
                            RaceNationalityDictionary[parentRace].Add(nationality);
                        }
                    }
                }
            }
        }
        
        private Race AccountRace
        {
            get
            {
                if (RACENATIONALITY_CONTROL.Equals(Race.RACENATIONALITY_CONTROL))
                {
                    return ModelAccount.Patient.Race;
                }
                else if (RACENATIONALITY_CONTROL.Equals(Race.RACENATIONALITY2_CONTROL))
                {
                    return ModelAccount.Patient.Race2;
                }
                else return new Race();
            }
        }

        private Race AccountNationality
        {
            get
            {
                if (RACENATIONALITY_CONTROL.Equals(Race.RACENATIONALITY_CONTROL))
                {
                    return ModelAccount.Patient.Nationality;
                }
                else if (RACENATIONALITY_CONTROL.Equals(Race.RACENATIONALITY2_CONTROL))
                {
                    return ModelAccount.Patient.Nationality2;
                }
                else return new Race();
            }
        }
        private void UpdateRaceToView()
        {
            if (IsPatientHasRace &&
                IsPatientHasNationality)
            {
                RaceView.Race = AccountNationality;
            }
            if (IsPatientHasRace &&
                !IsPatientHasNationality)
            {
                RaceView.Race = AccountRace;
            }
        }

        private bool IsPatientHasNationality
        {
            get
            {
                return (AccountNationality != null && !String.IsNullOrEmpty(AccountNationality.Code));
            }
        }

        private bool IsPatientHasRace
        {
            get
            {
                return (AccountRace != null && !String.IsNullOrEmpty(AccountRace.Code));
            }
        }
       
        public Race GetKeyRaceFromDictionary(string keyRaceCode)
        {
            Race keyRaceSearchedInRaceCollection = null;
            foreach (Race de in RaceCollection) 
            {
                var keyRace = de;
                if (keyRace.Code == keyRaceCode)
                {
                    return keyRace;
                }
            }
            return keyRaceSearchedInRaceCollection;
        }

     
        #endregion 
        #region Contants
        Race BlankRace = new Race(-1, ReferenceValue.NEW_VERSION, " ", Race.ZERO_CODE);
        #endregion

}
}
