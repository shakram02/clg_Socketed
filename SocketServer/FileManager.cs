using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace SocketServer
{
    /// <summary>Handles operations with hard disk</summary>
    internal static class FileManager
    {
        private const string WritingDirectory = @"D:\server";

        static FileManager()
        {
            if (!Directory.Exists(WritingDirectory))
                Directory.CreateDirectory(WritingDirectory);
        }

        /// <summary>Writes text or image files to hard disk</summary>
        /// <param name="filesPosted"></param>
        /// <returns></returns>
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
                            // Extra: if the file already exists append the date to its name
                            image.Save(Path.Combine(WritingDirectory, postedFile.FileName), ImageFormat.Jpeg);  // Or Png
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
                        File.WriteAllBytes(Path.Combine(WritingDirectory, postedFile.FileName), postedFile.FileContent);
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