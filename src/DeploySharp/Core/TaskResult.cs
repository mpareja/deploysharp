using System;
using System.Collections.Generic;
using System.Linq;

namespace DeploySharp.Core
{
	public class TaskResult
	{
		public void Error (string  message)
		{
			_subResults.Enqueue (new SubResult (SubResultType.Error, message));
		}

		public void Error (string message, params string[] args)
		{
			Error (string.Format (message, args));
		}

		public void Error (Exception exception, string message, params string[] args)
		{
			var exString = string.Format("{0}--- Exception Details ---{0}{1}", Environment.NewLine, exception);
			Error(message + exString, args);
		}

		public void Warning (string message)
		{
			_subResults.Enqueue (new SubResult (SubResultType.Warning, message));
		}

		public void Warning(string message, params string[] args)
		{
			Warning (string.Format (message, args));
		}

		public void Success (string message)
		{
			_subResults.Enqueue (new SubResult (SubResultType.Success, message));
		}

		public void Success(string message, params string[] args)
		{
			Success (string.Format (message, args));
		}

		public bool ContainsError()
		{
			return _subResults.Any (sr => sr.Type == SubResultType.Error);
		}

		public void SendSubResultsTo (ITaskResultReceiver receiver)
		{
			foreach (var result in _subResults)
			{
				switch (result.Type)
				{
					case SubResultType.Success:
						receiver.ReceiveSuccess (result.Message);
						break;
					case SubResultType.Warning:
						receiver.ReceiveWarning (result.Message);
						break;
					case SubResultType.Error:
						receiver.ReceiveError (result.Message);
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
			Success,
			Warning,
			Error
		}

		private readonly Queue<SubResult> _subResults = new Queue<SubResult>();
	}
}