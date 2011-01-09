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
		public void GivenAnErrorResult_WhenVisistingResults_ReportError()
		{
			var result = new TaskResult ();
			result.Error ("my error");

			var visitor = new TestTaskResultVisitor ();
			result.VisitSubResults (visitor);

			visitor.AssertResultCount (1);
			visitor.AssertMessageAndTypeEquals (1, "Error", "my error");
		}

		[Test]
		public void GivenMultipleErrorResults_WhenVisistingResults_ReportAllErrors()
		{
			var result = new TaskResult ();
			result.Error ("my error");
			result.Error ("my error2");

			var visitor = new TestTaskResultVisitor ();
			result.VisitSubResults (visitor);

			visitor.AssertResultCount (2);
			visitor.AssertMessageAndTypeEquals (1, "Error", "my error");
			visitor.AssertMessageAndTypeEquals (2, "Error", "my error2");
		}

		[Test]
		public void GivenAnErrorAndSuccess_WhenVisitingResults_ReportBoth()
		{
			var result = new TaskResult ();
			result.Success ("my success");
			result.Error ("my error");

			var visitor = new TestTaskResultVisitor ();
			result.VisitSubResults (visitor);

			visitor.AssertResultCount (2);
			visitor.AssertMessageAndTypeEquals (1, "Success", "my success");
			visitor.AssertMessageAndTypeEquals (2, "Error", "my error");
		}
	}

	public class TestTaskResultVisitor : ITaskResultVisitor
	{
		public void VisitError (string message)
		{
			_messages.Enqueue (new Results("Error", message));
		}

		public void VisitSuccess(string message)
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
	public class TestTaskResultVisitorTests
	{
		[Test]
		public void WhenNoResultsAdded_CountIsZero()
		{
			var visitor = new TestTaskResultVisitor ();
			visitor.AssertResultCount (0);
		}

		[Test]
		public void WhenResultIsAdded_TheCountIncreases()
		{
			var visitor  = new TestTaskResultVisitor();
			visitor.VisitError ("an error");

			visitor.AssertResultCount (1);
		}

		[Test]
		public void WhenMultipleResultsAdded_CountMatches()
		{
			var visitor = new TestTaskResultVisitor ();
			visitor.VisitError ("an error");
			visitor.VisitError ("an error2");

			visitor.AssertResultCount (2);
		}

		[Test]
		public void WhenErrorAdded_MessageMatches()
		{
			var visitor = new TestTaskResultVisitor ();
			visitor.VisitError ("an error");

			visitor.AssertMessageAndTypeEquals (1, "Error", "an error");
		}
	}
}