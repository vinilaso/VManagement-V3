using Microsoft.Data.SqlClient;
using VManagement.Commons.Entities;
using VManagement.Database.Command;
using VManagement.Database.Exceptions;
using VManagement.Database.Tests.TestEntities;

namespace VManagement.Database.Tests.Command
{
    [TestClass]
    public class CommandBuilderTests
    {
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

            string expectedFullQuery = $"SELECT ID, NAME, BIRTHDATE FROM USERS_TEST V WHERE (V.ID = {mockCommand.Parameters[0].ParameterName})";
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

            string expectedFullQuery = "SELECT ID, NAME, BIRTHDATE FROM USERS_TEST V ";

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

            UsersTestEntity user = TableEntityFactory.CreateInstanceFor<UsersTestEntity>();
            user.Id = 1;
            user.Name = "João";

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
    }
}
