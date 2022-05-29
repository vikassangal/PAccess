using System;

namespace PatientAccess.Domain.Parties
{
    /// <summary>
    /// Summary description for TypeOfRole.
    /// </summary>
    [Serializable]
    public enum TypeOfRole : int 
    {

            Patient = 1,
            Guarantor = 2,
            EmergencyContact = 3,
            NearestRelative = 4,
            Insured = 5
    }
}
