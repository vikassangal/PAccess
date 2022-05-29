using System;
using System.Collections;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    public interface IInterFacilityTransferFeatureManager
    {
        bool IsInterFacilityTransferFeatureEnabled(Activity activity);
        bool IsInterFacilityTransferFeatureEnabledForFacility();
        bool IsITFREnabled(Facility facility, Account activity);
        bool IsITFREnabled(Facility facility, Activity activity);
        bool IsHSVEnable(string hsv);
        bool IsPatientTypeEnable(string pt);
    }

    [Serializable]
    public class InterFacilityTransferFeatureManager : IInterFacilityTransferFeatureManager
    {
        private readonly Facility CurrentUserFacility;

        private ArrayList AllITFRFacilities = new ArrayList();
        
        public ArrayList AllTransferFacilities
        {
            get
            {
                if (AllITFRFacilities.Count == 0)
                {
                    AllTransferHospitals(CurrentUserFacility);
                }

                return AllITFRFacilities;
            }
        }

        public InterFacilityTransferFeatureManager()
        {
            CurrentUserFacility = User.GetCurrent().Facility;
            AllTransferHospitals(CurrentUserFacility);
        }

        public bool IsHSVEnable(string hsv)
        {
            //if (hsv == "56" || hsv == "57" || hsv == "58" || hsv == "59" || hsv == "65")
            //{
            //    return true;
            //}

            return true;
        }

        public bool IsPatientTypeEnable(string pt)
        {
            //if (pt != "9" && pt != "4")
            //{
            //    return true;
            //}

            return true;
        }
        public InterFacilityTransferFeatureManager(Facility facility)
        {
            CurrentUserFacility = facility;
            AllTransferHospitals(CurrentUserFacility);
        }

        public bool IsInterFacilityTransferFeatureEnabled(Activity activity)
        {
            return true; 
            //return ((CurrentUserFacility != null) && CurrentUserFacility.IsInterFacilityTransferEnabled && (activity != null) && activity.IsValidForUpdateFromInterFacilityTransfer);
        }
   
        public bool IsInterFacilityTransferFeatureEnabledForFacility()
        {
            return ((CurrentUserFacility != null) && CurrentUserFacility.IsInterFacilityTransferEnabled);
        }

        public void AllTransferHospitals(Facility facility)
        {
            IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();

            ArrayList transferHospitals = new ArrayList();
            var interfacilityTransferBroker = BrokerFactory.BrokerOfType<IInterfacilityTransferBroker>();
            transferHospitals = interfacilityTransferBroker.AllInterFacilityTransferHospitals(facility);
            if (transferHospitals.Count > 1)
            {
                AllITFRFacilities.Clear();
                foreach (string facilityCode in transferHospitals)
                {
                    if (!string.IsNullOrEmpty(facilityCode.Trim()))
                    {
                        Facility itfrFacility = facilityBroker.FacilityWith(facilityCode.Trim());
                        if (itfrFacility != null)
                        {
                            AllITFRFacilities.Add(itfrFacility);
                        }
                    }
                }
            }
        }

        public bool IsITFREnabled(Facility facility, Account account)
        {
            if ((account.Activity.IsQuickAccountCreationActivity() 
                 || account.Activity.IsDischargeActivity()
                 || ((account.Activity.IsRegistrationActivity() || account.Activity.IsDiagnosticRegistrationActivity())
                     && (account.Activity.AssociatedActivityType == typeof(ActivatePreRegistrationActivity) 
                         || account.Activity.AssociatedActivityType==null))
                )
                && IsIFTREnabledFacility(facility) 
                && IsPatientTypeEnable(account.KindOfVisit.Code)
                && IsHSVEnable(account.HospitalService.Code))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// This method only for Quick Account
        /// </summary>
        /// <param name="facility"></param>
        /// <param name="activity"></param>
        /// <returns></returns>
        public bool IsITFREnabled(Facility facility, Activity activity)
        {
            if ((activity.IsQuickAccountCreationActivity()
                 || activity.IsDischargeActivity()
                 || (activity.IsRegistrationActivity()
                     && (activity.AssociatedActivityType == typeof(ActivatePreRegistrationActivity)
                         || activity.AssociatedActivityType == null))
                )
                && IsIFTREnabledFacility(facility)
               )
            {
                return true;
            }
            return false;
        }

        public bool IsIFTREnabledFacility(Facility facility)
        {
            bool isIFTFREnabledFacility = false;
            
            foreach(Facility fac in AllITFRFacilities)
            {
                if (fac.Code==facility.Code)
                {
                    isIFTFREnabledFacility = true;
                    break;
                }
            }
            return isIFTFREnabledFacility;
        }
    }
}
