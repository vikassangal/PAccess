using System;
using System.Diagnostics;
using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI.Factories;
using PatientAccess.UI.Logging;

namespace PatientAccess.UI.AddressViews
{
    [Serializable]
    public class EmployerAddressEntryView : AddressEntryView
    {
        public EmployerAddressEntryView()
        {

        }

        public EmployerAddressEntryView( Facility facility, RuleEngine ruleEngine ) : base( facility, ruleEngine )
        {

        }

        protected override void IsVerificationValid()
        {
             
            YesNoFlag yesNo = new YesNoFlag();
            yesNo.SetNo();
            State selectedState = (State)this.comboBox_States.SelectedItem;

            if (selectedCountry != null &&
                 selectedCountry.Code != String.Empty &&
                CountryIsUSorUSTerritory &&                
                this.mtbCity.Text.Trim().Length > 0 &&
                selectedState != null &&
                selectedState.Code.Trim() != String.Empty )
            {
                MaskedEditTextBoxBuilder.ConfigureUSZipCode(axZipCode);
                this.btnVerify.Enabled = true;
                yesNo.SetYes();
            }
            else if (selectedCountry != null &&
                selectedCountry.Code != String.Empty &&
                !CountryIsUSorUSTerritory &&
                this.mtbCity.Text.Trim().Length > 0 &&
                selectedState != null &&
                selectedState.Code.Trim() != String.Empty &&
                HadValidZipCodeLength
                )
            {
              
                this.btnVerify.Enabled = true;
                yesNo.SetYes();
                MaskedEditTextBoxBuilder.ConfigureNonUSZipCode(axZipCode);
            }
            else
            {
                this.btnVerify.Enabled = false;
                yesNo.SetNo();
            }

            this.FireVerificationButtonEnabledEvent(yesNo);
        }
        public override bool AllFieldsValid()
        {
            // ****** PLEASE DO NOT REMOVE - CROSS-THREAD DEBUGGING - THIS WILL BE REMOVED ONCE THE BUG IS FIXED ******

            StackTrace st = new StackTrace();
            string stackOutput = StackTracer.LogTraceLog(st, this.comboBox_Countries.InvokeRequired);
            // this is being logged using the BreadCrumbLogger in namespace UI.Logging
            BreadCrumbLogger.GetInstance.Log(String.Format(stackOutput));

            // ********************************************************************************************************

            //mark all empty fields as required.
            if (this.comboBox_Countries.SelectedIndex < 0)
            {
                this.comboBox_Countries.Select();
                return false;
            }
           
            if (this.mtbCity.Text == String.Empty)
            {
                this.mtbCity.Select();
                return false;
            }

            if (this.comboBox_States.SelectedIndex < 1)
            {
                this.comboBox_States.Select();
                return false;
            }

            return true;
        }
        public override bool AllRequiredFieldsValid()
        {
            //mark all empty fields as required.
            if (this.comboBox_Countries.SelectedIndex < 0)
            {
                return false;
            }
            
            if (this.mtbCity.Text == String.Empty)
            {
                return false;
            }

            if (this.comboBox_States.SelectedIndex < 1)
            {
                return false;
            }

            return true;

        }

        public override  void AllFieldsRequiredRules()
        {
            RuleEngine.GetInstance().RegisterEvent(typeof(AddressCityRequired), this.Model, this.mtbCity, new EventHandler(AddressCityRequiredEventHandler));
            RuleEngine.GetInstance().RegisterEvent(typeof(AddressStateRequired), this.Model, new EventHandler(AddressStateRequiredEventHandler));
            this.runRules();
        }

        public override void OnlyStreetAndCountryRequiredRule()
        {
            RuleEngine.GetInstance().UnregisterEvent(typeof(AddressCityRequired), this.Model, new EventHandler(AddressCityRequiredEventHandler));
            RuleEngine.GetInstance().UnregisterEvent(typeof(AddressStateRequired), this.Model, new EventHandler(AddressStateRequiredEventHandler));
            this.runRules();
        }

        public override void OnlyStreetCityAndCountryRequiredRules()
        {
            RuleEngine.GetInstance().RegisterEvent(typeof(AddressCityRequired), this.Model, this.mtbCity, new EventHandler(AddressCityRequiredEventHandler));

            RuleEngine.GetInstance().RegisterEvent(typeof(AddressStateRequired), this.Model, new EventHandler(AddressStateRequiredEventHandler));
            this.runRules();
        }

        public override void RegisterStreetRule()
        {
            RuleEngine.GetInstance().RegisterEvent(typeof(AddressCityRequired), this.Model, this.mtbCity, new EventHandler(AddressCityRequiredEventHandler));

            RuleEngine.GetInstance().RegisterEvent(typeof(AddressStateRequired), this.Model, new EventHandler(AddressStateRequiredEventHandler));
            this.runRules();
        }
   
        public override void UnregisterStreetRule()
        {
            return; 
        }

        private bool HadValidZipCodeLength
        {
            get
            {
                if (CountryIsUSorUSTerritory)
                {
                    return this.axZipCode.UnMaskedText.Length >= 5;
                }
                else
                {
                    return this.axZipCode.UnMaskedText.Length >= 1;
                }
            }
        }
        #region Data Elements
        #endregion
    }

}
