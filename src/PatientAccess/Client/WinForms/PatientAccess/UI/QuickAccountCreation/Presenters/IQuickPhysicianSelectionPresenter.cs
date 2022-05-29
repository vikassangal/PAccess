using PatientAccess.Domain.Parties;

namespace PatientAccess.UI.QuickAccountCreation.Presenters
{
    public interface IQuickPhysicianSelectionPresenter
    {
        #region Operations

        bool PhysicianIsValid( Physician aPhysician );
        void RemovePhysicianRelationship( PhysicianRelationship physicianRelationship );
        void Find();
        void RecordNonStaffPhysician();
        void ShowDetails( long physicianNumber, string physicianRelationship );
        void UpdateViewDetail();
        void ValidatePhysicians();
        void ValidatePhysicianNumber();

        #endregion Operations
    }
}
