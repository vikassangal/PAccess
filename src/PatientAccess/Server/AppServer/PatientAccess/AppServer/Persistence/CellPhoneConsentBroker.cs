using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using log4net;

namespace PatientAccess.Persistence
{
    /// <summary>
    /// Summary description for CellPhoneConsentBroker.
    /// </summary>
    //TODO: Create XML summary comment for CellPhoneConsentBroker
    [Serializable]
    public class CellPhoneConsentBroker : MarshalByRefObject, ICellPhoneConsentBroker
    {
        #region Event Handlers
        #endregion

        #region Methods
        /// <summary>
        /// Get a list of CellPhoneConsent objects including oid, code and description.
        /// </summary>
        public ICollection<CellPhoneConsent> AllCellPhoneConsents()
        {
            try
            {
                ICollection<CellPhoneConsent> cellPhoneConsentList = new Collection<CellPhoneConsent>(CellPhoneConsentList);
                return cellPhoneConsentList;
            }
            catch( Exception e )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom("CellPhoneConsentBroker failed to initialize", e, c_log);
            }
        }

        public CellPhoneConsent ConsentWith(string code)
        {
            CellPhoneConsent returnConsent = null;
            if( code == null )
            {
                throw new ArgumentNullException( "code cannot be null or empty" );
            }
            code = code.Trim().ToUpper();
            try
            {
                ICollection<CellPhoneConsent> cellPhoneConsents = AllCellPhoneConsents();
                foreach (CellPhoneConsent cellPhoneConsent in cellPhoneConsents)
                {
                    if (cellPhoneConsent.Code.Equals(code))
                    {
                        returnConsent = cellPhoneConsent;
                        break;
                    }
                }
                if( returnConsent == null )
                {
                    var cellPhoneConsentInvalid = new CellPhoneConsent(PARAM_OID, DateTime.Now, code, code)
                    {
                        IsValid = false
                    };
                    returnConsent = cellPhoneConsentInvalid;
                }
            }
            catch( Exception e )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( "CellPhone Consent failed to initialize", e, c_log );
            }
            return returnConsent;
        }

        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        #endregion

        #region Data Elements
        private static readonly ILog c_log =
            LogManager.GetLogger(typeof( CellPhoneConsentBroker ));
        #endregion

        #region Constants
        private const int PARAM_OID = 0;

        private static readonly CellPhoneConsent[] CellPhoneConsentList =
        {
            new CellPhoneConsent(PARAM_OID, DateTime.Now, CellPhoneConsent.BLANK, CellPhoneConsent.BLANK),
            new CellPhoneConsent(PARAM_OID, DateTime.Now, CellPhoneConsent.VERBAL_CONSENT_DESCRIPTION, CellPhoneConsent.VERBAL_CONSENT),
            new CellPhoneConsent(PARAM_OID, DateTime.Now, CellPhoneConsent.WRITTEN_CONSENT_DESCRIPTION, CellPhoneConsent.WRITTEN_CONSENT),
            new CellPhoneConsent(PARAM_OID, DateTime.Now, CellPhoneConsent.DECLINE_CONSENT_DESCRIPTION, CellPhoneConsent.DECLINE_CONSENT),
            new CellPhoneConsent(PARAM_OID, DateTime.Now, CellPhoneConsent.NO_CONSENT_DESCRIPTION, CellPhoneConsent.NO_CONSENT),
            new CellPhoneConsent(PARAM_OID, DateTime.Now, CellPhoneConsent.ELECTRONIC_CONSENT_DESCRIPTION, CellPhoneConsent.DELIVARY_CONSENT),
            new CellPhoneConsent(PARAM_OID, DateTime.Now, CellPhoneConsent.REVOKE_CONSENT_DESCRIPTION, CellPhoneConsent.REVOKE_CONSENT)

        };

        #endregion
    }

}
