using System;
using Extensions.UI.Builder;
using NUnit.Framework;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Rules;
using PatientAccess.UI;
using PatientAccess.UI.DiagnosisViews;
using Rhino.Mocks;

namespace Tests.Unit.PatientAccess.UI.DiagnosisViews
{
    [TestFixture]
    public class DiagnosisViewTests
    {
        #region Test Methods

        [Test]
        [Category( "Fast" )]
        public void TestDisplayMethodForCurrentCondition()
        {
            var view = new DiagnosisView {Model = new Account {Diagnosis = new Diagnosis {Condition = new Illness()}}};

            var method = view.DisplayMethodForCurrentCondition();

            Assert.IsNotNull( view.ConditionMap );
            Assert.IsNotNull( method );
        }

        [Test]
        [ExpectedException( typeof( InvalidOperationException ) )]
        [Category( "Fast" )]
        public void DisplayMethodForCurrentConditionWithoutModel()
        {
            DiagnosisView view = new DiagnosisView {Model = new Account {Diagnosis = null}};

            view.DisplayMethodForCurrentCondition();
        }

        [Test]
        [Ignore]
        public void TestOccurrenceCodeManager()
        {
            PatientBrokerProxy patientBroker = new PatientBrokerProxy();

            PatientSearchCriteria criteria = new PatientSearchCriteria(
                "DEL",
                String.Empty,
                String.Empty,
                String.Empty,
                null,
                PatientSearchCriteria.NO_MONTH,
                PatientSearchCriteria.NO_YEAR,
                String.Empty,
                "5129966"
                );

            PatientSearchResponse patientSearchResponse = patientBroker.GetPatientSearchResponseFor( criteria );
            Patient aPatient = patientBroker.PatientFrom( patientSearchResponse.PatientSearchResults[0] );

            AccountProxy accountProxy = aPatient.Accounts[0] as AccountProxy;

            Account realAccount = accountProxy.AsAccount();

            //Test Illnes
            DiagnosisView view = new DiagnosisView();
            view.Model.Diagnosis = new Diagnosis();

            Illness illness = (Illness)view.Model.Diagnosis.Condition;
            view.onsetDateTxt = "Jan 21, 2005";
            illness.Onset = Convert.ToDateTime( view.onsetDateTxt );


            OccurrenceCode foundOC = null;
            foreach ( OccurrenceCode oc in realAccount.OccurrenceCodes )
            {
                if ( oc.Code == OccurrenceCode.OCCURRENCECODE_ILLNESS )
                {
                    foundOC = oc;
                }
            }
            Assert.IsNotNull( foundOC );

            Assert.AreEqual( 1, realAccount.OccurrenceCodes.Count );


            //Pregnancy Tests
            view = new DiagnosisView();
            view.Model.Diagnosis = new Diagnosis();
            Pregnancy pr = new Pregnancy();
            view.Model.Diagnosis.Condition = pr;
            view.lastMenstrualDateTxt = "Jan 21, 2005";
            pr.LastPeriod = Convert.ToDateTime( view.lastMenstrualDateTxt );


            foundOC = null;
            foreach ( OccurrenceCode oc in realAccount.OccurrenceCodes )
            {
                if ( oc.Code == OccurrenceCode.OCCURRENCECODE_LASTMENSTRUATION )
                {
                    foundOC = oc;
                }
            }
            Assert.IsNotNull( foundOC );
            Assert.AreEqual( 1, realAccount.OccurrenceCodes.Count );


            //Crime Tests
            view.Model.Diagnosis = new Diagnosis();
            Crime cr = new Crime();
            view.accidentDateTxt = "Jan 21, 2005";
            cr.OccurredOn = Convert.ToDateTime( view.accidentDateTxt );

            foundOC = null;
            foreach ( OccurrenceCode oc in realAccount.OccurrenceCodes )
            {
                if ( oc.Code == OccurrenceCode.OCCURRENCECODE_CRIME )
                {
                    foundOC = oc;
                }
            }
            Assert.IsNotNull( foundOC );
            Assert.AreEqual( 1, realAccount.OccurrenceCodes.Count );


            //Accident Tests when TypeOfAccident is set first
            view.Model.Diagnosis = new Diagnosis();
            TypeOfAccident type = new TypeOfAccident();
            type.Description = "AUTO";
            type.Code = OccurrenceCode.OCCURRENCECODE_ACCIDENT_AUTO;
            view.accidentType = type;

            Accident a = new Accident();

            view.accidentDateTxt = "Jan 21, 2005";
            a.Kind = view.accidentType;
            a.OccurredOn = Convert.ToDateTime( view.accidentDateTxt );

            foundOC = null;
            foreach ( OccurrenceCode oc in realAccount.OccurrenceCodes )
            {
                if ( oc.Code == OccurrenceCode.OCCURRENCECODE_ACCIDENT_AUTO )
                {
                    foundOC = oc;
                }
            }
            Assert.IsNotNull( foundOC );
            Assert.AreEqual( 1, realAccount.OccurrenceCodes.Count );

            //Accident Tests when OccuredOn Date is set first
            view.Model.Diagnosis = new Diagnosis();
            type = new TypeOfAccident();
            type.Description = "AUTO";
            type.Code = OccurrenceCode.OCCURRENCECODE_ACCIDENT_AUTO;
            view.accidentType = type;

            a = new Accident();

            view.accidentDateTxt = "Jan 21, 2005";

            a.OccurredOn = Convert.ToDateTime( view.accidentDateTxt );
            a.Kind = view.accidentType;

            foundOC = null;
            foreach ( OccurrenceCode oc in realAccount.OccurrenceCodes )
            {
                if ( oc.Code == OccurrenceCode.OCCURRENCECODE_ACCIDENT_AUTO )
                {
                    foundOC = oc;
                }
            }
            Assert.IsNotNull( foundOC );
            Assert.AreEqual( 1, realAccount.OccurrenceCodes.Count );

        }

        [Test]
        [Category( "Fast" )]
        public void TestEnableOrDisableEDLogFields_ShouldFireEvent()
        {
            DiagnosisView diagnosisView = new DiagnosisView();
            diagnosisView.Model = new Account { Activity = new RegistrationActivity() };

            var displayManager = MockRepository.GenerateStub<IEDLogsDisplayPresenter>();
            diagnosisView.EdLogDisplayPresenter = displayManager;
            VisitTypeEventArgs e = new VisitTypeEventArgs( new VisitType() );
            diagnosisView.EnableOrDisableEdLogFields( diagnosisView, e );

            displayManager.AssertWasCalled( x => x.UpdateEDLogDisplay( e.VisitType, false, false ) );

        }

        [Test]
        public void TestEvaluateNoMedicarePrimaryPayorForAutoAccidentRule_WhenCoverageIsMedicare_ShouldFireEvent()
        {

            var medicareCoverage = new GovernmentMedicareCoverage { CoverageOrder = new CoverageOrder( 1L, CoverageOrder.PRIMARY_DESCRIPTION ) };

            Account account = GetAutoAccidentRegistrationAccountWith( medicareCoverage );

            var diagnosisView = new DiagnosisView { Model = account };

            AddRuleToRuleEngine( typeof( NoMedicarePrimaryPayorForAutoAccident ), new NoMedicarePrimaryPayorForAutoAccident() );

            bool result = diagnosisView.EvaluateNoMedicarePrimaryPayorForAutoAccidentRule( delegate { } );

            Assert.IsFalse( result );
        }

        [Test]
        public void TestEvaluateNoMedicarePrimaryPayorForAutoAccidentRule_WhenCoverageIsCommercial_ShouldNotFireEvent()
        {
            var commercialCoverage = new CommercialCoverage { CoverageOrder = new CoverageOrder( 1L, CoverageOrder.PRIMARY_DESCRIPTION ) };

            Account account = GetAutoAccidentRegistrationAccountWith( commercialCoverage );

            var diagnosisView = new DiagnosisView { Model = account };

            AddRuleToRuleEngine( typeof( NoMedicarePrimaryPayorForAutoAccident ), new NoMedicarePrimaryPayorForAutoAccident() );

            bool result = diagnosisView.EvaluateNoMedicarePrimaryPayorForAutoAccidentRule( delegate { } );

            Assert.IsTrue( result );
        }
        [Test]
        public void TestEvaluateProcedureRequiredRule_WhenVisitTypeIsInpatient_ShouldNotFireEvent()
        {

            Account account = GetRegistrationAccountWith( new VisitType( 1L, DateTime.Now, VisitType.INPATIENT_DESC, VisitType.INPATIENT ) );

            var diagnosisView = new DiagnosisView { Model = account };

            AddRuleToRuleEngine( typeof( ProcedureRequired ), new ProcedureRequired() );

            bool result = diagnosisView.EvaluateProcedureRequiredRule( delegate { } );

            Assert.IsFalse( result );
        }

        #endregion

        #region Private Methods

        private static Account GetAutoAccidentRegistrationAccountWith( Coverage coverage )
        {
            var insurance = new Insurance();
            insurance.AddCoverage( coverage );

            var account = new Account { Activity = new RegistrationActivity(), Insurance = insurance };

            var condition = new Accident { Kind = TypeOfAccident.NewAuto() };
            condition.Kind.OccurrenceCode.Code = OccurrenceCode.OCCURRENCECODE_ACCIDENT_AUTO;

            account.Diagnosis.Condition = condition;
            return account;
        }
        private static Account GetRegistrationAccountWith( VisitType VisitType )
        {
            var account = new Account { Activity = new RegistrationActivity(), KindOfVisit = VisitType };

            var condition = new Accident { Kind = TypeOfAccident.NewAuto() };
            condition.Kind.OccurrenceCode.Code = OccurrenceCode.OCCURRENCECODE_ACCIDENT_AUTO;

            account.Diagnosis.Condition = condition;
            return account;
        }
        private static void AddRuleToRuleEngine( Type type, LeafRule ruleInstance )
        {
            if ( RuleEngine.RulesToRun.Contains( type ) )
            {
                RuleEngine.RulesToRun.Remove( type );
            }

            RuleEngine.RulesToRun.Add( type, ruleInstance );
        }

        #endregion

        #region Private Properties

        private static RuleEngine RuleEngine
        {
            get
            {
                return RuleEngine.GetInstance();
            }
        }

        #endregion
    }
}