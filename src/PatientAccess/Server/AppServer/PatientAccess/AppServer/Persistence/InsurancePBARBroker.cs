using System;
using System.Collections;
using System.Data;
using Extensions.DB2Persistence;
using Extensions.Exceptions;
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
    /// Summary description for InsurancePBARBroker.
    /// </summary>
    //TODO: Create XML summary comment for InsurancePBARBroker
    [Serializable]
    public class InsurancePBARBroker : PBARCodesBroker, IInsuranceBroker
    {
        #region Event Handlers
        #endregion

        #region Methods

        /// <summary>
        /// implement virtual method in base
        /// </summary>
        /// <returns></returns>
        protected override void InitProcNames()
        {
            this.AllStoredProcName = string.Empty;
            this.WithStoredProcName = string.Empty;
        }

        /// <summary>
        /// Get valid plan for given plan ID, admit date
        /// </summary>
        /// <param name="aPlanCode"></param>
        /// <param name="facilityOid"></param>
        /// <param name="admitDate"></param>
        /// <returns></returns>
        public InsurancePlan PlanWith(string aPlanCode, long facilityOid, DateTime admitDate)
        {
            InitFacility(facilityOid); 

            InsurancePlan plan = null;
            iDB2Command cmd = null;
            SafeReader reader = null;

            // Read the Payor as well as the plan.
            try
            {
                cmd = this.CommandFor("CALL " + SELECTPLANFORPAYORPLANCODE +
                    "(" + PARAM_PLANCD + 
                    "," + PARAM_FACILITYCODE + 
                    ")",
                    CommandType.Text,
                    this.Facility);

                cmd.Parameters[PARAM_PLANCD].Value = aPlanCode;
                cmd.Parameters[PARAM_FACILITYCODE].Value = this.Facility.Code;
                
                reader = this.ExecuteReader(cmd);

                ArrayList plans = this.PlanAndContractsFrom(reader, facilityOid);

                if (plans.Count > 0)
                {
                    plan = plans[0] as InsurancePlan;
                }

                if (plan != null)
                {
                    InsurancePlanContract aPlanContract = plan.GetBestPlanContractFor(admitDate);

                    if (aPlanContract != null)
                    {
                        plan.UpdateFromPlanContract(aPlanContract);
                    }

                    this.AddBillingInformationsFor(plan, this.Facility);
                }
            }
            catch (Exception e)
            {
                string msg = "Failed to load a plan with code of " + aPlanCode;
                throw BrokerExceptionFactory.BrokerExceptionFrom(msg, e, c_log);
            }
            finally
            {
                base.Close(reader);
                base.Close(cmd);
            }

            return plan;
        }
        public InsurancePlan PlanWith(string aPlanCode, long facilityOid)
        {
            InitFacility(facilityOid);

            InsurancePlan plan = null;
            iDB2Command cmd = null;
            SafeReader reader = null;

            // Read the Payor as well as the plan.
            try
            {
                cmd = this.CommandFor("CALL " + SELECTPLANFORPAYORPLANCODE +
                                      "(" + PARAM_PLANCD +
                                      "," + PARAM_FACILITYCODE +
                                      ")",
                    CommandType.Text,
                    this.Facility);

                cmd.Parameters[PARAM_PLANCD].Value = aPlanCode;
                cmd.Parameters[PARAM_FACILITYCODE].Value = this.Facility.Code;

                reader = this.ExecuteReader(cmd);
                while (reader.Read())
                {
                    DateTime effectiveDate
                        = DateTimeUtilities.DateTimeForYYYYMMDDFormat((long)reader.GetDecimal(COL_PLANEFFECTIVEDATE));
                    DateTime approvalDate
                        = DateTimeUtilities.DateTimeForYYYYMMDDFormat((long)reader.GetDecimal(COL_PLANAPPROVALDATE));
                    if (effectiveDate != DateTime.MinValue || approvalDate != DateTime.MinValue)
                    {
                        plan = PlanWith(aPlanCode, effectiveDate, approvalDate, facilityOid);
                    }
                }
            }
            catch (Exception e)
            {
                string msg = "Failed to load a plan with code of " + aPlanCode;
                throw BrokerExceptionFactory.BrokerExceptionFrom(msg, e, c_log);
            }
            finally
            {
                base.Close(reader);
                base.Close(cmd);
            }
            return plan;
        } 
 
        /// <summary>
        /// Get valid plan for given plan ID, approval date, effective date
        /// </summary>
        /// <param name="aPlanCode"></param>
        /// <param name="approvalDate"></param>
        /// <param name="effectiveDate"></param>
        /// <param name="facilityOid"></param>
        /// <param name="accountNumber"></param>
        /// <returns></returns>
        public InsurancePlan PlanWith(string aPlanCode,
            decimal approvalDate, decimal effectiveDate, long facilityOid, long accountNumber)
        {
            DateTime ApprovalDate = DateTimeUtilities.DateTimeForYYYYMMDDFormat(approvalDate.ToString());
            DateTime EffectiveDate = DateTimeUtilities.DateTimeForYYYYMMDDFormat(effectiveDate.ToString());

            InitFacility(facilityOid);

            InsurancePlan plan = null;
            iDB2Command cmd = null;
            SafeReader reader = null;

            // Read the Payor as well as the plan.
            try
            {
                if (aPlanCode.Equals(string.Empty))
                {
                    return plan;
                }
                cmd = this.CommandFor("CALL " + SELECTPLANFORPAYORPLANCODEWITHDATE +
                    "(" + PARAM_PLANCD +
                    "," + PARAM_FACILITYCODE + 
                    "," + PARAM_APPROVALDATE + 
                    "," + PARAM_EFFECTIVEDATE +
                    ")",
                    CommandType.Text,
                    this.Facility);

                cmd.Parameters[PARAM_PLANCD].Value = aPlanCode;
                cmd.Parameters[PARAM_FACILITYCODE].Value = this.Facility.Code;
                cmd.Parameters[PARAM_APPROVALDATE].Value = approvalDate;
                cmd.Parameters[PARAM_EFFECTIVEDATE].Value = effectiveDate;

                reader = this.ExecuteReader(cmd);

                ArrayList plans = PlanAndContractsFrom(reader, facilityOid);

                if (plans.Count > 0)
                {
                    plan = plans[0] as InsurancePlan;
                }
                
                if (plan != null)
                {
                    InsurancePlanContract aPlanContract = plan.GetPlanContractFor(ApprovalDate, EffectiveDate);

                    if (aPlanContract != null)
                    {
                        plan.UpdateFromPlanContract(aPlanContract);
                    }

                    if (plan.PlanName == string.Empty)
                    {
                        c_log.Error("Found an Account " + accountNumber +
                            " At Facility: " + this.Facility.Code +
                            " with Insurance plan that was not valid " + aPlanCode);
                    }
                    else
                    {
                        this.AddBillingInformationsFor(plan, this.Facility);
                    }
                }

            }
            catch (Exception e)
            {
                string msg = "Failed to load a plan with code of " + aPlanCode;
                throw BrokerExceptionFactory.BrokerExceptionFrom(msg, e, c_log);
            }
            finally
            {
                base.Close(reader);
                base.Close(cmd);
            }

            return plan;
        }

        /// <summary>
        /// Get valid plan for given plan ID, approval date, effective date
        /// </summary>
        /// <param name="aPlanCode"></param>
        /// <param name="approvalDate"></param>
        /// <param name="effectiveDate"></param>
        /// <param name="facilityOid"></param>
        /// <returns></returns>
        public InsurancePlan PlanWith(string aPlanCode, DateTime effectiveDate, DateTime approvalDate, long facilityOid)
        {
            InitFacility(facilityOid); 

            InsurancePlan plan = null;
            iDB2Command cmd = null;
            SafeReader reader = null;

            // Read the Payor as well as the plan.
            try
            {
                if (aPlanCode.Equals(string.Empty))
                {
                    return plan;
                }
                cmd = this.CommandFor("CALL " + SELECTPLANFORPAYORPLANCODEWITHDATE +
                    "(" + PARAM_PLANCD +
                    "," + PARAM_FACILITYCODE +
                    "," + PARAM_APPROVALDATE +
                    "," + PARAM_EFFECTIVEDATE +
                    ")",
                    CommandType.Text,
                    this.Facility);

                cmd.Parameters[PARAM_PLANCD].Value = aPlanCode;
                cmd.Parameters[PARAM_FACILITYCODE].Value = this.Facility.Code;
                cmd.Parameters[PARAM_APPROVALDATE].Value = (approvalDate != DateTime.MinValue) ? decimal.Parse(approvalDate.ToString("yyyyMMdd")) : 0;
                cmd.Parameters[PARAM_EFFECTIVEDATE].Value = (effectiveDate != DateTime.MinValue) ? decimal.Parse(effectiveDate.ToString("yyyyMMdd")) : 0;

                reader = this.ExecuteReader(cmd);

                ArrayList plans = PlanAndContractsFrom(reader, facilityOid);

                if (plans.Count > 0)
                {
                    plan = plans[0] as InsurancePlan;
                }

                if (plan != null)
                {
                    InsurancePlanContract aPlanContract = plan.GetPlanContractFor(approvalDate, effectiveDate);

                    if (aPlanContract != null)
                    {
                        plan.UpdateFromPlanContract(aPlanContract);
                    }

                    if (plan.PlanName == string.Empty)
                    {
                        c_log.Error("Facility: " + this.Facility.Code +
                            " - Insurance plan was not valid " + aPlanCode);
                    }
                    else
                    {
                        this.AddBillingInformationsFor(plan, this.Facility);
                    }
                }

            }
            catch (Exception e)
            {
                string msg = "Failed to load a plan with code of " + aPlanCode;
                throw BrokerExceptionFactory.BrokerExceptionFrom(msg, e, c_log);
            }
            finally
            {
                base.Close(reader);
                base.Close(cmd);
            }

            return plan;
        }

        /// <summary>
        /// Get IPA for given IPA code, clinic code
        /// </summary>
        /// <param name="facilityOid"></param>
        /// <param name="IPACode"></param>
        /// <param name="ClinicCode"></param>
        /// <returns></returns>
        public MedicalGroupIPA IPAWith(long facilityOid, string IPACode, string ClinicCode)
        {
            InitFacility(facilityOid); 


            MedicalGroupIPA ipa = null;
            SafeReader reader = null;
            iDB2Command cmd = null;
            IPACode = IPACode.Trim();
            ClinicCode = ClinicCode.Trim();

            try
            {
                cmd = this.CommandFor("CALL "+ SELECTIPASBYCODE +
                    "(" + PARAM_FACILITYID +
                    "," + PARAM_IPACODE +
                    "," + PARAM_CLINICCODE +
                    ")",
                    CommandType.Text,
                    this.Facility 
                    );
                cmd.Parameters[PARAM_FACILITYID].Value = facilityOid;
                cmd.Parameters[PARAM_IPACODE].Value = IPACode;
                cmd.Parameters[PARAM_CLINICCODE].Value = ClinicCode;
                
                reader = this.ExecuteReader(cmd);

                while (reader.Read())
                {
                    ipa = new MedicalGroupIPA();
                    ipa.Code = reader.GetString(COL_IPACODE);
                    ipa.Name = reader.GetString(COL_IPANAME);
                    ipa.Oid = reader.GetInt64(COL_IPAID);

                    Clinic clinic = new Clinic();
                    clinic.Code = reader.GetString(COL_CLINICCODE);
                    clinic.Name = reader.GetString(COL_CLINICNAME);
                    ipa.AddClinic(clinic);
                }

                if (ipa == null && IPACode.Equals(string.Empty) && ClinicCode.Equals(string.Empty))
                {
                    ipa = new MedicalGroupIPA();
                    ipa.AddClinic(new Clinic());
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                string msg = "Attempt to read IPA/Clinic Failed.";
                throw BrokerExceptionFactory.BrokerExceptionFrom(msg, e, c_log);
            }
            finally
            {
                base.Close(reader);
                base.Close(cmd);
            }
            return ipa;
        }

        /// <summary>
        /// Get IPAs for a given IPA name
        /// </summary>
        /// <param name="facilityOid"></param>
        /// <param name="IPAName"></param>
        /// <returns></returns>
        public ICollection IPAsFor(long facilityOid, string IPAName)
        {
            InitFacility(facilityOid);

            ArrayList IPAS = new ArrayList();

            SafeReader reader = null;
            iDB2Command cmd = null;

            try
            {
                cmd = this.CommandFor("CALL " + SELECTIPASBYNAME +
                    "(" + PARAM_FACILITYID +
                    "," + PARAM_IPANAME +
                    ")",
                    CommandType.Text,
                    this.Facility);
                
                cmd.Parameters[PARAM_FACILITYID].Value = facilityOid;
                cmd.Parameters[PARAM_IPANAME].Value = StringFilter.mangleName( IPAName );

                reader = this.ExecuteReader(cmd);

                MedicalGroupIPA ipa = null;

                while (reader.Read())
                {
                    string ipaCode = reader.GetString(COL_IPACODE);
                    if (ipa == null || ipa.Code  != ipaCode)
                    {
                        ipa = new MedicalGroupIPA();
                        ipa.Oid = reader.GetInt64(COL_IPAID);
                        ipa.Code = reader.GetString(COL_IPACODE);
                        ipa.Name = reader.GetString(COL_IPANAME);
                        IPAS.Add(ipa);
                    }
                    
                    string clinicCode = reader.GetString(COL_CLINICCODE);
                    if (clinicCode.Trim() != string.Empty)
                    {
                        Clinic clinic = new Clinic();
                        clinic.Code = reader.GetString(COL_CLINICCODE);
                        clinic.Name = reader.GetString(COL_CLINICNAME);
                        ipa.AddClinic(clinic);
                    }
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                string msg = "Attempt to read IPA/Clinic Failed.";
                throw BrokerExceptionFactory.BrokerExceptionFrom(msg, e, c_log);
            }
            finally
            {
                base.Close(reader);
                base.Close(cmd);
            }
            return IPAS;
        }

        /// <summary>
        /// Get a list of valid plans for a given payor or broker
        /// </summary>
        /// <param name="aProvider"></param>
        /// <param name="facilityOid"></param>
        /// <param name="admitDate"></param>
        /// <param name="planCategory"></param>
        /// <returns></returns>
        public ICollection InsurancePlansFor(AbstractProvider aProvider, long facilityOid, DateTime admitDate, InsurancePlanCategory planCategory)
        {
            if (aProvider.GetType() == typeof(Payor) ||
                aProvider.GetType() == typeof(OtherPayor))
            {
                return this.InsurancePlansForProvider(aProvider as Payor, facilityOid, admitDate, planCategory);
            }
            else if (aProvider.GetType() == typeof(Broker))
            {
                return this.InsurancePlansForProvider(aProvider as Broker, facilityOid, admitDate, planCategory);
            }

            string msg = String.Format("Expected either a Payor or Broker as a concrete provider, but recieved {0}",
                aProvider.GetType());
            EnterpriseException ex = new EnterpriseException(msg, Severity.High);
            throw BrokerExceptionFactory.BrokerExceptionFrom(ex, c_log);
        }

        /// <summary>
        /// Get a list of valid plans for a given covered group, admit date and plan category
        /// </summary>
        /// <param name="aCoveredGroup"></param>
        /// <param name="facilityOid"></param>
        /// <param name="admitDate"></param>
        /// <param name="planCategory"></param>
        /// <returns></returns>
        public ICollection InsurancePlansFor(CoveredGroup aCoveredGroup, long facilityOid, DateTime admitDate, InsurancePlanCategory planCategory)
        {
            InitFacility(facilityOid); 


            ArrayList list = new ArrayList();
            iDB2Command cmd = null;
            SafeReader reader = null;

            try
            {
                cmd = this.CommandFor("CALL " + SELECTPLANSFORCOVEREDGROUP +
                    "(" + PARAM_EMPLOYERCODE +
                    "," + PARAM_EMPLOYERNAME +
                    "," + PARAM_FACILITYID +
                    "," + PARAM_ADMIT_DATE +
                    ")",
                    CommandType.Text,
                    this.Facility );

                if (aCoveredGroup != null && aCoveredGroup.Employer != null && aCoveredGroup.Employer != null)
                {
                    cmd.Parameters[PARAM_EMPLOYERCODE].Value = ((EmployerProxy)aCoveredGroup.Employer).EmployerCode;
                    cmd.Parameters[PARAM_EMPLOYERNAME].Value = ((EmployerProxy)aCoveredGroup.Employer).Name;
                }
                else
                {
                    cmd.Parameters[PARAM_EMPLOYERCODE].Value = 0;
                    cmd.Parameters[PARAM_EMPLOYERNAME].Value = string.Empty;
                }
                cmd.Parameters[PARAM_FACILITYID].Value = facilityOid;
                cmd.Parameters[PARAM_ADMIT_DATE].Value = (admitDate!=DateTime.MinValue) ? decimal.Parse(admitDate.ToString("yyyyMMdd")) : 0;

                reader = this.ExecuteReader(cmd);

                list = this.PlanAndContractsFrom(reader, facilityOid);

                foreach (InsurancePlan aPlan in list)
                {
                    InsurancePlanContract aPlanContract = aPlan.GetBestPlanContractFor(admitDate);

                    if (aPlanContract != null)
                    {
                        aPlan.UpdateFromPlanContract(aPlanContract);
                    }
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                throw BrokerExceptionFactory.BrokerExceptionFrom("CoveredGroupsWith fails", e, c_log);
            }
            finally
            {
                base.Close(reader);
                base.Close(cmd);
            }

            return list;
        }

        /// <summary>
        /// Get a list of covered groups for given name, admit date
        /// </summary>
        /// <param name="aName"></param>
        /// <param name="facilityOid"></param>
        /// <param name="admitDate"></param>
        /// <returns></returns>
        public ICollection CoveredGroupsMatching(string aName, long facilityOid, DateTime admitDate)
        {
            InitFacility(facilityOid);

            ArrayList coveredGroups = new ArrayList();
            iDB2Command cmd = null;
            SafeReader reader = null;

            try
            {
                cmd = this.CommandFor("CALL " + SELECTCOVEREDGROUPSMATCHING +
                    "(" + PARAM_COVEREDGROUPNAME +
                    "," + PARAM_FACILITYID + 
                    "," + PARAM_ADMIT_DATE +
                    ")",
                    CommandType.Text,
                    this.Facility);

                cmd.Parameters[PARAM_COVEREDGROUPNAME].Value = StringFilter.mangleName( aName );
                cmd.Parameters[PARAM_FACILITYID].Value = facilityOid;
                cmd.Parameters[PARAM_ADMIT_DATE].Value = (admitDate!=DateTime.MinValue) ? decimal.Parse(admitDate.ToString("yyyyMMdd")) : 0;
                
                reader = this.ExecuteReader(cmd);

                // create brokers that will be used as 'helper' brokers
                IAddressBroker addressBroker = BrokerFactory.BrokerOfType<IAddressBroker>();
                
                long lastGroupCode = -1;
                string lastGroupName = string.Empty;
                while (reader.Read())
                {
                    long groupID = reader.GetInt64(COL_COVEREDGROUPID);
                    long groupCode = (long)reader.GetDecimal(COL_EMPLOYERCODE);
                    string groupName = reader.GetString(COL_GROUPCOVERED);
                    //long planID = reader.GetInt64(COL_PLANID);

                    if (groupCode != lastGroupCode && groupName != lastGroupName )
                    {
                        lastGroupCode = groupCode;
                        lastGroupName = groupName;
                        CoveredGroup group = new CoveredGroup();
                        group.Name = groupName;
                        group.Oid = groupID;
                        // need to get the address for this as well.
                        Address address = this.AddressFrom(facilityOid, reader);

                        IEmployer employerProxy = this.EmployerFrom(reader, this.Facility);

                        if (address != null)
                        {
                            group.AddContactPoint(new ContactPoint(address, null, null,
                                TypeOfContactPoint.NewBusinessContactPointType()));
                            //                            group.AddAddress(address);
                        }

                        if (employerProxy != null)
                        {
                            group.Employer = employerProxy;
                        }

                        group.PlansLoader = new CoveredGroupPlansLoader(group, facilityOid, admitDate);

                        coveredGroups.Add(group);
                    }
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                throw BrokerExceptionFactory.BrokerExceptionFrom("CoveredGroupsWith fails", e, c_log);
            }
            finally
            {
                base.Close(reader);
                base.Close(cmd);
            }

            return coveredGroups;
        }
        public int GetCoveredGroupCountFor( string planID, DateTime effectiveDate, DateTime approvalDate, long facilityOid, DateTime admitDate )
        {
            InitFacility( facilityOid );

            ArrayList coveredGroups = new ArrayList();
            iDB2Command cmd = null;
            SafeReader reader = null;
            int coveredGroupCount = 0;
            try
            {
                cmd = this.CommandFor( "CALL " + SELECTCOVEREDGROUPSFOR +
                    "(" + PARAM_PLANCD +
                    "," + PARAM_EFFECTIVEDATE +
                    "," + PARAM_APPROVALDATE +
                    "," + PARAM_FACILITYID +
                    "," + PARAM_ADMIT_DATE +
                    ")",
                    CommandType.Text,
                    this.Facility );

                cmd.Parameters[PARAM_PLANCD].Value = planID;
                cmd.Parameters[PARAM_EFFECTIVEDATE].Value = ( effectiveDate != DateTime.MinValue ) ? decimal.Parse( effectiveDate.ToString( "yyyyMMdd" ) ) : 0;
                cmd.Parameters[PARAM_APPROVALDATE].Value = ( approvalDate != DateTime.MinValue ) ? decimal.Parse( approvalDate.ToString( "yyyyMMdd" ) ) : 0;
                cmd.Parameters[PARAM_FACILITYID].Value = facilityOid;
                cmd.Parameters[PARAM_ADMIT_DATE].Value = ( admitDate != DateTime.MinValue ) ? decimal.Parse( admitDate.ToString( "yyyyMMdd" ) ) : 0;

                reader = this.ExecuteReader( cmd );
                while( reader.Read() )
                {
                    coveredGroupCount++;
                }
                return coveredGroupCount;
                
            }
            catch( Exception e )
            {
                Console.Error.WriteLine( e );
                throw BrokerExceptionFactory.BrokerExceptionFrom( "CoveredGroupsFor fails", e, c_log );
            }
            finally
            {
                base.Close( reader );
                base.Close( cmd );
            }

        }

        /// <summary>
        /// Get a list of covered groups for given plan ID, effective date, approval date, admite date
        /// </summary>
        /// <param name="planID"></param>
        /// <param name="effectiveDate"></param>
        /// <param name="approvalDate"></param>
        /// <param name="facilityOid"></param>
        /// <param name="admitDate"></param>
        /// <returns></returns>       
        public ICollection CoveredGroupsFor(string planID, DateTime effectiveDate, DateTime approvalDate, long facilityOid, DateTime admitDate)
        {
            InitFacility(facilityOid);

            ArrayList coveredGroups = new ArrayList();
            iDB2Command cmd = null;
            SafeReader reader = null;

            try
            {
                cmd = this.CommandFor("CALL " + SELECTCOVEREDGROUPSFOR +
                    "(" + PARAM_PLANCD +
                    "," + PARAM_EFFECTIVEDATE +
                    "," + PARAM_APPROVALDATE +
                    "," + PARAM_FACILITYID +
                    "," + PARAM_ADMIT_DATE +
                    ")",
                    CommandType.Text,
                    this.Facility);

                cmd.Parameters[PARAM_PLANCD].Value = planID;
                cmd.Parameters[PARAM_EFFECTIVEDATE].Value = (effectiveDate!=DateTime.MinValue) ? decimal.Parse(effectiveDate.ToString("yyyyMMdd")) : 0;
                cmd.Parameters[PARAM_APPROVALDATE].Value = (approvalDate!=DateTime.MinValue) ? decimal.Parse(approvalDate.ToString("yyyyMMdd")) : 0;
                cmd.Parameters[PARAM_FACILITYID].Value = facilityOid;
                cmd.Parameters[PARAM_ADMIT_DATE].Value = (admitDate!=DateTime.MinValue) ? decimal.Parse(admitDate.ToString("yyyyMMdd")) : 0;

                reader = this.ExecuteReader(cmd);

                // create brokers that will be used as 'helper' brokers
                long lastGroupCode = -1;
                string lastGroupName = string.Empty;
                while (reader.Read())
                {
                    long groupID = reader.GetInt64(COL_COVEREDGROUPID);
                    long groupCode = (long)reader.GetDecimal(COL_EMPLOYERCODE);
                    string groupName = reader.GetString(COL_GROUPCOVERED);
                    //long planID = reader.GetInt64(COL_PLANID);

                    if (groupCode != lastGroupCode && groupName != lastGroupName)
                    {
                        lastGroupCode = groupCode;
                        lastGroupName = groupName;
                        CoveredGroup group = new CoveredGroup();
                        group.Name = groupName;
                        group.Oid = groupID;
                        // need to get the address for this as well.
                        Address address = this.AddressFrom(facilityOid, reader);

                        IEmployer employerProxy = this.EmployerFrom(reader, this.Facility);

                        if (address != null)
                        {
                            group.AddContactPoint(new ContactPoint(address, null, null,
                                TypeOfContactPoint.NewEmployerContactPointType()));
                        }

                        if (employerProxy != null)
                        {
                            group.Employer = employerProxy;
                        }
                        
                        group.PlansLoader = new CoveredGroupPlansLoader(group, facilityOid, admitDate);
                        
                        coveredGroups.Add(group);
                    }
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                throw BrokerExceptionFactory.BrokerExceptionFrom("CoveredGroupsFor fails", e, c_log);
            }
            finally
            {
                base.Close(reader);
                base.Close(cmd);
            }

            return coveredGroups;
        }

        /// <summary>
        /// Get a list of providers (payors and brokers) starting with given name
        /// </summary>
        /// <param name="aName"></param>
        /// <param name="facilityOid"></param>
        /// <param name="admitDate"></param>
        /// <param name="planCategory"></param>
        /// <returns></returns>
        public ICollection ProvidersStartingWith(string aName, long facilityOid, DateTime admitDate,
                                                 InsurancePlanCategory planCategory)
        {
            return (this.ProvidersWith(aName, facilityOid, admitDate, planCategory, SEARCH_OPTION_BEGINSWITH));
        }

        /// <summary>
        /// Get a list of providers (payors and brokers) containing given name
        /// </summary>
        /// <param name="aName"></param>
        /// <param name="facilityOid"></param>
        /// <param name="admitDate"></param>
        /// <param name="planCategory"></param>
        /// <returns></returns>
        public ICollection ProvidersContaining(string aName, long facilityOid, DateTime admitDate,
                                               InsurancePlanCategory planCategory)
        {
            return (this.ProvidersWith(aName, facilityOid, admitDate, planCategory, SEARCH_OPTION_CONTAINS));
        }

        /// <summary>
        /// return a list of all insurance categories
        /// </summary>
        /// <param name="facilityOid"></param>
        /// <returns></returns>
        /// <exception cref="Exception"><c>Exception</c>.</exception>
        public IList AllTypesOfCategories(long facilityOid)
        {
            InitFacility(facilityOid);
            
            IList planCategoryList = new ArrayList();

            ReadDataDelegate<InsurancePlanCategory> PlanCategoryFrom = delegate(SafeReader fromReader)
            {
                long planCategoryId = (long)fromReader.GetDecimal(COL_PLANCATEGORYID);
                string planDescription = fromReader.GetString(COL_PLANCATEGORYDESCRIPTION);
                string planLabel = fromReader.GetString(COL_EXPECTEDASSOCIATEDNUMBERLABEL);

                InsurancePlanCategory ipc = new InsurancePlanCategory(planCategoryId, ReferenceValue.NEW_VERSION, planDescription, planLabel);
                return ipc;
            };

            try
            {
                this.AllStoredProcName = SELECTALLINSURANCECATEGORIES;
                planCategoryList = (IList)LoadDataToArrayList(PlanCategoryFrom);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                string msg = "InsurancePlanCategories cache failed to initialize.";
                throw BrokerExceptionFactory.BrokerExceptionFrom(msg, e, c_log);
            }
            return planCategoryList;
        }

        /// <summary>
        /// Return an insurance category with a specific category ID
        /// </summary>
        /// <param name="insurancePlanCategoryID"></param>
        /// <param name="facilityOid"></param>
        /// <returns></returns>
        public InsurancePlanCategory InsurancePlanCategoryWith(long insurancePlanCategoryID, long facilityOid)
        {
            InsurancePlanCategory foundCategory = null;

            ICollection categories = this.AllTypesOfCategories(facilityOid);
            foreach (InsurancePlanCategory category in categories)
            {
                if (category.Oid == insurancePlanCategoryID)
                {
                    foundCategory = category;
                    break;
                }
            }
            return foundCategory;
        }

        /// <summary>
        /// return a list of insurance plan types
        /// </summary>
        /// <param name="facilityOid"></param>
        /// <returns></returns>
        public ICollection AllPlanTypes(long facilityOid)
        {
            InitFacility(facilityOid);

            ICollection planTypes = new ArrayList();

            try
            {
                this.AllStoredProcName = SELECTALLPLANTYPES;
                planTypes = LoadDataToArrayList<InsurancePlanType>();
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                string msg = "PlanType cache failed to initialize.";
                throw BrokerExceptionFactory.BrokerExceptionFrom(msg, e, c_log);
            }
            return planTypes;
        }

        /// <summary>
        /// return an insurance type with a specific type ID
        /// </summary>
        /// <param name="planTypeID"></param>
        /// <returns></returns>
        public InsurancePlanType InsurancePlanTypeWith(long planTypeID)
        {
            throw new BrokerException("This method not implemented in PBAR Version");
        }

        /// <summary>
        /// Return an insurance Plan type for a plan type code
        /// </summary>
        /// <param name="planTypeCode"></param>
        /// <param name="facilityOid"></param>
        /// <returns></returns>
        public InsurancePlanType InsurancePlanTypeWith(string planTypeCode, long facilityOid)
        {
            ICollection plans = this.AllPlanTypes(facilityOid);

            InsurancePlanType foundPlanType = null;

            foreach (InsurancePlanType insType in plans)
            {
                if (insType.Code == planTypeCode.Trim())
                {
                    foundPlanType = insType;
                }
            }

            return foundPlanType;
        }

        /// <summary>
        /// return a list of fin classes for the payor with payor code
        /// </summary>
        /// <param name="facilityOid"></param>
        /// <param name="payorCode"></param>
        /// <returns></returns>
        public ICollection PayorFinClassesFor(long facilityOid, string payorCode)
        {
            InitFacility(facilityOid);

            SafeReader reader = null;
            iDB2Command cmd = null;

            IFinancialClassesBroker financialClassesBroker = BrokerFactory.BrokerOfType<IFinancialClassesBroker>();
            ArrayList finClassList = new ArrayList();

            try
            {
                cmd = this.CommandFor("CALL " + SELECTPAYORFINCLASSESWITH +
                    "(" + PARAM_PAYORCODE + 
                    "," + PARAM_FACILITYID +
                    ")",
                    CommandType.Text,
                    this.Facility );

                cmd.Parameters[PARAM_PAYORCODE].Value = payorCode;
                cmd.Parameters[PARAM_FACILITYID].Value = facilityOid;

                reader = this.ExecuteReader(cmd);

                while (reader.Read())
                {
                    string financialClassCode = reader.GetString(COL_FINANCIALCLASSCODE);
                    FinancialClass fc = financialClassesBroker.FinancialClassWith( facilityOid, financialClassCode );

                    finClassList.Add(fc);
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                string msg = "Unable to retrieve Payor Financial Classes.";
                throw BrokerExceptionFactory.BrokerExceptionFrom(msg, e, c_log);
            }
            finally
            {
                base.Close(reader);
                base.Close(cmd);
            }
            return finClassList;
        }

        /// <summary>
        /// return a list of fin classes for the plans with plan suffix
        /// </summary>
        /// <param name="facilityOid"></param>
        /// <param name="planSuffix"></param>
        /// <returns></returns>
        public ICollection PlanFinClassesFor(long facilityOid, string planSuffix)
        {
            InitFacility(facilityOid);

            SafeReader reader = null;
            iDB2Command cmd = null;

            IFinancialClassesBroker financialClassesBroker = BrokerFactory.BrokerOfType<IFinancialClassesBroker>();
            ArrayList finClassList = new ArrayList();
            string blankCode = "  ";
            FinancialClass blankfc = financialClassesBroker.FinancialClassWith( facilityOid, blankCode );

            finClassList.Add(blankfc);

            try
            {
                cmd = this.CommandFor("CALL " + SELECTPLANFINCLASSESWITH +
                    "(" + PARAM_PLANSUFFIX +
                    ")",
                    CommandType.Text,
                    this.Facility);

                cmd.Parameters[PARAM_PLANSUFFIX].Value = planSuffix;

                reader = this.ExecuteReader(cmd);

                while (reader.Read())
                {
                    string financialClassCode = reader.GetString(COL_FINANCIALCLASSCODE);
                    FinancialClass fc = financialClassesBroker.FinancialClassWith( facilityOid, financialClassCode );

                    finClassList.Add(fc);
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                string msg = "Unable to retrieve Plan Financial classes.";
                throw BrokerExceptionFactory.BrokerExceptionFrom(msg, e, c_log);
            }
            finally
            {
                base.Close(reader);
                base.Close(cmd);
            }
            return finClassList;
        }

        /// <summary>
        /// is valid HSV plan for specialty medicare
        /// </summary>
        /// <param name="facilityOid"></param>
        /// <param name="HSVCode"></param>
        /// <param name="planId"></param>
        /// <returns></returns>
        public bool IsValidHSVPlanForSpecialtyMedicare(long facilityOid, string HSVCode, string planId)
        {
            InitFacility(facilityOid);

            bool isvalidMapping = false;

            iDB2Command cmd = null;
            SafeReader reader = null;

            try
            {
                cmd = this.CommandFor("CALL " + SP_SELECTISVALIDHSVPLANMAPPING +
                    "(" + PARAM_SERVICECD + 
                    "," + PARAM_PLANCD + 
                    "," + PARAM_SERVICECOUNT +
                    ")",
                    CommandType.Text,
                    this.Facility);

                cmd.Parameters[PARAM_PLANCD].Value = planId;
                cmd.Parameters[PARAM_SERVICECD].Value = HSVCode;
                cmd.Parameters[PARAM_SERVICECOUNT].Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                long mappingCnt = Convert.ToInt32(cmd.Parameters[PARAM_SERVICECOUNT].Value);

                if (mappingCnt > 0)
                {
                    isvalidMapping = true;
                }
            }
            catch (Exception ex)
            {
                string msg = "Attempt to get count of HSV and PlanId for Specialty Medicare Failed.";
                throw BrokerExceptionFactory.BrokerExceptionFrom(msg, ex, c_log);
            }
            finally
            {
                base.Close(reader);
                base.Close(cmd);
            }

            return isvalidMapping;

        }

        /// <summary>
        /// This method builds the coverage for PreMSEActivity for an account
        /// </summary>
        /// <param name="facilityId"></param>
        /// <param name="admitDate"></param>
        /// <exception cref="EnterpriseException">No Insurance Plan found in the database for PRE-MSE Default Plan IDs.</exception>
        /// <exception cref="Exception"><c>Exception</c>.</exception>
        public InsurancePlan GetDefaultInsurancePlan(long facilityId, DateTime admitDate)
        {
            InsurancePlan plan;
            try
            {
                plan = PlanWith(InsurancePlan.QUICK_ACCOUNTS_DEFAULT_INSURANCE_PLAN_ID, facilityId,
                                admitDate);
                if (plan == null)
                {
                    throw new EnterpriseException(
                        "No Default Insurance Plan found in the database for Quick accounts.",
                        Severity.High);
                }
            }
            catch (Exception ex)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom(ex);
            }
            return plan;
        }
       

        #endregion

        #region Properties
        #endregion

        #region Private Methods
        
        /// <summary>
        /// add billing informations to the plan
        /// </summary>
        /// <param name="plan"></param>
        /// <param name="facility"></param>
        private void AddBillingInformationsFor(InsurancePlan plan, Facility facility)
        {
            if (plan == null)
            {
                return;
            } 

            iDB2Command cmd = null;
            SafeReader reader = null;

            // Read the billing info as well as the plan.
            try
            {
                cmd = this.CommandFor("CALL " + SP_BILLING_INFORMATIONS_FOR +
                    "(" + PARAM_PLANCD +
                    "," + PARAM_FACILITYID + 
                    "," + PARAM_EFFECTIVEDATE +
                    "," + PARAM_APPROVALDATE +
                    ")",
                    CommandType.Text,
                    facility);

                cmd.Parameters[PARAM_FACILITYID].Value = facility.Oid;
                cmd.Parameters[PARAM_PLANCD].Value = plan.PlanID;
                cmd.Parameters[PARAM_EFFECTIVEDATE].Value = (plan.EffectiveOn!=DateTime.MinValue) ? decimal.Parse(plan.EffectiveOn.ToString("yyyyMMdd")) : 0;
                cmd.Parameters[PARAM_APPROVALDATE].Value = (plan.ApprovedOn!=DateTime.MinValue) ? decimal.Parse(plan.ApprovedOn.ToString("yyyyMMdd")) : 0;
                reader = this.ExecuteReader(cmd);

                // use a standard method for reading plans from the reader
                this.AddBillingInfoTo(plan, reader, facility.Oid);
            }
            catch (Exception e)
            {
                string msg = "Failed to add billing informations to " + plan.PlanName;
                throw BrokerExceptionFactory.BrokerExceptionFrom(msg, e, c_log);
            }
            finally
            {
                base.Close(reader);
                base.Close(cmd);
            }
        }

        /// <summary>
        /// add billing informations to teh plan
        /// </summary>
        /// <param name="plan"></param>
        /// <param name="fromReader"></param>
        /// <param name="facilityID"></param>
        private void AddBillingInfoTo(InsurancePlan plan, SafeReader fromReader, long facilityID)
        {
            PhoneNumber aPhoneNumber = new PhoneNumber();
            EmailAddress anEmailAddress = new EmailAddress();
            Address anAddress = null;

            while ( fromReader.Read() )
            {
                // read address
                anAddress = this.AddressFrom( facilityID, fromReader );

                // read phone number
                string areaCode = fromReader.GetString(COL_PHONEAREACODE);
                areaCode = areaCode.PadLeft(3, '0');
                string number = fromReader.GetString(COL_PHONENUMBER);
                number = number.PadLeft(7, '0');
                aPhoneNumber = new PhoneNumber(areaCode, number);

                // read email address
                string emailAddress = StringFilter.RemoveAllNonEmailSpecialCharacters( fromReader.GetString( COL_EMAILADDRESS ) );

                try
                {
                    anEmailAddress = new EmailAddress(emailAddress);
                }
                catch (InvalidEmailAddressException e)
                {
                    BrokerException ex = new BrokerException("InvalidEmailAddressException: " + e.Message, null, Severity.High);
                    throw ex;
                }

                string tcDescription = fromReader.GetString(COL_CONTACTDESCRIPTION);

                TypeOfContactPoint tc = new TypeOfContactPoint(0L, tcDescription);

                BillingInformation billInfo = new BillingInformation(anAddress,
                    aPhoneNumber,
                    anEmailAddress,
                    tc);


                billInfo.BillingCOName = fromReader.GetString(COL_BILLINGCONAME);
                billInfo.BillingName = fromReader.GetString(COL_BILLINGNAME);
                billInfo.Area = fromReader.GetString(COL_PLANAREA);

                plan.AddBillingInformation(billInfo);
            }
        }

        /// <summary>
        /// read plans from reader
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="facilityOid"></param>
        /// <returns></returns>
        private ArrayList PlanAndContractsFrom(SafeReader reader, long facilityOid)
        {
            ArrayList plans = new ArrayList();

            string prevPayorCode = string.Empty;
            string prevPlanSuffix = string.Empty;

            InsurancePlan planHolder = null;

            while (reader.Read())
            {

                InsurancePlanContract contract = new InsurancePlanContract();
                InsurancePlan aPlan = null;


                long planID = reader.GetInt64(COL_PLANID);
                string planSuffix = reader.GetString(COL_PLANSUFFIX);
                string planName = reader.GetString(COL_PLANNAME);
                string planTypeCode = reader.GetString(COL_PLANTYPECODE);
                long planCategoryID = reader.GetInt32(COL_PLANCATEGORYID);
                string lineOfBusiness = reader.GetString(COL_LOBDESCRIPTION);

                InsurancePlanType planType = this.InsurancePlanTypeWith(planTypeCode, facilityOid);
                InsurancePlanCategory planCategory = this.InsurancePlanCategoryWith(planCategoryID, facilityOid);


                DateTime effectiveDate 
                    = DateTimeUtilities.DateTimeForYYYYMMDDFormat((long)reader.GetDecimal(COL_PLANEFFECTIVEDATE));
                DateTime approvalDate 
                    = DateTimeUtilities.DateTimeForYYYYMMDDFormat((long)reader.GetDecimal(COL_PLANAPPROVALDATE));
                DateTime terminationDate 
                    = DateTimeUtilities.DateTimeForYYYYMMDDFormat((long)reader.GetDecimal(COL_PLANTERMINATIONDATE));
                DateTime cancelDate 
                    = DateTimeUtilities.DateTimeForYYYYMMDDFormat((long)reader.GetDecimal(COL_PLANCANCELDATE));

                contract.TerminationGraceDays = reader.GetInt64(COL_TERMGRACEDAYS);
                contract.CancellationGraceDays = reader.GetInt64(COL_CANCELGRACEDAYS);
                contract.TerminationPurgeDays = reader.GetInt64(COL_TERMPURGEDAYS);
                contract.CancellationPurgeDays = reader.GetInt64(COL_CANCELPURGEDAYS);

                DateTime adjustedTermDate 
                    = DateTimeUtilities.DateTimeForYYYYMMDDFormat((long)reader.GetDecimal(COL_ADJUSTEDTERMDATE));
                DateTime adjustedCanDate 
                    = DateTimeUtilities.DateTimeForYYYYMMDDFormat((long)reader.GetDecimal(COL_ADJUSTEDCANCELDATE));

                long payorID = reader.GetInt64(COL_PAYORID);
                string payorName = reader.GetString(COL_PAYORNAME);
                string payorCode = reader.GetString(COL_PAYORCODE);

                Payor payor = new Payor();
                payor.Oid = payorID;
                payor.Name = payorName;
                payor.Code = payorCode;

                contract.Payor = payor;

                contract.EffectiveOn = effectiveDate;
                contract.ApprovedOn = approvalDate;
                contract.TerminatedOn = terminationDate;
                contract.CanceledOn = cancelDate;

                contract.AdjustedTerminationDate = adjustedTermDate;
                contract.AdjustedCancellationDate = adjustedCanDate;

                contract.Oid = planID;
                contract.PlanName = planName;
                contract.PlanType = planType;
                contract.PlanSuffix = planSuffix;
                contract.LineOfBusiness = lineOfBusiness;

                contract.Payor = payor;
                contract.PlanCategory = planCategory;

                if (payorCode != prevPayorCode
                    || planSuffix != prevPlanSuffix)
                {
                    if (planHolder != null)
                    {
                        plans.Add(planHolder);
                    }

                    aPlan = CreateNewInsurancePlan(planCategoryID);
                    planHolder = aPlan;
                }

                prevPayorCode = payorCode;
                prevPlanSuffix = planSuffix;

                planHolder.AddInsurancePlanContract(contract);
            }

            // add the last plan in

            if (planHolder != null)
            {
                plans.Add(planHolder);
            }
            return plans;
        }

        /// <summary>
        /// create and return an insurance plan of the specific type based on the category ID
        /// </summary>
        /// <param name="planCategoryID"></param>
        /// <returns></returns>
        // copy from InsuranceBroker
        private InsurancePlan CreateNewInsurancePlan(long planCategoryID)
        {
            InsurancePlan insurancePlan = null;
            switch (planCategoryID)
            {
                case InsurancePlanCategory.PLANCATEGORY_COMMERCIAL:
                    insurancePlan = new CommercialInsurancePlan();
                    break;
                case InsurancePlanCategory.PLANCATEGORY_GOVERNMENT_OTHER:
                    insurancePlan = new GovernmentOtherInsurancePlan();
                    break;
                case InsurancePlanCategory.PLANCATEGORY_GOVERNMENT_MEDICARE:
                    insurancePlan = new GovernmentMedicareInsurancePlan();
                    break;
                case InsurancePlanCategory.PLANCATEGORY_GOVERNMENT_MEDICAID:
                    insurancePlan = new GovernmentMedicaidInsurancePlan();
                    break;
                case InsurancePlanCategory.PLANCATEGORY_SELF_PAY:
                    insurancePlan = new SelfPayInsurancePlan();
                    break;
                case InsurancePlanCategory.PLANCATEGORY_WORKERS_COMPENSATION:
                    insurancePlan = new WorkersCompensationInsurancePlan();
                    break;
                case InsurancePlanCategory.PLANCATEGORY_OTHER:
                    insurancePlan = new OtherInsurancePlan();
                    break;
                default:
                    break;
            }
            return insurancePlan;
        }

        /// <summary>
        /// return a list of plans for a payor
        /// </summary>
        /// <param name="aPayor"></param>
        /// <param name="facilityOid"></param>
        /// <param name="admitDate"></param>
        /// <param name="planCategory"></param>
        /// <returns></returns>
        private ICollection InsurancePlansForProvider(Payor aPayor, long facilityOid, DateTime admitDate, InsurancePlanCategory planCategory)
        {
            InitFacility(facilityOid);

            ArrayList list = new ArrayList();
            iDB2Command cmd = null;
            SafeReader reader = null;

            // load the plans for a payor
            try
            {
                cmd = this.CommandFor("CALL " + SELECTPLANSFORPAYOR + 
                    "(" + PARAM_PAYORCODE + 
                    "," + PARAM_PLANCATEGORYID +
                    "," + PARAM_FACILITYCODE + 
                    "," + PARAM_ADMIT_DATE +
                    ")",
                    CommandType.Text,
                    this.Facility);

                cmd.Parameters[PARAM_PAYORCODE].Value = (aPayor!=null) ? aPayor.Code : string.Empty;
                cmd.Parameters[PARAM_PLANCATEGORYID].Value = (planCategory == null ? 0 : planCategory.Oid);
                cmd.Parameters[PARAM_FACILITYCODE].Value = this.Facility.Code;
                cmd.Parameters[PARAM_ADMIT_DATE].Value = (admitDate!=DateTime.MinValue) ? decimal.Parse(admitDate.ToString("yyyyMMdd")) : 0;
               
                reader = this.ExecuteReader(cmd);

                list = this.PlanAndContractsFrom(reader, facilityOid);

                foreach (InsurancePlan aPlan in list)
                {
                    InsurancePlanContract aPlanContract = aPlan.GetBestPlanContractFor(admitDate);
                    if (aPlanContract != null)
                    {
                        aPlan.UpdateFromPlanContract(aPlanContract);
                    }
                }

            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                throw BrokerExceptionFactory.BrokerExceptionFrom("Failed to load plans for a payor.", e, c_log);
            }
            finally
            {
                base.Close(reader);
                base.Close(cmd);
            }
            return list;
        }

        /// <summary>
        /// return a list of plans for a broker
        /// </summary>
        /// <param name="aBroker"></param>
        /// <param name="facilityOid"></param>
        /// <param name="admitDate"></param>
        /// <param name="planCategory"></param>
        /// <returns></returns>
        private ICollection InsurancePlansForProvider(Broker aBroker, long facilityOid, DateTime admitDate, InsurancePlanCategory planCategory)
        {
            InitFacility(facilityOid); 

            ArrayList list = new ArrayList();
            iDB2Command cmd = null;
            SafeReader reader = null;

            // load the plans for a payor
            try
            {
                //  .NET Provider implementation
                cmd = this.CommandFor("CALL " + SELECTPLANSFORBROKER +
                    "(" + PARAM_BROKERCODE +
                    "," + PARAM_PLANCATEGORYID +
                    "," + PARAM_FACILITYCODE +
                    "," + PARAM_ADMIT_DATE +
                    ")",
                    CommandType.Text,
                    this.Facility);

                cmd.Parameters[PARAM_BROKERCODE].Value = (aBroker != null) ? aBroker.Code : string.Empty;
                cmd.Parameters[PARAM_PLANCATEGORYID].Value = planCategory == null ? 0 : planCategory.Oid;
                cmd.Parameters[PARAM_FACILITYCODE].Value = this.Facility.Code;
                cmd.Parameters[PARAM_ADMIT_DATE].Value = (admitDate != DateTime.MinValue) ? decimal.Parse(admitDate.ToString("yyyyMMdd")) : 0;

                reader = this.ExecuteReader(cmd);

                list = this.PlanAndContractsFrom(reader, facilityOid);

                foreach (InsurancePlan aPlan in list)
                {
                    InsurancePlanContract aPlanContract = aPlan.GetBestPlanContractFor(admitDate);
                    if (aPlanContract != null)
                    {
                        aPlan.UpdateFromPlanContract(aPlanContract);
                    }
                }

            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                throw BrokerExceptionFactory.BrokerExceptionFrom("Failed to load plans for a Broker.", e, c_log);
            }
            finally
            {
                base.Close(reader);
                base.Close(cmd);
            }
            return list;
        }

        /// <summary>
        /// Get a list of providers (payors and brokers) for a given name
        /// </summary>
        /// <param name="aName"></param>
        /// <param name="facilityOid"></param>
        /// <param name="admitDate"></param>
        /// <param name="planCategory"></param>
        /// <param name="searchOption"></param>
        /// <returns></returns>
        private ICollection ProvidersWith(string aName, long facilityOid, DateTime admitDate,
                                          InsurancePlanCategory planCategory, long searchOption)
        {
            InitFacility(facilityOid);

            ArrayList providers = new ArrayList();
            SafeReader reader = null;
            iDB2Command cmd = null;

            try
            {

                // .NET Provider implementation
                if( planCategory == null || planCategory.Oid == 0 )
                {
                    cmd = this.CommandFor( "CALL " + SELECTALLPAYORBROKERS +
                        "(" + PARAM_PROVIDERNAME +
                        "," + PARAM_PLANCATEGORYID +
                        "," + PARAM_FACILITYCODE +
                        "," + PARAM_SEARCHOPTION +
                        "," + PARAM_ADMIT_DATE +
                        ")",
                        CommandType.Text,
                        this.Facility );

                    cmd.Parameters[PARAM_PROVIDERNAME].Value = StringFilter.mangleName( aName );
                    cmd.Parameters[PARAM_PLANCATEGORYID].Value = planCategory == null ? 0 : planCategory.Oid;
                    cmd.Parameters[PARAM_FACILITYCODE].Value = this.Facility.Code;
                    cmd.Parameters[PARAM_SEARCHOPTION].Value = searchOption;
                    cmd.Parameters[PARAM_ADMIT_DATE].Value = ( admitDate != DateTime.MinValue ) ? decimal.Parse( admitDate.ToString( "yyyyMMdd" ) ) : 0;



                    reader = this.ExecuteReader( cmd );
                }
                else
                {
                    cmd = this.CommandFor( "CALL " + SELECTALLPAYORBROKERSWITH +
                       "(" + PARAM_PROVIDERNAME +
                       "," + PARAM_PLANCATEGORYID +
                       "," + PARAM_FACILITYCODE +
                       "," + PARAM_SEARCHOPTION +
                       "," + PARAM_ADMIT_DATE +
                       ")",
                       CommandType.Text,
                       this.Facility );

                    cmd.Parameters[PARAM_PROVIDERNAME].Value = StringFilter.mangleName( aName );
                    cmd.Parameters[PARAM_PLANCATEGORYID].Value = planCategory == null ? 0 : planCategory.Oid;
                    cmd.Parameters[PARAM_FACILITYCODE].Value = this.Facility.Code;
                    cmd.Parameters[PARAM_SEARCHOPTION].Value = searchOption;
                    cmd.Parameters[PARAM_ADMIT_DATE].Value = ( admitDate != DateTime.MinValue ) ? decimal.Parse( admitDate.ToString( "yyyyMMdd" ) ) : 0;



                    reader = this.ExecuteReader( cmd );
                }

                while (reader.Read())
                {
                    AbstractProvider provider = null;
                    long providerID = (long)reader.GetDecimal(COL_PROVIDERID);
                    string providerName = reader.GetString(COL_PROVIDERNAME);
                    string providerCode = reader.GetString(COL_PROVIDERCODE);
                    string type = reader.GetString(COL_PROVIDERTYPE);
                    string planCount =  reader.GetString( COL_PLANCOUNT );
                    
                    switch (type)
                    {
                        case "P":
                            Payor payor = null;
                            if (providerCode == OTHER_PAYOR_CODE)
                            {
                                payor = new OtherPayor();
                            }
                            else
                            {
                                payor = new Payor();
                            }
                            payor.Oid = providerID;
                            payor.Name = providerName;
                            payor.Code = providerCode;
                            payor.NumberOfActivePlans = planCount ;
                            payor.PlansLoader = new PlansLoader(payor, facilityOid, admitDate, planCategory);
                            provider = payor;
                            break;

                        case "B":
                            Broker broker = new Broker();
                            broker.Oid = providerID;
                            broker.Name = providerName;
                            broker.Code = providerCode;
                            broker.NumberOfActivePlans =  planCount;
                            broker.PlansLoader = new PlansLoader(broker, facilityOid, admitDate, planCategory);
                            provider = broker;
                            break;
                        default:
                            break;
                    }
                    if (provider != null)
                    {
                        providers.Add(provider);
                    }
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                string msg = "Failed to execute ProvidersWith for " + aName;
                throw BrokerExceptionFactory.BrokerExceptionFrom(msg, e, c_log);
            }
            finally
            { 
                base.Close( reader );
                base.Close( cmd );
            }

            return providers;
        }

        /// <summary>
        /// read employer from reader
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="facility"></param>
        /// <returns></returns>
        private EmployerProxy EmployerFrom(SafeReader reader, Facility facility)
        {
            EmployerProxy employerProxy = null;

            if (!reader.IsDBNull(COL_EMPLOYERID))
            {
                long oid = reader.GetInt64(COL_EMPLOYERID);
                string name = reader.GetString(COL_EMPLOYERNAME);
                string nationalId = reader.GetString(COL_NATIONALID);
                long employerCode = reader.GetInt64(COL_EMPLOYERCODE);

                employerProxy = new EmployerProxy(oid, ReferenceValue.NEW_VERSION, name, nationalId, employerCode, facility);
            }

            return employerProxy;

        }

        /// <summary>
        /// read address from reader
        /// </summary>
        /// <param name="facilityID"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        private Address AddressFrom(long facilityID, SafeReader reader)
        {
            Address aAddress = null;
            IAddressBroker addressBroker = BrokerFactory.BrokerOfType<IAddressBroker>();

            // if there is no data return a null

            string address1 = reader.GetString(COL_ADDRESS1);
            string address2 = reader.GetString(COL_ADDRESS2);
            string city = reader.GetString(COL_CITY);
            string postalCode = reader.GetString(COL_POSTALCODE);
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
                country = addressBroker.CountryWith( facilityID , countryCode );
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

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public InsurancePBARBroker()
        {
        }
        public InsurancePBARBroker(string cxnString)
            : base( cxnString )
        {
        }
        public InsurancePBARBroker(IDbTransaction txn )
            : base( txn )
        {
        }
        #endregion

        #region Data Elements

        private static readonly ILog c_log = 
			LogManager.GetLogger( typeof( InsurancePBARBroker ) );

        #endregion

        #region Constants

        private const string OTHER_PAYOR_CODE = "OTH";
                         

        private const long
            SEARCH_OPTION_BEGINSWITH = 0L,
            SEARCH_OPTION_CONTAINS = 1L;

        private const string
            SP_BILLING_INFORMATIONS_FOR = "SELECTBILLINGINFOSFORPLANID",
            SELECTPLANFORPAYORPLANCODE = "SELECTPLANFORPAYORPLANCODE",
            SELECTPLANFORPAYORPLANCODEWITHDATE = "SELECTPLANFORPAYORPLANCODEWITHDATE",
            SELECTALLINSURANCECATEGORIES = "SELECTALLPLANCATEGORIES",
            SELECTALLPLANTYPES              = "SELECTALLPLANTYPES",
            SELECTIPASBYCODE                = "SELECTIPABYCODE",
            SELECTIPASBYNAME                = "SELECTIPASBYNAME",
            SELECTPLANSFORPAYOR             = "SELECTPLANSFORPAYOR",
            SELECTPLANSFORBROKER            = "SELECTPLANSFORBROKER",
            SELECTPLANSFORCOVEREDGROUP      = "SELECTPLANSFORCOVEREDGROUP",
            SELECTCOVEREDGROUPSMATCHING     = "SELECTCOVEREDGROUPSWITH",
            SELECTCOVEREDGROUPSFOR          = "SELECTCOVEREDGROUPSFOR",
            SELECTALLPAYORBROKERS           = "SELECTALLPAYORBROKERS",
            SELECTALLPAYORBROKERSWITH        = "SELECTALLPAYORBROKERSWITH",
            SELECTPLANFINCLASSESWITH        = "SELECTPLANFINCLASSESWITH",
            SELECTPAYORFINCLASSESWITH       = "SELECTPAYORFINCLASSESWITH",
            SP_SELECTISVALIDHSVPLANMAPPING  = "SELECTISVALIDHSVPLANMAPPING";

        private const string
            PARAM_PLANCD            = "@P_PLANCD",
            PARAM_EFFECTIVEDATE     = "@P_EFFECTIVEDATE",
            PARAM_APPROVALDATE      = "@P_APPROVALDATE",
            PARAM_IPACODE           = "@P_IPACODE",
            PARAM_CLINICCODE        = "@P_CLINICCODE",
            PARAM_IPANAME           = "@P_IPANAME",
            PARAM_PAYORCODE         = "@P_PAYORCODE",
            PARAM_PLANCATEGORYID    = "@P_PLANCATEGORYID",
            PARAM_ADMIT_DATE        = "@P_ADMITDATE",
            PARAM_BROKERCODE        = "@P_BROKERCODE",
            PARAM_EMPLOYERCODE      = "@P_EMPLOYERCODE",
            PARAM_EMPLOYERNAME      = "@P_EMPLOYERNAME",
            PARAM_COVEREDGROUPNAME  = "@P_COVEREDGROUPNAME",
            PARAM_PROVIDERNAME      = "@P_INSNAME",
            PARAM_SEARCHOPTION      = "@P_SEARCHOPTION",
            PARAM_PLANSUFFIX        = "@P_SUFFIX",
            PARAM_SERVICECD         = "@P_SERVICECD",
            PARAM_SERVICECOUNT      = "@O_SERVICECOUNT";

        private const string

            COL_ADDRESS1 = "ADDRESS1",
            COL_ADDRESS2 = "ADDRESS2",
            COL_CITY = "CITY",
            COL_POSTALCODE = "POSTALCODE",
            COL_STATECODE = "STATECODE",
            COL_COUNTRYCODE = "COUNTRYCODE",

            COL_PHONEAREACODE = "PHONEAREACODE",
            COL_PHONENUMBER = "PHONENUMBER",
            COL_EMAILADDRESS = "EMAILADDRESS",

            COL_CONTACTDESCRIPTION = "CONTACTDESCRIPTION",
            COL_BILLINGCONAME = "BILLINGCONAME",
            COL_BILLINGNAME = "BILLINGNAME",
            COL_PLANAREA = "BILLINGAREA",

            COL_PLANID = "PLANID",
            COL_PLANSUFFIX = "PLANSUFFIX",
            COL_PLANNAME = "PLANNAME",
            COL_PLANTYPECODE = "PLANTYPECODE",
            COL_PLANEFFECTIVEDATE = "PLANEFFECTIVEDATE",
            COL_PLANTERMINATIONDATE = "PLANTERMINATIONDATE",
            COL_PLANAPPROVALDATE = "PLANAPPROVALDATE",
            COL_PLANCANCELDATE = "PLANCANCELDATE",
            COL_TERMPURGEDAYS = "TERMPURGEDAYS",
            COL_CANCELPURGEDAYS = "CANCELPURGEDAYS",
            COL_TERMGRACEDAYS = "TERMGRACEDAYS",
            COL_CANCELGRACEDAYS = "CANCELGRACEDAYS",
            COL_ADJUSTEDTERMDATE = "ADJUSTEDTERMINATIONDATE",
            COL_ADJUSTEDCANCELDATE = "ADJUSTEDCANCELLATIONDATE",
            COL_PLANCATEGORYID = "PLANCATEGORYID",
            COL_PLANCATEGORYDESCRIPTION = "PLANCATEGORYDESCRIPTION",
            COL_LOBDESCRIPTION = "LOBDescription",
            COL_EXPECTEDASSOCIATEDNUMBERLABEL = "EXPECTEDASSOCIATEDNUMBERLABEL",

            COL_PAYORID = "PAYORID",
            COL_PAYORNAME = "PAYORNAME",
            COL_PAYORCODE = "PAYORCODE",

            COL_IPAID = "IPAID",
            COL_IPACODE = "IPACODE",
            COL_IPANAME = "IPANAME",
            COL_CLINICCODE = "CLINICCODE",
            COL_CLINICNAME = "CLINICNAME",

            COL_COVEREDGROUPID = "COVEREDGROUPID",
            COL_EMPLOYERCODE = "EMPLOYERCODE",
            COL_GROUPCOVERED = "GROUPCOVERED",

            COL_PROVIDERID = "PROVIDERID",
            COL_PROVIDERNAME = "PROVIDERNAME",
            COL_PROVIDERCODE = "PROVIDERCODE",
            COL_PROVIDERTYPE = "PROVIDERTYPE",
            COL_PLANCOUNT = "ACTIVEPLANCOUNT",

            COL_EMPLOYERID = "EMPLOYERID",
            COL_EMPLOYERNAME = "NAME",
            COL_NATIONALID = "NATIONALID",
            COL_FINANCIALCLASSCODE = "FINANCIALCLASSCODE";

            
        #endregion
    }
}
