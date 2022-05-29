                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  SELECTNEXTMRN - SQL PROC FOR PX                      */        
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
/* 08/02/2006 I                    I NEW STORED PROCEDURE             */        
/*************I********************I***********************************/        
                                                                                
SET PATH *LIBL ;                                                                
                                                                                
                                                                                
                                                                                
CREATE PROCEDURE SELECTNEXTMRN (                                                
                                                                                
        IN P_HSP INTEGER ,                                                      
                                                                                
        IN P_HSPCODE CHAR(3) )                                                  
                                                                                
        DYNAMIC RESULT SETS 1                                                   
                                                                                
        LANGUAGE SQL                                                            
                                                                                
        SPECIFIC SELNEXTMRN                                                     
                                                                                
        NOT DETERMINISTIC                                                       
                                                                                
        MODIFIES SQL DATA                                                       
                                                                                
        CALLED ON NULL INPUT                                                    
                                                                                
        SET OPTION DBGVIEW =*SOURCE                                             
                                                                                
        P1 : BEGIN                                                              
                                                                                
                DECLARE RETURNMRN INTEGER ;                                     
                                                                                
                DECLARE NEXTMRN INTEGER ;                                       
                                                                                
                DECLARE NBRPATIENTSWITHMRN INTEGER ;                            
                                                                                
                DECLARE NBRDUPKEYSWITHMRN INTEGER ;                             
                                                                                
                DECLARE NONUMBERSAVAILABLE INTEGER ;                            
                                                                                
                DECLARE CURSOR1 CURSOR FOR                                      
                                                                                
                SELECT NEXTMRN AS NEXTMRN                                       
                FROM SYSIBM/SYSDUMMY1 ;                                         
                                                                                
                SET RETURNMRN = 0 ;                                             
                                                                                
                SET NONUMBERSAVAILABLE = 0 ;                                    
                                                                                
                                                                                
                                                                                
                WHILE ( RETURNMRN = 0 AND NONUMBERSAVAILABLE < 2 ) DO           
                                                                                
                -- take a number                                                
                                                                                
                SELECT QCNMR#                                                   
                                                                                
                INTO NEXTMRN                                                    
                                                                                
                FROM HPADQCP2                                                   
                WHERE QCHSP# = P_HSP ;                                          
                                                                                
                 -- check to make sure it is valid;                             
                                                                                
                SELECT COUNT ( * ) INTO NBRPATIENTSWITHMRN                      
                                                                                
                FROM HPADMDP                                                    
                                                                                
                WHERE MDHSP# = P_HSP                                            
                                                                                
                AND ( MDMRC# = NEXTMRN ) ;                                      
                                                                                
                SELECT COUNT ( * ) INTO NBRDUPKEYSWITHMRN                       
                                                                                
                FROM HPADWQP                                                    
                                                                                
                WHERE WQHSP# = P_HSP                                            
                                                                                
                AND WQMRC# = NEXTMRN                                            
                                                                                
                AND WQID = '2' ;                                                
                                                                                
                                                                                
                -- if one already exists then this number                       
                -- is not 'available' so try again                              
                                                                                
                IF ( NBRPATIENTSWITHMRN <> 0                                    
                OR NBRDUPKEYSWITHMRN <> 0 ) THEN                                
                                                                                
                IF ( NEXTMRN >= 979999999 ) THEN                                
                                                                                
                UPDATE HPADQCP2                                                 
                                                                                
                SET QCNAC# = 1                                                  
                                                                                
                WHERE QCHSP# = P_HSP ;                                          
                                                                                
                SET NONUMBERSAVAILABLE = NONUMBERSAVAILABLE + 1 ;               
                                                                                
                ELSE                                                            
                                                                                
                UPDATE HPADQCP2                                                 
                                                                                
                SET QCNMR# = QCNMR# + 1                                         
                                                                                
                WHERE QCHSP# = P_HSP ;                                          
                                                                                
                END IF ;                                                        
                                                                                
                ELSE                                                            
                                                                                
                SET RETURNMRN = NEXTMRN ;                                       
                                                                                
                END IF ;                                                        
                                                                                
                END WHILE ;                                                     
                                                                                
                IF ( NONUMBERSAVAILABLE > 2 )                                   
                THEN                                                            
                                                                                
                SET NEXTMRN = 0 ;                                               
                                                                                
                ELSEIF ( RETURNMRN = 979999999 ) THEN                           
                                                                                
                UPDATE HPADQCP2                                                 
                                                                                
                SET QCNMR# = 1                                                  
                                                                                
                WHERE QCHSP# = P_HSP ;                                          
                                                                                
                ELSE                                                            
                                                                                
                UPDATE HPADQCP2                                                 
                                                                                
                SET QCNMR# = QCNMR# + 1                                         
                                                                                
                WHERE QCHSP# = P_HSP ;                                          
                END IF ;                                                        
                                                                                
                OPEN CURSOR1 ;                                                  
                                                                                
                END P1  ;                                                       
