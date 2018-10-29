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

        //public async Task Index()
        //{
        //    using (var writer = new DebPackageControlWriter(Response.Body))
        //    {
        //        await writer.WriteControlsForDebFiles(DebFolderPath);
        //    }
        //}

        [Route("Packages.bz2")]
        public async Task PackagesArchive()
        {
            using (var writer = new DebPackageControlCompressedWriter(Response.Body))
            {
                Response.ContentType = "application/octet-stream";
                await writer.WritePackagesArchive(DebFolderPath);
            }
        }
    }
}