using System;
using System.Collections.Generic;

namespace PatientAccess.Domain
{
    /// <summary>
    /// VIWebPreviousDocuments - extends the generic collection to allow for a collection of documents as returned
    /// by the VI web service for previously scanned documents.
    /// </summary>
    
    [Serializable]
    public class VIWebPreviousDocuments
    {
        #region Events
        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        // use an anonymous delegate to get docs with a specific docType

        public List<VIWebPreviousDocument> GetDocumentsWith( VIWebPreviousDocument.VIWebPreviousDocumentType docType )
        {
            return this.i_previousDocumentList.FindAll
                (
                    delegate( VIWebPreviousDocument doc )
                    {
                        return doc.DocType.Equals( docType );

                    }
                );
        }


        #endregion

        #region Properties

        public List<VIWebPreviousDocument> PreviousDocumentList
        {
            get
            {
                return this.i_previousDocumentList;
            }           
        }

        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public VIWebPreviousDocuments()
        {
        }

        #endregion

        #region Component Designer generated code
        #endregion

        #region Data Elements

        private List<VIWebPreviousDocument> i_previousDocumentList = new List<VIWebPreviousDocument>();

        #endregion

        #region Constants
        #endregion
    }
}
