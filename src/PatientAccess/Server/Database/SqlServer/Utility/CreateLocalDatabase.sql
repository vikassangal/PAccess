CREATE DATABASE [PatientAccess$(Branch)Local];
GO
EXEC dbo.sp_dbcmptlevel @dbname=N'PatientAccess$(Branch)Local', @new_cmptlevel=90;
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
	EXEC [PatientAccess$(Branch)Local].[dbo].[sp_fulltext_database] @action = 'disable';
ALTER DATABASE [PatientAccess$(Branch)Local] SET ANSI_NULL_DEFAULT OFF 
ALTER DATABASE [PatientAccess$(Branch)Local] SET ANSI_NULLS OFF 
ALTER DATABASE [PatientAccess$(Branch)Local] SET ANSI_PADDING OFF 
ALTER DATABASE [PatientAccess$(Branch)Local] SET ANSI_WARNINGS OFF 
ALTER DATABASE [PatientAccess$(Branch)Local] SET ARITHABORT OFF 
ALTER DATABASE [PatientAccess$(Branch)Local] SET AUTO_CLOSE OFF 
ALTER DATABASE [PatientAccess$(Branch)Local] SET AUTO_CREATE_STATISTICS ON 
ALTER DATABASE [PatientAccess$(Branch)Local] SET AUTO_SHRINK OFF 
ALTER DATABASE [PatientAccess$(Branch)Local] SET AUTO_UPDATE_STATISTICS ON 
ALTER DATABASE [PatientAccess$(Branch)Local] SET CURSOR_CLOSE_ON_COMMIT OFF 
ALTER DATABASE [PatientAccess$(Branch)Local] SET CURSOR_DEFAULT  GLOBAL 
ALTER DATABASE [PatientAccess$(Branch)Local] SET CONCAT_NULL_YIELDS_NULL OFF 
ALTER DATABASE [PatientAccess$(Branch)Local] SET NUMERIC_ROUNDABORT OFF 
ALTER DATABASE [PatientAccess$(Branch)Local] SET QUOTED_IDENTIFIER OFF 
ALTER DATABASE [PatientAccess$(Branch)Local] SET RECURSIVE_TRIGGERS OFF 
ALTER DATABASE [PatientAccess$(Branch)Local] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
ALTER DATABASE [PatientAccess$(Branch)Local] SET DATE_CORRELATION_OPTIMIZATION OFF 
ALTER DATABASE [PatientAccess$(Branch)Local] SET PARAMETERIZATION SIMPLE 
ALTER DATABASE [PatientAccess$(Branch)Local] SET  READ_WRITE 
ALTER DATABASE [PatientAccess$(Branch)Local] SET RECOVERY SIMPLE 
ALTER DATABASE [PatientAccess$(Branch)Local] SET  MULTI_USER 
ALTER DATABASE [PatientAccess$(Branch)Local] SET PAGE_VERIFY CHECKSUM
GO
USE [PatientAccess$(Branch)Local];
IF NOT EXISTS (SELECT name FROM sys.filegroups WHERE is_default=1 AND name = N'PRIMARY') ALTER DATABASE [PatientAccess$(Branch)Local] MODIFY FILEGROUP [PRIMARY] DEFAULT;
GO