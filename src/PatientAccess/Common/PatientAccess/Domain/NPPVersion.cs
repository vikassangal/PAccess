using System;

namespace PatientAccess.Domain
{
	/// <summary>
	/// Summary description for NPPVersion.
	/// </summary>
	[Serializable]
	public class NPPVersion : CodedReferenceValue
	{
        #region Event Handlers
        #endregion

        #region Methods
        public override string ToString()
        {   
            if( this.Description != null
                && this.Description.Length >= 8
                && this.NPPVersionNumber != null)
            {
                //we get data in format YYYYMMDD, need to return it in YYYY-MM-DD
                String description =  
                    this.Description.Substring(4, 2) + "/" +
                    this.Description.Substring(6, 2) + "/" +
                    this.Description.Substring(0, 4);
                return String.Format("{0}     {1}", this.NPPVersionNumber, description);
            }
            else if( (!this.IsValid) && this.NPPVersionNumber != null )
            {
                return String.Format("{0}     {1}", this.NPPVersionNumber, this.Description);
            }
            else
                return string.Empty;
        }
        #endregion

        #region Properties
        public DateTime NPPDate
        {
            get
            {
                return i_NPPDate;
            }
            set
            {
                i_NPPDate = value;
            }
        }

	    private string NPPVersionNumber
        {
            get
            {
                if( this.Code != null && this.Code.Length > 0)
                {
                    return this.Code.Insert( this.Code.Length - 1, "." );
                }
                return String.Empty;            
            }        
        }

        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public NPPVersion()
        {
        }
        public NPPVersion( long oid, DateTime version, string description )
            : base( oid, version, description )
    {
    }

        public NPPVersion( long oid, DateTime version, string description, string code )
            : base( oid, version, description, code )
    {
    }
        #endregion

        #region Data Elements        
        private DateTime i_NPPDate;
        private string i_NPPVersionNumber = String.Empty;
        #endregion

        #region Constants
        #endregion
    }
}
