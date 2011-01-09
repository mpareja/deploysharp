namespace DeploySharp.Core
{
	public interface ITaskResultVisitor
	{
		void VisitError (string message);

		void VisitSuccess (string message);
	}
}