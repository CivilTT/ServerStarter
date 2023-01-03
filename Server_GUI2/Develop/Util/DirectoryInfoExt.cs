using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_GUI2.Develop.Util
{
    public static class DirectoryInfoExt
    {
        public static void CopyTo(this DirectoryInfo dir, string destinationDir, bool recursive = true, bool notExistsOK = true)
        {
            dir.Refresh();

            // Check if the source directory exists
            if (!dir.Exists)
                if (notExistsOK)
                    return;
                else
                    throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");
            // Cache directories before we start copying
            
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Create the destination directory
            var d = Directory.CreateDirectory(destinationDir);
            while (!d.Exists)
            {
                d.Refresh();
            }

            // Get the files in the source directory and copy to the destination directory
            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                var f = file.CopyTo(targetFilePath);
                while (!f.Exists)
                {
                    f.Refresh();
                }
            }

            // If recursive and copying subdirectories, recursively call this method
            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                    CopyTo(subDir, newDestinationDir, true);
                }
            }
        }
    }
}
