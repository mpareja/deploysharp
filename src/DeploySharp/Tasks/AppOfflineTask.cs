using DeploySharp.Core;
using DeploySharp.Infrastructure;

using NDepend.Helpers.FileDirectoryPath;

namespace DeploySharp.Tasks
{
	public class AppOfflineTask : IExecutable
	{
		public AppOfflineTask(IWebServer webServer, IAssemblyResourceManager assemblyResourceManager)
		{
			_webServer = webServer;
			_assemblyResourceManager = assemblyResourceManager;
		}

		public void Execute(TaskContext context)
		{
			ApplyAppOffline (_webServer.GetLmsDirFor (context.Target.SiteName));
			ApplyAppOffline (_webServer.GetAeDirFor (context.Target.SiteName));
		}

		private void ApplyAppOffline(DirectoryPathAbsolute sitePath)
		{
			if (sitePath != DirectoryPathAbsolute.Empty)
			{
				var targetPath = APP_OFFLINE_FILE.GetAbsolutePathFrom (sitePath).Path;
				_assemblyResourceManager.SaveToDisk (APP_OFFLINE_FILE.FileName, targetPath);
			}
		}

		private IWebServer _webServer;
		private FilePathRelative APP_OFFLINE_FILE = new FilePathRelative (@".\App_Offline.htm");
		private IAssemblyResourceManager _assemblyResourceManager;
	}
}
