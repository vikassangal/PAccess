using System;
using System.Collections;

namespace PatientAccess.Domain.Parties
{
//TODO: Create XML summary comment for PhysicianRole
	[Serializable]
	public class PhysicianRole : ReferenceValue, IComparable 
	{
		#region Event Handlers
		#endregion

		#region Methods

	    private static Hashtable allPhysicianRoles()
        {
            Hashtable physicianRoles = new Hashtable();
            physicianRoles.Add(ADMITTING, Admitting());
            physicianRoles.Add(ATTENDING, Attending());
            physicianRoles.Add(CONSULTING,Consulting());
            physicianRoles.Add(OPERATING, Operating());
            physicianRoles.Add(PRIMARYCARE, PrimaryCare());
            physicianRoles.Add(REFERRING, Referring());
            
            return physicianRoles;
        }
        public static ArrayList AllPhysicianRoles()
        {
            ArrayList al = new ArrayList( allPhysicianRoles().Values );
            al.Sort();
            return al;
        }

	    public static PhysicianRole Admitting()
		{
			return new AdmittingPhysician();
		}

		public static PhysicianRole Referring()
		{
			return new ReferringPhysician();
		}

		public static PhysicianRole Attending()
		{
			return new AttendingPhysician();
		}

		public static PhysicianRole Operating()
		{
			return new OperatingPhysician();
		}

		public static PhysicianRole Consulting()
		{
			return new ConsultingPhysician();
		}

		public static PhysicianRole PrimaryCare()
		{
			return new PrimaryCarePhysician();
		}

		public virtual void ValidateFor( Account selectedAccount, Physician physician )
		{		
		}

		public virtual bool IsValidFor( Account selectedAccount, Physician physician )
		{
			return true;
		}

		public virtual PhysicianRole Role()
		{
			return this;
		}

		#endregion

		#region Properties
		#endregion

		#region Private Methods
		#endregion

		#region Private Properties
		#endregion

		#region Construction and Finalization

        public PhysicianRole() { }

        public PhysicianRole( long oid, string description ) :
			base( oid, description ) {}
	
        public PhysicianRole( long oid, DateTime version ) :
            base( oid, version ) {}

        public PhysicianRole( long oid, DateTime version, string description ) :
            base( oid, version, description ) {}

        #endregion

		#region Data Elements
		#endregion

		#region Constants
        public static long
            ADMITTING	= 1L,
            REFERRING	= 2L,
            ATTENDING	= 3L,
            OPERATING	= 4L,
            CONSULTING	= 5L,
            PRIMARYCARE		= 6L;

	    public static string
	        ADMITTINGNAME = "Admitting",
	        REFERRINGNAME = "Referring",
	        ATTENDINGNAME = "Attending",
	        OPERATINGNAME = "Operating",
	        CONSULTINGNAME = "Consulting",
	        PRIMARYCARENAME     = "PrimaryCare";

	    public const string ACTIVE = "Active";
        public const string PRIMARYCAREPHYSICIAN_LABEL  = "PCP:";
        public const string OTHERPHYSICIAN_LABEL = "Oth:";
		#endregion


        #region IComparable Members

        public int CompareTo(object obj)
        {           
            if(obj is PhysicianRole) 
            {
                PhysicianRole role = (PhysicianRole) obj;

                return DisplayString.CompareTo(role.DisplayString);
            }
    
            throw new ArgumentException("object is not a PhysicianRole");                
        }

        #endregion
	}
}
