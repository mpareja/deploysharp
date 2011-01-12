using System;
using System.Collections.Generic;

using DeploySharp.Core;

using NUnit.Framework;

using Shouldly;

namespace DeploySharp.Tests.Deployment.Core
{
	[TestFixture]
	public class TaskResultTests
	{
		[Test]
		public void CanCreateTaskResult()
		{
			Assert.IsNotNull (new TaskResult());
		}

		[Test]
		public void CanAddError()
		{
			var result = new TaskResult();
			result.Error ("my error");
		}

		[Test]
		public void WhenNoErrorsAdded_ContainsError_ShouldBeFalse()
		{
			var result = new TaskResult ();

			result.ContainsError().ShouldBe (false);
		}

		[Test]
		public void WhenAnErrorIsAdded_ContainsError_ShouldBeTrue()
		{
			var result = new TaskResult ();
			result.Error ("my error");

			result.ContainsError ().ShouldBe (true);
		}

		[Test]
		public void GivenAnErrorResult_ReportError()
		{
			var result = new TaskResult ();
			result.Error ("my error");

			var receiver = new TestTaskResultReceiver ();
			result.SendSubResultsTo (receiver);

			receiver.AssertResultCount (1);
			receiver.AssertMessageAndTypeEquals (1, "Error", "my error");
		}

		[Test]
		public void GivenMultipleErrorResults_ReportAllErrors()
		{
			var result = new TaskResult ();
			result.Error ("my error");
			result.Error ("my error2");

			var receiver = new TestTaskResultReceiver ();
			result.SendSubResultsTo (receiver);

			receiver.AssertResultCount (2);
			receiver.AssertMessageAndTypeEquals (1, "Error", "my error");
			receiver.AssertMessageAndTypeEquals (2, "Error", "my error2");
		}

		[Test]
		public void GivenAnErrorAndSuccess_ReportBoth()
		{
			var result = new TaskResult ();
			result.Success ("my success");
			result.Error ("my error");

			var receiver = new TestTaskResultReceiver ();
			result.SendSubResultsTo (receiver);

			receiver.AssertResultCount (2);
			receiver.AssertMessageAndTypeEquals (1, "Success", "my success");
			receiver.AssertMessageAndTypeEquals (2, "Error", "my error");
		}
	}

	public class TestTaskResultReceiver : ITaskResultReceiver
	{
		public void ReceiveError (string message)
		{
			_messages.Enqueue (new Results("Error", message));
		}

		public void ReceiveSuccess(string message)
		{
			_messages.Enqueue (new Results ("Success", message));
		}

		public void AssertResultCount (int expectedCount)
		{
			_messages.Count.ShouldBe (expectedCount);
		}

		private class Results
		{
			public Results(string type, string message)
			{
				Type = type;
				Message = message;
			}
			public string Type;
			public string Message;
		}

		public void AssertMessageAndTypeEquals (int i, string type, string message)
		{
			i.ShouldBeLessThan (_messages.Count + 1);
			i.ShouldBeGreaterThan(0);

			var result = _messages.ToArray()[i - 1];
			result.Type.ShouldBe (type);
			result.Message.ShouldBe (message);
		}

		private Queue<Results> _messages = new Queue<Results>();
	}

	[TestFixture]
	public class TestTaskResultReceiverTests
	{
		[Test]
		public void WhenNoResultsAdded_CountIsZero()
		{
			var receiver = new TestTaskResultReceiver ();
			receiver.AssertResultCount (0);
		}

		[Test]
		public void WhenResultIsAdded_TheCountIncreases()
		{
			var receiver  = new TestTaskResultReceiver();
			receiver.ReceiveError ("an error");

			receiver.AssertResultCount (1);
		}

		[Test]
		public void WhenMultipleResultsAdded_CountMatches()
		{
			var receiver = new TestTaskResultReceiver ();
			receiver.ReceiveError ("an error");
			receiver.ReceiveError ("an error2");

			receiver.AssertResultCount (2);
		}

		[Test]
		public void WhenErrorAdded_MessageMatches()
		{
			var receiver = new TestTaskResultReceiver ();
			receiver.ReceiveError ("an error");

			receiver.AssertMessageAndTypeEquals (1, "Error", "an error");
		}
	}
}