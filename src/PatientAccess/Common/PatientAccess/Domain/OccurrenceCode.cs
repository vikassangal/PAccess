using System;
using System.Collections;

namespace PatientAccess.Domain 
{
    [Serializable]
    public class OccurrenceCode : CodedReferenceValue, IComparable 
    {
        #region Event Handlers
        #endregion

        #region Methods
        public override string ToString()
        {   
            return String.Format("{0} {1}", Code, Description);
        }

        public bool IsAccidentCrimeOccurrenceCode()
        {
            return IsAutoAccidentOrEmploymentRelatedOrTortLiabilityOccurrenceCode() ||
                   IsOtherAccidentOccurrenceCode() ||
                   IsCrimeOccurrenceCode();
        }

        public bool IsCrimeOccurrenceCode()
        {
            return Code == OCCURRENCECODE_CRIME;
        }

        public bool IsOtherAccidentOccurrenceCode()
        {
            return Code == OCCURRENCECODE_ACCIDENT_OTHER;
        }
        public bool IsOccurenceCode50()
        {
            return Code == OCCURENCECODE_DATE_OF_RA_EOMB;
        }
        public bool IsAutoAccidentOccurrenceCode()
        {
            return Code == OCCURRENCECODE_ACCIDENT_AUTO ||
                   Code == OCCURRENCECODE_ACCIDENT_AUTO_NO_FAULT;
        }

        public bool IsEmploymentRelatedOrTortLiabilityOccurrenceCode()
        {
            return Code == OCCURRENCECODE_ACCIDENT_EMPLOYER_REL ||
                   Code == OCCURRENCECODE_ACCIDENT_TORT_LIABILITY;
        }

        public bool IsAutoAccidentOrEmploymentRelatedOrTortLiabilityOccurrenceCode()
        {
            return IsAutoAccidentOccurrenceCode() ||
                   IsEmploymentRelatedOrTortLiabilityOccurrenceCode();
        }

        public bool IsSystemOccurrenceCode( OccurrenceCode code )
        {
            bool result = false;
            foreach( string occString in SystemOccurrenceCodes )
            {
                if( occString == code.Code )
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        public int CompareTo(object obj)
        {           
            if(obj is OccurrenceCode) 
            {
                OccurrenceCode occ = (OccurrenceCode) obj;

                return Code.CompareTo( occ.Code );
            }
    
            throw new ArgumentException("object is not an OccurrenceCode");                
        }

        #endregion

        #region Properties
        /// <summary>
        /// Date when the event triggering this OccurrenceCode OccurredOn.
        /// </summary>
        public DateTime OccurrenceDate
        {
            get
            {
                return i_OccurrenceDate;
            }
            set
            {
                i_OccurrenceDate = value;
            }
        }
        #endregion

        #region Private Methods
        private void LoadSystemOccurrenceCodes()
        {
            i_SystemOccurrenceCodes.Add( OCCURRENCECODE_ACCIDENT_AUTO );
            i_SystemOccurrenceCodes.Add( OCCURRENCECODE_ACCIDENT_AUTO_NO_FAULT );
            i_SystemOccurrenceCodes.Add( OCCURRENCECODE_ACCIDENT_EMPLOYER_REL );
            i_SystemOccurrenceCodes.Add( OCCURRENCECODE_ACCIDENT_TORT_LIABILITY );
            i_SystemOccurrenceCodes.Add( OCCURRENCECODE_ACCIDENT_OTHER );
            i_SystemOccurrenceCodes.Add( OCCURRENCECODE_CRIME );
            i_SystemOccurrenceCodes.Add( OCCURRENCECODE_ILLNESS );
            i_SystemOccurrenceCodes.Add( OCCURRENCECODE_RETIREDATE );
            i_SystemOccurrenceCodes.Add( OCCURRENCECODE_SPOUSERETIRED );
        }

        #endregion

        #region Private Properties
        private ArrayList SystemOccurrenceCodes
        {
            get
            {
                LoadSystemOccurrenceCodes();
                return i_SystemOccurrenceCodes;
            }        
        }        
      
        #endregion

        #region Construction and Finalization
        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="oid"></param>
        /// <param name="description"></param>
        /// <param name="occurredOn"></param>
        public OccurrenceCode(long oid, string description, DateTime occurredOn)
            : base( oid, description )
        {
            OccurrenceDate = occurredOn ;
        }
        /// <summary>
        /// Default Constructor
        /// </summary>
        public OccurrenceCode()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oid"></param>
        /// <param name="version"></param>
        /// <param name="description"></param>
        public OccurrenceCode( long oid, DateTime version, string description )
            : base( oid, version, description )
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oid"></param>
        /// <param name="version"></param>
        /// <param name="description"></param>
        /// <param name="code"></param>
        public OccurrenceCode( long oid, DateTime version, string description, string code )
            : base( oid, version, description, code )
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oid"></param>
        /// <param name="version"></param>
        /// <param name="description"></param>
        /// <param name="code"></param>
        /// <param name="occurredDate"></param>
        public OccurrenceCode(long oid, DateTime version, string description, string code, DateTime occurredDate)
            : base(oid, version, description, code)
        {
            this.OccurrenceDate = occurredDate;
        }

        #endregion

        #region Data Elements        
        private DateTime i_OccurrenceDate;
        private ArrayList i_SystemOccurrenceCodes = new ArrayList();

        public const string
            OCCURRENCECODE_ACCIDENT_AUTO             = "01",
            OCCURRENCECODE_ACCIDENT_AUTO_NO_FAULT    = "02",
            OCCURRENCECODE_ACCIDENT_TORT_LIABILITY   = "03",
            OCCURRENCECODE_ACCIDENT_EMPLOYER_REL     = "04",            
            OCCURRENCECODE_ACCIDENT_OTHER            = "05",
            OCCURRENCECODE_CRIME                     = "06",
            OCCURRENCECODE_LASTMENSTRUATION          = "10",
            OCCURRENCECODE_ILLNESS                   = "11",
            OCCURRENCECODE_RETIREDATE                = "18",
            OCCURRENCECODE_SPOUSERETIRED             = "19",
            OCCURENCECODE_DATE_OF_RA_EOMB            = "50";
        #endregion
    }

    public class SortOccurrencesByCode : IComparer
    {
        public int Compare( object obj1, object obj2 )
        {
            OccurrenceCode a = ( OccurrenceCode )obj1;
            OccurrenceCode b = ( OccurrenceCode )obj2;

            return a.Code.CompareTo( b.Code );
        }
    }

    public class OccurrenceCodeComparerByCodeAndDate : IComparer
    {
        public int Compare( object obj1, object obj2 )
        {
            var a = (OccurrenceCode)obj1;
            var b = (OccurrenceCode)obj2;

            if ( a.Code != b.Code )
            {
                return a.Code.CompareTo( b.Code );
            }

            var aDateTimeValue = a.OccurrenceDate;
            var bDateTimeValue = b.OccurrenceDate;

            if ( aDateTimeValue == DateTime.MinValue )
            {
                aDateTimeValue = DateTime.MaxValue;
            }

            if ( bDateTimeValue == DateTime.MinValue )
            {
                bDateTimeValue = DateTime.MaxValue;
            }

            return ( aDateTimeValue.CompareTo( bDateTimeValue ) );
        }
    }
}