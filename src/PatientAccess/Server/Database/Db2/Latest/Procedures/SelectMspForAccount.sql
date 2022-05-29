/*************I********************************************************/
/*            I                                                       */
/* iSeries400 I    SELECTMSPFORACCOUNT        -STORED PROC FOR PX     */
/*            I                                                       */
/*    ************************************************************    */
/*    * Perot Systems, Copyright 2003, All rights reserved(U.S.) *    */
/*    *       I                                                  *    */
/*    * This unpublished material is proprietary to Perot Sys.   *    */
/*    * The methods and techniques described herein are          *    */
/*    * considered trade secrets and/or confidential.            *    */
/*    * Reproduction or distribution, in whole or in part, is    *    */
/*    * forbidden except by express written permission of        *    */
/*    * Perot Systems, Inc.                                      *    */
/*    ************************************************************    */
/*            I                                                       */
/*************I********************I***********************************/
/*  Date      I  Programmer        I  Modification Description        */
/*************I********************I***********************************/
/* 04/26/2006 I  Melissa Bouse     I NEW STORED PROCEDURE             */
/*************I********************I***********************************/
/* 07/06/2006 I  Melissa Bouse     I Modified STORED PROCEDURE        */
/* 11/29/2006 I  Tom Gauthreaux    I Modified Stored Proc             */
/* 02/28/2008 I  KJS               I remove CHGQRYA call              */
/* 02/29/2008 I  Jithin A          I Added the column TODAYSVISITBLACKLUNG */
/*************I********************I***********************************/
SET PATH *LIBL ;

CREATE PROCEDURE SELECTMSPFORACCOUNT (
        IN P_HSP INTEGER ,
        IN P_MRN INTEGER ,
        IN P_ACCOUNTNUMBER INTEGER )
        DYNAMIC RESULT SETS 1
        LANGUAGE SQL
        SPECIFIC SELMSPFACC
        NOT DETERMINISTIC
        MODIFIES SQL DATA
        CALLED ON NULL INPUT
        SET OPTION DBGVIEW =*SOURCE
        P1 : BEGIN

DECLARE CURSOR1 CURSOR FOR

SELECT
        MSPPTNM           AS      PATIENTNAME ,
        MSPBL             AS      BLACKLUNG ,
        MSPGP             AS      GOVTPGM ,
        MSPVA             AS      DVA ,
        MSPWA             AS      WORKRELATED ,
        MSPNA             AS      NONWORKRELATED ,
        MSPOP             AS      OTHERPARTY ,
        MSPAG             AS      AGE ,
        MSPEMPL           AS      EMPLOYED ,
        MSPSEMPL          AS      SPOUSEEMPLOYED ,
        MSPAGGHP          AS      AGEGHP ,
        MSPAGGHP20        AS      AGEGHP20 ,
        MSPAGGHPF         AS      AGEGHPTYPE ,
        MSPAGHP20P        AS      AGESPOUSEOVERX ,
        MSPDI             AS      DISABILITY ,
        MSPOEMPL          AS      OTHEREMPLOYED ,
        MSPDIGHP          AS      DISGHP ,
        MSPDIGHP00        AS      DISGHP100 ,
        MSPDGHP00P        AS      DISSPOUSEOVERX ,
        MSPDGHP00F        AS      DISFAMILYMEMBEROVERX ,
        MSPDIGHPF         AS      DISGHPTYPE ,
        MSPES             AS      ESRD ,
        MSPESGHP          AS      ESRDGHP ,
        MSPESKT           AS      ESRDKIDNEYTRANSPLANT ,
        MSPESDT           AS      ESRDDIALYSISTRTMNT ,
        MSPES30MO         AS      ESRD30MOCOORD ,
        MSPESMULTI        AS      ESRDMULTIENTITLE ,
        MSPESIA           AS      ESRDINITIALENTITLE ,
        MSPESOTHR         AS      ESRDNOTAGEDIS ,
        MSPGHP            AS      GROUPHEALTHPLAN ,
        MSPAUTO           AS      AUTOACCIDENT ,
        MSPINS#           AS      INSURANCESELECTED ,
        MSPPNID1          AS      INS1 ,
        MSPPNID2          AS      INS2 ,
        MSPBLDT           AS      BLACKLUNGDATE ,
        MSPAIDT           AS      ACCIDENTINJURYDATE ,
        MSPESKTDT         AS      TRANSPLANTDATE ,
        MSPESDTDT         AS      DIALYSISDATE ,
        MSPESSDTDT        AS      SELFTRAININGDATE ,
        MSPEMPLRDT        AS      RETIREDATE ,
        MSPSEMPRDT        AS      SPOUSERETIREDATE ,
        MSPSENM           AS      SPOUSEEMPLOYERNAME ,
        MSPSEADR          AS      SPOUSEEMPLOYERADDRESS ,
        MSPSECIT          AS      SPOUSEEMPLOYERCITY ,
        MSPSESTE          AS      SPOUSEEMPLOYERSTATE ,
        MSPSEZIP          AS      SPOUSEEMPLOYERZIP ,
        MSPSEZP4          AS      SPOUSEEMPLOYERZIP4 ,
        MSPSEMPLN         AS      SPOUSENEVEREMPLOYED,
        MSPFORMVER        AS      MSPVERSION ,
        MSPNFALT          AS      NOFAULTAVAILABLE ,
        MSPLIABL          AS      LIABILITYAVAILABLE,
        MSPEMPLN          AS      PATIENTNEVEREMPLOYED,
        MSPFENM           AS      FAMILYMBREMPLOYERNAME,
        MSPFEADR          AS      FAMILYMBREMPLOYERADDRESS,
        MSPFECIT          AS      FAMILYMBREMPLOYERCITY,
        MSPFESTE          AS      FAMILYMBREMPLOYERSTATE,
        MSPFEZIP          AS      FAMILYMBREMPLOYERZIP,
        MSPFEZP4          AS      FAMILYMBREMPLOYERZIP4,
        MSPBLTDY          AS      TODAYSVISITBLACKLUNG

         FROM    NHMSP01P

         WHERE MSPHSP#                  = P_HSP
        AND      MSPPT#9        = P_ACCOUNTNUMBER
        AND      MSPMRC#        = P_MRN ;

--CALL QSYS/QCMDEXC ( 'CHGQRYA QRYOPTLIB(PACCESS)' , 0000000026.00000 ) ;

OPEN CURSOR1 ;
END P1  ;
