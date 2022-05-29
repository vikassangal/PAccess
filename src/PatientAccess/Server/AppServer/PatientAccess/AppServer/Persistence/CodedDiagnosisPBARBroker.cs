using System;
using System.Collections;
using System.Data;
using Extensions.DB2Persistence;
using IBM.Data.DB2.iSeries;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using log4net;

namespace PatientAccess.Persistence
{
    //TODO: Create XML summary comment for CodedDiagnosisBroker
    [Serializable]
    public class CodedDiagnosisPBARBroker : AbstractPBARBroker, ICodedDiagnosisBroker
    {
        #region Event Handlers
        #endregion

        #region Methods
        /// <summary>
        /// CodedDiagnosisFor a specific facility ,accountNumber
        /// </summary>
        /// <param name="hsp"></param>
        /// <param name="accountNumber"></param>
        /// <param name="mrc"></param>
        /// <param name="forPreMse"></param>
        /// <param name="facility"></param>
        /// <returns></returns>
        /// <exception cref="Exception">CodedDiagnosisPBARBroker failed to initialize</exception>
        public CodedDiagnoses CodedDiagnosisFor(long hsp, long accountNumber, long mrc, bool forPreMse, Facility facility )
        {
            CodedDiagnoses codedDiagnoses = new CodedDiagnoses();
            ArrayList codes = new ArrayList();
            ArrayList admittingCodes = new ArrayList();

            iDB2Command cmd = null;
            SafeReader reader = null;
            
            try
            {
                
                string cmdText = String.Format (
                    "CALL {0}({1},{2},{3})",
                    forPreMse ? SP_SELECT_CODED_DIAGS_FOR_PAT : SP_SELECT_CODED_DIAGS_FOR_ACCT,
                    PARAM_HSP,
                    PARAM_ACCOUNTNUMBER,	
                    PARAM_MRC);

                cmd = this.CommandFor( cmdText, CommandType.Text, facility ); 

                cmd.Parameters[PARAM_HSP].Value    = hsp;
                cmd.Parameters[PARAM_ACCOUNTNUMBER].Value    = accountNumber;
                cmd.Parameters[PARAM_MRC].Value    = mrc;

                reader = this.ExecuteReader( cmd ) ; 

                while( reader.Read() )
                {
                    string paramName;

                    for (int x = 1; x <= NUMBER_OF_CODED_DIAG; x++)
                    {
                        paramName = "CD0" + x;
                        if( reader.IsDBNull(paramName) )
                        {
                            codes.Add(string.Empty);
                        }
                        else
                        {
                            codes.Add(reader.GetString(paramName).Trim());
                        }
                    }
                    // Admitting diagnosis codes
                    for (int x = 1; x <= NUMBER_OF_ADMITTING_DIAG; x++)
                    {
                        paramName = "RA0" + x;
                        if( reader.IsDBNull(paramName) )
                        {
                            admittingCodes.Add(string.Empty);
                        }
                        else
                        {
                            admittingCodes.Add(reader.GetString(paramName).Trim());
                        }
                    }
                }

                // if there were no codes found make sure there are 8 empty strings in the arraylist
                if(codes.Count != NUMBER_OF_CODED_DIAG)
                {
                    for (int x = 0; x < NUMBER_OF_CODED_DIAG; x++)
                    {
                        codes.Add(string.Empty);
                    }
                }
                if(admittingCodes.Count != NUMBER_OF_ADMITTING_DIAG)
                {
                    for (int x = 0; x < NUMBER_OF_ADMITTING_DIAG; x++)
                    {
                        admittingCodes.Add(string.Empty);
                    }
                }
            }
            catch(Exception ex)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( "CodedDiagnosisPBARBroker failed to initialize", ex, c_log );
            }
            finally
            {
                base.Close( reader );
                base.Close( cmd );
            }
            codedDiagnoses.CodedDiagnosises = codes;
            codedDiagnoses.AdmittingCodedDiagnosises = admittingCodes;

            return codedDiagnoses;
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public CodedDiagnosisPBARBroker()
        {
        }
        #endregion

        #region Data Elements

        private static readonly ILog c_log =
            LogManager.GetLogger( typeof( CodedDiagnosisPBARBroker ) );

        #endregion

        #region Constants
        
        private const string SP_SELECT_CODED_DIAGS_FOR_PAT = "SELECTCODEDDIAGSFORPAT";
        private const string SP_SELECT_CODED_DIAGS_FOR_ACCT = "SELECTCODEDDIAGSFORACCT";

        private const string 
            PARAM_HSP           = "@P_HSP",
            PARAM_ACCOUNTNUMBER = "@P_ACCTNUMBER",	
            PARAM_MRC           = "@P_MRC";

        private const int 
            NUMBER_OF_CODED_DIAG   = 8,
            NUMBER_OF_ADMITTING_DIAG = 5;

        #endregion
    }
}
