using System;
using System.Collections;
using System.Data;
using System.Text;
using Extensions.DB2Persistence;
using Extensions.UI.Builder;
using IBM.Data.DB2.iSeries;
using PatientAccess.Actions;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Rules;
using log4net;

namespace PatientAccess.Persistence
{
    /// <summary>
    /// Summary description for WorklistPBARBroker.
    /// </summary>
    //TODO: Create XML summary comment for WorklistPBARBroker
    [Serializable]
    public class WorklistPBARBroker : AbstractPBARBroker, IWorklistBroker
    {
        #region Event Handlers
        #endregion

        #region Methods

        /// <summary>
        /// Get a list of remaining actions for an account
        /// </summary>
        /// <param name="accountNumber">Account Number</param>
        /// <param name="medicalRecordNumber">Medical Record Number</param>
        /// <param name="facilityOid">Oid of facility</param>
        /// <returns>A Hashtable containing a group of Actionslists</returns>
        /// <exception cref="Exception"><c>Exception</c>.</exception>
        public Hashtable RemainingActionsFor( long accountNumber, long medicalRecordNumber, long facilityOid )
        {
            c_log.Debug( "Entry point : WorklistPBARBroker.remainingActionsFor " + facilityOid +
                " " + medicalRecordNumber + " " + accountNumber );

            Hashtable worklists = new Hashtable();
            IRuleBroker ruleBroker = BrokerFactory.BrokerOfType<IRuleBroker>();
            RuleEngine re = RuleEngine.NewInstance();
            Hashtable rulesByID = ruleBroker.AllRulesById();
            Hashtable allActions = ruleBroker.AllActions();


            SafeReader reader = null;
            iDB2Command cmd = null;
            try
            {
                IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
                Facility facility = facilityBroker.FacilityWith( facilityOid );
                cmd = this.CommandFor( "CALL " + SP_REMAININGCOMMANDSFOR +
                    "(" + PARAM_FACILITYID +
                    "," + PARAM_ACCOUNTNUMBER + ")",
                    CommandType.Text,
                    facility );

                cmd.Parameters[PARAM_FACILITYID].Value = facility.Oid;
                cmd.Parameters[PARAM_ACCOUNTNUMBER].Value = accountNumber;

                reader = this.ExecuteReader( cmd );

                this.actionList.Clear();

                while (reader.Read())
                {
                    // Populate the RuleContextID with the WorklistID so that we can 

                    CompositeAction cAction = null;
                    LeafAction action = null;
                    long worklistID = reader.GetInt64( COL_WORKLISTID );
                    long compositeRuleID = reader.GetInt64( COL_COMPOSITERULEID );
                    long actionID = reader.GetInt64( COL_RULEID );

                    if (compositeRuleID == 0)
                    {
                        action = allActions[actionID] as LeafAction;
                    }

                    CompositeRule cr = null;

                    if (compositeRuleID > 0)
                    {
                        cr = rulesByID[compositeRuleID] as CompositeRule;
                    }

                    if (cr != null && cr.Oid != 0)
                    {
                        cAction = re.GetMappedAction( compositeRuleID ) as CompositeAction;

                        if (cAction == null)
                        {
                            GenericCompositeAction gcAction = new GenericCompositeAction();
                            cAction = gcAction;
                            cAction.Oid = cr.Oid;
                        }


                        cAction.Severity = cr.Severity;
                        cAction.RuleContextID = Convert.ToInt32( worklistID );

                        // TODO [SR41185] Replace the next line with these commented lines

                        cAction.Description = cr.Description;

                        cAction.IsComposite = true;

                    }

                    if (action == null)
                    {
                        GenericAction lgAction = new GenericAction();
                        action = lgAction;
                        action.Oid = actionID;
                    }
                    if (action != null)
                    {
                        action.RuleContextID = Convert.ToInt32( worklistID );
                        action.IsComposite = false;
                        if (cAction != null)
                        {
                            action.CompositeActionID = cAction.Oid;
                        }
                    }

                    ActionsList remainingActionsList = (ActionsList)worklists[worklistID];
                    if (remainingActionsList != null)
                    {
                        if (action.IsComposite)
                        {
                            CompositeAction compAction = remainingActionsList.ActionWith( action.Oid ) as CompositeAction;
                            if (compAction == null)
                            {
                                remainingActionsList.Add( compAction );
                                worklists[worklistID] = remainingActionsList;
                            }
                        }

                        else
                        {
                            if (cAction != null
                                && cAction.Oid != 0)
                            {
                                CompositeAction compAction = remainingActionsList.ActionWith( cAction.Oid ) as CompositeAction;
                                if (compAction != null)
                                {
                                    compAction.Add( action );
                                }
                                else
                                {
                                    cAction.Add( action );
                                    remainingActionsList.Add( cAction );
                                }
                            }
                            else
                            {
                                LeafAction lAction = remainingActionsList.ActionWith( action.Oid ) as LeafAction;
                                if (lAction == null)
                                {
                                    remainingActionsList.Add( action );
                                }
                            }
                        }
                    }
                    else
                    {
                        remainingActionsList = new ActionsList();

                        if (cAction == null)
                        {
                            remainingActionsList.Add( action );
                            worklists.Add( worklistID, remainingActionsList );
                        }
                        else
                        {
                            if (action.CompositeActionID == 0)
                            {
                                remainingActionsList.Add( action );
                                worklists.Add( worklistID, remainingActionsList );
                            }
                            else
                            {
                                cAction.Add( action );
                                remainingActionsList.Add( cAction );
                                worklists.Add( worklistID, remainingActionsList );
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = "RemainingActionsFor method failed for Facility:" + facilityOid +
                    " MRN: " + medicalRecordNumber + " AccountNum: " + accountNumber;
                throw BrokerExceptionFactory.BrokerExceptionFrom( msg, ex, c_log );
            }
            finally
            {
                Close( reader );
                Close( cmd );
            }
            return worklists;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="anAccount"></param>
        public void SaveRemainingActions( Account anAccount )
        {
            c_log.Debug( "Entry point : WorklistPBARBroker.SaveRemainingActionsFor " + anAccount.Facility.Oid +
                " " + anAccount.Patient.MedicalRecordNumber + " " +
                anAccount.AccountNumber );
            DeleteAccountWorklistItemsWith( anAccount.Facility.Oid,
                anAccount.AccountNumber );
            if (anAccount.Activity != null &&
                anAccount.Activity.IsQuickAccountMaintenanceActivity())
            {
                anAccount.RemoveDefaultCoverage();
            }
            SaveWorklistActionItems( anAccount );
        }

        private void SaveWorklistActionItems( Account anAccount )
        {
            c_log.Debug( "Entry point : WorklistPBARBroker.SaveWorklistActionItems " + anAccount.Facility.Oid +
                " " + anAccount.Patient.MedicalRecordNumber + " " +
                anAccount.AccountNumber );
            long worklistID;
            LeafAction action;
            ArrayList actArray = new ArrayList();
            RuleEngine re = RuleEngine.NewInstance();
            IRuleBroker ruleBroker = BrokerFactory.BrokerOfType<IRuleBroker>();
            Hashtable allActions = ruleBroker.AllActions();
            if (( anAccount.KindOfVisit.Code == VisitType.EMERGENCY_PATIENT ) &&
                ( anAccount.FinancialClass != null ) &&
                ( anAccount.FinancialClass.Code == FinancialClass.MED_SCREEN_EXM_CODE )
                )
            {
                worklistID = Worklist.EMERGENCYDEPARMENTWORKLISTID;
                InsertWorklistItems( anAccount, worklistID, 0, COMPLETE_POST_MSE_REGISTRATION );
            }
            else if (anAccount.IsUrgentCarePreMse)
            {
                worklistID = Worklist.EMERGENCYDEPARMENTWORKLISTID;
                InsertWorklistItems(anAccount, worklistID, 0, COMPLETE_POST_MSE_REGISTRATION);
            }
            else if (anAccount.IsPAIWalkinRegistered &&
                     anAccount.Activity.IsPAIWalkinOutpatientCreationActivity())
            {
                InsertWorklistItems(anAccount, Worklist.PREREGWORKLISTID, 0, CANCEL_ACTIVATE_WALKIN_ACCOUNT);
            }
            else
            {
                actArray = re.GetWorklistActionItems(anAccount);

                string kindOfVisitCode = string.Empty;
                string financialClassCode = string.Empty;

                if (anAccount.KindOfVisit != null)
                {
                    kindOfVisitCode = anAccount.KindOfVisit.Code;
                }
                if (anAccount.FinancialClass != null)
                {
                    financialClassCode = anAccount.FinancialClass.Code;
                }

                Hashtable actionWorklistMapping = ruleBroker.ActionWorklistMapping(kindOfVisitCode,
                    financialClassCode);

                foreach (LeafAction la in actArray)
                {

                    worklistID = 0;
                    ArrayList al = actionWorklistMapping[la.Oid] as ArrayList;
                    if (al != null)
                    {
                        worklistID = (long) al[0];
                    }

                    if (worklistID == 0)
                    {
                        if (anAccount.KindOfVisit.Code == VisitType.PREREG_PATIENT)
                        {
                            worklistID = Worklist.PREREGWORKLISTID;
                        }

                        else
                        {
                            worklistID = Worklist.POSTREGWORKLISTID;
                        }
                    }

                    CompositeAction ca = null;

                    if (la.IsComposite)
                    {
                        ca = la as CompositeAction;
                    }
                    if (ca != null)
                    {
                        foreach (LeafAction leafAction in ca.Constituents)
                        {
                            long leafRuleID = leafAction.Oid;
                            long compRuleid = (long) leafAction.Context;
                            InsertWorklistItems(anAccount, worklistID, compRuleid, leafRuleID);
                        }
                    }
                    else
                    {
                        action = allActions[la.Oid] as LeafAction;
                        if (action != null)
                        {
                            InsertWorklistItems(anAccount, worklistID, 0, la.Oid);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Delete worklist items for the given facility and accountNumber
        /// </summary>
        /// <exception cref="Exception"><c>Exception</c>.</exception>
        public void DeleteAccountWorklistItemsWith( long facilityId,
            long accountNumber )
        {
            c_log.Debug( "Entry point : WorklistPBARBroker.DeleteAccountWorklistItemsWith " + facilityId +
                " " + accountNumber );

            iDB2Command cmd = null;
            try
            {
                IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
                Facility facility = facilityBroker.FacilityWith( facilityId );
                cmd = this.CommandFor( "CALL " + SP_DELETEWORKLISTSITEMS +
                    "(" + PARAM_FACILITYID +
                    "," + PARAM_ACCOUNTNUMBER + ")",
                    CommandType.Text,
                    facility );

                cmd.Parameters[PARAM_FACILITYID].Value = facilityId;
                cmd.Parameters[PARAM_ACCOUNTNUMBER].Value = accountNumber;

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                string msg = "DeleteAccountWorklistItemsWith method failed for Facility:" + facilityId +
                    " AccountNum:" + accountNumber;
                throw BrokerExceptionFactory.BrokerExceptionFrom( msg, ex, c_log );
            }
            finally
            {
                Close( cmd );
            }
        }

        /// <exception cref="Exception"><c>Exception</c>.</exception>
        private void InsertWorklistItems( Account anAccount, long worklistID, long caid, long leafRuleID )
        {
            if (anAccount != null
                && anAccount.HospitalService != null &&
                anAccount.HospitalService.Code == HospitalService.COVID_VACCINE_CLINIC && worklistID == 4)
            {
                return;
            }

            c_log.Debug( "Entry point : WorklistPBARBroker.InsertWorklistItems " + anAccount.Facility.Oid +
                         " " + anAccount.Patient.MedicalRecordNumber + " " +
                         anAccount.AccountNumber + " " + worklistID + " " + caid + " " + leafRuleID );
            SafeReader reader = null;
            iDB2Command cmd = null;
            try
            {
                cmd = this.CommandFor( "CALL " + SP_INSERTWORKLISTSITEMS +
                    "(" + PARAM_FACILITYID +
                    "," + PARAM_ACCOUNTNUMBER +
                    "," + PARAM_WORKLISTID +
                    "," + PARAM_COMPOSITEID +
                    "," + PARAM_RULEID + ")",
                    CommandType.Text,
                    anAccount.Facility );

                cmd.Parameters[PARAM_FACILITYID].Value = anAccount.Facility.Oid;

                cmd.Parameters[PARAM_ACCOUNTNUMBER].Value = anAccount.AccountNumber;
                cmd.Parameters[PARAM_WORKLISTID].Value = worklistID;
                cmd.Parameters[PARAM_COMPOSITEID].Value = caid;
                cmd.Parameters[PARAM_RULEID].Value = leafRuleID;

                reader = this.ExecuteReader( cmd );

                while (reader.Read())
                {
                }

            }
            catch (Exception ex)
            {
                string msg = "InsertWorklistItems method failed for Facility:" + anAccount.Facility.Oid +
                    " MRN:" + anAccount.Patient.MedicalRecordNumber + " AccountNum: " +
                    anAccount.AccountNumber + " ID:" + worklistID;
                throw BrokerExceptionFactory.BrokerExceptionFrom( msg, ex, c_log );
            }
            finally
            {
                Close( reader );
                Close( cmd );
            }
        }

        public void ProcessAllWorklistsFor( Facility facility )
        {
            WorklistActivity worklistActivity;

            while (( worklistActivity = GetNextAccountFor( facility ) ) != null)
            {
                try
                {
                    ProcessWorklistFor( worklistActivity );
                }
                catch
                {
                    ResetWorklistProcessingRecord( worklistActivity, WORKLISTPROCESSSTATUS_ERROR );
                    // do not throw the error from here. Keep processing the rest of the accounts
                    // the exception has been logged in the ProcessWorklistFor method.
                    //throw BrokerExceptionFactory.BrokerExceptionFrom("Failure in worklist Processing", ex, c_log);
                }
            }
            //each time we run we will delete any entries for hospitals that PA does not support
            CleanupPBARWorklistTable( facility );
        }


        #endregion

        #region Private Properties

        /// <exception cref="Exception">Failure cleaning up worklist processing table</exception>
        private void CleanupPBARWorklistTable( Facility facility )
        {
            iDB2Command cmd = null;

            try
            {
                string cmdText = CreateCleanupPBARWorklistTableSQL();
                cmd = CommandFor( cmdText, CommandType.Text, facility );

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( "Failure cleaning up worklist processing table",
                                                                 ex, c_log );
            }
            finally
            {
                Close( cmd );
            }
        }

        private string CreateCleanupPBARWorklistTableSQL()
        {
            IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            ICollection facilities = facilityBroker.AllFacilities();
            StringBuilder sb = new StringBuilder();

            bool firstTime = true;
            sb.Append( "DELETE FROM NHUPDPASP WHERE PASHSP# NOT IN (" );

            foreach (Facility facility in facilities)
            {
                if (firstTime)
                {
                    firstTime = false;
                }
                else
                {
                    sb.Append( "," );
                }
                sb.Append( facility.Oid );
            }
            sb.Append( ")" );

            return sb.ToString();

        }

        /// <exception cref="Exception"><c>Exception</c>.</exception>
        private void ProcessWorklistFor( WorklistActivity worklistActivity )
        {
            try
            {
                // if there is no med rec # it means that the account has already
                // been deleted. This happens in testing and if the account is purged
                // when this happens just delete the processing record
                if (worklistActivity.MedicalRecordNumber == 0)
                {
                    RemoveWorklistProcessingRecordFor( worklistActivity );
                }
                else
                {
                    switch (worklistActivity.Activity)
                    {
                        case WorklistActivity.UPDATED:
                        case WorklistActivity.INSERTED:
                            c_log.InfoFormat( "Begin processing worklist for hsp: {0} MRN:{1} ACCT:{2}",
                                             worklistActivity.Facility.Oid,
                                             worklistActivity.MedicalRecordNumber,
                                             worklistActivity.AccountNumber );

                            // load the account, start by loading the patient
                            IAccountBroker accountBroker = BrokerFactory.BrokerOfType<IAccountBroker>();
                            IPatientBroker patientBroker = BrokerFactory.BrokerOfType<IPatientBroker>();
                            Patient aPatient = patientBroker.SparsePatientWith( worklistActivity.MedicalRecordNumber,
                                                                               worklistActivity.Facility.Code );

                            if (aPatient == null)
                            {
                                throw new Exception( string.Format( "Patient NOT Found. HSP:{0} MRN:{1} ACCT:{2}",
                                                                  worklistActivity.Facility.Oid,
                                                                  worklistActivity.MedicalRecordNumber,
                                                                  worklistActivity.AccountNumber ) );
                            }
                            else
                            {
                                // now load the actual account
                                Account anAccount = accountBroker.AccountFor( aPatient, worklistActivity.AccountNumber );

                                Activity activity = new MaintenanceActivity();
                                activity.AppUser = User.GetCurrent();

                                if (anAccount != null)
                                {
                                    anAccount.Activity = activity;
                                    // do the actual parsing of worklist items
                                    RunWorklistsFor( worklistActivity, anAccount );
                                }
                                c_log.InfoFormat( "Completed processing worklist for hsp: {0} MRN:{1} ACCT:{2}",
                                                 worklistActivity.Facility.Oid,
                                                 worklistActivity.MedicalRecordNumber,
                                                 worklistActivity.AccountNumber );
                            }
                            break;
                        case WorklistActivity.DELETED:
                            // no need to load the account if we are just deleting the worklist items
                            c_log.InfoFormat( "Begin processing worklist(Delete) for hsp: {0} MRN:{1} ACCT:{2}",
                                             worklistActivity.Facility.Oid,
                                             worklistActivity.MedicalRecordNumber,
                                             worklistActivity.AccountNumber );

                            DeleteWorklistFor( worklistActivity );

                            c_log.InfoFormat( "Completed processing worklist(Delete) for hsp: {0} MRN:{1} ACCT:{2}",
                                             worklistActivity.Facility.Oid,
                                             worklistActivity.MedicalRecordNumber,
                                             worklistActivity.AccountNumber );
                            break;
                        default:
                            c_log.Error( "Invalid WorklistStatus found: " + worklistActivity.Activity );
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom(
                    string.Format( "Failure processing Worklist for HSP:{0} MRN:{1} ACCT{2}",
                                  worklistActivity.Facility.Code,
                                  worklistActivity.MedicalRecordNumber,
                                  worklistActivity.AccountNumber ),
                    ex, c_log );
            }
        }

        private WorklistActivity GetNextAccountFor( Facility facility )
        {
            WorklistActivity worklistActivity = null;
            iDB2Command cmd = null;

            try
            {
                cmd = this.CommandFor(
                    String.Format( "CALL {0}( {1},{2},{3},{4})",
                                  SP_GETNEXTACCOUNTFOR,
                                  PARAM_HSPNUMBER,
                                  OUT_PARAM_ACCOUNTNUMBER,
                                  OUT_PARAM_MEDICALRECORDNUMBER,
                                  OUT_PARAM_ACTIVITY ),
                                  CommandType.Text,
                                  facility );

                cmd.Parameters[PARAM_HSPNUMBER].Value = facility.Oid;

                cmd.Parameters[OUT_PARAM_ACCOUNTNUMBER].Direction = ParameterDirection.Output;
                cmd.Parameters[OUT_PARAM_MEDICALRECORDNUMBER].Direction = ParameterDirection.Output;
                cmd.Parameters[OUT_PARAM_ACTIVITY].Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                if (cmd.Parameters[OUT_PARAM_ACCOUNTNUMBER].Value != DBNull.Value)
                {
                    long accountNumber = Convert.ToInt32( cmd.Parameters[OUT_PARAM_ACCOUNTNUMBER].Value );
                    long medicalRecordNumber = 0;
                    if (cmd.Parameters[OUT_PARAM_MEDICALRECORDNUMBER].Value != DBNull.Value)
                    {
                        medicalRecordNumber = Convert.ToInt32( cmd.Parameters[OUT_PARAM_MEDICALRECORDNUMBER].Value );
                    }
                    string activity = cmd.Parameters[OUT_PARAM_ACTIVITY].Value.ToString();

                    if (accountNumber != 0)
                    {
                        worklistActivity = new WorklistActivity();
                        worklistActivity.AccountNumber = accountNumber;
                        worklistActivity.MedicalRecordNumber = medicalRecordNumber;
                        worklistActivity.Activity = activity;
                        worklistActivity.Facility = facility;
                    }
                }
            }
            catch (Exception ex)
            {
                BrokerExceptionFactory.BrokerExceptionFrom( "Unhandled Exception.", ex, c_log );
            }
            finally
            {
                Close( cmd );
            }
            return worklistActivity;
        }

        private void RunWorklistsFor( WorklistActivity worklistActivity, Account anAccount )
        {
            IWorklistBroker worklistBroker = BrokerFactory.BrokerOfType<IWorklistBroker>();
            worklistBroker.SaveRemainingActions( anAccount );

            // delete the processing record
            RemoveWorklistProcessingRecordFor( worklistActivity );
        }
        private void DeleteWorklistFor( WorklistActivity worklistActivity )
        {
            IWorklistBroker worklistBroker = BrokerFactory.BrokerOfType<IWorklistBroker>();
            worklistBroker.DeleteAccountWorklistItemsWith(
                worklistActivity.Facility.Oid, worklistActivity.AccountNumber );

            // delete the processing record
            RemoveWorklistProcessingRecordFor( worklistActivity );
        }

        /// <exception cref="Exception"><c>Exception</c>.</exception>
        private void ResetWorklistProcessingRecord( WorklistActivity worklistActivity, string status )
        {
            iDB2Command cmd = null;

            try
            {
                cmd = CommandFor(
                    string.Format( "CALL {0} ({1},{2},{3})",
                                  SP_RESETWRKLSTPROCESSINGRECFOR,
                                  PARAM_HSPNUMBER,
                                  PARAM_ACCOUNTNUMBER,
                                  PARAM_STATUS ),
                    CommandType.Text,
                    worklistActivity.Facility );

                cmd.Parameters[PARAM_HSPNUMBER].Value = worklistActivity.Facility.Oid;
                cmd.Parameters[PARAM_ACCOUNTNUMBER].Value = worklistActivity.AccountNumber;
                cmd.Parameters[PARAM_STATUS].Value = status;

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( ex, c_log );
            }
            finally
            {
                Close( cmd );
            }
        }

        /// <exception cref="Exception"><c>Exception</c>.</exception>
        private void RemoveWorklistProcessingRecordFor( WorklistActivity worklistActivity )
        {
            iDB2Command cmd = null;

            try
            {
                cmd = CommandFor(
                    string.Format( "CALL {0} ({1},{2})",
                                  SP_DELETEWRKLSTPROCESSINGRECFOR,
                                  PARAM_HSPNUMBER,
                                  PARAM_ACCOUNTNUMBER ),
                    CommandType.Text,
                    worklistActivity.Facility );

                cmd.Parameters[PARAM_HSPNUMBER].Value = worklistActivity.Facility.Oid;
                cmd.Parameters[PARAM_ACCOUNTNUMBER].Value = worklistActivity.AccountNumber;

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( ex, c_log );
            }
            finally
            {
                Close( cmd );
            }
        }

        #endregion

        #region Construction and Finalization
        public WorklistPBARBroker()
            : base()
        {
        }
        public WorklistPBARBroker( string cxnString )
            : base( cxnString )
        {
        }

        public WorklistPBARBroker( IDbTransaction txn )
            : base( txn )
        {
        }
        #endregion

        #region Data Elements

        private readonly Hashtable actionList = new Hashtable();

        private static readonly ILog c_log =
            LogManager.GetLogger( typeof( WorklistPBARBroker ) );

        #endregion

        #region Constants

        private const string

            SP_DELETEWORKLISTSITEMS = "DeleteWorklistItemsFORACCOUNT",
            SP_INSERTWORKLISTSITEMS = "InsertWorklistItemsFORACCOUNT",
            SP_REMAININGCOMMANDSFOR = "REMAININGACTIONSFORACCOUNT",
            SP_GETNEXTACCOUNTFOR = "GETNEXTACCTFORWRKPROCESSINGFOR",
            SP_RESETWRKLSTPROCESSINGRECFOR = "RESETWRKLSTPROCESSINGRECFOR",
            SP_DELETEWRKLSTPROCESSINGRECFOR = "DELETEWRKLSTPROCESSINGRECFOR";

        private const string
            PARAM_FACILITYID = "@P_FacilityID",
            PARAM_ACCOUNTNUMBER = "@P_AccountNumber",
            PARAM_STATUS = "@P_Status",
            PARAM_WORKLISTID = "@P_WorklistID",
            PARAM_COMPOSITEID = "@P_COMPOSITERULEID",
            PARAM_RULEID = "@P_RULEID",

            PARAM_HSPNUMBER = "@P_HSP",
            OUT_PARAM_ACCOUNTNUMBER = "@O_ACCOUNTNUMBER",
            OUT_PARAM_ACTIVITY = "@O_ACTIVITY",
            OUT_PARAM_MEDICALRECORDNUMBER = "@O_MEDICALRECORDNUMBER",

            COL_WORKLISTID = "WORKLISTID",
            COL_COMPOSITERULEID = "COMPOSITERULEID",
            COL_RULEID = "RULEID";

        private const long
            COMPLETE_POST_MSE_REGISTRATION = 18;
        private const long
            CANCEL_ACTIVATE_WALKIN_ACCOUNT = 58;
        private const string
            WORKLISTPROCESSSTATUS_ERROR = "E";

        #endregion
    }
}
