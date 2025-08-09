using VManagement.Commons.Entities.Attributes;
using VManagement.Core.Entities;

namespace VManagement.Database.Tests.TestEntities
{
    [TableEntity("USERS_TEST")]
    internal class UsersTestEntity : TableEntity<UsersTestEntity>
    {
        [EntityColumnName("NAME")]
        public string? Name { get; set; }

        [EntityColumnName("BIRTHDATE")]
        public DateTime? BirthDate { get; set; }

        public string? TestProperty { get; set; }        
    }
}
