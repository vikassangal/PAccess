using System;
using System.Collections;

namespace PatientAccess.Domain.Parties
{
    [Serializable]
    public class Employer : Organization, IEmployer, ICloneable
    {
        #region Event Handlers
        #endregion

        #region Methods

        public static string ModifyPBAREmployerName(string origName)
        {
            return origName.PadRight(PBAR_EMP_NAME_LENGTH,' ').Substring(0,PBAR_EMP_NAME_LENGTH).TrimEnd();
        }

        public override Insured CopyAsInsured()
        {
            Insured result = new Insured();
            result.LastName = this.Name;

            ContactPoint newCP = new ContactPoint();
            
            result.RemoveContactPoint( newCP.TypeOfContactPoint );
            newCP   = (ContactPoint)this.PartyContactPoint.Clone();
            newCP.TypeOfContactPoint = TypeOfContactPoint.NewPhysicalContactPointType();
            result.AddContactPoint ( newCP );
            
            return result;
        }

        public override Patient CopyAsPatient()
        {
            Patient result = new Patient();
            result.LastName = this.Name;

            ContactPoint newCP = new ContactPoint();
            
            result.RemoveContactPoint( newCP.TypeOfContactPoint );
            newCP   = (ContactPoint)this.PartyContactPoint.Clone();
            newCP.TypeOfContactPoint = TypeOfContactPoint.NewMailingContactPointType();
            result.AddContactPoint ( newCP );
            
            return result;
        }

        public override Guarantor CopyAsGuarantor()
        {
            Guarantor result = new Guarantor();
            result.LastName = this.Name;

            ContactPoint newCP = new ContactPoint();

            result.RemoveContactPoint( newCP.TypeOfContactPoint );
            newCP   = (ContactPoint)this.PartyContactPoint.Clone();
            newCP.TypeOfContactPoint = TypeOfContactPoint.NewMailingContactPointType();
            result.AddContactPoint ( newCP );

            return result;
        }

        public void AddAddress( Address address )
        {
            EmployerContactPoint cp = new EmployerContactPoint();
            cp.Address = address;
            this.AddContactPoint(cp);
        }

        public new object Clone()
        {
            Employer newObject = new Employer();
            newObject.Name = (string) this.Name.Clone();
            newObject.EmployerCode = this.EmployerCode;
            newObject.NationalId = this.NationalId;
            newObject.Industry = (string) this.Industry.Clone();
            newObject.PartyContactPoint = (ContactPoint) this.PartyContactPoint.Clone();
            newObject.primContactPoints = new ArrayList();
            foreach( ContactPoint cp in this.primContactPoints )
            {
                newObject.primContactPoints.Add( cp );
            }
            base.Clone();
            return newObject;
        }

        public override bool Equals( object obj )
        {
            if( obj == null )
            {
                return false;
            }
            Employer emp = obj as Employer;
            bool result = ( emp.Name == this.Name  &&
                emp.EmployerCode == this.EmployerCode &&
                emp.NationalId == this.NationalId &&
                emp.Industry == this.Industry  &&
                base.Equals( emp ));
            return result;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public IEmployer AsEmployer()
        {
            return this;
        }

        public override void AddContactPoint( ContactPoint aContactPoint )
        {
            if( !this.primContactPoints.Contains( aContactPoint ) )
            {
                this.primContactPoints.Add( aContactPoint );
            }
        }

        public override void RemoveContactPoint( ContactPoint aContactPoint )
        {
            if( this.primContactPoints.Contains( aContactPoint ) )
            {
                this.primContactPoints.Remove( aContactPoint );
            }
        }
        public override ICollection ContactPoints
        {
            get
            {
                return (ICollection)primContactPoints.Clone();
            }
        }
        #endregion

        #region Properties
        protected override ArrayList primContactPoints
        {
            get
            {
                return i_employerContactPoints;
            }
            set
            {
                i_employerContactPoints = value;
            }
        }

        public long EmployerCode
        {
            get
            {
                return i_EmployerCode;
            }
            set
            {
                i_EmployerCode = value;
            }
        }

        public string NationalId
        {
            get
            {
                return this.i_NationalId;
            }
            set
            {
                this.i_NationalId = value;
            }
        }

        public string Industry
        {
            get
            {
                return this.i_Industry;
            }
            set
            {
                this.i_Industry = value;
            }
        }

        /// <summary>
        /// The single ContactPoint from the Employer's CP collection that is for the party with whom
        /// this Employer is associated
        /// </summary>
        public ContactPoint PartyContactPoint
        {
            get
            {
                return i_PartyContactPoint;
            }
            set
            {
                i_PartyContactPoint = value;
            }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public Employer() : base()
        {
        }

        private Employer( long oid, DateTime version )
            : base( oid, version )
        {
        }

        private Employer( long oid, DateTime version, string nameOfParty )
            : this( oid, version )
        {
            this.Name = nameOfParty;
        }

        public Employer( long oid, DateTime version, string nameOfParty, string nationalId, long employerCode )
            : this( oid, version, nameOfParty )
        {
            this.NationalId = nationalId;
            this.EmployerCode = employerCode;
        }

        public Employer( long oid, DateTime version, string nameOfParty, string nationalId, long employerCode, ArrayList addresses )
            : this( oid, version, nameOfParty, nationalId, employerCode )
        {
            if( addresses != null )
            {
                foreach( Address address in addresses )
                {
                    this.AddAddress(address);
                }
            }
        }
        #endregion

        #region Data Elements
        private string          i_NationalId = String.Empty;
        private long            i_EmployerCode;
        private string          i_Industry = String.Empty;
        private ArrayList       i_employerContactPoints = new ArrayList();
        private ContactPoint    i_PartyContactPoint = new ContactPoint( TypeOfContactPoint.NewEmployerContactPointType() );
        #endregion

        #region Constants
        public const int       PBAR_EMP_NAME_LENGTH    = 23;
        #endregion
    }
}