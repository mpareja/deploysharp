namespace DeploySharp.Core
{
	public class BuildInfo
	{
		public BuildInfo(string release, string buildNumber)
		{
			Release = release;
			BuildNumber = buildNumber;
		}

		public string Release { get; private set; }

		public string BuildNumber { get; private set; }

		public bool IsEqual (BuildInfo info)
		{
			return this.Release == info.Release &&
			       this.BuildNumber == info.BuildNumber;
		}

		public string ToIdString()
		{
			return Release + "." + BuildNumber;
		}
	}
}