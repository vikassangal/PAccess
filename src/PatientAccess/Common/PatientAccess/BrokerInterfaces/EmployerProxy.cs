using System;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.BrokerInterfaces
{
    [Serializable]
    public class EmployerProxy : Organization, IEmployer
    {
        #region Event Handlers
        #endregion

        #region Methods
        public IEmployer AsEmployer(  )
        {
            IEmployerBroker employerBroker = BrokerFactory.BrokerOfType< IEmployerBroker >() ;
            return employerBroker.EmployerFor( this );
        }
        #endregion

        #region Properties

        public Facility Facility
        {
            get
            {
                return i_Facility;
            }
            private set
            {
                i_Facility = value;
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
        public EmployerProxy() : base()
        {
        }

        private EmployerProxy( long oid, DateTime version, Facility facility )
            : base( oid, version )
        {
            this.Facility = facility;
        }

        private EmployerProxy( long oid, DateTime version, string nameOfParty, Facility facility )
            : this( oid, version, facility )
        {
            this.Name = nameOfParty;
        }

        public EmployerProxy( long oid, DateTime version, string nameOfParty, string nationalId, long employerCode, Facility facility )
            : this( oid, version, nameOfParty, facility )
        {
            this.NationalId = nationalId;
            this.EmployerCode = employerCode;
        }
        #endregion

        #region Data Elements
        
        private string          i_NationalId;
        private long            i_EmployerCode;
        private string          i_Industry ;  
        private ContactPoint    i_PartyContactPoint;
        private Facility        i_Facility;

        #endregion

        #region Constants
        // TODO: Derive this from an real source
        //private const long HSPCode = 6;

        #endregion
    }
}