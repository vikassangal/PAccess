using System.Xml;

namespace PatientAccess.Services
{
    public interface ICFDBLookupService
    {
        XmlNode GetFacilityAddresses( string HspCD );
        XmlNode GetFacilityPhones( string HspCD );
    }
}
