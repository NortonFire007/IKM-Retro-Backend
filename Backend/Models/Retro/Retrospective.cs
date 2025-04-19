using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IKM_Retro.Enums;
using IKM_Retro.Models.Base;

namespace IKM_Retro.Models.Retro;

[Table("Retrospectives")]
public class Retrospective : Auditable<Guid>
{
    [Required, MaxLength(255)] 
    public required string Title { get; set; } = "Retrospective";

    public DateTime StartDate { get; set; } = DateTime.UtcNow;

    public DateTime? EndDate { get; set; }

    public bool IsActive { get; set; } = true;
    
    [Required]
    public required TemplateTypeEnum Template { get; set; }

    [Required]
    public string? InviteLink { get; set; } 

    public int OwnerId { get; set; } 
    
    public ICollection<RetrospectiveToUser> RetrospectiveUsers { get; set; } = new List<RetrospectiveToUser>();

    public ICollection<Group> Groups { get; set; } = new List<Group>();
}