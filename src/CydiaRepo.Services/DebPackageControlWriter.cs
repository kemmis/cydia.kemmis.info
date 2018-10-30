using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Packaging.Targets.Deb;

namespace cydia_repo.services
{
    public class DebPackageControlWriter : IDisposable
    {
        private static readonly string[] _controlEntries = new[]
        {
            "Package",
            "Version",
            "Architecture",
            "Maintainer",
            "Installed-Size",
            "Depends",
            "Filename",
            "Size",
            "MD5sum",
            "SHA1",
            "SHA256",
            "Section",
            "Description",
            "Author",
            "Name"
        };
        private readonly StreamWriter _streamWriter;
        public DebPackageControlWriter(Stream stream)
        {
            _streamWriter = new StreamWriter(stream);
            _streamWriter.NewLine = "\n";
        }

        public async Task<DateTime> WriteControlsForDebFiles(string debFileDirectory)
        {
            DateTime? lastModified = null;

            var dirInfo = new DirectoryInfo(debFileDirectory);
            foreach (var fileInfo in dirInfo.GetFiles("*.deb"))
            {
                Dictionary<string, string> controlEntries;

                using (var debFileStream = File.OpenRead(fileInfo.FullName))
                {
                    var debPackage = await DebPackageReader.Read(debFileStream);
                    controlEntries = debPackage.ControlFile;
                }

                controlEntries.Add("FileName", fileInfo.Name);
                controlEntries.Add("Size", fileInfo.Length.ToString());
                var hashingService = new PackageHashingService(fileInfo.FullName);
                controlEntries.Add("MD5sum", hashingService.Md5);
                controlEntries.Add("SHA1", hashingService.Sha1);
                controlEntries.Add("SHA256", hashingService.Sha256);

                foreach (var controlEntry in _controlEntries)
                {
                    if (controlEntries.ContainsKey(controlEntry))
                    {
                        await _streamWriter.WriteLineAsync($"{controlEntry}: {controlEntries[controlEntry]}");
                    }
                }

                await _streamWriter.WriteLineAsync(); // adds empty line after each control file

                if (lastModified == null || lastModified < fileInfo.LastWriteTimeUtc)
                {
                    lastModified = fileInfo.LastWriteTimeUtc;
                }
            }

            return lastModified ?? DateTime.UtcNow;
        }

        private async Task WriteFileNameForDebPackage(FileInfo fileInfo)
        {
            var relativePath = $"Filename: ./package/{fileInfo.Name}";
            await _streamWriter.WriteLineAsync(relativePath);
        }

        public void Dispose()
        {
            _streamWriter?.Dispose();
        }
    }
}
