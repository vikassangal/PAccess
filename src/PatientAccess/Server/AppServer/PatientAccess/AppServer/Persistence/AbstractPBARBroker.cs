using System;
using System.Data;
using Extensions.DB2Persistence;
using IBM.Data.DB2.iSeries;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;

namespace PatientAccess.Persistence
{
    /// <summary>
    /// Summary description for AbstractPBARBroker.
    /// </summary>
    //TODO: Create XML summary comment for AbstractPBARBroker
    [Serializable]
    public class AbstractPBARBroker : CachingBroker
    {
        #region Event Handlers
        #endregion

        #region Methods
        protected override iDB2Command CommandFor(string cmdText, CommandType cmdType)
        {
            throw new BrokerException("This method is no longer valid");
        }
        public iDB2Connection NewConnection(Facility facility)
        {
            this.SetConnectionString(facility);
            iDB2Connection conn = NewConnection();

            return conn;
        }
        public iDB2Command CommandFor(string cmdText, CommandType cmdType, Facility facility)
        {
            this.SetConnectionString(facility);
            iDB2Command cmd = base.CommandFor(cmdText, cmdType);
            return cmd;
        }

        public iDB2Transaction NewTransaction(Facility facility)
        {
            this.SetConnectionString(facility);
            iDB2Transaction txn = NewTransaction();

            return txn;
        }

        #endregion

        #region Properties
        #endregion

        #region Private Methods
        private void SetConnectionString(Facility facility)
        {
            this.ConnectionString = facility.ConnectionSpec.ConnectionString;
        }
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public AbstractPBARBroker()
        {
        }
        public AbstractPBARBroker(string cxnString)
            : base(cxnString)
        {
        }
        public AbstractPBARBroker(IDbTransaction txn)
            : base(txn)
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        const string PBAR_UNAVAILABLE_MESSAGE = "PBAR Service is Unavailable.";
        #endregion
    }
}
