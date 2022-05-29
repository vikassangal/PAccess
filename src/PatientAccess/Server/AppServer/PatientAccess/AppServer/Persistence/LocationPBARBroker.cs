using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Extensions.DB2Persistence;
using Extensions.PersistenceCommon;
using IBM.Data.DB2.iSeries;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Persistence.Utilities;
using log4net;

namespace PatientAccess.Persistence
{
	/// <summary>
	/// Summary description for LocationPBARBroker.
	/// </summary>
	[Serializable]
	public class LocationPBARBroker : AbstractPBARBroker, ILocationBroker
	{
        #region Events
        #endregion

        #region Methods
        
        public DuplicateLocationResult CheckForDuplicateBedAssignments( Facility facility,
                                                                        string lastName,
                                                                        string firstName,
                                                                        long accountNumber,
                                                                        long medicalRecordNumber,
                                                                        SocialSecurityNumber ssn,
                                                                        DateTime dob,
                                                                        string zip)
        {
            List<long> accounts = new List<long>();

            DuplicateLocationResult result = new DuplicateLocationResult();
            result.dupeStatus = DuplicateBeds.NoDupes;
            result.accounts = accounts;

            try
            {
                cmd = this.CommandFor( string.Concat( "CALL ", SP_CHECKFORDUPELOCATIONS,
                "(", INPUT_PARAM_FACID,
                ",", INPUT_PARAM_LAST_NAME,
                ",", INPUT_PARAM_FIRST_NAME,
                ",", INPUT_PARAM_ACCOUNT,
                ",", INPUT_PARAM_SSN,
                ",", INPUT_PARAM_DOB,
                ",", INPUT_PARAM_MRN,
                ",", INPUT_PARAM_ZIP,
                ")" ),
                CommandType.Text,
                facility );

                cmd.Parameters[INPUT_PARAM_FACID].Value = facility.Oid;
                cmd.Parameters[INPUT_PARAM_LAST_NAME].Value = lastName.Trim();
                cmd.Parameters[INPUT_PARAM_FIRST_NAME].Value = firstName.Trim();
                cmd.Parameters[INPUT_PARAM_ACCOUNT].Value = accountNumber;
                cmd.Parameters[INPUT_PARAM_SSN].Value = ssn.UnformattedSocialSecurityNumber.Trim();

                // Note, PBAR stores dates in a myriad for formats. This particular format is MMddYYYY converted to an integer.

                if( dob != DateTime.MinValue )
                {
                    cmd.Parameters[INPUT_PARAM_DOB].Value = Convert.ToInt64( dob.ToString( "MMddyyyy" ) );
                }

                cmd.Parameters[INPUT_PARAM_MRN].Value = medicalRecordNumber;
                cmd.Parameters[INPUT_PARAM_ZIP].Value = zip.Trim();

                reader = this.ExecuteReader( cmd );

                while( reader.Read() )
                {
                    int status = reader.GetInt32( COL_STATUS );
                    long acctNumber = reader.GetInt64( COL_ACCOUNTNUMBER );

                    result.dupeStatus = (DuplicateBeds)status;
                    accounts.Add( acctNumber );
                }

            }
            
            catch (Exception e)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom("LocationPBARBroker failed to initialize", e, c_log);
            }

            finally
            {
                base.Close( reader );
                base.Close( cmd );
            }

            return result;
        }


	    /// <summary>
	    /// Gets all the Nursing Stations for a given facility
	    /// </summary>
	    /// <param name="facility">contains details of a facility</param>
	    /// <param name="getCachedData">if true get data from the cache otherwise get it from the database</param>
	    /// <returns>collection of Nursing Stations</returns>
	    public IList<NursingStation> NursingStationsFor( Facility facility, bool getCachedData = true)
        {
            return getCachedData ? CachedNursingStationsFor( facility ) : ByPassCacheNursingStationsFor( facility );
        }
        
        private IList<NursingStation> CachedNursingStationsFor(Facility facility)
        {
            IList<NursingStation> nursingStations = new List<NursingStation>() ;

            LoadCacheDelegate loadNursingStationData = delegate
            {
                  nursingStations =  ByPassCacheNursingStationsFor(facility) ;
                  return nursingStations.ToList();
            };

            try
            {
                var cacheManager = new CacheManager();
                
                var cachedNursingStations = cacheManager.GetCollectionBy(CacheKeys.CACHE_KEY_FOR_NURSINGSTATIONS, facility.Code, loadNursingStationData);

                nursingStations = cachedNursingStations.Cast<NursingStation>().ToList();
            }

            catch (Exception e)
            {   
                throw BrokerExceptionFactory.BrokerExceptionFrom("locationPbarBroker(nursingStations) failed to initialize", e, c_log);
            }

            return nursingStations;
        }

	    private IList<NursingStation> ByPassCacheNursingStationsFor(Facility facility)
        {
            IList<NursingStation> nursingStations = new List<NursingStation>();

            try
            {
                cmd = CommandFor("CALL " + SP_SELECTNURSINGSTATIONSFOR +
                                  "(" + INPUT_PARAM_FACILITYID + ")",
                                      CommandType.Text,
                                      facility);

                cmd.Parameters[INPUT_PARAM_FACILITYID].Value = facility.Oid;

                reader = ExecuteReader(cmd);

                while (reader.Read())
                {
                    var nursingStation = new NursingStation(
                        PersistentModel.NEW_OID,
                        ReferenceValue.NEW_VERSION,
                        reader.GetString(COL_NURSINGSTATIONDESCRIPTION),
                        reader.GetString(COL_NURSINGSTATIONCODE),
                        reader.GetInt32(COL_PREVIOUS_CENSUS),
                        reader.GetInt32(COL_ADMIT_TODAY),
                        reader.GetInt32(COL_DISCHARGE_TODAY),
                        reader.GetInt32(COL_DEATHS_TODAY),
                        reader.GetInt32(COL_TRANSFER_FROM_TODAY),
                        reader.GetInt32(COL_TRANSFER_TO_TODAY),
                        reader.GetInt32(COL_AVAILABLE_BEDS),
                        reader.GetInt32(COL_TOTAL_BEDS),
                        reader.GetInt32(COL_TOTAL_OCCUPIEDBEDS_FOR_MONTH),
                        reader.GetInt32(COL_TOTAL_BEDS_FOR_MONTH),
                        reader.GetString(COL_SITECODE));

                    nursingStations.Add(nursingStation);
                }
            }

            catch (Exception e)
            {
                string msg = "Failed to execute NursingStations for " + facility.Description;
                throw BrokerExceptionFactory.BrokerExceptionFrom(msg, e, c_log);
            }

            finally
            {
                Close(reader);
                Close(cmd);
            }

            return nursingStations;
        }

        public NursingStation NursingStationFor(Facility facility, string nursingStationCode)
        {
            NursingStation nursingStation = null;

            IList<NursingStation> list = this.CachedNursingStationsFor(facility);
            foreach (NursingStation ns in list)
            {
                if (ns.Code.Equals(nursingStationCode))
                {
                    nursingStation = ns;
                }
            }
            return nursingStation;
        }

        public string GetEntireNursingStationCode(Facility facility, string nsCode)
        {
            string longNSCode = nsCode;
            IList<NursingStation> nursingStations = this.CachedNursingStationsFor(facility);
            foreach (NursingStation ns in nursingStations)
            {
                if (ns.Code.Trim().Equals(nsCode))
                {
                    longNSCode = ns.Code;
                    break;
                }
            }

            return longNSCode;
        }

        /// <summary>
        /// Releases the previously reserved location for an account if any, 
        /// then reserves the newly requested location for a given facility
        /// </summary>
        /// <param name="reservationCriteria"></param>
        /// <returns></returns>
        public ReservationResults Reserve( ReservationCriteria reservationCriteria )
        {
            try
            {
                cmd = this.CommandFor( "CALL " + SP_RESERVELOCATION +
                    "(" + PARAM_OLD_NURSING_STATION + 
                    "," + PARAM_OLD_ROOM + 
                    "," + PARAM_OLD_BED +
                    "," + PARAM_NEW_NURSING_STATION + 
                    "," + PARAM_NEW_ROOM + 
                    "," + PARAM_NEW_BED +
                    "," + INPUT_PARAM_FACILITYID +
                    "," + PARAM_PATIENT_TYPE_CODE + 
                    "," + OUTPUT_PARAM_PATIENT_FNAME + 
                    "," + OUTPUT_PARAM_PATIENT_LNAME +
                    "," + OUTPUT_PARAM_PATIENT_MI + 
                    "," + OUTPUT_PARAM_ACCOUNT_NUMBER +
                    "," + OUTPUT_PARAM_RESERVATION_RESULT + ")",
                    CommandType.Text,
                    reservationCriteria.Facility);            

                if( reservationCriteria.OldLocation != null )
                {
                    if( reservationCriteria.OldLocation.NursingStation != null )
                    {
                        cmd.Parameters[PARAM_OLD_NURSING_STATION].Value 
                            = reservationCriteria.OldLocation.NursingStation.Code;
                    }
                    else
                    {
                        cmd.Parameters[PARAM_OLD_NURSING_STATION].Value = DBNull.Value;
                    }
                    if( reservationCriteria.OldLocation.Room != null )
                    {
                        cmd.Parameters[PARAM_OLD_ROOM].Value = 
                            reservationCriteria.OldLocation.Room.Code;
                    }
                    else
                    {
                        cmd.Parameters[PARAM_OLD_ROOM].Value = DBNull.Value;
                    }
                    if( reservationCriteria.OldLocation.Bed != null )
                    {
                        cmd.Parameters[PARAM_OLD_BED].Value = 
                            reservationCriteria.OldLocation.Bed.Code;
                    }
                    else
                    {
                        cmd.Parameters[PARAM_OLD_BED].Value = DBNull.Value;
                    }
                }
                else
                {
                    cmd.Parameters[PARAM_OLD_NURSING_STATION].Value = DBNull.Value;
                    cmd.Parameters[PARAM_OLD_ROOM].Value = DBNull.Value;
                    cmd.Parameters[PARAM_OLD_BED].Value = DBNull.Value;
                }

                cmd.Parameters[PARAM_NEW_NURSING_STATION].Value 
                    = reservationCriteria.NewLocation.NursingStation.Code;
                cmd.Parameters[PARAM_NEW_ROOM].Value = reservationCriteria.NewLocation.Room.Code;
                cmd.Parameters[PARAM_NEW_BED].Value = reservationCriteria.NewLocation.Bed.Code;
                cmd.Parameters[INPUT_PARAM_FACILITYID].Value = reservationCriteria.Facility.Oid;
                cmd.Parameters[PARAM_PATIENT_TYPE_CODE].Value = reservationCriteria.PatientType.Code;

                cmd.Parameters[OUTPUT_PARAM_PATIENT_FNAME].Direction = ParameterDirection.Output;
                cmd.Parameters[OUTPUT_PARAM_PATIENT_LNAME].Direction = ParameterDirection.Output;
                cmd.Parameters[OUTPUT_PARAM_PATIENT_MI].Direction = ParameterDirection.Output;
                cmd.Parameters[OUTPUT_PARAM_ACCOUNT_NUMBER].Direction = ParameterDirection.Output;
                cmd.Parameters[OUTPUT_PARAM_RESERVATION_RESULT].Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                ReservationResults reservation = null;
                string patientFName = String.Empty;
                string patientLName = String.Empty;
                string patientMI = String.Empty;
                long accountNumber = 0;
                string reservationStatus = String.Empty;
                
                reservationStatus = cmd.Parameters[OUTPUT_PARAM_RESERVATION_RESULT].Value.ToString();
                patientFName = cmd.Parameters[OUTPUT_PARAM_PATIENT_FNAME].Value.ToString();
                patientLName = cmd.Parameters[OUTPUT_PARAM_PATIENT_LNAME].Value.ToString();
                patientMI = cmd.Parameters[OUTPUT_PARAM_PATIENT_MI].Value.ToString();
                if( cmd.Parameters[OUTPUT_PARAM_ACCOUNT_NUMBER].Value != DBNull.Value )
                {
                    accountNumber = Convert.ToInt64( cmd.Parameters[OUTPUT_PARAM_ACCOUNT_NUMBER].Value );
                }              

                reservation = new ReservationResults(
                    ReferenceValue.NEW_OID, ReferenceValue.NEW_VERSION,
                    String.Empty, String.Empty, reservationStatus,
                    patientFName, patientLName, patientMI, 
                    accountNumber, reservationCriteria.NewLocation );                

                return reservation;
            }
            catch( Exception ex )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( "Unexpected Exception", ex, c_log );
            }
            finally
            {
                base.Close( reader );
                base.Close( cmd );
            }        
        }

        /// <summary>
        /// Releases the previously reserved location for the account
        /// </summary>
        /// <param name="location"></param>
        /// <param name="aFacility"></param>
        public void ReleaseReservedBed( Location location, Facility aFacility )
        {
            try
            { 
                ReleaseBed( location, aFacility ); 
            }

            catch( Exception e )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( e, c_log );
            }
        }

        /// <summary>
        /// Given the nursing station and facility, return a list of applicable accomodation codes
        /// </summary>
        /// <param name="nursingStationCode"></param>
        /// <param name="aFacility"></param>
        /// <returns></returns>
        public ICollection AccomodationCodesFor( string nursingStationCode, Facility aFacility )
        {
            ArrayList accomodationCodes = null;
            string blankAcc = string.Empty;

            string key = CacheKeys.CACHE_KEY_FOR_ACCOMODATIONCODES;

            LoadCacheDelegate LoadData = delegate()
            {
                accomodationCodes = new ArrayList();

                Accomodation blankAccomodation = new Accomodation(
                    ReferenceValue.NEW_OID,
                    ReferenceValue.NEW_VERSION, 
                    blankAcc ,
                    blankAcc );
                accomodationCodes.Add (blankAccomodation);

                try
                {
                    cmd = this.CommandFor( "CALL " + SP_SELECTACCOMODATIONCODESFOR +
                        "(" + PARAM_NURSING_STATION +
                        "," + INPUT_PARAM_FACILITYID + ")",
                        CommandType.Text,
                        aFacility);
                    
                    cmd.Parameters[PARAM_NURSING_STATION].Value = nursingStationCode;
                    cmd.Parameters[INPUT_PARAM_FACILITYID].Value = aFacility.Oid;

                    reader = this.ExecuteReader( cmd );

                    while( reader.Read() )
                    {
                        Accomodation accomodation = new Accomodation(
                            ReferenceValue.NEW_OID,
                            ReferenceValue.NEW_VERSION, 
                            reader.GetString( COL_ACCOMODATION_DESC ).Trim(),
                            reader.GetString( COL_ACCOMODATION_KEY ).Trim() );

                        accomodationCodes.Add( accomodation );
                    }
                }
                catch( Exception e )
                {
                    throw BrokerExceptionFactory.BrokerExceptionFrom( e, c_log );
                }
                finally
                {
                   base.Close( reader );
                   base.Close( cmd );
                }
                return accomodationCodes;
            };
            try
            {
                CacheManager cacheManager = new CacheManager();
                accomodationCodes = (ArrayList)cacheManager.GetCollectionBy(key, aFacility.Code, nursingStationCode, LoadData);
            }
            catch (Exception e)
            {   
                throw BrokerExceptionFactory.BrokerExceptionFrom("locationPbarBroker(nursingStations) failed to initialize", e, c_log);
            }

          return accomodationCodes;
        }

        /// <summary>
        /// Returns Rooms for a given Facility and Nursing Station Code
        /// </summary>
        /// <param name="facility"></param>
        /// <param name="nursingStationCode"></param>
        /// <returns></returns>
        public ICollection RoomsFor( Facility facility, string nursingStationCode )
        {
            ArrayList rooms = new ArrayList();
            
            string key = CacheKeys.CACHE_KEY_FOR_ROOMS;

            LoadCacheDelegate LoadData = delegate()
            {
            try
            {
                cmd = this.CommandFor( "CALL " + SP_SELECTROOMSFOR +
                    "(" + INPUT_PARAM_FACILITYID + 
                    "," + PARAM_NURSING_STATION + ")",
                    CommandType.Text,
                    facility);

                cmd.Parameters[INPUT_PARAM_FACILITYID].Value = facility.Oid;
                cmd.Parameters[PARAM_NURSING_STATION].Value = nursingStationCode;

                reader = this.ExecuteReader( cmd ) ; 

                while( reader.Read() )
                {
                    Room room = new Room(
                        PersistentModel.NEW_OID,
                        ReferenceValue.NEW_VERSION,
                        string.Empty,
                        Convert.ToString( reader.GetValue( COL_ROOM ) ) );
                    rooms.Add( room );
                }
                return rooms;
            }
            catch( Exception e )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( e, c_log );
            }
            finally
            {
               base.Close( reader );
               base.Close( cmd );
            }     
        };
            try
            {
                CacheManager cacheManager = new CacheManager();
                rooms = (ArrayList)cacheManager.GetCollectionBy(key, facility.Code, nursingStationCode, LoadData);
            }
            catch (Exception e)
            {   
                throw BrokerExceptionFactory.BrokerExceptionFrom("locationPbarBroker(rooms) failed to initialize", e, c_log);
            }
            return rooms;
        }

        /// <summary>
        /// Gets all the Locations for a given facility, nursing station and room
        /// </summary>
        /// <param name="locationSearchCriteria"></param>
        /// <returns></returns>
        public ICollection LocationMatching( LocationSearchCriteria locationSearchCriteria )
        {
            c_log.InfoFormat("LocationsMatching - Started at: {0}", DateTime.Now.ToString());
            ArrayList accounts = new ArrayList();
            try
            {
                string patientFirstName             = String.Empty;
                string patientLastName              = String.Empty;
                string patientMiddleIntital         = String.Empty;
                string patientSexCode               = String.Empty;
                string patientSexDescription        = String.Empty;
                string nursingStationCode           = String.Empty;
                string roomCode                     = String.Empty;
                string bedCode                      = String.Empty;
                string overFlow                     = String.Empty;
                string pendingAdmission             = String.Empty;
                string patientTypeCode              = String.Empty;
                string patientDateOfBirthString     = String.Empty;
                string genderCode                   = String.Empty;
                long accountNumber                  = 0;
                int patientMRN                      = 0;
                DateTime patientDateOfBirth         = DateTime.MinValue;

                Gender gender                       = null;
                AccountProxy accountProxy           = null;
                Patient patient                     = null;
                Name patientName                    = null;
                Location location                   = null;
                Room room                           = null;
                Bed bed                             = null;
                NursingStation i_nursingStation     = null;
	            VisitType visitType                 = null;
                

				Facility facility = null;

                IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType< IFacilityBroker >() ;
				facility = facilityBroker.FacilityWith( locationSearchCriteria.HSPCode );

                string procName;
                if( locationSearchCriteria.Gender == null || locationSearchCriteria.Gender == string.Empty )
                {
                    procName = SP_SELECTLOCATIONMATCHINGWOGENDER;
                }
                else
                {
                    procName = SP_SELECTLOCATIONMATCHINGWITHGENDER;
                }
                cmd = this.CommandFor( "CALL " + procName +
                    "(" + INPUT_PARAM_FACILITYID + 
                    "," + INPUT_PARAM_ISOCCUPIED + 
                    "," + INPUT_PARAM_GENDER + 
                    "," + INPUT_PARAM_NURSINGSTATION + 
                    "," + INPUT_PARAM_ROOM + ")",
                    CommandType.Text,
                    facility);

                cmd.Parameters[INPUT_PARAM_FACILITYID].Value = facility.Oid;

                if( locationSearchCriteria.IsOccupiedBeds )
                {
                    cmd.Parameters[INPUT_PARAM_ISOCCUPIED].Value = SHOW_UNOCCUPIED_BEDS;
                }
                else
                {
                    cmd.Parameters[INPUT_PARAM_ISOCCUPIED].Value = SHOW_ALL_BEDS;
                }

                if( locationSearchCriteria.Gender == null || 
                    locationSearchCriteria.Gender == String.Empty )
                {
                    cmd.Parameters[INPUT_PARAM_GENDER].Value = DBNull.Value;
                }
                else
                {
                    cmd.Parameters[INPUT_PARAM_GENDER].Value = locationSearchCriteria.Gender;
                }

                if( locationSearchCriteria.NursingStation == ALL_NURSING_STATIONS )
                {
                    cmd.Parameters[INPUT_PARAM_NURSINGSTATION].Value = DBNull.Value;
                }
                else
                {
                    cmd.Parameters[INPUT_PARAM_NURSINGSTATION].Value = locationSearchCriteria.NursingStation;
                }

                if( locationSearchCriteria.Room == ALL_ROOMS )
                {
                    cmd.Parameters[INPUT_PARAM_ROOM].Value = DBNull.Value;
                }
                else
                {
                    cmd.Parameters[INPUT_PARAM_ROOM].Value = locationSearchCriteria.Room;
                }

                reader = null;
                cmd.CommandTimeout = 0;

                c_log.DebugFormat("LocationMatching - executing reader started at: {0}", DateTime.Now.ToString());

                reader = this.ExecuteReader( cmd );

                IPhysicianBroker physicianBroker = BrokerFactory.BrokerOfType< IPhysicianBroker >() ;
                IDemographicsBroker demographicsBroker = BrokerFactory.BrokerOfType< IDemographicsBroker >() ;
                IPatientBroker patientBroker = BrokerFactory.BrokerOfType< IPatientBroker >() ;

                c_log.DebugFormat("LocationMatching - looping reader {0}", DateTime.Now.ToString());

                while( reader.Read() )
                {
                    patientFirstName         = String.Empty;
                    patientLastName          = String.Empty;
                    patientMiddleIntital     = String.Empty;
                    patientSexCode           = String.Empty;
                    nursingStationCode       = String.Empty;
                    bedCode                  = String.Empty;
                    overFlow                 = String.Empty;
                    pendingAdmission         = String.Empty;
                    patientDateOfBirthString = String.Empty;
                    genderCode               = String.Empty;
                    patientDateOfBirth       = DateTime.MinValue;
                    
                    gender          = null;
                    accountProxy    = null;
                    patient         = null;
                    patientName     = null;
                    location        = null;
                    room            = null;
                    bed             = null;

                    patientFirstName     = reader.GetString( COL_FIRSTNAME );
                    patientLastName      = reader.GetString( COL_LASTNAME );
                    patientMiddleIntital = reader.GetString( COL_MIDDLEINITIAL );
                    patientDateOfBirthString = reader.GetString( COL_DATEOFBIRTH );
                    nursingStationCode   = reader.GetString( COL_NURSINGSTATION );
                    roomCode = Convert.ToString( reader.GetValue( COL_ROOM ) );
                    bedCode              = reader.GetString( COL_BED );
                    overFlow             = reader.GetString( COL_OVERFLOW );
                    pendingAdmission     = reader.GetString( COL_PENDING_ADMISSION );
                    genderCode           = reader.GetString( COL_GENDER_ID );

                    c_log.DebugFormat("LocationMatching - calling DemographicsBroker: {0}", DateTime.Now.ToString());
                    gender = demographicsBroker.GenderWith( facility.Oid, genderCode );

                    if( genderCode != null && 
                        genderCode != String.Empty &&
                        gender     != null )
                    {
                        gender.Code = genderCode;
                    }

                    patientName = new Name( patientFirstName, 
                                            patientLastName,
                                            patientMiddleIntital,
                                            String.Empty );

                    if( patientDateOfBirthString != null && 
                        patientDateOfBirthString != String.Empty )
                    {
                        patientDateOfBirth = 
                            DateTimeUtilities.DateTimeFromString(
                                                patientDateOfBirthString );
                    }

                    patient = new Patient(
                        PersistentModel.NEW_OID, 
                        PersistentModel.NEW_VERSION,
                        patientName,
                        patientMRN, 
                        patientDateOfBirth,
                        null,
                        gender,
                        facility );

                    accountProxy = new AccountProxy();
                    accountProxy.Patient = patient;
                    accountProxy.IsolationCode = 
                        reader.GetChar( COL_ISOLATIONCODE ).ToString();

                    if( nursingStationCode != null &&  
                        nursingStationCode.Trim() != String.Empty )
                    {
                        i_nursingStation = new NursingStation( 
                                                ReferenceValue.NEW_OID,
                                                ReferenceValue.NEW_VERSION,
                                                String.Empty, 
                                                nursingStationCode );
                    }
                    if( roomCode != null && roomCode != "0" )
                    {
                        room = new Room( ReferenceValue.NEW_OID,
                                         ReferenceValue.NEW_VERSION, 
                                         String.Empty, 
                                         roomCode );
                    }
                    if( bedCode != null && bedCode.Trim() != String.Empty )
                    {
                        bed = new Bed( ReferenceValue.NEW_OID,
                                       ReferenceValue.NEW_VERSION,
                                       String.Empty, 
                                       bedCode );
                    }

                    location = new Location( ReferenceValue.NEW_OID,
                                            ReferenceValue.NEW_VERSION, 
                                            String.Empty, 
                                            String.Empty, 
                                            i_nursingStation, 
                                            room, 
                                            bed );
                    accountProxy.Location = location;
                    accountNumber = reader.GetInt64(COL_ACCOUNTNUMBER); 
                    accountProxy.AccountNumber = accountNumber;

                    if( reader.GetInt64( COL_ATTENDING_DOCTOR_ID ) != 0 )
                    {
						accountProxy.AddPhysicianRelationship( 
							physicianBroker.BuildAttendingPhysicianRelationship(
							facility.Oid, 
							reader.GetInt32( COL_ATTENDING_DOCTOR_ID ) ) );

                        patientTypeCode = reader.GetString( COL_PATIENTTYPECODE );

                        visitType = patientBroker.PatientTypeWith( facility.Oid, patientTypeCode.Trim() );

                        accountProxy.KindOfVisit = visitType;
                    }
                    accountProxy.Overflow = overFlow;
                    accountProxy.PendingAdmission = pendingAdmission;

                    accounts.Add( accountProxy );
                    c_log.DebugFormat("LocationMatching - Account proxy added at: {0}", DateTime.Now.ToString());
                }

                c_log.InfoFormat("LocationMatching - executing reader completed at: {0}", DateTime.Now.ToString());
                return accounts;
            }
            catch( Exception e )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( e, c_log );
            }
            finally
            {
               base.Close( reader );
               base.Close( cmd );
            }      
        }

        /// <summary>
        /// This will return an arraylist containing accomodation_code & 
        /// accomodation_Description for all the facilities. 
        /// </summary>
        /// <returns></returns>
        public IList AccomodationsFor( long facilityNumber )
        {
            string accCode = string.Empty;
            string accDescription = string.Empty;
            ArrayList allAccomodations = new ArrayList();
//            SafeReader reader = null;
            iDB2Command cmd = null;
             
            string key = CacheKeys.CACHE_KEY_FOR_ALLACCOMODATIONS;

            LoadCacheDelegate LoadData = delegate()
            {
            Accomodation accomodation = null;

            string blankAcc = String.Empty ;
            Accomodation blankAccomodation = new Accomodation(
                ReferenceValue.NEW_OID,
                ReferenceValue.NEW_VERSION, 
                blankAcc ,
                blankAcc );
            allAccomodations.Add (blankAccomodation);

            try
            {
                IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType< IFacilityBroker >() ;
                Facility facility = facilityBroker.FacilityWith(facilityNumber);

               
                cmd = this.CommandFor( "CALL " + SP_SELECTALLACCOMODATIONCODESFOR + 
                    "(" + PARAM_HSP + ")",
                    CommandType.Text,
                    facility);

                cmd.Parameters[PARAM_HSP].Value = facilityNumber;

                reader = this.ExecuteReader(cmd); 
                
                while( reader.Read() )
                {
                    accCode = reader.GetString( COL_ACCOMODATION_CODE ).Trim();
                    accDescription = reader.GetString( COL_ACCOMODATION_DESC ).Trim();

                    accomodation = new Accomodation( ReferenceValue.NEW_OID,
                        ReferenceValue.NEW_VERSION,
                        accDescription,accCode );
                                    
                    allAccomodations.Add( accomodation );
                }
            }
            catch( Exception e )
            {
                string msg = "LocationPBARBroker(Accomodation) failed to initialize.";
                throw BrokerExceptionFactory.BrokerExceptionFrom( msg, e, c_log );
            }
            finally
            {
                base.Close( reader );
                base.Close( cmd );
            }
            return allAccomodations;
            };
            try
            {
                CacheManager cacheManager = new CacheManager();
                allAccomodations = (ArrayList)cacheManager.GetCollectionBy(key, facilityNumber, LoadData);
            }
            catch (Exception e)
            {   
                throw BrokerExceptionFactory.BrokerExceptionFrom("locationPbarBroker(AccomodationFor) failed to initialize", e, c_log);
            }
            return allAccomodations;

        }

        /// <summary>
        /// Get Accomodation for a particular Facility and Oid
        /// </summary>
        /// <param name="facilityNumber"></param>
        /// <param name="oid"></param>
        /// <returns></returns>
        public Accomodation AccomodationFor( long facilityNumber, long oid )
        {
            Accomodation selectedAccomodation = null;

            ArrayList accomodationsWith = ( ArrayList )this.AccomodationsFor( 
                facilityNumber );
            foreach( Accomodation accomodation in accomodationsWith )
            {
                if( accomodation.Oid == oid )
                {
                    selectedAccomodation = accomodation;
                }
            }
            return selectedAccomodation;
        }

        
        /// <summary>
        /// Releases the previously reserved location for an account if any, 
        /// then reserves the newly requested location for a given facility
        /// </summary>
        /// <returns></returns>
        public ReservationResults GetBedStatus( Location location, Facility facility )
        {
            try
            {
                cmd = this.CommandFor( "CALL " + SP_GET_BED_STATUS +
                    "(" + PARAM_NURSING_STATION + 
                    "," + PARAM_ROOM + 
                    "," + PARAM_BED +
                    "," + INPUT_PARAM_FACILITYID +
                    "," + OUTPUT_PARAM_PATIENT_FNAME + 
                    "," + OUTPUT_PARAM_PATIENT_LNAME +
                    "," + OUTPUT_PARAM_PATIENT_MI + 
                    "," + OUTPUT_PARAM_ACCOUNT_NUMBER +
                    "," + OUTPUT_PARAM_RESERVATION_RESULT + ")",
                    CommandType.Text,
                    facility);            

                cmd.Parameters[PARAM_NURSING_STATION].Value = location.NursingStation.Code;
                cmd.Parameters[PARAM_ROOM].Value = location.Room.Code;
                cmd.Parameters[PARAM_BED].Value = location.Bed.Code;
                cmd.Parameters[INPUT_PARAM_FACILITYID].Value = facility.Oid;

                cmd.Parameters[OUTPUT_PARAM_PATIENT_FNAME].Direction = ParameterDirection.Output;
                cmd.Parameters[OUTPUT_PARAM_PATIENT_LNAME].Direction = ParameterDirection.Output;
                cmd.Parameters[OUTPUT_PARAM_PATIENT_MI].Direction = ParameterDirection.Output;
                cmd.Parameters[OUTPUT_PARAM_ACCOUNT_NUMBER].Direction = ParameterDirection.Output;
                cmd.Parameters[OUTPUT_PARAM_RESERVATION_RESULT].Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                ReservationResults reservation = null;
                string patientFName = String.Empty;
                string patientLName = String.Empty;
                string patientMI = String.Empty;
                long accountNumber = 0;
                string reservationStatus = String.Empty;
            
                reservationStatus = cmd.Parameters[OUTPUT_PARAM_RESERVATION_RESULT].Value.ToString();
                patientFName = cmd.Parameters[OUTPUT_PARAM_PATIENT_FNAME].Value.ToString();
                patientLName = cmd.Parameters[OUTPUT_PARAM_PATIENT_LNAME].Value.ToString();
                patientMI = cmd.Parameters[OUTPUT_PARAM_PATIENT_MI].Value.ToString();
                if( cmd.Parameters[OUTPUT_PARAM_ACCOUNT_NUMBER].Value != DBNull.Value )
                {
                    accountNumber = Convert.ToInt64( cmd.Parameters[OUTPUT_PARAM_ACCOUNT_NUMBER].Value );
                }              

                reservation = new ReservationResults( ReferenceValue.NEW_OID, 
                                                      ReferenceValue.NEW_VERSION,
                                                      String.Empty, 
                                                      String.Empty, 
                                                      reservationStatus,
                                                      patientFName, 
                                                      patientLName, 
                                                      patientMI, 
                                                      accountNumber, 
                                                      location );                

                return reservation;
            }
            catch( Exception ex )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( "Unexpected Exception", ex, c_log );
            }
            finally
            {
                base.Close( reader );
                base.Close( cmd );
            }        
        }

        /// <summary>
        /// Validates the given location information for a given facility
        /// </summary>
        /// <param name="location"></param>
        /// <param name="aFacility"></param>
        public string ValidateLocation(Location location, Facility aFacility )
        {
            try
            {

                cmd = this.CommandFor( "CALL " + SP_VALIDATE_LOCATION +
                    "(" + PARAM_NEW_NURSING_STATION + 
                    "," + PARAM_NEW_ROOM + 
                    "," + PARAM_NEW_BED + 
                    "," + INPUT_PARAM_FACILITYID + 
                    "," + OUTPUT_PARAM_LOCATION_RESULT + ")",
                    CommandType.Text,
                    aFacility);
                                
                if( location != null )
                {
                    cmd.Parameters[PARAM_NEW_NURSING_STATION].Value = location.NursingStation.Code;
                    cmd.Parameters[PARAM_NEW_ROOM].Value = location.Room.Code; 
                    cmd.Parameters[PARAM_NEW_BED].Value = location.Bed.Code;
                }
                else
                {
                    cmd.Parameters[PARAM_NEW_NURSING_STATION].Value = DBNull.Value;
                    cmd.Parameters[PARAM_NEW_ROOM].Value = DBNull.Value;
                    cmd.Parameters[PARAM_NEW_BED].Value = DBNull.Value;
                }

                cmd.Parameters[INPUT_PARAM_FACILITYID].Value = aFacility.Oid;

                cmd.Parameters[OUTPUT_PARAM_LOCATION_RESULT].Direction = ParameterDirection.Output;
                            
                cmd.ExecuteNonQuery();
                //string locationStatus = String.Empty;
                string locationStatus = cmd.Parameters[OUTPUT_PARAM_LOCATION_RESULT].Value.ToString();
                return locationStatus;

            }
            catch( Exception e )
            {
                 throw BrokerExceptionFactory.BrokerExceptionFrom( e, c_log );
            }
            finally
            {
                base.Close( reader );
                base.Close( cmd );
            }
        }
      
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        /// <summary>
        /// Releases the previously reserved location for the account
        /// </summary>
        /// <param name="location"></param>
        /// <param name="aFacility"></param>
        private void ReleaseBed( Location location, Facility aFacility )
        {
            try
            {

                cmd = this.CommandFor( "CALL " + SP_RELEASE_RESERVED_BED +
                    "(" + PARAM_OLD_NURSING_STATION + 
                    "," + PARAM_OLD_ROOM + 
                    "," + PARAM_OLD_BED + 
                    "," + INPUT_PARAM_FACILITYID + ")",
                    CommandType.Text,
                    aFacility);
                                
                if( location != null )
                {
                    cmd.Parameters[PARAM_OLD_NURSING_STATION].Value = location.NursingStation.Code;
                    cmd.Parameters[PARAM_OLD_ROOM].Value = location.Room.Code; 
                    cmd.Parameters[PARAM_OLD_BED].Value = location.Bed.Code;
                }
                else
                {
                    cmd.Parameters[PARAM_OLD_NURSING_STATION].Value = DBNull.Value;
                    cmd.Parameters[PARAM_OLD_ROOM].Value = DBNull.Value;
                    cmd.Parameters[PARAM_OLD_BED].Value = DBNull.Value;
                }

                cmd.Parameters[INPUT_PARAM_FACILITYID].Value = aFacility.Oid;
                            
                cmd.ExecuteNonQuery();
                
            }
            catch( Exception e )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( e );
            }
            finally
            {
                base.Close( reader );
                base.Close( cmd );
            }
        }

        #endregion

        #region Construction and Finalization
        public LocationPBARBroker()
        {			
        }
        public LocationPBARBroker( string cxnString )
            : base( cxnString )
    {
    }
        public LocationPBARBroker( IDbTransaction txn )
            : base( txn )
    {
    }
        #endregion

        #region Data Elements

        private SafeReader reader = null;
        private iDB2Command cmd = null;
        private static readonly ILog c_log = 
            LogManager.GetLogger( typeof( LocationPBARBroker ) );

        #endregion

        #region Constants

        private const string 
            COL_ACCOMODATION_KEY                = "AccomodationKey",
            COL_ACCOMODATION_CODE               = "AccomodationCode",
            COL_ACCOMODATION_DESC               = "AccomodationDesc",
            COL_FIRSTNAME                       = "FirstName",
            COL_LASTNAME                        = "LastName",
            COL_MIDDLEINITIAL                   = "MiddleInitial",  
            COL_GENDER_ID                       = "GenderID",
            COL_DATEOFBIRTH                     = "DOB",
            COL_NURSINGSTATION                  = "NursingStation", 
            COL_ROOM	                        = "Room", 
            COL_BED	                            = "Bed", 
            COL_OVERFLOW                        = "OverFlowFlag",
            COL_PENDING_ADMISSION               = "PendingAdmission",
            COL_ISOLATIONCODE                   = "IsolationCode", 
            COL_PATIENTTYPECODE                 = "PatientType",
            COL_ACCOUNTNUMBER                   = "AccountNumber",
            COL_ATTENDING_DOCTOR_ID             = "AttendingDrId",
            COL_NURSINGSTATIONCODE              = "NursingStationCode",
            COL_NURSINGSTATIONDESCRIPTION       = "NursingStationDescription",
            COL_PREVIOUS_CENSUS                 = "PreviousCensus",
            COL_ADMIT_TODAY                     = "AdmitToday",
            COL_DISCHARGE_TODAY                 = "DischargeToday",
            COL_DEATHS_TODAY                    = "DeathsToday",
            COL_TRANSFER_FROM_TODAY             = "TransferredFromToday",
            COL_TRANSFER_TO_TODAY               = "TransferredToToday",
            COL_AVAILABLE_BEDS                  = "AvailableBeds",
            COL_TOTAL_BEDS                      = "TotalBeds",                        
            COL_TOTAL_OCCUPIEDBEDS_FOR_MONTH    = "TotalOccupiedBedsForMonth",
            COL_TOTAL_BEDS_FOR_MONTH            = "TotalBedsForMonth",
            COL_SITECODE                        = "SITECODE",
            COL_STATUS                          = "STATUS",
           
            PARAM_NURSING_STATION               = "@P_NursingStation",
            PARAM_ROOM                          = "@P_Room",
            PARAM_BED                           = "@P_Bed",
            PARAM_OLD_NURSING_STATION           = "@P_Old_NursingStation",
            PARAM_OLD_ROOM                      = "@P_Old_Room",
            PARAM_OLD_BED                       = "@P_Old_Bed",
            PARAM_NEW_NURSING_STATION           = "@P_New_NursingStation",
            PARAM_NEW_ROOM                      = "@P_New_Room",
            PARAM_NEW_BED                       = "@P_New_Bed",
            PARAM_PATIENT_TYPE_CODE             = "@P_PatientTypeCode",
            PARAM_HSP                           = "@P_HSP",
            INPUT_PARAM_FACILITYID              = "@P_facilityID",
            INPUT_PARAM_ISOCCUPIED              = "@P_IsOccupied",
            INPUT_PARAM_GENDER                  = "@P_Gender",
            INPUT_PARAM_NURSINGSTATION          = "@P_NursingStation",
            INPUT_PARAM_ROOM                    = "@P_Room",
            INPUT_PARAM_FIRST_NAME              = "@P_FirstName",
            INPUT_PARAM_LAST_NAME               = "@P_LastName",
            INPUT_PARAM_FACID                   = "@P_FacilityID",
            INPUT_PARAM_SSN                     = "@P_SSN",
            INPUT_PARAM_MRN                     = "@P_MRN",
            INPUT_PARAM_ACCOUNT                 = "@P_AccountNumber",
            INPUT_PARAM_DOB                     = "@P_DOB",
            INPUT_PARAM_ZIP                     = "@P_ZIP",
            OUTPUT_PARAM_PATIENT_FNAME          = "@O_PatientFName",
            OUTPUT_PARAM_PATIENT_LNAME          = "@O_PatientLName",
            OUTPUT_PARAM_PATIENT_MI             = "@O_PatientMI",
            OUTPUT_PARAM_ACCOUNT_NUMBER         = "@O_AccountNumber",
            OUTPUT_PARAM_RESERVATION_RESULT     = "@O_ReservationResult",
            OUTPUT_PARAM_LOCATION_RESULT        = "@O_LocationResult";

        private const string
            SP_SELECTACCOMODATIONCODESFOR       = "ACCOMODATIONCODESFOR4",
            SP_RESERVELOCATION                  = "RESERVE",
            SP_RELEASE_RESERVED_BED             = "ReleaseReservedBed",
            SP_SELECTROOMSFOR                   = "SelectRoomsFor",
            SP_SELECTLOCATIONMATCHINGWITHGENDER = "LOCATIONMATCHINGWITHGENDER",
            SP_SELECTLOCATIONMATCHINGWOGENDER   = "LOCATIONMATCHINGWOGENDER",
            SP_SELECTNURSINGSTATIONSFOR         = "SelectNursingStationsFor",
            SP_SELECTALLACCOMODATIONCODESFOR    = "SELECTALLACCOMODATIONCODESFOR",
            SP_GET_BED_STATUS                   = "GetBedStatus",
            SP_VALIDATE_LOCATION                = "ValidateLocation",
            SP_CHECKFORDUPELOCATIONS            = "CheckForDupeLocations";

        private const string
            ALL_NURSING_STATIONS = "All",
            ALL_ROOMS = "All",
            SHOW_UNOCCUPIED_BEDS = "N",
            SHOW_ALL_BEDS = "Y";

        private const long
            DUMMY_NUMBERS = 9999999999;

        #endregion
	}
}
