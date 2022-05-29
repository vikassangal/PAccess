using System;
using System.Collections.Generic;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.IdentityHubService;
using PatientAccess.Services.EMPIService;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Services
{
    /// <summary>
    /// Summary description for EMPIServiceTests
    /// </summary>
    [TestFixture]
    public class EMPIServiceTests  
    {

        #region Constants 

        private const int DHF_FACILITY_OID = 54;
        private const long MRN = 606;
        private const string EXPECTED_MEMIDNUM = "000000606";
        private const string EXPECTED_SRCCODE = "PBARPATPRV"; 
        private const string SEARCH_FIRSTNAME = "KAYLEE";
        private const string SEARCH_LASTNAME = "QUINATANA";
        private  DateTime  SEARCH_DOB = new DateTime(2014,01,24);
        private const string SEARCH_SSN = "469296999";
         private const string PATIENTNAME = "PATNAME";
        private const string LASTNAME = "RINGER";
        private const string FIRSTNAME = "TERRY";
        private const string DATEOFBIRTH = "PATDOB";
        private static readonly DateTime PATIENT_DOB = new DateTime( 1955, 3, 5 );
        private const string SOCIALSECURITYNUMBER = "PATSSN";
        private  const string PATIENT_SSN = "000000001";
        private const string HOSPITALCODE = "PATHSC";
        private const string MEDICALRECORDNUMBER = "MRN";

        #endregion Constants 

        #region Methods 

        [Test]
        public void TestGetMemberRequest_Build_MedicalRecordNumber()
        {
            var getMemberRequest = GetMemberRequest();
            Assert.AreEqual(EXPECTED_MEMIDNUM,getMemberRequest.memIdnum,"Could not build MemIdNum");
        }

        [Test]
        public void TestGetMemberRequest_Build_HospitalCode()
        {
            var getMemberRequest = GetMemberRequest();
            Assert.AreEqual(EXPECTED_SRCCODE, getMemberRequest.srcCode, "Could not build Source Code");
        }

        [Test]
        public void TestSearchMemberRequest_Build_DOB()
        {
            var service = GetEMPIServiceWithSearchMemberRequest();
            var searchCriteria = new PatientSearchCriteria(string.Empty, string.Empty, string.Empty,
                                                           new SocialSecurityNumber(), new Gender(), SEARCH_DOB.Month, SEARCH_DOB.Year, "0", "0")
            {
                DayOfBirth = 24
            };

            service.BuildSearchRequestWithDateOfBirth(searchCriteria);
            Assert.AreEqual(SEARCH_DOB.ToShortDateString(), service.SearchMemberRequest.member.memDate[0].dateVal,
                            "Could not build DOB in the search request");
        }

        [Test]
        public void TestSearchMemberRequest_Build_LN_FN()
        {
            var service = GetEMPIServiceWithSearchMemberRequest();
            var searchCriteria = new PatientSearchCriteria(string.Empty, SEARCH_FIRSTNAME, SEARCH_LASTNAME,
                                                           new SocialSecurityNumber(), new Gender(), 0, 0, "0", "0")
            {
                DayOfBirth = 0
            };

            service.BuildSearchRequestWithName(searchCriteria);
            Assert.AreEqual(SEARCH_FIRSTNAME, service.SearchMemberRequest.member.memName[0].onmFirst,
                            "Could not build first name in the search request");
            Assert.AreEqual(SEARCH_LASTNAME, service.SearchMemberRequest.member.memName[0].onmLast,
                            "Could not build last name in the search request");
        }
        [Test]
        public void TestSearchMemberRequest_Build_SSN()
        {
            var service = GetEMPIServiceWithSearchMemberRequest();
            var searchCriteria = new PatientSearchCriteria(string.Empty, string.Empty, string.Empty,
                                                           new SocialSecurityNumber(SEARCH_SSN), new Gender(), 01, 2014, "0", "0");
            searchCriteria.DayOfBirth = 0;

            service.BuildSearchRequestWithSSN(searchCriteria);
            Assert.AreEqual(SEARCH_SSN, service.SearchMemberRequest.member.memIdent[0].idNumber,
                            "Could not build first name in the search request"); 
        }

        [Test]
        public void TestBuildPatientsSearchResultsFrom_Count()
        {
            var response = GetEMPISearchMemberResponse();
            var service = GetEMPIServiceWithSearchMemberRequest();

            var results = service.BuildPatientsSearchResultsFrom(response);
            Assert.AreEqual(results.Count, 2);
        }

        [Test]
        public void TestBuildPatientsSearchResultsFrom_FirstPatient()
        {
            var response = GetEMPISearchMemberResponse();
            var service = GetEMPIServiceWithSearchMemberRequest();

            var results = service.BuildPatientsSearchResultsFrom(response);

            Assert.AreEqual(results[1].MedicalRecordNumber, 987654);
            Assert.AreEqual(results[1].SocialSecurityNumber, "--");
            Assert.AreEqual(results[1].DateOfBirth.ToShortDateString(), "3/16/2011");
            Assert.AreEqual(results[1].HspCode, "PRV");
        }

        [Test]
        public void TestBuildPatientsSearchResultsFrom_SecondPatient()
        {
            var response = GetEMPISearchMemberResponse();
            var service = GetEMPIServiceWithSearchMemberRequest();

            var results = service.BuildPatientsSearchResultsFrom(response);
            Assert.AreEqual(results[1].MedicalRecordNumber, 987654);
            Assert.AreEqual(results[1].DateOfBirth.ToShortDateString(), "3/16/2011");
            Assert.AreEqual(results[1].HspCode, "PRV");
        }

        [Test]
        public void TestBuildPatientsSearchResultsFrom()
        {
            var response = GetEMPISearchMemberResponse();
            var service = GetEMPIServiceWithSearchMemberRequest();

            var results = service.BuildPatientsSearchResultsFrom(response);
            Assert.AreEqual(results.Count, 2);
        }
        #endregion Methods 

        #region Support Methods
        private MemberGetRequest GetMemberRequest()
        {
            var facility = new Facility() { Code = "PRV", Oid = DHF_FACILITY_OID };
            var patient = new Patient() { Facility = facility, MedicalRecordNumber = MRN };
            var service = new EMPIService(facility, new TestFacilityNameMapper());
            service.BuildGetMemberRequest(patient);
            var getMemberRequest = service.GetMemberRequest;
            return getMemberRequest;
        }
        private EMPIService GetEMPIServiceWithSearchMemberRequest()
        {
            var facility = new Facility() {Code = "PRV", Oid = DHF_FACILITY_OID};
            var patient = new Patient() {Facility = facility, MedicalRecordNumber = MRN};
            var service = new EMPIService(facility, new TestFacilityNameMapper());
            service.BuildSearchMemberRequest();
            return service;
        }

        private IEnumerable<Member> GetMemberResponse()
        {
            var getMemberResponse = new Member[2];
            var member1 = new Member {memName = new MemNameWs[1]};

            member1.memName[0] = new MemNameWs {attrCode = PATIENTNAME, onmLast = LASTNAME, onmFirst = FIRSTNAME};

            member1.memDate = new MemDateWs[1];
            member1.memDate[0] = new MemDateWs {attrCode = DATEOFBIRTH, dateVal = PATIENT_DOB.ToShortDateString()};

            var member2 = new Member {memIdent = new MemIdentWs[1]};

            member2.memIdent[0] = new MemIdentWs {attrCode = SOCIALSECURITYNUMBER, idNumber = PATIENT_SSN};

            getMemberResponse[0] = member1;
            getMemberResponse[1] = member2;
            return getMemberResponse;
        }
        private IEnumerable<Member> GetEMPISearchMemberResponse()
        {
            
            var member1 = new Member
            {
                memHead = new MemHeadWs { entRecno = 1, matchScore = 27, entRecnos = new long[1] { 1 }, recMtime = "1/24/2014 12:00:00 AM" },
                memIdent = new MemIdentWs[2],
                memName = new MemNameWs[1]
            };
           
            member1.memIdent[0] = new MemIdentWs { attrCode = MEDICALRECORDNUMBER, idNumber = "456789" };
            member1.memIdent[1] = new MemIdentWs { attrCode = SOCIALSECURITYNUMBER, idNumber = "277-39-5165" };

            member1.memName[0] = new MemNameWs { attrCode = "PATALIAS", onmLast = "QUINCE", onmFirst = "HAYLEE", onmMiddle = "S" };


            var member2 = new Member
            {
                memHead = new MemHeadWs { entRecno = 1, matchScore = 27, entRecnos = new long[1] { 1 }, recMtime = "1/24/2014 12:00:00 AM" },
                memName = new MemNameWs[1],
                memDate = new MemDateWs[1],
                memAttr = new MemAttrWs[1]
            };
           
            member2.memAttr[0] = new MemAttrWs { attrVal = "DHF", attrCode = HOSPITALCODE };

            member2.memName[0] = new MemNameWs { attrCode = PATIENTNAME, onmLast = "QUINTANA", onmFirst = "KAYLEE", onmMiddle = "S" };
            member2.memDate[0] = new MemDateWs { attrCode = DATEOFBIRTH, dateVal = "1/24/2014 12:00:00 AM" };


            var member3 = new Member
            {
                memHead = new MemHeadWs { entRecno = 2, matchScore = 97, entRecnos = new long[1] { 2 }, recMtime = "1/24/2014 12:00:00 AM" },
                memIdent = new MemIdentWs[1],
                memDate = new MemDateWs[1],
                memName = new MemNameWs[2],
                memAttr = new MemAttrWs[1]
            };
          
            member3.memIdent[0] = new MemIdentWs { attrCode = MEDICALRECORDNUMBER, idNumber = "987654" };

            member3.memName[0] = new MemNameWs
            {
                attrCode = "PATALIAS",
                onmLast = "QUINCE",
                onmFirst = "HAYLEE",
                onmMiddle = "S",
            };

            member3.memAttr[0] = new MemAttrWs { attrVal = "PRV", attrCode = HOSPITALCODE };
            member3.memName[1] = new MemNameWs { attrCode = "PATNAME", onmLast = "QUINTANA", onmFirst = "KAYLEE", onmMiddle = "S" };

            member3.memDate[0] = new MemDateWs { attrCode = "PATDOB", dateVal = "3/16/2011 12:00:00 AM" };


            Member[] allMembers = { member1, member2, member3 };
            return allMembers;
        }
       
        #endregion

    }
}