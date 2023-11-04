
using Microsoft.CSharp;
using System.CodeDom;
using System.Runtime.Serialization;
using WestPacificUniversity.Utilities;

namespace WestPacificUniversity.EFCore.Entities;

[Serializable]
public class EntityNotFoundException : ApplicationException, ISerializable
{
    private const int InvalidId = -1;

    private static string GetFriendlyTypeName(Type type)
    {
        CheckArgument.ThrowIfNull(type, nameof(type));

        using var compiler = new CSharpCodeProvider();
        var typeRef = new CodeTypeReference(type);
        return compiler.GetTypeOutput(typeRef);
    }

    public Type EntityType { get; set; } = default!;
    public int EntityId { get; set; } = InvalidId;

    public EntityNotFoundException() : base()
    {
    }

    public EntityNotFoundException(string? message) : base(message)
    {
    }

    public EntityNotFoundException(string? message, Exception innerException) : base(message, innerException)
    {
    }

    public EntityNotFoundException(Type entityType, int entityId)
        : this(entityType, entityId, null)
    {
    }

    public EntityNotFoundException(Type entityType, int entityId, Exception? innerException)
        : base($"Entity not found. Entity Type: {GetFriendlyTypeName(entityType)}, Entity Id: {entityId}", innerException)
    {
        EntityType = entityType;
        EntityId = entityId;
    }

    protected EntityNotFoundException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        if (info != null)
        {
            EntityType = (Type)info.GetValue(nameof(EntityType), typeof(Type))!;
            EntityId = info.GetInt32(nameof(EntityId));
        }
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);

        if (info != null)
        {
            info.AddValue(nameof(EntityType), EntityType);
            info.AddValue(nameof(EntityId), EntityId);
        }
    }
}
