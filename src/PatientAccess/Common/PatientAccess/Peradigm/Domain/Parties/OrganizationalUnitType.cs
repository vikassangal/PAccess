using System;
using Peradigm.Framework.Persistence;

namespace Peradigm.Framework.Domain.Parties
{
    [Serializable]
    public class OrganizationalUnitType : ReferenceValue 
    {
        #region Constants
        public const string DIVISION = "Division";
        public const string MARKET   = "Market";
        public const string REGION   = "Region";
        #endregion

        #region Event Handlers
        #endregion

        #region Methods
        public override int GetHashCode()
        {
            return this.Description.ToLower().GetHashCode(); 
        }

        public bool Equals( OrganizationalUnitType type )
        {
			if( type != null )
			{
				return this.Description.ToLower() == type.Description.ToLower();
			}
			else
			{
				return base.Equals( type );
			}
        }

        public override bool Equals( ReferenceValue obj )
        {
            OrganizationalUnitType type = obj as OrganizationalUnitType;
            if( type != null )
            {
                return this.Equals( type );
            }
            return base.Equals( obj );
        }

        public override bool Equals( object obj )
        {
            OrganizationalUnitType type = obj as OrganizationalUnitType;
            if( type != null )
            {
                return this.Equals( type );
            }
            return base.Equals( obj );
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public OrganizationalUnitType( long oid, byte[] version, string description)
            : base( oid, description, version )
        {
        }

        public OrganizationalUnitType( long oid, string description )
            : base( oid, description, PersistentModel.NEW_VERSION )
        {
        }

        public OrganizationalUnitType( string description )
            : base( PersistentModel.NEW_OID, description, PersistentModel.NEW_VERSION )
        {
        }

        private OrganizationalUnitType()
            : base( PersistentModel.NEW_OID, String.Empty, PersistentModel.NEW_VERSION )
        {
            // This is necessary for serialization
        }
        #endregion

        #region Data Elements
        #endregion
    }
}