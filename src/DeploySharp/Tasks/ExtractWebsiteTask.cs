using DeploySharp.Core;

using DeploySharp;

using NDepend.Helpers.FileDirectoryPath;

namespace DeploySharp.Tasks
{
	public class ExtractWebsiteTask : IExecutableWithContext
	{
		public ExtractWebsiteTask(IExtractZip extractZip)
		{
			_extractZip = extractZip;
		}

		public void Execute(TaskContext context)
		{
			var newAppDirectory = GetAppDirectory (context.Target.SiteRoot, context.Build).Path;
			_extractZip.ExtractAllFiles (WEBSITE_FILENAME, newAppDirectory);
		}

		public static DirectoryPathAbsolute GetAppDirectory(DirectoryPathAbsolute targetSiteRoot, BuildInfo build)
		{
			return new DirectoryPathAbsolute (targetSiteRoot.Path + "-" + build.ToIdString ());
		}

		private readonly IExtractZip _extractZip;
		private const string WEBSITE_FILENAME = "website.zip";
	}
}
