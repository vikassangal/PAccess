using System;
using System.Collections;

namespace PatientAccess.Domain
{
    /// <summary>
    /// Summary description for MSPRecommendation.
    /// </summary>
    [Serializable]
    public class MSPRecommendation
    {
        #region Construction
        public MSPRecommendation()
        {
        }
        #endregion

        #region Methods
        public void AddCondition( ContributingCondition c )
        {
            ContributingConditions.Add( c );
        }

        public void Reset()
        {
            ContributingConditions.Clear();
            IsInSpecialProgramOrLiability = false;
            IsMedicareRecommended         = false;
            FirstSetOfQuestionsFailed     = false;
            SecondSetOfQuestionsFailed    = false;
        }
        #endregion

        #region Properties

        public ArrayList ContributingConditions
        {
            get
            {
                return i_contributingConditions;
            }
        }

        public bool FirstSetOfQuestionsFailed
        {
            get
            {
                return i_firstSetFailed;
            }
            set
            {
                i_firstSetFailed = value;
            }
        }

        public bool SecondSetOfQuestionsFailed
        {
            get
            {
                return i_secondSetFailed;
            }
            set
            {
                i_secondSetFailed = value;
            }
        }

        public bool IsInSpecialProgramOrLiability
        {
            get
            {
                return i_IsInSpecialProgramOrLiability;
            }
            set
            {
                i_IsInSpecialProgramOrLiability = value;
            }
        }

        public bool IsMedicareRecommended
        {
            get
            {
                return i_IsMedicareRecommended;
            }
            set
            {
                i_IsMedicareRecommended = value;
            }
        }

        public int MSPVersion
        {
            get
            {
                return i_MSPVersion;
            }
            set
            {
                i_MSPVersion = value;
            }
        }

        #endregion

        #region Data Elements

        private bool                                        i_IsInSpecialProgramOrLiability;
        private bool                                        i_IsMedicareRecommended;
        private bool                                        i_firstSetFailed;
        private bool                                        i_secondSetFailed;

        private ArrayList                                   i_contributingConditions = new ArrayList();

        private int                                         i_MSPVersion;

        #endregion
    }
}
