using System;

namespace PatientAccess.Domain
{
    [Serializable]
    public class AidCode : CodedReferenceValue
    {
        #region Event Handlers
        #endregion

        #region Methods
        public override string ToString()
        {   
            return String.Format("{0}", Description);
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public AidCode()
        {
        }
        
        public AidCode( string description, string code )
            : base( description, code )
        {
        }
        #endregion

        #region Data Elements
    
        #endregion

        #region Constants
        #endregion
    }
}
