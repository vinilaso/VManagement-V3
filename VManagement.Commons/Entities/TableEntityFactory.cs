using System.Reflection;
using Castle.DynamicProxy;
using VManagement.Commons.Entities.Attributes;
using VManagement.Commons.Entities.Interfaces;

namespace VManagement.Commons.Entities
{
    /// <summary>
    /// Fornece métodos estáticos de fábrica para criar e inspecionar instâncias de entidades.
    /// </summary>
    /// <remarks>
    /// Esta classe utiliza reflexão (reflection) para analisar os atributos das entidades
    /// e extrair metadados, como os nomes das colunas mapeadas.
    /// </remarks>
    public class TableEntityFactory
    {
        private readonly static ProxyGenerator _generator = new();
        private readonly static List<IInterceptor> _interceptors = [];

        /// <summary>
        /// Cria uma nova instância de proxy para uma entidade, aprimorando-a com as funcionalidades do ORM.
        /// </summary>
        /// <remarks>
        /// Em vez de retornar uma instância simples, este método utiliza <see cref="ProxyGenerator"/> para criar uma classe
        /// proxy em tempo de execução que herda de <typeparamref name="TEntity"/>.
        /// Interceptadores são anexados a este proxy para adicionar comportamento dinâmico às propriedades da entidade.
        /// </remarks>
        /// <typeparam name="TEntity">O tipo da entidade a ser criada, que deve ser uma classe que implementa <see cref="ITableEntity"/> e ter um construtor sem parâmetros.</typeparam>
        /// <returns>Uma instância de proxy de <typeparamref name="TEntity"/>, pronta para o rastreamento de alterações e outras funcionalidades do ORM.</returns>
        public static TEntity CreateInstanceFor<TEntity>() where TEntity : class, ITableEntity, new()
        {
            return _generator.CreateClassProxy<TEntity>([.._interceptors]);
        }

        /// <summary>
        /// Registra um novo tipo de interceptor que será aplicado a todas as instâncias de entidade criadas pela fábrica.
        /// </summary>
        /// <remarks>
        /// Este método permite estender o ORM com novas funcionalidades de "cross-cutting concern" (lógica transversal).
        /// Qualquer classe que implemente <see cref="IInterceptor"/> e tenha um construtor sem parâmetros pode ser registrada.
        /// Uma vez registrado, o interceptor será instanciado e adicionado a cada novo proxy de entidade criado pelo método
        /// <see cref="CreateInstanceFor{TEntity}"/>, aplicando seu comportamento a todas as entidades.
        /// </remarks>
        /// <typeparam name="TInterceptor">O tipo do interceptor a ser registrado. Deve implementar <see cref="IInterceptor"/> e ter um construtor 'new()'.</typeparam>
        public static void UseInterceptor<TInterceptor>() where TInterceptor : IInterceptor, new()
        {
            TInterceptor interceptor = Activator.CreateInstance<TInterceptor>();
            _interceptors.Add(interceptor);
        }

        /// <summary>
        /// Obtém todas as propriedades de uma entidade que estão marcadas com o atributo <see cref="EntityColumnNameAttribute"/>.
        /// </summary>
        /// <typeparam name="TEntity">O tipo da entidade a ser analisada.</typeparam>
        /// <returns>Uma enumeração das propriedades que representam colunas da tabela.</returns>
        public static IEnumerable<PropertyInfo> GetColumnProperties<TEntity>()
        {
            return typeof(TEntity)
                .GetProperties()
                .Where(p => Attribute.IsDefined(p, typeof(EntityColumnNameAttribute)));
        }

        /// <summary>
        /// Obtém os nomes de todas as colunas de uma entidade, com base nas suas propriedades marcadas com o atributo <see cref="EntityColumnNameAttribute"/>.
        /// </summary>
        /// <typeparam name="TEntity">O tipo da entidade a ser analisada.</typeparam>
        /// <returns>Uma enumeração dos nomes das colunas.</returns>
        public static IEnumerable<string> GetColumnNames<TEntity>()
        {
            return GetColumnProperties<TEntity>()
                .Select(prop => prop.GetCustomAttribute<EntityColumnNameAttribute>())
                .Where(attr => attr != null)
                .Select(attr => attr!.ColumnName);
        }
    }
}