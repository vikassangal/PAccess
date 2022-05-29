using System;
using System.Collections;
using System.Data.SqlClient;
using Extensions.Persistence;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using log4net;

namespace PatientAccess.Persistence
{
    /// <summary>
    /// Implements origin related data loading.
    /// </summary>
    [Serializable]
    public class CreditCardTypeBroker : AbstractBroker, ICreditCardTypeBroker
    {
		#region Constants 

        private const string DBPROCEDURE_SELECT_CREDIT_CARD_TYPES = 
            "FinancialCounseling.GetCreditCardTypes";

        private const string DBPROCEDURE_SELECT_CREDIT_CARD_TYPE_WITH =
            "FinancialCousneling.GetCreditCardTypeWith";

        #endregion Constants 

		#region Fields 

        private static readonly ILog _logger = 
            LogManager.GetLogger( typeof( CreditCardTypeBroker ) );

		#endregion Fields 

		#region Constructors 

        public CreditCardTypeBroker( string cxnString )
            : base( cxnString )
        {
        }


        public CreditCardTypeBroker( SqlTransaction txn )
            : base( txn )
        {
        }


        public CreditCardTypeBroker()
            : base()
        {
        }

		#endregion Constructors 

		#region Properties 

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>The logger.</value>
        private static ILog Logger
        {
            get
            {
                return _logger;
            }
        }

        #endregion Properties 

		#region Methods 

        /// <summary>
        /// Get a list of all credit card type objects including oid, code and description.
        /// </summary>
        /// <returns></returns>
        public IList AllCreditCardTypes()
        {

            ArrayList cardTypes = null;
            
            try
            {
                CacheManager cacheManager = 
                    new CacheManager();
                cardTypes = 
                    (ArrayList)cacheManager.GetCollectionBy( CacheKeys.CACHE_KEY_FOR_CREDITCARDTYPES, LoadAllCreditCardTypes );
            }
            catch( Exception anyException )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( anyException, Logger );
            }

            return cardTypes;
        }

        /// <summary>
        /// Loads all credit card types.
        /// </summary>
        /// <returns></returns>
        private ArrayList LoadAllCreditCardTypes()
        {
            ArrayList allCreditCardTypes = new ArrayList();
            SqlCommand sqlCommand = null;
            SafeReader reader = null;

            try
            {
                sqlCommand = this.CommandFor( DBPROCEDURE_SELECT_CREDIT_CARD_TYPES );

                reader = this.ExecuteReader(sqlCommand);

                while( reader.Read())
                {
                    allCreditCardTypes.Add( BuildCreditCardProviderFrom( reader ) );
                }

            }
            catch (Exception anyException)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom(anyException, Logger);
            }
            finally
            {
                base.Close(reader);
                base.Close(sqlCommand);
            }

            return allCreditCardTypes;                
        }


        /// <summary>
        /// Builds the credit card provider from.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        private static CreditCardProvider BuildCreditCardProviderFrom( SafeReader reader )
        {
            CreditCardProvider creditCardProvider = new CreditCardProvider();

            creditCardProvider.Oid = reader.GetInt32( "Oid" );
            creditCardProvider.Code = reader.GetInt32( "Code" ).ToString();
            creditCardProvider.Description = reader.GetString( "Description" );

            return creditCardProvider;
        }


        /// <summary>
        /// Get one CreditCardType object based on the code.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public CreditCardProvider CreditCardTypeWith( string code )
        {
            CreditCardProvider selectedCreditCardProvider = null;

            if (null == code)
            {
                throw new ArgumentNullException("code" ) ;
            }

            try
            {

                IList creditCardProviders = this.AllCreditCardTypes();

                foreach(CreditCardProvider creditCardProvider in creditCardProviders)
                {
                    if( creditCardProvider.Code.Equals( code ) )
                    {
                        selectedCreditCardProvider = creditCardProvider;
                        break;
                    }
                }

                if( selectedCreditCardProvider == null )
                {
                    throw new ArgumentException("Cannot find credit card provider","code");
                }

            }
            catch( Exception anyException )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( anyException, Logger );
            }

            return selectedCreditCardProvider;
        }

		#endregion Methods 
    }
}
