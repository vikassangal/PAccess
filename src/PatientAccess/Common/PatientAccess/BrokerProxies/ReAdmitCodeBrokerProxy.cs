using System;
using System.Collections;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;

namespace PatientAccess.BrokerProxies
{
	/// <summary>
	/// Summary description for ReAdmitCodeBrokerProxy.
	/// </summary>
	//TODO: Create XML summary comment for ReAdmitCodeBrokerProxy
    [Serializable]
    public class ReAdmitCodeBrokerProxy : AbstractBrokerProxy, IReAdmitCodeBroker
    {
        #region Event Handlers
        #endregion

        #region Methods
        public ICollection ReAdmitCodesFor( long facilityNumber )
        {
            ICollection reAdmitCodes = (ICollection)this.Cache[READMIT_CODE_BROKER_READMIT_CODE_FOR];

            if( reAdmitCodes == null )
            {
                lock( READMIT_CODE_BROKER_READMIT_CODE_FOR )
                {
                    reAdmitCodes = i_ReAdmitCodeBroker.ReAdmitCodesFor(facilityNumber);                       
                    if( this.Cache[READMIT_CODE_BROKER_READMIT_CODE_FOR] == null )
                    {
                        this.Cache.Insert( READMIT_CODE_BROKER_READMIT_CODE_FOR, 
                            reAdmitCodes );
                    }
                }
            }
            
            return reAdmitCodes;
        }
        public ReAdmitCode ReAdmitCodeWith(long facilityNumber, string code)
        {
            return i_ReAdmitCodeBroker.ReAdmitCodeWith(facilityNumber, code);
        }
       
	    #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public ReAdmitCodeBrokerProxy()
        {
        }
        #endregion

        #region Data Elements
        private IReAdmitCodeBroker i_ReAdmitCodeBroker = BrokerFactory.BrokerOfType< IReAdmitCodeBroker >() ;
        #endregion

        #region Constants
        
        private string READMIT_CODE_BROKER_READMIT_CODE_FOR = "READMIT_CODE_BROKER_READMIT_CODE_FOR";
        
        #endregion
    }
}
