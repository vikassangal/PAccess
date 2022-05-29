using System;

namespace PatientAccess.Domain
{
    /// <summary>
    /// Implemented for SR 41094 for the January 2008 release
    /// 
    /// VIWebPreviousDocument - this class represents a single instance of a previously scanned document, as
    /// returned by the web service call to retrieve previously scanned documents.  The service currently returns
    /// the following document types:
    /// 
    /// DL      - Driver's License
    /// ADV     - Advanced Directive
    /// INSCARD - Insurance Card(s)
    /// NPP     - Notice of Privacy Policy
    /// 
    /// </summary>
    
    [Serializable]
    public class VIWebPreviousDocument
    {
        #region Events
        #endregion

        #region Event Handlers
        #endregion

        #region Methods
        #endregion

        #region Properties

        public long AccountNumber
        {
            get
            {
                return i_AccountNumber;
            }
            set
            {
                i_AccountNumber = value;
            }
        }

        public VIWebPreviousDocumentType DocType
        {
            get
            {
                return i_DocType;
            }
            set
            {
                i_DocType = value;
            }
        }

        public DateTime DocumentDate
        {
            get
            {
                return i_DocumentDate;
            }
            set
            {
                try
                {
                    i_DocumentDate = value;
                }
                catch
                {
                    i_DocumentDate = DateTime.MinValue;
                }                
            }
        }

        public string DocumentID
        {
            get
            {
                return i_DocumentID;
            }
            set
            {
                i_DocumentID = value;
            }
        }

        public byte[] DocumentObject
        {
            get
            {
                return i_DocumentObject;
            }
            set
            {
                i_DocumentObject = value;
            }
        }

        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public VIWebPreviousDocument()
        {
        }

        public VIWebPreviousDocument( long accountNumber, VIWebPreviousDocumentType docType, DateTime docDate, string docID, byte[] docObj )
        {
            i_AccountNumber     = accountNumber;
            i_DocType           = docType;
            i_DocumentDate      = docDate;
            i_DocumentID        = docID;
            i_DocumentObject    = docObj;
        }

        #endregion

        #region Component Designer generated code
        #endregion

        #region Data Elements

        private long                                            i_AccountNumber = 0;
        private VIWebPreviousDocumentType                       i_DocType = VIWebPreviousDocumentType.UNKNOWN;
        private DateTime                                        i_DocumentDate = DateTime.MinValue;
        private string                                          i_DocumentID = string.Empty;
        private byte[]                                          i_DocumentObject;

        #endregion

        #region Constants & Enumerations

        // valid document types returned by VI web service and Unkown

        public enum VIWebPreviousDocumentType
        {
            ADV,
            DL,
            INSCARD,
            NPP,
            UNKNOWN
        }

        #endregion
    }
}
