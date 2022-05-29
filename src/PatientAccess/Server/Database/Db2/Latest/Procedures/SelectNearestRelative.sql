 /**********************************************************************/
/*                                                                    */
/* iSeries400    SLACNRREL   - STORED PROCEDURE FOR PX                */
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
/* 04/26/2006   Melissa Bouse       NEW STORED PROCEDURE              */
/**********************************************************************/
/* SP Definition - PACCESS.SELECTNEARESTRELATIVE	                  */
/*    Params     - P_HSP	- an HSP Code							  */
/*    Params     - P_MRN    - a Medical record number                 */
/*    Params     - P_ACCOUNTNUMBER	- an account number               */
/**********************************************************************/

SET PATH *LIBL ;

CREATE PROCEDURE SELECTNEARESTRELATIVE (
        IN P_HSP INTEGER ,
        IN P_MRN INTEGER ,
        IN P_ACCOUNTNUMBER INTEGER )
        DYNAMIC RESULT SETS 10
        LANGUAGE SQL
        SPECIFIC SLACNRREL
        NOT DETERMINISTIC
        MODIFIES SQL DATA
        CALLED ON NULL INPUT
        SET OPTION DBGVIEW =*SOURCE

        P1 : BEGIN

        DECLARE CURSOR4 CURSOR FOR
        SELECT LARNM AS CONTACTNAME ,  --CHAR(25) CCSID 37 NOT NULL DEFAULT ''
        LARADR AS ADDRESS ,  --CHAR(25) CCSID 37 NOT NULL DEFAULT ''
        LARCIT AS CITY ,  --CHAR(15) CCSID 37 NOT NULL DEFAULT ''
        LARSTE AS STATE ,  --CHAR(2) CCSID 37 NOT NULL DEFAULT ''
        LARZPA AS ZIP ,  --CHAR(5) CCSID 37 NOT NULL DEFAULT ''
        LARZ4A AS ZIPEXT ,  --CHAR(4) CCSID 37 NOT NULL DEFAULT ''
        LANRCD AS RELATIONSHIPCODE ,  --CHAR(2) CCSID 37 NOT NULL DEFAULT ''
        LARACD AS AREACODE ,  --DECIMAL(3, 0) NOT NULL DEFAULT 0
        LARPH# AS PHONENUMBER  --DECIMAL(7, 0) NOT NULL DEFAULT 0
        FROM HPADLAP2
        WHERE LAHSP# = P_HSP
        AND LAMRC# = P_MRN ORDER BY LARNM ;  -- Nearest relative

	OPEN CURSOR4 ;
	
	END P1 ;