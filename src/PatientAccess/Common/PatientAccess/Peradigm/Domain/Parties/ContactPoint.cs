using System;
using System.Collections;
using Peradigm.Framework.Domain.Parties.Exceptions;
using Peradigm.Framework.Persistence;

namespace Peradigm.Framework.Domain.Parties
{
	/// <summary>
	/// A class that is a container for holding various means of contacting an entity.
	/// Items such as address, phone, email can place in a contact point and designate
	/// an identifier, such as 'Home'.  This class allows the use of user defined
	/// types not inherent to the class itself
	/// </summary>
	[Serializable]
	public class ContactPoint : PersistentModel, IContactPoint
	{
		#region Constants
		/// <summary>
		/// Enumeration of various types of Contact Points
		/// </summary>
		public enum TypesOfContactPoint
		{
			Home,
			Work,
			Mobile,
			Pager,
			Fax,
			Other
		}
		/// <summary>
		/// Contacts URIs that the class is aware of, defined within this library.
		/// </summary>
		private enum EnumContactURIs
		{
			Address,
			Phone,
            EMailAddress
		}
		#endregion

        #region Methods
        /// <summary>
        /// Adds a user defined contact URI to the contact point.
        /// </summary>
        /// <param name="uriKey"></param>
        /// <param name="contactURI"></param>
        public void AddContactURI( String uriKey, object contactURI )
        {
            this.Contacts[uriKey] =  contactURI ;
        }

        /// <summary>
        /// Returns an IDictionaryEnumerator of the Contact URIs contained, order is not guaranteed.
        /// </summary>
        /// <returns>Contact URIs</returns>
        public IDictionaryEnumerator GetEnumerator()
        {
            return this.Contacts.GetEnumerator();
        }

        /// <summary>
        /// Retrieves a user defined contact URI to the contact point.  If no defined URI is found, null
        /// is returned.
        /// </summary>
        /// <param name="uriKey">The key used when calling 'AddContactURI'</param>
        /// <returns>ContactURI if found, otherwise null</returns>
        public object GetContactURI( string uriKey)
        {
            object anObject = null;
            if( this.Contacts.Contains( uriKey ) )
            {
                anObject = this.Contacts[ uriKey ];				
            }
            else
            {
                throw new ContactURINotFoundException( uriKey );
            }
            return anObject;
        }

        /// <summary>
        /// Removes a specific URI based on the key
        /// </summary>
        public void RemoveContactURI( string uriKey )
        {
            if( this.Contacts.Contains( uriKey ) )
            {
                this.Contacts.Remove( uriKey );
            }
            return;
        }

        /// <summary>
        /// Removes all URIs from the Contact Point
        /// </summary>
        public void ClearContactURIs()
        {
            this.Contacts.Clear();
        }
        #endregion

        #region Properties

	    private Hashtable Contacts
		{
			get
			{
				return i_Contacts;
			}
			set
			{
				i_Contacts = value;
			}
		}
		/// <summary>
		/// Property that designates the type of contact point
		/// </summary>
		public TypesOfContactPoint Type
		{
			get
			{
				return i_Type;
			}
		    private set
			{
				i_Type = value;
			}
		}

		/// <summary>
		/// Address as defined in EnumContactURIs.Address.  If no Address is found, a ContactURINotFoundException is thrown.
		/// </summary>
		public Address Address
		{
			get
			{	
				Address anAddress = null;
				if( this.Contacts.Contains( EnumContactURIs.Address.ToString() ) )
				{
					anAddress = (Address)this.Contacts[ EnumContactURIs.Address.ToString() ];
				}
				else
				{
					throw new ContactURINotFoundException( EnumContactURIs.Address.ToString() );
				}

				return anAddress;
			}
			set
			{
				this.Contacts[ EnumContactURIs.Address.ToString() ] = value; 				
			}
		}

		/// <summary>
		///  Phone as defined in EnumContactURIs.Phone.  If no PhoneNumber is found, a ContactURINotFoundException is thrown.
		/// </summary>
		public PhoneNumber Phone
		{
			get
			{
				PhoneNumber aPhone = null;
				if( this.Contacts.Contains( EnumContactURIs.Phone.ToString() ) )
				{
					aPhone = (PhoneNumber)this.Contacts[ EnumContactURIs.Phone.ToString() ];
				}
				else
				{
					throw new ContactURINotFoundException( EnumContactURIs.Phone.ToString() );
				}
				return aPhone;
			}
			set
			{
				this.Contacts[ EnumContactURIs.Phone.ToString() ] =  value ;
			}
		}

        /// <summary>
        ///  EMailAddress as defined in EnumContactURIs.EMailAddress.  If no EMailAddress is found, a ContactURINotFoundException is thrown.
        /// </summary>
        public EMailAddress EMailAddress
        {
            get
            {
                EMailAddress address = null;
                if( this.Contacts.Contains( EnumContactURIs.EMailAddress.ToString() ) )
                {
                    address = (EMailAddress)this.Contacts[EnumContactURIs.EMailAddress.ToString()];
                }
                else
                {
                    throw new ContactURINotFoundException( EnumContactURIs.EMailAddress.ToString() );
                }
                return address;
            }
            set
            {
                this.Contacts[EnumContactURIs.EMailAddress.ToString()] = value ;
            }
        }

        public bool Validate()
        {
            return true;
        }
        
        public void Parse( string text )
        {
        }

        public string AsString()
        {
            return base.ToString();
        }

        public bool Equals( IContactPoint cp )
        {
            return base.Equals( cp );
        }
        #endregion 

		#region Private Methods
		#endregion

		#region Private Properties
		#endregion	

        #region Construction and Finalization
        /// <summary>
        /// Constucts a empty contact point set to type EnumContactPointTypes.Other
        /// </summary>
        public ContactPoint()
            : this( TypesOfContactPoint.Other )
        {
        }

        /// <summary>
        /// Constucts a empty contact point with type set by user
        /// </summary>
        private ContactPoint( TypesOfContactPoint aPoint)
            : this( PersistentModel.NEW_OID, PersistentModel.NEW_VERSION, aPoint )
        {
        }

        private ContactPoint( long oid, byte[] timestamp, TypesOfContactPoint aPoint )
            : base( oid, timestamp )
        {
            this.Type = aPoint;
            this.Contacts = new Hashtable();
        }
        #endregion

        #region Data Elements		
        private Hashtable i_Contacts;
        private TypesOfContactPoint i_Type;
        #endregion
    }
}
