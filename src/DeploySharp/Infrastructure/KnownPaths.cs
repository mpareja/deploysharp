using NDepend.Helpers.FileDirectoryPath;

namespace DeploySharp.Infrastructure
{
	public class KnownPaths
	{
		public static DirectoryPathAbsolute ApplicationPath = 
			new FilePathAbsolute (typeof(KnownPaths).Assembly.Location).ParentDirectoryPath;
	}
}
