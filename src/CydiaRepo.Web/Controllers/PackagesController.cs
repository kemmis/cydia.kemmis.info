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
        private string RelativeToPath => $@"{_hostingEnvironment.WebRootPath}\";

        public PackagesController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task Index()
        {
            using (var writer = new DebPackageControlWriter(Response.Body))
            {
                await writer.WriteControlsForDebFiles(DebFolderPath, RelativeToPath);
            }
        }

        [Route("Packages.bz2")]
        public async Task PackagesArchive()
        {
            using (var writer = new DebPackageControlCompressedWriter(Response.Body))
            {
                await writer.WritePackagesArchive(DebFolderPath, RelativeToPath);
                Response.ContentType = "application/octet-stream";
            }
        }
    }
}