                                                                                
/*************I********************************************************/        
/*            I                                                       */        
/* iSeries400 I  SLNXTACCTN  - STORED PROCEDURE FOR PX                */        
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
/* 07/06/2006 I                    I Modified STORED PROCEDURE        */        
/* 07/28/2006 I                    I MODIFIED STORED PROCEDURE        */        
/*************I********************I***********************************/        
                                                                                
SET PATH *LIBL ;                                                                
                                                                                
CREATE PROCEDURE SELECTNEXTACCOUNTNUMBER (                                      
                                                                                
        IN P_HSP INTEGER ,                                                      
                                                                                
        IN P_HSPCODE CHAR(3) )                                                  
                                                                                
        DYNAMIC RESULT SETS 1                                                   
                                                                                
        LANGUAGE SQL                                                            
                                                                                
        SPECIFIC SLNXTACCTN                                                     
                                                                                
        NOT DETERMINISTIC                                                       
                                                                                
        MODIFIES SQL DATA                                                       
                                                                                
        CALLED ON NULL INPUT                                                    
                                                                                
        SET OPTION DBGVIEW =*SOURCE                                             
                                                                                
        P1 : BEGIN                                                              
                                                                                
        DECLARE RETURNACCOUNTNUMBER INTEGER ;                                   
                                                                                
        DECLARE NEXTACCOUNTNUMBERSEED INTEGER ;                                 
                                                                                
        DECLARE NBRACTIVEACCTSWITHNUMBER INTEGER ;                              
                                                                                
        DECLARE NBRMPIACCTSWITHNUMBER INTEGER ;                                 
                                                                                
        DECLARE NBRPURGEDACCOUNTNUMBER INTEGER ;                                
                                                                                
        DECLARE MAXACCOUNTNUMBER INTEGER ;                                      
                                                                                
        DECLARE ACCTNUMRESTARTNUM INTEGER ;                                     
                                                                                
        DECLARE PURGEDATE INTEGER ;                                             
                                                                                
        DECLARE NONUMBERSAVAILABLE INTEGER ;                                    
                                                                                
        DECLARE CURSOR1 CURSOR FOR                                              
                                                                                
        SELECT NEXTACCOUNTNUMBERSEED AS NEXTACCOUNTNUMBER                       
                                                                                
        FROM SYSIBM/SYSDUMMY1 ;                                                 
                                                                                
        SET                                                                     
                                                                                
        RETURNACCOUNTNUMBER = 0 ;                                               
                                                                                
        SET NONUMBERSAVAILABLE = 0 ;                                            
                                                                                
        WHILE ( RETURNACCOUNTNUMBER = 0 AND NONUMBERSAVAILABLE < 2 ) DO         
                                                                                
        -- take a number                                                        
                                                                                
        SELECT QCNAC# , QCMXA# , QCSTA#                                         
                                                                                
        INTO NEXTACCOUNTNUMBERSEED,                                             
        MAXACCOUNTNUMBER ,                                                      
        ACCTNUMRESTARTNUM                                                       
        FROM HPADQCP2                                                           
        WHERE QCHSP# = P_HSP ;                                                  
                                                                                
        -- check to make sure it is valid;                                      
        -- is there a current account with this account number                  
                                                                                
        SELECT COUNT ( * ) INTO NBRACTIVEACCTSWITHNUMBER                        
        FROM HPADLPP                                                            
        WHERE LPHSP# = P_HSP                                                    
                AND ( LPACCT >= ( NEXTACCOUNTNUMBERSEED * 10 ) )                
                AND ( LPACCT <= ( ( NEXTACCOUNTNUMBERSEED * 10 ) + 9 ) ) ;      
                                                                                
                -- Is there an account in the MPI file                          
                                                                                
        SELECT COUNT ( * ) INTO NBRMPIACCTSWITHNUMBER                           
        FROM HXMRRWP                                                            
        WHERE RWHSP# = P_HSP                                                    
                AND ( RWACCT >= ( NEXTACCOUNTNUMBERSEED * 10 ) )                
                AND ( RWACCT <= ( ( NEXTACCOUNTNUMBERSEED * 10 ) + 9 ) ) ;      
                                                                                
                -- Is there a purged account that is less that 10 years         
                -- old with this account number.                                
                                                                                
        SELECT COUNT ( * ) INTO NBRPURGEDACCOUNTNUMBER                          
        FROM PMP002P                                                            
        WHERE PMHSPC = P_HSPCODE                                                
                AND ( PMPT#9 >= ( NEXTACCOUNTNUMBERSEED * 10 ) )                
                AND ( PMPT#9 <= ( ( NEXTACCOUNTNUMBERSEED * 10 ) + 9 ) )        
                AND (                                                           
                ( ( PMFPDT != 999999 AND PMFPDT != 0 )                          
                AND (                                                           
                DATE (                                                          
                RTRIM (                                                         
                ( CASE WHEN LENGTH ( TRIM ( PMAPDT ) ) = 5 THEN                 
                SUBSTR ( TRIM ( PMAPDT ) , 2 , 2 )                              
                                                                                
                ELSE SUBSTR ( TRIM ( PMAPDT ) , 3 , 2 ) END ) || '/' ||         
                ( CASE WHEN LENGTH ( TRIM ( PMAPDT ) ) = 5                      
                                                                                
                THEN SUBSTR ( TRIM ( PMAPDT ) , 4 , 2 )                         
                                                                                
                ELSE SUBSTR ( TRIM ( PMAPDT ) , 5 , 2 ) END ) || '/' ||         
                                                                                
                PMAPDC ||                                                       
                                                                                
                ( CASE WHEN LENGTH ( TRIM ( PMAPDT ) ) = 5 THEN                 
                                                                                
                '0' || SUBSTR ( TRIM ( PMAPDT ) , 1 , 1 )                       
                                                                                
                ELSE SUBSTR ( PMAPDT , 1 , 2 ) END )                            
                                                                                
                )                                                               
                                                                                
                ) + 10 YEARS < CURRENT DATE                                     
                                                                                
                )                                                               
                                                                                
                AND (                                                           
                                                                                
                ( PMAPDT != 999999 AND PMAPDT != 0 )                            
                                                                                
                AND (                                                           
                                                                                
                DATE (                                                          
                                                                                
                RTRIM (                                                         
                                                                                
                ( CASE WHEN LENGTH ( TRIM ( PMAPDT ) ) = 5 THEN                 
                                                                                
                SUBSTR ( TRIM ( PMAPDT ) , 2 , 2 )                              
                                                                                
                ELSE SUBSTR ( TRIM ( PMAPDT ) , 3 , 2 ) END ) || '/' ||         
                                                                                
                ( CASE WHEN LENGTH ( TRIM ( PMAPDT ) ) = 5                      
                                                                                
                THEN SUBSTR ( TRIM ( PMAPDT ) , 4 , 2 )                         
                                                                                
                ELSE SUBSTR ( TRIM ( PMAPDT ) , 5 , 2 ) END ) || '/' ||         
                                                                                
                PMAPDC ||                                                       
                                                                                
                ( CASE WHEN LENGTH ( TRIM ( PMAPDT ) ) = 5 THEN                 
                                                                                
                '0' || SUBSTR ( TRIM ( PMAPDT ) , 1 , 1 )                       
                                                                                
                ELSE SUBSTR ( PMAPDT , 1 , 2 ) END )                            
                                                                                
                )                                                               
                                                                                
                ) + 10 YEARS < CURRENT DATE                                     
                                                                                
                )                                                               
                                                                                
                )                                                               
                                                                                
                )                                                               
                                                                                
                )                                                               
                                                                                
                ;                                                               
                                                                                
                 -- if one already exists then this number is not               
                 -- 'available' so try again                                    
                                                                                
                IF ( NBRACTIVEACCTSWITHNUMBER <> 0                              
                                                                                
                OR NBRMPIACCTSWITHNUMBER <> 0 ) THEN                            
                                                                                
                IF ( NEXTACCOUNTNUMBERSEED >= MAXACCOUNTNUMBER ) THEN           
                                                                                
                UPDATE HPADQCP2                                                 
                                                                                
                SET QCNAC# = ACCTNUMRESTARTNUM                                  
                                                                                
                WHERE QCHSP# = P_HSP ;                                          
                                                                                
                SET NONUMBERSAVAILABLE = NONUMBERSAVAILABLE + 1 ;               
                                                                                
                ELSE                                                            
                                                                                
                UPDATE HPADQCP2                                                 
                                                                                
                SET QCNAC# = QCNAC# + 1                                         
                                                                                
                WHERE QCHSP# = P_HSP ;                                          
                                                                                
                END IF ;                                                        
                                                                                
                ELSE                                                            
                                                                                
                SET RETURNACCOUNTNUMBER = NEXTACCOUNTNUMBERSEED ;               
                                                                                
                END IF ;                                                        
                                                                                
                END WHILE ;                                                     
                                                                                
                -- if there are no more numbers available return 0              
                                                                                
                IF ( NONUMBERSAVAILABLE > 1 ) THEN                              
                                                                                
                SET NEXTACCOUNTNUMBERSEED = 0 ;                                 
                                                                                
                -- if this number was already used                              
                                                                                
                ELSEIF ( RETURNACCOUNTNUMBER >= MAXACCOUNTNUMBER ) THEN         
                                                                                
                UPDATE HPADQCP2                                                 
                                                                                
                -- reset the next number back to the start                      
                                                                                
                SET QCNAC# = ACCTNUMRESTARTNUM                                  
                                                                                
                WHERE QCHSP# = P_HSP ;                                          
                                                                                
                -- keep track of how many times we have been                    
                -- trough the entire list                                       
                                                                                
                -- it is possible that there are no avilable numbers.           
                                                                                
                SET NONUMBERSAVAILABLE = NONUMBERSAVAILABLE + 1 ;               
                                                                                
                ELSE                                                            
                                                                                
                -- otherwise just increment the next available number and       
                -- try again.                                                   
                                                                                
                UPDATE HPADQCP2                                                 
                                                                                
                SET QCNAC# = QCNAC# + 1                                         
                                                                                
                WHERE QCHSP# = P_HSP ;                                          
                                                                                
                END IF ;                                                        
                                                                                
                OPEN CURSOR1 ;                                                  
                                                                                
                END P1  ;                                                       
