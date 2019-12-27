### What is this?
After I created my first iOS jb tweak, I scratched my head and at that point I realized that I had no idea where to host my tweaks. So I threw this together over the period of about a day or two, and it became the codebase for hosting my tweaks a cydia.kemmis.info.

But more importantly, I wrote it so that anyone can use it. The gist of it is that anyone can publish a copy of the ASP.NET Core web application, simply including the deb files they want to host. You place them in the wwwroot/deb/ folder before you publish the app. Everything else is done for you dynamically.

### Future Improvements
As it currently functions, the conventional URL endpoints that jb package managers look for are defined in the ASP.NET Core controllers/actions, directly inside the web application. I'd like to see those controllers and actions refactored out into a separate NetStandard library, so anyone can just reference that library in their own custom web app, and not have to customize the current web app in this project.
* Move conventional package manager endpoints out of the web application
* Convert the web application into an example to show others how to use the new NetStandard library in any ASP.NET Core web application.
* Publish the NetStandard library as a Nuget package.
* Profit?

### References
[How to Host a Cydia™ Repository (by Saurik)](http://www.saurik.com/id/7)