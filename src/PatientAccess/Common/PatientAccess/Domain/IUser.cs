namespace PatientAccess.Domain
{
    public interface IUser
    {
        Facility Facility { get; set; }
        string UserID { get; set; }
        string PBARSecurityCode { get; set; }
        Extensions.SecurityService.Domain.User SecurityUser { get; set; }
        string PBAREmployeeID { get; set; }
        string WorkstationID { get; set; }
    }
}