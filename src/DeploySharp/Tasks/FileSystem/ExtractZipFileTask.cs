using System;
using System.IO;
using DeploySharp.Core;
using Ionic.Zip;

namespace DeploySharp.Tasks
{
	public class ExtractZipFileTask : IExecutable, IPreparable
	{
		public string SourceZip { get; set; }
		public string DestinationDir { get; set; }
        public DestinationExistsAction OnDestinationExistsAction { get; set; }

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
				switch (OnDestinationExistsAction)
				{
					case DestinationExistsAction.Fail:
						zip.ExtractAll(DestinationDir);
						break;
					case DestinationExistsAction.Overwrite:
					case DestinationExistsAction.OverwriteEvenReadOnly:
						zip.ExtractAll(DestinationDir, ExtractExistingFileAction.OverwriteSilently);
						break;
					default:
						throw new ArgumentOutOfRangeException(OnDestinationExistsAction.ToString());
				}
			}
			var result = new TaskResult ();
			result.Success ("{0} extracted to {1}.", SourceZip, DestinationDir);
			return result;
		}
	}
}