using System;
using Extensions.PersistenceCommon;

namespace PatientAccess.Domain
{
	/// <summary>
	/// Summary description for RuleToActionMapping.
	/// </summary>
	//TODO: Create XML summary comment for RuleToActionMapping
    [Serializable]
    public class RuleToActionMapping : PersistentModel
    {
        #region Event Handlers
        #endregion

        #region Methods
        #endregion

        #region Properties

        public bool IsCompositeAction
        {
            get
            {
                return i_IsCompositeAction;
            }
            set
            {
                i_IsCompositeAction = value;
            }
        }

        public long ActionID
        {
            get
            {
                return i_ActionID;
            }
            set
            {
                i_ActionID = value;
            }
        }
        public long RuleID
        {
            get
            {
                return i_RuleID;
            }
            set
            {
                i_RuleID = value;
            }
        }
        public string ActionType
        {
            get
            {
                return i_ActionType;
            }
            set
            {
                i_ActionType = value;
            }
        }
        public string ActionName
        {
            get
            {
                return i_ActionName;
            }
            set
            {
                i_ActionName = value;
            }
        }
        public long CompositeActionID
        {
            get
            {
                return i_CompositeActionID;
            }
            set
            {
                i_CompositeActionID = value;
            }
        }
        #endregion

        #region Private Methods

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public RuleToActionMapping()
        {
        }

        public RuleToActionMapping(long ruleID, long actionID, string actionType, string actionName, 
            long compositeActionID, bool isCompositeAction)
        {
            i_IsCompositeAction = isCompositeAction;
            i_RuleID = ruleID;
            i_ActionID = actionID;
            i_ActionType = actionType;
            i_ActionName  = actionName;
            i_CompositeActionID = compositeActionID;
        }
        #endregion

        #region Data Elements
        private bool i_IsCompositeAction;
        private long i_RuleID;
        private long i_ActionID;
        private string i_ActionType;
        private string i_ActionName;
        private long i_CompositeActionID;

        #endregion

        #region Constants
        #endregion
    }
}
