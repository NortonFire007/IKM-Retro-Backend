using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IKM_Retro.Enums;
using IKM_Retro.Models.Base;

namespace IKM_Retro.Models.Retro;

[Table("RetrospectiveGroupItems")]
public class GroupItem : Auditable<int>
{
    [ForeignKey("Group")] 
    public int GroupId { get; set; }
    
    [Required, MaxLength(250)] 
    public string? Description { get; set; }

    [ForeignKey("User"), MaxLength(128)]
    public string? UserId { get; set; } 
    
    public DateTime? DueDate { get; set; }

    [Required, MaxLength(20)] 
    public GroupItemStatusEnum Status { get; set; } = GroupItemStatusEnum.Open; 

    public int OrderPosition { get; set; }

    public bool IsHidden { get; set; } = false; 

    public required ICollection<Comment> Comments { get; set; }
    
    public required Group Group { get; set; }
    
    public required User AssignedUser { get; set; }
}