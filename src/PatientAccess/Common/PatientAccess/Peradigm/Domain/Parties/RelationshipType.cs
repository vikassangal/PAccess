using System;
using Peradigm.Framework.Persistence;

namespace Peradigm.Framework.Domain.Parties
{
    //TODO: Create XML summary comment for RelationshipType
    [Serializable]
    public class RelationshipType : ReferenceValue
    {
        #region Constants
        #endregion

        #region Event Handlers
        #endregion

        #region Methods
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
		public RelationshipType( long oid, string description, byte[] version )
			: base( oid, description, version )
		{
		}

		public RelationshipType( long oid, string description )
			: base( oid, description )
		{
		}

		public RelationshipType( string description )
			: base( description )
		{
		}

		public RelationshipType() : base()
		{
		}
        #endregion

        #region Data Elements
        #endregion
    }
}
