/**********************************************************************/
/*                                                                    */
/* iSeries400    SLACEMCT    - STORED PROCEDURE FOR PX                */
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
/*                                                                    */
/**********************************************************************/
/*  Date        Programmer          Modification Description          */
/**********************************************************************/
/* 02/26/2008   Kevin Sedota        NEW STORED PROCEDURE              */
/**********************************************************************/
/* SP Definition - PACCESS.SELECTACCTEMERGCONTACT                     */
/*    Params     - P_HSP	- an HSP Code							  */
/*    Params     - P_MRN    - a Medical record number                 */
/*    Params     - P_ACCOUNTNUMBER	- an account number               */
/**********************************************************************/

SET PATH *LIBL ;

CREATE PROCEDURE SELECTACCTEMERGCONTACT (
        IN P_HSP INTEGER ,
        IN P_MRN INTEGER ,
        IN P_ACCOUNTNUMBER INTEGER )
        DYNAMIC RESULT SETS 1
        LANGUAGE SQL
        SPECIFIC SLACEMCT
        NOT DETERMINISTIC
        MODIFIES SQL DATA
        CALLED ON NULL INPUT
        SET OPTION DBGVIEW =*SOURCE

        P1 : BEGIN
		
        DECLARE CURSOR3 CURSOR FOR
        SELECT
        LACNM AS CONTACTNAME ,  --CHAR(25) CCSID 37 NOT NULL DEFAULT ''
        LACADR AS ADDRESS ,  --CHAR(25) CCSID 37 NOT NULL DEFAULT ''
        LACCIT AS CITY ,  --CHAR(15) CCSID 37 NOT NULL DEFAULT ''
        LACSTE AS STATE ,  --CHAR(2) CCSID 37 NOT NULL DEFAULT ''
        LACZPA AS ZIP ,  --CHAR(5) CCSID 37 NOT NULL DEFAULT ''
        LACZ4A AS ZIPEXT ,  --CHAR(4) CCSID 37 NOT NULL DEFAULT ''
        LACRCD AS RELATIONSHIPCODE ,  --CHAR(2) CCSID 37 NOT NULL DEFAULT ''
        LACACD AS AREACODE ,  --DECIMAL(3, 0) NOT NULL DEFAULT 0
        LACPH# AS PHONENUMBER  --DECIMAL(7, 0) NOT NULL DEFAULT 0
        FROM HPADLAP3 WHERE LAHSP# = P_HSP
        AND LAMRC# = P_MRN ORDER BY LACNM ;  --emergency contacts

	OPEN CURSOR3 ;
	
	END P1 ; 