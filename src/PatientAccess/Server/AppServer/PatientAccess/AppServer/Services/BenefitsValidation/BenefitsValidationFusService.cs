using System;
using System.Configuration;
using System.Net;
using PatientAccess.Services;

namespace PatientAccess.BenefitsValidationFusProxy
{

    /// <summary>
    /// Summary description for BenefitsValidationFusService
    /// </summary>
    public partial class BenefitsValidationFusService : IBenefitsValidationFusService 
    {

        #region Constants & Enumerations

        #endregion Constants & Enumerations

        #region Events and Delegates

        #endregion Events and Delegates

        #region Fields

        #endregion Fields

        #region Properties

        #endregion Properties

        #region Construction & Finalization

        /// <summary>
        /// Initializes a new instance of the <see cref="BenefitsValidationFusService"/> class.
        /// </summary>
        /// <param name="serviceUrl">The service URL.</param>
        public BenefitsValidationFusService( string serviceUrl ) : base()
        {

            base.Url = serviceUrl;

        }//method

        #endregion Construction & Finalization

        #region Public Methods

        #endregion Public Methods

        #region Non-Public Methods

        /// <summary>
        /// Gets the web request.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns></returns>
        /// <remarks>
        /// This allows us to stay functioning in a load balanced environment
        /// </remarks>
        protected override WebRequest GetWebRequest( Uri uri )
        {

            HttpWebRequest webRequest = (HttpWebRequest)base.GetWebRequest( uri );

            if( ( null != ConfigurationManager.AppSettings["WEB_REQUEST_DEFAULTS"] ) &&
                ( !bool.Parse( ConfigurationManager.AppSettings["WEB_REQUEST_DEFAULTS"] ) ) )
            {

                webRequest.KeepAlive =
                    bool.Parse( ConfigurationManager.AppSettings["WEB_REQUEST_KEEP_ALIVE"] );
                webRequest.ProtocolVersion =
                    new Version( ConfigurationManager.AppSettings["WEB_REQUEST_PROTOCOL_VERSION"] );

            }//if

            return webRequest;

        }//method

        #endregion Non-Public Methods

        #region Event Handlers

        #endregion Event Handlers

    }//class

}//namespace