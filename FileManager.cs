using System.IO;
using System.Collections.Generic;

namespace FileSystemManager
{
    public class FileManager
    {
        /// <summary>
        /// Loads the files from a directory to a ManagedDirectory.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ManagedDirectory LoadDirectory(string path) 
        {
            if (!Directory.Exists(path)) 
            {
                throw new DirectoryNotFoundException($"Unable to load files from '{path}'. That folder does not exist.");
            }

            List<ManagedFile> files = new List<ManagedFile>();

            foreach (string filename in Directory.EnumerateFiles(path))
            {
                ManagedFile managedFile = new ManagedFile(filename);
                files.Add(managedFile);
            }

            return new ManagedDirectory(path, files.ToArray());
        }

        /// <summary>
        /// Loads the file from the given location.
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static ManagedFile LoadFile(string filepath) 
        {
            if (!File.Exists(filepath)) 
            {
                throw new FileNotFoundException($"File '{filepath}' not found!");
            }
            else
            {
                return new ManagedFile(filepath);
            }
        }
    }
}
