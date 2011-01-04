namespace DeploySharp
{
	public interface IExtractZip
	{
		void ExtractAllFiles (string zipfile, string destinationDirectory);
	}
}