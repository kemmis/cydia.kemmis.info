using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using Noemax.Compression;

namespace cydia_repo.services
{
    public class DebPackageControlCompressedWriter : IDisposable
    {
        private readonly DebPackageControlWriter _debPackageControlWriter;
        private readonly CompressionStream _compressionStream;

        public DebPackageControlCompressedWriter(Stream stream)
        {
            _compressionStream = CompressionFactory.BZip2.CreateOutputStream(stream, -9, true);
            _debPackageControlWriter = new DebPackageControlWriter(_compressionStream);
        }

        public async Task WritePackagesArchive(string debFileDirectory)
        {
            await _debPackageControlWriter.WriteControlsForDebFiles(debFileDirectory);
        }

        public void Dispose()
        {
            _debPackageControlWriter?.Dispose();
            _compressionStream?.Dispose();
        }
    }
}
