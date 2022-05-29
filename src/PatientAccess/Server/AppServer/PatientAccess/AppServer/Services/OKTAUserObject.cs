using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PatientAccess.AppServer.Services
{
    public class OKTAUserObject
    {
        public string id { get; set; }
        public string status { get; set; }
        public DateTime created { get; set; }
        public DateTime activated { get; set; }
        public DateTime statusChanged { get; set; }
        public DateTime lastLogin { get; set; }
        public DateTime lastUpdated { get; set; }
        public object passwordChanged { get; set; }
        public Type type { get; set; }
        public UserProfile profile { get; set; }
        public Credentials credentials { get; set; }
        public Links1 _links { get; set; }
    }
     
    public class Type
    {
        public string id { get; set; }
        public string href { get; set; }
    }

    public class UserProfile
    {
        public string adamUserType { get; set; }
        public string lastName { get; set; }
        public string zipCode { get; set; }
        public string payrollLocation { get; set; }
        public string city { get; set; }
        public string displayName { get; set; }
        public string statuscd { get; set; }
        public string employeeID { get; set; }
        public string dn { get; set; }
        public string title { get; set; }
        public string login { get; set; }
        public string employeeNumber { get; set; }
        public string workLocation { get; set; }
        public string UPN { get; set; }
        public string adamJobCode { get; set; }
        public string company { get; set; }
        public string state { get; set; }
        public string department { get; set; }
        public string email { get; set; }
        public string info { get; set; }
        public string adamFaxNumber { get; set; }
        public string hireDate { get; set; }
        public string updatedBy { get; set; }
        public string manager { get; set; }
        public object secondEmail { get; set; }
        public string cn { get; set; }
        public string managerId { get; set; }
        public List<string> proxyAddresses { get; set; }
        public string firstName { get; set; }
        public string primaryPhone { get; set; }
        public string primaryContactNumber { get; set; }
        public string mobilePhone { get; set; }
        public string streetAddress { get; set; }
        public string organization { get; set; }
        public string middleName { get; set; }
        public string userType { get; set; }
        public string eTenetID { get; set; }
    }

    public class Provider
    {
        public string type { get; set; }
        public string name { get; set; }
    }

    public class Credentials
    {
        public Provider provider { get; set; }
    }

    public class Suspend
    {
        public string href { get; set; }
        public string method { get; set; }
    }

    public class Schema
    {
        public string href { get; set; }
    }

    public class ResetPassword
    {
        public string href { get; set; }
        public string method { get; set; }
    }

    public class ExpirePassword
    {
        public string href { get; set; }
        public string method { get; set; }
    }

    public class ChangeRecoveryQuestion
    {
        public string href { get; set; }
        public string method { get; set; }
    }

    public class Self
    {
        public string href { get; set; }
    }

    public class ResetFactors
    {
        public string href { get; set; }
        public string method { get; set; }
    }

    public class ChangePassword
    {
        public string href { get; set; }
        public string method { get; set; }
    }

    public class Deactivate
    {
        public string href { get; set; }
        public string method { get; set; }
    }

    public class Links1
    {
        public Suspend suspend { get; set; }
        public Schema schema { get; set; }
        public ResetPassword resetPassword { get; set; }
        public ExpirePassword expirePassword { get; set; }
        public ChangeRecoveryQuestion changeRecoveryQuestion { get; set; }
        public Self self { get; set; }
        public ResetFactors resetFactors { get; set; }
        public Type type { get; set; }
        public ChangePassword changePassword { get; set; }
        public Deactivate deactivate { get; set; }
    }
}