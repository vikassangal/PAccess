using Extensions.UI.Winforms;
using PatientAccess.UI.CommonControls.Suffix.Presenters;
using PatientAccess.UI.CommonControls.Suffix.Views;

namespace PatientAccess.UI.CommonControls.Suffix.ViewImpl
{
    public partial class SuffixView : ControlView, ISuffixView
    {

        public SuffixPresenter SuffixPresenter { get; set; }
        public void UpdateSuffix(string suffix)
        {
            cmbSuffix.SelectedItem = suffix;
        }

        public void ClearItems()
        {
            cmbSuffix.Items.Clear();
        }

        public void AddSuffix(string suffix)
        {
            cmbSuffix.Items.Add(suffix);
        }

        public void ClearSuffix()
        {
            cmbSuffix.SelectedIndex = -1;
        }

        private void cmbSuffix_SelectedIndexChanged(object sender, System.EventArgs e)
        {
             string  selectedSuffix = cmbSuffix.SelectedItem as string;
            if (selectedSuffix != null)
            {
                SuffixPresenter.UpdateSuffix(selectedSuffix);
            }
        }
        public SuffixView()
        {
            InitializeComponent(); 
        }

    }
}
