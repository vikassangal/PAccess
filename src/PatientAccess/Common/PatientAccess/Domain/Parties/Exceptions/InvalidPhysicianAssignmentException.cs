using System;
using System.Runtime.Serialization;
using Extensions.Exceptions;

namespace PatientAccess.Domain.Parties.Exceptions
{
	/// <summary>
	/// Summary description for InvalidPhysicianAssignmentException.
	/// </summary>
	//TODO: Create XML summary comment for InvalidPhysicianAssignmentException
	[Serializable]
	public class InvalidPhysicianAssignmentException : EnterpriseException
	{
		#region Event Handlers
		#endregion

		#region Methods
		#endregion

		#region Properties
		public long PhysicianNumber
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

		public string PhysicianName
		{
			get
			{
				return i_PhysicianName;
			}
			set
			{
				i_PhysicianName = value;
			}
		}

		public string RelationshipType
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
		public InvalidPhysicianAssignmentException()
            : base()
		{
		}
        public InvalidPhysicianAssignmentException(string msg)
            : base(msg)
        {
        }
        public InvalidPhysicianAssignmentException (string msg,Exception ex)
            : base(msg,ex)
        {
        }
        public InvalidPhysicianAssignmentException( string message, 
            Exception innerException,
            Severity severity )
            : base( message, innerException, severity )
        {
        }
        protected InvalidPhysicianAssignmentException( SerializationInfo info, 
            StreamingContext context ) 
            : base( info, context )
        {
        }

        public InvalidPhysicianAssignmentException( long physicianNumber,
            string physicianName,
            string relationshipType )
        {
            this.PhysicianNumber = physicianNumber;
            this.PhysicianName = physicianName;
            this.RelationshipType = relationshipType;
        }
        public InvalidPhysicianAssignmentException( long physicianNumber,
            string physicianName,
            string relationshipType, 
            Severity severity)
        {
            this.PhysicianNumber = physicianNumber;
            this.PhysicianName = physicianName;
            this.RelationshipType = relationshipType;
            this.Severity = severity;
        }
        #endregion

		#region Data Elements
		private long i_PhysicianNumber;
		private string i_RelationshipType;
		private string i_PhysicianName;
		#endregion

		#region Constants
		#endregion
	}
}
