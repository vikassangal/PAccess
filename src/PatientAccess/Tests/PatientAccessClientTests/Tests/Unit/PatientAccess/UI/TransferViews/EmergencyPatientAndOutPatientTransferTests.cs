using System;
using NUnit.Framework;
using PatientAccess.Rules;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.TransferViews.EmergencyPatientAndOutPatient.ViewImpl;
using Rhino.Mocks;

namespace Tests.Unit.PatientAccess.UI.TransferViews
{
    /// <summary>
    /// Summary description for EmergencyPatientAndOutPatientTransferTests
    /// </summary>
    [TestFixture]
    [Category( "Fast" )]
    public class EmergencyPatientAndOutPatientTransferTests
    {
        #region Test Methods
        

        [Test]
        public void TestEvaluateNoPrimaryMedicareForAutoAccidentRuleOnFinish_WhenErrorMessageNotAlreadyDisplayed_ThenEvaluateRule()
        {
            var messageStateManager = MockRepository.GenerateStub<IMessageDisplayStateManager>();
            var ruleEngine = MockRepository.GenerateMock<IRuleEngine>();

            var transferOutToInView = new EmergencyPatientAndOutPatientStep3View( messageStateManager, ruleEngine );

            transferOutToInView.EvaluateNoPrimaryMedicareForAutoAccidentRuleOnFinish( delegate { } );

            transferOutToInView.RuleEngine.AssertWasCalled( x => x.OneShotRuleEvaluation<NoMedicarePrimaryPayorForAutoAccident>( Arg<object>.Is.Anything, Arg<EventHandler>.Is.Anything ) );
        }
        #endregion
    }
}