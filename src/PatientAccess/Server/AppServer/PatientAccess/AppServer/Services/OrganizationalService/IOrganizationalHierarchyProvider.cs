using Peradigm.Framework.Domain.Parties;

namespace Extensions.OrganizationalService
{
	public interface IOrganizationalHierarchyProvider
	{
       OrganizationalUnit GetOrganizationalHierarchy();
	}
}
