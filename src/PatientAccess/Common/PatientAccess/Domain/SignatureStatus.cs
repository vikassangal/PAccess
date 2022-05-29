using System;

namespace PatientAccess.Domain
{
	/// <summary>
	/// Summary description for SignatureStatus.
	/// </summary>
    [Serializable]
    public class SignatureStatus : CodedReferenceValue
	{
        #region Event Handlers
        #endregion

        #region Methods

        public override string ToString()
        {   

            return String.Format("{0} {1}", Code, Description);

        }

        public bool IsSignedStatus()
        {

            return this.Code.Trim().Equals( SIGNED );

        }



        public bool IsEmptyStatus()
        {
            
            return this.Code.Trim().Equals( String.Empty );

        }

        public void SetSigned()
        {

            base.Code = SIGNED;
            base.Description = "Signed";

        }

        public void SetUnableToSign()
        {

            base.Code = UNABLE_TO_SIGN;
            base.Description = "Patient unable to sign";

        }


        public void SetRefusedToSign()
        {

            base.Code = REFUSED_TO_SIGN;
            base.Description = "Patient unwilling to sign";

        }


        public void SetToEmpty()
        {

            base.Code = String.Empty;
            base.Description = String.Empty;

        }

        #endregion

        #region Properties

        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        
        public SignatureStatus()
        {
        }

        public SignatureStatus( string code )
        {

            if( code.Equals( SIGNED ) )
            {
                this.SetSigned();
            }
            else if( code.Equals( UNABLE_TO_SIGN ) )
            {
                this.SetUnableToSign();
            }
            else
            {
                this.SetRefusedToSign();
            }

        }

        #endregion

        #region Data Elements

        #endregion

        #region Constants

        public const string SIGNED = "S";
        public const string UNABLE_TO_SIGN = "U";
        public const string REFUSED_TO_SIGN = "R";

        #endregion

    }//class

}//namespace