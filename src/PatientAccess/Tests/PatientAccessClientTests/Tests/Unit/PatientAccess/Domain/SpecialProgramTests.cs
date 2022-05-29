using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class SpecialProgramTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown SpecialProgramTests
        #endregion

        #region Methods
        [Test()]
        public void TestCollectRecommendationForConditionOne()
        {
            SpecialProgram program = new SpecialProgram();
            program.BlackLungBenefits = new YesNoFlag( YesNoFlag.CODE_YES );
            program.VisitForBlackLung = new YesNoFlag( YesNoFlag.CODE_YES );
            MSPRecommendation recommendation = new MSPRecommendation();
            program.CollectRecommendation( recommendation );
            Assert.AreEqual( recommendation.FirstSetOfQuestionsFailed, false );
            Assert.AreEqual( recommendation.IsMedicareRecommended, false );
            Assert.AreEqual( recommendation.ContributingConditions.Count, 2 );
            ContributingCondition specialBLCondition = new ContributingCondition( "Special Program",
                                                                                  "Are you receiving Black Lung (BL) benefits?", "Yes" );
            ContributingCondition specialBLConditionCheck = ( (ContributingCondition)recommendation.ContributingConditions[0] );

            Assert.AreEqual( specialBLConditionCheck.Answer, specialBLCondition.Answer );
            Assert.AreEqual( specialBLConditionCheck.Question, specialBLCondition.Question );
            Assert.AreEqual( specialBLConditionCheck.Section, specialBLCondition.Section );
        }

        [Test()]
        public void TestCollectRecommendationForConditionTwo()
        {
            SpecialProgram program = new SpecialProgram();
            program.GovernmentProgram = new YesNoFlag( YesNoFlag.CODE_YES );
            MSPRecommendation recommendation = new MSPRecommendation();
            program.CollectRecommendation( recommendation );
            Assert.AreEqual( recommendation.FirstSetOfQuestionsFailed, false );
            Assert.AreEqual( recommendation.IsMedicareRecommended, false );
            Assert.AreEqual( recommendation.ContributingConditions.Count, 1 );
            ContributingCondition specialBLCondition = new ContributingCondition( "Special Program",
                                                                                  "Are the services to be paid by a government program such as a " +
                                                                                  "research grant?", "Yes" );
            ContributingCondition specialBLConditionCheck = ( (ContributingCondition)recommendation.ContributingConditions[0] );

            Assert.AreEqual( specialBLConditionCheck.Answer, specialBLCondition.Answer );
            Assert.AreEqual( specialBLConditionCheck.Question, specialBLCondition.Question );
            Assert.AreEqual( specialBLConditionCheck.Section, specialBLCondition.Section );
        }

        [Test()]
        public void TestCollectRecommendationForConditionThree()
        {
            SpecialProgram program = new SpecialProgram();
            program.DVAAuthorized = new YesNoFlag( YesNoFlag.CODE_YES );
            MSPRecommendation recommendation = new MSPRecommendation();
            program.CollectRecommendation( recommendation );
            Assert.AreEqual( recommendation.FirstSetOfQuestionsFailed, false );
            Assert.AreEqual( recommendation.IsMedicareRecommended, false );
            Assert.AreEqual( recommendation.ContributingConditions.Count, 1 );
            ContributingCondition specialDVACondition = new ContributingCondition( "Special Program",
                                                                                   "Has the Department of Verteran Affairs (DVA) authorized and agreed " +
                                                                                   "to pay for care at this facility?", "Yes" );
            ContributingCondition specialDVAConditionCheck = ( (ContributingCondition)recommendation.ContributingConditions[0] );

            Assert.AreEqual( specialDVAConditionCheck.Answer, specialDVACondition.Answer );
            Assert.AreEqual( specialDVAConditionCheck.Question, specialDVACondition.Question );
            Assert.AreEqual( specialDVAConditionCheck.Section, specialDVACondition.Section );
        }

        [Test()]
        public void TestCollectRecommendationForConditionFour()
        {
            SpecialProgram program = new SpecialProgram();
            program.WorkRelated = new YesNoFlag( YesNoFlag.CODE_YES );
            MSPRecommendation recommendation = new MSPRecommendation();
            program.CollectRecommendation( recommendation );
            Assert.AreEqual( recommendation.FirstSetOfQuestionsFailed, false );
            Assert.AreEqual( recommendation.IsMedicareRecommended, false );
            Assert.AreEqual( recommendation.ContributingConditions.Count, 1 );
            ContributingCondition specialWorkCondition = new ContributingCondition( "Special Program",
                                                                                    "Was the illness/injury due to a work-related accident/condition?", "Yes" );
            ContributingCondition specialWorkConditionCheck = ( (ContributingCondition)recommendation.ContributingConditions[0] );

            Assert.AreEqual( specialWorkConditionCheck.Answer, specialWorkCondition.Answer );
            Assert.AreEqual( specialWorkConditionCheck.Question, specialWorkCondition.Question );
            Assert.AreEqual( specialWorkConditionCheck.Section, specialWorkCondition.Section );
        }

        [Test()]
        public void TestCollectRecommendationForConditionFive()
        {
            SpecialProgram program = new SpecialProgram();
            program.BlackLungBenefits = new YesNoFlag( YesNoFlag.CODE_YES );
            program.VisitForBlackLung = new YesNoFlag( YesNoFlag.CODE_NO );
            MSPRecommendation recommendation = new MSPRecommendation();
            program.CollectRecommendation( recommendation );
            Assert.AreEqual( recommendation.FirstSetOfQuestionsFailed, true );
            Assert.AreEqual( recommendation.IsMedicareRecommended, false );
            Assert.AreEqual( recommendation.ContributingConditions.Count, 5 );
            ContributingCondition specialBLCondition = new ContributingCondition( "Special Program",
                                                                                  "Are you receiving Black Lung (BL) benefits?", "Yes" );
            ContributingCondition specialBLConditionCheck = ( (ContributingCondition)recommendation.ContributingConditions[0] );

            Assert.IsTrue( recommendation.FirstSetOfQuestionsFailed );
            Assert.AreEqual( specialBLConditionCheck.Answer, specialBLCondition.Answer );
            Assert.AreEqual( specialBLConditionCheck.Question, specialBLCondition.Question );
            Assert.AreEqual( specialBLConditionCheck.Section, specialBLCondition.Section );
        }

        [Test()]
        public void TestCollectRecommendationForConditionSix()
        {
            SpecialProgram program = new SpecialProgram();
            program.BlackLungBenefits = new YesNoFlag( YesNoFlag.CODE_NO );
            program.VisitForBlackLung = new YesNoFlag( YesNoFlag.CODE_NO );
            MSPRecommendation recommendation = new MSPRecommendation();
            program.CollectRecommendation( recommendation );
            Assert.AreEqual( recommendation.FirstSetOfQuestionsFailed, true );
            Assert.AreEqual( recommendation.IsMedicareRecommended, false );
            Assert.AreEqual( recommendation.ContributingConditions.Count, 5 );
            ContributingCondition specialBLCondition = new ContributingCondition( "Special Program",
                                                                                  "Are you receiving Black Lung (BL) benefits?", "No" );
            ContributingCondition specialBLConditionCheck = ( (ContributingCondition)recommendation.ContributingConditions[0] );

            Assert.IsTrue( recommendation.FirstSetOfQuestionsFailed );
            Assert.AreEqual( specialBLConditionCheck.Answer, specialBLCondition.Answer );
            Assert.AreEqual( specialBLConditionCheck.Question, specialBLCondition.Question );
            Assert.AreEqual( specialBLConditionCheck.Section, specialBLCondition.Section );
        }

        [Test()]
        public void TestCloneAndEqual()
        {
            SpecialProgram program = new SpecialProgram();
            program.BlackLungBenefits = new YesNoFlag( YesNoFlag.CODE_YES );
            program.DVAAuthorized = new YesNoFlag( YesNoFlag.CODE_YES );
            program.GovernmentProgram = new YesNoFlag( YesNoFlag.CODE_NO );
            program.VisitForBlackLung = new YesNoFlag( YesNoFlag.CODE_YES );
            program.WorkRelated = new YesNoFlag( YesNoFlag.CODE_NO );
            SpecialProgram programClone = (SpecialProgram)program.Clone();
            Assert.IsTrue( program.Equals( programClone ) );
        }
        #endregion

        #region DataElements
        #endregion
    }
}