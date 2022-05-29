using System;

namespace PatientAccess.Domain
{
    [Serializable]
    public class DOFRAPIResponse
    {
        public string planId { get; set; }
        public string planName { get; set; }
        public string serviceCategory { get; set; }
        public int volume { get; set; }
    }
}
