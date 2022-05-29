using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for PrimaryInsuredDateOfBirthRequired.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class PrimaryInsuredDateOfBirthRequired : LeafRule
    {
        #region Event Handlers
        public event EventHandler PrimaryInsuredDateOfBirthRequiredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            PrimaryInsuredDateOfBirthRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            PrimaryInsuredDateOfBirthRequiredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.PrimaryInsuredDateOfBirthRequiredEvent = null;   
        }

        public override bool ShouldStopProcessing()
        {
            return true;
        }

        public override void ApplyTo( object context )
        {
        }

        /// <summary>
        /// CanBeAppliedTo - this rule is a member of a composite (to PrimaryInsuredInfo).
        /// Refer to PrimaryInsuredInfo.cs for additional logic applied to this rule.
        /// </summary>
        /// <param DateOfBirth="context"></param>
        /// <returns></returns>
        public override bool CanBeAppliedTo( object context )
        {
            if( context == null || context.GetType() != typeof( Account ))
            {
                return true;
            }
            

            Account anAccount = context as Account;

            string financialClass = string.Empty;
            
            if( anAccount != null
                && anAccount.FinancialClass != null )
            {
                financialClass = anAccount.FinancialClass.Code;
            }

            if( financialClass == FC_70 || financialClass == FC_71 || financialClass == FC_72 
                || financialClass == FC_73  || financialClass == FC_83 || financialClass == FC_30 )
            {
                return true;
            }

            Coverage primaryCoverage = anAccount.Insurance.CoverageFor(CoverageOrder.PRIMARY_OID);
            Insured aPrimaryInsured = null;

            if( primaryCoverage != null )
            {
                aPrimaryInsured = primaryCoverage.Insured;
            }            

            if( aPrimaryInsured == null 
                || aPrimaryInsured.DateOfBirth != DateTime.MinValue )
            {
                return true;
            }
            else
            {                  
                if( aPrimaryInsured.Relationships.Count != 0 )
                {
                    foreach(Relationship r in aPrimaryInsured.Relationships)
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
                else
                {
                    return true;
                }

                if( this.FireEvents && PrimaryInsuredDateOfBirthRequiredEvent != null )
                {
                    PrimaryInsuredDateOfBirthRequiredEvent( this, new PropertyChangedArgs( this.AssociatedControl ) );
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
        public PrimaryInsuredDateOfBirthRequired()
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
