using System;
using Extensions.Persistence;

namespace Extensions.Configuration
{
	/// <summary>
	/// Message may be used to retrieve string messages from a database.
	/// </summary>
    [Serializable]
    public class Message : PersistentModel
    {
        #region Constants
        #endregion
        
        #region Event Handlers
        #endregion
        
        #region Methods
        #endregion
        
        #region Properties
        /// <summary>
        /// The message text.
        /// </summary>
        public string Text
        {
            get
            {
                return i_Text;
            }
            private set
            {
                i_Text = value;
            }
        }

        #endregion
        
        #region Private Methods
        #endregion
        
        #region Private Properties
        #endregion
        
        #region Construction and Finalization
        public Message() : base()
        {
        }

        public Message( long OID, string text, byte[] Timestamp)
            : base( OID, Timestamp )
        {
            this.Text  = text;
        }
        #endregion
        
        #region Data Elements
        private string i_Text;
        #endregion
    }
}
