using System;

namespace PatientAccess.Domain
{
    /// <summary>
    /// Summary description for PatientSearchResultStatus.
    /// </summary>
    [Serializable]
    public enum PatientSearchResultStatus : int 
    {

        Success     = 1,
        Exception   = 2,
        Unknown     = 3

    }
}
