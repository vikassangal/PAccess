using System;
using System.Collections.Generic;

namespace PatientAccess.AppServer.Services
{
    public class OKTARolesObject
    {
        public string id { get; set; }
        public DateTime created { get; set; }
        public DateTime lastUpdated { get; set; }
        public DateTime lastMembershipUpdated { get; set; }
        public List<string> objectClass { get; set; }
        public string type { get; set; }
        public RoleProfile profile { get; set; }
        public RoleLinks _links { get; set; }
        public Source source { get; set; }
    }

    public class RoleType
    {
        public string id { get; set; }
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class RoleProfile
    {
        public string name { get; set; }
        public string description { get; set; }
        public string windowsDomainQualifiedName { get; set; }
        public string groupType { get; set; }
        public string groupScope { get; set; }
        public string samAccountName { get; set; }
        public string objectSid { get; set; }
        public string externalId { get; set; }
        public string dn { get; set; }
    }

    public class Logo
    {
        public string name { get; set; }
        public string href { get; set; }
        public string type { get; set; }
    }

    public class Users
    {
        public string href { get; set; }
    }

    public class Apps
    {
        public string href { get; set; }
    }

    public class Source
    {
        public string href { get; set; }
        public string id { get; set; }
    }

    public class RoleLinks
    {
        public List<Logo> logo { get; set; }
        public Users users { get; set; }
        public Apps apps { get; set; }
        public Source source { get; set; }
    }

}