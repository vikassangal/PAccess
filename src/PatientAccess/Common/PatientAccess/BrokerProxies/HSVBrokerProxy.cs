using System;
using System.Collections;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;

namespace PatientAccess.BrokerProxies
{
    [Serializable]
    public class HSVBrokerProxy : AbstractBrokerProxy, IHSVBroker
    {
        #region Event Handlers
        #endregion

        #region Methods

        /// <exception cref="Exception">This method not implemented in this Broker</exception>
        public HospitalService HospitalServiceWith( long facilityNumber, string code )
        {
            throw new Exception("This method not implemented in this Broker");
        }

        public IList SelectHospitalServicesFor( long facilityId )
        {            
            Hashtable hsvCodesByFacilityHashtable = this.HSVCodesByFacilityHashtable; 
            IList hsvCodes = null;

            if( hsvCodesByFacilityHashtable[facilityId] == null )
            {
                lock( HSV_CODES_BY_FACILITY )
                {
                    // Ensure that we are still null before loading from the real broker
                    if( hsvCodesByFacilityHashtable[facilityId] == null )
                    {
                        hsvCodes = i_remoteHSVBroker.SelectHospitalServicesFor( facilityId );
                        hsvCodesByFacilityHashtable[facilityId] = hsvCodes;
                    }
                    else
                    {
                        // codes were loaded on another thread before our double-check
                        hsvCodes = (IList)hsvCodesByFacilityHashtable[facilityId];
                    }
                }
            }
            else
            {
                // codes were already loaded.
                hsvCodes = (IList)hsvCodesByFacilityHashtable[facilityId];
            }

            return hsvCodes;
        }            

        public ICollection HospitalServicesFor( long facilityNumber, string patientTypeCode )
        {
			ArrayList facilityHospitalServices = 
				( ArrayList ) this.SelectHospitalServicesFor( facilityNumber );

			HospitalService hospitalService = new HospitalService();

			return hospitalService.HospitalServicesFor( 
				facilityHospitalServices, patientTypeCode );
		
        }

        public ICollection HospitalServicesFor( long facilityNumber, string patientTypeCode, string dayCare )
        {
			ArrayList facilityHospitalServices = 
				( ArrayList ) this.SelectHospitalServicesFor( facilityNumber );

			HospitalService hospitalService = new HospitalService();

			return hospitalService.HospitalServicesFor( 
				facilityHospitalServices, patientTypeCode, dayCare );
        }

        public ICollection HospitalServicesFor(long facilityNumber, VisitType visitType, Activity activity,HospitalService service,FinancialClass financialClass)
        {
            var facilityHospitalServices = (ArrayList)SelectHospitalServicesFor(facilityNumber);

            var hospitalService = new HospitalService();

            return hospitalService.HospitalServicesFor( facilityHospitalServices, visitType, activity, service, financialClass );
        }

        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        private Hashtable HSVCodesByFacilityHashtable
        {
            get
            {
                Hashtable ht = (Hashtable)this.Cache[HSV_CODES_BY_FACILITY];
                if( ht == null )
                {
                    lock( HSV_CODES_BY_FACILITY )
                    {
                        ht = new Hashtable();
                        if( this.Cache[HSV_CODES_BY_FACILITY] == null )
                        {
                            this.Cache.Insert( HSV_CODES_BY_FACILITY, ht );
                        }
                    }
                }

                return ht;
            }
        }
        #endregion

        #region Construction and Finalization
        public HSVBrokerProxy()
        {
        }
        #endregion

        #region Data Elements
        private IHSVBroker i_remoteHSVBroker = BrokerFactory.BrokerOfType< IHSVBroker >() ;
        #endregion

        #region Constants
        private const string             
            HSV_CODES_BY_FACILITY = "HSV_CODES_BY_FACILITY";
        #endregion
    }
}
