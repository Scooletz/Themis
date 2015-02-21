using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NHibernate;
using NHibernate.Bytecode;
using NHibernate.Cfg;
using NHibernate.Connection;
using NHibernate.Dialect;
using NHibernate.Driver;
using NUnit.Framework;
using Environment = NHibernate.Cfg.Environment;

namespace Themis.Tests.NHibernate
{
    [TestFixture]
    public abstract class NHibernateBaseTest
    {
        private const string TestDbExtension = ".Test.db";

        public void Clear( )
        {
            Factory.Dispose();
            NoneDbFileExists();
        }

        private string _dbFile;

        public ISessionFactory Factory { get; private set; }

        public void Init(params Assembly[] assembliesWithMappings)
        {
            _dbFile = GetDbFileName();
            NoneDbFileExists();

            var configuration = new Configuration()
                .AddProperties(new Dictionary<string, string>
                                   {
                                       {Environment.ConnectionDriver, typeof (SQLite20Driver).FullName},
                                       {Environment.Dialect, typeof (SQLiteDialect).FullName},
                                       {Environment.ConnectionProvider, typeof (DriverConnectionProvider).FullName},
                                       {Environment.ConnectionString,string.Format("Data Source={0};Version=3;New=True;", _dbFile)},
                                       {Environment.ProxyFactoryFactoryClass,typeof (DefaultProxyFactoryFactory).AssemblyQualifiedName},
                                       {Environment.Hbm2ddlAuto, "create"},
                                       {Environment.ShowSql, true.ToString()}
                                   });

            foreach (var a in assembliesWithMappings)
            {
                configuration.AddAssembly(a);
            }

            OneLastTimeWithConfigurationBeforeFactoryIsCreated(configuration);

            Factory = configuration.BuildSessionFactory();
        }

        protected virtual void OneLastTimeWithConfigurationBeforeFactoryIsCreated(Configuration cfg)
        {

        }
        
        private static string GetDbFileName( )
        {
            var path = Path.GetFullPath(Path.GetRandomFileName() + TestDbExtension);
            if (!File.Exists(path))
            {
                return path;
            }

            // let's try again  
            return GetDbFileName();
        }

        private static void NoneDbFileExists( )
        {
            var di = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            foreach (var fileInfo in di.GetFiles("*" + TestDbExtension))
            {
                fileInfo.Delete();
            }
        }
    }
}