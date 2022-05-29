using System;
using System.Xml.Serialization;
using PatientAccess.Domain.Parties;

namespace PatientAccess.BrokerInterfaces.CrashReporting 
{
    /// <summary> 
    /// Summary description for CrashReport 
    /// </summary>
    [Serializable]
    public class CrashReport
    {
        #region Event Handlers
        #endregion

        #region Methods
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the name of the application.
        /// </summary>
        /// <value>The name of the application.</value>
        public string ApplicationName
        {
            get
            {
                return i_ApplicationName;
            }
            set
            {
                i_ApplicationName = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the crash report in the format -
        ///     "ExceptionType:ClassName:MethodName:ExceptionMessage"
        ///     
        ///     Field Composition:   
        ///     e.GetType().Name
        ///     e.TargetSite.DeclaringType.Name
        ///     e.TargetSite.Name
        ///     e.Message
        /// </summary>
        /// <value>The name of the crash report.</value>
        [XmlIgnore()]
        public string CrashReportName
        {
            get
            {
                return String.Format( "{0}:{1}:{2}:{3}", 
                    this.ExceptionType, 
                    this.ClassName, 
                    this.MethodName, 
                    this.ExceptionMessage 
                    );
            }
        }

        /// <summary>
        /// Gets or sets the type of the exception.
        /// </summary>
        /// <value>The type of the exception.</value>
        public string ExceptionType
        {
            get
            {
                return i_ExceptionType;
            }
            set
            {
                i_ExceptionType = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the class.
        /// </summary>
        /// <value>The name of the class.</value>
        public string ClassName
        {
            get
            {
                return i_ClassName;
            }
            set
            {
                i_ClassName = value;
            }
        }
        
        /// <summary>
        /// Gets or sets the name of the method.
        /// </summary>
        /// <value>The name of the method.</value>
        public string MethodName
        {
            get
            {
                return i_MethodName;
            }
            set
            {
                i_MethodName = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [exception is fatal].
        /// </summary>
        /// <value><c>true</c> if [exception is fatal]; otherwise, <c>false</c>.</value>
        public bool ExceptionIsFatal
        {
            get 
            {
                return i_ExceptionIsFatal;
            }
            set 
            {
                i_ExceptionIsFatal = value;
            }
        }

        /// <summary>
        /// Gets or sets the exception message.
        /// </summary>
        /// <value>The exception message.</value>
        public string ExceptionMessage
        {
            get
            {
                return i_ExceptionMessage;
            }
            set
            {
                i_ExceptionMessage = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the assembly.
        /// </summary>
        /// <value>The name of the assembly.</value>
        public string AssemblyName
        {
            get
            {
                return i_AssemblyName;
            }
            set
            {
                i_AssemblyName = value;
            }
        }

        /// <summary>
        /// Gets or sets the namespace.
        /// </summary>
        /// <value>The namespace.</value>
        public string Namespace
        {
            get
            {
                return i_Namespace;
            }
            set
            {
                i_Namespace = value;
            }
        }

        /// <summary>
        /// Gets or sets the comments.
        /// </summary>
        /// <value>The comments.</value>
        public string Comments
        {
            get
            {
                return i_Comments;
            }
            set
            {
                i_Comments = value;
            }
        }

        /// <summary>
        /// Gets or sets the phone number.
        /// </summary>
        /// <value>The phone number.</value>
        public PhoneNumber PhoneNumber
        {
            get
            {
                return i_PhoneNumber;
            }
            set
            {
                i_PhoneNumber = value;
            }
        }
        
        /// <summary>
        /// Gets or sets the Email Address.
        /// </summary>
        /// <value>The Email Address.</value>
        public string EmailAddress
        {
            get
            {
                if( i_EmailAddress != null )
                {
                    return i_EmailAddress;
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                if( value != null )
                {
                    i_EmailAddress = value;
                }
                else
                {
                    i_EmailAddress = string.Empty;
                }
            }
        }

        /// <summary>
        /// Gets or sets the UPN (eTenet ID).
        /// </summary>
        /// <value>The UPN (eTenet ID).</value>
        public string Upn
        {
            get
            {
                if( i_Upn != null )
                {
                    return i_Upn;
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                if( value != null )
                {
                    i_Upn = value;
                }
                else
                {
                    i_Upn = string.Empty;
                }
            }
        }

        /// <summary>
        /// Gets or sets the user local permissions.
        /// </summary>
        /// <value>The user local permissions.</value>
        public string UserLocalPermissions
        {
            get
            {
                return i_UserLocalPermissions;
            }
            set
            {
                i_UserLocalPermissions = value;
            }
        }

        /// <summary>
        /// Gets or sets the facility HSP code.
        /// </summary>
        /// <value>The facility HSP code.</value>
        public string FacilityHSPCode
        {
            get
            {
                if( i_FacilityHSPCode != null )
                {
                    return i_FacilityHSPCode;
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                if( value != null )
                {
                    i_FacilityHSPCode = value;
                }
                else
                {
                    i_FacilityHSPCode = string.Empty;
                }
            }
        }

        /// <summary>
        /// Gets or sets the exception detail.
        /// </summary>
        /// <value>The exception detail.</value>
        public string ExceptionDetail
        {
            get
            {
                if( i_ExceptionDetail != null )
                {
                    return i_ExceptionDetail;
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                if( value != null )
                {
                    i_ExceptionDetail = value;
                }
                else
                {
                    i_ExceptionDetail = string.Empty;
                }
            }
        }

        /// <summary>
        /// Gets or sets the workstation ID.
        /// </summary>
        /// <value>The workstation ID.</value>
        public string WorkstationID
        {
            get
            {
                if( i_WorkstationID != null )
                {
                    return i_WorkstationID;
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                if( value != null )
                {
                    i_WorkstationID = value;
                } 
                else
                {
                    i_WorkstationID = string.Empty;
                }
            }
        }

        /// <summary>
        /// Gets or sets the RAM total on system.
        /// </summary>
        /// <value>The RAM total on system.</value>
        public int RAMTotalOnSystem
        {
            get
            {
                return i_RamTotalOnSystem;
            }
            set
            {
                i_RamTotalOnSystem = value;
            }
        }

        /// <summary>
        /// Gets or sets the RAM used by patient access.
        /// </summary>
        /// <value>The RAM used by patient access.</value>
        public int RAMUsedByPatientAccess
        {
            get
            {
                return i_RamUsedByPatientAccess;
            }
            set
            {
                i_RamUsedByPatientAccess = value;
            }
        }

        public Byte[] ScreenCapture
        {
            get
            {
                return i_ScreenCapture;
            }
            set
            {
                i_ScreenCapture = value;
            }
        }

        /// <summary>
        /// Gets or sets the version of patient access.
        /// </summary>
        /// <value>The version of patient access.</value>
        public string VersionOfPatientAccess
        {
            get
            {
                return i_VersionOfPatientAccess;
            }
            set
            {
                i_VersionOfPatientAccess = value;
            }
        }
        
        /// <summary>
        /// Gets or sets the bread crumb log archive file name.
        /// </summary>
        /// <value>The bread crumb log archive file name.</value>
        public string BreadCrumbLogArchiveName
        {
            get
            {
                if( i_BreadCrumbLogArchiveName != null )
                {
                    return i_BreadCrumbLogArchiveName;
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                if( value != null )
                {
                    i_BreadCrumbLogArchiveName = value;
                } 
                else
                {
                    i_BreadCrumbLogArchiveName = string.Empty;
                }
            }
        }
        
        /// <summary>
        /// Gets or sets the bread crumb log.
        /// </summary>
        /// <value>The bread crumb log.</value>
        public Byte[] BreadCrumbLog
        {
            get
            {
                return i_BreadCrumbLog;
            }
            set
            {
                i_BreadCrumbLog = value;
            }
        }

        /// <summary>
        /// Gets or sets the hard drive utilization.
        /// </summary>
        /// <value>The hard drive utilization.</value>
        public string HardDriveUtilization
        {
            get
            {
                return i_HardDriveUtilization;
            }
            set
            {
                i_HardDriveUtilization = value;
            }
        }

        /// <summary>
        /// Gets or sets the time on PC.
        /// </summary>
        /// <value>The time on PC.</value>
        public DateTime TimeOnPC
        {
            get
            {
                return i_TimeOnPC;
            }
            set
            {
                i_TimeOnPC = value;
            }
        }

        /// <summary>
        /// Gets or sets the OS version.
        /// String.Format( lblOs.Text, System.Environment.OsVersion.ToString() );
        /// </summary>
        /// <value>The OS version.</value>
        public string OsVersion
        {
            get
            {
                return i_OsVersion;
            }
            set
            {
                i_OsVersion = value;
            }
        }
        
        /// <summary>
        /// Gets or sets the installed hotfixes.
        /// </summary>
        /// <value>The installed hotfixes.</value>
        public string InstalledHotfixes
        {
            get
            {
                return i_InstalledHotfixes;
            }
            set
            {
                i_InstalledHotfixes = value;
            }
        }

        /// <summary>
        /// Gets or sets the framework version.
        /// String.Format( lblFramework.Text, System.Environment.Version.ToString() );
        /// </summary>
        /// <value>The framework version.</value>
        public string FrameworkVersion
        {
            get
            {
                return i_FrameworkVersion;
            }
            set
            {
                i_FrameworkVersion = value;
            }
        }

        /// <summary>
        /// Gets or sets the Background Intelligent Transfer Service (BITS) Version.
        /// </summary>
        /// <value>The Background Intelligent Transfer Service (BITS) Version.</value>
        public string BitsVersion
        {
            get
            {
                return i_BitsVersion;
            }
            set
            {
                i_BitsVersion = value;
            }
        }

        /// <summary>
        /// Gets or sets the internet explorer version.
        /// </summary>
        /// <value>The internet explorer version.</value>
        public string InternetExplorerVersion
        {
            get
            {
                return i_InternetExplorerVersion;
            }
            set
            {
                i_InternetExplorerVersion = value;
            }
        }
        
        /// <summary>
        /// Gets or sets the crash report ID.
        /// </summary>
        /// <value>The crash report ID.</value>
        public long CrashReportID
        {
            get
            {
                return i_CrashReportID;
            }
            set
            {
                i_CrashReportID = value;
            }
        }

        /// <summary>
        /// Gets or sets the defect ID.
        /// </summary>
        /// <value>The defect ID.</value>
        public int DefectID
        {
            get
            {
                return i_DefectID;
            }
            set
            {
                i_DefectID = value;
            }
        }

        /// <summary>
        /// Gets or sets the IP address of the machine.
        /// </summary>
        /// <value>The IP address.</value>
        public string ClientIP
        {
            get 
            {
                return i_ClientIP; 
            }
            set
            {
                i_ClientIP = value; 
            }
        }

        /// <summary>
        /// Gets or sets the Computer Name/Host Name of the machine.
        /// </summary>
        /// <value>The computer name.</value>
        public string ComputerName
        {
            get
            {
                return i_ComputerName;
            }
            set
            {
                i_ComputerName = value;
            }
        }

        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public CrashReport()
        {
        }

        public virtual void Dispose()
        {
        }
        #endregion

        #region Data Elements
        private string                                   i_ApplicationName = string.Empty;
        private string                                   i_ExceptionType = string.Empty;
        private string                                   i_ClassName = string.Empty;
        private string                                   i_MethodName = string.Empty;
        private bool                                     i_ExceptionIsFatal = false;
        private string                                   i_ExceptionMessage = string.Empty;
        private string                                   i_AssemblyName = string.Empty;
        private string                                   i_Namespace = string.Empty;
        private string                                   i_Comments = string.Empty;
        private PhoneNumber                              i_PhoneNumber;
        private string                                   i_EmailAddress = string.Empty;
        private string                                   i_Upn = string.Empty;
        private string                                   i_UserLocalPermissions = string.Empty;
        private string                                   i_FacilityHSPCode = string.Empty;
        private string                                   i_ExceptionDetail = string.Empty;
        private string                                   i_WorkstationID = string.Empty;
        private int                                      i_RamTotalOnSystem;
        private int                                      i_RamUsedByPatientAccess;
        private Byte[]                                   i_ScreenCapture;
        private string                                   i_VersionOfPatientAccess;
        private string                                   i_BreadCrumbLogArchiveName = string.Empty;
        private Byte[]                                   i_BreadCrumbLog;
        private string                                   i_HardDriveUtilization = string.Empty;
        private DateTime                                 i_TimeOnPC;
        private string                                   i_OsVersion = string.Empty;
        private string                                   i_InstalledHotfixes = string.Empty;
        private string                                   i_FrameworkVersion = string.Empty;
        private string                                   i_BitsVersion = string.Empty;
        private string                                   i_InternetExplorerVersion = string.Empty;
        private long                                     i_CrashReportID;
        private int                                      i_DefectID;
        private string i_ClientIP = string.Empty;
        private string i_ComputerName = string.Empty;
        #endregion

        #region Constants
        #endregion
    }
}