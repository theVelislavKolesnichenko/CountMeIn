using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utils
{
    public static class Files
    {
        public static string UploadDirectoryPath { get { return "C:\\"; } }

        internal static void MakeDirectoryIfNotExists(string fullFileName)
        {
            throw new NotImplementedException();
        }

        public static string uploadFileDirectory { get; set; }
    }
}
