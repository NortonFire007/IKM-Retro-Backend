using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IKM_Retro.Models.Base;

namespace IKM_Retro.Models.Retro;

public class ActionItemComment : Auditable<int>
{
    [ForeignKey("ActionItem")]
    public int ActionItemId { get; set; }
    
    [ForeignKey("User"), MaxLength(128)]
    public string? UserId { get; set; } 
    
    [Required, MaxLength(250)]
    public required string Content { get; set; }
    
    public ActionItem ActionItem { get; set; } = null!;
    public User? User { get; set; }
}