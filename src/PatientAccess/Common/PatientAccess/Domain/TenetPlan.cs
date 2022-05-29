using System;

namespace PatientAccess.Domain
{
    /// <summary>
    /// Summary description for AdmitSource.
    /// </summary>
    //TODO: Create XML summary comment for AdmitSource
    [Serializable]
    public class TentPlan : CodedReferenceValue 
    {
        #region Event Handlers
        #endregion

        #region Methods
        public override string ToString()
        {   
            return String.Format("{0} {1}", Code, Description);
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public TentPlan()
        {
        }

        public TentPlan( long oid, DateTime version, string description )
            : base( oid, version, description )
        {
        }

        public TentPlan( long oid, DateTime version, string description, string code )
            : base( oid, version, description, code )
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
