using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IKM_Retro.Models.Base;

namespace IKM_Retro.Models.Retro;

[Table("RetrospectiveInvites")]
public class RetrospectiveInvite : Auditable<Guid>
{
    [ForeignKey("Retrospective")]
    public Guid RetrospectiveId { get; set; }
    
    [Required, MaxLength(100)]
    public string Code { get; set; } = Guid.NewGuid().ToString();

    public DateTime? ExpiresAt { get; set; } = DateTime.UtcNow.AddDays(7); 

    public bool IsActive { get; set; } = true;
    
    public Retrospective Retrospective { get; set; } = null!;
}