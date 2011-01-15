using System;

using DeploySharp.ReportWebService;

namespace DeploySharp.Tasks.Ssrs
{
	public abstract class SsrsTask
	{
		public string ReportServerUrl { get; set; }
		public System.Net.ICredentials Credentials { get; set; }

		protected ReportingService2010 GetWsClient()
		{
			var rs = new ReportingService2010 ();
			rs.Url = ReportServerUrl + "/ReportService2010.asmx";
			rs.Credentials = Credentials;

			return rs;
		}
	}
}