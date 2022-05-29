using System;

namespace PatientAccess.Domain
{
    /// <summary>
    /// Summary description for ReferralSource.
    /// </summary>
    //TODO: Create XML summary comment for ReferralSource
    [Serializable]
    public class ReferralSource : CodedReferenceValue 
    {
        #region Event Handlers
        #endregion

        #region Methods

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            ReferralSource aSrc = obj as ReferralSource;
 
            if( aSrc != null )
            {
                return this.Code == aSrc.Code && this.Description == aSrc.Description;
            }
            else
            {
                return base.Equals( obj );
            }
            
        }


        public override string ToString()
        {   
            return String.Format("{0} {1}", Code.Trim(), Description);
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public ReferralSource()
        {
        }

        public ReferralSource( long oid, DateTime version, string description )
            : base( oid, version, description )
        {
        }

        public ReferralSource( long oid, DateTime version, string description, string code )
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
