using System;
using System.Collections;
using System.Collections.Generic;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;

namespace PatientAccess.BrokerProxies
{
	/// <summary>
	/// Summary description for SSNBrokerProxy.
	/// </summary>
	//TODO: Create XML summary comment for SSNBrokerProxy
    [Serializable]
    public class SSNBrokerProxy : AbstractBrokerProxy, ISSNBroker
    {
        #region Event Handlers
        #endregion

        #region Methods

        public SocialSecurityNumberStatus SSNStatusWith(long facilityNumber, string description)
        {
            return i_ssnBroker.SSNStatusWith( facilityNumber, description );
        }

        public ArrayList SSNStatuses(long facilityNumber, string stateCode)
        {
            ArrayList statuses = new ArrayList();
            Hashtable ssnStatusByStateHashtable = this.SSNStatusByStateHashtable;

            if( ssnStatusByStateHashtable[stateCode] == null )
            {
                lock( CACHE_SSN_STATUS_BY_STATE_HASHTABLE )
                {
                    // Ensure that we are still null before loading from the real broker
                    if( ssnStatusByStateHashtable[stateCode] == null )
                    {
                        statuses = i_ssnBroker.SSNStatuses(facilityNumber, stateCode);
                        ssnStatusByStateHashtable[stateCode] = statuses;
                    }
                    else
                    {
                        // The facility flags were loaded on another thread before our double-check
                        statuses = (ArrayList)ssnStatusByStateHashtable[stateCode];
                    }
                }
            }
            else
            {
                // FacilityFlags for this facility were already loaded.
                statuses = (ArrayList)ssnStatusByStateHashtable[stateCode];
            }
            return statuses;
        }

        public IList<SocialSecurityNumberStatus> SSNStatuses()
        {
            return i_ssnBroker.SSNStatuses();
        }

        public ArrayList SSNStatusesForGuarantor(long facilityNumber, string stateCode)
        {
            ArrayList statuses = new ArrayList();
            Hashtable ssnStatusForGuarantorByStateHashtable = this.SSNStatusForGuarantorByStateHashtable;

            if( ssnStatusForGuarantorByStateHashtable[stateCode] == null )
            {
                lock( CACHE_SSN_STATUS_FOR_GUARANTOR_BY_STATE_HASHTABLE )
                {
                    // Ensure that we are still null before loading from the real broker
                    if( ssnStatusForGuarantorByStateHashtable[stateCode] == null )
                    {
                        statuses = i_ssnBroker.SSNStatusesForGuarantor(facilityNumber, stateCode);
                        ssnStatusForGuarantorByStateHashtable[stateCode] = statuses;
                    }
                    else
                    {
                        // The ssnStatuses flags were loaded on another thread before our double-check
                        statuses = (ArrayList)ssnStatusForGuarantorByStateHashtable[stateCode];
                    }
                }
            }
            else
            {
                // ssnStatuses for this facility were already loaded.
                statuses = (ArrayList)ssnStatusForGuarantorByStateHashtable[stateCode];
            }
            return statuses;
        }

        #endregion

        #region Properties
        #endregion

        #region Private Methods
       private Hashtable SSNStatusByStateHashtable
        {
            get
            {
                Hashtable ssnStatusBystateHashTable = null;
                if ((ssnStatusBystateHashTable = (Hashtable)Cache[CACHE_SSN_STATUS_BY_STATE_HASHTABLE]) == null)
                {
                    lock (CACHE_SSN_STATUS_BY_STATE_HASHTABLE)
                    {
                        ssnStatusBystateHashTable = new Hashtable();
                        this.Cache.Insert(CACHE_SSN_STATUS_BY_STATE_HASHTABLE, ssnStatusBystateHashTable);
                    }
                }
                return ssnStatusBystateHashTable;
                
            }
        }

        private Hashtable SSNStatusForGuarantorByStateHashtable
        {
            get
            {
                Hashtable ssnStatusforGuarantorTable = null;
                if ((ssnStatusforGuarantorTable = (Hashtable)Cache[CACHE_SSN_STATUS_FOR_GUARANTOR_BY_STATE_HASHTABLE]) == null)
                {
                    lock (CACHE_SSN_STATUS_FOR_GUARANTOR_BY_STATE_HASHTABLE)
                    {
                        ssnStatusforGuarantorTable = new Hashtable();
                        this.Cache.Insert(CACHE_SSN_STATUS_FOR_GUARANTOR_BY_STATE_HASHTABLE, ssnStatusforGuarantorTable);
                    }
                }
                return ssnStatusforGuarantorTable;
            }
        }
        
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public SSNBrokerProxy()
        {
        }
        #endregion

        #region Data Elements
        private ISSNBroker i_ssnBroker = BrokerFactory.BrokerOfType< ISSNBroker >() ;
        #endregion

        #region Constants
        private const string CACHE_SSN_STATUS_BY_STATE_HASHTABLE = "CACHE_SSN_STATUS_BY_STATE_HASHTABLE";
        private const string CACHE_SSN_STATUS_FOR_GUARANTOR_BY_STATE_HASHTABLE = "CACHE_SSN_STATUS_FOR_GUARANTOR_BY_STATE_HASHTABLE";
        private const long ACO_FACILITY = 900 ;
        #endregion
    }
}
