using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using log4net;

namespace PatientAccess.Persistence
{
    /// <summary>
    /// Summary description for LeftOrStayedBroker.
    /// </summary>
    //TODO: Create XML summary comment for LeftOrStayedBroker
    [Serializable]
    public class LeftOrStayedBroker : MarshalByRefObject, ILeftOrStayedBroker
    {
        #region Event Handlers
        #endregion

        #region Methods
        /// <summary>
        /// Get a list of LeftOrStayed objects including oid, code and description.
        /// </summary>
        public ICollection<LeftOrStayed> AllLeftOrStayed()
        {
            ICollection<LeftOrStayed> leftOrStayedList;
            try
            {
                leftOrStayedList = new Collection<LeftOrStayed>( LeftOrStayedList );
                return  leftOrStayedList;
            }
            catch( Exception e )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( "LeftOrStayedBroker failed to initialize", e, c_log );
            }
        }

        public LeftOrStayed LeftOrStayedWith( string code )
        {
            LeftOrStayed returnLeftOrStayed = null;
            if( code == null )
            {
                throw new ArgumentNullException( "code cannot be null or empty" );
            }
            code = code.Trim().ToUpper();
            try
            {
                ICollection<LeftOrStayed> LeftOrStayedCollection = this.AllLeftOrStayed();
                foreach ( LeftOrStayed LeftOrStayed in LeftOrStayedCollection )
                {
                    if( LeftOrStayed.Code.Equals( code ) )
                    {
                        returnLeftOrStayed = LeftOrStayed;
                        break;
                    }
                }
                if ( returnLeftOrStayed == null )
                {
                    LeftOrStayed LeftOrStayedInvalid = new LeftOrStayed(  PARAM_OID, DateTime.Now, code, code );
                    LeftOrStayedInvalid.IsValid = false;
                    returnLeftOrStayed = LeftOrStayedInvalid;
                }
            }
            catch( Exception e )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( "LeftOrStayedBroker failed to initialize", e, c_log );
            }
            return returnLeftOrStayed;
        }


        public LeftOrStayed LeftOrStayedWith( long id )
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
        public LeftOrStayedBroker()
        {
        }

       #endregion

        #region Data Elements
        private static readonly ILog c_log =
            LogManager.GetLogger( typeof( LeftOrStayedBroker ) );
        #endregion

        #region Constants
        private const int PARAM_OID = 0;

        private static readonly LeftOrStayed[] LeftOrStayedList = new[] { 
                 new LeftOrStayed( PARAM_OID, DateTime.Now, LeftOrStayed.BLANK, LeftOrStayed.BLANK ),
                 new LeftOrStayed( PARAM_OID, DateTime.Now, LeftOrStayed.LEFT_DESCRIPTION, LeftOrStayed.LEFT ),
                  new LeftOrStayed( PARAM_OID, DateTime.Now, LeftOrStayed.STAYED_DESCRIPTION, LeftOrStayed.STAYED )
                 };
        #endregion
    }

}
