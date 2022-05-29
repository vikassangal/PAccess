using System;
using System.Data;
using Extensions.DB2Persistence;
using IBM.Data.DB2.iSeries;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using log4net;

namespace PatientAccess.Persistence
{
	/// <summary>
	/// Summary description for PBARSecurityCodeBroker.
	/// </summary>
    [Serializable]
	public class PBARSecurityCodeBroker : AbstractPBARBroker, ISecurityCodeBroker
	{
		
        #region Event Handlers
        #endregion

        #region Methods

	    /// <summary>
	    /// Returns printed security code based on Employee ID.
	    /// </summary>
	    /// <param name="PBAREmployeeId">Employee Id</param>
	    /// <param name="facility"></param>
	    /// <returns>Printed security code</returns>
	    public string GetPrintedSecurityCode( string PBAREmployeeId, Facility facility )
        {
            string printedSecurityCode = string.Empty;
            iDB2Command command = null;
            SafeReader reader = null;

            try
            {
                command = this.CommandFor( "CALL " + SP_SELECT_SECURITY_CODE_FOR_PBARID + 
                    "(" + PARAM_PBARID + ")",
                    CommandType.Text, facility );

                command.Parameters[PARAM_PBARID].Value = PBAREmployeeId;

                reader = this.ExecuteReader( command );
                if ( reader.Read() )
                {
                    printedSecurityCode = reader.GetString( COL_SECURITY_CODE );
                } 
            }
            catch( Exception e )
            {
                string errorMessage = "GetPrintedSecurityCode method failed with an unknown error.";
                throw BrokerExceptionFactory.BrokerExceptionFrom( errorMessage, e, c_log );
            }
            finally
            {
                base.Close( reader );
                base.Close( command );
            }
            return printedSecurityCode;
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public PBARSecurityCodeBroker()
        {
        }
        public PBARSecurityCodeBroker(string cxnString)
            : base( cxnString )
        {
        }
        public PBARSecurityCodeBroker(IDbTransaction txn )
            : base( txn )
        {
        }
        #endregion

        #region Data Elements
        private static readonly ILog c_log =
            LogManager.GetLogger( typeof( PBARSecurityCodeBroker ) );
        #endregion

        #region Constants
        private const string
            SP_SELECT_SECURITY_CODE_FOR_PBARID = "SELECTSECURITYCODEFORPBARID";

        private const string
            PARAM_PBARID = "@P_PBARID";

        private const string
            COL_SECURITY_CODE = "SECURITYCODE";

        #endregion
	}
}
