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
	/// Summary description for TypeOfProductBroker.
	/// </summary>
    [Serializable]
    public class TypeOfProductBroker : AbstractBroker, ITypeOfProductBroker
    {
		#region Constants 

        private const string DBPROCEDURE_SELECT_ALL_PRODUCT_TYPES  =
            "Insurance.SelectAllTypesOfProducts";

		#endregion Constants 

		#region Fields 

        private static readonly ILog _logger =
            LogManager.GetLogger( typeof( TypeOfProductBroker ) );

		#endregion Fields 

		#region Constructors 

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeOfProductBroker"/> class.
        /// </summary>
        /// <param name="cxnString">The CXN string.</param>
        public TypeOfProductBroker( string cxnString ) : base( cxnString )
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="TypeOfProductBroker"/> class.
        /// </summary>
        /// <param name="txn">The TXN.</param>
        public TypeOfProductBroker( SqlTransaction txn ) : base( txn )
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="TypeOfProductBroker"/> class.
        /// </summary>
        public TypeOfProductBroker()
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
        public IList AllTypeOfProducts()
        {
            ArrayList typeOfProducts;

            try
            {
                CacheManager cacheManager =
                    new CacheManager();
                typeOfProducts =
                    (ArrayList)cacheManager.GetCollectionBy(CacheKeys.CACHE_KEY_FOR_TYPESOFPRODUCTS, LoadAllProductTypes);
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
        public TypeOfProduct TypeOfProductWith( long oid )
        {
            TypeOfProduct typeOfProduct = null;
            ICollection productTypes = this.AllTypeOfProducts();

            foreach( TypeOfProduct product in productTypes )
            {
                if( product.Oid == oid )
                {
                    typeOfProduct = product;
                    break;
                }
            }

            if( typeOfProduct == null )
            {
                typeOfProduct = new TypeOfProduct();
                typeOfProduct.Oid = oid;
            }

            return typeOfProduct;
        }

        /// <summary>
        /// Builds the product type from.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        private static TypeOfProduct BuildProductTypeFrom(SafeReader reader)
        {                
            TypeOfProduct typeOfProduct = new TypeOfProduct();

            typeOfProduct.Oid = reader.GetInt32("Oid");
            typeOfProduct.Description = reader.GetString("Description");

            return typeOfProduct;
        }


        /// <summary>
        /// Loads all product types.
        /// </summary>
        /// <returns></returns>
        private ArrayList LoadAllProductTypes()
        {
            ArrayList allCreditCardTypes = new ArrayList();
            SqlCommand sqlCommand = null;
            SafeReader reader = null;

            try
            {
                sqlCommand = this.CommandFor(DBPROCEDURE_SELECT_ALL_PRODUCT_TYPES);

                reader = this.ExecuteReader(sqlCommand);

                while (reader.Read())
                {
                    allCreditCardTypes.Add(BuildProductTypeFrom(reader));
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

		#endregion Methods 
    }
}
