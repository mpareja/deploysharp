using DeploySharp.Infrastructure;

using NDepend.Helpers.FileDirectoryPath;

namespace DeploySharp.Core
{
	public class ConfigurationRepository
	{
		public virtual void SaveLmsWebConfigFile(string siteName, DirectoryPathAbsolute destination)
		{
			SaveWebConfigFile (siteName, @"LMS", destination);
		}

		public virtual void SaveAeWebConfigFile(string siteName, DirectoryPathAbsolute destination)
		{
			SaveWebConfigFile(siteName, @"AE", destination);
		}

		private void SaveWebConfigFile(string siteName, string application, DirectoryPathAbsolute destination) 
		{
			var webConfigRelative = new FilePathRelative (
				@".\Configuration\Web Configuration Files\" + siteName + @"\" + application + @"\Web.config");

			var webConfig = webConfigRelative.GetAbsolutePathFrom (KnownPaths.ApplicationPath);
			webConfig.FileInfo.CopyTo (destination.GetChildFileWithName ("Web.config").Path, true);
		}
	}
}