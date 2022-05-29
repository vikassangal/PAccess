using System;
using PatientAccess.Services;

namespace PatientAccess.PriorAccountBalanceProxy
{

    /// <summary>
    /// Summary description for PriorAccountBalanceService
    /// </summary>
    public partial class PriorAccountBalanceService : IPriorAccountBalanceService 
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
        /// Initializes a new instance of the <see cref="PriorAccountBalanceService"/> class.
        /// </summary>
        /// <param name="serviceUrl">The service URL.</param>
        public PriorAccountBalanceService( string serviceUrl )
        {

           // base.Url = serviceUrl;
        
        }//method

        #endregion Construction & Finalization

        #region Public Methods

        #endregion Public Methods

        #region Non-Public Methods
        
        #endregion Non-Public Methods

        #region Event Handlers

        #endregion Event Handlers


        public void CancelAsync(object userState)
        {
            throw new NotImplementedException();
        }

        public priorAccountBalanceRequest identifyPriorAccountBalances(priorAccountBalanceRequest identifyPriorAccountBalances1)
        {
            throw new NotImplementedException();
        }

        public string Url
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool UseDefaultCredentials
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }//class

}//namespace