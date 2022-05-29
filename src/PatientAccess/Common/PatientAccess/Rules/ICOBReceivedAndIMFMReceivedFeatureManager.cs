using PatientAccess.Domain;

namespace PatientAccess.Rules
{
  public interface ICOBReceivedAndIMFMReceivedFeatureManager
  {
      bool IsCOBReceivedEnabledForAccount(Account account);

      bool IsIMFMReceivedEnabledForAccount(Account account);

      void IfApplicableResetCOBReceivedOn(Account account);

      void IfApplicableResetIMFMReceivedOn(Account account);

   }
}
