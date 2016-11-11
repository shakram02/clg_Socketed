using System.Collections.Generic;
using System.IO;

namespace SocketServer
{
    /// <summary>Handles operations with hard disk</summary>
    internal static class FileManager
    {
        private static readonly string writingDirectory = ProjectConstants.SolutionDirectory;
        private static readonly string fileUploadDirectory;
        static FileManager()
        {
            fileUploadDirectory = Path.Combine(ProjectConstants.SolutionDirectory, ProjectConstants.UploadDirectory);
            if (!Directory.Exists(fileUploadDirectory))
                Directory.CreateDirectory(fileUploadDirectory);
        }

        /// <summary>Writes text or image files to hard disk</summary>
        /// <param name="filesPosted"></param>
        /// <returns></returns>
        public static List<bool> WriteFilesToDisk(List<HttpFile> filesPosted)
        {
            List<bool> checks = new List<bool>();
            foreach (HttpFile postedFile in filesPosted)
            {
                string dir = Path.Combine(fileUploadDirectory, postedFile.FileName);
                File.WriteAllBytes(dir, postedFile.FileContent);
            }
            return checks;
        }
    }
}