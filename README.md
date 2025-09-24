
# ORM Simples para .NET

Um ORM (Object-Relational Mapper) simples e intuitivo para .NET, projetado para facilitar a interação com o banco de dados através de objetos C#.




## Funcionalidades

- Definição de entidades via atributos
- Operações CRUD simplificadas
- Gerenciamento de transações
- Consultas flexíveis  com projeção
- Execução de SQL nativo


## Definindo sua entidade
Para mapear uma tabela do banco de dados, crie uma classe que herde de `TableEntity<T>`.
Use os seguintes atributos para configurar o mapeamento:

`[TableEntity("NOME_DA_TABELA")]`: Define o nome da tabela que a classe representa.

`[EntityColumnName("NOME_DA_COLUNA")]`: Mapeia uma propriedade da classe para uma coluna específica na tabela.
```csharp
public class Endereco : TableEntity<Endereco>
{
    [TableEntity("ENDERECOS")]
    public class Endereco : TableEntity<Endereco>
    {
        [EntityColumnName("RUA")]
        public string? Rua { get; set; }

        [EntityColumnName("NUMERO")]
        public int? Numero { get; set; }

        [EntityColumnName("COMPLEMENTO")]
        public string? Complemento { get; set; }
    }
}
```


## Utilizando o Sistema
É uma boa prática encapsular todas as operações de banco de dados em uma transação para garantir a atomicidade e consistência dos dados.

```csharp
using VManagementTransaction transaction = new();

// ... seu código de manipulação de dados aqui ...

// Confirma todas as alterações no banco de dados ao final do bloco using
transaction.Complete();
```

### Inserindo registros
Para criar um novo registro, utilize o método estático `.New()`, defina os valores das propriedades e chame `.Save()`.

```csharp
// Inclusão de registros
var novoEndereco = Endereco.New();
novoEndereco.Numero = 102;
novoEndereco.Rua = "Rua Água Verde";
novoEndereco.Complemento = "Complemento";
novoEndereco.Save();
```
### Atualizando registros
Para atualizar um registro, primeiro recupere-o com o método `.Find(id)`, modifique as propriedades desejadas e, em seguida, chame `.Save()`.

```csharp
// Atualização de registros
var enderecoJaExistente = Endereco.Find(1);
if (enderecoJaExistente is not null)
{
    enderecoJaExistente.Complemento += " - Alterado";
    enderecoJaExistente.Save();
}
```

### Selecionando registros
A biblioteca oferece métodos flexíveis para consultar dados.

#### Busca por restrição
Use `FindFirstOrDefault` com uma `Restriction` para buscar um único registro completo. 

```csharp
var restricao = new Restriction("UPPER(V.RUA) LIKE '%WERNER%'");
var endereco = Endereco.FindFirstOrDefault(restricao);
```

#### Seleção com projeção de dados
Para melhorar a performance, você pode selecionar apenas as colunas que precisa. Isso retorna um objeto anônimo contendo somente as propriedades especificadas.

```csharp
// Retorna um objeto anônimo contendo apenas Id e Rua do endereço com Id = 16
var enderecoProjetado = Endereco.FindFirstOrDefault(p => new { p.Id, p.Rua }, Restriction.FromId(16));
```

## Executando queries SQL nativas
É possível executar comandos SQL puros e converter os resultados diretamente para uma lista de objetos fortemente tipados.
Esses objetos não precisam ser entidades mapeadas.

Primeiro, defina uma classe simples para receber os dados:
```csharp
public class EnderecoSimples
{
    public int Id { get; set; }
    public string Rua { get; set; }
}
```

Em seguida, execute a query e mapeie o resultado usando `ParseManyTo<T>()`:
```csharp
// Conversão de queries para tipos de registro
var command = new RawSqlCommand("SELECT ID, RUA FROM ENDERECOS");
var enderecosSimples = command.ParseManyTo<EnderecoSimples>();
```
