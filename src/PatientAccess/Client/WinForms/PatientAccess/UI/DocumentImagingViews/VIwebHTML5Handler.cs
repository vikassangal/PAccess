using System;
using System.Collections; 
using System.Text;
using PatientAccess.Domain;
using PatientAccess.BrokerInterfaces;
using System.Diagnostics;
using Microsoft.Win32; 

namespace PatientAccess.UI.DocumentImagingViews
{
    public class VIwebHTML5Handler
    {
        public bool IsDynamsoftInstalled()
        {
            try
            {
                var process = Process.GetProcessesByName("DynamsoftService");
                if (process.Length > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public bool IsChromeInstalled()
        {
            try
            {
                object path;
                path = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\App Paths\chrome.exe", "", null);
                if (path != null)
                {
                    return true;
                }
                path = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\chrome.exe", "", null);
                if (path != null)
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
        public bool IsEdgeInstalled()
        {
            try
            {
                object path;
                path = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\App Paths\msedge.exe", "", null);
                if (path != null)
                {
                    return true;
                }
                path = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\msedge.exe", "", null);
                if (path != null)
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
        public bool IsFirefoxInstalled()
        {
            try
            {
                object path;
                path = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\App Paths\firefox.exe", "", null);
                if (path != null)
                {
                    return true;
                }
                path = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\firefox.exe", "", null);
                if (path != null)
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public void DoScanDocument()
        {
            try
            {
                IVIWebServiceBroker broker = BrokerFactory.BrokerOfType<IVIWebServiceBroker>();

                Process process = new Process();
                StringBuilder formattedName = new StringBuilder();
                formattedName.AppendFormat("{0} {1}",
                    this.Model.Patient.LastName.Trim(),
                    this.Model.Patient.FirstName.Trim());
                id = broker.SaveScanParameters(this.Model.Facility.Code, this.Model.AccountNumber.ToString(),
                    formattedName.ToString(), this.Model.PrimaryPayor, DateTime.Now.ToString("yyyyMMdd"),
                    this.Model.Patient.MedicalRecordNumber.ToString());
                string pbarEmployeeID = User.GetCurrent().PBAREmployeeID;
                pbarEmployeeID = System.Web.HttpUtility.UrlEncode(broker.Encrypt(pbarEmployeeID));
                url = VIwebLocalUrl + "?param=" + id + "&param1=scan"+"&param2="+ pbarEmployeeID;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                if (IsChromeInstalled())
                {
                    appDataPath = appDataPath.Replace("\\", "/")+"/chrometemp"; 
                    process.StartInfo.FileName = "chrome.exe";
                    process.StartInfo.Arguments = "   --user-data-dir="+appDataPath+" --app=" + url;
                    process.Start();
                    BrowerType = ("Chrome");
                    PID.Add(process.Id);
                }
                else if (IsEdgeInstalled())
                {
                    appDataPath = appDataPath.Replace("\\", "/") + "/edgetemp";
                    process.StartInfo.FileName = "msedge.exe";

                    process.StartInfo.Arguments = "    --user-data-dir="+ appDataPath + " "+ url;
                    process.Start();
                    BrowerType = ("Edge");
                    PID.Add(process.Id);
                }
                else if (IsFirefoxInstalled())
                {
                    process.StartInfo.FileName = "firefox.exe";
                    process.StartInfo.Arguments = "  -new-instance -private-window  " + url;
                    process.Start();
                    BrowerType = ("Firefox");
                    PID.Add(process.Id);
                }
            
                else
                {
                    ListOfDocumentsView lst = new ListOfDocumentsView();
                    lst.LoadLegacyVIweb("SCAN");
                }
            }
            catch
            {

            }
        }
        public void DoViewDocument(ArrayList i_Docs)
        {
            try
            {
                var broker = BrokerFactory.BrokerOfType<IVIWebServiceBroker>();
                var process = new Process();

                var viewingDocIdBuilder = new StringBuilder();
                foreach (string documentId in i_Docs)
                {
                    if (!String.IsNullOrEmpty(documentId))
                    {
                        viewingDocIdBuilder.AppendFormat(",{0}", documentId);
                    }
                }
                id = broker.SaveViewParameters(this.Model.Facility.Code, viewingDocIdBuilder.ToString());
                string pbarEmployeeID = User.GetCurrent().PBAREmployeeID;
                pbarEmployeeID = System.Web.HttpUtility.UrlEncode(broker.Encrypt(pbarEmployeeID));
                url = VIwebLocalUrl + "?param=" + id + "&param1=view" + "&param2=" + pbarEmployeeID; 
                 
                if (IsChromeInstalled())
                {
                    process.StartInfo.FileName = "chrome.exe";
                    process.StartInfo.Arguments = @"    --app=""" + url + "";
                    process.Start();
                    BrowerType = ("Chrome");
                    PID.Add(process.Id);
                }
                else if (IsEdgeInstalled())
                {
                    process.StartInfo.FileName = "msedge.exe";
                    process.StartInfo.Arguments = "    --user-data-dir=c:/temp " + url;
                    process.Start();
                    BrowerType = ("Edge");
                    PID.Add(process.Id);
                }
                else if (IsFirefoxInstalled())
                {
                    process.StartInfo.FileName = "firefox.exe";
                    process.StartInfo.Arguments = "  -new-instance -private-window  " + url;
                    process.Start();
                    BrowerType = ("Firefox");
                    PID.Add(process.Id);
                }
               

                    else
                    {
                        var lst = new ListOfDocumentsView();
                        lst.LoadLegacyVIweb("VIEW");
                    }
            }
            catch
            {
                //TODO" Add error message and show into application
            }
        }

        #region Properties
        public Account Model
        {
            get;
            set;
        }

        private string VIwebLocalUrl
        {
            get
            {
                if (_VIwebLocalUrl == string.Empty)
                {
                    var broker = BrokerFactory.BrokerOfType<IVIWebServiceBroker>();
                    this._VIwebLocalUrl = broker.GetVIwebLocalURL();
                }

                return _VIwebLocalUrl;
            }
        }

        public static ArrayList PID = new ArrayList();
        public static string BrowerType;
        #endregion

        #region Constant
        private string _VIwebLocalUrl = string.Empty;
        private string id = string.Empty;
        private string url = string.Empty;
        #endregion


    }
}
