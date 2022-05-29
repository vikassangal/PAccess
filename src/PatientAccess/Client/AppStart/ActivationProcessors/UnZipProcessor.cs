using System;
using System.IO;
using System.Xml;
using Microsoft.ApplicationBlocks.Updater;
using Microsoft.ApplicationBlocks.Updater.Configuration;

namespace PatientAccess.AppStart.ActivationProcessors
{
	/// <summary>
	/// Summary description for UnZipProcessor.
	/// </summary>
    public class UnZipProcessor : IActivationProcessor 
    {

        #region Event Handlers
        #endregion

        #region Methods
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public UnZipProcessor()
        {
        }

        #endregion

        #region Data Elements

        /// <summary>
        /// The UpdaterTask provided in the Init method.
        /// </summary>
        private UpdaterTask taskToProcess;

        /// <summary>
        /// The file specification that must be uncompressed.
        /// </summary>
        string fileSpec;

        #endregion

        #region Constants
        #endregion

        #region IActivationProcessor Members

        public void Execute()
        {
            // Combine the path to make it absolute.
            if ( !Path.IsPathRooted( fileSpec ) )
            {
                fileSpec = Path.Combine( taskToProcess.Manifest.Application.Location, fileSpec );
            }

  
        } 

        public void Init(ActivationProcessorProviderData data, UpdaterTask task)
        {
            this.taskToProcess = task;

            foreach( XmlAttribute attr in data.AnyAttributes )
            {
                if ( attr.Name == "source" )
                {
                    fileSpec = attr.Value;
                }
            }

            if ( fileSpec == null )
            {
                throw new ArgumentException( Resource.ResourceManager[ Resource.MessageKey.PathExpected ] ); 
            } 
        }

        public void OnError()
        {

        }

        public void PrepareExecution()
        {
        }

        #endregion
    }
}
