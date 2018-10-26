using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace cydia_repo.services
{
    public class PackageHashingService
    {
        private readonly string _packagePath;
        private byte[] _packageBytes;

        public PackageHashingService(string packagePath)
        {
            _packagePath = packagePath;
        }

        private byte[] PackageBytes
        {
            get
            {
                if (_packageBytes == null)
                {
                    _packageBytes = File.ReadAllBytes(_packagePath);
                }

                return _packageBytes;
            }
        }

        public string Md5
        {
            get
            {
                return Hash(bytes =>
                {
                    using (var md5 = MD5.Create())
                    {
                        return md5.ComputeHash(bytes);
                    }
                });
            }
        }

        public string Sha1
        {
            get
            {
                return Hash(bytes =>
                {
                    using (var sha1 = SHA1.Create())
                    {
                        return sha1.ComputeHash(bytes);
                    }
                });

            }
        }

        public string Sha256
        {
            get
            {
                return Hash(bytes =>
                {
                    using (var sha56 = SHA256.Create())
                    {
                        return sha56.ComputeHash(bytes);
                    }
                });
            }
        }

        private string Hash(Func<byte[], byte[]> hashFunc)
        {
            var bytes = hashFunc.Invoke(PackageBytes);
            var sb = new StringBuilder();
            foreach (var b in bytes)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
    }
}
