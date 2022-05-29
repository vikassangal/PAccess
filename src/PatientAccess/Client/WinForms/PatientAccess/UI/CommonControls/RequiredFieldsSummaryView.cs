using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PatientAccess.UI.CommonControls
{
    [Serializable]
    public partial class RequiredFieldsSummaryView : TimeOutFormView, IRequiredFieldsSummaryView
    {
        #region Properties

        public string HeaderText
        {
            set { lblHeader.Text = value; }
            get { return lblHeader.Text; }
        }

        public RequiredFieldsSummaryPresenter Presenter { get; set; }

        #endregion

        #region Construction and Finalization

        public RequiredFieldsSummaryView()
        {
            InitializeComponent();
        }

        #endregion

        public void ShowAsModalDialog( object owner )
        {
            ShowDialog( ( IWin32Window )owner );
        }

        public void Update( List<RequiredFieldItem> itemInLists )
        {
            dgvActionItems.DataSource = itemInLists;
        }

        #region Event Handlers

        private void btnOK_Click( object sender, EventArgs e )
        {
            Close();
        }

        /// <summary>
        /// dgvActionItems_CellMouseDoubleClick - user double-clicked a row... take them to the tab
        /// corresponding to their error
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvActionItems_CellMouseDoubleClick( object sender, DataGridViewCellMouseEventArgs e )
        {
            string tab = ( ( PADataGridView )sender ).SelectedCells[0].Value.ToString();

            Presenter.RequiredFieldSelected( tab );
        }

        #endregion
    }
}