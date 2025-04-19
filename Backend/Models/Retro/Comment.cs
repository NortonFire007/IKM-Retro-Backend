using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IKM_Retro.Models.Base;

namespace IKM_Retro.Models.Retro;

[Table("Comments")]
public class Comment : Auditable<int>
{
    [ForeignKey("GroupItem")]
    public int GroupItemId { get; set; }

    public GroupItem GroupItem { get; set; } = null!;

    [ForeignKey("User"), MaxLength(128)]
    public string? UserId { get; set; } 
    
    [Required, MaxLength(250)]
    public required string Content { get; set; }

    public int Likes { get; set; }

    public bool IsAnonymous { get; set; }

    public ICollection<CommentVote> CommentVotes { get; set; } = new List<CommentVote>();
    
    public User? User { get; set; }
}