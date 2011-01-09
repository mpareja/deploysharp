namespace DeploySharp.Core
{
	public interface ITaskBuilder
	{
		T BuildTask<T>();
	}
}