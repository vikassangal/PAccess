using ClosedXML.Excel;
 
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Persistence;
using System;
using System.Configuration;
using System.Data;
//using System.Data.SqlClient;
 
using System.IO;
  
using System.Threading; 
using Extensions.DB2Persistence;
using log4net;
using IBM.Data.DB2.iSeries;  

namespace PatientAccess
{
    public partial class CovideBulkUpload : System.Web.UI.Page
    {
        private DataTable dtTable;
        private DataTable dtResult;
        private int incrementCount = 100;
        private int startCount = 0;
        private int endCount = 0;
        private bool firstTime = true;

        private long MRN;
      

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void bulkUpload_Click(object sender, EventArgs e)
        {
            if (FileUpload1.HasFile)
            {
                string FileName = Path.GetFileName(FileUpload1.PostedFile.FileName);
                string Extension = Path.GetExtension(FileUpload1.PostedFile.FileName);
                string FolderPath = ConfigurationManager.AppSettings["FolderPath"];
                //If file is not in excel format then return  
                if (Extension != ".xls" && Extension != ".xlsx" && Extension != ".csv")
                { return; }
                string FilePath = Server.MapPath(FolderPath + FileName);
                FileUpload1.SaveAs(FilePath);
                bulkUpload.Enabled = false;
                Import_To_Grid(FilePath, Extension, "yes", FileName);
            }
        }

        private void Import_To_Grid(string FilePath, string Extension, string isHDR, string fileName)
        {
            DataTable dt = new DataTable();
            DataTable dtFailed = new DataTable();
            int TotalSuccessTrasaction = 0;
            var watch = new System.Diagnostics.Stopwatch();
            dtResult = new DataTable();
            dtResult.Columns.Add("Facility ID");
            dtResult.Columns.Add("LastName");
            dtResult.Columns.Add("FirstName");
            dtResult.Columns.Add("DOB");
            dtResult.Columns.Add("MRN");
            dtResult.Columns.Add("Account Number");
            dtResult.Columns.Add("Tenet EEID");
            dtResult.Columns.Add("Status");

            #region Reading Excel file
            bool firstRow = true;
            int x;
            if (Extension.ToLower() == ".csv")
            {
                // Read data from csv file
                firstRow = true;
                foreach (string row in File.ReadLines(FilePath))
                {
                    if (firstRow)
                    {
                        if (!string.IsNullOrEmpty(row.ToString()))
                        {
                            foreach (string cellVal in row.Split(','))
                            {
                                dt.Columns.Add(cellVal.ToString());
                                dtFailed.Columns.Add(cellVal.ToString());
                            }
                        }
                        firstRow = false;
                    }
                    else
                    {
                        x = 0;
                        if (!string.IsNullOrEmpty(row.ToString()))
                        {
                            dt.Rows.Add();
                            foreach (string cellVal in row.Split(','))
                            {
                                if (x <= dt.Columns.Count - 1)
                                {
                                    dt.Rows[dt.Rows.Count - 1][x] = cellVal.ToString();
                                    x++;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                using (XLWorkbook workBook = new XLWorkbook(FilePath))
                {
                    //Read the first Sheet from Excel file.
                    IXLWorksheet workSheet = workBook.Worksheet(1);

                    //Loop through the Worksheet rows.
                    foreach (IXLRow row in workSheet.Rows())
                    {
                        //Use the first row to add columns to DataTable.
                        if (firstRow)
                        {
                            foreach (IXLCell cell in row.Cells())
                            {
                                if (!string.IsNullOrEmpty(cell.Value.ToString()))
                                {
                                    dt.Columns.Add(cell.Value.ToString());
                                }
                            }
                            firstRow = false;
                        }
                        else
                        {
                            //Add rows to DataTable.
                            dt.Rows.Add();
                            int i = 0;
                            foreach (IXLCell cell in row.Cells(1, dt.Columns.Count))
                            {
                                dt.Rows[dt.Rows.Count - 1][i] = cell.Value.ToString();
                                i++;
                            }
                        }
                    }
                }
            }

            #endregion Reading Excel file
            int TotalPatientCount = 0;
            if (Extension.ToLower() == ".csv")
            {
                TotalPatientCount = dt.Rows.Count;
            }
            else
            {
                TotalPatientCount = dt.Rows.Count - 1;
            }

            int totalBatch = TotalPatientCount / incrementCount;
            for (int j = 0; j <= totalBatch; j++)
            {
                if (j > 0)
                {
                    Thread.Sleep(60000);
                }
                dtTable = DataTableSplitter(dt, j);
                #region Foreach Loop
                foreach (DataRow dr in dtTable.Rows)
                {
                    DataRow drResult = dtResult.NewRow();
                    try
                    {
                        if (dr["Employee Last Name"].ToString() != "" && dr["Employee First Name"].ToString() != "" && dr["HSPCode - Work Fac"].ToString() != "")
                        {
                            string firstNameString = dr["Employee First Name"].ToString();
                            string lastNameString = dr["Employee Last Name"].ToString();
                            int monthfromExcel = 0;
                            int yearfromExcel = 0;
                            int datefromExcel = 0;
                            if (Extension != ".csv")
                            {
                                monthfromExcel = Convert.ToDateTime(dr["Employee DOB"].ToString()).Month;
                                yearfromExcel = Convert.ToDateTime(dr["Employee DOB"].ToString()).Year;
                                datefromExcel = Convert.ToDateTime(dr["Employee DOB"].ToString()).Day;
                            }
                            else
                            {
                                string[] dob = dr["Employee DOB"].ToString().Split('/');

                                monthfromExcel = Convert.ToInt32(dob[0]);
                                datefromExcel = Convert.ToInt32(dob[1]);
                                yearfromExcel = Convert.ToInt32(dob[2].ToString().Substring(0, 4));
                            }

                            string ssn = "999-99-9999";
                            DateTime DOB = new DateTime(yearfromExcel, monthfromExcel, datefromExcel);

                            string firstName = String.Empty;
                            if (firstNameString.Trim().Length >= 13)
                            {
                                firstName = firstNameString.Substring(0, 13);
                            }
                            else
                            {
                                firstName = firstNameString;
                            }
                            string lastName = String.Empty;
                            if (lastNameString.Trim().Length >= 25)
                            {
                                lastName = lastNameString.Substring(0, 25);
                            }
                            else
                            {
                                lastName = lastNameString;
                            }
                            var hsp = dr["HSPCode - Work Fac"].ToString();
                            IFacilityBroker facilityBroker =
                            BrokerFactory.BrokerOfType<IFacilityBroker>();
                            Facility facility = facilityBroker.FacilityWith(hsp);
                            if (facility == null)
                            {
                                return;
                            }

                            // Checking MRN is Exists or not
                            CovidServiceBroker covidServiceBroker = new CovidServiceBroker();
                            MRN = covidServiceBroker.GetCovidEmployeeMRN(dr, facility);

                         

                            Name name = new Name(firstName.ToUpper(), lastName.ToUpper(), "");

                            Patient covidPatient = new Patient();
                            Account covidPatientAccount = new Account();

                            covidPatient.Name = name;
                            covidPatient.FirstName = firstName.ToUpper();
                            covidPatient.LastName = lastName.ToUpper();
                            covidPatient.DateOfBirth = DOB;
                            covidPatient.SocialSecurityNumber = new SocialSecurityNumber(ssn);
                            covidPatient.IsNew = true;
                            var activity = new RegistrationActivity();
                            activity.AppUser = PatientAccess.Domain.User.GetCurrent();
                            activity.AppUser.WorkstationID = "Vaccine";
                            activity.AppUser.PBAREmployeeID = "Vaccine";
                            covidPatientAccount.Activity = activity;

                            covidPatientAccount.Facility = facility;

                            string employeeGenderString = dr["Employee Gender"].ToString();
                            var DemographicsBroker = BrokerFactory.BrokerOfType<IDemographicsBroker>();
                            switch (employeeGenderString.Trim().ToUpper())
                            {
                                case "M" :
                                    employeeGenderString = "M";
                                    break;
                                case "F":
                                    employeeGenderString = "F";
                                    break;                               
                                default:
                                    employeeGenderString = "U";
                                    break;
                            }
                            var employeeGender = DemographicsBroker.GenderWith(facility.Oid, employeeGenderString);

                            covidPatient.Sex = employeeGender;
                            var language = DemographicsBroker.LanguageWith(facility.Oid, "EN");
                            covidPatient.Language = language;

                            string employeeRaceString = dr["Employee Race"].ToString();
                            #region Race
                            string employeeRaceCode;
                            switch (employeeRaceString.ToUpper())
                            {
                                case "WHITE":
                                    employeeRaceCode = "1";
                                    break;
                                case "BLACK OR AFRICAN AMERICAN":
                                    employeeRaceCode = "2";
                                    break;
                                case "NATIVE HAWAIIAN OR OTHER PACIFIC ISLANDER":
                                    employeeRaceCode = "3";
                                    break;
                                case "NATIV AMER":
                                    employeeRaceCode = "4";
                                    break;
                                case "AMERICAN INDIAN or ALASKAN NATIVE":
                                    employeeRaceCode = "6";
                                    break;
                                case "ASIAN":
                                    employeeRaceCode = "5";
                                    break;
                                case "HISPANIC OR LATINO":
                                    employeeRaceCode = "6";
                                    break;
                                case "OTHER":
                                    employeeRaceCode = "6";
                                    break;
                                case "DECLINED":
                                    employeeRaceCode = "D";
                                    break;
                                case "UNKNOWN":
                                    employeeRaceCode = "7";
                                    break;
                                default:
                                    employeeRaceCode = "7";
                                    break;
                            }

                            covidPatient.Race.Code = employeeRaceCode;
                            #endregion
                            #region Ethenticity
                            string employeeEthnicityString = dr["Employee Ethnicity"].ToString();
                            string employeeEthnicityCode;
                            switch (employeeEthnicityString.ToUpper())
                            {
                                case "DECLINED TO SPECIFY":
                                    employeeEthnicityCode = "D";
                                    break;
                                case "HISPANIC OR LATINO":
                                    employeeEthnicityCode = "1";
                                    break;
                                case "NON-HISPANIC":
                                    employeeEthnicityCode = "2";
                                    break;
                                case "WHITE":
                                    employeeEthnicityCode = "2";
                                    break;
                                case "ASIAN":
                                    employeeEthnicityCode = "2";
                                    break;
                                case "BLACK OR AFRICAN AMERICAN":
                                    employeeEthnicityCode = "2";
                                    break;
                                case "UNKNOWN":
                                    employeeEthnicityCode = "3";
                                    break;
                                default:
                                    employeeEthnicityCode = "3";
                                    break;
                            }

                            covidPatient.Ethnicity.Code = employeeEthnicityCode;
                            #endregion
                            #region Add MedicalDirector
                            long medicalDirectorID = 0;
                            try
                            {
                                medicalDirectorID = Convert.ToInt32(dr["Employee Health Medical Director"]);
                            }
                            catch
                            {
                            }

                            IPhysicianBroker PhysicianBroker = BrokerFactory.BrokerOfType<IPhysicianBroker>();
                           
                            PhysicianRelationship ppref = PhysicianBroker.BuildReferringPhysicianRelationship(facility.Oid, medicalDirectorID);
                            covidPatientAccount.AddPhysicianRelationship(ppref);

                            PhysicianRelationship ppad = PhysicianBroker.BuildAdmittingPhysicianRelationship(facility.Oid, medicalDirectorID);
                            covidPatientAccount.AddPhysicianRelationship(ppad);


                            #endregion

                            #region Add ContactPoint
                            string streetString = dr["Employee Street Address"].ToString();
                            string street = String.Empty;
                            if (streetString.Trim().Length >= 25)
                            {
                                street = streetString.Substring(0, 25);
                            }
                            else
                            {
                                street = streetString;
                            }

                            string cityString = dr["Employee City"].ToString();
                            string city = String.Empty;
                            if (cityString.Trim().Length >= 10)
                            {
                                city = cityString.Substring(0, 10);
                            }
                            else
                            {
                                city = cityString;
                            }

                            string stateString = dr["Employee State"].ToString();
                            string state = String.Empty;
                            if (stateString.Trim().Length >= 2)
                            {
                                state = stateString.Substring(0, 2);
                            }
                            else
                            {
                                state = stateString;
                            }

                            string zipcodeString = dr["Employee Zipcode"].ToString();
                            string zip = String.Empty;

                            if (zipcodeString.Trim().Length >= 6)
                            {
                                zip = zipcodeString.Substring(0, 6);
                            }
                            else
                            {
                                zip = zipcodeString;
                            }
                            string countryString = dr["Employee Country"].ToString();
                            var country = "USA";
                            switch (countryString.Trim().ToUpper())
                            {
                                case "USA":
                                    country = "USA";
                                    break;
                                case "US":
                                    country = "USA";
                                    break;
                                case "CAN":
                                    country = "CAN";
                                    break;
                                case "CA":
                                    country = "CAN";
                                    break;
                                default:
                                    country = "USA";
                                    break;
                            }

                            string phoneNumber = dr["Employee Phone Number Home"].ToString().Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
                            string cellNumber = dr["Employee Phone Number Cell"].ToString().Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
                            string emailAddress = dr["Employee Email Address"].ToString();

                            var i_AddressBroker = BrokerFactory.BrokerOfType<IAddressBroker>();
                            Address employeeAddress = new Address();

                            employeeAddress.Address1 = street.ToUpper();
                            employeeAddress.City = city.ToUpper();
                            employeeAddress.State = new State() { Code = state.ToUpper() };
                            employeeAddress.Country = new Country() { Code = country };
                            employeeAddress.ZipCode = new ZipCode(zip);
                            PhoneNumber employeePhone = new PhoneNumber();
                            if (phoneNumber != "" && phoneNumber.Length >=10)
                            {
                                string areaCodePhone = phoneNumber.Substring(0, 3);
                                string phoneNo = phoneNumber.Substring(3, 7);
                                employeePhone = new PhoneNumber() {Number = phoneNo, AreaCode = areaCodePhone};
                            }

                            PhoneNumber employeeCell = new PhoneNumber();
                            if (cellNumber != "" && cellNumber.Length >= 10)
                            {
                                string areaCodeCell = cellNumber.Substring(0, 3);
                                string cellNo = cellNumber.Substring(3, 7);
                                employeeCell = new PhoneNumber() { Number = cellNo, AreaCode = areaCodeCell };
                            }
                            EmailAddress email = new EmailAddress(emailAddress);

                            ContactPoint mailingContactPoint = new ContactPoint(
                                employeeAddress, employeePhone, employeeCell, email, TypeOfContactPoint.NewMailingContactPointType());
                            covidPatient.AddContactPoint(mailingContactPoint);

                            ContactPoint mobileContactPoint = new ContactPoint(
                                new Address(), employeeCell, employeeCell, new EmailAddress(), TypeOfContactPoint.NewMobileContactPointType());
                            covidPatient.AddContactPoint(mobileContactPoint);
                            #endregion Add ContactPoint

                            var hcBroker = BrokerFactory.BrokerOfType<IHospitalClinicsBroker>();
                            string clinicCode = dr["Employee Health Location - Clinic Code"].ToString();
                            HospitalClinic clinic = hcBroker.HospitalClinicWith(facility.Oid, clinicCode);
                            if (clinic != null)
                            {
                                covidPatientAccount.HospitalClinic = clinic;
                            }
                            else
                            {
                                covidPatientAccount.HospitalClinic = new HospitalClinic();
                            }

                            FinancialClass employeeFC = new FinancialClass() { Code = "91" };

                            HospitalService employeeHSV = new HospitalService() { Code = "EH" };
                            covidPatientAccount.FinancialClass = employeeFC;
                            covidPatientAccount.HospitalService = employeeHSV;

                            covidPatientAccount.KindOfVisit = VisitType.NonPatient;
                            var InsuranceBroker = BrokerFactory.BrokerOfType<IInsuranceBroker>();
                            var currentPlan = InsuranceBroker.PlanWith("TCVNQ", facility.Oid);

                            covidPatientAccount.Patient = covidPatient;
                            covidPatientAccount.Patient.IsNew = true;

                            Coverage coverage = null;
                            var relationShipTypeBroker = BrokerFactory.BrokerOfType<IRelationshipTypeBroker>();
                            RelationshipType relationshipType = relationShipTypeBroker.RelationshipTypeWith(
                                      facility.Oid,
                                      RelationshipType.RELATIONSHIP_CODE_SELF);
                            if (currentPlan != null)
                            {
                                //Insured
                                Insured insured = covidPatientAccount.Patient.CopyAsInsured();

                                relationshipType = relationShipTypeBroker.RelationshipTypeWith(
                                  facility.Oid,
                                  RelationshipType.RELATIONSHIP_CODE_SELF);
                                Relationship relationship = new Relationship(relationshipType, covidPatientAccount.Patient.GetType(), insured.GetType());
                                covidPatientAccount.Patient.AddRelationship(relationship);

                                coverage = Coverage.CoverageFor(currentPlan, insured);
                                coverage.CoverageOrder = new CoverageOrder(CoverageOrder.PRIMARY_OID, "PRIMARY");
                                covidPatientAccount.Insurance.AddCoverage(coverage);

                            }

                            covidPatientAccount.Patient.IsNew = true;
                            // Guarantor
                            Guarantor guarantor = covidPatientAccount.Patient.CopyAsGuarantor();
                            covidPatientAccount.Guarantor = guarantor;

                            relationshipType = relationShipTypeBroker.RelationshipTypeWith(
                                    facility.Oid,
                                    RelationshipType.RELATIONSHIP_CODE_SELF);
                            covidPatientAccount.GuarantorIs(guarantor, relationshipType);
                            // Employer
                            string EmprStreetString = dr["Work Location Address"].ToString();
                            string EmprCity = dr["Work Location City"].ToString();
                            string EmprState = dr["Work Location State"].ToString().Replace("??", "");
                            string EmprCountry = dr["Employee Country"].ToString();
                            string EmprZipcode = dr["Work Location Zip"].ToString();
                            Address emprAddress =
                                new Address(EmprStreetString, "", EmprCity, new ZipCode(EmprZipcode), new State() { Code = state }, new Country() { Code = EmprCountry });
                            Employment emp = new Employment();
                            Employer empr = new Employer { Name = "Tenet Covid" };
                            empr.PartyContactPoint.Address = emprAddress;
                            emp.Employer = empr;
                            emp.Status = EmploymentStatus.NewFullTimeEmployed();
                            emp.EmployeeID = dr["Tenet EEID"].ToString();
                            covidPatientAccount.Patient.Employment = emp;

                            covidPatientAccount.AdmitDate = DateTime.Now;
                            covidPatientAccount.IsNew = true;
                            covidPatientAccount.Patient.IsNew = true;
                            covidPatientAccount.Facility = facility;
                            covidPatientAccount.Patient.MedicalRecordNumber = MRN;
                            AccountPBARBroker accountBroker = new AccountPBARBroker();
                            AccountSaveResults saveresult = new AccountSaveResults();
                            //Save Logic
                            saveresult = accountBroker.Save(covidPatientAccount, new RegistrationActivity());
                            if (MRN == 0)
                            {
                                covidServiceBroker.InsertEmployeeRecord(dr, covidPatientAccount);

                            }
                            TotalSuccessTrasaction += 1;
                            drResult[0] = dr["HSPCode - Work Fac"].ToString();
                            drResult[1] = dr["Employee Last Name"].ToString();
                            drResult[2] = dr["Employee First Name"].ToString();
                            drResult[3] = dr["Employee DOB"].ToString();
                            drResult[4] = covidPatientAccount.Patient.MedicalRecordNumber;
                            drResult[5] = covidPatientAccount.AccountNumber;
                            drResult[6] = dr["Tenet EEID"].ToString();
                            drResult[7] = "Success";
                            dtResult.Rows.Add(drResult);
                            dtResult.AcceptChanges();
                        }
                    }
                    catch (Exception ex)
                    {
                        drResult[0] = dr["HSPCode - Work Fac"].ToString();
                        drResult[1] = dr["Employee Last Name"].ToString();
                        drResult[2] = dr["Employee First Name"].ToString();
                        drResult[3] = dr["Employee DOB"].ToString();
                        drResult[4] = 0;
                        drResult[5] = 0;
                        drResult[6] = dr["Tenet EEID"].ToString();
                        drResult[7] = ex.Message;
                        dtResult.Rows.Add(drResult);
                        dtResult.AcceptChanges();
                    }
                }

                #endregion

                lbl_msg.Text = "Total Successful Transaction : " + TotalSuccessTrasaction.ToString();
            }

            //Exporting result into excelsheet 
            ExportToExcel(dtResult, fileName);
            bulkUpload.Enabled = true;
        }

        private DataTable DataTableSplitter(DataTable sourceDataTable, int batchnumber)
        {

            DataTable dtSubset = new DataTable();
            dtSubset = sourceDataTable.Clone();
            int totalPatientCount = sourceDataTable.Rows.Count;
            try
            {
                if (batchnumber == 0 && firstTime || (incrementCount >= totalPatientCount))
                {
                    endCount = incrementCount;
                    firstTime = false;
                }
                else
                {
                    if (endCount + incrementCount < totalPatientCount)
                    {
                        endCount = endCount + incrementCount;
                    }
                    else
                    {
                        endCount = totalPatientCount;
                    }
                }
                for (int i = startCount; i <= endCount; i++)
                {
                    dtSubset.Rows.Add(sourceDataTable.Rows[i].ItemArray);
                }
                startCount = endCount + 1;
            }
            catch
            {

            }
            return dtSubset;

        }

        private void ExportToExcel(DataTable dt, string fileName)
        {
            string attachment = "attachment; filename= " + fileName + "_Result.xls";
            Response.ClearContent();
            Response.AddHeader("content-disposition", attachment);
            Response.ContentType = "application/vnd.ms-excel";
            string tab = "";
            foreach (DataColumn dc in dt.Columns)
            {
                Response.Write(tab + dc.ColumnName);
                tab = "\t";
            }
            Response.Write("\n");
            int i;
            foreach (DataRow dr in dt.Rows)
            {
                tab = "";
                for (i = 0; i < dt.Columns.Count; i++)
                {
                    Response.Write(tab + dr[i].ToString());
                    tab = "\t";
                }
                Response.Write("\n");
            }
            Response.End();
        }

    }

    [Serializable]
    public class CovidServiceBroker : AbstractPBARBroker
    {
        private const string DBPROCEDURE_EMPLOYEEHEALHBULKUPLOAD = "PACCESS.EmployeeHealthBulkUploadInsert";
        private const string DBPROCEDURE_GETCOVIDEMPLOYEEMRN = "PACCESS.GETCOVIDEMPLOYEEMRN";
        private static readonly ILog c_log =
        LogManager.GetLogger(typeof(CovideBulkUpload));

        public const string PARAM_HSPCODEWORKFAC = "@P_HSPCODEWORKFAC";
        public const string PARAM_EMPLOYEELASTNAME = "@P_EMPLOYEELASTNAME";
        public const string PARAM_EMPLOYEEFIRSTNAME = "@P_EMPLOYEEFIRSTNAME";
        public const string PARAM_EMPLOYEEDOB = "@P_EMPLOYEEDOB";
        public const string PARAM_EMPLOYEEGENDER = "@P_EMPLOYEEGENDER";
        public const string PARAM_MRN = "@P_MRN";
        public const string PARAM_EMPLOYEETENETID = "@EMPLOYEETENETID";
        public void InsertEmployeeRecord(DataRow dr, Account anAccount)
        {
            SafeReader reader = null;
            iDB2Command cmd = null;
            try
            {

                cmd = this.CommandFor("CALL " + DBPROCEDURE_EMPLOYEEHEALHBULKUPLOAD +

                        "(" + PARAM_HSPCODEWORKFAC +
                    "," + PARAM_EMPLOYEELASTNAME +
                    "," + PARAM_EMPLOYEEFIRSTNAME +
                    "," + PARAM_EMPLOYEEDOB +
                    "," + PARAM_EMPLOYEEGENDER +
                    "," + PARAM_MRN +
                    "," + PARAM_EMPLOYEETENETID +
                    ")",
                    CommandType.Text,
                    anAccount.Facility);


                cmd.Parameters[PARAM_HSPCODEWORKFAC].Value = dr["HSPCode - Work Fac"].ToString().Trim().ToUpper();
                cmd.Parameters[PARAM_EMPLOYEELASTNAME].Value = dr["Employee Last Name"].ToString().Trim().ToUpper();
                cmd.Parameters[PARAM_EMPLOYEEFIRSTNAME].Value = dr["Employee First Name"].ToString().Trim().ToUpper();
                cmd.Parameters[PARAM_EMPLOYEEDOB].Value = Convert.ToDateTime(dr["Employee DOB"]);
                cmd.Parameters[PARAM_EMPLOYEEGENDER].Value = dr["Employee Gender"].ToString().Trim().ToUpper();
                cmd.Parameters[PARAM_MRN].Value = anAccount.Patient.MedicalRecordNumber;
                cmd.Parameters[PARAM_EMPLOYEETENETID].Value = dr["Tenet EEID"].ToString().Trim().ToUpper();

                reader = this.ExecuteReader(cmd);

                while (reader.Read())
                {
                }

            }
            catch
            {
                string msg = "EMPLOYEEHEALTHBULKUPLOADINSERT failed for Facility:" + anAccount.Facility.Oid +
                    " MRN:" + anAccount.Patient.MedicalRecordNumber;

            }
            finally
            {
                Close(reader);
                Close(cmd);
            }
        }

      
        public long GetCovidEmployeeMRN(DataRow dr , Facility facility)
        {
          

            long mrn = 0;
            const string COL_MRN = "MRN";
            SafeReader reader = null;
            iDB2Command cmd = null;
            
            try
            {
                IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
                cmd = this.CommandFor("CALL " + DBPROCEDURE_GETCOVIDEMPLOYEEMRN +

                        "(" + PARAM_HSPCODEWORKFAC +               
                    "," + PARAM_EMPLOYEETENETID +
                    ")",
                    CommandType.Text,
                    facility);

                cmd.Parameters[PARAM_HSPCODEWORKFAC].Value = facility.Code;              
                cmd.Parameters[PARAM_EMPLOYEETENETID].Value = dr["Tenet EEID"].ToString().Trim().ToUpper();
                reader = this.ExecuteReader(cmd);
                while (reader.Read())
                {
                    mrn = reader.GetInt64(COL_MRN);
                }

                return mrn;
            }
            catch (Exception ex)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom(ex, c_log);
            }
            
        }

    }
}
