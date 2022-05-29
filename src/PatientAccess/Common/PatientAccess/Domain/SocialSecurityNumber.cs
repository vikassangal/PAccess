using System;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using Extensions.PersistenceCommon;

namespace PatientAccess.Domain
{
    [Serializable]
    public class SocialSecurityNumber : PersistentModel
    {
        #region Event Handlers
        #endregion

        #region Methods
        /// <summary>
        /// I will format a generic number with the familiar dashes found
        /// in social security numbers, for example I would return 999999999 
        /// as 999-99-9999.
        /// </summary>
        /// <returns>A 3x2x4 formatted social security number.</returns>
        public string AsFormattedString()
        {
            return String.Format( "{0}-{1}-{2}", AreaNumber, GroupNumber, Series );
        }

        public string AsFormattedMaskedString()
        {
            return String.Format( "{0}-{1}-{2}", "###", "##", Series );
        }

        public override string ToString()
        {
            return UnformattedSocialSecurityNumber;
        }

        public static bool operator ==( SocialSecurityNumber lOper, SocialSecurityNumber rOper )
        {
            return lOper.Equals( rOper );
        }

        public static bool operator !=( SocialSecurityNumber lOper, SocialSecurityNumber rOper )
        {
            return !lOper.Equals( rOper );
        }
        
        public static bool IsFloridaNewbornOldSSN( string ssn )
        {
            return ( ssn == FLORIDA_NEWBORN_SSN );
        }

        public static bool IsFloridaNoneOldSSN( string ssn )
        {
            return ( ssn == FLORIDA_NONE_SSN );
        }

        public static bool IsFloridaUnknownSSNPre01012010( string ssn )
        {
            return ( ssn == FLORIDA_UNKNOWN_PRE_01012010_SSN );
        }

        public static bool IsFloridaUnknownNoneNewbornSSNPost01012010( string ssn )
        {
            return ( ssn == FLORIDA_UNKNOWN_NONE_NEWBORN_POST_01012010_SSN );
        }

        public static bool IsNonFloridaNoneSSN( string ssn )
        {
            return ( ssn == NON_FLORIDA_NONE_SSN );
        }

        public static bool IsNonFloridaNewbornUnknownSSN( string ssn )
        {
            return ( ssn == NON_FLORIDA_NEWBORN_OR_UNKNOWN_SSN );
        }
        public static bool IsBaylorRefusedSSN( string ssn )
        {
            return ( ssn == BAYLOR_REFUSED_SSN );
        }
        public static bool IsBaylorNoneNewBornSSN( string ssn )
        {
            return ( ssn == BAYLOR_NONE_NEWBORN_SSN );
        }
        public static bool IsBaylorUnknownSSN( string ssn )
        {
            return ( ssn == BAYLOR_UNKNOWN_SSN );
        }
        public static bool IsSouthCarolinaNewBorn(string ssn)
        {
            return (ssn == SOUTHCAROLINA_NEWBORN_SSN);
        }
        public static bool IsSouthCarolinaUnknown(string ssn)
        {
            return (ssn == SOUTHCAROLINA_UNKNOWN_SSN);
        }
        public static bool IsSouthCarolinaOldUnknown(string ssn)
        {
            return (ssn == SOUTHCAROLINA_OLDUNKNOWN_SSN);
        }
        public static bool IsSouthCarolinaNone(string ssn)
        {
            return (ssn == SOUTHCAROLINA_NONE_REFUSED_SSN);
        }
        public static bool IsSouthCarolinaRefused(string ssn)
        {
            return (ssn == SOUTHCAROLINA_NONE_REFUSED_SSN);
        }
        public override object DeepCopy()
        {
            BinaryFormatter formatter;
            MemoryStream stream = null;
            SocialSecurityNumber result;

            try
            {
                stream = new MemoryStream();
                formatter = new BinaryFormatter();
                formatter.Serialize( stream, this );
                stream.Position = 0;
                result = formatter.Deserialize( stream ) as SocialSecurityNumber;
            }
            finally
            {
                if ( null != stream )
                {
                    stream.Flush();
                    stream.Close();
                }
            }

            return result;
        }

        public override bool Equals( object aSsn )
        {
            if ( aSsn == null )
            {
                return false;
            }

            SocialSecurityNumber ssn = (SocialSecurityNumber)aSsn;

            string ssnString = ssn.UnformattedSocialSecurityNumber;

            return UnformattedSocialSecurityNumber == ssnString;
        }

        public bool Equals( string aSsn )
        {
            bool result;
            if ( aSsn.Length == 11 )
            {
                result = AsFormattedString() == aSsn;
            }

            else
            {
                result = UnformattedSocialSecurityNumber == aSsn;
            }

            return result;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public bool IsDefaultSsn()
        {
            return !SSNStatus.IsKnownSSNStatus && DEFAULT_SSN_NUMBERS.Contains( ToString() );
        }

        #endregion

        #region Properties

        public string UnformattedSocialSecurityNumber
        {
            get
            {
                return AreaNumber + GroupNumber + Series;
            }
        }

        public override string DisplayString
        {
            get
            {
                return i_AreaNumber + "-" + i_GroupNumber + "-" + i_Series;
            }
        }

        public override string PrintString
        {
            get
            {
                return i_AreaNumber + "-" + i_GroupNumber + "-" + i_Series;
            }
        }

        /// <summary>
        /// The first three digits of the SSN are the area number. 
        /// For numbers assigned prior to 1973, it indicates the specific 
        /// Social Security office from which the card was issued. 
        /// Since 1973, certain blocks of numbers have been allocated to each State. 
        /// The area number indicates the State the number holder showed as 
        /// his/her mailing address on the application for a number. 
        /// The State is derived from the ZIP code in the mailing address.
        /// </summary>
        public string AreaNumber
        {
            get
            {
                return i_AreaNumber;
            }
        }

        /// <summary>
        /// The middle two digits are the group number and have no geographical significance. 
        /// They just break the SSN into conveniently sized blocks for use in 
        /// internal operations and order of issuance.
        /// </summary>
        public string GroupNumber
        {
            get
            {
                return i_GroupNumber;
            }
        }

        /// <summary>
        /// The last four digits are the serial number representing a straight 
        /// numerical series of numbers from 0001-9999 within each group.
        /// </summary>
        public string Series
        {
            get
            {
                return i_Series;
            }
        }

        public bool IsComplete
        {
            get
            {
                return UnformattedSocialSecurityNumber.Trim().Length == MAX_VALUE.Length;
            }
        }

        public bool IsPartialSSN
        {
            get
            {
                return i_IsPartialSSN;
            }
            private set
            {
                i_IsPartialSSN = value;
            }
        }

        public SocialSecurityNumberStatus SSNStatus
        {
            get
            {
                return i_SSNStatus;
            }
            set
            {
                i_SSNStatus = value;
            }
        }

        public static SocialSecurityNumber NonFloridaNewbornOrUnknownSSN
        {
            get { return new SocialSecurityNumber( NON_FLORIDA_NEWBORN_OR_UNKNOWN_SSN ); }
        }

        public static SocialSecurityNumber NonFloridaNoneSSN
        {
            get { return new SocialSecurityNumber( NON_FLORIDA_NONE_SSN ); }
        }

        public static SocialSecurityNumber FloridaNoneSSN
        {
            get { return new SocialSecurityNumber( FLORIDA_NONE_SSN ); }
        }

        public static SocialSecurityNumber FloridaNewbornSSN
        {
            get { return new SocialSecurityNumber( FLORIDA_NEWBORN_SSN ); }
        }

        public static SocialSecurityNumber FloridaUnknownPre01012010SSN
        {
            get { return new SocialSecurityNumber( FLORIDA_UNKNOWN_PRE_01012010_SSN ); }
        }

        public static SocialSecurityNumber FloridaUnknownNoneNewbornPost01012010SSN
        {
            get { return new SocialSecurityNumber( FLORIDA_UNKNOWN_NONE_NEWBORN_POST_01012010_SSN ); }
        }
        public static SocialSecurityNumber BaylorUnknownSSN
        {
            get { return new SocialSecurityNumber( BAYLOR_UNKNOWN_SSN ); }
        }
        public static SocialSecurityNumber BaylorNoneNewbornSSN
        {
            get { return new SocialSecurityNumber( BAYLOR_NONE_NEWBORN_SSN ); }
        }
        public static SocialSecurityNumber BaylorRefusedSSN
        {
            get { return new SocialSecurityNumber( BAYLOR_REFUSED_SSN ); }
        }
        public static SocialSecurityNumber SouthCarolinaUnknownSSN
        {
            get { return new SocialSecurityNumber(SOUTHCAROLINA_UNKNOWN_SSN); }
        }
        public static SocialSecurityNumber SouthCarolinaOldUnknownSSN
        {
            get { return new SocialSecurityNumber(SOUTHCAROLINA_OLDUNKNOWN_SSN); }
        }
        public static SocialSecurityNumber SouthCarolinaNewbornSSN
        {
            get { return new SocialSecurityNumber(SOUTHCAROLINA_NEWBORN_SSN); }
        }
        public static SocialSecurityNumber SouthCarolinaNoneRefusedSSN
        {
            get { return new SocialSecurityNumber(SOUTHCAROLINA_NONE_REFUSED_SSN); }
        }

        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public SocialSecurityNumber( string unformattedSsn )
        {
            try
            {
                // A null passed to the constructor most likely indicates
                // deeper problems and should be investigated
                if ( unformattedSsn == null )
                {
                    throw new ApplicationException( "SSN Can not be null" );
                }

                // Remove all non-alphanumeric characters. This allows us to still
                // treat values with non-standard delimiters (space, underscore, etc)
                // as potential candidates for a good SSN
                unformattedSsn =
                    Regex.Replace( unformattedSsn,
                                   @"[^0-9A-Za-z]",
                                   String.Empty );

                // Regular expression essentially reads: If the string is exactly nine
                // contiguous digits, then match
                if ( Regex.IsMatch( unformattedSsn, @"^\d{9}$" ) )
                {
                    i_AreaNumber =
                        unformattedSsn.Substring( AREA_NUMBER_INDEX,
                                                  AREA_NUMBER_LENGTH );
                    i_GroupNumber =
                        unformattedSsn.Substring( GROUP_NUMBER_INDEX,
                                                  GROUP_NUMBER_LENGTH );
                    i_Series =
                        unformattedSsn.Substring( SERIES_INDEX,
                                                  SERIES_LENGTH );
                }
                else if ( unformattedSsn != String.Empty )
                {
                    IsPartialSSN = true;
                }

                SSNStatus = SSNFactory.GetGeneralSSNStatusFor( UnformattedSocialSecurityNumber );
            }
            catch ( Exception anyException )
            {
                throw new ApplicationException( "Invalid SSN: " + unformattedSsn,
                                                anyException );
            }
        }

        public SocialSecurityNumber()
        {
        }

        #endregion

        #region Data Elements

        private readonly string i_AreaNumber = String.Empty;
        private readonly string i_GroupNumber = String.Empty;
        private readonly string i_Series = String.Empty;
        private bool i_IsPartialSSN;
        private SocialSecurityNumberStatus i_SSNStatus = new SocialSecurityNumberStatus();

        #endregion

        #region Constants
        private const int AREA_NUMBER_LENGTH = 3;
        private const int AREA_NUMBER_INDEX = 0;
        private const int GROUP_NUMBER_LENGTH = 2;
        private const int GROUP_NUMBER_INDEX = 3;
        private const int SERIES_LENGTH = 4;
        private const int SERIES_INDEX = 5;

        public const string MAX_VALUE = "999999999";
        public static string[] DEFAULT_SSN_NUMBERS = new[] { "000000001", "555555555", "777777777", "999999999", "000000000", "888888888", "000000002", "000000009" };

        public const string
            FLORIDA_NEWBORN_SSN = "000000000",
            FLORIDA_NONE_SSN = "555555555",
            FLORIDA_UNKNOWN_PRE_01012010_SSN = "777777777",
            FLORIDA_UNKNOWN_NONE_NEWBORN_POST_01012010_SSN = "777777777",

            NON_FLORIDA_NONE_SSN = "000000001";

        private const string
            NON_FLORIDA_NEWBORN_OR_UNKNOWN_SSN = "999999999";

        public const string
            BAYLOR_NONE_NEWBORN_SSN = "000000000",
            BAYLOR_REFUSED_SSN = "888888888",
            BAYLOR_UNKNOWN_SSN = "999999999";

        public const string
            SOUTHCAROLINA_NEWBORN_SSN = "000000002",
            SOUTHCAROLINA_NONE_REFUSED_SSN = "000000009",
            SOUTHCAROLINA_UNKNOWN_SSN = "000000001",
            SOUTHCAROLINA_OLDUNKNOWN_SSN = "999999999";

        #endregion
    }
}
