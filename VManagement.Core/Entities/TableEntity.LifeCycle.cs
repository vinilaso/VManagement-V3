using VManagement.Core.Exceptions;
using VManagement.Commons.Utility.Extensions;

namespace VManagement.Core.Entities
{
    public abstract partial class TableEntity<TEntity>
    {
        #region >> Triggers

        /// <summary>
        /// Persiste o estado atual da entidade no banco de dados, decidindo automaticamente entre uma operação de INSERT ou UPDATE.
        /// Esta é a principal forma de salvar uma entidade. A operação só é permitida se o estado da entidade for 'New' ou 'Loaded'.
        /// </summary>
        /// <remarks>
        /// O método primeiro dispara o evento 'OnBeforeSave'. Em seguida, ele verifica o <see cref="State"/> da entidade:
        /// <list type="bullet">
        /// <item>Se o estado for <see cref="EntityState.New"/>, a entidade será inserida como um novo registro.</item>
        /// <item>Se o estado for <see cref="EntityState.Loaded"/>, a entidade será atualizada.</item>
        /// </list>
        /// Ao final, o evento 'OnAfterSave' é disparado.
        /// </remarks>
        /// <exception cref="InvalidEntityActionException">Lançada se o método for chamado quando o estado da entidade não for 'New' ou 'Loaded'.</exception>
        public void Save()
        {
            OnBeforeSave();

            if (State == EntityState.New)
            {
                Insert();
            }
            else if (State == EntityState.Loaded)
            {
                Update();
            }

            OnAfterSave();
        }

        /// <summary>
        /// Persiste a nova entidade no banco de dados, com uma operação de INSERT.
        /// A operação só é permitida se o estado da entidade for 'New'.
        /// </summary>
        /// <remarks>
        /// O método primeiro dispara o evento <see cref="OnBeforeInsert"/>.
        /// Ao final, o evento <see cref="OnAfterSave"/> é disparado.
        /// </remarks>
        /// <exception cref="InvalidEntityActionException">Lançada se o método for chamado quando o estado da entidade não for 'New'.</exception>
        protected void Insert()
        {
            OnBeforeInsert();

            Id = _dao.Insert((TEntity)this);

            OnAfterInsert();
        }

        /// <summary>
        /// Atualiza uma entidade no banco de dados, com uma operação de UPDATE.
        /// A operação só é permitida se o estado da entidade for 'Loaded'.
        /// </summary>
        /// <remarks>
        /// O método primeiro dispara o evento <see cref="OnBeforeUpdate"/>.
        /// Ao final, o evento <see cref="OnAfterUpdate"/> é disparado.
        /// </remarks>
        /// <exception cref="InvalidEntityActionException">Lançada se o método for chamado quando o estado da entidade não for 'Loaded'.</exception>
        protected void Update()
        {
            OnBeforeUpdate();

            _dao.Update((TEntity)this);

            OnAfterUpdate();
        }

        /// <summary>
        /// Exclui uma entidade no banco de dados, com uma operação de DELETE.
        /// A operação só é permitida se o estado da entidade for 'Loaded'.
        /// </summary>
        /// <remarks>
        /// O método primeiro dispara o evento <see cref="OnBeforeDelete"/>.
        /// Ao final, o evento <see cref="OnAfterDelete"/> é disparado.
        /// </remarks>
        /// <exception cref="InvalidEntityActionException">Lançada se o método for chamado quando o estado da entidade não for 'Loaded'.</exception>
        public void Delete()
        {
            OnBeforeDelete();

            _dao.Delete((TEntity)this);

            OnAfterDelete();
        }

        #endregion >> Triggers

        #region >> Template methods

        /// <summary>
        /// TEMPLATE METHOD: Define o algoritmo a ser executado após uma nova instância ser criada em memória.
        /// </summary>
        protected void OnAfterCreated()
        {
            SetOriginalInstance((TEntity)MemberwiseClone());

            UpdateEntityState(EntityState.New);

            OnAfterCreatedCore();
        }

        /// <summary>
        /// TEMPLATE METHOD: Define o algoritmo a ser executado antes de uma operação de INSERT.
        /// </summary>
        /// <remarks>
        /// Este método garante que a entidade esteja no estado 'New' antes de prosseguir.
        /// Após a validação, ele chama o método <see cref="OnBeforeInsertCore"/>, que pode ser
        /// sobrescrito para adicionar lógica de negócio customizada.
        /// </remarks>
        /// <exception cref="InvalidEntityActionException">Lançada se o estado da entidade não for 'New'.</exception>
        protected void OnBeforeInsert()
        {
            ValidateEntityState("Insert", EntityState.New);

            OnBeforeInsertCore();
        }

        /// <summary>
        /// TEMPLATE METHOD: Define o algoritmo a ser executado imediatamente após uma operação de INSERT bem-sucedida.
        /// </summary>
        /// <remarks>
        /// Este método é responsável por duas tarefas críticas da classe base:
        /// <list type="number">
        /// <item>Atualiza o estado da entidade para <see cref="EntityState.Loaded"/>.</item>
        /// <item>Reseta o estado de 'Changed' de todos os campos rastreados, considerando a entidade sincronizada com o banco.</item>
        /// <item>Ao final, chama o método <see cref="OnAfterInsertCore"/> para permitir a execução de lógica customizada.</item>
        /// </list>
        /// </remarks>
        protected void OnAfterInsert()
        {
            UpdateEntityState(EntityState.Loaded);

            OnAfterInsertCore();
        }

        /// <summary>
        /// TEMPLATE METHOD: Define o algoritmo a ser executado antes de uma operação de UPDATE.
        /// </summary>
        protected void OnBeforeUpdate()
        {
            ValidateEntityState("Update", EntityState.Loaded);

            OnBeforeUpdateCore();
        }

        /// <summary>
        /// TEMPLATE METHOD: Define o algoritmo a ser executado antes da operação de Save.
        /// </summary>
        protected void OnBeforeSave()
        {
            ValidateEntityState("Save", EntityState.New, EntityState.Loaded);

            OnBeforeSaveCore();
        }

        /// <summary>
        /// TEMPLATE METHOD: Define o algoritmo a ser executado após uma operação de Save.
        /// </summary>
        protected void OnAfterSave()
        {
            AcceptChanges();

            OnAfterSaveCore();
        }

        /// <summary>
        /// TEMPLATE METHOD: Define o algoritmo a ser executado após uma operação de UPDATE.
        /// </summary>
        protected void OnAfterUpdate()
        {
            UpdateEntityState(EntityState.Loaded);

            OnAfterUpdateCore();
        }

        /// <summary>
        /// TEMPLATE METHOD: Define o algoritmo a ser executado antes de uma operação de DELETE.
        /// </summary>
        protected void OnBeforeDelete()
        {
            ValidateEntityState("Delete", EntityState.Loaded);

            OnBeforeDeleteCore();
        }

        /// <summary>
        /// TEMPLATE METHOD: Define o algoritmo a ser executado após uma operação de DELETE.
        /// </summary>
        protected void OnAfterDelete()
        {
            UpdateEntityState(EntityState.Deleted);

            OnAfterDeleteCore();
        }

        /// <summary>
        /// TEMPLATE METHOD: Define o algoritmo a ser executado após a entidade ser carregada do banco de dados.
        /// </summary>
        protected void OnAfterGet()
        {
            SetOriginalInstance((TEntity)MemberwiseClone());

            UpdateEntityState(EntityState.Loaded);

            OnAfterGetCore();
        }

        #endregion >> Template methods

        #region >> Core hooks

        /// <summary>
        /// HOOK: Ponto de extensão para adicionar lógica customizada antes da operação de Save. Executado antes de Insert ou Update.
        /// </summary>
        protected virtual void OnBeforeSaveCore()
        { }

        /// <summary>
        /// HOOK: Ponto de extensão para adicionar lógica customizada antes de uma operação de INSERT.
        /// </summary>
        protected virtual void OnBeforeInsertCore()
        { }

        /// <summary>
        /// HOOK: Ponto de extensão para adicionar lógica customizada após uma operação de INSERT bem-sucedida.
        /// </summary>
        protected virtual void OnAfterInsertCore()
        { }

        /// <summary>
        /// HOOK: Ponto de extensão para adicionar lógica customizada antes de uma operação de UPDATE.
        /// </summary>
        protected virtual void OnBeforeUpdateCore()
        { }

        /// <summary>
        /// HOOK: Ponto de extensão para adicionar lógica customizada após a operação de Save ser concluída.
        /// </summary>
        protected virtual void OnAfterSaveCore()
        { }

        /// <summary>
        /// HOOK: Ponto de extensão para adicionar lógica customizada após uma operação de UPDATE bem-sucedida.
        /// </summary>
        protected virtual void OnAfterUpdateCore()
        { }

        /// <summary>
        /// HOOK: Ponto de extensão para adicionar lógica customizada antes de uma operação de DELETE.
        /// </summary>
        protected virtual void OnBeforeDeleteCore()
        { }

        /// <summary>
        /// HOOK: Ponto de extensão para adicionar lógica customizada após uma operação de DELETE bem-sucedida.
        /// </summary>
        protected virtual void OnAfterDeleteCore()
        { }

        /// <summary>
        /// HOOK: Ponto de extensão para adicionar lógica customizada após uma entidade ser carregada do banco de dados.
        /// </summary>
        protected virtual void OnAfterGetCore()
        { }

        /// <summary>
        /// HOOK: Ponto de extensão para adicionar lógica customizada após uma nova instância ser criada em memória.
        /// </summary>
        protected virtual void OnAfterCreatedCore()
        { }

        #endregion >> Core hooks

        #region >> Private methods

        private void ValidateEntityState(string action, params EntityState[] expectedStates)
        {
            if (expectedStates.Length == 0)
                return;

            if (!State.In(expectedStates))
                throw new InvalidEntityActionException(State, action);
        }

        private void UpdateEntityState(EntityState state) => State = state;

        #endregion >> Private methods
    }
}
