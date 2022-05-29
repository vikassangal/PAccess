using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Extensions.DB2Persistence;
using Extensions.PersistenceCommon;
using IBM.Data.DB2.iSeries;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Persistence.Utilities;
using PatientAccess.Utilities;
using log4net;

namespace PatientAccess.Persistence
{
    /// <summary>
    /// Summary description for EmployerPbarBroker.
    /// </summary>
    //TODO: Create XML summary comment for EmployerPbarBroker
    [Serializable]
    public class EmployerPbarBroker : PBARCodesBroker, IEmployerBroker
    {
        #region Event Handlers
        #endregion

        #region Methods

        /// <summary>
        /// get employer given employer name
        /// </summary>
        /// <param name="facCode"> A string, facility code</param>
        /// <param name="empName"></param>
        /// <returns></returns>
        public Employer SelectEmployerByName(string facCode, string empName)
        {
            Employer emp = new Employer();
            iDB2Command cmd = null;
            SafeReader reader = null;

            try
            {
                IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
                Facility fac = facilityBroker.FacilityWith(facCode);

                string strFollowUpUnit = fac.FollowupUnit.AsFormattedString();

                cmd = this.CommandFor(
                    String.Format("CALL {0}({1},{2})",
                    SELECTEMPLOYERBYNAME,
                    PARAM_P_FUUN,
                    PARAM_P_EMNAME),
                    CommandType.Text,
                    fac);

                cmd.Parameters[PARAM_P_FUUN].Value = strFollowUpUnit;
                cmd.Parameters[PARAM_P_EMNAME].Value = empName;

                reader = this.ExecuteReader(cmd);

                if (reader.Read())
                {
                    emp.EmployerCode = Convert.ToInt64(reader.GetString(COL_EMPLOYERCODE).TrimEnd());
                    emp.Name =
                        StringFilter.RemoveFirstCharNonLetterNonNumberAndRestNonLetterNumberSpaceAndHyphen(
                        reader.GetString( COL_EMPLOYERNAME ) );
                    emp.Name = Employer.ModifyPBAREmployerName( emp.Name );
                }
            }
            catch (Exception ex)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom(ex, c_log);
            }
            finally
            {
                base.Close(reader);
                base.Close(cmd);
            }

            return emp;
        }

        public int AddNewEmployer(Employer emp, FollowupUnit followUpUnit, string facilityCode)
        {

            User currentUser = User.GetCurrent();
            string userName = PACCESS_USER;

            if (currentUser != null)
            {
                userName = currentUser.PBAREmployeeID ?? PACCESS_USER;
            }

            return this.AddNewEmployer(emp, followUpUnit, facilityCode, userName);

        }

        /// <summary>
        /// add new employer to db2
        /// </summary>
        /// <param name="emp"></param>
        /// <param name="followUpUnit"></param>
        /// <param name="facilityCode"> A string, facility code</param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public int AddNewEmployer(Employer emp, FollowupUnit followUpUnit, string facilityCode, string userName)
        {
            iDB2Command cmd = null;

            int nextEmpNumber = -1;

            try
            {
                IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
                Facility facility = facilityBroker.FacilityWith(facilityCode);

                string areaName = ConfigurationManager.AppSettings[EMPLOYER_CODE_DATA_AREA_NAME];
                string libName = ConfigurationManager.AppSettings[EMPLOYER_CODE_LIB_NAME];
                IBMUtilities util = new IBMUtilities(facility);
                nextEmpNumber = util.GetNextEmployerNumber(areaName, libName);

                // TODO: Adjust this date for the time zone
                DateTime dt = DateTime.Now;
                int packedDate = DateTimeUtilities.PackedDateFromDate(dt);
                c_log.InfoFormat("Inserting employerAdd Name:{0} FUUN:(1) CODE:{2}",
                    emp.Name, followUpUnit.Oid.ToString(), nextEmpNumber);

                c_log.InfoFormat("Inserting employerAdd Name:{0} FUUN:(1) CODE:{2}",
                    emp.Name, followUpUnit.AsFormattedString(), nextEmpNumber);
                cmd = this.CommandFor("CALL " + INSERTNEWEMPLOYER +
                    "(" + PARAM_P_EMFUUN +
                    "," + PARAM_P_EMNAME +
                    "," + PARAM_P_EMURFG +
                    "," + PARAM_P_EMADDT +
                    "," + PARAM_P_EMLMDT +
                    "," + PARAM_P_EMDLDT +
                    "," + PARAM_P_EMCODE +
                    "," + PARAM_P_EMACNT +
                    "," + PARAM_P_EMUSER +
                    "," + PARAM_P_EMNEID + ")",
                    CommandType.Text,
                    facility);

                cmd.Parameters[PARAM_P_EMFUUN].Value = followUpUnit.AsFormattedString();
                cmd.Parameters[PARAM_P_EMNAME].Value = Employer.ModifyPBAREmployerName(emp.Name);
                cmd.Parameters[PARAM_P_EMURFG].Value = " ";
                cmd.Parameters[PARAM_P_EMADDT].Value = packedDate;
                cmd.Parameters[PARAM_P_EMLMDT].Value = packedDate;
                cmd.Parameters[PARAM_P_EMDLDT].Value = 0;
                cmd.Parameters[PARAM_P_EMCODE].Value = nextEmpNumber;
                cmd.Parameters[PARAM_P_EMACNT].Value = 0;
                cmd.Parameters[PARAM_P_EMUSER].Value = userName ?? PACCESS_USER;
                if (emp.NationalId == null)
                {
                    cmd.Parameters[PARAM_P_EMNEID].Value = " ";
                }
                else
                {
                    cmd.Parameters[PARAM_P_EMNEID].Value = emp.NationalId;
                }

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom("Unhandled exception", ex, c_log);
            }
            finally
            {
                base.Close(cmd);
            }
            return nextEmpNumber;
        }

        /// <summary>
        /// Select all employers that match a name starting with the search Name
        /// </summary>
        /// <param name="facilityOid"></param>
        /// <param name="searchName"></param>
        /// <returns></returns>
        public SortedList AllEmployersWith(long facilityOid, string searchName)
        {
            InitFacility(facilityOid);

            iDB2Command cmd = null;
            SortedList employers = new SortedList();
            SafeReader reader = null;

            try
            {
                string strFollowUpUnit = this.Facility.FollowupUnit.AsFormattedString();

                cmd = this.CommandFor(
                    String.Format("CALL {0}({1},{2})",
                    SELECTEMPLOYERBYNAME,
                    PARAM_P_FUUN,
                    PARAM_P_EMNAME),
                    CommandType.Text,
                    this.Facility);

                cmd.Parameters[PARAM_P_FUUN].Value = strFollowUpUnit;
                cmd.Parameters[PARAM_P_EMNAME].Value = searchName + "%";

                reader = this.ExecuteReader(cmd);

                EmployerProxy employerProxy = null;
                while (reader.Read())
                {
                    employerProxy = this.EmployerFrom(reader, this.Facility);
                    var employerKey = CreateEmployerKey(employerProxy);
                    if ( !employers.Contains( employerKey ) )
                    {
                        employers.Add( employerKey , employerProxy );
                    }
                }
            }
            catch (Exception ex)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom("Unhandled Exception", ex, c_log);
            }
            finally
            {
                base.Close(reader);
                base.Close(cmd);
            }
            return employers;
        }

        /// <summary>
        /// Fill in the employment/employer information for a guarantor.
        /// </summary>
        /// <param name="aGuarantor"></param>
        /// <param name="accountNumber"></param>
        /// <param name="facilityOid"></param>
        /// <returns></returns>      
        public Guarantor EmployerFor(Guarantor aGuarantor, long accountNumber, long facilityOid)
        {
            Guarantor guarantor = aGuarantor;

            InitFacility(facilityOid);

            iDB2Command cmd = null;
            SortedList employers = new SortedList();
            SafeReader reader = null;
            ArrayList employerContactPoints = new ArrayList();

            try
            {
                cmd = this.CommandFor("CALL " + SELECTEMPLOYERFORGUARANTOR +
                    "(" + PARAM_ACCOUNTNUMBER +
                    "," + PARAM_FACILITYID +
                    ")",
                    CommandType.Text,
                    this.Facility);

                cmd.Parameters[PARAM_ACCOUNTNUMBER].Value = accountNumber;
                cmd.Parameters[PARAM_FACILITYID].Value = facilityOid;

                reader = this.ExecuteReader(cmd);

                while (reader.Read())
                {
                    string empName =
                        StringFilter.RemoveFirstCharNonLetterNonNumberAndRestNonLetterNumberSpaceAndHyphen(
                        reader.GetString( COL_EMPLOYERNAME ) );
                    empName = Employer.ModifyPBAREmployerName( empName );
                    string employmentStatusCode = reader.GetString(COL_EMPLOYMENTSTATUSCODE);
                    
                    string employeeID = reader.GetString(COL_EMPLOYEEID).TrimEnd();
                    employeeID = StringFilter.RemoveFirstCharNonLetterNonNumberAndRestNonLetterAndNumber( employeeID );

                    string occupation = reader.GetString(COL_OCCUPATION).TrimEnd();

                    Employer employer = new Employer(0, DateTime.Now, empName, string.Empty, 0);

                    EmployerContactPoint ecp = this.EmployerContactPointFrom( reader, facilityOid );
                    employer.AddContactPoint(ecp);

                    guarantor.Employment = new Employment(guarantor);

                    IEmploymentStatusBroker esb = BrokerFactory.BrokerOfType<IEmploymentStatusBroker>();
                    EmploymentStatus es = esb.EmploymentStatusWith(facilityOid, employmentStatusCode);
                    if (es == null)
                    {
                        es = esb.EmploymentStatusWith(facilityOid, " ");
                    }
                    guarantor.Employment.Status = es;
                    guarantor.Employment.EmployeeID = employeeID;
                    guarantor.Employment.Occupation = occupation;

                    guarantor.Employment.Employer = employer;
                    ContactPoint cp = guarantor.Employment.Employer.ContactPointWith(TypeOfContactPoint.NewEmployerContactPointType());
                    guarantor.Employment.Employer.PartyContactPoint = cp;

                }
            }
            catch (Exception ex)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom("Unhandled Exception", ex);
            }
            finally
            {
                base.Close(reader);
                base.Close(cmd);
            }

            return guarantor;
        }

        /// <summary>
        /// Fill in the employment/employer information for a patient.
        /// </summary>
        /// <param name="aPatient"></param>
        /// <param name="facilityOid"></param>
        /// <returns></returns>
        public Patient EmployerFor(Patient aPatient, long facilityOid)
        {
            Patient patient = aPatient;

            InitFacility(facilityOid);

            iDB2Command cmd = null;
            SortedList employers = new SortedList();
            SafeReader reader = null;
            ArrayList employerContactPoints = new ArrayList();

            try
            {
                cmd = this.CommandFor("CALL " + SELECTEMPLOYERFORPATIENT +
                    "(" + PARAM_MRCNUMBER +
                    "," + PARAM_FACILITYID +
                    ")",
                    CommandType.Text,
                    this.Facility);

                cmd.Parameters[PARAM_MRCNUMBER].Value = patient.MedicalRecordNumber;
                cmd.Parameters[PARAM_FACILITYID].Value = facilityOid;

                reader = this.ExecuteReader(cmd);

                while (reader.Read())
                {
                    long fuuID = this.Facility.FollowupUnit.Oid;
                    string empName =
                        StringFilter.RemoveFirstCharNonLetterNonNumberAndRestNonLetterNumberSpaceAndHyphen(
                        reader.GetString( COL_EMPLOYERNAME ) );
                    empName = Employer.ModifyPBAREmployerName( empName );
                    string employmentStatusCode = reader.GetString(COL_EMPLOYMENTSTATUSCODE);

                    string employeeID = reader.GetString(COL_EMPLOYEEID).TrimEnd();
                    employeeID = StringFilter.RemoveFirstCharNonLetterNonNumberAndRestNonLetterAndNumber( employeeID );

                    string occupation = reader.GetString( COL_OCCUPATION ).TrimEnd();

                    Employer employer = new Employer(0, DateTime.Now, empName, string.Empty, 0);
                    EmployerContactPoint ecp = this.EmployerContactPointFrom( reader, facilityOid );
                    employer.AddContactPoint(ecp);

                    patient.Employment = new Employment(patient);

                    IEmploymentStatusBroker esb = BrokerFactory.BrokerOfType<IEmploymentStatusBroker>();
                    EmploymentStatus es = esb.EmploymentStatusWith(facilityOid, employmentStatusCode);
                    if (es == null)
                    {
                        es = esb.EmploymentStatusWith(facilityOid, " ");
                    }
                    patient.Employment.Status = es;
                    patient.Employment.EmployeeID = employeeID;
                    patient.Employment.Occupation = occupation;

                    patient.Employment.Employer = employer;
                    ContactPoint cp = patient.Employment.Employer.ContactPointWith(TypeOfContactPoint.NewEmployerContactPointType());
                    patient.Employment.Employer.PartyContactPoint = cp;

                }
            }
            catch (Exception ex)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom("Unhandled Exception", ex);
            }
            finally
            {
                base.Close(reader);
                base.Close(cmd);
            }

            return patient;
        }

        /// <summary>
        /// get employer given employer proxy
        /// </summary>
        /// <param name="employerProxy"></param>
        /// <returns></returns>
        public Employer EmployerFor(EmployerProxy employerProxy)
        {
            ArrayList contactpoints = null;

            IAddressBroker addressBroker = BrokerFactory.BrokerOfType<IAddressBroker>();

            contactpoints = addressBroker.ContactPointsForEmployer(employerProxy.Facility.Code, employerProxy.EmployerCode);

            Employer employer = new Employer(employerProxy.Oid,
                employerProxy.Timestamp,
                employerProxy.Name,
                employerProxy.NationalId,
                employerProxy.EmployerCode);

            foreach (EmployerContactPoint ecp in contactpoints)
            {
                employer.AddContactPoint(ecp);
            }

            return employer;
        }

        /// <summary>
        /// Get employer from approval table given employer proxy
        /// </summary>
        /// <param name="employerProxy"></param>
        /// <returns></returns>
        private Employer EmployerForApproval(EmployerProxy employerProxy)
        {
            ArrayList contactpoints = null;

            IAddressBroker addressBroker = BrokerFactory.BrokerOfType<IAddressBroker>();
            //contactpoints = addressBroker.ContactPointsForEmployer(employerProxy.Facility, employerProxy.EmployerCode);
            contactpoints = addressBroker.ContactPointsForEmployerApproval(employerProxy.Facility.Code, employerProxy.EmployerCode);

            Employer employer = new Employer(employerProxy.Oid,
                employerProxy.Timestamp,
                employerProxy.Name,
                employerProxy.NationalId,
                employerProxy.EmployerCode);

            foreach (EmployerContactPoint ecp in contactpoints)
            {
                employer.AddContactPoint(ecp);
            }

            return employer;
        }



        public void AddContactPointForEmployer(Employer employer, ContactPoint contactPoint, string facilityCode)
        {
            IAddressBroker broker = BrokerFactory.BrokerOfType<IAddressBroker>();
            employer.PartyContactPoint = contactPoint;
            broker.SaveEmployerAddress(employer, facilityCode);
        }


        /// <exception cref="Exception">Unhandled Exception</exception>
        public SortedList<string, NewEmployerEntry> GetAllEmployersForApproval(string facilityHspCode)
        {
            this.InitFacility(facilityHspCode);

            iDB2Command cmd = null;
            SortedList<string, NewEmployerEntry> employers = new SortedList<string, NewEmployerEntry>();
            SafeReader reader = null;
            IEmployerBroker empBroker = BrokerFactory.BrokerOfType<IEmployerBroker>();

            try
            {
                string strFollowUpUnit = this.Facility.FollowupUnit.AsFormattedString();

                cmd = this.CommandFor(
                    String.Format("CALL {0}({1})",
                                  NEW_GETALLEMPLOYERS,
                                  NEW_PARAM_P_FUUN),
                    CommandType.Text,
                    this.Facility);

                cmd.Parameters[NEW_PARAM_P_FUUN].Value = strFollowUpUnit;

                reader = this.ExecuteReader(cmd);

                while (reader.Read())
                {

                    NewEmployerEntry employerEntry = this.NewEmployerEntryFrom(reader, this.Facility);
                    employers.Add(NewCreateEmployerKey(employerEntry.Employer), employerEntry);
                }
            }
            catch (Exception ex)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom("Unhandled Exception", ex, newc_log);
            }
            finally
            {
                this.Close(reader);
                this.Close(cmd);
            }
            return employers;
        }


        public SortedList<string, Employer> GetAllEmployersWith(long facilityOid, string searchName)
        {
            SortedList<string, Employer> result = new SortedList<string, Employer>();
            SortedList employerProxies = AllEmployersWith(facilityOid, searchName);

            foreach (DictionaryEntry proxyEntry in employerProxies)
            {
                Employer employer = this.EmployerFor((EmployerProxy)proxyEntry.Value);
                result.Add((string)proxyEntry.Key, employer);
            }
            return result;
        }

        public void DeleteAllEmployersForApproval(string facilityCode)
        {
            SortedList<string, NewEmployerEntry> newEmployerEntries = GetAllEmployersForApproval(facilityCode);
            foreach (NewEmployerEntry employerEntry in newEmployerEntries.Values)
            {
                DeleteEmployerForApproval(employerEntry.Employer, facilityCode);
            }
        }


        public void SaveEmployersForApproval(IList<NewEmployerEntry> newEmployerEntries, string facilityCode)
        {
            IAddressBroker addressBroker = BrokerFactory.BrokerOfType<IAddressBroker>();
            foreach (NewEmployerEntry employerEntry in newEmployerEntries)
            {
                employerEntry.Employer.EmployerCode= AddEmployerForApproval(employerEntry.Employer, facilityCode, employerEntry.UserID);
                addressBroker.SaveNewEmployerAddressForApproval(employerEntry.Employer,facilityCode);
            }
        }

        public int AddEmployerForApproval(Employer emp, string facilityCode, string usrID)
        {
            iDB2Command cmd = null;
            int nextEmpNumber = -1;
            int autoGeneratedID = -1;

            try
            {
                IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
                Facility facility = facilityBroker.FacilityWith(facilityCode);

                FollowupUnit followUpUnit = facility.FollowupUnit;

                string areaName = ConfigurationManager.AppSettings[NEW_EMPLOYER_CODE_DATA_AREA_NAME];
                string libName = ConfigurationManager.AppSettings[NEW_EMPLOYER_CODE_LIB_NAME];
                IBMUtilities util = new IBMUtilities(facility);
                nextEmpNumber = util.GetNextEmployerNumber(areaName, libName);

                DateTime dt = DateTime.Now;
                int packedDate = DateTimeUtilities.PackedDateFromDate(dt);
                newc_log.InfoFormat("Inserting EmployerForApproval Name:{0} FUUN:(1) CODE:{2}",
                                    emp.Name, followUpUnit.Oid.ToString(), nextEmpNumber);

                newc_log.InfoFormat("Inserting EmployerForApproval Name:{0} FUUN:(1) CODE:{2}",
                                    emp.Name, followUpUnit.AsFormattedString(), nextEmpNumber);
                cmd = this.CommandFor(
                    "CALL " + NEW_INSERTEMPLOYERFORAPPROVAL +
                    "(" + NEW_PARAM_P_EMFUUN +
                    "," + NEW_PARAM_P_EMNAME +
                    "," + NEW_PARAM_P_EMURFG +
                    "," + NEW_PARAM_P_EMADDT +
                    "," + NEW_PARAM_P_EMLMDT +
                    "," + NEW_PARAM_P_EMDLDT +
                    "," + NEW_PARAM_P_EMCODE +
                    "," + NEW_PARAM_P_EMACNT +
                    "," + NEW_PARAM_P_EMUSER +
                    "," + NEW_PARAM_P_EMNEID +
                    "," + NEW_OUT_PARAM_ID + ")",
                    CommandType.Text,
                    facility);

                cmd.Parameters[NEW_PARAM_P_EMFUUN].Value = followUpUnit.AsFormattedString();
                cmd.Parameters[NEW_PARAM_P_EMNAME].Value = Employer.ModifyPBAREmployerName(emp.Name);
                cmd.Parameters[NEW_PARAM_P_EMURFG].Value = " ";
                cmd.Parameters[NEW_PARAM_P_EMADDT].Value = packedDate;
                cmd.Parameters[NEW_PARAM_P_EMLMDT].Value = packedDate;
                cmd.Parameters[NEW_PARAM_P_EMDLDT].Value = 0;
                cmd.Parameters[NEW_PARAM_P_EMCODE].Value = nextEmpNumber;
                cmd.Parameters[NEW_PARAM_P_EMACNT].Value = 0;
                cmd.Parameters[NEW_PARAM_P_EMUSER].Value = usrID;
                cmd.Parameters[NEW_OUT_PARAM_ID].Direction = ParameterDirection.Output;
                if (emp.NationalId == null)
                {
                    cmd.Parameters[NEW_PARAM_P_EMNEID].Value = " ";
                }
                else
                {
                    cmd.Parameters[NEW_PARAM_P_EMNEID].Value = emp.NationalId;
                }

                cmd.ExecuteNonQuery();

                if (cmd.Parameters[NEW_OUT_PARAM_ID].Value != DBNull.Value)
                {
                    autoGeneratedID = Convert.ToInt32(cmd.Parameters[NEW_OUT_PARAM_ID].Value);
                }
            }
            catch (Exception ex)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom("Unhandled exception", ex, newc_log);
            }
            finally
            {
                this.Close(cmd);
            }
            emp.Oid = nextEmpNumber;
            return nextEmpNumber;//  autoGeneratedID;
        }

        public void DeleteEmployerForApproval(Employer employer, string facilityHspCode)
        {
            IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            Facility facility = facilityBroker.FacilityWith(facilityHspCode);

            this.InitFacility(facility.Code);

            iDB2Command cmd = null;
            SafeReader reader = null;

            try
            {
                cmd = this.CommandFor(
                    String.Format("CALL {0}({1})",
                                  NEW_DELETEEMPLOYERFORAPPROVAL,
                                  NEW_PARAM_P_ID),
                    CommandType.Text,
                    this.Facility);

                cmd.Parameters[NEW_PARAM_P_ID].Value = employer.Oid;

                reader = this.ExecuteReader(cmd);
            }
            catch (Exception ex)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom("Unhandled Exception", ex, newc_log);
            }
            finally
            {
                this.Close(reader);
                this.Close(cmd);
            }
        }

        #endregion

        #region Properties
        #endregion

        #region Private Methods

        private NewEmployerEntry NewEmployerEntryFrom(SafeReader reader, Facility facility)
        {
            string addedByUserID = String.Empty;
            EmployerProxy employerProxy = null;

            if (!reader.IsDBNull(NEW_COL_EMPLOYERNAME))
            {
                long oid = 0;
                string name =
                    StringFilter.RemoveFirstCharNonLetterNonNumberAndRestNonLetterNumberSpaceAndHyphen(
                    reader.GetString( NEW_COL_EMPLOYERNAME ) );
                name = Employer.ModifyPBAREmployerName( name );
                string nationalId = reader.GetString(NEW_COL_NATIONALID);
                long employerCode = reader.GetInt64(NEW_COL_EMPLOYERCODE);
                oid = employerCode;
                addedByUserID = reader.GetString(NEW_COL_USERID);
                employerProxy = new EmployerProxy(oid, ReferenceValue.NEW_VERSION, name, nationalId, employerCode, facility);
            }
            //Employer emp = this.EmployerFor(employerProxy);
            Employer emp = this.EmployerForApproval(employerProxy);
            return new NewEmployerEntry(emp, addedByUserID);
        }


        private static string NewCreateEmployerKey(Employer ep)
        {
            if (ep == null)
            {
                throw new ArgumentNullException("ep");
            }
            return ep.Name + ep.EmployerCode;
        }

        /// <summary>
        /// get employer key by combining employer name and employer code
        /// </summary>
        /// <param name="ep"></param>
        /// <returns></returns>
        private string CreateEmployerKey(EmployerProxy ep)
        {
            return ep.Name + ep.EmployerCode;
        }

        /// <summary>
        /// Given a reader return a new EmployerProxy object
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="facility"></param>
        /// <returns></returns>
        private EmployerProxy EmployerFrom(SafeReader reader, Facility facility)
        {
            EmployerProxy employerProxy = null;

            if (!reader.IsDBNull(COL_EMPLOYERNAME))
            {
                long oid = 0;
                string name =
                    StringFilter.RemoveFirstCharNonLetterNonNumberAndRestNonLetterNumberSpaceAndHyphen(
                    reader.GetString( COL_EMPLOYERNAME ) );
                name = Employer.ModifyPBAREmployerName( name );
                string nationalId = reader.GetString(COL_NATIONALID);
                long employerCode = reader.GetInt64(COL_EMPLOYERCODE);

                employerProxy = new EmployerProxy(oid, PersistentModel.NEW_VERSION, name, nationalId, employerCode, facility);
            }

            return employerProxy;
        }

        /// <summary>
        /// EmployerContactPointFrom - create an EmployerContactPoint instance from the result set
        /// </summary>
        /// <param name="reader">a reader</param>
        /// <param name="facilityID"></param>
        /// <returns>an EmployerContactPoint instance</returns>
        private EmployerContactPoint EmployerContactPointFrom(SafeReader reader, long facilityID)
        {
            Address address = this.AddressFrom( reader, facilityID );
            PhoneNumber pn = this.PhoneNumberFrom(reader);
            EmailAddress em = this.EmailAddressFrom(reader);
            TypeOfContactPoint cptype = this.TypeOfContactPointFrom(reader);
            EmployerContactPoint ecp = new EmployerContactPoint(address, pn, em, cptype);

            return ecp;
        }

        /// <summary>
        /// AddressFrom - construct an Address from the result set
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="facilityID"></param>
        /// <returns></returns>
        private Address AddressFrom(SafeReader reader, long facilityID)
        {
            Address aAddress = null;
            IAddressBroker addressBroker = BrokerFactory.BrokerOfType<IAddressBroker>();

            // if there is no data return a null

            string address1 = reader.GetString(COL_ADDRESS1);
            address1 = StringFilter.
                RemoveFirstCharNonLetterNonNumberAndRestNonLetterNumberSpaceHyphenPeriodAndForwardSlash( address1 );

            string address2 = reader.GetString( COL_ADDRESS2 );
            address2 = StringFilter.
                RemoveFirstCharNonLetterNonNumberAndRestNonLetterNumberSpaceHyphenPeriodAndForwardSlash( address2 );

            string city = reader.GetString( COL_CITY );
            city = StringFilter.RemoveFirstCharNonLetterAndRestNonLetterSpaceHyphenAndPeriod( city );

            string postalCode = reader.GetString( COL_POSTALCODE );
            postalCode = StringFilter.RemoveAllNonLetterNumberSpaceAndHyphen( postalCode );

            Country country = null;
            County county = null;
            State state = null;

            if (!reader.IsDBNull(COL_STATECODE))
            {
                string stateCode = reader.GetString(COL_STATECODE);
                state = addressBroker.StateWith(facilityID,stateCode);
            }

            country = null;
            if (!reader.IsDBNull(COL_COUNTRYCODE))
            {
                string countryCode = reader.GetString(COL_COUNTRYCODE);
                country = addressBroker.CountryWith( facilityID, countryCode );
            }

            // According to Kevin, county is not used
            county = null;

            aAddress = new Address(ReferenceValue.NEW_OID,
                StringFilter.RemoveHL7Chars(address1),
                StringFilter.RemoveHL7Chars(address2),
                StringFilter.RemoveHL7Chars(city),
                new ZipCode(postalCode),
                state,
                country,
                county
                );
            return aAddress;
        }

        /// <summary>
        /// Private method to obtain the employer's phone number from 
        /// stored procs that return the area code and 7 digit phone number
        /// as separate numbers not a single large number to be formatted 
        /// on the broker side.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private PhoneNumber PhoneNumberFrom(SafeReader reader)
        {
            string countryCode = reader.GetString(COL_PHONECOUNTRYCODE);
            string phoneNumber = string.Empty;
            string areaCode = string.Empty;
            string areaCodeAndPhone = string.Empty;

            areaCode = reader.GetString(COL_PHONEAREACODE);
            areaCode = areaCode.PadLeft(AREA_CODE_LENGTH, '0');
            phoneNumber = reader.GetString(COL_PHONENUMBER);
            phoneNumber = phoneNumber.PadLeft(MAX_LENGTH_OF_PHONE_NUMBER_NO_AREA_CODE, '0');
            PhoneNumber pn = new PhoneNumber(countryCode, areaCode, phoneNumber);
            return pn;
        }

        /// <summary>
        /// EmailAddressFrom - create an EmailAddress instance from the result set
        /// </summary>
        /// <param name="reader">a reader</param>
        /// <returns>an EmailAddress instance</returns>
        private EmailAddress EmailAddressFrom(SafeReader reader)
        {
            string email = StringFilter.RemoveAllNonEmailSpecialCharacters( reader.GetString( COL_EMAILADDRESS ) );
            EmailAddress em = new EmailAddress( email );
            return em;
        }

        /// <summary>
        /// TypeOfContactPointFrom - create a TypeOfContactPoint instance from the result set
        /// </summary>
        /// <param name="reader">a reader</param>
        /// <returns>a TypeOfContactPoint isntance</returns>
        private TypeOfContactPoint TypeOfContactPointFrom(SafeReader reader)
        {
            long cpID = reader.GetInt64(COL_TYPEOFCONTACTPOINTID);
            string cpDesc = reader.GetString(COL_CONTACTPOINTDESCRIPTION);
            TypeOfContactPoint cpType = new TypeOfContactPoint(cpID, cpDesc);

            return cpType;
        }

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public EmployerPbarBroker()
            : base()
        {
        }
        public EmployerPbarBroker(string cxn)
            : base(cxn)
        {
        }
        public EmployerPbarBroker(IDbTransaction txn)
            : base(txn)
        {
        }
        #endregion

        #region Data Elements
        private static readonly ILog c_log =
            LogManager.GetLogger(typeof(EmployerPbarBroker));

        private static readonly ILog newc_log =
            LogManager.GetLogger(typeof(EmployerPbarBroker));

        #endregion

        #region Constants

        private const int
             AREA_CODE_LENGTH = 3,
             LENGTH_OF_FULL_PHONE_NUMBER = 10,
             MAX_LENGTH_OF_PHONE_NUMBER_NO_AREA_CODE = 7;

        private const char
             PADDING_SYMBOL = '0';

        private const string
             INSERTNEWEMPLOYER = "INSERTNEWEMPLOYER",
             SELECTEMPLOYERBYNAME = "SELECTEMPLOYERBYNAME",
             SELECTEMPLOYERFORGUARANTOR = "SELECTEMPLOYERFORGUARANTOR",
             SELECTEMPLOYERFORPATIENT = "SELECTEMPLOYERFORPATIENT";

        private const string
             EMPLOYER_CODE_DATA_AREA_NAME = "EMPLOYER_CODE_DATA_AREA_NAME",
             EMPLOYER_CODE_LIB_NAME = "EMPLOYER_CODE_LIB_NAME";

        private const string
            PARAM_P_EMFUUN = "@P_EMFUUN",
            PARAM_P_EMNAME = "@P_EMNAME",
            PARAM_P_EMURFG = "@P_EMURFG",
            PARAM_P_EMADDT = "@P_EMADDT",
            PARAM_P_EMLMDT = "@P_EMLMDT",
            PARAM_P_EMDLDT = "@P_EMDLDT",
            PARAM_P_EMCODE = "@P_EMCODE",
            PARAM_P_EMACNT = "@P_EMACNT",
            PARAM_P_EMUSER = "@P_EMUSER",
            PARAM_P_EMNEID = "@P_EMNEID",
            PARAM_P_FUUN = "@P_FUUN",
            PARAM_ACCOUNTNUMBER = "@P_ACCOUNTNUMBER",
            PARAM_MRCNUMBER = "@P_MRCNUMBER";

        private const string
            COL_EMPLOYERNAME = "EMPLOYERNAME",
            COL_EMPLOYERCODE = "EMPLOYERCODE",
            COL_NATIONALID = "NATIONALID";

        private const string
            COL_EMPLOYMENTSTATUSCODE = "EMPLOYMENTSTATUSCODE",
            COL_EMPLOYEEID = "EMPLOYEEID",
            COL_OCCUPATION = "OCCUPATION";

        private const string
           COL_ADDRESS1 = "ADDRESS1",
           COL_ADDRESS2 = "ADDRESS2",
           COL_CITY = "CITY",
           COL_POSTALCODE = "POSTALCODE",
           COL_STATECODE = "STATECODE",
           COL_COUNTRYCODE = "COUNTRYCODE",
           COL_TYPEOFCONTACTPOINTID = "TYPEOFCONTACTPOINTID",
           COL_CONTACTPOINTDESCRIPTION = "CONTACTPOINTDESCRIPTION",
           COL_AREACODE_PHONE = "AREACODEANDPHONE",
           COL_PHONEAREACODE = "AreaCode",
           COL_PHONENUMBER = "PhoneNumber",
           COL_PHONECOUNTRYCODE = "PhoneCountryCode",
           COL_EMAILADDRESS = "EmailAddress";

        private const string
            NEW_EMPLOYER_CODE_DATA_AREA_NAME = "EMPLOYER_CODE_DATA_AREA_NAME"
            ;

        private const string NEW_EMPLOYER_CODE_LIB_NAME = "EMPLOYER_CODE_LIB_NAME"
                             ;

        private const string
            NEW_INSERTEMPLOYERFORAPPROVAL = "INSERTEMPLOYERFORAPPROVAL"
            ;

        private const string NEW_GETALLEMPLOYERS = "GETALLEMPLOYERS"
                             ;

        private const string NEW_DELETEEMPLOYERFORAPPROVAL = "DELETEEMPLOYERFORAPPROVAL"
                             ;

        private const string
            NEW_COL_EMPLOYERNAME = "EMPLOYERNAME";

        private const string NEW_COL_EMPLOYERCODE = "EMPLOYERCODE",
                                NEW_COL_NATIONALID = "NATIONALID",
                                NEW_COL_USERID = "ADDEDBYUSERID"
                                ;

        private const string NEW_PARAM_P_ID = "@P_ID";

        private const string NEW_PARAM_P_EMFUUN = "@P_EMFUUN"
                             ;

        private const string NEW_PARAM_P_EMNAME = "@P_EMNAME"
                             ;

        private const string NEW_PARAM_P_EMURFG = "@P_EMURFG"
                             ;

        private const string NEW_PARAM_P_EMADDT = "@P_EMADDT"
                             ;

        private const string NEW_PARAM_P_EMLMDT = "@P_EMLMDT"
                             ;

        private const string NEW_PARAM_P_EMDLDT = "@P_EMDLDT"
                             ;

        private const string NEW_PARAM_P_EMCODE = "@P_EMCODE"
                             ;

        private const string NEW_PARAM_P_EMACNT = "@P_EMACNT"
                             ;

        private const string NEW_PARAM_P_EMUSER = "@P_EMUSER"
                             ;

        private const string NEW_PARAM_P_EMNEID = "@P_EMNEID"
                             ;

        private const string NEW_PARAM_P_FUUN = "@P_FUUN"
                             ;

        private const string
            NEW_OUT_PARAM_ID = "@OUT_PARAM_ID"
            ;

        private const string PACCESS_USER = "PACCESS";

        #endregion

    }
}
