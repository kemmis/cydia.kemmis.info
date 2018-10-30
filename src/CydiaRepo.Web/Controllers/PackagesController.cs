using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using cydia_repo.services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace CydiaRepo.Web.Controllers
{
    public class PackagesController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private string DebFolderPath => $@"{_hostingEnvironment.WebRootPath}\deb\";

        public PackagesController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        [ResponseCache(Duration = 600)]
        [Route("/packages")]
        public async Task<IActionResult> Index()
        {
            var ms = new MemoryStream();
            DateTime lastModified;

            using (var writer = new DebPackageControlWriter(ms))
            {
                lastModified = await writer.WriteControlsForDebFiles(DebFolderPath);
            }

            var bytes = ms.ToArray();
            var currentETag = GetETag(lastModified, bytes);
            if (IfMatchGivenIfNoneMatch(currentETag))
            {
                return StatusCode((int)HttpStatusCode.NotModified);
            }

            return File(bytes, "application/octet-stream", new DateTimeOffset(lastModified), currentETag);
        }

        [ResponseCache(Duration = 600)]
        [Route("Packages.bz2")]
        public async Task<IActionResult> PackagesArchive()
        {
            var ms = new MemoryStream();
            DateTime lastModified;

            using (var writer = new DebPackageControlCompressedWriter(ms))
            {
                lastModified = await writer.WritePackagesArchive(DebFolderPath);
            }

            var bytes = ms.ToArray();
            var currentETag = GetETag(lastModified, bytes);
            if (IfMatchGivenIfNoneMatch(currentETag))
            {
                return StatusCode((int)HttpStatusCode.NotModified);
            }

            return File(bytes, "application/octet-stream", new DateTimeOffset(lastModified), currentETag);
        }

        private EntityTagHeaderValue GetETag(DateTime lastModified, Byte[] data)
        {
            var etagHash = lastModified.ToFileTimeUtc() ^ data.Length;
            return new EntityTagHeaderValue('\"' + Convert.ToString(etagHash, 16) + '\"');
        }

        private bool IfMatchGivenIfNoneMatch(EntityTagHeaderValue currentETag)
        {
            var requestHeaders = Request.GetTypedHeaders();
            return requestHeaders.IfNoneMatch != null &&
                   requestHeaders.IfNoneMatch.Contains(currentETag);
        }
    }
}