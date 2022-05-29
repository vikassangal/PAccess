using System;
using System.Collections; 
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Rules;
using PatientAccess.UI.DemographicsViews.Views;
using PatientAccess.Utilities;

namespace PatientAccess.UI.DemographicsViews.Presenters
{ 
    public class GenderViewPresenter 
    {
        #region Events
        public event EventHandler RefreshTopPanel;
        public event EventHandler SetNewBornName ;
        #endregion

        #region Fields

        private readonly IRuleEngine _ruleEngine = Rules.RuleEngine.GetInstance(); 

        #endregion Fields

        #region Constructors
        public GenderViewPresenter(IGenderView view , Account account , string genderProperty )
        {
            Guard.ThrowIfArgumentIsNull(view, "view");
            Guard.ThrowIfArgumentIsNull(account, "account"); 
            Guard.ThrowIfArgumentIsNull(genderProperty, "genderProperty"); 

            GenderView = view;
            ModelAccount = account;
            GenderProperty = genderProperty;
            GenderView.GenderViewPresenter = this;
        }

        #endregion Constructors
        
        #region Properties

        private IGenderView GenderView { get; set; }

        private IRuleEngine RuleEngine
        {
            get { return _ruleEngine; }
        }

        private Account ModelAccount { get; set; }

        #endregion Properties

        #region Event Handlers
        private void GenderRequiredEventHandler(object sender, EventArgs e)
        {
            GenderView.MakeGenderRequired();
        }
        private void InvalidGenderCodeChangeEventHandler(object sender, EventArgs e)
        {
            GenderView.ProcessInvalidCode();
        }
        private void InvalidGenderCodeEventHandler(object sender, EventArgs e)
        {
            GenderView.MakeGenderControlError();
        }

        #endregion Event Handlers

        #region Public Methods

        public void ValidateGender()
        {
            GenderView.SetNormal();
            RunRules();
        }

        public void UpdateGenderSelected(Gender gender)
        {
            RunInvalidRules();
            switch (GenderProperty)
            {
                case Gender.PATIENT_GENDER :
                    UpdateSelectedPatientGender(gender);
                    RefreshPatientPanel();
                    UpdateNewBornNameOnGenderSelected();
                    break;
                case Gender.BIRTH_GENDER :
                    UpdateSelectedBirthGender(gender);
                    break;
            }
        }

        private void RefreshPatientPanel()
        {
            if (RefreshTopPanel != null)
            {
                RefreshTopPanel(this, new LooseArgs(ModelAccount.Patient));
            }
        }

        private void UpdateNewBornNameOnGenderSelected()
        {
            if (SetNewBornName != null)
            {
                SetNewBornName(this, new LooseArgs(ModelAccount.Patient));
            }
        }

        public void UpdateView()
        {
            if (GenderView.GenderControl.Count == 0)
            {
                GenderView.GenderControl.InitializeGendersComboBox();
            }

            switch (GenderProperty)
            {
                case Gender.PATIENT_GENDER :
                    if (ModelAccount.Patient.Sex != null)
                    {
                        var gender = ModelAccount.Patient.Sex.AsDictionaryEntry() ;
                        GenderView.UpdateGenderOnView( gender);
                    }

                    break;
                case  Gender.BIRTH_GENDER :
                    if (ModelAccount.Patient.BirthSex != null)
                    {
                        var birthGender = ModelAccount.Patient.BirthSex.AsDictionaryEntry();
                        GenderView.UpdateGenderOnView(birthGender);
                    }

                    break;
            }
        }

        #endregion Public Methods

        #region Private Methods
        public void RunRules()
        {
            RunInvalidRules();
            RunRequiredRules();
        }

        private void RunInvalidRules()
        {
            GenderView.SetNormal();
            RuleEngine.OneShotRuleEvaluation<InvalidGenderCodeChange>(ModelAccount,
                InvalidGenderCodeChangeEventHandler);
            RuleEngine.OneShotRuleEvaluation<InvalidGenderCode>(ModelAccount, InvalidGenderCodeEventHandler);
        }
        private void RunRequiredRules()
        {
            if (GenderProperty == Gender.PATIENT_GENDER)
            {
                GenderView.SetNormal();
                RuleEngine.OneShotRuleEvaluation<GenderRequired>(ModelAccount, GenderRequiredEventHandler);
            }
        }
        public string GenderProperty { get; set; }

        private void UpdateSelectedPatientGender(Gender gender)
        {
            if (ModelAccount != null && ModelAccount.Patient != null)
            {
                ModelAccount.Patient.Sex = gender;

                if ( !ModelAccount.Activity.IsPreMSEActivities() && ModelAccount.Patient.Sex != null &&
                    ModelAccount.Patient.Sex.Code == Gender.MALE_CODE)
                {
                    RemoveLastMenstruationOccurrenceCode(); 
                }
            }

            RunRequiredRules();
        }

        private void UpdateSelectedBirthGender(Gender gender)
        {
            if (ModelAccount != null && ModelAccount.Patient != null)
            {
                ModelAccount.Patient.BirthSex = gender;
            }
        }
    
        private void RemoveLastMenstruationOccurrenceCode()
        {
            IList occCodes = ModelAccount.OccurrenceCodes;
            if (occCodes == null || occCodes.Count == 0)
            {
                return;
            }

            for (int i = 0; i < occCodes.Count; i++)
            {
                var occurrenceCode = occCodes[i] as OccurrenceCode;
                if (occurrenceCode != null && occurrenceCode.Code == OccurrenceCode.OCCURRENCECODE_LASTMENSTRUATION)
                {
                    ModelAccount.OccurrenceCodes.RemoveAt(i);
                    break;
                }
            }
        }

        #endregion Private Methods

     
    }
}