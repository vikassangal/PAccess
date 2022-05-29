using System;
using System.Data;
using IBM.Data.DB2.iSeries;
using PatientAccess.Domain;
using PatientAccess.Domain.Specialized;
using log4net;

namespace PatientAccess.Persistence.Specialized
{
    /// <summary>
    /// 
    /// </summary>
    public class ClinicalTrialsAccountPBARBroker : AccountPBARBroker
    {
		#region Constants 

        private const string SQL_QUERY_FOR_CLINICAL_TRIAL_VALUE_ON_ACCOUNT =
            "SELECT LPCL24 FROM HPADLPP WHERE LPHSP# = @P_HSP AND LPACCT = @P_ACCOUNTNUMBER";

		#endregion Constants 

		#region Fields 

        private static readonly ILog Logger =
            LogManager.GetLogger( typeof( ClinicalTrialsAccountPBARBroker ) );

		#endregion Fields 

		#region Methods 

        public override Account AccountFor( BrokerInterfaces.AccountProxy proxy )
        {
            Account anAccount = base.AccountFor( proxy );

            this.HandleIfFacilityIsClinicalTrialEnabled( anAccount.Facility, anAccount );

            return anAccount;
        }

        public override Account AccountFor( BrokerInterfaces.AccountProxy proxy, Activity activity )
        {
            Account anAccount = base.AccountFor( proxy, activity );

            this.HandleIfFacilityIsClinicalTrialEnabled( anAccount.Facility, anAccount );

            return anAccount;
        }
        /// <summary>
        /// Handles if facility is clinical trial enabled.
        /// </summary>
        /// <param name="facility">The facility.</param>
        /// <param name="account">The account.</param>
        /// <remarks>Move this out of the code and into an aspect</remarks>
        // TODO: Externalize this (IOC/AOP)
        private void HandleIfFacilityIsClinicalTrialEnabled( Facility facility, Account account )
        {
            if( !facility.HasExtendedProperty(
                    ClinicalTrialsConstants.KEY_IS_FACILITY_CLINICAL_TRIAL_ENABLED ) )
            {
                return;
            }

            iDB2Command db2Command =
                this.CommandFor(
                    SQL_QUERY_FOR_CLINICAL_TRIAL_VALUE_ON_ACCOUNT,
                    CommandType.Text,
                    facility );

            db2Command.Parameters[PARAM_HSPNUMBER].Value = facility.Oid;
            db2Command.Parameters[PARAM_ACCOUNTNUMBER].Value = account.AccountNumber;

            try
            {
                account[ClinicalTrialsConstants.KEY_IS_ACCOUNT_ELIGIBLE_FOR_CLINICAL_TRIALS] =
                    db2Command.ExecuteScalar().ToString().Trim();
            }
            catch( Exception anyException )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( anyException, Logger );
            }
            finally
            {
                base.Close( db2Command );
            }
        }

		#endregion Methods 
    }
}
