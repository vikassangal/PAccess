using System;
using System.Collections;

namespace PatientAccess.Domain.Auditing.FusNotes
{
    /// <summary>
    /// Summary description for FUSNoteFacory.
    /// </summary>
    //TODO: Create XML summary comment for FusNoteFactory
    [Serializable]
    public class FusNoteFactory 
    {
        #region Event Handlers
        #endregion

        #region Methods
        /// <summary>
        /// Generate FUS note for a primary payor
        /// </summary>
        /// <param name="anAccount">Account</param>
        /// <param name="aCoverage">Coverage</param>
        /// <param name="originalCoverage">The original coverage.</param>
        public void AddRVINBNoteTo( Account anAccount, Coverage aCoverage, Coverage originalCoverage )
        {
            if( aCoverage.CoverageOrder.Oid != CoverageOrder.PRIMARY_OID )
            {
                throw new ApplicationException(
                    "Writing a FUS Note with ActivityCode of RVINVB is only valid for Primary coverage");
            }
                     AddInsuranceVerificationFUSNote( anAccount, aCoverage, originalCoverage, RVINBActivityCode );
        }

        /// <summary>
        /// Generate FUS notes for a secondary payor
        /// </summary>
        /// <param name="anAccount">Account</param>
        /// <param name="aCoverage">Coverage</param>
        /// <param name="originalCoverage">The original coverage.</param>
        public void AddRVINSNoteTo( Account anAccount, Coverage aCoverage, Coverage originalCoverage )
        {
            if( aCoverage.CoverageOrder.Oid != CoverageOrder.SECONDARY_OID )
            {
                throw new ApplicationException(
                    "Writing a FUS Note with ActivityCode of RVINVS is only valid for Secondary coverage");
            }
         
            AddInsuranceVerificationFUSNote( anAccount, aCoverage, originalCoverage, RVINSActivityCode );
        }
        public void AddRDOTVNoteTo( Account anAccount, Coverage aCoverage, Coverage originalCoverage )
        {
            AddInsuranceVerificationFUSNote( anAccount, aCoverage, originalCoverage, RDOTVActivityCode );
        }

        /// <summary>
        /// Generate FUS notes for Medicaid Coverage category
        /// </summary>
        /// <param name="anAccount">Account</param>
        /// <param name="aCoverage">Coverage</param>
        /// <param name="originalCoverage">The original coverage.</param>
        public void AddIMEVCNoteTo( Account anAccount, Coverage aCoverage, Coverage originalCoverage )
        {
          
            if( !aCoverage.GetType().Equals( typeof(GovernmentMedicaidCoverage )) )
            {
                throw new ApplicationException(
                    "Writing a FUS Note with ActivityCode of IMEVC is only valid for Medicaid coverage");
            }
            AddInsuranceVerificationFUSNote( anAccount, aCoverage, originalCoverage, IMEVCActivityCode );
        }

        /// <summary>
        /// Generate FUS notes for Medicare Coverage category
        /// </summary>
        /// <param name="anAccount">Account</param>
        /// <param name="aCoverage">Coverage</param>
        /// <param name="originalCoverage">The original coverage.</param>
        public void AddMCWFINoteTo( Account anAccount, Coverage aCoverage, Coverage originalCoverage )
        {
            if( !aCoverage.GetType().Equals( typeof(GovernmentMedicareCoverage )) )
            {
                throw new ApplicationException(
                    "Writing a FUS Note with ActivityCode of MCWFI is only valid for Medicare coverage");
            }
            AddInsuranceVerificationFUSNote( anAccount, aCoverage, originalCoverage, MCWFIActivityCode );
        }



        /// <summary>
        /// Generate FUS notes for Benefits Verification notes
        /// </summary>
        /// <param name="anAccount">Account</param>
        /// <param name="aCoverage">Coverage</param>
        /// <param name="originalCoverage">The original coverage.</param>
        public void AddRBVCANoteTo( Account anAccount, Coverage aCoverage, Coverage originalCoverage )
        {
            AddInsuranceVerificationFUSNote( anAccount, aCoverage, originalCoverage, RBVCAActivityCode,
                RBVCAFusNoteFormatter);
        }

        /// <summary>
        /// Generate FUS notes for Authorization Required Notes
        /// </summary>
        /// <param name="anAccount">Account</param>
        /// <param name="aCoverage">Coverage</param>
        /// <param name="originalCoverage">The original coverage.</param>
        public void AddRARRANoteTo( Account anAccount, Coverage aCoverage, Coverage originalCoverage )
        {
            AddInsuranceVerificationFUSNote( anAccount, aCoverage, originalCoverage, RARRAActivityCode,
                RARRAFUSNoteFormatter );
        }

        /// <summary>
        /// Generate FUS notes for Financial Payment Liability type - RPFCR
        /// (SYS-FIN Counselled Patient for Financial Responsibility)
        /// </summary>
        /// <param name="anAccount">Account</param>
        public void AddRPFCRNoteTo( Account anAccount )
        {
            AddFinancialFUSNote( anAccount, RPFCRActivityCode, RPFCRFUSNoteFormatter );
        }

        /// <summary>
        /// Generate FUS notes for Financial Payment Liability type - RFCMO
        /// (SYS-FIN Counselled Monthly Payment Agreement)
        /// </summary>
        /// <param name="anAccount">Account</param>
        public void AddRFCMONoteTo( Account anAccount )
        {
            AddFinancialFUSNote( anAccount, RFCMOActivityCode, RFCMOFUSNoteFormatter );
        }

        /// <summary>
        /// Generate FUS notes for Financial Payment Liability type - RFCAC
        /// (SYS-FIN Counselled Amount Collected)
        /// </summary>
        /// <param name="anAccount">Account</param>
        public void AddRFCACNoteTo( Account anAccount )
        {
            AddFinancialFUSNote( anAccount, RFCACActivityCode, RFCACFUSNoteFormatter );
        }

        /// <summary>
        /// Generate FUS notes for Financial Payment Liability type - RCALC
        /// (Patient Liability Calculation)
        /// </summary>
        /// <param name="anAccount">Account</param>
        public void AddRCALCNoteTo( Account anAccount )
        {
            AddFinancialFUSNote( anAccount, RCALCActivityCode, RCALCFUSNoteFormatter );
        }

        /// <summary>
        /// Generate FUS notes for Financial Payment Liability type - RFCNL
        /// (SYS-NO Patient Liability)
        /// </summary>
        /// <param name="anAccount">Account</param>
        public void AddRFCNLNoteTo( Account anAccount )
        {
            AddFinancialFUSNote( anAccount, RFCNLActivityCode, RFCNLFUSNoteFormatter );
        }


        public void AddPTRPRFUSNoteTo( Account account )
        {
            GenerateFUSNote( account, privateRoomActivityCode, PTRPRFUSNoteFormatter );
        }
        
        public void AddIICOSFUSNoteTo( Account account )
        {
            GenerateFUSNote( account, IICOSActivityCode, COSSignedFUSNoteFormatter );
        }
        public void AddRPUTSFUSNoteTo( Account account )
        {
            GenerateFUSNote( account, RPUTSActivityCode, COSSignedFUSNoteFormatter );
        }
        public void AddRPRTSFUSNoteTo( Account account )
        {
            GenerateFUSNote( account, RPRTSActivityCode, COSSignedFUSNoteFormatter );
        }
        public void AddRPNASFUSNoteTo( Account account )
        {
            GenerateFUSNote( account, RPNASActivityCode, COSSignedFUSNoteFormatter );
        }
        public void AddICOSCFUSNoteTo( Account account )
        {
            GenerateFUSNote( account, ICOSCActivityCode, COSSignedFUSNoteFormatter );
        }
        public void AddIHIPPFUSNoteTo( Account account )
        {
            GenerateFUSNote( account, IHIPPActivityCode, OPTOUTFUSNoteFormatter );
        }
        public void AddINPOFFUSNoteTo( Account account )
        {
            GenerateFUSNote( account, INPOFActivityCode, INPOFFUSNoteFormatter );
        }
         public void AddRYCHNFUSNoteTo( Account account )
        {
            GenerateFUSNote( account, RYCHNActivityCode, RYCHNFUSNoteFormatter );
        }
         public void AddRSSVSFUSNoteTo( Account account )
        {
            GenerateFUSNote( account, RSSVSActivityCode, RSSVSFUSNoteFormatter );
        }
		
        public void AddRPRESFUSNoteTo(Account account)
	    {
	        GenerateFUSNote(account, RPRESActivityCode, RPRESFUSNoteFormatter);
	    }

        public void AddRGAPMFusNoteTo(Account account)
        {
            GenerateFUSNote(account, RGAPMActivityCode, RGAPMFUSNoteFormatter);
        }
		
        #endregion

        #region Properties
        #endregion

        #region Private Methods

        private void AddInsuranceVerificationFUSNote(   Account anAccount,
            Coverage aCoverage, 
            Coverage originalCoverage, 
            string code, 
            string strategyName )
        {
            FusActivity activity = new FusActivity();
            activity.Code = code;
            activity.StrategyName = strategyName;

            FusNote note = new FusNote( aCoverage, originalCoverage, activity );
            note.FusActivity = activity;
            
            anAccount.AddFusNote(note);
        }

        private void AddInsuranceVerificationFUSNote(Account anAccount, 
            Coverage aCoverage, 
            Coverage originalCoverage, 
            string code)
        {
            string formatStrategyName = (string)this.i_CoverageStrategyTable[aCoverage.GetType()];
            
            if( formatStrategyName != null )
            {
                FusActivity activity = new FusActivity();
                activity.Code = code;
                activity.StrategyName = formatStrategyName;

                FusNote note = new FusNote( aCoverage, originalCoverage, activity );

                note.FusActivity = activity;

                anAccount.AddFusNote(note);
            }
        }

        //Patient Liability Notes
        private void AddFinancialFUSNote( Account anAccount, string code, string strategyName )
        {
            FusActivity activity = new FusActivity();
            activity.Code = code;
            activity.StrategyName = strategyName;

            FusNote note = new FusNote( anAccount, null, activity );
            note.FusActivity = activity;
            anAccount.AddFusNote( note );
        }


	    private void InitCoverageStrategyTable()
        {
            this.i_CoverageStrategyTable.Add(typeof(CommercialCoverage),"CommercialFusFormatter");
            this.i_CoverageStrategyTable.Add(typeof(GovernmentMedicaidCoverage),"MedicaidFusFormatter");
            this.i_CoverageStrategyTable.Add(typeof(GovernmentMedicareCoverage),"MedicareFusFormatter");
            this.i_CoverageStrategyTable.Add(typeof(GovernmentOtherCoverage),"GovernmentMiscFusFormatter");
            this.i_CoverageStrategyTable.Add(typeof(SelfPayCoverage),"SelfPayFusFormatter");
            this.i_CoverageStrategyTable.Add(typeof(WorkersCompensationCoverage),"WorkersCompFusFormatter");
            this.i_CoverageStrategyTable.Add(typeof(OtherCoverage),"OtherCoverageFusFormatter");
        }


	    public void AddCREFPFUSNote( Account anAccount )
        {
            GenerateFUSNote( anAccount, CREFPActivityCode, CREFPFusNoteFormatter );

        }
        public void AddIRATAFUSNote( Account anAccount )
        {
            GenerateFUSNote( anAccount, IRATAActivityCode, IRATAFusNoteFormatter );

        }
        public void AddRUPPDFUSNote( Account anAccount )
        {
            GenerateFUSNote( anAccount, RUPPDActivityCode, RUPPDFusNoteFormatter );

        }
        public void AddRESRDFUSNote(Account anAccount)
        {
            GenerateFUSNote(anAccount, RESRDActivityCode, RESRDFusNoteFormatter);
        }

	    public void AddRNPCAFUSNote( Account anAccount )
        {
            GenerateFUSNote( anAccount, RNPCAActivityCode, RNPCAFusNoteFormatter );

        }
        public void AddDOFRPFUSNote(Account anAccount, DOFRAPIResponse dOFRAPIResponse)
        {
            FusNote note = new ExtendedFUSNote();
            FusActivity activity = new FusActivity();
            activity.Code = DOFRPActivityCode;
            activity.StrategyName = DOFPRFUSNoteFormatter;
            
            note.Account = anAccount;
            note.Context = anAccount;
            note.FusActivity = activity;
            note.ManuallyEntered = true;
            note.UserID = anAccount.Activity.AppUser.PBAREmployeeID.ToUpper();
            string formattedString;
            if (dOFRAPIResponse !=null && dOFRAPIResponse.planId !=null )
            {
                formattedString = String.Format("Plan ID: {0}, Plan Name: {1}, and Service Category: {2} has the highest probability of being accepted by the payer for this patient encounter ",
                    dOFRAPIResponse.planId, dOFRAPIResponse.planName, dOFRAPIResponse.serviceCategory);
            }
            else
            {
                formattedString = "Decision Analysis Tool (DAT) was not able to find a recommended Plan ID, Plan Name and Service Category for this patient encounter ";
            }

            IList noteMessages = note.CreateFUSNoteMessages(anAccount);
            if (noteMessages != null && noteMessages.Count > 0)
            {
                formattedString = formattedString + FormatFusNote((ArrayList)noteMessages);
            }
            note.Remarks = formattedString;
            anAccount.AddFusNote(note);
        }

        private string FormatFusNote(ArrayList noteMessages)
        {
            string formattedString = String.Empty;
            if (noteMessages.Count > 0)
            {

                for (int i = 0; i < noteMessages.Count; i++)
                {
                    formattedString = formattedString + noteMessages[i];
                }
            }
            return formattedString;
        }
        private void GenerateFUSNote( Account anAccount, string code, string strategyName )
        {
            FusActivity activity = new FusActivity();
            activity.Code = code;
            activity.StrategyName = strategyName;

            FusNote note = new FusNote( anAccount, null, activity );
            anAccount.AddFusNote( note );
        }
        
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public FusNoteFactory()
        {
            this.InitCoverageStrategyTable();
        }
        #endregion

        #region Data Elements

	    readonly Hashtable i_CoverageStrategyTable = new Hashtable();
        #endregion

        #region Constants
        private const string privateRoomActivityCode = "PTRPR";
        private const string PTRPRFUSNoteFormatter = "PTRPRFUSNoteFormatter";
        private const string RVINBActivityCode = "RVINB";
        private const string RVINSActivityCode = "RVINS";
        private const string RDOTVActivityCode = "RDOTV";
        private const string IMEVCActivityCode = "IMEVC";
        private const string MCWFIActivityCode = "MCWFI";
        private const string RBVCAActivityCode = "RBVCA";
        private const string RARRAActivityCode = "RARRA";
        private const string RPFCRActivityCode = "RPFCR";
        private const string RFCMOActivityCode = "RFCMO";
        private const string RFCACActivityCode = "RFCAC";
        private const string RCALCActivityCode = "RCALC";
        private const string RFCNLActivityCode = "RFCNL";
        private const string IICOSActivityCode = "IICOS";
        private const string RPRTSActivityCode = "RPRTS";
        private const string RPNASActivityCode = "RPNAS";
        private const string RPUTSActivityCode = "RPUTS";
        private const string ICOSCActivityCode = "ICOSC";
        private const string IHIPPActivityCode = "IHIPP";
        private const string INPOFActivityCode = "INPOF";
        private const string RYCHNActivityCode = "RYCHN";
        private const string REIFAActivityCode = "REIFA";
        private const string RSSVSActivityCode = "RSSVS";
        private const string RPRESActivityCode = "RPRES";
	 
        private const string CREFPActivityCode = "CREFP";
        private const string RUPPDActivityCode = "RUPPD";
        private const string RESRDActivityCode = "RESRD";
        private const string IRATAActivityCode = "IRATA";
        private const string RNPCAActivityCode = "RNPCA";
        private const string RGAPMActivityCode = "RGAPM";
        private const string DOFRPActivityCode = "DOFRP";
        private const string RBVCAFusNoteFormatter = "RBVCAFusNoteFormatter";
        private const string RARRAFUSNoteFormatter = "RARRAFUSNoteFormatter";
        private const string RPFCRFUSNoteFormatter = "RPFCRFUSNoteFormatter";
        private const string RFCMOFUSNoteFormatter = "RFCMOFUSNoteFormatter";
        private const string RFCACFUSNoteFormatter = "RFCACFUSNoteFormatter";
        private const string RCALCFUSNoteFormatter = "RCALCFUSNoteFormatter";
        private const string RFCNLFUSNoteFormatter = "RFCNLFUSNoteFormatter";
        private const string CREFPFusNoteFormatter = "CREFPFusNoteFormatter";
        private const string RUPPDFusNoteFormatter = "RUPPDFUSNoteFormatter";
        private const string RESRDFusNoteFormatter = "RESRDFUSNoteFormatter";
        private const string IRATAFusNoteFormatter = "IRATAFUSNoteFormatter";
        private const string RNPCAFusNoteFormatter = "RNPCAFUSNoteFormatter";
        private const string TITLEONLYFUSNoteFormatter = "TitleOnlyFusNoteFormatter";
        private const string COSSignedFUSNoteFormatter = "COSSignedFUSNoteFormatter";
        private const string OPTOUTFUSNoteFormatter = "OptOutFUSNoteFormatter";
        private const string INPOFFUSNoteFormatter = "INPOFFUSNoteFormatter";
        private const string RYCHNFUSNoteFormatter = "RYCHNFUSNoteFormatter";
        private const string RSSVSFUSNoteFormatter = "RSSVSFUSNoteFormatter";
        private const string REIFAFUSNoteFormatter = "REIFAFUSNoteFormatter";
	    private const string RPRESFUSNoteFormatter = "RPRESFUSNoteFormatter";
        private const string RGAPMFUSNoteFormatter = "RGAPMFUSNoteFormatter";
        private const string DOFPRFUSNoteFormatter = "DOFPRFUSNoteFormatter";



        #endregion
    }
}
