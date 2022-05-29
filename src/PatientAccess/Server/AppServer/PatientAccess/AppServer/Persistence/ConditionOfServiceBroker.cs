using System;
using System.Collections;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using log4net;

namespace PatientAccess.Persistence
{
    /// <summary>
    /// Summary description for ConditionOfServiceBroker.
    /// </summary>
    //TODO: Create XML summary comment for ConditionOfServiceBroker
    [Serializable]
    public class ConditionOfServiceBroker : MarshalByRefObject, IConditionOfServiceBroker
    {
        #region Event Handlers
        #endregion

        #region Methods
        /// <summary>
        /// Get a list of ConditionsOfService objects including oid, code and description.
        /// </summary>
        public ICollection AllConditionsOfService()
        {
            try
            {
                ICollection conditionOfServiceList = new ArrayList( ConditionOfServiceList );
                return conditionOfServiceList;
            }
            catch( Exception e )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( "ConditionOfServiceBroker failed to initialize", e, c_log );
            }
        }

        public ConditionOfService ConditionOfServiceWith( string code )
        {
            ConditionOfService returnCos = null;
            if( code == null )
            {
                throw new ArgumentNullException( "code cannot be null or empty" );
            }
            code = code.Trim().ToUpper();
            try
            {
                ICollection conditionOfServices = this.AllConditionsOfService();
                foreach( ConditionOfService conditionOfService in conditionOfServices )
                {
                    if( conditionOfService.Code.Equals( code ) )
                    {
                        returnCos = conditionOfService;
                        break;
                    }
                }
                if( returnCos == null )
                {
                    ConditionOfService conditionOfServiceInvalid = new ConditionOfService( PARAM_OID, DateTime.Now, code, code );
                    conditionOfServiceInvalid.IsValid = false;
                    returnCos = conditionOfServiceInvalid;
                }
            }
            catch( Exception e )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( "ConditionOfServiceBroker failed to initialize", e, c_log );
            }
            return returnCos;
        }


        public ConditionOfService ConditionOfServiceWith( long id )
        {
            throw new BrokerException( "This method not implemented in DB2 Version" );
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public ConditionOfServiceBroker()
        {
        }

       #endregion

        #region Data Elements
        private static readonly ILog c_log =
            LogManager.GetLogger( typeof( ConditionOfServiceBroker ) );
        #endregion

        #region Constants
        private const int PARAM_OID = 0;

        private static readonly ConditionOfService[] ConditionOfServiceList = new[] { 
                 new ConditionOfService( PARAM_OID, DateTime.Now, ConditionOfService.BLANK, ConditionOfService.BLANK ),
                 new ConditionOfService( PARAM_OID, DateTime.Now, ConditionOfService.YES_DESCRIPTION, ConditionOfService.YES ),
                 new ConditionOfService( PARAM_OID, DateTime.Now, ConditionOfService.UNABLE_DESCRIPTION, ConditionOfService.UNABLE ),
                 new ConditionOfService( PARAM_OID, DateTime.Now, ConditionOfService.REFUSED_DESCRIPTION, ConditionOfService.REFUSED ),
                 new ConditionOfService( PARAM_OID, DateTime.Now, ConditionOfService.NOT_AVAILABLE_DESCRIPTION, ConditionOfService.NOT_AVAILABLE )
            };
        #endregion
    }

}
