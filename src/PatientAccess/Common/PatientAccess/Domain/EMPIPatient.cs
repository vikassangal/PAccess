using System;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Domain
{
    /// <summary>
    /// Summary description for ActionItem.
    /// </summary>
    [Serializable]
    public class EMPIPatient
    {
       
        public Patient Patient
        {
            get { return i_Patient; }

            set { i_Patient = value; }
        }
        public Insurance Insurance
        {
            get { return i_Insurance; }

            set { i_Insurance = value; }
        }
        public Guarantor Guarantor
        {
            get { return i_Guarantor; }

            set { i_Guarantor = value; }
        }
       
        public EmergencyContact EmergencyContact1
        {
            get { return i_EmergencyContact1; }

            set { i_EmergencyContact1 = value; }
        }

        public string EMPIPrimaryInvalidPlanID { get; set; }
        public string EMPISecondaryInvalidPlanID { get; set; }

        private Patient i_Patient = new Patient();
        private Insurance i_Insurance = new Insurance();
        private Guarantor i_Guarantor = new Guarantor();
        private MedicalGroupIPA i_MedicalGroupIPA = new MedicalGroupIPA();
        private EmergencyContact i_EmergencyContact1 = new EmergencyContact();
        public bool EmpiPatientFound;
    }
}
