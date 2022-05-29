using PatientAccess.Domain;
using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Script.Serialization;

namespace PatientAccess.AppServer.Services
{
    public class DOFRInitiateService
    {
        public DOFRInitiateService()
        {
            DOFRApiUrl = ConfigurationManager.AppSettings["DOFRApiUrl"].ToString().Trim();
            DOFRApiKey = ConfigurationManager.AppSettings["DOFRApiKey"].ToString().Trim();
            DOFRUseTLS12 = bool.Parse(ConfigurationManager.AppSettings["DOFRUseTLS12"].ToString().Trim());
            DOFRAPITimeout = double.Parse(ConfigurationManager.AppSettings["DOFRAPITimeout"].ToString().Trim());
        }

        public HttpResponseMessage Predict(DOFRAPIRequest dOFRAPIRequest)
        {
            if (DOFRUseTLS12)
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            }
            HttpClient httpClient = new HttpClient();
            Uri newuri = new Uri(DOFRApiUrl);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (DOFRApiKey != string.Empty)
            {
                httpClient.DefaultRequestHeaders.Add("x-api-key", DOFRApiKey);
            }
            httpClient.BaseAddress = newuri;
            httpClient.Timeout = TimeSpan.FromSeconds(DOFRAPITimeout);
            JavaScriptSerializer js = new JavaScriptSerializer();
            string jsondata = js.Serialize(dOFRAPIRequest);
            HttpContent content = new StringContent(jsondata, Encoding.UTF8, "application/json");
            var httpTAsync = httpClient.PostAsync("", content);
            var httpTResult = httpTAsync.Result;
            return httpTResult;
        }

        string DOFRApiUrl, DOFRApiKey = string.Empty;
        double DOFRAPITimeout;
        bool DOFRUseTLS12 = false;
    }
}