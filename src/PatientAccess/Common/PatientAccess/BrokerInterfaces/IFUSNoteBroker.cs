using System;
using System.Collections;
using PatientAccess.Domain;
using PatientAccess.Domain.Auditing.FusNotes;

namespace PatientAccess.BrokerInterfaces
{
    /// <summary>
	/// Summary description for IFUSNoteBroker.
	/// </summary>
	public interface IFUSNoteBroker
    {
        FusActivity FusActivityWith( string activityCode );
        ICollection AllActivityCodes();
        ICollection AllWriteableActivityCodes();
        Hashtable   AllActivityCodesHashtable();
        Hashtable   AllWriteableActivityCodesHashtable();

        void        WriteFUSNotes( Account anAccount, string pbarEmployeeID );
        void PostRemarksFusNote(Account anAccount, string userID,FusActivity activity, string remarks, DateTime noteDateTime);

        ICollection GetMergedFUSNotesFor( Account anAccount );
    }
}
