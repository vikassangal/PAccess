--  Generate SQL 

--  Version:                   	V5R3M0 040528 

--  Generated on:              	12/07/06 17:26:41 

--  Relational Database:       	DVLA 

--  Standards Option:          	DB2 UDB iSeries 

CREATE INDEX WORKLISTITEMS_WORKLISTID_IDX 

	ON PACCESS/WORKLISTITEMS ( FACILITYID ASC , ACCOUNTNUMBER ASC , WORKLISTID ASC ) ;
