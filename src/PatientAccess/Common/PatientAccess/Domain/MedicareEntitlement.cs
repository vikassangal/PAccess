using System;
using Extensions.PersistenceCommon;

namespace PatientAccess.Domain
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]

    public abstract class MedicareEntitlement : PersistentModel
    {
        #region Event Handlers
        #endregion

        #region Methods
        public abstract string Conclusion();

        public override bool Equals( object obj )
        {
            if( obj == null )
            {
                return false;
            }
            MedicareEntitlement me = obj as MedicareEntitlement;

            bool ghp = me.GroupHealthPlanCoverage.Equals( this.GroupHealthPlanCoverage );

            bool patient = (me.PatientEmployment == null && this.PatientEmployment == null ? true : 
                me.PatientEmployment == null && this.PatientEmployment != null ? false :
                me.PatientEmployment != null && this.PatientEmployment == null ? false :
                me.PatientEmployment.Equals( this.PatientEmployment ));

            bool spouse = (me.SpouseEmployment == null && this.SpouseEmployment == null ? true : 
                me.SpouseEmployment == null && this.SpouseEmployment != null ? false :
                me.SpouseEmployment != null && this.SpouseEmployment == null ? false :
                me.SpouseEmployment.Equals( this.SpouseEmployment ));
 
            bool result = (ghp && patient && spouse);
            return result;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

        #region Properties

        public Type EntitlementType
        {
            get
            {
                return i_EntitlementType;
            }
            set
            {
                i_EntitlementType = value;
            }
        }

        public YesNoFlag GroupHealthPlanCoverage 
        {
            get
            {
                return i_GroupHealthPlanCoverage;
            }
            set
            {
                i_GroupHealthPlanCoverage = value;
            }
        }

        public Employment PatientEmployment
        {
            get
            {
                return this.i_PatientEmployment;
            }
            set
            {
                this.i_PatientEmployment = value;
            }
        }

        public Employment SpouseEmployment
        {
            get
            {
                return this.i_SpouseEmployment;
            }
            set
            {
                this.i_SpouseEmployment = value;
            }
        }

        public GroupHealthPlanType GroupHealthPlanType
        {
            get
            {
                return i_GroupHealthPlanType;
            }
            set
            {
                i_GroupHealthPlanType = value;
            }
        }

        #endregion

        #region Private Methods
        protected void SetYesNoState( YesNoFlag newObj, YesNoFlag oldObj )
        {
            if( oldObj.Code.Equals( "Y" ) )
            {
                newObj.SetYes();
            }
            else if( oldObj.Code.Equals( "N" ) )
            {
                newObj.SetNo();
            }
            else if( oldObj.Code.Equals( " " ) )
            {
                newObj.SetBlank();
            }
        }
        #endregion

        #region Construction and Finalization
        public MedicareEntitlement()
        {
        }
        #endregion

        #region Data Elements
        
        private YesNoFlag  i_GroupHealthPlanCoverage        = new YesNoFlag();

        private GroupHealthPlanType i_GroupHealthPlanType   = new GroupHealthPlanType();

        private Employment i_SpouseEmployment               = new Employment();
        private Employment i_PatientEmployment              = new Employment();
        private Type i_EntitlementType                      = null;

        #endregion

        #region Constants
        #endregion
    }
}