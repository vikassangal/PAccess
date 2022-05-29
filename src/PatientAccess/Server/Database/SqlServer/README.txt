1. Overview:
===============================================================================
Starting with the changeover from Oracle to SqlServer, the project started
using an opensource product named LiquiBase to manage changes to the 
database (not DB2, however).

For an overview of the product, please see the LiquiBase home page:

	http://www.liquibase.org/

From the site: 

	LiquiBase is an open source (LGPL), database-independent library for 
	tracking, managing and applying database changes. It is built on a 
	simple premise: All database changes (structure and data) are 
	stored in an XML-based descriptive manner and checked into source 
	control. 


2. Structure of files:
===============================================================================

Everything in LiquiBase centers around an artifact called a changelog. 
Changelogs can contain commands to execute against the database, references
to other changelogs, or both.

The following is a snapshot of the structure of our LiquiBase repository:

	SRC\PATIENTACCESS\SERVER\DATABASE\SQLSERVER
	|   changelog.xml
	|   Migrate.proj
	|   README.txt
	|
	+---Install
	|   +---Data
	|   |       Load.DataValidation.TicketTypes.xml
	|   |       Load.Facility.Facilities.xml
	|   |       ...
	|   |       _changelog.xml
	|   |       _template.xml
	|   |
	|   +---Functions
	|   |       RuleEngine.IsCompositeAction.xml
	|   |       RuleEngine.IsCompositeRule.xml
	|   |       Utility.ArrayItemAt.xml
	|   |       ...
	|   |       _changelog.xml
	|   |       _template.xml
	|   |
	|   +---Procedures
	|   |       Announcement.SaveAnnouncement.xml
	|   |       Announcement.SelectAllAnnouncementsFor.xml
	|   |       Announcement.SelectCurrentAnnouncementsFor.xml
	|   |       CrashDump.DeleteCrashReportsByComment.xml
	|   |       ...
	|   |       _changelog.xml
	|   |       _template.xml
	|   |
	|   +---Security
	|   |       Logins.xml
	|   |       Roles.xml
	|   |       Schemas.xml
	|   |       Users.xml
	|   |       _changelog.xml
	|   |
	|   \---Tables
	|           Announcement.Announcements.xml
	|           Announcement.AnnouncementsToRolesToFacilities.xml
	|           CrashDump.BreadCrumbLogs.xml
	|           ...
	|           _changelog.xml
	|           _template.xml
	|
	+---Releases
	|   \---v1.12
	|           2009-07-07-1868.xml
	|           2009-07-07-2376.xml
	|           2009-07-09-2374.xml
	|           2009-07-10-3471.xml
	|           2009-07-15-3513.xml
	|           2009-07-17-3539.xml
	|           _changelog.xml
	|
	\---Utility
			CreatePatientAccessSupportLocal.sql
			CreatePatientAccessTrunkLocal.sql

<will continue...>



































