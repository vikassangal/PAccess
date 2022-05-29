using System;
using System.Collections;
using PatientAccess.Domain.PAIWalkinOutpatientCreation;
using PatientAccess.Domain.QuickAccountCreation;
using PatientAccess.Domain.ShortRegistration;

namespace PatientAccess.Domain 
{
    [Serializable]
    public class HospitalService : CodedReferenceValue 
    {
        #region Event Handlers
        #endregion

        #region Methods

        public bool IsDayCare()
        {
            return this.Code == MED_OBSV_PT_IN_BED ||
                   Code == OUTPT_IN_BED_NON_OBS ||
                   this.Code == OUTPATIENT_FORENSIC ||
                   this.Code == SURG_DIAG_OBS_PT_BED ||
                   this.Code == LABOR_AND_DEL_OBSERVAT ||
                   this.Code == LEASED_BEDS ||
                   this.Code == ORGAN_HARVESTING ;
        }


        public override string ToString()
        {   // Display 2-digit code (with zero-placeholder if only 1 digit) and description
            return String.Format("{0:00} {1}", Code, Description);
        }

        /// <summary>
        /// Returns Valid Hospital Services for a given Visit Type (patient type)
        /// </summary>
        /// <param name="facilityHospitalServices"></param>
        /// <param name="patientTypeCode"></param>
        /// <returns></returns>
        public ICollection HospitalServicesFor( ArrayList facilityHospitalServices, string patientTypeCode )
        {
            ArrayList hospitalServicesList = new ArrayList();
 
            foreach( HospitalService hsv in facilityHospitalServices )
            {
                    switch( patientTypeCode )
                    {                    
                        case VisitType.PREREG_PATIENT:
                        {
                            if( hsv.IPTransferRestriction .Equals( "N" ) &&
                                hsv.OPFlag .Equals( "M" ) )
                            {
                                if( this.HSVValidForPatientType( patientTypeCode, hsv ) )
                                {
                                    hospitalServicesList.Add( hsv );
                                }
                                
                            }
                            break;
                        }
                        case VisitType.INPATIENT:
                        { 
                            if( !hsv.IPTransferRestriction .Equals( "N" ) )
                            {
                                if( this.HSVValidForPatientType( patientTypeCode, hsv ) )
                                {
                                    hospitalServicesList.Add( hsv );
                                }
                            }
                            break;
                        }                
                        default:
                        {
                            if( hsv.OPFlag .Equals( "Y" ) )
                            {
                                if( this.HSVValidForPatientType( patientTypeCode, hsv ) )
                                {
                                    hospitalServicesList.Add( hsv );
                                }
                            }
                            break;
                        }
                    } 
                }
 
            bool blankEntryFound = false;
            foreach( HospitalService sv in hospitalServicesList )
            {
                if( sv.Code.Equals(String.Empty)  )
                {
                    blankEntryFound = true;
                    break;
                }
            }
            if( !blankEntryFound )
            {
                this.InsertBlankHSV( facilityHospitalServices, hospitalServicesList );
            }

            return hospitalServicesList;
        }

        /// <summary>
        /// Returns Valid Hospital Services for a given Visit Type (patient type) and Day Care
        /// </summary>
        /// <param name="facilityHospitalServices"></param>
        /// <param name="patientTypeCode"></param>
        /// <param name="dayCare"></param>
        /// <returns></returns>
        public ICollection HospitalServicesFor( ArrayList facilityHospitalServices, string patientTypeCode, string dayCare )
        {
            ArrayList patientHSV = new ArrayList();

            foreach( HospitalService hsv in facilityHospitalServices )
            {
                
                    switch( patientTypeCode )
                    {                    
                        case VisitType.PREREG_PATIENT:
                        {
                            break;
                        }
                        case VisitType.INPATIENT:
                        { 
                            break;
                        } 
                        case VisitType.OUTPATIENT:
                        case VisitType.EMERGENCY_PATIENT:
                        case VisitType.RECURRING_PATIENT:
                        case VisitType.NON_PATIENT:
                        {    
                            if( hsv.OPFlag.Equals( "Y" ) && hsv.DayCare == dayCare)
                            {
                                if( this.HSVValidForPatientType( patientTypeCode, hsv ) )
                                {
                                    patientHSV.Add( hsv );
                                }
                            }
                            break;                        
                        }
                    } 
                }

            this.InsertBlankHSV( facilityHospitalServices, patientHSV );

            return patientHSV;
        }

        public ICollection HospitalServicesFor( ArrayList facilityHospitalServices,             
            VisitType patientType,
            Activity activity,
            HospitalService referenceHospitalService,
            FinancialClass financialClass )
        {
            ArrayList hospitalServices = new ArrayList();

            ArrayList filteredHospitalServicesList = ( ArrayList )
                HospitalServicesFor( facilityHospitalServices, patientType.Code );

            // change later to preregistration activity
            if( activity.GetType().Equals( typeof ( RegistrationActivity )) )
            { 
                hospitalServices = filteredHospitalServicesList;
            }
                // change later to registration activity
            else if( activity.GetType() == typeof( PreRegistrationActivity ) ||
                activity.GetType() == typeof( QuickAccountCreationActivity ) ||
                activity.GetType() == typeof( QuickAccountMaintenanceActivity ) ||
                activity.GetType() == typeof( PAIWalkinOutpatientCreationActivity ))
                
            {
                switch( patientType.Code )
                {
                    case VisitType.EMERGENCY_PATIENT:
                    {
                        if( financialClass.Code.Equals( FinancialClass.MED_SCREEN_EXM_CODE ) )
                        {
                            foreach( HospitalService hsv in filteredHospitalServicesList )
                            {
                                if( !( CheckForBedAssignment( hsv.Code ) ) )
                                {
                                    if( this.HSVValidForPatientType( patientType.Code, hsv ) )
                                    {
                                        hospitalServices.Add( hsv );
                                    }
                                }                
                            }                            
                        }
                        break;
                    }
                    default:
                    {
                        hospitalServices = filteredHospitalServicesList;
                        break;
                    }
                }
            }
            else if( activity.GetType().Equals( typeof( MaintenanceActivity ) ) )
            {
                switch( patientType.Code )
                {
                    case VisitType.PREREG_PATIENT:
					case VisitType.OUTPATIENT:
					case VisitType.NON_PATIENT:
                    {
                        hospitalServices = filteredHospitalServicesList;
                        break;
                    }
                    case VisitType.INPATIENT:
                    {     
                        foreach( HospitalService hsv in filteredHospitalServicesList )
                        {  
                            if( hsv.IPTransferRestriction.Equals( 
                                referenceHospitalService.IPTransferRestriction ) )
                            {
                                if( this.HSVValidForPatientType( patientType.Code, hsv ) )
                                {
                                    hospitalServices.Add( hsv );
                                }
                            }
                        }
                        break;
                    }
                    case VisitType.RECURRING_PATIENT:
                    {     
                        foreach( HospitalService hsv in filteredHospitalServicesList )
                        {  
                            if( hsv.DayCare.Equals( referenceHospitalService.DayCare ) )
                            {
                                if( this.HSVValidForPatientType( patientType.Code, hsv ) )
                                {
                                    hospitalServices.Add( hsv );
                                }
                            }
                        }
                        break;
                    }
                    default:
                    {
                        foreach( HospitalService hsv in filteredHospitalServicesList )
                        {
                            if( !( CheckForBedAssignment( hsv.Code ) ) )
                            {
                                if( this.HSVValidForPatientType( patientType.Code, hsv ) )
                                {
                                    hospitalServices.Add( hsv );
                                }
                            }                
                        }
                        break;
                    }
                }
            }
            else if ( activity.GetType().Equals( typeof( ShortPreRegistrationActivity ) ) )
            {
                foreach ( HospitalService hsv in filteredHospitalServicesList )
                {
                    if ( hsv.Code.Equals( PRE_REGISTER )  )
                    {
                        hospitalServices.Add( hsv );
                        break;
                    }
                }
            }
            else if ( activity.GetType().Equals( typeof( ShortRegistrationActivity ) ) )
            {
                foreach ( HospitalService hsv in filteredHospitalServicesList )
                {
                    if ( IsShortRegOutpatientAndRecurringPatientHSV( hsv.Code ) && HSVValidForPatientType( patientType.Code, hsv ) )
                    {
                        hospitalServices.Add( hsv );
                    }
                }
            }
            else if ( activity.GetType().Equals( typeof( ShortMaintenanceActivity ) ) )
            {
                switch ( patientType.Code )
                {
                    case VisitType.PREREG_PATIENT:
                        foreach ( HospitalService hsv in filteredHospitalServicesList )
                        {
                            if ( hsv.Code.Equals( PRE_REGISTER ) )
                            {
                                hospitalServices.Add( hsv );
                                break;
                            }
                        }
                        break;
                    case VisitType.OUTPATIENT:
                          foreach (HospitalService hsv in filteredHospitalServicesList)                            
                          {
                                if (hsv.OPFlag.Equals("Y") && hsv.DayCare == referenceHospitalService.DayCare)
                                {
                                    if (IsShortRegOutpatientAndRecurringPatientHSV(hsv.Code) && this.HSVValidForPatientType(patientType.Code, hsv))
                                    {
                                        hospitalServices.Add(hsv);
                                    }
                                }
                          }
                        break;
                    case VisitType.NON_PATIENT:
                        {
                            hospitalServices = filteredHospitalServicesList;
                            break;
                        }
                    case VisitType.INPATIENT:
                        {
                            foreach ( HospitalService hsv in filteredHospitalServicesList )
                            {
                                if ( hsv.IPTransferRestriction.Equals(
                                    referenceHospitalService.IPTransferRestriction ) )
                                {
                                    if ( this.HSVValidForPatientType( patientType.Code, hsv ) )
                                    {
                                        hospitalServices.Add( hsv );
                                    }
                                }
                            }
                            break;
                        }
                    case VisitType.RECURRING_PATIENT:
                        {
                            foreach ( HospitalService hsv in filteredHospitalServicesList )
                            {
                                if ( hsv.DayCare.Equals( referenceHospitalService.DayCare ) )
                                {
                                    if ( IsShortRegOutpatientAndRecurringPatientHSV(hsv.Code) && this.HSVValidForPatientType( patientType.Code, hsv ) )
                                    {
                                        hospitalServices.Add( hsv );
                                    }
                                }
                            }
                            break;
                        }
                    default:
                        {
                            foreach ( HospitalService hsv in filteredHospitalServicesList )
                            {
                                if ( !( CheckForBedAssignment( hsv.Code ) ) )
                                {
                                    if ( this.HSVValidForPatientType( patientType.Code, hsv ) )
                                    {
                                        hospitalServices.Add( hsv );
                                    }
                                }
                            }
                            break;
                        }
                }
            }
            else if ( activity.GetType().Equals( typeof( AdmitNewbornActivity ) )  )
            {
                foreach( HospitalService hsv in facilityHospitalServices )
                {
                    if( hsv.Code.Equals(NURSERY_ACUTE) || 
                        hsv.Code.Equals(NEONATAL_ACUTE) || 
                        hsv.Code.Equals(NURSERY_INTENSIVE) )
                    {
                        if( this.HSVValidForPatientType( patientType.Code, hsv ) )
                        {
                            hospitalServices.Add( hsv );
                        }
                    }                
                }            
            }
            else if ( activity.GetType().Equals( typeof( PreAdmitNewbornActivity ) ) )
            {
                foreach ( HospitalService hsv in filteredHospitalServicesList )
                {
                    if ( hsv.Code.Equals( PRE_ADMIT ) )
                    {
                        hospitalServices.Add( hsv );
                        break;
                    }
                }
            }
			else if( activity.GetType().Equals( typeof( PostMSERegistrationActivity ) ) )
			{
				hospitalServices = filteredHospitalServicesList;
			}

            this.InsertBlankHSV( facilityHospitalServices, hospitalServices );

            return hospitalServices;
        }

        public static bool CheckForBedAssignment( string hsvCode )
        {
            // In case of maintenance activity, for patient types other than 0 (PreReg) 
            // and 1 (InPatient), following four Daycare HSVs should not be shown
            return( hsvCode.Equals( MED_OBSV_PT_IN_BED ) ||
                    hsvCode.Equals( OUTPT_IN_BED_NON_OBS ) || 
                    hsvCode.Equals( OUTPATIENT_FORENSIC ) ||
                    hsvCode.Equals( SURG_DIAG_OBS_PT_BED ) || 
                    hsvCode.Equals( LEASED_BEDS ) || 
                    hsvCode.Equals( LABOR_AND_DEL_OBSERVAT ) ||
                    hsvCode.Equals(ORGAN_HARVESTING));
        }

        private bool IsShortRegOutpatientAndRecurringPatientHSV( string hsvCode )
        {
            return ( hsvCode.Equals( ACUTE_CARE_CLINIC_1 ) ||
                     hsvCode.Equals( ACUTE_CARE_CLINIC_2 ) ||
                     hsvCode.Equals( ACUTE_CARE_CLINIC_3 ) ||
                     hsvCode.Equals( ACUTE_CARE_CLINIC_4 ) ||
                     hsvCode.Equals( ACUTE_CARE_CLINIC_5 ) ||
                     hsvCode.Equals( ACUTE_CARE_CLINIC_6 ) ||
                     hsvCode.Equals( ACUTE_CARE_CLINIC_7 ) ||
                     hsvCode.Equals( PSYCH_OUTPATIENT ) ||
                     hsvCode.Equals( PSYCH_INTENSIVE_OP ) ||
                     hsvCode.Equals( CLINIC_7_OFFSITE ) ||
                     hsvCode.Equals( CLINIC_1_ONSITE ) ||
                     hsvCode.Equals( CLINIC_2_ONSITE ) ||
                     hsvCode.Equals( CLINIC_1_OFFSITE ) ||
                     hsvCode.Equals( CLINIC_2_OFFSITE ) ||
                     hsvCode.Equals( DIAGNOSTIC_OUTPT ) ||
                     hsvCode.Equals( CLINIC_3_OFFSITE ) ||
                     hsvCode.Equals( CLINIC_4_OFFSITE ) ||
                     hsvCode.Equals( CLINIC_5_OFFSITE ) ||
                     hsvCode.Equals( CLINIC_6_OFFSITE ) ||
                     hsvCode.Equals( BURN_REHAB ) ||
                     hsvCode.Equals( HOME_HEALTH ) ||
                     hsvCode.Equals( OUTPATIENT_REHAB ) ||
                     hsvCode.Equals( DAY_TREATMENT_PRGM ) ||
                     hsvCode.Equals( COMMUNITY_REENTRY ) ||
                     hsvCode.Equals( WORK_HARDENING_PRGM ) ||
                     hsvCode.Equals( CORRECTIONAL_OUTPT ) ||
                     hsvCode.Equals( CLIENT_ONE ) ||
                     hsvCode.Equals( CLIENT_TWO ) ||
                     hsvCode.Equals( FACILITY ) ||
                     hsvCode.Equals( PALLIATIVE ) ||
                     hsvCode.Equals( RADIATION_ONCOLOGY ) ||
                     hsvCode.Equals( RESEARCH_STUDY ) ||
                     hsvCode.Equals( SPECIMEN_ONLY ) ||
                     hsvCode.Equals( COVID_VACCINE_CLINIC ) ||
                     hsvCode.Equals( TENET_CARE ) ||
                     hsvCode.Equals( WALK_IN ) );
        }

        public bool IsHSV_ValidForDuplicateOcccurenceCode50()
        {
            return (this.Code.Equals(SNF_EXTENDED_CARE) ||
                    this.Code.Equals(SUB_ACUTE_NON_VENT) ||
                    this.Code.Equals(SWING_BED) ||
                    this.Code.Equals(SUB_ACUTE_NV_B_HOLD) ||
                    this.Code.Equals(VENT_DEPEND_BED_HLD) ||
                    this.Code.Equals(VENT_DEPEND_SUB_ACUT));
        }

        public bool IsPsychCode()
        {
            return (this.Code.Equals(PSYCH_OUTPATIENT)||
                    this.Code.Equals(PSYCH_INTENSIVE_OP)||
                    this.Code.Equals(PSYCH_NON_LOCKED)||
                    this.Code.Equals(PSYCH_LOCKED)||
                    this.Code.Equals(DETOXIFICATION)||
                    this.Code.Equals(PSYCH_NONEXEMPT)||
                    this.Code.Equals(RESIDENTIAL_PSYCH)||
                    this.Code.Equals(PSYCH_ADOLESCENT)||
                    this.Code.Equals(PSYCH_CHILD)||
                    this.Code.Equals(CHEMICAL_DEPENDENCY)||
                    this.Code.Equals(PSYCH_BED_HOLD));
        }
        
        #endregion

        #region Properties

        public string IPTransferRestriction
        {
            get
            {
                return i_IPFlag;
            }
            set
            {
                i_IPFlag = value;
            }
        }

        public string OPFlag
        {
            private get
            {
                return i_OPFlag;
            }
            set
            {
                i_OPFlag = value;
            }
        }

        public string DayCare
        {
            get
            {
                return i_DayCareFlag;
            }
            set
            {
                i_DayCareFlag = value;
            }
        }

        public long FacilityOid
        {
            get
            {
                return i_FacilityOid;
            }
            set
            {
                i_FacilityOid = value;
            }
        }

        public static HospitalService URGENT_CARE_HSV
        {
            get
            {
                return new HospitalService(NEW_OID, NEW_VERSION, URGENTCARE_DESCRIPTION, URGENTCARE_CODE);
            }
        }

        public bool IsPreAdmitCode
        {
            get { return ( Code == PRE_ADMIT ); }
        }

        #endregion

        #region Private Methods

        private bool HSVValidForPatientType( string patientType, HospitalService hs)
        {
            if( hs.Code.Equals( EMERGENCY_ROOM ) )
            {
                if( patientType == VisitType.EMERGENCY_PATIENT )                
                {
                    return true;
                }      
                else
                {
                    return false;
                }
            }

			if( hs.Code.Equals( BURN_REHAB ) || 
				hs.Code.Equals( AMBULANCE) )
			{
				if( patientType == VisitType.INPATIENT )
				{
					return false;
				}
				else
				{
					return true;
				}
			}

			if( hs.Code.Equals( BARIATRICS ) )
			{
				if( patientType == VisitType.INPATIENT )
				{
					return true;
				}
				else
				{
					return false;
				}
			}

            if( hs.Code.Equals( CLIENT_ONE ) 
                || hs.Code.Equals( CLIENT_TWO ) )
            {
                if(( patientType == VisitType.OUTPATIENT ) ||
                    ( patientType == VisitType.EMERGENCY_PATIENT )||
                    ( patientType == VisitType.RECURRING_PATIENT )||
                    ( patientType == VisitType.NON_PATIENT )
                    )
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            if (hs.Code.Equals(ORGAN_HARVESTING))
            {
                if ((patientType == VisitType.INPATIENT) ||
                    (patientType == VisitType.OUTPATIENT) ||
                    (patientType == VisitType.NON_PATIENT))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            if( hs.Code.Equals( RADIATION_ONCOLOGY ) && ( patientType == VisitType.EMERGENCY_PATIENT ) )
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void InsertBlankHSV( ArrayList facilityHospitalServices, ArrayList specificHospitalServices )
        {
            bool addBlankHsv = false;

            if( specificHospitalServices.Count == 0)
            {
                addBlankHsv = true;
            }
            else if( ((HospitalService) specificHospitalServices[0]).Code.Trim().Length != 0 )
            {
                addBlankHsv = true;
            }

            if( addBlankHsv )
            {
                foreach( HospitalService hsv in facilityHospitalServices )
                {
                    if(hsv.Code.Trim().Length == 0)
                    {
                        specificHospitalServices.Insert(0, hsv );
                    }
                }                
            }
        }
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public HospitalService()
        {
        }
        public HospitalService( long oid, DateTime version, string description )
            : base( oid, version, description )
        {
        }

        public HospitalService( long oid, DateTime version, string description, string code )
            : base( oid, version, description, code )
        {
        }

        public HospitalService( long oid, DateTime version, string description, string code, string ipFlag, string opFlag, string dayCareFlag, long facilityOid )
            : base( oid, version, description, code )
        {
            i_IPFlag = ipFlag;
            i_OPFlag = opFlag;
            i_DayCareFlag = dayCareFlag;
            i_FacilityOid = facilityOid;
        }
        #endregion

        #region Data Elements
        private string i_IPFlag;
        private string i_OPFlag;
        private string i_DayCareFlag;  
        private long i_FacilityOid;
        #endregion

        #region Constants

        public static readonly string BLANK_CODE = string.Empty;
        public const string  AMBULANCE              = "AB";
        public const string  LEASED_BEDS            = "LB";
        private const string LABOR_AND_DEL_OBSERVAT = "LD";
        private const string OUTPT_IN_BED_NON_OBS   = "57";
        public const string  HSV57                  = OUTPT_IN_BED_NON_OBS;
        private const string MED_OBSV_PT_IN_BED     = "58";
        public const string  HSV58                  = MED_OBSV_PT_IN_BED;
        public const string SURG_DIAG_OBS_PT_BED   = "59";
        public const string  HSV59                  = SURG_DIAG_OBS_PT_BED;
        private const string NURSERY_ACUTE           = "30";
        private const string NEONATAL_ACUTE          = "31";
        private const string NURSERY_INTENSIVE       = "40";
        public const string  EMERGENCY_ROOM         = "65";
        private const string BARIATRICS              = "BA";
        public const string  HSV_OB_16              = "16";
        private const string OUTPATIENT_FORENSIC    = "FO";

        public const string SNF_EXTENDED_CARE = "95";
        private const string SUB_ACUTE_NON_VENT = "96";
        public const string SUB_ACUTE_NV_B_HOLD = "97";
        private const string SWING_BED = "SB";
        private const string VENT_DEPEND_BED_HLD = "VB";
        private const string VENT_DEPEND_SUB_ACUT = "VE";

        //Psych HSV Codes
        //Declared under Short Reg HSVs 
        //PSYCH_OUTPATIENT = "08"
        //PSYCH_INTENSIVE_OP = "09"
        public const string PSYCH_NON_LOCKED = "14",
                            PSYCH_LOCKED = "15",
                            DETOXIFICATION = "21",
                            PSYCH_NONEXEMPT = "23",
                            RESIDENTIAL_PSYCH = "24",
                            PSYCH_ADOLESCENT = "28",
                            PSYCH_CHILD = "29",
                            CHEMICAL_DEPENDENCY = "72",
                            PSYCH_BED_HOLD = "99";



        // Short Reg HSVs
        public const string
            ACUTE_CARE_CLINIC_1 = "01",
            ACUTE_CARE_CLINIC_2 = "02",
            ACUTE_CARE_CLINIC_3 = "03",
            ACUTE_CARE_CLINIC_4 = "04",
            ACUTE_CARE_CLINIC_5 = "05",
            ACUTE_CARE_CLINIC_6 = "06",
            ACUTE_CARE_CLINIC_7 = "07",
            PSYCH_OUTPATIENT = "08",
            PSYCH_INTENSIVE_OP = "09",
            CLINIC_7_OFFSITE = "34",
            PRE_ADMIT = "35",
            CLINIC_1_ONSITE = "51",
            CLINIC_2_ONSITE = "52",
            CLINIC_1_OFFSITE = "53",
            CLINIC_2_OFFSITE = "54",
            DIAGNOSTIC_OUTPT = "60",
            CLINIC_3_OFFSITE = "61",
            CLINIC_4_OFFSITE = "62",
            CLINIC_5_OFFSITE = "63",
            CLINIC_6_OFFSITE = "64",
            BURN_REHAB = "69",
            HOME_HEALTH = "80",
            OUTPATIENT_REHAB = "90",
            DAY_TREATMENT_PRGM = "91",
            COMMUNITY_REENTRY = "93",
            WORK_HARDENING_PRGM = "94",
            CORRECTIONAL_OUTPT = "CO",
            CLIENT_ONE = "C1",
            CLIENT_TWO = "C2",
            FACILITY = "FA",
            PALLIATIVE = "PA",
            RADIATION_ONCOLOGY = "RO",
            RESEARCH_STUDY = "RS",
            TENET_CARE = "TC",
            WALK_IN = "WA",
            ORGAN_HARVESTING = "OH";

        public const string URGENTCARE_DESCRIPTION = "URGENT CARE CENTER";
        public const string URGENTCARE_CODE = "87";
        public const string
            PRE_REGISTER = "36",
            SPECIMEN_ONLY = "SP",
       COVID_VACCINE_CLINIC = "CV";
        #endregion
    }
}
