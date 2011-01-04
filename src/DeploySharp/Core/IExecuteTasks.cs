using System;

namespace DeploySharp.Core
{
	public interface IExecuteTasks
	{
		void ExecuteTask(Type taskType);
	}
}