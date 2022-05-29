using System;
using System.Xml.Serialization;
using Peradigm.Framework.Persistence;

namespace Peradigm.Framework.Domain.Parties
{
	/// <summary>
	/// Relationship class represents any relationship (i.e. contract relationship) among two parties.
	/// </summary>
	[Serializable]
	public class Relationship : PersistentModel
	{
		#region Constants
		#endregion

		#region Event Handlers
		#endregion

		#region Methods
		#endregion

		#region Properties
		[XmlIgnore]
		public Party FirstParty
		{
			get
			{
				return i_FirstParty;
			}
			set
			{
				i_FirstParty = value;
			}
		}

		[XmlIgnore]
		public Party SecondParty
		{
			get
			{
				return i_SecondParty;
			}
			set
			{
				i_SecondParty = value;
			}
		}

		public string TypeName
		{
			get
			{
				return this.Type.Description;
			}
			set
			{
				//Required for xml serialization
			}
		}

		[XmlIgnore]
		public virtual RelationshipType Type
		{
			get
			{
				return i_Type;
			}
			set
			{
				i_Type = value;
                if( value == null )
                {
                    i_Type = new RelationshipType( String.Empty );
                }
			}
		}
		#endregion

		#region Private Methods
		#endregion

		#region Private Properties
		#endregion

		#region Construction and Finalization
		//Serialization
		public Relationship()
		{
		}

        // To support base constructor in descendants
        protected Relationship( long oid, byte[] version )
            : base( oid, version )
        {
        }

        public Relationship( RelationshipType type, 
                             Party firstParty, 
                             Party secondParty )
            : this( NEW_OID, NEW_VERSION, type, firstParty, secondParty )
        {
        }

        public Relationship( long oid,
                             byte[] version,
                             RelationshipType type, 
                             Party firstParty,
                             Party secondParty ) : base( oid, version )
        {
            this.Type        = type;
            this.FirstParty  = firstParty;
            this.SecondParty = secondParty;
        }
        #endregion

        #region Data Elements
        private Party i_FirstParty;
        private Party i_SecondParty;
        private RelationshipType i_Type;
        #endregion
    }
}
