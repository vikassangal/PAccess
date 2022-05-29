using System;
using System.Collections;
using System.Collections.Generic;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Utilities;

namespace PatientAccess.BrokerProxies
{
    [Serializable]
    public class AddressBrokerProxy : AbstractBrokerProxy, IAddressBroker
    {
        #region Event Handlers
        #endregion

        #region Methods

        public IList AllCountries(long facilityID)
        {
            var cacheKey = "ADDRESS_BROKER_PROXY_ALL_COUNTRIES_AND_FACILITY_" + facilityID;
            IList allCountries = (IList)this.Cache[cacheKey];

            if( allCountries == null )
            {
                lock (cacheKey)
                {
                    allCountries = this.i_addressBroker.AllCountries( facilityID );
                    if (this.Cache[cacheKey] == null)
                    {
                        this.Cache.Insert(cacheKey, allCountries);
                    }
                }
            }
            
            return allCountries;
        }

        public ArrayList ContactPointsForPatient( string facilityCode, long medicalRecordNumber )
        {
            return this.i_addressBroker.ContactPointsForPatient( facilityCode, medicalRecordNumber );
        }

		// CodeReview: This method should first evaluate the cached collection; if the code is not found, then pass thru to the broker

        public Country CountryWith( long facilityID, string code )
        {
            return this.i_addressBroker.CountryWith(facilityID, code );
        }

        public Country CountryWith( string code, Facility aFacility )
        {
            return i_addressBroker.CountryWith( code, aFacility );
        }

        public ICollection AllCountiesFor(long facilityID)
        {

            ICollection allCounties = 
                (IList)this.Cache[string.Format("Facility-{0}-{1}",facilityID,ADDRESS_BROKER_PROXY_ALL_COUNTIES)];

            if (allCounties == null)
            {
                lock (ADDRESS_BROKER_PROXY_ALL_COUNTIES)
                {
                    allCounties = this.i_addressBroker.AllCountiesFor(facilityID);
                    if (this.Cache[ADDRESS_BROKER_PROXY_ALL_COUNTIES] == null)
                    {
                        this.Cache.Insert(ADDRESS_BROKER_PROXY_ALL_COUNTIES, allCounties);
                    }
                }
            }

            return allCounties;
        }

        public County CountyWith( long facilityNumber, string stateCode, string code )
        {
            County selectedCounty = null;
            if ( null == code )
            {
                throw new ArgumentNullException( "Argument, code, should not be null." );
            }
			
            if (null == stateCode)
            {
                throw new ArgumentNullException("Argument, state code, should not be null.");
            }

            // Look for the County in the Cached collection first
            ArrayList counties = ( ArrayList )this.AllCountiesFor( facilityNumber );
            foreach ( County county in counties )
            {
                if ( county.Code.Equals( code ) && county.StateCode.Equals(stateCode))
                {
                    selectedCounty = county;
                    break;
                }
            }

            // If County is not found in Cache, use broker to retrieve it from the database
            if ( selectedCounty == null )
            {
                return this.i_addressBroker.CountyWith( facilityNumber,stateCode, code );
            }

            return selectedCounty;
        }

        public IList<County> GetCountiesForAState(string stateCode, long facilityNumber)
        {
            Guard.ThrowIfArgumentIsNullOrEmpty(stateCode,"stateCode");

            var cacheKey = "ADDRESS_BROKER_PROXY_COUNTIES_FOR_STATE_" + stateCode + "_AND_FACILITY_" + facilityNumber;
            
            var countiesForAState = (IList<County>)Cache[cacheKey];

            if ( countiesForAState == null )
            {
                lock ( cacheKey )
                {
                    countiesForAState = i_addressBroker.GetCountiesForAState(stateCode, facilityNumber);;
                    
                    if ( Cache[cacheKey] == null )
                    {
                        Cache.Insert( cacheKey, countiesForAState );
                    }
                }
            }

            return countiesForAState;
        }
        
        public IList AllStates(long facilityID)
        {
            var cacheKey = "ADDRESS_BROKER_PROXY_ALL_STATES_AND_FACILITY_" + facilityID;
            IList allStates = (IList)this.Cache[cacheKey];

            if (allStates == null)
            {
                lock (cacheKey)
                {
                    allStates =(IList) this.i_addressBroker.AllStates(facilityID);
                    if (this.Cache[cacheKey] == null)
                    {
                        this.Cache.Insert(cacheKey, allStates);
                    }
                }
            }

            return allStates;
        }

        public IList<State> AllUSStates(long facilityID)
        {
            var cacheKey = "ADDRESS_BROKER_PROXY_ALL_US_STATES_AND_FACILITY_" + facilityID;
            var allUSStates = (IList<State>)this.Cache[cacheKey];

            if ( allUSStates == null )
            {
                lock (cacheKey)
                {
                    allUSStates = i_addressBroker.AllUSStates(facilityID);

                    if (Cache[cacheKey] == null)
                    {
                        Cache.Insert(cacheKey, allUSStates);
                    }
                }
            }

            return allUSStates;
        }

        public IList<State> AllNonUSStates(long facilityID)
        {
            var cacheKey = "ADDRESS_BROKER_PROXY_ALL_NON_US_STATES_AND_FACILITY_" + facilityID;
            var nonUSStates = (IList<State>)this.Cache[cacheKey];

            if ( nonUSStates == null )
            {
                lock (cacheKey)
                {
                    nonUSStates = i_addressBroker.AllNonUSStates(facilityID);

                    if (Cache[cacheKey] == null)
                    {
                        Cache.Insert(cacheKey, nonUSStates);
                    }
                }
            }

            return nonUSStates;
        }

        public State StateWith(long facilityID, string code)
        {
            return this.i_addressBroker.StateWith(facilityID, code );
        }
        
        public State StateWith( string code, Facility aFacility )
        {
            return i_addressBroker.StateWith( code, aFacility );
        }

        public IList AllZipCodeStatuses()
        {
            IList allZipCodeStatuses = (IList)this.Cache[ADDRESS_BROKER_PROXY_ALL_ZIPCODE_STATUSES];

            if( allZipCodeStatuses == null )
            {
                lock( ADDRESS_BROKER_PROXY_ALL_ZIPCODE_STATUSES )
                {
                    allZipCodeStatuses = ZipCodeStatus.AllZipCodeStatuses() as IList;
                    if (this.Cache[ADDRESS_BROKER_PROXY_ALL_ZIPCODE_STATUSES] == null)
                    {
                        this.Cache.Insert(ADDRESS_BROKER_PROXY_ALL_ZIPCODE_STATUSES, allZipCodeStatuses);
                    }
                }
            }
            
            return allZipCodeStatuses;
        }        

        public void SaveEmployerAddress( Employer employer, string facilityCode )
        {
            this.i_addressBroker.SaveEmployerAddress( employer, facilityCode );
        }

        public void SaveNewEmployerAddressForApproval( Employer employer, string facilityCode )
        {
            this.i_addressBroker.SaveNewEmployerAddressForApproval( employer, facilityCode ) ;
        }

        public ArrayList ContactPointsForGuarantor(string facilityCode, long accountNumber)
        {
            throw new Exception("This method is not implemented in proxy");
        }

        public ArrayList ContactPointsForEmployer(string facilityCode, long employerCode)
        {
            throw new Exception("This method is not implemented in proxy");
        }

        public ArrayList ContactPointsForEmployerApproval( string facilityCode, long employerCode )
        {
            throw new NotImplementedException();
        }


        public void DeleteEmployerAddressForApproval( Employer employer, string facilityHspCode )
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public AddressBrokerProxy()
        {
        }
        #endregion

        #region Data Elements
        private IAddressBroker i_addressBroker = BrokerFactory.BrokerOfType< IAddressBroker >() ;
        #endregion

        #region Constants
        private const string
            ADDRESS_BROKER_PROXY_ALL_COUNTIES           = "ADDRESS_BROKER_PROXY_ALL_COUNTIES",
            ADDRESS_BROKER_PROXY_ALL_ZIPCODE_STATUSES   = "ADDRESS_BROKER_PROXY_ALL_ZIPCODE_STATUSES";
        #endregion
    }
}
