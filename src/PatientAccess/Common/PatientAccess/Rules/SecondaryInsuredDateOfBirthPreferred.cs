using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for SecondaryInsuredDateOfBirthPreferred.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class SecondaryInsuredDateOfBirthPreferred : LeafRule
    {
        #region Event Handlers
        public event EventHandler SecondaryInsuredDateOfBirthPreferredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            SecondaryInsuredDateOfBirthPreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            SecondaryInsuredDateOfBirthPreferredEvent -= eventHandler;
            return true;
        }

        public override void UnregisterHandlers()
        {
            this.SecondaryInsuredDateOfBirthPreferredEvent = null;
        }

        public override bool ShouldStopProcessing()
        {
            return true;
        }

        public override void ApplyTo( object context )
        {
        }

        /// <summary>
        /// CanBeAppliedTo - this rule is a member of a composite (to SecondaryInsuredInfo).
        /// Refer to SecondaryInsuredInfo.cs for additional logic applied to this rule.
        /// </summary>
        /// <param DateOfBirth="context"></param>
        /// <returns></returns>
        public override bool CanBeAppliedTo( object context )
        {
            if ( context == null || context.GetType() != typeof( Account ))
            {
                return true;
            }

            Account anAccount = context as Account;

            Coverage secondaryCoverage = anAccount.Insurance.CoverageFor( CoverageOrder.SECONDARY_OID );
            Insured aSecondaryInsured = null;

            if ( secondaryCoverage != null )
            {
                aSecondaryInsured = secondaryCoverage.Insured;
            }

            if ( aSecondaryInsured == null
                || aSecondaryInsured.DateOfBirth != DateTime.MinValue )
            {
                return true;
            }

            bool result = true;
            if ( aSecondaryInsured.Relationships.Count == 0 )
            {
                return true;
            }
            foreach ( Relationship r in aSecondaryInsured.Relationships )
            {
                if ( r != null && r.Type != null &&
                    ( ValidRelationshipCode( r.Type.Code ) ||
                      ValidFinancialClass( anAccount ) )
                    )
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }

            if ( !result && this.FireEvents && SecondaryInsuredDateOfBirthPreferredEvent != null )
            {
                SecondaryInsuredDateOfBirthPreferredEvent( this, new PropertyChangedArgs( this.AssociatedControl ) );
                return false;
            }

            return true;
        }

        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        private static bool ValidRelationshipCode( string relationshipCode )
        {
            if ( relationshipCode == RelationshipType.RELATIONSHIPTYPE_EMPLOYEE
                            || relationshipCode == RelationshipType.RELATIONSHIPTYPE_UNKNOWN
                            || relationshipCode == RelationshipType.RELATIONSHIPTYPE_BLANK
                            || relationshipCode == RelationshipType.RELATIONSHIPTYPE_EMPTY
                            || relationshipCode == RelationshipType.RELATIONSHIPTYPE_ORGANDONOR
                            || relationshipCode == RelationshipType.RELATIONSHIPTYPE_CADAVAR
                            || relationshipCode == RelationshipType.RELATIONSHIPTYPE_INJUREDPLAINTIFF )
            {
                return true;
            }
            
            return false;
        }

        private static bool ValidFinancialClass( Account anAccount )
        {
            string financialClass = string.Empty;

            if ( anAccount != null
                && anAccount.FinancialClass != null )
            {
                financialClass = anAccount.FinancialClass.Code;
            }

            if ( anAccount.KindOfVisit == null || 
                 ( anAccount.KindOfVisit.Code == VisitType.PREREG_PATIENT && 
                   ( financialClass == FC_70 || financialClass == FC_71 || 
                     financialClass == FC_72 || financialClass == FC_73 || 
                     financialClass == FC_83 || financialClass == FC_30 ) ) )
            {
                return true;
            }
            
            return false;
        }
        #endregion

        #region Construction and Finalization
        public SecondaryInsuredDateOfBirthPreferred()
        {
        }
        #endregion

        #region Data Elements

        #endregion

        #region Constants
        private const string FC_70 = "70",
            FC_71 = "71",
            FC_72 = "72",
            FC_73 = "73",
            FC_83 = "83",
            FC_30 = "30";

        #endregion
    }
}
