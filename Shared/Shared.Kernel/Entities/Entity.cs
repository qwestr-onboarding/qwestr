using System.ComponentModel.DataAnnotations;

namespace Shared.Kernel.Entities;

public abstract class Entity<TId>
{
    [Key]
    public TId Id { get; protected set; }
}