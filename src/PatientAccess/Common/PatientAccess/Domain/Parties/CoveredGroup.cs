using System;
using System.Collections;
using Extensions.PersistenceCommon;

namespace PatientAccess.Domain.Parties
{
    [Serializable]
    public class CoveredGroup : PersistentModel
    {
        #region Event Handlers
        #endregion

        #region Methods

        public   void AddContactPoint(ContactPoint aContactPoint)
        {
            if( !this.primContactPoints.Contains( aContactPoint ) )
            {
                this.i_ContactPoints.Add( aContactPoint );
            }
        }
        #endregion

        #region Properties
        public IEmployer Employer
        {
            get
            {
                return i_Employer;
            }
            set
            {
                i_Employer = value;
            }
        }
        public IValueLoader PlansLoader
        {
            set
            {
                this.i_PlansHolder = new ValueHolder( value );
            }
        }
        public virtual ICollection InsurancePlans
        {
            get
            {
                return (ICollection)this.PrimInsurancePlans.Clone();
            }
        }
     
        public ICollection ContactPoints
        {
            get
            {
                return (ICollection)primContactPoints.Clone();
            }
        }
        public string Name
        {
            get
            {
                return i_Name;
            }
            set
            {
                i_Name = value;
            }
        }
   
        #endregion

        #region Private Methods
     
        #endregion

        #region Private Properties

        private ArrayList PrimInsurancePlans
        {
            get
            {
                if( this.i_PlansHolder != null )
                {
                    i_InsurancePlans = this.i_PlansHolder.GetValue() as ArrayList;
                }
                return i_InsurancePlans;
            }
            set
            {
                i_InsurancePlans = value;
            }
        }

        private ArrayList primContactPoints
        {
            get
            {
                if( i_ContactPoints == null )
                {
                    i_ContactPoints = new ArrayList();
                }
                return i_ContactPoints;
            }
            set
            {
                i_ContactPoints = value;
            }
        }
        #endregion

        #region Construction and Finalization
        public CoveredGroup() : base()
        {
        }
     
        #endregion

        #region Data Elements
        private string i_Name;

        private ArrayList    i_ContactPoints   = new ArrayList();
        private ArrayList i_InsurancePlans = new ArrayList();
        private ValueHolder i_PlansHolder ;
        private IEmployer i_Employer ;
      
        #endregion

        #region Constants
        #endregion
    }
}