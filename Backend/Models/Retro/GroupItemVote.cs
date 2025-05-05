using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IKM_Retro.Models.Base;

namespace IKM_Retro.Models.Retro;

[Table("GroupItemVotes")]
public class GroupItemVote : Auditable<int>
{
    [ForeignKey("GroupItem")]
    public int GroupItemId { get; set; }

    [ForeignKey("User"), MaxLength(128)]
    public string? UserId { get; set; }

    public int Count { get; set; }

    public GroupItem GroupItem { get; set; } = null!;
    public User User { get; set; } = null!;
}
