using System;
using System.Collections;
using System.Data;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;

namespace PatientAccess.UI.PatientSearch
{
	public class AccountFilter
	{
		Hashtable iData         = new Hashtable();
		DataTable iDataTable    = new DataTable();        

		public AccountFilter( IList accounts )
		{
			SetColumns( );
			LoadData( accounts );
		}

		public ArrayList ExecuteFilter( string filter )
		{			
            ArrayList results       = new ArrayList();
			DataView dataview = iDataTable.DefaultView;

			dataview.RowFilter = filter;

			foreach( DataRowView row in dataview )
			{
				//This will need to change to the name of the account id
                
                long rowID = (long)row["RowID"];
                AccountProxy accountProxy = (AccountProxy)iData[ rowID ];

				//Only add account proxy to the results arraylist if it's not PRE-Purged
				if( accountProxy.DerivedVisitType.Trim() != Account.PRE_PUR &&
					accountProxy.DerivedVisitType.Trim() != Account.PND_PUR )
				{
					results.Add( accountProxy );
				}
			}

			return results;
		}

		private void LoadData( IList accounts )
		{
            iDataTable.Rows.Clear();

			foreach( AccountProxy proxy in accounts )
			{
				string hospitalService = String.Empty;

				if( proxy.HospitalService != null)
				{
					hospitalService = proxy.HospitalService.Code;
				}
               
                long rowID = iDataTable.Rows.Count;
				iDataTable.Rows.Add(

					new Object[] {rowID, proxy.AccountNumber, proxy.AdmitDate.Date, proxy.DischargeDate,
									 proxy.KindOfVisit.Code, proxy.DerivedVisitType, proxy.FinancialClass.Code,
									 hospitalService, proxy.Patient.Sex.Code, proxy.IsQuickRegistered} );

				iData.Add( rowID, proxy);
			} 
		}

		private void SetColumns()
		{
			//This will need to change to add the proxy account properties

            iDataTable.Columns.Add("RowID", typeof(long));
			iDataTable.Columns.Add("AccountID", typeof(long));
			iDataTable.Columns.Add("AdmitDate", typeof(DateTime));
			iDataTable.Columns.Add("DischargeDate", typeof(DateTime));
			iDataTable.Columns.Add("PatientType", typeof(String));
			iDataTable.Columns.Add("DerivedVisitType", typeof(String));
			iDataTable.Columns.Add("FinancialClass", typeof(String));
			iDataTable.Columns.Add("HSV", typeof(String));
			iDataTable.Columns.Add("Gender", typeof(String));
		    iDataTable.Columns.Add("QacReg", typeof (Boolean));
		}

	}
}
