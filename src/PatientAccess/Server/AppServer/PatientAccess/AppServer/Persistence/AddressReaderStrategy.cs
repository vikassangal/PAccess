using System;
using System.Data;
using Extensions.DB2Persistence;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Persistence
{
    [Serializable]
    public abstract class AddressReaderStrategy : AbstractBroker
    {
        #region Event Handlers
        #endregion

        #region Methods
//        public abstract void LoadAddressOn( Party aParty );
//
//        public abstract void LoadAddressesOn( Party aParty, SafeReader fromReader );
        public abstract void LoadContactPointOn( Party aParty, SafeReader fromReader, long facilityID );
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public AddressReaderStrategy() : base()
        {
        }

        public AddressReaderStrategy( string cxnString ) : base( cxnString )
        {
        }

        public AddressReaderStrategy( IDbTransaction txn ) : base( txn )
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }

}
