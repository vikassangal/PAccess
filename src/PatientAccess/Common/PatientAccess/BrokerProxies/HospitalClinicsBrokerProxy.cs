using System;
using System.Collections;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;

namespace PatientAccess.BrokerProxies
{
    [Serializable]
    public class HospitalClinicsBrokerProxy : AbstractBrokerProxy, IHospitalClinicsBroker 
    {
        #region Event Handlers
        #endregion

        #region Methods
        public HospitalClinic PreTestHospitalClinicFor( long facilitynumber )
        {
            return i_remoteHospitalClinicsBroker.PreTestHospitalClinicFor( facilitynumber );
        }
        public ICollection HospitalClinicsFor( long facilityNumber )
        {
            Hashtable hospitalClinicsByFacility = this.HospitalClinicsByFacility;
            ICollection hospitalClinics = null;

            if( hospitalClinicsByFacility[facilityNumber] == null )
            {
                lock( CACHE_HOSPITAL_CLINICS_BY_FACILITY )
                {
                    if( hospitalClinicsByFacility[facilityNumber] == null )
                    {
                        hospitalClinics = this.i_remoteHospitalClinicsBroker.HospitalClinicsFor( facilityNumber );
                        hospitalClinicsByFacility[facilityNumber] = hospitalClinics;
                    }
                    else
                    {
                        hospitalClinics = (ICollection)hospitalClinicsByFacility[facilityNumber];
                    }
                }
            }
            else
            {
                hospitalClinics = (ICollection)hospitalClinicsByFacility[facilityNumber];
            }

            return hospitalClinics;
        }
      
        public HospitalClinic HospitalClinicWith( long facilityNumber, string code )
        {
            return this.i_remoteHospitalClinicsBroker.HospitalClinicWith( facilityNumber, code );
        }

        public HospitalClinic HospitalClinicWith( long facilityNumber, long oid )
        {
            return this.i_remoteHospitalClinicsBroker.HospitalClinicWith( facilityNumber, oid );
        }
        #endregion

        #region Properties
        private Hashtable HospitalClinicsByFacility
        {
            get
            {
                Hashtable ht = (Hashtable)this.Cache[CACHE_HOSPITAL_CLINICS_BY_FACILITY];
                if( ht == null )
                {
                    lock( CACHE_HOSPITAL_CLINICS_BY_FACILITY )
                    {
                        ht = new Hashtable();
                        if( this.Cache[CACHE_HOSPITAL_CLINICS_BY_FACILITY] == null )
                        {
                            this.Cache.Insert( CACHE_HOSPITAL_CLINICS_BY_FACILITY, ht );
                        }
                    }
                }

                return ht;
            }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public HospitalClinicsBrokerProxy()
        {
        }
        #endregion

        #region Data Elements
        private IHospitalClinicsBroker i_remoteHospitalClinicsBroker = BrokerFactory.BrokerOfType< IHospitalClinicsBroker >() ;
        #endregion

        #region Constants
        private const string CACHE_HOSPITAL_CLINICS_BY_FACILITY = "CACHE_HOSPITAL_CLINICS_BY_FACILITY";
        #endregion

    }
}
