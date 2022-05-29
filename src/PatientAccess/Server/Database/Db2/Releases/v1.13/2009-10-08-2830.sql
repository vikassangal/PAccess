 /**********************************************************************/
/*                                                                    */
/* iSeries400    SLCNACDT    - STORED PROCEDURE FOR PX                */
/*                                                                    */
/*    ************************************************************    */
/*    * Perot Systems, Copyright 2003, All rights reserved(U.S.) *    */
/*    *                                                          *    */
/*    * This unpublished material is proprietary to Perot Sys.   *    */
/*    * The methods and techniques described herein are          *    */
/*    * considered trade secrets and/or confidential.            *    */
/*    * Reproduction or distribution, in whole or in part, is    *    */
/*    * forbidden except by express written permission of        *    */
/*    * Perot Systems, Inc.                                      *    */
/*    ************************************************************    */
/* Select general account data for an account                         */
/*                                                                    */
/**********************************************************************/
/*  Date        Programmer          Modification Description          */
/**********************************************************************/
/* 02/26/2008   Kevin Sedota        NEW STORED PROCEDURE              */
/* 02/03/2009	Deepa Raju			Retrieve PBAR Value Codes and	  */
/*									 Value Amounts from PM0001P		  */
/* 05/20/2009   Deepa Raghavaraju   Modified DNR field mapping to     */
/*                                   HXMRRVP.RVPCNY				      */
/* 07/23/2009   Deepa Raghavaraju   Retrieve Mother's Patient Account */
/*                                   Number from HPADLPP			  */
/* 10/10/2009   Smitha             Select PreopDate and Preocedure    */
*/
/**********************************************************************/
/* SP Definition - PACCESS.SELECTGENERALACCTDATA	                  */
/*    Params     - P_HSP	- an HSP Code							  */
/*    Params     - P_MRN    - a Medical record number                 */
/*    Params     - P_ACCOUNTNUMBER	- an account number               */
/**********************************************************************/

SET PATH *LIBL ;

CREATE PROCEDURE SELECTGENERALACCTDATA(
        IN P_HSP INTEGER ,
        IN P_MRN INTEGER ,
        IN P_ACCOUNTNUMBER INTEGER )
        DYNAMIC RESULT SETS 1
        LANGUAGE SQL
        SPECIFIC  SLCNACD
        NOT DETERMINISTIC
        MODIFIES SQL DATA
        CALLED ON NULL INPUT
        SET OPTION DBGVIEW =*SOURCE

        P1 : BEGIN
        DECLARE CURSOR1 CURSOR FOR
        SELECT
        A . LPHSP# AS HSPNUMBER ,
        A . LPACCT AS ACCOUNTNUMBER ,
        A . LPALOC AS ACCIDENTLOCATION ,
        A . LPATIM AS ACCIDENTTIME ,
        A . LPACTP AS ACCIDENTTYPE ,
        A . LPDIAG AS DIAGNOSIS ,
        A . LPPROCD AS PROCEDUREDATA,
        A . LPACST AS AUTOACCIDENTSTATE ,
        A . LPACCN AS ACCIDENTCOUNTRYCODE ,
        A . LPLAT AS TIMEOFADMISSION ,
        A . LPOC01 AS OCCURANCECODE1 ,
        A . LPOC02 AS OCCURANCECODE2 ,
        A . LPOC03 AS OCCURANCECODE3 ,
        A . LPOC04 AS OCCURANCECODE4 ,
        A . LPOC05 AS OCCURANCECODE5 ,
        A . LPOC06 AS OCCURANCECODE6 ,
        A . LPOC07 AS OCCURANCECODE7 ,
        A . LPOC08 AS OCCURANCECODE8 ,
        A . LPOA01 AS OCCURANCEDATE1 ,
        A . LPOA02 AS OCCURANCEDATE2 ,
        A . LPOA03 AS OCCURANCEDATE3 ,
        A . LPOA04 AS OCCURANCEDATE4 ,
        A . LPOA05 AS OCCURANCEDATE5 ,
        A . LPOA06 AS OCCURANCEDATE6 ,
        A . LPOA07 AS OCCURANCEDATE7 ,
        A . LPOA08 AS OCCURANCEDATE8 ,
        A . LPCI01 AS CONDITIONCODE1 ,
        A . LPCI02 AS CONDITIONCODE2 ,
        A . LPCI03 AS CONDITIONCODE3 ,
        A . LPCI04 AS CONDITIONCODE4 ,
        A . LPCI05 AS CONDITIONCODE5 ,
        A . LPCI06 AS CONDITIONCODE6 ,
        A . LPCI07 AS CONDITIONCODE7 ,
        A . LPCLVS AS CLERGYVISIT ,
        A . LPIPA AS IPACODE ,
        A . LPIPAC AS IPACLINICCODE ,
        A . LPLMEN AS LASTMENSTRATION ,
        A . LPWRNF AS FACILITYFLAG ,
        A . LPPSTO AS OPSTATUSCODE ,
        A . LPPSTI AS IPSTATUSCODE ,
        A . LPFBIL AS FINALBILLINGFLAG ,
        A . LPVIS# AS OPVISITNUMBER ,
        A . LPPOLN AS PENDINGPURGE ,
        A . LPUBAL AS UNBILLEDBALANCE ,
        A . LPMSV AS MEDICALSERVICECODE ,
        A . LPADT1 AS ADMITMONTH ,
        A . LPADT2 AS ADMITDAY ,
        A . LPADT3 AS ADMITYEAR ,
         --a.LPLAT    AS TIMEOFA
        A . LPLDD AS DISCHARGEDATE ,
        A . LPFC AS FINANCIALCLASSCODE ,
        A . LPPTYP AS PATIENTTYPE ,
        A . LPDCOD AS DISCHARGEDISPOSITIONCODE ,
        A . LPSPNC AS FIRSTSPANCODE ,
        A . LPSPFR AS FIRSTSPANFROMDATE ,
        A . LPSPTO AS FIRSTSPANTODATE ,
        A . LPINSN AS FIRSTSPANFACILITY ,
        A . LPSPN2 AS SECONDSPANCODE ,
        A . LPAPFR AS SECONDSPANFROMDATE ,
        A . LPAPTO AS SECONDSPANTODATE ,
        A . LPRSRC AS REFERRALSOURCE ,
        A . LPLMD AS LASTMAINTENANCEDATE ,
        A . LPLML AS LASTMAINTENANCELOGNUMBER ,
        A . LPLUL# AS UPDATELOGNUMBER ,
        A . LPCL01 AS CLINICCODE1 ,
        A . LPCL02 AS CLINICCODE2 ,
        A . LPCL03 AS CLINICCODE3 ,
        A . LPCL04 AS CLINICCODE4 ,
        A . LPCL05 AS CLINICCODE5 ,
        A . LPPUBL AS REREGISTER ,
        A . LPSCHD AS SCHEDULECODE ,
        A . LPPREOPD AS PREOPDATE ,
        A . LPMSV AS MEDICALSERVICECODE ,
		A . LPMPT# AS MOTHERSPATIENTACCOUNT ,
         --S . QTKEY AS MEDICALSERVICECODE ,
         --S . QTMSVD AS MEDICALSERVICEDESC ,
        SUP . RVPRGI AS ISPREGNANT ,
        FUS . PMTCH8 AS TOTALCHARGES ,
        FUS . PMVCD1 AS VALUECODE1 ,
        FUS . PMVCD2 AS VALUECODE2 ,
        FUS . PMVCD3 AS VALUECODE3 ,
        FUS . PMVCD4 AS VALUECODE4 ,
        FUS . PMVCD5 AS VALUECODE5 ,
        FUS . PMVCD6 AS VALUECODE6 ,
        FUS . PMVCD7 AS VALUECODE7 ,
        FUS . PMVCD8 AS VALUECODE8 ,
        FUS . PMVAM1 AS VALUEAMOUNT1 ,
        FUS . PMVAM2 AS VALUEAMOUNT2 ,
        FUS . PMVAM3 AS VALUEAMOUNT3 ,
        FUS . PMVAM4 AS VALUEAMOUNT4 ,
        FUS . PMVAM5 AS VALUEAMOUNT5 ,
        FUS . PMVAM6 AS VALUEAMOUNT6 ,
        FUS . PMVAM7 AS VALUEAMOUNT7 ,
        FUS . PMVAM8 AS VALUEAMOUNT8 ,
        MPI . RWCNFG AS CONFIDENTIALFLAG ,
        D . MDNPPV AS NPPVERSION ,
        D . MDNPPF AS OPTOUTFLAGS ,
		D . MDNPPS AS NPPSIGNATURESTATUS ,
		D . MDNPPD AS NPPDATESIGNED ,
        D . MDSMOK AS SMOKER ,
        D . MDBLDL AS BLOODLESS ,
        D . MDRORG AS RESISTANTORGANISM ,
        D . MDFDOB AS FATHERDOB ,
        D . MDMDOB AS MOTHERDOB ,
		RVP . RVPCNY AS DNR ,
        D . MDPPT# as PASSPORTNUMBER,
        D . MDICUN as PASSPORTCOUNTRY,
        C . LACMT1 AS COMMENTLINE1 ,
        C . LACMT2 AS COMMENTLINE2 ,
        C . LATECR AS TENETCARE ,
        H . LFRCID AS LOCKINDICATOR ,
        H . LFUSER AS LOCKERPBARID ,
        H . LFTDAT AS LOCKDATE ,
        H . LFTTIM AS LOCKTIME ,
        EMER . EDRACD AS READMITCODE ,
        EMER . EDFRCD AS REFERRALFACILITY ,
        EMER . EDRTYP AS REFERRALTYPE ,
        EMER . EDAVTM AS ARRIVALTIME ,
        EMER . EDARVC AS MODEOFARRIVAL
        
        FROM HPADLPP A					-- Patient Physical File (HPDATA1)
        JOIN NOHLHSPP FAC				-- Hospital Definition Master (NMDATA1)
			ON ( FAC . HHHSPN = P_HSP )
        JOIN HPADMDP D					-- Patient Demographics (HPDATA1)
			ON ( D . MDHSP# = P_HSP
			AND D . MDMRC# = P_MRN )
        LEFT OUTER JOIN HPADLAP9 C		-- Patient comments/ states aux file (HPDATA1)
			ON ( C . LAHSP# = P_HSP
			AND C . LAACCT = P_ACCOUNTNUMBER )
        LEFT OUTER JOIN PM0001P FUS		-- FUS Patient Master File (PADATA)
			ON ( FUS . PMHSPC = FAC . HHHSPC
			AND FUS . PMPT#9 = P_ACCOUNTNUMBER )
        LEFT OUTER JOIN HXMRRV2P SUP	-- Supplemental Abstracts file (HPDATA1)
			ON ( SUP . RVHSP# = P_HSP
			AND SUP . RVPT#9 = P_ACCOUNTNUMBER  )
        LEFT OUTER JOIN HXMRRWP MPI		-- Master Patient Index (HPDATA1)
          ON ( MPI . RWHSP# = P_HSP
			AND MPI . RWACCT = P_ACCOUNTNUMBER )
		LEFT OUTER JOIN HXMRRVP RVP		-- Medical Records Abstracts Physical File (HPDATA1)
			ON ( RVP . RVHSP# = P_HSP
			AND RVP . RVACCT = P_ACCOUNTNUMBER )
        LEFT JOIN NQHRLFP H				-- HRAM locks (XADATA)
			ON ( LPAD ( TRIM ( CHAR ( P_ACCOUNTNUMBER  ) ) , 9 , '0' ) = SUBSTR
			( H . LFKEY , 1 , 9 ) AND H . LFHSP# = P_HSP )
        LEFT OUTER JOIN NDED01P EMER	-- Emergency Dept. log file (NMDATA1)
			ON ( EMER . EDHSP# = P_HSP
			AND EMER . EDACCT = P_ACCOUNTNUMBER )
        WHERE A . LPHSP# = P_HSP
        AND A . LPACCT = P_ACCOUNTNUMBER ;

 
	OPEN CURSOR1 ;
	
	END P1 ;