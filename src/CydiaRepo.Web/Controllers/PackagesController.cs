﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using cydia_repo.services;
using Microsoft.AspNetCore.Hosting;
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

        [Route("/packages")]
        public async Task<IActionResult> Index()
        {
            string requestETag = "";
            if (Request.Headers.ContainsKey("If-None-Match"))
            {
                requestETag = Request.Headers["If-None-Match"].First();
            }

            var ms = new MemoryStream();
            DateTime lastModified;

            using (var writer = new DebPackageControlWriter(ms))
            {
                lastModified = await writer.WriteControlsForDebFiles(DebFolderPath);
            }

            string responseETag = Convert.ToBase64String(BitConverter.GetBytes(lastModified.Ticks));

            if (Request.Headers.ContainsKey("If-None-Match") && responseETag == requestETag)
            {
                return StatusCode((int)HttpStatusCode.NotModified);
            }

            var bytes = ms.ToArray();
            return File(bytes, "application/octet-stream", new DateTimeOffset(lastModified), new EntityTagHeaderValue(responseETag));
        }

        [Route("Packages.bz2")]
        public async Task<IActionResult> PackagesArchive()
        {
            // Get the requested ETag
            string requestETag = "";
            if (Request.Headers.ContainsKey("If-None-Match"))
            {
                requestETag = Request.Headers["If-None-Match"].First();
            }

            var ms = new MemoryStream();
            DateTime lastModified;

            using (var writer = new DebPackageControlCompressedWriter(ms))
            {
                lastModified = await writer.WritePackagesArchive(DebFolderPath);
            }

            string responseETag = Convert.ToBase64String(BitConverter.GetBytes(lastModified.Ticks));

            if (Request.Headers.ContainsKey("If-None-Match") && responseETag == requestETag)
            {
                return StatusCode((int)HttpStatusCode.NotModified);
            }

            var bytes = ms.ToArray();
            return File(bytes, "application/octet-stream", new DateTimeOffset(lastModified), new EntityTagHeaderValue(responseETag));
        }
    }
}