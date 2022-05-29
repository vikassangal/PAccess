using System;
using System.Reflection;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Domain
{
    [Serializable]
    public class User : Person, IUser
    {
        #region Methods

        /// <summary>
        /// Gets the current user.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">User.GetCurrent() was executed in an invalid context.  It is only 'legal' to use User as a singleton in a non-shared enviornment such as on a user's desktop.  It is invalid to use User as a singleton when executing an a shared enviornment such as inside a Broker executing on the Application Server.</exception>
        public static User GetCurrent()
        {
            // Verify that GetCurrent is not being called in a shared context environment
            // such as executing from our AppServer.  If an instance of User is needed
            // in, for example a Broker, than the static factory method NewInstance() should be 
            // used instead.
            if( IsExecutingOnServer() )
            {
                throw new InvalidOperationException(
                    INVALID_IN_SHARED_ENV_MSG
                    );
            }
                
            if( c_instance == null )
            {
                lock( c_syncRoot )
                {
                    if( c_instance == null )
                    {
                        c_instance = new User();
                    }
                }
            }
            return c_instance;
        }

        public static void SetCurrentUserTo( User user )
        {
            c_instance = user;
        }

        public static User NewInstance()
        {
            return ( new User() );
        }
        #endregion

        #region Properties
        

        public Facility Facility
        {
            get
            {
                return i_Facility;
            }
            set
            {
                i_Facility = value;
            }
        }
       
        public string UserID
        {
            get
            {
                return i_UserID;
            }
            set
            {
                i_UserID = value;
            }
        }
        
        public string PBARSecurityCode
        {
            get
            {
                return i_PBARSecurityCode;
            }
            set
            {
                i_PBARSecurityCode = value;
            }
        }

        public Extensions.SecurityService.Domain.User SecurityUser
        {
            get
            {
                return i_SecurityUser;
            }
            set
            {
                i_SecurityUser = value;
            }
        }

        public string PBAREmployeeID
        {
            get
            {
                return i_PBAREmployeeID;
            }
            set
            {
                i_PBAREmployeeID = value;
            }
        }

        public string WorkstationID
        {
            get
            {
                return i_WorkstationID;
            }
            set
            {
                i_WorkstationID = value;
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Enumerates a list of known assemblies which if GetUser() is called from
        /// would be invalid, since they operate by default as a shared enviornment.
        /// e.g. Calling User.GetCurrent() when executing in a broker would share the 
        /// state of the User object between all other broker calls on the server therefore
        /// providing a User object with invalid data.
        /// </summary>
        /// <returns></returns>
        internal static bool IsExecutingOnServer()
        {
            string callingAssembly = Assembly.GetCallingAssembly().CodeBase;
            string executingAssembly = Assembly.GetExecutingAssembly().CodeBase;

            foreach( string assemblyName in INVALID_CALLING_ASSEMBLIES )
            {
                if( StringContains( assemblyName.ToUpper(), callingAssembly.ToUpper() ) ||
                    StringContains( assemblyName.ToUpper(), executingAssembly.ToUpper() ) 
                    )
                {
                    return true;
                }
            }
            return false;
        }


        private static bool StringContains( string aString, string stringToEvaluate )
        {
            if( stringToEvaluate.IndexOf( aString ) > 0 )
            {
                return true;
            }
            return false;
        }
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        private User()
        {
        }
        #endregion

        #region Data Elements
        private static volatile User    c_instance = null;
        private static readonly object           c_syncRoot = new Object();

        private Facility                i_Facility;
        private string                  i_UserID;
        private Extensions.SecurityService.Domain.User  i_SecurityUser;
        private string                  i_PBAREmployeeID;
        private string                  i_PBARSecurityCode;
        private string                  i_WorkstationID;
        #endregion

        #region Constants
        private const string 
            INVALID_IN_SHARED_ENV_MSG   = "User.GetCurrent() was executed in an invalid context.  It is only 'legal' to use User as a singleton in a non-shared enviornment such as on a user's desktop.  It is invalid to use User as a singleton when executing an a shared enviornment such as inside a Broker executing on the Application Server.";

        private static readonly string[]          
            INVALID_CALLING_ASSEMBLIES = new[] { "PATIENTACCESS.APPSERVER.DLL"};
        #endregion
    }
}
