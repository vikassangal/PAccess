using System;
using System.Xml.Serialization;

namespace Peradigm.Framework.Domain.Parties
{
    [Serializable]
    public class OrganizationalRelationship : Relationship
    {
        #region Constants
        private const string ORGANIZATIONAL_RELATIONSHIP = "Organizational";
        #endregion

        #region Event Handlers
        #endregion

        #region Methods
        #endregion

        #region Properties
        [XmlIgnore]
        public AbstractOrganizationalUnit Parent
        {
            get
            {
                return (AbstractOrganizationalUnit)this.FirstParty;
            }
            set
            {
                this.FirstParty = value;
            }
        }

        [XmlIgnore]
        public AbstractOrganizationalUnit Child
        {
            get
            {
                return (AbstractOrganizationalUnit)this.SecondParty;
            }
            set
            {
                this.SecondParty = value;
            }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        //Serialization
        public OrganizationalRelationship()
        {
        }

        public OrganizationalRelationship( 
            AbstractOrganizationalUnit parent, 
            AbstractOrganizationalUnit child )
            : base( NEW_OID, NEW_VERSION, 
                new RelationshipType( ORGANIZATIONAL_RELATIONSHIP ), 
                parent, child )
        {
        }

        public OrganizationalRelationship( long oid,
            byte[] version,
            AbstractOrganizationalUnit parent, 
            AbstractOrganizationalUnit child ) 
            : base( oid, version )
        {
            this.Type        = new RelationshipType( ORGANIZATIONAL_RELATIONSHIP );
            this.FirstParty  = parent;
            this.SecondParty = child;
        }
        #endregion

        #region Data Elements
        #endregion
    }
}
