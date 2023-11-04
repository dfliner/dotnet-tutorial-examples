namespace WestPacificUniversity.EFCore.Entities;

public interface ISoftDelete
{
    bool IsDeleted { get; set; }
}
