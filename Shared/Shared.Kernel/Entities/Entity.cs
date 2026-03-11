using System.ComponentModel.DataAnnotations;

namespace Qwestr.Shared.Kernel.Entities;

public abstract class Entity<TId>
{
    [Key]
    public TId Id { get; protected set; }
}