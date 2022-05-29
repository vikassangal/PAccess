using System;
using System.Text.RegularExpressions;
using Peradigm.Framework.Domain.Parties.Exceptions;

namespace Peradigm.Framework.Domain.Parties
{
    /// <summary>
    /// A class that describes a standard two-part e-mail address (Mailbox@host).
    /// EMailAddress includes validation and throws exceptions if constructed with invalid value;
    /// </summary>
    [Serializable]
    public class EMailAddress : Model, IContactPoint
    {
        #region Constants
        private const string
            VALID_EMAIL_PATTERN     = @"^([a-zA-Z]([-.\w]*[0-9a-zA-Z])*@(([0-9a-zA-Z])+([-\w]*[0-9a-zA-Z])*\.)+[a-zA-Z]{2,9})$",
            EMAIL_FORMAT            = "{0}@{1}",
            REGEX_EMAIL_PARTS       = "(@)",
            FORMAT_HASH_IDENTITY    = "{0}:{1}";

        private const int 
            REGEX_NUMBER_OF_PARTS   = 3,
            REGEX_MAILBOX           = 0,
            REGEX_HOST              = 2;
        #endregion

        #region Event Handlers
        #endregion

        #region Methods
        /// <summary>
        /// Returns string representation of E-Mail address
        /// </summary>
        /// <returns>A string</returns>
        public virtual string AsString()
        {
            return String.Format( EMAIL_FORMAT, this.MailBox, this.Host );
        }

        public override string ToString()
        {
            return this.AsString();
        }

        public bool Validate()
        {
            return this.IsValidAddress( this.Text );
        }

        public virtual void Parse( string text )
        {
            if( !this.IsValidAddress( text ) )
            {
                throw new InvalidEMailAddressException( text );
            }

            string[] parts = Regex.Split( text, REGEX_EMAIL_PARTS );
            if( parts.Length != REGEX_NUMBER_OF_PARTS )
            {
                throw new InvalidEMailAddressException( text );
            }

            this.MailBox = parts[REGEX_MAILBOX].Trim();
            this.Host = parts[REGEX_HOST].Trim();

            if( this.MailBox == String.Empty || this.Host == String.Empty )
            {
                throw new InvalidEMailAddressException( text );
            }
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
        #endregion

        #region Properties

        private string MailBox
        {
            get
            {
                return i_MailBox;
            }
            set
            {
                i_MailBox = value;
            }
        }

        private string Host
        {
            get
            {
                return i_Host;
            }
            set
            {
                i_Host = value;
            }
        }

        public virtual string Text
        {
            get
            {
                return this.AsString();
            }
            set
            {
                this.Parse( value );
            }
        }
        #endregion

        #region Private Methods
        protected virtual bool IsValidAddress( string text )
        {
            return Regex.IsMatch( text, VALID_EMAIL_PATTERN );
        }
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        [Obsolete( "Default constructor should be used for serialization only", false )]
        public EMailAddress()
        {
        }

        public EMailAddress( string address )
        {
            this.Parse( address );
        }

        public EMailAddress( string mailBox, string host )
        {
            this.MailBox = mailBox.Trim();
            this.Host = host.Trim();
            if( !this.IsValidAddress( this.AsString() ) )
            {
                throw new InvalidEMailAddressException( String.Format( EMAIL_FORMAT, mailBox, host ) );
            }
        }
        #endregion

        #region Data Elements
        private string i_MailBox;
        private string i_Host;
        #endregion
    }
}
