using Peradigm.Framework.Domain.Collections;
using Peradigm.Framework.Domain.Exceptions;
using Peradigm.Framework.Domain.Parties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;
using System.Configuration;
using Extensions.SecurityService.Domain;
using Extensions.OrganizationalService;
using System.Web.Script.Serialization;
using System.Text;

namespace PatientAccess.AppServer.Services
{
    public class OKTASecurityService
    {
        #region Constants
        private const string

           ERR_AUTHENTICATE_USER = "OKTA User ID and/or password were not recognized. Please try again.\nOR\nIf you have repeatedly attempted to log in using this User ID and password without success,\nplease call the Tenet Help Desk for assistance.",
           ERR_AUTHENTICATE_USER_FOR_LEGACY = "Security Service Error - Application failed during user OKTA authentication for PBAR Legacy.",
           ERR_READ_APPLICATION_ROLES = "Security Service Error - Error while retrieving roles for application.",
           ERR_READ_USER_PRIVILEGES = "Security Service Error - Error retrieving user privileges for application.";
        #endregion

        public Hashtable LegacyUserWith(string tenetOneID)
        {
            Hashtable legacyUser = new Hashtable();
            string hospCode, legacyUserID;
            bool isActive = false;
            try
            {
                var legacyResponse = ParseLegacyJsonData(tenetOneID).result;
                if (legacyResponse.Count() > 0)
                {
                    foreach (result ls in legacyResponse)
                    {
                        hospCode = ls.u_hsp_code;
                        legacyUserID = ls.LegacyId;
                        isActive = ls.u_active;
                        if (!(legacyUser.ContainsKey(hospCode))
                            && isActive 
                            && ls.u_application_name.Contains("PBAR"))
                        {
                            legacyUser.Add(hospCode, legacyUserID);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new SecurityException(String.Format(ERR_AUTHENTICATE_USER_FOR_LEGACY, e.Message), e, Severity.Catastrophic);
            }
            return legacyUser;
        }

        public Extensions.SecurityService.Domain.User UserWith(string upn, string password)
        {
            User domainUser = null;
            var oktaAuthenticationObject = new OKTAAuthenticationObject();
            string attributeLastName, attributeFirstName, oktaId = "", attributeTenetId, attributeRoleName;

            try
            {
                oktaAuthenticationObject = GetOKTAAuthenticationResponse(upn, password);
            }
            catch (Exception ex)
            {
                throw new SecurityException(ERR_AUTHENTICATE_USER, ex, Severity.Medium);
            }

            try
            {
                if (oktaAuthenticationObject.status != null)
                {
                    if (oktaAuthenticationObject.status.ToUpper() == "SUCCESS")
                    {
                        attributeLastName = oktaAuthenticationObject._embedded.user.profile.lastName;
                        attributeFirstName = oktaAuthenticationObject._embedded.user.profile.firstName;
                        oktaId = oktaAuthenticationObject._embedded.user.id;
                        // User Information 
                        OKTAUserObject oKtaUserObject = GetOKTAUserPrevilages(oktaId);
                        attributeTenetId = oKtaUserObject.profile.employeeID;

                        if (string.IsNullOrEmpty(attributeTenetId))
                        {
                            return domainUser;
                        }
                        domainUser = new User(Convert.ToInt64(attributeTenetId), attributeFirstName, attributeLastName);
                        domainUser.UPN = upn;
                        domainUser.EmployeeID = attributeTenetId;
                    }
                    else
                    {
                        throw new SecurityException(ERR_AUTHENTICATE_USER, Severity.Medium);
                    }
                }
                else
                {
                    throw new SecurityException(ERR_AUTHENTICATE_USER,  Severity.Medium);
                }
            }

            catch (Exception ex)
            {
                throw new SecurityException(ERR_READ_USER_PRIVILEGES, ex, Severity.Medium);
            }

            try
            {
                OrganizationalUnit hierarchy = OrganizationalService.Service.GetOrganizationalHierarchy();
                Role aRole;

                // Role Information 
                List<OKTARolesObject> oKtaRolesObject = GetOKTAUserRoles(oktaId);
                // Filter only "type": "OKTA_GROUP" from OKTA Roles response
                var filterOktaRoleResponse = oKtaRolesObject.Where(x => (x.type == "OKTA_GROUP"));

                if (filterOktaRoleResponse.Count() > 0)
                {
                    foreach (OKTARolesObject rolesObject in filterOktaRoleResponse)
                    {
                        if (rolesObject.profile.name.Contains("-"))
                        {
                            attributeRoleName = rolesObject.profile.name;
                            string roleName = attributeRoleName.Substring(attributeRoleName.LastIndexOf("-") + 1);
                            string hspcode = attributeRoleName.Substring(0, attributeRoleName.IndexOf("-"));
                            aRole = this.Application.RoleWith(roleName);
                            ArrayList facilityCodes = new ArrayList();
                            if (aRole != null)
                            {
                                facilityCodes.Add(hspcode);
                            }

                            this.BuildOrganizationalRelationshipsFor(domainUser, aRole, hierarchy, facilityCodes);
                        }
                    }
                }
                else
                {
                    throw new SecurityException(ERR_READ_APPLICATION_ROLES, Severity.Medium);
                }
            }
            catch (Exception ex)
            {
                throw new SecurityException(ERR_READ_APPLICATION_ROLES, ex,Severity.Medium);
            }

            return domainUser;
        }


        public OKTASecurityService()
        {
            OKTAAUTHURL = ConfigurationManager.AppSettings["OKTAAUTHURL"].Trim();
            OKTAAPIKEY = ConfigurationManager.AppSettings["OKTAAPIKey"].Trim();
            OKTAUSERINFOURL = ConfigurationManager.AppSettings["OKTAUSERINFOURL"].Trim();
            OKTAROLEINFOURL = ConfigurationManager.AppSettings["OKTAROLEURL"].Trim();
            LEGACYURL = ConfigurationManager.AppSettings["LEGACY_URL"].Trim();
            LEGACYUSER = ConfigurationManager.AppSettings["LEGACY_USER"].Trim();
            LEGACYPASSWORD = ConfigurationManager.AppSettings["LEGACY_PASSWORD"].Trim();
        }


        #region Private Methods
        private Hashtable AllFacilitiesFrom(OrganizationalUnit hierarchy)
        {
            Hashtable facilities = new Hashtable();
            foreach (Facility facility in hierarchy.AllFacilities())
            {
                facilities.Add(facility.Code, facility);
            }
            return facilities;
        }

        private void BuildOrganizationalRelationshipsFor(Extensions.SecurityService.Domain.User user,
                                                          Role role,
                                                          OrganizationalUnit hierarchy,
                                                          ArrayList facilityCodes)
        {
            ArrayList userFacilities = new ArrayList();

            if (this.IsGlobalRole)
            {
                userFacilities = hierarchy.AllFacilities();                    //get all facilities from hierarchy
            }
            else if (facilityCodes.Count > 0)
            {
                Hashtable allFacilities = this.AllFacilitiesFrom(hierarchy);
                foreach (string facilityCode in facilityCodes)
                {
                    Facility facility = (Facility)allFacilities[facilityCode]; //only facilities in hierarchy are accepted
                    if (facility != null)
                    {
                        userFacilities.Add(facility);
                    }
                }
            }
            foreach (Facility facility in userFacilities)
            {
                user.AddRelationship(new RoleRelationship(user, role, facility));

                ArrayList userLevels = this.OrganizationalHierarchyFor(facility, userFacilities);
                foreach (OrganizationalUnit organizationalUnit in userLevels)
                {
                    if (!user.HasRelationshipWith(organizationalUnit, role))
                    {
                        user.AddRelationship(new RoleRelationship(user, role, organizationalUnit));
                    }
                }
            }
        }

        private ArrayList OrganizationalHierarchyFor(Facility facility, ICollection allFacilities)
        {
            Set hierarchy = new Set();
            OrganizationalUnit parent = facility.Parent() as OrganizationalUnit;
            while (parent != null)
            {
                if (parent.IsComplete(allFacilities))
                {
                    hierarchy.Add(parent);
                    parent = parent.Parent() as OrganizationalUnit;
                }
                else
                {
                    parent = null;
                }
            }

            return new ArrayList(hierarchy);
        }

        private string PostOKTAAuthenticationRequest(string OKTAURL, UserCred userCred)
        {
            HttpClient httpClient;
            if (ConfigurationManager.AppSettings["PASServerEnvironment"].ToString() == "LOCAL")
            {
                string CONFIG_PROXY_URL = ConfigurationManager.AppSettings["CONFIG_PROXY_URL"].ToString();
                string PROXY_USER = "tenethealth\\" + ConfigurationManager.AppSettings["PROXY_USER"].ToString();
                string PROXY_PASSWORD = ConfigurationManager.AppSettings["PROXY_PASSWORD"].ToString();
                var proxy = new WebProxy(CONFIG_PROXY_URL, true);

                var networkCredential = new NetworkCredential(
                       PROXY_USER,
                       PROXY_PASSWORD
                       );

                var handler = new HttpClientHandler { Credentials = networkCredential, Proxy = proxy };
                httpClient = new HttpClient(handler);
            }
            else
            {
                httpClient = new HttpClient();
            }

            Uri newuri = new Uri(OKTAURL);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.BaseAddress = newuri;
            JavaScriptSerializer js = new JavaScriptSerializer();
            string jsondata = js.Serialize(userCred);
            HttpContent content = new StringContent(jsondata, Encoding.UTF8, "application/json");
            var httpTAsync = httpClient.PostAsync("", content);
            var httpTResult = httpTAsync.Result;
            string httpResponse = httpTResult.Content.ReadAsStringAsync().Result;

            return httpResponse;

        }

        private string GetOKTAUserDetails(string OKTAURL, string OKTAAPIKEY)
        {
            HttpClient httpClient;
            if (ConfigurationManager.AppSettings["PASServerEnvironment"].ToString() == "LOCAL")
            {
                string CONFIG_PROXY_URL = ConfigurationManager.AppSettings["CONFIG_PROXY_URL"].ToString();
                string PROXY_USER = "tenethealth\\" + ConfigurationManager.AppSettings["PROXY_USER"].ToString();
                string PROXY_PASSWORD = ConfigurationManager.AppSettings["PROXY_PASSWORD"].ToString();
                var proxy = new WebProxy(CONFIG_PROXY_URL, true);

                var networkCredential = new NetworkCredential(
                       PROXY_USER,
                       PROXY_PASSWORD
                       );

                var handler = new HttpClientHandler { Credentials = networkCredential, Proxy = proxy };
                httpClient = new HttpClient(handler);
            }
            else
            {
                httpClient = new HttpClient();
            }
            Uri newuri = new Uri(OKTAURL);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (OKTAAPIKEY != string.Empty)
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", "SSWS " + OKTAAPIKEY);
            }

            httpClient.BaseAddress = newuri;
            var httpTAsync = httpClient.GetAsync(newuri);
            var httpTResult = httpTAsync.Result;
            string httpResponse = httpTResult.Content.ReadAsStringAsync().Result;

            return httpResponse;

        }
        
        private string GetOKTAUserRoleDetails(string OKTAURL, string OKTAAPIKEY, List<OKTARolesObject> OKTARoleResponse)
        {
            HttpClient httpClient;
            if (ConfigurationManager.AppSettings["PASServerEnvironment"].ToString() == "LOCAL")
            {
                string CONFIG_PROXY_URL = ConfigurationManager.AppSettings["CONFIG_PROXY_URL"].ToString();
                string PROXY_USER = "tenethealth\\" + ConfigurationManager.AppSettings["PROXY_USER"].ToString();
                string PROXY_PASSWORD = ConfigurationManager.AppSettings["PROXY_PASSWORD"].ToString();
                var proxy = new WebProxy(CONFIG_PROXY_URL, true);

                var networkCredential = new NetworkCredential(
                    PROXY_USER,
                    PROXY_PASSWORD
                );

                var handler = new HttpClientHandler { Credentials = networkCredential, Proxy = proxy };
                httpClient = new HttpClient(handler);
            }
            else
            {
                httpClient = new HttpClient();
            }
            Uri newuri = new Uri(OKTAURL);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (OKTAAPIKEY != string.Empty)
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", "SSWS " + OKTAAPIKEY);
            }

            httpClient.BaseAddress = newuri;
            var httpTAsync = httpClient.GetAsync(newuri);
            var httpTResult = httpTAsync.Result;
            string httpResponse = httpTResult.Content.ReadAsStringAsync().Result;
            var javaScriptSerializer = new JavaScriptSerializer();
            var stuff = javaScriptSerializer.Deserialize<List<OKTARolesObject>>(httpResponse);

            foreach (var rolesObject in stuff)
            {
                OKTARoleResponse.Add(rolesObject);
            }

            IEnumerable<string> linkList;
            if (httpTAsync.Result.Headers.TryGetValues("link", out linkList))
            {
                if (linkList.Count() > 1)
                {
                    var nextLink = linkList.ElementAt(1).Replace(">; rel=\"next\"", "")
                        .Replace("<", "");
                    HeaderLink(nextLink, OKTAAPIKEY, OKTARoleResponse);
                }
            }
            return httpResponse;

        }
        private string GetLegacyResponse(string TenetOneURL)
        {
            HttpClient httpClient;
            if (ConfigurationManager.AppSettings["PASServerEnvironment"].ToString() == "LOCAL")
            {
                string CONFIG_PROXY_URL = ConfigurationManager.AppSettings["CONFIG_PROXY_URL"].ToString();
                string PROXY_USER = "tenethealth\\" + ConfigurationManager.AppSettings["PROXY_USER"].ToString();
                string PROXY_PASSWORD = ConfigurationManager.AppSettings["PROXY_PASSWORD"].ToString();
                var proxy = new WebProxy(CONFIG_PROXY_URL, true);

                var networkCredential = new NetworkCredential(
                       PROXY_USER,
                       PROXY_PASSWORD
                       );

                var handler = new HttpClientHandler { Credentials = networkCredential, Proxy = proxy };
                httpClient = new HttpClient(handler);
            }
            else
            {
                httpClient = new HttpClient();
            }
            Uri newuri = new Uri(TenetOneURL);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(LEGACYUSER + ":" + LEGACYPASSWORD);
            string val = System.Convert.ToBase64String(plainTextBytes);
            httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + val);

            httpClient.BaseAddress = newuri;
            var httpTAsync = httpClient.GetAsync(newuri);
            var httpTResult = httpTAsync.Result;
            string httpResponse = httpTResult.Content.ReadAsStringAsync().Result;

            return httpResponse;

        }
        private OKTAAuthenticationObject GetOKTAAuthenticationResponse(string upn, string password)
        {
            try
            {
                var userCred = new UserCred() { username = upn, password = password };
                url = OKTAAUTHURL;
                var JsonString = PostOKTAAuthenticationRequest(url, userCred);
                var javaScriptSerializer = new JavaScriptSerializer();
                var OKTAAuthenticationResponse = javaScriptSerializer.Deserialize<OKTAAuthenticationObject>(JsonString);
                return OKTAAuthenticationResponse;
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        private OKTAUserObject GetOKTAUserPrevilages(string OKTAID)
        {
            url = OKTAUSERINFOURL + "/" + OKTAID;
            string JsonString = GetOKTAUserDetails(url, OKTAAPIKEY);
            var javaScriptSerializer = new JavaScriptSerializer();
            var OKTAUserResponse = javaScriptSerializer.Deserialize<OKTAUserObject>(JsonString);
            return OKTAUserResponse;
        }
        private List<OKTARolesObject> GetOKTAUserRoles(string OKTAID)
        {
            if (ConfigurationManager.AppSettings["PASServerEnvironment"].ToString() == "LOCAL"
                || ConfigurationManager.AppSettings["PASServerEnvironment"].ToString() == "Model")
            {
                string OKTA_Role_Limit = ConfigurationManager.AppSettings["OKTA_ROLE_LIMIT"].ToString();
                url = OKTAROLEINFOURL + "/" + OKTAID + "/groups?limit="+ OKTA_Role_Limit;
            }
            else
            {
                url = OKTAROLEINFOURL + "/" + OKTAID + "/groups";
            }

            List<OKTARolesObject> OKTARoleResponse = new List<OKTARolesObject>();
            GetOKTAUserRoleDetails(url, OKTAAPIKEY, OKTARoleResponse);
            return OKTARoleResponse;
            
        }
        private void HeaderLink(string newURI, string OKTAAPIKEY, List<OKTARolesObject> OKTARoleResponse)
        {
            GetOKTAUserRoleDetails(newURI, OKTAAPIKEY, OKTARoleResponse);
        }

        
        private LegacyResult ParseLegacyJsonData(string TenetID)
        {
            var javaScriptSerializer = new JavaScriptSerializer();

            url = LEGACYURL + TenetID;
            string JsonString = GetLegacyResponse(url);
            string newJsonString = JsonString.Replace("u_user_credentials.u_user_id", "LegacyId").Replace("u_application.name", "u_application_name");
            var TenetOneResponse =
                javaScriptSerializer.Deserialize<LegacyResult>(newJsonString);
            return TenetOneResponse;
        }
        #endregion
        
        private Application Application
        {
            get
            {
                return ApplicationService;
            }
        }

        public static Application ApplicationService
        {
            private get
            {
                return c_ApplicationService;
            }
            set
            {
                c_ApplicationService = value;
            }
        }
        private bool IsGlobalRole
        {
            get
            {
                return i_IsGlobalRole;
            }
        }

        #region Data Elements
        
        private bool i_IsGlobalRole = false;
        private static Application c_ApplicationService;
        string OKTAAUTHURL, OKTAAPIKEY, OKTAUSERINFOURL, OKTAROLEINFOURL, LEGACYURL, LEGACYUSER, LEGACYPASSWORD, url = string.Empty;
        #endregion
    }
    public class UserCred
    {
        public string username { get; set; }
        public string password { get; set; }
    }

    public class result
    {
        public string u_hsp_code { get; set; }
        public string LegacyId { get; set; }
        public string u_application_name { get; set; }
        public bool u_active { get; set; }

    }
    public class LegacyResult
    {
        public List<result> result { get; set; }

    }

}