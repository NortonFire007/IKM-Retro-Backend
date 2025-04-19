using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IKM_Retro.Models.Base;


namespace IKM_Retro.Models.Retro;

[Table("CommentVotes")]
public class CommentVote : Auditable<int>
{
    [ForeignKey("Comment")] 
    public int CommentId { get; set; }

    [ForeignKey("User"), MaxLength(128)]
    public string? UserId { get; set; } 

    public User User { get; set; } = null!;
    
    public Comment Comment { get; set; } = null!;
}