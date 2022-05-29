CREATE TABLE PACCESS.WORKLISTITEMS ( 
	WORKLISTITEMID FOR COLUMN WORKL00001 INTEGER NOT NULL , 
	WORKLISTID INTEGER DEFAULT NULL , 
	FACILITYID INTEGER DEFAULT NULL , 
	ACCOUNTNUMBER FOR COLUMN ACCOU00001 INTEGER DEFAULT NULL ) ; 

CREATE TABLE PACCESS.WORKLISTRANGES ( 
	WORKLISTID INTEGER NOT NULL , 
	WORKLISTSELECTIONRANGEID FOR COLUMN WORKL00001 INTEGER NOT NULL ) ; 

CREATE TABLE PACCESS.WORKLISTRULES ( 
	WORKLISTID INTEGER NOT NULL , 
	RULEID INTEGER NOT NULL ) ; 

CREATE TABLE PACCESS.WORKLISTS ( 
	WORKLISTID INTEGER NOT NULL , 
	WORKLISTNAME FOR COLUMN WORKL00001 VARCHAR(150) CCSID 37 DEFAULT NULL ) ; 

CREATE TABLE PACCESS.WORKLISTSELECTIONRANGES ( 
	WORKLISTSELECTIONRANGEID FOR COLUMN WORKL00001 INTEGER NOT NULL , 
	DESCRIPTION FOR COLUMN DESCR00001 VARCHAR(50) CCSID 37 DEFAULT NULL , 
	RANGEINDAYS FOR COLUMN RANGE00001 INTEGER DEFAULT NULL , 
	DISPLAYORDER FOR COLUMN DISPL00001 INTEGER DEFAULT NULL ) ; 
  
CREATE TABLE PACCESS.WORKLISTSETTINGS ( 
	STARTLETTERS FOR COLUMN START00001 VARCHAR(15) CCSID 37 DEFAULT NULL , 
	ENDLETTERS VARCHAR(15) CCSID 37 DEFAULT NULL , 
	WORKLISTSELECTIONRANGEID FOR COLUMN WORKL00001 INTEGER DEFAULT NULL , 
	WORKLISTID INTEGER NOT NULL , 
	USERID INTEGER NOT NULL , 
	STARTDATE DATE DEFAULT NULL , 
	ENDDATE DATE DEFAULT NULL , 
	SORTEDCOLUMN FOR COLUMN SORTE00001 INTEGER DEFAULT NULL , 
	SORTEDCOLUMNDIRECTION FOR COLUMN SORTE00002 INTEGER DEFAULT 1 ) ;

CREATE INDEX PACCESS.REMAININGACTIONS_WORKLISTID_IDX ON PACCESS.REMAININGACTIONS (WORKLISTITEMID ASC );
CREATE INDEX PACCESS.WORKLISTITEMS_WORKLISTID_IDX ON PACCESS.WORKLISTITEMS (FACILITYID, ACCOUNTNUMBER, WORKLISTID );

insert into worklists values(1,'PreRegistration');
insert into worklists values(2,'PostRegistration');
insert into worklists values(3,'Insurance Verification');
insert into worklists values(4,'Patient Liability');
insert into worklists values(5,'Emergency Deparment Registration');
insert into worklists values(6,'No Show');

insert into worklistselectionranges values (1,'Today',0,5);
insert into worklistselectionranges values (2,'Tomorrow',1,6);
insert into worklistselectionranges values (3,'Next 3 days',3,7);
insert into worklistselectionranges values (4,'Next 10 days',10,8);
insert into worklistselectionranges values (5,'Last 3 days',-3,3);
insert into worklistselectionranges values (6,'Last 10 Days',-10,2);
insert into worklistselectionranges values (7,'Yesterday',-1,4);
insert into worklistselectionranges values (8,'All',null,1);
insert into worklistselectionranges values (9,'Date Range',null,9);

insert into worklistranges values(1,1);
insert into worklistranges values(1,2);
insert into worklistranges values(1,3);
insert into worklistranges values(1,4);
insert into worklistranges values(1,8);
insert into worklistranges values(1,9);

insert into worklistranges values(2,1);
insert into worklistranges values(2,7);
insert into worklistranges values(2,5);
insert into worklistranges values(2,6);
insert into worklistranges values(2,9);
insert into worklistranges values(2,8);

insert into worklistRanges values(3,8);
insert into worklistRanges values(3,6);
insert into worklistRanges values(3,5);
insert into worklistRanges values(3,7);
insert into worklistRanges values(3,1);
insert into worklistRanges values(3,2);
insert into worklistRanges values(3,3);
insert into worklistRanges values(3,4);
insert into worklistRanges values(3,9);

insert into worklistRanges values(4,8);
insert into worklistRanges values(4,6);
insert into worklistRanges values(4,5);
insert into worklistRanges values(4,7);
insert into worklistRanges values(4,1);
insert into worklistRanges values(4,2);
insert into worklistRanges values(4,3);
insert into worklistRanges values(4,4);
insert into worklistRanges values(4,9);

insert into worklistRanges values(5,8);
insert into worklistRanges values(5,1);
insert into worklistRanges values(5,2);
insert into worklistRanges values(5,3);
insert into worklistRanges values(5,4);
insert into worklistRanges values(5,9);

insert into worklistRanges values(6,8);
insert into worklistRanges values(6,1);
insert into worklistRanges values(6,2);
insert into worklistRanges values(6,3);
insert into worklistRanges values(6,4);
insert into worklistRanges values(6,9);

-- PreReg
Insert into WorklistRules values(1,33);
Insert into WorklistRules values(1,34);
Insert into WorklistRules values(1,35);
Insert into WorklistRules values(1,36);
Insert into WorklistRules values(1,37);
Insert into WorklistRules values(1,38);
Insert into WorklistRules values(1,39);
Insert into WorklistRules values(1,40);
Insert into WorklistRules values(1,41);
Insert into WorklistRules values(1,42);
Insert into WorklistRules values(1,43);
Insert into WorklistRules values(1,44);
Insert into WorklistRules values(1,45);
Insert into WorklistRules values(1,46);
Insert into WorklistRules values(1,47);
Insert into WorklistRules values(1,48);

-- PostReg
Insert into WorklistRules values(2,17);
Insert into WorklistRules values(2,18);
Insert into WorklistRules values(2,19);
Insert into WorklistRules values(2,20);
Insert into WorklistRules values(2,21);
Insert into WorklistRules values(2,22);
Insert into WorklistRules values(2,1);
Insert into WorklistRules values(2,2);
Insert into WorklistRules values(2,3);
Insert into WorklistRules values(2,4);
Insert into WorklistRules values(2,5);
Insert into WorklistRules values(2,6);
Insert into WorklistRules values(2,7);
Insert into WorklistRules values(2,8);
Insert into WorklistRules values(2,9);
Insert into WorklistRules values(2,10);
Insert into WorklistRules values(2,11);
Insert into WorklistRules values(2,12);
Insert into WorklistRules values(2,13);
Insert into WorklistRules values(2,14);
Insert into WorklistRules values(2,15);
Insert into WorklistRules values(2,16);

Insert into WorklistRules values(3,25);
Insert into WorklistRules values(3,26);
Insert into WorklistRules values(3,27);
Insert into WorklistRules values(3,28);

Insert into WorklistRules values(4,29);
Insert into WorklistRules values(4,30);
Insert into WorklistRules values(4,31);
Insert into WorklistRules values(4,32);
Insert into WorklistRules values(4,326);

Insert into WorklistRules values(5,23);

Insert into WorklistRules values(6,24);

