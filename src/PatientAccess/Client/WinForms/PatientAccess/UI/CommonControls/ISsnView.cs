using PatientAccess.Domain;

namespace PatientAccess.UI.CommonControls
{
    public interface ISsnView
    {
        void AddSsnStatus( SocialSecurityNumberStatus ssnStatus );
        void DeselectSelectedStatus();
        int SsnStatusCount { get; }
        SsnViewContext SsnContext { get; set; }
        bool SsnEnabled { get; set; }
        string SsnText { get; set; }
        ISsnFactory SsnFactory { get; set; }
        Account ModelAccount { get; }
        void ClearSsnStatus();
    }
}