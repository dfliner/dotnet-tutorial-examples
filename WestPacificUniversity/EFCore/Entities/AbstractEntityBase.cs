namespace WestPacificUniversity.EFCore.Entities;

public abstract class AbstractEntityBase : IEntity, ISoftDelete
{
    public int Id { get; set; }
    public bool IsDeleted { get; set; }
}
