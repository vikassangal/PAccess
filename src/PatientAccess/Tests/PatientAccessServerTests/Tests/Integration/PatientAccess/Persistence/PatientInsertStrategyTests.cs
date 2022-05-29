using System;
using System.Collections;
using Extensions.PersistenceCommon;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Domain.ShortRegistration;
using PatientAccess.Persistence;
using PatientAccess.Persistence.Factories;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    [TestFixture]
    public class PatientInsertStrategyTests : AbstractBrokerTests
    {
        #region SetUp and TearDown PatientInsertStrategyTests

        [TestFixtureSetUp]
        public static void SetUpPatientInsertStrategyTests()
        {
            CreateUser();

            IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();

            facility = facilityBroker.FacilityWith( FACILITY_CODE );
            facility2 = facilityBroker.FacilityWith( FACILITY_CODE2 );
        }

        [TestFixtureTearDown]
        public static void TearDownPatientInsertStrategyTests()
        {

        }
        #endregion

        #region Test Methods

        [Test]
        public void TestBuildSqlFromForPreRegistration()
        {
            Account anAccount = new Account();
            anAccount.AccountNumber = 23456789;
            anAccount.AdmitDate = DateTime.Parse( "Dec 17, 2001" );
            anAccount.ClergyVisit.SetYes( "YES" );
            anAccount.DischargeDate = DateTime.Parse( "Jan 21, 2002" );
            anAccount.DerivedVisitType = "OUTP";

            VisitType visitType = new VisitType(
                PersistentModel.NEW_OID, PersistentModel.NEW_VERSION, VisitType.PREREG_PATIENT_DESC, VisitType.PREREG_PATIENT );
            anAccount.KindOfVisit = visitType;

            Patient patient = new Patient(
                PATIENT_OID,
                PersistentModel.NEW_VERSION,
                PATIENT_NAME,
                PATIENT_MRN,
                PATIENT_DOB,
                PATIENT_SSN,
                PATIENT_SEX,
                facility2
                );

            ContactPoint contactPointE = new ContactPoint(
                addressE, phoneNumberE, null, typeOfContactPointE );
            ContactPoint contactPointM = new ContactPoint(
                addressMP, phoneNumberM, emailAddress, typeOfContactPointM );
            ContactPoint contactPointP = new ContactPoint(
                addressMP, phoneNumberP, null, typeOfContactPointP );

            anAccount.HospitalService = hsp;
            patient.PlaceOfWorship = placeOfWorship;
            patient.Race = race;
            patient.Race2 = race2;
            patient.Nationality = nationality;
            patient.Nationality2 = nationality2;
            patient.Ethnicity = ethnicity;
            patient.Ethnicity2 = ethnicity2;
            patient.Descent = descent;
            patient.Descent2 = descent2;
            patient.MaritalStatus = maritalStatus;
            patient.Language = language;
            patient.Religion = religion;
            patient.AddContactPoint( contactPointE );
            patient.AddContactPoint( contactPointM );
            patient.AddContactPoint( contactPointP );

            employment = new Employment( patient );
            Employer patEmpr = new Employer();
            employment.Employer = patEmpr;
            patEmpr.Name = "AMERICAN EXPRESS";
            patEmpr.AddContactPoint( contactPointM );

            anAccount.Patient = patient;
            anAccount.Facility = facility2;

            anAccount.Patient.Employment = new Employment( patient );
            anAccount.Patient.DriversLicense = new DriversLicense(
                "72368223828111111",
                new State( PersistentModel.NEW_OID, DateTime.Now, "Texas", "TX" ) );

            Assert.AreEqual(
                "72368223828111111",
                anAccount.Patient.DriversLicense.Number );

            Assert.AreEqual(
                "YES",
                anAccount.ClergyVisit.Description );
            anAccount.FinancialClass =
                new FinancialClass( 279,
                ReferenceValue.NEW_VERSION,
                "COMMERCIAL INS",
                "20" );
            CoverageOrder primary = new CoverageOrder( 1, "Primary" );
            Assert.AreEqual(
                "Primary",
                primary.Description );

            CoverageOrder secondary = new CoverageOrder( 2, "Secondary" );
            Assert.AreEqual(
                "Secondary",
                secondary.Description );

            CommercialCoverage coverage1 = new CommercialCoverage();
            coverage1.CoverageOrder = primary;
            coverage1.Oid = 1;

            GovernmentMedicareCoverage coverage2 = new GovernmentMedicareCoverage();
            coverage2.CoverageOrder = secondary;
            coverage2.Oid = 2;

            Insurance insurance = new Insurance();
            insurance.AddCoverage( coverage1 );
            Assert.AreEqual(
                1,
                insurance.Coverages.Count );

            insurance.AddCoverage( coverage2 );
            Assert.AreEqual(
                2,
                insurance.Coverages.Count );

            anAccount.MedicalGroupIPA = new MedicalGroupIPA();
            anAccount.MedicalGroupIPA.Code = TEST_IPA;
            anAccount.MedicalGroupIPA.AddClinic(
                new Clinic( 0, ReferenceValue.NEW_VERSION,
                    TEST_IPA_CLINIC, TEST_IPA_CLINIC ) );

            anAccount.DischargeDate = DateTime.MinValue;
            anAccount.Insurance = insurance;
            Insured insured = new Insured();
            Employment emp = new Employment( insured );
            emp.EmployeeID = "123456";
            emp.Status = new EmploymentStatus(
                PersistentModel.NEW_OID, DateTime.Now, "EMPLOYED FULL TIME", "1" );

            Employer empr = new Employer();
            empr.Name = "qwert";
            emp.Employer = empr;

            emp.Employer.PartyContactPoint = contactPointE;
            patient.Employment = emp;

            EmergencyContact ec = new EmergencyContact();
            ec.Name = "Nobody";
            ec.AddContactPoint( contactPointM );
            ec.AddContactPoint( contactPointP );

            RelationshipType relType = new RelationshipType(
                PersistentModel.NEW_OID, DateTime.Now, "Spouse", "2" );

            ec.RelationshipType = relType;

            anAccount.EmergencyContact1 = ec;
            anAccount.EmergencyContact2 = ec;
            anAccount.ValuablesAreTaken.SetYes();

            Assert.AreEqual(
                "Nobody",
                anAccount.EmergencyContact1.Name );

            Assert.AreEqual( relType.Code, "2" );

            Diagnosis diagnosis = new Diagnosis();

            Accident accident = new Accident();
            accident.OccurredOn = DateTime.Parse( "21 Jan, 2005" );
            accident.OccurredAtHour = "11:00";
            accident.Country = new Country( PersistentModel.NEW_OID, DateTime.Now, "USA" );
            accident.State = new State( PersistentModel.NEW_OID, DateTime.Now, "Texas" );
            accident.Kind = new TypeOfAccident(
                PersistentModel.NEW_OID, DateTime.Now, "Auto", "2" );
            diagnosis.Condition = accident;
            anAccount.Diagnosis = diagnosis;
            Accident acdt = (Accident)anAccount.Diagnosis.Condition;
            Assert.AreEqual(
                21,
                acdt.OccurredOn.Day );
            Assert.AreEqual(
                "11:00",
                acdt.OccurredAtHour );
            Assert.AreEqual(
                "USA",
                acdt.Country.Description );
            Assert.AreEqual(
                "Texas",
                acdt.State.Description );
            Assert.AreEqual(
                "Auto",
                acdt.Kind.Description );

            Assert.AreEqual(
                typeof( Accident ),
                anAccount.Diagnosis.Condition.GetType()
                );

            Pregnancy pregnancy = new Pregnancy();
            pregnancy.LastPeriod = DateTime.Parse( "Jan 21, 2005" );
            diagnosis.Condition = pregnancy;
            anAccount.Diagnosis = diagnosis;
            Pregnancy preg = (Pregnancy)anAccount.Diagnosis.Condition;
            Assert.AreEqual(
                21,
                preg.LastPeriod.Day );

            Assert.AreEqual(
                typeof( Pregnancy ),
                anAccount.Diagnosis.Condition.GetType()
                );

            Occ[0] = new OccurrenceCode(
                1, DateTime.Parse( "23, Nov 2001" ), "Accident_auto", "01" );
            ( (OccurrenceCode)Occ[0] ).OccurrenceDate = DateTime.Parse( "23, Nov 2001" );

            Occ[1] = new OccurrenceCode(
                19, DateTime.Parse( "26, Nov 2004" ), "Accident_other", "19" );
            ( (OccurrenceCode)Occ[1] ).OccurrenceDate = DateTime.Parse( "26, Nov 2004" );

            foreach ( OccurrenceCode occ in Occ )
            {
                anAccount.AddOccurrenceCode( occ );
            }

            Activity currentActivity =
                new PreRegistrationActivity();
            anAccount.Activity = currentActivity;
            anAccount.IsNew = true;

            anAccount.Patient.MothersDateOfBirth = new DateTime( 1945, 01, 01 );
            anAccount.Patient.FathersDateOfBirth = new DateTime( 1943, 02, 02 );
            anAccount.ClinicalComments = "TEST COMMENTS1 TEST COMMENTS1TEST COMMENTS1TEST COMMENTS1TEST COMMENTS1TEST COMMENTS1TEST COMMENTS1TEST COMMENTS1TES";
            anAccount.EmbosserCard = "1234567890";

            anAccount.CodedDiagnoses = new CodedDiagnoses();
            anAccount.CodedDiagnoses.CodedDiagnosises.Add( "789.58" );

            anAccount.ValueCode1 = "A";
            anAccount.ValueAmount1 = 1.23M;

            clinicCode.SiteCode = SITE_CODE;
            anAccount.Clinics[0] = clinicCode;

            anAccount.IsPatientInClinicalResearchStudy = YesNoFlag.Yes;

            ResearchStudy study1 = new ResearchStudy( "TEST001", "RESEARCH STUDY 1", "RESEARCH SPONSOR 1" );
            ResearchStudy study2 = new ResearchStudy( "TEST002", "RESEARCH STUDY 2", "RESEARCH SPONSOR 2" );
            ResearchStudy study3 = new ResearchStudy( "TEST003", "RESEARCH STUDY 3", "RESEARCH SPONSOR 3" );
            ResearchStudy study4 = new ResearchStudy( "TEST004", "RESEARCH STUDY 4", "RESEARCH SPONSOR 4" );
            ResearchStudy study5 = new ResearchStudy( "TEST005", "RESEARCH STUDY 5", "RESEARCH SPONSOR 5" );
            ResearchStudy study6 = new ResearchStudy( "TEST006", "RESEARCH STUDY 6", "RESEARCH SPONSOR 6" );
            ResearchStudy study7 = new ResearchStudy( "TEST007", "RESEARCH STUDY 7", "RESEARCH SPONSOR 7" );
            ResearchStudy study8 = new ResearchStudy( "TEST008", "RESEARCH STUDY 8", "RESEARCH SPONSOR 8" );
            ResearchStudy study9 = new ResearchStudy( "TEST009", "RESEARCH STUDY 9", "RESEARCH SPONSOR 9" );

            anAccount.AddConsentedResearchStudy( new ConsentedResearchStudy( study1, YesNoFlag.Yes ) );
            anAccount.AddConsentedResearchStudy( new ConsentedResearchStudy( study2, YesNoFlag.Yes ) );
            anAccount.AddConsentedResearchStudy( new ConsentedResearchStudy( study3, YesNoFlag.Yes ) );
            anAccount.AddConsentedResearchStudy( new ConsentedResearchStudy( study4, YesNoFlag.Yes ) );
            anAccount.AddConsentedResearchStudy( new ConsentedResearchStudy( study5, YesNoFlag.Yes ) );
            anAccount.AddConsentedResearchStudy( new ConsentedResearchStudy( study6, YesNoFlag.No ) );
            anAccount.AddConsentedResearchStudy( new ConsentedResearchStudy( study7, YesNoFlag.No ) );
            anAccount.AddConsentedResearchStudy( new ConsentedResearchStudy( study8, YesNoFlag.No ) );
            anAccount.AddConsentedResearchStudy( new ConsentedResearchStudy( study9, YesNoFlag.No ) );

            i_PatientInsertStrategy = SqlBuilderStrategyFactory.CreatePatientInsertStrategy();
            i_PatientInsertStrategy.TransactionFileId = "PC";
            i_PatientInsertStrategy.UserSecurityCode = "KEVN";

            NoticeOfPrivacyPracticeDocument nppDocument = new NoticeOfPrivacyPracticeDocument();
            nppDocument.SignatureStatus = new SignatureStatus( "S" );
            nppDocument.SignedOnDate = new DateTime( 2001, 12, 17 );

            INPPVersionBroker nppVersionBroker = BrokerFactory.BrokerOfType<INPPVersionBroker>();
            nppDocument.NPPVersion = nppVersionBroker.NPPVersionWith( FACILITY_ID, "10" );

            anAccount.Patient.NoticeOfPrivacyPracticeDocument = nppDocument;

            anAccount.AlternateCareFacility = "ABC";

            ArrayList sqlStrings =
                i_PatientInsertStrategy.BuildSqlFrom( anAccount, transactionKeys );
            string sqlString = sqlStrings[0] as string;

            string[] valueArray = ValueArray( sqlString );

            int i = 0;

            Assert.AreEqual( NUMBER_OF_ENTRIES, valueArray.Length, "Wrong number of entries in ValueArray" );

            Assert.AreEqual( " ''", valueArray[i], "Value of APIDWS should be ''" );
            Assert.AreEqual( "'PC'", valueArray[i=i+1], "Value of APIDID should be 'PC' " );
            Assert.AreEqual( "'$#G%&*'", valueArray[i=i+1], "Value of APGREC should be '$#G%&*' " );
            Assert.AreEqual( "'$#P@%&'", valueArray[i=i+1], "Value of APRR# should be '$#P@%&' " );
            Assert.AreEqual( "365", valueArray[i=i+1], "Value of APPAUX should be 365 " );
            Assert.AreEqual( "'KEVN'", valueArray[i=i+1], "Value of APSEC2 should be 'KEVN' " );
            Assert.AreEqual( "900", valueArray[i=i+1], "Value of APHSP# should be 900 " );
            Assert.AreEqual( "23456789", valueArray[i=i+2], "Value of APACCT should be 23456789 " );
            Assert.AreEqual( "'FRIED'", valueArray[i=i+1], "Value of APPLNM should be 'FRIED' " );
            Assert.AreEqual( "'CLAIRE R'", valueArray[i=i+1], "Value of APPFNM should be 'CLAIRE R' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APLAST should be '' " );
            Assert.AreEqual( "'F'", valueArray[i=i+1], "Value of APSEX should be 'F' " );
            Assert.AreEqual( "'1'", valueArray[i=i+1], "Value of APRACE should be '1' " );
            Assert.AreEqual( "'046Y'", valueArray[i=i+1], "Value of APAGE should be '046Y' " );
            Assert.AreEqual( "30555", valueArray[i=i+1], "Value of APDOB should be 30555 " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APADMC should be '' " );
            Assert.AreEqual( "121701", valueArray[i=i+1], "Value of APADMT should be 121701 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APADTM should be 0 " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APNS should be '' " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APROOM should be 0 " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APBED should be '' " );
            Assert.AreEqual( "'3'", valueArray[i=i+1], "Value of APETHC should be '3' " );
            Assert.AreEqual( "'0'", valueArray[i=i+1], "Value of APPTYP should be '0' " );
            Assert.AreEqual( "'99'", valueArray[i=i+1], "Value of APMSV should be '99' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APSMOK should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APDIET should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCOND should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APISO should be '' " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APADR# should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APRDR# should be 0 " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APPOB should be '' " );
            Assert.AreEqual( "'123121234'", valueArray[i=i+1], "Value of APSSN should be '123121234' " );
            Assert.AreEqual( "121701", valueArray[i=i+2], "Value of APLAD should be 121701 " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APLVLN should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APRLGN should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APALRG should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCOLG should be '' " );
            Assert.AreEqual("''", valueArray[i = i + 1], "Value of APPADR should be '' ");
            Assert.AreEqual( "'Austin'", valueArray[i=i+1], "Value of APPCIT should be 'Austin' " );
            Assert.AreEqual( "'TX'", valueArray[i=i+1], "Value of APPSTE should be 'TX' " );
            Assert.AreEqual( "60594", valueArray[i=i+1], "Value of APPZIP should be 60594 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APPZP4 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APPCNY should be 0 " );
            Assert.AreEqual( "972", valueArray[i=i+1], "Value of APPACD should be 972 " );
            Assert.AreEqual( "553453", valueArray[i=i+1], "Value of APPPH# should be 553453 " );
            Assert.AreEqual("''", valueArray[i = i + 1], "Value of APMADR should be '' ");
            Assert.AreEqual( "'Austin'", valueArray[i=i+1], "Value of APMCIT should be 'Austin' " );
            Assert.AreEqual( "'TX'", valueArray[i=i+1], "Value of APMSTE should be 'TX' " );
            Assert.AreEqual( "60594", valueArray[i=i+1], "Value of APMZIP should be 60594 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APMZP4 should be 0 " );
            Assert.AreEqual( "'123'", valueArray[i=i+1], "Value of APMCNY should be '123' " );
            Assert.AreEqual( "610", valueArray[i=i+1], "Value of APMACD should be 610 " );
            Assert.AreEqual( "546789", valueArray[i=i+1], "Value of APMPH# should be 546789 " );
            Assert.AreEqual( "'Nobody'", valueArray[i=i+1], "Value of APRNM should be 'Nobody' " );
            Assert.AreEqual( "'234 MulHolland Drive#1'", valueArray[i=i+1], "Value of APRADR should be '234 MulHolland Drive#1' " );
            Assert.AreEqual( "'Austin'", valueArray[i=i+1], "Value of APRCIT should be 'Austin' " );
            Assert.AreEqual( "'TX'", valueArray[i=i+1], "Value of APRSTE should be 'TX' " );
            Assert.AreEqual( "60594", valueArray[i=i+1], "Value of APRZIP should be 60594 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APRZP4 should be 0 " );
            Assert.AreEqual( "610", valueArray[i=i+1], "Value of APRACD should be 610 " );
            Assert.AreEqual( "546789", valueArray[i=i+1], "Value of APRPH# should be 546789 " );
            Assert.AreEqual( "'Nobody'", valueArray[i=i+1], "Value of APCNM should be 'Nobody' " );
            Assert.AreEqual( "'234 MulHolland Drive#1'", valueArray[i=i+1], "Value of APCADR should be '234 MulHolland Drive#1' " );
            Assert.AreEqual( "'Austin'", valueArray[i=i+1], "Value of APCCIT should be 'Austin' " );
            Assert.AreEqual( "'TX'", valueArray[i=i+1], "Value of APCSTE should be 'TX' " );
            Assert.AreEqual( "60594", valueArray[i=i+1], "Value of APCZIP should be 60594 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APCZP4 should be 0 " );
            Assert.AreEqual( "610", valueArray[i=i+1], "Value of APCACD should be 610 " );
            Assert.AreEqual( "546789", valueArray[i=i+1], "Value of APCPH# should be 546789 " );
            Assert.AreEqual( "'qwert'", valueArray[i=i+1], "Value of APWNM should be 'qwert' " );
            Assert.AreEqual( "'335 Nicholson Dr.#303'", valueArray[i=i+1], "Value of APWADR should be '335 Nicholson Dr.#303' " );
            Assert.AreEqual( "'Austin'", valueArray[i=i+1], "Value of APWCIT should be 'Austin' " );
            Assert.AreEqual( "'TX'", valueArray[i=i+1], "Value of APWSTE should be 'TX' " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APWZIP should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APWZP4 should be 0 " );
            Assert.AreEqual( "972", valueArray[i=i+1], "Value of APWACD should be 972 " );
            Assert.AreEqual( "546789", valueArray[i=i+1], "Value of APWPH# should be 546789 " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APHEMP should be '' " );
            Assert.AreEqual( "'1'", valueArray[i=i+1], "Value of APMAR should be '1' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APMNM should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APMOTH should be '' " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APMPT# should be 0 " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APSNM should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APSADR should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APSCIT should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APSSTE should be '' " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APSZIP should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APSZP4 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APSACD should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APSPH# should be 0 " );
            Assert.AreEqual( "'789.58'", valueArray[i=i+1], "Value of APCD01 should be '789.58' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCD02 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCD03 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCD04 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCD05 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCD06 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCD07 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCD08 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APDIAG should be '' " );
            Assert.AreEqual( "'09123'", valueArray[i=i+1], "Value of APCNTY should be '09123' " );
            Assert.AreEqual( "'TEST COMMENTS1 TEST COMMENTS1TEST COMMENTS1TEST COMMENTS1TEST COM'", valueArray[i=i+1], "Value of APCOM1 should be 'TEST COMMENTS1 TEST COMMENTS1TEST COMMENTS1TEST COMMENTS1TEST COM' " );
            Assert.AreEqual( "'MENTS1TEST COMMENTS1TEST COMMENTS1TEST COMMENTS1TES    1234567890'", valueArray[i=i+1], "Value of APCOM2 should be 'MENTS1TEST COMMENTS1TEST COMMENTS1TEST COMMENTS1TES    1234567890' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APTCRT should be '' " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APDCRT should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APSCRT should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APNCRT should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APICRT should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of AP#EXT should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APDTOT should be 0 " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCLOC should be '' " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APPMRC should be 0 " );
            Assert.AreEqual( "23456789", valueArray[i=i+1], "Value of APGAR# should be 23456789 " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APHOWA should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APPOLN should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCORC should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APALOC should be '' " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APATIM should be 0 " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APPST should be '' " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APDISC should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APDEDT should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APLVD should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APLAT should be 0 " );
            Assert.AreEqual( "'3'", valueArray[i=i+1], "Value of APTREG should be '3' " );
            Assert.AreEqual( "'AL'", valueArray[i=i+1], "Value of APCL01 should be 'AL' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCL02 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCL03 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCL04 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCL05 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCL06 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCL07 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCL08 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCL09 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCL10 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCL11 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCL12 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCL13 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCL14 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCL15 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCL16 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCL17 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCL18 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCL19 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCL20 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCL21 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCL22 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCL23 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCL24 should be '' " );
            Assert.AreEqual( "'SITE_CODE'", valueArray[i=i+1], "Value of APCL25 should be 'SITE_CODE' " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APLDD should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APLDT should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+2], "Value of APKACT should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APPGAR should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APAL01 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APAL02 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APAL03 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APAL04 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APAL05 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APAL06 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APAL07 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APAL08 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APAL09 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APAL10 should be 0 " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APPSRC should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APPSTS should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCI01 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCI02 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCI03 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCI04 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCI05 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APSPGM should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APAPVL should be '' " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APAPFR should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APAPTO should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APGRCD should be 0 " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APTACD should be '' " );
            Assert.AreEqual( "'01'", valueArray[i=i+1], "Value of APOC01 should be '01' " );
            Assert.AreEqual( "'19'", valueArray[i=i+1], "Value of APOC02 should be '19' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APOC03 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APOC04 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APOC05 should be '' " );
            Assert.AreEqual( "112301", valueArray[i=i+1], "Value of APOA01 should be 112301 " );
            Assert.AreEqual( "112604", valueArray[i=i+1], "Value of APOA02 should be 112604 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APOA03 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APOA04 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APOA05 should be 0 " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APSPNC should be '' " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APSPFR should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APSPTO should be 0 " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APRA01 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APRA02 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APRA03 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APRA04 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APRA05 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCC01 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCC02 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCC03 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APMRPR should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APRRCD should be '' " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APUCST should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APYUCS should be 0 " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APINSN should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APRF01 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APRF02 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APRF03 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APRF04 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APRF05 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APRF06 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APBA01 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APBA02 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APBA03 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APBA04 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APBA05 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APBA06 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APLN01 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APLN02 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APLN03 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APLN04 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APLN05 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APLN06 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APFI01 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APFI02 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APFI03 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APFI04 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APFI05 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APFI06 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APSX01 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APSX02 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APSX03 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APSX04 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APSX05 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APSX06 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APRS01 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APRS02 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APRS03 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APRS04 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APRS05 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APRS06 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APJA01 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APJA02 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APJA03 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APJA04 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APJA05 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APJA06 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCX01 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCX02 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCX03 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCX04 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCX05 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCX06 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APJS01 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APJS02 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APJS03 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APJS04 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APJS05 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APJS06 should be '' " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APJZ01 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APJZ02 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APJZ03 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APJZ04 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APJZ05 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APJZ06 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APJ401 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APJ402 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APJ403 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APJ404 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APJ405 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APJ406 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APJC01 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APJC02 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APJC03 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APJC04 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APJC05 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APJC06 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APJP01 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APJP02 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APJP03 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APJP04 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APJP05 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APJP06 should be 0 " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APG#01 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APG#02 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APG#03 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APG#04 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APG#05 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APG#06 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of API#01 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of API#02 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of API#03 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of API#04 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of API#05 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of API#06 should be '' " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APAG01 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APAG02 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APAG03 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APAG04 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APAG05 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APAG06 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APCH01 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APCH02 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APCH03 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APCH04 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APCH05 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APCH06 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APCP01 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APCP02 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APCP03 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APCP04 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APCP05 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APCP06 should be 0 " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCMED should be '' " );
            Assert.AreEqual( "'2'", valueArray[i=i+1], "Value of APNRCD should be '2' " );
            Assert.AreEqual( "'2'", valueArray[i=i+1], "Value of APCRCD should be '2' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APACOD should be '' " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APACDT should be 0 " );
            Assert.AreEqual( "'123456'", valueArray[i=i+1], "Value of APEEID should be '123456' " );
            Assert.AreEqual( "'1'", valueArray[i=i+1], "Value of APESCD should be '1' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APFANM should be '' " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APLVTD should be 0 " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APMONM should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APPOCC should be '' " );
            Assert.AreEqual( "'00000000000'", valueArray[i=i+1], "Value of APEID1 should be '00000000000' " );
            Assert.AreEqual( "'00000000000'", valueArray[i=i+1], "Value of APEID2 should be '00000000000' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APEDC1 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APEDC2 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APESC1 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APESC2 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APENM1 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APENM2 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APELO1 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APELO2 should be '' " );
            Assert.AreEqual( "' '", valueArray[i=i+1], "Value of APPUBL should be ' ' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APTSCR should be '' " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APSDED should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APDNOT should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APTALT should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APALTD should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of AP#COD should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APACTP should be 0 " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APWRKC should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APNOFT should be '' " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APABOR should be 0 " );
            Assert.AreEqual( "12105", valueArray[i=i+1], "Value of APLMEN should be 12105 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APRTCD should be 0 " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APSPC1 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APSPC2 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APSPC3 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APSPC4 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APSPC5 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APSPC6 should be '' " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APODR# should be 0 " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APOP01 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APOP02 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APOP03 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APOP04 should be '' " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APOD01 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APOD02 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APOD03 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APOD04 should be 0 " );
            Assert.AreEqual( "'AL'", valueArray[i=i+1], "Value of APLCL should be 'AL' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APTSRC should be '' " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APLML should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+2], "Value of APLUL# should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APLUL2 should be 0 " );
            Assert.AreEqual( "'A'", valueArray[i=i+1], "Value of APACFL should be 'A' " );
            Assert.AreEqual( "'$#L@%'", valueArray[i=i+2], "Value of APINLG should be '$#L@%' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APBYPS should be '' " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APSWPY should be 0 " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APPVRR should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APBDFG should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APBDAC should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APPLOE should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APMBF should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APERFG should be '' " );
            Assert.AreEqual( "'20'", valueArray[i=i+1], "Value of APFC should be '20' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APACC should be '' " );
            Assert.AreEqual( "''", valueArray[i = i + 1], "Value of APRSRC should be '' " ); 
            Assert.AreEqual( "20243", valueArray[i=i+1], "Value of APFDOB should be 20243 " );
            Assert.AreEqual( "10145", valueArray[i=i+1], "Value of APMDOB should be 10145 " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APEA01 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APEA02 should be '' " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APEZ01 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APEZ02 should be 0 " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APFORM should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APIN01 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APIN02 should be '' " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APFR01 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APFR02 should be 0 " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APOSIN should be '' " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APOSFD should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APOSTD should be 0 " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APPRGT should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APMELG should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APABC1 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APXMIT should be '' " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APQNUM should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APVIS# should be 0 " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APUPRV should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APUPRW should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APZDTE should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APZTME should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APWRNF should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APNMTL should be '' " );
            Assert.AreEqual( "3051955", valueArray[i=i+1], "Value of APDOB8 should be 3051955 " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APACMT should be '' " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APMB#1 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APMB#2 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APMB#3 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APLRDT should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APNRDT should be 0 " );
            Assert.AreEqual( "'60594'", valueArray[i=i+1], "Value of APPZPA should be '60594' " );
            Assert.AreEqual( "'0000'", valueArray[i=i+1], "Value of APPZ4A should be '0000' " );
            Assert.AreEqual( "'60503'", valueArray[i=i+1], "Value of APWZPA should be '60503' " );
            Assert.AreEqual( "'0000'", valueArray[i=i+1], "Value of APWZ4A should be '0000' " );
            Assert.AreEqual( "'USA'", valueArray[i=i+1], "Value of APWCUN should be 'USA' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APAKLN should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APAKFM should be '' " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APMDR# should be 0 " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APFBLF should be '' " );
            Assert.AreEqual( "'N'", valueArray[i=i+1], "Value of APABST should be 'N' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APAMLF should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APDTCD should be '' " );
            Assert.AreEqual( "'Y'", valueArray[i=i+1], "Value of APVALU should be 'Y' " );
            Assert.AreEqual( "'60594'", valueArray[i=i+1], "Value of APMZPA should be '60594' " );
            Assert.AreEqual( "'0000'", valueArray[i=i+1], "Value of APMZ4A should be '0000' " );
            Assert.AreEqual( "'60594'", valueArray[i=i+1], "Value of APCZPA should be '60594' " );
            Assert.AreEqual( "'0000'", valueArray[i=i+1], "Value of APCZ4A should be '0000' " );
            Assert.AreEqual( "'60594'", valueArray[i=i+1], "Value of APRZPA should be '60594' " );
            Assert.AreEqual( "'0000'", valueArray[i=i+1], "Value of APRZ4A should be '0000' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCNCD should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APMNU# should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APMNDN should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APMNFR should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APRNU# should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APRNDN should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APRNFR should be '' " );
            Assert.AreEqual( "'1'", valueArray[i=i+1], "Value of APLNGC should be '1' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APSPN2 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APOC06 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APOC07 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APOC08 should be '' " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APOA06 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APOA07 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APOA08 should be 0 " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APPRED should be '' " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of AP#FC should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of AP#RC should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APNOFC should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of AP#PL should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APELTM should be 0 " );
            Assert.AreEqual( "'TST'", valueArray[i=i+1], "Value of APIPA should be 'TST' " );
            Assert.AreEqual( "'CL34P'", valueArray[i=i+1], "Value of APIPAC should be 'CL34P' " );
            Assert.AreEqual( "'N'", valueArray[i=i+1], "Value of APCLVS should be 'N' " );
            Assert.AreEqual( "'FC@YAHOO.COM'", valueArray[i=i+1], "Value of APPEML should be 'FC@YAHOO.COM' " );
            Assert.AreEqual( "'723682238281111TX'", valueArray[i=i+1], "Value of APPDL# should be '723682238281111TX' " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APMNCD should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APMNP# should be 0 " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APRTYP should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APFRCD should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APARVC should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APRAMC should be '' " );
            Assert.AreEqual( "'123'", valueArray[i=i+1], "Value of APPCCD should be '123' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APBLDL should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APTECR should be '' " );
            Assert.AreEqual( "'USA'", valueArray[i=i+1], "Value of APPCUN should be 'USA' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APLBWT should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APPTID should be '' " );
            Assert.AreEqual( "' '", valueArray[i=i+1], "Value of APPRGI should be '' " );
            Assert.AreEqual( "'R'", valueArray[i=i+1], "Value of APPMI should be 'R' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APTDR# should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APMNLN should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APMNFN should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APMNMN should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APMNPR should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APMTXC should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APRNLN should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APRNFN should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APRNMN should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APRNPR should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APRTXC should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APRNP# should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APANLN should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APANFN should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APANMN should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APANU# should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APANPR should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APATXC should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APANP# should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APONLN should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APONFN should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APONMN should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APONU# should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APONPR should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APOTXC should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APONP# should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APTNLN should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APTNFN should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APTNMN should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APTNU# should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APTNPR should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APTTXC should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APTNP# should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APOZWT should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APPCPH should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APNTPP should be '' " );
            Assert.AreEqual( "'20030414'", valueArray[i=i+1], "Value of APDTRC should be '20030414' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APAVTM should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCI06 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCI07 should be '' " );
            Assert.AreEqual( "'10'", valueArray[i=i+1], "Value of APNPPV should be '10' " );
            Assert.AreEqual( "'NYYYY'", valueArray[i=i+1], "Value of APNPPF should be 'NYYYY' " );
            Assert.AreEqual( "'2'", valueArray[i=i+1], "Value of APPARC should be '2' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APACST should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APACCN should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APSCHD should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APMSL# should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APRSL# should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APASL# should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APOSL# should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APTSL# should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APPPT# should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APICUN should be '' " );
            Assert.AreEqual( "'A'", valueArray[i=i+1], "Value of APVCD1 should be 'A' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APVCD2 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APVCD3 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APVCD4 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APVCD5 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APVCD6 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APVCD7 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APVCD8 should be '' " );
            Assert.AreEqual( "1.23", valueArray[i=i+1], "Value of APVAM1 should be 1.23 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APVAM2 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APVAM3 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APVAM4 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APVAM5 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APVAM6 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APVAM7 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APVAM8 should be 0 " );
            Assert.AreEqual( "'PACCESS'", valueArray[i=i+1], "Value of APWSIR should be 'PACCESS' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APSECR should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APORR1 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APORR2 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APORR3 should be '' " );
            Assert.AreEqual( "'20011217'", valueArray[i=i+1], "Value of APNPPD should be '20011217' " );
            Assert.AreEqual( "'S'", valueArray[i=i+1], "Value of APNPPS should be 'S' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APPROCD should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APPREOPD should be '' " );
            Assert.AreEqual( "'Y'", valueArray[i=i+1], "Value of APCRSF should be 'Y' " );
            Assert.AreEqual( "'Y'", valueArray[i=i+1], "Value of APRSCF01 should be 'Y' " );
            Assert.AreEqual( "'Y'", valueArray[i=i+1], "Value of APRSCF02 should be 'Y' " );
            Assert.AreEqual( "'Y'", valueArray[i=i+1], "Value of APRSCF03 should be 'Y' " );
            Assert.AreEqual( "'Y'", valueArray[i=i+1], "Value of APRSCF04 should be 'Y' " );
            Assert.AreEqual( "'Y'", valueArray[i=i+1], "Value of APRSCF05 should be 'Y' " );
            Assert.AreEqual( "'N'", valueArray[i=i+1], "Value of APRSCF06 should be 'N' " );
            Assert.AreEqual( "'N'", valueArray[i=i+1], "Value of APRSCF07 should be 'N' " );
            Assert.AreEqual( "'N'", valueArray[i=i+1], "Value of APRSCF08 should be 'N' " );
            Assert.AreEqual( "'N'", valueArray[i=i+1], "Value of APRSCF09 should be 'N' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APRSCF10 should be '' " );
            Assert.AreEqual( "'TEST001'", valueArray[i=i+1], "Value of APRSID should be 'TEST001' " );
            Assert.AreEqual( "'TEST002'", valueArray[i=i+1], "Value of APRSID02 should be 'TEST002' " );
            Assert.AreEqual( "'TEST003'", valueArray[i=i+1], "Value of APRSID03 should be 'TEST003' " );
            Assert.AreEqual( "'TEST004'", valueArray[i=i+1], "Value of APRSID04 should be 'TEST004' " );
            Assert.AreEqual( "'TEST005'", valueArray[i=i+1], "Value of APRSID05 should be 'TEST005' " );
            Assert.AreEqual( "'TEST006'", valueArray[i=i+1], "Value of APRSID06 should be 'TEST006' " );
            Assert.AreEqual( "'TEST007'", valueArray[i=i+1], "Value of APRSID07 should be 'TEST007' " );
            Assert.AreEqual( "'TEST008'", valueArray[i=i+1], "Value of APRSID08 should be 'TEST008' " );
            Assert.AreEqual( "'TEST009'", valueArray[i=i+1], "Value of APRSID09 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APRSID10 should be '' " );
            Assert.AreEqual( "'ABC'", valueArray[i=i+1], "Value of APNHACF should be 'ABC' " );
            Assert.AreEqual("'234 MulHolland Drive'", valueArray[610], "Value of APPADRE1 should be '234 MulHolland Drive' ");
            Assert.AreEqual("'#1'", valueArray[611], "Value of APPADRE2 should be '#1' ");
            Assert.AreEqual("'234 MulHolland Drive'", valueArray[612], "Value of APMADRE1 should be '234 MulHolland Drive' ");
            Assert.AreEqual("'#1'", valueArray[613], "Value of APMADRE2 should be '#1' ");
            Assert.AreEqual("'2'", valueArray[614], "Value of APRACE2 should be '2' ");
            Assert.AreEqual("'102'", valueArray[615], "Value of APNTNLT1 should be '102' ");
            Assert.AreEqual("'204'", valueArray[616], "Value of APNTNLT2 should be '204' ");
            Assert.AreEqual("'2'", valueArray[617], "Value of APETHC2 should be '2' ");
            Assert.AreEqual("'106'", valueArray[618], "Value of APDESCNT1 should be '106' ");
            Assert.AreEqual("'104'", valueArray[619], "Value of APDESCNT2 should be '104' ");
            
        }

        [Test()]
        public void TestBuildSqlFromForPreDischarge()
        {
            Account anAccount = GetAccount();
            ContactPoint contactPointE = new ContactPoint(
                addressE, phoneNumberE, null, typeOfContactPointE );
            ContactPoint contactPointM = new ContactPoint(
                addressMP, phoneNumberM, emailAddress, typeOfContactPointM );
            ContactPoint contactPointP = new ContactPoint(
                addressMP, phoneNumberP, null, typeOfContactPointP );

            anAccount.Patient.PlaceOfWorship = placeOfWorship;
            anAccount.Patient.Race = race;
            anAccount.Patient.Race2 = race2;
            anAccount.Patient.Nationality = nationality;
            anAccount.Patient.Nationality2 = nationality2;
            anAccount.Patient.Ethnicity = ethnicity;
            anAccount.Patient.Ethnicity2 = ethnicity2;
            anAccount.Patient.Descent = descent;
            anAccount.Patient.Descent2 = descent2;
            anAccount.Patient.MaritalStatus = maritalStatus;
            anAccount.Patient.Language = language;
            anAccount.Patient.Religion = religion;
            anAccount.Patient.AddContactPoint( contactPointE );
            anAccount.Patient.AddContactPoint( contactPointM );
            anAccount.Patient.AddContactPoint( contactPointP );

            employment = new Employment( anAccount.Patient );
            Employer patEmpr = new Employer();
            employment.Employer = patEmpr;
            patEmpr.Name = "AMERICAN EXPRESS";

            anAccount.Patient = anAccount.Patient;
            anAccount.Facility = facility;

            anAccount.Patient.Employment = new Employment( anAccount.Patient );
            anAccount.Patient.DriversLicense = new DriversLicense(
                "72368223828",
                new State( PersistentModel.NEW_OID, DateTime.Now, "Texas" ) );

            Assert.AreEqual(
                "72368223828",
                anAccount.Patient.DriversLicense.Number );

            Assert.AreEqual(
                "YES",
                anAccount.ClergyVisit.Description );
            anAccount.FinancialClass =
                new FinancialClass( 279,
                ReferenceValue.NEW_VERSION,
                "COMMERCIAL INS",
                "20" );
            CoverageOrder primary = new CoverageOrder( 1, "Primary" );
            Assert.AreEqual(
                "Primary",
                primary.Description );

            CoverageOrder secondary = new CoverageOrder( 2, "Secondary" );
            Assert.AreEqual(
                "Secondary",
                secondary.Description );

            CommercialCoverage coverage1 = new CommercialCoverage();
            coverage1.CoverageOrder = primary;
            coverage1.Oid = 1;

            GovernmentMedicareCoverage coverage2 = new GovernmentMedicareCoverage();
            coverage2.CoverageOrder = secondary;
            coverage2.Oid = 2;

            Insurance insurance = new Insurance();
            insurance.AddCoverage( coverage1 );
            Assert.AreEqual(
                1,
                insurance.Coverages.Count );

            insurance.AddCoverage( coverage2 );
            Assert.AreEqual(
                2,
                insurance.Coverages.Count );

            anAccount.Insurance = insurance;
            Insured insured = new Insured();
            Employment emp = new Employment( insured );
            emp.EmployeeID = "123456";
            emp.Status = new EmploymentStatus(
                PersistentModel.NEW_OID, DateTime.Now, "EMPLOYED FULL TIME", "1" );

            Employer empr = new Employer();
            empr.Name = "qwert";
            emp.Employer = empr;

            emp.Employer.PartyContactPoint = contactPointE;
            anAccount.Patient.Employment = emp;

            EmergencyContact ec = new EmergencyContact();
            ec.Name = "Nobody";
            ec.AddContactPoint( contactPointM );
            ec.AddContactPoint( contactPointP );

            RelationshipType relType = new RelationshipType(
                PersistentModel.NEW_OID, DateTime.Now, "Spouse", "2" );

            ec.RelationshipType = relType;

            anAccount.EmergencyContact1 = ec;
            anAccount.EmergencyContact2 = ec;

            Assert.AreEqual(
                "Nobody",
                anAccount.EmergencyContact1.Name );

            Assert.AreEqual( relType.Code, "2" );

            Diagnosis diagnosis = new Diagnosis();

            Accident accident = new Accident();
            accident.OccurredOn = DateTime.Parse( "21 Jan, 2005" );
            accident.OccurredAtHour = "11:00";
            accident.Country = new Country( PersistentModel.NEW_OID, DateTime.Now, "USA" );
            accident.State = new State( PersistentModel.NEW_OID, DateTime.Now, "Texas" );
            accident.Kind = new TypeOfAccident(
                PersistentModel.NEW_OID, DateTime.Now, "Auto", "2" );
            diagnosis.Condition = accident;
            anAccount.Diagnosis = diagnosis;
            Accident acdt = (Accident)anAccount.Diagnosis.Condition;
            Assert.AreEqual(
                21,
                acdt.OccurredOn.Day );
            Assert.AreEqual(
                "11:00",
                acdt.OccurredAtHour );
            Assert.AreEqual(
                "USA",
                acdt.Country.Description );
            Assert.AreEqual(
                "Texas",
                acdt.State.Description );
            Assert.AreEqual(
                "Auto",
                acdt.Kind.Description );

            Assert.AreEqual(
                typeof( Accident ),
                anAccount.Diagnosis.Condition.GetType()
                );

            Pregnancy pregnancy = new Pregnancy();
            pregnancy.LastPeriod = DateTime.Parse( "Jan 21, 2005" );
            diagnosis.Condition = pregnancy;
            anAccount.Diagnosis = diagnosis;
            Pregnancy preg = (Pregnancy)anAccount.Diagnosis.Condition;
            Assert.AreEqual(
                21,
                preg.LastPeriod.Day );

            Assert.AreEqual(
                typeof( Pregnancy ),
                anAccount.Diagnosis.Condition.GetType()
                );

            Occ[0] = new OccurrenceCode(
                1, DateTime.Parse( "23, Nov 2001" ), "Accident_auto", "01" );
            ( (OccurrenceCode)Occ[0] ).OccurrenceDate = DateTime.Parse( "23, Nov 2001" );

            Occ[1] = new OccurrenceCode(
                19, DateTime.Parse( "26, Nov 2004" ), "Accident_other", "19" );
            ( (OccurrenceCode)Occ[1] ).OccurrenceDate = DateTime.Parse( "26, Nov 2004" );

            foreach ( OccurrenceCode occ in Occ )
            {
                anAccount.AddOccurrenceCode( occ );
            }

            Physician physician = new Physician( 123, ReferenceValue.NEW_VERSION, 1, 1, 1, 1, 1, 1 );
            physician.FirstName = "Doctor";
            physician.LastName = "Bob";
            physician.UPIN = "UP234";
            physician.NationalID = "NI234";
            physician.NPI = "1234567890";
            Address addr = new Address( "123 Dr Place", string.Empty, "Healthland", new ZipCode( "11223" ),
                new State( 1, ReferenceValue.NEW_VERSION, "Texas", "TX" ),
                new Country( 1, ReferenceValue.NEW_VERSION, "United States" ) );
            PhoneNumber pn = new PhoneNumber( "111", "2222222" );
            physician.AddContactPoint( new ContactPoint( addr, pn,
                new PhoneNumber(), new EmailAddress(), TypeOfContactPoint.NewBillingContactPointType() ) );
            anAccount.AddPhysicianWithRole( PhysicianRole.Admitting(), physician );

            Activity currentActivity =
                new PreDischargeActivity();
            anAccount.Activity = currentActivity;
            anAccount.ClinicalComments = "TEST COMMENTS1 TEST COMMENTS1TEST COMMENTS";
            anAccount.EmbosserCard = "1234567890";

            anAccount.IsPatientInClinicalResearchStudy = YesNoFlag.Yes;

            ResearchStudy study1 = new ResearchStudy( "TEST001", "RESEARCH STUDY 1", "RESEARCH SPONSOR 1" );
            ResearchStudy study2 = new ResearchStudy( "TEST002", "RESEARCH STUDY 2", "RESEARCH SPONSOR 2" );
            ResearchStudy study3 = new ResearchStudy( "TEST003", "RESEARCH STUDY 3", "RESEARCH SPONSOR 3" );
            ResearchStudy study4 = new ResearchStudy( "TEST004", "RESEARCH STUDY 4", "RESEARCH SPONSOR 4" );
            ResearchStudy study5 = new ResearchStudy( "TEST005", "RESEARCH STUDY 5", "RESEARCH SPONSOR 5" );
            ResearchStudy study6 = new ResearchStudy( "TEST006", "RESEARCH STUDY 6", "RESEARCH SPONSOR 6" );
            ResearchStudy study7 = new ResearchStudy( "TEST007", "RESEARCH STUDY 7", "RESEARCH SPONSOR 7" );
            ResearchStudy study8 = new ResearchStudy( "TEST008", "RESEARCH STUDY 8", "RESEARCH SPONSOR 8" );
            ResearchStudy study9 = new ResearchStudy( "TEST009", "RESEARCH STUDY 9", "RESEARCH SPONSOR 9" );

            anAccount.AddConsentedResearchStudy( new ConsentedResearchStudy( study1, YesNoFlag.Yes ) );
            anAccount.AddConsentedResearchStudy( new ConsentedResearchStudy( study2, YesNoFlag.Yes ) );
            anAccount.AddConsentedResearchStudy( new ConsentedResearchStudy( study3, YesNoFlag.Yes ) );
            anAccount.AddConsentedResearchStudy( new ConsentedResearchStudy( study4, YesNoFlag.Yes ) );
            anAccount.AddConsentedResearchStudy( new ConsentedResearchStudy( study5, YesNoFlag.Yes ) );
            anAccount.AddConsentedResearchStudy( new ConsentedResearchStudy( study6, YesNoFlag.No ) );
            anAccount.AddConsentedResearchStudy( new ConsentedResearchStudy( study7, YesNoFlag.No ) );
            anAccount.AddConsentedResearchStudy( new ConsentedResearchStudy( study8, YesNoFlag.No ) );
            anAccount.AddConsentedResearchStudy( new ConsentedResearchStudy( study9, YesNoFlag.No ) );

            i_PatientInsertStrategy = SqlBuilderStrategyFactory.CreatePatientInsertStrategy();

            i_PatientInsertStrategy.TransactionFileId = "PG";
            i_PatientInsertStrategy.PreDischargeFlag = "D";
            i_PatientInsertStrategy.UserSecurityCode = "KEVN";

            anAccount.CodedDiagnoses = new CodedDiagnoses();
            anAccount.CodedDiagnoses.CodedDiagnosises.Add( "V32.1" );
            anAccount.CodedDiagnoses.CodedDiagnosises.Add( "V12.3" );
            anAccount.CodedDiagnoses.AdmittingCodedDiagnosises.Add( "345.5" );
            anAccount.CodedDiagnoses.AdmittingCodedDiagnosises.Add( "987" );

            NoticeOfPrivacyPracticeDocument nppDocument = new NoticeOfPrivacyPracticeDocument();
            nppDocument.SignatureStatus = new SignatureStatus( "S" );
            nppDocument.SignedOnDate = new DateTime( 2001, 12, 17 );

            INPPVersionBroker nppVersionBroker = BrokerFactory.BrokerOfType<INPPVersionBroker>();
            nppDocument.NPPVersion = nppVersionBroker.NPPVersionWith( FACILITY_ID, "10" );

            anAccount.Patient.NoticeOfPrivacyPracticeDocument = nppDocument;

            anAccount.AlternateCareFacility = "ABC";

            ArrayList sqlStrings =
                i_PatientInsertStrategy.BuildSqlFrom( anAccount, transactionKeys );
            string sqlString = sqlStrings[0] as string;
            string[] valueArray = ValueArray( sqlString );

            int i = 0;

            Assert.AreEqual( NUMBER_OF_ENTRIES, valueArray.Length, "Wrong number of entries in ValueArray" );

            Assert.AreEqual( " ''", valueArray[i], "Value of APIDWS should be ''" );
            Assert.AreEqual( "'PG'", valueArray[i=i+1], "Value of APIDID should be 'PG' " );
            Assert.AreEqual( "'$#G%&*'", valueArray[i=i+1], "Value of APGREC should be '$#G%&*' " );
            Assert.AreEqual( "'$#P@%&'", valueArray[i=i+1], "Value of APRR# should be '$#P@%&' " );
            Assert.AreEqual( "365", valueArray[i=i+1], "Value of APPAUX should be 365 " );
            Assert.AreEqual( "'KEVN'", valueArray[i=i+1], "Value of APSEC2 should be 'KEVN' " );
            Assert.AreEqual( "6", valueArray[i=i+1], "Value of APHSP# should be 6 " );
            Assert.AreEqual( "23456789", valueArray[i=i+2], "Value of APACCT should be 23456789 " );
            Assert.AreEqual( "'FRIED'", valueArray[i=i+1], "Value of APPLNM should be 'FRIED' " );
            Assert.AreEqual( "'CLAIRE R'", valueArray[i=i+1], "Value of APPFNM should be 'CLAIRE R' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APLAST should be '' " );
            Assert.AreEqual( "'F'", valueArray[i=i+1], "Value of APSEX should be 'F' " );
            Assert.AreEqual( "'1'", valueArray[i=i+1], "Value of APRACE should be '1' " );
            Assert.AreEqual( "'046Y'", valueArray[i=i+1], "Value of APAGE should be '046Y' " );
            Assert.AreEqual( "30555", valueArray[i=i+1], "Value of APDOB should be 30555 " );
            Assert.AreEqual( "'3'", valueArray[i=i+1], "Value of APADMC should be '3' " );
            Assert.AreEqual( "121701", valueArray[i=i+1], "Value of APADMT should be 121701 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APADTM should be 0 " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APNS should be '' " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APROOM should be 0 " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APBED should be '' " );
            Assert.AreEqual( "'3'", valueArray[i=i+1], "Value of APETHC should be '3' " );
            Assert.AreEqual( "'1'", valueArray[i=i+1], "Value of APPTYP should be '1' " );
            Assert.AreEqual( "'99'", valueArray[i=i+1], "Value of APMSV should be '99' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APSMOK should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APDIET should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCOND should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APISO should be '' " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APADR# should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APRDR# should be 0 " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APPOB should be '' " );
            Assert.AreEqual( "'123121234'", valueArray[i=i+1], "Value of APSSN should be '123121234' " );
            Assert.AreEqual( "121701", valueArray[i=i+2], "Value of APLAD should be 121701 " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APLVLN should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APRLGN should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APALRG should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCOLG should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APPADR should be '' " );
            Assert.AreEqual( "'Austin'", valueArray[i=i+1], "Value of APPCIT should be 'Austin' " );
            Assert.AreEqual( "'TX'", valueArray[i=i+1], "Value of APPSTE should be 'TX' " );
            Assert.AreEqual( "60594", valueArray[i=i+1], "Value of APPZIP should be 60594 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APPZP4 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APPCNY should be 0 " );
            Assert.AreEqual( "972", valueArray[i=i+1], "Value of APPACD should be 972 " );
            Assert.AreEqual( "553453", valueArray[i=i+1], "Value of APPPH# should be 553453 " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APMADR should be '' " );
            Assert.AreEqual( "'Austin'", valueArray[i=i+1], "Value of APMCIT should be 'Austin' " );
            Assert.AreEqual( "'TX'", valueArray[i=i+1], "Value of APMSTE should be 'TX' " );
            Assert.AreEqual( "60594", valueArray[i=i+1], "Value of APMZIP should be 60594 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APMZP4 should be 0 " );
            Assert.AreEqual( "'123'", valueArray[i=i+1], "Value of APMCNY should be '123' " );
            Assert.AreEqual( "610", valueArray[i=i+1], "Value of APMACD should be 610 " );
            Assert.AreEqual( "546789", valueArray[i=i+1], "Value of APMPH# should be 546789 " );
            Assert.AreEqual( "'Nobody'", valueArray[i=i+1], "Value of APRNM should be 'Nobody' " );
            Assert.AreEqual( "'234 MulHolland Drive#1'", valueArray[i=i+1], "Value of APRADR should be '234 MulHolland Drive#1' " );
            Assert.AreEqual( "'Austin'", valueArray[i=i+1], "Value of APRCIT should be 'Austin' " );
            Assert.AreEqual( "'TX'", valueArray[i=i+1], "Value of APRSTE should be 'TX' " );
            Assert.AreEqual( "60594", valueArray[i=i+1], "Value of APRZIP should be 60594 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APRZP4 should be 0 " );
            Assert.AreEqual( "610", valueArray[i=i+1], "Value of APRACD should be 610 " );
            Assert.AreEqual( "546789", valueArray[i=i+1], "Value of APRPH# should be 546789 " );
            Assert.AreEqual( "'Nobody'", valueArray[i=i+1], "Value of APCNM should be 'Nobody' " );
            Assert.AreEqual( "'234 MulHolland Drive#1'", valueArray[i=i+1], "Value of APCADR should be '234 MulHolland Drive#1' " );
            Assert.AreEqual( "'Austin'", valueArray[i=i+1], "Value of APCCIT should be 'Austin' " );
            Assert.AreEqual( "'TX'", valueArray[i=i+1], "Value of APCSTE should be 'TX' " );
            Assert.AreEqual( "60594", valueArray[i=i+1], "Value of APCZIP should be 60594 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APCZP4 should be 0 " );
            Assert.AreEqual( "610", valueArray[i=i+1], "Value of APCACD should be 610 " );
            Assert.AreEqual( "546789", valueArray[i=i+1], "Value of APCPH# should be 546789 " );
            Assert.AreEqual( "'qwert'", valueArray[i=i+1], "Value of APWNM should be 'qwert' " );
            Assert.AreEqual( "'335 Nicholson Dr.#303'", valueArray[i=i+1], "Value of APWADR should be '335 Nicholson Dr.#303' " );
            Assert.AreEqual( "'Austin'", valueArray[i=i+1], "Value of APWCIT should be 'Austin' " );
            Assert.AreEqual( "'TX'", valueArray[i=i+1], "Value of APWSTE should be 'TX' " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APWZIP should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APWZP4 should be 0 " );
            Assert.AreEqual( "972", valueArray[i=i+1], "Value of APWACD should be 972 " );
            Assert.AreEqual( "546789", valueArray[i=i+1], "Value of APWPH# should be 546789 " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APHEMP should be '' " );
            Assert.AreEqual( "'1'", valueArray[i=i+1], "Value of APMAR should be '1' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APMNM should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APMOTH should be '' " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APMPT# should be 0 " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APSNM should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APSADR should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APSCIT should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APSSTE should be '' " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APSZIP should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APSZP4 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APSACD should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APSPH# should be 0 " );
            Assert.AreEqual( "'V32.1'", valueArray[i=i+1], "Value of APCD01 should be 'V32.1' " );
            Assert.AreEqual( "'V12.3'", valueArray[i=i+1], "Value of APCD02 should be 'V12.3' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCD03 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCD04 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCD05 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCD06 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCD07 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCD08 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APDIAG should be '' " );
            Assert.AreEqual( "'09123'", valueArray[i = i + 1], "Value of APCNTY should be '09123' " );
            Assert.AreEqual( "'TEST COMMENTS1 TEST COMMENTS1TEST COMMENTS'", valueArray[i=i+1], "Value of APCOM1 should be 'TEST COMMENTS1 TEST COMMENTS1TEST COMMENTS' " );
            Assert.AreEqual( "'                                                       1234567890'", valueArray[i=i+1], "Value of APCOM2 should be '                                                       1234567890' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APTCRT should be '' " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APDCRT should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APSCRT should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APNCRT should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APICRT should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of AP#EXT should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APDTOT should be 0 " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCLOC should be '' " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APPMRC should be 0 " );
            Assert.AreEqual( "23456789", valueArray[i=i+1], "Value of APGAR# should be 23456789 " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APHOWA should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APPOLN should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCORC should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APALOC should be '' " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APATIM should be 0 " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APPST should be '' " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APDISC should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APDEDT should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APLVD should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APLAT should be 0 " );
            Assert.AreEqual( "'1'", valueArray[i=i+1], "Value of APTREG should be '1' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCL01 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCL02 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCL03 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCL04 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCL05 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCL06 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCL07 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCL08 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCL09 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCL10 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCL11 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCL12 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCL13 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCL14 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCL15 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCL16 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCL17 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCL18 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCL19 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCL20 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCL21 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCL22 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCL23 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCL24 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCL25 should be '' " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APLDD should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APLDT should be 0 " );
            Assert.AreEqual( "23456789", valueArray[i=i+2], "Value of APKACT should be 23456789 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APPGAR should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APAL01 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APAL02 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APAL03 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APAL04 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APAL05 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APAL06 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APAL07 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APAL08 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APAL09 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APAL10 should be 0 " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APPSRC should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APPSTS should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCI01 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCI02 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCI03 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCI04 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCI05 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APSPGM should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APAPVL should be '' " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APAPFR should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APAPTO should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APGRCD should be 0 " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APTACD should be '' " );
            Assert.AreEqual( "'01'", valueArray[i=i+1], "Value of APOC01 should be '01' " );
            Assert.AreEqual( "'19'", valueArray[i=i+1], "Value of APOC02 should be '19' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APOC03 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APOC04 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APOC05 should be '' " );
            Assert.AreEqual( "112301", valueArray[i=i+1], "Value of APOA01 should be 112301 " );
            Assert.AreEqual( "112604", valueArray[i=i+1], "Value of APOA02 should be 112604 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APOA03 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APOA04 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APOA05 should be 0 " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APSPNC should be '' " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APSPFR should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APSPTO should be 0 " );
            Assert.AreEqual( "'345.5'", valueArray[i=i+1], "Value of APRA01 should be '345.5' " );
            Assert.AreEqual( "'987'", valueArray[i=i+1], "Value of APRA02 should be '987' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APRA03 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APRA04 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APRA05 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCC01 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCC02 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCC03 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APMRPR should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APRRCD should be '' " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APUCST should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APYUCS should be 0 " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APINSN should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APRF01 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APRF02 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APRF03 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APRF04 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APRF05 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APRF06 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APBA01 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APBA02 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APBA03 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APBA04 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APBA05 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APBA06 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APLN01 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APLN02 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APLN03 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APLN04 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APLN05 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APLN06 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APFI01 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APFI02 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APFI03 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APFI04 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APFI05 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APFI06 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APSX01 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APSX02 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APSX03 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APSX04 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APSX05 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APSX06 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APRS01 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APRS02 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APRS03 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APRS04 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APRS05 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APRS06 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APJA01 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APJA02 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APJA03 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APJA04 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APJA05 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APJA06 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCX01 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCX02 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCX03 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCX04 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCX05 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCX06 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APJS01 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APJS02 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APJS03 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APJS04 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APJS05 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APJS06 should be '' " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APJZ01 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APJZ02 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APJZ03 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APJZ04 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APJZ05 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APJZ06 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APJ401 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APJ402 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APJ403 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APJ404 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APJ405 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APJ406 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APJC01 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APJC02 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APJC03 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APJC04 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APJC05 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APJC06 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APJP01 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APJP02 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APJP03 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APJP04 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APJP05 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APJP06 should be 0 " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APG#01 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APG#02 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APG#03 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APG#04 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APG#05 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APG#06 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of API#01 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of API#02 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of API#03 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of API#04 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of API#05 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of API#06 should be '' " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APAG01 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APAG02 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APAG03 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APAG04 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APAG05 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APAG06 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APCH01 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APCH02 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APCH03 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APCH04 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APCH05 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APCH06 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APCP01 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APCP02 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APCP03 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APCP04 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APCP05 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APCP06 should be 0 " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCMED should be '' " );
            Assert.AreEqual( "'2'", valueArray[i=i+1], "Value of APNRCD should be '2' " );
            Assert.AreEqual( "'2'", valueArray[i=i+1], "Value of APCRCD should be '2' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APACOD should be '' " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APACDT should be 0 " );
            Assert.AreEqual( "'123456'", valueArray[i=i+1], "Value of APEEID should be '123456' " );
            Assert.AreEqual( "'1'", valueArray[i=i+1], "Value of APESCD should be '1' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APFANM should be '' " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APLVTD should be 0 " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APMONM should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APPOCC should be '' " );
            Assert.AreEqual( "'00000000000'", valueArray[i=i+1], "Value of APEID1 should be '00000000000' " );
            Assert.AreEqual( "'00000000000'", valueArray[i=i+1], "Value of APEID2 should be '00000000000' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APEDC1 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APEDC2 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APESC1 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APESC2 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APENM1 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APENM2 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APELO1 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APELO2 should be '' " );
            Assert.AreEqual( "' '", valueArray[i=i+1], "Value of APPUBL should be ' ' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APTSCR should be '' " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APSDED should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APDNOT should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APTALT should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APALTD should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of AP#COD should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APACTP should be 0 " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APWRKC should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APNOFT should be '' " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APABOR should be 0 " );
            Assert.AreEqual( "12105", valueArray[i=i+1], "Value of APLMEN should be 12105 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APRTCD should be 0 " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APSPC1 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APSPC2 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APSPC3 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APSPC4 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APSPC5 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APSPC6 should be '' " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APODR# should be 0 " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APOP01 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APOP02 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APOP03 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APOP04 should be '' " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APOD01 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APOD02 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APOD03 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APOD04 should be 0 " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APLCL should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APTSRC should be '' " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APLML should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+2], "Value of APLUL# should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APLUL2 should be 0 " );
            Assert.AreEqual( "'C'", valueArray[i=i+1], "Value of APACFL should be 'C' " );
            Assert.AreEqual( "'$#L@%'", valueArray[i=i+2], "Value of APINLG should be '$#L@%' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APBYPS should be '' " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APSWPY should be 0 " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APPVRR should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APBDFG should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APBDAC should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APPLOE should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APMBF should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APERFG should be '' " );
            Assert.AreEqual( "'20'", valueArray[i=i+1], "Value of APFC should be '20' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APACC should be '' " );
            Assert.AreEqual( "''", valueArray[i = i + 1], "Value of APRSRC should be '' " ); 
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APFDOB should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APMDOB should be 0 " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APEA01 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APEA02 should be '' " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APEZ01 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APEZ02 should be 0 " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APFORM should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APIN01 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APIN02 should be '' " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APFR01 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APFR02 should be 0 " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APOSIN should be '' " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APOSFD should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APOSTD should be 0 " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APPRGT should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APMELG should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APABC1 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APXMIT should be '' " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APQNUM should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APVIS# should be 0 " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APUPRV should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APUPRW should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APZDTE should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APZTME should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APWRNF should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APNMTL should be '' " );
            Assert.AreEqual( "3051955", valueArray[i=i+1], "Value of APDOB8 should be 3051955 " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APACMT should be '' " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APMB#1 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APMB#2 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APMB#3 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APLRDT should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APNRDT should be 0 " );
            Assert.AreEqual( "'60594'", valueArray[i=i+1], "Value of APPZPA should be '60594' " );
            Assert.AreEqual( "'0000'", valueArray[i=i+1], "Value of APPZ4A should be '0000' " );
            Assert.AreEqual( "'60503'", valueArray[i=i+1], "Value of APWZPA should be '60503' " );
            Assert.AreEqual( "'0000'", valueArray[i=i+1], "Value of APWZ4A should be '0000' " );
            Assert.AreEqual( "'USA'", valueArray[i=i+1], "Value of APWCUN should be 'USA' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APAKLN should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APAKFM should be '' " );
            Assert.AreEqual( "123", valueArray[i=i+1], "Value of APMDR# should be 123 " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APFBLF should be '' " );
            Assert.AreEqual( "'N'", valueArray[i=i+1], "Value of APABST should be 'N' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APAMLF should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APDTCD should be '' " );
            Assert.AreEqual( "' '", valueArray[i=i+1], "Value of APVALU should be ' ' " );
            Assert.AreEqual( "'60594'", valueArray[i=i+1], "Value of APMZPA should be '60594' " );
            Assert.AreEqual( "'0000'", valueArray[i=i+1], "Value of APMZ4A should be '0000' " );
            Assert.AreEqual( "'60594'", valueArray[i=i+1], "Value of APCZPA should be '60594' " );
            Assert.AreEqual( "'0000'", valueArray[i=i+1], "Value of APCZ4A should be '0000' " );
            Assert.AreEqual( "'60594'", valueArray[i=i+1], "Value of APRZPA should be '60594' " );
            Assert.AreEqual( "'0000'", valueArray[i=i+1], "Value of APRZ4A should be '0000' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCNCD should be '' " );
            Assert.AreEqual( "'UP234'", valueArray[i=i+1], "Value of APMNU# should be 'UP234' " );
            Assert.AreEqual( "'BobDoctor'", valueArray[i=i+1], "Value of APMNDN should be 'BobDoctor' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APMNFR should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APRNU# should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APRNDN should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APRNFR should be '' " );
            Assert.AreEqual( "'1'", valueArray[i=i+1], "Value of APLNGC should be '1' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APSPN2 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APOC06 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APOC07 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APOC08 should be '' " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APOA06 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APOA07 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APOA08 should be 0 " );
            Assert.AreEqual( "'D'", valueArray[i=i+1], "Value of APPRED should be 'D' " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of AP#FC should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of AP#RC should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APNOFC should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of AP#PL should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APELTM should be 0 " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APIPA should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APIPAC should be '' " );
            Assert.AreEqual( "'N'", valueArray[i=i+1], "Value of APCLVS should be 'N' " );
            Assert.AreEqual( "'FC@YAHOO.COM'", valueArray[i=i+1], "Value of APPEML should be 'FC@YAHOO.COM' " );
            Assert.AreEqual( "'72368223828'", valueArray[i=i+1], "Value of APPDL# should be '72368223828' " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APMNCD should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APMNP# should be 0 " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APRTYP should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APFRCD should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APARVC should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APRAMC should be '' " );
            Assert.AreEqual( "'123'", valueArray[i=i+1], "Value of APPCCD should be '123' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APBLDL should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APTECR should be '' " );
            Assert.AreEqual( "'USA'", valueArray[i=i+1], "Value of APPCUN should be 'USA' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APLBWT should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APPTID should be '' " );
            Assert.AreEqual( "' '", valueArray[i=i+1], "Value of APPRGI should be '' " );
            Assert.AreEqual( "'R'", valueArray[i=i+1], "Value of APPMI should be 'R' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APTDR# should be '' " );
            Assert.AreEqual( "'Bob'", valueArray[i=i+1], "Value of APMNLN should be 'Bob' " );
            Assert.AreEqual( "'Doctor'", valueArray[i=i+1], "Value of APMNFN should be 'Doctor' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APMNMN should be '' " );
            Assert.AreEqual( "'1234567890'", valueArray[i=i+1], "Value of APMNPR should be '1234567890' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APMTXC should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APRNLN should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APRNFN should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APRNMN should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APRNPR should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APRTXC should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APRNP# should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APANLN should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APANFN should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APANMN should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APANU# should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APANPR should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APATXC should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APANP# should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APONLN should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APONFN should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APONMN should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APONU# should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APONPR should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APOTXC should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APONP# should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APTNLN should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APTNFN should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APTNMN should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APTNU# should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APTNPR should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APTTXC should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APTNP# should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APOZWT should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APPCPH should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APNTPP should be '' " );
            Assert.AreEqual( "'20030414'", valueArray[i=i+1], "Value of APDTRC should be '20030414' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APAVTM should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCI06 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APCI07 should be '' " );
            Assert.AreEqual( "'10'", valueArray[i=i+1], "Value of APNPPV should be '10' " );
            Assert.AreEqual( "'NYYYY'", valueArray[i=i+1], "Value of APNPPF should be 'NYYYY' " );
            Assert.AreEqual( "'2'", valueArray[i=i+1], "Value of APPARC should be '2' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APACST should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APACCN should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APSCHD should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APMSL# should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APRSL# should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APASL# should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APOSL# should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APTSL# should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APPPT# should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APICUN should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APVCD1 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APVCD2 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APVCD3 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APVCD4 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APVCD5 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APVCD6 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APVCD7 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APVCD8 should be '' " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APVAM1 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APVAM2 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APVAM3 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APVAM4 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APVAM5 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APVAM6 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APVAM7 should be 0 " );
            Assert.AreEqual( "0", valueArray[i=i+1], "Value of APVAM8 should be 0 " );
            Assert.AreEqual( "'PACCESS'", valueArray[i=i+1], "Value of APWSIR should be 'PACCESS' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APSECR should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APORR1 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APORR2 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APORR3 should be '' " );
            Assert.AreEqual( "'20011217'", valueArray[i=i+1], "Value of APNPPD should be '20011217' " );
            Assert.AreEqual( "'S'", valueArray[i=i+1], "Value of APNPPS should be 'S' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APPROCD should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APPREOPD should be '' " );
            Assert.AreEqual( "'Y'", valueArray[i=i+1], "Value of APCRSF should be 'Y' " );
            Assert.AreEqual( "'Y'", valueArray[i=i+1], "Value of APRSCF01 should be 'Y' " );
            Assert.AreEqual( "'Y'", valueArray[i=i+1], "Value of APRSCF02 should be 'Y' " );
            Assert.AreEqual( "'Y'", valueArray[i=i+1], "Value of APRSCF03 should be 'Y' " );
            Assert.AreEqual( "'Y'", valueArray[i=i+1], "Value of APRSCF04 should be 'Y' " );
            Assert.AreEqual( "'Y'", valueArray[i=i+1], "Value of APRSCF05 should be 'Y' " );
            Assert.AreEqual( "'N'", valueArray[i=i+1], "Value of APRSCF06 should be 'N' " );
            Assert.AreEqual( "'N'", valueArray[i=i+1], "Value of APRSCF07 should be 'N' " );
            Assert.AreEqual( "'N'", valueArray[i=i+1], "Value of APRSCF08 should be 'N' " );
            Assert.AreEqual( "'N'", valueArray[i=i+1], "Value of APRSCF09 should be 'N' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APRSCF10 should be '' " );
            Assert.AreEqual( "'TEST001'", valueArray[i=i+1], "Value of APRSID should be 'TEST001' " );
            Assert.AreEqual( "'TEST002'", valueArray[i=i+1], "Value of APRSID02 should be 'TEST002' " );
            Assert.AreEqual( "'TEST003'", valueArray[i=i+1], "Value of APRSID03 should be 'TEST003' " );
            Assert.AreEqual( "'TEST004'", valueArray[i=i+1], "Value of APRSID04 should be 'TEST004' " );
            Assert.AreEqual( "'TEST005'", valueArray[i=i+1], "Value of APRSID05 should be 'TEST005' " );
            Assert.AreEqual( "'TEST006'", valueArray[i=i+1], "Value of APRSID06 should be 'TEST006' " );
            Assert.AreEqual( "'TEST007'", valueArray[i=i+1], "Value of APRSID07 should be 'TEST007' " );
            Assert.AreEqual( "'TEST008'", valueArray[i=i+1], "Value of APRSID08 should be 'TEST008' " );
            Assert.AreEqual( "'TEST009'", valueArray[i=i+1], "Value of APRSID09 should be '' " );
            Assert.AreEqual( "''", valueArray[i=i+1], "Value of APRSID10 should be '' " );
            Assert.AreEqual( "'ABC'", valueArray[i=i+1], "Value of APNHACF should be 'ABC' " );
            Assert.AreEqual("'234 MulHolland Drive'", valueArray[610], "Value of APPADRE1 should be '234 MulHolland Drive' ");
            Assert.AreEqual("'#1'", valueArray[611], "Value of APPADRE2 should be '#1' ");
            Assert.AreEqual("'234 MulHolland Drive'", valueArray[612], "Value of APMADRE1 should be '234 MulHolland Drive' ");
            Assert.AreEqual("'#1'", valueArray[613], "Value of APMADRE2 should be '#1' ");
            Assert.AreEqual("'2'", valueArray[614], "Value of APRACE2 should be '2' ");
            Assert.AreEqual("'102'", valueArray[615], "Value of APNTNLT1 should be '102' ");
            Assert.AreEqual("'204'", valueArray[616], "Value of APNTNLT2 should be '204' ");
            Assert.AreEqual("'2'", valueArray[617], "Value of APETHC2 should be '2' ");
            Assert.AreEqual("'106'", valueArray[618], "Value of APDESCNT1 should be '106' ");
            Assert.AreEqual("'104'", valueArray[619], "Value of APDESCNT2 should be '104' ");

        }

        [Test()]
        public void TestBuildSqlForAliasNamesForPreDischarge()
        {
            Account anAccount = GetAccount();
            Name alias1 = new Name( "C", "F", "MI", "SU", "Y" );
            alias1.EntryDate = DateTime.Parse( "5/12/2006 00:00:00" );

            Name alias2 = new Name( "C2", "F2", "m", "S", "N" );
            alias2.EntryDate = DateTime.Parse( "5/12/2006 00:00:00" );

            anAccount.Patient.AddAlias( alias1 );
            anAccount.Patient.AddAlias( alias2 );

            anAccount.Patient.DriversLicense = new DriversLicense(
                "72368223828",
                new State( PersistentModel.NEW_OID, DateTime.Now, "Texas" ) );

            ArrayList sqlStrings = BuildSql( anAccount );

            Assert.AreEqual( anAccount.Patient.Aliases.Count + 2, sqlStrings.Count,
               "Number of sqlStatements genereated should be 4" );
            var sqlString = sqlStrings[2] as string;
            string[] valueArray =
                ValueArray( sqlString );

            Assert.AreEqual( " 6", valueArray[0], "Value of AKHSP# should be ' 6' " );
            Assert.AreEqual( "24004", valueArray[1], "Value of AKMRC# should be 24004 " );
            Assert.AreEqual( "'F'", valueArray[2], "Value of AKPLNM should be 'F' " );
            Assert.AreEqual( "'C'", valueArray[3], "Value of AKPFNM should be 'C' " );
            Assert.AreEqual( "'SU'", valueArray[4], "Value of AKTITL should be 'SU' " );
            Assert.AreEqual( "60512", valueArray[5], "Value of AKEDAT should be 60512 " );
            Assert.AreEqual( "'Y'", valueArray[6], "Value of AKSECF should be 'Y' " );

        }
        [Test()]
        public void TestBuildSqlForPregnancyIndicator()
        {
            Account anAccount = GetPreRegAccount();
            anAccount.Pregnant = YesNoFlag.Yes;
            ArrayList sqlStrings = BuildSql( anAccount );
            Assert.AreEqual( anAccount.Patient.Aliases.Count + 3, sqlStrings.Count,
               "Number of sqlStatements genereated should be 3" );
            var sqlString = sqlStrings[2] as string;
            string[] valueArray =
                ValueArray( sqlString );
            Assert.AreEqual( " 6", valueArray[0], "Value of TMHSP# should be ' 6' " );
            Assert.AreEqual( "24004", valueArray[1], "Value of TMMRC# should be 24004 " );
            Assert.AreEqual( "23456789", valueArray[2], "Value of TMACCT should be '23456789' " );
            Assert.AreEqual( "'Y'", valueArray[3], "Value of TMPRGI should be 'Y' " );

        }
        
        [Test()]
        public void TestAddColumnsAndValuesToSqlStatement()
        {
            testOrderedList = new OrderedList();
            testOrderedList.Add( "APIDWS", string.Empty );
            testOrderedList.Add( "APIDID", "PC" );
            testOrderedList.Add( "APSEC2", "KEVN" );
            testOrderedList.Add( "APHSP#", 6 );
            testOrderedList.Add( "APMRC#", 123456789 );
            i_PatientInsertStrategy = SqlBuilderStrategyFactory.CreatePatientInsertStrategy();
            i_PatientInsertStrategy.AddColumnsAndValuesToSqlStatement(
                testOrderedList, TABLE_NAME );

            string expectedInsertSqlStatement = "INSERT INTO HPDATA2.HPADAPMP (APIDWS,APIDID,APSEC2,APHSP#,APMRC#) VALUES ( '','PC','KEVN',6,123456789)";

            Assert.AreEqual(
                expectedInsertSqlStatement,
                i_PatientInsertStrategy.SqlStatement,
                " The insert string generated is as expected" );
        }

        [Test()]
        public void TestFormatForSql()
        {
            i_PatientInsertStrategy = SqlBuilderStrategyFactory.CreatePatientInsertStrategy();
            stringTestObject = TESTSTRING;

            Assert.AreEqual(
                EXPECTEDSTRING,
                i_PatientInsertStrategy.FormatForSql( stringTestObject ),
                "The value formatted for Sql is as expected" );

            integerTestObject = TESTINTEGER;

            Assert.AreEqual(
                EXPECTEDINTEGER,
                i_PatientInsertStrategy.FormatForSql( integerTestObject ),
                "The value formatted for Sql is as expected" );
        }

        [Test()]
        public void TestConvertToInt()
        {
            i_PatientInsertStrategy = SqlBuilderStrategyFactory.CreatePatientInsertStrategy();

            Assert.AreEqual(
                0,
                i_PatientInsertStrategy.ConvertToInt( BLANKSTRING ),
                "Integer value is as expected" );

            Assert.AreEqual(
                34,
                i_PatientInsertStrategy.ConvertToInt( TESTVALUE ),
                "Integer value is as expected" );

            Assert.AreEqual(
                0,
                i_PatientInsertStrategy.ConvertToInt( BLANKSTRING ),
                "Integer value is as expected" );

            Assert.AreEqual(
                0,
                i_PatientInsertStrategy.ConvertToInt( ANOTHERSTRING ),
                "Integer value is as expected" );
        }

        [Test()]
        public void TestUpdateColumnValuesUsingForPreRegistration()
        {
            IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            Facility facility_ACO = facilityBroker.FacilityWith( FACILITY_ID );

            Gender patientSex = new Gender( 2, DateTime.Now, "Male", "M" );
            DateTime patientDOB = new DateTime( 1965, 01, 13 );
            Patient patient = new Patient();
            patient.Oid = 1723;
            patient.Facility = facility_ACO;
            patient.FirstName = "SONNY";
            patient.LastName = "SADSTORY";
            patient.DateOfBirth = patientDOB;
            patient.Sex = patientSex;
            patient.MedicalRecordNumber = 785138;
            AccountProxy proxy = new AccountProxy( 30015,
                patient,
                DateTime.Now,
                DateTime.Now,
                new VisitType( 0, ReferenceValue.NEW_VERSION, VisitType.OUTPATIENT_DESC, VisitType.OUTPATIENT ),
                facility_ACO,
                new FinancialClass( 299, ReferenceValue.NEW_VERSION, "MEDICARE", "40" ),
                new HospitalService( 0, ReferenceValue.NEW_VERSION, "DIAGNOSTIC OUTOPT", "60" ),
                "OL HSV60",
                false );

            IAccountBroker accountBroker = BrokerFactory.BrokerOfType<IAccountBroker>();
            Account account = accountBroker.AccountFor( proxy );
            account.Activity = new PreRegistrationActivity();
            i_PatientInsertStrategy = SqlBuilderStrategyFactory.CreatePatientInsertStrategy();
            i_PatientInsertStrategy.UpdateColumnValuesUsing( account );

            Assert.IsTrue( true, "Update of order list with values Succeeded" );

            ArrayList sqlStrings =
                i_PatientInsertStrategy.BuildSqlFrom( account, transactionKeys );
            string sqlString = sqlStrings[0] as string;
            sqlString = sqlString.Replace( "INSERT INTO HPADAPMP (", string.Empty );
            int startPositionOfValues = sqlString.IndexOf( "(" );
            string[] ColArray =
                sqlString.Substring( 0, startPositionOfValues ).Split( ',' );

            Assert.AreEqual( NUMBER_OF_ENTRIES, ColArray.Length, "Wrong number of entries in ValueArray" );
        }

        [Test()]
        public void TestUpdateColumnValuesUsingForPreDischarge()
        {
            IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            Facility facility_ACO = facilityBroker.FacilityWith( FACILITY_ID );


            PatientSearchCriteria criteria = new PatientSearchCriteria(
                HSPCODE,
                String.Empty,
                String.Empty,
                String.Empty,
                null,
                PatientSearchCriteria.NO_MONTH,
                PatientSearchCriteria.NO_YEAR,
                TEST_MRN,
                String.Empty
                );


            IPatientBroker patientBroker = BrokerFactory.BrokerOfType<IPatientBroker>();

            PatientSearchResponse patientSearchResponse = patientBroker.GetPatientSearchResponseFor( criteria );
            Patient patient = patientBroker.PatientFrom( patientSearchResponse.PatientSearchResults[0] );

            IAccountBroker accountBroker = BrokerFactory.BrokerOfType<IAccountBroker>();
            AccountProxy proxy = accountBroker.AccountProxyFor( facility_ACO.Code, patient,
                    785138, 30015 );
            Account account = accountBroker.AccountFor( proxy );
            account.Activity = new PreDischargeActivity();

            IntentToDischargeTransactionCoordinator intentToDischarge =
                new IntentToDischargeTransactionCoordinator( account );
            i_PatientInsertStrategy = SqlBuilderStrategyFactory.CreatePatientInsertStrategy();
            i_PatientInsertStrategy.UpdateColumnValuesUsing( account );

            Assert.IsTrue( true, "Update of order list with values Succeeded" );

            ArrayList sqlStrings =
                i_PatientInsertStrategy.BuildSqlFrom( account, transactionKeys );
            string sqlString = sqlStrings[0] as string;
            sqlString = sqlString.Replace( "INSERT INTO HPADAPMP (", string.Empty );
            int startPositionOfValues = sqlString.IndexOf( "(" );
            string[] ColArray =
                sqlString.Substring( 0, startPositionOfValues ).Split( ',' );

            Assert.AreEqual( NUMBER_OF_ENTRIES, ColArray.Length, "Wrong number of entries in ValueArray" );
        }
        
        [Test, Sequential]
        public void TestUpdateColumnValues_AdmitTypeShouldNotChange_EditMaintain(
            [Values("1","2","3","4","5","6", "")] string prevAdmitType)
        {
            IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            Facility facility_ACO = facilityBroker.FacilityWith(FACILITY_ID);

            Gender patientSex = new Gender(2, DateTime.Now, "Male", "M");
            DateTime patientDOB = new DateTime(1965, 01, 13);
            Patient patient = new Patient
                                  {
                                      Oid = 1723,
                                      Facility = facility_ACO,
                                      FirstName = "SONNY",
                                      LastName = "SADSTORY",
                                      DateOfBirth = patientDOB,
                                      Sex = patientSex,
                                      MedicalRecordNumber = 785138
                                  };
            
            AccountProxy proxy = new AccountProxy(30015,
                patient,
                DateTime.Now,
                DateTime.Now,
                new VisitType(0, ReferenceValue.NEW_VERSION, VisitType.INPATIENT_DESC, VisitType.INPATIENT),
                facility_ACO,
                new FinancialClass(299, ReferenceValue.NEW_VERSION, "MEDICARE", "40"),
                new HospitalService(0, ReferenceValue.NEW_VERSION, "DIAGNOSTIC OUTOPT", "60"),
                "OL HSV60",
                false);

            IAccountBroker accountBroker = BrokerFactory.BrokerOfType<IAccountBroker>();
            Account account = accountBroker.AccountFor(proxy);
            account.Activity = new MaintenanceActivity();
            account.AdmittingCategory = prevAdmitType;
            i_PatientInsertStrategy = SqlBuilderStrategyFactory.CreatePatientInsertStrategy();
            i_PatientInsertStrategy.UpdateColumnValuesUsing(account);
            
            ArrayList sqlStrings =
                i_PatientInsertStrategy.BuildSqlFrom(account, transactionKeys);
            string sqlString = sqlStrings[0] as string;
            sqlString = sqlString.Replace("INSERT INTO HPADAPMP (", string.Empty);
            int startPositionOfValues = sqlString.IndexOf("VALUES (");
            string[] ColArray =
                sqlString.Substring(startPositionOfValues,sqlString.Length-startPositionOfValues-1).Split(',');
            var admitType = ColArray[APADMC_INDEX];
            Assert.AreEqual("'"+prevAdmitType+"'", admitType,"Admit Type should not be changed for Edit/Maintain");
        }


        [Test]
        public void TestUpdateColumnValues_AdmitTypeShouldNotChange_TransferOutToIn()
        {
            var facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            var facilityAco = facilityBroker.FacilityWith(FACILITY_ID);

            var patientSex = new Gender(2, DateTime.Now, "Male", "M");
            var patientDob = new DateTime(1965, 01, 13);
            var patient = new Patient
            {
                Oid = 1723,
                Facility = facilityAco,
                FirstName = "SONNY",
                LastName = "SADSTORY",
                DateOfBirth = patientDob,
                Sex = patientSex,
                MedicalRecordNumber = 785138
            };

            var proxy = new AccountProxy(30015,
                patient,
                DateTime.Now,
                DateTime.Now,
                new VisitType(0, ReferenceValue.NEW_VERSION, VisitType.INPATIENT_DESC, VisitType.INPATIENT),
                facilityAco,
                new FinancialClass(299, ReferenceValue.NEW_VERSION, "MEDICARE", "40"),
                new HospitalService(0, ReferenceValue.NEW_VERSION, "DIAGNOSTIC OUTOPT", "60"),
                "OL HSV60",
                false);

            var accountBroker = BrokerFactory.BrokerOfType<IAccountBroker>();
            var account = accountBroker.AccountFor(proxy);
            account.Activity = new MaintenanceActivity();
            account.AdmittingCategory = ADMITTING_CATEGORY_EMERGENCY;
            i_PatientInsertStrategy = SqlBuilderStrategyFactory.CreatePatientInsertStrategy();
            i_PatientInsertStrategy.UpdateColumnValuesUsing(account);

            var sqlStrings = i_PatientInsertStrategy.BuildSqlFrom(account, transactionKeys);
            string sqlString = sqlStrings[0] as string;
            sqlString = sqlString.Replace("INSERT INTO HPADAPMP (", string.Empty);
            int startPositionOfValues = sqlString.IndexOf("VALUES (");
            string[] colArray = sqlString.Substring(startPositionOfValues, sqlString.Length - startPositionOfValues - 1).Split(',');
            var admitType = colArray[APADMC_INDEX];

            Assert.AreEqual("'" + ADMITTING_CATEGORY_EMERGENCY + "'", admitType, "Admit Type should not be changed for Transfer Out To In");
        } 

        [Test, Sequential]
        public void TestUpdateColumnValues_PatientPortalOptIn_And_RightToRestrict_Registration(
            [Values("Y", " ")] string patientPortalOptIn,
            [Values("Y", " ")] string rightToRestrict,
            [Values("N", "N")] string HIEConsent,
            [Values("Y", "N")] string HIEPhysicianConsent)
        {
            string ExpectedResult = patientPortalOptIn + rightToRestrict + HIEConsent + HIEPhysicianConsent;
            var account = GetRegAccount(new RegistrationActivity(), patientPortalOptIn, rightToRestrict, HIEConsent,
               HIEPhysicianConsent);
            string APFILL = GetColumnValue(APFILL_INDEX, account);
            Assert.AreEqual("'" + ExpectedResult + "'", APFILL, "Patient portal optin saved during registration activity");
        }
        [Test, Sequential]
        public void TestUpdateColumnValues_PatientPortalOptIn_Unable_Option_And_RightToRestrict_Registration(
            [Values("U", " ")] string patientPortalOptIn,
            [Values("Y", " ")] string rightToRestrict,
            [Values("N", "N")] string HIEConsent,
            [Values("Y", "N")] string HIEPhysicianConsent)
        {
            string ExpectedResult = patientPortalOptIn + rightToRestrict + HIEConsent + HIEPhysicianConsent;
            var account = GetRegAccount(new RegistrationActivity(), patientPortalOptIn, rightToRestrict, HIEConsent,
                HIEPhysicianConsent);
            string APFILL = GetColumnValue(APFILL_INDEX, account);
            Assert.AreEqual("'" + ExpectedResult + "'", APFILL, "Patient portal optin saved during registration activity");
        }
        [Test]
        public void TestUpdateColumnValues_PatientPortalOptIn_And_RightToRestrict_ShortRegistration(
            [Values("Y", " ")] string patientPortalOptIn,
            [Values("Y", " ")] string rightToRestrict,
            [Values("N", "N")] string HIEConsent,
            [Values("Y", "N")] string HIEPhysicianConsent)
        {
            string ExpectedResult = patientPortalOptIn + rightToRestrict + HIEConsent + HIEPhysicianConsent;
            var account = GetRegAccount(new ShortRegistrationActivity(), patientPortalOptIn, rightToRestrict, HIEConsent,
               HIEPhysicianConsent);
            string APFILL = GetColumnValue(APFILL_INDEX, account);
            Assert.AreEqual("'" + ExpectedResult + "'", APFILL, "Patient portal optin saved during shortregistration activity");
        }
        [Test]
        public void TestUpdateColumnValues_PatientPortalOptIn_Unable_Option_And_RightToRestrict_ShortRegistration(
            [Values("U", " ")] string patientPortalOptIn,
            [Values("Y", " ")] string rightToRestrict,
            [Values("N", "N")] string HIEConsent,
            [Values("Y", "N")] string HIEPhysicianConsent)
        {
            string ExpectedResult = patientPortalOptIn + rightToRestrict + HIEConsent + HIEPhysicianConsent;
            var account = GetRegAccount(new ShortRegistrationActivity(), patientPortalOptIn, rightToRestrict, HIEConsent,
                HIEPhysicianConsent);
            string APFILL = GetColumnValue(APFILL_INDEX, account);
            Assert.AreEqual("'" + ExpectedResult + "'", APFILL, "Patient portal optin saved during shortregistration activity");
        }
        [Test]
        public void TestUpdateColumnValues_CptCodes_For_Registration()
        {
            var facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            var facilityAco = facilityBroker.FacilityWith(FACILITY_ID);

            var patientSex = new Gender(2, DateTime.Now, "Male", "M");
            var patientDob = new DateTime(1965, 01, 13);
            var patient = new Patient
            {
                Oid = 1723,
                Facility = facilityAco,
                FirstName = "BRUCE",
                LastName = "LEEEE",
                DateOfBirth = patientDob,
                Sex = patientSex,
                MedicalRecordNumber = 785138
            };

            var proxy = new AccountProxy(30015,
                patient,
                DateTime.Now,
                DateTime.Now,
                new VisitType(0, ReferenceValue.NEW_VERSION, VisitType.INPATIENT_DESC, VisitType.INPATIENT),
                facilityAco,
                new FinancialClass(299, ReferenceValue.NEW_VERSION, "MEDICARE", "40"),
                new HospitalService(0, ReferenceValue.NEW_VERSION, "DIAGNOSTIC OUTOPT", "60"),
                "OL HSV60",
                false);

            var accountBroker = BrokerFactory.BrokerOfType<IAccountBroker>();
            var account = accountBroker.AccountFor(proxy);
            account.Activity = new RegistrationActivity();
            account.CptCodes.Add(1,"aaaaa");
            account.CptCodes.Add(3,"bbbbb");
            account.CptCodes.Add(5, "ccccc");
            account.CptCodes.Add(10, "ddddd");
            account.PatientPortalOptIn = YesNoFlag.Yes;
            account.RightToRestrict = YesNoFlag.Yes;
            i_PatientInsertStrategy = SqlBuilderStrategyFactory.CreatePatientInsertStrategy();
            i_PatientInsertStrategy.UpdateColumnValuesUsing(account);
            var sqlStrings = i_PatientInsertStrategy.BuildSqlFrom(account, transactionKeys);
            var sqlString = sqlStrings[0] as string;
            
            Assert.IsNotNull( sqlString , "Sql string to save CPT Codes is null during registration activity");
            
            string[] colArray = GetColumnArray( sqlString );
            string cptCodes = colArray[APHCPCCOD_INDEX];
            
            Assert.AreEqual("'" + CPT_CODES_STRING + "'", cptCodes, "CPT Codes not saved in required format during registration activity");
            
       }

        #endregion

        #region Support Methods
        private ArrayList BuildSql( Account anAccount )
        {
            i_PatientInsertStrategy = SqlBuilderStrategyFactory.CreatePatientInsertStrategy();
            i_PatientInsertStrategy.TransactionFileId = "PG";
            i_PatientInsertStrategy.PreDischargeFlag = "D";
            i_PatientInsertStrategy.UserSecurityCode = "KEVN";
            return i_PatientInsertStrategy.BuildSqlFrom( anAccount, transactionKeys );
        }
        private static string[] ValueArray( string sqlString )
        {
            var startPositionOfValues =
                sqlString.IndexOf( "(", sqlString.IndexOf( ")" ) ) + 1;
            var lengthOfValues =
                sqlString.Length - sqlString.IndexOf( "(", sqlString.IndexOf( ")" ) ) - 2;
            var valueArray =
                sqlString.Substring( startPositionOfValues, lengthOfValues ).Split( ',' );
            return valueArray;
        }

        private Account GetAccount()
        {

            Activity currentActivity =
                new PreDischargeActivity();
            var patient = new Patient(
           PATIENT_OID,
           PersistentModel.NEW_VERSION,
           PATIENT_NAME,
           PATIENT_MRN,
           PATIENT_DOB,
           PATIENT_SSN,
           PATIENT_SEX,
           facility
           );
            var visitType = new VisitType(
                PersistentModel.NEW_OID, PersistentModel.NEW_VERSION, VisitType.INPATIENT_DESC, VisitType.INPATIENT );
            var anAccount = new Account
                                {
                                    AccountNumber = 23456789,
                                    AdmitDate = DateTime.Parse( "Dec 17, 2001" ),
                                    DischargeDate = DateTime.MinValue,
                                    DerivedVisitType = "INPATENT",
                                    KindOfVisit = visitType,
                                    Patient =   patient,
                                    Activity = currentActivity,
                                    Facility = facility,
                                    HospitalService = hsp
                                };
            anAccount.ClergyVisit.SetYes( "YES" );
            return anAccount;
        }
        private Account GetPreRegAccount()
        {

            Activity currentActivity =
                new PreRegistrationActivity();
            var patient = new Patient(
           PATIENT_OID,
           PersistentModel.NEW_VERSION,
           PATIENT_NAME,
           PATIENT_MRN,
           PATIENT_DOB,
           PATIENT_SSN,
           PATIENT_SEX,
           facility
           );
            var visitType = new VisitType(
                PersistentModel.NEW_OID, PersistentModel.NEW_VERSION, VisitType.PREREG_PATIENT_DESC, VisitType.PREREG_PATIENT );
            var anAccount = new Account
            {
                AccountNumber = 23456789,
                AdmitDate = DateTime.Parse( "Dec 17, 2001" ),
                DischargeDate = DateTime.MinValue,
                DerivedVisitType = "INPATENT",
                KindOfVisit = visitType,
                Patient = patient,
                Activity = currentActivity,
                Facility = facility,
                HospitalService = hsp
            };
            anAccount.ClergyVisit.SetYes( "YES" );
            return anAccount;
        }
        private string[] GetColumnArray(string sqlString)
        {
            sqlString = sqlString.Replace("INSERT INTO HPADAPMP (", string.Empty);
            int startPositionOfValues = sqlString.IndexOf("VALUES (");
            return sqlString.Substring(startPositionOfValues, sqlString.Length - startPositionOfValues - 1).Split(',');
        }

        private Account GetRegAccount(Activity activity, string patientPortalFlag, string rightToRestrictFlag,
            string shareDataWithPublicHIEFlag, string shareDataWithPCPFlag)
        {
            var facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            var facilityAco = facilityBroker.FacilityWith(FACILITY_ID);

            var patientSex = new Gender(2, DateTime.Now, "Male", "M");
            var patientDob = new DateTime(1965, 01, 13);
            var patient = new Patient
            {
                Oid = 1723,
                Facility = facilityAco,
                FirstName = "SONNY",
                LastName = "SADSTORY",
                DateOfBirth = patientDob,
                Sex = patientSex,
                MedicalRecordNumber = 785138
            };

            var proxy = new AccountProxy(30015,
                patient,
                DateTime.Now,
                DateTime.Now,
                new VisitType(0, ReferenceValue.NEW_VERSION, VisitType.INPATIENT_DESC, VisitType.INPATIENT),
                facilityAco,
                new FinancialClass(299, ReferenceValue.NEW_VERSION, "MEDICARE", "40"),
                new HospitalService(0, ReferenceValue.NEW_VERSION, "DIAGNOSTIC OUTOPT", "60"),
                "OL HSV60",
                false);

            var accountBroker = BrokerFactory.BrokerOfType<IAccountBroker>();
            var account = accountBroker.AccountFor(proxy);
            account.Activity = activity;
            account.PatientPortalOptIn = new YesNoFlag( patientPortalFlag );
            account.RightToRestrict = new YesNoFlag( rightToRestrictFlag ) ;
            account.ShareDataWithPublicHieFlag = new YesNoFlag( shareDataWithPublicHIEFlag );
            account.ShareDataWithPCPFlag = new YesNoFlag( shareDataWithPCPFlag );
            return account;
        }

        private string GetColumnValue(int index, Account account)
        {
            var columnValue = String.Empty;
            i_PatientInsertStrategy = SqlBuilderStrategyFactory.CreatePatientInsertStrategy();
            i_PatientInsertStrategy.UpdateColumnValuesUsing(account);

            var sqlStrings = i_PatientInsertStrategy.BuildSqlFrom(account, transactionKeys);
            var sqlString = sqlStrings[0] as string;
            if (sqlString != null)
            {
                sqlString = sqlString.Replace("INSERT INTO HPADAPMP (", string.Empty);
                int startPositionOfValues = sqlString.IndexOf("VALUES (");
                string[] colArray =
                    sqlString.Substring(startPositionOfValues, sqlString.Length - startPositionOfValues - 1).Split(',');
                columnValue = colArray[index];
            }
            return columnValue;
        }

        #endregion

        #region Data Elements

        private object stringTestObject;
        private object integerTestObject;

        private OrderedList testOrderedList;

        private SocialSecurityNumber
            PATIENT_SSN = new SocialSecurityNumber( "123121234" );
        private Name
            PATIENT_NAME = new Name( PATIENT_F_NAME, PATIENT_L_NAME, PATIENT_MI );
        private DateTime
            PATIENT_DOB = new DateTime( 1955, 3, 5 );
        private Gender
            PATIENT_SEX = new Gender( 0, DateTime.Now, "Female", "F" );

        private static Facility facility = null;
        private static Facility facility2 = null;

        private PlaceOfWorship placeOfWorship = new PlaceOfWorship(
            PersistentModel.NEW_OID, PersistentModel.NEW_VERSION, "St. Mary's", "2" );
        private Ethnicity ethnicity = new Ethnicity(
            PersistentModel.NEW_OID, PersistentModel.NEW_VERSION, "HISPANIC", "3" );
        private Ethnicity ethnicity2 = new Ethnicity(
            PersistentModel.NEW_OID, PersistentModel.NEW_VERSION, "NON-HISPANIC", "2");
        private Ethnicity descent = new Ethnicity(
            PersistentModel.NEW_OID, PersistentModel.NEW_VERSION, "PUERTO RICAN", "106");
        private Ethnicity descent2 = new Ethnicity(
            PersistentModel.NEW_OID, PersistentModel.NEW_VERSION, "SOUTH AMERICAN", "104");

        private Race race = new Race(
            PersistentModel.NEW_OID, PersistentModel.NEW_VERSION, "WHITE", "1" );
        private Race nationality = new Race(
            PersistentModel.NEW_OID, PersistentModel.NEW_VERSION, "MID EAST NOR AFR", "102");
        private Race race2 = new Race(
            PersistentModel.NEW_OID, PersistentModel.NEW_VERSION, "BLACK", "2");
        private Race nationality2 = new Race(
            PersistentModel.NEW_OID, PersistentModel.NEW_VERSION, "BAHAMIAN", "204");
        private Religion religion = new Religion(
            PersistentModel.NEW_OID, PersistentModel.NEW_VERSION, "JEWISH", string.Empty );
        private Language language = new Language(
            PersistentModel.NEW_OID, PersistentModel.NEW_VERSION, "YIDDISH", "1" );
        private MaritalStatus maritalStatus = new MaritalStatus(
            PersistentModel.NEW_OID, PersistentModel.NEW_VERSION, "MARRIED", "1" );

        private Employment employment;

        private EmailAddress emailAddress = new EmailAddress( "FC@YAHOO.COM" );

        private PhoneNumber phoneNumberE = new PhoneNumber( "972", "546789" );
        private PhoneNumber phoneNumberM = new PhoneNumber( "972", "553453" );
        private PhoneNumber phoneNumberP = new PhoneNumber( "610", "546789" );

        private TypeOfContactPoint typeOfContactPointE =
            new TypeOfContactPoint( 1, "EMPLOYER" );
        private TypeOfContactPoint typeOfContactPointM =
            new TypeOfContactPoint( 0, "MAILING" );
        private TypeOfContactPoint typeOfContactPointP =
            new TypeOfContactPoint( 6, "PHYSICAL" );

        private HospitalService hsp = new HospitalService(
            PersistentModel.NEW_OID, DateTime.Now, "HSVC 60", "99" );

        private FinancialClass fin = new FinancialClass(
            PersistentModel.NEW_OID, DateTime.Now, "FIN CLASS 1", "1" );

        private object[] Occ = new object[2];
        private TransactionKeys transactionKeys = new TransactionKeys( 12, 24, 36, 0, 365 );
        private PatientInsertStrategy i_PatientInsertStrategy;

        private Address addressE = new Address( ADDRESS1E,
            ADDRESS2E,
            CITYE,
            new ZipCode( POSTALCODEE ),
            new State( PersistentModel.NEW_OID,
            ReferenceValue.NEW_VERSION,
            "TEXAS",
            "TX" ),
            new Country( PersistentModel.NEW_OID,
            ReferenceValue.NEW_VERSION,
            "United States",
            "USA" ),
            new County( PersistentModel.NEW_OID,
            ReferenceValue.NEW_VERSION,
            "ORANGE",
            "122" )
            );

        private Address addressMP = new Address( ADDRESS1MP,
            ADDRESS2MP,
            CITYMP,
            new ZipCode( POSTALCODEMP ),
            new State( PersistentModel.NEW_OID,
            ReferenceValue.NEW_VERSION,
            "TEXAS",
            "TX" ,
            "09"),
            new Country( PersistentModel.NEW_OID,
            ReferenceValue.NEW_VERSION,
            "United States",
            "USA" ),
            new County( PersistentModel.NEW_OID,
            ReferenceValue.NEW_VERSION,
            "ORANGE",
            "123" )
            );

        #endregion

        #region Constants

        private const int
            NUMBER_OF_ENTRIES = 623,
            APADMC_INDEX = 16,
            APFILL_INDEX = 583,
            APHCPCCOD_INDEX = 584;

        private const string
            TABLE_NAME = "HPDATA2.HPADAPMP",
            TESTSTRING = "PACCESS",
            EXPECTEDSTRING = "'PACCESS'",
            TESTVALUE = "34",

            ANOTHERSTRING = "AB34";

        private static readonly string BLANKSTRING = string.Empty;

        private const string
            PATIENT_F_NAME = "CLAIRE",
            PATIENT_L_NAME = "FRIED",
            PATIENT_MI = "R";

        private new const string FACILITY_CODE = "DEL";

        private const string
            FACILITY_CODE2 = "ACO";

        private const int
            TESTINTEGER = 999,
            EXPECTEDINTEGER = 999;

        private const long
            PATIENT_OID = 45L,
            PATIENT_MRN = 24004;

        private const string ADDRESS1MP = "234 MulHolland Drive",
            ADDRESS2MP = "#1",
            CITYMP = "Austin",
            POSTALCODEMP = "605940000";

        private const string ADDRESS1E = "335 Nicholson Dr.",
            ADDRESS2E = "#303",
            CITYE = "Austin",
            POSTALCODEE = "605030000";

        private const long FACILITY_ID = 900;
        private const long DHF_FACILITY_ID = 54;
        private const string HSPCODE = "ACO";
        private const string TEST_MRN = "785508";
        private const string
            TEST_IPA = "TST",
            TEST_IPA_CLINIC = "CL34P";

        private static readonly HospitalClinic clinicCode = new HospitalClinic
            ( 3, PersistentModel.NEW_VERSION ,"BLOOD TRANSFUSION" ,"AL" , string.Empty, "01" );

        private const string SITE_CODE = "SITE_CODE";
        private const string ADMITTING_CATEGORY_EMERGENCY = "2";
       
        private const string CPT_CODES_STRING = "aaaaa     bbbbb     ccccc                    ddddd";
        #endregion
    }
}

