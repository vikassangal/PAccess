using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;

namespace PatientAccess.Tests.Integration.UIAutomation.Util
{
    public static class PatientAccessProcess
    {
        public static void KillAllPatientAccessInstances()
        {
            var processes = Process.GetProcessesByName( "PatientAccess" );

            foreach ( var process in processes )
            {
                process.Kill();
                process.WaitForExit( 1000 );
            }
        }

        public static void StartNewPatientAccessProcess()
        {
            var patientAccessExecutablePath = ConfigurationManager.AppSettings["PatientAccessExecutablePath"];
            
            //var patientAccessExecutablePath =
            //    Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.ProgramFilesX86 ),
            //                  "Patient Access (Model Office)", "PatientAccess", "PatientAccess.exe" );

            Process.Start( patientAccessExecutablePath );
        }
    }
}
