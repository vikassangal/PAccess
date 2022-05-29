using System;
using System.Configuration;
using System.Reflection;
using System.Web;
using System.Web.Caching;
using Extensions.Exceptions;
using Peradigm.Framework.Domain.Parties;

namespace Extensions.OrganizationalService
{
    public class OrganizationalService
    {
        #region Constants
        private const string
            CACHE_HIERARCHY = "eCareOrganizationalHierarchy",
            CACHE_EXCEPTION_WRITER = "eCare.ExceptionWriter",
            CONFIG_RENEW_FREQUENCY = "OrganizationalService.RenewFrequency", 
            ERROR_HIERARCHY_UNAVAILABLE = "Application failed to retrieve organizational hierarchy: {0}";
        private const int
            DEFAULT_RENEW_FREQUENCY = 1440; // In minutes

        private const string
            CONFIG_PROVIDER_ASSEMBLY = "OrganizationalService.Provider.Assembly",
            CONFIG_PROVIDER_CLASS = "OrganizationalService.Provider.Class",
            ERR_LOADING_PROVIDER = "OrganizationalHierarchyProvider Error - Could not load Provider: {0}";


        #endregion

        #region Event Handlers
        #endregion

        #region Methods
        public OrganizationalUnit GetOrganizationalHierarchy()
        {
            if( !IsStarted )
            {
                Startup();
            }
            OrganizationalUnit hierarchy = this.HierarchyFromCache();
            if( hierarchy == null )
            {
                lock( typeof( OrganizationalService ) )
                {
                    hierarchy = this.HierarchyFromCache();
                    if( hierarchy == null )
                    {
                        hierarchy = this.GetHierarchy();
                    }
                }
            }
            return hierarchy;
        }

        public static void Startup( IOrganizationalHierarchyProvider provider )
        {
            Service.Provider = provider;
            Startup();
        }

        private static void Startup()
        {
            if( !IsStarted )
            {
                lock( typeof( OrganizationalService ) )
                {
                    if( !IsStarted )
                    {
                        if( Service.Provider == null )
                        {
                            Service.ConstructProvider();
                        }
                        Service.GetHierarchy();
                        IsStarted = true;
                    }
                }
            }
        }

        public static void Shutdown()
        {
            if( IsStarted )
            {
                lock( typeof( OrganizationalService ) )
                {
                    if( IsStarted )
                    {
                        IsStarted = false;
                        lock( Cache )
                        {
                            Cache.Remove( CACHE_HIERARCHY );
                        }
                        Service.Provider = null;
                    }
                }
            }
        }
        #endregion

        #region Properties
        public static OrganizationalService Service
        {
            get
            {
                return c_Service;
            }
        }
        #endregion

        #region Private Methods
        private void ConstructProvider()
        { 
            string providerAssemblyName = ConfigurationManager.AppSettings[CONFIG_PROVIDER_ASSEMBLY];
            string providerClass = ConfigurationManager.AppSettings[CONFIG_PROVIDER_CLASS];
            try
            {               
                string assemblyName = providerAssemblyName.Substring(0, providerAssemblyName.IndexOf(".dll"));

                Assembly providerAssembly = Assembly.Load( assemblyName );
                object instance = providerAssembly.CreateInstance( providerClass );
                this.Provider = (IOrganizationalHierarchyProvider)instance;
            }
            catch( Exception e )
            {
                string msg = String.Format( ERR_LOADING_PROVIDER, e.Message );
                HierarchyUnavailableException ee = new HierarchyUnavailableException(
                    String.Format( ERR_LOADING_PROVIDER, e.Message ), e, Severity.High );
                this.LogException( ee );
                throw;
            }
        }

        private void OnHierarchyExpired( string key, object value, CacheItemRemovedReason reason )
        {
            if( IsStarted )
            {
                lock( typeof( OrganizationalService ) )
                {
                    try
                    {
                        this.GetHierarchy();
                    }
                    catch( HierarchyUnavailableException hue )
                    {
                        // Keep using old hierarchy
                        this.StoreHierarchyInCache( (OrganizationalUnit)value );
                        this.LogException( hue );
                    }
                    catch( Exception e )
                    {
                        // Keep using old hierarchy
                        this.StoreHierarchyInCache( (OrganizationalUnit)value );
                        HierarchyUnavailableException ee = new HierarchyUnavailableException(
                            String.Format( ERROR_HIERARCHY_UNAVAILABLE, e.Message ), e, Severity.High );
                        this.LogException( ee );
                    }
                }
            }
        }

        private OrganizationalUnit HierarchyFromCache()
        {
            return Cache[CACHE_HIERARCHY] as OrganizationalUnit;
        }

        private OrganizationalUnit GetHierarchy()
        {
            OrganizationalUnit hierarchy = null;

            // 1-3-2007 - kjs - since the local provider and logers are not fully implemented
            // in Patient Access we can not use these. Just allow the underlaying exception to 
            // be thrown
            hierarchy = this.HierarchyUsing( this.Provider ); 

            this.StoreHierarchyInCache( hierarchy );

            return hierarchy;
        }

        private OrganizationalUnit HierarchyUsing( IOrganizationalHierarchyProvider provider )
        {
            OrganizationalUnit hierarchy = null;
            try
            {
                hierarchy = provider.GetOrganizationalHierarchy();
            }
            catch( Exception e )
            {
                HierarchyUnavailableException error = new HierarchyUnavailableException(
                    String.Format( ERROR_HIERARCHY_UNAVAILABLE, e.Message ),
                    e, Severity.High );
                if( provider is LocalProvider )
                {
                    error.Severity = Severity.Catastrophic;
                }
                throw error;
            }
            return hierarchy;
        }
        
        private void StoreHierarchyInCache( OrganizationalUnit hierarchy )
        {
            lock( Cache )
            {
                Cache.Insert(
                    CACHE_HIERARCHY, hierarchy, null,
                    DateTime.Now.AddMinutes( RenewFrequency ),
                    TimeSpan.Zero, CacheItemPriority.Normal,
                    new CacheItemRemovedCallback( this.OnHierarchyExpired ) );
            }
        }

        private void LogException( EnterpriseException exception )
        {
            ExceptionWriter log = (ExceptionWriter)Cache[CACHE_EXCEPTION_WRITER];
            if( log != null )
            {
                try
                {
                    log.WriteLine( exception );
                }
                catch
                {
                }
            }
        }
        #endregion

        #region Private Properties
        private static bool IsStarted
        {
            get
            {
                return c_IsStarted;
            }
            set
            {
                c_IsStarted = value;
            }
        }

        private IOrganizationalHierarchyProvider Provider
        {
            get
            {
                return i_Provider;
            }
            set
            {
                i_Provider = value;
            }
        }

        private static Cache Cache
        {
            get
            {
                HttpRuntime rt = new HttpRuntime();
                return HttpRuntime.Cache;
            }
        }

        private int RenewFrequency
        {
            get
            {
                string renewFrequency = ConfigurationManager.AppSettings[CONFIG_RENEW_FREQUENCY];
                if( renewFrequency != null )
                {
                    return Int32.Parse( renewFrequency );
                }
                return DEFAULT_RENEW_FREQUENCY;
            }
        }
        
        #endregion

        #region Construction and Finalization
        static OrganizationalService()
        {
            c_IsStarted = false;
            c_Service = new OrganizationalService();
        }

        private OrganizationalService()
        {
        }
        #endregion

        #region Data Elements
        private IOrganizationalHierarchyProvider i_Provider;

        private static bool c_IsStarted;
        private static OrganizationalService c_Service;
        #endregion
    }
}
