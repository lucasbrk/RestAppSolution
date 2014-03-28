using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

namespace RestApp.Common.Utility
{
    public class FileUtility
    {
        public static string GetFileExtension(string fileName)
        {
            int LastPointIntString = fileName.LastIndexOf('.');
            return fileName.Substring(LastPointIntString);
        }

        public static string GetFolderFile(string fileURL)
        {
            int LastMediaIntString = fileURL.LastIndexOf("Media");
            return fileURL.Substring(LastMediaIntString).Replace('\\', '/');
        }

        public static byte[] FileToArray(Stream inputStream)
        {
            MemoryStream ms = new MemoryStream();
            inputStream.CopyTo(ms);       
            return ms.ToArray();
        }
    }
}
