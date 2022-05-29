using System;

namespace PatientAccess.Domain
{
    /// <summary>
    /// Summary description for ContributingCondition.
    /// </summary>
    [Serializable]
    public class ContributingCondition
    {
        #region Construction
        public ContributingCondition( string section, string question, string answer )
        {
            Section  = section;
            Question = question;
            Answer   = answer;
        }
        #endregion

        #region Methods
        #endregion

        #region Properties
        public string Section
        {
            get
            {
                return i_Section;
            }
            private set
            {
                i_Section = value;
            }
        }

        public string Question
        {
            get
            {
                return i_Question;
            }
            set
            {
                i_Question = value;
            }
        }

        public string Answer
        {
            get
            {
                return i_Answer;
            }
            private set
            {
                i_Answer = value;
            }
        }

        #endregion

        #region Data Elements
        private string i_Section;
        private string i_Question;
        private string i_Answer;
        #endregion
    }
}
