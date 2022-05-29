using System;
using PatientAccess.AppServer;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Utilities;
using log4net;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Services.EMPIService;

namespace PatientAccess.Persistence
{
    /// <summary>
    /// Implements Demographics related methods.
    /// </summary>
    [Serializable]
    public class EMPIBroker : MarshalByRefObject,IEMPIBroker
    {
        #region Public Methods
 
        public EMPIPatient GetEMPIPatientFor(Patient pbarPatient, Facility currentFacility)
        {
            PBARToEMPIFacilityNameMapper = GetPABARToEMPIFacilityMapper();
            Guard.ThrowIfArgumentIsNull(currentFacility, "Facility"); 
            var empiService = new EMPIService(currentFacility, PBARToEMPIFacilityNameMapper);
            EMPIPatient ePatient;
            try
            {
                ePatient = empiService.GetEMPIPatientFor(pbarPatient);
                ePatient.Patient.MedicalRecordNumber =
                  PBARToEMPIFacilityNameMapper.GetPBARFacilityCode(pbarPatient.Facility.Code) == currentFacility.Code
                  ? ePatient.Patient.MedicalRecordNumber
                  : 0;
            }
            catch (Exception e)
            {
                const string msg = "Failed to get EMPI Account data";
                throw BrokerExceptionFactory.BrokerExceptionFrom(msg, e, c_log);
            }
            return ePatient;
        }

         
        /// <summary>
        /// Get Search Member.
        /// </summary>
        /// <param name="searchCriteria"></param>
        /// <param name="facility"> </param>
        /// <returns></returns>
        public PatientSearchResponse SearchEMPI(PatientSearchCriteria searchCriteria, Facility facility)
        {
            Guard.ThrowIfArgumentIsNull(facility, "Facility");
            var empiService = new EMPIService(facility, new ProductionFacilityNameMapper());
            
            var response = empiService.SearchEMPI(searchCriteria);
            return response;
        }

        public string GetPBARFacilityCode(string EMPIFacilityCode)
        {
            return GetPABARToEMPIFacilityMapper().GetPBARFacilityCode(EMPIFacilityCode);
        }

        private IPBARToEMPIFacilityNameMapper GetPABARToEMPIFacilityMapper()
        {
            Guard.ThrowIfArgumentIsNull(Global.PASServerEnvironment, "PASServerEnvironment"); 
            switch (Global.PASServerEnvironment)
            {
                case ServerEnvironment.LOCAL:
                    return new TestFacilityNameMapper();

                case ServerEnvironment.DEVELOPMENT:
                    return new TestFacilityNameMapper();

                case ServerEnvironment.TEST:
                    return new TestFacilityNameMapper();

                case ServerEnvironment.MODEL:
                    return new TestFacilityNameMapper();

                case ServerEnvironment.PRODUCTION:
                    return new ProductionFacilityNameMapper();
                default:
                    c_log.ErrorFormat("PAS environment is not mapped correctly as {0}", Global.PASServerEnvironment);
                    return new TestFacilityNameMapper();
            }
        }

        #endregion

        #region Data Elements

        private static readonly ILog c_log =
            LogManager.GetLogger(typeof(EMPIBroker));

        private IPBARToEMPIFacilityNameMapper PBARToEMPIFacilityNameMapper;

        #endregion
    }
}
