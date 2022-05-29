using System.Collections.Generic;
using PatientAccess.Rules;

namespace PatientAccess.UI.CommonControls
{
    public interface IMessageDisplayStateManager
    {
        void SetErrorMessageDisplayedFor( string ruleType, bool isMessageShown );
        bool HasErrorMessageBeenDisplayedEarlierFor( string ruleType );
    }

    public class MessageDisplayStateManager : IMessageDisplayStateManager
    {
        #region Methods

        public void SetErrorMessageDisplayedFor( string ruleType, bool isMessageShown )
        {
            if( messageDisplayStateMap.ContainsKey( ruleType ))
            {
                messageDisplayStateMap[ ruleType ] = isMessageShown;
            }
            else
            {
                messageDisplayStateMap.Add( ruleType, isMessageShown );
            }
        }

        public bool HasErrorMessageBeenDisplayedEarlierFor( string ruleType )
        {
            bool hasMessageBeenDisplayed;

            messageDisplayStateMap.TryGetValue( ruleType, out hasMessageBeenDisplayed );

            return hasMessageBeenDisplayed;
        }

        private void PopulateRuleMessageDisplayMap()
        {
            SetErrorMessageDisplayedFor( typeof( NoMedicarePrimaryPayorForAutoAccident ).ToString(), false );
        }

        #endregion


        #region Properties
        #endregion

        #region Construction and Finalization

        public MessageDisplayStateManager()
        {
            PopulateRuleMessageDisplayMap();
        }

        #endregion

        #region Data Elements

        private Dictionary<string, bool> messageDisplayStateMap = new Dictionary<string, bool>();

        #endregion

        #region Constants
        #endregion
    }
}
