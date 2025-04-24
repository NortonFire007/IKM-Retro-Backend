namespace IKM_Retro.DTOs.Base;

public class AuditableDTO<T>: IdentityDTO<T>
{
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set;}
}