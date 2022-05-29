using System;
using System.Collections;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using log4net;

namespace PatientAccess.Persistence
{
	/// <summary>
	/// Summary description for InfoReceivedSourceBroker.
	/// </summary>
    [Serializable]
    public class InfoReceivedSourceBroker : MarshalByRefObject, IInfoReceivedSourceBroker
    {
        #region Constants
        #endregion

        #region Event Handlers
        #endregion

        #region Methods
    
        /// <summary>
        /// Get a list of all InfoReceivedSource objects.
        /// </summary>
        /// <returns></returns>
        public ICollection AllInfoReceivedSources()
        {
            ICollection infoReceivedSources = null;

            try
            {
                infoReceivedSources = this.GetInfoReceivedSource();
                return infoReceivedSources;
            }
            catch( Exception e )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( "InfoReceivedSourceBroker failed to initialize", e, c_log );
            }
          
        }

	    /// <summary>
	    /// Get one InformationReceivedSource object based on the oid.
	    /// </summary>
	    /// <param name="code"></param>
	    /// <returns></returns>
	    public InformationReceivedSource InfoReceivedSourceWith( string code )
        {
            InformationReceivedSource informationReceivedSourceVal = null;
            if( code == null )
            {
                throw new ArgumentNullException( "code cannot be null or empty" );
            }
            code = code.Trim();
            try
            {
                ICollection informationReceivedSourceList = this.AllInfoReceivedSources();
                foreach( InformationReceivedSource informationReceivedSource in informationReceivedSourceList )
                {
                    if( informationReceivedSource.Code.Equals( code ) )
                    {
                        informationReceivedSourceVal = informationReceivedSource;
                        break;
                    }
                }
                if( informationReceivedSourceVal == null )
                {
                    informationReceivedSourceVal = new InformationReceivedSource( PARAM_OID, DateTime.Now, string.Empty, 
                        InformationReceivedSource.BLANK_VERIFICATION_OID.ToString() );
                    informationReceivedSourceVal.IsValid = false;
                }
            }
            catch( Exception e )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( "InfoReceivedSourceBroker failed to initialize", e, c_log );
            }
            return informationReceivedSourceVal;
        }
        
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        /// <summary>
        /// Populate InfoReceived Source
        /// </summary>
        /// <returns></returns>
        private ICollection GetInfoReceivedSource()
        {
            ArrayList infoReceivedSourceList = new ArrayList();
            infoReceivedSourceList.Add(
                new InformationReceivedSource( PARAM_OID, DateTime.Now, 
                    string.Empty,InformationReceivedSource.BLANK_VERIFICATION_OID.ToString() ) );
            infoReceivedSourceList.Add(
              new InformationReceivedSource( PARAM_OID, DateTime.Now,
                    "Phone",InformationReceivedSource.PHONE_VERIFICATION_OID.ToString() ) );
            infoReceivedSourceList.Add(
              new InformationReceivedSource( PARAM_OID, DateTime.Now, 
                    "System electronic verification",InformationReceivedSource.SYSTEM_VERIFICATION_OID.ToString() ) );
            infoReceivedSourceList.Add(
              new InformationReceivedSource( PARAM_OID, DateTime.Now, 
                    "Other electronic verification",InformationReceivedSource.OTHER_VERIFICATION_OID.ToString() ) );
            return infoReceivedSourceList;
        }
         
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public InfoReceivedSourceBroker() : base()
        {
        }

        #endregion

        #region Data Elements
        private static readonly ILog c_log = 
            LogManager.GetLogger( typeof( InfoReceivedSourceBroker ) );
        #endregion

        #region Constants
        private const int PARAM_OID = 0;
        #endregion
    }

}
