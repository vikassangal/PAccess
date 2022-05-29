using System;
using Extensions.PersistenceCommon;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Persistence.AccountCopy;
using PatientAccess.BrokerInterfaces;

namespace Tests.Integration.PatientAccess.Persistence.AccountCopy
{
    [TestFixture]
    public class AccountCopyStrategyTestsForNewBorn
    {
        #region Constants

        private readonly Facility facility = new Facility(54, PersistentModel.NEW_VERSION, "DHF", "DHF");

        private readonly Address someAddress = new Address("123 street",
                                                           "somestrert",
                                                           "asdfffCity",
                                                           new ZipCode("12332"),
                                                           new State(0L,
                                                                     PersistentModel.NEW_VERSION,
                                                                     "TEXAS",
                                                                     "TX"),
                                                           new Country(0L,
                                                                       PersistentModel.NEW_VERSION,
                                                                       "United States",
                                                                       "USA"),
                                                           new County(0L,
                                                                      PersistentModel.NEW_VERSION,
                                                                      "ORANGE",
                                                                      "122")
            );
        private readonly Address Address1MoreThan25Chars = new Address("123 street near collin creek mall and plano parkway",
            "somestrert",
            "asdfffCity",
            new ZipCode("12332"),
            new State(0L,
                PersistentModel.NEW_VERSION,
                "TEXAS",
                "TX"),
            new Country(0L,
                PersistentModel.NEW_VERSION,
                "United States",
                "USA"),
            new County(0L,
                PersistentModel.NEW_VERSION,
                "ORANGE",
                "122")
        );
        private readonly Address SouthCarolinaAddress = new Address("123 street",
                                                                  "somestrert",
                                                                  "asdfffCity",
                                                                  new ZipCode("12332"),
                                                                  new State(0L,
                                                                            PersistentModel.NEW_VERSION,
                                                                            "SouthCarolina",
                                                                            "SC"),
                                                                  new Country(0L,
                                                                              PersistentModel.NEW_VERSION,
                                                                              "United States",
                                                                              "USA"),
                                                                  new County(0L,
                                                                             PersistentModel.NEW_VERSION,
                                                                             "ORANGE",
                                                                             "122")
                   );
        #endregion

        #region Test Methods

        [Test]
        public void TesEditGeneralInformationUsing_IsNewBornFlagShouldBeTrue()
        {
            var admitDate = DateTime.Parse("1/2/2012");
            var preopDate = admitDate;
            var oldAccount = GetAccountWithAdmitDateForMother(admitDate, preopDate);
            oldAccount.KindOfVisit = VisitType.Inpatient;
            oldAccount.Activity = new PreAdmitNewbornActivity();
            var copyStrategy = new NewbornAccountCopyStrategy();
            var newAccount = copyStrategy.CopyAccount(oldAccount);
            Assert.IsTrue(newAccount.IsNewBorn, "IsNewBorn flag should be true");
        }

        [Test]
        public void TestEditDemographicsUsing_WhenOldAccountHasOtherLangauge_NewAccountShouldHaveOtherLanguage()
        {
            var admitDate = DateTime.Parse("1/2/2012");
            var preopDate = admitDate;
            var oldAccount = GetAccountWithAdmitDateForMother(admitDate, preopDate);
            oldAccount.KindOfVisit = VisitType.Inpatient;
            oldAccount.Activity = new PreAdmitNewbornActivity();
            var copyStrategy = new NewbornAccountCopyStrategy();
            var newAccount = copyStrategy.CopyAccount(oldAccount);

            Assert.IsNotNull(newAccount.Patient);
            Assert.IsNotNull(newAccount.Patient.Language);
            Assert.IsTrue(newAccount.Patient.OtherLanguage == oldAccount.Patient.OtherLanguage);
        }
        [Test]
        public void TestEditDemographicsUsing_NewBornAccountShouldHaveSSNStatus_NewBorn()
        {
            var admitDate = DateTime.Parse("1/2/2012");
            var preopDate = admitDate;
            var oldAccount = GetAccountWithAdmitDateForMother(admitDate, preopDate);
            oldAccount.Facility.FacilityState = SouthCarolinaAddress.State;
            oldAccount.KindOfVisit = VisitType.Inpatient;
            oldAccount.Activity = new PreAdmitNewbornActivity();
            var copyStrategy = new NewbornAccountCopyStrategy();
            var newAccount = copyStrategy.CopyAccount(oldAccount);

            Assert.IsNotNull(newAccount.Patient);
            Assert.IsNotNull(newAccount.Patient.SocialSecurityNumber);
            Assert.IsTrue( newAccount.Patient.SocialSecurityNumber.SSNStatus.IsNewbornSSNStatus );
        }
        [Test]
        public void TestEditDemographicsUsing_CopyForwardAdmitDate_ForPreAdmitNewBornActivity()
        {
            var admitDate = DateTime.Parse("1/2/2012");
            var preopDate = admitDate;
            var oldAccount = GetAccountWithAdmitDateForMother(admitDate, preopDate);
            oldAccount.KindOfVisit = VisitType.Inpatient;
            oldAccount.Activity = new PreAdmitNewbornActivity();
            var copyStrategy = new NewbornAccountCopyStrategy();
            var newAccount = copyStrategy.CopyAccount(oldAccount);

            Assert.AreEqual( oldAccount.AdmitDate.Date, newAccount.AdmitDate.Date, "Admit date did not copy from old account to new account" );
        }

        [Test]
        public void TestEditDemographicsUsing_DontCopyForwardAdmitDate_ForAdmitNewBornActivity()
        {
            var expectedAdmitDate = DateTime.Parse("1/2/2012");
            var expecdtedPreopDate = expectedAdmitDate;
            var oldAccount = GetAccountWithAdmitDateForMother(expectedAdmitDate, expecdtedPreopDate);
            oldAccount.KindOfVisit = VisitType.Inpatient;
            oldAccount.Activity = new AdmitNewbornActivity();
            var copyStrategy = new NewbornAccountCopyStrategy();
            var currentFacilityDate = facility.GetCurrentDateTime().Date;
            var newAccount = copyStrategy.CopyAccount(oldAccount);

            Assert.AreEqual(expectedAdmitDate.Date, oldAccount.AdmitDate.Date, "Old Admit date does not match");
            Assert.AreEqual(currentFacilityDate, newAccount.AdmitDate.Date, "New Admit date does not match");
            Assert.AreEqual( expecdtedPreopDate.Date, oldAccount.PreopDate.Date, "Old PreOp date is set when it should not be" );
            Assert.AreEqual( DateTime.MinValue.Date, newAccount.PreopDate.Date, "New PreOp date is set when it should not be" );
        }

        [Test]
        public void ForRegisterNewBornActivity_MothersContactInformationShouldCopyForwardAsNewBornsFirstEmergencyContact()
        {
            const string firstName = "Mother123";
            const string middleInitial = "N";
            const string lastName = "TEST123";
            var mothersAddress = someAddress;
            var mothersPhoneNumber = new PhoneNumber("1234567890");
            var motherAccount = GetAnAccountWith(mothersAddress, mothersPhoneNumber, firstName, middleInitial, lastName);
            motherAccount.Activity = new RegistrationActivity();
            var newBornAccount = new NewbornAccountCopyStrategy().CopyAccount(motherAccount);

            var newBornEmergencyContact1Information = newBornAccount.EmergencyContact1.ContactPointWith(TypeOfContactPoint.NewPhysicalContactPointType());
            var newBornEmergencyContact1Address = newBornEmergencyContact1Information.Address.Address1;
            var newBornEmergencyContact1PhoneNumber = newBornEmergencyContact1Information.PhoneNumber;
            var newBornEmergencyContact1Name = newBornAccount.EmergencyContact1.Name;
            var relationShipTypeBroker = BrokerFactory.BrokerOfType<IRelationshipTypeBroker>();

            RelationshipType relationshipType =  relationShipTypeBroker.RelationshipTypeWith(motherAccount.Facility.Oid,
                                                           RelationshipType.RELATIONSHIPTYPE_MOTHER);

            Assert.AreEqual(mothersAddress.Address1, newBornEmergencyContact1Address, "new born's emergency contact address does not match mothers address");
            Assert.AreEqual(mothersPhoneNumber, newBornEmergencyContact1PhoneNumber, "new born's emergency contact phone number does not match mothers phone number");
            Assert.AreEqual( motherAccount.Patient.Name.AsFormattedName(), newBornEmergencyContact1Name, "new born's emergency contact name does not match mothers name");
            Assert.AreEqual(newBornAccount.EmergencyContact1.RelationshipType,relationshipType, "The relationship type of mother is not assigned to the newborn emergency contact relationship");
        }

        [Test]
        public void
            ForRegisterNewBornActivity_MothersAddressShouldCopyForwardOnly25CharsAsNewBornsFirstEmergencyContact()
        {
            const string firstName = "Mother123";
            const string middleInitial = "N";
            const string lastName = "TEST123";
            var mothersAddress = Address1MoreThan25Chars;
            var mothersPhoneNumber = new PhoneNumber("1234567890");
            var motherAccount =
                GetAnAccountWith(mothersAddress, mothersPhoneNumber, firstName, middleInitial, lastName);
            motherAccount.Activity = new RegistrationActivity();
            var newBornAccount = new NewbornAccountCopyStrategy().CopyAccount(motherAccount);

            var newBornEmergencyContact1Information =
                newBornAccount.EmergencyContact1.ContactPointWith(TypeOfContactPoint.NewPhysicalContactPointType());
            var newBornEmergencyContact1Address = newBornEmergencyContact1Information.Address.Address1;
            var newBornEmergencyContact1Address2 = newBornEmergencyContact1Information.Address.Address2;
            var relationShipTypeBroker = BrokerFactory.BrokerOfType<IRelationshipTypeBroker>();
             
            Assert.AreEqual(mothersAddress.Address1.Substring(0, 25), newBornEmergencyContact1Address,
                "new born's emergency contact address does not match mothers address");
            Assert.AreEqual(newBornEmergencyContact1Address2, String.Empty,
                "new born's emergency contact address2 is not empty");
        }

        [Test]
        public void TestAccountCopy_WhenAccountHasShareDataWithPublicHIEFlagSet_ActivityISRegisterNewBorn_ShouldNotCopyforwardShareDataWithPublicHIE()
        {
            var oldAccount = GetMothersAccountWithHIEFlags(YesNoFlag.No, YesNoFlag.No);
            oldAccount.ShareDataWithPublicHieFlag = YesNoFlag.No;
            oldAccount.KindOfVisit = VisitType.Inpatient;
            oldAccount.Activity = new RegistrationActivity();
            var copyStrategy = new NewbornAccountCopyStrategy();
            var newAccount = copyStrategy.CopyAccount(oldAccount);
            Assert.AreNotEqual(oldAccount.ShareDataWithPublicHieFlag.Code, newAccount.ShareDataWithPublicHieFlag.Code, "Share Data with Public HIE did copy from old account to new account");


        }

        [Test]
        public void TestAccountCopy_WhenAccountHasHIEPhysicianFlagSet_ActivityISRegisterNewBorn_ShouldNotCopyForwardHIEPhysicianConsent()
        {
            var oldAccount = GetMothersAccountWithHIEFlags(YesNoFlag.No, YesNoFlag.Yes);
            oldAccount.ShareDataWithPCPFlag = YesNoFlag.Yes;
            oldAccount.KindOfVisit = VisitType.Inpatient;
            oldAccount.Activity = new RegistrationActivity();
            var copyStrategy = new NewbornAccountCopyStrategy();
            var newAccount = copyStrategy.CopyAccount(oldAccount);
            Assert.AreNotEqual(oldAccount.ShareDataWithPCPFlag.Code, newAccount.ShareDataWithPCPFlag.Code, "HIE Physician consent did copy from old account to new account");

        }

        [Test]
        public void TestAccountCopy_WhenAccountHasShareDataWithPublicHIEFlagSet_ActivityISPreAdmitNewBorn_ShouldNotCopyforwardShareDataWithPublicHIE()
        {
            var oldAccount = GetMothersAccountWithHIEFlags(YesNoFlag.Yes, YesNoFlag.No);
            oldAccount.KindOfVisit = VisitType.Inpatient;
            oldAccount.Activity = new PreAdmitNewbornActivity();
            var copyStrategy = new NewbornAccountCopyStrategy();
            var newAccount = copyStrategy.CopyAccount(oldAccount);
            Assert.AreNotEqual(oldAccount.ShareDataWithPublicHieFlag.Code, newAccount.ShareDataWithPublicHieFlag.Code, "Share Data With Public HIE did copy from old account to new account");

        }

        [Test]
        public void TestAccountCopy_WhenAccountHasPhysicianFlagSet_ActivityISPreAdmitNewBorn_ShouldCopyForwardHIEPhysicianConsent()
        {
            var oldAccount = GetMothersAccountWithHIEFlags(YesNoFlag.No, YesNoFlag.No);
            oldAccount.ShareDataWithPCPFlag = YesNoFlag.No;
            oldAccount.KindOfVisit = VisitType.Inpatient;
            oldAccount.Activity = new PreAdmitNewbornActivity();
            var copyStrategy = new NewbornAccountCopyStrategy();
            var newAccount = copyStrategy.CopyAccount(oldAccount);
            Assert.AreEqual(oldAccount.ShareDataWithPCPFlag.Code, newAccount.ShareDataWithPCPFlag.Code, "HIE Physician consent did not from old account to new account");

        }

        #endregion

        #region Support Methods

        private Account GetAnAccountWith(Address address, PhoneNumber phoneNumber, string firstName, string middleInitial, string lastName)
        {
            var contactInformation = new ContactPoint
                {
                    Address = address,
                    PhoneNumber = phoneNumber,
                    TypeOfContactPoint = TypeOfContactPoint.NewMailingContactPointType()
                };

            var patient = new Patient();

            patient.AddContactPoint(contactInformation);
            patient.FirstName = firstName;
            patient.MiddleInitial = middleInitial;
            patient.LastName = lastName;

            var account = new Account
                {
                    Patient = patient,
                    AdmitDate = DateTime.Now,
                    Facility = facility,
                    KindOfVisit = VisitType.Inpatient
                };

            return account;
        }

        private Account GetAccountWithAdmitDateForMother(DateTime admitDate, DateTime preopDate)
        {
            var patient = new Patient();

            var anAccount = new Account
                {
                    Patient = patient,
                    AdmitDate = admitDate,
                    PreopDate = preopDate,
                    Facility = facility
                };

            return anAccount;
        }

        private Account GetMothersAccountWithHIEFlags(YesNoFlag consent, YesNoFlag PhysicianFlag)
        {
            var flag = new YesNoFlag();
            flag.SetYes();
            var anAccount = new Account
            {
                Patient = new Patient(),
                ShareDataWithPublicHieFlag = consent,
                ShareDataWithPCPFlag = PhysicianFlag,
                Facility = facility
            };
            return anAccount;
        }

        #endregion
    }
}