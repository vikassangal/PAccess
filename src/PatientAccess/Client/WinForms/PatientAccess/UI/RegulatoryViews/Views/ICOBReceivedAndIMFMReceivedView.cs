using PatientAccess.UI.RegulatoryViews.Presenters;

namespace PatientAccess.UI.RegulatoryViews.Views
{
    public interface ICOBReceivedAndIMFMReceivedView
    {
        void ShowCOBReceived();

        void HideCOBReceived();

        void ShowIMFMReceived();

        void HideIMFMReceived();

        void SetCOBReceivedAsYes();

        void SetCOBReceivedAsNo();

        void SetIMFMReceivedAsYes();

        void SetIMFMReceivedAsNo();

        void COBUnSelected();

        void IMFMUnSelected();

        ICOBReceivedAndIMFMReceivedPresenter Presenter { get; set; }

    }
}
