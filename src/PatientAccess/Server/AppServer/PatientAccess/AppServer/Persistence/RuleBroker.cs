using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using Extensions.Persistence;
using Extensions.UI.Builder;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using log4net;

namespace PatientAccess.Persistence
{
    /// <summary>
    /// Sql broker for all things rule-related
    /// </summary>
    [Serializable]
    public class RuleBroker: CachingBroker, IRuleBroker
    {
		#region Constants 

        private const string DBCOLUMN_ID = "Id";
        private const string DBCOLUMN_TYPE = "Type";
        private const string DBCOLUMN_ACTIONID = "ActionId";
        private const string DBCOLUMN_COMPOSITEACTIONID = "CompositeActionId";
        private const string DBCOLUMN_COMPOSITERULEID = "CompositeRuleId";
        private const string DBCOLUMN_COMPRULETYPE = "CompositeRuleType";
        private const string DBCOLUMN_DESCRIPTION = "Description";
        private const string DBCOLUMN_ISCOMPOSITE = "IsComposite";
        private const string DBCOLUMN_RULECONTEXTID = "RuleContextId";
        private const string DBCOLUMN_RULEID = "RuleId";
        private const string DBCOLUMN_SEVERITY = "Severity";
        private const string DBCOLUMN_WORKLISTID = "WorklistId";
        private const string DBCOLUMN_WORKLISTNAME = "WorklistName";
        private const string DBPARAM_RULECONTEXT = "@RuleContext";
        private const string DBPROCEDURE_ACTIONTOWORKLISTMAPPING = "RuleEngine.SelectActionsWorklistMappings";
        private const string DBPROCEDURE_ALLACTIONS = "RuleEngine.SelectAllActions";
        private const string DBPROCEDURE_ALLRULECONTEXTS = "RuleEngine.SelectAllRuleContexts";
        private const string DBPROCEDURE_ALLRULES = "RuleEngine.SelectAllRules";
        private const string DBPROCEDURE_ALLRULESBYID = "RuleEngine.SelectAllRulesById";
        private const string DBPROCEDURE_RULEACTIONMAPPING = "RuleEngine.SelectRuleActionMappings";
        private const string DBPROCEDURE_RULESFORRULECONTEXT = "RuleEngine.SelectRulesForRuleContext";
        private const string DBPROCEDURE_RULETOWORKLISTMAPPING = "Worklist.SelectRuleWorklistMappings";

		#endregion Constants 

		#region Fields 

        private static readonly ILog _logger =
            LogManager.GetLogger( typeof( RuleBroker ) );

		#endregion Fields 

		#region Constructors 

        public  RuleBroker(string cxn)
            : base(cxn)
        {
        }


        public RuleBroker(SqlTransaction txn)
            : base(txn)
        {
        }


        public RuleBroker()            
        {
        }

		#endregion Constructors 

		#region Methods 

        /// <summary>
        /// ActionWorklistMapping - map the actionss to a worklist
        /// </summary>
        /// <returns></returns>
        public RuleHashtable ActionWorklistMapping( string kindOfVisitCode, string financialClassCode )
        {
            RuleHashtable mappings ;
            SafeReader reader = null;
            SqlCommand sqlCommand = null;

            try
            {
                mappings = new RuleHashtable();

                sqlCommand = this.CommandFor( DBPROCEDURE_ACTIONTOWORKLISTMAPPING );

                IWorklistSettingsBroker worklistBroker = 
                    BrokerFactory.BrokerOfType<IWorklistSettingsBroker>();

                ArrayList worklists = 
                    worklistBroker.GetAllWorkLists();
               
                reader = this.ExecuteReader(sqlCommand);

                while( reader.Read() )
                {
                    long worklistId = reader.GetInt32(DBCOLUMN_WORKLISTID);
                    long actionId = reader.GetInt32(DBCOLUMN_ACTIONID);
                    string worklistName = reader.GetString(DBCOLUMN_WORKLISTNAME);

                    if(worklistId == 0)
                    {
                        worklistId = InferWorklistIdUsing( kindOfVisitCode, financialClassCode );

                        Worklist worklist = worklists[ (int)worklistId - 1 ] as Worklist;            

                        if( worklist != null )
                        {
                            worklistName = worklist.Description;
                        }
                        else
                        {
                            worklistName = string.Empty;
                        }                        
                    }

                    RuleArrayList worklistArray;
                    if( mappings.ContainsKey( actionId ) )
                    {
                        worklistArray = (RuleArrayList)mappings[actionId];
                    }
                    else
                    {
                        worklistArray = new RuleArrayList();
                        mappings.Add(actionId, worklistArray);
                    }

                    worklistArray.Add(worklistId);
                    worklistArray.Add(worklistName);

                }
            }
            catch( Exception anyException )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( anyException, _logger );
            }
            finally
            {
                base.Close(reader);
                base.Close(sqlCommand);
            }

            return mappings;
        }


        /// <summary>
        /// AllActions - build a RuleHashtable (and cache it) of all actions 
        /// </summary>
        /// <returns></returns>
        public RuleHashtable AllActions()
        {
            RuleHashtable allActions = null;
            string key = CacheKeys.CACHE_ALLACTIONS;

            LoadCacheDelegate loadData = delegate()
            {
                SafeReader reader = null;
                SqlCommand cmd = null;

                try
                {
                    allActions = new RuleHashtable();

                    cmd = this.CommandFor(DBPROCEDURE_ALLACTIONS );

                    reader = this.ExecuteReader(cmd);

                    while( reader.Read() )
                    {                               
                        long actionId  = reader.GetInt32(DBCOLUMN_ACTIONID);
                        string actionType = reader.GetString(DBCOLUMN_TYPE);
                        string actionName = reader.GetString(DBCOLUMN_DESCRIPTION);
                        long compositeActionId = reader.GetInt32(DBCOLUMN_COMPOSITEACTIONID);
                        bool isComposite = Convert.ToBoolean(reader.GetValue( DBCOLUMN_ISCOMPOSITE ));

                        string type = string.Empty;

                        if( actionType != null )
                        {
                            type = actionType.Trim();
                        }
                        
                        object[] parms = new object[1];
                        parms[0] = null;
                        
                        if(type != string.Empty)
                        {
                            type += @",PatientAccess.Common";

                            Type t = null;

                            if( type != string.Empty )
                            {
                                t = Type.GetType(type);
                            }
                            
                            if( t != null )
                            {
                                if( isComposite )
                                {
                                    CompositeAction cAction = Activator.CreateInstance(t, parms) as CompositeAction;

                                    cAction.Oid             = actionId;
                                    cAction.Description     = actionName;
                                    cAction.IsComposite     = true;
                                    cAction.CompositeActionID = compositeActionId;

                                    if( !allActions.Contains( actionId ) )
                                    {
                                        allActions.Add(actionId, cAction);
                                    }
                                }
                                else
                                {
                                    LeafAction lAction = Activator.CreateInstance(t, parms) as LeafAction;

                                    lAction.Oid             = actionId;
                                    lAction.Description     = actionName;
                                    lAction.IsComposite     = false;
                                    lAction.CompositeActionID = compositeActionId;

                                    if( !allActions.Contains( actionId ) )
                                    {
                                        allActions.Add(actionId, lAction);
                                    }
                                }                                    
                            }
                        }                                                                
                    }
                    
                    this.Cache.Insert( CacheKeys.CACHE_ALLACTIONS, allActions );
                }
                catch( Exception exception )
                {
                    throw BrokerExceptionFactory.BrokerExceptionFrom( exception, _logger );
                }
                finally
                {
                    base.Close(reader);
                    base.Close(cmd);
                }
                return allActions;
            };
            try
            {
                CacheManager cacheManager = new CacheManager();
                allActions = (RuleHashtable)cacheManager.GetCollectionBy(key, loadData);
            }
            catch (Exception exception)
            {   
                throw BrokerExceptionFactory.BrokerExceptionFrom( exception, _logger);
            }
            return allActions;
        }


        /// <summary>
        /// AllRules - build a RuleHashtable (and cache it) of all actions 
        /// </summary>
        /// <returns></returns>
        public RuleHashtable AllRules()
        {
            RuleHashtable allRules = null;
            string key = CacheKeys.CACHE_ALLRULES;
            
            LoadCacheDelegate loadData = delegate
            {
                _logger.Debug( "Reading All Rules from Database" );
    
                SafeReader reader = null;
                SqlCommand cmd = null;

                try
                {
                    allRules = new RuleHashtable();

                    cmd = this.CommandFor(DBPROCEDURE_ALLRULES );
       
                    reader = this.ExecuteReader(cmd);

                    while( reader.Read() )
                    {                               
                        long    ruleId          = reader.GetInt32(DBCOLUMN_RULEID);
                        string  ruleType        = reader.GetString(DBCOLUMN_TYPE);
                        string  ruleName        = reader.GetString(DBCOLUMN_DESCRIPTION);
                        long    compositeRuleId = reader.GetInt32(DBCOLUMN_COMPOSITERULEID);    
                        bool    isComposite     = Convert.ToBoolean(reader.GetValue( DBCOLUMN_ISCOMPOSITE ));
                        int     severity        = reader.GetInt32(DBCOLUMN_SEVERITY);

                        string type = string.Empty;                                

                        if( ruleType != null )
                        {
                            type = ruleType.Trim();
                        }                               
                        
                        if(type != string.Empty)
                        {
                            type = type + @",PatientAccess.Common";

                            Type t = null;
                            
                            if( type != string.Empty )
                            {
                                t = Type.GetType(type);
                            }

                            if( t != null )
                            {
                                if( isComposite )
                                {
                                    CompositeRule cRule = Activator.CreateInstance(t) as CompositeRule;
                                    
                                    if( cRule != null )
                                    {
                                        cRule.IsComposite   = true;
                                        cRule.Severity      = severity;
                                        cRule.Oid           = ruleId;
                                        cRule.Description   = ruleName;

                                        if( !allRules.Contains( cRule.GetType() ) )
                                        {
                                            allRules.Add(cRule.GetType(), cRule);
                                        }
                                    }
                                }
                                else
                                {
                                    LeafRule lRule = Activator.CreateInstance(t) as LeafRule;

                                    if( lRule != null )
                                    {
                                        lRule.Oid               = ruleId;
                                        lRule.IsComposite       = false;
                                        lRule.Severity          = severity;
                                        lRule.Description       = ruleName;
                                        lRule.CompositeRuleID   = compositeRuleId;

                                        if( !allRules.Contains( lRule.GetType() ) )
                                        {
                                            allRules.Add(lRule.GetType(), lRule);
                                        }
                                    }
                                }                                    
                            }
                        }                                                                
                    }
                }
                catch( Exception ex )
                {
                    throw BrokerExceptionFactory.BrokerExceptionFrom( "Unexpected Exception", ex, _logger );
                }
                finally
                {
                    base.Close(reader);
                    base.Close(cmd);
                 }
                return allRules;
            };
            try
            {
                CacheManager cacheManager = new CacheManager();
                allRules = (RuleHashtable)cacheManager.GetCollectionBy(key, loadData);
            }
            catch (Exception e)
            {   
                throw BrokerExceptionFactory.BrokerExceptionFrom("AdmitSourceBroker failed to initialize", e, _logger);
            }
            return allRules;
        }


        /// <summary>
        /// AllRules - build a RuleHashtable (and cache it) of all actions 
        /// </summary>
        /// <returns></returns>
        public RuleHashtable AllRulesById()
        {
            RuleHashtable allRulesById = new RuleHashtable();
            string key = CacheKeys.CACHE_ALLRULESBYID;

            LoadCacheDelegate loadData = delegate()
            {
                SafeReader reader = null;
                SqlCommand cmd = null;

                try
                {
                    cmd = this.CommandFor(DBPROCEDURE_ALLRULESBYID );
              
                    reader = this.ExecuteReader(cmd);


                    while( reader.Read() )
                    {                               
                        long    ruleId          = reader.GetInt32(DBCOLUMN_RULEID);
                        string  ruleType        = reader.GetString(DBCOLUMN_TYPE);
                        string  ruleName        = reader.GetString(DBCOLUMN_DESCRIPTION);
                        long    compositeRuleId = reader.GetInt32(DBCOLUMN_COMPOSITERULEID);    
                        bool    isComposite     = Convert.ToBoolean(reader.GetValue( DBCOLUMN_ISCOMPOSITE ));
                        int     severity        = reader.GetInt32(DBCOLUMN_SEVERITY);

                        string type = string.Empty;                                

                        if( ruleType != null )
                        {
                            type = ruleType.Trim();
                        }                               
                        
                        if(type != string.Empty)
                        {
                            type = type + @",PatientAccess.Common";

                            Type t = null;
                            
                            if( type != string.Empty )
                            {
                                t = Type.GetType(type);
                            }

                            if( t != null )
                            {
                                if( isComposite )
                                {
                                    CompositeRule cRule = Activator.CreateInstance(t) as CompositeRule;
                                    
                                    if( cRule != null )
                                    {
                                        cRule.IsComposite   = true;
                                        cRule.Severity      = severity;
                                        cRule.Oid           = ruleId;
                                        cRule.Description   = ruleName;

                                        if( !allRulesById.Contains ( cRule.Oid ) )
                                        {
                                            allRulesById.Add(cRule.Oid, cRule);
                                        }
                                    }
                                }
                                else
                                {
                                    LeafRule lRule = Activator.CreateInstance(t) as LeafRule;

                                    if( lRule != null )
                                    {
                                        lRule.Oid               = ruleId;
                                        lRule.IsComposite       = false;
                                        lRule.Severity          = severity;
                                        lRule.Description       = ruleName;
                                        lRule.CompositeRuleID   = compositeRuleId;

                                        if( !allRulesById.Contains ( lRule.Oid ) )
                                        {
                                            allRulesById.Add(lRule.Oid, lRule);
                                        }
                                    }
                                }                                    
                            }
                        }                                                                
                    }
                    
                    this.Cache.Insert( CacheKeys.CACHE_ALLRULESBYID, allRulesById );
            
                }
                catch( Exception exception )
                {
                    throw BrokerExceptionFactory.BrokerExceptionFrom( exception, _logger );
                }
                finally
                {
                    base.Close(reader);
                    base.Close(cmd);
                }
                return allRulesById;
            };
            try
            {
                CacheManager cacheManager = new CacheManager();
                allRulesById = (RuleHashtable)cacheManager.GetCollectionBy(key, loadData);
            }
            catch (Exception e)
            {   
                throw BrokerExceptionFactory.BrokerExceptionFrom("AdmitSourceBroker failed to initialize", e, _logger);
            }
            return allRulesById;
        }


        /// <summary>
		/// Asynchronously loads Rules
		/// </summary>
		public Hashtable GetRulesForCaching()
		{
			Hashtable rulesForCaching = new Hashtable();

			foreach( string aType in this.AllRuleContexts()  )
			{                
				rulesForCaching.Add( aType, this.LoadRules( aType ) );
			}       
     
			return rulesForCaching;

		}


        /// <summary>
        /// LoadRules - load rules for the specified type.  Type will most often correspond to a particular
        /// class; however, to pull 'all the worklist rules' the method can be invoked with a more generic
        /// 'Worklist%' type.
        /// </summary>
        /// <param name="aType"></param>
        /// <returns></returns>
        public RuleArrayList LoadRules(string aType)
        {                                             
            RuleArrayList rules = null;

            string key = CacheKeys.CACHE_ACTIVITIESLOADED;

            LoadCacheDelegate loadData = delegate
            {
                SafeReader reader = null;
                SqlCommand cmd = null;

                try
                {
                    // preload all of the rules
                    RuleHashtable theRules = this.AllRules();
                    rules = new RuleArrayList();

				    cmd = this.CommandFor(DBPROCEDURE_RULESFORRULECONTEXT);

				    SqlParameter contextParam = cmd.Parameters.Add(
					    new SqlParameter( DBPARAM_RULECONTEXT, 
					    SqlDbType.VarChar ) 
					    );
				    contextParam.Value = ( aType );
            
				    reader = this.ExecuteReader(cmd);

				    _logger.InfoFormat("Loading rules for {0}", aType);

				    while( reader.Read() && !reader.IsClosed )
				    {                    
					    long    ruleId          = reader.GetInt32(DBCOLUMN_RULEID);
					    string  ruleType        = reader.GetString(DBCOLUMN_TYPE);
					    string  ruleName        = reader.GetString(DBCOLUMN_DESCRIPTION);
					    long    compositeRuleId = reader.GetInt32(DBCOLUMN_COMPOSITERULEID);                        
					    bool    isComposite     = Convert.ToBoolean(reader.GetValue( DBCOLUMN_ISCOMPOSITE ));
					    string  compositeType   = reader.GetString(DBCOLUMN_COMPRULETYPE);
					    int     severity        = reader.GetInt32(DBCOLUMN_SEVERITY);
					    int     ruleContextId   = reader.GetInt32(DBCOLUMN_RULECONTEXTID);

					    _logger.InfoFormat("  RuleID: {0} RuleType: {1} CompositeRuleID: {2} CompositeType: {3} " +
						    "IsComposite: {4} Severity: {5} RuleContextID: {6}", ruleId, ruleType, compositeRuleId, compositeType, isComposite, severity, ruleContextId);

					    string strType = ruleType + @", PatientAccess.Common";
                
					    Type  t = null;

					    if( strType != string.Empty )
					    {
						    t = Type.GetType(strType);
					    }                    

					    if( t != null )
					    {                        
						    if( isComposite )
						    {
							    CompositeRule cRule = (CompositeRule)theRules[t];
							    cRule.RuleContextID = ruleContextId;
							    cRule.Severity      = severity;
							    cRule.Oid           = ruleId;
							    cRule.Description   = ruleName;

    						    rules.Add(cRule);
						    }
						    else
						    {
							    LeafRule aRule = Activator.CreateInstance(t) as LeafRule;

							    long compRuleID = compositeRuleId;
    
							    aRule.Oid               = ruleId;
							    aRule.IsComposite       = false;
							    aRule.Severity          = severity;
							    aRule.Description       = ruleName;
							    aRule.CompositeRuleID   = compositeRuleId;

							    rules.Add(aRule);
						    }                        
					    }
				    }
                }
                catch( Exception ex )
                {
                    throw BrokerExceptionFactory.BrokerExceptionFrom( "Unexpected Exception", ex, _logger );
                }
                finally
                {
                    base.Close(reader);
                    base.Close(cmd);
                }

                return rules;
            };
            try
            {
                CacheManager cacheManager = new CacheManager();
                rules = (RuleArrayList)cacheManager.GetCollectionBy(key, aType, loadData);
            }
            catch (Exception e)
            {   
                throw BrokerExceptionFactory.BrokerExceptionFrom("RuleBroker(loadRules) failed to initialize", e, _logger);
            }

            return rules;
        }


        /// <summary>
        /// RuleActionMapping - map the rules to an action
        /// </summary>
        /// <returns></returns>
        public RuleHashtable RuleActionMapping()
        {
            RuleHashtable mappings = null;
            string key = CacheKeys.CACHE_RULEACTIONS;

            LoadCacheDelegate loadData = delegate()
            {            
                SafeReader reader = null;
                SqlCommand cmd = null;

                try
                {
                    mappings = new RuleHashtable();

                    cmd = this.CommandFor(DBPROCEDURE_RULEACTIONMAPPING );

                    reader = this.ExecuteReader(cmd);

                    while( reader.Read() )
                    {
                        long ruleId             = reader.GetInt32(DBCOLUMN_RULEID);
                        long actionId           = reader.GetInt32(DBCOLUMN_ACTIONID);
                        string actionType       = reader.GetString(DBCOLUMN_TYPE);
                        string actionName       = reader.GetString(DBCOLUMN_DESCRIPTION);

                        long compositeActionId  = reader.GetInt32(DBCOLUMN_COMPOSITEACTIONID);
                        bool    isComposite     =  Convert.ToBoolean(reader.GetValue( DBCOLUMN_ISCOMPOSITE ));

                        RuleToActionMapping mapping = new RuleToActionMapping(
                            ruleId, actionId, actionType, actionName, compositeActionId,
                            isComposite);

                        if( !mappings.Contains( ruleId ) )
                        {
                            mappings.Add(ruleId, mapping);
                        }
                    }

                    this.Cache.Insert( CacheKeys.CACHE_RULEACTIONS, mappings );
                }
                catch( Exception ex )
                {
                    throw BrokerExceptionFactory.BrokerExceptionFrom( "Unexpected Exception", ex, _logger );
                }
                finally
                {
                    base.Close(reader);
                    base.Close(cmd);
                }
                return mappings;
            };
            try
            {
                CacheManager cacheManager = new CacheManager();
                mappings = (RuleHashtable)cacheManager.GetCollectionBy(key, loadData);
            }
            catch (Exception e)
            {   
                throw BrokerExceptionFactory.BrokerExceptionFrom("RuleBroker(actionMappings) failed to initialize", e, _logger);
            }

          return mappings;
        }


        /// <summary>
        /// RuleWorklistMapping - map the rules to a worklist
        /// </summary>
        /// <returns></returns>
        public RuleHashtable RuleWorklistMapping()
        {
            RuleHashtable mappings = null;
            string key = CacheKeys.CACHE_RULEWORKLIST;

            LoadCacheDelegate LoadData = delegate()
            {                        
                
                SafeReader reader = null;
                SqlCommand cmd = null;

                try
                {
                    mappings = new RuleHashtable();

                    cmd = this.CommandFor(DBPROCEDURE_RULETOWORKLISTMAPPING );
       
                    reader = this.ExecuteReader(cmd);

                    while( reader.Read() )
                    {
                        long ruleId         = reader.GetInt32(DBCOLUMN_RULEID);
                        long worklistID     = reader.GetInt32(DBCOLUMN_WORKLISTID);

                        RuleArrayList rules = (RuleArrayList)mappings[ruleId];
                        if ( rules == null )
                        {
                            rules = new RuleArrayList();
                            mappings.Add(ruleId,rules);
                        }
                        rules.Add(worklistID);
                    }
                }
                catch( Exception ex )
                {
                    throw BrokerExceptionFactory.BrokerExceptionFrom( "Unexpected Exception", ex, _logger );
                }
                finally
                {
                    base.Close(reader);
                    base.Close(cmd);
                }
                return mappings;
            };
            try
            {
                CacheManager cacheManager = new CacheManager();
                mappings = (RuleHashtable)cacheManager.GetCollectionBy(key, LoadData);
            }
            catch (Exception e)
            {   
                throw BrokerExceptionFactory.BrokerExceptionFrom("RuleBroker(ruleMapping) failed to initialize", e, _logger);
            }
            return mappings;
        }
        private ArrayList AllRuleContexts()
        {
            ArrayList contexts = new ArrayList();

            SafeReader reader = null;
            SqlCommand cmd = null;

            try
            {
                cmd = this.CommandFor( DBPROCEDURE_ALLRULECONTEXTS );
                              
                reader = this.ExecuteReader(cmd);

                while( reader.Read() )
                {                               
                    string  ruleContext        = reader.GetString(DBCOLUMN_TYPE);
                    contexts.Add(ruleContext);
                }
            }
            catch( Exception ex )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( "Unexpected Exception reading rule contexts", ex, _logger );
            }
            finally
            {
                base.Close(reader);
                base.Close(cmd);
            }

            return contexts;
        }


        /// <summary>
        /// Infers the worklist id
        /// </summary>
        /// <param name="kindOfVisitCode">The kind of visit code.</param>
        /// <param name="financialClassCode">The financial class code.</param>
        /// <returns></returns>
        private static long InferWorklistIdUsing( string kindOfVisitCode, string financialClassCode )
        {
            long worklistId;
            
            if( ( kindOfVisitCode == VisitType.EMERGENCY_PATIENT || kindOfVisitCode == VisitType.OUTPATIENT ) 
                    &&
                financialClassCode == FinancialClass.MED_SCREEN_EXM_CODE )
            {
                worklistId = Worklist.EMERGENCYDEPARMENTWORKLISTID;
            }
            else if( kindOfVisitCode == VisitType.PREREG_PATIENT )
            {
                worklistId = Worklist.PREREGWORKLISTID;
            }
            else
            {
                worklistId = Worklist.POSTREGWORKLISTID;
            }

            return worklistId;
        }

		#endregion Methods 
    }
}
