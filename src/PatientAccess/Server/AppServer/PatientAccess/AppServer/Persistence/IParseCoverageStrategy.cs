using PatientAccess.Domain;

namespace PatientAccess.Persistence
{
    interface IParseCoverageStrategy
    {
        void SetBenefitsResponse( BenefitsValidationResponse response );
        
        void Execute( );
    }
}
