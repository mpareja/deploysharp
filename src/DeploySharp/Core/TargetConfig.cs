using NDepend.Helpers.FileDirectoryPath;

namespace DeploySharp.Core
{
	public class TargetConfig
	{
		public TargetConfig(string siteName, DirectoryPathAbsolute siteRoot)
		{
			SiteName = siteName;
			SiteRoot = siteRoot;
		}

		public string SiteName { get; private set; }
		public DirectoryPathAbsolute SiteRoot { get; private set; }
	}
}
