using System.Web.Services.Protocols;

using DeploySharp.Core;

namespace DeploySharp.Tasks.Ssrs
{
	public class VerifySsrsConnectionTask : SsrsTask, IPreparable, IExecutable
	{
		TaskResult IPreparable.Prepare()
		{
			var result = CheckConnectivity();
			if (result.ContainsError () == false)
				_verified = true;

			return result;
		}

		TaskResult IExecutable.Execute()
		{
			if (_verified)
				return new TaskResult ();
			return CheckConnectivity();
		}

		private TaskResult CheckConnectivity()
		{
			var results = new TaskResult ();
			try
			{
				var rs = GetWsClient();
				rs.GetSystemPermissions();
				results.Success ("Verified connection to report server at {0}.", ReportServerUrl);
			}
			catch (SoapException e)
			{
				results.Error ("Unable to connect to report server ({0}).\n"
				               + "Error details:\n{1}", ReportServerUrl, e.Message);
			}
			return results;
		}

		private bool _verified = false;
	}
}