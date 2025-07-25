using VManagement.Core.Entities;
using VManagement.Core.Exceptions;
using VManagement.Core.Tests.DAO;
using VManagement.Database.Clauses;

namespace VManagement.Core.Tests.Entities
{
    [TestClass]
    public class TableEntityTests
    {
        private MockTableEntityDAO<UsersTestEntity> _mockDao = new();

        [TestInitialize]
        public void Initialize()
        {
            _mockDao = new MockTableEntityDAO<UsersTestEntity>();
            UsersTestEntity.ConfigureDAO(_mockDao);        
        }

        [TestMethod]
        public void ExecuteSave_WhenEntityIsNew_ShouldCallInsertAndChangeState()
        {
            UsersTestEntity user = UsersTestEntity.New();
            user.Name = "Test";

            // Ação
            user.Save();

            // Asserções
            Assert.AreEqual(1, _mockDao.InsertCallCount, "DAO.Insert deveria ter sido chamado uma vez.");
            Assert.IsNotNull(_mockDao.LastInsertedEntity, "A entidade deveria ter sido persistida.");
            Assert.AreEqual(user.Name, _mockDao.LastInsertedEntity!.Name, "A entidade que foi persistida está errada.");
            Assert.AreEqual(EntityState.Loaded, user.State, "O estado da entidade deveria ser 'Loaded'.");
            Assert.AreEqual(_mockDao.InsertedId, user.Id, "O ID retornado pelo DAO deveria ter sido atribuído à entidade.");
        }

        [TestMethod]
        public void ExecuteSave_WhenEntityIsLoaded_ShouldCallUpdateAndChangeState()
        {
            _mockDao.EntityToReturnOnSelect = UsersTestEntity.New();
            _mockDao.EntityToReturnOnSelect.Id = 1;
            _mockDao.EntityToReturnOnSelect.Name = "Unset";

            UsersTestEntity user = UsersTestEntity.Find(new Restriction());

            Assert.IsNotNull(user);
            user.Name = "João Miguel";

            user.Save();

            // Assert
            Assert.AreEqual(0, _mockDao.InsertCallCount);
            Assert.AreEqual(1, _mockDao.UpdateCallCount);
            Assert.AreEqual("João Miguel", _mockDao.LastUpdatedEntity?.Name);
            Assert.AreEqual(EntityState.Loaded, user.State, "O estado da entidade deveria ser 'Loaded'.");
        }

        [TestMethod]
        public void ExecuteInsert_ShouldCallInsertSpecificHooks()
        {
            UsersTestEntity user = UsersTestEntity.New();

            user.RemoveHooksFromTrack(HookType.AfterCreate);

            user.SetExpectedHooks(HookType.BeforeInsert | HookType.AfterInsert | HookType.BeforeSave | HookType.AfterSave);

            user.Save();

            user.VerifyHooksWereCalled();
        }

        [TestMethod]
        public void ExecuteUpdate_ShouldCallUpdateSpecificHooks()
        {
            _mockDao.EntityToReturnOnSelect = UsersTestEntity.New();
            
            UsersTestEntity user = UsersTestEntity.Find(new Restriction());

            user.RemoveHooksFromTrack(HookType.AfterGet | HookType.AfterCreate);

            user.SetExpectedHooks(HookType.BeforeSave | HookType.AfterSave | HookType.BeforeUpdate | HookType.AfterUpdate);

            user.Save();

            user.VerifyHooksWereCalled();
        }

        [TestMethod]
        public void ExecuteCreate_ShouldCallCreateSpecificHooks()
        {
            UsersTestEntity user = UsersTestEntity.New();

            user.SetExpectedHooks(HookType.AfterCreate);

            user.VerifyHooksWereCalled();
        }

        [TestMethod]
        public void ExecuteDelete_ShouldCallDeleteSpecificHooks()
        {
            _mockDao.EntityToReturnOnSelect = UsersTestEntity.New();

            UsersTestEntity user = UsersTestEntity.Find(new Restriction());

            user.RemoveHooksFromTrack(HookType.AfterGet | HookType.AfterCreate);

            user.SetExpectedHooks(HookType.BeforeDelete | HookType.AfterDelete);

            user.Delete();

            user.VerifyHooksWereCalled();
        }

        [TestMethod]
        public void ExecuteGet_ShouldCallGetSpecificHooks()
        {
            _mockDao.EntityToReturnOnSelect = UsersTestEntity.New();

            UsersTestEntity user = UsersTestEntity.Find(new Restriction());

            user.RemoveHooksFromTrack(HookType.AfterCreate);

            user.SetExpectedHooks(HookType.AfterGet);

            user.VerifyHooksWereCalled();
        }

        [TestMethod]
        public void ExecuteSave_WhenEntityIsDeleted_ShouldThrowException()
        {
            _mockDao.EntityToReturnOnSelect = UsersTestEntity.New();
            _mockDao.EntityToReturnOnSelect.Id = 1;
            _mockDao.EntityToReturnOnSelect.Name = "Unset";

            UsersTestEntity user = UsersTestEntity.Find(new Restriction());

            Assert.IsNotNull(user);

            user.Delete();

            Assert.ThrowsException<InvalidEntityActionException>(user.Save, "Não deveria ser possível salvar uma entidade excluída.");
        }

        [TestMethod]
        public void ExecuteCreate_ShouldStartAsNew()
        {
            UsersTestEntity user = UsersTestEntity.New();
            Assert.AreEqual(EntityState.New, user.State, "O estado da entidade deveria ser 'New'");
        }

        [TestMethod]
        public void ExecuteDelete_WhenEntityIsNew_ShouldThrowException()
        {
            UsersTestEntity newEntity = UsersTestEntity.New();
            Assert.ThrowsException<InvalidEntityActionException>(newEntity.Delete, "Não deveria ser possível excluir uma entidade não carregada!");
        }

        [TestMethod]
        public void ExecuteDelete_WhenEntityIsLoaded_ShouldChangeState()
        {
            _mockDao.EntityToReturnOnSelect = UsersTestEntity.New();
            _mockDao.EntityToReturnOnSelect.Id = 1;
            _mockDao.EntityToReturnOnSelect.Name = "Unset";

            UsersTestEntity user = UsersTestEntity.Find(new Restriction());

            Assert.IsNotNull(user);

            user.Delete();

            Assert.AreEqual(EntityState.Deleted, user.State, "O estado da entidade deveria ser 'Deleted'");
        }

        [TestMethod]
        public void ExecuteFind_WhenReturnIsNull_ShouldThrowException()
        {
            _mockDao.EntityToReturnOnSelect = null;
            Assert.ThrowsException<EntityNotFoundException>(() => UsersTestEntity.Find(new Restriction()));
        }

        [TestMethod]
        public void ExecuteFindFirstOrDefault_WhenReturnIsNull_ShouldReturnNull()
        {
            _mockDao.EntityToReturnOnSelect = null;

            UsersTestEntity? user = UsersTestEntity.FindFirstOrDefault(new Restriction());

            Assert.IsNull(user, "FindFirstOrDefault deveria retornar nulo quando nenhuma entidade atende à restrição.");
        }
    }
}
