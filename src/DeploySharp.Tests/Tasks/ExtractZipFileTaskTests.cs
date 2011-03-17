using System.IO;
using DeploySharp.Tasks;
using NUnit.Framework;
using Tests;

namespace DeploySharp.Tests.Tasks
{ 
	[TestFixture]
	public class ExtractZipFileTaskTests
	{
		[Test]
		public void can_extract_file()
		{
			using (var tempdir = new TempDirectoryPathAbsolute())
			{
				var task = new ExtractZipFileTask {
					SourceZip = "Resources/TestFile.zip",
					DestinationDir = tempdir.Path
				};
				task.PrepareAndAssertNoErrors ();
				task.ExecuteAndAssertNoErrors ();

				var expected = Path.Combine(tempdir.Path, "TestFile.txt");
				Assert.True(File.Exists(expected));
			}
		}
	}
}