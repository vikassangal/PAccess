using System;
using System.Collections;
using Extensions.PersistenceCommon;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Persistence;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    /// <summary>
    /// Summary description for TransferInPatientToOutPatientInsertStrategyTests.
    /// </summary>
    [TestFixture]
    public class TransferInPatientToOutPatientInsertStrategyTests
    {
        #region SetUp and TearDown TransferPatientToNewLocationInsertStrategyTest
        #endregion

        #region Test Methods
        [Test]
        public void TestBuildSqlFromForTransferInToOutActivity()
        {
            IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            Facility facility_ACO = facilityBroker.FacilityWith( FACILITY_ID );

            Gender patientSex = new Gender( 2, DateTime.Now, "Male", "M" );
            DateTime patientDOB = new DateTime( 1965, 01, 13 );
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
            AccountProxy proxy = new AccountProxy( 30015,
                                   patient,
                                   DateTime.Now,
                                   DateTime.Now,
                                   new VisitType( 0, PersistentModel.NEW_VERSION, VisitType.OUTPATIENT_DESC, VisitType.OUTPATIENT ),
                                   facility_ACO,
                                   new FinancialClass( 299, PersistentModel.NEW_VERSION, "MEDICARE", "40" ),
                                   new HospitalService( 0, PersistentModel.NEW_VERSION, "DIAGNOSTIC OUTOPT", "60" ),
                                   "OL HSV60",
                                   false );

            IAccountBroker accountBroker = BrokerFactory.BrokerOfType<IAccountBroker>();
            Account anAccount = accountBroker.AccountFor( proxy );

            TransferInToOutActivity currentActivity = new TransferInToOutActivity { Remarks = "Transfer In to Out Remarks" };
            anAccount.Activity = currentActivity;
            Assert.AreEqual(
                30015,
                anAccount.AccountNumber,
                "AccountNumber should be 30015" );

            //Location Info.
            NursingStation nursingStationFrom = new NursingStation( PersistentModel.NEW_OID, DateTime.Now, "6N", "6N" );
            Room roomFrom = new Room( PersistentModel.NEW_OID, DateTime.Now, "610", "610" );
            Bed bedFrom = new Bed( PersistentModel.NEW_OID, DateTime.Now, "A", "A" );
            Location location = new Location { NursingStation = nursingStationFrom, Room = roomFrom, Bed = bedFrom };
            anAccount.Location = location;

            anAccount.CodedDiagnoses = new CodedDiagnoses();
            anAccount.CodedDiagnoses.CodedDiagnosises.Add( "123.1" );
            anAccount.CodedDiagnoses.AdmittingCodedDiagnosises.Add( "321.1" );

            ConfidentialCode confidentialCode = new ConfidentialCode { Code = "A", Description = "ADOPTION" };
            anAccount.ConfidentialityCode = confidentialCode;

            Physician physician = new Physician( 123, PersistentModel.NEW_VERSION, 1, 1, 1, 1, 1, 1 )
                                      {
                                          FirstName = "Doctor",
                                          LastName = "Bob",
                                          UPIN = "UP234",
                                          NPI = "NI234"
                                      };
            Address addr = new Address( "123 Dr Place", string.Empty, "Healthland", new ZipCode( "11223" ),
                                       new State( 1, PersistentModel.NEW_VERSION, "Texas", "TX" ),
                                       new Country( 1, PersistentModel.NEW_VERSION, "United States", "USA" ) );
            PhoneNumber pn = new PhoneNumber( "111", "2222222" );
            physician.AddContactPoint( new ContactPoint( addr, pn,
                                                       new PhoneNumber(), new EmailAddress(), TypeOfContactPoint.NewBillingContactPointType() ) );
            anAccount.AddPhysicianWithRole( PhysicianRole.Admitting(), physician );

            // initialize user with values explicitly
            User appUser = User.GetCurrent();
            appUser.PBAREmployeeID = "PAUSRT03";
            appUser.WorkstationID = string.Empty;
            appUser.UserID = "PAUSRT03";

            IConditionCodeBroker conditionCodeBroker = BrokerFactory.BrokerOfType<IConditionCodeBroker>();
            ConditionCode conditionCode = conditionCodeBroker.ConditionCodeWith( FACILITY_ID, ConditionCode.CONDITIONCODE_PRIVATE_ROOM_MEDICALLY_REQUIRED );
            ConditionCode conditionCode1 = conditionCodeBroker.ConditionCodeWith( FACILITY_ID, ConditionCode.CONDITIONCODE_DOB_OVER_100Y );

            anAccount.ConditionCodes.Add( conditionCode );
            anAccount.ConditionCodes.Add( conditionCode1 );
            anAccount.AdmittingCategory = "3";

            TransferInPatientToOutPatientInsertStrategy transferInPatientToOutPatientInsertStrategy
                = new TransferInPatientToOutPatientInsertStrategy();

            ArrayList sqlStrings =
                transferInPatientToOutPatientInsertStrategy.BuildSqlFrom( anAccount, transactionKeys );
            foreach ( string sqlString in sqlStrings )
            {
                int startPositionOfValues =
                    sqlString.IndexOf( "(", sqlString.IndexOf( ")" ) ) + 1;
                int lengthOfValues =
                    sqlString.Length - sqlString.IndexOf( "(", sqlString.IndexOf( ")" ) ) - 2;
                string[] ValueArray =
                    sqlString.Substring( startPositionOfValues, lengthOfValues ).Split( ',' );

                Assert.AreEqual( NUMBER_OF_ENTRIES, ValueArray.Length, "Wrong Number of entries in ValueArray" );

                Assert.AreEqual( " ''", ValueArray[0], "Value of APIDWS should be  '' " );
                Assert.AreEqual( "''", ValueArray[1], "Value of APIDID should be '' " );
                Assert.AreEqual( "'$#P@%&'", ValueArray[2], "Value of APRR# should be '$#P@%&' " );
                Assert.AreEqual( "''", ValueArray[3], "Value of APSEC2 should be '' " );
                Assert.AreEqual( "900", ValueArray[4], "Value of APHSP# should be 900 " );
                Assert.AreEqual( "30015", ValueArray[5], "Value of APACCT should be 30015 " );
                Assert.AreEqual( "0", ValueArray[6], "Value of APLAD should be 0 " );
                Assert.AreEqual( "0", ValueArray[7], "Value of APLAT should be 0 " );
                Assert.AreEqual( "'3'", ValueArray[8], "Value of APADMC should be '3'");
                Assert.AreEqual( "'6N'", ValueArray[9], "Value of APNS should be '6N' " );
                Assert.AreEqual( "610", ValueArray[10], "Value of APROOM should be 610 " );
                Assert.AreEqual( "'A'", ValueArray[11], "Value of APBED should be 'A' " );
                Assert.AreEqual( "'1'", ValueArray[12], "Value of APPTYP should be '1' " );
                Assert.AreEqual( "''", ValueArray[13], "Value of APMSV should be '' " );
                Assert.AreEqual( "''", ValueArray[14], "Value of APSMOK should be '' " );
                Assert.AreEqual( "''", ValueArray[15], "Value of APDIET should be '' " );
                Assert.AreEqual( "''", ValueArray[16], "Value of APCOND should be '' " );
                Assert.AreEqual( "''", ValueArray[17], "Value of APISO should be '' " );
                Assert.AreEqual( "''", ValueArray[18], "Value of APTCRT should be '' " );
                Assert.AreEqual( "0", ValueArray[19], "Value of APDCRT should be 0 " );
                Assert.AreEqual( "0", ValueArray[20], "Value of APSCRT should be 0 " );
                Assert.AreEqual( "0", ValueArray[21], "Value of APNCRT should be 0 " );
                Assert.AreEqual( "0", ValueArray[22], "Value of APICRT should be 0 " );
                Assert.AreEqual( "0", ValueArray[23], "Value of AP#EXT should be 0 " );
                Assert.AreEqual( "0", ValueArray[24], "Value of APDTOT should be 0 " );
                Assert.AreEqual( "0", ValueArray[25], "Value of APLML should be 0 " );
                Assert.AreEqual( "0", ValueArray[26], "Value of APLMD should be 0 " );
                Assert.AreEqual( "0", ValueArray[27], "Value of APLUL# should be 0 " );
                Assert.AreEqual( "'A'", ValueArray[28], "Value of APACFL should be 'A' " );
                //Assert.AreEqual( "60944", ValueArray[29],"Value of APTTME should be 60944 " );
                Assert.AreEqual( "'$#L@%'", ValueArray[30], "Value of APINLG should be '$#L@%' " );
                Assert.AreEqual( "''", ValueArray[31], "Value of APBYPS should be '' " );
                Assert.AreEqual( "0", ValueArray[32], "Value of APSWPY should be 0 " );
                Assert.AreEqual( "''", ValueArray[33], "Value of APPSRC should be '' " );
                Assert.AreEqual( "0", ValueArray[34], "Value of APADR# should be 0 " );
                Assert.AreEqual( "'321.1'", ValueArray[35], "Value of APRA01 should be '321.1' " );
                Assert.AreEqual( "''", ValueArray[36], "Value of APRA02 should be '' " );
                Assert.AreEqual( "''", ValueArray[37], "Value of APRA03 should be '' " );
                Assert.AreEqual( "''", ValueArray[38], "Value of APRA04 should be '' " );
                Assert.AreEqual( "''", ValueArray[39], "Value of APRA05 should be '' " );
                Assert.AreEqual( "''", ValueArray[40], "Value of APCC01 should be '' " );
                Assert.AreEqual( "''", ValueArray[41], "Value of APCC02 should be '' " );
                Assert.AreEqual( "''", ValueArray[42], "Value of APCC03 should be '' " );
                Assert.AreEqual( "''", ValueArray[43], "Value of APDIAG should be '' " );
                Assert.AreEqual( "''", ValueArray[44], "Value of APACC should be '' " );
                // Assert.AreEqual( "113006", ValueArray[45],"Value of APTDAT should be 113006 " );
                Assert.AreEqual( "0", ValueArray[46], "Value of APACTP should be 0 " );
                Assert.AreEqual( "''", ValueArray[47], "Value of APXMIT should be '' " );
                Assert.AreEqual( "0", ValueArray[48], "Value of APQNUM should be 0 " );
                Assert.AreEqual( "''", ValueArray[49], "Value of APUPRV should be '' " );
                Assert.AreEqual( "''", ValueArray[50], "Value of APUPRW should be '' " );
                Assert.AreEqual( "''", ValueArray[51], "Value of APZDTE should be '' " );
                Assert.AreEqual( "''", ValueArray[52], "Value of APZTME should be '' " );
                Assert.AreEqual( "123", ValueArray[53], "Value of APMDR# should be 123 " );
                Assert.AreEqual( "'1'", ValueArray[54], "Value of APTREG should be '1' " );
                Assert.AreEqual( "0", ValueArray[55], "Value of APRDR# should be 0 " );
                Assert.AreEqual( "'CP'", ValueArray[56], "Value of APCL01 should be 'CP' " );
                Assert.AreEqual( "''", ValueArray[57], "Value of APCL02 should be '' " );
                Assert.AreEqual( "''", ValueArray[58], "Value of APCL03 should be '' " );
                Assert.AreEqual( "''", ValueArray[59], "Value of APCL04 should be '' " );
                Assert.AreEqual( "''", ValueArray[60], "Value of APCL05 should be '' " );
                Assert.AreEqual( "''", ValueArray[61], "Value of APVALU should be '' " );
                Assert.AreEqual( "''", ValueArray[62], "Value of APFC should be '' " );
                Assert.AreEqual( "'BP'", ValueArray[63], "Value of APRLGN should be 'BP' " );
                Assert.AreEqual( "'Transfer In to Out Remarks'", ValueArray[64].Trim(),
                                 "Value of APCOM1 should be 'Transfer In to Out Remarks' " );
                Assert.AreEqual( "''", ValueArray[65].Trim(), "Value of APCOM2 should be '' " );
                Assert.AreEqual( "'UP234'", ValueArray[66], "Value of APMNU# should be 'UP234' " );
                Assert.AreEqual( "'BobDoctor'", ValueArray[67], "Value of APMNDN should be 'BobDoctor' " );
                Assert.AreEqual( "''", ValueArray[68], "Value of APMNFR should be '' " );
                Assert.AreEqual( "0", ValueArray[69], "Value of APRNU# should be 0 " );
                Assert.AreEqual( "0", ValueArray[70], "Value of APRNDN should be 0 " );
                Assert.AreEqual( "''", ValueArray[71], "Value of APRNFR should be '' " );
                Assert.AreEqual( "''", ValueArray[72], "Value of APSPNC should be '' " );
                Assert.AreEqual( "0", ValueArray[73], "Value of APSPFR should be 0 " );
                Assert.AreEqual( "0", ValueArray[74], "Value of APSPTO should be 0 " );
                Assert.AreEqual( "''", ValueArray[75], "Value of APINSN should be '' " );
                Assert.AreEqual( "''", ValueArray[76], "Value of APSPN2 should be '' " );
                Assert.AreEqual( "0", ValueArray[77], "Value of APAPFR should be 0 " );
                Assert.AreEqual( "0", ValueArray[78], "Value of APAPTO should be 0 " );
                Assert.AreEqual( "'A'", ValueArray[79], "Value of APCNFG should be 'A' " );
                Assert.AreEqual( "0", ValueArray[80], "Value of APMNCD should be 0 " );
                Assert.AreEqual( "0", ValueArray[81], "Value of APMNP# should be 0 " );
                Assert.AreEqual( "'CCS'", ValueArray[82], "Value of APPARC should be 'CCS' " );
                Assert.AreEqual( "''", ValueArray[83], "Value of APOPD# should be '' " );
                Assert.AreEqual( "''", ValueArray[84], "Value of APTDR# should be '' " );
                Assert.AreEqual( "'Bob'", ValueArray[85], "Value of APMNLN should be 'Bob' " );
                Assert.AreEqual( "'Doctor'", ValueArray[86], "Value of APMNFN should be 'Doctor' " );
                Assert.AreEqual( "''", ValueArray[87], "Value of APMNMN should be '' " );
                Assert.AreEqual( "'NI234'", ValueArray[88], "Value of APMNPR should be 'NI234' " );
                Assert.AreEqual( "''", ValueArray[89], "Value of APMTXC should be '' " );
                Assert.AreEqual( "''", ValueArray[90], "Value of APRNLN should be '' " );
                Assert.AreEqual( "''", ValueArray[91], "Value of APRNFN should be '' " );
                Assert.AreEqual( "''", ValueArray[92], "Value of APRNMN should be '' " );
                Assert.AreEqual( "''", ValueArray[93], "Value of APRNPR should be '' " );
                Assert.AreEqual( "''", ValueArray[94], "Value of APRTXC should be '' " );
                Assert.AreEqual( "''", ValueArray[95], "Value of APRNP# should be '' " );
                Assert.AreEqual( "''", ValueArray[96], "Value of APANLN should be '' " );
                Assert.AreEqual( "''", ValueArray[97], "Value of APANFN should be '' " );
                Assert.AreEqual( "''", ValueArray[98], "Value of APANMN should be '' " );
                Assert.AreEqual( "''", ValueArray[99], "Value of APANU# should be '' " );
                Assert.AreEqual( "''", ValueArray[100], "Value of APANPR should be '' " );
                Assert.AreEqual( "''", ValueArray[101], "Value of APATXC should be '' " );
                Assert.AreEqual( "''", ValueArray[102], "Value of APANP# should be '' " );
                Assert.AreEqual( "''", ValueArray[103], "Value of APONLN should be '' " );
                Assert.AreEqual( "''", ValueArray[104], "Value of APONFN should be '' " );
                Assert.AreEqual( "''", ValueArray[105], "Value of APONMN should be '' " );
                Assert.AreEqual( "''", ValueArray[106], "Value of APONU# should be '' " );
                Assert.AreEqual( "''", ValueArray[107], "Value of APONPR should be '' " );
                Assert.AreEqual( "''", ValueArray[108], "Value of APOTXC should be '' " );
                Assert.AreEqual( "''", ValueArray[109], "Value of APONP# should be '' " );
                Assert.AreEqual( "''", ValueArray[110], "Value of APTNLN should be '' " );
                Assert.AreEqual( "''", ValueArray[111], "Value of APTNFN should be '' " );
                Assert.AreEqual( "''", ValueArray[112], "Value of APTNMN should be '' " );
                Assert.AreEqual( "''", ValueArray[113], "Value of APTNU# should be '' " );
                Assert.AreEqual( "''", ValueArray[114], "Value of APTNPR should be '' " );
                Assert.AreEqual( "''", ValueArray[115], "Value of APTTXC should be '' " );
                Assert.AreEqual( "''", ValueArray[116], "Value of APTNP# should be '' " );
                Assert.AreEqual( "''", ValueArray[117], "Value of APMSL# should be '' " );
                Assert.AreEqual( "''", ValueArray[118], "Value of APRSL# should be '' " );
                Assert.AreEqual( "''", ValueArray[119], "Value of APASL# should be '' " );
                Assert.AreEqual( "''", ValueArray[120], "Value of APOSL# should be '' " );
                Assert.AreEqual( "''", ValueArray[121], "Value of APTSL# should be '' " );
                Assert.AreEqual( "'PACCESS'", ValueArray[122], "Value of APWSIR should be 'PACCESS' " );
                Assert.AreEqual( "''", ValueArray[123], "Value of APSECR should be '' " );
                Assert.AreEqual( "''", ValueArray[124], "Value of APORR1 should be '' " );
                Assert.AreEqual( "''", ValueArray[125], "Value of APORR2 should be '' " );
                Assert.AreEqual( "''", ValueArray[126], "Value of APORR3 should be '' " );
                Assert.AreEqual( "'02'", ValueArray[127], "Value of APCI01 should be 02' " );
                Assert.AreEqual( "'" + ConditionCode.CONDITIONCODE_PRIVATE_ROOM_MEDICALLY_REQUIRED + "'", ValueArray[128], "Value of APCI02 should be '" + ConditionCode.CONDITIONCODE_PRIVATE_ROOM_MEDICALLY_REQUIRED + "' " );
                Assert.AreEqual( "'" + ConditionCode.CONDITIONCODE_DOB_OVER_100Y + "'", ValueArray[129], "Value of APCI02 should be '" + ConditionCode.CONDITIONCODE_DOB_OVER_100Y + "' " );
                Assert.AreEqual( "''", ValueArray[130], "Value of APCI04 should be '' " );
                Assert.AreEqual( "''", ValueArray[131], "Value of APCI05 should be '' " );
                Assert.AreEqual( "''", ValueArray[132], "Value of APCI06 should be '' " );
                Assert.AreEqual( "''", ValueArray[133], "Value of APCI07 should be '' " );

            }
        }

        [Test]
        public void TestBuildSqlFromForTransferOutToInActivity()
        {
            IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            Facility facility_ACO = facilityBroker.FacilityWith( FACILITY_ID );

            Gender patientSex = new Gender( 2, DateTime.Now, "Male", "M" );
            DateTime patientDOB = new DateTime( 1965, 01, 13 );
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
            AccountProxy proxy = new AccountProxy( 30015,
                                   patient,
                                   DateTime.Now,
                                   DateTime.Now,
                                   new VisitType( 0, PersistentModel.NEW_VERSION, VisitType.OUTPATIENT_DESC, VisitType.OUTPATIENT ),
                                   facility_ACO,
                                   new FinancialClass( 299, PersistentModel.NEW_VERSION, "MEDICARE", "40" ),
                                   new HospitalService( 0, PersistentModel.NEW_VERSION, "DIAGNOSTIC OUTOPT", "60" ),
                                   "OL HSV60",
                                   false );

            IAccountBroker accountBroker = BrokerFactory.BrokerOfType<IAccountBroker>();
            Account anAccount = accountBroker.AccountFor( proxy );

            Activity currentActivity =
                new TransferOutToInActivity();
            anAccount.Activity = currentActivity;
            anAccount.ClinicalComments = "Clinical comments for Out to In Transfer";
            Assert.AreEqual(
                30015,
                anAccount.AccountNumber,
                "AccountNumber should be 30015" );

            //Location Info.
            NursingStation nursingStationFrom = new NursingStation( PersistentModel.NEW_OID, DateTime.Now, "6N", "6N" );
            Room roomFrom = new Room( PersistentModel.NEW_OID, DateTime.Now, "610", "610" );
            Bed bedFrom = new Bed( PersistentModel.NEW_OID, DateTime.Now, "A", "A" );
            Location location = new Location { NursingStation = nursingStationFrom, Room = roomFrom, Bed = bedFrom };
            anAccount.Location = location;

            anAccount.CodedDiagnoses = new CodedDiagnoses();
            anAccount.CodedDiagnoses.CodedDiagnosises.Add( "123.1" );
            anAccount.CodedDiagnoses.AdmittingCodedDiagnosises.Add( "321.1" );

            ConfidentialCode confidentialCode = new ConfidentialCode { Code = "A", Description = "ADOPTION" };
            anAccount.ConfidentialityCode = confidentialCode;

            Physician physician = new Physician( 123, PersistentModel.NEW_VERSION, 1, 1, 1, 1, 1, 1 )
                                      {
                                          FirstName = "Doctor",
                                          LastName = "Bob",
                                          UPIN = "UP234",
                                          NPI = "NI234"
                                      };
            Address addr = new Address( "123 Dr Place", string.Empty, "Healthland", new ZipCode( "11223" ),
                                       new State( 1, PersistentModel.NEW_VERSION, "Texas", "TX" ),
                                       new Country( 1, PersistentModel.NEW_VERSION, "United States", "USA" ) );
            PhoneNumber pn = new PhoneNumber( "111", "2222222" );
            physician.AddContactPoint( new ContactPoint( addr, pn,
                                                       new PhoneNumber(), new EmailAddress(), TypeOfContactPoint.NewBillingContactPointType() ) );
            anAccount.AddPhysicianWithRole( PhysicianRole.Admitting(), physician );

            // initialize user with values explicitly
            User appUser = User.GetCurrent();
            appUser.PBAREmployeeID = "PAUSRT03";
            appUser.WorkstationID = string.Empty;
            appUser.UserID = "PAUSRT03";
            IConditionCodeBroker conditionCodeBroker = BrokerFactory.BrokerOfType<IConditionCodeBroker>();
            ConditionCode conditionCode = conditionCodeBroker.ConditionCodeWith( FACILITY_ID, ConditionCode.CONDITIONCODE_PRIVATE_ROOM_MEDICALLY_REQUIRED );
            ConditionCode conditionCode1 = conditionCodeBroker.ConditionCodeWith( FACILITY_ID, ConditionCode.CONDITIONCODE_DOB_OVER_100Y );

            anAccount.ConditionCodes.Add( conditionCode );
            anAccount.ConditionCodes.Add( conditionCode1 );

            ISpanCodeBroker scBroker = new SpanCodeBrokerProxy();
            SpanCode spanCode70 = scBroker.SpanCodeWith( 900, SpanCode.QUALIFYING_STAY_DATES );
            SpanCode spanCode71 = scBroker.SpanCodeWith( 900, SpanCode.PRIOR_STAY_DATES );
            
            OccurrenceSpan occurrenceSpan1 = new OccurrenceSpan
            {
                SpanCode = spanCode70,
                FromDate = new DateTime(2010, 01, 01),
                ToDate = new DateTime(2010, 02, 02),
                Facility = "Span Facility"
            };

            OccurrenceSpan occurrenceSpan2 = new OccurrenceSpan
            {
                SpanCode = spanCode71,
                FromDate = new DateTime(2010, 03, 03),
                ToDate = new DateTime(2010, 04, 04)
            };

            anAccount.OccurrenceSpans.Clear();
            anAccount.OccurrenceSpans.Add( occurrenceSpan1 );
            anAccount.OccurrenceSpans.Add( occurrenceSpan2 );
            anAccount.AdmittingCategory = "2";
            TransferInPatientToOutPatientInsertStrategy transferInPatientToOutPatientInsertStrategy
                = new TransferInPatientToOutPatientInsertStrategy();

            ArrayList sqlStrings =
                transferInPatientToOutPatientInsertStrategy.BuildSqlFrom( anAccount, transactionKeys );
            foreach ( string sqlString in sqlStrings )
            {
                int startPositionOfValues =
                    sqlString.IndexOf( "(", sqlString.IndexOf( ")" ) ) + 1;
                int lengthOfValues =
                    sqlString.Length - sqlString.IndexOf( "(", sqlString.IndexOf( ")" ) ) - 2;
                string[] ValueArray =
                    sqlString.Substring( startPositionOfValues, lengthOfValues ).Split( ',' );

                Assert.AreEqual( NUMBER_OF_ENTRIES, ValueArray.Length, "Wrong Number of entries in ValueArray" );

                Assert.AreEqual( " ''", ValueArray[0], "Value of APIDWS should be  '' " );
                Assert.AreEqual( "''", ValueArray[1], "Value of APIDID should be '' " );
                Assert.AreEqual( "'$#P@%&'", ValueArray[2], "Value of APRR# should be '$#P@%&' " );
                Assert.AreEqual( "''", ValueArray[3], "Value of APSEC2 should be '' " );
                Assert.AreEqual( "900", ValueArray[4], "Value of APHSP# should be 900 " );
                Assert.AreEqual( "30015", ValueArray[5], "Value of APACCT should be 30015 " );
                Assert.AreEqual( "0", ValueArray[6], "Value of APLAD should be 0 " );
                Assert.AreEqual( "0", ValueArray[7], "Value of APLAT should be 0 " );
                Assert.AreEqual( "'" + anAccount.AdmittingCategory + "'", ValueArray[8], "Value of APADMC should not be changed." );
                Assert.AreEqual( "'6N'", ValueArray[9], "Value of APNS should be '6N' " );
                Assert.AreEqual( "610", ValueArray[10], "Value of APROOM should be 610 " );
                Assert.AreEqual( "'A'", ValueArray[11], "Value of APBED should be 'A' " );
                Assert.AreEqual( "'1'", ValueArray[12], "Value of APPTYP should be '1' " );
                Assert.AreEqual( "''", ValueArray[13], "Value of APMSV should be '' " );
                Assert.AreEqual( "''", ValueArray[14], "Value of APSMOK should be '' " );
                Assert.AreEqual( "''", ValueArray[15], "Value of APDIET should be '' " );
                Assert.AreEqual( "''", ValueArray[16], "Value of APCOND should be '' " );
                Assert.AreEqual( "''", ValueArray[17], "Value of APISO should be '' " );
                Assert.AreEqual( "''", ValueArray[18], "Value of APTCRT should be '' " );
                Assert.AreEqual( "0", ValueArray[19], "Value of APDCRT should be 0 " );
                Assert.AreEqual( "0", ValueArray[20], "Value of APSCRT should be 0 " );
                Assert.AreEqual( "0", ValueArray[21], "Value of APNCRT should be 0 " );
                Assert.AreEqual( "0", ValueArray[22], "Value of APICRT should be 0 " );
                Assert.AreEqual( "0", ValueArray[23], "Value of AP#EXT should be 0 " );
                Assert.AreEqual( "0", ValueArray[24], "Value of APDTOT should be 0 " );
                Assert.AreEqual( "0", ValueArray[25], "Value of APLML should be 0 " );
                Assert.AreEqual( "0", ValueArray[26], "Value of APLMD should be 0 " );
                Assert.AreEqual( "0", ValueArray[27], "Value of APLUL# should be 0 " );
                Assert.AreEqual( "'A'", ValueArray[28], "Value of APACFL should be 'A' " );
                //Assert.AreEqual( "60944", ValueArray[29],"Value of APTTME should be 60944 " );
                Assert.AreEqual( "'$#L@%'", ValueArray[30], "Value of APINLG should be '$#L@%' " );
                Assert.AreEqual( "''", ValueArray[31], "Value of APBYPS should be '' " );
                Assert.AreEqual( "0", ValueArray[32], "Value of APSWPY should be 0 " );
                Assert.AreEqual( "''", ValueArray[33], "Value of APPSRC should be '' " );
                Assert.AreEqual( "0", ValueArray[34], "Value of APADR# should be 0 " );
                Assert.AreEqual( "'321.1'", ValueArray[35], "Value of APRA01 should be '321.1' " );
                Assert.AreEqual( "''", ValueArray[36], "Value of APRA02 should be '' " );
                Assert.AreEqual( "''", ValueArray[37], "Value of APRA03 should be '' " );
                Assert.AreEqual( "''", ValueArray[38], "Value of APRA04 should be '' " );
                Assert.AreEqual( "''", ValueArray[39], "Value of APRA05 should be '' " );
                Assert.AreEqual( "''", ValueArray[40], "Value of APCC01 should be '' " );
                Assert.AreEqual( "''", ValueArray[41], "Value of APCC02 should be '' " );
                Assert.AreEqual( "''", ValueArray[42], "Value of APCC03 should be '' " );
                Assert.AreEqual( "''", ValueArray[43], "Value of APDIAG should be '' " );
                Assert.AreEqual( "''", ValueArray[44], "Value of APACC should be '' " );
                // Assert.AreEqual( "113006", ValueArray[45],"Value of APTDAT should be 113006 " );
                Assert.AreEqual( "0", ValueArray[46], "Value of APACTP should be 0 " );
                Assert.AreEqual( "''", ValueArray[47], "Value of APXMIT should be '' " );
                Assert.AreEqual( "0", ValueArray[48], "Value of APQNUM should be 0 " );
                Assert.AreEqual( "''", ValueArray[49], "Value of APUPRV should be '' " );
                Assert.AreEqual( "''", ValueArray[50], "Value of APUPRW should be '' " );
                Assert.AreEqual( "''", ValueArray[51], "Value of APZDTE should be '' " );
                Assert.AreEqual( "''", ValueArray[52], "Value of APZTME should be '' " );
                Assert.AreEqual( "123", ValueArray[53], "Value of APMDR# should be 123 " );
                Assert.AreEqual( "'1'", ValueArray[54], "Value of APTREG should be '1' " );
                Assert.AreEqual( "0", ValueArray[55], "Value of APRDR# should be 0 " );
                Assert.AreEqual( "'CP'", ValueArray[56], "Value of APCL01 should be 'CP' " );
                Assert.AreEqual( "''", ValueArray[57], "Value of APCL02 should be '' " );
                Assert.AreEqual( "''", ValueArray[58], "Value of APCL03 should be '' " );
                Assert.AreEqual( "''", ValueArray[59], "Value of APCL04 should be '' " );
                Assert.AreEqual( "''", ValueArray[60], "Value of APCL05 should be '' " );
                Assert.AreEqual( "''", ValueArray[61], "Value of APVALU should be '' " );
                Assert.AreEqual( "''", ValueArray[62], "Value of APFC should be '' " );
                Assert.AreEqual( "'BP'", ValueArray[63], "Value of APRLGN should be 'BP' " );
                Assert.AreEqual( "'Clinical comments for Out to In Transfer'", ValueArray[64].Trim(),
                                 "Value of APCOM1 should be 'Clinical comments for Out to In Transfer' " );
                Assert.AreEqual( "'                                                       '",
                                 ValueArray[65].Trim(), "Value of APCOM2 should be '                                                       ' " );
                Assert.AreEqual( "'UP234'", ValueArray[66], "Value of APMNU# should be 'UP234' " );
                Assert.AreEqual( "'BobDoctor'", ValueArray[67], "Value of APMNDN should be 'BobDoctor' " );
                Assert.AreEqual( "''", ValueArray[68], "Value of APMNFR should be '' " );
                Assert.AreEqual( "0", ValueArray[69], "Value of APRNU# should be 0 " );
                Assert.AreEqual( "0", ValueArray[70], "Value of APRNDN should be 0 " );
                Assert.AreEqual( "''", ValueArray[71], "Value of APRNFR should be '' " );
                Assert.AreEqual( "'70'", ValueArray[72], "Value of APSPNC should be '70' " );
                Assert.AreEqual( "10110", ValueArray[73], "Value of APSPFR should be 10110 " );
                Assert.AreEqual( "20210", ValueArray[74], "Value of APSPTO should be 20210 " );
                Assert.AreEqual( "'Span Facility'", ValueArray[75], "Value of APINSN should be 'Span Facility' " );
                Assert.AreEqual( "'71'", ValueArray[76], "Value of APSPN2 should be '71' " );
                Assert.AreEqual( "30310", ValueArray[77], "Value of APAPFR should be 30310 " );
                Assert.AreEqual( "40410", ValueArray[78], "Value of APAPTO should be 40410 " );
                Assert.AreEqual( "'A'", ValueArray[79], "Value of APCNFG should be 'A' " );
                Assert.AreEqual( "0", ValueArray[80], "Value of APMNCD should be 0 " );
                Assert.AreEqual( "0", ValueArray[81], "Value of APMNP# should be 0 " );
                Assert.AreEqual( "'CCS'", ValueArray[82], "Value of APPARC should be 'CCS' " );
                Assert.AreEqual( "''", ValueArray[83], "Value of APOPD# should be '' " );
                Assert.AreEqual( "''", ValueArray[84], "Value of APTDR# should be '' " );
                Assert.AreEqual( "'Bob'", ValueArray[85], "Value of APMNLN should be 'Bob' " );
                Assert.AreEqual( "'Doctor'", ValueArray[86], "Value of APMNFN should be 'Doctor' " );
                Assert.AreEqual( "''", ValueArray[87], "Value of APMNMN should be '' " );
                Assert.AreEqual( "'NI234'", ValueArray[88], "Value of APMNPR should be 'NI234' " );
                Assert.AreEqual( "''", ValueArray[89], "Value of APMTXC should be '' " );
                Assert.AreEqual( "''", ValueArray[90], "Value of APRNLN should be '' " );
                Assert.AreEqual( "''", ValueArray[91], "Value of APRNFN should be '' " );
                Assert.AreEqual( "''", ValueArray[92], "Value of APRNMN should be '' " );
                Assert.AreEqual( "''", ValueArray[93], "Value of APRNPR should be '' " );
                Assert.AreEqual( "''", ValueArray[94], "Value of APRTXC should be '' " );
                Assert.AreEqual( "''", ValueArray[95], "Value of APRNP# should be '' " );
                Assert.AreEqual( "''", ValueArray[96], "Value of APANLN should be '' " );
                Assert.AreEqual( "''", ValueArray[97], "Value of APANFN should be '' " );
                Assert.AreEqual( "''", ValueArray[98], "Value of APANMN should be '' " );
                Assert.AreEqual( "''", ValueArray[99], "Value of APANU# should be '' " );
                Assert.AreEqual( "''", ValueArray[100], "Value of APANPR should be '' " );
                Assert.AreEqual( "''", ValueArray[101], "Value of APATXC should be '' " );
                Assert.AreEqual( "''", ValueArray[102], "Value of APANP# should be '' " );
                Assert.AreEqual( "''", ValueArray[103], "Value of APONLN should be '' " );
                Assert.AreEqual( "''", ValueArray[104], "Value of APONFN should be '' " );
                Assert.AreEqual( "''", ValueArray[105], "Value of APONMN should be '' " );
                Assert.AreEqual( "''", ValueArray[106], "Value of APONU# should be '' " );
                Assert.AreEqual( "''", ValueArray[107], "Value of APONPR should be '' " );
                Assert.AreEqual( "''", ValueArray[108], "Value of APOTXC should be '' " );
                Assert.AreEqual( "''", ValueArray[109], "Value of APONP# should be '' " );
                Assert.AreEqual( "''", ValueArray[110], "Value of APTNLN should be '' " );
                Assert.AreEqual( "''", ValueArray[111], "Value of APTNFN should be '' " );
                Assert.AreEqual( "''", ValueArray[112], "Value of APTNMN should be '' " );
                Assert.AreEqual( "''", ValueArray[113], "Value of APTNU# should be '' " );
                Assert.AreEqual( "''", ValueArray[114], "Value of APTNPR should be '' " );
                Assert.AreEqual( "''", ValueArray[115], "Value of APTTXC should be '' " );
                Assert.AreEqual( "''", ValueArray[116], "Value of APTNP# should be '' " );
                Assert.AreEqual( "''", ValueArray[117], "Value of APMSL# should be '' " );
                Assert.AreEqual( "''", ValueArray[118], "Value of APRSL# should be '' " );
                Assert.AreEqual( "''", ValueArray[119], "Value of APASL# should be '' " );
                Assert.AreEqual( "''", ValueArray[120], "Value of APOSL# should be '' " );
                Assert.AreEqual( "''", ValueArray[121], "Value of APTSL# should be '' " );
                Assert.AreEqual( "'PACCESS'", ValueArray[122], "Value of APWSIR should be 'PACCESS' " );
                Assert.AreEqual( "''", ValueArray[123], "Value of APSECR should be '' " );
                Assert.AreEqual( "''", ValueArray[124], "Value of APORR1 should be '' " );
                Assert.AreEqual( "''", ValueArray[125], "Value of APORR2 should be '' " );
                Assert.AreEqual( "''", ValueArray[126], "Value of APORR3 should be '' " );
                Assert.AreEqual( "'02'", ValueArray[127], "Value of APCI01 should be 02' " );
                Assert.AreEqual( "'" + ConditionCode.CONDITIONCODE_PRIVATE_ROOM_MEDICALLY_REQUIRED + "'", ValueArray[128], "Value of APCI02 should be '" + ConditionCode.CONDITIONCODE_PRIVATE_ROOM_MEDICALLY_REQUIRED + "' " );
                Assert.AreEqual( "'" + ConditionCode.CONDITIONCODE_DOB_OVER_100Y + "'", ValueArray[129], "Value of APCI02 should be '" + ConditionCode.CONDITIONCODE_DOB_OVER_100Y + "' " );
                Assert.AreEqual( "''", ValueArray[130], "Value of APCI04 should be '' " );
                Assert.AreEqual( "''", ValueArray[131], "Value of APCI05 should be '' " );
                Assert.AreEqual( "''", ValueArray[132], "Value of APCI06 should be '' " );
                Assert.AreEqual( "''", ValueArray[133], "Value of APCI07 should be '' " );
            }
        }

        [Test]
        [Category( "Fast" )]
        public void TestInitializeColumnValues()
        {
            try
            {
                new TransferInPatientToOutPatientInsertStrategy();
                Assert.IsTrue( true, "Initialization of orderedlist with default values succeeded" );
            }
            catch ( Exception )
            {
                Assert.Fail( "Initialization of orderedlist with default values failed." );
            }
        }

        [Test]
        public void TestUpdateColumnValuesUsing()
        {
            try
            {
                IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
                Facility facility_ACO = facilityBroker.FacilityWith( FACILITY_ID );

                Gender patientSex = new Gender( 2, DateTime.Now, "Male", "M" );
                DateTime patientDOB = new DateTime( 1965, 01, 13 );
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
                AccountProxy proxy = new AccountProxy( 30015,
                                       patient,
                                       DateTime.Now,
                                       DateTime.Now,
                                       new VisitType( 0, PersistentModel.NEW_VERSION, VisitType.OUTPATIENT_DESC, VisitType.OUTPATIENT ),
                                       facility_ACO,
                                       new FinancialClass( 299, PersistentModel.NEW_VERSION, "MEDICARE", "40" ),
                                       new HospitalService( 0, PersistentModel.NEW_VERSION, "DIAGNOSTIC OUTOPT", "60" ),
                                       "OL HSV60",
                                       false );

                IAccountBroker accountBroker = BrokerFactory.BrokerOfType<IAccountBroker>();
                Account account = accountBroker.AccountFor( proxy );

                account.Activity = new TransferInToOutActivity();
                TransferInPatientToOutPatientInsertStrategy transferInOutPatientInsertStrategy
                    = new TransferInPatientToOutPatientInsertStrategy();
                transferInOutPatientInsertStrategy.UpdateColumnValuesUsing( account );

                Assert.IsTrue( true, "Update of orderedlist with values succeeded" );

                ArrayList sqlStrings =
                    transferInOutPatientInsertStrategy.BuildSqlFrom( account, transactionKeys );

                foreach ( string sqlString in sqlStrings )
                {
                    int startPositionOfValues =
                        sqlString.IndexOf( "(", sqlString.IndexOf( ")" ) ) + 1;
                    int lengthOfValues =
                        sqlString.Length - sqlString.IndexOf( "(", sqlString.IndexOf( ")" ) ) - 2;
                    string[] ValueArray =
                        sqlString.Substring( startPositionOfValues, lengthOfValues ).Split( ',' );

                    Assert.AreEqual( NUMBER_OF_ENTRIES, ValueArray.Length, "Wrong Number of entries in ValueArray" );
                }
            }
            catch ( Exception )
            {
                Assert.Fail( "Update of orderedlist with values failed." );
            }
        }
        #endregion

        #region Data Elements

        private readonly TransactionKeys transactionKeys = new TransactionKeys();

        #endregion

        #region Constants
        private const int
            NUMBER_OF_ENTRIES = 135;

        private const int FACILITY_ID = 900;

        #endregion
    }
}
