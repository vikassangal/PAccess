using System;
using System.Collections;
using System.Globalization;
using System.Text;
using PatientAccess.Domain;
using log4net;

namespace PatientAccess.Persistence
{
    [Serializable]
    public abstract class SqlBuilderStrategy
    {
        #region Construction and Finalization

        #endregion

        #region Methods

        protected internal virtual string AddColumnsAndValuesToSqlStatement( OrderedList orderedList, string tableName )
        {
            if ( orderedList != null )
            {
                insertColumnsSql.Append( tableName + " (");
                foreach( string key in orderedList.Keys )
                {
                    insertColumnsSql.Append( key + "," );

                    if( orderedList[key] == null )
                    {
                        c_log.Debug( "KEY - " + key + " has associated NULL value.");
                    }
                    insertValuesSql.Append( FormatForSql( orderedList[key] ) + "," );
                }
                insertColumnsSql.Replace( ",", ")", insertColumnsSql.Length - 1,1 );
                insertValuesSql.Replace( ",", ")", insertValuesSql.Length - 1,1 );

                sqlStmt = insertColumnsSql + insertValuesSql.ToString();
              
            }
            return sqlStmt ;
        }

        protected internal virtual string AddColumnsAndValuesToDeleteSqlStatement( OrderedList orderedList, string tableName )
        {
            string deleteSql = string.Empty ;
         
            StringBuilder deletesqlBuilder = new StringBuilder( "DELETE FROM ");
            deletesqlBuilder.Append( tableName );
            deletesqlBuilder.Append( " WHERE ");
            bool  firstValue = true;
            foreach( string key in orderedList.Keys )
            {

                if(!firstValue)
                {
                    deletesqlBuilder.Append(" AND ");
                }
                deletesqlBuilder.Append( key + " = " );
                deletesqlBuilder.Append( FormatForSql( orderedList[key] ));
                firstValue = false;
            }
            deletesqlBuilder.Replace( ",", ")", deletesqlBuilder.Length - 1,1 );
    
            deleteSql = deletesqlBuilder.ToString() ;
            return deleteSql;
            
        }

        protected internal virtual string AddColumnsAndValuesToUpdateSqlStatement( OrderedList orderedList, string tableName )
        {
            string updateSql = string.Empty ;
         
            StringBuilder updateSqlBuilder = new StringBuilder( "UPDATE ");
            updateSqlBuilder.Append( tableName );
            updateSqlBuilder.Append( " SET LRFLG = '' " );
            updateSqlBuilder.Append( " WHERE ");
            bool  firstValue = true;
            foreach( string key in orderedList.Keys )
            {
                if(!firstValue)
                {
                    updateSqlBuilder.Append(" AND ");
                }
                updateSqlBuilder.Append( key + " = " );
                updateSqlBuilder.Append( FormatForSql( orderedList[key] ));
                firstValue = false;
            }
            updateSqlBuilder.Replace( ",", ")", updateSqlBuilder.Length - 1,1 );
    
            updateSql = updateSqlBuilder.ToString() ;
            return updateSql;
            
        }

        /// <summary>
        /// Formats the previously generated insert string to make 
        /// it suitable for Sql
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public object FormatForSql( object val )
        {
            if( val == null )
            {
                return null;
            }

            Type t = val.GetType();
            string objectType = t.FullName;
            switch( objectType )
            {
                case "System.String":
                {
                    string strVal = (string)val;
                    if( strVal != CURRENT_TIMESTAMP  )
                    {
                        strVal = "'" + strVal.Replace( "'", "''" ) + "'";
                    }
                    return strVal;
                }
                default:
                {
                    return val;
                }
            }
        }

        /// <summary>
        /// Given a string, returns the integer equivalent of the string
        /// </summary>
        /// <param name="strval"></param>
        /// <returns></returns>
        public int ConvertToInt( string strval )
        {
            try
            {
                if( strval.Trim().Length == 0 )
                {
                    return 0;
                }
                else
                {
                    return( Convert.ToInt32(strval) );
                }
            }
            catch
            {
                return 0;
            }
        }
        /// <summary>
        /// Given a Date, returns the integer equivalent of the date in Mddyy format
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public int ConvertDateToIntInMddyyFormat( DateTime date )
        {
            int returnValue = 0;
            try
            {
                if( date == DateTime.MinValue )
                {
                    return returnValue;
                }
                else
                {
                    returnValue = 
                        ConvertToInt( date.ToString( SHORT_DATE_TO_INTEGER_FORMAT, DateTimeFormatInfo.InvariantInfo ) );
                    return returnValue;
                }
            }
            catch
            {
                return returnValue;
            }
        }
        /// <summary>
        /// Given a Date, returns the integer equivalent of the date in yyMMdd format
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public int ConvertDateToIntInyyMMddFormat( DateTime date )
        {
            int returnValue = 0;
            try
            {
                if( date == DateTime.MinValue )
                {
                    return returnValue;
                }
                else
                {
                    returnValue = 
                        ConvertToInt( date.ToString( SHORT_DATE_TO_INTEGER_YYMMDD_FORMAT, DateTimeFormatInfo.InvariantInfo ) );
                    return returnValue;
                }
            }
            catch
            {
                return returnValue;
            }
        }
        /// <summary>
        /// Given a Date, returns the string format in yyyy-MM-dd format.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public string ConvertDateToStringInyyyyMMddFormat( DateTime date )
        {
            string returnValue = DateTime.Now.ToString( DATE_FORMAT, 
                DateTimeFormatInfo.InvariantInfo );
            try
            {
                if( date != DateTime.MinValue )
                {
                    returnValue = date.ToString( DATE_FORMAT, 
                        DateTimeFormatInfo.InvariantInfo );
                }
                return returnValue;
            }
            catch
            {
                return returnValue;
            }
        }
        /// <summary>
        /// Given a Date, returns the string format in yyyyMMdd format.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public string ConvertDateToStringInShortyyyyMMddFormat( DateTime date )
        {
            string returnValue = String.Empty;
            try
            {
                if( date != DateTime.MinValue )
                {
                    returnValue = date.ToString( DATE_YYYYMMDD_FORMAT,
                        DateTimeFormatInfo.InvariantInfo );
                }
                return returnValue;
            }
            catch
            {
                return returnValue;
            }
        }
        /// <summary>
        /// Given a Date, returns the string format in MMddCCYY format.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public string ConvertDateToStringInShortMMddCCYYFormat(DateTime date)
        {
            string returnValue = String.Empty;
            try
            {
                if (date != DateTime.MinValue)
                {
                    returnValue = date.ToString(LONG_DATE_FORMAT,
                        DateTimeFormatInfo.InvariantInfo);
                }
                return returnValue;
            }
            catch (FormatException)
            {
                return returnValue;
            }
        }  
        /// <summary>
        /// Given a Date, returns the string format in yyyy-MM-dd format.
        /// </summary>
        /// <returns></returns>
        public string ConvertMinDateToStringInyyyyMMddFormat()
        {
            string returnValue = DateTime.MinValue.ToString( DATE_FORMAT, DateTimeFormatInfo.InvariantInfo );

            return returnValue;
        }
        /// <summary>
        /// Given a Date, returns the integer equivalent of the Time in HHmm format
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public int ConvertTimeToIntInHHmmFormat( DateTime date )
        {
            int returnValue = 0;
            try
            {
                if( date == DateTime.MinValue )
                {
                    return returnValue;
                }
                else
                {
                    returnValue = 
                        ConvertToInt( date.ToString( SHORT_TIME_TO_INTEGER_FORMAT, DateTimeFormatInfo.InvariantInfo ) );
                    return returnValue;
                }
            }
            catch
            {
                return returnValue;
            }
        }

        public int ConvertTimeToIntInHHmmSSFormat( DateTime date )
        {
            int returnValue = 0;
            try
            {
                if( date == DateTime.MinValue )
                {
                    return returnValue;
                }
                else
                {
                    returnValue = 
                        ConvertToInt( date.ToString( LONG_TIME_TO_INTEGER_FORMAT, DateTimeFormatInfo.InvariantInfo ) );
                    return returnValue;
                }
            }
            catch
            {
                return returnValue;
            }
        }


        /// <summary>
        /// Given a Time, returns the string format in HHMM format.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public string ConvertTimeToStringInHHmmFormat( DateTime date )
        {
            string returnValue = "0000";
            try
            {
                if( date != DateTime.MinValue )
                {
                    returnValue = date.ToString( SHORT_TIME_TO_INTEGER_FORMAT,
                        DateTimeFormatInfo.InvariantInfo );
                }
                return returnValue;
            }
            catch
            {
                return returnValue;
            }
        }
        /// <summary>
        /// Given an Account object, updates the hashtable values with values from this object
        /// </summary>
        /// <param name="account"></param>
        public virtual void UpdateColumnValuesUsing( Account account )
        {
        }

        /// <summary>
        /// Given two Account objects, updates the hashtable values with values from this object
        /// </summary>
        /// <param name="accountOne"></param>
        /// <param name="accountTwo"></param>
        public virtual void UpdateColumnValuesUsing( Account accountOne, Account accountTwo )
        {
        }

        /// <summary>
        /// This is an abstract method.
        /// Initializes the hashtable with the default values for each column of 
        /// the PBAR table, represented by the keys of the hashtable
        /// </summary>
        public abstract void InitializeColumnValues();
        
        /// <summary>
        /// This is an abstract method.
        /// Makes calls to all methods involved in creating the insert string 
        /// and retreives the insert string 
        /// </summary>
        /// <param name="account"></param>
        /// <param name="transactionKeys"></param>
        /// <returns></returns>
        public virtual ArrayList BuildSqlFrom( Account account, TransactionKeys transactionKeys )
        {
            return new ArrayList();
        }
        /// <summary>
        /// This is an abstract method.
        /// Makes calls to all methods involved in creating the insert string 
        /// and retreives the insert string 
        /// </summary>
        /// <param name="accountOne">Patient(Visit) One Information</param>
        /// <param name="accountTwo">Patient(Visit) Two Information</param>
        /// <param name="transactionKeys"></param>
        /// <returns></returns>

        public virtual ArrayList BuildSqlFrom( Account accountOne, Account accountTwo, TransactionKeys transactionKeys )
        {
            return new ArrayList() ;
        }


        protected void ReinitializeColumnsAndValues()
        {
            SqlStatement = string.Empty;
            insertColumnsSql = new StringBuilder("INSERT INTO ");
            insertValuesSql = new StringBuilder(" VALUES ( ");

        }
                 
        #endregion

        #region Properties

        public string SqlStatement
        {
            get
            {
                return sqlStmt;
            }
            private set
            {
                sqlStmt = value;
            }
        }
     
        public ArrayList SqlStatements
        {
            get
            {
                return i_SqlStatements;
            }
        }        
        #endregion
        protected string FormattedCity(string city)
        {
            if(city.Trim().Length > 15 )
            {
                return city.Trim().Substring(0,15);
            }
            else
            {
                return city.Trim();
            }
        }

        protected string FormatAddress( string address )
        {
            if( address.Trim().Length > 25 )
            {
                return address.Trim().Substring( 0,25 );
            }
            else
            {
                return address.Trim();
            }
        }

        protected string FormatAddressStreet1(string address)
        {
            if (address.Trim().Length > 45)
            {
                return address.Trim().Substring(0, 45);
            }
            else
            {
                return address.Trim();
            }
        }
        protected string FormatAddressStreet2(string address)
        {
            if (address.Trim().Length > 30)
            {
                return address.Trim().Substring(0, 30);
            }
            else
            {
                return address.Trim();
            }
        }

        #region Data Elements

        private StringBuilder insertColumnsSql = new StringBuilder("INSERT INTO ");
        private StringBuilder insertValuesSql = new StringBuilder(" VALUES ( ");
        private string sqlStmt;
        private ArrayList i_SqlStatements = new ArrayList();

        private static readonly ILog c_log = LogManager.GetLogger( typeof( SqlBuilderStrategy ) );

        #endregion

        #region Constants

        public const string
            WORKSTATION_ID = "PACCESS",
            SECURITY_CODE = "KEVN",
            CURRENT_TIMESTAMP = "'0001-01-01-00.00.00.000000'";

        private const string
            DATE_FORMAT = "yyyy-MM-dd",
            LONG_DATE_FORMAT = "MMddyyyy",
            DATE_YYYYMMDD_FORMAT = "yyyyMMdd",
            SHORT_DATE_TO_INTEGER_FORMAT = "Mddyy",
            SHORT_TIME_TO_INTEGER_FORMAT = "HHmm",
            SHORT_DATE_TO_INTEGER_YYMMDD_FORMAT = "yyMMdd",
            LONG_TIME_TO_INTEGER_FORMAT = "HHmmss";

        public static readonly string BLANK_FLAG = string.Empty;

        public const string ADD_FLAG    = "A",
            CHANGE_FLAG = "C",
            PATIENT_RECORD_NUMBER = "$#P@%&",
            OTHER_RECORD_NUMBER = "&*@&Q%",
            LOG_NUMBER  = "$#L@%", 
            GUARANTOR_TRANSACTION_NUMBER = "$#G%&*" ,
            PRIMARY_INSURANCE_RECORD_NUMBER   = "&*%P@#$",
            SECONDARY_INSURANCE_RECORD_NUMBER = "&*%S@#$",
            ACCOUNT_TWO_PRIMARY_INSURANCE_RECORD_NUMBER   = "*2*P@#2$",
            ACCOUNT_TWO_SECONDARY_INSURANCE_RECORD_NUMBER = "$2*S@#2*";


            
        #endregion
    }
}
