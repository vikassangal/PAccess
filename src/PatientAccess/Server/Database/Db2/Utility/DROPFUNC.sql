-- *****************************************************************************/
-- * Copyright (C) 2004, Perot Systems Corporation. All right reserved.        */
-- * Developer: kevin sedota   (kevin.sedota@ps.net)                           */
-- * Started:  09/29/2004                                                      */
-- *****************************************************************************/
DROP PROCEDURE PACCESS.DROP_FUNC;
--go
-- *****************************************************************************/
-- * SP Definition - PSCKJS.DROP_FUNC                                          */
-- *    Params     - @SCHEMANAME - Name of the schema where the func resides   */
-- *    Params     - @P_FUNCNAME	- Name of the func to drop                 */
-- *****************************************************************************/
 CREATE PROCEDURE PACCESS.DROP_FUNC(
 	IN P_SCHNAME VARCHAR(32),IN P_FUNCNAME VARCHAR(32))        
  LANGUAGE SQL                                                
    chk1: begin atomic                                        
      DECLARE STMT CHAR(3000);                                
      DECLARE CNT BIGINT;                                     
      set cnt = 0;                                            
      select count(*) into cnt from qsys2.sysfuncs            
       where routine_name = P_FUNCNAME           
       and routine_schema = P_SCHNAME;                        
      if cnt > 0 then                                         
         SET STMT =                                           
          'drop function ' || P_SCHNAME || '.' || P_FUNCNAME;     
         PREPARE S1 FROM STMT;                                
         EXECUTE S1;                                          
      end if;          
 end chk1;  