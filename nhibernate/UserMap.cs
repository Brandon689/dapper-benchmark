using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using nhibernateOrm;
// FLUENT MAPPING WAY
namespace nhibernateOrm
{
    public class UserMap : ClassMapping<User>
    {
        public UserMap()
        {
            Table("users");

            Id(x => x.Id, map =>
            {
                map.Generator(Generators.Native);
                map.Column("Id");
            });

            Property(x => x.Name, map =>
            {
                map.NotNullable(true);
                map.Length(100);
            });

            Property(x => x.Email, map =>
            {
                map.NotNullable(true);
                map.Length(100);
            });
        }
    }
}
