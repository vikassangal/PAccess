using PatientAccess.Domain;

namespace PatientAccess.BrokerInterfaces
{
    public interface ICodedDiagnosisBroker
    {
        CodedDiagnoses CodedDiagnosisFor(long hsp, long accountNumber, long mrc, bool forPreMse, Facility facility);
    }
}
