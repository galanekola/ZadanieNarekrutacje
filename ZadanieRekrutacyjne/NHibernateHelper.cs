using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using ZadanieRekrutacyjne.Models;

namespace ZadanieRekrutacyjne;


public class NHibernateHelper
    {
        private static ISessionFactory _sessionFactory;

        public static NHibernate.ISession OpenSession()
        {
            return SessionFactory.OpenSession();
        }

        private static ISessionFactory SessionFactory
        {
            get
            {
                if (_sessionFactory == null)
                {
                    _sessionFactory = Fluently.Configure()
                        .Database(
                            MsSqlConfiguration.MsSql2012.ConnectionString(
                                "Server=localhost\\TEW_SQLEXPRESS;Database=ZadanieRekrutacyjne;Integrated Security=SSPI;Application Name=ZadanieRekrutacyjne;TrustServerCertificate=true;")
                        )  .Mappings(m =>
                            m.FluentMappings.AddFromAssemblyOf<Tasks>())
                       
                        .ExposeConfiguration(cfg => new SchemaUpdate(cfg).Execute(false, true))
                        .BuildSessionFactory();
                }
                return _sessionFactory;
            }
        }
    }
    
    