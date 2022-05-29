using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for PreRegRule.
    /// </summary>
    //TODO: Create XML summary comment for PreRegRule
    [Serializable]
    [UsedImplicitly]
    public class PostRegRule : LeafRule
    {
        #region Event Handlers
        #endregion

        #region Methods
        public override bool RegisterHandler(EventHandler eventHandler)
        {
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            
        }

        public override bool ShouldStopProcessing()
        {
            return true;
        }

        public override void ApplyTo( object context )
        {
        }
        public override bool CanBeAppliedTo( object context )
        {
            if( context == null || context.GetType() != typeof( Account ) )
            {
                return true;
            }  
            Account anAccount = context as Account;
            if( anAccount.KindOfVisit != null && VisitTypeIsPostReg(anAccount.KindOfVisit) )
//                && anAccount.KindOfVisit.Code == "2")
            {
                return true;
            }
            else
            {
                return false;
            }
        
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        private bool VisitTypeIsPostReg(VisitType visit)
        {
            switch( visit.Code ) 
            {
                case "1":                    
                    return true ;
                case "2":                    
                   return true ;
                case "3":
                    return true ;
                case "4":
                    return true ;
            }
            return false ;
        }
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public PostRegRule()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
