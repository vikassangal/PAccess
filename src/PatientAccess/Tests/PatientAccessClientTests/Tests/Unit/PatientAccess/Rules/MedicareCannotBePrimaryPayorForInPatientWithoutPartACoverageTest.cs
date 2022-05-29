using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Domains;
using Microsoft.Pex.Framework.Settings;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.Utilities;

namespace Tests.Unit.PatientAccess.Rules
{
    /// <summary>
    /// Summary description for MedicareCannotBePrimaryPayorForInPatientWithoutPartACoverageTest
    /// </summary>
    [TestFixture]
    [Category( "Fast" )]
    [PexClass( typeof( MedicareCannotBePrimaryPayorForInPatientWithoutPartACoverage ) )]
    public partial class MedicareCannotBePrimaryPayorForInPatientWithoutPartACoverageTest
    {
        [Test]
        [ExpectedException( typeof( ArgumentException ) )]
        public void TestCanBeAppliedToWithInvalidContextType()
        {
            MedicareCannotBePrimaryPayorForInPatientWithoutPartACoverage ruleUnderTest = new MedicareCannotBePrimaryPayorForInPatientWithoutPartACoverage();
            object inValidObjectType = new object();
            ruleUnderTest.CanBeAppliedTo( inValidObjectType );
        }

        [Test]
        [ExpectedException( typeof( ArgumentNullException ) )]
        public void TestCanBeAppliedToWithNullContext()
        {
            var ruleUnderTest = new MedicareCannotBePrimaryPayorForInPatientWithoutPartACoverage();
            ruleUnderTest.CanBeAppliedTo( null );
        }

        [Test]
        public void TestCanBeAppliedTo_WithInPatientAndPartACoverage_ShouldReturnTrue()
        {

            var ruleUnderTest = new MedicareCannotBePrimaryPayorForInPatientWithoutPartACoverage();
            var account = new Account();
            account.KindOfVisit = new VisitType { Code = VisitType.INPATIENT };
            var partACoverage = new GovernmentMedicareCoverage();
            partACoverage.PartACoverage = new YesNoFlag( YesNoFlag.CODE_YES );
            account.Insurance.AddCoverage( partACoverage );
            var actualResult = ruleUnderTest.CanBeAppliedTo( account );
            Assert.IsTrue( actualResult );
        }

        [Test]
        public void TestCanBeAppliedTo_WithAdmitNewBornAndPartACoverage_ShouldViolateRule()
        {
            this.ParameterizedTestForCanBeAppliedTo( PatientType.InPatient, true, YesNoBlank.No, ActivityType.AdmitNewbornActivity );
        }
        [Test]
        public void TestCanBeAppliedTo_WithTransferERtoOutpatientPartACoverage_ShouldViolateRule()
        {
            this.ParameterizedTestForCanBeAppliedTo(PatientType.OutPatient, true, YesNoBlank.No, ActivityType.TransferERToOutpatientActivity);
        }
        /// <summary>
        /// Tests the can be applied to_ with medicare part A coverage.
        /// </summary>
        /// <param name="patientType">Type of the patient.</param>
        /// <param name="medicareIsPrimaryCoverage">if set to <c>true</c> [medicare is primary coverage].</param>
        /// <param name="hasPartACoverage">The has part A coverage.</param>
        /// <param name="activityType">Type of the activity.</param>
        [PexMethod( TestEmissionFilter = PexTestEmissionFilter.FailuresAndIncreasedBranchHits, MaxBranches = 80000 )]
        [PexEnumValuesDefined( typeof( PatientType ) )]
        [PexEnumValuesDefined( typeof( YesNoBlank ) )]
        [PexEnumValuesDefined( typeof( PatientType ) )]
        
        // ReSharper disable MemberCanBePrivate.Global
        public void ParameterizedTestForCanBeAppliedTo( PatientType patientType, bool medicareIsPrimaryCoverage, YesNoBlank hasPartACoverage, ActivityType activityType )
        {

            //limit patient types to in and out patient. This is done to reduce the number of permutations for which Pex has to generate tests 
            PexAssume.IsTrue( patientType == PatientType.InPatient || patientType == PatientType.OutPatient || patientType == PatientType.Null );

            //limit activities to valid activities and one invalid activity. This is done to reduce the number of permutations for which Pex has to generate tests
            PexAssume.IsTrue( IsValidActivity( activityType ) || activityType == ActivityType.TransferActivity || activityType == ActivityType.Null);

            Account account = GetAccountFor( hasPartACoverage, medicareIsPrimaryCoverage, patientType, activityType );

            var ruleUnderTest = new MedicareCannotBePrimaryPayorForInPatientWithoutPartACoverage();

            var canBeAppliedTo = ruleUnderTest.CanBeAppliedTo( account );
            var ruleWasViolated = !canBeAppliedTo;

            PexAssert
                .Case( IsValidActivity( activityType ) && patientType == PatientType.InPatient && medicareIsPrimaryCoverage && hasPartACoverage == YesNoBlank.No )
                .Implies( () => ruleWasViolated );

            PexAssert
                .Case( IsValidActivity( activityType ) && patientType == PatientType.InPatient && medicareIsPrimaryCoverage && hasPartACoverage != YesNoBlank.No )
                .Implies( () => !ruleWasViolated, "Rule should not be violated if selection is blank or yes" );

            PexAssert
                .Case( !IsValidActivity( activityType ) )
                .Implies( () => !ruleWasViolated, "Rule should not be violated for an in valid activity" );

            PexAssert
                .Case( patientType != PatientType.InPatient ).Implies( () => !ruleWasViolated, "Rule should not be violated for patient type other than InPatient" );

            PexAssert
                .Case( !medicareIsPrimaryCoverage ).Implies( () => !ruleWasViolated, "Rule should not be violated if medicare is not primary payor" );
            PexAssert
              .Case(IsValidActivity(activityType) && patientType == PatientType.OutPatient && medicareIsPrimaryCoverage && hasPartACoverage == YesNoBlank.No)
              .Implies(() => !ruleWasViolated);
        }
        // ReSharper restore MemberCanBePrivate.Global

        private static bool IsValidActivity( ActivityType activityType )
        {
            return  new MedicareCannotBePrimaryPayorForInPatientWithoutPartACoverage().IsValidActivity(
                CodeAndTypeHelper.GetActivityFrom( activityType ) );
        }

        private static Account GetAccountFor( YesNoBlank hasPartACoverage, bool medicareIsPrimaryCoverage, PatientType patientType, ActivityType activityType )
        {
            var medicareCoverage = new GovernmentMedicareCoverage
            {
                CoverageOrder = new CoverageOrder( 1L, CoverageOrder.PRIMARY_DESCRIPTION ),
                PartACoverage = CodeAndTypeHelper.GetYesNoFlagFrom( hasPartACoverage )
            };

            var otherCoverage = new GovernmentOtherCoverage { CoverageOrder = new CoverageOrder( 1L, CoverageOrder.PRIMARY_DESCRIPTION ) };
            var insurance = new Insurance();
            if (medicareIsPrimaryCoverage)
            {
                insurance.AddCoverage( medicareCoverage );
            }
            else
            {
                insurance.AddCoverage( otherCoverage );
            }
            VisitType kind = CodeAndTypeHelper.GetVisitTypeFrom( patientType );

            return new Account( 1L )
            {
                Activity = CodeAndTypeHelper.GetActivityFrom( activityType ),
                Insurance = insurance,
                KindOfVisit = kind
            };
        }
    }
}
