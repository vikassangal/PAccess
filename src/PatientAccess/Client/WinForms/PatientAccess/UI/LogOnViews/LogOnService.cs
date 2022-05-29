using System;
using System.Collections;
using System.Net.NetworkInformation;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Security;
using PatientAccess.RemotingServices;
using log4net;
using PatientAccess.Rules;
using PatientAccess.UI.Logging;

namespace PatientAccess.UI.LogOnViews
{
	/// <summary>
	/// Summary description for LogOnService.
	/// </summary>
    [Serializable]
    public class LogOnService
    {
        #region Event Handlers
        #endregion

        #region Methods
        public void SaveUserPreference( UserPreference preferences )
        {
            preferences.Save( preferences );
        }

        public UserPreference GetUserPreference()
        {
            UserPreference preferences = UserPreference.Load();
            return preferences;
        }

        public ICollection AllFacilities()
        {
            IFacilityBroker broker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            ICollection allFacilities = broker.AllFacilities();
            return allFacilities;        
        }

        public SecurityResponse LogOn( string userName, string password, Facility selectedFacility)
        {
            SecurityResponse securityResponse ;
            var userBroker = BrokerFactory.BrokerOfType<IUserBroker>();
            var OKTAUserBroker = BrokerFactory.BrokerOfType<IOKTAUserBroker>();
            OKTAServiceFeatureManager OKTAServiceFeatureManager = new OKTAServiceFeatureManager();
            bool IsOKTAEnabled = OKTAServiceFeatureManager.IsOKTAEnabled(selectedFacility);
            if (!IsOKTAEnabled)
            {
                securityResponse = userBroker.AuthenticateUser(userName, password, selectedFacility);
            }
            else
            {
               securityResponse = OKTAUserBroker.AuthenticateUser(userName, password, selectedFacility);
            }
            
            if( securityResponse.CanLogin() )
            {
                User.SetCurrentUserTo( securityResponse.PatientAccessUser );
               
                var userId = User.GetCurrent().SecurityUser.UPN;

                SetCallContextData( userId );

                var osVersionLogMessage = GetOsVersionLogMessage( userId );

                Logger.Info( osVersionLogMessage );
            }
            else
            {
                BreadCrumbLogger.GetInstance.Log(securityResponse.LoginFailureMessage);
            }

            return securityResponse;
        }

	    public void Credentials ( out string userName, out string password, out string workstationID )
        {
            IUserBroker userBroker = BrokerFactory.BrokerOfType<IUserBroker>();
            userBroker.Credentials( out userName, out password, out workstationID );
        }
        #endregion

        #region Properties

	    private ILog Logger { get; set; }

	    #endregion

        #region Private Methods

	    private string GetOsVersionLogMessage(string userId)
	    {
	        var operatingSystemVersion = Environment.OSVersion.ToString();
                
	        var macAddress = GetMacAddress();

	        var windowsUserName = Environment.UserName;


	        return string.Format( "User [{0}] from facility [{1}] logged on with windows user name [{2}] using operating system [{3}] with MAC address [{4}]",
	                              userId, User.GetCurrent().Facility.Code, windowsUserName, operatingSystemVersion, macAddress );
	    }

	    private string GetMacAddress()
	    {
	        var macAddress = string.Empty;
                
	        foreach ( var networkInterface in NetworkInterface.GetAllNetworkInterfaces() )
	        {
	            if ( networkInterface.OperationalStatus == OperationalStatus.Up )
	            {
	                macAddress = networkInterface.GetPhysicalAddress().ToString();
	                break;
	            }
	        }
	        return macAddress;
	    }

	    private void SetCallContextData(string userId)
	    {
	        var callContextAccesor = new CallContextAccessor();
	        var contextData = new CallContextData( userId );
	        callContextAccesor.SetContext( contextData );
	    }

	    #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public LogOnService(ILog logger)
        {
            this.Logger = logger;
        }

	    #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
