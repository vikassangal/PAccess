-- *****************************************************************************/
-- * PatientPackage                                                            */
-- *****************************************************************************/
-- *****************************************************************************/
-- * Copyright (C) 2004, Perot Systems Corporation. All right reserved.        */
-- * Developer: jimmy zhang                                                    */
-- * Started:  11/15/2004                                                      */
-- *****************************************************************************/
-- *****************************************************************************/
-- * Drop the proc if it exists
-- *****************************************************************************/

CALL DROP_PROC('PACCESS','SELECTINSURANCES');
--go

-- *****************************************************************************/
-- * SP Definition - PACCESS.SELECTINSURANCES                                  */
-- *    Params     - P_HSP	- an HSP Code                                  */
-- *    Params     - P_ACCT	- an account                                   */
-- *****************************************************************************/
CREATE PROCEDURE PACCESS.SELECTINSURANCES ( 
	IN P_HSP INTEGER,
	IN P_ACCT INTEGER) 
	DYNAMIC RESULT SETS 1 
	LANGUAGE SQL 
	SPECIFIC PACCESS.SELECTINSURANCES 
	NOT DETERMINISTIC 
	MODIFIES SQL DATA 
	CALLED ON NULL INPUT 
	P1 : BEGIN 		
	
		DECLARE CURSOR1 CURSOR FOR 
		SELECT i.HBHSP# as HSPNUMBER, 
		       i.HBACCT as ACCOUNTNUMBER,		
			   i.HBSLNM as INSUREDLASTNAME,
			   i.HBSFNM as INSUREDFIRSTNAME, 
			   i.HBSMI  as INSUREDMIDDLEINITIAL,
			   i.HBSSEX as INSUREDSEX,
			   i.HBSRCD as INSUREDRELATIONSHIP,	
			   i.HBSBID as INSUREDIDENTIFIER,	
			   i.HBSCUN as INSUREDCOUNTRYCODE,   
			   i.HBSBTH as INSUREDBIRTHDATE,
			   i.HBIID# as INSURANCECERTIFICATIONNUM,		
			   
			   i.HBGRP# as GROUPNUMBER,
			   i.HBPOL# as POLICYNUMBER,
			   i.HBEVC# as EVCNUMBER,
			   i.HBCON# as AUTHORIZATIONNUMBER,
			   i.HBAUDY as AUTHORIZEDDAYS,
			   i.HBMCDT as ISSUEDATE,
			   			   	   
			   i.HBJADR as INSUREDADDRESS,  
			   i.HBJCIT as INSUREDCITY,
			   i.HBJSTE as INSUREDSTATE,
			   i.HBJZIP as INSUREDZIP,
			   i.HBJZP4 as INSUREDZIPEXT,
			   
			   i.HBJACD as INSUREDAREACODE1,
			   i.HBJPH# as INSUREDPHONENUMBER1,			   
			   i.HBRACD as INSUREDAREACODE2,
			   i.HBRPH# as INSUREDPHONENUMBER2,				   
			   i.HBCLPH as INSUREDCELLPHONE,
			   
			   i.HBINS# as INSURANCECOMPANYNUMBER,
			   i.HBPTY  as PRIORITYCODE,
			   
 			   i.HBIAD1 as BILLINGADDRESS1,	
 			   i.HBIAD2 as BILLINGCITYSTATECOUNTRY,				
			   i.HBIAD3 as BILLINGZIPZIPEXTPHONE,

			   p.LAESC1 as PRIMARYEMPSTATUS,
			   p.LAGCD1 as PRIMARYEMPCODE,	
			   p.LAENM1 as PRIMARYEMPNAME,
			   p.LAEA01 as PRIMARYEMPADDRESS,	
			   p.LAELO1 as PRIMARYLOC,     --city, state 
			   p.LAEZP1 as PRIMARYEMPZIP,
			   p.LAEZ41 as PRIMARYEMPZIPEXT,
			   p.LAEID1 as PRIMARYPHONE,	

			   s.LAESC2 as SECONDARYEMPSTATUS,
			   s.LAGCD2 as SECONDARYEMPCODE,	
			   s.LAENM2 as SECONDARYEMPNAME,
			   s.LAEA02 as SECONDARYEMPADDRESS,	
			   s.LAELO2 as SECONDARYLOC,     --city, state 
			   s.LAEZP2 as SECONDARYEMPZIP,
			   s.LAEZ42 as SECONDARYEMPZIPEXT,
			   s.LAEID2 as SECONDARYPHONE		
					
		FROM HPDATA1.HPADHBP i
		     left outer join HPDATA1 . HPADLAP7 p on ( i.HBMRC# = p.LAMRC# and i.HBHSP# = p.LAHSP# )
	             left outer join HPDATA1 . HPADLAP8 s on ( i.HBMRC# = s.LAMRC# and i.HBHSP# = s.LAHSP# )		
		WHERE i.HBHSP# = P_HSP AND
		      i.HBACCT = P_ACCT AND ( i.HBPTY = '1' OR i.HBPTY = '2' )
		ORDER BY PRIORITYCODE; 
		OPEN CURSOR1 ; 
		END P1  ;  