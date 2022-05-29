using Peradigm.Framework.Domain.Parties;

namespace Extensions.SecurityService.Domain
{
	/// <summary>
	/// ISecurityProvider is the interface implemented by providers
	/// that work with the SecurityService.
	/// </summary>
	public interface ISecurityProvider
	{
		bool   HasUserWith( string upn, string password );
		void   Logout( User user );
		string RolesFor( Application application );
		string RolesFor( Application application, string upn, string password );
		string UsersIn( Application application, Role role, Facility facililty );
        string LegacyUserWith( string upn, string password, string legacyApplicationName );
	}
}
