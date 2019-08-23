﻿using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using System;

using System.Data.SqlClient;
using System.IO;
using System.Reflection;

namespace NHibernate.Caches.Redis.Tests
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class IntegrationTestBase : RedisTest
    {
        private const string databaseName = "NHibernateCachesRedisTests";
        private const string masterConnectionString = @"Server=(local)\SQLExpress;Database=master;Trusted_Connection=True;";
        private const string connectionString = @"Server=(local)\SQLExpress;Database=" + databaseName + @";Trusted_Connection=True;";
        private string dataFilePath;
        private string logFilePath;

        private static Configuration configuration;
        /// <summary>
        /// 
        /// </summary>
        protected IntegrationTestBase()
        {
            RedisCacheProvider.InternalSetConnectionMultiplexer(ConnectionMultiplexer);
            RedisCacheProvider.InternalSetOptions(CreateTestProviderOptions());

            InitializeDatabasePaths();

            using (var connection = new SqlConnection(masterConnectionString))
            {
                connection.Open();
                DeleteDatabaseIfExists(connection);
                CreateDatabase(connection);
            }

            if (configuration == null)
            {
                //configuration = Fluently.Configure()
                //    .Database(
                //        MsSqlConfiguration.MsSql2008.ConnectionString(connectionString)
                //    )
                //    .Mappings(m => m.FluentMappings.Add(typeof(PersonMapping)))
                //    .ExposeConfiguration(cfg => cfg.SetProperty(NHibernate.Cfg.Environment.GenerateStatistics, "true"))
                //    .Cache(c => c.UseQueryCache().
                //        UseSecondLevelCache().
                //        ProviderClass<RedisCacheProvider>())
                //    .BuildConfiguration();
            }

            new SchemaExport(configuration).Create(false, true);
        }

        private void InitializeDatabasePaths()
        {
            var currentPath = Assembly.GetExecutingAssembly().GetName().CodeBase.Replace("file:///", "");
            currentPath = Path.GetDirectoryName(currentPath);

            dataFilePath = Path.Combine(currentPath, databaseName + ".mdf");
            logFilePath = Path.Combine(currentPath, databaseName + "_log.ldf");
        }

        private void DeleteDatabaseIfExists(SqlConnection connection)
        {
            var drop = @"if exists(select name FROM sys.databases where name = '{0}')
                         begin
                             alter database [{0}] set SINGLE_USER with ROLLBACK IMMEDIATE;
                             drop database [{0}];
                         end";

            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = String.Format(drop, databaseName);
                cmd.ExecuteNonQuery();
            }

            if (File.Exists(dataFilePath)) File.Delete(dataFilePath);
            if (File.Exists(logFilePath)) File.Delete(logFilePath);
        }

        private void CreateDatabase(SqlConnection connection)
        {
            // Minimum DB size is 2MB on <= SQL 2008 R2, 3MB on SQL 2012 and 5MB >= SQL 2014.
            var create = @"create database [{0}] on PRIMARY";
            create += @" ( name = N'{0}', filename = N'{1}', size = 5MB, maxsize = unlimited, filegrowth = 10% ) ";
            create += @" log on ";
            create += @" ( name = N'{0}_log', filename = N'{2}', size = 1MB, maxsize = 2048GB, filegrowth = 10% ) ";

            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = String.Format(create,
                    databaseName, dataFilePath, logFilePath);
                cmd.ExecuteNonQuery();
            }
        }

        protected ISessionFactory CreateSessionFactory()
        {
            return configuration.BuildSessionFactory();
        }

        protected void UsingSession(ISessionFactory sessionFactory, Action<ISession> action)
        {
            using (var session = sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                action(session);
                transaction.Commit();
            }
        }
    }
}
