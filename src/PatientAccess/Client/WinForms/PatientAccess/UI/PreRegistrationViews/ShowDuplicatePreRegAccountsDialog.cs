using System.Collections;
using System.Windows.Forms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI.PreRegistrationViews
{
    public partial class ShowDuplicatePreRegAccountsDialog : TimeOutFormView
    {
        #region Event Handlers
        #endregion

        #region Methods
        public override void UpdateView()
        {
            ListViewItem lvItem;

            if( Model != null )
            {
                foreach( AccountProxy accountProxy in this.Model )
                {
                    lvItem = new ListViewItem();

                    lvItem.SubItems.Add( accountProxy.AdmitDate.ToString( "MM/dd/yyyy HH:mm" ) );

                    if( accountProxy.HospitalService != null )
                    {
                        lvItem.SubItems.Add( accountProxy.HospitalService.ToString() );
                    }
                    else
                    {
                        lvItem.SubItems.Add( "" );
                    }
                    if( accountProxy.HospitalClinic != null )
                    {
                        lvItem.SubItems.Add( accountProxy.HospitalClinic.ToString() );
                    }
                    else
                    {
                        lvItem.SubItems.Add( "" );
                    }

                    lvItem.SubItems.Add( accountProxy.AccountNumber.ToString() );

                    lvDuplicatePreregAccounts.Items.Add( lvItem );
                }
            }
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        public new ICollection Model
        {
            private get
            {
                return base.Model as ICollection;
            }
            set
            {
                base.Model = value;
            }
        }
       
        #endregion

        #region Construction and Finalization
        public ShowDuplicatePreRegAccountsDialog()
        {           
            InitializeComponent();
            base.EnableThemesOn( this );
        }
        #endregion
      
        #region Data Elements
        #endregion     
    }
}