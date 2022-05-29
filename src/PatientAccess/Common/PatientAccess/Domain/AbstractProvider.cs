using System;
using System.Collections;
using System.Reflection;
using Extensions.PersistenceCommon;

namespace PatientAccess.Domain
{
    [Serializable]
    public abstract class AbstractProvider : PersistentModel
    {
        #region Event Handlers
        #endregion

        #region Methods
        public void AddInsurancePlan( InsurancePlan anInsurancePlan )
        {
            if( !this.PrimInsurancePlans.Contains( anInsurancePlan ) )
            {
                this.PrimInsurancePlans.Add( anInsurancePlan );
            }
        }

        public void RemoveInsurancePlan( InsurancePlan anInsurancePlan )
        {
            if( this.PrimInsurancePlans.Contains( anInsurancePlan ) )
            {
                this.PrimInsurancePlans.Remove( anInsurancePlan );
            }
        }
        #endregion

        #region Properties

        public string Name
        {
            get
            {
                return i_Name;
            }
            set
            {
                this.SetAndTrack<string>( ref this.i_Name, value, MethodBase.GetCurrentMethod() );
                i_Name = value;
            }
        }

        public string Code
        {
            get
            {
                return i_Code;
            }
            set
            {
                i_Code = value;
            }
        }

        public virtual ICollection InsurancePlans
        {
            get
            {
                return (ICollection)this.PrimInsurancePlans.Clone();
            }
        }

        public string NumberOfActivePlans
        {
            get
            {
                  return i_NumberOfActivePlans;
            }
            set
            {
                this.i_NumberOfActivePlans = value;
            }
        }

        public IValueLoader PlansLoader
        {
            set
            {
                this.i_PlansHolder = new ValueHolder( value );
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
        #endregion

        #region Construction and Finalization
        public AbstractProvider()
        {
        }
        #endregion

        #region Data Elements
        private ArrayList i_InsurancePlans = new ArrayList();
        private ValueHolder i_PlansHolder;
        private string i_Name = String.Empty;
        private string i_Code = String.Empty;
        private string i_NumberOfActivePlans = "0";
        #endregion

        #region Constants
        #endregion
    }
}

