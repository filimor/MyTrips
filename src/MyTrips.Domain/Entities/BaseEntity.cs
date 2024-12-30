using System.Text.Json;
using System.Text.Json.Serialization;

namespace MyTrips.Domain.Entities;

public abstract class BaseEntity
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    protected BaseEntity()
    {
    }

    protected BaseEntity(int id)
    {
        Id = id;
    }

    public int Id { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj is null || GetType() != obj.GetType()) return false;

        var other = (BaseEntity)obj;

        return Id == other.Id;
    }

    public virtual bool Equals(BaseEntity? other)
    {
        return other is not null && Id == other.Id;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this, JsonOptions);
    }

    public static bool operator ==(BaseEntity? a, BaseEntity? b)
    {
        if (a is null && b is null) return true;
        if (a is null || b is null) return false;
        return a.Equals(b);
    }

    public static bool operator !=(BaseEntity? a, BaseEntity? b)
    {
        return !(a == b);
    }
}