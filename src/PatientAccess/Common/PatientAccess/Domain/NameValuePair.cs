using PatientAccess.Domain.Auditing.FusNotes;

namespace PatientAccess.Domain
{
	/// <summary>
	/// Summary description for NameValuePair.
	/// </summary>
	public class NameValuePair
	{
        #region Constants
        #endregion

        #region Event Handlers
        #endregion

        #region Methods
        public override string ToString()
        {
            return ( (this.Key == null ? string.Empty : this.Key.Trim()) + 
                FusLabel.LABEL_COLON_SPACE + 
                (this.Value == null ? string.Empty : this.Value.Trim()) + 
                FusLabel.LABEL_SEMI_COLON_SPACE );
        }
        #endregion

        #region Properties

	    private string Key
        {
            get
            {
                return i_key;
            }
            set
            {
                i_key = value;
            }
        }

	    private string Value
        {
            get
            {
                return i_value;
            }
            set
            {
                i_value = value;
            }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties

        #endregion

        #region Construction and Finalization
        public NameValuePair()
        {
        }

        public NameValuePair( string key, string val )
        {
            this.Key = key;
            this.Value = val;
        }
        #endregion

        #region Data Elements
        private string i_key;
        private string i_value;
        #endregion
	}
}
