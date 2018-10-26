using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;

namespace cydia_repo.services
{
    public class DebPackageControlCompressedWriter : IDisposable
    {
        private readonly DebPackageControlWriter _debPackageControlWriter;
        private readonly GZipStream _gZipStream;

        public DebPackageControlCompressedWriter(Stream stream)
        {
            _gZipStream = new GZipStream(stream, CompressionLevel.Optimal);
            _debPackageControlWriter = new DebPackageControlWriter(_gZipStream);
        }

        public async Task WritePackagesArchive(string debFileDirectory, string relativeToPath)
        {
            await _debPackageControlWriter.WriteControlsForDebFiles(debFileDirectory, relativeToPath);
        }

        public void Dispose()
        {
            _debPackageControlWriter?.Dispose();
            _gZipStream?.Dispose();
        }
    }
}
