//-----------------------------------------------------------------------------
// Copyright © 2003-2005 Perot Systems Coproration. All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Collections;
using System.Runtime.Serialization;
using System.Threading;
using Peradigm.Framework.Persistence;

namespace Peradigm.Framework.Domain.Exceptions
{
    [Serializable]
    public class EnterpriseException : ApplicationException, IPersistentModel
    {
        #region Constants
        private const string
            DEFAULT_MESSAGE = "No message provided.";
        #endregion

        #region Event Handlers
        #endregion

        #region Methods
        /// <summary>
        /// Add a value to the exception context.  May include runtime variables, parameters, or
        /// other values used by the method that creates and throws the exception.
        /// </summary>
        /// <param name="contextItem">DictionaryEntry to add</param>
        private void AddContextItem( DictionaryEntry contextItem )
        {
            if( contextItem.Key != null && contextItem.Value != null )
            {
                this.AddContextItem( contextItem.Key.ToString(), contextItem.Value.ToString() );
            }
        }
        /// <summary>
        /// Add a value to the exception context.  May include runtime variables, parameters, or
        /// other values used by the method that creates and throws the exception.
        /// </summary>
        /// <param name="key">
        /// Key used to access the context value.
        /// </param>
        /// <param name="value">
        /// Context value.
        /// </param>
        public void AddContextItem( string key, string value )
        {
            if( ( key != null ) && ( value != null ) )
            {
                this.primContext[key] = value;
            }
        }

        /// <summary>
        /// Answer with full name of the Assembly that threw an exception.
        /// Please notice that this method should be called after exception
        /// is thrown and caught
        /// </summary>
        /// <returns>Assembly Full Name ( FriendlyName, Version, Culture, Public Token )</returns>
        private string DetermineAssemblyName()
        {
            string assemblyName = String.Empty;
            try
            {
                assemblyName = this.TargetSite.ReflectedType.Assembly.ToString();
            }
            catch
            {
                assemblyName = String.Empty;
            }
            return assemblyName;
        }

        public bool HasBeenSaved()
        {
            return ( this.Oid != PersistentModel.NEW_OID ) &&
                   ( !this.HasVersion( PersistentModel.NEW_VERSION ) );
        }

        public bool HasNotBeenSaved()
        {
            return !this.HasBeenSaved();
        }

        public bool HasVersion( byte[] version )
        {
            if( ( null == this.Version ) ||
                ( null == version ) )
            {
                return false;
            }

            bool bytesMatch = true;
            for( long i = 0; i < this.Version.Length; i++ )
            {
                bytesMatch = this.Version[i] == version[i];
                if( !bytesMatch )
                {
                    break;
                }
            }
            return bytesMatch;
        }

        #endregion

        #region Properties
        /// <summary>
        /// Anser with name of AppDomain that raised the exeption
        /// </summary>
        private string AppDomainName
        {
            get
            {
                return i_AppDomainName;
            }
            set
            {
                i_AppDomainName = value;
            }
        }

        /// <summary>
        /// Anser with assembly name and version.
        /// Please notice that this property should be set AFTER exception is thrown caught, 
        /// because only then it's known which assembly threw it.
        /// Use results of DetermineAssemblyName() method to set this property.
        /// </summary>
        private string AssemblyName
        {
            get
            {
                return i_AssemblyName;
            }
            set
            {
                i_AssemblyName = value;
            }
        }
	

	
        /// <summary>
        /// Answer a collection that represents the context for the exception.
        /// </summary>
        private IDictionary Context
        {
            get
            {
                return (IDictionary)primContext.Clone();
            }
        }

        /// <summary>
        /// Property representing the record ID or key value from the underlying
        /// storage mechanism.  Typically, this property represents the primary key
        /// from a SQL table that has been defined as an integer auto-increment field.
        /// </summary>
        public long Oid
        {
            get
            {
                return i_Oid;
            }
            set
            {
                i_Oid = value;
            }
        }

        /// <summary>
        /// Indicates the importance of the exception that is thrown.
        /// </summary>
        private Severity Severity
        {
            get
            {
                return i_Severity;
            }
            set
            {
                Severity severity = value;
                EnterpriseException ee = this.InnerException as EnterpriseException;
                if( ee != null )
                {
                    severity = this.SeverityUsing( ee, value );
                }
                i_Severity = severity;
            }
        }

        /// <summary>
        /// Gets or sets the Source of the exception.  If a property is not provided, the
        /// source for the inner-most exception will be used.
        /// </summary>
        public override string Source
        {
            get
            {
                string source = base.Source;
                Exception inner = this;

                while( ( source == null ) && ( inner != null ) )
                {
                    inner = inner.InnerException;
                    if( inner != null )
                    {
                        source = inner.Source;
                    }
                }

                if (source == null)
                {
                    source = String.Empty;
                }
                return source;
            }
            set
            {
                base.Source = value;
            }
        }

        /// <summary>
        /// Gets the stack trace of the inner-most exception.
        /// </summary>
        public override string StackTrace
        {
            get
            {
                string stackTrace = base.StackTrace;
                Exception inner = this;

                while( ( stackTrace == null ) && ( inner != null ) )
                {
                    inner = inner.InnerException;
                    if( inner != null )
                    {
                        stackTrace = inner.StackTrace;
                    }
                }

                if (stackTrace == null)
                {
                    stackTrace = String.Empty;
                }

                return stackTrace;
            }
        }

		/// <summary>
		/// Answer with a name of the server where exception was raised.
		/// </summary>
		private string Server
		{
			get
			{
				return i_Server;
			}
			set
			{
				i_Server = value;
			}
		}

        /// <summary>
        /// Date and time the exception was created.
        /// </summary>
        private DateTime Timestamp
        {
            get
            {
                return i_Timestamp;
            }
            set
            {
                i_Timestamp = value;
            }
        }

        /// <summary>
        /// Answer with thread identity where exception was raised.
        /// </summary>
        private string ThreadIdentity
        {
            get
            {
                return i_ThreadIdentity;
            }
            set
            {
                i_ThreadIdentity = value;
            }
        }

        /// <summary>
        /// Property representing the timestamp of the last record update.  This value
        /// may be used to compare updates to determine if a refresh of client data
        /// may be necessary before saving data.
        /// </summary>
        public byte[] Version
        {
            get
            {
                return i_Version;
            }
            set
            {
                i_Version = value;
            }
        }

        /// <summary>
        /// Answer with Windows identity of the account running the process at the time when exception was raised.
        /// </summary>
        private string WindowsIdentity
        {
            get
            {
                return i_WindowsIdentity;
            }
            set
            {
                i_WindowsIdentity = value;
            }
        }
        #endregion

        #region Private Methods
        protected virtual void CopyContextFrom( Exception exception )
        {
            if( exception is EnterpriseException )
            {
                foreach( DictionaryEntry contextItem in ((EnterpriseException) exception).Context )
                {
                    this.AddContextItem( contextItem );
                }
            }
        }

        private string DetermineAppDomainName()
        {
            string appDomainName = String.Empty;
            try
            {
                appDomainName = AppDomain.CurrentDomain.FriendlyName;
            }
            catch
            {
                appDomainName = String.Empty;
            }
            return appDomainName;
        }

        private string DetermineServerName()
        {
            string serverName = String.Empty;
            try
            {
                serverName = Environment.MachineName;
            }
            catch
            {
                serverName = string.Empty;
            }
            return serverName;
        }

        private string DetermineThreadIdentity()
        {
            string threadIdentity = String.Empty;
            try
            {
                threadIdentity = Thread.CurrentPrincipal.Identity.Name;
            }
            catch
            {
                threadIdentity = String.Empty;
            }
            if( threadIdentity == null )
            {
                threadIdentity = String.Empty;
            }
            return threadIdentity;
        }

        private string DetermineWindowsIdentity()
        {
            string windowsIdentity = String.Empty;
            try
            {
                windowsIdentity = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            }
            catch
            {
                windowsIdentity = String.Empty;
            }
            if( windowsIdentity == null )
            {
                windowsIdentity = String.Empty;
            }

            return windowsIdentity;
        }

        private Severity SeverityUsing( EnterpriseException innerException, 
            Severity defaultSeverity )
        {
            if( defaultSeverity <= innerException.Severity )
            {
                return innerException.Severity;
            }
            return defaultSeverity;
        }
        #endregion

        #region Private Properties
        /// <summary>
        /// Contains the context of the exception.
        /// </summary>
        private Hashtable primContext
        {
            get
            {
                return i_Context;
            }
			set
			{
				i_Context = value;
			}
        }
        #endregion

        #region Construction and Finalization
        public EnterpriseException()
            : this( DEFAULT_MESSAGE )
        {
        }

        public EnterpriseException( string message )
            : this( message, Severity.Low )
        {
        }

        public EnterpriseException( string message, Severity severity )
            : this( message, null, severity )
        {
        }

		public EnterpriseException( string message, Severity severity, Hashtable context )
			: this( message, null, severity, context )
		{
		}

        public EnterpriseException( string message, Exception innerException )
            : this( message, innerException, Severity.Low )
        {
        }

        public EnterpriseException( string message, 
                                    Exception innerException,
                                    Severity severity )
            : this( message, innerException, severity, null )
        {
        }

		private EnterpriseException( string message, 
									Exception innerException, 
									Severity severity, 
									Hashtable context )
			: base( message, innerException )
		{
            this.Severity    = severity;
            this.Timestamp   = DateTime.Now;
            this.primContext = new Hashtable();

            this.Server = this.DetermineServerName();
            this.ThreadIdentity = this.DetermineThreadIdentity();
            this.WindowsIdentity = this.DetermineWindowsIdentity();
            this.AppDomainName = this.DetermineAppDomainName();
            this.AssemblyName = this.DetermineAssemblyName();

            if( context != null )
            {
                foreach( DictionaryEntry contextItem in context )
                {
                    this.AddContextItem( contextItem );
                }
            }

            if( null != innerException )
            {
                this.Source = innerException.Source;
                this.CopyContextFrom( innerException );
            }
		}

        public EnterpriseException( SerializationInfo info, StreamingContext context )
            : base( info, context )
        {
            this.primContext = new Hashtable();
        }
        #endregion

        #region Data Elements
        private string      i_AppDomainName;
        private string      i_AssemblyName;
        private Hashtable   i_Context;
        private long        i_Oid;
		private string      i_Server;
        private Severity    i_Severity;
        private string      i_ThreadIdentity;
        private DateTime    i_Timestamp;
        private byte[]      i_Version;
        private string      i_WindowsIdentity;
        #endregion
    }
}
