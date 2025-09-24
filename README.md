
# ORM Simples para .NET

Um ORM (Object-Relational Mapper) simples e intuitivo para .NET, projetado para facilitar a interação com o banco de dados através de objetos C#.




## Funcionalidades

- Definição de entidades via atributos
- Operações CRUD simplificadas
- Gerenciamento de transações
- Consultas flexíveis  com projeção
- Execução de SQL nativo


## Definindo sua entidade
Para mapear uma tabela do banco de dados, crie uma classe que herde de `TableEntity<T>`. Essa classe já possui o campo ID (chave primária) mapeado.
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

## Chaves estrangeiras
O ORM suporta relacionamentos de chave estrangeira com Lazy Loading automático. Isso significa que a entidade relacionada só é carregada do banco de dados quando sua propriedade de navegação é acessada pela primeira vez.

### Configurando o relacionamento
Para configurar um relacionamento, use o atributo `ForeignKey` em uma propriedade virtual.
```csharp
[TableEntity("CATEGORIAS")]
public class Categoria : TableEntity<Categoria>
{
    [EntityColumnName("DESCRICAO")]
    public string Descricao { get; set; }
}

[TableEntity("PRODUTOS")]
public class Produto : TableEntity<Produto>
{
    [EntityColumnName("NOME")]
    public string Nome { get; set; }

    // Coluna que armazena o ID da categoria
    [EntityColumnName("ID_CATEGORIA")]
    public long? IdCategoria { get; set; }

    // Propriedade de navegação para a Categoria
    // É VIRTUAL para permitir a interceptação do Lazy Loading
    [ForeignKey<Categoria>("ID_CATEGORIA")]
    public virtual Categoria Categoria { get; set; }
}
```

### Uso
O ORM gerencia o relacionamento para você.

Quando você acessa a propriedade Categoria pela primeira vez, o ORM executa um `Find` para buscar a categoria correspondente.

Ao atribuir um objeto à propriedade de navegação, o ORM atualiza automaticamente a propriedade de ID correspondente.

```csharp
var novaCategoria = Categoria.Find(2);
var produto = Produto.New();
produto.Nome = "Novo Produto";

// Atribui a instância da categoria
produto.Categoria = novaCategoria;

// O ORM sincroniza o ID automaticamente
Console.WriteLine(produto.IdCategoria); // Imprime '2'

produto.Save(); // Salva o produto com o ID_CATEGORIA correto
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

## Ciclo de vida das entidades
Você pode executar lógicas customizadas durante o ciclo de vida de uma entidade (antes/depois de inserir, atualizar ou excluir) sobrescrevendo os métodos de hook na sua classe de entidade.

Isso é útil para auditoria, validação ou para disparar outras ações.

```csharp
[TableEntity("ENDERECOS")]
public class Endereco : TableEntity<Endereco>
{
    // ... propriedades ...

    // Chamado antes de um novo registro ser inserido no banco.
    public override void OnBeforeInsert()
    {
        // Exemplo: garantir que o nome da rua seja sempre maiúsculo.
        if (!string.IsNullOrEmpty(this.Rua))
        {
            this.Rua = this.Rua.ToUpper();
        }
    }

    // Chamado antes de um registro ser atualizado.
    public override void OnBeforeUpdate()
    {
        // Lógica de validação ou modificação antes de atualizar.
    }

    // Chamado antes de um registro ser excluído.
    public override void OnBeforeDelete()
    {
        // Exemplo: registrar um log antes da exclusão.
        Console.WriteLine($"Registro de endereço com ID {this.Id} será excluído.");
    }

    // Existem outros hooks disponíveis, como OnAfterInsert, OnAfterUpdate, etc.
}
```
