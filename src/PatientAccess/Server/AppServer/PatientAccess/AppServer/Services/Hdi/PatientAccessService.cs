using System;
using System.Net;
using PatientAccess.Services;

namespace PatientAccess.HDI
{
    /// <summary>
    /// CieWebServiceProxy is a simple wrapper around the auto-generated web service
    /// proxy class PatientAccessServiceEngine.  There are methods we override here, namely
    /// GetWebRequest, which we do not want to lose the next time PatientAccessServiceEngine is
    /// auto generated.
    /// </summary>

    [Serializable]
    public partial class PatientAccessService : IPatientAccessService 
    {
        #region Event Handlers
        #endregion

        #region Methods
        /// <summary>
        /// We're overriding GetWebRequest so that we can set the Http Keep-Alive
        /// value to false.  This seems to be the recommended way to resolve the 
        /// ubiquitous "underlying connection was closed" problem.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        protected override WebRequest GetWebRequest( Uri uri )
        {
            WebRequest baseRequest = base.GetWebRequest( uri );
            WebRequest result = null;
            HttpWebRequest httpRequest = baseRequest as HttpWebRequest;

            if( httpRequest != null )
            {
                httpRequest.KeepAlive = false;
                result = httpRequest;
            }
            else
            {
                result = baseRequest;
            }

            return result;
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public PatientAccessService( string serviceUrl )
            : base()
        {
            base.Url = serviceUrl;
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        private const string REQUEST_PROPERTY_NAME = "Request";
        #endregion
    }
}
