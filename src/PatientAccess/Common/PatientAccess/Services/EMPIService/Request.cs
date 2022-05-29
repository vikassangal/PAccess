using System;

namespace PatientAccess.Services.EMPIService
{
    /// <summary>
    /// Summary description for getMemberRequest.
    /// </summary>
    [Serializable]
    public class Request
    {
        #region Properties

        public string srcCode;

        public string memIdnum;

        public string cvwName = CVWNAME;

        public string entType = ENTTYPE;

        public string getType = GETTYPE;

        public string memType = MEMTYPE;

        public string segCodeFilter = SEGCODEFILTER;

        public string audMode = AUDMODE;

        public string recStatFilter = RECSTATFILTER;

        public int maxRows = MAXROWS;

        public string searchSegCodeFilter = SEARCHSEGCODEFILTER;

        public string memStatFilter = MEMSTATFILTER;

        public short  minScore = MINSCORE;

        #endregion

        #region Constants

        private const string CVWNAME = "PACVW";
        private const string ENTTYPE = "patient";
        private const string GETTYPE = "ASENTITY";
        private const string SEGCODEFILTER = "MEMHEAD,MEMATTRALL";
        private const string MEMTYPE = "PATIENT";
        private const string AUDMODE = "AUDHEAD";
        private const string RECSTATFILTER = "A";
        private const string MEMSTATFILTER = "A";
        private const short MINSCORE = 1;
        private const int MAXROWS = 50;
        private const string SEARCHSEGCODEFILTER = "MEMHEAD,MEMATTRALL,MEMXTSK";

        #endregion
    }
}
