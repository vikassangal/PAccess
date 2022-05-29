using System;
 
using PatientAccess.BrokerInterfaces;
using PatientAccess.Persistence;
 
using System.Data;
using System.Web.Services;

namespace PatientAccess
{
    public partial class VIWebCall : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string id = Request.QueryString["param"];
                string action = Request.QueryString["param1"];
                string pBAREmployeeID = Request.QueryString["param2"];
                pBAREmployeeID = System.Web.HttpUtility.UrlDecode(VIWebServiceBroker.Decrypt(pBAREmployeeID));
                if (action !=null && action.ToLower() == "scan")
                    {
                        this.Title = "Scan Document";
                        ScanDocument(id, pBAREmployeeID);
                    }
                    else if (action != null && action.ToLower() == "view")
                    {
                        this.Title = "View Document";
                        ViewDocument(id, pBAREmployeeID);
                    }
                    else
                    {
                        //Throw error page
                    }
                
            }
            catch
            {
                //Throw error page
            }
        }

        private void ScanDocument(string id,string pbarEmployeeId)
        {
            VIWebServiceBroker viwebbroker = new VIWebServiceBroker();
            DataSet ds = new DataSet();
            ds = viwebbroker.GetScanParam(Convert.ToInt32(id));
            if (ds.Tables.Count != 0)
            {
                hspCd = ds.Tables[0].Rows[0][1].ToString().Trim();
                patNumber = ds.Tables[0].Rows[0][2].ToString().Trim();
                patName = ds.Tables[0].Rows[0][3].ToString().Trim();
                Payor = ds.Tables[0].Rows[0][4].ToString().Trim();
                currentDate = ds.Tables[0].Rows[0][5].ToString().Trim();
                mrNumber = ds.Tables[0].Rows[0][6].ToString().Trim();
            }
            else
            {
                // if parameter not found then?
            }

            ifram_viweb_url.Attributes.Add("src", ScanImageHTML5URL);

            string viwebSessionId = GetViwebSessionId(pbarEmployeeId);
            string script = "window.onload = function() { doScan('" + hspCd.Trim() + "','" + patNumber.Trim() + "','" + patName.Trim() + "','" + Payor.Trim() + "','" + currentDate.Trim() + "','" + mrNumber.Trim() + "','" + viwebSessionId + "','" + pbarEmployeeId + "'); };";
            ClientScript.RegisterStartupScript(this.GetType(), "doScan", script, true);
        }

        private void ViewDocument(string id, string pbarEmployeeId)
        {
            VIWebServiceBroker viwebbroker = new VIWebServiceBroker();
            DataSet ds = new DataSet();
            ds = viwebbroker.GetViewParam(Convert.ToInt32(id));
            if (ds.Tables.Count != 0)
            {
                hspCd = ds.Tables[0].Rows[0][1].ToString().Trim();
                docID = ds.Tables[0].Rows[0][2].ToString().Trim();
            }
            else
            {
                // if parameter not found then?
            }
            ifram_viweb_url.Attributes.Add("src", ViewImageHTML5URL);

            string viwebSessionId = GetViwebSessionId(pbarEmployeeId);
            string script = "window.onload = function() { doView('" + hspCd.Trim() + "','" + docID.Trim() + "','" + viwebSessionId + "','" + pbarEmployeeId + "'); };";
            ClientScript.RegisterStartupScript(this.GetType(), "doView", script, true);
        }

        private string GetViwebSessionId(string pbarEmployeeId)
        {
            try
            {
                IVIWebServiceBroker viwebbroker = BrokerFactory.BrokerOfType<IVIWebServiceBroker>();
                return viwebbroker.GetViwebSessionID(pbarEmployeeId);
            }
            catch
            {
                return string.Empty;
            }
        }

        [WebMethod(enableSession:true)]
        public static string SaveViwebSessionId(string sessionId,string pbarEmployeeId)
        {
            try
            {
                IVIWebServiceBroker viwebbroker = BrokerFactory.BrokerOfType<IVIWebServiceBroker>();
                viwebbroker.SaveViwebSessionID(sessionId, pbarEmployeeId);
                return viwebbroker.GetViwebSessionID(pbarEmployeeId);
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }


        #region Constant
        private string _scanImageUrl = string.Empty;
        private string _viewImageUrl = string.Empty;
        string hspCd = "";
        string patNumber = "";
        string patName = "";
        string Payor = "";
        string currentDate = "";
        string mrNumber = "";
        string docID = "";

        #endregion
        #region Private Property
        /// <summary>
        /// Gets the scan image URL.
        /// </summary>
        /// <value>The scan image URL.</value>
        private string ScanImageHTML5URL
        {
            get
            {
                if (this._scanImageUrl == string.Empty)
                {
                    IVIWebServiceBroker broker = BrokerFactory.BrokerOfType<IVIWebServiceBroker>();
                    this._scanImageUrl = broker.GetScanHTML5URL();
                }

                return this._scanImageUrl;
            }
        }

        /// <summary>
        /// Gets the view image URL.
        /// </summary>
        /// <value>The view image URL.</value>
        private string ViewImageHTML5URL
        {
            get
            {
                if (this._viewImageUrl == string.Empty)
                {
                    IVIWebServiceBroker broker = BrokerFactory.BrokerOfType<IVIWebServiceBroker>();
                    this._viewImageUrl = broker.GetViewHTML5URL();
                }

                return this._viewImageUrl;
            }
        }

        #endregion
    }
}