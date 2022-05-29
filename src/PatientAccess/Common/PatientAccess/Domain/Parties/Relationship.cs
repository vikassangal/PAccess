using System;
using Extensions.PersistenceCommon;

namespace PatientAccess.Domain.Parties
{
    [Serializable]
    public class Relationship : PersistentModel
    {
        #region Event Handlers
        #endregion

        #region Methods

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals( object obj )
        {
            Relationship aRelationship = obj as Relationship;
            if( aRelationship != null )
            {
                return this.Equals( aRelationship );
            }
            return base.Equals( obj );
        }

        public bool Equals( Relationship anotherRelationship )
        {
            bool result = false;
            if( this.Type != null && this.Type.Code != null &&
                anotherRelationship.Type != null && anotherRelationship.Type.Code != null &&
                this.InitiatingPartyType != null && anotherRelationship.InitiatingPartyType != null &&
                this.ReciprocatingPartyType != null && anotherRelationship.ReciprocatingPartyType != null )
            {
                result = this.Type.Code == anotherRelationship.Type.Code &&
                    this.InitiatingPartyType == anotherRelationship.InitiatingPartyType &&
                    this.ReciprocatingPartyType == anotherRelationship.ReciprocatingPartyType;
            }
            return result;
        }

        #endregion

        #region Properties

        private Type InitiatingPartyType
        {
            get
            {
                return i_InitiatingPartyType;
            }
            set
            {
                i_InitiatingPartyType = value;
            }
        }

        public Type ReciprocatingPartyType
        {
            get
            {
                return i_ReciprocatingPartyType;
            }
            private set
            {
                i_ReciprocatingPartyType = value;
            }
        }

        public RelationshipType Type
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
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public Relationship()
        {
        }

        public Relationship( RelationshipType type, 
            Type firstPartyType, 
            Type secondPartyType )
            : this( NEW_OID, NEW_VERSION, type, firstPartyType, secondPartyType )
        {
        }

        private Relationship( long oid,
            DateTime version,
            RelationshipType type, 
            Type firstPartyType,
            Type secondPartyType ) : base( oid, version )
        {
            this.Type        = type;
            this.InitiatingPartyType  = firstPartyType;
            this.ReciprocatingPartyType = secondPartyType;
        }
        #endregion

        #region Data Elements

        private Type                            i_InitiatingPartyType;
        private Type                            i_ReciprocatingPartyType;
        private RelationshipType                i_Type = new RelationshipType();

        #endregion

        #region Constants
        #endregion
    }
}
