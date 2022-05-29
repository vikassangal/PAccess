      create trigger RECORDNEWEMPLOYER
AFTER UPDATE ON PADATA.NCEM10P
REFERENCING NEW AS  NEWEMP
FOR EACH ROW
BEGIN ATOMIC
	INSERT INTO PACCESS.EMPTXN 
	VALUES(NEWEMP.EMFUUN, NEWEMP.EMNAME, NEWEMP.EMURFG, NEWEMP.EMADDT, NEWEMP.EMLMDT, NEWEMP.EMDLDT,
		NEWEMP.EMCODE, NEWEMP.EMACNT, NEWEMP.EMUSER, NEWEMP.EMNEID, CURRENT TIMESTAMP, 'U');
END

create trigger RECORDUPDATEEMPLOYER
AFTER INSERT ON PADATA.NCEM10P
REFERENCING NEW AS  NEWEMP
FOR EACH ROW
BEGIN ATOMIC
	INSERT INTO PACCESS.EMPTXN 
	VALUES(NEWEMP.EMFUUN, NEWEMP.EMNAME, NEWEMP.EMURFG, NEWEMP.EMADDT, NEWEMP.EMLMDT, NEWEMP.EMDLDT,
		NEWEMP.EMCODE, NEWEMP.EMACNT, NEWEMP.EMUSER, NEWEMP.EMNEID, CURRENT TIMESTAMP, 'A');
END

create trigger RECORDOLDEMPLOYER
AFTER DELETE ON PADATA.NCEM10P
REFERENCING OLD AS OLDEMP
FOR EACH ROW
BEGIN ATOMIC
	INSERT INTO PACCESS.EMPTXN 
	VALUES(OLDEMP.EMFUUN, OLDEMP.EMNAME, OLDEMP.EMURFG, OLDEMP.EMADDT, OLDEMP.EMLMDT, OLDEMP.EMDLDT,
		OLDEMP.EMCODE, OLDEMP.EMACNT, OLDEMP.EMUSER, OLDEMP.EMNEID, CURRENT TIMESTAMP, 'D');
END

create trigger RECORDNEWEMPADDR
AFTER INSERT ON PADATA.NCEMADR
REFERENCING NEW as NEWADDR
FOR EACH ROW
BEGIN ATOMIC
	INSERT INTO PACCESS.EMPADDR
	VALUES( NEWADDR.EMFUUN CHAR(3) CCSID 37 NOT NULL DEFAULT '' , 
	NEWADDR.EMCODE , NEWADDR.EMADD# ,NEWADDR.EMADDR , NEWADDR.EMCITY , NEWADDR.EMST , 
	NEWADDR.EMZIP , NEWADDR.EMPH# , NEWADDR.EMADDT ,NEWADDR.EMLMDT , NEWADDR.EMANAM ,
	CURRENT TIMESTAMP, 'A');
END

create trigger RECORDUPDATEEMPADDR
AFTER UPDATE ON PADATA.NCEMADR
REFERENCING NEW as NEWADDR
FOR EACH ROW
BEGIN ATOMIC
	INSERT INTO PACCESS.EMPADDR
	VALUES( NEWADDR.EMFUUN CHAR(3) CCSID 37 NOT NULL DEFAULT '' , 
	NEWADDR.EMCODE , NEWADDR.EMADD# ,NEWADDR.EMADDR , NEWADDR.EMCITY , NEWADDR.EMST , 
	NEWADDR.EMZIP , NEWADDR.EMPH# , NEWADDR.EMADDT ,NEWADDR.EMLMDT , NEWADDR.EMANAM ,
	CURRENT TIMESTAMP,'U');
END

create trigger RECORDOLDEMPADDR
AFTER DELETE ON PADATA.NCEMADR
REFERENCING OLD as OLDADDR
FOR EACH ROW
BEGIN ATOMIC
	INSERT INTO PACCESS.EMPADDR
	VALUES( OLDADDR.EMFUUN CHAR(3) CCSID 37 NOT NULL DEFAULT '' , 
	OLDADDR.EMCODE , OLDADDR.EMADD# ,OLDADDR.EMADDR , OLDADDR.EMCITY , OLDADDR.EMST , 
	OLDADDR.EMZIP , OLDADDR.EMPH# , OLDADDR.EMADDT ,OLDADDR.EMLMDT , OLDADDR.EMANAM ,
	CURRENT TIMESTAMP,'D');
END
