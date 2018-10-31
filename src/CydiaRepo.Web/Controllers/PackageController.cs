using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ApplicationInsights;

namespace CydiaRepo.Web.Controllers
{
    public class PackageController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly TelemetryClient _telemetryClient;

        public PackageController(IHostingEnvironment hostingEnvironment, TelemetryClient telemetryClient)
        {
            _hostingEnvironment = hostingEnvironment;
            _telemetryClient = telemetryClient;
        }

        [HttpGet("{debFile}")]
        [Route("/package/{debFile}")]
        public IActionResult Index(string debFile)
        {
            var debPath = GetSafePath(debFile);
            if (!System.IO.File.Exists(debPath)) return NotFound();
            TrackDebDownload(debFile);
            var debStream = System.IO.File.OpenRead(debPath);
            return File(debStream, "application/x-debian-package");
        }

        private string GetSafePath(string debFile)
        {
            var debFolder = $@"{_hostingEnvironment.WebRootPath}\deb\";
            var safePath = Path.GetFullPath(Path.Combine(debFolder, debFile));
            if (safePath.StartsWith(debFolder))
            {
                return safePath;
            }

            return null;
        }

        private void TrackDebDownload(string fileName)
        {
            var fileParts = fileName.Split("_");

            var properties = new Dictionary<string, string>
            {
                { "FileName", fileName }
            };

            if (fileParts.Length > 0)
            {
                properties.Add("Tweak", fileParts[0]);
            }

            if (fileParts.Length > 1)
            {
                properties.Add("Version", fileParts[1]);
            }

            _telemetryClient.TrackEvent("DebDownload", properties);
        }
    }
}