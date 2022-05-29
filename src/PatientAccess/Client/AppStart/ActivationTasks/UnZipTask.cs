using System;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace PatientAccess.AppStart.ActivationTasks
{
    //UnZipTask to extract zip archive on the client workstation.
    [Serializable]
    public class UnZipTask 
    {
        #region Event Handlers
        #endregion

        #region Methods

        public void UnZipFile( string fileName )
        {
            
            FileInfo zipFile = new FileInfo( fileName );

            try 
            {
                using (ZipInputStream s = new ZipInputStream(zipFile.OpenRead())) 
                { 
                    ZipEntry entry;

                    // extract the file or directory entry
                    while ((entry = s.GetNextEntry()) != null) 
                    {
                        if (entry.IsDirectory) 
                        {
                            ExtractDirectory(s, entry.Name, entry.DateTime);
                        } 
                        else 
                        {
                            ExtractFile(s, entry.Name, entry.DateTime);
                        }
                    }

                    // close the zip stream
                    s.Close();
                }
            } 
            catch (IOException) 
            {
                throw;
            } 
            catch (ZipException) 
            {
                throw;
            }
        }

        #endregion

        #region Properties

        public bool Overwrite
        {
            get
            {
                return i_Overwrite;
            }
            set
            {
                i_Overwrite = value;
            }
        }
        
        #endregion

        #region Private Methods

        protected void ExtractFile(Stream inputStream, string entryName, DateTime entryDate) 
        {
            // determine destination file
            FileInfo destFile = new FileInfo(Path.Combine(ToDirectory.FullName, entryName));

            // ensure destination directory exists
            if (!destFile.Directory.Exists) 
            {
                try 
                {
                    destFile.Directory.Create();
                    destFile.Directory.LastWriteTime = entryDate;
                    destFile.Directory.Refresh();
                } 
                catch (Exception) 
                {
                    throw;
                }
            }

            // determine if entry actually needs to be extracted
            if (!Overwrite && destFile.Exists && destFile.LastWriteTime >= entryDate) 
            {
                return;
            } 

            try 
            {
                // extract the entry
                using (FileStream sw = new FileStream(destFile.FullName, FileMode.Create, FileAccess.Write)) 
                {
                    int size = 2048;
                    byte[] data = new byte[2048];

                    while (true) 
                    {
                        size = inputStream.Read(data, 0, data.Length);
                        if (size > 0) 
                        {
                            sw.Write(data, 0, size);
                        } 
                        else 
                        {
                            break;
                        }
                    }

                    sw.Close();
                }
            } 
            catch (Exception) 
            {
                throw;
            }

            destFile.LastWriteTime = entryDate;
        }
        
        protected void ExtractDirectory(Stream inputStream, string entryName, DateTime entryDate) 
        {
            DirectoryInfo destDir = new DirectoryInfo(Path.Combine(ToDirectory.FullName, entryName));
            if (!destDir.Exists) 
            {
                try 
                {
                    destDir.Create();
                    destDir.LastWriteTime = entryDate;
                    destDir.Refresh();
                } 
                catch (Exception) 
                {
                }
            }
        }

        #endregion

        #region Private Properties

        private DirectoryInfo ToDirectory
        {
            get
            {
                return i_ToDirectory;
            }
            set
            {
                i_ToDirectory = value;
            }
        }

        #endregion

        #region Construction and Finalization

        public UnZipTask( string directoryName )
        {
            ToDirectory = new DirectoryInfo(directoryName);
        }

        public UnZipTask( string directoryName, bool overwrite ) : this( directoryName )
        {
            Overwrite = overwrite;
        }

        #endregion

        #region Data Elements

        private bool i_Overwrite = true;
        private DirectoryInfo i_ToDirectory;

        #endregion
    }
}
