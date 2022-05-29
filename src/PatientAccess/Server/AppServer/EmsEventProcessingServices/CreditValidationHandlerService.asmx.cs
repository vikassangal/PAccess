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
    public class CreditValidationHandlerService : WebService
    {

        private static ILog c_Logger =
            LogManager.GetLogger( typeof( CreditValidationHandlerService ) );

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>The logger.</value>
        private static ILog Logger
        {

            get
            {

                return c_Logger;

            }//get

        }//property

        /// <summary>
        /// Initializes a new instance of the <see cref="BenefitsValidationHandlerService"/> class.
        /// </summary>
        public CreditValidationHandlerService()
        {
            //CODEGEN: This call is required by the ASP.NET Web Services Designer
            InitializeComponent();
        }

        #region Component Designer generated code

        //Required by the Web Services Designer 
        private IContainer components = null;

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing && components != null )
            {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #endregion

        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="messageType">Type of the message.</param>
        [WebMethod( Description = "Process Credit Save Message", EnableSession = false )]
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

                    }//if
                    else
                    {

                        Logger.Warn( "Credit validation ticket was empty and not saved" );

                    }//else

                }//if
                else
                {

                    Logger.Warn( "Credit validation ticket was null and not saved" );

                }//else


            }//try
            catch( Exception theException )
            {

                Logger.Error( "Unable to save credit response indicator",
                              theException );

                throw new SoapException( theException.Message,
                                         SoapException.ServerFaultCode,
                                         theException.StackTrace );
            }//catch

        }//method

    }//class

}//namespace