using System;

namespace PatientAccess.Domain.Auditing.FusNotes
{
	//TODO: Create XML summary comment for FusActivity
    [Serializable]
    public class FusActivity : CodedReferenceValue, IComparable
    {
        #region Event Handlers
        #endregion

        #region Methods

        public static FusActivity CreateIrataFusActivity()
        {

            return new FusActivity( FUSACTIVITY_CODE_IRATA, FUSFORMATTER_IRATA );

        }

        public static FusActivity CreateRssvsFusActivity()
        {

            return new FusActivity( FUSACTIVITY_CODE_RSSVS, FUSFORMATTER_RSSVS );

        }


        public static FusActivity CreateInpofFusActivity()
        {

            return new FusActivity( FUSACTIVITY_CODE_INPOF, FUSFORMATTER_INPOF );

        }

        public static FusActivity CreateIhippFusActivity()
        {

            return new FusActivity( FUSACTIVITY_CODE_IHIPP, FUSFORMATTER_IHIPP );

        }
        public static FusActivity CreateIicosFusActivity()
        {

            return new FusActivity( FUSACTIVITY_CODE_IICOS, FUSFORMATTER_IICOS );

        }

        public static FusActivity CreateIcoscFusActivity()
        {

            return new FusActivity( FUSACTIVITY_CODE_ICOSC, FUSFORMATTER_ICOSC );

        }

        public int CompareTo( object obj )
        {
            if( obj is FusActivity )
            {
                FusActivity activityCode = ( FusActivity )obj;

                return this.Code.CompareTo( activityCode.Code );
            }

            throw new ArgumentException( "Object is not a FusActivity" );
        }

        public override string ToString()
        {
            string code = string.Empty;

            if( this.Code.Length != 5 )
            {
                code = this.Code.PadRight( 5, ' ' );
            }
            else
            {
                code = this.Code;
            }

            return String.Format("{0} {1}", code, Description);
        }
        #endregion

        #region Properties
   
        public string StrategyName
        {
            get
            {
                return i_StrategyName;
            }
            set
            {
                i_StrategyName = value;
            }
        }

        public bool Writeable
        {
            get
            {
                return i_Writeable;
            }
            private set
            {
                i_Writeable = value;
            }
        }
   
        public int DefaultWorklistDays
        {
            get
            {
                return i_DefaultWorklistDays;
            }
            private set
            {
                i_DefaultWorklistDays = value;
            }
        }
   
        public int MaxWorklistDays
        {
            get
            {
                return i_MaxWorklistDays;
            }
            private set
            {
                i_MaxWorklistDays = value;
            }
        }
   
        public FusActivityNoteType NoteType
        {
            get
            {
                return i_NoteType;
            }
            private set
            {
                i_NoteType = value;
            }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public FusActivity()
        {
        }

        private FusActivity( string code )
        {
            this.Code = code;
        }
        
        private FusActivity( string code, string formattingStrategy ) : this( code )
        {

            this.StrategyName = formattingStrategy;

        }

        public FusActivity( long oid, DateTime version, string description )
            : base( oid, version, description )
        {
        }

        private FusActivity( long oid, DateTime version, string description, string code )
            : base( oid, version, description, code )
        {
        }

        public FusActivity( string code, 
                            string description, 
                            int defaultWorklistDays, 
                            int maxWorklistDays, 
                            FusActivityNoteType noteType,
                            bool writeable )
            : this( NEW_OID, NEW_VERSION, description, code )
        {
            this.Code                   = code;
            this.Description            = description;
            this.DefaultWorklistDays    = defaultWorklistDays;
            this.MaxWorklistDays        = maxWorklistDays;
            this.NoteType               = noteType;
            this.Writeable              = writeable;
        }

        #endregion

        #region Data Elements
      
        private string              i_StrategyName = string.Empty;
        private int                 i_DefaultWorklistDays;
        private int                 i_MaxWorklistDays;
        private FusActivityNoteType i_NoteType;
        private bool                i_Writeable = false;

        #endregion

        #region Constants

        private const string FUSACTIVITY_CODE_PTRPR = "PTRPR";
        private const string FUSACTIVITY_CODE_RVINB = "RVINB";
        private const string FUSACTIVITY_CODE_RVINS = "RVINS";
        private const string FUSACTIVITY_CODE_RDOTV = "RDOTV";
        private const string FUSACTIVITY_CODE_IMEVC = "IMEVC";
        private const string FUSACTIVITY_CODE_MCWFI = "MCWFI";
        private const string FUSACTIVITY_CODE_RBVCA = "RBVCA";
        private const string FUSACTIVITY_CODE_RARRA = "RARRA";
        private const string FUSACTIVITY_CODE_RPFCR = "RPFCR";
        private const string FUSACTIVITY_CODE_RFCMO = "RFCMO";
        private const string FUSACTIVITY_CODE_RFCAC = "RFCAC";
        private const string FUSACTIVITY_CODE_RCALC = "RCALC";
        private const string FUSACTIVITY_CODE_RFCNL = "RFCNL";
        private const string FUSACTIVITY_CODE_IICOS = "IICOS";
        private const string FUSACTIVITY_CODE_ICOSC = "ICOSC";
        private const string FUSACTIVITY_CODE_IHIPP = "IHIPP";
        private const string FUSACTIVITY_CODE_INPOF = "INPOF";
        private const string FUSACTIVITY_CODE_RYCHN = "RYCHN";
        private const string FUSACTIVITY_CODE_REIFA = "REIFA";
        private const string FUSACTIVITY_CODE_RSSVS = "RSSVS";
        private const string FUSACTIVITY_CODE_CREFP = "CREFP";
        private const string FUSACTIVITY_CODE_RUPPD = "RUPPD";
        private const string FUSACTIVITY_CODE_IRATA = "IRATA";
        private const string FUSACTIVITY_CODE_RNPCA = "RNPCA";

        private const string FUSFORMATTER_PTRPR = "PTRPRFUSNoteFormatter";
        private const string FUSFORMATTER_RBVCA = "RBVCAFusNoteFormatter";
        private const string FUSFORMATTER_RARRA = "RARRAFUSNoteFormatter";
        private const string FUSFORMATTER_RPFCR = "RPFCRFUSNoteFormatter";
        private const string FUSFORMATTER_RFCMO = "RFCMOFUSNoteFormatter";
        private const string FUSFORMATTER_RFCAC = "RFCACFUSNoteFormatter";
        private const string FUSFORMATTER_RCALC = "RCALCFUSNoteFormatter";
        private const string FUSFORMATTER_RFCNL = "RFCNLFUSNoteFormatter";
        private const string FUSFORMATTER_CREFP = "CREFPFusNoteFormatter";
        private const string FUSFORMATTER_RUPPD = "RUPPDFUSNoteFormatter";
        private const string FUSFORMATTER_IRATA = "IRATAFUSNoteFormatter";
        private const string FUSFORMATTER_RNPCA = "RNPCAFUSNoteFormatter";
        private const string FUSFORMATTER_TITLEONLY = "TitleOnlyFusNoteFormatter";
        private const string FUSFORMATTER_IICOS = "COSSignedFUSNoteFormatter";
        private const string FUSFORMATTER_ICOSC = "COSSignedFUSNoteFormatter";
        private const string FUSFORMATTER_IHIPP = "OptOutFUSNoteFormatter";
        private const string FUSFORMATTER_INPOF = "INPOFFUSNoteFormatter";
        private const string FUSFORMATTER_RYCHN = "RYCHNFUSNoteFormatter";
        private const string FUSFORMATTER_RSSVS = "RSSVSFUSNoteFormatter";
        private const string FUSFORMATTER_REIFA = "REIFAFUSNoteFormatter";

        #endregion

        #region Enumerations

        // Valid FUS Activity Note Types returned from the database
        public enum FusActivityNoteType
        {
            TypeInvalid = -1,
            Type00 = 0,
            Type01 = 1,
            Type02 = 2,
            Type03 = 3,
            Type04 = 4,
            Type05 = 5,
            Type06 = 6,
            Type07 = 7,
            Type10 = 10,
            Type12 = 12
        }

        #endregion
    }
}
