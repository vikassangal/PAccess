using System;
using System.Collections;
using Peradigm.Framework.Domain.Collections;

namespace Peradigm.Framework.Domain.Parties
{
    [Serializable]
    public class OrganizationalUnit : AbstractOrganizationalUnit 
    {
        #region Constants
        #endregion

        #region Methods
        public bool IsComplete( ICollection facilities )
        {
            Set allFacilities = new Set( this.AllFacilities() );
            Set someFacilities = new Set( facilities );
            return someFacilities.ContainsSet( allFacilities );
        }

        public virtual ArrayList Children()
        {
            ArrayList childrenRelationships = this.FindChildrenRelationships();
            Set children = new Set();
            foreach( OrganizationalRelationship relationship in childrenRelationships )
            {
                children.Add( relationship.Child );
            }
            return new ArrayList( children );
        }

        private AbstractOrganizationalUnit ChildUnitWith( string code )
        {
            AbstractOrganizationalUnit selectedNode = null;
            foreach( AbstractOrganizationalUnit currentNode in this.Children() )
            {
                selectedNode = currentNode.UnitWith( code );
                if( selectedNode != null )
                {
                    break;
                }
            }
            return selectedNode;
        }
        
        public virtual ArrayList AllChildren()
        {
            ArrayList childrenRelationships = this.FindChildrenRelationships();
            Set children = new Set();
            foreach( OrganizationalRelationship relationship in childrenRelationships )
            {
                children.Add( relationship.Child );
                if( relationship.Child is OrganizationalUnit )
                {
                    children.AddSet( new Set( ((OrganizationalUnit)relationship.Child).AllChildren() ) );
                }
            }
            return new ArrayList( children );
        }

        public virtual ArrayList Facilities()
        {
            Set facilities = new Set();
            foreach( AbstractOrganizationalUnit child in this.Children() )
            {
                if( child is Facility )
                {
                    facilities.Add( child );
                }
            }
            return new ArrayList( facilities );
        }

        public virtual ArrayList AllFacilities()
        {
            Set facilities = new Set();
            foreach( AbstractOrganizationalUnit child in this.Children() )
            {
                if( child is Facility )
                {
                    facilities.Add( child );
                }
                else
                {
                    // TODO: Add AddSet method to Set class that accepts ICollection
                    facilities.AddSet( new Set( ((OrganizationalUnit)child).AllFacilities() ) );
                }
            }
            return new ArrayList( facilities );
        }

        private bool IsOfType( OrganizationalUnitType type )
        {
            return this.Type.Equals( type );
        }

        public bool IsOfType( string type )
        {
            string fullType = String.Empty;
			if( type != null )
			{
				fullType = type.Trim();
			}

			return this.IsOfType( new OrganizationalUnitType( type ) );
        }
        #endregion

        #region Properties
        public override AbstractOrganizationalUnit UnitWith( string code )
        {
            AbstractOrganizationalUnit selectedNode = null;
            if( this.Code == code )
            {
                selectedNode = this;
            }
            else
            {
                selectedNode = this.ChildUnitWith( code );
            }
            return selectedNode;
        }
        #endregion

        #region Private Methods

        private ArrayList FindChildrenRelationships()
        {
            ArrayList allRelationships = this.OrganizationalRelationships();
            ArrayList childrenRelationships = new ArrayList();
            foreach( OrganizationalRelationship relationship in allRelationships )
            {
                if( relationship.Parent.Equals( this ) )
                {
                    childrenRelationships.Add( relationship );
                }
            }
            return childrenRelationships;
        }
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public OrganizationalUnit()
			: this( String.Empty, String.Empty, String.Empty )
        {
        }

        public OrganizationalUnit( string code, string name, string type )
            : this(  NEW_OID, NEW_VERSION, code, name, type )
        {
        }

        private OrganizationalUnit( long oid, Byte[] version, string code, string name, string type )
            : base( oid, version, code, name )
        {
            this.Type = new OrganizationalUnitType( type );
        }
        #endregion

        #region Data Elements
        #endregion
    }
}