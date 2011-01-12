﻿using System.Collections.Generic;
using System.Linq;

namespace DeploySharp.Core
{
	public class TaskResult
	{
		public void Error (string  message)
		{
			_subResults.Enqueue (new SubResult (SubResultType.Error, message));
		}

		public void Success (string message)
		{
			_subResults.Enqueue (new SubResult (SubResultType.Success, message));
		}

		public bool ContainsError()
		{
			return _subResults.Any (sr => sr.Type == SubResultType.Error);
		}

		public void VisitSubResults (ITaskResultVisitor visitor)
		{
			foreach (var result in _subResults)
			{
				switch (result.Type)
				{
					case SubResultType.Error:
						visitor.VisitError (result.Message);
						break;

					case SubResultType.Success:
						visitor.VisitSuccess (result.Message);
						break;
				}
			}
		}

		private class SubResult
		{
			public SubResult(SubResultType type, string message)
			{
				Type = type;
				Message = message;
			}

			public SubResultType Type { get; private set; }
			public string Message { get; private set; }
		}

		private enum SubResultType
		{
			Error,
			Success
		}

		private readonly Queue<SubResult> _subResults = new Queue<SubResult>();
	}
}