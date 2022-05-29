using System;

namespace PatientAccess.Domain
{
    //TODO: Create XML summary comment for BenefitsCategory
    [Serializable]
    public class BenefitsCategory : ReferenceValue
    {
        #region Event Handlers
        #endregion

        #region Methods
        public override string ToString()
        {
            return Description.Trim();
        }
     
        public bool isInPatient()
        {
            return (this.Oid == INPATIENTOID );
        }
        public bool isOutPatient()
        {
            return (this.Oid   == OUTPATIENTOID );
        }
        public bool isOB()
        {
            return (this.Oid  == OBOID );
        }
        public bool isNewBorn()
        {
            return (this.Oid  == NEWBORNOID );
        }
        public bool isNICU()
        {
            return (this.Oid  == NICUOID );
        }
        public bool isPSYCHIP()
        {
            return (this.Oid  == PSYCH_IPOID );
        }
        public bool isPSYCHOP()
        {
            return (this.Oid  == PSYCH_OPOID );
        }
        public bool isCHEMDEP()
        {
            return (this.Oid  == CHEM_DEPOID );
        }
        public bool isSNFSubAcute()
        {
            return (this.Oid  == SNF_SUBACUTEOID );
        }
        public bool isREHABIP()
        {
            return (this.Oid  == REHAB_IPOID );
        }
        public bool isREHABOP()
        {
            return (this.Oid  == REHAB_OPOID );
        }
        #endregion

        #region Properties
  
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public BenefitsCategory() { }

        public BenefitsCategory( long oid, DateTime version, string description ) : 
            base( oid, version, description ){}

        #endregion

        #region Data Elements
        #endregion

        #region Constants

        public const long INPATIENTOID      =	1;
        public const long OUTPATIENTOID     =	2;
        public const long OBOID             =	3;
        public const long NEWBORNOID        =	4;
        public const long NICUOID           =	5;
        public const long PSYCH_IPOID       =	6;
        public const long PSYCH_OPOID       =	7;
        public const long CHEM_DEPOID       =	8;
        public const long SNF_SUBACUTEOID   =	9;
        public const long REHAB_IPOID       =	10;
        public const long REHAB_OPOID       =	11;
        public const long GENERAL_OID       =   12;

        #endregion
    }
}
