using WestPacificUniversity.EFCore.Entities;

namespace WestPacificUniversity.EFCore.Dto;

public abstract class EntityDto<TEntity> where TEntity : AbstractEntityBase
{
    public int Id { get; set; }

    public abstract TEntity ToEntity();
}
