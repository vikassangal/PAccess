using System.Collections;
using PatientAccess.Domain;

namespace PatientAccess.BrokerInterfaces
{
	/// <summary>
	/// Summary description for IBenefitsCategoryBroker.
	/// </summary>
	public interface IBenefitsCategoryBroker
	{
	    ICollection AllBenefitsCategories( long facilityID );
        ICollection BenefitsCategoriesFor( Facility facility, string hsvCode );
	    BenefitsCategory BenefitsCategoryWith(long facilityID, long benefitsCategoryID);
	}
}
