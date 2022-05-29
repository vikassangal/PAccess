using System.Collections;
using PatientAccess.Domain;

namespace PatientAccess.BrokerInterfaces
{
    /// <summary>
    /// interface for DischargeBroker and related methods
    /// </summary>
    public interface IDischargeBroker
    {
        //Get all Discharge Dispositions
        ICollection AllDischargeDispositions( long facilityID );
        //Get the  Discharge Disposition for a facility and code
        DischargeDisposition DischargeDispositionWith( long facilityID,string code );

     
    }
}
