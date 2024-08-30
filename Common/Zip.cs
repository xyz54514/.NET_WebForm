using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class Zip
    {

        public static byte[] Archive(byte[] fileBytes, string fileName)
        {
            var fileStream = new MemoryStream();
            using (var archive = new ZipArchive(fileStream, ZipArchiveMode.Create, true))
            {

                var zipArchiveEntry = archive.CreateEntry(fileName, CompressionLevel.Fastest);
                using (var zipStream = zipArchiveEntry.Open())
                {
                    zipStream.Write(fileBytes, 0, fileBytes.Length);
                }
            }

            return fileStream.ToArray();
        }
    }
}
