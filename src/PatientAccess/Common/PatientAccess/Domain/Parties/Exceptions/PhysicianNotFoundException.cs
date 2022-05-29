using System;
using System.Runtime.Serialization;
using Extensions.Exceptions;

namespace PatientAccess.Domain.Parties.Exceptions
{
	/// <summary>
	/// Summary description for PhysicianNotFoundException.
	/// </summary>
	//TODO: Create XML summary comment for PhysicianNotFoundException
	[Serializable]
	public class PhysicianNotFoundException : EnterpriseException
	{
		#region Event Handlers
		#endregion

		#region Methods
		#endregion

		#region Properties

	    private long PhysicianNumber
		{
			get
			{
				return i_PhysicianNumber;
			}
			set
			{
				i_PhysicianNumber = value;
			}
		}

	    private string RelationshipType
		{
			get
			{
				return i_RelationshipType;
			}
			set
			{
				i_RelationshipType = value;
			}
		}
		#endregion

		#region Private Methods
		#endregion

		#region Private Properties
		#endregion

		#region Construction and Finalization
		public PhysicianNotFoundException()
		{
		}
        public PhysicianNotFoundException(string msg)
            : base(msg)
        {
        }
        public PhysicianNotFoundException (string msg,Exception ex)
            : base(msg,ex)
        {
        }
        public PhysicianNotFoundException( string message, 
            Exception innerException,
            Severity severity )
            : base( message, innerException, severity )
        {
        }
        protected PhysicianNotFoundException( SerializationInfo info, 
            StreamingContext context ) 
            : base( info, context )
        {
        }

        public PhysicianNotFoundException( long physicianNumber,
            string relationshipType )
        {
            this.PhysicianNumber = physicianNumber;
            this.RelationshipType = relationshipType;
        }
        public PhysicianNotFoundException( long physicianNumber,
            string relationshipType, 
            Severity severity)
        {
            this.PhysicianNumber = physicianNumber;
            this.RelationshipType = relationshipType;
            this.Severity = severity;
        }
        #endregion

		#region Data Elements
		private long i_PhysicianNumber;
		private string i_RelationshipType;
		#endregion

		#region Constants
		#endregion
	}
}
