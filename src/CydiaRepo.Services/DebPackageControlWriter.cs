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
        private readonly StreamWriter _streamWriter;
        public DebPackageControlWriter(Stream stream)
        {
            _streamWriter = new StreamWriter(stream);
            _streamWriter.NewLine = "\n";
        }

        public async Task WriteControlsForDebFiles(string debFileDirectory)
        {
            var dirInfo = new DirectoryInfo(debFileDirectory);
            foreach (var fileInfo in dirInfo.GetFiles("*.deb"))
            {
                using (var debFileStream = File.OpenRead(fileInfo.FullName))
                {
                    var debPackage = await DebPackageReader.Read(debFileStream);
                    await WriteControlFileForDebPackage(debPackage);
                }

                await WriteFileNameForDebPackage(fileInfo);
                await WriteFileSizeForDebPackage(fileInfo);
                await WriteHashValuesForDebPackage(fileInfo.FullName);
                await _streamWriter.WriteLineAsync(); // adds empty line after each control file

            }
        }

        private async Task WriteControlFileForDebPackage(DebPackage debPackage)
        {
            var controlFile = debPackage.ControlFile;
            foreach (var controlFileKey in controlFile.Keys)
            {
                await _streamWriter.WriteLineAsync($"{controlFileKey}: {controlFile[controlFileKey]}");
            }
        }

        private async Task WriteHashValuesForDebPackage(string debPackagepath)
        {
            var hashingService = new PackageHashingService(debPackagepath);
            await _streamWriter.WriteLineAsync($"MD5sum: {hashingService.Md5}");
            await _streamWriter.WriteLineAsync($"SHA1: {hashingService.Sha1}");
            await _streamWriter.WriteLineAsync($"SHA256: {hashingService.Sha256}");
        }

        private async Task WriteFileNameForDebPackage(FileInfo fileInfo)
        {
            var relativePath = $"Filename: ./package/{fileInfo.Name}";
            await _streamWriter.WriteLineAsync(relativePath);
        }

        private async Task WriteFileSizeForDebPackage(FileInfo fileInfo)
        {
            await _streamWriter.WriteLineAsync($"Size: {fileInfo.Length}");
        }

        public void Dispose()
        {
            _streamWriter?.Dispose();
        }
    }
}
