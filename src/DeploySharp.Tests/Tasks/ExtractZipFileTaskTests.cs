using System.IO;
using DeploySharp.Tasks;
using NUnit.Framework;

namespace Tests.Tasks
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
				var result = task.Execute();

				Assert.NotNull(result);
				Assert.False(result.ContainsError());

				var expected = Path.Combine(tempdir.Path, "TestFile.txt");
				Assert.True(File.Exists(expected));
			}
		}
	}
}