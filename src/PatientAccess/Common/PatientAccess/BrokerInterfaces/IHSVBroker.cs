using System.Collections;
using PatientAccess.Domain;

namespace PatientAccess.BrokerInterfaces
{
	/// <summary>
	/// Summary description for IHSVBroker.
	/// </summary>
	public interface IHSVBroker
	{		
        IList SelectHospitalServicesFor( long facilityNumber );

        ICollection HospitalServicesFor( long facilityNumber, string patientTypeCode );
        ICollection HospitalServicesFor( long facilityNumber, string patientTypeCode, string dayCare );

	    HospitalService HospitalServiceWith(long facilityNumber, string code);
    }
}
