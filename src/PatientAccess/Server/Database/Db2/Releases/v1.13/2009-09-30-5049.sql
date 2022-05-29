/*************I********************************************************/
/*            I                                                       */
/* iSeries400 I   Fix for Bug 5049 - Guarantor relationship should    */ 
/*            I   show "Emancipated Minor" in drop-down list          */
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

-- Add "Emancipated Minor" (51) as valid mapping for Guarantor (2)

DELETE FROM VALIDRELATIONSHIPCONTEXTS 
WHERE TYPEOFROLEID=2 AND RELATIONSHIPCODE=51;

INSERT INTO VALIDRELATIONSHIPCONTEXTS VALUES (2,51);
