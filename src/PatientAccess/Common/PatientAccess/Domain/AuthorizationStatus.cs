using System;

namespace PatientAccess.Domain
{
    [Serializable]
    public class AuthorizationStatus : CodedReferenceValue
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
		public AuthorizationStatus()
		{
		}

        public AuthorizationStatus(long oid, string description)
            : base(oid, description)
        {
        }

        public AuthorizationStatus(long oid, string description, string code)
            : base(oid, description, code)
        {
        }

        public AuthorizationStatus(long oid, DateTime version, string code)
            : base(oid, version, code)
        {
        }

        public AuthorizationStatus(long oid, DateTime version, string description, string code)
            : base(oid, version, description, code)
        {
        }
		#endregion

		#region Data Elements
		#endregion

		#region Constants
        
		#endregion
    }
}
