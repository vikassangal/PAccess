using System;
using NUnit.Framework;
using PatientAccess.Rules;
using PatientAccess.UI;
using PatientAccess.UI.CommonControls;
using Rhino.Mocks;

namespace Tests.Unit.PatientAccess.UI
{
    [TestFixture]
    [Category( "Fast" )]
    public class AccountViewTests
    {
        #region Test Methods
        [Test]
        public void TestEvaluateNoPrimaryMedicareForAutoAccidentRuleOnFinish_WhenErrorMessageAlreadyDisplayed_ThenDoNotEvaluateRule()
        {
            var messageStateManager = MockRepository.GenerateStub<IMessageDisplayStateManager>();
            var ruleEngine = MockRepository.GenerateMock<IRuleEngine>();

            var accountView = new AccountView( messageStateManager, ruleEngine );
            
            accountView.MessageStateManager.Stub( 
                x => x.HasErrorMessageBeenDisplayedEarlierFor( typeof( NoMedicarePrimaryPayorForAutoAccident ).ToString() ) ).Return( true );
            
            accountView.EvaluateNoPrimaryMedicareForAutoAccidentRuleOnFinish( delegate {} );

            accountView.RuleEngine.AssertWasNotCalled( x => x.EvaluateRule<NoMedicarePrimaryPayorForAutoAccident>( Arg<object>.Is.Anything ) );
        }

        [Test]
        public void TestEvaluateNoPrimaryMedicareForAutoAccidentRuleOnFinish_WhenErrorMessageNotAlreadyDisplayed_ThenEvaluateRule()
        {
            var messageStateManager = MockRepository.GenerateStub<IMessageDisplayStateManager>();
            var ruleEngine = MockRepository.GenerateMock<IRuleEngine>();

            var accountView = new AccountView( messageStateManager, ruleEngine );

            accountView.EvaluateNoPrimaryMedicareForAutoAccidentRuleOnFinish( delegate { } );

            accountView.RuleEngine.AssertWasCalled( x => x.OneShotRuleEvaluation<NoMedicarePrimaryPayorForAutoAccident>( Arg<object>.Is.Anything, 
                                                                                                                         Arg<EventHandler>.Is.Anything ) );
        }
        #endregion

    }
}
