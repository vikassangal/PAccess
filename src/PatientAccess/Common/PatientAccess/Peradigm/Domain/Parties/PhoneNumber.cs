using System;
using System.Collections;
using System.Xml.Serialization;
using Peradigm.Framework.Domain.Parties.Exceptions;

namespace Peradigm.Framework.Domain.Parties
{
    /// <summary>
    /// A class that describes a phone number including international formats.
    /// Although most countries use formats that differ from the US format, 
    /// the number of digits remains the same and international formats such
    /// as French and UK can be parsed and stored in the canonical format(US).
    /// </summary>
    [Serializable]
    public class PhoneNumber : Model, IContactPoint
    {
        #region Constants
        private const string
            FORMAT_HASH_IDENTITY = "{0}:{1}";
        #endregion

        #region Methods
        /// <summary>
        /// Answer a default string representation of the phone number.
        /// </summary>
        /// <returns>
        /// A string formatted as internation phone number.
        /// </returns>
        public string AsString()
        {
            return this.AsInternationalString();
        }

        /// <summary>
        /// Answer an unformatted string representation of the area code, exchange and line number.
        /// </summary>
        /// <returns>
        /// A string formatted as nnnnnnnnnn.
        /// </returns>
        public virtual string AsUnformattedString()
        {
            return this.FormattedAreaCode + 
                   this.FormattedExchange + 
                   this.FormattedLineNumber;
        }

        /// <summary>
        /// Return a string representation of the phone in US format.
        /// </summary>
        /// <returns>
        /// A string formatted as (nnn)nnn-nnnn.
        /// </returns>
        public virtual string AsUSString()
        {
            return "(" + this.FormattedAreaCode + ")" +
                   this.FormattedExchange + "-" +
                   this.FormattedLineNumber;
        }

        /// <summary>
        /// Return a string representation of the phone number in
        /// international format with '.' as the component separator.
        /// Extension is optionally included in the formatted string if
        /// an extension has been set.
        /// </summary>
        /// <returns>
        /// A string formatted as +nnn.nnn.nnn.nnnn[xn]
        /// </returns>
        public virtual string AsInternationalString()
        {
            return this.AsInternationalStringUsing( '.' );
        }

        /// <summary>
        /// Return a string representation of the phone number in
        /// international format with the supplied separator.
        /// Extension is optionally included in the formatted string if
        /// an extension has been set.
        /// </summary>
        /// <returns>
        /// A string formatted as +nnn.nnn.nnn.nnnn[xn]
        /// </returns>
        public virtual string AsInternationalStringUsing( char Delimiter )
        {
            string number = "+" + this.CountryCode + Delimiter +
                            this.AreaCode + Delimiter +
                            this.Exchange + Delimiter +
                            this.LineNumber;
            if( this.Extension > 0 )
            {
                number += "x" + this.Extension;
            }
            return number;
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

        virtual public void Parse( string text )
        {
            try
            {
                string[] parts = text.Split( new char[] { ' ', '.', '(', ')', '-', 'x' } );
                parts = this.CleanUp( parts );
                switch( parts.Length )
                {
                    case 1:
                        if( parts[0].Length > 0 )
                        {
                            this.AreaCode = int.Parse( parts[0].Substring( 0, 3 ) );
                            this.Exchange = int.Parse( parts[0].Substring( 3, 3 ) );
                            this.LineNumber = int.Parse( parts[0].Substring( 6, 4 ) );
                        }
                        else
                        {
                            this.AreaCode = 0;
                            this.Exchange = 0;
                            this.LineNumber = 0;
                        }
                        this.CountryCode = 1;
                        break;
                    case 3:
                        this.CountryCode = 1;
                        this.AreaCode = int.Parse( parts[0] );
                        this.Exchange = int.Parse( parts[1] );
                        this.LineNumber = int.Parse( parts[2] );
                        break;
                    case 4:
                        if( parts[0].Length == 0 )
                        {
                            parts[0] = "1";
                        }
                        this.CountryCode = int.Parse( parts[0] );
                        this.AreaCode = int.Parse( parts[1] );
                        this.Exchange = int.Parse( parts[2] );
                        this.LineNumber = int.Parse( parts[3] );
                        break;
                    case 5:
                        this.CountryCode = int.Parse( parts[0] );
                        this.AreaCode = int.Parse( parts[1] );
                        this.Exchange = int.Parse( parts[2] );
                        this.LineNumber = int.Parse( parts[3] );
                        this.Extension = int.Parse( parts[4] );
                        break;
                    default:
                        throw new UnrecognizedPhoneNumberException( text );
                }
            }
            catch
            {
                this.CountryCode = 0;
                this.AreaCode = 0;
                this.Exchange = 0;
                this.Extension = 0;
                this.LineNumber = 0;
                throw new UnrecognizedPhoneNumberException( text );
            }
            return;
        }

        public virtual PhoneNumber SetPhoneNumber( string phone )
        {
            try
            {
                this.Parse( phone );
                return this;
            }
            catch( UnrecognizedPhoneNumberException )
            {
                throw;
            }
            catch( Exception )
            {
                throw new UnrecognizedPhoneNumberException( phone );
            }
        }

        public bool Validate()
        {
            return
                ( this.CountryCode > 0 &&
                  this.AreaCode > 0 &&
                  this.Exchange >= 0 &&
                  this.Extension >= 0 );
        }
        #endregion

        #region Properties
        /// <summary>
        /// Get or set the Area Code, [(NNN)]nnn-nnnn, for the phone number.  
        /// Typically a 3 digit numeral.
        /// </summary>
        private int AreaCode
        {
            get
            {
                return i_AreaCode;
            }
            set
            {
                int OldValue = i_AreaCode;
                i_AreaCode = value;
                this.RaiseChangedEvent( "AreaCode", OldValue, this.AreaCode );
            }
        }

		/// <summary>
		/// Get the Area Code formatted as string
		/// Typically a 3 digit numeral.
		/// </summary>
		[XmlIgnore]
		private string FormattedAreaCode
		{
			get 
			{
				string formattedString = "";
				string orginal = this.i_AreaCode.ToString();
				formattedString = orginal.PadLeft( 3, '0' );
				return formattedString; 
			}
		}

        /// <summary>
        /// Get or set the Country Code, [NNN](nnn)nnn-nnnn, for the phone number.  
        /// Typically a 1 to 3 digit numeral.
        /// </summary>
        private int CountryCode
        {
            get
            {
                return i_CountryCode;
            }
            set
            {
                int OldValue = i_CountryCode;
                i_CountryCode = value;
                this.RaiseChangedEvent( "CountryCode", OldValue, this.CountryCode );
            }
        }

        /// <summary>
        /// Get or set the Exchange, (nnn)[NNN]-nnnn, for the phone number.  Typically a 3 digit numeral.
        /// </summary>
        private int Exchange
        {
            get
            {
                return i_Exchange;
            }
            set
            {
                int OldValue = i_Exchange;
                i_Exchange = value;
                this.RaiseChangedEvent( "Exchange", OldValue, this.Exchange );
            }
        }

		/// <summary>
		/// Get formatted Exchange as string, (nnn)[NNN]-nnnn, for the phone number.  Typically a 3 digit numeral.
		/// </summary>
		[XmlIgnore]
		private string FormattedExchange
		{
			get 
			{
				string formattedString = "";
				string orginal = this.i_Exchange.ToString();
				formattedString = orginal.PadLeft( 3, '0' );			
				return formattedString;
			}
		}


        /// <summary>
        /// Get or set the optional extension for the phone number.  
        /// Typically a 1 to 5 digit numeral.
        /// </summary>
        private int Extension
        {
            get
            {
                return i_Extension;
            }
            set
            {
                int OldValue = i_Extension;
                i_Extension = value;
                this.RaiseChangedEvent( "Extension", OldValue, this.Extension );
            }
        }

		/// <summary>
		/// Get optional extension for the phone number as formatted string.  
		/// Typically a 1 to 5 digit numeral.
		/// </summary>
        [XmlIgnore]
        public string FormattedExtension
		{
			get
			{
				string formattedString = "";
				string orginal = this.i_Extension.ToString();
				formattedString = orginal.PadLeft( 5, '0' );
				return formattedString;
			}
		}

        /// <summary>
        /// The Line Number, (nnn)nnn-[NNNN], for phone number.  A 4 digit
        /// numeral.
        /// </summary>
        private int LineNumber
        {
            get
            {
                return i_LineNumber;
            }
            set
            {
                int OldValue = i_LineNumber;
                i_LineNumber = value;
                this.RaiseChangedEvent( "LineNumber", OldValue, this.LineNumber );
            }
        }

		
		/// <summary>
		/// The Line Number as formatted string, (nnn)nnn-[NNNN], for phone number.  A 4 digit
		/// numeral.
		/// </summary>
		[XmlIgnore]
		private string FormattedLineNumber
		{
			get
			{
				string formattedString = "";
				string orginal = this.i_LineNumber.ToString();
				formattedString = orginal.PadLeft( 4, '0' );
				return formattedString;
			}
		}
        #endregion

        #region Private Methods
        virtual protected void Parse( int number )
        {
            string[] parts = Convert.ToString( number ).Split( new char[]{ ' ', '.', '(', ')', '-', 'x' } );
            parts = this.CleanUp( parts );
            switch( parts.Length )
            {
                case 1:
                    if( parts[0].Length > 0 )
                    {                     
                        this.Exchange    = int.Parse( parts[0].Substring( 0, 3 ) );
                        this.LineNumber  = int.Parse( parts[0].Substring( 3, 4 ) );
                    }
                    else
                    {                     
                        this.Exchange    = 0;
                        this.LineNumber  = 0;
                    }
                    break;
                case 2:
                    this.Exchange   = int.Parse( parts[0] );
                    this.LineNumber = int.Parse( parts[1] );
                    break;
            }
            return;
        }

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
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        /// <summary>
        /// Construct an empty phone number with a value of '0' for all properties.
        /// </summary>
        public PhoneNumber()
            : this( 0, 0, 0, 0, 0 )
        {
        }

        /// <summary>
        /// Construct and initialize an instance of PhoneNumber.
        /// </summary>
        /// <param name="countryCode">
        /// Country code for the PhoneNumber, "1" for US phone numbers.
        /// </param>
        /// <param name="areaCode">
        /// A 3 digit Area Code for the phone number.
        /// </param>
        /// <param name="exchange">
        /// A 3 digit exchange for the phone number.
        /// </param>
        /// <param name="lineNumber">
        /// A 4 digit Line Number for the phone number.
        /// </param>
        /// <param name="extension">
        /// An optional extension for the phone number.
        /// </param>
        private PhoneNumber( int countryCode, int areaCode, int exchange, int lineNumber, int extension )
        {
            this.CountryCode = countryCode;
            this.AreaCode    = areaCode;
            this.Exchange    = exchange;
            this.LineNumber  = lineNumber;
            this.Extension   = extension;
        }

        /// <summary>
        /// Construct and initialize an instance of PhoneNumber.
        /// </summary>
        /// <param name="countryCode">
        /// Country code for the PhoneNumber, "1" for US phone numbers.
        /// </param>
        /// <param name="areaCode">
        /// A 3 digit area code for the phone number.
        /// </param>
        /// <param name="number">
        /// A 7 digit number for the phone number.
        /// </param>
        /// <param name="extension">
        /// An optional extension for the phone number.
        /// </param>
        public PhoneNumber( int countryCode, int areaCode, int number, int extension )
        {
            this.CountryCode = countryCode;            
            this.AreaCode    = areaCode;
            this.Extension   = extension;
            try
            {
                this.Parse( number );
            }
            catch
            {
                throw new UnrecognizedPhoneNumberException( Convert.ToString( number ) );
            }
        }

        /// <summary>
        /// Create an instance of PhoneNumber attempting to parse the supplied
        /// string into constituent parts.  The delimiters allowed are spaces, 
        /// periods, dashes, parenthesis, and commas or any combination thereof.
        /// </summary>
        /// <param name="phoneNumber">
        /// A string containing one or more delimiting characters.
        /// </param>
        public PhoneNumber( string phoneNumber )
        {
            try
            {
                this.Parse( phoneNumber );
            }
            catch
            {
                throw new UnrecognizedPhoneNumberException( phoneNumber );
            }
        }
        #endregion

        #region Data Elements
        private int i_AreaCode;
        private int i_CountryCode;
        private int i_Exchange;
        private int i_LineNumber;
        private int i_Extension;
        #endregion
    }
}
