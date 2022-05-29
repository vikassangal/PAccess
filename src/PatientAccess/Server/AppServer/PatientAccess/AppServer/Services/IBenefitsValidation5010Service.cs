using PatientAccess.BenefitsValidation5010Proxy;

namespace PatientAccess.Services
{
    public interface IBenefitsValidation5010ServiceSoapClient
    {
        initiateResponse initiate(initiateRequest initiate1);

        obtainResultResponse obtainResult(obtainResultRequest obtainResult1);

    }
}