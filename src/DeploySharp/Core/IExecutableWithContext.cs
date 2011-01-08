namespace DeploySharp.Core
{
	public interface IExecutableWithContext
	{
		void Execute(TaskContext context);
	}
}