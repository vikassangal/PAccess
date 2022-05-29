using System;
using Extensions.PersistenceCommon;

namespace PatientAccess.Domain
{
    [Serializable]
    public class ZipCode : PersistentModel, ICloneable
    {
        #region Event Handlers
        #endregion

        #region Methods

        public override object Clone()
        {
            ZipCode newObject = new ZipCode();
            newObject.PostalCode = (string)PostalCode.Clone();
            newObject.ZipCodeExtended = (string)ZipCodeExtended.Clone();
            newObject.ZipCodePrimary = (string)ZipCodePrimary.Clone();
            newObject.i_PostalCodeAsInt = PostalCodeAsInt;
            newObject.i_ZipCodePrimaryAsInt = ZipCodePrimaryAsInt;
            newObject.i_ZipCodeExtendedAsInt = ZipCodeExtendedAsInt;
            newObject.ZipCodeStatus = (ZipCodeStatus)ZipCodeStatus.Clone();
            return newObject;
        }

        public override bool Equals(object obj)
        {
            if( obj == null )
            {
                return false;
            }
            ZipCode zipCode = ( ZipCode )obj;

            return ( zipCode.i_PostalCode == i_PostalCode &&
                    zipCode.PostalCodeAsInt == PostalCodeAsInt &&
                    zipCode.ZipCodePrimary == ZipCodePrimary &&
                    zipCode.ZipCodeExtended == ZipCodeExtended );
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public string FormattedPostalCodeFor( bool isUnitedStatesAddress )
        {
            string postCode = PostalCode;

            if( Has9DigitZipCode() )
            {
                string primaryZipCodeExt = PostalCode.Substring( ZIPCODE_PRIMARY_LEN, ZIPCODE_EXTENDED_LEN );
                if( primaryZipCodeExt != DEFAULT_PBAR_EXTENDEDZIPCODE )
                {
                    if( isUnitedStatesAddress && HasUnFormattedZipCode() )
                        return String.Format(
                         "{0}-{1}",
                         PostalCode.Substring( 0, ZIPCODE_PRIMARY_LEN ),
                         PostalCode.Substring( ZIPCODE_PRIMARY_LEN, ZIPCODE_EXTENDED_LEN )
                         );
                    
                    return postCode;
                }
                
                return String.Format( "{0}", PostalCode.Substring( 0, ZIPCODE_PRIMARY_LEN ) );
            }
            
            return postCode;
        }

        public string PadZeroZipCodeExtended( string strZipExt )
        {
            string result = DEFAULT_PBAR_EXTENDEDZIPCODE;

            if( strZipExt != null )
            {
                if( strZipExt.Length < 4 )
                {
                    result = strZipExt.PadRight( 4, '0' );
                }
                else
                {
                    result = strZipExt;
                }
            }

            return result;
        }

        public bool ZipAllZeros( bool isUnitedStatesAddress )
        {
            bool allZeros = true;
            string zipStr = FormattedPostalCodeFor( isUnitedStatesAddress );
            char[] zipArray = zipStr.ToCharArray();
            foreach( char c in zipArray )
            {
                if( c != '0' )
                {
                    allZeros = false;
                    break;
                }
            }
            return allZeros;
        }

        /// <summary>
        /// Initializes zip code: First five chars of postal code
        /// and zip code Extended: Last four chars of postal code
        /// </summary>
        public void InitializeZipCode( bool isUnitedStatesAddress, bool isEmptyStateAndCountry )
        {
            if( string.IsNullOrEmpty(i_PostalCode) || 
                i_PostalCode == DEFAULT_PBAR_EXTENDEDZIPCODE || 
                i_PostalCode == DEFAULT_PBAR_POSTALCODE )
            {
                i_ZipCodePrimary = String.Empty;
                i_ZipCodeExtended = String.Empty;
                i_PostalCode = String.Empty;
                return;
            }

            if( isUnitedStatesAddress || isEmptyStateAndCountry )
            {
                string tempPostalCode = i_PostalCode;
                int dashIndex = tempPostalCode.IndexOf( "-" );
                if( dashIndex != -1 )
                {
                    i_PostalCode = tempPostalCode.Remove( dashIndex, 1 );
                }
            }

            if( i_PostalCode.Trim().Length > ZIPCODE_PRIMARY_LEN )
            {
                i_ZipCodePrimary = i_PostalCode.Substring( 0, ZIPCODE_PRIMARY_LEN );
                i_ZipCodeExtended = i_PostalCode.Substring(
                    ZIPCODE_PRIMARY_LEN, ( i_PostalCode.Length ) - ZIPCODE_PRIMARY_LEN );
            }

            else
            {
                i_ZipCodePrimary = i_PostalCode;
            }
           
            try
            {
                // Getting int value outside above if loop to make sure
                // non numeric value of postal code doesn't affect 
                // initializing values for zip and extended zip codes.
                if( i_PostalCode.Trim().Length > 0 )
                {
                    i_PostalCodeAsInt = Convert.ToInt32( i_PostalCode );
                    i_ZipCodePrimaryAsInt = Convert.ToInt32( i_ZipCodePrimary );
                    if( i_PostalCode.Trim().Length > ZIPCODE_PRIMARY_LEN )
                    {
                        i_ZipCodeExtendedAsInt = Convert.ToInt32( i_ZipCodeExtended );
                    }
                }
            }
            catch
            {
                // take default value i.e. 0
            }
        }

        public ZipCodeStatus GetZipCodeStatusFor( string stateCode )
        {
            ZipCodeStatus zipCodeStatus =  new ZipCodeStatus(); 

            if( !string.IsNullOrEmpty( stateCode ) && stateCode == State.CALIFORNIA_CODE )
            {
                if( !string.IsNullOrEmpty( ZipCodePrimary ) && 
                    ZipCodePrimary.Trim() == ZipCodeStatus.CONST_HOMELESS )
                {
                    zipCodeStatus = new ZipCodeStatus(
                        (int)ZipCodeStatus.ZipCodeStatuses.Homeless, NEW_VERSION, ZipCodeStatus.ZipCodeStatuses.Homeless.ToString() ); 
                }
                else if( !string.IsNullOrEmpty( ZipCodePrimary ) && 
                         ZipCodePrimary.Trim() == ZipCodeStatus.CONST_UNKNOWN )
                {
                    zipCodeStatus = new ZipCodeStatus(
                        (int)ZipCodeStatus.ZipCodeStatuses.Unknown, NEW_VERSION, ZipCodeStatus.ZipCodeStatuses.Unknown.ToString() ); 
                }
                else if( !string.IsNullOrEmpty( ZipCodePrimary ) && 
                         ZipCodePrimary.Trim() == ZipCodeStatus.CONST_INTERNATIONAL )
                {
                    zipCodeStatus = new ZipCodeStatus(); 
                }
                else
                {
                    zipCodeStatus = new ZipCodeStatus(
                        (int)ZipCodeStatus.ZipCodeStatuses.Known, NEW_VERSION, ZipCodeStatus.ZipCodeStatuses.Known.ToString() );
                }
            }
            
            return zipCodeStatus;
        }

        private void SetZipCodeStatus()
        {
            if( !string.IsNullOrEmpty( PostalCode ) && PostalCode.Trim() == ZipCodeStatus.CONST_HOMELESS )
            {
                ZipCodeStatus = new ZipCodeStatus(
                    3, NEW_VERSION, ZipCodeStatus.DESC_HOMELESS ); 
            }
            else if( !string.IsNullOrEmpty( PostalCode ) && PostalCode.Trim() == ZipCodeStatus.CONST_UNKNOWN )
            {
                ZipCodeStatus = new ZipCodeStatus(
                    2, NEW_VERSION, ZipCodeStatus.DESC_UNKNOWN ); 
            }
            else if( ( !string.IsNullOrEmpty( PostalCode ) && PostalCode.Trim() == ZipCodeStatus.CONST_INTERNATIONAL ) ||
                     ( !string.IsNullOrEmpty( PostalCode ) && PostalCode.Trim() == ZipCodeStatus.CONST_ZERO ) )
            {
                ZipCodeStatus = new ZipCodeStatus();
            }
            else
            {
                ZipCodeStatus = new ZipCodeStatus(
                    1, NEW_VERSION, ZipCodeStatus.DESC_KNOWN);
            }
        }

        #endregion

        #region Properties
        /// <summary>
        /// Postal Code = Primary Zip Code + Extended Zip Code
        /// </summary>
        public string PostalCode
        {
            get
            {
                if( i_PostalCode == null || i_PostalCode == DEFAULT_PBAR_POSTALCODE 
                    || i_PostalCode == DEFAULT_PBAR_EXTENDEDZIPCODE )
                {
                    return string.Empty;
                }
                
                return i_PostalCode.Trim();
            }
            set
            {
                i_PostalCode = value;
                SetZipCodeStatus();
            }
        }

        public int PostalCodeAsInt
        {
            get
            {
                try
                {
                    if( i_PostalCode.Trim().Length > 0 )
                    {
                        i_PostalCodeAsInt = Convert.ToInt32( i_PostalCode );
                    }
                }
                catch
                {
                    // take default value i.e. 0
                }

                return i_PostalCodeAsInt;
            }
        }

        public string ZipCodePrimary
        {
            get
            {
                if( i_PostalCode.Trim().Length > ZIPCODE_PRIMARY_LEN )
                {
                    i_ZipCodePrimary = i_PostalCode.Substring( 0, ZIPCODE_PRIMARY_LEN );
                }
                else
                {
                    i_ZipCodePrimary = i_PostalCode;
                }
                
                if( i_ZipCodePrimary != DEFAULT_PBAR_POSTALCODE 
                    || i_ZipCodePrimary != DEFAULT_PBAR_EXTENDEDZIPCODE )
                {
                    return i_ZipCodePrimary;
                }
                
                return string.Empty;
            }
            set
            {
                i_ZipCodePrimary = value;
                PostalCode = i_ZipCodePrimary + ZipCodeExtended;
            }
        }

        public int ZipCodePrimaryAsInt
        {
            get
            {
                try
                {
                    if( i_PostalCode.Trim().Length > 0 )
                    {
                        i_ZipCodePrimaryAsInt = Convert.ToInt32( i_ZipCodePrimary );
                    }
                }
                catch
                {
                    // take default value i.e. 0
                }

                return i_ZipCodePrimaryAsInt;
            }
        }

        public string ZipCodeExtended
        {
            get
            {
                if( i_PostalCode.Trim().Length > ZIPCODE_PRIMARY_LEN )
                {
                    i_ZipCodeExtended = i_PostalCode.Substring(
                        ZIPCODE_PRIMARY_LEN, ( i_PostalCode.Length ) - ZIPCODE_PRIMARY_LEN );
                }
                
                if( i_ZipCodeExtended != DEFAULT_PBAR_EXTENDEDZIPCODE && i_ZipCodeExtended != "0" )
                {
                    return i_ZipCodeExtended;
                }
                
                return string.Empty;
            }
            set
            {
                i_ZipCodeExtended = value;
                PostalCode = ZipCodePrimary + i_ZipCodeExtended;
            }
        }

        public string ZipCodeExtendedZeroPadded
        {
            get
            {
                return PadZeroZipCodeExtended( i_ZipCodeExtended );
            }
        }

        public int ZipCodeExtendedAsInt
        {
            get
            {
                try
                {
                    if ( i_PostalCode.Trim().Length > ZIPCODE_PRIMARY_LEN )
                    {
                        i_ZipCodeExtendedAsInt = Convert.ToInt32( i_ZipCodeExtended );
                    }
                }
                catch
                {
                    // take default value i.e. 0
                }

                return i_ZipCodeExtendedAsInt;
            }
        }

        public ZipCodeStatus ZipCodeStatus
        {
            private get
            {
                return i_ZipCodeStatus;
            }
            set
            {
                i_ZipCodeStatus = value;
            }
        }
        #endregion

        #region Private Methods

        private bool Has9DigitZipCode()
        {
            return PostalCode.Length == ZIPCODE_PRIMARY_LEN + ZIPCODE_EXTENDED_LEN;
        }

        private bool HasUnFormattedZipCode()
        {
            return PostalCode.IndexOf( "-" ) == -1;
        }
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public ZipCode()
            : this( String.Empty )
        {
        }

        public ZipCode( string postalCode )
        {
            PostalCode = postalCode;
        }
        #endregion

        #region Data Elements
        private string i_PostalCode = String.Empty;
        private string i_ZipCodePrimary = String.Empty;
        private string i_ZipCodeExtended = DEFAULT_PBAR_EXTENDEDZIPCODE;
        private int i_PostalCodeAsInt;
        private int i_ZipCodePrimaryAsInt;
        private int i_ZipCodeExtendedAsInt;
        private ZipCodeStatus i_ZipCodeStatus = new ZipCodeStatus();
        #endregion

        #region Constants
        private const int ZIPCODE_PRIMARY_LEN = 5;
        private const int ZIPCODE_EXTENDED_LEN = 4;
        private const string DEFAULT_PBAR_POSTALCODE = "000000000";
        private const string DEFAULT_PBAR_EXTENDEDZIPCODE = "0000";
        #endregion
    }
}
