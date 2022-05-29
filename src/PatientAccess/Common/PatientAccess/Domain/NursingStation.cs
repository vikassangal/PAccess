using System;

namespace PatientAccess.Domain 
{
    [Serializable]
    public class NursingStation : CodedReferenceValue 
    {
        #region Event Handlers
        #endregion

        #region Methods
        #endregion

        #region Properties
/// <summary>
/// This property is readonly since it returns a calculated value.
/// </summary>
        public int CurrentCensus
        {
            get
            {
                i_CurrentCensus =  i_PreviousCensus +
                    i_AdmitToday -
                    i_DischargeToday -
                    i_DeathsToday -
                    i_TransferredFromToday +
                    i_TransferredToToday;
                return i_CurrentCensus;
            }
        }

        public int PreviousCensus
        {
            get
            {
                return i_PreviousCensus;
            }
            set
            {
                i_PreviousCensus = value;
            }
        }

        public int AdmitToday
        {
            get
            {
                return i_AdmitToday;
            }
            set
            {
                i_AdmitToday = value;
            }
        }

        public int DischargeToday
        {
            get
            {
                return i_DischargeToday;
            }
            set
            {
                i_DischargeToday = value;
            }
        }

        public int DeathsToday
        {
            get
            {
                return i_DeathsToday;
            }
            set
            {
                i_DeathsToday = value;
            }
        }

        public int TransferredFromToday
        {
            get
            {
                return i_TransferredFromToday;
            }
            set
            {
                i_TransferredFromToday = value;
            }
        }

        public int TransferredToToday
        {
            get
            {
                return i_TransferredToToday;
            }
            set
            {
                i_TransferredToToday = value;
            }
        }

        public int AvailableBeds
        {
            get
            {
                return i_AvailableBeds;
            }
            set
            {
                i_AvailableBeds = value;
            }
        }
        
        public int TotalBeds
        {
            get
            {
                return i_TotalBeds;
            }
            set
            {
                i_TotalBeds = value;
            }
        }

        public int TotalOccupiedBedsForMonth
        {
            get
            {
                return i_TotalOccupiedBedsForMonth;
            }
            set
            {
                i_TotalOccupiedBedsForMonth = value;
            }
        }
        
        public int TotalBedsForMonth
        {
            get
            {
                return i_TotalBedsForMonth;
            }
            set
            {
                i_TotalBedsForMonth = value;
            }
        }


        public string SiteCode
        {
            get { return i_SiteCode; }
            set { i_SiteCode = value; }
        }
	

        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public NursingStation()
        {
        }
        public NursingStation( long oid, DateTime version, string description )
            : base( oid, version, description )
        {
        }

        public NursingStation( long oid, DateTime version, string description, string code )
            : base( oid, version, description, code )
        {
        }

        public NursingStation( long oid, DateTime version, string description, string room, string bed )
            : base( oid, version, description )
        {
            i_Room = room;
            i_Bed  = bed; 
        }

        public NursingStation( long oid,
            DateTime version,
            string description,
            string code,
            int previousCensus,
            int admitToday,
            int dischargeToday,
            int deathsToday,
            int transferredFromToday,
            int transferredToToday,
            int availableBeds,
            int totalBeds,
            int totalOccupiedBedsForMonth,
            int totalBedsForMonth,
            string siteCode) : base( oid, version, description, code )
        {
            i_PreviousCensus = previousCensus;
            i_AdmitToday = admitToday;
            i_DischargeToday = dischargeToday;
            i_DeathsToday = deathsToday;
            i_TransferredFromToday = transferredFromToday;
            i_TransferredToToday = transferredToToday;
            i_AvailableBeds = availableBeds;
            i_TotalBeds = totalBeds;
            i_TotalOccupiedBedsForMonth = totalOccupiedBedsForMonth;
            i_TotalBedsForMonth = totalBedsForMonth;
            i_SiteCode = siteCode;
        }
        #endregion

        #region Data Elements        
        private string i_Room;
        private string i_Bed;
        private int i_PreviousCensus;
        private int i_CurrentCensus;
        private int i_AdmitToday;
        private int i_DischargeToday;
        private int i_DeathsToday;
        private int i_TransferredFromToday;
        private int i_TransferredToToday;
        private int i_AvailableBeds;
        private int i_TotalBeds;
        private int i_TotalOccupiedBedsForMonth;
        private int i_TotalBedsForMonth;
        private string i_SiteCode = string.Empty;
        #endregion
    }
}