Most of the critical patient and enterprise data for Patient Access is stored
in a DB2 database owned by our sister team, PBAR. We maintain a few of our
own tables inside the PACCESS schema, but these are exceptions to the rule.

We do, however, have a large number of functions and stored procedures which
server to speed up execution time and isolate the main Patient Access
application from structure changes in the PBAR database.

Since deployment to the iSeries machines is strictly controlled through a
product called TurnOver, we do not have an easy way to automate the
deployment of new data and programs. That aside, we will still organize
our schema repository so it is similar to the Liquibase solution used by
our SQL Server 2005 installation.

The file layout looks like the following:

DB2
├───Install
│   ├───Data
│   ├───Indexes
│   ├───Sequences
│   └───Tables
├───Latest
│   ├───Functions
│   ├───Procedures
│   ├───Triggers
│   └───Views
└───Releases
    ├───v1.13
    ├───v1.14
    └───v1.15
    └───...

Install:
===============================================================================
This directory contains creation and load scripts for items that can only
be easily created when there is no existing database. Generally speaking, you
won't be changing much or adding much of anything in this directory.


Latest:
===============================================================================
This contains create scripts for things which can be easily dropped and 
created without data loss. You can edit these items directly and event 
notification will tell the DB2 DBA that the item has been updated. New
procdeures and functions can also be added directly to the appropriate
directory.

Releases:
===============================================================================
This directory contains any schema changes that are needed for a particular
release. This includes things like adding columns to tables, modifying data,
dropping objects, and creating new tables.

We tried to follow the Liquibase naming standard for the DB2 schema, but this
has proven to be cumbersome at best. So, we now use a more descriptive standard
for the DB2 changes:

	Create.TableName
	Alter.TableName
	Create.IndexName
	Load.TableName (for bulk loading)
	Update.TableName (for all other data changes)

NOTE: This is not a general change. This only applies to DB2.

