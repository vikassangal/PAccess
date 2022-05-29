using System;

namespace PatientAccess.Domain
{
    [Serializable]
    public class DOFRAPIRequest
    {
        public const string Expansion = "Expansion";
        public const string NonExpansion = "Non-Expansion";

        public string facilityCode { get; set; }
        public string patientType { get; set; }
        public string clinicCode { get; set; }
        public string serviceCode { get; set; }
        public string ipaCode { get; set; }
        public string healthPlanCode { get; set; }
        public string productType { get; set; }
        public string serviceCategory { get; set; }
        public string planType { get; set; }

        public enum DOFRAPIPatientType
        {
            I,
            E,
            O
        }
        
        public override string ToString()
        {
            return "FacilityCode:" + facilityCode +
                "patientType:" + patientType +
                "clinicCode:" + clinicCode +
                "serviceCode:" + serviceCode +
                "ipaCode:" + ipaCode +
                "healthPlanCode:" + healthPlanCode +
                "productType:" + productType +
                "serviceCategory:" + serviceCategory +
                "planType:" + planType;
            ;
        }
    }
}
