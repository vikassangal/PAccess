using System;

namespace PatientAccess.Domain 
{
    [Serializable]
    public class Country : CodedReferenceValue , ICloneable
    {
        #region Event Handlers
        #endregion

        #region Methods
        public override string ToString()
        {
            if ( Description != null )
            {
                return String.Format( "{0}", Description.ToUpper() );
            }
            else
            {
                return string.Empty;
            }
        }

        public override object Clone()
        {
            Country newObject = new Country();
            newObject.Code = (string) this.Code.Clone();
            newObject.Description = (string) this.Description.Clone();
            newObject.Oid = this.Oid;
            return newObject;
        }

        public static Country NewUnitedStatesCountry()
        {
            return new Country( NEW_OID,
                NEW_VERSION,
                USA_DESC, USA_CODE);
        }

        public static bool IsTerritoryOfCountry( string territoryCode, string countryCode )
        {
            bool isTerritory = false;
            if ( countryCode == USA_CODE || countryCode == CFDB_USA_CODE )
            {
                isTerritory = IsUnitedStatesTerritory( territoryCode );
            }

            return isTerritory;
        }

        private static bool IsUnitedStatesTerritory( string territoryCode )
        {
            bool result = false;
            if( !string.IsNullOrEmpty( territoryCode ) )
            {
                if( territoryCode == CODE_AMERICAN_SOMOA
                    || territoryCode == CODE_GUAM
                    || territoryCode == CODE_MICRONESIA
                    || territoryCode == CODE_PUERTO_RICO
                    || territoryCode == CODE_VIRGIN_ISLANDS )
                {
                    result = true;
                }
            }
            return result;
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public Country()
        {
        }
        public Country( string code )
        {
            base.Code = code;
        }

        public Country( string code, string description ) : this( code )
        {
            base.Description = description;
        }

        public Country( long oid, DateTime version, string description )
            : base( oid, version, description )
        {
        }

        public Country( long oid, DateTime version, string description, string code )
            : base( oid, version, description, code )
        {
        }

        public Country( long oid, string description, string code )
            : base( oid, description, code )
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        //public const long USA_OID    = 559L;
        public const string USA_CODE            = "USA";
        public const string CFDB_USA_CODE       = "US";
        private const string USA_DESC = "UNITED STATES";
        public const string STATE_CALIFORNIA    = "CA";

        public const string CANADA_CODE = "CAN";
        public const string MEXICO_CODE = "MEX";

        //US Territories
        private const string
            CODE_AMERICAN_SOMOA     = "ASM",
            CODE_GUAM               = "GUM",
            CODE_MICRONESIA         = "FSM",
            CODE_PUERTO_RICO        = "PRI",
            CODE_VIRGIN_ISLANDS     = "VIR";
        #endregion
    }
}
