using VManagement.Commons.Entities.Attributes;
using VManagement.Commons.Entities.Interfaces;

namespace VManagement.Database.Tests.TestEntities
{
    [TableEntity("USERS_TEST")]
    internal class UsersTestEntity : ITableEntity
    {
        private long? _id;
        private string? _name;
        private DateTime? _birthdate;

        [EntityColumnName("ID")]
        public long? Id
        {
            get => _id;
            set
            {
                _id = value;
                TrackedFields["ID"].ChangeValue(value);
            }
        }

        [EntityColumnName("NAME")]
        public string? Name
        {
            get => _name;
            set
            {
                _name = value;
                TrackedFields["NAME"].ChangeValue(value);
            }
        }

        [EntityColumnName("BIRTHDATE")]
        public DateTime? BirthDate
        {
            get => _birthdate;
            set
            {
                _birthdate = value;
                TrackedFields["BIRTHDATE"].ChangeValue(value);
            }
        }

        public string? TestProperty { get; set; }

        public ITrackedFieldCollection TrackedFields { get; set; }

        
    }
}
