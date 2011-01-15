using System;

using NDepend.Helpers.FileDirectoryPath;

namespace DeploySharp.Infrastructure
{
	public static class FileDirectoryPathHelpers
	{
		/// <summary>
		/// This method assumes that relative paths are relative to
		/// the environment's current directory.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static FilePathAbsolute MakeAbsoluteAssumeCurrentDir(this FilePath path)
		{
			FilePathAbsolute absolute;
			var currentDir = new DirectoryPathAbsolute (Environment.CurrentDirectory);
			if (path.IsAbsolutePath)
				absolute = (FilePathAbsolute) path;
			else
				absolute = ((FilePathRelative) path).GetAbsolutePathFrom (currentDir);

			return absolute;
		}

		/// <summary>
		/// This method assumes that relative paths are relative to
		/// the environment's current directory.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static DirectoryPathAbsolute MakeAbsoluteAssumeCurrentDir(this DirectoryPath path)
		{
			DirectoryPathAbsolute absolute;
			var currentDir = new DirectoryPathAbsolute (Environment.CurrentDirectory);
			if (path.IsAbsolutePath)
				absolute = (DirectoryPathAbsolute) path;
			else
				absolute = ((DirectoryPathRelative) path).GetAbsolutePathFrom (currentDir);

			return absolute;
		}
	}
}