using System;
using System.Text.RegularExpressions;

namespace PatientAccess.Domain
{
    [Serializable]
    public class EmailAddress : ICloneable
    {
        #region Event Handlers
        #endregion

        #region Methods
        public override bool Equals( object obj )
        {
            if( obj == null )
            {
                return false;
            }
            EmailAddress em = obj as EmailAddress;
            bool result = em.i_Uri.Equals( this.i_Uri );
            return result;
        }

        public object Clone()
        {
            EmailAddress newObject = new EmailAddress();
            newObject.i_Uri = this.i_Uri;
            return newObject;
        }

        public override string ToString()
        {
            return this.Uri;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

        #region Properties
        public string Uri
        {
            get
            {
                return i_Uri;
            }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public EmailAddress()
        {
             i_Uri = String.Empty;
        }
        public EmailAddress( string uri )
        {
            Regex expression = new Regex( VALID_EMAIL_PATTERN );
           
            this.i_Uri          = uri;
        }
        #endregion

        #region Data Elements
        private string          i_Uri = String.Empty;
        #endregion

        #region Constants
        private const string VALID_EMAIL_PATTERN = @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";
        #endregion
    }
}
