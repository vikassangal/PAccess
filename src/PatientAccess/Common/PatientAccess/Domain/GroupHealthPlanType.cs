using System;

namespace PatientAccess.Domain
{
    [Serializable]
    public class GroupHealthPlanType : ReferenceValue
    {
        #region Event Handlers
        #endregion

        #region Methods

        public static GroupHealthPlanType NewSpouseType()
        {
            return new GroupHealthPlanType( SPOUSE_OID, "SPOUSE" );
        }

        public static GroupHealthPlanType NewSelfType()
        {
            return new GroupHealthPlanType( SELF_OID, "SELF" );
        }

        public static GroupHealthPlanType NewBothType()
        {
            return new GroupHealthPlanType( BOTH_OID, "BOTH" );
        }

        public static GroupHealthPlanType NewOtherFamiliyType()
        {
            return new GroupHealthPlanType( OTHER_FAMILY_OID, "OTHER_FAMILY_MEMBER" );
        }

        public static string SelfCode
        {
            get
            {
                return "S";
            }
        }

        public static string SpouseCode
        {
            get
            {
                return "P";
            }
        }

        public static string BothCode
        {
            get
            {
                return "B";
            }
        }

        public static string OtherFamilityCode
        {
            get
            {
                return "O";
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
        public GroupHealthPlanType() : base()
        {
            this.Oid = -1;
        }
        private GroupHealthPlanType( long oid, string description )
            : base( oid, description )
        {
        }
       
        #endregion

        #region Data Elements
        #endregion

        #region Constants

        public const long
            BOTH_OID    = 0,
            SELF_OID    = 1,
            SPOUSE_OID  = 2;

        private const long
            OTHER_FAMILY_OID = 3;

        #endregion
    }
}
