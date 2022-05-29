using System;
using System.Configuration;
using System.IO;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Domain
{
	/// <summary>
	/// Summary description for UserPreference.
	/// </summary>
    [Serializable]
    public class UserPreference 
    {
        #region Event Handlers
        #endregion

        #region Methods
        public static UserPreference Load()
        {
            UserPreference preferences = new UserPreference();
            XmlSerializer serializer = new XmlSerializer( typeof( UserPreference ));

            IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForDomain();

            String [] fileNames = store.GetFileNames("*");

            for( int i=0; i < fileNames.Length; ++i )
            {
                if( fileNames[i].Equals( c_UserPreferenceFile ) )
                {
                    IsolatedStorageFileStream stream = new IsolatedStorageFileStream( c_UserPreferenceFile, FileMode.Open, store );  
    
                    StreamReader streamReader = new StreamReader( stream );
                    preferences = (UserPreference) serializer.Deserialize( streamReader );

                    streamReader.Close();
                    stream.Close();
                    store.Dispose();
                    store.Close();
                }
            }
            return preferences;
        }

        public void Save( UserPreference preferences )
        {
            XmlSerializer serializer = new XmlSerializer( typeof( UserPreference ));
            
            IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForDomain();

            IsolatedStorageFileStream stream = new IsolatedStorageFileStream( c_UserPreferenceFile, FileMode.Create, store );

            StreamWriter streamWriter = new StreamWriter( stream );
            serializer.Serialize( streamWriter, preferences );

            streamWriter.Close();
            stream.Close();
            store.Dispose();
            store.Close();
        }
        #endregion

        #region Properties
        public string LastUsedFacilityCode
        {
            get
            {
                if( i_LastUsedFacilityCode != null )
                {
                    return i_LastUsedFacilityCode;
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                if( value != null )
                {
                    i_LastUsedFacilityCode = value;
                } 
                else
                {
                    i_LastUsedFacilityCode = string.Empty;
                }
            }
        }

        public string WorkstationID
        {
            get
            {
                if( i_WorkstationID != null )
                {
                    return i_WorkstationID;
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                if( value != null )
                {
                    i_WorkstationID = value;
                } 
                else
                {
                    i_WorkstationID = string.Empty;
                }
            }
        }

        public string DisableWorkstationID
        {
            get
            {
                return i_DisableWorkstationID;
            }
            set
            {
                i_DisableWorkstationID = value;
            }
        }

        public bool ShowMSPWelcomeScreen
        {
            get
            {
                return i_showMSPWelcomeScreen;
            }
            set
            {
                i_showMSPWelcomeScreen = value;
            }
        }

        public PhoneNumber PhoneNumber
        {
            get
            {
                return i_PhoneNumber;
            }
            set
            {
                i_PhoneNumber = value;
            }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        static UserPreference()
        {
            c_UserPreferenceFile = ConfigurationManager.AppSettings[ USER_SETTINGS_FILE_NAME ];
        }
        #endregion

        #region Data Elements
        private string                          i_LastUsedFacilityCode = string.Empty;
        private string                          i_WorkstationID        = string.Empty;
        private string                          i_DisableWorkstationID = string.Empty;
        private bool                            i_showMSPWelcomeScreen = true;
        private PhoneNumber                     i_PhoneNumber          = new PhoneNumber();
        private readonly static string          c_UserPreferenceFile;
        #endregion

        #region Constants
        private const string USER_SETTINGS_FILE_NAME = "UserSettingsFileName";
        #endregion
    }
}