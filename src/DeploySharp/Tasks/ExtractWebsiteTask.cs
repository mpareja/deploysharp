using DeploySharp.Core;

using NDepend.Helpers.FileDirectoryPath;

namespace DeploySharp.Tasks
{
	public class ExtractWebsiteTask : IExecutable
	{
		public ExtractWebsiteTask(IExtractZip extractZip)
		{
			_extractZip = extractZip;
		}

		public TargetConfig Target { get; set; }

		public BuildInfo Build { get; set; }

		public TaskResult Execute()
		{
			var newAppDirectory = GetAppDirectory (Target.SiteRoot, Build).Path;
			_extractZip.ExtractAllFiles (WEBSITE_FILENAME, newAppDirectory);

			var result = new TaskResult();
			result.Success ("Testing result.");
			return result;
		}

		public static DirectoryPathAbsolute GetAppDirectory(DirectoryPathAbsolute targetSiteRoot, BuildInfo build)
		{
			return new DirectoryPathAbsolute (targetSiteRoot.Path + "-" + build.ToIdString ());
		}

		private readonly IExtractZip _extractZip;
		private const string WEBSITE_FILENAME = "website.zip";
	}
}
