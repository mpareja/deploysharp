using System.IO;
using DeploySharp.Core;
using Ionic.Zip;

namespace DeploySharp.Tasks
{
	public class ExtractZipFileTask : IExecutable, IPreparable
	{
		public string SourceZip { get; set; }
		public string DestinationDir { get; set; }

		public TaskResult Prepare()
		{
			var result = new TaskResult ();
			if (File.Exists (SourceZip) == false)
				result.Error ("Zip file not found: " + SourceZip);

			return result;
		}

		public TaskResult Execute()
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