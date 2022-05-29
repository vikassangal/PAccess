using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    [Serializable]
    [UsedImplicitly]
    public class ServiceCategoryRequired : LeafRule
    {
        [NonSerialized]
        private IDOFRFeatureManager _DOFRFeatureManager;

        private IDOFRFeatureManager DOFRFeatureManager
        {
            get { return _DOFRFeatureManager; }
            set { _DOFRFeatureManager = value; }
        }
        #region Event Handlers
        public event EventHandler ServiceCategoryRequiredEvent;
        #endregion
        public override bool RegisterHandler(EventHandler eventHandler)
        {
            ServiceCategoryRequiredEvent += eventHandler;
            return true;
        }
        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            ServiceCategoryRequiredEvent -= eventHandler;
            return true;
        }
        public override void UnregisterHandlers()
        {
            ServiceCategoryRequiredEvent = null;
        }

        public override void ApplyTo(object context)
        {

        }

        public override bool ShouldStopProcessing()
        {
            return false;
        }

        public override bool CanBeAppliedTo(object context)
        {
            if (context == null ||
                context.GetType() != typeof(Account))
            {
                return true;
            }

            DOFRFeatureManager = new DOFRFeatureManager();
            var anAccount = context as Account;


            if (anAccount == null)
            {
                return true;
            }
            if (anAccount.Facility == null)
            {
                return true;
            }
            if (anAccount.KindOfVisit != null
                && anAccount.KindOfVisit.Code == VisitType.INPATIENT)
            {
                return true;
            }
            if (DOFRFeatureManager.IsDOFREnabledForFacility(anAccount))
            {
                if(!DOFRFeatureManager.IsDOFRServiceCategoryValid(anAccount))
                {
                    return true;
                }
              
                if (DOFRFeatureManager.IsDOFRServiceCategoryValid(anAccount) 
                    && (anAccount.ServiceCategory == null || string.IsNullOrEmpty(anAccount.ServiceCategory.Code)))
                {
                    if (this.FireEvents && ServiceCategoryRequiredEvent != null)
                    {
                        ServiceCategoryRequiredEvent(this, null);
                    }
                    return false;
                }
            }
            
            return true;
        }
    }
   
}
