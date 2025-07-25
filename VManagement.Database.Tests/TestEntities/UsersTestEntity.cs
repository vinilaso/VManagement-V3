using VManagement.Commons.Entities;
using VManagement.Commons.Entities.Attributes;
using VManagement.Commons.Entities.Interfaces;
using VManagement.Core.Entities;

namespace VManagement.Database.Tests.TestEntities
{
    [TableEntity("USERS_TEST")]
    internal class UsersTestEntity : TableEntity<UsersTestEntity>
    {
        private long? _id;
        private string? _name;
        private DateTime? _birthdate;

        [EntityColumnName("NAME")]
        public string? Name { get; set; }

        [EntityColumnName("BIRTHDATE")]
        public DateTime? BirthDate { get; set; }

        public string? TestProperty { get; set; }        
    }
}
