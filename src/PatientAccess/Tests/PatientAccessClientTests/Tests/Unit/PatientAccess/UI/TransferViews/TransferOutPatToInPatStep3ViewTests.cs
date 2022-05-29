using System;
using NUnit.Framework;
using PatientAccess.Rules;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.TransferViews;
using Rhino.Mocks;

namespace Tests.Unit.PatientAccess.UI.TransferViews
{
    /// <summary>
    /// Summary description for TransferOutPatToInPatStep3ViewTests
    /// </summary>
    [TestFixture]
    [Category( "Fast" )]
    public class TransferOutPatToInPatStep3ViewTests
    {
        #region Test Methods
        [Test]
        public void TestEvaluateNoPrimaryMedicareForAutoAccidentRuleOnFinish_WhenErrorMessageAlreadyDisplayed_ThenShouldoNotEvaluateRule()
        {
            var messageStateManager = MockRepository.GenerateStub<IMessageDisplayStateManager>();
            var ruleEngine = MockRepository.GenerateMock<IRuleEngine>();

            var transferOutToInView = new TransferOutPatToInPatStep3View( messageStateManager, ruleEngine );

            transferOutToInView.MessageStateManager.Stub(
                x => x.HasErrorMessageBeenDisplayedEarlierFor( typeof( NoMedicarePrimaryPayorForAutoAccident ).ToString() ) ).Return( true );

            transferOutToInView.EvaluateNoPrimaryMedicareForAutoAccidentRuleOnFinish( delegate { } );

            transferOutToInView.RuleEngine.AssertWasNotCalled( x => x.EvaluateRule<NoMedicarePrimaryPayorForAutoAccident>( Arg<object>.Is.Anything ) );
        }

        [Test]
        public void TestEvaluateNoPrimaryMedicareForAutoAccidentRuleOnFinish_WhenErrorMessageNotAlreadyDisplayed_ThenEvaluateRule()
        {
            var messageStateManager = MockRepository.GenerateStub<IMessageDisplayStateManager>();
            var ruleEngine = MockRepository.GenerateMock<IRuleEngine>();

            var transferOutToInView = new TransferOutPatToInPatStep3View( messageStateManager, ruleEngine );

            transferOutToInView.EvaluateNoPrimaryMedicareForAutoAccidentRuleOnFinish( delegate { } );

            transferOutToInView.RuleEngine.AssertWasCalled( x => x.OneShotRuleEvaluation<NoMedicarePrimaryPayorForAutoAccident>( Arg<object>.Is.Anything, Arg<EventHandler>.Is.Anything ) );
        }
        #endregion
    }
}