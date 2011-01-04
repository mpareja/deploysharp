using DeploySharp.Core;

namespace DeploySharp.Core
{
	public class TaskContext
	{
		public TargetConfig Target { get; set; }

		public BuildInfo Build { get; set; }
	}
}
