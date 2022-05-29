using System;

namespace PatientAccess.Domain
{
    [Serializable]
    public class KindOfConstraint : ReferenceValue
    {
        #region Event Handlers
        #endregion

        #region Methods       

        #endregion

        #region Properties

        private bool AuthorizationRequired
        {
            get
            {
                return i_authorizationRequired;
            }
            set
            {
                i_authorizationRequired = value;
            }
        }

        public bool IsInNetwork
        {
            get
            {
                return i_isInNetwork;
            }
            set
            {
                i_isInNetwork = value;
            }
        }

        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public KindOfConstraint()
        {
        }
        public KindOfConstraint( long oid, string description )
            : base( oid, description )
        {
        }

        public KindOfConstraint( long oid, string description, bool authReq, bool isInNetwork )
            : base( oid, description )
        {
            this.AuthorizationRequired  = authReq;
            this.IsInNetwork            = IsInNetwork;
        }

        #endregion

        #region Data Elements

        private bool                                        i_authorizationRequired = false;
        private bool                                        i_isInNetwork = true;

        #endregion

        #region Constants
        #endregion
    }
}
