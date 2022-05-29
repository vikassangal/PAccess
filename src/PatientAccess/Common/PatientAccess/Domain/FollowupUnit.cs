using System;

namespace PatientAccess.Domain 
{
    [Serializable]
    public class FollowupUnit : ReferenceValue 
    {
        #region Event Handlers
        #endregion

        #region Methods
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion
        #region Public Methods
        public string AsFormattedString()
        {
            if( this.Oid.ToString().Length < 3 )
            {
                return this.Oid.ToString().PadLeft( 3, '0' );
            }
            else
            {
                return this.Oid.ToString();
            }
        }
        #endregion
        #region Private Properties
        #endregion

        #region Construction and Finalization
        public FollowupUnit()
        {
        }
        public FollowupUnit( long oid, DateTime version, string description )
            : base( oid, version, description )
        {
        }

        #endregion

        #region Data Elements

        #endregion
    }
}
