using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Extensions.PersistenceCommon;
using log4net;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.IdentityHubService;
using PatientAccess.Utilities;

namespace PatientAccess.Services.EMPIService
{
    [Serializable]
    public class EMPIService
    {

        #region Public Methods
      
        public EMPIPatient GetEMPIPatientFor(Patient pbarPatient)
        {
            var ePatient = new EMPIPatient {Patient = pbarPatient};
            try
            {
                BuildGetMemberRequest(pbarPatient);
                var getMemberResponse = identityHub.getMember(GetMemberRequest);
                if (getMemberResponse.Count() != 0)
                {
                    ePatient = BuildEMPIPatientFromResponse(pbarPatient, getMemberResponse);
                    ePatient.EmpiPatientFound = true;
                } 
            }
            catch (Exception e)
            {
                if (e.Message.Contains("no member(s) found") || e.Message.Contains("no candidates"))
                {
                    LogGetMemberExceptionMessage(e, "No patient found in EMPI service for the given MRN:");
                }
                else if (e.Message.Contains("There was no endpoint listening at") ||
                         e.Message.Contains("The IBM Initiate Identity Hub Server may be down"))
                {
                    LogGetMemberExceptionMessage(e, "EMPI System down, no endpoint listening at:");
                }
                else if (e.Message.Contains("The request channel timed out"))
                {
                    LogGetMemberExceptionMessage(e, "EMPI System timed out : ");
                }
                else
                {
                    LogGetMemberExceptionMessage(e, "Exception occurred while getting member for the given MRN:");
                }
            }

            return ePatient;
        }

        private void LogGetMemberExceptionMessage(Exception e, string message)
        {
            Logger.ErrorFormat(
                message + " {0},Facility: {1}, Message: {2}, Exception: {3} , at: {4}",
                GetMemberRequest.memIdnum, GetMemberRequest.srcCode, e.Message, e, ConfigurationManager.AppSettings["IdentityHubServiceUrl"]);
        }

        private EMPIPatient BuildEMPIPatientFromResponse(Patient patient, Member[] getMemberResponse)
        {
            var ePatient = new EMPIPatient(); 
            var responsePatient = new Patient();
            foreach (var member in getMemberResponse)
            {
                ExtractMemberAttributes(responsePatient, member);
                ExtractMemberDateOfBirth(responsePatient, member);
                ExtractMemberIdentity(responsePatient, member);
                ExtractMemberName(responsePatient, member);
                ExtractMemberAddress(responsePatient, member);
                ExtractMemberPhone(responsePatient, member);
                responsePatient.MedicalRecordNumber = patient.MedicalRecordNumber;
            }
            responsePatient.SetPatientContextHeaderData();
            patient.PatientContextHeaderData = responsePatient.PatientContextHeaderData;
            ePatient.Patient = responsePatient;
            foreach (var member in getMemberResponse)
            {
                ExtractMemberID(ePatient, member);
            }
            return ePatient;
        }


        public PatientSearchResponse SearchEMPI(PatientSearchCriteria searchCriteria)
        {
            patientSearchCriteria = searchCriteria;
            BuildSearchMemberRequest();
            BuildSearchRequestWithSSN(searchCriteria);
            BuildSearchRequestWithPhone(searchCriteria);
            BuildSearchRequestWithDateOfBirth(searchCriteria);
            BuildSearchRequestWithName(searchCriteria);
            BuildSearchRequestWithGender(searchCriteria);
            return GetPatientSearchResponse();
        }

        #endregion

        #region Private Methods
        private PatientSearchResponse GetPatientSearchResponse()
        {
            try
            {
                var searchMemberResponse = identityHub.searchMember(SearchMemberRequest);
                patientSearchResponse = new PatientSearchResponse
                {
                    PatientSearchResults = BuildPatientsSearchResultsFrom(searchMemberResponse)
                };
                if (patientSearchResponse.PatientSearchResults.Count > 0)
                {
                    patientSearchResponse.HasEMPIResults = true;
                    patientSearchResponse.EMPIResultStatus = EMPIResultStatus.EMPIRESULTSFOUND;
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("no buckets were generated from search input data") ||
                    e.Message.Contains("no candidates"))
                {
                    LogSearchMemberExceptionMessage(e,  "No patients found in EMPI service for search criteria: ");
                    patientSearchResponse.EMPIResultStatus = EMPIResultStatus.NOEMPIRESULTSFOUND;
                }
                else if (e.Message.Contains("There was no endpoint listening at") ||
                         e.Message.Contains("The IBM Initiate Identity Hub Server may be down"))
                {
                    LogSearchMemberExceptionMessage(e, "EMPI System down, no endpoint listening at");
                    
                    patientSearchResponse.EMPIResultStatus = EMPIResultStatus.SYSTEMDOWN;
                }
                else if (e.Message.Contains("The request channel timed out"))
                {
                    LogSearchMemberExceptionMessage(e, "EMPI System timed out :"); 

                    patientSearchResponse.EMPIResultStatus = EMPIResultStatus.TIMEOUT;
                }
                else
                {
                    patientSearchResponse.EMPIResultStatus = EMPIResultStatus.NOEMPIRESULTSFOUND;
                    LogSearchMemberExceptionMessage(e, "Exception occurred while searching for search criteria:"); 
                   
                    patientSearchResponse.EMPIResultStatus = EMPIResultStatus.NOEMPIRESULTSFOUND;
                }
            }
            return patientSearchResponse;
        }

        private void LogSearchMemberExceptionMessage(Exception e, string message)
        {
            Logger.ErrorFormat(
                message +
                " {0},Last Name: {1}, SSN: {2}, DOB: {3},Gender: {4} ,Phone Number: {5}, Error Message: {6}, Exception: {7} , at: {8} ",
                patientSearchCriteria.FirstName, patientSearchCriteria.LastName,
                patientSearchCriteria.SocialSecurityNumber, patientSearchCriteria.DateOfBirth.ToLongDateString(),
                patientSearchCriteria.Gender, patientSearchCriteria.PhoneNumber, e.Message, e,
                ConfigurationManager.AppSettings["IdentityHubServiceUrl"]);
        }

        private void BuildSearchRequestWithGender(PatientSearchCriteria searchCriteria)
        {
            if (!String.IsNullOrEmpty(searchCriteria.Gender.Code))
            {
                SearchMemberRequest.member.memAttr = new MemAttrWs[10];
                SearchMemberRequest.member.memAttr[0] = new MemAttrWs
                {
                    attrCode = PATIENTGENDER,
                    attrVal = searchCriteria.Gender.Code,
                    elemDesc = searchCriteria.Gender.Description
                };
            }
        }

        public void BuildSearchRequestWithName(PatientSearchCriteria searchCriteria)
        {
            if (!String.IsNullOrEmpty(searchCriteria.LastName) || !String.IsNullOrEmpty(searchCriteria.FirstName))
            {
                SearchMemberRequest.member.memName = new MemNameWs[10];
                SearchMemberRequest.member.memName[0] = new MemNameWs
                {
                    attrCode = PATIENTNAME,
                    onmLast = searchCriteria.LastName,
                    onmFirst = searchCriteria.FirstName
                };
            }
        }

        public void BuildSearchRequestWithDateOfBirth(PatientSearchCriteria searchCriteria)
        {
            if (!String.IsNullOrEmpty(searchCriteria.DateOfBirth.ToLongDateString()) &&
                searchCriteria.DateOfBirth != DateTime.MinValue)
            {
                SearchMemberRequest.member.memDate = new MemDateWs[10];
                SearchMemberRequest.member.memDate[0] = new MemDateWs

                {
                    attrCode = DATEOFBIRTH,
                    dateVal = searchCriteria.DateOfBirth.ToShortDateString()
                };
            }
        }

        public void BuildSearchRequestWithSSN(PatientSearchCriteria searchCriteria)
        {
            if (!String.IsNullOrEmpty(searchCriteria.SocialSecurityNumber.ToString()))
            {
                SearchMemberRequest.member.memIdent[0] = new MemIdentWs
                    {
                        attrCode = SOCIALSECURITYNUMBER,
                        idIssuer = CurrentFacility.Code,
                        idNumber = searchCriteria.SocialSecurityNumber.ToString()
                    };
            }
        }

        public void BuildSearchRequestWithPhone(PatientSearchCriteria searchCriteria)
        {
            if (searchCriteria.PhoneNumber == null ||
                String.IsNullOrEmpty(searchCriteria.PhoneNumber.AsUnformattedString())) return;
            SearchMemberRequest.member.memPhone = new MemPhoneWs[10];
            SearchMemberRequest.member.memPhone[0] = new MemPhoneWs
            {
                attrCode = MAILINGPHONE,
                phArea = searchCriteria.PhoneNumber.AreaCode,
                phNumber = searchCriteria.PhoneNumber.Number
            };
            SearchMemberRequest.member.memPhone[1] = new MemPhoneWs
            {
                attrCode = CELLPHONE ,
                phArea = searchCriteria.PhoneNumber.AreaCode,
                phNumber = searchCriteria.PhoneNumber.Number
            };
        }

        public void BuildSearchMemberRequest()
        {
            SearchMemberRequest = new MemberSearchRequest
            {
                cvwName = request.cvwName,
                userName = ConfigurationManager.AppSettings[EMPISERVICE_USERNAME],
                userPassword = ConfigurationManager.AppSettings[EMPISERVICE_PASSWORD],
                getType = request.getType,
                memType = request.memType,
                audMode = request.audMode,
                recStatFilter = request.recStatFilter,
                maxRows = request.maxRows,
                segCodeFilter = request.searchSegCodeFilter,
                entType = request.entType,
                memStatFilter = request.memStatFilter,
                minScore = request.minScore,
                member = new Member()
            };
            var memHead = new MemHeadWs { memRecno = 0, entRecno = 0 };
            SearchMemberRequest.member.memHead = memHead;
            SearchMemberRequest.member.memIdent = new MemIdentWs[10];
        }
      
    
       private static void ExtractMemberPhone(Patient patient, Member member)
        {
            var dictionaryEntry = new Dictionary<string, MemPhoneWs>(); 
            if (member.memPhone != null)
            {
                foreach (var attribute in member.memPhone)
                {
                    dictionaryEntry.Add(attribute.attrCode, attribute);
                }
                if (dictionaryEntry.ContainsKey(MAILINGPHONE))
                {
                    var homePhone = dictionaryEntry[MAILINGPHONE];
                    if (homePhone != null)
                    {
                        var homePhoneNumber = BuildMailingPhone(homePhone);
                       
                        if (patient.ContactPointWith(TypeOfContactPoint.NewMailingContactPointType()) != null)
                        {
                            patient.ContactPointWith(TypeOfContactPoint.NewMailingContactPointType()).PhoneNumber =
                                homePhoneNumber;
                        }
                        else
                        {
                            var cp = new ContactPoint(TypeOfContactPoint.NewMailingContactPointType())
                            {
                                PhoneNumber = homePhoneNumber
                            };
                            patient.AddContactPoint(cp);
                        }
                    }
                }

                if (dictionaryEntry.ContainsKey(CELLPHONE))
                {
                    var mobilePhone = dictionaryEntry[CELLPHONE];
                    if (mobilePhone != null)
                    {
                        var mobilePhoneNumber = BuildMobilePhone(mobilePhone);

                        if (patient.ContactPointWith(TypeOfContactPoint.NewMobileContactPointType()) != null)
                        {
                            patient.ContactPointWith(TypeOfContactPoint.NewMobileContactPointType()).PhoneNumber =
                                mobilePhoneNumber;
                        }
                        else
                        {
                            var cp = new ContactPoint(TypeOfContactPoint.NewMobileContactPointType())
                            {
                                PhoneNumber = mobilePhoneNumber
                            };
                            patient.AddContactPoint(cp);
                        }
                    }
                }

                if (dictionaryEntry.ContainsKey(PHYSICALPHONE))
                {
                    var physicalPhone =  dictionaryEntry[PHYSICALPHONE];
                    if (physicalPhone != null)
                    {
                        var physicalPhoneNumber = BuildPhysicalPhone(physicalPhone);

                        if (patient.ContactPointWith(TypeOfContactPoint.NewPhysicalContactPointType()) != null)
                        {
                            patient.ContactPointWith(TypeOfContactPoint.NewPhysicalContactPointType()).PhoneNumber = physicalPhoneNumber;
                        }
                        else
                        {
                            var cp = new ContactPoint(TypeOfContactPoint.NewMailingContactPointType())
                            {
                                PhoneNumber = physicalPhoneNumber
                            };
                            patient.AddContactPoint(cp);
                        }
                    }
                }
            }
        }

     private void ExtractMemberID(EMPIPatient empiPatient, Member member)
        {
            var dictionaryEntry = new Dictionary<string, MemIdsWs>();

            if (member.memIds != null)
            {
                foreach (var attribute in member.memIds)
                {
                    dictionaryEntry.Add(attribute.attrCode, attribute);
                }

                if (dictionaryEntry.ContainsKey(DRIVERSLICENSE))
                {
                    var driversLic = dictionaryEntry[DRIVERSLICENSE];
                    if (driversLic != null)
                    {
                        var driversLicense = BuildDriversLicense(driversLic, CurrentFacility.Oid);
                        if (!String.IsNullOrEmpty(driversLicense.Number))
                        {
                            empiPatient.Patient.DriversLicense = driversLicense;
                        }
                    }
                }

                if (dictionaryEntry.ContainsKey(NEXTOFKIN))
                {
                    var nok = dictionaryEntry[NEXTOFKIN];
                    if (nok != null)
                    {
                        empiPatient.EmergencyContact1 = BuildPatientNextOfKin(empiPatient.Patient, nok);
                    }
                }

                if (dictionaryEntry.ContainsKey(GUARANTOR))
                {
                    var guarantor = dictionaryEntry[GUARANTOR];
                    if (guarantor != null)
                    {
                        empiPatient.Guarantor = BuildGuarantor(empiPatient.Patient, guarantor);
                    }
                }

                if (dictionaryEntry.ContainsKey(PRIMARYINSURANCE))
                {
                    var primaryInsurance = dictionaryEntry[PRIMARYINSURANCE];
                    if (primaryInsurance != null)
                    {
                        empiPatient.Insurance = BuildInsurance(empiPatient, primaryInsurance);
                    }
                }

                if (dictionaryEntry.ContainsKey(SECONDARYINSURANCE))
                {
                    var secondaryInsurance = dictionaryEntry[SECONDARYINSURANCE];
                    if (secondaryInsurance != null)
                    {
                        empiPatient.Insurance = BuildInsurance(empiPatient, secondaryInsurance);
                    }
                }
            }
        }

        private void ExtractMemberName(Patient patient, Member member)
        {
            var dictionaryEntry = new Dictionary<string, MemNameWs>();
            if (member.memName != null)
            {
                foreach (var attribute in member.memName)
                {
                    dictionaryEntry.Add(attribute.attrCode, attribute);
                }

                if (dictionaryEntry.ContainsKey(PATIENTNAME))
                {
                    var name = dictionaryEntry[PATIENTNAME];
                    if (name != null)
                    {
                        var patientName = BuildPatientName(name);
                        if (patientName != null)
                        {
                            patient.FirstName = patientName.FirstName;
                            patient.LastName = patientName.LastName;
                            patient.MiddleInitial = patientName.MiddleInitial;
                        }
                    }
                }

                if (dictionaryEntry.ContainsKey(PATIENTALIAS))
                {
                    var alias = dictionaryEntry[PATIENTALIAS];
                    if (alias != null)
                    {
                        var aliasName = BuildPatientAlias(alias);
                        if (aliasName != null)
                        {
                            patient.AddAlias(aliasName);
                        }
                    }
                }

                if (dictionaryEntry.ContainsKey(MADIENNAME))
                {
                    var maidenName = dictionaryEntry[MADIENNAME];
                    if (maidenName != null)
                    {
                        var patientMaidenName = BuildPatientMaidenName(maidenName);
                        if (patientMaidenName != null)
                        {
                            patient.MaidenName = patientMaidenName;
                        }
                    }
                }
            }
        }

        private void ExtractMemberIdentity(Patient patient, Member member)
        {
            var dictionaryEntry = new Dictionary<string, MemIdentWs>();
            if (member.memIdent != null)
            {
                foreach (var attribute in member.memIdent)
                {
                    dictionaryEntry.Add(attribute.attrCode, attribute);
                }

                if (dictionaryEntry.ContainsKey(SOCIALSECURITYNUMBER))
                {
                    var ssn = dictionaryEntry[SOCIALSECURITYNUMBER];
                    if (ssn != null)
                    {
                        var patientSsn = BuildPatientSSN(ssn);
                        if (patientSsn != null)
                        {
                            patient.SocialSecurityNumber = patientSsn;
                        }
                    }
                }
            }
        }

        private void ExtractMemberDateOfBirth(Patient patient, Member member)
        {
            var dictionaryEntry = new Dictionary<string, MemDateWs>();
            if (member.memDate != null)
            {
                foreach (var attribute in member.memDate)
                {
                    dictionaryEntry.Add(attribute.attrCode, attribute);
                }

                if (dictionaryEntry.ContainsKey(DATEOFBIRTH))
                {
                    var dob = dictionaryEntry[DATEOFBIRTH];
                    if (dob != null)
                    {
                        var patientDob = BuildPatientDOB(dob);
                        if (patientDob != DateTime.MinValue)
                            patient.DateOfBirth = patientDob;
                    }
                }
            }
        }

        private void ExtractMemberAddress(Patient patient, Member member)
        {
            var dictionaryEntry = new Dictionary<string, MemAddrWs>();
            if (member.memAddr != null)
            {
                foreach (var attribute in member.memAddr)
                {
                    dictionaryEntry.Add(attribute.attrCode, attribute);
                }

                if (dictionaryEntry.ContainsKey(MAILADDRESS))
                {
                    var mailingAddress = dictionaryEntry[MAILADDRESS];
                    if (mailingAddress != null)
                    {
                        var patientMailingAddress = BuildPatientMailingAddress(mailingAddress);
                        var patientMailingContactPoint =
                            patient.ContactPointWith(TypeOfContactPoint.NewMailingContactPointType());
                        if (patientMailingContactPoint != null)
                        {
                            patientMailingContactPoint.Address = new Address();
                            if (patientMailingAddress.Address1 != string.Empty)
                            {
                                patientMailingContactPoint.Address = patientMailingAddress;
                            }
                        }
                        else
                        {
                            patientMailingContactPoint =
                                new ContactPoint(TypeOfContactPoint.NewMailingContactPointType())
                                {
                                    Address = new Address()
                                };
                            if (patientMailingAddress.Address1 != string.Empty)
                            {
                                patientMailingContactPoint.Address = patientMailingAddress;
                            }
                        }
                    }
                }

                if (dictionaryEntry.ContainsKey(PHYSICALADDRESS))
                {
                    var physicalAddress = dictionaryEntry[PHYSICALADDRESS];
                    if (physicalAddress != null)
                    {
                        var patientPhysicalAddress = BuildPatientPhysicalAddress(physicalAddress);
                        var patientPhysicalContactPoint =
                            patient.ContactPointWith(TypeOfContactPoint.NewPhysicalContactPointType());
                        if (patientPhysicalContactPoint != null)
                        {
                            patientPhysicalContactPoint.Address = new Address();
                            if (patientPhysicalAddress.Address1 != string.Empty)
                            {
                                patientPhysicalContactPoint.Address = patientPhysicalAddress;
                            }
                        }
                        else
                        {
                            patientPhysicalContactPoint =
                                new ContactPoint(TypeOfContactPoint.NewPhysicalContactPointType())
                                {
                                    Address = new Address()
                                };
                            if (patientPhysicalAddress.Address1 != string.Empty)
                            {
                                patientPhysicalContactPoint.Address = patientPhysicalAddress;
                            }
                        }
                    }
                }

                if (dictionaryEntry.ContainsKey(EMPLOYERADDRESS))
                {
                    var employerAddress = dictionaryEntry[EMPLOYERADDRESS];
                    if (employerAddress != null)
                    {
                        var patientEmployerAddress = BuildPatientEmployerAddress(employerAddress);
                        var patientEmployerContactPoint =
                            patient.ContactPointWith(TypeOfContactPoint.NewEmployerContactPointType());
                        if (patientEmployerContactPoint != null)
                        {
                            patientEmployerContactPoint.Address = new Address();
                            if (patientEmployerAddress.Address1 != string.Empty)
                            {
                                patientEmployerContactPoint.Address = patientEmployerAddress;
                            }
                        }
                        else
                        {
                            patientEmployerContactPoint =
                                new ContactPoint(TypeOfContactPoint.NewEmployerContactPointType())
                                {
                                    Address = new Address()
                                };
                            if (patientEmployerAddress.Address1 != string.Empty)
                            {
                                patientEmployerContactPoint.Address = patientEmployerAddress;
                            }
                        }
                        patient.Employment.Employer.PartyContactPoint = patientEmployerContactPoint;
                    }
                }
            }
        }

        private void ExtractMemberAttributes(Patient patient, Member member)
        {
            var dictionaryEntry = new Dictionary<string, MemAttrWs>();
            if (member.memAttr != null)
            {
                foreach (var attribute in member.memAttr)
                {
                    dictionaryEntry.Add(attribute.attrCode, attribute);
                }

                if (dictionaryEntry.ContainsKey(PATIENTGENDER))
                {
                    var gender = dictionaryEntry[PATIENTGENDER];
                    if (gender != null)
                    {
                        var patientGender = BuildPatientGender(gender);
                        if (patientGender != null && !String.IsNullOrEmpty(patientGender.Code))
                        {
                            patient.Sex = patientGender;
                        }
                    }
                }

                if (dictionaryEntry.ContainsKey(PATIENTRELIGION))
                {
                    var religion = dictionaryEntry[PATIENTRELIGION];
                    if (religion != null)
                    {
                        var patientReligion = BuildPatientReligion(religion);
                        if (patientReligion != null && !String.IsNullOrEmpty(patientReligion.Code))
                        {
                            patient.Religion = patientReligion;
                        }
                    }
                }

                if (dictionaryEntry.ContainsKey(PATIENTRACE))
                {
                    var race = dictionaryEntry[PATIENTRACE];
                    if (race != null)
                    {
                        var patientRace = BuildPatientRace(race);
                        if (patientRace != null && !String.IsNullOrEmpty(patientRace.Code))
                        {
                            patient.Race = patientRace;
                        }
                    }
                }

                if (dictionaryEntry.ContainsKey(PATIENTMARITALSTATUS))
                {
                    var maritalStatus = dictionaryEntry[PATIENTMARITALSTATUS];
                    if (maritalStatus != null)
                    {
                        var patientMaritalStatus = BuildPatientMaritalStatus(maritalStatus);
                        if (patientMaritalStatus != null && !String.IsNullOrEmpty(patientMaritalStatus.Code))
                        {
                            patient.MaritalStatus = patientMaritalStatus;
                        }
                    }
                }

                if (dictionaryEntry.ContainsKey(PATIENTETHNICITY))
                {
                    var ethnicity = dictionaryEntry[PATIENTETHNICITY];
                    if (ethnicity != null)
                    {
                        var patientEthnicity = BuildPatientEthnicity(ethnicity);
                        if (patientEthnicity != null && !String.IsNullOrEmpty(patientEthnicity.Code))
                        {
                            patient.Ethnicity = patientEthnicity;
                        }
                    }
                }

                if (dictionaryEntry.ContainsKey(IPACODE))
                {
                    var ipa = dictionaryEntry[IPACODE];
                    if (ipa != null)
                    {
                        patient.MedicalGroupIPA = BuildMedicalGroupIpa(ipa);
                    }
                }

                if (dictionaryEntry.ContainsKey(EMPLOYER))
                {
                    var employerName = dictionaryEntry[EMPLOYER];
                    if (employerName != null)
                    {
                        var employment = BuildEmployerName(employerName);
                        if (employment != null && !String.IsNullOrEmpty(employment.Employer.Name))
                        {
                            patient.Employment = new Employment
                            {
                                Employer = new Employer { Name = employerName.attrVal.Trim()  }
                            };
                            
                        }
                        var employmentStatus = BuildEmploymentStatusFromEmpi(employerName);
                        if (employmentStatus != null && !String.IsNullOrEmpty(employmentStatus.Code))
                        {
                            if(patient.Employment != null)
                            patient.Employment.Status = employmentStatus;
                        }

                    }
                }
            }
        }

        private EmploymentStatus BuildEmploymentStatusFromEmpi(MemAttrWs employerName)
        {
            string employerNameStr = employerName.attrVal.Trim().Replace("-", " ").Replace("\r\n", string.Empty);
            switch (employerNameStr)
            {
                case EmploymentStatus.NOT_EMPLOYED_DESC:
                    return EmploymentStatus.NewNotEmployed();
                case EmploymentStatus.RETIRED_DESC:
                    return EmploymentStatus.NewRetired();
                case EmploymentStatus.SELF_EMPLOYED_DESC:
                    return  EmploymentStatus.NewSelfEmployed();
            }
            return null;
        }

        public void BuildGetMemberRequest(Patient patient)
        {
            var patientFacilityCode = patient.Facility.Code;
            
            patientFacilityCode = PbartoEMPIFacilityNameMapper.GetEMPIFacilityCode( patientFacilityCode );
            
            var getRequest = new Request
                {
                    memIdnum = patient.MedicalRecordNumber.ToString(CultureInfo.InvariantCulture).PadLeft(9, '0'),
                    srcCode = PBARPAT + patientFacilityCode
                };
            GetMemberRequest = new MemberGetRequest
            {
                cvwName = getRequest.cvwName,
                entType = getRequest.entType,
                getType = getRequest.getType,
                memIdnum = getRequest.memIdnum,
                memType = getRequest.memType,
                userName = ConfigurationManager.AppSettings[EMPISERVICE_USERNAME],
                userPassword = ConfigurationManager.AppSettings[EMPISERVICE_PASSWORD],
                segCodeFilter = getRequest.segCodeFilter,
                srcCode = getRequest.srcCode
            };
        }

        private static PhoneNumber BuildPhysicalPhone(MemPhoneWs physicalPhone)
        {
            var phoneNumber = physicalPhone.phArea != string.Empty
                                    ? new PhoneNumber(physicalPhone.phArea,
                                                      physicalPhone.phNumber)
                                    : new PhoneNumber(
                                          StringFilter.RemoveFirstCharNonNumberAndRestNonNumber(physicalPhone.phNumber));
            return phoneNumber;
        }

        private static PhoneNumber BuildMobilePhone(MemPhoneWs mobilePhone)
        {

            var cellPhoneNumber = mobilePhone.phArea != string.Empty
                                      ? new PhoneNumber(mobilePhone.phArea,
                                                        mobilePhone.phNumber)
                                      : new PhoneNumber(
                                            StringFilter.RemoveFirstCharNonNumberAndRestNonNumber(mobilePhone.phNumber));
            return cellPhoneNumber;
        }

        private static PhoneNumber BuildMailingPhone(MemPhoneWs homePhone)
        {
            //EMPI provides home phone in formats
            //1. phArea (915) and phNumber (2012177)
            //2. phNumber ((915)861-7086)

            var phoneNumber = homePhone.phArea != string.Empty
                ? new PhoneNumber(homePhone.phArea,
                    homePhone.phNumber)
                : new PhoneNumber(
                    StringFilter.RemoveFirstCharNonNumberAndRestNonNumber(homePhone.phNumber));

            return phoneNumber;
        }
 
        private Guarantor BuildGuarantor(Patient patient, MemIdsWs guarantor)
        {
            var lastname = string.Empty;
            var relationship = string.Empty;
            var pharea = string.Empty;
            var mobilephone = string.Empty;
            var firstname = string.Empty;
            var middlename = string.Empty;
            var phnumber = string.Empty;
            for (var x = 0; x < guarantor.fldNames.Length; x++)
            {
                var attribute = guarantor.fldNames[x];
                switch (attribute)
                {
                    case "lastname":
                        lastname = guarantor.values[x];
                        break;
                    case "relationship":
                        relationship = guarantor.values[x];
                        break;
                    case "pharea":
                        pharea = guarantor.values[x];
                        break;
                    case "mobilephone":
                        mobilephone = guarantor.values[x];
                        break;
                    case "firstname":
                        firstname = guarantor.values[x];
                        break;
                    case "middlename":
                        middlename = guarantor.values[x];
                        break;
                    case "phnumber":
                        phnumber = guarantor.values[x];
                        break;
                }
            }
            var guarantorName = new Name(firstname, lastname, middlename);
            var pbarGuarantor = new Guarantor(PersistentModel.NEW_OID, DateTime.Now,
                guarantorName);

            var guarantorRelationshipType = RelationshipTypeBroker.RelationshipTypeWith(CurrentFacility.Oid, relationship);

            if (guarantorRelationshipType != null)
            {
             
                    var aRelationship = new Relationship(guarantorRelationshipType, patient.GetType(), pbarGuarantor.GetType());

                    patient.RemoveRelationship(guarantorRelationshipType);
                    patient.AddRelationship(aRelationship);

                    pbarGuarantor.RemoveRelationship(guarantorRelationshipType);
                    pbarGuarantor.AddRelationship(aRelationship);
                
                if (pbarGuarantor.ContactPointWith(TypeOfContactPoint.NewMobileContactPointType()) != null)
                {
                    pbarGuarantor.ContactPointWith(TypeOfContactPoint.NewMobileContactPointType()).PhoneNumber =
                        new PhoneNumber(StringFilter.RemoveFirstCharNonNumberAndRestNonNumber(mobilephone));
                }
                else
                {
                    var mobileCP = new ContactPoint(TypeOfContactPoint.NewMobileContactPointType())
                    {
                        PhoneNumber = new PhoneNumber(mobilephone)
                    };
                    pbarGuarantor.AddContactPoint(mobileCP);
                }

                if (pbarGuarantor.ContactPointWith(TypeOfContactPoint.NewMailingContactPointType()) != null)
                {
                    pbarGuarantor.ContactPointWith(TypeOfContactPoint.NewMailingContactPointType()).PhoneNumber =
                   pharea != string.Empty
                       ? new PhoneNumber(pharea, phnumber)
                       : new PhoneNumber(StringFilter.RemoveFirstCharNonNumberAndRestNonNumber(phnumber));
                }
                else
                {
                    var mailingCP = new ContactPoint(TypeOfContactPoint.NewMailingContactPointType()) { PhoneNumber = new PhoneNumber(pharea, phnumber) };
                    pbarGuarantor.AddContactPoint(mailingCP);
                }

            }
            return pbarGuarantor;
        }
        private Insurance BuildInsurance(EMPIPatient patient, MemIdsWs insurance)
        {
            var insplanid = string.Empty;
            var grpno = string.Empty;
            var policyno = string.Empty;
            var insrel = string.Empty;

            //Billingaddress
            var inscoaddr = string.Empty;
            var inscoaddrcity = string.Empty;
            State inscoaddrstate = new State();
            ZipCode inscoaddrzip = new ZipCode();

            //BillingPhone
            var inscoPhone = string.Empty;

            //Authorization effective date
            var effectdate = string.Empty;

            //Authorization expiry
            var expiredate = string.Empty;
            //verification typeofproduct
            var plantype = string.Empty;

            for (var x = 0; x < insurance.fldNames.Length; x++)
            {
                var attribute = insurance.fldNames[x];
                switch (attribute)
                {
                    case "insplanid":
                        insplanid = insurance.values[x];
                        break;
                    case "policyno":
                        policyno = insurance.values[x];
                        break;
                    case "grpno":
                        grpno = insurance.values[x];
                        break;
                    case "insrel":
                        insrel = insurance.values[x];
                        break;
                    case "inscoaddr":
                        inscoaddr = insurance.values[x];
                        break;
                    case "inscoaddrcity":
                        inscoaddrcity = insurance.values[x];
                        break;
                    case "inscoaddrstate":
                        inscoaddrstate = AddressBroker.StateWith(CurrentFacility.Oid,insurance.values[x]);
                        break;
                    case "inscoaddrzip":
                        inscoaddrzip = new ZipCode(insurance.values[x]);
                        break;
                    case "inscophone":
                        inscoPhone = insurance.values[x];
                        break;
                    case "effectdate":
                        effectdate = insurance.values[x];
                        break;
                    case "expiredate":
                        expiredate = insurance.values[x];
                        break;
                    case "plantype":
                        plantype = insurance.values[x];
                        break;
                }
            }
            if (!String.IsNullOrEmpty(insplanid) && insplanid != PRE_MSE_INSURANCE_PLAN_ID && insplanid != InsurancePlan.QUICK_ACCOUNTS_DEFAULT_INSURANCE_PLAN_ID)
            {
                var currentPlan = InsuranceBroker.PlanWith(insplanid, CurrentFacility.Oid);
                var currentInsured =  new Insured();
                if (currentPlan != null)
                {
                    var insuredRelationshipType = RelationshipTypeBroker.RelationshipTypeWith(CurrentFacility.Oid,
                                                                                              insrel);
                    var aRelationship = new Relationship(insuredRelationshipType, patient.Patient.GetType(),
                                                         currentInsured.GetType());
                    currentInsured.AddRelationship(aRelationship);

                    var currentCoverage = Coverage.CoverageFor(currentPlan, currentInsured);
                    if (insurance.attrCode == PRIMARYINSURANCE)
                    {
                        currentCoverage.CoverageOrder =
                            new CoverageOrder(CoverageOrder.PRIMARY_OID,
                                              CoverageOrder.NewPrimaryCoverageOrder().Description);
                    }
                    if (insurance.attrCode == SECONDARYINSURANCE)
                    {
                        currentCoverage.CoverageOrder =
                            new CoverageOrder(CoverageOrder.SECONDARY_OID,
                                              CoverageOrder.NewSecondaryCoverageOrder().Description);
                    }

                    if (currentCoverage.GetType() == typeof(GovernmentMedicaidCoverage))
                    {
                        ((GovernmentMedicaidCoverage)currentCoverage).PolicyCINNumber = policyno;
                    }
                    else if (currentCoverage.GetType() == typeof(WorkersCompensationCoverage))
                    {
                        ((WorkersCompensationCoverage)currentCoverage).PolicyNumber = policyno;
                    }
                    else if (currentCoverage.GetType() == typeof(GovernmentMedicareCoverage))
                    {
                        ((GovernmentMedicareCoverage)currentCoverage).MBINumber = policyno;
                    }
                    else if (currentCoverage.GetType() == typeof(CommercialCoverage) ||
                             currentCoverage.GetType() == typeof(OtherCoverage) ||
                             currentCoverage.GetType() == typeof(GovernmentOtherCoverage))
                    {

                        ((CoverageForCommercialOther)currentCoverage).GroupNumber = grpno;
                        if (!string.IsNullOrEmpty(policyno))
                        {
                            bool isValidMBINumber = ValidateMBIForEMPI(policyno);
                            if (isValidMBINumber)
                                ((CoverageForCommercialOther)currentCoverage).MBINumber = policyno;
                            else
                                ((CoverageForCommercialOther)currentCoverage).CertSSNID = policyno;
                        }
                    }
               
                    currentCoverage.BillingInformation.Address.Address1 = inscoaddr;
                    currentCoverage.BillingInformation.Address.City = inscoaddrcity;
                    currentCoverage.BillingInformation.Address.State = inscoaddrstate;
                    currentCoverage.BillingInformation.Address.ZipCode = inscoaddrzip;

                    currentCoverage.BillingInformation.PhoneNumber =
                        new PhoneNumber(StringFilter.RemoveFirstCharNonNumberAndRestNonNumber(inscoPhone));

                    if (currentCoverage.GetType() == typeof(CommercialCoverage))
                    {
                        var typeOfProduct = new TypeOfProduct { Description = plantype };
                        ((CommercialCoverage)currentCoverage).TypeOfProduct = typeOfProduct;
                    }

                    patient.Insurance.AddCoverage(currentCoverage);
                    if (insurance.attrCode == PRIMARYINSURANCE)
                    {
                        patient.EMPIPrimaryInvalidPlanID = string.Empty;
                    }
                    if (insurance.attrCode == SECONDARYINSURANCE)
                    {
                        patient.EMPISecondaryInvalidPlanID = string.Empty;
                    }
                }

                else
                {
                    if (insurance.attrCode == PRIMARYINSURANCE)
                    {
                        patient.EMPIPrimaryInvalidPlanID = insplanid;
                        if (patient.Insurance.PrimaryCoverage != null)
                        {
                            patient.Insurance.RemovePrimaryCoverage();
                        }
                    }
                    if (insurance.attrCode == SECONDARYINSURANCE)
                    {
                        patient.EMPISecondaryInvalidPlanID = insplanid;
                        if (patient.Insurance.SecondaryCoverage != null)
                        {
                            patient.Insurance.RemoveSecondaryCoverage();
                        }
                    }
                }
            }
            return patient.Insurance;
        }

        private EmergencyContact BuildPatientNextOfKin(Patient patient, MemIdsWs nok)
        {
            var contact1 = new EmergencyContact();
            var nokName = string.Empty;
            var relationshipCode = string.Empty;
            var phone = string.Empty;
            for (var x = 0; x < nok.fldNames.Length; x++)
            {
                var attribute = nok.fldNames[x];
                switch (attribute)
                {
                    case "lastname":
                        nokName = nok.values[x];
                        break;
                    case "relationship":
                        relationshipCode = nok.values[x].Trim();
                        break;
                    case "phnumber":
                        phone = nok.values[x];
                        break;
                }
            }
            string contactName = StringFilter.
                RemoveFirstCharNonLetterAndRestNonLetterSpaceHyphenAndComma(nokName.TrimEnd());

            contact1.Name = contactName;

            RelationshipType aRelationshipType =
                RelationshipTypeBroker.RelationshipTypeWith(CurrentFacility.Oid,
                                                            relationshipCode);

            if (aRelationshipType != null)
            {
                var aRelationship = new Relationship(aRelationshipType, patient.GetType(),
                                                     contact1.GetType());

                contact1.RemoveRelationship(aRelationshipType);
                contact1.AddRelationship(aRelationship);
                contact1.RelationshipType = aRelationshipType;

                var cp = new ContactPoint();
                var cps = (ArrayList) contact1.ContactPoints;
                // set the phone number on the contact point;
                if (cps.Count == 0)
                {
                    contact1.AddContactPoint(cp);
                }
                else
                {
                    cp = (ContactPoint) cps[0];
                }
                cp.TypeOfContactPoint = TypeOfContactPoint.NewPhysicalContactPointType();
                cp.PhoneNumber =
                    new PhoneNumber(StringFilter.RemoveFirstCharNonNumberAndRestNonNumber(phone));
            }
            patient.RemoveRelationship(contact1.RelationshipType);
            var anRelationship = new Relationship(contact1.RelationshipType, patient.GetType(), contact1.GetType());
            patient.AddRelationship(anRelationship);
            return contact1;
        }

        private static DriversLicense BuildDriversLicense(MemIdsWs driversLic, long facilityID)
        {
            var issuer = string.Empty;
            var licnumber = string.Empty;
            for (var x = 0; x < driversLic.fldNames.Length; x++)
            {
                var attribute = driversLic.fldNames[x];
                switch (attribute)
                {
                    case "issuer":
                        issuer = driversLic.values[x];
                        break;
                    case "licnumber":
                        licnumber = driversLic.values[x];
                        break;
                }
            }
            var issuerState = AddressBroker.StateWith(facilityID,issuer);
            return  new DriversLicense(licnumber, issuerState);
        }

        private static string BuildPatientMaidenName(MemNameWs maidenName)
        {
            var mName =
                new Name(maidenName.onmFirst, maidenName.onmLast, maidenName.onmMiddle).ToString();
            var patientMaidenName = StringFilter.RemoveFirstCharNonLetterAndRestNonLetterSpaceHyphenAndComma(mName);
            return patientMaidenName;
        }

        private static Name BuildPatientAlias(MemNameWs alias)
        {
            var aliasName = new Name(alias.onmFirst, alias.onmLast, alias.onmMiddle);
            return aliasName;
        }

        private static Name BuildPatientName(MemNameWs name)
        {
            return new Name(name.onmFirst, name.onmLast, name.onmMiddle, string.Empty, TypeOfName.Normal);
        }

        private static SocialSecurityNumber BuildPatientSSN(MemIdentWs ssn)
        {
            return new SocialSecurityNumber(ssn.idNumber.ToString(CultureInfo.InvariantCulture));
        }

        private static DateTime BuildPatientDOB(MemDateWs dob)
        {
            return DateTime.Parse(dob.dateVal);
        }

        private Address BuildPatientEmployerAddress(MemAddrWs employerAddress)
        {
            var zipCode = new ZipCode(employerAddress.zipCode);
            var state = AddressBroker.StateWith(CurrentFacility.Oid,employerAddress.state);
            var country = AddressBroker.CountryWith(employerAddress.country, CurrentFacility);
            var address1 = employerAddress.stLine1 + employerAddress.stLine2 + employerAddress.stLine3 +
                           employerAddress.stLine4;
            var address = new Address(address1, string.Empty, employerAddress.city,
                zipCode, state, country);
            return address;
        }

        private Address BuildPatientPhysicalAddress(MemAddrWs physicalAddress)
        {
            var zipCode = new ZipCode(physicalAddress.zipCode);
            var state = AddressBroker.StateWith(CurrentFacility.Oid,physicalAddress.state);
            var country = AddressBroker.CountryWith(physicalAddress.country, CurrentFacility);
            var address1 = physicalAddress.stLine1 + physicalAddress.stLine2 + physicalAddress.stLine3 +
                           physicalAddress.stLine4;
            var address = new Address(address1, string.Empty, physicalAddress.city,
                zipCode, state, country);
            return address;
        }

        private Address BuildPatientMailingAddress(MemAddrWs mailingAddress)
        {
            var zipCode = new ZipCode(mailingAddress.zipCode);
            var state = AddressBroker.StateWith(CurrentFacility.Oid,mailingAddress.state);
            var country = AddressBroker.CountryWith(mailingAddress.country, CurrentFacility);
            var address1 = mailingAddress.stLine1 + mailingAddress.stLine2 + mailingAddress.stLine3 +
                           mailingAddress.stLine4;
            var address = new Address(address1, string.Empty, mailingAddress.city,
                zipCode, state, country);
            return address;
        }

        private static Employment BuildEmployerName(MemAttrWs employerName)
        {
            var employment = new Employment { Employer = new Employer { Name = employerName.attrVal.Trim() } };
            return employment;
        }

        private static MedicalGroupIPA BuildMedicalGroupIpa(MemAttrWs ipa)
        {
            return new MedicalGroupIPA { Code = ipa.attrVal.Trim() };
        }

        private Ethnicity BuildPatientEthnicity(MemAttrWs ethnicity)
        {
            return OriginBroker.EthnicityWith(CurrentFacility.Oid, ethnicity.attrVal);
        }

        private MaritalStatus BuildPatientMaritalStatus(MemAttrWs maritalStatus)
        {
            return DemographicsBroker.MaritalStatusWith(CurrentFacility.Oid, maritalStatus.attrVal);
        }

        private Race BuildPatientRace(MemAttrWs race)
        {
            return OriginBroker.RaceWith(CurrentFacility.Oid, race.attrVal);
        }

        private Religion BuildPatientReligion(MemAttrWs religion)
        {
            return ReligionBroker.ReligionWith(CurrentFacility.Oid, religion.attrVal);
        }

        private Gender BuildPatientGender(MemAttrWs gender)
        {
            return DemographicsBroker.GenderWith(CurrentFacility.Oid, gender.attrVal);
        }

        internal List<PatientSearchResult> BuildPatientsSearchResultsFrom(IEnumerable<Member> patientSearchResponse)
        {

            var patientSearchHashResults = new Dictionary<Tuple<long, DateTime> , PatientSearchResult>();
            foreach (var member in patientSearchResponse)
            {
                if (member != null)
                {
                    long eMRN = ExtractMedicalRecordNumber(member);
                    var eHSPCode = ExtractFacilityCode(member);

                    var eName = ExtractName(member);
                    var eSSN = ExtractSocialSecurityNumber(member);
                    var eAliasNames = ExtractAliasName(member);
                    var eAddress = ExtractAddress(member);
                    var eDateOfBirth = ExtractDateOfBirth(member);
                    var eGender = ExtractGender(member);
                    var eScore = member.memHead.matchScore;
                    //var entRecNo = ExtractEMPIEntRecNumber(member);
                    var entRecNos = ExtractEMPIEntRecNumbersItem(member);
                    var entRecNoTime = ExtractEMPIEntRecNumbersTime(member);

                    Tuple<long, DateTime> entRecNo = new Tuple<long, DateTime>( entRecNos , entRecNoTime);

                    var patient = new PatientSearchResult();

                    if (patientSearchHashResults.Keys.Any(k => k.Item1 == entRecNo.Item1))
                    {
                        var founditem = patientSearchHashResults.Where(x => x.Key.Item1 == entRecNo.Item1).FirstOrDefault();
                        patient = patientSearchHashResults[founditem.Key];
                       
                        int comparevalue = DateTime.Compare(founditem.Key.Item2, entRecNo.Item2);
                        if (comparevalue > 0) //if old item modified date is latest then assign MRN and HospCode from memhead
                        {
                            var oldmember = patientSearchResponse.Where(o => o.memHead.entRecno == founditem.Key.Item1 && DateTime.Parse(o.memHead.recMtime) == founditem.Key.Item2).FirstOrDefault();
                            eMRN = ExtractEMPIMEMHeadMRN(oldmember);
                            eHSPCode = ExtractEMPIMEMHeadHospCode(oldmember);
                            entRecNo = founditem.Key;
                        }
                        else if(comparevalue == 0) // Both old and new item modified date is same then assign latest item MRN and HospCode from memhead
                        {
                            eMRN = ExtractEMPIMEMHeadMRN(member);
                            eHSPCode = ExtractEMPIMEMHeadHospCode(member);
                        }
                        else // if new item modified date is latest then assign MRN and HospCode from new item memhead
                        {
                            eMRN = ExtractEMPIMEMHeadMRN(member);
                            eHSPCode = ExtractEMPIMEMHeadHospCode(member);
                            patientSearchHashResults.RenameKey(founditem.Key, entRecNo);
                        }
                    }
                    var highestEMPIScore = (short)patient.EMPIScore;
                    eScore = eScore < highestEMPIScore ? highestEMPIScore : eScore;

                    patient = BuildPatientSearchResult(patient, eScore, eMRN, eHSPCode,
                                                        eName, eAliasNames, eSSN, eAddress, eDateOfBirth, eGender);
                    
                    patientSearchHashResults[entRecNo] = patient;
                    
                }
            }
            return patientSearchHashResults.Values.ToList();
        }

        private static PatientSearchResult BuildPatientSearchResult(PatientSearchResult patient, short eScore, long eMRN, string eHSPCode,
            Name eName, List<Name> eAliasNames, string eSSN, Address eAddress, DateTime eDateOfBirth, Gender eGender)
        {
            patient.EMPIScore = eScore;
            if (eMRN != 0)
            {
                patient.MedicalRecordNumber = eMRN;
            }
            if (!String.IsNullOrEmpty(eHSPCode))
            {
                patient.HspCode = eHSPCode;
            }
            if (eName != null)
            {
                patient.Name = new Name(PersistentModel.NEW_OID, PersistentModel.NEW_VERSION, eName.FirstName,
                                        eName.LastName, eName.MiddleInitial);
            }
            if (eAliasNames.Count > 0)
            {
                patient.AkaNames = eAliasNames;
            }
            if (!String.IsNullOrEmpty(eSSN))
            {
                patient.SocialSecurityNumber =
                    new SocialSecurityNumber(eSSN).DisplayString;
            }
            if (eAddress != null)
            {
                patient.Address = eAddress;
            }
            if (eDateOfBirth != DateTime.MinValue)
            {
                patient.DateOfBirth = eDateOfBirth;
            }
            if (eGender != null)
            {
                patient.Gender = eGender;
            }
            return patient;
        }

        private Gender ExtractGender(Member member)
        {
            var dictionaryEntry = new Dictionary<string, MemAttrWs>();
            Gender empiGender = null;
            if (member.memAttr != null)
            {
                foreach (var attribute in member.memAttr)
                {
                    dictionaryEntry.Add(attribute.attrCode, attribute);
                }

                if (dictionaryEntry.ContainsKey(PATIENTGENDER))
                {
                    var gender = dictionaryEntry[PATIENTGENDER];
                    if (gender != null)
                    {
                        empiGender = DemographicsBroker.GenderWith(CurrentFacility.Oid,
                            gender.attrVal);
                    }
                }
            }
            return empiGender;
        }
    

        private static DateTime ExtractDateOfBirth(Member member)
        {
            var dictionaryEntry = new Dictionary<string, MemDateWs>();
            var empiDateOfBirth = DateTime.MinValue;
            if (member.memDate != null)
            {
                foreach (var attribute in member.memDate)
                {
                    dictionaryEntry.Add(attribute.attrCode, attribute);
                }

                if (dictionaryEntry.ContainsKey(DATEOFBIRTH))
                {
                    var dob = dictionaryEntry[DATEOFBIRTH];
                    if (dob != null)
                    {
                        empiDateOfBirth = DateTime.Parse(dob.dateVal);
                    }
                }
            }
            return empiDateOfBirth;
        }

        private Address ExtractAddress(Member member)
        {
            var dictionaryEntry = new Dictionary<string, MemAddrWs>();
            Address empiAddress = null;
            if (member.memAddr != null)
            {
                var empiMailingAddress = new Address();
                var empiPhysicalAddress = new Address();
                foreach (var attribute in member.memAddr)
                {
                    dictionaryEntry.Add(attribute.attrCode, attribute);
                }

                if (dictionaryEntry.ContainsKey(MAILADDRESS))
                {
                    var mailingAddress = dictionaryEntry[MAILADDRESS];
                    if (mailingAddress != null)
                    {
                        var zipCode = new ZipCode(mailingAddress.zipCode);
                        var state = AddressBroker.StateWith(CurrentFacility.Oid,mailingAddress.state);
                        var country = AddressBroker.CountryWith(mailingAddress.country, CurrentFacility);
                        var address1 = mailingAddress.stLine1 + mailingAddress.stLine2 + mailingAddress.stLine3 +
                                       mailingAddress.stLine4;
                        empiMailingAddress = new Address(address1, string.Empty, mailingAddress.city,
                                                         zipCode, state, country);
                    }
                }

                if (dictionaryEntry.ContainsKey(PHYSICALADDRESS))
                {
                    var physicalAddress = dictionaryEntry[PHYSICALADDRESS];
                    if (physicalAddress != null)
                    {
                        var zipCode = new ZipCode(physicalAddress.zipCode);
                        var state = AddressBroker.StateWith(CurrentFacility.Oid,physicalAddress.state);
                        var country = AddressBroker.CountryWith(physicalAddress.country, CurrentFacility);
                        var address1 = physicalAddress.stLine1 + physicalAddress.stLine2 + physicalAddress.stLine3 +
                                       physicalAddress.stLine4;
                        empiPhysicalAddress = new Address(address1, string.Empty, physicalAddress.city,
                                                          zipCode, state, country);
                    }
                }
                if (!String.IsNullOrEmpty(empiPhysicalAddress.OneLineAddressLabel()))
                {
                    empiAddress = empiPhysicalAddress;
                }
                else if (!String.IsNullOrEmpty(empiMailingAddress.OneLineAddressLabel()))
                {
                    empiAddress = empiMailingAddress;
                }
            }
            return empiAddress;
        }

        private static Name ExtractName(Member member)
        {
            var dictionaryEntry = new Dictionary<string, MemNameWs>();
            Name eName = null;
            if (member.memName != null)
            {
                foreach (var attribute in member.memName)
                {
                    dictionaryEntry.Add(attribute.attrCode, attribute);
                }

                if (dictionaryEntry.ContainsKey(PATIENTNAME))
                {
                    var name =  dictionaryEntry[PATIENTNAME];
                    if (name != null)
                    {
                        eName = new Name(PersistentModel.NEW_OID, PersistentModel.NEW_VERSION, name.onmFirst,
                            name.onmLast, name.onmMiddle);
                    }
                }
            }
            return eName;
        }

        private static List<Name> ExtractAliasName(Member member)
        {
            var dictionaryEntry = new Dictionary<string, MemNameWs>();
           var empiAliasNames = new List<Name>();
           if (member.memName != null)
            {
                foreach (var attribute in member.memName)
                {
                    dictionaryEntry.Add(attribute.attrCode, attribute);
                }

                if (dictionaryEntry.ContainsKey(PATIENTALIAS))
                {
                    var alias = dictionaryEntry[PATIENTALIAS];
                    if (alias != null)
                    {
                        var empAliasName = new Name(alias.onmFirst, alias.onmLast, alias.onmMiddle);
                        empiAliasNames.Add(empAliasName);
                    }
                }

                if (dictionaryEntry.ContainsKey(MADIENNAME))
                {
                    var maidenName = dictionaryEntry[MADIENNAME];
                    if (maidenName != null)
                    {
                        var empiMaidenName = new Name(maidenName.onmFirst, maidenName.onmLast, maidenName.onmMiddle);
                        empiAliasNames.Add(empiMaidenName);
                    }
                }
            }
            return empiAliasNames;
        }

        private static string ExtractSocialSecurityNumber(Member member)
        {
            var dictionaryEntry = new Dictionary<string, MemIdentWs>();
            string socialSecurityNumber = null;
            if (member.memIdent != null)
            {
                foreach (var attribute in member.memIdent)
                {
                    dictionaryEntry.Add(attribute.attrCode, attribute);
                }

                if (dictionaryEntry.ContainsKey(SOCIALSECURITYNUMBER))
                {
                    var ssn = dictionaryEntry[SOCIALSECURITYNUMBER];
                    if (ssn != null)
                    {
                        socialSecurityNumber = new SocialSecurityNumber(ssn.idNumber).DisplayString;
                    }
                }
            }
            return socialSecurityNumber;
        }

        private static string ExtractFacilityCode(Member member )
        {
            var dictionaryEntry = new Dictionary<string, MemAttrWs>();
            var eHSPCode = String.Empty;
            if (member.memAttr != null)
            {
                foreach (var attribute in member.memAttr)
                {
                    dictionaryEntry.Add(attribute.attrCode, attribute);
                }
                if (dictionaryEntry.ContainsKey(PATIENTHOSPITALCODE))
                {
                    var hspCode = dictionaryEntry[PATIENTHOSPITALCODE];
                    if (hspCode != null)
                    {
                        eHSPCode = hspCode.attrVal;
                    }
                }
            }
            return eHSPCode;
        }

        private static long ExtractEMPIEntRecNumber(Member member)
        {
            var empiENTRECNO = 0;
            if (member.memHead != null)
            {
                if (member.memHead.entRecno != 0)
                {
                    empiENTRECNO = (int)member.memHead.entRecno;
                }
            }
            return empiENTRECNO;
        }

        private static long ExtractEMPIMEMHeadMRN(Member member)
        {
            long empimemIdnum = 0;
            if (member.memHead != null)
            {
                if (!String.IsNullOrEmpty(member.memHead.memIdnum))
                {
                    empimemIdnum = long.Parse(member.memHead.memIdnum);
                }
            }
            return empimemIdnum;
        }

        private static string ExtractEMPIMEMHeadHospCode(Member member)
        {
            var empisrcCode = string.Empty;
            if (member.memHead != null)
            {
                if (!String.IsNullOrEmpty(member.memHead.srcCode))
                {
                    empisrcCode = member.memHead.srcCode.Replace(PBARPAT,"");
                }
            }
            return empisrcCode;
        }


        private static long ExtractEMPIEntRecNumbersItem(Member member)
        {
            var empiENTRECNO = 0;
            if (member.memHead != null)
            {
                if (member.memHead.entRecnos[0] != 0)
                {
                    empiENTRECNO = (int)member.memHead.entRecnos[0];
                }
            }
            return empiENTRECNO;
        }
        private static DateTime ExtractEMPIEntRecNumbersTime(Member member)
        {
            var empirecMtime = DateTime.MinValue;
            if (member.memHead != null)
            {
                if (member.memHead.recMtime !=null )
                {
                    empirecMtime = DateTime.Parse(member.memHead.recMtime);
                }
            }
            return empirecMtime;
        }
        private static long ExtractMedicalRecordNumber(Member member)
        {
            var dictionaryEntry = new Dictionary<string, MemIdentWs>();
            long outMRN = 0;
            if (member.memIdent != null)
            {
                foreach (var attribute in member.memIdent)
                {
                    dictionaryEntry.Add(attribute.attrCode, attribute);
                }

                if (dictionaryEntry.ContainsKey(MEDICALRECORDNUMBER))
                {
                    var empiMRN = dictionaryEntry[MEDICALRECORDNUMBER];
                    if (empiMRN != null)
                    {
                        Int64.TryParse(empiMRN.idNumber, out outMRN);
                    }
                }
            }
            return outMRN;
        }

        public static bool ValidateMBIForEMPI(string MBINumber)
        {
            var mbiNumberEntered = MBINumber;
            if (String.IsNullOrEmpty(mbiNumberEntered))
            {
                return true;
            }
            if (mbiNumberEntered.Length != 11)
            {
                return false;
            }
            else
            {
                char[] mbiCharacters = mbiNumberEntered.ToCharArray();
                if (
                    !Regex.IsMatch(mbiCharacters[0].ToString(), REGEX_1To9) ||
                    !Regex.IsMatch(mbiCharacters[1].ToString(), REGEX_AToZ) ||
                    !Regex.IsMatch(mbiCharacters[2].ToString(), REGEX_0To9AToZ) ||
                    !Regex.IsMatch(mbiCharacters[3].ToString(), REGEX_0To9) ||
                    !Regex.IsMatch(mbiCharacters[4].ToString(), REGEX_AToZ) ||
                    !Regex.IsMatch(mbiCharacters[5].ToString(), REGEX_0To9AToZ) ||
                    !Regex.IsMatch(mbiCharacters[6].ToString(), REGEX_0To9) ||
                    !Regex.IsMatch(mbiCharacters[7].ToString(), REGEX_AToZ) ||
                    !Regex.IsMatch(mbiCharacters[8].ToString(), REGEX_AToZ) ||
                    !Regex.IsMatch(mbiCharacters[9].ToString(), REGEX_0To9) ||
                    !Regex.IsMatch(mbiCharacters[10].ToString(), REGEX_0To9)
                    )
                {
                    return false;
                }
            }

           var TestMBI = Coverage.TestMBINumbers.Contains(mbiNumberEntered);
            if (TestMBI)
            {
                return false;
            }
            return true;
        }

       
        #endregion

        #region Private Properties

        public MemberSearchRequest SearchMemberRequest { get; private set; }

        public MemberGetRequest GetMemberRequest { get; private set; }

        private PatientSearchCriteria patientSearchCriteria { get; set; }

        #endregion

        #region Construction and Finalization

        public EMPIService(Facility facility, IPBARToEMPIFacilityNameMapper facilityMapper)
        {
            Guard.ThrowIfArgumentIsNull(facility, "Facility");
            Guard.ThrowIfArgumentIsNull(facilityMapper, "facilityMapper");

            identityHub = new IdentityHubPortClient(CONFIG_INDENTITYHUBSERVICE_ENTRY);
            CurrentFacility = facility;
            PbartoEMPIFacilityNameMapper = facilityMapper;
        }

        #endregion

        #region Data Elements
        
        private static readonly IDemographicsBroker DemographicsBroker = BrokerFactory.BrokerOfType<IDemographicsBroker>();
        private static readonly IOriginBroker OriginBroker = BrokerFactory.BrokerOfType<IOriginBroker>();
        private readonly Request request = new Request();
        private static readonly IAddressBroker AddressBroker = BrokerFactory.BrokerOfType<IAddressBroker>();
        private static readonly IReligionBroker ReligionBroker = BrokerFactory.BrokerOfType<IReligionBroker>();
        private static readonly IRelationshipTypeBroker RelationshipTypeBroker = BrokerFactory.BrokerOfType<IRelationshipTypeBroker>();
        private Facility currentFacility = new Facility();
        private readonly IdentityHubPortClient identityHub;
        private readonly static IInsuranceBroker InsuranceBroker = BrokerFactory.BrokerOfType<IInsuranceBroker>();
        private Facility CurrentFacility
        {
            get { return currentFacility; }
            set { currentFacility = value; }
        }

        private IPBARToEMPIFacilityNameMapper PbartoEMPIFacilityNameMapper { get; set; } 
        private static readonly ILog Logger = LogManager.GetLogger(typeof(EMPIService));
        PatientSearchResponse patientSearchResponse = new PatientSearchResponse();
        #endregion

        #region Constants

        private const string CONFIG_INDENTITYHUBSERVICE_ENTRY = "IdentityHub";
        private const string MEDICALRECORDNUMBER = "MRN";
        private const string SOCIALSECURITYNUMBER = "PATSSN";
        private const string DATEOFBIRTH = "PATDOB";
        private const string PATIENTNAME = "PATNAME";
        private const string PATIENTGENDER = "PATGENDER";
        private const string PATIENTHOSPITALCODE = "PATHSC";
        private const string PBARPAT = "PBARPAT";
        private const string PATIENTRELIGION = "RELIGION";
        private const string PATIENTRACE = "PATRACE";
        private const string PATIENTMARITALSTATUS = "MARITALSTAT";
        private const string PATIENTETHNICITY = "ETHNICITY";
        private const string IPACODE = "IPACODE";
        private const string EMPLOYER = "EMPLOYER";
        private const string MAILADDRESS = "MAILADDRESS";
        private const string PHYSICALADDRESS = "PHYADDRESS";
        private const string EMPLOYERADDRESS = "EMPADDRESS";
        private const string PATIENTALIAS = "PATALIAS";
        private const string MADIENNAME = "MAIDENAME";
        private const string DRIVERSLICENSE = "DRIVERSLIC";
        private const string NEXTOFKIN = "PATNOK";
        private const string GUARANTOR = "GUARANTOR";
        private const string PRIMARYINSURANCE = "PRIMARYINS";
        private const string SECONDARYINSURANCE = "SECONDARYINS";
        private const string MAILINGPHONE = "HOMEPHONE";
        private const string CELLPHONE = "MOBILEPHONE";
        private const string PHYSICALPHONE = "PHYPHONE";
        private const string EMPISERVICE_USERNAME = "EMPISERVICE_USERNAME";
        private const string EMPISERVICE_PASSWORD = "EMPISERVICE_PASSWORD";
        private const string PRE_MSE_INSURANCE_PLAN_ID = "EDL81";
        private const string REGEX_0To9 = "[0-9]";
        private const string REGEX_1To9 = "[1-9]";
        private const string REGEX_AToZ = "[AC-HJKMNPQRT-Yac-hjkmnpqrt-y]";
        private const string REGEX_0To9AToZ = "[0-9AC-HJKMNPQRT-Yac-hjkmnpqrt-y]";
        #endregion
    }
}

public static class DicExtensions
{
    public static void RenameKey<TKey, TValue>(this IDictionary<TKey, TValue> dic,
                                      TKey fromKey, TKey toKey)
    {
        TValue value = dic[fromKey];
        dic.Remove(fromKey);
        dic[toKey] = value;
    }
}

