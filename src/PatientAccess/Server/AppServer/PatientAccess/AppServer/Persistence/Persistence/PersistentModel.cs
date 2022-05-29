using System;
using System.Xml.Serialization;

namespace Extensions.Persistence
{
    /// <summary>
    /// PersistentModel may be used as the base class for any class
    /// that may be expected to be persisted at any point.  This class
    /// provides a property to uniquely identify the object.  This
    /// value may represent the ID or key provided by a relational
    /// database server.
    /// </summary>
    [Serializable]
    [XmlType( Namespace = "Extensions.ClassLibrary.Persistence" )]
    public class PersistentModel : Model
    {
        #region Constants
        public static long   NEW_OID       = 0;
        private static byte[] NEW_VERSION = new byte[8]{ 0, 0, 0, 0, 0, 0, 0, 0 };
        #endregion

        #region Methods
        public bool HasTimestamp( byte[] timestamp )
        {
            if( ( null == this.Timestamp ) || 
                ( null == timestamp ) )
            {
                return false;
            }

            bool bytesMatch = true;
            for( long i = 0 ; i < this.Timestamp.Length; i++ )
            {
                bytesMatch = this.Timestamp[i] == timestamp[i];
                if( !bytesMatch )
                {
                    break;
                }
            }
            return bytesMatch;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Property representing the record ID or key value from the underlying
        /// storage mechanism.  Typically, this property represents the primary key
        /// from a SQL table that has been defined as an integer auto-increment field.
        /// </summary>
        public long Oid
        {
            get
            {
                return i_Oid;
            }
            private set
            {
                i_Oid = value;
            }
        }

        /// <summary>
        /// Property representing the timestamp of the last record update.  This value
        /// may be used to compare updates to determine if a refresh of client data
        /// may be necessary before saving data.
        /// </summary>
        private byte[] Timestamp
        {
            get
            {
                return i_Timestamp;
            }
            set
            {
                i_Timestamp = value;
            }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        /// <summary>
        /// Construct a new instance of a persistent object that has 
        /// not been persisted
        /// </summary>
        public PersistentModel() : this( 0 )
        {
        }

        /// <summary>
        /// Construct a new instance representing an existing entity from the
        /// storage mechanism.  Timestamp is defaulted to the time of instance
        /// creation.
        /// </summary>
        /// <param name="oid">
        /// The record ID or key value from the underlying storage mechanism.
        /// </param>
        public PersistentModel( long oid ) : this( oid, NEW_VERSION )
        {
        }

        /// <summary>
        /// Construct a new instance representing an existing entity from the
        /// storage mechanism.
        /// </summary>
        /// <param name="oid">
        /// The record ID or key value from the underlying storage mechanism.
        /// </param>
        /// <param name="timestamp">
        /// Date and time of the last update to the record.
        /// </param>
        public PersistentModel( long oid, byte[] timestamp )
        {
            this.Oid        = oid;
            this.Timestamp  = timestamp;
        }
        #endregion

        #region Data Elements
        private long   i_Oid;
        private byte[] i_Timestamp;
        #endregion
    }
}
