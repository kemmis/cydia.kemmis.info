using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using cydia_repo.services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

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

        [Route("/packages")]
        public async Task Index()
        {
            using (var writer = new DebPackageControlWriter(Response.Body))
            {
                await writer.WriteControlsForDebFiles(DebFolderPath);
            }
        }

        [Route("Packages.bz2")]
        public async Task<IActionResult> PackagesArchive()
        {
            var ms = new MemoryStream();

            using (var writer = new DebPackageControlCompressedWriter(ms))
            {
                await writer.WritePackagesArchive(DebFolderPath);
            }

            var bytes = ms.ToArray();
            return File(bytes, "application/octet-stream");
        }
    }
}