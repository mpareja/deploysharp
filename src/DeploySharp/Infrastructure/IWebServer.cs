using NDepend.Helpers.FileDirectoryPath;

namespace DeploySharp.Infrastructure
{
	public interface IWebServer
	{
		DirectoryPathAbsolute GetLmsDirFor (string siteName);

		DirectoryPathAbsolute GetAeDirFor (string siteName);
	}
}