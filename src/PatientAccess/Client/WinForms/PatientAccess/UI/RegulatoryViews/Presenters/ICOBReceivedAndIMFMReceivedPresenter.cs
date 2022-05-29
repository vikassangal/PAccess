namespace PatientAccess.UI.RegulatoryViews.Presenters
{
    public interface ICOBReceivedAndIMFMReceivedPresenter
    {
        void UpdateView();

        void SetCOBReceivedAsYes();

        void SetCOBReceivedAsNo();

        void SetIMFMReceivedAsYes();

        void SetIMFMReceivedAsNo();
        
    }
}