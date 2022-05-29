using System;
using System.Collections;
using Extensions.PersistenceCommon;

namespace PatientAccess.Domain
{
	/// <summary>
	/// Summary description for CodedDiagnoses.
	/// </summary>
	//TODO: Create XML summary comment for CodedDiagnoses
    [Serializable]
    public class CodedDiagnoses : PersistentModel
    {
        #region Event Handlers
        #endregion

        #region Methods
        #endregion

        #region Properties
        public ArrayList CodedDiagnosises
        {
            get
            {
                return i_CodedDiagnosises;
            }
            set
            {
                i_CodedDiagnosises = value;
            }
        }
        public ArrayList AdmittingCodedDiagnosises
        {
            get
            {
                return i_AdmittingCodedDiagnosises;
            }
            set
            {
                i_AdmittingCodedDiagnosises = value;
            }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public CodedDiagnoses()
        {
        }
        #endregion

        #region Data Elements
        private ArrayList               i_CodedDiagnosises = new ArrayList();
        private ArrayList               i_AdmittingCodedDiagnosises = new ArrayList();
        #endregion

        #region Constants
        #endregion
    }
}
