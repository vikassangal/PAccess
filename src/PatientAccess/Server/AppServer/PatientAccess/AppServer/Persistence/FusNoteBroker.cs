using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Extensions.Persistence;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Auditing.FusNotes;
using PatientAccess.Services.Hdi;
using log4net;

namespace PatientAccess.Persistence
{
    [Serializable]
    public class FusNoteBroker : CodesBroker, IFUSNoteBroker
    {
		#region Constants 

        private const string CODE_RFCMO = "RFCMO";
        private const string CODE_RFCAC = "RFCAC";
        private const string CODE_RCALC = "RCALC";
        private const string CODE_RFCNL = "RFCNL";
        private const string CODE_RUPPD = "RUPPD";
        private const string CODE_RVINB = "RVINB";
        private const string CODE_RVINS = "RVINS";
        private const string CODE_DOFRP = "DOFRP";
        private const string DBCOLUMN_ACTIVITY_CODE = "ActivityCode";
        private const string DBCOLUMN_DEFAULT_WORKLIST_DAYS = "DefaultWorklistDays";
        private const string DBCOLUMN_DESCRIPTION = "Description";
        private const string DBCOLUMN_MAX_WORKLIST_DAYS = "MaxWorklistDays";
        private const string DBCOLUMN_NOTE_TYPE = "NoteType";
        private const string DBCOLUMN_WRITABLE = "Writeable";
        private const string DBPROCEDURE_SELECT_ALL_ACTIVITY_CODES = "FusNote.SelectAllActivityCodes";
        private const string DBPROCEDURE_SELECT_ALL_WRITABLE_ACTIVITY_CODES = "FusNote.SelectAllWriteableActivityCodes";
        private const string INVALID_ACTIVITY_DESCRIPTION = "* INACTIVE ACTIVITY CODE";

        #endregion Constants 

		#region Fields 

        private static readonly ILog _logger = LogManager.GetLogger( typeof( FusNoteBroker ) );

		#endregion Fields 

		#region Constructors 

        public FusNoteBroker( string cxnString )
            : base( cxnString )
        {
        }


        public FusNoteBroker( SqlTransaction txn )
            : base( txn )
        {
        }


        public FusNoteBroker()
        {
        }

		#endregion Constructors 

		#region Properties 

        private Hashtable AllCodesHashtable
        {
            get
            {
                CacheManager cacheManager = new CacheManager();
                Hashtable allCodesHashtable =
                    (Hashtable)cacheManager.GetCollectionBy(CacheKeys.CACHE_KEY_FOR_ACTIVITY_CODES, this.LoadAllActivityCodes);

                return allCodesHashtable;   
            }
        }


        private Hashtable AllWriteableCodesHashtable
        {
            get
            {
                CacheManager cacheManager = new CacheManager();
                Hashtable allWriteableCodesHashtable =
                    (Hashtable)cacheManager.GetCollectionBy(CacheKeys.CACHE_KEY_FOR_WRITEABLE_ACTIVITY_CODES, this.LoadAllWriteableActivityCodes);

                return allWriteableCodesHashtable;
            }
        }

		#endregion Properties 

		#region Methods 

        public ICollection AllActivityCodes()
        {
            ArrayList returnArray = new ArrayList( this.AllCodesHashtable.Values );

            // load a 'blank' value to the beginning of the list and sort
            returnArray.Insert( 0, new FusActivity() );
            returnArray.Sort();

            return returnArray;
        }


        public Hashtable AllActivityCodesHashtable()
        {
            return this.AllCodesHashtable;
        }


        public ICollection AllWriteableActivityCodes()
        {
            ArrayList returnArray = new ArrayList( this.AllWriteableCodesHashtable.Values );

            // load a 'blank' value to the beginning of the list and sort
            returnArray.Insert( 0, new FusActivity() );
            returnArray.Sort();

            return returnArray;
        }


        public Hashtable AllWriteableActivityCodesHashtable()
        {
            return this.AllWriteableCodesHashtable;
        }


        public FusActivity FusActivityWith( string activityCode )
        {
            FusActivity anActivity;
            if (activityCode == string.Empty)
            {
                anActivity = new FusActivity();
            }
            else
            {
                anActivity = AllCodesHashtable[activityCode] as FusActivity;
            }

            if (anActivity == null ||
                                (anActivity != null && !anActivity.IsValid))
            {
                anActivity = new FusActivity();
                anActivity.Code = activityCode;
                anActivity.Description = INVALID_ACTIVITY_DESCRIPTION;
                anActivity.IsValid = false;
            }
            return anActivity;
        }


        public ICollection GetMergedFUSNotesFor( Account anAccount )
        {
            HDIService hdiService = new HDIService();

            var allFusNotes = hdiService.GetFUSNotesFor( anAccount );
            var extendedFusNotes = allFusNotes.Cast<ExtendedFUSNote>().ToList();
            var compressedFusNotes = MergeExtensionNotesWithParentNotes( extendedFusNotes );
            return new ArrayList( compressedFusNotes.ToArray() );
            //return compressedFusNotes.ToArray();
            //return allFusNotes;
        }


        public void PostRemarksFusNote( Account anAccount, string userId,
                                        FusActivity activity, string remarks, DateTime noteDateTime )
        {
            HDIService hdiService = new HDIService();
            ExtendedFUSNote fusNote = new ExtendedFUSNote();

            fusNote.Account = anAccount;
            fusNote.AccountNumber = anAccount.AccountNumber.ToString();
            fusNote.UserID = userId;


            fusNote.FusActivity = activity;
            fusNote.Remarks = remarks;
            fusNote.CreatedOn = noteDateTime;

            hdiService.PostExtendedRemarksFusNote( fusNote );
        }


        /// <summary>
        /// WriteFUSNotes - write all non-persisted FUS notes on the Account instance
        /// </summary>
        /// <param name="anAccount"></param>
        /// <param name="pbarEmployeeId"></param>
        public void WriteFUSNotes( Account anAccount, string pbarEmployeeId )
        {
            // following is a hack to ensure that the timestamps are 'unique' thereby ensuring that
            // cremc notes don't get associated with the wrong parent activity note

            DateTime prevDateTime = new DateTime();
            int increment = 0;

            try
            {
                if (anAccount.FusNotes.Count > 0)
                {
                    foreach (FusNote note in anAccount.FusNotes)
                    {
                        // Manually entered notes already have an offset (in seconds) for the number of CREMC notes that
                        // will be generated... we must calc the increment for the next note (if it should happen to be a
                        // system-generated note, it would use this offset to calculate a new time)

                        if (note.ManuallyEntered)
                        {
                            note.AccountNumber = anAccount.AccountNumber.ToString();
                            note.Account = anAccount;

                            this.PostExtendedFUSNote( note as ExtendedFUSNote );

                            prevDateTime = note.CreatedOn;
                            increment = ( note.Remarks.Length / 200 ) + 2;
                        }
                        else
                        {
                            IList noteMessages = note.CreateFUSNoteMessages( anAccount );

                            if (noteMessages != null
                                //&& noteMessages.Count > 0
                               )
                            {
                                string formattedString = this.FormatFusNote( (ArrayList)noteMessages );

                                ITimeBroker timeBroker = BrokerFactory.BrokerOfType<ITimeBroker>();
                                DateTime noteDateTime = timeBroker.TimeAt( anAccount.Facility.GMTOffset, anAccount.Facility.DSTOffset );

                                note.FusActivity = this.FusActivityWith( note.FusActivity.Code );

                                // Compare the facility date/time to the prev note with the offset for the number of CREMC's to be added
                                // If the timestamps will collide, then use the offset from the prev note.

                                if (noteDateTime <= prevDateTime.AddSeconds( increment ))
                                {
                                    noteDateTime = prevDateTime.AddSeconds( increment );
                                }

                                if (note.FusActivity.Code == CODE_RFCMO)
                                {
                                    this.PostExtendedFUSNote( anAccount, pbarEmployeeId, note.FusActivity,
                                        formattedString, anAccount.MonthlyPayment, anAccount.OriginalMonthlyPayment, noteDateTime );
                                }
                                else if (note.FusActivity.Code == CODE_RFCAC)
                                {
                                    decimal amountCollectedToday = anAccount.Payment.CalculateTotalPayments();
                                    decimal previousTotalPaid = anAccount.TotalPaid - amountCollectedToday;

                                    this.PostExtendedFUSNote( anAccount, pbarEmployeeId, note.FusActivity,
                                        formattedString, anAccount.TotalPaid, previousTotalPaid, noteDateTime );
                                }
                                else if (note.FusActivity.Code == CODE_RCALC)
                                {
                                    this.PostExtendedFUSNote( anAccount, pbarEmployeeId, note.FusActivity,
                                        formattedString, anAccount.TotalCurrentAmtDue, anAccount.PreviousTotalCurrentAmtDue, noteDateTime );
                                }
                                else if (note.FusActivity.Code == CODE_RFCNL)
                                {
                                    this.PostExtendedFUSNote( anAccount, pbarEmployeeId, note.FusActivity,
                                        formattedString, anAccount.TotalCurrentAmtDue, 0, noteDateTime );
                                }
                                else if (note.FusActivity.Code == CODE_RUPPD)
                                {
                                    this.PostExtendedFUSNote( anAccount, pbarEmployeeId, note.FusActivity,
                                        formattedString, anAccount.TotalCurrentAmtDue, 0, noteDateTime );
                                }
                                else if (note.FusActivity.Code == CODE_RVINB || note.FusActivity.Code == CODE_RVINS)
                                {
                                    if (formattedString.Trim().Length != 0)
                                    {
                                        this.PostRemarksFusNote( anAccount, pbarEmployeeId, note.FusActivity,
                                                                formattedString, noteDateTime );
                                    }
                                }
                                else
                                {
                                    this.PostRemarksFusNote( anAccount, pbarEmployeeId, note.FusActivity, formattedString, noteDateTime );
                                }

                                // we must calc the increment for the next note (if it should happen to be a
                                // system-generated note, it would use this offset to calculate a new time)

                                prevDateTime = noteDateTime;
                                increment = ( formattedString.Length / 200 ) + 2;

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( ex, _logger );
            }
        }
        /// <summary>
        /// FormatFusNote - format a system-generated FUS note
        /// </summary>
        /// <param name="noteMessages">The note messages.</param>
        /// <returns></returns>
        private string FormatFusNote( ArrayList noteMessages )
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


        /// <summary>
        /// Fus activity from
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        private FusActivity FusActivityFrom(SafeReader reader)
        {
            string activityCode =
                reader.GetString(DBCOLUMN_ACTIVITY_CODE);
            string description =
                reader.GetString(DBCOLUMN_DESCRIPTION);
            int noteType =
                reader.GetInt32(DBCOLUMN_NOTE_TYPE);
            int defaultWorklistDays =
                reader.GetInt32(DBCOLUMN_DEFAULT_WORKLIST_DAYS);
            int maxWorklistDays =
                reader.GetInt32(DBCOLUMN_MAX_WORKLIST_DAYS);
            bool writeable =
                reader.GetString(DBCOLUMN_WRITABLE).ToUpper().Equals(YesNoFlag.CODE_YES);

            FusActivity.FusActivityNoteType activityNoteType;

            // if the value is a valid Note Type available in the enumeration, create corresponding 
            // Enumeration Note type, else create Note Type of TypeInvalid to avoid errors.
            if (Enum.IsDefined(typeof(FusActivity.FusActivityNoteType), noteType))
            {
                activityNoteType = (FusActivity.FusActivityNoteType)noteType;
            }
            else
            {
                activityNoteType = FusActivity.FusActivityNoteType.TypeInvalid;
            }

            FusActivity fusActivity =
                new FusActivity(activityCode, description, defaultWorklistDays,
                                 maxWorklistDays, activityNoteType, writeable);

            return fusActivity;
        }


        /// <summary>
        /// Gets the merged notes for a set of FUS notes created by a single system (user id).
        /// </summary>
        /// <param name="fusNotes">The FUS notes.</param>
        /// <param name="userId">The user ID.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">"When all FUS notes do not have the user ID provided by the argument <c>userId</c>"</exception>
        private static IList<ExtendedFUSNote> GetMergedNotesFor( IEnumerable<ExtendedFUSNote> fusNotes, string userId )
        {
            var fusNoteSets = new List<FusNoteSet>();
            var notesList = fusNotes.OrderBy( x => x.CreatedOn ).ToList();

            if (!notesList.All( x => x.UserID == userId ))
            {
                throw new ArgumentException( "All FUS notes should have the user ID provided by the argument userId" );
            }

            var currentSet = new FusNoteSet( notesList[0] );
            fusNoteSets.Add( currentSet );

            for (int i = 1; i < notesList.Count; i++)
            {
                var currentNote = notesList[i];

                if (currentNote.IsExtensionNote)
                {
                    currentSet.AddExtensionNote( currentNote );
                }

                else if (!currentNote.IsExtensionNote)
                {
                    currentSet = new FusNoteSet( currentNote );
                    fusNoteSets.Add( currentSet );
                }
            }

            var mergedFusNotes = new List<ExtendedFUSNote>();

            fusNoteSets.ForEach( x => mergedFusNotes.Add( x.GetMergedFusNote() ) );

            return mergedFusNotes;
        }


        /// <summary>
        /// Loads activity codes using specific stored procedure.
        /// </summary>
        /// <returns></returns>
        private ICollection LoadActivityCodesUsing(string procedureName)
        {

            Hashtable allActivityCodes = new Hashtable();
            SqlCommand sqlCommand = null;
            SafeReader reader = null;

            try
            {
                sqlCommand = this.CommandFor(procedureName);

                reader = this.ExecuteReader(sqlCommand);

                while (reader.Read())
                {
                    FusActivity newActivity = this.FusActivityFrom(reader);
                    allActivityCodes.Add(newActivity.Code, newActivity);
                }

            }
            catch (Exception anyException)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom(anyException, _logger);
            }
            finally
            {
                base.Close(reader);
                base.Close(sqlCommand);
            }

            return allActivityCodes;
        }


        /// <summary>
        /// Loads all activity codes.
        /// </summary>
        /// <returns></returns>
        private ICollection LoadAllActivityCodes()
        {
            return this.LoadActivityCodesUsing(DBPROCEDURE_SELECT_ALL_ACTIVITY_CODES);
        }


        /// <summary>
        /// Loads all writeable activity codes.
        /// </summary>
        /// <returns></returns>
        private ICollection LoadAllWriteableActivityCodes()
        {
            return this.LoadActivityCodesUsing(DBPROCEDURE_SELECT_ALL_WRITABLE_ACTIVITY_CODES);
        }


        private void PostExtendedFUSNote( Account anAccount, string pbarEmployeeID, FusActivity activity,
                               string remarks, decimal amount1, decimal amount2, DateTime noteDateTime )
        {
            HDIService hdiService = new HDIService();

            ExtendedFUSNote fusNote = new ExtendedFUSNote();

            fusNote.Account = anAccount;
            fusNote.AccountNumber = anAccount.AccountNumber.ToString();
            fusNote.UserID = pbarEmployeeID;
            fusNote.FusActivity = activity;
            fusNote.Remarks = remarks;
            fusNote.Dollar1 = amount1;
            fusNote.Dollar2 = amount2;

            fusNote.CreatedOn = noteDateTime;

            hdiService.PostExtendedRemarksFusNote( fusNote );
        }


        private void PostExtendedFUSNote( ExtendedFUSNote aNote )
        {
            HDIService hdiService = new HDIService();

            // apply the UTC offset so the notes match pbar-generated timestamps

            hdiService.PostExtendedRemarksFusNote( aNote );
        }

        /// <summary>
        /// Merges the extension notes with parent notes.
        /// </summary>
        /// <param name="notes">The notes.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"><c>notes</c> is null.</exception>
        /// <exception cref="ArgumentException">When all FUS notes in <c>notes</c> are extension notes</exception>
        /// <exception cref="ArgumentException">When the earliest FUS note in <c>notes</c> (by creation time) is an extension note</exception>
        internal static IList<ExtendedFUSNote> MergeExtensionNotesWithParentNotes( IList<ExtendedFUSNote> notes )
        {
            if (notes == null)
            {
                throw new ArgumentNullException( "notes" );
            }

            if (notes.Count == 0)
            {
                return notes;
            }

            if (notes.All( x => x.IsExtensionNote ))
            {
                throw new ArgumentException( "All notes are extension notes" );
            }

            notes = notes.OrderBy( x => x.CreatedOn ).ToList();

            if (notes[0].IsExtensionNote)
            {
                throw new ArgumentException( "When FUS notes are sorted in descending order by creation time then the first FUS note can not be an extension note" );
            }

            var mergedNotes = notes
                .GroupBy( x => x.UserID )
                .SelectMany( g => GetMergedNotesFor( g, g.Key ) )
                .ToList();

            return mergedNotes;
        }

		#endregion Methods 

		#region Nested Classes 


        [Serializable]
        // ReSharper disable MemberCanBePrivate.Local
        private class FusNoteSet
        {
		#region Fields 

            private readonly List<ExtendedFUSNote> _extensionNotes;

		#endregion Fields 

		#region Constructors 

            public FusNoteSet( ExtendedFUSNote parentNote )
            {
                this.ParentNote = parentNote;
                this._extensionNotes = new List<ExtendedFUSNote>();
            }

		#endregion Constructors 

		#region Properties 


            public ReadOnlyCollection<ExtendedFUSNote> ExtensionNotes

            {
                get
                {
                    return new ReadOnlyCollection<ExtendedFUSNote>( this._extensionNotes );
                }
            }


            public ExtendedFUSNote ParentNote { get; private set; }

		#endregion Properties 

		#region Methods 

            /// <summary>
            /// Adds the extension note.
            /// </summary>
            /// <param name="extensionNote">The extension note.</param>
            /// <exception cref="ArgumentException">FUS not is not an extension note or FUS note user id does not match the user id of the parent FUS note</exception>
            /// <exception cref="ArgumentNullException"><c>extensionNote</c> is null.</exception>
            public void AddExtensionNote( ExtendedFUSNote extensionNote )
            {
                if (extensionNote == null)
                {
                    throw new ArgumentNullException( "extensionNote" );
                }

                if (!extensionNote.IsExtensionNote)
                {
                    throw new ArgumentException( "FUS not is not an extension note", "extensionNote" );
                }

                if (extensionNote.UserID != ParentNote.UserID)
                {
                    throw new ArgumentException( "FUS not user id does not match user id of the parent FUS note", "extensionNote" );
                }

                this._extensionNotes.Add( extensionNote );
            }


            public ExtendedFUSNote GetMergedFusNote()
            {
                var sortedExtensionNotes = this.ExtensionNotes.OrderBy( x => x.CreatedOn );

                var combinedNote = new StringBuilder();

                combinedNote.Append( ParentNote.Remarks );

                foreach (var note in sortedExtensionNotes)
                {
                    combinedNote.Append( note.Remarks.Replace( "\r", string.Empty ).TrimEnd() );
                }

                ParentNote.Remarks = combinedNote.ToString();

                return ParentNote;
            }

		#endregion Methods 
        }
        // ReSharper restore MemberCanBePrivate.Local
		#endregion Nested Classes 
    }
}
