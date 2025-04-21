using System.ComponentModel.DataAnnotations;

namespace IKM_Retro.Models.Base;

public abstract class Identity<T>
{
    [Key]
    public T ?Id { get; set; }
}