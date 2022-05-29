using System;
using System.Collections;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    [TestFixture()]
    public class InsurancePBARBrokerTests : AbstractBrokerTests
    {
        #region SetUp and TearDown AccountBrokerTests
        [TestFixtureSetUp()]
        public static void SetUpInsurancePBARBroker()
        {
            IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker >();
                   
            insuranceBroker = BrokerFactory.BrokerOfType<IInsuranceBroker>();
            
            facilityDEL = facilityBroker.FacilityWith( "DEL" );
            facilityACO = facilityBroker.FacilityWith( "ACO" );
            facilityICE = facilityBroker.FacilityWith( "ICE" );
            facilityDHF = facilityBroker.FacilityWith("DHF");
            
            iIpa = new MedicalGroupIPA();
            iIpa.Code = "BOLSA";
            iIpa.Name = "BOLSA MED GP";

            iClinic = new Clinic();
            iClinic.Code = "11";
            iClinic.Name = "BOLSA";

            iAccount = new Account(ACCOUNT_NUM);
            iAccount.AdmitDate = new DateTime(2008, 1, 22);
            iAccount.Facility = facilityACO;
        }

        [TestFixtureTearDown()]
        public static void TearDownInsurancePBARBroker()
        {
            insuranceBroker = null;

            iAccount = null;
            
            facilityDEL = null;
            facilityACO = null;
            facilityICE = null;

            iIpa = null;

            iClinic = null;
        }
        #endregion

        #region Test Methods

        [Test()]
        public void TestInsPlanCategories()
        {
            ICollection categories = insuranceBroker.AllTypesOfCategories(facilityACO.Oid);
            Assert.IsNotNull(categories, "Did not find any Insurance Plan Categories");
            Assert.AreEqual(7, categories.Count, "Did not find the correct number of Insurance Plan Categories");


            InsurancePlanCategory ipc2 = insuranceBroker.InsurancePlanCategoryWith(6, facilityACO.Oid);
            //            Assert.AreEqual( 6,ipc2.Oid,"Ins Plan ID is not 6" );
            Assert.AreEqual("Workers compensation", ipc2.Description, "Ins Plan Description is not 'Workers Compensation'");
            Assert.AreEqual("Policy Number", ipc2.AssociatedIDLabel, "Category associatedID not as expected");
        }

        [Test()]
        public void TestGetPlanTypes()
        {
            ICollection planTypes = insuranceBroker.AllPlanTypes(facilityACO.Oid);
            Assert.IsNotNull(planTypes, "Can not load Insurance Plan Types");
            Assert.IsTrue(planTypes.Count != 0, "Did not find any Insurance plan types");

            InsurancePlanType ptype = insuranceBroker.InsurancePlanTypeWith("PROV", facilityACO.Oid);
            Assert.IsNotNull(ptype, "Did not find Insurance Plan Type with code of PROV");

            // method not implemented in PBAR version
            //InsurancePlanType pType2 = insuranceBroker.InsurancePlanTypeWith(14);
            //Assert.AreEqual("COMFR", pType2.Code, "Plan Type does not have expected Code: COMFR");
        }

        [Test()]
        public void TestGetPlan()
        {
            InsurancePlan planCommercial = insuranceBroker.PlanWith("1B132", iAccount.Facility.Oid, iAccount.AdmitDate);
            Assert.IsNotNull(planCommercial, "Can not find Plan with plan id of: 1B132");
            Assert.AreEqual("BLUE CROSS HMO", planCommercial.PlanName, "Plan does not have correct Plan Name");

            BillingInformation bi = null;
            foreach ( BillingInformation bi1 in planCommercial.BillingInformations)
            {
                bi = bi1;
                break;
            }
            Assert.AreEqual(
                string.Empty,
                bi.BillingCOName,
                "billing info's  C/O Name should be ''"
                );
            Assert.AreEqual(
                string.Empty,
                bi.BillingName,
                "billing info's  Name should be ''"
                );

            Assert.AreEqual(
                string.Empty,
                bi.Area,
                "billing info's  area should be ''"
                );

            Assert.AreEqual(
                "PO BOX 123",
                bi.Address.Address1,
                "billing info's  addr1 should be 'PO BOX 123'"
                );

            Assert.AreEqual(
                "WOODLAND HILLS",
                bi.Address.City,
                "billing info's  city should be 'WOODLAND HILLS'"
                );

            Assert.AreEqual(
                "CA",
                bi.Address.State.Code,
                "billing info's  state code should be 'CA'"
                );

            Assert.AreEqual(
                "CALIFORNIA",
                bi.Address.State.Description,
                "billing info's  state should be 'CALIFORNIA'"
                );

            Assert.AreEqual(
                "USA",
                bi.Address.Country.Code,
                "billing info's  country code should be 'USA'"
                );

            Assert.AreEqual(
                "91364",
                bi.Address.ZipCode.PostalCode,
                "billing info's  postal code should be '91364'"
                );

            Assert.AreEqual(
                "(800) 141-4141",
                bi.PhoneNumber.AsFormattedString(),
                "billing info's  phone should be '(800) 141-4141'"
                );

            Assert.AreEqual(
                string.Empty,
                bi.EmailAddress.ToString(),
                "billing info's email should be ''"
                );

            InsurancePlan planGovernmentOther = insuranceBroker.PlanWith("FR912", iAccount.Facility.Oid, iAccount.AdmitDate);
            Assert.IsNotNull(planGovernmentOther, "Can not find Plan with plan id of: FR912");
            Assert.AreEqual("TRICARE (PRIME)HMO", planGovernmentOther.PlanName, "Plan does not have correct Plan Name");

            InsurancePlan planMedicare = insuranceBroker.PlanWith("53544", iAccount.Facility.Oid, iAccount.AdmitDate);
            Assert.IsNotNull(planMedicare, "Can not find Plan with plan id of: 53544");
            Assert.AreEqual("MEDICARE", planMedicare.PlanName, "Plan does not have correct Plan Name");

            InsurancePlan planMedicaid = insuranceBroker.PlanWith("5315Q", iAccount.Facility.Oid, iAccount.AdmitDate);
            Assert.IsNotNull(planMedicaid, "Can not find Plan with plan id of: 1B132");
            Assert.AreEqual("MEDICAID-FL MEDIPASS", planMedicaid.PlanName, "Plan does not have correct Plan Name");

            InsurancePlan planSelfPay = insuranceBroker.PlanWith("6628F", iAccount.Facility.Oid, iAccount.AdmitDate);
            Assert.IsNotNull(planSelfPay, "Can not find Plan with plan id of: 6628F");
            Assert.AreEqual("PRIVATE PAY/UNINSURED NON/COMPACT", planSelfPay.PlanName, "Plan does not have correct Plan Name");

//TODO-AC investigate and remove if appropriate
//            InsurancePlan planWorkersCompensation = insuranceBroker.PlanWith("37435", iAccount.Facility.Oid, iAccount.AdmitDate);
//            Assert.IsNotNull(planWorkersCompensation, "Can not find Plan with plan id of: 37435");
//            Assert.AreEqual("STATE COMP MANAGED W/COMP", planWorkersCompensation.PlanName, "Plan does not have correct Plan Name");

            //InsurancePlan planOther = insuranceBroker.PlanWith("1B132", facilityICE.Oid, new DateTime(2008, 2, 11));
            //Assert.IsNotNull(planOther, "Can not find Plan with plan id of: 1B132");
            //Assert.AreEqual("BLUE CROSS HMO", planOther.PlanName, "Plan does not have correct Plan Name");

            InsurancePlan noPlan = insuranceBroker.PlanWith("eee", iAccount.Facility.Oid, iAccount.AdmitDate);
            Assert.IsNull(noPlan, "Should not have found plan with id if 'eee'");
        }

        [Test()]
        [Ignore]
        public void TestGetPlanForBroker()
        {
            Broker b = new Broker();
            b.Code = "ADVH";
            ICollection plans = insuranceBroker.InsurancePlansFor(b, iAccount.Facility.Oid, iAccount.AdmitDate, null);
            Assert.IsNotNull(plans, "Not plans list returned from InsurancePlansFor");
            Assert.IsTrue(plans.Count > 0, "No Plans returned from broker");

            bool found = false;
            foreach (InsurancePlan plan in plans)
            {
                if (plan.PlanID.Equals(PLAN_CODE1))
                {
                    found = true;
                    Assert.AreEqual("ACCOUNTABLE HMO", plan.PlanName, "Name is not as expected");
                    // Assert.AreEqual(42539, plan.Oid, "PlanOid is not as expected, should be 42539");
                    Assert.AreEqual("32", plan.PlanSuffix, "Plan Suffix is not as expected");
                    Assert.AreEqual("CONTRACT", plan.LineOfBusiness, "Plan LOB is not correct");
                }
            }
            if (found == false)
            {
                Assert.Fail("Did not find expected Insurance Plan with PlanId = AA132");
            }
        }

        [Test()]
        public void TestGetPlanForPayor()
        {
            Payor payor = new Payor();
            payor.Code = "WZ9";
            ICollection plans = insuranceBroker.InsurancePlansFor(payor, iAccount.Facility.Oid, iAccount.AdmitDate, null);
            Assert.IsNotNull(plans, "No plans array returned from InsurancePlansFor");
            Assert.IsTrue(plans.Count > 0, "Wrong number of plans found for Payor");

            bool found = false;
            foreach (InsurancePlan plan in plans)
            {
                if (plan.PlanID.Equals("WZ933"))
                {
                    found = true;
                    Assert.AreEqual("AETNA TEST PLAN", plan.PlanName, "Name should be 'AETNA TEST PLAN'");
                    // Assert.AreEqual(42571, plan.Oid, "PlanOid is not as expected, should be 42571");
                    Assert.AreEqual("33", plan.PlanSuffix, "Plan Suffix is not as expected");
                    Assert.AreEqual("CONTRACT", plan.LineOfBusiness, "Plan LOB should be 'CONTRACT'");
                }
            }
            if (found == false)
            {
                Assert.Fail("Did not find expected Insurance Plan with PlanId = WZ933");
            }
        }

        [Test()]
        public void TestGetPlanForCoveredGroup()
        {
            ICollection plans = null;

            CoveredGroup aCoveredGroup = new CoveredGroup();
            EmployerProxy anEmployer = new EmployerProxy();
            aCoveredGroup.Employer = anEmployer;

            InsurancePlanCategory planCategory = insuranceBroker.InsurancePlanCategoryWith(2, facilityACO.Oid);

            anEmployer.EmployerCode = 4676;
            anEmployer.Name = "SEARS";
            plans = insuranceBroker.InsurancePlansFor(aCoveredGroup, facilityACO.Oid, new DateTime(2008, 1, 1), planCategory);
            Assert.IsTrue(plans.Count > 0, "Did not find any plans for employer 'SEARS'");

            aCoveredGroup.Employer = null;
            plans = insuranceBroker.InsurancePlansFor(aCoveredGroup, facilityACO.Oid, new DateTime(2008, 1, 1), planCategory);
            Assert.IsTrue(plans.Count == 0, "There is no plan with EmployerCode=0 and Name=''");
        }

        [Test()]
        public void TestSpecificIPA()
        {
            MedicalGroupIPA ipa = insuranceBroker.IPAWith(iAccount.Facility.Oid, "KAISE", "KA");
            Assert.IsNotNull(ipa, "Did not find expected IPA");
            Assert.AreEqual("KAISER", ipa.Name, "IPA Name not as expected");
            Assert.AreEqual("KAISE", ipa.Code, "IPA Code not as expected");
            ICollection clinics = ipa.Clinics;
            Assert.IsNotNull(clinics, "Did not find clinic associated with IPA");
            Assert.AreEqual(1, clinics.Count, "Did not find proper number of clinics");
            Clinic clinic = null;
            foreach (Clinic clinic1 in clinics)
            {
                clinic = clinic1;
                break;
            }
            Assert.AreEqual("KAISER", clinic.Name, "Clinic Name not as expected");

            ipa = insuranceBroker.IPAWith(iAccount.Facility.Oid, "XCXCX", "XC");
            Assert.IsNull(ipa, "Found IPA when expecting none for Code: XCXCX");

        }

        [Test()]
        public void TestIPAsByName()
        {
            ICollection IPAs = insuranceBroker.IPAsFor(iAccount.Facility.Oid, "KAISER");
            Assert.IsNotNull(IPAs, "Did not find any exected IPAs with the name 'KAISER'");
            bool found = false;

            foreach (MedicalGroupIPA ipa in IPAs)
            {
                if (ipa.Code.Equals("KAISE"))
                {
                    found = true;
                    ICollection clinics = ipa.Clinics;
                    Assert.IsNotNull(clinics.Count, "Did not find any clinics for test IPA");
                    Assert.AreEqual(1, clinics.Count, "Did not find proper number of clinics");
                    //                    Assert.AreEqual( 23,clinics.Count,"Did not find proper number of clinics" );
                }
            }
            Assert.AreEqual(true, found, "Did not find expected IPA");
        }

        [Test()]
        public void TestIPAsForBlank()
        {
            string blank = string.Empty;
            
            MedicalGroupIPA ipa1 = insuranceBroker.IPAWith(facilityACO.Oid, blank, blank);
            Clinic clinic1 = null;
            ICollection clinics1 = ipa1.Clinics;
            foreach (Clinic clinic in clinics1)
            {
                clinic1 = clinic;
                break;
            }
            Assert.AreEqual(blank, ipa1.Code, "IPA1 Code should be blank");
            Assert.AreEqual(blank, ipa1.Name, "IPA1 Name should be blank");
            Assert.AreEqual(blank, clinic1.Code, "Clinic1 Code should be blank");
            Assert.AreEqual(blank, clinic1.Name, "Clinic1 Name should be blank");

            MedicalGroupIPA ipa2 = insuranceBroker.IPAWith(facilityDEL.Oid, blank, blank);
            Clinic clinic2 = null;
            ICollection clinics2 = ipa2.Clinics;
            foreach (Clinic clinic in clinics2)
            {
                clinic2 = clinic;
                break;
            }
            Assert.AreEqual(blank, ipa2.Code, "IPA2 Code should be blank");
            Assert.AreEqual(blank, ipa2.Name, "IPA2 Name should be blank");
            Assert.AreEqual(blank, clinic2.Code, "Clinic2 Code should be blank");
            Assert.AreEqual(blank, clinic2.Name, "Clinic2 Name should be blank");

            MedicalGroupIPA ipa3 = insuranceBroker.IPAWith(facilityICE.Oid, blank, blank);
            Clinic clinic3 = null;
            ICollection clinics3 = ipa3.Clinics;
            foreach (Clinic clinic in clinics3)
            {
                clinic3 = clinic;
                break;
            }
            Assert.AreEqual(blank, ipa3.Code, "IPA3 Code should be blank");
            Assert.AreEqual(blank, ipa3.Name, "IPA3 Name should be blank");
            Assert.AreEqual(blank, clinic3.Code, "Clinic3 Code should be blank");
            Assert.AreEqual(blank, clinic3.Name, "Clinic3 Name should be blank");
        }

        [Test()]
        public void TestPlanWithDates()
        {
            InsurancePlan plan = insuranceBroker.PlanWith(
                "1B132",
                20060130,
                20030101,
                iAccount.Facility.Oid, iAccount.AccountNumber);
            Assert.IsNotNull(plan, "Can not find Plan with plan ID of: 1B132");
            Assert.AreEqual("BLUE CROSS HMO",
                            plan.PlanName, "Plan does not have correct Plan Name");

            InsurancePlan plan2 = insuranceBroker.PlanWith(
                "1B132",
                new DateTime(2003, 1, 1),
                new DateTime(2006, 1, 30),
                facilityACO.Oid);
            Assert.IsNotNull(plan2,"Can not find Plan with plan ID of: 1B132");
            Assert.AreEqual("BLUE CROSS HMO",
                            plan2.PlanName, "Plan does not have correct Plan Name"); 

        }

        [Test()]
        public void TestInsurancePlanTypesForBlank()
        {
            string blank = string.Empty;
            InsurancePlanType planType = insuranceBroker.InsurancePlanTypeWith(blank, facilityACO.Oid);

            Assert.AreEqual(blank, planType.Code, "Code should be blank");
            Assert.AreEqual(blank, planType.Description, "Description should be blank");
        }

        [Test()]
        public void TestPlanWithPlanID()
        {
            InsurancePlan plan = insuranceBroker.PlanWith(PLAN_CODE3, iAccount.Facility.Oid, iAccount.AdmitDate);

            Assert.IsTrue(plan.BillingInformations.Count > 0, "No BillingInfo retrieved");
        }

        [Test()]
        public void TestOtherInsurancePayor()
        {
            ICollection providers = insuranceBroker.ProvidersStartingWith("OTHER", iAccount.Facility.Oid, iAccount.AdmitDate, null);
            Assert.IsTrue(providers.Count > 0, "Other Providers not read");
            foreach (AbstractProvider provider in providers)
            {
                Assert.IsTrue(typeof(OtherPayor) == provider.GetType(), "Payor 'OTHER' is not of type otherPayor");
            }
        }

        [Test()]
        public void TestGetProviderListStartsWith()
        {
            bool found = false;

            ICollection providers = insuranceBroker.ProvidersStartingWith("CI", iAccount.Facility.Oid, iAccount.AdmitDate, null);

            Assert.IsNotNull( providers, "Did not find any providers matching 'CI'" );
            Assert.IsTrue( providers.Count != 0,"Did not find any providers matching 'CI'" );
            
            foreach( AbstractProvider provider in providers )
            {
                if(provider.Code.Equals("081"))
                {
                    Assert.AreEqual( "CIGNA",provider.Name, "Did not find expected name for Payor" );
                    Assert.AreEqual( "081",provider.Code,"Did not find expected payor code for payor" );
                    
                    ICollection plans = insuranceBroker.InsurancePlansFor(provider, iAccount.Facility.Oid, iAccount.AdmitDate, null);
                    Assert.IsNotNull( plans, "Did not find any plans for provider: " + provider.Name );
                    Assert.IsTrue( plans.Count > 0,"Did not find any plans for Provider" );
                    //Console.Out.WriteLine("Plan Count: " + plans.Count.ToString());
                    found = true;
                    break;
                }
            }
            if( !found ) 
            {
                Assert.Fail("Did not find Payor with OID of 236");
            }
        }

        [Test()]
        [Ignore]
        public void TestGetProviderListIncludes()
        {
            bool found = false;

            ICollection providers = insuranceBroker.ProvidersContaining("ACCOUNTABLE MAYBE", iAccount.Facility.Oid, iAccount.AdmitDate, null);

            Assert.IsNotNull(providers, "Did not find any providers matching 'ACCOUNTABLE MAYBE'");
            Assert.IsTrue(providers.Count != 0, "Did not find any providers matching 'ACCOUNTABLE MAYBE'");

            foreach (AbstractProvider provider in providers)
            {
                if (provider.Code.Equals("ADVH"))
                {
                    Assert.AreEqual("ACCOUNTABLE MAYBE", provider.Name, "Did not find expected name for broker");

                    ICollection plans = insuranceBroker.InsurancePlansFor(provider, iAccount.Facility.Oid, iAccount.AdmitDate, null);
                    Assert.IsNotNull(plans, "Did not find any plans for provider: " + provider.Name);

                    // TLG 09/13/2007 - Modified from 2 to 1 plan because of master plan exclusion

                    Assert.AreEqual(1, plans.Count, "Did not find 1 plan for Provider");

                    //Console.Out.WriteLine("Plan Count: " + plans.Count.ToString());
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                Assert.Fail("Did not find broker with code 'ADVH'");
            }
        }

        [Test()]
        //[Ignore("No valid plans after reload")]
        public void TestCoveredGroupsMatching()
        {
            ICollection groups = insuranceBroker.CoveredGroupsMatching("S", iAccount.Facility.Oid, iAccount.AdmitDate);
            Assert.IsNotNull(groups, "Can not load covered Groups");
            Assert.IsTrue(groups.Count > 0, "No plan loaded when searching for covered groups matching 'S'");

            bool found = false;
            foreach (CoveredGroup group in groups)
            {
                if (group.Name.Equals("SEARS"))
                {
                    found = true;
                    ICollection plans = group.InsurancePlans;
                    Assert.IsNotNull(plans, "No plans list loaded for covered Group");
                    Assert.IsTrue(plans.Count > 0, "No plans loaded for covered group");
                    EmployerProxy emp = (EmployerProxy)group.Employer;
                    Assert.IsNotNull(emp, "No employer found attached to CoveredGroup");
                    //Assert.AreEqual( 530,emp.EmployerCode,"Incorrect employerCode found for employer attached to covered Group" );
                }
            }
            if (found == false)
            {
                Assert.Fail("Did not find expected Covered Group with Name='SEARS' and OID = 451");
            }

            ICollection noGroups = insuranceBroker.CoveredGroupsMatching("Zippity doo dah", iAccount.Facility.Oid, iAccount.AdmitDate);
            Assert.AreEqual(0, noGroups.Count, "There should not have been any groups for this test");
        }
        [Test()]
        public void TestCoveredGroupsFor()
        {
            int groups = insuranceBroker.GetCoveredGroupCountFor( "1B132", new DateTime( 2003, 1, 1 ), new DateTime( 2006, 1, 30 ), 900, iAccount.AdmitDate );
            Assert.IsTrue( groups > 0, "No plan loaded for group with PlanID 1B132" );
        }

        [Test()]
        public void TestCoveredGroupsCountFor()
        {
            ICollection groups = insuranceBroker.CoveredGroupsFor("1B132", new DateTime(2003,1,1), new DateTime(2006, 1, 30), 900, iAccount.AdmitDate);
            Assert.IsNotNull(groups, "Cannot load covered groups");
            Assert.IsTrue(groups.Count > 0, "No plan loaded for group with PlanID 1B132");

            bool found = false;
            foreach (CoveredGroup group in groups)
            {
                if (group.Name.Equals("SEARS"))
                {
                    found = true;
                    ICollection plans = group.InsurancePlans;
                    Assert.IsNotNull(plans, "No plans list loaded for covered Group");
                    Assert.IsTrue(plans.Count > 0, "No plans loaded for covered group");
                    EmployerProxy emp = (EmployerProxy)group.Employer;
                    Assert.IsNotNull(emp, "No employer found attached to CoveredGroup");
                }
            }
            if (found == false)
            {
                Assert.Fail("Did not find expected Covered Group with Name='SEARS'");
            }

            ICollection noGroups = insuranceBroker.CoveredGroupsFor(PLAN_CODE2, new DateTime(2007, 12, 30), new DateTime(2007, 7, 30), 900, iAccount.AdmitDate);
            Assert.AreEqual(0, noGroups.Count, "There should not have been any groups for this test");

        }

        [Test()]
        public void TestPlanFinClasses()
        {
            ICollection finClasses = insuranceBroker.PlanFinClassesFor(facilityACO.Oid,"0Q");
            Assert.IsNotNull(finClasses, "Did not find any fin classes for plan suffix: 0Q");
            Assert.IsTrue(finClasses.Count > 0, "Did not find financial classes");
        }
        [Test()]
        public void TestGetDefaultInsurancePlan()
        {

            InsurancePlan defaultPlan = insuranceBroker.GetDefaultInsurancePlan(facilityDHF.Oid,
                                                                                iAccount.AdmitDate);
            Assert.IsNotNull(defaultPlan, "No Default Insurance Plan found in the database for Quick accounts.");

            Assert.AreEqual(DEFAULT_PLAN, defaultPlan.PlanID, "Default plan ID is not as expected");
        }

        /// <summary>
        /// Cardinality test: check to see if N1 returns the correct number of values.
        /// Always add 1 to the number of values you expect this to return as 
        /// the first value returned is always a blank. 
        /// </summary>
        [Test()]
        public void TestPlanFinClassesN1()
        {
            ICollection finClasses = insuranceBroker.PlanFinClassesFor( facilityACO.Oid, "N1" ) ;
            Assert.IsNotNull( finClasses, "Did not find any fin classes for plan suffix: N1" ) ;
            Assert.AreEqual( NUMBER_OF_RETURNED_RESULTS_FOR_SUFFIX_N1, finClasses.Count, "Incorrect number of financial classes found." );
        }

        [Test()]
        public void TestPlanFinClassesN()
        {
            ICollection finClasses = insuranceBroker.PlanFinClassesFor(facilityACO.Oid, "N");
            Assert.IsNotNull(finClasses, "Did not find any fin classes for plan suffix: N");
            Assert.IsTrue(finClasses.Count == NUMBER_OF_RETURNED_RESULTS_FOR_SUFFIX_N, "Incorrect number of financial classes found.");

            finClasses = insuranceBroker.PlanFinClassesFor(facilityACO.Oid, "N5");
            Assert.IsTrue(finClasses.Count == NUMBER_OF_RETURNED_RESULTS_FOR_SUFFIX_N_STAR, "Incorrect number of financial classes found.");

            finClasses = insuranceBroker.PlanFinClassesFor(facilityACO.Oid, "N*");
            Assert.IsTrue(finClasses.Count == NUMBER_OF_RETURNED_RESULTS_FOR_SUFFIX_N_STAR, "Incorrect number of financial classes found.");
        }

        [Test()]
        public void TestPlanFinClassesQ()
        {
            var finClasses = insuranceBroker.PlanFinClassesFor(facilityACO.Oid, "Q");
            Assert.AreEqual( NUMBER_OF_RETURNED_RESULTS_FOR_SUFFIX_Q, finClasses.Count, "Incorrect number of financial classes found." );
        }

        [Test()]
        [Ignore]
        public void TestPlanFinClassesQStar()
        {
            var finClasses = insuranceBroker.PlanFinClassesFor( facilityACO.Oid, "Q*" );
            Assert.AreEqual( NUMBER_OF_RETURNED_RESULTS_FOR_SUFFIX_Q_STAR, finClasses.Count, "Incorrect number of financial classes found." );

            finClasses = insuranceBroker.PlanFinClassesFor( facilityACO.Oid, "Q5" );
            Assert.AreEqual( NUMBER_OF_RETURNED_RESULTS_FOR_SUFFIX_Q5, finClasses.Count, "Incorrect number of financial classes found." );
        }

        [Test()]
        [Ignore]
        public void TestPlanFinClassesQ5()
        {
            var finClasses = insuranceBroker.PlanFinClassesFor( facilityACO.Oid, "Q5" );
            Assert.AreEqual( NUMBER_OF_RETURNED_RESULTS_FOR_SUFFIX_Q5, finClasses.Count, "Incorrect number of financial classes found." );
        }


        [Test()]
        public void TestPayorFinClasses()
        {
            ICollection finClasses = insuranceBroker.PayorFinClassesFor(facilityACO.Oid, "535");
            Assert.IsNotNull(finClasses, "Did not find any fin classes for plan suffix: 535");
            Assert.IsTrue(finClasses.Count > 0, "Did not find financial classes");
        }

        [Test()]
        public void TestValidHSVPlan()
        {
            IHSVBroker hsvBroker = BrokerFactory.BrokerOfType<IHSVBroker>();
            HospitalService hospitalService = hsvBroker.HospitalServiceWith(facilityACO.Oid, HSV_CODE);
            bool isValid = insuranceBroker.IsValidHSVPlanForSpecialtyMedicare(facilityACO.Oid, hospitalService.Code, PLAN_ID);
            Assert.IsTrue(isValid, "Valid Plan HSV mapping");

        }

        [Test()]
        public void TestinValidHSVPlan()
        {
            IHSVBroker hsvBroker = BrokerFactory.BrokerOfType<IHSVBroker>();
            HospitalService hospitalService = hsvBroker.HospitalServiceWith(facilityACO.Oid, HSV_CODE1);
            bool isValid = insuranceBroker.IsValidHSVPlanForSpecialtyMedicare(facilityACO.Oid, hospitalService.Code, PLAN_ID);
            Assert.IsTrue(!isValid, "Invalid Plan HSV mapping");
        }

        #endregion

        #region Support Methods

        #endregion

        #region Data Elements
        private static  IInsuranceBroker insuranceBroker    = null;
        private static  Account iAccount                    = null;

        private static Facility
            facilityDEL = null,
            facilityACO = null,
            facilityICE = null,
            facilityDHF = null;

        private static  MedicalGroupIPA iIpa                = null;
        private static  Clinic iClinic                      = null;

        private const string
            PLAN_CODE1                                      = "AA132",
            PLAN_CODE2                                      = "WZ801",
            PLAN_CODE3                                      = "070A3",
            DEFAULT_PLAN                                    = "UNK81";
        private const string
            HSV_CODE                                        = "88",
            HSV_CODE1                                       = "85",
            PLAN_ID                                         = "5354D";

        private static int
            ACCOUNT_NUM                                     = 496022,
            NUMBER_OF_RETURNED_RESULTS_FOR_SUFFIX_N1        = 3,
            NUMBER_OF_RETURNED_RESULTS_FOR_SUFFIX_N         = 3,
            NUMBER_OF_RETURNED_RESULTS_FOR_SUFFIX_N_STAR    = 3,
            NUMBER_OF_RETURNED_RESULTS_FOR_SUFFIX_Q         = 1,
            NUMBER_OF_RETURNED_RESULTS_FOR_SUFFIX_Q_STAR    = 1,
            NUMBER_OF_RETURNED_RESULTS_FOR_SUFFIX_Q5        = 3 ;
        #endregion
    }
}