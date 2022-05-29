using System;
using System.Configuration;
using System.Diagnostics;
using System.Xml;

namespace PatientAccess.AppStart
{
    /// <summary>
    /// Implements IConfigurationSectionHandler.  Creates an AppStartConfig object which encapsulates necessary configuration settings
    /// in a strong-typed reliable class for consumption by AppStart core process.
    /// </summary>
    internal class ConfigSectionHandler : IConfigurationSectionHandler
    {
        #region Construction and Finalization
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ConfigSectionHandler()
        {
        }
        #endregion

        #region IConfigurationSectionHandler members
        /// <summary>
        /// This class is responsible for reading the configuration and creating an instance of the configuration class.
        /// <ClientApplicationInfo>
        ///     <appFolderName>.\PatientAccess\</appFolderName>
        ///     <appExeName>PatientAccess.exe</appExeName>
        ///     <appID>{7DE1B29E-7BD3-4772-B3A1-8A6CCC43DE70}</appID>
        ///     <updateTime>BeforeStart</updateTime>
        ///     <manifestUri>http://pa-dev.hdc.net/PatientAccess.AppUpdater/Manifests/PatientAccessClientManifest.xml</manifestUri>
        ///     <maxWaitTime>0</maxWaitTime>
        ///     <appZipArchive>PatientAccessClientWin32.zip</appZipArchive>
        /// </ClientApplicationInfo>
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="configContext"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        object IConfigurationSectionHandler.Create( object parent, object configContext, XmlNode section )
        {
            AppStartConfiguration config = null;
            XmlNode rootNode = null;
            XmlNode subNode = null;
            string temp = string.Empty;

            //  grab an instance of AppStartConfiguration
            config = new AppStartConfiguration();

            //  get the primary node "ClientApplicationInfo" so we can access others easily
            rootNode = section.SelectSingleNode("ClientApplicationInfo");

            //  access each subnode, validating values and adding them to our instance of config object			
            try
            {
                // get the Application Folder Name: appFolderName
                subNode = rootNode.SelectSingleNode("appFolderName");
                if ( null != subNode )
                {
                    temp = subNode.InnerText;
                    //  check if terminal slash, add if missing
                    if ( !temp.EndsWith( @"\" ) )
                    {
                        temp += @"\";
                    }
                    config.FolderName = temp;
                }
			          
                // get the Application Executable Name: appExeName
                subNode = rootNode.SelectSingleNode("appExeName");
                if ( null != subNode )
                {
                    config.ExecutableName = subNode.InnerText;
                }

                // get the Application GUID: appID
                subNode = rootNode.SelectSingleNode( "appID" );
                if ( null != subNode )
                {
                    temp = subNode.InnerText;
                    config.ApplicationID = subNode.InnerText;
                }

                // get the Application GUID: appID
                subNode = rootNode.SelectSingleNode( "updateTime" );
                if ( subNode != null )
                {
                    config.UpdateTime = (UpdateTimeEnum)Enum.Parse( typeof( UpdateTimeEnum ), subNode.InnerText );
                }

                // get the Application Manifest URI: manifestUri
                subNode = rootNode.SelectSingleNode( "manifestUri" );
                if ( subNode != null )
                {
                    config.ManifestUri = new Uri( subNode.InnerText );
                }

                // get the Application Maximum Wait Time: maxWaitTime
                // Adds support for download timeout configuration on the client when downloading synchronously.
                subNode = rootNode.SelectSingleNode( "maxWaitTime" );
                if ( subNode != null )
                {
                    // maxWaitTime stored in minutes. A 0 indicates no timeout period.
                    Double minutes = Double.Parse( subNode.InnerText );
                    if( minutes == 0 )
                    {
                        config.MaxWaitTime = TimeSpan.MaxValue;
                    } 
                    else
                    {
                        config.MaxWaitTime = TimeSpan.FromMinutes( minutes );
                    }
                }

                // get the Application GUID: appID
                // Adds support for storing the application zip archive file name.
                subNode = rootNode.SelectSingleNode( "appZipArchive" );
                if ( subNode != null )
                {
                    config.ZipArchive = subNode.InnerText;
                }            
            }
            catch( Exception e )
            {
                Trace.WriteLine( "AppStart:[ConfigSectionHandler.Create]: Error during parsing of app.config file:" + Environment.NewLine + e.Message );
                throw e;
            }

            return config;

        }
        #endregion
    }
}