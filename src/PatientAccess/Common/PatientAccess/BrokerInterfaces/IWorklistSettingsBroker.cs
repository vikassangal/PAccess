using System.Collections;
using PatientAccess.Domain;

namespace PatientAccess.BrokerInterfaces
{
    /// <summary>
    /// Summary description for IWorklistBroker.
    /// </summary>
    public interface IWorklistSettingsBroker
    {
        ArrayList GetEmergencyDepartmentWorklistRanges();
        ArrayList GetInsuranceVerificationWorklistRanges();
        ArrayList GetNoShowWorklistRanges();
        ArrayList GetPatientLiabilityWorklistRanges();
        ArrayList GetPostRegWorklistRanges();
        ArrayList GetPreRegWorklistRanges();
        ArrayList GetOnlinePreRegWorklistRanges();

        WorklistSettings EmergencyDepartmentWorklistSettings( long tenetID );
        WorklistSettings DefaultEmergencyDepartmentWorklistSettings();

        WorklistSettings InsuranceVerificationWorklistSettings( long tenetID );
        WorklistSettings DefaultInsuranceVerificationWorklistSettings();

        WorklistSettings NoShowWorklistSettings( long tenetID );
        WorklistSettings DefaultNoShowWorklistSettings();

        WorklistSettings PatientLiabilityWorklistSettings( long tenetID );
        WorklistSettings DefaultPatientLiabilityWorklistSettings();

        WorklistSettings PostRegWorklistSettings( long tenetID );
        WorklistSettings DefaultPostRegWorklistSettings();

        WorklistSettings PreRegWorklistSettings( long tenetID );
        WorklistSettings DefaultPreRegWorklistSettings();

        WorklistSettings OnlinePreRegWorklistSettings( long tenetID );
        WorklistSettings DefaultOnlinePreRegWorklistSettings();
        
        void SaveEmergencyDepartmentWorklistSettings( long tenetID, WorklistSettings worklistSettings );
        void SaveInsuranceVerificationWorklistSettings( long tenetID, WorklistSettings worklistSettings );
        void SaveNoShowWorklistSettings( long tenetID, WorklistSettings worklistSettings );
        void SavePatientLiabilityWorklistSettings( long tenetID, WorklistSettings worklistSettings );
        void SavePostRegWorklistSettings( long tenetID, WorklistSettings worklistSettings );
        void SavePreRegWorklistSettings( long tenetID, WorklistSettings worklistSettings );
        void SaveOnlinePreRegWorklistSettings( long tenetID, WorklistSettings worklistSettings );

        void SaveWorklistSettings( long tenetID, WorklistSettings worklistSettings, Worklist worklist );

        WorklistSelectionRange WorklistSelectionRangeWith(long rangeID);

        Worklist EmergencyDepartmentWorklist();
        Worklist InsuranceVerificationWorklist();
        Worklist NoShowWorklist();
        Worklist PatientLiabilityWorklist();
        Worklist PostRegWorklist();
        Worklist PreRegWorklist();
        Worklist OnlinePreRegWorklist();
        Worklist WorklistWith( long worklistID );

        ArrayList GetAllWorkLists();
    }
}
