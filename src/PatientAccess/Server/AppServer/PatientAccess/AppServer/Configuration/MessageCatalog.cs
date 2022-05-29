using System;
using System.Collections;

namespace Extensions.Configuration
{
	/// <summary>
	/// MessageCatalog contains a hashtable of string messages.
	/// </summary>
	//TODO: Create XML summary comment for MessageClass
    [Serializable]
    public class MessageCatalog : object
    {
        #region Constants
        #endregion

        #region Event Handlers
        #endregion

        #region Methods
        public void Add( Message message )
        {
            if( !this.primMessages.Contains( message ) )
            {
                this.primMessages.Add( message.Oid, message );
            }
        }

        public Message MessageWith( long oid )
        {
            if( this.primMessages.ContainsKey( oid ) )
            {
                return (Message)this.primMessages[ oid ];
            }
            else
            {
                throw new MessageNotFoundException();
            }
        }

        public void Remove( Message message )
        {
            if( this.primMessages.Contains( message ) )
            {
                this.primMessages.Remove( message );
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Indexing the message.
        /// </summary>
        public Message this[ long oid ]
        {
            get
            {
                if( this.primMessages.ContainsKey( oid ) )
                {
                    return (Message)this.primMessages[ oid ];
                }
                else
                {
                    throw new MessageNotFoundException();
                }
            }
        }
        
        public static MessageCatalog Messages
        {
            get
            {
                return c_Messages;
            }
            set
            {
                c_Messages = value;
            }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        private Hashtable primMessages
        {
            get
            {
                return i_Messages;
            }
        }
        #endregion

        #region Construction and Finalization
        public MessageCatalog()
        {
            this.i_Messages = new Hashtable();
        }
        #endregion

        #region Data Elements
        private Hashtable i_Messages;
        private static MessageCatalog c_Messages;
        #endregion
    }
}
