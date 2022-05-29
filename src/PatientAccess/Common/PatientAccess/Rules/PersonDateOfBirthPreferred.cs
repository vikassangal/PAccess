using System;
using Extensions.UI.Builder;
using PatientAccess.Actions;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Annotations;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for PersonDateOfBirthPreferred.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class PersonDateOfBirthPreferred : LeafRule
    {
        #region Event Handlers
        public event EventHandler PersonDateOfBirthPreferredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            PersonDateOfBirthPreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            PersonDateOfBirthPreferredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.PersonDateOfBirthPreferredEvent = null;   
        }

        public override bool ShouldStopProcessing()
        {
            return true;
        }

        public override void ApplyTo( object context )
        {
        }

        /// <summary>
        /// CanBeAppliedTo - this rule is a member of a composite (to PersonInfo).
        /// Refer to PersonInfo.cs for additional logic applied to this rule.
        /// </summary>
        /// <param DateOfBirth="context"></param>
        /// <returns></returns>
        public override bool CanBeAppliedTo( object context )
        {
            if( context == null || context.GetType() != typeof( Person )
                && context.GetType().BaseType != typeof( Person ))
            {
                return true;
            }

            Person aPerson = context as Person;
           
            if( aPerson != null 
                && aPerson.DateOfBirth != DateTime.MinValue )
            {
                return true;
            }
            else
            {
                IAccountView accountView = ActionHelper.LoadAccountView();

                if( accountView == null )
                {
                    return true;
                }

                Account anAccount = accountView.GetModel();

                string financialClass = string.Empty;
            
                if( anAccount != null
                    && anAccount.FinancialClass != null )
                {
                    financialClass = anAccount.FinancialClass.Code;
                }

                if( (financialClass != FC_70 && financialClass != FC_71 && financialClass != FC_72 
                    && financialClass != FC_73  && financialClass != FC_83 && financialClass != FC_30 )
                    
                    && ( anAccount.Activity == null
                            || anAccount.Activity.GetType() != typeof(PreRegistrationActivity) ) )
                {
                    return true;
                }

                if( context.GetType() == typeof(Guarantor)
                    || context.GetType() == typeof(Insured) )
                {                    
                    if( aPerson.Relationships.Count == 0 )
                    {
                        return true;
                    }

                    foreach(Relationship r in aPerson.Relationships)
                    {
                        if( r != null
							&& r.Type != null
							&&
							(r.Type.Code == RelationshipType.RELATIONSHIPTYPE_EMPLOYEE
                            || r.Type.Code == RelationshipType.RELATIONSHIPTYPE_UNKNOWN
                            || r.Type.Code == RelationshipType.RELATIONSHIPTYPE_BLANK
                            || r.Type.Code == RelationshipType.RELATIONSHIPTYPE_EMPTY
                            || r.Type.Code == RelationshipType.RELATIONSHIPTYPE_ORGANDONOR      
                            || r.Type.Code == RelationshipType.RELATIONSHIPTYPE_CADAVAR 
                            || r.Type.Code == RelationshipType.RELATIONSHIPTYPE_INJUREDPLAINTIFF)
							)
                        {
                            return true;
                        }
                    }
                }

                if( this.FireEvents && PersonDateOfBirthPreferredEvent != null )
                {
                    PersonDateOfBirthPreferredEvent( this, new PropertyChangedArgs( this.AssociatedControl ) );
                }
                return false;
            }
        }

        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public PersonDateOfBirthPreferred()
        {
        }
        #endregion

        #region Data Elements
        
        #endregion

        #region Constants
        private const string    FC_70   = "70",
                                FC_71   = "71",
                                FC_72   = "72",
                                FC_73   = "73",
                                FC_83   = "83",
                                FC_30   = "30";
        #endregion
    }
}
