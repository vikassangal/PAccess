using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Extensions.PersistenceCommon;

namespace PatientAccess.Domain
{
    [Serializable]
    public class Address : PersistentModel, ICloneable
    {
        #region Event Handlers
        #endregion

        #region Methods

        public static bool IsUSState(State state)
        {
            var isUSState = USStateCodes.Contains(state.Code);

            return isUSState;
        }
       
        public string AsMailingLabelWithCountry()
        {
            string formattedString = String.Empty;
            string addrline1 = Address1;
            string addrline2 = Address2;
            string cityLine = String.Empty;
            string countyLine = String.Empty;
            string countryLine = String.Empty;
            isUnitedStatesAddress = this.IsUnitedStatesAddress();
            string formattedZipCode = this.FormattedZipCode;

            if (City == null || City.Trim().Length == 0)
            {
                if (State == null || State.ToString().Trim().Length == 0)
                {
                    cityLine = String.Format(STR_FORMAT_ADDRLINE1,
                        String.Empty,
                        String.Empty,
                        formattedZipCode);
                }
                else
                {
                    cityLine = String.Format(STR_FORMAT_ADDRLINE1,
                        String.Empty,
                        this.State,
                        formattedZipCode);
                }
            }
            else
            {
                if (State == null || State.ToString().Trim().Length == 0)
                {
                    cityLine = String.Format(STR_FORMAT_CITYLINE,
                        City,
                        String.Empty,
                        formattedZipCode);
                }
                else
                {
                    cityLine = String.Format(STR_FORMAT_CITYLINE,
                        City,
                        this.State,
                        formattedZipCode);
                }
            }

            if (County != null && County.Code != String.Empty)
            {
                countyLine = String.Format(STR_FORMAT_COUNTYLINEWITHCOUNTRY,   
                    Environment.NewLine, County.Code, County.Description);

            }
            if (Country != null && ! String.IsNullOrEmpty(Country.Code.Trim()))
            {   // Don't display country
                countryLine = String.Format(STR_FORMAT_COUNTRYLINE,  
                    Environment.NewLine, Country.Description);
            }
           
            if (!string.IsNullOrEmpty(addrline2))
            {
                if (String.IsNullOrEmpty(countyLine.Trim()))
                {
                    formattedString = String.Format(STR_FORMAT_ADDRLINE2WITHCOUNTRYNOCOUNTY,
                        addrline1,
                        Environment.NewLine,
                        addrline2,
                        cityLine,
                        countryLine);
                }
                else
                {
                    formattedString = String.Format(STR_FORMAT_ADDRLINE2WITHCOUNTRY,  
                        addrline1,
                        Environment.NewLine,
                        addrline2,
                        cityLine, 
                        countyLine, 
                        countryLine);
                }
            }
            else if (addrline1 != String.Empty || cityLine != String.Empty)
            {
                if (String.IsNullOrEmpty(countyLine.Trim()))
                {
                    formattedString =
                        String.Format(STR_FORMAT_ADDRLINE3WITHCOUNTRYNOCOUNTY,  
                            addrline1,
                            Environment.NewLine,
                            cityLine, countryLine);
                }
                else
                {
                    formattedString = String.Format(STR_FORMAT_ADDRLINE3WITHCOUNTRY,
                        addrline1,
                        Environment.NewLine,
                        cityLine, countyLine, countryLine);
                }
            }

          
            return formattedString;
        }
        public string AsMailingLabel()
        {
            string formattedString = String.Empty;
            string addrline1 = Address1;
            string addrline2 = Address2;
            string cityLine = String.Empty;
            isUnitedStatesAddress = this.IsUnitedStatesAddress();
            string formattedZipCode = this.FormattedZipCode;

            if( City == null || City.Trim().Length == 0 )
            {
                if( State == null || State.ToString().Trim().Length == 0 )
                {
                    cityLine = String.Format( STR_FORMAT_ADDRLINE1,
                        String.Empty,
                        String.Empty,
                        formattedZipCode );
                }
                else
                {
                    cityLine = String.Format( STR_FORMAT_ADDRLINE1,
                        String.Empty,
                        this.State,
                        formattedZipCode );
                }
            }
            else
            {
                if( State == null || State.ToString().Trim().Length == 0 )
                {
                    cityLine = String.Format( STR_FORMAT_CITYLINE,
                        City,
                        String.Empty,
                        formattedZipCode );
                }
                else
                {
                    cityLine = String.Format( STR_FORMAT_CITYLINE,
                        City,
                        this.State,
                        formattedZipCode );
                }
            }

            if( addrline2 != null && addrline2.Length > 0 )
            {
                if (Country == null || Country.Code.Equals(String.Empty) || 
                    Country.Code.Equals(Country.USA_CODE) || Country.Code.Equals(Country.CFDB_USA_CODE))
                {   // Don't display country
                    formattedString = String.Format( STR_FORMAT_ADDRLINE2,
                        addrline1,
                        Environment.NewLine,
                        addrline2,
                        cityLine );
                }
                else
                {
                    formattedString = String.Format( STR_FORMAT_ADDRLINE3,
                        addrline1,
                        Environment.NewLine,
                        addrline2,
                        cityLine,
                        this.Country );
                }
            }
            else if( addrline1 != String.Empty || cityLine != String.Empty )
            {
                if( Country == null || Country.Code.Equals( String.Empty ) ||
                    Country.Code.Equals( Country.USA_CODE ) || Country.Code.Equals(Country.CFDB_USA_CODE) )
                {
                    formattedString = String.Format( STR_FORMAT_ADDRLINE1,
                        addrline1,
                        Environment.NewLine,
                        cityLine );
                }
                else
                {
                    formattedString = String.Format( STR_FORMAT_ADDRLINE2,
                        addrline1,
                        Environment.NewLine,
                        cityLine,
                        this.Country );
                }
            }

            if ( County != null && County.Code != String.Empty )
            {
                formattedString = String.Format(STR_FORMAT_COUNTYLINE, formattedString, Environment.NewLine, Environment.NewLine, County.Code, County.Description);

            }
            return formattedString;
        }

        public string OneLineAddressLabelNoCountry()
        {
            string addressLabel = String.Empty;
            isUnitedStatesAddress = this.IsUnitedStatesAddress();
            string formattedZipCode = this.FormattedZipCode;

            ArrayList addressFieldsList = new ArrayList();
            if( this.Address1.Trim().Length > 0 )
            {
                addressFieldsList.Add( this.Address1.Trim() );
            }
            if( this.Address2.Trim().Length > 0 )
            {
                addressFieldsList.Add( this.Address2.Trim() );
            }
            if( this.City.Trim().Length > 0 )
            {
                addressFieldsList.Add( this.City.Trim() );
            }
            if( this.State != null && this.State.Code != null && this.State.Code.Trim().Length > 0 )
            {
                addressFieldsList.Add( this.State.Code.Trim() );
            }
            if( !formattedZipCode.Equals( "0" ) )
            {
                addressFieldsList.Add( formattedZipCode );
            }
            string[] arrayAddress = new string[addressFieldsList.Count];
            int i = 0;
            foreach( string addressField in addressFieldsList )
            {
                arrayAddress[i] = addressField;
                i++;
            }

            addressLabel = String.Join( ", ", arrayAddress );

            return addressLabel;
        }

        public string OneLineAddressLabel()
        {
            string addressLabel = String.Empty;
            isUnitedStatesAddress = this.IsUnitedStatesAddress();
            string formattedZipCode = this.FormattedZipCode;

            ArrayList addressFieldsList = new ArrayList();
            if( this.Address1.Trim().Length > 0 )
            {
                addressFieldsList.Add( this.Address1.Trim() );
            }
            if( this.Address2.Trim().Length > 0 )
            {
                addressFieldsList.Add( this.Address2.Trim() );
            }
            if( this.City.Trim().Length > 0 )
            {
                addressFieldsList.Add( this.City.Trim() );
            }
            if( this.State != null && this.State.Code != null && this.State.Code.Trim().Length > 0 )
            {
                addressFieldsList.Add( this.State.Code.Trim() );
            }
            if( !formattedZipCode.Equals( "0" ) 
                && this.ZipCode != null
                && !this.ZipCode.ZipAllZeros( isUnitedStatesAddress ) )
            {
                addressFieldsList.Add( formattedZipCode );
            }
            string[] arrayAddress = new string[addressFieldsList.Count];
            int i = 0;
            foreach( string addressField in addressFieldsList )
            {
                arrayAddress[i] = addressField;
                i++;
            }

            addressLabel = String.Join( ", ", arrayAddress );
            if( !this.isUnitedStatesAddress && this.Country != null &&
                this.Country.Description.Trim().Length > 0 )
            {
                if( addressLabel.Trim().Length > 0 )
                {
                    addressLabel = addressLabel + ", " +
                        this.Country.Description.Trim();
                }
                else
                {
                    addressLabel = this.Country.Description.Trim();
                }
            }

            return addressLabel;
        }

        public string OneLineAddressLabelWithoutStreet2()
        {
            string addressLabel = String.Empty;
            isUnitedStatesAddress = this.IsUnitedStatesAddress();
            string formattedZipCode = this.FormattedZipCode;

            ArrayList addressFieldsList = new ArrayList();
            if ( this.Address1.Trim().Length > 0 && this.Address1.Trim().Length < 26 )
            {
                addressFieldsList.Add(this.Address1.Trim());
            }
            else if (this.Address1.Trim().Length > 25)
            {
                addressFieldsList.Add(this.Address1.Trim().Substring(0,25));
            }
            if (this.City.Trim().Length > 0)
            {
                addressFieldsList.Add(this.City.Trim());
            }
            if (this.State != null && this.State.Code != null && this.State.Code.Trim().Length > 0)
            {
                addressFieldsList.Add(this.State.Code.Trim());
            }
            if (!formattedZipCode.Equals("0")
                && this.ZipCode != null
                && !this.ZipCode.ZipAllZeros(isUnitedStatesAddress))
            {
                addressFieldsList.Add(formattedZipCode);
            }
            string[] arrayAddress = new string[addressFieldsList.Count];
            int i = 0;
            foreach (string addressField in addressFieldsList)
            {
                arrayAddress[i] = addressField;
                i++;
            }

            addressLabel = String.Join(", ", arrayAddress);
            if (!this.isUnitedStatesAddress && this.Country != null &&
                this.Country.Description.Trim().Length > 0)
            {
                if (addressLabel.Trim().Length > 0)
                {
                    addressLabel = addressLabel + ", " +
                        this.Country.Description.Trim();
                }
                else
                {
                    addressLabel = this.Country.Description.Trim();
                }
            }

            return addressLabel;
        }

        public StringCollection GetMailingLabelStrings()
        {
            StringCollection addressLines = new StringCollection();

            string addrline1 = this.Address1;
            string addrline2 = this.Address2;
            addressLines.Add( addrline1 );

            if( addrline2 != null && addrline2.Length > 0 )
            {
                addressLines.Add( addrline2 );
            }

            if( this.City != String.Empty &&
                State.ToString() != String.Empty )
            {
                string cityLine = String.Format
                    ( STR_FORMAT_CITYLINE
                    , this.City
                    , this.State
                    , this.ZipCode.PostalCode
                    );
                addressLines.Add( cityLine );
            }
            return addressLines;
        }

        public override object DeepCopy()
        {
            BinaryFormatter formatter;
            MemoryStream stream = null;
            Address result = null;
            try
            {
                stream = new MemoryStream();
                formatter = new BinaryFormatter();
                formatter.Serialize( stream, this );
                stream.Position = 0;
                result = formatter.Deserialize( stream ) as Address;
            }
            finally
            {
                if( null != stream )
                {
                    stream.Flush();
                    stream.Close();
                }
            }
            return result;
        }

        public bool IsUnitedStatesAddress()
        {
            bool result = false;

            if( this.Country != null &&
                this.Country.Code != string.Empty )
            {
                result = this.Country.Code.Equals( Country.USA_CODE )
                         || this.Country.Code.Equals(Country.CFDB_USA_CODE) 
                         || Country.IsTerritoryOfCountry( Country.USA_CODE, this.Country.Code );
            }
            else if( this.State != null )
            {
                for( int i = 0; i < USStateCodes.Length; i++ )
                {
                    if( USStateCodes[i].Equals( this.State.Code ) )
                    {
                        result = true;
                        break;
                    }
                }
            }

            return result;
        }

        public override bool Equals( object obj )
        {
            Address address = obj as Address;

            if( address == null )
            {
                return false;
            }
            
            bool county = address.County == null && this.County == null ? true :
                address.County == null && this.County != null ? false :
                address.County != null && this.County == null ? false :
                address.County.Equals( this.County );

            bool country = address.Country == null && this.Country == null ? true :
                address.Country == null && this.Country != null ? false :
                address.Country != null && this.Country == null ? false :
                address.Country.Equals( this.Country );

            bool state = address.State == null && this.State == null ? true :
                address.State == null && this.State != null ? false :
                address.State != null && this.State == null ? false :
                address.State.Equals( this.State );

            bool zipCode = false;
            if( this.ZipCode != null )
            {
               zipCode = this.ZipCode.Equals( address.ZipCode );
            }
            else if( this.ZipCode == null && address.ZipCode == null )
            {
                zipCode = true;
            }
            

            return ( address.i_Address1 == this.i_Address1 &&
                    address.i_Address2 == this.i_Address2 &&
                    address.i_City == this.i_City &&
                    zipCode &&
                    state &&
                    country &&
                    county );
        }

        public override object Clone()
        {
            Address newObject = new Address();
            newObject.Address1 = (string)this.Address1.Clone();
            newObject.Address2 = (string)this.Address2.Clone();
            newObject.City = (string)this.City.Clone();
            if (this.ZipCode != null)
            {
                newObject.ZipCode = (ZipCode)this.ZipCode.Clone();
            }
            if( this.State != null )
            {
                newObject.State = (State)this.State.Clone();
            }
            if( this.Country != null )
            {
                newObject.Country = (Country)this.Country.Clone();
            }
            if (this.County != null)
            {
                newObject.County = (County)this.County.Clone();
            }
            return newObject;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

        #region Properties
        public string Address1
        {
            get
            {
                if( i_Address1 != null )
                {
                    return i_Address1.Trim();
                }
                else
                    return string.Empty;
            }
            set
            {
                i_Address1 = value;
            }
        }

        public string Address2
        {
            get
            {
                if( i_Address2 != null )
                {
                    return i_Address2.Trim();
                }
                else
                    return string.Empty;
            }
            set
            {
                i_Address2 = value;
            }
        }

        public string City
        {
            get
            {
                if( i_City != null )
                {
                    return i_City.Trim();
                }
                else
                    return string.Empty;
            }
            set
            {
                i_City = value;
            }
        }

        public ZipCode ZipCode
        {
            get
            {
                return i_ZipCode;
            }
            set
            {
                i_ZipCode = value;
            }
        }

        public State State
        {
            get
            {
                return i_State;
            }
            set
            {
                i_State = value;
            }
        }

        public Country Country
        {
            get
            {
                return i_Country;
            }
            set
            {
                i_Country = value;
            }
        }

        public County County
        {
            get
            {
                return i_County;
            }
            set
            {
                i_County = value;
            }
        }

        private string FormattedZipCode
        {
            get
            {
                string formattedZipCode = String.Empty;

                if ( this.ZipCode != null && this.ZipCode.PostalCode.Trim().Length != 0 )
                {
                    formattedZipCode = this.ZipCode.FormattedPostalCodeFor( isUnitedStatesAddress );
                }

                return formattedZipCode;
            }
        }

        public string FIPSCountyCode
        {
            get
            {
                string fipsCode = string.Empty;

                if ( IsUnitedStatesAddress() )
                {
                    if ( State != null && County != null )
                    {
                        if ( !string.IsNullOrEmpty( State.StateNumber ) && !string.IsNullOrEmpty( County.Code ) )
                        {
                            fipsCode = State.StateNumber + County.Code;
                        }
                    }
                }

                return fipsCode;
            }
        }

        #endregion

        #region Private Methods

        private bool isEmptyStateAndCountry()
        {
            return ( this.Country == null || this.Country.Code == string.Empty ||
                this.Country.Description == string.Empty || this.State == null );
        }

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public Address( long oid, string address1, string address2, string city, ZipCode zipCode,
            State state, Country country, County county )
            : base( oid )
        {
            i_Address1 = address1;
            i_Address2 = address2;
            i_City = city;
            i_State = state;
            i_Country = country;
            i_County = county;
            isUnitedStatesAddress = this.IsUnitedStatesAddress();

            i_ZipCode = zipCode;
            if ( i_ZipCode != null )
            {
                i_ZipCode.InitializeZipCode( isUnitedStatesAddress, isEmptyStateAndCountry() );
            }
        }

        public Address( string address1, string address2, string city, ZipCode zipCode,
            State state, Country country, County county )
            : this( NEW_OID, address1, address2, city, zipCode, state, country, county )
        {
        }

        public Address( string address1, string address2, string city, ZipCode zipCode, State state, Country country )
            : this( NEW_OID, address1, address2, city, zipCode, state, country, null )
        {
        }

        public Address()
            : this(
            NEW_OID, String.Empty, String.Empty, String.Empty, new ZipCode(),
            new State(), new UnknownCountry( String.Empty ), new County() )
        {
        }
        #endregion

        #region Data Elements
        private string i_Address1 = String.Empty;
        private string i_Address2 = String.Empty;
        private string i_City = String.Empty;
        private Country i_Country = new Country();
        private State i_State = new State();
        private County i_County = new County();
        private ZipCode i_ZipCode = new ZipCode();
        private StringBuilder RollingAddress = new StringBuilder();
        private bool isUnitedStatesAddress;
        private static string[] USStateCodes = new string[62] 
            { "AA", 
              "AE", 
              "AK", 
              "AL", 
              "AP",
              "AR",
              "AS",
              "AZ",
              "CA",
              "CO",
              "CT",
              "DC",
              "DE",
              "FL",
              "FM",
              "GA",
              "GU",
              "HI",
              "IA",
              "ID",
              "IL",
              "IN",
              "KS",
              "KY",
              "LA",
              "MA",
              "MD",
              "ME",
              "MH",
              "MI",
              "MN",
              "MO",
              "MP",
              "MS",
              "MT",
              "NC",
              "ND",
              "NE",
              "NH",
              "NJ",
              "NM",
              "NV",
              "NY",
              "OH",
              "OK",
              "OR",
              "PA",
              "PR",
              "PW",
              "RI",
              "SC",
              "SD",
              "TN",
              "TX",
              "UT",
              "VA",
              "VI",
              "VT",
              "WA",
              "WI",
              "WV",
              "WY"
            };

        #endregion

        #region Constants
        private const string STR_FORMAT_CITYLINE = "{0}, {1} {2}";
        private const string STR_FORMAT_ADDRLINE1 = "{0}{1}{2}";
        private const string STR_FORMAT_ADDRLINE2 = "{0}{1}{2}{1}{3}";
        private const string STR_FORMAT_ADDRLINE3 = "{0}{1}{2}{1}{3}{1}{4}";
        private const string STR_FORMAT_COUNTYLINE = "{0}{1}{2}County: {3} {4}";
        private const string STR_FORMAT_COUNTRYLINE = "{0}Country: {1}";
        private const string  STR_FORMAT_COUNTYLINEWITHCOUNTRY = "{0}County: {1} {2}";
        public const long FIPSCOUNTYCODE_LENGTH = 5;
        public const string PatientMailing = "Patient-Mailing";
        public const string PatientPhysical = "Patient-Physical";
        public const int ADDRESS1_LENGTH = 45;
        public const int ADDRESS2_LENGTH = 30;
        public const int ADDRESS_MAXIMUM_LENGTH = 75;
        private const string STR_FORMAT_ADDRLINE2WITHCOUNTRY = "{0}{1}{2}{1}{3}{1}{4}{5}";
        private const string STR_FORMAT_ADDRLINE3WITHCOUNTRY = "{0}{1}{2}{1}{3}{4}";
        private const string STR_FORMAT_ADDRLINE2WITHCOUNTRYNOCOUNTY = "{0}{1}{2}{1}{3}{1}{4}";
        private const string STR_FORMAT_ADDRLINE3WITHCOUNTRYNOCOUNTY = "{0}{1}{2}{1}{1}{3}";

        #endregion

        
    }
}
