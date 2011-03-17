using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using DeploySharp.Tasks;
using NUnit.Framework;
using Tests;

namespace DeploySharp.Tests.Tasks
{
	[TestFixture]
	public class ExecuteSqlScriptsTaskTests
	{
		[SetUp]
		public void Setup()
		{
			_table = "my_table_" + DateTime.Now.Ticks;
		}

		[TearDown]
		public void TearDown()
		{
			using (var connection = GetConnection())
			{
				connection.Open();
				using (var command = connection.CreateCommand ())
				{
					command.CommandType = CommandType.Text;
					command.CommandText = string.Format(@"
						if object_id('{0}') is not null
							drop table {0}", _table);
					command.ExecuteNonQuery ();
				}
			}
		}

		[Test]
		public void can_execute_script()
		{
			var task = GetTaskWithCredentials();
			using (var tempdir = new TempDirectoryPathAbsolute())
			{
				// write the script file to drive
				var file = tempdir.SaveToTempFile(string.Format (@"
					create table {0} ( my_id int );
					go
					insert into {0} values ( 2 );
					go", _table));

				task.ScriptFiles = new[] { file };

				task.PrepareAndAssertNoErrors();
				task.ExecuteAndAssertNoErrors();
			}
			AssertQueryReturns("select my_id from " + _table, 2);
		}

		[Test]
		public void can_execute_multiple_scripts()
		{
			var task = GetTaskWithCredentials ();
			using (var tempdir = new TempDirectoryPathAbsolute ())
			{
				var file1 = tempdir.SaveToTempFile (string.Format (@"
					create table {0} ( my_id int );
					go
					insert into {0} values ( 1 );
					go", _table));

				var file2 = tempdir.SaveToTempFile(string.Format(@"
					insert into {0} values ( 2 )", _table));

				task.ScriptFiles = new[] { file1, file2 };

				task.PrepareAndAssertNoErrors ();
				task.ExecuteAndAssertNoErrors ();
			}

			using (var connection = GetConnection ())
			{
				connection.Open ();
				using (var command = connection.CreateCommand ())
				{
					command.CommandType = CommandType.Text;
					command.CommandText = "select my_id from " + _table + " where my_id = 1";
					Assert.AreEqual (1, command.ExecuteScalar ());
				}
				using (var command = connection.CreateCommand ())
				{
					command.CommandType = CommandType.Text;
					command.CommandText = "select my_id from " + _table + " where my_id = 2";
					Assert.AreEqual (2, command.ExecuteScalar ());
				}
			}
		}

		private void AssertQueryReturns(string commandText, int expected)
		{
			using (var connection = GetConnection ())
			{
				connection.Open();
				using (var command = connection.CreateCommand ())
				{
					command.CommandType = CommandType.Text;
					command.CommandText = commandText;
					Assert.AreEqual(expected, command.ExecuteScalar());
				}
			}
		}

		private SqlConnection GetConnection() 
		{
			var task = GetTaskWithCredentials();
			var builder = new SqlConnectionStringBuilder {
				DataSource = task.Server,
				InitialCatalog = task.Database,
				UserID = task.Username,
				Password = task.Password
			};

			return new SqlConnection (builder.ToString ());
		}

		private ExecuteSqlScriptsTask GetTaskWithCredentials()
		{
			return new ExecuteSqlScriptsTask {
				Server = ConfigurationManager.AppSettings["testDbServer"],
				Database = ConfigurationManager.AppSettings["testDbName"],
				Username = ConfigurationManager.AppSettings["testDbUsername"],
				Password = ConfigurationManager.AppSettings["testDbPassword"]
			};
		}

		private string _table;
	}
}