using System;
using System.Collections;
using System.Reflection;
using Extensions.PersistenceCommon;

namespace PatientAccess.Domain
{
    [Serializable]
    public class Insurance : PersistentModel
    {

        #region Fields

        private readonly Hashtable i_Coverages = new Hashtable();
        private FinancialClass i_FinancialClass = new FinancialClass();
        private bool i_HasNoLiability;
        private readonly Hashtable i_OrigCoverages = new Hashtable();

        #endregion Fields

        #region Constructors

        public Insurance()
        {
        }

        #endregion Constructors

        #region Properties

        public ICollection Coverages
        {
            get
            {
                return this.i_Coverages.Values;
            }
        }

        public FinancialClass FinancialClass
        {
            get
            {
                return i_FinancialClass;
            }
        }

        public bool HasNoLiability
        {
            get
            {
                foreach (Coverage coverage in this.Coverages)
                {
                    if (coverage.CoverageOrder.Oid == CoverageOrder.PRIMARY_OID)
                    {
                        i_HasNoLiability = coverage.NoLiability;
                    }
                }
                return i_HasNoLiability;
            }
            set
            {
                this.SetAndTrack( ref this.i_HasNoLiability, value, MethodBase.GetCurrentMethod() );
                foreach (Coverage coverage in this.Coverages)
                {
                    if (coverage.CoverageOrder.Oid == CoverageOrder.PRIMARY_OID)
                    {
                        coverage.NoLiability = i_HasNoLiability;
                    }
                }
            }
        }

        public decimal PrimaryCopay
        {
            get
            {
                decimal primaryCopay = 0;

                foreach (Coverage coverage in this.Coverages)
                {
                    primaryCopay = coverage.CoverageOrder.Oid == 1 ? coverage.CoPay : 0;
                }

                return primaryCopay;
            }
        }

        public Coverage PrimaryCoverage
        {
            get
            {
                return this.CoverageFor( CoverageOrder.PRIMARY_OID );
            }
        }

        public decimal PrimaryDeductible
        {
            get
            {
                decimal primaryDeductible = 0m;

                foreach (Coverage coverage in this.Coverages)
                {
                    primaryDeductible = coverage.CoverageOrder.Oid == 1 ? coverage.Deductible : 0;
                }

                return primaryDeductible;
            }
        }

        public Coverage SecondaryCoverage
        {
            get
            {
                return this.CoverageFor( CoverageOrder.SECONDARY_OID );
            }
        }

        #endregion Properties

        #region Methods

        public void AddCoverage( Coverage aCoverage )
        {
            AddCoverage( aCoverage, i_Coverages );
        }

        public void AddOrigCoverage( Coverage aCoverage )
        {
            AddCoverage( aCoverage, i_OrigCoverages );
        }

        public Coverage CoverageFor( CoverageOrder aCoverageOrder )
        {
            return CoverageFor( aCoverageOrder, this.i_Coverages );
        }

        public Coverage CoverageFor( long aCoverageOrderOid )
        {
            return CoverageFor( aCoverageOrderOid, this.i_Coverages );
        }

        public Coverage OrigCoverageFor( CoverageOrder aCoverageOrder )
        {
            return CoverageFor( aCoverageOrder, this.i_OrigCoverages );
        }

        public void RemoveCoverage( Coverage aCoverage )
        {

            if (aCoverage != null)
            {

                this.RemoveCoverage( aCoverage.CoverageOrder.Oid );

            }

        }

        private void RemoveCoverage( long coverageOrder )
        {

            if (this.i_Coverages.Contains( coverageOrder ))
            {

                string oldPlanId = String.Empty;
                Coverage targetCoverage = this.i_Coverages[coverageOrder] as Coverage;

                this.i_Coverages.Remove( coverageOrder );

                if (targetCoverage != null && targetCoverage.InsurancePlan != null)
                {

                    oldPlanId = targetCoverage.InsurancePlan.PlanID;

                }

                switch (coverageOrder)
                {

                    case CoverageOrder.PRIMARY_OID:
                        this.TrackChange( "PrimaryCoverage", oldPlanId, String.Empty );
                        break;

                    case CoverageOrder.SECONDARY_OID:
                        this.TrackChange( "SecondaryCoverage", oldPlanId, String.Empty );
                        break;

                }//switch

            }

        }

        public void RemovePrimaryCoverage()
        {

            this.RemoveCoverage( CoverageOrder.PRIMARY_OID );

        }

        public void RemoveSecondaryCoverage()
        {

            this.RemoveCoverage( CoverageOrder.SECONDARY_OID );

        }

        public void SetAsPrimary( Coverage aCoverage )
        {
            this.ValidateCoverage( aCoverage );

            // refresh key, value pair in this.Coverages
            Hashtable ht = new Hashtable();

            foreach (Coverage coverage in this.Coverages)
            {
                coverage.CoverageOrder = CoverageOrder.NewSecondaryCoverageOrder();
            }
            aCoverage.CoverageOrder = CoverageOrder.NewPrimaryCoverageOrder();

            foreach (Coverage coverage in this.Coverages)
            {
                ht.Add( coverage.CoverageOrder.Oid, coverage );
            }
            i_Coverages.Clear();

            foreach (Coverage coverage in ht.Values)
            {
                this.AddCoverage( coverage );
            }
        }

        public void SetAsSecondary( Coverage aCoverage )
        {
            this.ValidateCoverage( aCoverage );

            foreach (Coverage coverage in this.Coverages)
            {
                coverage.CoverageOrder = CoverageOrder.NewPrimaryCoverageOrder();
            }

            aCoverage.CoverageOrder = CoverageOrder.NewSecondaryCoverageOrder();
        }


        private void AddCoverage( Coverage aCoverage, IDictionary coverages )
        {
            if (aCoverage != null)
            {
                string oldPlanId = null;

                if (coverages.Contains( aCoverage.CoverageOrder.Oid ))
                {

                    oldPlanId = ( (Coverage)coverages[aCoverage.CoverageOrder.Oid] ).InsurancePlan.PlanID;
                    coverages.Remove( aCoverage.CoverageOrder.Oid );
                }

                coverages.Add( aCoverage.CoverageOrder.Oid, aCoverage );

                string newPlanId = String.Empty;

                if (aCoverage.InsurancePlan != null)
                {
                    newPlanId = aCoverage.InsurancePlan.PlanID;
                }

                switch (aCoverage.CoverageOrder.Oid)
                {

                    case CoverageOrder.PRIMARY_OID:
                        TrackChange( "PrimaryCoverage", oldPlanId, newPlanId );
                        break;

                    case CoverageOrder.SECONDARY_OID:
                        TrackChange( "SecondaryCoverage", oldPlanId, newPlanId );
                        break;
                }

            }
        }

        private static Coverage CoverageFor( PersistentModel aCoverageOrder, IDictionary coverages )
        {
            return coverages[aCoverageOrder.Oid] as Coverage;
        }

        private static Coverage CoverageFor( long aCoverageOrderOid, IDictionary coverages )
        {
            return coverages[aCoverageOrderOid] as Coverage;
        }

        /// <exception cref="ArgumentException">aCoverage cannot have 'null' for its CoverageOrder</exception>
        private void ValidateCoverage( Coverage aCoverage )
        {
            if (aCoverage.CoverageOrder == null)
            {
                throw new ArgumentException( "aCoverage cannot have 'null' for its CoverageOrder" );
            }

            if (!this.i_Coverages.Contains( aCoverage.CoverageOrder.Oid ))
            {
                throw new ArgumentException( "Insurance does not already contain the supplied coverage." );
            }
        }

        #endregion Methods

        /// <summary>
        /// Gets the primary coverage. Returns null if there is no primary coverage
        /// </summary>
        /// <returns></returns>
        public Coverage GetPrimaryCoverage()
        {
            Coverage coverageReturned = null;
            
            ICollection coverageCollection = Coverages;

            if (coverageCollection == null)
            {
                return coverageReturned;
            }

            foreach (Coverage coverage in coverageCollection)
            {
                if (coverage == null)
                {
                    continue;
                }
                
                else if (coverage.CoverageOrder.Oid.Equals(CoverageOrder.PRIMARY_OID))
                {
                    coverageReturned = coverage;
                }
            }
            return coverageReturned;
        }

        /// <summary>
        /// Gets the secondary coverage. Returns null if there is no secondary coverage
        /// </summary>
        /// <returns></returns>
        public Coverage GetSecondaryCoverage()
        {
            Coverage coverageReturned = null;

            ICollection coverageCollection = Coverages;

            if (coverageCollection == null)
            {
                return coverageReturned;
            }

            foreach (Coverage coverage in coverageCollection)
            {
                if (coverage == null)
                {
                    continue;
                }
                else if (coverage.CoverageOrder.Oid.Equals(CoverageOrder.SECONDARY_OID))
                {
                    coverageReturned = coverage;
                }
            }
            return coverageReturned;
        }
    }
}