using System;
using System.Collections;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Auditing.FusNotes;

namespace PatientAccess.BrokerProxies
{
    [Serializable]
    public class FusNoteBrokerProxy : AbstractBrokerProxy, IFUSNoteBroker
    {
        #region Event Handlers
        #endregion

        #region Methods

        public void WriteFUSNotes( Account anAccount, string pbarEmployeeID )
        {
            this.i_FUSNoteBroker.WriteFUSNotes( anAccount, pbarEmployeeID );
        }

        public Hashtable AllActivityCodesHashtable()
            {
                Hashtable allActivityCodes = new Hashtable();
                allActivityCodes = (Hashtable)this.Cache[FUS_NOTE_BROKER_PROXY_ALL_ACTIVITY_CODES];

                if( allActivityCodes == null )
                {
                    lock( FUS_NOTE_BROKER_PROXY_ALL_ACTIVITY_CODES )
                    {
                    allActivityCodes = this.i_FUSNoteBroker.AllActivityCodesHashtable();
                        if( this.Cache[FUS_NOTE_BROKER_PROXY_ALL_ACTIVITY_CODES] == null )
                        {
                            this.Cache.Insert( FUS_NOTE_BROKER_PROXY_ALL_ACTIVITY_CODES, allActivityCodes );
                        }
                    }
                }

                return allActivityCodes;
            }

        public Hashtable AllWriteableActivityCodesHashtable()
            {
                Hashtable allWriteableActivityCodes = new Hashtable();
                allWriteableActivityCodes = (Hashtable)this.Cache[FUS_NOTE_BROKER_PROXY_ALL_WRITEABLE_ACTIVITY_CODES];

                if( allWriteableActivityCodes == null )
                {
                    lock( FUS_NOTE_BROKER_PROXY_ALL_WRITEABLE_ACTIVITY_CODES )
                    {
                    allWriteableActivityCodes = this.i_FUSNoteBroker.AllWriteableActivityCodesHashtable();
                        if( this.Cache[FUS_NOTE_BROKER_PROXY_ALL_WRITEABLE_ACTIVITY_CODES] == null )
                        {
                            this.Cache.Insert( FUS_NOTE_BROKER_PROXY_ALL_WRITEABLE_ACTIVITY_CODES, allWriteableActivityCodes );
                        }
                    }
                }

                return allWriteableActivityCodes;
            }

        public ICollection AllActivityCodes()
        {
            Hashtable activityCodesHashtable = ( Hashtable )this.AllCodesHashtable;

            ArrayList returnArray = new ArrayList( activityCodesHashtable.Values );
            returnArray.Sort();

            return returnArray;
        }

        public ICollection AllWriteableActivityCodes()
        {
            Hashtable writeableActivityCodesHashtable = ( Hashtable )this.AllWriteableCodesHashtable;

            ArrayList returnArray = new ArrayList( writeableActivityCodesHashtable.Values );
            returnArray.Sort();

            return returnArray;
        }

        public FusActivity FusActivityWith( string activityCode )
        {
            FusActivity fusActivity = ( FusActivity )this.AllCodesHashtable[activityCode];

            if ( fusActivity == null )
            {
                fusActivity = this.i_FUSNoteBroker.FusActivityWith( activityCode );
            }

            return fusActivity;
        }

        public void PostRemarksFusNote(Account anAccount, string userID,
           FusActivity activity, string remarks, DateTime noteDateTime)
        {
            throw new Exception("this method is not implemented in proxy");
        }

        public ICollection GetMergedFUSNotesFor( Account anAccount )
        {
            return this.i_FUSNoteBroker.GetMergedFUSNotesFor( anAccount );
        }

        #endregion

        #region Properties

        private Hashtable AllCodesHashtable
        {
            get
            {
                if( this.i_AllCodesHashtable.Count == 0 )
                {
                    this.i_AllCodesHashtable = ( Hashtable )this.AllActivityCodesHashtable();
                }

                return this.i_AllCodesHashtable;
            }
        }

        public Hashtable AllWriteableCodesHashtable
        {
            get
            {
                if( this.i_AllWriteableCodesHashtable.Count == 0 )
                {
                    this.i_AllWriteableCodesHashtable = ( Hashtable )this.AllWriteableActivityCodesHashtable();
                }

                return this.i_AllWriteableCodesHashtable;
            }
        }

        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public FusNoteBrokerProxy()
        {
        }
        #endregion

        #region Data Elements
        private Hashtable   i_AllCodesHashtable = new Hashtable();
        private Hashtable   i_AllWriteableCodesHashtable = new Hashtable();
        private IFUSNoteBroker i_FUSNoteBroker = BrokerFactory.BrokerOfType< IFUSNoteBroker >() ;
        
        #endregion

        #region Constants
        
        private const string
            FUS_NOTE_BROKER_PROXY_ALL_ACTIVITY_CODES            = "FUS_NOTE_BROKER_PROXY_ALL_ACTIVITY_CODES",
            FUS_NOTE_BROKER_PROXY_ALL_WRITEABLE_ACTIVITY_CODES  = "FUS_NOTE_BROKER_PROXY_ALL_WRITEABLE_ACTIVITY_CODES";
        
#endregion
    }
}
