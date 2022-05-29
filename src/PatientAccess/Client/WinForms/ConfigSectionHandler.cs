using System;
using System.Configuration;
using System.Xml;
using Extensions.Exceptions;
using log4net;

namespace PatientAccess
{
    /// <summary>
    /// Implements IConfigurationSectionHandler.  Creates an PatientAccessConfig object which encapsulates necessary configuration settings
    /// in a strong-typed reliable class for consumption by the PatientAccess core process.
    /// </summary>
    internal class ConfigSectionHandler : IConfigurationSectionHandler
    {
        #region IConfigurationSectionHandler members
        /// <summary>
        /// This class is responsible for reading the configuration and creating an instance of the configuration class.
        /// <ClientApplicationInfo>
        ///     <appName>Patient Access</appName>
        ///     <appFolderName>C:\Program Files\Patient Access</appFolderName>
        ///     <appExeName>PatientAccess.exe</appExeName>
        ///     <appID>{7DE1B29E-7BD3-4772-B3A1-8A6CCC43DE70}</appID>
        /// </ClientApplicationInfo>
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="configContext"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        object IConfigurationSectionHandler.Create( object parent, object configContext, XmlNode section )
        {
            PatientAccessConfiguration config = null;
            XmlNode rootNode = null;
            XmlNode subNode = null;
            string temp = string.Empty;

            //  grab an instance of PatientAccessConfiguration
            config = new PatientAccessConfiguration();

            //  get the primary node "ClientApplicationInfo" so we can access others easily
            rootNode = section.SelectSingleNode( "ClientApplicationInfo" );

            //  access each subnode, validating values and adding them to our instance of config object			
            try
            {
                // get the Application Name: appName
                subNode = rootNode.SelectSingleNode( "appName" );
                if( null != subNode )
                {
                    config.AppName = subNode.InnerText;
                }

                // get the Application Folder Name: appFolderName
                subNode = rootNode.SelectSingleNode( "appFolderName" );
                if( null != subNode )
                {
                    temp = subNode.InnerText;
                    //  check if terminal slash, add if missing
                    if( !temp.EndsWith( @"\" ) )
                    {
                        temp += @"\";
                    }
                    config.AppFolderName = temp;
                }

                // get the Application Executable File Name: appExeName
                subNode = rootNode.SelectSingleNode( "appExeName" );
                if( null != subNode )
                {
                    config.ExecutableName = subNode.InnerText;
                }

                // get the Application GUID: appID
                subNode = rootNode.SelectSingleNode( "appID" );
                if( null != subNode )
                {
                    config.ApplicationID = subNode.InnerText;
                }
            }

            catch( Exception e )
            {
                //Trace.WriteLine( "PatientAccess:[ConfigSectionHandler.Create]: Error during parsing of app.config file:" + Environment.NewLine + e.Message );
                c_log.Error( "PatientAccess:[ConfigSectionHandler.Create]: Error during parsing of app.config file:" + Environment.NewLine + e.Message, e );
                //throw e;
                throw new EnterpriseException( e.Message, Severity.Catastrophic );
            }

            return config;

        }
        #endregion

        #region Construction and Finalization
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ConfigSectionHandler()
        {
        }
        #endregion

        #region Data Elements
        private static readonly ILog c_log = LogManager.GetLogger( typeof( ConfigSectionHandler ) );
        #endregion
    }
}
