using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Microsoft.Extensions.Configuration;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using ZadanieRekrutacyjne.Models;
using NHibernateSession = NHibernate.ISession;

namespace ZadanieRekrutacyjne
{
    /// <summary>
    /// Nhibernate helper class to open session with database
    /// </summary>
    public static class NHibernateHelper
    {
        private static ISessionFactory _sessionFactory;
        private static IConfiguration _configuration;

        public static void Initialize(IConfiguration configuration)
        {
            _configuration = configuration;

            if (_sessionFactory == null)
            {
                var connectionString = _configuration.GetConnectionString("Default");

                _sessionFactory = Fluently.Configure()
                    .Database(
                        MySQLConfiguration.Standard.ConnectionString(connectionString)
                    )
                    .Mappings(m =>
                        m.FluentMappings.AddFromAssemblyOf<Tasks>()
                    )
                    .ExposeConfiguration(cfg => new SchemaUpdate(cfg).Execute(false, true))
                    .BuildSessionFactory();
            }
        }

        public static NHibernateSession OpenSession()
        {
            return _sessionFactory.OpenSession();
        }
    }
}