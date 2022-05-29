
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Persistence;
using System;
using System.Configuration;
using System.Data;
using System.IO; 
using Extensions.DB2Persistence;
using log4net;
using IBM.Data.DB2.iSeries;
using ClosedXML.Excel;
using PatientAccess;
using System.Threading;
using PatientAccess.Domain.Auditing.FusNotes;


namespace PatientAccess
{
    public partial class PatientMyTurnUpload : System.Web.UI.Page
    {
        private DataTable dtTable;
        //private DataTable dtResult;
        private readonly int incrementCount = 100;
        private int startCount = 0;
        private int endCount = 0;
        private bool firstTime = true;
        private long MRN;
        private long MPIPatientMRN;

        private Facility facility = null;
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void bulkUpload_Click(object sender, EventArgs e)
        {
            if (FileUpload1.HasFile)
            {
                i_OriginBroker = BrokerFactory.BrokerOfType<IOriginBroker>();

                string FileName = Path.GetFileName(FileUpload1.PostedFile.FileName);
                string Extension = Path.GetExtension(FileUpload1.PostedFile.FileName);
                string FolderPath = ConfigurationManager.AppSettings["FolderPath"];
                //If file is not in excel format then return  
                if (Extension != ".xls" && Extension != ".xlsx" && Extension != ".csv")
                {
                    return;
                }
                string FilePath = Server.MapPath(FolderPath + FileName);
                FileUpload1.SaveAs(FilePath);
                bulkUpload.Enabled = false;
                Import_To_Grid(FilePath, Extension, FileName);
            }
        }

        private void Import_To_Grid(string FilePath, string Extension, string fileName)
        {
            DataTable dt = new DataTable();
            DataTable dtFailed = new DataTable();
            int TotalSuccessTrasaction = 0;
            GenerateResultSheet generateResultSheet = new GenerateResultSheet();
            GenerateChargeSheet generateChargeSheet = new GenerateChargeSheet();
            
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
                    //Read the appropriate WorkSheet from Excel file.
                    IXLWorksheet workSheet = workBook.Worksheet(3);

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
                    //DataRow drResult = dtResult.NewRow();
                    try
                    {
                        var lastNameString = dr["Person Account: Last Name"].ToString().Trim().ToUpper();
                        var firstNameString = dr["Person Account: First Name"].ToString().Trim().ToUpper();
                        var vaccinatedDateTimeFromData = dr["Vaccine Administered Time"].ToString();
                        string[] admitDateSplit = dr["Vaccine Administered Time"].ToString().Replace('-','/').Split('/');
                        string[] admitTimeSpliter = dr["Vaccine Administered Time"].ToString().Replace('-','/').Split(',');
                        DateTime AdmitDateTime = GetAdmitDateTime(admitDateSplit, admitTimeSpliter);

                        #region DOB
                        DateTime DOB;
                        int monthfromExcel = 0;
                        int yearfromExcel = 0;
                        int datefromExcel = 0;
                       try
                       {               
                         
                        if (Extension != ".csv")
                        {
                            monthfromExcel = Convert.ToDateTime(dr["Person Account: Birthdate"].ToString()).Month;
                            yearfromExcel = Convert.ToDateTime(dr["Person Account: Birthdate"].ToString()).Year;
                            datefromExcel = Convert.ToDateTime(dr["Person Account: Birthdate"].ToString()).Day;
                        }
                        else
                        {
                            string[] dob = dr["Person Account: Birthdate"].ToString().Split('/');

                            monthfromExcel = Convert.ToInt32(dob[0]);
                            datefromExcel = Convert.ToInt32(dob[1]);
                            yearfromExcel = Convert.ToInt32(dob[2].ToString().Substring(0, 4));
                        }

                                             
                              DOB = new DateTime(yearfromExcel, monthfromExcel, datefromExcel);
                        }
                        catch
                        {
                            DOB = DateTime.MinValue;
                        }


                        #endregion
                                                
                        if (!String.IsNullOrEmpty(lastNameString) && 
                            lastNameString.Trim().ToUpper() != "TEST" &&
                             !String.IsNullOrEmpty(firstNameString) &&
                             !String.IsNullOrEmpty(vaccinatedDateTimeFromData) &&
                              DOB != DateTime.MinValue &&
                             AdmitDateTime != DateTime.MinValue 
                    )
                        {
                            Patient covidPatient = new Patient();
                            Account covidPatientAccount = new Account();
                            IFacilityBroker facilityBroker =
                                BrokerFactory.BrokerOfType<IFacilityBroker>();
                            var hsp = dr["HSPCode"].ToString();
                            facility = facilityBroker.FacilityWith(hsp);
                            if (facility == null)
                            {
                                return;
                            }
                            PatientCovidServiceBroker patientServiceBroker = new PatientCovidServiceBroker();
                             
                            string clinicCode = string.Empty;
                            string industryFromData = dr["Person Account: Industry"].ToString().Trim();
                            if (industryFromData.ToUpper() == "HEALTHCARE WORKER")
                            {
                                continue;
                            }
                            else
                            {
                                clinicCode = "C9";
                                HospitalService employeeHSV = new HospitalService() {Code = "CV"};
                                covidPatientAccount.HospitalService = employeeHSV;
                                covidPatientAccount.KindOfVisit = VisitType.Outpatient;
                            }
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

                            var MIFromData = dr["Person Account: Middle Name"].ToString().Trim().ToUpper();
                            string miString = String.Empty;
                            if (MIFromData.Trim().Length >= 1)
                            {
                                miString = MIFromData.Substring(0, 1);
                            }
                            else
                            {
                                miString = String.Empty;
                            }

                            Name name = new Name(firstName.ToUpper(), lastName.ToUpper(), miString );
                            covidPatient.Name = name;
                            covidPatient.FirstName = firstName.ToUpper();
                            covidPatient.LastName = lastName.ToUpper();
                            covidPatient.MiddleInitial = miString.ToUpper();

                            covidPatient.DateOfBirth = DOB;
                            string ssn = "000-00-0001";
                            covidPatient.SocialSecurityNumber = new SocialSecurityNumber(ssn);
                            covidPatient.IsNew = true;
                            var activity = new RegistrationActivity();
                            activity.AppUser = Domain.User.GetCurrent();
                            activity.AppUser.WorkstationID = "Vaccine";
                            activity.AppUser.PBAREmployeeID = "Vaccine";
                            covidPatientAccount.Activity = activity;
                            covidPatientAccount.Facility = facility;
                            covidPatient.MaritalStatus = new MaritalStatus {Code = "U"};

                            string employeeGenderString = dr["Gender"].ToString();
                            var DemographicsBroker = BrokerFactory.BrokerOfType<IDemographicsBroker>();
                            switch (employeeGenderString.Trim().ToUpper())
                            {
                                case "MALE":
                                    employeeGenderString = "M";
                                    break;
                                case "FEMALE":
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

                            

                            #region Race
                            string patientRaceString = dr["Race"].ToString();

                            var allRaces = i_OriginBroker.AllRaces(facility.Oid);
                            Race patientRace = null;
                            foreach (Race race in allRaces)
                            {
                                if (race.Description.Equals(patientRaceString.ToUpper().Trim()))
                                {
                                    patientRace = race;
                                }
                            }
                            if (patientRace == null || String.IsNullOrEmpty(patientRace.Code))
                            {
                                patientRace = new Race();
                                switch (patientRaceString.ToUpper())
                                {                                    
                                    case "UNKNOWN":
                                        patientRace.Code = "7";
                                        break;
                                    case "PREFER NOT TO SAY":
                                        patientRace.Code = "7";
                                        break;
                                    default:
                                        patientRace.Code = "7";
                                        break;
                                }
                            }

                            #endregion

                            #region Ethnicity

                            string patientEthnictyString = dr["Person Account: Ethnicity"].ToString();

                            var allEthnicities = i_OriginBroker.AllEthnicities(facility.Oid);
                            Ethnicity patientEthnicity = null;
                            foreach (Ethnicity ethnicity in allEthnicities)
                            {
                                if (ethnicity.Description.Equals(patientEthnictyString.ToUpper().Trim()))
                                {
                                    patientEthnicity = ethnicity;
                                }
                            }
                            if (patientEthnicity == null || String.IsNullOrEmpty(patientEthnicity.Code))
                            {
                                patientEthnicity = new Ethnicity();
                                switch (patientEthnictyString.ToUpper())
                                {                                    
                                    case "NOT OF HISPANIC, LATINO OR SPANISH ORIGIN":
                                        patientEthnicity.Code = "2";
                                        break;
                                    case "PREFER NOT TO SAY":
                                        patientEthnicity.Code = "D";
                                        break;
                                    default:
                                        patientEthnicity.Code = "3";
                                        break;
                                }
                            }
                           
                            #endregion

                            #region Add MedicalDirector

                            long medicalDirectorID = 0;
                            try
                            {
                                medicalDirectorID = Convert.ToInt32(dr["Refferring Dr"]);
                            }
                            catch
                            {
                            }

                            IPhysicianBroker PhysicianBroker = BrokerFactory.BrokerOfType<IPhysicianBroker>();

                            PhysicianRelationship ppref =
                                PhysicianBroker.BuildReferringPhysicianRelationship(facility.Oid, medicalDirectorID);
                            covidPatientAccount.AddPhysicianRelationship(ppref);


                            #endregion

                            #region Add ContactPoint

                            string streetString = dr["Person Account: Mailing Street"].ToString();
                            string street = String.Empty;
                            if (streetString.Trim().Length >= 25)
                            {
                                street = streetString.Substring(0, 25);
                            }
                            else
                            {
                                street = streetString;
                            }

                            string cityString = dr["Person Account: Mailing City"].ToString();
                            string city = String.Empty;
                            if (cityString.Trim().Length >= 10)
                            {
                                city = cityString.Substring(0, 10);
                            }
                            else
                            {
                                city = cityString;
                            }

                            string stateString = dr["Person Account: Mailing State/Province"].ToString();
                            if (String.IsNullOrEmpty(stateString))
                            {
                                stateString = "CA";
                            }

                            string state = String.Empty;
                            if (stateString.Trim().Length >= 2)
                            {
                                state = stateString.Substring(0, 2);
                            }
                            else
                            {
                                state = stateString;
                            }

                            string zipcodeString = dr["Person Account: Mailing Zip/Postal Code"].ToString();
                            string zip = String.Empty;

                            if (zipcodeString.Trim().Length >= 6)
                            {
                                zip = zipcodeString.Substring(0, 6);
                            }
                            else
                            {
                                zip = zipcodeString;
                            }
                          
                            var country = "USA";
                      

                            string cellNumber = dr["Person Account: Mobile"].ToString().Replace("(", "")
                                .Replace(")", "").Replace("-", "").Replace(" ", "").Replace("+1", "");
                            string emailAddress = dr["Person Account: Email"].ToString();

                            var i_AddressBroker = BrokerFactory.BrokerOfType<IAddressBroker>();
                            Address patientAddress = new Address();

                            patientAddress.Address1 = street.ToUpper();
                            patientAddress.City = city.ToUpper();
                            patientAddress.State = new State() {Code = state.ToUpper()};
                            patientAddress.Country = new Country() {Code = country};
                            patientAddress.ZipCode = new ZipCode(zip);

                            PhoneNumber patientCell = new PhoneNumber();
                            if (cellNumber != "" && cellNumber.Length >= 10)
                            {
                                string areaCodeCell = cellNumber.Trim().Substring(0, 3);
                                string cellNo = cellNumber.Substring(3, 7);
                                patientCell = new PhoneNumber() {Number = cellNo, AreaCode = areaCodeCell};
                            }
                            EmailAddress email = new EmailAddress(emailAddress);

                            ContactPoint mailingContactPoint = new ContactPoint(
                                patientAddress, new PhoneNumber(), patientCell, email,
                                TypeOfContactPoint.NewMailingContactPointType());
                            covidPatient.AddContactPoint(mailingContactPoint);

                            ContactPoint mobileContactPoint = new ContactPoint(
                                new Address(), patientCell, patientCell, new EmailAddress(),
                                TypeOfContactPoint.NewMobileContactPointType());
                            covidPatient.AddContactPoint(mobileContactPoint);

                            #endregion Add ContactPoint

                            var hcBroker = BrokerFactory.BrokerOfType<IHospitalClinicsBroker>();

                            HospitalClinic clinic = hcBroker.HospitalClinicWith(facility.Oid, clinicCode);
                            if (clinic != null)
                            {
                                covidPatientAccount.HospitalClinic = clinic;
                            }
                            else
                            {
                                covidPatientAccount.HospitalClinic = new HospitalClinic();
                            }
                            var relationShipTypeBroker = BrokerFactory.BrokerOfType<IRelationshipTypeBroker>();
                            //When Insurance Data are NOT available in MyTurn spreadsheet (Person Account: Primary Carrier column is blank), 
                           //the tool will consider the person to be Self-Pay/NOT having insurance.  For Self-Pay accounts:
                           // Default the PlanID=66281 and  Default the FC=70
                            string planID = String.Empty;
                            string financialClass = String.Empty;
                            string primaryCarrier = dr["Person Account: Primary Carrier"].ToString().Trim().ToUpper();
                            if (String.IsNullOrEmpty(primaryCarrier))
                            {
                                planID = "66281";
                                financialClass = "70";
                                var InsuranceBroker = BrokerFactory.BrokerOfType<IInsuranceBroker>();
                                var currentPlan = InsuranceBroker.PlanWith(planID, facility.Oid);
                                Coverage coverage = null;

                                RelationshipType insuredRelationshipType = relationShipTypeBroker.RelationshipTypeWith(
                                    facility.Oid,
                                    RelationshipType.RELATIONSHIP_CODE_SELF);
                                if (currentPlan != null)
                                {
                                    //Insured
                                    Insured insured = covidPatientAccount.Patient.CopyAsInsured();

                                    insuredRelationshipType = relationShipTypeBroker.RelationshipTypeWith(
                                        facility.Oid,
                                        RelationshipType.RELATIONSHIP_CODE_SELF);
                                    Relationship relationship =
                                        new Relationship(insuredRelationshipType, covidPatientAccount.Patient.GetType(),
                                            insured.GetType());
                                    covidPatientAccount.Patient.AddRelationship(relationship);

                                    coverage = Coverage.CoverageFor(currentPlan, insured);
                                    coverage.CoverageOrder = new CoverageOrder(CoverageOrder.PRIMARY_OID, "PRIMARY");
                                    covidPatientAccount.Insurance.AddCoverage(coverage);
                                }
                            }
                            else
                            {
                                financialClass = "91";
                            }

                            FinancialClass patientFC = new FinancialClass() { Code = financialClass };
                            covidPatientAccount.FinancialClass = patientFC;

                            covidPatientAccount.Patient.IsNew = true;
                            covidPatientAccount.Patient = covidPatient;

                            // Guarantor

                            RelationshipType relationshipType = relationShipTypeBroker.RelationshipTypeWith(
                                facility.Oid,
                                RelationshipType.RELATIONSHIP_CODE_SELF);
                            Guarantor guarantor = covidPatientAccount.Patient.CopyAsGuarantor();
                            covidPatientAccount.Guarantor = guarantor;
                            covidPatientAccount.GuarantorIs(guarantor, relationshipType);

                            covidPatientAccount.AdmitDate = AdmitDateTime;
                            covidPatientAccount.IsNew = true;
                            covidPatientAccount.Patient.IsNew = true;

                            covidPatientAccount.Patient = covidPatient;
                            UpdateRaceAndNationalityModelValue(patientRace, covidPatientAccount);
                            UpdateEthnicityAndDescentModelValue(patientEthnicity, covidPatientAccount);

                            covidPatientAccount.Facility = facility;
                            // Checking MRN is Exists or not
                            MRN = patientServiceBroker.GetPatientMRN(covidPatientAccount, facility);

                            // if MRN value equals to 1 that means account is already created for patient with same 
                            // Admit date, will not allow to registration
                            if(MRN==1)
                            {
                                continue;
                            }
                            if (MRN != 0 && MRN != 1)
                            {
                                covidPatientAccount.Patient.MedicalRecordNumber = MRN;
                            }
                            else if (MRN == 0)
                            {
                                MPIPatientMRN = patientServiceBroker . GetMRNFromAccountProxies(covidPatient , facility);
                                covidPatientAccount.Patient.MedicalRecordNumber = MPIPatientMRN;
                            }

                            AccountPBARBroker accountBroker = new AccountPBARBroker();
                            if (!String.IsNullOrEmpty(primaryCarrier))
                            {
                                GenerateRABINFUSNote(dr, covidPatientAccount, "RABIN");
                            }
                            AccountSaveResults saveresult = new AccountSaveResults();
                            //Save Logic
                            saveresult = accountBroker.Save(covidPatientAccount, new RegistrationActivity());
                            patientServiceBroker.InsertPatientRecord(covidPatientAccount);
                            //Preparing result sheet of successfully processed records 
                            TotalSuccessTrasaction += 1;

                            //TODO: Un-comment to generate result sheet in below lines
                            generateResultSheet.GetResultSheet(dr["HSPCode"].ToString()
                                , dr["Person Account: Last Name"].ToString(),
                                dr["Person Account: First Name"].ToString()
                                , covidPatientAccount.Patient.DateOfBirth.ToString("MM/dd/yyyy")
                                , covidPatientAccount.Patient.MedicalRecordNumber.ToString()
                                , covidPatientAccount.AccountNumber.ToString()
                                , "Success");

                            //Preparing charge sheet of successfully processed records
                            string manufacturer = dr["Vaccine Inventory: Brand/Manufacturer"].ToString().ToUpper();                            
                            string doseNumber = dr["Dose Number"].ToString();
                            generateChargeSheet.GetChargeSheet(hsp
                                , covidPatientAccount.AccountNumber.ToString()
                                , covidPatientAccount.AdmitDate.ToString("yyyyMMdd")
                                , manufacturer
                                , doseNumber
                                , facility);                   
                        }
                    }
                    catch (Exception ex)
                    {
                        generateResultSheet.GetResultSheet(dr["HSPCode"].ToString()
                                , dr["Person Account: Last Name"].ToString(),
                                dr["Person Account: First Name"].ToString()
                                , dr["Person Account: Birthdate"].ToString()
                                , "0"
                                , "0"
                                , ex.Message);
                    }
                }

                #endregion

                lbl_msg.Text = "Total Successful Transaction : " + TotalSuccessTrasaction.ToString();
            }

            //Exporting result into excelsheet 
            DataSet ds = new DataSet();
            
            ds.Tables.Add(generateChargeSheet.dtChargeSheet);
            ds.Tables[0].TableName = "ChargeSheet";

            //TODO: Un-comment to generate result sheet in below two lines
            ds.Tables.Add(generateResultSheet.dtResult);
            ds.Tables[1].TableName = "ResultSheet";
            
            ExportToExcel(ds, fileName);
       
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

        private void ExportToExcel(DataSet ds, string fileName)
        {            
            using (XLWorkbook wb = new XLWorkbook())
            {
                foreach (DataTable dt in ds.Tables)
                {
                    //Add DataTable as Worksheet.
                    wb.Worksheets.Add(dt);
                }

                //Export the Excel file.
                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename=" + fileName + "_Result.xlsx");
                using (MemoryStream MyMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(MyMemoryStream);
                    MyMemoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
            }
        }

       
        private static IOriginBroker i_OriginBroker = null;

        public void UpdateRaceAndNationalityModelValue(Race race, Account anAccount)
        {

            if (!string.IsNullOrEmpty(race.ParentRaceCode))
            {
                anAccount.Patient.Nationality = race;
                Race parentRace = GetKeyRaceFromDictionary(race.ParentRaceCode );
                if (parentRace != null)
                    anAccount.Patient.Race = parentRace;
            }
            else
            {
                anAccount.Patient.Race = race;
                anAccount.Patient.Nationality = new Race();
            }

        }

        public void UpdateEthnicityAndDescentModelValue(Ethnicity ethnicity, Account anAccount)
        {

            if (!string.IsNullOrEmpty(ethnicity.ParentEthnicityCode))
            {
                anAccount.Patient.Descent = ethnicity;
                Ethnicity parentEthnicity = GetKeyEthnicityFromDictionary(ethnicity.ParentEthnicityCode);
                if (parentEthnicity != null)
                    anAccount.Patient.Ethnicity = parentEthnicity;
            }
            else
            {
                anAccount.Patient.Ethnicity = ethnicity;
                anAccount.Patient.Descent = new Ethnicity();
            }
        }

        private Race GetKeyRaceFromDictionary(string keyRaceCode )
        {
            var RaceCollection = i_OriginBroker.AllRaces(facility.Oid);
            Race keyRaceSearchedInRaceCollection = null;
            foreach (Race de in RaceCollection)
            {
                var keyRace = de;
                if (keyRace.Code == keyRaceCode)
                {
                    return keyRace;
                }
            }
            return keyRaceSearchedInRaceCollection;
        }
        private Ethnicity GetKeyEthnicityFromDictionary(string keyEthnicityCode)
        {
            var ethnicityCollection = i_OriginBroker.AllEthnicities(facility.Oid);

            Ethnicity keyEthnicitySearchedInDictionary = null;
            foreach (Ethnicity de in ethnicityCollection) // checking to see if there is a entry for r1 in the dictionary
            {
                var keyEthnicity = de;
                if (keyEthnicity.Code == keyEthnicityCode)
                {
                    return keyEthnicity;
                }
            }
            return keyEthnicitySearchedInDictionary;
        }

        private DateTime GetAdmitDateTime(string[] admitDate, string[] admitTimeSpliter)
        {
            int monthfromExcel = 0;
            int yearfromExcel = 0;
            int datefromExcel = 0;
            DateTime AdmitDateTime = DateTime.MinValue;
            try
            {
                
                monthfromExcel = Convert.ToInt32(admitDate[0]);
                datefromExcel = Convert.ToInt32(admitDate[1]);
                yearfromExcel = Convert.ToInt32(admitDate[2].ToString().Substring(0, 4));
               
                string[] admitTime = admitTimeSpliter[1].Split(':');
                int hour = 0;
                int minute = 0;
                if (admitTime.Length > 0)
                {
                    hour = Convert.ToInt32(admitTime[0]);
                    minute = Convert.ToInt32(admitTime[1].ToString().Substring(0, 2));
                }
                else
                {
                    hour = 11;
                    minute = 11;
                }

                AdmitDateTime =
                    new DateTime(yearfromExcel, monthfromExcel, datefromExcel, hour, minute, 0);
            }
            catch
            {
                if (yearfromExcel > 1900)
                {
                    AdmitDateTime =
                        new DateTime(yearfromExcel, monthfromExcel, datefromExcel, 11, 11, 0);
                }

            }
            return AdmitDateTime;

        }

        private void GenerateRABINFUSNote(DataRow dr, Account anAccount, string code)
        {
            FusActivity activity = new FusActivity();
            activity.Code = code;
            activity.StrategyName = "RBAINStrategy";

            FusNote note = new ExtendedFUSNote();
            note.Account = anAccount;
            note.Context = anAccount;
            note.FusActivity = activity;
            note.ManuallyEntered = true;
            note.UserID = anAccount.Activity.AppUser.PBAREmployeeID.ToUpper();
            var policyNumber = dr["Person Account: Medical Record Number or Policy Number"].ToString().Trim().ToUpper();
            var primaryCarrier = dr["Person Account: Primary Carrier"].ToString().Trim().ToUpper();
            var primaryHolder = dr["Person Account: Primary Holder"].ToString().Trim().ToUpper();
            var primaryHolderFirst = dr["Person Account: Primary Holder First"].ToString().Trim().ToUpper();
            var primaryHolderMiddle = dr["Person Account: Primary Holder Middle"].ToString().Trim().ToUpper();
            var primaryHolderLast = dr["Person Account: Primary Holder Last"].ToString().Trim().ToUpper();
            var primaryHolderBirth = dr["Person Account: Primary Birth"].ToString().Trim().ToUpper();
            var primaryHolderRelationship = dr["Person Account: Primary Relationship"].ToString().Trim().ToUpper();
            var primaryAccountGroupNumber = dr["Person Account: Group Number"].ToString().Trim().ToUpper();

            string formattedString;
            if (primaryHolder == "YES")
            {
                formattedString = "ADDITIONAL INSURANCE INFORMATION FROM MY TURN REGISTRATION - POLICY NUMBER: " +
                                  policyNumber + ", PRIMARY CARRIER: " + primaryCarrier +
                                  ", [PATIENT IS] PRIMARY HOLDER: YES" +
                                  ", PRIMARY HOLDER GROUP NUMBER: " + primaryAccountGroupNumber;

            }
            else
            {
                formattedString = "ADDITIONAL INSURANCE INFORMATION FROM MY TURN REGISTRATION - POLICY NUMBER: " +
                                  policyNumber + ", PRIMARY CARRIER: " + primaryCarrier +
                                  ", [PATIENT IS] PRIMARY HOLDER: NO" +
                                  ", PRIMARY HOLDER FIRST: " + primaryHolderFirst +
                                  ", PRIMARY HOLDER LAST: " + primaryHolderLast +
                                  ", PRIMARY HOLDER MIDDLE: " + primaryHolderMiddle +
                                  ", PRIMARY HOLDER BIRTH: " + primaryHolderBirth +
                                  ", PRIMARY HOLDER RELATIONSHIP: " + primaryHolderRelationship +
                                  ", PRIMARY HOLDER GROUP NUMBER: " + primaryAccountGroupNumber;

            }

            note.Remarks = formattedString;

            anAccount.AddFusNote(note);
        }
        
    }
}

[Serializable]
    public class PatientCovidServiceBroker : AbstractPBARBroker
    {
        private const string DBPROCEDURE_PATIENTMYTURNUPLOADINSERT = "PACCESS.PATIENTMYTURNUPLOADINSERT";
        private const string DBPROCEDURE_GETPATIENTMYTURNMRN = "PACCESS.GETCOVIDVACCINEPATIENTMRN";
        private const string DBPROCEDURE_PATIENTMRNFORMYTURNUPLOADS = "PACCESS .PATIENTMRNFORMYTURNUPLOADS";
        private const string DBPROCEDURE_GETCHARGECODEFORMYTURNUPLOAD = "PACCESS.GETCHARGECODEFORMYTURNUPLOAD";
        private static readonly ILog c_log = LogManager.GetLogger(typeof(PatientMyTurnUpload));
        public const string PARAM_HSPCODE = "@P_HSPCODE";
        public const string PARAM_PATIENTLASTNAME = "@P_LASTNAME";
        public const string PARAM_PATIENTFIRSTNAME = "@P_FIRSTNAME";
        public const string PARAM_MI = "@P_MI";
        public const string PARAM_PATIENTDOB = "@P_DOB";      
        public const string PARAM_MRN = "@P_MRN";
        public const string PARAM_ADMITDATETIME = "@P_ADMITDATETIME";
        public const string PARAM_HSV = "@P_HOSPITALSERVICE";
        public const string PARAM_CLINICCODE = "@P_CLINICCODE";
        public const string PARAM_ACCOUNTNUMBER = "@P_ACCOUNTNUMBER";
        public const string COL_MRN = "MRN";
        public const string COL_ADMITDATE = "ADMITDATETIME";
        public const string COL_ACCOUNTNUMBER = "ACCOUNTNUMBER";
        public const string PARAM_FACILITYID = "@P_HSP";
        public const string PARAM_DOSENUMBER = "@P_DOSENUMBER";
        public const string PARAM_MANUFACTURER = "@P_MANUFACTURER";
        public const string PARAM_TYPE = "@P_TYPE";
        public const string COL_CHARGECODE = "CHARGECODE";

    public void InsertPatientRecord(Account account )
        {
            SafeReader reader = null;
            iDB2Command cmd = null;
            try
            {
                cmd = this.CommandFor("CALL " + DBPROCEDURE_PATIENTMYTURNUPLOADINSERT +
                        "(" + PARAM_HSPCODE +
                    "," + PARAM_PATIENTLASTNAME +
                    "," + PARAM_PATIENTFIRSTNAME +
                    "," + PARAM_PATIENTDOB + 
                    "," + PARAM_MRN +
                    "," + PARAM_ACCOUNTNUMBER +
                    "," + PARAM_ADMITDATETIME +
                    "," + PARAM_HSV +
                    "," + PARAM_CLINICCODE +
                    ")",
                    CommandType.Text,
                    account.Facility);

                cmd.Parameters[PARAM_HSPCODE].Value = account.Facility.Code;
                cmd.Parameters[PARAM_PATIENTLASTNAME].Value = account.Patient.Name.LastName;
                cmd.Parameters[PARAM_PATIENTFIRSTNAME].Value = account.Patient.Name.FirstName;
                cmd.Parameters[PARAM_PATIENTDOB].Value = account.Patient.DateOfBirth;                
                cmd.Parameters[PARAM_MRN].Value = account.Patient.MedicalRecordNumber;
                cmd.Parameters[PARAM_ACCOUNTNUMBER].Value = account.AccountNumber;
                cmd.Parameters[PARAM_ADMITDATETIME].Value = account.AdmitDate;
                cmd.Parameters[PARAM_HSV].Value = account.HospitalService.Code;
                cmd.Parameters[PARAM_CLINICCODE].Value = account.HospitalClinic.Code;
                reader = this.ExecuteReader(cmd);

            }
            catch(Exception ex)
            {
                string msg = "PATIENTHEALTHBULKUPLOADINSERT failed for Facility:" + account.Facility.Oid +
                    " MRN:" + account.Patient.MedicalRecordNumber +" Error: "+ex.Message;
            }
            finally
            {
                Close(reader);
                Close(cmd);
            }
        }

    public long GetPatientMRN(Account account, Facility facility)
        {
            long mrn = 0;
            DateTime existsAdmitDate = DateTime.MinValue;
            SafeReader reader = null;
            iDB2Command cmd = null;

            try
            {
                IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
                cmd = this.CommandFor("CALL " + DBPROCEDURE_GETPATIENTMYTURNMRN +
                                      "(" + PARAM_HSPCODE +
                                      "," + PARAM_PATIENTLASTNAME +
                                      "," + PARAM_PATIENTFIRSTNAME +
                                      "," + PARAM_PATIENTDOB +
                                      ")",
                    CommandType.Text,
                    facility);

                cmd.Parameters[PARAM_HSPCODE].Value = facility.Code;
                cmd.Parameters[PARAM_PATIENTLASTNAME].Value = account.Patient.Name.LastName;
                cmd.Parameters[PARAM_PATIENTFIRSTNAME].Value = account.Patient.Name.FirstName;
                cmd.Parameters[PARAM_PATIENTDOB].Value = account.Patient.DateOfBirth;

                reader = this.ExecuteReader(cmd);
                while (reader.Read())
                {
                    mrn = reader.GetInt64(COL_MRN);
                    existsAdmitDate = reader.GetDateTime(COL_ADMITDATE);
                    if (existsAdmitDate.Date == account.AdmitDate.Date)
                    {
                        mrn = 1;
                        break;
                    }
                }

                return mrn;
            }
            catch (Exception ex)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom(ex, c_log);
            }
            finally
            {
                Close(reader);
                Close(cmd);
            }
        }
    public long GetMRNFromAccountProxies(Patient patient, Facility facility)
        {
            long mrn = 0;
            SafeReader reader = null;
            iDB2Command cmd = null;

            try
            {
                IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
                cmd = this.CommandFor("CALL " + DBPROCEDURE_PATIENTMRNFORMYTURNUPLOADS +
                                      "(" + PARAM_FACILITYID +                                     
                                      "," + PARAM_PATIENTFIRSTNAME +
                                       "," + PARAM_PATIENTLASTNAME +                                     
                                      "," + PARAM_PATIENTDOB +
                                      ")",
                    CommandType.Text,
                    facility);

                cmd.Parameters[PARAM_FACILITYID].Value = facility.Oid;
                cmd.Parameters[PARAM_PATIENTLASTNAME].Value = patient.Name.LastName.ToUpper();

                string[] fName = patient.FirstName.Split(' ');

                cmd.Parameters[PARAM_PATIENTFIRSTNAME].Value = fName[0].ToString().Trim().ToUpper(); ;
 
                
                cmd.Parameters[PARAM_PATIENTDOB].Value = patient.DateOfBirth.ToString("MMddyyyy");

                reader = this.ExecuteReader(cmd);
                while (reader.Read())
                {
                    mrn = reader.GetInt64(COL_MRN);
                    if(mrn != 0)
                    {
                        break;
                    }
                }

                return mrn;
            }
            catch (Exception ex)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom(ex, c_log);
            }
            finally
            {
                Close(reader);
                Close(cmd);
            }
        }

    public string GetChargeCode(string manufacture, string doseNumber, string type,Facility facility)
    {
        string chargeCode = string.Empty;
        SafeReader reader = null;
        iDB2Command cmd = null;

        try
        {
            IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            cmd = this.CommandFor("CALL " + DBPROCEDURE_GETCHARGECODEFORMYTURNUPLOAD +
                                  "(" + PARAM_MANUFACTURER +
                                  "," + PARAM_DOSENUMBER +
                                   "," + PARAM_TYPE +                                  
                                  ")",
                CommandType.Text,
                facility);

            cmd.Parameters[PARAM_MANUFACTURER].Value = manufacture;
            cmd.Parameters[PARAM_DOSENUMBER].Value = doseNumber;
            cmd.Parameters[PARAM_TYPE].Value = type;
            
            reader = this.ExecuteReader(cmd);
            while (reader.Read())
            {
                chargeCode = reader.GetString(COL_CHARGECODE).Trim();
            }

            return chargeCode;
        }
        catch (Exception ex)
        {
            throw BrokerExceptionFactory.BrokerExceptionFrom(ex, c_log);
        }
        finally
        {
            Close(reader);
            Close(cmd);
        }
    }
}

public class GenerateResultSheet
{
   public DataTable dtResult = new DataTable();

    public GenerateResultSheet()
    {
        dtResult = new DataTable();
        dtResult.Columns.Add("Facility ID");
        dtResult.Columns.Add("LastName");
        dtResult.Columns.Add("FirstName");
        dtResult.Columns.Add("DOB");
        dtResult.Columns.Add("MRN");
        dtResult.Columns.Add("Account Number");
        dtResult.Columns.Add("Status");
    }

    public DataTable GetResultSheet(string HSPCode,string PersonAccountLastName,string PersonAccountFirstName,string PersonAccountBirthdate
        ,string MedicalRecordNumber,string AccountNumber,string Status)
    {
        DataRow drResult = dtResult.NewRow();
        drResult[0] = HSPCode;
        drResult[1] = PersonAccountLastName;
        drResult[2] = PersonAccountFirstName;
        drResult[3] = PersonAccountBirthdate;
        drResult[4] = MedicalRecordNumber;
        drResult[5] = AccountNumber;
        drResult[6] = Status;
        dtResult.Rows.Add(drResult);
        dtResult.AcceptChanges();

        return dtResult;
    }
}

public class GenerateChargeSheet
{
    public DataTable dtChargeSheet = new DataTable();
    PatientCovidServiceBroker patientServiceBroker = new PatientCovidServiceBroker();
    public GenerateChargeSheet()
    {
        dtChargeSheet = new DataTable();
        dtChargeSheet.Columns.Add("Hospital Code");
        dtChargeSheet.Columns.Add("Account #");
        dtChargeSheet.Columns.Add("Service Date(ccyymmdd)");
        dtChargeSheet.Columns.Add("Charge #");
        dtChargeSheet.Columns.Add("Quantity");
    }

    public DataTable GetChargeSheet(string HSPCode, string AccountNumber, string AdmitDate, string manufacturer
        , string doseNumber, Facility facility)
    {
        if (manufacturer != "" && manufacturer != null)
        {
            for (int i = 0; i <= 1; i++)
            {
                DataRow drChargeSheet = dtChargeSheet.NewRow();
                drChargeSheet[0] =HSPCode;
                drChargeSheet[1] = AccountNumber;
                drChargeSheet[2] = AdmitDate;
                drChargeSheet[4] = 1;
                if (i == 0)
                {
                    drChargeSheet[3] = patientServiceBroker.GetChargeCode(manufacturer, doseNumber, "ADMIN", facility);
                }
                else
                {
                    drChargeSheet[3] = patientServiceBroker.GetChargeCode(manufacturer, doseNumber, "COMPOUND", facility);
                }

                dtChargeSheet.Rows.Add(drChargeSheet);
                dtChargeSheet.AcceptChanges();
            }
        }

        return dtChargeSheet;
    }
}