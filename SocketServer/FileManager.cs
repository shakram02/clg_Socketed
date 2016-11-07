using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace SocketServer
{
    /// <summary>
    /// Handles operations with hard disk
    /// </summary>
    internal static class FileManager
    {
        static FileManager()
        {
            WritingDirectory = @"D:\server";
            if (!Directory.Exists(WritingDirectory))
                Directory.CreateDirectory(WritingDirectory);
        }

        private static string WritingDirectory { get; set; }

        public static List<bool> WriteFilesToDisk(List<HttpFile> filesPosted)
        {
            List<bool> checks = new List<bool>();
            foreach (HttpFile postedFile in filesPosted)
            {
                if (Path.GetExtension(postedFile.FileExtension) != ".txt")
                {
                    byte[] bitmap = postedFile.FileContent;

                    using (Image image = Image.FromStream(new MemoryStream(bitmap)))
                    {
                        try
                        {
                            image.Save(Path.Combine(WritingDirectory, postedFile.FileName), ImageFormat.Png);  // Or Png
                            checks.Add(true);
                        }
                        catch (Exception)
                        {
                            checks.Add(false);
                        }
                    }
                }
                else
                {
                    try
                    {
                        File.WriteAllBytes(postedFile.FileName, postedFile.FileContent);
                        checks.Add(true);
                    }
                    catch (Exception)
                    {
                        checks.Add(false);
                    }
                }
            }
            return checks;
        }
    }
}