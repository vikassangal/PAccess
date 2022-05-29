using System;
using System.ComponentModel;
using System.Web.Services;
using System.Web.Services.Protocols;
using PatientAccess.BrokerInterfaces;
using log4net;

namespace PatientAccess.Services.EmsEventProcessing
{

    /// <summary>
    /// Summary description for BenefitsValidationHandlerService.
    /// </summary>
    public class BenefitsValidationHandlerService : WebService
    {
		#region Fields 

        private static readonly ILog _logger = 
            LogManager.GetLogger( typeof( BenefitsValidationHandlerService ) );
        //Required by the Web Services Designer 
        private IContainer components = null;

		#endregion Fields 

		#region Constructors 

        /// <summary>
        /// Initializes a new instance of the <see cref="BenefitsValidationHandlerService"/> class.
        /// </summary>
        public BenefitsValidationHandlerService()
        {
            //CODEGEN: This call is required by the ASP.NET Web Services Designer
            InitializeComponent();
        }

		#endregion Constructors 

		#region Properties 

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>The logger.</value>
        private static ILog Logger
        {
            get
            {
                return _logger;
            }
        }

		#endregion Properties 

		#region Methods 

        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="messageType">Type of the message.</param>
        [WebMethod(Description="Process Save Message",EnableSession=false)]
        public void ProcessMessage( string message, string messageType )
        {

            try
            {

                IDataValidationBroker aDataValidationBroker =
                    BrokerFactory.BrokerOfType<IDataValidationBroker>();


                if( null != message )
                {

                    string ticket = message.Trim();

                    if( !String.IsNullOrEmpty( ticket ) )
                    {

                        aDataValidationBroker.SaveResponseIndicator( ticket, true );

                    }
                    else
                    {

                        Logger.Warn( "Benefits validation ticket was empty and not saved" );

                    }

                }
                else
                {

                    Logger.Warn( "Benefits validation ticket was null and not saved" );

                }

                
            }
            catch( Exception theException )
            {

                Logger.Error( "Unable to save benefits response indicator", 
                              theException );

                throw new SoapException( theException.Message, 
                                         SoapException.ServerFaultCode, 
                                         theException.StackTrace );
            }

        }
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if(disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);		
        }
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
        }

		#endregion Methods 
    }

}