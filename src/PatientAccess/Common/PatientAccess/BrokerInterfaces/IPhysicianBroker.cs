using System.Collections;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.BrokerInterfaces
{
    /// <summary>
    /// Summary description for IPhysicianBroker.
    /// </summary>
    public interface IPhysicianBroker
    {
        #region Event Handlers
        #endregion

        #region Methods
        
        ICollection PhysiciansMatching( PhysicianSearchCriteria physicianSearchCriteria );
        ICollection SpecialtiesFor( long facilityOid );
        Speciality SpecialityWith(long facilityNumber, string code);
        Physician PhysicianDetails( long facilityOid, long physicianNumber );
        ICollection PhysiciansSpecialtyMatching( long facilityOid, Speciality speciality );

		Physician PhysicianWith( long facilityNumber, long physicianNumber );

        Physician VerifyPhysicianWith( long facilityNumber, long physicianNumber );
       
        Physician PhysicianStatisticsFor( long facilityOid, long physicianNumber );

        PhysicianRelationship BuildAdmittingPhysicianRelationship( 
			long facilityNumber, 
			long physicianNumber );
		PhysicianRelationship BuildReferringPhysicianRelationship( 
			long facilityNumber, 
			long physicianNumber );
		PhysicianRelationship BuildAttendingPhysicianRelationship( 
			long facilityNumber, 
			long physicianNumber );
		PhysicianRelationship BuildOperatingPhysicianRelationship( 
			long facilityNumber, 
			long physicianNumber );
		PhysicianRelationship BuildPrimaryCarePhysicianRelationship( 
			long facilityNumber, 
			long physicianNumber );
		PhysicianRelationship BuildConsultingPhysicianRelationship( 
			long facilityNumber, 
			long physicianNumber );
        long GetNonStaffPhysicianNumber();
		#endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
