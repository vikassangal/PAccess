using System;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using log4net;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.UI.PatientSearch;

namespace PatientAccess.UI.Registration
{
	//TODO: Create XML summary comment for PreRegistrationView
	[Serializable]
    public partial class NewbornPreRegistrationView : ControlView, INewbornPreRegistrationView
	{
		#region Events
        public EventHandler ReturnToMainScreen { get; set; }
		#endregion

		#region Methods
        public void EnableAccountView()
        {
            ClearControls();

            accountView = AccountView.NewInstance();

            SuspendLayout();
            accountView.Dock = DockStyle.Fill;
            ResumeLayout( false );
            Controls.Add( (Control)accountView );
        }

        public void DisplayMasterPatientIndexView()
        {
            ClearControls();
            masterPatientIndexView = new MasterPatientIndexView();

            if ( !IsInDesignMode )
            {
                masterPatientIndexView.CurrentActivity = CurrentActivity;
            }

            masterPatientIndexView.Dock = DockStyle.Fill;
            Controls.Add( masterPatientIndexView );
        }

        public void SetMasterPatientIndexViewActivity( Activity activity )
        {
            masterPatientIndexView.CurrentActivity = activity;
        }

        public void RegisterOnReturnToMainScreen( EventHandler onReturnToMainScreen )
        {
            masterPatientIndexView.ReturnToMainScreen += onReturnToMainScreen;
        }

        public void SetCursor( Cursor cursor )
        {
            Cursor = cursor;
        }

		#endregion

		#region Properties

		public Activity CurrentActivity
		{
			get
			{
				if( i_CurrentActivity == null )
				{
					i_CurrentActivity = new PreAdmitNewbornActivity();
				}
				return i_CurrentActivity;
			}
		}

        public IAccount SelectedAccount { get; set; }

        public Account ModelAccount
        {
            get { return (Account)Model; }
            set { Model = value; }
        }

        public IAccountView accountView
        {
            get;
            set;
        }

        public bool IsDisposedOrDisposing
        {
            get { return ( IsDisposed || Disposing ); }
        }

		#endregion

		#region Private Methods
		
		private void ClearControls()
		{
			foreach( Control control in Controls )
			{
				if( control != null )
				{
					try
					{
						control.Dispose();
					}
					catch( Exception ex )
					{
						c_log.Error( "Failed to dispose of a control; " + ex.Message, ex );
					}
				}
			}
			Controls.Clear();
		}

		protected override void Dispose( bool disposing )
		{
            //Dispose presenter which will unregister SearchEventAggregator events and cancel backgroundworker
            if ( presenter != null )
            {
                presenter.Dispose(disposing);
                presenter = null;
            }

		    if( disposing )
			{                
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
	   
		#endregion

		#region Private Properties
		#endregion

		#region Construction and Finalization

		public NewbornPreRegistrationView()
		{
			InitializeComponent();
		    presenter = new NewbornPreRegistrationPresenter(this);
		}
		#endregion

		#region Data Elements
		private static readonly ILog c_log = LogManager.GetLogger( typeof( NewbornPreRegistrationView ) );
	    private NewbornPreRegistrationPresenter presenter;
		private Activity                                                i_CurrentActivity;

		#endregion

		#region Constants
		#endregion
        
    }
}
