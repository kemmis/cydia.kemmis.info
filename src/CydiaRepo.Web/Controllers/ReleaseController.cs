using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace CydiaRepo.Web.Controllers
{
    public class RepoInfoController : Controller
    {
        [Route("/Release")]
        public string Index()
        {
            var sb = new StringBuilder();

            sb.Append("Origin: cydia.kemmis.info\n");
            sb.Append("Label: cydia.kemmis.info\n");
            sb.Append("Suite: stable\n");
            sb.Append("Version: 1.2\n");
            sb.Append("Codename: ios\n");
            sb.Append("Architectures: iphoneos-arm\n");
            sb.Append("Components: main\n");
            sb.Append("Description: cydia.kemmis.info\n");

            return sb.ToString();
        }
    }
}
