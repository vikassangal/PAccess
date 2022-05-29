using System;
using System.Collections;
using Extensions.PersistenceCommon;

namespace PatientAccess.Domain
{
    [Serializable]
    public class SortPlanContractsByPlanName : IComparer
    {
        public int Compare( object obj1, object obj2 )
        {
            InsurancePlanContract a, b;
            a = (InsurancePlanContract)obj1;
            b = (InsurancePlanContract)obj2;

            return a.PlanName.CompareTo( b.PlanName );
        }
    }

    [Serializable]
    public class InsurancePlanContract : PersistentModel, IComparable
    {
        #region Event Handlers
        #endregion

        #region Methods

        /// <exception cref="ArgumentException">object is not an InsurancePlanContract</exception>
        public int CompareTo( object obj )
        {
            if( obj is InsurancePlanContract )
            {
                InsurancePlanContract contract = (InsurancePlanContract)obj;

                int i = this.EffectiveOn.CompareTo( contract.EffectiveOn );

                // invert the sort ( this causes the sort to be descending )
               
                return i * (-1);
            }

            throw new ArgumentException( "object is not an InsurancePlanContract" );
        }

        #endregion

        public bool IsValidFor( DateTime admitDate, bool includeGraceDays )
        {
            bool blnRC = false;

            if( includeGraceDays )
            {
                if(
                    this.ApprovedOn != DateTime.MinValue
                    &&
                    this.EffectiveOn != DateTime.MinValue
                    &&
                    this.EffectiveOn <= admitDate
                    &&
                    ( this.TerminatedOn == DateTime.MinValue || this.AdjustedTerminationDate >= admitDate )
                    &&
                    ( this.CanceledOn == DateTime.MinValue || this.AdjustedCancellationDate >= admitDate )
                    )
                {
                    blnRC = true;
                }
            }
            else
            {
                if(
                    this.ApprovedOn != DateTime.MinValue
                    &&
                    this.EffectiveOn != DateTime.MinValue
                    &&
                    this.EffectiveOn <= admitDate
                    &&
                    ( this.TerminatedOn == DateTime.MinValue || this.TerminatedOn >= admitDate )
                    &&
                    ( this.CanceledOn == DateTime.MinValue || this.CanceledOn >= admitDate )
                    )
                {
                    blnRC = true;
                }
            }

            return blnRC;
        }


        #region Properties

        public ICollection BillingInformations
        {
            get
            {
                return (ICollection)PrimBillingInformations.Clone();
            }
        }

        public string LineOfBusiness
        {
            get
            {
                return i_LineOfBusiness;
            }
            set
            {
                i_LineOfBusiness = value;
            }
        }

        public string PlanSuffix
        {
            get
            {
                return i_PlanSuffix;
            }
            set
            {
                i_PlanSuffix = value;
            }
        }

        public string PlanName
        {
            get
            {
                return i_PlanName;
            }
            set
            {
                i_PlanName = value;
            }
        }

        public Payor Payor
        {
            get
            {
                return i_Payor;
            }
            set
            {
                i_Payor = value;
            }
        }

        public InsurancePlanCategory PlanCategory
        {
            get
            {
                return i_PlanCategory;
            }
            set
            {
                i_PlanCategory = value;
            }
        }

        public InsurancePlanType PlanType
        {
            get
            {
                return i_PlanType;
            }
            set
            {
                i_PlanType = value;
            }
        }

        public DateTime EffectiveOn
        {
            get
            {
                return i_EffectiveOn;
            }
            set
            {
                i_EffectiveOn = value;
            }
        }

        public DateTime ApprovedOn
        {
            get
            {
                return i_ApprovedOn;
            }
            set
            {
                i_ApprovedOn = value;
            }
        }

        public DateTime TerminatedOn
        {
            get
            {
                return i_TerminatedOn;
            }
            set
            {
                i_TerminatedOn = value;
            }
        }

        public DateTime CanceledOn
        {
            get
            {
                return i_CanceledOn;
            }
            set
            {
                i_CanceledOn = value;
            }
        }


        // TLG 3/16/2007 Add the FacInsuranceOpts values for extending the
        // Termination and Cancellation dates.  (Also, adding the Purge days for
        // future use.)

        // The termination date with the grace period added

        public DateTime AdjustedTerminationDate
        {
            get
            {
                return i_AdjustedTerminationDate;
            }
            set
            {
                i_AdjustedTerminationDate = value;
            }
        }

        // The cancellation date with the grace period added

        public DateTime AdjustedCancellationDate
        {
            get
            {
                return i_AdjustedCancellationDate;
            }
            set
            {
                i_AdjustedCancellationDate = value;
            }
        }

        public long TerminationGraceDays
        {
            get
            {
                return i_TerminationGraceDays;
            }
            set
            {
                i_TerminationGraceDays = value;
            }
        }

        public long CancellationGraceDays
        {
            get
            {
                return i_CancellationGraceDays;
            }
            set
            {
                i_CancellationGraceDays = value;
            }
        }

        public long TerminationPurgeDays
        {
            get
            {
                return i_TerminationPurgeDays;
            }
            set
            {
                i_TerminationPurgeDays = value;
            }
        }

        public long CancellationPurgeDays
        {
            get
            {
                return i_CancellationPurgeDays;
            }
            set
            {
                i_CancellationPurgeDays = value;
            }
        }

        #endregion

        #region Private Methods

        #endregion

        #region Private Properties

        private ArrayList PrimBillingInformations
        {
            get
            {
                if (i_BillingInformations == null)
                {
                    i_BillingInformations = new ArrayList();
                }
                return i_BillingInformations;
            }
            set
            {
                i_BillingInformations = value;
            }
        }

        #endregion

        #region Construction and Finalization

        public InsurancePlanContract()
        {
        }

        public InsurancePlanContract(
            InsurancePlanType planType,
            string planSuffix,
            string lineOfBusiness,
            InsurancePlanType insurancePlanType,
            DateTime effectiveOn,
            DateTime approvedOn,
            DateTime terminatedOn,
            DateTime canceledOn,
            DateTime adjustedTerminationDate,
            DateTime adjustedCancellationDate)
        {
            this.i_PlanSuffix = planSuffix;
            this.i_LineOfBusiness = lineOfBusiness;
            this.i_PlanType = planType;
            this.i_EffectiveOn = effectiveOn;
            this.i_ApprovedOn = approvedOn;
            this.i_TerminatedOn = terminatedOn;
            this.i_CanceledOn = canceledOn;
            this.i_AdjustedTerminationDate = adjustedTerminationDate;
            this.i_AdjustedCancellationDate = adjustedCancellationDate;
        }

        public InsurancePlanContract(
            InsurancePlanType planType,
            string planSuffix,
            string lineOfBusiness,
            InsurancePlanType insurancePlanType,
            DateTime effectiveOn,
            DateTime approvedOn,
            DateTime terminatedOn,
            DateTime canceledOn,
            DateTime adjustedTerminationDate,
            DateTime adjustedCancellationDate,
            long terminationGraceDays,
            long cancellationGraceDays,
            long terminationPurgeDays,
            long cancellationPurgeDays)
        {
            this.i_PlanSuffix = planSuffix;
            this.i_LineOfBusiness = lineOfBusiness;
            this.i_PlanType = planType;
            this.i_EffectiveOn = effectiveOn;
            this.i_ApprovedOn = approvedOn;
            this.i_TerminatedOn = terminatedOn;
            this.i_CanceledOn = canceledOn;
            this.i_AdjustedTerminationDate = adjustedTerminationDate;
            this.i_AdjustedCancellationDate = adjustedCancellationDate;

            this.i_TerminationGraceDays = terminationGraceDays;
            this.i_CancellationGraceDays = cancellationGraceDays;
            this.i_TerminationPurgeDays = terminationPurgeDays;
            this.i_CancellationPurgeDays = cancellationPurgeDays;
        }

        #endregion

        #region Data Elements

        private string i_PlanName                       = string.Empty;
        private string i_PlanSuffix                     = string.Empty;
        private string i_LineOfBusiness                 = string.Empty;

        private InsurancePlanType i_PlanType            = new InsurancePlanType();

        private ArrayList i_BillingInformations         = new ArrayList();

        private DateTime i_EffectiveOn                  = DateTime.MinValue;
        private DateTime i_ApprovedOn                   = DateTime.MinValue;
        private DateTime i_TerminatedOn                 = DateTime.MinValue;
        private DateTime i_CanceledOn                   = DateTime.MinValue;
        private DateTime i_AdjustedTerminationDate      = DateTime.MinValue;
        private DateTime i_AdjustedCancellationDate     = DateTime.MinValue;

        private long i_TerminationGraceDays             = 0;
        private long i_CancellationGraceDays            = 0;
        private long i_TerminationPurgeDays             = 0;
        private long i_CancellationPurgeDays            = 0;

        private Payor i_Payor                           = new Payor();
        private InsurancePlanCategory i_PlanCategory    = new InsurancePlanCategory();

        #endregion

        #region Constants
        #endregion
    }
}