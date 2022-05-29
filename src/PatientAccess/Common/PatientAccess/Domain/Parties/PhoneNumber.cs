using System;
using System.Reflection;
using System.Text.RegularExpressions;
using Extensions;

namespace PatientAccess.Domain.Parties
{
    [Serializable]
    public class PhoneNumber : Model, ICloneable
    {
        #region Constants
        #endregion

        #region Event Handlers
        #endregion

        #region Methods
        public override string ToString()
        {
            if( this.IsBlankOrZero() )
            {
                return String.Empty;
            }
            else
            {
                return String.Format( "{0}{1}", this.AreaCode, this.Number );
            }
        }

        public override bool Equals( object obj )
        {
            PhoneNumber rightSideValue = obj as PhoneNumber;

            if( rightSideValue == null )
            {
                return false;
            }

            return ( rightSideValue.AreaCode.Equals( this.AreaCode ) &&
                     rightSideValue.Number.Equals( this.Number ) &&
                     rightSideValue.CountryCode.Equals( this.CountryCode ) );
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public new object Clone()
        {
            PhoneNumber phoneNumber = new PhoneNumber( this.CountryCode, this.AreaCode, this.Number );
            return phoneNumber;
        }

        public string AsUnformattedString()
        {
            return this.ToString();
        }

        public string AsFormattedString()
        {
            string firstPart = null;;
            string secondPart = null ;
            string areaCode = null;

			if( this.IsBlankOrZero() ) return string.Empty;
            if (Number.Length >= 3)
            {
                firstPart = this.Number.Substring( 0, 3 );
                secondPart = this.Number.Substring( 3 );
            }
            
            if( this.AreaCode.Length == 0 )
            {
                areaCode = string.Empty;
            }
            else if( this.AreaCode.Length != 3 )
            {
                areaCode = this.AreaCode.PadLeft(3,'0');
            }
            else
            {
                areaCode = this.AreaCode;
            }
            return String.Format( 
                "({0}) {1}-{2}",
                areaCode,
                firstPart,
                secondPart
                );
        }
        #endregion

        #region Properties
        public string AreaCode
        {
            get
            {
                return i_AreaCode;
            }
            set
            {
                  if( value != null &&
                    Regex.IsMatch( value, @"\d{3,}") &&
                    (long.Parse(value.Trim()) > 0) )
                {
                    this.SetAndTrack<string>(ref this.i_AreaCode, value, MethodBase.GetCurrentMethod() );
                }
                else
                {
                    this.SetAndTrack<string>( ref this.i_AreaCode, string.Empty, MethodBase.GetCurrentMethod() );
                }
            }
        }

        public string Number
        {
            get
            {
                return i_Number;
            }
            set
            {
                if( value != null &&
                   Regex.IsMatch( value, @"\d{6,}" ) &&
                   ( long.Parse( value.Trim() ) > 0 ) )
                {
                    this.SetAndTrack<string>( ref this.i_Number, value, MethodBase.GetCurrentMethod() );
                }
                else
                {
                    this.SetAndTrack<string>( ref this.i_Number, string.Empty, MethodBase.GetCurrentMethod() );
                }
            }
        }

        public string CountryCode
        {
            get
            {
                return i_CountryCode;
            }
      
            set
            {
                i_CountryCode = value;
            }
        }

        public bool IsValid
        {
            get
            {
                return i_IsValid;
            }
        }

        #endregion

        #region Private Methods
		private bool IsBlankOrZero()
		{
			return ( ( this.AreaCode.Equals( string.Empty ) && this.Number.Equals( string.Empty ) ) || 
                     ( this.AreaCode.Equals( "0" ) && this.Number.Equals( "0" ) ) ||
                     ( this.AreaCode.Equals( "000" ) && this.Number.Equals( "0000000" ) )
                   );
		}

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public PhoneNumber()
        {
            this.CountryCode = String.Empty;
            this.AreaCode = String.Empty;
            this.Number = String.Empty;
        }

        public PhoneNumber( string countryCode, string areaCode, string phoneNumber )
        {
            this.CountryCode = countryCode;
            this.AreaCode    = areaCode;
            this.Number      = phoneNumber;
         }
 
        public PhoneNumber( string areaCodeAndPhoneNumber )
        {
            this.i_IsValid = true;

            if( areaCodeAndPhoneNumber.Length == 12 )
            {
                try
                {
                    areaCodeAndPhoneNumber = areaCodeAndPhoneNumber.Replace("-", string.Empty);
                }
                catch (ArgumentNullException)
                {
                }
                catch (ArgumentException)
                {
                }
            }

            if( areaCodeAndPhoneNumber.Length != 10 )
            {
                this.i_IsValid = false;                
            }
            
            if( areaCodeAndPhoneNumber.Length ==  10 )
            {
                this.AreaCode = areaCodeAndPhoneNumber.Substring( 0, 3 );
                this.Number = areaCodeAndPhoneNumber.Substring( 3 );
            }
            else if( areaCodeAndPhoneNumber.Length == 7 )
            {
                this.Number = areaCodeAndPhoneNumber;
            }
            else if( areaCodeAndPhoneNumber.Length > 2 )
            {
                this.AreaCode = areaCodeAndPhoneNumber.Substring( 0, 3 );
            }
        }

        public PhoneNumber( string areaCode, string phoneNumber )
        {
            this.AreaCode = areaCode;
            this.Number   = phoneNumber;
        }
        #endregion

        #region Data Elements
        private string          i_CountryCode = String.Empty;
        private string          i_AreaCode = String.Empty;
        private string          i_Number = String.Empty;
        private bool            i_IsValid = true;
        #endregion
    }
}
