using System;
using Extensions;

namespace PatientAccess.Domain
{
    [Serializable]
    public abstract class CoverageConstraints : Model
    {
        public CoverageConstraints( )
        {
        }

        public abstract void ResetFieldsToDefault( bool resetTracking );
        
    }
}
