using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IKM_Retro.Enums;
using IKM_Retro.Models.Base;

namespace IKM_Retro.Models.Retro;

[Table("RetrospectiveToUsers")]
public class RetrospectiveToUser : Auditable<int>
{
    [ForeignKey("Retrospective")] 
    public Guid RetrospectiveId { get; set; }

    public required Retrospective Retrospective { get; set; }

    [ForeignKey("User"), MaxLength(128)]
    public string? UserId { get; set; } 

    public RoleTypeEnum Role { get; set; }
    
    public required User User { get; set; }
}