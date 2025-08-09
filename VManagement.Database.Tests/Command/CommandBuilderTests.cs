using Microsoft.Data.SqlClient;
using VManagement.Commons.Entities;
using VManagement.Database.Clauses;
using VManagement.Database.Command;
using VManagement.Database.Exceptions;
using VManagement.Database.Tests.TestEntities;

namespace VManagement.Database.Tests.Command
{
    [TestClass]
    public class CommandBuilderTests
    {
        private MockTableEntityDAO<UsersTestEntity> _mockDao = new();

        [TestInitialize]
        public void Initialize()
        {
            _mockDao = new MockTableEntityDAO<UsersTestEntity>();
            UsersTestEntity.ConfigureDAO(_mockDao);
        }

        [TestMethod]
        public void BuildSelectCommand_WhenPopulateCommandObjectIsTrue_ShouldPopulateMockCommand()
        {
            // Criando meu comando SQL
            MockVManagementCommand mockCommand = new();

            // Criando o objeto de configuração
            CommandBuilderOptions options = new()
            {
                AutoGenerateRestriction = true,
                PopulateCommandObject = true
            };

            // Criando a entidade de teste
            UsersTestEntity user = TableEntityFactory.CreateInstanceFor<UsersTestEntity>();
            user.Id = 1;

            // Instanciando o CommandBuilder
            CommandBuilder<UsersTestEntity> commandBuilder = new(options, entity: user, command: mockCommand);

            // Ação
            commandBuilder.BuildSelectCommand();

            // Asserções - Verificando de o comando foi populado corretamente
            Assert.AreNotEqual(mockCommand.CommandText, string.Empty, "O CommandText do comando não deveria estar vazio.");
            Assert.AreEqual(1, mockCommand.Parameters.Count, "O comando deveria ter um parâmetro.");
            Assert.AreEqual(user.Id, mockCommand.Parameters[0].Value, "O valor do parâmetro no comando está incorreto.");

            string expectedFullQuery = $"SELECT NAME, BIRTHDATE, ID FROM USERS_TEST V WHERE (V.ID = {mockCommand.Parameters[0].ParameterName})";
            Assert.AreEqual(expectedFullQuery, mockCommand.CommandText, "O CommandText populado no comando está incorreto.");
        }

        [TestMethod]
        public void BuildSelectCommand_WhenPopulateCommandObjectIsTrue_ShouldThrowException()
        {
            // Criando o objeto de configuração
            CommandBuilderOptions options = new()
            {
                AutoGenerateRestriction = true,
                PopulateCommandObject = true
            };

            // Criando a entidade de teste
            UsersTestEntity user = TableEntityFactory.CreateInstanceFor<UsersTestEntity>();
            user.Id = 1;

            // Instanciando o CommandBuilder
            CommandBuilder<UsersTestEntity> commandBuilder = new(options, entity: user);

            // Ação - Asserção
            Assert.ThrowsException<CommandBuilderAbortedException>(commandBuilder.BuildSelectCommand, "A operação deveria ter sido cancelada por não ter sido informado comando válido.");
        }

        [TestMethod]
        public void BuildSelectCommand_WhenAutoGenerateRestrictionIsFalse_ShouldCreateGenericSelectCommand()
        {
            // Criando o objeto de configuração
            CommandBuilderOptions options = new()
            {
                AutoGenerateRestriction = false,
                PopulateCommandObject = false
            };

            // Instanciando o CommandBuilder
            CommandBuilder<UsersTestEntity> commandBuilder = new(options);
            CommandBuilderResult result = commandBuilder.BuildSelectCommand();

            // Asserção
            Assert.AreNotEqual(string.Empty, result.CommandText, "O CommandText não deveria estar vazio.");
            Assert.AreEqual(0, result.Restriction.Parameters.Count, "Não deveriam ter sido gerados parâmetros.");

            string expectedFullQuery = "SELECT NAME, BIRTHDATE, ID FROM USERS_TEST V ";

            Assert.AreEqual(expectedFullQuery, result.CommandText, "O CommandText gerado está incorreto.");
        }

        [TestMethod]
        public void BuildSelectCommand_WhenAutoGenerateRestrictionIsTrue_ShouldAppendExistingCondition()
        {
            CommandBuilderOptions options = new()
            {
                AutoGenerateRestriction = false,
                PopulateCommandObject = false,
                AppendExistingRestriction = true
            };

            const string NAME_PARAM_VALUE = "JOÃO";

            Restriction preRestriction = new("UPPER(V.NAME) = @pNAME");
            preRestriction.Parameters.Add("pNAME", NAME_PARAM_VALUE);

            preRestriction.AddSorting("V.ID", OrderByClause.SortDirection.Descending);

            CommandBuilder<UsersTestEntity> commandBuilder = new(options, preRestriction: preRestriction);
            CommandBuilderResult result = commandBuilder.BuildSelectCommand();

            Assert.AreNotEqual(string.Empty, result.CommandText, "O CommandText não deveria estar vazio.");
            Assert.AreEqual(1, result.Restriction.Parameters.Count, "O comando deveria ter um parâmetro.");

            SqlParameter parameter = result.Restriction.Parameters[0];

            Assert.AreEqual(NAME_PARAM_VALUE, parameter.Value, "O valor do parâmetro no comando está incorreto.");

            string expectedFullQuery = $"SELECT NAME, BIRTHDATE, ID FROM USERS_TEST V WHERE ((UPPER(V.NAME) = {parameter.ParameterName})) ORDER BY V.ID DESC";
            Assert.AreEqual(expectedFullQuery, result.CommandText, "O CommandText gerado está incorreto.");
        }

        [TestMethod]
        public void BuildUpdateCommand_WhenNameIsChanged_ShouldAppendJustNameColumnToUpdate()
        {
            const int
                ID_PARAM_INDEX = 0,
                NAME_PARAM_INDEX = 1;

            CommandBuilderOptions options = new()
            {
                AutoGenerateRestriction = true,
                PopulateCommandObject = false
            };

            _mockDao.EntityToReturnOnSelect = new();
            _mockDao.EntityToReturnOnSelect.Id = 1;
            _mockDao.EntityToReturnOnSelect.Name = "João";

            UsersTestEntity user = UsersTestEntity.Find(Restriction.Empty);
            user.Name = "João Miguel";

            CommandBuilder<UsersTestEntity> commandBuilder = new(options, entity: user);
            CommandBuilderResult result = commandBuilder.BuildUpdateCommand();

            Assert.AreNotEqual(string.Empty, result.CommandText, "O CommandText gerado não deveria ser vazio.");
            Assert.AreEqual(2, result.Restriction.Parameters.Count, "O número de parâmetros gerados está incorreto.");

            SqlParameter
                idParameter = result.Restriction.Parameters[ID_PARAM_INDEX],
                nameParemeter = result.Restriction.Parameters[NAME_PARAM_INDEX];

            Assert.AreEqual(user.Id, idParameter.Value, "O valor do parâmetro para ID está incorreto.");
            Assert.AreEqual(user.Name, nameParemeter.Value, "O valor do parâmetro para NAME está incorreto.");

            string expectedFullQuery = $"UPDATE USERS_TEST SET NAME = {nameParemeter.ParameterName} WHERE (ID = {idParameter.ParameterName})";
            Assert.AreEqual(expectedFullQuery, result.CommandText, "O CommandText gerado está incorreto.");
        }

        [TestMethod]
        public void BuildInsertCommand_ShouldCreateCorrectParams()
        {
            const int
                NAME_PARAM_INDEX = 0,
                BIRTH_PARAM_INDEX = 1;

            CommandBuilderOptions options = new()
            {
                AutoGenerateRestriction = false,
                PopulateCommandObject = false
            };

            UsersTestEntity user = TableEntityFactory.CreateInstanceFor<UsersTestEntity>();
            user.Name = "João";
            user.BirthDate = new DateTime(2006, 4, 12);

            CommandBuilder<UsersTestEntity> commandBuilder = new(options, entity: user);
            CommandBuilderResult result = commandBuilder.BuildInsertCommand();

            Assert.AreNotEqual(string.Empty, result.CommandText, "O CommandText gerado não deveria ser vazio.");
            Assert.AreEqual(2, result.Restriction.Parameters.Count, "O número de parâmetros gerados está incorreto.");

            SqlParameter
                nameParameter = result.Restriction.Parameters[NAME_PARAM_INDEX],
                birthParameter = result.Restriction.Parameters[BIRTH_PARAM_INDEX];

            Assert.AreEqual(user.Name, nameParameter.Value, "O valor do parâmetro para NAME está incorreto.");
            Assert.AreEqual(user.BirthDate, birthParameter.Value, "O valor do parâmetro para BIRTHDATE está incorreto.");

            string expectedFullQuery = $"INSERT INTO USERS_TEST (NAME, BIRTHDATE) OUTPUT INSERTED.ID VALUES ({nameParameter.ParameterName}, {birthParameter.ParameterName})";
            Assert.AreEqual(expectedFullQuery, result.CommandText, "O CommandText gerado está incorreto.");
        }

        [TestMethod]
        public void BuildDeleteCommand_ShouldCreateCorrectParams()
        {
            CommandBuilderOptions options = new()
            {
                AutoGenerateRestriction = true,
                PopulateCommandObject = false
            };

            UsersTestEntity user = TableEntityFactory.CreateInstanceFor<UsersTestEntity>();
            user.Id = 1;

            CommandBuilder<UsersTestEntity> commandBuilder = new(options, entity: user);
            CommandBuilderResult result = commandBuilder.BuildDeleteCommand();

            Assert.AreNotEqual(string.Empty, result.CommandText, "O CommandText gerado não deveria ser vazio.");
            Assert.AreEqual(1, result.Restriction.Parameters.Count, "O número de parâmetros gerados está incorreto.");

            SqlParameter idParameter = result.Restriction.Parameters[0];

            Assert.AreEqual(user.Id, idParameter.Value, "O valor do parâmetro para ID está incorreto.");

            string expectedFullQuery = $"DELETE FROM USERS_TEST WHERE (ID = {idParameter.ParameterName})";
            Assert.AreEqual(expectedFullQuery, result.CommandText, "O CommandText gerado está incorreto.");
        }

        [TestMethod]
        public void BuildExistsCommand_ShouldCreateCorrectParams()
        {
            CommandBuilderOptions options = new()
            {
                AutoGenerateRestriction = false,
                PopulateCommandObject = false,
                AppendExistingRestriction = true
            };

            Restriction restriction = new("V.ID = @pID AND UPPER(V.NAME) LIKE @pNAME");
            restriction.Parameters.Add("pID", 1);
            restriction.Parameters.Add("pNAME", "SILVA");

            CommandBuilder<UsersTestEntity> builder = new(options, preRestriction: restriction);
            CommandBuilderResult result = builder.BuildExistsCommand();

            Assert.AreNotEqual(string.Empty, result.CommandText, "O CommandText gerado não deveria ser vazio.");
            Assert.AreEqual(2, result.Restriction.Parameters.Count, "O número de parâmetros gerados está incorreto.");

            SqlParameter
                idParameter = result.Restriction.Parameters[0],
                nameParameter = result.Restriction.Parameters[1];

            Assert.AreEqual(1, idParameter.Value, "O valor do parâmetro para ID está incorreto.");
            Assert.AreEqual("SILVA", nameParameter.Value, "O valor do parâmetro para NAME está incorreto.");

            string expectedFullQuery = $"SELECT CASE WHEN EXISTS(SELECT V.ID FROM USERS_TEST V WHERE ((V.ID = @pID AND UPPER(V.NAME) LIKE @pNAME))) THEN 1 ELSE 0 END";
            Assert.AreEqual(expectedFullQuery, result.CommandText, "O CommandText gerado está incorreto.");
        }

        [TestMethod]
        public void BuildSelectCommand_WhenFetchPredefinedColumnsIsTrue_ShouldAppendPredefinedColumnsOnly()
        {
            CommandBuilderOptions options = new()
            {
                AutoGenerateRestriction = false,
                PopulateCommandObject = false,
                AppendExistingRestriction = false,
                FetchPredefinedColumns = true
            };

            CommandBuilder<UsersTestEntity> builder = new(options, predefinedSelectColumns: ["NAME"]);
            CommandBuilderResult result = builder.BuildSelectCommand();

            string expectedFullQuery = "SELECT NAME FROM USERS_TEST V ";
            Assert.AreEqual(expectedFullQuery, result.CommandText);
        }
    }
}
