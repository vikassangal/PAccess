--  Generate SQL 
--  Version:                   	V5R3M0 040528 
--  Generated on:              	02/27/07 07:39:18 
--  Relational Database:       	DVLG 
--  Standards Option:          	DB2 UDB iSeries 

SET PATH *LIBL ;   

CREATE PROCEDURE PACCESS/RELEASEASSIGNEDBED ( 
	IN P_FACILITY_ID INTEGER , 
	IN P_ACCOUNT_NUMBER INTEGER ) 
	DYNAMIC RESULT SETS 1 
	LANGUAGE SQL 
	SPECIFIC PACCESS/RELASSGBED 
	NOT DETERMINISTIC 
	MODIFIES SQL DATA 
	CALLED ON NULL INPUT 

	P1 : BEGIN 
		 
		DECLARE NURSING_STATION VARCHAR ( 2 ) ; 
		DECLARE ROOM VARCHAR ( 4 ) ; 
		DECLARE BED VARCHAR ( 2 ) ;  

		SELECT	LPNS , LPROOM , LPBED 
		INTO	NURSING_STATION , ROOM , BED 
		FROM	HPADLPP 
		WHERE	LPHSP# = P_FACILITY_ID 
		AND	LPACCT = P_ACCOUNT_NUMBER ; 	 

		IF	TRIM ( NURSING_STATION ) <> '' THEN 
	 
	--		Update the patient physical file 
		 
			UPDATE	HPADLPP 
				SET	LPNS	= '' , 
					LPROOM	= 0 , 
					LPBED	= '' 				 

				WHERE	LPHSP#	= P_FACILITY_ID 
				AND	LPACCT	= P_ACCOUNT_NUMBER 
	;						 

	--		and the Bed file 		 

			UPDATE HPADLRP 
  
				SET	LRFLG	= '' , 
					LRACCT	= 0 , 
					LRPNM	= '' 	  

				WHERE	LRHSP#	= P_FACILITY_ID 
				AND	LRNS	= NURSING_STATION 
				AND	LRROOM	= ROOM 
				AND	LRBED	= BED ;		 

		ELSE 		 

			SELECT	LRNS , LRROOM , LRBED 
				INTO	NURSING_STATION , ROOM , BED 
				FROM	HPADLRP 
				WHERE	LRHSP# = P_FACILITY_ID 
				AND	LRACCT = P_ACCOUNT_NUMBER ; 
			 
			IF TRIM ( NURSING_STATION ) <> '' THEN 

	--			update the Bed file only (the bed is only reserved) 

				UPDATE HPADLRP 

				SET	LRFLG	= '' , 
					LRACCT	= 0 , 
					LRPNM	= ''   

				WHERE	LRHSP#	= P_FACILITY_ID 
				AND	LRNS	= NURSING_STATION 
				AND	LRROOM	= ROOM	 
				AND	LRBED	= BED ; 	  

			END IF ; 		 
		END IF ; 

		END P1  ;
