using System;

namespace PatientAccess.Utilities
{
    /// <summary>
    /// Summary description for TypeOfName.
    /// </summary>
    [Serializable]
    public enum ServerEnvironment
    {
        LOCAL ,
        DEVELOPMENT ,
        TEST ,
        MODEL ,
        PRODUCTION ,
        UNKNOWN

    }
}
