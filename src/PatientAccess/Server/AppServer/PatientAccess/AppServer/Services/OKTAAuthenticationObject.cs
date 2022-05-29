using System;
using System.Collections.Generic;

namespace PatientAccess.AppServer.Services
{
    public class OKTAAuthenticationObject
    {
        public DateTime expiresAt { get; set; }
        public string status { get; set; }
        public string sessionToken { get; set; }
        public Embedded _embedded { get; set; }
        public Links _links { get; set; }
    }


    public class Profile
    {
        public string login { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string locale { get; set; }
        public string timeZone { get; set; }
    }

    public class OKTAUser
    {
        public string id { get; set; }
        public Profile profile { get; set; }
    }

    public class Embedded
    {
        public OKTAUser user { get; set; }
    }

    public class Hints
    {
        public List<string> allow { get; set; }
    }

    public class Cancel
    {
        public string href { get; set; }
        public Hints hints { get; set; }
    }

    public class Links
    {
        public Cancel cancel { get; set; }
    }

}