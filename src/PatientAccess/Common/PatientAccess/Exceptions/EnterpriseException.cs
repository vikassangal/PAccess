using System;
using System.Collections;
using System.Runtime.Serialization;

namespace Extensions.Exceptions
{
    [Serializable]
    public class EnterpriseException : ApplicationException
    {
        #region Constants
        private const string DEFAULT_MESSAGE = "No message provided.";
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
        /// Remove a value from the exception's context. 
        /// </summary>
        /// <param name="key">
        /// Name of the key to remove.
        /// </param>
        public void RemoveContextItem( string key )
        {
            this.primContext.Remove( key );
        }
        #endregion

        #region Properties
        /// <summary>
        /// Answer a collection that represents the context for the exception.
        /// </summary>
        public IDictionary Context
        {
            get
            {
                return (IDictionary)primContext.Clone();
            }
        }

        /// <summary>
        /// Indicates the importance of the exception that is thrown.
        /// </summary>
        public Severity Severity
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

		public EnterpriseException( string message, 
									Exception innerException, 
									Severity severity, 
									Hashtable context )
			: base( message, innerException )
		{
            this.Severity    = severity;
            this.Timestamp   = DateTime.Now;
            this.primContext = new Hashtable();
			
			try
			{
				this.Server = Environment.MachineName;
			}
			catch
			{
				this.Server = string.Empty;
			}
            
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
        private Severity  i_Severity;
        private DateTime  i_Timestamp;
        private Hashtable i_Context;
		private string i_Server;
        #endregion
    }
}
