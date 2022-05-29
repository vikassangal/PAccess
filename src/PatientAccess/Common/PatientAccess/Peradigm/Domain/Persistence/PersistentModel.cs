//-----------------------------------------------------------------------------
// Copyright © 2003-2005 Perot Systems Coproration. All rights reserved.
//-----------------------------------------------------------------------------

using System;
using Peradigm.Framework.Domain;

namespace Peradigm.Framework.Persistence
{
    /// <summary>
    /// PersistentModel may be used as the base class for any class
    /// that may be expected to be persisted at any point.  This class
    /// provides a property to uniquely identify the object.  This
    /// value may represent the ID or key provided by a relational
    /// database server.
    /// </summary>
    [Serializable]
    public class PersistentModel : Model, IPersistentModel
    {
        #region Constants
        public static long   NEW_OID       = 0;
        public static byte[] NEW_VERSION = new byte[8]{ 0, 0, 0, 0, 0, 0, 0, 0 };
        #endregion

        #region Methods
        public bool HasBeenSaved()
        {
            return ( this.Oid != PersistentModel.NEW_OID ) &&
                   ( !this.HasVersion( NEW_VERSION ) );
        }
        
        public bool HasNotBeenSaved()
        {
            return !this.HasBeenSaved();
        }

        public bool HasVersion( byte[] version )
        {
            if( ( null == this.Version ) ||
                ( null == version ) )
            {
                return false;
            }

            bool bytesMatch = true;
            for( long i = 0; i < this.Version.Length; i++ )
            {
                bytesMatch = this.Version[i] == version[i];
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
            set
            {
                i_Oid = value;
            }
        }

        /// <summary>
        /// Property representing the timestamp of the last record update.  This value
        /// may be used to compare updates to determine if a refresh of client data
        /// may be necessary before saving data.
        /// </summary>
        public byte[] Version
        {
            get
            {
                return i_Version;
            }
            set
            {
                i_Version = value;
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
        public PersistentModel() 
            : this( NEW_OID )
        {
        }

        /// <summary>
        /// Construct a new instance representing an existing entity from the
        /// storage mechanism.  Version is defaulted to the time of instance
        /// creation.
        /// </summary>
        /// <param name="oid">
        /// The record ID or key value from the underlying storage mechanism.
        /// </param>
        private PersistentModel( long oid ) 
            : this( oid, NEW_VERSION )
        {
        }

        /// <summary>
        /// Construct a new instance representing an existing entity from the
        /// storage mechanism.
        /// </summary>
        /// <param name="oid">
        /// The record ID or key value from the underlying storage mechanism.
        /// </param>
        /// <param name="version">
        /// ByteArray representing the version of the object.
        /// </param>
        public PersistentModel( long oid, byte[] version )
        {
            this.Oid     = oid;
            this.Version = version;
        }
        #endregion

        #region Data Elements
        private long   i_Oid;
        private byte[] i_Version;
        #endregion
    }
}
