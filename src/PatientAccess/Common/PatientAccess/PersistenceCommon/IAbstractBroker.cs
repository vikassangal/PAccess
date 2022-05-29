using System.Data;

namespace Extensions.PersistenceCommon
{
    public interface IAbstractBroker
    {
        /// <summary>
        /// Provides the default connection string used by the broker.
        /// </summary>
        string ConnectionString
        {
            get;
            set;
        }
        /// <summary>
        /// Transaction currently associated with the broker if one is present.
        /// </summary>
        IDbTransaction Transaction
        {
            get;
            set;
        }
    }
}
