namespace PatientAccess.Services.EMPIService
{
    public interface IPBARToEMPIFacilityNameMapper
    {
        string GetEMPIFacilityCode(string code);
        string GetPBARFacilityCode(string code);
    }
}
