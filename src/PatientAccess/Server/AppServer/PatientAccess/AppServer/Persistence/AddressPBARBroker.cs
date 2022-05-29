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
using PatientAccess.Utilities;
using log4net;
using System.Text;

namespace PatientAccess.Persistence
{
    /// <summary>
    /// PBAR Address persistence broker
    /// </summary>
    public class AddressPBARBroker : PBARCodesBroker, IAddressBroker
    {
        #region Event Handlers
        #endregion

        #region Methods

        /// <summary>
        /// AllCountiesFor - return a list of counties for the specified facility
        /// </summary>
        /// <param name="facilityNumber">Facility ID</param>
        /// <returns>List of County objects</returns>

        public ICollection AllCountiesFor( long facilityNumber )
        {
            ICollection allCounties = null;
            string key = CacheKeys.CACHE_KEY_FOR_COUNTIES;
            InitFacility( facilityNumber );

            try
            {
                CacheManager cacheManager = new CacheManager();
                AllStoredProcName = SP_SELECTALLCOUNTIESFOR;
                allCounties = cacheManager.GetCollectionBy<County>( key, facilityNumber,
                                                                    LoadDataToArrayList,
                                                                    CountyFrom );
            }
            catch ( Exception e )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( "AddressPBARBroker(County) failed to initialize", e, c_log );
            }
            return allCounties;
        }

        /// <summary>
        /// AllCountries - return a list of all countries for the STD facility (999)
        /// </summary>
        /// <returns>List of Country objects</returns>
        public IList AllCountries( long facilityNumber )
        {
            ArrayList allCountries = null;
            string key = CacheKeys.CACHE_KEY_FOR_COUNTRIES;
            InitFacility( facilityNumber );
            try
            {
                CacheManager cacheManager = new CacheManager();
                AllStoredProcName = SP_SELECTALLCOUNTRIES;
                allCountries = ( ArrayList )cacheManager.GetCollectionBy<Country>( key, facilityNumber,
                    LoadDataToArrayList,
                    CountryFrom );
            }
            catch ( Exception e )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( "AddressPBARBroker(Country) failed to initialize", e, c_log );
            }

            return allCountries;
        }

        /// <summary>
        /// AllStates - return a list of all states for the STD facility (999)
        /// </summary>
        /// <returns>List of State objects</returns>
       public IList AllStates(long facilityID)
        {
            IList allStates = null;
            var key = CacheKeys.CACHE_KEY_FOR_STATES;
            this.InitFacility(facilityID);
            ArrayList oList = new ArrayList();
            iDB2Command cmd = null;
            StringBuilder sb = new StringBuilder();
            iDB2DataReader read = null;

            LoadCacheDelegate loadData = delegate()
            {
                try
                {
                    IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
                    Facility facility = facilityBroker.FacilityWith(facilityID);
                    sb.Append("CALL " + SP_SELECTALLSTATES + "(");
                    if (facilityID != 0)
                    {
                        sb.Append(PARAM_FACILITYID);
                    }
                    sb.Append(")");

                    cmd = this.CommandFor(sb.ToString(), CommandType.Text, facility);
                    if (facilityID != 0)
                        cmd.Parameters[PARAM_FACILITYID].Value = facilityID;
                    read = cmd.ExecuteReader();
                    while (read.Read())
                    {
                        State state = new State();
                        state.Code = read.GetString(1).TrimEnd();
                        state.Description = read.GetString(2).TrimEnd();
                        state.StateNumber = read.GetString(3).TrimEnd();

                        oList.Add(state);
                    }
                    allStates = oList;
                }
                catch (Exception e)
                {
                    throw BrokerExceptionFactory.BrokerExceptionFrom("AddressPBARBroker(State) failed to initialize", e, c_log);
                }
                return allStates;
            };
            try
            {
                CacheManager cacheManager = new CacheManager();
                this.AllStoredProcName = SP_SELECTALLSTATES;
                allStates =(IList) cacheManager.GetCollectionBy(key, facilityID,
                    loadData);
            }
            catch (Exception e)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom("AddressPBARBroker(State) failed to initialize", e, c_log);
            }
            return allStates;
        }
        public IList<State> AllUSStates(long facilityID)
        {
            return AllStates(facilityID)
                        .Cast<State>()
                        .Where( Address.IsUSState ).ToList();
        }

        public IList<State> AllNonUSStates(long facilityID)
        {
            return AllStates(facilityID)
                        .Cast<State>()
                        .Where( state => !Address.IsUSState( state ) ).ToList();
        }

        /// <summary>
        /// ContactPointFrom - construct a ContactPoint instance from the values
        /// in the result set
        /// </summary>
        /// <param name="reader">a SafeReader instance</param>
        /// <param name="facilityID"></param>
        /// <returns>a ContactPoint instance</returns>
        private ContactPoint ContactPointFrom( SafeReader reader, long facilityID )
        {
            Address address = AddressWithStreet2From( reader, facilityID );
            PhoneNumber pn = PhoneNumberFrom( reader );
            // view details window requires the cell phone number to be read too
            PhoneNumber cn = ReadCellPhoneNumberFrom( reader );
            EmailAddress em = EmailAddressFrom( reader );
            TypeOfContactPoint cptype = TypeOfContactPointFrom( reader );
            // use cell phone number constructor 
            ContactPoint cp = new ContactPoint( address, pn, cn, em, cptype );
            return cp;
        }

        /// <summary>
        /// ContactPointsForEmployer - Retrieve a collection of ContactPoints for the specified employer.
        /// For PBAR, employers are retrieved by Employer Code.
        /// </summary>
        /// <param name="facilityCode"> A string, facility code</param>
        /// <param name="employerCode">An employer code (long)</param>
        /// <returns>ArrayList of ContactPoints</returns>        
        public ArrayList ContactPointsForEmployer( string facilityCode, long employerCode )
        {
            iDB2Command cmd = null;
            ArrayList employerContactPoints = new ArrayList();
            SafeReader reader = null;

            try
            {
                IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
                Facility facility = facilityBroker.FacilityWith( facilityCode );

                cmd = CommandFor(
                    string.Format( "CALL {0} ({1},{2})",
                    SELECTCONTACTPOINTSFOREMPLOYER,
                    PARAM_FACILITYID,
                    PARAM_EMPLOYER_CODE ),
                    CommandType.Text,
                    facility );

                cmd.Parameters[PARAM_FACILITYID].Value = facility.Oid;
                cmd.Parameters[PARAM_EMPLOYER_CODE].Value = employerCode;

                reader = ExecuteReader( cmd ); 

                while ( reader.Read() )
                {
                    employerContactPoints.Add( EmployerContactPointFrom( reader, facility.Oid ) );
                }
            }
            catch ( Exception ex )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( "Unhandled Exception", ex, c_log );
            }
            finally
            {
                Close( reader );
                Close( cmd );
            }

            return employerContactPoints;
        }

        /// <summary>
        /// ContactPointsForEmployer - Retrieve a collection of ContactPoints for the specified employer.
        /// For PBAR, employers are retrieved by Employer Code.
        /// </summary>
        /// <param name="facilityCode"> A string, facility code</param>
        /// <param name="employerCode">An employer code (long)</param>
        /// <returns>ArrayList of ContactPoints</returns>        
        public ArrayList ContactPointsForEmployerApproval( string facilityCode, long employerCode )
        {
            iDB2Command cmd = null;
            ArrayList employerContactPoints = new ArrayList();
            SafeReader reader = null;

            try
            {
                IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
                Facility facility = facilityBroker.FacilityWith( facilityCode );

                cmd = CommandFor(
                    string.Format( "CALL {0} ({1},{2})",
                    SP_SELECTCONTACTPOINTSFOREMPLOYERAPPROVAL,
                    PARAM_FACILITYID,
                    PARAM_EMPLOYER_CODE ),
                    CommandType.Text,
                    facility );

                cmd.Parameters[PARAM_FACILITYID].Value = facility.Oid;
                cmd.Parameters[PARAM_EMPLOYER_CODE].Value = employerCode;

                reader = ExecuteReader( cmd ); 

                while ( reader.Read() )
                {
                    employerContactPoints.Add( EmployerContactPointFrom( reader, facility.Oid ) );
                }
            }
            catch ( Exception ex )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( "Unhandled Exception", ex, c_log );
            }
            finally
            {
                Close( reader );
                Close( cmd );
            }

            return employerContactPoints;
        }

        /// <summary>
        /// ContactPointsForGuarantor - Retrieve a collection of ContactPoints for the specified guarantor.
        /// For PBAR, employers are retrieved by Employer Code.
        /// </summary>
        /// <param name="facilityCode"> A string, facility code</param>
        /// <param name="accountNumber">An guarantor proxy (or guarantor)</param>
        /// <returns>ArrayList of ContactPoints</returns>        
        public ArrayList ContactPointsForGuarantor( string facilityCode, long accountNumber )
        {
            iDB2Command cmd = null;
            ContactPoint guarantorContactPoint;
            ArrayList guarantorContactPoints = new ArrayList();
            SafeReader reader = null;

            try
            {
                IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
                Facility facility = facilityBroker.FacilityWith( facilityCode );

                cmd = CommandFor(
                    string.Format( "CALL {0} ({1}, {2})",
                    SELECTCONTACTPOINTSFORGUARANTOR,
                    PARAM_FACILITYID,
                    PARAM_ACCOUNT_NUMBER ),
                    CommandType.Text,
                    facility );

                cmd.Parameters[PARAM_FACILITYID].Value = facility.Oid;
                cmd.Parameters[PARAM_ACCOUNT_NUMBER].Value = accountNumber;

                reader = ExecuteReader( cmd );

                while ( reader.Read() )
                {
                    guarantorContactPoint = ContactPointFrom( reader, facility.Oid );
                    guarantorContactPoints.Add( guarantorContactPoint );
                }

                PhoneNumber cellPhone = ReadCellPhoneNumberFrom( reader );
                CellPhoneConsent cellPhoneConsent = ReadCellPhoneConsentFrom( reader );

                if ( null != cellPhone )
                {
                    var contactPoint = CreateCellPhoneContactPoint(cellPhone, cellPhoneConsent);
                    guarantorContactPoints.Add( contactPoint );
                }
               
            }
            catch ( Exception ex )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( "Unhandled Exception", ex, c_log );
            }
            finally
            {
                Close( reader );
                Close( cmd );
            }

            return guarantorContactPoints;
        }

        /// <summary>
        /// ContactPointsForPatient - return an ArrayList of ContactPoint objects
        /// for the specified Mecial Record Number
        /// </summary>
        /// <param name="facilityCode"> A string, facility code</param>
        /// <param name="medicalRecordNumber">a MRN</param>
        /// <returns>an ArrayList of ContactPoint objects</returns>
        public ArrayList ContactPointsForPatient( string facilityCode, long medicalRecordNumber )
        {
            iDB2Command cmd = null;
            ArrayList personContactPoints = new ArrayList();
            SafeReader reader = null;

            try
            {
                IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
                Facility facility = facilityBroker.FacilityWith( facilityCode );

                cmd = CommandFor(
                        string.Format( "CALL {0} ({1},{2})",
                        SELECTCONTACTPOINTSFORPATIENT,
                        PARAM_FACILITYID,
                        PARAM_MRN ),
                        CommandType.Text,
                        facility );

                cmd.Parameters[PARAM_FACILITYID].Value = facility.Oid;
                cmd.Parameters[PARAM_MRN].Value = medicalRecordNumber;

                reader = ExecuteReader( cmd );

                //Address address = null;
                //PhoneNumber pn = null;
                //EmailAddress em = null;

                while ( reader.Read() )
                {
                    personContactPoints.Add( ContactPointFrom( reader, facility.Oid ) );
                }

                PhoneNumber cellPhone = ReadCellPhoneNumberFrom( reader );
                if ( null != cellPhone )
                {
                    personContactPoints.Add( CreateCellPhoneContactPoint( cellPhone ) );
                }
            }
            catch ( Exception ex )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( "Unhandled Exception", ex, c_log );
            }
            finally
            {
                Close( reader );
                Close( cmd );
            }

            return personContactPoints;
        }

        /// <summary>
        /// Returns a newly created "CELL" ContactPoint 
        /// initialized with a passed in PhoneNumber.
        /// </summary>
        /// <param name="cellPhone"></param>
        /// <returns></returns>
        private ContactPoint CreateCellPhoneContactPoint( PhoneNumber cellPhone )
        {
            ContactPoint cp = new ContactPoint();
            cp.PhoneNumber = cellPhone;
            cp.TypeOfContactPoint = TypeOfContactPoint.NewMobileContactPointType();
            return cp;
        }

        /// <summary>
        /// Returns a newly created "CELL" ContactPoint 
        /// initialized with a passed in PhoneNumber and CellPhoneConsent.
        /// </summary>
        /// <param name="cellPhone"></param>
        /// <param name ="cellPhoneConsent"></param>
        /// <returns></returns>
        private ContactPoint CreateCellPhoneContactPoint(PhoneNumber cellPhone, CellPhoneConsent cellPhoneConsent)
        {
            var cp = new ContactPoint
            {
                PhoneNumber = cellPhone,
                CellPhoneConsent = cellPhoneConsent,
                TypeOfContactPoint = TypeOfContactPoint.NewMobileContactPointType()
            };
            return cp;
        }

        /// <summary>
        /// CountryWith - retrieve an instance of Country for the specified Country Code
        /// </summary>
        /// <param name="facilityNumber"></param>
        /// <param name="code">a Country code</param>
        /// <returns>a Country instance</returns>
        public Country CountryWith( long facilityNumber, string code )
        {
            Country selectedCountry = null;
            if ( null == code )
            {
                throw new ArgumentNullException( "Argument, code, should not be null." );
            }
            code = code.Trim();
            InitFacility( facilityNumber );
            try
            {
                ArrayList countries = ( ArrayList )AllCountries( facilityNumber );
                foreach ( Country country in countries )
                {
                    if ( country.Code.Equals( code ) )
                    {
                        selectedCountry = country;
                        break;
                    }
                }

                if ( selectedCountry == null )
                {
                    WithStoredProcName = SP_SELECTCOUNTRYWITH;
                    selectedCountry = CodeWith<Country>( facilityNumber, code );
                    //Facility facility = new FacilityBroker().FacilityWith( facilityNumber );
                    //selectedCountry = CountryWith( code, facility );

                    if ( !selectedCountry.IsValid )
                    {
                        selectedCountry = new UnknownCountry( string.Empty, code );
                    }
                }
            }
            catch ( Exception e )
            {
                Console.Error.WriteLine( e );
                throw BrokerExceptionFactory.BrokerExceptionFrom( "AddressPBARBroker(Country) failed to initialize", e, c_log );
            }

            return selectedCountry;
        }

        /// <summary>
        /// CountryWith - overload that takes a facility object will read directly from the database
        /// </summary>
        /// <param name="code"></param>
        /// <param name="facility"></param>
        /// <returns></returns>

        public Country CountryWith( string code, Facility facility )
        {
            Country selectedCountry = null;
            if ( null == code )
            {
                throw new ArgumentNullException( "Argument, code, should not be null." );
            }
            code = code.Trim();

            iDB2Command cmd = null;
            SafeReader reader = null;
            string country = string.Empty;

            try
            {
                cmd = CommandFor(
                    string.Format( "CALL {0} ({1})",
                    SP_SELECTCOUNTRY,
                    PARAM_COUNTRYCD ),
                    CommandType.Text,
                    facility );

                cmd.Parameters[PARAM_COUNTRYCD].Value = code;

                reader = ExecuteReader( cmd );

                while ( reader.Read() )
                {
                    country = reader.GetString( COL_DESCRIPTION );
                }

                if ( string.IsNullOrEmpty( country ) )
                {
                    selectedCountry = new Country( 0, string.Empty, code );
                }
                else
                {
                    selectedCountry = new Country();
                    selectedCountry.Code = code;
                    selectedCountry.Description = country;
                }
            }
            catch ( Exception e )
            {
                Console.Error.WriteLine( e );
                throw BrokerExceptionFactory.BrokerExceptionFrom( "AddressPBARBroker(Country) failed to initialize", e, c_log );
            }
            finally
            {
                base.Close( reader );
                base.Close( cmd );
            }

            return selectedCountry;
        }

        /// <summary>
        /// CountyWith - return the county for the specified facility and county code
        /// </summary>
        /// <param name="facilityNumber"></param>
        /// <param name="stateCode"></param>
        /// <param name="code"></param>
        /// <returns></returns>

        public County CountyWith( long facilityNumber, string stateCode , string code )
        {
            Guard.ThrowIfArgumentIsNull(code, "code");
            Guard.ThrowIfArgumentIsNull(stateCode, "stateCode");
            code = code.Trim();
            
            InitFacility( facilityNumber );
            
            County selectedCounty;   
            
            try
            {
                var counties = AllCountiesFor(facilityNumber);

                selectedCounty = (from County county in counties
                                  where county.Code.Equals(code) && county.StateCode.Equals(stateCode)
                                  select county ).FirstOrDefault();

            }

            catch ( Exception e )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( "CountyBroker failed to initialize", e, c_log );
            }

            return selectedCounty;
        }

        /// <summary>
        /// EmployerContactPointFrom - create an EmployerContactPoint instance from the result set
        /// </summary>
        /// <param name="reader">a reader</param>
        /// <param name="facilityID"></param>
        /// <returns>an EmployerContactPoint instance</returns>
        private EmployerContactPoint EmployerContactPointFrom( SafeReader reader, long facilityID )
        {

            Address address = AddressFrom( reader, facilityID );
            PhoneNumber pn = PhoneNumberFromEmployer( reader );
            EmailAddress em = EmailAddressFrom( reader );
            TypeOfContactPoint cptype = TypeOfContactPointFrom( reader );
            EmployerContactPoint ecp = new EmployerContactPoint( address, pn, em, cptype );

            return ecp;
        }

        /// <summary>
        /// Saves Employer address to the PADATA.NCEMADP
        /// </summary>
        /// <param name="employer"></param>
        /// <param name="facilityCode"> A string, facility code</param>
        public void SaveEmployerAddress( Employer employer, string facilityCode )
        {
            iDB2Command cmd = null;
            try
            {

                string phoneNumber = string.Empty;
                Address address = null;
                string stateCode = string.Empty;

                IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
                Facility facility = facilityBroker.FacilityWith( facilityCode );

                c_log.InfoFormat( "Inserting EmployerAddress Name:{0} FUUN:{1} Code:{2}",
                    employer.Name, facility.FollowupUnit.Oid.ToString(), employer.EmployerCode );

                string cmdText = String.Format(
                    "CALL {0}({1},{2},{3},{4},{5},{6},{7},{8},{9},{10})",
                    SP_SAVE_EMPLOYER_ADDRESS,
                    PARAM_FOLLOWUP_UNIT_ID,
                    PARAM_EMPLOYER_CODE,
                    PARAM_EMPLOYER_ADDRESS,
                    PARAM_EMPLOYER_CITY,
                    PARAM_EMPLOYER_STATE,
                    PARAM_EMPLOYER_ZIP,
                    PARAM_EMPLOYER_PHONE,
                    PARAM_ADD_DATE,
                    PARAM_LAST_MAINT_DATE,
                    PARAM_EMPLOYER_NAME );

                cmd = CommandFor( cmdText, CommandType.Text, facility );

                if ( employer.PartyContactPoint.Address != null )
                {
                    address = employer.PartyContactPoint.Address;

                    int packedDate = DateTimeUtilities.PackedDateFromDate( DateTime.Now );
                    if ( employer.PartyContactPoint.PhoneNumber.ToString() != string.Empty )
                    {
                        phoneNumber = employer.PartyContactPoint.PhoneNumber.ToString();
                    }
                    if ( address.State != null )
                    {
                        stateCode = address.State.Code;
                    }
                    int FUUNLength = facility.FollowupUnit.Oid.ToString().Trim().Length;
                    if ( FUUNLength == FUUNPBARLength )
                    {
                        cmd.Parameters[PARAM_FOLLOWUP_UNIT_ID].Value = facility.FollowupUnit.Oid.ToString().Trim();
                    }
                    else
                        if ( FUUNLength < FUUNPBARLength )
                        {

                            string paddedFUUN = facility.FollowupUnit.Oid.ToString().Trim().PadLeft( FUUNPBARLength, '0' );
                            cmd.Parameters[PARAM_FOLLOWUP_UNIT_ID].Value = paddedFUUN;
                        }
                        else
                            if ( FUUNLength > FUUNPBARLength )
                            {
                                cmd.Parameters[PARAM_FOLLOWUP_UNIT_ID].Value = facility.FollowupUnit.Oid.ToString().Trim().Substring( 0, 3 );
                            }
                    cmd.Parameters[PARAM_EMPLOYER_CODE].Value = Convert.ToUInt32( employer.EmployerCode );
                    cmd.Parameters[PARAM_EMPLOYER_ADDRESS].Value = address.Address1;
                    cmd.Parameters[PARAM_EMPLOYER_CITY].Value = address.City;
                    cmd.Parameters[PARAM_EMPLOYER_STATE].Value = stateCode;
                    cmd.Parameters[PARAM_EMPLOYER_ZIP].Value = address.ZipCode.PostalCode;
                    cmd.Parameters[PARAM_EMPLOYER_PHONE].Value = phoneNumber;
                    cmd.Parameters[PARAM_ADD_DATE].Value = packedDate;
                    cmd.Parameters[PARAM_LAST_MAINT_DATE].Value = packedDate;
                    cmd.Parameters[PARAM_EMPLOYER_NAME].Value = employer.Name;

                    cmd.ExecuteNonQuery();
                }
            }
            catch ( Exception ex )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( "Unhandled Exception", ex, c_log );
            }
            finally
            {
                Close( cmd );
            }
        }

        /// <summary>
        /// Saves Employer address to the PADATA.NCEMADP
        /// </summary>
        /// <param name="employer"></param>
        /// <param name="facilityCode"> A string, facility code</param>
        public void SaveNewEmployerAddressForApproval( Employer employer, string facilityCode )
        {
            iDB2Command cmd = null;
            try
            {

                string phoneNumber = string.Empty;
                Address address = null;
                string stateCode = string.Empty;

                IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
                Facility facility = facilityBroker.FacilityWith( facilityCode );

                c_log.InfoFormat( "Inserting EmployerAddress Name:{0} FUUN:{1} Code:{2}",
                    employer.Name, facility.FollowupUnit.Oid.ToString(), employer.EmployerCode );

                string cmdText = String.Format(
                    "CALL {0}({1},{2},{3},{4},{5},{6},{7},{8},{9},{10})",
                    SP_SAVE_NEW_EMPLOYER_ADDRESS,
                    PARAM_FOLLOWUP_UNIT_ID,
                    PARAM_EMPLOYER_CODE,
                    PARAM_EMPLOYER_ADDRESS,
                    PARAM_EMPLOYER_CITY,
                    PARAM_EMPLOYER_STATE,
                    PARAM_EMPLOYER_ZIP,
                    PARAM_EMPLOYER_PHONE,
                    PARAM_ADD_DATE,
                    PARAM_LAST_MAINT_DATE,
                    PARAM_EMPLOYER_NAME );

                cmd = CommandFor( cmdText, CommandType.Text, facility );

                if ( employer.PartyContactPoint.Address != null )
                {
                    address = employer.PartyContactPoint.Address;

                    int packedDate = DateTimeUtilities.PackedDateFromDate( DateTime.Now );
                    if ( employer.PartyContactPoint.PhoneNumber.ToString() != string.Empty )
                    {
                        phoneNumber = employer.PartyContactPoint.PhoneNumber.ToString();
                    }
                    if ( address.State != null )
                    {
                        stateCode = address.State.Code;
                    }
                    int FUUNLength = facility.FollowupUnit.Oid.ToString().Trim().Length;
                    if ( FUUNLength == FUUNPBARLength )
                    {
                        cmd.Parameters[PARAM_FOLLOWUP_UNIT_ID].Value = facility.FollowupUnit.Oid.ToString().Trim();
                    }
                    else
                        if ( FUUNLength < FUUNPBARLength )
                        {

                            string paddedFUUN = facility.FollowupUnit.Oid.ToString().Trim().PadLeft( FUUNPBARLength, '0' );
                            cmd.Parameters[PARAM_FOLLOWUP_UNIT_ID].Value = paddedFUUN;
                        }
                        else
                            if ( FUUNLength > FUUNPBARLength )
                            {
                                cmd.Parameters[PARAM_FOLLOWUP_UNIT_ID].Value = facility.FollowupUnit.Oid.ToString().Trim().Substring( 0, 3 );
                            }
                    cmd.Parameters[PARAM_EMPLOYER_CODE].Value = Convert.ToUInt32( employer.EmployerCode );
                    cmd.Parameters[PARAM_EMPLOYER_ADDRESS].Value = address.Address1;
                    cmd.Parameters[PARAM_EMPLOYER_CITY].Value = address.City;
                    cmd.Parameters[PARAM_EMPLOYER_STATE].Value = stateCode;
                    cmd.Parameters[PARAM_EMPLOYER_ZIP].Value = address.ZipCode.PostalCode;
                    cmd.Parameters[PARAM_EMPLOYER_PHONE].Value = phoneNumber;
                    cmd.Parameters[PARAM_ADD_DATE].Value = packedDate;
                    cmd.Parameters[PARAM_LAST_MAINT_DATE].Value = packedDate;
                    cmd.Parameters[PARAM_EMPLOYER_NAME].Value = employer.Name;

                    cmd.ExecuteNonQuery();
                }
            }
            catch ( Exception ex )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( "Unhandled Exception", ex, c_log );
            }
            finally
            {
                Close( cmd );
            }
        }

        public void DeleteEmployerAddressForApproval( Employer employer, string facilityHspCode )
        {
            IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            Facility facility = facilityBroker.FacilityWith( facilityHspCode );

            InitFacility( facility.Code );

            iDB2Command cmd = null;
            SafeReader reader = null;

            try
            {
                cmd = CommandFor(
                    String.Format( "CALL {0}({1})",
                                  NEW_DELETEEMPLOYERADDRESSFORAPPROVAL,
                                  NEW_PARAM_P_ID ),
                    CommandType.Text,
                    Facility );

                cmd.Parameters[NEW_PARAM_P_ID].Value = employer.EmployerCode;

                reader = ExecuteReader( cmd );
            }
            catch ( Exception ex )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( "Unhandled Exception", ex, c_log );
            }
            finally
            {
                Close( reader );
                Close( cmd );
            }
        }

        /// <summary>
        /// StateWith - overload that takes a facility object will read directly from the database
        /// </summary>
        /// <param name="code"></param>
        /// <param name="facility"></param>
        /// <returns></returns>

        public State StateWith( string code, Facility facility )
        {
            Guard.ThrowIfArgumentIsNull(code, "code");            

            code = code.Trim();

            SafeReader reader = null;

            var stateNumber = string.Empty;
            var description = string.Empty;

            var parameters = new[] { PARAM_STATECD ,PARAM_FACILITY};

            var cmdTextForCallingSelectStateWith = new Db2StoredProcedureCallBuilder(parameters, SP_SELECTSTATEWITH).Build();

            var command = CommandFor(cmdTextForCallingSelectStateWith, CommandType.Text, facility);

            command.Parameters[PARAM_STATECD].Value = code;
            command.Parameters[PARAM_FACILITY].Value = facility.Oid;
            State selectedState;

            try
            {
                reader = ExecuteReader(command);

                while ( reader.Read() )
                {
                    description = reader.GetString(COL_DESCRIPTION);
                    stateNumber = reader.GetString(COL_STATENUMBER);
                }

                selectedState = new State(0, PersistentModel.NEW_VERSION, description, code, stateNumber);
            }

            catch ( Exception exception)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom("AddressPBARBroker(State) failed to initialize", exception, c_log);
            }

            finally
            {
                Close( reader );
                Close(command);
            }

            return selectedState;
        }

        /// <summary>
        /// StateWith - return an instance of State for the specified code
        /// </summary>
        /// <param name="code">a State code</param>
        /// <returns>a State instance</returns>
        public State StateWith(long facilityID, string code)
        {
            State selectedState = null;
            if (code == null)
            {
                throw new ArgumentNullException("code cannot be null or empty");
            }
            code = code.Trim().ToUpper();
            this.InitFacility(facilityID);

            try
            {
                ICollection allStates = this.AllStates(facilityID);
                foreach (State allstate in allStates)
                {
                    if (allstate.Code.Equals(code))
                    {
                        selectedState = allstate;
                        break;
                    }
                }

                if (selectedState == null)
                {
                    ArrayList oList = new ArrayList();
                    iDB2Command cmd = null;
                    StringBuilder sb = new StringBuilder();
                    iDB2DataReader read = null;
                    var stateNumber = string.Empty;
                    var description = string.Empty;
                    IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
                    Facility facility = facilityBroker.FacilityWith(facilityID);

                    sb.Append("CALL " + SP_SELECTSTATEWITH + "(");
                    if (facilityID != 0)
                    {
                        sb.Append(PARAM_STATECD);
                        sb.Append("," + PARAM_FACILITY);
                    }
                    sb.Append(")");
                    cmd = this.CommandFor(sb.ToString(), CommandType.Text, facility);
                    if (facilityID != 0)
                        cmd.Parameters[PARAM_FACILITYID].Value = facilityID;
                    cmd.Parameters[PARAM_STATECD].Value = code;

                    read = cmd.ExecuteReader();
                    while (read.Read())
                    {
                        State state = new State();
                        state.Code = read.GetString(1).TrimEnd();
                        state.Description = read.GetString(2).TrimEnd();
                        state.StateNumber = read.GetString(3).TrimEnd();
                        oList.Add(state);
                    }
                    selectedState = new State(0, PersistentModel.NEW_VERSION, description, code, stateNumber);
                }
            }
            catch (Exception e)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom("AddressPBARBroker(State) failed to initialize", e, c_log);
            }
            return selectedState;
        }
        
        public IList<County> GetCountiesForAState(string stateCode, long facilityNumber)
        {
            Guard.ThrowIfArgumentIsNull(stateCode, "stateCode");

            var counties = AllCountiesFor(facilityNumber);

            var countiesForAState = (from County county in counties
                                     where county.StateCode.Equals(stateCode)
                                     select county).ToList();

            return countiesForAState;
        }

        #endregion

        #region Properties
        #endregion

        #region Private and Protected Methods

        protected override void InitProcNames()
        {
            AllStoredProcName = string.Empty;
            WithStoredProcName = string.Empty;
        }

        /// <summary>
        /// AddressFrom - construct an Address from the result set
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="facilityID"></param>
        /// <returns></returns>

        private Address AddressWithStreet2From( SafeReader reader, long facilityID )
        {
            Address aAddress = null;

            // if there is no data return a null

            string address1 = reader.GetString(COL_ADDRESS1EXTENDED).Trim();
            if (string.IsNullOrEmpty(address1))
            {
                address1 = reader.GetString(COL_ADDRESS1).Trim();
            }

            address1 = StringFilter.
                RemoveFirstCharNonLetterNonNumberAndRestNonLetterNumberSpaceHyphenPeriodAndForwardSlash( address1 );

            string address2 = reader.GetString(COL_ADDRESS2EXTENDED).Trim();
            if (string.IsNullOrEmpty(address2))
            {

                address2 = reader.GetString(COL_ADDRESS2).Trim();
            }

            address2 = StringFilter.
                RemoveFirstCharNonLetterNonNumberAndRestNonLetterNumberSpaceHyphenPeriodAndForwardSlash( address2 );

            string city = reader.GetString( COL_CITY ).Trim();
            city = StringFilter.RemoveFirstCharNonLetterAndRestNonLetterSpaceHyphenAndPeriod( city );

            string postalCode = reader.GetString( COL_POSTALCODE ).Trim();
            postalCode = StringFilter.RemoveAllNonLetterNumberSpaceAndHyphen( postalCode );

            Country country = null;
            County county = null;
            State state = null;
            string stateCode = null;
            if ( !reader.IsDBNull( COL_STATECODE ) )
            {
                stateCode = reader.GetString( COL_STATECODE ).Trim();
                state = StateWith(facilityID, stateCode);
            }

            country = null;
            if ( !reader.IsDBNull( COL_COUNTRYCODE ) )
            {
                string countryCode = reader.GetString( COL_COUNTRYCODE ).Trim();
                country = CountryWith( facilityID, countryCode );
            }

            county = null;
            if ( !reader.IsDBNull( COL_COUNTYCODE ) &&
                 !reader.IsDBNull( COL_COUNTYDESCRIPTION ) )
            {
                string countyCode = reader.GetString( COL_COUNTYCODE ).Trim();
                string countyDesc = reader.GetString( COL_COUNTYDESCRIPTION ).Trim();
                county = new County( countyCode, countyDesc );

                if ( !string.IsNullOrEmpty( stateCode ) && !string.IsNullOrEmpty( countyCode ) )
                {
                    county = CountyWith( facilityID, stateCode, countyCode );
                }
            }

            string fipsCountyCode = string.Empty;
            string typeOfContactPoint = reader.GetString(COL_CONTACTPOINTDESCRIPTION);

            if (typeOfContactPoint == TypeOfContactPoint.PHYSICAL_CONTACTPOINT_DESCRIPTION)
            {
                if (!reader.IsDBNull(COL_FIPSCOUNTYCODE))
                {
                    fipsCountyCode = reader.GetString(COL_FIPSCOUNTYCODE).Trim();
					
                    if (!String.IsNullOrEmpty(fipsCountyCode) && (!String.IsNullOrEmpty(stateCode)) &&
                        fipsCountyCode.Length == Address.FIPSCOUNTYCODE_LENGTH)
                    {
                        string countyCode = fipsCountyCode.Substring(2, 3);
                        county = CountyWith(facilityID, stateCode, countyCode);
                    }
                }
            }

            aAddress = new Address( PersistentModel.NEW_OID,
                StringFilter.RemoveHL7Chars( address1 ),
                StringFilter.RemoveHL7Chars( address2 ),
                StringFilter.RemoveHL7Chars( city ),
                new ZipCode( postalCode ),
                state,
                country,
                county
                );
            
            return aAddress;
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

            // if there is no data return a null

            string address1 = reader.GetString(COL_ADDRESS1).Trim();
            address1 = StringFilter.
                RemoveFirstCharNonLetterNonNumberAndRestNonLetterNumberSpaceHyphenPeriodAndForwardSlash(address1);

            string address2 = reader.GetString(COL_ADDRESS2).Trim();
            address2 = StringFilter.
                RemoveFirstCharNonLetterNonNumberAndRestNonLetterNumberSpaceHyphenPeriodAndForwardSlash(address2);

            string city = reader.GetString(COL_CITY).Trim();
            city = StringFilter.RemoveFirstCharNonLetterAndRestNonLetterSpaceHyphenAndPeriod(city);

            string postalCode = reader.GetString(COL_POSTALCODE).Trim();
            postalCode = StringFilter.RemoveAllNonLetterNumberSpaceAndHyphen(postalCode);

            Country country = null;
            County county = null;
            State state = null;
            string stateCode = null;
            if (!reader.IsDBNull(COL_STATECODE))
            {
                stateCode = reader.GetString(COL_STATECODE).Trim();
                state = StateWith(facilityID,stateCode);
            }

            country = null;
            if (!reader.IsDBNull(COL_COUNTRYCODE))
            {
                string countryCode = reader.GetString(COL_COUNTRYCODE).Trim();
                country = CountryWith(facilityID, countryCode);
            }

            county = null;
            if (!reader.IsDBNull(COL_COUNTYCODE) &&
                 !reader.IsDBNull(COL_COUNTYDESCRIPTION))
            {
                string countyCode = reader.GetString(COL_COUNTYCODE).Trim();
                string countyDesc = reader.GetString(COL_COUNTYDESCRIPTION).Trim();
                county = new County(countyCode, countyDesc);

                if (!string.IsNullOrEmpty(stateCode) && !string.IsNullOrEmpty(countyCode))
                {
                    county = CountyWith(facilityID, stateCode, countyCode);
                }
            }

            string fipsCountyCode = string.Empty;
            string typeOfContactPoint = reader.GetString(COL_CONTACTPOINTDESCRIPTION);

            if (typeOfContactPoint == TypeOfContactPoint.PHYSICAL_CONTACTPOINT_DESCRIPTION)
            {
                if (!reader.IsDBNull(COL_FIPSCOUNTYCODE))
                {
                    fipsCountyCode = reader.GetString(COL_FIPSCOUNTYCODE).Trim();

                    if (!String.IsNullOrEmpty(fipsCountyCode) && (!String.IsNullOrEmpty(stateCode)) &&
                        fipsCountyCode.Length == Address.FIPSCOUNTYCODE_LENGTH)
                    {
                        string countyCode = fipsCountyCode.Substring(2, 3);
                        county = CountyWith(facilityID, stateCode, countyCode);
                    }
                }
            }

            aAddress = new Address(PersistentModel.NEW_OID,
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
        /// CountyFrom - return an instance of County based on the result set values
        /// </summary>
        /// <param name="reader">a SafeReader instance</param>
        /// <returns>a County instance</returns>
        private County CountyFrom( SafeReader reader )
        {
            County county = null;

            long countyID = reader.GetInt64( COL_COUNTYID );
            string countyCode = reader.GetString( COL_CODE );
            string description = reader.GetString( COL_DESCRIPTION );
            string stateCode = reader.GetString(COL_STATECODE);
            county = new County( countyID,
                PersistentModel.NEW_VERSION,
                description,
                countyCode, stateCode);

            return county;
        }
        private Country CountryFrom( SafeReader reader )
        {
            Country country = null;

            long countryID = reader.GetInt64( COL_COUNTRYID );
            string countryCode = reader.GetString( COL_CODE );
            string description = reader.GetString( COL_DESCRIPTION );

            country = new Country( countryID,
                PersistentModel.NEW_VERSION,
                description,
                countryCode );

            return country;
        }
		
        private State StateFrom(SafeReader reader)
        {
            State State = null;

            long stateID = reader.GetInt64(COL_STATEID);
            string stateCode = reader.GetString(COL_CODE);
            string description = reader.GetString(COL_DESCRIPTION);
            string stateNumber = reader.GetString(COL_STATENUMBER);
            State = new State(stateID,PersistentModel.NEW_VERSION,description,stateCode, stateNumber);

            return State;
        }
		
        /// <summary>
        /// EmailAddressFrom - create an EmailAddress instance from the result set
        /// </summary>
        /// <param name="reader">a reader</param>
        /// <returns>an EmailAddress instance</returns>
        private EmailAddress EmailAddressFrom( SafeReader reader )
        {
            string email = StringFilter.RemoveAllNonEmailSpecialCharacters( reader.GetString( COL_EMAILADDRESS ) );
            EmailAddress em = new EmailAddress( email );
            return em;
        }

        /// <summary>
        /// CellPhoneConsentFrom - create a CellPhoneConsent instance from the result set
        /// </summary>
        /// <param name="reader">a reader</param>
        /// <returns>a CellPhoneConsent instance</returns>
        private CellPhoneConsent ReadCellPhoneConsentFrom(SafeReader reader)
        {
            var cellPhoneConsent = new CellPhoneConsent();
            string guarantorCellPhoneConsent = reader.GetString(COL_CELL_PHONE_CONSENT);
            if (guarantorCellPhoneConsent != null && guarantorCellPhoneConsent.Trim().Length > 0)
            {
                var cellPhoneConsentBroker = BrokerFactory.BrokerOfType<ICellPhoneConsentBroker>();
                cellPhoneConsent = cellPhoneConsentBroker.ConsentWith( guarantorCellPhoneConsent );
                if (!cellPhoneConsent.IsValid)
                {
                    cellPhoneConsent = cellPhoneConsentBroker.ConsentWith( CellPhoneConsent.BLANK );
                }
            }
            return cellPhoneConsent;
        }


        /// <summary>
        /// PhoneNumberFromReader - create a PhoneNumber instance from the result set;
        /// The phone number and area code are returned as two separate columns.
        /// </summary>
        /// <param name="reader">a reader</param>
        /// <returns>a PhoneNumber instance</returns>
        private PhoneNumber PhoneNumberFrom( SafeReader reader )
        {
            string countryCode = reader.GetString( COL_PHONECOUNTRYCODE );
            string phoneNumber = string.Empty;
            string areaCode = string.Empty;

            areaCode = reader.GetString( COL_PHONEAREACODE );
            areaCode = areaCode.PadLeft(AREA_CODE_LENGTH, '0');
            phoneNumber = reader.GetString( COL_PHONENUMBER );
            phoneNumber = phoneNumber.PadLeft(MAX_LENGTH_OF_PHONE_NUMBER_NO_AREA_CODE , '0');
            PhoneNumber pn = new PhoneNumber( countryCode, areaCode, phoneNumber );
            return pn;
        }

        /// <summary>
        /// PhoneNumberFromReader - create a PhoneNumber instance from the result set
        /// for the patient's cell phone number. 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private PhoneNumber ReadCellPhoneNumberFrom( SafeReader reader )
        {
            string countryCode = reader.GetString( COL_PHONECOUNTRYCODE );
            string cellPhoneNumber = string.Empty;

            cellPhoneNumber = reader.GetString( COL_CELL_PHONE_NUMBER );

            PhoneNumber cpn = new PhoneNumber( cellPhoneNumber );
            cpn.CountryCode = countryCode;
            return cpn;
        }

        /// <summary>
        /// Private method to be used only for obtaining employer phone numbers.
        /// PhoneNumberFromEmployer - create a PhoneNumber instance from the result set.
        /// The phone number and area code are returned as one column.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private PhoneNumber PhoneNumberFromEmployer( SafeReader reader )
        {
            string countryCode = reader.GetString( COL_PHONECOUNTRYCODE );
            string phoneNumber = string.Empty;
            string areaCode = string.Empty;
            string areaCodeAndPhone = string.Empty;

            areaCodeAndPhone = reader.GetString( COL_AREACODE_PHONE );

            if ( areaCodeAndPhone.Length <= MAX_LENGTH_OF_PHONE_NUMBER_NO_AREA_CODE )
            {
                phoneNumber = areaCodeAndPhone;
            }
            else
            {
                if ( areaCodeAndPhone.Length == LENGTH_OF_FULL_PHONE_NUMBER )
                {
                    phoneNumber = areaCodeAndPhone.Substring(
                                       AREA_CODE_LENGTH,
                                       areaCodeAndPhone.Length - AREA_CODE_LENGTH );

                    areaCode = areaCodeAndPhone.Substring( 0, AREA_CODE_LENGTH );
                }
            }
            phoneNumber = phoneNumber.Trim().PadLeft( MAX_LENGTH_OF_PHONE_NUMBER_NO_AREA_CODE, PADDING_SYMBOL );
            areaCode = areaCode.Trim().PadLeft( AREA_CODE_LENGTH, PADDING_SYMBOL );

            PhoneNumber pn = new PhoneNumber( countryCode, areaCode, phoneNumber );
            return pn;
        }

        /// <summary>
        /// TypeOfContactPointFrom - crate a TypeOfContactPoint instance from the result set
        /// </summary>
        /// <param name="reader">a reader</param>
        /// <returns>a TypeOfContactPoint isntance</returns>
        private TypeOfContactPoint TypeOfContactPointFrom( SafeReader reader )
        {
            long cpID = reader.GetInt64( COL_TYPEOFCONTACTPOINTID );
            string cpDesc = reader.GetString( COL_CONTACTPOINTDESCRIPTION );
            TypeOfContactPoint cpType = new TypeOfContactPoint( cpID, cpDesc );

            return cpType;
        }

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public AddressPBARBroker()
        {
        }
        public AddressPBARBroker( string cxnString )
            : base( cxnString )
        {
        }
        public AddressPBARBroker( IDbTransaction txn )
            : base( txn )
        {
        }
        #endregion

        #region Data Elements

        private static readonly ILog c_log =
            LogManager.GetLogger( typeof( AddressPBARBroker ) );

        #endregion

        #region Constants

        private const int
                                AREA_CODE_LENGTH = 3,
                                LENGTH_OF_FULL_PHONE_NUMBER = 10,
                                MAX_LENGTH_OF_PHONE_NUMBER_NO_AREA_CODE = 7;

        private const char PADDING_SYMBOL = '0';

        private const string
                                SP_SAVE_NEW_EMPLOYER_ADDRESS = "SAVENEWEMPLOYERADDRESS",
                                SP_SAVE_EMPLOYER_ADDRESS = "SAVEEMPLOYERADDRESS",
                                SP_SELECTALLSTATES = "SELECTALLSTATES",
                                SP_SELECTSTATEWITH = "SELECTSTATEWITH",
                                SP_SELECTALLCOUNTRIES = "SELECTALLCOUNTRIESFOR",
                                SP_SELECTCOUNTRY = "SELECTCOUNTRYWITH",
                                SP_SELECTCOUNTRYWITH = "SELECTCOUNTRYWITHFACILITY",
                                SP_SELECTALLCOUNTIESFOR = "SELECTALLCOUNTIESFOR",
                                SP_SELECTCOUNTYWITH = "SELECTCOUNTYWITH",
                                SELECTCONTACTPOINTSFOREMPLOYER = "SELECTCONTACTPOINTSFOREMPLOYER",
                                SELECTCONTACTPOINTSFORGUARANTOR = "SELECTCONTACTPOINTSFORGUARANTOR",
                                SELECTCONTACTPOINTSFORPATIENT = "SELECTCONTACTPOINTSFORPATIENT", 
                                SP_SELECTCONTACTPOINTSFOREMPLOYERAPPROVAL = "SELECTCONTACTPOINTSFOREMPLOYERAPPROVAL";

        private const string NEW_DELETEEMPLOYERADDRESSFORAPPROVAL = "DELETEEMPLOYERADDRESSFORAPPROVAL";
        private const string NEW_PARAM_P_ID = "@P_ID";

        private const string PARAM_FOLLOWUP_UNIT_ID = "@P_FOLLOWUP_UNIT_ID",
                                PARAM_EMPLOYER_CODE = "@P_EMP_CODE",
                                PARAM_EMPLOYER_ADDRESS = "@P_EMPLOYER_ADDRESS",
                                PARAM_EMPLOYER_CITY = "@P_EMPLOYER_CITY",
                                PARAM_EMPLOYER_STATE = "@P_EMPLOYER_STATE",
                                PARAM_EMPLOYER_ZIP = "@P_EMPLOYER_ZIP",
                                PARAM_EMPLOYER_PHONE = "@P_EMPLOYER_PHONE",
                                PARAM_ADD_DATE = "@P_ADD_DATE",
                                PARAM_LAST_MAINT_DATE = "@P_LAST_MAINT_DATE",
                                PARAM_EMPLOYER_NAME = "@P_EMPLOYER_NAME",
                                PARAM_MRN = "@P_MRN",
                                PARAM_ACCOUNT_NUMBER = "@P_ACCOUNT_NUMBER",
                                PARAM_STATECD = "@P_STATECD",
                                PARAM_COUNTRYCD = "@P_COUNTRYCD",
                                PARAM_FACILITY = "@P_FACILITYID",
                                PARAM_COUNTYCD = "@P_CODE";


        private const string COL_ADDRESS1 = "ADDRESS1",
            COL_ADDRESS2 = "ADDRESS2",
            COL_ADDRESS1EXTENDED = "ADDRESS1EXTENDED",
            COL_ADDRESS2EXTENDED = "ADDRESS2EXTENDED",
            COL_CITY = "CITY",
            COL_POSTALCODE = "POSTALCODE",
            COL_STATECODE = "STATECODE",
            COL_COUNTRYCODE = "COUNTRYCODE",
            COL_CODE = "CODE",
            COL_COUNTYID = "OID",
            COL_COUNTRYID = "OID",
            COL_STATEID = "OID",
            COL_DESCRIPTION = "DESCRIPTION",
            COL_COUNTYCODE = "COUNTYCODE",
            COL_FIPSCOUNTYCODE = "FIPSCountyCode",
            COL_COUNTYDESCRIPTION = "COUNTYDESCRIPTION",
            COL_TYPEOFCONTACTPOINTID = "TYPEOFCONTACTPOINTID",
            COL_CONTACTPOINTDESCRIPTION = "CONTACTPOINTDESCRIPTION",
            COL_STATENUMBER = "STATENUMBER",
            COL_AREACODE_PHONE = "AREACODEANDPHONE",
            COL_PHONEAREACODE = "AreaCode",
            COL_PHONENUMBER = "PhoneNumber",
            COL_PHONECOUNTRYCODE = "PhoneCountryCode",
            COL_EMAILADDRESS = "EmailAddress",
            COL_CELL_PHONE_NUMBER = "CellPhoneNumber",
            COL_CELL_PHONE_CONSENT = "CellPhoneConsent";

        private const int FUUNPBARLength = 3;

        #endregion
    }
}

