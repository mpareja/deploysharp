using System;
using System.IO;

using NDepend.Helpers.FileDirectoryPath;

using NUnit.Framework;

using Shouldly;

namespace Tests
{
	public class TempDirectoryPathAbsolute :IDisposable
	{
		public TempDirectoryPathAbsolute()
		{
			var guid = Guid.NewGuid();

			_path = _tempPathRoot.GetChildDirectoryWithName (guid.ToString());

			Directory.CreateDirectory (_path.Path);
		}

		/// <summary>
		/// Creates a temporary file with the specified contents.
		/// </summary>
		/// <param name="contents"></param>
		/// <returns>The filename of the temporary file.</returns>
		public string SaveToTempFile(string contents)
		{
			var filename = System.IO.Path.Combine (Path, Guid.NewGuid().ToString());
			File.WriteAllText (filename, contents);
			return filename;
		}

		public string Path { get { return _path.Path; } }

		public DirectoryPathAbsolute AsDir { get { return _path; } }

		public void Dispose()
		{
			Directory.Delete (_path.Path, true);

			// deletes root if empty (exception if an earlier test failed to clean it out)
			try
			{
				Directory.Delete (_tempPathRoot.Path, false);
			}
			catch (IOException)
			{
				// now make sure to cleanup so all other tests don't fail
				Directory.Delete (_tempPathRoot.Path, true);
				Assert.Fail ("An earlier test forgot to clean junk out of the temp directory: "
				             + _tempPathRoot.Path);
			}
		}
		private DirectoryPathAbsolute _path;
		private readonly DirectoryPathAbsolute _tempPathRoot = new DirectoryPathAbsolute (
			Environment.GetEnvironmentVariable ("temp") + "/DeliveryConsole");
	}

	
	[TestFixture]
	public class Test_TempDirectoryPathAbsolute
	{
		[Test()]
		public void A_test_that_doesnt_clean_up_temp_dir()
		{
			var tempDir = new TempDirectoryPathAbsolute();
		}

		[Test]
		public void B_test_that_does_cleanup_temp_dir_but_fails_because_last_test()
		{
			var tempDir = new TempDirectoryPathAbsolute();

			Assert.Throws<AssertionException> (tempDir.Dispose);
		}
	}
}