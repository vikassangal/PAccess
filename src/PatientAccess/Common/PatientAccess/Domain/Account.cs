using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Extensions.PersistenceCommon;
using Extensions.UI.Builder;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain.Auditing.FusNotes;
using PatientAccess.Domain.Parties;
using PatientAccess.Domain.ShortRegistration;
using PatientAccess.Utilities;

namespace PatientAccess.Domain
{
    [Serializable]
    public class Account : PersistentModel, IAccount
    {
        #region Event Handlers
        #endregion

        #region Methods

        public Account AsAccount()
        {
            return this;
        }

        public bool IsPatientTypeChangeable()
        {
            bool isPatientTypeChangeableForThisActivity = Activity.CanPatientTypeChange();

            //Filtering out maintenace activity here as the  dependency between 
            //Activity and KindOfVisit is only for maintenance activity
            if ( !( Activity is MaintenanceActivity || Activity is ShortMaintenanceActivity ) )
            {
                return isPatientTypeChangeableForThisActivity;
            }

            if ( KindOfVisit != null )
            {
                return KindOfVisit.IsPatientTypeChangeableFor( Location ) && isPatientTypeChangeableForThisActivity;
            }
            return true;
        }

        // An account is routed to the 8-tab view on Edit/Maintain if the account:
        // a) was Short-Registered or Short-PreRegistered with the Short-Reg flag set to 'Y' in PBAR (and)
        // b) is of PatientType - Outpatient (or) Recurring Patient (or) Prereg patient (and)
        // c) its hospital service code is not that of a bedded Outpatient.
        public bool IsShortRegisteredNonDayCareAccount()
        {
            return ( IsShortRegistered && KindOfVisit != null &&
                     ( KindOfVisit.Code == VisitType.OUTPATIENT ||
                       KindOfVisit.Code == VisitType.RECURRING_PATIENT ||
                       KindOfVisit.Code == VisitType.PREREG_PATIENT ) &&
                     HospitalService != null && !HospitalService.IsDayCare()
                   );
        }
        public bool IsDiagnosticPreregistrationAccount
        {
            get
            {
                return (Activity != null && (Activity.IsDiagnosticPreRegistrationActivity() ||
                                           IsDiagnosticPreRegistrationMaintenanceAccount()));
            }
        }
        
        public bool IsPreRegisteredMaintenanceAccount
        {
            get
            {
                return (KindOfVisit.Code == VisitType.PreRegistration.Code
                        && (Activity is MaintenanceActivity ||
                            Activity is ShortMaintenanceActivity)
                    );
            }
        }
        public bool IsNewBornRegistrationMaintenanceAccount
        {
            get
            {
                return (IsNewBorn && (Activity is MaintenanceActivity  )
                    );
            }
        }
        // An account is routed to the 8-tab view on Edit/Maintain if the account:
        // a) was Short-Registered or Short-PreRegistered with the Short-Reg flag set to 'Y' in PBAR (and)
        // b) is of PatientType - Outpatient (or) Recurring Patient (or) Prereg patient (and)
        // c) its hospital service code is not that of a bedded Outpatient.

        /// <summary>
        /// Returns all occurences of a physician relationship matching the physician role
        /// </summary>
        /// <param name="aRole">The PhysicianRole I'm looking for</param>
        /// <returns>A collection of all physician relationships matching the physician role</returns>
        public ICollection PhysicianRelationshipsWith( PhysicianRole aRole )
        {
            var physicianRelationships = new ArrayList();

            foreach ( PhysicianRelationship aRelationship in AllPhysicianRelationships )
            {
                if ( aRelationship.PhysicianRole.Role().Equals( aRole ) )
                {
                    physicianRelationships.Add( aRelationship );
                }
            }
            return physicianRelationships;
        }

        /// <summary>
        /// Returns all occurences of physicians matching the physician role
        /// </summary>
        /// <param name="aRole">The PhysicianRole I'm looking for</param>
        /// <returns>A collection of all physicians  matching the physician role</returns>
        public ICollection PhysiciansWith( PhysicianRole aRole )
        {
            var physicians = new ArrayList();

            foreach ( PhysicianRelationship aRelationship in AllPhysicianRelationships )
            {
                if ( aRelationship.PhysicianRole.Role().Equals( aRole ) )
                {
                    physicians.Add( aRelationship.Physician );
                }
            }

            return physicians;
        }

        /// <summary>
        /// Adds a physician relationship to the collection of physician relationships on the proxy.
        /// If a physician relationship exists in the collection with the identical physician role 
        /// being added, then that physician relationship will be replaced by the physician relationship
        /// being added.  Consulting physician relationships will allow a maximum of 5 relationships before 
        /// recycling a relationship.
        /// </summary>
        /// <param name="aPhysicianRelationship"></param>
        public void AddPhysicianRelationship( PhysicianRelationship aPhysicianRelationship )
        {
            //Question:  When adding a new physician do we replace any existing physician
            //with that same role?
            //Answer 08/09/2005 from Vic and Drew:  Yes, with no warning message!
            if ( IsConsultingPhysician( aPhysicianRelationship ) )
            {
                AddConsultingPhysicianRelationship( aPhysicianRelationship );
            }
            else
            {
                AddNonConsultingPhysicianRelationship( aPhysicianRelationship );
            }
        }

        /// <summary>
        /// Encapsulates a physician role and a physician into a physician relationship and then adds
        /// this relationship to the collection of all physician relationships.
        /// </summary>
        /// <param name="aRole"></param>
        /// <param name="aPhysician"></param>
        public void AddPhysicianWithRole( PhysicianRole aRole, Physician aPhysician )
        {
            var aPhysicianRelationship = new PhysicianRelationship( aRole, aPhysician );
            AddPhysicianRelationship( aPhysicianRelationship );
        }

        public bool IsQACPreRegAccount()
        {
            return ( IsQuickRegistered && KindOfVisit != null &&
                     ( KindOfVisit.Code == VisitType.PREREG_PATIENT )
                   );
        }

        public void ClearVisitType()
        {
            KindOfVisit = new VisitType();
        }

        public void GuarantorIs( Guarantor aGuarantor, RelationshipType aRelationshipType )
        {
            if ( aRelationshipType != null )
            {
                var aRelationship = new Relationship( aRelationshipType, Patient.GetType(), aGuarantor.GetType() );

                Patient.RemoveRelationship( aRelationshipType );
                Patient.AddRelationship( aRelationship );

                aGuarantor.RemoveRelationship( aRelationshipType );
                aGuarantor.AddRelationship( aRelationship );

                i_Guarantor = aGuarantor;
            }
        }

        public IDictionary PartiesForCopyingTo( Type kindOfParty )
        {
            var parties = new Hashtable();

            if ( kindOfParty != typeof( Patient ) && Patient != null )
            {
                parties.Add( "Patient", Patient );
                if ( Patient.Employment != null && Patient.Employment.Employer != null )
                {
                    parties.Add( "Patient's Employer", Patient.Employment.Employer );
                }
            }

            if ( kindOfParty != typeof( Guarantor ) && Guarantor != null )
            {
                parties.Add( "Guarantor", Guarantor );
            }

            if ( kindOfParty != typeof( Insured ) && PrimaryInsured != null )
            {
                parties.Add( "Insured - Primary", PrimaryInsured );
            }

            if ( kindOfParty != typeof( Insured ) && SecondaryInsured != null )
            {
                parties.Add( "Insured - Secondary", SecondaryInsured );
            }

            return (IDictionary)parties.Clone();
        }

        public IDictionary PartiesForCopyingTo( Type kindOfParty, CoverageOrder cov )
        {
            var parties = new Hashtable();

            if ( kindOfParty != typeof( Patient ) &&
                Patient != null &&
                Patient.Name != null &&
                Patient.Name.LastName != String.Empty &&
                Patient.Name.FirstName != String.Empty )
            {
                parties.Add( "Patient", Patient );
                if ( Patient.Employment != null &&
                    Patient.Employment.Employer != null
                    )
                {
                    parties.Add( "Patient's Employer", Patient.Employment.Employer );
                }
            }
            // Defect 34698 Guarantor object created before data is entered by the user.
            if ( kindOfParty != typeof( Guarantor ) && Guarantor.LastName != String.Empty )
            {
                parties.Add( "Guarantor", Guarantor );
            }
            if ( kindOfParty != typeof( Insured ) && PrimaryInsured != null )
            {
                parties.Add( "Insured - Primary", PrimaryInsured );
            }
            if ( kindOfParty != typeof( Insured ) && SecondaryInsured != null )
            {
                parties.Add( "Insured - Secondary", SecondaryInsured );
            }

            if ( kindOfParty == typeof( Insured ) && PrimaryInsured != null && cov.Oid != CoverageOrder.PRIMARY_OID )
            {
                parties.Add( "Insured - Primary", PrimaryInsured );
            }

            if ( kindOfParty == typeof( Insured ) && SecondaryInsured != null && cov.Oid != CoverageOrder.SECONDARY_OID )
            {
                parties.Add( "Insured - Secondary", SecondaryInsured );
            }


            return (IDictionary)parties.Clone();
        }

        public IDictionary ContactPointsForCopyingToWithContext( String context )
        {
            var contactPoints = new Hashtable();

            ContactPoint generalContactPoint = null;
            ContactPoint tempContactPoint = null;

            if ( Patient != null && Patient.ContactPoints != null && Patient.ContactPoints.Count > 0 )
            {
                if ( context != Address.PatientMailing ) // add patient mailing
                {
                    generalContactPoint = new ContactPoint();
                    tempContactPoint = Patient.ContactPointWith( TypeOfContactPoint.NewMailingContactPointType() );
                    generalContactPoint.Address = (Address)tempContactPoint.Address.Clone();
                    // SR 54716 - County code should be displayed and saved only for 
                    // Patient Mailing Address, and hence should not be copied forward
                    
                    if (context != Address.PatientPhysical) // County should be copied when copying from Mailing to Physical 
                    {
                        generalContactPoint.Address.County = null;
                    }
                    
                    generalContactPoint.PhoneNumber = tempContactPoint.PhoneNumber;
                    generalContactPoint.EmailAddress = tempContactPoint.EmailAddress;
                    generalContactPoint.CellPhoneNumber =
                        Patient.ContactPointWith( TypeOfContactPoint.NewMobileContactPointType() ).PhoneNumber;
                    ContactPointWithContext( context, generalContactPoint );

                    if ( !generalContactPoint.IsTrivial() )
                        contactPoints.Add( "Patient - Mailing", generalContactPoint );
                }

                if ( context != Address.PatientPhysical ) // add patient physical
                {
                    generalContactPoint = new ContactPoint();
                    tempContactPoint = Patient.ContactPointWith( TypeOfContactPoint.NewPhysicalContactPointType() );
                    generalContactPoint.Address = (Address) tempContactPoint.Address.Clone();

                    if (context != Address.PatientMailing) // County should be copied when copying from Physical to Mailing 
                    {
                        generalContactPoint.Address.County = null;
                    }

                    generalContactPoint.PhoneNumber = tempContactPoint.PhoneNumber;
                    ContactPointWithContext( context, generalContactPoint );

                    if ( !generalContactPoint.IsTrivial() )
                        contactPoints.Add( "Patient - Physical", generalContactPoint );
                }
            }

            if ( context != "Guarantor" && Guarantor != null
                && Guarantor.ContactPoints != null && Guarantor.ContactPoints.Count > 0 )
            {
                generalContactPoint = new ContactPoint();
                tempContactPoint = Guarantor.ContactPointWith( TypeOfContactPoint.NewMailingContactPointType() );
                generalContactPoint.Address = (Address)tempContactPoint.Address.Clone();
                generalContactPoint.PhoneNumber = tempContactPoint.PhoneNumber;
                generalContactPoint.EmailAddress = tempContactPoint.EmailAddress;
                generalContactPoint.CellPhoneNumber =
                    Guarantor.ContactPointWith( TypeOfContactPoint.NewMobileContactPointType() ).PhoneNumber;
                ContactPointWithContext( context, generalContactPoint );

                if ( !generalContactPoint.IsTrivial() )
                    contactPoints.Add( "Guarantor", generalContactPoint );
            }

            if (context != "PrimaryInsured" && PrimaryInsured != null && !Activity.IsEditPreMseActivity() 
                && PrimaryInsured.ContactPoints != null && PrimaryInsured.ContactPoints.Count > 0 )
            {
                generalContactPoint = new ContactPoint();
                tempContactPoint = PrimaryInsured.ContactPointWith( TypeOfContactPoint.NewPhysicalContactPointType() );
                generalContactPoint.Address = (Address)tempContactPoint.Address.Clone();
                generalContactPoint.PhoneNumber = tempContactPoint.PhoneNumber;
                generalContactPoint.CellPhoneNumber =
                    PrimaryInsured.ContactPointWith( TypeOfContactPoint.NewMobileContactPointType() ).PhoneNumber;
                ContactPointWithContext( context, generalContactPoint );

                if ( !generalContactPoint.IsTrivial() )
                    contactPoints.Add( "Insured - Primary", generalContactPoint );
            }

            if ( context != "SecondaryInsured" && SecondaryInsured != null
                && SecondaryInsured.ContactPoints != null && SecondaryInsured.ContactPoints.Count > 0 )
            {
                generalContactPoint = new ContactPoint();
                tempContactPoint = SecondaryInsured.ContactPointWith( TypeOfContactPoint.NewPhysicalContactPointType() );
                generalContactPoint.Address = (Address)tempContactPoint.Address.Clone();
                generalContactPoint.PhoneNumber = tempContactPoint.PhoneNumber;
                generalContactPoint.CellPhoneNumber =
                    SecondaryInsured.ContactPointWith( TypeOfContactPoint.NewMobileContactPointType() ).PhoneNumber;
                ContactPointWithContext( context, generalContactPoint );

                if ( !generalContactPoint.IsTrivial() )
                    contactPoints.Add( "Insured - Secondary", generalContactPoint );
            }

            if ( context != "EmergencyContact1" && EmergencyContact1 != null
                && EmergencyContact1.ContactPoints != null && EmergencyContact1.ContactPoints.Count > 0 )
            {
                generalContactPoint =
                    (ContactPoint)
                    EmergencyContact1.ContactPointWith( TypeOfContactPoint.NewPhysicalContactPointType() ).Clone();
                ContactPointWithContext( context, generalContactPoint );
                if ( !generalContactPoint.IsTrivial() )
                    contactPoints.Add( "Emergency Contact 1", generalContactPoint );
            }

            if ( context != "EmergencyContact2" && EmergencyContact2 != null
                && EmergencyContact2.ContactPoints != null && EmergencyContact2.ContactPoints.Count > 0 )
            {
                generalContactPoint =
                    (ContactPoint)
                    EmergencyContact2.ContactPointWith( TypeOfContactPoint.NewPhysicalContactPointType() ).Clone();
                ContactPointWithContext( context, generalContactPoint );
                if ( !generalContactPoint.IsTrivial() )
                    contactPoints.Add( "Emergency Contact 2", generalContactPoint );
            }

            return contactPoints;
        }

        /// <summary>
        /// Adds OccurrenceCodes Generated by OccurrenceCodeManager
        /// </summary>
        /// <param name="code"></param>
        public void AddOccurrenceCode( OccurrenceCode code )
        {
            if ( code.IsAccidentCrimeOccurrenceCode() )
            {
                if ( i_OccurrenceCodes.Count == 0 )
                {
                    i_OccurrenceCodes.Add( code );
                }
                else
                {
                    i_OccurrenceCodes[0] = code;
                }
            }
            else
            {
                if ( !code.IsSystemOccurrenceCode( code ) )
                {
                    if ( OccurrenceCodes.Count < 20 )
                    {
                        if (
                            !(code.IsOccurenceCode50() && IsValidForDuplicateOccurenceCode50() &&
                              !HasMoreThan2OccurenceCodes50()))
                        {
                            RemoveOccurrenceCode(code);
                        }
                        i_OccurrenceCodes.Add( code );
                    }
                }
                else
                {
                    RemoveOccurrenceCode( code );
                    i_OccurrenceCodes.Sort();
                    if ( OccurrenceCodes.Count < 20 )
                    {
                        i_OccurrenceCodes.Add( code );
                    }
                    else if ( String.Compare( code.Code, ( (OccurrenceCode)i_OccurrenceCodes[19] ).Code ) < 0 )
                    {
                        i_OccurrenceCodes[19] = code;
                    }
                }
            }
        }

        /// <summary>
        /// Removes the MutualOccurrenceCodes  generated by <see cref="OccurrenceCodeManager"/> 
        /// for a Condition
        /// </summary>
        /// <param name="code"></param>
        public void RemoveOccurrenceCode(OccurrenceCode code)
        {
            if (code.IsAccidentCrimeOccurrenceCode())
            {
                if (i_OccurrenceCodes.Count > 0)
                {
                    i_OccurrenceCodes[0] = new OccurrenceCode();
                }
            }
            else
            {
                foreach (OccurrenceCode occ in OccurrenceCodes)
                {
                    if (occ.Code == code.Code)
                    {
                        i_OccurrenceCodes.Remove(occ);
                        break;
                    }

                }
            }
        }
        public  bool RemoveOccurrenceCodes50()
        {
            return (!this.IsValidForDuplicateOccurenceCode50() && this.NumberOf50OccurrenceCodes() > 0);
        }
        /// <summary>
        /// Add an ConditionCodes by like UB, MSP, Diagnosis, etc views
        /// </summary>
        /// <param name="code"></param>
        public void AddConditionCode( ConditionCode code )
        {
            if ( !code.IsSystemConditionCode() )
            {
                if ( ConditionCodes.Count < 7 )
                {
                    RemoveConditionCode( code );
                    i_ConditionCodes.Add( code );
                }
            }
            else
            {
                i_ConditionCodes.Sort();
                if ( ConditionCodes.Count < 7 )
                {
                    i_ConditionCodes.Add( code );
                }
                else if ( String.Compare( code.Code, ( (ConditionCode)i_ConditionCodes[6] ).Code ) < 0 )
                {
                    i_ConditionCodes.RemoveAt( 6 );
                    i_ConditionCodes.Add( code );
                }
            }
        }

        /// <summary>
        /// Removes a ConditionCode  
        /// </summary>
        /// <param name="code"></param>
        public void RemoveConditionCode( ConditionCode code )
        {
            if ( ConditionCodes.Contains( code ) )
            {
                i_ConditionCodes.Remove( code );
            }
        }

        /// <summary>
        /// This method can be used to check if a new Research Study can be added 
        /// to an account before trying to add the Research Study, to avoid getting 
        /// an exception thrown for trying to add more than ten Research Studies.
        /// </summary>
        public bool CanAddConsentedResearchStudy()
        {
            return ClinicalResearchStudies.Count() < 10;
        }

        /// <summary>
        /// Add ClinicalResearchStudies that the patient is enrolled in. 
        /// The method <c>CanAddConsentedResearchStudy</c> can be used to check
        /// if a new Research Study can be added to an account, to avoid getting 
        /// an exception thrown for trying to add more than ten Research Studies.
        /// </summary>
        /// <param name="consentedResearchStudy"></param>
        /// <exception cref="InvalidOperationException">When attempting to add a eleventh Research Study element to an account.</exception>
        /// <exception cref="ArgumentNullException"><c>consentedResearchStudy</c> is null.</exception>
        public void AddConsentedResearchStudy( ConsentedResearchStudy consentedResearchStudy )
        {
            Guard.ThrowIfArgumentIsNull( consentedResearchStudy, "consentedResearchStudy" );

            if ( ClinicalResearchStudies.Count() < 10 )
            {
                RemoveConsentedResearchStudy( consentedResearchStudy );
                i_ClinicalResearchStudies.Add( consentedResearchStudy );
            }
            else
            {
                throw new InvalidOperationException( "Cannot add more than ten Research Studies for an account." );
            }
        }

        public void AddFusNote( FusNote note )
        {
            if ( FusNotes.Contains( note ) == false )
            {
                i_FusNotes.Add( note );
            }
        }

        public ICollection GetAllRemainingActions()
        {
            var result = new ArrayList();
            foreach ( DictionaryEntry entry in RemainingActions )
            {
                foreach ( IAction action in entry.Value as ActionsList )
                {
                    if ( !result.Contains( action ) )
                    {
                        result.Add( action );
                    }
                }
            }

            return result;
        }

        public static int Mod10( int accountNumberSeed )
        {
            try
            {
                var table =
                    new int[2, 10] { { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, { 0, 2, 4, 6, 8, 1, 3, 5, 7, 9 } };

                string strAccountNumberSeed = accountNumberSeed.ToString();

                int accountNumberSeedLength = strAccountNumberSeed.Length;
                int weightedSum = 0;
                int odd = 0;
                int startVal = accountNumberSeedLength - 1;

                for ( int accountNumberSeedIterator = startVal;
                     accountNumberSeedIterator >= 0;
                     accountNumberSeedIterator-- )
                {
                    if ( Char.IsDigit(
                        strAccountNumberSeed[accountNumberSeedIterator] ) )
                    {
                        weightedSum +=
                            table[odd = 1 - odd,
                                  strAccountNumberSeed[accountNumberSeedIterator] - '0'];
                    }
                }

                weightedSum %= 10;

                return ( weightedSum != 0 ? 10 - weightedSum : 0 );
            }
            catch
            {
                return 0;
            }
        }

        public static int Mod11(int accountNumberSeed)
        {
            string straccountNumberSeed = accountNumberSeed.ToString();
            int accountNumberSeedLength = straccountNumberSeed.Length;
            int weightedSum = 0;
            int remainder = 0;
            int checkingFactor = 2;

            try
            {
                for (int accountNumberSeedIterator = accountNumberSeedLength;
                     accountNumberSeedIterator > 0;
                     accountNumberSeedIterator--)
                {
                    weightedSum += Convert.ToInt16(checkingFactor*
                                                   Char.GetNumericValue(
                                                       straccountNumberSeed[accountNumberSeedIterator - 1]));
                    checkingFactor++;
                    if (checkingFactor > 7)
                    {
                        checkingFactor = 2;
                    }
                }
                if ((remainder = weightedSum%11) == 0)
                {
                    return 0;
                }

                if (remainder == 1)
                {
                    return 0;
                }
                int checkDigit = 11 - remainder;
                return checkDigit;
            }
            catch
            {
                return -1;
            }
        }

        public void RemoveOccurrenceCode50IfNotApplicable()
        {
            if ( IsValidForDuplicateOccurenceCode50() ) return;

            var codesToRemove = OccurrenceCodes.Cast<OccurrenceCode>().ToList().Where( x => x.IsOccurenceCode50() );

            codesToRemove.ToList().ForEach( RemoveOccurrenceCode );

        }

        public bool HasMoreThan2OccurenceCodes50()
        {
            return (NumberOf50OccurrenceCodes() >= 2);
        }

        private int NumberOf50OccurrenceCodes()
        {
            return OccurrenceCodes.Cast<OccurrenceCode>().Count(occ => occ.Code == OccurrenceCode.OCCURENCECODE_DATE_OF_RA_EOMB);
        }
        public bool IsValidForDuplicateOccurenceCode50()
        {
            return (this.KindOfVisit.IsInpatient && HospitalService.IsHSV_ValidForDuplicateOcccurenceCode50());
        }
       
        public static bool IsValidAccountNumber( Facility facility, long accountNumber )
        {
            var modType = (int)facility.ModType;
            return IsValidAccountNumber( modType, accountNumber );
        }

        public static bool IsValidAccountNumber( int facilityModType, long accountNumber )
        {
            string acctNum = accountNumber.ToString();
            bool isValid = false;

            if ( acctNum.Length > 1 )
            {
                int droppedDigit = Convert.ToInt16( acctNum.Substring( acctNum.Length - 1, 1 ) );
                int accountNumSeed = Convert.ToInt32( acctNum.Substring( 0, acctNum.Length - 1 ) );

                int result = facilityModType == 10 ? Mod10( accountNumSeed ) : Mod11( accountNumSeed );
                if ( droppedDigit == result )
                {
                    isValid = true;
                }
                else // try with different facilityModType; facilityModType can be 10 OR 11  <--TG
                {
                    facilityModType = facilityModType == 10 ? 11 : 10;
                    result = facilityModType == 10 ? Mod10( accountNumSeed ) : Mod11( accountNumSeed );
                    if ( droppedDigit == result )
                    {
                        isValid = true;
                    }
                }
            }
            return isValid;
        }

        // Determines if the Patient type is disabled or enabled for the Activity and Visit Type. 

        /// <summary>
        /// Returns the first occurences of a physician relationship matching the physician role
        /// </summary>
        /// <param name="aRole">The PhysicianRole I'm looking for</param>
        /// <returns>A physician relationships matching the physician role</returns>
        public PhysicianRelationship PhysicianRelationshipWithRole( PhysicianRole aRole )
        {
            PhysicianRelationship aPhysicianRelationship = null;

            foreach ( PhysicianRelationship aRelationship in AllPhysicianRelationships )
            {
                if ( aRelationship.PhysicianRole.Role().Equals( aRole ) )
                {
                    aPhysicianRelationship = aRelationship;
                    break;
                }
            }

            return aPhysicianRelationship;
        }


        /// <summary>
        /// Removes the physician relationships.
        /// </summary>
        public void RemovePhysicianRelationships()
        {
            // Downcasting -- not great, but we own it
            ( (ArrayList)i_AllPhysicianRelationships ).Clear();
        }

        /// <summary>
        /// Removes an exisiting physician relationship from the collection 
        /// of physician relationships
        /// </summary>
        /// <param name="aPhysicianRelationship"></param>
        public void RemovePhysicianRelationship( PhysicianRelationship aPhysicianRelationship )
        {
            try
            {
                foreach ( PhysicianRelationship aRelationship in AllPhysicianRelationships )
                {
                    if ( aRelationship.PhysicianRole.Role().Equals( aPhysicianRelationship.PhysicianRole.Role() ) )
                    {
                        int index = ( (ArrayList)AllPhysicianRelationships ).IndexOf( aRelationship );
                        ( (ArrayList)AllPhysicianRelationships ).RemoveAt( index );
                        break;
                    }
                }
            }
            catch ( NotSupportedException nse )
            {
                Console.WriteLine( nse.ToString() );
                throw;
            }
        }

        /// <summary>
        /// Removes all the relationships except the passed relationship from the collection.
        /// </summary>
        /// <param name="aPhysicianRelationship"></param>
        public void RemoveAllPhysicianRelationshipsExcept(PhysicianRelationship aPhysicianRelationship)
        {
            var allPhysicianRelationships = AllPhysicianRelationships.Cast<PhysicianRelationship>().ToList();

            var indicesOfRelationshipsToRemove = 
                from relationship in allPhysicianRelationships 
                where !relationship.PhysicianRole.Role().Equals(aPhysicianRelationship.PhysicianRole.Role()) 
                select ((ArrayList) AllPhysicianRelationships).IndexOf(relationship);
            
            foreach (var index in indicesOfRelationshipsToRemove)
            {
                ((ArrayList)AllPhysicianRelationships).RemoveAt(index);
            }
        }

        public void AddClinicalTrialsConditionCode( long facilityID )
        {
            var broker = BrokerFactory.BrokerOfType<IConditionCodeBroker>();
            ConditionCode clinicalTrialConditionCode = broker.ConditionCodeWith( facilityID,
                                    ConditionCode.CONDITIONCODE_QUALIFYING_CLINICAL_TRIALS );

            var tempConditionCodesList = ConditionCodes.Cast<ConditionCode>().ToList();

            foreach ( ConditionCode conditionCode in tempConditionCodesList )
            {
                if ( conditionCode.Code == clinicalTrialConditionCode.Code )
                {
                    RemoveConditionCode( conditionCode );
                }
            }

            AddConditionCode( clinicalTrialConditionCode );
        }

        public void RemoveClinicalTrialsConditionCode( long facilityID )
        {
            var broker = BrokerFactory.BrokerOfType<IConditionCodeBroker>();
            ConditionCode clinicalTrialConditionCode = broker.ConditionCodeWith( facilityID,
                                    ConditionCode.CONDITIONCODE_QUALIFYING_CLINICAL_TRIALS );

            RemoveConditionCode( clinicalTrialConditionCode );
        }

        public void RemovePreMseFinancialClass()
        {
            if ( FinancialClass != null )
            {
                if ( FinancialClass.Code == FinancialClass.MED_SCREEN_EXM_CODE )
                {
                    FinancialClass = new FinancialClass();
                }
            }

            return;
        }

        public void RemoveDefaultCoverage()
        {
            if ( Insurance != null )
            {
                Coverage primaryCoverage = Insurance.CoverageFor( CoverageOrder.PRIMARY_OID );
                if ( primaryCoverage != null && primaryCoverage.InsurancePlan != null &&
                    primaryCoverage.InsurancePlan.IsDefaultPlan() )
                {
                    Insurance.RemoveCoverage( primaryCoverage );
                    FinancialClass = new FinancialClass();
                }
            }
        }

        public void SetDefaultInsurancePlan()
        {
            Coverage existingPrimaryCoverage = null;
            if ( Insurance != null && Insurance.Coverages.Count > 0 )
            {
                existingPrimaryCoverage = Insurance.CoverageFor( CoverageOrder.PRIMARY_OID );
            }
            if ( existingPrimaryCoverage == null || existingPrimaryCoverage.InsurancePlan.HasEmptyPlanID() )
            {
                AddDefaultCoverage();
            }
        }

        public void ClearHospitalClinic()
        {
            HospitalClinic = null;
        }

        public void ClearResearchStudies()
        {
            i_ClinicalResearchStudies.Clear();
        }

        public void ClearPersistedFusNotes()
        {
            if ( i_PersistedFusNotes != null )
            {
                i_PersistedFusNotes.Clear();
            }
        }

        public void ClearFusNotes()
        {
            if ( i_FusNotes != null )
            {
                i_FusNotes.Clear();
            }

            if ( i_PersistedFusNotes != null )
            {
                i_PersistedFusNotes.Clear();
            }
        }

        public bool IsValidForCancel()
        {
            if ( ( KindOfVisit.Code.Equals( VisitType.PREREG_PATIENT ) ) && IsCanceled == false )
            {
                return true;
            }

            return false;
        }

        public bool IsSetInsuranceVerifiedToDefault()
        {
            if ( Activity != null )
            {
                if ( Activity.GetType() != typeof( TransferBedSwapActivity ) )
                {
                    if ( ( old_HospitalService != null ) && ( old_VisitType != null ) &&
                         ( old_HospitalService.Code.Length > 0 ) && ( old_VisitType.Code.Length > 0 ) )
                    {
                        return !( ( old_HospitalService.Code == HospitalService.Code ) &&
                                  ( old_VisitType.Code == KindOfVisit.Code ) );
                    }

                    return false;
                }

                if ( ( TransferredFromHospitalService != null ) &&
                     ( old_VisitType != null ) )
                {
                    return !( ( TransferredFromHospitalService.Code == HospitalService.Code ) &&
                              ( old_VisitType.Code == KindOfVisit.Code ) );
                }
            }
            return true;
        }

        public bool IsReasonForAccommodationRequiredForSelectedActivity()
        {
            if ( Activity is RegistrationActivity
                 || Activity is ActivatePreRegistrationActivity
                 || Activity is TransferOutToInActivity
                 || Activity is TransferActivity
                 || Activity is MaintenanceActivity )
            {
                return true;
            }

            return false;
        }
        public bool IsDiagnosticRegistrationMaintenanceAccount()
        {
            return (KindOfVisit.Code != VisitType.PreRegistration.Code
                && IsShortRegistered == true
                && Activity is ShortMaintenanceActivity);
        }

        public bool IsDiagnosticPreRegistrationMaintenanceAccount()
        {
            return (KindOfVisit != null && KindOfVisit.Code == VisitType.PreRegistration.Code
                && IsShortRegistered == true
                && Activity is ShortMaintenanceActivity );
        }

        /// <summary>
        /// Determines whether [is preop date enabled].
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if [is preop date enabled]; otherwise, <c>false</c>.
        /// </returns>
        public bool ShouldWeEnablePreopDate()
        {
            if ( Activity != null )
            {
                return AccountCreatedDate >= GetPreopReleaseDate();
            }

            return false;
        }

        public virtual bool CanAccept( SpanCode spanCode )
        {
            return Activity.CanAccept( spanCode, KindOfVisit );
        }

        public bool ShouldPCPCarryForward()
        {
            return ((Facility.GetCurrentDateTime() - Patient.MostRecentAccountCreationDate).Days < ThresholdForPullingPcpForwardInDays);
        }
        public void OverLayEMPIData()
        {
            var ePatient = Activity.EmpiPatient;
            if (ePatient != null && ePatient.EmpiPatientFound)
            {
                Patient.FirstName = ePatient.Patient.FirstName;
                Patient.LastName = ePatient.Patient.LastName;
                Patient.MiddleInitial = ePatient.Patient.MiddleInitial;
                foreach (Name alias in ePatient.Patient.Aliases)
                {
                    Patient.AddAlias(alias);
                }
                Patient.MaidenName = ePatient.Patient.MaidenName;
                Patient.Sex = ePatient.Patient.Sex;
                Patient.DateOfBirth = ePatient.Patient.DateOfBirth;
                Patient.SocialSecurityNumber = ePatient.Patient.SocialSecurityNumber;
                Patient.Religion = ePatient.Patient.Religion;
                Patient.MaritalStatus = ePatient.Patient.MaritalStatus;
                Patient.Race = ePatient.Patient.Race;
                Patient.Ethnicity = ePatient.Patient.Ethnicity;
                //Patient.Ethnicity2 = ePatient.Patient.Ethnicity2;
                Patient.Descent = ePatient.Patient.Descent;
                //Patient.Descent2 = ePatient.Patient.Descent2;
                ///Commented MedicalGroupIPA below as per client pulling from the PBAR database and hoping the patient was previously at the same facility
                //Patient.MedicalGroupIPA = ePatient.Patient.MedicalGroupIPA;
                // MedicalGroupIPA = ePatient.Patient.MedicalGroupIPA;
                Patient.DateOfBirth = ePatient.Patient.DateOfBirth;
                var ePatientPhysicalContactPoint =
                    ePatient.Patient.ContactPointWith(TypeOfContactPoint.NewPhysicalContactPointType());
                var patientPhysicalContactPoint =
                    Patient.ContactPointWith(TypeOfContactPoint.NewPhysicalContactPointType());
                if (ePatientPhysicalContactPoint != null &&
                    patientPhysicalContactPoint != null)
                {
                    SetPBARCountryAndCountyToEMPIAddress(ePatientPhysicalContactPoint.Address, patientPhysicalContactPoint.Address);
                    patientPhysicalContactPoint.PhoneNumber =
                        ePatientPhysicalContactPoint.PhoneNumber;
                    patientPhysicalContactPoint.Address = ePatientPhysicalContactPoint.Address;
                }
                var ePatientMailingContactPoint =
                    ePatient.Patient.ContactPointWith(TypeOfContactPoint.NewMailingContactPointType());

                var patientMailingContactPoint =
                    Patient.ContactPointWith(TypeOfContactPoint.NewMailingContactPointType());
                if (ePatientMailingContactPoint != null &&
                    patientMailingContactPoint != null)
                {
                    SetPBARCountryAndCountyToEMPIAddress(ePatientMailingContactPoint.Address, patientMailingContactPoint.Address);
                    patientMailingContactPoint.PhoneNumber = ePatientMailingContactPoint.PhoneNumber;
                    patientMailingContactPoint.Address = ePatientMailingContactPoint.Address;
                }

                var ePatientMobileContactPoint =
                    ePatient.Patient.ContactPointWith(TypeOfContactPoint.NewMobileContactPointType());
                var patientMobileContactPoint =
                    Patient.ContactPointWith(TypeOfContactPoint.NewMobileContactPointType());
                if (ePatientMobileContactPoint != null &&
                    patientMobileContactPoint != null)
                {
                    patientMobileContactPoint.PhoneNumber = ePatientMobileContactPoint.PhoneNumber;
                }

                var ePatientEmployerContactPoint = ePatient.Patient.Employment.Employer.PartyContactPoint;
                ContactPoint patientEmployerContactPoint;
                if (Patient.Employment != null && Patient.Employment.Employer != null &&
                    Patient.Employment.Employer.PartyContactPoint != null)
                {
                    patientEmployerContactPoint = Patient.Employment.Employer.PartyContactPoint;
                }
                else
                {
                    patientEmployerContactPoint = new ContactPoint(TypeOfContactPoint.NewEmployerContactPointType());
                    Patient.Employment = new Employment
                    {
                        Employer =
                            new Employer
                            {
                                PartyContactPoint = patientEmployerContactPoint
                            }
                    };
                }
                
                if (ePatientEmployerContactPoint != null)
                {
                    SetPBARCountryAndCountyToEMPIAddress(ePatientEmployerContactPoint.Address, patientEmployerContactPoint.Address);
                    patientEmployerContactPoint.Address = ePatientEmployerContactPoint.Address;
                    Patient.Employment.Employer.Name = ePatient.Patient.Employment.Employer.Name;
                    if(ePatient.Patient.Employment.Status !=null && !string.IsNullOrEmpty(ePatient.Patient.Employment.Status.Code))
                        Patient.Employment.Status = ePatient.Patient.Employment.Status;
                }
                
                Patient.DriversLicense = ePatient.Patient.DriversLicense;
                EmergencyContact1.Name = ePatient.EmergencyContact1.Name;

                if (ePatient.EmergencyContact1.Relationships.Count > 0)
                {
                    if (EmergencyContact1.Relationships != null)
                    {
                        foreach (Relationship rel in EmergencyContact1.Relationships)
                        {
                            EmergencyContact1.RemoveRelationship(rel);
                        }
                        foreach (Relationship rel in ePatient.EmergencyContact1.Relationships)
                        {
                            EmergencyContact1.AddRelationship(rel);
                        }
                    }
                }
               
                if (EmergencyContact1 != null &&
                    EmergencyContact1.ContactPointWith(TypeOfContactPoint.NewPhysicalContactPointType()) != null)
                {
                    EmergencyContact1.ContactPointWith(TypeOfContactPoint.NewPhysicalContactPointType()).PhoneNumber =
                        ePatient.EmergencyContact1.ContactPointWith(TypeOfContactPoint.NewPhysicalContactPointType()).
                            PhoneNumber;
                }
                Guarantor.FirstName = ePatient.Guarantor.FirstName;
                Guarantor.LastName = ePatient.Guarantor.LastName;
                Guarantor.Name.MiddleInitial = ePatient.Guarantor.Name.MiddleInitial;
                foreach (Relationship rel in Guarantor.Relationships)
                {
                    Guarantor.RemoveRelationship(rel);
                }
                foreach (Relationship rel in ePatient.Guarantor.Relationships)
                {
                    Guarantor.AddRelationship(rel);
                }

                Guarantor.ContactPointWith(TypeOfContactPoint.NewMailingContactPointType()).PhoneNumber =
                    ePatient.Guarantor.ContactPointWith(TypeOfContactPoint.NewMailingContactPointType()).
                        PhoneNumber;
                Guarantor.ContactPointWith(TypeOfContactPoint.NewMobileContactPointType()).PhoneNumber =
                    ePatient.Guarantor.ContactPointWith(TypeOfContactPoint.NewMobileContactPointType()).PhoneNumber;
                if (Activity.AssociatedActivityType != typeof(ActivatePreRegistrationActivity))
                { 
                    BuildInsuranceDetails(ePatient);
                }
            }
        }
        private void SetPBARCountryAndCountyToEMPIAddress(Address empiAddress, Address pbarAddress)
        {
            if (empiAddress != null && pbarAddress != null)

            {
                empiAddress.Country = (empiAddress.Country == null) ?
                                        pbarAddress.Country :
                                        (string.IsNullOrEmpty(empiAddress.Country.Description) ? pbarAddress.Country : empiAddress.Country);

                empiAddress.County = (empiAddress.County == null) ?
                                        pbarAddress.County :
                                        (string.IsNullOrEmpty(empiAddress.County.Description) ? pbarAddress.County : empiAddress.County);

            }
        }

        private void BuildInsuranceDetails(EMPIPatient ePatient)
        {
            if (!String.IsNullOrEmpty(ePatient.EMPIPrimaryInvalidPlanID))
            {
                EMPIPrimaryInvalidPlanID = ePatient.EMPIPrimaryInvalidPlanID;
            }
            if (!String.IsNullOrEmpty(ePatient.EMPISecondaryInvalidPlanID))
            {
                EMPISecondaryInvalidPlanID = ePatient.EMPISecondaryInvalidPlanID;
            }

            if (ePatient.Insurance != null)
            {
                BuildCoverages(ePatient);
            }
        }

        private void BuildCoverages(EMPIPatient ePatient)
        {
            //Build Primary coverage
            Coverage ePrimaryCoverage = null;
            if (ePatient.Insurance.CoverageFor(CoverageOrder.NewPrimaryCoverageOrder()) != null) 
            {
                ePrimaryCoverage = ePatient.Insurance.CoverageFor(CoverageOrder.NewPrimaryCoverageOrder());
            }

            Coverage currentPrimaryCoverage = null;
            if (Insurance.CoverageFor(CoverageOrder.NewPrimaryCoverageOrder()) != null) 
            {
                currentPrimaryCoverage = Insurance.CoverageFor(CoverageOrder.NewPrimaryCoverageOrder());
            }
            if (ePrimaryCoverage != null)
            {
                copyCoverage(ePrimaryCoverage, currentPrimaryCoverage);
            }

            //Build Secondary Coverage
            Coverage eSecondaryCoverage = null;

            if (ePatient.Insurance.CoverageFor(CoverageOrder.NewSecondaryCoverageOrder()) != null) 
            {
                eSecondaryCoverage = ePatient.Insurance.CoverageFor(CoverageOrder.NewSecondaryCoverageOrder());
            }

            Coverage currentSecondaryCoverage = null;

            if (Insurance.CoverageFor(CoverageOrder.NewSecondaryCoverageOrder()) != null) 
            {
                currentSecondaryCoverage = Insurance.CoverageFor(CoverageOrder.NewSecondaryCoverageOrder());
            }
            if (eSecondaryCoverage != null)
            {
                copyCoverage(eSecondaryCoverage, currentSecondaryCoverage);
            }
        }

        private void copyCoverage (Coverage eCoverage, Coverage currentCoverage )
        {
               if (eCoverage != null)
            {
                 var currentInsured = new Insured();
                if (currentCoverage != null && currentCoverage.Insured != null)
                {
                    currentInsured = currentCoverage.Insured;
                }
                var empiInsured = eCoverage.Insured;
                if (currentCoverage != null)
                {
                    var bilCoName = currentCoverage.BillingInformation.BillingCOName;
                    var bilName = currentCoverage.BillingInformation.BillingName;
                    currentCoverage = eCoverage;
                    foreach (Relationship rel in currentInsured.Relationships)
                    {
                        currentInsured.RemoveRelationship(rel);
                    }
                    foreach (Relationship rel in empiInsured.Relationships)
                    {
                        currentInsured.AddRelationship(rel);
                    }
                    currentCoverage.Insured = currentInsured;
                    currentCoverage.BillingInformation.BillingCOName = bilCoName;
                    currentCoverage.BillingInformation.BillingName = bilName;
                    Insurance.AddCoverage(currentCoverage);
                }
                else
                {
                    Insurance.AddCoverage(eCoverage);
                }
            }
        }

        public void SetUCCVisitType()
        {
            if (KindOfVisit != null && KindOfVisit.Code == VisitType.OUTPATIENT &&
                FinancialClass != null && FinancialClass.IsMedScreenExam())
            {
                KindOfVisit = VisitType.UCCOutpatient;
            }
        }

        public void ResetCOBReceived()
        {
           COBReceived = new YesNoFlag();
        }

        public void ResetIMFMReceived()
        {
            IMFMReceived = new YesNoFlag();
        }

        public bool HasValidInsurance()
        {
            if(Insurance.Coverages.Count != 0 )
            {
                var priorCoverage = Insurance.CoverageFor(CoverageOrder.NewPrimaryCoverageOrder());
                if (priorCoverage != null)
                {
                    var priorPlanID = priorCoverage.InsurancePlan.PlanID;
                    if (priorPlanID != PRE_MSE_INSURANCE_PLAN_ID && priorPlanID != PRE_MSE_INSURANCE_BACKUP_PLAN_ID)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void ResetPatientMRN()
        {
            if (this.Activity.IsAdmitNewbornActivity())
            {
                Patient.MedicalRecordNumber = 0;
            }
        }
        #endregion

        #region Properties

        public bool HideEmailReason
        {
            get
            {
                return Activity.IsPreMSEActivities() ||
                       Activity.IsUCCPreMSEActivity() ||
                       IsDiagnosticPreregistrationAccount ||
                       IsDiagnosticPreRegistrationMaintenanceAccount() ||
                       Activity.IsPreAdmitNewbornActivity() ||
                       Activity.IsEditPreAdmitNewbornActivity() ||
                       Activity.IsTransferOutToInActivity() ||
                       Activity.IsTransferERToOutpatientActivity();

            }
        }
        public bool HideEmailAddress
        {
            get
            {
                return Activity.IsPreMSEActivities() ||
                       Activity.IsUCCPreMSEActivity();

            }
        }
        public bool IsActivityValidToHandleCOSSigned
        {
            get
            {
                return !( Activity.IsPreRegistrationActivity() ||
                       Activity.IsPreAdmitNewbornActivity() ||
                       Activity.IsEditPreAdmitNewbornActivity() ||
                       Activity.IsDiagnosticPreRegistrationActivity() ||
                       IsDiagnosticPreRegistrationMaintenanceAccount() ||
                       IsPreRegisteredMaintenanceAccount ) ;
            }
        }

        public bool IsTransferActivityValidForPatientPortal
        {
            get { return Activity.IsTransferERToOutpatientActivity() || 
                Activity.IsTransferOutToInActivity(); }
        }

        public IValueLoader ActionsLoader
        {
            set { i_ActionHolder = new ValueHolder( value ); }
        }

        internal static DateTime CpasFeatureActivationDate { private get; set; }

        public long PreMSECopiedAccountNumber
        {
            get { return i_PreMSECopiedAccountNumber; }
            set { i_PreMSECopiedAccountNumber = value; }
        }

        /// <summary>
        /// List of OccurrenceCodes Populated by AccountPBARBroker, DB2 HPADLPP has 
        /// 8 occurrence code fields.
        /// </summary>
        public IList OccurrenceCodes
        {
            get
            {
                return i_OccurrenceCodes;
            }
        }

        /// <summary>
        /// List of Research Studies with consent details populated by 
        /// AccountPBARBroker, DB2 HPADLPP has 10 Research Study fields.
        /// </summary>
        public IEnumerable<ConsentedResearchStudy> ClinicalResearchStudies
        {
            get { return i_ClinicalResearchStudies; }
        }

        public bool IsRetired
        {
            get
            {
                bool isRetired = false;

                foreach ( OccurrenceCode oc in OccurrenceCodes )
                {
                    if ( oc.Code == OccurrenceCode.OCCURRENCECODE_RETIREDATE )
                    {
                        isRetired = true;
                        break;
                    }
                }

                return isRetired;
            }
        }

        public DateTime RetirementDate
        {
            get
            {
                var retireDate = new DateTime();
                foreach ( OccurrenceCode oc in OccurrenceCodes )
                {
                    if ( oc.Code == OccurrenceCode.OCCURRENCECODE_RETIREDATE )
                        retireDate = oc.OccurrenceDate;
                }
                return retireDate;
            }
        }

        public IList FusNotes
        {
            get { return ( (IList)i_FusNotes.Clone() ); }
        }

        public ArrayList PersistedFusNotes
        {
            get
            {
                if ( i_PersistedFusNotes == null )
                {
                    i_PersistedFusNotes = new ArrayList();
                }

                return i_PersistedFusNotes;
            }
            set
            {
                if ( i_PersistedFusNotes == null )
                {
                    i_PersistedFusNotes = new ArrayList();
                }

                i_PersistedFusNotes = value;
            }
        }

        public ArrayList AllFusNotes
        {
            get
            {
                var allFusNotes = new ArrayList();

                if ( i_PersistedFusNotes != null
                    && i_PersistedFusNotes.Count > 0 )
                {
                    allFusNotes = (ArrayList)i_PersistedFusNotes.Clone();
                }

                if ( i_FusNotes != null
                    && i_FusNotes.Count > 0 )
                {
                    allFusNotes.AddRange( (ArrayList)i_FusNotes.Clone() );
                }

                allFusNotes.Sort();
                return allFusNotes;
            }
        }

        public DateTime AdmitDateUnaltered
        {
            get { return i_AdmitDateUnaltered; }
            set { i_AdmitDateUnaltered = value; }
        }

        public DateTime PreopDate
        {
            get { return i_PreopDate; }
            set { i_PreopDate = value; }
        }

        public DateTime TransferDate
        {
            get { return i_TransferDate; }
            set { i_TransferDate = value; }
        }

        public DateTime LastChargeDate
        {
            get { return i_LastChargeDate; }
            set { i_LastChargeDate = value; }
        }

        public HospitalService TransferredFromHospitalService
        {
            get { return i_TransferredFromHospitalService; }
            set { i_TransferredFromHospitalService = value; }
        }

        public Guarantor Guarantor
        {
            get
            {
                if ( i_Guarantor == null )
                {
                    i_Guarantor = new Guarantor();
                }
                return i_Guarantor;
            }
            set { i_Guarantor = value; }
        }

        public Insured PrimaryInsured
        {
            get
            {
                Coverage aCoverage = Insurance.CoverageFor( CoverageOrder.PRIMARY_OID );
                if ( aCoverage != null )
                {
                    return aCoverage.Insured;
                }
                return null;
            }
        }

        public Insured SecondaryInsured
        {
            get
            {
                Coverage aCoverage = Insurance.CoverageFor( CoverageOrder.SECONDARY_OID );
                if ( aCoverage != null )
                {
                    return aCoverage.Insured;
                }
                return null;
            }
        }

        public Insurance Insurance
        {
            get { return i_Insurance ?? new Insurance(); }
            set { i_Insurance = value ?? new Insurance(); }
        }

        /// <summary>
        /// Part of Demographics Data for this Account. 
        /// Possible Values are 'Yes','No','Unknown'
        /// </summary>
        public YesNoFlag ClergyVisit
        {
            get { return i_ClergyVisit; }
            set { i_ClergyVisit = value; }
        }

        /// <summary>
        /// First EmergencyContact Data .Contains ContactPoint, Name, RelationShipType.
        /// </summary>
        public EmergencyContact EmergencyContact1
        {
            get { return i_EmergencyContact1; }
            set { i_EmergencyContact1 = value; }
        }

        /// <summary>
        /// Second EmergencyContact Data .Contains ContactPoint, Name, RelationShipType.
        /// </summary>
        public EmergencyContact EmergencyContact2
        {
            get { return i_EmergencyContact2; }
            set { i_EmergencyContact2 = value; }
        }

        /// <summary>
        /// Contains ChiefComplaint and Details of the Condition for this visit
        /// </summary>
        public Diagnosis Diagnosis
        {
            get { return i_Diagnosis; }
            set { i_Diagnosis = value; }
        }

        public decimal RequestedPayment
        {
            get { return i_RequestedPayment; }
            set { i_RequestedPayment = value; }
        }

        public decimal PreviousTotalCurrentAmtDue
        {
            get { return i_PreviousTotalCurrentAmtDue; }
            set { i_PreviousTotalCurrentAmtDue = value; }
        }

        public decimal TotalCurrentAmtDue
        {
            get { return i_TotalCurrentAmtDue; }
            set
            {
                SetAndTrack( ref i_TotalCurrentAmtDue, value, MethodBase.GetCurrentMethod() );
                if (value == 0)
                {
                    DayOfMonthPaymentDue = String.Empty;
                }
            }
        }

        public HospitalClinic HospitalClinic
        {
            get
            {
                if ( Clinics != null
                    && Clinics.Count > 0 )
                {
                    return (HospitalClinic)Clinics[0];
                }

                return null;
            }
            set
            {
                Clinics[0] = value; // HospitalClinic;
            }
        }

        public ArrayList Clinics
        {
            get
            {
                if ( i_Clinics == null || i_Clinics.Count == 0 )
                {
                    i_Clinics = new ArrayList( MAX_NUMBER_OF_CLINICS );
                    i_Clinics.Insert( 0, null );
                    i_Clinics.Insert( 1, null );
                    i_Clinics.Insert( 2, null );
                    i_Clinics.Insert( 3, null );
                    i_Clinics.Insert( 4, null );
                }
                return i_Clinics;
            }
        }

        public int NumberOfMonthlyPayments
        {
            get { return i_NumberOfMonthlyPayments; }
            set { i_NumberOfMonthlyPayments = value; }
        }

        public int OriginalNumberOfMonthlyPayments
        {
            get { return i_OriginalNumberOfMonthlyPayments; }
            set { i_OriginalNumberOfMonthlyPayments = value; }
        }

        public Payment Payment
        {
            get { return i_Payment; }
            set { i_Payment = value; }
        }

        public DischargeStatus DischargeStatus
        {
            get { return i_DischargeStatus; }
            set { i_DischargeStatus = value; }
        }

        public string PendingDischarge
        {
            get { return i_PendingDischarge; }
            set { i_PendingDischarge = value; }
        }

        public DischargeDisposition DischargeDisposition
        {
            get { return i_DischargeDisposition; }
            set { i_DischargeDisposition = value; }
        }

        public bool AbstractExists
        {
            get { return i_AbstractExists; }
            set { i_AbstractExists = value; }
        }

        public Location Location
        {
            get { return i_Location; }
            set { i_Location = value; }
        }

        public Location PreDischargeLocation
        {
            get { return i_PreDischargeLocation; }
            set { i_PreDischargeLocation = value; }
        }

        public decimal TotalPaid
        {
            get { return i_TotalPaid; }
            set { i_TotalPaid = value; }
        }

        public bool BillHasDropped
        {
            get { return i_BillHasDropped; }
            set { i_BillHasDropped = value; }
        }

        public Location LocationFrom
        {
            get { return i_LocationFrom; }
            set { i_LocationFrom = value; }
        }
        public Accomodation AccomodationFrom
        {
            get { return i_AccomodationFrom; }
            set { i_AccomodationFrom = value; }
        }

      
        public Location LocationTo
        {
            get { return i_LocationTo; }
            set { i_LocationTo = value; }
        }

        public ModeOfArrival ModeOfArrival
        {
            get { return i_ModeOfArrival; }
            set { i_ModeOfArrival = value; }
        }

        public AdmitSource AdmitSource
        {
            get { return i_AdmitSource; }
            set { i_AdmitSource = value; }
        }

        public decimal TotalCharges
        {
            get { return i_TotalCharges; }
            set { i_TotalCharges = value; }
        }

        public decimal OriginalMonthlyPayment
        {
            get { return i_OriginalMonthlyPayment; }
            set { i_OriginalMonthlyPayment = value; }
        }

        public decimal MonthlyPayment
        {
            get
            {
                if ( NumberOfMonthlyPayments == 0 )
                {
                    return i_MonthlyPayment;
                }

                if ( BalanceDue > 0m )
                {
                    decimal amount = BalanceDue / NumberOfMonthlyPayments;
                    string paymentAmount = String.Format( "{0, 10:f2}", amount );
                    i_MonthlyPayment = Decimal.Parse( paymentAmount );
                }
                return i_MonthlyPayment;
            }
            set
            {
                // UC170 requirement 12b #4
                i_MonthlyPayment = value;
            }
        }

        public ConfidentialCode ConfidentialityCode
        {
            get { return i_ConfidentialityCode; }
            set { i_ConfidentialityCode = value; }
        }

        public bool HasInValidPrimaryPlanID
        {
            get { return !String.IsNullOrEmpty(EMPIPrimaryInvalidPlanID); }
        }

        public bool HasInValidSecondaryPlanID
        {
            get { return !String.IsNullOrEmpty(EMPISecondaryInvalidPlanID); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [opt out name].
        /// </summary>
        /// <value><c>true</c> if [opt out name]; otherwise, <c>false</c>.</value>
        public bool OptOutName
        {
            get { return i_OptOutName; }
            set { SetAndTrack( ref i_OptOutName, value, MethodBase.GetCurrentMethod() ); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [opt out location].
        /// </summary>
        /// <value><c>true</c> if [opt out location]; otherwise, <c>false</c>.</value>
        public bool OptOutLocation
        {
            get { return i_OptOutLocation; }
            set { SetAndTrack( ref i_OptOutLocation, value, MethodBase.GetCurrentMethod() ); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [opt out health information].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [opt out health information]; otherwise, <c>false</c>.
        /// </value>
        public bool OptOutHealthInformation
        {
            get { return i_OptOutHealthInformation; }
            set { SetAndTrack( ref i_OptOutHealthInformation, value, MethodBase.GetCurrentMethod() ); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [opt out religion].
        /// </summary>
        /// <value><c>true</c> if [opt out religion]; otherwise, <c>false</c>.</value>
        public bool OptOutReligion
        {
            get { return i_OptOutReligion; }
            set { SetAndTrack( ref i_OptOutReligion, value, MethodBase.GetCurrentMethod() ); } //set
        }

        public ConditionOfService COSSigned
        {
            get { return i_COSSigned; }
            set
            {
                Debug.Assert( value != null );
                SetAndTrack( ref i_COSSigned, value, MethodBase.GetCurrentMethod() );
            }
        }

        public FacilityDeterminedFlag FacilityDeterminedFlag
        {
            get { return i_FacilityDeterminedFlag; }
            set { i_FacilityDeterminedFlag = value; }
        }

        public YesNoFlag Smoker
        {
            get { return i_Smoker; }
            set { i_Smoker = value; }
        }
        public YesNoFlag RightToRestrict
        {
            get { return i_RightToRestrict; }
            set
            {
                Debug.Assert(value != null);
                SetAndTrack(ref i_RightToRestrict, value, MethodBase.GetCurrentMethod());
            }
        }
        public YesNoFlag ShareDataWithPublicHieFlag
        {
            get { return iShareDataWithPublicHieFlag; }
            set { iShareDataWithPublicHieFlag = value; }
        }
        public YesNoFlag ShareDataWithPCPFlag
        {
            get { return  iShareDataWithPcpFlag; }
            set { iShareDataWithPcpFlag = value; }
        }
        public YesNoFlag Bloodless
        {
            get { return i_Bloodless; }
            set { i_Bloodless = value; }
        }

        public YesNoFlag Pregnant
        {
            get { return i_Pregnant; }
            set { i_Pregnant = value; }
        }

        public string ResistantOrganism
        {
            get;
            set;
        }

        public string ClinicalComments
        {
            get { return i_ClinicalComments; }
            set { i_ClinicalComments = value; }
        }

        public string EmbosserCard
        {
            get { return i_EmbosserCard; }
            set { SetAndTrack(ref i_EmbosserCard, value, MethodBase.GetCurrentMethod()); }
        }
        public ClinicServiceCategory ServiceCategory { get; set; }

        public MedicareSecondaryPayor MedicareSecondaryPayor
        {
            get { return i_MedicareSecondaryPayor; }
            set { i_MedicareSecondaryPayor = value; }
        }

        // TODO :
        //[Obsolete( "Do not use arraylist. Change property to individual 
        // OccurrenceSpanCode1 and OccurrenceSpanCode2 properties to make it 
        // more strongly-typed. Refactor solution to use the new properties" )]
        public ArrayList OccurrenceSpans
        {
            get { return i_OccurrenceSpans; }
        }

        public ReferralSource ReferralSource
        {
            get { return i_ReferralSource; }
            set { i_ReferralSource = value; }
        }

        public ScheduleCode ScheduleCode
        {
            get { return i_ScheduleCode; }
            set { i_ScheduleCode = value; }
        }

        public long LastMaintenanceDate
        {
            get { return i_LastMaintenanceDate; }
            set { i_LastMaintenanceDate = value; }
        }

        public long LastMaintenanceLogNumber
        {
            get { return i_LastMaintenanceLogNumber; }
            set { i_LastMaintenanceLogNumber = value; }
        }

        public long UpdateLogNumber
        {
            get { return i_UpdateLogNumber; }
            set { i_UpdateLogNumber = value; }
        }

        public YesNoFlag TenetCare
        {
            get { return i_TenetCare; }
            set { i_TenetCare = value; }
        }

        public YesNoFlag Reregister
        {
            get { return i_Reregister; }
            set { i_Reregister = value; }
        }

        public string DayOfMonthPaymentDue
        {
            set
            {
                Debug.Assert(value != null);
                SetAndTrack(ref i_DayOfMonthPaymentDue, value, MethodBase.GetCurrentMethod());
            }
            get
            {
                return i_DayOfMonthPaymentDue;
            }
        }

        /// <summary>
        /// This flag is used as an reminder so that the rep provides,
        /// a list of free county clinics or urgent cares
        /// that uninsured patients can utilize so that they do not 
        /// incur emergency room charges. It is something that is 
        /// provided to patients in various ER’s across the company. 
        /// </summary>
        public YesNoFlag ResourceListProvided
        {
            get { return i_ResourceListProvided; }
            set { i_ResourceListProvided = value; }
        }

        public ReAdmitCode ReAdmitCode
        {
            get { return i_ReAdmitCode; }
            set { i_ReAdmitCode = value; }
        }

        public ReferralFacility ReferralFacility
        {
            get { return i_ReferralFacility; }
            set { i_ReferralFacility = value; }
        }

        public ReferralType ReferralType
        {
            get { return i_ReferralType; }
            set { i_ReferralType = value; }
        }

        public DateTime ArrivalTime
        {
            get { return i_ArrivalTime; }
            set { i_ArrivalTime = value; }
        }

        public MedicalGroupIPA MedicalGroupIPA
        {
            get { return i_MedicalGroupIPA; }
            set { SetAndTrack<MedicalGroupIPA>(ref i_MedicalGroupIPA, value, MethodBase.GetCurrentMethod()); }
        }

        public Coverage DeletedSecondaryCoverage
        {
            get { return i_DeletedSecondaryCoverage; }
            set { i_DeletedSecondaryCoverage = value; }
        }

        public string ValueCode1
        {
            get { return i_ValueCode1; }
            set { i_ValueCode1 = value; }
        }

        public string ValueCode2
        {
            get { return i_ValueCode2; }
            set { i_ValueCode2 = value; }
        }

        public string ValueCode3
        {
            get { return i_ValueCode3; }
            set { i_ValueCode3 = value; }
        }

        public string ValueCode4
        {
            get { return i_ValueCode4; }
            set { i_ValueCode4 = value; }
        }

        public string ValueCode5
        {
            get { return i_ValueCode5; }
            set { i_ValueCode5 = value; }
        }

        public string ValueCode6
        {
            get { return i_ValueCode6; }
            set { i_ValueCode6 = value; }
        }

        public string ValueCode7
        {
            get { return i_ValueCode7; }
            set { i_ValueCode7 = value; }
        }

        public string ValueCode8
        {
            get { return i_ValueCode8; }
            set { i_ValueCode8 = value; }
        }

        public decimal ValueAmount1
        {
            get { return i_ValueAmount1; }
            set { i_ValueAmount1 = value; }
        }

        public decimal ValueAmount2
        {
            get { return i_ValueAmount2; }
            set { i_ValueAmount2 = value; }
        }

        public decimal ValueAmount3
        {
            get { return i_ValueAmount3; }
            set { i_ValueAmount3 = value; }
        }

        public decimal ValueAmount4
        {
            get { return i_ValueAmount4; }
            set { i_ValueAmount4 = value; }
        }

        public decimal ValueAmount5
        {
            get { return i_ValueAmount5; }
            set { i_ValueAmount5 = value; }
        }

        public decimal ValueAmount6
        {
            get { return i_ValueAmount6; }
            set { i_ValueAmount6 = value; }
        }

        public decimal ValueAmount7
        {
            get { return i_ValueAmount7; }
            set { i_ValueAmount7 = value; }
        }

        public decimal ValueAmount8
        {
            get { return i_ValueAmount8; }
            set { i_ValueAmount8 = value; }
        }

        public CodedDiagnoses CodedDiagnoses
        {
            get { return i_CodedDiagnoses; }
            set { i_CodedDiagnoses = value; }
        }

        public DateTime AccountCreatedDate
        {
            get { return i_AccountCreatedDate; }
            set { i_AccountCreatedDate = value; }
        }

        public RightCareRightPlace RightCareRightPlace
        {
            get { return i_RightCareRightPlace; }
        }

        public YesNoFlag LeftWithOutBeingSeen { get; set; }
        public YesNoFlag LeftWithoutFinancialClearance { get; set; }
        public string AlternateCareFacility { get; set; }

        public String IsolationCode { get; set; }

        public bool OnlinePreRegistered
        {
            get { return i_OnlinePreRegistered; }
            set { i_OnlinePreRegistered = value; }
        }

        public string AdmittingCategory { get; set; }

        public bool IsCanceledPreRegistration
        {
            get { return DerivedVisitType == PRE_CAN; }
        }

        public ArrayList ConsultingPhysicians
        {
            get
            {
                var result = new ArrayList();

                foreach ( PhysicianRelationship aRelationship in AllPhysicianRelationships )
                {
                    if ( aRelationship.PhysicianRole.Role().Equals( PhysicianRole.Consulting().Role() ) )
                    {
                        result.Add( aRelationship.Physician );
                    }
                }
                return result;
            }
        }

        public Physician AdmittingPhysician
        {
            get { return PhysicianWithRole( PhysicianRole.Admitting().Role() ); }
        }

        public Physician AttendingPhysician
        {
            get { return PhysicianWithRole( PhysicianRole.Attending().Role() ); }
        }

        public Physician ReferringPhysician
        {
            get { return PhysicianWithRole( PhysicianRole.Referring().Role() ); }
        }

        public Physician OperatingPhysician
        {
            get { return PhysicianWithRole( PhysicianRole.Operating().Role() ); }
        }

        public Physician PrimaryCarePhysician
        {
            get { return PhysicianWithRole( PhysicianRole.PrimaryCare().Role() ); }
        }

        private int ThresholdForPullingPcpForwardInDays
        {
            get { return Convert.ToInt32( ConfigurationManager.AppSettings["THRESHOLD_FOR_PULLING_PCP_FORWARD_IN_DAYS"] ); }
        }

        public long AccountNumber
        {
            get { return i_AccountNumber; }
            set { i_AccountNumber = value; }
        }

        /// <summary>
        /// List of ConditionCodes Populated by AccountPBARBroker, DB2 HPADLPP has
        /// 7 condition code fields.
        /// </summary>
        public IList ConditionCodes
        {
            get { return i_ConditionCodes; }
        }

        public DateTime AdmitDate
        {
            get { return i_AdmitDate; }
            set { i_AdmitDate = value; }
        }

        public DateTime DischargeDate
        {
            get { return i_DischargeDate; }
            set { i_DischargeDate = value; }
        }

        public Facility Facility
        {
            get { return Patient.Facility; }
            set { Patient.Facility = value; }
        }

        public FinancialClass FinancialClass
        {
            get { return i_FinancialClass; }
            set { SetAndTrack<FinancialClass>(ref i_FinancialClass, value, MethodBase.GetCurrentMethod()); }
        }
        public bool IsDOFRInitiated { get; set; }

        public HospitalService HospitalService
        {
            get { return i_HospitalService; }
            set
            {
                HospitalService oldValue = i_HospitalService;
                i_HospitalService = value;
                RaiseChangedEvent( "HospitalService", oldValue, value );
                if ( old_HospitalService == null )
                {
                    old_HospitalService = value;
                }
            }
        }

        public VisitType KindOfVisit
        {
            get { return i_KindOfVisit; }
            set
            {
                VisitType oldValue = i_KindOfVisit;
                i_KindOfVisit = value;
                RaiseChangedEvent( "KindOfVisit", oldValue, value );
                if ( old_VisitType == null )
                {
                    old_VisitType = value;
                }
            }
        }

        public string DerivedVisitType
        {
            get { return i_DerivedVisitType; }
            set { i_DerivedVisitType = value; }
        }

        public bool IsLocked
        {
            get { return AccountLock.IsLocked; }
            set
            {
                //Remove this after removing all IsLocked set usages
                i_IsLocked = value;
            }
        }

        public Patient Patient
        {
            get { return i_Patient; }
            set { i_Patient = value; }
        }

        public decimal BalanceDue
        {
            get { return i_BalanceDue; }
            set { i_BalanceDue = value; }
        }

        public string PrimaryPayor
        {
            get
            {
                if ( Insurance.CoverageFor( CoverageOrder.PRIMARY_OID ) != null &&
                    Insurance.CoverageFor( CoverageOrder.PRIMARY_OID ).InsurancePlan != null &&
                    Insurance.CoverageFor( CoverageOrder.PRIMARY_OID ).InsurancePlan.Payor != null )
                {
                    return Insurance.CoverageFor( CoverageOrder.PRIMARY_OID ).InsurancePlan.Payor.Name;
                }

                return String.Empty;
            }
        }

        public Clinic Clinic
        {
            get { return GetClinicFor(); }
        }

        public Activity Activity
        {
            get { return i_Activity; }
            set
            {
                RaiseChangedEvent( "Activity", i_Activity, value );
                i_Activity = value;
            }
        }

        public YesNoFlag ValuablesAreTaken
        {
            get { return i_ValuablesAreTaken; }
            set
            {
                Debug.Assert( value != null );
                SetAndTrack( ref i_ValuablesAreTaken, value, MethodBase.GetCurrentMethod() );
            }
        }

        public ICollection AllPhysicianRelationships
        {
            get { return i_AllPhysicianRelationships; }
            set { i_AllPhysicianRelationships = value; }
        }

        public Dictionary<int, string> CptCodes { get; set; }

        public bool IsCanceled
        {
            get { return i_IsCanceled; }
            set { i_IsCanceled = value; }
        }

        public bool IsNew
        {
            get { return i_IsNew; }
            set { i_IsNew = value; }
        }

        public bool IsShortRegistered
        {
            get { return i_IsShortRegistered; }
            set { i_IsShortRegistered = value; }
        }
        public bool IsQuickRegistered
        {
            get { return i_IsQuickRegistered; }
            set { i_IsQuickRegistered = value; }
        }
        public bool IsPAIWalkinRegistered
        {
            get { return i_IsPAIWalkinRegistered; }
            set { i_IsPAIWalkinRegistered = value; }
        }

               

        public bool IsNewBorn { get; set; }

        public AccountLock AccountLock
        {
            get { return i_AccountLock; }
            set { i_AccountLock = value; }
        }

         public bool IsLatestPostMSEAccount()
        {
            return (this.Activity.IsPostMSEActivity() && this.Patient.MostRecentAccountNumber.Equals(this.AccountNumber));
        }


        public YesNoFlag IsPatientInClinicalResearchStudy
        {
            get { return isPatientInClinicalResearchStudy; }

            set { isPatientInClinicalResearchStudy = value; }
        }

        public YesNoFlag PatientPortalOptIn
        {
            get { return i_PatientPortalOptIn; }

            set { i_PatientPortalOptIn = value; }
        }
        
        public YesNoFlag AuthorizeAdditionalPortalUsers
        {
            get { return i_AuthorizeAdditionalPortalUsers; }

            set { i_AuthorizeAdditionalPortalUsers = value; }
        }

        public Dictionary<int, AuthorizedAdditionalPortalUser> AuthorizedAdditionalPortalUsers
        {
            get { return i_AuthorizedAdditionalPortalUsers; }
            set { i_AuthorizedAdditionalPortalUsers = value; }
        }

        public CellPhoneConsent OldGuarantorCellPhoneConsent
        {
            get { return i_OldGuarantorCellPhoneConsent; }
            set { i_OldGuarantorCellPhoneConsent = value; }
        }

        public bool IsEDorUrgentCarePremseAccount
        {
            get { return IsEDPremseAccount || IsUrgentCarePreMse; }
        }

        public bool IsEDPremseAccount
        {
            get
            {
                return KindOfVisit != null &&
                    KindOfVisit.Code == VisitType.EMERGENCY_PATIENT &&
                    FinancialClass != null &&
                    FinancialClass.Code.Equals(FinancialClass.MED_SCREEN_EXM_CODE);
            }
        }

        public bool IsUrgentCarePreMse
        {
            get
            {
                return ( (KindOfVisit != null &&
                       KindOfVisit.Code == VisitType.OUTPATIENT &&
                       FinancialClass != null &&
                       FinancialClass.IsMedScreenExam() ) ||
                       (Activity != null && Activity.IsUCCPreMSEActivity() ) 
                       );
            }
        }

        public bool IsCOSSignedNo
        {
            get
            {
                return ( COSSigned.IsNotAvailable ||
                         COSSigned.IsUnable ||
                         COSSigned.IsRefused );
            }
        }

        public string EMPIPrimaryInvalidPlanID
        {
            get { return i_EmpiPrimaryInvalidPlanId; }
            set { i_EmpiPrimaryInvalidPlanId = value; }
        }

        public string EMPISecondaryInvalidPlanID
        {
            get { return i_EmpiSecondaryInvalidPlanId; }
            set { i_EmpiSecondaryInvalidPlanId = value; }
        }
        
        public YesNoFlag COBReceived
        {
            get { return i_COBReceived; }
            set { i_COBReceived = value; }
        }

        public YesNoFlag IMFMReceived
        {
            get { return i_IMFMReceived; }
            set { i_IMFMReceived = value; }
        }

        public bool HasFinancialClassChanged
        {
            get
            {
                if (!i_FinancialClassChanged)
                {
                    if (FinancialClass != null
                        && FinancialClass.Code != FinancialClass.MCAID_MGD_CONTR_CODE)
                    {
                        i_FinancialClassChanged = true;
                    }
                }
                return i_FinancialClassChanged;
            }
            set { i_FinancialClassChanged = value; }
        }

        #endregion

        #region Private Methods

        private Clinic GetClinicFor()
        {
            var clinic = new Clinic();

            foreach ( Clinic c in MedicalGroupIPA.Clinics )
            {
                clinic = c;
                break;
            }

            return clinic;
        }

        /// <summary>
        /// Removes a ConsentedResearchStudy from the list of studies the account is enrolled in.  
        /// </summary>
        /// <param name="researchStudy"></param>
        private void RemoveConsentedResearchStudy( ConsentedResearchStudy researchStudy )
        {
            if ( i_ClinicalResearchStudies.Contains( researchStudy ) )
            {
                i_ClinicalResearchStudies.Remove( researchStudy );
            }
        }

        private void AddDefaultCoverage()
        {
            var insuranceBroker = BrokerFactory.BrokerOfType<IInsuranceBroker>();
            var defaultInsurancePlan = insuranceBroker.GetDefaultInsurancePlan( Facility.Oid, AdmitDate );
            var defaultCoverage = Coverage.CoverageFor( defaultInsurancePlan, new Insured() );
            defaultCoverage.CoverageOrder = new CoverageOrder( CoverageOrder.PRIMARY_OID, CoverageOrder.PRIMARY_DESCRIPTION );
            Insurance.AddCoverage( defaultCoverage );
        }

        private Physician PhysicianWithRole( PhysicianRole aRole )
        {
            Physician result = null;

            foreach ( PhysicianRelationship aRelationship in AllPhysicianRelationships )
            {
                if ( aRelationship.PhysicianRole.Role().Equals( aRole ) )
                {
                    result = aRelationship.Physician;
                    if ( result != null )
                    {
                        break;
                    }
                }
            }

            return result;
        }

        private void AddConsultingPhysicianRelationship( PhysicianRelationship aRelationship )
        {
            if ( IsAtMaxConsultingPhysicians() )
            {
                RemovePhysicianRelationship( aRelationship );
            }

            ( (ArrayList)AllPhysicianRelationships ).Add( aRelationship );
        }

        private void AddNonConsultingPhysicianRelationship( PhysicianRelationship aRelationship )
        {
            if ( PhysicianWithRole( aRelationship.PhysicianRole.Role() ) != null )
            {
                RemovePhysicianRelationship( aRelationship );
            }

            ( (ArrayList)AllPhysicianRelationships ).Add( aRelationship );
        }

        private bool IsAtMaxConsultingPhysicians()
        {
            return ConsultingPhysicians.Count >= MAX_NUMBER_OF_CONSULTING_PHYSICIANS;
        }

        private bool IsConsultingPhysician( PhysicianRelationship aPhysicianRelationship )
        {
            return aPhysicianRelationship.PhysicianRole.Role().Equals( PhysicianRole.Consulting().Role() );
        }

        private void ContactPointWithContext( string Context, ContactPoint cp )
        {
            switch ( Context )
            {
                case Address.PatientMailing:
                    break;

                case Address.PatientPhysical:
                    cp.CellPhoneNumber = new PhoneNumber();
                    cp.EmailAddress = new EmailAddress();
                    break;

                case "Guarantor":
                    break;

                case "PrimaryInsured":
                    cp.EmailAddress = new EmailAddress();
                    break;

                case "SecondaryInsured":
                    cp.EmailAddress = new EmailAddress();
                    break;

                case "EmergencyContact1":
                    cp.CellPhoneNumber = new PhoneNumber();
                    cp.EmailAddress = new EmailAddress();
                    break;

                case "EmergencyContact2":
                    cp.CellPhoneNumber = new PhoneNumber();
                    cp.EmailAddress = new EmailAddress();
                    break;

                case "Billing":
                    cp.CellPhoneNumber = new PhoneNumber();
                    cp.EmailAddress = new EmailAddress();
                    break;

                default:
                    cp.PhoneNumber = new PhoneNumber();
                    cp.CellPhoneNumber = new PhoneNumber();
                    cp.EmailAddress = new EmailAddress();
                    break;
            }
        }

        private static DateTime GetPreopReleaseDate()
        {
            DateTime preopStartDate = CpasFeatureActivationDate;

            // The static CpasFeatureActivationDate allows us to create more stable tests
            if ( preopStartDate.Equals( DateTime.MinValue ) )
            {
                string strPreopStartDate = ConfigurationManager.AppSettings[PREOP_START_DATE];
                preopStartDate = DateTime.Parse( strPreopStartDate );
            }

            return preopStartDate;
        }

        #endregion

        #region Private Properties

        private ICollection RemainingActions
        {
            get
            {
                if ( i_RemainingActions == null
                    || i_RemainingActions.Count == 0 )
                {
                    i_RemainingActions = (Hashtable)i_ActionHolder.GetValue();
                }
                return i_RemainingActions;
            }
        }

        public bool BirthTimeIsEntered
        {   get; set; }

        public bool HasOutstandingBalance
        {
            get { return TotalCurrentAmtDue > 0 && (TotalCurrentAmtDue - TotalPaid) > 0; }
        }

        #endregion

        #region Construction and Finalization

        public Account()
        {
            CptCodes = new Dictionary<int, string>(MAX_NUMBER_OF_CPTCODES);
            AlternateCareFacility = String.Empty;
            ResistantOrganism = String.Empty;
        }

        public Account( long accountNumber )
            : base( accountNumber )
        {
            CptCodes = new Dictionary<int, string>(10);
            AccountNumber = accountNumber;
        }

        #endregion

        #region Data Elements

        private readonly ArrayList i_ConditionCodes = new ArrayList();
        private readonly ArrayList i_FusNotes = new ArrayList();
        private readonly ArrayList i_OccurrenceCodes = new ArrayList();
        private bool i_AbstractExists;
        private DateTime i_AccountCreatedDate = DateTime.Now;
        private AccountLock i_AccountLock = new AccountLock();
        private long i_AccountNumber;
        [NonSerialized]
        private ValueHolder i_ActionHolder;
        private Activity i_Activity;
        private DateTime i_AdmitDate;
        private DateTime i_AdmitDateUnaltered;
        private AdmitSource i_AdmitSource = new AdmitSource();
        private ICollection i_AllPhysicianRelationships = new ArrayList();
        private DateTime i_ArrivalTime;
        private decimal i_BalanceDue;
        private bool i_BillHasDropped;
        private YesNoFlag i_Bloodless;
        private YesNoFlag i_ClergyVisit = new YesNoFlag();
        private string i_ClinicalComments;
        private readonly List<ConsentedResearchStudy> i_ClinicalResearchStudies = new List<ConsentedResearchStudy>();
        private ArrayList i_Clinics = new ArrayList( MAX_NUMBER_OF_CLINICS );
        private CodedDiagnoses i_CodedDiagnoses = new CodedDiagnoses();
        private ConfidentialCode i_ConfidentialityCode = new ConfidentialCode();
        private ConditionOfService i_COSSigned = new ConditionOfService();
        private string i_DayOfMonthPaymentDue = String.Empty;
        private Coverage i_DeletedSecondaryCoverage;
        private string i_DerivedVisitType = String.Empty;
        private Diagnosis i_Diagnosis = new Diagnosis();
        private DateTime i_DischargeDate;
        private DischargeDisposition i_DischargeDisposition = new DischargeDisposition();
        private DischargeStatus i_DischargeStatus = new DischargeStatus();
        private string i_EmbosserCard;
        private EmergencyContact i_EmergencyContact1 = new EmergencyContact();
        private EmergencyContact i_EmergencyContact2 = new EmergencyContact();
        private FacilityDeterminedFlag i_FacilityDeterminedFlag = new FacilityDeterminedFlag();
        private FinancialClass i_FinancialClass = new FinancialClass();
        private Guarantor i_Guarantor = new Guarantor();
        private HospitalService i_HospitalService = new HospitalService();
        private Insurance i_Insurance = new Insurance();
        private bool i_IsCanceled;
        private bool i_IsLocked;
        private bool i_IsNew;
        private VisitType i_KindOfVisit = new VisitType();
        private DateTime i_LastChargeDate;
        private long i_LastMaintenanceDate;
        private long i_LastMaintenanceLogNumber;
        private YesNoFlag i_PatientPortalOptIn = new YesNoFlag();
        private YesNoFlag i_PatientOptInPatientPortal = new YesNoFlag();
        private YesNoFlag i_AuthorizeAdditionalPortalUsers = new YesNoFlag();
        private CellPhoneConsent i_OldGuarantorCellPhoneConsent = new CellPhoneConsent();
        //public long TransferFromHospitalNumber {get;set;}
        //public long TransferFromAccountNumber { get; set; }
        //public long TransferToHospitalNumber { get; set; }
        //public long TransferToAccountNumber { get; set; }
        // at the time of coding these values were not used by PatientAccess. However they
        // needed to be read and returned to pbar. Therefore they were stored as string and
        // not blown out into object. 
        private Location i_Location;
        private Location i_LocationFrom;
        private Location i_LocationTo;
        public Accomodation i_AccomodationFrom { get; set; }
        private MedicalGroupIPA i_MedicalGroupIPA = new MedicalGroupIPA();
        private MedicareSecondaryPayor i_MedicareSecondaryPayor = new MedicareSecondaryPayor();

        private ModeOfArrival i_ModeOfArrival = new ModeOfArrival();
        private decimal i_MonthlyPayment;
        private int i_NumberOfMonthlyPayments;
        private ArrayList i_OccurrenceSpans = new ArrayList();
        private bool i_OptOutHealthInformation;
        private bool i_OptOutLocation;
        private bool i_OptOutName;
        private bool i_OptOutReligion;
        private decimal i_OriginalMonthlyPayment;
        private int i_OriginalNumberOfMonthlyPayments;
        private Patient i_Patient = new Patient();
        private Payment i_Payment = new Payment();
        private string i_PendingDischarge = String.Empty;

        [NonSerialized]
        private ArrayList i_PersistedFusNotes = new ArrayList();
        private Location i_PreDischargeLocation;

        private YesNoFlag i_Pregnant = new YesNoFlag();
        private long i_PreMSECopiedAccountNumber;
        private DateTime i_PreopDate;
        private decimal i_PreviousTotalCurrentAmtDue;
        private ReAdmitCode i_ReAdmitCode = new ReAdmitCode();
        private ReferralFacility i_ReferralFacility = new ReferralFacility();
        private ReferralSource i_ReferralSource = new ReferralSource();
        private ReferralType i_ReferralType = new ReferralType();
        private Hashtable i_RemainingActions = new Hashtable();
        private decimal i_RequestedPayment;
        private YesNoFlag i_Reregister = new YesNoFlag();
        private YesNoFlag i_ResourceListProvided = new YesNoFlag();
        private RightCareRightPlace i_RightCareRightPlace = new RightCareRightPlace();
        private ScheduleCode i_ScheduleCode = new ScheduleCode();
        private YesNoFlag i_Smoker;
        private YesNoFlag i_RightToRestrict = new YesNoFlag();
        private YesNoFlag iShareDataWithPublicHieFlag = new YesNoFlag(YesNoFlag.CODE_NO);
        private YesNoFlag iShareDataWithPcpFlag = new YesNoFlag(YesNoFlag.CODE_NO);
        private YesNoFlag i_TenetCare = new YesNoFlag();
        private decimal i_TotalCharges;
        private decimal i_TotalCurrentAmtDue;
        private decimal i_TotalPaid;
        private DateTime i_TransferDate;
        private HospitalService i_TransferredFromHospitalService = new HospitalService();
        private long i_UpdateLogNumber;
        private YesNoFlag i_ValuablesAreTaken = new YesNoFlag();

        private bool i_OnlinePreRegistered;
        private bool i_IsShortRegistered;
        private bool i_IsQuickRegistered;
		private bool i_IsPAIWalkinRegistered;

        // PBAR Value Amounts
        private decimal i_ValueAmount1;
        private decimal i_ValueAmount2;
        private decimal i_ValueAmount3;
        private decimal i_ValueAmount4;
        private decimal i_ValueAmount5;
        private decimal i_ValueAmount6;
        private decimal i_ValueAmount7;
        private decimal i_ValueAmount8;
        private string i_ValueCode1 = String.Empty;
        private string i_ValueCode2 = String.Empty;
        private string i_ValueCode3 = String.Empty;
        private string i_ValueCode4 = String.Empty;
        private string i_ValueCode5 = String.Empty;
        private string i_ValueCode6 = String.Empty;
        private string i_ValueCode7 = String.Empty;
        private string i_ValueCode8 = String.Empty;
        private YesNoFlag isPatientInClinicalResearchStudy = YesNoFlag.Blank;
        public HospitalService old_HospitalService;
        private VisitType old_VisitType;
        private string i_EmpiSecondaryInvalidPlanId = String.Empty;
        private string i_EmpiPrimaryInvalidPlanId = String.Empty;
        private YesNoFlag i_COBReceived = new YesNoFlag();
        private YesNoFlag i_IMFMReceived = new YesNoFlag();
        private bool i_FinancialClassChanged;

        private Dictionary<int, AuthorizedAdditionalPortalUser> i_AuthorizedAdditionalPortalUsers =
            new Dictionary<int, AuthorizedAdditionalPortalUser>(MAX_NUMBER_OF_PORTALUSERS);
        #endregion
       
        #region Constants

        private const string FIRST_OF_THE_MONTH = "1";
        public const string INH_PUR = "INH-PUR";

        // these are pbar values. they are used in a variety of places to indicate 
        // a sub status of the account. There are constants for these because there
        // are places where these values are checked.

        public const string INP_CAN = "INP-CAN";

        public const string INP_DIS = "INP-DIS";
        public const string INP_INH = "INP-INH";

        private const int
            MAX_NUMBER_OF_CLINICS = 5;

        private const int
            MAX_NUMBER_OF_CONSULTING_PHYSICIANS = 5;
        
        public const int MAX_NUMBER_OF_CPTCODES = 10;
        public const int MAX_NUMBER_OF_PORTALUSERS = 4;

        public const string O_HSV = "O HSV";

        public const string OUT_FIN = "OUT-FIN";
        public const string OUT_PRE = "OUT-PRE";

        public const string OUT_PUR = "OUT-PUR";

        public const string OUT_REC = "OUT-REC";

        public const string PND_PUR = "PND_PUR";

        public const string PRE_CAN = "PRE-CAN";
        public const string PRE_CHG = "PRE-CHG";
        public const string PRE_NC = "PRE-N/C";
        public const string PRE_PUR = "PRE-PUR";

        private const string PREOP_START_DATE = "PREOP_START_DATE";

        private const string PRE_MSE_INSURANCE_PLAN_ID = "EDL81";
        private const string PRE_MSE_INSURANCE_BACKUP_PLAN_ID = "EDL99";

        #endregion
    }
}