namespace IKM_Retro.Models.Base;

public class Auditable<T> : Identity<T>
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}