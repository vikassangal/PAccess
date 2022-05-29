using PatientAccess.Domain;

namespace PatientAccess.UI.TransferViews
{
    public class TransferOutPatToInPatStep1Presenter: ITransferOutPatToInPatStep1Presenter
    {
        private ITransferOutPatToInPatStep1View step1Step1View;
        public TransferOutPatToInPatStep1Presenter(ITransferOutPatToInPatStep1View step1View)
        {
            step1Step1View = step1View;
        }

        public void SetAdmittingCategory()
        {
            if ( step1Step1View.Model.KindOfVisit != null )
            {
                if ( step1Step1View.Model.KindOfVisit.Code == VisitType.EMERGENCY_PATIENT )
                    step1Step1View.Model.AdmittingCategory = ADMITTING_CATEGORY_EMERGENCY;
                else
                    step1Step1View.Model.AdmittingCategory = ADMITTING_CATEGORY_URGENT;
            }
            else
            {
                step1Step1View.Model.AdmittingCategory = ADMITTING_CATEGORY_URGENT;
            }
        }

        private const string ADMITTING_CATEGORY_URGENT = "3";
        private const string ADMITTING_CATEGORY_EMERGENCY = "2";
    }
}
