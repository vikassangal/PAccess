using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;

namespace PatientAccess.BrokerProxies
{
	/// <summary>
	/// Summary description for LocationBrokerProxy.
	/// </summary>
	//TODO: Create XML summary comment for LocationBrokerProxy
    [Serializable]
    public class LocationBrokerProxy :  AbstractBrokerProxy, ILocationBroker
    {
        #region Event Handlers
        #endregion

        #region Methods

        public Accomodation AccomodationFor( long facilityNumber, long oid )
        {
            return i_LocationBroker.AccomodationFor( facilityNumber, oid );
        }

        public IList AccomodationsFor( long facilityNumber )
        {
            return i_LocationBroker.AccomodationsFor( facilityNumber );
        }

        public ReservationResults Reserve( ReservationCriteria reservationCriteria )
        {
            return i_LocationBroker.Reserve( reservationCriteria );
        }

        public void ReleaseReservedBed( Location location, Facility aFacility )
        {
            i_LocationBroker.ReleaseReservedBed( location, aFacility );
        }

        public ICollection AccomodationCodesFor( string nursingStationCode, Facility aFacility )
        {
            return GetAccomodationcodesFor( aFacility, nursingStationCode );
        }

        public string GetEntireNursingStationCode(Facility facility, string nsCode)
        {
            return i_LocationBroker.GetEntireNursingStationCode( facility, nsCode);
        }

        private ICollection GetAccomodationcodesFor( Facility aFacility, string nursingStationCode )
        {
            ICollection accomodationCodes = null;
            
            //Create a unique key
            string facilityNusingStationKey = GetFacilityNursingStationKey( 
                aFacility.Code, nursingStationCode );

             //Get the hashTable out of the cache
            Hashtable accomodationCodesHashTable =
                    (Hashtable)this.Cache[LOCATION_PBAR_BROKER_PROXY_ACCOMODATION_CODES_FOR];

            if( accomodationCodesHashTable != null )
            {
                //Get the collection out of the hashTable
                accomodationCodes = (ICollection) accomodationCodesHashTable[facilityNusingStationKey];
            }
                
            if( accomodationCodes == null )
            {
                accomodationCodes = i_LocationBroker.AccomodationCodesFor( 
                    nursingStationCode, aFacility );   

                AddAccomodationCodesToCache( facilityNusingStationKey, accomodationCodes );
            }
           
            return accomodationCodes;
        }

        private void AddAccomodationCodesToCache( string key, ICollection accomodationCodes )
        {           
            lock( LOCATION_PBAR_BROKER_PROXY_ACCOMODATION_CODES_FOR )
            {
                Hashtable accomodationCodesHashTable =
                    (Hashtable)this.Cache[LOCATION_PBAR_BROKER_PROXY_ACCOMODATION_CODES_FOR];
                if (accomodationCodesHashTable == null)
                {
                    accomodationCodesHashTable  = new Hashtable( );
                    accomodationCodesHashTable.Add( key, accomodationCodes );
                    this.Cache.Insert( LOCATION_PBAR_BROKER_PROXY_ACCOMODATION_CODES_FOR, accomodationCodesHashTable );
                }
                else
                {
                    //adding or updating key and data
                    accomodationCodesHashTable[key] = accomodationCodes;
                }
            }
        }

        private string GetFacilityNursingStationKey( 
            string facilityCode, string nursingStationCode )
        {
            StringBuilder keyBuilder = new StringBuilder();
            keyBuilder.Append( facilityCode );
            keyBuilder.Append( "-" );
            keyBuilder.Append( nursingStationCode );
            return keyBuilder.ToString( );
        }

        public ICollection LocationMatching( LocationSearchCriteria locationSearchCriteria )
        {
            return i_LocationBroker.LocationMatching( locationSearchCriteria );
        }

		// CodeReview: Rooms could be cached client-side

        public ICollection RoomsFor( Facility facility, string nursingStationCode )
        {
            return i_LocationBroker.RoomsFor( facility, nursingStationCode );
        }

		// CodeReview: NursingStations could be cached client-side

        public IList<NursingStation> NursingStationsFor( Facility facility, bool getCachedData = true )
        {
            return i_LocationBroker.NursingStationsFor( facility, getCachedData );
        }
        
        public NursingStation NursingStationFor( Facility facility, string nursingStationCode )
        {
            return i_LocationBroker.NursingStationFor( facility, nursingStationCode );
        }

        public string ValidateLocation(Location location, Facility aFacility )
        {
            return i_LocationBroker.ValidateLocation( location, aFacility );
        }

        public ReservationResults GetBedStatus( Location location, Facility facility )
        {
            return i_LocationBroker.GetBedStatus( location, facility );
        }

        public DuplicateLocationResult CheckForDuplicateBedAssignments( Facility facility,
                                                                        string lastName,
                                                                        string firstName,
                                                                        long accountNumber,
                                                                        long medicalRecordNumber,
                                                                        SocialSecurityNumber ssn,
                                                                        DateTime dob,
                                                                        string zip )
        {
            return i_LocationBroker.CheckForDuplicateBedAssignments( facility,
                                                                         lastName,
                                                                         firstName,
                                                                         accountNumber,
                                                                         medicalRecordNumber,
                                                                         ssn,
                                                                         dob,
                                                                         zip );
        }

        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public LocationBrokerProxy()
        {
        }
        #endregion

        #region Data Elements
        private ILocationBroker i_LocationBroker = BrokerFactory.BrokerOfType< ILocationBroker >() ;
        #endregion

        #region Constants
        private const string
            LOCATION_PBAR_BROKER_PROXY_ACCOMODATION_CODES_FOR = "LOCATION_PBAR_BROKER_PROXY_ACCOMODATION_CODES_FOR";           
        #endregion
    }
}
