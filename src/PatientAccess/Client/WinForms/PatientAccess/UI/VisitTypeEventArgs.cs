using System;
using PatientAccess.Domain;

namespace PatientAccess.UI
{
    public class VisitTypeEventArgs : EventArgs
    {
        private readonly VisitType _visitType;

        public VisitTypeEventArgs( VisitType visitType )
        {
            this._visitType = visitType;
        }
        
        public VisitType VisitType
        {
            get
            {
                return this._visitType;
            }
        }
    }

}
