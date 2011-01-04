//using System.Linq;

//using Microsoft.Web.Administration;

//using NDepend.Helpers.FileDirectoryPath;

//namespace DeploySharp.Infrastructure
//{
//    public class IisExpressWebServer : IWebServer
//    {
//        public DirectoryPathAbsolute GetLmsDirFor(string siteName)
//        {
//            return GetDirFor(siteName, "/lms");
//        }

//        public DirectoryPathAbsolute GetAeDirFor(string siteName)  
//        {
//            return GetDirFor (siteName, "/ae/candidate");
//        }

//        private DirectoryPathAbsolute GetDirFor(string siteName, string appPath)
//        {
//            var dir = (from site in ServerManager.Sites
//                       where site.Name == siteName
//                       from app in site.Applications
//                       where app.Path == appPath
//                       from vdir in app.VirtualDirectories
//                       where vdir.Path == "/"
//                       select vdir.PhysicalPath).SingleOrDefault ();

//            if (dir == null)
//                return DirectoryPathAbsolute.Empty;
			
//            return new DirectoryPathAbsolute (dir);
//        }

//        private ServerManager ServerManager
//        {
//            get
//            {
//                if (_serverManager == null)
//                    _serverManager = new ServerManager (@"D:\projects\deploysharp\applicationhost.config");
//                return _serverManager;
//            }
//        }
//        private ServerManager _serverManager;
//    }
//}