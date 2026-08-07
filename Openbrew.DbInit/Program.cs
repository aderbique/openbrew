using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using ctorx.Core.Configuration;

namespace Openbrew.DbInit
{
	internal static class Program
	{
		static int Main(string[] args)
		{
			try
			{
				var config = DbInitConfig.FromEnvironment();
				Console.WriteLine("Bootstrapping database '{0}' on {1}:{2}", config.DatabaseName, config.SqlHost, config.SqlPort);

				WaitForSqlServer(config);

				var createdDatabase = EnsureDatabaseExists(config);
				var tableExists = SchemaTableExists(config, "dbo", "IngredientCategory");
				var tableCount = GetUserTableCount(config);

				if (tableExists)
				{
					Console.WriteLine("Database already seeded.");
					return 0;
				}

				if (!createdDatabase && tableCount > 0 && !config.RecreateOnMissingSchema)
				{
					throw new InvalidOperationException(
						"The database exists but the expected schema is missing. Set OPENBREW_DB_INIT_RECREATE_ON_MISSING=true to rebuild the database.");
				}

				if (!createdDatabase && tableCount > 0 && config.RecreateOnMissingSchema)
				{
					Console.WriteLine("Existing schema is incomplete. Recreating database because OPENBREW_DB_INIT_RECREATE_ON_MISSING=true.");
					DropDatabase(config);
					EnsureDatabaseExists(config);
				}

				ExecuteSchemaScript(config);

				if (!SchemaTableExists(config, "dbo", "IngredientCategory"))
				{
					throw new InvalidOperationException("Database initialization completed but the expected schema table is still missing.");
				}

				Console.WriteLine("Database bootstrap complete.");
				return 0;
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine(ex);
				return 1;
			}
		}

		static void WaitForSqlServer(DbInitConfig config)
		{
			for (var attempt = 1; attempt <= config.MaxAttempts; attempt++)
			{
				try
				{
					using (var connection = new SqlConnection(config.MasterConnectionString))
					{
						connection.Open();
						using (var command = connection.CreateCommand())
						{
							command.CommandText = "SELECT 1";
							command.CommandType = CommandType.Text;
							command.ExecuteScalar();
						}
					}

					Console.WriteLine("SQL Server is ready.");
					return;
				}
				catch (Exception ex)
				{
					if (attempt == config.MaxAttempts)
					{
						throw new TimeoutException("Timed out waiting for SQL Server to become ready.", ex);
					}

					Console.WriteLine("Waiting for SQL Server ({0}/{1})...", attempt, config.MaxAttempts);
					Thread.Sleep(config.DelayMilliseconds);
				}
			}
		}

		static bool EnsureDatabaseExists(DbInitConfig config)
		{
			using (var connection = new SqlConnection(config.MasterConnectionString))
			{
				connection.Open();

				if (DatabaseExists(connection, config.DatabaseName))
				{
					Console.WriteLine("Database {0} already exists.", config.DatabaseName);
					return false;
				}

				Console.WriteLine("Creating database {0}.", config.DatabaseName);
				using (var command = connection.CreateCommand())
				{
					command.CommandText = string.Format("CREATE DATABASE [{0}];", config.DatabaseName.Replace("]", "]]"));
					command.CommandType = CommandType.Text;
					command.CommandTimeout = 0;
					command.ExecuteNonQuery();
				}

				return true;
			}
		}

		static void DropDatabase(DbInitConfig config)
		{
			using (var connection = new SqlConnection(config.MasterConnectionString))
			{
				connection.Open();
				Console.WriteLine("Dropping database {0}.", config.DatabaseName);
				using (var command = connection.CreateCommand())
				{
					command.CommandText = string.Format(
						"IF DB_ID(N'{0}') IS NOT NULL BEGIN ALTER DATABASE [{0}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{0}]; END",
						config.DatabaseName.Replace("]", "]]"));
					command.CommandType = CommandType.Text;
					command.CommandTimeout = 0;
					command.ExecuteNonQuery();
				}
			}
		}

		static bool DatabaseExists(SqlConnection connection, string databaseName)
		{
			using (var command = connection.CreateCommand())
			{
				command.CommandText = "SELECT COUNT(1) FROM sys.databases WHERE name = @databaseName";
				command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue("@databaseName", databaseName);
				return Convert.ToInt32(command.ExecuteScalar()) > 0;
			}
		}

		static int GetUserTableCount(DbInitConfig config)
		{
			using (var connection = new SqlConnection(config.TargetConnectionString))
			{
				connection.Open();
				using (var command = connection.CreateCommand())
				{
					command.CommandText = "SELECT COUNT(1) FROM sys.tables WHERE is_ms_shipped = 0";
					command.CommandType = CommandType.Text;
					return Convert.ToInt32(command.ExecuteScalar());
				}
			}
		}

		static bool SchemaTableExists(DbInitConfig config, string schemaName, string tableName)
		{
			using (var connection = new SqlConnection(config.TargetConnectionString))
			{
				connection.Open();
				using (var command = connection.CreateCommand())
				{
					command.CommandText = "SELECT CASE WHEN OBJECT_ID(@objectName, 'U') IS NULL THEN 0 ELSE 1 END";
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@objectName", string.Concat("[", schemaName, "].[", tableName, "]"));
					return Convert.ToInt32(command.ExecuteScalar()) == 1;
				}
			}
		}

		static void ExecuteSchemaScript(DbInitConfig config)
		{
			if (!File.Exists(config.SchemaScriptPath))
			{
				throw new FileNotFoundException("Schema script not found.", config.SchemaScriptPath);
			}

			Console.WriteLine("Applying schema script {0}.", config.SchemaScriptPath);

			var script = File.ReadAllText(config.SchemaScriptPath);
			var batches = SplitSqlBatches(script).ToList();

			using (var connection = new SqlConnection(config.TargetConnectionString))
			{
				connection.Open();

				for (var i = 0; i < batches.Count; i++)
				{
					var batch = batches[i];
					if (string.IsNullOrWhiteSpace(batch))
					{
						continue;
					}

					using (var command = connection.CreateCommand())
					{
						command.CommandText = batch;
						command.CommandType = CommandType.Text;
						command.CommandTimeout = 0;
						command.ExecuteNonQuery();
					}

					if ((i + 1) % 25 == 0 || i + 1 == batches.Count)
					{
						Console.WriteLine("Applied {0}/{1} SQL batches.", i + 1, batches.Count);
					}
				}
			}
		}

		static IEnumerable<string> SplitSqlBatches(string script)
		{
			using (var reader = new StringReader(script))
			{
				var batch = new StringBuilder();
				string line;

				while ((line = reader.ReadLine()) != null)
				{
					if (string.Equals(line.Trim(), "GO", StringComparison.OrdinalIgnoreCase))
					{
						if (batch.Length > 0)
						{
							yield return batch.ToString();
							batch.Length = 0;
						}
						continue;
					}

					batch.AppendLine(line);
				}

				if (batch.Length > 0)
				{
					yield return batch.ToString();
				}
			}
		}
	}

	internal sealed class DbInitConfig
	{
		public string SqlHost { get; private set; }
		public int SqlPort { get; private set; }
		public string DatabaseName { get; private set; }
		public string SqlAdminUser { get; private set; }
		public string SqlAdminPassword { get; private set; }
		public string SchemaScriptPath { get; private set; }
		public bool RecreateOnMissingSchema { get; private set; }
		public int MaxAttempts { get; private set; }
		public int DelayMilliseconds { get; private set; }

		public string MasterConnectionString
		{
			get { return BuildConnectionString("master"); }
		}

		public string TargetConnectionString
		{
			get { return BuildConnectionString(this.DatabaseName); }
		}

		DbInitConfig()
		{
		}

		public static DbInitConfig FromEnvironment()
		{
			return new DbInitConfig
			{
				SqlHost = GetEnv("OPENBREW_SQL_HOST", "db", "BREWGR_SQL_HOST"),
				SqlPort = GetIntEnv("OPENBREW_SQL_PORT", 1433, "BREWGR_SQL_PORT"),
				DatabaseName = GetEnv("OPENBREW_DB_NAME", "Brewgr_DEV", "BREWGR_DB_NAME"),
				SqlAdminUser = GetEnv("OPENBREW_SQL_ADMIN_USER", "sa", "BREWGR_SQL_ADMIN_USER"),
				SqlAdminPassword = GetEnv("OPENBREW_SA_PASSWORD", "Brewgr_dev_123!", "BREWGR_SA_PASSWORD"),
				SchemaScriptPath = GetEnv("OPENBREW_DB_INIT_SCRIPT", "/workspace/brewgr/Setup/Database/Build.20150807/20150807_initial.sql", "BREWGR_DB_INIT_SCRIPT"),
				RecreateOnMissingSchema = GetBoolEnv("OPENBREW_DB_INIT_RECREATE_ON_MISSING", false, "BREWGR_DB_INIT_RECREATE_ON_MISSING"),
				MaxAttempts = GetIntEnv("OPENBREW_SQL_WAIT_ATTEMPTS", 60, "BREWGR_SQL_WAIT_ATTEMPTS"),
				DelayMilliseconds = GetIntEnv("OPENBREW_SQL_WAIT_DELAY_MS", 2000, "BREWGR_SQL_WAIT_DELAY_MS")
			};
		}

		string BuildConnectionString(string databaseName)
		{
			var builder = new SqlConnectionStringBuilder
			{
				DataSource = string.Format("{0},{1}", this.SqlHost, this.SqlPort),
				InitialCatalog = databaseName,
				UserID = this.SqlAdminUser,
				Password = this.SqlAdminPassword,
				TrustServerCertificate = true,
				Encrypt = false,
				MultipleActiveResultSets = true,
				ConnectTimeout = 30
			};

			return builder.ToString();
		}

		static string GetEnv(string name, string fallback, string legacyName = null)
		{
			var value = ConfigReader.EnvironmentVariables.Read(name, legacyName);
			return string.IsNullOrWhiteSpace(value) ? fallback : value;
		}

		static int GetIntEnv(string name, int fallback, string legacyName = null)
		{
			int parsed;
			return int.TryParse(ConfigReader.EnvironmentVariables.Read(name, legacyName), out parsed) ? parsed : fallback;
		}

		static bool GetBoolEnv(string name, bool fallback, string legacyName = null)
		{
			bool parsed;
			return bool.TryParse(ConfigReader.EnvironmentVariables.Read(name, legacyName), out parsed) ? parsed : fallback;
		}
	}
}
