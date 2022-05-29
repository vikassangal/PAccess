using System;
using System.Configuration;
using System.Xml;
using Extensions.Configuration;
using Peradigm.Framework.Domain.Parties;

namespace Extensions.OrganizationalService
{
    [Serializable]
	public class CFDBHierarchyReader
	{
        #region Constants
        private const long
            ERROR_CONFIG_SETTING        = 207;

        private const string 
            DEFAULT_TOP_LEVEL_NAME      = "All Tenet",
            DEFAULT_TOP_LEVEL_CODE      = "ALL",
            DEFAULT_TOP_LEVEL_TYPE      = "ALL",
            
            CONFIG_FACILITY_TYPE        = "CFDB.FacilityType",
            CONFIG_OVERWRITE_TOP_LEVEL  = "CFDB.OverwriteTopLevel",
            CONFIG_TOP_LEVEL_CODE       = "CFDB.TopLevelCode",
            CONFIG_TOP_LEVEL_NAME       = "CFDB.TopLevelName",
            CONFIG_TOP_LEVEL_TYPE       = "CFDB.TopLevelType",
            
            ORGANIZATIONAL_UNIT_NODE    = "Level",
            FACILITY_NODE               = "Facility",
            LEVEL_TYPE                  = "GroupName",
            LEVEL_CODE                  = "LevelCD",
            LEVEL_NAME                  = "Name",

            FACILITY_TYPE               = "FacilityTypeCD",
            FACILITY_CODE               = "HSPCD",
            FACILITY_NAME               = "FullName";

        private const char    
            SEPARATOR                   = ',';
        #endregion

        #region Event Handlers
        #endregion

        #region Methods
        public OrganizationalUnit ReadFrom( XmlNode root )
        {
            OrganizationalUnit hierarchyRoot = null;
            XmlNodeList nodes = root.ChildNodes;

            if( root.ChildNodes.Count > 1 )
            {
                // Allways create top level if one doesn't exist
                hierarchyRoot = new OrganizationalUnit( this.TopLevelCode, this.TopLevelName, this.TopLevelType );
            }
            else
            {
                if( this.OverwriteTopLevel )
                {
                    hierarchyRoot = this.OrganizationalUnitFrom( root.ChildNodes[0] );
                    hierarchyRoot.Name = this.TopLevelName;
                    hierarchyRoot.Code = this.TopLevelCode;
                    hierarchyRoot.Type = new OrganizationalUnitType( this.TopLevelType );
                    nodes = root.ChildNodes[0].ChildNodes;
                }
                else
                {
                    hierarchyRoot = new OrganizationalUnit( this.TopLevelCode, this.TopLevelName, this.TopLevelType );
                }
            }
            this.CreateChildRelationshipsFor( hierarchyRoot, nodes );
            this.RemoveEmptyChildrenOf( hierarchyRoot );

            return hierarchyRoot;
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        private AbstractOrganizationalUnit HierarchyFrom( XmlNode node )
        {
            AbstractOrganizationalUnit level = null;
            if( this.IsOrganizationalUnit( node ) )
            {
                level = this.OrganizationalUnitFrom( node );            
                this.CreateChildRelationshipsFor( level, node.ChildNodes );
            }
            if( this.IsFacility( node ) )
            {
                level = this.FacilityFrom( node );
            }
            return level;
        }

        private void CreateChildRelationshipsFor( AbstractOrganizationalUnit parent, XmlNodeList childNodes )
        {
            AbstractOrganizationalUnit level;
            foreach( XmlNode node in childNodes )
            {
                level = this.HierarchyFrom( node );
                if( level != null )
                {
                    parent.AddRelationship( new OrganizationalRelationship( parent, level ) );
                    level.AddRelationship( new OrganizationalRelationship( parent, level ) );
                }
            }
        }
        private OrganizationalUnit OrganizationalUnitFrom( XmlNode hierarchyNode )
        {
            string levelType = hierarchyNode.Attributes[LEVEL_TYPE].Value.Trim();
            string levelCode = hierarchyNode.Attributes[LEVEL_CODE].Value.Trim();
            string levelName = hierarchyNode.Attributes[LEVEL_NAME].Value.Trim();

            return new OrganizationalUnit( levelCode, levelName, levelType );
        }

        private Facility FacilityFrom( XmlNode hierarchyNode )
        {
            string facilityCode = hierarchyNode.Attributes[FACILITY_CODE].Value.Trim();
            string facilityName = hierarchyNode.Attributes[FACILITY_NAME].Value.Trim();
            string facilityType = hierarchyNode.Attributes[FACILITY_TYPE].Value.Trim();
            
            if( this.IsSelectedFacilityType( facilityType ) )
            {
                return new Facility( facilityCode, facilityName );
            }
            return null;
        }

        private void RemoveEmptyChildrenOf( OrganizationalUnit parent )
        {
            foreach( AbstractOrganizationalUnit child in parent.Children() )
            {
                if( !( child is Facility ) )
                {
                    this.RemoveEmptyChildrenOf( (OrganizationalUnit)child );
                    if( ((OrganizationalUnit)child).Children().Count == 0 )
                    {
                        parent.RemoveRelationshipWith( child );
                    }
                }
            }
        }

        private bool IsFacility( XmlNode node )
        {
            return node.Name == FACILITY_NODE;
        }

        private bool IsOrganizationalUnit( XmlNode node )
        {
            return node.Name == ORGANIZATIONAL_UNIT_NODE;
        }

        private bool IsSelectedFacilityType( string type )
        {
            foreach( string facilityType in this.FacilityTypes )
            {
                if( type == facilityType )
                {
                    return true;
                }
            }
            return false;
        }

        private string ConfigurationSettingWith( string key )
        {
            string setting = ConfigurationManager.AppSettings[key];
            if( setting == null || setting.Trim() == String.Empty )
            {
                throw new ConfigurationErrorsException( 
                    String.Format( MessageCatalog.Messages[ERROR_CONFIG_SETTING].Text, key ) );
            }
            return setting;
        }
        #endregion

        #region Private Properties
        private string[] FacilityTypes
        {
            get
            {
                if( i_FacilityTypes == null )
                {
                    string facilityTypes = this.ConfigurationSettingWith( CONFIG_FACILITY_TYPE );
                    i_FacilityTypes = facilityTypes.Split( SEPARATOR );
                    for( int i = 0; i < i_FacilityTypes.Length; i++ )
                    {
                        i_FacilityTypes[i] = i_FacilityTypes[i].Trim();
                    }
                }
                return i_FacilityTypes;
            }
        }

        private bool OverwriteTopLevel
        {
            get
            {
                string setting = ConfigurationManager.AppSettings[CONFIG_OVERWRITE_TOP_LEVEL];
                bool toOverwrite = true;
                if( setting != null )
                {
                    try
                    {
                        toOverwrite = Boolean.Parse( setting );
                    }
                    catch( Exception )
                    {
                    }
                }
                return toOverwrite;
            }
        }

        private string TopLevelName
        {
            get
            {
                string setting = ConfigurationManager.AppSettings[CONFIG_TOP_LEVEL_NAME];
                if( setting == null || setting == String.Empty )
                {
                    setting = DEFAULT_TOP_LEVEL_NAME;
                }
                return setting;
            }
        }

        private string TopLevelCode
        {
            get
            {
                string setting = ConfigurationManager.AppSettings[CONFIG_TOP_LEVEL_CODE];
                if( setting == null || setting == String.Empty )
                {
                    setting = DEFAULT_TOP_LEVEL_CODE;
                }
                return setting;
            }
        }

        private string TopLevelType
        {
            get
            {
                string setting = ConfigurationManager.AppSettings[CONFIG_TOP_LEVEL_TYPE];
                if( setting == null || setting == String.Empty )
                {
                    setting = DEFAULT_TOP_LEVEL_TYPE;
                }
                return setting;
            }
        }
        #endregion

        #region Construction and Finalization
        public CFDBHierarchyReader()
        {
        }
        #endregion

        #region Data Elements
        string[] i_FacilityTypes = null;
        #endregion
    }
}
