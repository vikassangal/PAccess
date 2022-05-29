using System;
using System.Collections;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Persistence
{
    /// <summary>
    /// Summary description for CancelDischargeInsertStrategy.
    /// </summary>
    public class TransferInPatientToOutPatientInsertStrategy : SqlBuilderStrategy
    {
        #region Construction And Finalization

        public TransferInPatientToOutPatientInsertStrategy()
        {
            InitializeColumnValues();
        }

        #endregion

        #region Methods

        public override void UpdateColumnValuesUsing( Account account )
        {
            if ( account != null )
            {
                //Set Transaction APACFL Add/Change flag
                if ( account.Activity.GetType() == typeof( TransferInToOutActivity ) ||
                    account.Activity.GetType() == typeof( TransferOutToInActivity ) )
                {
                    AddChangeFlag = ADD_FLAG;
                }

                if ( account.Facility != null )
                {
                    HospitalNumber = account.Facility.Oid;
                }
                Patientaccountnumber = account.AccountNumber;

                // SR40984 - Capture comments on Inpatient to Outpatient Transfer screen
                if ( account.Activity.GetType() == typeof( TransferInToOutActivity ) )
                {
                    CaptureCommentsFrom( account.Activity as TransferInToOutActivity );
                    AdmittingCategory = ADMITTING_CATEGORY_FOR_INPATIENT;
                }
                else if ( account.Activity.GetType() == typeof( TransferOutToInActivity ) ||
                   account.Activity.GetType() == typeof(CancelInpatientStatusActivity))
                {
                    CaptureCommentsFrom( account );
                    AdmittingCategory = account.AdmittingCategory;
                }
                else
                {
                    AdmittingCategory = account.AdmittingCategory;
                }

                // TLG 06/21/2006 We should be passing the Transfer date, not the Admit date!

                DateOfAdmission = account.TransferDate;
                TimeOfAdmission = account.TransferDate;
                TypeOfRegistration = TYPE_OF_REGISTRATION_FOR_OUTPATIENT;

                if ( account.AttendingPhysician != null )
                {
                    AttendingPhysicianNumber =
                        account.AttendingPhysician.PhysicianNumber;
                }
                if ( account.Location != null )
                {
                    if ( account.Location.NursingStation != null )
                    {
                        NursingStation = account.Location.NursingStation.Code;
                    }
                    if ( account.Location.Room != null )
                    {
                        RoomNumber = Convert.ToInt32( account.Location.Room.Code );
                    }
                    if ( account.Location.Bed != null )
                    {
                        BedNumber = account.Location.Bed.Code;
                        if ( account.Location.Bed.Accomodation != null )
                        {
                            AccomodationCode = account.Location.Bed.Accomodation.Code;
                        }
                    }
                }

                if ( account.AdmitSource != null )
                {
                    PatientAdmitSourceCode = account.AdmitSource.Code;
                }
                if ( account.AlternateCareFacility != null )
                {
                    AlternateCareFacility = account.AlternateCareFacility;
                }
                if ( account.KindOfVisit != null )
                {
                    PatientType = account.KindOfVisit.Code;
                }
                if ( account.HospitalService != null )
                {
                    MedicalServiceCode = account.HospitalService.Code;
                }
                if ( account.HospitalClinic != null )
                {
                    ClinicalCode = account.HospitalClinic.Code;
                }
                if ( account.ValuablesAreTaken != null )
                {
                    ValuablesAreTaken = account.ValuablesAreTaken.Code.Trim();
                }
                if ( account.FinancialClass != null )
                {
                    FinancialClassCode = account.FinancialClass.Code;
                }
                if ( account.Patient != null )
                {
                    if ( account.Patient.Religion != null )
                    {
                        ReligionCode = account.Patient.Religion.Code;
                    }
                    if ( account.Patient.PlaceOfWorship != null )
                    {
                        ParishCodeAfter = account.Patient.PlaceOfWorship.Code;
                    }
                }
                if ( account.Smoker != null )
                {
                    SmokerCode = account.Smoker.Code.Trim();
                }

                if ( account.Diagnosis != null &&
                    account.Diagnosis.ChiefComplaint != null )
                {
                    Diagnosis = account.Diagnosis.ChiefComplaint.Trim();
                }

                if ( account.ConfidentialityCode != null &&
                     account.ConfidentialityCode.Code != String.Empty )
                {
                    ConfidentialityCode = account.ConfidentialityCode.Code.Trim();
                }

                if ( account.Activity != null )
                {
                    User appUser = account.Activity.AppUser;
                    if ( appUser != null )
                    {
                        if ( appUser.WorkstationID != null )
                        {
                            WorkStationId = appUser.WorkstationID;
                        }
                    }
                }

                var broker = BrokerFactory.BrokerOfType<IPhysicianBroker>();
                nonStaffPhysicianNumber = broker.GetNonStaffPhysicianNumber();
                ReadAdmittingPhysicianInfo( account );
                ReadAttendingPhysicianInfo( account );
                ReadOperatingPhysicianInfo( account );
                ReadPrimaryCarePhysicianInfo( account );
                ReadReferringPhysicianInfo( account );
                ReadOccurrenceSpanCodeInfo( account );
                ReadConditionCode( account.ConditionCodes );
                ReadCodedDiagnoses( account );

                var tBroker = BrokerFactory.BrokerOfType<ITimeBroker>();
                DateTime facilityDateTime = tBroker.TimeAt( account.Facility.GMTOffset, account.Facility.DSTOffset );

                TransactionDate = facilityDateTime;
                TimeRecordCreation = facilityDateTime;
            }
        }

        public override void InitializeColumnValues()
        {
            transferInPatientToOutPatientOrderedList.Add( APIDWS, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APIDID, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APRR_, PATIENT_RECORD_NUMBER );
            transferInPatientToOutPatientOrderedList.Add( APSEC2, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APHSP_, 0 );
            transferInPatientToOutPatientOrderedList.Add( APACCT, 0 );
            transferInPatientToOutPatientOrderedList.Add( APLAD, 0 );
            transferInPatientToOutPatientOrderedList.Add( APLAT, 0 );
            transferInPatientToOutPatientOrderedList.Add( APADMC, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APNS, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APROOM, 0 );
            transferInPatientToOutPatientOrderedList.Add( APBED, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APPTYP, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APMSV, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APSMOK, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APDIET, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APCOND, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APISO, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APTCRT, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APDCRT, 0 );
            transferInPatientToOutPatientOrderedList.Add( APSCRT, 0 );
            transferInPatientToOutPatientOrderedList.Add( APNCRT, 0 );
            transferInPatientToOutPatientOrderedList.Add( APICRT, 0 );
            transferInPatientToOutPatientOrderedList.Add( AP_EXT, 0 );
            transferInPatientToOutPatientOrderedList.Add( APDTOT, 0 );
            transferInPatientToOutPatientOrderedList.Add( APLML, 0 );
            transferInPatientToOutPatientOrderedList.Add( APLMD, 0 );
            transferInPatientToOutPatientOrderedList.Add( APLUL_, 0 );
            transferInPatientToOutPatientOrderedList.Add( APACFL, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APTTME, 0 );
            transferInPatientToOutPatientOrderedList.Add( APINLG, LOG_NUMBER );
            transferInPatientToOutPatientOrderedList.Add( APBYPS, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APSWPY, 0 );
            transferInPatientToOutPatientOrderedList.Add( APPSRC, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APADR_, 0 );
            transferInPatientToOutPatientOrderedList.Add( APRA01, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APRA02, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APRA03, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APRA04, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APRA05, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APCC01, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APCC02, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APCC03, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APDIAG, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APACC, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APTDAT, 0 );
            transferInPatientToOutPatientOrderedList.Add( APACTP, 0 );
            transferInPatientToOutPatientOrderedList.Add( APXMIT, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APQNUM, 0 );
            transferInPatientToOutPatientOrderedList.Add( APUPRV, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APUPRW, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APZDTE, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APZTME, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APMDR_, 0 );
            transferInPatientToOutPatientOrderedList.Add( APTREG, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APRDR_, 0 );
            transferInPatientToOutPatientOrderedList.Add( APCL01, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APCL02, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APCL03, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APCL04, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APCL05, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APVALU, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( "APFC", string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APRLGN, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APCOM1, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APCOM2, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APMNU_, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APMNDN, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APMNFR, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APRNU_, 0 );
            transferInPatientToOutPatientOrderedList.Add( APRNDN, 0 );
            transferInPatientToOutPatientOrderedList.Add( APRNFR, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APSPNC, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APSPFR, 0 );
            transferInPatientToOutPatientOrderedList.Add( APSPTO, 0 );
            transferInPatientToOutPatientOrderedList.Add( APINSN, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APSPN2, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APAPFR, 0 );
            transferInPatientToOutPatientOrderedList.Add( APAPTO, 0 );
            transferInPatientToOutPatientOrderedList.Add( APCNFG, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APMNCD, 0 );
            transferInPatientToOutPatientOrderedList.Add( APMNP_, 0 );
            transferInPatientToOutPatientOrderedList.Add( APPARC, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APOPD_, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APTDR_, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APMNLN, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APMNFN, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APMNMN, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APMNPR, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APMTXC, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APRNLN, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APRNFN, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APRNMN, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APRNPR, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APRTXC, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APRNP_, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APANLN, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APANFN, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APANMN, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APANU_, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APANPR, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APATXC, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APANP_, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APONLN, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APONFN, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APONMN, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APONU_, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APONPR, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APOTXC, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APONP_, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APTNLN, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APTNFN, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APTNMN, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APTNU_, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APTNPR, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APTTXC, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APTNP_, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APMSL_, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APRSL_, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APASL_, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APOSL_, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APTSL_, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APWSIR, WORKSTATION_ID );
            transferInPatientToOutPatientOrderedList.Add( APSECR, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APORR1, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APORR2, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APORR3, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APCI01, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APCI02, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APCI03, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APCI04, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APCI05, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APCI06, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APCI07, string.Empty );
            transferInPatientToOutPatientOrderedList.Add( APNHACF, string.Empty );
        }

        public override ArrayList BuildSqlFrom( Account account, TransactionKeys transactionKeys )
        {
            UpdateColumnValuesUsing( account );
            AddColumnsAndValuesToSqlStatement( transferInPatientToOutPatientOrderedList,
                                              "HPADAPOT" );
            SqlStatements.Add( SqlStatement );

            return SqlStatements;
        }

        #endregion

        #region Private Methods

        private void CaptureCommentsFrom( Account account )
        {
            string embosserCard = String.Empty;
            if ( account.EmbosserCard != null )
            {
                embosserCard = account.EmbosserCard;
            }

            if ( account.ClinicalComments != null )
            {
                if ( account.ClinicalComments.Trim().Length == 0
                    && embosserCard.Trim().Length == 0 )
                {
                    ClinicalCommentsOne = String.Empty;
                    ClinicalCommentsTwo = String.Empty;
                }
                else if ( account.ClinicalComments.Length <= TOTAL_LENGTH_OF_PBAR_COMMENTS )
                {
                    ClinicalCommentsOne = account.ClinicalComments.Trim();
                    var leftPad = new string( ' ', 55 );
                    ClinicalCommentsTwo = leftPad + embosserCard.Trim();
                }
                else
                {
                    ClinicalCommentsOne = account.ClinicalComments.Substring( 0, TOTAL_LENGTH_OF_PBAR_COMMENTS );
                    ClinicalCommentsTwo = account.ClinicalComments.Substring( TOTAL_LENGTH_OF_PBAR_COMMENTS ).PadRight(
                                              LENGTH_OF_COMMENTS_TWO, ' ' )
                                          + embosserCard.Trim();
                }
            }
        }

        // SR40984 - Capture comments on Inpatient to Outpatient Transfer screen
        private void CaptureCommentsFrom( TransferInToOutActivity activity )
        {
            string remarks = activity.Remarks.Trim();
            if ( remarks.Length == 0 )
            {
                ClinicalCommentsOne = String.Empty;
            }
            else if ( activity.Remarks.Length <= LENGTH_OF_IN_TO_OUT_TRANSFER_REMARKS )
            {
                ClinicalCommentsOne = remarks;
            }
            else
            {
                ClinicalCommentsOne = remarks.Substring( 0, LENGTH_OF_IN_TO_OUT_TRANSFER_REMARKS );
            }
        }

        private void ReadAttendingPhysicianInfo( Account account )
        {
            PhysicianRelationship physicianRelationship =
                account.PhysicianRelationshipWithRole( PhysicianRole.Attending().Role() );
            if ( physicianRelationship != null &&
                physicianRelationship.Physician != null )
            {
                AttendingPhysicianNumber =
                    physicianRelationship.Physician.PhysicianNumber;
                AttendingPhysicianFirstName =
                    physicianRelationship.Physician.FirstName;
                AttendingPhysicianMiddleInitial =
                    physicianRelationship.Physician.Name.MiddleInitial;
                AttendingPhysicianLastName =
                    physicianRelationship.Physician.LastName;
                AttendingPhysicianNationalProviderId =
                    physicianRelationship.Physician.NPI;
                AttendingPhysicianUPINNumber =
                    physicianRelationship.Physician.UPIN;
                AttendingPhysicianPhoneNumber =
                    physicianRelationship.Physician.PhoneNumber.AreaCode +
                    physicianRelationship.Physician.PhoneNumber.Number;
                AttendingPhysicianStateLicense =
                    physicianRelationship.Physician.StateLicense;
            }
        }

        private void ReadAdmittingPhysicianInfo( Account account )
        {
            //Admitting Physician Number
            PhysicianRelationship physicianRelationship =
                account.PhysicianRelationshipWithRole( PhysicianRole.Admitting().Role() );
            if ( physicianRelationship != null &&
                physicianRelationship.Physician != null )
            {
                AdmittingPhysicianNumber =
                    physicianRelationship.Physician.PhysicianNumber;
                string fullName =
                    physicianRelationship.Physician.LastName + physicianRelationship.Physician.FirstName;
                if ( fullName.Length > 13 )
                {
                    AdmittingPhysicianFullName = fullName.Substring( 0, 13 );
                }
                else
                {
                    AdmittingPhysicianFullName = fullName;
                }

                AdmittingPhysicianFirstName =
                    physicianRelationship.Physician.FirstName;
                AdmittingPhysicianMiddleInitial =
                    physicianRelationship.Physician.Name.MiddleInitial;
                AdmittingPhysicianLastName =
                    physicianRelationship.Physician.LastName;
                AdmittingPhysicianNationalProviderId =
                    physicianRelationship.Physician.NPI;
                AdmittingPhysicianUPINNumber =
                    physicianRelationship.Physician.UPIN;

                AdmittingPhysicianPhoneNumber =
                    ConvertToInt( physicianRelationship.Physician.PhoneNumber.Number );
                AdmittingPhysicianAreaCode =
                    ConvertToInt( physicianRelationship.Physician.PhoneNumber.AreaCode );
                AdmittingPhysicianStateLicense =
                    physicianRelationship.Physician.StateLicense;
            }
        }

        private void ReadReferringPhysicianInfo( Account account )
        {
            PhysicianRelationship physicianRelationship =
                account.PhysicianRelationshipWithRole( PhysicianRole.Referring().Role() );
            if ( physicianRelationship != null &&
                physicianRelationship.Physician != null )
            {
                ReferringPhysicianNumber =
                    physicianRelationship.Physician.PhysicianNumber;

                string fullName =
                    physicianRelationship.Physician.LastName + physicianRelationship.Physician.FirstName;
                if ( fullName.Length > 14 )
                {
                    ReferringPhysicianFullName = fullName.Substring( 0, 13 );
                }
                else
                {
                    ReferringPhysicianFullName = fullName;
                }

                ReferringPhysicianFirstName =
                    physicianRelationship.Physician.FirstName;
                ReferringPhysicianMiddleInitial =
                    physicianRelationship.Physician.Name.MiddleInitial;
                ReferringPhysicianLastName =
                    physicianRelationship.Physician.LastName;
                ReferringPhysicianNationalProviderId =
                    physicianRelationship.Physician.NPI;
                ReferringPhysicianUPINNumber =
                    physicianRelationship.Physician.UPIN;

                ReferringPhysicianPhoneNumber =
                    physicianRelationship.Physician.PhoneNumber.AreaCode +
                    physicianRelationship.Physician.PhoneNumber.Number;

                ReferringPhysicianStateLicense =
                    physicianRelationship.Physician.StateLicense;
            }
        }

        private void ReadConditionCode( IList conditionCodes )
        {
            int index = 1;
            foreach ( ConditionCode conditionCode in conditionCodes )
            {
                string conditionCodeValue = conditionCode.Code;
                switch ( index )
                {
                    case 1:
                        {
                            ConditionCode1 = conditionCodeValue;
                            break;
                        }
                    case 2:
                        {
                            ConditionCode2 = conditionCodeValue;
                            break;
                        }
                    case 3:
                        {
                            ConditionCode3 = conditionCodeValue;
                            break;
                        }
                    case 4:
                        {
                            ConditionCode4 = conditionCodeValue;
                            break;
                        }
                    case 5:
                        {
                            ConditionCode5 = conditionCodeValue;
                            break;
                        }
                    case 6:
                        {
                            ConditionCode6 = conditionCodeValue;
                            break;
                        }
                    case 7:
                        {
                            ConditionCode7 = conditionCodeValue;
                            break;
                        }
                }
                index++;
            }
        }

        private void ReadOperatingPhysicianInfo( Account account )
        {
            PhysicianRelationship physicianRelationship =
                account.PhysicianRelationshipWithRole( PhysicianRole.Operating().Role() );
            if ( physicianRelationship != null &&
                physicianRelationship.Physician != null )
            {
                OperatingPhysicianNumber =
                    physicianRelationship.Physician.PhysicianNumber.ToString().PadLeft( 5, '0' );

                OperatingPhysicianMiddleInitial =
                    physicianRelationship.Physician.Name.MiddleInitial;
                OperatingPhysicianFirstName =
                    physicianRelationship.Physician.FirstName;
                OperatingPhysicianLastName =
                    physicianRelationship.Physician.LastName;
                OperatingPhysicianNationalProviderId =
                    physicianRelationship.Physician.NPI;
                OperatingPhysicianUPINNumber =
                    physicianRelationship.Physician.UPIN;
                OperatingPhysicianPhoneNumber =
                    physicianRelationship.Physician.PhoneNumber.AreaCode +
                    physicianRelationship.Physician.PhoneNumber.Number;
                OperatingPhysicianStateLicense =
                    physicianRelationship.Physician.StateLicense;
            }
        }

        private void ReadPrimaryCarePhysicianInfo( Account account )
        {
            PhysicianRelationship physicianRelationship =
                account.PhysicianRelationshipWithRole( PhysicianRole.PrimaryCare().Role() );
            if ( physicianRelationship != null &&
                physicianRelationship.Physician != null )
            {
                PrimaryCarePhysicianNumber =
                    physicianRelationship.Physician.PhysicianNumber.ToString().PadLeft( 5, '0' );

                PrimaryCarePhysicianMiddleInitial =
                    physicianRelationship.Physician.Name.MiddleInitial;
                PrimaryCarePhysicianFirstName =
                    physicianRelationship.Physician.FirstName;
                PrimaryCarePhysicianLastName =
                    physicianRelationship.Physician.LastName;
                PrimaryCarePhysicianNationalProviderId =
                    physicianRelationship.Physician.NPI;
                PrimaryCarePhysicianUPINNumber =
                    physicianRelationship.Physician.UPIN;
                PrimaryCarePhysicianPhoneNumber =
                    physicianRelationship.Physician.PhoneNumber.AreaCode +
                    physicianRelationship.Physician.PhoneNumber.Number;
                PrimaryCarePhysicianStateLicense =
                    physicianRelationship.Physician.StateLicense;
            }
        }

        private void ReadOccurrenceSpanCodeInfo( Account account )
        {
            if ( account.OccurrenceSpans != null )
            {
                if ( account.OccurrenceSpans.Count > 0
                    && account.OccurrenceSpans[0] != null )
                {
                    if ( ( ( OccurrenceSpan )account.OccurrenceSpans[0] ).SpanCode != null )
                    {
                        OccurranceSpanCodeOne =
                            ( ( OccurrenceSpan )account.OccurrenceSpans[0] ).SpanCode.Code;
                    }
                    OccurranceSpanFromDateOne =
                        ( ( OccurrenceSpan )account.OccurrenceSpans[0] ).FromDate;
                    OccurranceSpanToDateOne =
                        ( ( OccurrenceSpan )account.OccurrenceSpans[0] ).ToDate;

                    OccurranceSpanFacility = ( ( OccurrenceSpan )account.OccurrenceSpans[0] ).Facility;
                }

                if ( account.OccurrenceSpans.Count > 1
                    && account.OccurrenceSpans[1] != null )
                {
                    if ( ( ( OccurrenceSpan )account.OccurrenceSpans[1] ).SpanCode != null )
                    {
                        OccurranceSpanCodeSecond =
                            ( ( OccurrenceSpan )account.OccurrenceSpans[1] ).SpanCode.Code;
                    }
                    OccurranceSpanSecondFromDateOne =
                        ( ( OccurrenceSpan )account.OccurrenceSpans[1] ).FromDate;
                    OccurranceSpanSecondToDateOne =
                        ( ( OccurrenceSpan )account.OccurrenceSpans[1] ).ToDate;
                }
            }
        }

        private void ReadCodedDiagnoses( Account account )
        {
            for ( int x = 0; x < account.CodedDiagnoses.AdmittingCodedDiagnosises.Count; x++ )
            {
                string colName = "APRA0" + ( x + 1 );
                transferInPatientToOutPatientOrderedList[colName] =
                    account.CodedDiagnoses.AdmittingCodedDiagnosises[x];
            }
        }

        #endregion

        #region Private Properties

        private long HospitalNumber
        {
            set { transferInPatientToOutPatientOrderedList[APHSP_] = value; }
        }

        private long Patientaccountnumber
        {
            set { transferInPatientToOutPatientOrderedList[APACCT] = value; }
        }

        private DateTime DateOfAdmission
        {
            set { transferInPatientToOutPatientOrderedList[APLAD] = ConvertDateToIntInMddyyFormat( value ); }
        }

        private DateTime TimeOfAdmission
        {
            set { transferInPatientToOutPatientOrderedList[APLAT] = ConvertTimeToIntInHHmmFormat( value ); }
        }

        private string AdmittingCategory
        {
            set { transferInPatientToOutPatientOrderedList[APADMC] = value; }
        }

        private string NursingStation
        {
            set { transferInPatientToOutPatientOrderedList[APNS] = value; }
        }

        private int RoomNumber
        {
            set { transferInPatientToOutPatientOrderedList[APROOM] = value; }
        }

        private string BedNumber
        {
            set { transferInPatientToOutPatientOrderedList[APBED] = value; }
        }

        private string PatientType
        {
            set { transferInPatientToOutPatientOrderedList[APPTYP] = value; }
        }

        private string MedicalServiceCode
        {
            set { transferInPatientToOutPatientOrderedList[APMSV] = value; }
        }

        private DateTime TimeRecordCreation
        {
            set
            {
                transferInPatientToOutPatientOrderedList[APTTME] =
                    ConvertTimeToIntInHHmmSSFormat( value );
            }
        }

        private string PatientAdmitSourceCode
        {
            set { transferInPatientToOutPatientOrderedList[APPSRC] = value; }
        }

        private string AlternateCareFacility
        {
            set { transferInPatientToOutPatientOrderedList[APNHACF] = value; }
        }

        private string Diagnosis
        {
            set { transferInPatientToOutPatientOrderedList[APDIAG] = value; }
        }

        private string ConfidentialityCode
        {
            set { transferInPatientToOutPatientOrderedList[APCNFG] = value; }
        }

        private DateTime TransactionDate
        {
            set
            {
                transferInPatientToOutPatientOrderedList[APTDAT] =
                    ConvertDateToIntInMddyyFormat( value );
            }
        }

        private string ValuablesAreTaken
        {
            set { transferInPatientToOutPatientOrderedList[APVALU] = value; }
        }

        private string FinancialClassCode
        {
            set { transferInPatientToOutPatientOrderedList[APFC] = value; }
        }

        private string ReligionCode
        {
            set { transferInPatientToOutPatientOrderedList[APRLGN] = value; }
        }

        private string ParishCodeAfter
        {
            set { transferInPatientToOutPatientOrderedList[APPARC] = value; }
        }

        private string TypeOfRegistration
        {
            set { transferInPatientToOutPatientOrderedList[APTREG] = value; }
        }

        private string AccomodationCode
        {
            set { transferInPatientToOutPatientOrderedList[APACC] = value; }
        }

        private string SmokerCode
        {
            set { transferInPatientToOutPatientOrderedList[APSMOK] = value; }
        }

        private string WorkStationId
        {
            set { transferInPatientToOutPatientOrderedList[APIDWS] = value; }
        }


        private string ClinicalCommentsOne
        {
            set { transferInPatientToOutPatientOrderedList[APCOM1] = value; }
        }

        private string ClinicalCommentsTwo
        {
            set { transferInPatientToOutPatientOrderedList[APCOM2] = value; }
        }

        private string ClinicalCode
        {
            set { transferInPatientToOutPatientOrderedList[APCL01] = value; }
        }

        #region Admitting Physician

        private long AdmittingPhysicianNumber
        {
            set { transferInPatientToOutPatientOrderedList[APMDR_] = value; }
        }

        private string AdmittingPhysicianFullName
        {
            set { transferInPatientToOutPatientOrderedList[APMNDN] = value; }
        }

        private string AdmittingPhysicianLastName
        {
            set { transferInPatientToOutPatientOrderedList[APMNLN] = value; }
        }

        private string AdmittingPhysicianFirstName
        {
            set { transferInPatientToOutPatientOrderedList[APMNFN] = value; }
        }

        private string AdmittingPhysicianMiddleInitial
        {
            set { transferInPatientToOutPatientOrderedList[APMNMN] = value; }
        }

        private string AdmittingPhysicianNationalProviderId
        {
            set { transferInPatientToOutPatientOrderedList[APMNPR] = value; }
        }

        private string AdmittingPhysicianUPINNumber
        {
            set { transferInPatientToOutPatientOrderedList[APMNU_] = value; }
        }

        private int AdmittingPhysicianPhoneNumber
        {
            set { transferInPatientToOutPatientOrderedList[APMNP_] = value; }
        }

        private int AdmittingPhysicianAreaCode
        {
            set { transferInPatientToOutPatientOrderedList[APMNCD] = value; }
        }

        private string AdmittingPhysicianStateLicense
        {
            set { transferInPatientToOutPatientOrderedList[APMSL_] = value; }
        }

        #endregion

        #region Attending Physician

        private long AttendingPhysicianNumber
        {
            set { transferInPatientToOutPatientOrderedList[APADR_] = value; }
        }

        private string AttendingPhysicianLastName
        {
            set { transferInPatientToOutPatientOrderedList[APANLN] = value; }
        }

        private string AttendingPhysicianFirstName
        {
            set { transferInPatientToOutPatientOrderedList[APANFN] = value; }
        }

        private string AttendingPhysicianMiddleInitial
        {
            set { transferInPatientToOutPatientOrderedList[APANMN] = value; }
        }

        private string AttendingPhysicianNationalProviderId
        {
            set { transferInPatientToOutPatientOrderedList[APANPR] = value; }
        }

        private string AttendingPhysicianUPINNumber
        {
            set { transferInPatientToOutPatientOrderedList[APANU_] = value; }
        }

        private string AttendingPhysicianPhoneNumber
        {
            set { transferInPatientToOutPatientOrderedList[APANP_] = value; }
        }

        private string AttendingPhysicianStateLicense
        {
            set { transferInPatientToOutPatientOrderedList[APASL_] = value; }
        }

        #endregion

        #region Referring Physician

        private long ReferringPhysicianNumber
        {
            set { transferInPatientToOutPatientOrderedList[APRDR_] = value; }
        }

        private string ReferringPhysicianFullName
        {
            set { transferInPatientToOutPatientOrderedList[APRNDN] = value; }
        }

        private string ReferringPhysicianLastName
        {
            set { transferInPatientToOutPatientOrderedList[APRNLN] = value; }
        }

        private string ReferringPhysicianFirstName
        {
            set { transferInPatientToOutPatientOrderedList[APRNFN] = value; }
        }

        private string ReferringPhysicianMiddleInitial
        {
            set { transferInPatientToOutPatientOrderedList[APRNMN] = value; }
        }

        private string ReferringPhysicianNationalProviderId
        {
            set { transferInPatientToOutPatientOrderedList[APRNPR] = value; }
        }

        private string ReferringPhysicianPhoneNumber
        {
            set { transferInPatientToOutPatientOrderedList[APRNP_] = value; }
        }

        private string ReferringPhysicianUPINNumber
        {
            set { transferInPatientToOutPatientOrderedList[APRNU_] = value; }
        }

        private string ReferringPhysicianStateLicense
        {
            set { transferInPatientToOutPatientOrderedList[APRSL_] = value; }
        }

        #endregion

        #region Operating Physician

        private string OperatingPhysicianNumber
        {
            set { transferInPatientToOutPatientOrderedList[APOPD_] = value; }
        }

        private string OperatingPhysicianLastName
        {
            set { transferInPatientToOutPatientOrderedList[APONLN] = value; }
        }

        private string OperatingPhysicianFirstName
        {
            set { transferInPatientToOutPatientOrderedList[APONFN] = value; }
        }

        private string OperatingPhysicianMiddleInitial
        {
            set { transferInPatientToOutPatientOrderedList[APONMN] = value; }
        }

        private string OperatingPhysicianNationalProviderId
        {
            set { transferInPatientToOutPatientOrderedList[APONPR] = value; }
        }

        private string OperatingPhysicianUPINNumber
        {
            set { transferInPatientToOutPatientOrderedList[APONU_] = value; }
        }

        private string OperatingPhysicianPhoneNumber
        {
            set { transferInPatientToOutPatientOrderedList[APONP_] = value; }
        }

        private string OperatingPhysicianStateLicense
        {
            set { transferInPatientToOutPatientOrderedList[APOSL_] = value; }
        }

        #endregion

        #region PrimaryCare Physician

        private string PrimaryCarePhysicianNumber
        {
            set { transferInPatientToOutPatientOrderedList[APTDR_] = value; }
        }

        private string PrimaryCarePhysicianLastName
        {
            set { transferInPatientToOutPatientOrderedList[APTNLN] = value; }
        }

        private string PrimaryCarePhysicianFirstName
        {
            set { transferInPatientToOutPatientOrderedList[APTNFN] = value; }
        }

        private string PrimaryCarePhysicianMiddleInitial
        {
            set { transferInPatientToOutPatientOrderedList[APTNMN] = value; }
        }

        private string PrimaryCarePhysicianNationalProviderId
        {
            set { transferInPatientToOutPatientOrderedList[APTNPR] = value; }
        }

        private string PrimaryCarePhysicianUPINNumber
        {
            set { transferInPatientToOutPatientOrderedList[APTNU_] = value; }
        }

        private string PrimaryCarePhysicianPhoneNumber
        {
            set { transferInPatientToOutPatientOrderedList[APTNP_] = value; }
        }

        private string PrimaryCarePhysicianStateLicense
        {
            set { transferInPatientToOutPatientOrderedList[APTSL_] = value; }
        }

        #endregion

        #region Occurrance Span Details

        private string OccurranceSpanCodeOne
        {
            set
            {
                transferInPatientToOutPatientOrderedList[APSPNC] =
                    value;
            }
        }

        private DateTime OccurranceSpanFromDateOne
        {
            set
            {
                transferInPatientToOutPatientOrderedList[APSPFR] =
                    ConvertDateToIntInMddyyFormat( value );
            }
        }

        private DateTime OccurranceSpanToDateOne
        {
            set
            {
                transferInPatientToOutPatientOrderedList[APSPTO] =
                    ConvertDateToIntInMddyyFormat( value );
            }
        }

        private string OccurranceSpanCodeSecond
        {
            set
            {
                transferInPatientToOutPatientOrderedList[APSPN2] =
                    value;
            }
        }

        private DateTime OccurranceSpanSecondFromDateOne
        {
            set
            {
                transferInPatientToOutPatientOrderedList[APAPFR] =
                    ConvertDateToIntInMddyyFormat( value );
            }
        }

        private DateTime OccurranceSpanSecondToDateOne
        {
            set
            {
                transferInPatientToOutPatientOrderedList[APAPTO] =
                    ConvertDateToIntInMddyyFormat( value );
            }
        }

        private string OccurranceSpanFacility
        {
            set
            {
                transferInPatientToOutPatientOrderedList[APINSN] = value;
            }
        }

        #endregion

        #endregion

        #region Public Property

        public string TransactionFileId
        {
            set { transferInPatientToOutPatientOrderedList[APIDID] = value; }
        }

        public string SecurityCode
        {
            set { transferInPatientToOutPatientOrderedList[APSEC2] = value; }
        }

        private string AddChangeFlag
        {
            set { transferInPatientToOutPatientOrderedList[APACFL] = value; }
        }

        #endregion

        # region Condition Codes

        private string ConditionCode1
        {
            set { transferInPatientToOutPatientOrderedList[APCI01] = value; }
        }

        private string ConditionCode2
        {
            set { transferInPatientToOutPatientOrderedList[APCI02] = value; }
        }

        private string ConditionCode3
        {
            set { transferInPatientToOutPatientOrderedList[APCI03] = value; }
        }

        private string ConditionCode4
        {
            set { transferInPatientToOutPatientOrderedList[APCI04] = value; }
        }

        private string ConditionCode5
        {
            set { transferInPatientToOutPatientOrderedList[APCI05] = value; }
        }

        private string ConditionCode6
        {
            set { transferInPatientToOutPatientOrderedList[APCI06] = value; }
        }

        private string ConditionCode7
        {
            set { transferInPatientToOutPatientOrderedList[APCI07] = value; }
        }

        #endregion

        #region Data Elements

        private long nonStaffPhysicianNumber;
        private readonly OrderedList transferInPatientToOutPatientOrderedList = new OrderedList();

        #endregion

        #region Constants

        private const string
            AP_EXT = "AP#EXT";

        private const string
            APACC = "APACC";

        private const string
            APACCT = "APACCT";

        private const string
            APACFL = "APACFL";

        private const string
            APACTP = "APACTP";

        private const string
            APADMC = "APADMC";

        private const string
            APADR_ = "APADR#";

        private const string
            APANFN = "APANFN";

        private const string
            APANLN = "APANLN";

        private const string
            APANMN = "APANMN";

        private const string
            APANP_ = "APANP#";

        private const string
            APANPR = "APANPR";

        private const string
            APANU_ = "APANU#";

        private const string
            APAPFR = "APAPFR",
            APAPTO = "APAPTO";

        private const string
            APASL_ = "APASL#";

        private const string
            APATXC = "APATXC";

        private const string
            APBED = "APBED";

        private const string
            APBYPS = "APBYPS";

        private const string
            APCC01 = "APCC01",
            APCC02 = "APCC02",
            APCC03 = "APCC03";

        private const string
            APCI01 = "APCI01",
            APCI02 = "APCI02",
            APCI03 = "APCI03",
            APCI04 = "APCI04",
            APCI05 = "APCI05",
            APCI06 = "APCI06",
            APCI07 = "APCI07";

        private const string
            APCL01 = "APCL01",
            APCL02 = "APCL02",
            APCL03 = "APCL03",
            APCL04 = "APCL04",
            APCL05 = "APCL05";

        private const string
            APCNFG = "APCNFG";

        private const string
            APCOM1 = "APCOM1",
            APCOM2 = "APCOM2";

        private const string
            APCOND = "APCOND";

        private const string
            APDCRT = "APDCRT";

        private const string
            APDIAG = "APDIAG";

        private const string
            APDIET = "APDIET";

        private const string
            APDTOT = "APDTOT";

        private const string
            APFC = "APFC";

        private const string
            APHSP_ = "APHSP#";

        private const string
            APICRT = "APICRT";

        private const string
            APIDID = "APIDID";

        private const string
            APIDWS = "APIDWS";

        private const string
            APINLG = "APINLG";

        private const string
            APINSN = "APINSN";

        private const string
            APISO = "APISO";

        private const string
            APLAD = "APLAD",
            APLAT = "APLAT";

        private const string
            APLMD = "APLMD";

        private const string
            APLML = "APLML";

        private const string
            APLUL_ = "APLUL#";

        private const string
            APMDR_ = "APMDR#";

        private const string
            APMNCD = "APMNCD";

        private const string
            APMNDN = "APMNDN";

        private const string
            APMNFN = "APMNFN";

        private const string
            APMNFR = "APMNFR";

        private const string
            APMNLN = "APMNLN";

        private const string
            APMNMN = "APMNMN";

        private const string
            APMNP_ = "APMNP#";

        private const string
            APMNPR = "APMNPR";

        private const string
            APMNU_ = "APMNU#";

        private const string
            APMSL_ = "APMSL#";

        private const string
            APMSV = "APMSV";

        private const string
            APMTXC = "APMTXC";

        private const string
            APNCRT = "APNCRT";

        private const string
            APNS = "APNS";

        private const string
            APONFN = "APONFN";

        private const string
            APONLN = "APONLN";

        private const string
            APONMN = "APONMN";

        private const string
            APONP_ = "APONP#";

        private const string
            APONPR = "APONPR";

        private const string
            APONU_ = "APONU#";

        private const string
            APOPD_ = "APOPD#";

        private const string
            APORR1 = "APORR1",
            APORR2 = "APORR2",
            APORR3 = "APORR3";

        private const string
            APOSL_ = "APOSL#";

        private const string
            APOTXC = "APOTXC";

        private const string
            APPARC = "APPARC";

        private const string
            APPSRC = "APPSRC";

        private const string
            APPTYP = "APPTYP";

        private const string
            APQNUM = "APQNUM";

        private const string
            APRA01 = "APRA01",
            APRA02 = "APRA02",
            APRA03 = "APRA03",
            APRA04 = "APRA04",
            APRA05 = "APRA05";

        private const string
            APRDR_ = "APRDR#";

        private const string
            APRLGN = "APRLGN";

        private const string
            APRNDN = "APRNDN";

        private const string
            APRNFN = "APRNFN";

        private const string
            APRNFR = "APRNFR";

        private const string
            APRNLN = "APRNLN";

        private const string
            APRNMN = "APRNMN";

        private const string
            APRNP_ = "APRNP#";

        private const string
            APRNPR = "APRNPR";

        private const string
            APRNU_ = "APRNU#";

        private const string
            APROOM = "APROOM";

        private const string
            APRR_ = "APRR#";

        private const string
            APRSL_ = "APRSL#";

        private const string
            APRTXC = "APRTXC";

        private const string
            APSCRT = "APSCRT";

        private const string
            APSEC2 = "APSEC2";

        private const string
            APSECR = "APSECR";

        private const string
            APSMOK = "APSMOK";

        private const string
            APSPFR = "APSPFR";

        private const string
            APSPN2 = "APSPN2";

        private const string
            APSPNC = "APSPNC";

        private const string
            APSPTO = "APSPTO";

        private const string
            APSWPY = "APSWPY";

        private const string
            APTCRT = "APTCRT";

        private const string
            APTDAT = "APTDAT";

        private const string
            APTDR_ = "APTDR#";

        private const string
            APTNFN = "APTNFN";

        private const string
            APTNLN = "APTNLN";

        private const string
            APTNMN = "APTNMN";

        private const string
            APTNP_ = "APTNP#";

        private const string
            APTNPR = "APTNPR";

        private const string
            APTNU_ = "APTNU#";

        private const string
            APTREG = "APTREG";

        private const string
            APTSL_ = "APTSL#";

        private const string
            APTTME = "APTTME";

        private const string
            APTTXC = "APTTXC";

        private const string
            APUPRV = "APUPRV",
            APUPRW = "APUPRW";

        private const string
            APVALU = "APVALU";

        private const string
            APWSIR = "APWSIR";

        private const string
            APXMIT = "APXMIT";

        private const string
            APZDTE = "APZDTE",
            APZTME = "APZTME";

        private const string
            APNHACF = "APNHACF";

        private const int
            LENGTH_OF_COMMENTS_TWO = 55;

        private const int
            LENGTH_OF_IN_TO_OUT_TRANSFER_REMARKS = 65,
            TOTAL_LENGTH_OF_PBAR_COMMENTS = 65;

        private const string TYPE_OF_REGISTRATION_FOR_OUTPATIENT = "1";
        private const string ADMITTING_CATEGORY_FOR_INPATIENT = "3";
        #endregion
    }
}