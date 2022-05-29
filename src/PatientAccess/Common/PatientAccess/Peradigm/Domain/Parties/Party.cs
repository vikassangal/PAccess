using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Peradigm.Framework.Domain.Collections;
using Peradigm.Framework.Persistence;

namespace Peradigm.Framework.Domain.Parties
{
    /// <summary>
    /// Party is a base class for Parties and Organizations,
    /// as described in Martin Fowler's Analysis Patterns
    /// </summary>
	[Serializable]
	public class Party : PersistentModel
	{
		#region Constants
        private const string
            COMPOSITE_CONTACT_POINT_TYPE = "Composite";
		#endregion

		#region Event Handlers
		#endregion

		#region Methods
		public ContactPoint AddContactPoint( ContactPoint cp )
		{
            return this.Add( cp, TypeOfContactPoint.With( COMPOSITE_CONTACT_POINT_TYPE ) ) as ContactPoint;
		}

        private IContactPoint Add( IContactPoint contactPoint, TypeOfContactPoint typeOfContactPoint )
        {
            if( contactPoint != null && typeOfContactPoint != null )
            {
                Set allContactPointsForType = this.primContactPointsMatching( typeOfContactPoint );
                allContactPointsForType.Add( contactPoint );
            }
            return contactPoint;
        }

        public bool HasContactPoint( IContactPoint contactPoint )
        {
            bool found = false;
            if( contactPoint != null )
            {
                foreach( TypeOfContactPoint typeOfContactPoint in this.primContactPoints.Keys )
                {
                    Set allContactPointsForType = this.primContactPointsMatching( typeOfContactPoint );
                    found = ( allContactPointsForType.ElementLike( contactPoint ) != null );
                    if( found )
                    {
                        break;
                    }
                }
            }
            return found;
        }

        private ArrayList ContactPointsMatching( TypeOfContactPoint typeOfContactPoint )
        {
            return this.primContactPointsMatching( typeOfContactPoint ).AsArrayList();
        }

        private List<T> ContactPointsMatching<T>( TypeOfContactPoint typeOfContactPoint )
        {
            return this.primContactPointsMatching( typeOfContactPoint ).AsListOf<T>();
        }

        private List<T> AllContactPoints<T>()
        {
            List<T> resultingList = new List<T>();
            foreach( TypeOfContactPoint typeOfContactPoint in this.primContactPoints.Keys )
            {
                resultingList.AddRange( this.ContactPointsMatching<T>( typeOfContactPoint ) );
            }
            return resultingList;
        }

		public Relationship AddRelationship( Relationship relationship )
		{
			if( !this.primRelationships.Contains( relationship ) )
			{
				this.primRelationships.Add( relationship );
			}
			return relationship;
		}

        public ArrayList FindContactPoints( ContactPoint.TypesOfContactPoint type )
		{
			ArrayList cps = new ArrayList();
            List<ContactPoint> listOfCompositContacts =
                this.AllContactPoints<ContactPoint>() ;
			foreach( ContactPoint cp in listOfCompositContacts )
			{
				if( cp.Type == type )
				{
					cps.Add( cp );
				}
			}
			return cps;
		}

		public ArrayList FindRelationships( RelationshipType type )
		{
			ArrayList relationships = new ArrayList();
			foreach( Relationship r in this.primRelationships )
			{
				if( r.Type.Oid == type.Oid )
				{
					relationships.Add( r );
				}
			}
			return relationships;
		}

		public ArrayList RelationshipOf( string typeName )
		{
			ArrayList relationships = new ArrayList();
			foreach( Relationship r in this.primRelationships )
			{
				if( r.Type.Description == typeName )
				{
					relationships.Add( r );
				}
			}
			return relationships;
		}

        private IContactPoint Remove( IContactPoint cp )
        {
            foreach( TypeOfContactPoint typeOfContactPoint in this.primContactPoints.Keys )
            {
                this.Remove( cp, typeOfContactPoint );
            }
            return cp;
        }

        private IContactPoint Remove( IContactPoint cp, TypeOfContactPoint typeOfContactPoint )
        {
            Set allContactPointsForType = this.primContactPointsMatching( typeOfContactPoint );
            allContactPointsForType.Remove( cp );
            return cp;
        }


		public ContactPoint RemoveContactPoint( ContactPoint cp )
		{
            return this.Remove( cp ) as ContactPoint;
		}

		public Relationship RemoveRelationship( Relationship relationship )
		{
			if( this.primRelationships.Contains( relationship ) )
			{
				this.primRelationships.Remove( relationship );
			}
			return relationship;
		}

        public TypeOfContactPoint TypeOfContactPointFor( IContactPoint cp )
        {
            TypeOfContactPoint resultingType = null;
            foreach( TypeOfContactPoint typeOfContactPoint in this.primContactPoints.Keys )
            {
                Set allContactPointsForType = this.primContactPointsMatching( typeOfContactPoint );
                if( allContactPointsForType.ElementLike( cp ) != null )
                {
                    resultingType = typeOfContactPoint;
                    break;
                }
            }
            return resultingType;
        }
		#endregion

		#region Properties
		[XmlArrayItem( "ContactPoint", typeof( ContactPoint ) )]
        public ArrayList ContactPoints
		{
			get
			{
                ArrayList allContactPoints = new ArrayList();
                List<ContactPoint> listOfCompositContacts =
                    this.AllContactPoints<ContactPoint>();
                allContactPoints.AddRange( listOfCompositContacts ); ;
                return allContactPoints;
			}
			set
			{
				//required for serialization
			}
		}

        [XmlArrayItem( typeof( ContactPoint ) )]
        [XmlArrayItem( typeof( EMailAddress ) )]
        [XmlArrayItem( typeof( PhoneNumber ) )]
        [XmlArrayItem( typeof( Address ) )]
        public ArrayList ContactList
        {
            get
            {
                ArrayList allContactPoints = new ArrayList();
                foreach( TypeOfContactPoint typeOfContactPoint in this.primContactPoints.Keys )
                {
                    allContactPoints.AddRange(
                        this.ContactPointsMatching( typeOfContactPoint ) );
                }
                return allContactPoints;
            }
            set
            {
                //required for serialization
            }
        }

		virtual public string Name
		{
			get
			{
				return i_Name;
			}
			set
			{
				i_Name = value;
			}
		}

		[XmlArrayItem( "Relationship", typeof( Relationship ) )]
		public ArrayList Relationships
		{
			get
			{
				return (ArrayList)this.primRelationships.Clone();
			}
			set
			{
				//required for serialization
			}
		}
		#endregion

		#region Private Methods
        private Set primContactPointsMatching( TypeOfContactPoint typeOfContactPoint )
        {
            Set allContactPointsForType = this.primContactPoints[typeOfContactPoint] as Set;
            if( allContactPointsForType == null )
            {
                allContactPointsForType = new Set();
                this.primContactPoints[typeOfContactPoint] = allContactPointsForType;
            }
            return allContactPointsForType;
        }
		#endregion

		#region Private Properties

        private Hashtable primContactPoints
		{
			get
			{
				return i_ContactPoints;
			}
			set
			{
				i_ContactPoints = value;
			}
		}

        protected ArrayList primRelationships
		{
			get
			{
				return i_Relationships;
			}
            private set
			{
				i_Relationships = value;
			}
		}
		#endregion

		#region Construction and Finalization
		//Required for Serialization
		public Party()
            : this( String.Empty )
		{
		}

        private Party( string name )
            : this( NEW_OID, NEW_VERSION, name )
        {
        }

        public Party( long oid, 
                      byte[] version, 
                      string name )
            : base( oid, version )
        {
            this.Name  = name;
            this.primContactPoints = new Hashtable();
            this.primRelationships = new ArrayList();
        }
        #endregion

        #region Data Elements
        private Hashtable   i_ContactPoints;
        private string      i_Name;
        private ArrayList   i_Relationships;
        #endregion
    }
}
