CALL DROP_TABLE ('PACCESS', 'WORKLISTITEMS');

CALL DROP_TABLE ('PACCESS', 'VISITS');

CREATE TABLE PACCESS.WORKLISTITEMS (
WorklistItemID    INTEGER NOT NULL,
WorklistID          INTEGER DEFAULT NULL,
FacilityID            INTEGER DEFAULT NULL,
AccountNumber INTEGER DEFAULT NULL);

CREATE TABLE PACCESS.VISITS (
FacilityID           INTEGER NOT NULL,
AccountNumber INTEGER NOT NULL);

COMMIT;

insert into worklistitems values (5,4,900,30049);
insert into worklistitems values (6,3,900,30072);
insert into worklistitems values (7,1,900,31179);
insert into worklistitems values (103,6,900,33381);
insert into worklistitems values (104,6,900,34975);
insert into worklistitems values (3,6,900,33332);
insert into worklistitems values (160,6,900,37127);
insert into worklistitems values (161,6,900,35683);
insert into worklistitems values (1,1,900,30015);
insert into worklistitems values (4,5,900,34991);
insert into worklistitems values (2,2,900,32110);
insert into worklistitems values (140,6,6,5124620);

COMMIT;

insert into visits values (6,4975952);
insert into visits values (6,5122457);
insert into visits values (6,5277507);
insert into visits values (6,5205557);
insert into visits values (6,3190471);
insert into visits values (6,4479629);
insert into visits values (6,3337804);
insert into visits values (6,4266420);
insert into visits values (6,4701836);
insert into visits values (6,5345278);
insert into visits values (6,4384725);
insert into visits values (6,3515311);
insert into visits values (6,5320836);
insert into visits values (6,3676684);
insert into visits values (6,3051382);
insert into visits values (6,334455 );
insert into visits values (99,88935 );
insert into visits values (99,88943 );
insert into visits values (99,88188 );
insert into visits values (99,88595 );
insert into visits values (99,88846 );
insert into visits values (98,33381 );
insert into visits values (900,33357);
insert into visits values (900,33365);
insert into visits values (900,33373);
insert into visits values (900,33381);
insert into visits values (900,40899);
insert into visits values (900,35352);
insert into visits values (900,35337);
insert into visits values (98,35345 );
insert into visits values (900,35329);
insert into visits values (900,35345);
insert into visits values (900,31963);
insert into visits values (900,32086);
insert into visits values (900,32169);
insert into visits values (900,32227);
insert into visits values (900,32292);
insert into visits values (900,32300);
insert into visits values (900,32318);
insert into visits values (900,32359);
insert into visits values (900,32391);
insert into visits values (900,32441);
insert into visits values (900,32375);
insert into visits values (900,32243);
insert into visits values (900,32367);
insert into visits values (900,32276);
insert into visits values (900,32383);
insert into visits values (900,32268);
insert into visits values (900,32326);
insert into visits values (900,32334);
insert into visits values (900,32409);
insert into visits values (900,32417);
insert into visits values (900,32425);
insert into visits values (900,32433);
insert into visits values (900,32458);
insert into visits values (900,37143);
insert into visits values (900,37697);
insert into visits values (900,37713);
insert into visits values (900,37721);
insert into visits values (900,37739);
insert into visits values (900,37747);
insert into visits values (900,31773);
insert into visits values (900,31930);
insert into visits values (900,32151);
insert into visits values (900,32342);
insert into visits values (6,5287502);
insert into visits values (6,5210305);
insert into visits values (6,3901254);
insert into visits values (6,5163412);
insert into visits values (6,4854250);
insert into visits values (6,3050190);
insert into visits values (6,4787986);
insert into visits values (6,3434613);
insert into visits values (6,5137608);
insert into visits values (6,7195746);
insert into visits values (98,35352 );
insert into visits values (6,3966151);
insert into visits values (6,3723062);
insert into visits values (6,3860140);
insert into visits values (6,3938220);
insert into visits values (6,3938964);
insert into visits values (6,4074602);
insert into visits values (6,4096606);
insert into visits values (6,4136837);
insert into visits values (6,4336534);
insert into visits values (6,4349997);
insert into visits values (6,4374142);
insert into visits values (6,4420608);
insert into visits values (6,4475569);
insert into visits values (6,4477677);
insert into visits values (6,4482158);
insert into visits values (6,4487753);
insert into visits values (6,4507010);
insert into visits values (6,4531540);
insert into visits values (6,4541812);
insert into visits values (6,4602218);
insert into visits values (6,4611560);
insert into visits values (6,4624637);
insert into visits values (6,4644352);
insert into visits values (6,4658329);
insert into visits values (6,4664841);
insert into visits values (6,4670671);
insert into visits values (6,4745698);
insert into visits values (6,4762444);
insert into visits values (6,4763211);
insert into visits values (6,4803957);
insert into visits values (6,4809750);
insert into visits values (6,4845021);
insert into visits values (900,33266);
insert into visits values (900,33274);
insert into visits values (900,33282);
insert into visits values (900,33290);
insert into visits values (900,33332);
insert into visits values (6,4897498);
insert into visits values (6,4900693);
insert into visits values (6,4920406);
insert into visits values (6,4928385);
insert into visits values (6,4953711);
insert into visits values (6,4959426);
insert into visits values (6,4975618);
insert into visits values (6,4980131);
insert into visits values (6,4984234);
insert into visits values (6,4989848);
insert into visits values (6,5010551);
insert into visits values (6,5021804);
insert into visits values (6,5046394);
insert into visits values (6,5069211);
insert into visits values (6,5074126);
insert into visits values (6,5093392);
insert into visits values (6,5111994);
insert into visits values (6,5118093);
insert into visits values (6,5120918);
insert into visits values (6,5124620);
insert into visits values (6,5129966);
insert into visits values (6,5136105);
insert into visits values (6,5144086);
insert into visits values (6,5144302);
insert into visits values (6,5146119);
insert into visits values (6,5164559);
insert into visits values (6,5165520);
insert into visits values (6,5183545);
insert into visits values (6,5196914);
insert into visits values (6,5198070);
insert into visits values (6,5198992);
insert into visits values (6,5199050);
insert into visits values (6,5207770);
insert into visits values (900,30049);
insert into visits values (900,30114);
insert into visits values (900,30346);
insert into visits values (900,30387);
insert into visits values (900,30494);
insert into visits values (900,30536);
insert into visits values (900,30577);
insert into visits values (900,30627);
insert into visits values (900,30882);
insert into visits values (900,30924);
insert into visits values (900,30973);
insert into visits values (900,31021);
insert into visits values (900,31088);
insert into visits values (900,31138);
insert into visits values (900,31179);
insert into visits values (900,31229);
insert into visits values (900,31278);
insert into visits values (900,31310);
insert into visits values (900,31351);
insert into visits values (900,31401);
insert into visits values (900,31443);
insert into visits values (900,31484);
insert into visits values (900,31534);
insert into visits values (900,31575);
insert into visits values (900,31625);
insert into visits values (900,31666);
insert into visits values (900,30866);
insert into visits values (900,30874);
insert into visits values (900,30890);
insert into visits values (900,30916);
insert into visits values (900,30932);
insert into visits values (900,30940);
insert into visits values (900,30965);
insert into visits values (900,30999);
insert into visits values (900,31005);
insert into visits values (900,37705);
insert into visits values (900,36988);
insert into visits values (900,37754);
insert into visits values (900,36863);
insert into visits values (900,37127);
insert into visits values (900,36970);
insert into visits values (900,36855);
insert into visits values (900,37119);
insert into visits values (900,30015);
insert into visits values (900,30056);
insert into visits values (900,30072);
insert into visits values (900,30106);
insert into visits values (900,30171);
insert into visits values (900,30247);
insert into visits values (900,30379);
insert into visits values (900,30395);
insert into visits values (900,30429);
insert into visits values (900,30486);
insert into visits values (900,30502);
insert into visits values (900,30510);
insert into visits values (900,30544);
insert into visits values (900,30551);
insert into visits values (900,30569);
insert into visits values (900,30593);
insert into visits values (900,30601);
insert into visits values (900,30619);
insert into visits values (900,31344);
insert into visits values (900,31369);
insert into visits values (900,31377);
insert into visits values (900,31393);
insert into visits values (900,31419);
insert into visits values (900,31427);
insert into visits values (900,31450);
insert into visits values (900,31468);
insert into visits values (900,31476);
insert into visits values (900,33308);
insert into visits values (98,31096 );
insert into visits values (900,34405);
insert into visits values (900,35683);
insert into visits values (900,35691);
insert into visits values (98,34405 );
insert into visits values (900,33613);
insert into visits values (98,30452 );
insert into visits values (900,31039);
insert into visits values (900,31047);
insert into visits values (900,31070);
insert into visits values (900,33225);
insert into visits values (900,33324);
insert into visits values (900,31104);
insert into visits values (900,31112);
insert into visits values (900,31120);
insert into visits values (900,31146);
insert into visits values (900,31161);
insert into visits values (900,31187);
insert into visits values (900,31195);
insert into visits values (900,31245);
insert into visits values (900,31252);
insert into visits values (900,31260);
insert into visits values (900,31294);
insert into visits values (900,31302);
insert into visits values (900,31328);
insert into visits values (900,31708);
insert into visits values (900,31757);
insert into visits values (900,31807);
insert into visits values (900,31948);
insert into visits values (900,31989);
insert into visits values (900,32219);
insert into visits values (900,30353);
insert into visits values (900,31435);
insert into visits values (900,31880);
insert into visits values (900,31500);
insert into visits values (900,31518);
insert into visits values (900,31526);
insert into visits values (900,31559);
insert into visits values (900,31567);
insert into visits values (900,31583);
insert into visits values (900,31609);
insert into visits values (900,31617);
insert into visits values (900,31633);
insert into visits values (900,31658);
insert into visits values (900,31674);
insert into visits values (900,31682);
insert into visits values (900,31716);
insert into visits values (900,31724);
insert into visits values (900,31732);
insert into visits values (900,31765);
insert into visits values (900,31781);
insert into visits values (900,31799);
insert into visits values (900,31823);
insert into visits values (900,31872);
insert into visits values (900,31898);
insert into visits values (900,31914);
insert into visits values (900,31922);
insert into visits values (900,31955);
insert into visits values (900,31971);
insert into visits values (900,32078);
insert into visits values (900,32094);
insert into visits values (900,32102);
insert into visits values (900,32110);
insert into visits values (900,32136);
insert into visits values (900,32144);
insert into visits values (900,32177);
insert into visits values (900,32185);
insert into visits values (900,32193);
insert into visits values (900,32201);
insert into visits values (900,32250);
insert into visits values (900,30031);
insert into visits values (900,30163);
insert into visits values (900,30361);
insert into visits values (900,30452);
insert into visits values (900,30528);
insert into visits values (900,30635);
insert into visits values (900,30767);
insert into visits values (900,30759);
insert into visits values (900,30585);
insert into visits values (900,30783);
insert into visits values (900,30775);
insert into visits values (900,30809);
insert into visits values (900,30791);
insert into visits values (900,30841);
insert into visits values (900,30833);
insert into visits values (900,30957);
insert into visits values (900,30817);
insert into visits values (900,30825);
insert into visits values (900,30858);
insert into visits values (900,30908);
insert into visits values (900,31013);
insert into visits values (900,31054);
insert into visits values (900,31062);
insert into visits values (900,31153);
insert into visits values (900,31096);
insert into visits values (900,31492);
insert into visits values (900,31542);
insert into visits values (900,31286);
insert into visits values (900,31336);
insert into visits values (900,31385);
insert into visits values (900,31237);
insert into visits values (900,31690);
insert into visits values (900,31740);
insert into visits values (900,31591);
insert into visits values (900,31641);
insert into visits values (900,31906);
insert into visits values (900,31815);
insert into visits values (6,5188652);
insert into visits values (6,5246288);
insert into visits values (6,4655044);
insert into visits values (6,5008808);
insert into visits values (6,4602323);
insert into visits values (6,4527615);
insert into visits values (6,5094275);
insert into visits values (6,4929357);
insert into visits values (6,5063779);
insert into visits values (6,5208009);
insert into visits values (6,5208513);
insert into visits values (6,5237254);
insert into visits values (6,5241952);
insert into visits values (6,5245060);
insert into visits values (6,5248698);
insert into visits values (6,5265860);
insert into visits values (6,5267889);
insert into visits values (6,5280338);
insert into visits values (6,5280354);
insert into visits values (6,5283922);
insert into visits values (6,5295777);
insert into visits values (6,5313490);
insert into visits values (6,5315654);
insert into visits values (6,5326575);
insert into visits values (6,5336040);
insert into visits values (6,5337739);
insert into visits values (6,5338344);
insert into visits values (6,5345740);
insert into visits values (6,5350115);
insert into visits values (6,5351227);
insert into visits values (900,41913);
insert into visits values (900,34868);
insert into visits values (900,34975);
insert into visits values (900,42671);
insert into visits values (900,42663);
insert into visits values (900,38950);
insert into visits values (900,38943);
insert into visits values (900,38976);
insert into visits values (900,34983);
insert into visits values (900,34991);
insert into visits values (900,38935);
insert into visits values (900,38968);
insert into visits values (900,38190);
insert into visits values (900,30007);

commit;

CALL DROP_TABLE ('PACCESS','WORKLISTSETTINGS');

CREATE TABLE PACCESS.WORKLISTSETTINGS ( 
  STARTLETTERS                       VARCHAR (15), 
  ENDLETTERS                           VARCHAR (15), 
  WORKLISTSELECTIONRANGEID  INT, 
  WORKLISTID                           INT   NOT NULL, 
  USERID                                   INT   NOT NULL, 
  STARTDATE                            DATE DEFAULT NULL,
  ENDDATE                                DATE DEFAULT NULL,
  SORTEDCOLUMN                      INT,
  SORTEDCOLUMNDIRECTION      INT DEFAULT 1);

COMMIT;

insert into worklistsettings values ('A','Z',8,3,5,'6/7/2005','6/7/2005',1,1);
insert into worklistsettings values ('A','Z',9,1,5,'6/29/2005','6/24/2005',1,1);
insert into worklistsettings values ('A','W',6,2,5,'5/28/2005','6/7/2005',8,1);
insert into worklistsettings values ('A','Z',8,4,5,'6/7/2005','6/7/2005',1,1);
insert into worklistsettings values ('A','Z',8,6,5,'6/7/2005','6/7/2005',1,1);
insert into worklistsettings values ('A','Z',8,5,5,'6/7/2005','6/7/2005',1,1);
insert into worklistsettings values ('A','Z',0,3,1,null,null,1,1);
insert into worklistsettings values ('F','M',1,9,1,'5/10/2005 2:30:12 PM','5/10/2005 2:30:12 PM',1,1);
insert into worklistsettings values ('C','G',1,5,3,'5/12/2005 12:37:07 PM','5/12/2005 12:37:07 PM',1,1);
insert into worklistsettings values ('C','G',1,4,3,'5/12/2005 12:37:07 PM','5/12/2005 12:37:07 PM',1,1);
insert into worklistsettings values ('A','Z',4,0,1,null,null,1,1);
insert into worklistsettings values ('A','Z',0,6,1,null,null,1,1);

insert into worklistsettings values ('F','M',9,1,1, '6/2/2005', '6/2/2005',1,1);
insert into worklistsettings values ('C','G',9,5,1, '6/2/2005', '6/2/2005',1,1);
insert into worklistsettings values ('C','G',1,4,1, '6/2/2005', '6/2/2005',1,1);
--insert into worklistsettings values ('F','M',9,1,1, '6/2/2005 12:12:43 PM', '6/2/2005 12:12:43 PM',1,1);
--insert into worklistsettings values ('C','G',9,5,1, '6/2/2005 12:12:43 PM', '6/2/2005 12:12:43 PM',1,1);
--insert into worklistsettings values ('C','G',1,4,1, '6/2/2005 12:12:43 PM', '6/2/2005 12:12:43 PM',1,1);

insert into worklistsettings values ('A','B',1,1,3,null,null,1,1);
insert into worklistsettings values ('A','B',9,2,1,null,null,1,1);

COMMIT;


CALL DROP_TABLE ('PACCESS', 'WORKLISTRANGES');

CREATE TABLE PACCESS.WORKLISTRANGES ( 
  WORKLISTID               INT NOT NULL, 
  WORKLISTSELECTIONRANGEID  INT NOT NULL);

-- WORKLISTRANGES
insert into worklistranges values (1,2);
insert into worklistranges values (1,3);
insert into worklistranges values (1,4);
insert into worklistranges values (1,8);
insert into worklistranges values (1,9);
insert into worklistranges values (2,7);
insert into worklistranges values (2,5);
insert into worklistranges values (2,6);
insert into worklistranges values (2,9);
insert into worklistranges values (2,8);
insert into worklistranges values (1,1);
insert into worklistranges values (3,8);
insert into worklistranges values (3,6);
insert into worklistranges values (3,5);
insert into worklistranges values (3,7);
insert into worklistranges values (3,1);
insert into worklistranges values (3,2);
insert into worklistranges values (3,3);
insert into worklistranges values (3,4);
insert into worklistranges values (3,9);
insert into worklistranges values (4,8);
insert into worklistranges values (4,6);
insert into worklistranges values (4,5);
insert into worklistranges values (4,7);
insert into worklistranges values (4,1);
insert into worklistranges values (4,2);
insert into worklistranges values (4,3);
insert into worklistranges values (4,4);
insert into worklistranges values (4,9);
insert into worklistranges values (5,8);
insert into worklistranges values (5,1);
insert into worklistranges values (5,2);
insert into worklistranges values (5,3);
insert into worklistranges values (5,4);
insert into worklistranges values (5,9);
insert into worklistranges values (6,8);
insert into worklistranges values (6,1);
insert into worklistranges values (6,2);
insert into worklistranges values (6,3);
insert into worklistranges values (6,4);
insert into worklistranges values (6,9);
insert into worklistranges values (2,1);

COMMIT;

CALL DROP_TABLE('PACCESS','WORKLISTSELECTIONRANGES');

CREATE TABLE PACCESS.WORKLISTSELECTIONRANGES ( 
  WORKLISTSELECTIONRANGEID  INT      NOT NULL, 
  DESCRIPTION                VARCHAR (50) DEFAULT NULL, 
  RANGEINDAYS               INT                DEFAULT NULL, 
  DISPLAYORDER              INT                DEFAULT NULL);

insert into worklistselectionranges values (2,'Tomorrow',1,6);
insert into worklistselectionranges values (3,'Next 3 days',3,7);
insert into worklistselectionranges values (4,'Next 10 days',10,8); 
insert into worklistselectionranges values (5,'Last 3 days',-3,3);
insert into worklistselectionranges values (6,'Last 10 Days',-10,2);
insert into worklistselectionranges values (7,'Yesterday',-1,4);
insert into worklistselectionranges values (8,'All',null,1);
insert into worklistselectionranges values (9,'Date Range',null,9);
insert into worklistselectionranges values (1,'Today',0,5);

COMMIT;

CALL DROP_TABLE('PACCESS', 'WORKLISTS');

CREATE TABLE PACCESS.WORKLISTS ( 
  WORKLISTID       INT  NOT NULL, 
  WORKLISTNAME  VARCHAR (150) DEFAULT NULL);

insert into worklists values (2,'PostRegistration');
insert into worklists values (3,'Insurance Verification');
insert into worklists values (1,'PreRegistration');
insert into worklists values (4,'Patient Liability');
insert into worklists values (5,'Emergency Deparment Registration');
insert into worklists values (6,'No Show');

COMMIT;


CALL DROP_TABLE('PACCESS', 'REMAININGACTIONS');

CREATE TABLE PACCESS.REMAININGACTIONS ( 
  ACTIONID             INT, 
  WORKLISTITEMID  INT);

INSERT INTO REMAININGACTIONS VALUES (24,103);
INSERT INTO REMAININGACTIONS VALUES (24,104);
INSERT INTO REMAININGACTIONS VALUES (18,2  );
INSERT INTO REMAININGACTIONS VALUES (24,160);
INSERT INTO REMAININGACTIONS VALUES (24,161);
INSERT INTO REMAININGACTIONS VALUES (2,1   );
INSERT INTO REMAININGACTIONS VALUES (4,1   );
INSERT INTO REMAININGACTIONS VALUES (13,1  );
INSERT INTO REMAININGACTIONS VALUES (23,4  );
INSERT INTO REMAININGACTIONS VALUES (1,7   );
INSERT INTO REMAININGACTIONS VALUES (3,7   );
INSERT INTO REMAININGACTIONS VALUES (7,7   );
INSERT INTO REMAININGACTIONS VALUES (8,7   );
INSERT INTO REMAININGACTIONS VALUES (10,7  );
INSERT INTO REMAININGACTIONS VALUES (16,7  );
INSERT INTO REMAININGACTIONS VALUES (13,7  );
INSERT INTO REMAININGACTIONS VALUES (18,7  );
INSERT INTO REMAININGACTIONS VALUES (24,3  );
INSERT INTO REMAININGACTIONS VALUES (30,5  );
INSERT INTO REMAININGACTIONS VALUES (28,6  );
INSERT INTO REMAININGACTIONS VALUES (24,140);

COMMIT;

CALL DROP_TABLE ('PACCESS','ACTIONS');

CREATE TABLE ACTIONS ( 
  ACTIONID                   INT NOT NULL, 
  TYPE                         VARCHAR (1500), 
  RULEID                       INT, 
  COMPOSITEACTIONID  INT, 
  ACTIONNAME              VARCHAR (128));

insert into actions values (33,'PatientAccess.Actions.ProvidePreferredDriversLicense',33,45,'Provide Drivers License');
insert into actions values (34,'PatientAccess.Actions.ProvidePreferredEmployer',34,45,'Provide Employer');
insert into actions values (35,'PatientAccess.Actions.ProvidePreferredEmployerAddress',35,45,'Provide Employer Address');
insert into actions values (36,'PatientAccess.Actions.ProvidePreferredEmployerPhoneAreaCode',36,45,'Provide Employer Phone Area Code');
insert into actions values (37,'PatientAccess.Actions.ProvidePreferredEmployerPhoneSubscriber',37,45,'Provide Employer Phone Subscriber');
insert into actions values (38,'PatientAccess.Actions.ProvidePreferredEmploymentStatus',38,45,'Provide Employment Status');
insert into actions values (39,'PatientAccess.Actions.ProvidePreferredEthnicity',39,45,'Provide Ethnicity');
insert into actions values (40,'PatientAccess.Actions.ProvidePreferredLanguage',40,45,'Provide Language');
insert into actions values (41,'PatientAccess.Actions.ProvidePreferredMailingAddress',41,45,'Provide Mailing Address');
insert into actions values (42,'PatientAccess.Actions.ProvidePreferredMailingAddressAreaCode',42,45,'Provide Mailing Address Area Code');
insert into actions values (43,'PatientAccess.Actions.ProvidePreferredMailingAddressPhone',43,45,'Provide Mailing Address Phone');
insert into actions values (44,'PatientAccess.Actions.ProvidePreferredMaritalStatus',44,45,'Provide Marital Status');
insert into actions values (45,'PatientAccess.Actions.ProvideMissingDemographics',45,NULL,'Provide Missing Demographics');
insert into actions values (46,'PatientAccess.Actions.ProvidePreferredOccIndustry',46,45,'Provide Occurance Industry');
insert into actions values (47,'PatientAccess.Actions.ProvidePreferredPatientRetiredDate',47,45,'Provide Patient Retired Date');
insert into actions values (48,'PatientAccess.Actions.ProvidePreferredRace',48,45,'Provide Race');
insert into actions values (1,'PatientAccess.Actions.ProvideDriversLicense',1,13,'Provide Drivers License');
insert into actions values (2,'PatientAccess.Actions.ProvideEmployer',2,13,'Provide Employer');
insert into actions values (3,'PatientAccess.Actions.ProvideEmployerAddress',3,13,'Provide Employer Address');
insert into actions values (4,'PatientAccess.Actions.ProvideEmployerPhoneAreaCode',4,13,'Provide Employer Phone Area Code');
insert into actions values (5,'PatientAccess.Actions.ProvideEmployerPhoneSubscriber',5,13,'Provide Employer Phone Subscriber');
insert into actions values (6,'PatientAccess.Actions.ProvideEmploymentStatus',6,13,'Provide Employment Status');
insert into actions values (7,'PatientAccess.Actions.ProvideEthnicity',7,13,'Provide Ethnicity');
insert into actions values (8,'PatientAccess.Actions.ProvideLanguage',8,13,'Provide Language');
insert into actions values (9,'PatientAccess.Actions.ProvideMailingAddress',9,13,'Provide Mailing Address');
insert into actions values (10,'PatientAccess.Actions.ProvideMailingAddressAreaCode',10,13,'Provide Mailing Address Area Code');
insert into actions values (11,'PatientAccess.Actions.ProvideMailingAddressPhone',11,13,'Provide Mailing Address Phone');
insert into actions values (12,'PatientAccess.Actions.ProvideMaritalStatus',12,13,'Provide Marital Status');
insert into actions values (13,'PatientAccess.Actions.ProvideMissingDemographics',13,NULL,'Provide Missing Demographics');
insert into actions values (14,'PatientAccess.Actions.ProvideOccIndustry',14,13,'Provide Occurance Industry');
insert into actions values (15,'PatientAccess.Actions.ProvidePatientRetiredDate',15,13,'Provide Patient Retired Date');
insert into actions values (16,'PatientAccess.Actions.ProvideRace',16,13,'Provide Race');
insert into actions values (17,'PatientAccess.Actions.FollowUpGuarantorValidation',17,NULL,'Followup Guarantor Validation');
insert into actions values (18,'PatientAccess.Actions.UnknownPatientGender',18,NULL,'Unknown Patient gender');
insert into actions values (19,'PatientAccess.Actions.UnknownSocialSecurityNumberStatus',19,NULL,'Unknown SSN Status');
insert into actions values (20,'PatientAccess.Actions.RequiredSignatureOnCOSForm',20,NULL,'Required Signature on COS Form');
insert into actions values (21,'PatientAccess.Actions.COSFormNotSigned',21,NULL,'Patient Refused to sign COS');
insert into actions values (22,'PatientAccess.Actions.FollowUpInsuranceInformation',22,NULL,'Followup Insurance Information');
insert into actions values (23,'PatientAccess.Actions.CompletePostMSERegistration',23,NULL,'Complete Post-MSE Registration');
insert into actions values (24,'PatientAccess.Actions.NoShow',24,NULL,'Reschedule, cancel, or activate the account');
insert into actions values (25,'PatientAccess.Actions.InsuranceVerification',25,NULL,'Insurance Verification');
insert into actions values (26,'PatientAccess.Actions.BenefitsVerificationIncomplete',26,NULL,'Benefits Verification Incomplete, Follow up required.');
insert into actions values (27,'PatientAccess.Actions.AuthorizationIncomplete',27,NULL,'Authorization Incomplete, Follow up required.');
insert into actions values (28,'PatientAccess.Actions.IncorrectPlanID',28,NULL,'Incorrect PlanID. The plan you have selected is not a workers''s comp plan. Review insurance plan and diagnosis selections.');
insert into actions values (29,'PatientAccess.Actions.PatientLiability',29,NULL,'Patient Liability');
insert into actions values (30,'PatientAccess.Actions.DeterminePatientLiability',30,NULL,'Determine Patient Liability');
insert into actions values (31,'PatientAccess.Actions.RedeterminePatientLiability',31,NULL,'Redetermine Patient Liability');
insert into actions values (32,'PatientAccess.Actions.CollectPatientLiability',32,NULL,'Collect PatientLiability');

CALL DROP_TABLE ('PACCESS','RULES');

CREATE TABLE RULES ( 
  RULEID                  INT NOT NULL, 
  DESCRIPTION         VARCHAR (150), 
  TYPE                     VARCHAR (1500), 
  WORKLISTID           INT, 
  COMPOSITERULEID  INT);


insert into rules values (1,'Provide Drivers License','PatientAccess.Rules.DriversLicenseRequired',1,13);
insert into rules values (2,'Provide Employer','PatientAccess.Rules.EmployerRequired',1,13);
insert into rules values (3,'Provide Employer Address','PatientAccess.Rules.EmployerAddressRequired',1,13);
insert into rules values (4,'Provide Employer Phone Area Code','PatientAccess.Rules.EmployerPhoneAreaCodeRequired',1,13);
insert into rules values (5,'Provide Employer Phone Subscriber','PatientAccess.Rules.EmployerPhoneSubscriberRequired',1,13);
insert into rules values (6,'Provide Employment Status','PatientAccess.Rules.EmploymentStatusRequired',1,13);
insert into rules values (7,'Provide Ethnicity','PatientAccess.Rules.EthnicityRequired',1,13);
insert into rules values (8,'Provide Language','PatientAccess.Rules.LanguageRequired',1,13);
insert into rules values (9,'Provide Mailing Address','PatientAccess.Rules.MailingAddressRequired',1,13);
insert into rules values (10,'Provide Mailing Address Area Code','PatientAccess.Rules.MailingAddressAreaCodeRequired',1,13);
insert into rules values (11,'Provide Mailing Address Phone','PatientAccess.Rules.MailingAddressPhoneRequired',1,13);
insert into rules values (12,'Provide Marital Status','PatientAccess.Rules.MaritalStatusRequired',1,13);
insert into rules values (13,'Provide Missing Demographics','PatientAccess.Rules.DemographicsRequired',1,null);
insert into rules values (14,'Provide Occurance Industry','PatientAccess.Rules.OCCIndustryRequired',1,13);
insert into rules values (15,'Provide Patient Retired Date','PatientAccess.Rules.PatientRetiredDateRequired',1,13);
insert into rules values (16,'Provide Race','PatientAccess.Rules.RaceRequired',1,13);
insert into rules values (17,'FollowUp Guarantor Validation','PatientAccess.Rules.GuarantorValidationRequired',2,null);
insert into rules values (18,'Unknown Patient Gender','PatientAccess.Rules.PatientGenderRequired',2,null);
insert into rules values (19,'Unknown SSN Status','PatientAccess.Rules.SocialSecurityNumberStatusRequired',2,null);
insert into rules values (20,'Required signature on COS Form','PatientAccess.Rules.SignatureOnCOSFormRequired',2,null);
insert into rules values (21,'PatientRefused to sign COS Form','PatientAccess.Rules.RefusedToSignCOSForm',2,null);
insert into rules values (22,'Followup Insurance Information','PatientAccess.Rules.FollowupInsuranceInformationRequired',2,null);
insert into rules values (23,'Complete Post MSE Registration','PatientAccess.Rules.PostMSERegistrationRequired',5,null);
insert into rules values (24,'No Show','PatientAccess.Rules.PatientDidNotAppear',6,null);
insert into rules values (25,'Insurance Verification','PatientAccess.Rules.InsuranceVerificationRule',3,null);
insert into rules values (26,'Benefits Verification Incomplete','PatientAccess.Rules.BenefitsVerificationRequired',3,null);
insert into rules values (27,'Authorization Incomplete','PatientAccess.Rules.AuthorizationRequired',3,null);
insert into rules values (28,'Incorrect PlanID','PatientAccess.Rules.PlanIDRequired',3,null);
insert into rules values (29,'Patient Liability','PatientAccess.Rules.PatientLiabilityRule',4,null);
insert into rules values (30,'Determine Patient Liability','PatientAccess.Rules.PatientLiabilityToBeDetermined',4,null);
insert into rules values (31,'Redetermine Patient Liability','PatientAccess.Rules.PatientLiabilityToBeRedetermined',4,null);
insert into rules values (32,'Collect PatientLiability','PatientAccess.Rules.PatientLiabilityToBeCollected',4,null);
insert into rules values (33,'Provide Drivers License','PatientAccess.Rules.DriversLicensePreferred',1,45);
insert into rules values (34,'Provide Employer','PatientAccess.Rules.EmployerPreferred',1,45);
insert into rules values (35,'Provide Employer Address','PatientAccess.Rules.EmployerAddressPreferred',1,45);
insert into rules values (36,'Provide Employer Phone Area Code','PatientAccess.Rules.EmployerPhoneAreaCodePreferred',1,45);
insert into rules values (37,'Provide Employer Phone Subscriber','PatientAccess.Rules.EmployerPhoneSubscriberPreferred',1,45);
insert into rules values (38,'Provide Employment Status','PatientAccess.Rules.EmploymentStatusPreferred',1,45);
insert into rules values (39,'Provide Ethnicity','PatientAccess.Rules.EthnicityPreferred',1,45);
insert into rules values (40,'Provide Language','PatientAccess.Rules.LanguagePreferred',1,45);
insert into rules values (41,'Provide Mailing Address','PatientAccess.Rules.MailingAddressPreferred',1,45);
insert into rules values (42,'Provide Mailing Address Area Code','PatientAccess.Rules.MailingAddressAreaCodePreferred',1,45);
insert into rules values (43,'Provide Mailing Address Phone','PatientAccess.Rules.MailingAddressPhonePreferred',1,45);
insert into rules values (44,'Provide Marital Status','PatientAccess.Rules.MaritalStatusPreferred',1,45);
insert into rules values (45,'Provide Missing Demographics','PatientAccess.Rules.DemographicsPreferred',1,null);
insert into rules values (46,'Provide Occurance Industry','PatientAccess.Rules.OCCIndustryPreferred',1,45);
insert into rules values (47,'Provide Patient Retired Date','PatientAccess.Rules.PatientRetiredDatePreferred',1,45);
insert into rules values (48,'Provide Race','PatientAccess.Rules.RacePreferred',1,45);

COMMIT;

CALL DROP_TABLE ('PACCESS','WORKLISTRULES');

CREATE TABLE WORKLISTRULES ( 
  WORKLISTID  INT NOT NULL, 
  RULEID          INT NOT NULL);

INSERT INTO WORKLISTRULES VALUES (2,1 );
INSERT INTO WORKLISTRULES VALUES (2,2 );
INSERT INTO WORKLISTRULES VALUES (2,3 );
INSERT INTO WORKLISTRULES VALUES (2,4 );
INSERT INTO WORKLISTRULES VALUES (2,5 );
INSERT INTO WORKLISTRULES VALUES (2,6 );
INSERT INTO WORKLISTRULES VALUES (2,7 );
INSERT INTO WORKLISTRULES VALUES (2,8 );
INSERT INTO WORKLISTRULES VALUES (2,9 );
INSERT INTO WORKLISTRULES VALUES (2,10);
INSERT INTO WORKLISTRULES VALUES (2,11);
INSERT INTO WORKLISTRULES VALUES (2,12);
INSERT INTO WORKLISTRULES VALUES (2,13);
INSERT INTO WORKLISTRULES VALUES (2,14);
INSERT INTO WORKLISTRULES VALUES (2,15);
INSERT INTO WORKLISTRULES VALUES (2,16);
INSERT INTO WORKLISTRULES VALUES (2,17);
INSERT INTO WORKLISTRULES VALUES (2,18);
INSERT INTO WORKLISTRULES VALUES (2,19);
INSERT INTO WORKLISTRULES VALUES (2,20);
INSERT INTO WORKLISTRULES VALUES (2,21);
INSERT INTO WORKLISTRULES VALUES (2,22);
INSERT INTO WORKLISTRULES VALUES (2,33);
INSERT INTO WORKLISTRULES VALUES (2,34);
INSERT INTO WORKLISTRULES VALUES (2,35);
INSERT INTO WORKLISTRULES VALUES (2,36);
INSERT INTO WORKLISTRULES VALUES (2,37);
INSERT INTO WORKLISTRULES VALUES (2,38);
INSERT INTO WORKLISTRULES VALUES (2,39);
INSERT INTO WORKLISTRULES VALUES (2,40);
INSERT INTO WORKLISTRULES VALUES (2,41);
INSERT INTO WORKLISTRULES VALUES (2,42);
INSERT INTO WORKLISTRULES VALUES (2,43);
INSERT INTO WORKLISTRULES VALUES (2,44);
INSERT INTO WORKLISTRULES VALUES (2,45);
INSERT INTO WORKLISTRULES VALUES (2,46);
INSERT INTO WORKLISTRULES VALUES (2,47);
INSERT INTO WORKLISTRULES VALUES (2,48);
INSERT INTO WORKLISTRULES VALUES (3,25);
INSERT INTO WORKLISTRULES VALUES (3,26);
INSERT INTO WORKLISTRULES VALUES (3,27);
INSERT INTO WORKLISTRULES VALUES (3,28);
INSERT INTO WORKLISTRULES VALUES (4,29);
INSERT INTO WORKLISTRULES VALUES (4,30);
INSERT INTO WORKLISTRULES VALUES (4,31);
INSERT INTO WORKLISTRULES VALUES (4,32);
INSERT INTO WORKLISTRULES VALUES (5,23);
INSERT INTO WORKLISTRULES VALUES (6,24);

COMMIT;

CALL DROP_TABLE('PACCESS','SEXES');

CREATE TABLE PACCESS.SEXES( 
	SexId                INT NOT NULL,
	SexCode            CHAR(1) NOT NULL,
	Description         VARCHAR(32)
,
                    DISPLAYSTRING  VARCHAR(32)
,
                    STATUSCD        CHAR(1) 
,
                    LASTUPDATED DATE);

INSERT INTO SEXES VALUES (1, 'F', 'FEMALE', 'FEMALE', null, null);
INSERT INTO SEXES VALUES (2, 'M', 'MALE', 'MALE', null, null);
INSERT INTO SEXES VALUES (3, 'U', 'UNKNOW',' UNKNOWN', null, null);

COMMIT;

CALL DROP_TABLE ('PACCESS','DISCHARGEDISPOSITIONCODES');

CREATE TABLE DISCHARGEDISPOSITIONCODES ( 
  DISPOSITIONCODE  CHAR (10)     NOT NULL, 
  CODEDESCRIPTION  VARCHAR(20));



INSERT INTO DISCHARGEDISPOSITIONCODES VALUES ('F','ACUTE-ANOTHER HOSP');
INSERT INTO DISCHARGEDISPOSITIONCODES VALUES ('B','ACUTE-THIS HOSP');
INSERT INTO DISCHARGEDISPOSITIONCODES VALUES ('L','AGAINST MED ADVICE');
INSERT INTO DISCHARGEDISPOSITIONCODES VALUES ('M','EXPIRED');
INSERT INTO DISCHARGEDISPOSITIONCODES VALUES ('N','HOME HEALTH');
INSERT INTO DISCHARGEDISPOSITIONCODES VALUES ('O','HOME HEALTH IV');
INSERT INTO DISCHARGEDISPOSITIONCODES VALUES ('A','HOME/SELF CARE');
INSERT INTO DISCHARGEDISPOSITIONCODES VALUES ('U','HOSPICE - FACILITY');
INSERT INTO DISCHARGEDISPOSITIONCODES VALUES ('T','HOSPICE - HOME');
INSERT INTO DISCHARGEDISPOSITIONCODES VALUES ('I','ICF-ELSEWHERE');
INSERT INTO DISCHARGEDISPOSITIONCODES VALUES ('E','ICF-THIS HOSPITAL');
INSERT INTO DISCHARGEDISPOSITIONCODES VALUES ('K','JAIL/PRISON');
INSERT INTO DISCHARGEDISPOSITIONCODES VALUES ('Z','L/T CARE-ELSEWHERE');
INSERT INTO DISCHARGEDISPOSITIONCODES VALUES ('Y','L/T CARE-THIS HOSP');
INSERT INTO DISCHARGEDISPOSITIONCODES VALUES ('G','OTHR HSP CR-OTHR HSP');
INSERT INTO DISCHARGEDISPOSITIONCODES VALUES ('C','OTHR HSP CR-THIS HSP');
INSERT INTO DISCHARGEDISPOSITIONCODES VALUES ('@','REHAB - OTHER HOSP');
INSERT INTO DISCHARGEDISPOSITIONCODES VALUES ('#','REHAB - THIS HOSP');
INSERT INTO DISCHARGEDISPOSITIONCODES VALUES ('J','RESIDENTIAL CARE');
INSERT INTO DISCHARGEDISPOSITIONCODES VALUES ('H','SNF-ELSEWHERE');
INSERT INTO DISCHARGEDISPOSITIONCODES VALUES ('{','SNF-OTH MCD/NO MCR');
INSERT INTO DISCHARGEDISPOSITIONCODES VALUES ('D','SNF-THIS HOSPITAL');
INSERT INTO DISCHARGEDISPOSITIONCODES VALUES ('V','SWING BD-MCRE APPR');
INSERT INTO DISCHARGEDISPOSITIONCODES VALUES ('&','XFER TO FEDERAL HOSP');
INSERT INTO DISCHARGEDISPOSITIONCODES VALUES ('<','XFR PSYCH - OTH HOSP');
INSERT INTO DISCHARGEDISPOSITIONCODES VALUES ('}','XFR PSYCH -THIS HOSP');

COMMIT;

CALL DROP_TABLE('PACCESS','PATIENTTYPES');

CREATE TABLE PATIENTTYPES ( 
  PATIENTTYPECODE         INT NOT NULL, 
  PATIENTTYPEDESCRIPTION  CHAR (10)     NOT NULL, 
  CONSTRAINT PK_PATIENTTYPE
  PRIMARY KEY ( PATIENTTYPECODE ) ) ; 

insert into patienttypes values (0, 'PREADMIT');
insert into patienttypes values (1, 'INPATIENT');
insert into patienttypes values (2, 'OUTPATIENT');
insert into patienttypes values (3, 'OP-ER PAT');
insert into patienttypes values (4, 'OP-RECRNG');
insert into patienttypes values (9, 'NONPATIENT');

COMMIT;

