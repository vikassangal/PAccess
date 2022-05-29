using System;
using System.Collections;
using System.Data;
using System.Text;
using Extensions.DB2Persistence;
using IBM.Data.DB2.iSeries;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using log4net;

namespace PatientAccess.Persistence
{
    /// <summary>
    /// <![CDATA[
    /// CodesBroker
    /// A Class for supporting general lists of objects that represent standard codes in
    /// a generic way
    /// 
    /// Implementing concrete classes from this base class
    /// 1. Inherit the actual broker from codesBroker
    /// 2. Implement the method InitProcNames in the new class
    ///     set the 2 props AllStoredProcName and WithStoredProcName With the names of the stored
    ///     procedures that will return the values
    ///     If the broker supports only 2 set of objects then the initProcName methods can set the
    ///         values in the method ifself.
    ///     If the broker supports multiple objects such as the demographics broker which supports
    ///         genders, maritalStatus' and languages then the values in this init method can be set
    ///         to "" (the empty string).
    /// 3. Implement the stored proc(s)
    ///     To use this class the methods MUST take a standard set of parameters
    ///     AllStoredProc Name is the name of the stored procedures used to get a list of all values.
    ///     For AllStoredProcName the proc the 1st parameter may be a key used
    ///     to look up the values. Normally this is a facility ID. This parameter is optional.
    ///     The second param must be 1 output parameter (io_cursor)
    /// 
    ///     The WithStoredProcName is used to get 1 value
    ///     For WithStoredProcName the stored procedure the 1st parameter may be a keyID (normally the 
    ///         facilityID) This parameter is optional
    ///     The 2nd parameter is a code value to look for and can be either a string or a long
    ///     The 3rd parameter must be a output cursor.
    /// 
    ///     Both of these methods MUST return columns named OID, CODE, DESCRIPTION, STATUSCD unless
    ///     The broker is giong to use the version of the methods in this class that takes as method as 
    ///     a delegate to read the values from the reader. (see below). If the broker IS using a
    ///     delegate that is passed in as the final parameter then they can call the output paramters what
    ///     ever they wish.
    /// 
    /// 4. Implement the get all methods in the actual broker. A broker may choose to cache its values
    ///     If this is the case then the LoadDataToArrayList is passed to the cacheManager.GetCollectionBy
    ///         Method as a delegate as shown below.
    /// 
    ///            CacheManager cacheManager = new CacheManager();
    ///            this.AllStoredProcName = SP_SELECTALLLANGUAGES;
    ///            allLanguages = cacheManager.GetCollectionBy(key, 
    ///                LoadDataToArrayList<Language>);
    ///         The LoadDataToArrayList method will be called as a delegate if the collection needs
    ///         to be loaded into cache. 
    ///     Alternately there are some brokers that do not cache their collections. In these cases they
    ///     can be called directly as in:
    /// 
    ///         ICollection finclassesForType = LoadDataToArrayList<FinancialClass>(financialClassTypeID);
    /// 4a. 
    ///     There are also version of these methonds named LoadDataToHashTable that do exactly what
    ///     their name implies.
    /// 
    /// 5. By default the class being loaded (T) must inherit from CodedReferenceValue. If it inherits 
    ///     from ReferenceValue then the broker should call LoadUncodedDataToArrayList as in:
    ///         typeOfVerificationRules = LoadUncodedDataToArrayList<TypeOfVerificationRule>();
    ///     This will create object that inherit from ReferenceValue.
    /// 
    /// 
    /// 6. Implement 'with' methods. These are the methods that retrieve 1 class from the database
    ///     based on the passed parameters. These methods come in several 'flavors'. They may take a
    ///     keyID to represent what is normally a facility ID as in :
    ///         selectedPlaceOfWorship = CodeWith<PlaceOfWorship>( facilityNumber,code);
    ///     or it may take only a single value which may be a string or a long as in:
    ///         selectedReligion = CodeWith<Religion>( oid );
    ///     or
    ///         selectedReligion = CodeWith<Religion>( code );
    ///     Again there is a difference in whether the class being loaded inherits from CodedReferenceValue
    ///         or from Reference value. If the latter is the case then the broker should call 
    ///         RefValueWith instead of CodeWith as in:
    /// 
    ///         typeOfVerificationRule = this.RefValueWith<TypeOfVerificationRule>(oid);
    ///
    /// Special considerations: 
    ///     There are some brokers that return hashtables rather than arraylists. To accomodate this
    ///     the broker should be written to call a local private delegate which calls the normal
    ///     LoadDataToArrayList generic method which will return an arraylist and then transform the
    ///     results into a hashtable before loading the collection, the hashtable, into cache. 
    ///     For an example of this refer to the method FinancialClassBroker.FinancialClassesFor()
    /// ]]>
    /// </summary>
    [Serializable]
    public class PBARCodesBroker : AbstractPBARBroker//CachingBroker 
    {
        #region Event Handlers
        #endregion

        #region Methods

        protected string AllStoredProcName
        {
            private get { return i_AllStoredProcName; }
            set { i_AllStoredProcName = value; }
        }

        protected string WithStoredProcName
        {
            private get { return i_WithStoredProcName; }
            set { i_WithStoredProcName = value; }
        }

        protected Facility Facility
        {
            get { return i_facility; }
            private set { i_facility = value; }
        }

        protected string HubName
        {
            get { return i_hubName; }
            private set { i_hubName = value; }
        }
        private string i_AllStoredProcName;
        private string i_WithStoredProcName;
        private Facility i_facility;
        private string i_hubName;
        #endregion

        #region Properties
        #endregion

        public delegate T ReadDataDelegate<T>(SafeReader reader);

        #region Methods
        public void InitFacility(long facilityID)
        {
            try
            {
                IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
                this.Facility = facilityBroker.FacilityWith(facilityID);

                HubName = this.Facility.ConnectionSpec.DatabaseName;
            }
            catch (Exception ex)
            {
                string msg = string.Format("Failed to Initialize PBARCodes Broker. Facility {0} stack: {1}",
                    facilityID, ex.StackTrace);
                throw BrokerExceptionFactory.BrokerExceptionFrom(msg, ex, c_log);
            }
        }

        public void InitFacility(string facilityCode)
        {
            try
            {
                IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
                this.Facility = facilityBroker.FacilityWith(facilityCode);

                HubName = this.Facility.ConnectionSpec.DatabaseName;
            }
            catch (Exception ex)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom("Failed to Initialize PBARCodes Broker", ex, c_log);
            }
        }

        /// <summary>
        /// Generic methods for getting a list of code objects without a key. 
        /// </summary>
        /// <typeparam name="T">The type of object being handled. The object must inherit from CodedReferenceValue</typeparam>
        /// <returns>An ArrayList of objects of type T</returns>
        public ICollection LoadDataToArrayList<T>() where T : CodedReferenceValue, new()
        {
            long? parm = null;
            return LoadDataToArrayListOpt<T>(parm, null);
        }
        /// <summary>
        /// Generic method to get a list of objects of type T using a delegate to read the data from the reader
        /// </summary>
        /// <typeparam name="T">The type of object being handled. The object must inherit from CodedReferenceValue</typeparam>
        /// <param name="readMethod">The delegate used to create the object by reading data from the reader</param>
        /// <returns>An ArrayList of objects of type T</returns>
        public ICollection LoadDataToArrayList<T>(ReadDataDelegate<T> readMethod)
            where T : CodedReferenceValue, new()
        {
            long? parm = null;
            return LoadDataToArrayListOpt<T>(parm, readMethod);
        }
        /// <summary>
        /// Generic method used to get a list of objects based on a key such as a facilityKey
        /// </summary>
        /// <typeparam name="T">The type of the object being handled. The object must inherit from CodedReferenceValue</typeparam>
        /// <param name="keyID">The key of the collection of objects being read. Normally a FacilityID</param>
        /// <returns>An ArrayList of objects of type T</returns>
        public ICollection LoadDataToArrayList<T>(long keyID)
            where T : CodedReferenceValue, new()
        {
            return LoadDataToArrayListOpt<T>(keyID, null);
        }
        /// <summary>
        /// Generic method used to get a list of objects based on a key such as a facilityKey
        /// and using a delegate to read the data from the data reader
        /// </summary>
        /// <typeparam name="T">The type of object being handled. The object must inherit from CodedReferenceValue</typeparam>
        /// <param name="keyID">The key of the collection of objects being read. Normally a FacilityID</param>
        /// <param name="readMethod">The delegate used to create the object T as data is read from the data reader</param>
        /// <returns>An ArrayList containing a objects of type T</returns>
        public ICollection LoadDataToArrayList<T>(long keyID, ReadDataDelegate<T> readMethod)
            where T : CodedReferenceValue, new()
        {
            return LoadDataToArrayListOpt<T>(keyID, readMethod);
        }
        /// <summary>
        /// Generic method used to get a list of objects of type T. this methods is used when objects have
        /// a common key which is a string.
        /// </summary>
        /// <typeparam name="T">The type of object being handled. The object must inherit from CodedReferenceValue</typeparam>
        /// <param name="keyID">The key which is common to all objects</param>
        /// <returns>An ArrayList containing a objects of type T</returns>
        public ICollection LoadDataToArrayList<T>(string keyID)
            where T : CodedReferenceValue, new()
        {
            return LoadDataToArrayListOpt<T>(keyID, null);
        }
        /// <summary>
        /// Generic method used to get a list of objects of type T. this methods is used when objects have
        /// a common key which is a string. this version uses a delegate to create the objects with the 
        /// data that is read from the database.
        /// </summary>
        /// <typeparam name="T">The type of object being handled</typeparam>
        /// <param name="keyID">The key which is common to all objects in the collection</param>
        /// <param name="readMethod">The delegate used to create the object T as data is read from the database</param>
        /// <returns>An ArrayList containing a objects of type T</returns>
        public ICollection LoadDataToArrayList<T>(string keyID, ReadDataDelegate<T> readMethod)
            where T : CodedReferenceValue, new()
        {
            return LoadDataToArrayListOpt<T>(keyID, readMethod);
        }
        /// <summary>
        /// A Generic method used to get an ArrayList containing objects of type T which uses a 
        /// delegate to create the objects as they are read from the reader
        /// </summary>
        /// <typeparam name="T">The type of object being handled</typeparam>
        /// <param name="keyID">The key which is common to all objects in the collection</param>
        /// <param name="readMethod">The delegate used to create the object T as data is read from the database</param>
        /// <returns>An ArrayList containing a objects of type T</returns>
        /// <exception cref="Exception"><c>Exception</c>.</exception>
        private ICollection LoadDataToArrayListOpt<T>(long? keyID, ReadDataDelegate<T> readMethod)
            where T : CodedReferenceValue, new()
        {
            ArrayList oList = new ArrayList();

            SafeReader reader = null;
            iDB2Command cmd = null;
            StringBuilder sb = new StringBuilder();

            try
            {
                sb.Append("CALL " + AllStoredProcName + "(");
                if (keyID.HasValue)
                {
                    sb.Append(PARAM_FACILITYID);
                }
                sb.Append(")");

                cmd = this.CommandFor(sb.ToString(), CommandType.Text, this.GetFacilityByIdOrElseAnyActiveFacility(keyID));

                if (keyID.HasValue)
                    cmd.Parameters[PARAM_FACILITYID].Value = keyID;

                reader = this.ExecuteReader(cmd);

                while (reader.Read())
                {
                    if (readMethod != null)
                    {
                        oList.Add(readMethod(reader));
                    }
                    else
                    {
                        if (keyID.HasValue)
                        {
                            oList.Add(this.ObjFrom<T>(reader, keyID));
                        }
                        else
                        {
                            oList.Add(this.ObjFrom<T>(reader));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                string msg = typeof(T) + " failed to initialize.";
                throw BrokerExceptionFactory.BrokerExceptionFrom(msg, e, c_log);
            }
            finally
            {
                base.Close(reader);
                base.Close(cmd);
            }
            return oList;
        }

        /// <exception cref="Exception"><c>Exception</c>.</exception>
        private ICollection LoadDataToArrayListOpt<T>(string keyID, ReadDataDelegate<T> readMethod) where T : CodedReferenceValue, new()
        {
            ArrayList oList = new ArrayList();

            SafeReader reader = null;
            iDB2Command cmd = null;
            StringBuilder sb = new StringBuilder();

            try
            {
                sb.Append("CALL " + AllStoredProcName + "(");
                if (keyID != null)
                {
                    sb.Append(PARAM_FACILITYID);
                }
                sb.Append(")");

                cmd = this.CommandFor(sb.ToString(), CommandType.Text, this.GetFacilityByIdOrElseAnyActiveFacility(keyID));

                if (keyID != null)
                    cmd.Parameters[PARAM_FACILITYID].Value = keyID;

                reader = this.ExecuteReader(cmd);

                while (reader.Read())
                {
                    if (readMethod != null)
                    {
                        oList.Add(readMethod(reader));
                    }
                    else
                    {
                        if (keyID != null)
                        {
                            oList.Add(this.ObjFrom<T>(reader, keyID));
                        }
                        else
                        {
                            oList.Add(this.ObjFrom<T>(reader));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                string msg = typeof(T) + " failed to initialize.";
                throw BrokerExceptionFactory.BrokerExceptionFrom(msg, e, c_log);
            }
            finally
            {
                base.Close(reader);
                base.Close(cmd);
            }
            return oList;
        }

        /// <summary>
        /// Generic methods for getting a list of code objects without a key. 
        /// </summary>
        /// <typeparam name="T">The type of object being handled. The object must inherit from CodedReferenceValue</typeparam>
        /// <returns>A Hashtable of objects of type T</returns>
        public ICollection LoadDataToHashtable<T>() where T : CodedReferenceValue, new()
        {
            long? parm = null;
            return LoadDataToHashtableOpt<T>(parm, null);
        }
        /// <summary>
        /// Generic method to get a list of objects of type T using a delegate to read the data from the reader
        /// </summary>
        /// <typeparam name="T">The type of object being handled. The object must inherit from ReferenceValue</typeparam>
        /// <param name="readMethod">The delegate used to create the object by reading data from the reader</param>
        /// <returns>A Hashtable of objects of type T</returns>
        public ICollection LoadDataToHashtable<T>(ReadDataDelegate<T> readMethod)
            where T : CodedReferenceValue, new()
        {
            long? parm = null;
            return LoadDataToHashtableOpt<T>(parm, readMethod);
        }
        /// <summary>
        /// Generic method used to get a list of objects based on a key such as a facilityKey
        /// </summary>
        /// <typeparam name="T">The type of object being handled. The object must inherit from ReferenceValue</typeparam>
        /// <param name="keyID">The key of the collection of objects being read. Normally a FacilityID</param>
        /// <returns>A Hashtable of objects of type T</returns>
        public ICollection LoadDataToAHashtable<T>(long keyID)
            where T : CodedReferenceValue, new()
        {
            return LoadDataToHashtableOpt<T>(keyID, null);
        }
        /// <summary>
        /// Generic method used to get a list of objects based on a key such as a facilityKey
        /// and using a delegate to read the data from the data reader
        /// </summary>
        /// <typeparam name="T">The type of object being handled</typeparam>
        /// <param name="keyID">The key of the collection of objects being read. Normally a FacilityID</param>
        /// <param name="readMethod">The delegate used to create the object T as data is read from the data reader</param>
        /// <returns>A Hashtable containing a objects of type T</returns>
        public ICollection LoadDataToHashtable<T>(long keyID, ReadDataDelegate<T> readMethod)
            where T : CodedReferenceValue, new()
        {
            return LoadDataToHashtableOpt<T>(keyID, readMethod);
        }
        /// <summary>
        /// Generic method used to get a list of objects of type T. this methods is used when objects have
        /// a common key which is a string.
        /// </summary>
        /// <typeparam name="T">The type of object being handled</typeparam>
        /// <param name="keyID">The key which is common to all objects</param>
        /// <returns>A Hashtable containing a objects of type T</returns>
        public ICollection LoadDataToHashtable<T>(string keyID)
            where T : CodedReferenceValue, new()
        {
            return LoadDataToHashtableOpt<T>(keyID, null);
        }
        /// <summary>
        /// Generic method used to get a list of objects of type T. this methods is used when objects have
        /// a common key which is a string. this version uses a delegate to create the objects with the 
        /// data that is read from the database.
        /// </summary>
        /// <typeparam name="T">The type of object being handled</typeparam>
        /// <param name="keyID">The key which is common to all objects in the collection</param>
        /// <param name="readMethod">The delegate used to create the object T as data is read from the database</param>
        /// <returns>A Hashtable containing a objects of type T</returns>
        public ICollection LoadDataToHashtable<T>(string keyID, ReadDataDelegate<T> readMethod)
            where T : CodedReferenceValue, new()
        {
            return LoadDataToHashtableOpt<T>(keyID, readMethod);
        }
        /// <summary>
        /// A Generic method used to get a Hashtable containing objects of type T which uses a 
        /// delegate to create the objects as they are read from the reader
        /// </summary>
        /// <typeparam name="T">The type of object being handled</typeparam>
        /// <param name="keyID">The key which is common to all objects in the collection</param>
        /// <param name="readMethod">The delegate used to create the object T as data is read from the database</param>
        /// <returns>A Hashtable containing a objects of type T</returns>
        /// <exception cref="Exception"><c>Exception</c>.</exception>
        private ICollection LoadDataToHashtableOpt<T>(long? keyID, ReadDataDelegate<T> readMethod)
            where T : CodedReferenceValue, new()
        {
            Hashtable oList = new Hashtable();

            SafeReader reader = null;
            iDB2Command cmd = null;
            StringBuilder sb = new StringBuilder();

            try
            {
                sb.Append("CALL " + AllStoredProcName + "(");
                if (keyID.HasValue)
                {
                    sb.Append(PARAM_FACILITYID);
                }
                sb.Append(")");

                cmd = this.CommandFor(sb.ToString(), CommandType.Text, this.GetFacilityByIdOrElseAnyActiveFacility(keyID));

                if (keyID.HasValue)
                    cmd.Parameters[PARAM_FACILITYID].Value = keyID;

                reader = this.ExecuteReader(cmd);

                while (reader.Read())
                {
                    T obj;
                    if (readMethod != null)
                    {
                        obj = readMethod(reader);
                    }
                    else
                    {
                        if (keyID.HasValue)
                        {
                            obj = this.ObjFrom<T>(reader, keyID);
                        }
                        else
                        {
                            obj = this.ObjFrom<T>(reader);
                        }
                    }
                    oList.Add(obj.Code, obj);
                }
            }
            catch (Exception e)
            {
                string msg = typeof(T) + " failed to initialize.";
                throw BrokerExceptionFactory.BrokerExceptionFrom(msg, e, c_log);
            }
            finally
            {
                base.Close(reader);
                base.Close(cmd);
            }
            return oList;
        }

        /// <exception cref="Exception"><c>Exception</c>.</exception>
        private ICollection LoadDataToHashtableOpt<T>(string keyID, ReadDataDelegate<T> readMethod) where T : CodedReferenceValue, new()
        {
            Hashtable oList = new Hashtable();

            SafeReader reader = null;
            iDB2Command cmd = null;
            StringBuilder sb = new StringBuilder();

            try
            {
                sb.Append("CALL " + AllStoredProcName + "(");
                if (keyID != null)
                {
                    sb.Append(PARAM_FACILITYID);
                }
                sb.Append(")");

                cmd = this.CommandFor(sb.ToString(), CommandType.Text, this.GetFacilityByIdOrElseAnyActiveFacility(keyID));

                if (keyID != null)
                    cmd.Parameters[PARAM_FACILITYID].Value = keyID;

                reader = this.ExecuteReader(cmd);

                while (reader.Read())
                {
                    T obj;
                    if (readMethod != null)
                    {
                        obj = readMethod(reader);
                    }
                    else
                    {
                        if (keyID != null)
                        {
                            obj = this.ObjFrom<T>(reader, keyID);
                        }
                        else
                        {
                            obj = this.ObjFrom<T>(reader);
                        }
                    }
                    oList.Add(obj.Code, obj);
                }
            }
            catch (Exception e)
            {
                string msg = typeof(T) + " failed to initialize.";
                throw BrokerExceptionFactory.BrokerExceptionFrom(msg, e, c_log);
            }
            finally
            {
                base.Close(reader);
                base.Close(cmd);
            }
            return oList;
        }


        /// <summary>
        /// Generic methods for getting a list of code objects without a key.
        /// </summary>
        /// <typeparam name="T">The type of object being handled. The object must inherit from ReferenceValue</typeparam>
        /// <returns>An ArrayList of objects of type T</returns>
        public ICollection LoadUncodedDataToArrayList<T>()
            where T : ReferenceValue, new()
        {
            return LoadUncodedDataToArrayListOpt<T>(null, null);
        }
        /// <summary>
        /// Generic method to get a list of objects of type T using a delegate to read the data from the reader
        /// </summary>
        /// <typeparam name="T">The type of object being handled. The object must inherit from ReferenceValue</typeparam>
        /// <param name="readMethod">The delegate used to create the object by reading data from the reader</param>
        /// <returns>An ArrayList of objects of type T</returns>
        public ICollection LoadUncodedDataToArrayList<T>(ReadDataDelegate<T> readMethod)
            where T : ReferenceValue, new()
        {
            return LoadUncodedDataToArrayListOpt<T>(null, readMethod);
        }
        /// <summary>
        /// Generic method used to get a list of objects based on a key such as a facilityKey
        /// </summary>
        /// <typeparam name="T">The type of the object being handled. The object must inherit from ReferenceValue</typeparam>
        /// <param name="keyID">The key of the collection of objects being read. Normally a FacilityID</param>
        /// <returns>An ArrayList of objects of type T</returns>
        public ICollection LoadUncodedDataToArrayList<T>(long keyID)
            where T : ReferenceValue, new()
        {
            return LoadUncodedDataToArrayListOpt<T>(keyID, null);
        }
        /// <summary>
        /// Generic method used to get a list of objects based on a key such as a facilityKey
        /// and using a delegate to read the data from the data reader
        /// </summary>
        /// <typeparam name="T">The type of object being handled. The object must inherit from ReferenceValue</typeparam>
        /// <param name="keyID">The key of the collection of objects being read. Normally a FacilityID</param>
        /// <param name="readMethod">The delegate used to create the object T as data is read from the data reader</param>
        /// <returns>An ArrayList containing a objects of type T</returns>
        public ICollection LoadUncodedDataToArrayList<T>(long keyID,
            ReadDataDelegate<T> readMethod)
            where T : ReferenceValue, new()
        {
            return LoadUncodedDataToArrayListOpt<T>(keyID, readMethod);
        }
        /// <summary>
        /// Generic method used to get a list of objects of type T. this methods is used when objects have
        /// a common key which is a string.
        /// </summary>
        /// <typeparam name="T">The type of object being handled. The object must inherit from ReferenceValue</typeparam>
        /// <param name="keyID">The key which is common to all objects</param>
        /// <param name="readMethod">The delegate used to create the object T as data is read from the data reader</param>
        /// <returns>An ArrayList containing a objects of type T</returns>
        /// <exception cref="Exception"><c>Exception</c>.</exception>
        private ICollection LoadUncodedDataToArrayListOpt<T>(long? keyID,
            ReadDataDelegate<T> readMethod)
            where T : ReferenceValue, new()
        {
            ArrayList oList = new ArrayList();

            SafeReader reader = null;
            iDB2Command cmd = null;
            StringBuilder sb = new StringBuilder();

            try
            {
                sb.Append("CALL " + AllStoredProcName + "(");
                if (keyID.HasValue)
                {
                    sb.Append(PARAM_FACILITYID);
                }
                sb.Append(")");

                cmd = this.CommandFor(sb.ToString(), CommandType.Text, this.GetFacilityByIdOrElseAnyActiveFacility(keyID));

                if (keyID.HasValue)
                    cmd.Parameters[PARAM_FACILITYID].Value = keyID;

                reader = this.ExecuteReader(cmd);

                while (reader.Read())
                {
                    if (readMethod != null)
                    {
                        oList.Add(readMethod(reader));
                    }
                    else
                    {
                        if (keyID.HasValue)
                        {
                            oList.Add(this.UnCodedObjFrom<T>(reader, keyID));
                        }
                        else
                        {
                            oList.Add(this.UnCodedObjFrom<T>(reader));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                string msg = typeof(T) + " failed to initialize.";
                throw BrokerExceptionFactory.BrokerExceptionFrom(msg, e, c_log);
            }
            finally
            {
                base.Close(reader);
                base.Close(cmd);
            }
            return oList;
        }

        /// <summary>
        /// Generic methods for getting a list of code objects without a key. 
        /// </summary>
        /// <typeparam name="T">The type of object being handled. The object must inherit from ReferenceValue</typeparam>
        /// <returns>A Hashtable of objects of type T</returns>
        public ICollection LoadUncodedDataToHashtable<T>()
            where T : ReferenceValue, new()
        {
            return LoadUncodedDataToHashtableOpt<T>(null, null);
        }
        /// <summary>
        /// Generic method to get a list of objects of type T using a delegate to read the data from the reader
        /// </summary>
        /// <typeparam name="T">The type of object being handled. The object must inherit from ReferenceValue</typeparam>
        /// <param name="readMethod">The delegate used to create the object by reading data from the reader</param>
        /// <returns>A Hashtable of objects of type T</returns>
        public ICollection LoadUncodedDataToHashtable<T>(ReadDataDelegate<T> readMethod)
            where T : ReferenceValue, new()
        {
            return LoadUncodedDataToHashtableOpt<T>(null, readMethod);
        }
        /// <summary>
        /// Generic method used to get a list of objects based on a key such as a facilityKey
        /// </summary>
        /// <typeparam name="T">The type of object being handled. The object must inherit from ReferenceValue</typeparam>
        /// <param name="keyID">The key of the collection of objects being read. Normally a FacilityID</param>
        /// <returns>A Hashtable of objects of type T</returns>
        public ICollection LoadUncodedDataToHashtable<T>(long keyID)
            where T : ReferenceValue, new()
        {
            return LoadUncodedDataToHashtableOpt<T>(keyID, null);
        }
        /// <summary>
        /// Generic method used to get a list of objects based on a key such as a facilityKey
        /// and using a delegate to read the data from the data reader
        /// </summary>
        /// <typeparam name="T">The type of object being handled. The object must inherit from ReferenceValue</typeparam>
        /// <param name="keyID">The key of the collection of objects being read. Normally a FacilityID</param>
        /// <param name="readMethod">The delegate used to create the object by reading data from the reader</param>
        /// <returns>A Hashtable of objects of type T</returns>
        public ICollection LoadUncodedDataToHashtable<T>(long keyID,
            ReadDataDelegate<T> readMethod)
            where T : ReferenceValue, new()
        {
            return LoadUncodedDataToHashtableOpt<T>(keyID, readMethod);
        }

        /// <exception cref="Exception"><c>Exception</c>.</exception>
        private ICollection LoadUncodedDataToHashtableOpt<T>(long? keyID,
            ReadDataDelegate<T> readMethod)
            where T : ReferenceValue, new()
        {
            Hashtable oList = new Hashtable();

            SafeReader reader = null;
            iDB2Command cmd = null;
            StringBuilder sb = new StringBuilder();

            try
            {
                sb.Append("CALL " + AllStoredProcName + "(");
                if (keyID.HasValue)
                {
                    sb.Append(PARAM_FACILITYID);
                }
                sb.Append(")");

                cmd = this.CommandFor(sb.ToString(), CommandType.Text, this.GetFacilityByIdOrElseAnyActiveFacility(keyID));

                if (keyID.HasValue)
                    cmd.Parameters[PARAM_FACILITYID].Value = keyID;

                reader = this.ExecuteReader(cmd);

                while (reader.Read())
                {
                    T obj;
                    if (readMethod != null)
                    {
                        obj = readMethod(reader);
                    }
                    else
                    {
                        if (keyID.HasValue)
                        {
                            obj = this.UnCodedObjFrom<T>(reader, keyID);
                        }
                        else
                        {
                            obj = this.UnCodedObjFrom<T>(reader);
                        }
                    }
                    oList.Add(obj.Oid, obj);
                }
            }
            catch (Exception e)
            {
                string msg = typeof(T) + " failed to initialize.";
                throw BrokerExceptionFactory.BrokerExceptionFrom(msg, e, c_log);
            }
            finally
            {
                base.Close(reader);
                base.Close(cmd);
            }
            return oList;
        }

        /// <summary>
        /// Generic methods for reading 1 object from the database where the search criteria
        ///     is an id or OID of type long
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="oid"></param>
        /// <returns></returns>
        public T CodeWith<T>(long oid) where T : CodedReferenceValue, new()
        {
            return CodeWith<T>(null, oid, null);
        }
        public T CodeWith<T>(long oid, ReadDataDelegate<T> readMethod)
            where T : CodedReferenceValue, new()
        {
            return CodeWith<T>(null, oid, readMethod);
        }
        public T CodeWith<T>(long? keyID, long oid) where T : CodedReferenceValue, new()
        {
            return CodeWith<T>(keyID, oid, null);
        }

        private T CodeWith<T>(long? keyID, long oid, ReadDataDelegate<T> readMethod) where T : CodedReferenceValue, new()
        {
            SafeReader reader = null;
            iDB2Command cmd = null;
            T gCode = null;

            try
            {
                cmd = this.createWithCmdFor(oid, keyID);
                reader = this.ExecuteReader(cmd);

                if (reader.Read())
                {
                    if (readMethod != null)
                    {
                        gCode = readMethod(reader);
                    }
                    else
                    {
                        gCode = this.ObjFrom<T>(reader);
                    }
                }
                else
                {
                    gCode = new T();
                    gCode.Oid = oid;
                }
            }
            catch (Exception e)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom(typeof(T) + "broker failed to initialize", e, c_log);
            }
            finally
            {
                base.Close(reader);
                base.Close(cmd);
            }

            return gCode;
        }

        public T CodeWith<T>(string code) where T : CodedReferenceValue, new()
        {
            return CodeWith<T>(null, code, null);
        }
        public T CodeWith<T>(string code, ReadDataDelegate<T> readMethod) where T : CodedReferenceValue, new()
        {
            return CodeWith<T>(null, code, readMethod);
        }
        public T CodeWith<T>(long? keyID, string code)
            where T : CodedReferenceValue, new()
        {
            return CodeWith<T>(keyID, code, null);
        }

        /// <exception cref="Exception"><c>Exception</c>.</exception>
        public T CodeWith<T>(long? keyID, string code, ReadDataDelegate<T> readMethod)
            where T : CodedReferenceValue, new()
        {
            iDB2Command cmd = null;
            SafeReader reader = null;
            T gCode = null;
            Type t = typeof(T);

            try
            {
                cmd = this.createWithCmdFor(code, keyID);
                reader = this.ExecuteReader(cmd);

                if (reader.Read())
                {
                    if (readMethod != null)
                    {
                        gCode = readMethod(reader);
                    }
                    else
                    {
                        gCode = this.ObjFrom<T>(reader);
                    }
                }
                else
                {
                    gCode = new T();
                    gCode.Oid = ReferenceValue.NEW_OID;
                    gCode.Code = code;
                    gCode.Description = code;
                    gCode.IsValid = false;
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                throw BrokerExceptionFactory.BrokerExceptionFrom(typeof(T) + "broker failed to initialize", e, c_log);
            }
            finally
            {
                base.Close(reader);
                base.Close(cmd);
            }

            return gCode;
        }

        // Codes without Code (internal codes

        public T RefValueWith<T>(long oid) where T : ReferenceValue, new()
        {
            return RefValueWith<T>(null, oid);
        }

        /// <exception cref="Exception"><c>Exception</c>.</exception>
        private T RefValueWith<T>(long? keyID, long oid) where T : ReferenceValue, new()
        {
            SafeReader reader = null;
            iDB2Command cmd = null;
            T gCode = null;

            try
            {
                cmd = this.createWithCmdFor(oid, keyID);
                reader = this.ExecuteReader(cmd);

                if (reader.Read())
                {
                    gCode = this.UnCodedObjFrom<T>(reader);
                }
                else
                {
                    gCode = new T();
                    gCode.Oid = oid;
                }
            }
            catch (Exception e)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom(typeof(T) + "broker failed to initialize", e, c_log);
            }
            finally
            {
                base.Close(reader);
                base.Close(cmd);
            }

            return gCode;
        }

        public T RefValueWith<T>(string code) where T : ReferenceValue, new()
        {
            return RefValueWith<T>(null, code);
        }

        /// <exception cref="Exception"><c>Exception</c>.</exception>
        private T RefValueWith<T>(long? keyID, string code) where T : ReferenceValue, new()
        {
            iDB2Command cmd = null;
            SafeReader reader = null;
            T gCode = null;
            Type t = typeof(T);

            try
            {
                cmd = this.createWithCmdFor(code, keyID);
                reader = this.ExecuteReader(cmd);

                if (reader.Read())
                {
                    gCode = this.UnCodedObjFrom<T>(reader);
                }
                else
                {
                    gCode = new T();
                    gCode.Oid = ReferenceValue.NEW_OID;
                    gCode.Description = code;
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                throw BrokerExceptionFactory.BrokerExceptionFrom(typeof(T) + "broker failed to initialize", e, c_log);
            }
            finally
            {
                base.Close(reader);
                base.Close(cmd);
            }

            return gCode;
        }

        #endregion

        #region Private Methods
        private iDB2Command createWithCmdFor(string code, long? keyID)
        {
            iDB2Command cmd = null;
            StringBuilder sb = new StringBuilder();

            sb.Append("CALL " + this.WithStoredProcName + "(");
            if (keyID.HasValue)
            {
                sb.Append(PARAM_FACILITYID + ",");
            }
            sb.Append(PARAM_CODE + ")");

            cmd = this.CommandFor(sb.ToString(), CommandType.Text, this.GetFacilityByIdOrElseAnyActiveFacility(keyID));
            if (keyID.HasValue)
            {
                cmd.Parameters[PARAM_FACILITYID].Value = keyID;
            }
            cmd.Parameters[PARAM_CODE].Value = code;
            return (cmd);
        }

        private iDB2Command createWithCmdFor(long code, long? keyID)
        {
            iDB2Command cmd = null;
            StringBuilder sb = new StringBuilder();

            sb.Append("CALL " + this.WithStoredProcName + "(");
            if (keyID.HasValue)
            {
                sb.Append(PARAM_FACILITYID + ",");
            }
            sb.Append(PARAM_CODE + ")");

            cmd = this.CommandFor(sb.ToString(), CommandType.Text, this.GetFacilityByIdOrElseAnyActiveFacility(keyID));
            if (keyID.HasValue)
            {
                cmd.Parameters[PARAM_FACILITYID].Value = keyID;
            }
            cmd.Parameters[PARAM_CODE].Value = code;
            return (cmd);
        }

        /// <summary>
        /// construct an object of type T and populate with the values read from the reader
        /// the proc is required to return the values in this order: OID, CODE, Description
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <param name="keyID"></param>
        /// <returns></returns>
        private T ObjFrom<T>(SafeReader reader, string keyID) where T : CodedReferenceValue, new()
        {
            return ObjFrom<T>(reader);
        }
        private T ObjFrom<T>(SafeReader reader, long? keyID) where T : CodedReferenceValue, new()
        {
            return ObjFrom<T>(reader);
        }
        private T ObjFrom<T>(SafeReader reader) where T : CodedReferenceValue, new()
        {
            T gobj;

            long id = reader.GetInt64(COL_OID);
            string code = reader.GetString(COL_CODE);
            string description = reader.GetString(COL_DESCRIPTION);

            gobj = new T();
            gobj.Oid = id;
            gobj.Code = code;
            gobj.Description = description;

            return gobj;
        }

        private T UnCodedObjFrom<T>(SafeReader reader, long? keyID) where T : ReferenceValue, new()
        {
            return UnCodedObjFrom<T>(reader);
        }
        private T UnCodedObjFrom<T>(SafeReader reader) where T : ReferenceValue, new()
        {
            T gobj;

            long id = reader.GetInt64(COL_OID);
            string description = reader.GetString(COL_DESCRIPTION);

            gobj = new T();
            gobj.Oid = id;
            gobj.Description = description;

            return gobj;
        }

        protected virtual void InitProcNames()
        {
            //throw new Exception("this method must be implemented in the derived class");
        }
        
        /// <exception cref="BrokerException">No Facilities are active at this time. PatientAccess can not continue.</exception>
        private Facility GetFacilityByIdOrElseAnyActiveFacility(long? keyID)
        {
            Facility activeFacility = null;

            IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();

        
        
            if (this.Facility != null)
            {
                activeFacility = this.Facility;
            }
            else if (keyID.HasValue)
            {
                activeFacility = facilityBroker.FacilityWith(keyID.Value);
            }
            else
            {
                activeFacility = this.GetAnyFacilityWherePbarIsAvailableForNonFacilitySpecificData(facilityBroker, activeFacility);
            }

            if (activeFacility == null)
            {
                throw new BrokerException("No Facilities are active at this time. PatientAccess can not continue.");
            }
            return activeFacility;
        }

        /// <exception cref="BrokerException">No Facilities are active at this time. PatientAccess can not continue.</exception>
        private Facility GetFacilityByIdOrElseAnyActiveFacility(string keyID)
        {
            Facility activeFacility = null;

            IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            
            if (this.Facility != null)
            {
                activeFacility = this.Facility;
            }
            else if (keyID != null)
            {
                activeFacility = facilityBroker.FacilityWith(keyID);
            }
            else
            {
                activeFacility = this.GetAnyFacilityWherePbarIsAvailableForNonFacilitySpecificData(facilityBroker, activeFacility);
            }
        
            if (activeFacility == null)
            {
                throw new BrokerException("No Facilities are active at this time. PatientAccess can not continue.");
            }
            return activeFacility;
        }


        private Facility GetAnyFacilityWherePbarIsAvailableForNonFacilitySpecificData(IFacilityBroker facilityBroker, Facility activeFacility)
        {
            // For Non-Facility-Specific data
            ICollection allFacilities = facilityBroker.AllFacilities();
            foreach (Facility facility in allFacilities)
            {
                if ( facilityBroker.IsDatabaseAvailableFor(facility.ConnectionSpec.ServerIP))
                {
                    activeFacility = facility;
                    break;
                }
            }
            return activeFacility;
        }

        #endregion

        #region Construction and Finalization
        public PBARCodesBroker()
            : base()
        {
            InitProcNames();
        }
        public PBARCodesBroker(string cxnString)
            : base(cxnString)
        {
            InitProcNames();
        }
        public PBARCodesBroker(IDbTransaction txn)
            : base(txn)
        {
            InitProcNames();
        }
        #endregion

        #region Data Elements

        private static readonly ILog c_log =
            LogManager.GetLogger(typeof(PBARCodesBroker));

        #endregion

        #region Constants

        private const string
            COL_DESCRIPTION = "DESCRIPTION",
            COL_OID = "OID",
            COL_CODE = "CODE";

        private const string
            PARAM_CODE = "@P_CODE";

        public const string
            PARAM_FACILITYID = "@P_FACILITYID",
            PARAM_DESCRIPTION = "p_Description",
            PARAM_SITECODE = "p_siteCode",
            PARAM_ACTION = "p_Action",
            PARAM_LAST_UPDATED = "p_LastUpdatedDate",
            PARAM_FACILITYCODE = "@P_FACILITYCODE";

        protected const string
            TABLE_SPECIALITIES = "Specialties",
            TABLE_COUNTY = "County",
            TABLE_STATE = "State",
            TABLE_COUNTRY = "Country",
            TABLE_MARITALSTATUS = "Marital Status",
            TABLE_SEX = "Sex",
            TABLE_LANGUAGES = "Languages",
            TABLE_EMPLOYMENT_STATUS = "Employment Status",
            TABLE_FINANCIALCLASSES = "Financial Classes",
            TABLE_PLANTYPE = "Plan Type",
            TABLE_OCCURRENCECODE = "Occurance Code",
            TABLE_RACE = "Race",
            TABLE_ETHNICITY = "Ethnicity",
            TABLE_RELATIONSHIP = "Relationship",
            TABLE_RELIGION = "Religion",
            TABLE_MODE_OF_ARRIVAL = "Mode of Arrival",
            TABLE_NPP_VERSIONS = "NPP Versions",
            TABLE_ADMITSOURCE = "AdmitSource",
            TABLE_RELIGIOUS_CONGREGATIONS
                                    = "Religious Congregations", 
            TABLE_FACILITY_FLAGS = "FacilityFlags",
            TABLE_REFERRAL_FACILITY = "ReferralFacility",
            TABLE_REFERRALTYPE = "ReferralTypes",
            TABLE_RE_ADMIT = "ReAdmit",
            TABLE_REFERRAL_SOURCE = "Referral_Sources",
            TABLE_SCHEDULE_CODE = "Schedule_Codes",
            TABLE_CONFIDENTIAL_CODES = "ConfidentialCodes",
            TABLE_CONDITION_CODES = "ConditionCodes",
            TABLE_DISCHARGEDISPOSITION_CODE = "DischargeDispositionCodes",
            TABLE_PHYSICIAN_STATUS = "PhysicianStatuses",
            TABLE_HOSPITAL_SERVICES = "Services";

        protected const int UNIQUE_CONSTRAINT_VIOLATION_ERROR_NO = 1;

        public const string DELETED_CONST = "D";
        #endregion
    }
}

