namespace VManagement.Commons.Entities.Interfaces
{
    public interface ITrackedFieldCollection : ICollection<ITrackedField>
    {
        ITrackedField this[string fieldName] { get; }
    }
}
