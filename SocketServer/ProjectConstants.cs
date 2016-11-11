using System;
using System.IO;

namespace SocketServer
{
    static class ProjectConstants
    {
        public static string SolutionDirectory { get; }
        public const string UploadDirectory = "clientUploads";
        public const string RootDirectory = "www";
        static ProjectConstants()
        {
            SolutionDirectory = Path.GetDirectoryName(Path.GetDirectoryName(Environment.CurrentDirectory));

        }
    }
}
