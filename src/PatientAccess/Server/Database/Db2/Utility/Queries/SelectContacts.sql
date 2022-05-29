-- *****************************************************************************/
-- * PatientPackage                                                            */
-- *****************************************************************************/
-- *****************************************************************************/
-- * Copyright (C) 2004, Perot Systems Corporation. All right reserved.        */
-- * Developer: jimmy zhang                                                    */
-- * Started:  02/07/2004                                                      */
-- *****************************************************************************/
-- *****************************************************************************/
-- * Drop the proc if it exists
-- *****************************************************************************/

CALL DROP_PROC('PACCESS','SELECTCONTACTS');
--go

-- *****************************************************************************/
-- * SP Definition - PACCESS.SELECTCONTACTS                                    */
-- *    Params     - P_HSP	- an HSP Code                                      */
-- *    Params     - P_MRC	- a medical record number                          */
-- *****************************************************************************/
CREATE PROCEDURE PACCESS.SELECTCONTACTS ( 
	IN P_HSP INTEGER,
	IN P_MRC INTEGER) 
	DYNAMIC RESULT SETS 2 
	LANGUAGE SQL 
	SPECIFIC PACCESS.SELECTCONTACTS 
	NOT DETERMINISTIC 
	MODIFIES SQL DATA 
	CALLED ON NULL INPUT 
	P1 : BEGIN 		
	
	    --emergency contacts
		DECLARE CURSOR1 CURSOR FOR         
		SELECT LACNM as CONTACTNAME,       --CHAR(25) CCSID 37 NOT NULL DEFAULT ''
		       LACADR as ADDRESS,          --CHAR(25) CCSID 37 NOT NULL DEFAULT ''  
		       LACCIT as CITY,             --CHAR(15) CCSID 37 NOT NULL DEFAULT ''
		       LACSTE as STATE,            --CHAR(2) CCSID 37 NOT NULL DEFAULT ''
		       LACZPA as ZIP,              --CHAR(5) CCSID 37 NOT NULL DEFAULT ''
		       LACZ4A as ZIPEXT,           --CHAR(4) CCSID 37 NOT NULL DEFAULT ''
		       LACRCD as RELATIONSHIPCODE, --CHAR(2) CCSID 37 NOT NULL DEFAULT '' 
		       LACACD as AREACODE,         --DECIMAL(3, 0) NOT NULL DEFAULT 0  
               LACPH# as PHONENUMBER	   --DECIMAL(7, 0) NOT NULL DEFAULT 0 	
		FROM HPDATA1.HPADLAP3		     
		WHERE LAHSP# = P_HSP AND
		      LAMRC# = P_MRC
		ORDER BY LACNM; 		
		
		--nearest relative contacts
		DECLARE CURSOR2 CURSOR FOR        
		SELECT LARNM  as CONTACTNAME,      --CHAR(25) CCSID 37 NOT NULL DEFAULT ''
		       LARADR as ADDRESS,          --CHAR(25) CCSID 37 NOT NULL DEFAULT ''  
		       LARCIT as CITY,             --CHAR(15) CCSID 37 NOT NULL DEFAULT ''
		       LARSTE as STATE,            --CHAR(2) CCSID 37 NOT NULL DEFAULT ''
		       LARZPA as ZIP,              --CHAR(5) CCSID 37 NOT NULL DEFAULT ''
		       LARZ4A as ZIPEXT,           --CHAR(4) CCSID 37 NOT NULL DEFAULT ''
		       LANRCD as RELATIONSHIPCODE, --CHAR(2) CCSID 37 NOT NULL DEFAULT '' 
		       LARACD as AREACODE,         --DECIMAL(3, 0) NOT NULL DEFAULT 0  
               LARPH# as PHONENUMBER	   --DECIMAL(7, 0) NOT NULL DEFAULT 0 	
		FROM HPDATA1.HPADLAP2		     
		WHERE LAHSP# = P_HSP AND
		      LAMRC# = P_MRC
		ORDER BY LARNM; 
		
		OPEN CURSOR1 ; 
		OPEN CURSOR2 ; 
			

		END P1  ;  