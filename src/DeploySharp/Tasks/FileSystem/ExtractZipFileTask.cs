using System.IO;
using DeploySharp.Core;
using Ionic.Zip;

namespace DeploySharp.Tasks
{
	public class ExtractZipFileTask : IExecutable, IPreparable
	{
		public string SourceZip { get; set; }
		public string DestinationDir { get; set; }

		TaskResult IPreparable.Prepare()
		{
			var result = new TaskResult ();
			if (File.Exists (SourceZip) == false)
				result.Error ("Zip file not found: " + SourceZip);

			return result;
		}

		TaskResult IExecutable.Execute()
		{
			using (var zip = ZipFile.Read(SourceZip))
			{
				zip.ExtractAll(DestinationDir);
			}
			var result = new TaskResult ();
			result.Success ("{0} extracted to {1}.", SourceZip, DestinationDir);
			return result;
		}
	}
}