using System;
using System.Collections;
using PatientAccess.Domain;

namespace PatientAccess.Persistence
{
	/// <summary>
	/// Summary description for ReleaseReservedBedUpdateStrategy.
	/// </summary>
	public class ReleaseReservedBedUpdateStrategy : SqlBuilderStrategy
	{
        #region Construction and Finalization
        public ReleaseReservedBedUpdateStrategy()
        {
            InitializeColumnValues();
        }
        #endregion

        #region Methods
        public override void UpdateColumnValuesUsing( Account account )
        {
            if( account != null )
            {
                //update the local properties with values in domain objects (Account)
                if( account.Facility != null )
                {
                    this.HospitalNumber = ( int )account.Facility.Oid;
                }
                if( account.Location != null )
                {
                    if( account.Location.NursingStation != null )
                    {
                        this.NursingStation = account.Location.NursingStation.Code;
                    }
                    if( account.Location.Room != null )
                    {
                        this.RoomNumber = Convert.ToInt32( account.Location.Room.Code );
                    }
                    if( account.Location.Bed != null )
                    {
                        this.BedNumber = account.Location.Bed.Code;
                    }
                }
            }
        }


        public override void InitializeColumnValues()
        {    
            releaseReservedBedOrderedList.Add( LRHSP_, 0 );
            releaseReservedBedOrderedList.Add( LRNS, string.Empty );
            releaseReservedBedOrderedList.Add( LRROOM, 0 );
            releaseReservedBedOrderedList.Add( LRBED, string.Empty );
        }



        public override ArrayList BuildSqlFrom( Account account, 
            TransactionKeys transactionKeys )
        {
            UpdateColumnValuesUsing( account );
            string sqlStatement = AddColumnsAndValuesToUpdateSqlStatement( this.releaseReservedBedOrderedList, 
                "HPADLRP" );
            this.SqlStatements.Add(sqlStatement);
            return SqlStatements;
        }  

        #endregion

        #region private Properties
        private int HospitalNumber
        {
            set
            {
                this.releaseReservedBedOrderedList[LRHSP_] = value;
            }
        }

 
        private string NursingStation
        {
            set
            {
                this.releaseReservedBedOrderedList[LRNS] = value;
            }
        }

        private int RoomNumber
        {
            set
            {
                this.releaseReservedBedOrderedList[LRROOM] = value;
            }
        }

        private string BedNumber
        {
            set
            {
                this.releaseReservedBedOrderedList[LRBED] = value;
            }
        }
        #endregion

        #region public properties

        #endregion

        #region Data Elements
        private OrderedList releaseReservedBedOrderedList = new OrderedList();
        #endregion

        #region Constants
        private const string
            LRHSP_   = "LRHSP#",
            LRNS     = "LRNS",
            LRROOM   = "LRROOM",
            LRBED    = "LRBED";
            
        #endregion

	}
}
