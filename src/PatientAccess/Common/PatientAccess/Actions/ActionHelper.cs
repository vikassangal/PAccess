using System;
using System.Reflection;
using PatientAccess.Annotations;
using PatientAccess.BrokerInterfaces;
using log4net;

namespace PatientAccess.Actions
{
    /// <summary>
    /// Summary description for ActionHelper.
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class ActionHelper
    {
        public ActionHelper()
        {
        }

        private static ILog Logger
        {
            get { return logger; }
        }

        public static IAccountView LoadAccountView()
        {
            IAccountView anAccountView = null;

            try
            {
                Assembly a = Assembly.Load( "PatientAccess" );

                Type avType = a.GetType( "PatientAccess.UI.AccountViewProxy" );

                IAccountView accountViewProxy = Activator.CreateInstance( avType ) as IAccountView;

                anAccountView = accountViewProxy.GetIInstance();
            }
            catch
            {
                Logger.Debug( "do nothing... this will happen when there is no accountView (e.g. from ADTBroker)" );
            }

            return anAccountView;
        }

        private static ILog logger = LogManager.GetLogger( typeof( ActionHelper ) );
    }

}
