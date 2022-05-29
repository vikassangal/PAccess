using System;

namespace Extensions.PersistenceCommon
{
	/// <summary>
	/// PersistentModel may be used as the base class for any class
	/// that may be expected to be persisted at any point.  This class
	/// provides a property to uniquely identify the object.  This
	/// value may represent the ID or key provided by a relational
	/// database server.
	/// </summary>
	[Serializable]
	public class PersistentModel : Model
	{

		#region Constants 

		public const long NEW_OID = 0;

		#endregion Constants 

		#region Fields 

		private long i_Oid;
		private DateTime i_Timestamp;
		public static DateTime NEW_VERSION = new DateTime(0);

		#endregion Fields 

		#region Constructors 

		/// <summary>
		/// Construct a new instance representing an existing entity from the
		/// storage mechanism.
		/// </summary>
		/// <param name="OID">
		/// The record ID or key value from the underlying storage mechanism.
		/// </param>
		/// <param name="Timestamp">
		/// Date and time of the last update to the record.
		/// </param>
		public PersistentModel( long oid, DateTime timestamp )
		{

			this.Oid        = oid;
			this.Timestamp  = timestamp;

		}

		/// <summary>
		/// Construct a new instance representing an existing entity from the
		/// storage mechanism.  Timestamp is defaulted to the time of instance
		/// creation.
		/// </summary>
		/// <param name="OID">
		/// The record ID or key value from the underlying storage mechanism.
		/// </param>
		public PersistentModel( long oid ) : this( oid, NEW_VERSION ){}

		/// <summary>
		/// Construct a new instance of a persistent object that has 
		/// not been persisted
		/// </summary>
		public PersistentModel() : this( 0 ){}

		#endregion Constructors 

		#region Properties 

		/// <summary>
		/// Property representing the record ID or key value from the underlying
		/// storage mechanism.  Typically, this property represents the primary key
		/// from a SQL table that has been defined as an integer auto-increment field.
		/// </summary>
		public long Oid
		{

			get
			{

				return this.i_Oid;

			}
			set
			{

                long oldValue = this.i_Oid;

				this.i_Oid = value;

                if( !this.i_Oid.Equals( oldValue ) )
                    this.RaiseChangedEvent( "Oid", oldValue, value );
			
            }

		}

		/// <summary>
		/// Property representing the timestamp of the last record update.  This value
		/// may be used to compare updates to determine if a refresh of client data
		/// may be necessary before saving data.
		/// </summary>
		public DateTime Timestamp
		{

			get
			{

				return this.i_Timestamp;

			}
			set
			{

                DateTime oldValue = this.i_Timestamp;

				this.i_Timestamp = value;

                if( !this.i_Timestamp.Equals( oldValue ) )
                    this.RaiseChangedEvent( "Timestamp", oldValue, value );

			}

		}

		#endregion Properties 

		#region Methods 

        /// <summary>
        /// Determines whether the specified timestamp has timestamp.
        /// </summary>
        /// <param name="timestamp">The timestamp.</param>
        /// <returns>
        /// 	<c>true</c> if the specified timestamp has timestamp; otherwise, <c>false</c>.
        /// </returns>
		public bool HasTimestamp( DateTime timestamp )
		{

			return( this.Timestamp.Equals( timestamp ) );

		}

		#endregion Methods 

	}//class

}//namespace
