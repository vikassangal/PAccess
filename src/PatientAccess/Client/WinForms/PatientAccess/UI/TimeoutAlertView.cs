using System;
using System.ComponentModel;
using System.Timers;
using System.Windows.Forms;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.HelperClasses;
using Timer = System.Timers.Timer;

namespace PatientAccess.UI
{
	/// <summary>
	/// Summary description for TimeoutAlert.
	/// </summary>
    public class TimeoutAlertView : TimeOutFormView
    {   
        #region Event Handlers
        private void timer1_Tick( object sender, ElapsedEventArgs e )
        {
            timespan = timespan.Subtract( this.timespanInterval );
            this.warningLabel.Text = String.Format( 
                UIErrorMessages.ACTIVITY_TIMEOUT_ALERT,
                timespan.Duration().ToString().Substring(3) );

            if( timespan.TotalMilliseconds == 0 )
            {
                this.Cursor = Cursors.WaitCursor;

                this.timer1.Stop();
                this.timer1 = null;

                if( !this.Visible )
                {
                    this.Visible = true;
                    this.Refresh();
                }
                this.warningLabel.Text = UIErrorMessages.ACTIVITY_END_MSG;

                ActivityEventAggregator.GetInstance().RaiseActivityCancelEvent( 
                    this, new LooseArgs( Model_Account ) );

                ActivityEventAggregator.GetInstance().RaiseReturnToMainScreen( this, e );

                this.Focus();
                this.Cursor = Cursors.Default;
            }
        }
        private void buttonOK_Click( object sender, EventArgs e )
        {
                this.Hide();
        }

        private void TimeoutAlert_ActivityStop(object sender, EventArgs e)
        {
            if( this.timer1 != null ) 
            {
                this.timer1.Stop();
                this.timer1 = null;
            }
            this.Model = null;
            this.Dispose();
        }

       #endregion
	
        #region Methods
        public override void UpdateView()
        {            
            if( this.GetModel().TotalMilliseconds > 0 )
            {
                this.warningLabel.Text = String.Format( 
                    UIErrorMessages.ACTIVITY_TIMEOUT_ALERT, 
                    timespan.Duration().ToString().Substring(3) );

                if( this.timer1 == null )
                {
                    this.timer1 = new Timer();
                }
                this.timer1.Start();
                this.timer1.Interval = COUNTDOWN_INTERVAL;
				this.timer1.SynchronizingObject = this;	
                this.timer1.Elapsed				+= new ElapsedEventHandler(timer1_Tick);
            }
        }
        public override void UpdateModel()
        {
            int ms = Convert.ToInt32( Model_Account.Activity.Timeout.SecondAlertTime );
            this.timespan = new TimeSpan( 0,0,0,0, ms);
        }        
        #endregion
	
        #region Properties

	    private IAccount Model_Account
        {
            get
            {
                return this.Model as IAccount;
            }
        }
        #endregion
	
        #region Private Methods     

        private TimeSpan GetModel()
        {
            return this.timespan;
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            ActivityEventAggregator.GetInstance().ActivityCancelled -=
                new EventHandler( TimeoutAlert_ActivityStop );
            ActivityEventAggregator.GetInstance().ActivityCompleted -=
                new EventHandler( TimeoutAlert_ActivityStop );
            ActivityEventAggregator.GetInstance().AccountUnLocked -=
                new EventHandler( TimeoutAlert_ActivityStop );

            if( disposing )
            {
                if(components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(TimeoutAlertView));
            this.warningLabel = new System.Windows.Forms.Label();
            this.buttonOK = new LoggingButton();
            this.timer1 = new System.Timers.Timer();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.SuspendLayout();
            // 
            // warningLabel
            // 
            this.warningLabel.Location = new System.Drawing.Point(64, 8);
            this.warningLabel.Name = "warningLabel";
            this.warningLabel.Size = new System.Drawing.Size(240, 80);
            this.warningLabel.TabIndex = 0;
            this.warningLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(123, 96);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.TabIndex = 3;
            this.buttonOK.Text = "OK";
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(8, 24);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(34, 34);
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // TimeoutAlert
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(320, 126);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.warningLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TimeoutAlert";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Warning";
            this.TopMost = true;
            this.ResumeLayout(false);

        }
        #endregion
        
        #endregion
	
        #region Private Properties
        #endregion
	
        #region Construction and Finalization
        public TimeoutAlertView()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            base.EnableThemesOn( this ); 
            ActivityEventAggregator.GetInstance().ActivityCancelled +=
                new EventHandler( TimeoutAlert_ActivityStop );
            ActivityEventAggregator.GetInstance().ActivityCompleted +=
                new EventHandler( TimeoutAlert_ActivityStop );
            ActivityEventAggregator.GetInstance().AccountUnLocked +=
                new EventHandler( TimeoutAlert_ActivityStop );

            base.EnableThemesOn( this );
        }
        #endregion
	
        #region Data Elements

        private LoggingButton buttonOK;
        private Timer timer1;
        private IContainer components;    
        private TimeSpan timespan = new TimeSpan(0);
        private TimeSpan timespanInterval = new TimeSpan(0,0,0,1,0);
        private Label warningLabel;
        private PictureBox pictureBox1;
        #endregion
	
        #region Constants
	    private const int COUNTDOWN_INTERVAL = 1000;
        #endregion        
    }
}
