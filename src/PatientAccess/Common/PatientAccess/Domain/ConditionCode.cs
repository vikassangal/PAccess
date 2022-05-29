using System;
using System.Collections;
using System.Linq;

namespace PatientAccess.Domain
{
    /// <summary>
    /// Summary description for ConditionCode.
    /// </summary>
    [Serializable]
    public class ConditionCode : CodedReferenceValue, IComparable
    {
        #region Event Handlers
        #endregion

        #region Methods
        public override string ToString()
        {
            return String.Format( "{0} {1}", Code, Description );
        }

        public bool IsSystemConditionCode()
        {
            return SystemConditionCodes.Cast<string>().Any( x => x == Code );
        }

        public int CompareTo( object obj )
        {
            if ( obj is ConditionCode )
            {
                ConditionCode cond = ( ConditionCode )obj;

                return Code.CompareTo( cond.Code );
            }

            throw new ArgumentException( "object is not a ConditionCode" );
        }

        internal bool IsEmergencyToInpatientTransferConditionCode()
        {
            return Code == ADMITTED_AS_IP_FROM_ER;
        }

        #endregion

        #region Properties
        #endregion

        #region Private Methods
        private void LoadSystemConditionCodes()
        {
            i_SystemConditionCodes.Add( CONDITIONCODE_ALLMEMBERS_RETIRED );
            i_SystemConditionCodes.Add( CONDITIONCODE_AGE_NO_GHP );
            i_SystemConditionCodes.Add( CONDITIONCODE_DISABILITY_NO_GHP );
            i_SystemConditionCodes.Add( CONDITIONCODE_DOB_OVER_100Y );
            i_SystemConditionCodes.Add( CONDITIONCODE_AGE_GHP_LIMIT_NOT_EXCEED );
            i_SystemConditionCodes.Add( CONDITIONCODE_DISABILITY_GHP_LIMIT_NOT_EXCEED );
            i_SystemConditionCodes.Add( CONDITIONCODE_QUALIFYING_CLINICAL_TRIALS );
            i_SystemConditionCodes.Add( ADMITTED_AS_IP_FROM_ER );
        }
        #endregion

        #region Private Properties
        private ArrayList SystemConditionCodes
        {
            get
            {
                LoadSystemConditionCodes();
                return i_SystemConditionCodes;
            }
        }

        #endregion

        #region Construction and Finalization
        public ConditionCode()
        {
        }

        public ConditionCode( long oid, DateTime version, string description )
            : base( oid, version, description )
        {
        }

        public ConditionCode( long oid, DateTime version, string description, string code )
            : base( oid, version, description, code )
        {
        }
        #endregion

        #region Data Elements

        private readonly ArrayList i_SystemConditionCodes = new ArrayList();
        #endregion

        #region Constants

        public const string
            CONDITIONCODE_ALLMEMBERS_RETIRED = "09",
            CONDITIONCODE_AGE_NO_GHP = "10",
            CONDITIONCODE_DISABILITY_NO_GHP = "11",
            CONDITIONCODE_DOB_OVER_100Y = "17",
            CONDITIONCODE_AGE_GHP_LIMIT_NOT_EXCEED = "28",
            CONDITIONCODE_DISABILITY_GHP_LIMIT_NOT_EXCEED = "29",
            CONDITIONCODE_PRIVATE_ROOM_MEDICALLY_REQUIRED = "39",
            CONDITIONCODE_SEMI_PRIVATE_ROOM_NOT_AVAILABLE = "38",
            ADMITTED_AS_IP_FROM_ER = "P7",
            CONDITIONCODE_QUALIFYING_CLINICAL_TRIALS = "30";

        #endregion
    }

    public class SortConditionsByCode : IComparer
    {
        public int Compare( object obj1, object obj2 )
        {
            ConditionCode a = ( ConditionCode )obj1;
            ConditionCode b = ( ConditionCode )obj2;

            return a.Code.CompareTo( b.Code );
        }
    }
}
