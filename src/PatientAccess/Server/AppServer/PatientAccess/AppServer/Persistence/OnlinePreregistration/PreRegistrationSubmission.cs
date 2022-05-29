using System;
using System.Xml;

namespace PatientAccess.Persistence.OnlinePreregistration
{
    public class PreRegistrationSubmission
    {

        public virtual Guid Id { get; set; }

        public virtual DateTime DateTimeReceived { get; set; }

        public virtual DateTime AdmitDate { get; set; }

        public virtual string FirstName { get; set; }

        public virtual string LastName { get; set; }

        public virtual string MiddleInitial { get; set; }

        public virtual string Gender { get; set; }

        public virtual DateTime DateOfBirth { get; set; }

        public virtual bool? ReturningPatient { get; set; }

        public virtual string SSN { get; set; }

        public virtual XmlDocument Message { get; set; }

        public virtual long FacilityId { get; set; }

        public virtual string Address { get; set; }

        public override bool Equals( object obj )
        {
            if ( ReferenceEquals( null, obj ) ) return false;
            if ( ReferenceEquals( this, obj ) ) return true;
            if ( obj.GetType() != typeof( PreRegistrationSubmission ) ) return false;
            return Equals( (PreRegistrationSubmission)obj );
        }

        public virtual bool Equals( PreRegistrationSubmission other )
        {
            if ( ReferenceEquals( null, other ) ) return false;
            if ( ReferenceEquals( this, other ) ) return true;
            return Equals( other.FirstName, FirstName ) && other.DateTimeReceived.Equals( DateTimeReceived );
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ( ( FirstName != null ? FirstName.GetHashCode() : 0 ) * 397 ) ^ DateTimeReceived.GetHashCode();
            }
        }
    }
}
