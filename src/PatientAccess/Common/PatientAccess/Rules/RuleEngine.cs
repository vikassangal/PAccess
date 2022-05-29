using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Extensions.UI.Builder;
using PatientAccess.Actions;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Annotations;
using PatientAccess.Domain;
using PatientAccess.Domain.PAIWalkinOutpatientCreation;
using PatientAccess.Domain.Parties;
using PatientAccess.Domain.QuickAccountCreation;
using PatientAccess.Domain.ShortRegistration;
using log4net;
using PatientAccess.Domain.UCCRegistration;

namespace PatientAccess.Rules
{
    public interface IRuleEngine
    {
        bool EvaluateRule<T>( object context );
        bool EvaluateRule( Type aRuleType, object context );
        bool EvaluateRule<T>( object context, object control );
        bool EvaluateRule( Type aRuleType, object context, object control );
        bool EvaluateRules( Account anAccount );
        bool EvaluateAllRules( Account anAccount );
        void LoadRules( string inContext );
        void LoadRules( Account anAccount );
        void UnloadHandlers();
        void RegisterEvent<T>( EventHandler eventHandler );
        void RegisterEvent<T>( object context, EventHandler eventHandler );
        void RegisterEvent<T, R>( object context, EventHandler eventHandler );
        void RegisterEvent<T>( object context, object control, EventHandler eventHandler );
        void RegisterEvent( Type ruleType, EventHandler eventHandler );
        void RegisterEvent( Type ruleType, object context, EventHandler eventHandler );
        void RegisterEvent( Type ruleType, EventHandler eventHandler, Type compRuleType );
        void RegisterEvent( Type ruleType, object context, EventHandler eventHandler, Type compRuleType );
        void RegisterEvent( Type ruleType, object context, object control, EventHandler eventHandler );
        void RegisterEvent( Type ruleType, object context, object control, EventHandler eventHandler, Type compRuleType );
        void RegisterGlobalEvent( string ruleType, EventHandler eventHandler );
        void UnregisterEvent<T>( EventHandler eventHandler );
        void UnregisterEvent<T>( object context, EventHandler eventHandler );
        void UnregisterEvent( Type ruleType, EventHandler eventHandler );
        void UnregisterEvent( Type ruleType, object context, EventHandler eventHandler );
        void UnregisterGlobalEvent( string ruleType, EventHandler eventHandler );
        bool AccountHasFailedError();
        void AddGenericError( string fieldName );
        void RemoveGenericError( string fieldName );
        void ClearActions();
        ArrayList GetFailedActions();
        IAction GetMappedAction( long oid );
        void ClearActionsForRule( Type aRuleType );
        ArrayList GetWorklistActionItems( Account anAccount );
        string GetInvalidCodeFieldSummary();
        string GetRemainingErrorsSummary();
        string GetRequiredFieldSummary();
        ICollection<CompositeAction> GetCompositeItemsCollection();
        string GetCompositeSummary( Account anAccount, Coverage aCoverage, string composite );
        bool OneShotRuleEvaluation<T>( object context, EventHandler eventHandler );
    }

    [Serializable]
    [UsedImplicitly]
    public class RuleEngine : IRuleEngine
    {
        #region Constants

        public const string ALL_RULES_PASSED = "AllRulesPassedEventHandler";
        private const short COMPOSITE = 2;
        private const short CONTEXT = 1;
        private const short CONTROL = 3;
        private const string INDENT = "          ";
        private const long NON_STAFF_PHYSICIAN_NUMBER = 8888;
        public const string NOT_ALL_RULES_PASSED = "NotAllRulesPassedEventHandler";
        private const short RULETYPE = 0;
        private const int SEVERITY_DEPRICATED = 16;
        private const int SEVERITY_DEPRICATED_CHANGE = 32;
        private const int SEVERITY_ERROR = 8;
        public const int SEVERITY_INFO = 1;
        public const int SEVERITY_PREFERRED = 2;
        private const int SEVERITY_REQUIRED = 4;
        private const string preAdmit = "PreAdmit";

        #endregion Constants

        #region Fields

        private static readonly ILog c_log =
            LogManager.GetLogger( typeof( RuleEngine ) );

        private static RuleEngine c_singletonInstance;
        private static readonly object c_syncRoot = new Object();
        private readonly Hashtable actionList = new RuleHashtable();
        private readonly RuleHashtable activityCompositeRules = new RuleHashtable();
        private readonly RuleHashtable activityRuleList = new RuleHashtable();
        private readonly RuleArrayList benefitsRegisteredContexts = new RuleArrayList();
        private readonly RuleHashtable validationCompositeRules = new RuleHashtable();
        private readonly RuleHashtable validationRuleList = new RuleHashtable();
        private ArrayList actionArray;
        private bool blnAllErrors;
        private bool blnErrorSeverityRuleFailed;
        private bool blnFireEvents = true;
        private bool blnSingleRunRule;

        private RuleHashtable compositeRules = new RuleHashtable();
        private string i_Activity;
        private RuleHashtable i_AllActions;
        private long i_LoadedAccount;
        private Account i_account;
        private RuleHashtable i_actionsForRules;
        private RuleBrokerProxy i_ruleBroker;
        private RuleHashtable rulesToRun = new RuleHashtable();

        #endregion Fields

        #region Constructors

        private RuleEngine()
        {
        }

        #endregion Constructors

        #region Properties

        private Account account
        {
            get { return i_account; }
            set { i_account = value; }
        }

        private ArrayList ActionArray
        {
            get
            {
                // return the actionList as an array
                actionArray = new ArrayList( actionList.Values );
                return actionArray;
            }
        }

        private RuleHashtable actionsForRules
        {
            get
            {
                if ( i_actionsForRules == null )
                {
                    i_actionsForRules = ruleBroker.RuleActionMapping();
                }

                return i_actionsForRules;
            }
        }


        private RuleHashtable AllActions
        {
            get
            {
                if ( i_AllActions == null )
                {
                    i_AllActions = ruleBroker.AllActions();
                }

                return i_AllActions;
            }
        }

        private RuleBrokerProxy ruleBroker
        {
            get
            {
                if ( i_ruleBroker == null )
                {
                    i_ruleBroker = new RuleBrokerProxy();
                }

                return i_ruleBroker;
            }
        }

        internal RuleHashtable RulesToRun
        {
            get { return rulesToRun; }
            set { rulesToRun = value; }
        }

        #endregion Properties

        public event EventHandler AllRulesPassedEvent;

        public event EventHandler NotAllRulesPassedEvent;

        #region Methods

        public bool AccountHasFailedError()
        {
            lock ( c_syncRoot )
            {
                foreach ( LeafAction la in ActionArray )
                {
                    if ( la.Severity == SEVERITY_ERROR
                        || la.Severity == SEVERITY_DEPRICATED_CHANGE )
                    {
                        return true;
                    }

                    if ( la.IsComposite )
                    {
                        var ca = la as CompositeAction;

                        if ( ca != null
                            && ca.Constituents != null )
                        {
                            foreach ( LeafAction cla in ca.Constituents )
                            {
                                if ( cla.Severity == SEVERITY_ERROR
                                    || cla.Severity == SEVERITY_DEPRICATED_CHANGE )
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
                return false;
            }
        }

        public void AddGenericError( string fieldName )
        {
            lock ( c_syncRoot )
            {
                var nAction = new GenericAction();

                nAction.Severity = SEVERITY_ERROR;
                nAction.RuleContextID = 999;
                nAction.Description = fieldName;
                nAction.IsComposite = false;

                bool blnExists = false;

                if ( actionList != null
                    && actionList.Count > 0 )
                {
                    foreach ( LeafAction la in actionList.Values )
                    {
                        if ( la.IsComposite )
                        {
                            var ca = la as CompositeAction;

                            if ( ca != null )
                            {
                                foreach ( LeafAction a in ca.Constituents )
                                {
                                    if ( a.Description == fieldName )
                                    {
                                        blnExists = true;
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if ( la.Description == fieldName )
                            {
                                blnExists = true;
                                break;
                            }
                        }
                    }
                }

                if ( blnExists ) return;

                nAction.Oid = getNextGenericOid();
                if ( actionList != null )
                {
                    actionList.Add( nAction.Oid, nAction );
                }
            }
        }

        public void ClearActions()
        {
            lock ( c_syncRoot )
            {
                actionList.Clear();
            }
        }

        public void ClearActionsForRule( Type aRuleType )
        {
            lock ( c_syncRoot )
            {
                clearActionsForRule( aRuleType );
            }
        }

        /// <summary>
        /// EvaluateAllRules - this method serves as a Service class method to 'build out' the 
        /// context collections for the generic rules.  It will 
        ///   1. load rules for the account's Activity, 
        ///   2. 'register' a context item from the account for each required rule implementation, 
        ///   3. run the rules
        ///   4. unload the rules
        /// 
        /// The generic rules are for:
        /// 
        /// Person      - Last name, First name, DOB, Drivers license state, Employer, Employment status, Gender,
        ///               Relationship, and SSN
        /// Contact     - Name, Phone, Relationship
        /// Guarantor   - Address, Employer address
        /// Insured     - Address
        /// Coverage 
        ///     Medicaid    - Policy/CIN
        ///     Medicare    - HIC number
        ///     All         - Authorization
        ///     Comm/Other  - Cert SSN
        ///     WC          - Patient supervisor, Policy #
        /// Physician   - First name, Last name, UPIN # - these are NOT registered here.  We assume that if
        ///     the AttendingPhysicianRequired, AdmittingPhysicianRequired, and ReferringPhysicianRequired
        ///     rules pass (those are Account-based, non-generic rules), then these will all have passed.
        /// Billing
        ///     Billing CO Name
        ///     Billing Name
        ///     Billing Address
        ///     Billing Phone
        /// </summary>
        /// <param name="anAccount"></param>
        public bool EvaluateAllRules( Account anAccount )
        {
            lock ( c_syncRoot )
            {
                return evaluateAllRules( anAccount );
            }
        }

        /// <summary>
        /// EvaluateRule - run a single rule; context is variable (not necessarily an account).
        /// No actions are loaded if the rule fails (it is a 'stand-alone' evaluation for setting UI attributes
        /// via the event handler).
        /// </summary>
        /// <param name="context"></param>
        public bool EvaluateRule<T>( object context )
        {
            return EvaluateRule( typeof( T ), context );
        }

        public bool EvaluateRule( Type aRuleType, object context )
        {
            lock ( c_syncRoot )
            {
                bool rc = true;

                blnFireEvents = true;
                blnSingleRunRule = true;

                rc = evaluateRule( context, aRuleType, null );

                return rc;
            }
        }

        public bool EvaluateRule<T>( object context, object control )
        {
            return EvaluateRule( typeof( T ), context, control );
        }

        public bool EvaluateRule( Type aRuleType, object context, object control )
        {
            lock ( c_syncRoot )
            {
                bool rc = true;

                blnFireEvents = true;
                blnSingleRunRule = true;

                rc = evaluateRule( context, aRuleType, control );

                return rc;
            }
        }

        /// <summary>
        /// EvaluateRules - run a collection of activity-based rules that have been previously loaded
        /// </summary>
        /// <param name="anAccount"></param>
        /// <returns></returns>
        public bool EvaluateRules( Account anAccount )
        {
            lock ( c_syncRoot )
            {
                blnSingleRunRule = false;

                bool rc = evaluateRules( anAccount );

                return rc;
            }
        }

        // return a formatted string to display in the Required Field Summary listing all Severity level 4 rules that failed
        public ICollection<CompositeAction> GetCompositeItemsCollection()
        {
            lock ( c_syncRoot )
            {
                return getCompositeItemsCollection();
            }
        }

        /// <summary>
        /// GetBenefitsValidationActionsItems - run the Benefits validation rules and return the associated actions
        /// for any failed rules
        /// </summary>
        /// <returns></returns>
        public string GetCompositeSummary( Account anAccount, Coverage aCoverage, string composite )
        {
            //We need to lock this object because it is accessed from multiple threads
            lock ( c_syncRoot )
            {
                RuleHashtable prevRules = RulesToRun;
                RuleHashtable prevCompRules = compositeRules;

                // load the composite rules
                loadValidationRules( composite );

                blnFireEvents = false;
                blnSingleRunRule = false;

                RulesToRun = ( RuleHashtable )validationRuleList.Clone();
                compositeRules = ( RuleHashtable )validationCompositeRules.Clone();

                actionList.Clear();

                if ( composite == "BenefitsValidation" )
                {
                    registerBenefitsValidation( anAccount, aCoverage );
                }

                evaluateRules( anAccount );

                if ( composite == "BenefitsValidation" )
                {
                    unregisterBenefitsValidation();
                }

                RulesToRun = prevRules;
                compositeRules = prevCompRules;

                string summary = string.Empty;
                var sb = new StringBuilder();

                foreach ( LeafAction la in GetFailedActions() )
                {
                    if ( la.IsComposite )
                    {
                        var ca = ( CompositeAction )la;

                        if ( ca.Constituents.Count > 0 )
                        {
                            sb.Append( ca.Description + "\r\n" );

                            foreach ( LeafAction cla in ca.Constituents )
                            {
                                // refactor to remove 'magic number'
                                if ( cla.Severity == SEVERITY_REQUIRED )
                                {
                                    sb.Append( INDENT + cla.Description + "\r\n" );
                                }
                            }
                        }
                    }

                    if ( sb.Length > 0 )
                    {
                        summary = sb.ToString();
                    }
                }

                blnFireEvents = true;

                return summary;
            }
        }

        /// <summary>
        /// GetFailedActions - return an array of actions associated with any failed rules
        /// </summary>
        /// <returns></returns>
        public ArrayList GetFailedActions()
        {
            lock ( c_syncRoot )
            {
                return ActionArray;
            }
        }

        // return a formatted string to display in the Deactivated Code Summary listing all Severity level 16 rules that failed
        public string GetInvalidCodeFieldSummary()
        {
            lock ( c_syncRoot )
            {
                string summary = string.Empty;
                var sb = new StringBuilder();

                foreach ( LeafAction la in GetFailedActions() )
                {
                    if ( la.IsComposite )
                    {
                        var ca = ( CompositeAction )la;

                        if ( ca.Constituents.Count > 0 )
                        {
                            bool descAdded = false;

                            foreach ( LeafAction cla in ca.Constituents )
                            {
                                // refactor to remove 'magic number'
                                if ( cla.Severity == SEVERITY_DEPRICATED
                                    || cla.Severity == SEVERITY_DEPRICATED_CHANGE )
                                {
                                    if ( !descAdded )
                                    {
                                        sb.Append( ca.Description + "\r\n" );
                                        descAdded = true;
                                    }
                                    sb.Append( INDENT + cla.Description + "\r\n" );
                                }
                            }
                        }
                    }

                    if ( sb.Length > 0 )
                    {
                        summary = sb.ToString();
                    }
                }

                return summary;
            }
        }

        // This method is called from the server and the client
        // From the Client: this is called from private methods which are wrapped in a lock
        // From the Server: the instance needs no locking (ie. not a singleton)
        public IAction GetMappedAction( long oid )
        {
            var ram = ( RuleToActionMapping )actionsForRules[oid];

            CompositeAction cAction = null;
            LeafAction lAction = null;

            if ( ram == null )
            {
                return null;
            }

            if ( ram.IsCompositeAction )
            {
                var anAction = AllActions[ram.ActionID] as LeafAction;

                if ( anAction != null )
                {
                    var parms = new object[1];
                    parms[0] = account;

                    cAction = Activator.CreateInstance( anAction.GetType(), parms ) as CompositeAction;
                }

                if ( cAction != null )
                {
                    cAction.Oid = anAction.Oid;
                    cAction.CompositeActionID = anAction.Oid;
                }

                return cAction;
            }
            else // we have a leaf action...
            {
                var anAction = AllActions[ram.ActionID] as LeafAction;

                if ( anAction != null )
                {
                    var parms = new object[1];
                    parms[0] = account;

                    lAction = Activator.CreateInstance( anAction.GetType(), parms ) as LeafAction;
                }

                if ( lAction != null )
                {
                    lAction.Oid = anAction.Oid;
                }

                return lAction;
            }
        }

        // return a formatted string to display in the Remaining Error Summary listing all Severity level 8 rules that failed
        public string GetRemainingErrorsSummary()
        {
            lock ( c_syncRoot )
            {
                string summary = string.Empty;
                var message = new StringBuilder();

                foreach ( LeafAction la in GetFailedActions() )
                {
                    var header = new StringBuilder();
                    var detail = new StringBuilder();

                    if ( la.IsComposite )
                    {
                        var ca = ( CompositeAction )la;

                        if ( ca.Constituents.Count > 0 )
                        {
                            header.Append( ca.Description + "\r\n" );

                            foreach ( LeafAction cla in ca.Constituents )
                            {
                                if ( cla.Severity == SEVERITY_ERROR )
                                {
                                    detail.Append( INDENT + cla.Description + "\r\n" );
                                }
                            }
                        }
                    }
                    else
                    {
                        if ( la.Severity == SEVERITY_ERROR )
                        {
                            detail.Append( la.Description + "\r\n" );
                        }
                    }

                    if ( detail.Length > 0 )
                    {
                        header.Append( detail.ToString() );
                        message.Append( header.ToString() );
                    }
                }

                if ( message.Length > 0 )
                {
                    summary = message.ToString();
                }


                return summary;
            }
        }

        public string GetRequiredFieldSummary()
        {
            lock ( c_syncRoot )
            {
                string summary = string.Empty;
                var message = new StringBuilder();

                foreach ( CompositeAction ca in getCompositeItemsCollection() )
                {
                    if ( ca.Constituents.Count > 0 )
                    {
                        var header = new StringBuilder();
                        var detail = new StringBuilder();

                        header.Append( ca.Description + "\r\n" );

                        foreach ( LeafAction cla in ca.Constituents )
                        {
                            // refactor to remove 'magic number'
                            if ( cla.Severity == SEVERITY_REQUIRED )
                            {
                                detail.Append( INDENT + cla.Description + "\r\n" );
                            }
                        }

                        if ( detail.Length > 0 )
                        {
                            header.Append( detail.ToString() );
                            message.Append( header.ToString() );
                        }
                    }

                    if ( message.Length > 0 )
                    {
                        summary = message.ToString();
                    }
                }

                return summary;
            }
        }

        /// <summary>
        /// GetWorklistActionsItems - run the worklist rules and return the associated actions for any
        /// failed rules
        /// </summary>
        /// <returns></returns>
        // This is called from the server and the client. We only need to lock if we are on the client.
        public ArrayList GetWorklistActionItems( Account anAccount )
        {
            if ( anAccount.BillHasDropped )
            {
                return new ArrayList();
            }

            c_log.Debug( "Getting Worklist Action Items" );
            //Check to see if we are on the client or on the server
            if ( c_singletonInstance == null )
            {
                //Don't lock on the server
                evaluateAllRules( anAccount );
                blnFireEvents = true;
                ArrayList returnArray = ActionArray;
                actionList.Clear();
                return returnArray;
            }

            //We need to lock if we are on the client
            lock ( c_syncRoot )
            {
                evaluateAllRules( anAccount );
                blnFireEvents = true;
                ArrayList returnArray = ActionArray;
                actionList.Clear();
                return returnArray;
            }
        }

        public void LoadRules( string inContext )
        {
            lock ( c_syncRoot )
            {
                loadRules( inContext );
            }
        }

        /// <summary>
        /// LoadRules - load a collection of rules relative to the activity for the current account
        /// </summary>
        /// <param name="anAccount"></param>
        public void LoadRules( Account anAccount )
        {
            lock ( c_syncRoot )
            {
                loadRules( anAccount );
            }
        }

        public void RegisterEvent<T>( object context, object control, EventHandler eventHandler )
        {
            RegisterEvent( typeof( T ), context, control, eventHandler );
        }

        /// <summary>
        /// RegisterEvent - overload to register an event and add an item to context with an associated control
        /// </summary>
        /// <param name="ruleType"></param>
        /// <param name="context"></param>
        /// <param name="control"></param>
        /// <param name="eventHandler"></param>
        public void RegisterEvent( Type ruleType, object context, object control, EventHandler eventHandler )
        {
            lock ( c_syncRoot )
            {
                addContextItem( ruleType, context, control );
                registerEvent( ruleType, eventHandler, null );
            }
        }

        /// <summary>
        /// RegisterEvent - overload to register an event and add an item to context with an associated control
        /// for a specific composite
        /// </summary>
        /// <param name="ruleType"></param>
        /// <param name="context"></param>
        /// <param name="control"></param>
        /// <param name="eventHandler"></param>
        /// <param name="compRuleType"></param>
        public void RegisterEvent( Type ruleType, object context, object control, EventHandler eventHandler,
                                  Type compRuleType )
        {
            lock ( c_syncRoot )
            {
                addContextItem( ruleType, context, control, compRuleType );
                registerEvent( ruleType, eventHandler, compRuleType );
            }
        }

        public void RegisterEvent<T>( object context, EventHandler eventHandler )
        {
            RegisterEvent( typeof( T ), context, eventHandler );
        }

        /// <summary>
        /// RegisterEvent - overload to register an event and add an item to context
        /// </summary>
        /// <param name="ruleType"></param>
        /// <param name="context"></param>
        /// <param name="eventHandler"></param>
        public void RegisterEvent( Type ruleType, object context, EventHandler eventHandler )
        {
            lock ( c_syncRoot )
            {
                addContextItem( ruleType, context );
                registerEvent( ruleType, eventHandler, null );
            }
        }

        public void RegisterEvent<T, R>( object context, EventHandler eventHandler )
        {
            RegisterEvent( typeof( T ), context, eventHandler, typeof( R ) );
        }

        /// <summary>
        /// RegisterEvent - overload to register an event and add an item to context for a specific composite rule
        /// </summary>
        /// <param name="ruleType"></param>
        /// <param name="context"></param>
        /// <param name="eventHandler"></param>
        /// <param name="compRuleType"></param>
        public void RegisterEvent( Type ruleType, object context, EventHandler eventHandler, Type compRuleType )
        {
            lock ( c_syncRoot )
            {
                addContextItem( ruleType, context, compRuleType );
                registerEvent( ruleType, eventHandler, compRuleType );
            }
        }

        public void RegisterEvent<T>( EventHandler eventHandler )
        {
            RegisterEvent( typeof( T ), eventHandler );
        }

        /// <summary>
        /// RegisterEvent - the vanilla variety
        /// </summary>
        /// <param name="ruleType"></param>
        /// <param name="eventHandler"></param>
        public void RegisterEvent( Type ruleType, EventHandler eventHandler )
        {
            lock ( c_syncRoot )
            {
                registerEvent( ruleType, eventHandler, null );
            }
        }

        /// <summary>
        /// RegisterEvent - for a rule in the collection of rules for this activity, register a client
        /// listener
        /// </summary>
        /// <param name="ruleType"></param>
        /// <param name="eventHandler"></param>
        /// <param name="compRuleType"></param>
        public void RegisterEvent( Type ruleType, EventHandler eventHandler, Type compRuleType )
        {
            lock ( c_syncRoot )
            {
                registerEvent( ruleType, eventHandler, compRuleType );
            }
        }

        public void RegisterGlobalEvent( string ruleType, EventHandler eventHandler )
        {
            lock ( c_syncRoot )
            {
                if ( ruleType == ALL_RULES_PASSED )
                {
                    AllRulesPassedEvent += eventHandler;
                }
                else if ( ruleType == NOT_ALL_RULES_PASSED )
                {
                    NotAllRulesPassedEvent += eventHandler;
                }
            }
        }

        public void RemoveGenericError( string fieldName )
        {
            // TLG - 7/24/2006 modified to capture index of item to be deleted, then
            // delete the item outside of the foreach loop; this, in hopes of avoiding
            // UE for index out of range error.
            lock ( c_syncRoot )
            {
                ArrayList aArray = ActionArray;
                if ( aArray != null
                    && aArray.Count > 0
                    && fieldName != string.Empty )
                {
                    long foundOID = -1;

                    foreach ( LeafAction la in aArray )
                    {
                        if ( la.Description == fieldName )
                        {
                            foundOID = la.Oid;
                            break;
                        }
                    }

                    if ( foundOID != -1 )
                    {
                        actionList.Remove( foundOID );
                    }
                }
            }
        }

        public void UnloadHandlers()
        {
            lock ( c_syncRoot )
            {
                foreach ( DictionaryEntry o in activityRuleList )
                {
                    var r = ( LeafRule )o.Value;
                    r.UnregisterHandlers();
                }

                AllRulesPassedEvent = null;
                NotAllRulesPassedEvent = null;
            }
        }

        public void UnregisterEvent<T>( EventHandler eventHandler )
        {
            UnregisterEvent( typeof( T ), eventHandler );
        }

        /// <summary>
        /// UnregisterEvent - for a rule in the collection of rules for this activity, unregister a client
        /// listener
        /// </summary>
        /// <param name="ruleType"></param>
        /// <param name="eventHandler"></param>
        public void UnregisterEvent( Type ruleType, EventHandler eventHandler )
        {
            lock ( c_syncRoot )
            {
                unregisterEvent( ruleType, eventHandler );
            }
        }

        public void UnregisterEvent<T>( object context, EventHandler eventHandler )
        {
            UnregisterEvent( typeof( T ), context, eventHandler );
        }

        /// <summary>
        /// UnregisterEvent - overload to remove context items for a rule
        /// </summary>
        /// <param name="ruleType"></param>
        /// <param name="context"></param>
        /// <param name="eventHandler"></param>
        public void UnregisterEvent( Type ruleType, object context, EventHandler eventHandler )
        {
            lock ( c_syncRoot )
            {
                removeAllContextItems( ruleType );

                unregisterEvent( ruleType, eventHandler );
            }
        }

        public void UnregisterGlobalEvent( string ruleType, EventHandler eventHandler )
        {
            lock ( c_syncRoot )
            {
                if ( ruleType == ALL_RULES_PASSED )
                {
                    AllRulesPassedEvent -= eventHandler;
                }
                else if ( ruleType == NOT_ALL_RULES_PASSED )
                {
                    NotAllRulesPassedEvent -= eventHandler;
                }
            }
        }

        public bool OneShotRuleEvaluation<T>( object context, EventHandler eventHandler )
        {
            RegisterEvent<T>( context, eventHandler );
            bool result = EvaluateRule<T>( context );
            UnregisterEvent<T>( context, eventHandler );
            return result;
        }

        public static bool AccountHasRequiredFields( ICollection<CompositeAction> actionItems )
        {
            foreach ( CompositeAction ca in actionItems )
            {
                if ( ca != null
                    && ca.Constituents != null )
                {
                    foreach ( LeafAction cla in ca.Constituents )
                    {
                        if ( cla.Severity == SEVERITY_REQUIRED )
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// GetInstance - return the singleton instance of this class
        /// </summary>
        /// <returns></returns>
        public static RuleEngine GetInstance()
        {
            if ( c_singletonInstance == null )
            {
                lock ( c_syncRoot )
                {
                    if ( c_singletonInstance == null )
                    {
                        c_singletonInstance = new RuleEngine();
                    }
                }
            }

            return c_singletonInstance;
        }

        /// <summary>
        /// NewInstance - should be called on the Server only.
        /// </summary>
        /// <returns></returns>
        public static RuleEngine NewInstance()
        {
            return new RuleEngine();
        }

        private void addContextItem( Type ruleType, object context )
        {
            var rc = new RuleComponent { RuleContext = context };

            addRuleComponent( ruleType, rc );
        }

        private void addContextItem( Type ruleType, object context, Type compRuleType )
        {
            var rc = new RuleComponent { RuleContext = context, CompositeRuleType = compRuleType };

            addRuleComponent( ruleType, rc );
        }

        private void addContextItem( Type ruleType, object context, object control )
        {
            var rc = new RuleComponent { RuleContext = context, RuleControl = control };

            addRuleComponent( ruleType, rc );
        }

        private void addContextItem( Type ruleType, object context, object control, Type compRuleType )
        {
            var rc = new RuleComponent
                         {
                             RuleContext = context,
                             RuleControl = control,
                             CompositeRuleType = compRuleType
                         };

            addRuleComponent( ruleType, rc );
        }

        /// <summary>
        /// addContextItem - add an item to the rules context collection
        /// </summary>
        /// <param name="ruleType"></param>
        /// <param name="rc"></param>
        private void addRuleComponent( Type ruleType, RuleComponent rc )
        {
            if ( ruleType == null || rc == null || RulesToRun == null )
            {
                return;
            }

            object o = RulesToRun[ruleType];
            LeafRule aRule = null;

            if ( o != null )
            {
                aRule = ( LeafRule )o;
            }

            if ( aRule != null )
            {
                bool blnExists = false;

                foreach ( RuleComponent rComp in aRule.ContextItems )
                {
                    if ( rComp.CompositeRuleType == rc.CompositeRuleType
                        && rComp.RuleContext == rc.RuleContext
                        && rComp.RuleControl == rc.RuleControl )
                    {
                        blnExists = true;
                        break;
                    }
                }

                if ( !blnExists )
                {
                    aRule.ContextItems.Add( rc );
                }
            }
        }

        private void clearActionsForRule( Type aRuleType )
        {
            RuleHashtable ht = actionsForRules;

            if ( actionList.Count <= 0 || RulesToRun == null
                || RulesToRun.Count <= 0 || aRuleType == null )
            {
                return;
            }

            var rule = ( LeafRule )RulesToRun[aRuleType];

            if ( rule != null )
            {
                if ( rule.IsComposite )
                {
                    var mapping = ( RuleToActionMapping )ht[rule.Oid];

                    if ( mapping != null )
                    {
                        if ( actionList.Contains( mapping.ActionID ) )
                        {
                            actionList.Remove( mapping.ActionID );
                        }
                    }
                }
                else
                {
                    if ( rule.CompositeRuleID > 0 )
                    {
                        var mapping = ( RuleToActionMapping )ht[rule.CompositeRuleID];

                        if ( mapping != null )
                        {
                            var cAction = actionList[mapping.ActionID] as CompositeAction;

                            if ( actionList.Contains( mapping.ActionID ) )
                            {
                                if ( cAction != null && cAction.Constituents != null )
                                {
                                    foreach ( LeafAction la in cAction.Constituents )
                                    {
                                        var leafMapping = ( RuleToActionMapping )ht[rule.Oid];

                                        if ( leafMapping != null
                                            && la.Oid == leafMapping.ActionID )
                                        {
                                            cAction.Remove( la );
                                        }
                                        else
                                        {
                                            if ( la.Oid == rule.Oid )
                                            {
                                                cAction.Remove( la );
                                            }
                                        }
                                    }
                                }
                            }

                            if ( cAction != null
                                && cAction.Constituents != null
                                && cAction.Constituents.Count == 0 )
                            {
                                actionList.Remove( cAction );
                            }
                        }
                    }
                    else
                    {
                        var leafMapping = ( RuleToActionMapping )ht[rule.Oid];

                        if ( leafMapping != null )
                        {
                            if ( actionList.Contains( leafMapping.ActionID ) )
                            {
                                actionList.Remove( leafMapping.ActionID );
                            }
                        }
                        else
                        {
                            if ( actionList.Contains( rule.Oid ) )
                            {
                                actionList.Remove( rule.Oid );
                            }
                        }
                    }
                }
            }
        }

        private bool evaluateAllRules( Account anAccount )
        {
            if ( (anAccount.IsShortRegisteredNonDayCareAccount()) ||
                ( anAccount.IsShortRegistered && String.IsNullOrEmpty( anAccount.KindOfVisit.Code ) )
            )
              
            {
                return evaluateAllRulesForShortRegistration(anAccount);
            }
            else if ( ( anAccount.Activity.GetType() == typeof(QuickAccountCreationActivity) ) ||
                       (anAccount.Activity.GetType() == typeof(QuickAccountMaintenanceActivity ) )
                    )
            {
                return evaluateAllRulesForQuickAccountRegistration(anAccount);
            }
                else if (anAccount.Activity.GetType() == typeof(PAIWalkinOutpatientCreationActivity))
            {
                return EvaluateAllRulesForPAIWalkinAccountRegistration(anAccount);
            }
            else
            {
                return evaluateAllRulesForRegularRegistration(anAccount);
            }
        }
      
        private bool evaluateAllRulesForRegularRegistration( Account anAccount )
        {
            c_log.Debug( "Start - evaluateAllRules" );

            blnAllErrors = true;
            blnFireEvents = false;
            blnSingleRunRule = false;

            bool blnRC = false;

            // 1. load the rules

            LoadRules( anAccount );

            c_log.Debug( "  evaluateAllRules - Rules loaded" );
            // 2. register context items

            // set up the coverages for Primary and Secondary

            if ( anAccount == null )
            {
                blnAllErrors = false;
                blnFireEvents = true;
                return false;
            }

            removeAllContextItems();

            c_log.Debug( "  evaluateAllRules - context items removed" );

            Coverage primaryCoverage = null;
            Coverage secondaryCoverage = null;

            if ( anAccount.Insurance != null )
            {
                primaryCoverage = anAccount.Insurance.CoverageFor( CoverageOrder.PRIMARY_OID );
                if ( primaryCoverage != null )
                {
                    primaryCoverage.Account = anAccount;
                }

                secondaryCoverage = anAccount.Insurance.CoverageFor( CoverageOrder.SECONDARY_OID );
                if ( secondaryCoverage != null )
                {
                    secondaryCoverage.Account = anAccount;
                }
            }

            // Store the rule type (RULETYPE), the context on which it applies (CONTEXT), 
            // and the composite type (COMPOSITE)

            var types = new RuleArrayList();

            c_log.Debug( "  evaluateAllRules - loading types collection" );
          
            // this section is for enforcing phone number required when area code is present
            if ( anAccount.Activity != null && 
                (anAccount.Activity.GetType() == typeof( PreMSERegisterActivity ) || anAccount.Activity.GetType() == typeof( EditPreMseActivity ) )||
                (anAccount.Activity.GetType() == typeof(UCCPreMSERegistrationActivity) || anAccount.Activity.GetType() == typeof(EditUCCPreMSEActivity)))
            {
                Type preMseDemogCompType = typeof( OnPreMSEDemographicsForm );
                PhoneNumber preMseMailingPhoneNumber =
                    anAccount.Patient.ContactPointWith( TypeOfContactPoint.NewMailingContactPointType() ).PhoneNumber;
                PhoneNumber preMsePhysicalPhoneNumber =
                    anAccount.Patient.ContactPointWith( TypeOfContactPoint.NewPhysicalContactPointType() ).PhoneNumber;
                PhoneNumber preMseMobilePhoneNumber =
                   anAccount.Patient.ContactPointWith(TypeOfContactPoint.NewMobileContactPointType()).PhoneNumber;
                types.Add( new object[] { typeof( AreaCodeRequiresPhoneNumber ), preMseMailingPhoneNumber, preMseDemogCompType } );
                types.Add( new object[] { typeof( AreaCodeRequiresPhoneNumber ), preMsePhysicalPhoneNumber, preMseDemogCompType } );
                types.Add( new object[] { typeof( AreaCodeRequiresPhoneNumber ), preMseMobilePhoneNumber, preMseDemogCompType });

                if ( anAccount.EmergencyContact1 != null )
                {
                    var emergencyContact = anAccount.EmergencyContact1;
                    var compositeRuleType = typeof( OnContactAndDiagnosisForm );
                    var emergencyContactPhoneNumber = emergencyContact.ContactPointWith( TypeOfContactPoint.NewPhysicalContactPointType() ).PhoneNumber;
                    
                    types.Add( new object[] { typeof( AreaCodeRequiresPhoneNumber ), emergencyContactPhoneNumber, compositeRuleType } );
                }

            }
            Type demogCompType = typeof( OnPatientDemographicsForm );

            PhoneNumber mailingPhoneNumber =
                anAccount.Patient.ContactPointWith( TypeOfContactPoint.NewMailingContactPointType() ).PhoneNumber;
            PhoneNumber cellPhoneNumber =
                anAccount.Patient.ContactPointWith( TypeOfContactPoint.NewMobileContactPointType() ).PhoneNumber;
            PhoneNumber physicalPhoneNumber =
                anAccount.Patient.ContactPointWith( TypeOfContactPoint.NewPhysicalContactPointType() ).PhoneNumber;

            types.Add( new object[] { typeof( AreaCodeRequiresPhoneNumber ), mailingPhoneNumber, demogCompType } );
            types.Add( new object[] { typeof( AreaCodeRequiresPhoneNumber ), cellPhoneNumber, demogCompType } );
            types.Add( new object[] { typeof( AreaCodeRequiresPhoneNumber ), physicalPhoneNumber, demogCompType } );
            types.Add( new object[] { typeof( SocialSecurityNumberRequired ), anAccount.Patient, demogCompType } );

            if ( anAccount.Patient != null
                 && anAccount.Patient.Employment != null
                 && anAccount.Patient.Employment.Employer != null
                 && anAccount.Patient.Employment.Employer.PartyContactPoint != null )
            {
                demogCompType = typeof( OnEmploymentForm );

                PhoneNumber employmentPhone = anAccount.Patient.Employment.Employer.PartyContactPoint.PhoneNumber;

                types.Add( new object[] { typeof( AreaCodeRequiresPhoneNumber ), employmentPhone, demogCompType } );
            }

            if ( anAccount.AllPhysicianRelationships != null
                 && anAccount.AllPhysicianRelationships.Count > 0 )
            {
                foreach ( PhysicianRelationship physicianRelationship in anAccount.AllPhysicianRelationships )
                {
                    if ( physicianRelationship.Physician != null
                         && physicianRelationship.Physician.PhysicianNumber == NON_STAFF_PHYSICIAN_NUMBER )
                    {
                        demogCompType = typeof( OnClinicalForm );

                        PhoneNumber physicianPhone = physicianRelationship.Physician.PhoneNumber;

                        types.Add( new object[] { typeof( AreaCodeRequiresPhoneNumber ), physicianPhone, demogCompType } );
                    }
                }
            }

            if ( anAccount.Guarantor != null )
            {
                Guarantor guarantor = anAccount.Guarantor;
                Type compType = typeof( OnGuarantorForm );

                types.Add( new object[] { typeof( PersonRelationshipRequired ), guarantor, compType } );
                types.Add( new object[] { typeof( PersonLastNameRequired ), guarantor, compType } );
                types.Add( new object[] { typeof( PersonFirstNameRequired ), guarantor, compType } );
                types.Add( new object[] { typeof( PersonGenderRequired ), guarantor, compType } );
                types.Add( new object[] { typeof( PersonDateOfBirthRequired ), guarantor, compType } );
                types.Add( new object[] { typeof( InsuredAddressRequired ), guarantor, compType } );
                types.Add( new object[] { typeof( PersonEmployerRequired ), guarantor, compType } );
                types.Add( new object[] { typeof( GuarantorAddressRequired ), guarantor, compType } );
                types.Add( new object[] { typeof( GuarantorEmployerAddressRequired ), guarantor, compType } );
                types.Add( new object[] { typeof( PersonDriversLicenseStateRequired ), guarantor, compType } );
                types.Add( new object[] { typeof( PersonSSNRequired ), guarantor, compType } );
                types.Add( new object[] { typeof( PersonRelationshipPreferred ), guarantor, compType } );
                types.Add( new object[] { typeof( PersonLastNamePreferred ), guarantor, compType } );
                types.Add( new object[] { typeof( PersonFirstNamePreferred ), guarantor, compType } );
                types.Add( new object[] { typeof( PersonSSNPreferred ), guarantor, compType } );
                types.Add( new object[] { typeof( PersonGenderPreferred ), guarantor, compType } );
                types.Add( new object[] { typeof( PersonDriversLicensePreferred ), guarantor, compType } );
                types.Add( new object[] { typeof( GuarantorAddressPreferred ), guarantor, compType } );
                types.Add( new object[] { typeof( PersonPhoneNumberPreferred ), guarantor, compType } );
                types.Add( new object[] { typeof( PersonPhoneAreaCodePreferred ), guarantor, compType } );
                types.Add( new object[] { typeof( GuarantorEmploymentStatusPreferred ), guarantor, compType } );
                types.Add( new object[] { typeof( GuarantorEmploymentPhoneNumberPreferred ), anAccount, compType } );
                types.Add( new object[] { typeof( GuarantorEmploymentPhoneAreaCodePreferred ), anAccount, compType } );
                types.Add( new object[] { typeof( GuarantorConsentRequired ), anAccount, compType });
                types.Add( new object[] { typeof( GuarantorConsentPreferred ), anAccount, compType });

                PhoneNumber guarantorMailingPhoneNumber =
                    anAccount.Guarantor.ContactPointWith( TypeOfContactPoint.NewMailingContactPointType() ).PhoneNumber;
                PhoneNumber guarantorCellPhoneNumber =
                    anAccount.Guarantor.ContactPointWith( TypeOfContactPoint.NewMobileContactPointType() ).PhoneNumber;

                if ( anAccount.Guarantor.Employment != null
                    && anAccount.Guarantor.Employment.Employer != null
                    && anAccount.Guarantor.Employment.Employer.PartyContactPoint != null )
                {
                    PhoneNumber employmentPhoneNumber =
                        anAccount.Guarantor.Employment.Employer.PartyContactPoint.PhoneNumber;
                    types.Add( new object[] { typeof( AreaCodeRequiresPhoneNumber ), employmentPhoneNumber, compType } );
                }

                types.Add( new object[] { typeof( AreaCodeRequiresPhoneNumber ), guarantorMailingPhoneNumber, compType } );
                types.Add( new object[] { typeof( AreaCodeRequiresPhoneNumber ), guarantorCellPhoneNumber, compType } );
            }

            if ( primaryCoverage != null
                && primaryCoverage.Insured != null )
            {
                Insured insured = primaryCoverage.Insured;
                Type compType = typeof( OnInsuredFormForPrimaryInsurance );

                types.Add( new object[] { typeof( PersonRelationshipRequired ), insured, compType } );
                types.Add( new object[] { typeof( PersonLastNameRequired ), insured, compType } );
                types.Add( new object[] { typeof( PersonFirstNameRequired ), insured, compType } );
                types.Add( new object[] { typeof( PersonGenderRequired ), insured, compType } );
                types.Add( new object[] { typeof( PersonDateOfBirthRequired ), insured, compType } );
                types.Add( new object[] { typeof( InsuredAddressRequired ), insured, compType } );
                types.Add( new object[] { typeof( PersonEmploymentStatusRequired ), insured, compType } );
                types.Add( new object[] { typeof( InsuredEmployerRequired ), insured, compType } );
                types.Add( new object[] { typeof( PersonEmployerAddressRequired ), insured, compType } );
                types.Add( new object[] { typeof( PersonRelationshipPreferred ), insured, compType } );
                types.Add( new object[] { typeof( PersonLastNamePreferred ), insured, compType } );
                types.Add( new object[] { typeof( PersonFirstNamePreferred ), insured, compType } );
                types.Add( new object[] { typeof( PersonDateOfBirthPreferred ), insured, compType } );
                types.Add( new object[] { typeof( PersonGenderPreferred ), insured, compType } );
                types.Add( new object[] { typeof( InsuredAddressPreferred ), insured, compType } );
                types.Add( new object[] { typeof( PersonPhoneNumberPreferred ), insured, compType } );
                types.Add( new object[] { typeof( PersonPhoneAreaCodePreferred ), insured, compType } );
                types.Add( new object[] { typeof( PersonEmploymentStatusPreferred ), insured, compType } );
                types.Add( new object[] { typeof( EmploymentPhoneNumberPreferred ), insured.Employment, compType } );
                types.Add( new object[] { typeof( EmploymentPhoneAreaCodePreferred ), insured.Employment, compType } );

                addContextItem( typeof( AdmitDateToPrimaryPlanDates ), anAccount, "demographicsView", null );
                registerEvent( typeof( AdmitDateToPrimaryPlanDates ), null, null );

                if ( insured != null )
                {
                    ContactPoint contactPoint =
                        insured.ContactPointWith( TypeOfContactPoint.NewPhysicalContactPointType() );

                    if ( contactPoint != null )
                    {
                        types.Add( new object[] { typeof( AreaCodeRequiresPhoneNumber ), contactPoint.PhoneNumber, compType } );
                    }

                    contactPoint = insured.ContactPointWith( TypeOfContactPoint.NewMobileContactPointType() );

                    if ( contactPoint != null )
                    {
                        types.Add( new object[] { typeof( AreaCodeRequiresPhoneNumber ), contactPoint.PhoneNumber, compType } );
                    }

                    if ( insured.Employment != null
                        && insured.Employment.Employer != null
                        && insured.Employment.Employer.PartyContactPoint != null )
                    {
                        contactPoint = insured.Employment.Employer.PartyContactPoint;

                        types.Add( new object[] { typeof( AreaCodeRequiresPhoneNumber ), contactPoint.PhoneNumber, compType } );
                    }
                }

                if ( primaryCoverage.InsurancePlan != null
                    && primaryCoverage.BillingInformation != null )
                {
                    compType = typeof( OnPayorDetailsFormForPrimaryPayor );

                    types.Add( new object[]
                                  {
                                      typeof (AreaCodeRequiresPhoneNumber), primaryCoverage.BillingInformation.PhoneNumber,
                                      compType
                                  } );
                }

                if ( primaryCoverage is CoverageGroup )
                {
                    if ( ( primaryCoverage as CoverageGroup ).Authorization != null )
                    {
                        compType = typeof( OnVerificationFormForPrimaryInsurance );

                        types.Add( new object[]
                                      {
                                          typeof (AreaCodeRequiresPhoneNumber),
                                          (primaryCoverage as CoverageGroup).Authorization.AuthorizationPhone, compType
                                      } );
                    }
                }

                if ( primaryCoverage.Attorney != null )
                {
                    ContactPoint contactPoint =
                        primaryCoverage.Attorney.ContactPointWith( TypeOfContactPoint.NewBusinessContactPointType() );

                    compType = typeof( OnVerificationFormForPrimaryInsurance );

                    types.Add( new object[] { typeof( AreaCodeRequiresPhoneNumber ), contactPoint.PhoneNumber, compType } );
                }

                if ( primaryCoverage.InsuranceAgent != null )
                {
                    ContactPoint contactPoint =
                        primaryCoverage.InsuranceAgent.ContactPointWith( TypeOfContactPoint.NewBusinessContactPointType() );

                    compType = typeof( OnVerificationFormForPrimaryInsurance );

                    types.Add( new object[] { typeof( AreaCodeRequiresPhoneNumber ), contactPoint.PhoneNumber, compType } );
                }
                if ( primaryCoverage is CoverageGroup )
                {
                    if ( ( primaryCoverage as CoverageGroup ).Authorization != null )
                    {
                        compType = typeof( OnAuthorizationFormForPrimaryCoverage );
                        types.Add( new object[] { typeof( CompanyRepFirstNamePreferred ), primaryCoverage, compType } );
                        types.Add( new object[] { typeof( CompanyRepLastNamePreferred ), primaryCoverage, compType } );
                        types.Add( new object[] { typeof( ServicesAuthorizedPreferred ), primaryCoverage, compType } );
                        types.Add( new object[] { typeof( WorkersCompAuthCodePreferred ), primaryCoverage, compType } );
                    }
                }
            }

            if ( secondaryCoverage != null
                && secondaryCoverage.Insured != null )
            {
                Insured insured = secondaryCoverage.Insured;
                Type compType = typeof( OnInsuredFormForSecondaryInsurance );

                types.Add( new object[] { typeof( PersonRelationshipRequired ), insured, compType } );
                types.Add( new object[] { typeof( PersonLastNameRequired ), insured, compType } );
                types.Add( new object[] { typeof( PersonFirstNameRequired ), insured, compType } );
                types.Add( new object[] { typeof( PersonGenderRequired ), insured, compType } );
                types.Add( new object[] { typeof( PersonDateOfBirthRequired ), insured, compType } );
                types.Add( new object[] { typeof( InsuredAddressRequired ), insured, compType } );
                types.Add( new object[] { typeof( PersonEmploymentStatusRequired ), insured, compType } );
                types.Add( new object[] { typeof( InsuredEmployerRequired ), insured, compType } );
                types.Add( new object[] { typeof( PersonEmployerAddressRequired ), insured, compType } );
                types.Add( new object[] { typeof( PersonRelationshipPreferred ), insured, compType } );
                types.Add( new object[] { typeof( PersonLastNamePreferred ), insured, compType } );
                types.Add( new object[] { typeof( PersonFirstNamePreferred ), insured, compType } );
                types.Add( new object[] { typeof( PersonDateOfBirthPreferred ), insured, compType } );
                types.Add( new object[] { typeof( PersonGenderPreferred ), insured, compType } );
                types.Add( new object[] { typeof( InsuredAddressPreferred ), insured, compType } );
                types.Add( new object[] { typeof( PersonPhoneNumberPreferred ), insured, compType } );
                types.Add( new object[] { typeof( PersonPhoneAreaCodePreferred ), insured, compType } );
                types.Add( new object[] { typeof( PersonEmploymentStatusPreferred ), insured, compType } );
                types.Add( new object[] { typeof( EmploymentPhoneNumberPreferred ), insured.Employment, compType } );
                types.Add( new object[] { typeof( EmploymentPhoneAreaCodePreferred ), insured.Employment, compType } );

                addContextItem( typeof( AdmitDateToSecondaryPlanDates ), anAccount, "demographicsView", null );
                registerEvent( typeof( AdmitDateToSecondaryPlanDates ), null, null );

                if ( insured != null )
                {
                    ContactPoint contactPoint =
                        insured.ContactPointWith( TypeOfContactPoint.NewPhysicalContactPointType() );

                    if ( contactPoint != null )
                    {
                        types.Add( new object[] { typeof( AreaCodeRequiresPhoneNumber ), contactPoint.PhoneNumber, compType } );
                    }

                    contactPoint = insured.ContactPointWith( TypeOfContactPoint.NewMobileContactPointType() );

                    if ( contactPoint != null )
                    {
                        types.Add( new object[] { typeof( AreaCodeRequiresPhoneNumber ), contactPoint.PhoneNumber, compType } );
                    }

                    if ( insured.Employment != null
                        && insured.Employment.Employer != null
                        && insured.Employment.Employer.PartyContactPoint != null )
                    {
                        contactPoint = insured.Employment.Employer.PartyContactPoint;

                        types.Add( new object[] { typeof( AreaCodeRequiresPhoneNumber ), contactPoint.PhoneNumber, compType } );
                    }
                }

                if ( secondaryCoverage.InsurancePlan != null
                    && secondaryCoverage.BillingInformation != null )
                {
                    compType = typeof( OnPayorDetailsFormForPrimaryPayor );

                    types.Add( new object[]
                                  {
                                      typeof (AreaCodeRequiresPhoneNumber),
                                      secondaryCoverage.BillingInformation.PhoneNumber, compType
                                  } );
                }


                if ( secondaryCoverage is CoverageGroup )
                {
                    if ( ( secondaryCoverage as CoverageGroup ).Authorization != null )
                    {
                        compType = typeof( OnVerificationFormForSecondaryInsurance );

                        types.Add( new object[]
                                      {
                                          typeof (AreaCodeRequiresPhoneNumber),
                                          (secondaryCoverage as CoverageGroup).Authorization.AuthorizationPhone, compType
                                      } );
                    }
                }

                if ( secondaryCoverage.Attorney != null )
                {
                    ContactPoint contactPoint =
                        secondaryCoverage.Attorney.ContactPointWith( TypeOfContactPoint.NewBusinessContactPointType() );

                    compType = typeof( OnVerificationFormForPrimaryInsurance );

                    types.Add( new object[] { typeof( AreaCodeRequiresPhoneNumber ), contactPoint.PhoneNumber, compType } );
                }

                if ( secondaryCoverage.InsuranceAgent != null )
                {
                    ContactPoint contactPoint =
                        secondaryCoverage.InsuranceAgent.ContactPointWith(
                            TypeOfContactPoint.NewBusinessContactPointType() );

                    compType = typeof( OnVerificationFormForPrimaryInsurance );

                    types.Add( new object[] { typeof( AreaCodeRequiresPhoneNumber ), contactPoint.PhoneNumber, compType } );
                }
                if ( secondaryCoverage is CoverageGroup )
                {
                    if ( ( secondaryCoverage as CoverageGroup ).Authorization != null )
                    {
                        compType = typeof( OnAuthorizationFormForSecondaryCoverage );
                        types.Add( new object[] { typeof( CompanyRepFirstNamePreferred ), secondaryCoverage, compType } );
                        types.Add( new object[] { typeof( CompanyRepLastNamePreferred ), secondaryCoverage, compType } );
                        types.Add( new object[] { typeof( ServicesAuthorizedPreferred ), secondaryCoverage, compType } );
                        types.Add( new object[] { typeof( WorkersCompAuthCodePreferred ), primaryCoverage, compType } );
                    }
                }
            }

            // Contact - these generic rules are implemented for:
            //   Primary contact

            if ( anAccount.EmergencyContact1 != null )
            {
                EmergencyContact econtact = anAccount.EmergencyContact1;
                Type compType = typeof( OnContactsForm );

                types.Add( new object[] { typeof( ContactNameRequired ), econtact, compType } );
                types.Add( new object[] { typeof( ContactRelationshipRequired ), econtact, compType } );
                types.Add( new object[] { typeof( ContactAreaCodeRequired ), econtact, compType } );
                types.Add( new object[] { typeof( ContactPhoneRequired ), econtact, compType } );
                types.Add( new object[] { typeof( ContactNamePreferred ), econtact, compType } );
                types.Add( new object[] { typeof( ContactRelationshipPreferred ), econtact, compType } );
                types.Add( new object[] { typeof( ContactAreaCodePreferred ), econtact, compType } );
                types.Add( new object[] { typeof( ContactPhonePreferred ), econtact, compType } );
                types.Add( new object[] { typeof( ContactAddressPreferred ), econtact, compType } );

                types.Add( new object[]
                              {
                                  typeof (AreaCodeRequiresPhoneNumber),
                                  econtact.ContactPointWith(TypeOfContactPoint.NewPhysicalContactPointType()).PhoneNumber,
                                  compType
                              } );
            }

            if ( anAccount.EmergencyContact2 != null )
            {
                EmergencyContact econtact = anAccount.EmergencyContact2;
                Type compType = typeof( OnContactsForm );

                types.Add( new object[]
                              {
                                  typeof (AreaCodeRequiresPhoneNumber),
                                  econtact.ContactPointWith(TypeOfContactPoint.NewPhysicalContactPointType()).PhoneNumber,
                                  compType
                              } );
            }
            if (anAccount != null)
            {

                Type compType = typeof (OnPaymentForm);

                types.Add(new object[]
                {
                    typeof (MonthlyDueDateRequired),
                    anAccount,
                    compType
                });
            }

            // Coverage 
            //     Medicaid    - Eligibility date, Patient has Medicare, Policy/CIN
            //     Medicare    - HIC number
            //     All         - Authorization
            //     Comm/Other  - Cert SSN, Medicare HIC# (if IME facility and Financial Class = '84')
            //     WC          - Authorization #, Patient supervisor, Policy #

            if ( primaryCoverage != null )
            {
                Type compType = typeof( OnPayorDetailsFormForPrimaryPayor );

                types.Add( new object[] { typeof( MedicaidIssueDateRequired ), primaryCoverage, compType } );
                types.Add( new object[] { typeof( MedicaidPolicyCINNumberRequired ), primaryCoverage, compType } );
                
                types.Add(new object[] { typeof(MBINumberRequired), primaryCoverage, compType });
                types.Add( new object[] { typeof( InsuranceAuthorizationRequiredPreferred ), primaryCoverage, compType } );
                types.Add( new object[] { typeof( InsurancePlanSSNRequired ), primaryCoverage, compType } );
             
                types.Add( new object[] { typeof( WorkersCompEmpSupervisorRequired ), primaryCoverage, compType } );
                types.Add( new object[] { typeof( WorkersCompPolicyNumberRequired ), primaryCoverage, compType } );

                types.Add( new object[] { typeof( MedicaidIssueDatePreferred ), primaryCoverage, compType } );
                types.Add( new object[] { typeof( MedicaidPolicyCINNumberPreferred ), primaryCoverage, compType } );
                
                types.Add(new object[] { typeof(MBINumberPreferred), primaryCoverage, compType });
                types.Add( new object[] { typeof( InsurancePlanSSNPreferred ), primaryCoverage, compType } );
                types.Add( new object[] { typeof( WorkersCompEmpSupervisorPreferred ), primaryCoverage, compType } );
                types.Add( new object[] { typeof( WorkersCompPolicyNumberPreferred ), primaryCoverage, compType } );


                types.Add( new object[] { typeof( BillingCONameRequired ), primaryCoverage, compType } );
                types.Add( new object[] { typeof( BillingNameRequired ), primaryCoverage, compType } );
                types.Add( new object[] { typeof( BillingAddressRequired ), primaryCoverage.BillingInformation, compType } );
                types.Add( new object[] { typeof( BillingPhoneRequired ), primaryCoverage, compType } );

                types.Add( new object[] { typeof( BillingCONamePreferred ), primaryCoverage, compType } );
                types.Add( new object[] { typeof( BillingNamePreferred ), primaryCoverage, compType } );
                types.Add( new object[] { typeof( BillingAddressPreferred ), primaryCoverage.BillingInformation, compType } );
                types.Add( new object[] { typeof( BillingPhonePreferred ), primaryCoverage, compType } );

                types.Add( new object[] { typeof( InsurancePlanIPARequired ), primaryCoverage, compType } );
                types.Add(new object[] { typeof(DOFRInsurancePartIPARequired), primaryCoverage, compType });
                types.Add(new object[] { typeof(DOFRAidCodeRequired), primaryCoverage, compType });
            }

            if ( secondaryCoverage != null )
            {
                Type compType = typeof( OnPayorDetailsFormForSecondaryPayor );

                types.Add( new object[] { typeof( MedicaidIssueDateRequired ), secondaryCoverage, compType } );
                types.Add( new object[] { typeof( MedicaidPolicyCINNumberRequired ), secondaryCoverage, compType } );
                
                types.Add(new object[] { typeof(MBINumberRequired), secondaryCoverage, compType });

                types.Add( new object[] { typeof( InsuranceAuthorizationRequiredPreferred ), secondaryCoverage, compType } );
                types.Add( new object[] { typeof( InsurancePlanSSNRequired ), secondaryCoverage, compType } );
                
                types.Add( new object[] { typeof( WorkersCompEmpSupervisorRequired ), secondaryCoverage, compType } );
                types.Add( new object[] { typeof( WorkersCompPolicyNumberRequired ), secondaryCoverage, compType } );

                types.Add( new object[] { typeof( MedicaidIssueDatePreferred ), secondaryCoverage, compType } );
                types.Add( new object[] { typeof( MedicaidPolicyCINNumberPreferred ), secondaryCoverage, compType } );
              
                types.Add(new object[] { typeof(MBINumberPreferred), secondaryCoverage, compType });
                types.Add(new object[] { typeof(MBINumberPreferred), secondaryCoverage, compType });
                types.Add( new object[] { typeof( InsurancePlanSSNPreferred ), secondaryCoverage, compType } );
                types.Add( new object[] { typeof( WorkersCompEmpSupervisorPreferred ), secondaryCoverage, compType } );
                types.Add( new object[] { typeof( WorkersCompPolicyNumberPreferred ), secondaryCoverage, compType } );


                types.Add( new object[] { typeof( BillingCONameRequired ), secondaryCoverage, compType } );
                types.Add( new object[] { typeof( BillingNameRequired ), secondaryCoverage, compType } );
                types.Add( new object[] { typeof( BillingAddressRequired ), secondaryCoverage.BillingInformation, compType } );
                types.Add( new object[] { typeof( BillingPhoneRequired ), secondaryCoverage, compType } );

                types.Add( new object[] { typeof( BillingCONamePreferred ), secondaryCoverage, compType } );
                types.Add( new object[] { typeof( BillingNamePreferred ), secondaryCoverage, compType } );
                types.Add( new object[] { typeof( BillingAddressPreferred ), secondaryCoverage.BillingInformation, compType } );
                types.Add( new object[] { typeof( BillingPhonePreferred ), secondaryCoverage, compType } );

                types.Add( new object[] { typeof( InsurancePlanIPARequired ), secondaryCoverage, compType } );
            }

            // Store the rule type (RULETYPE), the context on which it applies (CONTEXT), 
            // the composite rule type (COMPOSITE), and the related "control" (CONTROL).
            // The "control" here is used to pass in the Activity value necessary for rule evaluation

            var nonStaffPhysicianRules = new RuleArrayList();

            // Referring NonStaff Physician
            if ( anAccount.ReferringPhysician != null )
            {
                Physician referringPhysician = anAccount.ReferringPhysician;
                Type compType = typeof( OnReferringNonStaffPhysicianForm );

                nonStaffPhysicianRules.Add( new object[]
                                               {
                                                   typeof (NonStaffPhysicianNPIRequired), referringPhysician, compType,
                                                   anAccount.Activity
                                               } );
            }

            // Admitting NonStaff Physician
            if ( anAccount.AdmittingPhysician != null )
            {
                Physician admittingPhysician = anAccount.AdmittingPhysician;
                Type compType = typeof( OnAdmittingNonStaffPhysicianForm );

                nonStaffPhysicianRules.Add( new object[]
                                               {
                                                   typeof (NonStaffPhysicianNPIRequired), admittingPhysician, compType,
                                                   anAccount.Activity
                                               } );
            }

            // Attending NonStaff Physician
            if ( anAccount.AttendingPhysician != null )
            {
                Physician attendingPhysician = anAccount.AttendingPhysician;
                Type compType = typeof( OnAttendingNonStaffPhysicianForm );

                nonStaffPhysicianRules.Add( new object[]
                                               {
                                                   typeof (NonStaffPhysicianNPIRequired), attendingPhysician, compType,
                                                   anAccount.Activity
                                               } );
            }

            // Operating NonStaff Physician
            if ( anAccount.OperatingPhysician != null )
            {
                Physician operatingPhysician = anAccount.OperatingPhysician;
                Type compType = typeof( OnOperatingNonStaffPhysicianForm );

                nonStaffPhysicianRules.Add( new object[]
                                               {
                                                   typeof (NonStaffPhysicianNPIRequired), operatingPhysician, compType,
                                                   anAccount.Activity
                                               } );
            }

            // PrimaryCare NonStaff Physician
            if ( anAccount.PrimaryCarePhysician != null )
            {
                Physician primaryCarePhysician = anAccount.PrimaryCarePhysician;
                Type compType = typeof( OnPCPNonStaffPhysicianForm );

                nonStaffPhysicianRules.Add( new object[]
                                               {
                                                   typeof (NonStaffPhysicianNPIRequired), primaryCarePhysician, compType,
                                                   anAccount.Activity
                                               } );
            }

            c_log.Debug( "  evaluateAllRules - finished loading types collection" );

            // register all of the events we've just stored
            foreach ( object[] type in types )
            {
                addContextItem( (Type)type[RULETYPE], type[CONTEXT], (Type)type[COMPOSITE] );
                registerEvent( (Type)type[RULETYPE], null, (Type)type[COMPOSITE] );
            }

            foreach ( object[] type in nonStaffPhysicianRules )
            {
                addContextItem( (Type)type[RULETYPE], type[CONTEXT], type[CONTROL], (Type)type[COMPOSITE] );
                registerEvent( (Type)type[RULETYPE], null, (Type)type[COMPOSITE] );
            }

            c_log.Debug( "  evaluateAllRules - finished registering events" );

            // Physician - these are NOT registered here.  We assume that if
            //     the AttendingPhysicianRequired, AdmittingPhysicianRequired, and ReferringPhysicianRequired
            //     rules pass (those are Account-based, non-generic rules), then these will all have passed.

            // 3. run the rules

            blnSingleRunRule = false;

            blnRC = evaluateRules( anAccount );

            c_log.Debug( "  evaluateAllRules - finished running the rules" );

            // 4. remove context items we added
            foreach ( object[] type in types )
            {
                removeContextItem( (Type)type[RULETYPE], type[CONTEXT] );
                unregisterEvent( (Type)type[RULETYPE], null );
            }

            c_log.Debug( "  evaluateAllRules - context items removed (again)" );

            // manually remove the extraordinary ones
            removeContextItem( typeof( AdmitDateToPrimaryPlanDates ), anAccount );
            removeContextItem( typeof( AdmitDateToSecondaryPlanDates ), anAccount );

            blnAllErrors = false;
            blnFireEvents = true;

            return blnRC;
        }
        private bool evaluateAllRulesForShortRegistration( Account anAccount )
        {
            c_log.Debug( "Start - evaluateAllRulesForShortRegistration" );

            blnAllErrors = true;
            blnFireEvents = false;
            blnSingleRunRule = false;

            bool blnRC = false;

            // 1. load the rules

            LoadRules( anAccount );

            c_log.Debug( "  evaluateAllRulesForSHort - Rules loaded" );
            // 2. register context items

            // set up the coverages for Primary and Secondary

            if ( anAccount == null )
            {
                blnAllErrors = false;
                blnFireEvents = true;
                return false;
            }

            removeAllContextItems();

            c_log.Debug( "  evaluateAllRules - context items removed" );

            Coverage primaryCoverage = null;
            Coverage secondaryCoverage = null;

            if ( anAccount.Insurance != null )
            {
                primaryCoverage = anAccount.Insurance.CoverageFor( CoverageOrder.PRIMARY_OID );
                if ( primaryCoverage != null )
                {
                    primaryCoverage.Account = anAccount;
                }

                secondaryCoverage = anAccount.Insurance.CoverageFor( CoverageOrder.SECONDARY_OID );
                if ( secondaryCoverage != null )
                {
                    secondaryCoverage.Account = anAccount;
                }
            }

            // Store the rule type (RULETYPE), the context on which it applies (CONTEXT), 
            // and the composite type (COMPOSITE)

            var types = new RuleArrayList();

            c_log.Debug( "  evaluateAllRules - loading types collection" );
           
           
            Type demogCompType = typeof( OnShortDemographicsForm );

            PhoneNumber mailingPhoneNumber =
                anAccount.Patient.ContactPointWith( TypeOfContactPoint.NewMailingContactPointType() ).PhoneNumber;
            PhoneNumber cellPhoneNumber =
                anAccount.Patient.ContactPointWith( TypeOfContactPoint.NewMobileContactPointType() ).PhoneNumber;
            
            types.Add( new object[] { typeof( AreaCodeRequiresPhoneNumber ), mailingPhoneNumber, demogCompType } );
            types.Add( new object[] { typeof( AreaCodeRequiresPhoneNumber ), cellPhoneNumber, demogCompType } );
            types.Add( new object[] { typeof( SocialSecurityNumberRequired ), anAccount.Patient, demogCompType } );
            
            if ( anAccount.AllPhysicianRelationships != null
                 && anAccount.AllPhysicianRelationships.Count > 0 )
            {
                foreach ( PhysicianRelationship physicianRelationship in anAccount.AllPhysicianRelationships )
                {
                    if ( physicianRelationship.Physician != null
                         && physicianRelationship.Physician.PhysicianNumber == NON_STAFF_PHYSICIAN_NUMBER )
                    {
                        demogCompType = typeof( OnShortDiagnosisForm );

                        PhoneNumber physicianPhone = physicianRelationship.Physician.PhoneNumber;

                        types.Add( new object[] { typeof( AreaCodeRequiresPhoneNumber ), physicianPhone, demogCompType } );
                        types.Add( new object[] {typeof ( PatientPortalOptInRequired ), anAccount.Patient, demogCompType } );
                    }
                }
            }

            if ( anAccount.Guarantor != null )
            {
                Guarantor guarantor = anAccount.Guarantor;
                Type compType = typeof( OnShortGuarantorForm );

               
                types.Add( new object[] { typeof( PersonLastNameRequired ), guarantor, compType } );
                types.Add( new object[] { typeof( PersonFirstNameRequired ), guarantor, compType } );
                types.Add( new object[] { typeof( PersonDateOfBirthRequired ), guarantor, compType } );
                types.Add( new object[] { typeof( GuarantorAddressRequired ), guarantor, compType } );
                types.Add( new object[] { typeof( PersonPhoneAreaCodeRequired ), guarantor, compType } );
                types.Add( new object[] { typeof( PersonPhoneNumberRequired ), guarantor, compType } );
                types.Add( new object[] { typeof( PersonSSNRequired ), guarantor, compType } );
                types.Add( new object[] { typeof( PersonDriversLicenseStateRequired ), guarantor, compType } );
                types.Add( new object[] { typeof( PersonRelationshipPreferred ), guarantor, compType } );
                types.Add( new object[] { typeof( PersonRelationshipRequired ), guarantor, compType } );
                types.Add( new object[] { typeof( GuarantorConsentRequired ), anAccount, compType });
                types.Add( new object[] { typeof( GuarantorConsentPreferred ), anAccount, compType });

                PhoneNumber guarantorMailingPhoneNumber =
                    anAccount.Guarantor.ContactPointWith( TypeOfContactPoint.NewMailingContactPointType() ).PhoneNumber;
                PhoneNumber guarantorCellPhoneNumber =
                    anAccount.Guarantor.ContactPointWith( TypeOfContactPoint.NewMobileContactPointType() ).PhoneNumber;

                types.Add( new object[] { typeof( AreaCodeRequiresPhoneNumber ), guarantorMailingPhoneNumber, compType } );
                types.Add( new object[] { typeof( AreaCodeRequiresPhoneNumber ), guarantorCellPhoneNumber, compType } );
            }


            if ( primaryCoverage != null
             && primaryCoverage.Insured != null )
            {
                Insured insured = primaryCoverage.Insured;
                Type compType = typeof( OnInsuredFormForPrimaryInsurance );

                types.Add( new object[] { typeof( PersonRelationshipRequired ), insured, compType } );
                if ( !IsShortPreRegistrationAccount( anAccount ) )
                {
                    types.Add(new object[] {typeof (PersonLastNameRequired), insured, compType});
                    types.Add(new object[] {typeof (PersonFirstNameRequired), insured, compType});
                }
                types.Add( new object[] { typeof( PersonGenderRequired ), insured, compType } );
                types.Add( new object[] { typeof( PersonDateOfBirthRequired ), insured, compType } );
                types.Add( new object[] { typeof( InsuredAddressRequired ), insured, compType } );
                types.Add( new object[] { typeof( PersonEmploymentStatusRequired ), insured, compType } );
                types.Add( new object[] { typeof( InsuredEmployerRequired ), insured, compType } );
                types.Add( new object[] { typeof( PersonEmployerAddressRequired ), insured, compType } );
                types.Add( new object[] { typeof( PersonRelationshipPreferred ), insured, compType } );
                types.Add( new object[] { typeof( PersonLastNamePreferred ), insured, compType } );
                types.Add( new object[] { typeof( PersonFirstNamePreferred ), insured, compType } );
                types.Add( new object[] { typeof( PersonDateOfBirthPreferred ), insured, compType } );
                types.Add( new object[] { typeof( PersonGenderPreferred ), insured, compType } );
                types.Add( new object[] { typeof( InsuredAddressPreferred ), insured, compType } );
                types.Add( new object[] { typeof( PersonPhoneNumberPreferred ), insured, compType } );
                types.Add( new object[] { typeof( PersonPhoneAreaCodePreferred ), insured, compType } );
                types.Add( new object[] { typeof( PersonEmploymentStatusPreferred ), insured, compType } );
                types.Add( new object[] { typeof( EmploymentPhoneNumberPreferred ), insured.Employment, compType } );
                types.Add( new object[] { typeof( EmploymentPhoneAreaCodePreferred ), insured.Employment, compType } );

                addContextItem( typeof( AdmitDateToPrimaryPlanDates ), anAccount, "demographicsView", null );
                registerEvent( typeof( AdmitDateToPrimaryPlanDates ), null, null );

                if ( insured != null )
                {
                    ContactPoint contactPoint =
                        insured.ContactPointWith( TypeOfContactPoint.NewPhysicalContactPointType() );

                    if ( contactPoint != null )
                    {
                        types.Add( new object[] { typeof( AreaCodeRequiresPhoneNumber ), contactPoint.PhoneNumber, compType } );
                    }

                    contactPoint = insured.ContactPointWith( TypeOfContactPoint.NewMobileContactPointType() );

                    if ( contactPoint != null )
                    {
                        types.Add( new object[] { typeof( AreaCodeRequiresPhoneNumber ), contactPoint.PhoneNumber, compType } );
                    }

                    if ( insured.Employment != null
                        && insured.Employment.Employer != null
                        && insured.Employment.Employer.PartyContactPoint != null )
                    {
                        contactPoint = insured.Employment.Employer.PartyContactPoint;

                        types.Add( new object[] { typeof( AreaCodeRequiresPhoneNumber ), contactPoint.PhoneNumber, compType } );
                    }
                }

                if ( primaryCoverage.InsurancePlan != null
                    && primaryCoverage.BillingInformation != null )
                {
                    compType = typeof( OnPayorDetailsFormForPrimaryPayor );

                    types.Add( new object[]
                                  {
                                      typeof (AreaCodeRequiresPhoneNumber), primaryCoverage.BillingInformation.PhoneNumber,
                                      compType
                                  } );
                }

                if ( primaryCoverage is CoverageGroup )
                {
                    if ( ( primaryCoverage as CoverageGroup ).Authorization != null )
                    {
                        compType = typeof( OnVerificationFormForPrimaryInsurance );

                        types.Add( new object[]
                                      {
                                          typeof (AreaCodeRequiresPhoneNumber),
                                          (primaryCoverage as CoverageGroup).Authorization.AuthorizationPhone, compType
                                      } );
                    }
                }

                if ( primaryCoverage.Attorney != null )
                {
                    ContactPoint contactPoint =
                        primaryCoverage.Attorney.ContactPointWith( TypeOfContactPoint.NewBusinessContactPointType() );

                    compType = typeof( OnVerificationFormForPrimaryInsurance );

                    types.Add( new object[] { typeof( AreaCodeRequiresPhoneNumber ), contactPoint.PhoneNumber, compType } );
                }

                if ( primaryCoverage.InsuranceAgent != null )
                {
                    ContactPoint contactPoint =
                        primaryCoverage.InsuranceAgent.ContactPointWith( TypeOfContactPoint.NewBusinessContactPointType() );

                    compType = typeof( OnVerificationFormForPrimaryInsurance );

                    types.Add( new object[] { typeof( AreaCodeRequiresPhoneNumber ), contactPoint.PhoneNumber, compType } );
                }
                if ( primaryCoverage is CoverageGroup )
                {
                    if ( ( primaryCoverage as CoverageGroup ).Authorization != null )
                    {
                        compType = typeof( OnAuthorizationFormForPrimaryCoverage );
                        types.Add( new object[] { typeof( CompanyRepFirstNamePreferred ), primaryCoverage, compType } );
                        types.Add( new object[] { typeof( CompanyRepLastNamePreferred ), primaryCoverage, compType } );
                        types.Add( new object[] { typeof( ServicesAuthorizedPreferred ), primaryCoverage, compType } );
                        types.Add( new object[] { typeof( WorkersCompAuthCodePreferred ), primaryCoverage, compType } );
                    }
                }
            }

            if ( secondaryCoverage != null
                && secondaryCoverage.Insured != null )
            {
                Insured insured = secondaryCoverage.Insured;
                Type compType = typeof( OnInsuredFormForSecondaryInsurance );

                types.Add( new object[] { typeof( PersonRelationshipRequired ), insured, compType } );
                if ( !IsShortPreRegistrationAccount( anAccount ) )
                {
                    types.Add(new object[] {typeof (PersonLastNameRequired), insured, compType});
                    types.Add(new object[] {typeof (PersonFirstNameRequired), insured, compType});
                }
                types.Add( new object[] { typeof( PersonGenderRequired ), insured, compType } );
                types.Add( new object[] { typeof( PersonDateOfBirthRequired ), insured, compType } );
                types.Add( new object[] { typeof( InsuredAddressRequired ), insured, compType } );
                types.Add( new object[] { typeof( PersonEmploymentStatusRequired ), insured, compType } );
                types.Add( new object[] { typeof( InsuredEmployerRequired ), insured, compType } );
                types.Add( new object[] { typeof( PersonEmployerAddressRequired ), insured, compType } );
                types.Add( new object[] { typeof( PersonRelationshipPreferred ), insured, compType } );
                types.Add( new object[] { typeof( PersonLastNamePreferred ), insured, compType } );
                types.Add( new object[] { typeof( PersonFirstNamePreferred ), insured, compType } );
                types.Add( new object[] { typeof( PersonDateOfBirthPreferred ), insured, compType } );
                types.Add( new object[] { typeof( PersonGenderPreferred ), insured, compType } );
                types.Add( new object[] { typeof( InsuredAddressPreferred ), insured, compType } );
                types.Add( new object[] { typeof( PersonPhoneNumberPreferred ), insured, compType } );
                types.Add( new object[] { typeof( PersonPhoneAreaCodePreferred ), insured, compType } );
                types.Add( new object[] { typeof( PersonEmploymentStatusPreferred ), insured, compType } );
                types.Add( new object[] { typeof( EmploymentPhoneNumberPreferred ), insured.Employment, compType } );
                types.Add( new object[] { typeof( EmploymentPhoneAreaCodePreferred ), insured.Employment, compType } );

                addContextItem( typeof( AdmitDateToSecondaryPlanDates ), anAccount, "demographicsView", null );
                registerEvent( typeof( AdmitDateToSecondaryPlanDates ), null, null );

                if ( insured != null )
                {
                    ContactPoint contactPoint =
                        insured.ContactPointWith( TypeOfContactPoint.NewPhysicalContactPointType() );

                    if ( contactPoint != null )
                    {
                        types.Add( new object[] { typeof( AreaCodeRequiresPhoneNumber ), contactPoint.PhoneNumber, compType } );
                    }

                    contactPoint = insured.ContactPointWith( TypeOfContactPoint.NewMobileContactPointType() );

                    if ( contactPoint != null )
                    {
                        types.Add( new object[] { typeof( AreaCodeRequiresPhoneNumber ), contactPoint.PhoneNumber, compType } );
                    }

                    if ( insured.Employment != null
                        && insured.Employment.Employer != null
                        && insured.Employment.Employer.PartyContactPoint != null )
                    {
                        contactPoint = insured.Employment.Employer.PartyContactPoint;

                        types.Add( new object[] { typeof( AreaCodeRequiresPhoneNumber ), contactPoint.PhoneNumber, compType } );
                    }
                }

                if ( secondaryCoverage.InsurancePlan != null
                    && secondaryCoverage.BillingInformation != null )
                {
                    compType = typeof( OnPayorDetailsFormForPrimaryPayor );

                    types.Add( new object[]
                                  {
                                      typeof (AreaCodeRequiresPhoneNumber),
                                      secondaryCoverage.BillingInformation.PhoneNumber, compType
                                  } );
                }


                if ( secondaryCoverage is CoverageGroup )
                {
                    if ( ( secondaryCoverage as CoverageGroup ).Authorization != null )
                    {
                        compType = typeof( OnVerificationFormForSecondaryInsurance );

                        types.Add( new object[]
                                      {
                                          typeof (AreaCodeRequiresPhoneNumber),
                                          (secondaryCoverage as CoverageGroup).Authorization.AuthorizationPhone, compType
                                      } );
                    }
                }

                if ( secondaryCoverage.Attorney != null )
                {
                    ContactPoint contactPoint =
                        secondaryCoverage.Attorney.ContactPointWith( TypeOfContactPoint.NewBusinessContactPointType() );

                    compType = typeof( OnVerificationFormForPrimaryInsurance );

                    types.Add( new object[] { typeof( AreaCodeRequiresPhoneNumber ), contactPoint.PhoneNumber, compType } );
                }

                if ( secondaryCoverage.InsuranceAgent != null )
                {
                    ContactPoint contactPoint =
                        secondaryCoverage.InsuranceAgent.ContactPointWith(
                            TypeOfContactPoint.NewBusinessContactPointType() );

                    compType = typeof( OnVerificationFormForPrimaryInsurance );

                    types.Add( new object[] { typeof( AreaCodeRequiresPhoneNumber ), contactPoint.PhoneNumber, compType } );
                }
                if ( secondaryCoverage is CoverageGroup )
                {
                    if ( ( secondaryCoverage as CoverageGroup ).Authorization != null )
                    {
                        compType = typeof( OnAuthorizationFormForSecondaryCoverage );
                        types.Add( new object[] { typeof( CompanyRepFirstNamePreferred ), secondaryCoverage, compType } );
                        types.Add( new object[] { typeof( CompanyRepLastNamePreferred ), secondaryCoverage, compType } );
                        types.Add( new object[] { typeof( ServicesAuthorizedPreferred ), secondaryCoverage, compType } );
                        types.Add( new object[] { typeof( WorkersCompAuthCodePreferred ), primaryCoverage, compType } );
                    }
                }
            }

             
             ///////

            // Contact - these generic rules are implemented for:
            //   Primary contact

            if ( anAccount.EmergencyContact1 != null )
            {
                EmergencyContact econtact = anAccount.EmergencyContact1;
                Type compType = typeof( OnShortDemographicsForm );

                types.Add( new object[] { typeof( ContactNameRequired ), econtact, compType } );
                types.Add( new object[] { typeof( ContactRelationshipRequired ), econtact, compType } );
                types.Add( new object[] { typeof( ContactAreaCodeRequired ), econtact, compType } );
                types.Add( new object[] { typeof( ContactPhoneRequired ), econtact, compType } );
                types.Add( new object[] { typeof( ContactNamePreferred ), econtact, compType } );
                types.Add( new object[] { typeof( ContactRelationshipPreferred ), econtact, compType } );
                types.Add( new object[] { typeof( ContactAreaCodePreferred ), econtact, compType } );
                types.Add( new object[] { typeof( ContactPhonePreferred ), econtact, compType } );
                types.Add( new object[] { typeof( ContactAddressPreferred ), econtact, compType } );

                types.Add( new object[]
                              {
                                  typeof (AreaCodeRequiresPhoneNumber),
                                  econtact.ContactPointWith(TypeOfContactPoint.NewPhysicalContactPointType()).PhoneNumber,
                                  compType
                              } );
            }
            if (anAccount != null)
            {

                Type compType = typeof (OnPaymentForm);

                types.Add(new object[]
                {
                    typeof (MonthlyDueDateRequired),
                    anAccount,
                    compType
                });
            }

            if ( primaryCoverage != null )
            {
                Type compType = typeof( OnPayorDetailsFormForPrimaryPayor );

                types.Add( new object[] { typeof( MedicaidIssueDateRequired ), primaryCoverage, compType } );
                types.Add( new object[] { typeof( MedicaidPolicyCINNumberRequired ), primaryCoverage, compType } );
                
                types.Add(new object[] { typeof(MBINumberRequired), primaryCoverage, compType });
                types.Add( new object[] { typeof( InsuranceAuthorizationRequiredPreferred ), primaryCoverage, compType } );
                types.Add( new object[] { typeof( InsurancePlanSSNRequired ), primaryCoverage, compType } );
            
                types.Add( new object[] { typeof( WorkersCompEmpSupervisorRequired ), primaryCoverage, compType } );
                types.Add( new object[] { typeof( WorkersCompPolicyNumberRequired ), primaryCoverage, compType } );

                types.Add( new object[] { typeof( MedicaidIssueDatePreferred ), primaryCoverage, compType } );
                types.Add( new object[] { typeof( MedicaidPolicyCINNumberPreferred ), primaryCoverage, compType } );
               
                types.Add(new object[] { typeof(MBINumberPreferred), primaryCoverage, compType });
                types.Add( new object[] { typeof( InsurancePlanSSNPreferred ), primaryCoverage, compType } );
                types.Add( new object[] { typeof( WorkersCompEmpSupervisorPreferred ), primaryCoverage, compType } );
                types.Add( new object[] { typeof( WorkersCompPolicyNumberPreferred ), primaryCoverage, compType } );


                types.Add( new object[] { typeof( BillingCONameRequired ), primaryCoverage, compType } );
                types.Add( new object[] { typeof( BillingNameRequired ), primaryCoverage, compType } );
                types.Add( new object[] { typeof( BillingAddressRequired ), primaryCoverage.BillingInformation, compType } );
                types.Add( new object[] { typeof( BillingPhoneRequired ), primaryCoverage, compType } );

                types.Add( new object[] { typeof( BillingCONamePreferred ), primaryCoverage, compType } );
                types.Add( new object[] { typeof( BillingNamePreferred ), primaryCoverage, compType } );
                types.Add( new object[] { typeof( BillingAddressPreferred ), primaryCoverage.BillingInformation, compType } );
                types.Add( new object[] { typeof( BillingPhonePreferred ), primaryCoverage, compType } );

                types.Add( new object[] { typeof( InsurancePlanIPARequired ), primaryCoverage, compType } );
                types.Add(new object[] { typeof(DOFRInsurancePartIPARequired), primaryCoverage, compType });
                types.Add(new object[] { typeof(DOFRAidCodeRequired), primaryCoverage, compType });
            }

            if ( secondaryCoverage != null )
            {
                Type compType = typeof( OnPayorDetailsFormForSecondaryPayor );

                types.Add( new object[] { typeof( MedicaidIssueDateRequired ), secondaryCoverage, compType } );
                types.Add( new object[] { typeof( MedicaidPolicyCINNumberRequired ), secondaryCoverage, compType } );
                 
                types.Add(new object[] { typeof(MBINumberRequired), secondaryCoverage, compType });
                types.Add( new object[] { typeof( InsuranceAuthorizationRequiredPreferred ), secondaryCoverage, compType } );
                types.Add( new object[] { typeof( InsurancePlanSSNRequired ), secondaryCoverage, compType } );
                
                types.Add( new object[] { typeof( WorkersCompEmpSupervisorRequired ), secondaryCoverage, compType } );
                types.Add( new object[] { typeof( WorkersCompPolicyNumberRequired ), secondaryCoverage, compType } );

                types.Add( new object[] { typeof( MedicaidIssueDatePreferred ), secondaryCoverage, compType } );
                types.Add( new object[] { typeof( MedicaidPolicyCINNumberPreferred ), secondaryCoverage, compType } );
               
                types.Add(new object[] { typeof(MBINumberPreferred), secondaryCoverage, compType });
                types.Add( new object[] { typeof( InsurancePlanSSNPreferred ), secondaryCoverage, compType } );
                types.Add( new object[] { typeof( WorkersCompEmpSupervisorPreferred ), secondaryCoverage, compType } );
                types.Add( new object[] { typeof( WorkersCompPolicyNumberPreferred ), secondaryCoverage, compType } );


                types.Add( new object[] { typeof( BillingCONameRequired ), secondaryCoverage, compType } );
                types.Add( new object[] { typeof( BillingNameRequired ), secondaryCoverage, compType } );
                types.Add( new object[] { typeof( BillingAddressRequired ), secondaryCoverage.BillingInformation, compType } );
                types.Add( new object[] { typeof( BillingPhoneRequired ), secondaryCoverage, compType } );

                types.Add( new object[] { typeof( BillingCONamePreferred ), secondaryCoverage, compType } );
                types.Add( new object[] { typeof( BillingNamePreferred ), secondaryCoverage, compType } );
                types.Add( new object[] { typeof( BillingAddressPreferred ), secondaryCoverage.BillingInformation, compType } );
                types.Add( new object[] { typeof( BillingPhonePreferred ), secondaryCoverage, compType } );

                types.Add( new object[] { typeof( InsurancePlanIPARequired ), secondaryCoverage, compType } );
            }
           
            // Store the rule type (RULETYPE), the context on which it applies (CONTEXT), 
            // the composite rule type (COMPOSITE), and the related "control" (CONTROL).
            // The "control" here is used to pass in the Activity value necessary for rule evaluation

            var nonStaffPhysicianRules = new RuleArrayList();

            // Referring NonStaff Physician
            if ( anAccount.ReferringPhysician != null )
            {
                Physician referringPhysician = anAccount.ReferringPhysician;
                Type compType = typeof( OnReferringNonStaffPhysicianForm );

                nonStaffPhysicianRules.Add( new object[]
                                               {
                                                   typeof (NonStaffPhysicianNPIRequired), referringPhysician, compType,
                                                   anAccount.Activity
                                               } );
            }

            // Admitting NonStaff Physician
            if ( anAccount.AdmittingPhysician != null )
            {
                Physician admittingPhysician = anAccount.AdmittingPhysician;
                Type compType = typeof( OnAdmittingNonStaffPhysicianForm );

                nonStaffPhysicianRules.Add( new object[]
                                               {
                                                   typeof (NonStaffPhysicianNPIRequired), admittingPhysician, compType,
                                                   anAccount.Activity
                                               } );
            }

            // Attending NonStaff Physician
            if ( anAccount.AttendingPhysician != null )
            {
                Physician attendingPhysician = anAccount.AttendingPhysician;
                Type compType = typeof( OnAttendingNonStaffPhysicianForm );

                nonStaffPhysicianRules.Add( new object[]
                                               {
                                                   typeof (NonStaffPhysicianNPIRequired), attendingPhysician, compType,
                                                   anAccount.Activity
                                               } );
            }

            // Operating NonStaff Physician
            if ( anAccount.OperatingPhysician != null )
            {
                Physician operatingPhysician = anAccount.OperatingPhysician;
                Type compType = typeof( OnOperatingNonStaffPhysicianForm );

                nonStaffPhysicianRules.Add( new object[]
                                               {
                                                   typeof (NonStaffPhysicianNPIRequired), operatingPhysician, compType,
                                                   anAccount.Activity
                                               } );
            }

            // PrimaryCare NonStaff Physician
            if ( anAccount.PrimaryCarePhysician != null )
            {
                Physician primaryCarePhysician = anAccount.PrimaryCarePhysician;
                Type compType = typeof( OnPCPNonStaffPhysicianForm );

                nonStaffPhysicianRules.Add( new object[]
                                               {
                                                   typeof (NonStaffPhysicianNPIRequired), primaryCarePhysician, compType,
                                                   anAccount.Activity
                                               } );
            }

            c_log.Debug( "  evaluateAllRules - finished loading types collection" );

            // register all of the events we've just stored
            foreach ( object[] type in types )
            {
                addContextItem( (Type)type[RULETYPE], type[CONTEXT], (Type)type[COMPOSITE] );
                registerEvent( (Type)type[RULETYPE], null, (Type)type[COMPOSITE] );
            }

            foreach ( object[] type in nonStaffPhysicianRules )
            {
                addContextItem( (Type)type[RULETYPE], type[CONTEXT], type[CONTROL], (Type)type[COMPOSITE] );
                registerEvent( (Type)type[RULETYPE], null, (Type)type[COMPOSITE] );
            }

            c_log.Debug( "  evaluateAllRules - finished registering events" );

            // Physician - these are NOT registered here.  We assume that if
            //     the AttendingPhysicianRequired, AdmittingPhysicianRequired, and ReferringPhysicianRequired
            //     rules pass (those are Account-based, non-generic rules), then these will all have passed.

            // 3. run the rules

            blnSingleRunRule = false;

            blnRC = evaluateRules( anAccount );

            c_log.Debug( "  evaluateAllRules - finished running the rules" );

            // 4. remove context items we added
            foreach ( object[] type in types )
            {
                removeContextItem( (Type)type[RULETYPE], type[CONTEXT] );
                unregisterEvent( (Type)type[RULETYPE], null );
            }

            c_log.Debug( "  evaluateAllRules - context items removed (again)" );

            // manually remove the extraordinary ones
            removeContextItem( typeof( AdmitDateToPrimaryPlanDates ), anAccount );
            removeContextItem( typeof( AdmitDateToSecondaryPlanDates ), anAccount );

            blnAllErrors = false;
            blnFireEvents = true;

            return blnRC;
        }
        private bool evaluateAllRulesForQuickAccountRegistration( Account anAccount )
        {
            c_log.Debug( "Start - evaluateAllRules" );

            blnAllErrors = true;
            blnFireEvents = false;
            blnSingleRunRule = false;

            bool blnRC = false;

            // 1. load the rules

            LoadRules( anAccount );

            c_log.Debug( "  evaluateAllRules - Rules loaded" );
            // 2. register context items

            // set up the coverages for Primary and Secondary

            if ( anAccount == null )
            {
                blnAllErrors = false;
                blnFireEvents = true;
                return false;
            }
            Coverage primaryCoverage = null;
           
            if ( anAccount.Insurance != null )
            {
                primaryCoverage = anAccount.Insurance.CoverageFor( CoverageOrder.PRIMARY_OID );
                if ( primaryCoverage != null )
                {
                    primaryCoverage.Account = anAccount;
                }

              
            }
            removeAllContextItems();

            c_log.Debug( "  evaluateAllRules - context items removed" );

          

            if ( anAccount.Insurance != null )
            {
                primaryCoverage = anAccount.Insurance.CoverageFor(CoverageOrder.PRIMARY_OID);
                if (primaryCoverage != null)
                {
                    primaryCoverage.Account = anAccount;
                }
            }


            // Store the rule type (RULETYPE), the context on which it applies (CONTEXT), 
            // and the composite type (COMPOSITE)

            var types = new RuleArrayList();

            c_log.Debug( "  evaluateAllRules - loading types collection" );

            Type demogCompType = typeof( OnQuickAccountCreationForm );
            
            types.Add( new object[] { typeof( SocialSecurityNumberRequired ), anAccount.Patient, demogCompType } );


            if ( anAccount.AllPhysicianRelationships != null
                 && anAccount.AllPhysicianRelationships.Count > 0 )
            {
                foreach ( PhysicianRelationship physicianRelationship in anAccount.AllPhysicianRelationships )
                {
                    if ( physicianRelationship.Physician != null
                         && physicianRelationship.Physician.PhysicianNumber == NON_STAFF_PHYSICIAN_NUMBER )
                    {
                        demogCompType = typeof( OnClinicalForm );

                        PhoneNumber physicianPhone = physicianRelationship.Physician.PhoneNumber;

                        types.Add( new object[] { typeof( AreaCodeRequiresPhoneNumber ), physicianPhone, demogCompType } );
                    }
                }
            }

            if ( primaryCoverage != null
                && primaryCoverage.Insured != null )
            {
                Insured insured = primaryCoverage.Insured;
                Type compType = typeof( OnInsuredFormForPrimaryInsurance );

                types.Add( new object[] { typeof( PersonRelationshipRequired ), insured, compType } );
                types.Add( new object[] { typeof( PersonLastNameRequired ), insured, compType } );
                types.Add( new object[] { typeof( PersonFirstNameRequired ), insured, compType } );
                types.Add( new object[] { typeof( PersonGenderRequired ), insured, compType } );
                types.Add( new object[] { typeof( PersonDateOfBirthRequired ), insured, compType } );
                types.Add( new object[] { typeof( InsuredAddressRequired ), insured, compType } );
                types.Add( new object[] { typeof( PersonEmploymentStatusRequired ), insured, compType } );
                types.Add( new object[] { typeof( InsuredEmployerRequired ), insured, compType } );
                types.Add( new object[] { typeof( PersonEmployerAddressRequired ), insured, compType } );
                types.Add( new object[] { typeof( PersonRelationshipPreferred ), insured, compType } );
                types.Add( new object[] { typeof( PersonLastNamePreferred ), insured, compType } );
                types.Add( new object[] { typeof( PersonFirstNamePreferred ), insured, compType } );
                types.Add( new object[] { typeof( PersonDateOfBirthPreferred ), insured, compType } );
                types.Add( new object[] { typeof( PersonGenderPreferred ), insured, compType } );
                types.Add( new object[] { typeof( InsuredAddressPreferred ), insured, compType } );
                types.Add( new object[] { typeof( PersonPhoneNumberPreferred ), insured, compType } );
                types.Add( new object[] { typeof( PersonPhoneAreaCodePreferred ), insured, compType } );
                types.Add( new object[] { typeof( PersonEmploymentStatusPreferred ), insured, compType } );
                types.Add( new object[] { typeof( EmploymentPhoneNumberPreferred ), insured.Employment, compType } );
                types.Add( new object[] { typeof( EmploymentPhoneAreaCodePreferred ), insured.Employment, compType } );

                addContextItem( typeof( AdmitDateToPrimaryPlanDates ), anAccount, "demographicsView", null );
                registerEvent( typeof( AdmitDateToPrimaryPlanDates ), null, null );

                if ( insured != null )
                {
                    ContactPoint contactPoint =
                        insured.ContactPointWith( TypeOfContactPoint.NewPhysicalContactPointType() );

                    if ( contactPoint != null )
                    {
                        types.Add( new object[] { typeof( AreaCodeRequiresPhoneNumber ), contactPoint.PhoneNumber, compType } );
                    }

                    contactPoint = insured.ContactPointWith( TypeOfContactPoint.NewMobileContactPointType() );

                    if ( contactPoint != null )
                    {
                        types.Add( new object[] { typeof( AreaCodeRequiresPhoneNumber ), contactPoint.PhoneNumber, compType } );
                    }

                    if ( insured.Employment != null
                        && insured.Employment.Employer != null
                        && insured.Employment.Employer.PartyContactPoint != null )
                    {
                        contactPoint = insured.Employment.Employer.PartyContactPoint;

                        types.Add( new object[] { typeof( AreaCodeRequiresPhoneNumber ), contactPoint.PhoneNumber, compType } );
                    }
                }

                if ( primaryCoverage.InsurancePlan != null
                    && primaryCoverage.BillingInformation != null )
                {
                    compType = typeof( OnPayorDetailsFormForPrimaryPayor );

                    types.Add( new object[]
                                  {
                                      typeof (AreaCodeRequiresPhoneNumber), primaryCoverage.BillingInformation.PhoneNumber,
                                      compType
                                  } );
                }

                if ( primaryCoverage is CoverageGroup )
                {
                    if ( ( primaryCoverage as CoverageGroup ).Authorization != null )
                    {
                        compType = typeof( OnVerificationFormForPrimaryInsurance );

                        types.Add( new object[]
                                      {
                                          typeof (AreaCodeRequiresPhoneNumber),
                                          (primaryCoverage as CoverageGroup).Authorization.AuthorizationPhone, compType
                                      } );
                    }
                }

                if ( primaryCoverage.Attorney != null )
                {
                    ContactPoint contactPoint =
                        primaryCoverage.Attorney.ContactPointWith( TypeOfContactPoint.NewBusinessContactPointType() );

                    compType = typeof( OnVerificationFormForPrimaryInsurance );

                    types.Add( new object[] { typeof( AreaCodeRequiresPhoneNumber ), contactPoint.PhoneNumber, compType } );
                }

                if ( primaryCoverage.InsuranceAgent != null )
                {
                    ContactPoint contactPoint =
                        primaryCoverage.InsuranceAgent.ContactPointWith( TypeOfContactPoint.NewBusinessContactPointType() );

                    compType = typeof( OnVerificationFormForPrimaryInsurance );

                    types.Add( new object[] { typeof( AreaCodeRequiresPhoneNumber ), contactPoint.PhoneNumber, compType } );
                }
                if ( primaryCoverage is CoverageGroup )
                {
                    if ( ( primaryCoverage as CoverageGroup ).Authorization != null )
                    {
                        compType = typeof( OnAuthorizationFormForPrimaryCoverage );
                        types.Add( new object[] { typeof( CompanyRepFirstNamePreferred ), primaryCoverage, compType } );
                        types.Add( new object[] { typeof( CompanyRepLastNamePreferred ), primaryCoverage, compType } );
                        types.Add( new object[] { typeof( ServicesAuthorizedPreferred ), primaryCoverage, compType } );
                        types.Add( new object[] { typeof( WorkersCompAuthCodePreferred ), primaryCoverage, compType } );
                    }
                }
            }

            if ( primaryCoverage != null )
            {
                Type compType = typeof( OnPayorDetailsFormForPrimaryPayor );

                types.Add( new object[] { typeof( MedicaidIssueDateRequired ), primaryCoverage, compType } );
                types.Add( new object[] { typeof( MedicaidPolicyCINNumberRequired ), primaryCoverage, compType } );
               
                types.Add(new object[] { typeof(MBINumberRequired), primaryCoverage, compType });
                types.Add( new object[] { typeof( InsuranceAuthorizationRequiredPreferred ), primaryCoverage, compType } );
                types.Add( new object[] { typeof( InsurancePlanSSNRequired ), primaryCoverage, compType } );
           
                types.Add( new object[] { typeof( WorkersCompEmpSupervisorRequired ), primaryCoverage, compType } );
                types.Add( new object[] { typeof( WorkersCompPolicyNumberRequired ), primaryCoverage, compType } );

                types.Add( new object[] { typeof( MedicaidIssueDatePreferred ), primaryCoverage, compType } );
                types.Add( new object[] { typeof( MedicaidPolicyCINNumberPreferred ), primaryCoverage, compType } );
        
                types.Add(new object[] { typeof(MBINumberPreferred), primaryCoverage, compType });
                types.Add( new object[] { typeof( InsurancePlanSSNPreferred ), primaryCoverage, compType } );
                types.Add( new object[] { typeof( WorkersCompEmpSupervisorPreferred ), primaryCoverage, compType } );
                types.Add( new object[] { typeof( WorkersCompPolicyNumberPreferred ), primaryCoverage, compType } );


                types.Add( new object[] { typeof( BillingCONameRequired ), primaryCoverage, compType } );
                types.Add( new object[] { typeof( BillingNameRequired ), primaryCoverage, compType } );
                types.Add( new object[] { typeof( BillingAddressRequired ), primaryCoverage.BillingInformation, compType } );
                types.Add( new object[] { typeof( BillingPhoneRequired ), primaryCoverage, compType } );

                types.Add( new object[] { typeof( BillingCONamePreferred ), primaryCoverage, compType } );
                types.Add( new object[] { typeof( BillingNamePreferred ), primaryCoverage, compType } );
                types.Add( new object[] { typeof( BillingAddressPreferred ), primaryCoverage.BillingInformation, compType } );
                types.Add( new object[] { typeof( BillingPhonePreferred ), primaryCoverage, compType } );

                types.Add( new object[] { typeof( InsurancePlanIPARequired ), primaryCoverage, compType } );
                types.Add(new object[] { typeof(DOFRInsurancePartIPARequired), primaryCoverage, compType });
                types.Add(new object[] { typeof(DOFRAidCodeRequired), primaryCoverage, compType });
            }

            // Store the rule type (RULETYPE), the context on which it applies (CONTEXT), 
            // the composite rule type (COMPOSITE), and the related "control" (CONTROL).
            // The "control" here is used to pass in the Activity value necessary for rule evaluation

            var nonStaffPhysicianRules = new RuleArrayList();

            // Referring NonStaff Physician
            if ( anAccount.ReferringPhysician != null )
            {
                Physician referringPhysician = anAccount.ReferringPhysician;
                Type compType = typeof( OnReferringNonStaffPhysicianForm );

                nonStaffPhysicianRules.Add( new object[]
                                               {
                                                   typeof (NonStaffPhysicianNPIRequired), referringPhysician, compType,
                                                   anAccount.Activity
                                               } );
            }

            // Admitting NonStaff Physician
            if ( anAccount.AdmittingPhysician != null )
            {
                Physician admittingPhysician = anAccount.AdmittingPhysician;
                Type compType = typeof( OnAdmittingNonStaffPhysicianForm );

                nonStaffPhysicianRules.Add( new object[]
                                               {
                                                   typeof (NonStaffPhysicianNPIRequired), admittingPhysician, compType,
                                                   anAccount.Activity
                                               } );
            }

            c_log.Debug( "  evaluateAllRules - finished loading types collection" );

            // register all of the events we've just stored
            foreach ( object[] type in types )
            {
                addContextItem( (Type)type[RULETYPE], type[CONTEXT], (Type)type[COMPOSITE] );
                registerEvent( (Type)type[RULETYPE], null, (Type)type[COMPOSITE] );
            }

            foreach ( object[] type in nonStaffPhysicianRules )
            {
                addContextItem( (Type)type[RULETYPE], type[CONTEXT], type[CONTROL], (Type)type[COMPOSITE] );
                registerEvent( (Type)type[RULETYPE], null, (Type)type[COMPOSITE] );
            }

            c_log.Debug( "  evaluateAllRules - finished registering events" );

            // Physician - these are NOT registered here.  We assume that if
            //     the AttendingPhysicianRequired, AdmittingPhysicianRequired, and ReferringPhysicianRequired
            //     rules pass (those are Account-based, non-generic rules), then these will all have passed.

            // 3. run the rules

            blnSingleRunRule = false;

            blnRC = evaluateRules( anAccount );

            c_log.Debug( "  evaluateAllRules - finished running the rules" );

            // 4. remove context items we added
            foreach ( object[] type in types )
            {
                removeContextItem( (Type)type[RULETYPE], type[CONTEXT] );
                unregisterEvent( (Type)type[RULETYPE], null );
            }

            c_log.Debug( "  evaluateAllRules - context items removed (again)" );

            // manually remove the extraordinary ones
            removeContextItem( typeof( AdmitDateToPrimaryPlanDates ), anAccount );
            removeContextItem( typeof( AdmitDateToSecondaryPlanDates ), anAccount );

            blnAllErrors = false;
            blnFireEvents = true;

            return blnRC;
        }

        private bool EvaluateAllRulesForPAIWalkinAccountRegistration(Account anAccount)
        {
            c_log.Debug("Start - evaluateAllRules");

            blnAllErrors = true;
            blnFireEvents = false;
            blnSingleRunRule = false;

            bool blnRC = false;

            // 1. load the rules

            LoadRules(anAccount);

            c_log.Debug("  evaluateAllRules - Rules loaded");
            // 2. register context items

            // set up the coverages for Primary and Secondary

            if (anAccount == null)
            {
                blnAllErrors = false;
                blnFireEvents = true;
                return false;
            }
            Coverage primaryCoverage = null;

            if (anAccount.Insurance != null)
            {
                primaryCoverage = anAccount.Insurance.CoverageFor(CoverageOrder.PRIMARY_OID);
                if (primaryCoverage != null)
                {
                    primaryCoverage.Account = anAccount;
                }


            }
            removeAllContextItems();

            c_log.Debug("  evaluateAllRules - context items removed");



            if (anAccount.Insurance != null)
            {
                primaryCoverage = anAccount.Insurance.CoverageFor(CoverageOrder.PRIMARY_OID);
                if (primaryCoverage != null)
                {
                    primaryCoverage.Account = anAccount;
                }
            }


            // Store the rule type (RULETYPE), the context on which it applies (CONTEXT), 
            // and the composite type (COMPOSITE)

            var types = new RuleArrayList();

            c_log.Debug("  evaluateAllRules - loading types collection");

            Type demogCompType = typeof(OnPAIWalkinAccountCreationForm);

            types.Add(new object[] { typeof(SocialSecurityNumberRequired), anAccount.Patient, demogCompType });

            if (anAccount.AllPhysicianRelationships != null
                 && anAccount.AllPhysicianRelationships.Count > 0)
            {
                foreach (PhysicianRelationship physicianRelationship in anAccount.AllPhysicianRelationships)
                {
                    if (physicianRelationship.Physician != null
                         && physicianRelationship.Physician.PhysicianNumber == NON_STAFF_PHYSICIAN_NUMBER)
                    {
                        demogCompType = typeof(OnClinicalForm);

                        PhoneNumber physicianPhone = physicianRelationship.Physician.PhoneNumber;

                        types.Add(new object[] { typeof(AreaCodeRequiresPhoneNumber), physicianPhone, demogCompType });
                    }
                }
            }

            if (primaryCoverage != null
                && primaryCoverage.Insured != null)
            {
                Insured insured = primaryCoverage.Insured;
                Type compType = typeof(OnInsuredFormForPrimaryInsurance);

                types.Add(new object[] { typeof(PersonRelationshipRequired), insured, compType });
                types.Add(new object[] { typeof(PersonLastNameRequired), insured, compType });
                types.Add(new object[] { typeof(PersonFirstNameRequired), insured, compType });
                types.Add(new object[] { typeof(PersonGenderRequired), insured, compType });
                types.Add(new object[] { typeof(PersonDateOfBirthRequired), insured, compType });
                types.Add(new object[] { typeof(InsuredAddressRequired), insured, compType });
                types.Add(new object[] { typeof(PersonEmploymentStatusRequired), insured, compType });
                types.Add(new object[] { typeof(InsuredEmployerRequired), insured, compType });
                types.Add(new object[] { typeof(PersonEmployerAddressRequired), insured, compType });
                types.Add(new object[] { typeof(PersonRelationshipPreferred), insured, compType });
                types.Add(new object[] { typeof(PersonLastNamePreferred), insured, compType });
                types.Add(new object[] { typeof(PersonFirstNamePreferred), insured, compType });
                types.Add(new object[] { typeof(PersonDateOfBirthPreferred), insured, compType });
                types.Add(new object[] { typeof(PersonGenderPreferred), insured, compType });
                types.Add(new object[] { typeof(InsuredAddressPreferred), insured, compType });
                types.Add(new object[] { typeof(PersonPhoneNumberPreferred), insured, compType });
                types.Add(new object[] { typeof(PersonPhoneAreaCodePreferred), insured, compType });
                types.Add(new object[] { typeof(PersonEmploymentStatusPreferred), insured, compType });
                types.Add(new object[] { typeof(EmploymentPhoneNumberPreferred), insured.Employment, compType });
                types.Add(new object[] { typeof(EmploymentPhoneAreaCodePreferred), insured.Employment, compType });

                addContextItem(typeof(AdmitDateToPrimaryPlanDates), anAccount, "demographicsView", null);
                registerEvent(typeof(AdmitDateToPrimaryPlanDates), null, null);

                if (insured != null)
                {
                    ContactPoint contactPoint =
                        insured.ContactPointWith(TypeOfContactPoint.NewPhysicalContactPointType());

                    if (contactPoint != null)
                    {
                        types.Add(new object[] { typeof(AreaCodeRequiresPhoneNumber), contactPoint.PhoneNumber, compType });
                    }

                    contactPoint = insured.ContactPointWith(TypeOfContactPoint.NewMobileContactPointType());

                    if (contactPoint != null)
                    {
                        types.Add(new object[] { typeof(AreaCodeRequiresPhoneNumber), contactPoint.PhoneNumber, compType });
                    }

                    if (insured.Employment != null
                        && insured.Employment.Employer != null
                        && insured.Employment.Employer.PartyContactPoint != null)
                    {
                        contactPoint = insured.Employment.Employer.PartyContactPoint;

                        types.Add(new object[] { typeof(AreaCodeRequiresPhoneNumber), contactPoint.PhoneNumber, compType });
                    }
                }

                if (primaryCoverage.InsurancePlan != null
                    && primaryCoverage.BillingInformation != null)
                {
                    compType = typeof(OnPayorDetailsFormForPrimaryPayor);

                    types.Add(new object[]
                                  {
                                      typeof (AreaCodeRequiresPhoneNumber), primaryCoverage.BillingInformation.PhoneNumber,
                                      compType
                                  });
                }

                if (primaryCoverage is CoverageGroup)
                {
                    if ((primaryCoverage as CoverageGroup).Authorization != null)
                    {
                        compType = typeof(OnVerificationFormForPrimaryInsurance);

                        types.Add(new object[]
                                      {
                                          typeof (AreaCodeRequiresPhoneNumber),
                                          (primaryCoverage as CoverageGroup).Authorization.AuthorizationPhone, compType
                                      });
                    }
                }

                if (primaryCoverage.Attorney != null)
                {
                    ContactPoint contactPoint =
                        primaryCoverage.Attorney.ContactPointWith(TypeOfContactPoint.NewBusinessContactPointType());

                    compType = typeof(OnVerificationFormForPrimaryInsurance);

                    types.Add(new object[] { typeof(AreaCodeRequiresPhoneNumber), contactPoint.PhoneNumber, compType });
                }

                if (primaryCoverage.InsuranceAgent != null)
                {
                    ContactPoint contactPoint =
                        primaryCoverage.InsuranceAgent.ContactPointWith(TypeOfContactPoint.NewBusinessContactPointType());

                    compType = typeof(OnVerificationFormForPrimaryInsurance);

                    types.Add(new object[] { typeof(AreaCodeRequiresPhoneNumber), contactPoint.PhoneNumber, compType });
                }
                if (primaryCoverage is CoverageGroup)
                {
                    if ((primaryCoverage as CoverageGroup).Authorization != null)
                    {
                        compType = typeof(OnAuthorizationFormForPrimaryCoverage);
                        types.Add(new object[] { typeof(CompanyRepFirstNamePreferred), primaryCoverage, compType });
                        types.Add(new object[] { typeof(CompanyRepLastNamePreferred), primaryCoverage, compType });
                        types.Add(new object[] { typeof(ServicesAuthorizedPreferred), primaryCoverage, compType });
                        types.Add(new object[] { typeof(WorkersCompAuthCodePreferred), primaryCoverage, compType });
                    }
                }
            }

            if (primaryCoverage != null)
            {
                Type compType = typeof(OnPayorDetailsFormForPrimaryPayor);

                types.Add(new object[] { typeof(MedicaidIssueDateRequired), primaryCoverage, compType });
                types.Add(new object[] { typeof(MedicaidPolicyCINNumberRequired), primaryCoverage, compType });
                
                types.Add(new object[] { typeof(MBINumberRequired), primaryCoverage, compType });
                types.Add(new object[] { typeof(InsuranceAuthorizationRequiredPreferred), primaryCoverage, compType });
                types.Add(new object[] { typeof(InsurancePlanSSNRequired), primaryCoverage, compType });
                
                types.Add(new object[] { typeof(WorkersCompEmpSupervisorRequired), primaryCoverage, compType });
                types.Add(new object[] { typeof(WorkersCompPolicyNumberRequired), primaryCoverage, compType });

                types.Add(new object[] { typeof(MedicaidIssueDatePreferred), primaryCoverage, compType });
                types.Add(new object[] { typeof(MedicaidPolicyCINNumberPreferred), primaryCoverage, compType });
                
                types.Add(new object[] { typeof(MBINumberPreferred), primaryCoverage, compType });
                types.Add(new object[] { typeof(InsurancePlanSSNPreferred), primaryCoverage, compType });
                types.Add(new object[] { typeof(WorkersCompEmpSupervisorPreferred), primaryCoverage, compType });
                types.Add(new object[] { typeof(WorkersCompPolicyNumberPreferred), primaryCoverage, compType });


                types.Add(new object[] { typeof(BillingCONameRequired), primaryCoverage, compType });
                types.Add(new object[] { typeof(BillingNameRequired), primaryCoverage, compType });
                types.Add(new object[] { typeof(BillingAddressRequired), primaryCoverage.BillingInformation, compType });
                types.Add(new object[] { typeof(BillingPhoneRequired), primaryCoverage, compType });

                types.Add(new object[] { typeof(BillingCONamePreferred), primaryCoverage, compType });
                types.Add(new object[] { typeof(BillingNamePreferred), primaryCoverage, compType });
                types.Add(new object[] { typeof(BillingAddressPreferred), primaryCoverage.BillingInformation, compType });
                types.Add(new object[] { typeof(BillingPhonePreferred), primaryCoverage, compType });

                types.Add(new object[] { typeof(InsurancePlanIPARequired), primaryCoverage, compType });
                types.Add(new object[] { typeof(DOFRInsurancePartIPARequired), primaryCoverage, compType });
                types.Add(new object[] { typeof(DOFRAidCodeRequired), primaryCoverage, compType });
            }

            // Store the rule type (RULETYPE), the context on which it applies (CONTEXT), 
            // the composite rule type (COMPOSITE), and the related "control" (CONTROL).
            // The "control" here is used to pass in the Activity value necessary for rule evaluation

            var nonStaffPhysicianRules = new RuleArrayList();

            // Referring NonStaff Physician
            if (anAccount.ReferringPhysician != null)
            {
                Physician referringPhysician = anAccount.ReferringPhysician;
                Type compType = typeof(OnReferringNonStaffPhysicianForm);

                nonStaffPhysicianRules.Add(new object[]
                                               {
                                                   typeof (NonStaffPhysicianNPIRequired), referringPhysician, compType,
                                                   anAccount.Activity
                                               });
            }

            // Admitting NonStaff Physician
            if (anAccount.AdmittingPhysician != null)
            {
                Physician admittingPhysician = anAccount.AdmittingPhysician;
                Type compType = typeof(OnAdmittingNonStaffPhysicianForm);

                nonStaffPhysicianRules.Add(new object[]
                                               {
                                                   typeof (NonStaffPhysicianNPIRequired), admittingPhysician, compType,
                                                   anAccount.Activity
                                               });
            }

            c_log.Debug("  evaluateAllRules - finished loading types collection");

            // register all of the events we've just stored
            foreach (object[] type in types)
            {
                addContextItem((Type)type[RULETYPE], type[CONTEXT], (Type)type[COMPOSITE]);
                registerEvent((Type)type[RULETYPE], null, (Type)type[COMPOSITE]);
            }

            foreach (object[] type in nonStaffPhysicianRules)
            {
                addContextItem((Type)type[RULETYPE], type[CONTEXT], type[CONTROL], (Type)type[COMPOSITE]);
                registerEvent((Type)type[RULETYPE], null, (Type)type[COMPOSITE]);
            }

            c_log.Debug("  evaluateAllRules - finished registering events");

            // Physician - these are NOT registered here.  We assume that if
            //     the AttendingPhysicianRequired, AdmittingPhysicianRequired, and ReferringPhysicianRequired
            //     rules pass (those are Account-based, non-generic rules), then these will all have passed.

            // 3. run the rules

            blnSingleRunRule = false;

            blnRC = evaluateRules(anAccount);

            c_log.Debug("  evaluateAllRules - finished running the rules");

            // 4. remove context items we added
            foreach (object[] type in types)
            {
                removeContextItem((Type)type[RULETYPE], type[CONTEXT]);
                unregisterEvent((Type)type[RULETYPE], null);
            }

            c_log.Debug("  evaluateAllRules - context items removed (again)");

            // manually remove the extraordinary ones
            removeContextItem(typeof(AdmitDateToPrimaryPlanDates), anAccount);
            removeContextItem(typeof(AdmitDateToSecondaryPlanDates), anAccount);

            blnAllErrors = false;
            blnFireEvents = true;

            return blnRC;
        }

        private bool IsShortPreRegistrationAccount(Account anAccount)
        {
            if ( anAccount.Activity.IsShortPreRegistrationActivity() ||
                ( anAccount.Activity.IsShortMaintenanceActivity() &&
                   anAccount.KindOfVisit.Code == VisitType.PREREG_PATIENT ) )
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// evaluateRule - run a single rule
        /// </summary>
        /// <param name="context"></param>
        /// <param name="aRuleType"></param>
        /// <param name="control"></param>
        /// <returns></returns>
        private bool evaluateRule( object context, Type aRuleType, object control )
        {
            bool rc = true;

            clearActionsForRule( aRuleType );

            blnErrorSeverityRuleFailed = false;

            var rule = ( LeafRule )RulesToRun[aRuleType];
            if ( rule != null )
            {
                rule.FireEvents = blnFireEvents;

                rc = processRuleDetail( rule, null, context, control );
            }

            if ( rc )
            {
                if ( AllRulesPassedEvent != null )
                {
                    AllRulesPassedEvent( this, null );
                }
            }
            else
            {
                if ( NotAllRulesPassedEvent != null )
                {
                    NotAllRulesPassedEvent( this, null );
                }
            }

            return rc;
        }

        /// <summary>
        /// evaluateRules - run a collection of rules
        /// </summary>
        /// <param name="anAccount"></param>
        /// <returns></returns>
        private bool evaluateRules( Account anAccount )
        {
            if ( anAccount == null )
            {
                return true;
            }

            blnErrorSeverityRuleFailed = false;

            account = anAccount;

            if ( RulesToRun == null || RulesToRun.Count <= 0 )
            {
                return false;
            }

            actionList.Clear();

            bool rc = true;

            foreach ( DictionaryEntry o in RulesToRun )
            {
                LeafRule r = null;

                try
                {
                    r = ( LeafRule )o.Value;

                    var aRule = ( LeafRule )RulesToRun[r.GetType()];

                    // process only those rules that are not composited... (they will be processed as
                    // constituents of the composite rule)

                    if ( r != null )
                    {
                        if ( aRule != null )
                        {
                            if ( !processRuleDetail( aRule, anAccount, null, null ) )
                            {
                                rc = false;
                            }
                        }
                        else
                        {
                            c_log.ErrorFormat( "Rule Not found in rulesToRun RuleHashtable: {0}",
                                              r.GetType() );
                        }
                    }
                    else
                    {
                        c_log.ErrorFormat( "Rule in DictionaryObject not present: {0}", o );
                    }
                }
                catch ( Exception ex )
                {
                    if ( r != null )
                    {
                        c_log.ErrorFormat( "Rule failed: {0}: {1} {2}",
                                          r.GetType(),
                                          ex.Message,
                                          ex );
                    }
                    else
                    {
                        c_log.ErrorFormat( "Rule Failed for rule with Key: {0}: {1} {2} ",
                                          o.Key,
                                          ex.Message,
                                          ex
                            );
                    }
                    // System.Diagnostics.Trace.WriteLine( "Rule crashed: " + o.GetType().ToString() );
                    // throw;
                }
            }

            if ( rc )
            {
                if ( AllRulesPassedEvent != null )
                {
                    AllRulesPassedEvent( this, null );
                }
            }
            else
            {
                if ( NotAllRulesPassedEvent != null )
                {
                    NotAllRulesPassedEvent( this, null );
                }
            }

            return rc;
        }

        private ICollection<CompositeAction> getCompositeItemsCollection()
        {
            ICollection<CompositeAction> collection = new Collection<CompositeAction>();

            foreach ( LeafAction la in GetFailedActions() )
            {
                if ( la.IsComposite )
                {
                    collection.Add( ( CompositeAction )la );
                }
            }

            return collection;
        }

        private long getNextGenericOid()
        {
            long prevOid = 9990000;

            foreach ( long l in actionList.Keys )
            {
                if ( l > prevOid )
                {
                    // note, this is a lowercase "L", not a 1 (one)
                    prevOid = l;
                }
            }

            return prevOid + 1;
        }

        /// <summary>
        /// loadActionsForFailedRule - a rule failed; if any actions are associated with it, add them
        /// to the collection of failed actions
        /// </summary>
        /// <param name="lr"></param>
        /// <param name="compRule"></param>
        /// <param name="ruleComp"></param>
        private void loadActionsForFailedRule( LeafRule lr, CompositeRule compRule, RuleComponent ruleComp )
        {
            object aContext = null;
            object aControl = null;
            Type compRuleType = null;

            try
            {
                if ( ruleComp != null )
                {
                    aContext = ruleComp.RuleContext;
                    aControl = ruleComp.RuleControl;
                    compRuleType = ruleComp.CompositeRuleType;
                }

                // this method copied from RulesEngine... only handles 1 action per rule

                CompositeAction cAction = null;

                var nAction = GetMappedAction( lr.Oid ) as LeafAction;

                if ( nAction == null )
                {
                    nAction = new GenericAction { Oid = lr.Oid };
                }

                nAction.Severity = lr.Severity;
                nAction.RuleContextID = lr.RuleContextID;
                nAction.Description = lr.Description;
                nAction.IsComposite = false;

                // use the Context property to hold our composite ID so that we can
                // determine if we already have this failed rule for the appropriate composite

                if ( compRule != null )
                {
                    nAction.Context = compRule.Oid;
                }
                else
                {
                    nAction.Context = lr.CompositeRuleID;
                }


                // if the leaf is not a member of a composite, add it to the list

                long compositeRuleID = 0;

                if ( lr.CompositeRuleID == 0
                    || blnSingleRunRule )
                {
                    if ( compRule != null )
                    {
                        compositeRuleID = compRule.Oid;
                    }
                    else
                    {
                        compositeRuleID = lr.CompositeRuleID;
                    }

                    if ( compositeRuleID != 0 )
                    {
                        var gAction = GetMappedAction( compositeRuleID ) as CompositeAction;

                        var cr = compositeRules[compositeRuleID] as CompositeRule;

                        if ( gAction != null )
                        {
                            cAction = gAction;
                        }
                        else
                        {
                            cAction = new GenericCompositeAction { Oid = cr.Oid };
                        }

                        if ( cr != null )
                        {
                            cAction.Severity = cr.Severity;
                            cAction.RuleContextID = cr.RuleContextID;
                            cAction.Description = cr.Description;
                            cAction.IsComposite = true;
                        }

                        if ( actionList.Contains( cAction.Oid ) )
                        {
                            var aCompAction = actionList[cAction.Oid] as CompositeAction;

                            if ( aCompAction != null )
                            {
                                bool blnExists = false;

                                foreach ( LeafAction la in aCompAction.Constituents )
                                {
                                    if ( aCompAction.Description == cAction.Description
                                        && la.Description == nAction.Description
                                        && la.Severity == nAction.Severity )
                                    {
                                        blnExists = true;
                                        break;
                                    }
                                }
                                if ( !blnExists )
                                {
                                    aCompAction.Add( nAction );
                                }
                            }
                        }
                        else
                        {
                            cAction.Add( nAction );
                            actionList.Add( cAction.Oid, cAction );
                        }
                    }
                    else
                    {
                        if ( !actionList.Contains( nAction.Oid ) )
                        {
                            actionList.Add( nAction.Oid, nAction );
                        }
                    }
                }
                else
                {
                    if ( compRule != null )
                    {
                        compositeRuleID = compRule.Oid;
                    }
                    else
                    {
                        compositeRuleID = lr.CompositeRuleID;
                    }

                    // determine if this rule is for this composite
                    var cr = ( CompositeRule )compositeRules[compositeRuleID];

                    if ( cr != null )
                    {
                        if ( cr.GetType() != compRule.GetType() )
                        {
                            return;
                        }
                    }

                    // this rule has a composite rule id
                    // find the existing or create a new composite action
                    var gAction = GetMappedAction( compositeRuleID ) as CompositeAction;

                    if ( gAction != null )
                    {
                        cAction = gAction;
                    }
                    else
                    {
                        cAction = new GenericCompositeAction { Oid = compRule.Oid };
                    }

                    cAction.Severity = compRule.Severity;
                    cAction.RuleContextID = compRule.RuleContextID;
                    cAction.Description = compRule.Description;
                    cAction.IsComposite = true;

                    if ( aContext.GetType() == typeof( Account )
                        || ( aContext.GetType() != typeof( Account )
                            && compRule.GetType() == compRuleType ) )
                    {
                        CompositeAction aCompAction = null;
                        aCompAction = actionList[cAction.Oid] as CompositeAction;

                        if ( actionList.Contains( cAction.Oid ) )
                        {
                            aCompAction = actionList[cAction.Oid] as CompositeAction;

                            if ( aCompAction == null )
                            {
                                if ( !actionList.Contains( nAction.Oid ) )
                                {
                                    actionList.Add( nAction.Oid, nAction );
                                }
                            }
                            else
                            {
                                bool blnExists = false;

                                foreach ( LeafAction la in aCompAction.Constituents )
                                {
                                    if ( aCompAction.Description == cAction.Description
                                        && la.Description == nAction.Description
                                        && la.Severity == nAction.Severity )
                                    {
                                        blnExists = true;
                                        break;
                                    }
                                }
                                if ( !blnExists )
                                {
                                    aCompAction.Add( nAction );
                                }
                            }
                        }
                        else
                        {
                            cAction.Add( nAction );
                            actionList.Add( cAction.Oid, cAction );
                        }
                    }
                }
            }
            catch
            {
                c_log.ErrorFormat( "Failed to load action for rule: {0}", lr.GetType() );
            }
        }

        /// <summary>
        /// loadActivityRules - load the activityRuleList with activity-based rules
        /// </summary>
        /// <param name="aType"></param>
        private void loadActivityRules( string aType )
        {
            activityRuleList.Clear();
            activityCompositeRules.Clear();

            var ar = new RuleArrayList();

            if ( aType == typeof( EditPreMseActivity ).ToString() )
            {
                aType = typeof( PreMSERegisterActivity ).ToString();
            }

            if ( aType != string.Empty )
            {
                var ruleBrokerProxy = new RuleBrokerProxy();
                ar = ruleBrokerProxy.LoadRules( aType );
            }

            foreach ( LeafRule lr in ar )
            {
                if ( lr.IsComposite )
                {
                    // the rule is a composite rule.

                    var cRule = ( CompositeRule )lr;

                    if ( cRule != null )
                    {
                        // add it to the list of activity rules
                        if ( !activityRuleList.Contains( cRule.GetType() ) )
                        {
                            activityRuleList.Add( cRule.GetType(), cRule );
                        }

                        // and add it to the composite collection
                        if ( !activityCompositeRules.Contains( cRule.Oid ) )
                        {
                            activityCompositeRules.Add( cRule.Oid, cRule );
                        }
                    }
                }
                else
                {
                    // the rule is not a composite...
                    if ( lr.CompositeRuleID > 0 )
                    {
                        // is member of a composite
                        var cr = ( CompositeRule )activityCompositeRules[lr.CompositeRuleID];

                        if ( cr != null )
                        {
                            cr.Add( lr );
                        }
                    }

                    if ( !activityRuleList.Contains( lr.GetType() ) )
                    {
                        activityRuleList.Add( lr.GetType(), lr );
                    }
                }
            }

            RulesToRun = ( RuleHashtable )activityRuleList.Clone();
            compositeRules = ( RuleHashtable )activityCompositeRules.Clone();
        }

        /// <summary>
        /// unregisterEvent - for a rule in the collection of rules for this activity, unregister a client
        /// listener. NO LOCK
        /// </summary>
        private void loadRules( string inContext )
        {
            if ( i_Activity == inContext )
            {
                return;
            }

            i_Activity = inContext;
            loadActivityRules( inContext );
        }

        /// <summary>
        /// LoadRules - load a collection of rules relative to the activity for the current account. 
        /// NO LOCK - private method to manipulate loading of rules
        /// </summary>
        /// <param name="anAccount"></param>
        private void loadRules( Account anAccount )
        {
            if ( anAccount == null
                || anAccount.Activity == null )
            {
                return;
            }

            if ( anAccount.AccountNumber == i_LoadedAccount
                && RulesToRun != null
                && RulesToRun.Count > 0 )
            {
                if ( i_Activity == anAccount.Activity.GetType().ToString() )
                {
                    return;
                }
            }

            if ( activityRuleList != null
                && activityRuleList.Count > 0 )
            {
                if ( anAccount.Activity.GetType() == typeof( MaintenanceActivity ) )
                {
                    // pre reg rules
                    if ( anAccount.KindOfVisit != null
                        && anAccount.KindOfVisit.Code == VisitType.PREREG_PATIENT
                        && !anAccount.IsNewBorn)
                    {
                        if ( i_Activity == anAccount.Activity.GetType() + "PreReg" )
                        {
                            return;
                        }
                    }
                    // post reg rules
                    else if ( anAccount.IsEDorUrgentCarePremseAccount )
                    {
                        if ( i_Activity == anAccount.Activity.GetType() + "PostReg" )
                        {
                            return;
                        }
                    }
                    // reg rules
                    else if ( i_Activity == anAccount.Activity.GetType() + "Reg" )
                    {
                        return;
                    }
                }
            }

            i_LoadedAccount = anAccount.AccountNumber;

            if ( anAccount != null && anAccount.Activity != null )
            {
                i_Activity = anAccount.Activity.GetType().ToString();
                string aType = anAccount.Activity.GetType().ToString();

                if ( anAccount.Activity.GetType() == typeof( MaintenanceActivity ) )
                {
                    if ( anAccount.KindOfVisit != null
                        && anAccount.KindOfVisit.Code == VisitType.PREREG_PATIENT )
                    {
                        if ( anAccount.IsNewBorn )
                        {
                            i_Activity = anAccount.Activity.GetType() + preAdmit;
                            aType = typeof( PreAdmitNewbornActivity).ToString();
                        }
                        else
                        {
                            i_Activity = anAccount.Activity.GetType() + "PreReg";
                            aType = typeof( PreRegistrationActivity ).ToString();
                        }
                    }
                    else if ( anAccount.IsEDorUrgentCarePremseAccount )
                    {
                        i_Activity = anAccount.Activity.GetType() + "PostReg";
                        aType = typeof( PostMSERegistrationActivity ).ToString();
                    }
                    else
                    {
                        i_Activity = anAccount.Activity.GetType() + "Reg";
                        aType = typeof( RegistrationActivity ).ToString();
                    }
                }
                else 
                 
                     if ( anAccount.Activity.GetType() == typeof( ShortMaintenanceActivity ) )
                     {

                         if ( anAccount.KindOfVisit != null
                             && anAccount.KindOfVisit.Code == VisitType.PREREG_PATIENT )
                         {
                             i_Activity = anAccount.Activity.GetType() + "PreReg";
                             aType = typeof( ShortPreRegistrationActivity ).ToString();
                         }
                         else if ( anAccount.IsEDorUrgentCarePremseAccount )
                         {
                             i_Activity = anAccount.Activity.GetType() + "PostReg";
                             aType = typeof( PostMSERegistrationActivity ).ToString();
                         }
                         else
                         {
                             i_Activity = anAccount.Activity.GetType() + "Reg";
                             aType = typeof( ShortRegistrationActivity ).ToString();
                         }
                     }
                     else

                         if ( anAccount.Activity.GetType() == typeof( QuickAccountMaintenanceActivity ) )
                         {

                             if ( anAccount.KindOfVisit != null
                                 && anAccount.KindOfVisit.Code == VisitType.PREREG_PATIENT )
                             {
                                 i_Activity = anAccount.Activity.GetType() + "PreReg";
                                 aType = typeof( QuickAccountCreationActivity ).ToString();
                             }
                             else if ( anAccount.IsEDorUrgentCarePremseAccount )
                             {
                                 i_Activity = anAccount.Activity.GetType() + "PostReg";
                                 aType = typeof( PostMSERegistrationActivity ).ToString();
                             }
                             else
                             {
                                 i_Activity = anAccount.Activity.GetType() + "Reg";
                                 aType = typeof( ShortRegistrationActivity ).ToString();
                             }
                         }

                loadActivityRules( aType );
            }
        }
       
        /// <summary>
        /// loadValidationRules - load the validationRuleArray with benefits validation rules
        /// </summary>
        /// <param name="aType"></param>
        private void loadValidationRules( string aType )
        {
            validationRuleList.Clear();
            validationCompositeRules.Clear();

            var ar = new RuleArrayList();

            if ( aType != string.Empty )
            {
                var ruleBrokerProxy = new RuleBrokerProxy();
                ar = ruleBrokerProxy.LoadRules( aType );
            }

            foreach ( LeafRule lr in ar )
            {
                if ( lr.IsComposite )
                {
                    // the rule is a composite rule.

                    var cRule = ( CompositeRule )lr;

                    if ( cRule != null )
                    {
                        // add it to the list of activity rules
                        if ( !validationRuleList.Contains( cRule.GetType() ) )
                        {
                            validationRuleList.Add( cRule.GetType(), cRule );
                        }

                        // and add it to the composite collection
                        if ( !validationCompositeRules.Contains( cRule.Oid ) )
                        {
                            validationCompositeRules.Add( cRule.Oid, cRule );
                        }
                    }
                }
                else
                {
                    // the rule is not a composite...

                    if ( lr.CompositeRuleID > 0 )
                    {
                        // is member of a composite
                        var cr = ( CompositeRule )validationCompositeRules[lr.CompositeRuleID];

                        if ( cr != null )
                        {
                            cr.Add( lr );
                        }
                    }

                    if ( !validationRuleList.Contains( lr.GetType() ) )
                    {
                        validationRuleList.Add( lr.GetType(), lr );
                    }
                }
            }

            RulesToRun = ( RuleHashtable )validationRuleList.Clone();
        }

        /// <summary>
        /// processCompositeRule - process each constituent in the collection of rules
        /// for a composited rule
        /// </summary>
        /// <param name="compRule"></param>
        /// <param name="ruleComp"></param>
        /// <returns></returns>
        private bool processCompositeRule( CompositeRule compRule, RuleComponent ruleComp )
        {
            bool rc = true;

            // spin the constituents loaded into the compositeRules RuleHashtable
            var cr = ( CompositeRule )compositeRules[compRule.Oid];

            if ( cr != null )
            {
                foreach ( LeafRule lr in cr.Constituents )
                {
                    // pull the rule from the rulesToRun RuleHashtable
                    var aRule = ( LeafRule )RulesToRun[lr.GetType()];

                    if ( aRule != null )
                    {
                        if ( blnSingleRunRule )
                        {
                            if ( !processRule( aRule, ruleComp, compRule, null ) )
                            {
                                rc = false;
                            }
                        }
                        else
                        {
                            if ( aRule.ContextItems == null || aRule.ContextItems.Count <= 0 )
                            {
                                if ( !processRule( aRule, ruleComp, compRule, null ) )
                                {
                                    rc = false;
                                }
                            }
                            else
                            {
                                foreach ( RuleComponent rComp in aRule.ContextItems )
                                {
                                    if ( !processRule( aRule, rComp, compRule, null ) )
                                    {
                                        rc = false;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return rc;
        }

        /// <summary>
        /// processRule - process a single rule from the collection
        /// </summary>
        /// <param name="r"></param>
        /// <param name="ruleComp"></param>
        /// <param name="compRule"></param>
        /// <param name="control"></param>
        /// <returns></returns>
        private bool processRule( LeafRule r, RuleComponent ruleComp, CompositeRule compRule, object control )
        {
            bool rc = true;
            bool stopProcessing = false;

            object aContext = null;
            object aControl = null;
            Type compRuleType = null;

            if ( ruleComp != null )
            {
                aContext = ruleComp.RuleContext;
                aControl = ruleComp.RuleControl;
                compRuleType = ruleComp.CompositeRuleType;
            }

            // we only process severity level Error rules until one fails.  If one fails, all other
            // severity level Error rules are bypassed
            try
            {
                if ( r != null )
                {
                    if ( ( r.Severity == SEVERITY_ERROR
                         || r.Severity == SEVERITY_DEPRICATED_CHANGE )
                        && blnErrorSeverityRuleFailed
                        && !blnAllErrors )
                    {
                        return false;
                    }

                    if ( aControl != null )
                    {
                        r.AssociatedControl = aControl;
                    }
                    else if ( control != null )
                    {
                        r.AssociatedControl = control;
                    }

                    if ( r.IsComposite )
                    {
                        var cr = ( CompositeRule )RulesToRun[r.GetType()];

                        if ( r.AssociatedControl != null )
                        {
                            cr.AssociatedControl = r.AssociatedControl;
                        }
                        else if ( control != null )
                        {
                            cr.AssociatedControl = control;
                        }

                        cr.FireEvents = blnFireEvents;
                        bool ruleRC = cr.CanBeAppliedTo( aContext );

                        if ( !ruleRC )
                        {
                            if ( cr.Severity == SEVERITY_ERROR
                                || cr.Severity == SEVERITY_DEPRICATED_CHANGE )
                            {
                                blnErrorSeverityRuleFailed = true;
                            }

                            loadActionsForFailedRule( cr, compRule, ruleComp );
                            stopProcessing = cr.ShouldStopProcessing();
                            rc = false;
                            return rc;
                        }

                        var comp = ( CompositeRule )compositeRules[cr.Oid];

                        if ( rc
                            && comp != null
                            && comp.Constituents.Count > 0 )
                        {
                            rc = processCompositeRule( comp, ruleComp );
                        }
                    }
                    else // not a composite rule
                    {
                        LeafRule aRule = r;

                        if ( r.AssociatedControl != null )
                        {
                            aRule.AssociatedControl = r.AssociatedControl;
                        }
                        else if ( control != null )
                        {
                            aRule.AssociatedControl = control;
                        }

                        aRule.FireEvents = blnFireEvents;
                        bool ruleRC = aRule.CanBeAppliedTo( aContext );

                        if ( !ruleRC )
                        {
                            if ( r.Severity == SEVERITY_ERROR
                                || r.Severity == SEVERITY_DEPRICATED_CHANGE )
                            {
                                blnErrorSeverityRuleFailed = true;
                            }

                            loadActionsForFailedRule( aRule, compRule, ruleComp );
                            stopProcessing = aRule.ShouldStopProcessing();
                            rc = false;
                        }
                    }
                }
            }
            catch ( Exception ex )
            {
                string msg = "Rule failed: " + r.GetType() +
                             " " + ex.Message +
                             " " + ex;
                c_log.Error( msg, ex );
            }

            return rc;
        }

        private bool processRuleDetail( LeafRule aRule, Account anAccount, object context, object control )
        {
            bool rc = true;

            if ( context == null )
            {
                if ( aRule.CompositeRuleID <= 0 )
                {
                    //not a composited rule
                    // if no context items were added, run the rule for the account
                    if ( aRule.ContextItems == null || aRule.ContextItems.Count <= 0 )
                    {
                        var rComp = new RuleComponent();
                        rComp.RuleContext = anAccount;

                        if ( !processRule( aRule, rComp, null, null ) )
                        {
                            rc = false;
                        }
                    }
                    else
                    {
                        // run the rule for each item in the context collection
                        foreach ( RuleComponent ruleComp in aRule.ContextItems )
                        {
                            if ( !processRule( aRule, ruleComp, null, null ) )
                            {
                                rc = false;
                            }
                        }
                    }
                }
            }
            else
            {
                // context was passed from evaluateRule... use this if no context was registered

                if ( aRule.CompositeRuleID <= 0
                    || blnSingleRunRule )
                {
                    if ( aRule.ContextItems == null
                        || aRule.ContextItems.Count <= 0
                        || blnSingleRunRule )
                    {
                        var rComp = new RuleComponent();
                        rComp.RuleContext = context;

                        if ( !processRule( aRule, rComp, null, control ) )
                        {
                            rc = false;
                        }
                    }
                    else
                    {
                        foreach ( RuleComponent ruleComp in aRule.ContextItems )
                        {
                            if ( !processRule( aRule, ruleComp, null, null ) )
                            {
                                rc = false;
                            }
                        }
                    }
                }
            }

            return rc;
        }

        private void registerBenefitsValidation( Account anAccount, Coverage aCoverage )
        {
            // register context items

            // Store the rule type (RULETYPE), the context on which it applies (CONTEXT), 
            // the composite rule type (COMPOSITE), and the related "control" (CONTROL)

            InsurancePlanCategory control = aCoverage.InsurancePlan.PlanCategory;
            Type compType = null;
            BenefitsRegisterAdmitDate(anAccount, compType, control);
            
           
            if ( aCoverage.CoverageOrder.Oid == CoverageOrder.PRIMARY_OID )
            {
                compType = typeof( BenVerOnPayorDetailsFormForPrimaryInsurance );
            }
            else
            {
                compType = typeof( BenVerOnPayorDetailsFormForSecondaryInsurance );
            }

            benefitsRegisteredContexts.Add( new object[] { typeof( BenVerCertSSN ), aCoverage, compType, control } );
            benefitsRegisteredContexts.Add( new object[] { typeof( BenVerPolicyCIN ), aCoverage, compType, control } );
            benefitsRegisteredContexts.Add( new object[] { typeof( BenVerMBI ), aCoverage, compType, control } );
            benefitsRegisteredContexts.Add( new object[] { typeof( BenVerPolicyNumber ), aCoverage, compType, control } );

            if ( anAccount != null && anAccount.Facility != null )
            {
                benefitsRegisteredContexts.Add( new object[]
                                                   {
                                                       typeof (BenVerMedicaidIssueDate), aCoverage, compType,
                                                       anAccount.Facility
                                                   } );
            }

            if ( aCoverage.CoverageOrder.Oid == CoverageOrder.PRIMARY_OID )
            {
                compType = typeof( BenVerOnInsuredFormForPrimaryInsurance );
            }
            else
            {
                compType = typeof( BenVerOnInsuredFormForSecondaryInsurance );
            }

            benefitsRegisteredContexts.Add( new object[]
                                               {
                                                   typeof (BenVerInsuredRelationship), aCoverage.Insured, compType, control
                                               } );
            benefitsRegisteredContexts.Add( new object[] { typeof( BenVerInsuredLastName ), aCoverage.Insured, compType, control } );
            benefitsRegisteredContexts.Add( new object[] { typeof( BenVerInsuredFirstName ), aCoverage.Insured, compType, control } );
            benefitsRegisteredContexts.Add( new object[] { typeof( BenVerInsuredSSN ), aCoverage.Insured, compType, control } );

            foreach ( object[] type in benefitsRegisteredContexts )
            {
                addContextItem( ( Type )type[RULETYPE], type[CONTEXT], type[CONTROL], ( Type )type[COMPOSITE] );
                registerEvent( ( Type )type[RULETYPE], null, ( Type )type[COMPOSITE] );
            }
        }
        private void BenefitsRegisterAdmitDate(Account anAccount, Type compType, InsurancePlanCategory control)
        {
            if (anAccount == null || anAccount.Activity == null )
            {
                registerBenefitVerificationRules(anAccount, compType, control);
                return;
            }
            var activity = anAccount.Activity;
            if (activity is QuickAccountMaintenanceActivity ||
                 activity is QuickAccountCreationActivity)
            {
               registerBenefitVerificationRulesForQuickAccount( anAccount,   compType,   control );
            }
            else
            {
                registerBenefitVerificationRules( anAccount, compType, control );

            }
        }
        private void registerBenefitVerificationRules(Account anAccount, Type compType, InsurancePlanCategory control)
        {
            benefitsRegisteredContexts.Add(new object[] {typeof (BenVerAdmitDate), anAccount, compType, control});
            benefitsRegisteredContexts.Add(new object[] {typeof (BenVerLastName), anAccount, compType, control});
            benefitsRegisteredContexts.Add(new object[] {typeof (BenVerFirstName), anAccount, compType, control});
            benefitsRegisteredContexts.Add(new object[] {typeof (BenVerGender), anAccount, compType, control});
            benefitsRegisteredContexts.Add(new object[] {typeof (BenVerDateOfBirth), anAccount, compType, control});
            benefitsRegisteredContexts.Add(new object[] {typeof (BenVerPatientSSN), anAccount, compType, control});
        }

        private void registerBenefitVerificationRulesForQuickAccount(Account anAccount, Type compType, InsurancePlanCategory control)
        {
            benefitsRegisteredContexts.Add(new object[]
                                               {
                                                   typeof (BenVerAdmitDateOnQuickAccountForm), anAccount, compType, control
                                               });
            benefitsRegisteredContexts.Add(new object[]
                                               {typeof (BenVerLastNameOnQuickAccountForm), anAccount, compType, control});
            benefitsRegisteredContexts.Add(new object[]
                                               {
                                                   typeof (BenVerFirstNameOnQuickAccountForm), anAccount, compType, control
                                               });
            benefitsRegisteredContexts.Add(new object[]
                                               {typeof (BenVerGenderOnQuickAccountForm), anAccount, compType, control});
            benefitsRegisteredContexts.Add(new object[]
                                               {
                                                   typeof (BenVerDateOfBirthOnQuickAccountForm), anAccount, compType,
                                                   control
                                               });
            benefitsRegisteredContexts.Add(new object[]
                                               {
                                                   typeof (BenVerPatientSSNOnQuickAccountForm), anAccount, compType,
                                                   control
                                               });
        }

        /// <summary>
        /// registerEvent - for a rule in the collection of rules for this activity, register a client
        /// listener. NO LOCK since this is a private method
        /// </summary>
        /// <param name="ruleType"></param>
        /// <param name="eventHandler"></param>
        /// <param name="compRuleType"></param>
        private void registerEvent( Type ruleType, EventHandler eventHandler, Type compRuleType )
        {
            if ( RulesToRun == null )
            {
                return;
            }

            object o = RulesToRun[ruleType];

            LeafRule lr = null;
            if ( o != null )
            {
                lr = ( LeafRule )o;

                if ( lr != null && eventHandler != null )
                {
                    lr.RegisterHandler( eventHandler );
                }
            }
        }

        private void removeAllContextItems()
        {
            if ( RulesToRun == null )
            {
                return;
            }

            foreach ( DictionaryEntry de in RulesToRun )
            {
                removeAllContextItems( de.Value.GetType() );
            }
        }

        private void removeAllContextItems( Type ruleType )
        {
            if ( ruleType == null
                || RulesToRun == null )
            {
                return;
            }

            object o = RulesToRun[ruleType];
            LeafRule rule = null;
            if ( o != null )
            {
                rule = ( LeafRule )o;
            }

            if ( rule != null )
            {
                rule.ContextItems.Clear();
                rule.AssociatedControl = null;

                if ( rule.IsComposite )
                {
                    var cr = ( CompositeRule )rule;

                    if ( cr != null
                        && cr.Constituents != null
                        && cr.Constituents.Count > 0 )
                    {
                        foreach ( LeafRule lr in cr.Constituents )
                        {
                            if ( lr != null )
                            {
                                lr.ContextItems.Clear();
                                lr.AssociatedControl = null;
                            }
                        }
                    }
                }
            }
        }

        private void removeContextItem( Type ruleType, object context )
        {
            if ( ruleType == null )
            {
                return;
            }

            object o = activityRuleList[ruleType];
            LeafRule rule = null;
            if ( o != null )
            {
                rule = ( LeafRule )o;
            }

            if ( rule != null )
            {
                for ( int i = 0; i < rule.ContextItems.Count; i++ )
                {
                    if ( ( ( RuleComponent )rule.ContextItems[i] ).RuleContext.Equals( context ) )
                    {
                        rule.ContextItems.Remove( rule.ContextItems[i] );
                        //break;
                    }
                }

                if ( rule.IsComposite )
                {
                    var cr = ( CompositeRule )rule;

                    foreach ( LeafRule lr in cr.Constituents )
                    {
                        var aRule = ( LeafRule )activityRuleList[lr.GetType()];

                        for ( int i = 0; i < aRule.ContextItems.Count; i++ )
                        {
                            if ( ( ( RuleComponent )aRule.ContextItems[i] ).RuleContext.Equals( context ) )
                            {
                                aRule.ContextItems.Remove( aRule.ContextItems[i] );
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void unregisterBenefitsValidation()
        {
            foreach ( object[] type in benefitsRegisteredContexts )
            {
                removeContextItem( ( Type )type[RULETYPE], type[CONTEXT] );
                unregisterEvent( ( Type )type[RULETYPE], null );
            }

            benefitsRegisteredContexts.Clear();
        }

        private void unregisterEvent( Type ruleType, EventHandler eventHandler )
        {
            if ( RulesToRun == null )
            {
                return;
            }

            object o = RulesToRun[ruleType];
            LeafRule lr = null;
            if ( o != null )
            {
                lr = ( LeafRule )o;
            }

            if ( lr != null )
            {
                lr.UnregisterHandler( eventHandler );
            }
        }

        #endregion Methods
    }
}