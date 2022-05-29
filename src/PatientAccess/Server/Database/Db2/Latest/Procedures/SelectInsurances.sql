                                                                                
/**********************************************************************/        
/* AS400 Short Name: SELEC00002                                       */        
/* iSeries400        SELECTINSURANCES -SQL VIEW FOR PATIENT ACCESS    */        
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
/*                                                                    */        
/***********I**********I************I***********************************/        
/*  Date    I Request #I  Pgmr      I  Modification Description        */        
/***********I**********I************I***********************************/        
/* 05/17/06 I 4043030  I  D Evans   I NEW VIEW                         */   
/* 10/23/07 I          I Gauthreaux I Add column INSURANCECOMPANYNAME  */
/* 02/07/08 I		   I Sophie		I method calling the proc is obsolete */			
/***********I**********I************I***********************************/        
                                                                                
SET PATH *LIBL ;                                                                
                                                                                
CREATE PROCEDURE SELECTINSURANCES (                                             
        IN P_HSP INTEGER ,                                                      
        IN P_ACCT INTEGER )                                                     
        DYNAMIC RESULT SETS 1                                                   
        LANGUAGE SQL                                                            
        SPECIFIC SELINS                                                         
        NOT DETERMINISTIC                                                       
        READS SQL DATA                                                          
        CALLED ON NULL INPUT                                                    
        SET OPTION DBGVIEW = *SOURCE                                            
        P1 : BEGIN                                                              
                                                                                
        DECLARE CURSOR1 CURSOR FOR                                              
        SELECT HSPNUMBER ,                                                      
        ACCOUNTNUMBER ,                                                         
        INSUREDLASTNAME ,                                                       
        INSUREDFIRSTNAME ,                                                      
        INSUREDMIDDLEINITIAL ,                                                  
        INSUREDSEX ,                                                            
        INSUREDRELATIONSHIP ,                                                   
        INSUREDIDENTIFIER ,                                                     
        INSUREDCOUNTRYCODE ,                                                    
        INSUREDBIRTHDATE ,                                                      
        INSURANCECERTIFICATIONNUM , 
		INSURANCEGROUPNUMBER ,
        GROUPNUMBER ,                                                           
        POLICYNUMBER ,                                                          
        EVCNUMBER ,                                                             
        AUTHORIZATIONNUMBER ,                                                   
        AUTHORIZEDDAYS ,                                                        
        ISSUEDATE ,                                                             
        INSUREDADDRESS ,                                                        
        INSUREDCITY ,                                                           
        INSUREDSTATE ,                                                          
        INSUREDZIP ,                                                            
        INSUREDZIPEXT ,                                                         
        INSUREDAREACODE1 ,                                                      
        INSUREDPHONENUMBER1 ,                                                   
        INSUREDAREACODE2 ,                                                      
        INSUREDPHONENUMBER2 ,                                                   
        INSUREDCELLPHONE ,                                                      
        INSURANCECOMPANYNUMBER ,                                                
        SIGNEDOVERMEDICAREHICNUMBER ,
        PRIORITYCODE ,                                                          
        BILLINGADDRESS1 ,                                                       
        BILLINGCITYSTATECOUNTRY ,                                               
        BILLINGZIPZIPEXTPHONE ,                                                 
        DEDUCTIBLE ,                                                            
        COPAY ,                                                                 
        NOLIABILITY ,                                                           
        ELIGIBILITY ,                                                           
        CONDITIONOFSERVICE ,                                                    
        PRIMARYEMPSTATUS ,                                                      
        PRIMARYEMPCODE ,                                                        
        PRIMARYEMPNAME ,                                                        
        PRIMARYEMPADDRESS ,                                                     
        PRIMARYLOC ,  --city, state                                             
        PRIMARYEMPZIP ,                                                         
        PRIMARYEMPZIPEXT ,                                                      
        PRIMARYPHONE ,                                                          
        SECONDARYEMPSTATUS ,                                                    
        SECONDARYEMPCODE ,                                                      
        SECONDARYEMPNAME ,                                                      
        SECONDARYEMPADDRESS ,                                                   
        SECONDARYLOC ,  --city, state                                           
        SECONDARYEMPZIP ,                                                       
        SECONDARYEMPZIPEXT ,                                                    
        SECONDARYPHONE ,                                                        
        EFFECTIVEON ,                                                           
        APPROVEDON ,                                                            
        AUTHORIZATIONPHONE ,                                                    
        AUTHORIZATIONCOMPANY ,                                                  
        AUTHORIZATIONPMTEXT ,                                                   
        AUTHORIZATIONFLAG ,                                                     
        VERIFICATIONDATE ,                                                      
        VERIFICATIONFLAG ,                                                      
        VERIFIEDBY ,                                                            
        ATTORNEYNAME ,                                                          
        ATTORNEYSTREET ,                                                        
        ATTORNEYCITY ,                                                          
        ATTORNEYSTATE ,                                                         
        ATTORNEYZIP5 ,                                                          
        ATTORNEYZIP4 ,                                                          
        ATTORNEYCOUNTRYCODE ,                                                   
        ATTORNEYPHONE ,                                                         
        AGENTNAME ,                                                             
        AGENTPHONE ,                                                            
        AGENTSTREET ,                                                           
        AGENTCITY ,                                                             
        AGENTSTATE ,                                                            
        AGENTZIP5 ,                                                             
        AGENTZIP4 ,                                                             
        AGENTCOUNTRYCODE ,                                                      
        BILLINGCAREOFNAME ,                                                     
        BILLINGNAME ,                                                           
        TRACKINGNUMBER ,                                                        
        ADJUSTERSNAME ,                                                         
        EMPLOYEESSUPERVISOR  ,
        INSURANCECOMPANYNAME                                                   
        FROM INSURANCES I                                                       
        WHERE I . HSPNUMBER = P_HSP AND                                         
        I . ACCOUNTNUMBER = P_ACCT AND ( PRIORITYCODE = '1'                     
        OR PRIORITYCODE = '2' )                                                 
        ORDER BY PRIORITYCODE ;                                                 
        OPEN CURSOR1 ;                                                          
        END P1  ;                                                               
