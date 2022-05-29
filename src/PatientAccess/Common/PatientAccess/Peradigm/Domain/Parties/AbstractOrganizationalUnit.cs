using System;
using System.Collections;
using Peradigm.Framework.Domain.Collections;

namespace Peradigm.Framework.Domain.Parties
{
	/// <summary>
	/// Summary description for AbstractOrganizationalUnit.
	/// </summary>
	[Serializable]
	public abstract class AbstractOrganizationalUnit : Party
	{
		#region Constants
		private const string 
			FULL_CODE_SEPARATOR = ":::",
			FULL_CODE = "{0}" +  FULL_CODE_SEPARATOR + "{1}";
		#endregion

		#region Methods
		public override int GetHashCode()
		{
			string typeDescription = String.Empty;
			string code = String.Empty;
			if( this.Type != null && this.Type.Description != null )
			{
				typeDescription = this.Type.Description.Trim();
			}
			if( this.Code != null )
			{
				code = this.Code.Trim();
			}
			return String.Format( FULL_CODE, typeDescription, code ).ToLower().GetHashCode();
		}

		public virtual bool Equals( AbstractOrganizationalUnit unit )
		{
			return this.GetHashCode().Equals( unit.GetHashCode() );
		}

		public virtual bool Equals( string code )
		{
			string hashValue = String.Empty;
			
			if( code != null )
			{
				hashValue = code.ToLower();
			}

			if( hashValue.IndexOf( FULL_CODE_SEPARATOR ) == -1 )
			{
				string typeDescription = String.Empty;
				if( this.Type != null && this.Type.Description != null )
				{
					typeDescription = this.Type.Description.Trim();
				}
				hashValue = String.Format( FULL_CODE, typeDescription, code ).ToLower();
			}

			return this.GetHashCode().Equals( hashValue.GetHashCode() );
		}

		public override bool Equals( object obj )
		{
			AbstractOrganizationalUnit unit = obj as AbstractOrganizationalUnit;
			if( unit != null )
			{
				return this.Equals( unit );
			}
			return base.Equals( obj );
		}

		public ArrayList OrganizationalRelationships()
		{
			Set relationships = new Set();
			foreach( Relationship relationship in this.primRelationships )
			{
				if( relationship is OrganizationalRelationship )
				{
					relationships.Add( relationship );
				}
			}
			return new ArrayList( relationships );
		}

		public virtual AbstractOrganizationalUnit Parent()
		{
			OrganizationalRelationship relationship = this.FindParentRelationship();
			if( relationship != null )
			{
				return (AbstractOrganizationalUnit)relationship.Parent;
			}
			return null;
		}

		public void RemoveRelationshipWith( AbstractOrganizationalUnit organizationalUnit )
		{
			foreach( Relationship relationship in this.Relationships )
			{
				OrganizationalRelationship orgRelationship = relationship as OrganizationalRelationship;
				if( orgRelationship != null )
				{
					if( orgRelationship.Child.Equals(organizationalUnit) ||
						orgRelationship.Parent.Equals(organizationalUnit) )
					{
						this.RemoveRelationship( orgRelationship );
					}
				}
			}
		}

		public abstract AbstractOrganizationalUnit UnitWith( string code );
		#endregion

		#region Properties

	    private bool Active
		{
			get
			{
				return i_Active;
			}
			set
			{
				i_Active = value;
			}
		}

		public string Code
		{
			get
			{
				return i_Code;
			}
			set
			{
				i_Code = value;
			}
		}

		virtual public OrganizationalUnitType Type
		{
			get
			{
				return i_Type;
			}
			set
			{
				i_Type = value;
			}
		}
		#endregion

		#region Private Methods

	    private OrganizationalRelationship FindParentRelationship()
		{
			ArrayList allRelationships = this.OrganizationalRelationships();
			foreach( OrganizationalRelationship relationship in allRelationships )
			{
				if( relationship.Child.Equals( this ) )
				{
					return relationship;
				}
			}
			return null;
		}
		#endregion

		#region Private Properties
		#endregion
        
		#region Construction and Finalization
		public AbstractOrganizationalUnit()
			: this( String.Empty, String.Empty )
		{
		}

		private AbstractOrganizationalUnit( string code, string name  )
			: this(  NEW_OID, NEW_VERSION, code, name )
		{
		}

		public AbstractOrganizationalUnit( long oid, Byte[] version, string code, string name )
			: this( oid, version, code, name, String.Empty )
		{
		}

		private AbstractOrganizationalUnit( long oid, Byte[] version, string code, string name, string type )
			: base( oid, version, name )
		{
			this.Active = true;
			this.Code = code;
			this.Type = new OrganizationalUnitType( type );
		}
		#endregion

		#region Data Elements
		private bool                   i_Active;
		private string                 i_Code;
		private OrganizationalUnitType i_Type;
		#endregion
	}
}
