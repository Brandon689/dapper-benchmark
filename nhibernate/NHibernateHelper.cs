using NHibernate;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Mapping.ByCode;
using nhibernateOrm;

public static class NHibernateHelper
{
    private static ISessionFactory _sessionFactory;

    private static ISessionFactory SessionFactory
    {
        get
        {
            if (_sessionFactory == null)
            {
                var configuration = new Configuration();
                configuration.DataBaseIntegration(db =>
                {
                    db.Dialect<PostgreSQL83Dialect>();
                    db.Driver<NpgsqlDriver>();
                    db.ConnectionString = "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=mysecretpassword;";
                    db.SchemaAction = SchemaAutoAction.Update;
                    db.LogSqlInConsole = true;
                    db.LogFormattedSql = true;
                });

                // Add fluent mappings
                var mapper = new ModelMapper();
                mapper.AddMapping<UserMap>();
                HbmMapping domainMapping = mapper.CompileMappingForAllExplicitlyAddedEntities();
                configuration.AddMapping(domainMapping);

                _sessionFactory = configuration.BuildSessionFactory();
            }
            return _sessionFactory;
        }
    }

    public static ISession OpenSession()
    {
        return SessionFactory.OpenSession();
    }
}




// XML MAPPING WAY (NO TYPE SAFETY)

//using NHibernate;
//using NHibernate.Cfg;
//using NHibernate.Dialect;
//using NHibernate.Driver;
//using nhibernateOrm;

//public static class NHibernateHelper
//{
//    private static ISessionFactory _sessionFactory;

//    private static ISessionFactory SessionFactory
//    {
//        get
//        {
//            if (_sessionFactory == null)
//            {
//                var configuration = new Configuration();
//                configuration.DataBaseIntegration(db =>
//                {
//                    db.Dialect<PostgreSQL83Dialect>();
//                    db.Driver<NpgsqlDriver>();
//                    db.ConnectionString = "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=mysecretpassword;";
//                    db.SchemaAction = SchemaAutoAction.Update; // Changed from Create to Update
//                    db.LogSqlInConsole = true; // This will log SQL statements to console
//                    db.LogFormattedSql = true;
//                });

//                configuration.AddAssembly(typeof(User).Assembly);

//                _sessionFactory = configuration.BuildSessionFactory();
//            }
//            return _sessionFactory;
//        }
//    }

//    public static ISession OpenSession()
//    {
//        return SessionFactory.OpenSession();
//    }
//}
