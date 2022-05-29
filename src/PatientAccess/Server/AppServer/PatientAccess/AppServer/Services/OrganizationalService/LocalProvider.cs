using System;
using System.Configuration;
using System.Xml;
using Extensions.Configuration;
using PatientAccess.Annotations;
using Peradigm.Framework.Domain.Parties;

namespace Extensions.OrganizationalService
{
    [UsedImplicitly]
    public class LocalProvider : IOrganizationalHierarchyProvider
    {
        #region Constants
        private const long
            ERROR_CONFIG_SETTING    = 207;
        private const string 
            CONFIG_HIERARCHY_PATH   = "OrganizationalService.LocalHierarchyLocation";
        #endregion

        #region Event Handlers
        #endregion

        #region Methods
        public OrganizationalUnit GetOrganizationalHierarchy()
        {
            XmlDocument document = new XmlDocument();
            document.Load( this.HierarchyPath );

            CFDBHierarchyReader reader = new CFDBHierarchyReader();
            OrganizationalUnit hierarchyRoot = reader.ReadFrom( document.DocumentElement );

            return hierarchyRoot;
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        private string ConfigurationSettingWith( string key )
        {
            string setting = ConfigurationManager.AppSettings[key];
            if( setting == null || setting.Trim() == String.Empty )
            {
                throw new ConfigurationErrorsException( 
                    String.Format( MessageCatalog.Messages[ERROR_CONFIG_SETTING].Text, key ) );
            }
            return setting.Trim();
        }        
        #endregion

        #region Private Properties
        private string HierarchyPath
        {
            get
            {
                return this.ConfigurationSettingWith( CONFIG_HIERARCHY_PATH );
            }
        }
        #endregion

        #region Construction and Finalization
        public LocalProvider()
        {
        }
        #endregion

        #region Data Elements
        #endregion
    }
}
