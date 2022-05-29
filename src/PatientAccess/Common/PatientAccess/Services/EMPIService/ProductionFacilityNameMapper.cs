
namespace PatientAccess.Services.EMPIService
{
    public class ProductionFacilityNameMapper : IPBARToEMPIFacilityNameMapper
    {
        public string GetEMPIFacilityCode(string code)
        {
            return code;
        }

        public string GetPBARFacilityCode(string code)
        {
            return code;
        }
    }
}