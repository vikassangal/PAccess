using System;
using System.Collections;
using Extensions.PersistenceCommon;

namespace PatientAccess.Domain
{
    //	  [Serializable]
    ///	<summary>
    ///	
    ///	</summary>
    public class OccurrenceCodeManager : PersistentModel
    {
        #region	Event Handlers
        #endregion

        #region	Methods
        ///	<summary>
        ///	Generates <see cref="OccurrenceCode"/>	for	all	Condition types	(Illness,Pregnancy,Accident	and	Crime.
        ///	Gets the <see cref="OccurrenceCode"/> matching	the	<see cref="OccurrenceCode"/> for the ConditionType	
        ///	from the List of OccurrenceCodes	from <see cref="OccurrenceCodesLoader"/>.
        ///	Derives	a list of MutualOccurrenceCodes that	needs to be	removed	from the Account to	add	
        ///	the	 <see cref="OccurrenceCode"/> For this	Condition type.	and	removes	all	these OccurrenceCodes from Account Object
        ///	Finally	adds the new	Generated <see cref="OccurrenceCode"/>	to the Account object.	
        ///	</summary>
        ///	<param name="condition"></param>
        public void GenerateOccurrenceCode(Condition condition)
        {

            OccurrenceCode foundOC = null;
            string occ = condition.OccurrenceCodeStr;

            ArrayList list = (ArrayList)this.OccurrenceCodes;
            foreach (OccurrenceCode oc in list)
            {
                if (oc.Code == occ)
                {
                    foundOC = oc;
                    foundOC.OccurrenceDate = condition.OccurredOn;
                }
            }
            if (foundOC != null)
            {
                ArrayList removeCodes = this.CodesToRemove(condition);
                if (removeCodes != null)
                {
                    foreach (OccurrenceCode oc in removeCodes)
                    {
                        Account.RemoveOccurrenceCode(oc);
                    }
                }
                Account.AddOccurrenceCode(foundOC);

            }
            else
            {
                if (condition.GetType() == typeof(UnknownCondition))
                {
                    ArrayList removeCodes = this.CodesToRemove(condition);
                    if (removeCodes != null)
                    {
                        foreach (OccurrenceCode oc in removeCodes)
                        {
                            Account.RemoveOccurrenceCode(oc);
                        }
                    }
                }

            }
        }

        ///	<summary>
        ///	Generates List of MutualCodes that need	to be removed from the Account to
        ///	add	a new OccurrenceCode. For Example if	the	Account	Object already has a 
        ///	Accident.OccurrenceCode and Now,	We are adding a	 Crime.OccurrenceCode , 
        ///	The	Accident.OccurrenceCode needs to	be removed before adding the 
        ///	Crime.OccurrenceCode.
        ///	
        ///	</summary>
        ///	<param name="condition"></param>
        ///	<returns></returns>
        private ArrayList CodesToRemove(Condition condition)
        {
            ArrayList codesToRemove = new ArrayList();
            ArrayList list = (ArrayList)this.OccurrenceCodes;

            foreach (OccurrenceCode oc in list)
            {
                if (ConditionCodes.Contains(oc.Code))
                {
                    codesToRemove.Add(oc);
                }
            }

            return codesToRemove;

        }

        #endregion

        #region	Properties

        ///	<summary>
        ///	List of	OccurrenceCodes Loaded from the OccurrenceCodesLoader
        ///	</summary>
        private ICollection OccurrenceCodes
        {
            get
            {
                return (ICollection)this.OccurrenceCodesList.Clone();
            }

        }


        // TODO: Document
        ///	<summary>
        /// OccurrenceCodes Holder
        ///	</summary>
        public IValueLoader OccurrenceCodesLoader
        {

            set
            {
                this.i_OccurrenceCodesHolder = new ValueHolder(value);
            }
        }

        ///	<summary>
        ///	Account	object to which	Occurrence Codes	will be	added to. 
        ///	</summary>
        public Account Account
        {
            get
            {
                return i_Account;
            }
            set
            {
                i_Account = value;
            }
        }

        ///	<summary>
        ///	Singleton instance of this object. 
        ///	</summary>
        public static OccurrenceCodeManager Instance
        {
            get
            {
                if (i_Instance == null)
                {
                    i_Instance = new OccurrenceCodeManager();
                }
                return i_Instance;
            }
        }
        #endregion

        #region	Private	Methods

        #endregion

        #region	Private	Properties
        private static ArrayList ConditionCodes
        {
            get
            {
                if (i_ConditionCodes.Count != 0)
                {
                    i_ConditionCodes.Clear();
                }
                i_ConditionCodes.Add(OccurrenceCode.OCCURRENCECODE_ACCIDENT_AUTO);
                i_ConditionCodes.Add(OccurrenceCode.OCCURRENCECODE_ACCIDENT_AUTO_NO_FAULT);
                i_ConditionCodes.Add(OccurrenceCode.OCCURRENCECODE_ACCIDENT_EMPLOYER_REL);
                i_ConditionCodes.Add(OccurrenceCode.OCCURRENCECODE_ACCIDENT_OTHER);
                i_ConditionCodes.Add(OccurrenceCode.OCCURRENCECODE_ACCIDENT_TORT_LIABILITY);
                i_ConditionCodes.Add(OccurrenceCode.OCCURRENCECODE_CRIME);
                
                return i_ConditionCodes;

            }
        }

        private ArrayList OccurrenceCodesList
        {
            get
            {
                if (this.i_OccurrenceCodesHolder != null)
                {
                    this.i_OccurrenceCodes = this.i_OccurrenceCodesHolder.GetValue(this.Account.Facility) as ArrayList;
                }
                return this.i_OccurrenceCodes;
            }
            set
            {
                this.i_OccurrenceCodes = value;
            }
        }

        #endregion

        #region	Construction and Finalization

        public OccurrenceCodeManager()
        {
        }

        #endregion

        #region	Data Elements

        [NonSerialized()]
        private Account i_Account;
        private ValueHolder i_OccurrenceCodesHolder;
        private ArrayList i_OccurrenceCodes = new ArrayList();
        private static OccurrenceCodeManager i_Instance;
        private static ArrayList i_ConditionCodes = new ArrayList();


        #endregion
    }
}