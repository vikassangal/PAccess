using System;
using System.Collections;

namespace PatientAccess.Domain 
{
    [Serializable]
    public class ZipCodeStatus : ReferenceValue 
    {
        #region Event Handlers
        #endregion

        #region Methods

        public static ICollection AllZipCodeStatuses()
        {
            ArrayList zipCodeStatues = new ArrayList();

            ZipCodeStatus zipCodeStatusHomeless = new ZipCodeStatus(
            (int)ZipCodeStatuses.Homeless, NEW_VERSION, ZipCodeStatuses.Homeless.ToString() );

            ZipCodeStatus zipCodeStatusKnown = new ZipCodeStatus(
                        (int)ZipCodeStatuses.Known, NEW_VERSION, ZipCodeStatuses.Known.ToString() );

            ZipCodeStatus zipCodeStatusUnknown = new ZipCodeStatus(
                       (int)ZipCodeStatuses.Unknown, NEW_VERSION, ZipCodeStatuses.Unknown.ToString() );

            zipCodeStatues.Add( zipCodeStatusHomeless );
            zipCodeStatues.Add( zipCodeStatusKnown );
            zipCodeStatues.Add( zipCodeStatusUnknown );

            return zipCodeStatues;
        }
        
        public override object Clone()
        {
            ZipCodeStatus newObject = new ZipCodeStatus();
            newObject.Description = (string) Description.Clone();
            newObject.Oid = Oid;
            return newObject;
        }

        public string GetZipCodeFor( string stateCode )
        {
           string zipCode = string.Empty;
          
            if( !string.IsNullOrEmpty( stateCode ) && stateCode == State.CALIFORNIA_CODE )
            {
                if ( Description == DESC_HOMELESS )
                {
                    zipCode = CONST_HOMELESS;
                }
                else if ( Description == DESC_UNKNOWN )
                {
                    zipCode = CONST_UNKNOWN;
                }
            }

            return zipCode;
        }

        public override string ToString()
        {
            if ( Description != null )
            {
                return String.Format( "{0}", Description.ToUpper() );
            }
            
            return string.Empty;
        }

        #endregion

        #region Properties
        public bool IsZipStatusUnknownOrHomeless
        {
            get
            {
                return ( Description == DESC_HOMELESS ||
                         Description == DESC_UNKNOWN );
            }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public ZipCodeStatus()
        {
        }
        public ZipCodeStatus( long oid, DateTime version, string description )
            : base( oid, version, description )
        {
        }
        #endregion

        #region Data Elements
        #endregion

		#region Constants
		public const string
            CONST_ZERO          = "0",
            CONST_UNKNOWN       = "XXXXX",
            CONST_HOMELESS      = "ZZZZZ",
            CONST_INTERNATIONAL = "YYYYY";

        public enum ZipCodeStatuses
        {            
            Known,
            Unknown,
            Homeless
        }

        public const string
            DESC_KNOWN          = "Known",
			DESC_UNKNOWN        = "Unknown",
            DESC_HOMELESS       = "Homeless";			
		#endregion
    }
}