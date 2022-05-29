using System;
using System.Runtime.Remoting.Messaging;
using PatientAccess.Utilities;

namespace PatientAccess.RemotingServices
{
    public class CallContextAccessor
    {
        private const string Key = "DA2EF43A-80A3-4A75-8BDF-9198CA44B03D";

        /// <exception cref="ArgumentNullException"><c>contextData</c> is null.</exception>
        public void SetContext( CallContextData contextData )
        {
            Guard.ThrowIfArgumentIsNull( contextData, "contextData" );

            CallContext.SetData( Key, contextData );
        }

        public CallContextData GetContext()
        {
            var data = CallContext.GetData( Key );

            CallContextData callContextData;

            if ( data != null )
            {
                callContextData = (CallContextData)data;
            }

            else
            {
                callContextData = new CallContextData( "User Id not set" );
            }

            return callContextData;
        }
    }
}
