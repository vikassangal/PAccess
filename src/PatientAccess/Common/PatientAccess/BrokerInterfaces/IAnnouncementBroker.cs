using System.Collections;
using PatientAccess.Domain;

namespace PatientAccess.BrokerInterfaces
{
	/// <summary>
	/// Summary description for IAnnouncementBroker.
	/// </summary>
	public interface IAnnouncementBroker
	{
        ICollection CurrentAnnouncementsFor( ICollection facilityRoles, Facility currentFacility );
        ICollection AllAnnouncementsFor( ICollection facilitiesToManage );
        int SaveAnnouncement( Announcement announcementToSave );
	}
}
