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
	/// Summary description for TypeOfVerificationRuleBroker.
	/// </summary>
    [Serializable]
    public class TypeOfVerificationRuleBroker : AbstractBroker, ITypeOfVerificationRuleBroker
    {
		#region Constants 

        private const string DBPROCEDURE_SELECT_ALL_VERIFICATION_RULES  =
            "Insurance.SelectAllTypesOfVerificationRules";

		#endregion Constants 

		#region Fields 

        private static readonly ILog _logger =
            LogManager.GetLogger( typeof( TypeOfProductBroker ) );

		#endregion Fields 

		#region Constructors 

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeOfProductBroker"/> class.
        /// </summary>
        /// <param name="txn">The TXN.</param>
        public TypeOfVerificationRuleBroker( SqlTransaction txn ) : base( txn )
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="TypeOfVerificationRuleBroker"/> class.
        /// </summary>
        /// <param name="cxnString">The CXN string.</param>
        public TypeOfVerificationRuleBroker( string cxnString ) : base( cxnString )
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="TypeOfProductBroker"/> class.
        /// </summary>
        public TypeOfVerificationRuleBroker()
        {
        }


		#endregion Constructors 

		#region Properties 

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
        /// Get a list of all TypeOfProduct objects.
        /// </summary>
        /// <returns></returns>
        public IList AllTypeOfVerificationRules()
        {
            ArrayList typeOfProducts;

            try
            {
                CacheManager cacheManager =
                    new CacheManager();
                typeOfProducts =
                    (ArrayList)cacheManager.GetCollectionBy(CacheKeys.CACHE_KEY_FOR_TYPESOFVERIFICATIONRULES, LoadAllVerificationRules);
            }
            catch (Exception anyException)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom(anyException, Logger);
            }

            return typeOfProducts;
        }


        /// <summary>
        /// Get one TypeOfProduct object based on the oid.
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public TypeOfVerificationRule TypeOfVerificationRuleWith(long oid)
        {
            TypeOfVerificationRule typeOfVerificationRule = null;
            ICollection allTypeOfVerificationRules = this.AllTypeOfVerificationRules();

            foreach (TypeOfVerificationRule product in allTypeOfVerificationRules)
            {
                if( product.Oid == oid )
                {
                    typeOfVerificationRule = product;
                    break;
                }
            }

            if( typeOfVerificationRule == null )
            {
                typeOfVerificationRule = new TypeOfVerificationRule();
                typeOfVerificationRule.Oid = oid;
            }

            return typeOfVerificationRule;
        }


        /// <summary>
        /// Builds the verification rule from.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        private static TypeOfVerificationRule BuildVerificationRuleFrom(SafeReader reader)
        {
            TypeOfVerificationRule typeOfVerificationRule = new TypeOfVerificationRule();

            typeOfVerificationRule.Oid = reader.GetInt32("Oid");
            typeOfVerificationRule.Description = reader.GetString("Description");
            return typeOfVerificationRule;
        }


        /// <summary>
        /// Loads all product types.
        /// </summary>
        /// <returns></returns>
        private ArrayList LoadAllVerificationRules()
        {
            ArrayList allVerificationRules = new ArrayList();
            SqlCommand sqlCommand = null;
            SafeReader reader = null;

            try
            {
                sqlCommand = this.CommandFor(DBPROCEDURE_SELECT_ALL_VERIFICATION_RULES);

                reader = this.ExecuteReader(sqlCommand);

                while (reader.Read())
                {
                    allVerificationRules.Add(BuildVerificationRuleFrom(reader));
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

            return allVerificationRules;
        }

		#endregion Methods 
    }
}