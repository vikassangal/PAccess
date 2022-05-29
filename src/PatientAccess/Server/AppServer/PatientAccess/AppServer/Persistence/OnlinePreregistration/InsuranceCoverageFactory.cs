using System;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Persistence.OnlinePreregistration
{
    public class InsuranceCoverageFactory
    {
        #region Constants

        public const string SELF_PAY_PLAN_ID = "66281";

        #endregion

        #region Methods

        public Coverage BuildDefaultPrimaryInsuranceCoverageForUnInsured( DateTime admitDate, Address billingAddress )
        {
            InsurancePlan plan = null;
            if ( InsurancePlan.VerifyForGenericAndMasterPlans( SELF_PAY_PLAN_ID ) )
            {
                plan = insuranceBroker.PlanWith( SELF_PAY_PLAN_ID, Facility.Oid, admitDate );
            }

            Coverage coverage = null;
            coverage = Coverage.CoverageFor( plan, new Insured() );

            coverage.CoverageOrder = new CoverageOrder( CoverageOrder.PRIMARY_OID, "Primary" );

            coverage.Insured = new Insured { Employment = new Employment() };
            coverage.IsNew = true;
            coverage.Account.Facility = Facility;
            coverage.BillingInformation.Address = billingAddress;

            return coverage;
        }

        #endregion

        #region Private Properties

        private Facility Facility
        {
            get { return i_Facility; }
        }

        #endregion

        #region Construction and Finalization

        public InsuranceCoverageFactory( Facility facility )
        {
            i_Facility = facility;
        }

        #endregion

        #region Data Elements
        private readonly Facility i_Facility;
        private readonly IInsuranceBroker insuranceBroker = BrokerFactory.BrokerOfType<IInsuranceBroker>();

        #endregion
    }
}
