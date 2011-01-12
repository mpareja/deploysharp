using System.IO;
using System.Linq;
using System.Security.Cryptography;

using DeploySharp.Core;

namespace DeploySharp.Tasks.FileSystem
{
	public class CopyFileTask : IExecutable, IPreparable
	{
		public string Source { get; set; }
		public string Destination { get; set; }

		public DestinationExistsAction OnDestinationExistsWithDiffContent { get; set; }

		private bool _exactFileExists;

		public TaskResult Prepare()
		{
			var result = new TaskResult ();
			if (File.Exists (Source) == false)
			{
				result.Error ("Source file does not exist!");
				return result;
			}

			var dest = new FileInfo (Destination);
			if (dest.Exists)
			{
				var sourceMd5 = GetMd5 (Source);
				var destMd5 = GetMd5 (Destination);

				_exactFileExists = sourceMd5.SequenceEqual (destMd5);
			}
			else
			{
				_exactFileExists = false;
			}

			if (_exactFileExists)
				return result;

            DetectAndAppendCopyErrorsToResult (result, dest);
			return result;
		}

		public TaskResult Execute()
		{
			var result = new TaskResult();
			result.Success ("Copying {0} to {1}. ", Source, Destination);
			if (_exactFileExists)
			{
				result.Success (Destination + " already exists with matching file contents.");
				return result;
			}

			var destInfo = new FileInfo (Destination);
			DetectAndAppendCopyErrorsToResult (result, destInfo);
			if (result.ContainsError())
				return result;

			if (destInfo.Exists && destInfo.IsReadOnly)
			{
				destInfo.IsReadOnly = false;
				File.Copy (Source, Destination, true);
				destInfo.IsReadOnly = true;
			}
			else
			{
				File.Copy (Source, Destination, true);
			}
			result.Success ("{0} successfully copied to {1}.", Source, Destination);
			return result;
		}

		private void DetectAndAppendCopyErrorsToResult(TaskResult result, FileInfo destInfo)
		{
			switch (OnDestinationExistsWithDiffContent)
			{
				case DestinationExistsAction.Fail:
					if (destInfo.Exists)
						result.Error (Destination + " already exists!");
					break;
				
				case DestinationExistsAction.Overwrite:
					if (destInfo.Exists && destInfo.IsReadOnly)
						result.Error("{0} exists and is readonly.", Destination);
					break;
				
				case DestinationExistsAction.OverwriteEvenReadOnly:
					break;

				default:
					result.Error ("Unexpected destination action: " +
					              OnDestinationExistsWithDiffContent);
					break;
			}
		}

		private byte[] GetMd5 (string filepath)
		{
			var provider = new MD5CryptoServiceProvider ();
			using (var stream = File.OpenRead (filepath))
			{
				return provider.ComputeHash (stream);
			}
		}
	}
}
