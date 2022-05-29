using PatientAccess.Domain;

namespace PatientAccess.Persistence
{
	/// <summary>
	/// Defines the interface for PBAR ConnectionSpec Providers.
	/// </summary>
	internal interface IConnectionSpecProvider
	{
		ConnectionSpec ConnectionSpecFor( Facility aFacility );
	}
}
