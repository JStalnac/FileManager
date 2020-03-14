using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace DirtBot.Database.FileManagement
{
    public class ManagedDirectory : IDisposable
    {
        public DirectoryInfo DirectoryInfo { get; private set; }
        public ManagedFile[] Files { get; private set; }
        public ManagedDirectory[] Directories { get; private set; }

        public ManagedDirectory(string path, ManagedFile[] files, ManagedDirectory[] directories)
        {
            DirectoryInfo = new DirectoryInfo(path);
            Files = files;
            Directories = directories;
        }

        #region IDisposable
        bool disposed = false;
        SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                handle.Dispose();
                // TODO: Dispose everything else...
            }

            disposed = true;
        }
        #endregion

        /// <summary>
        /// Gets the file by name.
        /// </summary>
        /// <param name="filename">Filename to search for.</param>
        /// <returns></returns>
        public ManagedFile GetFile(string filename)
        {
            // Loop through the files in this ManagedDirectory.
            foreach (var file in Files)
            {
                // Check if the filename is correct
                if (file.FileInfo.FullName == filename || file.FileInfo.Name == filename)
                {
                    // Return the file
                    return file;
                }
            }

            // There is no file...
            throw new FileNotFoundException($"No file named '{filename}'");
        }

        /// <summary>
        /// Gets a subdirectory by name
        /// </summary>
        /// <param name="directoryName">Name of the subdirectory.</param>
        /// <returns></returns>
        public ManagedDirectory GetDirectory(string directoryName)
        {
            foreach (ManagedDirectory directory in Directories)
            {
                if (directory.DirectoryInfo.Name == directoryName || directory.DirectoryInfo.FullName == directoryName)
                {
                    return directory;
                }
            }

            // Directory not found!
            return new ManagedDirectory(directoryName, null, null);
        }

        /// <summary>
        /// Gets the index of a file in Files. Returns -1 if no file is found
        /// </summary>
        /// <param name="filename">Filename to search for.</param>
        /// <returns></returns>
        public int IndexOf(string filename)
        {
            for (int i = 0; i < Files.Length; i++)
            {
                if (Files[i].FileInfo.Name == filename || Files[i].FileInfo.FullName == filename)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Reloads this directory.
        /// </summary>
        public void Refresh()
        {
            ManagedDirectory refreshed = FileManager.LoadDirectory(DirectoryInfo.FullName);
            Files = refreshed.Files;
            Directories = refreshed.Directories;
            DirectoryInfo = refreshed.DirectoryInfo;
        }

        /// <summary>
        /// Deletes the file with the name.
        /// </summary>
        /// <param name="filename">File that will be removed.</param>
        public void DeleteFile(string filename)
        {
            GetFile(filename).Delete();
            Refresh();
        }

        /// <summary>
        /// Deletes this directory and all the contents of it.
        /// </summary>
        public void DeleteDirectory()
        {
            Refresh();

            foreach (var file in Files)
            {
                DeleteFile(file.FileInfo.FullName);
            }

            foreach (var subDirectory in Directories)
            {
                subDirectory.DeleteDirectory();
            }

            DirectoryInfo.Delete();
        }

        /// <summary>
        /// Creates a file to this directory
        /// </summary>
        /// <param name="filename"></param>
        public void AddFile(string filename)
        {
            StreamWriter writer = new StreamWriter($"{DirectoryInfo.FullName}/{filename}");
            writer.Close();
            // Refreshing is important! Without it the new file won't be found!
            Refresh();
        }

        /// <summary>
        /// Creates a subdirectory to this directory
        /// </summary>
        /// <param name="name"></param>
        public void CreateSubdirectory(string name)
        {
            DirectoryInfo.CreateSubdirectory(name);
        }
    }
}
