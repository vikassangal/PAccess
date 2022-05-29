
using System;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Rules;

namespace PatientAccess.UI.CommonControls
{
    public class SequesteredPatientPresenter
    {
        #region Properties
        public Account Account { get; private set; }
        private bool isPatientSequestered = true;
        private readonly ISequesteredPatientFeatureManager SequesteredPatientFeatureManager;
        #endregion

        public SequesteredPatientPresenter(SequesteredPatientFeatureManager featureManager, Account account)
        {
            SequesteredPatientFeatureManager = featureManager;
            Account = account;
        }

        public void IsPatientSequestered()
        {
            if (IsNewPatientDetailsAvailable() &&
                !Account.Activity.IsAnyNewBornActivity &&
                !Account.Patient.SequesteredPatientAlertShown &&
                IsSequesteredPatientFeatureManagerEnabled())
            {
                IPatientBroker patientBroker = BrokerFactory.BrokerOfType<IPatientBroker>();
                isPatientSequestered = patientBroker.IsPatientSequestered(Account);

                if (isPatientSequestered)
                {
                    SequesteredPatientsAlertDialog frmSequesteredPatient = new SequesteredPatientsAlertDialog()
                    {
                        FirstName = Account.Patient.FirstName,
                        LastName = Account.Patient.LastName,
                        Phone = Account.Facility.SequesteredHelpLine
                    };
                    frmSequesteredPatient.ShowAlert();
                    
                    Account.Patient.SequesteredPatientAlertShown = true;
                }
            }
        }

        private bool IsSequesteredPatientFeatureManagerEnabled()
        {
            return SequesteredPatientFeatureManager.ShouldFeatureBeEnabled(Account);
        }

        private bool IsNewPatientDetailsAvailable()
        {
            if (!String.IsNullOrEmpty(Account.Patient.FirstName) &&
                !String.IsNullOrEmpty(Account.Patient.LastName) &&
                !String.IsNullOrEmpty(Account.Patient.Sex.Code) &&
                Account.Patient.DateOfBirth != DateTime.MinValue
                )
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
