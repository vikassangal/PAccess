using System;

namespace PatientAccess.Utilities
{
    /// <summary>
    /// Summary description for EMPIResultStatus.
    /// </summary>
    [Serializable]
    public enum EMPIResultStatus
    {
        EMPIRESULTSFOUND,
        NOEMPIRESULTSFOUND,
        SYSTEMDOWN,
        TIMEOUT
    }
}
