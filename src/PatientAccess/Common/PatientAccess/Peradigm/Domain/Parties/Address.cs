using System;
using System.Collections;
using System.Xml.Serialization;

namespace Peradigm.Framework.Domain.Parties
{
	/// <summary>
	/// An Address class that can be used to store simple or complex address 
	/// information, including an optional second street address and/or country
	/// information.  Very basic validation is currently performed on the information
	/// that is provided.
	/// This class inherits from Peradigm.Framework.Domain.Model and will throw
	/// Changed events when the mutators are invoked and values are changed.
	/// </summary>
    [Serializable]
    public class Address : Model, IContactPoint
    {
        #region Constants
        private const string
            SEPARATOR_SPACE = " ",
            SEPARATOR_NEW_LINE = "\n",
            COMMA = ",",
            DEFAULT_COUNTRY = "USA",
            PROPERTY_DELIVERY_ADDRESS = "DeliveryAddress",
            PROPERTY_SECONDARY_UNIT = "SecondaryUnit",
            PROPERTY_CITY = "City",
            PROPERTY_PROVINCE = "Province",
            PROPERTY_POSTAL_CODE = "PostalCode",
            PROPERTY_COUNTRY = "Country",
            FORMAT_HASH_IDENTITY = "{0}:{1}";
        #endregion

        #region Methods
        public string AsString()
        {
            return this.AsPostalLabelUsing( SEPARATOR_SPACE );
        }

        /// <summary>
        /// Return a formatted address string suitable for display in a multi-line
        /// display field.
        /// </summary>
        /// <returns>
        /// A string formatted to resemble a postal address label suitable for 
        /// multi-line display fields.
        /// </returns>
		public string AsPostalLabel()
		{
			return AsPostalLabelUsing( SEPARATOR_NEW_LINE );
		}

        /// <summary>
        /// Return a formatted address string suitable for display in a multi-line
        /// display field.
        /// </summary>
        /// <returns>
        /// A string formatted to resemble a postal address label suitable for 
        /// multi-line display fields.
        /// </returns>
        private string AsPostalLabelUsing( string Separator )
        {
            string street = this.DeliveryAddress;
            if( this.SecondaryUnit.Length > 0 )
            {
                street += Separator + this.SecondaryUnit;
            }
            
            string label = street + Separator + this.FormatCityStateZip();
            if( this.Country.Length > 0 )
            {
                label += Separator + this.Country;
            }
            return label.Trim();
        }

        public override int GetHashCode()
        {
            return String.Format( FORMAT_HASH_IDENTITY, 
                this.GetType().FullName,
                this.AsString() ).GetHashCode();
        }

        public override bool Equals( object obj )
        {
            IContactPoint contactPoint = obj as IContactPoint;
            if( contactPoint != null )
            {
                return this.Equals( contactPoint );
            }
            else
            {
                return base.Equals( obj );
            }
        }

        public virtual bool Equals( IContactPoint contactPoint )
        {
            if( contactPoint != null )
            {
                return
                    ( String.Compare( this.AsString(), contactPoint.AsString(), true ) == 0 );
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Return a string representation of an address instance for debugging purposes.
        /// </summary>
        /// <returns>
        /// A loosely formatted string.
        /// </returns>
		override public string ToString()
		{
            return this.AsString();
		}

        public bool Validate()
        {
            return !( this.AsString().Equals( String.Empty ) );
        }

        public void Parse( string text )
        {
            string[] parts = text.Split( new string[] { SEPARATOR_NEW_LINE, COMMA }, StringSplitOptions.RemoveEmptyEntries );
            parts = this.CleanUp( parts );
            switch( parts.Length )
            {
                case 6:
                    this.DeliveryAddress = parts[0];
                    this.SecondaryUnit = parts[1];
                    this.City = parts[2];
                    this.Province = parts[3];
                    this.PostalCode = parts[4];
                    this.Country = parts[5];
                    break;
                case 5:
                    if( String.Compare( parts[4], DEFAULT_COUNTRY, true ) == 0 )
                    {
                        this.DeliveryAddress = parts[0];
                        this.SecondaryUnit = String.Empty;
                        this.City = parts[1];
                        this.Province = parts[2];
                        this.PostalCode = parts[3];
                        this.Country = parts[4];
                    }
                    else
                    {
                        this.DeliveryAddress = parts[0];
                        this.SecondaryUnit = parts[1];
                        this.City = parts[2];
                        this.Province = parts[3];
                        this.PostalCode = parts[4];
                        this.Country = DEFAULT_COUNTRY;
                    }
                    break;
                case 4:
                    this.DeliveryAddress = parts[0];
                    this.SecondaryUnit = String.Empty;
                    this.City = parts[1];
                    this.Province = parts[2];
                    this.PostalCode = parts[3];
                    this.Country = DEFAULT_COUNTRY;
                    break;
                default:
                    this.Country = DEFAULT_COUNTRY;
                    break;
            }
            return;
        }
		#endregion

		#region Properties
        /// <summary>
        /// Street property of the address
        /// </summary>
        private string DeliveryAddress
        {
            get
            {
                return i_DeliveryAddress;
            }
            set
            {
                string oldValue = this.DeliveryAddress;
                i_DeliveryAddress = value;
                this.RaiseChangedEvent( PROPERTY_DELIVERY_ADDRESS, oldValue, this.DeliveryAddress );
            }
        }
	
        /// <summary>
        /// Street property of the address
        /// </summary>
        [Obsolete("User Delivery Address instead", false)]
        [XmlIgnore()]
		public string Street
		{
			get
			{
                return this.DeliveryAddress;
			}
			set
			{
                this.DeliveryAddress = value;
            }
		}

        /// <summary>
        /// Optional address information such as apartment number
        /// </summary>
        private string SecondaryUnit
        {
            get
            {
                return i_SecondaryUnit;
            }
            set
            {
                string oldValue = i_SecondaryUnit;
                i_SecondaryUnit = value;
                this.RaiseChangedEvent( PROPERTY_SECONDARY_UNIT, oldValue, this.SecondaryUnit );
            }
        }

        [Obsolete( "Use SecondaryUnit instead", false )]
        [XmlIgnore()]
        public string Street2
        {
            get
            {
                return this.SecondaryUnit;
            }
            set
            {
                this.SecondaryUnit = value;
            }
        }

        /// <summary>
        /// Name of the city.
        /// </summary>
        private string City
		{
			get
			{
				return i_City;
			}
			set
			{
                string oldValue = i_City;
                i_City = value;
                this.RaiseChangedEvent( PROPERTY_CITY, oldValue, this.City );
			}
		}

        /// <summary>
        /// String representing the Province.  May be either a 2 character state abbreviation
        /// or the full state name.
        /// </summary>
        private string Province
		{
			get
			{
				return i_Province;
			}
			set
			{
                string oldValue = i_Province;
                i_Province = value;
                this.RaiseChangedEvent( PROPERTY_PROVINCE, oldValue, this.Province );
			}
		}

        [Obsolete( "Use Province instead", false )]
        [XmlIgnore()]
        public string State
        {
            get
            {
                return this.Province;
            }
            set
            {
                this.Province = value;
            }
        }

        /// <summary>
        /// Postal code property for the address.  May be a standard US ZIP Code, a ZIP+4
        /// formatted ZIP code, or a standard UK Postal Code.
        /// </summary>
        private string PostalCode
		{
			get
			{
				return i_PostalCode;
			}
			set
			{
                string oldValue = i_PostalCode;
                i_PostalCode = value;
                this.RaiseChangedEvent( PROPERTY_POSTAL_CODE, oldValue, this.PostalCode );
			}
		}

        [Obsolete( "Use PostalCode instead", false )]
        [XmlIgnore()]
        public string ZIP
        {
            get
            {
                return this.PostalCode;
            }
            set
            {
                this.PostalCode = value;
            }
        }

        /// <summary>
        /// Name of the country for the specified address.
        /// </summary>
        private string Country
        {
            get
            {
                return i_Country;
            }
            set
            {
                string oldValue = i_Country;
                i_Country = value;
                this.RaiseChangedEvent( PROPERTY_COUNTRY, oldValue, this.Country );
            }
        }
        #endregion

        #region Private Methods
        private string[] CleanUp( string[] parts )
        {
            ArrayList partCollection = new ArrayList();
            foreach( string part in parts )
            {
                string newPart = part.Trim();
                if( newPart.Length > 0 )
                {
                    partCollection.Add( newPart );
                }
            }

            int counter = 0;
            string[] newParts = new string[partCollection.Count];
            foreach( string part in partCollection )
            {
                newParts[counter] = part;
                counter++;
            }

            return newParts;
        }

        private String FormatCityStateZip()
		{
			String formattedLabel = this.City;
			if( this.City.Trim().Length > 0 && this.Province.Trim().Length > 0 )
			{
				formattedLabel += COMMA + SEPARATOR_SPACE;
			}
			formattedLabel +=  (this.Province + SEPARATOR_SPACE + this.PostalCode);
			return formattedLabel;
		}
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        /// <summary>
        /// Build a new instance of the Address class with
        /// empty strings for the values.
        /// </summary>
        public Address()
            : this( String.Empty, String.Empty, String.Empty, String.Empty, String.Empty )
        {
        }

        public Address( string text )
        {
            this.Parse( text );
        }

        /// <summary>
        /// Build and initialize an instance of Address with the supplied parameters.
        /// </summary>
        /// <param name="StreetAddress">
        /// House or apartment number and street name.
        /// </param>
        /// <param name="CityName">
        /// Name of the city
        /// </param>
        /// <param name="StateAbbreviation">
        /// State or provence name or abbreviation
        /// </param>
        /// <param name="ZIPCode">
        /// ZIP or Postal Code string
        /// </param>
        public Address( string StreetAddress, 
                        string CityName, 
                        string StateAbbreviation, 
                        string ZIPCode )
            : this( StreetAddress, String.Empty, CityName, StateAbbreviation, ZIPCode, DEFAULT_COUNTRY )
		{
		}

        /// <summary>
        /// Build and initialize an instance of Address with the supplied parameters.
        /// </summary>
        /// <param name="StreetAddress">
        /// House or apartment number and street name.
        /// </param>
        /// <param name="CityName">
        /// Name of the city
        /// </param>
        /// <param name="StateAbbreviation">
        /// State or provence name or abbreviation
        /// </param>
        /// <param name="ZIPCode">
        /// ZIP or Postal Code string
        /// </param>
        /// <param name="Country">
        /// Country name
        /// </param>
        private Address( string StreetAddress, 
                        string CityName, 
                        string StateAbbreviation, 
                        string ZIPCode, 
                        string Country )
            : this( StreetAddress, String.Empty, CityName, StateAbbreviation, ZIPCode, Country )
		{
		}

        /// <summary>
        /// Build and initialize an instance of Address with the supplied parameters.
        /// </summary>
        /// <param name="StreetAddress1">
        /// House or apartment number and street name.
        /// </param>
		/// <param name="StreetAddress2">
		/// House or apartment number and street name.
		/// </param>
		/// <param name="CityName">
        /// Name of the city
        /// </param>
        /// <param name="StateAbbreviation">
        /// State or provence name or abbreviation
        /// </param>
        /// <param name="ZIPCode">
        /// ZIP or Postal Code string
        /// </param>
        /// <param name="Country">
        /// Country name
        /// </param>
        private Address( string StreetAddress1, 
                        string StreetAddress2, 
                        string CityName, 
                        string StateAbbreviation, 
                        string ZIPCode, 
                        string Country )
		{
            this.DeliveryAddress = StreetAddress1;
            this.SecondaryUnit = StreetAddress2;
            this.City = CityName;
            this.Province = StateAbbreviation;
            this.PostalCode = ZIPCode;
            this.Country = Country;
		}
        #endregion

        #region Data Elements
        private string i_DeliveryAddress;
        private string i_SecondaryUnit;
        private string i_City;
        private string i_Province;
        private string i_PostalCode;
        private string i_Country;

		#endregion
    }
}
