-- *****************************************************************************/
-- * PatientPackage                                                            */
-- *****************************************************************************/
-- *****************************************************************************/
-- * Copyright (C) 2004, Perot Systems Corporation. All right reserved.        */
-- * Developer: kevin sedota   (kevin.sedota@ps.net)                           */
-- * Started:  07/02/2004                                                      */
-- *****************************************************************************/
DROP PROCEDURE PACCESS.DROP_PROC;
--go
-- *****************************************************************************/
-- * SP Definition - PSCKJS.DROP_PROC                                          */
-- *    Params     - @SCHEMANAME - Name of the schema where the proc resides   */
-- *    Params     - @PROCNAME	- Name of the proc to drop                     */
-- *****************************************************************************/
 CREATE PROCEDURE PACCESS.DROP_PROC(
 	IN P_SCHNAME VARCHAR(32),IN P_PROCNAME VARCHAR(32))        
  LANGUAGE SQL                                                
    chk1: begin atomic                                        
      DECLARE STMT CHAR(3000);                                
      DECLARE CNT BIGINT;                                     
      set cnt = 0;                                            
      select count(*) into cnt from qsys2.sysprocs            
       where routine_name = P_PROCNAME           
       and routine_schema = P_SCHNAME;                        
      if cnt > 0 then                                         
         SET STMT =                                           
          'drop procedure ' || P_SCHNAME || '.' || P_PROCNAME;     
         PREPARE S1 FROM STMT;                                
         EXECUTE S1;                                          
      end if;          
 end chk1; 