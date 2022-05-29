using System;
using PatientAccess.Domain;
using PatientAccess.Utilities;
using System.Collections;
using System.Collections.Generic;
using PatientAccess.Rules; 
using PatientAccess.BrokerInterfaces;

namespace PatientAccess.UI.DemographicsViews
{
    public class EthnicityViewPresenter : IEthnicityViewPresenter
    {
        #region Variables and Properties
        
        private IEthnicityView EthnicityView;
        public readonly Account ModelAccount;
        private readonly string ETHNICITYType;
        public IDictionary<Ethnicity, ArrayList> EthnicityDictionary { get; set; }
        Ethnicity BlankEthnicity = new Ethnicity(-1, ReferenceValue.NEW_VERSION, " ", Ethnicity.ZERO_CODE);

        #endregion

        #region Constructors
        public EthnicityViewPresenter(IEthnicityView ethnicityView, Account account, string ethnicityType)
        {
            Guard.ThrowIfArgumentIsNull(ethnicityView, "EthnicityView");
            Guard.ThrowIfArgumentIsNull(account, "account");

            EthnicityView = ethnicityView;
            ModelAccount = account;
            EthnicityView.EthnicityViewPresenter = this;
            ETHNICITYType = ethnicityType;
        }
        #endregion

        #region Public Methods

        public void UpdateView()
        {
            EthnicityView.ModelAccount = ModelAccount;
            PopulateEthnicity();
            UpdateEthnicityToView();
            RunInvalidCodeRules();
            RunRules();
        }

        public void RunInvalidCodeRules()
        {
            if (ETHNICITYType.Equals(Ethnicity.ETHNICITY_PROPERTY))
            {
                RuleEngine.GetInstance()
                    .OneShotRuleEvaluation<InvalidEthnicityCode>(ModelAccount, InvalidEthnicityCodeEventHandler);
                RuleEngine.GetInstance()
                    .OneShotRuleEvaluation<InvalidEthnicityCodeChange>(ModelAccount,
                        InvalidEthnicityCodeChangeEventHandler);
            }

            if (ETHNICITYType.Equals(Ethnicity.ETHNICITY2_PROPERTY))
            {
                RuleEngine.GetInstance()
                    .OneShotRuleEvaluation<InvalidEthnicity2Code>(ModelAccount, InvalidEthnicityCodeEventHandler);
                RuleEngine.GetInstance()
                    .OneShotRuleEvaluation<InvalidEthnicity2CodeChange>(ModelAccount,
                        InvalidEthnicityCodeChangeEventHandler);
            }
        }

        public void RunRules()
        {
            EthnicityView.SetNormalBgColor();
            if (ETHNICITYType.Equals(Ethnicity.ETHNICITY_PROPERTY))
            {
                RuleEngine.GetInstance().OneShotRuleEvaluation<EthnicityPreferred>(ModelAccount, EthnicityPreferredEventHandler);
                RuleEngine.GetInstance().OneShotRuleEvaluation<EthnicityRequired>(ModelAccount, EthnicityRequiredEventHandler);
            }
        }

        public void UpdateEthnicityAndDescentModelValue(Ethnicity ethnicity)
        {
            if (ETHNICITYType.Equals(Ethnicity.ETHNICITY_PROPERTY))
            {
                if (!string.IsNullOrEmpty(ethnicity.ParentEthnicityCode))
                {
                    ModelAccount.Patient.Descent = ethnicity;
                    Ethnicity parentEthnicity = GetKeyEthnicityFromDictionary(ethnicity.ParentEthnicityCode);
                    if (parentEthnicity != null)
                    ModelAccount.Patient.Ethnicity = parentEthnicity;
                }
                else
                {
                    ModelAccount.Patient.Ethnicity = ethnicity;
                    ModelAccount.Patient.Descent = new Ethnicity();
                }
            }

            if (ETHNICITYType.Equals(Ethnicity.ETHNICITY2_PROPERTY))
            {
                if (!string.IsNullOrEmpty(ethnicity.ParentEthnicityCode))
                {
                    ModelAccount.Patient.Descent2 = ethnicity;
                    Ethnicity parentEthnicity = GetKeyEthnicityFromDictionary(ethnicity.ParentEthnicityCode);
                    if (parentEthnicity != null)
                    ModelAccount.Patient.Ethnicity2 = parentEthnicity;
                }
                else
                {
                    ModelAccount.Patient.Ethnicity2 = ethnicity;
                    ModelAccount.Patient.Descent2 = new Ethnicity();
                }
            }
        }

        #endregion

        #region Private Methods

        private void InvalidEthnicityCodeChangeEventHandler(object sender, EventArgs e)
        {
           EthnicityView.ProcessInvalidCodeEvent();
        }

        private void InvalidEthnicityCodeEventHandler(object sender, EventArgs e)
        {
            EthnicityView.SetDeactivatedBgColor();
        }
        
        private void EthnicityRequiredEventHandler(object sender, EventArgs e)
        {
           EthnicityView.SetEthnicityAsRequiredColor();
        }

        private void EthnicityPreferredEventHandler(object sender, EventArgs e)
        {
            EthnicityView.SetEthnicityAsPreferredColor();
        }

        private void PopulateEthnicity()
        {

            var originBroker = BrokerFactory.BrokerOfType<IOriginBroker>();
            EthnictyCollection = originBroker.LoadEthnicities(User.GetCurrent().Facility.Oid);
            DescentCollection = originBroker.LoadDescent(User.GetCurrent().Facility.Oid);
            EthnicityDictionary = BuildEthnicityDictionary();
            BuildDescent();
            EthnicityView.PopulateEthnicity(EthnicityDictionary);
            
        }
        public ICollection EthnictyCollection { get; set; }

        public ICollection DescentCollection { get; set; }
        public IDictionary<Ethnicity, ArrayList> BuildEthnicityDictionary()
        {
            EthnicityDictionary = new Dictionary<Ethnicity, ArrayList>();
            EthnicityDictionary.Add(BlankEthnicity, new ArrayList());
            foreach (Ethnicity ethnicity in EthnictyCollection)
            {
                if (!EthnicityDictionary.ContainsKey(ethnicity))
                {
                    var ethnicityList = new ArrayList();
                    EthnicityDictionary.Add(ethnicity,ethnicityList);
                }
            }
            return EthnicityDictionary;
        }
        public void BuildDescent()
        {
            foreach (Ethnicity descent in DescentCollection)
            {
                var parentEthnictyCode = descent.ParentEthnicityCode;
                var parentEthnicity = GetKeyEthnicityFromDictionary(parentEthnictyCode);
                if (parentEthnicity != null)
                {
                    if (EthnicityDictionary.ContainsKey(parentEthnicity))
                    {
                        if (!EthnicityDictionary[parentEthnicity].Contains(descent))
                        {
                            EthnicityDictionary[parentEthnicity].Add(descent);
                        }
                    }
                    else
                    {
                        ArrayList raceList = null;
                        raceList = new ArrayList();
                        EthnicityDictionary.Add(parentEthnicity, raceList);
                        if (!EthnicityDictionary[parentEthnicity].Contains(descent))
                        {
                            EthnicityDictionary[parentEthnicity].Add(descent);
                        }
                    }
                }
            }
        }

        private Ethnicity AccountEthnicity
        {
            get
            {
                if (ETHNICITYType.Equals(Ethnicity.ETHNICITY_PROPERTY))
                {
                    return ModelAccount.Patient.Ethnicity;
                }
                else if (ETHNICITYType.Equals(Ethnicity.ETHNICITY2_PROPERTY))
                {
                    return ModelAccount.Patient.Ethnicity2;
                }
                else return new Ethnicity();
            }
        }

        private Ethnicity AccountDescent
        {
            get
            {
                if (ETHNICITYType.Equals(Ethnicity.ETHNICITY_PROPERTY))
                {
                    return ModelAccount.Patient.Descent;
                }
                else if (ETHNICITYType.Equals(Ethnicity.ETHNICITY2_PROPERTY))
                {
                    return ModelAccount.Patient.Descent2;
                }
                else return new Ethnicity();
            }
        }
        private void UpdateEthnicityToView()
        {
            if (IsPatientHasEthnicity &&
                IsPatientHasDescent)
            {
                EthnicityView.Ethnicity = AccountDescent;
            }
            if (IsPatientHasEthnicity &&
                !IsPatientHasDescent)
            {
                EthnicityView.Ethnicity = AccountEthnicity;
            }
        }

        private bool IsPatientHasDescent
        {
            get
            {
                return (AccountDescent != null && !String.IsNullOrEmpty(AccountDescent.Code));
            }
        }
      
        private bool IsPatientHasEthnicity
        {
            get
            {
                return (AccountEthnicity != null && !String.IsNullOrEmpty(AccountEthnicity.Code));
            }
        }
     
        public Ethnicity GetKeyEthnicityFromDictionary(string keyEthnicityCode)
        {

            Ethnicity keyEthnicitySearchedInDictionary = null;
            foreach (Ethnicity de in EthnicityDictionary.Keys) // checking to see if there is a entry for r1 in the dictionary
            {
                var keyEthnicity = de;
                if (keyEthnicity.Code == keyEthnicityCode)
                {
                    return keyEthnicity;
                }
            }
            return keyEthnicitySearchedInDictionary;
        }
        
        #endregion 
        
    }
}
