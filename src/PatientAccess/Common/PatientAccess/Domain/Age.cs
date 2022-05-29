using System;
using Extensions.PersistenceCommon;

namespace PatientAccess.Domain
{
    [Serializable]
    public class Age : PersistentModel
    {
        #region Event Handlers
        #endregion

        #region Methods
      
        #endregion

        #region Properties

        private int AgeNumber
        {
            get
            {
                return i_AgeNumber;
            }
            set
            {
                i_AgeNumber = value;
            }
        }

        private AgeType AgeType
        {
            get
            {
                return i_Type;
            }
            set
            {
                i_Type = value;
            }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public Age()
        {
        }

        public Age( AgeType type, int number )
            : this( NEW_OID, NEW_VERSION, type, number )
        {
        }

        private Age( long oid, DateTime version, AgeType type, int number )
           : base( oid, version )
        {
            this.AgeType        = type;
            this.AgeNumber = number;
                    
        }
        #endregion

        #region Data Elements
        private int i_AgeNumber ;
        private AgeType i_Type = new AgeType();
        #endregion

        #region Constants
        #endregion
    }
}
