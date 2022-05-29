using System;

namespace PatientAccess.BrokerInterfaces
{
    [Serializable]
    public class VIWebPreviousDocumentCnts
    {
        public long ADVDocumentCnt 
        {
            get
            {
                return i_ADVDocumentCnt;
            }
            set
            {
                i_ADVDocumentCnt = value;
            }
        }
        public long DLDocumentCnt 
        {
            get
            {
                return i_DLDocumentCnt;
            }
            set
            {
                i_DLDocumentCnt = value;
            }
        }
        public long INSCardDocumentCnt 
        {
            get
            {
                return i_INSCardDocumentCnt;
            }
            set
            {
                i_INSCardDocumentCnt = value;
            }
        }
        public long NPPDocumentCnt 
        {
            get
            {
                return i_NPPDocumentCnt;
            }
            set
            {
                i_NPPDocumentCnt = value;
            }
        }

        public VIWebPreviousDocumentCnts()
        {
        }


        #region Members
        private long i_ADVDocumentCnt = 0;
        private long i_DLDocumentCnt = 0;
        private long i_INSCardDocumentCnt = 0;
        private long i_NPPDocumentCnt = 0;
        #endregion
    }
}
