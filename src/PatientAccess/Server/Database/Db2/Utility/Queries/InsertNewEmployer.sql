-- *****************************************************************************/
-- * Copyright (C) 2004, Perot Systems Corporation. All right reserved.        */
-- * Developer: Kevin Sedota   (kevin.sedota@ps.net)                           */
-- * Started:  10/07/2004                                                      */
-- *****************************************************************************/
-- *****************************************************************************/
-- * Drop the proc if it exists
-- *****************************************************************************/

CALL DROP_PROC('PACCESS','INSERTNEWEMPLOYER');
--go

-- *****************************************************************************/
-- * SP Definition - PACCESS.INSERTNEWEMPLOYER                                 */
-- *    Params     - P_EMFUUN	- Folloup Unit Number                          */
-- *    Params     - P_EMNAME	- Employer name                                */
-- *    Params     - P_EMURFG	- U/R FLAG                                     */
-- *    Params     - P_EMADDT	- ADD DATE                                     */
-- *    Params     - P_EMLMDT	- Last Maintenance Date                        */
-- *    Params     - P_EMDLDT	- Delete date                                  */
-- *    Params     - P_EMCODE	- Employer Code                                */
-- *    Params     - P_EMACNT	- Address count                                */
-- *    Params     - P_EMUSER	- Added by                                     */
-- *    Params     - P_EMNEID	- National Employer ID                         */
-- *****************************************************************************/
CREATE PROCEDURE PACCESS.INSERTNEWEMPLOYER ( 
	IN P_EMFUUN VARCHAR(3),
	IN P_EMNAME	VARCHAR(25),
	IN P_EMURFG VARCHAR(1),
	IN P_EMADDT INTEGER,
	IN P_EMLMDT INTEGER,
	IN P_EMDLDT INTEGER,
	IN P_EMCODE INTEGER,
	IN P_EMACNT INTEGER,
	IN P_EMUSER VARCHAR(10),
	IN P_EMNEID VARCHAR(10)) 
	DYNAMIC RESULT SETS 1 
	LANGUAGE SQL 
	SPECIFIC PACCESS.INSERTNEWEMPLOYER 
	NOT DETERMINISTIC 
	MODIFIES SQL DATA 
	CALLED ON NULL INPUT 
	P1 : BEGIN 
		
		INSERT INTO PADATA.NCEM10P VALUES(P_EMFUUN, P_EMNAME, P_EMURFG, P_EMADDT, P_EMLMDT,
											P_EMDLDT, P_EMCODE, P_EMACNT, P_EMUSER, P_EMNEID);
		END P1  ;  