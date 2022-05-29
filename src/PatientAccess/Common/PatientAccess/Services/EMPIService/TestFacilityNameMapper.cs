using System.Collections.Generic;

namespace PatientAccess.Services.EMPIService
{
    public class TestFacilityNameMapper : IPBARToEMPIFacilityNameMapper
    {
        public string GetEMPIFacilityCode(string code)
        {
            return PBARToEMPIfacilityMap.ContainsKey(code) ? PBARToEMPIfacilityMap[code] : code;
        }

        public string GetPBARFacilityCode(string code)
        {
            return EMPIToPBARfacilityMap.ContainsKey(code) ? EMPIToPBARfacilityMap[code] : code;
        }

        public TestFacilityNameMapper()
        {
            PBARToEMPIfacilityMap.Add("DHF", "DHF");
            PBARToEMPIfacilityMap.Add("PRV", "PRV");

            EMPIToPBARfacilityMap.Add("DHF", "DHF");
            EMPIToPBARfacilityMap.Add("PRV", "PRV");
        }

        readonly Dictionary<string, string> EMPIToPBARfacilityMap = new Dictionary<string, string>();
        readonly Dictionary<string, string> PBARToEMPIfacilityMap = new Dictionary<string, string>(); 
    }
}