using System;
using System.Collections;
using PatientAccess.BrokerInterfaces;

namespace PatientAccess.BrokerProxies
{
    [Serializable]
    public class RuleBrokerProxy : AbstractBrokerProxy, IRuleBroker
    {		

        #region Event Handlers
        #endregion

        #region Methods

        public RuleHashtable AllRulesById()
        {
            return i_remoteRuleBroker.AllRulesById();
        }

        public Hashtable GetRulesForCaching()
		{
			return this.i_remoteRuleBroker.GetRulesForCaching();
		}

		// Cache the entire collection of rules by context

		public void PreCacheRules()
		{
			if( this.activityRulesHashtable.Count <= 0 )
			{
				lock( ACTIVITY_RULES_HASHTABLE )
				{
					this.Cache[ ACTIVITY_RULES_HASHTABLE ] = this.GetRulesForCaching();
				}
			}			
		}

		public RuleArrayList LoadRules( string aType )
		{			
			RuleArrayList activityRules = (RuleArrayList)this.activityRulesHashtable[ aType ];

			if( activityRules == null )
			{
				lock( ACTIVITY_RULES_HASHTABLE )
				{
					// Ensure that we are still null before loading from the real broker
					if( this.activityRulesHashtable[ aType ] == null )
					{
						activityRules = i_remoteRuleBroker.LoadRules( aType );
						activityRulesHashtable.Add( aType, activityRules );                        
					}
					else
					{
						// The rules were loaded on another thread before our double-check
						activityRules = (RuleArrayList)this.activityRulesHashtable[ aType ];
					}
				}
			}

			return activityRules;
		}
             
        public RuleHashtable RuleActionMapping()
        {
            return this.ruleActionMapping;
        }

        public RuleHashtable RuleWorklistMapping()
        {
            return this.ruleWorklistMapping;
        }

        public RuleHashtable ActionWorklistMapping( string kindOfVisitCode, string financialClassCode )
        {
            return this.i_remoteRuleBroker.ActionWorklistMapping( kindOfVisitCode, financialClassCode );
        }
        
        public RuleHashtable AllActions()
        {
            return this.allActions;
        }

        public RuleHashtable AllRules()
        {
            return this.i_remoteRuleBroker.AllRules();
        }

        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties

        // activityRulesHashtable is a hashtable Keyed by an activity type - e.g. RegistrationActivity
        // or PreRegistrationActivity... it holds the collection of rules for an activity as a RuleArrayList
        // as its Value.
            
        private Hashtable activityRulesHashtable
        {
            get
            {
                Hashtable ht = (Hashtable)this.Cache[ACTIVITY_RULES_HASHTABLE]; 
                if (ht == null)
                {
                    lock( ACTIVITY_RULES_HASHTABLE )
                    {
                        ht = new Hashtable();
                        if( this.Cache[ACTIVITY_RULES_HASHTABLE] == null )
                        {
                            this.Cache.Insert( ACTIVITY_RULES_HASHTABLE, ht );
                        }
                    }
                }
           
                return ht;
            }
        }

		private RuleHashtable allActions
		{
			get
			{
                RuleHashtable ht = (RuleHashtable)this.Cache[ALL_ACTIONS];
				if( ht == null )
				{
					lock( ALL_ACTIONS )
					{
                        ht = i_remoteRuleBroker.AllActions();
						if( this.Cache[ALL_ACTIONS] == null )
						{
							this.Cache.Insert( ALL_ACTIONS, ht );
						}
					}
				}

				return ht;
			}
		}

		private RuleHashtable ruleActionMapping
		{
			get
			{
				RuleHashtable ht = (RuleHashtable)this.Cache[RULE_ACTION_MAPPING];

				if( ht == null )
				{
					lock( RULE_ACTION_MAPPING )
					{
                        ht = i_remoteRuleBroker.RuleActionMapping();
						if( this.Cache[RULE_ACTION_MAPPING] == null )
						{
							this.Cache.Insert( RULE_ACTION_MAPPING, ht );
						}
					}
				}

				return ht;
			}
		}

		private RuleHashtable ruleWorklistMapping
		{
			get
			{
                RuleHashtable ht = (RuleHashtable)this.Cache[RULE_WORKLIST_MAPPING];
				if( ht == null )
				{
					lock( RULE_WORKLIST_MAPPING )
					{
                        ht = i_remoteRuleBroker.RuleWorklistMapping();
                        if( this.Cache[RULE_WORKLIST_MAPPING] == null )
						{
							this.Cache.Insert( RULE_WORKLIST_MAPPING, ht );
						}
					}
				}

				return ht;
			}
		}

        #endregion

        #region Construction and Finalization

        public RuleBrokerProxy()
        {
        }

        #endregion

        #region Data Elements
            
            private IRuleBroker i_remoteRuleBroker = BrokerFactory.BrokerOfType< IRuleBroker >() ;                    
    
        #endregion

        #region Constants

        private const string             
            ALL_RULES_HASHTABLE = "ALL_RULES_HASHTABLE",
            ACTIVITY_RULES_HASHTABLE = "ACTIVITY_RULES_HASHTABLE",
			ALL_ACTIONS = "ALL_ACTIONS",
			RULE_WORKLIST_MAPPING = "RULE_WORKLIST_MAPPING",
			RULE_ACTION_MAPPING = "RULE_ACTION_MAPPING";

        #endregion
    }
}
