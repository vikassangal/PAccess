using System;
using System.Collections;
using Extensions.PersistenceCommon;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Persistence;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    [TestFixture]
    public class InsuranceInsertStrategyTests : AbstractBrokerTests
    {
        #region SetUp and TearDown InsuranceInsertStrategyTests

        [TestFixtureSetUp]
        public static void SetUpInsuranceInsertStrategyTests()
        {
            var facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            facility_ACO = facilityBroker.FacilityWith( 900 );

            CreateUser();

            anAccount = new Account( 4477677 );
            anAccount.Facility = facility_ACO;

            anAccount.PreMSECopiedAccountNumber = 777777;
            anAccount.Activity = new PostMSERegistrationActivity();

            var patient = new Patient();
            patient.MedicalRecordNumber = 23456;

            InsurancePlan insurancePlan1 = new CommercialInsurancePlan();
            insurancePlan1.PlanName = "PRIMARY";
            insurancePlan1.EffectiveOn = DateTime.Parse( "Jan 21, 1998" );

            InsurancePlan insurancePlan2 = new SelfPayInsurancePlan();
            insurancePlan2.PlanName = "SECONDARY";
            insurancePlan2.EffectiveOn = DateTime.Parse( "Feb 21, 1998" );

            var primary = new CoverageOrder( 1, "Primary" );
            Assert.AreEqual(
                "Primary",
                primary.Description );

            var secondary = new CoverageOrder( 2, "Secondary" );
            Assert.AreEqual(
                "Secondary",
                secondary.Description );

            var coverage1 = new CommercialCoverage();
            coverage1.CoverageOrder = primary;
            coverage1.Oid = 1;

            var coverage2 =
                new GovernmentMedicareCoverage();
            coverage2.CoverageOrder = secondary;
            coverage2.Oid = 2;
            coverage2.RemainingPartADeductible = 5000;
            coverage2.RemainingPartBDeductible = 2000;

            var insurance = new Insurance();
            insurance.AddCoverage( coverage1 );
            Assert.AreEqual(
                1,
                insurance.Coverages.Count );

            insurance.AddCoverage( coverage2 );
            Assert.AreEqual(
                2,
                insurance.Coverages.Count );

            billingInformation[0] = new BillingInformation(
                addressE, phoneNumberE, null, typeOfContactPointE );
            billingInformation[1] = new BillingInformation(
                addressB, phoneNumberB, emailAddress, typeOfContactPointB );

            insurancePlan1.AddBillingInformation(
                (BillingInformation)billingInformation[0] );
            insurancePlan2.AddBillingInformation(
                (BillingInformation)billingInformation[1] );

            coverage1.InsurancePlan = insurancePlan1;
            coverage2.InsurancePlan = insurancePlan2;

            var insured1 = new Insured(
                PersistentModel.NEW_OID, PersistentModel.NEW_VERSION, INSURED_NAME1 );
            var insured2 = new Insured(
                PersistentModel.NEW_OID, PersistentModel.NEW_VERSION, INSURED_NAME2 );
            insured1.Sex = new Gender(
                PersistentModel.NEW_OID, PersistentModel.NEW_VERSION, "FEMALE", "F" );
            insured2.Sex = new Gender(
                PersistentModel.NEW_OID, PersistentModel.NEW_VERSION, "MALE", "M" );

            insured1.GroupNumber = "1";
            Assert.AreEqual(
                "1",
                insured1.GroupNumber,
                "GroupNumber should be 1" );

            insured2.GroupNumber = "2";

            coverage1.Insured = insured1;
            coverage2.Insured = insured2;

            var emp = new Employment( insured1 );
            emp.EmployeeID = "123456";
            emp.Status = new EmploymentStatus(
                PersistentModel.NEW_OID, DateTime.Now,
                "EMPLOYED FULL TIME", "1" );

            var empr = new Employer();
            empr.Name = "US POSTAL SERVICE";
            empr.EmployerCode = 1010;

            insured1.Employment.Employer = empr;

            var contactPointE = new ContactPoint(
                addressE, phoneNumberE, null, typeOfContactPointE );

            empr.PartyContactPoint = contactPointE;

            var physicalCP = new ContactPoint(
                addressE, phoneNumberE, null, TypeOfContactPoint.NewPhysicalContactPointType() );

            insured1.AddContactPoint( physicalCP );
            insured2.AddContactPoint( physicalCP );

            anAccount.Patient = patient;
            anAccount.Insurance = insurance;

            var patientSex = new Gender( 2, DateTime.Now, "Male", "M" );
            var patientDOB = new DateTime( 1965, 01, 13 );

            patient.Oid = 1723;
            patient.Facility.Oid = 900;
            patient.FirstName = "SONNY";
            patient.LastName = "SADSTORY";
            patient.DateOfBirth = patientDOB;
            patient.Sex = patientSex;
            patient.MedicalRecordNumber = 785138;
            var proxy = new AccountProxy( 30015,
                                         patient,
                                         DateTime.Now,
                                         DateTime.Now,
                                         new VisitType( 0, PersistentModel.NEW_VERSION, VisitType.OUTPATIENT_DESC,
                                                       VisitType.OUTPATIENT ),
                                         facility_ACO,
                                         new FinancialClass( 299, PersistentModel.NEW_VERSION, "MEDICARE", "40" ),
                                         new HospitalService( 0, PersistentModel.NEW_VERSION, "DIAGNOSTIC OUTOPT", "60" ),
                                         "OL HSV60",
                                         false );

            var accountBroker = BrokerFactory.BrokerOfType<IAccountBroker>();
            account = accountBroker.AccountFor( proxy );
            account.Activity = new PreRegistrationActivity();

            var anAccount1 = new Account();
            anAccount1.AccountNumber = 52704;
            var patient1 = new Patient();
            patient1.MedicalRecordNumber = 785749;
            anAccount1.Patient = patient1;
            anAccount1.Facility = facility_ACO;
            var mspBroker = BrokerFactory.BrokerOfType<IMSPBroker>();
            MedicareSecondaryPayor msp = mspBroker.MSPFor( anAccount1 );

            var empl = new Employment();
            empl.Employer = new Employer( 1L, DateTime.Now, "PerotSystems", "001", 100 );
            empl.EmployeeID = "234";
            empl.PhoneNumber = new PhoneNumber( "9725770000" );
            empl.Occupation = "Software Engineer";
            empl.Status.Code = "1";

            msp.MedicareEntitlement.PatientEmployment.Status.Code = "5";
            msp.MedicareEntitlement.SpouseEmployment = empl;
            anAccount.MedicareSecondaryPayor = msp;
        }

        [TestFixtureTearDown]
        public static void TearDownInsuranceInsertStrategyTests()
        {
        }

        #endregion

        #region Test Methods

        [Test]
        public void TestUpdateColumnValuesUsingForPrimaryCoverage()
        {
            i_InsuranceInsertStrategy = new
                InsuranceInsertStrategy( PRIMARY_COVERAGE );

            i_InsuranceInsertStrategy.UpdateColumnValuesUsing( account );

            ArrayList sqlStrings =
                i_InsuranceInsertStrategy.BuildSqlFrom( account, transactionKeys );
            foreach ( string sqlString in sqlStrings )
            {
                int startPositionOfValues =
                    sqlString.IndexOf( "(", sqlString.IndexOf( ")" ) ) + 1;
                int lengthOfValues =
                    sqlString.Length - sqlString.IndexOf( "(", sqlString.IndexOf( ")" ) ) - 2;
                string[] ValueArray =
                    sqlString.Substring( startPositionOfValues, lengthOfValues ).Split( ',' );

                Assert.AreEqual( NUMBER_OF_ENTRIES, ValueArray.Length, "Wrong number of entries in ValueArray" );
            }
        }

        [Test]
        public void TestInitializeColumnValuesForPrimaryCoverage()
        {
            try
            {
                i_InsuranceInsertStrategy = new InsuranceInsertStrategy( PRIMARY_COVERAGE );
                Assert.IsTrue( true, "Initialization of hashtable with default values succeeded" );
            }
            catch ( Exception ex )
            {
                Assert.Fail( "Initialization of hashtable with default values failed." );
                throw new BrokerException( ex.Message );
            }
        }

        [Test]
        public void TestUpdateColumnValuesUsingForSecondaryCoverage()
        {
            i_InsuranceInsertStrategy = new InsuranceInsertStrategy( SECONDARY_COVERAGE );

            i_InsuranceInsertStrategy.UpdateColumnValuesUsing( account );

            Assert.IsTrue( true, "Update of hashtable with values succeeded" );

            ArrayList sqlStrings =
                i_InsuranceInsertStrategy.BuildSqlFrom( account, transactionKeys );
            foreach ( string sqlString in sqlStrings )
            {
                int startPositionOfValues =
                    sqlString.IndexOf( "(", sqlString.IndexOf( ")" ) ) + 1;
                int lengthOfValues =
                    sqlString.Length - sqlString.IndexOf( "(", sqlString.IndexOf( ")" ) ) - 2;
                string[] ValueArray =
                    sqlString.Substring( startPositionOfValues, lengthOfValues ).Split( ',' );

                Assert.AreEqual( NUMBER_OF_ENTRIES, ValueArray.Length, "Wrong number of entries in ValueArray" );
            }
        }

        [Test]
        public void TestInitializeColumnValuesForSecondaryCoverage()
        {
            try
            {
                i_InsuranceInsertStrategy = new InsuranceInsertStrategy( SECONDARY_COVERAGE );
                Assert.IsTrue( true, "Initialization of hashtable with default values succeeded" );
            }
            catch ( Exception ex )
            {
                Assert.Fail( "Initialization of hashtable with default values failed." );
                throw new BrokerException( ex.Message );
            }
        }

        [Test]
        public void TestBuildSqlFromForPrimary()
        {
            i_InsuranceInsertStrategy = new
                InsuranceInsertStrategy( PRIMARY_COVERAGE );
            i_InsuranceInsertStrategy.UserSecurityCode = "KEVN";
            i_InsuranceInsertStrategy.PreRegistrationFlag = "P";
            i_InsuranceInsertStrategy.OrignalTransactionId = "PC";
            ArrayList sqlStrings =
                i_InsuranceInsertStrategy.BuildSqlFrom( anAccount, transactionKeys );
            foreach ( string sqlString in sqlStrings )
            {
                int startPositionOfValues =
                    sqlString.IndexOf( "(", sqlString.IndexOf( ")" ) ) + 1;
                int lengthOfValues =
                    sqlString.Length - sqlString.IndexOf( "(", sqlString.IndexOf( ")" ) ) - 2;
                string[] ValueArray =
                    sqlString.Substring( startPositionOfValues, lengthOfValues ).Split( ',' );

                Assert.AreEqual( NUMBER_OF_ENTRIES, ValueArray.Length, "Wrong number of entries in ValueArray" );

                Assert.AreEqual( " ''", ValueArray[0], "Value of APIDWS should be ''" );
                Assert.AreEqual( "'ID'", ValueArray[1], "Value of APIDID should be 'ID' " );
                Assert.AreEqual( "'&*%P@#$'", ValueArray[2], "Value of APRR# should be '&*%P@#$' " );
                Assert.AreEqual( "'KEVN'", ValueArray[3], "Value of APSEC2 should be 'KEVN' " );
                Assert.AreEqual( "900", ValueArray[4], "Value of APHSP# should be 900 " );
                Assert.AreEqual( "4477677", ValueArray[5], "Value of APACCT should be 4477677 " );
                Assert.AreEqual( "1", ValueArray[6], "Value of APSEQ# should be 1 " );
                Assert.AreEqual( "4477677", ValueArray[7], "Value of APGAR# should be 0 " );
                Assert.AreEqual( "'0'", ValueArray[8], "Value of APPTY should be '0' " );
                if ( anAccount.Activity != null )
                {
                    if ( anAccount.Activity.GetType().Equals( typeof( PreMSERegisterActivity ) ) )
                    {
                        Assert.AreEqual( "0", ValueArray[9], "Value of APPLAN should be 0 " );
                    }
                    else
                    {
                        Assert.AreEqual( "1", ValueArray[9], "Value of APPLAN should be 1 " );
                    }
                }
                Assert.AreEqual( "''", ValueArray[10], "Value of APBCFL should be '' " );
                Assert.AreEqual( "''", ValueArray[11], "Value of APCSPL should be '' " );
                Assert.AreEqual( "''", ValueArray[12], "Value of APINM should be '' " );
                Assert.AreEqual( "''", ValueArray[13], "Value of APGNM should be '' " );
                Assert.AreEqual( "''", ValueArray[14], "Value of APIAD1 should be '' " );
                Assert.AreEqual( "'                    '", ValueArray[15],
                                "Value of APIAD2 should be '                   ' " );
                Assert.AreEqual( "'                   '", ValueArray[16], "Value of APIAD3 should be '' " );
                Assert.AreEqual( "''", ValueArray[17], "Value of APVST should be '' " );
                Assert.AreEqual( "''", ValueArray[18], "Value of APPROR should be '' " );
                Assert.AreEqual( "''", ValueArray[19], "Value of APLAST should be '' " );
                Assert.AreEqual( "''", ValueArray[20], "Value of APVBV should be '' " );
                Assert.AreEqual( "0", ValueArray[21], "Value of APSDAT should be 0 " );
                Assert.AreEqual( "''", ValueArray[22], "Value of AP1FLG should be '' " );
                Assert.AreEqual( "0", ValueArray[23], "Value of AP1DED should be 0 " );
                Assert.AreEqual( "0", ValueArray[24], "Value of AP1$ should be 0 " );
                Assert.AreEqual( "0", ValueArray[25], "Value of AP1PCT should be 0 " );
                Assert.AreEqual( "0", ValueArray[26], "Value of AP1DYS should be 0 " );
                Assert.AreEqual( "''", ValueArray[27], "Value of AP2FLG should be '' " );
                Assert.AreEqual( "0", ValueArray[28], "Value of AP2DED should be 0 " );
                Assert.AreEqual( "0", ValueArray[29], "Value of AP2PCT should be 0 " );
                Assert.AreEqual( "0", ValueArray[30], "Value of AP2MX$ should be 0 " );
                Assert.AreEqual( "0", ValueArray[31], "Value of AP2DR$ should be 0 " );
                Assert.AreEqual( "0", ValueArray[32], "Value of AP2DID should be 0 " );
                Assert.AreEqual( "0", ValueArray[33], "Value of AP2IR$ should be 0 " );
                Assert.AreEqual( "0", ValueArray[34], "Value of AP2IRD should be 0 " );
                Assert.AreEqual( "''", ValueArray[35], "Value of AP3FLG should be '' " );
                Assert.AreEqual( "0", ValueArray[36], "Value of AP3DED should be 0 " );
                Assert.AreEqual( "0", ValueArray[37], "Value of AP3PCT should be 0 " );
                Assert.AreEqual( "0", ValueArray[38], "Value of AP3$ should be 0 " );
                Assert.AreEqual( "''", ValueArray[39], "Value of AP4FLG should be '' " );
                Assert.AreEqual( "0", ValueArray[40], "Value of AP4DED should be 0 " );
                Assert.AreEqual( "0", ValueArray[41], "Value of AP4PCT should be 0 " );
                Assert.AreEqual( "0", ValueArray[42], "Value of AP4$ should be 0 " );
                Assert.AreEqual( "0", ValueArray[43], "Value of AP4MR$ should be 0 " );
                Assert.AreEqual( "0", ValueArray[44], "Value of AP4MRD should be 0 " );
                Assert.AreEqual( "0", ValueArray[45], "Value of AP4MI$ should be 0 " );
                Assert.AreEqual( "0", ValueArray[46], "Value of AP4MID should be 0 " );
                Assert.AreEqual( "''", ValueArray[47], "Value of AP5FLG should be '' " );
                Assert.AreEqual( "0", ValueArray[48], "Value of AP5DED should be 0 " );
                Assert.AreEqual( "0", ValueArray[49], "Value of AP5PCT should be 0 " );
                Assert.AreEqual( "0", ValueArray[50], "Value of AP5$ should be 0 " );
                Assert.AreEqual( "0", ValueArray[51], "Value of AP5MR$ should be 0 " );
                Assert.AreEqual( "0", ValueArray[52], "Value of AP5MRD should be 0 " );
                Assert.AreEqual( "0", ValueArray[53], "Value of AP5MI$ should be 0 " );
                Assert.AreEqual( "0", ValueArray[54], "Value of AP5MID should be 0 " );
                Assert.AreEqual( "''", ValueArray[55], "Value of AP6DBM should be '' " );
                Assert.AreEqual( "''", ValueArray[56], "Value of AP6XMM should be '' " );
                Assert.AreEqual( "''", ValueArray[57], "Value of AP6RBM should be '' " );
                Assert.AreEqual( "''", ValueArray[58], "Value of AP6DM should be '' " );
                Assert.AreEqual( "''", ValueArray[59], "Value of APCOB should be '' " );
                Assert.AreEqual( "0", ValueArray[60], "Value of APEX01 should be 0 " );
                Assert.AreEqual( "0", ValueArray[61], "Value of APEX02 should be 0 " );
                Assert.AreEqual( "0", ValueArray[62], "Value of APEX03 should be 0 " );
                Assert.AreEqual( "0", ValueArray[63], "Value of APEX04 should be 0 " );
                Assert.AreEqual( "0", ValueArray[64], "Value of APEX05 should be 0 " );
                Assert.AreEqual( "0", ValueArray[65], "Value of APEX06 should be 0 " );
                Assert.AreEqual( "0", ValueArray[66], "Value of APEX07 should be 0 " );
                Assert.AreEqual( "0", ValueArray[67], "Value of APEX08 should be 0 " );
                Assert.AreEqual( "0", ValueArray[68], "Value of APEX09 should be 0 " );
                Assert.AreEqual( "0", ValueArray[69], "Value of APEX10 should be 0 " );
                Assert.AreEqual( "0", ValueArray[70], "Value of APEX11 should be 0 " );
                Assert.AreEqual( "0", ValueArray[71], "Value of APEX12 should be 0 " );
                Assert.AreEqual( "0", ValueArray[72], "Value of APEX13 should be 0 " );
                Assert.AreEqual( "0", ValueArray[73], "Value of APEX14 should be 0 " );
                Assert.AreEqual( "0", ValueArray[74], "Value of APEX15 should be 0 " );
                Assert.AreEqual( "0", ValueArray[75], "Value of APEX16 should be 0 " );
                Assert.AreEqual( "0", ValueArray[76], "Value of APEX17 should be 0 " );
                Assert.AreEqual( "0", ValueArray[77], "Value of APEX18 should be 0 " );
                Assert.AreEqual( "0", ValueArray[78], "Value of APEX19 should be 0 " );
                Assert.AreEqual( "0", ValueArray[79], "Value of APEX20 should be 0 " );
                Assert.AreEqual( "0", ValueArray[80], "Value of APFDAY should be 0 " );
                Assert.AreEqual( "0", ValueArray[81], "Value of APCDAY should be 0 " );
                Assert.AreEqual( "0", ValueArray[82], "Value of APCDOL should be 0 " );
                Assert.AreEqual( "0", ValueArray[83], "Value of APLDAY should be 0 " );
                Assert.AreEqual( "0", ValueArray[84], "Value of APLDOL should be 0 " );
                Assert.AreEqual( "0", ValueArray[85], "Value of APBDPT should be 0 " );
                Assert.AreEqual( "0", ValueArray[86], "Value of APBD$ should be 0 " );
                Assert.AreEqual( "0", ValueArray[87], "Value of APPMET should be 0 " );
                Assert.AreEqual( "0", ValueArray[88], "Value of APPM$ should be 0 " );
                Assert.AreEqual( "0", ValueArray[89], "Value of APPMPR should be 0 " );
                Assert.AreEqual( "''", ValueArray[90], "Value of APMCRE should be '' " );
                Assert.AreEqual( "''", ValueArray[91], "Value of APMAID should be '' " );
                Assert.AreEqual( "0", ValueArray[92], "Value of APMDED should be 0 " );
                Assert.AreEqual( "0", ValueArray[93], "Value of APLML should be 0 " );
                Assert.AreEqual( "0", ValueArray[94], "Value of APLMD should be 0 " );
                Assert.AreEqual( "0", ValueArray[95], "Value of APLUL# should be 0 " );
                Assert.AreEqual( "'C'", ValueArray[96], "Value of APACFL should be 'C' " );
                Assert.AreEqual( "'$#L@%'", ValueArray[98], "Value of APINLG should be '$#L@%' " );
                Assert.AreEqual( "''", ValueArray[99], "Value of APBYPS should be '' " );
                Assert.AreEqual( "0", ValueArray[100], "Value of APSWPY should be 0 " );
                Assert.AreEqual( "''", ValueArray[101], "Value of APABCF should be '' " );
                Assert.AreEqual( "''", ValueArray[102], "Value of APRICF should be '' " );
                Assert.AreEqual( "'SADSTORY'", ValueArray[103], "Value of APSLNM should be 'SADSTORY' " );
                Assert.AreEqual( "'HAPPY'", ValueArray[104], "Value of APSFNM should be 'HAPPY' " );
                Assert.AreEqual( "'F'", ValueArray[105], "Value of APSSEX should be 'F' " );
                Assert.AreEqual( "''", ValueArray[106], "Value of APSRCD should be '' " );
                Assert.AreEqual( "''", ValueArray[107], "Value of APIID# should be '' " );
                Assert.AreEqual( "''", ValueArray[108], "Value of APGRPN should be '' " );
                Assert.AreEqual( "''", ValueArray[109], "Value of APESCD should be '' " );
                Assert.AreEqual( "''", ValueArray[110], "Value of APSBEN should be '' " );
                Assert.AreEqual( "''", ValueArray[111], "Value of APEEID should be '' " );
                Assert.AreEqual( "''", ValueArray[112], "Value of APSBEL should be '' " );
                Assert.AreEqual( "''", ValueArray[113],
                                "Value of APJADR should be '' " );
                Assert.AreEqual( "'Austin'", ValueArray[114], "Value of APJCIT should be 'Austin' " );
                Assert.AreEqual( "'TX'", ValueArray[115], "Value of APJSTE should be 'TX' " );
                Assert.AreEqual( "0", ValueArray[116], "Value of APJZIP should be 0 " );
                Assert.AreEqual( "0", ValueArray[117], "Value of APJZP4 should be 0 " );
                Assert.AreEqual( "972", ValueArray[118], "Value of APJACD should be 972 " );
                Assert.AreEqual( "546789", ValueArray[119], "Value of APJPH# should be 546789 " );
                Assert.AreEqual( "''", ValueArray[120], "Value of APNEIC should be '' " );
                Assert.AreEqual( "0", ValueArray[121], "Value of APDOV should be 0 " );
                Assert.AreEqual( "''", ValueArray[122], "Value of APELGS should be '' " );
                Assert.AreEqual( "0", ValueArray[123], "Value of APTDOC should be 0 " );
                Assert.AreEqual( "0", ValueArray[124], "Value of APELDT should be 0 " );
                Assert.AreEqual( "''", ValueArray[125], "Value of APSCHB should be '' " );
                Assert.AreEqual( "''", ValueArray[126], "Value of APVC01 should be '' " );
                Assert.AreEqual( "''", ValueArray[127], "Value of APVC02 should be '' " );
                Assert.AreEqual( "''", ValueArray[128], "Value of APVC03 should be '' " );
                Assert.AreEqual( "''", ValueArray[129], "Value of APVC04 should be '' " );
                Assert.AreEqual( "0", ValueArray[130], "Value of APVA01 should be 0 " );
                Assert.AreEqual( "0", ValueArray[131], "Value of APVA02 should be 0 " );
                Assert.AreEqual( "0", ValueArray[132], "Value of APVA03 should be 0 " );
                Assert.AreEqual( "0", ValueArray[133], "Value of APVA04 should be 0 " );
                Assert.AreEqual( "0", ValueArray[134], "Value of APAGNY should be 0 " );
                Assert.AreEqual( "0", ValueArray[135], "Value of APCHAM should be 0 " );
                Assert.AreEqual( "0", ValueArray[136], "Value of APCHAP should be 0 " );
                Assert.AreEqual( "''", ValueArray[137], "Value of APMSPC should be '' " );
                Assert.AreEqual( "0", ValueArray[138], "Value of APDBEN should be 0 " );
                Assert.AreEqual( "0", ValueArray[139], "Value of APDURR should be 0 " );
                Assert.AreEqual( "0", ValueArray[140], "Value of APACRE should be 0 " );
                Assert.AreEqual( "0", ValueArray[141], "Value of APDGPS should be 0 " );
                Assert.AreEqual( "''", ValueArray[142], "Value of APNEPS should be '' " );
                Assert.AreEqual( "0", ValueArray[143], "Value of APNEPI should be 0 " );
                Assert.AreEqual( "''", ValueArray[144], "Value of APITYP should be '' " );
                Assert.AreEqual( "0", ValueArray[145], "Value of APRIPL should be 0 " );
                Assert.AreEqual( "''", ValueArray[146], "Value of APCBFL should be '' " );
                Assert.AreEqual( "''", ValueArray[147], "Value of APINEX should be '' " );
                Assert.AreEqual( "0", ValueArray[148], "Value of APSURP should be 0 " );
                Assert.AreEqual( "0", ValueArray[149], "Value of APSURA should be 0 " );
                Assert.AreEqual( "''", ValueArray[150], "Value of APINS# should be '' " );
                Assert.AreEqual( "''", ValueArray[151], "Value of APCNFC should be '' " );
                Assert.AreEqual( "''", ValueArray[152], "Value of APPFCC should be '' " );
                Assert.AreEqual( "''", ValueArray[153], "Value of APRICO should be '' " );
                Assert.AreEqual( "'US POSTAL SERVICE'", ValueArray[154], "Value of APENM1 should be 'US POSTAL SERVICE' " );
                Assert.AreEqual( "'Austin          TX'", ValueArray[155],
                                "Value of APELO1 should be 'Austin          TX' " );
                Assert.AreEqual( "''", ValueArray[156], "Value of APEDC1 should be '' " );
                Assert.AreEqual( "''", ValueArray[157], "Value of APESC1 should be '' " );
                Assert.AreEqual( "'972546789'", ValueArray[158], "Value of APEID1 should be '972546789' " );
                Assert.AreEqual( "'234 MulHolland Drive'", ValueArray[159],
                                "Value of APEA01 should be '234 MulHolland Drive' " );
                Assert.AreEqual( "0", ValueArray[160], "Value of APEZ01 should be 0 " );
                Assert.AreEqual( "''", ValueArray[161], "Value of APENM2 should be '' " );
                Assert.AreEqual( "''", ValueArray[162], "Value of APELO2 should be '' " );
                Assert.AreEqual( "''", ValueArray[163], "Value of APEDC2 should be '' " );
                Assert.AreEqual( "''", ValueArray[164], "Value of APESC2 should be '' " );
                Assert.AreEqual( "'00000000000'", ValueArray[165], "Value of APEID2 should be '00000000000' " );
                Assert.AreEqual( "''", ValueArray[166], "Value of APEA02 should be '' " );
                Assert.AreEqual( "0", ValueArray[167], "Value of APEZ02 should be 0 " );
                Assert.AreEqual( "''", ValueArray[168], "Value of APDEDU should be '' " );
                Assert.AreEqual( "''", ValueArray[169], "Value of APBLDE should be '' " );
                Assert.AreEqual( "''", ValueArray[170], "Value of APINDY should be '' " );
                Assert.AreEqual( "''", ValueArray[171], "Value of APLIFD should be '' " );
                Assert.AreEqual( "''", ValueArray[172], "Value of APNCVD should be '' " );
                Assert.AreEqual( "''", ValueArray[174], "Value of APCLRK should be '' " );
                Assert.AreEqual( "''", ValueArray[175], "Value of APZDTE should be '' " );
                Assert.AreEqual( "''", ValueArray[176], "Value of APZTME should be '' " );
                Assert.AreEqual( "''", ValueArray[177], "Value of APPNID should be '' " );
                Assert.AreEqual( "'PRIMARY'", ValueArray[178], "Value of APP#NM should be 'PRIMARY' " );
                Assert.AreEqual( "980121", ValueArray[179], "Value of APCBGD should be 980121 " );
                Assert.AreEqual( "0", ValueArray[180], "Value of APPLAD should be 0 " );
                Assert.AreEqual( "0", ValueArray[182], "Value of APSTCP should be 0 " );
                Assert.AreEqual( "0", ValueArray[183], "Value of APSTAM should be 0 " );
                Assert.AreEqual( "0", ValueArray[184], "Value of APST$L should be 0 " );
                Assert.AreEqual( "0", ValueArray[185], "Value of APSTPA should be 0 " );
                Assert.AreEqual( "''", ValueArray[186], "Value of APAUTF should be '' " );
                Assert.AreEqual( "''", ValueArray[187], "Value of APAUWV should be '' " );
                Assert.AreEqual( "0", ValueArray[188], "Value of APAUDY should be 0 " );
                Assert.AreEqual( "''", ValueArray[189], "Value of AP2OPF should be '' " );
                Assert.AreEqual( "''", ValueArray[190], "Value of AP2CMM should be '' " );
                Assert.AreEqual( "''", ValueArray[191], "Value of APCOPF should be '' " );
                Assert.AreEqual( "''", ValueArray[192], "Value of APDEDF should be '' " );
                Assert.AreEqual( "''", ValueArray[193], "Value of APBLNM should be '' " );
                Assert.AreEqual( "''", ValueArray[194], "Value of APOBCF should be '' " );
                Assert.AreEqual( "0", ValueArray[195], "Value of APOBCP should be 0 " );
                Assert.AreEqual( "0", ValueArray[196], "Value of APOBAM should be 0 " );
                Assert.AreEqual( "0", ValueArray[197], "Value of APOB$L should be 0 " );
                Assert.AreEqual( "0", ValueArray[198], "Value of APOBPA should be 0 " );
                Assert.AreEqual( "''", ValueArray[199], "Value of APNBCF should be '' " );
                Assert.AreEqual( "0", ValueArray[200], "Value of APNBCP should be 0 " );
                Assert.AreEqual( "0", ValueArray[201], "Value of APNBAM should be 0 " );
                Assert.AreEqual( "0", ValueArray[202], "Value of APNB$L should be 0 " );
                Assert.AreEqual( "0", ValueArray[203], "Value of APNBPA should be 0 " );
                Assert.AreEqual( "''", ValueArray[204], "Value of APCDCF should be '' " );
                Assert.AreEqual( "0", ValueArray[205], "Value of APCDCP should be 0 " );
                Assert.AreEqual( "0", ValueArray[206], "Value of APCDAM should be 0 " );
                Assert.AreEqual( "0", ValueArray[207], "Value of APCD$L should be 0 " );
                Assert.AreEqual( "0", ValueArray[208], "Value of APCDPA should be 0 " );
                Assert.AreEqual( "''", ValueArray[209], "Value of APDCCF should be '' " );
                Assert.AreEqual( "0", ValueArray[210], "Value of APDCCP should be 0 " );
                Assert.AreEqual( "0", ValueArray[211], "Value of APDCAM should be 0 " );
                Assert.AreEqual( "0", ValueArray[212], "Value of APDC$L should be 0 " );
                Assert.AreEqual( "0", ValueArray[213], "Value of APDCPA should be 0 " );
                Assert.AreEqual( "0", ValueArray[214], "Value of APDCAL should be 0 " );
                Assert.AreEqual( "0", ValueArray[215], "Value of APDADM should be 0 " );
                Assert.AreEqual( "''", ValueArray[216], "Value of APDDMF should be '' " );
                Assert.AreEqual( "0", ValueArray[217], "Value of APLFMX should be 0 " );
                Assert.AreEqual( "0", ValueArray[218], "Value of APMXAD should be 0 " );
                Assert.AreEqual( "''", ValueArray[219], "Value of APCOBF should be '' " );
                Assert.AreEqual( "''", ValueArray[220], "Value of APCOMM should be '' " );
                Assert.AreEqual( "''", ValueArray[221], "Value of APIFRF should be '' " );
                Assert.AreEqual( "0", ValueArray[222], "Value of APSUAC should be 0 " );
                Assert.AreEqual( "0", ValueArray[223], "Value of APSUPH should be 0 " );
                Assert.AreEqual( "''", ValueArray[224], "Value of APVFFG should be '' " );
                Assert.AreEqual( "''", ValueArray[225], "Value of APVFBY should be '' " );
                Assert.AreEqual( "0", ValueArray[226], "Value of APVFDT should be 0 " );
                Assert.AreEqual( "0", ValueArray[227], "Value of APAMDY should be 0 " );
                Assert.AreEqual( "0", ValueArray[228], "Value of APNCDY should be 0 " );
                Assert.AreEqual( "0", ValueArray[229], "Value of APELOS should be 0 " );
                Assert.AreEqual( "0", ValueArray[230], "Value of APRCDE should be 0 " );
                Assert.AreEqual( "0", ValueArray[231], "Value of APRSLC should be 0 " );
                Assert.AreEqual( "''", ValueArray[232], "Value of APUROP should be '' " );
                Assert.AreEqual( "''", ValueArray[233], "Value of APMSPF should be '' " );
                Assert.AreEqual( "''", ValueArray[234], "Value of APWVCP should be '' " );
                Assert.AreEqual( "101000000", ValueArray[235], "Value of APGCD1 should be 101000000 " );
                Assert.AreEqual( "0", ValueArray[236], "Value of APGCD2 should be 0 " );
                Assert.AreEqual( "''", ValueArray[237], "Value of APCONM should be '' " );
                Assert.AreEqual( "'60594'", ValueArray[238], "Value of APEZP1 should be '60594' " );
                Assert.AreEqual( "'4000'", ValueArray[239], "Value of APEZ41 should be '4000' " );
                Assert.AreEqual( "''", ValueArray[240], "Value of APEZP2 should be '' " );
                Assert.AreEqual( "'0000'", ValueArray[241], "Value of APEZ42 should be '0000' " );
                if ( anAccount.Activity != null )
                {
                    if ( anAccount.Activity.GetType().Equals( typeof( PreMSERegisterActivity ) ) )
                    {
                        Assert.AreEqual( "''", ValueArray[242], "Value of APJZPA should be '' " );
                    }
                    else
                    {
                        Assert.AreEqual( "'60594'", ValueArray[242], "Value of APJZPA should be '60594' " );
                    }
                }
                Assert.AreEqual( "'4000'", ValueArray[243], "Value of APJZ4A should be '4000' " );
                Assert.AreEqual( "'PC'", ValueArray[244], "Value of APOID should be 'PC' " );
                Assert.AreEqual( "'P'", ValueArray[245], "Value of APPRE should be 'P' " );
                Assert.AreEqual( "'000785138'", ValueArray[246], "Value of APMR# should be '000785138' " );
                Assert.AreEqual( "''", ValueArray[247], "Value of APOACT should be '' " );
                Assert.AreEqual( "''", ValueArray[248], "Value of APEVC# should be '' " );
                Assert.AreEqual( "''", ValueArray[249], "Value of APSMI should be '' " );
                Assert.AreEqual( "''", ValueArray[250], "Value of APSBID should be '' " );
                Assert.AreEqual( "'USA'", ValueArray[251], "Value of APSCUN should be 'USA' " );
                Assert.AreEqual( "''", ValueArray[252], "Value of APSBTH should be '' " );
                Assert.AreEqual( "''", ValueArray[253], "Value of APOILN should be '' " );
                Assert.AreEqual( "''", ValueArray[254], "Value of APOIFN should be '' " );
                Assert.AreEqual( "''", ValueArray[255], "Value of APOIMI should be '' " );
                Assert.AreEqual( "''", ValueArray[256], "Value of APOISX should be '' " );
                Assert.AreEqual( "''", ValueArray[257], "Value of APOIBD should be '' " );
                Assert.AreEqual( "''", ValueArray[258], "Value of APOIID should be '' " );
                Assert.AreEqual( "''", ValueArray[259], "Value of APOIAD should be '' " );
                Assert.AreEqual( "''", ValueArray[260], "Value of APOICT should be '' " );
                Assert.AreEqual( "''", ValueArray[261], "Value of APOIST should be '' " );
                Assert.AreEqual( "''", ValueArray[262], "Value of APOICN should be '' " );
                Assert.AreEqual( "''", ValueArray[263], "Value of APOIZP should be '' " );
                Assert.AreEqual( "'0000'", ValueArray[264], "Value of APOIZ4 should be '0000' " );
                Assert.AreEqual( "''", ValueArray[265], "Value of APICPH should be '' " );
                Assert.AreEqual( "''", ValueArray[266], "Value of APMCDT should be '' " );
                Assert.AreEqual( "0", ValueArray[267], "Value of APDAMT should be 0 " );
                Assert.AreEqual( "0", ValueArray[268], "Value of APCPAY should be 0 " );
                Assert.AreEqual( "0", ValueArray[269], "Value of APCINS should be 0 " );
                Assert.AreEqual( "0", ValueArray[270], "Value of APAMCL should be 0 " );
                Assert.AreEqual( "0", ValueArray[271], "Value of APPYCL should be 0 " );
                Assert.AreEqual( "0", ValueArray[272], "Value of APINCL should be 0 " );
                Assert.AreEqual( "0", ValueArray[273], "Value of APPVCL should be 0 " );
                Assert.AreEqual( "0", ValueArray[274], "Value of APUCBL should be 0 " );
                Assert.AreEqual( "0", ValueArray[275], "Value of APEDBL should be 0 " );
                Assert.AreEqual( "0", ValueArray[276], "Value of APUFPT should be 0 " );
                Assert.AreEqual( "0", ValueArray[277], "Value of APEFPT should be 0 " );
                Assert.AreEqual( "0", ValueArray[278], "Value of APNUM1 should be 0 " );
                Assert.AreEqual( "0", ValueArray[279], "Value of APNUM2 should be 0 " );
                Assert.AreEqual( "0", ValueArray[280], "Value of APNUM3 should be 0 " );
                Assert.AreEqual( "0", ValueArray[281], "Value of APMNPY should be 0 " );
                Assert.AreEqual( "0", ValueArray[282], "Value of APCOPY should be 0 " );
                Assert.AreEqual( "0", ValueArray[283], "Value of APCIPY should be 0 " );
                Assert.AreEqual( "0", ValueArray[284], "Value of APUCPY should be 0 " );
                Assert.AreEqual( "0", ValueArray[285], "Value of APEDPY should be 0 " );
                Assert.AreEqual( "''", ValueArray[286], "Value of APPDUE should be '' " );
                Assert.AreEqual( "''", ValueArray[287], "Value of APCRPT should be '' " );
                Assert.AreEqual( "''", ValueArray[288], "Value of APCRSC should be '' " );
                Assert.AreEqual( "''", ValueArray[289], "Value of APCR01 should be '' " );
                Assert.AreEqual( "''", ValueArray[290], "Value of APCR02 should be '' " );
                Assert.AreEqual( "''", ValueArray[291], "Value of APCR03 should be '' " );
                Assert.AreEqual( "''", ValueArray[292], "Value of APCR04 should be '' " );
                Assert.AreEqual( "''", ValueArray[293], "Value of APCR05 should be '' " );
                Assert.AreEqual( "''", ValueArray[294], "Value of APCR06 should be '' " );
                Assert.AreEqual( "''", ValueArray[295], "Value of APCR07 should be '' " );
                Assert.AreEqual( "''", ValueArray[296], "Value of APCR08 should be '' " );
                Assert.AreEqual( "''", ValueArray[297], "Value of APCR09 should be '' " );
                Assert.AreEqual( "''", ValueArray[298], "Value of APCR10 should be '' " );
                Assert.AreEqual( "''", ValueArray[299], "Value of APCR11 should be '' " );
                Assert.AreEqual( "''", ValueArray[300], "Value of APCR12 should be '' " );
                Assert.AreEqual( "''", ValueArray[301], "Value of APHIC# should be '' " );
                Assert.AreEqual( "''", ValueArray[302], "Value of APITR# should be '' " );
                Assert.AreEqual( "''", ValueArray[303], "Value of APAUTH# should be '' " );
                Assert.AreEqual( "''", ValueArray[304], "Value of APAUTFG should be '' " );
                Assert.AreEqual( "''", ValueArray[305], "Value of APAUCP should be '' " );
                Assert.AreEqual( "0", ValueArray[306], "Value of APAUPH should be 0 " );
                Assert.AreEqual( "''", ValueArray[307], "Value of APAUEX should be '' " );
                Assert.AreEqual( "''", ValueArray[308], "Value of APADNM should be '' " );
                Assert.AreEqual( "''", ValueArray[309], "Value of APEMSU should be '' " );
                Assert.AreEqual( "''", ValueArray[310], "Value of APATNM should be '' " );
                Assert.AreEqual( "''", ValueArray[311], "Value of APATST should be '' " );
                Assert.AreEqual( "''", ValueArray[312], "Value of APATCT should be '' " );
                Assert.AreEqual( "''", ValueArray[313], "Value of APATSA should be '' " );
                Assert.AreEqual( "''", ValueArray[314], "Value of APATZ5 should be '' " );
                Assert.AreEqual( "'0000'", ValueArray[315], "Value of APATZ4 should be '0000' " );
                Assert.AreEqual( "''", ValueArray[316], "Value of APATCC should be '' " );
                Assert.AreEqual( "0", ValueArray[317], "Value of APATPH should be 0 " );
                Assert.AreEqual( "''", ValueArray[318], "Value of APAGNM should be '' " );
                Assert.AreEqual( "0", ValueArray[319], "Value of APAGPH should be 0 " );
                Assert.AreEqual( "''", ValueArray[320], "Value of APAGST should be '' " );
                Assert.AreEqual( "''", ValueArray[321], "Value of APAGCT should be '' " );
                Assert.AreEqual( "''", ValueArray[322], "Value of APAGSA should be '' " );
                Assert.AreEqual( "''", ValueArray[323], "Value of APAGZ5 should be '' " );
                Assert.AreEqual( "'0000'", ValueArray[324], "Value of APAGZ4 should be '0000' " );
                Assert.AreEqual( "''", ValueArray[325], "Value of APAGCC should be '' " );
                Assert.AreEqual( "'N'", ValueArray[326], "Value of AP_BL should be 'N' " );
                Assert.AreEqual( "'N'", ValueArray[327], "Value of AP_GP should be 'N' " );
                Assert.AreEqual( "'N'", ValueArray[328], "Value of AP_VA should be 'N' " );
                Assert.AreEqual( "'N'", ValueArray[329], "Value of AP_WA should be 'N' " );
                Assert.AreEqual( "'N'", ValueArray[330], "Value of AP_NA should be 'N' " );
                Assert.AreEqual( "' '", ValueArray[331], "Value of AP_OP should be ' ' " );
                Assert.AreEqual( "''", ValueArray[332], "Value of AP_AG should be '' " );
                Assert.AreEqual( "'N'", ValueArray[333], "Value of AP_EMPL should be 'N' " );
                Assert.AreEqual( "'Y'", ValueArray[334], "Value of AP_SEMPL should be 'Y' " );
                Assert.AreEqual( "''", ValueArray[335], "Value of AP_AGGHP should be '' " );
                Assert.AreEqual( "''", ValueArray[336], "Value of AP_AGGHP20 should be '' " );
                Assert.AreEqual( "'Y'", ValueArray[337], "Value of AP_DI should be 'Y' " );
                Assert.AreEqual( "''", ValueArray[338], "Value of AP_OEMPL should be '' " );
                Assert.AreEqual( "'Y'", ValueArray[339], "Value of AP_DIGHP should be 'Y' " );
                Assert.AreEqual( "'Y'", ValueArray[340], "Value of AP_DIGHP00 should be 'Y' " );
                Assert.AreEqual( "''", ValueArray[341], "Value of AP_ES should be '' " );
                Assert.AreEqual( "''", ValueArray[342], "Value of AP_ESGHP should be '' " );
                Assert.AreEqual( "''", ValueArray[343], "Value of AP_ESKT should be '' " );
                Assert.AreEqual( "''", ValueArray[344], "Value of AP_ESDT should be '' " );
                Assert.AreEqual( "''", ValueArray[345], "Value of AP_ES30MO should be '' " );
                Assert.AreEqual( "''", ValueArray[346], "Value of AP_ESMULTI should be '' " );
                Assert.AreEqual( "''", ValueArray[347], "Value of AP_ESIA should be '' " );
                Assert.AreEqual( "''", ValueArray[348], "Value of AP_ESOTHR should be '' " );
                Assert.AreEqual( "'Y'", ValueArray[349], "Value of AP_GHP should be 'Y' " );
                Assert.AreEqual( "''", ValueArray[350], "Value of AP_AUTO should be '' " );
                Assert.AreEqual( "0", ValueArray[351], "Value of AP_INS# should be 0 " );
                Assert.AreEqual( "''", ValueArray[352], "Value of AP_PNID1 should be '' " );
                Assert.AreEqual( "''", ValueArray[353], "Value of AP_PNID2 should be '' " );
                Assert.AreEqual( "''", ValueArray[354], "Value of AP_USER should be '' " );
                Assert.AreEqual( "'0001-01-01'", ValueArray[359], "Value of AP_ESSDTDT should be '0001-01-01' " );
                Assert.AreEqual( CURRENT_TIMESTAMP, ValueArray[362], "Value of AP_DTTM should be " + CURRENT_TIMESTAMP );
                Assert.AreEqual( "'PerotSystems'", ValueArray[363], "Value of AP_SENM should be 'PerotSystems' " );
                Assert.AreEqual( "''", ValueArray[364], "Value of AP_SEADR should be '' " );
                Assert.AreEqual( "''", ValueArray[365], "Value of AP_SECIT should be '' " );
                Assert.AreEqual( "''", ValueArray[366], "Value of AP_SESTE should be '' " );
                Assert.AreEqual( "''", ValueArray[367], "Value of AP_SEZIP should be '' " );
                Assert.AreEqual( "''", ValueArray[368], "Value of AP_SEZP4 should be '' " );
                Assert.AreEqual( "''", ValueArray[369], "Value of AP_BLTDY should be '' " );
                Assert.AreEqual( "''", ValueArray[370], "Value of AP_FENM should be '' " );
                Assert.AreEqual( "''", ValueArray[371], "Value of AP_FEADR should be '' " );
                Assert.AreEqual( "''", ValueArray[372], "Value of AP_FECIT should be '' " );
                Assert.AreEqual( "''", ValueArray[373], "Value of AP_FESTE should be '' " );
                Assert.AreEqual( "''", ValueArray[374], "Value of AP_FEZIP should be '' " );
                Assert.AreEqual( "''", ValueArray[375], "Value of AP_FEZP4 should be '' " );
                Assert.AreEqual( "'N'", ValueArray[376], "Value of AP_EMPLN should be 'N' " );
                Assert.AreEqual( "'N'", ValueArray[377], "Value of AP_SEMPLN should be 'N' " );
                Assert.AreEqual( "'PACCESS'", ValueArray[378], "Value of APWSIR should be 'PACCESS' " );
                Assert.AreEqual( "''", ValueArray[379], "Value of APSECR should be '' " );
                Assert.AreEqual( "''", ValueArray[380], "Value of APORR1 should be '' " );
                Assert.AreEqual( "''", ValueArray[381], "Value of APORR2 should be '' " );
                Assert.AreEqual( "''", ValueArray[382], "Value of APORR3 should be '' " );
                Assert.AreEqual( "''", ValueArray[383], "Value of AP_NFALT should be '' " );
                Assert.AreEqual( "''", ValueArray[384], "Value of AP_LIABL should be '' " );
                Assert.AreEqual( "''", ValueArray[385], "Value of AP_AGGHPF should be '' " );
                Assert.AreEqual( "''", ValueArray[386], "Value of AP_AGHP20P should be '' " );
                Assert.AreEqual( "''", ValueArray[387], "Value of AP_DIGHPF should be '' " );
                Assert.AreEqual( "''", ValueArray[388], "Value of AP_DGHP00P should be '' " );
                Assert.AreEqual( "''", ValueArray[389], "Value of AP_DGHP00F should be '' " );
                Assert.AreEqual( "'1'", ValueArray[390], "Value of AP_FORMVER should be '1' " );
                Assert.AreEqual( "777777", ValueArray[391], "Value of APPACCT should be 777777 " );
                if ( anAccount.Activity != null )
                {
                    if ( anAccount.PreMSECopiedAccountNumber != 0 )
                    {
                        if ( anAccount.Activity.GetType().Equals( typeof( PreMSERegisterActivity ) ) )
                        {
                            Assert.AreEqual( "'A'", ValueArray[392], "Value of APACTF should be 'A' " );
                        }
                        if ( anAccount.Activity.GetType().Equals( typeof( PostMSERegistrationActivity ) ) )
                        {
                            Assert.AreEqual( "'D'", ValueArray[392], "Value of APACTF should be 'D' " );
                        }
                    }
                    else
                    {
                        Assert.AreEqual( "''", ValueArray[64], "Value of APACTF should be '' " );
                    }
                }

                // Benefits Validation fields                
                Assert.AreEqual( "0", ValueArray[393], "Value of APCVID should be 0 " );
                Assert.AreEqual( "''", ValueArray[394], "Value of APACRFN should be '' " );
                Assert.AreEqual( "''", ValueArray[395], "Value of APACRLN should be '' " );
                Assert.AreEqual( "''", ValueArray[396], "Value of APSAUTH should be '' " );
                Assert.AreEqual( "'0001-01-01'", ValueArray[397], "Value of APEFDA should be '0001-01-01' " );
                Assert.AreEqual( "'0001-01-01'", ValueArray[398], "Value of APEXDA should be '0001-01-01' " );
                Assert.AreEqual( "''", ValueArray[399], "Value of APASTS should be '' " );
                Assert.AreEqual( "''", ValueArray[400], "Value of APARMKS should be '' " );
                Assert.AreEqual( "'0001-01-01'", ValueArray[401], "Value of AP1ELDT should be '0001-01-01' " );
                Assert.AreEqual( "''", ValueArray[402], "Value of AP1PTMC should be '' " );
                Assert.AreEqual( "''", ValueArray[403], "Value of AP1PTOI should be '' " );
                Assert.AreEqual( "-1", ValueArray[404], "Value of AP1CPAY should be -1 " );
                Assert.AreEqual( "''", ValueArray[405], "Value of AP1EVCN should be '' " );
                Assert.AreEqual( "1", ValueArray[406], "Value of AP1INFF should be 1 " );
                Assert.AreEqual( "''", ValueArray[407], "Value of AP1REMK should be '' " );
                Assert.AreEqual( "''", ValueArray[408], "Value of AP2NMNT should be '' " );
                Assert.AreEqual( "''", ValueArray[409], "Value of AP2CLMN should be '' " );
                Assert.AreEqual( "''", ValueArray[410], "Value of AP2ADVR should be '' " );
                Assert.AreEqual( "''", ValueArray[411], "Value of AP2INPH should be '' " );
                Assert.AreEqual( "''", ValueArray[412], "Value of AP2EMPM should be '' " );
                Assert.AreEqual( "1", ValueArray[413], "Value of AP2INFF should be 1 " );
                Assert.AreEqual( "''", ValueArray[414], "Value of AP2REMK should be '' " );
                Assert.AreEqual( "''", ValueArray[415], "Value of AP3PACV should be '' " );
                Assert.AreEqual( "'0001-01-01'", ValueArray[416], "Value of AP3PAED should be '0001-01-01' " );
                Assert.AreEqual( "''", ValueArray[417], "Value of AP3PBCV should be '' " );
                Assert.AreEqual( "'0001-01-01'", ValueArray[418], "Value of AP3PBED should be '0001-01-01' " );
                Assert.AreEqual( "''", ValueArray[419], "Value of AP3MHCV should be '' " );
                Assert.AreEqual( "''", ValueArray[420], "Value of AP3MDSC should be '' " );
                Assert.AreEqual( "'0001-01-01'", ValueArray[421], "Value of AP3DTLB should be '0001-01-01' " );
                Assert.AreEqual( "-1", ValueArray[422], "Value of AP3RMHD should be -1 " );
                Assert.AreEqual( "0", ValueArray[423], "Value of AP3RCID should be 0 " );
                Assert.AreEqual( "-1", ValueArray[424], "Value of AP3RLRD should be -1 " );
                Assert.AreEqual( "-1", ValueArray[425], "Value of AP3RSNF should be -1 " );
                Assert.AreEqual( "-1", ValueArray[426], "Value of AP3RSNC should be -1 " );
                Assert.AreEqual( "''", ValueArray[427], "Value of AP3PTHS should be '' " );
                Assert.AreEqual( "''", ValueArray[428], "Value of AP3BNNV should be '' " );
                Assert.AreEqual( "1", ValueArray[429], "Value of AP3INFF should be 1 " );
                Assert.AreEqual( "''", ValueArray[430], "Value of AP3REMK should be '' " );
                Assert.AreEqual( "1", ValueArray[431], "Value of AP4INFF should be 1 " );
                Assert.AreEqual( "''", ValueArray[432], "Value of AP4ELPH should be '' " );
                Assert.AreEqual( "''", ValueArray[433], "Value of AP4INRP should be '' " );
                Assert.AreEqual( "''", ValueArray[434], "Value of AP4TYCV should be '' " );
                Assert.AreEqual( "'0001-01-01'", ValueArray[435], "Value of AP4EFDI should be '0001-01-01' " );
                Assert.AreEqual( "'0001-01-01'", ValueArray[436], "Value of AP4TMDI should be '0001-01-01' " );
                Assert.AreEqual( "-1", ValueArray[437], "Value of AP4DEDA should be -1 " );
                Assert.AreEqual( "''", ValueArray[438], "Value of AP4DEMT should be '' " );
                Assert.AreEqual( "-1", ValueArray[439], "Value of AP4DEAM should be -1 " );
                Assert.AreEqual( "-1", ValueArray[440], "Value of AP4COIN should be -1 " );
                Assert.AreEqual( "-1", ValueArray[441], "Value of AP4OTPK should be -1 " );
                Assert.AreEqual( "''", ValueArray[442], "Value of AP4OTPM should be '' " );
                Assert.AreEqual( "-1", ValueArray[443], "Value of AP4OTPA should be -1 " );
                Assert.AreEqual( "-1", ValueArray[444], "Value of AP4PROT should be -1 " );
                Assert.AreEqual( "-1", ValueArray[445], "Value of AP4CPAY should be -1 " );
                Assert.AreEqual( "''", ValueArray[446], "Value of AP4REMK should be '' " );
                Assert.AreEqual( "''", ValueArray[447], "Value of AP5PTMD should be '' " );
                Assert.AreEqual( "''", ValueArray[448], "Value of AP5INIF should be '' " );
                Assert.AreEqual( "1", ValueArray[449], "Value of AP6INFF should be 1 " );
                Assert.AreEqual( "''", ValueArray[450], "Value of AP6ELPH should be '' " );
                Assert.AreEqual( "''", ValueArray[451], "Value of AP6INRP should be '' " );
                Assert.AreEqual( "'0001-01-01'", ValueArray[452], "Value of AP6EFDI should be '0001-01-01' " );
                Assert.AreEqual( "'0001-01-01'", ValueArray[453], "Value of AP6TMDI should be '0001-01-01' " );
                Assert.AreEqual( "' '", ValueArray[454], "Value of AP6SFPC should be ' ' " );
                Assert.AreEqual( "' '", ValueArray[455], "Value of AP6SACB should be ' ' " );
                Assert.AreEqual( "' '", ValueArray[456], "Value of AP6CAAV should be ' ' " );
                Assert.AreEqual( "' '", ValueArray[457], "Value of AP6COBF should be ' ' " );
                Assert.AreEqual( "0", ValueArray[458], "Value of AP6RCOB should be 0 " );
                Assert.AreEqual( "0", ValueArray[459], "Value of AP6TPPR should be 0 " );
                Assert.AreEqual( "''", ValueArray[460], "Value of AP6NPPO should be '' " );
                Assert.AreEqual( "' '", ValueArray[461], "Value of AP6HCPR should be ' ' " );
                Assert.AreEqual( "''", ValueArray[462], "Value of AP6ACNM should be '' " );
                Assert.AreEqual( "' '", ValueArray[463], "Value of AP6MPCV should be ' ' " );
                Assert.AreEqual( "''", ValueArray[464], "Value of AP6REMK should be '' " );
                Assert.AreEqual( "0", ValueArray[465], "Value of AP601CTID should be 0 " );
                Assert.AreEqual( "-1", ValueArray[466], "Value of AP601DEDA should be -1 " );
                Assert.AreEqual( "''", ValueArray[467], "Value of AP601TMPR should be '' " );
                Assert.AreEqual( "''", ValueArray[468], "Value of AP601DEMT should be '' " );
                Assert.AreEqual( "-1", ValueArray[469], "Value of AP601DEAM should be -1 " );
                Assert.AreEqual( "-1", ValueArray[470], "Value of AP601COIN should be -1 " );
                Assert.AreEqual( "-1", ValueArray[471], "Value of AP601OTPK should be -1 " );
                Assert.AreEqual( "''", ValueArray[472], "Value of AP601OTPM should be '' " );
                Assert.AreEqual( "-1", ValueArray[473], "Value of AP601OTPA should be -1 " );
                Assert.AreEqual( "-1", ValueArray[474], "Value of AP601PROT should be -1 " );
                Assert.AreEqual( "-1", ValueArray[475], "Value of AP601CPAY should be -1 " );
                Assert.AreEqual( "''", ValueArray[476], "Value of AP601WCPY should be '' " );
                Assert.AreEqual( "-1", ValueArray[477], "Value of AP601NMVS should be -1 " );
                Assert.AreEqual( "-1", ValueArray[478], "Value of AP601LFMB should be -1 " );
                Assert.AreEqual( "-1", ValueArray[479], "Value of AP601LFRM should be -1 " );
                Assert.AreEqual( "''", ValueArray[480], "Value of AP601LFVM should be '' " );
                Assert.AreEqual( "-1", ValueArray[481], "Value of AP601MXBN should be -1 " );
                Assert.AreEqual( "-1", ValueArray[482], "Value of AP601RMBN should be -1 " );
                Assert.AreEqual( "''", ValueArray[483], "Value of AP601RMMT should be '' " );
                Assert.AreEqual( "0", ValueArray[484], "Value of AP602CTID should be 0 " );
                Assert.AreEqual( "-1", ValueArray[485], "Value of AP602DEDA should be -1 " );
                Assert.AreEqual( "''", ValueArray[486], "Value of AP602TMPR should be '' " );
                Assert.AreEqual( "''", ValueArray[487], "Value of AP602DEMT should be '' " );
                Assert.AreEqual( "-1", ValueArray[488], "Value of AP602DEAM should be -1 " );
                Assert.AreEqual( "-1", ValueArray[489], "Value of AP602COIN should be -1 " );
                Assert.AreEqual( "-1", ValueArray[490], "Value of AP602OTPK should be -1 " );
                Assert.AreEqual( "''", ValueArray[491], "Value of AP602OTPM should be '' " );
                Assert.AreEqual( "-1", ValueArray[492], "Value of AP602OTPA should be -1 " );
                Assert.AreEqual( "-1", ValueArray[493], "Value of AP602PROT should be -1 " );
                Assert.AreEqual( "-1", ValueArray[494], "Value of AP602CPAY should be -1 " );
                Assert.AreEqual( "''", ValueArray[495], "Value of AP602WCPY should be '' " );
                Assert.AreEqual( "-1", ValueArray[496], "Value of AP602NMVS should be -1 " );
                Assert.AreEqual( "-1", ValueArray[497], "Value of AP602LFMB should be -1 " );
                Assert.AreEqual( "-1", ValueArray[498], "Value of AP602LFRM should be -1 " );
                Assert.AreEqual( "''", ValueArray[499], "Value of AP602LFVM should be '' " );
                Assert.AreEqual( "-1", ValueArray[500], "Value of AP602MXBN should be -1 " );
                Assert.AreEqual( "-1", ValueArray[501], "Value of AP602RMBN should be -1 " );
                Assert.AreEqual( "''", ValueArray[502], "Value of AP602RMMT should be '' " );
                Assert.AreEqual( "0", ValueArray[503], "Value of AP603CTID should be 0 " );
                Assert.AreEqual( "-1", ValueArray[504], "Value of AP603DEDA should be -1 " );
                Assert.AreEqual( "''", ValueArray[505], "Value of AP603TMPR should be '' " );
                Assert.AreEqual( "''", ValueArray[506], "Value of AP603DEMT should be '' " );
                Assert.AreEqual( "-1", ValueArray[507], "Value of AP603DEAM should be -1 " );
                Assert.AreEqual( "-1", ValueArray[508], "Value of AP603COIN should be -1 " );
                Assert.AreEqual( "-1", ValueArray[509], "Value of AP603OTPK should be -1 " );
                Assert.AreEqual( "''", ValueArray[510], "Value of AP603OTPM should be '' " );
                Assert.AreEqual( "-1", ValueArray[511], "Value of AP603OTPA should be -1 " );
                Assert.AreEqual( "-1", ValueArray[512], "Value of AP603PROT should be -1 " );
                Assert.AreEqual( "-1", ValueArray[513], "Value of AP603CPAY should be -1 " );
                Assert.AreEqual( "''", ValueArray[514], "Value of AP603WCPY should be '' " );
                Assert.AreEqual( "-1", ValueArray[515], "Value of AP603NMVS should be -1 " );
                Assert.AreEqual( "-1", ValueArray[516], "Value of AP603LFMB should be -1 " );
                Assert.AreEqual( "-1", ValueArray[517], "Value of AP603LFRM should be -1 " );
                Assert.AreEqual( "''", ValueArray[518], "Value of AP603LFVM should be '' " );
                Assert.AreEqual( "-1", ValueArray[519], "Value of AP603MXBN should be -1 " );
                Assert.AreEqual( "-1", ValueArray[520], "Value of AP603RMBN should be -1 " );
                Assert.AreEqual( "''", ValueArray[521], "Value of AP603RMMT should be '' " );
                Assert.AreEqual( "0", ValueArray[522], "Value of AP604CTID should be 0 " );
                Assert.AreEqual( "-1", ValueArray[523], "Value of AP604DEDA should be -1 " );
                Assert.AreEqual( "''", ValueArray[524], "Value of AP604TMPR should be '' " );
                Assert.AreEqual( "''", ValueArray[525], "Value of AP604DEMT should be '' " );
                Assert.AreEqual( "-1", ValueArray[526], "Value of AP604DEAM should be -1 " );
                Assert.AreEqual( "-1", ValueArray[527], "Value of AP604COIN should be -1 " );
                Assert.AreEqual( "-1", ValueArray[528], "Value of AP604OTPK should be -1 " );
                Assert.AreEqual( "''", ValueArray[529], "Value of AP604OTPM should be '' " );
                Assert.AreEqual( "-1", ValueArray[530], "Value of AP604OTPA should be -1 " );
                Assert.AreEqual( "-1", ValueArray[531], "Value of AP604PROT should be -1 " );
                Assert.AreEqual( "-1", ValueArray[532], "Value of AP604CPAY should be -1 " );
                Assert.AreEqual( "''", ValueArray[533], "Value of AP604WCPY should be '' " );
                Assert.AreEqual( "-1", ValueArray[534], "Value of AP604NMVS should be -1 " );
                Assert.AreEqual( "-1", ValueArray[535], "Value of AP604LFMB should be -1 " );
                Assert.AreEqual( "-1", ValueArray[536], "Value of AP604LFRM should be -1 " );
                Assert.AreEqual( "''", ValueArray[537], "Value of AP604LFVM should be '' " );
                Assert.AreEqual( "-1", ValueArray[538], "Value of AP604MXBN should be -1 " );
                Assert.AreEqual( "-1", ValueArray[539], "Value of AP604RMBN should be -1 " );
                Assert.AreEqual( "''", ValueArray[540], "Value of AP604RMMT should be '' " );
                Assert.AreEqual( "0", ValueArray[541], "Value of AP605CTID should be 0 " );
                Assert.AreEqual( "-1", ValueArray[542], "Value of AP605DEDA should be -1 " );
                Assert.AreEqual( "''", ValueArray[543], "Value of AP605TMPR should be '' " );
                Assert.AreEqual( "''", ValueArray[544], "Value of AP605DEMT should be '' " );
                Assert.AreEqual( "-1", ValueArray[545], "Value of AP605DEAM should be -1 " );
                Assert.AreEqual( "-1", ValueArray[546], "Value of AP605COIN should be -1 " );
                Assert.AreEqual( "-1", ValueArray[547], "Value of AP605OTPK should be -1 " );
                Assert.AreEqual( "''", ValueArray[548], "Value of AP605OTPM should be '' " );
                Assert.AreEqual( "-1", ValueArray[549], "Value of AP605OTPA should be -1 " );
                Assert.AreEqual( "-1", ValueArray[550], "Value of AP605PROT should be -1 " );
                Assert.AreEqual( "-1", ValueArray[551], "Value of AP605CPAY should be -1 " );
                Assert.AreEqual( "''", ValueArray[552], "Value of AP605WCPY should be '' " );
                Assert.AreEqual( "-1", ValueArray[553], "Value of AP605NMVS should be -1 " );
                Assert.AreEqual( "-1", ValueArray[554], "Value of AP605LFMB should be -1 " );
                Assert.AreEqual( "-1", ValueArray[555], "Value of AP605LFRM should be -1 " );
                Assert.AreEqual( "''", ValueArray[556], "Value of AP605LFVM should be '' " );
                Assert.AreEqual( "-1", ValueArray[557], "Value of AP605MXBN should be -1 " );
                Assert.AreEqual( "-1", ValueArray[558], "Value of AP605RMBN should be -1 " );
                Assert.AreEqual( "''", ValueArray[559], "Value of AP605RMMT should be '' " );
                Assert.AreEqual( "0", ValueArray[560], "Value of AP606CTID should be 0 " );
                Assert.AreEqual( "-1", ValueArray[561], "Value of AP606DEDA should be -1 " );
                Assert.AreEqual( "''", ValueArray[562], "Value of AP606TMPR should be '' " );
                Assert.AreEqual( "''", ValueArray[563], "Value of AP606DEMT should be '' " );
                Assert.AreEqual( "-1", ValueArray[564], "Value of AP606DEAM should be -1 " );
                Assert.AreEqual( "-1", ValueArray[565], "Value of AP606COIN should be -1 " );
                Assert.AreEqual( "-1", ValueArray[566], "Value of AP606OTPK should be -1 " );
                Assert.AreEqual( "''", ValueArray[567], "Value of AP606OTPM should be '' " );
                Assert.AreEqual( "-1", ValueArray[568], "Value of AP606OTPA should be -1 " );
                Assert.AreEqual( "-1", ValueArray[569], "Value of AP606PROT should be -1 " );
                Assert.AreEqual( "-1", ValueArray[570], "Value of AP606CPAY should be -1 " );
                Assert.AreEqual( "''", ValueArray[571], "Value of AP606WCPY should be '' " );
                Assert.AreEqual( "-1", ValueArray[572], "Value of AP606NMVS should be -1 " );
                Assert.AreEqual( "-1", ValueArray[573], "Value of AP606LFMB should be -1 " );
                Assert.AreEqual( "-1", ValueArray[574], "Value of AP606LFRM should be -1 " );
                Assert.AreEqual( "''", ValueArray[575], "Value of AP606LFVM should be '' " );
                Assert.AreEqual( "-1", ValueArray[576], "Value of AP606MXBN should be -1 " );
                Assert.AreEqual( "-1", ValueArray[577], "Value of AP606RMBN should be -1 " );
                Assert.AreEqual( "''", ValueArray[578], "Value of AP606RMMT should be '' " );
                Assert.AreEqual( "0", ValueArray[579], "Value of AP607CTID should be 0 " );
                Assert.AreEqual( "-1", ValueArray[580], "Value of AP607DEDA should be -1 " );
                Assert.AreEqual( "''", ValueArray[581], "Value of AP607TMPR should be '' " );
                Assert.AreEqual( "''", ValueArray[582], "Value of AP607DEMT should be '' " );
                Assert.AreEqual( "-1", ValueArray[583], "Value of AP607DEAM should be -1 " );
                Assert.AreEqual( "-1", ValueArray[584], "Value of AP607COIN should be -1 " );
                Assert.AreEqual( "-1", ValueArray[585], "Value of AP607OTPK should be -1 " );
                Assert.AreEqual( "''", ValueArray[586], "Value of AP607OTPM should be '' " );
                Assert.AreEqual( "-1", ValueArray[587], "Value of AP607OTPA should be -1 " );
                Assert.AreEqual( "-1", ValueArray[588], "Value of AP607PROT should be -1 " );
                Assert.AreEqual( "-1", ValueArray[589], "Value of AP607CPAY should be -1 " );
                Assert.AreEqual( "''", ValueArray[590], "Value of AP607WCPY should be '' " );
                Assert.AreEqual( "-1", ValueArray[591], "Value of AP607NMVS should be -1 " );
                Assert.AreEqual( "-1", ValueArray[592], "Value of AP607LFMB should be -1 " );
                Assert.AreEqual( "-1", ValueArray[593], "Value of AP607LFRM should be -1 " );
                Assert.AreEqual( "''", ValueArray[594], "Value of AP607LFVM should be '' " );
                Assert.AreEqual( "-1", ValueArray[595], "Value of AP607MXBN should be -1 " );
                Assert.AreEqual( "-1", ValueArray[596], "Value of AP607RMBN should be -1 " );
                Assert.AreEqual( "''", ValueArray[597], "Value of AP607RMMT should be '' " );
                Assert.AreEqual( "0", ValueArray[598], "Value of AP608CTID should be 0 " );
                Assert.AreEqual( "-1", ValueArray[599], "Value of AP608DEDA should be -1 " );
                Assert.AreEqual( "''", ValueArray[600], "Value of AP608TMPR should be '' " );
                Assert.AreEqual( "''", ValueArray[601], "Value of AP608DEMT should be '' " );
                Assert.AreEqual( "-1", ValueArray[602], "Value of AP608DEAM should be -1 " );
                Assert.AreEqual( "-1", ValueArray[603], "Value of AP608COIN should be -1 " );
                Assert.AreEqual( "-1", ValueArray[604], "Value of AP608OTPK should be -1 " );
                Assert.AreEqual( "''", ValueArray[605], "Value of AP608OTPM should be '' " );
                Assert.AreEqual( "-1", ValueArray[606], "Value of AP608OTPA should be -1 " );
                Assert.AreEqual( "-1", ValueArray[607], "Value of AP608PROT should be -1 " );
                Assert.AreEqual( "-1", ValueArray[608], "Value of AP608CPAY should be -1 " );
                Assert.AreEqual( "''", ValueArray[609], "Value of AP608WCPY should be '' " );
                Assert.AreEqual( "-1", ValueArray[610], "Value of AP608NMVS should be -1 " );
                Assert.AreEqual( "-1", ValueArray[611], "Value of AP608LFMB should be -1 " );
                Assert.AreEqual( "-1", ValueArray[612], "Value of AP608LFRM should be -1 " );
                Assert.AreEqual( "''", ValueArray[613], "Value of AP608LFVM should be '' " );
                Assert.AreEqual( "-1", ValueArray[614], "Value of AP608MXBN should be -1 " );
                Assert.AreEqual( "-1", ValueArray[615], "Value of AP608RMBN should be -1 " );
                Assert.AreEqual( "''", ValueArray[616], "Value of AP608RMMT should be '' " );
                Assert.AreEqual( "0", ValueArray[617], "Value of AP609CTID should be 0 " );
                Assert.AreEqual( "-1", ValueArray[618], "Value of AP609DEDA should be -1 " );
                Assert.AreEqual( "''", ValueArray[619], "Value of AP609TMPR should be '' " );
                Assert.AreEqual( "''", ValueArray[620], "Value of AP609DEMT should be '' " );
                Assert.AreEqual( "-1", ValueArray[621], "Value of AP609DEAM should be -1 " );
                Assert.AreEqual( "-1", ValueArray[622], "Value of AP609COIN should be -1 " );
                Assert.AreEqual( "-1", ValueArray[623], "Value of AP609OTPK should be -1 " );
                Assert.AreEqual( "''", ValueArray[624], "Value of AP609OTPM should be '' " );
                Assert.AreEqual( "-1", ValueArray[625], "Value of AP609OTPA should be -1 " );
                Assert.AreEqual( "-1", ValueArray[626], "Value of AP609PROT should be -1 " );
                Assert.AreEqual( "-1", ValueArray[627], "Value of AP609CPAY should be -1 " );
                Assert.AreEqual( "''", ValueArray[628], "Value of AP609WCPY should be '' " );
                Assert.AreEqual( "-1", ValueArray[629], "Value of AP609NMVS should be -1 " );
                Assert.AreEqual( "-1", ValueArray[630], "Value of AP609LFMB should be -1 " );
                Assert.AreEqual( "-1", ValueArray[631], "Value of AP609LFRM should be -1 " );
                Assert.AreEqual( "''", ValueArray[632], "Value of AP609LFVM should be '' " );
                Assert.AreEqual( "-1", ValueArray[633], "Value of AP609MXBN should be -1 " );
                Assert.AreEqual( "-1", ValueArray[634], "Value of AP609RMBN should be -1 " );
                Assert.AreEqual( "''", ValueArray[635], "Value of AP609RMMT should be '' " );
                Assert.AreEqual( "0", ValueArray[636], "Value of AP610CTID should be 0 " );
                Assert.AreEqual( "-1", ValueArray[637], "Value of AP610DEDA should be -1 " );
                Assert.AreEqual( "''", ValueArray[638], "Value of AP610TMPR should be '' " );
                Assert.AreEqual( "''", ValueArray[639], "Value of AP610DEMT should be '' " );
                Assert.AreEqual( "-1", ValueArray[640], "Value of AP610DEAM should be -1 " );
                Assert.AreEqual( "-1", ValueArray[641], "Value of AP610COIN should be -1 " );
                Assert.AreEqual( "-1", ValueArray[642], "Value of AP610OTPK should be -1 " );
                Assert.AreEqual( "''", ValueArray[643], "Value of AP610OTPM should be '' " );
                Assert.AreEqual( "-1", ValueArray[644], "Value of AP610OTPA should be -1 " );
                Assert.AreEqual( "-1", ValueArray[645], "Value of AP610PROT should be -1 " );
                Assert.AreEqual( "-1", ValueArray[646], "Value of AP610CPAY should be -1 " );
                Assert.AreEqual( "''", ValueArray[647], "Value of AP610WCPY should be '' " );
                Assert.AreEqual( "-1", ValueArray[648], "Value of AP610NMVS should be -1 " );
                Assert.AreEqual( "-1", ValueArray[649], "Value of AP610LFMB should be -1 " );
                Assert.AreEqual( "-1", ValueArray[650], "Value of AP610LFRM should be -1 " );
                Assert.AreEqual( "''", ValueArray[651], "Value of AP610LFVM should be '' " );
                Assert.AreEqual( "-1", ValueArray[652], "Value of AP610MXBN should be -1 " );
                Assert.AreEqual( "-1", ValueArray[653], "Value of AP610RMBN should be -1 " );
                Assert.AreEqual( "''", ValueArray[654], "Value of AP610RMMT should be '' " );
                Assert.AreEqual( "0", ValueArray[655], "Value of AP611CTID should be 0 " );
                Assert.AreEqual( "-1", ValueArray[656], "Value of AP611DEDA should be -1 " );
                Assert.AreEqual( "''", ValueArray[657], "Value of AP611TMPR should be '' " );
                Assert.AreEqual( "''", ValueArray[658], "Value of AP611DEMT should be '' " );
                Assert.AreEqual( "-1", ValueArray[659], "Value of AP611DEAM should be -1 " );
                Assert.AreEqual( "-1", ValueArray[660], "Value of AP611COIN should be -1 " );
                Assert.AreEqual( "-1", ValueArray[661], "Value of AP611OTPK should be -1 " );
                Assert.AreEqual( "''", ValueArray[662], "Value of AP611OTPM should be '' " );
                Assert.AreEqual( "-1", ValueArray[663], "Value of AP611OTPA should be -1 " );
                Assert.AreEqual( "-1", ValueArray[664], "Value of AP611PROT should be -1 " );
                Assert.AreEqual( "-1", ValueArray[665], "Value of AP611CPAY should be -1 " );
                Assert.AreEqual( "''", ValueArray[666], "Value of AP611WCPY should be '' " );
                Assert.AreEqual( "-1", ValueArray[667], "Value of AP611NMVS should be -1 " );
                Assert.AreEqual( "-1", ValueArray[668], "Value of AP611LFMB should be -1 " );
                Assert.AreEqual( "-1", ValueArray[669], "Value of AP611LFRM should be -1 " );
                Assert.AreEqual( "''", ValueArray[670], "Value of AP611LFVM should be '' " );
                Assert.AreEqual( "-1", ValueArray[671], "Value of AP611MXBN should be -1 " );
                Assert.AreEqual( "-1", ValueArray[672], "Value of AP611RMBN should be -1 " );
                Assert.AreEqual( "''", ValueArray[673], "Value of AP611RMMT should be '' " );
                Assert.AreEqual( "0", ValueArray[674], "Value of AP612CTID should be 0 " );
                Assert.AreEqual( "-1", ValueArray[675], "Value of AP612DEDA should be -1 " );
                Assert.AreEqual( "''", ValueArray[676], "Value of AP612TMPR should be '' " );
                Assert.AreEqual( "''", ValueArray[677], "Value of AP612DEMT should be '' " );
                Assert.AreEqual( "-1", ValueArray[678], "Value of AP612DEAM should be -1 " );
                Assert.AreEqual( "-1", ValueArray[679], "Value of AP612COIN should be -1 " );
                Assert.AreEqual( "-1", ValueArray[680], "Value of AP612OTPK should be -1 " );
                Assert.AreEqual( "''", ValueArray[681], "Value of AP612OTPM should be '' " );
                Assert.AreEqual( "-1", ValueArray[682], "Value of AP612OTPA should be -1 " );
                Assert.AreEqual( "-1", ValueArray[683], "Value of AP612PROT should be -1 " );
                Assert.AreEqual( "-1", ValueArray[684], "Value of AP612CPAY should be -1 " );
                Assert.AreEqual( "''", ValueArray[685], "Value of AP612WCPY should be '' " );
                Assert.AreEqual( "-1", ValueArray[686], "Value of AP612NMVS should be -1 " );
                Assert.AreEqual( "-1", ValueArray[687], "Value of AP612LFMB should be -1 " );
                Assert.AreEqual( "-1", ValueArray[688], "Value of AP612LFRM should be -1 " );
                Assert.AreEqual( "''", ValueArray[689], "Value of AP612LFVM should be '' " );
                Assert.AreEqual( "-1", ValueArray[690], "Value of AP612MXBN should be -1 " );
                Assert.AreEqual( "-1", ValueArray[691], "Value of AP612RMBN should be -1 " );
                Assert.AreEqual( "''", ValueArray[692], "Value of AP612RMMT should be '' " );
                Assert.AreEqual("'234 MulHolland Drive'", ValueArray[698], "Value of APJADRE1 should be '234 MulHolland Drive' ");
                Assert.AreEqual("'#1'", ValueArray[699], "Value of APJADRE2 should be '#1' ");
            }
        }

        [Test]
        public void TestBuildSqlFromForSecondary()
        {
            i_InsuranceInsertStrategy = new
                InsuranceInsertStrategy( SECONDARY_COVERAGE );
            i_InsuranceInsertStrategy.UserSecurityCode = "KEVN";
            i_InsuranceInsertStrategy.PreRegistrationFlag = "P";
            i_InsuranceInsertStrategy.OrignalTransactionId = "PC";

            ArrayList sqlStrings =
                i_InsuranceInsertStrategy.BuildSqlFrom( anAccount, transactionKeys );
            foreach ( string sqlString in sqlStrings )
            {
                int startPositionOfValues =
                    sqlString.IndexOf( "(", sqlString.IndexOf( ")" ) ) + 1;
                int lengthOfValues =
                    sqlString.Length - sqlString.IndexOf( "(", sqlString.IndexOf( ")" ) ) - 2;
                string[] ValueArray =
                    sqlString.Substring( startPositionOfValues, lengthOfValues ).Split( ',' );

                Assert.AreEqual( NUMBER_OF_ENTRIES, ValueArray.Length, "Wrong number of entries in ValueArray" );
                Assert.AreEqual( " ''", ValueArray[0], "Value of APIDWS should be ''" );
                Assert.AreEqual( "'ID'", ValueArray[1], "Value of APIDID should be 'ID' " );
                Assert.AreEqual( "'&*%S@#$'", ValueArray[2], "Value of APRR# should be '&*%S@#$' " );
                Assert.AreEqual( "'KEVN'", ValueArray[3], "Value of APSEC2 should be 'KEVN' " );
                Assert.AreEqual( "900", ValueArray[4], "Value of APHSP# should be 900 " );
                Assert.AreEqual( "4477677", ValueArray[5], "Value of APACCT should be 4477677 " );
                Assert.AreEqual( "2", ValueArray[6], "Value of APSEQ# should be 2 " );
                Assert.AreEqual( "4477677", ValueArray[7], "Value of APGAR# should be 4477677 " );
                Assert.AreEqual( "'0'", ValueArray[8], "Value of APPTY should be '0' " );
                Assert.AreEqual( "2", ValueArray[9], "Value of APPLAN should be 2 " );
                Assert.AreEqual( "''", ValueArray[10], "Value of APBCFL should be '' " );
                Assert.AreEqual( "''", ValueArray[11], "Value of APCSPL should be '' " );
                Assert.AreEqual( "''", ValueArray[12], "Value of APINM should be '' " );
                Assert.AreEqual( "''", ValueArray[13], "Value of APGNM should be '' " );
                Assert.AreEqual( "''", ValueArray[14], "Value of APIAD1 should be '' " );
                Assert.AreEqual( "'                    '", ValueArray[15],
                                "Value of APIAD2 should be '                   ' " );
                Assert.AreEqual( "'                   '", ValueArray[16], "Value of APIAD3 should be '' " );
                Assert.AreEqual( "''", ValueArray[17], "Value of APVST should be '' " );
                Assert.AreEqual( "''", ValueArray[18], "Value of APPROR should be '' " );
                Assert.AreEqual( "''", ValueArray[19], "Value of APLAST should be '' " );
                Assert.AreEqual( "''", ValueArray[20], "Value of APVBV should be '' " );
                Assert.AreEqual( "0", ValueArray[21], "Value of APSDAT should be 0 " );
                Assert.AreEqual( "''", ValueArray[22], "Value of AP1FLG should be '' " );
                Assert.AreEqual( "0", ValueArray[23], "Value of AP1DED should be 0 " );
                Assert.AreEqual( "0", ValueArray[24], "Value of AP1$ should be 0 " );
                Assert.AreEqual( "0", ValueArray[25], "Value of AP1PCT should be 0 " );
                Assert.AreEqual( "0", ValueArray[26], "Value of AP1DYS should be 0 " );
                Assert.AreEqual( "''", ValueArray[27], "Value of AP2FLG should be '' " );
                Assert.AreEqual( "0", ValueArray[28], "Value of AP2DED should be 0 " );
                Assert.AreEqual( "0", ValueArray[29], "Value of AP2PCT should be 0 " );
                Assert.AreEqual( "0", ValueArray[30], "Value of AP2MX$ should be 0 " );
                Assert.AreEqual( "0", ValueArray[31], "Value of AP2DR$ should be 0 " );
                Assert.AreEqual( "0", ValueArray[32], "Value of AP2DID should be 0 " );
                Assert.AreEqual( "0", ValueArray[33], "Value of AP2IR$ should be 0 " );
                Assert.AreEqual( "0", ValueArray[34], "Value of AP2IRD should be 0 " );
                Assert.AreEqual( "''", ValueArray[35], "Value of AP3FLG should be '' " );
                Assert.AreEqual( "0", ValueArray[36], "Value of AP3DED should be 0 " );
                Assert.AreEqual( "0", ValueArray[37], "Value of AP3PCT should be 0 " );
                Assert.AreEqual( "0", ValueArray[38], "Value of AP3$ should be 0 " );
                Assert.AreEqual( "''", ValueArray[39], "Value of AP4FLG should be '' " );
                Assert.AreEqual( "0", ValueArray[40], "Value of AP4DED should be 0 " );
                Assert.AreEqual( "0", ValueArray[41], "Value of AP4PCT should be 0 " );
                Assert.AreEqual( "0", ValueArray[42], "Value of AP4$ should be 0 " );
                Assert.AreEqual( "0", ValueArray[43], "Value of AP4MR$ should be 0 " );
                Assert.AreEqual( "0", ValueArray[44], "Value of AP4MRD should be 0 " );
                Assert.AreEqual( "0", ValueArray[45], "Value of AP4MI$ should be 0 " );
                Assert.AreEqual( "0", ValueArray[46], "Value of AP4MID should be 0 " );
                Assert.AreEqual( "''", ValueArray[47], "Value of AP5FLG should be '' " );
                Assert.AreEqual( "0", ValueArray[48], "Value of AP5DED should be 0 " );
                Assert.AreEqual( "0", ValueArray[49], "Value of AP5PCT should be 0 " );
                Assert.AreEqual( "0", ValueArray[50], "Value of AP5$ should be 0 " );
                Assert.AreEqual( "0", ValueArray[51], "Value of AP5MR$ should be 0 " );
                Assert.AreEqual( "0", ValueArray[52], "Value of AP5MRD should be 0 " );
                Assert.AreEqual( "0", ValueArray[53], "Value of AP5MI$ should be 0 " );
                Assert.AreEqual( "0", ValueArray[54], "Value of AP5MID should be 0 " );
                Assert.AreEqual( "''", ValueArray[55], "Value of AP6DBM should be '' " );
                Assert.AreEqual( "''", ValueArray[56], "Value of AP6XMM should be '' " );
                Assert.AreEqual( "''", ValueArray[57], "Value of AP6RBM should be '' " );
                Assert.AreEqual( "''", ValueArray[58], "Value of AP6DM should be '' " );
                Assert.AreEqual( "''", ValueArray[59], "Value of APCOB should be '' " );
                Assert.AreEqual( "0", ValueArray[60], "Value of APEX01 should be 0 " );
                Assert.AreEqual( "0", ValueArray[61], "Value of APEX02 should be 0 " );
                Assert.AreEqual( "0", ValueArray[62], "Value of APEX03 should be 0 " );
                Assert.AreEqual( "0", ValueArray[63], "Value of APEX04 should be 0 " );
                Assert.AreEqual( "0", ValueArray[64], "Value of APEX05 should be 0 " );
                Assert.AreEqual( "0", ValueArray[65], "Value of APEX06 should be 0 " );
                Assert.AreEqual( "0", ValueArray[66], "Value of APEX07 should be 0 " );
                Assert.AreEqual( "0", ValueArray[67], "Value of APEX08 should be 0 " );
                Assert.AreEqual( "0", ValueArray[68], "Value of APEX09 should be 0 " );
                Assert.AreEqual( "0", ValueArray[69], "Value of APEX10 should be 0 " );
                Assert.AreEqual( "0", ValueArray[70], "Value of APEX11 should be 0 " );
                Assert.AreEqual( "0", ValueArray[71], "Value of APEX12 should be 0 " );
                Assert.AreEqual( "0", ValueArray[72], "Value of APEX13 should be 0 " );
                Assert.AreEqual( "0", ValueArray[73], "Value of APEX14 should be 0 " );
                Assert.AreEqual( "0", ValueArray[74], "Value of APEX15 should be 0 " );
                Assert.AreEqual( "0", ValueArray[75], "Value of APEX16 should be 0 " );
                Assert.AreEqual( "0", ValueArray[76], "Value of APEX17 should be 0 " );
                Assert.AreEqual( "0", ValueArray[77], "Value of APEX18 should be 0 " );
                Assert.AreEqual( "0", ValueArray[78], "Value of APEX19 should be 0 " );
                Assert.AreEqual( "0", ValueArray[79], "Value of APEX20 should be 0 " );
                Assert.AreEqual( "0", ValueArray[80], "Value of APFDAY should be 0 " );
                Assert.AreEqual( "0", ValueArray[81], "Value of APCDAY should be 0 " );
                Assert.AreEqual( "0", ValueArray[82], "Value of APCDOL should be 0 " );
                Assert.AreEqual( "0", ValueArray[83], "Value of APLDAY should be 0 " );
                Assert.AreEqual( "0", ValueArray[84], "Value of APLDOL should be 0 " );
                Assert.AreEqual( "0", ValueArray[85], "Value of APBDPT should be 0 " );
                Assert.AreEqual( "0", ValueArray[86], "Value of APBD$ should be 0 " );
                Assert.AreEqual( "0", ValueArray[87], "Value of APPMET should be 0 " );
                Assert.AreEqual( "0", ValueArray[88], "Value of APPM$ should be 0 " );
                Assert.AreEqual( "0", ValueArray[89], "Value of APPMPR should be 0 " );
                Assert.AreEqual( "''", ValueArray[90], "Value of APMCRE should be '' " );
                Assert.AreEqual( "''", ValueArray[91], "Value of APMAID should be '' " );
                Assert.AreEqual( "0", ValueArray[92], "Value of APMDED should be 0 " );
                Assert.AreEqual( "0", ValueArray[93], "Value of APLML should be 0 " );
                Assert.AreEqual( "0", ValueArray[94], "Value of APLMD should be 0 " );
                Assert.AreEqual( "0", ValueArray[95], "Value of APLUL# should be 0 " );
                Assert.AreEqual( "'C'", ValueArray[96], "Value of APACFL should be 'C' " );
                Assert.AreEqual( "'$#L@%'", ValueArray[98], "Value of APINLG should be '$#L@%' " );
                Assert.AreEqual( "''", ValueArray[99], "Value of APBYPS should be '' " );
                Assert.AreEqual( "0", ValueArray[100], "Value of APSWPY should be 0 " );
                Assert.AreEqual( "''", ValueArray[101], "Value of APABCF should be '' " );
                Assert.AreEqual( "''", ValueArray[102], "Value of APRICF should be '' " );
                Assert.AreEqual( "'SADSTORY'", ValueArray[103], "Value of APSLNM should be 'SADSTORY' " );

                // firstname for insurance should now contain the middle initial appended to the end of the firstname 
                // and preceded by a space - See OTD 36226
                string firstNameAndMI = "'SOMETHING " + ValueArray[249];
                string removeApost = firstNameAndMI.Remove( 11, 1 );
                Assert.AreEqual( removeApost, ValueArray[104], "Value of APSFNM should be 'SOMETHING' " );
                Assert.AreEqual( "'M'", ValueArray[105], "Value of APSSEX should be 'M' " );
                Assert.AreEqual( "''", ValueArray[106], "Value of APSRCD should be '' " );
                Assert.AreEqual( "''", ValueArray[107], "Value of APIID# should be '' " );

                Assert.AreEqual( "''", ValueArray[108], "Value of APGRPN should be '' " );
                Assert.AreEqual( "''", ValueArray[109], "Value of APESCD should be '' " );
                Assert.AreEqual( "''", ValueArray[110], "Value of APSBEN should be '' " );
                Assert.AreEqual( "''", ValueArray[111], "Value of APEEID should be '' " );
                Assert.AreEqual( "''", ValueArray[112], "Value of APSBEL should be '' " );
                Assert.AreEqual( "''", ValueArray[113],
                                "Value of APJADR should be '' " );
                Assert.AreEqual( "'Austin'", ValueArray[114], "Value of APJCIT should be 'Austin' " );
                Assert.AreEqual( "'TX'", ValueArray[115], "Value of APJSTE should be 'TX' " );
                Assert.AreEqual( "0", ValueArray[116], "Value of APJZIP should be 0 " );
                Assert.AreEqual( "0", ValueArray[117], "Value of APJZP4 should be 0 " );
                Assert.AreEqual( "972", ValueArray[118], "Value of APJACD should be 972 " );
                Assert.AreEqual( "546789", ValueArray[119], "Value of APJPH# should be 546789 " );
                Assert.AreEqual( "''", ValueArray[120], "Value of APNEIC should be '' " );
                Assert.AreEqual( "0", ValueArray[121], "Value of APDOV should be 0 " );
                Assert.AreEqual( "''", ValueArray[122], "Value of APELGS should be '' " );
                Assert.AreEqual( "0", ValueArray[123], "Value of APTDOC should be 0 " );
                Assert.AreEqual( "0", ValueArray[124], "Value of APELDT should be 0 " );
                Assert.AreEqual( "''", ValueArray[125], "Value of APSCHB should be '' " );
                Assert.AreEqual( "''", ValueArray[126], "Value of APVC01 should be '' " );
                Assert.AreEqual( "''", ValueArray[127], "Value of APVC02 should be '' " );
                Assert.AreEqual( "''", ValueArray[128], "Value of APVC03 should be '' " );
                Assert.AreEqual( "''", ValueArray[129], "Value of APVC04 should be '' " );
                Assert.AreEqual( "0", ValueArray[130], "Value of APVA01 should be 0 " );
                Assert.AreEqual( "0", ValueArray[131], "Value of APVA02 should be 0 " );
                Assert.AreEqual( "0", ValueArray[132], "Value of APVA03 should be 0 " );
                Assert.AreEqual( "0", ValueArray[133], "Value of APVA04 should be 0 " );
                Assert.AreEqual( "0", ValueArray[134], "Value of APAGNY should be 0 " );
                Assert.AreEqual( "0", ValueArray[135], "Value of APCHAM should be 0 " );
                Assert.AreEqual( "0", ValueArray[136], "Value of APCHAP should be 0 " );
                Assert.AreEqual( "''", ValueArray[137], "Value of APMSPC should be '' " );
                Assert.AreEqual( "0", ValueArray[138], "Value of APDBEN should be 0 " );
                Assert.AreEqual( "0", ValueArray[139], "Value of APDURR should be 0 " );
                Assert.AreEqual( "0", ValueArray[140], "Value of APACRE should be 0 " );
                Assert.AreEqual( "0", ValueArray[141], "Value of APDGPS should be 0 " );
                Assert.AreEqual( "''", ValueArray[142], "Value of APNEPS should be '' " );
                Assert.AreEqual( "0", ValueArray[143], "Value of APNEPI should be 0 " );
                Assert.AreEqual( "''", ValueArray[144], "Value of APITYP should be '' " );
                Assert.AreEqual( "0", ValueArray[145], "Value of APRIPL should be 0 " );
                Assert.AreEqual( "''", ValueArray[146], "Value of APCBFL should be '' " );
                Assert.AreEqual( "''", ValueArray[147], "Value of APINEX should be '' " );
                Assert.AreEqual( "0", ValueArray[148], "Value of APSURP should be 0 " );
                Assert.AreEqual( "0", ValueArray[149], "Value of APSURA should be 0 " );
                Assert.AreEqual( "''", ValueArray[150], "Value of APINS# should be '' " );
                Assert.AreEqual( "''", ValueArray[151], "Value of APCNFC should be '' " );
                Assert.AreEqual( "''", ValueArray[152], "Value of APPFCC should be '' " );
                Assert.AreEqual( "''", ValueArray[153], "Value of APRICO should be '' " );
                Assert.AreEqual( "''", ValueArray[154], "Value of APENM1 should be '' " );
                Assert.AreEqual( "''", ValueArray[155], "Value of APELO1 should be '' " );
                Assert.AreEqual( "''", ValueArray[156], "Value of APEDC1 should be '' " );
                Assert.AreEqual( "''", ValueArray[157], "Value of APESC1 should be '' " );
                Assert.AreEqual( "'00000000000'", ValueArray[158], "Value of APEID1 should be '00000000000' " );
                Assert.AreEqual( "''", ValueArray[159], "Value of APEA01 should be '' " );
                Assert.AreEqual( "0", ValueArray[160], "Value of APEZ01 should be 0 " );
                Assert.AreEqual( "''", ValueArray[161], "Value of APENM2 should be '' " );
                Assert.AreEqual( "'                '", ValueArray[162], "Value of APELO2 should be '                ' " );
                Assert.AreEqual( "''", ValueArray[163], "Value of APEDC2 should be '' " );
                Assert.AreEqual( "''", ValueArray[164], "Value of APESC2 should be '' " );
                Assert.AreEqual( "'00000000000'", ValueArray[165], "Value of APEID2 should be '00000000000' " );
                Assert.AreEqual( "''", ValueArray[166], "Value of APEA02 should be '' " );
                Assert.AreEqual( "0", ValueArray[167], "Value of APEZ02 should be 0 " );
                Assert.AreEqual( "''", ValueArray[168], "Value of APDEDU should be '' " );
                Assert.AreEqual( "''", ValueArray[169], "Value of APBLDE should be '' " );
                Assert.AreEqual( "''", ValueArray[170], "Value of APINDY should be '' " );
                Assert.AreEqual( "''", ValueArray[171], "Value of APLIFD should be '' " );
                Assert.AreEqual( "''", ValueArray[172], "Value of APNCVD should be '' " );
                Assert.AreEqual( "''", ValueArray[174], "Value of APCLRK should be '' " );
                Assert.AreEqual( "''", ValueArray[175], "Value of APZDTE should be '' " );
                Assert.AreEqual( "''", ValueArray[176], "Value of APZTME should be '' " );
                Assert.AreEqual( "''", ValueArray[177], "Value of APPNID should be '' " );
                Assert.AreEqual( "'SECONDARY'", ValueArray[178], "Value of APP#NM should be 'SECONDARY' " );
                Assert.AreEqual( "980221", ValueArray[179], "Value of APCBGD should be 980221 " );
                Assert.AreEqual( "0", ValueArray[180], "Value of APPLAD should be 0 " );
                Assert.AreEqual( "''", ValueArray[181], "Value of APSVTP should be '' " );
                Assert.AreEqual( "0", ValueArray[182], "Value of APSTCP should be 0 " );
                Assert.AreEqual( "0", ValueArray[183], "Value of APSTAM should be 0 " );
                Assert.AreEqual( "0", ValueArray[184], "Value of APST$L should be 0 " );
                Assert.AreEqual( "0", ValueArray[185], "Value of APSTPA should be 0 " );
                Assert.AreEqual( "''", ValueArray[186], "Value of APAUTF should be '' " );
                Assert.AreEqual( "''", ValueArray[187], "Value of APAUWV should be '' " );
                Assert.AreEqual( "0", ValueArray[188], "Value of APAUDY should be 0 " );
                Assert.AreEqual( "''", ValueArray[189], "Value of AP2OPF should be '' " );
                Assert.AreEqual( "''", ValueArray[190], "Value of AP2CMM should be '' " );
                Assert.AreEqual( "''", ValueArray[191], "Value of APCOPF should be '' " );
                Assert.AreEqual( "''", ValueArray[192], "Value of APDEDF should be '' " );
                Assert.AreEqual( "''", ValueArray[193], "Value of APBLNM should be '' " );
                Assert.AreEqual( "''", ValueArray[194], "Value of APOBCF should be '' " );
                Assert.AreEqual( "0", ValueArray[195], "Value of APOBCP should be 0 " );
                Assert.AreEqual( "0", ValueArray[196], "Value of APOBAM should be 0 " );
                Assert.AreEqual( "0", ValueArray[197], "Value of APOB$L should be 0 " );
                Assert.AreEqual( "0", ValueArray[198], "Value of APOBPA should be 0 " );
                Assert.AreEqual( "''", ValueArray[199], "Value of APNBCF should be '' " );
                Assert.AreEqual( "0", ValueArray[200], "Value of APNBCP should be 0 " );
                Assert.AreEqual( "0", ValueArray[201], "Value of APNBAM should be 0 " );
                Assert.AreEqual( "0", ValueArray[202], "Value of APNB$L should be 0 " );
                Assert.AreEqual( "0", ValueArray[203], "Value of APNBPA should be 0 " );
                Assert.AreEqual( "''", ValueArray[204], "Value of APCDCF should be '' " );
                Assert.AreEqual( "0", ValueArray[205], "Value of APCDCP should be 0 " );
                Assert.AreEqual( "0", ValueArray[206], "Value of APCDAM should be 0 " );
                Assert.AreEqual( "0", ValueArray[207], "Value of APCD$L should be 0 " );
                Assert.AreEqual( "0", ValueArray[208], "Value of APCDPA should be 0 " );
                Assert.AreEqual( "''", ValueArray[209], "Value of APDCCF should be '' " );
                Assert.AreEqual( "0", ValueArray[210], "Value of APDCCP should be 0 " );
                Assert.AreEqual( "0", ValueArray[211], "Value of APDCAM should be 0 " );
                Assert.AreEqual( "0", ValueArray[212], "Value of APDC$L should be 0 " );
                Assert.AreEqual( "0", ValueArray[213], "Value of APDCPA should be 0 " );
                Assert.AreEqual( "0", ValueArray[214], "Value of APDCAL should be 0 " );
                Assert.AreEqual( "0", ValueArray[215], "Value of APDADM should be 0 " );
                Assert.AreEqual( "''", ValueArray[216], "Value of APDDMF should be '' " );
                Assert.AreEqual( "0", ValueArray[217], "Value of APLFMX should be 0 " );
                Assert.AreEqual( "0", ValueArray[218], "Value of APMXAD should be 0 " );
                Assert.AreEqual( "''", ValueArray[219], "Value of APCOBF should be '' " );
                Assert.AreEqual( "''", ValueArray[220], "Value of APCOMM should be '' " );
                Assert.AreEqual( "''", ValueArray[221], "Value of APIFRF should be '' " );
                Assert.AreEqual( "0", ValueArray[222], "Value of APSUAC should be 0 " );
                Assert.AreEqual( "0", ValueArray[223], "Value of APSUPH should be 0 " );
                Assert.AreEqual( "''", ValueArray[224], "Value of APVFFG should be '' " );
                Assert.AreEqual( "''", ValueArray[225], "Value of APVFBY should be '' " );
                Assert.AreEqual( "0", ValueArray[226], "Value of APVFDT should be 0 " );
                Assert.AreEqual( "0", ValueArray[227], "Value of APAMDY should be 0 " );
                Assert.AreEqual( "0", ValueArray[228], "Value of APNCDY should be 0 " );
                Assert.AreEqual( "0", ValueArray[229], "Value of APELOS should be 0 " );
                Assert.AreEqual( "0", ValueArray[230], "Value of APRCDE should be 0 " );
                Assert.AreEqual( "0", ValueArray[231], "Value of APRSLC should be 0 " );
                Assert.AreEqual( "''", ValueArray[232], "Value of APUROP should be '' " );
                Assert.AreEqual( "''", ValueArray[233], "Value of APMSPF should be '' " );
                Assert.AreEqual( "''", ValueArray[234], "Value of APWVCP should be '' " );
                Assert.AreEqual( "0", ValueArray[235], "Value of APGCD1 should be 0 " );
                Assert.AreEqual( "0", ValueArray[236], "Value of APGCD2 should be 0 " );
                Assert.AreEqual( "''", ValueArray[237], "Value of APCONM should be '' " );
                Assert.AreEqual( "''", ValueArray[238], "Value of APEZP1 should be '' " );
                Assert.AreEqual( "'0000'", ValueArray[239], "Value of APEZ41 should be '0000' " );
                Assert.AreEqual( "''", ValueArray[240], "Value of APEZP2 should be '' " );
                Assert.AreEqual( "'0000'", ValueArray[241], "Value of APEZ42 should be '0000' " );
                if ( anAccount.Activity != null )
                {
                    if ( anAccount.Activity.GetType().Equals( typeof( PreMSERegisterActivity ) ) )
                    {
                        Assert.AreEqual( "''", ValueArray[242], "Value of APJZPA should be '' " );
                    }
                    else
                    {
                        Assert.AreEqual( "'60594'", ValueArray[242], "Value of APJZPA should be '60594' " );
                    }
                }
                Assert.AreEqual( "'4000'", ValueArray[243], "Value of APJZ4A should be '4000' " );
                Assert.AreEqual( "'PC'", ValueArray[244], "Value of APOID should be 'PC' " );
                Assert.AreEqual( "'P'", ValueArray[245], "Value of APPRE should be 'P' " );
                Assert.AreEqual( "'000785138'", ValueArray[246], "Value of APMR# should be '000785138' " );
                Assert.AreEqual( "''", ValueArray[247], "Value of APOACT should be '' " );
                Assert.AreEqual( "''", ValueArray[248], "Value of APEVC# should be '' " );
                Assert.AreEqual( "'N'", ValueArray[249], "Value of APSMI should be 'N' " );
                Assert.AreEqual( "''", ValueArray[250], "Value of APSBID should be '' " );
                Assert.AreEqual( "'USA'", ValueArray[251], "Value of APSCUN should be 'USA' " );
                Assert.AreEqual( "''", ValueArray[252], "Value of APSBTH should be '' " );
                Assert.AreEqual( "''", ValueArray[253], "Value of APOILN should be '' " );
                Assert.AreEqual( "''", ValueArray[254], "Value of APOIFN should be '' " );
                Assert.AreEqual( "''", ValueArray[255], "Value of APOIMI should be '' " );
                Assert.AreEqual( "''", ValueArray[256], "Value of APOISX should be '' " );
                Assert.AreEqual( "''", ValueArray[257], "Value of APOIBD should be '' " );
                Assert.AreEqual( "''", ValueArray[258], "Value of APOIID should be '' " );
                Assert.AreEqual( "''", ValueArray[259], "Value of APOIAD should be '' " );
                Assert.AreEqual( "''", ValueArray[260], "Value of APOICT should be '' " );
                Assert.AreEqual( "''", ValueArray[261], "Value of APOIST should be '' " );
                Assert.AreEqual( "''", ValueArray[262], "Value of APOICN should be '' " );
                Assert.AreEqual( "''", ValueArray[263], "Value of APOIZP should be '' " );
                Assert.AreEqual( "'0000'", ValueArray[264], "Value of APOIZ4 should be '0000' " );
                Assert.AreEqual( "''", ValueArray[265], "Value of APICPH should be '' " );
                Assert.AreEqual( "''", ValueArray[266], "Value of APMCDT should be '' " );
                Assert.AreEqual( "0", ValueArray[267], "Value of APDAMT should be 0 " );
                Assert.AreEqual( "0", ValueArray[268], "Value of APCPAY should be 0 " );
                Assert.AreEqual( "0", ValueArray[269], "Value of APCINS should be 0 " );
                Assert.AreEqual( "0", ValueArray[270], "Value of APAMCL should be 0 " );
                Assert.AreEqual( "0", ValueArray[271], "Value of APPYCL should be 0 " );
                Assert.AreEqual( "0", ValueArray[272], "Value of APINCL should be 0 " );
                Assert.AreEqual( "0", ValueArray[273], "Value of APPVCL should be 0 " );
                Assert.AreEqual( "0", ValueArray[274], "Value of APUCBL should be 0 " );
                Assert.AreEqual( "0", ValueArray[275], "Value of APEDBL should be 0 " );
                Assert.AreEqual( "0", ValueArray[276], "Value of APUFPT should be 0 " );
                Assert.AreEqual( "0", ValueArray[277], "Value of APEFPT should be 0 " );
                Assert.AreEqual( "0", ValueArray[278], "Value of APNUM1 should be 0 " );
                Assert.AreEqual( "0", ValueArray[279], "Value of APNUM2 should be 0 " );
                Assert.AreEqual( "0", ValueArray[280], "Value of APNUM3 should be 0 " );
                Assert.AreEqual( "0", ValueArray[281], "Value of APMNPY should be 0 " );
                Assert.AreEqual( "0", ValueArray[282], "Value of APCOPY should be 0 " );
                Assert.AreEqual( "0", ValueArray[283], "Value of APCIPY should be 0 " );
                Assert.AreEqual( "0", ValueArray[284], "Value of APUCPY should be 0 " );
                Assert.AreEqual( "0", ValueArray[285], "Value of APEDPY should be 0 " );
                Assert.AreEqual( "''", ValueArray[286], "Value of APPDUE should be '' " );
                Assert.AreEqual( "''", ValueArray[287], "Value of APCRPT should be '' " );
                Assert.AreEqual( "''", ValueArray[288], "Value of APCRSC should be '' " );
                Assert.AreEqual( "''", ValueArray[289], "Value of APCR01 should be '' " );
                Assert.AreEqual( "''", ValueArray[290], "Value of APCR02 should be '' " );
                Assert.AreEqual( "''", ValueArray[291], "Value of APCR03 should be '' " );
                Assert.AreEqual( "''", ValueArray[292], "Value of APCR04 should be '' " );
                Assert.AreEqual( "''", ValueArray[293], "Value of APCR05 should be '' " );
                Assert.AreEqual( "''", ValueArray[294], "Value of APCR06 should be '' " );
                Assert.AreEqual( "''", ValueArray[295], "Value of APCR07 should be '' " );
                Assert.AreEqual( "''", ValueArray[296], "Value of APCR08 should be '' " );
                Assert.AreEqual( "''", ValueArray[297], "Value of APCR09 should be '' " );
                Assert.AreEqual( "''", ValueArray[298], "Value of APCR10 should be '' " );
                Assert.AreEqual( "''", ValueArray[299], "Value of APCR11 should be '' " );
                Assert.AreEqual( "''", ValueArray[300], "Value of APCR12 should be '' " );
                Assert.AreEqual( "''", ValueArray[301], "Value of APHIC# should be '' " );
                Assert.AreEqual( "''", ValueArray[302], "Value of APITR# should be '' " );
                Assert.AreEqual( "''", ValueArray[303], "Value of APAUTH# should be '' " );
                Assert.AreEqual( "''", ValueArray[304], "Value of APAUTFG should be '' " );
                Assert.AreEqual( "''", ValueArray[305], "Value of APAUCP should be '' " );
                Assert.AreEqual( "0", ValueArray[306], "Value of APAUPH should be 0 " );
                Assert.AreEqual( "''", ValueArray[307], "Value of APAUEX should be '' " );
                Assert.AreEqual( "''", ValueArray[308], "Value of APADNM should be '' " );
                Assert.AreEqual( "''", ValueArray[309], "Value of APEMSU should be '' " );
                Assert.AreEqual( "''", ValueArray[310], "Value of APATNM should be '' " );
                Assert.AreEqual( "''", ValueArray[311], "Value of APATST should be '' " );
                Assert.AreEqual( "''", ValueArray[312], "Value of APATCT should be '' " );
                Assert.AreEqual( "''", ValueArray[313], "Value of APATSA should be '' " );
                Assert.AreEqual( "''", ValueArray[314], "Value of APATZ5 should be '' " );
                Assert.AreEqual( "'0000'", ValueArray[315], "Value of APATZ4 should be '0000' " );
                Assert.AreEqual( "''", ValueArray[316], "Value of APATCC should be '' " );
                Assert.AreEqual( "0", ValueArray[317], "Value of APATPH should be 0 " );
                Assert.AreEqual( "''", ValueArray[318], "Value of APAGNM should be '' " );
                Assert.AreEqual( "0", ValueArray[319], "Value of APAGPH should be 0 " );
                Assert.AreEqual( "''", ValueArray[320], "Value of APAGST should be '' " );
                Assert.AreEqual( "''", ValueArray[321], "Value of APAGCT should be '' " );
                Assert.AreEqual( "''", ValueArray[322], "Value of APAGSA should be '' " );
                Assert.AreEqual( "''", ValueArray[323], "Value of APAGZ5 should be '' " );
                Assert.AreEqual( "'0000'", ValueArray[324], "Value of APAGZ4 should be '0000' " );
                Assert.AreEqual( "''", ValueArray[325], "Value of APAGCC should be '' " );
                Assert.AreEqual( "0", ValueArray[391], "Value of APPACCT should be 0 " );
                //MSE Activity Flag for secondary insurance should always be blank for all activities:
                Assert.AreEqual( "''", ValueArray[392], "Value of APACTF should be '' " );
                // Benefits Validation fields

                Assert.AreEqual( "0", ValueArray[393], "Value of APCVID should be 0 " );
                Assert.AreEqual( "''", ValueArray[394], "Value of APACRFN should be '' " );
                Assert.AreEqual( "''", ValueArray[395], "Value of APACRLN should be '' " );
                Assert.AreEqual( "''", ValueArray[396], "Value of APSAUTH should be '' " );
                Assert.AreEqual( "'0001-01-01'", ValueArray[397], "Value of APEFDA should be '0001-01-01' " );
                Assert.AreEqual( "'0001-01-01'", ValueArray[398], "Value of APEXDA should be '0001-01-01' " );
                Assert.AreEqual( "''", ValueArray[399], "Value of APASTS should be '' " );
                Assert.AreEqual( "''", ValueArray[400], "Value of APARMKS should be '' " );
                Assert.AreEqual( "'0001-01-01'", ValueArray[401], "Value of AP1ELDT should be '0001-01-01' " );
                Assert.AreEqual( "''", ValueArray[402], "Value of AP1PTMC should be '' " );
                Assert.AreEqual( "''", ValueArray[403], "Value of AP1PTOI should be '' " );
                Assert.AreEqual( "-1", ValueArray[404], "Value of AP1CPAY should be -1 " );
                Assert.AreEqual( "''", ValueArray[405], "Value of AP1EVCN should be '' " );
                Assert.AreEqual( "1", ValueArray[406], "Value of AP1INFF should be 1 " );
                Assert.AreEqual( "''", ValueArray[407], "Value of AP1REMK should be '' " );
                Assert.AreEqual( "''", ValueArray[408], "Value of AP2NMNT should be '' " );
                Assert.AreEqual( "''", ValueArray[409], "Value of AP2CLMN should be '' " );
                Assert.AreEqual( "''", ValueArray[410], "Value of AP2ADVR should be '' " );
                Assert.AreEqual( "''", ValueArray[411], "Value of AP2INPH should be '' " );
                Assert.AreEqual( "''", ValueArray[412], "Value of AP2EMPM should be '' " );
                Assert.AreEqual( "1", ValueArray[413], "Value of AP2INFF should be 1 " );
                Assert.AreEqual( "''", ValueArray[414], "Value of AP2REMK should be '' " );
                Assert.AreEqual( "' '", ValueArray[415], "Value of AP3PACV should be ' ' " );
                Assert.AreEqual( "'0001-01-01'", ValueArray[416], "Value of AP3PAED should be '0001-01-01' " );
                Assert.AreEqual( "' '", ValueArray[417], "Value of AP3PBCV should be ' ' " );
                Assert.AreEqual( "'0001-01-01'", ValueArray[418], "Value of AP3PBED should be '0001-01-01' " );
                Assert.AreEqual( "' '", ValueArray[419], "Value of AP3MHCV should be ' ' " );
                Assert.AreEqual( "' '", ValueArray[420], "Value of AP3MDSC should be ' ' " );
                Assert.AreEqual( "'0001-01-01'", ValueArray[421], "Value of AP3DTLB should be '0001-01-01' " );
                Assert.AreEqual( "-1", ValueArray[422], "Value of AP3RMHD should be -1 " );
                Assert.AreEqual( "-1", ValueArray[423], "Value of AP3RCID should be -1 " );
                Assert.AreEqual( "-1", ValueArray[424], "Value of AP3RLRD should be -1 " );
                Assert.AreEqual( "-1", ValueArray[425], "Value of AP3RSNF should be -1 " );
                Assert.AreEqual( "-1", ValueArray[426], "Value of AP3RSNC should be -1 " );
                Assert.AreEqual( "' '", ValueArray[427], "Value of AP3PTHS should be ' ' " );
                Assert.AreEqual( "' '", ValueArray[428], "Value of AP3BNNV should be ' ' " );
                Assert.AreEqual( "1", ValueArray[429], "Value of AP3INFF should be 1 " );
                Assert.AreEqual( "''", ValueArray[430], "Value of AP3REMK should be '' " );
                Assert.AreEqual( "1", ValueArray[431], "Value of AP4INFF should be 1 " );
                Assert.AreEqual( "''", ValueArray[432], "Value of AP4ELPH should be '' " );
                Assert.AreEqual( "''", ValueArray[433], "Value of AP4INRP should be '' " );
                Assert.AreEqual( "''", ValueArray[434], "Value of AP4TYCV should be '' " );
                Assert.AreEqual( "'0001-01-01'", ValueArray[435], "Value of AP4EFDI should be '0001-01-01' " );
                Assert.AreEqual( "'0001-01-01'", ValueArray[436], "Value of AP4TMDI should be '0001-01-01' " );
                Assert.AreEqual( "-1", ValueArray[437], "Value of AP4DEDA should be -1 " );
                Assert.AreEqual( "''", ValueArray[438], "Value of AP4DEMT should be '' " );
                Assert.AreEqual( "-1", ValueArray[439], "Value of AP4DEAM should be -1 " );
                Assert.AreEqual( "-1", ValueArray[440], "Value of AP4COIN should be -1 " );
                Assert.AreEqual( "-1", ValueArray[441], "Value of AP4OTPK should be -1 " );
                Assert.AreEqual( "''", ValueArray[442], "Value of AP4OTPM should be '' " );
                Assert.AreEqual( "-1", ValueArray[443], "Value of AP4OTPA should be -1 " );
                Assert.AreEqual( "-1", ValueArray[444], "Value of AP4PROT should be -1 " );
                Assert.AreEqual( "-1", ValueArray[445], "Value of AP4CPAY should be -1 " );
                Assert.AreEqual( "''", ValueArray[446], "Value of AP4REMK should be '' " );
                Assert.AreEqual( "''", ValueArray[447], "Value of AP5PTMD should be '' " );
                Assert.AreEqual( "''", ValueArray[448], "Value of AP5INIF should be '' " );
                Assert.AreEqual( "1", ValueArray[449], "Value of AP6INFF should be 1 " );
                Assert.AreEqual( "''", ValueArray[450], "Value of AP6ELPH should be '' " );
                Assert.AreEqual( "''", ValueArray[451], "Value of AP6INRP should be '' " );
                Assert.AreEqual( "'0001-01-01'", ValueArray[452], "Value of AP6EFDI should be '0001-01-01' " );
                Assert.AreEqual( "'0001-01-01'", ValueArray[453], "Value of AP6TMDI should be '0001-01-01' " );
                Assert.AreEqual( "''", ValueArray[454], "Value of AP6SFPC should be '' " );
                Assert.AreEqual( "''", ValueArray[455], "Value of AP6SACB should be '' " );
                Assert.AreEqual( "''", ValueArray[456], "Value of AP6CAAV should be '' " );
                Assert.AreEqual( "''", ValueArray[457], "Value of AP6COBF should be '' " );
                Assert.AreEqual( "-1", ValueArray[458], "Value of AP6RCOB should be -1 " );
                Assert.AreEqual( "-1", ValueArray[459], "Value of AP6TPPR should be -1 " );
                Assert.AreEqual( "''", ValueArray[460], "Value of AP6NPPO should be '' " );
                Assert.AreEqual( "''", ValueArray[461], "Value of AP6HCPR should be '' " );
                Assert.AreEqual( "''", ValueArray[462], "Value of AP6ACNM should be '' " );
                Assert.AreEqual( "''", ValueArray[463], "Value of AP6MPCV should be '' " );
                Assert.AreEqual( "''", ValueArray[464], "Value of AP6REMK should be '' " );
                Assert.AreEqual( "0", ValueArray[465], "Value of AP601CTID should be 0 " );
                Assert.AreEqual( "-1", ValueArray[466], "Value of AP601DEDA should be -1 " );
                Assert.AreEqual( "''", ValueArray[467], "Value of AP601TMPR should be '' " );
                Assert.AreEqual( "''", ValueArray[468], "Value of AP601DEMT should be '' " );
                Assert.AreEqual( "-1", ValueArray[469], "Value of AP601DEAM should be -1 " );
                Assert.AreEqual( "-1", ValueArray[470], "Value of AP601COIN should be -1 " );
                Assert.AreEqual( "-1", ValueArray[471], "Value of AP601OTPK should be -1 " );
                Assert.AreEqual( "''", ValueArray[472], "Value of AP601OTPM should be '' " );
                Assert.AreEqual( "-1", ValueArray[473], "Value of AP601OTPA should be -1 " );
                Assert.AreEqual( "-1", ValueArray[474], "Value of AP601PROT should be -1 " );
                Assert.AreEqual( "-1", ValueArray[475], "Value of AP601CPAY should be -1 " );
                Assert.AreEqual( "''", ValueArray[476], "Value of AP601WCPY should be '' " );
                Assert.AreEqual( "-1", ValueArray[477], "Value of AP601NMVS should be -1 " );
                Assert.AreEqual( "-1", ValueArray[478], "Value of AP601LFMB should be -1 " );
                Assert.AreEqual( "-1", ValueArray[479], "Value of AP601LFRM should be -1 " );
                Assert.AreEqual( "''", ValueArray[480], "Value of AP601LFVM should be '' " );
                Assert.AreEqual( "-1", ValueArray[481], "Value of AP601MXBN should be -1 " );
                Assert.AreEqual( "-1", ValueArray[482], "Value of AP601RMBN should be -1 " );
                Assert.AreEqual( "''", ValueArray[483], "Value of AP601RMMT should be '' " );
                Assert.AreEqual( "0", ValueArray[484], "Value of AP602CTID should be 0 " );
                Assert.AreEqual( "-1", ValueArray[485], "Value of AP602DEDA should be -1 " );
                Assert.AreEqual( "''", ValueArray[486], "Value of AP602TMPR should be '' " );
                Assert.AreEqual( "''", ValueArray[487], "Value of AP602DEMT should be '' " );
                Assert.AreEqual( "-1", ValueArray[488], "Value of AP602DEAM should be -1 " );
                Assert.AreEqual( "-1", ValueArray[489], "Value of AP602COIN should be -1 " );
                Assert.AreEqual( "-1", ValueArray[490], "Value of AP602OTPK should be -1 " );
                Assert.AreEqual( "''", ValueArray[491], "Value of AP602OTPM should be '' " );
                Assert.AreEqual( "-1", ValueArray[492], "Value of AP602OTPA should be -1 " );
                Assert.AreEqual( "-1", ValueArray[493], "Value of AP602PROT should be -1 " );
                Assert.AreEqual( "-1", ValueArray[494], "Value of AP602CPAY should be -1 " );
                Assert.AreEqual( "''", ValueArray[495], "Value of AP602WCPY should be '' " );
                Assert.AreEqual( "-1", ValueArray[496], "Value of AP602NMVS should be -1 " );
                Assert.AreEqual( "-1", ValueArray[497], "Value of AP602LFMB should be -1 " );
                Assert.AreEqual( "-1", ValueArray[498], "Value of AP602LFRM should be -1 " );
                Assert.AreEqual( "''", ValueArray[499], "Value of AP602LFVM should be '' " );
                Assert.AreEqual( "-1", ValueArray[500], "Value of AP602MXBN should be -1 " );
                Assert.AreEqual( "-1", ValueArray[501], "Value of AP602RMBN should be -1 " );
                Assert.AreEqual( "''", ValueArray[502], "Value of AP602RMMT should be '' " );
                Assert.AreEqual( "0", ValueArray[503], "Value of AP603CTID should be 0 " );
                Assert.AreEqual( "-1", ValueArray[504], "Value of AP603DEDA should be -1 " );
                Assert.AreEqual( "''", ValueArray[505], "Value of AP603TMPR should be '' " );
                Assert.AreEqual( "''", ValueArray[506], "Value of AP603DEMT should be '' " );
                Assert.AreEqual( "-1", ValueArray[507], "Value of AP603DEAM should be -1 " );
                Assert.AreEqual( "-1", ValueArray[508], "Value of AP603COIN should be -1 " );
                Assert.AreEqual( "-1", ValueArray[509], "Value of AP603OTPK should be -1 " );
                Assert.AreEqual( "''", ValueArray[510], "Value of AP603OTPM should be '' " );
                Assert.AreEqual( "-1", ValueArray[511], "Value of AP603OTPA should be -1 " );
                Assert.AreEqual( "-1", ValueArray[512], "Value of AP603PROT should be -1 " );
                Assert.AreEqual( "-1", ValueArray[513], "Value of AP603CPAY should be -1 " );
                Assert.AreEqual( "''", ValueArray[514], "Value of AP603WCPY should be '' " );
                Assert.AreEqual( "-1", ValueArray[515], "Value of AP603NMVS should be -1 " );
                Assert.AreEqual( "-1", ValueArray[516], "Value of AP603LFMB should be -1 " );
                Assert.AreEqual( "-1", ValueArray[517], "Value of AP603LFRM should be -1 " );
                Assert.AreEqual( "''", ValueArray[518], "Value of AP603LFVM should be '' " );
                Assert.AreEqual( "-1", ValueArray[519], "Value of AP603MXBN should be -1 " );
                Assert.AreEqual( "-1", ValueArray[520], "Value of AP603RMBN should be -1 " );
                Assert.AreEqual( "''", ValueArray[521], "Value of AP603RMMT should be '' " );
                Assert.AreEqual( "0", ValueArray[522], "Value of AP604CTID should be 0 " );
                Assert.AreEqual( "-1", ValueArray[523], "Value of AP604DEDA should be -1 " );
                Assert.AreEqual( "''", ValueArray[524], "Value of AP604TMPR should be '' " );
                Assert.AreEqual( "''", ValueArray[525], "Value of AP604DEMT should be '' " );
                Assert.AreEqual( "-1", ValueArray[526], "Value of AP604DEAM should be -1 " );
                Assert.AreEqual( "-1", ValueArray[527], "Value of AP604COIN should be -1 " );
                Assert.AreEqual( "-1", ValueArray[528], "Value of AP604OTPK should be -1 " );
                Assert.AreEqual( "''", ValueArray[529], "Value of AP604OTPM should be '' " );
                Assert.AreEqual( "-1", ValueArray[530], "Value of AP604OTPA should be -1 " );
                Assert.AreEqual( "-1", ValueArray[531], "Value of AP604PROT should be -1 " );
                Assert.AreEqual( "-1", ValueArray[532], "Value of AP604CPAY should be -1 " );
                Assert.AreEqual( "''", ValueArray[533], "Value of AP604WCPY should be '' " );
                Assert.AreEqual( "-1", ValueArray[534], "Value of AP604NMVS should be -1 " );
                Assert.AreEqual( "-1", ValueArray[535], "Value of AP604LFMB should be -1 " );
                Assert.AreEqual( "-1", ValueArray[536], "Value of AP604LFRM should be -1 " );
                Assert.AreEqual( "''", ValueArray[537], "Value of AP604LFVM should be '' " );
                Assert.AreEqual( "-1", ValueArray[538], "Value of AP604MXBN should be -1 " );
                Assert.AreEqual( "-1", ValueArray[539], "Value of AP604RMBN should be -1 " );
                Assert.AreEqual( "''", ValueArray[540], "Value of AP604RMMT should be '' " );
                Assert.AreEqual( "0", ValueArray[541], "Value of AP605CTID should be 0 " );
                Assert.AreEqual( "-1", ValueArray[542], "Value of AP605DEDA should be -1 " );
                Assert.AreEqual( "''", ValueArray[543], "Value of AP605TMPR should be '' " );
                Assert.AreEqual( "''", ValueArray[544], "Value of AP605DEMT should be '' " );
                Assert.AreEqual( "-1", ValueArray[545], "Value of AP605DEAM should be -1 " );
                Assert.AreEqual( "-1", ValueArray[546], "Value of AP605COIN should be -1 " );
                Assert.AreEqual( "-1", ValueArray[547], "Value of AP605OTPK should be -1 " );
                Assert.AreEqual( "''", ValueArray[548], "Value of AP605OTPM should be '' " );
                Assert.AreEqual( "-1", ValueArray[549], "Value of AP605OTPA should be -1 " );
                Assert.AreEqual( "-1", ValueArray[550], "Value of AP605PROT should be -1 " );
                Assert.AreEqual( "-1", ValueArray[551], "Value of AP605CPAY should be -1 " );
                Assert.AreEqual( "''", ValueArray[552], "Value of AP605WCPY should be '' " );
                Assert.AreEqual( "-1", ValueArray[553], "Value of AP605NMVS should be -1 " );
                Assert.AreEqual( "-1", ValueArray[554], "Value of AP605LFMB should be -1 " );
                Assert.AreEqual( "-1", ValueArray[555], "Value of AP605LFRM should be -1 " );
                Assert.AreEqual( "''", ValueArray[556], "Value of AP605LFVM should be '' " );
                Assert.AreEqual( "-1", ValueArray[557], "Value of AP605MXBN should be -1 " );
                Assert.AreEqual( "-1", ValueArray[558], "Value of AP605RMBN should be -1 " );
                Assert.AreEqual( "''", ValueArray[559], "Value of AP605RMMT should be '' " );
                Assert.AreEqual( "0", ValueArray[560], "Value of AP606CTID should be 0 " );
                Assert.AreEqual( "-1", ValueArray[561], "Value of AP606DEDA should be -1 " );
                Assert.AreEqual( "''", ValueArray[562], "Value of AP606TMPR should be '' " );
                Assert.AreEqual( "''", ValueArray[563], "Value of AP606DEMT should be '' " );
                Assert.AreEqual( "-1", ValueArray[564], "Value of AP606DEAM should be -1 " );
                Assert.AreEqual( "-1", ValueArray[565], "Value of AP606COIN should be -1 " );
                Assert.AreEqual( "-1", ValueArray[566], "Value of AP606OTPK should be -1 " );
                Assert.AreEqual( "''", ValueArray[567], "Value of AP606OTPM should be '' " );
                Assert.AreEqual( "-1", ValueArray[568], "Value of AP606OTPA should be -1 " );
                Assert.AreEqual( "-1", ValueArray[569], "Value of AP606PROT should be -1 " );
                Assert.AreEqual( "-1", ValueArray[570], "Value of AP606CPAY should be -1 " );
                Assert.AreEqual( "''", ValueArray[571], "Value of AP606WCPY should be '' " );
                Assert.AreEqual( "-1", ValueArray[572], "Value of AP606NMVS should be -1 " );
                Assert.AreEqual( "-1", ValueArray[573], "Value of AP606LFMB should be -1 " );
                Assert.AreEqual( "-1", ValueArray[574], "Value of AP606LFRM should be -1 " );
                Assert.AreEqual( "''", ValueArray[575], "Value of AP606LFVM should be '' " );
                Assert.AreEqual( "-1", ValueArray[576], "Value of AP606MXBN should be -1 " );
                Assert.AreEqual( "-1", ValueArray[577], "Value of AP606RMBN should be -1 " );
                Assert.AreEqual( "''", ValueArray[578], "Value of AP606RMMT should be '' " );
                Assert.AreEqual( "0", ValueArray[579], "Value of AP607CTID should be 0 " );
                Assert.AreEqual( "-1", ValueArray[580], "Value of AP607DEDA should be -1 " );
                Assert.AreEqual( "''", ValueArray[581], "Value of AP607TMPR should be '' " );
                Assert.AreEqual( "''", ValueArray[582], "Value of AP607DEMT should be '' " );
                Assert.AreEqual( "-1", ValueArray[583], "Value of AP607DEAM should be -1 " );
                Assert.AreEqual( "-1", ValueArray[584], "Value of AP607COIN should be -1 " );
                Assert.AreEqual( "-1", ValueArray[585], "Value of AP607OTPK should be -1 " );
                Assert.AreEqual( "''", ValueArray[586], "Value of AP607OTPM should be '' " );
                Assert.AreEqual( "-1", ValueArray[587], "Value of AP607OTPA should be -1 " );
                Assert.AreEqual( "-1", ValueArray[588], "Value of AP607PROT should be -1 " );
                Assert.AreEqual( "-1", ValueArray[589], "Value of AP607CPAY should be -1 " );
                Assert.AreEqual( "''", ValueArray[590], "Value of AP607WCPY should be '' " );
                Assert.AreEqual( "-1", ValueArray[591], "Value of AP607NMVS should be -1 " );
                Assert.AreEqual( "-1", ValueArray[592], "Value of AP607LFMB should be -1 " );
                Assert.AreEqual( "-1", ValueArray[593], "Value of AP607LFRM should be -1 " );
                Assert.AreEqual( "''", ValueArray[594], "Value of AP607LFVM should be '' " );
                Assert.AreEqual( "-1", ValueArray[595], "Value of AP607MXBN should be -1 " );
                Assert.AreEqual( "-1", ValueArray[596], "Value of AP607RMBN should be -1 " );
                Assert.AreEqual( "''", ValueArray[597], "Value of AP607RMMT should be '' " );
                Assert.AreEqual( "0", ValueArray[598], "Value of AP608CTID should be 0 " );
                Assert.AreEqual( "-1", ValueArray[599], "Value of AP608DEDA should be -1 " );
                Assert.AreEqual( "''", ValueArray[600], "Value of AP608TMPR should be '' " );
                Assert.AreEqual( "''", ValueArray[601], "Value of AP608DEMT should be '' " );
                Assert.AreEqual( "-1", ValueArray[602], "Value of AP608DEAM should be -1 " );
                Assert.AreEqual( "-1", ValueArray[603], "Value of AP608COIN should be -1 " );
                Assert.AreEqual( "-1", ValueArray[604], "Value of AP608OTPK should be -1 " );
                Assert.AreEqual( "''", ValueArray[605], "Value of AP608OTPM should be '' " );
                Assert.AreEqual( "-1", ValueArray[606], "Value of AP608OTPA should be -1 " );
                Assert.AreEqual( "-1", ValueArray[607], "Value of AP608PROT should be -1 " );
                Assert.AreEqual( "-1", ValueArray[608], "Value of AP608CPAY should be -1 " );
                Assert.AreEqual( "''", ValueArray[609], "Value of AP608WCPY should be '' " );
                Assert.AreEqual( "-1", ValueArray[610], "Value of AP608NMVS should be -1 " );
                Assert.AreEqual( "-1", ValueArray[611], "Value of AP608LFMB should be -1 " );
                Assert.AreEqual( "-1", ValueArray[612], "Value of AP608LFRM should be -1 " );
                Assert.AreEqual( "''", ValueArray[613], "Value of AP608LFVM should be '' " );
                Assert.AreEqual( "-1", ValueArray[614], "Value of AP608MXBN should be -1 " );
                Assert.AreEqual( "-1", ValueArray[615], "Value of AP608RMBN should be -1 " );
                Assert.AreEqual( "''", ValueArray[616], "Value of AP608RMMT should be '' " );
                Assert.AreEqual( "0", ValueArray[617], "Value of AP609CTID should be 0 " );
                Assert.AreEqual( "-1", ValueArray[618], "Value of AP609DEDA should be -1 " );
                Assert.AreEqual( "''", ValueArray[619], "Value of AP609TMPR should be '' " );
                Assert.AreEqual( "''", ValueArray[620], "Value of AP609DEMT should be '' " );
                Assert.AreEqual( "-1", ValueArray[621], "Value of AP609DEAM should be -1 " );
                Assert.AreEqual( "-1", ValueArray[622], "Value of AP609COIN should be -1 " );
                Assert.AreEqual( "-1", ValueArray[623], "Value of AP609OTPK should be -1 " );
                Assert.AreEqual( "''", ValueArray[624], "Value of AP609OTPM should be '' " );
                Assert.AreEqual( "-1", ValueArray[625], "Value of AP609OTPA should be -1 " );
                Assert.AreEqual( "-1", ValueArray[626], "Value of AP609PROT should be -1 " );
                Assert.AreEqual( "-1", ValueArray[627], "Value of AP609CPAY should be -1 " );
                Assert.AreEqual( "''", ValueArray[628], "Value of AP609WCPY should be '' " );
                Assert.AreEqual( "-1", ValueArray[629], "Value of AP609NMVS should be -1 " );
                Assert.AreEqual( "-1", ValueArray[630], "Value of AP609LFMB should be -1 " );
                Assert.AreEqual( "-1", ValueArray[631], "Value of AP609LFRM should be -1 " );
                Assert.AreEqual( "''", ValueArray[632], "Value of AP609LFVM should be '' " );
                Assert.AreEqual( "-1", ValueArray[633], "Value of AP609MXBN should be -1 " );
                Assert.AreEqual( "-1", ValueArray[634], "Value of AP609RMBN should be -1 " );
                Assert.AreEqual( "''", ValueArray[635], "Value of AP609RMMT should be '' " );
                Assert.AreEqual( "0", ValueArray[636], "Value of AP610CTID should be 0 " );
                Assert.AreEqual( "-1", ValueArray[637], "Value of AP610DEDA should be -1 " );
                Assert.AreEqual( "''", ValueArray[638], "Value of AP610TMPR should be '' " );
                Assert.AreEqual( "''", ValueArray[639], "Value of AP610DEMT should be '' " );
                Assert.AreEqual( "-1", ValueArray[640], "Value of AP610DEAM should be -1 " );
                Assert.AreEqual( "-1", ValueArray[641], "Value of AP610COIN should be -1 " );
                Assert.AreEqual( "-1", ValueArray[642], "Value of AP610OTPK should be -1 " );
                Assert.AreEqual( "''", ValueArray[643], "Value of AP610OTPM should be '' " );
                Assert.AreEqual( "-1", ValueArray[644], "Value of AP610OTPA should be -1 " );
                Assert.AreEqual( "-1", ValueArray[645], "Value of AP610PROT should be -1 " );
                Assert.AreEqual( "-1", ValueArray[646], "Value of AP610CPAY should be -1 " );
                Assert.AreEqual( "''", ValueArray[647], "Value of AP610WCPY should be '' " );
                Assert.AreEqual( "-1", ValueArray[648], "Value of AP610NMVS should be -1 " );
                Assert.AreEqual( "-1", ValueArray[649], "Value of AP610LFMB should be -1 " );
                Assert.AreEqual( "-1", ValueArray[650], "Value of AP610LFRM should be -1 " );
                Assert.AreEqual( "''", ValueArray[651], "Value of AP610LFVM should be '' " );
                Assert.AreEqual( "-1", ValueArray[652], "Value of AP610MXBN should be -1 " );
                Assert.AreEqual( "-1", ValueArray[653], "Value of AP610RMBN should be -1 " );
                Assert.AreEqual( "''", ValueArray[654], "Value of AP610RMMT should be '' " );
                Assert.AreEqual( "0", ValueArray[655], "Value of AP611CTID should be 0 " );
                Assert.AreEqual( "-1", ValueArray[656], "Value of AP611DEDA should be -1 " );
                Assert.AreEqual( "''", ValueArray[657], "Value of AP611TMPR should be '' " );
                Assert.AreEqual( "''", ValueArray[658], "Value of AP611DEMT should be '' " );
                Assert.AreEqual( "-1", ValueArray[659], "Value of AP611DEAM should be -1 " );
                Assert.AreEqual( "-1", ValueArray[660], "Value of AP611COIN should be -1 " );
                Assert.AreEqual( "-1", ValueArray[661], "Value of AP611OTPK should be -1 " );
                Assert.AreEqual( "''", ValueArray[662], "Value of AP611OTPM should be '' " );
                Assert.AreEqual( "-1", ValueArray[663], "Value of AP611OTPA should be -1 " );
                Assert.AreEqual( "-1", ValueArray[664], "Value of AP611PROT should be -1 " );
                Assert.AreEqual( "-1", ValueArray[665], "Value of AP611CPAY should be -1 " );
                Assert.AreEqual( "''", ValueArray[666], "Value of AP611WCPY should be '' " );
                Assert.AreEqual( "-1", ValueArray[667], "Value of AP611NMVS should be -1 " );
                Assert.AreEqual( "-1", ValueArray[668], "Value of AP611LFMB should be -1 " );
                Assert.AreEqual( "-1", ValueArray[669], "Value of AP611LFRM should be -1 " );
                Assert.AreEqual( "''", ValueArray[670], "Value of AP611LFVM should be '' " );
                Assert.AreEqual( "-1", ValueArray[671], "Value of AP611MXBN should be -1 " );
                Assert.AreEqual( "-1", ValueArray[672], "Value of AP611RMBN should be -1 " );
                Assert.AreEqual( "''", ValueArray[673], "Value of AP611RMMT should be '' " );
                Assert.AreEqual( "0", ValueArray[674], "Value of AP612CTID should be 0 " );
                Assert.AreEqual( "-1", ValueArray[675], "Value of AP612DEDA should be -1 " );
                Assert.AreEqual( "''", ValueArray[676], "Value of AP612TMPR should be '' " );
                Assert.AreEqual( "''", ValueArray[677], "Value of AP612DEMT should be '' " );
                Assert.AreEqual( "-1", ValueArray[678], "Value of AP612DEAM should be -1 " );
                Assert.AreEqual( "-1", ValueArray[679], "Value of AP612COIN should be -1 " );
                Assert.AreEqual( "-1", ValueArray[680], "Value of AP612OTPK should be -1 " );
                Assert.AreEqual( "''", ValueArray[681], "Value of AP612OTPM should be '' " );
                Assert.AreEqual( "-1", ValueArray[682], "Value of AP612OTPA should be -1 " );
                Assert.AreEqual( "-1", ValueArray[683], "Value of AP612PROT should be -1 " );
                Assert.AreEqual( "-1", ValueArray[684], "Value of AP612CPAY should be -1 " );
                Assert.AreEqual( "''", ValueArray[685], "Value of AP612WCPY should be '' " );
                Assert.AreEqual( "-1", ValueArray[686], "Value of AP612NMVS should be -1 " );
                Assert.AreEqual( "-1", ValueArray[687], "Value of AP612LFMB should be -1 " );
                Assert.AreEqual( "-1", ValueArray[688], "Value of AP612LFRM should be -1 " );
                Assert.AreEqual( "''", ValueArray[689], "Value of AP612LFVM should be '' " );
                Assert.AreEqual( "-1", ValueArray[690], "Value of AP612MXBN should be -1 " );
                Assert.AreEqual( "-1", ValueArray[691], "Value of AP612RMBN should be -1 " );
                Assert.AreEqual( "''", ValueArray[692], "Value of AP612RMMT should be '' " );
                Assert.AreEqual("'234 MulHolland Drive'", ValueArray[698], "Value of APJADRE1 should be '234 MulHolland Drive' ");
                Assert.AreEqual("'#1'", ValueArray[699], "Value of APJADRE2 should be '#1' ");
            }
        }

        #endregion

        #region Data Elements

        private static Account anAccount;
        private static readonly TransactionKeys transactionKeys = new TransactionKeys();

        private static readonly Address addressB = new Address( ADDRESS1B,
                                                               ADDRESS2B,
                                                               CITYB,
                                                               new ZipCode( POSTALCODEB ),
                                                               new State( PersistentModel.NEW_OID,
                                                                         PersistentModel.NEW_VERSION,
                                                                         "TEXAS",
                                                                         "TX" ),
                                                               new Country( PersistentModel.NEW_OID,
                                                                           PersistentModel.NEW_VERSION,
                                                                           "United States",
                                                                           "USA" ),
                                                               new County( PersistentModel.NEW_OID,
                                                                          PersistentModel.NEW_VERSION,
                                                                          "ORANGE",
                                                                          "122" )
            );

        private static readonly Address addressE = new Address( ADDRESS1E,
                                                               ADDRESS2E,
                                                               CITYE,
                                                               new ZipCode( POSTALCODEE ),
                                                               new State( PersistentModel.NEW_OID,
                                                                         PersistentModel.NEW_VERSION,
                                                                         "TEXAS",
                                                                         "TX" ),
                                                               new Country( PersistentModel.NEW_OID,
                                                                           PersistentModel.NEW_VERSION,
                                                                           "United States",
                                                                           "USA" ),
                                                               new County( PersistentModel.NEW_OID,
                                                                          PersistentModel.NEW_VERSION,
                                                                          "ORANGE",
                                                                          "123" )
            );

        private static readonly Name
            INSURED_NAME1 =
                new Name( INSURED_F_NAME1, INSURED_L_NAME1, INSURED_MI1 );

        private static readonly Name
            INSURED_NAME2 =
                new Name( INSURED_F_NAME2, INSURED_L_NAME2, INSURED_MI2 );

        private static readonly PhoneNumber phoneNumberE = new PhoneNumber( "972", "546789" );
        private static readonly PhoneNumber phoneNumberB = new PhoneNumber( "972", "553453" );

        private static readonly EmailAddress emailAddress = new EmailAddress( "FC@YAHOO.COM" );

        private static readonly TypeOfContactPoint typeOfContactPointB =
            new TypeOfContactPoint( 3, "BILLING" );

        private static readonly TypeOfContactPoint typeOfContactPointE =
            new TypeOfContactPoint( 1, "EMPLOYER" );

        private static readonly object[] billingInformation = new object[2];
        private static InsuranceInsertStrategy i_InsuranceInsertStrategy;
        private static Facility facility_ACO;
        private static Account account;

        #endregion

        #region Constants
        private const int
            NUMBER_OF_ENTRIES = 700;

        private const string
            INSURED_F_NAME1 = "HAPPY",
            INSURED_L_NAME1 = "SADSTORY",
            INSURED_F_NAME2 = "SOMETHING",
            INSURED_L_NAME2 = "SADSTORY",
            INSURED_MI2 = "N";

        private const string ADDRESS1B = "335 Nicholson Dr.",
                             ADDRESS2B = "#303",
                             CITYB = "Austin",
                             POSTALCODEB = "605034000";

        private const string ADDRESS1E = "234 MulHolland Drive",
                             ADDRESS2E = "#1",
                             CITYE = "Austin",
                             POSTALCODEE = "605944000";

        private const int PRIMARY_COVERAGE = 1;
        private const int SECONDARY_COVERAGE = 2;
        private const string CURRENT_TIMESTAMP = "'0001-01-01-00.00.00.000000'";
        private static readonly string INSURED_MI1 = string.Empty;

        #endregion
    }
}