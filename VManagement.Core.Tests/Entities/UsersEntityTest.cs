using VManagement.Commons.Entities;
using VManagement.Commons.Entities.Attributes;
using VManagement.Commons.Entities.Interfaces;
using VManagement.Core.Entities;

namespace VManagement.Core.Tests.Entities
{
    [Flags]
    internal enum HookType
    {
        None = 0,

        // Save
        BeforeSave = 1,
        AfterSave = 2,

        // Insert
        BeforeInsert = 4,
        AfterInsert = 8,

        // Update
        BeforeUpdate = 16,
        AfterUpdate = 32,

        // Delete
        BeforeDelete = 64,
        AfterDelete = 128,

        // Create
        AfterCreate = 256,

        // Get
        AfterGet = 512
    }

    [TableEntity("USERS_TEST")]
    internal class UsersTestEntity : TableEntity<UsersTestEntity>
    {
        private string? _name;
        private DateTime? _birthdate;

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

        private HookType _expectedHooks = HookType.None;
        private HookType _calledHooks = HookType.None;

        internal void SetExpectedHooks(HookType hooks) => _expectedHooks = hooks;
        internal void VerifyHooksWereCalled() => Assert.AreEqual(_expectedHooks, _calledHooks);
        internal void RemoveHooksFromTrack(HookType hooks) => _calledHooks &= ~hooks;


        protected override void OnAfterCreatedCore()
        {
            _calledHooks |= HookType.AfterCreate;
        }

        protected override void OnAfterGetCore()
        {
            _calledHooks |= HookType.AfterGet;
        }

        protected override void OnBeforeSaveCore()
        {
            _calledHooks |= HookType.BeforeSave;
        }

        protected override void OnAfterSaveCore()
        {
            _calledHooks |= HookType.AfterSave;
        }

        protected override void OnBeforeInsertCore()
        {
            _calledHooks |= HookType.BeforeInsert;
        }

        protected override void OnAfterInsertCore()
        {
            _calledHooks |= HookType.AfterInsert;
        }

        protected override void OnBeforeUpdateCore()
        {
            _calledHooks |= HookType.BeforeUpdate;
        }

        protected override void OnAfterUpdateCore()
        {
            _calledHooks |= HookType.AfterUpdate;
        }

        protected override void OnBeforeDeleteCore()
        {
            _calledHooks |= HookType.BeforeDelete;
        }

        protected override void OnAfterDeleteCore()
        {
            _calledHooks |= HookType.AfterDelete;
        }
    }
}
