using System;
using Peradigm.Framework.Domain.Time;

namespace Peradigm.Framework.Domain.Parties
{
	[Serializable]
	public class Facility : AbstractOrganizationalUnit 
	{
		#region Constants
        private const string 
            FACILITY_TYPE                       = "Facility",
            DFLT_TZ_NAME                        = "DefaultTimeZoneName",
            DFLT_TZ_OBSERVES_DAYLIGHT_SAVINGS   = "DefaultTimeZoneObservesDaylightSavings";
		#endregion

		#region Event Handlers
		#endregion

		#region Methods
        public OrganizationalUnit Division()
        {
            return this.RootOfType( OrganizationalUnitType.DIVISION );
        }

        public OrganizationalUnit Market()
        {
            return this.RootOfType( OrganizationalUnitType.MARKET );
        }

        public OrganizationalUnit Region()
        {
            return this.RootOfType( OrganizationalUnitType.REGION );
        }

        public override AbstractOrganizationalUnit UnitWith( string code )
        {
            if( this.Code == code )
            {
                return this;
            }
            else
            {
                return null;
            }
        }
        #endregion

		#region Properties
        [Obsolete("Refactor client code to use property Code instead of HspCode")]
		public string HspCode
		{
			get 
            { 
                return this.Code; 
            }
			set 
            { 
                this.Code = value;
            }
		}

        public override OrganizationalUnitType Type
        {
            set
            {
                //Cannot change the type for Facility objects.
            }
        }

	    private USTimeZone TimeZone
        {
            get
            {
                return i_TimeZone;
            }
            set
            {
                i_TimeZone = value;
            }
        }
        #endregion

		#region Private Methods
        private OrganizationalUnit RootOfType( string rootType )
        {
            OrganizationalUnit parent = this.Parent() as OrganizationalUnit;
            while( ( parent != null ) && !parent.IsOfType( rootType ) )
            {
                parent = parent.Parent() as OrganizationalUnit;
            }
            return parent;
        }
		#endregion

		#region Private Properties

	    private OrganizationalUnitType primType
		{
			get
			{
				return this.Type;
			}
			set
			{
				base.Type = value;
			}
		}
		#endregion

		#region Construction and Finalization
        public Facility()            
        {
            //Default Constructor allows class to be serialized
        }

        public Facility( string hspCode, string name )
            : this( Party.NEW_OID, Party.NEW_VERSION, hspCode, name )
        {
        }
        
        public Facility( string hspCode, string name, USTimeZone timeZone )
			: this( Party.NEW_OID, Party.NEW_VERSION, hspCode, name, timeZone )
		{
		}

        public Facility( long oid, string hspCode, string name )
            : this( oid, Party.NEW_VERSION, hspCode, name )
        {

        }

        public Facility( long oid, string hspCode, string name, USTimeZone timeZone )
            : this( oid, Party.NEW_VERSION, hspCode, name, timeZone )
        {
        }

        private Facility( long oid, Byte[] version, string hspCode, string name )
            : base( oid, version, hspCode, name )
        {
            this.primType = new OrganizationalUnitType( FACILITY_TYPE );
            
            try
            {
                this.TimeZone = USTimeZone.LocalTimeZone();   
            }
            catch
            {
                //swallow the exception and let the TimeZone attribute remain null
            }
        }

        private Facility( long oid, Byte[] version, string hspCode, string name, USTimeZone timeZone )
            : base( oid, version, hspCode, name )
        {
            this.primType = new OrganizationalUnitType( FACILITY_TYPE );
            this.TimeZone = timeZone;
        }
        #endregion

		#region Data Elements
        private USTimeZone i_TimeZone;
		#endregion
	}
}