namespace PatientAccess.BrokerInterfaces
{
    /// <summary>
    /// Summary description for IPriorVisitBroker.
    /// </summary>
    public interface IPriorVisitBroker
    {
        PriorVisitInformationResponse GetPriorVisitResponse(PriorVisitInformationRequest request);
    }
}
