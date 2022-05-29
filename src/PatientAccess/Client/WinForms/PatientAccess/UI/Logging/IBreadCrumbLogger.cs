using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;

namespace PatientAccess.UI.Logging
{
    public interface IBreadCrumbLogger
    {
        void Log(string message);
        void Log(string message, Account account);
        void Log(string message, AccountProxy ap);
    }
}