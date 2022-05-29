using System;
using System.Collections;
using System.Configuration;
using System.Runtime.InteropServices;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Domain.UCCRegistration;

namespace PatientAccess.Persistence
{
    [Serializable, Guid( "FDDD502A-453C-4651-AABE-F787252F17E2" )]
    public class InsuranceInsertStrategy : SqlBuilderStrategy
    {
        #region Construction and Finalization
        public InsuranceInsertStrategy( int typeOfCoverage )
        {
            coverageType = typeOfCoverage;
            InitializeColumnValues();
        }
        #endregion

        #region Methods
        public override void UpdateColumnValuesUsing( Account account )
        {
            try
            {
                if( account != null )
                {
                    if( account.Insurance != null )
                    {
                        Coverage coverage = null;
                        // this method only works for primary and secondary coverages
                        if( coverageType == 1 || coverageType == 2 )
                        {
                            coverage = account.Insurance.CoverageFor( coverageType );
                        }
                        else if( coverageType == 0 )
                        {
                            coverage = account.DeletedSecondaryCoverage;
                        }

                        // so if there is a deleted 2ndary coverage we need to handle
                        if( coverage != null )
                        {
                            //Set Transaction APACFL Add/Change flag
                            if( coverage.IsNew )
                            {
                                AddChangeFlag = ADD_FLAG;
                            }
                            else
                            {
                                AddChangeFlag = CHANGE_FLAG;
                            }

                            ReadAccountInfo( account );
                            ReadCommonCoverageValues( coverage, account.Activity );
                            ReadAuthorizationDetails( coverage );
                            ReadTypeSpecificCoverageValues( coverage );
                            ReadFinancialCounselingDetails( account );
                            ReadCompleteCoverageDetails( coverage, account );

                            if( coverage.Insured.Relationships != null )
                            {
                                RelationshipType rt = coverage.Insured.RelationshipWith( coverage.Insured );
                                if( rt != null )
                                {
                                    PatientInsuredRelationShip = rt.Code;
                                }

                            }
                            if( account.Activity != null )
                            {
                                User appUser = account.Activity.AppUser;
                                if( appUser != null )
                                {
                                    if( appUser.WorkstationID != null )
                                    {
                                        WorkStationId = appUser.WorkstationID;
                                    }
                                }
                            }
                            if( coverageType == 1 )
                            {
                                ReadPrimaryCoverageValues( coverage, account );
                            }
                            else if( coverageType == 2 )
                            {
                                ReadSecondaryCoverageValues( coverage );
                            }
                            else if( coverageType == 0 ) // deleted secondary coverage
                            {
                                PlanId = string.Empty;   // OTD 36379
                                ReadSecondaryCoverageValues( coverage );
                                ClearValuesForDeletedCoverage();
                            }
                        }
                    }

                    // TLG 04/05/2007 - we only populate MSP fields on the Primary insurance transaction,
                    // regardless of which insurance qualifies for MSP
                    if (account.MedicareSecondaryPayor != null && account.MedicareSecondaryPayor.MSPVersion > 0 && coverageType == 1)	
                    {
                        // SR 464 - Since all the MSP values are copied forward during ‘Activate Pre-Reg account’ activity, 
                        // if the user were to select a patient type-9 after opening the account, the MSP button would become
                        // disabled although the copied over MSP fields are retained in the background. At this point, if the 
                        // user were to change the patient type to something other than PT-9, the MSP values are retrieved 
                        // back to enable the MSP button again and display the copied over values. The MSP values
                        // retained that way should be cleared if the patient type is 9-NONPATIENT during account save.

                        if ( account.KindOfVisit != null && 
                             account.KindOfVisit.Code.Equals( VisitType.NON_PATIENT ) )
                        {
                            account.MedicareSecondaryPayor = new MedicareSecondaryPayor();
                        }

                        string strMSP2Start = ConfigurationManager.AppSettings["MSP2_START_DATE"];
                        DateTime msp2StartDate = DateTime.Parse( strMSP2Start );

                        if( account.AdmitDate > msp2StartDate
                            && account.MedicareSecondaryPayor.MSPVersion != 1 )
                        {
                            ReadMSP2( account );
                        }
                        else
                        {
                            ReadMedicareSecondaryPayor( account.MedicareSecondaryPayor );
                        }

                    }
                    
                }
            }
            catch( BrokerException be )
            {
                throw be;
            }
        }

        public override void InitializeColumnValues()
        {
            string strMinDate = ConvertMinDateToStringInyyyyMMddFormat();

            insuranceDetailsOrderedList.Add( APIDWS, string.Empty );
            insuranceDetailsOrderedList.Add( APIDID, "ID" );
            if( coverageType == CoverageOrder.PRIMARY_OID )
            {
                insuranceDetailsOrderedList.Add( APRR_, PRIMARY_INSURANCE_RECORD_NUMBER );
            }
            else
            {
                insuranceDetailsOrderedList.Add( APRR_, SECONDARY_INSURANCE_RECORD_NUMBER );
            }
            insuranceDetailsOrderedList.Add( APSEC2, string.Empty );
            insuranceDetailsOrderedList.Add( APHSP_, 0 );
            insuranceDetailsOrderedList.Add( APACCT, 0 );
            insuranceDetailsOrderedList.Add( APSEQ_, 0 );
            insuranceDetailsOrderedList.Add( APGAR_, 0 );
            insuranceDetailsOrderedList.Add( APPTY, string.Empty );
            insuranceDetailsOrderedList.Add( APPLAN, 0 );
            insuranceDetailsOrderedList.Add( APBCFL, string.Empty );
            insuranceDetailsOrderedList.Add( APCSPL, string.Empty );
            insuranceDetailsOrderedList.Add( APINM, string.Empty );
            insuranceDetailsOrderedList.Add( APGNM, string.Empty );
            insuranceDetailsOrderedList.Add( APIAD1, string.Empty );
            insuranceDetailsOrderedList.Add( APIAD2, string.Empty );
            insuranceDetailsOrderedList.Add( APIAD3, string.Empty );
            insuranceDetailsOrderedList.Add( APVST, string.Empty );
            insuranceDetailsOrderedList.Add( APPROR, string.Empty );
            insuranceDetailsOrderedList.Add( APLAST, string.Empty );
            insuranceDetailsOrderedList.Add( APVBV, string.Empty );
            insuranceDetailsOrderedList.Add( APSDAT, 0 );
            insuranceDetailsOrderedList.Add( AP1FLG, string.Empty );
            insuranceDetailsOrderedList.Add( AP1DED, 0 );
            insuranceDetailsOrderedList.Add( AP1_, 0 );
            insuranceDetailsOrderedList.Add( AP1PCT, 0 );
            insuranceDetailsOrderedList.Add( AP1DYS, 0 );
            insuranceDetailsOrderedList.Add( AP2FLG, string.Empty );
            insuranceDetailsOrderedList.Add( AP2DED, 0 );
            insuranceDetailsOrderedList.Add( AP2PCT, 0 );
            insuranceDetailsOrderedList.Add( AP2MX_, 0 );
            insuranceDetailsOrderedList.Add( AP2DR_, 0 );
            insuranceDetailsOrderedList.Add( AP2DID, 0 );
            insuranceDetailsOrderedList.Add( AP2IR_, 0 );
            insuranceDetailsOrderedList.Add( AP2IRD, 0 );
            insuranceDetailsOrderedList.Add( AP3FLG, string.Empty );
            insuranceDetailsOrderedList.Add( AP3DED, 0 );
            insuranceDetailsOrderedList.Add( AP3PCT, 0 );
            insuranceDetailsOrderedList.Add( AP3_, 0 );
            insuranceDetailsOrderedList.Add( AP4FLG, string.Empty );
            insuranceDetailsOrderedList.Add( AP4DED, 0 );
            insuranceDetailsOrderedList.Add( AP4PCT, 0 );
            insuranceDetailsOrderedList.Add( AP4_, 0 );
            insuranceDetailsOrderedList.Add( AP4MR_, 0 );
            insuranceDetailsOrderedList.Add( AP4MRD, 0 );
            insuranceDetailsOrderedList.Add( AP4MI_, 0 );
            insuranceDetailsOrderedList.Add( AP4MID, 0 );
            insuranceDetailsOrderedList.Add( AP5FLG, string.Empty );
            insuranceDetailsOrderedList.Add( AP5DED, 0 );
            insuranceDetailsOrderedList.Add( AP5PCT, 0 );
            insuranceDetailsOrderedList.Add( AP5_, 0 );
            insuranceDetailsOrderedList.Add( AP5MR_, 0 );
            insuranceDetailsOrderedList.Add( AP5MRD, 0 );
            insuranceDetailsOrderedList.Add( AP5MI_, 0 );
            insuranceDetailsOrderedList.Add( AP5MID, 0 );
            insuranceDetailsOrderedList.Add( AP6DBM, string.Empty );
            insuranceDetailsOrderedList.Add( AP6XMM, string.Empty );
            insuranceDetailsOrderedList.Add( AP6RBM, string.Empty );
            insuranceDetailsOrderedList.Add( AP6DM, string.Empty );
            insuranceDetailsOrderedList.Add( APCOB, string.Empty );
            insuranceDetailsOrderedList.Add( APEX01, 0 );
            insuranceDetailsOrderedList.Add( APEX02, 0 );
            insuranceDetailsOrderedList.Add( APEX03, 0 );
            insuranceDetailsOrderedList.Add( APEX04, 0 );
            insuranceDetailsOrderedList.Add( APEX05, 0 );
            insuranceDetailsOrderedList.Add( APEX06, 0 );
            insuranceDetailsOrderedList.Add( APEX07, 0 );
            insuranceDetailsOrderedList.Add( APEX08, 0 );
            insuranceDetailsOrderedList.Add( APEX09, 0 );
            insuranceDetailsOrderedList.Add( APEX10, 0 );
            insuranceDetailsOrderedList.Add( APEX11, 0 );
            insuranceDetailsOrderedList.Add( APEX12, 0 );
            insuranceDetailsOrderedList.Add( APEX13, 0 );
            insuranceDetailsOrderedList.Add( APEX14, 0 );
            insuranceDetailsOrderedList.Add( APEX15, 0 );
            insuranceDetailsOrderedList.Add( APEX16, 0 );
            insuranceDetailsOrderedList.Add( APEX17, 0 );
            insuranceDetailsOrderedList.Add( APEX18, 0 );
            insuranceDetailsOrderedList.Add( APEX19, 0 );
            insuranceDetailsOrderedList.Add( APEX20, 0 );
            insuranceDetailsOrderedList.Add( APFDAY, 0 );
            insuranceDetailsOrderedList.Add( APCDAY, 0 );
            insuranceDetailsOrderedList.Add( APCDOL, 0 );
            insuranceDetailsOrderedList.Add( APLDAY, 0 );
            insuranceDetailsOrderedList.Add( APLDOL, 0 );
            insuranceDetailsOrderedList.Add( APBDPT, 0 );
            insuranceDetailsOrderedList.Add( APBD_, 0 );
            insuranceDetailsOrderedList.Add( APPMET, 0 );
            insuranceDetailsOrderedList.Add( APPM_, 0 );
            insuranceDetailsOrderedList.Add( APPMPR, 0 );
            insuranceDetailsOrderedList.Add( APMCRE, string.Empty );
            insuranceDetailsOrderedList.Add( APMAID, string.Empty );
            insuranceDetailsOrderedList.Add( APMDED, 0 );
            insuranceDetailsOrderedList.Add( APLML, 0 );
            insuranceDetailsOrderedList.Add( APLMD, 0 );
            insuranceDetailsOrderedList.Add( APLUL_, 0 );
            insuranceDetailsOrderedList.Add( APACFL, string.Empty );
            insuranceDetailsOrderedList.Add( APTTME, 0 );
            insuranceDetailsOrderedList.Add( APINLG, LOG_NUMBER );
            insuranceDetailsOrderedList.Add( APBYPS, string.Empty );
            insuranceDetailsOrderedList.Add( APSWPY, 0 );
            insuranceDetailsOrderedList.Add( APABCF, string.Empty );
            insuranceDetailsOrderedList.Add( APRICF, string.Empty );
            insuranceDetailsOrderedList.Add( APSLNM, string.Empty );
            insuranceDetailsOrderedList.Add( APSFNM, string.Empty );
            insuranceDetailsOrderedList.Add( APSSEX, string.Empty );
            insuranceDetailsOrderedList.Add( APSRCD, string.Empty );
            insuranceDetailsOrderedList.Add( APIID_, string.Empty );
            insuranceDetailsOrderedList.Add( APGRPN, string.Empty );
            insuranceDetailsOrderedList.Add( APESCD, string.Empty );
            insuranceDetailsOrderedList.Add( APSBEN, string.Empty );
            insuranceDetailsOrderedList.Add( APEEID, string.Empty );
            insuranceDetailsOrderedList.Add( APSBEL, string.Empty );
            insuranceDetailsOrderedList.Add( APJADR, string.Empty );
            insuranceDetailsOrderedList.Add( APJCIT, string.Empty );
            insuranceDetailsOrderedList.Add( APJSTE, string.Empty );
            insuranceDetailsOrderedList.Add( APJZIP, 0 );
            insuranceDetailsOrderedList.Add( APJZP4, 0 );
            insuranceDetailsOrderedList.Add( APJACD, 0 );
            insuranceDetailsOrderedList.Add( APJPH_, 0 );
            insuranceDetailsOrderedList.Add( APNEIC, string.Empty );
            insuranceDetailsOrderedList.Add( APDOV, 0 );
            insuranceDetailsOrderedList.Add( APELGS, string.Empty );
            insuranceDetailsOrderedList.Add( APTDOC, 0 );
            insuranceDetailsOrderedList.Add( APELDT, 0 );
            insuranceDetailsOrderedList.Add( APSCHB, string.Empty );
            insuranceDetailsOrderedList.Add( APVC01, string.Empty );
            insuranceDetailsOrderedList.Add( APVC02, string.Empty );
            insuranceDetailsOrderedList.Add( APVC03, string.Empty );
            insuranceDetailsOrderedList.Add( APVC04, string.Empty );
            insuranceDetailsOrderedList.Add( APVA01, 0 );
            insuranceDetailsOrderedList.Add( APVA02, 0 );
            insuranceDetailsOrderedList.Add( APVA03, 0 );
            insuranceDetailsOrderedList.Add( APVA04, 0 );
            insuranceDetailsOrderedList.Add( APAGNY, 0 );
            insuranceDetailsOrderedList.Add( APCHAM, 0 );
            insuranceDetailsOrderedList.Add( APCHAP, 0 );
            insuranceDetailsOrderedList.Add( APMSPC, string.Empty );
            insuranceDetailsOrderedList.Add( APDBEN, 0 );
            insuranceDetailsOrderedList.Add( APDURR, 0 );
            insuranceDetailsOrderedList.Add( APACRE, 0 );
            insuranceDetailsOrderedList.Add( APDGPS, 0 );
            insuranceDetailsOrderedList.Add( APNEPS, string.Empty );
            insuranceDetailsOrderedList.Add( APNEPI, 0 );
            insuranceDetailsOrderedList.Add( APITYP, string.Empty );
            insuranceDetailsOrderedList.Add( APRIPL, 0 );
            insuranceDetailsOrderedList.Add( APCBFL, string.Empty );
            insuranceDetailsOrderedList.Add( APINEX, string.Empty );
            insuranceDetailsOrderedList.Add( APSURP, 0 );
            insuranceDetailsOrderedList.Add( APSURA, 0 );
            insuranceDetailsOrderedList.Add( APINS_, string.Empty );
            insuranceDetailsOrderedList.Add( APCNFC, string.Empty );
            insuranceDetailsOrderedList.Add( APPFCC, string.Empty );
            insuranceDetailsOrderedList.Add( APRICO, string.Empty );
            insuranceDetailsOrderedList.Add( APENM1, string.Empty );
            insuranceDetailsOrderedList.Add( APELO1, string.Empty );
            insuranceDetailsOrderedList.Add( APEDC1, string.Empty );
            insuranceDetailsOrderedList.Add( APESC1, string.Empty );
            insuranceDetailsOrderedList.Add( APEID1, "00000000000" );
            insuranceDetailsOrderedList.Add( APEA01, string.Empty );
            insuranceDetailsOrderedList.Add( APEZ01, 0 );
            insuranceDetailsOrderedList.Add( APENM2, string.Empty );
            insuranceDetailsOrderedList.Add( APELO2, string.Empty );
            insuranceDetailsOrderedList.Add( APEDC2, string.Empty );
            insuranceDetailsOrderedList.Add( APESC2, string.Empty );
            insuranceDetailsOrderedList.Add( APEID2, "00000000000" );
            insuranceDetailsOrderedList.Add( APEA02, string.Empty );
            insuranceDetailsOrderedList.Add( APEZ02, 0 );
            insuranceDetailsOrderedList.Add( APDEDU, string.Empty );
            insuranceDetailsOrderedList.Add( APBLDE, string.Empty );
            insuranceDetailsOrderedList.Add( APINDY, string.Empty );
            insuranceDetailsOrderedList.Add( APLIFD, string.Empty );
            insuranceDetailsOrderedList.Add( APNCVD, string.Empty );
            insuranceDetailsOrderedList.Add( APTDAT, 0 );
            insuranceDetailsOrderedList.Add( APCLRK, string.Empty );
            insuranceDetailsOrderedList.Add( APZDTE, string.Empty );
            insuranceDetailsOrderedList.Add( APZTME, string.Empty );
            insuranceDetailsOrderedList.Add( APPNID, string.Empty );
            insuranceDetailsOrderedList.Add( APP_NM, string.Empty );
            insuranceDetailsOrderedList.Add( APCBGD, 0 );
            insuranceDetailsOrderedList.Add( APPLAD, 0 );
            insuranceDetailsOrderedList.Add( APSVTP, string.Empty );
            insuranceDetailsOrderedList.Add( APSTCP, 0 );
            insuranceDetailsOrderedList.Add( APSTAM, 0 );
            insuranceDetailsOrderedList.Add( APST_L, 0 );
            insuranceDetailsOrderedList.Add( APSTPA, 0 );
            insuranceDetailsOrderedList.Add( APAUTF, string.Empty );
            insuranceDetailsOrderedList.Add( APAUWV, string.Empty );
            insuranceDetailsOrderedList.Add( APAUDY, 0 );
            insuranceDetailsOrderedList.Add( AP2OPF, string.Empty );
            insuranceDetailsOrderedList.Add( AP2CMM, string.Empty );
            insuranceDetailsOrderedList.Add( APCOPF, string.Empty );
            insuranceDetailsOrderedList.Add( APDEDF, string.Empty );
            insuranceDetailsOrderedList.Add( APBLNM, string.Empty );
            insuranceDetailsOrderedList.Add( APOBCF, string.Empty );
            insuranceDetailsOrderedList.Add( APOBCP, 0 );
            insuranceDetailsOrderedList.Add( APOBAM, 0 );
            insuranceDetailsOrderedList.Add( APOB_L, 0 );
            insuranceDetailsOrderedList.Add( APOBPA, 0 );
            insuranceDetailsOrderedList.Add( APNBCF, string.Empty );
            insuranceDetailsOrderedList.Add( APNBCP, 0 );
            insuranceDetailsOrderedList.Add( APNBAM, 0 );
            insuranceDetailsOrderedList.Add( APNB_L, 0 );
            insuranceDetailsOrderedList.Add( APNBPA, 0 );
            insuranceDetailsOrderedList.Add( APCDCF, string.Empty );
            insuranceDetailsOrderedList.Add( APCDCP, 0 );
            insuranceDetailsOrderedList.Add( APCDAM, 0 );
            insuranceDetailsOrderedList.Add( APCD_L, 0 );
            insuranceDetailsOrderedList.Add( APCDPA, 0 );
            insuranceDetailsOrderedList.Add( APDCCF, string.Empty );
            insuranceDetailsOrderedList.Add( APDCCP, 0 );
            insuranceDetailsOrderedList.Add( APDCAM, 0 );
            insuranceDetailsOrderedList.Add( APDC_L, 0 );
            insuranceDetailsOrderedList.Add( APDCPA, 0 );
            insuranceDetailsOrderedList.Add( APDCAL, 0 );
            insuranceDetailsOrderedList.Add( APDADM, 0 );
            insuranceDetailsOrderedList.Add( APDDMF, string.Empty );
            insuranceDetailsOrderedList.Add( APLFMX, 0 );
            insuranceDetailsOrderedList.Add( APMXAD, 0 );
            insuranceDetailsOrderedList.Add( APCOBF, string.Empty );
            insuranceDetailsOrderedList.Add( APCOMM, string.Empty );
            insuranceDetailsOrderedList.Add( APIFRF, string.Empty );
            insuranceDetailsOrderedList.Add( APSUAC, 0 );
            insuranceDetailsOrderedList.Add( APSUPH, 0 );
            insuranceDetailsOrderedList.Add( APVFFG, string.Empty );
            insuranceDetailsOrderedList.Add( APVFBY, string.Empty );
            insuranceDetailsOrderedList.Add( APVFDT, 0 );
            insuranceDetailsOrderedList.Add( APAMDY, 0 );
            insuranceDetailsOrderedList.Add( APNCDY, 0 );
            insuranceDetailsOrderedList.Add( APELOS, 0 );
            insuranceDetailsOrderedList.Add( APRCDE, 0 );
            insuranceDetailsOrderedList.Add( APRSLC, 0 );
            insuranceDetailsOrderedList.Add( APUROP, string.Empty );
            insuranceDetailsOrderedList.Add( APMSPF, string.Empty );
            insuranceDetailsOrderedList.Add( APWVCP, string.Empty );
            insuranceDetailsOrderedList.Add( APGCD1, 0 );
            insuranceDetailsOrderedList.Add( APGCD2, 0 );
            insuranceDetailsOrderedList.Add( APCONM, string.Empty );
            insuranceDetailsOrderedList.Add( APEZP1, string.Empty );
            insuranceDetailsOrderedList.Add( APEZ41, "0000" );
            insuranceDetailsOrderedList.Add( APEZP2, string.Empty );
            insuranceDetailsOrderedList.Add( APEZ42, "0000" );
            insuranceDetailsOrderedList.Add( APJZPA, string.Empty );
            insuranceDetailsOrderedList.Add( APJZ4A, "0000" );
            insuranceDetailsOrderedList.Add( APOID, string.Empty );
            insuranceDetailsOrderedList.Add( APPRE, string.Empty );
            insuranceDetailsOrderedList.Add( APMR_, string.Empty );
            insuranceDetailsOrderedList.Add( APOACT, string.Empty );
            insuranceDetailsOrderedList.Add( APEVC_, string.Empty );
            insuranceDetailsOrderedList.Add( APSMI, string.Empty );
            insuranceDetailsOrderedList.Add( APSBID, string.Empty );
            insuranceDetailsOrderedList.Add( APSCUN, string.Empty );
            insuranceDetailsOrderedList.Add( APSBTH, string.Empty );
            insuranceDetailsOrderedList.Add( APOILN, string.Empty );
            insuranceDetailsOrderedList.Add( APOIFN, string.Empty );
            insuranceDetailsOrderedList.Add( APOIMI, string.Empty );
            insuranceDetailsOrderedList.Add( APOISX, string.Empty );
            insuranceDetailsOrderedList.Add( APOIBD, string.Empty );
            insuranceDetailsOrderedList.Add( APOIID, string.Empty );
            insuranceDetailsOrderedList.Add( APOIAD, string.Empty );
            insuranceDetailsOrderedList.Add( APOICT, string.Empty );
            insuranceDetailsOrderedList.Add( APOIST, string.Empty );
            insuranceDetailsOrderedList.Add( APOICN, string.Empty );
            insuranceDetailsOrderedList.Add( APOIZP, string.Empty );
            insuranceDetailsOrderedList.Add( APOIZ4, "0000" );
            insuranceDetailsOrderedList.Add( APICPH, string.Empty );
            insuranceDetailsOrderedList.Add( APMCDT, string.Empty );
            insuranceDetailsOrderedList.Add( APDAMT, 0 );
            insuranceDetailsOrderedList.Add( APCPAY, 0 );
            insuranceDetailsOrderedList.Add( APCINS, 0 );
            insuranceDetailsOrderedList.Add( APAMCL, 0 );
            insuranceDetailsOrderedList.Add( APPYCL, 0 );
            insuranceDetailsOrderedList.Add( APINCL, 0 );
            insuranceDetailsOrderedList.Add( APPVCL, 0 );
            insuranceDetailsOrderedList.Add( APUCBL, 0 );
            insuranceDetailsOrderedList.Add( APEDBL, 0 );
            insuranceDetailsOrderedList.Add( APUFPT, 0 );
            insuranceDetailsOrderedList.Add( APEFPT, 0 );
            insuranceDetailsOrderedList.Add( APNUM1, 0 );
            insuranceDetailsOrderedList.Add( APNUM2, 0 );
            insuranceDetailsOrderedList.Add( APNUM3, 0 );
            insuranceDetailsOrderedList.Add( APMNPY, 0 );
            insuranceDetailsOrderedList.Add( APCOPY, 0 );
            insuranceDetailsOrderedList.Add( APCIPY, 0 );
            insuranceDetailsOrderedList.Add( APUCPY, 0 );
            insuranceDetailsOrderedList.Add( APEDPY, 0 );
            insuranceDetailsOrderedList.Add( APPDUE, string.Empty );
            insuranceDetailsOrderedList.Add( APCRPT, string.Empty );
            insuranceDetailsOrderedList.Add( APCRSC, string.Empty );
            insuranceDetailsOrderedList.Add( APCR01, string.Empty );
            insuranceDetailsOrderedList.Add( APCR02, string.Empty );
            insuranceDetailsOrderedList.Add( APCR03, string.Empty );
            insuranceDetailsOrderedList.Add( APCR04, string.Empty );
            insuranceDetailsOrderedList.Add( APCR05, string.Empty );
            insuranceDetailsOrderedList.Add( APCR06, string.Empty );
            insuranceDetailsOrderedList.Add( APCR07, string.Empty );
            insuranceDetailsOrderedList.Add( APCR08, string.Empty );
            insuranceDetailsOrderedList.Add( APCR09, string.Empty );
            insuranceDetailsOrderedList.Add( APCR10, string.Empty );
            insuranceDetailsOrderedList.Add( APCR11, string.Empty );
            insuranceDetailsOrderedList.Add( APCR12, string.Empty );
            insuranceDetailsOrderedList.Add( APHIC_, string.Empty );
            insuranceDetailsOrderedList.Add( APITR_, string.Empty );
            insuranceDetailsOrderedList.Add( APAUTH_, string.Empty );
            insuranceDetailsOrderedList.Add( APAUTFG, string.Empty );
            insuranceDetailsOrderedList.Add( APAUCP, string.Empty );
            insuranceDetailsOrderedList.Add( APAUPH, 0 );
            insuranceDetailsOrderedList.Add( APAUEX, string.Empty );
            insuranceDetailsOrderedList.Add( APADNM, string.Empty );
            insuranceDetailsOrderedList.Add( APEMSU, string.Empty );
            insuranceDetailsOrderedList.Add( APATNM, string.Empty );
            insuranceDetailsOrderedList.Add( APATST, string.Empty );
            insuranceDetailsOrderedList.Add( APATCT, string.Empty );
            insuranceDetailsOrderedList.Add( APATSA, string.Empty );
            insuranceDetailsOrderedList.Add( APATZ5, 0 );
            insuranceDetailsOrderedList.Add( APATZ4, "0000" );
            insuranceDetailsOrderedList.Add( APATCC, string.Empty );
            insuranceDetailsOrderedList.Add( APATPH, 0 );
            insuranceDetailsOrderedList.Add( APAGNM, string.Empty );
            insuranceDetailsOrderedList.Add( APAGPH, 0 );
            insuranceDetailsOrderedList.Add( APAGST, string.Empty );
            insuranceDetailsOrderedList.Add( APAGCT, string.Empty );
            insuranceDetailsOrderedList.Add( APAGSA, string.Empty );
            insuranceDetailsOrderedList.Add( APAGZ5, 0 );
            insuranceDetailsOrderedList.Add( APAGZ4, "0000" );
            insuranceDetailsOrderedList.Add( APAGCC, string.Empty );
            insuranceDetailsOrderedList.Add( AP_BL, string.Empty );
            insuranceDetailsOrderedList.Add( AP_GP, string.Empty );
            insuranceDetailsOrderedList.Add( AP_VA, string.Empty );
            insuranceDetailsOrderedList.Add( AP_WA, string.Empty );
            insuranceDetailsOrderedList.Add( AP_NA, string.Empty );
            insuranceDetailsOrderedList.Add( AP_OP, string.Empty );
            insuranceDetailsOrderedList.Add( AP_AG, string.Empty );
            insuranceDetailsOrderedList.Add( AP_EMPL, string.Empty );
            insuranceDetailsOrderedList.Add( AP_SEMPL, string.Empty );
            insuranceDetailsOrderedList.Add( AP_AGGHP, string.Empty );
            insuranceDetailsOrderedList.Add( AP_AGGHP20, string.Empty );
            insuranceDetailsOrderedList.Add( AP_DI, string.Empty );
            insuranceDetailsOrderedList.Add( AP_OEMPL, string.Empty );
            insuranceDetailsOrderedList.Add( AP_DIGHP, string.Empty );
            insuranceDetailsOrderedList.Add( AP_DIGHP00, string.Empty );
            insuranceDetailsOrderedList.Add( AP_ES, string.Empty );
            insuranceDetailsOrderedList.Add( AP_ESGHP, string.Empty );
            insuranceDetailsOrderedList.Add( AP_ESKT, string.Empty );
            insuranceDetailsOrderedList.Add( AP_ESDT, string.Empty );
            insuranceDetailsOrderedList.Add( AP_ES30MO, string.Empty );
            insuranceDetailsOrderedList.Add( AP_ESMULTI, string.Empty );
            insuranceDetailsOrderedList.Add( AP_ESIA, string.Empty );
            insuranceDetailsOrderedList.Add( AP_ESOTHR, string.Empty );
            insuranceDetailsOrderedList.Add( AP_GHP, string.Empty );
            insuranceDetailsOrderedList.Add( AP_AUTO, string.Empty );
            insuranceDetailsOrderedList.Add( AP_INS_, 0 );
            insuranceDetailsOrderedList.Add( AP_PNID1, string.Empty );
            insuranceDetailsOrderedList.Add( AP_PNID2, string.Empty );
            insuranceDetailsOrderedList.Add( AP_USER, string.Empty );
            insuranceDetailsOrderedList.Add( AP_BLDT, DateTime.MinValue.ToString("yyyy-MM-dd") );
            insuranceDetailsOrderedList.Add( AP_AIDT, DateTime.MinValue.ToString( "yyyy-MM-dd" ) );
            insuranceDetailsOrderedList.Add( AP_ESKTDT, DateTime.MinValue.ToString( "yyyy-MM-dd" ) );
            insuranceDetailsOrderedList.Add( AP_ESDTDT, DateTime.MinValue.ToString( "yyyy-MM-dd" ) );
            insuranceDetailsOrderedList.Add( AP_ESSDTDT, DateTime.MinValue.ToString( "yyyy-MM-dd" ) );
            insuranceDetailsOrderedList.Add( AP_EMPLRDT, DateTime.MinValue.ToString( "yyyy-MM-dd" ) );
            insuranceDetailsOrderedList.Add( AP_SEMPRDT, DateTime.MinValue.ToString( "yyyy-MM-dd" ) );
            insuranceDetailsOrderedList.Add( AP_DTTM, CURRENT_TIMESTAMP );
            insuranceDetailsOrderedList.Add( AP_SENM, string.Empty );
            insuranceDetailsOrderedList.Add( AP_SEADR, string.Empty );
            insuranceDetailsOrderedList.Add( AP_SECIT, string.Empty );
            insuranceDetailsOrderedList.Add( AP_SESTE, string.Empty );
            insuranceDetailsOrderedList.Add( AP_SEZIP, string.Empty );
            insuranceDetailsOrderedList.Add( AP_SEZP4, string.Empty );
            insuranceDetailsOrderedList.Add( AP_BLTDY, string.Empty );
            insuranceDetailsOrderedList.Add( AP_FENM, string.Empty );
            insuranceDetailsOrderedList.Add( AP_FEADR, string.Empty );
            insuranceDetailsOrderedList.Add( AP_FECIT, string.Empty );
            insuranceDetailsOrderedList.Add( AP_FESTE, string.Empty );
            insuranceDetailsOrderedList.Add( AP_FEZIP, string.Empty );
            insuranceDetailsOrderedList.Add( AP_FEZP4, string.Empty );
            insuranceDetailsOrderedList.Add( AP_EMPLN, string.Empty );
            insuranceDetailsOrderedList.Add( AP_SEMPLN, string.Empty );
            insuranceDetailsOrderedList.Add( APWSIR, WORKSTATION_ID );
            insuranceDetailsOrderedList.Add( APSECR, string.Empty );
            insuranceDetailsOrderedList.Add( APORR1, string.Empty );
            insuranceDetailsOrderedList.Add( APORR2, string.Empty );
            insuranceDetailsOrderedList.Add( APORR3, string.Empty );
            insuranceDetailsOrderedList.Add( AP_NFALT, string.Empty );
            insuranceDetailsOrderedList.Add( AP_LIABL, string.Empty );
            insuranceDetailsOrderedList.Add( AP_AGGHPF, string.Empty );
            insuranceDetailsOrderedList.Add( AP_AGHP20P, string.Empty );
            insuranceDetailsOrderedList.Add( AP_DIGHPF, string.Empty );
            insuranceDetailsOrderedList.Add( AP_DGHP00P, string.Empty );
            insuranceDetailsOrderedList.Add( AP_DGHP00F, string.Empty );
            insuranceDetailsOrderedList.Add( AP_FORMVER, string.Empty );

            insuranceDetailsOrderedList.Add( APPACCT, 0 );  // next two fields are used for Pre-MSE and Post-MSE
            insuranceDetailsOrderedList.Add( APACTF, string.Empty );

            //Benefits Validation
            insuranceDetailsOrderedList.Add( APCVID, 0 );
            insuranceDetailsOrderedList.Add( APACRFN, string.Empty );
            insuranceDetailsOrderedList.Add( APACRLN, string.Empty );
            insuranceDetailsOrderedList.Add( APSAUTH, string.Empty );
            insuranceDetailsOrderedList.Add( APEFDA, strMinDate );
            insuranceDetailsOrderedList.Add( APEXDA, strMinDate );
            insuranceDetailsOrderedList.Add( APASTS, string.Empty );
            insuranceDetailsOrderedList.Add( APARMKS, string.Empty );

            insuranceDetailsOrderedList.Add( AP1ELDT, strMinDate );
            insuranceDetailsOrderedList.Add( AP1PTMC, string.Empty );
            insuranceDetailsOrderedList.Add( AP1PTOI, string.Empty );
            insuranceDetailsOrderedList.Add( AP1CPAY, -1 );
            insuranceDetailsOrderedList.Add( AP1EVCN, string.Empty );
            insuranceDetailsOrderedList.Add( AP1INFF, InformationReceivedSource.BLANK_VERIFICATION_OID );
            insuranceDetailsOrderedList.Add( AP1REMK, string.Empty );

            insuranceDetailsOrderedList.Add( AP2NMNT, string.Empty );
            insuranceDetailsOrderedList.Add( AP2CLMN, string.Empty );
            insuranceDetailsOrderedList.Add( AP2ADVR, string.Empty );
            insuranceDetailsOrderedList.Add( AP2INPH, string.Empty );
            insuranceDetailsOrderedList.Add( AP2EMPM, string.Empty );
            insuranceDetailsOrderedList.Add( AP2INFF, InformationReceivedSource.BLANK_VERIFICATION_OID );
            insuranceDetailsOrderedList.Add( AP2REMK, string.Empty );

            insuranceDetailsOrderedList.Add( AP3PACV, string.Empty );
            insuranceDetailsOrderedList.Add( AP3PAED, strMinDate );
            insuranceDetailsOrderedList.Add( AP3PBCV, string.Empty );
            insuranceDetailsOrderedList.Add( AP3PBED, strMinDate );
            insuranceDetailsOrderedList.Add( AP3MHCV, string.Empty );
            insuranceDetailsOrderedList.Add( AP3MDSC, string.Empty );
            insuranceDetailsOrderedList.Add( AP3DTLB, strMinDate );
            insuranceDetailsOrderedList.Add( AP3RMHD, -1 );
            insuranceDetailsOrderedList.Add( AP3RCID, 0 );
            insuranceDetailsOrderedList.Add( AP3RLRD, -1 );
            insuranceDetailsOrderedList.Add( AP3RSNF, -1 );
            insuranceDetailsOrderedList.Add( AP3RSNC, -1 );
            insuranceDetailsOrderedList.Add( AP3PTHS, string.Empty );
            insuranceDetailsOrderedList.Add( AP3BNNV, string.Empty );
            insuranceDetailsOrderedList.Add( AP3INFF, InformationReceivedSource.BLANK_VERIFICATION_OID );
            insuranceDetailsOrderedList.Add( AP3REMK, string.Empty );

            insuranceDetailsOrderedList.Add( AP4INFF, InformationReceivedSource.BLANK_VERIFICATION_OID );
            insuranceDetailsOrderedList.Add( AP4ELPH, string.Empty );
            insuranceDetailsOrderedList.Add( AP4INRP, string.Empty );
            insuranceDetailsOrderedList.Add( AP4TYCV, string.Empty );
            insuranceDetailsOrderedList.Add( AP4EFDI, strMinDate );
            insuranceDetailsOrderedList.Add( AP4TMDI, strMinDate );
            insuranceDetailsOrderedList.Add( AP4DEDA, -1 );
            insuranceDetailsOrderedList.Add( AP4DEMT, string.Empty );
            insuranceDetailsOrderedList.Add( AP4DEAM, -1 );
            insuranceDetailsOrderedList.Add( AP4COIN, -1 );
            insuranceDetailsOrderedList.Add( AP4OTPK, -1 );
            insuranceDetailsOrderedList.Add( AP4OTPM, string.Empty );
            insuranceDetailsOrderedList.Add( AP4OTPA, -1 );
            insuranceDetailsOrderedList.Add( AP4PROT, -1 );
            insuranceDetailsOrderedList.Add( AP4CPAY, -1 );
            insuranceDetailsOrderedList.Add( AP4REMK, string.Empty );

            insuranceDetailsOrderedList.Add( AP5PTMD, string.Empty );
            insuranceDetailsOrderedList.Add( AP5INIF, string.Empty );

            insuranceDetailsOrderedList.Add( AP6INFF, InformationReceivedSource.BLANK_VERIFICATION_OID );
            insuranceDetailsOrderedList.Add( AP6ELPH, string.Empty );
            insuranceDetailsOrderedList.Add( AP6INRP, string.Empty );
            insuranceDetailsOrderedList.Add( AP6EFDI, strMinDate );
            insuranceDetailsOrderedList.Add( AP6TMDI, strMinDate );
            insuranceDetailsOrderedList.Add( AP6SFPC, string.Empty );
            insuranceDetailsOrderedList.Add( AP6SACB, string.Empty );
            insuranceDetailsOrderedList.Add( AP6CAAV, string.Empty );
            insuranceDetailsOrderedList.Add( AP6COBF, string.Empty );
            insuranceDetailsOrderedList.Add( AP6RCOB, -1 );
            insuranceDetailsOrderedList.Add( AP6TPPR, -1 );
            insuranceDetailsOrderedList.Add( AP6NPPO, string.Empty );
            insuranceDetailsOrderedList.Add( AP6HCPR, string.Empty );
            insuranceDetailsOrderedList.Add( AP6ACNM, string.Empty );
            insuranceDetailsOrderedList.Add( AP6MPCV, string.Empty );
            insuranceDetailsOrderedList.Add( AP6REMK, string.Empty );

            insuranceDetailsOrderedList.Add( AP601CTID, 0 );
            insuranceDetailsOrderedList.Add( AP601DEDA, -1 );
            insuranceDetailsOrderedList.Add( AP601TMPR, string.Empty );
            insuranceDetailsOrderedList.Add( AP601DEMT, string.Empty );
            insuranceDetailsOrderedList.Add( AP601DEAM, -1 );
            insuranceDetailsOrderedList.Add( AP601COIN, -1 );
            insuranceDetailsOrderedList.Add( AP601OTPK, -1 );
            insuranceDetailsOrderedList.Add( AP601OTPM, string.Empty );
            insuranceDetailsOrderedList.Add( AP601OTPA, -1 );
            insuranceDetailsOrderedList.Add( AP601PROT, -1 );
            insuranceDetailsOrderedList.Add( AP601CPAY, -1 );
            insuranceDetailsOrderedList.Add( AP601WCPY, string.Empty );
            insuranceDetailsOrderedList.Add( AP601NMVS, -1 );
            insuranceDetailsOrderedList.Add( AP601LFMB, -1 );
            insuranceDetailsOrderedList.Add( AP601LFRM, -1 );
            insuranceDetailsOrderedList.Add( AP601LFVM, string.Empty );
            insuranceDetailsOrderedList.Add( AP601MXBN, -1 );
            insuranceDetailsOrderedList.Add( AP601RMBN, -1 );
            insuranceDetailsOrderedList.Add( AP601RMMT, string.Empty );

            insuranceDetailsOrderedList.Add( AP602CTID, 0 );
            insuranceDetailsOrderedList.Add( AP602DEDA, -1 );
            insuranceDetailsOrderedList.Add( AP602TMPR, string.Empty );
            insuranceDetailsOrderedList.Add( AP602DEMT, string.Empty );
            insuranceDetailsOrderedList.Add( AP602DEAM, -1 );
            insuranceDetailsOrderedList.Add( AP602COIN, -1 );
            insuranceDetailsOrderedList.Add( AP602OTPK, -1 );
            insuranceDetailsOrderedList.Add( AP602OTPM, string.Empty );
            insuranceDetailsOrderedList.Add( AP602OTPA, -1 );
            insuranceDetailsOrderedList.Add( AP602PROT, -1 );
            insuranceDetailsOrderedList.Add( AP602CPAY, -1 );
            insuranceDetailsOrderedList.Add( AP602WCPY, string.Empty );
            insuranceDetailsOrderedList.Add( AP602NMVS, -1 );
            insuranceDetailsOrderedList.Add( AP602LFMB, -1 );
            insuranceDetailsOrderedList.Add( AP602LFRM, -1 );
            insuranceDetailsOrderedList.Add( AP602LFVM, string.Empty );
            insuranceDetailsOrderedList.Add( AP602MXBN, -1 );
            insuranceDetailsOrderedList.Add( AP602RMBN, -1 );
            insuranceDetailsOrderedList.Add( AP602RMMT, string.Empty );

            insuranceDetailsOrderedList.Add( AP603CTID, 0 );
            insuranceDetailsOrderedList.Add( AP603DEDA, -1 );
            insuranceDetailsOrderedList.Add( AP603TMPR, string.Empty );
            insuranceDetailsOrderedList.Add( AP603DEMT, string.Empty );
            insuranceDetailsOrderedList.Add( AP603DEAM, -1 );
            insuranceDetailsOrderedList.Add( AP603COIN, -1 );
            insuranceDetailsOrderedList.Add( AP603OTPK, -1 );
            insuranceDetailsOrderedList.Add( AP603OTPM, string.Empty );
            insuranceDetailsOrderedList.Add( AP603OTPA, -1 );
            insuranceDetailsOrderedList.Add( AP603PROT, -1 );
            insuranceDetailsOrderedList.Add( AP603CPAY, -1 );
            insuranceDetailsOrderedList.Add( AP603WCPY, string.Empty );
            insuranceDetailsOrderedList.Add( AP603NMVS, -1 );
            insuranceDetailsOrderedList.Add( AP603LFMB, -1 );
            insuranceDetailsOrderedList.Add( AP603LFRM, -1 );
            insuranceDetailsOrderedList.Add( AP603LFVM, string.Empty );
            insuranceDetailsOrderedList.Add( AP603MXBN, -1 );
            insuranceDetailsOrderedList.Add( AP603RMBN, -1 );
            insuranceDetailsOrderedList.Add( AP603RMMT, string.Empty );

            insuranceDetailsOrderedList.Add( AP604CTID, 0 );
            insuranceDetailsOrderedList.Add( AP604DEDA, -1 );
            insuranceDetailsOrderedList.Add( AP604TMPR, string.Empty );
            insuranceDetailsOrderedList.Add( AP604DEMT, string.Empty );
            insuranceDetailsOrderedList.Add( AP604DEAM, -1 );
            insuranceDetailsOrderedList.Add( AP604COIN, -1 );
            insuranceDetailsOrderedList.Add( AP604OTPK, -1 );
            insuranceDetailsOrderedList.Add( AP604OTPM, string.Empty );
            insuranceDetailsOrderedList.Add( AP604OTPA, -1 );
            insuranceDetailsOrderedList.Add( AP604PROT, -1 );
            insuranceDetailsOrderedList.Add( AP604CPAY, -1 );
            insuranceDetailsOrderedList.Add( AP604WCPY, string.Empty );
            insuranceDetailsOrderedList.Add( AP604NMVS, -1 );
            insuranceDetailsOrderedList.Add( AP604LFMB, -1 );
            insuranceDetailsOrderedList.Add( AP604LFRM, -1 );
            insuranceDetailsOrderedList.Add( AP604LFVM, string.Empty );
            insuranceDetailsOrderedList.Add( AP604MXBN, -1 );
            insuranceDetailsOrderedList.Add( AP604RMBN, -1 );
            insuranceDetailsOrderedList.Add( AP604RMMT, string.Empty );

            insuranceDetailsOrderedList.Add( AP605CTID, 0 );
            insuranceDetailsOrderedList.Add( AP605DEDA, -1 );
            insuranceDetailsOrderedList.Add( AP605TMPR, string.Empty );
            insuranceDetailsOrderedList.Add( AP605DEMT, string.Empty );
            insuranceDetailsOrderedList.Add( AP605DEAM, -1 );
            insuranceDetailsOrderedList.Add( AP605COIN, -1 );
            insuranceDetailsOrderedList.Add( AP605OTPK, -1 );
            insuranceDetailsOrderedList.Add( AP605OTPM, string.Empty );
            insuranceDetailsOrderedList.Add( AP605OTPA, -1 );
            insuranceDetailsOrderedList.Add( AP605PROT, -1 );
            insuranceDetailsOrderedList.Add( AP605CPAY, -1 );
            insuranceDetailsOrderedList.Add( AP605WCPY, string.Empty );
            insuranceDetailsOrderedList.Add( AP605NMVS, -1 );
            insuranceDetailsOrderedList.Add( AP605LFMB, -1 );
            insuranceDetailsOrderedList.Add( AP605LFRM, -1 );
            insuranceDetailsOrderedList.Add( AP605LFVM, string.Empty );
            insuranceDetailsOrderedList.Add( AP605MXBN, -1 );
            insuranceDetailsOrderedList.Add( AP605RMBN, -1 );
            insuranceDetailsOrderedList.Add( AP605RMMT, string.Empty );

            insuranceDetailsOrderedList.Add( AP606CTID, 0 );
            insuranceDetailsOrderedList.Add( AP606DEDA, -1 );
            insuranceDetailsOrderedList.Add( AP606TMPR, string.Empty );
            insuranceDetailsOrderedList.Add( AP606DEMT, string.Empty );
            insuranceDetailsOrderedList.Add( AP606DEAM, -1 );
            insuranceDetailsOrderedList.Add( AP606COIN, -1 );
            insuranceDetailsOrderedList.Add( AP606OTPK, -1 );
            insuranceDetailsOrderedList.Add( AP606OTPM, string.Empty );
            insuranceDetailsOrderedList.Add( AP606OTPA, -1 );
            insuranceDetailsOrderedList.Add( AP606PROT, -1 );
            insuranceDetailsOrderedList.Add( AP606CPAY, -1 );
            insuranceDetailsOrderedList.Add( AP606WCPY, string.Empty );
            insuranceDetailsOrderedList.Add( AP606NMVS, -1 );
            insuranceDetailsOrderedList.Add( AP606LFMB, -1 );
            insuranceDetailsOrderedList.Add( AP606LFRM, -1 );
            insuranceDetailsOrderedList.Add( AP606LFVM, string.Empty );
            insuranceDetailsOrderedList.Add( AP606MXBN, -1 );
            insuranceDetailsOrderedList.Add( AP606RMBN, -1 );
            insuranceDetailsOrderedList.Add( AP606RMMT, string.Empty );

            insuranceDetailsOrderedList.Add( AP607CTID, 0 );
            insuranceDetailsOrderedList.Add( AP607DEDA, -1 );
            insuranceDetailsOrderedList.Add( AP607TMPR, string.Empty );
            insuranceDetailsOrderedList.Add( AP607DEMT, string.Empty );
            insuranceDetailsOrderedList.Add( AP607DEAM, -1 );
            insuranceDetailsOrderedList.Add( AP607COIN, -1 );
            insuranceDetailsOrderedList.Add( AP607OTPK, -1 );
            insuranceDetailsOrderedList.Add( AP607OTPM, string.Empty );
            insuranceDetailsOrderedList.Add( AP607OTPA, -1 );
            insuranceDetailsOrderedList.Add( AP607PROT, -1 );
            insuranceDetailsOrderedList.Add( AP607CPAY, -1 );
            insuranceDetailsOrderedList.Add( AP607WCPY, string.Empty );
            insuranceDetailsOrderedList.Add( AP607NMVS, -1 );
            insuranceDetailsOrderedList.Add( AP607LFMB, -1 );
            insuranceDetailsOrderedList.Add( AP607LFRM, -1 );
            insuranceDetailsOrderedList.Add( AP607LFVM, string.Empty );
            insuranceDetailsOrderedList.Add( AP607MXBN, -1 );
            insuranceDetailsOrderedList.Add( AP607RMBN, -1 );
            insuranceDetailsOrderedList.Add( AP607RMMT, string.Empty );

            insuranceDetailsOrderedList.Add( AP608CTID, 0 );
            insuranceDetailsOrderedList.Add( AP608DEDA, -1 );
            insuranceDetailsOrderedList.Add( AP608TMPR, string.Empty );
            insuranceDetailsOrderedList.Add( AP608DEMT, string.Empty );
            insuranceDetailsOrderedList.Add( AP608DEAM, -1 );
            insuranceDetailsOrderedList.Add( AP608COIN, -1 );
            insuranceDetailsOrderedList.Add( AP608OTPK, -1 );
            insuranceDetailsOrderedList.Add( AP608OTPM, string.Empty );
            insuranceDetailsOrderedList.Add( AP608OTPA, -1 );
            insuranceDetailsOrderedList.Add( AP608PROT, -1 );
            insuranceDetailsOrderedList.Add( AP608CPAY, -1 );
            insuranceDetailsOrderedList.Add( AP608WCPY, string.Empty );
            insuranceDetailsOrderedList.Add( AP608NMVS, -1 );
            insuranceDetailsOrderedList.Add( AP608LFMB, -1 );
            insuranceDetailsOrderedList.Add( AP608LFRM, -1 );
            insuranceDetailsOrderedList.Add( AP608LFVM, string.Empty );
            insuranceDetailsOrderedList.Add( AP608MXBN, -1 );
            insuranceDetailsOrderedList.Add( AP608RMBN, -1 );
            insuranceDetailsOrderedList.Add( AP608RMMT, string.Empty );

            insuranceDetailsOrderedList.Add( AP609CTID, 0 );
            insuranceDetailsOrderedList.Add( AP609DEDA, -1 );
            insuranceDetailsOrderedList.Add( AP609TMPR, string.Empty );
            insuranceDetailsOrderedList.Add( AP609DEMT, string.Empty );
            insuranceDetailsOrderedList.Add( AP609DEAM, -1 );
            insuranceDetailsOrderedList.Add( AP609COIN, -1 );
            insuranceDetailsOrderedList.Add( AP609OTPK, -1 );
            insuranceDetailsOrderedList.Add( AP609OTPM, string.Empty );
            insuranceDetailsOrderedList.Add( AP609OTPA, -1 );
            insuranceDetailsOrderedList.Add( AP609PROT, -1 );
            insuranceDetailsOrderedList.Add( AP609CPAY, -1 );
            insuranceDetailsOrderedList.Add( AP609WCPY, string.Empty );
            insuranceDetailsOrderedList.Add( AP609NMVS, -1 );
            insuranceDetailsOrderedList.Add( AP609LFMB, -1 );
            insuranceDetailsOrderedList.Add( AP609LFRM, -1 );
            insuranceDetailsOrderedList.Add( AP609LFVM, string.Empty );
            insuranceDetailsOrderedList.Add( AP609MXBN, -1 );
            insuranceDetailsOrderedList.Add( AP609RMBN, -1 );
            insuranceDetailsOrderedList.Add( AP609RMMT, string.Empty );

            insuranceDetailsOrderedList.Add( AP610CTID, 0 );
            insuranceDetailsOrderedList.Add( AP610DEDA, -1 );
            insuranceDetailsOrderedList.Add( AP610TMPR, string.Empty );
            insuranceDetailsOrderedList.Add( AP610DEMT, string.Empty );
            insuranceDetailsOrderedList.Add( AP610DEAM, -1 );
            insuranceDetailsOrderedList.Add( AP610COIN, -1 );
            insuranceDetailsOrderedList.Add( AP610OTPK, -1 );
            insuranceDetailsOrderedList.Add( AP610OTPM, string.Empty );
            insuranceDetailsOrderedList.Add( AP610OTPA, -1 );
            insuranceDetailsOrderedList.Add( AP610PROT, -1 );
            insuranceDetailsOrderedList.Add( AP610CPAY, -1 );
            insuranceDetailsOrderedList.Add( AP610WCPY, string.Empty );
            insuranceDetailsOrderedList.Add( AP610NMVS, -1 );
            insuranceDetailsOrderedList.Add( AP610LFMB, -1 );
            insuranceDetailsOrderedList.Add( AP610LFRM, -1 );
            insuranceDetailsOrderedList.Add( AP610LFVM, string.Empty );
            insuranceDetailsOrderedList.Add( AP610MXBN, -1 );
            insuranceDetailsOrderedList.Add( AP610RMBN, -1 );
            insuranceDetailsOrderedList.Add( AP610RMMT, string.Empty );

            insuranceDetailsOrderedList.Add( AP611CTID, 0 );
            insuranceDetailsOrderedList.Add( AP611DEDA, -1 );
            insuranceDetailsOrderedList.Add( AP611TMPR, string.Empty );
            insuranceDetailsOrderedList.Add( AP611DEMT, string.Empty );
            insuranceDetailsOrderedList.Add( AP611DEAM, -1 );
            insuranceDetailsOrderedList.Add( AP611COIN, -1 );
            insuranceDetailsOrderedList.Add( AP611OTPK, -1 );
            insuranceDetailsOrderedList.Add( AP611OTPM, string.Empty );
            insuranceDetailsOrderedList.Add( AP611OTPA, -1 );
            insuranceDetailsOrderedList.Add( AP611PROT, -1 );
            insuranceDetailsOrderedList.Add( AP611CPAY, -1 );
            insuranceDetailsOrderedList.Add( AP611WCPY, string.Empty );
            insuranceDetailsOrderedList.Add( AP611NMVS, -1 );
            insuranceDetailsOrderedList.Add( AP611LFMB, -1 );
            insuranceDetailsOrderedList.Add( AP611LFRM, -1 );
            insuranceDetailsOrderedList.Add( AP611LFVM, string.Empty );
            insuranceDetailsOrderedList.Add( AP611MXBN, -1 );
            insuranceDetailsOrderedList.Add( AP611RMBN, -1 );
            insuranceDetailsOrderedList.Add( AP611RMMT, string.Empty );

            insuranceDetailsOrderedList.Add( AP612CTID, 0 );
            insuranceDetailsOrderedList.Add( AP612DEDA, -1 );
            insuranceDetailsOrderedList.Add( AP612TMPR, string.Empty );
            insuranceDetailsOrderedList.Add( AP612DEMT, string.Empty );
            insuranceDetailsOrderedList.Add( AP612DEAM, -1 );
            insuranceDetailsOrderedList.Add( AP612COIN, -1 );
            insuranceDetailsOrderedList.Add( AP612OTPK, -1 );
            insuranceDetailsOrderedList.Add( AP612OTPM, string.Empty );
            insuranceDetailsOrderedList.Add( AP612OTPA, -1 );
            insuranceDetailsOrderedList.Add( AP612PROT, -1 );
            insuranceDetailsOrderedList.Add( AP612CPAY, -1 );
            insuranceDetailsOrderedList.Add( AP612WCPY, string.Empty );
            insuranceDetailsOrderedList.Add( AP612NMVS, -1 );
            insuranceDetailsOrderedList.Add( AP612LFMB, -1 );
            insuranceDetailsOrderedList.Add( AP612LFRM, -1 );
            insuranceDetailsOrderedList.Add( AP612LFVM, string.Empty );
            insuranceDetailsOrderedList.Add( AP612MXBN, -1 );
            insuranceDetailsOrderedList.Add( AP612RMBN, -1 );
            insuranceDetailsOrderedList.Add( AP612RMMT, string.Empty );

            insuranceDetailsOrderedList.Add( AP3RPAD, -1 );
            insuranceDetailsOrderedList.Add( AP3RPBD, -1 );
            insuranceDetailsOrderedList.Add(APDIALCTR, string.Empty);
            insuranceDetailsOrderedList.Add(APSGNRN, string.Empty);
            insuranceDetailsOrderedList.Add(APMBI, string.Empty);
            // Benefits Validation - End
            insuranceDetailsOrderedList.Add(APJADRE1, string.Empty);
            insuranceDetailsOrderedList.Add(APJADRE2, string.Empty);
        }

        public override ArrayList BuildSqlFrom( Account account,
            TransactionKeys transactionKeys )
        {
            UpdateColumnValuesUsing( account );
            AddColumnsAndValuesToSqlStatement(
                insuranceDetailsOrderedList,
                "HPADAPID" );
           
            SqlStatements.Add( SqlStatement );
            return SqlStatements;
        }


        #endregion

        #region Public Properties
        public int AccountNumber
        {
            get
            {
                return (int)insuranceDetailsOrderedList[APACCT];
            }
            set
            {
                insuranceDetailsOrderedList[APACCT] = value;

            }
        }
        public string UserSecurityCode
        {
            set
            {
                insuranceDetailsOrderedList[APSEC2] = value;
            }
        }
        public string OrignalTransactionId
        {
            set
            {
                insuranceDetailsOrderedList[APOID] = value;
            }
        }
        public string PreRegistrationFlag
        {
            get { return insuranceDetailsOrderedList[APPRE].ToString(); }

            set
            {
                insuranceDetailsOrderedList[APPRE] = value;
            }
        }
        public string LastTransactionInGroup
        {
            set
            {
                insuranceDetailsOrderedList[APLAST] = value;
            }
        }
        #endregion

        #region Private Methods
        private void ClearValuesForDeletedCoverage()
        {
            PriorityCode = "0";
            PlanCode = 0;
            InsuranceName = string.Empty;
            GroupName = string.Empty;
            BillingMailingAddress1 = string.Empty;
            BillingMailingAddress2 = string.Empty;
            BillingMailingAddress3 = string.Empty;
            StartDate = 0;
            AuthorizationNumber = string.Empty;
            CityStatePlan = string.Empty;
            DaysOfFullCoverage = 0;
            DaysOfCoInsurance = 0;
            ConditionOfService = string.Empty;
            CertSSNID = string.Empty;
            InsuranceGroupNumber = string.Empty;
            TrackingNumber = string.Empty;
            SubscriberLastName = string.Empty;
            SubscriberFirstName = string.Empty;
            SubscriberSuffix = string.Empty;
            SubscriberSex = string.Empty;
            PatientInsuredRelationShip = string.Empty;
            SubscriberAddress1 = string.Empty;
            SubscriberAddress2 = string.Empty;
            SubscriberCity = string.Empty;
            SubscriberStateCode = string.Empty;
            SubscriberAreaCode = 0;
            SubscriberPhoneNumber = 0;
            PlanTerminatedOn = DateTime.MinValue;
            SubscriberEmployerName = string.Empty;
            InsuredEmpCityState1 = string.Empty;
            OtherEmployerDataID1 = string.Empty;
            OtherEmploymentStatus1 = string.Empty;
            OEmployerAddress1 = string.Empty;
            InsuredEmpPhone1 = string.Empty;
            OEmployerZip1 = 0;
            InsuredEmpStreet1 = string.Empty;
            OtherEmployerName2 = string.Empty;
            OtherEmployerLocation2 = string.Empty;
            OtherEmployerDataID2 = string.Empty;
            OtherEmploymentStatus2 = string.Empty;
            OtherEmployerAddress2 = string.Empty;
            InsuredEmpPhone2 = string.Empty;
            OtherEmployerZip2 = 0;
            SubscriberCountryCode = string.Empty;
            InsuredDateOfBirth = string.Empty;
            SubscriberID = string.Empty;
            SubscriberZip = string.Empty;
            SubscriberZipExt = string.Empty;

        }

        private void ReadAccountInfo( Account account )
        {
            AccountNumber = (int)account.AccountNumber;
            GuarantorNumber = (int)account.AccountNumber;

            ITimeBroker tBroker = BrokerFactory.BrokerOfType<ITimeBroker>();
            DateTime facilityDateTime = tBroker.TimeAt( account.Facility.GMTOffset, account.Facility.DSTOffset );


            TransactionDate = facilityDateTime;// DateTime.Today;
            TimeRecordCreation = facilityDateTime; // DateTime.Now;
            if( account.Facility != null )
            {
                HospitalNumber = (int)account.Facility.Oid;
            }
            if( account.Patient != null )
            {
                MedicalRecNumber =
                    account.Patient.MedicalRecordNumber.ToString().PadLeft( 9, '0' );
            }
            if( account.COSSigned != null )
            {
                ConditionOfService = account.COSSigned.Code;
            }
        }

        private void ReadTypeSpecificCoverageValues( Coverage aCoverage )
        {
            if( aCoverage != null )
            {

                if( aCoverage.GetType() != typeof( GovernmentMedicareCoverage ) &&
                    aCoverage.GetType() != typeof( SelfPayCoverage ) )
                {
                    AuthorizationNumber = ( (CoverageGroup)aCoverage ).Authorization.AuthorizationNumber;
                    NumberOfAuthorizedDays = ( (CoverageGroup)aCoverage ).Authorization.NumberOfDaysAuthorized;
                }
                if ( aCoverage.IsMedicareCoverageValidForAuthorization)
                {
                    AuthorizationNumber = ((GovernmentMedicareCoverage)aCoverage).Authorization.AuthorizationNumber;
                    NumberOfAuthorizedDays = ((GovernmentMedicareCoverage)aCoverage).Authorization.NumberOfDaysAuthorized;
                    string trackNum = ((GovernmentMedicareCoverage)aCoverage).TrackingNumber;
                    TrackingNumber = trackNum.Trim().Length > 20 ? trackNum.Trim().Substring(0, 20) : trackNum.Trim();
                }

                if( aCoverage.GetType() == typeof( GovernmentMedicaidCoverage ) )
                {
                    // was PolicyNumber
                    CertSSNID = ( (GovernmentMedicaidCoverage)aCoverage ).PolicyCINNumber;
                    EVCNumber = ( (GovernmentMedicaidCoverage)aCoverage ).EVCNumber;
                    string trackNum = ((GovernmentMedicaidCoverage)aCoverage).TrackingNumber;
                    TrackingNumber = trackNum.Trim().Length > 20 ? trackNum.Trim().Substring(0, 20) : trackNum.Trim();

                    // TLG 06/07/2006 format from green screen transaction:  19980101
                    MedicareIssueDate = ( (GovernmentMedicaidCoverage)aCoverage ).IssueDate.ToString( "yyyyMMdd" );
                }

                if( aCoverage.GetType() == typeof( WorkersCompensationCoverage ) )
                {
                    // was PolicyNumber
                    CertSSNID = ( (WorkersCompensationCoverage)aCoverage ).PolicyNumber;
                    AuthorizationNumber = ( (WorkersCompensationCoverage)aCoverage ).Authorization.AuthorizationNumber;
                    InsuranceAdjuster = ( (WorkersCompensationCoverage)aCoverage ).InsuranceAdjuster;
                    EmployeeSupervisor = ( (WorkersCompensationCoverage)aCoverage ).PatientsSupervisor;
                }

                if( aCoverage.GetType() == typeof( GovernmentMedicareCoverage ) )
                {
                    CertSSNID = ( (GovernmentMedicareCoverage)aCoverage ).HICNumber;
                    MBINUmber = ((GovernmentMedicareCoverage) aCoverage).MBINumber;
                }

                if( aCoverage.GetType() == typeof( CommercialCoverage ) ||
                    aCoverage.GetType() == typeof( OtherCoverage ) ||
                    aCoverage.GetType() == typeof( GovernmentOtherCoverage ) )
                {
                    CertSSNID = ( (CoverageForCommercialOther)aCoverage ).CertSSNID;
                    InsuranceGroupNumber = ( (CoverageForCommercialOther)aCoverage ).GroupNumber;
                    SignedOverMedicareHICNumber = ( (CoverageForCommercialOther)aCoverage ).SignedOverMedicareHICNumber;
                    MBINUmber = ((CoverageForCommercialOther)aCoverage).MBINumber;
                    string trackNum = ( (CoverageForCommercialOther)aCoverage ).TrackingNumber;
                    if( trackNum.Trim().Length > 20 )
                    {
                        TrackingNumber = trackNum.Trim().Substring( 0, 20 );
                    }
                    else
                    {
                        TrackingNumber = trackNum.Trim();
                    }
                }
            }
        }

        private void ReadCompleteCoverageDetails( Coverage aCoverage, Account account )
        {
            /* PBAR - PatientAccess Mapping
                PBAR	Name	                Patient Access
               ================================================
                1	    Medicaid	                4
                2	    Workers Comp	            6
                3	    Medicare	                3
                4	    Government Insurance	    2
                5	    Self Pay	                5
                6	    Commercial	                1
            */

            if( aCoverage != null )
            {
                MapPlanCategoriesWithPBARFor( aCoverage );

                CoverageGroup covGroup = aCoverage as CoverageGroup;
                if( covGroup != null && covGroup.Authorization != null )
                {
                    if( covGroup.Authorization.NameOfCompanyRepresentative != null )
                    {
                        AuthCompanyRepFirstName = covGroup.Authorization.NameOfCompanyRepresentative.FirstName;
                        AuthCompanyRepLastName = covGroup.Authorization.NameOfCompanyRepresentative.LastName;
                    }
                    ServicesAuthorized = covGroup.Authorization.ServicesAuthorized;
                    EffectiveDateOfAuthorization = covGroup.Authorization.EffectiveDate;
                    ExpirationDateOfAuthorization = covGroup.Authorization.ExpirationDate;
                    if( covGroup.Authorization.AuthorizationStatus != null )
                    {
                        AuthorizationStatus = covGroup.Authorization.AuthorizationStatus.Code;
                    }
                    AuthorizationRemarks = TrimCoverageRemarks(
                        covGroup.Authorization.Remarks, AUTHORIZATION_REMARKS_LENGTH );
                }

                if( aCoverage is GovernmentMedicaidCoverage )
                {
                    GovernmentMedicaidCoverage medicaidCov = aCoverage as GovernmentMedicaidCoverage;

                    EligibilityDate_1Medicaid = medicaidCov.EligibilityDate;
                    if( medicaidCov.PatienthasMedicare != null )
                    {
                        PatientHasMedicare_1Medicaid = medicaidCov.PatienthasMedicare.Code;
                    }
                    if( medicaidCov.PatienthasOtherInsuranceCoverage != null )
                    {
                        PatientHasOtherInsurance_1Medicaid = medicaidCov.PatienthasOtherInsuranceCoverage.Code;
                    }
                    CopayAmount_1Medicaid = medicaidCov.MedicaidCopay;
                    EvcNumber_1Medicaid = medicaidCov.EVCNumber;
                    if( medicaidCov.InformationReceivedSource != null && medicaidCov.InformationReceivedSource.Code != string.Empty)
                    {
                        InformationReceivedFrom_1Medicaid = Int32.Parse(medicaidCov.InformationReceivedSource.Code);
                    }
                    Remarks_1Medicaid = TrimCoverageRemarks( medicaidCov.Remarks, COVERAGE_REMARKS_LENGTH );
                }

                if( aCoverage is WorkersCompensationCoverage  )
                {
                    WorkersCompensationCoverage wcCov = aCoverage as WorkersCompensationCoverage;

                    NameOfNetwork_2WC = wcCov.PPOPricingOrBroker;
                    IncidentClaimNumber_2WC = wcCov.ClaimNumberForIncident;
                    if( wcCov.ClaimsAddressVerified != null )
                    {
                        ClaimAddressVerified_2WC = wcCov.ClaimsAddressVerified.Code;
                    }
                    InsurancePhoneNumber_2WC = wcCov.InsurancePhone;
                    if( wcCov.EmployerhasPaidPremiumsToDate != null )
                    {
                        EmployerPremiumPaidToDate_2WC = wcCov.EmployerhasPaidPremiumsToDate.Code;
                    }
                    if( wcCov.InformationReceivedSource != null && wcCov.InformationReceivedSource.Code != string.Empty)
                    {
                        InformationReceivedFrom_2WC = Int32.Parse(wcCov.InformationReceivedSource.Code);
                    }
                    Remarks_2WC = TrimCoverageRemarks( wcCov.Remarks, COVERAGE_REMARKS_LENGTH );
                }

                if( aCoverage is GovernmentMedicareCoverage )
                {
                    GovernmentMedicareCoverage mCareCov = aCoverage as GovernmentMedicareCoverage;

                    PartACoverage_3Medicare = mCareCov.PartACoverage.Code;
                    
                    PartAEffectiveDate_3Medicare = mCareCov.PartACoverageEffectiveDate;
                    if( mCareCov.PartBCoverage != null )
                    {
                        PartBCoverage_3Medicare = mCareCov.PartBCoverage.Code;
                    }
                    PartBEffectiveDate_3Medicare = mCareCov.PartBCoverageEffectiveDate;
                    if( mCareCov.PatientHasMedicareHMOCoverage != null )
                    {
                        PatientHasMedicareHMO_3Medicare = mCareCov.PatientHasMedicareHMOCoverage.Code;
                    }
                    if( mCareCov.MedicareIsSecondary != null )
                    {
                        MedicareIsSecondary_3Medicare = mCareCov.MedicareIsSecondary.Code;
                    }
                    DateOfLastBillingActivity_3Medicare = mCareCov.DateOfLastBillingActivity;
                    RemainingHospitalDays_3Medicare = mCareCov.RemainingBenefitPeriod;
                    RemainingCoInsuranceDays_3Medicare = mCareCov.RemainingCoInsurance;
                    RemainingLifetimeReserveDays_3Medicare = mCareCov.RemainingLifeTimeReserve;
                    RemainingSNFDays_3Medicare = mCareCov.RemainingSNF;
                    RemainingSNFCoInsuranceDays_3Medicare = mCareCov.RemainingSNFCoInsurance;
                    RemainingPartADeductible_3Medicare = mCareCov.RemainingPartADeductible;
                    RemainingPartBDeductible_3Medicare = mCareCov.RemainingPartBDeductible;
                    if (mCareCov.PatientIsPartOfHospiceProgram != null)
                    {
                        PatientIsInHospice_3Medicare = mCareCov.PatientIsPartOfHospiceProgram.Code;
                    }
                    if( mCareCov.VerifiedBeneficiaryName != null )
                    {
                        BeneficiaryNameVerified_3Medicare = mCareCov.VerifiedBeneficiaryName.Code;
                    }
                    if( mCareCov.InformationReceivedSource != null && mCareCov.InformationReceivedSource.Code != string.Empty)
                    {
                        InformationReceivedFrom_3Medicare = Int32.Parse(mCareCov.InformationReceivedSource.Code);
                    }
                    Remarks_3Medicare = TrimCoverageRemarks( mCareCov.Remarks, COVERAGE_REMARKS_LENGTH );
                    ReadAuthorizationDetailsForMedicare53544(mCareCov);
                }

                if( aCoverage is GovernmentOtherCoverage )
                {
                    GovernmentOtherCoverage govOtherCov = aCoverage as GovernmentOtherCoverage;

                    if( govOtherCov.InformationReceivedSource != null && govOtherCov.InformationReceivedSource.Code != string.Empty)
                    {
                        InformationReceivedFrom_4GovIns = Int32.Parse(govOtherCov.InformationReceivedSource.Code);
                    }
                    EligibilityPhoneNumber_4GovIns = govOtherCov.EligibilityPhone;
                    InsuranceCompanyRepName_4GovIns = govOtherCov.InsuranceCompanyRepName;
                    TypeOfCoverage_4GovIns = govOtherCov.TypeOfCoverage;
                    EffectiveDateOfInsured_4GovIns = govOtherCov.EffectiveDateForInsured;
                    TerminationDateForInsured_4GovIns = govOtherCov.TerminationDateForInsured;
                    if( govOtherCov.BenefitsCategoryDetails != null )
                    {
                        DeductibleAmount_4GovIns = govOtherCov.BenefitsCategoryDetails.Deductible;
                        if( govOtherCov.BenefitsCategoryDetails.DeductibleMet != null )
                        {
                            IsDeductibleMet_4GovIns = govOtherCov.BenefitsCategoryDetails.DeductibleMet.Code;
                        }
                         DeductibleAmountMet_4GovIns = govOtherCov.BenefitsCategoryDetails.DeductibleDollarsMet;
                        CoInsurance_4GovIns = govOtherCov.BenefitsCategoryDetails.CoInsurance;
                        OutOfPocket_4GovIns = govOtherCov.BenefitsCategoryDetails.OutOfPocket;
                        IsOutOfPocketMet_4GovIns = govOtherCov.BenefitsCategoryDetails.OutOfPocketMet.Code;
                        OutOfPocketAmountMet_4GovIns = govOtherCov.BenefitsCategoryDetails.OutOfPocketDollarsMet;
                        PercentOutOfPocket_4GovIns = govOtherCov.BenefitsCategoryDetails.AfterOutOfPocketPercent;
                        CoPayAmount_4GovIns = govOtherCov.BenefitsCategoryDetails.CoPay;
                    }
                    Remarks_4GovIns = TrimCoverageRemarks( govOtherCov.Remarks, COVERAGE_REMARKS_LENGTH );
                }

                if( aCoverage is SelfPayCoverage )
                {
                    SelfPayCoverage spCov = aCoverage as SelfPayCoverage;

                    if( null != spCov.PatientHasMedicaid )
                    {
                        HasMedicaid_5SelfPay = spCov.PatientHasMedicaid.Code;
                    }
                    if( null != spCov.InsuranceInfoUnavailable )
                    {
                        InsuranceInfoAvailable_5SelfPay = spCov.InsuranceInfoUnavailable.Code;
                    }

                }

                if( aCoverage is CommercialCoverage )
                {
                    CommercialCoverage commCov = aCoverage as CommercialCoverage;

                    if( commCov.InformationReceivedSource != null && commCov.InformationReceivedSource.Code != string.Empty)
                    {
                        InformationReceivedFrom_6Comm = Int32.Parse(commCov.InformationReceivedSource.Code);
                    }
                    EligibilityPhoneNumber_6Comm = commCov.EligibilityPhone;
                    InsuranceCompanyRepName_6Comm = commCov.InsuranceCompanyRepName;
                    EffectiveDateOfInsured_6Comm = commCov.EffectiveDateForInsured;
                    TerminationDateForInsured_6Comm = commCov.TerminationDateForInsured;
                    if( commCov.ServiceForPreExistingCondition != null )
                    {
                        IsServiceForPreExistingCondition_6Comm = commCov.ServiceForPreExistingCondition.Code;
                    }
                    if( commCov.ServiceIsCoveredBenefit != null )
                    {
                        IsServiceACoveredBenefit_6Comm = commCov.ServiceIsCoveredBenefit.Code;
                    }
                    if( commCov.ClaimsAddressVerified != null )
                    {
                        ClaimsAAddressVerified_6Comm = commCov.ClaimsAddressVerified.Code;
                    }
                    if( commCov.CoordinationOfbenefits != null )
                    {
                        CoordinationOfBenefits_6Comm = commCov.CoordinationOfbenefits.Code;
                    }
                    if( commCov.TypeOfVerificationRule != null )
                    {
                        RuleToDetermineCOB_6Comm = commCov.TypeOfVerificationRule.Oid;
                    }
                    if( commCov.TypeOfProduct != null )
                    {
                        TypeOfProduct_6Comm = commCov.TypeOfProduct.Oid;
                    }
                    PPONetworkBrokerName_6Comm = commCov.PPOPricingOrBroker;
                    if( commCov.FacilityContractedProvider != null )
                    {
                        HospitalIsAContractedProvider_6Comm = commCov.FacilityContractedProvider.Code;
                    }
                    AutoClaimNumber_6Comm = commCov.AutoInsuranceClaimNumber;
                    if( commCov.AutoMedPayCoverage != null )
                    {
                        AutoMedPayCoverage_6Comm = commCov.AutoMedPayCoverage.Code;
                    }
                    Remarks_6Comm = TrimCoverageRemarks( commCov.Remarks, COVERAGE_REMARKS_LENGTH );

                    Hashtable benefitsCategories = commCov.BenefitsCategories;
                    ICollection categories = new ArrayList();
                    if( benefitsCategories.Count > 0 )
                    {
                        categories = benefitsCategories.Keys;
                    }

                    foreach( BenefitsCategory benefitsCategory in categories )
                    {
                        BenefitsCategoryDetails benefitsCategoryDetails =
                            commCov.BenefitsCategoryDetailsFor( benefitsCategory );

                        // Comment out for July 07 release
                        ReadBenefitsCategoryValuesFor( commCov, benefitsCategory, benefitsCategoryDetails );
                    }
                }
            }
        }

        private void MapPlanCategoriesWithPBARFor( Coverage aCoverage )
        {
            if( aCoverage.InsurancePlan != null && aCoverage.InsurancePlan.PlanCategory != null )
            {
                long planCategoryID = aCoverage.InsurancePlan.PlanCategory.Oid;

                switch( planCategoryID )
                {
                    case InsurancePlanCategory.PLANCATEGORY_COMMERCIAL:
                        TypeOfCoverageId = InsurancePlanCategory.PBAR_PLANCATEGORY_COMMERCIAL;
                        break;
                    case InsurancePlanCategory.PLANCATEGORY_GOVERNMENT_OTHER:
                        TypeOfCoverageId = InsurancePlanCategory.PBAR_PLANCATEGORY_GOVERNMENT_OTHER;
                        break;
                    case InsurancePlanCategory.PLANCATEGORY_GOVERNMENT_MEDICARE:
                        TypeOfCoverageId = InsurancePlanCategory.PBAR_PLANCATEGORY_GOVERNMENT_MEDICARE;
                        break;
                    case InsurancePlanCategory.PLANCATEGORY_GOVERNMENT_MEDICAID:
                        TypeOfCoverageId = InsurancePlanCategory.PBAR_PLANCATEGORY_GOVERNMENT_MEDICAID;
                        break;
                    case InsurancePlanCategory.PLANCATEGORY_SELF_PAY:
                        TypeOfCoverageId = InsurancePlanCategory.PBAR_PLANCATEGORY_SELF_PAY;
                        break;
                    case InsurancePlanCategory.PLANCATEGORY_WORKERS_COMPENSATION:
                        TypeOfCoverageId = InsurancePlanCategory.PBAR_PLANCATEGORY_WORKERS_COMPENSATION;
                        break;
                    default:
                        break;
                }
            }
        }

        private void ReadBenefitsCategoryValuesFor( CommercialCoverage commCov,
            BenefitsCategory benefitsCategory, BenefitsCategoryDetails benefitsCategoryDetails )
        {
            // The Benefits Category Oid should match the corresponding column names 
            // bucket created for that category. For example, if the category Oid = 1
            // the following 19 field values for that category should be assigned 
            // exactly to their matching column names in PBAR starting with AP601####.
            // If Oid = 2, they should be assigned to AP602####.....

            string index = string.Empty;
            if( benefitsCategory.Oid.ToString().Length < 2 )
            {
                index = benefitsCategory.Oid.ToString().PadLeft( 2, '0' );
            }
            else
            {
                index = benefitsCategory.Oid.ToString();
            }

            string APCTID = AP6 + index + CTID;
            insuranceDetailsOrderedList[APCTID] = benefitsCategory.Oid;

            if( benefitsCategoryDetails != null )
            {
                if( benefitsCategoryDetails.Deductible != -1)
                {
                    string APDEDA = AP6 + index + DEDA;
                    insuranceDetailsOrderedList[APDEDA] = benefitsCategoryDetails.Deductible;
                }

                if( benefitsCategoryDetails.TimePeriod != null )
                {
                    string APTMPR = AP6 + index + TMPR;
                    insuranceDetailsOrderedList[APTMPR] = benefitsCategoryDetails.TimePeriod.Code;
                }
                if( benefitsCategoryDetails.DeductibleMet != null &&
                    benefitsCategoryDetails.DeductibleMet.Code != YesNoFlag.CODE_BLANK )
                {
                    string APDEMT = AP6 + index + DEMT;
                    insuranceDetailsOrderedList[APDEMT] = benefitsCategoryDetails.DeductibleMet.Code;
                }
                if( benefitsCategoryDetails.DeductibleDollarsMet != -1 )
                {
                    string APDEAM = AP6 + index + DEAM;
                    insuranceDetailsOrderedList[APDEAM] = benefitsCategoryDetails.DeductibleDollarsMet;
                }
                if( benefitsCategoryDetails.CoInsurance != -1 )
                {
                    string APCOIN = AP6 + index + COIN;
                    insuranceDetailsOrderedList[APCOIN] = benefitsCategoryDetails.CoInsurance;
                }
                if( benefitsCategoryDetails.OutOfPocket != -1 )
                {
                    string APOTPK = AP6 + index + OTPK;
                    insuranceDetailsOrderedList[APOTPK] = benefitsCategoryDetails.OutOfPocket;
                }
                if( benefitsCategoryDetails.OutOfPocketMet != null &&
                    benefitsCategoryDetails.OutOfPocketMet.Code != YesNoFlag.CODE_BLANK )
                {
                    string APOTPM = AP6 + index + OTPM;
                    insuranceDetailsOrderedList[APOTPM] = benefitsCategoryDetails.OutOfPocketMet.Code;
                }
                if( benefitsCategoryDetails.OutOfPocketDollarsMet != -1 )
                {
                    string APOTPA = AP6 + index + OTPA;
                    insuranceDetailsOrderedList[APOTPA] = benefitsCategoryDetails.OutOfPocketDollarsMet;
                }
                if( benefitsCategoryDetails.AfterOutOfPocketPercent != -1 )
                {
                    string APPROT = AP6 + index + PROT;
                    insuranceDetailsOrderedList[APPROT] = benefitsCategoryDetails.AfterOutOfPocketPercent;
                }
                if( benefitsCategoryDetails.CoPay != -1 )
                {
                    string APCPAY = AP6 + index + CPAY;
                    insuranceDetailsOrderedList[APCPAY] = Convert.ToSingle( benefitsCategoryDetails.CoPay );
                }
                if( benefitsCategoryDetails.WaiveCopayIfAdmitted != null &&
                    benefitsCategoryDetails.WaiveCopayIfAdmitted.Code != YesNoFlag.CODE_BLANK )
                {
                    string APWCPY = AP6 + index + WCPY;
                    insuranceDetailsOrderedList[APWCPY] = benefitsCategoryDetails.WaiveCopayIfAdmitted.Code;
                }
                if( benefitsCategoryDetails.VisitsPerYear != -1 )
                {
                    string APNMVS = AP6 + index + NMVS;
                    insuranceDetailsOrderedList[APNMVS] = benefitsCategoryDetails.VisitsPerYear;
                }
                if( benefitsCategoryDetails.LifeTimeMaxBenefit != -1 )
                {
                    string APLFMB = AP6 + index + LFMB;
                    insuranceDetailsOrderedList[APLFMB] = benefitsCategoryDetails.LifeTimeMaxBenefit;
                }
                if( benefitsCategoryDetails.RemainingLifetimeValue != -1 )
                {
                    string APLFRM = AP6 + index + LFRM;
                    insuranceDetailsOrderedList[APLFRM] = benefitsCategoryDetails.RemainingLifetimeValue;
                }
                if( benefitsCategoryDetails.RemainingLifetimeValueMet != null &&
                    benefitsCategoryDetails.RemainingLifetimeValueMet.Code != YesNoFlag.CODE_BLANK )
                {
                    string APLFVM = AP6 + index + LFVM;
                    insuranceDetailsOrderedList[APLFVM] = benefitsCategoryDetails.RemainingLifetimeValueMet.Code;
                }
                if( benefitsCategoryDetails.MaxBenefitPerVisit != -1 )
                {
                    string APMXBN = AP6 + index + MXBN;
                    insuranceDetailsOrderedList[APMXBN] = benefitsCategoryDetails.MaxBenefitPerVisit;
                }
                if( benefitsCategoryDetails.RemainingBenefitPerVisits != -1 )
                {
                    string APRMBN = AP6 + index + RMBN;
                    insuranceDetailsOrderedList[APRMBN] = benefitsCategoryDetails.RemainingBenefitPerVisits;
                }
                if( benefitsCategoryDetails.RemainingBenefitPerVisitsMet != null &&
                    benefitsCategoryDetails.RemainingBenefitPerVisitsMet.Code != YesNoFlag.CODE_BLANK )
                {
                    string APRMMT = AP6 + index + RMMT;
                    insuranceDetailsOrderedList[APRMMT] = benefitsCategoryDetails.RemainingBenefitPerVisitsMet.Code;
                }
            }
        }

        private static string TrimCoverageRemarks( string remarks, int remarksLength )
        {
            string trimmedRemarks = string.Empty;

            if( !string.IsNullOrEmpty( remarks ) )
            {
                if( remarks.Length < remarksLength )
                {
                    trimmedRemarks = remarks;
                }
                else
                {
                    trimmedRemarks = remarks.Substring( 0, remarksLength );
                }
            }

            return trimmedRemarks;
        }

        private void ReadCommonCoverageValues( Coverage coverage, Activity activity )
        {
            if( coverage != null )
            {
                SequenceNumber = (int)coverage.CoverageOrder.Oid;
                PriorityCode = coverage.Priority.ToString();

                if (activity.GetType() == typeof(PreMSERegisterActivity) || activity.GetType() == typeof(UCCPreMSERegistrationActivity))
                {
                    ServiceType = ALLEMERGENCYSERVICETYPE;
                    PlanCode = 0;
                }
                else
                {
                    PlanCode = (int)coverage.CoverageOrder.Oid;
                }
                if( coverage.NoLiability )
                {
                    NoPatientLiability = NO_LIABLITY_FLAG;
                }
                if( coverage.BenefitsVerified != null )
                {
                    switch( coverage.BenefitsVerified.Code )
                    {
                        case YesNotApplicableFlag.CODE_YES:
                            BenefitsVerificationStatus = "V";
                            break;
                        case "":
                            BenefitsVerificationStatus = YesNotApplicableFlag.CODE_BLANK;
                            break;
                        default:
                            BenefitsVerificationStatus = coverage.BenefitsVerified.Code.Substring( 0, 1 );
                            break;
                    }
                }
                if( coverage.AuthorizingPerson.Length > 4 )
                {
                    BenefitsVerificationDoneBy = coverage.AuthorizingPerson.Substring( 0, 4 );
                }
                else
                {
                    BenefitsVerificationDoneBy = coverage.AuthorizingPerson;
                }
                BenefitsVerificationStartDate = coverage.DateTimeOfVerification;

                if( coverage.InsurancePlan != null )
                {
                    PlanName = coverage.InsurancePlan.PlanName;
                    PlanApprovedOn = coverage.InsurancePlan.ApprovedOn;
                    PlanEffectiveOn = coverage.InsurancePlan.EffectiveOn;
                    PlanTerminatedOn = coverage.InsurancePlan.TerminatedOn;

                    PlanId = coverage.InsurancePlan.PlanID;
                    if( coverage.InsurancePlan.Payor != null )
                    {
                        InsuranceName = coverage.InsurancePlan.Payor.Name;
                    }
                    UBF1PlanId = coverage.InsurancePlan.PlanID;

                    if( coverage.InsurancePlan.GetType() == typeof( OtherInsurancePlan ) )
                    {
                        CertSSNID = ( (OtherCoverage)coverage ).CertSSNID;
                        SignedOverMedicareHICNumber = ( (OtherCoverage)coverage ).SignedOverMedicareHICNumber;
                        MBINUmber = ((OtherCoverage)coverage).MBINumber;
                    }

                    if( coverage.BillingInformation != null )
                    {
                        BillingInformation billingInformation = coverage.BillingInformation;
                        BillingCareOfName = billingInformation.BillingCOName;
                        BillingName = billingInformation.BillingName;
                        string postalCode = String.Empty;
                        string phoneNumber = String.Empty;
                        string billingAddress3 = String.Empty;
                        if( billingInformation.Address != null )
                        {
                            string stateCode = String.Empty;
                            string countryCode = String.Empty;

                            var address = billingInformation.Address.Address1;
                            if (address.Length > BILLING_ADDRESS_LENGTH)
                            {
                                address = address.Substring(0, BILLING_ADDRESS_LENGTH);
                            }
                            BillingMailingAddress1 = address;
                            if( billingInformation.Address.State != null )
                            {
                                stateCode = billingInformation.Address.State.Code;
                            }
                            if( billingInformation.Address.Country != null )
                            {
                                countryCode = billingInformation.Address.Country.Code;
                            }
                            BillingMailingAddress2 =
                                billingInformation.Address.City.PadRight( 15, ' ' ).Substring( 0, 15 ) +
                                stateCode.PadRight( 2, ' ' ).Substring( 0, 2 ) +
                                countryCode.PadRight( 3, ' ' ).Substring( 0, 3 );

                            if( billingInformation.Address.ZipCode != null )
                            {
                                postalCode = billingInformation.Address.ZipCode.PostalCode;
                            }
                            else
                            {
                                postalCode = string.Empty;
                            }
                        }

                        if( billingInformation.PhoneNumber != null )
                        {
                            phoneNumber = billingInformation.PhoneNumber.AreaCode.PadRight( 3, ' ' ) +
                                billingInformation.PhoneNumber.Number.PadRight( 7, ' ' );

                        }
                        billingAddress3 = postalCode.PadRight( 9, ' ' ) + phoneNumber;
                        BillingMailingAddress3 = billingAddress3;
                    }
                }

                if( coverage.InformationReceivedSource != null )
                {
                    string info = string.Empty;
                    if( coverage.InformationReceivedSource.Description.Length < 25 )
                    {
                        info = coverage.InformationReceivedSource.Description;
                    }
                    else
                    {
                        info = coverage.InformationReceivedSource.Description.Substring( 0, 24 );
                    }
                    InfoReceivedSource = info;
                }
                if( coverage.Insured != null )
                {
                    SubscriberLastName = coverage.Insured.LastName;

                    // save the name back to pbar with the middle initial appended to the first name
                    if (coverage.Insured.Name.MiddleInitial != null &&
                        coverage.Insured.Name.MiddleInitial.Trim().Length > 0)
                    {
                        // the column to hold the 1st name is only 15 characters long
                        // so if there is a middle initial and the first name is longer that 13
                        // truncate the first name to 13 characters so the first name and the MI 
                        // will fit in the transaction table's column.
                        string trimmedFirstName = coverage.Insured.FirstName;
                        if (coverage.Insured.FirstName.Length > 13 &&
                            coverage.Insured.Name.MiddleInitial.Trim().Length > 0)
                        {
                            trimmedFirstName = coverage.Insured.FirstName.Substring(0, 13);
                        }

                        SubscriberFirstName = trimmedFirstName + " " + coverage.Insured.Name.MiddleInitial;
                    }
                    else
                    {
                        SubscriberFirstName = coverage.Insured.FirstName;
                    }

                    if( coverage.Insured.Name != null &&
                        coverage.Insured.Name.MiddleInitial != null &&
                        coverage.Insured.Name.MiddleInitial.Trim() != String.Empty )
                    {
                        SubscriberMInitial = coverage.Insured.Name.MiddleInitial.Trim().Substring( 0, 1 );
                    }
                    if (coverage.Insured.Name != null &&
                        coverage.Insured.Name.Suffix != null &&
                        coverage.Insured.Name.Suffix.Trim() != String.Empty)
                    {
                        SubscriberSuffix = coverage.Insured.Name.Suffix.Trim();
                    }
                    SubscriberID = coverage.Insured.NationalID;

                    if( coverage.Insured.Sex != null )
                    {
                        SubscriberSex = coverage.Insured.Sex.Code;
                    }

                    if( coverage.Insured.Employment != null )
                    {
                        EmployeeID = coverage.Insured.Employment.EmployeeID;
                    }

                    if( !coverage.Insured.DateOfBirth.Equals( DateTime.MinValue ) )
                    {
                        // TLG 05/26/2006 changed from 'packed' format to 8-char format
                        InsuredDateOfBirth = coverage.Insured.DateOfBirth.ToString( "MMddyyyy" );
                    }

                    ContactPoint aCP = coverage.Insured.ContactPointWith(
                        TypeOfContactPoint.NewPhysicalContactPointType() );

                    if( aCP.Address != null )
                    {
                        ReadSubscriberAddress( aCP.Address );
                    }
                }

                if( coverage.Attorney != null )
                {
                    AttorneyName = coverage.Attorney.AttorneyName;
                    ReadAttorneyAddress( coverage.Attorney.ContactPointWith(
                        TypeOfContactPoint.NewBusinessContactPointType() ).Address );
                    ReadAttorneyPhoneNumber( coverage.Attorney.ContactPointWith(
                        TypeOfContactPoint.NewBusinessContactPointType() ).PhoneNumber );
                }

                if( coverage.InsuranceAgent != null )
                {
                    InsuranceAgentName = coverage.InsuranceAgent.AgentName;
                    ReadInsuranceAgentAddress( coverage.InsuranceAgent.ContactPointWith(
                        TypeOfContactPoint.NewBusinessContactPointType() ).Address );
                    ReadInsuranceAgentPhoneNumber( coverage.InsuranceAgent.ContactPointWith(
                        TypeOfContactPoint.NewBusinessContactPointType() ).PhoneNumber );
                }
            }
        }

        /// <summary>
        /// This method sets properties related to primary coverage.
        /// </summary>
        /// <param name="coverage"></param>
        /// <param name="anAccount"></param>
        private void ReadPrimaryCoverageValues( Coverage coverage, Account anAccount )
        {
            ContactPoint aCP = coverage.Insured.ContactPointWith(
                TypeOfContactPoint.NewPhysicalContactPointType() );

            if( aCP.Address != null )
            {
                ReadSubscriberPhoneNumber( aCP.PhoneNumber );
            }

            ContactPoint cp2 = new ContactPoint();

            // insured employer info

            if( coverage.Insured.Employment.Status != null )
            {
                OtherEmploymentStatus1 = coverage.Insured.
                    Employment.Status.Code;
            }

            if( coverage.Insured.Employment.Employer != null )
            {
                EmployerCode1 = coverage.Insured.Employment.Employer.EmployerCode;
                SubscriberEmployerName = coverage.Insured.
                    Employment.Employer.Name;
                if( coverage.Insured.Employment.Employer.PartyContactPoint != null )
                {
                    cp2 = coverage.Insured.Employment.Employer.PartyContactPoint;
                }
            }

            if( cp2 != null && cp2.PhoneNumber != null )
            {
                string empPhone = cp2.PhoneNumber.AsUnformattedString();
                if( empPhone.Length > 10 )
                {
                    empPhone = empPhone.Substring( empPhone.Length - 10, 10 );
                }
                InsuredEmpPhone1 = empPhone;
            }

            if( coverage.Insured.ContactPointWith(
                TypeOfContactPoint.NewMobileContactPointType() ).
                PhoneNumber != null )
            {
                ContactPoint cp = coverage.Insured.ContactPointWith
                    ( TypeOfContactPoint.NewMobileContactPointType() );

                SubscriberCellPhone = cp.PhoneNumber.AreaCode + cp.PhoneNumber.Number;
            }

            Address empAddress = cp2.Address;

            if( empAddress != null )
            {
                if( empAddress.Address1.Length > 25 )
                {
                    InsuredEmpStreet1 = empAddress.Address1.Substring( 0, 25 );
                }
                else
                {
                    InsuredEmpStreet1 = empAddress.Address1;
                }

                string city = string.Empty;
                if( empAddress.City.Length > 16 )
                {
                    city = empAddress.City.Substring( 0, 16 );
                }
                else
                {
                    city = empAddress.City.PadRight( 16, ' ' );
                }

                InsuredEmpCityState1 = city + empAddress.State.Code;
                if( empAddress.ZipCode != null && empAddress.ZipCode.ZipCodePrimary != null )
                {
                    if( empAddress.ZipCode.ZipCodePrimary.Trim().Length > 5 )
                    {
                        InsuredEmpZip1 = empAddress.ZipCode.ZipCodePrimary.Trim().Substring( 0, 5 );
                    }
                    else
                    {
                        InsuredEmpZip1 = empAddress.ZipCode.ZipCodePrimary.Trim();
                    }
                }

                if( empAddress.ZipCode != null && empAddress.ZipCode.ZipCodeExtended != null )
                {
                    if( empAddress.ZipCode.ZipCodeExtended.Trim().Length > 4 )
                    {
                        InsuredEmpZipExt1 = empAddress.ZipCode.ZipCodeExtended.Trim().Substring( 0, 4 );
                    }
                    else
                    {
                        InsuredEmpZipExt1 = empAddress.ZipCode.PadZeroZipCodeExtended( empAddress.ZipCode.ZipCodeExtended.Trim() );
                    }
                }
            }

            //next fields used only for Pre-MSE and Post-MSE:
            if( anAccount.Activity != null && anAccount.PreMSECopiedAccountNumber != 0 )
            {
                if (anAccount.Activity.GetType().Equals(typeof (PreMSERegisterActivity)) ||
                    anAccount.Activity.GetType().Equals(typeof (UCCPreMSERegistrationActivity)))
                {
                    MSEActivityFlag = MSE_FLAG_ADD;
                    //next statement intentionally hasn’t been moved outside if-statement 
                    //because we don’t want this field to be populated for other activities than Pre-MSE and Post-MSE 
                    PreMSECopiedAccountNumber = anAccount.PreMSECopiedAccountNumber;
                }
                else if (anAccount.Activity.GetType().Equals(typeof (PostMSERegistrationActivity)) ||
                    anAccount.Activity.GetType().Equals(typeof(UCCPostMseRegistrationActivity)))
                {
                    MSEActivityFlag = MSE_FLAG_DELETE;
                    //next statement intentionally hasn’t been moved outside if-staement 
                    //because we don’t want this field to be populated for other activities than Pre-MSE and Post-MSE 
                    PreMSECopiedAccountNumber = anAccount.PreMSECopiedAccountNumber;
                }
            }
        }

        // This method sets properties related to Secondary coverage.
        private void ReadSecondaryCoverageValues( Coverage coverage )
        {
            ContactPoint cp1 = new ContactPoint();
            ContactPoint cp2 = new ContactPoint();

            // insured phone info

            cp1 = coverage.Insured.ContactPointWith( TypeOfContactPoint.NewPhysicalContactPointType() );
            PhoneNumber phoneNumber = cp1.PhoneNumber;

            if( phoneNumber != null )
            {
                ReadSubscriberPhoneNumber( phoneNumber );
            }
            // insured employer info

            if( coverage.Insured.Employment.Status != null )
            {
                OtherEmploymentStatus2 = coverage.Insured.
                    Employment.Status.Code;
            }

            if( coverage.Insured.Employment.Employer != null )
            {
                EmployerCode2 = coverage.Insured.Employment.Employer.EmployerCode;
                OtherEmployerName2 = coverage.Insured.
                    Employment.Employer.Name;
                if( coverage.Insured.Employment.Employer.PartyContactPoint != null )
                {
                    cp2 = coverage.Insured.Employment.Employer.PartyContactPoint;
                }
            }

            if( cp2 != null && cp2.PhoneNumber != null )
            {
                string empPhone = cp2.PhoneNumber.AsUnformattedString();
                if ( empPhone.Length > 0 && empPhone.Length < 10 )
                {
                    string PhoneZeros = "0000000000";
                    empPhone = empPhone + PhoneZeros.Substring (1, 10 - empPhone.Length);
                }
                if( empPhone.Length > 10 )
                {
                    empPhone = empPhone.Substring( empPhone.Length - 10, 10 );
                }
                InsuredEmpPhone2 = empPhone;
            }


            if( coverage.Insured.ContactPointWith(
                TypeOfContactPoint.NewMobileContactPointType() ).
                PhoneNumber != null )
            {
                SubscriberCellPhone =
                    coverage.Insured.ContactPointWith(
                    TypeOfContactPoint.NewMobileContactPointType() ).
                    PhoneNumber.ToString();
            }

            Address empAddress = cp2.Address;

            if( empAddress != null )
            {
                if( empAddress.Address1.Length > 25 )
                {
                    InsuredEmpStreet2 = empAddress.Address1.Substring( 0, 25 );
                }
                else
                {
                    InsuredEmpStreet2 = empAddress.Address1;
                }

                string city = string.Empty;
                if( empAddress.City.Length > 16 )
                {
                    city = empAddress.City.Substring( 0, 16 );
                }
                else
                {
                    city = empAddress.City.PadRight( 16, ' ' );
                }

                InsuredEmpCityState2 = city + empAddress.State.Code;
                if( empAddress.ZipCode != null && empAddress.ZipCode.ZipCodePrimary != null )
                {
                    if( empAddress.ZipCode.ZipCodePrimary.Trim().Length > 5 )
                    {
                        InsuredEmpZip2 = empAddress.ZipCode.ZipCodePrimary.Trim().Substring( 0, 5 );
                    }
                    else
                    {
                        InsuredEmpZip2 = empAddress.ZipCode.ZipCodePrimary.Trim();
                    }
                }

                if( empAddress.ZipCode != null && empAddress.ZipCode.ZipCodeExtended != null )
                {
                    if( empAddress.ZipCode.ZipCodeExtended.Trim().Length > 4 )
                    {
                        InsuredEmpZipExt2 = empAddress.ZipCode.ZipCodeExtended.Trim().Substring( 0, 4 );
                    }
                    else
                    {
                        InsuredEmpZipExt2 = empAddress.ZipCode.PadZeroZipCodeExtended( empAddress.ZipCode.ZipCodeExtended.Trim() );
                    }
                }
            }
        }

        private void ReadSubscriberAddress( Address address )
        {
            if( address != null )
            {
                SubscriberAddress1 = address.Address1;

                SubscriberAddress2 = address.Address2;

                SubscriberCity = FormattedCity( address.City );

                if( address.ZipCode != null && address.ZipCode.ZipCodePrimary != null )
                {
                    SubscriberZip = address.ZipCode.ZipCodePrimary;
                }

                if( address.ZipCode != null && address.ZipCode.ZipCodeExtendedZeroPadded != null )
                {
                    SubscriberZipExt = address.ZipCode.ZipCodeExtendedZeroPadded;
                }

                if( address.State != null )
                {
                    SubscriberStateCode = address.State.Code;
                }

                if( address.Country != null )
                {
                    SubscriberCountryCode = address.Country.Code;
                }
            }
        }

        private void ReadSubscriberPhoneNumber( PhoneNumber phoneNumber )
        {
            if( phoneNumber != null )
            {
                SubscriberAreaCode = ConvertToInt( phoneNumber.AreaCode );
                SubscriberPhoneNumber = ConvertToInt( phoneNumber.Number );
            }
        }

        private void ReadAttorneyAddress( Address address )
        {
            if( address != null )
            {
                AttorneyStreet = address.Address1 + address.Address2;
                AttorneyCity = FormattedCity( address.City );
                if( address.State != null )
                {
                    AttorneyState = address.State.Code;
                }
                else
                {
                    AttorneyState = String.Empty;
                }
                if( address.Country != null )
                {
                    AttorneyCountryCode = address.Country.Code;
                }
                else
                {
                    AttorneyCountryCode = String.Empty;
                }

                if( address.ZipCode != null
                    && address.ZipCode.ZipCodePrimary != null )
                {
                    AttorneyZip5 = address.ZipCode.ZipCodePrimary;
                }

                if( address.ZipCode != null
                    && address.ZipCode.ZipCodeExtendedZeroPadded != null )
                {
                    AttorneyZip4 = address.ZipCode.ZipCodeExtendedZeroPadded;
                }
            }
        }

        private void ReadAttorneyPhoneNumber( PhoneNumber phoneNumber )
        {
            if( phoneNumber != null )
            {
                string tenDigitPhoneNumber = phoneNumber.AreaCode + phoneNumber.Number;
                if( tenDigitPhoneNumber != String.Empty )
                {
                    AttorneyPhoneNumber = Convert.ToInt64( tenDigitPhoneNumber );
                }
            }
        }

        private void ReadInsuranceAgentAddress( Address address )
        {
            if( address != null )
            {
                InsuranceAgentStreet = address.Address1 + address.Address2;
                InsuranceAgentCity = FormattedCity( address.City );
                if( address.State != null )
                {
                    InsuranceAgentState = address.State.Code;
                }
                else
                {
                    InsuranceAgentState = String.Empty;
                }
                if( address.Country != null )
                {
                    InsuranceAgentCountryCode = address.Country.Code;
                }
                else
                {
                    InsuranceAgentCountryCode = String.Empty;
                }

                if( address.ZipCode != null
                    && address.ZipCode.ZipCodePrimary != null )
                {
                    InsuranceAgentZip5 = address.ZipCode.ZipCodePrimary;
                }

                if( address.ZipCode != null
                    && address.ZipCode.ZipCodeExtendedZeroPadded != null )
                {
                    InsuranceAgentZip4 = address.ZipCode.ZipCodeExtendedZeroPadded;
                }
            }
        }

        private void ReadInsuranceAgentPhoneNumber( PhoneNumber phoneNumber )
        {
            if( phoneNumber != null )
            {
                string tenDigitPhoneNumber = phoneNumber.AreaCode + phoneNumber.Number;
                if( tenDigitPhoneNumber != String.Empty )
                {
                    InsuranceAgentPhoneNumber = Convert.ToInt64( tenDigitPhoneNumber );
                }
            }
        }

        private void ReadMedicareSecondaryPayor( MedicareSecondaryPayor medicareSecondaryPayor )
        {
            MSPVersion = VERSION_1;

            if( medicareSecondaryPayor.SpecialProgram != null )
            {
                SpecialProgram specialProgram = medicareSecondaryPayor.SpecialProgram;
                BlackLungBenefitsFlag = specialProgram.BlackLungBenefits.Code;
                GovernmentProgramFlag = specialProgram.GovernmentProgram.Code;
                DVAAuthorizedFlag = specialProgram.DVAAuthorized.Code;
                WorkRelatedFlag = specialProgram.WorkRelated.Code;
                BlackLungDate = specialProgram.BLBenefitsStartDate;
            }

            if( medicareSecondaryPayor.LiabilityInsurer != null )
            {
                LiabilityInsurer liabilityInsurer =
                    medicareSecondaryPayor.LiabilityInsurer;
                NonWorkRelatedFlag = liabilityInsurer.NonWorkRelated.Code;
                AnotherPartyResponsibilityFlag = liabilityInsurer.AnotherPartyResponsibility.Code;
                AccidentInjuryDate = liabilityInsurer.AccidentDate;
                AutoAccidentType = liabilityInsurer.AccidentType.Code;
            }
            if( medicareSecondaryPayor.MedicareEntitlement != null )
            {
                MedicareEntitlement medicareEntitlement =
                    medicareSecondaryPayor.MedicareEntitlement;
                AgeEntitlement ageEntitlement = new AgeEntitlement();
                ESRDEntitlement esrdEntitlement = new ESRDEntitlement();
                DisabilityEntitlement disabilityEntitlement =
                    new DisabilityEntitlement();

                Employed = EmploymentStatusFlagFor( medicareEntitlement.PatientEmployment.Status.Code );
                PatientNeverEmployedFlag =
                    NeverEmployedStatusFlagFor( medicareEntitlement.PatientEmployment.Status.Code );

                GroupHealthPlan =
                    medicareEntitlement.GroupHealthPlanCoverage.Code;
                if( medicareEntitlement.GetType() == ageEntitlement.GetType() )
                {
                    AgeFlag = "Y";
                    ageEntitlement = medicareEntitlement as AgeEntitlement;

                    if( ageEntitlement == null ) return;

                    if( ageEntitlement.GroupHealthPlanCoverage != null )
                    {
                        AgeGroupHealthPlanCoverage =
                            ageEntitlement.GroupHealthPlanCoverage.Code;
                    }
                    if( ageEntitlement.GHPLimitExceeded != null )
                    {
                        AgeGroupHealthPlanCoverageLimitExceeded =
                            ageEntitlement.GHPLimitExceeded.Code;
                    }
                    if( ageEntitlement.PatientEmployment != null )
                    {
                        RetireDate =
                            ageEntitlement.PatientEmployment.RetiredDate;
                    }
                    if( ageEntitlement.SpouseEmployment != null )
                    {
                        Employment spouseEmployment =
                            ageEntitlement.SpouseEmployment;

                        SpouseEmploymentRetireDate =
                            spouseEmployment.RetiredDate;
                        SpouseEmployerName =
                            spouseEmployment.Employer.Name;
                        if( spouseEmployment.Employer.PartyContactPoint.Address.Address1.Length > 25 )
                        {
                            SpouseEmployerAddress = spouseEmployment.Employer.PartyContactPoint.Address.Address1.Substring( 0, 25 );
                        }
                        else
                        {
                            SpouseEmployerAddress = spouseEmployment.Employer.PartyContactPoint.Address.Address1;
                        }
                        SpouseEmployerCity =
                            MSPFormattedCity( spouseEmployment.Employer.PartyContactPoint.Address.City );
                        SpouseEmployerState =
                            spouseEmployment.Employer.PartyContactPoint.Address.State.Code;

                        if( spouseEmployment.Employer.PartyContactPoint.Address.ZipCode != null &&
                            spouseEmployment.Employer.PartyContactPoint.Address.ZipCode.ZipCodePrimary != null )
                        {
                            SpouseEmployerZipCodePrimary =
                                spouseEmployment.Employer.PartyContactPoint.Address.ZipCode.ZipCodePrimary;

                            if( spouseEmployment.Employer.PartyContactPoint.Address.ZipCode.ZipCodeExtended != null && 
                                spouseEmployment.Employer.PartyContactPoint.Address.ZipCode.ZipCodeExtended.Trim().Length >= 4 )
                            {
                                SpouseEmployerZipCodeExtended =
                                    spouseEmployment.Employer.PartyContactPoint.Address.ZipCode.ZipCodeExtended.Trim().Substring( 0, 4 );
                            }
                            else if( spouseEmployment.Employer.PartyContactPoint.Address.ZipCode.ZipCodePrimary.Trim().Length > 0 )
                            {
                                SpouseEmployerZipCodeExtended = "0000";
                            }
                        }

                        SpouseEmployedFlag = EmploymentStatusFlagFor( spouseEmployment.Status.Code );
                        SpouseNeverEmployedFlag = NeverEmployedStatusFlagFor( spouseEmployment.Status.Code );
                    }

                }
                else if( medicareEntitlement.GetType() == disabilityEntitlement.GetType() )
                {
                    DisabilityFlag = "Y";
                    disabilityEntitlement = medicareEntitlement as DisabilityEntitlement;
                    
                    if ( disabilityEntitlement == null ) return;

                    if ( disabilityEntitlement.FamilyMemberEmployment != null )
                    {
                        Employment familyMemberEmployment =
                            disabilityEntitlement.FamilyMemberEmployment;

                        FamilyMemberEmployerName =
                            familyMemberEmployment.Employer.Name;
                        if( familyMemberEmployment.Employer.PartyContactPoint.Address.Address1.Length > 25 )
                        {
                            FamilyMemberEmployerAddress = 
                                familyMemberEmployment.Employer.PartyContactPoint.Address.Address1.Substring( 0, 25 );
                        }
                        else
                        {
                            FamilyMemberEmployerAddress = 
                                familyMemberEmployment.Employer.PartyContactPoint.Address.Address1;
                        }
                        FamilyMemberEmployerCity =
                            MSPFormattedCity( familyMemberEmployment.Employer.PartyContactPoint.Address.City );
                        FamilyMemberEmployerState =
                            familyMemberEmployment.Employer.PartyContactPoint.Address.State.Code;

                        if( familyMemberEmployment.Employer.PartyContactPoint.Address.ZipCode != null &&
                            familyMemberEmployment.Employer.PartyContactPoint.Address.ZipCode.ZipCodePrimary != null )
                        {
                            FamilyMemberEmployerZipCodePrimary =
                                familyMemberEmployment.Employer.PartyContactPoint.Address.ZipCode.ZipCodePrimary;

                            if( familyMemberEmployment.Employer.PartyContactPoint.Address.ZipCode.ZipCodeExtended != null && 
                                familyMemberEmployment.Employer.PartyContactPoint.Address.ZipCode.ZipCodeExtended.Trim().Length >= 4 )
                            {
                                FamilyMemberEmployerZipCodeExtended =
                                    familyMemberEmployment.Employer.PartyContactPoint.Address.ZipCode.ZipCodeExtended.Trim().Substring( 0, 4 );
                            }
                            else if( familyMemberEmployment.Employer.PartyContactPoint.Address.ZipCode.ZipCodePrimary.Trim().Length > 0 )
                            {
                                FamilyMemberEmployerZipCodeExtended = "0000";
                            }
                        }
                    }

                    if( disabilityEntitlement.GroupHealthPlanCoverage != null )
                    {
                        DisabilityGroupHealthPlanCoverageFlag =
                            disabilityEntitlement.GroupHealthPlanCoverage.Code;
                    }
                    if( disabilityEntitlement.GHPLimitExceeded != null )
                    {
                        DisabilityGHPCoverageLimitExceededFlag =
                            disabilityEntitlement.GHPLimitExceeded.Code;
                    }
                    if( disabilityEntitlement.PatientEmployment != null )
                    {
                        RetireDate =
                            disabilityEntitlement.PatientEmployment.RetiredDate;
                    }
                    if( disabilityEntitlement.SpouseEmployment != null )
                    {
                        Employment spouseEmployment =
                            disabilityEntitlement.SpouseEmployment;

                        SpouseEmploymentRetireDate =
                            spouseEmployment.RetiredDate;
                        SpouseEmployerName =
                            spouseEmployment.Employer.Name;
                        if( spouseEmployment.Employer.PartyContactPoint.Address.Address1.Length > 25 )
                        {
                            SpouseEmployerAddress = 
                                spouseEmployment.Employer.PartyContactPoint.Address.Address1.Substring( 0, 25 );
                        }
                        else
                        {
                            SpouseEmployerAddress = 
                                spouseEmployment.Employer.PartyContactPoint.Address.Address1;
                        }
                        SpouseEmployerCity =
                            MSPFormattedCity( spouseEmployment.Employer.PartyContactPoint.Address.City );
                        SpouseEmployerState =
                            spouseEmployment.Employer.PartyContactPoint.Address.State.Code;

                        if( spouseEmployment.Employer.PartyContactPoint.Address.ZipCode != null &&
                            spouseEmployment.Employer.PartyContactPoint.Address.ZipCode.ZipCodePrimary != null )
                        {
                            SpouseEmployerZipCodePrimary =
                                spouseEmployment.Employer.PartyContactPoint.Address.ZipCode.ZipCodePrimary;
                        }

                        if( spouseEmployment.Employer.PartyContactPoint.Address.ZipCode != null &&
                            spouseEmployment.Employer.PartyContactPoint.Address.ZipCode.ZipCodeExtended != null && 
                            spouseEmployment.Employer.PartyContactPoint.Address.ZipCode.ZipCodeExtended.Trim().Length >= 4 )
                        {
                            SpouseEmployerZipCodeExtended =
                                spouseEmployment.Employer.PartyContactPoint.Address.ZipCode.ZipCodeExtended.Trim().Substring( 0, 4 );
                        }

                        SpouseEmployedFlag = EmploymentStatusFlagFor( spouseEmployment.Status.Code );
                        SpouseNeverEmployedFlag = NeverEmployedStatusFlagFor( spouseEmployment.Status.Code );
                    }
                }
                else if( medicareEntitlement.GetType() == esrdEntitlement.GetType() )
                {
                    ESRDFlag = "Y";
                    esrdEntitlement = medicareEntitlement as ESRDEntitlement;

                    if ( esrdEntitlement == null ) return;

                    if ( esrdEntitlement.GroupHealthPlanCoverage != null )
                    {
                        ESRDGroupHealthPlanFlag =
                            esrdEntitlement.GroupHealthPlanCoverage.Code;
                    }
                    if( esrdEntitlement.KidneyTransplant != null )
                    {
                        KidneyTransplantFlag =
                            esrdEntitlement.KidneyTransplant.Code;
                    }
                    if( esrdEntitlement.DialysisTreatment != null )
                    {
                        ESRDDialysisTreatmentFlag =
                            esrdEntitlement.DialysisTreatment.Code;
                    }
                    TransplantDate = esrdEntitlement.TransplantDate;
                    DialysisDate = esrdEntitlement.DialysisDate;
                    if (esrdEntitlement.DialysisCenterName != null )
                    {
                        DialysisCenterName = esrdEntitlement.DialysisCenterName;
                    }
                    else
                    {
                        DialysisCenterName = String.Empty;
                    }
                    if ( esrdEntitlement.DialysisTrainingStartDate != DateTime.MinValue)
                    {
                          SelfDialysisTrainingStartDate =
                        esrdEntitlement.DialysisTrainingStartDate;
                    }
                  
                    ESRDWithinCoordinationPeriod =
                        esrdEntitlement.WithinCoordinationPeriod.Code;
                    ESRDProvisionApplies =
                        esrdEntitlement.ESRDandAgeOrDisability.Code;
                }
            }
        }

        private void ReadMSP2( Account Model_Account )
        {
            MedicareSecondaryPayor medicareSecondaryPayor = Model_Account.MedicareSecondaryPayor;

            MSPVersion = VERSION_2;

            // TLG 11/28/2006 per Prashant field mapping discussion

            // Pass the user who created or modified this MSP

            if( Model_Account.Activity != null
                && Model_Account.Activity.AppUser != null )
            {
                MSPUser = Model_Account.Activity.AppUser.PBAREmployeeID;
            }

            // The primary and secondary ins planids should be passed along with MSP
            // The default for InsuranceSelected is 0, and the following logic determines if 1 or 2 is applicable

            Coverage primaryCoverage = Model_Account.Insurance.CoverageFor( CoverageOrder.PRIMARY_OID );

            if( primaryCoverage != null )
            {
                InsurancePlanIdOne = primaryCoverage.InsurancePlan.PlanID;

                if( primaryCoverage.InsurancePlan.Payor.Code != MEDICARE
                    && primaryCoverage.InsurancePlan.Payor.Code != MEDICAID )
                {
                    InsuranceSelected = 1;
                }
                else
                {
                    Coverage secondaryCoverage = Model_Account.Insurance.CoverageFor( CoverageOrder.SECONDARY_OID );

                    if( secondaryCoverage != null )
                    {
                        InsurancePlanIdTwo = secondaryCoverage.InsurancePlan.PlanID;

                        if( secondaryCoverage.InsurancePlan.Payor.Code != MEDICARE
                            && secondaryCoverage.InsurancePlan.Payor.Code != MEDICAID )
                        {
                            InsuranceSelected = 2;
                        }
                    }
                }
            }

            // TLG 11/28/2006 per Prashant field mapping discussion

            if( Model_Account.Diagnosis != null
                && Model_Account.Diagnosis.Condition != null )
            {
                if( Model_Account.Diagnosis.Condition.GetType() == typeof( Illness ) )
                {
                    AccidentInjuryDate = ( ( Illness )Model_Account.Diagnosis.Condition ).DateOfOnset;
                }
                else if( Model_Account.Diagnosis.Condition.GetType() == typeof( Crime ) )
                {
                    AccidentInjuryDate = ( ( Crime )Model_Account.Diagnosis.Condition ).OccurredOn;
                }
                else if( Model_Account.Diagnosis.Condition.GetType() == typeof( Accident ) )
                {
                    AccidentInjuryDate = ( ( Accident )Model_Account.Diagnosis.Condition ).OccurredOn;
                }
            }

            if( medicareSecondaryPayor.SpecialProgram != null )
            {
                SpecialProgram specialProgram = medicareSecondaryPayor.SpecialProgram;

                BlackLungBenefitsFlag = specialProgram.BlackLungBenefits.Code;
                BlackLungDate = specialProgram.BLBenefitsStartDate;
                BlackLungTodaysVisitFlag = specialProgram.VisitForBlackLung.Code;
                GovernmentProgramFlag = specialProgram.GovernmentProgram.Code;
                DVAAuthorizedFlag = specialProgram.DVAAuthorized.Code;
                WorkRelatedFlag = specialProgram.WorkRelated.Code;
            }

            if( medicareSecondaryPayor.LiabilityInsurer != null )
            {
                LiabilityInsurer liabilityInsurer = medicareSecondaryPayor.LiabilityInsurer;

                NonWorkRelatedFlag = liabilityInsurer.NonWorkRelated.Code;
                // new fields follow
                NoFaultInsuranceAvailable = liabilityInsurer.NoFaultInsuranceAvailable.Code;
                LiabilityInsuranceAvailable = liabilityInsurer.LiabilityInsuranceAvailable.Code;
            }

            // initialize all to "N" since PBAR requires ..

            AgeFlag = "N";
            DisabilityFlag = "N";
            ESRDFlag = "N";

            if( medicareSecondaryPayor.MedicareEntitlement != null )
            {
                // fields on the parent class

                Employed = EmploymentStatusFlagFor( medicareSecondaryPayor.MedicareEntitlement.PatientEmployment.Status.Code );

                PatientNeverEmployedFlag = NeverEmployedStatusFlagFor(
                    medicareSecondaryPayor.MedicareEntitlement.PatientEmployment.Status.Code );

                GroupHealthPlan = medicareSecondaryPayor.MedicareEntitlement.GroupHealthPlanCoverage.Code;

                // entitlement-specific fields

                if( medicareSecondaryPayor.MedicareEntitlement.GetType() == typeof( AgeEntitlement ) )
                {
                    AgeEntitlement ageEntitlement
                        = medicareSecondaryPayor.MedicareEntitlement as AgeEntitlement;

                    if ( ageEntitlement == null ) return;

                    AgeFlag = "Y";

                    if( ageEntitlement.GroupHealthPlanCoverage != null )
                    {
                        AgeGroupHealthPlanCoverage =
                            ageEntitlement.GroupHealthPlanCoverage.Code;
                    }

                    if( ageEntitlement.GHPEmploysX != null )
                    {
                        AgeGroupHealthPlanCoverageLimitExceeded =
                            ageEntitlement.GHPEmploysX.Code;
                    }

                    if( ageEntitlement.PatientEmployment != null
                        && ageEntitlement.PatientEmployment.Status.Code == EmploymentStatus.NewRetired().Code )
                    {
                        RetireDate =
                            ageEntitlement.PatientEmployment.RetiredDate;
                    }

                    if( ageEntitlement.SpouseEmployment != null )
                    {
                        Employment spouseEmployment =
                            ageEntitlement.SpouseEmployment;

                        SpouseEmploymentRetireDate =
                            spouseEmployment.RetiredDate;
                        SpouseEmployerName =
                            spouseEmployment.Employer.Name;
                        if( spouseEmployment.Employer.PartyContactPoint.Address.Address1.Length > 25 )
                        {
                            SpouseEmployerAddress = 
                                spouseEmployment.Employer.PartyContactPoint.Address.Address1.Substring( 0, 25 );
                        }
                        else
                        {
                            SpouseEmployerAddress = 
                                spouseEmployment.Employer.PartyContactPoint.Address.Address1;
                        }
                        SpouseEmployerCity =
                            MSPFormattedCity( spouseEmployment.Employer.PartyContactPoint.Address.City );
                        SpouseEmployerState =
                            spouseEmployment.Employer.PartyContactPoint.Address.State.Code;

                        if( spouseEmployment.Employer.PartyContactPoint.Address.ZipCode != null &&
                            spouseEmployment.Employer.PartyContactPoint.Address.ZipCode.ZipCodePrimary != null )
                        {
                            SpouseEmployerZipCodePrimary =
                                spouseEmployment.Employer.PartyContactPoint.Address.ZipCode.ZipCodePrimary;
                        }

                        if( spouseEmployment.Employer.PartyContactPoint.Address.ZipCode != null &&
                            spouseEmployment.Employer.PartyContactPoint.Address.ZipCode.ZipCodeExtended != null && 
                            spouseEmployment.Employer.PartyContactPoint.Address.ZipCode.ZipCodeExtended.Trim().Length >= 4 )
                        {
                            SpouseEmployerZipCodeExtended =
                                spouseEmployment.Employer.PartyContactPoint.Address.ZipCode.ZipCodeExtended.Trim().Substring( 0, 4 );
                        }

                        SpouseEmployedFlag = EmploymentStatusFlagFor( spouseEmployment.Status.Code );
                        SpouseNeverEmployedFlag = NeverEmployedStatusFlagFor( spouseEmployment.Status.Code );
                    }

                    // new fields

                    if( ageEntitlement.GHPSpouseEmploysX.Code != null )
                    {
                        AgeEntitlementSpouseEmpMoreThanX = ageEntitlement.GHPSpouseEmploysX.Code;
                    }

                    if( ageEntitlement.GroupHealthPlanCoverage.Code != null
                        && ageEntitlement.GroupHealthPlanType != null
                        && ageEntitlement.GroupHealthPlanCoverage.Code == YesNoFlag.CODE_YES )
                    {
                        if( ageEntitlement.GroupHealthPlanType.Oid == GroupHealthPlanType.SELF_OID )
                        {
                            AgeEntitlementGHPType = GroupHealthPlanType.SelfCode;
                        }
                        else if( ageEntitlement.GroupHealthPlanType.Oid == GroupHealthPlanType.SPOUSE_OID )
                        {
                            AgeEntitlementGHPType = GroupHealthPlanType.SpouseCode;
                        }
                        else if( ageEntitlement.GroupHealthPlanType.Oid == GroupHealthPlanType.BOTH_OID )
                        {
                            AgeEntitlementGHPType = GroupHealthPlanType.BothCode;
                        }
                    }
                }
                else if( medicareSecondaryPayor.MedicareEntitlement.GetType() == typeof( DisabilityEntitlement ) )
                {
                    DisabilityEntitlement disabilityEntitlement
                        = medicareSecondaryPayor.MedicareEntitlement as DisabilityEntitlement;

                    if ( disabilityEntitlement == null ) return;

                    DisabilityFlag = "Y";

                    // TLG 11/28/2006 per Prashant field mapping discussion

                    if( disabilityEntitlement.FamilyMemberGHPFlag != null )
                    {
                        OtherEmployed = disabilityEntitlement.FamilyMemberGHPFlag.Code;
                    }

                    if( disabilityEntitlement.FamilyMemberGHPFlag.Code == YesNoFlag.CODE_YES
                        && disabilityEntitlement.FamilyMemberEmployment != null )
                    {
                        Employment familyMemberEmployment =
                            disabilityEntitlement.FamilyMemberEmployment;

                        FamilyMemberEmployerName =
                            familyMemberEmployment.Employer.Name;
                        if( familyMemberEmployment.Employer.PartyContactPoint.Address.Address1.Length > 25 )
                        {
                            FamilyMemberEmployerAddress = 
                                familyMemberEmployment.Employer.PartyContactPoint.Address.Address1.Substring( 0, 25 );
                        }
                        else
                        {
                            FamilyMemberEmployerAddress = 
                                familyMemberEmployment.Employer.PartyContactPoint.Address.Address1;
                        }
                        FamilyMemberEmployerCity =
                            MSPFormattedCity( familyMemberEmployment.Employer.PartyContactPoint.Address.City );
                        FamilyMemberEmployerState =
                            familyMemberEmployment.Employer.PartyContactPoint.Address.State.Code;

                        if( familyMemberEmployment.Employer.PartyContactPoint.Address.ZipCode != null &&
                            familyMemberEmployment.Employer.PartyContactPoint.Address.ZipCode.ZipCodePrimary != null )
                        {
                            FamilyMemberEmployerZipCodePrimary =
                                familyMemberEmployment.Employer.PartyContactPoint.Address.ZipCode.ZipCodePrimary;
                        }

                        if( familyMemberEmployment.Employer.PartyContactPoint.Address.ZipCode != null &&
                            familyMemberEmployment.Employer.PartyContactPoint.Address.ZipCode.ZipCodeExtended != null && 
                            familyMemberEmployment.Employer.PartyContactPoint.Address.ZipCode.ZipCodeExtended.Trim().Length >= 4 )
                        {
                            FamilyMemberEmployerZipCodeExtended =
                                familyMemberEmployment.Employer.PartyContactPoint.Address.ZipCode.ZipCodeExtended.Trim().Substring( 0, 4 );
                        }
                    }

                    if( disabilityEntitlement.GroupHealthPlanCoverage != null )
                    {
                        DisabilityGroupHealthPlanCoverageFlag =
                            disabilityEntitlement.GroupHealthPlanCoverage.Code;
                    }

                    if( disabilityEntitlement.GHPEmploysMoreThanXFlag != null )
                    {
                        DisabilityGHPCoverageLimitExceededFlag =
                            disabilityEntitlement.GHPEmploysMoreThanXFlag.Code;
                    }

                    if( disabilityEntitlement.PatientEmployment != null
                        && disabilityEntitlement.PatientEmployment.Status.Code == EmploymentStatus.NewRetired().Code )
                    {
                        RetireDate =
                            disabilityEntitlement.PatientEmployment.RetiredDate;
                    }

                    if( disabilityEntitlement.SpouseEmployment != null )
                    {
                        Employment spouseEmployment =
                            disabilityEntitlement.SpouseEmployment;

                        SpouseEmploymentRetireDate =
                            spouseEmployment.RetiredDate;
                        SpouseEmployerName =
                            spouseEmployment.Employer.Name;
                        if( spouseEmployment.Employer.PartyContactPoint.Address.Address1.Length > 25 )
                        {
                            SpouseEmployerAddress = 
                                spouseEmployment.Employer.PartyContactPoint.Address.Address1.Substring( 0, 25 );
                        }
                        else
                        {
                            SpouseEmployerAddress = 
                                spouseEmployment.Employer.PartyContactPoint.Address.Address1;
                        }
                        SpouseEmployerCity =
                            MSPFormattedCity( spouseEmployment.Employer.PartyContactPoint.Address.City );
                        SpouseEmployerState =
                            spouseEmployment.Employer.PartyContactPoint.Address.State.Code;

                        if( spouseEmployment.Employer.PartyContactPoint.Address.ZipCode != null &&
                            spouseEmployment.Employer.PartyContactPoint.Address.ZipCode.ZipCodePrimary != null )
                        {
                            SpouseEmployerZipCodePrimary =
                                spouseEmployment.Employer.PartyContactPoint.Address.ZipCode.ZipCodePrimary;
                        }

                        if( spouseEmployment.Employer.PartyContactPoint.Address.ZipCode != null &&
                            spouseEmployment.Employer.PartyContactPoint.Address.ZipCode.ZipCodeExtended != null && 
                            spouseEmployment.Employer.PartyContactPoint.Address.ZipCode.ZipCodeExtended.Trim().Length >= 4 )
                        {
                            SpouseEmployerZipCodeExtended =
                                spouseEmployment.Employer.PartyContactPoint.Address.ZipCode.ZipCodeExtended.Trim().Substring( 0, 4 );
                        }

                        SpouseEmployedFlag = EmploymentStatusFlagFor( spouseEmployment.Status.Code );
                        SpouseNeverEmployedFlag = NeverEmployedStatusFlagFor( spouseEmployment.Status.Code );
                    }

                    // new fields

                    if( disabilityEntitlement.SpouseGHPEmploysMoreThanXFlag != null )
                    {
                        DisabilityEntitlementSpouseEmpMoreThanX
                            = disabilityEntitlement.SpouseGHPEmploysMoreThanXFlag.Code;
                    }

                    if( disabilityEntitlement.FamilyMemberGHPEmploysMoreThanXFlag != null )
                    {
                        DisabilityEntitlementFamiliyMemberEmpMoreThanX
                            = disabilityEntitlement.FamilyMemberGHPEmploysMoreThanXFlag.Code;
                    }
                    if( disabilityEntitlement.GroupHealthPlanCoverage != null
                        && disabilityEntitlement.GroupHealthPlanType != null
                        && disabilityEntitlement.GroupHealthPlanCoverage.Code == YesNoFlag.CODE_YES )
                    {
                        if( disabilityEntitlement.GroupHealthPlanType.Oid == GroupHealthPlanType.SELF_OID )
                        {
                            DisabilityEntitlementGHPType = GroupHealthPlanType.SelfCode;
                        }
                        else if( disabilityEntitlement.GroupHealthPlanType.Oid == GroupHealthPlanType.SPOUSE_OID )
                        {
                            DisabilityEntitlementGHPType = GroupHealthPlanType.SpouseCode;
                        }
                        else if( disabilityEntitlement.GroupHealthPlanType.Oid == GroupHealthPlanType.BOTH_OID )
                        {
                            DisabilityEntitlementGHPType = GroupHealthPlanType.BothCode;
                        }
                    }
                }
                else if( medicareSecondaryPayor.MedicareEntitlement.GetType() == typeof( ESRDEntitlement ) )
                {
                    ESRDEntitlement esrdEntitlement
                        = medicareSecondaryPayor.MedicareEntitlement as ESRDEntitlement;

                    if ( esrdEntitlement == null ) return;

                    ESRDFlag = "Y";

                    if (esrdEntitlement.GroupHealthPlanCoverage != null)
                    {
                        ESRDGroupHealthPlanFlag = esrdEntitlement.GroupHealthPlanCoverage.Code;
                    }
                    if( esrdEntitlement.KidneyTransplant != null )
                    {
                        KidneyTransplantFlag = esrdEntitlement.KidneyTransplant.Code;
                    }

                    if( esrdEntitlement.DialysisTreatment != null )
                    {
                        ESRDDialysisTreatmentFlag = esrdEntitlement.DialysisTreatment.Code;
                    }

                    TransplantDate = esrdEntitlement.TransplantDate;
                    DialysisDate = esrdEntitlement.DialysisDate;
                    if (esrdEntitlement.DialysisCenterName != null)
                    {
                        DialysisCenterName = esrdEntitlement.DialysisCenterName;
                    }
                    else
                    {
                        DialysisCenterName = String.Empty;
                    }
                    if (esrdEntitlement.DialysisTrainingStartDate != DateTime.MinValue)
                    {
                        SelfDialysisTrainingStartDate = esrdEntitlement.DialysisTrainingStartDate;
                    }

                    if( esrdEntitlement.WithinCoordinationPeriod != null )
                    {
                        ESRDWithinCoordinationPeriod = esrdEntitlement.WithinCoordinationPeriod.Code;
                    }

                    // these were wrong in or not mapped in the previous version!!!

                    if( esrdEntitlement.ESRDandAgeOrDisability != null )
                    {
                        ESRDMultiEntitlement = esrdEntitlement.BasedOnAgeOrDisability.Code;
                    }

                    if( esrdEntitlement.BasedOnESRD != null )
                    {
                        ESRDInitialEntitlement = esrdEntitlement.BasedOnESRD.Code;
                    }

                    if( esrdEntitlement.ProvisionAppliesFlag != null )
                    {
                        ESRDProvisionApplies = esrdEntitlement.ProvisionAppliesFlag.Code;
                    }
                }
            }
        }

        private void ReadAuthorizationDetails(Coverage coverage)
        {
            Authorization authorization = null;
            if (coverage.GetType().IsSubclassOf(typeof(CoverageGroup)))
            {
                CoverageGroup coverageGroup = coverage as CoverageGroup;
                authorization = coverageGroup.Authorization;
            }

            if (coverage.GetType() == typeof(GovernmentMedicareCoverage))
            {
                GovernmentMedicareCoverage medicareCoverage = coverage as GovernmentMedicareCoverage;
                authorization = medicareCoverage.Authorization;
            }

            if (authorization != null)
            {
                AuthorizationCompanyName = authorization.AuthorizationCompany;
                if (authorization.AuthorizationPhone != null &&
                    authorization.AuthorizationPhone.ToString().Length > 0)
                {
                    AuthorizationCompanyPhoneNumber = Convert.ToInt64(authorization.AuthorizationPhone.ToString());
                }

                if (authorization.AuthorizationRequired != null)
                {
                    // PBAR stores "YES" instead of "Y", so do the translation
                    if (authorization.AuthorizationRequired.IsYes)
                    {
                        AuthorizationRequired = "YES";
                    }
                    else
                    {
                        AuthorizationRequired = authorization.AuthorizationRequired.Code;
                    }
                }

                AuthorizationCompanyPhoneNumberExtension = authorization.PromptExt;
                if (authorization.NameOfCompanyRepresentative != null)
                {
                    AuthCompanyRepFirstName = authorization.NameOfCompanyRepresentative.FirstName;
                    AuthCompanyRepLastName = authorization.NameOfCompanyRepresentative.LastName;
                }

                ServicesAuthorized = authorization.ServicesAuthorized;
                EffectiveDateOfAuthorization = authorization.EffectiveDate;
                ExpirationDateOfAuthorization = authorization.ExpirationDate;
                if (authorization.AuthorizationStatus != null)
                {
                    AuthorizationStatus = authorization.AuthorizationStatus.Code;
                }

                AuthorizationRemarks = authorization.Remarks;
            }
        }

        private void ReadFinancialCounselingDetails( Account account )
        {
            bool isUninsured = false;
            IFinancialClassesBroker broker = BrokerFactory.BrokerOfType<IFinancialClassesBroker>();
            isUninsured = broker.IsUninsured( account.Facility.Oid,account.FinancialClass );
            if( account.Patient != null )
            {
                PreviousBalanceDue = account.Patient.PreviousBalanceDue;
            }
            if( account.Insurance != null )
            {
                CopayDueAmount = account.Insurance.PrimaryCopay;
                DeductibleDueAmount = account.Insurance.PrimaryDeductible;
            }

            DayOfMonthPayementDue = account.DayOfMonthPaymentDue;

            if( isUninsured )
            {
                NumberOfMonthlyPaymentsUnInsured = account.NumberOfMonthlyPayments;
                MonthlyPaymentUnInsured = account.MonthlyPayment;
                TotalPaymentsCollectedForUnInsuredCurrentAccount = account.TotalPaid;
                TotalUninsuredAmountDue = account.TotalCurrentAmtDue;
                if( account.ResourceListProvided != null )
                {
                    ResourceListProvided = account.ResourceListProvided.Code.Trim();
                }
            }
            else
            {
                TotalCurrentAmountDue = account.TotalCurrentAmtDue;
                NumberOfMonthlyPaymentsInsured = account.NumberOfMonthlyPayments;
                MonthlyPaymentInsured = account.MonthlyPayment;
                TotalPaymentsCollectedForInsuredCurrentAccount = account.TotalPaid;
            }
        }

        private static string EmploymentStatusFlagFor( string empStatusCode )
        {
            string employedStatusFlag = " ";

            if( empStatusCode == EmploymentStatus.EMPLOYED_FULL_TIME_CODE ||
                empStatusCode == EmploymentStatus.EMPLOYED_PART_TIME_CODE )
            {
                employedStatusFlag = YES_FLAG;
            }
            else if( empStatusCode == EmploymentStatus.RETIRED_CODE )
            {
                employedStatusFlag = NO_FLAG;
            }
            else if( empStatusCode == EmploymentStatus.NOT_EMPLOYED_CODE )
            {
                employedStatusFlag = NO_FLAG;
            }

            return employedStatusFlag;
        }

        private static string NeverEmployedStatusFlagFor( string empStatusCode )
        {
            string neverEmployedFlag = " ";

            if( empStatusCode == EmploymentStatus.EMPLOYED_FULL_TIME_CODE ||
                empStatusCode == EmploymentStatus.EMPLOYED_PART_TIME_CODE )
            {
                neverEmployedFlag = NO_FLAG;
            }
            else if( empStatusCode == EmploymentStatus.RETIRED_CODE )
            {
                neverEmployedFlag = NO_FLAG;
            }
            else if( empStatusCode == EmploymentStatus.NOT_EMPLOYED_CODE )
            {
                neverEmployedFlag = YES_FLAG;
            }

            return neverEmployedFlag;
        }

        private static string MSPFormattedCity( string city )
        {
            if( city.Trim().Length > 17 )
            {
                return city.Trim().Substring( 0, 17 );
            }
            else
            {
                return city.Trim();
            }
        }

        private void ReadAuthorizationDetailsForMedicare53544(Coverage aCoverage)
        {
            var covGroup = aCoverage as GovernmentMedicareCoverage;
            if (covGroup != null && covGroup.Authorization != null && 
                covGroup.IsMedicareCoverageValidForAuthorization )
            {
                if (covGroup.Authorization.NameOfCompanyRepresentative != null)
                {
                    AuthCompanyRepFirstName = covGroup.Authorization.NameOfCompanyRepresentative.FirstName;
                    AuthCompanyRepLastName = covGroup.Authorization.NameOfCompanyRepresentative.LastName;
                }
                ServicesAuthorized = covGroup.Authorization.ServicesAuthorized;
                EffectiveDateOfAuthorization = covGroup.Authorization.EffectiveDate;
                ExpirationDateOfAuthorization = covGroup.Authorization.ExpirationDate;
                if (covGroup.Authorization.AuthorizationStatus != null)
                {
                    AuthorizationStatus = covGroup.Authorization.AuthorizationStatus.Code;
                }
                AuthorizationRemarks = TrimCoverageRemarks(
                    covGroup.Authorization.Remarks, AUTHORIZATION_REMARKS_LENGTH);
            }
        }

        #endregion

        #region Public Properties
        public string OriginalID
        {
            set
            {
                insuranceDetailsOrderedList[APOID] = value;
            }
        }

        #endregion

        #region Private Properties


        private int HospitalNumber
        {
            set
            {
                insuranceDetailsOrderedList[APHSP_] = value;
            }
        }


        public string PriorityCode
        {
            set
            {
                insuranceDetailsOrderedList[APPTY] = value;
            }
        }
        private int PlanCode
        {
            set
            {
                insuranceDetailsOrderedList[APPLAN] = value;
            }
        }
        private string UBF1PlanId
        {
            set
            {
                insuranceDetailsOrderedList[APPNID] = value;
            }
        }
        private string PlanId
        {
            set
            {
                insuranceDetailsOrderedList[APINS_] = value;
            }
        }
        private string CertSSNID
        {
            set
            {
                insuranceDetailsOrderedList[APIID_] = value;
            }
        }
        private string MBINUmber
        {
            set
            {
                insuranceDetailsOrderedList[APMBI] = value;
            }
        }
        private string SignedOverMedicareHICNumber
        {
            set
            {
                insuranceDetailsOrderedList[APHIC_] = value;
            }
        }
        private string ConditionOfService
        {
            set
            {
                insuranceDetailsOrderedList[APRICF] = value;
            }
        }

        private string CityStatePlan
        {
            set
            {
                insuranceDetailsOrderedList[APCSPL] = value;
            }
        }

        private string InsuranceName
        {
            set
            {
                insuranceDetailsOrderedList[APINM] = value;
            }
        }

        private string BillingCareOfName
        {
            set
            {
                insuranceDetailsOrderedList[APCONM] = value;
            }
        }

        private string BillingName
        {
            set
            {
                insuranceDetailsOrderedList[APBLNM] = value;
            }
        }

        private string GroupName
        {
            set
            {
                insuranceDetailsOrderedList[APGNM] = value;
            }
        }

        private string BillingMailingAddress1
        {
            set
            {
                insuranceDetailsOrderedList[APIAD1] = value;
            }
        }

        private string BillingMailingAddress2
        {
            set
            {
                insuranceDetailsOrderedList[APIAD2] = value;
            }
        }
        private string BillingMailingAddress3
        {
            set
            {
                insuranceDetailsOrderedList[APIAD3] = value;
            }
        }
        private int StartDate
        {
            set
            {
                insuranceDetailsOrderedList[APSDAT] = value;
            }
        }

        private int DaysOfFullCoverage
        {
            set
            {
                insuranceDetailsOrderedList[APFDAY] = value;
            }
        }

        private int DaysOfCoInsurance
        {
            set
            {
                insuranceDetailsOrderedList[APCDAY] = value;
            }
        }

        private int SequenceNumber
        {
            set
            {
                insuranceDetailsOrderedList[APSEQ_] = value;
            }
        }

        private int GuarantorNumber
        {
            set
            {
                insuranceDetailsOrderedList[APGAR_] = value;
            }
        }

        private string SubscriberLastName
        {
            set
            {
                insuranceDetailsOrderedList[APSLNM] = value;
            }
        }

        private string SubscriberFirstName
        {
            set
            {
                insuranceDetailsOrderedList[APSFNM] = value;
            }
        }
        private string SubscriberSuffix
        {
            set
            {
                insuranceDetailsOrderedList[APSGNRN] = value;
            }
        }
        private string SubscriberSex
        {
            set
            {
                insuranceDetailsOrderedList[APSSEX] = value;
            }
        }

        private string InfoReceivedSource
        {
            set
            {
                insuranceDetailsOrderedList[APIFRF] = value;
            }
        }
        private string InsuranceGroupNumber
        {
            set
            {
                insuranceDetailsOrderedList[APGRPN] = value;
            }
        }


        private string SubscriberEmployerName
        {
            // TLG 05/30/2006 changed from APSBEN to APENM1
            set
            {
                insuranceDetailsOrderedList[APENM1] = value;
            }
        }

        private string EmployeeID
        {
            set
            {
                insuranceDetailsOrderedList[APEEID] = value;
            }
        }

        private string SubscriberAddress
        {
            set
            {
                insuranceDetailsOrderedList[APJADR] = value;
            }
        }

        private string SubscriberAddress1
        {
            set
            {
                insuranceDetailsOrderedList[APJADRE1] = value;
            }
        }

        private string SubscriberAddress2
        {
            set
            {
                insuranceDetailsOrderedList[APJADRE2] = value;
            }
        }

        private string SubscriberCity
        {
            set
            {
                insuranceDetailsOrderedList[APJCIT] = value;
            }
        }

        private string SubscriberStateCode
        {
            set
            {
                insuranceDetailsOrderedList[APJSTE] = value;
            }
        }

        private string SubscriberZip
        {
            // TLG 05/30/2006 Changed from APJZIP to APJZPA
            set
            {
                insuranceDetailsOrderedList[APJZPA] = value;
            }
        }

        private string SubscriberZipExt
        {
            // TLG 05/30/2006 Changed from APJZP4 to APJZ4A
            set
            {
                insuranceDetailsOrderedList[APJZ4A] = value;
            }
        }

        private int SubscriberAreaCode
        {
            set
            {
                insuranceDetailsOrderedList[APJACD] = value;
            }
        }

        private int SubscriberPhoneNumber
        {
            set
            {
                insuranceDetailsOrderedList[APJPH_] = value;
            }
        }



        private string OtherEmployerDataID1
        {
            set
            {
                insuranceDetailsOrderedList[APEDC1] = value;
            }
        }

        private string OtherEmploymentStatus1
        {
            set
            {
                insuranceDetailsOrderedList[APESC1] = value;
            }
        }

        private string OEmployerAddress1
        {
            set
            {
                insuranceDetailsOrderedList[APEA01] = value;
            }
        }

        private int OEmployerZip1
        {
            set
            {
                insuranceDetailsOrderedList[APEZ01] = value;
            }
        }

        private string OtherEmployerName2
        {
            set
            {
                insuranceDetailsOrderedList[APENM2] = value;
            }
        }

        private string OtherEmployerLocation2
        {
            set
            {
                insuranceDetailsOrderedList[APELO2] = value;
            }
        }

        private string OtherEmployerDataID2
        {
            set
            {
                insuranceDetailsOrderedList[APEDC2] = value;
            }
        }

        private string OtherEmploymentStatus2
        {
            set
            {
                insuranceDetailsOrderedList[APESC2] = value;
            }
        }

        private string OtherEmployerAddress2
        {
            set
            {
                insuranceDetailsOrderedList[APEA02] = value;
            }
        }

        private int OtherEmployerZip2
        {
            set
            {
                insuranceDetailsOrderedList[APEZ02] = value;
            }
        }

        private DateTime TransactionDate
        {
            set
            {
                insuranceDetailsOrderedList[APTDAT] = ConvertDateToIntInMddyyFormat( value );
            }
        }

        private string PlanName
        {
            set
            {
                insuranceDetailsOrderedList[APP_NM] = value;
            }
        }

        private DateTime PlanApprovedOn
        {
            set
            {
                insuranceDetailsOrderedList[APPLAD] = ConvertDateToIntInyyMMddFormat( value );
            }
        }
        private DateTime PlanEffectiveOn
        {
            set
            {
                insuranceDetailsOrderedList[APCBGD] = ConvertDateToIntInyyMMddFormat( value );
            }
        }
        private DateTime PlanTerminatedOn
        {
            set
            {
                insuranceDetailsOrderedList[APTDOC] = ConvertDateToIntInyyMMddFormat( value );
            }
        }





        private long EmployerCode1
        {
            // TLG 08/02/2006 added the multiply by 100000 to right-pad the value with 5 trailing zeroes,
            // as this is what PBAR is expecting!!! (Per Prashant phone call)
            set
            {
                insuranceDetailsOrderedList[APGCD1] = value * 100000;
            }
        }

        private long EmployerCode2
        {
            // TLG 08/02/2006 added the multiply by 100000 to right-pad the value with 5 trailing zeroes,
            // as this is what PBAR is expecting!!! (Per Prashant phone call)
            set
            {
                insuranceDetailsOrderedList[APGCD2] = value * 100000;
            }
        }

        private string InsuredEmpStreet1
        {
            set
            {
                insuranceDetailsOrderedList[APEA01] = value;
            }
        }

        private string InsuredEmpStreet2
        {
            set
            {
                insuranceDetailsOrderedList[APEA02] = value;
            }
        }

        private string InsuredEmpCityState1
        {
            set
            {
                insuranceDetailsOrderedList[APELO1] = value;
            }
        }

        private string InsuredEmpCityState2
        {
            set
            {
                insuranceDetailsOrderedList[APELO2] = value;
            }
        }

        private string InsuredEmpPhone1
        {
            set
            {
                if( value == null || value.Trim().Equals( string.Empty ) )
                {
                    insuranceDetailsOrderedList[APEID1] = "00000000000";
                }
                else
                {
                    insuranceDetailsOrderedList[APEID1] = value;
                }
            }
        }

        private string InsuredEmpPhone2
        {
            set
            {
                if( value == null || value.Trim().Equals( string.Empty ) )
                {
                    insuranceDetailsOrderedList[APEID2] = "00000000000";
                }
                else
                {
                    insuranceDetailsOrderedList[APEID2] = value;
                }
            }
        }

        private string InsuredEmpZip1
        {
            set
            {
                insuranceDetailsOrderedList[APEZP1] = value;
            }
        }

        private string InsuredEmpZipExt1
        {
            set
            {
                insuranceDetailsOrderedList[APEZ41] = value;
            }
        }

        private string InsuredEmpZip2
        {
            set
            {
                insuranceDetailsOrderedList[APEZP2] = value;
            }
        }

        private string InsuredEmpZipExt2
        {
            set
            {
                insuranceDetailsOrderedList[APEZ42] = value;
            }
        }

        private string MedicalRecNumber
        {
            set
            {
                insuranceDetailsOrderedList[APMR_] = value;
            }
        }

        private string SubscriberMInitial
        {
            set
            {
                insuranceDetailsOrderedList[APSMI] = value;
            }
        }

        private string SubscriberID
        {
            set
            {
                insuranceDetailsOrderedList[APSBID] = value;
            }
        }

        private string SubscriberCountryCode
        {
            set
            {
                insuranceDetailsOrderedList[APSCUN] = value;
            }
        }

        private string SubscriberCellPhone
        {
            set
            {
                insuranceDetailsOrderedList[APICPH] = value;
            }
        }

        private DateTime TimeRecordCreation
        {
            set
            {
                insuranceDetailsOrderedList[APTTME] = ConvertTimeToIntInHHmmSSFormat( value );
            }
        }

        //next two fields used for Pre-MSE and Post-MSE only. Should be empty for others.
        private long PreMSECopiedAccountNumber
        {
            set
            {
                insuranceDetailsOrderedList[APPACCT] = value;
            }
        }

        private string MSEActivityFlag
        {
            set
            {
                insuranceDetailsOrderedList[APACTF] = value;
            }
        }

        #region Medicare Secondary Payor related

        private string BlackLungBenefitsFlag
        {
            set
            {
                insuranceDetailsOrderedList[AP_BL] = value;
            }
        }

        private string BlackLungTodaysVisitFlag
        {
            set
            {
                insuranceDetailsOrderedList[AP_BLTDY] = value;
            }
        }

        private string GovernmentProgramFlag
        {
            set
            {
                insuranceDetailsOrderedList[AP_GP] = value;
            }
        }

        private string DVAAuthorizedFlag
        {
            set
            {
                insuranceDetailsOrderedList[AP_VA] = value;
            }
        }

        private string WorkRelatedFlag
        {
            set
            {
                insuranceDetailsOrderedList[AP_WA] = value;
            }
        }

        private string NonWorkRelatedFlag
        {
            set
            {
                insuranceDetailsOrderedList[AP_NA] = value;
            }
        }

        private string NoFaultInsuranceAvailable
        {
            set
            {
                insuranceDetailsOrderedList[AP_NFALT] = value;
            }
        }

        private string LiabilityInsuranceAvailable
        {
            set
            {
                insuranceDetailsOrderedList[AP_LIABL] = value;
            }
        }

        private string AgeEntitlementGHPType
        {
            set
            {
                insuranceDetailsOrderedList[AP_AGGHPF] = value;
            }
        }

        private string AgeEntitlementSpouseEmpMoreThanX
        {
            set
            {
                insuranceDetailsOrderedList[AP_AGHP20P] = value;
            }
        }

        private string DisabilityEntitlementGHPType
        {
            set
            {
                insuranceDetailsOrderedList[AP_DIGHPF] = value;
            }
        }

        private string DisabilityEntitlementSpouseEmpMoreThanX
        {
            set
            {
                insuranceDetailsOrderedList[AP_DGHP00P] = value;
            }
        }

        private string DisabilityEntitlementFamiliyMemberEmpMoreThanX
        {
            set
            {
                insuranceDetailsOrderedList[AP_DGHP00F] = value;
            }
        }

        private string AnotherPartyResponsibilityFlag
        {
            set
            {
                insuranceDetailsOrderedList[AP_OP] = value;
            }
        }

        private string MSPUser
        {
            set
            {
                insuranceDetailsOrderedList[AP_USER] = value;
            }
        }

        private string MSPVersion
        {
            set
            {
                insuranceDetailsOrderedList[AP_FORMVER] = value;
            }
        }

        private string AgeFlag
        {
            set
            {
                insuranceDetailsOrderedList[AP_AG] = value;
            }
        }

        private string Employed
        {
            set
            {
                insuranceDetailsOrderedList[AP_EMPL] = value;
            }
        }

        private string SpouseEmployedFlag
        {
            set
            {
                insuranceDetailsOrderedList[AP_SEMPL] = value;
            }
        }

        private string PatientNeverEmployedFlag
        {
            set
            {
                insuranceDetailsOrderedList[AP_EMPLN] = value;
            }
        }

        private string SpouseNeverEmployedFlag
        {
            set
            {
                insuranceDetailsOrderedList[AP_SEMPLN] = value;
            }
        }

        private string AgeGroupHealthPlanCoverage
        {
            set
            {
                insuranceDetailsOrderedList[AP_AGGHP] = value;
            }
        }

        private string AgeGroupHealthPlanCoverageLimitExceeded
        {
            set
            {
                insuranceDetailsOrderedList[AP_AGGHP20] = value;
            }
        }

        private string OtherEmployed
        {
            set
            {
                insuranceDetailsOrderedList[AP_OEMPL] = value;
            }
        }
        private string DisabilityFlag
        {
            set
            {
                insuranceDetailsOrderedList[AP_DI] = value;
            }
        }
        private string DisabilityGroupHealthPlanCoverageFlag
        {
            set
            {
                insuranceDetailsOrderedList[AP_DIGHP] = value;
            }
        }
        private string DisabilityGHPCoverageLimitExceededFlag
        {
            set
            {
                insuranceDetailsOrderedList[AP_DIGHP00] = value;
            }
        }
        private string ESRDFlag
        {
            set
            {
                insuranceDetailsOrderedList[AP_ES] = value;
            }
        }

        private string ESRDGroupHealthPlanFlag
        {
            set
            {
                insuranceDetailsOrderedList[AP_ESGHP] = value;
            }
        }

        private string KidneyTransplantFlag
        {
            set
            {
                insuranceDetailsOrderedList[AP_ESKT] = value;
            }
        }

        private string ESRDDialysisTreatmentFlag
        {
            set
            {
                insuranceDetailsOrderedList[AP_ESDT] = value;
            }
        }
        private string ESRDWithinCoordinationPeriod
        {
            set
            {
                insuranceDetailsOrderedList[AP_ES30MO] = value;
            }
        }
        private string ESRDMultiEntitlement
        {
            set
            {
                insuranceDetailsOrderedList[AP_ESMULTI] = value;
            }
        }
        private string ESRDInitialEntitlement
        {
            set
            {
                insuranceDetailsOrderedList[AP_ESIA] = value;
            }
        }
        private string ESRDProvisionApplies
        {
            set
            {
                insuranceDetailsOrderedList[AP_ESOTHR] = value;
            }
        }
        private string GroupHealthPlan
        {
            set
            {
                insuranceDetailsOrderedList[AP_GHP] = value;
            }
        }
        private string AutoAccidentType
        {
            set
            {
                insuranceDetailsOrderedList[AP_AUTO] = value;
            }
        }
        private int InsuranceSelected
        {
            set
            {
                insuranceDetailsOrderedList[AP_INS_] = value;
            }
        }
        private string InsurancePlanIdOne
        {
            set
            {
                insuranceDetailsOrderedList[AP_PNID1] = value;
            }
        }
        private string InsurancePlanIdTwo
        {
            set
            {
                insuranceDetailsOrderedList[AP_PNID2] = value;
            }
        }
        private DateTime BlackLungDate
        {
            set
            {
                insuranceDetailsOrderedList[AP_BLDT] =
                    ConvertDateToStringInyyyyMMddFormat( value );
            }
        }
        private DateTime TransplantDate
        {
            set
            {
                insuranceDetailsOrderedList[AP_ESKTDT] =
                    ConvertDateToStringInyyyyMMddFormat( value );
            }
        }
        private DateTime AccidentInjuryDate
        {
            set
            {

                insuranceDetailsOrderedList[AP_AIDT] =
                    ConvertDateToStringInyyyyMMddFormat( value );
            }
        }

        private DateTime DialysisDate
        {
            set
            {
                insuranceDetailsOrderedList[AP_ESDTDT] =
                    ConvertDateToStringInyyyyMMddFormat( value );
            }
        }
        private String DialysisCenterName
        {
            set 
            {
                insuranceDetailsOrderedList[APDIALCTR] = value;
            }
        }
        private DateTime SelfDialysisTrainingStartDate
        {
            set
            {
                insuranceDetailsOrderedList[AP_ESSDTDT] =
                    ConvertDateToStringInyyyyMMddFormat( value );
            }
        }
        private DateTime RetireDate
        {
            set
            {
                insuranceDetailsOrderedList[AP_EMPLRDT] =
                    ConvertDateToStringInyyyyMMddFormat( value );
            }
        }
        private string WorkStationId
        {
            set
            {
                insuranceDetailsOrderedList[APIDWS] = value;
            }
        }
        private DateTime SpouseEmploymentRetireDate
        {
            set
            {
                insuranceDetailsOrderedList[AP_SEMPRDT] =
                    ConvertDateToStringInyyyyMMddFormat( value );
            }
        }

        private string SpouseEmployerName
        {
            set
            {
                insuranceDetailsOrderedList[AP_SENM] = value;
            }
        }
        private string SpouseEmployerAddress
        {
            set
            {
                insuranceDetailsOrderedList[AP_SEADR] = value;
            }
        }
        private string SpouseEmployerCity
        {
            set
            {
                insuranceDetailsOrderedList[AP_SECIT] = value;
            }
        }
        private string SpouseEmployerState
        {
            set
            {
                insuranceDetailsOrderedList[AP_SESTE] = value;
            }
        }

        private string SpouseEmployerZipCodePrimary
        {
            set
            {
                insuranceDetailsOrderedList[AP_SEZIP] = value;
            }
        }
        private string SpouseEmployerZipCodeExtended
        {
            set
            {
                insuranceDetailsOrderedList[AP_SEZP4] = value;
            }
        }

        // Family Member Employment Details
        private string FamilyMemberEmployerName
        {
            set
            {
                insuranceDetailsOrderedList[AP_FENM] = value;
            }
        }
        private string FamilyMemberEmployerAddress
        {
            set
            {
                insuranceDetailsOrderedList[AP_FEADR] = value;
            }
        }
        private string FamilyMemberEmployerCity
        {
            set
            {
                insuranceDetailsOrderedList[AP_FECIT] = value;
            }
        }
        private string FamilyMemberEmployerState
        {
            set
            {
                insuranceDetailsOrderedList[AP_FESTE] = value;
            }
        }

        private string FamilyMemberEmployerZipCodePrimary
        {
            set
            {
                insuranceDetailsOrderedList[AP_FEZIP] = value;
            }
        }
        private string FamilyMemberEmployerZipCodeExtended
        {
            set
            {
                insuranceDetailsOrderedList[AP_FEZP4] = value;
            }
        }

        // Attorney Details
        private string AttorneyName
        {
            set
            {
                insuranceDetailsOrderedList[APATNM] = value;
            }
        }
        private string AttorneyStreet
        {
            set
            {
                insuranceDetailsOrderedList[APATST] = value;
            }
        }
        private string AttorneyCity
        {
            set
            {
                insuranceDetailsOrderedList[APATCT] = value;
            }
        }
        private string AttorneyState
        {
            set
            {
                insuranceDetailsOrderedList[APATSA] = value;
            }
        }
        private string AttorneyCountryCode
        {
            set
            {
                insuranceDetailsOrderedList[APATCC] = value;
            }
        }
        private string AttorneyZip5
        {
            set
            {
                insuranceDetailsOrderedList[APATZ5] = value;
            }
        }
        private string AttorneyZip4
        {
            set
            {
                insuranceDetailsOrderedList[APATZ4] = value;
            }
        }
        private long AttorneyPhoneNumber
        {
            set
            {
                insuranceDetailsOrderedList[APATPH] = value;
            }
        }

        // Insurance Agent Details
        private string InsuranceAgentName
        {
            set
            {
                insuranceDetailsOrderedList[APAGNM] = value;
            }
        }
        private string InsuranceAgentStreet
        {
            set
            {
                insuranceDetailsOrderedList[APAGST] = value;
            }
        }
        private string InsuranceAgentCity
        {
            set
            {
                insuranceDetailsOrderedList[APAGCT] = value;
            }
        }
        private string InsuranceAgentState
        {
            set
            {
                insuranceDetailsOrderedList[APAGSA] = value;
            }
        }
        private string InsuranceAgentCountryCode
        {
            set
            {
                insuranceDetailsOrderedList[APAGCC] = value;
            }
        }
        private string InsuranceAgentZip5
        {
            set
            {
                insuranceDetailsOrderedList[APAGZ5] = value;
            }
        }
        private string InsuranceAgentZip4
        {
            set
            {
                insuranceDetailsOrderedList[APAGZ4] = value;
            }
        }
        private long InsuranceAgentPhoneNumber
        {
            set
            {
                insuranceDetailsOrderedList[APAGPH] = value;
            }
        }
        #endregion

        #region Authorization Details
        private string AuthorizationNumber
        {
            set
            {
                insuranceDetailsOrderedList[APAUTH_] = value;
            }
        }
        private string AuthorizationRequired
        {
            set
            {
                insuranceDetailsOrderedList[APAUTFG] = value;
            }
        }
        private string AuthorizationCompanyName
        {
            set
            {
                insuranceDetailsOrderedList[APAUCP] = value;
            }
        }
        private long AuthorizationCompanyPhoneNumber
        {
            set
            {
                insuranceDetailsOrderedList[APAUPH] = value;
            }
        }
        private string AuthorizationCompanyPhoneNumberExtension
        {
            set
            {
                insuranceDetailsOrderedList[APAUEX] = value;
            }
        }
        private long NumberOfAuthorizedDays
        {
            set
            {
                insuranceDetailsOrderedList[APAUDY] = value;
            }
        }

        #endregion

        #region FinancialCounseling Details
        private string NoPatientLiability
        {
            set
            {
                insuranceDetailsOrderedList[APCR09] = value;
            }
        }
        private decimal DeductibleDueAmount
        {
            set
            {
                insuranceDetailsOrderedList[APDAMT] = value;
            }
        }
        private decimal CopayDueAmount
        {
            set
            {
                insuranceDetailsOrderedList[APCPAY] = value;
            }
        }
        private int NumberOfMonthlyPaymentsInsured
        {
            set
            {
                insuranceDetailsOrderedList[APMNPY] = value;
            }
        }
        private int NumberOfMonthlyPaymentsUnInsured
        {
            set
            {
                insuranceDetailsOrderedList[APUCPY] = value;
            }
        }
        private decimal MonthlyPaymentInsured
        {
            set
            {
                insuranceDetailsOrderedList[APNUM3] = value;
            }
        }
        private decimal MonthlyPaymentUnInsured
        {
            set
            {
                insuranceDetailsOrderedList[APEFPT] = value;
            }
        }
        private decimal TotalCurrentAmountDue
        {
            set
            {
                insuranceDetailsOrderedList[APNUM1] = value;
            }
        }
        private decimal TotalPaymentsCollectedForInsuredCurrentAccount
        {
            set
            {
                insuranceDetailsOrderedList[APAMCL] = value;
            }
        }
        private decimal TotalPaymentsCollectedForUnInsuredCurrentAccount
        {
            set
            {
                insuranceDetailsOrderedList[APUFPT] = value;
            }
        }

        private decimal TotalUninsuredAmountDue
        {
            set
            {
                insuranceDetailsOrderedList[APNUM2] = value;
            }
        }

        private decimal PreviousBalanceDue
        {
            set
            {
                insuranceDetailsOrderedList[APPVCL] = value;
            }
        }
        private string AddChangeFlag
        {
            set
            {
                insuranceDetailsOrderedList[APACFL] = value;
            }
        }
        private string DayOfMonthPayementDue
        {
            set
            {
                insuranceDetailsOrderedList[APPDUE] = value;
            }
        }
        private string ResourceListProvided
        {
            set
            {
                insuranceDetailsOrderedList[APCRSC] = value;
            }
        }
        private string MedicareIssueDate
        {
            set
            {
                insuranceDetailsOrderedList[APMCDT] = value;
            }
        }

        private string PatientInsuredRelationShip
        {
            set
            {
                insuranceDetailsOrderedList[APSRCD] = value;
            }
        }

        private string InsuredDateOfBirth
        {
            set
            {
                insuranceDetailsOrderedList[APSBTH] = value;
            }
        }

        private string TrackingNumber
        {
            set
            {
                insuranceDetailsOrderedList[APITR_] = value;
            }
        }

        private string InsuranceAdjuster
        {
            set
            {
                insuranceDetailsOrderedList[APADNM] = value;
            }
        }

        private string EmployeeSupervisor
        {
            set
            {
                insuranceDetailsOrderedList[APEMSU] = value;
            }
        }
        private string EVCNumber
        {
            set
            {
                insuranceDetailsOrderedList[APEVC_] = value;
            }
        }

        private string BenefitsVerificationStatus
        {
            set
            {
                insuranceDetailsOrderedList[APVFFG] = value;
            }
        }
        private string BenefitsVerificationDoneBy
        {
            set
            {
                insuranceDetailsOrderedList[APVFBY] = value;
            }
        }
        private DateTime BenefitsVerificationStartDate
        {
            set
            {
                insuranceDetailsOrderedList[APVFDT] = ConvertDateToIntInMddyyFormat( value );
            }
        }

        private string ServiceType
        {
            set
            {
                insuranceDetailsOrderedList[APSVTP] = value;
            }
        }

        #endregion

        #region Benefits Validation
        /* PBAR - PatientAccess Mapping
            PBAR	Name	                Patient Access
            1	    Medicaid	                4
            2	    Workers Comp	            6
            3	    Medicare	                3
            4	    Government Insurance	    2
            5	    Self Pay	                5
            6	    Commercial	                1
        */

        private long TypeOfCoverageId
        {
            set
            {
                insuranceDetailsOrderedList[APCVID] = value;
            }
        }
        private string AuthCompanyRepFirstName
        {
            set
            {
                insuranceDetailsOrderedList[APACRFN] = value;
            }
        }
        private string AuthCompanyRepLastName
        {
            set
            {
                insuranceDetailsOrderedList[APACRLN] = value;
            }
        }
        private string ServicesAuthorized
        {
            set
            {
                insuranceDetailsOrderedList[APSAUTH] = value;
            }
        }
        private DateTime EffectiveDateOfAuthorization
        {
            set
            {
                if( value == DateTime.MinValue )
                {
                    insuranceDetailsOrderedList[APEFDA] = ConvertMinDateToStringInyyyyMMddFormat();
                }
                else
                {
                    insuranceDetailsOrderedList[APEFDA] = ConvertDateToStringInyyyyMMddFormat( value );
                }
            }
        }
        private DateTime ExpirationDateOfAuthorization
        {
            set
            {
                if( value == DateTime.MinValue )
                {
                    insuranceDetailsOrderedList[APEXDA] = ConvertMinDateToStringInyyyyMMddFormat();
                }
                else
                {
                    insuranceDetailsOrderedList[APEXDA] = ConvertDateToStringInyyyyMMddFormat( value );
                }
            }
        }
        private string AuthorizationStatus
        {
            set
            {
                insuranceDetailsOrderedList[APASTS] = value;
            }
        }
        private string AuthorizationRemarks
        {
            set
            {
                insuranceDetailsOrderedList[APARMKS] = value;
            }
        }

        // Medicaid
        private DateTime EligibilityDate_1Medicaid
        {
            set
            {
                if( value == DateTime.MinValue )
                {
                    insuranceDetailsOrderedList[AP1ELDT] = ConvertMinDateToStringInyyyyMMddFormat();
                }
                else
                {
                    insuranceDetailsOrderedList[AP1ELDT] = ConvertDateToStringInyyyyMMddFormat( value );
                }
            }
        }
        private string PatientHasMedicare_1Medicaid
        {
            set
            {
                insuranceDetailsOrderedList[AP1PTMC] = value;
            }
        }
        private string PatientHasOtherInsurance_1Medicaid
        {
            set
            {
                insuranceDetailsOrderedList[AP1PTOI] = value;
            }
        }
        private float CopayAmount_1Medicaid
        {
            set
            {
                insuranceDetailsOrderedList[AP1CPAY] = value;
            }
        }
        private string EvcNumber_1Medicaid
        {
            set
            {
                insuranceDetailsOrderedList[AP1EVCN] = value;
            }
        }
        private long InformationReceivedFrom_1Medicaid
        {
            set
            {
                insuranceDetailsOrderedList[AP1INFF] = value;
            }
        }
        private string Remarks_1Medicaid
        {
            set
            {
                insuranceDetailsOrderedList[AP1REMK] = value;
            }
        }

        // Workers Comp
        private string NameOfNetwork_2WC
        {
            set
            {
                insuranceDetailsOrderedList[AP2NMNT] = value;
            }
        }
        private string IncidentClaimNumber_2WC
        {
            set
            {
                insuranceDetailsOrderedList[AP2CLMN] = value;
            }
        }
        private string ClaimAddressVerified_2WC
        {
            set
            {
                insuranceDetailsOrderedList[AP2ADVR] = value;
            }
        }
        private string InsurancePhoneNumber_2WC
        {
            set
            {
                insuranceDetailsOrderedList[AP2INPH] = value;
            }
        }
        private string EmployerPremiumPaidToDate_2WC
        {
            set
            {
                insuranceDetailsOrderedList[AP2EMPM] = value;
            }
        }
        private long InformationReceivedFrom_2WC
        {
            set
            {
                insuranceDetailsOrderedList[AP2INFF] = value;
            }
        }
        private string Remarks_2WC
        {
            set
            {
                insuranceDetailsOrderedList[AP2REMK] = value;
            }
        }

        // Medicare
        private string PartACoverage_3Medicare
        {
            set
            {
                insuranceDetailsOrderedList[AP3PACV] = value;
            }
        }
        private DateTime PartAEffectiveDate_3Medicare
        {
            set
            {
                if( value == DateTime.MinValue )
                {
                    insuranceDetailsOrderedList[AP3PAED] = ConvertMinDateToStringInyyyyMMddFormat();
                }
                else
                {
                    insuranceDetailsOrderedList[AP3PAED] = ConvertDateToStringInyyyyMMddFormat( value );
                }
            }
        }
        private string PartBCoverage_3Medicare
        {
            set
            {
                insuranceDetailsOrderedList[AP3PBCV] = value;
            }
        }
        private DateTime PartBEffectiveDate_3Medicare
        {
            set
            {
                if( value == DateTime.MinValue )
                {
                    insuranceDetailsOrderedList[AP3PBED] = ConvertMinDateToStringInyyyyMMddFormat();
                }
                else
                {
                    insuranceDetailsOrderedList[AP3PBED] = ConvertDateToStringInyyyyMMddFormat( value );
                }
            }
        }
        private string PatientHasMedicareHMO_3Medicare
        {
            set
            {
                insuranceDetailsOrderedList[AP3MHCV] = value;
            }
        }
        private string MedicareIsSecondary_3Medicare
        {
            set
            {
                insuranceDetailsOrderedList[AP3MDSC] = value;
            }
        }
        private DateTime DateOfLastBillingActivity_3Medicare
        {
            set
            {
                if( value == DateTime.MinValue )
                {
                    insuranceDetailsOrderedList[AP3DTLB] = ConvertMinDateToStringInyyyyMMddFormat();
                }
                else
                {
                    insuranceDetailsOrderedList[AP3DTLB] = ConvertDateToStringInyyyyMMddFormat( value );
                }
            }
        }
        private int RemainingHospitalDays_3Medicare
        {
            set
            {
                insuranceDetailsOrderedList[AP3RMHD] = value;
            }
        }
        private int RemainingCoInsuranceDays_3Medicare
        {
            set
            {
                insuranceDetailsOrderedList[AP3RCID] = value;
            }
        }
        private int RemainingLifetimeReserveDays_3Medicare
        {
            set
            {
                insuranceDetailsOrderedList[AP3RLRD] = value;
            }
        }
        private int RemainingSNFDays_3Medicare
        {
            set
            {
                insuranceDetailsOrderedList[AP3RSNF] = value;
            }
        }
        private int RemainingSNFCoInsuranceDays_3Medicare
        {
            set
            {
                insuranceDetailsOrderedList[AP3RSNC] = value;
            }
        }
        private float RemainingPartADeductible_3Medicare
        {
            set
            {
                insuranceDetailsOrderedList[AP3RPAD] = value;
            }
        }
        private float RemainingPartBDeductible_3Medicare
        {
            set
            {
                insuranceDetailsOrderedList[AP3RPBD] = value;
            }
        }
        private string PatientIsInHospice_3Medicare
        {
            set
            {
                insuranceDetailsOrderedList[AP3PTHS] = value;
            }
        }
        private string BeneficiaryNameVerified_3Medicare
        {
            set
            {
                insuranceDetailsOrderedList[AP3BNNV] = value;
            }
        }
        private long InformationReceivedFrom_3Medicare
        {
            set
            {
                insuranceDetailsOrderedList[AP3INFF] = value;
            }
        }
        private string Remarks_3Medicare
        {
            set
            {
                insuranceDetailsOrderedList[AP3REMK] = value;
            }
        }

        // Government Insurance
        private long InformationReceivedFrom_4GovIns
        {
            set
            {
                insuranceDetailsOrderedList[AP4INFF] = value;
            }
        }
        private string EligibilityPhoneNumber_4GovIns
        {
            set
            {
                insuranceDetailsOrderedList[AP4ELPH] = value;
            }
        }
        private string InsuranceCompanyRepName_4GovIns
        {
            set
            {
                insuranceDetailsOrderedList[AP4INRP] = value;
            }
        }
        private string TypeOfCoverage_4GovIns
        {
            set
            {
                insuranceDetailsOrderedList[AP4TYCV] = value;
            }
        }
        private DateTime EffectiveDateOfInsured_4GovIns
        {
            set
            {
                if( value == DateTime.MinValue )
                {
                    insuranceDetailsOrderedList[AP4EFDI] = ConvertMinDateToStringInyyyyMMddFormat();
                }
                else
                {
                    insuranceDetailsOrderedList[AP4EFDI] = ConvertDateToStringInyyyyMMddFormat( value );
                }
            }
        }
        private DateTime TerminationDateForInsured_4GovIns
        {
            set
            {
                if( value == DateTime.MinValue )
                {
                    insuranceDetailsOrderedList[AP4TMDI] = ConvertMinDateToStringInyyyyMMddFormat();
                }
                else
                {
                    insuranceDetailsOrderedList[AP4TMDI] = ConvertDateToStringInyyyyMMddFormat( value );
                }
            }
        }
        private float DeductibleAmount_4GovIns
        {
            set
            {
                insuranceDetailsOrderedList[AP4DEDA] = value;
            }
        }
        private string IsDeductibleMet_4GovIns
        {
            set
            {
                insuranceDetailsOrderedList[AP4DEMT] = value;
            }
        }
        private float DeductibleAmountMet_4GovIns
        {
            set
            {
                insuranceDetailsOrderedList[AP4DEAM] = value;
            }
        }
        private int CoInsurance_4GovIns
        {
            set
            {
                insuranceDetailsOrderedList[AP4COIN] = value;
            }
        }
        private float OutOfPocket_4GovIns
        {
            set
            {
                insuranceDetailsOrderedList[AP4OTPK] = value;
            }
        }
        private string IsOutOfPocketMet_4GovIns
        {
            set
            {
                insuranceDetailsOrderedList[AP4OTPM] = value;
            }
        }
        private float OutOfPocketAmountMet_4GovIns
        {
            set
            {
                insuranceDetailsOrderedList[AP4OTPA] = value;
            }
        }
        private int PercentOutOfPocket_4GovIns
        {
            set
            {
                insuranceDetailsOrderedList[AP4PROT] = value;
            }
        }
        private float CoPayAmount_4GovIns
        {
            set
            {
                insuranceDetailsOrderedList[AP4CPAY] = value;
            }
        }
        private string Remarks_4GovIns
        {
            set
            {
                insuranceDetailsOrderedList[AP4REMK] = value;
            }
        }

        // Self Pay
        private string HasMedicaid_5SelfPay
        {
            set
            {
                insuranceDetailsOrderedList[AP5PTMD] = value;
            }
        }
        private string InsuranceInfoAvailable_5SelfPay
        {
            set
            {
                insuranceDetailsOrderedList[AP5INIF] = value;
            }
        }

        // Commercial
        private long InformationReceivedFrom_6Comm
        {
            set
            {
                insuranceDetailsOrderedList[AP6INFF] = value;
            }
        }
        private string EligibilityPhoneNumber_6Comm
        {
            set
            {
                insuranceDetailsOrderedList[AP6ELPH] = value;
            }
        }
        private string InsuranceCompanyRepName_6Comm
        {
            set
            {
                insuranceDetailsOrderedList[AP6INRP] = value;
            }
        }
        private DateTime EffectiveDateOfInsured_6Comm
        {
            set
            {
                if( value == DateTime.MinValue )
                {
                    insuranceDetailsOrderedList[AP6EFDI] = ConvertMinDateToStringInyyyyMMddFormat();
                }
                else
                {
                    insuranceDetailsOrderedList[AP6EFDI] = ConvertDateToStringInyyyyMMddFormat( value );
                }

            }
        }
        private DateTime TerminationDateForInsured_6Comm
        {
            set
            {
                if( value == DateTime.MinValue )
                {
                    insuranceDetailsOrderedList[AP6TMDI] = ConvertMinDateToStringInyyyyMMddFormat();
                }
                else
                {
                    insuranceDetailsOrderedList[AP6TMDI] = ConvertDateToStringInyyyyMMddFormat( value );
                }
            }
        }
        private string IsServiceForPreExistingCondition_6Comm
        {
            set
            {
                insuranceDetailsOrderedList[AP6SFPC] = value;
            }
        }
        private string IsServiceACoveredBenefit_6Comm
        {
            set
            {
                insuranceDetailsOrderedList[AP6SACB] = value;
            }
        }
        private string ClaimsAAddressVerified_6Comm
        {
            set
            {
                insuranceDetailsOrderedList[AP6CAAV] = value;
            }
        }
        private string CoordinationOfBenefits_6Comm
        {
            set
            {
                insuranceDetailsOrderedList[AP6COBF] = value;
            }
        }
        private long RuleToDetermineCOB_6Comm
        {
            set
            {
                insuranceDetailsOrderedList[AP6RCOB] = value;
            }
        }
        private long TypeOfProduct_6Comm
        {
            set
            {
                insuranceDetailsOrderedList[AP6TPPR] = value;
            }
        }
        private string PPONetworkBrokerName_6Comm
        {
            set
            {
                insuranceDetailsOrderedList[AP6NPPO] = value;
            }
        }
        private string HospitalIsAContractedProvider_6Comm
        {
            set
            {
                insuranceDetailsOrderedList[AP6HCPR] = value;
            }
        }
        private string AutoClaimNumber_6Comm
        {
            set
            {
                insuranceDetailsOrderedList[AP6ACNM] = value;
            }
        }
        private string AutoMedPayCoverage_6Comm
        {
            set
            {
                insuranceDetailsOrderedList[AP6MPCV] = value;
            }
        }
        private string Remarks_6Comm
        {
            set
            {
                insuranceDetailsOrderedList[AP6REMK] = value;
            }
        }
        #endregion

        #endregion

        #region Data Elements

        private OrderedList insuranceDetailsOrderedList = new OrderedList();
        private int coverageType;

        #endregion

        #region Constants

        private const string NO_LIABLITY_FLAG = "X";
        private const string YES_FLAG = "Y";
        private const string NO_FLAG = "N";
        private const string ALLEMERGENCYSERVICETYPE = "ALLER";
        private const string VERSION_1 = "1";
        private const string VERSION_2 = "2";
        private const string MSE_FLAG_ADD = "A";
        private const string MSE_FLAG_DELETE = "D";

        private const string MEDICAID = "531";
        private const string MEDICARE = "535";
        private const int AUTHORIZATION_REMARKS_LENGTH = 120;
        private const int COVERAGE_REMARKS_LENGTH = 60;

        private const string
            APIDWS = "APIDWS",
            APIDID = "APIDID",
            APRR_ = "APRR#",
            APSEC2 = "APSEC2",
            APHSP_ = "APHSP#",
            APACCT = "APACCT",
            APSEQ_ = "APSEQ#",
            APGAR_ = "APGAR#",
            APPTY = "APPTY",
            APPLAN = "APPLAN",
            APBCFL = "APBCFL",
            APCSPL = "APCSPL",
            APINM = "APINM",
            APGNM = "APGNM",
            APIAD1 = "APIAD1",
            APIAD2 = "APIAD2",
            APIAD3 = "APIAD3",
            APVST = "APVST",
            APPROR = "APPROR",
            APLAST = "APLAST",
            APVBV = "APVBV",
            APSDAT = "APSDAT",
            AP1FLG = "AP1FLG",
            AP1DED = "AP1DED",
            AP1_ = "AP1$",
            AP1PCT = "AP1PCT",
            AP1DYS = "AP1DYS",
            AP2FLG = "AP2FLG",
            AP2DED = "AP2DED",
            AP2PCT = "AP2PCT",
            AP2MX_ = "AP2MX$",
            AP2DR_ = "AP2DR$",
            AP2DID = "AP2DID",
            AP2IR_ = "AP2IR$",
            AP2IRD = "AP2IRD",
            AP3FLG = "AP3FLG",
            AP3DED = "AP3DED",
            AP3PCT = "AP3PCT",
            AP3_ = "AP3$",
            AP4FLG = "AP4FLG",
            AP4DED = "AP4DED",
            AP4PCT = "AP4PCT",
            AP4_ = "AP4$",
            AP4MR_ = "AP4MR$",
            AP4MRD = "AP4MRD",
            AP4MI_ = "AP4MI$",
            AP4MID = "AP4MID",
            AP5FLG = "AP5FLG",
            AP5DED = "AP5DED",
            AP5PCT = "AP5PCT",
            AP5_ = "AP5$",
            AP5MR_ = "AP5MR$",
            AP5MRD = "AP5MRD",
            AP5MI_ = "AP5MI$",
            AP5MID = "AP5MID",
            AP6DBM = "AP6DBM",
            AP6XMM = "AP6XMM",
            AP6RBM = "AP6RBM",
            AP6DM = "AP6DM",
            APCOB = "APCOB",
            APEX01 = "APEX01",
            APEX02 = "APEX02",
            APEX03 = "APEX03",
            APEX04 = "APEX04",
            APEX05 = "APEX05",
            APEX06 = "APEX06",
            APEX07 = "APEX07",
            APEX08 = "APEX08",
            APEX09 = "APEX09",
            APEX10 = "APEX10",
            APEX11 = "APEX11",
            APEX12 = "APEX12",
            APEX13 = "APEX13",
            APEX14 = "APEX14",
            APEX15 = "APEX15",
            APEX16 = "APEX16",
            APEX17 = "APEX17",
            APEX18 = "APEX18",
            APEX19 = "APEX19",
            APEX20 = "APEX20",
            APFDAY = "APFDAY",
            APCDAY = "APCDAY",
            APCDOL = "APCDOL",
            APLDAY = "APLDAY",
            APLDOL = "APLDOL",
            APBDPT = "APBDPT",
            APBD_ = "APBD$",
            APPMET = "APPMET",
            APPM_ = "APPM$",
            APPMPR = "APPMPR",
            APMCRE = "APMCRE",
            APMAID = "APMAID",
            APMDED = "APMDED",
            APLML = "APLML",
            APLMD = "APLMD",
            APLUL_ = "APLUL#",
            APACFL = "APACFL",
            APTTME = "APTTME",
            APINLG = "APINLG",
            APBYPS = "APBYPS",
            APSWPY = "APSWPY",
            APABCF = "APABCF",
            APRICF = "APRICF",
            APSLNM = "APSLNM",
            APSFNM = "APSFNM",
            APSSEX = "APSSEX",
            APSRCD = "APSRCD",
            APIID_ = "APIID#",
            APGRPN = "APGRPN",
            APESCD = "APESCD",
            APSBEN = "APSBEN",
            APEEID = "APEEID",
            APSBEL = "APSBEL",
            APJADR = "APJADR",
            APJADRE1 = "APJADRE1",
            APJADRE2 = "APJADRE2",
            APJCIT = "APJCIT",
            APJSTE = "APJSTE",
            APJZIP = "APJZIP",
            APJZP4 = "APJZP4",
            APJACD = "APJACD",
            APJPH_ = "APJPH#",
            APNEIC = "APNEIC",
            APDOV = "APDOV",
            APELGS = "APELGS",
            APTDOC = "APTDOC",
            APELDT = "APELDT",
            APSCHB = "APSCHB",
            APVC01 = "APVC01",
            APVC02 = "APVC02",
            APVC03 = "APVC03",
            APVC04 = "APVC04",
            APVA01 = "APVA01",
            APVA02 = "APVA02",
            APVA03 = "APVA03",
            APVA04 = "APVA04",
            APAGNY = "APAGNY",
            APCHAM = "APCHAM",
            APCHAP = "APCHAP",
            APMSPC = "APMSPC",
            APDBEN = "APDBEN",
            APDURR = "APDURR",
            APACRE = "APACRE",
            APDGPS = "APDGPS",
            APNEPS = "APNEPS",
            APNEPI = "APNEPI",
            APITYP = "APITYP",
            APRIPL = "APRIPL",
            APCBFL = "APCBFL",
            APINEX = "APINEX",
            APSURP = "APSURP",
            APSURA = "APSURA",
            APINS_ = "APINS#",
            APCNFC = "APCNFC",
            APPFCC = "APPFCC",
            APRICO = "APRICO",
            APENM1 = "APENM1",
            APELO1 = "APELO1",
            APEDC1 = "APEDC1",
            APESC1 = "APESC1",
            APEID1 = "APEID1",
            APEA01 = "APEA01",
            APEZ01 = "APEZ01",
            APENM2 = "APENM2",
            APELO2 = "APELO2",
            APEDC2 = "APEDC2",
            APESC2 = "APESC2",
            APEID2 = "APEID2",
            APEA02 = "APEA02",
            APEZ02 = "APEZ02",
            APDEDU = "APDEDU",
            APBLDE = "APBLDE",
            APINDY = "APINDY",
            APLIFD = "APLIFD",
            APNCVD = "APNCVD",
            APTDAT = "APTDAT",
            APCLRK = "APCLRK",
            APZDTE = "APZDTE",
            APZTME = "APZTME",
            APPNID = "APPNID",
            APP_NM = "APP#NM",
            APCBGD = "APCBGD",
            APPLAD = "APPLAD",
            APSVTP = "APSVTP",
            APSTCP = "APSTCP",
            APSTAM = "APSTAM",
            APST_L = "APST$L",
            APSTPA = "APSTPA",
            APAUTF = "APAUTF",
            APAUWV = "APAUWV",
            APAUDY = "APAUDY",
            AP2OPF = "AP2OPF",
            AP2CMM = "AP2CMM",
            APCOPF = "APCOPF",
            APDEDF = "APDEDF",
            APBLNM = "APBLNM",
            APOBCF = "APOBCF",
            APOBCP = "APOBCP",
            APOBAM = "APOBAM",
            APOB_L = "APOB$L",
            APOBPA = "APOBPA",
            APNBCF = "APNBCF",
            APNBCP = "APNBCP",
            APNBAM = "APNBAM",
            APNB_L = "APNB$L",
            APNBPA = "APNBPA",
            APCDCF = "APCDCF",
            APCDCP = "APCDCP",
            APCDAM = "APCDAM",
            APCD_L = "APCD$L",
            APCDPA = "APCDPA",
            APDCCF = "APDCCF",
            APDCCP = "APDCCP",
            APDCAM = "APDCAM",
            APDC_L = "APDC$L",
            APDCPA = "APDCPA",
            APDCAL = "APDCAL",
            APDADM = "APDADM",
            APDDMF = "APDDMF",
            APLFMX = "APLFMX",
            APMXAD = "APMXAD",
            APCOBF = "APCOBF",
            APCOMM = "APCOMM",
            APIFRF = "APIFRF",
            APSUAC = "APSUAC",
            APSUPH = "APSUPH",
            APVFFG = "APVFFG",
            APVFBY = "APVFBY",
            APVFDT = "APVFDT",
            APAMDY = "APAMDY",
            APNCDY = "APNCDY",
            APELOS = "APELOS",
            APRCDE = "APRCDE",
            APRSLC = "APRSLC",
            APUROP = "APUROP",
            APMSPF = "APMSPF",
            APWVCP = "APWVCP",
            APGCD1 = "APGCD1",
            APGCD2 = "APGCD2",
            APCONM = "APCONM",
            APEZP1 = "APEZP1",
            APEZ41 = "APEZ41",
            APEZP2 = "APEZP2",
            APEZ42 = "APEZ42",
            APJZPA = "APJZPA",
            APJZ4A = "APJZ4A",
            APOID = "APOID",
            APPRE = "APPRE",
            APMR_ = "APMR#",
            APOACT = "APOACT",
            APEVC_ = "APEVC#",
            APSMI = "APSMI",
            APSBID = "APSBID",
            APSCUN = "APSCUN",
            APSBTH = "APSBTH",
            APOILN = "APOILN",
            APOIFN = "APOIFN",
            APOIMI = "APOIMI",
            APOISX = "APOISX",
            APOIBD = "APOIBD",
            APOIID = "APOIID",
            APOIAD = "APOIAD",
            APOICT = "APOICT",
            APOIST = "APOIST",
            APOICN = "APOICN",
            APOIZP = "APOIZP",
            APOIZ4 = "APOIZ4",
            APICPH = "APICPH",
            APMCDT = "APMCDT",
            APDAMT = "APDAMT",
            APCPAY = "APCPAY",
            APCINS = "APCINS",
            APAMCL = "APAMCL",
            APPYCL = "APPYCL",
            APINCL = "APINCL",
            APPVCL = "APPVCL",
            APUCBL = "APUCBL",
            APEDBL = "APEDBL",
            APUFPT = "APUFPT",
            APEFPT = "APEFPT",
            APNUM1 = "APNUM1",
            APNUM2 = "APNUM2",
            APNUM3 = "APNUM3",
            APMNPY = "APMNPY",
            APCOPY = "APCOPY",
            APCIPY = "APCIPY",
            APUCPY = "APUCPY",
            APEDPY = "APEDPY",
            APPDUE = "APPDUE",
            APCRPT = "APCRPT",
            APCRSC = "APCRSC",
            APCR01 = "APCR01",
            APCR02 = "APCR02",
            APCR03 = "APCR03",
            APCR04 = "APCR04",
            APCR05 = "APCR05",
            APCR06 = "APCR06",
            APCR07 = "APCR07",
            APCR08 = "APCR08",
            APCR09 = "APCR09",
            APCR10 = "APCR10",
            APCR11 = "APCR11",
            APCR12 = "APCR12",
            APHIC_ = "APHIC#",
            APITR_ = "APITR#",
            APAUTH_ = "APAUTH#",
            APAUTFG = "APAUTFG",
            APAUCP = "APAUCP",
            APAUPH = "APAUPH",
            APAUEX = "APAUEX",
            APADNM = "APADNM",
            APEMSU = "APEMSU",
            APATNM = "APATNM",
            APATST = "APATST",
            APATCT = "APATCT",
            APATSA = "APATSA",
            APATZ5 = "APATZ5",
            APATZ4 = "APATZ4",
            APATCC = "APATCC",
            APATPH = "APATPH",
            APAGNM = "APAGNM",
            APAGPH = "APAGPH",
            APAGST = "APAGST",
            APAGCT = "APAGCT",
            APAGSA = "APAGSA",
            APAGZ5 = "APAGZ5",
            APAGZ4 = "APAGZ4",
            APAGCC = "APAGCC",
            AP_BL = "AP_BL",
            AP_GP = "AP_GP",
            AP_VA = "AP_VA",
            AP_WA = "AP_WA",
            AP_NA = "AP_NA",
            AP_OP = "AP_OP",
            AP_AG = "AP_AG",
            AP_EMPL = "AP_EMPL",
            AP_SEMPL = "AP_SEMPL",
            AP_AGGHP = "AP_AGGHP",
            AP_AGGHP20 = "AP_AGGHP20",
            AP_DI = "AP_DI",
            AP_OEMPL = "AP_OEMPL",
            AP_DIGHP = "AP_DIGHP",
            AP_DIGHP00 = "AP_DIGHP00",
            AP_ES = "AP_ES",
            AP_ESGHP = "AP_ESGHP",
            AP_ESKT = "AP_ESKT",
            AP_ESDT = "AP_ESDT",
            AP_ES30MO = "AP_ES30MO",
            AP_ESMULTI = "AP_ESMULTI",
            AP_ESIA = "AP_ESIA",
            AP_ESOTHR = "AP_ESOTHR",
            AP_GHP = "AP_GHP",
            AP_AUTO = "AP_AUTO",
            AP_INS_ = "AP_INS#",
            AP_PNID1 = "AP_PNID1",
            AP_PNID2 = "AP_PNID2",
            AP_USER = "AP_USER",
            AP_BLDT = "AP_BLDT",
            AP_AIDT = "AP_AIDT",
            AP_ESKTDT = "AP_ESKTDT",
            AP_ESDTDT = "AP_ESDTDT",
            APDIALCTR = "APDIALCTR",
            AP_ESSDTDT = "AP_ESSDTDT",
            AP_EMPLRDT = "AP_EMPLRDT",
            AP_SEMPRDT = "AP_SEMPRDT",
            AP_DTTM = "AP_DTTM",
            AP_SENM = "AP_SENM",
            AP_SEADR = "AP_SEADR",
            AP_SECIT = "AP_SECIT",
            AP_SESTE = "AP_SESTE",
            AP_SEZIP = "AP_SEZIP",
            AP_SEZP4 = "AP_SEZP4",
            AP_BLTDY = "AP_BLTDY",
            AP_FENM = "AP_FENM",
            AP_FEADR = "AP_FEADR",
            AP_FECIT = "AP_FECIT",
            AP_FESTE = "AP_FESTE",
            AP_FEZIP = "AP_FEZIP",
            AP_FEZP4 = "AP_FEZP4",
            AP_EMPLN = "AP_EMPLN",
            AP_SEMPLN = "AP_SEMPLN",
            APWSIR = "APWSIR",
            APSECR = "APSECR",
            APORR1 = "APORR1",
            APORR2 = "APORR2",
            APORR3 = "APORR3",
            AP_NFALT = "AP_NFALT",
            AP_LIABL = "AP_LIABL",
            AP_AGGHPF = "AP_AGGHPF",
            AP_AGHP20P = "AP_AGHP20P",
            AP_DIGHPF = "AP_DIGHPF",
            AP_DGHP00P = "AP_DGHP00P",
            AP_DGHP00F = "AP_DGHP00F",
            AP_FORMVER = "AP_FORMVER",
            APPACCT = "APPACCT",
            APACTF = "APACTF",

            // Benefits Validation
            APCVID = "APCVID",
            APACRFN = "APACRFN",
            APACRLN = "APACRLN",
            APSAUTH = "APSAUTH",
            APEFDA = "APEFDA",
            APEXDA = "APEXDA",
            APASTS = "APASTS",
            APARMKS = "APARMKS",

            AP1ELDT = "AP1ELDT",
            AP1PTMC = "AP1PTMC",
            AP1PTOI = "AP1PTOI",
            AP1CPAY = "AP1CPAY",
            AP1EVCN = "AP1EVCN",
            AP1INFF = "AP1INFF",
            AP1REMK = "AP1REMK",

            AP2NMNT = "AP2NMNT",
            AP2CLMN = "AP2CLMN",
            AP2ADVR = "AP2ADVR",
            AP2INPH = "AP2INPH",
            AP2EMPM = "AP2EMPM",
            AP2INFF = "AP2INFF",
            AP2REMK = "AP2REMK",

            AP3PACV = "AP3PACV",
            AP3PAED = "AP3PAED",
            AP3PBCV = "AP3PBCV",
            AP3PBED = "AP3PBED",
            AP3MHCV = "AP3MHCV",
            AP3MDSC = "AP3MDSC",
            AP3DTLB = "AP3DTLB",
            AP3RMHD = "AP3RMHD",
            AP3RCID = "AP3RCID",
            AP3RLRD = "AP3RLRD",
            AP3RSNF = "AP3RSNF",
            AP3RSNC = "AP3RSNC",
            AP3RPAD = "AP3RPAD",
            AP3RPBD = "AP3RPBD",
            AP3PTHS = "AP3PTHS",
            AP3BNNV = "AP3BNNV",
            AP3INFF = "AP3INFF",
            AP3REMK = "AP3REMK",

            AP4INFF = "AP4INFF",
            AP4ELPH = "AP4ELPH",
            AP4INRP = "AP4INRP",
            AP4TYCV = "AP4TYCV",
            AP4EFDI = "AP4EFDI",
            AP4TMDI = "AP4TMDI",
            AP4DEDA = "AP4DEDA",
            AP4DEMT = "AP4DEMT",
            AP4DEAM = "AP4DEAM",
            AP4COIN = "AP4COIN",
            AP4OTPK = "AP4OTPK",
            AP4OTPM = "AP4OTPM",
            AP4OTPA = "AP4OTPA",
            AP4PROT = "AP4PROT",
            AP4CPAY = "AP4CPAY",
            AP4REMK = "AP4REMK",

            AP5PTMD = "AP5PTMD",
            AP5INIF = "AP5INIF",

            AP6INFF = "AP6INFF",
            AP6ELPH = "AP6ELPH",
            AP6INRP = "AP6INRP",
            AP6EFDI = "AP6EFDI",
            AP6TMDI = "AP6TMDI",
            AP6SFPC = "AP6SFPC",
            AP6SACB = "AP6SACB",
            AP6CAAV = "AP6CAAV",
            AP6COBF = "AP6COBF",
            AP6RCOB = "AP6RCOB",
            AP6TPPR = "AP6TPPR",
            AP6NPPO = "AP6NPPO",
            AP6HCPR = "AP6HCPR",
            AP6ACNM = "AP6ACNM",
            AP6MPCV = "AP6MPCV",
            AP6REMK = "AP6REMK",

            AP601CTID = "AP601CTID",
            AP601DEDA = "AP601DEDA",
            AP601TMPR = "AP601TMPR",
            AP601DEMT = "AP601DEMT",
            AP601DEAM = "AP601DEAM",
            AP601COIN = "AP601COIN",
            AP601OTPK = "AP601OTPK",
            AP601OTPM = "AP601OTPM",
            AP601OTPA = "AP601OTPA",
            AP601PROT = "AP601PROT",
            AP601CPAY = "AP601CPAY",
            AP601WCPY = "AP601WCPY",
            AP601NMVS = "AP601NMVS",
            AP601LFMB = "AP601LFMB",
            AP601LFRM = "AP601LFRM",
            AP601LFVM = "AP601LFVM",
            AP601MXBN = "AP601MXBN",
            AP601RMBN = "AP601RMBN",
            AP601RMMT = "AP601RMMT",

            AP602CTID = "AP602CTID",
            AP602DEDA = "AP602DEDA",
            AP602TMPR = "AP602TMPR",
            AP602DEMT = "AP602DEMT",
            AP602DEAM = "AP602DEAM",
            AP602COIN = "AP602COIN",
            AP602OTPK = "AP602OTPK",
            AP602OTPM = "AP602OTPM",
            AP602OTPA = "AP602OTPA",
            AP602PROT = "AP602PROT",
            AP602CPAY = "AP602CPAY",
            AP602WCPY = "AP602WCPY",
            AP602NMVS = "AP602NMVS",
            AP602LFMB = "AP602LFMB",
            AP602LFRM = "AP602LFRM",
            AP602LFVM = "AP602LFVM",
            AP602MXBN = "AP602MXBN",
            AP602RMBN = "AP602RMBN",
            AP602RMMT = "AP602RMMT",

            AP603CTID = "AP603CTID",
            AP603DEDA = "AP603DEDA",
            AP603TMPR = "AP603TMPR",
            AP603DEMT = "AP603DEMT",
            AP603DEAM = "AP603DEAM",
            AP603COIN = "AP603COIN",
            AP603OTPK = "AP603OTPK",
            AP603OTPM = "AP603OTPM",
            AP603OTPA = "AP603OTPA",
            AP603PROT = "AP603PROT",
            AP603CPAY = "AP603CPAY",
            AP603WCPY = "AP603WCPY",
            AP603NMVS = "AP603NMVS",
            AP603LFMB = "AP603LFMB",
            AP603LFRM = "AP603LFRM",
            AP603LFVM = "AP603LFVM",
            AP603MXBN = "AP603MXBN",
            AP603RMBN = "AP603RMBN",
            AP603RMMT = "AP603RMMT",

            AP604CTID = "AP604CTID",
            AP604DEDA = "AP604DEDA",
            AP604TMPR = "AP604TMPR",
            AP604DEMT = "AP604DEMT",
            AP604DEAM = "AP604DEAM",
            AP604COIN = "AP604COIN",
            AP604OTPK = "AP604OTPK",
            AP604OTPM = "AP604OTPM",
            AP604OTPA = "AP604OTPA",
            AP604PROT = "AP604PROT",
            AP604CPAY = "AP604CPAY",
            AP604WCPY = "AP604WCPY",
            AP604NMVS = "AP604NMVS",
            AP604LFMB = "AP604LFMB",
            AP604LFRM = "AP604LFRM",
            AP604LFVM = "AP604LFVM",
            AP604MXBN = "AP604MXBN",
            AP604RMBN = "AP604RMBN",
            AP604RMMT = "AP604RMMT",

            AP605CTID = "AP605CTID",
            AP605DEDA = "AP605DEDA",
            AP605TMPR = "AP605TMPR",
            AP605DEMT = "AP605DEMT",
            AP605DEAM = "AP605DEAM",
            AP605COIN = "AP605COIN",
            AP605OTPK = "AP605OTPK",
            AP605OTPM = "AP605OTPM",
            AP605OTPA = "AP605OTPA",
            AP605PROT = "AP605PROT",
            AP605CPAY = "AP605CPAY",
            AP605WCPY = "AP605WCPY",
            AP605NMVS = "AP605NMVS",
            AP605LFMB = "AP605LFMB",
            AP605LFRM = "AP605LFRM",
            AP605LFVM = "AP605LFVM",
            AP605MXBN = "AP605MXBN",
            AP605RMBN = "AP605RMBN",
            AP605RMMT = "AP605RMMT",

            AP606CTID = "AP606CTID",
            AP606DEDA = "AP606DEDA",
            AP606TMPR = "AP606TMPR",
            AP606DEMT = "AP606DEMT",
            AP606DEAM = "AP606DEAM",
            AP606COIN = "AP606COIN",
            AP606OTPK = "AP606OTPK",
            AP606OTPM = "AP606OTPM",
            AP606OTPA = "AP606OTPA",
            AP606PROT = "AP606PROT",
            AP606CPAY = "AP606CPAY",
            AP606WCPY = "AP606WCPY",
            AP606NMVS = "AP606NMVS",
            AP606LFMB = "AP606LFMB",
            AP606LFRM = "AP606LFRM",
            AP606LFVM = "AP606LFVM",
            AP606MXBN = "AP606MXBN",
            AP606RMBN = "AP606RMBN",
            AP606RMMT = "AP606RMMT",

            AP607CTID = "AP607CTID",
            AP607DEDA = "AP607DEDA",
            AP607TMPR = "AP607TMPR",
            AP607DEMT = "AP607DEMT",
            AP607DEAM = "AP607DEAM",
            AP607COIN = "AP607COIN",
            AP607OTPK = "AP607OTPK",
            AP607OTPM = "AP607OTPM",
            AP607OTPA = "AP607OTPA",
            AP607PROT = "AP607PROT",
            AP607CPAY = "AP607CPAY",
            AP607WCPY = "AP607WCPY",
            AP607NMVS = "AP607NMVS",
            AP607LFMB = "AP607LFMB",
            AP607LFRM = "AP607LFRM",
            AP607LFVM = "AP607LFVM",
            AP607MXBN = "AP607MXBN",
            AP607RMBN = "AP607RMBN",
            AP607RMMT = "AP607RMMT",

            AP608CTID = "AP608CTID",
            AP608DEDA = "AP608DEDA",
            AP608TMPR = "AP608TMPR",
            AP608DEMT = "AP608DEMT",
            AP608DEAM = "AP608DEAM",
            AP608COIN = "AP608COIN",
            AP608OTPK = "AP608OTPK",
            AP608OTPM = "AP608OTPM",
            AP608OTPA = "AP608OTPA",
            AP608PROT = "AP608PROT",
            AP608CPAY = "AP608CPAY",
            AP608WCPY = "AP608WCPY",
            AP608NMVS = "AP608NMVS",
            AP608LFMB = "AP608LFMB",
            AP608LFRM = "AP608LFRM",
            AP608LFVM = "AP608LFVM",
            AP608MXBN = "AP608MXBN",
            AP608RMBN = "AP608RMBN",
            AP608RMMT = "AP608RMMT",

            AP609CTID = "AP609CTID",
            AP609DEDA = "AP609DEDA",
            AP609TMPR = "AP609TMPR",
            AP609DEMT = "AP609DEMT",
            AP609DEAM = "AP609DEAM",
            AP609COIN = "AP609COIN",
            AP609OTPK = "AP609OTPK",
            AP609OTPM = "AP609OTPM",
            AP609OTPA = "AP609OTPA",
            AP609PROT = "AP609PROT",
            AP609CPAY = "AP609CPAY",
            AP609WCPY = "AP609WCPY",
            AP609NMVS = "AP609NMVS",
            AP609LFMB = "AP609LFMB",
            AP609LFRM = "AP609LFRM",
            AP609LFVM = "AP609LFVM",
            AP609MXBN = "AP609MXBN",
            AP609RMBN = "AP609RMBN",
            AP609RMMT = "AP609RMMT",

            AP610CTID = "AP610CTID",
            AP610DEDA = "AP610DEDA",
            AP610TMPR = "AP610TMPR",
            AP610DEMT = "AP610DEMT",
            AP610DEAM = "AP610DEAM",
            AP610COIN = "AP610COIN",
            AP610OTPK = "AP610OTPK",
            AP610OTPM = "AP610OTPM",
            AP610OTPA = "AP610OTPA",
            AP610PROT = "AP610PROT",
            AP610CPAY = "AP610CPAY",
            AP610WCPY = "AP610WCPY",
            AP610NMVS = "AP610NMVS",
            AP610LFMB = "AP610LFMB",
            AP610LFRM = "AP610LFRM",
            AP610LFVM = "AP610LFVM",
            AP610MXBN = "AP610MXBN",
            AP610RMBN = "AP610RMBN",
            AP610RMMT = "AP610RMMT",

            AP611CTID = "AP611CTID",
            AP611DEDA = "AP611DEDA",
            AP611TMPR = "AP611TMPR",
            AP611DEMT = "AP611DEMT",
            AP611DEAM = "AP611DEAM",
            AP611COIN = "AP611COIN",
            AP611OTPK = "AP611OTPK",
            AP611OTPM = "AP611OTPM",
            AP611OTPA = "AP611OTPA",
            AP611PROT = "AP611PROT",
            AP611CPAY = "AP611CPAY",
            AP611WCPY = "AP611WCPY",
            AP611NMVS = "AP611NMVS",
            AP611LFMB = "AP611LFMB",
            AP611LFRM = "AP611LFRM",
            AP611LFVM = "AP611LFVM",
            AP611MXBN = "AP611MXBN",
            AP611RMBN = "AP611RMBN",
            AP611RMMT = "AP611RMMT",

            AP612CTID = "AP612CTID",
            AP612DEDA = "AP612DEDA",
            AP612TMPR = "AP612TMPR",
            AP612DEMT = "AP612DEMT",
            AP612DEAM = "AP612DEAM",
            AP612COIN = "AP612COIN",
            AP612OTPK = "AP612OTPK",
            AP612OTPM = "AP612OTPM",
            AP612OTPA = "AP612OTPA",
            AP612PROT = "AP612PROT",
            AP612CPAY = "AP612CPAY",
            AP612WCPY = "AP612WCPY",
            AP612NMVS = "AP612NMVS",
            AP612LFMB = "AP612LFMB",
            AP612LFRM = "AP612LFRM",
            AP612LFVM = "AP612LFVM",
            AP612MXBN = "AP612MXBN",
            AP612RMBN = "AP612RMBN",
            AP612RMMT = "AP612RMMT",

            AP6 = "AP6",
            CTID = "CTID",
            DEDA = "DEDA",
            TMPR = "TMPR",
            DEMT = "DEMT",
            DEAM = "DEAM",
            COIN = "COIN",
            OTPK = "OTPK",
            OTPM = "OTPM",
            OTPA = "OTPA",
            PROT = "PROT",
            CPAY = "CPAY",
            WCPY = "WCPY",
            NMVS = "NMVS",
            LFMB = "LFMB",
            LFRM = "LFRM",
            LFVM = "LFVM",
            MXBN = "MXBN",
            RMBN = "RMBN",
            RMMT = "RMMT",
            APSGNRN = "APSGNRN",
            APMBI = "APMBI";
        private const int BILLING_ADDRESS_LENGTH = 25;

        #endregion
    }
}
