using System;
using System.Collections;
using System.IO;

namespace Extensions.Exceptions
{
    //TODO: Create XML summary comment for ExceptionStream
    [Serializable]
    public class ExceptionWriter : StringWriter, IDisposable
    {
        #region Constants
        private const string
            DEFAULT_MSG = "Wrapped Exception - ";
        #endregion

        #region Event Handlers
        #endregion

        #region Class Methods
        public static ExceptionWriter DisableLogging()
        {
            ExceptionWriter currentErrorStream = null;
            if( System.Console.Error is ExceptionWriter )
            {
                currentErrorStream = (ExceptionWriter)System.Console.Error;
                System.Console.SetError( currentErrorStream.Console );
            }
            return currentErrorStream;
        }

        public static ExceptionWriter EnableLogging()
        {
            ExceptionWriter ew = null;
            if( !( System.Console.Error is ExceptionWriter ) )
            {
                ew = new ExceptionWriter();
                ew.Console = System.Console.Error;
                System.Console.SetError( ew );
            }
            return ew;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Add a new handler with a default severity of low.
        /// </summary>
        /// <param name="handler"></param>
        public void AddHandler( IExceptionHandler handler )
        {
            this.AddHandler( handler, Severity.Low );
        }

        private void AddHandler( IExceptionHandler handler, 
                                Severity severity )
        {
            ArrayList handlers = this.HandlersFor( severity );
            handlers.Add( handler );
        }

        private ArrayList HandlersFor( Severity severity )
        {
            if( !this.primHandlers.ContainsKey( severity ) )
            {
                this.primHandlers[severity] = new ArrayList();
            }
            return (ArrayList)this.primHandlers[severity];
        }

        virtual public void WriteLine( EnterpriseException exception )
        {
            this.HandleException( exception );
        }

        virtual public void WriteLine( EnterpriseException exception,
                                       Severity severity )
        {
            exception.Severity = severity;
            this.WriteLine( exception );
        }

        virtual public void WriteLine( Exception exception )
        {
            EnterpriseException ee = new EnterpriseException( exception.Message,
                                                              exception );
            ee.Severity = Severity.Low;
            this.WriteLine( ee );
        }

        virtual public void WriteLine( Exception exception,
                                       Severity severity )
        {
            EnterpriseException ee = new EnterpriseException( exception.Message,
                                                              exception );
            ee.Severity = severity;
            this.WriteLine( ee );
        }

        override public void WriteLine( object anObject )
        {
            try
            {
                EnterpriseException exception = null;
                if( anObject is EnterpriseException )
                {
                    exception = (EnterpriseException)anObject;
                    this.WriteLine( exception );
                }
                else if( anObject is Exception )
                {
                    Exception innerException = (Exception)anObject;
                    string msg = DEFAULT_MSG + innerException.Message;
                    exception = new EnterpriseException( msg, 
                        innerException,
                        Severity.Low );
                    this.WriteLine( exception );
                }
                else
                {
                    this.Console.WriteLine( anObject );
                }
            }
            catch
            {
                //TODO:  Figure out what to do since we can't let
                //exceptions escape from here...
            }
        }
        #endregion

        #region Properties
        #endregion

        #region Private Class Methods
        
        #endregion

        #region Private Methods
        private void HandleException( EnterpriseException exception )
        {
            ArrayList handlers = this.HandlersFor( exception.Severity );
            foreach( IExceptionHandler handler in handlers )
            {
                handler.HandleException( exception );
            }
        }
        #endregion

        #region Private Properties

        private TextWriter Console
        {
            get
            {
                return i_Console;
            }
            set
            {
                i_Console = value;
            }
        }

        private Hashtable primHandlers
        {
            get
            {
                return i_Handlers;
            }
            set
            {
                i_Handlers = value;
            }
        }
        #endregion

        #region Construction and Finalization
        protected ExceptionWriter()
        {
            this.primHandlers = new Hashtable();
        }

        public new void Dispose() 
        {
            this.Dispose( true );
            GC.SuppressFinalize( this ); 
        }

        protected override void Dispose( bool disposing ) 
        {
            if( disposing ) 
            {
                System.Console.SetError( this.Console );
            }
        }

        ~ExceptionWriter()
        {
            this.Dispose( false );
        }
        #endregion

        #region Data Elements
        private TextWriter      i_Console;
        private Hashtable       i_Handlers;
        #endregion
    }
}
