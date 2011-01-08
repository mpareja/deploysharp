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
			var newAppDirectory = GetAppDirectory (context).Path;
			_extractZip.ExtractAllFiles (WEBSITE_FILENAME, newAppDirectory);
		}

		public static DirectoryPathAbsolute GetAppDirectory(TaskContext context)
		{
			return new DirectoryPathAbsolute (context.Target.SiteRoot.Path
											  + "-" + context.Build.ToIdString ());
		}

		private readonly IExtractZip _extractZip;
		private const string WEBSITE_FILENAME = "website.zip";
	}
}
