using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Rules;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.Factories;
using PatientAccess.UI.HelperClasses;
using PatientAccess.UI.Reports.FaceSheet;

namespace PatientAccess.UI.TransferViews
{
    public class SwapPatientLocationsView : ControlView
    {
        #region Events

        public event EventHandler RepeatActivity;
        public event EventHandler CloseView;

        #endregion

        #region Event Handlers

        private void SwapPatientLocationsView_Load( object sender, EventArgs e )
        {
            progressPanel1.Visible = false;
            panelActions.Visible = false;
        }

        private void swapPatientLocationView1_LeaveSearchField( object sender, EventArgs e )
        {
            ChangeAcceptButton();
        }

        private void swapPatientLocationView2_LeaveSearchField( object sender, EventArgs e )
        {
            ChangeAcceptButton();
        }

        private void btnOk_Click( object sender, EventArgs e )
        {
            if ( RuleEngine.AccountHasFailedError() )
            {
                btnOK.Enabled = true;
                return;
            }

            if ( validateDates( e, "NONE" ) )
            {
                Cursor = Cursors.WaitCursor;
                SwapPatientLocations( e );
                Cursor = Cursors.Default;
            }
        }

        private void btnCancel_Click( object sender, EventArgs e )
        {
            if ( AccountActivityService.ConfirmCancelActivity(
                sender, new LooseArgs( Patient1AccountProxy ) ) )
            {
                CloseView( this, new EventArgs() );

                // cancel the background worker here...and in the dispose since it's 
                // called too - redundant code, but better to be safe 
                if ( backgroundWorker != null )
                {
                    backgroundWorker.CancelAsync();
                }

                Dispose();
            }
        }

        private void btnRepeatActivity_Click( object sender, EventArgs e )
        {
            Dispose();
            RepeatActivity( this, new LooseArgs( new Patient() ) );
        }

        // Might need to add the background worker Cancel functionality here too. 
        // what happens if the user does not cancel and instead leaves this view?
        private void btnCloseActivity_Click( object sender, EventArgs e )
        {
            // cancel the background worker here...
            if ( backgroundWorker != null )
            {
                backgroundWorker.CancelAsync();
            }
            CloseView( this, new EventArgs() );
            Dispose();
        }

        private void Pat1VerificationSucceeded( object sender, EventArgs e )
        {
            Patient1AccountProxy = ( AccountProxy )( ( LooseArgs )e ).Context;
            swapPatientLocationView2.OtherAccountProxy = Patient1AccountProxy;

            i_Account1.TransferredFromHospitalService = ( HospitalService )Patient1AccountProxy.HospitalService.Clone();

            if ( Patient2VerificationSucceeded )
            {
                swapPatientLocationView2.NewLocation = Patient1AccountProxy.Location;
                swapPatientLocationView1.NewLocation = Patient2AccountProxy.Location;
            }

            Patient1VerificationSucceeded = true;

            if ( Patient1VerificationSucceeded && Patient2VerificationSucceeded )
            {
                DoForBothVerificationSucceeded();
            }

            RunRules();
        }

        private void Pat2VerificationSucceeded( object sender, EventArgs e )
        {
            Patient2AccountProxy = ( AccountProxy )( ( LooseArgs )e ).Context;
            swapPatientLocationView1.OtherAccountProxy = Patient2AccountProxy;

            i_Account2.TransferredFromHospitalService = ( HospitalService )Patient2AccountProxy.HospitalService.Clone();

            if ( Patient1VerificationSucceeded )
            {
                swapPatientLocationView1.NewLocation = Patient2AccountProxy.Location;
                swapPatientLocationView2.NewLocation = Patient1AccountProxy.Location;
            }

            Patient2VerificationSucceeded = true;

            if ( Patient1VerificationSucceeded && Patient2VerificationSucceeded )
            {
                DoForBothVerificationSucceeded();
            }

            RunRules();
        }

        private void Pat1VerificationFailed( object sender, EventArgs e )
        {
            Patient1AccountProxy = null;
            swapPatientLocationView2.OtherAccountProxy = null;
            swapPatientLocationView2.NewLocation = null;
            Patient1VerificationSucceeded = false;

            DoForNotBothVerificationSucceeded();
        }

        private void Pat2VerificationFailed( object sender, EventArgs e )
        {
            Patient2AccountProxy = null;
            swapPatientLocationView1.OtherAccountProxy = null;
            swapPatientLocationView1.NewLocation = null;
            Patient2VerificationSucceeded = false;

            DoForNotBothVerificationSucceeded();
        }

        private void cboPat1HSVChanged( object sender, EventArgs e )
        {
            ChangeAcceptButton();

            checkForRequiredFields( sender, e );

            UIColors.SetNormalBgColor( swapPatientLocationView1.cboHospitalService );
            RuleEngine.GetInstance().EvaluateRule( typeof( HospitalServiceToInPat1Required ),
                                                  swapPatientLocationView1.cboHospitalService );
        }

        private void cboPat1ACCChanged( object sender, EventArgs e )
        {
            ChangeAcceptButton();

            checkForRequiredFields( sender, e );
            UIColors.SetNormalBgColor( swapPatientLocationView1.cboAccomodations1 );
            RuleEngine.GetInstance().EvaluateRule( typeof( AccommodationToInPat1Required ),
                                                  swapPatientLocationView1.cboAccomodations1 );
        }

        private void cboPat2HSVChanged( object sender, EventArgs e )
        {
            ChangeAcceptButton();

            checkForRequiredFields( sender, e );
            UIColors.SetNormalBgColor( swapPatientLocationView2.cboHospitalService );
            RuleEngine.GetInstance().EvaluateRule( typeof( HospitalServiceToInPat2Required ),
                                                  swapPatientLocationView2.cboHospitalService );
        }

        private void cboPat2ACCChanged( object sender, EventArgs e )
        {
            ChangeAcceptButton();

            checkForRequiredFields( sender, e );
            UIColors.SetNormalBgColor( swapPatientLocationView2.cboAccomodations1 );
            RuleEngine.GetInstance().EvaluateRule( typeof( AccommodationToInPat2Required ),
                                                  swapPatientLocationView2.cboAccomodations1 );
        }

        private void dateTimePicker_CloseUp( object sender, EventArgs e )
        {
            mtbTransferDate.UnMaskedText = CommonFormatting.MaskedDateFormat( dateTimePicker.Value );

            if ( !dateTimePicker.Focused &&
                DateValidator.IsValidTime( mtbTransferTimeEdit ) )
            {
                i_Account1.TransferDate = GetTransferDateTime();
                UIColors.SetNormalBgColor( mtbTransferDate );

                RuleEngine.GetInstance().EvaluateRule( typeof( TransferDateRequired ), i_Account1 );
                mtbTransferDate.Focus();
            }
            else
            {
                if ( !dateTimePicker.Focused )
                {
                    mtbTransferTimeEdit.Focus();
                }
                UIColors.SetErrorBgColor( mtbTransferTimeEdit );
            }
        }

        private void mtbTransferDate_Validating( object sender, CancelEventArgs e )
        {
            UIColors.SetNormalBgColor( mtbTransferDate );

            if ( !dateTimePicker.Focused &&
                DateValidator.IsValidTime( mtbTransferTimeEdit ) )
            {
                UpdateTransferDateTimeAndRunRule();
                validateDates( e, "DATE" );
                checkForRequiredFields( null, null );
            }
            else
            {
                if ( !dateTimePicker.Focused )
                {
                    mtbTransferTimeEdit.Focus();
                }
                UIColors.SetErrorBgColor( mtbTransferTimeEdit );
            }
        }

        private void mtbTransferTimeEdit_Validating( object sender, CancelEventArgs e )
        {
            UIColors.SetNormalBgColor( mtbTransferTimeEdit );

            if ( DateValidator.IsValidTime( mtbTransferTimeEdit ) )
            {
                UpdateTransferDateTimeAndRunRule();

                Refresh();
                validateDates( e, "TIME" );
                checkForRequiredFields( null, null );
            }
            else
            {
                if ( !dateTimePicker.Focused )
                {
                    mtbTransferTimeEdit.Focus();
                }
                UIColors.SetErrorBgColor( mtbTransferTimeEdit );
            }
        }

        private void checkForRequiredFields( object sender, EventArgs e )
        {
            if ( Patient1AccountProxy == null ||
                Patient2AccountProxy == null )
            {
                btnOK.Enabled = false;
            }
            else if ( mtbTransferDate.UnMaskedText.Trim() == string.Empty ||
                     mtbTransferTimeEdit.UnMaskedText.Trim() == string.Empty )
            {
                btnOK.Enabled = false;
            }
            else if ( swapPatientLocationView1.cboAccomodations1.Enabled &&
                     swapPatientLocationView1.cboAccomodations1.SelectedIndex <= 0 ||
                     swapPatientLocationView2.cboAccomodations1.Enabled &&
                     swapPatientLocationView2.cboAccomodations1.SelectedIndex <= 0 )
            {
                btnOK.Enabled = false;
            }
            else
            {
                btnOK.Enabled = true;
            }
        }

        private void HospitalServiceToInPat1RequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( swapPatientLocationView1.cboHospitalService );
        }

        private void HospitalServiceToInPat2RequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( swapPatientLocationView2.cboHospitalService );
        }

        private void AccommodationToInPat1RequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( swapPatientLocationView1.cboAccomodations1 );
        }

        private void AccommodationToInPat2RequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( swapPatientLocationView2.cboAccomodations1 );
        }

        private void TransferDateRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( mtbTransferDate );
        }

        private void btnPrintFaceSheet1_Click( object sender, EventArgs e )
        {
            // TLG 05/21/2007 replace .AsAccount with streamlined call to AccountActivityService

            var faceSheetPrintService =
                new PrintService( AccountActivityService.SelectedAccountFor( swapPatientLocationView1.Model ) );
            faceSheetPrintService.Print();
        }

        private void btnPrintFaceSheet2_Click( object sender, EventArgs e )
        {
            // TLG 05/21/2007 replace .AsAccount with streamlined call to AccountActivityService

            var faceSheetPrintService =
                new PrintService( AccountActivityService.SelectedAccountFor( swapPatientLocationView2.Model ) );
            faceSheetPrintService.Print();
        }

        #endregion

        #region Public Methods

        public override void UpdateView()
        {
            RegisterRulesEvents();

            swapPatientLocationView1.PatientLabel = "Patient 1";
            swapPatientLocationView2.PatientLabel = "Patient 2";


            i_Account1.TransferDate = FacilityDateTime;
            i_Account2.TransferDate = FacilityDateTime;

            TransferService.PopulateDefaultTransferDateTime( mtbTransferDate, mtbTransferTimeEdit, FacilityDateTime );
        }

        #endregion

        #region Public Properties

        private DateTime FacilityDateTime
        {
            get
            {
                if ( i_FacilityDateTime == DateTime.MinValue )
                {
                    i_FacilityDateTime = TransferService.GetLocalDateTime( User.GetCurrent().Facility.GMTOffset,
                                                                          User.GetCurrent().Facility.DSTOffset );
                }

                return i_FacilityDateTime;
            }
        }

        #endregion

        #region Private Methods

        private bool validateDates( EventArgs e, string field )
        {
            if ( TransferService.IsTransferDateValid( mtbTransferDate ) )
            {
                if ( !TransferService.IsFutureTransferDate( mtbTransferDate,
                                                          mtbTransferTimeEdit,
                                                          FacilityDateTime,
                                                          field ) )
                {
                    if ( !TransferService.IsTransferDateBeforeAdmitDate( mtbTransferDate,
                                                                       mtbTransferTimeEdit,
                                                                       Patient1AccountProxy.AdmitDate >
                                                                       Patient2AccountProxy.AdmitDate
                                                                           ? Patient1AccountProxy.AdmitDate
                                                                           : Patient2AccountProxy.AdmitDate,
                                                                       field, true ) )
                    {
                        if ( DateValidator.IsValidTime( mtbTransferTimeEdit ) )
                        {
                            return true;
                        }

                        if ( !dateTimePicker.Focused )
                        {
                            mtbTransferTimeEdit.Focus();
                        }
                        UIColors.SetErrorBgColor( mtbTransferTimeEdit );
                    }
                }
            }

            return false;
        }

        private void DoForBothVerificationSucceeded()
        {
            checkForRequiredFields( null, null );

            mtbTransferDate.Enabled = true;
            dateTimePicker.Enabled = true;
            mtbTransferTimeEdit.Enabled = true;
        }

        private void DoForNotBothVerificationSucceeded() //might be combined with above one
        {
            mtbTransferDate.Enabled = false;
            dateTimePicker.Enabled = false;
            mtbTransferTimeEdit.Enabled = false;
        }

        private void BeforeWork()
        {
            progressPanel1.Visible = true;
            progressPanel1.BringToFront();

            Patient1AccountProxy.HospitalService =
                ( HospitalService )swapPatientLocationView1.cboHospitalService.SelectedItem;
            Patient2AccountProxy.HospitalService =
                ( HospitalService )swapPatientLocationView2.cboHospitalService.SelectedItem;

            Patient1AccountProxy.Location.Bed.Accomodation =
                ( Accomodation )swapPatientLocationView1.cboAccomodations1.SelectedItem;
            Patient2AccountProxy.Location.Bed.Accomodation =
                ( Accomodation )swapPatientLocationView2.cboAccomodations1.SelectedItem;
        }

        private void DoSwap( object sender, DoWorkEventArgs e )
        {
            // do lengthy processing here...

            Patient1AccountProxy.Location =
                TransferService.DeepCopyLocationAndAccomodation( Patient2AccountProxy.LocationFrom,
                                                                Patient1AccountProxy.Location.Bed.Accomodation );
            Patient1AccountProxy.LocationTo = Patient1AccountProxy.Location;

            Patient2AccountProxy.Location =
                TransferService.DeepCopyLocationAndAccomodation( Patient1AccountProxy.LocationFrom,
                                                                Patient2AccountProxy.Location.Bed.Accomodation );
            Patient2AccountProxy.LocationTo = Patient2AccountProxy.Location;

            var hsv1From = ( HospitalService )i_Account1.TransferredFromHospitalService.Clone();
            var hsv2From = ( HospitalService )i_Account2.TransferredFromHospitalService.Clone();

            Account acct1 = ExtendProxyToAccount( Patient1AccountProxy, hsv1From );

            // This background worker supports cancellation (WorkerSupportsCancellation= true )
            // so I've added a check for cancellationPending, if true set e.Cancel to true and return 
            // rationale: if the user has cancelled the task do not bother them with the results of the task!
            //
            if ( backgroundWorker.CancellationPending )
            {
                e.Cancel = true;
                return;
            }

            Account acct2 = ExtendProxyToAccount( Patient2AccountProxy, hsv2From );
            if ( backgroundWorker.CancellationPending )
            {
                e.Cancel = true;
                return;
            }
            TransferService.QueueTransfer( acct1, acct2 );
        }

        private void AfterWork( object sender, RunWorkerCompletedEventArgs e )
        {
            if ( IsDisposed || Disposing )
                return;

            if ( e.Cancelled )
            {
                // user cancelled
                // Due to a race condition in the DoWork event handler, the Cancelled
                // flag may not have been set, even though CancelAsync was called.

                Refresh();
            }
            else if ( e.Error != null )
            {
                // handle errors/exceptions thrown from within the DoWork method here
                // What to do with the error?
                throw e.Error;
            }
            else
            {
                // success...
                userContextView.Description = "Swap Patient Locations - Submitted";
                infoControl1.DisplayErrorMessage( "Swap Patient Locations submitted for processing." );

                lblInstruction.Visible = false;
                mtbTransferDate.Visible = false;
                dateTimePicker.Visible = false;
                mtbTransferTimeEdit.Visible = false;

                panelTransferDateTime.Location = new Point( panelTransferDateTime.Location.X, 290 );
                lblConfirmTransferDateVal.Visible = true;
                lblConfirmTransferTimeVal.Visible = true;

                btnPrintFaceSheet1.Visible = true;
                btnPrintFaceSheet2.Visible = true;
                btnPrintFaceSheet1.Location = new Point( btnPrintFaceSheet1.Location.X, 350 );
                btnPrintFaceSheet2.Location = new Point( btnPrintFaceSheet2.Location.X, 350 );

                swapPatientLocationView1.ViewStatus = "ConfirmView";
                swapPatientLocationView1.Size = new Size( 344, 210 );
                swapPatientLocationView2.ViewStatus = "ConfirmView";
                swapPatientLocationView2.Size = new Size( 344, 210 );

                lblConfirmTransferDateVal.Text = mtbTransferDate.Text;
                lblConfirmTransferTimeVal.Text = mtbTransferTimeEdit.Text;

                btnCloseActivity.Focus();

                // clear any FUS notes that were manually entered so that - if the View FUS notes icon/menu option is selected,
                // the newly added notes do not show twice.
                if ( ViewFactory.Instance.CreateView<PatientAccessView>().Model != null )
                {
                    ( ( Account )ViewFactory.Instance.CreateView<PatientAccessView>().Model ).ClearFusNotes();
                }

                panelActions.Location = new Point( panelActions.Location.X, 380 );
                panelActions.Visible = true;

                btnOK.Visible = false;
                btnCancel.Visible = false;

                // OTD# 37031 fix - Disable icon/menu options for FUS notes upon confirmation
                ViewFactory.Instance.CreateView<PatientAccessView>().SetFUSNotesOptions( false );

                //Raise Activitycomplete event
                ActivityEventAggregator.GetInstance().RaiseActivityCompleteEvent( this,
                                                                                 new LooseArgs( Patient1AccountProxy ) );
            }

            // post completion operations...
            Cursor = Cursors.Default;
            progressPanel1.Visible = false;
            progressPanel1.SendToBack();
        }

        private void SwapPatientLocations( EventArgs e )
        {
            RunRules();

            if ( RuleEngine.AccountHasFailedError() )
            {
                btnOK.Enabled = true;
                return;
            }

            if ( backgroundWorker == null || !backgroundWorker.IsBusy )
            {
                BeforeWork();

                backgroundWorker = new BackgroundWorker {WorkerSupportsCancellation = true};

                backgroundWorker.DoWork += DoSwap;
                backgroundWorker.RunWorkerCompleted += AfterWork;

                backgroundWorker.RunWorkerAsync();
            }
        }

        private Account ExtendProxyToAccount( AccountProxy proxy, HospitalService hsvFrom )
        {
            //AccountPBARBroker
            Account account = AccountActivityService.SelectedAccountFor( proxy );
            proxy.Activity = new TransferBedSwapActivity();

            account.HospitalService = ( HospitalService )proxy.HospitalService.Clone();
            account.TransferredFromHospitalService = ( HospitalService )hsvFrom.Clone();
            account.LocationFrom = ( Location )proxy.LocationFrom.Clone();
            account.Location = ( Location )proxy.Location.Clone();
            account.LocationTo = account.Location;
            account.TransferDate = i_Account1.TransferDate;

            return account;
        }

        private DateTime GetTransferDateTime()
        {
            string date = mtbTransferDate.Text;
            string time = mtbTransferTimeEdit.UnMaskedText != string.Empty ? mtbTransferTimeEdit.Text : "00:00";

            return mtbTransferDate.UnMaskedText == string.Empty
                       ? DateTime.MinValue
                       : Convert.ToDateTime( date + " " + time );
        }

        private void UpdateTransferDateTimeAndRunRule()
        {
            i_Account1.TransferDate = GetTransferDateTime();

            UIColors.SetNormalBgColor( mtbTransferDate );
            UIColors.SetNormalBgColor( mtbTransferTimeEdit );

            RuleEngine.GetInstance().EvaluateRule( typeof( TransferDateRequired ), i_Account1 );
        }

        private void ChangeAcceptButton()
        {
            AcceptButton = btnOK;
        }

        private void RegisterRulesEvents()
        {
            RuleEngine.LoadRules( "PatientAccess.Domain.TransferBedSwapActivity" );

            RuleEngine.GetInstance().RegisterEvent( typeof( HospitalServiceToInPat1Required ),
                                                   swapPatientLocationView1.cboHospitalService,
                                                   new EventHandler( HospitalServiceToInPat1RequiredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( HospitalServiceToInPat2Required ),
                                                   swapPatientLocationView2.cboHospitalService,
                                                   new EventHandler( HospitalServiceToInPat2RequiredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( AccommodationToInPat1Required ),
                                                   swapPatientLocationView1.cboAccomodations1,
                                                   new EventHandler( AccommodationToInPat1RequiredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( AccommodationToInPat2Required ),
                                                   swapPatientLocationView2.cboAccomodations1,
                                                   new EventHandler( AccommodationToInPat2RequiredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( TransferDateRequired ), i_Account1,
                                                   new EventHandler( TransferDateRequiredEventHandler ) );
        }

        private void UnRegisterRulesEvents()
        {
            //          UNREGISTER EVENTS             
            RuleEngine.GetInstance().UnregisterEvent( typeof( HospitalServiceToInPat1Required ),
                                                     swapPatientLocationView1.cboHospitalService, HospitalServiceToInPat1RequiredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( HospitalServiceToInPat2Required ),
                                                     swapPatientLocationView2.cboHospitalService, HospitalServiceToInPat2RequiredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( AccommodationToInPat1Required ),
                                                     swapPatientLocationView1.cboAccomodations1, AccommodationToInPat1RequiredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( AccommodationToInPat2Required ),
                                                     swapPatientLocationView2.cboAccomodations1, AccommodationToInPat2RequiredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( TransferDateRequired ), i_Account1, TransferDateRequiredEventHandler );
        }

        private void RunRules()
        {
            // reset all fields that might have error, preferred, or required backgrounds          
            UIColors.SetNormalBgColor( swapPatientLocationView1.cboHospitalService );
            UIColors.SetNormalBgColor( swapPatientLocationView1.cboAccomodations1 );
            UIColors.SetNormalBgColor( swapPatientLocationView2.cboHospitalService );
            UIColors.SetNormalBgColor( swapPatientLocationView2.cboAccomodations1 );
            UIColors.SetNormalBgColor( mtbTransferDate );
            UIColors.SetNormalBgColor( mtbTransferTimeEdit );

            Refresh();

            RuleEngine.GetInstance().EvaluateRule( typeof( HospitalServiceToInPat1Required ), swapPatientLocationView1.cboHospitalService );
            RuleEngine.GetInstance().EvaluateRule( typeof( HospitalServiceToInPat2Required ), swapPatientLocationView2.cboHospitalService );
            RuleEngine.GetInstance().EvaluateRule( typeof( AccommodationToInPat1Required ), swapPatientLocationView1.cboAccomodations1 );
            RuleEngine.GetInstance().EvaluateRule( typeof( AccommodationToInPat2Required ), swapPatientLocationView2.cboAccomodations1 );
            RuleEngine.GetInstance().EvaluateRule( typeof( TransferDateRequired ), i_Account1 );
        }

        #endregion

        #region Private Properties

        private bool Patient1VerificationSucceeded
        {
            get { return i_Patient1VerificationSucceeded; }
            set { i_Patient1VerificationSucceeded = value; }
        }

        private bool Patient2VerificationSucceeded
        {
            get { return i_Patient2VerificationSucceeded; }
            set { i_Patient2VerificationSucceeded = value; }
        }

        private AccountProxy Patient1AccountProxy
        {
            get { return i_Patient1AccountProxy; }
            set { i_Patient1AccountProxy = value; }
        }

        private AccountProxy Patient2AccountProxy
        {
            get { return i_Patient2AccountProxy; }
            set { i_Patient2AccountProxy = value; }
        }

        private RuleEngine RuleEngine
        {
            get
            {
                if ( i_RuleEngine == null )
                {
                    i_RuleEngine = RuleEngine.GetInstance();
                }
                return i_RuleEngine;
            }
        }

        #endregion

        #region Construction and Finalization

        public SwapPatientLocationsView()
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();
            EnableThemesOn( this );
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            UnRegisterRulesEvents();

            if ( disposing )
            {
                if ( components != null )
                {
                    components.Dispose();
                }

                // cancel the background worker here...
                if ( backgroundWorker != null )
                {
                    backgroundWorker.CancelAsync();
                }
            }
            base.Dispose( disposing );
        }

        #endregion

        #region Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelUserContext = new Panel();
            this.userContextView = new UserContextView();
            this.label13 = new Label();
            this.btnOK = new ClickOnceLoggingButton();
            this.btnCancel = new LoggingButton();
            this.panelActions = new Panel();
            this.btnRepeatActivity = new LoggingButton();
            this.btnCloseActivity = new LoggingButton();
            this.lblAction = new Label();
            this.panelTransferInPatToOutPat = new Panel();
            this.lblInstruction = new Label();
            this.swapPatientLocationView2 = new SwapPatientLocationView();
            this.swapPatientLocationView1 = new SwapPatientLocationView();
            this.infoControl1 = new InfoControl();
            this.panelTransferDateTime = new Panel();
            this.mtbTransferTimeEdit = new MaskedEditTextBox();
            this.lblConfirmTransferTimeVal = new Label();
            this.mtbTransferDate = new MaskedEditTextBox();
            this.dateTimePicker = new DateTimePicker();
            this.lblConfirmTransferDateVal = new Label();
            this.lblTime = new Label();
            this.lblTransferDate = new Label();
            this.cboAccomodations = new ComboBox();
            this.progressPanel1 = new ProgressPanel();
            this.btnPrintFaceSheet1 = new LoggingButton();
            this.btnPrintFaceSheet2 = new LoggingButton();
            this.panelUserContext.SuspendLayout();
            this.panelActions.SuspendLayout();
            this.panelTransferInPatToOutPat.SuspendLayout();
            this.panelTransferDateTime.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelUserContext
            // 
            this.panelUserContext.Controls.Add( this.userContextView );
            this.panelUserContext.Dock = DockStyle.Top;
            this.panelUserContext.Location = new Point( 0, 0 );
            this.panelUserContext.Name = "panelUserContext";
            this.panelUserContext.Size = new Size( 1024, 22 );
            this.panelUserContext.TabIndex = 0;
            // 
            // userContextView
            // 
            this.userContextView.BackColor = SystemColors.Control;
            this.userContextView.Description = "Swap Patient Locations";
            this.userContextView.Dock = DockStyle.Fill;
            this.userContextView.Location = new Point( 0, 0 );
            this.userContextView.Model = null;
            this.userContextView.Name = "userContextView";
            this.userContextView.Size = new Size( 1024, 22 );
            this.userContextView.TabIndex = 0;
            this.userContextView.TabStop = false;
            // 
            // label13
            // 
            this.label13.Location = new Point( 0, 0 );
            this.label13.Name = "label13";
            this.label13.TabIndex = 0;
            // 
            // btnOK
            // 
            this.btnOK.BackColor = SystemColors.Control;
            this.btnOK.Enabled = false;
            this.btnOK.Location = new Point( 864, 592 );
            this.btnOK.Name = "btnOK";
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new EventHandler( this.btnOk_Click );
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = SystemColors.Control;
            this.btnCancel.CausesValidation = false;
            this.btnCancel.Location = new Point( 944, 592 );
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new EventHandler( this.btnCancel_Click );
            // 
            // panelActions
            // 
            this.panelActions.BackColor = Color.White;
            this.panelActions.Controls.Add( this.btnRepeatActivity );
            this.panelActions.Controls.Add( this.btnCloseActivity );
            this.panelActions.Controls.Add( this.lblAction );
            this.panelActions.Location = new Point( 16, 480 );
            this.panelActions.Name = "panelActions";
            this.panelActions.Size = new Size( 968, 64 );
            this.panelActions.TabIndex = 5;
            this.panelActions.Visible = false;
            // 
            // btnRepeatActivity
            // 
            this.btnRepeatActivity.Location = new Point( 102, 40 );
            this.btnRepeatActivity.Name = "btnRepeatActivity";
            this.btnRepeatActivity.Size = new Size( 88, 23 );
            this.btnRepeatActivity.TabIndex = 2;
            this.btnRepeatActivity.Text = "Repeat Acti&vity";
            this.btnRepeatActivity.Click += new EventHandler( this.btnRepeatActivity_Click );
            // 
            // btnCloseActivity
            // 
            this.btnCloseActivity.Location = new Point( 7, 40 );
            this.btnCloseActivity.Name = "btnCloseActivity";
            this.btnCloseActivity.Size = new Size( 88, 23 );
            this.btnCloseActivity.TabIndex = 1;
            this.btnCloseActivity.Text = "C&lose Activity";
            this.btnCloseActivity.Click += new EventHandler( this.btnCloseActivity_Click );
            // 
            // lblAction
            // 
            this.lblAction.BackColor = Color.White;
            this.lblAction.FlatStyle = FlatStyle.System;
            this.lblAction.ForeColor = Color.Black;
            this.lblAction.Location = new Point( 9, 10 );
            this.lblAction.Name = "lblAction";
            this.lblAction.Size = new Size( 986, 16 );
            this.lblAction.TabIndex = 0;
            this.lblAction.Text = "Next Action _____________________________________________________________________" +
                                  "________________________________________________________________________________" +
                                  "____";
            // 
            // panelTransferInPatToOutPat
            // 
            this.panelTransferInPatToOutPat.BackColor = Color.White;
            this.panelTransferInPatToOutPat.Controls.Add( this.btnPrintFaceSheet2 );
            this.panelTransferInPatToOutPat.Controls.Add( this.btnPrintFaceSheet1 );
            this.panelTransferInPatToOutPat.Controls.Add( this.lblInstruction );
            this.panelTransferInPatToOutPat.Controls.Add( this.swapPatientLocationView2 );
            this.panelTransferInPatToOutPat.Controls.Add( this.swapPatientLocationView1 );
            this.panelTransferInPatToOutPat.Controls.Add( this.infoControl1 );
            this.panelTransferInPatToOutPat.Controls.Add( this.panelTransferDateTime );
            this.panelTransferInPatToOutPat.Controls.Add( this.panelActions );
            this.panelTransferInPatToOutPat.Controls.Add( this.progressPanel1 );
            this.panelTransferInPatToOutPat.Location = new Point( 8, 24 );
            this.panelTransferInPatToOutPat.Name = "panelTransferInPatToOutPat";
            this.panelTransferInPatToOutPat.Size = new Size( 1008, 560 );
            this.panelTransferInPatToOutPat.TabIndex = 0;
            // 
            // lblInstruction
            // 
            this.lblInstruction.Location = new Point( 16, 48 );
            this.lblInstruction.Name = "lblInstruction";
            this.lblInstruction.Size = new Size( 640, 16 );
            this.lblInstruction.TabIndex = 1;
            this.lblInstruction.Text =
                "If patient account is unknown, use the Census by Patient activity to determine th" +
                "e account.";
            // 
            // swapPatientLocationView2
            // 
            this.swapPatientLocationView2.BackColor = Color.White;
            this.swapPatientLocationView2.Location = new Point( 368, 64 );
            this.swapPatientLocationView2.Model = null;
            this.swapPatientLocationView2.Name = "swapPatientLocationView2";
            this.swapPatientLocationView2.NewLocation = null;
            this.swapPatientLocationView2.OtherAccountProxy = null;
            this.swapPatientLocationView2.PatientLabel = "Patient 1";
            this.swapPatientLocationView2.Size = new Size( 336, 408 );
            this.swapPatientLocationView2.TabIndex = 3;
            this.swapPatientLocationView2.ViewStatus = null;
            this.swapPatientLocationView2.NewAccommodationCodeChanged += new EventHandler( this.cboPat2ACCChanged );
            this.swapPatientLocationView2.NewHSVCodeChanged += new EventHandler( this.cboPat2HSVChanged );
            this.swapPatientLocationView2.VerificationFailed += new EventHandler( this.Pat2VerificationFailed );
            this.swapPatientLocationView2.LeaveSearchField +=
                new EventHandler( this.swapPatientLocationView2_LeaveSearchField );
            this.swapPatientLocationView2.VerificationSucceeded += new EventHandler( this.Pat2VerificationSucceeded );
            // 
            // swapPatientLocationView1
            // 
            this.swapPatientLocationView1.BackColor = Color.White;
            this.swapPatientLocationView1.Location = new Point( 16, 64 );
            this.swapPatientLocationView1.Model = null;
            this.swapPatientLocationView1.Name = "swapPatientLocationView1";
            this.swapPatientLocationView1.NewLocation = null;
            this.swapPatientLocationView1.OtherAccountProxy = null;
            this.swapPatientLocationView1.PatientLabel = "Patient 1";
            this.swapPatientLocationView1.Size = new Size( 336, 408 );
            this.swapPatientLocationView1.TabIndex = 2;
            this.swapPatientLocationView1.ViewStatus = null;
            this.swapPatientLocationView1.NewAccommodationCodeChanged += this.cboPat1ACCChanged;
            this.swapPatientLocationView1.NewHSVCodeChanged += this.cboPat1HSVChanged;
            this.swapPatientLocationView1.VerificationFailed += this.Pat1VerificationFailed;
            this.swapPatientLocationView1.LeaveSearchField += this.swapPatientLocationView1_LeaveSearchField;
            this.swapPatientLocationView1.VerificationSucceeded += this.Pat1VerificationSucceeded;
            // 
            // infoControl1
            // 
            this.infoControl1.Location = new Point( 24, 16 );
            this.infoControl1.Message = string.Empty;
            this.infoControl1.Name = "infoControl1";
            this.infoControl1.Size = new Size( 960, 24 );
            this.infoControl1.TabIndex = 0;
            this.infoControl1.TabStop = false;
            // 
            // panelTransferDateTime
            // 
            this.panelTransferDateTime.Controls.Add( this.mtbTransferTimeEdit );
            this.panelTransferDateTime.Controls.Add( this.lblConfirmTransferTimeVal );
            this.panelTransferDateTime.Controls.Add( this.mtbTransferDate );
            this.panelTransferDateTime.Controls.Add( this.dateTimePicker );
            this.panelTransferDateTime.Controls.Add( this.lblConfirmTransferDateVal );
            this.panelTransferDateTime.Controls.Add( this.lblTime );
            this.panelTransferDateTime.Controls.Add( this.lblTransferDate );
            this.panelTransferDateTime.Location = new Point( 336, 464 );
            this.panelTransferDateTime.Name = "panelTransferDateTime";
            this.panelTransferDateTime.Size = new Size( 384, 40 );
            this.panelTransferDateTime.TabIndex = 4;
            // 
            // mtbTransferTimeEdit
            // 
            this.mtbTransferTimeEdit.Enabled = false;
            this.mtbTransferTimeEdit.EntrySelectionStyle = EntrySelectionStyle.SelectionAtEnd;
            this.mtbTransferTimeEdit.KeyPressExpression = DateValidator.TIMEKeyPressExpression;
            this.mtbTransferTimeEdit.Location = new Point( 290, 13 );
            this.mtbTransferTimeEdit.Mask = "  :";
            this.mtbTransferTimeEdit.MaxLength = 5;
            this.mtbTransferTimeEdit.Multiline = true;
            this.mtbTransferTimeEdit.Name = "mtbTransferTimeEdit";
            this.mtbTransferTimeEdit.Size = new Size( 37, 20 );
            this.mtbTransferTimeEdit.TabIndex = 1;
            this.mtbTransferTimeEdit.ValidationExpression = DateValidator.TIMEValidationExpression;
            this.mtbTransferTimeEdit.Validating += new CancelEventHandler( this.mtbTransferTimeEdit_Validating );
            // 
            // lblConfirmTransferTimeVal
            // 
            this.lblConfirmTransferTimeVal.Location = new Point( 280, 15 );
            this.lblConfirmTransferTimeVal.Name = "lblConfirmTransferTimeVal";
            this.lblConfirmTransferTimeVal.Size = new Size( 32, 14 );
            this.lblConfirmTransferTimeVal.TabIndex = 55;
            this.lblConfirmTransferTimeVal.Visible = false;
            // 
            // mtbTransferDate
            // 
            this.mtbTransferDate.Enabled = false;
            this.mtbTransferDate.EntrySelectionStyle = EntrySelectionStyle.SelectionAtEnd;
            this.mtbTransferDate.KeyPressExpression = DateValidator.DATEKeyPressExpression;
            this.mtbTransferDate.Location = new Point( 113, 13 );
            this.mtbTransferDate.Mask = "  /  /";
            this.mtbTransferDate.MaxLength = 10;
            this.mtbTransferDate.Name = "mtbTransferDate";
            this.mtbTransferDate.Size = new Size( 66, 20 );
            this.mtbTransferDate.TabIndex = 0;
            this.mtbTransferDate.ValidationExpression = DateValidator.DATEValidationExpression;
            this.mtbTransferDate.Validating += new CancelEventHandler( this.mtbTransferDate_Validating );
            // 
            // dateTimePicker
            // 
            this.dateTimePicker.CalendarTitleForeColor = SystemColors.ControlLightLight;
            this.dateTimePicker.Checked = false;
            this.dateTimePicker.Enabled = false;
            this.dateTimePicker.Format = DateTimePickerFormat.Short;
            this.dateTimePicker.Location = new Point( 177, 13 );
            this.dateTimePicker.MinDate = new DateTime( 1800, 1, 1, 0, 0, 0, 0 );
            this.dateTimePicker.Name = "dateTimePicker";
            this.dateTimePicker.Size = new Size( 21, 20 );
            this.dateTimePicker.TabIndex = 51;
            this.dateTimePicker.TabStop = false;
            this.dateTimePicker.CloseUp += new EventHandler( this.dateTimePicker_CloseUp );
            // 
            // lblConfirmTransferDateVal
            // 
            this.lblConfirmTransferDateVal.Location = new Point( 114, 15 );
            this.lblConfirmTransferDateVal.Name = "lblConfirmTransferDateVal";
            this.lblConfirmTransferDateVal.Size = new Size( 64, 15 );
            this.lblConfirmTransferDateVal.TabIndex = 54;
            this.lblConfirmTransferDateVal.Visible = false;
            // 
            // lblTime
            // 
            this.lblTime.Location = new Point( 247, 15 );
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new Size( 35, 21 );
            this.lblTime.TabIndex = 50;
            this.lblTime.Text = "Time:";
            // 
            // lblTransferDate
            // 
            this.lblTransferDate.Location = new Point( 37, 15 );
            this.lblTransferDate.Name = "lblTransferDate";
            this.lblTransferDate.Size = new Size( 78, 21 );
            this.lblTransferDate.TabIndex = 49;
            this.lblTransferDate.Text = "Transfer date:";
            // 
            // cboAccomodations
            // 
            this.cboAccomodations.Location = new Point( 0, 0 );
            this.cboAccomodations.Name = "cboAccomodations";
            this.cboAccomodations.TabIndex = 0;
            // 
            // progressPanel1
            // 
            this.progressPanel1.BackColor = Color.White;
            this.progressPanel1.Location = new Point( 12, 12 );
            this.progressPanel1.Name = "progressPanel1";
            this.progressPanel1.Size = new Size( 980, 532 );
            this.progressPanel1.TabIndex = 6;
            // 
            // btnPrintFaceSheet1
            // 
            this.btnPrintFaceSheet1.Location = new Point( 112, 2456 );
            this.btnPrintFaceSheet1.Message = null;
            this.btnPrintFaceSheet1.Name = "btnPrintFaceSheet1";
            this.btnPrintFaceSheet1.Size = new Size( 120, 23 );
            this.btnPrintFaceSheet1.TabIndex = 7;
            this.btnPrintFaceSheet1.Text = "Print Face Sheet";
            this.btnPrintFaceSheet1.Click += this.btnPrintFaceSheet1_Click;
            this.btnPrintFaceSheet1.Visible = true;
            // 
            // btnPrintFaceSheet2
            // 
            this.btnPrintFaceSheet2.Location = new Point( 490, 2456 );
            this.btnPrintFaceSheet2.Message = null;
            this.btnPrintFaceSheet2.Name = "btnPrintFaceSheet2";
            this.btnPrintFaceSheet2.Size = new Size( 120, 23 );
            this.btnPrintFaceSheet2.TabIndex = 8;
            this.btnPrintFaceSheet2.Text = "Print Face Sheet";
            this.btnPrintFaceSheet2.Click += this.btnPrintFaceSheet2_Click;
            this.btnPrintFaceSheet2.Visible = true;
            // 
            // SwapPatientLocationsView
            // 
            this.Load += new EventHandler( SwapPatientLocationsView_Load );
            this.AcceptButton = this.btnOK;
            this.BackColor = Color.FromArgb( 209, 228, 243 );
            this.Controls.Add( this.btnCancel );
            this.Controls.Add( this.btnOK );
            this.Controls.Add( this.panelTransferInPatToOutPat );
            this.Controls.Add( this.panelUserContext );
            this.Name = "SwapPatientLocationsView";
            this.Size = new Size( 1024, 620 );
            this.panelUserContext.ResumeLayout( false );
            this.panelActions.ResumeLayout( false );
            this.panelTransferInPatToOutPat.ResumeLayout( false );
            this.panelTransferDateTime.ResumeLayout( false );
            this.ResumeLayout( false );
        }

        #endregion

        #region Data Elements

        private BackgroundWorker backgroundWorker;
        private LoggingButton btnCancel;
        private LoggingButton btnCloseActivity;
        private ClickOnceLoggingButton btnOK;
        private LoggingButton btnPrintFaceSheet1;
        private LoggingButton btnPrintFaceSheet2;
        private LoggingButton btnRepeatActivity;
        private ComboBox cboAccomodations;
        private IContainer components = null;
        private DateTimePicker dateTimePicker;
        private Account i_Account1 = new Account();
        private Account i_Account2 = new Account();
        private DateTime i_FacilityDateTime;
        private AccountProxy i_Patient1AccountProxy;
        private bool i_Patient1VerificationSucceeded;
        private AccountProxy i_Patient2AccountProxy;
        private bool i_Patient2VerificationSucceeded;
        private RuleEngine i_RuleEngine;
        private InfoControl infoControl1;

        private Label label13;
        private Label lblAction;
        private Label lblConfirmTransferDateVal;

        public Label lblConfirmTransferTimeVal;
        private Label lblInstruction;
        private Label lblTime;
        private Label lblTransferDate;
        private MaskedEditTextBox mtbTransferDate;

        public MaskedEditTextBox mtbTransferTimeEdit;
        private Panel panelActions;
        private Panel panelTransferDateTime;
        private Panel panelTransferInPatToOutPat;
        private Panel panelUserContext;
        private ProgressPanel progressPanel1;
        private SwapPatientLocationView swapPatientLocationView1;
        private SwapPatientLocationView swapPatientLocationView2;
        private UserContextView userContextView;

        #endregion

        #region Constants

        #endregion
    }
}